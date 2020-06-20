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
	public class CodeGenerationPanel : AbstractOptionPanel
	{
		static readonly string codeGenerationProperty = "SharpDevelop.UI.CodeGenerationOptions";
		PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.OK) {
				IProperties p = (IProperties)propertyService.GetProperty(codeGenerationProperty, new DefaultProperties());
				p.SetProperty("GenerateAdditionalComments", ((CheckBox)ControlDictionary["generateAdditonalCommentsCheckBox"]).Checked);
				p.SetProperty("GenerateDocumentComments",   ((CheckBox)ControlDictionary["generateDocCommentsCheckBox"]).Checked);
				p.SetProperty("UseFullyQualifiedNames",           ((CheckBox)ControlDictionary["useFullTypeNamesCheckBox"]).Checked);
				p.SetProperty("BlankLinesBetweenMembers",   ((CheckBox)ControlDictionary["blankLinesBetweenMemberCheckBox"]).Checked);
				p.SetProperty("ElseOnClosing",              ((CheckBox)ControlDictionary["elseOnClosingCheckbox"]).Checked);
				p.SetProperty("StartBlockOnSameLine",       ((CheckBox)ControlDictionary["startBlockOnTheSameLineCheckBox"]).Checked);
				propertyService.SetProperty(codeGenerationProperty, p);
			}
	    	return true;
		}

		public CodeGenerationPanel() : base(Application.StartupPath + @"\..\data\resources\panels\CodeGenerationOptionsPanel.xfrm")
		{
			IProperties p = (IProperties)propertyService.GetProperty(codeGenerationProperty, new DefaultProperties());
			
			((CheckBox)ControlDictionary["generateAdditonalCommentsCheckBox"]).Checked = p.GetProperty("GenerateAdditionalComments", true);
			((CheckBox)ControlDictionary["generateDocCommentsCheckBox"]).Checked       = p.GetProperty("GenerateDocumentComments", true);
			((CheckBox)ControlDictionary["useFullTypeNamesCheckBox"]).Checked          = p.GetProperty("UseFullyQualifiedNames", true);
			
			((CheckBox)ControlDictionary["blankLinesBetweenMemberCheckBox"]).Checked   = p.GetProperty("BlankLinesBetweenMembers", true);
			((CheckBox)ControlDictionary["elseOnClosingCheckbox"]).Checked             = p.GetProperty("ElseOnClosing", true);
			((CheckBox)ControlDictionary["startBlockOnTheSameLineCheckBox"]).Checked   = p.GetProperty("StartBlockOnSameLine", true);
		}
	}
}
