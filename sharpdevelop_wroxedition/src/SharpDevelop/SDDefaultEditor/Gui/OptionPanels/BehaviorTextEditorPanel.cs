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
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.Core.Services;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Gui.Dialogs;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.OptionPanels
{
	/// <summary>
	/// Summary description for Form8.
	/// </summary>
	public class BehaviorTextEditorPanel : AbstractOptionPanel
	{
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.OK) {
				((IProperties)CustomizationObject).SetProperty("TabsToSpaces",         ((RadioButton)ControlDictionary["convertTabsToSpacesRadioButton"]).Checked);
				((IProperties)CustomizationObject).SetProperty("MouseWheelScrollDown", ((ComboBox)ControlDictionary["mouseWhellDirectionComboBox"]).SelectedIndex == 0);
				
				((IProperties)CustomizationObject).SetProperty("AutoInsertCurlyBracket", ((CheckBox)ControlDictionary["autoinsertCurlyBraceCheckBox"]).Checked);
				((IProperties)CustomizationObject).SetProperty("HideMouseCursor",        ((CheckBox)ControlDictionary["hideMouseCursorCheckBox"]).Checked);
				((IProperties)CustomizationObject).SetProperty("CursorBehindEOL",        ((CheckBox)ControlDictionary["caretBehindEOLCheckBox"]).Checked);
				((IProperties)CustomizationObject).SetProperty("AutoInsertTemplates",    ((CheckBox)ControlDictionary["auotInsertTemplatesCheckBox"]).Checked);
				
				((IProperties)CustomizationObject).SetProperty("IndentStyle", ((ComboBox)ControlDictionary["indentStyleComboBox"]).SelectedIndex);
				
				try {
					((IProperties)CustomizationObject).SetProperty("TabIndent", Int32.Parse(ControlDictionary["tabSizeTextBox"].Text));
				} catch (Exception) {
				}
				
				try {
					((IProperties)CustomizationObject).SetProperty("IndentationSize", Int32.Parse(ControlDictionary["indentSizeTextBox"].Text));
				} catch (Exception) {
				}
			}
			return true;
		}

		void SetValues(object sender, EventArgs e)
		{
			((CheckBox)ControlDictionary["autoinsertCurlyBraceCheckBox"]).Checked = ((IProperties)CustomizationObject).GetProperty("AutoInsertCurlyBracket", true);
			((CheckBox)ControlDictionary["hideMouseCursorCheckBox"]).Checked      = ((IProperties)CustomizationObject).GetProperty("HideMouseCursor", true);
			((CheckBox)ControlDictionary["caretBehindEOLCheckBox"]).Checked       = ((IProperties)CustomizationObject).GetProperty("CursorBehindEOL", false);
			((CheckBox)ControlDictionary["auotInsertTemplatesCheckBox"]).Checked  = ((IProperties)CustomizationObject).GetProperty("AutoInsertTemplates", true);
			
			((RadioButton)ControlDictionary["convertTabsToSpacesRadioButton"]).Checked  = ((IProperties)CustomizationObject).GetProperty("TabsToSpaces", false);
			((RadioButton)ControlDictionary["leaveTabsRadioButton"]).Checked  = !((RadioButton)ControlDictionary["convertTabsToSpacesRadioButton"]).Checked ;
			
			
			ControlDictionary["tabSizeTextBox"].Text    = ((IProperties)CustomizationObject).GetProperty("TabIndent", 4).ToString();
			ControlDictionary["indentSizeTextBox"].Text = ((IProperties)CustomizationObject).GetProperty("IndentationSize", 4).ToString();
			
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			((ComboBox)ControlDictionary["indentStyleComboBox"]).Items.Add(resourceService.GetString("Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentStyle.None"));
			((ComboBox)ControlDictionary["indentStyleComboBox"]).Items.Add(resourceService.GetString("Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentStyle.Automatic"));
			((ComboBox)ControlDictionary["indentStyleComboBox"]).Items.Add(resourceService.GetString("Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentStyle.Smart"));
			
			((ComboBox)ControlDictionary["indentStyleComboBox"]).SelectedIndex = (int)(IndentStyle)((IProperties)CustomizationObject).GetProperty("IndentStyle", IndentStyle.Smart);
		
			((ComboBox)ControlDictionary["mouseWhellDirectionComboBox"]).Items.Add(resourceService.GetString("Dialog.Options.IDEOptions.TextEditor.Behaviour.NormalMouseDirectionRadioButton"));
			((ComboBox)ControlDictionary["mouseWhellDirectionComboBox"]).Items.Add(resourceService.GetString("Dialog.Options.IDEOptions.TextEditor.Behaviour.ReverseMouseDirectionRadioButton"));
			((ComboBox)ControlDictionary["mouseWhellDirectionComboBox"]).SelectedIndex = ((IProperties)CustomizationObject).GetProperty("MouseWheelScrollDown", true) ? 0 : 1;
		}
		
		public BehaviorTextEditorPanel() : base(Application.StartupPath + @"\..\data\resources\panels\BehaviorTextEditorPanel.xfrm")
		{
			CustomizationObjectChanged += new EventHandler(SetValues);
		}
	}
}
