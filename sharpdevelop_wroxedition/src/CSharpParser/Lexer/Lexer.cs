// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Globalization;
using System.Text;

namespace SharpDevelop.Internal.Parser {
	
	public class Lexer
	{
		public KeyWord keywords;
		IReader reader;
		int col = 1;
		int line = 1;
		
		CommentCollection miscComments = new CommentCollection();
		CommentCollection dokuComments = new CommentCollection();
		TagCollection tagComments = new TagCollection();
		string[] tags;
		
		public CommentCollection MiscComments { 
			get {
				return miscComments;
			}
		}
		
		public CommentCollection DokuComments { 
			get {
				return dokuComments;
			}
		}
		
		public TagCollection TagComments {
			get {
				return tagComments;
			}
		}
		
		public string[] Tags {
			get {
				return tags;
			}
			set {
				tags = value;
			}
		}
		
		public Lexer(IReader reader)
		{
			this.reader = reader;
			keywords = new KeyWord();
		}
		
		public Token Next()
		{
			while (!reader.Eos()) {
				char ch = reader.GetNext();
				if (Char.IsWhiteSpace(ch)) {
					++col;
					if (ch == '\n') {
						++line;
						col = 1;
					}
					continue;
				}
				if (Char.IsLetter(ch) || ch == '_') {
					int x = col;
					int y = line;
					string s = ReadIdent(ch);
					if (keywords.IsKeyWord(s)) {
						return new Token(TokenType.Keyword, keywords.GetKeyWord(s), x, y);
					}
					return new Token(TokenType.Identifier, s, x, y);
				}
				if (Char.IsDigit(ch)) {
					return ReadDigit(ch, col);
				}
				if (ch == '/') {
					if (reader.Peek() == '/' || reader.Peek() == '*') {
						++col;
						ReadComment();
						continue;
					}
				}
				if (ch == '"') {
					++col;
					return new Token(TokenType.String, ReadString(), col, line);
				}
				if (ch == '\'') {
					++col;
					return new Token(TokenType.Char, ReadChar(), col, line);
				}
				if (ch == '#') {
					++col;
					while (!reader.Eos()) {
						ch = reader.GetNext();
						++col;
						if (ch == '\n') {
							++line;
							col = 1;
							break;
						}
					}
					continue;
				}
				object o;
				if (!false.Equals(o = ReadOperator(ch))) {
					return (Token)o;
				}
				if (!false.Equals(o = ReadPunctuation(ch))) {
					return (Token)o;
				}
				if (ch == '@') {
					int x = col;
					int y = line;
					ch = reader.GetNext();
					++col;
					if (ch == '"') {
						return new Token(TokenType.String, ReadVerbatimString(), x, y);
					}
					if (Char.IsLetterOrDigit(ch)) {
						return new Token(TokenType.Identifier, ReadIdent(ch), x, y);
					}
				}
				throw new ParserException("Error: Unknowen char not read at (" + col + "/" + line + ") in Lexer.Next()\nIt was: " + ch, line, col);
			}
			
			return new Token(TokenType.EOF, null, col, line);
		}
		
		string ReadIdent(char ch) {
			StringBuilder s = new StringBuilder(ch.ToString());
			++col;
			while (!reader.Eos() && (Char.IsLetterOrDigit(ch = reader.GetNext()) || ch == '_')) {
				s.Append(ch.ToString());
				++col;
			}
			reader.UnGet();
			return s.ToString();
		}
		
