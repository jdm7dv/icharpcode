// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using ICSharpCode.SharpDevelop.Internal.Project;

namespace VBBinding {
	
	////<summary>
	/// Provides functions to run VB.DOC and to read the configuration of VB.DOC.
	/// </summary>
	public class VBDOCServices
	{
		///<summary>
		/// Returns if the filename will be parsed when running VB.DOC.
		/// </summary>
		public static bool ParseFile(string filename, VBProject project)
		{
			VBCompilerParameters compilerparameters = (VBCompilerParameters)project.ActiveConfiguration;
			return Array.IndexOf(compilerparameters.VBDOCFiles, filename) == -1;
		}
		
		///<summary>
		/// Runs VB.DOC for the given project
		/// </summary>
		public static bool RunVBDOC(IProject project, string assembly)
		{
			VBProject p = (VBProject)project;
			VBCompilerParameters compilerparameters = (VBCompilerParameters)p.ActiveConfiguration;
			ArrayList files = new ArrayList();
			
			if(compilerparameters.EnableVBDOC == true) {
				if(File.Exists(assembly)) {
					foreach(ProjectFile pfile in project.ProjectFiles) {
						if(ParseFile(pfile.Name, p)) {
							files.Add(pfile.Name);
						}
					}
					
					DocGeneration docgen = new DocGeneration();
					docgen.Destination = compilerparameters.VBDOCOutputFile;
					docgen.RootNamespace = compilerparameters.RootNamespace;
					docgen.AssemblyFile = assembly;
					
					try {
						docgen.ParseFiles(files);
					} catch(Exception err) {
						System.Windows.Forms.MessageBox.Show("An error occured when running VB.DOC " + err.Message);
						return false;
					}
					
					return true;
				} else {
					return false;
				}
			} else {
				return true;
			}
		}
	}
}
