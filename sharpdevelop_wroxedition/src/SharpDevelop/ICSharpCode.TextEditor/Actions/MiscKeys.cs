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
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor.Actions
{
	public class Tab : AbstractEditAction
	{
		void InsertTabs(IDocumentAggregator document, ISelection selection, int y1, int y2)
		{
			int  redocounter = 0;
			for (int i = y2; i >= y1; --i) {
				LineSegment line = document.GetLineSegment(i);
				if (i == y2 && line.Offset == selection.Offset + selection.Length) {
					continue;
				}
				
				/// this bit is optional - but useful if you are using block tabbing to sort out
				/// a source file with a mixture of tabs and spaces
				string newLine = TextUtilities.LeadingWhiteSpaceToTabs(document.GetText(line.Offset,line.Length),document.Properties.GetProperty("TabIndent", 4));
				document.Replace(line.Offset,line.Length,newLine);
				++redocounter;
		
				document.Insert(line.Offset, "\t");
				++redocounter;
			}
			
			if (redocounter > 0) {
				document.UndoStack.UndoLast(redocounter); // redo the whole operation (not the single deletes)
			}
		}
		
		public override void Execute(IEditActionHandler services)
		{
			if (services.Document.ReadOnly) {
				return;
			}
			if (services.HasSomethingSelected) {
				foreach (ISelection selection in services.Document.SelectionCollection) {
					int startLine = selection.StartLine;
					int endLine   = selection.EndLine;
					services.BeginUpdate();
					InsertTabs(services.Document, selection, startLine, endLine);
					services.EndUpdate();
					services.UpdateLines(startLine, endLine);
				}
				services.AutoClearSelection = false;
			} else {
				switch (services.Document.Caret.CaretMode) {
					case CaretMode.InsertMode:
						services.InsertChar('\t');
						break;
					case CaretMode.OverwriteMode:
						services.ReplaceChar('\t');
						break;
				}
				services.Document.SetDesiredColumn();
			}
		}
	}
	
	public class ShiftTab : AbstractEditAction
	{
		void RemoveTabs(IDocumentAggregator document, ISelection selection, int y1, int y2) 
		{
			int  redocounter = 0;
			for (int i = y2; i >= y1; --i) {
				LineSegment line = document.GetLineSegment(i);
				if (i == y2 && line.Offset == selection.Offset + selection.Length) {
					continue;
				}
				if (line.Length > 0) {
					/**** TextPad Strategy:
					/// first convert leading whitespace to tabs (controversial! - not all editors work like this)
					string newLine = TextUtilities.LeadingWhiteSpaceToTabs(document.GetText(line.Offset,line.Length),document.Properties.GetProperty("TabIndent", 4));
					if(newLine.Length > 0 && newLine[0] == '\t') {
						document.Replace(line.Offset,line.Length,newLine.Substring(1));
						++redocounter;
					}
					else if(newLine.Length > 0 && newLine[0] == ' ') { 
						/// there were just some leading spaces but less than TabIndent of them
						int leadingSpaces = 1;
						for(leadingSpaces = 1; leadingSpaces < newLine.Length && newLine[leadingSpaces] == ' '; leadingSpaces++) {
							/// deliberately empty
						}
						document.Replace(line.Offset,line.Length,newLine.Substring(leadingSpaces));
						++redocounter;
					}
					/// else
					/// there were no leading tabs or spaces on this line so do nothing
					****/
					/// MS Visual Studio 6 strategy:
					string temp = document.GetText(line.Offset,line.Length);
					if(temp.Length > 0) {
						int charactersToRemove = 0;
						if(temp[0] == '\t') { // first character is a tab - just remove it
							charactersToRemove = 1;
						}
						else if(temp[0] == ' ') {
							int leadingSpaces = 1;
							int tabIndent = document.Properties.GetProperty("TabIndent", 4);
							for(leadingSpaces = 1; leadingSpaces < temp.Length && temp[leadingSpaces] == ' '; leadingSpaces++) {
								/// deliberately empty
							}
							if(leadingSpaces >= tabIndent) {
								/// just remove tabIndent
								charactersToRemove = tabIndent;
							}
							else if(temp.Length > leadingSpaces && temp[leadingSpaces] == '\t') {
								/// remove the leading spaces and the following tab as they add up
								/// to just one tab stop
								charactersToRemove = leadingSpaces+1;
							}
							else {
								/// just remove the leading spaces
								charactersToRemove = leadingSpaces;
							}
						}
						if(charactersToRemove > 0) {
							document.Remove(line.Offset,charactersToRemove);
							++redocounter;
						}
					}
				}
			}
			
			if (redocounter > 0) {
				document.UndoStack.UndoLast(redocounter); // redo the whole operation (not the single deletes)
			}
		}
		public override void Execute(IEditActionHandler services)
		{
			if (services.HasSomethingSelected) {
				foreach (ISelection selection in services.Document.SelectionCollection) {
					int startLine = selection.StartLine;
					int endLine   = selection.EndLine;
					services.BeginUpdate();
					RemoveTabs(services.Document, selection, startLine, endLine);
					services.EndUpdate();
					services.UpdateLines(startLine, endLine);
				}
				services.AutoClearSelection = false;
			} else {
				/// Pressing Shift-Tab with nothing selected the cursor will move back to the 
				/// previous tab stop. It will stop at the beginning of the line. Also, the desired
				/// column is updated to that column.
				LineSegment line = services.Document.GetLineSegmentForOffset(services.Document.Caret.Offset);
				string startOfLine = services.Document.GetText(line.Offset,services.Document.Caret.Offset - line.Offset);
				int tabIndent = services.Document.Properties.GetProperty("TabIndent", 4);
				int currentColumn = services.Document.GetViewXPos(line, services.Document.Caret.Offset - line.Offset);
				int remainder = currentColumn % tabIndent;
				if (remainder == 0) {
					services.Document.Caret.DesiredColumn = Math.Max(0, currentColumn - tabIndent);
				} else {
					services.Document.Caret.DesiredColumn = Math.Max(0, currentColumn - remainder);
				}

				services.Document.SetCaretToDesiredColumn(line);
			}
		}		
	}
	
	public class CommentAction : AbstractEditAction
	{
		int firstLine;
		int lastLine;
		
		void SetCommentAt(IDocumentAggregator document, string comment, ISelection selection, int y1, int y2)
		{
			int  redocounter = 0;
			firstLine = y1;
			lastLine  = y2;
			
			for (int i = y2; i >= y1; --i) {
				LineSegment line = document.GetLineSegment(i);
				if (selection != null && i == y2 && line.Offset == selection.Offset + selection.Length) {
					--lastLine;
					continue;
				}
				
				string lineText = document.GetText(line.Offset, line.Length);
				document.Insert(line.Offset, comment);
				++redocounter;
			}
			
			if (redocounter > 0) {
				document.UndoStack.UndoLast(redocounter); // redo the whole operation (not the single deletes)
			}
		}
		
		public override void Execute(IEditActionHandler services)
		{
			if (services.Document.ReadOnly) {
				return;
			}
			string comment = "//";
			
			// HACK : VBNET commenting, better solution : the highlighting strategy should 
			//        have a IProperties which is filled with values from a property node in
			//        the highlighting definiton file and a property "SINGELINECOMMENT" should
			//        be defined.
			//  Currently I'm having something other to do, but it is listed. Mike
			if (services.Document.HighlightingStrategy.Name == "VBNET") {
				comment = "'";
			}
			
			if (services.HasSomethingSelected) {
				foreach (ISelection selection in services.Document.SelectionCollection) {
					services.BeginUpdate();
					SetCommentAt(services.Document, comment, selection, selection.StartLine, selection.EndLine);
					services.EndUpdate();
					services.UpdateLines(firstLine, lastLine);
				}
				services.AutoClearSelection = false;
			} else {
				int caretLine = services.Document.GetLineNumberForOffset(services.Document.Caret.Offset);
				SetCommentAt(services.Document, comment, null, caretLine, caretLine);
			}
		}
	}
	
	public class UncommentAction : AbstractEditAction
	{
		int firstLine;
		int lastLine;
		
		void RemoveCommentAt(IDocumentAggregator document, string comment, ISelection selection, int y1, int y2)
		{
			int  redocounter = 0;
			firstLine = y1;
			lastLine  = y2;
			
			for (int i = y2; i >= y1; --i) {
				LineSegment line = document.GetLineSegment(i);
				if (selection != null && i == y2 && line.Offset == selection.Offset + selection.Length) {
					--lastLine;
					continue;
				}
				
				string lineText = document.GetText(line.Offset, line.Length);
				if (lineText.Trim().StartsWith(comment)) {
					document.Remove(line.Offset + lineText.IndexOf(comment), comment.Length);
					++redocounter;
				} 
			}
			
			if (redocounter > 0) {
				document.UndoStack.UndoLast(redocounter); // redo the whole operation (not the single deletes)
			}
		}
		
		public override void Execute(IEditActionHandler services)
		{
			if (services.Document.ReadOnly) {
				return;
			}
			string comment = "//";
			
			// HACK : VBNET commenting, better solution : the highlighting strategy should 
			//        have a IProperties which is filled with values from a property node in
			//        the highlighting definiton file and a property "SINGELINECOMMENT" should
			//        be defined.
			//  Currently I'm having something other to do, but it is listed. Mike
			if (services.Document.HighlightingStrategy.Name == "VBNET") {
				comment = "'";
			}
			
			if (services.HasSomethingSelected) {
				foreach (ISelection selection in services.Document.SelectionCollection) {
					services.BeginUpdate();
					RemoveCommentAt(services.Document, comment, selection, selection.StartLine, selection.EndLine);
					services.EndUpdate();
					services.UpdateLines(firstLine, lastLine);
				}
				services.AutoClearSelection = false;
			} else {
				int caretLine = services.Document.GetLineNumberForOffset(services.Document.Caret.Offset);
				RemoveCommentAt(services.Document, comment, null, caretLine, caretLine);
			}
		}
	}
	
	/*
	public class ToggleComment : AbstractEditAction
	{
		int firstLine;
		int lastLine;
		
		void ToggleCommentAt(IDocumentAggregator document, string comment, ISelection selection, int y1, int y2)
		{
			int  redocounter = 0;
			int  addCounter    = 0;
			int  removeCounter = 0;
			firstLine = y1;
			lastLine  = y2;
			
			for (int i = y2; i >= y1; --i) {
				LineSegment line = document.GetLineSegment(i);
				if (selection != null && i == y2 && line.Offset == selection.Offset + selection.Length) {
					--lastLine;
					continue;
				}
				
				string lineText = document.GetText(line.Offset, line.Length);
				if (lineText.Trim().StartsWith(comment)) {
					document.Remove(line.Offset + lineText.IndexOf(comment), comment.Length);
					++removeCounter;
				} else {
					document.Insert(line.Offset, comment);
					++addCounter;
				}
				++redocounter;
			}
			
			if (redocounter > 0) {
				document.UndoStack.UndoLast(redocounter); // redo the whole operation (not the single deletes)
			}
		}
		
		public override void Execute(IEditActionHandler services)
		{
			if (services.Document.ReadOnly) {
				return;
			}
			string comment = "//";
			
			// HACK : VBNET commenting, better solution : the highlighting strategy should 
			//        have a IProperties which is filled with values from a property node in
			//        the highlighting definiton file and a property "SINGELINECOMMENT" should
			//        be defined.
			//  Currently I'm having something other to do, but it is listed. Mike
			if (services.Document.HighlightingStrategy.Name == "VBNET") {
				comment = "'";
			}
			
			if (services.HasSomethingSelected) {
				foreach (ISelection selection in services.Document.SelectionCollection) {
					services.BeginUpdate();
					ToggleCommentAt(services.Document, comment, selection, selection.StartLine, selection.EndLine);
					services.EndUpdate();
					services.UpdateLines(firstLine, lastLine);
				}
				services.AutoClearSelection = false;
			} else {
				int caretLine = services.Document.GetLineNumberForOffset(services.Document.Caret.Offset);
				ToggleCommentAt(services.Document, comment, null, caretLine, caretLine);
			}
		}
	}
	*/
	
	public class IndentSelection : AbstractEditAction
	{
		public override void Execute(IEditActionHandler services)
		{
			if (services.Document.ReadOnly) {
				return;
			}
			TextAreaControl textarea = (TextAreaControl)services;
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
	
	public class Backspace : AbstractEditAction
	{
		public override void Execute(IEditActionHandler services)
		{
			if (services.Document.ReadOnly) {
				return;
			}
			if (services.HasSomethingSelected) {
				services.RemoveSelectedText();
				services.ScrollToCaret();
			} else {
				if (services.Document.Caret.Offset > 0) {
					services.BeginUpdate();
					int curLineNr     = services.Document.GetLineNumberForOffset(services.Document.Caret.Offset);
					int curLineOffset = services.Document.GetLineSegment(curLineNr).Offset;
					
					if (curLineOffset == services.Document.Caret.Offset) {
						LineSegment line = services.Document.GetLineSegment(curLineNr - 1);
						
						bool lastLine = curLineNr == services.Document.TotalNumberOfLines;
						int lineEndOffset = line.Offset + line.Length;
						services.Document.Remove(lineEndOffset, curLineOffset - lineEndOffset);
						services.Document.Caret.Offset = lineEndOffset;					
						services.EndUpdate();
						services.UpdateToEnd(curLineNr - 1);
					} else {
						--services.Document.Caret.Offset;
						services.Document.Remove(services.Document.Caret.Offset, 1);
						services.EndUpdate();
						services.UpdateLineToEnd(curLineNr, services.Document.Caret.Offset - services.Document.GetLineSegment(curLineNr).Offset);
					}
				}
			}
		}
	}
	
	public class Delete : AbstractEditAction
	{
		public override void Execute(IEditActionHandler services)
		{
			if (services.Document.ReadOnly) {
				return;
			}
			if (services.HasSomethingSelected) {
				services.RemoveSelectedText();
				services.ScrollToCaret();
			} else {
			
				if (services.Document.Caret.Offset < services.Document.TextLength) {
					services.BeginUpdate();
					int curLineNr   = services.Document.GetLineNumberForOffset(services.Document.Caret.Offset);
					LineSegment curLine = services.Document.GetLineSegment(curLineNr);
					
					if (curLine.Offset + curLine.Length == services.Document.Caret.Offset) {
						if (curLineNr + 1 < services.Document.TotalNumberOfLines) {
							LineSegment nextLine = services.Document.GetLineSegment(curLineNr + 1);
							
							services.Document.Remove(services.Document.Caret.Offset, nextLine.Offset - services.Document.Caret.Offset);
							services.EndUpdate();
							services.UpdateToEnd(curLineNr);
						}
					} else {
						services.Document.Remove(services.Document.Caret.Offset, 1);
						services.EndUpdate();
						services.UpdateLineToEnd(curLineNr, services.Document.Caret.Offset - services.Document.GetLineSegment(curLineNr).Offset);
					}
				}
			}
		}
	}
	
	public class MovePageDown : AbstractEditAction
	{
		public override void Execute(IEditActionHandler services)
		{
			int curLineNr     = services.Document.GetLineNumberForOffset(services.Document.Caret.Offset);
			int requestedLineNumber = Math.Min(services.Document.GetNextVisibleLineAbove(curLineNr, services.MaxVisibleLine), services.Document.TotalNumberOfLines - 1);
			
			if (curLineNr != requestedLineNumber) {
				LineSegment line = services.Document.GetLineSegment(requestedLineNumber);
				services.Document.Caret.Offset = line.Offset + Math.Min(line.Length, services.Document.Caret.DesiredColumn);
			}
		}
	}
	
	public class MovePageUp : AbstractEditAction
	{
		public override void Execute(IEditActionHandler services)
		{
			int curLineNr     = services.Document.GetLineNumberForOffset(services.Document.Caret.Offset);
			int requestedLineNumber = Math.Max(services.Document.GetNextVisibleLineBelow(curLineNr, services.MaxVisibleLine), 0);
			
			if (curLineNr != requestedLineNumber) {
				LineSegment line = services.Document.GetLineSegment(requestedLineNumber);
				services.Document.Caret.Offset = line.Offset + Math.Min(line.Length, services.Document.Caret.DesiredColumn);
			}
		}
	}
	
	public class Return : AbstractEditAction
	{	
		public override void Execute(IEditActionHandler services)
		{
			if (services.Document.ReadOnly) {
				return;
			}
			services.BeginUpdate();
			services.InsertChar('\n');
			
			int curLineNr = services.Document.GetLineNumberForOffset(services.Document.Caret.Offset);
			services.Document.Caret.Offset += services.Document.FormattingStrategy.FormatLine(curLineNr, services.Document.Caret.Offset, '\n');
			services.Document.SetDesiredColumn();
			
			services.EndUpdate();
			services.UpdateToEnd(curLineNr - 1);
		}
	}
	
	public class ToggleEditMode : AbstractEditAction
	{
		public override void Execute(IEditActionHandler services)
		{
			if (services.Document.ReadOnly) {
				return;
			}
			switch (services.Document.Caret.CaretMode) {
				case CaretMode.InsertMode:
					services.Document.Caret.CaretMode = CaretMode.OverwriteMode;
					break;
				case CaretMode.OverwriteMode:
					services.Document.Caret.CaretMode = CaretMode.InsertMode;
					break;
			} 
		}
	}
	
	public class Undo : AbstractEditAction
	{
		public override void Execute(IEditActionHandler services)
		{
			if (services.Document.ReadOnly) {
				return;
			}			
			if (services.Document.UndoStack.CanUndo) {
				services.BeginUpdate();
				services.Document.UndoStack.Undo();
				
				services.EndUpdate();
				services.Refresh();
			}
		}
	}
	
	public class Redo : AbstractEditAction
	{
		public override void Execute(IEditActionHandler services)
		{
			if (services.Document.ReadOnly) {
				return;
			}
			if (services.Document.UndoStack.CanRedo) {
				services.BeginUpdate();
				services.Document.UndoStack.Redo();
				
				services.EndUpdate();
				services.Refresh();
			}
		}
	}
	
	/// <summary>
	/// handles the ctrl-backspace key
	/// functionality attempts to roughly mimic MS Developer studio
	/// I will implement this as deleting back to the point that ctrl-leftarrow would
	/// take you to
	/// </summary>
	public class WordBackspace : AbstractEditAction 
	{	
		public override void Execute(IEditActionHandler services)
		{
			// if anything is selected we will just delete it first
			services.BeginUpdate();
			if (services.HasSomethingSelected) {
				services.RemoveSelectedText();
				services.ScrollToCaret();
			}
			// now delete from the caret to the beginning of the word
			LineSegment line =
			services.Document.GetLineSegmentForOffset(services.Document.Caret.Offset);
			// if we are not at the beginning of a line
			if(services.Document.Caret.Offset > line.Offset) {
				int prevWordStart = TextUtilities.FindPrevWordStart(services.Document,
				                                                    services.Document.Caret.Offset);
				if(prevWordStart < services.Document.Caret.Offset) {
					services.Document.Remove(prevWordStart,services.Document.Caret.Offset -
					                         prevWordStart);
					services.Document.Caret.Offset = prevWordStart;
				}
			}
			// if we are now at the beginning of a line
			if(services.Document.Caret.Offset == line.Offset) {
				// if we are not on the first line
				int curLineNr =
				services.Document.GetLineNumberForOffset(services.Document.Caret.Offset);
				if(curLineNr > 0) {
					// move to the end of the line above
					LineSegment lineAbove = services.Document.GetLineSegment(curLineNr -
					                                                         1);
					int endOfLineAbove = lineAbove.Offset + lineAbove.Length;
					int charsToDelete = services.Document.Caret.Offset - endOfLineAbove;
					services.Document.Remove(endOfLineAbove,charsToDelete);
					services.Document.Caret.Offset = endOfLineAbove;
				}
			}
			services.Document.SetDesiredColumn();
			services.EndUpdate();
			// if there are now less lines, we need this or there are redraw problems
			
			services.UpdateToEnd(services.Document.GetLineNumberForOffset(services.Document.Caret.Offset));
		}
	}
	
	/// <summary>
	/// handles the ctrl-delete key
	/// functionality attempts to mimic MS Developer studio
	/// I will implement this as deleting forwardto the point that 
	/// ctrl-leftarrow would take you to
	/// </summary>
	public class DeleteWord : Delete 
	{
		public override void Execute(IEditActionHandler services)
		{
			// if anything is selected we will just delete it first
			services.BeginUpdate();
			if (services.HasSomethingSelected) {
				services.RemoveSelectedText();
				services.ScrollToCaret();
			}
			// now delete from the caret to the beginning of the word
			LineSegment line =
			services.Document.GetLineSegmentForOffset(services.Document.Caret.Offset);
			if(services.Document.Caret.Offset == line.Offset + line.Length) {
				// if we are at the end of a line
				base.Execute(services);
			} else {
				int nextWordStart = TextUtilities.FindNextWordStart(services.Document,
				                                                    services.Document.Caret.Offset);
				if(nextWordStart > services.Document.Caret.Offset) {
					services.Document.Remove(services.Document.Caret.Offset,nextWordStart -
					                         services.Document.Caret.Offset);
					// cursor never moves with this command
				}
			}
			services.EndUpdate();
			// if there are now less lines, we need this or there are redraw problems
			services.UpdateToEnd(services.Document.GetLineNumberForOffset(services.Document.Caret.Offset));
		}
	}
}
