﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;

namespace ICSharpCode.Reports.Core {	
	/// <summary>
	/// Abstract BaseClass for all AbstractFillPatterns
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Peter
	/// 	created on - 12.10.2005 08:57:05
	/// </remarks>
	
	public abstract class AbstractFillPattern
	{
		
		Color color;
		Brush brush;
		iTextSharp.text.BaseColor pdfColor;
		
		protected AbstractFillPattern(Color color) {
			this.color = color;
		}
		
		
		protected AbstractFillPattern(iTextSharp.text.BaseColor pdfColor)
		{
			this.pdfColor = pdfColor;
		}
		
		
		public abstract Brush CreateBrush(RectangleF rect);

		public Color Color {
			get {
				return color;
			}
			set {
				color = value;
			}
		}
		
		public iTextSharp.text.BaseColor PdfColor {
			get { return pdfColor; }
			set { pdfColor = value; }
		}
		
		public Brush Brush {
			get {
				return brush;
			}
			set {
				brush = value;
			}
		}
		
	}
	
	/// <summary>
	/// Solid Fill Pattern
	/// </summary>
	public class SolidFillPattern : AbstractFillPattern {
		
		public SolidFillPattern (Color color) :base(color){
		}
		
		public SolidFillPattern (iTextSharp.text.BaseColor pdfColor):base(pdfColor)
		{
			
		}
		
		public override Brush CreateBrush(RectangleF rect){
			return new SolidBrush(this.Color);
		}
		
	}
}
