﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextTemplating;

namespace TextTemplating.Tests.Helpers
{
	public class TestableTextTemplatingFileGeneratorCustomTool : TextTemplatingFileGeneratorCustomTool
	{
		public FileProjectItem ProjectFilePassedToCreateTextTemplatingFileGenerator;
		public CustomToolContext ContextPassedToCreateTextTemplatingFileGenerator;
		public FakeTextTemplatingFileGenerator FakeTextTemplatingFileGenerator = new FakeTextTemplatingFileGenerator();
		
		protected override ITextTemplatingFileGenerator CreateTextTemplatingFileGenerator(
			FileProjectItem projectFile,
			CustomToolContext context)
		{
			ProjectFilePassedToCreateTextTemplatingFileGenerator = projectFile;
			ContextPassedToCreateTextTemplatingFileGenerator = context;
			
			return FakeTextTemplatingFileGenerator;
		}
	}
}
