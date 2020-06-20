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
	
	public class Class : AbstractClass, MemberParent
	{
		CompilationUnit unit;
		bool isStruct = false;
		Type parent;
		IUsing     usings;
		
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
		
		public bool IsStruct {
			get {
				return isStruct;
			}
		}
		
		public override ClassType ClassType {
			get {
				if (isStruct) {
					return ClassType.Struct;
				} else {
					return ClassType.Class;
				}
			}
		}
		
//		[ObsoleteAttribute("This property will be removed from future Versions.Use Region instead",false)]
		
		public IUsing Usings {
			get {
				return usings;
			}
		}
		
		public Class(Type parent, CompilationUnit unit, IUsing usings, ModifierEnum modifiers)
		{
//			if (unit == null) {
//				throw new Exception("Unit == null is class Constructor");
//			}
			this.parent = parent;
			this.unit   = unit;
			this.usings = usings;
			this.modifiers = modifiers;
		}
		
		public Class(Type parent, CompilationUnit unit, IUsing usings, ModifierEnum modifiers, bool isStruct) : this(parent, unit, usings, modifiers)
		{
//			if (unit == null) {
//				throw new Exception("Unit == null is class Constructor2");
//			}
			this.isStruct = isStruct;
		}
		
		public Property.GetSet Accessors()
		{
	//		Console.Write("\"Accessor\"");
			bool hasget = false;
			bool hasset = false;
			if (unit.NextToken.Equals(Token.OpenSquareBraceToken)) {
				unit.ParseAttributes();
			}
			if (unit.NextToken.Equals(Token.GetToken)) {
				hasget = true;
				unit.Match();
				if (unit.NextToken.Equals(Token.SemicolonToken)) {
					unit.Match(Token.SemicolonToken);
				} else {
					new Block(unit).Parse();
				}
			}
			if (unit.NextToken.Equals(Token.OpenSquareBraceToken)) {
				unit.ParseAttributes();
			}
			if (unit.NextToken.Equals(Token.SetToken)) {
				hasset = true;
				unit.Match();
				if (unit.NextToken.Equals(Token.SemicolonToken)) {
					unit.Match(Token.SemicolonToken);
				} else {
					new Block(unit).Parse();
				}
			}
			if (unit.NextToken.Equals(Token.OpenSquareBraceToken)) {
				unit.ParseAttributes();
			}
			if (unit.NextToken.Equals(Token.GetToken)) {
				if (hasget) {
					throw new ParserException("\n Error: not two \"get\" allowed at (" + unit.NextToken.Line + "/" + unit.NextToken.Col + ")", unit.NextToken.Line, unit.NextToken.Col);
				} else {
					hasget = true;
				}
				unit.Match();
				if (unit.NextToken.Equals(Token.SemicolonToken)) {
					unit.Match(Token.SemicolonToken);
				} else {
					new Block(unit).Parse();
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
	//		Console.Write("\"\\Accessor\"");
			return Property.GetSet.Nothing;
		}
		
		void ClassBase()
		{
	//		Console.Write("\"ClassBase\"");
			unit.Match(Token.ColonToken);
			if (unit.NextToken.Type == TokenType.Keyword) {
				if ((KeyWordEnum)unit.NextToken.Value != KeyWordEnum.Object && (KeyWordEnum)unit.NextToken.Value != KeyWordEnum.String) {
					throw new ParserException("\n Error: ClassType expected at (" + unit.NextToken.Line + "/" + unit.NextToken.Col + ")", unit.NextToken.Line, unit.NextToken.Col);
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
		
		void ClassMember()
		{
	//		Console.Write(" \"ClassMembers\" ");
			if (unit.NextToken.Equals(Token.CloseCurlyBraceToken)) {
				return;
			}
			bool found = false;
			if (unit.NextToken.Equals(Token.OpenSquareBraceToken)) {
				attributes = unit.ParseAttributes();
			}
			ModifierEnum memberModifier = Unit.ParseModifier();
			if (memberModifier == ModifierEnum.None) {
				memberModifier = ModifierEnum.Private;
			}
			if (unit.NextToken.Type == TokenType.Keyword) {
				found = true;
				KeyWordEnum w = (KeyWordEnum)unit.NextToken.Value;
				switch(w) {
					case KeyWordEnum.Const:
						Constant c = new Constant(this, attributes, memberModifier);
						fields.Add(c);
						c.Parse();
						break;
					case KeyWordEnum.Event:
						Event e = new Event(this, attributes, memberModifier);
						events.Add(e);
						e.Parse();
						break;
					case KeyWordEnum.Implicit://Conversion Operator
						Operator imO = new Operator(this, attributes, memberModifier, Operator.Cast.Implicit);
						methods.Add(imO);
						imO.ConversionOperator();
						break;
					case KeyWordEnum.Explicit://Conversion Operator
						Operator exO = new Operator(this, attributes, memberModifier, Operator.Cast.Explicit);
						methods.Add(exO);
						exO.ConversionOperator();
						break;
					case KeyWordEnum.Void:
						Method m = new Method(this, attributes, memberModifier);
						methods.Add(m);
						m.Parse();
						break;
					case KeyWordEnum.Class:
						Class cl = new Class(this, unit, usings, memberModifier);
						innerClasses.Add(cl);
						cl.ClassDeclaration();
						break;
					case KeyWordEnum.Struct:
						Class str = new Class(this, unit, usings, memberModifier, true);
						innerClasses.Add(str);
						str.StructDeclaration();
						break;
					case KeyWordEnum.Interface:
						Interface inter = new Interface(this, unit, usings, memberModifier);
						innerClasses.Add(inter);
						inter.InterfaceDeclaration();
						break;
					case KeyWordEnum.Enum:
						Enumeration en = new Enumeration(this, unit, usings, memberModifier);
						innerClasses.Add(en);
						en.EnumDeclaration();
						break;
					case KeyWordEnum.Delegate:
						Delegate del = new Delegate(this, unit, usings, memberModifier);
						innerClasses.Add(del);
						del.DelegateDeclaration();
						break;
					default:
						if (unit.Lexer.keywords.IsType(w)) {
							found = false;
						} else {
							throw new ParserException("\n Error: ClassMember expected at (" + unit.NextToken.Line + "/" + unit.NextToken.Col + ")", unit.NextToken.Line, unit.NextToken.Col);
						}
						break;
				}
			} else if (unit.NextToken.Equals(Token.BitwiseComplementToken)) {
				found = true;
				Destructor d = new Destructor(this, attributes);
				methods.Add(d);
				d.Parse();
			} else if (unit.NextToken.Type == TokenType.Identifier && unit.LookUpToken(1).Equals(Token.OpenBraceToken)) {
				found = true;
				Constructor c = new Constructor(this, attributes, memberModifier);
				methods.Add(c);
				c.Parse();
			}
			
			if (!found) {
				IRegion memberRegion = new DefaultRegion(unit.NextToken.Line, unit.NextToken.Col);
				ReturnType t = unit.Type(true);
				string name = "";
				if (unit.NextToken.Equals(Token.OperatorToken)) {
					Operator o = new Operator(this, attributes, memberModifier, memberRegion, t);
					methods.Add(o);
					o.ParseRest();
				} else if (unit.NextToken.Equals(Token.ThisToken)) {
					Indexer i = new Indexer(this, attributes, memberModifier, t, memberRegion);
					indexer.Add(i);
					i.ParseRest();
				} else {
					while (unit.NextToken.Type == TokenType.Identifier) {
						name += unit.NextToken.Value.ToString();
						unit.Match(TokenType.Identifier);
						if (!unit.NextToken.Equals(Token.PeriodToken)) {
							break;
						}
						unit.Match(Token.PeriodToken);
					}
					if (unit.NextToken.Equals(Token.ThisToken)) {
						Indexer i = new Indexer(this, attributes, memberModifier, t, memberRegion);
						indexer.Add(i);
						i.ParseRest();
					} else if (unit.NextToken.Equals(Token.OpenBraceToken)) {
						Method m = new Method(this, attributes, memberModifier, t, name, memberRegion);
						methods.Add(m);
						m.ParseRest();
					} else if (unit.NextToken.Equals(Token.OpenCurlyBraceToken)) {
						Property p = new Property(this, attributes, memberModifier, t, name, memberRegion);
						properties.Add(p);
						p.ParseRest();
					} else if (unit.NextToken.Equals(Token.CommaToken) || unit.NextToken.Equals(Token.SemicolonToken) ||
						       unit.NextToken.Equals(Token.AssignToken)) {
							Field f = new Field(this, attributes, memberModifier, t, name, memberRegion);
							fields.Add(f);
							f.ParseRest();
					} else { 
						throw new ParserException("\n Error: (, {, this or , expected at (" + unit.NextToken.Line + "/" + unit.NextToken.Col + ")", unit.NextToken.Line, unit.NextToken.Col);
					}
				}
			}
	//		Console.Write(" \"\\ClassMembers\" ");
		}
		
		public void StructDeclaration()
		{
	//		Console.Write("\"StructDeclaration\"");
			if (unit.NextToken.Equals(Token.OpenSquareBraceToken)) {
				attributes = unit.ParseAttributes();
			}
			modifiers |= Unit.ParseModifier();
			region = new DefaultRegion(unit.NextToken.Line, unit.NextToken.Col);
			unit.Match(Token.StructToken);
			fullyQualifiedName = parent.FullyQualifiedName;
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
				baseTypes = unit.Inheritance();
			}
			unit.Match(Token.OpenCurlyBraceToken);
			while (!unit.NextToken.Equals(Token.CloseCurlyBraceToken)) {
				ClassMember();
			}
			unit.Match(Token.CloseCurlyBraceToken);
			region.EndLine = unit.NextToken.Line;
			region.EndColumn = unit.NextToken.Col;
			if (unit.NextToken.Equals(Token.SemicolonToken)) {
				unit.Match();
			}
	//		Console.Write("\"\\StructDeclaration\"");
		}
	
		public void ClassDeclaration()
		{
//			Console.Write(" \"ClassDeclaration\" ");
			if (unit.NextToken.Equals(Token.OpenSquareBraceToken)) {
				unit.ParseAttributes();
			}
			modifiers |= unit.ParseModifier();
			region = new DefaultRegion(unit.NextToken.Line, unit.NextToken.Col);
			unit.Match(Token.ClassToken);
			fullyQualifiedName = parent.FullyQualifiedName;
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
				ClassBase();
			}
			unit.Match(Token.OpenCurlyBraceToken);
			while (!unit.NextToken.Equals(Token.CloseCurlyBraceToken)) {
				ClassMember();
			}
			region.EndLine = unit.NextToken.Line;
			region.EndColumn = unit.NextToken.Col;
			unit.Match(Token.CloseCurlyBraceToken);
			if (unit.NextToken.Equals(Token.SemicolonToken)) {
				unit.Match();
			}
//			Console.Write(" \"\\ClassDeclaration\" ");
		}
	}
}
