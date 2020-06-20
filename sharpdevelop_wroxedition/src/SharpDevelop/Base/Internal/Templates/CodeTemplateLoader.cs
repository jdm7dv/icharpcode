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
using System.Xml;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Internal.Templates;

using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	/// <summary>
	/// This class handles the code templates
	/// </summary>
	public class CodeTemplateLoader
	{
		static string TEMPLATEFILE        = "SharpDevelop-templates.xml";
		static string TEMPLATEFILEVERSION = "1";
		
		static ArrayList template         = new ArrayList();
		
		public static ArrayList Template {
			get {
				return template;
			}
			set {
				template = value;
				Debug.Assert(template != null, "SharpDevelop.Tool.Data.TemplateLoader : set ArrayList Template (value == null)");
			}
		}
		
		static bool LoadTemplatesFromStream(string filename)
		{
			XmlDocument doc = new XmlDocument();
			try {
				doc.Load(filename);
				
				template = new ArrayList();
				
				XmlNodeList nodes = doc.DocumentElement.ChildNodes;
				
				if (doc.DocumentElement.Attributes["VERSION"].InnerText != TEMPLATEFILEVERSION) {
					return false;
				}
				
				foreach (XmlElement el in nodes) {
					template.Add(new CodeTemplate(el));
				}
				
			} catch (Exception) {
				return false;
			}
			return true;
		}
		
		static void WriteTemplatesToFile(string fileName)
		{
			XmlDocument doc    = new XmlDocument();
			
			doc.LoadXml("<TEMPLATES VERSION = \"" + TEMPLATEFILEVERSION + "\" />");
			
			foreach (CodeTemplate temp in template) {
				doc.DocumentElement.AppendChild(temp.ToXmlElement(doc));
			}
			
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			fileUtilityService.ObservedSave(new NamedFileOperationDelegate(doc.Save), fileName, FileErrorPolicy.ProvideAlternative);
		}
		
		/// <summary>
		/// This method loads the code templates from a XML based
		/// configuration file.
		/// </summary>
		static CodeTemplateLoader()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			if (!LoadTemplatesFromStream(propertyService.ConfigDirectory + TEMPLATEFILE)) {
				Console.WriteLine("Templates: can't load user defaults, reading system defaults");
				if (!LoadTemplatesFromStream(Application.StartupPath + 
				                             Path.DirectorySeparatorChar + ".." +
				                             Path.DirectorySeparatorChar + "data" +
				                             Path.DirectorySeparatorChar + "options" +
				                             Path.DirectorySeparatorChar + TEMPLATEFILE)) {
					ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));
					MessageBox.Show(resourceService.GetString("Internal.Templates.CodeTemplateLoader.CantLoadTemplatesWarning"), resourceService.GetString("Global.WarningText"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
		}
		
		/// <summary>
		/// This method saves the code templates to a XML based
		/// configuration file in the current user's own files directory
		/// </summary>
		public static void SaveTemplates()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			WriteTemplatesToFile(propertyService.ConfigDirectory + TEMPLATEFILE);
		}
		
	}
}
