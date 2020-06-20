// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Specialized;

using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui.Components;

namespace ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser
{
	/// <summary>
	/// This class represents a named folder. A named folder gets a localized name
    /// out of the resourceService instead directly.
	/// </summary>
	public class NamedFolderNode : FolderNode 
	{
		string resourceReference;
		
		public NamedFolderNode(string resourceReference) : base(((ResourceService)ServiceManager.Services.GetService(typeof(ResourceService))).GetString(resourceReference))
		{
			this.resourceReference = resourceReference;
		}
		
		public override void UpdateNaming()
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
			
			Text = resourceService.GetString(resourceReference);
			base.UpdateNaming();
		}
	}
}
