// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;

using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

using SharpDevelop.Internal.Parser;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class DeclarationViewWindow : Form
	{
		string description = String.Empty;
		
		public string Description {
			get {
				return description;
			}
			set {
				description = value;
				if (Visible) {
					Refresh();
				}
			}
		}
		
		public DeclarationViewWindow()
		{
			StartPosition   = FormStartPosition.Manual;
			FormBorderStyle = FormBorderStyle.None;
			TopMost         = true;
			ShowInTaskbar   = false;
			
//			Enabled         = false;
			Size            = new Size(0, 0);
			
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
		}
		
		public bool resized = false;
		protected override void OnPaint(PaintEventArgs pe)
		{
			Graphics g = pe.Graphics;
			
			SizeF size = g.MeasureString(description, Font);
			int h = (int)size.Height + 4;
			int w = (int)size.Width  + 4;
			if (!resized && (h != Height || w != Width)) {
				resized = true;
				size = g.MeasureString(description, Font, Width);
				Height = h;
				Width  = w;
				SizeF size2 = g.MeasureString(description, Font, Width);
				Height = (int)size2.Height;
				Refresh();
				return;
			}
			resized = false;
			if (description != null && description.Length > 0) {
				RectangleF rect = new RectangleF(0, 0, Width, Height);
				g.DrawRectangle(new Pen(SystemColors.WindowFrame), new Rectangle( 0, 0, Width - 1, Height - 1));
				g.DrawString(description, Font, new SolidBrush(SystemColors.InfoText), rect);
			}
		}
		
		protected override void OnPaintBackground(PaintEventArgs pe)
		{
			if (description != null && description.Length > 0) {
				pe.Graphics.FillRectangle(new SolidBrush(SystemColors.Info), pe.ClipRectangle);
			}
		}
	}
}
