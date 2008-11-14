// DockWindow.cs
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
using Gtk;
using Cairo;

using MonoDock.Util;
using MonoDock.XLib;

using Do.Addins;
using Do.UI;
using Do.Universe;

namespace MonoDock.UI
{
	
	
	public class DockWindow : Gtk.Window, IDoWindow
	{
		DockArea dock_area;
		IDoController controller;
		
		public DockWindow(IDoController controller) : base (Gtk.WindowType.Toplevel)
		{
			this.controller = controller;
			
			AppPaintable = true;
			KeepAbove = true;
			Decorated = false;
			SkipPagerHint = true;
			SkipTaskbarHint = true;
			Resizable = false;
			TypeHint = WindowTypeHint.Dock;
			
			this.SetCompositeColormap ();
			
			Build ();
		}
		
		void Build ()
		{
			dock_area = new DockArea (this, GetItems ());
			
			Add (dock_area);
			ShowAll ();
			
			GLib.Timeout.Add (3000, delegate {
				dock_area.SetIcons (GetItems ());
				return false;
			});
		}
		
		public void SetInputMask (int heightOffset)
		{
			Gdk.Pixmap pixmap = new Gdk.Pixmap (null, dock_area.Width, dock_area.Height-heightOffset, 1);
			Context cr = Gdk.CairoHelper.Create (pixmap);
			
			cr.Color = new Cairo.Color (0, 0, 0, 1);
			cr.Paint ();
			
			InputShapeCombineMask (pixmap, 0, heightOffset);
			
			(cr as IDisposable).Dispose ();
			pixmap.Dispose ();
		}
		
		IEnumerable<DockItem> GetItems ()
		{
			List<DockItem> items = new List<DockItem> ();
			
			foreach (IItem item in controller.Statistics.GetMostUsedItems ()) {
				items.Add (new DockItem (item));
			}
			
			
			return items;
		}
		
		protected override bool OnExposeEvent(EventExpose evnt)
		{
			Context cr = Gdk.CairoHelper.Create (GdkWindow);
			cr.Color = new Cairo.Color (0, 0, 0, 0);
			cr.Paint ();
			(cr as IDisposable).Dispose ();
			
			return base.OnExposeEvent (evnt);
		}
		
		protected override void OnShown ()
		{
			base.OnShown ();
			Reposition ();
			
			IntPtr display = Xlib.gdk_x11_drawable_get_xdisplay (GdkWindow.Handle);
			X11Atoms atoms = new X11Atoms (display);
			uint[] struts = new uint[12];
			
			struts[(int) XLib.Struts.Bottom] = (uint) dock_area.DockHeight;
			
			if (!IsRealized)
				return;
			
			Xlib.XChangeProperty (display, Xlib.gdk_x11_drawable_get_xid (GdkWindow.Handle), atoms._NET_WM_STRUT, 
			                      atoms.XA_CARDINAL, 32, (int) XLib.PropertyMode.PropModeAppend, struts, 4);
		}
		
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			
			Reposition ();
		}
		
		void Reposition ()
		{
			Gdk.Rectangle geo, main;
			
			GetSize (out main.Width, out main.Height);
			geo = Screen.GetMonitorGeometry (0);
			Move (((geo.X+geo.Width)/2) - main.Width/2, geo.Y+geo.Height-main.Height);
		}



		#region IDoWindow implementation 
		bool visible = false;
		
		public event Do.Addins.DoEventKeyDelegate KeyPressEvent;
		
		public void Summon ()
		{
			visible = true;
		}
		
		public void Vanish ()
		{
			visible = false;
		}
		
		public void Reset ()
		{
//			throw new System.NotImplementedException();
		}
		
		public void Grow ()
		{
//			throw new System.NotImplementedException();
		}
		
		public void Shrink ()
		{
//			throw new System.NotImplementedException();
		}
		
		public void GrowResults ()
		{
//			throw new System.NotImplementedException();
		}
		
		public void ShrinkResults ()
		{
//			throw new System.NotImplementedException();
		}
		
		public void SetPaneContext (Pane pane, IUIContext context)
		{
//			throw new System.NotImplementedException();
		}
		
		public void ClearPane (Pane pane)
		{
//			throw new System.NotImplementedException();
		}
		
		public bool Visible {
			get {
				return visible;
			}
		}
		
		Pane current_pane = Pane.First;
		public Pane CurrentPane {
			get {
				return current_pane;
			}
			set {
				current_pane = value;
			}
		}
		
		#endregion 
		

	}
}
