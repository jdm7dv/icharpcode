// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor
{
	/// <summary>
	/// This class handles all mouse stuff for a textarea.
	/// </summary>
	public class TextAreaMouseHandler
	{
		TextAreaControl textarea;
		bool  doubleclick = false;
	    Point mousepos = new Point(0, 0);
		int  selbegin, selend;
		bool clickedOnSelectedText = false;
		
		MouseButtons button;
		
		public TextAreaMouseHandler(TextAreaControl textarea)
		{
			this.textarea = textarea;
		}
		
		public void Attach(TextAreaControl control)
		{
			control.TextAreaPainter.Click       += new EventHandler(OnMouseClick);
			control.TextAreaPainter.MouseUp     += new MouseEventHandler(OnMouseUp);
			control.TextAreaPainter.MouseDown   += new MouseEventHandler(OnMouseDown);
			control.TextAreaPainter.MouseMove   += new MouseEventHandler(OnMouseMove);
			control.TextAreaPainter.DoubleClick += new EventHandler(OnDoubleClick);
		}
		
		public void OnMouseClick(object sender, EventArgs e)
		{
			if (dodragdrop) {
				return;
			}

			if (clickedOnSelectedText) {
				textarea.ClearSelection();
				
				Point realmousepos = new Point(0, 0);
				
				realmousepos.Y = Math.Min(textarea.Document.TotalNumberOfLines - 1, Math.Max(0, ((int)((mousepos.Y + textarea.TextAreaPainter.ScreenVirtualTop) / textarea.TextAreaPainter.FontHeight))));
				LineSegment line = textarea.Document.GetLineSegment(realmousepos.Y);
				
				realmousepos.X = textarea.TextAreaPainter.GetVirtualPos(line, mousepos.X + textarea.TextAreaPainter.ScreenVirtualLeft);
				
				Point pos = new Point();
				pos.Y = Math.Min(textarea.Document.TotalNumberOfLines - 1,  realmousepos.Y);
				pos.X = realmousepos.X;
				
				textarea.Document.Caret.Offset = textarea.Document.ViewToOffset(pos);
				textarea.Document.SetDesiredColumn();

			} 
		}
		
		public void OnMouseUp(object sender, MouseEventArgs e)
		{
		}
		
			
		public void OnMouseDown(object sender, MouseEventArgs e)
		{ 
			if (dodragdrop) {
				return;
			}
			
			if (doubleclick) {
				doubleclick = false;
				return;
			}
			
			button = e.Button;
			
			if (button == MouseButtons.Left) {
				Point realmousepos = new Point(0, 0);
				
				realmousepos.Y = textarea.Document.GetVisibleLine(Math.Min(textarea.Document.TotalNumberOfLines - 1, Math.Max(0, ((int)((mousepos.Y + textarea.TextAreaPainter.ScreenVirtualTop)/ textarea.TextAreaPainter.FontHeight)))));
				LineSegment line = textarea.Document.GetLineSegment(realmousepos.Y);
				
				realmousepos.X = textarea.TextAreaPainter.GetVirtualPos(line, mousepos.X + textarea.TextAreaPainter.ScreenVirtualLeft);
				
				clickedOnSelectedText = false;
				
				int offset = line.Offset + Math.Min(line.Length, realmousepos.X);
						
				
				if (textarea.HasSomethingSelected && 
				    textarea.IsSelected(offset)) {	
					clickedOnSelectedText = true;
				} else {
					selbegin = selend = offset;
					textarea.ClearSelection();
					first = true;
					if (mousepos.Y > 0 && mousepos.Y < textarea.TextAreaPainter.Height) {
						Point pos = new Point();
						pos.Y = Math.Min(textarea.Document.TotalNumberOfLines - 1,  realmousepos.Y);
						pos.X = realmousepos.X;
						
						textarea.Document.Caret.Offset = line.Offset + Math.Min(line.Length, pos.X);
						textarea.Document.SetDesiredColumn();
					}
				}
			}
			textarea.Focus();
		}
		
		bool dodragdrop = false;
		bool first = true; // HACK WARNING !!! GOT OnMouseMove everytime I 
		                   // open a damn window 
		public void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (dodragdrop) {
				dodragdrop = false;
				return;
			}
			
			doubleclick = false;
			textarea.TextAreaPainter.OnToolTipEvent(e.X, e.Y);
			mousepos = new Point (e.X, e.Y);
			
			if (clickedOnSelectedText) {
				clickedOnSelectedText = false;
				ISelection selection = textarea.GetSelectionAt(textarea.Document.Caret.Offset);
				if (selection != null) {
					string text = selection.SelectedText;
					if (text != null && text.Length > 0) {
						DataObject dataObject = new DataObject ();
						dataObject.SetData(text);
						dataObject.SetData(selection);
						dodragdrop = true;
						textarea.TextAreaPainter.DoDragDrop(dataObject, DragDropEffects.All);
					}
					return;
				}
			}
			
			if (e.Button == MouseButtons.Left) {
				if  (!first) {
					Point realmousepos = new Point(0, 0);
					
					realmousepos.Y = textarea.Document.GetVisibleLine(Math.Min(textarea.Document.TotalNumberOfLines - 1, Math.Max(0, ((int)((mousepos.Y + textarea.TextAreaPainter.ScreenVirtualTop)/ textarea.TextAreaPainter.FontHeight)))));
					LineSegment line = textarea.Document.GetLineSegment(realmousepos.Y);
					realmousepos.X = textarea.TextAreaPainter.GetVirtualPos(line, mousepos.X + textarea.TextAreaPainter.ScreenVirtualLeft);

					int y1 = textarea.Document.Caret.Offset;
					int offset = line.Offset + realmousepos.X;
					
					textarea.ExtendSelection(textarea.Document.Caret.Offset, offset);
					textarea.Document.Caret.Offset = offset;
					textarea.Document.SetDesiredColumn();
				}
			} 
			first = false;
		}
		
		int FindNext(IDocumentAggregator document, int offset, char ch)
		{
			LineSegment line = document.GetLineSegmentForOffset(offset);
			int         endPos = line.Offset + line.Length;
			
			while (offset < endPos && document.GetCharAt(offset) != ch) {
				++offset;
			}
			return offset;
		}
		
		bool IsSelectableChar(char ch)
		{
			return Char.IsLetterOrDigit(ch) || ch=='_';
		}
		
		int FindWordStart(IDocumentAggregator document, int offset)
		{
			LineSegment line = document.GetLineSegmentForOffset(offset);
			
			if (offset > 0 && Char.IsWhiteSpace(document.GetCharAt(offset - 1)) && Char.IsWhiteSpace(document.GetCharAt(offset))) {
				while (offset > line.Offset && Char.IsWhiteSpace(document.GetCharAt(offset - 1))) {
					--offset;
				}
			} else  if (IsSelectableChar(document.GetCharAt(offset)) || (offset > 0 && Char.IsWhiteSpace(document.GetCharAt(offset)) && IsSelectableChar(document.GetCharAt(offset - 1))))  {
				while (offset > line.Offset && IsSelectableChar(document.GetCharAt(offset - 1))) {
					--offset;
				}
			} else {
				if (offset > 0 && !Char.IsWhiteSpace(document.GetCharAt(offset - 1)) && !IsSelectableChar(document.GetCharAt(offset - 1)) ) {
					return Math.Max(0, offset - 1);
				}
			}
			return offset;
		}
		
		int FindWordEnd(IDocumentAggregator document, int offset)
		{
			LineSegment line   = document.GetLineSegmentForOffset(offset);
			int         endPos = line.Offset + line.Length;
			
			if (IsSelectableChar(document.GetCharAt(offset)))  {
				while (offset < endPos && IsSelectableChar(document.GetCharAt(offset))) {
					++offset;
				}
			} else if (Char.IsWhiteSpace(document.GetCharAt(offset))) {
				if (offset > 0 && Char.IsWhiteSpace(document.GetCharAt(offset - 1))) {
					while (offset < endPos && Char.IsWhiteSpace(document.GetCharAt(offset))) {
						++offset;
					}
				}
			} else {
				return Math.Max(0, offset + 1);
			}
			
			return offset;
		}
		
	    public void OnDoubleClick(object sender, System.EventArgs e)
	    {
			if (dodragdrop) {
				return;
			}
    	
	    	doubleclick = true;
			
			textarea.ClearSelection();
	    	if (textarea.Document.Caret.Offset < textarea.Document.TextLength) {
				switch (textarea.Document.GetCharAt(textarea.Document.Caret.Offset)) {
					case '"':
						if (textarea.Document.Caret.Offset < textarea.Document.TextLength) {
							int next = FindNext(textarea.Document, textarea.Document.Caret.Offset + 1, '"');
							textarea.ExtendSelection(textarea.Document.Caret.Offset,
							                         next > textarea.Document.Caret.Offset ? next + 1 : next);
						}
						break;
					default:
						textarea.ExtendSelection(FindWordStart(textarea.Document, textarea.Document.Caret.Offset), FindWordEnd(textarea.Document, textarea.Document.Caret.Offset));
						break;
				
				}
				// HACK WARNING !!! 
				// must refresh here, because when a error tooltip is showed and the underlined
				// code is double clicked the textarea don't update corrctly, updateline doesn't
				// work ... but the refresh does.
				// Mike
				textarea.Refresh(); 
	    	}
	    }
	}
}
