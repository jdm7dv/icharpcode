// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml;

using ICSharpCode.SharpDevelop.Internal.Project;

namespace VBBinding
{
	/// <summary>
	/// This class describes a VB.NET project and it compilation options.
	/// </summary>
	public class VBProject : AbstractProject
	{		
		public override string ProjectType {
			get {
				return VBLanguageBinding.LanguageName;
			}
		}
		
		public override IConfiguration CreateConfiguration()
		{
			return new VBCompilerParameters();
		}
		
		public VBProject()
		{
		}
		
		public VBProject(ProjectCreateInformation info)
		{
			if (info != null) {
				Description       = info.Description;
				Name              = info.Name;
				
				Configurations.Add(CreateConfiguration("Debug"));
				Configurations.Add(CreateConfiguration("Release"));

				XmlElement el = info.ProjectTemplate.ProjectOptions;
				
				foreach (VBCompilerParameters parameter in Configurations) {
					parameter.OutputDirectory = info.Location;
					parameter.OutputAssembly  = Name;
					
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
