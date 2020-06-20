﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ResolveMethodWhenFromImportIsUnknownTestFixture : ResolveTestsBase
	{
		protected override ExpressionResult GetExpressionResult()
		{
			return new ExpressionResult("methodcall", ExpressionContext.Default);
		}
		
		protected override string GetPythonScript()
		{
			return 
				"from unknown import methodcall\r\n" +
				"methodcall\r\n" +
				"\r\n";
		}
		
		[Test]
		public void ResolveResultIsNull()
		{
			Assert.IsNull(resolveResult);
		}
	}
}
