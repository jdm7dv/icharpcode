// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;

using ICSharpCode.Core.AddIns;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class ShowColorDialog : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextAreaControlProvider)) {
				return;
			}
			TextAreaControl textarea = ((ITextAreaControlProvider)window.ViewContent).TextAreaControl;
			
			ColorDialog cd = new ColorDialog();
			if (cd.ShowDialog() == DialogResult.OK) {
				string colorstr = "#" + cd.Color.ToArgb().ToString("X");
				if (cd.Color.IsKnownColor) {
					colorstr = cd.Color.ToKnownColor().ToString();
				}
				
				textarea.Document.Insert(textarea.Document.Caret.Offset, colorstr);
				int lineNumber = textarea.Document.GetLineNumberForOffset(textarea.Document.Caret.Offset);
				textarea.Document.Caret.Offset += colorstr.Length;
				textarea.UpdateLines(lineNumber, lineNumber);
			}
			cd.Dispose();
			
		}
	}
	
}
