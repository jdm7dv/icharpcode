﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Reflection;
using System.Reflection.Emit;
using IronPython.Runtime;
using ICSharpCode.Python.Build.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Runtime;
using NUnit.Framework;

namespace Python.Build.Tasks.Tests
{
	/// <summary>
	/// If the filename returned from the SyntaxErrorException cannot be found in the project then
	/// just use the project's folder concatenated with the filename.
	/// </summary>
	[TestFixture]
	public class SyntaxErrorUnknownFileNameTestFixture
	{
		MockPythonCompiler mockCompiler;
		DummyPythonCompilerTask compiler;
		bool success;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			ScriptEngine engine = IronPython.Hosting.Python.CreateEngine();
		}
		
		[SetUp]
		public void Init()
		{
			mockCompiler = new MockPythonCompiler();
			compiler = new DummyPythonCompilerTask(mockCompiler, @"D:\Projects\MyProject");
			compiler.TargetType = "Exe";
			compiler.OutputAssembly = "test.exe";
			
			TaskItem sourceFile = new TaskItem(@"D:\Projects\MyProject\test.py");
			compiler.Sources = new ITaskItem[] {sourceFile};
			
			SourceUnit source = DefaultContext.DefaultPythonContext.CreateSourceUnit(NullTextContentProvider.Null, @"test\unknown", SourceCodeKind.InteractiveCode);
			
			SourceLocation start = new SourceLocation(0, 1, 1);
			SourceLocation end = new SourceLocation(0, 2, 3);
			SourceSpan span = new SourceSpan(start, end);
			SyntaxErrorException ex = new SyntaxErrorException("Error", source, span, 1000, Severity.FatalError);
			mockCompiler.ThrowExceptionAtCompile = ex;
			
			success = compiler.Execute();
		}

		[Test]
		public void SourceFile()
		{
			Assert.AreEqual(@"D:\Projects\MyProject\test.unknown.py", compiler.LoggedErrorFile);
		}
	}
}
