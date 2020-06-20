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
using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.Core.Properties;

using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class RemoveLeadingWS : AbstractMenuCommand
	{
		void Convert(IDocumentAggregator document, int y1, int y2) 
		{
			int  redocounter = 0; // must count how many Delete operations occur
			for (int i = y1; i < y2; ++i) {
				LineSegment line = document.GetLineSegment(i);
				int removeNumber = 0;
				for (int x = line.Offset; x < line.Offset + line.Length && Char.IsWhiteSpace(document.GetCharAt(x)); ++x) {
					++removeNumber;
				}
				if (removeNumber > 0) {
					document.Remove(line.Offset, removeNumber);
					++redocounter; // count deletes
				}
			}
			if (redocounter > 0) {
				document.UndoStack.UndoLast(redocounter); // redo the whole operation (not the single deletes)
			}
		}
		
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextAreaControlProvider)) {
				return;
			}
			
			TextAreaControl textarea = ((ITextAreaControlProvider)window.ViewContent).TextAreaControl;
			textarea.BeginUpdate();
			if (textarea.HasSomethingSelected) {
				foreach (ISelection selection in textarea.Document.SelectionCollection) {
					Convert(textarea.Document, selection.StartLine, selection.EndLine);
				}
			} else {
				Convert(textarea.Document, 0, textarea.Document.TotalNumberOfLines);
			}
			textarea.CheckCaretPos();
			textarea.EndUpdate();
			textarea.Refresh();
		}
	}

	
	public class RemoveTrailingWS : AbstractMenuCommand
	{
		void Convert(IDocumentAggregator document, int y1, int y2) 
		{
			int  redocounter = 0; // must count how many Delete operations occur
			for (int i = y2 - 1; i >= y1; --i) {
				LineSegment line = document.GetLineSegment(i);
				int removeNumber = 0;
				for (int x = line.Offset + line.Length - 1; x >= line.Offset && Char.IsWhiteSpace(document.GetCharAt(x)); --x) {
					++removeNumber;
				}
				if (removeNumber > 0) {
					document.Remove(line.Offset + line.Length - removeNumber, removeNumber);
					++redocounter;         // count deletes
				}
			}
			if (redocounter > 0) {
				document.UndoStack.UndoLast(redocounter); // redo the whole operation (not the single deletes)
			}
		}
		
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextAreaControlProvider)) {
				return;
			}
			
			TextAreaControl textarea = ((ITextAreaControlProvider)window.ViewContent).TextAreaControl;
			textarea.BeginUpdate();
			if (textarea.HasSomethingSelected) {
				foreach (ISelection selection in textarea.Document.SelectionCollection) {
					Convert(textarea.Document, selection.StartLine, selection.EndLine);
				}
			} else {
				Convert(textarea.Document, 0, textarea.Document.TotalNumberOfLines);
			}
			textarea.CheckCaretPos();
			textarea.EndUpdate();
			textarea.Refresh();
		}
	}
	
	
	public class ToUpperCase : AbstractMenuCommand
	{
		void Convert(IDocumentAggregator document, int startOffset, int length)
		{
			string what = document.GetText(startOffset, length).ToUpper();
			document.Replace(startOffset, length, what);
		}
		
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextAreaControlProvider)) {
				return;
			}
			
			TextAreaControl textarea = ((ITextAreaControlProvider)window.ViewContent).TextAreaControl;
			textarea.BeginUpdate();
			if (textarea.HasSomethingSelected) {
				foreach (ISelection selection in textarea.Document.SelectionCollection) {
					Convert(textarea.Document, selection.Offset, selection.Length);
				}
			} else {
				Convert(textarea.Document, 0, textarea.Document.TextLength);
			}
			textarea.EndUpdate();
			textarea.Refresh();
		}
	}
	
	public class ToLowerCase : AbstractMenuCommand
	{
		void Convert(IDocumentAggregator document, int startOffset, int length)
		{
			string what = document.GetText(startOffset, length).ToLower();
			document.Replace(startOffset, length, what);
		}
		
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextAreaControlProvider)) {
				return;
			}
			
			TextAreaControl textarea = ((ITextAreaControlProvider)window.ViewContent).TextAreaControl;
			textarea.BeginUpdate();
			if (textarea.HasSomethingSelected) {
				foreach (ISelection selection in textarea.Document.SelectionCollection) {
					Convert(textarea.Document, selection.Offset, selection.Length);
				}
			} else {
				Convert(textarea.Document, 0, textarea.Document.TextLength);
			}
			textarea.EndUpdate();
			textarea.Refresh();
		}
	}
	
	public class InvertCaseAction : AbstractMenuCommand
	{
		void Convert(IDocumentAggregator document, int startOffset, int length)
		{
			StringBuilder what = new StringBuilder(document.GetText(startOffset, length));
			
			for (int i = 0; i < what.Length; ++i) {
				what[i] = Char.IsUpper(what[i]) ? Char.ToLower(what[i]) : Char.ToUpper(what[i]);
			}
			
			document.Replace(startOffset, length, what.ToString());
		}
		
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextAreaControlProvider)) {
				return;
			}
			
			TextAreaControl textarea = ((ITextAreaControlProvider)window.ViewContent).TextAreaControl;
			textarea.BeginUpdate();
			if (textarea.HasSomethingSelected) {
				foreach (ISelection selection in textarea.Document.SelectionCollection) {
					Convert(textarea.Document, selection.Offset, selection.Length);
				}
			} else {
				Convert(textarea.Document, 0, textarea.Document.TextLength);
			}
			textarea.EndUpdate();
			textarea.Refresh();
		}
	}
	
	public class CapitalizeAction : AbstractMenuCommand
	{
		void Convert(IDocumentAggregator document, int startOffset, int length)
		{
			StringBuilder what = new StringBuilder(document.GetText(startOffset, length));
			
			for (int i = 0; i < what.Length; ++i) {
				if (!Char.IsLetter(what[i]) && i < what.Length - 1) {
					what[i + 1] = Char.ToUpper(what[i + 1]);
				}
			}
			
			document.Replace(startOffset, length, what.ToString());
		}
		
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextAreaControlProvider)) {
				return;
			}
			
			TextAreaControl textarea = ((ITextAreaControlProvider)window.ViewContent).TextAreaControl;
			textarea.BeginUpdate();
			if (textarea.HasSomethingSelected) {
				foreach (ISelection selection in textarea.Document.SelectionCollection) {
					Convert(textarea.Document, selection.Offset, selection.Length);
				}
			} else {
				Convert(textarea.Document, 0, textarea.Document.TextLength);
			}
			textarea.EndUpdate();
			textarea.Refresh();
		}
		
	}
	
	public class ConvertTabsToSpaces : AbstractMenuCommand
	{
		void Convert(IDocumentAggregator document, int startOffset, int length)
		{
			string what = document.GetText(startOffset, length);
			string spaces = new string(' ',document.Properties.GetProperty("TabIndent", 4));
			document.Replace(startOffset, length, what.Replace("\t", spaces));
		}
		
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextAreaControlProvider)) {
				return;
			}
			
			TextAreaControl textarea = ((ITextAreaControlProvider)window.ViewContent).TextAreaControl;
			textarea.BeginUpdate();
			if (textarea.HasSomethingSelected) {
				foreach (ISelection selection in textarea.Document.SelectionCollection) {
					Convert(textarea.Document, selection.Offset, selection.Length);
				}
			} else {
				Convert(textarea.Document, 0, textarea.Document.TextLength);
			}
			textarea.CheckCaretPos();
			textarea.EndUpdate();
			textarea.Refresh();
		}
	}
	
	public class ConvertSpacesToTabs : AbstractMenuCommand
	{
		void Convert(IDocumentAggregator document, int startOffset, int length)
		{
			string what = document.GetText(startOffset, length);
			string spaces = new string(' ',document.Properties.GetProperty("TabIndent", 4));
			document.Replace(startOffset, length, what.Replace(spaces, "\t"));
		}
		
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextAreaControlProvider)) {
				return;
			}
			
			TextAreaControl textarea = ((ITextAreaControlProvider)window.ViewContent).TextAreaControl;
			textarea.BeginUpdate();
			if (textarea.HasSomethingSelected) {
				foreach (ISelection selection in textarea.Document.SelectionCollection) {
					Convert(textarea.Document, selection.Offset, selection.Length);
				}
			} else {
				Convert(textarea.Document, 0, textarea.Document.TextLength);
			}
			textarea.CheckCaretPos();
			textarea.EndUpdate();
			textarea.Refresh();
		}
	}
	
	public class ConvertLeadingTabsToSpaces : AbstractMenuCommand
	{
		void Convert(IDocumentAggregator document, ISelection selection,int y1, int y2) 
		{
			int  redocounter = 0;
			for (int i = y2; i >= y1; --i) {
				LineSegment line = document.GetLineSegment(i);
				if(selection != null) {
					/// this is for the case where a selection ends just before the first on line y2
					if (i == y2 && line.Offset == selection.Offset + selection.Length) {
						continue;
					}
				}
				
				if(line.Length > 0) {
					// count how many whitespace characters there are at the start
					int whiteSpace = 0;
					for(whiteSpace = 0; whiteSpace < line.Length && Char.IsWhiteSpace(document.GetCharAt(line.Offset + whiteSpace)); whiteSpace++) {
						// deliberately empty
					}
					if(whiteSpace > 0) {
						string newLine = document.GetText(line.Offset,whiteSpace);
						string newPrefix = newLine.Replace("\t",new string(' ',document.Properties.GetProperty("TabIndent", 4)));
						document.Replace(line.Offset,whiteSpace,newPrefix);
						++redocounter;
					}
				}
			}
			
			if (redocounter > 0) {
				document.UndoStack.UndoLast(redocounter); // redo the whole operation (not the single deletes)
			}
		}
		
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextAreaControlProvider)) {
				return;
			}
			
			TextAreaControl textarea = ((ITextAreaControlProvider)window.ViewContent).TextAreaControl;
			textarea.BeginUpdate();
			if (textarea.HasSomethingSelected) {
				foreach (ISelection selection in textarea.Document.SelectionCollection) {
					Convert(textarea.Document, selection, selection.StartLine, selection.EndLine);
				}
			} else {
				Convert(textarea.Document, null, 0, textarea.Document.TotalNumberOfLines - 1);
			}
			textarea.CheckCaretPos();
			textarea.EndUpdate();
			textarea.Refresh();
		}
	}
	
	public class ConvertLeadingSpacesToTabs : AbstractMenuCommand
	{
		void Convert(IDocumentAggregator document, ISelection selection, int y1, int y2) 
		{
			int  redocounter = 0;
			for (int i = y2; i >= y1; --i) {
				LineSegment line = document.GetLineSegment(i);
				if(selection != null) {
					/// this is for the case where a selection ends just before the first on line y2
					if (i == y2 && line.Offset == selection.Offset + selection.Length) {
						continue;
					}
				}
				if(line.Length > 0) {
					/// note: some users may prefer a more radical ConvertLeadingSpacesToTabs that
					/// means there can be no spaces before the first character even if the spaces
					/// didn't add up to a whole number of tabs
					string newLine = TextUtilities.LeadingWhiteSpaceToTabs(document.GetText(line.Offset,line.Length),document.Properties.GetProperty("TabIndent", 4));
					document.Replace(line.Offset,line.Length,newLine);
					++redocounter;
				}
			}
			
			if (redocounter > 0) {
				document.UndoStack.UndoLast(redocounter); // redo the whole operation (not the single deletes)
			}
		}
		
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextAreaControlProvider)) {
				return;
			}
			
			TextAreaControl textarea = ((ITextAreaControlProvider)window.ViewContent).TextAreaControl;
			textarea.BeginUpdate();
			if (textarea.HasSomethingSelected) {
				foreach (ISelection selection in textarea.Document.SelectionCollection) {
					Convert(textarea.Document, selection, selection.StartLine, selection.EndLine);
				}
			} else { 
				Convert(textarea.Document, null, 0, textarea.Document.TotalNumberOfLines - 1);
			}
			textarea.CheckCaretPos();
			textarea.EndUpdate();
			textarea.Refresh();
		}
	}
	
	/// <summary>
	/// This is a sample editaction plugin, it indents the selected area.
	/// </summary>
	public class IndentSelection : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextAreaControlProvider)) {
				return;
			}
			
			TextAreaControl textarea = ((ITextAreaControlProvider)window.ViewContent).TextAreaControl;
			textarea.BeginUpdate();
			if (textarea.HasSomethingSelected) {
				foreach (ISelection selection in textarea.Document.SelectionCollection) {
					textarea.Document.FormattingStrategy.IndentLines(selection.StartLine, selection.EndLine);
				}
			} else {
				textarea.Document.FormattingStrategy.IndentLines(0, textarea.Document.TotalNumberOfLines - 1);
			}
			textarea.CheckCaretPos();
			textarea.EndUpdate();
			textarea.Refresh();
		}
	}
	
}
