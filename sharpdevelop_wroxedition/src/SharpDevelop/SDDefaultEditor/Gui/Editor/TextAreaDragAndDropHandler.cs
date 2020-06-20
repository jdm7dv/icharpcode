// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.InteropServices;
using System.Xml;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.Core.Properties;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class TextAreaDragDropHandler
	{
		TextAreaControl textarea;
		TextAreaPainter painter;
		
		public void Attach(TextAreaControl textarea)
		{
			this.textarea = textarea;
			this.painter = textarea.TextAreaPainter;
			textarea.TextAreaPainter.AllowDrop = true;
			
			painter.DragEnter += new DragEventHandler(OnDragEnter);
			painter.DragDrop  += new DragEventHandler(OnDragDrop);
			painter.DragOver  += new DragEventHandler(OnDragOver);
		}
		
		static DragDropEffects GetDragDropEffect(DragEventArgs e)
		{
			if ((e.AllowedEffect & DragDropEffects.Move) > 0 &&
			    (e.AllowedEffect & DragDropEffects.Copy) > 0) {
				return (e.KeyState & 8) > 0 ? DragDropEffects.Copy : DragDropEffects.Move;
			} else if ((e.AllowedEffect & DragDropEffects.Move) > 0) {
				return DragDropEffects.Move;
			} else if ((e.AllowedEffect & DragDropEffects.Copy) > 0) {
				return DragDropEffects.Copy;
			}
			return DragDropEffects.None;
		}
		
		protected void OnDragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(string))) {
				e.Effect = GetDragDropEffect(e);
			}
		}
		
/*		Point GetCursorPos(Point screenpos)
		{
			Point p = textarea.Document.ViewToOffset(screenpos);
//			painter.Buffer.Foldings.GetPhysicalLine((int)(screenpos.Y / painter.FontHeight)));
			return screenpos;
		}*/
		
		void InsertString(string str)
		{
			painter.Document.Insert(painter.Document.Caret.Offset, str);
			
			textarea.SetSelection(new DefaultSelection(painter.Document, painter.Document.Caret.Offset, str.Length));
			painter.Document.Caret.Offset = painter.Document.Caret.Offset + str.Length;
			painter.Refresh();
			
//			if (newPos.Y != painter.Caret.CaretPos.Y) {
//				textarea.UpdateToEnd(painter.Caret.CaretPos.Y);
//			} else {
//				textarea.UpdateLines(painter.Caret.CaretPos.Y, painter.Caret.CaretPos.Y);
//			}
		}
		
		protected void OnDragDrop(object sender, DragEventArgs e)
		{
//			Console.WriteLine("OnDragDrop");
			Point p = painter.PointToClient(new Point(e.X, e.Y));
			
			if (e.Data.GetDataPresent(typeof(string))) {
				bool two = false;
				if (e.Data.GetDataPresent(typeof(DefaultSelection))) {
					ISelection sel = (ISelection)e.Data.GetData(typeof(DefaultSelection));
					if (painter.Document.Caret.Offset >= sel.Offset && 
					    painter.Document.Caret.Offset <= sel.Offset + sel.Length) {
					    	return;
					}
					if (GetDragDropEffect(e) == DragDropEffects.Move) {
						painter.Document.Remove(sel.Offset, sel.Length);
					}
					two = true;
				}
				textarea.ClearSelection();
				InsertString((string)e.Data.GetData(typeof(string)));
				if (two) {
					painter.Document.UndoStack.UndoLast(2);
				}
			}
		}
		
		protected void OnDragOver(object sender, DragEventArgs e)
		{
//			Console.WriteLine("OnDragOver");
			if (!painter.IHaveTheFocus) {
				painter.Focus();
			}
			
			Point p = painter.PointToClient(new Point(e.X, e.Y));
			
			Point realmousepos = new Point(0, 0);
// KSL Start, lines change to adjust for virtualLeft and virtualTop
			realmousepos.Y = Math.Min(textarea.Document.TotalNumberOfLines - 1, Math.Max(0, (int)((p.Y  + textarea.TextAreaPainter.ScreenVirtualTop)/ textarea.TextAreaPainter.FontHeight)));
			realmousepos.X = (int)((p.X  + textarea.TextAreaPainter.ScreenVirtualLeft) / textarea.TextAreaPainter.FontWidth);
//			realmousepos.Y = Math.Min(textarea.Document.TotalNumberOfLines - 1, Math.Max(0, (int)(p.Y / textarea.TextAreaPainter.FontHeight)));
//			realmousepos.X = (int)(p.X / textarea.TextAreaPainter.FontWidth);
// KSL End
			LineSegment line = textarea.Document.GetLineSegment(realmousepos.Y);
			
			int newOffset = textarea.Document.ViewToOffset(realmousepos);
			textarea.Document.Caret.Offset = newOffset;

			if (e.Data.GetDataPresent(typeof(string))) {
				e.Effect = GetDragDropEffect(e);
			}
		}
	}
}
