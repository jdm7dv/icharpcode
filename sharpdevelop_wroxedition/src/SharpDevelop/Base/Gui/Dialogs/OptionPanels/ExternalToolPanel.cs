// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels
{
	public class ExternalToolPane : AbstractOptionPanel
	{
		private System.ComponentModel.Container components;
//		private System.Windows.Forms.Button helpButton;
		private System.Windows.Forms.Button removeButton;
		private System.Windows.Forms.Button addButton;
		
//		private System.Windows.Forms.Button       okButton;
//		private System.Windows.Forms.Button       cancelButton      =  new Button();
		private System.Windows.Forms.PropertyGrid toolPropertyGrid;
		private System.Windows.Forms.Label        toolsLabel;
		private System.Windows.Forms.ListBox      toolListBox;
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.OK) {
				ArrayList newlist = new ArrayList();
				foreach (object o in toolListBox.Items) {
					if (o != null)
						newlist.Add(o);
				}
				ToolLoader.Tool = newlist;
				ToolLoader.SaveTools();
			}
			return true;
		}
		
		void selectEvent(object sender, EventArgs e)
		{
			toolPropertyGrid.SelectedObject = toolListBox.SelectedItem;
		}
		
		void removeEvent(object sender, EventArgs e)
		{
			if (toolListBox.SelectedIndex != -1) {
				toolListBox.Items.Remove(toolListBox.Items[toolListBox.SelectedIndex]);
			}
		}
		
		void addEvent(object sender, EventArgs e)
		{
			toolListBox.Items.Add(new ExternalTool());
		}
		
		public ExternalToolPane() //: base(resourceService.GetString("Dialog.Options.ExternalToolsText"))
		{
			InitializeComponents();
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
		void InitializeComponents()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			bool flat = Crownwood.Magic.Common.VisualStyle.IDE == (Crownwood.Magic.Common.VisualStyle)propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.VisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);
			
			this.components = new System.ComponentModel.Container();
//			this.helpButton = new System.Windows.Forms.Button();
			this.removeButton = new System.Windows.Forms.Button();
			this.toolsLabel = new System.Windows.Forms.Label();
			this.addButton = new System.Windows.Forms.Button();
//			this.okButton = new System.Windows.Forms.Button();
			this.toolListBox = new System.Windows.Forms.ListBox();
			this.toolPropertyGrid = new System.Windows.Forms.PropertyGrid();
			
			removeButton.Location = new System.Drawing.Point(90, 248);
			removeButton.Size = new System.Drawing.Size(74, 24);
			removeButton.TabIndex = 6;
			removeButton.Text = resourceService.GetString("Global.RemoveButtonText");
			removeButton.Anchor       = AnchorStyles.Bottom | AnchorStyles.Left;
			removeButton.Click += new EventHandler(removeEvent);
			removeButton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			toolsLabel.Location = new System.Drawing.Point(8, 8);
			toolsLabel.Text = resourceService.GetString("Dialog.Options.ExternalTool.ToolsLabel");
			
			toolsLabel.Size = new System.Drawing.Size(100, 16);
			toolsLabel.TabIndex = 1;
			
//			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			//@design this.TrayLargeIcon = true;
			//@design this.TrayHeight = 0;
			this.ClientSize = new System.Drawing.Size(368 + 60, 277);
			
			addButton.Location = new System.Drawing.Point(8, 248);
			addButton.Size = new System.Drawing.Size(74, 24);
			addButton.TabIndex = 5;
			addButton.Text = resourceService.GetString("Global.AddButtonText");
			addButton.Anchor       = AnchorStyles.Bottom | AnchorStyles.Left;
			addButton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			toolListBox.Location = new System.Drawing.Point(8, 24);
			toolListBox.Size = new System.Drawing.Size(144, 212);
			toolListBox.TabIndex = 0;
			toolListBox.Anchor       = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			toolListBox.BorderStyle  = flat ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
			
			toolPropertyGrid.Location = new System.Drawing.Point(168, 8);
			toolPropertyGrid.Text = "PropertyGrid";
			toolPropertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
//			toolPropertyGrid.OutlineColor = System.Drawing.SystemColors.GrayText;
//			toolPropertyGrid.TextColor = System.Drawing.SystemColors.WindowText;
			toolPropertyGrid.Size = new System.Drawing.Size(184 + 60, 232);
			toolPropertyGrid.LargeButtons = false;
//			toolPropertyGrid.ActiveDocument = null;
			toolPropertyGrid.TabIndex = 2;
			toolPropertyGrid.CommandsVisibleIfAvailable = true;
			toolPropertyGrid.Anchor       = AnchorStyles.Top | AnchorStyles.Bottom| AnchorStyles.Right |AnchorStyles.Left;
			
			
//			this.Controls.Add(helpButton);
			this.Controls.Add(removeButton);
			this.Controls.Add(addButton);
//			this.Controls.Add(okButton);
//			this.Controls.Add(cancelButton);
			this.Controls.Add(toolPropertyGrid);
			this.Controls.Add(toolsLabel);
			this.Controls.Add(toolListBox);
			
			// Required for Win Form Designer support
			//        	CancelButton = cancelButton;
			//        	AcceptButton = okButton;
			foreach (object o in ToolLoader.Tool)
				toolListBox.Items.Add(o);
			toolListBox.SelectedIndexChanged += new EventHandler(selectEvent);
			//        	okButton.Click += new EventHandler(closeEvent);
			removeButton.Click+=new EventHandler(removeEvent);
			addButton.Click += new EventHandler(addEvent);
			//			MaximizeBox  = MinimizeBox = false;
			
		}
	}
}
