// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Diagnostics;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.Core.Properties;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class OpenTaskView : ListView, IPadContent
	{
		Panel     myPanel  = new Panel();
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		
		public Control Control {
			get {
				return myPanel;
			}
		}
		
		public string Title {
			get {
				return resourceService.GetString("MainWindow.Windows.TaskList");
			}
		}
		
		public Bitmap Icon {
			get {
				return resourceService.GetBitmap("Icons.16x16.TaskListIcon");
			}
		}
		
		public void RedrawContent()
		{
			line.Text        = resourceService.GetString("CompilerResultView.LineText");
			description.Text = resourceService.GetString("CompilerResultView.DescriptionText");
			file.Text        = resourceService.GetString("CompilerResultView.FileText");
			path.Text        = "Path";
			OnTitleChanged(null);
			OnIconChanged(null);
		}
		
		ColumnHeader type        = new ColumnHeader();
		ColumnHeader line        = new ColumnHeader();
		ColumnHeader description = new ColumnHeader();
		ColumnHeader file        = new ColumnHeader();
		ColumnHeader path        = new ColumnHeader();
		
		public OpenTaskView()
		{
			type.Text = "!";
			
			RedrawContent();
			
			Columns.Add(type);
			Columns.Add(line);
			Columns.Add(description);
			Columns.Add(file);
			Columns.Add(path);
			
			FullRowSelect = true;
			AutoArrange = true;
			Alignment   = ListViewAlignment.Left;
			View = View.Details;
			Dock = DockStyle.Fill;
			GridLines  = true;
			Activation = ItemActivation.OneClick;
			OnResize(null);
			
			TaskService taskService        = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			
			taskService.TasksChanged  += new EventHandler(ShowResults);
			
			projectService.EndBuild   += new EventHandler(SelectTaskView);
			
			projectService.CombineOpened += new CombineEventHandler(OnCombineOpen);
			projectService.CombineClosed += new CombineEventHandler(OnCombineClosed);
			
			myPanel.Controls.Add(this);
			
			ImageList imglist = new ImageList();
			
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Error"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Warning"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Information"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Question"));
			this.SmallImageList = this.LargeImageList = imglist;
			
//			type.Width = 24;
//			line.Width = 50;
//			description.Width = 600;
//			file.Width = 150;
//			path.Width = 300;
		}
		
		protected override void Dispose(bool disposing)
		{
//			if (disposing) {
//				PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
//				propertyService.SetProperty("CompilerResultView.typeWidth", type.Width);
//				propertyService.SetProperty("CompilerResultView.lineWidth", line.Width);
//				propertyService.SetProperty("CompilerResultView.descriptionWidth", description.Width);
//				propertyService.SetProperty("CompilerResultView.fileWidth", file.Width);
//				propertyService.SetProperty("CompilerResultView.pathWidth", path.Width);
//			}
			base.Dispose(disposing);
		}
		void OnCombineOpen(object sender, CombineEventArgs e)
		{
			Items.Clear();
		}
		
		void OnCombineClosed(object sender, CombineEventArgs e)
		{
			Items.Clear();
		}
		
		void SelectTaskView(object sender, EventArgs e)
		{
			TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
			if (WorkbenchSingleton.Workbench.WorkbenchLayout.IsVisible(this) && taskService.Tasks.Count > 0) {
				Invoke(new EventHandler(SelectTaskView2));
			}
		}
		
		void SelectTaskView2(object sender, EventArgs e)
		{
			WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad(this);
		}
		
		protected override void OnItemActivate(EventArgs e)
		{
			base.OnItemActivate(e);
			
			if (FocusedItem != null) {
				Task task = (Task)FocusedItem.Tag;
				Debug.Assert(task != null);
				task.JumpToPosition();
			}
		}
		
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			type.Width = 24;
			line.Width = 50;
			int w = Width - type.Width - line.Width;
			file.Width = w * 15 / 100;
			path.Width = w * 15 / 100;
			description.Width = w - file.Width - path.Width - 5;
		}
		
		public CompilerResults CompilerResults = null;
		
		void ShowResults2(object sender, EventArgs e)
		{
			BeginUpdate();
			Items.Clear();
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
			foreach (Task task in taskService.Tasks) {
				int imageIndex = 0;
				switch (task.TaskType) {
					case TaskType.Warning:
						imageIndex = 1;
						break;
					case TaskType.Error:
						imageIndex = 0;
						break;
					case TaskType.Comment:
						imageIndex = 3;
						break;
					case TaskType.SearchResult:
						imageIndex = 2;
						break;
				}
				
				string tmpPath = task.FileName;
				if (task.Project != null) {
					tmpPath = fileUtilityService.AbsoluteToRelativePath(task.Project.BaseDirectory, task.FileName);
				} 
				
				string fileName = tmpPath;
				string path     = tmpPath;
				
				try {
					fileName = Path.GetFileName(tmpPath);
				} catch (Exception) {}
				
				try {
					path = Path.GetDirectoryName(tmpPath);
				} catch (Exception) {}
				
				ListViewItem item = new ListViewItem(new string[] {
					String.Empty,
					(task.Line + 1).ToString(),
					task.Description,
					fileName,
					path
				});
				item.ImageIndex = item.StateImageIndex = imageIndex;
				item.Tag = task;
				Items.Add(item);
			}
			EndUpdate();
		}
		
		public void ShowResults(object sender, EventArgs e)
		{
			Invoke(new EventHandler(ShowResults2));
			SelectTaskView(null, null);
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
