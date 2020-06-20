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

namespace JScriptBinding
{
	public class JScriptProject : AbstractProject
	{
		public override string ProjectType {
			get {
				return JScriptLanguageBinding.LanguageName;
			}
		}
		
		public override IConfiguration CreateConfiguration()
		{
			return new JScriptCompilerParameters();
		}
		
		public JScriptProject()
		{
		}
		
		public JScriptProject(ProjectCreateInformation info)
		{
			if (info != null) {
				Description       = info.Description;
				Name              = info.Name;
				
				Configurations.Add(CreateConfiguration("Debug"));
				Configurations.Add(CreateConfiguration("Release"));
				
				foreach (JScriptCompilerParameters parameter in Configurations) {
					parameter.OutputDirectory = info.Location;
					parameter.OutputAssembly  = Name;
				}
			}
		}
	}
}
