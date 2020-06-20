// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

namespace VBBinding
{
	public class OutputOptionsPanel : AbstractOptionPanel
	{
		VBCompilerParameters compilerParameters;
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.OK) {
				if (compilerParameters == null) {
					return true;
				}
				
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				if (!fileUtilityService.IsValidFileName(ControlDictionary["assemblyNameTextBox"].Text)) {
					MessageBox.Show("Invalid assembly name specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
					return false;
				}
				if (!fileUtilityService.IsValidFileName(ControlDictionary["outputDirectoryTextBox"].Text)) {
					MessageBox.Show("Invalid output directory specified", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
					return false;
				}
				
				compilerParameters.OutputAssembly  = ControlDictionary["assemblyNameTextBox"].Text;
				compilerParameters.OutputDirectory = ControlDictionary["outputDirectoryTextBox"].Text;
				compilerParameters.CommandLineParameters = ControlDictionary["parametersTextBox"].Text;
				
				compilerParameters.PauseConsoleOutput = ((CheckBox)ControlDictionary["pauseConsoleOutputCheckBox"]).Checked;
			}
			return true;
		}
	
		void SetValues(object sender, EventArgs e)
		{
			this.compilerParameters = (VBCompilerParameters)((IProperties)CustomizationObject).GetProperty("Config");
			
			ControlDictionary["assemblyNameTextBox"].Text = compilerParameters.OutputAssembly;
			ControlDictionary["outputDirectoryTextBox"].Text = compilerParameters.OutputDirectory;
			ControlDictionary["parametersTextBox"].Text = compilerParameters.CommandLineParameters;
			
			((CheckBox)ControlDictionary["pauseConsoleOutputCheckBox"]).Checked = compilerParameters.PauseConsoleOutput;
		}
		
		public OutputOptionsPanel() : base(Application.StartupPath + @"\..\data\resources\panels\ProjectOptions\OutputPanel.xfrm")
		{
			CustomizationObjectChanged += new EventHandler(SetValues);
		}
	}

}
