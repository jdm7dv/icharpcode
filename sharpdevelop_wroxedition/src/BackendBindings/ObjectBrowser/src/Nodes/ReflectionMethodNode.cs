// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Reflection;
using System.Reflection.Emit;

using SharpDevelop.Gui.Edit;

namespace SharpDevelop.Gui.Edit.Reflection
{
	public class ReflectionMethodNode : ReflectionNode
	{
		MethodInfo      methodinfo      = null;
		ConstructorInfo constructorinfo = null;
		
		public ReflectionMethodNode(MethodInfo methodinfo2) : base ("", methodinfo2, ReflectionNodeType.Method)
		{
			this.methodinfo = methodinfo2;
			SetNodeName();
		}
		
		public ReflectionMethodNode(ConstructorInfo constructorinfo2) : base ("", constructorinfo2, ReflectionNodeType.Constructor)
		{
			this.constructorinfo = constructorinfo2;
			SetNodeName();
		}
		
		void SetNodeName()
		{
			if (attribute == null) {
				Text = "NONAME";
				return;
			}
			switch (type) {
				case ReflectionNodeType.Method:
					Text = ReflectionTree.languageConversion.Convert(methodinfo);
					break;
				case ReflectionNodeType.Constructor:
					Text = ReflectionTree.languageConversion.Convert(constructorinfo);
					break;
			}
		}
		
		protected override void SetIcon()
		{
			if (attribute == null)
				return;
			switch (type) {
				case ReflectionNodeType.Method:
					if (methodinfo == null)
						methodinfo      = (MethodInfo)attribute;
					if (methodinfo.IsPrivate) { // private
//						Console.WriteLine("Set to " +(METHODINDEX + 3));
						ImageIndex  = SelectedImageIndex = METHODINDEX + 3;
						break;
					} else
					if (methodinfo.IsFamily ) { // protected
//						Console.WriteLine("Set to " +(METHODINDEX + 2));
						ImageIndex  = SelectedImageIndex = METHODINDEX + 2;
						break;
					} 
//					Console.WriteLine("Set to " +(METHODINDEX ));
					ImageIndex  = SelectedImageIndex = METHODINDEX;
					break;
				case ReflectionNodeType.Constructor:
					if (constructorinfo == null)
						constructorinfo = (ConstructorInfo)attribute;
					if (constructorinfo.IsPrivate) { // private
//						Console.WriteLine("cSet to " +(METHODINDEX + 3));
						ImageIndex  = SelectedImageIndex = METHODINDEX + 3;
						break;
					} else
					if (constructorinfo.IsFamily ) { // protected
//						Console.WriteLine("cSet to " +(METHODINDEX + 2));

						ImageIndex  = SelectedImageIndex = METHODINDEX + 2;
						break;
					} 
//					Console.WriteLine("Set to " +(METHODINDEX ));
					ImageIndex  = SelectedImageIndex = METHODINDEX;
					break;
			}
		}
	}
}
