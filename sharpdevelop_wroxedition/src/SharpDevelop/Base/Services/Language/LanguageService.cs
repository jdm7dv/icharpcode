// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.Resources;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Services
{
	public class LanguageService : AbstractService
	{
		string languagePath = Application.StartupPath +
		                      Path.DirectorySeparatorChar + ".." +
		                      Path.DirectorySeparatorChar + "data" +
		                      Path.DirectorySeparatorChar + "resources" +
		                      Path.DirectorySeparatorChar + "languages" +
		                      Path.DirectorySeparatorChar;
		
		ImageList languageImageList = new ImageList();
		ArrayList languages         = new ArrayList();
		
		public ImageList LanguageImageList {
			get {
				return languageImageList;
			}
		}
		
		public ArrayList Languages {
			get {
				return languages;
			}
		}
		
		public LanguageService()
		{
			LanguageImageList.ImageSize = new Size(46, 38);
			
			XmlDocument doc = new XmlDocument();
			doc.Load(languagePath + "LanguageDefinition.xml");
			
			XmlNodeList nodes = doc.DocumentElement.ChildNodes;
			
			foreach (XmlElement el in nodes) {
				languages.Add(new Language(el.Attributes["name"].InnerText,
				                           el.Attributes["code"].InnerText,
				                           LanguageImageList.Images.Count));
				LanguageImageList.Images.Add(new Bitmap(languagePath + el.Attributes["icon"].InnerText));
			}
		}
	}
}
