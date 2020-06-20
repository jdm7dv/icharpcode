// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Text;
namespace SharpDevelop.Internal.Parser {
	
	public enum KeyWordEnum {
		 This,
		 Base,
		 As,
		 Is,
		 New,
		 Sizeof,
		 Typeof,
		 True,
		 False,
		 Stackalloc,
		 Else,
		 If,
		 Switch,
		 Case,
		 Do,
		 For,
		 Foreach,
		 In,
		 While,
		 Break,
		 Continue,
		 Default,
		 Goto,
		 Return,
		 Try,
		 Throw,
		 Catch,
		 Finally,
		 Checked,
		 Unchecked,
		 Fixed,
		 Unsafe,
		 Bool,
		 Byte,
		 Char,
		 Decimal,
		 Double,
		 Enum,
		 Float,
		 Int,
		 Long,
		 Sbyte,
		 Short,
		 Struct,
		 Uint,
		 Ushort,
		 Ulong,
		 Class,
		 Interface,
		 Delegate,
		 Object,
		 String,
		 Void,
		 Explicit,
		 Implicit,
		 Operator,
		 Params,
		 Ref,
		 Out,
		 Abstract,
		 Const,
		 Event,
		 Extern,
		 Override,
		 Readonly,
		 Sealed,
		 Static,
		 Virtual,
		 Public,
		 Protected,
		 Private,
		 Internal,
		 Namespace,
		 Using,
		 Lock,
		 Null,
	}
	
	public class KeyWord
	{
		string[]  keywords = {
			"this",
			"base",
			"as",
			"is",
			"new",
			"sizeof",
			"typeof",
			"true",
			"false",
			"stackalloc",
			"else",
			"if",
			"switch",
			"case",
			"do",
			"for",
			"foreach",
			"in",
			"while",
			"break",
			"continue",
			"default",
			"goto",
			"return",
			"try",
			"throw",
			"catch",
			"finally",
			"checked",
			"unchecked",
			"fixed",
			"unsafe",
			"bool",
			"byte",
			"char",
			"decimal",
			"double",
			"enum",
			"float",
			"int",
			"long",
			"sbyte",
			"short",
			"struct",
			"uint",
			"ushort",
			"ulong",
			"class",
			"interface",
			"delegate",
			"object",
			"string",
			"void",
			"explicit",
			"implicit",
			"operator",
			"params",
			"ref",
			"out",
			"abstract",
			"const",
			"event",
			"extern",
			"override",
			"readonly",
			"sealed",
			"static",
			"virtual",
			"public",
			"protected",
			"private",
			"internal",
			"namespace",
			"using",
			"lock",
			"null",
		};
		
		Hashtable keyhash = new Hashtable();
		
		public KeyWord()
		{
			foreach (string keyword in keywords) {
				string newword = Char.ToUpper(keyword[0]) + keyword.Substring(1);
				keyhash[keyword] = (KeyWordEnum)Enum.Parse(typeof(KeyWordEnum), newword);
			}
		}
		
		public bool IsKeyWord(string word)
		{
			return keyhash[word] != null;
		}
		
		public KeyWordEnum GetKeyWord(string word)
		{
			return (KeyWordEnum)keyhash[word];
		}
		
		public bool IsType(KeyWordEnum word)
		{
			return word == KeyWordEnum.Bool || word == KeyWordEnum.Byte || word == KeyWordEnum.Char || 
			       word == KeyWordEnum.Decimal || word == KeyWordEnum.Double || word == KeyWordEnum.Float || 
			       word == KeyWordEnum.Int || word == KeyWordEnum.Long || word == KeyWordEnum.Object ||
			       word == KeyWordEnum.Sbyte || word == KeyWordEnum.Short || word == KeyWordEnum.String ||
			       word == KeyWordEnum.Uint || word == KeyWordEnum.Ulong || word == KeyWordEnum.Ushort;
			               
		}
	}
}
