/* Shortcut.cs
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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Env = System.Environment;

using Do.Platform;

namespace Do
{
    class Shortcut {
            public string ShortcutName; // name of the shortcut
            public string FriendlyName; // display name of the shortcut
            public BitArray Flags; // future flags (possibly global 
            public ShortcutCallback Callback; // callback function for this shortcut
            
            public Shortcut(string name, string friendly, ShortcutCallback cb, BitArray flags)
            {
                ShortcutName = name;
                FriendlyName = friendly;
                Flags = flags;
                Callback = cb;

            }

            public Shortcut(string name, string friendly, ShortcutCallback cb)
            {
                ShortcutName = name;
                FriendlyName = friendly;
                Flags = new BitArray(32, false); // ummm... this should be fixed, I guess. TODO: fix it
                Callback = cb;

            }
    }
}


