/* Controller.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * source distribution.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;

using Gdk;
using Mono.Unix;

using Do;
using Do.UI;
using Do.Universe;
using Do.DBusLib;
using Do.Platform;
using Do.Interface;

namespace Do.Core {

	public class Controller : IController, IDoController {
		
		const int SearchDelay = 250;

		protected IDoWindow window;
		protected Gtk.Window addin_window;
		protected Gtk.AboutDialog about_window;
		protected PreferencesWindow prefs_window;
		protected ISearchController [] controllers;
		
		Act action;
		List<Item> items;
		List<Item> modItems;
		bool thirdPaneVisible;
		bool results_grown;
		Gtk.IMContext im;
		string last_theme;
		
		public Controller ()
		{
			im = new Gtk.IMMulticontext ();
			items = new List<Item> ();
			modItems = new List<Item> ();
			results_grown = false;
			last_theme = "";
			
			controllers = new SimpleSearchController [3];
			
			// Each controller needs to be aware of the controllers before it.
			// Going down the line however is not needed at current.
			controllers [0] = new FirstSearchController  ();
			controllers [1] = new SecondSearchController (controllers [0]);
			controllers [2] = new ThirdSearchController  (controllers [0], controllers [1]);
			
			// We want to show a blank box during our searches
			// controllers [0].SearchStarted += (u) => { };
			controllers [1].SearchStarted += (u) => {
				if (u && !ControllerExplicitTextMode (Pane.Second))
					window.ClearPane (Pane.Second); 
			};

			controllers [2].SearchStarted += (u) => { 
				if (u && !ControllerExplicitTextMode (Pane.Third))
					window.ClearPane (Pane.Third); 
			};
			
			controllers [0].SearchFinished +=
				(o, state) => SearchFinished (o, state, Pane.First);
			controllers [1].SearchFinished +=
				(o, state) => SearchFinished (o, state, Pane.Second);
			controllers [2].SearchFinished +=
				(o, state) => SearchFinished (o, state, Pane.Third);
			
			im.UsePreedit = false;
			im.Commit += OnIMCommit;
			im.FocusIn ();
		}
		
		private void OnIMCommit (object o, Gtk.CommitArgs args)
		{
			foreach (char c in args.Str.ToCharArray ())
				SearchController.AddChar (c);
			
			// Horrible hack: The reason this exists and exists here is to update the
			// clipboard in a place that we know will always be safe for GTK.
			// Unfortunately due to the way we have designed Do, this has proven
			// extremely difficult to put some place more logical.  We NEED to
			// rethink how we handle Summon () and audit our usage of
			// Gdk.Threads.Enter ()
			if (SearchController.Query.Length <= 1)
				SelectedTextItem.UpdateText ();
		}

		public void Initialize ()
		{
			OnThemeChanged (this, null);
			Do.Preferences.ThemeChanged += OnThemeChanged;
		}
		
		void OnThemeChanged (object sender, PreferencesChangedEventArgs e)
		{
			// If the current theme is already loaded, return.
			if (window != null && last_theme == Do.Preferences.Theme)
				return;

			if (window != null) {
				Vanish ();
				window.KeyPressEvent -= KeyPressWrap;
				window.Dispose ();
				window = null;
			}
			
			Orientation = ControlOrientation.Vertical;

			if (Screen.Default.IsComposited) {
				window = InterfaceManager.MaybeGetInterfaceNamed (Do.Preferences.Theme)
					?? new ClassicWindow ();
			} else {
				window = new ClassicWindow ();
			}
			
			window.Initialize (this);
			window.KeyPressEvent += KeyPressWrap;
			if (window is Gtk.Window)
				(window as Gtk.Window).Title = "Do";
			
			Reset ();
			last_theme = Do.Preferences.Theme;
		}

		bool IsSummonable {
			get { return prefs_window == null && about_window == null; }
		}

		/// <value>
		/// Convenience Method
		/// </value>
		ISearchController SearchController {
			get { return controllers [(int) CurrentPane]; }
		}
		
		/// <value>
		/// The currently active pane, setting does not imply searching
		/// currently
		/// </value>
		public Pane CurrentPane {
			set {
				// If we have no results, we can't go to the second pane.
				if (window.CurrentPane == Pane.First &&
					!SearchController.Results.Any ())
					return;
				
				switch (value) {
				case Pane.First:
					window.CurrentPane = Pane.First;
					break;
				case Pane.Second:
					window.CurrentPane = Pane.Second;
					break;
				case Pane.Third:
					if (ThirdPaneAllowed)
						window.CurrentPane = Pane.Third;
					break;
				}

				// Determine if third pane needed
				if (!ThirdPaneAllowed || ThirdPaneCanClose)
					ThirdPaneVisible = false;
				
				if ((ThirdPaneAllowed && window.CurrentPane == Pane.Third) || ThirdPaneRequired)
					ThirdPaneVisible = true;
			}
			
			get { return window.CurrentPane; }
		}
		
		/// <value>
		/// Check if First Controller is in a reset state
		/// </value>
		bool FirstControllerIsReset {
			get {
				return string.IsNullOrEmpty (controllers [0].Query) &&
					!controllers [0].Results.Any ();
			}
		}
		
		/// <summary>
		/// Sets/Unsets third pane visibility if possible.
		/// </summary>
		bool ThirdPaneVisible {
			set {
				if (value == thirdPaneVisible)
					return;
				
				if (value)
					window.Grow ();
				else
					window.Shrink ();
				thirdPaneVisible = value;
			}
			
			get {
				return thirdPaneVisible;
			}
		}
		
		/// <value>
		/// Check if the third pane is capable of closing.  When actions require the third pane, this will
		/// return false
		/// </value>
		bool ThirdPaneCanClose {
			get {
				return (!ThirdPaneRequired &&
				        controllers [2].Cursor == 0 && 
				        string.IsNullOrEmpty (controllers [2].Query) && 
				        !ControllerExplicitTextMode (Pane.Third));
			}
		}
		
		/// <summary>
		/// Determine if the third pane is allowed.  If allowed, tabbing will result in
		/// third pane opening when tabbing from the second pane.
		/// </summary>
		bool ThirdPaneAllowed {
			get {
				Element first, second;
				Act action;

				first = GetSelection (Pane.First);
				second = GetSelection (Pane.Second);
				action = first as Act ?? second as Act;
				return action != null &&
					action.SupportedModifierItemTypes.Any () &&
					controllers [1].Results.Any ();
			}
		}

		/// <value>
		/// Third pane required states that the current controller state requires that the third pane 
		/// be visible
		/// </value>
		bool ThirdPaneRequired {
			get {
				Element first, second;
				Act action;
				Item item;

				first = GetSelection (Pane.First);
				second = GetSelection (Pane.Second);
				action = first as Act ?? second as Act;
				item = first as Item ?? second as Item;
				return action != null && item != null &&
					action.SupportedModifierItemTypes.Any () &&
					!action.ModifierItemsOptional &&
					controllers [1].Results.Any ();
			}
		}
		
		bool AlwaysShowResults {
			get { return Do.Preferences.AlwaysShowResults || !window.ResultsCanHide; }
		}

		/// <value>
		/// Check if the symbol window is currently visible to the user
		/// </value>
		public bool IsSummoned {
			get {
				return null != window && window.Visible;
			}
		}

		internal PreferencesWindow PreferencesWindow {
			get { return prefs_window; }
		}
		
		/// <summary>
		/// Summons a window with elements in it... seems to work
		/// </summary>
		/// <param name="elements">
		/// A <see cref="Element"/>
		/// </param>
		public void SummonWithElements (IEnumerable<Element> elements)
		{
			if (!IsSummonable) return;
			
			Reset ();
			
			Summon ();
			
			//Someone is going to need to explain this to me -- Now with less stupid!
			controllers [0].Results = elements.ToList ();

			// If there are multiple results, show results window after a short
			// delay.
			if (elements.Any ()) {
				GLib.Timeout.Add (50,
					delegate {
//						Gdk.Threads.Enter ();
						GrowResults ();
//						Gdk.Threads.Leave ();
						return false;
					}
				);
			}
		}
		
		/// <summary>
		/// Determines if the user has requested Text Mode explicitly, even if he has finalized that input
		/// </summary>
		/// <param name="pane">
		/// The <see cref="Pane"/> for which you wish to check
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public bool ControllerExplicitTextMode (Pane pane) {
			return controllers [(int) pane].TextType == TextModeType.Explicit ||
				controllers [(int) pane].TextType == TextModeType.ExplicitFinalized;
		}
		
#region KeyPress Handling
		Gdk.Key UpKey    { get { return (Orientation == ControlOrientation.Vertical) ? Gdk.Key.Up    : Gdk.Key.Left;  } }
		Gdk.Key DownKey  { get { return (Orientation == ControlOrientation.Vertical) ? Gdk.Key.Down  : Gdk.Key.Right; } }
		Gdk.Key LeftKey  { get { return (Orientation == ControlOrientation.Vertical) ? Gdk.Key.Left  : Gdk.Key.Up;    } }
		Gdk.Key RightKey { get { return (Orientation == ControlOrientation.Vertical) ? Gdk.Key.Right : Gdk.Key.Down;  } }
		
		private void KeyPressWrap (Gdk.EventKey evnt)
		{
			// User set keybindings
			if (KeyEventToString (evnt).Equals (Do.Preferences.SummonKeybinding)) {
				OnSummonKeyPressEvent (evnt);
				return;
			} 
			
			if (KeyEventToString (evnt).Equals (Do.Preferences.TextModeKeybinding)) {
				OnTextModePressEvent (evnt);
				return;
			}
			
			// Check for paste
			if ((evnt.State & ModifierType.ControlMask) != 0) {
				if (evnt.Key == Key.v) {
					OnPasteEvent ();
					return;
				}
				if (evnt.Key == Key.c) {
					OnCopyEvent ();
					return;
				}
			}

			if ((Gdk.Key) evnt.KeyValue == Gdk.Key.Escape) {
				OnEscapeKeyPressEvent (evnt);
			} else if ((Gdk.Key) evnt.KeyValue == Gdk.Key.Return ||
			           (Gdk.Key) evnt.KeyValue == Gdk.Key.ISO_Enter ||
			           (Gdk.Key) evnt.KeyValue == Gdk.Key.KP_Enter) {
				OnActivateKeyPressEvent (evnt);
			} else if ((Gdk.Key) evnt.KeyValue == Gdk.Key.Delete ||
			           (Gdk.Key) evnt.KeyValue == Gdk.Key.BackSpace) {
				OnDeleteKeyPressEvent (evnt);
			} else if ((Gdk.Key) evnt.KeyValue == Gdk.Key.Tab ||
			           (Gdk.Key) evnt.KeyValue == Gdk.Key.ISO_Left_Tab) {
				OnTabKeyPressEvent (evnt);
			} else if ((Gdk.Key) evnt.KeyValue == UpKey ||
			           (Gdk.Key) evnt.KeyValue == DownKey ||
			           (Gdk.Key) evnt.KeyValue == Gdk.Key.Home ||
			           (Gdk.Key) evnt.KeyValue == Gdk.Key.End ||
			           (Gdk.Key) evnt.KeyValue == Gdk.Key.Page_Up ||
			           (Gdk.Key) evnt.KeyValue == Gdk.Key.Page_Down) {
				OnUpDownKeyPressEvent (evnt);
			} else if ((Gdk.Key) evnt.KeyValue == RightKey ||
			           (Gdk.Key) evnt.KeyValue == LeftKey) {
				OnRightLeftKeyPressEvent (evnt);
			} else if ((Gdk.Key) evnt.KeyValue == Gdk.Key.comma) {
				OnSelectionKeyPressEvent (evnt);
			} else {
				OnInputKeyPressEvent (evnt);
			}
		}
		
		void OnPasteEvent ()
		{
			Gtk.Clipboard clip = Gtk.Clipboard.Get (Gdk.Selection.Clipboard);
			if (!clip.WaitIsTextAvailable ()) {
				return;
			}
			string str = clip.WaitForText ();
			SearchController.SetString (SearchController.Query + str);
		}
		
		void OnCopyEvent ()
		{
			Gtk.Clipboard clip = Gtk.Clipboard.Get (Gdk.Selection.Clipboard);
			if (SearchController.Selection != null)
				clip.Text = SearchController.Selection.Name;
		}
		
		void OnActivateKeyPressEvent (EventKey evnt)
		{
			im.Reset ();
			if (SearchController.TextType == TextModeType.Explicit) {
				OnInputKeyPressEvent (evnt);
				return;
			}
			bool shift_pressed = (evnt.State & ModifierType.ShiftMask) != 0;
			PerformAction (!shift_pressed);
		}
		
		/// <summary>
		/// This will set a secondary cursor unless we are operating on a text item, in which case we
		/// pass the event to the input key handler
		/// </summary>
		/// <param name="evnt">
		/// A <see cref="EventKey"/>
		/// </param>
		void OnSelectionKeyPressEvent (EventKey evnt)
		{
			im.Reset ();
			if (SearchController.Selection is ITextItem || !results_grown)
				OnInputKeyPressEvent (evnt);
			else if (SearchController.ToggleSecondaryCursor (SearchController.Cursor))
				UpdatePane (CurrentPane);
		}
		
		void OnDeleteKeyPressEvent (EventKey evnt)
		{
			im.Reset ();
			SearchController.DeleteChar ();
		}
		
		void OnSummonKeyPressEvent (EventKey evnt)
		{
			Reset ();
			Vanish ();
		}
		
		void OnEscapeKeyPressEvent (EventKey evnt)
		{
			im.Reset ();
			if (SearchController.TextType == TextModeType.Explicit) {
				if (SearchController.Query.Length > 0)
					SearchController.FinalizeTextMode ();
				else 
					SearchController.TextMode = false;
				UpdatePane (CurrentPane);
				return;
			}
			
			bool results, something_typed;

			something_typed = SearchController.Query.Length > 0;
			results = SearchController.Results.Any ();
			
			ClearSearchResults ();
			
			ShrinkResults ();
			
			if (CurrentPane == Pane.First && !results) Vanish ();
			else if (!something_typed) Reset ();
			
			if (GetSelection (Pane.Third) == null || !ThirdPaneAllowed) {
				ThirdPaneVisible = false;
			}
		}
		
		void OnInputKeyPressEvent (EventKey evnt)
		{
			if (im.FilterKeypress (evnt) || ((evnt.State & ModifierType.ControlMask) != 0))
				return;
//			im.Reset ();
			char c;
			if (evnt.Key == Key.Return) {
				c = '\n';
			} else {
				c = (char) Gdk.Keyval.ToUnicode (evnt.KeyValue);
			}
			if (char.IsLetterOrDigit (c)
					|| char.IsPunctuation (c)
					|| c == '\n'
					|| (c == ' ' && SearchController.Query.Length > 0)
					|| char.IsSymbol (c)) {
				SearchController.AddChar (c);
			}
		}
		
		void OnRightLeftKeyPressEvent (EventKey evnt)
		{
			im.Reset ();
			if (!SearchController.Results.Any ()) return;

			if ((Gdk.Key) evnt.KeyValue == RightKey) {
				// We're attempting to browse the contents of an item, so increase its
				// relevance.
				SearchController.Selection
					.IncreaseRelevance (SearchController.Query, null);
				if (SearchController.ItemChildSearch ()) GrowResults ();
			} else if ((Gdk.Key) evnt.KeyValue == LeftKey) {
				// We're attempting to browse the parent of an item, so decrease its
				// relevance. This makes it so we can merely visit an item's children,
				// and navigate back out of the item, and leave that item's relevance
				// unchanged.
				SearchController.Selection
					.DecreaseRelevance (SearchController.Query, null);
				if (SearchController.ItemParentSearch ()) GrowResults ();
			}
		}
		
		void OnTabKeyPressEvent (EventKey evnt)
		{
			im.Reset ();
			ShrinkResults ();

			if (SearchController.TextType == TextModeType.Explicit) {
				SearchController.FinalizeTextMode ();
				UpdatePane (CurrentPane);
			}
				
			if (evnt.Key == Key.Tab) {
				NextPane ();
			} else if (evnt.Key == Key.ISO_Left_Tab) {
				PrevPane ();
			}
		}
		
		void OnTextModePressEvent (EventKey evnt)
		{
			im.Reset ();

			// If this isn't the first keypress in text mode (we just entered text
			// mode) or if we're already in text mode, treat keypress as normal
			// input.
			bool pass_keypress = (0 < SearchController.Query.Length || SearchController.TextType == TextModeType.Explicit);
			SearchController.TextMode = true;
			
			if (pass_keypress || SearchController.TextMode == false)
				OnInputKeyPressEvent (evnt);
			UpdatePane (CurrentPane);
		}
		
		void OnUpDownKeyPressEvent (EventKey evnt)
		{
			im.Reset ();
			if (evnt.Key == UpKey) {
				if (!results_grown) {
					if (SearchController.Cursor > 0)
						GrowResults ();
					return;
				} else {
					if (SearchController.Cursor <= 0) {
						ShrinkResults ();
						return;
					}
					SearchController.Cursor--;
                }
			} else if (evnt.Key == DownKey) {
				if (!results_grown) {
					GrowResults ();
					return;
				}
				SearchController.Cursor++;
			} else if (evnt.Key == Gdk.Key.Home) {
				SearchController.Cursor = 0;
			} else if (evnt.Key == Gdk.Key.End) {
				SearchController.Cursor = SearchController.Results.Count - 1;
			} else if (evnt.Key == Gdk.Key.Page_Down) {
				SearchController.Cursor += 5;
			} else if (evnt.Key == Gdk.Key.Page_Up) {
				SearchController.Cursor -= 5;
			}
		}
		
		/// <summary>
		/// Converts a keypress into a human readable string for comparing
		/// against values in GConf.
		/// </summary>
		/// <param name="evnt">
		/// A <see cref="EventKey"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/> in the form "<Modifier>key"
		/// </returns>
		string KeyEventToString (EventKey evnt) {
			string modifier = "";
			if ((evnt.State & ModifierType.ControlMask) != 0) {
				modifier += "<Control>";
			}
			if ((evnt.State & ModifierType.SuperMask) != 0) {
				modifier += "<Super>";
			}
			if ((evnt.State & ModifierType.Mod1Mask) != 0) {
				modifier += "<Alt>";
			}
			return modifier + evnt.Key.ToString ();
		}
#endregion
		
		/// <summary>
		/// Selects the logical next pane in the UI left to right
		/// </summary>
		void NextPane ()
		{
			switch (CurrentPane) {
			case Pane.First:
				CurrentPane = Pane.Second;
				break;
			case Pane.Second:
				if (ThirdPaneAllowed)
					CurrentPane = Pane.Third;
				else
					CurrentPane = Pane.First;
				break;
			case Pane.Third:
				CurrentPane = Pane.First;
				break;
			}
		}
		
		/// <summary>
		/// Selects the logical previous pane in the UI left to right
		/// </summary>
		void PrevPane ()
		{
			switch (CurrentPane) {
			case Pane.First:
				if (ThirdPaneAllowed)
					CurrentPane = Pane.Third;
				else
					CurrentPane = Pane.Second;
				break;
			case Pane.Second:
				CurrentPane = Pane.First;
				break;
			case Pane.Third:
				CurrentPane = Pane.Second;
				break;
			}
		}
		
		/// <summary>
		/// This method determines what to do when a search is completed and takes the appropriate action
		/// </summary>
		/// <param name="o">
		/// A <see cref="System.Element"/>
		/// </param>
		/// <param name="state">
		/// A <see cref="SearchFinishState"/>
		/// </param>
		/// <param name="pane">
		/// A <see cref="Pane"/>
		/// </param>
		void SearchFinished (object o, SearchFinishState state, Pane pane)
		{
			if (pane == Pane.First && FirstControllerIsReset) {
				Reset ();
				return;
			}
			
			if (state.QueryChanged || state.SelectionChanged)
				SmartUpdatePane (pane);
		}

		/// <summary>
		/// Intended to wrap UpdatePane with smart conditionals to avoid excess/unexpected updates
		/// </summary>
		/// <param name="pane">
		/// A <see cref="Pane"/>
		/// </param>
		void SmartUpdatePane (Pane pane)
		{
			if (!FirstControllerIsReset)
				UpdatePane (pane);
		}
		
		protected void ClearSearchResults ()
		{
			switch (CurrentPane) {
			case Pane.First:
				Reset ();
				break;
			case Pane.Second:
				controllers [1].Reset ();
				controllers [2].Reset ();
				break;
			case Pane.Third:
				controllers [2].Reset ();
				break;
			}
		}
		
		protected void UpdatePane (Pane pane)
		{
			if (!window.Visible) return;
			
			//Lets see if we need to play with Third pane visibility
			if (pane == Pane.Third) {
				if (ThirdPaneRequired) {
					ThirdPaneVisible = true;
				} else if (CurrentPane != Pane.Third && (!ThirdPaneAllowed || ThirdPaneCanClose)) {
					ThirdPaneVisible = false;
				}
			}
			window.SetPaneContext (pane, controllers [(int) pane].UIContext);
		}
		
		/// <summary>
		/// Call to fully reset Do.  This should return Do to its starting state
		/// </summary>
		void Reset ()
		{
			ThirdPaneVisible = false;
			
			foreach (ISearchController controller in controllers)
				controller.Reset ();
			
			CurrentPane = Pane.First;
			
			ShrinkResults ();
			window.Reset ();
		}
		
		/// <summary>
		/// Should cause UI to display more results
		/// </summary>
		void GrowResults ()
		{
			UpdatePane (CurrentPane);
			window.GrowResults ();
			results_grown = true;	
		}
		
		/// <summary>
		/// Should cause UI to display fewer results, 0 == no results displayed
		/// </summary>
		void ShrinkResults ()
		{
			if (AlwaysShowResults) return;
			window.ShrinkResults ();
			results_grown = false;
		}
		
		Element GetSelection (Pane pane)
		{
			Element o;

			try {
				o = controllers [(int) pane].Selection;
			} catch {
				o = null;
			}
			return o;
		}
		
		protected virtual void PerformAction (bool vanish)
		{
			Element first, second, third;
			string actionQuery, itemQuery, modItemQuery;

			items.Clear ();
			modItems.Clear ();
			if (vanish) {
				Vanish ();
			}

			first  = GetSelection (Pane.First);
			second = GetSelection (Pane.Second);
			third  = GetSelection (Pane.Third);

			if (first != null && second != null) {

				if (first is Item) {
					foreach (Item item in controllers [0].FullSelection)
						items.Add (item);
					action = second as Act;
					itemQuery = controllers [0].Query;
					actionQuery = controllers [1].Query;
				} else {
					foreach (Item item in controllers [1].FullSelection)
						items.Add (item);
					action = first as Act;
					itemQuery = controllers [1].Query;
					actionQuery = controllers [0].Query;
				}

				modItemQuery = null;
				if (third != null && ThirdPaneVisible) {
					foreach (Item item in controllers [2].FullSelection)
						modItems.Add (item);
					modItemQuery = controllers [2].Query;
				}

				/////////////////////////////////////////////////////////////
				/// Relevance accounting
				/////////////////////////////////////////////////////////////
				
				if (first is Item) {
					// Act is in second pane.

					// Increase the relevance of the items.
					foreach (Element item in items)
						item.IncreaseRelevance (itemQuery, null);

					// Increase the relevance of the action /for each item/:
					foreach (Element item in items)
						action.IncreaseRelevance (actionQuery, item);
				} else {
					// Act is in first pane.

					// Increase the relevance of each item for the action.
					foreach (Element item in items)
						item.IncreaseRelevance (itemQuery, action);

					action.IncreaseRelevance (actionQuery, null);
				}

				if (third != null && ThirdPaneVisible)
					third.IncreaseRelevance (modItemQuery, action);

			}

			PerformAction (action, items, modItems);
			if (vanish) Reset ();
		}

		void PerformAction (Act action, IEnumerable<Item> items,
			IEnumerable<Item> modItems)
		{
			IEnumerable<Item> results = action.Safe.Perform (items, modItems);
			// If we have results to feed back into the window, do so in a new
			// iteration.
			if (results.Any ()) {
				GLib.Timeout.Add (10, delegate {
					Do.Controller.SummonWithElements (results.OfType<Element> ());
					return false;
				});
			}
		}

		#region IController Implementation
		public void Summon ()
		{
			if (!IsSummonable) return;
			
			// We want to disable updates so that any updates to universe dont happen
			// while controller is summoned.  We will disable this on vanish.  This
			// way we can be sure to dedicate our CPU resources to searching and
			// leave updating to a more reasonable time.
			Do.UniverseManager.UpdatesEnabled = false;
			window.Summon ();
			if (AlwaysShowResults) GrowResults ();
			im.FocusIn ();
		}
		
		public void Vanish ()
		{
			window.ShrinkResults ();
			results_grown = false;
			window.Vanish ();
			Do.UniverseManager.UpdatesEnabled = true;
		}

		public void ShowPreferences ()
		{
			Vanish ();
			Reset ();

			if (prefs_window == null) {
				prefs_window = new PreferencesWindow ();
				prefs_window.Hidden += delegate {
					// Release the window.
					prefs_window.Destroy ();
					prefs_window = null;
					// Reload universe.
					Do.UniverseManager.Reload ();
				};
			}
			
			prefs_window.Show ();
		}

		public void ShowAbout ()
		{
			string logo;

			Vanish ();
			Reset ();

			about_window = new Gtk.AboutDialog ();
			about_window.ProgramName = "GNOME Do";
			about_window.Modal = false;

			about_window.Version = AssemblyInfo.DisplayVersion + "\n" + AssemblyInfo.VersionDetails;

			logo = "gnome-do.svg";

			about_window.Logo = IconProvider.PixbufFromIconName (logo, 140);
			about_window.Copyright = "Copyright \xa9 2008 GNOME Do Developers";
			about_window.Comments = "Do things as quickly as possible\n" +
				"(but no quicker) with your files, bookmarks,\n" +
				"applications, music, contacts, and more!";
			about_window.Website = "http://do.davebsd.com/";
			about_window.WebsiteLabel = "Visit Homepage";
			about_window.IconName = "gnome-do";

			if (null != about_window.Screen.RgbaColormap)
				Gtk.Widget.DefaultColormap = about_window.Screen.RgbaColormap;

			about_window.Run ();
			about_window.Destroy ();
			about_window = null;
		}
		#endregion
		
		#region IDoController implementation
		
		public void NewContextSelection (Pane pane, int index)
		{
			if (!controllers [(int) pane].Results.Any () || index == controllers [(int) pane].Cursor) return;
			
			controllers [(int) pane].Cursor = index;
			window.SetPaneContext (pane, controllers [(int) pane].UIContext);
		}

		public void ButtonPressOffWindow ()
		{
			Vanish ();
			Reset ();
		}
		
		public ControlOrientation Orientation { get; set; }
		
		public bool ElementHasChildren (Element element)
		{
			return element is Item && (element as Item).HasChildren ();
		}
		
		#endregion
		
		public void PerformDefaultAction (Item item, IEnumerable<Type> filter) 
		{
			Act action =
				Do.UniverseManager.Search ("", typeof (Act).Cons (filter), item)
					.OfType<Act> ()
					.Where (act => act.Safe.SupportsItem (item))
					.FirstOrDefault ();

			if (action == null) return;
			
			item.IncreaseRelevance ("", null);
			PerformActionOnItem (action, item);
		}

		public void PerformActionOnItem (Act action, Item item)
		{
			PerformAction (action, new [] { item }, new Item [0]);
		}

	}
		
}
