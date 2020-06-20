// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Resources;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	public class GotoLineNumberDialog : Form
	{
		Container  components;
		Button     cancelButton;
		Button     okButton;
		TextBox    lineNumberTextBox;
		Label      lineNumberLabel;
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		
		void cancelEvent(object sender, EventArgs e)
		{
			Close();
		}
		
		void closeEvent(object sender, EventArgs e)
		{
			try {
				IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
				
				if (window != null && window.ViewContent.Control is TextAreaControl) {
					TextAreaControl textarea = (TextAreaControl)window.ViewContent.Control;
					
					int i = Math.Min(textarea.Document.TotalNumberOfLines, Math.Max(1, Int32.Parse(lineNumberTextBox.Text)));
					
					LineSegment line = textarea.Document.GetLineSegment(i - 1);
					textarea.Document.Caret.Offset = line.Offset;
					textarea.ScrollToCaret();
				}
			} catch (Exception) {
				
			} finally {
				Close();
			}
		}
		
		public GotoLineNumberDialog()
		{
			InitializeComponent();
			
			AcceptButton = okButton;
			CancelButton = cancelButton;
			
			okButton.Click     += new EventHandler(closeEvent);
			cancelButton.Click += new EventHandler(cancelEvent);
			
			MaximizeBox  = MinimizeBox = false;
			
			ShowInTaskbar = false;
			StartPosition = FormStartPosition.CenterParent;
			Icon = null;
		}
		
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null){
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		private void InitializeComponent()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			bool flat = Crownwood.Magic.Common.VisualStyle.IDE == (Crownwood.Magic.Common.VisualStyle)propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.VisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);
			
			this.components = new System.ComponentModel.Container();
			this.cancelButton = new System.Windows.Forms.Button();
			this.lineNumberLabel = new System.Windows.Forms.Label();
			this.okButton = new System.Windows.Forms.Button();
			this.lineNumberTextBox = new System.Windows.Forms.TextBox();
			
			cancelButton.Location = new System.Drawing.Point(90, 40);
			cancelButton.Size = new System.Drawing.Size(74, 24);
			cancelButton.TabIndex = 3;
			cancelButton.Text = resourceService.GetString("Global.CancelButtonText");
			cancelButton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			lineNumberLabel.Location = new System.Drawing.Point(10, 10);
			lineNumberLabel.Text = resourceService.GetString("Dialog.GotoLineNumber.label1Text");
			lineNumberLabel.Size = new System.Drawing.Size(72, 16);
			lineNumberLabel.TabIndex =0;
			
			okButton.Location = new System.Drawing.Point(8, 40);
			okButton.Size = new System.Drawing.Size(74, 24);
			okButton.TabIndex = 2;
			okButton.Text = resourceService.GetString("Global.OKButtonText");
			okButton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.Text = resourceService.GetString("Dialog.GotoLineNumber.DialogName");
			this.ClientSize = new System.Drawing.Size(200, 69);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			
			lineNumberTextBox.Location = new System.Drawing.Point(80, 8);
			lineNumberTextBox.Text = "";
			lineNumberTextBox.TabIndex = 1;
			lineNumberTextBox.Size = new System.Drawing.Size(112, 20);
			
			this.Controls.Add(cancelButton);
			this.Controls.Add(okButton);
			this.Controls.Add(lineNumberTextBox);
			this.Controls.Add(lineNumberLabel);
		}
	}
}
