// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Internal.Project
{
	public class ScriptDeploy : IDeploymentStrategy
	{
		public void DeployProject(IProject project)
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			if (project.DeployInformation.DeployScript.Length == 0) {
				MessageBox.Show(resourceService.GetString("Internal.Project.Deploy.ScriptDeploy.DeployWithoutScriptError"), resourceService.GetString("Global.ErrorText"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			try {
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				if (fileUtilityService.TestFileExists(project.DeployInformation.DeployScript)) {
					ProcessStartInfo pInfo = new ProcessStartInfo(project.DeployInformation.DeployScript);
					pInfo.WorkingDirectory = Path.GetDirectoryName(project.DeployInformation.DeployScript);
					Process.Start(pInfo);
				}
			} catch (Exception e) {
				MessageBox.Show(resourceService.GetString("Internal.Project.Deploy.ScriptDeploy.ErrorWhileExecuteScript") + "\n" + e.ToString(), resourceService.GetString("Global.ErrorText"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
