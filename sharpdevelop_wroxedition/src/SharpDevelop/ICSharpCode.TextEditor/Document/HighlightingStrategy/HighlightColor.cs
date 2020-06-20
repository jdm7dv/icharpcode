// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace ICSharpCode.TextEditor.Document
{
	public class HighlightColor
	{
		bool   systemColor     = false;
		string systemColorName = null;
		
		bool   systemBgColor     = false;
		string systemBgColorName = null;
		
		Color  color;
		Color  backgroundcolor = System.Drawing.Color.WhiteSmoke;
		
		bool   bold   = false;
		bool   italic = false;
		string name   = "";
		
		public string Name {
			get {
				return name;
			}
		}
		
		public bool Bold {
			get {
				return bold;
			}
		}
		
		public bool Italic {
			get {
				return italic;
			}
		}
		
		public Color BackgroundColor {
			get {
				if (!systemBgColor) {
					return backgroundcolor;
				}
				return ParseColorString(systemBgColorName);
			}
		}
		
		public Color Color {
			get {
				if (!systemColor) {
					return color;
				}
				return ParseColorString(systemColorName);
			}
		}
		
		public Font Font {
			get {
				if (Bold) {
					return Italic ? FontContainer.BoldItalicFont : FontContainer.BoldFont;
				}
				return Italic ? FontContainer.ItalicFont :FontContainer.DefaultFont;
			}
		}
		
		Color ParseColorString(string colorName)
		{
			string[] cNames = colorName.Split('*');
			PropertyInfo myPropInfo = typeof(System.Drawing.SystemColors).GetProperty(cNames[0], BindingFlags.Public | 
			                                                                                     BindingFlags.Instance | 
			                                                                                     BindingFlags.Static);
			Color c = (Color)myPropInfo.GetValue(null, null);
			
			if (cNames.Length == 2) {
				// hack : can't figure out how to parse doubles with '.' (culture info might set the '.' to ',')
				double factor = Double.Parse(cNames[1]) / 100;
				c = Color.FromArgb((int)((double)c.R * factor), (int)((double)c.G * factor), (int)((double)c.B * factor));
			}
			
           	return c;
		}
		
		public HighlightColor(XmlElement el)
		{
			Debug.Assert(el != null, "ICSharpCode.TextEditor.Document.SyntaxColor(XmlElement el) : el == null");
			if (el.Attributes["bold"] != null) {
				bold = Boolean.Parse(el.Attributes["bold"].InnerText);
			}
			
			if (el.Attributes["italic"] != null) {
				italic = Boolean.Parse(el.Attributes["italic"].InnerText);
			}
			
			if (el.Attributes["color"] != null) {
				string c = el.Attributes["color"].InnerText;
				if (c[0] == '#') {
					color = ParseColor(c);
				} else if (c.StartsWith("SystemColors.")) {
					systemColor     = true;
					systemColorName = c.Substring("SystemColors.".Length);
				} else {
					color = (Color)(Color.GetType()).InvokeMember(c, BindingFlags.GetProperty, null, Color, new object[0]);
				}
				
			} else {
				color = Color.Transparent; // to set it to the default value.
			}
			
			if (el.Attributes["bgcolor"] != null) {
				string c = el.Attributes["bgcolor"].InnerText;
				if (c[0] == '#') {
					backgroundcolor = ParseColor(c);
				} else if (c.StartsWith("SystemColors.")) {
					systemBgColor     = true;
					systemBgColorName = c.Substring("SystemColors.".Length);
				} else {
					backgroundcolor = (Color)(Color.GetType()).InvokeMember(c, BindingFlags.GetProperty, null, Color, new object[0]);
				}
			}
		}
		
		public HighlightColor(XmlElement el, HighlightColor defaultColor)
		{
			Debug.Assert(el != null, "ICSharpCode.TextEditor.Document.SyntaxColor(XmlElement el) : el == null");
			if (el.Attributes["bold"] != null) {
				bold = Boolean.Parse(el.Attributes["bold"].InnerText);
			} else {
				bold = defaultColor.Bold;
			}
			
			if (el.Attributes["italic"] != null) {
				italic = Boolean.Parse(el.Attributes["italic"].InnerText);
			} else {
				italic = defaultColor.Italic;
			}
			
			if (el.Attributes["color"] != null) {
				string c = el.Attributes["color"].InnerText;
				if (c[0] == '#') {
					color = ParseColor(c);
				} else if (c.StartsWith("SystemColors.")) {
					systemColor     = true;
					systemColorName = c.Substring("SystemColors.".Length);
				} else {
					color = (Color)(Color.GetType()).InvokeMember(c, BindingFlags.GetProperty, null, Color, new object[0]);
				}
			} else {
				color = defaultColor.color;
			}
			
			if (el.Attributes["bgcolor"] != null) {
				string c = el.Attributes["bgcolor"].InnerText;
				if (c[0] == '#') {
					backgroundcolor = ParseColor(c);
				} else if (c.StartsWith("SystemColors.")) {
					systemBgColor     = true;
					systemBgColorName = c.Substring("SystemColors.".Length);
				} else {
					backgroundcolor = (Color)(Color.GetType()).InvokeMember(c, BindingFlags.GetProperty, null, Color, new object[0]);
				}
			} else {
				backgroundcolor = defaultColor.BackgroundColor;
			}
		}
		
		public HighlightColor(Color color, bool bold, bool italic)
		{
			this.color  = color;
			this.bold   = bold;
			this.italic = italic;
		}
		
		public HighlightColor(Color color, Color backgroundcolor, bool bold, bool italic)
		{
			this.color            = color;
			this.backgroundcolor  = backgroundcolor;
			this.bold             = bold;
			this.italic           = italic;
		}
		
		static Color ParseColor(string c)
		{
			int a = 255;
			int offset = 0;
			if (c.Length > 7) {
				offset = 2;
				a = Int32.Parse(c.Substring(1,2), NumberStyles.HexNumber);
			}
			
			int r = Int32.Parse(c.Substring(1 + offset,2), NumberStyles.HexNumber);
			int g = Int32.Parse(c.Substring(3 + offset,2), NumberStyles.HexNumber);
			int b = Int32.Parse(c.Substring(5 + offset,2), NumberStyles.HexNumber);
			return Color.FromArgb(a, r, g, b);
		}
		
		public override string ToString()
		{
			return "[HighlightColor: Name = " + Name + ", Bold = " + 
			       Bold + ", Italic = " + Italic + ", Color = " + 
			       Color + ", BackgroundColor = " + BackgroundColor + "]";
		}
	}
}