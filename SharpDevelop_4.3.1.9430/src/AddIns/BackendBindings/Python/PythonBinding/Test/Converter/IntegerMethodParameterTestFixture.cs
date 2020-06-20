﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Converter
{
	/// <summary>
	/// Tests that a method call is converted correctly.
	/// </summary>
	[TestFixture]
	public class IntegerMethodParameterTestFixture
	{
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    public void Print()\r\n" +
						"    {\r\n" +
						"        int i = 0;\r\n" +
						"        PrintInt(i);\r\n" +
						"    }\r\n" +
						"}";
		
		[Test]
		public void GeneratedPythonSourceCode()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter(SupportedLanguage.CSharp);
			string python = converter.Convert(csharp);
			string expectedPython = "class Foo(object):\r\n" +
				"\tdef Print(self):\r\n" +
				"\t\ti = 0\r\n" +
				"\t\tself.PrintInt(i)";
			
			Assert.AreEqual(expectedPython, python);
		}
	}
}
