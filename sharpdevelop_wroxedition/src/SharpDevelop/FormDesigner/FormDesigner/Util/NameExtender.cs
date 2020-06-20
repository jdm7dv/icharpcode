// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using ICSharpCode.SharpDevelop.FormDesigner.Services;

namespace ICSharpCode.SharpDevelop.FormDesigner.Util
{
	[ProvideProperty("Name", typeof(IComponent))]
	public class NameExtender : IExtenderProvider
	{
		public NameExtender()
		{
		}
		
		public bool CanExtend(object extendee)
		{
			//System.Console.WriteLine("[NameExtender:CanExtend] " + extendee);
			return (extendee is IComponent);
		}
		
		[DesignOnly(true)]
		[Category("Design")]
		[Browsable(true)]
		[ParenthesizePropertyName(true)]
		[Description("The variable used to refer to this component in source code.")]
		public string GetName(IComponent component)
		{
			ISite site = component.Site;
			if(site != null)
				return site.Name;
			else
				return null;
		}
		
		// Fixme: Probably need to ensure that the name is
		// unique in the container
		public void SetName(IComponent component, string name)
		{
			ISite site = component.Site;
			string oldName = site.Name;
			
			// If the name is in use, don't go any farther
			ComponentCollection comCol = site.Container.Components;
			foreach(IComponent c in comCol)
			{
				if(component == c)
					continue;
				
				ISite s = c.Site;
				if(s.Name == name)
				{
					throw new ArgumentException("name", "A component named "
						+ name + " already exists in the container");
				}
			}
			
			IComponentChangeService service = 
				site.GetService(typeof(IComponentChangeService)) 
				as IComponentChangeService;
			
			MemberDescriptor md = TypeDescriptor.CreateProperty(
				component.GetType(),
				"Name",
				typeof(string),
				new Attribute[0]
			);
			
			if(service != null)
			{
				service.OnComponentChanging(component, md);
			}
			
			if(site != null)
				site.Name = name;
			
			if(service != null)
			{
				service.OnComponentChanged(component, md, oldName, name);
				((ComponentChangeService)service).OnComponentRename(
					component, oldName, name);
			}
		}
	}
}
