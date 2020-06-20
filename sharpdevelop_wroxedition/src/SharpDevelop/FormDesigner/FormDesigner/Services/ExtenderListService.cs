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
	
	public class ExtenderListService : IExtenderListService
	{
		ArrayList extenderProviders = new ArrayList();
		
		public ArrayList ExtenderProviders {
			get {
				return extenderProviders;
			}
		}
		
		public ExtenderListService()
		{
		}
		
		public IExtenderProvider[] GetExtenderProviders()
		{
			IExtenderProvider[] extenderProvidersArray = new IExtenderProvider[extenderProviders.Count];
			extenderProviders.CopyTo(extenderProvidersArray, 0);
			return extenderProvidersArray;
		}
	}
}
