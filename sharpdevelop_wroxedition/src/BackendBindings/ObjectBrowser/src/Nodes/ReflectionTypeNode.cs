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

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

using SharpDevelop.Gui.Edit;

namespace SharpDevelop.Gui.Edit.Reflection
{
	public class ReflectionTypeNode : ReflectionNode
	{
		public ReflectionTypeNode(string name, Type type) : base (name, type, ReflectionNodeType.Type)
		{
		}
		
		protected override void SetIcon()
		{
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
			ImageIndex  = SelectedImageIndex = classBrowserIconService.GetIcon((Type)attribute);
		}
		
		public override void Populate()
		{
			Type type = (Type)attribute;
//			object[] attr = type.GetCustomAttributes(false);
//			foreach (object o in attr)
//				Console.WriteLine("ATTR : " + o.ToString());

			Nodes.Clear();
			
			ConstructorInfo[] constructorinfos  = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic);
			MethodInfo[]      methodinfos       = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			FieldInfo[]       fieldinfos        = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			PropertyInfo[]    propertyinfos     = type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			EventInfo[]       eventinfos        = type.GetEvents(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			
			ReflectionNode supertype = new ReflectionNode("SuperTypes", type, ReflectionNodeType.SuperTypes);
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
			
			Nodes.Add(supertype);
			if (type.BaseType != null) {
				ReflectionNode basetype = new ReflectionNode(type.BaseType.Name,  type.BaseType, ReflectionNodeType.Link);
				basetype.ImageIndex = basetype.SelectedImageIndex = classBrowserIconService.GetIcon(type.BaseType);
				supertype.Nodes.Add(basetype);
			}
				
			foreach (Type baseinterface in  type.GetInterfaces()) {
				ReflectionNode inode = new ReflectionNode(baseinterface.Name,  baseinterface, ReflectionNodeType.Link);
				inode.ImageIndex = inode.SelectedImageIndex = classBrowserIconService.GetIcon(baseinterface);
				supertype.Nodes.Add(inode);
			}
			
			Nodes.Add(new ReflectionNode("SubTypes", type, ReflectionNodeType.SubTypes));
			
			foreach (ConstructorInfo constructorinfo in constructorinfos) {
				Nodes.Add(new ReflectionMethodNode(constructorinfo));
			}
			
			foreach (MethodInfo methodinfo in methodinfos) {
				if (methodinfo.DeclaringType.Equals(type))
				if ((methodinfo.Attributes & MethodAttributes.SpecialName) == 0)
					Nodes.Add(new ReflectionMethodNode(methodinfo));
			}
			
			foreach (FieldInfo fieldinfo  in fieldinfos) {
				if (fieldinfo.DeclaringType.Equals(type))
					Nodes.Add(new ReflectionNode(fieldinfo.Name, fieldinfo, ReflectionNodeType.Field));
			}
			
			foreach (PropertyInfo propertyinfo in propertyinfos) {
				if (propertyinfo.DeclaringType.Equals(type))
					Nodes.Add(new ReflectionNode(propertyinfo.Name, propertyinfo, ReflectionNodeType.Property));
			}
			
			foreach (EventInfo eventinfo in eventinfos) {
				if (eventinfo.DeclaringType.Equals(type))
					Nodes.Add(new ReflectionNode(eventinfo.Name, eventinfo, ReflectionNodeType.Event));
			}
			
			populated = true;
		}
	}
}
