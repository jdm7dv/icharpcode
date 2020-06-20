﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Dom;
using System;
using NUnit.Framework;

namespace ICSharpCode.XamlBinding.Tests
{
	[TestFixture]
	public class XamlExpressionFinderTests
	{
		XamlExpressionContext GetXamlContext(string text)
		{
			return (XamlExpressionContext)GetContext(text);
		}

		ExpressionContext GetContext(string text)
		{
			return XamlExpressionFinder.Instance.FindExpression(text, text.Length).Context;
		}

		[Test]
		public void FindContextAfterElementName()
		{
			XamlExpressionContext c = GetXamlContext("<Window><Grid");
			Assert.AreEqual(2, c.ElementPath.Elements.Count);
			Assert.AreEqual("Window > Grid", c.ElementPath.ToString());
			Assert.IsNull(c.AttributeName);
			Assert.IsFalse(c.InAttributeValue);
		}

		[Test]
		public void FindContextAtElementStart()
		{
			XamlExpressionContext c = GetXamlContext("<Window><");
			Assert.AreEqual(0, c.ElementPath.Elements.Count);
			Assert.IsNull(c.AttributeName);
			Assert.IsFalse(c.InAttributeValue);
		}
	}
}
