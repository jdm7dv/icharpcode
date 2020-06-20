// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;

namespace SharpDevelop.Internal.Parser {
	
	public class Expr
	{
		
		CompilationUnit unit;
		
		public Expr(CompilationUnit unit)
		{
			this.unit = unit;
		}
		
		public CompilationUnit Unit {
			get {
				return unit;
			}
		}
		
		public void Assignment()
		{
			Unary();
			if(!AssignmentOperator()) {
				throw new ParserException("\n Error: AssignmentOperator Expected at (" + unit.NextToken.Line + "/" + unit.NextToken.Col + ")", unit.NextToken.Line, unit.NextToken.Col);
			}
			Expression();
		}
		
		public bool AssignmentOperator()
		{
			if (unit.NextToken.Equals(Token.AssignToken)){
				unit.Match();
				return true;
			}
			else if (unit.NextToken.Equals(Token.PlusEqualToken)){
				unit.Match();
				return true;
			}
			else if (unit.NextToken.Equals(Token.MinusEqualToken)){
				unit.Match();
				return true;
			}
			else if (unit.NextToken.Equals(Token.MulEqualToken)){
				unit.Match();
				return true;
			}
			else if (unit.NextToken.Equals(Token.DivEqualToken)){
				unit.Match();
				return true;
			}
			else if (unit.NextToken.Equals(Token.ModEqualToken)){
				unit.Match();
				return true;
			}
			else if (unit.NextToken.Equals(Token.AndEqualToken)){
				unit.Match();
				return true;
			}
			else if (unit.NextToken.Equals(Token.OrEqualToken)){
				unit.Match();
				return true;
			}
			else if (unit.NextToken.Equals(Token.XorEqualToken)){
				unit.Match();
				return true;
			}
			else if(unit.NextToken.Equals(Token.ShiftLeftEqualToken)){
				unit.Match();
				return true;
			}
			else if(unit.NextToken.Equals(Token.ShiftRightEqualToken)){
				unit.Match();
				return true;
			}
			else{
				return false;
			}
			
		}
		
		void Conditional()
		{
	//		Console.Write(" \"Conditional\" ");
			ConditionalOr();
			ConditionalRest();
	//		Console.Write(" \"\\Conditional\" ");
		}
		
		void ConditionalRest()
		{
	//		Console.Write(" \"ConditionalRest\" ");
			if(unit.NextToken.Equals(Token.QuestionToken)){
				unit.Match();
				Expression();
				unit.Match(Token.ColonToken);
				Expression();
				ConditionalRest();
			}
	//		Console.Write(" \"\\ConditionalRest\" ");
		}
		
		void ConditionalOr()
		{
	//		Console.Write(" \"ConditionalOr\" ");
			ConditionalAnd();
			ConditionalOrRest();
	//		Console.Write(" \"\\ConditionalOr\" ");
		}
		
		void ConditionalOrRest()
		{
	//		Console.Write(" \"\\ConditionalOrRest\" ");
			if(unit.NextToken.Equals(Token.OrToken)){
				unit.Match();
				ConditionalAnd();
				ConditionalOrRest();
			}
	//		Console.Write(" \"\\ConditionalOrRest\" ");
		}
		
		void ConditionalAnd()
		{
	//		Console.Write(" \"ConditionalAnd\" ");
			InclusiveOr();
			ConditionalAndRest();
	//		Console.Write(" \"\\ConditionalAnd\" ");
		}
		
		void ConditionalAndRest()
		{
	//		Console.Write(" \"\\ConditionalAndRest\" ");
			if(unit.NextToken.Equals(Token.AndToken)){
				unit.Match();
				InclusiveOr();
				ConditionalAndRest();
			}
	//		Console.Write(" \"\\ConditionalAndRest\" ");
		}
		
