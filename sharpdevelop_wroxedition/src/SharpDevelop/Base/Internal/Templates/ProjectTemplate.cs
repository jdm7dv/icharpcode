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
using System.Reflection;
using System.Windows.Forms;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	/// <summary>
	/// This class defines and holds the new project templates.
	/// </summary>
	public class ProjectTemplate
	{
		public static ArrayList ProjectTemplates = new ArrayList();
		
		string    originator   = null;
		string    created      = null;
		string    lastmodified = null;
		string    name         = null;
		string    category     = null;
		string    languagename = null;
		string    description  = null;
		string    icon         = null;
		string    wizardpath   = null;
		
		ArrayList projectfiles = new ArrayList(); // contains FileTemplate classes
		ArrayList openfiles    = new ArrayList(); // contains FileTemplate classes
		
		XmlElement projectoptions = null;
		
		public string WizardPath {
			get {
				return wizardpath;
			}
		}
		
		public string Originator {
			get {
				return originator;
			}
		}
		
		public string Created {
			get {
				return created;
			}
		}
		
		public string LastModified {
			get {
				return lastmodified;
			}
		}
		
		public string Name {
			get {
				return name;
			}
		}
		
		public string Category {
			get {
				return category;
			}
		}
		
		public string LanguageName {
			get {
				return languagename;
			}
		}
		
		public string Description {
			get {
				return description;
			}
		}
		
		public string Icon {
			get {
				return icon;
			}
		}
		
		public XmlElement ProjectOptions {
			get {
				return projectoptions;
			}
		}
		
		public ArrayList ProjectFiles {
			get {
				return projectfiles;
			}
		}
		public ArrayList OpenFiles {
			get {
				return openfiles;
			}
		}
		
		public ProjectTemplate(string filename)
		{
			XmlDocument doc = new XmlDocument();
			
			doc.Load(filename);
			
			XmlElement config = doc.DocumentElement["TemplateConfiguration"];
			
			originator   = doc.DocumentElement.Attributes["Originator"].InnerText;
			created      = doc.DocumentElement.Attributes["Created"].InnerText;
			lastmodified = doc.DocumentElement.Attributes["LastModified"].InnerText;
			
			if (config["Wizard"] != null) {
				wizardpath = config["Wizard"].Attributes["path"].InnerText;
			}
			
			
			name         = config["Name"].InnerText;
			category     = config["Category"].InnerText;
			languagename = config["LanguageName"].InnerText;
			
			if (config["Description"] != null)
				description  = config["Description"].InnerText;
			
			if (config["Icon"] != null)
				icon         = config["Icon"].InnerText;
			
			projectoptions = doc.DocumentElement["ProjectOptions"];
			
			// load the files
			XmlElement files  = doc.DocumentElement["ProjectFiles"];
			XmlNodeList nodes = files.ChildNodes;
			foreach (XmlElement filenode in nodes) {
				FileDescriptionTemplate template = new FileDescriptionTemplate(filenode.Attributes["Name"].InnerText, filenode.InnerText);
				projectfiles.Add(template);
				if (filenode.Attributes["Open"] != null && filenode.Attributes["Open"].InnerText == "True") {
					openfiles.Add(template);
				}
			}
		}
		
		static void LoadProjectTemplate(string filename)
		{
			try {
				ProjectTemplates.Add(new ProjectTemplate(filename));
			} catch (Exception e) {
				throw new ApplicationException("error while loading " + filename + " original exception was : " + e.ToString());
			}
		}
		
		static ProjectTemplate()
		{
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			StringCollection files = fileUtilityService.SearchDirectory(Application.StartupPath +
			                            Path.DirectorySeparatorChar + ".." +
			                            Path.DirectorySeparatorChar + "data" +
			                            Path.DirectorySeparatorChar + "templates" +
			                            Path.DirectorySeparatorChar + "project", "*.xpt");
			try {
				foreach (string file in files) {
					LoadProjectTemplate(file);
				}
			} catch (Exception e) {
				ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
				MessageBox.Show(resourceService.GetString("Internal.Templates.ProjectTemplate.LoadingError") + "\n" + e.ToString(), resourceService.GetString("Global.ErrorText"), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
			}
		}
	}
}
