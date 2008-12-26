// DockItem.cs
// 
// Copyright (C) 2008 GNOME Do
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Linq;

using Gdk;
using Cairo;

using Do.Interface;
using Do.Platform;
using Do.Universe;
using Do.Universe.Common;
using Do.Interface.CairoUtils;

using Docky.Utilities;

using Wnck;

namespace Docky.Interface
{
	
	
	public class DockItem : AbstractDockItem, IRightClickable, IDockAppItem
	{
		Element item;
		Surface icon_surface;
		List<Wnck.Application> apps;
		Gdk.Rectangle icon_region;
		Gdk.Pixbuf drag_pixbuf;
		bool needs_attention;
		uint handle_timer = 0;
		
		public event EventHandler RemoveClicked;
		
		public event UpdateRequestHandler UpdateNeeded;
		
		public string Icon { 
			get { return item.Icon; } 
		}
		
		public override string Description { 
			get { return item.Name; } 
		}
		
		public Element Element { 
			get { return item; } 
		}
		
		public Wnck.Application [] Apps { 
			get { return apps.ToArray (); } 
		}
		
		public IEnumerable<int> Pids { 
			get { return apps.Select (item => item.Pid).ToArray (); } 
		}
		
		public override int WindowCount {
			get {
				int out_val = 0;
				foreach (Application app in Apps) {
					out_val += app.Windows.Where (w => !w.IsSkipTasklist).Count ();
				}
				return out_val;
			}
		}

		public bool NeedsAttention { 
			get { return needs_attention; } 
			private set {
				if (needs_attention == value)
					return;
				needs_attention = value;
				AttentionRequestStartTime = DateTime.UtcNow;
			}
		}
		
		public DateTime AttentionRequestStartTime { get; private set; }
		
		public IEnumerable<Act> ActionsForItem {
			get {
				if (!(item is Item))
					return new Act [0];
				return Services.Core.GetActionsForItemOrderedByRelevance (item as Item, false);
			}
		}
			
		
		bool HasVisibleApps {
			get {
				if (apps == null)
					return false;
				return apps
					.Select (app => app.Windows)
					.Any (wins => wins.Any (win => !win.IsSkipTasklist));
			}
		}	
		
		public DockItem (Element item) : base ()
		{
			apps =  new List<Wnck.Application> ();
			this.item = item;
			
			AttentionRequestStartTime = DateTime.UtcNow;
			UpdateApplication ();
			NeedsAttention = DetermineAttentionStatus ();
		}
		
		protected override void OnIconSizeChanged ()
		{
			if (icon_surface != null) {
				icon_surface.Destroy ();
				icon_surface = null;
			}
			
			base.OnIconSizeChanged ();
		}

		
		public void UpdateApplication ()
		{
			UnregisterStateChangeEvents ();
			
			if (item is IApplicationItem) {
				apps = WindowUtils.GetApplicationList ((item as IApplicationItem).Exec);
			}
			
			RegisterStateChangeEvents ();
		}
		
		void RegisterStateChangeEvents ()
		{
			foreach (Application app in Apps) {
				foreach (Wnck.Window w in app.Windows) {
					if (!w.IsSkipTasklist)
						w.StateChanged += OnWindowStateChanged;
				}
			}
		}
		
		void UnregisterStateChangeEvents ()
		{
			foreach (Application app in Apps) {
				foreach (Wnck.Window w in app.Windows) {
					try {
						w.StateChanged -= OnWindowStateChanged;
					} catch {}
				}
			}
		}
		
		void OnWindowStateChanged (object o, StateChangedArgs args)
		{
			if (handle_timer > 0)
				return;
			
			handle_timer = GLib.Timeout.Add (100, HandleUpdate);
		}
		
		bool HandleUpdate ()
		{
			bool needed_attention = NeedsAttention;
			NeedsAttention = DetermineAttentionStatus ();
			
			if (NeedsAttention != needed_attention) {
				UpdateRequestType req;
				if (NeedsAttention) 
					req = UpdateRequestType.NeedsAttentionSet;
				else
					req = UpdateRequestType.NeedsAttentionUnset;
				if (UpdateNeeded != null)
					UpdateNeeded (this, new UpdateRequestArgs (this, req));
			}
			
			handle_timer = 0;
			return false;
		}
		
		bool DetermineAttentionStatus  ()
		{
			foreach (Application app in Apps) {
				if (app.Windows.Any ((Wnck.Window w) => w.NeedsAttention ()))
					return true;
			}
			return false;
		}
		
		Gdk.Pixbuf GetPixbuf ()
		{
			Gdk.Pixbuf pbuf = IconProvider.PixbufFromIconName (Icon, DockPreferences.FullIconSize);
			
			if (pbuf.Height != DockPreferences.FullIconSize && pbuf.Width != DockPreferences.FullIconSize) {
				double scale = (double)DockPreferences.FullIconSize / Math.Max (pbuf.Width, pbuf.Height);
				Gdk.Pixbuf temp = pbuf.ScaleSimple ((int) (pbuf.Width * scale), (int) (pbuf.Height * scale), InterpType.Bilinear);
				pbuf.Dispose ();
				pbuf = temp;
			}
			
			return pbuf;
		}
		
