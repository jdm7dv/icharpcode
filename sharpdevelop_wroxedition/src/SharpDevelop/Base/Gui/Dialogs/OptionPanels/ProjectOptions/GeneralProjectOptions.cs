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

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels
{
	public class GeneralProjectOptions : AbstractOptionPanel
	{
		IProject project;
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.OK) {
				project.Name                 = ControlDictionary["ProjectNameTextBox"].Text;
				project.Description          = ControlDictionary["ProjectDescriptionTextBox"].Text;
				
				project.EnableViewState = ((CheckBox)ControlDictionary["EnableViewStateCheckBox"]).Checked;
				
				if (((CheckBox)ControlDictionary["NewFilesOnLoadCheckBox"]).Checked) {
					project.NewFileSearch = ((CheckBox)ControlDictionary["AutoInsertNewFilesCheckBox"]).Checked ?  NewFileSearch.OnLoadAutoInsert : NewFileSearch.OnLoad;
				} else {
					project.NewFileSearch = NewFileSearch.None;
				}
			}
			return true;
		}
		
		void AutoLoadCheckBoxCheckedChangeEvent(object sender, EventArgs e)
		{
			((CheckBox)ControlDictionary["AutoInsertNewFilesCheckBox"]).Enabled = ((CheckBox)ControlDictionary["NewFilesOnLoadCheckBox"]).Checked;
		}
		
		void BrowseFileEvent(object sender, EventArgs e)
		{
			using (OpenFileDialog fdiag  = new OpenFileDialog()) {
				fdiag.CheckFileExists = true;
				
				if (fdiag.ShowDialog() == DialogResult.OK) {
					ControlDictionary["ProjectDocumentationLocationTextBox"].Text = fdiag.FileName;
				}
			}
		}
		
		void SetValues(object sender, EventArgs e)
		{
			this.project = (IProject)((IProperties)CustomizationObject).GetProperty("Project");
			
			ControlDictionary["ProjectNameTextBox"].Text        = project.Name;
			ControlDictionary["ProjectDescriptionTextBox"].Text = project.Description;
			
			((CheckBox)ControlDictionary["EnableViewStateCheckBox"]).Checked = project.EnableViewState;
			
			switch (project.NewFileSearch) {
				case NewFileSearch.None:
					((CheckBox)ControlDictionary["NewFilesOnLoadCheckBox"]).Checked = ((CheckBox)ControlDictionary["AutoInsertNewFilesCheckBox"]).Checked = false;
					break;
				case NewFileSearch.OnLoad:
					((CheckBox)ControlDictionary["NewFilesOnLoadCheckBox"]).Checked = true;
					((CheckBox)ControlDictionary["AutoInsertNewFilesCheckBox"]).Checked = false;
					break;
				default:
					((CheckBox)ControlDictionary["NewFilesOnLoadCheckBox"]).Checked = ((CheckBox)ControlDictionary["AutoInsertNewFilesCheckBox"]).Checked = true;
					break;
			}
			AutoLoadCheckBoxCheckedChangeEvent(null, null);
		}
		
		public GeneralProjectOptions() : base(Application.StartupPath + @"\..\data\resources\panels\GeneralProjectOptions.xfrm")
		{
			CustomizationObjectChanged += new EventHandler(SetValues);
		}
	}
}
