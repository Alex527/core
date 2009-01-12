// DockPopupMenu.cs
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

using Cairo;
using Gdk;
using Gtk;

using Do.Interface;
using Do.Interface.CairoUtils;
using Do.Platform;

using Docky.Utilities;

namespace Docky.Interface
{
	
	
	public class DockPopupMenu : Gtk.Window
	{
		const int TailHeight = 25;
		new const int BorderWidth = 2;
		const int Radius = 10;
		const int Width = 230;
		const int TailOffset = 2;
		const double Pointiness = 2;
		const double BaseCurviness = 1.2;
		const double TailCurviness = .8;
		
		int horizontal_offset;
		
		public Gtk.VBox Container { get; private set; } 
		
		public DockPopupMenu() : base (Gtk.WindowType.Popup)
		{
			Decorated = false;
			KeepAbove = true;
			AppPaintable = true;
			SkipPagerHint = true;
			SkipTaskbarHint = true;
			Resizable = false;
			Modal = true;
			TypeHint = WindowTypeHint.PopupMenu;
			
			WidthRequest = Width;
			
			this.SetCompositeColormap ();
			
			AddEvents ((int) EventMask.PointerMotionMask | 
			           (int) EventMask.LeaveNotifyMask |
			           (int) EventMask.ButtonPressMask | 
			           (int) EventMask.ButtonReleaseMask |
			           (int) EventMask.FocusChangeMask);
			
			Container = new Gtk.VBox ();
			Build ();
		}
		
		protected virtual void Build ()
		{
			VBox space = new VBox ();
			space.Add (Container);
			space.Add (new Label (" "));
			Container.BorderWidth = 5;
			Add (space);
			space.ShowAll ();
		}

		
		public virtual void PopUp (IEnumerable<AbstractMenuButtonArgs> args, int x, int y)
		{
			ShowAll ();
			Gdk.Rectangle geo = Screen.GetMonitorGeometry (DockPreferences.Monitor);
			Gtk.Requisition req = SizeRequest ();
			
			int posx = Math.Min (Math.Max (0, x - req.Width / TailOffset), geo.X + geo.Width - req.Width);
			horizontal_offset = (x - req.Width / TailOffset) - posx;
			Move (posx, y - req.Height);
			
			Do.Interface.Windowing.PresentWindow (this);
		}
		
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			using (Context cr = Gdk.CairoHelper.Create (GdkWindow)) {
				cr.AlphaFill ();
				DrawBackground (cr);
			}
			
			return base.OnExposeEvent (evnt);
		}
		
		protected override bool OnButtonReleaseEvent (Gdk.EventButton evnt)
		{
			Gdk.Rectangle rect;
			GetSize (out rect.Width, out rect.Height);
			
			if (evnt.X < 0 || evnt.Y < 0 || evnt.X > rect.Width || evnt.Y > rect.Height)
				Hide ();
			return base.OnButtonReleaseEvent (evnt);
		}
		
		protected override bool OnKeyReleaseEvent (Gdk.EventKey evnt)
		{
			if (evnt.Key == Gdk.Key.Escape)
				Hide ();
			return base.OnKeyReleaseEvent (evnt);
		}


		void DrawBackground (Context cr)
		{
			Gdk.Rectangle rect;
			GetSize (out rect.Width, out rect.Height);
			
			cr.MoveTo (BorderWidth + Radius, BorderWidth);
			cr.Arc (rect.Width - BorderWidth - Radius, BorderWidth + Radius, Radius, Math.PI * 1.5, Math.PI * 2);

			Cairo.PointD rightCurveStart = new Cairo.PointD (rect.Width - BorderWidth, rect.Height - BorderWidth - TailHeight);
			Cairo.PointD leftCurveStart = new Cairo.PointD (BorderWidth, rect.Height - BorderWidth - TailHeight);
			Cairo.PointD apex = new Cairo.PointD (rect.Width / 2 + horizontal_offset, rect.Height - BorderWidth);
			cr.LineTo (rightCurveStart);
			
			cr.CurveTo (rightCurveStart.X, rightCurveStart.Y + TailHeight * BaseCurviness, 
			            apex.X + 10 * Pointiness, apex.Y - TailHeight * TailCurviness, 
			            apex.X, apex.Y);
			
			cr.CurveTo (apex.X - 10 * Pointiness, 
			            apex.Y - TailHeight * TailCurviness, 
			            leftCurveStart.X, leftCurveStart.Y + TailHeight * BaseCurviness, 
			            leftCurveStart.X, leftCurveStart.Y);
			
			cr.Arc (BorderWidth + Radius, BorderWidth + Radius, Radius, Math.PI, Math.PI * 1.5);
			
			cr.Color = new Cairo.Color (0.1, 0.1, 0.1, .9);
			cr.FillPreserve ();
			
			cr.Color = new Cairo.Color (.2, .2, .2, .8);
			cr.Stroke ();
		}
	}
}