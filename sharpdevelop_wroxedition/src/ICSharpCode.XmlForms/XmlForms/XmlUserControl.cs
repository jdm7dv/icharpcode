// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ICSharpCode.XmlForms {
	
	/// <summary>
	/// The basic xml generated user control.
	/// </summary>
	public abstract class XmlUserControl : UserControl
	{
		protected XmlLoader xmlLoader;
		
		/// <summary>
		/// Gets the ControlDictionary for this user control.
		/// </summary>
		public ControlDictionary ControlDictionary {
			get {
				return xmlLoader.ControlDictionary;
			}
		}
		
		/// <summary>
		/// Creates a new instance
		/// </summary>
		/// <param name="fileName">
		/// Name of the xml file which defines this user control.
		/// </param>
		public XmlUserControl(string fileName)
		{
			xmlLoader = new XmlLoader();
			SetupXmlLoader();
			if (fileName != null && fileName.Length > 0) {
				xmlLoader.LoadObjectFromFileDefinition(this, fileName);
			}
		}
		
		protected virtual void SetupXmlLoader()
		{
		}
	}
}
