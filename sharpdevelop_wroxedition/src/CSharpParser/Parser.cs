// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using ICSharpCode.SharpDevelop.Services;

namespace SharpDevelop.Internal.Parser {
	
	public class Parser : IParser
	{
		
		///////// IParser Interface
		string[] lexerTags;
		public string[] LexerTags {
			set {
				lexerTags = value;
			}
		}
		
		public ICompilationUnitBase Parse(string fileName)
		{
			Lexer lexer = new Lexer(new FileReader(fileName));
			lexer.Tags = lexerTags;
			return Parse(lexer);
		}
		
		public ICompilationUnitBase Parse(string fileName, string fileContent)
		{
			Lexer lexer = new Lexer(new StringReader(fileContent));
			lexer.Tags = lexerTags;
			return Parse(lexer);
		}
		
		public ResolveResult Resolve(IParserService parserService, string expression, int caretLineNumber, int caretColumn, string fileName)
		{
			return new Resolver().Resolve(parserService, expression, caretLineNumber, caretColumn, fileName);
		}
		
		///////// IParser Interface END

		CompilationUnit Parse(Lexer lexer)
		{
			CompilationUnit unit = new CompilationUnit(lexer);
			try {
				unit.Parse();
				if (!unit.NextToken.Type.Equals(TokenType.EOF)){
					throw new ParserException("\n Error: EOF Expected at (" + unit.NextToken.Line + "/" + unit.NextToken.Col + ")", unit.NextToken.Line, unit.NextToken.Col);
				}
			} catch (System.Exception) {
				unit.SetErrorFlag();
			}
			return unit;
		}
	}
}
