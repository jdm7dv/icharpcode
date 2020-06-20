// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;

namespace SharpDevelop.Internal.Parser {
	
	public class CompilationUnit : AbstractCompilationUnit, Scope
	{
		IRegion region = new DefaultRegion(0, 0);
		ArrayList namespaces;
		Lexer     lexer;
		TokenList tokens;
		
		public Token NextToken {
			get{
				return tokens.Next(0);
			}
		}
		
		public Lexer Lexer{
			get{
				return lexer;
			}
		}
		
		public override CommentCollection MiscComments {
			get {
				return lexer.MiscComments;
			}
		}
		
		public override CommentCollection DokuComments {
			get {
				return lexer.DokuComments;
			}
		}
		
		public override TagCollection TagComments {
			get {
				return lexer.TagComments;
			}
		}
		
		public Token LookUpToken(int i)
		{
			return tokens.Next(i);
		}
		
		public void SetErrorFlag()
		{
			errorsDuringCompile = true;
		}
		
		public bool Match(Token t)
		{
			bool isOk = t.Equals(NextToken);
			if(!isOk){
				throw new ParserException("\n Error: " + t.Type + " >" + t.Value + "< Expected at (" + NextToken.Line + "/" + NextToken.Col + ")", NextToken.Line, NextToken.Col);
			}
	//		else
	//			Console.Write(t.Type + " >" + t.Value + "< / ");
			if (tokens.Next() == null) {
				throw new ParserException("\n CodingError in Parser.Match: next Token == null", 0, 0);
			}
			return isOk;
		}
		
		public bool Match(TokenType t)
		{
			bool isOk = (NextToken.Type == t);
			if(!isOk){
				throw new ParserException("\n Error: " + t + " Expected at (" + NextToken.Line + "/" + NextToken.Col + ")", NextToken.Line, NextToken.Col);
			}
	//		else
	//			Console.Write(NextToken.Type + " >" + NextToken.Value + "< / ");
			if(tokens.Next() == null) {
				throw new ParserException("\n CodingError in Parser.Match: next Token == null", 0, 0);
			}
			return isOk;
		}
		
		public void Match()
		{
	//		Console.Write(NextToken.Type + " >" + NextToken.Value + "< / ");
			if (tokens.Next() == null) {
				throw new ParserException("\n CodingError in Parser.Match: next Token == null", 0, 0);
			}
		}
		
		public string NamespaceOrTypeName()
		{
	//		Console.Write(" \"NamespaceOrTypeName\" ");
			try {
				StringBuilder name = new StringBuilder((string)NextToken.Value);
				Match(TokenType.Identifier);
				while(NextToken.Equals(Token.PeriodToken)) {
					Match(Token.PeriodToken);
					name.Append('.');
					name.Append((string)NextToken.Value);
					Match(TokenType.Identifier);
				}
				return name.ToString();
			} catch (Exception) {
				throw new ParserException("\n Invalid token " + NextToken.Type + " expected Identifier", NextToken.Line, NextToken.Col);
			}
	//		Console.Write(" \"\\NamespaceOrTypeName\" ");
		}
		static readonly string[,] types = new string[,] {
				{"System.Void",    "void"},
				{"System.Object",  "object"},
				{"System.Boolean", "bool"},
				{"System.Byte",    "byte"},
				{"System.SByte",   "sbyte"},
				{"System.Char",    "char"},
				{"System.Enum",    "enum"},
				{"System.Int16",   "short"},
				{"System.Int32",   "int"},
				{"System.Int64",   "long"},
				{"System.UInt16",  "ushort"},
				{"System.UInt32",  "uint"},
				{"System.UInt64",  "ulong"},
				{"System.Single",  "float"},
				{"System.Double",  "double"},
				{"System.Decimal", "decimal"},
				{"System.String",  "string"}
			};
		
