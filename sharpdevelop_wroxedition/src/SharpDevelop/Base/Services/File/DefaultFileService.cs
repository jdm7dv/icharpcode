// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Windows.Forms;

using ICSharpCode.Core.AddIns;

using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.SharpDevelop.Services
{
	public class DefaultFileService : AbstractService, IFileService
	{
		string currentFile;
		RecentOpen       recentOpen = null;
		FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		
		public RecentOpen RecentOpen {
			get {
				if (recentOpen == null) {
					PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
					recentOpen = (RecentOpen)propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.MainWindow.RecentOpen", new RecentOpen());
				}
				return recentOpen;
			}
		}
		
		public string CurrentFile {
			get {
				return currentFile;
			}
			set {
				currentFile = value;
			}
		}
		
		class LoadFileWrapper
		{
			IDisplayBinding binding;
			public LoadFileWrapper(IDisplayBinding binding)
			{
				this.binding = binding;
			}
			
			public void Invoke(string fileName)
			{
				WorkbenchSingleton.Workbench.ShowView(binding.CreateContentForFile(fileName));
			}
		}
		public void OpenFile(string fileName)
		{
			Debug.Assert(fileUtilityService.IsValidFileName(fileName));
				
			// test, if file fileName exists
			if (!fileName.StartsWith("http://")) {
				// test, if an untitled file should be opened
				if (!Path.IsPathRooted(fileName)) { 
					foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
						if (content.IsUntitled && content.UntitledName == fileName) {
							content.WorkbenchWindow.SelectWindow();
							return;
						}
					}
				} else if (!fileUtilityService.TestFileExists(fileName)) {
					return;
				}
			}
			
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				// WINDOWS DEPENDENCY : ToUpper()
				if (content.ContentName != null && 
				    content.ContentName.ToUpper() == fileName.ToUpper()) {
					content.WorkbenchWindow.SelectWindow();
					return;
				}
			}
			
			DisplayBindingService displayBindingService = (DisplayBindingService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(DisplayBindingService));
			
			IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			IDisplayBinding binding = displayBindingService.GetBindingPerFileName(fileName);
			
			if (binding != null) {
				if (fileUtilityService.ObservedLoad(new NamedFileOperationDelegate(new LoadFileWrapper(binding).Invoke),
				                                    fileName) == FileOperationResult.OK) {
					fileService.RecentOpen.AddLastFile(fileName);
				}
			} else {
				throw new ApplicationException("Can't open " + fileName + ", no display codon found.");
			}
		}
		
		public void NewFile(string defaultName, string language, string content)
		{
			DisplayBindingService displayBindingService = (DisplayBindingService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(DisplayBindingService));
			IDisplayBinding binding = displayBindingService.GetBindingPerLanguageName(language);
			
			if (binding != null) {
				IViewContent newContent = binding.CreateContentForLanguage(language, content);
				newContent.UntitledName = defaultName;
				newContent.IsDirty      = false;
				WorkbenchSingleton.Workbench.ShowView(newContent);
			} else {
				throw new ApplicationException("Can't create display binding for language " + language);				
			}
		}
		
		public IWorkbenchWindow GetOpenFile(string fileName)
		{
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				// WINDOWS DEPENDENCY : ToUpper()
				if (content.ContentName != null &&
				    content.ContentName.ToUpper() == fileName.ToUpper()) {
					return content.WorkbenchWindow;
				}
			}
			return null;
		}
		
		public void RemoveFileFromProject(string fileName)
		{
			if (Directory.Exists(fileName)) {
				OnFileRemovedFromProject(new FileEventArgs(fileName, true));
			} else {
				OnFileRemovedFromProject(new FileEventArgs(fileName, false));
			}
		}
		
		public void RemoveFile(string fileName)
		{
			if (Directory.Exists(fileName)) {
				try {
					Directory.Delete(fileName);
				} catch (Exception e) {
					MessageBox.Show("Can't remove directory " + fileName + " reason:\n" + e.ToString(), "Error");
					return;
				}
				OnFileRemoved(new FileEventArgs(fileName, true));
			} else {
				try {
					File.Delete(fileName);
				} catch (Exception e) {
					MessageBox.Show("Can't remove file " + fileName + " reason:\n" + e.ToString(), "Error");
					return;
				}
				OnFileRemoved(new FileEventArgs(fileName, false));
			}
		}
		
		public void RenameFile(string oldName, string newName)
		{
			if (oldName != newName) {
				if (Directory.Exists(oldName)) {
					try {
						Directory.Move(oldName, newName);
					} catch (Exception e) {
						MessageBox.Show("Can't rename directory " + oldName + " reason:\n" + e.ToString(), "Error");
						return;
					}
					OnFileRenamed(new FileEventArgs(oldName, newName, true));
				} else {
					try {
						File.Move(oldName, newName);
					} catch (Exception e) {
						MessageBox.Show("Can't rename file " + oldName + " reason:\n" + e.ToString(), "Error");
						return;
					}
					OnFileRenamed(new FileEventArgs(oldName, newName, false));
				}
			}
		}
		
		protected virtual void OnFileRemoved(FileEventArgs e)
		{
			if (FileRemoved != null) {
				FileRemoved(this, e);
			}
		}

		protected virtual void OnFileRenamed(FileEventArgs e)
		{
			if (FileRenamed != null) {
				FileRenamed(this, e);
			}
		}

		protected virtual void OnFileRemovedFromProject(FileEventArgs e)
		{
			if (FileRemovedFromProject != null) {
				FileRemovedFromProject(this, e);
			}
		}
		
		public event FileEventHandler FileRemovedFromProject;
		public event FileEventHandler FileRenamed;
		public event FileEventHandler FileRemoved;
	}
}
