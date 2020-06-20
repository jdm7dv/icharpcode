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
	
	public class Delegate : AbstractClass, Type
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
		
		public Delegate(Type parent, CompilationUnit u, IUsing usings, ModifierEnum modifiers)
		{
			unit = u;
			this.modifiers = modifiers;
			this.parent = parent;
			this.usings = usings;
			classType = ClassType.Delegate;
		}
		
		public void DelegateDeclaration()
		{
			if (unit.NextToken.Equals(Token.OpenSquareBraceToken)) {
				attributes = unit.ParseAttributes();
			}
			
			modifiers |= unit.ParseModifier();
			region = new DefaultRegion(unit.NextToken.Line, unit.NextToken.Col);
			unit.Match(Token.DelegateToken);
			
			Method method = new Method();
			Methods.Add(method);
			method.FullyQualifiedName = "Invoke";
			method.ReturnType = unit.Type(true);
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
			unit.Match(Token.OpenBraceToken);
			if (!unit.NextToken.Equals(Token.CloseBraceToken)) {
				method.Parameters = unit.ParameterList();
			}
			unit.Match(Token.CloseBraceToken);
			region.EndLine = unit.NextToken.Line;
			region.EndColumn = unit.NextToken.Col;
			unit.Match(Token.SemicolonToken);
		}
	}
	
}

