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
	
	public class Constant : AbstractField, Member
	{
		MemberParent parent;
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
		
		public Constant(MemberParent parent, AttributeSectionCollection attributes, ModifierEnum modifiers)
		{
			this.parent = parent;
			if (parent != null && parent is Interface) {
				interfaceContent = true;
			}
			if (attributes != null && attributes.Count > 0) {
				this.attributes = attributes;
			}
			this.modifiers |= modifiers;
			modifiers |= ModifierEnum.Const;
		}
		
		public void Parse()
		{
			if (parent.Unit.NextToken.Equals(Token.OpenSquareBraceToken)) {
				attributes = parent.Unit.ParseAttributes();
			}
			modifiers |= parent.Unit.ParseModifier();
			region = new DefaultRegion(parent.Unit.NextToken.Line, parent.Unit.NextToken.Col);
			parent.Unit.Match(Token.ConstToken);
			returnType = parent.Unit.Type(true);
			fullyQualifiedName = (string)parent.Unit.NextToken.Value;
			parent.Unit.Match(TokenType.Identifier);
			parent.Unit.Match(Token.AssignToken);
			new Expr(parent.Unit).Expression();
			// TODO: if more than one field is declared in this statement, 
			// only the first one is put in the parse tree.
			while (parent.Unit.NextToken.Equals(Token.CommaToken)) {
				parent.Unit.Match(TokenType.Identifier);
				parent.Unit.Match(Token.AssignToken);
				new Expr(parent.Unit).Expression();
			}
			region.EndLine = parent.Unit.NextToken.Line;
			region.EndColumn = parent.Unit.NextToken.Col;
			parent.Unit.Match(Token.SemicolonToken);
		}
	}
}
