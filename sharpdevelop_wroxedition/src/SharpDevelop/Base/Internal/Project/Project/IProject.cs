// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System.Collections;
using System.Xml;
using ICSharpCode.SharpDevelop.Internal.Project.Collections;

namespace ICSharpCode.SharpDevelop.Internal.Project
{
	public enum NewFileSearch {
		None,
		OnLoad,
		OnLoadAutoInsert
	}
	
	/// <summary>
	/// This interface describes the whole functionalaty a SharpDevelop project has.
	/// </summary>
	public interface IProject
	{
		/// <summary>
		/// Returns the language codon name, which is used
		/// to compile this project.
		/// </summary>
		string ProjectType {
			get;
		}
		
		/// <summary>
		/// Gets the directory where the projectfile is located.
		/// </summary>
		string BaseDirectory {
			get;
		}
		
		/// <summary>
		/// The name of the project.
		/// </summary>
		string Name {
			get;
			set;
		}
		
		/// <summary>
		/// The description of the project.
		/// </summary>
		string Description {
			get;
			set;
		}
		
		/// <summary>
		/// Gets/Sets the active configuration.
		/// </summary>
		IConfiguration ActiveConfiguration {
			get;
			set;
		}
		
		/// <summary>
		/// Gets the arraylist which contains all project configurations.
		/// </summary>
		ArrayList Configurations {
			get;
		}
		
		/// <summary>
		/// A collection containing all files & empty directories in the project.
		/// </summary>
		ProjectFileCollection ProjectFiles {
			get;
		}
		
		/// <summary>
		/// A collection containing all references in the project.
		/// </summary>
		ProjectReferenceCollection ProjectReferences {
			get;
		}
		
		/// <summary>
		/// The method on how newly found files (files which are in the project subdir
		/// but not in the project) are handled.
		/// </summary>
		NewFileSearch NewFileSearch {
			get;
			set;
		}
		
		/// <summary>
		/// If this property is true the state of the workbench (open files, project/class scout
		/// state) should be made persistent
		/// </summary>
		bool EnableViewState {
			get;
			set;
		}
		
		
		DeployInformation DeployInformation {
			get;
		}
		
		/// <summary>
		/// Returns true, if the language module which defined this project is able
		/// to compile the file 'fileName'. e.g. returns true, if the file fileName
		/// could be added as compileable file to the project.
		/// </summary>
		bool IsCompileable(string fileName);
		
		/// <summary>
		/// Loads this Project from fileName
		/// </summary>
		void LoadProject(string fileName);
		
		/// <summary>
		/// Saves this Project under fileName
		/// </summary>
		void SaveProject(string fileName);
		
		/// <summary>
		/// Returns true, if a specific file (given by it's name) 
		/// is inside this project.
		/// </summary>
		bool IsFileInProject(string fileName);
		
		/// <summary>
		/// Creates a IConfiguration object which is used by this project type.
		/// </summary>
		IConfiguration CreateConfiguration(string name);
		
		/// <summary>
		/// Creates a IConfiguration object which is used by this project type.
		/// </summary>
		IConfiguration CreateConfiguration();
	}
}
