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

using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Dialogs;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class ShowHelp : AbstractMenuCommand
	{
		public override void Run()
		{
			string fileName = Application.StartupPath + 
			              Path.DirectorySeparatorChar + ".." +
			              Path.DirectorySeparatorChar + "doc" +
			              Path.DirectorySeparatorChar + "help" +
			              Path.DirectorySeparatorChar + "sharpdevelop.chm";
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			if (fileUtilityService.TestFileExists(fileName)) {
				Help.ShowHelp((Form)WorkbenchSingleton.Workbench, fileName);
			}
		}
	}
	
	public class ViewGPL : AbstractMenuCommand
	{
		public override void Run()
		{
			using (ViewGPLDialog totdd = new ViewGPLDialog()) {
				totdd.Owner = (Form)WorkbenchSingleton.Workbench;
				totdd.ShowDialog();
			}
		}
	}
	
	public class GotoWebSite : AbstractMenuCommand
	{
		string site;
		
		public GotoWebSite(string site)
		{
			this.site = site;
		}
		
		public override void Run()
		{
			IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			fileService.OpenFile(site);
		}
	}
	
	public class GotoLink : AbstractMenuCommand
	{
		string site;
		
		public GotoLink(string site)
		{
			this.site = site;
		}
		
		public override void Run()
		{
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			string file = site.StartsWith("home://") ? fileUtilityService.GetDirectoryNameWithSeparator(Application.StartupPath) + site.Substring(7).Replace('/', Path.DirectorySeparatorChar) : site;
			try {
				Process.Start(file);
			} catch (Exception) {
				MessageBox.Show("Can't execute/view " + file + "\n Please check that the file exists and that you can open this file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
	
	public class ViewTipOfTheDay : AbstractMenuCommand
	{
		public override void Run()
		{
			using (TipOfTheDayDialog totdd = new TipOfTheDayDialog()) {
				totdd.Owner = (Form)WorkbenchSingleton.Workbench;
				totdd.ShowDialog();
			}
		}
	}
	
	public class AboutSharpDevelop : AbstractMenuCommand
	{
		public override void Run()
		{
			using (CommonAboutDialog ad = new CommonAboutDialog()) {
				ad.Owner = (Form)WorkbenchSingleton.Workbench;
				ad.ShowDialog();
			}
		}
	}
}
