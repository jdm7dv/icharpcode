// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.CodeDom.Compiler;
using System.Windows.Forms;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class StartWorkbenchCommand : AbstractCommand
	{
		const string workbenchMemento = "SharpDevelop.Workbench.WorkbenchMemento";
		
		EventHandler idleEventHandler;
		bool isCalled = false;
		
		/// <remarks>
		/// The worst workaround in the whole project
		/// </remarks>
		void ShowTipOfTheDay(object sender, EventArgs e)
		{
			if (isCalled) {
				Application.Idle -= idleEventHandler;
				return;
			}
			isCalled = true;
			// show tip of the day
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			if (propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.Dialog.TipOfTheDayView.ShowTipsAtStartup", true)) {
				ViewTipOfTheDay dview = new ViewTipOfTheDay();
				dview.Run();
			}			
		}
		
		public override void Run()
		{
			Form f = (Form)WorkbenchSingleton.Workbench;
			f.Show();
			idleEventHandler = new EventHandler(ShowTipOfTheDay);
			Application.Idle += idleEventHandler;
			
			if (SharpDevelopMain.CommandLineArgs != null) {
				foreach (string file in SharpDevelopMain.CommandLineArgs) {
					switch (System.IO.Path.GetExtension(file).ToUpper()) {
						case ".CMBX":
						case ".PRJX":
							try {
								IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
								projectService.OpenCombine(file);
							} catch (Exception e) {
								Console.WriteLine("unable to open project/combine {0} exception was :\n{1}", file, e.ToString());
							}
							break;
						default:
							try {
								IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
								fileService.OpenFile(file);
							} catch (Exception e) {
								Console.WriteLine("unable to open file {0} exception was :\n{1}", file, e.ToString());
							}
							break;
					}
				}
			}
			
			f.Focus(); // windows.forms focus workaround	
			
			// start parser thread because I don't know a better location
			DefaultParserService parserService = (DefaultParserService)ServiceManager.Services.GetService(typeof(DefaultParserService));
			parserService.StartParserThread();
			
			// finally run the workbench window ...
			Application.Run(f);
			
			// save the workbench memento in the ide properties
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			propertyService.SetProperty(workbenchMemento, WorkbenchSingleton.Workbench.CreateMemento());
		}
	}
}
