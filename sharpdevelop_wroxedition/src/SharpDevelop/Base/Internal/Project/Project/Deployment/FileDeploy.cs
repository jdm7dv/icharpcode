// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.CodeDom.Compiler;
using System.Windows.Forms;


using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Internal.Project
{
	public class FileDeploy : IDeploymentStrategy
	{
		public void DeployProject(IProject project)
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			if (project.DeployInformation.DeployTarget.Length == 0) {
				MessageBox.Show(resourceService.GetString("Internal.Project.Deploy.ScriptDeploy.DeployWithoutScriptError"), resourceService.GetString("Global.ErrorText"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			
			try {
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				foreach (ProjectFile fInfo in project.ProjectFiles) {
					try { 
						if (!project.DeployInformation.IsFileExcluded(fInfo.Name)) {
							string newFileName = fileUtilityService.GetDirectoryNameWithSeparator(project.DeployInformation.DeployTarget) + fileUtilityService.AbsoluteToRelativePath(project.BaseDirectory, fInfo.Name);
							if (!Directory.Exists(Path.GetDirectoryName(newFileName))) {
								Directory.CreateDirectory(Path.GetDirectoryName(newFileName));
							}
							File.Copy(fInfo.Name, newFileName, true);
						}
					} catch (Exception e) {
						throw new ApplicationException("Error while copying '" + fInfo.Name + "' to '" + fileUtilityService.GetDirectoryNameWithSeparator(project.DeployInformation.DeployTarget) + fileUtilityService.AbsoluteToRelativePath(project.BaseDirectory, fInfo.Name) + "'.\nException thrown was :\n" + e.ToString());
					}
				}
			} catch (Exception e) {
				MessageBox.Show(e.ToString(), resourceService.GetString("Global.ErrorText"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
