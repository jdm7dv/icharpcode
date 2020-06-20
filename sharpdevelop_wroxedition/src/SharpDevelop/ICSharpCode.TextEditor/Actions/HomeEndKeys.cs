// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor.Actions 
{
	public class Home : AbstractEditAction
	{
		public override void Execute(IEditActionHandler services)
		{
			LineSegment curLine = services.Document.GetLineSegmentForOffset(services.Document.Caret.Offset);
			
			if (TextUtilities.IsEmptyLine(services.Document, curLine)) {
				if (services.Document.Caret.Offset != curLine.Offset) {
					services.Document.Caret.Offset = curLine.Offset;
				} else if (curLine.Length > 0) {
					services.Document.Caret.Offset = curLine.Offset + curLine.Length;
				}
			} else {
				int firstCharOffset = TextUtilities.GetFirstNonWSChar(services.Document, curLine.Offset);
				
				if (services.Document.Caret.Offset == firstCharOffset) {
					if (services.Document.Caret.Offset != curLine.Offset) {
						services.Document.Caret.Offset = curLine.Offset;
					}
				} else {
					services.Document.Caret.Offset = firstCharOffset;
				}
			}
			
			services.Document.SetDesiredColumn();
		}
	}
	
	
	public class End : AbstractEditAction
	{
		public override void Execute(IEditActionHandler services)
		{
			LineSegment curLine = services.Document.GetLineSegmentForOffset(services.Document.Caret.Offset);
			services.Document.Caret.Offset        = curLine.Offset + curLine.Length;
			services.Document.SetDesiredColumn();
		}		
	}
	
	
	public class MoveToStart : AbstractEditAction
	{
		public override void Execute(IEditActionHandler services)
		{
			services.Document.Caret.Offset = 0;
			services.Document.SetDesiredColumn();
		}
	}
	
	
	public class MoveToEnd : AbstractEditAction
	{
		public override void Execute(IEditActionHandler services)
		{
			services.Document.Caret.Offset = services.Document.TextLength;
			services.Document.SetDesiredColumn();
		}
	}
}