		void InclusiveOr()
		{
	//		Console.Write(" \"InclusiveOr\" ");
			ExclusiveOr();
			InclusiveOrRest();
	//		Console.Write(" \"\\InclusiveOr\" ");
		}
		
		void InclusiveOrRest()
		{
	//		Console.Write(" \"\\ExclusiveOrRest\" ");
			if(unit.NextToken.Equals(Token.BitwiseOrToken)){
				unit.Match();
				ExclusiveOr();
				InclusiveOrRest();
			}
	//		Console.Write(" \"\\ExclusiveOrRest\" ");
		}
		
		void ExclusiveOr()
		{
	//		Console.Write(" \"ExclusiveOr\" ");
			And();
			ExclusiveOrRest();
	//		Console.Write(" \"\\ExclusiveOr\" ");
			
		}
		
		void ExclusiveOrRest()
		{
	//		Console.Write(" \"\\ExclusiveOrRest\" ");
			if(unit.NextToken.Equals(Token.XorToken)){
				unit.Match();
				And();
				ExclusiveOrRest();
			}
	//		Console.Write(" \"\\ExclusiveOrRest\" ");
		}
		
		void And()
		{
	//		Console.Write(" \"And\" ");
			Equality();
			AndRest();
	//		Console.Write(" \"\\And\" ");
		}
		
		void AndRest()
		{
	//		Console.Write(" \"\\AndRest\" ");
			if(unit.NextToken.Equals(Token.BitwiseAndToken)){
				unit.Match();
				Equality();
				AndRest();
			}
	//		Console.Write(" \"\\AndRest\" ");
		}
		
		void Equality()
		{
	//		Console.Write(" \"Equality\" ");
			relational();
			EqualityRest();
	//		Console.Write(" \"\\Equality\" ");
		}
		
		void EqualityRest()
		{
	//		Console.Write(" \"\\EqualityRest\" ");
			if(unit.NextToken.Equals(Token.EqualToken)){
				unit.Match();
				relational();
				EqualityRest();
			}
			else if(unit.NextToken.Equals(Token.NotEqualToken)){
				unit.Match();
				relational();
				EqualityRest();
			}
	//		Console.Write(" \"\\EqualityRest\" ");
		}
		
		void relational()
		{
	//		Console.Write(" \"relational\" ");
			Shift();
			relationalRest();
	//		Console.Write(" \"\\relational\" ");
		}
		
		void relationalRest()
		{
	//		Console.Write(" \"relationalRest\" ");
			if(unit.NextToken.Equals(Token.LessToken)){
				unit.Match();
				Shift();
				relationalRest();
			}
			else if(unit.NextToken.Equals(Token.MoreToken)){
				unit.Match();
				Shift();
				relationalRest();
			}
			else if(unit.NextToken.Equals(Token.LessEqualToken)){
				unit.Match();
				Shift();
				relationalRest();
			} else if(unit.NextToken.Equals(Token.MoreEqualToken)){
				unit.Match();
				Shift();
				relationalRest();
			} else if(unit.NextToken.Equals(Token.IsToken)) {
				unit.Match();
				unit.Type(true);//TODO: ReferenceType //goes right
				relationalRest();
			} else if(unit.NextToken.Equals(Token.AsToken)) {
				unit.Match();
				unit.Type(true);//TODO: ReferenceType //goes right
				relationalRest();
			}
	//		Console.Write(" \"\\relationalRest\" ");
		}
		
		void Shift()
		{
	//		Console.Write(" \"Shift\" ");
			Additive();
			ShiftRest();
	//		Console.Write(" \"\\Shift\" ");
		}
		
		void ShiftRest()
		{
	//		Console.Write(" \"ShiftRest\" ");
			if(unit.NextToken.Equals(Token.ShiftLeftToken)){
				unit.Match();
				Additive();
				ShiftRest();
			}
			else if(unit.NextToken.Equals(Token.ShiftRightToken)){
				unit.Match();
				Additive();
				ShiftRest();
			}
	//		Console.Write(" \"\\ShiftRest\" ");
		}
		
