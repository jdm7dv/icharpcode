using System.Collections.Generic;
using System.Text;
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.NRefactory.Semantics;
using System.Linq;

namespace ICSharpCode.NRefactory6.CSharp.Diagnostics
{
    //	[IssueDescription("Methods have duplicate body",
    //                      Description = "One method has the same body as other method",
    //                      Severity = Severity.Hint,
    //                      IssueMarker = IssueMarker.Underline)]
    public class DuplicateBodyMethodDiagnosticAnalyzer : CodeIssueProvider
    {
        #region ICodeIssueProvider implementation
        public override IEnumerable<CodeIssue> GetIssues(BaseSemanticModel context, string subIssue)
        {
            var visitor = new GatherVisitor(context);
            visitor.GetMethods();
            visitor.ComputeConflicts();
            return visitor.GetIssues();
        }
        #endregion

        private class GatherVisitor : GatherVisitorBase<DuplicateBodyMethodIssue>
        {
            public List<MethodDeclaration> DeclaredMethods;

            public GatherVisitor(BaseSemanticModel context)
                : base(context)
            {
                DeclaredMethods = new List<MethodDeclaration>();
            }

            static string GetMethodDescriptor(MethodDeclaration methodDeclaration)
            {
                var sb = new StringBuilder();
                sb.Append(methodDeclaration.ReturnType);
                sb.Append(";");
                foreach (var parameter in methodDeclaration.Parameters)
                {
                    sb.AppendFormat("{0}:{1};", parameter.Name, parameter.Type);
                }
                sb.Append(methodDeclaration.Modifiers);
                return sb.ToString();
            }

            public void GetMethods()
            {
                ctx.RootNode.AcceptVisitor(this);
            }

            internal void ComputeConflicts()
            {
                var dict = new Dictionary<string, List<MethodDeclaration>>();
                foreach (var declaredMethod in DeclaredMethods)
                {
                    var methodDescriptor = GetMethodDescriptor(declaredMethod);
                    List<MethodDeclaration> listMethods;
                    if (!dict.TryGetValue(methodDescriptor, out listMethods))
                    {
                        listMethods = new List<MethodDeclaration>();
                        dict[methodDescriptor] = listMethods;
                    }
                    listMethods.Add(declaredMethod);
                }
                DeclaredMethods.Clear();

                foreach (var list in dict.Values.Where(list => list.Count >= 2))
                {
                    for (var i = 0; i < list.Count - 1; i++)
                    {
                        var firstMethod = list[i];
                        for (var j = i + 1; j < list.Count; j++)
                        {
                            var secondMethod = list[j];
                            if (firstMethod.Body.IsMatch(secondMethod.Body))
                            {
                                AddDiagnosticAnalyzer(new CodeIssue(secondMethod.NameToken,
                                         string.Format("Method '{0}' has the same with '{1}' ", secondMethod.Name,
                                              firstMethod.Name), string.Format("Method '{0}' has the same with '{1}' ", secondMethod.Name,
                                                                 firstMethod.Name),
                                                                 script =>
                                                                 {
                                                                     InvokeMethod(script, firstMethod, secondMethod);
                                                                 }
                                ));
                            }
                        }
                    }
                }
            }

            readonly InsertParenthesesVisitor _insertParenthesesVisitor = new InsertParenthesesVisitor();
            private TypeDeclaration _parentType;

            private void InvokeMethod(Script script, MethodDeclaration firstMethod, MethodDeclaration secondMethod)
            {
                var statement =
                    new ExpressionStatement(new InvocationExpression(new IdentifierExpression(firstMethod.Name),
                                                                     firstMethod.Parameters.Select(
                                                                         declaration =>
                                                                         GetArgumentExpression(declaration).Clone())));
                statement.AcceptVisitor(_insertParenthesesVisitor);
                if (firstMethod.ReturnType.ToString() != "System.Void")
                {
                    var returnStatement = new ReturnStatement(statement.Expression.Clone());

                    script.Replace(secondMethod.Body, new BlockStatement { returnStatement });
                }
                else
                {
                    script.Replace(secondMethod.Body, new BlockStatement { statement });
                }
            }

            static Expression GetArgumentExpression(ParameterDeclaration parameter)
            {
                var identifierExpr = new IdentifierExpression(parameter.Name);
                switch (parameter.ParameterModifier)
                {
                    case ParameterModifier.Out:
                        return new DirectionExpression(FieldDirection.Out, identifierExpr);
                    case ParameterModifier.Ref:
                        return new DirectionExpression(FieldDirection.Ref, identifierExpr);
                }
                return identifierExpr;
            }

            public override void VisitMethodDeclaration(MethodDeclaration declaration)
            {
                var context = ctx;
                var methodDeclaration = declaration;

                var resolved = context.Resolve(methodDeclaration) as MemberResolveResult;
                if (resolved == null)
                    return;
                var isImplementingInterface = resolved.Member.ImplementedInterfaceMembers.Any();

                if (isImplementingInterface)
                    return;
                if (declaration.Body.IsNull)
                    return;
                var parentType = declaration.Parent as TypeDeclaration;
                if (parentType == null)
                    return;
                if (_parentType == null)
                    _parentType = parentType;
                else
                {
                    //if we are here, it means that we switched from one class to another, so it means that we should compute 
                    //the duplicates up-to now, then reset the list of methods
                    if (parentType != _parentType)
                    {
                        ComputeConflicts();
                        DeclaredMethods.Add(declaration);
                        _parentType = parentType;
                        return;
                    }
                }

                DeclaredMethods.Add(declaration);
            }
        }
    }
}
