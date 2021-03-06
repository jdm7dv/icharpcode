// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System.Drawing;
using System.Windows.Forms;
using System;

using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Actions
{
	public class TemplateCompletion : AbstractEditAction
	{
		public override void Execute(IEditActionHandler services)
		{
			CompletionWindow completionWindow = new CompletionWindow((TextAreaControl)services, ((TextAreaControl)services).FileName, new TemplateCompletionDataProvider());
			completionWindow.ShowCompletionWindow('\0');
		}
	}	
}