		void Additive()
		{
	//		Console.Write(" \"Additive\" ");
			Multiplicative();
			AdditiveRest();
	//		Console.Write(" \"\\Additive\" ");
		}
		
		void AdditiveRest()
		{
	//		Console.Write(" \"AdditiveRest\" ");
			if(unit.NextToken.Equals(Token.PlusToken)){
				unit.Match();
				Multiplicative();
				AdditiveRest();
			}
			else if(unit.NextToken.Equals(Token.MinusToken)){
				unit.Match();
				Multiplicative();
				AdditiveRest();
			}
	//		Console.Write(" \"\\AdditiveRest\" ");
		}
		
		void Multiplicative()
		{
	//		Console.Write(" \"Multiplicative\" ");
			Unary();
			MultiplicativeRest();
	//		Console.Write(" \"\\Multiplicative\" ");
		}
		
		void MultiplicativeRest()
		{
	//		Console.Write(" \"MultiplicativeRest\" ");
			if(unit.NextToken.Equals(Token.MulToken)){
				unit.Match();
				Unary();
				MultiplicativeRest();
			}
			else if(unit.NextToken.Equals(Token.DivToken)){
				unit.Match();
				Unary();
				MultiplicativeRest();
			}
			else if(unit.NextToken.Equals(Token.ModToken)){
				unit.Match();
				Unary();
				MultiplicativeRest();
			}
	//		Console.Write(" \"\\MultiplicativeRest\" ");
		}
		
