// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

namespace ICSharpCode.TextEditor.Document
{
	/// <summary>
	/// The line tracker keeps track of all lines in a document.
	/// </summary>
	public interface ILineManager
	{
		/// <remarks>
		/// Get a collection of all line segments
		/// </remarks>
		LineSegmentCollection LineSegmentCollection {
			get;
		}
		
		/// <remarks>
		/// get LineSegmentCollection.Count
		/// </remarks>
		int TotalNumberOfLines {
			get;
		}
		
		IHighlightingStrategy HighlightingStrategy {
			get;
			set;
		}
		
		int GetLineNumberForOffset(int offset);
		
		LineSegment GetLineSegmentForOffset(int offset);
		LineSegment GetLineSegment(int lineNumber);
		
		void Insert(int offset, string text);
		void Remove(int offset, int length);
		void Replace(int offset, int length, string text);
		
		void SetContent(string text);
		
		int GetLogicalLine(int lineNumber);
		int GetVisibleLine(int lineNumber);
		
		int GetNextVisibleLineAbove(int lineNumber, int lineCount);
		int GetNextVisibleLineBelow(int lineNumber, int lineCount);
		
		event LineManagerEventHandler LineCountChanged;
	}
}
