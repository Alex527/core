/* UniverseManager.cs
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
using System.Reflection;
using System.Collections.Generic;

using Do;
using Do.Universe;

namespace Do.Core
{	
	
	public class UniverseManager
	{
		
		Dictionary<string, IObject[]> firstResults;
		Dictionary<int, IObject> universe;
		
		List<DoItemSource> doItemSources;
		List<DoCommand> doCommands;
		
		public UniverseManager()
		{
			universe = new Dictionary<int, IObject> ();
			doItemSources = new List<DoItemSource> ();
			doCommands = new List<DoCommand> ();
			firstResults = new Dictionary<string, IObject[]> ();
			
			LoadBuiltins ();
			LoadAddins ();
			BuildUniverse ();
			BuildFirstResults ();	
		}
		
		public IObject[] ChildrenOfObject (IObject parent)
		{
			List<IObject> children;
			
			children = new List<IObject> ();
			if (parent is DoItem) {
				foreach (DoItemSource source in doItemSources) {
					foreach (IItem child in source.ChildrenOfItem (parent as DoItem)) {
						children.Add (child);
					}
				}
			}
			
			return children.ToArray ();
		}

		protected void LoadBuiltins ()
		{
			LoadAssembly (typeof (IItem).Assembly);
		}

		protected void LoadAddins ()
		{
			List<string> addin_dirs;
			
			addin_dirs = new List<string> ();
			addin_dirs.Add ("~/.do/addins".Replace ("~",
				   Environment.GetFolderPath (Environment.SpecialFolder.Personal)));
			
			foreach (string addin_dir in addin_dirs) {
				string[] files;
				
				files = null;
				try {
					files = System.IO.Directory.GetFiles (addin_dir);
				} catch (Exception e) {
					Log.Error ("Could not read addins directory {0}: {1}", addin_dir, e.Message);
					continue;
				}
				
				foreach (string file in files) {
					Assembly addin;
					
					if (!file.EndsWith (".dll")) continue;
					try {
						addin = Assembly.LoadFile (file);
						LoadAssembly (addin);
					} catch (Exception e) {
						Log.Error ("Do encountered and error while trying to load addin {0}: {1}", file, e.Message);
						continue;
					}
				}
			}
		}
		
		private void LoadAssembly (Assembly addin)
		{
			if (addin == null) return;
			
			foreach (Type type in addin.GetTypes ()) {
				if (type.IsAbstract) continue;
				if (type == typeof(VoidCommand)) continue;
				
				foreach (Type iface in type.GetInterfaces ()) {
					if (iface == typeof (IItemSource)) {
						IItemSource source;
						
						source = System.Activator.CreateInstance (type) as IItemSource;
						doItemSources.Add (new DoItemSource (source));
						Log.Info ("Successfully loaded \"{0}\" Item Source.", source.Name);
					}
					if (iface == typeof (ICommand)) {
						ICommand command;
						
						command = System.Activator.CreateInstance (type) as ICommand;
						doCommands.Add (new DoCommand (command));
						Log.Info ("Successfully loaded \"{0}\" Command.", command.Name);
					}
				}
			}
		}

		private void BuildFirstResults () 
		{			
			List<IObject> results;
			RelevanceSorter comparer;

			//For each starting character add every matching object from the universe to
			//the firstResults dictionary with the key of the character
			for (char keypress = 'a'; keypress < 'z'; keypress++) {
				results = new List<IObject> (universe.Values);
				comparer = new RelevanceSorter (keypress.ToString ());
				firstResults[keypress.ToString ()] = comparer.NarrowResults (results).ToArray ();
			}
		}
		
		private void BuildUniverse () {
			// Hash items.
			foreach (DoItemSource source in doItemSources) {
				foreach (DoItem item in source.Items) {
					universe[item.GetHashCode ()] = item;
				}
			}
			// Hash commands.
			foreach (DoCommand command in doCommands) {
				universe[command.GetHashCode ()] = command;
			}
		}
		
		public SearchContext Search (SearchContext context)
		{
			List<IObject> results;
			SearchContext last;

			// Do backwards search.	
			// This is very tricky because it can mess up on 
			last = context.LastContext;
			if (last != null && last.LastContext != null &&
					last.SearchString.Length > 0 &&
					last.SearchString.StartsWith (last.LastContext.SearchString) &&
					last.LastContext.SearchString == context.SearchString &&
					last.LastContext.FirstObject == last.LastContext.FirstObject) {
				return last.LastContext;
			} 

			// Get the results based on the search string
			results = GenerateUnfilteredList (context);		
			if (results.Count == 0) {
				results.Add (new DoItem (new TextItem (context.SearchString)));
			}

			// Filter results based on the required type
			results = FilterResultsByType (results, context.SearchTypes);
			// Filter results based on object dependencies
			results = FilterResultsByDependency (results, context.FirstObject);		
			context.Results = results.ToArray ();
			
			// Keep a stack of incremental results.
			last = context;
			context = context.Clone ();
			context.LastContext = last;
			return context;
		}
		
		private List<IObject> GenerateUnfilteredList (SearchContext context) 
		{
			string query;
			RelevanceSorter comparer;
			List<IObject> results;
			
			query = context.SearchString.ToLower ();
		
			// Special handling for commands that take only text:
			if (context.FirstObject != null && context.FirstObject is DoCommand &&
			    (context.FirstObject as DoCommand).AcceptsOnlyText) {
				results = new List<IObject> ();
				results.Add (new DoItem (new TextItem (query == "" ? "Enter Text" : context.SearchString)));
				return results;
			}

			//If this is the initial search for the all the corresponding items/commands for the first object
			/// we don't need to filter based on search string
			if (query == "" && context.FirstObject != null) {
				results = new List<IObject> ();
				
				// If command, just grab the commands for the item
				if (ContainsType (context.SearchTypes, typeof (ICommand))) {
					foreach (DoCommand command in CommandsForItem (context.FirstObject as DoItem)) {
						results.Add (command);
					}
				}
				else if (ContainsType (context.SearchTypes, typeof (IItem))) {
					results.AddRange (universe.Values);
				}

			} else {

				// We are given results (e.g. begin browsing)
				if (context.ResultsIn != null) {
					results = new List<IObject> (context.ResultsIn);
				}
				
				// We can build on the last results.
				// example: searched for "f" then "fi"
				else if (context.LastContext != null) {
					comparer = new RelevanceSorter (query);
					results = new List<IObject> (context.LastContext.Results);
					results = comparer.NarrowResults (results);
				}

			
				// If someone typed a single key, BOOM we're done.
				else if (firstResults.ContainsKey (query)) {
					results = new List<IObject> (firstResults[query]);
					
				}

				// Or we just have to do an expensive search...
				// This is the current behavior on first keypress.
				else {
					results = new List<IObject> ();
					results.AddRange (universe.Values);
					comparer = new RelevanceSorter (query);
					results.Sort (comparer);
				}
			}
			return results;
		}
			
		
		private List<IObject> FilterResultsByType (List<IObject> results, Type[] acceptableTypes) 
		{
			List<IObject> new_results;
			
			new_results = new List<IObject> ();			
			//Now we look through the list and add an object when its type belongs in acceptableTypes
			foreach (IObject result in results) {
				List<Type> implementedTypes = DoObject.GetAllImplementedTypes (result);
				foreach (Type type in acceptableTypes) {
					if (implementedTypes.Contains (type)) {
						new_results.Add (result);
						break;
					}
				}
			}
			return new_results;
		}
		
		private List<IObject> FilterResultsByDependency (List<IObject> results, IObject constraint)
		{
			List <IObject> filtered_results;
			
			if (constraint == null) return results;
			filtered_results = new List<IObject> ();
			
			if (constraint is DoCommand) {
				foreach (DoItem item in results) {
					// If the constraint is a DoCommand, add the result if it's supported.
					if ((constraint as DoCommand).SupportsItem (item)) {
						filtered_results.Add (item);
					}
				}
			}
			else if (constraint is DoItem) {
				filtered_results = results;
			}
			return filtered_results;
		}
				
		
		//Function to determine whether a type array contains a type
		private bool ContainsType (Type[] typeArray, Type checkType) {
			foreach (Type type in typeArray) {
				if (type.Equals (checkType))
					return true;
			}
			return false;
		}

		List<DoCommand> CommandsForItem (DoItem item)
		{
			List<DoCommand> item_commands;

			item_commands = new List<DoCommand> ();
			foreach (DoCommand command in doCommands) {
				if (command.SupportsItem (item)) {
					item_commands.Add (command);
				}
			}
			return item_commands;
		}
		
	}
}
