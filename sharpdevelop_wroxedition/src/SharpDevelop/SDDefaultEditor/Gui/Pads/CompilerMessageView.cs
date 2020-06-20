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
using System.IO;
using System.Diagnostics;
using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	// Note: I moved the pads to this assembly, because I want no cyclic dll dependency
	// on the ICSharpCode.TextEditor assembly.
	
	/// <summary>
	/// This class displays the errors and warnings which the compiler outputs and
	/// allows the user to jump to the source of the warnig / error
	/// </summary>
	public class CompilerMessageView : IPadContent
	{
		TextAreaControl textAreaControl = new TextAreaControl();
		Panel       myPanel = new Panel();
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		
		public Control Control {
			get {
				return myPanel;
			}
		}
		
		public string Title {
			get {
				return resourceService.GetString("MainWindow.Windows.OutputWindow");
			}
		}
		
		public Bitmap Icon {
			get {
				return resourceService.GetBitmap("Icons.16x16.OutputIcon");
			}
		}
		
		public void Dispose()
		{
		}
		
		public void RedrawContent()
		{
			OnTitleChanged(null);
			OnIconChanged(null);
		}
		
		public CompilerMessageView()
		{
			textAreaControl.Dock     = DockStyle.Fill;
			textAreaControl.Document.Properties = new DefaultProperties();
			textAreaControl.Document.ReadOnly = true;
			textAreaControl.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy();
			textAreaControl.ShowHRuler       = false;
			textAreaControl.ShowVRuler       = false;
			textAreaControl.ShowLineNumbers  = false;
			textAreaControl.ShowInvalidLines = false;			
			textAreaControl.EnableFolding = false;
			textAreaControl.ScrollLineHeight = 0;
			
			textAreaControl.VisibleChanged += new EventHandler(ActivateTextBox);
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			textAreaControl.Font = resourceService.LoadFont("Courier New", 10);
			myPanel.Controls.Add(textAreaControl);
			
			TaskService     taskService    = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			
			taskService.CompilerOutputChanged += new EventHandler(SetOutput);
			
			projectService.StartBuild    += new EventHandler(SelectMessageView);
			projectService.CombineOpened += new CombineEventHandler(OnCombineOpen);
			projectService.CombineClosed += new CombineEventHandler(OnCombineClosed);
		}
		
		void OnCombineOpen(object sender, CombineEventArgs e)
		{
			textAreaControl.Document.TextContent = String.Empty;
			textAreaControl.Refresh();
		}
		
		void OnCombineClosed(object sender, CombineEventArgs e)
		{
			textAreaControl.Document.TextContent = String.Empty;
			textAreaControl.Refresh();
		}
		
		void SelectMessageView(object sender, EventArgs e)
		{
			if (WorkbenchSingleton.Workbench.WorkbenchLayout.IsVisible(this)) {
				WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad(this);
			}
		}
		
		void SetOutput2(object sender, EventArgs e)
		{
			TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
			try {
				textAreaControl.Document.TextContent = taskService.CompilerOutput;
				UpdateTextArea();
			} catch (Exception) {}
			
			System.Threading.Thread.Sleep(100);
		}
		
		void UpdateTextArea()
		{
			textAreaControl.Document.Caret.Offset = textAreaControl.Document.TextLength;
			textAreaControl.ScrollToCaret();
			textAreaControl.Refresh();
		}
		string outputText = null;
		void SetOutput(object sender, EventArgs e)
		{
			if (WorkbenchSingleton.Workbench.WorkbenchLayout.IsVisible(this)) {
				textAreaControl.Invoke(new EventHandler(SetOutput2));
				outputText = null;
			} else {
				TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
				outputText = taskService.CompilerOutput;
				UpdateTextArea();
			}
		}
		
		void ActivateTextBox(object sender, EventArgs e)
		{
			if (outputText != null && textAreaControl.Visible) {
				textAreaControl.Document.TextContent = outputText;
				UpdateTextArea();
				outputText = null;
			}
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
