// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;

using SharpDevelop.Internal.Parser;
using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	class CodeCompletionData : ICompletionData
	{
		static ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
		static IParserService           parserService           = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
		static AmbienceService          ambienceService = (AmbienceService)ServiceManager.Services.GetService(typeof(AmbienceService));
		
		int      imageIndex;
		int      overloads;
		string   text;
		string   description;
		string   documentation;
		string   completionString;
		IClass   c;
		bool     convertedDocumentation = false;
		
		public int Overloads {
			get {
				return overloads;
			}
			set {
				overloads = value;
			}
		}
		
		public int ImageIndex {
			get {
				return imageIndex;
			}
			set {
				imageIndex = value;
			}
		}
		
		public string[] Text {
			get {
				return new string[] { text };
			}
			set {
				text = value[0];
			}
		}
		
		public string Description {
			get {
				// get correct delegate description (when description is requested)
				// in the classproxies aren't methods saved, therefore delegate methods
				// must be get through the real class instead out of the proxy
				//
				// Mike
				if (c is ClassProxy && c.ClassType == ClassType.Delegate) {
					description = ambienceService.CurrentAmbience.Convert(parserService.GetClass(c.FullyQualifiedName));
					c = null;
				}
				
				// don't give a description string, if no documentation or description is provided
				if (description.Length + documentation.Length == 0) {
					return null;
				}
				if (!convertedDocumentation) {
					convertedDocumentation = true;
					try {
						XmlDocument doc = new XmlDocument();
						doc.LoadXml("<doc>" + documentation + "</doc>");
						XmlNode root      = doc.DocumentElement;
						XmlNode paramDocu = root.SelectSingleNode("summary");
						documentation = paramDocu.InnerXml;
					} catch (Exception e) {
						Console.WriteLine(e.ToString());
					}
				}
				return description + (overloads > 0 ? " (+" + overloads + " overloads)" : String.Empty) + "\n" + documentation;
			}
			set {
				description = value;
			}
		}
		
		public CodeCompletionData(string s, int imageIndex)
		{
			description = documentation = String.Empty;
			text = s;
			completionString = s;
			this.imageIndex = imageIndex;
		}
		
		public CodeCompletionData(IClass c)
		{
			// save class (for the delegate description shortcut
			this.c = c;
			imageIndex = classBrowserIconService.GetIcon(c);
			text = c.Name;
			completionString = c.Name;
			description = ambienceService.CurrentAmbience.Convert(c);
			documentation = c.Documentation;
		}
		
		public CodeCompletionData(IMethod method)
		{
			imageIndex  = classBrowserIconService.GetIcon(method);
			text        = method.Name;
			description = ambienceService.CurrentAmbience.Convert(method);
			completionString = method.Name;
			documentation = method.Documentation;
		}
		
		public CodeCompletionData(IField field)
		{
			imageIndex  = classBrowserIconService.GetIcon(field);
			text        = field.Name;
			description = ambienceService.CurrentAmbience.Convert(field);
			completionString = field.Name;
			documentation = field.Documentation;
		}
		
		public CodeCompletionData(IProperty property)
		{
			imageIndex  = classBrowserIconService.GetIcon(property);
			text        = property.Name;
			description = ambienceService.CurrentAmbience.Convert(property);
			completionString = property.Name;
			documentation = property.Documentation;
		}
		
		public CodeCompletionData(IEvent e)
		{
			imageIndex  = classBrowserIconService.GetIcon(e);
			text        = e.Name;
			description = ambienceService.CurrentAmbience.Convert(e);
			completionString = e.Name;
			documentation = e.Documentation;
		}
		
		public void InsertAction(TextAreaControl control)
		{
			((SharpDevelopTextAreaControl)control).InsertString(completionString);
		}
	}
}
