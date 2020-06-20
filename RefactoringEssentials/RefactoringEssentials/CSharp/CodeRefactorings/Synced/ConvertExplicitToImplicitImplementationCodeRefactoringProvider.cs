using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;

namespace RefactoringEssentials.CSharp.CodeRefactorings
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = "Convert explicit to implict implementation")]
    public class ConvertExplicitToImplicitImplementationCodeRefactoringProvider : CodeRefactoringProvider
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
            var node = root.FindNode(span);
            while (node != null && !(node is MemberDeclarationSyntax))
                node = node.Parent;
            if (node == null)
                return;

            if (!node.IsKind(SyntaxKind.MethodDeclaration) &&
                !node.IsKind(SyntaxKind.PropertyDeclaration) &&
                !node.IsKind(SyntaxKind.IndexerDeclaration) &&
                !node.IsKind(SyntaxKind.EventDeclaration))
                return;

            var memberDeclaration = node as MemberDeclarationSyntax;
            var explicitSyntax = memberDeclaration.GetExplicitInterfaceSpecifierSyntax();
            if (explicitSyntax == null || !explicitSyntax.Span.Contains(span))
                return;

            var enclosingSymbol = model.GetDeclaredSymbol(memberDeclaration, cancellationToken);
            if (enclosingSymbol == null)
                return;
            var containingType = enclosingSymbol.ContainingType;


            foreach (var member in containingType.GetMembers())
            {
                if (member == enclosingSymbol ||
                    member.Kind != enclosingSymbol.Kind)
                    continue;

                switch (member.Kind)
                {
                    case SymbolKind.Property:
                        var property1 = (IPropertySymbol)enclosingSymbol;
                        var property2 = (IPropertySymbol)member;

                        foreach (var explictProperty in property1.ExplicitInterfaceImplementations)
                        {
                            if (explictProperty.Name == property2.Name)
                            {
                                if (SignatureComparer.HaveSameSignature(property1.Parameters, property2.Parameters))
                                    return;
                            }
                        }
                        break;
                    case SymbolKind.Method:
                        var method1 = (IMethodSymbol)enclosingSymbol;
                        var method2 = (IMethodSymbol)member;
                        foreach (var explictMethod in method1.ExplicitInterfaceImplementations)
                        {
                            if (explictMethod.Name == method2.Name)
                            {
                                if (SignatureComparer.HaveSameSignature(method1.Parameters, method2.Parameters))
                                    return;
                            }
                        }
                        break;
                    case SymbolKind.Event:
                        var evt1 = (IEventSymbol)enclosingSymbol;
                        var evt2 = (IEventSymbol)member;
                        foreach (var explictProperty in evt1.ExplicitInterfaceImplementations)
                        {
                            if (explictProperty.Name == evt2.Name)
                                return;
                        }
                        break;
                }
            }

            context.RegisterRefactoring(
                CodeActionFactory.Create(
                    span,
                    DiagnosticSeverity.Info,
                    GettextCatalog.GetString("To implicit implementation"),
                    t2 =>
                    {
                        var newNode = memberDeclaration;
                        switch (newNode.Kind())
                        {
                            case SyntaxKind.MethodDeclaration:
                                var method = (MethodDeclarationSyntax)memberDeclaration.WithoutLeadingTrivia();
                                newNode = method
                                    .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                                    .WithExplicitInterfaceSpecifier(null);
                                break;
                            case SyntaxKind.PropertyDeclaration:
                                var property = (PropertyDeclarationSyntax)memberDeclaration.WithoutLeadingTrivia();
                                newNode = property
                                    .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                                    .WithExplicitInterfaceSpecifier(null);
                                break;
                            case SyntaxKind.IndexerDeclaration:
                                var indexer = (IndexerDeclarationSyntax)memberDeclaration.WithoutLeadingTrivia();
                                newNode = indexer
                                    .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                                    .WithExplicitInterfaceSpecifier(null);
                                break;
                            case SyntaxKind.EventDeclaration:
                                var evt = (EventDeclarationSyntax)memberDeclaration.WithoutLeadingTrivia();
                                newNode = evt
                                    .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                                    .WithExplicitInterfaceSpecifier(null);
                                break;
                        }
                        var newRoot = root.ReplaceNode((SyntaxNode)
                            memberDeclaration,
                            newNode.WithAdditionalAnnotations(Formatter.Annotation).WithLeadingTrivia(memberDeclaration.GetLeadingTrivia())
                        );
                        return Task.FromResult(document.WithSyntaxRoot(newRoot));
                    }
                )
            );
        }
    }
}