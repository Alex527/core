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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using Gtk;
using Mono.Addins;
using Mono.Addins.Setup;
using Mono.Unix;

using Do;
using Do.Core;
using Do.Core.Addins;
using Do.Interface;
using Do.Platform;
using Do.Platform.Linux;

namespace Do.UI
{
	[System.ComponentModel.Category("Do")]
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ManagePluginsPreferencesWidget : Bin, IConfigurable
	{
		PluginNodeView nview;
		SearchEntry search_entry;

		new public string Name {
			get { return Catalog.GetString ("Plugins"); }
		}

		public string Description {
			get { return ""; }
		}

		public string Icon {
			get { return ""; }
		}

		public ManagePluginsPreferencesWidget ()
		{
			Build ();
			
			search_entry = new SearchEntry ();
			nview = new PluginNodeView ();
			nview.PluginToggled += OnPluginToggled;
			nview.PluginSelected += OnPluginSelected;
			
			TargetEntry[] targets = {
				new TargetEntry ("text/uri-list", 0, 0), 
			};
			
			Gtk.Drag.DestSet (nview, DestDefaults.All, targets, Gdk.DragAction.Copy);
			nview.DragDataReceived += new DragDataReceivedHandler (OnDragDataReceived);
			
			scrollw.Add (nview);
			scrollw.ShowAll ();

			foreach (AddinClassifier cfier in PluginManager.Classifiers) {
				show_combo.AppendText (cfier.Name);
			}
			show_combo.Active = 0;
			
			search_entry = new SearchEntry ();
			
			search_entry.Changed += OnSearchEntryChanged;
			search_entry.Show();
			search_entry.Ready = true;
			
			hbox1.PackStart (search_entry, true, true, 0);
			hbox1.ShowAll ();
			
			Services.Application.RunOnMainThread (() => search_entry.InnerEntry.GrabFocus ());
		}
		
		protected void OnDragDataReceived (object sender, DragDataReceivedArgs args)
		{
			string data;
			string [] uriList;
			List<string> errors;
			
			data = Encoding.UTF8.GetString (args.SelectionData.Data);
			// Sometimes we get a null at the end, and it crashes us.
			data = data.TrimEnd ('\0');
			
			errors = new List<string> ();
			uriList = Regex.Split (data, "\r\n");
			
			foreach (string uri in uriList) {
				string file, path, filename;
				
				if (string.IsNullOrEmpty (uri))
					continue;
					
				try {
					file = uri.Remove (0, 7); // 7 is the length of file://
					// I have to use System.IO here due to a Gtk namespace conflict
					filename = System.IO.Path.GetFileName (file);
					
					if (!file.EndsWith (".dll")) {
						errors.Add (filename);
						continue;
					}

					if (!Directory.Exists (Paths.UserAddinInstallationDirectory))
						Directory.CreateDirectory (Paths.UserAddinInstallationDirectory);
					
					path = Paths.UserAddinInstallationDirectory.Combine (filename);
					File.Copy (file, path, true);
					
					if (errors.Count > 0)
						new PluginErrorDialog (errors.ToArray ());
			
					PluginManager.InstallLocalPlugins ();
				} catch (Exception e) { 
					Log<ManagePluginsPreferencesWidget>.Error ("An unexpected error occurred installing your plugin");
					Log<ManagePluginsPreferencesWidget>.Debug ("{0}\n{1}", e.Message, e.StackTrace);
				}
			} 
		}

		public Bin GetConfiguration ()
		{
			return this;
		}

		private void OnPluginSelected (object sender, PluginSelectionEventArgs e)
		{
			UpdateButtonState ();
		}

		protected void UpdateButtonState ()
		{
			string [] ids = nview.GetSelectedAddins ();
			
			btn_configure.Sensitive = ids
				.SelectMany (id => PluginManager.ConfigurablesForAddin (id))
				.Any ();
			
			btn_about.Sensitive = ids
				.Where (id => !string.IsNullOrEmpty (AddinManager.Registry.GetAddin (id).Description.Url))
				.Any ();
		}

		private void OnPluginToggled (string id, bool enabled)
		{	
			// If the addin isn't found, install it.
			if (null == AddinManager.Registry.GetAddin (id)) {
				IAddinInstaller installer = new ConsoleAddinInstaller ();
				try {
					installer.InstallAddins (AddinManager.Registry,
						string.Format ("Installing \"{0}\" addin...", id), new [] { id });
				} catch (InstallException e) {
					Log<ManagePluginsPreferencesWidget>.Error (e.Message);
					Log<ManagePluginsPreferencesWidget>.Debug (e.StackTrace);
					return;
				}
			}
			// Now enable or disable the plugin.
			if (enabled) {
				AddinManager.Registry.EnableAddin (id);
			} else {
				AddinManager.Registry.DisableAddin (id);
			}
			UpdateButtonState ();
		}
		
		void OnBtnRefreshClicked (object sender, EventArgs e)
		{
			nview.Refresh (true);
			UpdateButtonState ();
		}

		void OnBtnConfigurePluginClicked (object sender, EventArgs e)
		{
			Window win;
			string[] ids;

			ids = nview.GetSelectedAddins ();
			if (ids.Length == 0) return;

			win = new PluginConfigurationWindow (ids [0]);
			win.Modal = true;
			win.ShowAll ();
		}

		void OnBtnAboutClicked (object sender, EventArgs e)
		{
			foreach (string id in nview.GetSelectedAddins ()) {
				Addin addin = AddinManager.Registry.GetAddin (id);
				if (!string.IsNullOrEmpty (addin.Description.Url))
					Services.Environment.OpenUrl (addin.Description.Url);
			}
		}

		void OnShowComboChanged (object sender, EventArgs e)
		{
			nview.ShowCategory = show_combo.ActiveText;
			nview.Filter = search_entry.Query;
		}

		void OnSearchEntryChanged (object sender, EventArgs e)
		{
			nview.Filter = search_entry.Query;
		}

		void OnScrollwDragDataReceived (object o, DragDataReceivedArgs e)
		{
		}
	}
}
