// GLBWindow.cs created with MonoDevelop
// User: dave at 11:15 AM 8/25/2007
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Runtime.InteropServices;
using Gtk;
using Gdk;

using Do.Core;

namespace Do.UI
{
	
	public class LBWindow : Gtk.Window
	{
		
		private static Pixbuf default_item_pixbuf;
		const int ResultsListIconSize = 32;
		const int ResultsListLength = 7; 
		
		protected enum WindowFocus {
			ItemFocus,
			CommandFocus
		}
		
		protected enum Column {
			ItemColumn = 0,
			PixbufColumn = 1,
			NameColumn = 2,
			NumberColumns = 3
		}
		
		protected Commander commander;
		protected Command current_command;
		protected Do.Core.Item current_item;
		protected int current_item_index;
		protected int current_command_index;
		protected WindowFocus focus;
		protected string searchString;
		protected string itemSearchString;
		
		LBFrame frame;
		Alignment instruction;
		Label DisplayLabel;
	
		ScrolledWindow result_sw;
		HBox result_hbox;

		LBIconBox item_icon_box;
		LBIconBox command_icon_box;

		TreeView result_treeview;

		bool          can_rgba;
		bool          transparent;
		
		static LBWindow ()
		{
			default_item_pixbuf = Util.PixbufFromIconName ("gtk-find", Util.DefaultIconSize);
		}
		
		public LBWindow (Commander commander, bool transparent) : base ("GNOME Go")
		{
			Build ();
			
			this.commander = commander;
			SetDefaultState ();
			Transparent = transparent;
			
			commander.SetDefaultStateEvent += OnDefaultStateEvent;
			commander.SetSearchingItemsStateEvent += OnSearchingStateEvent;
			commander.SetSearchingCommandsStateEvent += OnSearchingStateEvent;
			commander.SetItemSearchCompleteStateEvent += OnSearchCompleteStateEvent;
			commander.SetCommandSearchCompleteStateEvent += OnSearchCompleteStateEvent;
		}
		
		public bool Transparent {
			get { return transparent; }
			set {
				if (value && !can_rgba) {
					Console.Error.WriteLine ("Cannot paint window transparent (no rgba).");
					return;
				}
				transparent = value;	
				if (transparent) {
					item_icon_box.Transparent = true;
					command_icon_box.Transparent = true;
					frame.Fill = true;
					DisplayLabel.ModifyFg (StateType.Normal, DisplayLabel.Style.White);
					result_sw.ShadowType = ShadowType.None;
				} else {
					DisplayLabel.ModifyFg (StateType.Normal, DisplayLabel.Style.Foreground (StateType.Normal));
					result_sw.ShadowType = ShadowType.In;
				}
				if (IsDrawable) {
					QueueDraw ();
				}
			}
		}
		
