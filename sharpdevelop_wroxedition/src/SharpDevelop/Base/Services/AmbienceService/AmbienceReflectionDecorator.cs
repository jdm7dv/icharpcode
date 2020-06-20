// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
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
	/// <summary>
	/// This class wraps a ILanguageConversion to System.Reflection
	/// </summary>
	public class AmbienceReflectionDecorator : IAmbience
	{
		IAmbience conv;
		
		public ConversionFlags ConversionFlags {
			get {
				return conv.ConversionFlags;
			}
			set {
				conv.ConversionFlags = value;
			}
		}
		
		public string Convert(IClass c)
		{
			return conv.Convert(c);
		}
		public string Convert(IField field)
		{
			return conv.Convert(field);
		}

		public string Convert(IProperty property)
		{
			return conv.Convert(property);
		}
		
		public string Convert(IEvent e)
		{
			return conv.Convert(e);
		}
		
		public string Convert(IIndexer indexer)
		{
			return conv.Convert(indexer);
		}
		
		public string Convert(IMethod m)
		{
			return conv.Convert(m);
		}
		
		public string Convert(IParameter param)
		{
			return conv.Convert(param);
		}		
		
		public AmbienceReflectionDecorator(IAmbience conv)
		{
			this.conv = conv;
		}
		
		public string Convert(Type type)
		{
			return conv.Convert(new ReflectionClass(type, null));
		}
		
		public string Convert(FieldInfo field)
		{
			return conv.Convert(new ReflectionField(field, null));
		}
		
		public string Convert(PropertyInfo property)
		{
			return conv.Convert(new ReflectionProperty(property, null));
		}
		
		public string Convert(EventInfo e)
		{
			return conv.Convert(new ReflectionEvent(e, null));
		}
		
		public string Convert(MethodBase m)
		{
			return conv.Convert(new ReflectionMethod(m, null));
		}
		
		public string Convert(ParameterInfo param)
		{
			return conv.Convert(new ReflectionParameter(param, null));
		}
	}
}
