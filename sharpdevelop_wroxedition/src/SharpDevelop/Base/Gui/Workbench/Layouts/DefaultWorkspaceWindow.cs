// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class DefaultWorkspaceWindow : Form, IWorkbenchWindow
	{
		IViewContent content;
		
		EventHandler setTitleEvent = null;
		Crownwood.Magic.Controls.TabPage tabPage = null;
		
		public Crownwood.Magic.Controls.TabPage TabPage {
			get {
				return tabPage;
			}
			set {
				tabPage = value;
			}
		}
		
		public string Title {
			get {
				return Text;
			}
			set {
				Text = value;
				string fileName = content.ContentName;
				if (tabPage != null) {
					tabPage.Title = value;
				}
				if (fileName == null) {
					fileName = content.UntitledName;
				}
				if (fileName != null) {
					FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
					int index = fileUtilityService.GetImageIndexForFile(fileName);					
					this.Icon = System.Drawing.Icon.FromHandle(((Bitmap)fileUtilityService.ImageList.Images[index]).GetHicon());
				}
				OnTitleChanged(null);
			}
		}
		
		public IViewContent ViewContent {
			get {
				return content;
			}
			set {
				content = value;
			}
		}
		
		public void SelectWindow()
		{
			if (tabPage  != null) {
				tabPage.Select();
			}
			
			Activate();
			Select();
			content.Control.Focus();
			OnWindowSelected(EventArgs.Empty);
			
			foreach (IViewContent viewContent in WorkbenchSingleton.Workbench.ViewContentCollection) {
				if (viewContent != this.content) {
					viewContent.WorkbenchWindow.OnWindowDeselected(EventArgs.Empty);
				}
			}
		}
		
		public DefaultWorkspaceWindow(IViewContent content)
		{
			this.content = content;
			content.WorkbenchWindow = this;
			content.Control.Dock = DockStyle.Fill;
			Controls.Add(content.Control);
			
			setTitleEvent = new EventHandler(SetTitleEvent);
			content.ContentNameChanged += setTitleEvent;
			content.DirtyChanged    += setTitleEvent;
			SetTitleEvent(null, null);
		}
		
		string myUntitledTitle = null;
		
		public void SetTitleEvent(object sender, EventArgs e)
		{
			if (content == null) {
				return;
			}
			string newTitle = "";
			if (content.ContentName == null) {
				if (myUntitledTitle == null) {
					string baseName  = Path.GetFileNameWithoutExtension(content.UntitledName);
					int    number    = 1;
					bool   found     = true;
					while (found) {
						found = false;
						foreach (IViewContent windowContent in WorkbenchSingleton.Workbench.ViewContentCollection) {
							string title = windowContent.WorkbenchWindow.Title;
							if (title.EndsWith("*") || title.EndsWith("+")) {
								title = title.Substring(0, title.Length - 1);
							}
							if (title == baseName + number) {
								found = true;
								++number;
								break;
							}
						}
					}
					myUntitledTitle = baseName + number;
				}
				newTitle = myUntitledTitle;
			} else {
				newTitle = WindowState == FormWindowState.Minimized ? Path.GetFileName(content.ContentName) : content.ContentName;
			}
			
			if (content.IsDirty) {
				newTitle += "*";
			} else if (content.IsReadOnly) {
				newTitle += "+";
			}
			
			if (newTitle != Title) {
				Title = newTitle;
			}
		}
		
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			if (!forceClose && ViewContent != null && ViewContent.IsDirty) {
				ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
				DialogResult dr = MessageBox.Show(resourceService.GetString("MainWindow.SaveChangesMessage"),
				                                  resourceService.GetString("MainWindow.SaveChangesMessageHeader") + " " + Title + " ?",
				                                  MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				switch (dr) {
					case DialogResult.Yes:
						Activate();
						if (content.ContentName == null) {
							while (true) {
								new ICSharpCode.SharpDevelop.Commands.SaveFileAs().Run();
								if (ViewContent.IsDirty) {
									DialogResult result = MessageBox.Show("Do you really want to discard your changes ?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
									if (result == DialogResult.Yes) {
										break;
									}
								} else {
									break;
								}
							}
						} else {
							ViewContent.SaveFile();
						}
						break;
					case DialogResult.No:
						// set view content dirty = false, because I want to prevent double
						// checks, if Close() is called twice.
						ViewContent.IsDirty = false;
						break;
					case DialogResult.Cancel:
						e.Cancel = true;
						return;
				}
			}
		}
		
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			SetTitleEvent(null, null);
		}
		
		public void DetachContent()
		{
			content.ContentNameChanged -= setTitleEvent;
			content.DirtyChanged    -= setTitleEvent;
		}
		
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			OnWindowDeselected(e);
			OnCloseEvent(null);
		}
		
		bool forceClose = false;
		public void CloseWindow(bool force)
		{
			forceClose = force;
			Close();
		}
		
		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			OnWindowSelected(e);
		}
		
		protected override void OnDeactivate(EventArgs e)
		{
			base.OnActivated(e);
			OnWindowDeselected(e);
		}
		
		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			OnWindowSelected(e);
			
		}
		
//		protected override void OnLostFocus(EventArgs e)
//		{
//			base.OnLostFocus(e);
//			OnWindowDeselected(e);
//		}
		
		protected virtual void OnTitleChanged(EventArgs e)
		{
			if (TitleChanged != null) {
				TitleChanged(this, e);
			}
		}

		protected virtual void OnCloseEvent(EventArgs e)
		{
			if (CloseEvent != null) {
				CloseEvent(this, e);
			}
		}
		
		public virtual void OnWindowSelected(EventArgs e)
		{
			if (WindowSelected != null) {
				WindowSelected(this, e);
			}
		}
		
		public virtual void OnWindowDeselected(EventArgs e)
		{
			if (WindowDeselected != null) {
				WindowDeselected(this, e);
			}
		}
		
		public event EventHandler WindowSelected;
		public event EventHandler WindowDeselected;
		public event EventHandler TitleChanged;
		public event EventHandler CloseEvent;
	}
}
