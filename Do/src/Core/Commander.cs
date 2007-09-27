// Main.cs created with MonoDevelop
// User: dave at 8:25 PM 8/16/2007
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//
// project created on 8/16/2007 at 8:25 PM

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using Do.DBusLib;
using Do.PluginLib;
using Do.PluginLib.Builtin;

namespace Do.Core
{
	public delegate void OnCommanderStateChange ();
	public delegate void VisibilityChangedHandler (bool visible);
	
	public enum CommanderState {
			Default,
			SearchingItems,
			ItemSearchComplete,
			SearchingCommands,
			CommandSearchComplete
	}
	
	public abstract class Commander : ICommander {
		
		protected Tomboy.GConfXKeybinder keybinder;
		
		private CommanderState state;
		
		public event OnCommanderStateChange SetSearchingItemsStateEvent;
		public event OnCommanderStateChange SetItemSearchCompleteStateEvent;
		public event OnCommanderStateChange SetSearchingCommandsStateEvent;
		public event OnCommanderStateChange SetCommandSearchCompleteStateEvent;
		public event OnCommanderStateChange SetDefaultStateEvent;
		
		public event VisibilityChangedHandler VisibilityChanged;
		
		private ItemManager itemManager;
		private CommandManager commandManager;
		private string itemSearchString, commandSearchString;
		
		private Item[] currentItems;
		private Command[] currentCommands;
		
		private int currentItemIndex;
		private int currentCommandIndex;
		
		public static ItemSource [] BuiltinItemSources {
			get {
				return new ItemSource [] {
					new ItemSource (new ApplicationItemSource ()),
					new ItemSource (new FirefoxBookmarkItemSource ()),
					// Index contents of Home (~) directory to 1 level
					new ItemSource (new DirectoryFileItemSource ("~", 1)),
					// Index contents of ~/Documents to 3 levels
					new ItemSource (new DirectoryFileItemSource ("~/Documents", 2)),
					// Index contents of ~/Desktop to 1 levels
					new ItemSource (new DirectoryFileItemSource ("~/Desktop", 1)),
				};
			}
		}
		
		public static Command [] BuiltinCommands {
			get {
				return new Command [] {
					new Command (new RunCommand ()),
					new Command (new OpenCommand ()),
					new Command (new RunInShellCommand ()),
					new Command (new DefineWordCommand ()),
					// new Command (new VoidCommand ()),
				};
			}
		}
		
		public Commander () {
			itemManager = new ItemManager ();
			commandManager = new CommandManager ();
			
			keybinder = new Tomboy.GConfXKeybinder ();
			
			SetSearchingItemsStateEvent = SetSearchingItemsState;
			SetItemSearchCompleteStateEvent = SetItemSearchCompleteState;
			SetSearchingCommandsStateEvent = SetSearchingCommandsState;
			SetCommandSearchCompleteStateEvent = SetCommandSearchCompleteState;
			SetDefaultStateEvent = SetDefaultState;
			VisibilityChanged = OnVisibilityChanged;
			
			LoadBuiltins ();
			LoadAssemblies ();
			SetupKeybindings ();
			State = CommanderState.Default;
		}
		
		public CommanderState State
		{
			get { return state; }
			set {
				this.state = value;
				switch (state) {
				case CommanderState.Default:
					SetDefaultStateEvent ();
					break;
				case CommanderState.SearchingItems:
					SetSearchingItemsStateEvent ();
					break;
				case CommanderState.ItemSearchComplete:
					SetItemSearchCompleteStateEvent ();
					break;
				case CommanderState.SearchingCommands:
					SetSearchingCommandsStateEvent ();
					break;
				case CommanderState.CommandSearchComplete:
					SetCommandSearchCompleteStateEvent ();
					break;
				}
			}
		}
		
		public CommandManager CommandManager
		{
			get { return commandManager; }
		}
		
		public ItemManager ItemManager
		{
			get { return itemManager; }
		}

		public string ItemSearchString
		{
			get { return itemSearchString; }
		}
		
		public string CommandSearchString
		{
			get { return commandSearchString; }
		}
		
		protected virtual void SetDefaultState ()
		{
			currentItems = new Item [0];
			currentCommands = new Command [0];
			currentItemIndex = -1;
			currentCommandIndex = -1;
			itemSearchString = "";
		}
		
		protected virtual void SetSearchingItemsState ()
		{
		}
		
