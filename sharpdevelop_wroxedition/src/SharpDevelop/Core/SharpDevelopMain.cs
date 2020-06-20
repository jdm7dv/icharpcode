// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Reflection;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Resources;
using System.Xml;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop
{
	public class SplashScreenForm : Form
	{
		public SplashScreenForm()
		{
#if !DEBUG
			TopMost         = true;
#endif
			FormBorderStyle = FormBorderStyle.None;
			StartPosition   = FormStartPosition.CenterScreen;
			ShowInTaskbar   = false;
			ResourceManager resources = new ResourceManager("IconResources", Assembly.GetCallingAssembly());
			Bitmap bitmap = (Bitmap)resources.GetObject("SplashScreen");
			Size = bitmap.Size;
			BackgroundImage = bitmap;
		}
	}
		
	/// <summary>
	/// This Class is the Core main class, it starts the program.
	/// </summary>
	public class SharpDevelopMain
	{
		static SplashScreenForm splashScreen = null;
		static string[] commandLineArgs;
		
		public static SplashScreenForm SplashScreen {
			get {
				return splashScreen;
			}
		}
		
		public static string[] CommandLineArgs {
			get {
				return commandLineArgs;
			}
		}
		
		/// <summary>
		/// Starts the core of SharpDevelop.
		/// </summary>
		[STAThread()]
		public static void Main(string[] args)
		{
			commandLineArgs = args;
			bool noLogo = false;
			
			foreach (string arg in args) {
				if (arg.ToUpper().EndsWith("NOLOGO")) {
					noLogo = true;
				}
			}
			
			if (!noLogo) {
				splashScreen = new SplashScreenForm();
				splashScreen.Show();
			}
			
			ArrayList commands = null;
			try {
				ServiceManager.Services.InitializeServicesSubsystem("/Workspace/Services");
			
				commands = AddInTreeSingleton.AddInTree.GetTreeNode("/Workspace/Autostart").BuildChildItems(null);
				
				for (int i = 0; i < commands.Count - 1; ++i) {
					((ICommand)commands[i]).Run();
				}
			} catch (XmlException e) {
				MessageBox.Show("Could not load XML :\n" + e.Message);
				return;
			} catch (Exception e) {
				MessageBox.Show("Loading error, please reinstall :\n" + e.ToString());
				return;
			} finally {
				if (splashScreen != null) {
					splashScreen.Close();
					splashScreen.Dispose();
				}
			}
			
			// run the last autostart command, this must be the workbench starting command
			if (commands.Count > 0) {
				((ICommand)commands[commands.Count - 1]).Run();
			}
			
			// unloading services
			ServiceManager.Services.UnloadAllServices();			
		}
	}
}
