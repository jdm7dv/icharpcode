// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
namespace SharpDevelop.Internal.Parser {
	
	public class Token
	{
		//constants for easier using:
		public static readonly Token PlusToken = new Token(TokenType.Operator, OperatorType.Plus);
		public static readonly Token MinusToken = new Token(TokenType.Operator, OperatorType.Minus);
		public static readonly Token MulToken = new Token(TokenType.Operator, OperatorType.Mul);
		public static readonly Token DivToken = new Token(TokenType.Operator, OperatorType.Div);
		public static readonly Token ModToken = new Token(TokenType.Operator, OperatorType.Mod);
		public static readonly Token BitwiseAndToken = new Token(TokenType.Operator, OperatorType.BitwiseAnd);
		public static readonly Token BitwiseOrToken = new Token(TokenType.Operator, OperatorType.BitwiseOr);
		public static readonly Token XorToken = new Token(TokenType.Operator, OperatorType.Xor);
		public static readonly Token NotToken = new Token(TokenType.Operator, OperatorType.Not);
		public static readonly Token BitwiseComplementToken = new Token(TokenType.Operator, OperatorType.BitwiseComplement);
		public static readonly Token AssignToken = new Token(TokenType.Operator, OperatorType.Assign);
		public static readonly Token LessToken = new Token(TokenType.Operator, OperatorType.Less);
		public static readonly Token MoreToken = new Token(TokenType.Operator, OperatorType.More);
		public static readonly Token QuestionToken = new Token(TokenType.Operator, OperatorType.Question);
		public static readonly Token IncrementToken = new Token(TokenType.Operator, OperatorType.Increment);
		public static readonly Token DecrementToken = new Token(TokenType.Operator, OperatorType.Decrement);
		public static readonly Token AndToken = new Token(TokenType.Operator, OperatorType.And);
		public static readonly Token OrToken = new Token(TokenType.Operator, OperatorType.Or);
		public static readonly Token ShiftLeftToken = new Token(TokenType.Operator, OperatorType.ShiftLeft);
		public static readonly Token ShiftRightToken = new Token(TokenType.Operator, OperatorType.ShiftRight);
		public static readonly Token EqualToken = new Token(TokenType.Operator, OperatorType.Equal);
		public static readonly Token NotEqualToken = new Token(TokenType.Operator, OperatorType.NotEqual);
		public static readonly Token LessEqualToken = new Token(TokenType.Operator, OperatorType.LessEqual);
		public static readonly Token MoreEqualToken = new Token(TokenType.Operator, OperatorType.MoreEqual);
		public static readonly Token PlusEqualToken = new Token(TokenType.Operator, OperatorType.PlusEqual);
		public static readonly Token MinusEqualToken = new Token(TokenType.Operator, OperatorType.MinusEqual);
		public static readonly Token MulEqualToken = new Token(TokenType.Operator, OperatorType.MulEqual);
		public static readonly Token DivEqualToken = new Token(TokenType.Operator, OperatorType.DivEqual);
		public static readonly Token ModEqualToken = new Token(TokenType.Operator, OperatorType.ModEqual);
		public static readonly Token AndEqualToken = new Token(TokenType.Operator, OperatorType.AndEqual);
		public static readonly Token OrEqualToken = new Token(TokenType.Operator, OperatorType.OrEqual);
		public static readonly Token XorEqualToken = new Token(TokenType.Operator, OperatorType.XorEqual);
		public static readonly Token ShiftLeftEqualToken = new Token(TokenType.Operator, OperatorType.ShiftLeftEqual);
		public static readonly Token ShiftRightEqualToken = new Token(TokenType.Operator, OperatorType.ShiftRightEqual);
		public static readonly Token DereferenceToken = new Token(TokenType.Operator, OperatorType.Dereference);
		
		public static readonly Token OpenCurlyBraceToken = new Token(TokenType.Punctuation, PunctuationType.OpenCurlyBrace);
		public static readonly Token CloseCurlyBraceToken = new Token(TokenType.Punctuation, PunctuationType.CloseCurlyBrace);
		public static readonly Token OpenBraceToken = new Token(TokenType.Punctuation, PunctuationType.OpenBrace);
		public static readonly Token CloseBraceToken = new Token(TokenType.Punctuation, PunctuationType.CloseBrace);
		public static readonly Token OpenSquareBraceToken = new Token(TokenType.Punctuation, PunctuationType.OpenSquareBracket);
		public static readonly Token CloseSquareBraceToken = new Token(TokenType.Punctuation, PunctuationType.CloseSquareBracket);
		public static readonly Token PeriodToken = new Token(TokenType.Punctuation, PunctuationType.Period);
		public static readonly Token CommaToken = new Token(TokenType.Punctuation, PunctuationType.Comma);
		public static readonly Token ColonToken = new Token(TokenType.Punctuation, PunctuationType.Colon);
		public static readonly Token SemicolonToken = new Token(TokenType.Punctuation, PunctuationType.Semicolon);
		
