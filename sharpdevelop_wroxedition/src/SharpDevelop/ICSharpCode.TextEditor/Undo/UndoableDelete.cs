// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.Drawing;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Internal.Undo;
	
namespace ICSharpCode.TextEditor.Undo
{
	/// <summary>
	/// This class is for the undo of Document insert operations
	/// </summary>
	public class UndoableDelete : IUndoableOperation
	{
		IDocumentAggregator document;
		int      oldCaretPos;
		int      offset;
		string   text;
		
		public UndoableDelete(IDocumentAggregator document, int offset, string text)
		{
			if (document == null) {
				throw new ArgumentNullException("document");
			}
			if (offset < 0 || offset > document.TextLength) {
				throw new ArgumentOutOfRangeException("offset");
			}
			
			Debug.Assert(text != null, "text can't be null");
			oldCaretPos   = document.Caret.Offset;
			this.document = document;
			this.offset   = offset;
			this.text     = text;
		}
		
		public void Undo()
		{
			// we clear all selection direct, because the redraw
			// is done per refresh at the end of the action
			document.SelectionCollection.Clear();

			document.UndoStack.AcceptChanges = false;
			document.Insert(offset, text);
			document.Caret.Offset = Math.Min(document.TextLength, Math.Max(0, oldCaretPos));
			document.UndoStack.AcceptChanges = true;
		}
		public void Redo()
		{
			// we clear all selection direct, because the redraw
			// is done per refresh at the end of the action
			document.SelectionCollection.Clear();

			document.UndoStack.AcceptChanges = false;
			document.Remove(offset, text.Length);
			document.Caret.Offset = Math.Min(document.TextLength, Math.Max(0, document.Caret.Offset));
			document.UndoStack.AcceptChanges = true;
		}
	}
}