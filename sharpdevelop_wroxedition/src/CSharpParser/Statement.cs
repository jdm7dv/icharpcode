// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
namespace SharpDevelop.Internal.Parser {
	
	public class Stmt
	{
		CompilationUnit unit;
		
		public Stmt(CompilationUnit unit)
		{
			this.unit = unit;
		}
		/*
		public void Block()
		{
			unit.Match(Token.OpenCurlyBraceToken);
			if(!unit.NextToken.Equals(Token.CloseCurlyBraceToken)){
				unit.Parser.StatementList(this, Token.CloseCurlyBraceToken);
			}
			unit.Match(Token.CloseCurlyBraceToken);
		}
		*/
		void IfStatement()
		{
			unit.Match();
			unit.Match(Token.OpenBraceToken);
			(new Expr(unit)).Expression();
			unit.Match(Token.CloseBraceToken);
			EmbeddedStatement();
			if (unit.NextToken.Equals(Token.ElseToken)) {
				unit.Match();
				EmbeddedStatement();
			}
		}
		
		void SwitchStatement()
		{
			SortedList all = new SortedList();
			unit.Match();
			unit.Match(Token.OpenBraceToken);
			(new Expr(unit)).Expression();
			unit.Match(Token.CloseBraceToken);
			unit.Match(Token.OpenCurlyBraceToken);
			while (!unit.NextToken.Equals(Token.CloseCurlyBraceToken)) {
				SortedList l = SwitchSection();
				if (l != null) {
					foreach (string s in l.Keys) {
						all[s] = l[s];
					}
				}
			}
			if (all != null) {
				foreach (Variable v in all.Values) {
					v.Region.EndLine = unit.NextToken.Line;
					v.Region.EndColumn = unit.NextToken.Col;
				}
			}
			unit.Match(Token.CloseCurlyBraceToken);
		}
		
		SortedList SwitchSection()
		{
			SortedList all = new SortedList();
			while (unit.NextToken.Equals(Token.CaseToken) || unit.NextToken.Equals(Token.DefaultToken)) {
				if (unit.NextToken.Equals(Token.CaseToken)) {
					unit.Match();
					(new Expr(unit)).Expression();
				} else {
					unit.Match(Token.DefaultToken);
				}
				unit.Match(Token.ColonToken);
			}
			do {
				SortedList l = Statement();
				if (l != null) {
					foreach (string s in l.Keys) {
						all[s] = l[s];
					}
				}
			} while (!(unit.NextToken.Type == TokenType.EOF) && !unit.NextToken.Equals(Token.CloseCurlyBraceToken) &&
				   !unit.NextToken.Equals(Token.CaseToken) &&
				   !unit.NextToken.Equals(Token.DefaultToken));
			return all;
		}
		
		void WhileStatement()
		{
			unit.Match();
			unit.Match(Token.OpenBraceToken);
			(new Expr(unit)).Expression();
			unit.Match(Token.CloseBraceToken);
			EmbeddedStatement();
		}
		
