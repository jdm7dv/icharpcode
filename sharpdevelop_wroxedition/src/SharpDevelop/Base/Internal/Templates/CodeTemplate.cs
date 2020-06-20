// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.Diagnostics;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	/// <summary>
	/// This class reperesents a single Code Template
	/// </summary>
	public class CodeTemplate
	{
		string shortcut     = "";
		string description  = "";
		
		string text         = "";
		
		public string Shortcut {
			get {
				return shortcut;
			}
			set {
				shortcut = value;
				Debug.Assert(shortcut != null, "ICSharpCode.SharpDevelop.Internal.Template : string Shortcut == null");
			}
		}
		
		public string Description {
			get {
				return description;
			}
			set {
				description = value;
				Debug.Assert(description != null, "ICSharpCode.SharpDevelop.Internal.Template : string Description == null");
			}
		}
		
		public string Text {
			get {
				return text;
			}
			set {
				text = value;
				Debug.Assert(text != null, "ICSharpCode.SharpDevelop.Internal.Template : string Text == null");
			}
		}
		
		public CodeTemplate()
		{}
		
		public CodeTemplate(string shortcut, string description, string text)
		{
			this.shortcut    = shortcut;
			this.description = description;
			this.text        = text;
		}
		
		public CodeTemplate(XmlElement el)
		{
			if (el == null) {
				throw new ArgumentNullException("CodeTemplate(XmlElement el) : el can't be null");
			}
			
			if (el.Attributes["SHORTCUT"] == null || el.Attributes["DESCRIPTION"] == null) {
				throw new Exception("CodeTemplate(XmlElement el) : SHORTCUT and DESCRIPTION attributes must exist (check the CodeTemplate XML)");
			}
			Shortcut    = el.Attributes["SHORTCUT"].InnerText;
			Description = el.Attributes["DESCRIPTION"].InnerText;
			Text        = el.InnerText;
		}
		
		public XmlElement ToXmlElement(XmlDocument doc)
		{
			if (doc == null) {
				throw new ArgumentNullException("CodeTemplate.ToXmlElement(XmlDocument doc) : doc can't be null");
			}
			XmlElement el = doc.CreateElement("TEMPLATE");
			
			XmlAttribute shortcutattr = doc.CreateAttribute("SHORTCUT");
			shortcutattr.InnerText = Shortcut;
			
			XmlAttribute descriptionattr = doc.CreateAttribute("DESCRIPTION");
			descriptionattr.InnerText = Description;
			
			el.Attributes.Append(shortcutattr);
			el.Attributes.Append(descriptionattr);
			el.InnerText = Text;
			return el;
		}
	}
}
