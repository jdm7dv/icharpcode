// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class Highlight
	{
		int offset;
		public int Offset {
			get {
				return offset;
			}
		}
		
		public Highlight(int offset)
		{
			this.offset = offset;
		}
	}
	
	public class BracketHighlightingSheme
	{
		char opentag;
		char closingtag;
		
		public char OpenTag {
			get {
				return opentag;
			}
			set {
				opentag = value;
			}
		}
		
		public char ClosingTag {
			get {
				return closingtag;
			}
			set {
				closingtag = value;
			}
		}
		
		public BracketHighlightingSheme(char opentag, char closingtag)
		{
			this.opentag    = opentag;
			this.closingtag = closingtag;
		}
		public Highlight GetHighlight(IDocumentAggregator document, int offset)
		{
//			Point wordcoordinate =  new Point(pos.X - 1, pos.Y);
//			Debug.Assert(wordcoordinate.X >= 0 && 
//			             wordcoordinate.Y >= 0 && 
//			             wordcoordinate.Y < buffer.Length &&
//			             wordcoordinate.X < buffer[wordcoordinate.Y].Text.Length 
//			             , "ICSharpCode.SharpDevelop.Gui.Edit.Text.BracketHighlightingSheme.GetHighlight(TextBuffer buffer, Point pos) : pos not valid.");
			
			char word = document.GetCharAt(Math.Max(0, Math.Min(document.TextLength - 1, offset)));
			
			if (word == opentag) {
				if (offset < document.TextLength) {
					int bracketOffset = TextUtilities.SearchBracketForward(document, offset + 1, opentag, closingtag);
					if (bracketOffset >= 0) {
						return new Highlight(bracketOffset);
					}
				}
			} else if (word == closingtag) {
				if (offset > 0) {
					int bracketOffset = TextUtilities.SearchBracketBackward(document, offset - 1, opentag, closingtag);
					if (bracketOffset >= 0) {
						return new Highlight(bracketOffset);
					}
				}
			}
			return null;
		}
	}
	
	public class BracketHighlighter
	{
		TextAreaPainter textareapainter;
		ArrayList       bracketshemes   = new ArrayList();
		
		public Highlight Highlight;
		
		public BracketHighlighter(TextAreaPainter textareapainter)
		{
			this.textareapainter = textareapainter;
			
			bracketshemes.Add(new BracketHighlightingSheme('{', '}'));
			bracketshemes.Add(new BracketHighlightingSheme('(', ')'));
			bracketshemes.Add(new BracketHighlightingSheme('[', ']'));
			
			textareapainter.Document.Caret.OffsetChanged += new CaretEventHandler(CaretPosChanged);
		}
		
		void CaretPosChanged(object sender, CaretEventArgs e)
		{
			bool changed = false;
			if (textareapainter.Document.Caret.Offset == 0) {
				return;
			}
			foreach (BracketHighlightingSheme bracketsheme in bracketshemes) {
//				if (bracketsheme.IsInside(textareapainter.Document, textareapainter.Document.Caret.Offset)) {
					Highlight highlight = bracketsheme.GetHighlight(textareapainter.Document, textareapainter.Document.Caret.Offset - 1);
					if (Highlight != null && Highlight.Offset >=0 && Highlight.Offset < textareapainter.Document.TextLength) {
						textareapainter.InvalidateOffset(Highlight.Offset);
					}
					Highlight = highlight;
					if (highlight != null) {
						changed = true;
						break; 
					}
//				}
			}
			if (changed || Highlight != null) {
				int offset = Highlight.Offset;
				if (!changed) {
					Highlight = null;
				}
				if (offset >=0 && offset < textareapainter.Document.TextLength) {
					textareapainter.InvalidateOffset(offset);
				}
			}
		}
	}
}