		void DoWhileStatement()
		{
			unit.Match();
			EmbeddedStatement();
			unit.Match(Token.WhileToken);
			unit.Match(Token.OpenBraceToken);
			(new Expr(unit)).Expression();
			unit.Match(Token.CloseBraceToken);
			unit.Match(Token.SemicolonToken);
		}
		/*
		void ForInitializer(){//copied to Statement()
		//local-variable-declaration || statement-expression-list 
		//		Console.WriteLine(" \"ForInitializer\"");
			if(unit.NextToken.Type == TokenType.Keyword){
				if(unit.Parser.Lexer.keywords.IsType((EKeyWord)unit.NextToken.Value)){
					if(unit.Parser.LookUpToken(1).Equals(Token.PeriodToken))
						StatementExpressionList();
					LocalVariableDeclaration();
				}else if(unit.NextToken.Equals(Token.VoidToken)) {
					LocalVariableDeclaration();
				}else
					StatementExpressionList();
			}else if(unit.NextToken.Type == TokenType.Identifier){
				if(unit.Parser.LookUpToken(1).Equals(Token.OpenSquareBraceToken)){
					Token t = unit.Parser.LookUpToken(2);
					if(t.Equals(Token.CommaToken) || t.Equals(Token.CloseSquareBraceToken)){
						LocalVariableDeclaration();
					}else
						StatementExpressionList();
				}else if(unit.Parser.LookUpToken(1).Type == TokenType.Identifier){
					LocalVariableDeclaration();
				}else if(unit.Parser.LookUpToken(1).Equals(Token.PeriodToken)){
					ArrayList a = unit.Parser.NamespaceOrTypeName();
					if(unit.NextToken.Type == TokenType.Identifier){//LocalVariableDeclaration
						VariableDeclarators();
						unit.Match(Token.SemicolonToken);
					}else{//StatementExpression
						Expr e = new Expr(unit.Parser);
						e.PrimaryRest();
						//Copied from StatementExpression:
						if(unit.NextToken.Equals(Token.OpenBraceToken)) { //invocation-expression = primary-expression ( argument-listopt ) 
							unit.Match();
							unit.Parser.ArgumentList(e);
							unit.Match(Token.CloseBraceToken);
						}else if(unit.NextToken.Equals(Token.IncrementToken)) //post-increment-expression
							unit.Match();
						else if(unit.NextToken.Equals(Token.DecrementToken))//post-decrement-expression
							unit.Match();
						if(e.AssignmentOperator())
							e.Expression();
					}
				}else
					StatementExpressionList();
			}else
				StatementExpressionList();
	//		Console.WriteLine(" \"\\ForInitializer\"");
		}
		
		void ForStatement()
		{
	//		Console.WriteLine(" \"ForStatement\"");
			unit.Match();
			unit.Match(Token.OpenBraceToken);
			if(!unit.NextToken.Equals(Token.SemicolonToken))
				ForInitializer();//local-variable-declaration || statement-expression-list 
			unit.Match(Token.SemicolonToken);
			if(!unit.NextToken.Equals(Token.SemicolonToken))
				(new Expr(unit.Parser)).Expression();
			unit.Match(Token.SemicolonToken);
			if(!unit.NextToken.Equals(Token.CloseBraceToken))
				StatementExpressionList();
			unit.Match(Token.CloseBraceToken);
			EmbeddedStatement();
	//		Console.WriteLine(" \"\\ForStatement\"");
		}
		*/
		void ForeachStatement()
		{
			unit.Match();
			unit.Match(Token.OpenBraceToken);
			ReturnType type = unit.Type(true);
			Variable v = new Variable((string)unit.NextToken.Value, new DefaultRegion(unit.NextToken.Line, unit.NextToken.Col));
			v.ReturnType = type;
			unit.AddToLookUpTable(v);
			unit.Match(TokenType.Identifier);
			unit.Match(Token.InToken);
			(new Expr(unit)).Expression();
			unit.Match(Token.CloseBraceToken);
			EmbeddedStatement();
			v.Region.EndLine = unit.NextToken.Line;
			v.Region.EndColumn = unit.NextToken.Col;
		}
		
		bool IsJumpKeyword(Token t)
		{
			if (unit.NextToken.Type != TokenType.Keyword) {
				return false;
			}
			switch ((KeyWordEnum)t.Value) {
				case KeyWordEnum.Break:
				case KeyWordEnum.Continue:
				case KeyWordEnum.Goto:
				case KeyWordEnum.Return:
				case KeyWordEnum.Throw:
					return true;
				default:
					return false;
			}
		}
		
		void JumpStatement()
		{
			if (unit.NextToken.Type != TokenType.Keyword) {
				throw new ParserException("\nError: JumpStatement expected at (" + unit.NextToken.Line + "/" + unit.NextToken.Col + ")", unit.NextToken.Line, unit.NextToken.Col);
			} else {
				switch ((KeyWordEnum)unit.NextToken.Value) {
					case KeyWordEnum.Break:
						BreakStatement();
						break;
					case KeyWordEnum.Continue:
						ContinueStatement();
						break;
					case KeyWordEnum.Goto:
						GotoStatement();
						break;
					case KeyWordEnum.Return:
						ReturnSatement();
						break;
					case KeyWordEnum.Throw:
						ThrowStatement();
						break;
				}
			}
		}
		
		void BreakStatement()
		{
			unit.Match();
			unit.Match(Token.SemicolonToken);
		}
		
