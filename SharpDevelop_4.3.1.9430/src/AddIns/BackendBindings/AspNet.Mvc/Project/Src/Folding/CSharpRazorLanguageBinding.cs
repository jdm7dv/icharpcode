﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AspNet.Mvc.Folding
{
	public class CSharpRazorLanguageBinding : HtmlLanguageBinding
	{
		public CSharpRazorLanguageBinding()
			: base(
				new TextEditorWithParseInformationFoldingFactory(),
				new RazorFoldGeneratorFactory(".cshtml"))
		{
		}
	}
}
