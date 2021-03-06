// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Drawing;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.SharpDevelop.Gui;

using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Properties;

namespace Plugins.Wizards.MessageBoxBuilder.Command {
	
	public class WizardCommand : AbstractMenuCommand
	{
		const string WizardPath = "Plugins/Wizards/MessageBoxBuilderWizard";
		
		public override void Run()
		{
			IProperties customizer = new DefaultProperties();
			Plugins.Wizards.MessageBoxBuilder.Generator.MessageBoxGenerator generator = new Plugins.Wizards.MessageBoxBuilder.Generator.MessageBoxGenerator();
			customizer.SetProperty("Generator", generator);
			
			using (WizardDialog wizard = new WizardDialog("MessageBox Wizard", customizer, WizardPath)) {
				if (wizard.ShowDialog() == DialogResult.OK) {
					
					IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
					
					TextAreaControl textarea = (TextAreaControl)window.ViewContent.Control;
					
					string txtLength = generator.GenerateCode();
					textarea.Document.Insert(textarea.Document.Caret.Offset, txtLength);
					int lineNumber = textarea.Document.GetLineNumberForOffset(textarea.Document.Caret.Offset);
					textarea.Document.Caret.Offset += txtLength.Length;
					textarea.Refresh();
				}
			}
		}
	}
}
