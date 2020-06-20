// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.SharpDevelop.Gui.Dialogs;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.OptionPanels
{
	/// <summary>
	/// Summary description for Form9.
	/// </summary>
	public class MarkersTextEditorPanel : AbstractOptionPanel
	{
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.OK) {
		    	((IProperties)CustomizationObject).SetProperty("ShowInvalidLines",     ((CheckBox)ControlDictionary["showInvalidLinesCheckBox"]).Checked);
				((IProperties)CustomizationObject).SetProperty("ShowLineNumbers",      ((CheckBox)ControlDictionary["showLineNumberCheckBox"]).Checked);
				((IProperties)CustomizationObject).SetProperty("ShowBracketHighlight", ((CheckBox)ControlDictionary["showBracketHighlighterCheckBox"]).Checked);
				((IProperties)CustomizationObject).SetProperty("ShowErrors",           ((CheckBox)ControlDictionary["showErrorsCheckBox"]).Checked);
				((IProperties)CustomizationObject).SetProperty("ShowHRuler",           ((CheckBox)ControlDictionary["showHRulerCheckBox"]).Checked);
				((IProperties)CustomizationObject).SetProperty("ShowEOLMarkers",       ((CheckBox)ControlDictionary["showEOLMarkersCheckBox"]).Checked);
				((IProperties)CustomizationObject).SetProperty("ShowVRuler",           ((CheckBox)ControlDictionary["showVRulerCheckBox"]).Checked);
				((IProperties)CustomizationObject).SetProperty("ShowTabs",             ((CheckBox)ControlDictionary["showTabCharsCheckBox"]).Checked);
				((IProperties)CustomizationObject).SetProperty("ShowSpaces",           ((CheckBox)ControlDictionary["showSpaceCharsCheckBox"]).Checked);
		    	
		    	try {
		    		((IProperties)CustomizationObject).SetProperty("VRulerRow", Int32.Parse(ControlDictionary["vRulerRowTextBox"].Text));
		    	} catch (Exception) {
		    	}
	
				((IProperties)CustomizationObject).SetProperty("LineViewerStyle", (LineViewerStyle)((ComboBox)ControlDictionary["lineMarkerStyleComboBox"]).SelectedIndex);
			}
			return true;
	    }
		
		void SetValues(object sender, EventArgs e)
		{
			((CheckBox)ControlDictionary["showLineNumberCheckBox"]).Checked         = ((IProperties)CustomizationObject).GetProperty("ShowLineNumbers", true);
			((CheckBox)ControlDictionary["showInvalidLinesCheckBox"]).Checked       = ((IProperties)CustomizationObject).GetProperty("ShowInvalidLines", true);
			((CheckBox)ControlDictionary["showBracketHighlighterCheckBox"]).Checked = ((IProperties)CustomizationObject).GetProperty("ShowBracketHighlight", true);
			((CheckBox)ControlDictionary["showErrorsCheckBox"]).Checked             = ((IProperties)CustomizationObject).GetProperty("ShowErrors", true);
			((CheckBox)ControlDictionary["showHRulerCheckBox"]).Checked             = ((IProperties)CustomizationObject).GetProperty("ShowHRuler", false);
			((CheckBox)ControlDictionary["showEOLMarkersCheckBox"]).Checked         = ((IProperties)CustomizationObject).GetProperty("ShowEOLMarkers", false);
			((CheckBox)ControlDictionary["showVRulerCheckBox"]).Checked             = ((IProperties)CustomizationObject).GetProperty("ShowVRuler", false);
			((CheckBox)ControlDictionary["showTabCharsCheckBox"]).Checked           = ((IProperties)CustomizationObject).GetProperty("ShowTabs", false);
			((CheckBox)ControlDictionary["showSpaceCharsCheckBox"]).Checked         = ((IProperties)CustomizationObject).GetProperty("ShowSpaces", false);
			
			ControlDictionary["vRulerRowTextBox"].Text = ((IProperties)CustomizationObject).GetProperty("VRulerRow", 80).ToString();
			
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			((ComboBox)ControlDictionary["lineMarkerStyleComboBox"]).Items.Add(resourceService.GetString("Dialog.Options.IDEOptions.TextEditor.Markers.LineViewerStyle.None"));
			((ComboBox)ControlDictionary["lineMarkerStyleComboBox"]).Items.Add(resourceService.GetString("Dialog.Options.IDEOptions.TextEditor.Markers.LineViewerStyle.FullRow"));
			((ComboBox)ControlDictionary["lineMarkerStyleComboBox"]).SelectedIndex = (int)(LineViewerStyle)((IProperties)CustomizationObject).GetProperty("LineViewerStyle", LineViewerStyle.None);
		}
		
		public MarkersTextEditorPanel() : base(Application.StartupPath + @"\..\data\resources\panels\MarkersTextEditorPanel.xfrm")
		{
			CustomizationObjectChanged += new EventHandler(SetValues);
		}
	}
}
