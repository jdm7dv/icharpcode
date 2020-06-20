// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Windows.Forms;

using Crownwood.Magic.Menus;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.SharpDevelop.Internal.Project;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class RecentFilesMenuBuilder : ISubmenuBuilder
	{
		public MenuCommand[] BuildSubmenu(object owner)
		{
			IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			RecentOpen recentOpen = fileService.RecentOpen;
			
			if (recentOpen.RecentFile.Count > 0) {
				SdMenuCommand[] items = new SdMenuCommand[recentOpen.RecentFile.Count];
				
				for (int i = 0; i < recentOpen.RecentFile.Count; ++i) {
					items[i] = new SdMenuCommand(recentOpen.RecentFile[i].ToString(), new EventHandler(LoadRecentFile));
					items[i].Description = stringParserService.Parse(resourceService.GetString("Dialog.Componnents.RichMenuItem.LoadFileDescription"),
					                                          new string[,] { {"FILE", recentOpen.RecentFile[i].ToString()} });
				}
				return items;
			}
			
			SdMenuCommand defaultMenu = new SdMenuCommand(resourceService.GetString("Dialog.Componnents.RichMenuItem.NoRecentFilesString"));
			defaultMenu.Enabled = false;
			
			return new MenuCommand[] { defaultMenu };
		}
		
		void LoadRecentFile(object sender, EventArgs e)
		{
			SdMenuCommand item = (SdMenuCommand)sender;
			IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			fileService.OpenFile(item.Text);
		}
	}
	
	public class RecentProjectsMenuBuilder : ISubmenuBuilder
	{
		public MenuCommand[] BuildSubmenu(object owner)
		{
			IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			RecentOpen recentOpen = fileService.RecentOpen;
			
			if (recentOpen.RecentProject.Count > 0) {
				SdMenuCommand[] items = new SdMenuCommand[recentOpen.RecentProject.Count];
				
				for (int i = 0; i < recentOpen.RecentProject.Count; ++i) {
					items[i] = new SdMenuCommand(recentOpen.RecentProject[i].ToString(), new EventHandler(LoadRecentProject));
					items[i].Description = stringParserService.Parse(resourceService.GetString("Dialog.Componnents.RichMenuItem.LoadProjectDescription"),
					                                         new string[,] { {"PROJECT", recentOpen.RecentProject[i].ToString()} });
				}
				return items;
			}
			
			SdMenuCommand defaultMenu = new SdMenuCommand(resourceService.GetString("Dialog.Componnents.RichMenuItem.NoRecentProjectsString"));
			defaultMenu.Enabled = false;
			
			return new MenuCommand[] { defaultMenu };
		}
		void LoadRecentProject(object sender, EventArgs e)
		{
			SdMenuCommand item = (SdMenuCommand)sender;
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			projectService.OpenCombine(item.Text);
		}		
	}
	
	public class ToolMenuBuilder : ISubmenuBuilder
	{
		public MenuCommand[] BuildSubmenu(object owner)
		{
			//			IconMenuStyle iconMenuStyle = (IconMenuStyle)propertyService.GetProperty("IconMenuItem.IconMenuStyle", IconMenuStyle.VSNet);
			SdMenuCommand[] items = new SdMenuCommand[ToolLoader.Tool.Count];
			for (int i = 0; i < ToolLoader.Tool.Count; ++i) {
				SdMenuCommand item = new SdMenuCommand(ToolLoader.Tool[i].ToString(), new EventHandler(ToolEvt));
				item.Description = "Start tool " + String.Join(String.Empty, ToolLoader.Tool[i].ToString().Split('&'));
				items[i] = item;
			}
			return items;
		}
		
		void ToolEvt(object sender, EventArgs e)
		{
			SdMenuCommand item = (SdMenuCommand)sender;
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			for (int i = 0; i < ToolLoader.Tool.Count; ++i) {
				if (item.Text == ToolLoader.Tool[i].ToString()) {
					ExternalTool tool = (ExternalTool)ToolLoader.Tool[i];
					stringParserService.Properties["StartupPath"] = Application.StartupPath;
					string command = stringParserService.Parse(tool.Command);
					
					if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
						stringParserService.Properties["File"] = '"' + WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName + '"';
					} else {
						stringParserService.Properties["File"] = "";
					}
					
					stringParserService.Properties["Assembly"] = "";
					LanguageBindingService languageBindingService = (LanguageBindingService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(LanguageBindingService));
					if (projectService.CurrentSelectedProject != null) {
						ILanguageBinding binding = languageBindingService.GetBindingPerLanguageName(projectService.CurrentSelectedProject.ProjectType);
						if (binding != null) {
							stringParserService.Properties["Assembly"] = '"' + binding.GetCompiledOutputName(projectService.CurrentSelectedProject)  + '"';
						}
					} else if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
						
						string fileName = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName;
						ILanguageBinding binding = languageBindingService.GetBindingPerFileName(fileName);
						if (binding != null) {
							stringParserService.Properties["Assembly"] = '"' + binding.GetCompiledOutputName(fileName) + '"';
						}
					}
					
					string args = stringParserService.Parse(tool.Arguments);
					try {
						ProcessStartInfo startinfo = new ProcessStartInfo(command, args);
						startinfo.WorkingDirectory = tool.InitialDirectory;
						Process.Start(startinfo);
					} catch (Exception ex) {
						MessageBox.Show("Error while starting:\n '" + command + " " + args + "'" + "\n" + ex.ToString(), "External program execution failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
						break;
					}
				}
			}
		}
				
	public class OpenContentsMenuBuilder : ISubmenuBuilder
	{
		public MenuCommand[] BuildSubmenu(object owner)
		{
			//			IconMenuStyle iconMenuStyle = (IconMenuStyle)propertyService.GetProperty("IconMenuItem.IconMenuStyle", IconMenuStyle.VSNet);
			int contentCount = WorkbenchSingleton.Workbench.ViewContentCollection.Count;
			if (contentCount == 0) {
				return null;
			}
			MenuCommand[] items = new MenuCommand[contentCount + 1];
			items[0] = new MenuCommand("-");
			for (int i = 0; i < contentCount; ++i) {
				IViewContent content = (IViewContent)WorkbenchSingleton.Workbench.ViewContentCollection[i];
				
				SdMenuCommand item = new SdMenuCommand(content.WorkbenchWindow.Title, new EventHandler(ClickEvent));
				item.Tag = content.WorkbenchWindow;
				item.Checked = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == content.WorkbenchWindow;
				item.Description = "Activate this window ";
				items[i + 1] = item;
			}
			return items;
		}
		
		void ClickEvent(object sender, EventArgs e)
		{
			MenuCommand item = ((MenuCommand)sender);
			if (item != null && item.Tag != null) {
				((IWorkbenchWindow)item.Tag).SelectWindow();
			}
		}
	}
	
	public class IncludeFilesBuilder : ISubmenuBuilder
	{
		public ProjectBrowserView browser;
		
		public MenuCommand includeInCompileItem;
		public MenuCommand includeInDeployItem;
		
		class MyMenuItem : MenuCommand
		{
			IncludeFilesBuilder builder;
			public MyMenuItem(IncludeFilesBuilder builder, string name, EventHandler handler) : base(name, handler)
			{
				this.builder = builder;
				Update += new EventHandler(UpdateThisItem);
			}
			
			public void UpdateThisItem(object sender, EventArgs e)
			{
				AbstractBrowserNode node = builder.browser.SelectedNode as AbstractBrowserNode;
				
				if (node == null) {
					return;
				}
				
				ProjectFile finfo = node.UserData as ProjectFile;
				if (finfo == null) {
					builder.includeInCompileItem.Enabled = builder.includeInCompileItem.Enabled = false;
				} else {
					if (!builder.includeInCompileItem.Enabled) {
						builder.includeInCompileItem.Enabled = builder.includeInCompileItem.Enabled = true;
					}
					builder.includeInCompileItem.Checked = finfo.BuildAction == BuildAction.Compile;
					builder.includeInDeployItem.Checked = !node.Project.DeployInformation.IsFileExcluded(finfo.Name);
				}
			}
		}
		
		public MenuCommand[] BuildSubmenu(object owner)
		{
			//			IconMenuStyle iconMenuStyle = (IconMenuStyle)propertyService.GetProperty("IconMenuItem.IconMenuStyle", IconMenuStyle.VSNet);
			browser = (ProjectBrowserView)owner;
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			includeInCompileItem = new MyMenuItem(this, resourceService.GetString("ProjectComponent.ContextMenu.IncludeMenu.InCompile"), new EventHandler(ChangeCompileInclude));
			includeInDeployItem  = new MyMenuItem(this, resourceService.GetString("ProjectComponent.ContextMenu.IncludeMenu.InDeploy"),  new EventHandler(ChangeDeployInclude));
			
			return new MenuCommand[] {
				includeInCompileItem,
				includeInDeployItem
			};
			
		}
		void ChangeCompileInclude(object sender, EventArgs e)
		{
			AbstractBrowserNode node = browser.SelectedNode as AbstractBrowserNode;
			
			if (node == null) {
				return;
			}
			
			ProjectFile finfo = node.UserData as ProjectFile;
			if (finfo != null) {
				if (finfo.BuildAction == BuildAction.Compile) {
					finfo.BuildAction = BuildAction.Nothing;
				} else {
					finfo.BuildAction = BuildAction.Compile;
				}
			}
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			projectService.SaveCombine();
		}
		
		void ChangeDeployInclude(object sender, EventArgs e)
		{
			AbstractBrowserNode node = browser.SelectedNode as AbstractBrowserNode;
			
			if (node == null) {
				return;
			}
			
			ProjectFile finfo = node.UserData as ProjectFile;
			if (finfo != null) {
				if (node.Project.DeployInformation.IsFileExcluded(finfo.Name)) {
					node.Project.DeployInformation.RemoveExcludedFile(finfo.Name);
				} else {
					node.Project.DeployInformation.AddExcludedFile(finfo.Name);
				}
			}
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			projectService.SaveCombine();
		}
	}
	
	
	public class ViewMenuBuilder : ISubmenuBuilder
	{
		class MyMenuItem : SdMenuCommand
		{
			IPadContent padContent;
			
			bool IsPadVisible {
				get {
					return WorkbenchSingleton.Workbench.WorkbenchLayout.IsVisible(padContent); 
				}
			}
			
			public MyMenuItem(IPadContent padContent) : base(padContent.Title)
			{
				this.padContent = padContent;
				this.Click += new EventHandler(ClickEvent);
				Update += new EventHandler(UpdateThisItem);
			}
			
			public void UpdateThisItem(object sender, EventArgs e)
			{
				Checked = IsPadVisible;
			}
			
			void ClickEvent(object sender, EventArgs e)
			{
				if (IsPadVisible) {
					WorkbenchSingleton.Workbench.WorkbenchLayout.HidePad(padContent);
				} else {
					WorkbenchSingleton.Workbench.WorkbenchLayout.ShowPad(padContent);
				}
			}
		}
		
		public MenuCommand[] BuildSubmenu(object owner)
		{
			ArrayList items = new ArrayList();
			foreach (IPadContent padContent in WorkbenchSingleton.Workbench.PadContentCollection) {
				items.Add(new MyMenuItem(padContent));
			}
			return (MenuCommand[])items.ToArray(typeof(MenuCommand));
		}
	}
}