		Token ReadDigit(char ch, int x)
		{
			int y = line;
			++col;
			string digit = "" + ch;
			
			bool ishex      = false;
			bool isunsigned = false;
			bool islong     = false;
			bool isfloat    = false;
			bool isdouble   = false;
			bool isdecimal  = false;
			
			if (ch == '0' && Char.ToUpper(reader.Peek()) == 'X') {
				const string hex = "0123456789ABCDEF";
				reader.GetNext(); // skip 'x'
				++col;
				while (hex.IndexOf(Char.ToUpper(reader.Peek())) != -1) {
					digit += Char.ToUpper(reader.GetNext());
					++col;
				}
				ishex = true;
			} else {
				while (Char.IsDigit(reader.Peek())) {
					digit += reader.GetNext();
					++col;
				}
			}
			
			if (reader.Peek() == '.') { // read floating point number
				isdouble = true; // double is default
				if (ishex) {
					throw new ParserException("No hexadecimal floating point values allowed (" + col + "/" + line + ")", line, col);
				}
				digit += reader.GetNext();
				++col;
				
				while (Char.IsDigit(reader.Peek())){ // read decimal digits beyond the dot
					digit += reader.GetNext();
					++col;
				}
			}
			
			if (Char.ToUpper(reader.Peek()) == 'E') { // read exponent
				isdouble = true;
				digit +=  reader.GetNext();
				++col;
				if (reader.Peek() == '-' || reader.Peek() == '+') {
					digit += reader.GetNext();
					++col;
				}
				while (Char.IsDigit(reader.Peek())) { // read exponent value
					digit += reader.GetNext();
					++col;
				}
				isunsigned = true;
			}
			
			if (Char.ToUpper(reader.Peek()) == 'F') { // float value
				reader.GetNext();
				++col;
				isfloat = true;
			} else if (Char.ToUpper(reader.Peek()) == 'M') { // double type suffix (obsolete, double is default)
				reader.GetNext();
				++col;
				isdouble = true;
			} else if (Char.ToUpper(reader.Peek()) == 'D') { // decimal value
				reader.GetNext();
				++col;
				isdecimal = true;
			} else if (!isdouble) {
				if (Char.ToUpper(reader.Peek()) == 'U') {
					reader.GetNext();
					++col;
					isunsigned = true;
				}
				
				if (Char.ToUpper(reader.Peek()) == 'L') {
					reader.GetNext();
					++col;
					islong = true;
					if (!isunsigned && Char.ToUpper(reader.Peek()) == 'U') {
						reader.GetNext();
						++col;
						isunsigned = true;
					}
				}
			}
			
			if (isfloat) {
				return new Token(TokenType.Float, Single.Parse(digit), x, y);
			}
			if (isdecimal) {
				return new Token(TokenType.Decimal, Decimal.Parse(digit), x, y);
			}
			if (isdouble) {
				return new Token(TokenType.Double, Double.Parse(digit), x, y);
			}
			if (islong) {
				if (isunsigned) {
					return new Token(TokenType.ULong, UInt64.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number), x, y);
				} else {
					return new Token(TokenType.Long, Int64.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number), x, y);
				}
			} else {
				if (isunsigned) {
					return new Token(TokenType.UInt, UInt32.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number), x, y);
				} else {
					return new Token(TokenType.Int, Int32.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number), x, y);
				}
			}
		}
		
		string ReadString()
		{
			char ch = '\0';
			StringBuilder s = new StringBuilder();
			while (!reader.Eos() && ((ch = reader.GetNext()) != '"')) {
				++col;
				if (ch == '\\') {
					s.Append(ReadEscapeSequence());
				} else if (ch == '\n') {
					throw new ParserException("No return allowed in String at (" + col + "/" + line + ")", line, col);
				} else {
					s.Append(ch);
				}
			}
			if (ch != '"') {
				throw new ParserException("End of File before String terminated at (" + col + "/" + line + ")", line, col);
			}
			return s.ToString();
		}
		
		string ReadVerbatimString()
		{
			char ch = '\0';
			string s = "";
			while (!reader.Eos() && (ch = reader.GetNext()) != '"') {
				++col;
				if (ch == '\n') {
					++line;
					col = 1;
				}
				s += ch;
			}
			if (ch != '"') {
				throw new ParserException("End of File before String terminated at (" + col + "/" + line + ")", line, col);
			}
			return s;
		}
		
		string hexdigits = "0123456789ABCDEF";
		char ReadEscapeSequence()
		{
			if (reader.Eos()) {
				throw new ParserException("End of File before EscapeSequence terminated at (" + col + "/" + line + ")", line, col);
			}
			char ch = reader.GetNext();
			++col;
			switch (ch)  {
				case '\'':
					return '\'';
				case '\"':
					return '\"';
				case '\\':
					return '\\';
				case '0':
					return '\0';
				case 'a':
					return '\a';
				case 'b':
					return '\b';
				case 'f':
					return '\f';
				case 'n':
					return '\n';
				case 'r':
					return '\r';
				case 't':
					return '\t';
				case 'v':
					return '\v';
				case 'u':
				case 'x':
					string number = reader.GetNext().ToString();
					for (int i = 0; i < 3; ++i) {
						ch = reader.GetNext();
						if (hexdigits.IndexOf(Char.ToUpper(ch)) > 0) {
							number += ch;
						} else {
							reader.UnGet();
							break;
						}
					}
					return (char)Byte.Parse(number, NumberStyles.HexNumber);
				default:
					throw new ParserException("Unknown escape sequence " + ch, line, col);
			}
		}
		
		char ReadChar()
		{
			if (reader.Eos()) {
				throw new ParserException("End of File before char terminated at (" + col + "/" + line + ")", line, col);
			}
			char ch = reader.GetNext();
			++col;
			
			if (ch == '\\') {
				ch = ReadEscapeSequence();
			}
			if (reader.Eos()) {
				throw new ParserException("End of File before char terminated at (" + col + "/" + line + ")", line, col);
			}
			if (reader.GetNext() != '\'') {
				throw new ParserException("char not terminated at (" + col + "/" + line + ")", line, col);
			}
			return ch;
		}
		
		void ReadComment()
		{
			char ch = reader.GetNext();
			++col;
			switch (ch) {
				case '*':
					ReadMoreComment();
				break;
				case '/':
					switch (ch = reader.GetNext()) {
						case '/':
							++col;
							ReadDocumentationComment();
							break;
						default:
							Read1LineComment(ch);
						break;
					}
					break;
				default:
					throw new ParserException("Error while reading Comment at (" + col + "/" + line + ")", line, col);
			}
		}
		
		void ReadDocumentationComment()
		{
			int x = col - 3;
			int y = line;
			string s = "";
			char ch;
			while (!reader.Eos()) {
				ch = reader.GetNext();
				if (ch == '\n') {
					++line;
					col = 1;
					break;
				}
				s += ch;
				++col;
			}
			if (reader.Eos()) {
				throw new ParserException("Error: Eof detected in DokuComment at (" + col + "/" + line + ")", line, col);
			}
			dokuComments.Add(new Comment(new DefaultRegion(y, x, line, col), s));
		}
		
		void Read1LineComment(char ch)
		{
			StringBuilder comment = new StringBuilder(ch.ToString());
			int x = col;
			int y = line;
			while (!reader.Eos()) {
				if (ch == '\n') {
					miscComments.Add(new Comment(new DefaultRegion(y, x, line, col), comment.ToString()));
					++line;
					col = 1;
					return;
				}
				if (Char.IsLetter(ch)) {
					miscComments.Add(new Comment(new DefaultRegion(y, x, line, col), comment.ToString()));
					int yTag = line;
					int xTag = col;
					string s = ReadIdent(ch);
					if (tags != null) {
						for (int i = 0; i < tags.Length; ++i) {
							if (s == tags[i]) {
								Tag tag = new Tag(s, new DefaultRegion(yTag, xTag));
								tag.CommentString = (string)(ReadTagComment(true)[0]);
								tagComments.Add(tag);
								tag.Region.EndLine = line;
								tag.Region.EndColumn = col;
								return;
							}
						}
					}
				}
				ch = reader.GetNext();
				++col;
			}
		}
		
		object[] ReadTagComment(bool lineComment)
		{
			object[] ret = new object[2];
			StringBuilder s = new StringBuilder();
			char ch;
			if (lineComment) {
				while (!reader.Eos() && (ch = reader.GetNext()) != '\n') {
					s.Append(ch.ToString());
					++col;
				}
				++line;
				col = 1;
				ret[0] = s.ToString();
				ret[1] = false;
				return ret;
			}
			while (!reader.Eos() && (ch = reader.GetNext()) != '\n') {
				if (ch == '*') {
					if (reader.Peek() == '/') {
						col += 2;
						reader.GetNext();
						ret[0] = s.ToString();
						ret[1] = true;
						return ret;
					}
				}
				++col;
				s.Append(ch.ToString());
			}
			++line;
			col = 1;
			ret[0] = s.ToString();
			ret[1] = false;
			return ret;
		}
		
		void ReadMoreComment()
		{
			StringBuilder comment = new StringBuilder();
			int x = col;
			int y = line;
			while (!reader.Eos()) {
				char ch;
				switch (ch = reader.GetNext()) {
					case '\n':
						comment.Append("\n");
						++line;
						col = 1;
						break;
					case '*':
						++col;
						switch (reader.Peek()) {
							case '/':
								reader.GetNext();
								++col;
								miscComments.Add(new Comment(new DefaultRegion(y, x, line, col - 2), comment.ToString()));
								return;
							default:
								comment.Append("*" + reader.Peek().ToString());
								continue;
						}
					default:
						if (Char.IsLetter(ch)) {
							int yTag = line;
							int xTag = col;
							string s = ReadIdent(ch);
							if (tags != null) {
								for (int i = 0; i < tags.Length; ++i) {
									if (s == tags[i]) {
										Tag tag = new Tag(s, new DefaultRegion(yTag, xTag));
										object[] o = ReadTagComment(false);
										tag.CommentString = (string)(o[0]);
										tagComments.Add(tag);
										if ((bool)(o[1])) {
											return;
										}
									}
								}
							}
							comment.Append(s);
						} else {
							comment.Append(ch);
							++col;
						}
						break;
				}
			}
		}
		
		object ReadOperator(char ch)
		{
			int x = col;
			int y = line;
			++col;
			switch(ch) {
				case '+':
					switch (reader.GetNext()) {
						case '+':
							++col;
							return new Token(TokenType.Operator, OperatorType.Increment, x, y);
						case '=':
							++col;
							return new Token(TokenType.Operator, OperatorType.PlusEqual, x, y);
						default:
							reader.UnGet();
							return new Token(TokenType.Operator, OperatorType.Plus, x, y);
					}
				case '-':
					switch (reader.GetNext()) {
						case '-':
							++col;
							return new Token(TokenType.Operator, OperatorType.Decrement, x, y);
						case '=':
							++col;
							return new Token(TokenType.Operator, OperatorType.MinusEqual, x, y);
						case '>':
							++col;
							return new Token(TokenType.Operator, OperatorType.Dereference, x, y);
						default:
							reader.UnGet();
							return new Token(TokenType.Operator, OperatorType.Minus, x, y);
					}
				case '*':
					switch (reader.GetNext()) {
						case '=':
							++col;
							return new Token(TokenType.Operator, OperatorType.MulEqual, x, y);
						default:
							reader.UnGet();
							return new Token(TokenType.Operator, OperatorType.Mul, x, y);
					}
				case '/':
					switch (reader.GetNext()) {
						case '=':
							++col;
							return new Token(TokenType.Operator, OperatorType.DivEqual, x, y);
						default:
							reader.UnGet();
							return new Token(TokenType.Operator, OperatorType.Div, x, y);
					}
				case '%':
					switch (reader.GetNext()) {
						case '=':
							++col;
							return new Token(TokenType.Operator, OperatorType.ModEqual, x, y);
						default:
							reader.UnGet();
							return new Token(TokenType.Operator, OperatorType.Mod, x, y);
					}
				case '&':
					switch (reader.GetNext()) {
						case '&':
							++col;
							return new Token(TokenType.Operator, OperatorType.And, x, y);
						case '=':
							++col;
							return new Token(TokenType.Operator, OperatorType.AndEqual, x, y);
						default:
							reader.UnGet();
							return new Token(TokenType.Operator, OperatorType.BitwiseAnd, x, y);
					}
				case '|':
					switch (reader.GetNext()) {
						case '|':
							++col;
							return new Token(TokenType.Operator, OperatorType.Or, x, y);
						case '=':
							++col;
							return new Token(TokenType.Operator, OperatorType.OrEqual, x, y);
						default:
							reader.UnGet();
							return new Token(TokenType.Operator, OperatorType.BitwiseOr, x, y);
					}
				case '^':
					switch (reader.GetNext()) {
						case '=':
							++col;
							return new Token(TokenType.Operator, OperatorType.XorEqual, x, y);
						default:
							reader.UnGet();
							return new Token(TokenType.Operator, OperatorType.Xor, x, y);
					}
				case '!':
					switch (reader.GetNext()) {
						case '=':
							++col;
							return new Token(TokenType.Operator, OperatorType.NotEqual, x, y);
						default:
							reader.UnGet();
							return new Token(TokenType.Operator, OperatorType.Not, x, y);
					}
				case '~':
					return new Token(TokenType.Operator, OperatorType.BitwiseComplement, x, y);
				case '=':
					switch (reader.GetNext()) {
						case '=':
							++col;
							return new Token(TokenType.Operator, OperatorType.Equal, x, y);
						default:
							reader.UnGet();
							return new Token(TokenType.Operator, OperatorType.Assign, x, y);
					}
				case '<':
					switch (reader.GetNext()) {
						case '<':
							switch (reader.GetNext()) {
								case '=':
									col += 2;
									return new Token(TokenType.Operator, OperatorType.ShiftLeftEqual, x, y);
								default:
									++col;
									reader.UnGet();
									return new Token(TokenType.Operator, OperatorType.ShiftLeft, x, y);
							}
						case '=':
							++col;
							return new Token(TokenType.Operator, OperatorType.LessEqual, x, y);
						default:
							reader.UnGet();
							return new Token(TokenType.Operator, OperatorType.Less, x, y);
					}
				case '>':
					switch (reader.GetNext()) {
						case '>':
							switch (reader.GetNext()) {
								case '=':
									col += 2;
									return new Token(TokenType.Operator, OperatorType.ShiftRightEqual, x, y);
								default:
									++col;
									reader.UnGet();
									return new Token(TokenType.Operator, OperatorType.ShiftRight, x, y);
							}
						case '=':
							++col;
							return new Token(TokenType.Operator, OperatorType.MoreEqual, x, y);
						default:
							reader.UnGet();
							return new Token(TokenType.Operator, OperatorType.More, x, y);
					}
				case '?':
					return new Token(TokenType.Operator, OperatorType.Question, x, y);
				default:
					--col;
					return false;
			}
		}
		
		Object ReadPunctuation(char ch)
		{
			int x = col;
			int y = line;
			++col;
			switch (ch) {
				case ';':
					return new Token(TokenType.Punctuation, PunctuationType.Semicolon, x, y);
				case ':':
					return new Token(TokenType.Punctuation, PunctuationType.Colon, x, y);
				case ',':
					return new Token(TokenType.Punctuation, PunctuationType.Comma, x, y);
				case '.':
					if (Char.IsDigit(reader.Peek())) {
						 reader.UnGet();
						 col -= 2;
						 return ReadDigit('0', col + 1);
					}
					return new Token(TokenType.Punctuation, PunctuationType.Period, x, y);
				case ')':
					return new Token(TokenType.Punctuation, PunctuationType.CloseBrace, x, y);
				case '(':
					return new Token(TokenType.Punctuation, PunctuationType.OpenBrace, x, y);
				case ']':
					return new Token(TokenType.Punctuation, PunctuationType.CloseSquareBracket, x, y);
				case '[':
					return new Token(TokenType.Punctuation, PunctuationType.OpenSquareBracket, x, y);
				case '}':
					return new Token(TokenType.Punctuation, PunctuationType.CloseCurlyBrace, x, y);
				case '{':
					return new Token(TokenType.Punctuation, PunctuationType.OpenCurlyBrace, x, y);
				default:
					--col;
					return false;
			}
		}
	}
}
