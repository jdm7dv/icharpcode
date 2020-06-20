// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

namespace SharpDevelop.Internal.Parser {
	
	public class TokenList
	{
		class Element
		{
			public Token   Value;
			public Element Next;
			
			public Element(Token t)
			{
				Value = t;
			}
		}
		
		Element me;
		static Token EOF = new Token(TokenType.EOF, null);
		Element first;
		Element last;
		int count = 0;
		Lexer lexer;
		bool isFinished = false;
		
		public TokenList(Lexer l)
		{
			lexer = l;
			first = last = me = new Element(lexer.Next());
			if (me.Equals(EOF)) {
				isFinished = true;
			}
			count = 1;
		}
		
		bool Add()
		{
			if (isFinished) {
				return false;
			}
			last.Next = new Element(lexer.Next());
			last = last.Next;
			++count;
			if (last.Equals(EOF)) {
				isFinished = true;
			}
			return true;
		}
		
		public Token Next()
		{
			if (count < 1) {
				return null;
			}
			if (count < 2) {
				Add();
			}
			Element erg = first;
			first = first.Next;
			--count;
			return erg.Value;
		}
		
		public Token Next(int witch)
		{
			if (count-1 < witch) {
				for (int i = count - 1; i < witch - 1; ++i) {
					Add();
				}
				if (Add()) {
					return last.Value;
				} else {
					return null;
				}
			} else {
				Element erg = first;
				for (int i = 0; i < witch; ++i) {
					erg = erg.Next;
				}
				return erg.Value;
			}
		}
	}
}