		protected void Build ()
		{
			VBox         vbox;
			TreeViewColumn column;
			CellRenderer   cell;
			
			AppPaintable = true;
			KeepAbove = true;
			Decorated = false;
			
			SetPosition (WindowPosition.CenterAlways);
			
			try { SetIconFromFile ("/usr/share/icons/gnome/scalable/actions/system-run.svg"); } catch { }
			SetColormap ();
			
			focus = WindowFocus.ItemFocus;
			
			searchString = itemSearchString = "";
			
			frame = new LBFrame ();
			frame.FillColor = new Gdk.Color (0, 0, 0);
			frame.FillAlpha = (ushort) (ushort.MaxValue * 0.8);
			Add (frame);
			frame.Show ();
			
			vbox = new VBox (false, 6);
			vbox.BorderWidth = 18;
			frame.Add (vbox);
			vbox.Show ();		
			
			result_hbox = new HBox (false, 12);
			vbox.PackStart (result_hbox, false, false, 0);
			result_hbox.Show ();
			
			item_icon_box = new LBIconBox ("", null);
			item_icon_box.IsFocused = true;
			result_hbox.PackStart (item_icon_box, false, false, 0);
			item_icon_box.Show ();
			
			command_icon_box = new LBIconBox ("", null);
			command_icon_box.IsFocused = false;
			result_hbox.PackStart (command_icon_box, false, false, 0);
			command_icon_box.Show ();

			instruction = new Alignment (0.5F, 0.5F, 1, 1);
			instruction.SetPadding (8, 0, 0, 0);
			DisplayLabel = new Label ();
			DisplayLabel.UseMarkup = true;

			instruction.Add (DisplayLabel);
			vbox.PackStart (instruction, false, false, 0);
			DisplayLabel.Show ();
			instruction.Show ();
			
			result_sw = new ScrolledWindow ();
			result_sw.SetSizeRequest (-1, (ResultsListIconSize + 2) * ResultsListLength + 2);
			result_sw.SetPolicy (PolicyType.Automatic, PolicyType.Automatic);
			result_sw.ShadowType = ShadowType.In;
			vbox.PackStart (result_sw, true, true, 0);
			result_sw.Show ();
			
			result_treeview = new TreeView ();
			result_treeview.EnableSearch = false;
			result_treeview.HeadersVisible = false;
			result_sw.Add (result_treeview);
			result_treeview.Show ();
			
			result_treeview.Model = new ListStore (new Type[] { typeof(GCObject), typeof(Pixbuf), typeof(string) }); 
			
			column = new TreeViewColumn ();
			cell = new CellRendererPixbuf ();
			column.PackStart (cell, false);
			
			cell.CellBackgroundGdk = new Color (byte.MaxValue, byte.MaxValue, byte.MaxValue);
			// This property is not available.
			// cell.CellBackgroundSet = true;
			// Maybe below?
			// cell.CellBackground = "white";
			cell.SetFixedSize (-1, 4 + ResultsListIconSize - (int) cell.Ypad);
			column.AddAttribute (cell, "pixbuf", (int) Column.PixbufColumn);
			
			cell = new CellRendererText ();
			(cell as CellRendererText).Ellipsize = Pango.EllipsizeMode.End;
			column.PackStart (cell, true);
			column.AddAttribute (cell, "markup", (int) Column.NameColumn);
			
			result_treeview.AppendColumn (column);
			
			result_treeview.Selection.Changed += OnWindowResultRowSelected;
			ScreenChanged += OnScreenChanged;
		}
		
		protected virtual void SetColormap ()
		{
			Gdk.Colormap  colormap;

			colormap = this.Screen.RgbaColormap;
			if (colormap != null) {
				can_rgba = true;
			} else {
				colormap = this.Screen.RgbColormap;
				System.Console.WriteLine ("No alpha support.");
			}
			this.Colormap = colormap;
		}
		
		protected virtual void ShowSearchResults ()
		{
			Requisition size;
			
			result_sw.Show ();
			// instruction.Hide ();
			size = SizeRequest ();
			Resize (size.Width, size.Height);
		}
		
		protected virtual void HideSearchResults ()
		{
			Requisition size;
			
			result_sw.Hide ();
			// instruction.Show ();
			size = SizeRequest();
			Resize (size.Width, size.Height);
		}	
		
