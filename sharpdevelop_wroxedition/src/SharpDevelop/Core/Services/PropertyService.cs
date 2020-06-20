// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using ICSharpCode.Core.Properties;

namespace ICSharpCode.Core.Services
{ 
	/// <summary>
	/// This class handles the Global Properties for the IDE, all what can be configured should be
	/// loaded/saved by this class. It is a bit like a Singleton with static delegation instead
	/// of returning a static reference to a <code>IProperties</code> object.
	/// </summary>
	public class PropertyService : DefaultProperties, IService
	{
		
		readonly static string propertyFileName    = "SharpDevelopProperties.xml";
		readonly static string propertyFileVersion = "1.1";
		
		readonly static string propertyXmlRootNodeName  = "SharpDevelopProperties";
		
		readonly static string defaultPropertyDirectory = Application.StartupPath + 
		                                                  Path.DirectorySeparatorChar + ".." +
		                                                  Path.DirectorySeparatorChar + "data" +
		                                                  Path.DirectorySeparatorChar + "options";
		
		readonly static string configDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + 
		                                         Path.DirectorySeparatorChar + ".ICSharpCode" +
		                                         Path.DirectorySeparatorChar + "SharpDevelop" +
		                                         Path.DirectorySeparatorChar;
		
		/// <summary>
		/// returns the path of the default application configuration directory
		/// </summary>
		public string ConfigDirectory {
			get {
				return configDirectory;
			}
		}
		
		public PropertyService()
		{
			try {
				LoadProperties();
			} catch (PropertyFileLoadException) {
				MessageBox.Show("Can't load property file", "Warning",MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}
		
		void WritePropertiesToFile(string fileName)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<?xml version=\"1.0\"?>\n<" + propertyXmlRootNodeName + " fileversion = \"" + propertyFileVersion + "\" />");
			
			doc.DocumentElement.AppendChild(ToXmlElement(doc));
			
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			fileUtilityService.ObservedSave(new NamedFileOperationDelegate(doc.Save), fileName, FileErrorPolicy.ProvideAlternative);
		}
		
		bool LoadPropertiesFromStream(string filename)
		{
			try {
				XmlDocument doc = new XmlDocument();
				doc.Load(filename);
				
				if (doc.DocumentElement.Attributes["fileversion"].InnerText != propertyFileVersion) {
					return false;
				}
				SetValueFromXmlElement(doc.DocumentElement["Properties"]);
			} catch (Exception e) {
				Console.WriteLine("Exception while load properties from stream :\n " + e.ToString());
				return false;
			}
			return true;
		}
		
		/// <summary>
		/// Loads the global properties from the current users application data folder, or
		/// if it doesn't exists or couldn't read them it reads the default properties out
		/// of the application folder.
		/// </summary>
		/// <exception cref="PropertyFileLoadException">
		/// Is thrown when no property file could be loaded.
		/// </exception>
		void LoadProperties()
		{
			if (!Directory.Exists(configDirectory)) {
				Directory.CreateDirectory(configDirectory);
			}
			
			if (!LoadPropertiesFromStream(configDirectory + propertyFileName)) {
				Console.WriteLine("Can't read global properties in user path, using default properties");
				if (!LoadPropertiesFromStream(defaultPropertyDirectory + Path.DirectorySeparatorChar + propertyFileName)) {
					throw new PropertyFileLoadException();
				}
			}
		}
		
		/// <summary>
		/// Saves the current global property state to a file in the users application data folder.
		/// </summary>
		void SaveProperties()
		{
			WritePropertiesToFile(configDirectory + propertyFileName);
		}
		
		// IService implementation:
		public virtual void InitializeService()
		{
			OnInitialize(EventArgs.Empty);
		}
		
		public virtual void UnloadService()
		{
			// save properties on exit
			SaveProperties();
			OnUnload(EventArgs.Empty);
		}
		
		protected virtual void OnInitialize(EventArgs e)
		{
			if (Initialize != null) {
				Initialize(this, e);
			}
		}
		
		protected virtual void OnUnload(EventArgs e)
		{
			if (Unload != null) {
				Unload(this, e);
			}
		}
		
		public event EventHandler Initialize;
		public event EventHandler Unload;			
	}
}
