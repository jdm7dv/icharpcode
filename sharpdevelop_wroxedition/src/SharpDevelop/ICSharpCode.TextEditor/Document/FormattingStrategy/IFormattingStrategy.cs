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
	/// This interface handles the auto and smart indenting and formating
	/// in the document while  you type. Language bindings could overwrite this 
	/// interface and define their own indentation/formating.
	/// </summary>
	public interface IFormattingStrategy
	{
		/// <summary>
		/// The document which the formating strategy is attached to.
		/// </summary>
		IDocumentAggregator Document {
			get;
			set;
		}
			
		/// <summary>
		/// This function formats a specific line after <code>ch</code> is pressed.
		/// </summary>
		/// <returns>
		/// the caret delta position the caret will be moved this number
		/// of bytes (e.g. the number of bytes inserted before the caret, or
		/// removed, if this number is negative)
		/// </returns>
		int FormatLine(int line, int caretOffset, char charTyped);
		
		/// <summary>
		/// This function sets the indentation level in a specific line
		/// </summary>
		/// <returns>
		/// the number of inserted characters.
		/// </returns>
		int IndentLine(int line);
		
		/// <summary>
		/// This function sets the indentlevel in a range of lines.
		/// </summary>
		void IndentLines(int begin, int end);
	}	
}
