// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor
{
	/// <summary>
	/// This class views the line numbers and folding markers.
	/// </summary>
	public class Gutter : UserControl
	{
		string lineNumberFormat = "{0,3}";
		
		TextAreaPainter  textarea; 
		int virtualTop;
		int lastline = 0;
		
		public int VirtualTop {
			get {
				return virtualTop;
			}
			set {
				virtualTop = value;
			}
		}
		
		public Gutter(TextAreaPainter textarea)
		{
			this.textarea = textarea;
			Visible       = true;
			textarea.Document.DocumentChanged += new DocumentAggregatorEventHandler(LineCountChange);
			ResizeRedraw  = false;
			
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.Selectable, false);
		}
		
		void LineCountChange(object sender, DocumentAggregatorEventArgs e)
		{
			float y1 = (lastline * textarea.FontHeight);
			float y2 = (e.Document.TotalNumberOfLines * textarea.FontHeight);
			lastline = e.Document.TotalNumberOfLines;
			
			int numberOfPositions = lastline < 1000 ? 3 : (int)Math.Log10(lastline) + 1;
			lineNumberFormat = "{0," + numberOfPositions.ToString()+ "}";
			int yMin = (int)Math.Min(y1, y2) - virtualTop;
			int yMax = (int)Math.Max(y1, y2) - virtualTop;
			Invalidate(new Rectangle(0, yMin, Width, yMax-yMin+1));			
			Invalidate(new Rectangle(Width - 12, 0, Width, Height));
			Update();
		}
		
		void DrawFoldMarker(Graphics g, RectangleF rectangle, bool isClosed)
		{
			HighlightColor foldMarkerColor = textarea.Document.HighlightingStrategy.GetColorFor("FoldMarker");
			HighlightColor foldLineColor   = textarea.Document.HighlightingStrategy.GetColorFor("FoldLine");
			
			g.FillRectangle(new SolidBrush(foldMarkerColor.BackgroundColor), rectangle);
			g.DrawRectangle(new Pen(new SolidBrush(foldMarkerColor.Color)), new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height));
			
			g.DrawLine(new Pen(new SolidBrush(foldLineColor.BackgroundColor)), 
			           rectangle.X + 2, 
			           rectangle.Y + rectangle.Height / 2, 
			           rectangle.X + rectangle.Width - 2, 
			           rectangle.Y + rectangle.Height / 2);
			
			if (isClosed) {
				g.DrawLine(new Pen(new SolidBrush(foldLineColor.BackgroundColor)), 
				           rectangle.X + rectangle.Width / 2, 
				           rectangle.Y + 2, 
				           rectangle.X + rectangle.Width / 2, 
				           rectangle.Y + rectangle.Height - 2);
			}
		}
		
		protected override void OnPaint(PaintEventArgs pe)
		{
			try {
				Graphics  drawTo     =  pe.Graphics;
				
				HighlightColor lineNumberPainterColor = textarea.Document.HighlightingStrategy.GetColorFor("LineNumbers");
				HighlightColor foldLineColor          = textarea.Document.HighlightingStrategy.GetColorFor("FoldLine");
				
				if (Enabled) {
					int realline = textarea.Document.GetLogicalLine((int)((pe.ClipRectangle.Y + virtualTop) / textarea.FontHeight));
					int lastline = (int)((pe.ClipRectangle.Y + pe.ClipRectangle.Height + virtualTop) / textarea.FontHeight);
					
					for (int line = realline; line <= lastline && realline < textarea.Document.TotalNumberOfLines; ++line) {
						
						// search next visible line
						while (realline < textarea.Document.TotalNumberOfLines && !textarea.Document.GetLineSegment(realline).IsVisible) {
							++realline;
						}
						if (realline >= textarea.Document.TotalNumberOfLines) {
							break;
						}
						
						if (textarea.Properties.GetProperty("ShowLineNumbers", true)) {
							drawTo.DrawString(String.Format(lineNumberFormat, realline + 1),
							                  lineNumberPainterColor.Font,
							                  new SolidBrush(lineNumberPainterColor.Color),
							                  new PointF(0, textarea.FontHeight * line - virtualTop));
						}
						
						if (textarea.Document.Properties.GetProperty("EnableFolding", true)) {
							LineSegment prevLine = realline > 0 && realline - 1 < textarea.Document.TotalNumberOfLines ? textarea.Document.GetLineSegment(realline - 1) : null;
							LineSegment curLine  = realline < textarea.Document.TotalNumberOfLines ? textarea.Document.GetLineSegment(realline) : null;
							LineSegment nextLine = realline + 1 < textarea.Document.TotalNumberOfLines ? textarea.Document.GetLineSegment(realline + 1) : null;
							
							if (nextLine != null && !nextLine.IsVisible) {
									DrawFoldMarker(drawTo, new RectangleF(Width - 9, 
																		 (textarea.FontHeight * line + (textarea.FontHeight - 8) / 2) - virtualTop, 
																		  8, 
																		  8),
														  !nextLine.IsVisible);						
							} else if (curLine != null) {
								bool first = nextLine != null ? curLine.FoldLevel < nextLine.FoldLevel : false;
								bool last  = prevLine != null ? curLine.FoldLevel > prevLine.FoldLevel : false;
								if (first) {
									DrawFoldMarker(drawTo, new RectangleF(Width - 9, 
																		 (textarea.FontHeight * line + (textarea.FontHeight - 8) / 2) - virtualTop, 
																		  8, 
																		  8),
														   !nextLine.IsVisible);
									drawTo.DrawLine(new Pen(new SolidBrush(foldLineColor.Color)),
													Width - 12,
													(int)(textarea.FontHeight * line),
													Width - 12,
													(int)(textarea.FontHeight * line + textarea.FontHeight / 2) - virtualTop);
								} else if (last) {
									drawTo.DrawLine(new Pen(new SolidBrush(foldLineColor.Color)),
													Width - 12,
													(int)(textarea.FontHeight * line + textarea.FontHeight / 2),
													Width - 12,
													(int)(textarea.FontHeight * line + textarea.FontHeight + 1)- virtualTop);
								} else {
									if (curLine.FoldLevel > 0) {
										drawTo.DrawLine(new Pen(new SolidBrush(foldLineColor.Color)),
														Width - 12,
														(int)(textarea.FontHeight * line),
														Width - 12,
														(int)(textarea.FontHeight * line + textarea.FontHeight + 1)- virtualTop);
									}
								}
							}
							
						}
						++realline;
					}
				}
			} catch (Exception) {} // insanity check
		}
		
		protected override void OnPaintBackground(PaintEventArgs pe)
		{
			try {
				HighlightColor lineNumberPainterColor = textarea.Document.HighlightingStrategy.GetColorFor("LineNumbers");
				pe.Graphics.FillRectangle(Enabled ? new SolidBrush(lineNumberPainterColor.BackgroundColor) :
													SystemBrushes.InactiveBorder, 
										  pe.ClipRectangle);
			} catch (Exception) {} // insanity check
		}
		
		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			bool  showFolding = textarea.Document.Properties.GetProperty("EnableFolding", true);
			Point mousepos    = PointToClient(Control.MousePosition);
			int   realline    = textarea.Document.GetVisibleLine((int)((mousepos.Y + virtualTop) / textarea.FontHeight));
			
			// focus the textarea if the user clicks on the line number view
			textarea.Focus();
			
			if (!showFolding || mousepos.X < Width - 15 || realline < 0 || realline + 1 >= textarea.Document.TotalNumberOfLines) {
				return;
			}
			
			LineSegment prevLine = realline > 0 && realline - 1 < textarea.Document.TotalNumberOfLines ? textarea.Document.GetLineSegment(realline - 1) : null;
			LineSegment curLine  = realline < textarea.Document.TotalNumberOfLines ? textarea.Document.GetLineSegment(realline) : null;
			LineSegment nextLine = realline + 1 < textarea.Document.TotalNumberOfLines ? textarea.Document.GetLineSegment(realline + 1) : null;
			
			int startFoldLevel = curLine.FoldLevel;
			if (nextLine != null && !nextLine.IsVisible) {
				while (true) {
					if (realline + 1 >= textarea.Document.TotalNumberOfLines) {
						curLine.IsVisible = true;
						break;
					}
					nextLine = textarea.Document.GetLineSegment(realline + 1);
					curLine.IsVisible = true;
					if (nextLine.IsVisible) {
						break;
					}
					
					curLine = nextLine;
					++realline;
				}
			} else if (curLine != null) {
				bool first = nextLine != null ? curLine.FoldLevel < nextLine.FoldLevel : false;
				curLine = nextLine; 
				realline++;
				if (first) {
					while (true) {
						if (realline + 1 >= textarea.Document.TotalNumberOfLines) {
							break;
						}
						
						nextLine = textarea.Document.GetLineSegment(realline + 1);
						curLine.IsVisible = false;
						if (nextLine.FoldLevel <= startFoldLevel) {
							break;
						}
						
						curLine = nextLine;
						++realline;
					}
				}
			}
					
			Refresh();
			textarea.Refresh();
		}
	}
}
