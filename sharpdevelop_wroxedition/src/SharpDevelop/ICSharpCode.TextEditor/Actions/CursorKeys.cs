// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System.Drawing;
using System.Windows.Forms;
using System;

using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor.Actions 
{
	public class CaretLeft : AbstractEditAction
	{
		public override void Execute(IEditActionHandler services)
		{
			int curLineNr     = services.Document.GetLineNumberForOffset(services.Document.Caret.Offset);
			int curLineOffset = services.Document.GetLineSegment(curLineNr).Offset;
			
			int relOffset = services.Document.Caret.Offset - curLineOffset;
			
			if (relOffset > 0) {
				--services.Document.Caret.Offset;
			} else if (curLineNr > 0) {
				LineSegment lineAbove = services.Document.GetLineSegment(services.Document.GetNextVisibleLineBelow(curLineNr, 1));
				services.Document.Caret.Offset = lineAbove.Offset + lineAbove.Length;
			}
			services.Document.SetDesiredColumn();
		}
	}
	
	public class CaretRight : AbstractEditAction
	{
		public override void Execute(IEditActionHandler services)
		{
			int curLineNr     = services.Document.GetLineNumberForOffset(services.Document.Caret.Offset);
			int curLineOffset = services.Document.GetLineSegment(curLineNr).Offset;
			
			int relOffset = services.Document.Caret.Offset - curLineOffset;
			
			if (relOffset < services.Document.GetLineSegment(curLineNr).Length) {
				++services.Document.Caret.Offset;
			} else if (curLineNr < services.Document.TotalNumberOfLines - 1) {
				services.Document.Caret.Offset = services.Document.GetLineSegment(services.Document.GetNextVisibleLineAbove(curLineNr, 1)).Offset;;
			}
			
			services.Document.SetDesiredColumn();
		}
	}
	
	public class CaretUp : AbstractEditAction
	{
		public override void Execute(IEditActionHandler services)
		{
			int curLineNr = services.Document.GetLineNumberForOffset(services.Document.Caret.Offset);
			
			if (curLineNr > 0) {
				LineSegment line = services.Document.GetLineSegment(services.Document.GetNextVisibleLineBelow(curLineNr, 1));
				services.Document.SetCaretToDesiredColumn(line);
			}
		}
	}
	
	public class CaretDown : AbstractEditAction
	{
		public override void Execute(IEditActionHandler services)
		{
			int curLineNr = services.Document.GetLineNumberForOffset(services.Document.Caret.Offset);
			
			if (curLineNr + 1 < services.Document.TotalNumberOfLines) {
				LineSegment line = services.Document.GetLineSegment(services.Document.GetNextVisibleLineAbove(curLineNr, 1));
				services.Document.SetCaretToDesiredColumn(line);
			}
		}
	}
	
	public class WordRight : CaretRight
	{
		public override void Execute(IEditActionHandler services)
		{
			LineSegment line = services.Document.GetLineSegmentForOffset(services.Document.Caret.Offset);
			if (services.Document.Caret.Offset == line.Offset + line.Length || line.Length == 0) {
				base.Execute(services);
			} else {
				int nextWordStart = TextUtilities.FindNextWordStart(services.Document, services.Document.Caret.Offset);
				services.Document.Caret.Offset = nextWordStart;
			}
			services.Document.SetDesiredColumn();
		}
	}	
	
	public class WordLeft : CaretLeft
	{
		public override void Execute(IEditActionHandler services)
		{
			LineSegment line = services.Document.GetLineSegmentForOffset(services.Document.Caret.Offset);
			if (services.Document.Caret.Offset == line.Offset || line.Length == 0) {
				base.Execute(services);
			} else {
				int prevWordStart = TextUtilities.FindPrevWordStart(services.Document, services.Document.Caret.Offset);
				services.Document.Caret.Offset = prevWordStart;
			}
			services.Document.SetDesiredColumn();
		}
	}
	
	public class ScrollLineUp : AbstractEditAction
	{
		public override void Execute(IEditActionHandler services)
		{
			services.AutoClearSelection = false;
			services.FirstVisibleColumn = Math.Max(0, services.FirstVisibleColumn - 1);
		}
	}
	
	public class ScrollLineDown : AbstractEditAction
	{
		public override void Execute(IEditActionHandler services)
		{
			services.AutoClearSelection = false;
			services.FirstVisibleColumn = Math.Max(0, Math.Min(services.Document.TotalNumberOfLines - 3,  services.FirstVisibleColumn + 1));
		}
	}
}
