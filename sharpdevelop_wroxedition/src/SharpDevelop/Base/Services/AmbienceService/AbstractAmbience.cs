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
	public abstract class AbstractAmbience : IAmbience
	{
		ConversionFlags conversionFlags = ConversionFlags.ShowParameterNames     |
		                                  ConversionFlags.UseFullyQualifiedNames |
		                                  ConversionFlags.ShowInheritanceList    |
		                                  ConversionFlags.ShowModifiers;
		
		public ConversionFlags ConversionFlags {
			get {
				return conversionFlags;
			}
			set {
				conversionFlags = value;
			}
		}		
		
		public bool ShowAccessibility {
			get {
				return (conversionFlags & ConversionFlags.ShowAccessibility) == ConversionFlags.ShowAccessibility;
			}
		}
		
		public bool ShowParameterNames {
			get {
				return (conversionFlags & ConversionFlags.ShowParameterNames) == ConversionFlags.ShowParameterNames;
			}
		}
		
		public bool UseFullyQualifiedNames {
			get {
				return (conversionFlags & ConversionFlags.UseFullyQualifiedNames) == ConversionFlags.UseFullyQualifiedNames;
			}
		}
		
		public bool ShowModifiers {
			get {
				return (conversionFlags & ConversionFlags.ShowModifiers) == ConversionFlags.ShowModifiers;
			}
		}
		
		public bool ShowInheritanceList {
			get {
				return (conversionFlags & ConversionFlags.ShowInheritanceList) == ConversionFlags.ShowInheritanceList;
			}
		}
		
		public abstract string Convert(IClass c);
		public abstract string Convert(IField c);
		public abstract string Convert(IProperty property);
		public abstract string Convert(IEvent e);
		public abstract string Convert(IIndexer indexer);
		public abstract string Convert(IMethod m);
		public abstract string Convert(IParameter param);
	}
}