		void ContinueStatement()
		{
			unit.Match();
			unit.Match(Token.SemicolonToken);
		}
		
		void GotoStatement()
		{
			unit.Match();
			if (unit.NextToken.Type == TokenType.Identifier) {
				unit.Match();
			} else if (unit.NextToken.Equals(Token.CaseToken)) {
				unit.Match();
				(new Expr(unit)).Expression();
			} else if (unit.NextToken.Equals(Token.DefaultToken)) {
				unit.Match();
			}
			unit.Match(Token.SemicolonToken);
		}
		
		void ReturnSatement()
		{
			unit.Match();
			if (!unit.NextToken.Equals(Token.SemicolonToken)) {
				(new Expr(unit)).Expression();
			}
			unit.Match(Token.SemicolonToken);
		}
		
		void ThrowStatement()
		{
			unit.Match();
			if (!unit.NextToken.Equals(Token.SemicolonToken)) {
				(new Expr(unit)).Expression();
			}
			unit.Match(Token.SemicolonToken);
		}
		
		void TryStatement()
		{
			unit.Match(Token.TryToken);
			(new Block(unit)).Parse();
			while (unit.NextToken.Equals(Token.CatchToken)) { //catch ( class-type identifieropt ) block 
				unit.Match();
				if (!unit.NextToken.Equals(Token.OpenBraceToken)) {
					(new Block(unit)).Parse();
					break;
				}
				unit.Match(Token.OpenBraceToken);
				ReturnType type = unit.Type(false);
				Variable v = null;
				if (!unit.NextToken.Equals(Token.CloseBraceToken)) {
					v = new Variable((string)unit.NextToken.Value, new DefaultRegion(unit.NextToken.Line, unit.NextToken.Col));
					v.ReturnType = type;
					unit.AddToLookUpTable(v);
					unit.Match(TokenType.Identifier);
				}
				unit.Match(Token.CloseBraceToken);
				(new Block(unit)).Parse();
				if (v != null) {
					v.Region.EndLine = unit.NextToken.Line;
					v.Region.EndColumn = unit.NextToken.Col;
				}
			}
			if (unit.NextToken.Equals(Token.FinallyToken)) { //finally block 
				unit.Match();
				(new Block(unit)).Parse();
			}
		}
		
		void CheckedStatement()
		{
			unit.Match(Token.CheckedToken);
			(new Block(unit)).Parse();
		}
		
		void UncheckedStatement()
		{
			unit.Match(Token.UncheckedToken);
			(new Block(unit)).Parse();
		}
		
		void LockStatement() //lock ( expression ) embedded-statement 
		{
			unit.Match(Token.LockToken);
			unit.Match(Token.OpenBraceToken);
			(new Expr(unit)).Expression();
			unit.Match(Token.CloseBraceToken);
			EmbeddedStatement();
		}
		
		void FixedStatement() //fixed ( pointer-type fixed-pointer-declarators ) embedded-statement 
		{
			ArrayList list = new ArrayList();
			unit.Match(Token.FixedToken);
			unit.Match(Token.OpenBraceToken);
			ReturnType type = unit.Type(true);
			Variable v = new Variable((string)unit.NextToken.Value, new DefaultRegion(unit.NextToken.Line, unit.NextToken.Col));
			v.ReturnType = type;
			unit.AddToLookUpTable(v);
			list.Add(v);
			unit.Match(TokenType.Identifier);// fixed-pointer-declarator == identifier = fixed-pointer-initializer 
			unit.Match(Token.AssignToken);
			if (unit.NextToken.Equals(Token.BitwiseAndToken)) {
				unit.Match();
			}
			(new Expr(unit)).Expression();
			while (unit.NextToken.Equals(Token.CommaToken)) {
				v = new Variable((string)unit.NextToken.Value, new DefaultRegion(unit.NextToken.Line, unit.NextToken.Col));
				v.ReturnType = type;
				unit.AddToLookUpTable(v);
				list.Add(v);
				unit.Match(TokenType.Identifier);
				unit.Match(Token.AssignToken);
				if(unit.NextToken.Equals(Token.BitwiseAndToken))
					unit.Match();
				(new Expr(unit)).Expression();
			}
			unit.Match(Token.CloseBraceToken);
			EmbeddedStatement();
			foreach (Variable w in list) {
				w.Region.EndLine = unit.NextToken.Line;
				w.Region.EndColumn = unit.NextToken.Col;
			}
		}
		void UnsafeStatement()
		{
			unit.Match(Token.UnsafeToken);
			(new Block(unit)).Parse();
		}
		
