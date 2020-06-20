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
using System.Xml;

using ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	/// <summary>
	/// Basic "tabbed" options dialog
	/// </summary>
	public class TabbedOptions : System.Windows.Forms.Form
	{
		System.Windows.Forms.Button cancelButton;
		System.Windows.Forms.Button okButton;
		System.Windows.Forms.TabControl optionPanelTabControl;
		
		ArrayList OptionPanels = new ArrayList();
		IProperties properties = null;
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		
		void AcceptEvent(object sender, EventArgs e)
		{
			foreach (AbstractOptionPanel pane in OptionPanels) {
				if (!pane.ReceiveDialogMessage(DialogMessage.OK)) {
					return;
				}
			}
			DialogResult = DialogResult.OK;
		}
		
		void AddOptionPanels(ArrayList dialogPanelDescriptors)
		{
			foreach (IDialogPanelDescriptor descriptor in dialogPanelDescriptors) {
				
				if (descriptor.DialogPanel != null) { // may be null, if it is only a "path"
					descriptor.DialogPanel.CustomizationObject = properties;
					descriptor.DialogPanel.Control.Dock = DockStyle.Fill;
					OptionPanels.Add(descriptor.DialogPanel);
					
					TabPage page = new TabPage(descriptor.Label);
					page.Controls.Add(descriptor.DialogPanel.Control);
					optionPanelTabControl.TabPages.Add(page);
				}
				
				if (descriptor.DialogPanelDescriptors != null) {
					AddOptionPanels(descriptor.DialogPanelDescriptors);
				}
			}
		}
		
		public TabbedOptions(IProperties properties, IAddInTreeNode node)
		{
			this.properties = properties;
			InitializeComponent();
			
			okButton.Click += new EventHandler(AcceptEvent);
			MaximizeBox  = MinimizeBox = false;
			ShowInTaskbar = false;
			StartPosition = FormStartPosition.CenterParent;
			Icon = null;
			
			AddOptionPanels(node.BuildChildItems(this));
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				foreach (TabPage p in optionPanelTabControl.TabPages) {
					p.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		private void InitializeComponent()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			bool flat = Crownwood.Magic.Common.VisualStyle.IDE == (Crownwood.Magic.Common.VisualStyle)propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.VisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.optionPanelTabControl = new System.Windows.Forms.TabControl();
			
			okButton.Location = new System.Drawing.Point(250 - 8, 308);
			okButton.Size = new System.Drawing.Size(74, 23);
			okButton.TabIndex = 1;
			okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | AnchorStyles.Right;
			okButton.Text = resourceService.GetString("Global.OKButtonText");
			okButton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			cancelButton.Location = new System.Drawing.Point(332 - 8, 308);
			cancelButton.Size = new System.Drawing.Size(74, 23);
			cancelButton.TabIndex = 2;
			cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | AnchorStyles.Right;
			cancelButton.Text = resourceService.GetString("Global.CancelButtonText");
			cancelButton.DialogResult = DialogResult.Cancel;
			cancelButton.FlatStyle = flat ? FlatStyle.Flat : FlatStyle.Standard;
			
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.Text = resourceService.GetString("Dialog.Options.ProjectOptions.DialogName");
			
			this.AcceptButton = okButton;
			this.CancelButton = cancelButton;
			this.ClientSize = new System.Drawing.Size(408, 341);
			this.FormBorderStyle = FormBorderStyle.Sizable;
			
//			ControlBox  = false;
			
			optionPanelTabControl.Location = new System.Drawing.Point(8, 8);
			optionPanelTabControl.Text = "optionPanelTabControl";
			optionPanelTabControl.Size = new System.Drawing.Size(392, 296);
			optionPanelTabControl.SelectedIndex = 0;
			optionPanelTabControl.TabIndex = 0;
			optionPanelTabControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom| AnchorStyles.Right |AnchorStyles.Left;
			
			this.Controls.Add(cancelButton);
			this.Controls.Add(okButton);
			this.Controls.Add(optionPanelTabControl);
		}
	}
}
