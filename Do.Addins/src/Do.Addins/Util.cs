/* Util.cs
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

using Do.Universe;

namespace Do.Addins
{
	public delegate void EnvironmentOpenDelegate (string item);
	public delegate void PopupMainMenuAtPositionDelegate (int x, int y);
	public delegate Gdk.Pixbuf PixbufFromIconNameDelegate (string icon_name, int size);
	public delegate string StringTransformationDelegate (string old);
	public delegate string FormatCommonSubstringsDelegate (string main, string highlight, string format);
	public delegate void PresentWindowDelegate (Gtk.Window window);
	public delegate void DoEventKeyDelegate (Gdk.EventKey key);
	
	/// <summary>
	/// Useful functionality for plugins. See <see cref="Do.Util"/>.
	/// </summary>
	public class Util
	{
		
		public static FormatCommonSubstringsDelegate FormatCommonSubstrings;
		
		public static class Environment
		{
			public static EnvironmentOpenDelegate Open;
		}
		
		public class Appearance
		{
			public static PresentWindowDelegate PresentWindow;
			public static PixbufFromIconNameDelegate PixbufFromIconName;
			public static StringTransformationDelegate MarkupSafeString;
			public static PopupMainMenuAtPositionDelegate PopupMainMenuAtPosition;
			
			public static void RGBToHSV (ref byte r, ref byte g, ref byte b)
			{
				// Ported from Murrine Engine.
				double red, green, blue;
				double hue = 0, lum, sat;
				double max, min;
				double delta;
				
				red = (double) r;
				green = (double) g;
				blue = (double) b;
				
				max = Math.Max (red, Math.Max (blue, green));
				min = Math.Min (red, Math.Min (blue, green));
				delta = max - min;
				lum = max / 255.0 * 100.0;
				
				if (Math.Abs (delta) < 0.0001) {
					lum = 0;
					sat = 0;
				} else {
					sat = (delta / max) * 100;
					
					if (red == max)   hue = (green - blue) / delta;
					if (green == max) hue = 2 + (blue - red) / delta;
					if (blue == max)  hue = 4 + (red - green) / delta;
					
					hue *= 60;
					if (hue <= 0) hue += 360;
				}
				r = (byte) hue;
				g = (byte) sat;
				b = (byte) lum;
			}
			
			public static void HSVToRGB (ref byte hue, ref byte sat, ref byte val)
			{
				double h, s, v;
				double r = 0, g = 0, b = 0;

				h = (double) hue;
				s = (double) sat / 100;
				v = (double) val / 100;

				if (s == 0) {
					r = v;
					g = v;
					b = v;
				} else {
					int secNum;
					double fracSec;
					double p, q, t;
					
					secNum = (int) Math.Floor(h / 60);
					fracSec = h/60 - secNum;

					p = v * (1 - s);
					q = v * (1 - s*fracSec);
					t = v * (1 - s*(1 - fracSec));

					switch (secNum) {
						case 0:
							r = v;
							g = t;
							b = p;
							break;
						case 1:
							r = q;
							g = v;
							b = p;
							break;
						case 2:
							r = p;
							g = v;
							b = t;
							break;
						case 3:
							r = p;
							g = q;
							b = v;
							break;
						case 4:
							r = t;
							g = p;
							b = v;
							break;
						case 5:
							r = v;
							g = p;
							b = q;
							break;
					}
				}
				hue = Convert.ToByte(r*255);
				sat = Convert.ToByte(g*255);
				val = Convert.ToByte(b*255);
			}
		}
	}
}
