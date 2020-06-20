// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Windows.Forms;

using ICSharpCode.Core.AddIns;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.SharpDevelop.Internal.Project;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class RunTestsInProject : AbstractMenuCommand
	{
		public override void Run()
		{
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			
			if (projectService.CurrentSelectedProject != null) {
				LanguageBindingService languageBindingService = (LanguageBindingService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(LanguageBindingService));
				ILanguageBinding csc = languageBindingService.GetBindingPerLanguageName(projectService.CurrentSelectedProject.ProjectType);
				string assembly = csc.GetCompiledOutputName(projectService.CurrentSelectedProject);
				
				if (!File.Exists(assembly)) {
					MessageBox.Show("Compile the project first", "Assembly not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
				} else {
					FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
					string command = fileUtilityService.GetDirectoryNameWithSeparator(Application.StartupPath) + "SharpUnit.exe";
					string args = '"' + assembly + '"';
					Process.Start(command, args);
				}
			}
		}
	}

	public class ViewProjectOptions : AbstractMenuCommand
	{
		public override void Run()
		{
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			
			if (projectService.CurrentSelectedProject == null) {
				return;
			}
			
			using (ProjectOptionsDialog optionsDialog = new ProjectOptionsDialog(projectService.CurrentSelectedProject,
			                                                                     AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/Workbench/ProjectOptions/GeneralOptions"),
			                                                                     AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/Workbench/ProjectOptions/ConfigurationProperties"))) {
			
				optionsDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
				
				optionsDialog.Owner = (Form)WorkbenchSingleton.Workbench;
				if (optionsDialog.ShowDialog() == DialogResult.OK) {
					projectService.MarkProjectDirty(projectService.CurrentSelectedProject);
				}
			}
			
			projectService.SaveCombine();
		}
	}
	
	public class DeployProject : AbstractMenuCommand
	{
		public override void Run()
		{
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			
			if (projectService.CurrentSelectedProject != null) {
				DeployInformation.Deploy(projectService.CurrentSelectedProject);
			}
		}
	}
	
	public class GenerateProjectDocumentation : AbstractMenuCommand
	{
		public override void Run()
		{
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			
			if (projectService.CurrentSelectedProject != null) {
				LanguageBindingService languageBindingService = (LanguageBindingService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(LanguageBindingService));
				ILanguageBinding csc = languageBindingService.GetBindingPerLanguageName(projectService.CurrentSelectedProject.ProjectType);
				
				string assembly    = csc.GetCompiledOutputName(projectService.CurrentSelectedProject);
				string projectFile = Path.ChangeExtension(assembly, ".ndoc");
				if (!File.Exists(projectFile)) {
					StreamWriter sw = File.CreateText(projectFile);
					sw.WriteLine("<project>");
					sw.WriteLine("    <assemblies>");
					sw.WriteLine("        <assembly location=\""+ assembly +"\" documentation=\"" + Path.ChangeExtension(assembly, ".xml") + "\" />");
					sw.WriteLine("    </assemblies>");
					/*
					sw.WriteLine("    				    <documenters>");
					sw.WriteLine("    				        <documenter name=\"JavaDoc\">");
					sw.WriteLine("    				            <property name=\"Title\" value=\"NDoc\" />");
					sw.WriteLine("    				            <property name=\"OutputDirectory\" value=\".\\docs\\JavaDoc\" />");
					sw.WriteLine("    				            <property name=\"ShowMissingSummaries\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"ShowMissingRemarks\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"ShowMissingParams\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"ShowMissingReturns\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"ShowMissingValues\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"DocumentInternals\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"DocumentProtected\" value=\"True\" />");
					sw.WriteLine("    				            <property name=\"DocumentPrivates\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"DocumentEmptyNamespaces\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"IncludeAssemblyVersion\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"CopyrightText\" value=\"\" />");
					sw.WriteLine("    				            <property name=\"CopyrightHref\" value=\"\" />");
					sw.WriteLine("    				        </documenter>");
					sw.WriteLine("    				        <documenter name=\"MSDN\">");
					sw.WriteLine("    				            <property name=\"OutputDirectory\" value=\".\\docs\\MSDN\" />");
					sw.WriteLine("    				            <property name=\"HtmlHelpName\" value=\"NDoc\" />");
					sw.WriteLine("    				            <property name=\"HtmlHelpCompilerFilename\" value=\"C:\\Program Files\\HTML Help Workshop\\hhc.exe\" />");
					sw.WriteLine("    				            <property name=\"IncludeFavorites\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"Title\" value=\"An NDoc Documented Class Library\" />");
					sw.WriteLine("    				            <property name=\"SplitTOCs\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"DefaulTOC\" value=\"\" />");
					sw.WriteLine("    				            <property name=\"ShowVisualBasic\" value=\"True\" />");
					sw.WriteLine("    				            <property name=\"ShowMissingSummaries\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"ShowMissingRemarks\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"ShowMissingParams\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"ShowMissingValues\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"DocumentInternals\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"DocumentProtected\" value=\"True\" />");
					sw.WriteLine("    				            <property name=\"DocumentPrivates\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"DocumentEmptyNamespaces\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"IncludeAssemblyVersion\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"CopyrightText\" value=\"\" />");
					sw.WriteLine("                <property name=\"CopyrightHref\" value=\"\" />");
					sw.WriteLine("            </documenter>");
					sw.WriteLine("    				        <documenter name=\"XML\">");
					sw.WriteLine("    				            <property name=\"OutputFile\" value=\".\\docs\\doc.xml\" />");
					sw.WriteLine("    				            <property name=\"ShowMissingSummaries\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"ShowMissingRemarks\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"ShowMissingParams\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"ShowMissingReturns\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"ShowMissingValues\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"DocumentInternals\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"DocumentProtected\" value=\"True\" />");
					sw.WriteLine("    				            <property name=\"DocumentPrivates\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"DocumentEmptyNamespaces\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"IncludeAssemblyVersion\" value=\"False\" />");
					sw.WriteLine("    				            <property name=\"CopyrightText\" value=\"\" />");
					sw.WriteLine("    				            <property name=\"CopyrightHref\" value=\"\" />");
					sw.WriteLine("    				        </documenter>");
					sw.WriteLine("    				    </documenters>");*/
					sw.WriteLine("    				</project>");
					sw.Close();
				}
				
				string command = Application.StartupPath + Path.DirectorySeparatorChar + "ndoc" + Path.DirectorySeparatorChar + "NDocGui.exe";
				string args    = '"' + projectFile + '"';
				
				ProcessStartInfo psi = new ProcessStartInfo(command, args);
				psi.WorkingDirectory = Application.StartupPath;
				psi.UseShellExecute = false;
				Process p = new Process();
				p.StartInfo = psi;
				p.Start();
			}
		}
	}
}
