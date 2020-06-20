// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

namespace SharpDevelop.Internal.Parser {
	
	using System;
	
	/// <summary>
	/// This exception is thrown, when the parser founds an error 
	/// into a sourcefile.
	/// </summary>
	public class ParserException : Exception
	{
		int line;
		int column;
		
		public int Line {
			get {
				return line;
			}
		}
		
		public int Column {
			get {
				return column;
			}
		}
		
		public ParserException(string message, int line, int column) : base(message)
		{
			this.line   = line;
			this.column = column;
		}
	}
}