		/*
		void ResourceAcquisition()
		{
			if (unit.NextToken.Type == TokenType.Keyword) {
				if (unit.Parser.Lexer.keywords.IsType((EKeyWord)unit.NextToken.Value)) {
					if (unit.Parser.LookUpToken(1).Equals(Token.PeriodToken)) {
						(new Expr(unit.Parser)).Expression();
					}
					LocalVariableDeclaration();
				} else if (unit.NextToken.Equals(Token.VoidToken)) {
					LocalVariableDeclaration();
				} else {
					(new Expr(unit.Parser)).Expression();
				}
			} else if (unit.NextToken.Type == TokenType.Identifier) {
				if (unit.Parser.LookUpToken(1).Equals(Token.OpenSquareBraceToken)) {
					Token t = unit.Parser.LookUpToken(2);
					if (t.Equals(Token.CommaToken) || t.Equals(Token.CloseSquareBraceToken)) {
						LocalVariableDeclaration();
					} else {
						(new Expr(unit.Parser)).Expression();
					}
				} else if (unit.Parser.LookUpToken(1).Type == TokenType.Identifier) {
					LocalVariableDeclaration();
				} else {
					(new Expr(unit.Parser)).Expression();
				}
			} else {
				(new Expr(unit.Parser)).Expression();
			}
		}
		
		void UsingStatement() //using ( resource-acquisition ) embedded-statement 
		{
			unit.Match(Token.UsingToken);
			unit.Match(Token.OpenBraceToken);
			ResourceAcquisition();//local-variable-declaration || expression 
			unit.Match(Token.CloseBraceToken);
			EmbeddedStatement();
		}
		*/
		SortedList LocalConstantDeclaration()
		{
			unit.Match(Token.ConstToken);
			ReturnType type = unit.Type(true);
			SortedList l = ConstantDeclarators();
			foreach (Variable v in l.Values) {
				v.ReturnType = type;
				unit.AddToLookUpTable(v);
			}
			unit.Match(Token.SemicolonToken);
			return l;
		}
		
		SortedList ConstantDeclarators()
		{
			SortedList l = new SortedList();
			Variable v = ConstantDeclarator();
			l[v.Name] = v;
			while (unit.NextToken.Equals(Token.CommaToken)) {
				unit.Match(Token.CommaToken);
				v = ConstantDeclarator();
				l[v.Name] = v;
			}
			return l;
		}
		
		Variable ConstantDeclarator()
		{
			Variable v = new Variable((string)unit.NextToken.Value, new DefaultRegion(unit.NextToken.Line, unit.NextToken.Col));
			unit.Match(TokenType.Identifier);
			unit.Match(Token.AssignToken);
			(new Expr(unit)).Expression();
			return v;
		}
		
		public SortedList LocalVariableDeclaration() //not used in Statement()
		{
	//		Console.Write(" \"LocalVariableDeclaration\"");
			ReturnType type = unit.Type(true);
			SortedList l = VariableDeclarators();
			foreach (Variable v in l.Values) {
				v.ReturnType = type;
				unit.AddToLookUpTable(v);
			}
	//		Console.Write(" \"\\LocalVariableDeclaration\"");
			return l;
		}
		
		public SortedList VariableDeclarators()
		{
			SortedList l = new SortedList();
			Variable v = VariableDeclarator();
			l[v.Name] = v;
			while (unit.NextToken.Equals(Token.CommaToken)) {
				unit.Match(Token.CommaToken);
				v = VariableDeclarator();
				l[v.Name] = v;
			}
			return l;
		}
		
