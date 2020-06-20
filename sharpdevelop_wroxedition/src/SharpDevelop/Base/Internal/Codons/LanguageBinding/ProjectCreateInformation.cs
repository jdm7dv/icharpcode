// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System.Collections;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Internal.Project
{
	/// <summary>
	/// This class holds all information the language binding need to create
	/// a predefined project for their language, if no project template for a 
	/// specific language is avaiable, the language binding shouldn't care about
	/// this stuff.
	/// </summary>
	public class ProjectCreateInformation
	{
		string name;
		string solution;
		string location;
		string description; // not yet used
		
		ProjectTemplate projectTemplate;
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public string Solution {
			get {
				return solution;
			}
			set {
				solution = value;
			}
		}

		public string Location {
			get {
				return location;
			}
			set {
				location = value;
			}
		} 
		
		public string Description {
			get {
				return description;
			}
			set {
				description = value;
			}
		}

		public ProjectTemplate ProjectTemplate {
			get {
				return projectTemplate;
			}
			set {
				projectTemplate = value;
			}  
		}
	}
}
