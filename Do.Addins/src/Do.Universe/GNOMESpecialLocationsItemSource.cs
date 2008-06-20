//  GNOMESpecialLocationsItemSource.cs
//
//  GNOME Do is the legal property of its developers, whose names are too numerous
//  to list here.  Please refer to the COPYRIGHT file distributed with this
//  source distribution.
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Collections.Generic;

using Do.Addins;

namespace Do.Universe {

	public class GNOMESpecialLocationsItemSource : IItemSource {
		
		class GNOMEURIItem : IURIItem {
			protected string uri, name, icon;
			
			public GNOMEURIItem (string uri, string name, string icon)
			{
				this.uri = uri;
				this.name = name;
				this.icon = icon;
			}
			
			virtual public string Name { get { return name; } }
			virtual public string Description { get { return URI; } }
			virtual public string Icon { get { return icon; } }
			virtual public string URI { get { return uri; } }
		}
			
		public string Name { get { return "GNOME Special Locations"; } }
		public string Description { get {
			return "Special locations in GNOME, such as Computer and Network.";
		} }
		public string Icon { get { return "user-home"; } }

		public Type[] SupportedItemTypes
		{
			get {
				return new Type[] {
					typeof (IURIItem),
				};
			}
		}
		
		public ICollection<IItem> Items
		{
			get {
				return new IItem [] {
					new GNOMETrashFileItem (),
					new GNOMEURIItem ("computer:///", "Computer", "computer"),
					new GNOMEURIItem ("network://", "Network", "network"),
				};
			}

		}
		
		public ICollection<IItem> ChildrenOfItem (IItem item)
		{
			return null;
		}
		
		public void UpdateItems ()
		{
		}

	}
	
	class GNOMETrashFileItem : IFileItem, IOpenableItem {
		
		public string Path {
			get { 
				return Paths.Combine (
					Paths.ReadXdgUserDir ("XDG_DATA_HOME", ".local/share"),
					"Trash/files");
			}
		}

		public string Name {
			get { return "Trash"; }
		}

		public string Description {
			get { return "Trash"; }
		}

		public string URI {
			get { return "trash://"; }
		}

		public string Icon
		{
			get {
				if (Directory.Exists (Path) &&
					Directory.GetFileSystemEntries (Path).Length > 0) {
					return "user-trash-full";
				} else {
					return "user-trash";
				}
			}
		}

		public void Open ()
		{
			// Override Open to open trash:// instead of ~/.Trash.
			Util.Environment.Open ("trash://");
		}
	}
}
