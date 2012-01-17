// 
//  TestPositionWindow.cs
//  
//  Author:
//       Christopher James Halse Rogers <raof@ubuntu.com>
// 
//  Copyright © 2012 Christopher James Halse Rogers <raof@ubuntu.com>
// 
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
// 
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using NUnit.Framework;

namespace Do.Interface.Linux
{
	[TestFixture()]
	public class TestPositionWindow
	{
		[Test()]
		public void TestSingleHeadPositionCalc ()
		{
			var positioner = new PositionWindow (null, null);
			var calculatePosition = positioner.GetType ().GetMethod ("CalculateBasePosition",
			                                                         System.Reflection.BindingFlags.NonPublic |
			                                                         System.Reflection.BindingFlags.Instance);
			// Single-head displays have an origin of (0,0)
			Gdk.Rectangle screen = new Gdk.Rectangle (0, 0, 1024, 768);
			// We only care about width and height here
			Gdk.Rectangle window = new Gdk.Rectangle (0, 0, 200, 100);

			object[] parameters = new object[] {screen, window, new Gdk.Rectangle ()};
			Gdk.Rectangle result = (Gdk.Rectangle)calculatePosition.Invoke (positioner, parameters);

			Assert.AreEqual (412, result.X);
			Assert.AreEqual (267, result.Y);
		}

		[Test]
		public void TestHorizMultiHeadPositionCalc ()
		{
			var positioner = new PositionWindow (null, null);
			var calculatePosition = positioner.GetType ().GetMethod ("CalculateBasePosition",
			                                                         System.Reflection.BindingFlags.NonPublic |
			                                                         System.Reflection.BindingFlags.Instance);
			// Single-head displays have an origin of (0,0)
			Gdk.Rectangle screen_one = new Gdk.Rectangle (0, 0, 1024, 768);
			Gdk.Rectangle screen_two = new Gdk.Rectangle (screen_one.Width, 0, 1024, 768);
			// We only care about width and height here
			Gdk.Rectangle window = new Gdk.Rectangle (0, 0, 200, 100);

			object[] parameters = new object[] {screen_one, window, new Gdk.Rectangle ()};
			Gdk.Rectangle screen_one_result = (Gdk.Rectangle)calculatePosition.Invoke (positioner, parameters);
			parameters = new object[] {screen_two, window, new Gdk.Rectangle ()};
			Gdk.Rectangle screen_two_result = (Gdk.Rectangle)calculatePosition.Invoke (positioner, parameters);

			Assert.AreEqual (screen_one_result.X + screen_one.Width, screen_two_result.X);
			Assert.AreEqual (screen_one_result.Y, screen_two_result.Y);
		}

		[Test]
		public void TestVertMultiHeadPositionCalc ()
		{
			var positioner = new PositionWindow (null, null);
			var calculatePosition = positioner.GetType ().GetMethod ("CalculateBasePosition",
			                                                         System.Reflection.BindingFlags.NonPublic |
			                                                         System.Reflection.BindingFlags.Instance);
			// Single-head displays have an origin of (0,0)
			Gdk.Rectangle screen_one = new Gdk.Rectangle (0, 0, 1024, 768);
			Gdk.Rectangle screen_two = new Gdk.Rectangle (0, screen_one.Height, 1024, 768);
			// We only care about width and height here
			Gdk.Rectangle window = new Gdk.Rectangle (0, 0, 200, 100);

			object[] parameters = new object[] {screen_one, window, new Gdk.Rectangle ()};
			Gdk.Rectangle screen_one_result = (Gdk.Rectangle)calculatePosition.Invoke (positioner, parameters);
			parameters = new object[] {screen_two, window, new Gdk.Rectangle ()};
			Gdk.Rectangle screen_two_result = (Gdk.Rectangle)calculatePosition.Invoke (positioner, parameters);

			Assert.AreEqual (screen_one_result.X, screen_two_result.X);
			Assert.AreEqual (screen_one_result.Y + screen_one.Height, screen_two_result.Y);
		}
	}
}

