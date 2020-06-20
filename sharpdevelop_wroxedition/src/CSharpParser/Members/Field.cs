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
	
	public class Field : AbstractField, Member
	{
		Type parent;
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
		
		public Field(Type parent, AttributeSectionCollection attributes, ModifierEnum modifier, ReturnType type, string fullyQualifiedName, IRegion region)
		{
			this.parent = parent;
			if (parent != null && parent is Interface) {
				interfaceContent = true;
			}
			
			if (attributes != null && attributes.Count > 0) {
				this.attributes = attributes;
			}
			
			this.modifiers = modifier;
			this.returnType = type;
			this.fullyQualifiedName = fullyQualifiedName;
			this.region = region;
		}
		
		public void Parse()
		{
			if (parent.Unit.NextToken.Equals(Token.OpenSquareBraceToken)) {
				attributes = parent.Unit.ParseAttributes();
			}
			modifiers |= parent.Unit.ParseModifier();
			region = new DefaultRegion(parent.Unit.NextToken.Line, parent.Unit.NextToken.Col);
			returnType = parent.Unit.Type(true);
			fullyQualifiedName = (string)parent.Unit.NextToken.Value;
			Unit.Match(TokenType.Identifier);
			ParseRest();
		}
		
		public void ParseRest()
		{
			if (Unit.NextToken.Equals(Token.AssignToken)) {
				Unit.Match();
				if (Unit.NextToken.Equals(Token.OpenCurlyBraceToken)) {
					parent.Unit.ArrayInitializer();
				} else if (Unit.NextToken.Equals(Token.StackallocToken)) {
					Unit.Match();
					Unit.Type(true);
					Unit.Match(Token.OpenSquareBraceToken);
					new Expr(Unit).Expression();
					Unit.Match(Token.CloseSquareBraceToken);
				} else {
					new Expr(Unit).Expression();
				}
			}
			// TODO: if more than one field is declared in this statement, 
			// only the first one is put in the parse tree.
			while (Unit.NextToken.Equals(Token.CommaToken)) {
				Unit.Match(Token.CommaToken);
				Unit.Match(TokenType.Identifier);
				if (Unit.NextToken.Equals(Token.AssignToken)) {
					Unit.Match();
					if(Unit.NextToken.Equals(Token.OpenCurlyBraceToken)) {
						parent.Unit.ArrayInitializer();
					} else if (Unit.NextToken.Equals(Token.StackallocToken)) {
						Unit.Match();
						Unit.Type(true);
						Unit.Match(Token.OpenSquareBraceToken);
						new Expr(Unit).Expression();
						Unit.Match(Token.CloseSquareBraceToken);
					} else {
						new Expr(Unit).Expression();
					}
				}
			}
			region.EndLine = parent.Unit.NextToken.Line;
			region.EndColumn = parent.Unit.NextToken.Col;
			Unit.Match(Token.SemicolonToken);
		}
	}
}
