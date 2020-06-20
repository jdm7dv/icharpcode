// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.FormDesigner.Services;

namespace ICSharpCode.SharpDevelop.FormDesigner.Hosts
{
	public class DesignComponentContainer : IContainer
	{
		DefaultDesignerHost host          = null;
		
		HybridDictionary    components    = new HybridDictionary();
		HybridDictionary    designers     = new HybridDictionary();
		
		IComponent          rootComponent = null;
		
		int unnamedCount = 0;
		
		public DesignComponentContainer(DefaultDesignerHost host)
		{
			this.host = host;
			
			ComponentChangeService componentChangeService = componentChangeService = host.GetService(typeof(IComponentChangeService)) as ComponentChangeService;
			if (componentChangeService != null) {
				componentChangeService.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
			}
		}
		
		void OnComponentRename(object sender, ComponentRenameEventArgs e)
		{
			components.Remove(e.OldName);
			components.Add(e.NewName, e.Component);
		}
		
		
		public IDictionary Designers {
			get {
				return designers;
			}
		}
		
		public IComponent RootComponent {
			get {
				return rootComponent;
			}
		}
		
		public ComponentCollection Components {
			get {
				IComponent[] datalist = new IComponent[components.Count];
				components.Values.CopyTo(datalist, 0);
				return new ComponentCollection(datalist);
			}
		}
		
		public void Dispose()
		{
			foreach (IComponent component in components.Values) {
				component.Dispose();
			}
			components.Clear();
		}

		public bool ContainsName(string name)
		{
			return components.Contains(name);
		}
		
		public void Add(IComponent component, string name)
		{
			if (name == null) {
				name = unnamedCount + "_unnamed";
				++unnamedCount;
			}
			
			if (ContainsName(name)) {
				throw new ArgumentException("name", "A component named " + name + " already exists in this container");
			}
			
			ISite site = new ComponentSite(host, component);
			site.Name = name;
			component.Site = site;
			
			ComponentChangeService componentChangeService = host.GetService(typeof(IComponentChangeService)) as ComponentChangeService;
			
			if (componentChangeService != null) {
				componentChangeService.OnComponentAdding(component);
			}
			
			IDesigner designer = null;
			
			if (components.Count == 0) {
				// this is the first component. It must be the
				// "root" component and therefore it must offer
				// a root designer
				designer = TypeDescriptor.CreateDesigner(component, typeof(IRootDesigner));
				rootComponent = component;
			} else {
				designer = TypeDescriptor.CreateDesigner(component, typeof(IDesigner));
			}
			
			// If we got a designer, initialize it
			if (designer != null) {
				designer.Initialize(component);
				designers[component] = designer;
			}

			components.Add(component.Site.Name, component);
			if (componentChangeService != null) {
				componentChangeService.OnComponentAdded(component);
			}
		}
		
		public void Add(IComponent component)
		{
			this.Add(component, null);
		}
		
		public void Remove(IComponent component)
		{
			string name = null;
			ISite site  = component.Site;
			
			if (site != null) {
				name = site.Name;
			} else {
				foreach (string k in components.Keys) {
					IComponent c = components[k] as IComponent;
					if (c == component) {
						name = k;
					}
				}
			}
			
			if (name == null) {
				return;
			}
			
//			ComponentChangeService componentChangeService = componentChangeService = host.GetService(typeof(IComponentChangeService)) as ComponentChangeService;
//			if (componentChangeService != null) {
//				componentChangeService.OnComponentRemoving(component);
//			}
			
			if (components.Contains(name)) {
				components.Remove(name);
				if (site != null) {
					component.Site = null;
				}
				IDesigner designer = designers[component] as IDesigner;
				
				if (designer != null) {
					designer.Dispose();
					designers.Remove(component);
				}
			}
			
//			if (componentChangeService != null) {
//				componentChangeService.OnComponentRemoved(component);
//			}
		}
		
		// ISite implementation
		class ComponentSite : ISite
		{
			IComponent               component;
			IDesignerHost            host;
			bool                     isInDesignMode;
			string                   name;
			
			public ComponentSite(IDesignerHost host, IComponent component)
			{
				this.component      = component;
				this.host           = host;
				this.isInDesignMode = true;
			}
				
			public IComponent Component {
				get {
					return component;
				}
			}
			
			public IContainer Container {
				get {
					return host.Container;
				}
			}
			
			public bool DesignMode {
				get {
					return isInDesignMode;
				}
			}
			
			public string Name {
				get {
					return name;
				}
				set {
					Control nameable = component as Control;
					if (nameable != null) {
						nameable.Name = value;
					}
					name = value;
				}
			}
			
			public object GetService(Type serviceType)
			{
				return host.GetService(serviceType);
			}
		}
	}
}
