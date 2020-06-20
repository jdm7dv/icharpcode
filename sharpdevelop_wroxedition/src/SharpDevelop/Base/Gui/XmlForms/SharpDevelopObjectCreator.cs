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
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.XmlForms;

namespace ICSharpCode.SharpDevelop.Gui.XmlForms
{
	public class SharpDevelopObjectCreator : DefaultObjectCreator
	{
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		public override object CreateObject(string name)
		{
			object o = base.CreateObject(name);
			if (o != null) {
				

				try {
					bool useFlatStyle = Crownwood.Magic.Common.VisualStyle.IDE == (Crownwood.Magic.Common.VisualStyle)propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.VisualStyle", Crownwood.Magic.Common.VisualStyle.IDE);
					if (o is System.Windows.Forms.ButtonBase ) {
						((System.Windows.Forms.ButtonBase)o).FlatStyle = useFlatStyle ? FlatStyle.Flat : FlatStyle.Standard;
					} else if (o is System.Windows.Forms.TextBoxBase) {
						((System.Windows.Forms.TextBoxBase)o).BorderStyle = useFlatStyle ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
					}
				} catch (Exception) {}
			}
			return o;
		}
		
	}
}
