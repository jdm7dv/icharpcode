﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace RubyBinding.Tests.Parsing
{
	[TestFixture]
	public class ParseClassWithCtorTestFixture
	{
		ICompilationUnit compilationUnit;
		IClass c;
		IMethod method;
		FoldingSection methodFold;
		FoldingSection classFold;
		TextDocument document;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string ruby = "class Test\r\n" +
							"\tdef initialize\r\n" +
							"\t\tputs 'test'\r\n" +
							"\tend\r\n" +
							"end";
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			RubyParser parser = new RubyParser();
			compilationUnit = parser.Parse(projectContent, @"C:\test.rb", ruby);			
			if (compilationUnit.Classes.Count > 0) {
				c = compilationUnit.Classes[0];
				if (c.Methods.Count > 0) {
					method = c.Methods[0];
				}
				
				TextArea textArea = new TextArea();
				document = new TextDocument();
				textArea.Document = document;
				textArea.Document.Text = ruby;
				
				// Get folds.
				ParserFoldingStrategy foldingStrategy = new ParserFoldingStrategy(textArea);
				
				ParseInformation parseInfo = new ParseInformation(compilationUnit);
				foldingStrategy.UpdateFoldings(parseInfo);
				List<FoldingSection> folds = new List<FoldingSection>(foldingStrategy.FoldingManager.AllFoldings);
			
				if (folds.Count > 1) {
					classFold = folds[0];
					methodFold = folds[1];
				}
			}
		}
		
		[Test]
		public void MethodBodyRegion()
		{
			int startLine = 2;
			int startColumn = 18;
			int endLine = 4;
			int endColumn = 5;
			DomRegion region = new DomRegion(startLine, startColumn, endLine, endColumn);
			Assert.AreEqual(region.ToString(), method.BodyRegion.ToString());
		}
		
		[Test]
		public void MethodFoldMarkerInnerText()
		{
			string textInsideFold = document.GetText(methodFold.StartOffset, methodFold.Length);
			Assert.AreEqual("\r\n\t\tputs 'test'\r\n\tend", textInsideFold);
		}
		
		[Test]
		public void MethodIsConstructor()
		{
			Assert.IsTrue(method.IsConstructor);
		}
	}
}
