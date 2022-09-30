using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public static class SyntaxExtensions
    {
        internal static ArrowExpressionClauseSyntax? GetExpressionBodySyntax(this CSharpSyntaxNode node)
        {
            ArrowExpressionClauseSyntax result = null;
            switch (node.Kind())
            {
                case SyntaxKind.ArrowExpressionClause:
                    result = (ArrowExpressionClauseSyntax)node;
                    break;
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                case SyntaxKind.ConstructorDeclaration:
                case SyntaxKind.DestructorDeclaration:
                    result = ((BaseMethodDeclarationSyntax)node).ExpressionBody;
                    break;
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                case SyntaxKind.InitAccessorDeclaration:
                    result = ((AccessorDeclarationSyntax)node).ExpressionBody;
                    break;
                case SyntaxKind.PropertyDeclaration:
                    result = ((PropertyDeclarationSyntax)node).ExpressionBody;
                    break;
                case SyntaxKind.IndexerDeclaration:
                    result = ((IndexerDeclarationSyntax)node).ExpressionBody;
                    break;
                default:
                    ExceptionUtilities.UnexpectedValue(node.Kind());
                    break;
            }
            return result;
        }

        public static SyntaxToken NormalizeWhitespace(this SyntaxToken token, string indentation, bool elasticTrivia)
        {
            return SyntaxNormalizer.Normalize(token, indentation, "\r\n", elasticTrivia);
        }

        internal static SyntaxToken Identifier(this DeclarationExpressionSyntax self)
        {
            return ((SingleVariableDesignationSyntax)self.Designation).Identifier;
        }

        public static SyntaxToken NormalizeWhitespace(this SyntaxToken token, string indentation = "    ", string eol = "\r\n", bool elasticTrivia = false)
        {
            return SyntaxNormalizer.Normalize(token, indentation, eol, elasticTrivia);
        }

        public static SyntaxTriviaList NormalizeWhitespace(this SyntaxTriviaList list, string indentation, bool elasticTrivia)
        {
            return SyntaxNormalizer.Normalize(list, indentation, "\r\n", elasticTrivia);
        }

        public static SyntaxTriviaList NormalizeWhitespace(this SyntaxTriviaList list, string indentation = "    ", string eol = "\r\n", bool elasticTrivia = false)
        {
            return SyntaxNormalizer.Normalize(list, indentation, eol, elasticTrivia);
        }

        public static SyntaxTriviaList ToSyntaxTriviaList(this IEnumerable<SyntaxTrivia> sequence)
        {
            return SyntaxFactory.TriviaList(sequence);
        }

        internal static XmlNameAttributeElementKind GetElementKind(this XmlNameAttributeSyntax attributeSyntax)
        {
            CSharpSyntaxNode parent = attributeSyntax.Parent;
            SyntaxKind syntaxKind = parent.Kind();
            string text = syntaxKind switch
            {
                SyntaxKind.XmlEmptyElement => ((XmlEmptyElementSyntax)parent).Name.LocalName.ValueText,
                SyntaxKind.XmlElementStartTag => ((XmlElementStartTagSyntax)parent).Name.LocalName.ValueText,
                _ => throw ExceptionUtilities.UnexpectedValue(syntaxKind),
            };
            if (DocumentationCommentXmlNames.ElementEquals(text, "param"))
            {
                return XmlNameAttributeElementKind.Parameter;
            }
            if (DocumentationCommentXmlNames.ElementEquals(text, "paramref"))
            {
                return XmlNameAttributeElementKind.ParameterReference;
            }
            if (DocumentationCommentXmlNames.ElementEquals(text, "typeparam"))
            {
                return XmlNameAttributeElementKind.TypeParameter;
            }
            if (DocumentationCommentXmlNames.ElementEquals(text, "typeparamref"))
            {
                return XmlNameAttributeElementKind.TypeParameterReference;
            }
            throw ExceptionUtilities.UnexpectedValue(text);
        }

        internal static bool ReportDocumentationCommentDiagnostics(this SyntaxTree tree)
        {
            return (int)tree.Options.DocumentationMode >= 2;
        }

        public static SimpleNameSyntax WithIdentifier(this SimpleNameSyntax simpleName, SyntaxToken identifier)
        {
            if (simpleName.Kind() != SyntaxKind.IdentifierName)
            {
                return ((GenericNameSyntax)simpleName).WithIdentifier(identifier);
            }
            return ((IdentifierNameSyntax)simpleName).WithIdentifier(identifier);
        }

        internal static bool IsTypeInContextWhichNeedsDynamicAttribute(this IdentifierNameSyntax typeNode)
        {
            if (SyntaxFacts.IsInTypeOnlyContext(typeNode))
            {
                return IsInContextWhichNeedsDynamicAttribute(typeNode);
            }
            return false;
        }

        internal static ExpressionSyntax SkipParens(this ExpressionSyntax expression)
        {
            while (expression.Kind() == SyntaxKind.ParenthesizedExpression)
            {
                expression = ((ParenthesizedExpressionSyntax)expression).Expression;
            }
            return expression;
        }

        internal static bool IsDeconstructionLeft(this ExpressionSyntax node)
        {
            return node.Kind() switch
            {
                SyntaxKind.TupleExpression => true,
                SyntaxKind.DeclarationExpression => ((DeclarationExpressionSyntax)node).Designation.Kind() == SyntaxKind.ParenthesizedVariableDesignation,
                _ => false,
            };
        }

        internal static bool IsDeconstruction(this AssignmentExpressionSyntax self)
        {
            return self.Left.IsDeconstructionLeft();
        }

        private static bool IsInContextWhichNeedsDynamicAttribute(CSharpSyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.DelegateDeclaration:
                case SyntaxKind.BaseList:
                case SyntaxKind.SimpleBaseType:
                case SyntaxKind.FieldDeclaration:
                case SyntaxKind.EventFieldDeclaration:
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.EventDeclaration:
                case SyntaxKind.IndexerDeclaration:
                case SyntaxKind.Parameter:
                case SyntaxKind.PrimaryConstructorBaseType:
                    return true;
                case SyntaxKind.Block:
                case SyntaxKind.VariableDeclarator:
                case SyntaxKind.EqualsValueClause:
                case SyntaxKind.Attribute:
                case SyntaxKind.TypeParameterConstraintClause:
                    return false;
                default:
                    if (node.Parent != null)
                    {
                        return IsInContextWhichNeedsDynamicAttribute(node.Parent);
                    }
                    return false;
            }
        }

        public static IndexerDeclarationSyntax Update(this IndexerDeclarationSyntax syntax, SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax type, ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier, SyntaxToken thisKeyword, BracketedParameterListSyntax parameterList, AccessorListSyntax accessorList)
        {
            return syntax.Update(attributeLists, modifiers, type, explicitInterfaceSpecifier, thisKeyword, parameterList, accessorList, null, default(SyntaxToken));
        }

        public static OperatorDeclarationSyntax Update(this OperatorDeclarationSyntax syntax, SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax returnType, SyntaxToken operatorKeyword, SyntaxToken operatorToken, ParameterListSyntax parameterList, BlockSyntax block, SyntaxToken semicolonToken)
        {
            return syntax.Update(attributeLists, modifiers, returnType, operatorKeyword, operatorToken, parameterList, block, null, semicolonToken);
        }

        public static MethodDeclarationSyntax Update(this MethodDeclarationSyntax syntax, SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax returnType, ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier, SyntaxToken identifier, TypeParameterListSyntax typeParameterList, ParameterListSyntax parameterList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, BlockSyntax block, SyntaxToken semicolonToken)
        {
            return syntax.Update(attributeLists, modifiers, returnType, explicitInterfaceSpecifier, identifier, typeParameterList, parameterList, constraintClauses, block, null, semicolonToken);
        }

        internal static CSharpSyntaxNode? GetContainingDeconstruction(this ExpressionSyntax expr)
        {
            SyntaxKind syntaxKind = expr.Kind();
            if (syntaxKind != SyntaxKind.TupleExpression && syntaxKind != SyntaxKind.DeclarationExpression && syntaxKind != SyntaxKind.IdentifierName)
            {
                return null;
            }
            while (true)
            {
                CSharpSyntaxNode parent = expr.Parent;
                if (parent == null)
                {
                    break;
                }
                switch (parent.Kind())
                {
                    case SyntaxKind.Argument:
                        {
                            CSharpSyntaxNode? parent2 = parent.Parent;
                            if (parent2 == null || parent2!.Kind() != SyntaxKind.TupleExpression)
                            {
                                return null;
                            }
                            break;
                        }
                    case SyntaxKind.SimpleAssignmentExpression:
                        if (((AssignmentExpressionSyntax)parent).Left == expr)
                        {
                            return parent;
                        }
                        return null;
                    case SyntaxKind.ForEachVariableStatement:
                        if (((ForEachVariableStatementSyntax)parent).Variable == expr)
                        {
                            return parent;
                        }
                        return null;
                    default:
                        return null;
                }
                expr = (TupleExpressionSyntax)parent.Parent;
            }
            return null;
        }

        internal static bool IsOutDeclaration(this DeclarationExpressionSyntax p)
        {
            CSharpSyntaxNode? parent = p.Parent;
            if (parent != null && parent!.Kind() == SyntaxKind.Argument)
            {
                return ((ArgumentSyntax)p.Parent).RefOrOutKeyword.Kind() == SyntaxKind.OutKeyword;
            }
            return false;
        }

        internal static bool IsOutVarDeclaration(this DeclarationExpressionSyntax p)
        {
            if (p.Designation.Kind() == SyntaxKind.SingleVariableDesignation)
            {
                return p.IsOutDeclaration();
            }
            return false;
        }

        internal static void VisitRankSpecifiers<TArg>(this TypeSyntax type, Action<ArrayRankSpecifierSyntax, TArg> action, in TArg argument)
        {
            ArrayBuilder<SyntaxNode> instance = ArrayBuilder<SyntaxNode>.GetInstance();
            instance.Push(type);
            while (instance.Count > 0)
            {
                SyntaxNode syntaxNode = instance.Pop();
                if (syntaxNode is ArrayRankSpecifierSyntax arg)
                {
                    action(arg, argument);
                    continue;
                }
                type = (TypeSyntax)syntaxNode;
                switch (type.Kind())
                {
                    case SyntaxKind.ArrayType:
                        {
                            ArrayTypeSyntax arrayTypeSyntax = (ArrayTypeSyntax)type;
                            for (int num4 = arrayTypeSyntax.RankSpecifiers.Count - 1; num4 >= 0; num4--)
                            {
                                instance.Push(arrayTypeSyntax.RankSpecifiers[num4]);
                            }
                            instance.Push(arrayTypeSyntax.ElementType);
                            break;
                        }
                    case SyntaxKind.NullableType:
                        {
                            NullableTypeSyntax nullableTypeSyntax = (NullableTypeSyntax)type;
                            instance.Push(nullableTypeSyntax.ElementType);
                            break;
                        }
                    case SyntaxKind.PointerType:
                        {
                            PointerTypeSyntax pointerTypeSyntax = (PointerTypeSyntax)type;
                            instance.Push(pointerTypeSyntax.ElementType);
                            break;
                        }
                    case SyntaxKind.FunctionPointerType:
                        {
                            FunctionPointerTypeSyntax functionPointerTypeSyntax = (FunctionPointerTypeSyntax)type;
                            for (int num3 = functionPointerTypeSyntax.ParameterList.Parameters.Count - 1; num3 >= 0; num3--)
                            {
                                TypeSyntax type2 = functionPointerTypeSyntax.ParameterList.Parameters[num3].Type;
                                instance.Push(type2);
                            }
                            break;
                        }
                    case SyntaxKind.TupleType:
                        {
                            TupleTypeSyntax tupleTypeSyntax = (TupleTypeSyntax)type;
                            for (int num2 = tupleTypeSyntax.Elements.Count - 1; num2 >= 0; num2--)
                            {
                                instance.Push(tupleTypeSyntax.Elements[num2].Type);
                            }
                            break;
                        }
                    case SyntaxKind.RefType:
                        {
                            RefTypeSyntax refTypeSyntax = (RefTypeSyntax)type;
                            instance.Push(refTypeSyntax.Type);
                            break;
                        }
                    case SyntaxKind.GenericName:
                        {
                            GenericNameSyntax genericNameSyntax = (GenericNameSyntax)type;
                            for (int num = genericNameSyntax.TypeArgumentList.Arguments.Count - 1; num >= 0; num--)
                            {
                                instance.Push(genericNameSyntax.TypeArgumentList.Arguments[num]);
                            }
                            break;
                        }
                    case SyntaxKind.QualifiedName:
                        {
                            QualifiedNameSyntax qualifiedNameSyntax = (QualifiedNameSyntax)type;
                            instance.Push(qualifiedNameSyntax.Right);
                            instance.Push(qualifiedNameSyntax.Left);
                            break;
                        }
                    case SyntaxKind.AliasQualifiedName:
                        {
                            AliasQualifiedNameSyntax aliasQualifiedNameSyntax = (AliasQualifiedNameSyntax)type;
                            instance.Push(aliasQualifiedNameSyntax.Name);
                            break;
                        }
                    default:
                        throw ExceptionUtilities.UnexpectedValue(type.Kind());
                    case SyntaxKind.IdentifierName:
                    case SyntaxKind.PredefinedType:
                    case SyntaxKind.OmittedTypeArgument:
                        break;
                }
            }
            instance.Free();
        }
    }
}
