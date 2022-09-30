using Microsoft.CodeAnalysis.CSharp.Syntax;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal static class SyntaxNodeExtensions
    {
        public static TNode WithAnnotations<TNode>(this TNode node, params SyntaxAnnotation[] annotations) where TNode : CSharpSyntaxNode
        {
            return (TNode)node.Green.SetAnnotations(annotations).CreateRed();
        }

        public static bool IsAnonymousFunction(this SyntaxNode syntax)
        {
            SyntaxKind syntaxKind = syntax.Kind();
            if (syntaxKind - 8641 <= (SyntaxKind)2)
            {
                return true;
            }
            return false;
        }

        public static bool IsQuery(this SyntaxNode syntax)
        {
            switch (syntax.Kind())
            {
                case SyntaxKind.QueryExpression:
                case SyntaxKind.FromClause:
                case SyntaxKind.LetClause:
                case SyntaxKind.JoinClause:
                case SyntaxKind.JoinIntoClause:
                case SyntaxKind.WhereClause:
                case SyntaxKind.OrderByClause:
                case SyntaxKind.SelectClause:
                case SyntaxKind.GroupClause:
                case SyntaxKind.QueryContinuation:
                    return true;
                default:
                    return false;
            }
        }

        internal static bool CanHaveAssociatedLocalBinder(this SyntaxNode syntax)
        {
            switch (syntax.Kind())
            {
                case SyntaxKind.ArgumentList:
                case SyntaxKind.AnonymousMethodExpression:
                case SyntaxKind.SimpleLambdaExpression:
                case SyntaxKind.ParenthesizedLambdaExpression:
                case SyntaxKind.EqualsValueClause:
                case SyntaxKind.SwitchSection:
                case SyntaxKind.CatchClause:
                case SyntaxKind.CatchFilterClause:
                case SyntaxKind.Attribute:
                case SyntaxKind.ConstructorDeclaration:
                case SyntaxKind.BaseConstructorInitializer:
                case SyntaxKind.ThisConstructorInitializer:
                case SyntaxKind.ArrowExpressionClause:
                case SyntaxKind.SwitchExpression:
                case SyntaxKind.SwitchExpressionArm:
                case SyntaxKind.PrimaryConstructorBaseType:
                    return true;
                case SyntaxKind.RecordDeclaration:
                    return ((RecordDeclarationSyntax)syntax).ParameterList != null;
                case SyntaxKind.RecordStructDeclaration:
                    return false;
                default:
                    if (!(syntax is StatementSyntax))
                    {
                        return (syntax as ExpressionSyntax).IsValidScopeDesignator();
                    }
                    return true;
            }
        }

        internal static bool IsValidScopeDesignator(this ExpressionSyntax? expression)
        {
            CSharpSyntaxNode cSharpSyntaxNode = expression?.Parent;
            switch (cSharpSyntaxNode?.Kind())
            {
                case SyntaxKind.SimpleLambdaExpression:
                case SyntaxKind.ParenthesizedLambdaExpression:
                    return ((LambdaExpressionSyntax)cSharpSyntaxNode).Body == expression;
                case SyntaxKind.SwitchStatement:
                    return ((SwitchStatementSyntax)cSharpSyntaxNode).Expression == expression;
                case SyntaxKind.ForStatement:
                    {
                        ForStatementSyntax forStatementSyntax = (ForStatementSyntax)cSharpSyntaxNode;
                        if (forStatementSyntax.Condition != expression)
                        {
                            return forStatementSyntax.Incrementors.FirstOrDefault() == expression;
                        }
                        return true;
                    }
                case SyntaxKind.ForEachStatement:
                case SyntaxKind.ForEachVariableStatement:
                    return ((CommonForEachStatementSyntax)cSharpSyntaxNode).Expression == expression;
                default:
                    return false;
            }
        }

        internal static bool IsLegalCSharp73SpanStackAllocPosition(this SyntaxNode node)
        {
            if (node.Parent.IsKind(SyntaxKind.CastExpression))
            {
                node = node.Parent;
            }
            while (node.Parent.IsKind(SyntaxKind.ConditionalExpression))
            {
                node = node.Parent;
            }
            SyntaxNode parent = node.Parent;
            if (parent == null)
            {
                return false;
            }
            switch (parent.Kind())
            {
                case SyntaxKind.EqualsValueClause:
                    {
                        SyntaxNode parent2 = parent.Parent;
                        if (parent2.IsKind(SyntaxKind.VariableDeclarator))
                        {
                            return parent2.Parent.IsKind(SyntaxKind.VariableDeclaration);
                        }
                        return false;
                    }
                case SyntaxKind.SimpleAssignmentExpression:
                    return parent.Parent.IsKind(SyntaxKind.ExpressionStatement);
                default:
                    return false;
            }
        }

        internal static CSharpSyntaxNode AnonymousFunctionBody(this SyntaxNode lambda)
        {
            return ((AnonymousFunctionExpressionSyntax)lambda).Body;
        }

        internal static SyntaxToken ExtractAnonymousTypeMemberName(this ExpressionSyntax input)
        {
            while (true)
            {
                switch (input.Kind())
                {
                    case SyntaxKind.IdentifierName:
                        return ((IdentifierNameSyntax)input).Identifier;
                    case SyntaxKind.SimpleMemberAccessExpression:
                        input = ((MemberAccessExpressionSyntax)input).Name;
                        break;
                    case SyntaxKind.ConditionalAccessExpression:
                        input = ((ConditionalAccessExpressionSyntax)input).WhenNotNull;
                        if (input.Kind() == SyntaxKind.MemberBindingExpression)
                        {
                            return ((MemberBindingExpressionSyntax)input).Name.Identifier;
                        }
                        break;
                    default:
                        return default(SyntaxToken);
                }
            }
        }

        internal static RefKind GetRefKind(this TypeSyntax syntax)
        {
            syntax.SkipRef(out var refKind);
            return refKind;
        }

        internal static TypeSyntax SkipRef(this TypeSyntax syntax)
        {
            if (syntax.Kind() == SyntaxKind.RefType)
            {
                syntax = ((RefTypeSyntax)syntax).Type;
            }
            return syntax;
        }

        internal static TypeSyntax SkipRef(this TypeSyntax syntax, out RefKind refKind)
        {
            refKind = RefKind.None;
            if (syntax.Kind() == SyntaxKind.RefType)
            {
                RefTypeSyntax refTypeSyntax = (RefTypeSyntax)syntax;
                refKind = ((refTypeSyntax.ReadOnlyKeyword.Kind() != SyntaxKind.ReadOnlyKeyword) ? RefKind.Ref : RefKind.In);
                syntax = refTypeSyntax.Type;
            }
            return syntax;
        }

        internal static ExpressionSyntax? CheckAndUnwrapRefExpression(this ExpressionSyntax? syntax, BindingDiagnosticBag diagnostics, out RefKind refKind)
        {
            refKind = RefKind.None;
            if (syntax != null && syntax!.Kind() == SyntaxKind.RefExpression)
            {
                refKind = RefKind.Ref;
                syntax = ((RefExpressionSyntax)syntax).Expression;
                syntax.CheckDeconstructionCompatibleArgument(diagnostics);
            }
            return syntax;
        }

        internal static void CheckDeconstructionCompatibleArgument(this ExpressionSyntax expression, BindingDiagnosticBag diagnostics)
        {
            if (IsDeconstructionCompatibleArgument(expression))
            {
                diagnostics.Add(ErrorCode.ERR_VarInvocationLvalueReserved, expression.GetLocation());
            }
        }

        private static bool IsDeconstructionCompatibleArgument(ExpressionSyntax expression)
        {
            if (expression.Kind() == SyntaxKind.InvocationExpression)
            {
                ExpressionSyntax expression2 = ((InvocationExpressionSyntax)expression).Expression;
                if (expression2.Kind() == SyntaxKind.IdentifierName)
                {
                    return ((IdentifierNameSyntax)expression2).IsVar;
                }
                return false;
            }
            return false;
        }
    }
}
