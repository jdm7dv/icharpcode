// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Text;

using ICSharpCode.TextEditor.Document;

using ICSharpCode.Core.Properties;

namespace JavaBinding.FormattingStrategy
{
	/// <summary>
	/// This class handles the auto and smart indenting in the textbuffer while
	/// you type.
	/// </summary>
	public class JavaFormattingStrategy : DefaultFormattingStrategy
	{
		public JavaFormattingStrategy()
		{
		}
				
		/// <summary>
		/// Define CSharp specific smart indenting for a line :)
		/// </summary>
		protected override int SmartIndentLine(int lineNr)
		{
			if (lineNr > 0) {
				LineSegment lineAbove    = document.GetLineSegment(lineNr - 1);
				string  lineAboveText = document.GetText(lineAbove.Offset, lineAbove.Length).Trim();
				
				LineSegment curLine = document.GetLineSegment(lineNr);
				string  curLineText = document.GetText(curLine.Offset, curLine.Length).Trim();
				
				if (lineAboveText.EndsWith(")") && curLineText.StartsWith("{")) {
					string indentation = GetIndentation(lineNr - 1);
					document.Replace(curLine.Offset, curLine.Length, indentation + curLineText);
					return indentation.Length;
				}
				
				if (curLineText.StartsWith("}")) { // indent closing bracket.
					int closingBracketOffset = TextUtilities.SearchBracketBackward(document, curLine.Offset + document.GetText(curLine.Offset, curLine.Length).IndexOf('}') - 1, '{', '}');
					if (closingBracketOffset == -1) {  // no closing bracket found -> autoindent
						return AutoIndentLine(lineNr);
					}
					
					string indentation = GetIndentation(document.GetLineNumberForOffset(closingBracketOffset));
					
					document.Replace(curLine.Offset, curLine.Length, indentation + curLineText);
					return indentation.Length;
				}
				
				if (lineAboveText.EndsWith(";")) { // expression ended, reset to valid indent.
					int closingBracketOffset = TextUtilities.SearchBracketBackward(document, curLine.Offset + document.GetText(curLine.Offset, curLine.Length).IndexOf('}') - 1, '{', '}');
					
					if (closingBracketOffset == -1) {  // no closing bracket found -> autoindent
						return AutoIndentLine(lineNr);
					}
					
					int closingBracketLineNr = document.GetLineNumberForOffset(closingBracketOffset);
					LineSegment closingBracketLine = document.GetLineSegment(closingBracketLineNr);
					string  closingBracketLineText = document.GetText(closingBracketLine.Offset, closingBracketLine.Length).Trim();
					
					string indentation = GetIndentation(closingBracketLineNr);
					
					// special handling for switch statement formatting.
					if (closingBracketLineText.StartsWith("switch")) {
						if (lineAboveText.StartsWith("break;") || 
						    lineAboveText.StartsWith("goto")   || 
						    lineAboveText.StartsWith("return")) {
					    } else {
					    	indentation += "\t";
					    }
					}
			    	indentation += "\t";
					
					document.Replace(curLine.Offset, curLine.Length, indentation + curLineText);
					return indentation.Length;
				}
				
				if (lineAboveText.EndsWith("{") || // indent opening bracket.
				    lineAboveText.EndsWith(":") || // indent case xyz:
				    (lineAboveText.EndsWith(")") &&  // indent single line if, for ... etc
			    	(lineAboveText.StartsWith("if") ||
			    	 lineAboveText.StartsWith("while") ||
			    	 lineAboveText.StartsWith("for"))) ||
			    	 lineAboveText.EndsWith("else")) {
			    	 	string indentation = GetIndentation(lineNr - 1) + "\t";
						document.Replace(curLine.Offset, curLine.Length, indentation + curLineText);
						return indentation.Length;
			    } else {
			    	// try to indent linewrap
			    	ArrayList bracketPos = new ArrayList();
			    	for (int i = 0; i < lineAboveText.Length; ++i) { // search for a ( bracket that isn't closed
						switch (lineAboveText[i]) {
							case '(':
								bracketPos.Add(i);
								break;
							case ')':
								if (bracketPos.Count > 0) {
									bracketPos.RemoveAt(bracketPos.Count - 1);
								}
								break;
						}
			    	}
			    	if (bracketPos.Count > 0) {
			    		int bracketIndex = (int)bracketPos[bracketPos.Count - 1];
			    		string indentation = GetIndentation(lineNr - 1);
			    		
			    		for (int i = 0; i <= bracketIndex; ++i) { // insert enough spaces to match
			    			indentation += " ";                   // brace start in the next line
			    		}
			    		
						document.Replace(curLine.Offset, curLine.Length, indentation + curLineText);
						return indentation.Length;
			    	}
			    }
			}
			return AutoIndentLine(lineNr);
		}
		
