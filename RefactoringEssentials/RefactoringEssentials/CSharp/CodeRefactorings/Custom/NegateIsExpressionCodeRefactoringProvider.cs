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
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = "Negate 'is' expression")]
    public class NegateIsExpressionCodeRefactoringProvider : SpecializedCodeRefactoringProvider<BinaryExpressionSyntax>
    {
        protected override IEnumerable<CodeAction> GetActions(Document document, SemanticModel semanticModel, SyntaxNode root, TextSpan span, BinaryExpressionSyntax node, CancellationToken cancellationToken)
        {
            if (!node.IsKind(SyntaxKind.IsExpression) || !node.OperatorToken.Span.Contains(span))
                return Enumerable.Empty<CodeAction>();

            var pExpr = node.Parent as ParenthesizedExpressionSyntax;
            if (pExpr != null)
            {
                var uOp = pExpr.Parent as PrefixUnaryExpressionSyntax;
                if (uOp != null && uOp.IsKind(SyntaxKind.LogicalNotExpression))
                {

                    return new[] {
                        CodeActionFactory.Create(
                            span,
                            DiagnosticSeverity.Info,
                            string.Format (GettextCatalog.GetString ("Negate '{0}'"), uOp),
                            t2 => {
                                var newRoot = root.ReplaceNode((SyntaxNode)
                                    (SyntaxNode)uOp,
                                    node.WithAdditionalAnnotations(Formatter.Annotation)
                                );
                                return Task.FromResult(document.WithSyntaxRoot(newRoot));
                            }
                        )
                    };
                }
            }

            return new[] {
                CodeActionFactory.Create(
                    span,
                    DiagnosticSeverity.Info,
                    string.Format ("Negate '{0}'", node),
                    t2 => {
                        var newRoot = root.ReplaceNode((SyntaxNode)
                            (SyntaxNode)node,
                            SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, SyntaxFactory.ParenthesizedExpression(node)).WithAdditionalAnnotations(Formatter.Annotation)
                        );
                        return Task.FromResult(document.WithSyntaxRoot(newRoot));
                    }
                )
            };
        }
    }
}