		protected virtual void SetItemSearchCompleteState ()
		{
		}
		
		protected virtual void SetSearchingCommandsState ()
		{
		}
		
		protected virtual void SetCommandSearchCompleteState ()
		{
		}
		
		public Item [] CurrentItems
		{
			get { return currentItems; }
		}
		
		public Item CurrentItem
		{
			get {
				if (currentItemIndex >= 0)
					return currentItems [currentItemIndex];
				else
					return null;
			}
		}
		
		public Command [] CurrentCommands
		{
			get { return currentCommands; }
		}
		
		public Command CurrentCommand
		{
			get {
				if (this.currentCommandIndex >= 0) {
					return currentCommands [currentCommandIndex];
				} else {
					return null;
				}
			}
		}
		
		public int CurrentItemIndex
		{
			get { return currentItemIndex; }
			set {
				if (value < 0 || value >= currentItems.Length) {
					throw new IndexOutOfRangeException ();
				}
				if (currentItemIndex != value) {
					currentItemIndex = value;
					currentCommandIndex = 0;
					currentCommands = commandManager.CommandsForItem (CurrentItem, "");
				}
			}
		}
		
		public int CurrentCommandIndex
		{
			get { return currentCommandIndex; }
			set {
				if (value < 0 || value >= currentCommands.Length) {
					throw new IndexOutOfRangeException ();
				}
				currentCommandIndex = value;
			}
		}
		
		protected void LoadBuiltins ()
		{
			foreach (ItemSource source in BuiltinItemSources) {
				itemManager.AddItemSource (source);
			}
			foreach (Command command in BuiltinCommands) {
				commandManager.AddCommand (command);
			}
		}
		
		protected virtual void SetupKeybindings ()
		{
			keybinder.Bind ("/apps/do/bindings/activate",
						 "<Control>space",
						 OnActivate);
		}
		
		private void OnActivate (object sender, EventArgs args)
		{
			Show ();
		}
		
		protected void LoadAssemblies ()
		{
			/*
			Assembly currentAssembly;
			string appAssembly;
			
			appAssembly = "/home/dave/Current Documents/gnome-commander/gnome-commander-applications/bin/Debug/gnome-commander-applications.dll";
			currentAssembly = Assembly.LoadFile (appAssembly);
			
			foreach (Type type in currentAssembly.GetTypes ())
			foreach (Type iface in type.GetInterfaces ()) {
				if (iface == typeof (IItemSource)) {
					IItemSource source = currentAssembly.CreateInstance (type.ToString ()) as IItemSource;
					itemManager.AddItemSource (new ItemSource (source));
				}
				if (iface == typeof (ICommand)) {
					ICommand command = currentAssembly.CreateInstance (type.ToString ()) as ICommand;
					commandManager.AddCommand (new Command (command));
				}
			}
			*/
		}
		
		protected abstract void OnVisibilityChanged (bool visible);
			
		public void SearchItems (string itemSearchString)
		{
			State = CommanderState.SearchingItems;
			
			this.itemSearchString = itemSearchString;
			commandSearchString = "";
			currentItems = itemManager.ItemsForAbbreviation (itemSearchString);
			if (currentItems.Length == 0) {
				currentItems = new Item[] { new Item (new TextItem (itemSearchString)) };
			}
			
			// Update items and commands state.
			currentItemIndex = 0;
			currentCommandIndex = 0;
			currentCommands = commandManager.CommandsForItem (CurrentItem, "");
			
			State = CommanderState.ItemSearchComplete;
		}
		
		public void SearchCommands (string commandSearchString)
		{
			State = CommanderState.SearchingCommands;
			
			this.commandSearchString = commandSearchString;
			currentCommands = commandManager.CommandsForItem (CurrentItem, commandSearchString);
			
			// Update items and commands state.
			if (currentCommands.Length >  0) {
				CurrentCommandIndex = 0;
			} else {
				currentCommandIndex = -1;
			}
			
			State = CommanderState.CommandSearchComplete;
		}
		
		public void Execute ()
		{
			Item o;
			Command c;

			o = this.CurrentItem;
			c = this.CurrentCommand;
			if (o != null && c != null) {
				c.Perform (new IItem[] {o}, new IItem[] {});
			}
		}
		
		// ICommand members
		
		public void Show ()
		{
			VisibilityChanged (true);
		}
		
		public void Hide ()
		{
			VisibilityChanged (false);
		}
		
	}
}
