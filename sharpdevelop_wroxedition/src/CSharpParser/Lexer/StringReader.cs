// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;

namespace SharpDevelop.Internal.Parser {
	
	public class StringReader : IReader
	{
		string data = null;
		int    ptr  = 0;
		
		public StringReader(string data)
		{
			this.data = data;
		}
		
		public char GetNext()
		{
			if (Eos()) {
				throw new ParserException("warning : FileReader.GetNext : Read char over eos.", 0, 0);
			}
			return data[ptr++];
		}
		
		public char Peek()
		{
			if (Eos()) {
				throw new ParserException("warning : FileReader.Peek : Read char over eos.", 0, 0);
			}
			return data[ptr];
		}
		
		public void UnGet()
		{
			--ptr;
			if (ptr < 0) {
				throw new ParserException("ungetted first char", 0, 0);
			}
		}
		
		public bool Eos()
		{
			return ptr >= data.Length;
		}
	}
}
