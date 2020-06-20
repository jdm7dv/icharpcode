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
	public class CSharpAmbience :  AbstractAmbience
	{
		static string[,] typeConversionList = new string[,] {
			{"System.Void",    "void"},
			{"System.Object",  "object"},
			{"System.Boolean", "bool"},
			{"System.Byte",    "byte"},
			{"System.SByte",   "sbyte"},
			{"System.Char",    "char"},
			{"System.Enum",    "enum"},
			{"System.Int16",   "short"},
			{"System.Int32",   "int"},
			{"System.Int64",   "long"},
			{"System.UInt16",  "ushort"},
			{"System.UInt32",  "uint"},
			{"System.UInt64",  "ulong"},
			{"System.Single",  "float"},
			{"System.Double",  "double"},
			{"System.Decimal", "decimal"},
			{"System.String",  "string"}
		};
		
		static Hashtable typeConversionTable = new Hashtable();
		
		static CSharpAmbience()
		{
			for (int i = 0; i < typeConversionList.GetLength(0); ++i) {
				typeConversionTable[typeConversionList[i, 0]] = typeConversionList[i, 1];
			}
		}
		
		public string Convert(ModifierEnum modifier)
		{
			StringBuilder builder = new StringBuilder();
			if (ShowAccessibility) {
				if ((modifier & ModifierEnum.Public) == ModifierEnum.Public) {
					builder.Append("public");
				} else if ((modifier & ModifierEnum.Private) == ModifierEnum.Private) {
					builder.Append("private");
				} else if ((modifier & (ModifierEnum.Protected | ModifierEnum.Internal)) == (ModifierEnum.Protected | ModifierEnum.Internal)) {
					builder.Append("protected internal");
				} else if ((modifier & ModifierEnum.ProtectedOrInternal) == ModifierEnum.ProtectedOrInternal) {
					builder.Append("internal protected");
				} else if ((modifier & ModifierEnum.Internal) == ModifierEnum.Internal) {
					builder.Append("internal");
				} else if ((modifier & ModifierEnum.Protected) == ModifierEnum.Protected) {
					builder.Append("protected");
				}
				builder.Append(' ');
			}
			return builder.ToString();
		}
		
		string GetModifier(IDecoration decoration)
		{
			StringBuilder builder = new StringBuilder();
			
			if (decoration.IsStatic) {
				builder.Append("static ");
			}
			
			if (decoration.IsVirtual) {
				builder.Append("virtual ");
			}
			
			if (decoration.IsOverride) {
				builder.Append("override ");
			}
			
			if (decoration.IsNew) {
				builder.Append("new ");
			}
			
			return builder.ToString();
		}
		
		
		public override string Convert(IClass c)
		{
			StringBuilder builder = new StringBuilder();
			
			builder.Append(Convert(c.Modifiers));
			
			if (ShowModifiers) {
				if (c.IsSealed) {
					builder.Append("sealed ");
				} else if (c.IsAbstract) {
					builder.Append("abstract ");
				}
			}
			
			if (ShowModifiers) {
				switch (c.ClassType) {
					case ClassType.Delegate:
						builder.Append("delegate");
						break;
					case ClassType.Class:
						builder.Append("class");
						break;
					case ClassType.Struct:
						builder.Append("struct");
						break;
					case ClassType.Interface:
						builder.Append("interface");
						break;
					case ClassType.Enum:
						builder.Append("enum");
						break;
				}
				builder.Append(' ');
			}
			
			if (c.ClassType == ClassType.Delegate && c.Methods.Count > 0) {
				builder.Append(Convert(c.Methods[0].ReturnType));
				builder.Append(' ');
			}
			
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
				builder.Append(')');
			} else if (ShowInheritanceList) {
				if (c.BaseTypes.Count > 0) {
					builder.Append(" : ");
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
					builder.Append("const ");
				} else if (field.IsStatic) {
					builder.Append("static ");
				}
				
				if (field.IsReadonly) {
					builder.Append("readonly ");
				}
			}
			
			if (field.ReturnType != null) {
				builder.Append(Convert(field.ReturnType));
				builder.Append(' ');
			}
			
			if (UseFullyQualifiedNames) {
				builder.Append(field.FullyQualifiedName);
			} else {
				builder.Append(field.Name);
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
			
			if (property.ReturnType != null) {
				builder.Append(Convert(property.ReturnType));
				builder.Append(' ');
			}
			
			if (UseFullyQualifiedNames) {
				builder.Append(property.FullyQualifiedName);
			} else {
				builder.Append(property.Name);
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
			
			if (e.ReturnType != null) {
				builder.Append(Convert(e.ReturnType));
				builder.Append(' ');
			}
			
			if (UseFullyQualifiedNames) {
				builder.Append(e.FullyQualifiedName);
			} else {
				builder.Append(e.Name);
			}
			
			return builder.ToString();
		}
		
		public override string Convert(IIndexer m)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(Convert(m.Modifiers));
			
			if (ShowModifiers) {
				if (m.IsStatic) {
					builder.Append("static ");
				}
			}
			
			if (m.ReturnType != null) {
				builder.Append(Convert(m.ReturnType));
				builder.Append(' ');
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
			
			builder.Append(']');
			return builder.ToString();
		}
		
		public override string Convert(IMethod m)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(Convert(m.Modifiers));
			
			if (ShowModifiers) {
				builder.Append(GetModifier(m));
			}
			
			if (m.ReturnType != null) {
				builder.Append(Convert(m.ReturnType));
				builder.Append(' ');
			}
			
			if (UseFullyQualifiedNames) {
				builder.Append(m.FullyQualifiedName);
			} else {
				builder.Append(m.Name);
			}
			
			builder.Append('(');
			
			for (int i = 0; i < m.Parameters.Count; ++i) {
				builder.Append(Convert(m.Parameters[i]));
				if (i + 1 < m.Parameters.Count) {
					builder.Append(", ");
				}
			}
			
			builder.Append(")");
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
				if (UseFullyQualifiedNames) {
					builder.Append(returnType.FullyQualifiedName);
				} else {
					builder.Append(returnType.Name);
				}
			}
			
			for (int i = 0; i < returnType.PointerNestingLevel; ++i) {
				builder.Append('*');
			}
			
			
			for (int i = 0; i < returnType.ArrayCount; ++i) {
				builder.Append('[');
				for (int j = 1; j < returnType.ArrayDimensions[i]; ++j) {
					builder.Append(',');
				}
				builder.Append(']');
			}
			
			return builder.ToString();
		}
		
		public override string Convert(IParameter param)
		{
			StringBuilder builder = new StringBuilder();
			if (param.IsRef) {
				builder.Append("ref ");
			} else if (param.IsOut) {
				builder.Append("out ");
			} else if (param.IsParams) {
				builder.Append("params ");
			}
			
			builder.Append(Convert(param.ReturnType));
			
			if (ShowParameterNames) {
				builder.Append(' ');
				builder.Append(param.Name);
			}
			return builder.ToString();
		}
	}
}
