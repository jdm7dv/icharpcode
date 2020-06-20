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
	
	public class Interface : AbstractClass, MemberParent
	{
		CompilationUnit unit;
		Type parent;
		IUsing usings;
		
		public Type Parent {
			get {
				return parent;
			}
		}
		public override ICompilationUnit CompilationUnit {
			get {
				return unit;
			}
		}
		
		public CompilationUnit Unit {
			get {
				return unit;
			}
		}
		
		public IUsing Usings {
			get {
				return usings;
			}
		}
		
		public override ClassType ClassType {
			get {
				return ClassType.Interface;
			}
		}
		
		public Interface(Type parent, CompilationUnit u, IUsing usings, ModifierEnum modifiers)
		{
			unit = u;
			this.parent = parent;
			this.usings = usings;
			this.modifiers = modifiers;
		}
		
		public Property.GetSet Accessors()
		{
	//		Console.Write(" \"Accessors\" ");
			bool hasget = false;
			bool hasset = false;
			if (unit.NextToken.Equals(Token.GetToken)) {
				hasget = true;
				unit.Match();
				unit.Match(Token.SemicolonToken);
			}
			if (unit.NextToken.Equals(Token.OpenSquareBraceToken)) {
				unit.ParseAttributes();
			}
			if (unit.NextToken.Equals(Token.SetToken)) {
				hasset = true;
				unit.Match();
				unit.Match(Token.SemicolonToken);
			}
			if (unit.NextToken.Equals(Token.OpenSquareBraceToken)) {
				unit.ParseAttributes();
			}
			if (unit.NextToken.Equals(Token.GetToken)) {
				if (hasget) {
					throw new ParserException("\n Error: not two \"get\" allowed at (" + unit.NextToken.Line + "/" + unit.NextToken.Col + ")", unit.NextToken.Line, unit.NextToken.Col);
				} else {
					hasget = true;
					unit.Match();
					unit.Match(Token.SemicolonToken);
				}
			}
			if (hasset && hasget) {
				return Property.GetSet.Both;
			}
			if (hasset) {
				return Property.GetSet.Set;
			}
			if (hasget) {
				return Property.GetSet.Get;
			}
	//		Console.Write(" \"\\Accessors\" ");
			return Property.GetSet.Nothing;
		}
		
		void InterfaceMember()
		{
	//		Console.Write(" \"InterfaceMembers\" ");
			if (unit.NextToken.Equals(Token.CloseCurlyBraceToken)) {
				return;
			}
			bool found = false;
			AttributeSectionCollection attributes = new AttributeSectionCollection();
			if (unit.NextToken.Equals(Token.OpenSquareBraceToken)) {
				attributes = unit.ParseAttributes();
			}
			ModifierEnum memberModifiers = unit.ParseModifier();
			
			if (memberModifiers == ModifierEnum.None) {
				memberModifiers = ModifierEnum.Public;
			}
			if (unit.NextToken.Type == TokenType.Keyword) { 
				found = true;
				KeyWordEnum w = (KeyWordEnum)unit.NextToken.Value;
				switch(w) {
					case KeyWordEnum.Event:
						Event e = new Event(this, attributes, memberModifiers);
						events.Add(e);
						e.Parse();
						break;
					case KeyWordEnum.Void:
						Method m = new Method(this, attributes, memberModifiers);
						methods.Add(m);
						m.Parse();
						break;
					default:
						if (unit.Lexer.keywords.IsType(w)) {
							found = false;
						} else {
							throw new ParserException("\n Error: InterfaceMember expected at (" + unit.NextToken.Line + "/" + unit.NextToken.Col + ")", unit.NextToken.Line, unit.NextToken.Col);
						}
						break;
				}
			}
			if (!found) {
				IRegion memberRegion = new DefaultRegion(unit.NextToken.Line, unit.NextToken.Col);
				ReturnType t = unit.Type(true);
				string name = "";
				if (unit.NextToken.Equals(Token.ThisToken)) {
					Indexer i1 = new Indexer(this, attributes, memberModifiers, t, memberRegion);
					indexer.Add(i1);
					i1.ParseRest();
				}
				while (unit.NextToken.Type == TokenType.Identifier) {
					name += (string)unit.NextToken.Value;
					unit.Match(TokenType.Identifier);
					if (!unit.NextToken.Equals(Token.PeriodToken)) {
						break;
					}
				}
				if (unit.NextToken.Equals(Token.ThisToken)) {
					Indexer i2 = new Indexer(this, attributes, memberModifiers, t, memberRegion);
					indexer.Add(i2);
					i2.ParseRest();
				} else if(unit.NextToken.Equals(Token.OpenBraceToken)) {
					Method m = new Method(this, attributes, memberModifiers, t, name, memberRegion);
					methods.Add(m);
					m.ParseRest();
				} else if(unit.NextToken.Equals(Token.OpenCurlyBraceToken)) {
					Property p = new Property(this, attributes, memberModifiers, t, name, memberRegion);
					properties.Add(p);
					p.ParseRest();
				} else {
					throw new ParserException("\n Error: (, {, this or , expected at (" + unit.NextToken.Line + "/" + unit.NextToken.Col + ")", unit.NextToken.Line, unit.NextToken.Col);
				}
			}
	//		Console.Write(" \"\\InterfaceMembers\" ");
		}
		
		void InterfaceBase()
		{
	//		Console.Write("\"ClassBase\"");
			unit.Match(Token.ColonToken);
			if (unit.NextToken.Type == TokenType.Keyword) {
				if ((KeyWordEnum)unit.NextToken.Value != KeyWordEnum.Object && (KeyWordEnum)unit.NextToken.Value != KeyWordEnum.String) {
					throw new ParserException("\n Error: InterfaceType expected at (" + unit.NextToken.Line + "/" + unit.NextToken.Col + ")", unit.NextToken.Line, unit.NextToken.Col);
				} else {
					baseTypes.Add(unit.NextToken.Value.ToString());
					unit.Match();
					if (!unit.NextToken.Equals(Token.CommaToken)) {
						return;
					} else {
						unit.Match(Token.CommaToken);
					}
				}
			}
			StringCollection col = unit.Inheritance();
			string[] s = new string[col.Count];
			col.CopyTo(s, 0);
			baseTypes.AddRange(s);
	//		Console.Write("\"\\ClassBase\"");
		}
		
		public void InterfaceDeclaration()
		{
	//		Console.Write(" \"InterfaceDeclaration\" ");
			if (unit.NextToken.Equals(Token.OpenSquareBraceToken)) {
				unit.ParseAttributes();
			}
			modifiers |= Unit.ParseModifier();
			region = new DefaultRegion(unit.NextToken.Line, unit.NextToken.Col);
			unit.Match(Token.InterfaceToken);
			this.fullyQualifiedName = parent.FullyQualifiedName;
			fullyQualifiedName += '.';
			if (fullyQualifiedName.StartsWith("Global")) {
				if (fullyQualifiedName.Length <= 7) {
					fullyQualifiedName = "";
				} else {
					fullyQualifiedName.Remove(0, 7);
				}
			}
			fullyQualifiedName += (string)unit.NextToken.Value;
			unit.Match(TokenType.Identifier);
			if (unit.NextToken.Equals(Token.ColonToken)) {
				InterfaceBase();
//				unit.Match();
//				unit.Inheritance();
			}
			unit.Match(Token.OpenCurlyBraceToken);
			while (!unit.NextToken.Equals(Token.CloseCurlyBraceToken)) {
				InterfaceMember();
			}
			unit.Match(Token.CloseCurlyBraceToken);
			region.EndLine = unit.NextToken.Line;
			region.EndColumn = unit.NextToken.Col;
			if (unit.NextToken.Equals(Token.SemicolonToken)) {
				unit.Match(Token.SemicolonToken);
			}
	//		Console.Write(" \"\\InterfaceDeclaration\" ");
		}
	}
	
}

