using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;

namespace RefactoringEssentials.CSharp.CodeRefactorings
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = "Convert '*'/'/' to '<<'/'>>'")]
    public class ConvertMultiplyToShiftCodeRefactoringProvider : SpecializedCodeRefactoringProvider<BinaryExpressionSyntax>
    {
        protected override IEnumerable<CodeAction> GetActions(Document document, SemanticModel semanticModel, SyntaxNode root, TextSpan span, BinaryExpressionSyntax node, CancellationToken cancellationToken)
        {
            if (!node.OperatorToken.Span.Contains(span) || !(node.OperatorToken.IsKind(SyntaxKind.AsteriskToken) || node.OperatorToken.IsKind(SyntaxKind.SlashToken)))
                return Enumerable.Empty<CodeAction>();
            var rightSide = node.Right as LiteralExpressionSyntax;
            if (rightSide == null || !(rightSide.Token.Value is int))
                return Enumerable.Empty<CodeAction>();

            int value = (int)rightSide.Token.Value;
            int log2 = (int)Math.Log(value, 2);
            if (value != 1 << log2)
                return Enumerable.Empty<CodeAction>();

            bool isLeftShift = node.OperatorToken.IsKind(SyntaxKind.AsteriskToken);

            return new[] { CodeActionFactory.Create(
                span,
                DiagnosticSeverity.Info,
                isLeftShift ? GettextCatalog.GetString ("To '<<'") : GettextCatalog.GetString ("To '>>'"),
                t2 => {
                    var newRoot = root.ReplaceNode((SyntaxNode)node, SyntaxFactory.BinaryExpression(isLeftShift ? SyntaxKind.LeftShiftExpression : SyntaxKind.RightShiftExpression, node.Left,
                        SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(log2))).WithAdditionalAnnotations(Formatter.Annotation));
                    return Task.FromResult(document.WithSyntaxRoot(newRoot));
                }
            )
            };
        }
    }
}