// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Resources;

using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

using ICSharpCode.XmlForms;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	public class ViewGPLDialog : XmlForm 
	{
		public ViewGPLDialog() : base(Application.StartupPath + @"\..\data\resources\dialogs\ViewGPLDialog.xfrm")
		{
			LoadGPL();
		}
		
		protected override void SetupXmlLoader()
		{
			xmlLoader.StringValueFilter    = new SharpDevelopStringValueFilter();
			xmlLoader.PropertyValueCreator = new SharpDevelopPropertyValueCreator();
		}
		
		void LoadGPL()
		{
			string filename = Application.StartupPath + 
			                  Path.DirectorySeparatorChar + ".." +
			                  Path.DirectorySeparatorChar + "doc" +
			                  Path.DirectorySeparatorChar + "license.txt";
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			if (fileUtilityService.TestFileExists(filename)) {
				RichTextBox licenseRichTextBox = (RichTextBox)ControlDictionary["licenseRichTextBox"];
				licenseRichTextBox.LoadFile(filename, RichTextBoxStreamType.PlainText);
			}
		}
	}
}
