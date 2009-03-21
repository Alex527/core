// Util.cs
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
using System.Diagnostics;

using Do.Universe;
using Do.Interface.CairoUtils;

using Cairo;
using Gtk;
using Gdk;

using Docky.Utilities;

namespace Docky.Interface
{

	public enum IconSource {
		Statistics,
		Custom,
		Application,
		Unknown,
	}
	
	public enum ScalingType {
		None = 0,
		Upscaled,
		Downscaled,
		HighLow,
	}
	
	public delegate void UpdateRequestHandler (object sender, UpdateRequestArgs args);
	public delegate void DockItemsChangedHandler (IEnumerable<AbstractDockItem> items);
	
	public static class Util
	{
		const int Height = 35;
		const string FormatString = "<span weight=\"600\">{0}</span>";
		
		public static Surface GetBorderedTextSurface (string text, int maxWidth, Surface similar) 
		{
			return GetBorderedTextSurface (text, maxWidth, similar, DockOrientation.Bottom);
		}
		
		/// <summary>
		/// Gets a surface containing a transparent black rounded rectangle with the provided text on top.
		/// </summary>
		/// <param name="text">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="maxWidth">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="similar">
		/// A <see cref="Surface"/>
		/// </param>
		/// <returns>
		/// A <see cref="Surface"/>
		/// </returns>
		public static Surface GetBorderedTextSurface (string text, int maxWidth, Surface similar, 
		                                              DockOrientation orientation)
		{
			Surface sr;
			sr = similar.CreateSimilar (similar.Content, maxWidth, Height);
			
			Context cr = new Context (sr);
			
			TextRenderContext textContext = new TextRenderContext (cr, string.Format (FormatString, text), maxWidth - 18);
			textContext.Alignment = Pango.Alignment.Center;
			
			Gdk.Rectangle textArea = Core.DockServices.DrawingService.TextPathAtPoint (textContext);
			cr.NewPath ();
			
			int localHeight = textArea.Height;
			cr.SetRoundedRectanglePath (textArea.X + .5,  .5, textArea.Width + 20 - 1,  localHeight + 10 - 1, 5);
			
			cr.Color = new Cairo.Color (0.1, 0.1, 0.1, .75);
			cr.FillPreserve ();

			cr.Color = new Cairo.Color (1, 1, 1, .4);
			cr.LineWidth = 1;
			cr.Stroke ();

			cr.Translate(1,1);
			
			textContext.LeftCenteredPoint = new Gdk.Point (10, (localHeight + 10) / 2);
			Core.DockServices.DrawingService.TextPathAtPoint (textContext);
			cr.Color = new Cairo.Color (0, 0, 0, 0.6);
			cr.Fill ();
			
			cr.Translate(-1,-1);

			Core.DockServices.DrawingService.TextPathAtPoint (textContext);
			cr.Color = new Cairo.Color (1, 1, 1);
			cr.Fill ();

			(cr as IDisposable).Dispose ();
			return sr;
		}
	}
}