		public static readonly Token ThisToken       = new Token(TokenType.Keyword, KeyWordEnum.This);
		public static readonly Token BaseToken       = new Token(TokenType.Keyword, KeyWordEnum.Base);
		public static readonly Token AsToken         = new Token(TokenType.Keyword, KeyWordEnum.As);
		public static readonly Token IsToken         = new Token(TokenType.Keyword, KeyWordEnum.Is);
		public static readonly Token NewToken        = new Token(TokenType.Keyword, KeyWordEnum.New);
		public static readonly Token SizeofToken     = new Token(TokenType.Keyword, KeyWordEnum.Sizeof);
		public static readonly Token TypeofToken     = new Token(TokenType.Keyword, KeyWordEnum.Typeof);
		public static readonly Token StackallocToken = new Token(TokenType.Keyword, KeyWordEnum.Stackalloc);
		public static readonly Token ElseToken       = new Token(TokenType.Keyword, KeyWordEnum.Else);
		public static readonly Token IfToken         = new Token(TokenType.Keyword, KeyWordEnum.If);
		public static readonly Token SwitchToken     = new Token(TokenType.Keyword, KeyWordEnum.Switch);
		public static readonly Token CaseToken       = new Token(TokenType.Keyword, KeyWordEnum.Case);
		public static readonly Token DoToken         = new Token(TokenType.Keyword, KeyWordEnum.Do);
		public static readonly Token ForToken        = new Token(TokenType.Keyword, KeyWordEnum.For);
		public static readonly Token ForeachToken    = new Token(TokenType.Keyword, KeyWordEnum.Foreach);
		public static readonly Token InToken         = new Token(TokenType.Keyword, KeyWordEnum.In);
		public static readonly Token WhileToken      = new Token(TokenType.Keyword, KeyWordEnum.While);
		public static readonly Token DefaultToken    = new Token(TokenType.Keyword, KeyWordEnum.Default);
		public static readonly Token ReturnToken     = new Token(TokenType.Keyword, KeyWordEnum.Return);
		public static readonly Token TryToken        = new Token(TokenType.Keyword, KeyWordEnum.Try);
		public static readonly Token CatchToken      = new Token(TokenType.Keyword, KeyWordEnum.Catch);
		public static readonly Token FinallyToken    = new Token(TokenType.Keyword, KeyWordEnum.Finally);
		public static readonly Token CheckedToken    = new Token(TokenType.Keyword, KeyWordEnum.Checked);
		public static readonly Token UncheckedToken  = new Token(TokenType.Keyword, KeyWordEnum.Unchecked);
		public static readonly Token FixedToken      = new Token(TokenType.Keyword, KeyWordEnum.Fixed);
		public static readonly Token UnsafeToken     = new Token(TokenType.Keyword, KeyWordEnum.Unsafe);
		public static readonly Token EnumToken       = new Token(TokenType.Keyword, KeyWordEnum.Enum);
		public static readonly Token StructToken     = new Token(TokenType.Keyword, KeyWordEnum.Struct);
		public static readonly Token ClassToken      = new Token(TokenType.Keyword, KeyWordEnum.Class);
		public static readonly Token InterfaceToken  = new Token(TokenType.Keyword, KeyWordEnum.Interface);
		public static readonly Token DelegateToken   = new Token(TokenType.Keyword, KeyWordEnum.Delegate);
		public static readonly Token VoidToken       = new Token(TokenType.Keyword, KeyWordEnum.Void);
		public static readonly Token ExplicitToken   = new Token(TokenType.Keyword, KeyWordEnum.Explicit);
		public static readonly Token ImplicitToken   = new Token(TokenType.Keyword, KeyWordEnum.Implicit);
		public static readonly Token OperatorToken   = new Token(TokenType.Keyword, KeyWordEnum.Operator);
		public static readonly Token ParamsToken     = new Token(TokenType.Keyword, KeyWordEnum.Params);
		public static readonly Token RefToken        = new Token(TokenType.Keyword, KeyWordEnum.Ref);
		public static readonly Token OutToken        = new Token(TokenType.Keyword, KeyWordEnum.Out);
		public static readonly Token ConstToken      = new Token(TokenType.Keyword, KeyWordEnum.Const);
		public static readonly Token EventToken      = new Token(TokenType.Keyword, KeyWordEnum.Event);
		public static readonly Token NamespaceToken  = new Token(TokenType.Keyword, KeyWordEnum.Namespace);
		public static readonly Token UsingToken      = new Token(TokenType.Keyword, KeyWordEnum.Using);
		public static readonly Token LockToken       = new Token(TokenType.Keyword, KeyWordEnum.Lock);
		public static readonly Token TrueToken       = new Token(TokenType.Keyword, KeyWordEnum.True);
		public static readonly Token FalseToken      = new Token(TokenType.Keyword, KeyWordEnum.False);
		