		Variable VariableDeclarator()
		{
			Variable v = new Variable((string)unit.NextToken.Value, new DefaultRegion(unit.NextToken.Line, unit.NextToken.Col));
			unit.Match(TokenType.Identifier);
			if (unit.NextToken.Equals(Token.AssignToken)) {
				unit.Match(Token.AssignToken);
				if (unit.NextToken.Equals(Token.OpenCurlyBraceToken)) {
					ArrayInitializer();
				} else {
					(new Expr(unit)).Expression();
				}
			}
			return v;
		}
		
		public void ArrayInitializer()//Also in Class
		{
	//		Console.Write("\"ArrayInitializer\"");
			unit.Match(Token.OpenCurlyBraceToken);
			if (!unit.NextToken.Equals(Token.CloseCurlyBraceToken)) {
				if (unit.NextToken.Equals(Token.OpenCurlyBraceToken)) {
					ArrayInitializer();
				} else {
					(new Expr(unit)).Expression();
				}
				while (unit.NextToken.Equals(Token.CommaToken)) {
					unit.Match(Token.CommaToken);
					if (unit.NextToken.Equals(Token.OpenCurlyBraceToken)) {
						ArrayInitializer();
					} else if (unit.NextToken.Equals(Token.CloseCurlyBraceToken)) {
						break;
					} else {
						(new Expr(unit)).Expression();
					}
				}
			}
			unit.Match(Token.CloseCurlyBraceToken);
	//		Console.Write("\"\\ArrayInitializer\"");
		}
		
		SortedList LabeledStatement()
		{
			unit.Match(TokenType.Identifier);
			unit.Match(Token.ColonToken);
			return Statement();
		}
		
		public void StatementExpressionList()
		{
			StatementExpression();
			while (unit.NextToken.Equals(Token.CommaToken)) {
				unit.Match(Token.CommaToken);
				StatementExpression();
			}
		}
		
		void StatementExpression()//invocation-expression || object-creation-expression || assignment || 
			                      //post-increment-expression || post-decrement-expression || pre-increment-expression ||
		                          //pre-decrement-expression 
		{
			/*
		//		Console.Write(" \"StatementExpression\" ");
			if (unit.NextToken.Equals(Token.NewToken)) { //new type ( argument-listopt ) 
				Expr e = new Expr(unit.Parser);
				unit.Match();
				unit.Parser.Type(true);
				unit.Match(Token.OpenBraceToken);
				if (unit.NextToken != Token.CloseBraceToken) {
					unit.Parser.ArgumentList(e);
				}
				unit.Match(Token.CloseBraceToken);
				if (e.AssignmentOperator()) {
					e.Expression();
				}
			} else if ((new Expr(unit.Parser)).IsFirstOfPrimary(unit.NextToken)){
				Expr e = new Expr(unit.Parser);
				(e).Primary();
				//Copied to Statement():
				if (unit.NextToken.Equals(Token.OpenBraceToken)){//invocation-expression = primary-expression ( argument-listopt ) 
					unit.Match();
					unit.Parser.ArgumentList(e);
					unit.Match(Token.CloseBraceToken);
				} else if (unit.NextToken.Equals(Token.IncrementToken)) { //post-increment-expression
					unit.Match();
				} else if (unit.NextToken.Equals(Token.DecrementToken)) { //post-decrement-expression
					unit.Match();
				}
				if (e.AssignmentOperator()) {
					e.Expression();
				}
			} else if (unit.NextToken.Equals(Token.IncrementToken)){//pre-increment-expression
				Expr e = new Expr(unit.Parser);
				unit.Match();
				(e).Unary();
				if (e.AssignmentOperator()) {
					e.Expression();
				}
			} else if (unit.NextToken.Equals(Token.DecrementToken)){//pre-decrement-expression
				Expr e = new Expr(unit.Parser);
				unit.Match();
				(e).Unary();
				if (e.AssignmentOperator()) {
					e.Expression();
				}
			} else { //unary-expression assignment-operator expression
				(new Expr(unit.Parser)).Assignment();
			}
	//		Console.Write(" \"\\StatementExpression\" ");
			*/
			Expr e = new Expr(unit);
			e.Unary();
			if (e.AssignmentOperator()) {
//				Console.WriteLine("Assignment at " + unit.NextToken.Line + "/" + unit.NextToken.Col);
				e.Expression();
			}
		}
		
