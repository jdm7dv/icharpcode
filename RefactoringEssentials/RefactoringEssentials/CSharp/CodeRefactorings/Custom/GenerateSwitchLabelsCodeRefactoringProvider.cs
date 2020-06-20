using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;

namespace RefactoringEssentials.CSharp.CodeRefactorings
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = "Generate switch labels")]
    public class GenerateSwitchLabelsCodeRefactoringProvider : CodeRefactoringProvider
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

            var token = root.FindToken(span.Start);
            if (!token.IsKind(SyntaxKind.SwitchKeyword))
                return;

            var switchStatement = token.Parent as SwitchStatementSyntax;
            if (switchStatement == null)
                return;

            var result = model.GetSymbolInfo(switchStatement.Expression);
            if (result.Symbol == null)
                return;
            var resultType = result.Symbol.GetReturnType();
            if (resultType.TypeKind != TypeKind.Enum)
                return;

            if (switchStatement.Sections.Count == 0)
            {
                SyntaxList<SwitchSectionSyntax> sections = new SyntaxList<SwitchSectionSyntax>();
                foreach (var field in resultType.GetMembers().OfType<IFieldSymbol>())
                {
                    if (!field.IsConst)
                        continue;
                    sections = sections.Add(GetSectionFromSymbol(model, field, span).WithStatements(SyntaxFactory.SingletonList<StatementSyntax>(SyntaxFactory.BreakStatement())));
                }
                sections = sections.Add(SyntaxFactory.SwitchSection()
                    .WithLabels(SyntaxFactory.SingletonList<SwitchLabelSyntax>(SyntaxFactory.DefaultSwitchLabel()))
                    .WithStatements(SyntaxFactory.SingletonList<StatementSyntax>(
                        SyntaxFactory.ThrowStatement(
                        SyntaxFactory.ObjectCreationExpression(
                            SyntaxFactory.IdentifierName("ArgumentOutOfRangeException")).WithArgumentList(SyntaxFactory.ArgumentList())))));
                var newRoot = root.ReplaceNode((SyntaxNode)switchStatement, switchStatement.WithSections(sections).WithAdditionalAnnotations(Formatter.Annotation));
                context.RegisterRefactoring(
                    CodeActionFactory.Create(span, DiagnosticSeverity.Info, GettextCatalog.GetString("Generate switch labels"), document.WithSyntaxRoot(newRoot))
                );
            }
            else
            {
                List<IFieldSymbol> fields = new List<IFieldSymbol>();
                foreach (var field in resultType.GetMembers())
                {
                    var fieldSymbol = field as IFieldSymbol;
                    if (fieldSymbol == null || !fieldSymbol.IsConst)
                        continue;
                    if (!IsHandled(model, switchStatement, fieldSymbol))
                        fields.Add(fieldSymbol);
                }
                if (fields.Count == 0)
                    return;
                var newSections = new SyntaxList<SwitchSectionSyntax>().AddRange(
                    fields.Select(f => GetSectionFromSymbol(model, f, span).WithStatements(SyntaxFactory.SingletonList<StatementSyntax>(SyntaxFactory.BreakStatement()))));

                //default section - if it exists, remove it, add in our new sections, and replace it
                var defaultSection = switchStatement.Sections.FirstOrDefault(s => s.Labels.Any(l => l is DefaultSwitchLabelSyntax));

                if (defaultSection == null)
                    newSections = switchStatement.Sections.AddRange(newSections);
                else
                    newSections = switchStatement.Sections.Remove(defaultSection).AddRange(newSections).Add(defaultSection);

                var newRoot = root.ReplaceNode((SyntaxNode)switchStatement, switchStatement.WithSections(newSections).WithAdditionalAnnotations(Formatter.Annotation));
                context.RegisterRefactoring(CodeActionFactory.Create(span, DiagnosticSeverity.Info, "Create missing switch labels", document.WithSyntaxRoot(newRoot)));
            }
        }

        private SwitchSectionSyntax GetSectionFromSymbol(SemanticModel model, IFieldSymbol field, TextSpan span)
        {
            return SyntaxFactory.SwitchSection().WithLabels(SyntaxFactory.SingletonList<SwitchLabelSyntax>(SyntaxFactory.CaseSwitchLabel(
                SyntaxFactory.IdentifierName(field.ToMinimalDisplayString(model, span.Start))))).WithStatements(SyntaxFactory.SingletonList<StatementSyntax>(SyntaxFactory.BreakStatement()));
        }

        private bool IsHandled(SemanticModel model, SwitchStatementSyntax switchStatement, IFieldSymbol field)
        {
            //if any label in any section has a value equal to the field name, return true
            return switchStatement.Sections.Any(s => s.Labels.Any(l => l is CaseSwitchLabelSyntax && ((CaseSwitchLabelSyntax)l).Value != null && ((MemberAccessExpressionSyntax)((CaseSwitchLabelSyntax)l).Value).Name.Identifier.ValueText.Equals(field.Name)));
        }
    }
}
