// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

using Crownwood.Magic.Menus;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Conditions;


using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Components;

namespace ICSharpCode.Core.AddIns.Codons
{
	public sealed class PopupMenuManager
	{
		public static void ShowPopupMenu(object owner, string addInTreePath, int x, int y)
		{
			ArrayList buildItems = AddInTreeSingleton.AddInTree.GetTreeNode(addInTreePath).BuildChildItems(owner);
			ArrayList items      = new ArrayList();
			
			AddItems(items, buildItems);
			
			PopupMenu popup = new PopupMenu();
			popup.MenuCommands.AddRange((MenuCommand[])items.ToArray(typeof(MenuCommand)));
			popup.TrackPopup(new System.Drawing.Point(x, y));
		}
		
		static void AddItems(ArrayList items, IList itemsToAdd) 
		{
			foreach (object item in itemsToAdd) {
				if (item is MenuCommand[]) {
					AddItems(items, (MenuCommand[])item);
				} else {
					items.Add(item);
				}
			}
		}
	}
}
