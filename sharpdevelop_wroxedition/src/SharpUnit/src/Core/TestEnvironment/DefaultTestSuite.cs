/*
 * DefaultTestSuite.cs
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
using System.Diagnostics;
using System.Reflection;
using System.Collections;

namespace ICSharpCode.SharpUnit.Environment {
	
	/// <summary>
	/// 
	/// </summary>
	public sealed class DefaultTestSuite : MarshalByRefObject, ITestSuite
	{
		Type type;
		string fullTypeName;
		ArrayList tests  = null;
		object testClass = null;
		AssemblyLoader assemblyLoader = null;
		
		public Type Type {
			get {
				return type;
			}
		}
		
		public string FullTypeName {
			get {
				return fullTypeName;
			}
		}
		
		public string Description {
			get {
				return ((TestSuiteAttribute)Attribute.GetCustomAttribute(type, typeof(TestSuiteAttribute))).Description;
			}
		}
		
		public object TestClass {
			get {
				if (testClass == null) {
					testClass = type.Assembly.CreateInstance(type.FullName);
				}
				return testClass;
			}
		}
		
		public ArrayList Tests {
			get {
				if (tests == null || testClass == null) {
					tests = RetrieveTestMethods();
				}
				return tests;
			}
		}
		
		public DefaultTestSuite(AssemblyLoader assemblyLoader, string fullTypeName)
		{
			this.assemblyLoader = assemblyLoader;
			this.fullTypeName   = fullTypeName;
			RetrieveTestMethods();
		}
		
		public void LoadTestClass()
		{
			type      = assemblyLoader.Assembly.GetType(fullTypeName);
			testClass = assemblyLoader.Assembly.CreateInstance(fullTypeName);
		}
		
		public void UnloadTestClass()
		{
			if (testClass is IDisposable) {
				((IDisposable)testClass).Dispose();
			}
			
			testClass = null;
		}
		
		ArrayList RetrieveTestMethods()
		{
			type = assemblyLoader.Assembly.GetType(fullTypeName);
			
			ArrayList testMethods = new ArrayList();
			MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			
			foreach (MethodInfo methodInfo in methodInfos) {
				if (!type.IsAbstract && Attribute.GetCustomAttribute(methodInfo, typeof(TestMethodAttribute)) != null) {
					testMethods.Add(new DefaultTest(this, methodInfo));
				}
			}
			return testMethods;
		}
	}
}
