// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace ICSharpCode.SharpDevelop.FormDesigner.Services
{
	public class PropertyValueUIService : IPropertyValueUIService
	{
		PropertyValueUIHandler propertyValueUIHandler;
		
		public void AddPropertyValueUIHandler(PropertyValueUIHandler newHandler)
		{
			propertyValueUIHandler += newHandler;
		}
		
		public PropertyValueUIItem[] GetPropertyUIValueItems(ITypeDescriptorContext context, PropertyDescriptor propDesc)
		{
			return new PropertyValueUIItem[0];
		}
		
		public void NotifyPropertyValueUIItemsChanged()
		{
			OnPropertyUIValueItemsChanged(EventArgs.Empty);
		}
		
		public void RemovePropertyValueUIHandler(PropertyValueUIHandler newHandler)
		{
			propertyValueUIHandler -= newHandler;
		}
		
		protected virtual void OnPropertyUIValueItemsChanged(EventArgs e)
		{
			if (PropertyUIValueItemsChanged != null) {
				PropertyUIValueItemsChanged(this, e);
			}
		}
		
		public event EventHandler PropertyUIValueItemsChanged;
	}
}