		public void Unary()
		{
	//		Console.Write(" \"Unary\" ");
			if(unit.NextToken.Equals(Token.PlusToken)){
				unit.Match();
				Unary();
			}
			else if (unit.NextToken.Equals(Token.MinusToken)) {
				unit.Match();
				Unary();
			}
			else if (unit.NextToken.Equals(Token.NotToken)) {
				unit.Match();
				Unary();
			}
			else if (unit.NextToken.Equals(Token.BitwiseComplementToken)) {
				unit.Match();
				Unary();
			}
			else if (unit.NextToken.Equals(Token.MulToken)) {
				unit.Match();
				Unary();
			}
			else if (unit.NextToken.Equals(Token.BitwiseAndToken)) {
				unit.Match();
				Unary();
			}
			else if (unit.NextToken.Equals(Token.IncrementToken)) {
				unit.Match();
				Unary();
			}
			else if (unit.NextToken.Equals(Token.DecrementToken)) {
				unit.Match();
				Unary();
			}
			else if (unit.NextToken.Equals(Token.OpenBraceToken))
				CastOrPrimary();
			else
				Primary();
	//		Console.Write(" \"\\Unary\" ");
		}
		
/*				if(unit.NextToken.Equals(Token.OpenBraceToken) || unit.NextToken.Equals(unit.Increment) ||
				   unit.NextToken.Equals(unit.Decrement)){//(Expression)
	//				Console.WriteLine(" WAEHLE Expression1");
					PrimaryRest();
					ExpressionRest();
					unit.Match(Token.CloseBraceToken);
					PrimaryRest();
				}else if(unit.NextToken.Equals(Token.CloseBraceToken)){//Cast
					unit.Match(Token.CloseBraceToken);
					Unary();
				}else if(unit.NextToken.Equals(Token.MulToken)){//Cast
					while(unit.NextToken.Equals(Token.MulToken))
						unit.Match();
					unit.Match(Token.CloseBraceToken);
					Unary();
				}else if(unit.NextToken.Equals(Token.OpenSquareBraceToken)){
					t = unit.LookUpToken(1);
					if(t.Equals(Token.CommaToken) || t.Equals(Token.CloseSquareBraceToken)){//Cast
	//					Console.WriteLine(" WAEHLE Cast2");
						while(unit.NextToken.Equals(Token.OpenSquareBraceToken)){
							unit.Match(Token.OpenSquareBraceToken);
							while(unit.NextToken.Equals(Token.CommaToken))
								unit.Match(Token.CommaToken);
							unit.Match(Token.CloseSquareBraceToken);
							Unary();
						}
						while(unit.NextToken.Equals(Token.MulToken)){
							unit.Match();
						}
						unit.Match(Token.CloseBraceToken);
					}else{//(Expression)
	//					Console.WriteLine(" WAEHLE Expression3");
						PrimaryRest();
						ExpressionRest();
						unit.Match(Token.CloseBraceToken);
						PrimaryRest();
					}
				}else{ //Expression
					PrimaryRest();
					ExpressionRest();
					unit.Match(Token.CloseBraceToken);
					PrimaryRest();
				}*/
		
		
		void CastOrPrimary()
		{
	//		Console.Write(" \"CastOrPrimary\" ");
			Token t = unit.LookUpToken(1);
	//		Console.WriteLine("\n\nLookUpToken: Type: {0} Value: {1}", t.Type, t.Value);
			if((t.Type == TokenType.Keyword) && unit.Lexer.keywords.IsType((KeyWordEnum)t.Value))
				if(unit.LookUpToken(2).Equals(Token.PeriodToken))
					Primary();
				else
					CastExpression();
			else if(!(t.Type == TokenType.Identifier || t.Equals(Token.VoidToken)))
				Primary();
			else{
				unit.Match(Token.OpenBraceToken);
				unit.NamespaceOrTypeName();
				if(unit.NextToken.Equals(Token.MulToken)){
					if(unit.LookUpToken(1).Equals(Token.MulToken) ||
					   unit.LookUpToken(1).Equals(Token.CloseBraceToken)){//Cast
						while(unit.NextToken.Equals(Token.MulToken))
							unit.Match();
						unit.Match(Token.CloseBraceToken);
						Unary();
					}else{//Expression
						PrimaryRest();
						ExpressionRest();
						unit.Match(Token.CloseBraceToken);
						PrimaryRest();
					}
				}else if(unit.NextToken.Equals(Token.OpenSquareBraceToken)){
					if(unit.LookUpToken(1).Equals(Token.CommaToken) || unit.LookUpToken(1).Equals(Token.CloseSquareBraceToken)){
						while(unit.NextToken.Equals(Token.OpenSquareBraceToken)){
							unit.Match(Token.OpenSquareBraceToken);
							while(unit.NextToken.Equals(Token.CommaToken))
								unit.Match(Token.CommaToken);
							unit.Match(Token.CloseSquareBraceToken);
						}
						while(unit.NextToken.Equals(Token.MulToken))
							unit.Match();
						unit.Match(Token.CloseBraceToken);
						Unary();
					}else{//Expression
						PrimaryRest();
						ExpressionRest();
						unit.Match(Token.CloseBraceToken);
						PrimaryRest();
					}
				}else if(!unit.NextToken.Equals(Token.CloseBraceToken)){//Expression
					PrimaryRest();
					ExpressionRest();
					unit.Match(Token.CloseBraceToken);
					PrimaryRest();
				}else{
					unit.Match(Token.CloseBraceToken);
					if(unit.IsPredefinedType(unit.NextToken) || unit.NextToken.IsLiteral() ||
					   unit.NextToken.Type == TokenType.Identifier || unit.NextToken.Equals(Token.OpenBraceToken) ||
					   unit.NextToken.Equals(Token.ThisToken) ||
					   unit.NextToken.Equals(Token.BaseToken) ||
					   unit.NextToken.Equals(Token.NewToken) ||
					   unit.NextToken.Equals(Token.TypeofToken) ||
					   unit.NextToken.Equals(Token.SizeofToken) ||
					   unit.NextToken.Equals(Token.CheckedToken) ||
					   unit.NextToken.Equals(Token.UncheckedToken) ||
					   unit.NextToken.Equals(Token.NotToken) ||
					   unit.NextToken.Equals(Token.BitwiseComplementToken) ||
					   unit.NextToken.Equals(Token.IncrementToken) ||
					   unit.NextToken.Equals(Token.DecrementToken))
						Unary();
					else 
						PrimaryRest();
				}
			}
	//		Console.Write(" \"\\CastOrPrimary\" ");
		}
		
