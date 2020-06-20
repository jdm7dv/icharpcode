// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels
{
	public class SelectStylePanel : AbstractOptionPanel
	{
		CheckBox showExtensionsCheckBox = new CheckBox();
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.OK) {
				propertyService.SetProperty("SharpDevelop.Workbench.WorkbenchLayout", ((RadioButton)ControlDictionary["sdiRadioButton"]).Checked ? "SDI" : "MDI");
				propertyService.SetProperty("ICSharpCode.SharpDevelop.Gui.VisualStyle", ((CheckBox)ControlDictionary["enableFlatStyleCheckBox"]).Checked ? Crownwood.Magic.Common.VisualStyle.IDE : Crownwood.Magic.Common.VisualStyle.Plain);
				propertyService.SetProperty("ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ShowExtensions", ((CheckBox)ControlDictionary["showExtensionsCheckBox"]).Checked);
				propertyService.SetProperty("SharpDevelop.UI.CurrentAmbience", ((ComboBox)ControlDictionary["selectAmbienceComboBox"]).Text);
			}
			return true;
		}

		public SelectStylePanel() : base(Application.StartupPath + @"\..\data\resources\panels\SelectStylePanel.xfrm")
		{
			((RadioButton)ControlDictionary["sdiRadioButton"]).Checked = propertyService.GetProperty("SharpDevelop.Workbench.WorkbenchLayout", "MDI") == "SDI";
			((RadioButton)ControlDictionary["mdiRadioButton"]).Checked = !((RadioButton)ControlDictionary["sdiRadioButton"]).Checked;
			
			((CheckBox)ControlDictionary["enableFlatStyleCheckBox"]).Checked  = (Crownwood.Magic.Common.VisualStyle)propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.VisualStyle", Crownwood.Magic.Common.VisualStyle.IDE) == Crownwood.Magic.Common.VisualStyle.IDE;
			((CheckBox)ControlDictionary["showExtensionsCheckBox"]).Checked  = propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ShowExtensions", true);
			
			IAddInTreeNode treeNode = AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/Workbench/Ambiences");
			foreach (IAddInTreeNode childNode in treeNode.ChildNodes.Values) {
				((ComboBox)ControlDictionary["selectAmbienceComboBox"]).Items.Add(childNode.Codon.ID);
			}			
			
			((ComboBox)ControlDictionary["selectAmbienceComboBox"]).Text = propertyService.GetProperty("SharpDevelop.UI.CurrentAmbience", "CSharp");
		}
	}
}
