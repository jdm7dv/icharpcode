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

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	public class EditTemplateDialog : Form 
	{
		
		private System.ComponentModel.Container components;
		private Button cancelButton;
		private Button button1;
		private TextBox descriptionTextBox;
		private Label label2;
		private TextBox ShortCutTextBox;
		private Label label1;
		CodeTemplate t;
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		
		void CloseEvent(object sender, EventArgs e)
		{
			t.Shortcut    = ShortCutTextBox.Text;
			t.Description = descriptionTextBox.Text;
		}
		
		public EditTemplateDialog(CodeTemplate t) {
			
			this.t = t;
			// Required for Win Form Designer support
			InitializeComponent();
			
			ShortCutTextBox.Text = t.Shortcut;
			descriptionTextBox.Text = t.Description;
			AcceptButton = button1;
			CancelButton = cancelButton;
			button1.Click += new EventHandler(CloseEvent);
			TopMost = true;
			ShortCutTextBox.Select();
			StartPosition = FormStartPosition.CenterParent;
			MaximizeBox  = MinimizeBox = false;
			ShowInTaskbar = false;
			Icon = null;
			StartPosition = FormStartPosition.CenterParent;
			
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
		
		
		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with an editor
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.button1 = new Button();
			this.label1 = new Label();
			this.label2 = new Label();
			this.ShortCutTextBox = new TextBox();
			this.descriptionTextBox = new TextBox();
			this.cancelButton = new Button();
			
			button1.Location = new System.Drawing.Point(124, 72);
			button1.Size = new System.Drawing.Size(74, 24);
			button1.TabIndex = 4;
			button1.Text = resourceService.GetString("Global.OKButtonText");
			
			label1.Location = new System.Drawing.Point(8, 11);
			label1.Text = resourceService.GetString("Dialog.Options.CodeTemplate.Template");
			label1.Size = new System.Drawing.Size(74, 16);
			label1.TabIndex = 0;
			
			label2.Location = new System.Drawing.Point(8, 43);
			label2.Text = resourceService.GetString("Dialog.Options.CodeTemplate.Description");
			label2.Size = new System.Drawing.Size(74, 16);
			label2.TabIndex = 2;
			
			ShortCutTextBox.Location = new System.Drawing.Point(80, 8);
			ShortCutTextBox.Text = "";
			ShortCutTextBox.TabIndex = 1;
			ShortCutTextBox.Size = new System.Drawing.Size(200, 20);
			
			descriptionTextBox.Location = new System.Drawing.Point(80, 40);
			descriptionTextBox.Text = "";
			descriptionTextBox.TabIndex = 3;
			descriptionTextBox.Size = new System.Drawing.Size(200, 20);
			
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.Text = "Edit Template";
			//@design this.TrayLargeIcon = true;
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			//@design this.TrayHeight = 0;
			this.ClientSize = new System.Drawing.Size(290, 103);
			
			cancelButton.Location = new System.Drawing.Point(206, 72);
			cancelButton.Size = new System.Drawing.Size(74, 24);
			cancelButton.TabIndex = 5;
			cancelButton.Text = resourceService.GetString("Global.CancelButtonText");
			button1.DialogResult = DialogResult.OK;
			cancelButton.DialogResult = DialogResult.Cancel;
			
			this.Controls.Add(cancelButton);
			this.Controls.Add(button1);
			this.Controls.Add(descriptionTextBox);
			this.Controls.Add(label2);
			this.Controls.Add(ShortCutTextBox);
			this.Controls.Add(label1);
		}
	}
}