		public ReturnType Type(bool parseArray)
		{
	//		Console.Write(" \"Type\" ");
			string fullName = "";
			int[] arrayLength;
			int pointer = 0;
	
			if (NextToken.Type == TokenType.Keyword) {
				if (Lexer.keywords.IsType((KeyWordEnum)NextToken.Value)) {
					string newtype = Char.ToLower(NextToken.Value.ToString()[0]) + NextToken.Value.ToString().Substring(1);
					for (int i = 0; i < types.GetLength(0); ++i) {
						if (newtype == types[i, 1]) {
							fullName = types[i, 0];
							break;
						}
					}
					Match();
				} else if ((KeyWordEnum)NextToken.Value == KeyWordEnum.Void) {
					string newtype = Char.ToLower(NextToken.Value.ToString()[0]) + NextToken.Value.ToString().Substring(1);
					for (int i = 0; i < types.GetLength(0); ++i) {
						if (newtype == types[i, 1]) {
							fullName = types[i, 0];
							break;
						}
					}
					Match();
				}
			} else if (NextToken.Type == TokenType.Identifier){
				fullName = NamespaceOrTypeName();
			} else {
				throw new ParserException("\nError: Type Expected at (" + NextToken.Line + "/" + NextToken.Col + ")", NextToken.Line, NextToken.Col);
			}
			ArrayList l = new ArrayList();
			if (!parseArray) {
				return new ReturnType(fullName, null, 0);
			}
			while (NextToken.Equals(Token.MulToken)) {
				++pointer;
				Match();
			}
			while (NextToken.Equals(Token.OpenSquareBraceToken)) {
				Match(Token.OpenSquareBraceToken);
				int i = 1;
				while (NextToken.Equals(Token.CommaToken)) {
					++i;
					Match(Token.CommaToken);
				}
				l.Add(i);
				Match(Token.CloseSquareBraceToken);
			}
			arrayLength = new int[l.Count];
			for (int i = 0; i < l.Count; ++i) {
				arrayLength[i] = (int)l[i];
			}
			return new ReturnType(fullName, arrayLength, pointer);
	//		Console.Write(" \"\\Type\" ");
		}
		
		public bool IsPredefinedType(Token t)
		{
			return t.Type == TokenType.Keyword && Lexer.keywords.IsType((KeyWordEnum)t.Value);
		}
		
		public IRegion Region {
			get {
				return region;
			}
		}
		
		public ArrayList Namespaces{
			get{
				return namespaces;
			}
		}
		
		public CompilationUnit(Lexer l)
		{
			lexer = l;
			tokens = new TokenList(lexer);
		}
		
		public void AddToLookUpTable(Variable v)
		{
			lookUpTable.Add(v);
		}
		
		public IUsing Using(Namespace currentNamespace)
		{
			IUsing us = new Using(currentNamespace.Region);
			us.Aliases[""] = currentNamespace.FullyQualifiedName;
			string name;
			while(NextToken.Equals(Token.UsingToken)){
				Match();
				name = NamespaceOrTypeName();
				if(NextToken.Equals(Token.AssignToken)){
					Match();
					us.Aliases[name] = NamespaceOrTypeName();
				} else {
					us.Usings.Add(name);
				}
				Match(Token.SemicolonToken);
			}
			return us;
		}
		
		public void VariableDeclarators(bool matchIdent)
		{
	//		Console.Write("\"VariableDeclarators\"");
			if(matchIdent)
				Match(TokenType.Identifier);
			if(NextToken.Equals(Token.AssignToken)){
				Match();
				if(NextToken.Equals(Token.OpenCurlyBraceToken)){//array-initializer
					ArrayInitializer();
				} else {
					(new Expr(this)).Expression();
				}
			}
			while(NextToken.Equals(Token.CommaToken)){
				Match(Token.CommaToken);
				Match(TokenType.Identifier);
				if(NextToken.Equals(Token.AssignToken)){
					Match();
					if(NextToken.Equals(Token.OpenCurlyBraceToken)){//array-initializer
						ArrayInitializer();
					}else
						(new Expr(this)).Expression();
				}
			}
	//		Console.Write("\"\\VariableDeclarators\"");
		}
		