		protected override bool OnButtonPressEvent (EventButton evnt)
		{
			Hide ();
			return false;
		}

		
		protected override bool OnKeyPressEvent (EventKey evnt)
		{
			Gdk.Key key;
			
			key = (Gdk.Key) evnt.KeyValue;
			
			// Handle command keys (Quit, etc.)
			if (((int) evnt.State & (int) ModifierType.ControlMask) != 0) {
				switch (key) {
				case Gdk.Key.q:
					Application.Quit ();
					break;
				}
				return false;
			}
			
			switch (key) {
			// Throwaway keys
			case Gdk.Key.Shift_L:
			case Gdk.Key.Control_L:
				break;
			case Gdk.Key.Escape:
				if (focus == WindowFocus.ItemFocus) {
					if (searchString.Length == 0) {
						Hide ();
					}
					commander.State = CommanderState.Default;
				} else {
					if (searchString.Length == 0) {
						commander.State = CommanderState.Default;
					} else {
						searchString = "";
						QueueSearch ();
					}
				}
				break;
			case Gdk.Key.Return:
			case Gdk.Key.ISO_Enter:
				ActivateCommand ();
				break;
			case Gdk.Key.Delete:
			case Gdk.Key.BackSpace:
				if (searchString.Length > 1) {
					searchString = searchString.Substring (0, searchString.Length-1);
					QueueSearch ();
				} else {
					commander.State = CommanderState.Default;
				}
				break;
			case Gdk.Key.Tab:
				if (focus == WindowFocus.ItemFocus && commander.CurrentItem != null) {
					SetWindowFocus (WindowFocus.CommandFocus);
				} else if (focus == WindowFocus.CommandFocus) {
					SetWindowFocus (WindowFocus.ItemFocus);
				}
				break;
			case Gdk.Key.Up:
			case Gdk.Key.Down:
			{
				TreeModel model;
				TreeIter iter;
				TreePath path;
				int num_items = 0;
				
				switch (focus) {
				case WindowFocus.ItemFocus:
					num_items = commander.CurrentItems.Length;
					break;
				case WindowFocus.CommandFocus:
					num_items = commander.CurrentCommands.Length;
					break;
				}
				if (num_items == 0) {
					return false;
				}
				
				result_treeview.Selection.GetSelected (out model, out iter);
				path = model.GetPath (iter);
				
				if (key == Gdk.Key.Up) {
					if (!path.Prev ()) {
						HideSearchResults ();
					}
				}
				else if (key == Gdk.Key.Down && num_items > 0) {
					if (result_sw.Visible) {
						path.Next ();
					} else {
						ShowSearchResults ();
					}
				}					
				result_treeview.Selection.SelectPath (path);
				result_treeview.ScrollToCell (path, null, false, 0.0F, 0.0F);
			}
				break;
			case Gdk.Key.Right:
			case Gdk.Key.Left:
				break;
			default:
				// Default input behavior
				if (key == Gdk.Key.space) {
					searchString += " ";
				} else {
					searchString += key;
				}
				QueueSearch ();
				break;
			}
			return false;
		}
		
		protected virtual void ActivateCommand ()
		{
			Hide ();
			commander.Execute ();
			commander.State = CommanderState.Default;
		}
		
		private void OnWindowResultRowSelected (object sender, EventArgs args)
		{
			int selectedRowIndex;
			
			try {
			selectedRowIndex = result_treeview.Selection.GetSelectedRows()[0].Indices[0];
			} catch (IndexOutOfRangeException) {
				return;
			}
			if (focus == WindowFocus.ItemFocus) {
				SetItemIndex (selectedRowIndex, searchString);
			} else if (focus == WindowFocus.CommandFocus) {
				SetCommandIndex (selectedRowIndex, searchString);
			}			
		}

		private void OnScreenChanged (object sender, EventArgs args)
		{
			SetColormap ();
		}
		
		protected override bool OnExposeEvent (EventExpose evnt)
		// private void OnExposeEvent (object sender, ExposeEventArgs args)
		{
			//return;
			Cairo.Context cairo;
			
			if (!transparent) {
				Gtk.Style.PaintBox (Style,
				                    GdkWindow,
				                    StateType.Normal,
				                    ShadowType.In,
				                    evnt.Area,
				                    this,
				                    "base",
				                    0, 0, -1, -1);
			} else {
				cairo = Gdk.CairoHelper.Create (GdkWindow);
				cairo.Rectangle (evnt.Area.X, evnt.Area.Y, evnt.Area.Width, evnt.Area.Height);
				cairo.Color = new Cairo.Color (1.0, 1.0, 1.0, 0.0);
				cairo.Operator = Cairo.Operator.Source;
				cairo.Paint ();
			}
			return base.OnExposeEvent (evnt);
		}
		
		protected virtual void SetWindowFocus (WindowFocus focus)
		{
			if (this.focus == focus) {
				return;
			}
			if (focus == WindowFocus.CommandFocus && commander.CurrentCommand == null) {
				return;
			}
			
			this.focus = focus;
			
			(result_treeview.Model as ListStore).Clear ();
			switch (focus) {
			case WindowFocus.CommandFocus:
				searchString = commander.CommandSearchString;
				break;
			case WindowFocus.ItemFocus:
				searchString = commander.ItemSearchString;
				break;
			}
			OnSearchCompleteStateEvent ();
			DisplayLabel.Markup = string.Format ("<big>{0}</big>", searchString);
			item_icon_box.IsFocused = (focus == WindowFocus.ItemFocus);
			command_icon_box.IsFocused = (focus == WindowFocus.CommandFocus);
		}
		
