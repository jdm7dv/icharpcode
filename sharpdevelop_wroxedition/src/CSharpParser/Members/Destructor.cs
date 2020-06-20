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
	
	public class Destructor : AbstractMethod, Member
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
		
		public Destructor(Class parent, AttributeSectionCollection attributes)
		{
			this.parent = parent;
			fullyQualifiedName = parent.FullyQualifiedName + "~";
			if (attributes != null && attributes.Count > 0) {
				this.attributes = attributes;
			}
		}
		
		public void Parse()
		{
			if (parent.Unit.NextToken.Equals(Token.OpenSquareBraceToken)) {
				attributes = parent.Unit.ParseAttributes();
			}
			if (parent.IsStruct) {
				throw new ParserException("\n Error: Structs don't have Destructors: at (" + parent.Unit.NextToken.Line + "/" + parent.Unit.NextToken.Col + ")", parent.Unit.NextToken.Line, parent.Unit.NextToken.Col);
			}
			region = new DefaultRegion(parent.Unit.NextToken.Line, parent.Unit.NextToken.Col);
			parent.Unit.Match(Token.BitwiseComplementToken);
			fullyQualifiedName += parent.Unit.NextToken.Value.ToString();
			parent.Unit.Match(TokenType.Identifier);
			parent.Unit.Match(Token.OpenBraceToken);
			parent.Unit.Match(Token.CloseBraceToken);
			new Block(parent.Unit).Parse();
			region.EndLine = parent.Unit.NextToken.Line;
			region.EndColumn = parent.Unit.NextToken.Col;
		}
	}
}
