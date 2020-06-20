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
	/// <summary>
	/// This class handles the auto and smart indenting in the textbuffer while
	/// you type.
	/// </summary>
	public class DefaultFormattingStrategy : IFormattingStrategy
	{
		protected IDocumentAggregator document;
		
		public IDocumentAggregator Document {
			get {
				return document;
			}
			set {
				document = value;
			}
		}
		
		public DefaultFormattingStrategy()
		{
		}
		
		/// <summary>
		/// returns the whitespaces which are before a non white space character in the line line
		/// as a string.
		/// </summary>
		protected string GetIndentation(int lineNumber)
		{
			if (lineNumber < 0 || lineNumber > document.TotalNumberOfLines) {
				throw new ArgumentOutOfRangeException("lineNumber");
			}
			
			string lineText = TextUtilities.GetLineAsString(document, lineNumber);
			StringBuilder whitespaces = new StringBuilder();
			
			foreach (char ch in lineText) {
				if (Char.IsWhiteSpace(ch)) {
					whitespaces.Append(ch);
				} else {
					break;
				}
			}
			return whitespaces.ToString();
		}
		
		/// <summary>
		/// Could be overwritten to define more complex indenting.
		/// </summary>
		protected virtual int AutoIndentLine(int lineNumber)
		{
			string indentation = lineNumber != 0 ? GetIndentation(lineNumber - 1) : "";
			if(indentation.Length > 0) {
				string newLineText = indentation + TextUtilities.GetLineAsString(document, lineNumber).Trim();
				LineSegment oldLine  = document.GetLineSegment(lineNumber);
				document.Replace(oldLine.Offset, oldLine.Length, newLineText);
			}
			
			return indentation.Length;
		}
		
		/// <summary>
		/// Could be overwritten to define more complex indenting.
		/// </summary>
		protected virtual int SmartIndentLine(int line)
		{
			return AutoIndentLine(line); // smart = autoindent in normal texts
		}
		
		public virtual int FormatLine(int line, int cursorOffset, char ch)
		{
			if (ch == '\n') {
				return IndentLine(line);
			}
			return 0;
		}
		
		public int IndentLine(int line)
		{
			switch ((IndentStyle)document.Properties.GetProperty("IndentStyle", IndentStyle.Smart)) {
				case IndentStyle.None:
					break;
				case IndentStyle.Auto:
					return AutoIndentLine(line);
				case IndentStyle.Smart:
					return SmartIndentLine(line);
			}
			return 0;
		}
		
		public void IndentLines(int begin, int end)
		{
			int redocounter = 0;
			for (int i = begin; i <= end; ++i) {
				if (IndentLine(i) > 0) {
					++redocounter;
				}
			}
			if (redocounter > 0) {
				document.UndoStack.UndoLast(redocounter);
			}
		}
	}
}
