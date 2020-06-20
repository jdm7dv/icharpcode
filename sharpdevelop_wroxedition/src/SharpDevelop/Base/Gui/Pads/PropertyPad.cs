// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class PropertyPad : AbstractPadContent
	{
		static PropertyGrid        grid = null;
		static IDesignerHost host = null;
		
		public override Control Control {
			get {
				return grid;
			}
		}
		
		public PropertyPad() : base("${res:MainWindow.Windows.PropertiesScoutLabel}", "Icons.16x16.PropertiesIcon")
		{
			grid = new PropertyGrid();
			grid.PropertyValueChanged += new PropertyValueChangedEventHandler(PropertyChanged);
		}
		
		public static void SetDesignableObject(object obj)
		{
			grid.SelectedObjects = null;
			grid.SelectedObject  = obj;
		}
		
		public static void SetDesignerHost(IDesignerHost host)
		{
			PropertyPad.host = host;
			ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
			
			if (selectionService != null) {
				selectionService.SelectionChanging += new EventHandler(SelectionChangingHandler);
				selectionService.SelectionChanged  += new EventHandler(SelectionChangedHandler);
			}
		}
		
		public static void SelectionChangingHandler(object sender, EventArgs args)
		{
		}

		public static void SelectionChangedHandler(object sender, EventArgs args)
		{
			ISelectionService selectionService = sender as ISelectionService;
			
			if (selectionService != null) {
				ICollection selection = selectionService.GetSelectedComponents();
				if (selection.Count > 0) {
					object[] selArray = new object[selection.Count];
					selection.CopyTo(selArray, 0);
					grid.SelectedObjects = selArray;
				} else {
					grid.SelectedObjects = new object[0];
					grid.SelectedObject = null;
				}
			}
		}
		
		void PropertyChanged(object sender, PropertyValueChangedEventArgs e)
		{
			if (host != null) {
				DesignerTransaction transaction = host.CreateTransaction("Property Changed");
				transaction.Commit();
			}
		}
	}
}
