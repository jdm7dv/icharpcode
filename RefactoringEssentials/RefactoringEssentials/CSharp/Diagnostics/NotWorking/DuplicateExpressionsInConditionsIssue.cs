using System.Collections.Generic;
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory6.CSharp.Diagnostics
{
    //    [IssueDescription("Expression has some redundant items",
    //		                   Description = "Expression has some redundant items",
    //		                   Category = IssueCategories.CodeQualityIssues,
    //		                   Severity = Severity.Warning,
    //		                   AnalysisDisableKeyword = "ConditionalTernaryEqualBranch")]
    public class DuplicateExpressionsInConditionsDiagnosticAnalyzer : GatherVisitorCodeIssueProvider
    {
        protected override IGatherVisitor CreateVisitor(BaseSemanticModel context)
        {
            return new GatherVisitor(context);
        }

        static readonly List<BinaryOperatorType> SupportedOperators = new List<BinaryOperatorType>();
        static DuplicateExpressionsInConditionsIssue()
        {
            SupportedOperators.Add(BinaryOperatorType.BitwiseAnd);
            SupportedOperators.Add(BinaryOperatorType.BitwiseOr);
            SupportedOperators.Add(BinaryOperatorType.ConditionalAnd);
            SupportedOperators.Add(BinaryOperatorType.ConditionalOr);
        }

        class GatherVisitor : GatherVisitorBase<DuplicateExpressionsInConditionsIssue>
        {
            public GatherVisitor(BaseSemanticModel ctx)
                : base(ctx)
            {
            }

            public override void VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression)
            {
                var expression = binaryOperatorExpression;
                base.VisitBinaryOperatorExpression(expression);
                if (!SupportedOperators.Contains(expression.Operator))
                    return;
                var parentExpression = expression.Parent as BinaryOperatorExpression;
                if (parentExpression != null && parentExpression.Operator == expression.Operator)
                {
                    //handle only parent sequence
                    return;
                }
                var expressions = GetExpressions(binaryOperatorExpression, expression);
                for (var i = 0; i < expressions.Count - 1; i++)
                {
                    for (var j = i + 1; j < expressions.Count; j++)
                    {
                        var expressionLeft = expressions[i];
                        var expressionRight = expressions[j];
                        if (!expressionLeft.IsMatch(expressionRight))
                            continue;
                        var action = new CodeAction(ctx.TranslateString("Remove redundant expression"),
                                                    script => RemoveRedundantExpression(script, expressionRight),
                                                    expressionRight);

                        AddDiagnosticAnalyzer(
                            new CodeIssue(expressionRight,
                                ctx.TranslateString(string.Format("The expression '{0}' is identical in the left branch", expressionRight)),
                                action
                            )
                            { IssueMarker = IssueMarker.GrayOut }
                        );


                    }
                }
            }

            private static void RemoveRedundantExpression(Script script, AstNode expressionRight)
            {
                var parent = expressionRight.Parent as BinaryOperatorExpression;
                if (parent == null) //should never happen!
                    return;
                script.Replace(parent, parent.Left.Clone());
            }

            private static List<Expression> GetExpressions(BinaryOperatorExpression binaryOperatorExpression,
                                               BinaryOperatorExpression expression)
            {
                var baseExpression = expression;
                var leftExpression = baseExpression.FirstChild as BinaryOperatorExpression;
                var expressions = new List<Expression>();
                while (leftExpression != null && binaryOperatorExpression.Operator == leftExpression.Operator)
                {
                    expressions.Add(baseExpression.Right);
                    baseExpression = leftExpression;
                    leftExpression = leftExpression.Left as BinaryOperatorExpression;
                }
                expressions.Add(baseExpression.Right);
                expressions.Add(baseExpression.Left);
                expressions.Reverse();
                return expressions;
            }
        }
    }
}
