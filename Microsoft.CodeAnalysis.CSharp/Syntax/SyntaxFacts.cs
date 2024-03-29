// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Roslyn.Utilities;

using static Microsoft.CodeAnalysis.CSharp.SyntaxKind;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public static partial class SyntaxFacts
    {
        /// <summary>
        /// Returns true if the node is the alias of an AliasQualifiedNameSyntax
        /// </summary>
        public static bool IsAliasQualifier(SyntaxNode node)
        {
            return node.Parent is AliasQualifiedNameSyntax p && p.Alias == node;
        }

        public static bool IsAttributeName(SyntaxNode node)
        {
            var parent = node.Parent;
            if (parent == null || !IsName(node.Kind()))
            {
                return false;
            }

            switch (parent.Kind())
            {
                case QualifiedName:
                    var qn = (QualifiedNameSyntax)parent;
                    return qn.Right == node && IsAttributeName(parent);

                case AliasQualifiedName:
                    var an = (AliasQualifiedNameSyntax)parent;
                    return an.Name == node && IsAttributeName(parent);
            }

            return node.Parent is AttributeSyntax p && p.Name == node;
        }

        /// <summary>
        /// Returns true if the node is the object of an invocation expression.
        /// </summary>
        public static bool IsInvoked(ExpressionSyntax node)
        {
            node = SyntaxFactory.GetStandaloneExpression(node);
            return node.Parent is InvocationExpressionSyntax inv && inv.Expression == node;
        }

        /// <summary>
        /// Returns true if the node is the object of an element access expression.
        /// </summary>
        public static bool IsIndexed(ExpressionSyntax node)
        {
            node = SyntaxFactory.GetStandaloneExpression(node);
            return node.Parent is ElementAccessExpressionSyntax indexer && indexer.Expression == node;
        }

        public static bool IsNamespaceAliasQualifier(ExpressionSyntax node)
        {
            return node.Parent is AliasQualifiedNameSyntax parent && parent.Alias == node;
        }

        /// <summary>
        /// Returns true if the node is in a tree location that is expected to be a type
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool IsInTypeOnlyContext(ExpressionSyntax node)
        {
            node = SyntaxFactory.GetStandaloneExpression(node);
            var parent = node.Parent;
            if (parent != null)
            {
                switch (parent.Kind())
                {
                    case Attribute:
                        return ((AttributeSyntax)parent).Name == node;

                    case ArrayType:
                        return ((ArrayTypeSyntax)parent).ElementType == node;

                    case PointerType:
                        return ((PointerTypeSyntax)parent).ElementType == node;

                    case FunctionPointerType:
                        // FunctionPointerTypeSyntax has no direct children that are ExpressionSyntaxes
                        throw ExceptionUtilities.Unreachable;

                    case PredefinedType:
                        return true;

                    case NullableType:
                        return ((NullableTypeSyntax)parent).ElementType == node;

                    case TypeArgumentList:
                        // all children of GenericNames are type arguments
                        return true;

                    case CastExpression:
                        return ((CastExpressionSyntax)parent).Type == node;

                    case ObjectCreationExpression:
                        return ((ObjectCreationExpressionSyntax)parent).Type == node;

                    case StackAllocArrayCreationExpression:
                        return ((StackAllocArrayCreationExpressionSyntax)parent).Type == node;

                    case FromClause:
                        return ((FromClauseSyntax)parent).Type == node;

                    case JoinClause:
                        return ((JoinClauseSyntax)parent).Type == node;

                    case VariableDeclaration:
                        return ((VariableDeclarationSyntax)parent).Type == node;

                    case ForEachStatement:
                        return ((ForEachStatementSyntax)parent).Type == node;

                    case CatchDeclaration:
                        return ((CatchDeclarationSyntax)parent).Type == node;

                    case AsExpression:
                    case IsExpression:
                        return ((BinaryExpressionSyntax)parent).Right == node;

                    case TypeOfExpression:
                        return ((TypeOfExpressionSyntax)parent).Type == node;

                    case SizeOfExpression:
                        return ((SizeOfExpressionSyntax)parent).Type == node;

                    case DefaultExpression:
                        return ((DefaultExpressionSyntax)parent).Type == node;

                    case RefValueExpression:
                        return ((RefValueExpressionSyntax)parent).Type == node;

                    case RefType:
                        return ((RefTypeSyntax)parent).Type == node;

                    case Parameter:
                    case FunctionPointerParameter:
                        return ((BaseParameterSyntax)parent).Type == node;

                    case TypeConstraint:
                        return ((TypeConstraintSyntax)parent).Type == node;

                    case MethodDeclaration:
                        return ((MethodDeclarationSyntax)parent).ReturnType == node;

                    case IndexerDeclaration:
                        return ((IndexerDeclarationSyntax)parent).Type == node;

                    case OperatorDeclaration:
                        return ((OperatorDeclarationSyntax)parent).ReturnType == node;

                    case ConversionOperatorDeclaration:
                        return ((ConversionOperatorDeclarationSyntax)parent).Type == node;

                    case PropertyDeclaration:
                        return ((PropertyDeclarationSyntax)parent).Type == node;

                    case DelegateDeclaration:
                        return ((DelegateDeclarationSyntax)parent).ReturnType == node;

                    case EventDeclaration:
                        return ((EventDeclarationSyntax)parent).Type == node;

                    case LocalFunctionStatement:
                        return ((LocalFunctionStatementSyntax)parent).ReturnType == node;

                    case SimpleBaseType:
                        return true;

                    case PrimaryConstructorBaseType:
                        return ((PrimaryConstructorBaseTypeSyntax)parent).Type == node;

                    case CrefParameter:
                        return true;

                    case ConversionOperatorMemberCref:
                        return ((ConversionOperatorMemberCrefSyntax)parent).Type == node;

                    case ExplicitInterfaceSpecifier:
                        // #13.4.1 An explicit member implementation is a method, property, event or indexer
                        // declaration that references a fully qualified interface member name.
                        // A ExplicitInterfaceSpecifier represents the left part (QN) of the member name, so it
                        // should be treated like a QualifiedName.
                        return ((ExplicitInterfaceSpecifierSyntax)parent).Name == node;

                    case DeclarationPattern:
                        return ((DeclarationPatternSyntax)parent).Type == node;

                    case RecursivePattern:
                        return ((RecursivePatternSyntax)parent).Type == node;

                    case TupleElement:
                        return ((TupleElementSyntax)parent).Type == node;

                    case DeclarationExpression:
                        return ((DeclarationExpressionSyntax)parent).Type == node;

                    case IncompleteMember:
                        return ((IncompleteMemberSyntax)parent).Type == node;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if a node is in a tree location that is expected to be either a namespace or type
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool IsInNamespaceOrTypeContext(ExpressionSyntax? node)
        {
            if (node != null)
            {
                node = SyntaxFactory.GetStandaloneExpression(node);
                var parent = node.Parent;
                if (parent != null)
                {
                    return parent.Kind() switch
                    {
                        UsingDirective => ((UsingDirectiveSyntax)parent).Name == node,
                        QualifiedName => ((QualifiedNameSyntax)parent).Left == node,// left of QN is namespace or type.  Note: when you have "a.b.c()", then
                                                                                    // "a.b" is not a qualified name, it is a member access expression.
                                                                                    // Qualified names are only parsed when the parser knows it's a type only
                                                                                    // context.
                        _ => IsInTypeOnlyContext(node),
                    };
                }
            }

            return false;
        }

        /// <summary>
        /// Is the node the name of a named argument of an invocation, object creation expression, 
        /// constructor initializer, or element access, but not an attribute.
        /// </summary>
        public static bool IsNamedArgumentName(SyntaxNode node)
        {
            // An argument name is an IdentifierName inside a NameColon, inside an Argument, inside an ArgumentList, inside an
            // Invocation, ObjectCreation, ObjectInitializer, ElementAccess or Subpattern.

            if (!node.IsKind(IdentifierName))
            {
                return false;
            }

            var parent1 = node.Parent;
            if (parent1 == null || !parent1.IsKind(NameColon))
            {
                return false;
            }

            var parent2 = parent1.Parent;
            if (parent2.IsKind(SyntaxKind.Subpattern))
            {
                return true;
            }

            if (parent2 == null || !(parent2.IsKind(Argument) || parent2.IsKind(AttributeArgument)))
            {
                return false;
            }

            var parent3 = parent2.Parent;
            if (parent3 == null)
            {
                return false;
            }

            if (parent3.IsKind(SyntaxKind.TupleExpression))
            {
                return true;
            }

            if (!(parent3 is BaseArgumentListSyntax || parent3.IsKind(AttributeArgumentList)))
            {
                return false;
            }

            var parent4 = parent3.Parent;
            if (parent4 == null)
            {
                return false;
            }

            return parent4.Kind() switch
            {
                InvocationExpression or TupleExpression or ObjectCreationExpression or ImplicitObjectCreationExpression or ObjectInitializerExpression or ElementAccessExpression or Attribute or BaseConstructorInitializer or ThisConstructorInitializer or PrimaryConstructorBaseType => true,
                _ => false,
            };
        }

        /// <summary>
        /// Is the expression the initializer in a fixed statement?
        /// </summary>
        public static bool IsFixedStatementExpression(SyntaxNode node)
        {
            var current = node.Parent;
            // Dig through parens because dev10 does (even though the spec doesn't say so)
            // Dig through casts because there's a special error code (CS0254) for such casts.
            while (current != null && (current.IsKind(ParenthesizedExpression) || current.IsKind(CastExpression))) current = current.Parent;
            if (current == null || !current.IsKind(EqualsValueClause)) return false;
            current = current.Parent;
            if (current == null || !current.IsKind(VariableDeclarator)) return false;
            current = current.Parent;
            if (current == null || !current.IsKind(VariableDeclaration)) return false;
            current = current.Parent;
            return current != null && current.IsKind(FixedStatement);
        }

        public static string GetText(Accessibility accessibility)
        {
            return accessibility switch
            {
                Accessibility.NotApplicable => string.Empty,
                Accessibility.Private => SyntaxFacts.GetText(PrivateKeyword),
                Accessibility.ProtectedAndInternal => SyntaxFacts.GetText(PrivateKeyword) + " " + SyntaxFacts.GetText(ProtectedKeyword),
                Accessibility.Internal => SyntaxFacts.GetText(InternalKeyword),
                Accessibility.Protected => SyntaxFacts.GetText(ProtectedKeyword),
                Accessibility.ProtectedOrInternal => SyntaxFacts.GetText(ProtectedKeyword) + " " + SyntaxFacts.GetText(InternalKeyword),
                Accessibility.Public => SyntaxFacts.GetText(PublicKeyword),
                _ => throw ExceptionUtilities.UnexpectedValue(accessibility),
            };
        }

        internal static bool IsStatementExpression(SyntaxNode syntax)
        {
            // The grammar gives:
            //
            // expression-statement:
            //     statement-expression
            //
            // statement-expression:
            //     invocation-expression
            //     object-creation-expression
            //     assignment
            //     post-increment-expression
            //     post-decrement-expression
            //     pre-increment-expression
            //     pre-decrement-expression
            //     await-expression

            switch (syntax.Kind())
            {
                case InvocationExpression:
                case ObjectCreationExpression:
                case SimpleAssignmentExpression:
                case AddAssignmentExpression:
                case SubtractAssignmentExpression:
                case MultiplyAssignmentExpression:
                case DivideAssignmentExpression:
                case ModuloAssignmentExpression:
                case AndAssignmentExpression:
                case OrAssignmentExpression:
                case ExclusiveOrAssignmentExpression:
                case LeftShiftAssignmentExpression:
                case RightShiftAssignmentExpression:
                case CoalesceAssignmentExpression:
                case PostIncrementExpression:
                case PostDecrementExpression:
                case PreIncrementExpression:
                case PreDecrementExpression:
                case AwaitExpression:
                    return true;

                case ConditionalAccessExpression:
                    var access = (ConditionalAccessExpressionSyntax)syntax;
                    return IsStatementExpression(access.WhenNotNull);

                // Allow missing IdentifierNames; they will show up in error cases
                // where there is no statement whatsoever.

                case IdentifierName:
                    return syntax.IsMissing;

                default:
                    return false;
            }
        }

        internal static bool IsIdentifierVar(this Syntax.InternalSyntax.SyntaxToken node)
        {
            return node.ContextualKind == SyntaxKind.VarKeyword;
        }

        internal static bool IsIdentifierVarOrPredefinedType(this Syntax.InternalSyntax.SyntaxToken node)
        {
            return node.IsIdentifierVar() || IsPredefinedType(node.Kind);
        }

        internal static bool IsDeclarationExpressionType(SyntaxNode node, [NotNullWhen(true)] out DeclarationExpressionSyntax? parent)
        {
            parent = node.Parent as DeclarationExpressionSyntax;
            return node == parent?.Type;
        }

        /// <summary>
        /// Given an initializer expression infer the name of anonymous property or tuple element.
        /// Returns null if unsuccessful
        /// </summary>
        public static string? TryGetInferredMemberName(this SyntaxNode syntax)
        {
            SyntaxToken nameToken;
            switch (syntax.Kind())
            {
                case SyntaxKind.SingleVariableDesignation:
                    nameToken = ((SingleVariableDesignationSyntax)syntax).Identifier;
                    break;

                case SyntaxKind.DeclarationExpression:
                    var declaration = (DeclarationExpressionSyntax)syntax;
                    var designationKind = declaration.Designation.Kind();
                    if (designationKind == SyntaxKind.ParenthesizedVariableDesignation ||
                        designationKind == SyntaxKind.DiscardDesignation)
                    {
                        return null;
                    }

                    nameToken = ((SingleVariableDesignationSyntax)declaration.Designation).Identifier;
                    break;

                case SyntaxKind.ParenthesizedVariableDesignation:
                case SyntaxKind.DiscardDesignation:
                    return null;

                default:
                    if (syntax is ExpressionSyntax expr)
                    {
                        nameToken = expr.ExtractAnonymousTypeMemberName();
                        break;
                    }
                    return null;
            }

            return nameToken.RawKind != 0 ? nameToken.ValueText : null;
        }

        /// <summary>
        /// Checks whether the element name is reserved.
        ///
        /// For example:
        /// "Item3" is reserved (at certain positions).
        /// "Rest", "ToString" and other members of System.ValueTuple are reserved (in any position).
        /// Names that are not reserved return false.
        /// </summary>
        public static bool IsReservedTupleElementName(string elementName)
        {
            return NamedTypeSymbol.IsTupleElementNameReserved(elementName) != -1;
        }

        internal static bool HasAnyBody(this BaseMethodDeclarationSyntax declaration)
        {
            return (declaration.Body ?? (SyntaxNode?)declaration.ExpressionBody) != null;
        }

        internal static bool IsTopLevelStatement([NotNullWhen(true)] GlobalStatementSyntax? syntax)
        {
            return syntax?.Parent?.IsKind(SyntaxKind.CompilationUnit) == true;
        }

        internal static bool IsSimpleProgramTopLevelStatement(GlobalStatementSyntax? syntax)
        {
            return IsTopLevelStatement(syntax) && syntax.SyntaxTree.Options.Kind == SourceCodeKind.Regular;
        }

        internal static bool HasAwaitOperations(SyntaxNode node)
        {
            // Do not descend into functions
            return node.DescendantNodesAndSelf(child => !IsNestedFunction(child)).Any(
                            node =>
                            {
                                switch (node)
                                {
                                    case AwaitExpressionSyntax _:
                                    case LocalDeclarationStatementSyntax local when local.AwaitKeyword.IsKind(SyntaxKind.AwaitKeyword):
                                    case CommonForEachStatementSyntax @foreach when @foreach.AwaitKeyword.IsKind(SyntaxKind.AwaitKeyword):
                                    case UsingStatementSyntax @using when @using.AwaitKeyword.IsKind(SyntaxKind.AwaitKeyword):
                                        return true;
                                    default:
                                        return false;
                                }
                            });
        }

        internal static bool IsNestedFunction(SyntaxNode child)
        {
            return child.Kind() switch
            {
                SyntaxKind.LocalFunctionStatement or SyntaxKind.AnonymousMethodExpression or SyntaxKind.SimpleLambdaExpression or SyntaxKind.ParenthesizedLambdaExpression => true,
                _ => false,
            };
        }

        internal static bool HasYieldOperations(SyntaxNode? node)
        {
            // Do not descend into functions and expressions
            return node is not null &&
                   node.DescendantNodesAndSelf(child =>
                   {
                       return !IsNestedFunction(child) && node is not ExpressionSyntax;
                   }).Any(n => n is YieldStatementSyntax);
        }

        internal static bool HasReturnWithExpression(SyntaxNode? node)
        {
            // Do not descend into functions and expressions
            return node is not null &&
                   node.DescendantNodesAndSelf(child => !IsNestedFunction(child) && node is not ExpressionSyntax).Any(n => n is ReturnStatementSyntax { Expression: { } });
        }
    }
}
