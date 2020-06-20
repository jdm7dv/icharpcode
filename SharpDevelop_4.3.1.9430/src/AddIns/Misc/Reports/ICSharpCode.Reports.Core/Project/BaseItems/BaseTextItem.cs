﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Exporter;

/// <summary>
/// This class is the BaseClass  for all TextBased Items 
/// like <see cref="BaseDataItem"></see> etc.
/// </summary>


namespace ICSharpCode.Reports.Core
{
	public class BaseTextItem : BaseReportItem,IExportColumnBuilder,IReportExpression {

		private string text;
		private string dataType;
		private string formatString;
		private StringFormat stringFormat;
		private StringTrimming stringTrimming;
		private ContentAlignment contentAlignment;

		
		#region Constructor
		
		public BaseTextItem():base() {
			this.dataType = "System.String";
			this.stringFormat = StringFormat.GenericTypographic;
			this.contentAlignment = ContentAlignment.TopLeft;
			this.stringTrimming = StringTrimming.None;
//			VisibleInReport = true;
		}
		
		#endregion
		
		#region IExportColumnBuilder  implementation
		
		public virtual IBaseExportColumn CreateExportColumn(){
			TextStyleDecorator st = this.CreateItemStyle();
			ExportText item = new ExportText(st);
			item.Text = this.text;
			item.Expression = this.Expression;
			return item;
		}
		
		
		protected TextStyleDecorator CreateItemStyle () {
			
			TextStyleDecorator style = new TextStyleDecorator();
			
			style.BackColor = this.BackColor;
			style.FrameColor = this.FrameColor;
			style.ForeColor = this.ForeColor;
			
			style.Font = this.Font;
			
			style.Location = this.Location;
			style.Size = this.Size;
			style.DrawBorder = this.DrawBorder;
			style.DisplayRectangle = this.DisplayRectangle;
			
			style.StringFormat = this.stringFormat;
			style.StringTrimming = this.stringTrimming;
			style.ContentAlignment = this.contentAlignment;
			style.FormatString = this.formatString;
			style.DataType = this.dataType;
			style.RightToLeft = this.RTL;
			
			return style;
		}

		#endregion
		
		
		public override void Render(ReportPageEventArgs rpea) {
			
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			
			base.Render(rpea);
			
			StandardPrinter.FillBackground(rpea.PrintPageEventArgs.Graphics,this.BaseStyleDecorator);
			
			StandardPrinter.DrawBorder(rpea.PrintPageEventArgs.Graphics,this.BaseStyleDecorator);
			
			string formated = StandardFormatter.FormatOutput(this.text,this.FormatString,this.DataType,String.Empty);
		
			Print (rpea,formated,base.DisplayRectangle);
		
			base.NotifyAfterPrint (rpea.LocationAfterDraw);
		}
		
		
		public override string ToString() {
			return "BaseTextItem";
		}
		
		
		/// <summary>
		/// Standart Function to Draw Strings
		/// </summary>
		/// <param name="e">ReportpageEventArgs</param>
		/// <param name="toPrint">Formatted String toprint</param>
		/// <param name="rectangle">rectangle where to draw the string</param>
	
		protected void Print (ReportPageEventArgs rpea,
		                              string toPrint,
		                              RectangleF rectangle ) {
			
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			
			TextDrawer.DrawString(rpea.PrintPageEventArgs.Graphics,
			                      toPrint,this.Font,
			                      new SolidBrush(this.ForeColor),
			                      rectangle,
			                      this.StringFormat);
			
			                      
			rpea.LocationAfterDraw = new Point (this.Location.X + this.Size.Width,
			                                    this.Location.Y + this.Size.Height);
		}
	
		
		public virtual string Text {
			get {
				return text;
			}
			set {
				text = value;
			}
		}
		
		public string DataType 
		{
			get {
				if (String.IsNullOrEmpty(this.dataType)) {
					this.dataType = typeof(System.String).ToString();
				}
				return dataType;
			}
			set {
				dataType = value;
			}
		}
		
		
		
		public virtual string FormatString {
			get {
				return formatString;
			}
			set {
				formatString = value;
			}
		}
		
		
		public  StringTrimming StringTrimming {
			get {
				return stringTrimming;
			}
			set {
				stringTrimming = value;
			}
		}
		
		
		public virtual System.Drawing.ContentAlignment ContentAlignment {
			get {
				return this.contentAlignment;
			}
			set {
				this.contentAlignment = value;
			}
		}
	

		public virtual StringFormat StringFormat {
			get {
				var sf = TextDrawer.BuildStringFormat (this.StringTrimming,this.ContentAlignment);
				if (this.RTL == System.Windows.Forms.RightToLeft.Yes) {
					sf.FormatFlags = sf.FormatFlags | StringFormatFlags.DirectionRightToLeft;
				}
				return sf;
			}
		}
		
		
		public System.Windows.Forms.RightToLeft RTL {get;set;}
		
		#region IExpression
		
		public string Expression {get;set;}
		
		#endregion
		
	}
}
