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
	
	public class Event : AbstractEvent, Member
	{
		MemberParent parent;
		AttributeSectionCollection  addattributes;
		AttributeSectionCollection  removeattributes;
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
		
		public Event(MemberParent parent, AttributeSectionCollection attributes, ModifierEnum modifiers)
		{
			this.parent    = parent;
			if (parent != null && parent is Interface) {
				interfaceContent = true;
			}
			if (attributes != null && attributes.Count > 0) {
				this.attributes = attributes;
			}
			this.modifiers = modifiers;
		}
		
		public void Parse()
		{
			if (parent.Unit.NextToken.Equals(Token.OpenSquareBraceToken)) {
				attributes = parent.Unit.ParseAttributes();
			}
			modifiers |= parent.Unit.ParseModifier();
			region = new DefaultRegion(parent.Unit.NextToken.Line, parent.Unit.NextToken.Col);
			parent.Unit.Match(Token.EventToken);
			returnType = parent.Unit.Type(true);
			fullyQualifiedName = parent.Unit.NamespaceOrTypeName();
			if (interfaceContent) {
				parent.Unit.Match(Token.SemicolonToken);
				return;
			}
			if (parent.Unit.NextToken.Equals(Token.OpenCurlyBraceToken)) {
				parent.Unit.Match(Token.OpenCurlyBraceToken);
				AttributeSectionCollection att = new AttributeSectionCollection();
				if (parent.Unit.NextToken.Equals(Token.OpenSquareBraceToken)) {
					att = parent.Unit.ParseAttributes();
				}
				if (parent.Unit.NextToken.Equals(Token.AddToken)) {
					parent.Unit.Match();
					addattributes = att;
					new Block(parent.Unit).Parse();
				}
				
				if (parent.Unit.NextToken.Equals(Token.OpenSquareBraceToken)) {
					att = parent.Unit.ParseAttributes();
				}
				
				if (parent.Unit.NextToken.Equals(Token.RemoveToken)) {
					parent.Unit.Match();
					removeattributes = att;
					new Block(parent.Unit).Parse();
				}
				
				if (parent.Unit.NextToken.Equals(Token.OpenSquareBraceToken)) {
					att = parent.Unit.ParseAttributes();
				}
				
				if (parent.Unit.NextToken.Equals(Token.AddToken)) {
					parent.Unit.Match();
					addattributes = att;
					new Block(parent.Unit).Parse();
				}
				
				region.EndLine = parent.Unit.NextToken.Line;
				region.EndColumn = parent.Unit.NextToken.Col;
				parent.Unit.Match(Token.CloseCurlyBraceToken);
			} else { //todo: Create more Event Objects
				parent.Unit.VariableDeclarators(false);
				region.EndLine = parent.Unit.NextToken.Line;
				region.EndColumn = parent.Unit.NextToken.Col;
				parent.Unit.Match(Token.SemicolonToken);
			}
		}
	}
}
