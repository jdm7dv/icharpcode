// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Gui.Components;

using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels
{
	public enum LineTerminatorStyle {
		Windows,
		Macintosh,
		Unix
	}
	
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class LoadSavePanel : AbstractOptionPanel
	{
		private System.Windows.Forms.Label loadLabel;
		private System.Windows.Forms.Label loadSeparatorLabel;
		private System.Windows.Forms.Label saveLabel;
		private System.Windows.Forms.Label saveSeparatorLabel;
		private System.Windows.Forms.CheckBox loadUserDataCheckBox;
		private System.Windows.Forms.CheckBox createBackupCopyCheckBox;
		private System.Windows.Forms.GroupBox lineTerminatorStyleGroupBox;
		private System.Windows.Forms.RadioButton windowsRadioButton;
		private System.Windows.Forms.RadioButton macintoshRadioButton;
		private System.Windows.Forms.RadioButton unixRadioButton;
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.OK) {
				if (windowsRadioButton.Checked) {
					propertyService.SetProperty("SharpDevelop.LineTerminatorStyle", LineTerminatorStyle.Windows);
				} else if (macintoshRadioButton.Checked) {
					propertyService.SetProperty("SharpDevelop.LineTerminatorStyle", LineTerminatorStyle.Macintosh);
				} else {
					propertyService.SetProperty("SharpDevelop.LineTerminatorStyle", LineTerminatorStyle.Unix);
				}
				propertyService.SetProperty("SharpDevelop.LoadDocumentProperties", loadUserDataCheckBox.Checked);
				propertyService.SetProperty("SharpDevelop.CreateBackupCopy", createBackupCopyCheckBox.Checked);
			}
	    	return true;
		}
		
		public LoadSavePanel()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			
			LineTerminatorStyle lineTerminatorStyle = (LineTerminatorStyle)propertyService.GetProperty("SharpDevelop.LineTerminatorStyle", LineTerminatorStyle.Windows);
			
			windowsRadioButton.Checked   = (lineTerminatorStyle == LineTerminatorStyle.Windows);
			macintoshRadioButton.Checked = (lineTerminatorStyle == LineTerminatorStyle.Macintosh);
			unixRadioButton.Checked      = (lineTerminatorStyle == LineTerminatorStyle.Unix);
			
			loadUserDataCheckBox.Checked     = propertyService.GetProperty("SharpDevelop.LoadDocumentProperties", true);
			createBackupCopyCheckBox.Checked = propertyService.GetProperty("SharpDevelop.CreateBackupCopy", false);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			bool flat = Crownwood.Magic.Common.VisualStyle.IDE == (Crownwood.Magic.Common.VisualStyle)propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.VisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);
			
			this.loadLabel = new System.Windows.Forms.Label();
			this.loadSeparatorLabel = new System.Windows.Forms.Label();
			this.saveSeparatorLabel = new System.Windows.Forms.Label();
			this.saveLabel = new System.Windows.Forms.Label();
			this.loadUserDataCheckBox = new System.Windows.Forms.CheckBox();
			this.createBackupCopyCheckBox = new System.Windows.Forms.CheckBox();
			this.lineTerminatorStyleGroupBox = new System.Windows.Forms.GroupBox();
			this.windowsRadioButton = new System.Windows.Forms.RadioButton();
			this.macintoshRadioButton = new System.Windows.Forms.RadioButton();
			this.unixRadioButton = new System.Windows.Forms.RadioButton();
			this.lineTerminatorStyleGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// loadLabel
			// 
			this.loadLabel.Location = new System.Drawing.Point(8, 8);
			this.loadLabel.Size = new System.Drawing.Size(60, 16);
			this.loadLabel.TabIndex = 0;
			this.loadLabel.Text = resourceService.GetString("Dialog.Options.IDEOptions.LoadSaveOptions.LoadLabel");
			
			//
			// loadSeparatorLabel
			// 
			this.loadSeparatorLabel.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.loadSeparatorLabel.BorderStyle  = flat ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
			this.loadSeparatorLabel.Location = new System.Drawing.Point(48 + 20, 16);
			this.loadSeparatorLabel.Size = new System.Drawing.Size(276, flat ? 1 : 2);
			this.loadSeparatorLabel.TabIndex = 1;
			// 
			// saveSeparatorLabel
			// 
			this.saveSeparatorLabel.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.saveSeparatorLabel.BorderStyle  = flat ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
			this.saveSeparatorLabel.Location = new System.Drawing.Point(48 + 20, 64);
			this.saveSeparatorLabel.Size = new System.Drawing.Size(276, flat ? 1 : 2);
			this.saveSeparatorLabel.TabIndex = 3;
			// 
			// saveLabel
			// 
			this.saveLabel.Location = new System.Drawing.Point(8, 56);
			this.saveLabel.Size = new System.Drawing.Size(60, 16);
			this.saveLabel.TabIndex = 2;
			this.saveLabel.Text = resourceService.GetString("Dialog.Options.IDEOptions.LoadSaveOptions.SaveLabel");
			// 
			// loadUserDataCheckBox
			// 
			this.loadUserDataCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.loadUserDataCheckBox.Location = new System.Drawing.Point(24, 32);
			this.loadUserDataCheckBox.Size = new System.Drawing.Size(320, 16);
			this.loadUserDataCheckBox.TabIndex = 4;
			this.loadUserDataCheckBox.Text = resourceService.GetString("Dialog.Options.IDEOptions.LoadSaveOptions.LoadUserDataCheckBox");
			loadUserDataCheckBox.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			// 
			// createBackupCopyCheckBox
			// 
			this.createBackupCopyCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.createBackupCopyCheckBox.Location = new System.Drawing.Point(24, 80);
			this.createBackupCopyCheckBox.Size = new System.Drawing.Size(320, 24);
			this.createBackupCopyCheckBox.TabIndex = 5;
			this.createBackupCopyCheckBox.Text = resourceService.GetString("Dialog.Options.IDEOptions.LoadSaveOptions.CreateBackupCopyCheckBox");
			createBackupCopyCheckBox.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			// 
			// lineTerminatorStyleGroupBox
			// 
			this.lineTerminatorStyleGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.lineTerminatorStyleGroupBox.Controls.AddRange(new System.Windows.Forms.Control[] {
																									  this.unixRadioButton,
																									  this.macintoshRadioButton,
																									  this.windowsRadioButton});
			this.lineTerminatorStyleGroupBox.Location = new System.Drawing.Point(24, 112);
			this.lineTerminatorStyleGroupBox.Size = new System.Drawing.Size(320, 96);
			this.lineTerminatorStyleGroupBox.TabIndex = 6;
			this.lineTerminatorStyleGroupBox.TabStop = false;
			this.lineTerminatorStyleGroupBox.Text = resourceService.GetString("Dialog.Options.IDEOptions.LoadSaveOptions.LineTerminatorStyleGroupBox");
			// 
			// windowsRadioButton
			// 
			this.windowsRadioButton.Location = new System.Drawing.Point(8, 64);
			this.windowsRadioButton.Size = new System.Drawing.Size(152, 24);
			this.windowsRadioButton.TabIndex = 0;
			this.windowsRadioButton.Text = resourceService.GetString("Dialog.Options.IDEOptions.LoadSaveOptions.WindowsRadioButton");
			this.windowsRadioButton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			// 
			// macintoshRadioButton
			// 
			this.macintoshRadioButton.Location = new System.Drawing.Point(8, 16);
			this.macintoshRadioButton.Name = "macintoshRadioButton";
			this.macintoshRadioButton.Size = new System.Drawing.Size(152, 24);
			this.macintoshRadioButton.TabIndex = 1;
			this.macintoshRadioButton.Text = resourceService.GetString("Dialog.Options.IDEOptions.LoadSaveOptions.MacintoshRadioButton");
			this.macintoshRadioButton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			// 
			// unixRadioButton
			// 
			this.unixRadioButton.Location = new System.Drawing.Point(8, 40);
			this.unixRadioButton.Size = new System.Drawing.Size(152, 24);
			this.unixRadioButton.TabIndex = 2;
			this.unixRadioButton.Text = resourceService.GetString("Dialog.Options.IDEOptions.LoadSaveOptions.UnixRadioButton");
			this.unixRadioButton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			// 
			// Form1
			// 
			this.ClientSize = new System.Drawing.Size(352, 277);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.lineTerminatorStyleGroupBox,
																		  this.createBackupCopyCheckBox,
																		  this.loadUserDataCheckBox,
																		  this.saveSeparatorLabel,
																		  this.saveLabel,
																		  this.loadSeparatorLabel,
																		  this.loadLabel});
			this.lineTerminatorStyleGroupBox.ResumeLayout(false);
			this.ResumeLayout(false);

		}
	}
}
