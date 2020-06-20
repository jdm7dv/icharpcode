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
using System.Text;
using System.Xml;

using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Internal.Undo;

namespace ICSharpCode.TextEditor.Document
{
	public class DefaultTextModel : ITextModel
	{
		static Encoding encoding = null;
		
		static DefaultTextModel()
		{
			try {
				encoding = Encoding.GetEncoding(Thread.CurrentThread.CurrentCulture.TextInfo.ANSICodePage);
			} catch (Exception) {
			}
		}
		
		IDocumentAggregator document;
		
		public DefaultTextModel(IDocumentAggregator document)
		{
			this.document = document;
		}
		
		public int GetLogicalXPos(LineSegment line, int viewXPos)
		{
			string text = "";
			if (line.Length > 0) {
				text = document.GetText(line.Offset, line.Length);
			}
			
			int max        = viewXPos;
			int xpos       = 0;
			int logicalPos = 0;
			
			while (logicalPos < text.Length) {
				if (xpos >= viewXPos) {
					break;
				}
				
				if (text[logicalPos] == '\t') {
					xpos += document.Properties.GetProperty("TabIndent", 4);
					xpos = (xpos / document.Properties.GetProperty("TabIndent", 4)) * document.Properties.GetProperty("TabIndent", 4);
				} else {
					if (encoding != null) {
						xpos += encoding.GetByteCount(text[logicalPos].ToString());
					} else {
						++xpos;
					}
				}
				++logicalPos;
			}
			return logicalPos;
		}
		
		public int GetViewXPos(LineSegment line, int logicalXPos)
		{
			string text = "";
			if (line.Length > 0) {
				text = document.GetText(line.Offset, line.Length);
			}
			
			int max = Math.Max(0, Math.Min(text.Length, logicalXPos));
			int xpos = 0;
			
			for (int i = 0; i < max; ++i) {
				if (text[i] == '\t') {
					xpos += document.Properties.GetProperty("TabIndent", 4);
					xpos = (xpos / document.Properties.GetProperty("TabIndent", 4)) * document.Properties.GetProperty("TabIndent", 4);
				} else {
					if (encoding != null) {
						xpos += encoding.GetByteCount(text[i].ToString());
					} else {
						++xpos;
					}
				}
			}
			return xpos;
		}
		
		public Point OffsetToView(int offset)
		{
			int lineNr    = document.GetLineNumberForOffset(offset);
			LineSegment line = document.GetLineSegment(lineNr);
			
			
			return new Point(GetViewXPos(line, offset - line.Offset), document.GetLogicalLine(lineNr));
		}
		
		public int ViewToOffset(Point p)
		{
			p.Y = document.GetVisibleLine(p.Y);
			
			LineSegment line = document.GetLineSegment(p.Y);
			
			int xpos    = 0; //logik
			int realpos = 0; //physik
			
			for (int i = 0; i < line.Length; ++i) {
				if (realpos >= p.X) {
					break;
				}
				
				if (document.GetCharAt(line.Offset + i) == '\t') {
					realpos += document.Properties.GetProperty("TabIndent", 4);
					realpos = (realpos / document.Properties.GetProperty("TabIndent", 4)) * document.Properties.GetProperty("TabIndent", 4);
				} else {
					++realpos;
				}
				
				++xpos;
			}
			
//			if (to.X > realpos && buffer.BufferOptions.CursorBehindEOL) {
//				xpos += to.X - realpos;
//			}
			return line.Offset + xpos;
		}
	}
}
