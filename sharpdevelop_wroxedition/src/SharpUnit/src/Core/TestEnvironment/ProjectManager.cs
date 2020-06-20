/*
 * ProjectManager.cs
 * Copyright (C) 2002 Mike Krueger
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 * 3. The name of the author may not be used to endorse or promote products
 *    derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 * IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using System;
using System.IO;
using System.Reflection;

namespace ICSharpCode.SharpUnit.Environment {
	/*
	/// <summary>
	/// 
	/// </summary>
	public class ProjectManager
	{
		static IProject project = null;
		
		public static IProject Project {
			get {
				if (project == null) {
					CreateNewProject();
				}
				return project;
			}
		}
		
		public static void CreateNewProject()
		{
			project = new DefaultProject();
//			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);
		}
		
		static Assembly MyResolveEventHandler(object sender, ResolveEventArgs e)
		{
			Console.WriteLine("Lookup " + e.Name);
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				if (assembly.FullName == e.Name) {
					return assembly;
				}
			}
			string[] assemblyDescr = e.Name.Split(new char[] {','});
			
			foreach (string path in project.AssemblyLookupPaths) {
				Console.WriteLine("PATH : " + path);
				string dllName = path + Path.DirectorySeparatorChar + assemblyDescr[0] + ".dll";
				string exeName = path + Path.DirectorySeparatorChar + assemblyDescr[0] + ".exe";
				
				if (File.Exists(dllName)) {
					return Assembly.LoadFrom(dllName);
				}
				if (File.Exists(exeName)) {
					return Assembly.LoadFrom(exeName);
				}
			}
			return null;
		}
	}*/
}
