// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.Text;

using ICSharpCode.Core.Properties;

namespace ICSharpCode.TextEditor.Document
{
	public interface IHighlightingStrategy
	{
		string Name {
			get;
		}
		
		string[] Extensions {
			set;
			get;
		}
		
		// returns special color. (BackGround Color, Cursor Color and so on)
		HighlightColor   GetColorFor(string name);
		HighlightRuleSet GetRuleSet(Span span);
		HighlightColor   GetColor(IDocumentAggregator document, LineSegment keyWord, int index, int length);
		
		void MarkTokens(IDocumentAggregator document, LineSegmentCollection lines);
		void MarkTokens(IDocumentAggregator document);
	}
}
