// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
 
using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Windows.Forms;

using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using Crownwood.Magic.Common;
using Crownwood.Magic.Docking;
using Crownwood.Magic.Menus;

namespace ICSharpCode.SharpDevelop.Gui
{	
	/// <summary>
	/// This is the a Workspace with a single document interface.
	/// </summary>
	public class SdiWorkbenchLayout : IWorkbenchLayout
	{
		static string defaultconfigFile;
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		static string configFile = propertyService.ConfigDirectory + "MdiLayoutConfig.xml";
		Form wbForm;
		
		DockingManager dockManager;
		ICSharpCode.SharpDevelop.Gui.Components.OpenFileTab tabControl = new ICSharpCode.SharpDevelop.Gui.Components.OpenFileTab();
		
		public IWorkbenchWindow ActiveWorkbenchwindow {
			get {
				if (tabControl.SelectedTab == null)  {
					return null;
				}
				return (IWorkbenchWindow)tabControl.SelectedTab.Tag;
			}
		}
		
		static SdiWorkbenchLayout()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			string language = propertyService.GetProperty("CoreProperties.UILanguage").ToString();
			if (language.IndexOf('-') > 0) {
				language = language.Split(new char[] {'-'})[0];
			}
			
			defaultconfigFile = Application.StartupPath + 
								Path.DirectorySeparatorChar + ".." +
								Path.DirectorySeparatorChar + "data" +
								Path.DirectorySeparatorChar + "options" + 
								Path.DirectorySeparatorChar + "Layouts" + 
								Path.DirectorySeparatorChar + language + 
								Path.DirectorySeparatorChar + "LayoutConfig.xml";
			if (!File.Exists(defaultconfigFile)) {
				defaultconfigFile = Application.StartupPath + 
									Path.DirectorySeparatorChar + ".." +
									Path.DirectorySeparatorChar + "data" +
									Path.DirectorySeparatorChar + "options" + 
									Path.DirectorySeparatorChar + "Layouts" + 
									Path.DirectorySeparatorChar + "LayoutConfig.xml";
			}
		}
		
//		void LeftSelectionChanged(object sender, EventArgs e)
//		{
//			if (tabControlLeft.SelectedTab == null) {
//				return;
//			}
//			leftContent.Title = tabControlLeft.SelectedTab.Title;
//		}
//		
//		void BottomSelectionChanged(object sender, EventArgs e)
//		{
//			if (tabControlBottom.SelectedTab == null) {
//				return;
//			}
//			bottomContent.Title = tabControlBottom.SelectedTab.Title;
//		}
		
