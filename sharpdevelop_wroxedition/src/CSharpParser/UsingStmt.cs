// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
namespace SharpDevelop.Internal.Parser {
	
	public class UsingStmt{
		
		CompilationUnit unit;
		int line;
		int end;
		
		public UsingStmt(CompilationUnit unit)
		{
			this.unit = unit;
		}
		
		public int Line {
			get {
				return line;
			}
		}
		
		public int End {
			get {
				return end;
			}
		}
		
		SortedList ResourceAcquisition()
		{
			SortedList variables = new SortedList(); 
			Stmt stmt = new Stmt(unit);
			if (unit.NextToken.Type == TokenType.Keyword) {
				if (unit.Lexer.keywords.IsType((KeyWordEnum)unit.NextToken.Value)) {
					if (unit.LookUpToken(1).Equals(Token.PeriodToken)) {
						(new Expr(unit)).Expression();
					}
					variables = stmt.LocalVariableDeclaration();
					return variables;
				}
				if (unit.NextToken.Equals(Token.VoidToken)) {
					variables = stmt.LocalVariableDeclaration();
					return variables;
				}
				(new Expr(unit)).Expression();
				return variables;
			}
			if (unit.NextToken.Type == TokenType.Identifier) {
				if (unit.LookUpToken(1).Equals(Token.OpenSquareBraceToken)) {
					Token t = unit.LookUpToken(2);
					if (t.Equals(Token.CommaToken) || t.Equals(Token.CloseSquareBraceToken)) {
						variables = stmt.LocalVariableDeclaration();
						return variables;
					}
					(new Expr(unit)).Expression();
					return variables;
				}
				if (unit.LookUpToken(1).Type == TokenType.Identifier) {
					variables = stmt.LocalVariableDeclaration();
					return variables;
				}
				(new Expr(unit)).Expression();
				return variables;
			}
			(new Expr(unit)).Expression();
			return variables;
		}
		
		public void Parse() //using ( resource-acquisition ) embedded-statement
		{
			SortedList variables = new SortedList();
			line = unit.NextToken.Line;
			unit.Match(Token.UsingToken);
			unit.Match(Token.OpenBraceToken);
			variables = ResourceAcquisition();//local-variable-declaration || expression 
			unit.Match(Token.CloseBraceToken);
			(new Stmt(unit)).EmbeddedStatement();
//			if(b != null){
//				foreach(string name in b.Variables.Keys){
//					variables[name] = b.Variables[name];
//				}
//			}
			foreach (string s in variables.Keys) {
				((Variable)variables[s]).Region.EndLine = unit.NextToken.Line;
				((Variable)variables[s]).Region.EndColumn = unit.NextToken.Col;
				unit.AddToLookUpTable((Variable)variables[s]);
			}
			end = unit.NextToken.Line;
		}
	}
}
