/* ManagePluginsPreferencesWidget.cs
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
using Mono.Addins;
using Mono.Addins.Gui;
using Mono.Addins.Setup;

using Do;

namespace Do.UI
{
    public partial class ManagePluginsPreferencesWidget : Gtk.Bin, IPreferencePage
    {
        PluginNodeView nview;
		
		public Widget Page {
			get { return this; }
		}
		
		public string Label {
			get { return "Plugins"; }
		}

        public ManagePluginsPreferencesWidget()
        {
            Build ();
			
            nview = new PluginNodeView ();
            nview.PluginToggled += OnPluginToggled;
			nview.PluginSelected += OnPluginSelected;

            scrollw.Add (nview);
            scrollw.ShowAll ();
        }

		private void OnPluginSelected (string id)
        {
			btn_about.Sensitive = true;
		}
		
        private void OnPluginToggled (string id, bool enabled)
        {
            // If the addin isn't found, install it.
            if (null == AddinManager.Registry.GetAddin (id)) {
                IAddinInstaller installer;

                installer = new ConsoleAddinInstaller ();
                //installer = new Mono.Addins.Gui.AddinInstaller ();
                try {
                    installer.InstallAddins (AddinManager.Registry,
                        string.Format ("Installing \"{0}\" addin...", id),
                        new string[] { id });
                } catch (InstallException) {
                    return;
                }
            }

            // Now enable or disable the plugin.
            if (enabled)
                AddinManager.Registry.EnableAddin (id);
            else
                AddinManager.Registry.DisableAddin (id);

            // For now...
            // TODO: remove this
            Do.UniverseManager.Reload ();
        }

        protected virtual void OnBtnRefreshClicked (object sender, System.EventArgs e)
        {
            nview.Refresh ();
        }

        protected virtual void OnBtnUpdateClicked (object sender, System.EventArgs e)
        {
			if (Do.PluginManager.InstallAvailableUpdates (true))
				nview.Refresh ();
        }
		
		protected virtual void OnBtnConfigurePluginClicked (object sender, System.EventArgs e)
        {
        }

		protected virtual void OnBtnAboutClicked (object sender, System.EventArgs e)
		{
		}
    }
}