		public void Attach(IWorkbench workbench)
		{
			wbForm = (Form)workbench;
			wbForm.Controls.Clear();
			
			tabControl.Dock = DockStyle.Fill;
			tabControl.ShrinkPagesToFit = true;
			tabControl.Appearance = Crownwood.Magic.Controls.TabControl.VisualAppearance.MultiDocument;
			wbForm.Controls.Add(tabControl);
			
			dockManager = new DockingManager(wbForm, VisualStyle.IDE);
			
			Control firstControl = null;
			IStatusBarService statusBarService = (IStatusBarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
			wbForm.Controls.Add(statusBarService.Control);
			
			foreach (ToolBar toolBar in ((DefaultWorkbench)workbench).ToolBars) {
				if (firstControl == null) {
					firstControl = firstControl;
				}
				wbForm.Controls.Add(toolBar);
			}
						
			((DefaultWorkbench)workbench).TopMenu.Dock = DockStyle.Top;
			wbForm.Controls.Add(((DefaultWorkbench)workbench).TopMenu);
			
			wbForm.Menu = null;
			dockManager.InnerControl = tabControl;
			dockManager.OuterControl = statusBarService.Control;
			
			foreach (IViewContent content in workbench.ViewContentCollection) {
				ShowView(content);
			}
			
			foreach (IPadContent content in workbench.PadContentCollection) {
				ShowPad(content);
			}
			tabControl.SelectionChanged += new EventHandler(ActiveMdiChanged);
			
			try {
				if (File.Exists(configFile)) {
					dockManager.LoadConfigFromFile(configFile);
				} else if (File.Exists(defaultconfigFile)) {
					dockManager.LoadConfigFromFile(defaultconfigFile);
				}
			} catch (Exception) {
				Console.WriteLine("can't load docking configuration, version clash ?");
			}
			RedrawAllComponents();
		}
		
		public void Detach()
		{
			if (dockManager != null) {
				dockManager.SaveConfigToFile(configFile);
			}
			
			foreach (Crownwood.Magic.Controls.TabPage page in tabControl.TabPages) {
				SdiWorkspaceWindow f = (SdiWorkspaceWindow)page.Tag;
				f.DetachContent();
				f.ViewContent = null;
			}
			
			tabControl.TabPages.Clear();
			tabControl.Controls.Clear();
			
			if (dockManager != null) {
				dockManager.Contents.Clear();
			}
			
			wbForm.Controls.Clear();
		}
		
		WindowContent leftContent = null;
		WindowContent bottomContent = null;
		Hashtable contentHash = new Hashtable();
		
		public void ShowPad(IPadContent content)
		{
			if (contentHash[content] == null) {
				IProperties properties = (IProperties)propertyService.GetProperty("Workspace.ViewMementos", new DefaultProperties());
				string type = content.GetType().ToString();
				content.Control.Dock = DockStyle.None;
				Content c1;
				if (content.Icon != null) {
					ImageList imgList = new ImageList();
					imgList.Images.Add(content.Icon);
					c1 = dockManager.Contents.Add(content.Control, content.Title, imgList, 0);
				} else {
					c1 = dockManager.Contents.Add(content.Control, content.Title);
				}
				
				c1.DisplaySize = new Size(270,200);
				contentHash[content] = c1;
				
				if (properties.GetProperty(type, "Left") == "Left") {
					if (leftContent == null) {
						leftContent = dockManager.AddContentWithState(c1, State.DockLeft);
					} else {
						dockManager.AddContentToWindowContent(c1, leftContent);
					}
				} else if (properties.GetProperty(type, "Left") == "Bottom") {
					if (bottomContent == null) {
						bottomContent = dockManager.AddContentWithState(c1, State.DockBottom);
					} else {
						dockManager.AddContentToWindowContent(c1, bottomContent);
					}
				}
			} else {
				Content c = (Content)contentHash[content];
				if (c != null) {
					dockManager.ShowContent(c);
				}
			}
		}
				
		public bool IsVisible(IPadContent padContent)
		{
			Content content = (Content)contentHash[padContent];
			if (content != null) {
				return content.Visible;
			}
			return false;
		}
		
		public void HidePad(IPadContent padContent)
		{
			Content content = (Content)contentHash[padContent];
			if (content != null) {
				dockManager.HideContent(content);
			}
		}
		
		public void ActivatePad(IPadContent padContent)
		{
			Content content = (Content)contentHash[padContent];
			if (content != null) {
				content.BringToFront();
			}
		}
		
		public void RedrawAllComponents()
		{
			tabControl.Style = (Crownwood.Magic.Common.VisualStyle)propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.VisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);
			
			// redraw correct pad content names (language changed).
			foreach (IPadContent content in ((IWorkbench)wbForm).PadContentCollection) {
				Content c = (Content)contentHash[content];
				if (c != null) {
					c.Title = c.FullTitle = content.Title;
				}
			}
		}
		
		public void CloseWindowEvent(object sender, EventArgs e)
		{
			SdiWorkspaceWindow f = (SdiWorkspaceWindow)sender;
			if (f.ViewContent != null) {
				((IWorkbench)wbForm).CloseContent(f.ViewContent);
				ActiveMdiChanged(this, null);
			}
		}
		
		public IWorkbenchWindow ShowView(IViewContent content)
		{
			content.Control.Dock = DockStyle.None;
			content.Control.Visible = true;
			SdiWorkspaceWindow sdiWorkspace = new SdiWorkspaceWindow(content, tabControl);
			sdiWorkspace.TabPage = tabControl.AddWindow(sdiWorkspace);
			sdiWorkspace.CloseEvent += new EventHandler(CloseWindowEvent);
			return sdiWorkspace;
		}
		
		void ActiveMdiChanged(object sender, EventArgs e)
		{
			if (ActiveWorkbenchWindowChanged != null) {
				ActiveWorkbenchWindowChanged(this, e);
			}
		}
		
		public event EventHandler ActiveWorkbenchWindowChanged;
	}
}
