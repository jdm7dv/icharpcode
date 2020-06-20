// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.ComponentModel.Design.Serialization;

namespace ICSharpCode.SharpDevelop.FormDesigner.Hosts
{
	public class DefaultDesignerLoader : DesignerLoader
	{
		bool                isLoading = false;
		IDesignerLoaderHost myHost    = null;
		
		public DefaultDesignerLoader()
		{
		}
		
		public override bool Loading {
			get {
				return isLoading;
			}
		}
		
		public override void BeginLoad(IDesignerLoaderHost host)
		{
			isLoading = true;
			myHost    = host; 
			
			// Perform the load...
			host.EndLoad("BaseClassName", false, null);
			isLoading = false;
		}
		
		public override void Dispose()
		{
		}
		
		public override void Flush()
		{
			base.Flush();
		}
	}
}
