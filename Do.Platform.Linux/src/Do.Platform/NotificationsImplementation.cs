/* NotificationsImplementation.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * source distribution.
 *  
 * This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
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
using Notifications;
using Mono.Unix;
using Gdk;
using GLib;

namespace Do.Platform.Linux
{
	
	public class NotificationsImplementation : Notifications.Implementation
	{
		const int IconSize = 24, MinNotifyShow = 5000, MaxNotifyShow = 10000;
		Pixbuf default_icon = IconProvider.PixbufFromIconName ("gnome-do", IconSize);
		
		#region Notifications.Implementation
		
		public void Notify (string title, string message, string icon, Action onClick)
		{

			Notification msg;
			try {
				msg = new Notification ();
			} catch (Exception e) {
				Log.Error ("Could not show notification: " + e.Message);
				return;
			}
			
			msg.Closed += new EventHandler (OnNotificationClosed); 
			msg.Summary = GLib.Markup.EscapeText (title);
			msg.Body = GLib.Markup.EscapeText (message);
			if (icon != null)
				msg.Icon = IconProvider.PixbufFromIconName (icon, IconSize);
			else
				msg.Icon = default_icon;
			
			if (onClick != null)
				msg.AddAction (action_name, action_label, (o, a) => onClick ());
				
			msg.Timeout = message.Length / 10 * 1000;
			if (msg.Timeout > MaxNotifyShow) msg.Timeout = MaxNotifyShow;
			if (msg.Timeout < MinNotifyShow) msg.Timeout = MinNotifyShow;
			
			msg.Show ();
		}
		
		#endregion
		
	}
}
