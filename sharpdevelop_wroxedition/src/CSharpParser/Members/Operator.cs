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
	
	public class Operator : AbstractMethod, Member
	{
		
		static SortedList operatorNames = null;
		
		// todo: implicit, explicit, true, false
		static void InitializeNames() {
			operatorNames = new SortedList();
//			operatorNames[OperatorType.Plus] = "op_Addition";
			operatorNames[OperatorType.BitwiseAnd] = "op_BitwiseAnd";
			operatorNames[OperatorType.BitwiseOr] = "op_BitwiseOr";
			operatorNames[OperatorType.Decrement] = "op_Decrement";
			operatorNames[OperatorType.Div] = "op_Division";
			operatorNames[OperatorType.Equal] = "op_Equality";
			operatorNames[OperatorType.Xor] = "op_ExclusiveOr";
//			operatorNames[OperatorType.Explicit] = "op_Explicit";
//			operatorNames[OperatorType.False] = "op_False";
			operatorNames[OperatorType.More] = "op_GreaterThan";
			operatorNames[OperatorType.MoreEqual] = "op_GreaterThanOrEqual";
//			operatorNames[OperatorType.Implicit] = "op_Implicit";
			operatorNames[OperatorType.Increment] = "op_Increment";
			operatorNames[OperatorType.NotEqual] = "op_Inequality";
			operatorNames[OperatorType.ShiftLeft] = "op_LeftShift";
			operatorNames[OperatorType.Less] = "op_LessThan";
			operatorNames[OperatorType.LessEqual] = "op_LessThanOrEqual";
			operatorNames[OperatorType.Mod] = "op_Modulus";
			operatorNames[OperatorType.Mul] = "op_Multiply";
			operatorNames[OperatorType.Not] = "op_Negation";
			operatorNames[OperatorType.BitwiseComplement] = "op_OnesComplement";
			operatorNames[OperatorType.ShiftRight] = "op_RightShift";
//			operatorNames[OperatorType.Minus] = "op_Subtraction";
//			operatorNames[OperatorType.True] = "op_True";
			operatorNames[OperatorType.Minus] = "op_UnaryNegation";
			operatorNames[OperatorType.Plus] = "op_UnaryPlus";
		}
		
		MemberParent parent;
		Cast cast = Cast.None;
		bool interfaceContent = false;
		
		public bool InterfaceContent {
			get {
				return interfaceContent;
			}
		}
		
		public Type Parent {
			get {
				return parent;
			}
		}
		
		public CompilationUnit Unit {
			get {
				return parent.Unit;
			}
		}
		
		public Cast IsCast {
			get {
				return cast;
			}
		}
		
		public Operator(MemberParent parent)
		{
			this.parent = parent;
			if (parent != null && parent is Interface) {
				interfaceContent = true;
			}
			if (operatorNames == null) {
				InitializeNames();
			}
			modifiers |= ModifierEnum.SpecialName;
		}
		
		public Operator(MemberParent parent, AttributeSectionCollection attributes, ModifierEnum modifier, Cast cast)
		{
			this.parent = parent;
			if (parent != null && parent is Interface) {
				interfaceContent = true;
			}
			if (operatorNames == null) {
				InitializeNames();
			}
			this.attributes = attributes;
			this.modifiers = modifier;
			this.cast = cast;
			modifiers |= ModifierEnum.SpecialName;
		}
		
		public Operator(MemberParent parent, AttributeSectionCollection attributes, ModifierEnum modifier, IRegion region, ReturnType type)
		{
			this.parent = parent;
			if (parent != null && parent is Interface) {
				interfaceContent = true;
			}
			if (operatorNames == null) {
				InitializeNames();
			}
			this.attributes = attributes;
			this.modifiers = modifier;
			this.region = region;
			this.returnType = type;
			modifiers |= ModifierEnum.SpecialName;
		}
		
		public void ConversionOperator()
		{
	//		Console.Write("\"ConversionOperator\"");
			if (parent.Unit.NextToken.Equals(Token.OpenSquareBraceToken)) {
				parent.Unit.ParseAttributes();
			}
			modifiers |= parent.Unit.ParseModifier();
			region = new DefaultRegion(parent.Unit.NextToken.Line, parent.Unit.NextToken.Col);
			if (parent.Unit.NextToken.Equals(Token.ImplicitToken)) {
				cast = Cast.Implicit;
				fullyQualifiedName = "op_Implicit";
			} else if (parent.Unit.NextToken.Equals(Token.ExplicitToken)) {
				cast = Cast.Explicit;
				fullyQualifiedName = "op_Explicit";
			} else {
				throw new ParserException("\n Error: implicit or explicit expected at (" + parent.Unit.NextToken.Line + "/" + parent.Unit.NextToken.Col + ")", parent.Unit.NextToken.Line, parent.Unit.NextToken.Col);
			}
			fullyQualifiedName = parent.Unit.NextToken.Value.ToString() + fullyQualifiedName;
			parent.Unit.Match();
			parent.Unit.Match(Token.OperatorToken);
			returnType = parent.Unit.Type(true);
			parent.Unit.Match(Token.OpenBraceToken);
			Parameter p = new Parameter();
			p.ReturnType = parent.Unit.Type(true);
			p.Name = parent.Unit.NextToken.Value.ToString();
			parameters.Add(p);
			parent.Unit.Match(TokenType.Identifier);
			parent.Unit.Match(Token.CloseBraceToken);
			new Block(parent.Unit).Parse();
	//		Console.Write("\"\\ConversionOperator\"");
		}
		
		public void Parse()
		{
	//		Console.Write("\"Operator\"");
			if (parent.Unit.NextToken.Equals(Token.OpenSquareBraceToken)) {
				parent.Unit.ParseAttributes();
			}
			modifiers |= parent.Unit.ParseModifier();
			region = new DefaultRegion(parent.Unit.NextToken.Line, parent.Unit.NextToken.Col);
			if (parent.Unit.NextToken.Equals(Token.ImplicitToken) || parent.Unit.NextToken.Equals(Token.ExplicitToken)) {
				ConversionOperator();
			}
			returnType = parent.Unit.Type(true);
			ParseRest();
	//		Console.Write("\"\\Operator\"");
		}
		
		public void ParseRest()
		{
	//		Console.Write("\"OperatorRest\"");
			parent.Unit.Match(Token.OperatorToken);
			fullyQualifiedName = String.Concat(parent.FullyQualifiedName, ".", operatorNames[parent.Unit.NextToken.Value]);
			parent.Unit.Match(TokenType.Operator);
			parent.Unit.Match(Token.OpenBraceToken);
			Parameter p = new Parameter();
			p.ReturnType = parent.Unit.Type(true);
			p.Name = parent.Unit.NextToken.Value.ToString();
			parameters.Add(p);
			parent.Unit.Match(TokenType.Identifier);
			if (!parent.Unit.NextToken.Equals(Token.CloseBraceToken)) {
				// Binary Plus or Minus
				if (Name == "op_UnaryNegation") {
					fullyQualifiedName = parent.FullyQualifiedName + ".op_Subtraction";
				} else if (Name == "op_UnaryPlus") {
					fullyQualifiedName = parent.FullyQualifiedName + ".op_Addition";
				}
				parent.Unit.Match(Token.CommaToken);
				p = new Parameter();
				p.ReturnType = parent.Unit.Type(true);
				p.Name = parent.Unit.NextToken.Value.ToString();
				parameters.Add(p);
				parent.Unit.Match(TokenType.Identifier);
			}
			parent.Unit.Match(Token.CloseBraceToken);
			new Block(parent.Unit).Parse();
			region.EndLine = parent.Unit.NextToken.Line;
			region.EndColumn = parent.Unit.NextToken.Col;
	//		Console.Write("\"\\OperatorRest\"");
		}
		
		public enum Cast {
			Implicit,
			Explicit,
			None
		}
	}
	
	/*
	public class Conversions : Member
	{
		Class parent;
		ArrayList fullName;
		ReturnType type;
		ArrayList attributes;
		ArrayList modifiers = new ArrayList(); //String[]
		bool interfaceContent = false;
		public bool InterfaceContent{
			get{
				return interfaceContent;
			}
		}
		public Parser Parser{
			get{
				return parent.Unit;
			}
		}
		public ArrayList FullName{
			get{
				return fullName;
			}
		}
		public ReturnType Type{
			get{
				return type;
			}
		}
		public Conversions(Class parent)
		{
			this.parent = parent;
		}
		public Conversions(Class parent, ArrayList attributes, ReturnType type, ArrayList fullName)
		{
			this.parent = parent;
			this.attributes = attributes;
			this.type = type;
			this.fullName = fullName;
		}
		public void Parse()
		{
			if(parent.Unit.NextToken.Equals(Token.OpenSquareBraceToken))
				parent.Unit.Attributes();
			while(parent.Unit.IsModifier(parent.Unit.NextToken) != null){
				modifiers.Add(parent.Unit.NextToken.Value.ToString());
				parent.Unit.Match();
			}
			if(!parent.Unit.NextToken.Equals(Token.ImplicitToken) && !parent.Unit.NextToken.Equals(Token.ExplicitToken))
				Console.WriteLine("\n Error: implicit or explicit expected at (" + parent.Unit.NextToken.Line + "/" + parent.Unit.NextToken.Col + ")");
			parent.Unit.Match();
			parent.Unit.Match(Token.OperatorToken);
			parent.Unit.Type();
			parent.Unit.Match(Token.OpenBraceToken);
			parent.Unit.Type();
			parent.Unit.Match(TokenType.Identifier);
			parent.Unit.Match(Token.CloseBraceToken);
			(new Stmt(parent.Unit)).Block();
		}
	}*/	
}
