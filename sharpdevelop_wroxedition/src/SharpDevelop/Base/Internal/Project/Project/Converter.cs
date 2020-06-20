// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace ICSharpCode.SharpDevelop.Internal.Project
{
	/// <summary>
	/// This class converts Visual Studio.NET C# project files
	/// (as in *.csproj) into SharpDevelop C# project files (as in
	/// *.prjx)
	/// </summary>
	public class ConvertXml
	{
		
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		/// <remarks>
		/// The main module loads the three required input vars
		/// and performs the transform
		/// </remarks>
		/// <param name="args">
		/// arg1 - the input file (preferably VS.NET .csproj)
		/// arg2 - path to XSL transform file
		/// arg3 - path to output file (preferably SD .prjx)
		/// </param>
		public static void Convert(string sInputFilePath, string sXSLFilePath, string sOutputFilePath)
		{
			// UNCOMMENT FOR DEBUGGING
			// string sInputFilePath = "my.csproj";
			// string sXSLFilePath = "Convert VSNet csharp To SharpDevelop.xsl";
			// string sOutputFilePath = "mysdproj.prjx";
			
			// Transform the file
			XmlReader reader = GetXML(sInputFilePath);
			XmlReader oTransformed = TransformXmlToXml(reader, sXSLFilePath, null);
			reader.Close();
			
			// Output results to file path
			XmlDocument myDoc = new XmlDocument();
			myDoc.Load(oTransformed);
			myDoc.Save(sOutputFilePath);
			
		}
		
		/// <summary>
		/// Transforms XML to XML
		/// </summary>
		/// <param name="oXML"></param>
		/// <param name="XSLPath"></param>
		/// <param name="xsltArgList"></param>
		/// <returns></returns>
		public static XmlReader TransformXmlToXml(XmlReader oXML, string XSLPath, XsltArgumentList xsltArgList)
		{
			
			// Load the xml document
			XmlDataDocument myxmldatadoc = new XmlDataDocument();
			myxmldatadoc.Load(oXML);
			
			return TransformXmlToXml(myxmldatadoc, XSLPath, xsltArgList);
			
		}
		
		/// <summary>
		/// Does the transform work
		/// </summary>
		/// <param name="oXDoc"></param>
		/// <param name="XSLPath"></param>
		/// <param name="xsltArgumentList"></param>
		/// <returns></returns>
		public static XmlReader TransformXmlToXml(XmlDataDocument oXDoc, string XSLPath, XsltArgumentList xsltArgumentList)
		{
			XslTransform myxsl = new XslTransform();
			
			// Get the stylesheet
			myxsl.Load(GetXML(XSLPath));
			
			// Perform transform and return
			return myxsl.Transform(oXDoc, xsltArgumentList);
		}
		
		/// <summary>
		/// GetXML returns an XmlReader dependent on the contents
		/// of the passed input param.
		/// GetXML checks for the following conditions:
		/// blank string returns an empty XmlReader
		/// less-than at start assumes an XML file
		/// back-slash at start assumes UNC path
		/// otherwise, URL is assumed
		/// </summary>
		/// <param name="strInput"></param>
		/// <returns></returns>
		public static XmlReader GetXML(string strInput)
		{
			// Check if string is blank
			if (strInput.Length == 0)
			{
				// Return the empty xml reader
				return new XmlTextReader("");
			}
			else
				{
					// Check if string starts with "<"
					// If it does, it is an XML file
					if (strInput.Substring(0,1) == "<")
					{
						//String could be an xml file - load
						return new XmlTextReader(new StringReader(strInput));
					}
					else
						{
							// Assume this is a file path - return loaded XML
							return new XmlTextReader(strInput);
						}
				}
		}
	}
}