		public void ArrayInitializer()//Also in Statement
		{
	//		Console.Write("\"ArrayInitializer\"");
			Match(Token.OpenCurlyBraceToken);
			if(!NextToken.Equals(Token.CloseCurlyBraceToken)){
				if(NextToken.Equals(Token.OpenCurlyBraceToken))
					ArrayInitializer();
				else
					(new Expr(this)).Expression();
				while(NextToken.Equals(Token.CommaToken)){
					Match(Token.CommaToken);
					if(NextToken.Equals(Token.OpenCurlyBraceToken))
						ArrayInitializer();
					else if(NextToken.Equals(Token.CloseCurlyBraceToken))
						break;
					else
						(new Expr(this)).Expression();
				}
			}
			Match(Token.CloseCurlyBraceToken);
	//		Console.Write("\"\\ArrayInitializer\"");
		}
		
		public ParameterCollection ParameterList()
		{
			ParameterCollection parameters = new ParameterCollection();
			Parameter parameter = new Parameter();
			AttributeSectionCollection attributes;
			if(NextToken.Equals(Token.OpenSquareBraceToken))
				attributes = ParseAttributes();
			if(NextToken.Equals(Token.RefToken)){
				parameter.Modifier |= ParameterModifier.Ref;
				Match();
			}
			else if(NextToken.Equals(Token.OutToken)){
				parameter.Modifier |= ParameterModifier.Out;
				Match();
			}
			else if(NextToken.Equals(Token.ParamsToken)){
				parameter.Modifier |= ParameterModifier.Out;
				Match();
			}
			parameter.ReturnType = Type(true);
			parameter.Name = (string)NextToken.Value;
			Match(TokenType.Identifier);
			parameters.Add(parameter);
			while(NextToken.Equals(Token.CommaToken)){
				Match(Token.CommaToken);
				parameter = new Parameter();
				if(NextToken.Equals(Token.OpenSquareBraceToken))
					attributes = ParseAttributes();
				if(NextToken.Equals(Token.RefToken)){
					parameter.Modifier |= ParameterModifier.Ref;
					Match();
				}
				else if(NextToken.Equals(Token.OutToken)){
					parameter.Modifier |= ParameterModifier.Out;
					Match();
				}
				else if(NextToken.Equals(Token.ParamsToken)){
					parameter.Modifier |= ParameterModifier.Out;
					Match();
				}
				parameter.ReturnType = Type(true);
				if(NextToken.Type != TokenType.Identifier) {
					throw new ParserException("Identifier Expected at(" + NextToken.Line + "/" + NextToken.Col + ") was " + NextToken.Value, NextToken.Line, NextToken.Col);
				}
				parameter.Name = (string)NextToken.Value;
				Match(TokenType.Identifier);
				parameters.Add(parameter);
			}
			return parameters;
		}
		
		public StringCollection Inheritance()
		{
			StringCollection baseTypes = new StringCollection();
			baseTypes.Add(NamespaceOrTypeName());
			while(NextToken.Equals(Token.CommaToken)){
				Match(Token.CommaToken);
				baseTypes.Add(NamespaceOrTypeName());
			}
			return baseTypes;
		}
		
		public ModifierEnum ParseModifier() 
		{
			ModifierEnum modifiers = ModifierEnum.None;
			ModifierEnum curModifier = IsModifier(NextToken);
			while ( curModifier!= ModifierEnum.None) {
				if ((curModifier == ModifierEnum.Internal) && ((modifiers & ModifierEnum.Protected) == ModifierEnum.Protected)) {
					modifiers = modifiers & ~ModifierEnum.Protected;
					modifiers |= ModifierEnum.ProtectedOrInternal;
				}
				modifiers |= curModifier;
				Match();
				curModifier = IsModifier(NextToken);
			}
			return modifiers;
		}
		
		public ModifierEnum IsModifier(Token t)
		{
			if(t.Type != TokenType.Keyword)
				return ModifierEnum.None;
			KeyWordEnum w = (KeyWordEnum)t.Value;
			switch(w){
				case KeyWordEnum.New:
					return ModifierEnum.New;
				case KeyWordEnum.Public:
					return ModifierEnum.Public;
				case KeyWordEnum.Protected:
					return ModifierEnum.Protected;
				case KeyWordEnum.Internal:
					return ModifierEnum.Internal;
				case KeyWordEnum.Private:
					return ModifierEnum.Private;
				case KeyWordEnum.Abstract:
					return ModifierEnum.Abstract;
				case KeyWordEnum.Sealed:
					return ModifierEnum.Sealed;
				case KeyWordEnum.Static:
					return ModifierEnum.Static;
				case KeyWordEnum.Readonly:
					return ModifierEnum.Readonly;
				case KeyWordEnum.Virtual:
					return ModifierEnum.Virtual;
				case KeyWordEnum.Override:
					return ModifierEnum.Override;
				case KeyWordEnum.Extern:
					return ModifierEnum.Extern;
				case KeyWordEnum.Unsafe:
					return ModifierEnum.Unsafe;
				default:
					return ModifierEnum.None;
			}
		}
		
