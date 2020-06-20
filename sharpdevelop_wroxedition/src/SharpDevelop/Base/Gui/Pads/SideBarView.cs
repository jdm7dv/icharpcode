// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Drawing;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class SideBarView : IPadContent, IDisposable
	{
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		public Control Control {
			get {
				return sideBar;
			}
		}
		
		public string Title {
			get {
				return resourceService.GetString("MainWindow.Windows.ToolbarLabel");
			}
		}
		
		public Bitmap Icon {
			get {
				return resourceService.GetBitmap("Icons.16x16.ToolBar");
			}
		}
		
		public void RedrawContent()
		{
			OnTitleChanged(null);
			OnIconChanged(null);
		}
		
		public void Dispose()
		{
			SaveSideBarViewConfig();
			sideBar.Dispose();
		}
		
		public static SharpDevelopSideBar sideBar = null;
		public SideBarView()
		{
			try {
				XmlDocument doc = new XmlDocument();
				PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
				doc.Load(propertyService.ConfigDirectory + "SideBarConfig.xml");
				sideBar = new SharpDevelopSideBar(doc.DocumentElement["SideBar"]);
			} catch (Exception) {
				sideBar = new SharpDevelopSideBar();
				AxSideTab tab = new AxSideTab(sideBar, "General");
				sideBar.Tabs.Add(tab);
				sideBar.ActiveTab = tab;
				tab = new AxSideTab(sideBar, "Clipboard Ring");
				tab.CanBeDeleted = false;
				tab.CanDragDrop  = false;
				sideBar.Tabs.Add(tab);
			}
			
			sideBar.Dock = DockStyle.Fill;
		}
		
		public static void PutInClipboardRing(string text)
		{
			if (sideBar != null) {
				sideBar.PutInClipboardRing(text);
			}
		}
		
		public void SaveSideBarViewConfig()
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<SideBarConfig/>");
			doc.DocumentElement.AppendChild(sideBar.ToXmlElement(doc));
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			fileUtilityService.ObservedSave(new NamedFileOperationDelegate(doc.Save), 
			                                propertyService.ConfigDirectory + "SideBarConfig.xml",
			                                FileErrorPolicy.ProvideAlternative);
		}
		
		protected virtual void OnTitleChanged(EventArgs e)
		{
			if (TitleChanged != null) {
				TitleChanged(this, e);
			}
		}
		protected virtual void OnIconChanged(EventArgs e)
		{
			if (IconChanged != null) {
				IconChanged(this, e);
			}
		}
		public event EventHandler TitleChanged;
		public event EventHandler IconChanged;
	}
}

