// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace ICSharpCode.SharpDevelop.FormDesigner.Services
{
	public class ExtenderProviderService : IExtenderProviderService
	{
		ArrayList extenderProviders = new ArrayList();
		
		public ArrayList ExtenderProviders {
			get {
				return extenderProviders;
			}
		}
		
		public void AddExtenderProvider(IExtenderProvider provider)
		{
			extenderProviders.Add(provider);
		}
		
		public void RemoveExtenderProvider(IExtenderProvider provider)
		{
			extenderProviders.Remove(provider);
		}
	}
}
