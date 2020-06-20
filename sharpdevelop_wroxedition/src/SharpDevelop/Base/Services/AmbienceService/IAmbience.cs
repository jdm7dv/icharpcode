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
	[Flags]
	public enum ConversionFlags {
		None                   = 0,
		ShowParameterNames     = 1,
		ShowAccessibility      = 16,
		UseFullyQualifiedNames = 2,
		ShowModifiers          = 4,
		ShowInheritanceList    = 8,
		
		StandardConversionFlags = ShowParameterNames | 
		                          UseFullyQualifiedNames | 
		                          ShowModifiers,
		All = ShowParameterNames | 
		      ShowAccessibility | 
		      UseFullyQualifiedNames |
		      ShowModifiers | 
		      ShowInheritanceList
	}
	
	public interface IAmbience
	{
		ConversionFlags ConversionFlags {
			get;
			set;
		}
		
		string Convert(IClass c);
		string Convert(IIndexer c);
		string Convert(IField field);
		string Convert(IProperty property);
		string Convert(IEvent e);
		string Convert(IMethod m);
		string Convert(IParameter param);
	}
}