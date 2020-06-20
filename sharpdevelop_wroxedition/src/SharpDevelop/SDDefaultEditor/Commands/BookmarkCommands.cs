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
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class ToggleBookmark : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextAreaControlProvider)) {
				return;
			}
			
			TextAreaControl textarea = ((ITextAreaControlProvider)window.ViewContent).TextAreaControl;
			textarea.Document.BookmarkManager.ToggleMarkAt(textarea.Document.GetLineNumberForOffset(textarea.Document.Caret.Offset));
		}
	}
	
	public class PrevBookmark : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextAreaControlProvider)) {
				return;
			}
			
			TextAreaControl textarea = ((ITextAreaControlProvider)window.ViewContent).TextAreaControl;
			int lineNumber = textarea.Document.BookmarkManager.GetPrevMark(textarea.Document.GetLineNumberForOffset(textarea.Document.Caret.Offset));
			if (lineNumber != -1) {
				textarea.Document.Caret.Offset = textarea.Document.GetLineSegment(lineNumber).Offset;
			}
		}
	}
		
	public class NextBookmark : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextAreaControlProvider)) {
				return;
			}
			
			TextAreaControl textarea = ((ITextAreaControlProvider)window.ViewContent).TextAreaControl;
			int lineNumber = textarea.Document.BookmarkManager.GetNextMark(textarea.Document.GetLineNumberForOffset(textarea.Document.Caret.Offset));
			if (lineNumber != -1 && lineNumber < textarea.Document.TotalNumberOfLines) {
				textarea.Document.Caret.Offset = textarea.Document.GetLineSegment(lineNumber).Offset;
			}
		}
	}
		
	public class ClearBookmarks : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextAreaControlProvider)) {
				return;
			}
			
			TextAreaControl textarea = ((ITextAreaControlProvider)window.ViewContent).TextAreaControl;
			textarea.Document.BookmarkManager.Clear();
		}
	}
}
