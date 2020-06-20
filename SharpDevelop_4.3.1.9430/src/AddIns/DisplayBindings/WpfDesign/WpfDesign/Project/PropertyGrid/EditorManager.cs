﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.WpfDesign.PropertyGrid.Editors;

namespace ICSharpCode.WpfDesign.PropertyGrid
{
	/// <summary>
	/// Manages registered type and property editors.
	/// </summary>
	public static class EditorManager
	{
		// property return type => editor type
		static Dictionary<Type, Type> typeEditors = new Dictionary<Type, Type>();
		// property full name => editor type
		static Dictionary<string, Type> propertyEditors = new Dictionary<string, Type>();

		/// <summary>
		/// Creates a property editor for the specified <paramref name="property"/>
		/// </summary>
		public static FrameworkElement CreateEditor(DesignItemProperty property)
		{
			Type editorType;
			if (!propertyEditors.TryGetValue(property.FullName, out editorType)) {
				var type = property.ReturnType;
				while (type != null) {
					if (typeEditors.TryGetValue(type, out editorType)) {
						break;
					}
					type = type.BaseType;
				}
				if (editorType == null) {
					var standardValues = Metadata.GetStandardValues(property.ReturnType);
					if (standardValues != null) {
						return new ComboBoxEditor() { ItemsSource = standardValues };
					}
					return new TextBoxEditor();
				}
			}
			return (FrameworkElement)Activator.CreateInstance(editorType);
		}
		
		/// <summary>
		/// Registers property editors defined in the specified assembly.
		/// </summary>
		public static void RegisterAssembly(Assembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");
			
			foreach (Type type in assembly.GetExportedTypes()) {
				foreach (TypeEditorAttribute editorAttribute in type.GetCustomAttributes(typeof(TypeEditorAttribute), false)) {
					CheckValidEditor(type);
					typeEditors[editorAttribute.SupportedPropertyType] = type;
				}
				foreach (PropertyEditorAttribute editorAttribute in type.GetCustomAttributes(typeof(PropertyEditorAttribute), false)) {
					CheckValidEditor(type);
					string propertyName = editorAttribute.PropertyDeclaringType.FullName + "." + editorAttribute.PropertyName;
					propertyEditors[propertyName] = type;
				}
			}
		}
		
		static void CheckValidEditor(Type type)
		{
			if (!typeof(FrameworkElement).IsAssignableFrom(type)) {
				throw new DesignerException("Editor types must derive from FrameworkElement!");
			}
		}
	}
}
