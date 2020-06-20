// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace ICSharpCode.SharpDevelop.FormDesigner.Services
{
	public class TypeDescriptorFilterService : ITypeDescriptorFilterService
	{
		IDesignerFilter GetDesignerFilter(IComponent component)
		{
			ISite site = component.Site;
			
			if (site == null) {
				return null;
			}
			
			IDesignerHost host =  site.GetService(typeof(IDesignerHost)) as IDesignerHost;
			
			if (host == null) {
				return null;
			}
			
			IDesigner designer = host.GetDesigner(component);

			return designer as IDesignerFilter;
		}
		
		public bool FilterAttributes(IComponent component, IDictionary attributes)
		{
			IDesignerFilter designerFilter = GetDesignerFilter(component);
			if (designerFilter != null) {
				designerFilter.PreFilterAttributes(attributes);
				designerFilter.PostFilterAttributes(attributes);
			}
			return false;
		}

		public bool FilterEvents(IComponent component, IDictionary events)
		{
			IDesignerFilter designerFilter = GetDesignerFilter(component);
			if (designerFilter != null) {
				designerFilter.PreFilterEvents(events);
				designerFilter.PostFilterEvents(events);
			}
			return false;
		}
		
		public bool FilterProperties(IComponent component, IDictionary properties)
		{
			IDesignerFilter designerFilter = GetDesignerFilter(component);
			if (designerFilter != null) {
				designerFilter.PreFilterProperties(properties);
				designerFilter.PostFilterProperties(properties);
			}
			return false;
		}
	}
}
