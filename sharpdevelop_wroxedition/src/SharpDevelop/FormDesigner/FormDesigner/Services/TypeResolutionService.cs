// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace ICSharpCode.SharpDevelop.FormDesigner.Services
{
	public class TypeResolutionService : ITypeResolutionService
	{
		public Assembly GetAssembly(AssemblyName name)
		{
			return GetAssembly(name, false);
		}
		
		public Assembly GetAssembly(AssemblyName name, bool throwOnError)
		{
			Console.WriteLine("Try to get assembly : " + name);
			Console.ReadLine();
			return null;
		}
		
		
		public string GetPathOfAssembly(AssemblyName name)
		{
			Console.WriteLine("GetPathOfAssembly " + name);
			Console.ReadLine();
			Assembly assembly = GetAssembly(name);
			if (assembly != null) {
				return assembly.FullName;
			}
			return null;
		}
		
		public Type GetType(string name)
		{
			return GetType(name, false);
		}
		
		public Type GetType(string name, bool throwOnError)
		{
			return GetType(name, throwOnError);
		}
		
		public Type GetType(string name, bool throwOnError, bool ignoreCase)
		{
			Console.WriteLine("name " + name + " throw : " + throwOnError + " ignoreCase : " + ignoreCase);
			Console.ReadLine();
			
			return null;
		}
		
		public void ReferenceAssembly(AssemblyName name)
		{
			Console.WriteLine("Add Assembly reference : " + name);
			Console.ReadLine();
		}
	}
}
