// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Forms;

using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	/// <summary>
	/// This class defines and holds text templates
	/// they're a bit similar than code templates, but they're
	/// not inserted automaticaly
	/// </summary>
	public class TextTemplate
	{
		public static ArrayList TextTemplates = new ArrayList();
		
		string    name    = null;
		ArrayList entries = new ArrayList();
		
		public string Name {
			get {
				return name;
			}
		}
		
		public ArrayList Entries {
			get {
				return entries;
			}
		}
		
		public class Entry 
		{
			public string Display;
			public string Value;
			
			public Entry(XmlElement el)
			{
				this.Display = el.Attributes["Display"].InnerText;
				this.Value   = el.Attributes["Value"].InnerText;
			}
			
			public override string ToString()
			{
				return Display;
			}
		}
		
		public TextTemplate(string filename)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			
			name = doc.DocumentElement.Attributes["Name"].InnerText;
			
			XmlNodeList nodes = doc.DocumentElement.ChildNodes;
			foreach (XmlElement entrynode in nodes) {
				entries.Add(new Entry(entrynode));
			}
		}
		
		static void LoadTextTemplate(string filename)
		{
			TextTemplates.Add(new TextTemplate(filename));
		}
		
		static TextTemplate()
		{
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			StringCollection files = fileUtilityService.SearchDirectory(Application.StartupPath +
			                            Path.DirectorySeparatorChar + ".." +
			                            Path.DirectorySeparatorChar + "data" + 
			                            Path.DirectorySeparatorChar + "options" + 
			                            Path.DirectorySeparatorChar + "textlib", "*.xml");
			foreach (string file in files) {
				LoadTextTemplate(file);
			}
		}
	}
}

