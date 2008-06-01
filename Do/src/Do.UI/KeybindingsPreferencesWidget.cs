/* KeybindingsPreferencesWidget.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this source distribution.
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
using System.Collections.Generic;

using Gtk;

using Do;

namespace Do.UI
{
	public partial class KeybindingsPreferencesWidget : Bin, Addins.IConfigurable
	{
		private int IconSize = 32;
		private BindingsNodeView kbNodeView;
		
		public string Name {
			get { return "Keyboard"; }
		}
		
        public string Description {
        	get { return ""; }
        }
        
        public string Icon {
        	get { return ""; }
        }
		
		public KeybindingsPreferencesWidget ()
		{
			Build ();
			
			kbNodeView = new BindingsNodeView ();
			kbNodeView.ColumnsAutosize ();
            action_scroll.Add (kbNodeView);
            action_scroll.ShowAll ();
			
			help_icn.Pixbuf = IconProvider.PixbufFromIconName ("gtk-dialog-info",
			                                                    IconSize);
			
			/*
			// Initialize combo_summon
			if (!SummonKeyBindings.Contains (Do.Preferences.SummonKeyBinding)) {
				SummonKeyBindings.Insert (0, Do.Preferences.SummonKeyBinding);
			}
			foreach (string combo in SummonKeyBindings) {
				combo_summon.AppendText (combo);
			}
			combo_summon.Active = SummonKeyBindings.IndexOf (Do.Preferences.SummonKeyBinding);
			*/
		}
		
		public Bin GetConfiguration ()
        {
        	return this;
        }
	}
}
