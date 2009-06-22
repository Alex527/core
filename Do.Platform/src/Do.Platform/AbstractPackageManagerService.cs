/* AbstractPackageManagerService.cs
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

using Do.Platform.ServiceStack;

namespace Do.Platform
{
	
	public abstract class AbstractPackageManagerService : IService
	{
		public const string ShowPluginAvailableKey = "ShowPluginAvailableDialog";
		public const bool ShowPluginAvailableDefault = true;
		
		protected AbstractPackageManagerService ()
		{
		}
		
		protected void Initialize ()
		{
			Preferences = Services.Preferences.Get<AbstractPackageManagerService> ();
		}
		
		IPreferences Preferences { get; set; }
		
		protected void PromptForPluginInstall (string appName, string pluginName)
		{
		}
		
		protected bool ShouldShowPluginAvailableDialog {
			get { return Preferences.Get (ShowPluginAvailableKey, ShowPluginAvailableDefault); }
			set { Preferences.Get (ShowPluginAvailableKey, value); }
		}
	}
}
