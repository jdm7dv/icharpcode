// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

using Crownwood.Magic.Menus;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace ICSharpCode.SharpDevelop.Gui.Components
{
	public class SharpDevelopSideTabItemFactory : ISideTabItemFactory
	{
		public AxSideTabItem CreateSideTabItem(string name)
		{
			return new SharpDevelopSideTabItem(name);
		}
		
		public AxSideTabItem CreateSideTabItem(string name, object tag)
		{
			return new SharpDevelopSideTabItem(name, tag);
		}
	}
	
	public class SharpDevelopSideBar : AxSideBar, IOwnerState
	{
		readonly static string contextMenuPath        = "/SharpDevelop/Workbench/SharpDevelopSideBar/ContextMenu";
		readonly static string sideTabContextMenuPath = "/SharpDevelop/Workbench/SharpDevelopSideBar/SideTab/ContextMenu";
		
		Point mousePosition;
		Point itemMousePosition;
		
		public Point ItemMousePosition {
			get {
				return itemMousePosition;
			}
		}
		public Point SideBarMousePosition {
			get {
				return mousePosition;
			}
		}
		
		[Flags]
		public enum SidebarState {
			Nothing       = 0,
			CanMoveUp     = 1,
			CanMoveDown   = 2,
			TabCanBeDeleted = 4,
			CanMoveItemUp = 8,
			CanMoveItemDown = 16
		}
		
		protected SidebarState internalState = SidebarState.TabCanBeDeleted;

		public System.Enum InternalState {
			get {
				return internalState;
			}
		}
		
		Hashtable   standardTabs = new Hashtable();
		
		public static SharpDevelopSideBar SideBar;
		
		public SharpDevelopSideBar(XmlElement el) : this()
		{
			SetOptions(el);
		}
		
		public SharpDevelopSideBar()
		{
			SideBar = this;
			
			SideTabItemFactory = new SharpDevelopSideTabItemFactory();
			
			MouseUp                  += new MouseEventHandler(SetContextMenu);
			sideTabContent.MouseUp   += new MouseEventHandler(SetItemContextMenu);
			
			/*
			deleteMenuHeaderItem = new IconMenuItem(mainwindow, resourceService.GetString("SideBarComponent.ContextMenu.DeleteTab"), new EventHandler(DeleteTabHeader));
			moveUpMenuHeaderItem = new IconMenuItem(mainwindow, resourceService.GetString("SideBarComponent.ContextMenu.MoveTabUp"), new EventHandler(MoveTabUp));
			moveDownMenuHeaderItem = new IconMenuItem(mainwindow, resourceService.GetString("SideBarComponent.ContextMenu.MoveTabDown"), new EventHandler(MoveTabDown));
			
			moveItemTabUpMenuHeaderItem = new IconMenuItem(mainwindow, resourceService.GetString("SideBarComponent.ContextMenu.MoveTabUp"), new EventHandler(MoveActiveTabUp));
			moveItemTabDownMenuHeaderItem = new IconMenuItem(mainwindow, resourceService.GetString("SideBarComponent.ContextMenu.MoveTabDown"), new EventHandler(MoveActiveTabDown));
			
			ContextMenu = new ContextMenu(new MenuItem[] {
				deleteMenuHeaderItem,
				new IconMenuItem(mainwindow, resourceService.GetString("SideBarComponent.ContextMenu.RenameTab"), new EventHandler(RenameTabHeader)),
				new IconMenuItem(mainwindow, "-"),
				new IconMenuItem(mainwindow, resourceService.GetString("SideBarComponent.ContextMenu.AddTab"), new EventHandler(AddTabHeader)),
				new IconMenuItem(mainwindow, "-"),
				moveUpMenuHeaderItem,
				moveDownMenuHeaderItem
			});
			
			
			
			sideTabContent.MouseMove += new MouseEventHandler(MoveItem);
			
			sideTabContent.ContextMenu = new ContextMenu(new MenuItem[] {
					new IconMenuItem(mainwindow, resourceService.GetString("SideBarComponent.ContextMenu.RenameTabItem"), new EventHandler(RenameTabItem)),
					new IconMenuItem(mainwindow, resourceService.GetString("SideBarComponent.ContextMenu.DeleteTabItem"), new EventHandler(DeleteTabItem)),
					new IconMenuItem(mainwindow, "-"),
					moveItemTabUpMenuHeaderItem,
					moveItemTabDownMenuHeaderItem
			});
			*/
			
			foreach (TextTemplate template in TextTemplate.TextTemplates) {
				AxSideTab tab = new AxSideTab(this, template.Name);
				tab.CanSaved  = false;
				foreach (TextTemplate.Entry entry in template.Entries)  {
					tab.Items.Add(SideTabItemFactory.CreateSideTabItem(entry.Display, entry.Value));
				}
				tab.CanBeDeleted = tab.CanDragDrop = false;
				standardTabs[tab] = true;
				Tabs.Add(tab);
			}
			sideTabContent.DoubleClick += new EventHandler(MyDoubleClick);
		}
		
		public void MyDoubleClick(object sender, EventArgs e)
		{
//			if (mainWindow.ActiveContentWindow == null) {
//				return;
//			}
//			string text = ActiveTab.SelectedItem.Tag.ToString();
//			
//			mainWindow.ActiveContentWindow.IEditable.ClipboardHandler.Delete(this, null);
//			
//			TextAreaControl sharptextarea = (TextAreaControl)mainWindow.ActiveContentWindow.ISdEditable;
//			
//			int curLineNr     = sharptextarea.Document.GetLineNumberForOffset(sharptextarea.Document.Caret.Offset);
//			sharptextarea.Document.Insert(sharptextarea.Document.Caret.Offset, text);
//			
//			sharptextarea.Document.Caret.Offset += text.Length;
//			
//			if (curLineNr != sharptextarea.Document.GetLineNumberForOffset(sharptextarea.Document.Caret.Offset)) {
//				sharptextarea.UpdateToEnd(curLineNr);
//			} else {
//				sharptextarea.UpdateLines(curLineNr, curLineNr);
//			}
		}
		
		public void PutInClipboardRing(string text)
		{
			foreach (AxSideTab tab in Tabs) {
				if (tab.Name == "Clipboard Ring") {
					tab.Items.Add("Text:" + text.Trim(), text);
					if (tab.Items.Count > 20) {
						tab.Items.RemoveAt(0);
					}
					Refresh();
					return;
				}
			}
		}
		
		////////////////////////////////////////////////////////////////////////////
		// Tab Context Menu
		
		void SetDeletedState(AxSideTab tab)
		{
			if (tab.CanBeDeleted) {
				internalState |= SidebarState.TabCanBeDeleted;
			} else {
				internalState = internalState & ~SidebarState.TabCanBeDeleted;
			}
		}
		
		void SetContextMenu(object sender, MouseEventArgs e)
		{
			ExitRenameMode();
			
			int index = GetTabIndexAt(e.X, e.Y);
			if (index >= 0) {
				AxSideTab tab = Tabs[index];
				
				SetDeletedState(tab);
				
				if (index > 0) {
					internalState |= SidebarState.CanMoveUp;
				} else {
					internalState = internalState & ~SidebarState.CanMoveUp;
				}
				
				if (index < Tabs.Count - 1) {
					internalState |= SidebarState.CanMoveDown;
				} else {
					internalState = internalState & ~(SidebarState.CanMoveDown);
				}
				Tabs.DragOverTab = tab;				
				Refresh();
				Tabs.DragOverTab = null;
			}
			
			if (e.Button == MouseButtons.Right) {
				MenuCommand[] contextMenu = (MenuCommand[])(AddInTreeSingleton.AddInTree.GetTreeNode(contextMenuPath).BuildChildItems(this)).ToArray(typeof(MenuCommand));
				PopupMenu popup = new PopupMenu();
				popup.MenuCommands.AddRange(contextMenu);
				popup.TrackPopup(PointToScreen(new Point(e.X, e.Y)));
			}
		}
		
		void SetItemContextMenu(object sender, MouseEventArgs e)
		{
			ExitRenameMode();
			if (e.Button == MouseButtons.Right) {
				int index = Tabs.IndexOf(ActiveTab);
				
				if (index > 0) {
					internalState |= SidebarState.CanMoveUp;
				} else {
					internalState = internalState & ~SidebarState.CanMoveUp;
				}
				
				if (index < Tabs.Count - 1) {
					internalState |= SidebarState.CanMoveDown;
				} else {
					internalState = internalState & ~(SidebarState.CanMoveDown);
				}
				
				Tabs.DragOverTab = ActiveTab;
				Refresh();
				Tabs.DragOverTab = null;
			}
			
			if (e.Button == MouseButtons.Right) {
				// set moveup/down states correctly
				SetDeletedState(ActiveTab);
				
				int index = ActiveTab.Items.IndexOf(ActiveTab.SelectedItem);
				if (index > 0) {
					internalState |= SidebarState.CanMoveItemUp;
				} else {
					internalState = internalState & ~(SidebarState.CanMoveItemUp);
				}
				
				if (index < ActiveTab.Items.Count - 1) {
					internalState |= SidebarState.CanMoveItemDown;
				} else {
					internalState = internalState & ~(SidebarState.CanMoveItemDown);
				}
				
				MenuCommand[] contextMenu = (MenuCommand[])(AddInTreeSingleton.AddInTree.GetTreeNode(sideTabContextMenuPath).BuildChildItems(this)).ToArray(typeof(MenuCommand));
				
				PopupMenu popup = new PopupMenu();
				PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
				popup.Style = (Crownwood.Magic.Common.VisualStyle)propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.VisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);
				popup.MenuCommands.AddRange(contextMenu);
				popup.TrackPopup(sideTabContent.PointToScreen(new Point(e.X, e.Y)));
			}
		}
		
		void MoveItem(object sender, MouseEventArgs e)
		{
			itemMousePosition = new Point(e.X, e.Y);
		}
		
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			mousePosition = new Point(e.X, e.Y);
		}
		
		void SetOptions(XmlElement el)
		{
			foreach (XmlElement sideTabEl in el.ChildNodes) {
				AxSideTab tab = new AxSideTab(this, sideTabEl.Attributes["text"].InnerText);
				if (tab.Name == el.Attributes["activetab"].InnerText) {
					ActiveTab = tab;
				} else {
					if (ActiveTab == null) {
						ActiveTab = tab;
					}
				}
				if (tab.Name == "Clipboard Ring") {
					tab.CanBeDeleted = false;
					tab.CanDragDrop  = false;
				}
				foreach (XmlElement sideTabItemEl in sideTabEl.ChildNodes) {
					tab.Items.Add(SideTabItemFactory.CreateSideTabItem(sideTabItemEl.Attributes["text"].InnerText,
					                              sideTabItemEl.Attributes["value"].InnerText));
				}
				Tabs.Add(tab);
			}
			

		}
		public XmlElement ToXmlElement(XmlDocument doc)
		{
			if (doc == null) {
				throw new ArgumentNullException("doc");
			}
			XmlElement el = doc.CreateElement("SideBar");
			
			XmlAttribute attr = doc.CreateAttribute("activetab");
			attr.InnerText  = ActiveTab.Name;
			el.Attributes.Append(attr);
			
			foreach (AxSideTab tab in Tabs) 
			if (tab.CanSaved) {
				if (standardTabs[tab] == null) {
					XmlElement child = doc.CreateElement("SideTab");
					
					attr = doc.CreateAttribute("text");
					attr.InnerText  = tab.Name;
					child.Attributes.Append(attr);
					
					foreach (AxSideTabItem item in tab.Items) {
						XmlElement itemChild = doc.CreateElement("SideTabItem");
						
						attr = doc.CreateAttribute("text");
						attr.InnerText  = item.Name;
						itemChild.Attributes.Append(attr);
						
						attr = doc.CreateAttribute("value");
						attr.InnerText  = item.Tag.ToString();
						itemChild.Attributes.Append(attr);
						
						child.AppendChild(itemChild);
					}
					el.AppendChild(child);
				}
			}
			
			return el;
		}
		
	}
}
