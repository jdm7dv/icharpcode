// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.ComponentModel.Design;

namespace ICSharpCode.SharpDevelop.FormDesigner.Services
{
	public class DesignerOptionService : IDesignerOptionService
	{
		public const string GridSize   = "GridSize";
		public const string ShowGrid   = "ShowGrid";
		public const string SnapToGrid = "SnapToGrid";
		
		const string GridSizeWidth  = "GridSize.Width";
		const string GridSizeHeight = "GridSize.Height";
		
		public const string FormsDesignerPageName = "SharpDevelop Forms Designer\\General";
		
		Hashtable pageOptionTable = new Hashtable();
		
		public DesignerOptionService()
		{
			Hashtable defaultTable = new Hashtable();
			
			defaultTable[GridSize]   = new Size(8, 8);
			defaultTable[ShowGrid]   = false;
			defaultTable[SnapToGrid] = false;
			
			pageOptionTable[FormsDesignerPageName] = defaultTable;
		}
		
		public object GetOptionValue(string pageName, string valueName)
		{
			Hashtable pageTable = (Hashtable)pageOptionTable[pageName];
			
			if (pageTable == null) {
				return null;
			}
			
			switch (valueName) {
				case GridSizeWidth:
					return ((Size)pageTable[GridSize]).Width;
				case GridSizeHeight:
					return ((Size)pageTable[GridSize]).Height;
				default:
					return pageTable[valueName];
			}
		}
		
		public void SetOptionValue(string pageName, string valueName, object val)
		{
			Hashtable pageTable = (Hashtable)pageOptionTable[pageName];
			
			if (pageTable == null) {
				pageOptionTable[pageName] = pageTable = new Hashtable();
			}
			
			switch (valueName) {
				case GridSizeWidth:
					Size size      = ((Size)pageTable[GridSize]);
					size.Width     = (int)val;
					pageTable[GridSize] = size;
					break;
				case GridSizeHeight:
					size           = ((Size)pageTable[GridSize]);
					size.Height    = (int)val;
					pageTable[GridSize] = size;
					break;
				default:
					pageTable[valueName] = val;
					break;
			}
		}
	}
}
