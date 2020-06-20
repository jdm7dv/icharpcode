// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace ICSharpCode.TextEditor.Document
{
	internal class HighlightingDefinitionParser
	{
		private HighlightingDefinitionParser()
		{
			// This is a pure utility class with no instances.	
		}
		
		static string[] environmentColors = {"VRuler", "Selection", "Cursor" , "LineNumbers",
											"InvalidLines", "EOLMarkers", "SpaceMarkers", "TabMarkers",
											"CaretMarker", "BookMarks", "FoldLine", "FoldMarker"};
		static ArrayList errors = null;
		
		internal static DefaultHighlightingStrategy Parse(string aFileName)
		{
			try {
				string schemaFileName = aFileName.Substring(0, aFileName.LastIndexOf(Path.DirectorySeparatorChar) + 1) + "Mode.xsd"; 
				XmlValidatingReader validatingReader = new XmlValidatingReader(new XmlTextReader(aFileName));
				validatingReader.Schemas.Add("", schemaFileName);
				validatingReader.ValidationType = ValidationType.Schema;
			    validatingReader.ValidationEventHandler += new ValidationEventHandler (ValidationHandler);
				
				
				XmlDocument doc = new XmlDocument();
				doc.Load(validatingReader);
				
				DefaultHighlightingStrategy highlighter = new DefaultHighlightingStrategy(doc.DocumentElement.Attributes["name"].InnerText);
				
				if (doc.DocumentElement.Attributes["extensions"]!= null) {
					highlighter.Extensions = doc.DocumentElement.Attributes["extensions"].InnerText.Split(new char[] { '|' });
				}
				/*
				if (doc.DocumentElement.Attributes["indent"]!= null) {
					highlighter.DoIndent = Boolean.Parse(doc.DocumentElement.Attributes["indent"].InnerText);
				}
				*/
				XmlElement environment = doc.DocumentElement["Environment"];
		
				highlighter.SetDefaultColor(new HighlightBackground(environment["Default"]));
				
				foreach (string aColorName in environmentColors) {
					highlighter.SetColorFor(aColorName, new HighlightColor(environment[aColorName]));
				}	
		
				if (doc.DocumentElement["Digits"]!= null) {
					highlighter.SetColorFor("Digits", new HighlightColor(doc.DocumentElement["Digits"]));
				}
				
				XmlNodeList nodes = doc.DocumentElement.GetElementsByTagName("RuleSet");
				foreach (XmlElement element in nodes) {
					highlighter.AddRuleSet(new HighlightRuleSet(element));
				}
				
				if(errors!=null) {
					ReportErrors(aFileName);
					errors = null;
					return null;
				} else {
					return highlighter;		
				}
			} catch (Exception e) {
				MessageBox.Show("Could not load mode definition file. Reason:\n\n" + e.ToString(), "SharpDevelop", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return null;
			}
		}	
		
		private static void ValidationHandler(object sender, ValidationEventArgs args)
		{
			if (errors==null) {
				errors=new ArrayList();
			}
			errors.Add(args);
		}

		private static void ReportErrors(string fileName)
		{
			StringBuilder msg = new StringBuilder();
			msg.Append("Could not load mode definition file. Reason:\n\n");
			foreach(ValidationEventArgs args in errors) {
				msg.Append(args.Message);
				msg.Append(Console.Out.NewLine);
			}
			MessageBox.Show(msg.ToString(), "SharpDevelop", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}

	}
}
