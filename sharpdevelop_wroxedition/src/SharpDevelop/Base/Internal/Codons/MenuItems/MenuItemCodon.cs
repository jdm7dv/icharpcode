// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

using Crownwood.Magic.Menus;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Conditions;

using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Commands;

namespace ICSharpCode.Core.AddIns.Codons
{
	[CodonName("MenuItem")]
	public class MenuItemCodon : AbstractCodon
	{
		[XmlMemberAttribute("label", IsRequired=true)]
		string label       = null;
		
		[XmlMemberAttribute("description")]
		string description = null;
		
		[XmlMemberAttribute("shortcut")]
		string shortcut    = null;
		
		[XmlMemberAttribute("icon")]
		string icon        = null;
		
		[XmlMemberAttribute("link")]
		string link        = null;
		
		public string Link {
			get {
				return link;
			}
			set {
				link = value;
			}
		}
		
		public string Label {
			get {
				return label;
			}
			set {
				label = value;
			}
		}
		
		public string Description {
			get {
				return description;
			}
			set {
				description = value;
			}
		}
		
		public string Icon {
			get {
				return icon;
			}
			set {
				icon = value;
			}
		}
		
		public string Shortcut {
			get {
				return shortcut;
			}
			set {
				shortcut = value;
			}
		}
		
		/// <summary>
		/// Creates an item with the specified sub items. And the current
		/// Condition status for this item.
		/// </summary>
		public override object BuildItem(object owner, ArrayList subItems, ConditionFailedAction action)
		{
			SdMenuCommand newItem = null;
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			if (Link != null) {
				newItem = new SdMenuCommand(stringParserService.Parse(Label), 
				                            new EventHandler(new MenuEventHandler(owner, 
				                                                                  Link.StartsWith("http") ? 
				                                                                  (IMenuCommand)new GotoWebSite(Link) :
				                                                                  new GotoLink(Link)).Execute));
			} else {
				object o = null;
				if (Class != null) {
					o = AddIn.CreateObject(Class);
				}
				if (o != null) {
					if (o is ISubmenuBuilder) {
						return ((ISubmenuBuilder)o).BuildSubmenu(owner);
					}
				
					if (o is IMenuCommand) {
						newItem = new SdMenuCommand(stringParserService.Parse(Label), new EventHandler(new MenuEventHandler(owner, (IMenuCommand)o).Execute));
					}
				}
			}
			
			if (newItem == null) {
				newItem = new SdMenuCommand(stringParserService.Parse(Label));
				if (subItems != null && subItems.Count > 0) {
					foreach (object item in subItems) {
						if (item is MenuCommand) {
							newItem.MenuCommands.Add((MenuCommand)item);
						} else {
							newItem.MenuCommands.AddRange((MenuCommand[])item);
						}
					}
				}
			}
			
			Debug.Assert(newItem != null);
			
			if (Icon != null) {
				ImageList imgList = new ImageList();
				ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
				imgList.Images.Add(resourceService.GetBitmap(Icon));
				newItem.ImageList  = imgList;
				newItem.ImageIndex = 0;
			}
			newItem.Description = stringParserService.Parse(description);
			
			if (Shortcut != null) {
				try {
					newItem.Shortcut = (Shortcut)((System.Windows.Forms.Shortcut.F1.GetType()).InvokeMember(Shortcut, BindingFlags.GetField, null, System.Windows.Forms.Shortcut.F1, new object[0]));
				} catch (Exception) {
					newItem.Shortcut = System.Windows.Forms.Shortcut.None;
				}
			}
			newItem.Enabled = action != ConditionFailedAction.Disable;
			return newItem;
		}
		
		class MenuEventHandler
		{
			IMenuCommand action;
			
			public MenuEventHandler(object owner, IMenuCommand action)
			{
				this.action       = action;
				this.action.Owner = owner;
			}
			
			public void Execute(object sender, EventArgs e)
			{
				action.Run();
			}
		}
	}
}
