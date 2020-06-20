// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.Core.Properties;

namespace ICSharpCode.TextEditor
{
	public class Ruler : UserControl
	{
		TextAreaPainter  textarea;
		
		public Ruler(TextAreaPainter textarea)
		{
			this.textarea = textarea;
		}
		
		protected override void OnPaint(PaintEventArgs pe)
		{
			float start = textarea.Properties.GetProperty("ShowLineNumbers", true) ? 40 : 10;
			for (float i = start; i < Width; i += textarea.FontWidth) {
				int lineheight = ((int)((i - start + 1) / textarea.FontWidth)) % 5 == 0 ? 4 : 6;
				
				lineheight = ((int)((i - start + 1) / textarea.FontWidth)) % 10 == 0 ? 2 : lineheight;
				
				pe.Graphics.DrawLine(new Pen(Color.Black), (int)i, Height - lineheight, (int)i, lineheight);
			}
		}
		
		protected override void OnPaintBackground(PaintEventArgs pe)
		{
			HighlightColor hColor = textarea.Document.HighlightingStrategy.GetColorFor("LineNumber");
			Color color = Enabled ? hColor.BackgroundColor : SystemColors.InactiveBorder;
			pe.Graphics.FillRectangle(new SolidBrush(color), pe.ClipRectangle);
		}
	}
}
