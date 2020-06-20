using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;

namespace RefactoringEssentials.CSharp.CodeRefactorings
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = "Convert 'switch' to 'if'")]
    public class ConvertSwitchToIfCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            var document = context.Document;
            if (document.Project.Solution.Workspace.Kind == WorkspaceKind.MiscellaneousFiles)
                return;
            var span = context.Span;
            if (!span.IsEmpty)
                return;
            var cancellationToken = context.CancellationToken;
            if (cancellationToken.IsCancellationRequested)
                return;
            var model = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            if (model.IsFromGeneratedCode(cancellationToken))
                return;
            var root = await model.SyntaxTree.GetRootAsync(cancellationToken).ConfigureAwait(false);
            var node = root.FindNode(span) as SwitchStatementSyntax;

            if (node == null || node.Sections.Count == 0 || node.Sections.All(l => l.Labels.Any(s => s.Keyword.IsKind(SyntaxKind.DefaultKeyword))))
                return;

            foreach (var section in node.Sections)
            {
                var lastStatement = section.Statements.LastOrDefault();
                //ignore non-trailing breaks
                if (HasNonTrailingBreaks(section, lastStatement as BreakStatementSyntax))
                    return;
            }

            context.RegisterRefactoring(
                CodeActionFactory.Create(
                    span,
                    DiagnosticSeverity.Info,
                    GettextCatalog.GetString("To 'if'"),
                    t2 =>
                    {
                        var ifNodes = new List<IfStatementSyntax>();
                        ElseClauseSyntax defaultElse = null;

                        foreach (var section in node.Sections)
                        {
                            var condition = CollectCondition(node.Expression, section.Labels);
                            var body = SyntaxFactory.Block();
                            var last = section.Statements.LastOrDefault();
                            foreach (var statement in section.Statements)
                            {
                                if (statement.IsEquivalentTo(last) && statement is BreakStatementSyntax)
                                    continue;
                                body = body.WithStatements(body.Statements.Add(statement));
                            }

                            //default => else
                            if (condition == null)
                            {
                                defaultElse = SyntaxFactory.ElseClause(body)
                                    .WithLeadingTrivia(section.GetLeadingTrivia())
                                    .WithTrailingTrivia(section.GetTrailingTrivia());
                                break;
                            }
                            ifNodes.Add(SyntaxFactory.IfStatement(condition, body).WithLeadingTrivia(section.GetLeadingTrivia()));
                        }

                        IfStatementSyntax ifStatement = null;
                        //reverse the list and chain them
                        foreach (IfStatementSyntax ifs in ifNodes.Reverse<IfStatementSyntax>())
                        {
                            if (ifStatement == null)
                            {
                                ifStatement = ifs;
                                if (defaultElse != null)
                                    ifStatement = ifStatement.WithElse(defaultElse);
                            }
                            else
                                ifStatement = ifs.WithElse(SyntaxFactory.ElseClause(ifStatement));
                        }

                        ifStatement = ifStatement.WithLeadingTrivia(node.GetLeadingTrivia().Concat(ifStatement.GetLeadingTrivia())).WithTrailingTrivia(node.GetTrailingTrivia());

                        return Task.FromResult(document.WithSyntaxRoot(root.ReplaceNode((SyntaxNode)node, ifStatement.WithAdditionalAnnotations(Formatter.Annotation))));
                    })
            );
        }

        ExpressionSyntax CollectCondition(ExpressionSyntax expressionSyntax, SyntaxList<SwitchLabelSyntax> labels)
        {
            //default
            if (labels.Count == 0 || labels.OfType<DefaultSwitchLabelSyntax>().Any())
                return null;

            var conditionList =
                labels
                    .OfType<CaseSwitchLabelSyntax>()
                    .Select(l => SyntaxFactory.BinaryExpression(SyntaxKind.EqualsExpression, expressionSyntax, l.Value)).ToList();

            //attempt to add parentheses
            //TODO: port InsertParentheses in-full rather than a make-do (but I didn't think I had the time to do a full-port)
            for (int i = 0; i < conditionList.Count; ++i)
            {
                var cond = conditionList[i];
                if (NeedsParentheses((cond.Right)))
                {
                    conditionList[i] = cond.WithRight(SyntaxFactory.ParenthesizedExpression(cond.Right));
                }
            }

            if (conditionList.Count == 1)
                return conditionList.First();

            //combine case labels
            BinaryExpressionSyntax condition = conditionList[0];
            for (int i = 1; i < conditionList.Count; ++i)
            {
                condition = SyntaxFactory.BinaryExpression(SyntaxKind.LogicalOrExpression, condition, conditionList[i]);
            }
            return condition;
        }

        internal bool HasNonTrailingBreaks(SyntaxNode node, BreakStatementSyntax trailing)
        {
            //if our trailing 'break' is actually return, then /any/ break is non-trailing
            if (node is BreakStatementSyntax && (trailing == null || !node.GetLocation().Equals(trailing.GetLocation())))
                return true;
            return node.DescendantNodes().Any(n => HasNonTrailingBreaks(n, trailing));
        }

        internal bool NeedsParentheses(ExpressionSyntax expr)
        {
            if (expr.IsKind(SyntaxKind.ConditionalExpression) || expr.IsKind(SyntaxKind.EqualsExpression) || expr.IsKind(SyntaxKind.GreaterThanExpression) ||
                expr.IsKind(SyntaxKind.GreaterThanOrEqualExpression)
                || expr.IsKind(SyntaxKind.LessThanExpression) || expr.IsKind(SyntaxKind.LessThanOrEqualExpression) || expr.IsKind(SyntaxKind.LogicalAndExpression) ||
                expr.IsKind(SyntaxKind.LogicalOrExpression) || expr.IsKind(SyntaxKind.NotEqualsExpression))
            {
                return true;
            }

            var bOp = expr as BinaryExpressionSyntax;
            return bOp != null && NeedsParentheses(bOp.Right);
        }
    }
}
