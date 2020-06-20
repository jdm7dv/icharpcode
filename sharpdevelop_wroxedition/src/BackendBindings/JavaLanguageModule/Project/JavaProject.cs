// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml;

using ICSharpCode.SharpDevelop.Internal.Project;

namespace JavaBinding
{
	
	/// <summary>
	/// This class describes a C Sharp project and it compilation options.
	/// </summary>
	public class JavaProject : AbstractProject
	{		
		public override string ProjectType {
			get {
				return JavaLanguageBinding.LanguageName;
			}
		}
		
		public override IConfiguration CreateConfiguration()
		{
			return new JavaCompilerParameters();
		}
		
		public JavaProject()
		{
		}
		
		public JavaProject(ProjectCreateInformation info)
		{
			if (info != null) {
				Description       = info.Description;
				Name              = info.Name;
				
				Configurations.Add(CreateConfiguration("Debug"));
				Configurations.Add(CreateConfiguration("Release"));
				
				XmlElement el = info.ProjectTemplate.ProjectOptions;
				
				foreach (JavaCompilerParameters parameter in Configurations) {
					parameter.OutputDirectory = info.Location;
					parameter.OutputAssembly  = Name;
					
					if (el != null) {
						if (el.Attributes["MainClass"] != null) {
							parameter.MainClass = el.Attributes["MainClass"].InnerText;
						}
						if (el.Attributes["PauseConsoleOutput"] != null) {
							parameter.PauseConsoleOutput = Boolean.Parse(el.Attributes["PauseConsoleOutput"].InnerText);
						}
						if (el.Attributes["ClassPath"] != null) {
							parameter.ClassPath = el.Attributes["ClassPath"].InnerText;
						}
					}
				}
			}
		}
	}
}