		public void CastExpression()
		{
	//		Console.Write(" \"CastExpression\" ");
			unit.Match(Token.OpenBraceToken);
			unit.Type(true);
			unit.Match(Token.CloseBraceToken);
			Unary();
	//		Console.Write(" \"\\CastExpression\" ");
		}
		
		public void Primary()
		{
	//		Console.Write(" \"Primary\" ");
			if (unit.NextToken.Equals(Token.OpenBraceToken)) {
				unit.Match(Token.OpenBraceToken);
				Expression();
				unit.Match(Token.CloseBraceToken);
			} else if (unit.IsPredefinedType(unit.NextToken)) {
				unit.Match(TokenType.Keyword);
				unit.Match(Token.PeriodToken);
				unit.Match(TokenType.Identifier);
			} else if (unit.NextToken.Type == TokenType.Identifier) {
				unit.Match(TokenType.Identifier);
			} else if (unit.NextToken.IsLiteral()){
				unit.Match();
			} else if (unit.NextToken.Equals(Token.ThisToken)) {
				unit.Match();
			} else if (unit.NextToken.Equals(Token.BaseToken)) {
				unit.Match();
				BaseAccess();
			} else if (unit.NextToken.Equals(Token.NewToken)) {
				unit.Match();
				unit.Type(false);// Todo: Delegate Creation //goes right
				if(unit.NextToken.Equals(Token.OpenBraceToken)){//Object Creation
					unit.Match(Token.OpenBraceToken);
					if(!unit.NextToken.Equals(Token.CloseBraceToken))
						ArgumentList();
					unit.Match(Token.CloseBraceToken);
				} else if (unit.NextToken.Equals(Token.OpenSquareBraceToken)){
					ArrayCreation();
				} else {
					throw new ParserException("\nError: '(' or '[' Expected at (" + unit.NextToken.Line + "/" + unit.NextToken.Col + ")", unit.NextToken.Line, unit.NextToken.Col);
				}
			} else if (unit.NextToken.Equals(Token.TypeofToken)) {
				unit.Match();
				unit.Match(Token.OpenBraceToken);
				unit.Type(true);
				unit.Match(Token.CloseBraceToken);
			} else if (unit.NextToken.Equals(Token.SizeofToken)) {
				unit.Match();
				unit.Match(Token.OpenBraceToken);
				unit.Type(true);
				unit.Match(Token.CloseBraceToken);
			} else if (unit.NextToken.Equals(Token.CheckedToken)) {
				unit.Match();
				unit.Match(Token.OpenBraceToken);
				Expression();
				unit.Match(Token.CloseBraceToken);
			} else if (unit.NextToken.Equals(Token.UncheckedToken)) {
				unit.Match();
				unit.Match(Token.OpenBraceToken);
				Expression();
				unit.Match(Token.CloseBraceToken);
			}
			else {
				throw new ParserException("\n Error: FirstOf(Primary) Expected at (" + unit.NextToken.Line + "/" + unit.NextToken.Col + ")\nwas " + unit.NextToken.Value, unit.NextToken.Line, unit.NextToken.Col);
			}
			PrimaryRest();
	//		Console.Write(" \"\\Primary\" ");
		}
		
