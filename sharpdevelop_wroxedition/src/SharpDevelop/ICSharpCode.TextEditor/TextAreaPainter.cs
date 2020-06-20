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

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.Properties;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor
{
	public delegate void ToolTipEvent(int xpos, int ypos);
	
	/// <summary>
	/// To add a own painter, make this delagte.
	/// </summary>
	public delegate void LinePainter(Graphics g, int line, RectangleF rect, PointF pos, int virtualLeft, int virtualTop);
	
	/// <summary>
	/// This class paints the textarea.
	/// </summary>
	public class TextAreaPainter : Control
	{
		/// <remarks>
		/// The TextAreaControl that owns this painter
		/// </remarks>
		TextAreaControl textAreaControl = null;
		
		int screenVirtualTop  = 0;
		int screenVirtualLeft = 0;
		
		IDocumentAggregator document;
		PrintDocument       printdocument;
		
		StringFormat measureStringFormat = StringFormat.GenericTypographic;
		 
		ToolTip toolTip = new ToolTip();
		
		float fontWidth;
		float fontHeight;
		
		bool iHaveTheFocusLock = false;
		bool iHaveTheFocus = false;
		
		// internationalization support
		Ime    ime = null;
		static Encoding encoding = null;
		
		public ToolTip ToolTip {
			get {
				return toolTip;
			}
			set {
				toolTip = value;
			}
		}
		
		public bool IHaveTheFocusLock {
			get {
				return iHaveTheFocusLock;
			}
			set {
				iHaveTheFocusLock = value;
			}
		}
		
		static TextAreaPainter()
		{
			try {
				encoding = Encoding.GetEncoding(Thread.CurrentThread.CurrentCulture.TextInfo.ANSICodePage);
			} catch (Exception) {
			}
		}
		
		public bool IHaveTheFocus {
			get {
				return iHaveTheFocus;
			}
			set {
				iHaveTheFocus = value;
				OnIHaveTheFocusChanged(EventArgs.Empty);
			}
		}
		
		public int ScreenVirtualTop {
			get {
				return screenVirtualTop;
			}
			set {
				screenVirtualTop = value;
			}
		}
		
		public int ScreenVirtualLeft {
			get {
				return screenVirtualLeft;
			}
			set {
				screenVirtualLeft = value;
			}
		}

		public PrintDocument PrintDocument {
			get {
				return printdocument;
			}
		}
		
		public IDocumentAggregator Document {
			get {
				return document;
			}
		}
		
		public IProperties Properties {
			get {
				return document.Properties;
			}
		}
		
		public float FontWidth {
			get {
				return fontWidth;
			}
		}
		
		public new float FontHeight {
			get {
				return fontHeight;
			}
		}
		
		public TextAreaControl TextAreaControl {
			get {
				return textAreaControl;
			}
		}
		
		void PaintInvalidTextLine(Graphics g, int x, int y)
		{
			if (Properties.GetProperty("ShowInvalidLines", true)) {
				HighlightColor invalidLinesColor = document.HighlightingStrategy.GetColorFor("InvalideLines");
				
				g.DrawString("~", invalidLinesColor.Font, new SolidBrush(invalidLinesColor.Color), x, y, measureStringFormat);
			}
		}
		
		/// <summary>
	    /// This Method paints the characters on the screen
	    /// </summary>
		void PaintTextLine(Graphics g, int lineNumber, RectangleF rect, PointF pos, int virtualLeft, int virtualTop)
		{
			int  logicalXPosition = (int)(pos.X / fontWidth);
			bool lineIsBookmark   = Document.BookmarkManager.IsMarked(lineNumber);
			HighlightColor defaultColor = document.HighlightingStrategy.GetColorFor("DefaultColor");
			
			double drawingXPos  = -virtualLeft;
			if (lineNumber >= Document.TotalNumberOfLines) {
				PaintInvalidTextLine(g, (int)(pos.X - virtualLeft), (int)(pos.Y - virtualTop));
			} else {
				LineSegment line = document.GetLineSegment(lineNumber);
				
				if (line.Words == null) {
					string text = document.GetText(line.Offset, line.Length);
					
					float xPos = 0;
					for (int i = 0; i < text.Length; ++i) {
						if (text[i] == '\t') {
							g.DrawString("    ", defaultColor.Font, new SolidBrush(defaultColor.Color), xPos - virtualLeft, pos.Y - virtualTop);
							xPos += g.MeasureString("    ", defaultColor.Font, 100, measureStringFormat).Width;
						} else {
							g.DrawString(text[i].ToString(), defaultColor.Font, new SolidBrush(defaultColor.Color), xPos - virtualLeft, pos.Y - virtualTop);
							xPos += g.MeasureString(text[i].ToString(), defaultColor.Font, 100, measureStringFormat).Width; 
						}
					}
				} else {
					HighlightColor spaceMarkerColor = document.HighlightingStrategy.GetColorFor("SpaceMarker");
					HighlightColor tabMarkerColor   = document.HighlightingStrategy.GetColorFor("TabMarker");
					HighlightColor bookmarkColor    = document.HighlightingStrategy.GetColorFor("Bookmark");
					
					for (int i = 0; i < line.Words.Count; ++i) {
						switch (((TextWord)line.Words[i]).Type) {
							case TextWordType.Space:
								if (Properties.GetProperty("ShowSpaces", false) && pos.X < rect.X + rect.Width) {
									g.DrawString("\u00B7", spaceMarkerColor.Font, new SolidBrush(spaceMarkerColor.Color), (int)drawingXPos, pos.Y - virtualTop);
								}
								drawingXPos += g.MeasureString(" ", defaultColor.Font, 2000, measureStringFormat).Width;
								++logicalXPosition;
								break;
							
							case TextWordType.Tab:
								if (Properties.GetProperty("ShowTabs", false) && pos.X < rect.X + rect.Width) {
									g.DrawString("\u00BB", tabMarkerColor.Font, new SolidBrush(tabMarkerColor.Color), (int)drawingXPos, pos.Y - virtualTop);
								}
								int oldLogicalXPosition = logicalXPosition;
								logicalXPosition += Properties.GetProperty("TabIndent", 4);
								logicalXPosition = (logicalXPosition / Properties.GetProperty("TabIndent", 4)) * Properties.GetProperty("TabIndent", 4);
								
								string measureMe = "";
								for (int j = 0; j < logicalXPosition - oldLogicalXPosition; ++j) {
									measureMe += " ";
								}
								drawingXPos += g.MeasureString(measureMe, defaultColor.Font, 2000, measureStringFormat).Width;
								break;
							
							case TextWordType.Word:
								g.DrawString(((TextWord)line.Words[i]).Word, ((TextWord)line.Words[i]).Font, new SolidBrush(lineIsBookmark ? bookmarkColor.Color : ((TextWord)line.Words[i]).Color), (int)drawingXPos, pos.Y - virtualTop, measureStringFormat);
								
								drawingXPos += g.MeasureString(((TextWord)line.Words[i]).Word, ((TextWord)line.Words[i]).Font, 2000, measureStringFormat).Width;
								
								if (encoding != null) {
									logicalXPosition += encoding.GetByteCount(((TextWord)line.Words[i]).Word);
								} else {
									logicalXPosition += ((TextWord)line.Words[i]).Word.Length;
								}
								break;
						}
					}
				}
				
				// paint EOL markers
				if (Properties.GetProperty("ShowEOLMarkers", false) && pos.X < rect.X + rect.Width) {
					HighlightColor EolMarkerColor = document.HighlightingStrategy.GetColorFor("EolMarker");
					g.DrawString("\u00B6", EolMarkerColor.Font, new SolidBrush(EolMarkerColor.Color), (int)drawingXPos, pos.Y - virtualTop);
				}
			}
		}
		
		void PaintCursorLine(object sender, PaintEventArgs pe)
		{
			if ((LineViewerStyle)Properties.GetProperty("LineViewerStyle", LineViewerStyle.None) == LineViewerStyle.FullRow) {
				int lineNr   = document.GetLineNumberForOffset(Document.Caret.Offset);
				LineSegment line = document.GetLineSegment(lineNr);
				
				int ypos = (int)(Document.GetLogicalLine(lineNr) * fontHeight);
				HighlightColor caretMarkerColor = document.HighlightingStrategy.GetColorFor("CaretMarker");
				
				RectangleF rectangle = new RectangleF(0, ypos-ScreenVirtualTop, Width, fontHeight);
				pe.Graphics.FillRectangle(new SolidBrush(caretMarkerColor.Color), rectangle);
			}
		}
		
		void PaintVerticalRuler(object sender, PaintEventArgs pe)
		{
			if (Properties.GetProperty("ShowVRuler", false)) {
				string measureMe = "";
				for (int j = 1; j < Properties.GetProperty("VRulerRow", 80); ++j) {
					measureMe += "-";
				}
				HighlightColor vRulerColor = document.HighlightingStrategy.GetColorFor("VRulerColor");
				int xpos = (int)pe.Graphics.MeasureString(measureMe, vRulerColor.Font, 2000, measureStringFormat).Width - ScreenVirtualLeft;
				
				pe.Graphics.DrawLine(new Pen(vRulerColor.Color), 
				                     xpos, 
				                     pe.ClipRectangle.Y, 
				                     xpos, 
				                     pe.ClipRectangle.Y + pe.ClipRectangle.Height);
			}
		}
		
		public int GetVirtualPos(LineSegment line, int xPos)
		{
			Graphics g  = CreateGraphics();
			
			float  paintPos = 0;
			string text = document.GetText(line.Offset, line.Length);
			int    xpos = 0;
			HighlightColor defaultColor = document.HighlightingStrategy.GetColorFor("DefaultColor");
			
			for (int i = 0; i < line.Length; ++i) {
				char ch;
				if (i >= text.Length) {
					ch = ' ';
				} else {
					ch = text[i];
				}
				if (ch == ' ') {
					paintPos += g.MeasureString(" ",     defaultColor.Font, 32768, measureStringFormat).Width;
					++xpos;
				} else
				if (ch == '\t') {
					int oldPos = xpos;
					xpos += Properties.GetProperty("TabIndent", 4);
					xpos = (xpos / Properties.GetProperty("TabIndent", 4)) * Properties.GetProperty("TabIndent", 4);
					
					string measureMe = String.Empty;
					for (int j = 0; j < xpos - oldPos; ++j) {
						measureMe += " ";
					}
					paintPos += g.MeasureString(measureMe, defaultColor.Font, 32768, measureStringFormat).Width;
				} else {
					HighlightColor color = line.GetColorForPosition(i);
					Font f = color == null ? defaultColor.Font : color.Font;
					paintPos += g.MeasureString(ch.ToString(), f, 32768, measureStringFormat).Width;
					++xpos;
				}
				if (paintPos >= xPos) {
					g.Dispose();
					return i;
				}
			}
			g.Dispose();
			return line.Length;
		}
		
		public float GetScreenPos(Graphics g, LineSegment line, int dX) 
		{
			if (line == null) {
				return dX * fontWidth;
			}
				
			int lastChar = Math.Min(dX, line.Length);
			int delta    = dX - lastChar;
			
			float  paintPos = 0;
			string text = document.GetText(line.Offset, line.Length);
			int    xpos = 0;
			HighlightColor defaultColor = document.HighlightingStrategy.GetColorFor("DefaultColor");
			
			for (int i = 0; i < lastChar; ++i) {
				char ch;
				if (i >= text.Length) {
					ch = ' ';
				} else {
					ch = text[i];
				}
				if (ch == ' ') {
					paintPos += g.MeasureString(" ",     defaultColor.Font, 32768, measureStringFormat).Width;
					++xpos;
				} else
				if (ch == '\t') {
					int oldPos = xpos;
					xpos += Properties.GetProperty("TabIndent", 4);
					xpos = (xpos / Properties.GetProperty("TabIndent", 4)) * Properties.GetProperty("TabIndent", 4);
					
					string measureMe = String.Empty;
					for (int j = 0; j < xpos - oldPos; ++j) {
						measureMe += " ";
					}
					paintPos += g.MeasureString(measureMe,     defaultColor.Font, 32768, measureStringFormat).Width;
				} else {
					HighlightColor color = line.GetColorForPosition(i);
					Font f = color == null ? defaultColor.Font : color.Font;
					paintPos += g.MeasureString(ch.ToString(), f, 32768, measureStringFormat).Width;
					++xpos;
				}
			}
			return paintPos;
		}
		
		void PaintCaret(Graphics g)
		{
			if (Document.Caret.Visible && Document.Caret.Offset <= Document.TextLength) {
				lock (this) {
					int lineNr   = document.GetLineNumberForOffset(Document.Caret.Offset);
					LineSegment line = document.GetLineSegment(lineNr);
					
					int ypos = (int)(Document.GetLogicalLine(lineNr) * fontHeight);
					int xpos = (int)GetScreenPos(g, line, Document.Caret.Offset - line.Offset) - 2;
					
					if (ime == null) {
						ime = new Ime(this.Handle, FontContainer.DefaultFont);
					} else {
						ime.Font = FontContainer.DefaultFont;
					}
					ime.SetIMEWindowLocation(xpos + 2 - ScreenVirtualLeft, ypos - ScreenVirtualTop);  // for set ime position.
					
					HighlightColor caretColor = document.HighlightingStrategy.GetColorFor("Caret");
					
					
					switch (Document.Caret.CaretMode) {
						case CaretMode.InsertMode:
							g.DrawLine(new Pen(caretColor.Color),
							           new Point((int)xpos + 2 - ScreenVirtualLeft, ypos + 1 - ScreenVirtualTop),
							           new Point((int)xpos + 2 - ScreenVirtualLeft, ypos + (int)fontHeight - ScreenVirtualTop));
							g.DrawLine(new Pen(caretColor.Color), 
							           new Point((int)xpos + 3 - ScreenVirtualLeft, ypos + 1 - ScreenVirtualTop),
							           new Point((int)xpos + 3 - ScreenVirtualLeft, ypos + (int)fontHeight - ScreenVirtualTop));
							break;
						case CaretMode.OverwriteMode:
							g.DrawLine(
								new Pen(caretColor.Color),
								new Point((int)xpos + 3 - ScreenVirtualLeft, ypos + (int)fontHeight - ScreenVirtualTop),
								new Point((int)xpos + (int)fontWidth + 2 - ScreenVirtualLeft, ypos + (int)fontHeight - ScreenVirtualTop)
							);
							g.DrawLine(
								new Pen(caretColor.Color),
								new Point((int)xpos + 3 - ScreenVirtualLeft, ypos + (int)fontHeight - 1 - ScreenVirtualTop),
								new Point((int)xpos + (int)fontWidth + 2 - ScreenVirtualLeft, ypos + (int)fontHeight - 1 - ScreenVirtualTop)
							);
							break;
					}
				}
			}
		}
		
		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			IHaveTheFocus = true;
		}
		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			IHaveTheFocus = false;
		}
		
		protected override void OnPaint(PaintEventArgs pe)
		{
			Graphics  drawTo     = pe.Graphics;
			Rectangle rectangle  = pe.ClipRectangle;
			
			if (Enabled) {
				drawTo.CompositingQuality = CompositingQuality.AssumeLinear;
				drawTo.SmoothingMode      = SmoothingMode.AntiAlias;
				drawTo.InterpolationMode  = InterpolationMode.High;
					
				if (Properties.GetProperty("UseAntiAliasFont", false)) {
					drawTo.TextRenderingHint = TextRenderingHint.AntiAlias;
				}  else {
					drawTo.TextRenderingHint  = TextRenderingHint.AntiAliasGridFit;
				}
				
				PaintCursorLine(null, pe);
				PaintVerticalRuler(null, pe);
				
				Font   textFont      = FontContainer.DefaultFont;
				
				drawTo.SmoothingMode     = SmoothingMode.HighSpeed;
				drawTo.InterpolationMode = InterpolationMode.Low;
				
				int firstInvalid = (int)((rectangle.Y + ScreenVirtualTop) /  fontHeight);
				int lastInvalid  = (int)((rectangle.Y + rectangle.Height - 1 + ScreenVirtualTop) / fontHeight);
				
				int realline = Document.GetVisibleLine(firstInvalid);
				
				if (LinePainter != null){
					for (int line = firstInvalid; line <= lastInvalid; ++line) {
						while (realline < Document.TotalNumberOfLines && !Document.GetLineSegment(realline).IsVisible) {
							++realline;
						}
						LinePainter(drawTo, realline, (RectangleF)rectangle, new PointF(0, line * fontHeight), ScreenVirtualLeft, ScreenVirtualTop);
						++realline;
					}
				}
				PaintCaret(drawTo);
			}
		}
		
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
		}

		void PaintBookMarks(Graphics g, int line, RectangleF rect, PointF pos, int virtualLeft, int virtualTop)
		{
			if (Document.BookmarkManager.IsMarked(line)) {
				RectangleF rectangle = new RectangleF(0, pos.Y - virtualTop, Width, fontHeight);
				HighlightColor BookmarkColor = document.HighlightingStrategy.GetColorFor("Bookmark");
				
				g.FillRectangle(new SolidBrush(BookmarkColor.BackgroundColor), rectangle);
			}
		}

		/// <summary>
		/// This Method paints the background of a selected area 
		/// </summary>
		void PaintSelectedArea(Graphics g, int lineNr, RectangleF rect, PointF pos, int virtualLeft, int virtualTop)
		{
			int r_Begin = int.MaxValue;
			int r_End   = 0;
			
			if (lineNr >= document.TotalNumberOfLines) {
				return;
			}
			LineSegment line = document.GetLineSegment(lineNr);
			
			HighlightColor selectionColor = document.HighlightingStrategy.GetColorFor("Selection");
			
			foreach (ISelection selection in Document.SelectionCollection) {
				int selectionStartLine = selection.StartLine;
				int selectionEndLine   = selection.EndLine;
				
				if (lineNr < selectionStartLine || lineNr > selectionEndLine) {
					continue;
				}
				
				if (selectionStartLine != selectionEndLine) {
					if (lineNr == selectionStartLine) {
						r_Begin = selection.Offset - line.Offset;
						r_End   = line.Length + 1;
					} else {
						if (lineNr == selectionEndLine) {
							r_Begin = 0;
							r_End   = selection.Offset + selection.Length - line.Offset;
						} else {
							r_Begin = 0;
							r_End   = line.Length + 1; 
						}
					}
				} else {
					r_Begin = selection.Offset - line.Offset;
					r_End   = r_Begin + Math.Min(line.Length - r_Begin + 1, selection.Length);
				}
				r_Begin = (int)GetScreenPos(g, line, r_Begin) - 1;
				r_End   = (int)GetScreenPos(g, line, r_End) + 2;
				
				// Draw the selection rectangle
				if (r_End > 0) {
					g.FillRectangle(new SolidBrush(selectionColor.BackgroundColor), 
					                new RectangleF(r_Begin - virtualLeft, pos.Y - virtualTop + 1, r_End - r_Begin, fontHeight));
				}
			}
		}
		
		protected override void OnPaintBackground(PaintEventArgs pe)
		{
			HighlightBackground background = (HighlightBackground)document.HighlightingStrategy.GetColorFor("DefaultColor");
			if (background.BackgroundImage == null) {
				pe.Graphics.FillRectangle(Enabled ? new SolidBrush(background.BackgroundColor) : 
					                                SystemBrushes.InactiveBorder, 
					                      pe.ClipRectangle);
			} else {
// TODO : finish the draw background image routine
//				int xPos = pe.ClipRectangle.X % background.BackgroundImage.Width;
//				int yPos = pe.ClipRectangle.Y % background.BackgroundImage.Height;
//				Rectangle imgRectangle = new Rectangle(xPos,
//				                                       yPos,
//				                                       Math.Min(background.BackgroundImage.Width - xPos, pe.ClipRectangle.Width),
//				                                       Math.Min(background.BackgroundImage.Height - yPos, pe.ClipRectangle.Height));
//				                                       
//				pe.Graphics.DrawImage(background.BackgroundImage, imgRectangle, pe.ClipRectangle, GraphicsUnit.Pixel);
			}
		}
		
		public void CalculateFontSize()
		{
			Graphics g = CreateGraphics();
			fontHeight = FontContainer.DefaultFont.GetHeight(g);
			
			fontWidth  = fontHeight / 1.831f;
		}
				
		public TextAreaPainter(TextAreaControl textAreaControl, IDocumentAggregator document)
		{
			this.textAreaControl = textAreaControl;
			this.document   = document;
			
			measureStringFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
			
			this.ToolTip    = new ToolTip();
			CalculateFontSize();
			
			Cursor = Cursors.IBeam;
			printdocument = new PrintDocument();
			printdocument.BeginPrint += new PrintEventHandler(BeginPrint);
			printdocument.PrintPage += new PrintPageEventHandler(PrintPage);
			Paint += new PaintEventHandler(PaintVerticalRuler);
			ResizeRedraw  = false;
			
			LinePainter += new LinePainter(PaintBookMarks);
			LinePainter += new LinePainter(PaintSelectedArea);
			LinePainter += new LinePainter(PaintTextLine);
			
			SetStyle(ControlStyles.DoubleBuffer, Properties.GetProperty("DoubleBuffer", true));
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		}
		
		int printedline = 0;
		void BeginPrint(object sender, PrintEventArgs e)
		{
			printedline = 0;
		}
		
		void PrintPage(object sender, PrintPageEventArgs e)
		{
			Graphics g = e.Graphics;
			float  linesPerPage = 0;
			float  leftMargin = e.MarginBounds.Left;
			
			linesPerPage = e.MarginBounds.Height / fontHeight - 1;
			
			for (int line = 0; line < linesPerPage; ++line) {
				PaintTextLine(e.Graphics, printedline++, e.MarginBounds,new PointF((int)leftMargin, e.MarginBounds.Y + line * fontHeight), 0, 0);
			}
			
			g.DrawLine(new Pen(Color.Black), e.MarginBounds.Left, e.MarginBounds.Bottom + 2, e.MarginBounds.Right, e.MarginBounds.Bottom + 2);
			g.DrawLine(new Pen(Color.Black), e.MarginBounds.Left, e.MarginBounds.Top - 2, e.MarginBounds.Right, e.MarginBounds.Top - 2);
			string pagestr = "Page " + (int)(printedline / linesPerPage) + "/" + (int)(document.TotalNumberOfLines / linesPerPage + 1);
			int    width   = (int)(g.MeasureString(pagestr, FontContainer.DefaultFont).Width + 5);
			g.DrawString(pagestr, FontContainer.DefaultFont, new SolidBrush(Color.Black), e.MarginBounds.Right - width, e.MarginBounds.Bottom + 4);
			
			e.HasMorePages = printedline < document.TotalNumberOfLines;
		}
		
		public int CalculateVisualXPos(int lineNr, int xPos)
		{
			LineSegment line = document.GetLineSegment(lineNr);
			
			Graphics g  = CreateGraphics();
			float x     = GetScreenPos(g, line, xPos);
			g.Dispose();
			return (int)x;
		}
		
		public void InvalidateOffset(int offset)
		{
			if (offset < 0 || offset > Document.TextLength) {
				return;
			}
			int logicalLineNumber = Document.GetLineNumberForOffset(offset);
			int visibleLineNumber = Document.GetLogicalLine(logicalLineNumber);
			
			LineSegment line = document.GetLineSegment(logicalLineNumber);
				
			int col        = offset - line.Offset;
			
			float width = fontWidth;
			float y     = visibleLineNumber * fontHeight;
			
			float x     = 0;
			if (logicalLineNumber < document.TotalNumberOfLines) {
				Graphics g  = CreateGraphics();
				x = GetScreenPos(g, line, col);
				g.Dispose();
			} else {
				// best fit for the case that the requested row is outside of the total number of 
				// lines
				x  = fontWidth * col;
			}
			
			Invalidate(new Rectangle((int)x - ScreenVirtualLeft,
				                     (int)y - 1 - ScreenVirtualTop,
				                     (int)width + 1,
			                         (int)FontHeight + 2));
		}
		
		public void OnToolTipEvent(int xpos, int ypos)
		{
			if (ToolTipEvent != null){
				ToolTipEvent(xpos + ScreenVirtualLeft, ypos + ScreenVirtualTop);
			}
		}	
		
		protected virtual void OnIHaveTheFocusChanged(EventArgs e)
		{
			if (IHaveTheFocusLock) {
				return;
			}
			if (IHaveTheFocusChanged != null) {
				IHaveTheFocusChanged(this, e);
			}
		}
		
		public event ToolTipEvent ToolTipEvent;
		public event LinePainter  LinePainter;
		public event EventHandler IHaveTheFocusChanged;
	}
}