		AttributeTarget AttributeTarget()
		{
			if(NextToken.Equals(Token.ReturnToken))
				return SharpDevelop.Internal.Parser.AttributeTarget.Return;
			if(NextToken.Type == TokenType.Identifier){
				string s = (string)NextToken.Value;
				switch(s){
					case "assembly":
						return SharpDevelop.Internal.Parser.AttributeTarget.Assembly;
					case "field":
						return SharpDevelop.Internal.Parser.AttributeTarget.Field;
					case "event":
						return SharpDevelop.Internal.Parser.AttributeTarget.Event;
					case "method":
						return SharpDevelop.Internal.Parser.AttributeTarget.Method;
					case "module":
						return SharpDevelop.Internal.Parser.AttributeTarget.Module;
					case "param":
						return SharpDevelop.Internal.Parser.AttributeTarget.Param;
					case "property":
						return SharpDevelop.Internal.Parser.AttributeTarget.Property;
					case "@return":
						return SharpDevelop.Internal.Parser.AttributeTarget.Return;
					case "type":
						return SharpDevelop.Internal.Parser.AttributeTarget.Type;
					default:
						return SharpDevelop.Internal.Parser.AttributeTarget.None;
				}
			}
			return SharpDevelop.Internal.Parser.AttributeTarget.None;
		}
		
		Attributes.Attribute Attribute()
		{
			Attributes.Attribute a = new Attributes.Attribute();
			a.Name = NamespaceOrTypeName();
			if(NextToken.Equals(Token.OpenBraceToken))
			{//Todo:attribute-argumentsopt: positional-argument, goes right
				Match(Token.OpenBraceToken);
				if(!NextToken.Equals(Token.CloseBraceToken)){
					Expr e = (new Expr(this));
					(new Expr(this)).Expression();
					a.PositionalArguments.Add(e);
					while(!NextToken.Equals(Token.CloseBraceToken)){
						Match(Token.CommaToken);
						e = (new Expr(this));
						(new Expr(this)).Expression();
						a.PositionalArguments.Add(e);
					}
				}
				Match(Token.CloseBraceToken);
			}
			return a;
		}
		
		void NamedArguments()
		{
			while(!NextToken.Equals(Token.CloseBraceToken)){
				Match(Token.CommaToken);
				Match(TokenType.Identifier);
				Match(Token.AssignToken);
				(new Expr(this)).Expression();
			}
		}
		
		public AttributeSectionCollection ParseAttributes()
		{
			AttributeSectionCollection list = new AttributeSectionCollection();
			while(NextToken.Equals(Token.OpenSquareBraceToken)){
				Attributes a = new Attributes();
				Match(Token.OpenSquareBraceToken);
				if(AttributeTarget() != SharpDevelop.Internal.Parser.AttributeTarget.None){
					a.AttributeTarget = AttributeTarget();
					Match();
					Match(Token.ColonToken);
				}
				a.Attributes.Add(Attribute());
				while(NextToken.Equals(Token.CommaToken)){
					Match();
					if(!NextToken.Equals(Token.CloseSquareBraceToken))
						a.Attributes.Add(Attribute());
				}
				Match(Token.CloseSquareBraceToken);
				list.Add(a);
			}
			return list;
		}
		
