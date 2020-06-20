﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Description of XamlExpressionFinder.
	/// </summary>
	public class XamlExpressionFinder : IExpressionFinder
	{
		public static readonly XamlExpressionFinder Instance = new XamlExpressionFinder();

		static bool IsValidChar(char c)
		{
			return char.IsLetterOrDigit(c) || c == '_' || c == ':' || c == '.';
		}

		public ExpressionResult FindExpression(string text, int offset)
		{
			int pos = offset - 1;
			while (pos > 0 && IsValidChar(text[pos])) {
				pos--;
			}
			pos++;
			return new ExpressionResult(text.Substring(pos, offset - pos), GetContext(text, offset));
		}

		public ExpressionResult FindFullExpression(string text, int offset)
		{
			int start = offset - 1;
			while (start > 0 && IsValidChar(text[start])) {
				start--;
			}
			start++;
			while (offset < text.Length && IsValidChar(text[offset])) {
				offset++;
			}
			
			var startLocation = Utils.GetLocationInfoFromOffset(text, start);
			var endLocation = Utils.GetLocationInfoFromOffset(text, offset);
			
			return new ExpressionResult(text.Substring(start, offset - start), GetContext(text, offset)) { Region = new DomRegion(startLocation.Line, startLocation.Column, endLocation.Line, endLocation.Column) };
		}

		public string RemoveLastPart(string expression)
		{
			return "";
		}

		static ExpressionContext GetContext(string text, int offset)
		{
			XmlElementPath path = XmlParser.GetActiveElementStartPathAtIndex(text, offset);
			if (path == null || path.Elements.Count == 0) {
				if (offset > 0 && text[offset - 1] == '<')
					return XamlExpressionContext.Empty;
				else
					return ExpressionContext.Default;
			}
			string attributeName = XmlParser.GetAttributeNameAtIndex(text, offset);
			if (!string.IsNullOrEmpty(attributeName)) {
				return new XamlExpressionContext(path, attributeName, XmlParser.IsInsideAttributeValue(text, offset));
			}
			else {
				return new XamlExpressionContext(path, null, false);
			}
		}
	}
}
