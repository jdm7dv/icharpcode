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
	
	public class Constructor : AbstractMethod, Member
	{
		Class parent;
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
		
		public Constructor(Class parent)
		{
			this.parent = parent;
		}
		
		public Constructor(Class parent, AttributeSectionCollection attributes, ModifierEnum modifiers)
		{
			this.parent = parent;
			if (attributes != null && attributes.Count > 0) {
				this.attributes = attributes;
			}
			this.modifiers = modifiers;
		}
		
		public void Parse()
		{
			if (parent.Unit.NextToken.Equals(Token.OpenSquareBraceToken)) {
				parent.Unit.ParseAttributes();
			}
			modifiers |= parent.Unit.ParseModifier();
			region = new DefaultRegion(parent.Unit.NextToken.Line, parent.Unit.NextToken.Col);
			fullyQualifiedName = (string)parent.Unit.NextToken.Value;
			parent.Unit.Match(TokenType.Identifier);
			parent.Unit.Match(Token.OpenBraceToken);
			
			if (!parent.Unit.NextToken.Equals(Token.CloseBraceToken)) {
				parameters = parent.Unit.ParameterList();
			}
			parent.Unit.Match(Token.CloseBraceToken);
			if (parent.Unit.NextToken.Equals(Token.ColonToken)) {
				parent.Unit.Match(Token.ColonToken);
				if (!parent.Unit.NextToken.Equals(Token.BaseToken) && !parent.Unit.NextToken.Equals(Token.ThisToken)) {
					throw new ParserException("\n Error: base or this expected at (" + parent.Unit.NextToken.Line + "/" + parent.Unit.NextToken.Col + ")\nWas: " + parent.Unit.NextToken.Value, parent.Unit.NextToken.Line, parent.Unit.NextToken.Col);
				}
				parent.Unit.Match();
				parent.Unit.Match(Token.OpenBraceToken);
				if (!parent.Unit.NextToken.Equals(Token.CloseBraceToken)) {
					new Expr(parent.Unit).ArgumentList();
				}
				parent.Unit.Match(Token.CloseBraceToken);
			}
			new Block(parent.Unit).Parse();
			region.EndLine = parent.Unit.NextToken.Line;
			region.EndColumn = parent.Unit.NextToken.Col;
		}
		
	}
	
}

