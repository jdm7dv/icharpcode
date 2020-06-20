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

namespace CSharpBinding
{
	/// <summary>
	/// This class describes a C Sharp project and it compilation options.
	/// </summary>
	public class CSharpProject : AbstractProject
	{		
		public override string ProjectType {
			get {
				return CSharpLanguageBinding.LanguageName;
			}
		}
		
		public CSharpProject()
		{
		}
		
		public override IConfiguration CreateConfiguration()
		{
			return new CSharpCompilerParameters();
		}
		
		public CSharpProject(ProjectCreateInformation info)
		{
			if (info != null) {
				Description = info.Description;
				Name = info.Name;
				Configurations.Add(CreateConfiguration("Debug"));
				Configurations.Add(CreateConfiguration("Release"));
				
				XmlElement el = info.ProjectTemplate.ProjectOptions;
				
				foreach (CSharpCompilerParameters parameter in Configurations) {
					parameter.OutputDirectory = info.Location;
					parameter.OutputAssembly = Name;
					//}
					//XmlElement el = info.ProjectTemplate.ProjectOptions; -- moved above foreach loop
					//para variable renamed parameter
					//CSharpCompilerParameters para = ActiveConfiguration;?? - removed as nolonger needed
					if (el != null) {
						if (el.Attributes["Target"] != null) {
							parameter.CompileTarget = (CompileTarget)Enum.Parse(typeof(CompileTarget), el.Attributes["Target"].InnerText);
						}
						if (el.Attributes["PauseConsoleOutput"] != null) {
							parameter.PauseConsoleOutput = Boolean.Parse(el.Attributes["PauseConsoleOutput"].InnerText);
						}
					}
				}
			}
		}
	}
}
