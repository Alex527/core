/* KeybindingTreeView.cs
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

using Gtk;
using Mono.Unix;

using Do.Platform;
using Do.Platform.Common;

namespace Do.UI
{
	public class KeybindingTreeView : TreeView
	{
		enum Column {
			Action = 0,
			BoundKeyString,
			DefaultKeybinding,
			Binding,
			NumColumns
		}
		
		public KeybindingTreeView ()
		{	
			Model = new ListStore (typeof (string), typeof (string), typeof (string), typeof (KeyBinding));
			
			CellRendererText actionCell = new CellRendererText ();
			actionCell.Width = 150;
			InsertColumn (-1, Catalog.GetString ("Action"), actionCell, "text", (int)Column.Action);
			
			CellRendererAccel bindingCell = new CellRendererAccel ();
			bindingCell.AccelMode = CellRendererAccelMode.Other;
			bindingCell.Editable = true;
			bindingCell.AccelEdited += new AccelEditedHandler (OnAccelEdited);
			bindingCell.AccelCleared += new AccelClearedHandler (OnAccelCleared);
			InsertColumn (-1, Catalog.GetString ("Shortcut"), bindingCell, "text", (int)Column.BoundKeyString);
						
			RowActivated += new RowActivatedHandler (OnRowActivated);
			ButtonPressEvent += new ButtonPressEventHandler (OnButtonPress);
			
			AddBindings ();
			Selection.SelectPath (TreePath.NewFirst ());
		}
		
		private void AddBindings ()
		{
			ListStore store = Model as ListStore;
			store.Clear ();

			foreach (KeyBinding binding in Services.Keybinder.Bindings.Values.OrderBy (k => k.Description)) {
				store.AppendValues (binding.Description, binding.KeyString, binding.DefaultKeyString, binding);
			}
		}
		
		[GLib.ConnectBefore]
		private void OnButtonPress (object o, ButtonPressEventArgs args)
		{
			TreePath path;
			if (!args.Event.Window.Equals (BinWindow))
				return;
				
			if (GetPathAtPos ((int) args.Event.X, (int) args.Event.Y,out path)) {
				GrabFocus ();
				SetCursor (path, GetColumn ((int) Column.BoundKeyString), true);
			}				
		}
		
		private void OnRowActivated (object o, RowActivatedArgs args)
		{
			GrabFocus ();
			SetCursor (args.Path, GetColumn ((int) Column.BoundKeyString), true);
		}

		private bool ClearPreviousBinding (TreeModel model, TreePath path, TreeIter treeiter, string keyBinding) 
		{
			string binding = model.GetValue (treeiter, (int) Column.BoundKeyString) as string;
			if (binding == keyBinding) {
				model.SetValue (treeiter, (int) Column.BoundKeyString, "");
			}
			return false;
		}
		
		private void OnAccelEdited (object o, AccelEditedArgs args)
		{
			TreeIter iter;
			ListStore store;
			
			store = Model as ListStore;
			store.GetIter (out iter, new TreePath (args.PathString));
			
			string realKey = Gtk.Accelerator.Name (args.AccelKey, args.AccelMods);
			
			// Look for any other rows that have the same binding and then zero that binding out
			Model.Foreach ((model, path, treeiter) => ClearPreviousBinding (model, path, treeiter, realKey));

			store.SetValue (iter, (int) Column.BoundKeyString, realKey);

			SaveBindings ();
		}
		
		private void OnAccelCleared (object o, AccelClearedArgs args)
		{
			TreeIter iter;
			ListStore store;

			store = Model as ListStore;
			store.GetIter (out iter, new TreePath (args.PathString));
			try {
				string defaultVal = store.GetValue (iter, (int) Column.DefaultKeybinding).ToString ();
				store.SetValue (iter, (int) Column.BoundKeyString, defaultVal);
			} catch (Exception e) {
				store.SetValue (iter, (int) Column.BoundKeyString, "");
			}

			SaveBindings ();
		}
		
		private void SaveBindings ()
		{
			Model.Foreach (SaveBindingsForeachFunc);
		}
		
		private bool SaveBindingsForeachFunc (TreeModel model, TreePath path, TreeIter iter)
		{
			string newKeyString = model.GetValue (iter, (int) Column.BoundKeyString) as string;
			KeyBinding binding = model.GetValue (iter, (int) Column.Binding) as KeyBinding;

			if (newKeyString != null) {
				//try to save
				if (!Services.Keybinder.SetKeyString (binding, newKeyString)) {
					//if we fail reset to the default value
					model.SetValue (iter, (int) Column.BoundKeyString, binding.DefaultKeyString);
					Services.Keybinder.SetKeyString (binding, binding.DefaultKeyString);
				}
			}
			return false;
		}
		
	}

}
