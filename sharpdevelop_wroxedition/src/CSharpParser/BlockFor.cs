// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;

namespace SharpDevelop.Internal.Parser {
	
	public class Block : Scope
	{
		CompilationUnit unit;
		IRegion region;
		
		public IRegion Region {
			get {
				return region;
			}
		}
		
		public Block(CompilationUnit unit)
		{
			this.unit = unit;
		}
		
		public void Parse()
		{
			SortedList variables = new SortedList();
			region = new DefaultRegion(unit.NextToken.Line, unit.NextToken.Col);
			unit.Match(Token.OpenCurlyBraceToken);
			if(!unit.NextToken.Equals(Token.CloseCurlyBraceToken)){
				variables = new Stmt(unit).StatementList(Token.CloseCurlyBraceToken);
			}
			
			foreach(string s in variables.Keys){
				((Variable)variables[s]).Region.EndLine = unit.NextToken.Line;
				((Variable)variables[s]).Region.EndColumn = unit.NextToken.Col;
//				unit.AddToLookUpTable((Variable)variables[s]);
			}
			region.EndLine = unit.NextToken.Line;
			region.EndColumn = unit.NextToken.Col;
			unit.Match(Token.CloseCurlyBraceToken);
		}
	}
	
	public class ForStatement : Scope
	{
		CompilationUnit unit;
		Stmt stmt;
		IRegion region;
//		int line;
//		int end;
		
		public IRegion Region {
			get {
				return region;
			}
		}
		
		public ForStatement(CompilationUnit unit)
		{
			this.unit = unit;
			stmt = new Stmt(unit);
		}
		
		SortedList ForInitializer(){//copied to Statement()
		//local-variable-declaration || statement-expression-list 
		//		Console.WriteLine(" \"ForInitializer\"");
			if(unit.NextToken.Type == TokenType.Keyword){
				if(unit.Lexer.keywords.IsType((KeyWordEnum)unit.NextToken.Value)){
					if(unit.LookUpToken(1).Equals(Token.PeriodToken))
						stmt.StatementExpressionList();
					return stmt.LocalVariableDeclaration();
				}
				if(unit.NextToken.Equals(Token.VoidToken))
					return stmt.LocalVariableDeclaration();
				stmt.StatementExpressionList();
				return null;
			}
			if(unit.NextToken.Type == TokenType.Identifier){
				if(unit.LookUpToken(1).Equals(Token.OpenSquareBraceToken)){
					Token t = unit.LookUpToken(2);
					if(t.Equals(Token.CommaToken) || t.Equals(Token.CloseSquareBraceToken))
						return stmt.LocalVariableDeclaration();
					stmt.StatementExpressionList();
					return null;
				}
				if(unit.LookUpToken(1).Type == TokenType.Identifier){
					return stmt.LocalVariableDeclaration();
				}
				if(unit.LookUpToken(1).Equals(Token.PeriodToken)){
					string a = unit.NamespaceOrTypeName();
					if(unit.NextToken.Type == TokenType.Identifier){//LocalVariableDeclaration
						ReturnType type = new ReturnType(a, null, 0);
						SortedList l = stmt.VariableDeclarators();
						foreach(Variable v in l.Values){
							v.ReturnType = type;
						}
						unit.Match(Token.SemicolonToken);
						return l;
					}
					//StatementExpression
					Expr e = new Expr(unit);
					e.PrimaryRest();
					//Copied from StatementExpression:
					if(unit.NextToken.Equals(Token.OpenBraceToken)){//invocation-expression = primary-expression ( argument-listopt ) 
						unit.Match();
						e.ArgumentList();
						unit.Match(Token.CloseBraceToken);
					}else if(unit.NextToken.Equals(Token.IncrementToken))//post-increment-expression
						unit.Match();
					else if(unit.NextToken.Equals(Token.DecrementToken))//post-decrement-expression
						unit.Match();
					if(e.AssignmentOperator())
						e.Expression();
					return null;
				}
				stmt.StatementExpressionList();
				return null;
			}
			stmt.StatementExpressionList();
			return null;
	//		Console.WriteLine(" \"\\ForInitializer\"");
		}
		
		public void Parse()
		{
			SortedList variables = new SortedList();
			region = new DefaultRegion(unit.NextToken.Line, unit.NextToken.Col);
			unit.Match(Token.ForToken);
			unit.Match(Token.OpenBraceToken);
			if(!unit.NextToken.Equals(Token.SemicolonToken))
				variables = ForInitializer();//local-variable-declaration || statement-expression-list 
			unit.Match(Token.SemicolonToken);
			if(!unit.NextToken.Equals(Token.SemicolonToken))
				(new Expr(unit)).Expression();
			unit.Match(Token.SemicolonToken);
			if(!unit.NextToken.Equals(Token.CloseBraceToken))
				stmt.StatementExpressionList();
			unit.Match(Token.CloseBraceToken);
			stmt.EmbeddedStatement();
			if(variables != null) {
				foreach(string s in variables.Keys){
					((Variable)variables[s]).Region.EndLine = unit.NextToken.Line;
					((Variable)variables[s]).Region.EndColumn = unit.NextToken.Col;
					unit.AddToLookUpTable((Variable)variables[s]);
				}
			}
			region.EndLine = unit.NextToken.Line;
			region.EndColumn = unit.NextToken.Col;
		}
	}
}