		public static readonly Token AddToken = new Token(TokenType.Identifier, "add");
		public static readonly Token RemoveToken = new Token(TokenType.Identifier, "remove");
		public static readonly Token GetToken = new Token(TokenType.Identifier, "get");
		public static readonly Token SetToken = new Token(TokenType.Identifier, "set");
		
		TokenType type;
		object val;
		int col;
		int line;
		
		public Token(TokenType type, object val)
		{
			this.type = type;
			this.val = val;
		}
		
		public Token(TokenType type, object val, int col, int line)
		{
			this.type = type;
			this.val = val;
			this.col = col;
			this.line = line;
		}
		
		public bool Equals(Token t)
		{
			if (t == null)
				return false;
			if (t.Type != this.Type)
				return false;
			switch (t.Type) {
				case TokenType.DocumentationComment:
				case TokenType.Identifier:
				case TokenType.String:
					return ((string)t.Value).Equals((string)this.Value);
				case TokenType.Keyword:
					return (KeyWordEnum)t.Value == (KeyWordEnum)this.Value;
				case TokenType.Operator:
					return (OperatorType)t.Value == (OperatorType)this.Value;
				case TokenType.Punctuation:
					return (PunctuationType)t.Value == (PunctuationType)this.Value;
				case TokenType.Char:
					return (char)t.Value == (char)this.Value;
				case TokenType.Int:
					return (int)t.Value == (int)this.Value;
				case TokenType.UInt:
					return (uint)t.Value == (uint)this.Value;
				case TokenType.Long:
					return (long)t.Value == (long)this.Value;
				case TokenType.ULong:
					return (ulong)t.Value == (ulong)this.Value;
				case TokenType.Float:
					return (float)t.Value == (float)this.Value;
				case TokenType.Double:
					return (double)t.Value == (double)this.Value;
				case TokenType.Decimal:
					return (decimal)t.Value == (decimal)this.Value;
				case TokenType.EOF:
					return true;
				case TokenType.Unknown:
					return false;
				default:
					throw new ParserException("Error, Unknowen Type in Token.Equal: " + t.Type, 0, 0);
			}
		}
		
		public bool IsLiteral()
		{
			switch (Type) {
				case TokenType.String:
				case TokenType.Char:
				case TokenType.Int:
				case TokenType.UInt:
				case TokenType.Long:
				case TokenType.ULong:
				case TokenType.Float:
				case TokenType.Double:
				case TokenType.Decimal:
					return true;
				case TokenType.Keyword:
					KeyWordEnum w = (KeyWordEnum)Value;
					if (w == KeyWordEnum.True || w == KeyWordEnum.False || w == KeyWordEnum.Null) {
						return true;
					}
					return false;
				default:
					return false;
			}
		}
		
		public TokenType Type {
			get {
				return type;
			}
		}
		
		public Object Value {
			get {
				return val;
			}
		}
		
		public int Col {
			get {
				return col;
			}
		}
		
		public int Line {
			get {
				return line;
			}
		}
	}
	
	public enum TokenType {
		DocumentationComment,
		Identifier,
		Keyword,
		Operator,
		Punctuation,
		String,
		Char,
		Int,
		UInt,
		Long,
		ULong,
		Float,
		Double,
		Decimal,
		EOF,
		Unknown
	}
	
	public enum OperatorType {
		Plus,
		Minus,
		Mul,
		Div,
		Mod,
		BitwiseAnd,
		BitwiseOr,
		Xor,
		Not,
		BitwiseComplement,
		Assign,
		Less,
		More,
		Question,
		Increment,
		Decrement,
		And,
		Or,
		ShiftLeft,
		ShiftRight,
		Equal,
		NotEqual,
		LessEqual,
		MoreEqual,
		PlusEqual,
		MinusEqual,
		MulEqual,
		DivEqual,
		ModEqual,
		AndEqual,
		OrEqual,
		XorEqual,
		ShiftLeftEqual,
		ShiftRightEqual,
		Dereference,
		Unknown
	}
	
	public enum PunctuationType {
		OpenCurlyBrace,
		CloseCurlyBrace,
		OpenBrace,
		CloseBrace,
		OpenSquareBracket,
		CloseSquareBracket,
		Period,
		Comma,
		Colon,
		Semicolon,
		Unknown
	}
}
