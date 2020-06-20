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
	
	public class Enumeration : AbstractClass, Type
	{
		CompilationUnit unit;
		IUsing usings;
		Type parent;
		
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
				return ClassType.Enum;
			}
		}
		
		public Enumeration(Type parent, CompilationUnit u, IUsing usings, ModifierEnum modifiers)
		{
			unit = u;
			this.modifiers = modifiers;
			this.parent = parent;
			this.usings = usings;
		}
		
		public void EnumMembers()
		{
			while (!unit.NextToken.Equals(Token.CloseCurlyBraceToken)) {
				if (unit.NextToken.Equals(Token.OpenSquareBraceToken)) {
					unit.ParseAttributes();
				}
				Field field = new Field(this, new AttributeSectionCollection(),
				                        ModifierEnum.Const | ModifierEnum.Public, null,
				                        fullyQualifiedName + "." + unit.NextToken.Value.ToString(),
				                        new DefaultRegion(unit.NextToken.Line, unit.NextToken.Col));
				fields.Add(field);
				unit.Match(TokenType.Identifier);
				if (unit.NextToken.Equals(Token.AssignToken)) {
					unit.Match();
					new Expr(unit).Expression();
				}
				field.Region.EndLine = unit.NextToken.Line;
				field.Region.EndColumn = unit.NextToken.Col;
				if (!unit.NextToken.Equals(Token.CommaToken)) {
					return;
				}
				unit.Match(Token.CommaToken);
			}
		}
		
		void IntegralType()
		{
			if (unit.NextToken.Type != TokenType.Keyword) {
				throw new ParserException("\n Error: IntegralType expected at (" + unit.NextToken.Line + "/" + unit.NextToken.Col + ")", unit.NextToken.Line, unit.NextToken.Col);
			}
			switch ((KeyWordEnum)unit.NextToken.Value) {
				case KeyWordEnum.Sbyte:
				case KeyWordEnum.Byte:
				case KeyWordEnum.Short:
				case KeyWordEnum.Ushort:
				case KeyWordEnum.Int:
				case KeyWordEnum.Uint:
				case KeyWordEnum.Long:
				case KeyWordEnum.Ulong:
				case KeyWordEnum.Char:
					break;
				default:
					throw new ParserException("\n Error: IntegralType expected at (" + unit.NextToken.Line + "/" + unit.NextToken.Col + ")", unit.NextToken.Line, unit.NextToken.Col);
			}
			unit.Match();
		}
		
		public void EnumDeclaration()
		{
			if (unit.NextToken.Equals(Token.OpenSquareBraceToken)) {
				unit.ParseAttributes();
			}
			modifiers |= unit.ParseModifier();
			region = new DefaultRegion(unit.NextToken.Line, unit.NextToken.Col);
			unit.Match(Token.EnumToken);
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
				unit.Match();
				IntegralType();
			}
			unit.Match(Token.OpenCurlyBraceToken);
			EnumMembers();
			unit.Match(Token.CloseCurlyBraceToken);
			region.EndLine = unit.NextToken.Line;
			region.EndColumn = unit.NextToken.Col;
			if (unit.NextToken.Equals(Token.SemicolonToken)) {
				unit.Match(Token.SemicolonToken);
			}
		}
	}	
}
