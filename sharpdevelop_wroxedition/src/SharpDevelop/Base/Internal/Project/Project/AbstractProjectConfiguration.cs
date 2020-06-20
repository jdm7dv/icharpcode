// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml;

using ICSharpCode.SharpDevelop.Internal.Project;

namespace ICSharpCode.SharpDevelop.Internal.Project
{
	/// <summary>
	/// External language bindings may choose to extend this class.
	/// It makes things a bit easier.
	/// </summary>
	[XmlNodeNameAttribute("Configuration")]
	public abstract class AbstractProjectConfiguration : AbstractConfiguration
	{
		[XmlNodeName("Output")]
		protected class OutputConfiguration
		{
			[XmlAttribute("directory")]
			[ConvertToRelativePath()]
			public string Directory = "." + Path.DirectorySeparatorChar.ToString();
			
			[XmlAttribute("assembly")]
			public string Assembly = "a";
		}
		
		[XmlAttribute("runwithwarnings")]
		protected bool runWithWarnings = false;
		
		protected OutputConfiguration outputConfiguration = new OutputConfiguration();
		
		public virtual string OutputDirectory {
			get {
				return outputConfiguration.Directory;
			}
			set {
				outputConfiguration.Directory = value;
			}
		}
		
		public virtual string OutputAssembly {
			get {
				return outputConfiguration.Assembly;
			}
			set {
				outputConfiguration.Assembly = value;
			}
		}
		
		public virtual bool RunWithWarnings {
			get {
				return runWithWarnings;
			}
			set {
				runWithWarnings = value;
			}
		}
		
		public AbstractProjectConfiguration()
		{
		}
	}
}
