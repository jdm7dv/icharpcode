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

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels.CompletionDatabaseWizard
{
	public class UseExistingFilePanel : AbstractWizardPanel
	{
		IProperties properties;
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			switch (message) {
				case DialogMessage.Activated:
					SetFinishedState(this, EventArgs.Empty);
					break;
				case DialogMessage.Prev:
					EnableFinish = false;
					break;
			}
			return true;
		}
		
		void SetFinishedState(object sender, EventArgs e)
		{
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			string path = ControlDictionary["locationTextBox"].Text;
			EnableFinish = fileUtilityService.IsValidFileName(path) &&
			               Directory.Exists(path) && 
			               File.Exists(fileUtilityService.GetDirectoryNameWithSeparator(path) + "CodeCompletionProxyData.bin");
			if (EnableFinish) {
				properties.SetProperty("SharpDevelop.CodeCompletion.DataDirectory",
				                       path);
			}
		}
		
		void SetValues(object sender, EventArgs e)
		{
			properties = (IProperties)CustomizationObject;
		}
		
		void BrowseLocationEvent(object sender, EventArgs e)
		{
			FolderDialog fd = new FolderDialog();
			if (fd.DisplayDialog("choose the location in which you want the code completion files to be generated") == DialogResult.OK) {
				ControlDictionary["locationTextBox"].Text = fd.Path;
			}
		}
		
		public UseExistingFilePanel() : base(Application.StartupPath + @"\..\data\resources\panels\CompletionDatabaseWizard\UseExistingFilePanel.xfrm")
		{
			IsLastPanel       = true;
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			((TextBox)ControlDictionary["textBox"]).Lines = resourceService.GetString("Dialog.Wizards.CodeCompletionDatabaseWizard.UseExistingFilePanel.PanelDescription").Split('\n');
			
			ControlDictionary["locationTextBox"].TextChanged += new EventHandler(SetFinishedState);
			ControlDictionary["browseButton"].Click          += new EventHandler(BrowseLocationEvent);
			
			SetFinishedState(this, EventArgs.Empty);
			CustomizationObjectChanged += new EventHandler(SetValues);
			
		}
	}
}
