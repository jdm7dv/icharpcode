// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.Services;

namespace ResEdit
{
	/// <summary>
	/// This class displays a bitmap in a window, the window and the bitmap can be resized.
	/// </summary>
	class BitmapView : Form
	{
		Bitmap bitmap;
		Font   font;
		
		public BitmapView(string text, Bitmap bitmap)
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			font = resourceService.LoadFont("Tahoma", 8);

			this.Text   = text;
			this.bitmap = bitmap;
			
			ClientSize    = new Size(bitmap.Size.Width + 100, bitmap.Size.Height + 60);
			StartPosition = FormStartPosition.CenterScreen;
			TopMost       = true;
			Owner         = (Form)WorkbenchSingleton.Workbench;
		}
		protected override void OnResize(EventArgs e)
		{
			Refresh();
		}
		
		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			
			// calculate zoom factor 
			int BitmapWitdh  = Width - 50;
			int BitmapHeight = Height - 50;
			
			float factor1 = (float)BitmapWitdh / bitmap.Width;
			float factor2 = (float)BitmapHeight / bitmap.Height;
			float factor  = Math.Min(factor1, factor2); // always take the minimum zoom factor -> zoomed bitmap fits in the window
			BitmapWitdh  = (int)factor * bitmap.Width;
			BitmapHeight = (int)factor * bitmap.Height;
			
			g.InterpolationMode = InterpolationMode.NearestNeighbor; // Interpolation doesn't look nice for icons, so I turn it off.
			
			// draw white window background
			g.FillRectangle(new SolidBrush(Color.White), e.ClipRectangle);
			
			// calculate bitmap position
			Point p = new Point((ClientSize.Width  - BitmapWitdh) / 2, 20);
			
			// draw "transparent" color (transparent pixels are DarkCyan)
			g.FillRectangle(new SolidBrush(Color.DarkCyan), p.X, p.Y, BitmapWitdh, BitmapHeight);
			
			// draw Image
			g.DrawImage(bitmap, p.X, p.Y, BitmapWitdh, BitmapHeight);
			
			// draw Image Border
			g.DrawRectangle(new Pen(Color.Black, 1), p.X - 1, p.Y - 1, BitmapWitdh + 1, BitmapHeight + 1);
			
			// draw Size
			g.DrawString(bitmap.Size.ToString(), 
				         font, 
				         new SolidBrush(Color.Black), 0, 0);
			
		}
	}
}
