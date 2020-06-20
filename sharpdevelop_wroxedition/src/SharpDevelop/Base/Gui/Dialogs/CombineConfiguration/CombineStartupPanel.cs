// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Internal.Project;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels
{
	/// <summary>
	/// Summary description for UserControl1.
	/// </summary>
	public class CombineStartupPanel : AbstractOptionPanel
	{
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ComboBox singlecomboBox;
		private System.Windows.Forms.ComboBox actioncomboBox;
		private System.Windows.Forms.ListView entrylistView;
		private System.Windows.Forms.Button moveupButton;
		private System.Windows.Forms.Button movedownButton;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		
		Combine combine;

		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.OK) {
				// write back singlestartup project
				combine.SingleStartProjectName = singlecomboBox.Text;
				combine.SingleStartupProject   = radioButton1.Checked;
				
				// write back new combine execute definitions
				combine.CombineExecuteDefinitions.Clear();
				foreach (ListViewItem item in entrylistView.Items) {
					EntryExecuteType type = EntryExecuteType.None;
					if (item.SubItems[1].Text == resourceService.GetString("Dialog.Options.CombineOptions.Startup.Action.Execute")) {
						type = EntryExecuteType.Execute;
					}				
					combine.CombineExecuteDefinitions.Add(new CombineExecuteDefinition(
						combine.GetEntry(item.Text),
						type
					));
				}
			}
			return true;
		}
		
		void SetValues(object sender, EventArgs e)
		{
			this.combine = (Combine)((IProperties)CustomizationObject).GetProperty("Combine");
			
			radioButton1.Checked =  combine.SingleStartupProject;
			radioButton2.Checked = !combine.SingleStartupProject;
			
			foreach (CombineEntry entry in combine.Entries) 
				singlecomboBox.Items.Add(entry.Name);
			
			singlecomboBox.SelectedIndex = combine.GetEntryNumber(combine.SingleStartProjectName);
			
			radioButton1.CheckedChanged += new EventHandler(CheckedChanged);
			
			entrylistView.SelectedIndexChanged += new EventHandler(SelectedEntryChanged);
			actioncomboBox.SelectedIndexChanged += new EventHandler(OptionsChanged);
			foreach (CombineExecuteDefinition edef in combine.CombineExecuteDefinitions) {
				entrylistView.Items.Add(new ListViewItem(new string[] {
					edef.Entry.Name,
					edef.Type == EntryExecuteType.None ? resourceService.GetString("Dialog.Options.CombineOptions.Startup.Action.None") : resourceService.GetString("Dialog.Options.CombineOptions.Startup.Action.Execute")
				}));
			}
			CheckedChanged(null, null);
		}
		
		public CombineStartupPanel()
		{
			// This call is required by the Windows.Forms Form Designer. 
			InitializeComponent();
			
			CustomizationObjectChanged += new EventHandler(SetValues);
		}
		
		void CheckedChanged(object sender, EventArgs e)
		{
			entrylistView.Enabled = actioncomboBox.Enabled = radioButton2.Checked;
			singlecomboBox.Enabled = radioButton1.Checked;
		}
		void OptionsChanged(object sender, EventArgs e)
		{
			if (entrylistView.SelectedItems == null || 
				entrylistView.SelectedItems.Count == 0) 
				return;
			ListViewItem item = entrylistView.SelectedItems[0]; 
			item.SubItems[1].Text = actioncomboBox.SelectedItem.ToString();
			
		}
		void SelectedEntryChanged(object sender, EventArgs e)
		{
			if (entrylistView.SelectedItems == null || 
				entrylistView.SelectedItems.Count == 0) 
				return;
			ListViewItem item = entrylistView.SelectedItems[0]; 
			string       txt = item.SubItems[1].Text;
			actioncomboBox.Items.Clear();
			actioncomboBox.Items.Add(resourceService.GetString("Dialog.Options.CombineOptions.Startup.Action.None"));
			actioncomboBox.Items.Add(resourceService.GetString("Dialog.Options.CombineOptions.Startup.Action.Execute"));
			
			if (txt == resourceService.GetString("Dialog.Options.CombineOptions.Startup.Action.None")) {
				actioncomboBox.SelectedIndex = 0;
			} else {
				actioncomboBox.SelectedIndex = 1;
			}
			
			
			
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			bool flat = Crownwood.Magic.Common.VisualStyle.IDE == (Crownwood.Magic.Common.VisualStyle)propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.VisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);
			
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.actioncomboBox = new System.Windows.Forms.ComboBox();
			this.radioButton2 = new System.Windows.Forms.RadioButton();
			this.entrylistView = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.movedownButton = new System.Windows.Forms.Button();
			this.moveupButton = new System.Windows.Forms.Button();
			this.singlecomboBox = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// radioButton1
			// 
			this.radioButton1.Location = new System.Drawing.Point(8, 8);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(264, 24);
			this.radioButton1.TabIndex = 0;
			this.radioButton1.Text = resourceService.GetString("Dialog.Options.CombineOptions.Startup.SingleStartupRadioButton");
			radioButton1.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			//
			// actioncomboBox
			// 
			this.actioncomboBox.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.actioncomboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.actioncomboBox.DropDownWidth = 168;
			this.actioncomboBox.Location = new System.Drawing.Point(348, 104);
			this.actioncomboBox.Name = "actioncomboBox";
			this.actioncomboBox.Size = new System.Drawing.Size(128, 21);
			this.actioncomboBox.TabIndex = 5;
			// 
			// radioButton2
			// 
			this.radioButton2.Location = new System.Drawing.Point(8, 64);
			this.radioButton2.Name = "radioButton2";
			this.radioButton2.Size = new System.Drawing.Size(264, 24);
			this.radioButton2.TabIndex = 1;
			this.radioButton2.Text = resourceService.GetString("Dialog.Options.CombineOptions.Startup.MultipleStartupRadioButton");
			radioButton2.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			//
			// entrylistView
			// 
			this.entrylistView.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.entrylistView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							this.columnHeader1,
																							this.columnHeader2});
			this.entrylistView.FullRowSelect = true;
			this.entrylistView.GridLines = true;
			this.entrylistView.HideSelection = false;
			this.entrylistView.Location = new System.Drawing.Point(8, 88);
			this.entrylistView.MultiSelect = false;
			this.entrylistView.Name = "entrylistView";
			this.entrylistView.Size = new System.Drawing.Size(336, 240);
			this.entrylistView.TabIndex = 6;
			this.entrylistView.View = System.Windows.Forms.View.Details;
			entrylistView.BorderStyle  = flat ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
			
			//
			// columnHeader1
			// 
			this.columnHeader1.Text = resourceService.GetString("Dialog.Options.CombineOptions.Startup.EntryColumnHeader");
			this.columnHeader1.Width = 185;
			
			//
			// columnHeader2
			// 
			this.columnHeader2.Text = resourceService.GetString("Dialog.Options.CombineOptions.Startup.ActionColumnHeader");
			this.columnHeader2.Width = 100;
			
			// 
			// movedownButton
			// 
			this.movedownButton.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.movedownButton.Location = new System.Drawing.Point(348, 168);
			this.movedownButton.Name = "movedownButton";
			this.movedownButton.TabIndex = 8;
			this.movedownButton.Text = resourceService.GetString("Dialog.Options.CombineOptions.Startup.MoveDownButton");
			movedownButton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			//
			// moveupButton
			// 
			this.moveupButton.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.moveupButton.Location = new System.Drawing.Point(348, 136);
			this.moveupButton.Name = "moveupButton";
			this.moveupButton.TabIndex = 7;
			this.moveupButton.Text = resourceService.GetString("Dialog.Options.CombineOptions.Startup.MoveUpButton");
			moveupButton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			//
			// singlecomboBox
			// 
			this.singlecomboBox.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.singlecomboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.singlecomboBox.DropDownWidth = 264;
			this.singlecomboBox.Location = new System.Drawing.Point(8, 32);
			this.singlecomboBox.Name = "singlecomboBox";
			this.singlecomboBox.Size = new System.Drawing.Size(304, 21);
			this.singlecomboBox.TabIndex = 2;
			
			// 
			// label1
			// 
			this.label1.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.label1.Location = new System.Drawing.Point(348, 88);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 16);
			this.label1.TabIndex = 4;
			this.label1.Text = resourceService.GetString("Dialog.Options.CombineOptions.Startup.ActionLabel");
			
			//
			// UserControl1
			//
			this.Controls.AddRange(new System.Windows.Forms.Control[] {this.movedownButton,
			                       this.moveupButton,
			                       this.entrylistView,
			                       this.actioncomboBox,
			                       this.label1,
			                       this.singlecomboBox,
			                       this.radioButton2,
			                       this.radioButton1});
			this.Name = "UserControl1";
			this.Size = new System.Drawing.Size(488, 336);
			this.ResumeLayout(false);
			
		}
		#endregion
	}
}

