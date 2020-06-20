﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Converter
{
	/// <summary>
	/// Tests that all the binary operators are converted to
	/// Ruby correctly.
	/// </summary>
	[TestFixture]
	public class BinaryOperatorConversionTests
	{
		string csharp = "class Foo\r\n" +
						"{\r\n" +
						"    public int Run(i)\r\n" +
						"    {\r\n" +
						"        if (i BINARY_OPERATOR 0) {\r\n" +
						"            return 10;\r\n" +
						"        }\r\n" +
						"        return 0;\r\n" +
						"    }\r\n" +
						"}";
		
		[Test]
		public void GreaterThan()
		{
			Assert.AreEqual(">", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.GreaterThan));
		}

		[Test]
		public void NotEqual()
		{
			Assert.AreEqual("!=", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.InEquality));
		}
		
		[Test]
		public void Divide()
		{
			Assert.AreEqual("/", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.Divide));
		}
		
		[Test]
		public void LessThan()
		{
			Assert.AreEqual("<", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.LessThan));
		}

		[Test]
		public void Equals()
		{
			string code = GetCode(@"==");
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp);
			converter.IndentString = "    ";
			string RubyCode = converter.Convert(code);
			string expectedRubyCode = "class Foo\r\n" +
						"    def Run(i)\r\n" +
						"        if i == 0 then\r\n" +
						"            return 10\r\n" +
						"        end\r\n" +
						"        return 0\r\n" +
						"    end\r\n" +
						"end";
			Assert.AreEqual(expectedRubyCode, RubyCode);
		}

		[Test]
		public void LessThanOrEqual()
		{
			Assert.AreEqual("<=", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.LessThanOrEqual));
		}

		[Test]
		public void GreaterThanOrEqual()
		{
			Assert.AreEqual(">=", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.GreaterThanOrEqual));
		}
		
		[Test]
		public void Add()
		{
			Assert.AreEqual("+", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.Add));
		}
		
		[Test]
		public void Multiply()
		{
			Assert.AreEqual("*", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.Multiply));
		}
		
		[Test]
		public void BitwiseAnd()
		{
			Assert.AreEqual("&", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.BitwiseAnd));
		}

		[Test]
		public void BitwiseOr()
		{
			Assert.AreEqual("|", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.BitwiseOr));
		}

		[Test]
		public void BooleanAnd()
		{
			Assert.AreEqual("and", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.LogicalAnd));
		}

		[Test]
		public void BooleanOr()
		{
			Assert.AreEqual("or", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.LogicalOr));
		}
		
		[Test]
		public void BooleanXor()
		{
			Assert.AreEqual("^", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.ExclusiveOr));
		}		

		[Test]
		public void Modulus()
		{
			Assert.AreEqual("%", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.Modulus));
		}

		[Test]
		public void Subtract()
		{
			Assert.AreEqual("-", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.Subtract));
		}
		
		[Test]
		public void DivideInteger()
		{
			Assert.AreEqual("/", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.DivideInteger));
		}

		[Test]
		public void ReferenceEquality()
		{
			Assert.AreEqual("is", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.ReferenceEquality));
		}
		
		[Test]
		public void BitShiftRight()
		{
			Assert.AreEqual(">>", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.ShiftRight));
		}
		
		[Test]
		public void BitShiftLeft()
		{
			Assert.AreEqual("<<", NRefactoryToRubyConverter.GetBinaryOperator(BinaryOperatorType.ShiftLeft));
		}
	
		/// <summary>
		/// Gets the C# code with the binary operator replaced with the
		/// specified string.
		/// </summary>
		string GetCode(string op)
		{
			return csharp.Replace("BINARY_OPERATOR", op);
		}
	}
}
