// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using ICSharpCode.SharpDevelop.Internal.Project;

namespace ICSharpCode.SharpDevelop.Internal.Project
{
	public enum Subtype {
		Code,
		Directory,
		WinForm,
		WebForm,
		XmlForm,
		WebService,		
		WebReferences,
		Dataset
	}
	
	public enum BuildAction {
		Nothing,
		Compile,
		EmbedAsResource,
		Exclude		
	}
	
	/// <summary>
	/// This class represent a file information in an IProject object.
	/// </summary>
	[XmlNodeName("File")]	
	public class ProjectFile  : ICloneable
	{
		[XmlAttribute("name"),
		 ConvertToRelativePath()]
		string      filename;
		
		[XmlAttribute("subtype")]	
		Subtype     subtype;
		
		[XmlAttribute("buildaction")]
		BuildAction buildaction;
		
		[XmlAttribute("dependson"),
		 ConvertToRelativePath()]		
		string		dependsOn;
		
		[XmlAttribute("data")]
		string		data;
																
		public string Name {
			get {
				return filename;
			}
			set {
				filename = value;
				Debug.Assert(filename != null && filename.Length > 0, "name == null || name.Length == 0");
			}
		}
		
		[ReadOnly(true)]
		public Subtype Subtype {
			get {
				return subtype;
			}
			set {
				subtype = value;
			}
		}
		
		public BuildAction BuildAction {
			get {
				return buildaction;
			}
			set {
				buildaction = value;
			}
		}
		
		[ReadOnly(true)]
		public string DependsOn {
			get {
				return dependsOn;
			}
			set {
				dependsOn = value;
			}
		}
		
		[ReadOnly(true)]
		public string Data {
			get {
				return data;
			}
			set {
				data = value;
			}
		}
												
		public ProjectFile()
		{
		}
		
		public ProjectFile(string filename)
		{
			this.filename = filename;
			subtype       = Subtype.Code;
			buildaction   = BuildAction.Compile;
		}
		
		public ProjectFile(string filename, BuildAction buildAction)
		{
			this.filename = filename;
			subtype       = Subtype.Code;
			buildaction   = buildAction;
		}
		
		public object Clone()
		{
			return MemberwiseClone();
		}
		
		public override string ToString()
		{
			return "[ProjectFile: FileName=" + filename + ", Subtype=" + subtype + ", BuildAction=" + BuildAction + "]";
		}
										
		
	}
}
