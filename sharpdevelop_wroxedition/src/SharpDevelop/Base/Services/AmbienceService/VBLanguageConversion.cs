// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Diagnostics;
using System.Windows.Forms;
using SharpDevelop.Internal.Parser;

using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Services
{
	public class VBAmbience :  AbstractAmbience
	{
		static string[,] typeConversionList = new string[,] {
			{"System.String",  "String"},
			{"System.Single",  "Single"},
			{"System.Int16",   "Short"},
			{"System.Void",    "Void"},
			{"System.Object",  "Object"},
			{"System.Int64",   "Long"},
			{"System.Int32",   "Integer"},
			{"System.Double",  "Double"},
			{"System.Char",    "Char"},
			{"System.Boolean", "Boolean"},
			{"System.Byte",    "Byte"},
			{"System.Decimal", "Decimal"},
			{"System.DateTime",  "Date"},
		};
		
		static Hashtable typeConversionTable = new Hashtable();
		
		static VBAmbience()
		{
			for (int i = 0; i < typeConversionList.GetLength(0); ++i) {
				typeConversionTable[typeConversionList[i, 0]] = typeConversionList[i, 1];
			}
		}
		
		string GetModifier(IDecoration decoration)
		{
			StringBuilder builder = new StringBuilder();
			
			if (decoration.IsStatic) {
				builder.Append("Shared ");
			}
			if (decoration.IsVirtual) {
				builder.Append("Overridable ");
			}
			if (decoration.IsAbstract) {
				builder.Append("MustOverride ");
			}
			if (decoration.IsOverride) {
				builder.Append("Overrides ");
			}
			if (decoration.IsNew) {
				builder.Append("Shadows ");
			}
			
			return builder.ToString();
		}
		
		public string Convert(ModifierEnum modifier)
		{
			StringBuilder builder = new StringBuilder();
			if (ShowAccessibility) {
				if ((modifier & ModifierEnum.Public) == ModifierEnum.Public) {
					builder.Append("Public");
				} else if ((modifier & ModifierEnum.Private) == ModifierEnum.Private) {
					builder.Append("Private");
				} else if ((modifier & (ModifierEnum.Protected | ModifierEnum.Internal)) == (ModifierEnum.Protected | ModifierEnum.Internal)) {
					builder.Append("Protected Friend");
				} else if ((modifier & ModifierEnum.ProtectedOrInternal) == ModifierEnum.ProtectedOrInternal) {
					builder.Append("Protected Friend");
				} else if ((modifier & ModifierEnum.Internal) == ModifierEnum.Internal) {
					builder.Append("Friend");
				} else if ((modifier & ModifierEnum.Protected) == ModifierEnum.Protected) {
					builder.Append("Protected");
				}
				builder.Append(' ');
			}
			return builder.ToString();
		}
		
		public override string Convert(IClass c)
		{
			StringBuilder builder = new StringBuilder();
			
			builder.Append(Convert(c.Modifiers));
			
			if (ShowModifiers) {
				if (c.IsSealed) {
					builder.Append("NotInheritable ");
				} else if (c.IsAbstract) {
					builder.Append("MustInherit ");
				}
			}
			
			switch (c.ClassType) {
				case ClassType.Delegate:
					builder.Append("Delegate");
					break;
				case ClassType.Class:
					builder.Append("Class");
					break;
				case ClassType.Struct:
					builder.Append("Structure");
					break;
				case ClassType.Interface:
					builder.Append("Interface");
					break;
				case ClassType.Enum:
					builder.Append("Enum");
					break;
			}
			builder.Append(' ');
			
			if (UseFullyQualifiedNames) {
				builder.Append(c.FullyQualifiedName);
			} else {
				builder.Append(c.Name);
			}
			
			if (c.ClassType == ClassType.Delegate) {
				builder.Append('(');
				if (c.Methods.Count > 0) {
					for (int i = 0; i < c.Methods[0].Parameters.Count; ++i) {
						builder.Append(Convert(c.Methods[0].Parameters[i]));
						if (i + 1 < c.Methods[0].Parameters.Count) {
							builder.Append(", ");
						}
					}
				}
				builder.Append(")");
				if (c.Methods.Count > 0) {
					builder.Append(" As "); 
					builder.Append(Convert(c.Methods[0].ReturnType));
				}
			} else if (ShowInheritanceList) {
				if (c.BaseTypes.Count > 0) {
					builder.Append(" Inherits ");
					for (int i = 0; i < c.BaseTypes.Count; ++i) {
						builder.Append(c.BaseTypes[i]);
						if (i + 1 < c.BaseTypes.Count) {
							builder.Append(", ");
						}
					}
				}
			}
			return builder.ToString();		
		}
		
		public override string Convert(IField field)
		{
			StringBuilder builder = new StringBuilder();
			
			builder.Append(Convert(field.Modifiers));
			
			if (ShowModifiers) {
				if (field.IsStatic && field.IsLiteral) {
					builder.Append("Const ");
				} else if (field.IsStatic) {
					builder.Append("Shared ");
				}
			}
						
			if (UseFullyQualifiedNames) {
				builder.Append(field.FullyQualifiedName);
			} else {
				builder.Append(field.Name);
			}
			
			if (field.ReturnType != null) {
				builder.Append(" As ");
				builder.Append(Convert(field.ReturnType));
			}			
			
			return builder.ToString();			
		}
		
		public override string Convert(IProperty property)
		{
			StringBuilder builder = new StringBuilder();
			
			builder.Append(Convert(property.Modifiers));
			
			if (ShowModifiers) {
				builder.Append(GetModifier(property));
			}
			
			if (UseFullyQualifiedNames) {
				builder.Append(property.FullyQualifiedName);
			} else {
				builder.Append(property.Name);
			}
			
			if (property.ReturnType != null) {
				builder.Append(" As ");
				builder.Append(Convert(property.ReturnType));
			}
			
			return builder.ToString();
		}
		
		public override string Convert(IEvent e)
		{
			StringBuilder builder = new StringBuilder();
			
			builder.Append(Convert(e.Modifiers));
			
			if (ShowModifiers) {
				builder.Append(GetModifier(e));
			}
			
			if (UseFullyQualifiedNames) {
				builder.Append(e.FullyQualifiedName);
			} else {
				builder.Append(e.Name);
			}
			
			if (e.ReturnType != null) {
				builder.Append(" As ");
				builder.Append(Convert(e.ReturnType));
			}
			
			return builder.ToString();
		}
		
		public override string Convert(IIndexer m)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(Convert(m.Modifiers));
			
			if (ShowModifiers) {
				if (m.IsStatic) {
					builder.Append("Shared ");
				}
			}
			
			if (UseFullyQualifiedNames) {
				builder.Append(m.FullyQualifiedName);
			} else {
				builder.Append(m.Name);
			}
			builder.Append('[');
			for (int i = 0; i < m.Parameters.Count; ++i) {
				builder.Append(Convert(m.Parameters[i]));
				if (i + 1 < m.Parameters.Count) {
					builder.Append(", ");
				}
			}
			
			builder.Append("]");
			
			if (m.ReturnType != null) {
				builder.Append(" As ");
				builder.Append(Convert(m.ReturnType));
			}			
			
			return builder.ToString();
		}
		
		public override string Convert(IMethod m)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(Convert(m.Modifiers));
			
			if (ShowModifiers) {
				builder.Append(GetModifier(m));
			}
			
			if (m.ReturnType == null || m.ReturnType.FullyQualifiedName == "System.Void") {
				builder.Append("Sub ");
			} else {
				builder.Append("Function ");
			}
			string dispName = m.Name;
			if (m.Name == "ctor" || m.Name == "cctor") {
				dispName = "New";
			}
			
			builder.Append(dispName);
			
			builder.Append('(');
			for (int i = 0; i < m.Parameters.Count; ++i) {
				builder.Append(Convert(m.Parameters[i]));
				if (i + 1 < m.Parameters.Count) {
					builder.Append(", ");
				}
			}
			
			builder.Append(")");
			
			if (m.ReturnType != null && m.ReturnType.FullyQualifiedName != "System.Void") {
				builder.Append(" As ");
				builder.Append(Convert(m.ReturnType));
			}
			
			return builder.ToString();
		}
		
		public string Convert(IReturnType returnType)
		{
			if (returnType == null) {
				return String.Empty;
			}
			StringBuilder builder = new StringBuilder();
			
			if (typeConversionTable[returnType.FullyQualifiedName] != null) {
				builder.Append(typeConversionTable[returnType.FullyQualifiedName].ToString());
			} else {
				builder.Append(returnType.Name);
			}
			
			for (int i = 0; i < returnType.PointerNestingLevel; ++i) {
				builder.Append('*');
			}
			
			for (int i = 0; i < returnType.ArrayCount; ++i) {
				builder.Append('(');
				for (int j = 1; j < returnType.ArrayDimensions[i]; ++j) {
					builder.Append(',');
				}
				builder.Append(')');
			}
			
			return builder.ToString();
		}
		
		public override string Convert(IParameter param)
		{
			StringBuilder builder = new StringBuilder();
			if (ShowParameterNames) {
				if (param.IsRef) {
					builder.Append("ByRef ");
				} else if (param.IsParams) {
					builder.Append("ByVal ParamArray ");
				} else  {
					builder.Append("ByVal ");
				}
			
				builder.Append(param.Name);
				builder.Append(" As ");
			}

			builder.Append(Convert(param.ReturnType));

			return builder.ToString();
		}
	}
}
