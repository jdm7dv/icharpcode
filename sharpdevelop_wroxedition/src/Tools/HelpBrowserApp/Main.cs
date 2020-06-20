// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.IO;
using System.Diagnostics;
using System.Xml;

namespace ICSharpCode.HelpConverter {
	
	public class HelpBrowserApp
	{
		HhcFileParser hhcFileParser = new HhcFileParser();
		
		/// <remarks>
		/// Parses the xml tree and generates a TreeNode tree out of it.
		/// </remarks>
		void ParseTree(XmlDocument doc, XmlNode docParent, XmlNode parentNode)
		{
			foreach (XmlNode node in parentNode.ChildNodes) {
				XmlNode importNode = doc.ImportNode(node, true);
				switch (node.Name) {
					case "HelpFolder":
						docParent.AppendChild(importNode);
						ParseTree(doc, importNode, node);
						break;
					case "HelpFile":
						hhcFileParser.Parse(doc,
						                    docParent,
						                    node.Attributes["hhc"].InnerText,
						                    node.Attributes["chm"].InnerText);
						break;
				}
			}
		}
		
		void ConvertHelpfile()
		{
			XmlDocument doc = new XmlDocument();
			doc.Load("HelpDescription.xml");
			
			XmlDocument newDoc = new XmlDocument();
			newDoc.LoadXml("<HelpCollection/>");
			ParseTree(newDoc, newDoc.DocumentElement, doc.DocumentElement);
			
			newDoc.Save("HelpConv.xml");
		}
		
		public static void Main(String[] args)
		{
			new HelpBrowserApp().ConvertHelpfile();
		}
	}	
}