		bool NeedCurlyBracket(string text) 
		{
			int curlyCounter = 0;
			
			bool inString = false;
			bool inChar   = false;
			
			bool lineComment  = false;
			bool blockComment = false;
			
			for (int i = 0; i < text.Length; ++i) {
				switch (text[i]) {
					case '\r':
					case '\n':
						lineComment = false;
						break;
					case '/':
						if (blockComment) {
							Debug.Assert(i > 0);
							if (text[i - 1] == '*') {
								blockComment = false;
							}
						}
						if (!inString && !inChar && i + 1 < text.Length) {
							if (!blockComment && text[i + 1] == '/') {
								lineComment = true;
							}
							if (!lineComment && text[i + 1] == '*') {
								blockComment = true;
							}
						}
						break;
					case '"':
						inString = !inString;
						break;
					case '\'':
						inChar = !inChar;
						break;
					case '{':
						if (!(inString || inChar || lineComment || blockComment)) {
							++curlyCounter;
						}
						break;
					case '}':
						if (!(inString || inChar || lineComment || blockComment)) {
							--curlyCounter;
						}
						break;
				}
			}
			return curlyCounter > 0;
		}
		
		public override int FormatLine(int lineNr, int cursorOffset, char ch) // used for comment tag formater/inserter
		{
			switch (ch) {
				case '}':
				case '{':
					return Document.FormattingStrategy.IndentLine(lineNr);
				case '\n':
					if (lineNr <= 0) {
						return IndentLine(lineNr);
					}
					
					if (document.Properties.GetProperty("AutoInsertCurlyBracket", true)) {
						string oldLineText = TextUtilities.GetLineAsString(document, lineNr - 1);
						if (oldLineText.EndsWith("{")) {
							if (NeedCurlyBracket(document.TextContent)) {
								document.Insert(document.Caret.Offset, "\n}");
								IndentLine(lineNr + 1);
							}
						}
					}
					
					LineSegment    lineAbove     = document.GetLineSegment(lineNr - 1);
					string  lineAboveText = document.GetText(lineAbove.Offset, lineAbove.Length);
					
					LineSegment    curLine       = document.GetLineSegment(lineNr);
			
					LineSegment    nextLine      = lineNr + 1 < document.TotalNumberOfLines ? document.GetLineSegment(lineNr + 1) : null;
					string  nextLineText  = lineNr + 1 < document.TotalNumberOfLines ? document.GetText(nextLine.Offset, nextLine.Length) : "";
					
					if (lineAbove.HighlightSpanStack != null && lineAbove.HighlightSpanStack.Count > 0) {				
						if (!((Span)lineAbove.HighlightSpanStack.Peek()).StopEOL) {	// case for /* style comments
							int index = lineAboveText.IndexOf("/*");
							
							if (index > 0) {
								string indentation = GetIndentation(lineNr - 1);
								for (int i = indentation.Length; i < index; ++ i) {
									indentation += ' ';
								}
								document.Replace(curLine.Offset, cursorOffset - curLine.Offset, indentation + " * ");
								return indentation.Length + 3;
							}
							
							index = lineAboveText.IndexOf("*");
							if (index > 0) {
								string indentation = GetIndentation(lineNr - 1);
								for (int i = indentation.Length; i < index; ++ i) {
									indentation += ' ';
								}
								document.Replace(curLine.Offset, cursorOffset - curLine.Offset, indentation + "* ");
								return indentation.Length + 2;
							}
						}
					}
					return IndentLine(lineNr);
			}
			return 0;
		}
	}
}