		public void PrimaryRest(){
	//		Console.Write(" \"PrimaryRest\" ");
			if(unit.NextToken.Equals(Token.PeriodToken)) {
				unit.Match(Token.PeriodToken);
				unit.Match(TokenType.Identifier);
				PrimaryRest();
			} else if (unit.NextToken.Equals(Token.OpenBraceToken)) {
				unit.Match(Token.OpenBraceToken);
				if(!unit.NextToken.Equals(Token.CloseBraceToken))
					ArgumentList();
				unit.Match(Token.CloseBraceToken);
				PrimaryRest();
			} else if (unit.NextToken.Equals(Token.OpenSquareBraceToken)) {
				unit.Match(Token.OpenSquareBraceToken);
				ExpressionList();
				unit.Match(Token.CloseSquareBraceToken);
				PrimaryRest();
			} else if (unit.NextToken.Equals(Token.IncrementToken)) {
				unit.Match(Token.IncrementToken);
				PrimaryRest();
			} else if (unit.NextToken.Equals(Token.DecrementToken)) {
				unit.Match(Token.DecrementToken);
				PrimaryRest();
			} else if (unit.NextToken.Equals(Token.DereferenceToken)) {
				unit.Match();
				unit.Match(TokenType.Identifier);
				PrimaryRest();
			}
	//		Console.Write(" \"\\PrimaryRest\" ");
		}
		
		public bool IsFirstOfPrimary(Token t)
		{
			if(unit.NextToken.Equals(Token.OpenBraceToken) || unit.IsPredefinedType(unit.NextToken) ||
			   unit.NextToken.Type == TokenType.Identifier || unit.NextToken.IsLiteral() || 
			   unit.NextToken.Equals(Token.ThisToken) || 
			   unit.NextToken.Equals(Token.BaseToken) ||
			   unit.NextToken.Equals(Token.NewToken) || 
			   unit.NextToken.Equals(Token.TypeofToken) ||
			   unit.NextToken.Equals(Token.SizeofToken) || 
			   unit.NextToken.Equals(Token.CheckedToken) ||
			   unit.NextToken.Equals(Token.UncheckedToken))
			   	return true;
			return false;
		}
		
		void ArrayCreation()
		{			//non-array-type already done
					unit.Match(Token.OpenSquareBraceToken);
					if(!unit.NextToken.Equals(Token.CommaToken) && !unit.NextToken.Equals(Token.CloseSquareBraceToken) && 
					   !unit.NextToken.Equals(Token.OpenSquareBraceToken)){//[ expression-list ] rank-specifiersopt
						ExpressionList();
						unit.Match(Token.CloseSquareBraceToken);
						while(unit.NextToken.Equals(Token.OpenSquareBraceToken)){
							unit.Match(Token.OpenSquareBraceToken);
							while(unit.NextToken.Equals(Token.CommaToken))
								unit.Match(Token.CommaToken);
							unit.Match(Token.CloseSquareBraceToken);
						}
					}else{//rank-specifiers
						while(unit.NextToken.Equals(Token.CommaToken))
							unit.Match(Token.CommaToken);
						while(unit.NextToken.Equals(Token.OpenSquareBraceToken)){
							unit.Match(Token.OpenSquareBraceToken);
							while(unit.NextToken.Equals(Token.CommaToken))
								unit.Match(Token.CommaToken);
							unit.Match(Token.CloseSquareBraceToken);
						}
						unit.Match(Token.CloseSquareBraceToken);
					}
					if(unit.NextToken.Equals(Token.OpenCurlyBraceToken)){//{ variable-initializer-listopt } || { variable-initializer-list , } 
						unit.Match(Token.OpenCurlyBraceToken);
						if(!unit.NextToken.Equals(Token.CloseCurlyBraceToken)){//variable-initializer-list
							VariableInitializer();
							while(unit.NextToken.Equals(Token.CommaToken)){
								unit.Match(Token.CommaToken);
								if(unit.NextToken.Equals(Token.CloseCurlyBraceToken))
									break;
								VariableInitializer();
							}
						}
						unit.Match(Token.CloseCurlyBraceToken);
					}
					if(unit.NextToken.Equals(Token.OpenSquareBraceToken)){// [ dim-separatorsopt ]
						unit.Match(Token.OpenSquareBraceToken);
						while(unit.NextToken.Equals(Token.CommaToken))
							unit.Match(Token.OpenSquareBraceToken);
						unit.Match(Token.CloseSquareBraceToken);
					}
		}//non-array-type [ expression-list ] rank-specifiersopt array-initializeropt ||
		 //non-array-type                     rank-specifiers    array-initializer    [ dim-separatorsopt ] 
		
