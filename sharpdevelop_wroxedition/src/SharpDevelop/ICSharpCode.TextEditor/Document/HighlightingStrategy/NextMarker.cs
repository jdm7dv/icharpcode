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
	public class NextMarker
	{
		string      what;
		HighlightColor color;
		bool        markMarker = false;
		
		public string What {
			get {
				return what;
			}
		}
		
		public HighlightColor Color {
			get {
				return color;
			}
		}
		
		public bool MarkMarker {
			get {
				return markMarker;
			}
		}
		
		public NextMarker(XmlElement mark)
		{
			color = new HighlightColor(mark);
			what  = mark.InnerText;
			if (mark.Attributes["markmarker"] != null) {
				markMarker = Boolean.Parse(mark.Attributes["markmarker"].InnerText);
			}
		}
	}

}
