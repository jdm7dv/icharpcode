/*
 * ITestSuite.cs
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
	/// This is the basic collection of tests.
	/// (e.g. a testclass)
	/// </summary>
	public interface ITestSuite
	{
		/// <summary>
		/// The type in which the testmethods are.
		/// </summary>
		Type Type {
			get;
		}
		
		/// <summary>
		/// The full type name of the type.
		/// </summary>
		string FullTypeName {
			get;
		}
		
		/// <summary>
		/// Description of this testsuite, may be <code>null</code> if no
		/// description is given.
		/// </summary>
		string Description {
			get;
		}
		
		/// <summary>
		/// An object of type <code>Type</code>
		/// </summary>
		object TestClass {
			get;
		}
		
		/// <summary>
		/// An ArrayList containing all Tests in this suite.
		/// </summary>
		ArrayList Tests {
			get;
		}
		
		/// <summary>
		/// Loads the testclass. Initializes type, creates the TestClass object.
		/// </summary>
		void LoadTestClass();
		
		/// <summary>
		/// Unloads the testclass, after this call TestClass == null and Type == null
		/// only the FullTypeName is valid.
		/// </summary>
		void UnloadTestClass();
	}
}
