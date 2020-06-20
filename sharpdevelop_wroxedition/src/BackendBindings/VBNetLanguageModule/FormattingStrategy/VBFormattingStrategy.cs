// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Drawing;
using System.Text;

using ICSharpCode.TextEditor.Document;

using ICSharpCode.Core.Properties;

namespace VBBinding.FormattingStrategy
{
	/// <summary>
	/// This class handles the auto and smart indenting in the textbuffer while
	/// you type.
	/// </summary>
	public class VBFormattingStrategy : DefaultFormattingStrategy
	{
		ArrayList statements;
		
		public VBFormattingStrategy()
		{
			statements = new ArrayList();
			statements.Add(new VBStatement("^if.*?then$", "^end if$", "End If", true));
			statements.Add(new VBStatement("class \\w+$", "^end class$", "End Class", true));
			statements.Add(new VBStatement("module \\w+$", "^end module$", "End Module", true));
			statements.Add(new VBStatement("structure \\w+$", "^end structure$", "End Structure", true));
			statements.Add(new VBStatement("^while ", "^end while$", "End While", true));
			statements.Add(new VBStatement("^select case", "^end select$", "End Select", true));
			statements.Add(new VBStatement("sub \\w+", "^end sub$", "End Sub", true));
			statements.Add(new VBStatement("property \\w+", "^end property$", "End Property", true));
			statements.Add(new VBStatement("function \\w+", "^end function$", "End Function", true));
			statements.Add(new VBStatement("for (.*?)=(.*?) to .*?$", "^next$", "Next", true));
			statements.Add(new VBStatement("^synclock .*?$", "^end synclock$", "End SyncLock", true));
			statements.Add(new VBStatement("^get$", "^end get$", "End Get", true));
			statements.Add(new VBStatement("^set\\(.*?\\)$", "^end set$", "End Set", true));
			statements.Add(new VBStatement("^try$", "^end try$", "End Try", true));
			statements.Add(new VBStatement("^for each .*? in .+?$", "^next$", "Next", true));
			statements.Add(new VBStatement("Enum .*?$", "^end enum$", "End Enum", true));
		}

		/// <summary>
		/// Define VB.net specific smart indenting for a line :)
		/// </summary>
		protected override int SmartIndentLine(int lineNr)
		{
			if (lineNr > 0) {
				LineSegment lineAbove = document.GetLineSegment(lineNr - 1);
				string lineAboveText = document.GetText(lineAbove.Offset, lineAbove.Length).Trim();
				
				LineSegment curLine = document.GetLineSegment(lineNr);
				string  curLineText = document.GetText(curLine.Offset, curLine.Length).Trim();
			}
			return AutoIndentLine(lineNr);
		}
		
		public override int FormatLine(int lineNr, int cursorOffset, char ch)
		{
			if (lineNr > 0) {
				LineSegment curLine = document.GetLineSegment(lineNr);
				LineSegment lineAbove = lineNr > 0 ? document.GetLineSegment(lineNr - 1) : null;
				
				string curLineText = document.GetText(curLine.Offset, curLine.Length).Trim();
				string lineAboveText = document.GetText(lineAbove.Offset, lineAbove.Length).Trim();
				
				if (ch == '\n') {
					foreach (VBStatement statement in statements) {
						if (Regex.IsMatch(lineAboveText, statement.StartRegex, RegexOptions.IgnoreCase)) {
							string indentation = GetIndentation(lineNr - 1);
							if (isEndStatementNeeded(statement, lineNr)) {
								document.Insert(document.Caret.Offset, "\n" + indentation + statement.EndStatement);
							}
							if (statement.IndentPlus) {
								indentation += '\t';
							}
							
							document.Replace(curLine.Offset, curLine.Length, indentation + curLineText);
							
							return indentation.Length;
						}
					}
					string indent = GetIndentation(lineNr - 1);
					if (indent.Length > 0) {
						string newLineText = indent + TextUtilities.GetLineAsString(document, lineNr).Trim();
						LineSegment oldLine = document.GetLineSegment(lineNr);
						document.Replace(oldLine.Offset, oldLine.Length, newLineText);
					}
					return indent.Length;
				}
			}
			return 0;
		}
		
		bool isEndStatementNeeded(VBStatement statement, int lineNr)
		{
			int count = 0;
			
			for (int i = 0; i < document.TotalNumberOfLines; i++) {
				LineSegment line = document.GetLineSegment(i);
				string lineText = document.GetText(line.Offset, line.Length).Trim();
				
				if (lineText.StartsWith("'")) {
					continue;
				}
				
				if (Regex.IsMatch(lineText, statement.StartRegex, RegexOptions.IgnoreCase)) {
					count++;
				} else if (Regex.IsMatch(lineText, statement.EndRegex, RegexOptions.IgnoreCase)) {
					count--;
				}
			}
			return count > 0;
		}
		
		class VBStatement
		{
			public string StartRegex   = "";
			public string EndRegex     = "";
			public string EndStatement = "";
			
			public bool IndentPlus = false;
			
			public VBStatement()
			{
			}
			
			public VBStatement(string startRegex, string endRegex, string endStatement, bool indentPlus)
			{
				StartRegex = startRegex;
				EndRegex   = endRegex;
				EndStatement = endStatement;
				IndentPlus   = indentPlus;
			}
		}
	}
}
