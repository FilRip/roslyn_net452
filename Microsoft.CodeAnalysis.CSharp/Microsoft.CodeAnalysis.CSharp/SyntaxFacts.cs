using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Symbols;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public static class SyntaxFacts
    {
        private sealed class SyntaxKindEqualityComparer : IEqualityComparer<SyntaxKind>
        {
            public bool Equals(SyntaxKind x, SyntaxKind y)
            {
                return x == y;
            }

            public int GetHashCode(SyntaxKind obj)
            {
                return (int)obj;
            }
        }

        public static IEqualityComparer<SyntaxKind> EqualityComparer { get; } = new SyntaxKindEqualityComparer();


        internal static bool IsHexDigit(char c)
        {
            if ((c < '0' || c > '9') && (c < 'A' || c > 'F'))
            {
                if (c >= 'a')
                {
                    return c <= 'f';
                }
                return false;
            }
            return true;
        }

        internal static bool IsBinaryDigit(char c)
        {
            return c == '0' || c == '1';
        }

        internal static bool IsDecDigit(char c)
        {
            if (c >= '0')
            {
                return c <= '9';
            }
            return false;
        }

        internal static int HexValue(char c)
        {
            if (c < '0' || c > '9')
            {
                return (c & 0xDF) - 65 + 10;
            }
            return c - 48;
        }

        internal static int BinaryValue(char c)
        {
            return c - 48;
        }

        internal static int DecValue(char c)
        {
            return c - 48;
        }

        public static bool IsWhitespace(char ch)
        {
            if (ch != ' ' && ch != '\t' && ch != '\v' && ch != '\f' && ch != '\u00a0' && ch != '\ufeff' && ch != '\u001a')
            {
                if (ch > 'ÿ')
                {
                    return CharUnicodeInfo.GetUnicodeCategory(ch) == UnicodeCategory.SpaceSeparator;
                }
                return false;
            }
            return true;
        }

        public static bool IsNewLine(char ch)
        {
            if (ch != '\r' && ch != '\n' && ch != '\u0085' && ch != '\u2028')
            {
                return ch == '\u2029';
            }
            return true;
        }

        public static bool IsIdentifierStartCharacter(char ch)
        {
            return UnicodeCharacterUtilities.IsIdentifierStartCharacter(ch);
        }

        public static bool IsIdentifierPartCharacter(char ch)
        {
            return UnicodeCharacterUtilities.IsIdentifierPartCharacter(ch);
        }

        public static bool IsValidIdentifier(string name)
        {
            return UnicodeCharacterUtilities.IsValidIdentifier(name);
        }

        internal static bool ContainsDroppedIdentifierCharacters(string? name)
        {
            if (RoslynString.IsNullOrEmpty(name))
            {
                return false;
            }
            if (name![0] == '@')
            {
                return true;
            }
            int length = name!.Length;
            for (int i = 0; i < length; i++)
            {
                if (UnicodeCharacterUtilities.IsFormattingChar(name![i]))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool IsNonAsciiQuotationMark(char ch)
        {
            switch (ch)
            {
                case '‘':
                case '’':
                    return true;
                case '“':
                case '”':
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsAliasQualifier(SyntaxNode node)
        {
            if (node.Parent is Microsoft.CodeAnalysis.CSharp.Syntax.AliasQualifiedNameSyntax aliasQualifiedNameSyntax)
            {
                return aliasQualifiedNameSyntax.Alias == node;
            }
            return false;
        }

        public static bool IsAttributeName(SyntaxNode node)
        {
            SyntaxNode parent = node.Parent;
            if (parent == null || !IsName(node.Kind()))
            {
                return false;
            }
            switch (parent.Kind())
            {
                case SyntaxKind.QualifiedName:
                    if (((Microsoft.CodeAnalysis.CSharp.Syntax.QualifiedNameSyntax)parent).Right != node)
                    {
                        return false;
                    }
                    return IsAttributeName(parent);
                case SyntaxKind.AliasQualifiedName:
                    if (((Microsoft.CodeAnalysis.CSharp.Syntax.AliasQualifiedNameSyntax)parent).Name != node)
                    {
                        return false;
                    }
                    return IsAttributeName(parent);
                default:
                    if (node.Parent is Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax attributeSyntax)
                    {
                        return attributeSyntax.Name == node;
                    }
                    return false;
            }
        }

        public static bool IsInvoked(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax node)
        {
            node = SyntaxFactory.GetStandaloneExpression(node);
            if (node.Parent is Microsoft.CodeAnalysis.CSharp.Syntax.InvocationExpressionSyntax invocationExpressionSyntax)
            {
                return invocationExpressionSyntax.Expression == node;
            }
            return false;
        }

        public static bool IsIndexed(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax node)
        {
            node = SyntaxFactory.GetStandaloneExpression(node);
            if (node.Parent is Microsoft.CodeAnalysis.CSharp.Syntax.ElementAccessExpressionSyntax elementAccessExpressionSyntax)
            {
                return elementAccessExpressionSyntax.Expression == node;
            }
            return false;
        }

        public static bool IsNamespaceAliasQualifier(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax node)
        {
            if (node.Parent is Microsoft.CodeAnalysis.CSharp.Syntax.AliasQualifiedNameSyntax aliasQualifiedNameSyntax)
            {
                return aliasQualifiedNameSyntax.Alias == node;
            }
            return false;
        }

        public static bool IsInTypeOnlyContext(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax node)
        {
            node = SyntaxFactory.GetStandaloneExpression(node);
            CSharpSyntaxNode parent = node.Parent;
            if (parent != null)
            {
                switch (parent.Kind())
                {
                    case SyntaxKind.Attribute:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax)parent).Name == node;
                    case SyntaxKind.ArrayType:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.ArrayTypeSyntax)parent).ElementType == node;
                    case SyntaxKind.PointerType:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.PointerTypeSyntax)parent).ElementType == node;
                    case SyntaxKind.FunctionPointerType:
                        throw ExceptionUtilities.Unreachable;
                    case SyntaxKind.PredefinedType:
                        return true;
                    case SyntaxKind.NullableType:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.NullableTypeSyntax)parent).ElementType == node;
                    case SyntaxKind.TypeArgumentList:
                        return true;
                    case SyntaxKind.CastExpression:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.CastExpressionSyntax)parent).Type == node;
                    case SyntaxKind.ObjectCreationExpression:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.ObjectCreationExpressionSyntax)parent).Type == node;
                    case SyntaxKind.StackAllocArrayCreationExpression:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.StackAllocArrayCreationExpressionSyntax)parent).Type == node;
                    case SyntaxKind.FromClause:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.FromClauseSyntax)parent).Type == node;
                    case SyntaxKind.JoinClause:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.JoinClauseSyntax)parent).Type == node;
                    case SyntaxKind.VariableDeclaration:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax)parent).Type == node;
                    case SyntaxKind.ForEachStatement:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.ForEachStatementSyntax)parent).Type == node;
                    case SyntaxKind.CatchDeclaration:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.CatchDeclarationSyntax)parent).Type == node;
                    case SyntaxKind.IsExpression:
                    case SyntaxKind.AsExpression:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.BinaryExpressionSyntax)parent).Right == node;
                    case SyntaxKind.TypeOfExpression:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.TypeOfExpressionSyntax)parent).Type == node;
                    case SyntaxKind.SizeOfExpression:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.SizeOfExpressionSyntax)parent).Type == node;
                    case SyntaxKind.DefaultExpression:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.DefaultExpressionSyntax)parent).Type == node;
                    case SyntaxKind.RefValueExpression:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.RefValueExpressionSyntax)parent).Type == node;
                    case SyntaxKind.RefType:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeSyntax)parent).Type == node;
                    case SyntaxKind.Parameter:
                    case SyntaxKind.FunctionPointerParameter:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.BaseParameterSyntax)parent).Type == node;
                    case SyntaxKind.TypeConstraint:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.TypeConstraintSyntax)parent).Type == node;
                    case SyntaxKind.MethodDeclaration:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax)parent).ReturnType == node;
                    case SyntaxKind.IndexerDeclaration:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.IndexerDeclarationSyntax)parent).Type == node;
                    case SyntaxKind.OperatorDeclaration:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.OperatorDeclarationSyntax)parent).ReturnType == node;
                    case SyntaxKind.ConversionOperatorDeclaration:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.ConversionOperatorDeclarationSyntax)parent).Type == node;
                    case SyntaxKind.PropertyDeclaration:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.PropertyDeclarationSyntax)parent).Type == node;
                    case SyntaxKind.DelegateDeclaration:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.DelegateDeclarationSyntax)parent).ReturnType == node;
                    case SyntaxKind.EventDeclaration:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.EventDeclarationSyntax)parent).Type == node;
                    case SyntaxKind.LocalFunctionStatement:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax)parent).ReturnType == node;
                    case SyntaxKind.SimpleBaseType:
                        return true;
                    case SyntaxKind.PrimaryConstructorBaseType:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.PrimaryConstructorBaseTypeSyntax)parent).Type == node;
                    case SyntaxKind.CrefParameter:
                        return true;
                    case SyntaxKind.ConversionOperatorMemberCref:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.ConversionOperatorMemberCrefSyntax)parent).Type == node;
                    case SyntaxKind.ExplicitInterfaceSpecifier:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.ExplicitInterfaceSpecifierSyntax)parent).Name == node;
                    case SyntaxKind.DeclarationPattern:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationPatternSyntax)parent).Type == node;
                    case SyntaxKind.RecursivePattern:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax)parent).Type == node;
                    case SyntaxKind.TupleElement:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax)parent).Type == node;
                    case SyntaxKind.DeclarationExpression:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationExpressionSyntax)parent).Type == node;
                    case SyntaxKind.IncompleteMember:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.IncompleteMemberSyntax)parent).Type == node;
                }
            }
            return false;
        }

        public static bool IsInNamespaceOrTypeContext(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax? node)
        {
            if (node != null)
            {
                node = SyntaxFactory.GetStandaloneExpression(node);
                CSharpSyntaxNode parent = node!.Parent;
                if (parent != null)
                {
                    return parent.Kind() switch
                    {
                        SyntaxKind.UsingDirective => ((Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax)parent).Name == node,
                        SyntaxKind.QualifiedName => ((Microsoft.CodeAnalysis.CSharp.Syntax.QualifiedNameSyntax)parent).Left == node,
                        _ => IsInTypeOnlyContext(node),
                    };
                }
            }
            return false;
        }

        public static bool IsNamedArgumentName(SyntaxNode node)
        {
            if (!node.IsKind(SyntaxKind.IdentifierName))
            {
                return false;
            }
            SyntaxNode parent = node.Parent;
            if (parent == null || !parent.IsKind(SyntaxKind.NameColon))
            {
                return false;
            }
            SyntaxNode parent2 = parent.Parent;
            if (parent2.IsKind(SyntaxKind.Subpattern))
            {
                return true;
            }
            if (parent2 == null || (!parent2.IsKind(SyntaxKind.Argument) && !parent2.IsKind(SyntaxKind.AttributeArgument)))
            {
                return false;
            }
            SyntaxNode parent3 = parent2.Parent;
            if (parent3 == null)
            {
                return false;
            }
            if (parent3.IsKind(SyntaxKind.TupleExpression))
            {
                return true;
            }
            if (!(parent3 is Microsoft.CodeAnalysis.CSharp.Syntax.BaseArgumentListSyntax) && !parent3.IsKind(SyntaxKind.AttributeArgumentList))
            {
                return false;
            }
            SyntaxNode parent4 = parent3.Parent;
            if (parent4 == null)
            {
                return false;
            }
            switch (parent4.Kind())
            {
                case SyntaxKind.InvocationExpression:
                case SyntaxKind.ElementAccessExpression:
                case SyntaxKind.ObjectInitializerExpression:
                case SyntaxKind.ObjectCreationExpression:
                case SyntaxKind.ImplicitObjectCreationExpression:
                case SyntaxKind.Attribute:
                case SyntaxKind.BaseConstructorInitializer:
                case SyntaxKind.ThisConstructorInitializer:
                case SyntaxKind.TupleExpression:
                case SyntaxKind.PrimaryConstructorBaseType:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsFixedStatementExpression(SyntaxNode node)
        {
            SyntaxNode parent = node.Parent;
            while (parent != null && (parent.IsKind(SyntaxKind.ParenthesizedExpression) || parent.IsKind(SyntaxKind.CastExpression)))
            {
                parent = parent.Parent;
            }
            if (parent == null || !parent.IsKind(SyntaxKind.EqualsValueClause))
            {
                return false;
            }
            parent = parent.Parent;
            if (parent == null || !parent.IsKind(SyntaxKind.VariableDeclarator))
            {
                return false;
            }
            parent = parent.Parent;
            if (parent == null || !parent.IsKind(SyntaxKind.VariableDeclaration))
            {
                return false;
            }
            return parent.Parent?.IsKind(SyntaxKind.FixedStatement) ?? false;
        }

        public static string GetText(Accessibility accessibility)
        {
            return accessibility switch
            {
                Accessibility.NotApplicable => string.Empty,
                Accessibility.Private => GetText(SyntaxKind.PrivateKeyword),
                Accessibility.ProtectedAndInternal => GetText(SyntaxKind.PrivateKeyword) + " " + GetText(SyntaxKind.ProtectedKeyword),
                Accessibility.Internal => GetText(SyntaxKind.InternalKeyword),
                Accessibility.Protected => GetText(SyntaxKind.ProtectedKeyword),
                Accessibility.ProtectedOrInternal => GetText(SyntaxKind.ProtectedKeyword) + " " + GetText(SyntaxKind.InternalKeyword),
                Accessibility.Public => GetText(SyntaxKind.PublicKeyword),
                _ => throw ExceptionUtilities.UnexpectedValue(accessibility),
            };
        }

        internal static bool IsStatementExpression(SyntaxNode syntax)
        {
            switch (syntax.Kind())
            {
                case SyntaxKind.InvocationExpression:
                case SyntaxKind.ObjectCreationExpression:
                case SyntaxKind.SimpleAssignmentExpression:
                case SyntaxKind.AddAssignmentExpression:
                case SyntaxKind.SubtractAssignmentExpression:
                case SyntaxKind.MultiplyAssignmentExpression:
                case SyntaxKind.DivideAssignmentExpression:
                case SyntaxKind.ModuloAssignmentExpression:
                case SyntaxKind.AndAssignmentExpression:
                case SyntaxKind.ExclusiveOrAssignmentExpression:
                case SyntaxKind.OrAssignmentExpression:
                case SyntaxKind.LeftShiftAssignmentExpression:
                case SyntaxKind.RightShiftAssignmentExpression:
                case SyntaxKind.CoalesceAssignmentExpression:
                case SyntaxKind.PreIncrementExpression:
                case SyntaxKind.PreDecrementExpression:
                case SyntaxKind.PostIncrementExpression:
                case SyntaxKind.PostDecrementExpression:
                case SyntaxKind.AwaitExpression:
                    return true;
                case SyntaxKind.ConditionalAccessExpression:
                    return IsStatementExpression(((Microsoft.CodeAnalysis.CSharp.Syntax.ConditionalAccessExpressionSyntax)syntax).WhenNotNull);
                case SyntaxKind.IdentifierName:
                    return syntax.IsMissing;
                default:
                    return false;
            }
        }

        [Obsolete("IsLambdaBody API is obsolete", true)]
        public static bool IsLambdaBody(SyntaxNode node)
        {
            return LambdaUtilities.IsLambdaBody(node);
        }

        internal static bool IsIdentifierVar(this Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken node)
        {
            return node.ContextualKind == SyntaxKind.VarKeyword;
        }

        internal static bool IsIdentifierVarOrPredefinedType(this Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken node)
        {
            if (!node.IsIdentifierVar())
            {
                return IsPredefinedType(node.Kind);
            }
            return true;
        }

        internal static bool IsDeclarationExpressionType(SyntaxNode node, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationExpressionSyntax? parent)
        {
            parent = node.Parent as Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationExpressionSyntax;
            return node == parent?.Type;
        }

        public static string? TryGetInferredMemberName(this SyntaxNode syntax)
        {
            SyntaxToken syntaxToken;
            switch (syntax.Kind())
            {
                case SyntaxKind.SingleVariableDesignation:
                    syntaxToken = ((Microsoft.CodeAnalysis.CSharp.Syntax.SingleVariableDesignationSyntax)syntax).Identifier;
                    break;
                case SyntaxKind.DeclarationExpression:
                    {
                        Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationExpressionSyntax declarationExpressionSyntax = (Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationExpressionSyntax)syntax;
                        SyntaxKind syntaxKind = declarationExpressionSyntax.Designation.Kind();
                        if (syntaxKind == SyntaxKind.ParenthesizedVariableDesignation || syntaxKind == SyntaxKind.DiscardDesignation)
                        {
                            return null;
                        }
                        syntaxToken = ((Microsoft.CodeAnalysis.CSharp.Syntax.SingleVariableDesignationSyntax)declarationExpressionSyntax.Designation).Identifier;
                        break;
                    }
                case SyntaxKind.ParenthesizedVariableDesignation:
                case SyntaxKind.DiscardDesignation:
                    return null;
                default:
                    if (syntax is Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax input)
                    {
                        syntaxToken = input.ExtractAnonymousTypeMemberName();
                        break;
                    }
                    return null;
            }
            if (syntaxToken.RawKind == 0)
            {
                return null;
            }
            return syntaxToken.ValueText;
        }

        public static bool IsReservedTupleElementName(string elementName)
        {
            return NamedTypeSymbol.IsTupleElementNameReserved(elementName) != -1;
        }

        internal static bool HasAnyBody(this Microsoft.CodeAnalysis.CSharp.Syntax.BaseMethodDeclarationSyntax declaration)
        {
            return (declaration.Body ?? ((object)declaration.ExpressionBody)) != null;
        }

        internal static bool IsTopLevelStatement([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] Microsoft.CodeAnalysis.CSharp.Syntax.GlobalStatementSyntax? syntax)
        {
            if (syntax == null)
            {
                return false;
            }
            return syntax!.Parent?.IsKind(SyntaxKind.CompilationUnit) == true;
        }

        internal static bool IsSimpleProgramTopLevelStatement(Microsoft.CodeAnalysis.CSharp.Syntax.GlobalStatementSyntax? syntax)
        {
            if (IsTopLevelStatement(syntax))
            {
                return syntax!.SyntaxTree.Options.Kind == SourceCodeKind.Regular;
            }
            return false;
        }

        internal static bool HasAwaitOperations(SyntaxNode node)
        {
            // Do not descend into functions
            return node.DescendantNodesAndSelf(child => !IsNestedFunction(child)).Any(
                            node =>
                            {
                                switch (node)
                                {
                                    case Syntax.AwaitExpressionSyntax _:
                                    case Syntax.LocalDeclarationStatementSyntax local when local.AwaitKeyword.IsKind(SyntaxKind.AwaitKeyword):
                                    case Syntax.CommonForEachStatementSyntax @foreach when @foreach.AwaitKeyword.IsKind(SyntaxKind.AwaitKeyword):
                                    case Syntax.UsingStatementSyntax @using when @using.AwaitKeyword.IsKind(SyntaxKind.AwaitKeyword):
                                        return true;
                                    default:
                                        return false;
                                }
                            });
        }

        internal static bool IsNestedFunction(SyntaxNode child)
        {
            SyntaxKind syntaxKind = child.Kind();
            if (syntaxKind - 8641 <= (SyntaxKind)2 || syntaxKind == SyntaxKind.LocalFunctionStatement)
            {
                return true;
            }
            return false;
        }

        internal static bool HasYieldOperations(SyntaxNode? node)
        {
            SyntaxNode node2 = node;
            if (node2 != null)
            {
                return node2.DescendantNodesAndSelf((SyntaxNode child) => !IsNestedFunction(child) && !(node2 is Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax)).Any((SyntaxNode n) => n is Microsoft.CodeAnalysis.CSharp.Syntax.YieldStatementSyntax);
            }
            return false;
        }

        internal static bool HasReturnWithExpression(SyntaxNode? node)
        {
            SyntaxNode node2 = node;
            if (node2 != null)
            {
                return node2.DescendantNodesAndSelf((SyntaxNode child) => !IsNestedFunction(child) && !(node2 is Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax)).Any((SyntaxNode n) => n is Microsoft.CodeAnalysis.CSharp.Syntax.ReturnStatementSyntax returnStatementSyntax && returnStatementSyntax.Expression != null);
            }
            return false;
        }

        public static bool IsKeywordKind(SyntaxKind kind)
        {
            if (!IsReservedKeyword(kind))
            {
                return IsContextualKeyword(kind);
            }
            return true;
        }

        public static IEnumerable<SyntaxKind> GetReservedKeywordKinds()
        {
            for (int i = 8304; i <= 8384; i++)
            {
                yield return (SyntaxKind)i;
            }
        }

        public static IEnumerable<SyntaxKind> GetKeywordKinds()
        {
            foreach (SyntaxKind reservedKeywordKind in GetReservedKeywordKinds())
            {
                yield return reservedKeywordKind;
            }
            foreach (SyntaxKind contextualKeywordKind in GetContextualKeywordKinds())
            {
                yield return contextualKeywordKind;
            }
        }

        public static bool IsReservedKeyword(SyntaxKind kind)
        {
            if ((int)kind >= 8304)
            {
                return (int)kind <= 8384;
            }
            return false;
        }

        public static bool IsAttributeTargetSpecifier(SyntaxKind kind)
        {
            if (kind - 8409 <= SyntaxKind.List)
            {
                return true;
            }
            return false;
        }

        public static bool IsAccessibilityModifier(SyntaxKind kind)
        {
            if (kind - 8343 <= (SyntaxKind)3)
            {
                return true;
            }
            return false;
        }

        public static bool IsPreprocessorKeyword(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                case SyntaxKind.IfKeyword:
                case SyntaxKind.ElseKeyword:
                case SyntaxKind.DefaultKeyword:
                case SyntaxKind.ElifKeyword:
                case SyntaxKind.EndIfKeyword:
                case SyntaxKind.RegionKeyword:
                case SyntaxKind.EndRegionKeyword:
                case SyntaxKind.DefineKeyword:
                case SyntaxKind.UndefKeyword:
                case SyntaxKind.WarningKeyword:
                case SyntaxKind.ErrorKeyword:
                case SyntaxKind.LineKeyword:
                case SyntaxKind.PragmaKeyword:
                case SyntaxKind.HiddenKeyword:
                case SyntaxKind.ChecksumKeyword:
                case SyntaxKind.DisableKeyword:
                case SyntaxKind.RestoreKeyword:
                case SyntaxKind.ReferenceKeyword:
                case SyntaxKind.LoadKeyword:
                case SyntaxKind.NullableKeyword:
                case SyntaxKind.EnableKeyword:
                case SyntaxKind.WarningsKeyword:
                case SyntaxKind.AnnotationsKeyword:
                    return true;
                default:
                    return false;
            }
        }

        internal static bool IsPreprocessorContextualKeyword(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                case SyntaxKind.DefaultKeyword:
                case SyntaxKind.HiddenKeyword:
                case SyntaxKind.ChecksumKeyword:
                case SyntaxKind.DisableKeyword:
                case SyntaxKind.RestoreKeyword:
                case SyntaxKind.EnableKeyword:
                case SyntaxKind.WarningsKeyword:
                case SyntaxKind.AnnotationsKeyword:
                    return false;
                default:
                    return IsPreprocessorKeyword(kind);
            }
        }

        public static IEnumerable<SyntaxKind> GetPreprocessorKeywordKinds()
        {
            yield return SyntaxKind.TrueKeyword;
            yield return SyntaxKind.FalseKeyword;
            yield return SyntaxKind.DefaultKeyword;
            yield return SyntaxKind.HiddenKeyword;
            for (int i = 8467; i <= 8480; i++)
            {
                yield return (SyntaxKind)i;
            }
        }

        public static bool IsPunctuation(SyntaxKind kind)
        {
            if ((int)kind >= 8193)
            {
                return (int)kind <= 8284;
            }
            return false;
        }

        public static bool IsLanguagePunctuation(SyntaxKind kind)
        {
            if (IsPunctuation(kind) && !IsPreprocessorKeyword(kind))
            {
                return !IsDebuggerSpecialPunctuation(kind);
            }
            return false;
        }

        public static bool IsPreprocessorPunctuation(SyntaxKind kind)
        {
            return kind == SyntaxKind.HashToken;
        }

        private static bool IsDebuggerSpecialPunctuation(SyntaxKind kind)
        {
            return kind == SyntaxKind.DollarToken;
        }

        public static IEnumerable<SyntaxKind> GetPunctuationKinds()
        {
            for (int i = 8193; i <= 8283; i++)
            {
                yield return (SyntaxKind)i;
            }
        }

        public static bool IsPunctuationOrKeyword(SyntaxKind kind)
        {
            if ((int)kind >= 8193)
            {
                return (int)kind <= 8496;
            }
            return false;
        }

        internal static bool IsLiteral(SyntaxKind kind)
        {
            if (kind - 8508 <= (SyntaxKind)6)
            {
                return true;
            }
            return false;
        }

        public static bool IsAnyToken(SyntaxKind kind)
        {
            if ((int)kind >= 8193 && (int)kind < 8539)
            {
                return true;
            }
            switch (kind)
            {
                case SyntaxKind.InterpolatedStringStartToken:
                case SyntaxKind.InterpolatedStringEndToken:
                case SyntaxKind.InterpolatedVerbatimStringStartToken:
                case SyntaxKind.LoadKeyword:
                case SyntaxKind.NullableKeyword:
                case SyntaxKind.EnableKeyword:
                case SyntaxKind.UnderscoreToken:
                case SyntaxKind.InterpolatedStringToken:
                case SyntaxKind.InterpolatedStringTextToken:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsTrivia(SyntaxKind kind)
        {
            if (kind - 8539 <= (SyntaxKind)7 || kind == SyntaxKind.ConflictMarkerTrivia)
            {
                return true;
            }
            return IsPreprocessorDirective(kind);
        }

        public static bool IsPreprocessorDirective(SyntaxKind kind)
        {
            if (kind - 8548 <= (SyntaxKind)14 || kind - 8922 <= SyntaxKind.List || kind == SyntaxKind.NullableDirectiveTrivia)
            {
                return true;
            }
            return false;
        }

        public static bool IsName(SyntaxKind kind)
        {
            if (kind - 8616 <= (SyntaxKind)2 || kind == SyntaxKind.AliasQualifiedName)
            {
                return true;
            }
            return false;
        }

        public static bool IsPredefinedType(SyntaxKind kind)
        {
            if (kind - 8304 <= (SyntaxKind)15)
            {
                return true;
            }
            return false;
        }

        public static bool IsTypeSyntax(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PredefinedType:
                case SyntaxKind.ArrayType:
                case SyntaxKind.PointerType:
                case SyntaxKind.NullableType:
                case SyntaxKind.TupleType:
                case SyntaxKind.FunctionPointerType:
                    return true;
                default:
                    return IsName(kind);
            }
        }

        public static bool IsGlobalMemberDeclaration(SyntaxKind kind)
        {
            if (kind == SyntaxKind.GlobalStatement || kind - 8873 <= (SyntaxKind)2 || kind - 8892 <= SyntaxKind.List)
            {
                return true;
            }
            return false;
        }

        public static bool IsTypeDeclaration(SyntaxKind kind)
        {
            if (kind - 8855 <= (SyntaxKind)4 || kind == SyntaxKind.RecordDeclaration || kind == SyntaxKind.RecordStructDeclaration)
            {
                return true;
            }
            return false;
        }

        public static bool IsNamespaceMemberDeclaration(SyntaxKind kind)
        {
            if (!IsTypeDeclaration(kind))
            {
                return kind == SyntaxKind.NamespaceDeclaration;
            }
            return true;
        }

        public static bool IsAnyUnaryExpression(SyntaxKind token)
        {
            if (!IsPrefixUnaryExpression(token))
            {
                return IsPostfixUnaryExpression(token);
            }
            return true;
        }

        public static bool IsPrefixUnaryExpression(SyntaxKind token)
        {
            return GetPrefixUnaryExpression(token) != SyntaxKind.None;
        }

        public static bool IsPrefixUnaryExpressionOperatorToken(SyntaxKind token)
        {
            return GetPrefixUnaryExpression(token) != SyntaxKind.None;
        }

        public static SyntaxKind GetPrefixUnaryExpression(SyntaxKind token)
        {
            return token switch
            {
                SyntaxKind.PlusToken => SyntaxKind.UnaryPlusExpression,
                SyntaxKind.MinusToken => SyntaxKind.UnaryMinusExpression,
                SyntaxKind.TildeToken => SyntaxKind.BitwiseNotExpression,
                SyntaxKind.ExclamationToken => SyntaxKind.LogicalNotExpression,
                SyntaxKind.PlusPlusToken => SyntaxKind.PreIncrementExpression,
                SyntaxKind.MinusMinusToken => SyntaxKind.PreDecrementExpression,
                SyntaxKind.AmpersandToken => SyntaxKind.AddressOfExpression,
                SyntaxKind.AsteriskToken => SyntaxKind.PointerIndirectionExpression,
                SyntaxKind.CaretToken => SyntaxKind.IndexExpression,
                _ => SyntaxKind.None,
            };
        }

        public static bool IsPostfixUnaryExpression(SyntaxKind token)
        {
            return GetPostfixUnaryExpression(token) != SyntaxKind.None;
        }

        public static bool IsPostfixUnaryExpressionToken(SyntaxKind token)
        {
            return GetPostfixUnaryExpression(token) != SyntaxKind.None;
        }

        public static SyntaxKind GetPostfixUnaryExpression(SyntaxKind token)
        {
            return token switch
            {
                SyntaxKind.PlusPlusToken => SyntaxKind.PostIncrementExpression,
                SyntaxKind.MinusMinusToken => SyntaxKind.PostDecrementExpression,
                SyntaxKind.ExclamationToken => SyntaxKind.SuppressNullableWarningExpression,
                _ => SyntaxKind.None,
            };
        }

        internal static bool IsIncrementOrDecrementOperator(SyntaxKind token)
        {
            if (token - 8262 <= SyntaxKind.List)
            {
                return true;
            }
            return false;
        }

        public static bool IsUnaryOperatorDeclarationToken(SyntaxKind token)
        {
            if (!IsPrefixUnaryExpressionOperatorToken(token) && token != SyntaxKind.TrueKeyword)
            {
                return token == SyntaxKind.FalseKeyword;
            }
            return true;
        }

        public static bool IsAnyOverloadableOperator(SyntaxKind kind)
        {
            if (!IsOverloadableBinaryOperator(kind))
            {
                return IsOverloadableUnaryOperator(kind);
            }
            return true;
        }

        public static bool IsOverloadableBinaryOperator(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PercentToken:
                case SyntaxKind.CaretToken:
                case SyntaxKind.AmpersandToken:
                case SyntaxKind.AsteriskToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.PlusToken:
                case SyntaxKind.BarToken:
                case SyntaxKind.LessThanToken:
                case SyntaxKind.GreaterThanToken:
                case SyntaxKind.SlashToken:
                case SyntaxKind.ExclamationEqualsToken:
                case SyntaxKind.EqualsEqualsToken:
                case SyntaxKind.LessThanEqualsToken:
                case SyntaxKind.LessThanLessThanToken:
                case SyntaxKind.GreaterThanEqualsToken:
                case SyntaxKind.GreaterThanGreaterThanToken:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsOverloadableUnaryOperator(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.TildeToken:
                case SyntaxKind.ExclamationToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusMinusToken:
                case SyntaxKind.PlusPlusToken:
                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsPrimaryFunction(SyntaxKind keyword)
        {
            return GetPrimaryFunction(keyword) != SyntaxKind.None;
        }

        public static SyntaxKind GetPrimaryFunction(SyntaxKind keyword)
        {
            return keyword switch
            {
                SyntaxKind.MakeRefKeyword => SyntaxKind.MakeRefExpression,
                SyntaxKind.RefTypeKeyword => SyntaxKind.RefTypeExpression,
                SyntaxKind.RefValueKeyword => SyntaxKind.RefValueExpression,
                SyntaxKind.CheckedKeyword => SyntaxKind.CheckedExpression,
                SyntaxKind.UncheckedKeyword => SyntaxKind.UncheckedExpression,
                SyntaxKind.DefaultKeyword => SyntaxKind.DefaultExpression,
                SyntaxKind.TypeOfKeyword => SyntaxKind.TypeOfExpression,
                SyntaxKind.SizeOfKeyword => SyntaxKind.SizeOfExpression,
                _ => SyntaxKind.None,
            };
        }

        public static bool IsLiteralExpression(SyntaxKind token)
        {
            return GetLiteralExpression(token) != SyntaxKind.None;
        }

        public static SyntaxKind GetLiteralExpression(SyntaxKind token)
        {
            return token switch
            {
                SyntaxKind.StringLiteralToken => SyntaxKind.StringLiteralExpression,
                SyntaxKind.CharacterLiteralToken => SyntaxKind.CharacterLiteralExpression,
                SyntaxKind.NumericLiteralToken => SyntaxKind.NumericLiteralExpression,
                SyntaxKind.NullKeyword => SyntaxKind.NullLiteralExpression,
                SyntaxKind.TrueKeyword => SyntaxKind.TrueLiteralExpression,
                SyntaxKind.FalseKeyword => SyntaxKind.FalseLiteralExpression,
                SyntaxKind.ArgListKeyword => SyntaxKind.ArgListExpression,
                _ => SyntaxKind.None,
            };
        }

        public static bool IsInstanceExpression(SyntaxKind token)
        {
            return GetInstanceExpression(token) != SyntaxKind.None;
        }

        public static SyntaxKind GetInstanceExpression(SyntaxKind token)
        {
            return token switch
            {
                SyntaxKind.ThisKeyword => SyntaxKind.ThisExpression,
                SyntaxKind.BaseKeyword => SyntaxKind.BaseExpression,
                _ => SyntaxKind.None,
            };
        }

        public static bool IsBinaryExpression(SyntaxKind token)
        {
            return GetBinaryExpression(token) != SyntaxKind.None;
        }

        public static bool IsBinaryExpressionOperatorToken(SyntaxKind token)
        {
            return GetBinaryExpression(token) != SyntaxKind.None;
        }

        public static SyntaxKind GetBinaryExpression(SyntaxKind token)
        {
            return token switch
            {
                SyntaxKind.QuestionQuestionToken => SyntaxKind.CoalesceExpression,
                SyntaxKind.IsKeyword => SyntaxKind.IsExpression,
                SyntaxKind.AsKeyword => SyntaxKind.AsExpression,
                SyntaxKind.BarToken => SyntaxKind.BitwiseOrExpression,
                SyntaxKind.CaretToken => SyntaxKind.ExclusiveOrExpression,
                SyntaxKind.AmpersandToken => SyntaxKind.BitwiseAndExpression,
                SyntaxKind.EqualsEqualsToken => SyntaxKind.EqualsExpression,
                SyntaxKind.ExclamationEqualsToken => SyntaxKind.NotEqualsExpression,
                SyntaxKind.LessThanToken => SyntaxKind.LessThanExpression,
                SyntaxKind.LessThanEqualsToken => SyntaxKind.LessThanOrEqualExpression,
                SyntaxKind.GreaterThanToken => SyntaxKind.GreaterThanExpression,
                SyntaxKind.GreaterThanEqualsToken => SyntaxKind.GreaterThanOrEqualExpression,
                SyntaxKind.LessThanLessThanToken => SyntaxKind.LeftShiftExpression,
                SyntaxKind.GreaterThanGreaterThanToken => SyntaxKind.RightShiftExpression,
                SyntaxKind.PlusToken => SyntaxKind.AddExpression,
                SyntaxKind.MinusToken => SyntaxKind.SubtractExpression,
                SyntaxKind.AsteriskToken => SyntaxKind.MultiplyExpression,
                SyntaxKind.SlashToken => SyntaxKind.DivideExpression,
                SyntaxKind.PercentToken => SyntaxKind.ModuloExpression,
                SyntaxKind.AmpersandAmpersandToken => SyntaxKind.LogicalAndExpression,
                SyntaxKind.BarBarToken => SyntaxKind.LogicalOrExpression,
                _ => SyntaxKind.None,
            };
        }

        public static bool IsAssignmentExpression(SyntaxKind kind)
        {
            if (kind - 8714 <= (SyntaxKind)11)
            {
                return true;
            }
            return false;
        }

        public static bool IsAssignmentExpressionOperatorToken(SyntaxKind token)
        {
            if (token == SyntaxKind.EqualsToken || token == SyntaxKind.LessThanLessThanEqualsToken || token - 8275 <= (SyntaxKind)9)
            {
                return true;
            }
            return false;
        }

        public static SyntaxKind GetAssignmentExpression(SyntaxKind token)
        {
            return token switch
            {
                SyntaxKind.BarEqualsToken => SyntaxKind.OrAssignmentExpression,
                SyntaxKind.AmpersandEqualsToken => SyntaxKind.AndAssignmentExpression,
                SyntaxKind.CaretEqualsToken => SyntaxKind.ExclusiveOrAssignmentExpression,
                SyntaxKind.LessThanLessThanEqualsToken => SyntaxKind.LeftShiftAssignmentExpression,
                SyntaxKind.GreaterThanGreaterThanEqualsToken => SyntaxKind.RightShiftAssignmentExpression,
                SyntaxKind.PlusEqualsToken => SyntaxKind.AddAssignmentExpression,
                SyntaxKind.MinusEqualsToken => SyntaxKind.SubtractAssignmentExpression,
                SyntaxKind.AsteriskEqualsToken => SyntaxKind.MultiplyAssignmentExpression,
                SyntaxKind.SlashEqualsToken => SyntaxKind.DivideAssignmentExpression,
                SyntaxKind.PercentEqualsToken => SyntaxKind.ModuloAssignmentExpression,
                SyntaxKind.EqualsToken => SyntaxKind.SimpleAssignmentExpression,
                SyntaxKind.QuestionQuestionEqualsToken => SyntaxKind.CoalesceAssignmentExpression,
                _ => SyntaxKind.None,
            };
        }

        public static SyntaxKind GetCheckStatement(SyntaxKind keyword)
        {
            return keyword switch
            {
                SyntaxKind.CheckedKeyword => SyntaxKind.CheckedStatement,
                SyntaxKind.UncheckedKeyword => SyntaxKind.UncheckedStatement,
                _ => SyntaxKind.None,
            };
        }

        public static SyntaxKind GetAccessorDeclarationKind(SyntaxKind keyword)
        {
            return keyword switch
            {
                SyntaxKind.GetKeyword => SyntaxKind.GetAccessorDeclaration,
                SyntaxKind.SetKeyword => SyntaxKind.SetAccessorDeclaration,
                SyntaxKind.InitKeyword => SyntaxKind.InitAccessorDeclaration,
                SyntaxKind.AddKeyword => SyntaxKind.AddAccessorDeclaration,
                SyntaxKind.RemoveKeyword => SyntaxKind.RemoveAccessorDeclaration,
                _ => SyntaxKind.None,
            };
        }

        public static bool IsAccessorDeclaration(SyntaxKind kind)
        {
            if (kind - 8896 <= (SyntaxKind)3 || kind == SyntaxKind.InitAccessorDeclaration)
            {
                return true;
            }
            return false;
        }

        public static bool IsAccessorDeclarationKeyword(SyntaxKind keyword)
        {
            if (keyword - 8417 <= (SyntaxKind)3 || keyword == SyntaxKind.InitKeyword)
            {
                return true;
            }
            return false;
        }

        public static SyntaxKind GetSwitchLabelKind(SyntaxKind keyword)
        {
            return keyword switch
            {
                SyntaxKind.CaseKeyword => SyntaxKind.CaseSwitchLabel,
                SyntaxKind.DefaultKeyword => SyntaxKind.DefaultSwitchLabel,
                _ => SyntaxKind.None,
            };
        }

        public static SyntaxKind GetBaseTypeDeclarationKind(SyntaxKind kind)
        {
            if (kind != SyntaxKind.EnumKeyword)
            {
                return GetTypeDeclarationKind(kind);
            }
            return SyntaxKind.EnumDeclaration;
        }

        public static SyntaxKind GetTypeDeclarationKind(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.ClassKeyword => SyntaxKind.ClassDeclaration,
                SyntaxKind.StructKeyword => SyntaxKind.StructDeclaration,
                SyntaxKind.InterfaceKeyword => SyntaxKind.InterfaceDeclaration,
                SyntaxKind.RecordKeyword => SyntaxKind.RecordDeclaration,
                _ => SyntaxKind.None,
            };
        }

        public static SyntaxKind GetKeywordKind(string text)
        {
            return text switch
            {
                "bool" => SyntaxKind.BoolKeyword,
                "byte" => SyntaxKind.ByteKeyword,
                "sbyte" => SyntaxKind.SByteKeyword,
                "short" => SyntaxKind.ShortKeyword,
                "ushort" => SyntaxKind.UShortKeyword,
                "int" => SyntaxKind.IntKeyword,
                "uint" => SyntaxKind.UIntKeyword,
                "long" => SyntaxKind.LongKeyword,
                "ulong" => SyntaxKind.ULongKeyword,
                "double" => SyntaxKind.DoubleKeyword,
                "float" => SyntaxKind.FloatKeyword,
                "decimal" => SyntaxKind.DecimalKeyword,
                "string" => SyntaxKind.StringKeyword,
                "char" => SyntaxKind.CharKeyword,
                "void" => SyntaxKind.VoidKeyword,
                "object" => SyntaxKind.ObjectKeyword,
                "typeof" => SyntaxKind.TypeOfKeyword,
                "sizeof" => SyntaxKind.SizeOfKeyword,
                "null" => SyntaxKind.NullKeyword,
                "true" => SyntaxKind.TrueKeyword,
                "false" => SyntaxKind.FalseKeyword,
                "if" => SyntaxKind.IfKeyword,
                "else" => SyntaxKind.ElseKeyword,
                "while" => SyntaxKind.WhileKeyword,
                "for" => SyntaxKind.ForKeyword,
                "foreach" => SyntaxKind.ForEachKeyword,
                "do" => SyntaxKind.DoKeyword,
                "switch" => SyntaxKind.SwitchKeyword,
                "case" => SyntaxKind.CaseKeyword,
                "default" => SyntaxKind.DefaultKeyword,
                "lock" => SyntaxKind.LockKeyword,
                "try" => SyntaxKind.TryKeyword,
                "throw" => SyntaxKind.ThrowKeyword,
                "catch" => SyntaxKind.CatchKeyword,
                "finally" => SyntaxKind.FinallyKeyword,
                "goto" => SyntaxKind.GotoKeyword,
                "break" => SyntaxKind.BreakKeyword,
                "continue" => SyntaxKind.ContinueKeyword,
                "return" => SyntaxKind.ReturnKeyword,
                "public" => SyntaxKind.PublicKeyword,
                "private" => SyntaxKind.PrivateKeyword,
                "internal" => SyntaxKind.InternalKeyword,
                "protected" => SyntaxKind.ProtectedKeyword,
                "static" => SyntaxKind.StaticKeyword,
                "readonly" => SyntaxKind.ReadOnlyKeyword,
                "sealed" => SyntaxKind.SealedKeyword,
                "const" => SyntaxKind.ConstKeyword,
                "fixed" => SyntaxKind.FixedKeyword,
                "stackalloc" => SyntaxKind.StackAllocKeyword,
                "volatile" => SyntaxKind.VolatileKeyword,
                "new" => SyntaxKind.NewKeyword,
                "override" => SyntaxKind.OverrideKeyword,
                "abstract" => SyntaxKind.AbstractKeyword,
                "virtual" => SyntaxKind.VirtualKeyword,
                "event" => SyntaxKind.EventKeyword,
                "extern" => SyntaxKind.ExternKeyword,
                "ref" => SyntaxKind.RefKeyword,
                "out" => SyntaxKind.OutKeyword,
                "in" => SyntaxKind.InKeyword,
                "is" => SyntaxKind.IsKeyword,
                "as" => SyntaxKind.AsKeyword,
                "params" => SyntaxKind.ParamsKeyword,
                "__arglist" => SyntaxKind.ArgListKeyword,
                "__makeref" => SyntaxKind.MakeRefKeyword,
                "__reftype" => SyntaxKind.RefTypeKeyword,
                "__refvalue" => SyntaxKind.RefValueKeyword,
                "this" => SyntaxKind.ThisKeyword,
                "base" => SyntaxKind.BaseKeyword,
                "namespace" => SyntaxKind.NamespaceKeyword,
                "using" => SyntaxKind.UsingKeyword,
                "class" => SyntaxKind.ClassKeyword,
                "struct" => SyntaxKind.StructKeyword,
                "interface" => SyntaxKind.InterfaceKeyword,
                "enum" => SyntaxKind.EnumKeyword,
                "delegate" => SyntaxKind.DelegateKeyword,
                "checked" => SyntaxKind.CheckedKeyword,
                "unchecked" => SyntaxKind.UncheckedKeyword,
                "unsafe" => SyntaxKind.UnsafeKeyword,
                "operator" => SyntaxKind.OperatorKeyword,
                "implicit" => SyntaxKind.ImplicitKeyword,
                "explicit" => SyntaxKind.ExplicitKeyword,
                _ => SyntaxKind.None,
            };
        }

        public static SyntaxKind GetOperatorKind(string operatorMetadataName)
        {
            return operatorMetadataName switch
            {
                "op_Addition" => SyntaxKind.PlusToken,
                "op_BitwiseAnd" => SyntaxKind.AmpersandToken,
                "op_BitwiseOr" => SyntaxKind.BarToken,
                "op_Decrement" => SyntaxKind.MinusMinusToken,
                "op_Division" => SyntaxKind.SlashToken,
                "op_Equality" => SyntaxKind.EqualsEqualsToken,
                "op_ExclusiveOr" => SyntaxKind.CaretToken,
                "op_Explicit" => SyntaxKind.ExplicitKeyword,
                "op_False" => SyntaxKind.FalseKeyword,
                "op_GreaterThan" => SyntaxKind.GreaterThanToken,
                "op_GreaterThanOrEqual" => SyntaxKind.GreaterThanEqualsToken,
                "op_Implicit" => SyntaxKind.ImplicitKeyword,
                "op_Increment" => SyntaxKind.PlusPlusToken,
                "op_Inequality" => SyntaxKind.ExclamationEqualsToken,
                "op_LeftShift" => SyntaxKind.LessThanLessThanToken,
                "op_LessThan" => SyntaxKind.LessThanToken,
                "op_LessThanOrEqual" => SyntaxKind.LessThanEqualsToken,
                "op_LogicalNot" => SyntaxKind.ExclamationToken,
                "op_Modulus" => SyntaxKind.PercentToken,
                "op_Multiply" => SyntaxKind.AsteriskToken,
                "op_OnesComplement" => SyntaxKind.TildeToken,
                "op_RightShift" => SyntaxKind.GreaterThanGreaterThanToken,
                "op_Subtraction" => SyntaxKind.MinusToken,
                "op_True" => SyntaxKind.TrueKeyword,
                "op_UnaryNegation" => SyntaxKind.MinusToken,
                "op_UnaryPlus" => SyntaxKind.PlusToken,
                _ => SyntaxKind.None,
            };
        }

        public static SyntaxKind GetPreprocessorKeywordKind(string text)
        {
            return text switch
            {
                "true" => SyntaxKind.TrueKeyword,
                "false" => SyntaxKind.FalseKeyword,
                "default" => SyntaxKind.DefaultKeyword,
                "if" => SyntaxKind.IfKeyword,
                "else" => SyntaxKind.ElseKeyword,
                "elif" => SyntaxKind.ElifKeyword,
                "endif" => SyntaxKind.EndIfKeyword,
                "region" => SyntaxKind.RegionKeyword,
                "endregion" => SyntaxKind.EndRegionKeyword,
                "define" => SyntaxKind.DefineKeyword,
                "undef" => SyntaxKind.UndefKeyword,
                "warning" => SyntaxKind.WarningKeyword,
                "error" => SyntaxKind.ErrorKeyword,
                "line" => SyntaxKind.LineKeyword,
                "pragma" => SyntaxKind.PragmaKeyword,
                "hidden" => SyntaxKind.HiddenKeyword,
                "checksum" => SyntaxKind.ChecksumKeyword,
                "disable" => SyntaxKind.DisableKeyword,
                "restore" => SyntaxKind.RestoreKeyword,
                "r" => SyntaxKind.ReferenceKeyword,
                "load" => SyntaxKind.LoadKeyword,
                "nullable" => SyntaxKind.NullableKeyword,
                "enable" => SyntaxKind.EnableKeyword,
                "warnings" => SyntaxKind.WarningsKeyword,
                "annotations" => SyntaxKind.AnnotationsKeyword,
                _ => SyntaxKind.None,
            };
        }

        public static IEnumerable<SyntaxKind> GetContextualKeywordKinds()
        {
            for (int i = 8405; i <= 8446; i++)
            {
                yield return (SyntaxKind)i;
            }
        }

        public static bool IsContextualKeyword(SyntaxKind kind)
        {
            if (kind - 8405 <= (SyntaxKind)41 || kind - 8490 <= SyntaxKind.List)
            {
                return true;
            }
            return false;
        }

        public static bool IsQueryContextualKeyword(SyntaxKind kind)
        {
            if (kind - 8421 <= (SyntaxKind)12)
            {
                return true;
            }
            return false;
        }

        public static SyntaxKind GetContextualKeywordKind(string text)
        {
            return text switch
            {
                "yield" => SyntaxKind.YieldKeyword,
                "partial" => SyntaxKind.PartialKeyword,
                "from" => SyntaxKind.FromKeyword,
                "group" => SyntaxKind.GroupKeyword,
                "join" => SyntaxKind.JoinKeyword,
                "into" => SyntaxKind.IntoKeyword,
                "let" => SyntaxKind.LetKeyword,
                "by" => SyntaxKind.ByKeyword,
                "where" => SyntaxKind.WhereKeyword,
                "select" => SyntaxKind.SelectKeyword,
                "get" => SyntaxKind.GetKeyword,
                "set" => SyntaxKind.SetKeyword,
                "add" => SyntaxKind.AddKeyword,
                "remove" => SyntaxKind.RemoveKeyword,
                "orderby" => SyntaxKind.OrderByKeyword,
                "alias" => SyntaxKind.AliasKeyword,
                "on" => SyntaxKind.OnKeyword,
                "equals" => SyntaxKind.EqualsKeyword,
                "ascending" => SyntaxKind.AscendingKeyword,
                "descending" => SyntaxKind.DescendingKeyword,
                "assembly" => SyntaxKind.AssemblyKeyword,
                "module" => SyntaxKind.ModuleKeyword,
                "type" => SyntaxKind.TypeKeyword,
                "field" => SyntaxKind.FieldKeyword,
                "method" => SyntaxKind.MethodKeyword,
                "param" => SyntaxKind.ParamKeyword,
                "property" => SyntaxKind.PropertyKeyword,
                "typevar" => SyntaxKind.TypeVarKeyword,
                "global" => SyntaxKind.GlobalKeyword,
                "async" => SyntaxKind.AsyncKeyword,
                "await" => SyntaxKind.AwaitKeyword,
                "when" => SyntaxKind.WhenKeyword,
                "nameof" => SyntaxKind.NameOfKeyword,
                "_" => SyntaxKind.UnderscoreToken,
                "var" => SyntaxKind.VarKeyword,
                "and" => SyntaxKind.AndKeyword,
                "or" => SyntaxKind.OrKeyword,
                "not" => SyntaxKind.NotKeyword,
                "data" => SyntaxKind.DataKeyword,
                "with" => SyntaxKind.WithKeyword,
                "init" => SyntaxKind.InitKeyword,
                "record" => SyntaxKind.RecordKeyword,
                "managed" => SyntaxKind.ManagedKeyword,
                "unmanaged" => SyntaxKind.UnmanagedKeyword,
                _ => SyntaxKind.None,
            };
        }

        public static string GetText(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.TildeToken => "~",
                SyntaxKind.ExclamationToken => "!",
                SyntaxKind.DollarToken => "$",
                SyntaxKind.PercentToken => "%",
                SyntaxKind.CaretToken => "^",
                SyntaxKind.AmpersandToken => "&",
                SyntaxKind.AsteriskToken => "*",
                SyntaxKind.OpenParenToken => "(",
                SyntaxKind.CloseParenToken => ")",
                SyntaxKind.MinusToken => "-",
                SyntaxKind.PlusToken => "+",
                SyntaxKind.EqualsToken => "=",
                SyntaxKind.OpenBraceToken => "{",
                SyntaxKind.CloseBraceToken => "}",
                SyntaxKind.OpenBracketToken => "[",
                SyntaxKind.CloseBracketToken => "]",
                SyntaxKind.BarToken => "|",
                SyntaxKind.BackslashToken => "\\",
                SyntaxKind.ColonToken => ":",
                SyntaxKind.SemicolonToken => ";",
                SyntaxKind.DoubleQuoteToken => "\"",
                SyntaxKind.SingleQuoteToken => "'",
                SyntaxKind.LessThanToken => "<",
                SyntaxKind.CommaToken => ",",
                SyntaxKind.GreaterThanToken => ">",
                SyntaxKind.DotToken => ".",
                SyntaxKind.QuestionToken => "?",
                SyntaxKind.HashToken => "#",
                SyntaxKind.SlashToken => "/",
                SyntaxKind.SlashGreaterThanToken => "/>",
                SyntaxKind.LessThanSlashToken => "</",
                SyntaxKind.XmlCommentStartToken => "<!--",
                SyntaxKind.XmlCommentEndToken => "-->",
                SyntaxKind.XmlCDataStartToken => "<![CDATA[",
                SyntaxKind.XmlCDataEndToken => "]]>",
                SyntaxKind.XmlProcessingInstructionStartToken => "<?",
                SyntaxKind.XmlProcessingInstructionEndToken => "?>",
                SyntaxKind.BarBarToken => "||",
                SyntaxKind.AmpersandAmpersandToken => "&&",
                SyntaxKind.MinusMinusToken => "--",
                SyntaxKind.PlusPlusToken => "++",
                SyntaxKind.ColonColonToken => "::",
                SyntaxKind.QuestionQuestionToken => "??",
                SyntaxKind.MinusGreaterThanToken => "->",
                SyntaxKind.ExclamationEqualsToken => "!=",
                SyntaxKind.EqualsEqualsToken => "==",
                SyntaxKind.EqualsGreaterThanToken => "=>",
                SyntaxKind.LessThanEqualsToken => "<=",
                SyntaxKind.LessThanLessThanToken => "<<",
                SyntaxKind.LessThanLessThanEqualsToken => "<<=",
                SyntaxKind.GreaterThanEqualsToken => ">=",
                SyntaxKind.GreaterThanGreaterThanToken => ">>",
                SyntaxKind.GreaterThanGreaterThanEqualsToken => ">>=",
                SyntaxKind.SlashEqualsToken => "/=",
                SyntaxKind.AsteriskEqualsToken => "*=",
                SyntaxKind.BarEqualsToken => "|=",
                SyntaxKind.AmpersandEqualsToken => "&=",
                SyntaxKind.PlusEqualsToken => "+=",
                SyntaxKind.MinusEqualsToken => "-=",
                SyntaxKind.CaretEqualsToken => "^=",
                SyntaxKind.PercentEqualsToken => "%=",
                SyntaxKind.QuestionQuestionEqualsToken => "??=",
                SyntaxKind.DotDotToken => "..",
                SyntaxKind.BoolKeyword => "bool",
                SyntaxKind.ByteKeyword => "byte",
                SyntaxKind.SByteKeyword => "sbyte",
                SyntaxKind.ShortKeyword => "short",
                SyntaxKind.UShortKeyword => "ushort",
                SyntaxKind.IntKeyword => "int",
                SyntaxKind.UIntKeyword => "uint",
                SyntaxKind.LongKeyword => "long",
                SyntaxKind.ULongKeyword => "ulong",
                SyntaxKind.DoubleKeyword => "double",
                SyntaxKind.FloatKeyword => "float",
                SyntaxKind.DecimalKeyword => "decimal",
                SyntaxKind.StringKeyword => "string",
                SyntaxKind.CharKeyword => "char",
                SyntaxKind.VoidKeyword => "void",
                SyntaxKind.ObjectKeyword => "object",
                SyntaxKind.TypeOfKeyword => "typeof",
                SyntaxKind.SizeOfKeyword => "sizeof",
                SyntaxKind.NullKeyword => "null",
                SyntaxKind.TrueKeyword => "true",
                SyntaxKind.FalseKeyword => "false",
                SyntaxKind.IfKeyword => "if",
                SyntaxKind.ElseKeyword => "else",
                SyntaxKind.WhileKeyword => "while",
                SyntaxKind.ForKeyword => "for",
                SyntaxKind.ForEachKeyword => "foreach",
                SyntaxKind.DoKeyword => "do",
                SyntaxKind.SwitchKeyword => "switch",
                SyntaxKind.CaseKeyword => "case",
                SyntaxKind.DefaultKeyword => "default",
                SyntaxKind.TryKeyword => "try",
                SyntaxKind.CatchKeyword => "catch",
                SyntaxKind.FinallyKeyword => "finally",
                SyntaxKind.LockKeyword => "lock",
                SyntaxKind.GotoKeyword => "goto",
                SyntaxKind.BreakKeyword => "break",
                SyntaxKind.ContinueKeyword => "continue",
                SyntaxKind.ReturnKeyword => "return",
                SyntaxKind.ThrowKeyword => "throw",
                SyntaxKind.PublicKeyword => "public",
                SyntaxKind.PrivateKeyword => "private",
                SyntaxKind.InternalKeyword => "internal",
                SyntaxKind.ProtectedKeyword => "protected",
                SyntaxKind.StaticKeyword => "static",
                SyntaxKind.ReadOnlyKeyword => "readonly",
                SyntaxKind.SealedKeyword => "sealed",
                SyntaxKind.ConstKeyword => "const",
                SyntaxKind.FixedKeyword => "fixed",
                SyntaxKind.StackAllocKeyword => "stackalloc",
                SyntaxKind.VolatileKeyword => "volatile",
                SyntaxKind.NewKeyword => "new",
                SyntaxKind.OverrideKeyword => "override",
                SyntaxKind.AbstractKeyword => "abstract",
                SyntaxKind.VirtualKeyword => "virtual",
                SyntaxKind.EventKeyword => "event",
                SyntaxKind.ExternKeyword => "extern",
                SyntaxKind.RefKeyword => "ref",
                SyntaxKind.OutKeyword => "out",
                SyntaxKind.InKeyword => "in",
                SyntaxKind.IsKeyword => "is",
                SyntaxKind.AsKeyword => "as",
                SyntaxKind.ParamsKeyword => "params",
                SyntaxKind.ArgListKeyword => "__arglist",
                SyntaxKind.MakeRefKeyword => "__makeref",
                SyntaxKind.RefTypeKeyword => "__reftype",
                SyntaxKind.RefValueKeyword => "__refvalue",
                SyntaxKind.ThisKeyword => "this",
                SyntaxKind.BaseKeyword => "base",
                SyntaxKind.NamespaceKeyword => "namespace",
                SyntaxKind.UsingKeyword => "using",
                SyntaxKind.ClassKeyword => "class",
                SyntaxKind.StructKeyword => "struct",
                SyntaxKind.InterfaceKeyword => "interface",
                SyntaxKind.EnumKeyword => "enum",
                SyntaxKind.DelegateKeyword => "delegate",
                SyntaxKind.CheckedKeyword => "checked",
                SyntaxKind.UncheckedKeyword => "unchecked",
                SyntaxKind.UnsafeKeyword => "unsafe",
                SyntaxKind.OperatorKeyword => "operator",
                SyntaxKind.ImplicitKeyword => "implicit",
                SyntaxKind.ExplicitKeyword => "explicit",
                SyntaxKind.ElifKeyword => "elif",
                SyntaxKind.EndIfKeyword => "endif",
                SyntaxKind.RegionKeyword => "region",
                SyntaxKind.EndRegionKeyword => "endregion",
                SyntaxKind.DefineKeyword => "define",
                SyntaxKind.UndefKeyword => "undef",
                SyntaxKind.WarningKeyword => "warning",
                SyntaxKind.ErrorKeyword => "error",
                SyntaxKind.LineKeyword => "line",
                SyntaxKind.PragmaKeyword => "pragma",
                SyntaxKind.HiddenKeyword => "hidden",
                SyntaxKind.ChecksumKeyword => "checksum",
                SyntaxKind.DisableKeyword => "disable",
                SyntaxKind.RestoreKeyword => "restore",
                SyntaxKind.ReferenceKeyword => "r",
                SyntaxKind.LoadKeyword => "load",
                SyntaxKind.NullableKeyword => "nullable",
                SyntaxKind.EnableKeyword => "enable",
                SyntaxKind.WarningsKeyword => "warnings",
                SyntaxKind.AnnotationsKeyword => "annotations",
                SyntaxKind.YieldKeyword => "yield",
                SyntaxKind.PartialKeyword => "partial",
                SyntaxKind.FromKeyword => "from",
                SyntaxKind.GroupKeyword => "group",
                SyntaxKind.JoinKeyword => "join",
                SyntaxKind.IntoKeyword => "into",
                SyntaxKind.LetKeyword => "let",
                SyntaxKind.ByKeyword => "by",
                SyntaxKind.WhereKeyword => "where",
                SyntaxKind.SelectKeyword => "select",
                SyntaxKind.GetKeyword => "get",
                SyntaxKind.SetKeyword => "set",
                SyntaxKind.AddKeyword => "add",
                SyntaxKind.RemoveKeyword => "remove",
                SyntaxKind.OrderByKeyword => "orderby",
                SyntaxKind.AliasKeyword => "alias",
                SyntaxKind.OnKeyword => "on",
                SyntaxKind.EqualsKeyword => "equals",
                SyntaxKind.AscendingKeyword => "ascending",
                SyntaxKind.DescendingKeyword => "descending",
                SyntaxKind.AssemblyKeyword => "assembly",
                SyntaxKind.ModuleKeyword => "module",
                SyntaxKind.TypeKeyword => "type",
                SyntaxKind.FieldKeyword => "field",
                SyntaxKind.MethodKeyword => "method",
                SyntaxKind.ParamKeyword => "param",
                SyntaxKind.PropertyKeyword => "property",
                SyntaxKind.TypeVarKeyword => "typevar",
                SyntaxKind.GlobalKeyword => "global",
                SyntaxKind.NameOfKeyword => "nameof",
                SyntaxKind.AsyncKeyword => "async",
                SyntaxKind.AwaitKeyword => "await",
                SyntaxKind.WhenKeyword => "when",
                SyntaxKind.InterpolatedStringStartToken => "$\"",
                SyntaxKind.InterpolatedStringEndToken => "\"",
                SyntaxKind.InterpolatedVerbatimStringStartToken => "$@\"",
                SyntaxKind.UnderscoreToken => "_",
                SyntaxKind.VarKeyword => "var",
                SyntaxKind.AndKeyword => "and",
                SyntaxKind.OrKeyword => "or",
                SyntaxKind.NotKeyword => "not",
                SyntaxKind.DataKeyword => "data",
                SyntaxKind.WithKeyword => "with",
                SyntaxKind.InitKeyword => "init",
                SyntaxKind.RecordKeyword => "record",
                SyntaxKind.ManagedKeyword => "managed",
                SyntaxKind.UnmanagedKeyword => "unmanaged",
                _ => string.Empty,
            };
        }

        public static bool IsTypeParameterVarianceKeyword(SyntaxKind kind)
        {
            if (kind != SyntaxKind.OutKeyword)
            {
                return kind == SyntaxKind.InKeyword;
            }
            return true;
        }

        public static bool IsDocumentationCommentTrivia(SyntaxKind kind)
        {
            if (kind != SyntaxKind.SingleLineDocumentationCommentTrivia)
            {
                return kind == SyntaxKind.MultiLineDocumentationCommentTrivia;
            }
            return true;
        }
    }
}