		protected virtual void QueueSearch ()
		{
			switch (focus) {
			case WindowFocus.ItemFocus:
				commander.SearchItems (searchString);
				break;
			case WindowFocus.CommandFocus:
				commander.SearchCommands (searchString);
				break;
			}
		}
		
		protected virtual void SetItemIndex (int itemIndex, string match)
		{
			string markup;
				
			if (itemIndex >= commander.CurrentItems.Length) {
				SetItemIndex (commander.CurrentItems.Length-1, match);
				return;
			}
			try {
				commander.CurrentItemIndex = itemIndex;
			} catch (IndexOutOfRangeException) {
				return;
			}
			markup = Util.MarkupSubstring (commander.CurrentItem.Name, match);
			item_icon_box.Caption = markup;
			item_icon_box.Pixbuf = commander.CurrentItem.Pixbuf;
			SetCommandIndex (0, "");
		}
		
		protected virtual void SetCommandIndex (int commandIndex, string match)
		{
			string markup;
			
			try {
				commander.CurrentCommandIndex = commandIndex;
			} catch (IndexOutOfRangeException) {
				return;
			}
			if (match != null) {
				markup = Util.MarkupSubstring (commander.CurrentCommand.Name, match);
			} else {
				markup = commander.CurrentCommand.Name;
			}
			command_icon_box.Caption = markup;
			command_icon_box.Pixbuf = commander.CurrentCommand.Pixbuf;
		}
		
		protected virtual void SetDefaultState ()
		{
			searchString = itemSearchString = "";
			
			HideSearchResults ();
			(result_treeview.Model as ListStore).Clear ();
			SetWindowFocus (WindowFocus.ItemFocus);
			
			item_icon_box.Pixbuf = default_item_pixbuf;
			item_icon_box.Caption = "";
			command_icon_box.Clear ();
			
			DisplayLabel.Markup = String.Format ("<i><big>{0}</big></i>", "Type to begin searching");			
		}
		
		protected virtual void SetNoResultsFoundState ()
		{
			switch (focus) {
			case WindowFocus.CommandFocus:
				command_icon_box.Clear ();
				DisplayLabel.Markup = String.Format ("<i><big>{0}</big></i>", "No commands found");
				break;
			default:
			// case WindowFocus.ItemFocus:
				command_icon_box.Clear ();
				item_icon_box.Clear ();
				DisplayLabel.Markup = String.Format ("<i><big>{0}</big></i>", "No items found");
				break;
			}
		}
		
		protected void OnDefaultStateEvent ()
		{
			SetDefaultState ();
		}
		
		protected void OnSearchingStateEvent ()
		{
			DisplayLabel.Markup = String.Format ("<big>{0}</big>", searchString);
		}
		
		protected void OnSearchCompleteStateEvent ()
		{
			ListStore store;
			TreeIter  iter, selected_iter;
			GCObject[] results;
			
			switch (focus) {
			case WindowFocus.ItemFocus:
				results = commander.CurrentItems;
				break;
			case WindowFocus.CommandFocus:
				results = commander.CurrentCommands;
				break;
			default:
				results = new GCObject [0];
				break;
			}
			
			if (results.Length == 0) {
				SetNoResultsFoundState ();
			} else {
				SetItemIndex (commander.CurrentItemIndex, commander.ItemSearchString);
				SetCommandIndex (commander.CurrentCommandIndex, commander.CommandSearchString);
				
				store = result_treeview.Model as ListStore;
				store.Clear ();
				int current_index = 0;
				selected_iter = default (TreeIter);
				foreach (GCObject result in results) {
					Pixbuf small_icon = Util.PixbufFromIconName (result.Icon, ResultsListIconSize);
					string result_info = string.Format ("<b>{0}</b>\n<i><small>{1}</small></i>", result.Name, result.Description);
					iter = store.AppendValues (new object[] { result, small_icon, result_info });
					
					if ((focus == WindowFocus.ItemFocus && commander.CurrentItemIndex == current_index) ||
						(focus == WindowFocus.CommandFocus && commander.CurrentCommandIndex == current_index)) {
						selected_iter = iter;
					}
					current_index++;
				}
				result_treeview.Selection.SelectIter (selected_iter);
				result_treeview.ScrollToCell (result_treeview.Model.GetPath (selected_iter),
				                              null, false, 0.0F, 0.0F);
				
			}
		}
		
	}

	
}