		public override Pixbuf GetDragPixbuf ()
		{
			if (drag_pixbuf == null)
				drag_pixbuf = IconProvider.PixbufFromIconName (Icon, 32);
			return drag_pixbuf;
		}
		
		public override Surface GetIconSurface (Surface sr)
		{
			if (icon_surface == null) {
				Gdk.Pixbuf pixbuf = GetPixbuf ();
				icon_surface = sr.CreateSimilar (sr.Content, DockPreferences.FullIconSize, DockPreferences.FullIconSize);
				Context cr = new Context (icon_surface);
				Gdk.CairoHelper.SetSourcePixbuf (cr, 
				                                 pixbuf, 
				                                 (int) ((DockPreferences.FullIconSize - pixbuf.Width) / 2),
				                                 (int) ((DockPreferences.FullIconSize - pixbuf.Height) / 2));
				cr.Paint ();
				
				(cr as IDisposable).Dispose ();
				pixbuf.Dispose ();
				pixbuf = null;
			}
			return icon_surface;
		}
		
		public override void Clicked (uint button)
		{
			if (!apps.Any () || !HasVisibleApps || button == 2) {
				Launch ();
				return;
			}
				
			if (button == 1)
				WindowUtils.PerformLogicalClick (apps);
		}
		
		void Launch ()
		{
			LastClick = DateTime.UtcNow;
			if (Element is IFileItem)
				Services.Core.PerformDefaultAction (Element as Item, new [] { typeof (OpenAction), });
			else
				Services.Core.PerformDefaultAction (Element as Item, Type.EmptyTypes);
			return;
		}
		
		void Launch (Act action)
		{
			LastClick = DateTime.UtcNow;
			if (!(item is Item))
				return;
			Services.Core.PerformActionForItem (action, item as Item);
		}
		
		public override void SetIconRegion (Gdk.Rectangle region)
		{
			if (icon_region == region)
				return;
			icon_region = region;
			
			foreach (Wnck.Application application in apps) {
				foreach (Wnck.Window window in application.Windows) {
					window.SetIconGeometry (region.X, region.Y, region.Width, region.Height);
				}
			}
		}
		
		public override bool Equals (IDockItem other)
		{
			DockItem di = other as DockItem;
			if (di == null)
				return false;
			
			return di.Element.UniqueId == Element.UniqueId;
		}

		#region IDisposable implementation 
		
		public override void Dispose ()
		{
			UnregisterStateChangeEvents ();
			item = null;
			apps.Clear ();
			
			if (drag_pixbuf != null)
				drag_pixbuf.Dispose ();
			
			if (icon_surface != null) {
				icon_surface.Destroy ();
				icon_surface = null;
			}
			
			base.Dispose ();
		}
		
		#region IRightClickable implementation 
		
		public IEnumerable<MenuArgs> GetMenuItems ()
		{
			List<MenuArgs> outList = new List<MenuArgs> ();
			bool hasApps = HasVisibleApps;
			
			if (hasApps) {
				foreach (Application app in Apps) {
					foreach (Wnck.Window window in app.Windows) {
						Wnck.Window copy_win = window;
						if (!copy_win.IsSkipTasklist) {
							string name = copy_win.Name;
							if (name.Length > 50)
								name = name.Substring (0, 47) + "...";
							outList.Add (new MenuArgs ((o, a) => copy_win.CenterAndFocusWindow (), name, Icon, true));
						}
					}
				}
				outList.Add (new SeparatorMenuArgs ());
			}
			
			foreach (Act act in ActionsForItem) {
				Act internal_act = act;
				outList.Add (new MenuArgs ((o, a) => Launch (internal_act), internal_act.Name, internal_act.Icon, true));
			}
			outList.Add (new MenuArgs (MinimizeRestoreWindows, "Minimize/Restore", Gtk.Stock.GoDown, hasApps));
			outList.Add (new MenuArgs (CloseAllOpenWindows, "Close All", Gtk.Stock.Quit, hasApps));
			outList.Add (new MenuArgs (OnRemoveClicked, "Remove From Dock", Gtk.Stock.Remove, true));
			
			return outList;
		}
		
		#endregion 
		#endregion
		void CloseAllOpenWindows (object o, EventArgs a)
		{
			List<Wnck.Window> windows = new List<Wnck.Window> ();
			foreach (Application app in Apps)
				windows.AddRange (app.Windows);
			WindowControl.CloseWindows (windows);
		}
		
		void MinimizeRestoreWindows (object o, EventArgs a)
		{
			List<Wnck.Window> windows = new List<Wnck.Window> ();
			foreach (Application app in Apps)
				windows.AddRange (app.Windows);
			WindowControl.MinimizeRestoreWindows (windows);
		}
		
		void OnRemoveClicked (object o, EventArgs a)
		{
			if (RemoveClicked != null)
				RemoveClicked (this, a);
		}
	}
}
