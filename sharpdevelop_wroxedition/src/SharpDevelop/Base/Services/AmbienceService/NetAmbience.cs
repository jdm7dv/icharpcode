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
	public class NetAmbience :  AbstractAmbience
	{
		public override string Convert(IClass c)
		{
			StringBuilder builder = new StringBuilder();
			
			if (ShowModifiers) {
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
						builder.Append("Enumeration");
						break;
				}
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
				if (c.Methods.Count > 0) {
					builder.Append(" : ");
					builder.Append(Convert(c.Methods[0].ReturnType));
				}
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
			if (ShowModifiers) {
				builder.Append("Field");
				builder.Append(' ');
			}
			
			if (UseFullyQualifiedNames) {
				builder.Append(field.FullyQualifiedName);
			} else {
				builder.Append(field.Name);
			}
			
			if (field.ReturnType != null) {
				builder.Append(" : ");
				builder.Append(Convert(field.ReturnType));
			}
			return builder.ToString();			
		}
		
		public override string Convert(IProperty property)
		{
			StringBuilder builder = new StringBuilder();
			if (ShowModifiers) {
				builder.Append("Property");
				builder.Append(' ');
			}
			
			if (UseFullyQualifiedNames) {
				builder.Append(property.FullyQualifiedName);
			} else {
				builder.Append(property.Name);
			}
			if (property.ReturnType != null) {
				builder.Append(" : ");
				builder.Append(Convert(property.ReturnType));
			}
			return builder.ToString();
		}
		
		public override string Convert(IEvent e)
		{
			StringBuilder builder = new StringBuilder();
			if (ShowModifiers) {
				builder.Append("Event ");
			}
			
			if (UseFullyQualifiedNames) {
				builder.Append(e.FullyQualifiedName);
			} else {
				builder.Append(e.Name);
			}
			if (e.ReturnType != null) {
				builder.Append(" : ");
				builder.Append(Convert(e.ReturnType));
			}
			return builder.ToString();
		}
		
		public override string Convert(IIndexer m)
		{
			StringBuilder builder = new StringBuilder();
			if (ShowModifiers) {
				builder.Append("Indexer ");
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
				builder.Append(" : ");
				builder.Append(Convert(m.ReturnType));
			}
			return builder.ToString();
		}
		
		public override string Convert(IMethod m)
		{
			StringBuilder builder = new StringBuilder();
			if (ShowModifiers) {
				builder.Append("Method ");
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
			if (m.ReturnType != null) {
				builder.Append(" : ");
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
			builder.Append(returnType.Name);
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
			if (ShowParameterNames) {
				builder.Append(param.Name);
				builder.Append(" : ");
			}
			builder.Append(Convert(param.ReturnType));
			if (param.IsRef) {
				builder.Append("&");
			}
			return builder.ToString();
		}
	}
}
