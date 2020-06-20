﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.Reports.Core.Interfaces;
using ICSharpCode.Reports.Expressions.ReportingLanguage;

namespace ICSharpCode.Reports.Core.BaseClasses.Printing
{
	/// <summary>
	/// Description of StandardPrinter.
	/// </summary>
	
	
	internal sealed class StandardPrinter
	{
		public StandardPrinter()
		{
		}
		
		
		public static void AdjustBackColor (ISimpleContainer container)
		{
			foreach (var item in container.Items)
			{
				item.BackColor = container.BackColor;
			}
		}
		
		
		public static void FillBackground (Graphics  graphics,BaseStyleDecorator decorator)
		{
			if (decorator.BackColor != GlobalValues.DefaultBackColor) {
				
				RectangleShape backgroundShape = new RectangleShape();
				
				backgroundShape.FillShape(graphics,
				                          new SolidFillPattern(decorator.BackColor),
				                          decorator.DisplayRectangle);
			}
		}
		
		
		public static void DrawBorder (Graphics graphics,BaseStyleDecorator decorator)
		{
			if (decorator.DrawBorder)
			{
				if (decorator.FrameColor == Color.Empty)
				{
					decorator.FrameColor = decorator.ForeColor;
				}
				Border border = new Border(new BaseLine (decorator.FrameColor,System.Drawing.Drawing2D.DashStyle.Solid,1));
				border.DrawBorder(graphics,decorator.DisplayRectangle);
			}
		}
		
		
		#region Render/Convert SimpleItem
		
		/// <summary>
		/// Convert a single item, Location is calculated as follows
		/// </summary>
		/// <param name="offset"> only Y value is used, gives the offset to Items location.Y </param>
		/// <param name="item">Item to convert</param>
		/// <returns></returns>
		
		
		private static void RenderLineItem (BaseReportItem item, Point offset,IExpressionEvaluatorFacade evaluator,ReportPageEventArgs rpea)
		{
			
			Point saveLocation = new Point (item.Location.X,item.Location.Y);
			
			PrintHelper.AdjustChildLocation(item,offset);
			
			IReportExpression epr = item as IReportExpression;
			
			string evaluatedValue = String.Empty;
			
			if (epr != null) {
				try {
					if (!String.IsNullOrEmpty(epr.Expression))
					{
						evaluatedValue =  evaluator.Evaluate(epr.Expression);
						
					} else 
					{
						evaluatedValue =  evaluator.Evaluate(epr.Text);
					}
					epr.Text = evaluatedValue;
					
				} catch (UnknownFunctionException ufe) {
					
					epr.Text = GlobalValues.UnkownFunctionMessage(ufe.Message);
				}
				
			}
			item.Render (rpea);
			item.Location = saveLocation;
		}
		
		
		#endregion
		
		
		#region Render Collection
		
		public static Rectangle RenderPlainCollection (BaseReportItem parent,
		                                               ReportItemCollection items,
		                                               IExpressionEvaluatorFacade evaluator,
		                                               Point offset,
		                                               ReportPageEventArgs rpea)
		{
			
			if (items.Count > 0) {
				foreach (BaseReportItem child in items) {
					child.Parent = parent;
					 StandardPrinter.RenderLineItem (child,offset,evaluator,rpea);
				}
			}
			return new Rectangle(offset,parent.Size);
		}
		
		
		#endregion
	
		

		public static Rectangle RenderContainer (ISimpleContainer simpleContainer,IExpressionEvaluatorFacade evaluator,Point offset,ReportPageEventArgs rpea)
		{
			
			BaseReportItem item = simpleContainer as BaseReportItem;
			Rectangle retVal = new Rectangle(offset,item.Size);
			Point saveLocation = item.Location;
			PrintHelper.AdjustChildLocation(item,offset);
			
			item.Render(rpea);
			
			
			if (simpleContainer.Items != null)  {
				retVal = StandardPrinter.RenderPlainCollection(item,simpleContainer.Items,evaluator,offset,rpea);
			}
		
			retVal = new Rectangle (retVal.X,retVal.Y,
		                        retVal.X + item.Size.Width,
		                        item.Size.Height + 3 * GlobalValues.GapBetweenContainer);
			item.Location = saveLocation;
			return retVal;
		}

	}
}
