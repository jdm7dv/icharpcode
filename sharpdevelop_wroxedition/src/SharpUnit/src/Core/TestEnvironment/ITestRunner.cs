/*
 * ITestRunner.cs
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
using System.Reflection;
using System.Collections;

namespace ICSharpCode.SharpUnit.Environment {
	
	/// <summary>
	/// The basic interface for the test runner. The test runner is a container
	/// for all assembly loaders and it does run the tests.
	/// </summary>
	public interface ITestRunner
	{
		/// <summary>
		/// An ArrayList which contains all assembly loaders.
		/// </summary>
		ArrayList AssemblyLoader {
			get;
		}
		
		/// <summary>
		/// This method runs a single test.
		/// </summary>
		void RunTest(ITest test);
		
		/// <summary>
		/// This method runs a all tests in all AssemblyLoaders
		/// </summary>
		void RunAllTests();
		
		/// <summary>
		/// Adds an Assembly to the runner, creates an AssemblyLoader which is
		/// put in the AssemblyLoader ArrayList.
		/// </summary>
		void AddAssembly(string fileName);
		
		/// <summary>
		/// Reloads all test suites.
		/// </summary>
		void ReloadTestSuites();
		
		/// <summary>
		/// Unloads all test suites.
		/// </summary>
		void UnloadTestSuites();
		
		event TestEventHandler TestStarted;
		event TestEventHandler TestEnded;
		
		event TestFailedEventHandler TestFailed;
		event TestEventHandler       TestSucceeded;
	}
}
