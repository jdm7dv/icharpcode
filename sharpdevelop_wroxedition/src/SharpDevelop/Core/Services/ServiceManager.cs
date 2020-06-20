// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.Resources;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Properties;

namespace ICSharpCode.Core.Services
{
	/// <summary>
	/// This class does basic service handling for you.
	/// </summary>
	public class ServiceManager
	{
		ArrayList serviceList       = new ArrayList();
		Hashtable servicesHashtable = new Hashtable();
		
		static ServiceManager defaultServiceManager = new ServiceManager();
		
		/// <summary>
		/// Gets the default ServiceManager
		/// </summary>
		public static ServiceManager Services {
			get {
				return defaultServiceManager;
			}
		}		
		
		/// <summary>
		/// Don't create ServiceManager objects, only have ONE per application.
		/// </summary>
		private ServiceManager()
		{
		}
		
		/// <remarks>
		/// This method initializes the service system to a path inside the add-in tree.
		/// This method must be called ONCE.
		/// </remarks>
		public void InitializeServicesSubsystem(string servicesPath)
		{
			// add 'core' services
			AddService(new PropertyService());
			AddService(new ResourceService());
			AddService(new StringParserService());
			AddService(new FileUtilityService());
			
			// add add-in tree services
			AddServices((IService[])AddInTreeSingleton.AddInTree.GetTreeNode(servicesPath).BuildChildItems(this).ToArray(typeof(IService)));
			
			// initialize all services
			foreach (IService service in serviceList) {
				service.InitializeService();
			}
		}
		
		/// <remarks>
		/// Calls UnloadService on all services. This method must be called ONCE.
		/// </remarks>
		public void UnloadAllServices()
		{
			foreach (IService service in serviceList) {
				service.UnloadService();
			}
		}
		
		protected void AddService(IService service)
		{
			serviceList.Add(service);
		}
		
		protected void AddServices(IService[] services)
		{
			foreach (IService service in services) {
				AddService(service);
			}
		}
		
		/// <remarks>
		/// Requestes a specific service, may return null if this service is not found.
		/// </remarks>
		public IService GetService(Type serviceType)
		{
			IService s = (IService)servicesHashtable[serviceType];
			if (s != null) {
				return s;
			}
			
			foreach (IService service in serviceList) {
				if (serviceType.IsInstanceOfType(service)) {
					servicesHashtable[serviceType] = service;
					return service;
				}
			}
			
			return null;
		}
	}
}
