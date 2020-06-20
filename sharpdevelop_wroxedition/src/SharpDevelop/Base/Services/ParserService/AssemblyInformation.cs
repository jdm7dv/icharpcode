// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.Core.AddIns;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui;
using SharpDevelop.Internal.Parser;
using ICSharpCode.SharpDevelop.Gui.Dialogs;

namespace ICSharpCode.SharpDevelop.Services
{
	/// <summary>
	/// This class loads an assembly and converts all types from this assembly
	/// to a parser layer Class Collection.
	/// </summary>
	public class AssemblyInformation
	{
		ClassCollection classes = new ClassCollection();
		
		/// <value>
		/// A <code>ClassColection</code> that contains all loaded classes.
		/// </value>
		public ClassCollection Classes {
			get {
				return classes;
			}
		}
				
		/// <remarks>
		/// Loads an assembly given by <code>fileName</code> and adds all the classes
		/// to the <code>Classes</code> collection.
		/// Does convert the xml documentation for them too (if avaiable under the same
		/// name as the assembly but with the extension ".xml"
		/// </remarks>
		public void LoadAssembly(string fileName)
		{
			string   xmlDocFile = Path.ChangeExtension(fileName, ".xml");
//			AppDomain appDomain = AppDomain.CreateDomain("friendlyName");
//			Assembly assembly   = appDomain.Load(AssemblyName.GetAssemblyName(fileName));			
			Assembly assembly   = Assembly.LoadFrom(fileName);
			
			// read xml documentation for the assembly
			XmlDocument doc       = null;
			Hashtable   docuNodes = new Hashtable();
			if (File.Exists(xmlDocFile)) {
				doc = new XmlDocument();
				doc.Load(xmlDocFile);
				
				// convert the XmlDocument into a hash table
				if (doc.DocumentElement != null && doc.DocumentElement["members"] != null) {
					foreach (XmlNode node in doc.DocumentElement["members"].ChildNodes) {
						if (node != null && node.Attributes != null && node.Attributes["name"] != null) {
							docuNodes[node.Attributes["name"].InnerText] = node;
						}
					}
				}
			}
			
			foreach (Type type in assembly.GetTypes()) {
				if (type.IsPublic) {
					classes.Add(new ReflectionClass(type, docuNodes));
				}
			}
			
//			AppDomain.Unload(appDomain);
		}
	}
}
