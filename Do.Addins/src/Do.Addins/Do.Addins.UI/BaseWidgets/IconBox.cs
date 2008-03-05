/* IconBox.cs
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

using Gtk;
using Gdk;

using Do.Addins;
using Do.Universe;

namespace Do.Addins.UI
{
	public class IconBox : Frame
	{
		const string captionFormat = "{0}";
		const string highlightFormat = "<span weight=\"bold\" underline=\"single\">{0}</span>";

		protected bool isFocused;

		protected string caption, iconName;
		protected Pixbuf pixbuf, emptyPixbuf;
		protected int iconSize;

		protected VBox vbox;
		protected Gtk.Image image;
		protected Label label;

		protected double focusedTransparency = 0.4;
		protected double unfocusedTransparency = 0.1;

		public IconBox (int iconSize) : base ()
		{
			this.iconSize = iconSize;
			Build ();
		}
		
		protected virtual void Build ()
		{
			Alignment label_align;

			caption = "";
			pixbuf = emptyPixbuf;

			vbox = new VBox (false, 4);
			vbox.BorderWidth = 6;
			Add (vbox);
			vbox.Show ();

			emptyPixbuf = new Pixbuf (Colorspace.Rgb, true, 8, iconSize, iconSize);
			emptyPixbuf.Fill (uint.MinValue);

			image = new Gtk.Image ();
			vbox.PackStart (image, false, false, 0);
			image.Show ();

			label = new Label ();
			label.Ellipsize = Pango.EllipsizeMode.End;
			label.ModifyFg (StateType.Normal, Style.White);
			label_align = new Alignment (1.0F, 0.0F, 0, 0);
			label_align.SetPadding (0, 2, 2, 2);
			label_align.Add (label);
			vbox.PackStart (label_align, false, false, 0);
			label.Show ();
			label_align.Show ();

			image.SetSizeRequest (iconSize, iconSize);
			label.SetSizeRequest (iconSize / 4 * 5, -1);
			// SetSizeRequest (iconSize * 2, iconSize * 2);

			DrawFrame = DrawFill = true;
			FrameColor = FillColor = new Color (byte.MaxValue, byte.MaxValue, byte.MaxValue);

			Realized += OnRealized;
			UpdateFocus ();
		}

		public virtual void Clear ()
		{
			Pixbuf = emptyPixbuf;
			Caption = "";
		}

		protected virtual void OnRealized (object o, EventArgs args)
		{
			UpdateFocus ();
		}

		public bool IsFocused
		{
			get { return isFocused; }
			set {
				isFocused = value;
				UpdateFocus ();
			}
		}

		public string Caption
		{
			get { return caption; }
			set {
				caption = value ?? "";
				label.Markup = string.Format (captionFormat, Util.Appearance.MarkupSafeString (caption));
			}
		}

		public string Icon
		{
			set {
				iconName = value;
				Pixbuf = IconProvider.PixbufFromIconName (value, iconSize);
			}
		}

		public Pixbuf Pixbuf
		{
			get { return pixbuf; }
			set {
				pixbuf = value ?? emptyPixbuf;
				image.Pixbuf = pixbuf;
			}
		}

		public IObject DisplayObject
		{
			set {
				string name, icon;

				icon = null;
				name = null;
				if (value != null) {
					icon = value.Icon;
					name = value.Name;
				}				
				Icon = icon;
				Caption = name;
			}
		}

		public string Highlight
		{
			set {
				string highlight;

				if (value != null) {
					highlight = Util.FormatCommonSubstrings (caption, value, highlightFormat);
				} else {
					highlight = caption;
				}
				Caption = highlight;
			}
		}

		protected virtual void UpdateFocus ()
		{
			FrameAlpha = FillAlpha = (isFocused ? focusedTransparency : unfocusedTransparency);
		}

	}
}