		public void EmbeddedStatement()
		{
	//		Console.Write(" \"EmbeddedStatement\" ");
			if (unit.NextToken.Equals(Token.OpenCurlyBraceToken)) {
				Block b = new Block(unit);
				b.Parse();
			} else if (unit.NextToken.Equals(Token.SemicolonToken)) {//empty-statement
				unit.Match(Token.SemicolonToken);
			} else if (unit.NextToken.Equals(Token.IfToken)) {
				IfStatement();
			} else if (unit.NextToken.Equals(Token.SwitchToken)) {
				SwitchStatement();
			} else if (unit.NextToken.Equals(Token.WhileToken)) {
				WhileStatement();
			} else if (unit.NextToken.Equals(Token.DoToken)) {
				DoWhileStatement();
			} else if (unit.NextToken.Equals(Token.ForToken)) {
				(new ForStatement(unit)).Parse();
			} else if (unit.NextToken.Equals(Token.ForeachToken)) {
				ForeachStatement();
			} else if (IsJumpKeyword(unit.NextToken)) {
				JumpStatement();
			} else if (unit.NextToken.Equals(Token.TryToken)) {
				TryStatement();
			} else if (unit.NextToken.Equals(Token.CheckedToken)) {
				CheckedStatement();
			} else if (unit.NextToken.Equals(Token.UncheckedToken)) {
				UncheckedStatement();
			} else if (unit.NextToken.Equals(Token.LockToken)) {
				LockStatement();
			} else if (unit.NextToken.Equals(Token.UsingToken)) {
				(new UsingStmt(unit)).Parse();
			} else if (unit.NextToken.Equals(Token.FixedToken)) {
				FixedStatement();
			} else if (unit.NextToken.Equals(Token.UnsafeToken)) {
				UnsafeStatement();
			} else {
				StatementExpression();
				unit.Match(Token.SemicolonToken);
			}
	//		Console.Write(" \"\\EmbeddedStatement\" ");
		}
		
		bool IsEmbeddedStatementNotExpr()
		{
			return (unit.NextToken.Equals(Token.OpenCurlyBraceToken) || 
			    unit.NextToken.Equals(Token.SemicolonToken) ||
				unit.NextToken.Equals(Token.IfToken) ||
				unit.NextToken.Equals(Token.SwitchToken) ||
				unit.NextToken.Equals(Token.WhileToken) ||
				unit.NextToken.Equals(Token.DoToken) ||
				unit.NextToken.Equals(Token.ForToken) ||
				unit.NextToken.Equals(Token.TryToken) ||
				unit.NextToken.Equals(Token.CheckedToken) ||
				unit.NextToken.Equals(Token.UncheckedToken) ||
				unit.NextToken.Equals(Token.LockToken) ||
				unit.NextToken.Equals(Token.UsingToken) ||
				unit.NextToken.Equals(Token.ForeachToken) ||
				unit.NextToken.Equals(Token.FixedToken) ||
				unit.NextToken.Equals(Token.UnsafeToken) ||
				IsJumpKeyword(unit.NextToken));
		}
		
		public SortedList StatementList(Token end)
		{
			SortedList all = new SortedList();
			do{
				SortedList l = Statement();
				if(l != null){
					foreach(string name in l.Keys){
						all[name] = l[name];
					}
				}
			} while (!unit.NextToken.Equals(end) && !(unit.NextToken.Type == TokenType.EOF));
			if (!unit.NextToken.Equals(end)) {
				throw new ParserException("\nEnd of File Reached, >" + end.Type + " , " + end.Value + "< expected", unit.NextToken.Line, unit.NextToken.Col);
			}
			return all;
		}
		
