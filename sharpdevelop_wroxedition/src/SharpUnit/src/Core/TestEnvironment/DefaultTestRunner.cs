/*
 * DefaultTestRunner.cs
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
using System.Collections;
using System.Windows.Forms;

namespace ICSharpCode.SharpUnit.Environment {
	
	public class UnloadTestSuitException : Exception
	{
		public UnloadTestSuitException(ITestSuite suite, Exception e) : base("Got exception while unload " + suite.FullTypeName,e)
		{
			
		}
	}
	
	/// <summary>
	/// 
	/// </summary>
	public class DefaultTestRunner : MarshalByRefObject, ITestRunner
	{
		ArrayList assemblyLoader = new ArrayList();
		
		public ArrayList AssemblyLoader {
			get {
				return assemblyLoader;
			}
		}
		
		public void AddAssembly(string name)
		{
			assemblyLoader.Add(new AssemblyLoader(name));
		}
		
		public Assembly MyResolveEventHandler(object sender, ResolveEventArgs e)
		{
			string[] assemblyDescr = e.Name.Split(new char[] {','});
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				if (assembly.FullName == e.Name) {
					return assembly;
				}
			}
			
			foreach (AssemblyLoader loader in assemblyLoader) {
				foreach (string path in loader.AssemblyLookupPaths) {
					string dllName = path + Path.DirectorySeparatorChar + assemblyDescr[0] + ".dll";
					string exeName = path + Path.DirectorySeparatorChar + assemblyDescr[0] + ".exe";
					
					if (File.Exists(dllName)) {
						return AssemblyUtilities.LoadAssembly(dllName);
					}
					
					if (File.Exists(exeName)) {
						return AssemblyUtilities.LoadAssembly(exeName);
					}
				}
			}
			
			return null;
		}
		
		public DefaultTestRunner() 
		{
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);
		}
		
		public void RunTest(ITest test)
		{
			OnTestStarted(new TestEventArgs(test));
			try {
				test.Run();
				OnTestSucceeded(new TestEventArgs(test));
			} catch (Exception e) {
				if (e.InnerException != null) {
					OnTestFailed(new TestFailedEventArgs(test, e.InnerException));
				} else {
					OnTestFailed(new TestFailedEventArgs(test, e));
				}
			}
			OnTestEnded(new TestEventArgs(test));			
		}
		
		public void RunAllTests()
		{
			foreach (AssemblyLoader loader in assemblyLoader) {
				foreach (ITestSuite suite in loader.TestSuites) {
					try {
						suite.LoadTestClass();
					} catch (Exception e) {
						Console.WriteLine("Failed to create test suite '" + suite.FullTypeName + "'" + e.InnerException.GetType().Name + (e.InnerException.Message != null ? " : " + e.InnerException.Message : "") + "\n");
						continue;
					}
					ArrayList tests = suite.Tests;
					foreach (ITest test in tests) {
						RunTest(test);
					}
				}
			}
			try {
				UnloadTestSuites();
			} catch (Exception e) {
				Console.WriteLine("\nGot exception while unloading " + e.GetType().Name + (e.Message != null ? " : " + e.Message : "") + "\n");
				Console.WriteLine("Original exception " + e.InnerException.GetType().Name + (e.InnerException.Message != null ? " : " + e.InnerException.Message : "") + "\n");
			}
		}
		
		public void ReloadTestSuites()
		{
			foreach (AssemblyLoader loader in assemblyLoader) {
				loader.ReloadAssembly();
				foreach (ITestSuite suite in loader.TestSuites) {
					suite.LoadTestClass();
				}
			}
		}
		
		public void UnloadTestSuites()
		{
			foreach (AssemblyLoader loader in assemblyLoader) {
				foreach (ITestSuite suite in loader.TestSuites) {
					try {
						suite.UnloadTestClass();
						GC.Collect();
					} catch (Exception e) {
						throw new UnloadTestSuitException(suite, e);
					}
				}
				loader.Assembly = null;
			}
			GC.Collect();
		}
		
		void OnTestStarted(TestEventArgs e)
		{
			if (TestStarted != null) {
				TestStarted(this, e);
			}
		}
		
		void OnTestEnded(TestEventArgs e)
		{
			if (TestEnded != null) {
				TestEnded(this, e);
			}
		}

		void OnTestFailed(TestFailedEventArgs e)
		{
			if (TestFailed != null) {
				TestFailed(this, e);
			}
		}
		
		void OnTestSucceeded(TestEventArgs e)
		{
			if (TestSucceeded != null) {
				TestSucceeded(this, e);
			}
		}

		public event TestEventHandler TestStarted;
		public event TestEventHandler TestEnded;
		
		public event TestFailedEventHandler TestFailed;
		public event TestEventHandler       TestSucceeded;
	}
}
