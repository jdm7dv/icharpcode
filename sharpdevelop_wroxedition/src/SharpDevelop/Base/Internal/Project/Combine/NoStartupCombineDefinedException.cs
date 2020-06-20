// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Project;

using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Internal.Project
{
	public class NoStartupCombineDefinedException : Exception
	{
		public NoStartupCombineDefinedException()
		{
		}
		public NoStartupCombineDefinedException(string msg) : base(msg)
		{
		}
	}
	
}