		public SortedList Statement(){
	
//			Console.Write("\"Statement\" ");
//			Console.WriteLine(" NextToken: " + unit.NextToken.Value);
			if (IsEmbeddedStatementNotExpr()) {
				EmbeddedStatement();
				return null;
			}
			if (unit.NextToken.Equals(Token.ConstToken)) {
				return LocalConstantDeclaration();
			}
			//Copied from ForInitializer(): LocalVariableDeclaration || StatementExpression
			if (unit.NextToken.Type == TokenType.Keyword) {
				if (unit.Lexer.keywords.IsType((KeyWordEnum)unit.NextToken.Value)) {
					if (unit.LookUpToken(1).Equals(Token.PeriodToken)) {
						StatementExpression();
						unit.Match(Token.SemicolonToken);
						return null;
					}
					SortedList l = LocalVariableDeclaration();
					unit.Match(Token.SemicolonToken);
					return l;
				}
				if (unit.NextToken.Equals(Token.VoidToken)) {
					SortedList l = LocalVariableDeclaration();
					unit.Match(Token.SemicolonToken);
					return l;
				}
				StatementExpression();
				unit.Match(Token.SemicolonToken);
				return null;
			}
			if (unit.NextToken.Type == TokenType.Identifier) {
				if (unit.LookUpToken(1).Equals(Token.ColonToken)) {
					return LabeledStatement();
				}
				if (unit.LookUpToken(1).Equals(Token.OpenSquareBraceToken)) {
					Token t = unit.LookUpToken(2);
					if (t.Equals(Token.CommaToken) || t.Equals(Token.CloseSquareBraceToken)) {
						SortedList l = LocalVariableDeclaration();
						unit.Match(Token.SemicolonToken);
						return l;
					}
					StatementExpression();
					unit.Match(Token.SemicolonToken);
					return null;
				}
				if (unit.LookUpToken(1).Type == TokenType.Identifier) {
					SortedList l = LocalVariableDeclaration();
					unit.Match(Token.SemicolonToken);
					return l;
				}
				if (unit.LookUpToken(1).Equals(Token.PeriodToken)) {
					string a = unit.NamespaceOrTypeName();
					if (unit.NextToken.Type == TokenType.Identifier) {//LocalVariableDeclaration
						ReturnType type = new ReturnType(a, null, 0);
						SortedList l = VariableDeclarators();
						foreach (Variable v in l.Values) {
							v.ReturnType = type;
							unit.AddToLookUpTable(v);
						}
						unit.Match(Token.SemicolonToken);
						return l;
					}
					if (unit.NextToken.Equals(Token.OpenSquareBraceToken) &&
					   (unit.LookUpToken(1).Equals(Token.CommaToken) ||
					    unit.LookUpToken(1).Equals(Token.CloseSquareBraceToken))){//LocalVariableDeclaration (ArrayType)
						
						ReturnType type = unit.Type(true);
					    if (type.ArrayCount == 0) {
					    	throw new ParserException("Error: ArryType Expected ", unit.NextToken.Col, unit.NextToken.Line);
					    }
						SortedList var = VariableDeclarators();
						foreach (Variable v in var.Values) {
							v.ReturnType = type;
							unit.AddToLookUpTable(v);
						}
						unit.Match(Token.SemicolonToken);
						return var;
					}
					if (unit.NextToken.Equals(Token.MulToken)) { //LocalVariableDeclaration (PointerType)
						int pointer = 0;
						ArrayList l = new ArrayList();
						
						while (unit.NextToken.Equals(Token.MulToken)) {
							++pointer;
							unit.Match();
						}
						
						ReturnType type = new ReturnType(a, null, pointer);
						SortedList var = VariableDeclarators();
						foreach (Variable v in var.Values) {
							v.ReturnType = type;
							unit.AddToLookUpTable(v);
						}
						unit.Match(Token.SemicolonToken);
						return var;
					}
					//StatementExpression
					Expr e = new Expr(unit);
					e.PrimaryRest();
					//Copied from StatementExpression:
					if (unit.NextToken.Equals(Token.OpenBraceToken)) { //invocation-expression = primary-expression ( argument-listopt ) 
						unit.Match();
						e.ArgumentList();
						unit.Match(Token.CloseBraceToken);
					} else if (unit.NextToken.Equals(Token.IncrementToken)) { //post-increment-expression
						unit.Match();
					} else if (unit.NextToken.Equals(Token.DecrementToken)) { //post-decrement-expression
						unit.Match();
					}
					if (e.AssignmentOperator()) {
						e.Expression();
					}
					return null;
				}
				StatementExpression();
				unit.Match(Token.SemicolonToken);
				return null;
			}
			StatementExpression();
			unit.Match(Token.SemicolonToken);
	//		Console.Write("\"\\Statement\" ");
			return null;
		}
		
	}
}
