// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System.Drawing;

using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Internal.Undo;

namespace ICSharpCode.TextEditor.Document
{
	/// <summary>
	/// This interface represents a container which holds a text sequence and
	/// all necessary information about it. It is used as the base for a text editor.
	/// </summary>
	public interface IDocumentAggregator
	{
		IProperties Properties {
			get;
			set;
		}
		
		UndoStack UndoStack {
			get;
		}
		
		bool ReadOnly {
			get;
			set;
		}
		
		bool UpdateDocumentRequested {
			get;
			set;
		}
		
		bool UpdateCaretLineRequested {
			get;
			set;
		}
		
		SelectionCollection SelectionCollection {
			get;
		}
		
		LineSegmentCollection LineSegmentCollection {
			get;
		}
				
		IFormattingStrategy FormattingStrategy {
			get;
			set;
		}
		
		ITextBufferStrategy TextBufferStrategy {
			get;
		}
		
		IFoldingStrategy FoldingStrategy {
			get;
			set;
		}
		
		ITextModel TextModel {
			get;
		}
		
		ICaret Caret {
			get;
		}
		
		IHighlightingStrategy HighlightingStrategy {
			get;
			set;
		}
		
		IBookMarkManager BookmarkManager {
			get;
		}
		
		// Caret Stuff
		void SetDesiredColumn();
		void SetCaretToDesiredColumn(LineSegment line);
		
		//LineManager stuff
		int TotalNumberOfLines {
			get;
		}
		
		int GetLineNumberForOffset(int offset);
		LineSegment GetLineSegmentForOffset(int offset);
		LineSegment GetLineSegment(int lineNumber);
		
		// functions that are used for folding
		
		/// <remarks>
		/// Get the logical line for a given visible line.
		/// example : lineNumber == 100 foldings are in the linetracker
		/// between 0..1 (2 folded, invisible lines) this method returns 102
		/// the 'logical' line number
		/// </remarks>
		int GetLogicalLine(int lineNumber);
		
		/// <remarks>
		/// Get the visible line for a given logical line.
		/// example : lineNumber == 100 foldings are in the linetracker
		/// between 0..1 (2 folded, invisible lines) this method returns 98
		/// the 'visible' line number
		/// </remarks>
		int GetVisibleLine(int lineNumber);
		
		int GetNextVisibleLineAbove(int lineNumber, int lineCount);
		int GetNextVisibleLineBelow(int lineNumber, int lineCount);
		
		// TextStore Interface
		string TextContent {
			get;
			set;
		}
		
		int TextLength {
			get;
		}
		
		void Insert(int offset, string text);
		void Remove(int offset, int length);
		void Replace(int offset, int length, string text);
		char GetCharAt(int offset);
		string GetText(int offset, int length);
		
		// TextModel interface
		Point OffsetToView(int offset);
		int ViewToOffset(Point p);
		int GetViewXPos(LineSegment line, int logicalXPos);
		int GetLogicalXPos(LineSegment line, int viewXPos);
		
		event DocumentAggregatorEventHandler DocumentAboutToBeChanged;
		event DocumentAggregatorEventHandler DocumentChanged;
	}
}