		public bool NameMember(Namespace current)
		{
	//		Console.Write(" \"NameMember\" ");
			AttributeSectionCollection att = ParseAttributes();
			ModifierEnum modifiers = ModifierEnum.None;
			modifiers = ParseModifier();
			if (modifiers == ModifierEnum.None) {
				modifiers = ModifierEnum.Internal;
			}
			
			if (NextToken.Equals(Token.NamespaceToken)){
				IRegion region = new DefaultRegion(NextToken.Line, NextToken.Col);
				Match();
				string name = current.FullyQualifiedName;
				name += '.';
				if (name.StartsWith("Global")) {
					if (name.Length <= 7) {
						name = "";
					} else {
						name.Remove(0, 7);
					}
				}
				name += NamespaceOrTypeName();
				Namespace a = new Namespace(name, current, region, this);
				namespaces.Add(a);
				Match(Token.OpenCurlyBraceToken);
				Usings.Add(Using(a));
				while(!NextToken.Equals(Token.CloseCurlyBraceToken)) {
					if(!NameMember(a)){
						a.Region.EndLine = NextToken.Line;
						a.Region.EndColumn = NextToken.Col;
						break;
					}
				}
				Match(Token.CloseCurlyBraceToken);
				if(NextToken.Equals(Token.SemicolonToken))
					Match();
				return true;
			}else if(NextToken.Equals(Token.StructToken)){
				Class c = new Class(current, this, current.Usings, modifiers, true);
				classes.Add(c);
				c.StructDeclaration();
				return true;
			}else if(NextToken.Equals(Token.InterfaceToken)){
				Interface i = new Interface(current, this, current.Usings, modifiers);
				classes.Add(i);
				i.InterfaceDeclaration();
				return true;
			}else if(NextToken.Equals(Token.EnumToken)){
				Enumeration e = new Enumeration(current, this, current.Usings, modifiers);
				classes.Add(e);
				e.EnumDeclaration();
				return true;
			}else if(NextToken.Equals(Token.DelegateToken)){
				Delegate d = new Delegate(current, this, current.Usings, modifiers);
				classes.Add(d);
				d.DelegateDeclaration();
				return true;
			}else if(NextToken.Equals(Token.ClassToken)){
				Class c = new Class(current, this, current.Usings, modifiers);
				classes.Add(c);
				c.ClassDeclaration();
				return true;
			}
			return false;
	//		Console.Write(" \"\\NameMember\" ");
		}
		
		public void Parse()
		{
			namespaces = new ArrayList();
			string name = "Global";
			Namespace current = new Namespace(name, null, new DefaultRegion(0, 0), this);
			namespaces.Add(current);
			usings.Add(Using(current));
			if(NextToken.Equals(Token.OpenSquareBraceToken)){
				attributes = ParseAttributes();
			}
			while(NextToken.Type != TokenType.EOF){
				if (!NameMember(current)){
					current.Region.EndLine = NextToken.Line;
					current.Region.EndColumn = NextToken.Col;
					break;
				}
			}
			region.EndLine = NextToken.Line;
			region.EndColumn = NextToken.Col;
		}
		
		public class Namespace : Type
		{
			string fullyQualifiedName;
			CompilationUnit unit;
			Type parent;
			public ArrayList namespaces = new ArrayList();
			public ArrayList structs = new ArrayList();
			public ArrayList interfaces = new ArrayList();
			public ArrayList enums = new ArrayList();
			public ArrayList delegates = new ArrayList();
			public ArrayList classes = new ArrayList();
			IRegion region;
			public Namespace(string name, Type parent, IRegion region, CompilationUnit unit)
			{
				fullyQualifiedName = name; 
				this.parent = parent;
				this.region = region;
			}
			public IUsing Usings { //TODO: rausschmeissen
				get {
					return null;
				}
			}
			public IRegion Region {
				get {
					return region;
				}
			}
			public Type Parent {
				get {
					return parent;
				}
			}
			public CompilationUnit Unit {
				get {
					return unit;
				}
				set {
					unit = value;
				}
			}
			public string FullyQualifiedName {
				get {
					return fullyQualifiedName;
				}
			}
			public string Name {
				get {
					string[] name = fullyQualifiedName.Split(new char[] {'.'});
					return name[name.Length - 1];
				}
			}
			public ModifierEnum Modifiers {
				get {
					return ModifierEnum.None;
				}
				set {
				}
			}
		}
	}
}
