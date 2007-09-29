// LBFrame.cs created with MonoDevelop
// User: dave at 11:15 AM 8/25/2007
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using Gtk;
using Gdk;

using Do.Core;
using Do.PluginLib;

namespace Do.UI
{
	
	public class LBDisplayText : Label
	{
		
		// const string displayFormat = " <big>{0}</big> \n {1} ";
		
		// Description only:
		const string displayFormat = "<span size=\"medium\"> {1} </span>";
		
		string highlight;
		string name, description;
		
		public LBDisplayText () : base ()
		{		
			Build ();
			highlight = name = description = "";
		}
		
		void Build ()
		{
			UseMarkup = true;
			Ellipsize = Pango.EllipsizeMode.End;
			Justify = Justification.Center;
			ModifyFg (StateType.Normal, Style.White);
		}
		
		public IObject DisplayObject {
			set {
				IObject displayObject;
				
				displayObject = value;
				name = description = highlight = "";
				if (displayObject != null) {
					name = displayObject.Name;
					description = displayObject.Description;
				}
				SetDisplayText (name, description);
			}
		}
		
		public void SetDisplayText (string name, string description) {
			this.name = (name == null ? "" : name);
			this.description = (description == null ? "" : description);
			highlight = "";
			UpdateText ();
		}
		
		public string Highlight {
			get { return highlight; }
			set {
				highlight = (value == null ? "" : value);
				UpdateText ();
			}
		}
		
		void UpdateText ()
		{
			Markup = string.Format (displayFormat, Util.UnderlineStringWithString (name, highlight), description);
		}
		
	}
}
