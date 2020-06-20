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
	
	public class Indexer : AbstractIndexer, Member
	{
		MemberParent parent;
		Property.GetSet hasGetOrSet;
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
		
		public Indexer(MemberParent parent)
		{
			System.Diagnostics.Debug.Assert(parent != null);
			this.parent = parent;
			if (parent != null && parent is Interface) {
				interfaceContent = true;
			}
			this.fullyQualifiedName = parent.FullyQualifiedName;
		}
		public Indexer(MemberParent parent, AttributeSectionCollection attributes, ModifierEnum modifiers, ReturnType type, IRegion region)
		{
			System.Diagnostics.Debug.Assert(parent != null);
			this.parent = parent;
			if (parent != null && parent is Interface) {
				interfaceContent = true;
			}
			this.fullyQualifiedName = parent.FullyQualifiedName;
			this.modifiers = modifiers;
			this.attributes = attributes;
			this.returnType = type;
			this.region = region;
		}
		
		public void Parse()
		{
			if (parent.Unit.NextToken.Equals(Token.OpenSquareBraceToken)) {
				parent.Unit.ParseAttributes();
			}
			modifiers |= parent.Unit.ParseModifier();
			region = new DefaultRegion(parent.Unit.NextToken.Line, parent.Unit.NextToken.Col);
			parent.Unit.Type(true);
			ParseRest();
		}
		
		public void ParseRest()
		{
			while (parent.Unit.NextToken.Type == TokenType.Identifier) {
				parent.Unit.Match(TokenType.Identifier);
				parent.Unit.Match(Token.PeriodToken);
			}
			
			parent.Unit.Match(Token.ThisToken);
			parent.Unit.Match(Token.OpenSquareBraceToken);
			
			parameters = parent.Unit.ParameterList();
			parent.Unit.Match(Token.CloseSquareBraceToken);
			parent.Unit.Match(Token.OpenCurlyBraceToken);
			
			hasGetOrSet = parent.Accessors();
			region.EndLine = parent.Unit.NextToken.Line;
			region.EndColumn = parent.Unit.NextToken.Col;
			parent.Unit.Match(Token.CloseCurlyBraceToken);
		}
	}	
}
