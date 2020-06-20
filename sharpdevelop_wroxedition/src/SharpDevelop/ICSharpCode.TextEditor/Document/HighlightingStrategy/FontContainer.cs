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
using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;

namespace ICSharpCode.TextEditor.Document
{
	public class FontContainer
	{
		static Font defaultfont    = null;
		static Font boldfont       = null;
		static Font italicfont     = null;
		static Font bolditalicfont = null;
		
		public static Font BoldFont {
			get {
				Debug.Assert(boldfont != null, "ICSharpCode.TextEditor.Document.FontContainer : boldfont == null");
				return boldfont;
			}
		}
			
		public static Font ItalicFont {
			get {
				Debug.Assert(italicfont != null, "ICSharpCode.TextEditor.Document.FontContainer : italicfont == null");
				return italicfont;
			}
		}
		
		public static Font BoldItalicFont {
			get {
				Debug.Assert(bolditalicfont != null, "ICSharpCode.TextEditor.Document.FontContainer : bolditalicfont == null");
				return bolditalicfont;
			}
		}
		
		public static Font DefaultFont {
			get {
				return defaultfont;
			}
			set {
				defaultfont    = value;
				boldfont       = new Font(defaultfont, FontStyle.Bold);
				italicfont     = new Font(defaultfont, FontStyle.Italic);
				bolditalicfont = new Font(defaultfont, FontStyle.Bold | FontStyle.Italic);
			}
		}
		
		static void CheckFontChange(object sender, PropertyEventArgs e)
		{
			if (e.Key == "DefaultFont") {
				DefaultFont = ParseFont(e.NewValue.ToString());
			}
		}
//		[Font: Name=Courier New, Size=10, Units=3, GdiCharSet=1, GdiVerticalFont=False]
		
		static Font ParseFont(string font)
		{
			string[] descr = font.Split(new char[]{',', '='});
			return new Font(descr[1], Single.Parse(descr[3]));
		}
		
		static FontContainer()
		{
#if !BuildAsStandalone
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			IProperties properties = ((IProperties)propertyService.GetProperty("ICSharpCode.TextEditor.Document.Document.DefaultDocumentAggregatorProperties", new DefaultProperties()));
			DefaultFont = ParseFont(properties.GetProperty("DefaultFont", new Font("Courier New", 10).ToString()));
			properties.PropertyChanged += new PropertyEventHandler(CheckFontChange);
#else
			DefaultFont = new Font("Courier New", 10);
#endif			
		}
	}
}
