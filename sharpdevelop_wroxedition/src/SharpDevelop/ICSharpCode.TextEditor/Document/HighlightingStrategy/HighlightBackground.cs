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
	public class HighlightBackground : HighlightColor
	{
		Image backgroundImage;
		
		public Image BackgroundImage {
			get {
				return backgroundImage;
			}
		}
		
		public HighlightBackground(XmlElement el) : base(el)
		{
			if (el.Attributes["image"] != null) {
				backgroundImage = new Bitmap(el.Attributes["image"].InnerText);
			}
		}
		
		public HighlightBackground(Color color, Color backgroundcolor, bool bold, bool italic) : base(color, backgroundcolor, bold, italic)
		{
		}
	}
}
