/*
 * AssemblyLoader.cs
 * Copyright (C) 2002 Mike Krueger, icsharpcode
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 * 
 * - Redistributions of source code must retain the above copyright notice, 
 *   this list of conditions and the following disclaimer. 
 * 
 * - Redistributions in binary form must reproduce the above copyright notice, 
 *   this list of conditions and the following disclaimer in the documentation 
 *   and/or other materials provided with the distribution. 
 * 
 * - Neither the name of icsharpcode nor the names of its contributors may 
 *   be used to endorse or promote products derived from this software without specific 
 *   prior written permission. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY 
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES 
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT 
 * SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT 
 * OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS 
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;

namespace ICSharpCode.SharpUnit.Environment {
	
	public class AssemblyLoader : MarshalByRefObject
	{
		string    assemblyLocation = null;
		
		AssemblyName assemblyName  = null;
		Assembly     assembly      = null;
		
		ArrayList testSuites       = new ArrayList();
		
		StringCollection dependendAssemblies = new StringCollection();
		StringCollection assemblyLookupPaths = new StringCollection();
		
		public Assembly Assembly {
			get {
				if (assembly == null) {
					LoadAssembly();
				}
				return assembly;
			}
			set {
				
				assembly = value;
			}
		}
		
		public AssemblyName AssemblyName {
			get {
				return assemblyName;
			}
		}
		
		public string AssemblyLocation {
			get {
				return assemblyLocation;
			}
		}
		
		public ArrayList TestSuites {
			get {
				if (assembly == null) {
					LoadAssembly();
				}
				return testSuites;
			}
		}
		
		public StringCollection DependendAssemblies {
			get {
				return dependendAssemblies;
			}
		}
		
		public StringCollection AssemblyLookupPaths {
			get {
				return assemblyLookupPaths;
			}
		}
		
		public AssemblyLoader(string assemblyLocation)
		{
			this.assemblyLocation = assemblyLocation;
			assemblyLookupPaths.Add(Path.GetDirectoryName(assemblyLocation));
			assemblyName          = AssemblyName.GetAssemblyName(assemblyLocation);
			LoadAssemblyConfiguration();
//			LoadAssembly();
		}
		
		public void ReloadAssembly()
		{
			assembly = AssemblyUtilities.LoadAssembly(assemblyLocation);
		}
		
		void LoadAssembly()
		{
			try {
				ReloadAssembly();
				testSuites.Clear();
				// retrieve testsuites in this assembly
				Type[] types = assembly.GetTypes();
				foreach(Type type in types) {
					if (!type.IsAbstract && Attribute.GetCustomAttribute(type, typeof(TestSuiteAttribute)) != null) {
						testSuites.Add(new DefaultTestSuite(this, type.FullName));
					}
				}
				
			} catch (Exception e) {
				throw new Exception("Can't load assembly " + assemblyLocation + " reason : " +e.ToString());
			}
		}
		
		public void LoadAssemblyConfiguration()
		{
			string fileName = assemblyLocation + ".testconfig";

			if (!File.Exists(fileName)) {
				return;
			}
			
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(fileName);
			foreach (XmlElement el in xmlDocument.DocumentElement.ChildNodes) {
				switch (el.Name) {
					case "Codebases":
						foreach (XmlElement assembly in el.ChildNodes) {
							AssemblyLookupPaths.Add(Path.GetDirectoryName(fileName) + Path.DirectorySeparatorChar + assembly.Attributes["location"].InnerText);
						}
						break;
					case "AssemblyDependencies":
						foreach (XmlElement assembly in el.ChildNodes) {
							string dependendAssembly = Path.GetDirectoryName(fileName) + Path.DirectorySeparatorChar + assembly.Attributes["location"].InnerText;
							dependendAssemblies.Add(dependendAssembly);
							Assembly.Load(dependendAssembly);
						}
						break;
					default:
						Console.WriteLine("Unknown node " + el.Name + " in configuration file " + fileName);
						break;
				}
			}
		}
	}
}
