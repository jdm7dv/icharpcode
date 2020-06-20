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
	
	public class Method : AbstractMethod, Member
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
		
		public CompilationUnit Unit {
			get {
				return parent.Unit;
			}
		}
		
		public Method()
		{
			this.region = new DefaultRegion(-1, -1, -1, -1);
		}
		
		public Method(MemberParent parent, AttributeSectionCollection attributes, ModifierEnum modifiers)
		{
			this.parent = parent;
			if (parent != null && parent is Interface) {
				interfaceContent = true;
			}
			if (attributes != null && attributes.Count > 0) {
				this.attributes = attributes;
			}
			this.modifiers = modifiers;
		}
		
		public Method(MemberParent parent, AttributeSectionCollection attributes, ModifierEnum modifiers, ReturnType type, string fullyQualifiedName, IRegion region)
		{
			this.parent = parent;
			if (parent != null && parent is Interface) {
				interfaceContent = true;
			}
			if (attributes != null && attributes.Count > 0) {
				this.attributes = attributes;
			}
			this.modifiers = modifiers;
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
			fullyQualifiedName = parent.Unit.NamespaceOrTypeName();
			ParseRest();
		}
		
		public void ParseRest()
		{
			parent.Unit.Match(Token.OpenBraceToken);
			if (!parent.Unit.NextToken.Equals(Token.CloseBraceToken)) {
				parameters = parent.Unit.ParameterList();
			}
			parent.Unit.Match(Token.CloseBraceToken);
			if (parent.Unit.NextToken.Equals(Token.SemicolonToken) || parent is Interface) {
				parent.Unit.Match(Token.SemicolonToken);
			} else {
				new Block(parent.Unit).Parse();
			}
			region.EndLine = parent.Unit.NextToken.Line;
			region.EndColumn = parent.Unit.NextToken.Col;
		}
	}
	
}
