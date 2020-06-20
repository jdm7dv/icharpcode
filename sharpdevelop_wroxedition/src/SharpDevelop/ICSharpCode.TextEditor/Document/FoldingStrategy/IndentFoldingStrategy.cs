// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Collections;

namespace ICSharpCode.TextEditor.Document
{
	public class IndentFoldingStrategy : IFoldingStrategy
	{
		public int CalculateFoldLevel(IDocumentAggregator document, int lineNumber)
		{
			LineSegment line = document.GetLineSegment(lineNumber);
			int foldLevel = 0;
			
			while (document.GetCharAt(line.Offset + foldLevel) == '\t' && foldLevel + 1  < line.TotalLength) {
				++foldLevel;
			}
			
			return foldLevel;
		}
	}
}