		void VariableInitializer()// expression || array-initializer || stackalloc-initializer
		{
			if(unit.NextToken.Equals(Token.StackallocToken)){//stackalloc unmanaged-type [ expression ] 
				unit.Match(Token.StackallocToken);
				unit.Type(true);
				unit.Match(Token.OpenSquareBraceToken);
				Expression();
				unit.Match(Token.CloseSquareBraceToken);
			}else if(unit.NextToken.Equals(Token.OpenCurlyBraceToken)){//{ variable-initializer-listopt } || { variable-initializer-list , }
				unit.Match(Token.OpenCurlyBraceToken);
				if(!unit.NextToken.Equals(Token.CloseCurlyBraceToken)){//variable-initializer-list
					VariableInitializer();
					while(unit.NextToken.Equals(Token.CommaToken)){
						unit.Match(Token.CommaToken);
						VariableInitializer();
					}
				}
				unit.Match(Token.CloseCurlyBraceToken);
			}else{
				Expression();
			}
		}
		
		void BaseAccess()
		{
	//		Console.Write(" \"BaseAccess\" ");
			if(unit.NextToken.Equals(Token.PeriodToken)){
				unit.Match(Token.PeriodToken);
				unit.Match(TokenType.Identifier);
			}
			else if(unit.NextToken.Equals(Token.OpenSquareBraceToken)){
				unit.Match(Token.OpenSquareBraceToken);
				ExpressionList();
				unit.Match(Token.CloseSquareBraceToken);
			}
			else{
				throw new ParserException("Error in BaseAccess: . or [ expected at (" + unit.NextToken.Line + "/" + unit.NextToken.Col + ")", unit.NextToken.Line, unit.NextToken.Col);
			}
		}
		
		public void ArgumentList()
		{
	//		Console.Write(" \"ArgumentList\" ");
			Argument();
			while (unit.NextToken.Equals(Token.CommaToken)) {
				unit.Match(Token.CommaToken);
				Argument();
			}
	//		Console.Write(" \"\\ArgumentList\" ");
		}
		
		public void Argument(){
	//		Console.Write(" \"Argument\" ");
			if (unit.NextToken.Equals(Token.RefToken)) {
				unit.Match();
			} else if (unit.NextToken.Equals(Token.OutToken)) {
				unit.Match();
			}
			Expression();
	//		Console.Write(" \"\\Argument\" ");
		}
		
		public void ExpressionList()
		{
			Expression();
			while(unit.NextToken.Equals(Token.CommaToken)){
				unit.Match(Token.CommaToken);
				Expression();
			}
		}
		
		public void Expression(){//conditional-expression | assignment 
//			Console.WriteLine(" \"Expression\" at " + unit.NextToken.Line + "/" + unit.NextToken.Col);
			Unary();
			ExpressionRest();
//	 		Console.WriteLine(" \"\\Expression\" at " + unit.NextToken.Line + "/" + unit.NextToken.Col);
		}
		
		public void ExpressionRest(){
			if(AssignmentOperator()){
				Expression();
			}else{
				MultiplicativeRest();
				AdditiveRest();
				ShiftRest();
				relationalRest();
				EqualityRest();
				AndRest();
				ExclusiveOrRest();
				InclusiveOrRest();
				ConditionalAndRest();
				ConditionalOrRest();
				ConditionalRest();
			}
		}
		
	}
}
