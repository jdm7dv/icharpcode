// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels
{
	/// <summary>
	/// Summary description for Form3.
	/// </summary>
	public class DeployFileProjectOptions : AbstractOptionPanel
	{
		IProject project;
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.OK) {
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				
				if (ControlDictionary["deployTargetTextBox"].Text.Length > 0) {
					if (!fileUtilityService.IsValidFileName(ControlDictionary["deployTargetTextBox"].Text)) {
						MessageBox.Show("Invalid deploy target specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
						return false;
					}
				}

				if (ControlDictionary["deployScriptTextBox"].Text.Length > 0) {
					if (!fileUtilityService.IsValidFileName(ControlDictionary["deployScriptTextBox"].Text)) {
						MessageBox.Show("Invalid deploy script specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
						return false;
					}
					if (!File.Exists(ControlDictionary["deployScriptTextBox"].Text)) {
						MessageBox.Show("Deploy script doesn't exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
						return false;
					}
				}
				
				project.DeployInformation.DeployTarget = ControlDictionary["deployTargetTextBox"].Text;
				project.DeployInformation.DeployScript = ControlDictionary["deployScriptTextBox"].Text;
				
				if (((RadioButton)ControlDictionary["projectFileRadioButton"]).Checked) {
					project.DeployInformation.DeploymentStrategy = DeploymentStrategy.File;
				} else if (((RadioButton)ControlDictionary["compiledAssemblyRadioButton"]).Checked) {
					project.DeployInformation.DeploymentStrategy = DeploymentStrategy.Assembly;
				} else {
					project.DeployInformation.DeploymentStrategy = DeploymentStrategy.Script;
				}
				
				project.DeployInformation.ClearExcludedFiles();
				for (int i = 0; i < ((CheckedListBox)ControlDictionary["projectFilesCheckedListBox"]).Items.Count; ++i) {
					if (!((CheckedListBox)ControlDictionary["projectFilesCheckedListBox"]).GetItemChecked(i)) {
						project.DeployInformation.AddExcludedFile(fileUtilityService.RelativeToAbsolutePath(project.BaseDirectory, ((CheckedListBox)ControlDictionary["projectFilesCheckedListBox"]).Items[i].ToString()));
					}
				}
			}
	    	return true;
		}
		
		void SelectScriptFileEvent(object sender, EventArgs e)
		{
			using (OpenFileDialog fdiag  = new OpenFileDialog()) {
				fdiag.CheckFileExists = true;
			
				if (fdiag.ShowDialog() == DialogResult.OK) {
					ControlDictionary["deployScriptTextBox"].Text = fdiag.FileName;
				}
			}
		}
		
		void SelectTargetFolderEvent(object sender, EventArgs e)
		{
			FolderDialog fdiag = new  FolderDialog();
			
			if (fdiag.DisplayDialog(resourceService.GetString("Dialog.Options.PrjOptions.DeployFile.FolderDialogDescription")) == DialogResult.OK) {
				ControlDictionary["deployTargetTextBox"].Text = fdiag.Path;
			}
		}
		
		void RadioButtonCheckedChange(object sender, EventArgs e)
		{
			ControlDictionary["deployTargetTextBox"].Enabled = ControlDictionary["selectTargetButton"].Enabled = ((RadioButton)ControlDictionary["projectFileRadioButton"]).Checked || ((RadioButton)ControlDictionary["compiledAssemblyRadioButton"]).Checked;
			ControlDictionary["deployScriptTextBox"].Enabled = ControlDictionary["selectScriptFileButton"].Enabled = ((RadioButton)ControlDictionary["scriptFileRadioButton"]).Checked;
		}
		
		void SetValues(object sender, EventArgs e)
		{
			this.project = (IProject)((IProperties)CustomizationObject).GetProperty("Project");
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			foreach (ProjectFile info in project.ProjectFiles) {
				if (info.BuildAction != BuildAction.Exclude) {
					string name = fileUtilityService.AbsoluteToRelativePath(project.BaseDirectory, info.Name);
					((CheckedListBox)ControlDictionary["projectFilesCheckedListBox"]).Items.Add(name, project.DeployInformation.IsFileExcluded(info.Name) ? CheckState.Unchecked : CheckState.Checked);
			    }
			}
			
			ControlDictionary["deployTargetTextBox"].Text = project.DeployInformation.DeployTarget;
			ControlDictionary["deployScriptTextBox"].Text = project.DeployInformation.DeployScript;
			
			((RadioButton)ControlDictionary["projectFileRadioButton"]).Checked = project.DeployInformation.DeploymentStrategy == DeploymentStrategy.File;
			((RadioButton)ControlDictionary["compiledAssemblyRadioButton"]).Checked = project.DeployInformation.DeploymentStrategy == DeploymentStrategy.Assembly;
			((RadioButton)ControlDictionary["scriptFileRadioButton"]).Checked = project.DeployInformation.DeploymentStrategy == DeploymentStrategy.Script;
			
			RadioButtonCheckedChange(null, null);
		}
		
		public DeployFileProjectOptions() : base(Application.StartupPath + @"\..\data\resources\panels\DeployFileOptions.xfrm")
		{
			((RadioButton)ControlDictionary["projectFileRadioButton"]).CheckedChanged += new EventHandler(RadioButtonCheckedChange);
			((RadioButton)ControlDictionary["compiledAssemblyRadioButton"]).CheckedChanged += new EventHandler(RadioButtonCheckedChange);
			((RadioButton)ControlDictionary["scriptFileRadioButton"]).CheckedChanged += new EventHandler(RadioButtonCheckedChange);
			
			CustomizationObjectChanged += new EventHandler(SetValues);
		}
	}	
}