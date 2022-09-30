using System;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Syntax;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public abstract class CSharpSyntaxRewriter : CSharpSyntaxVisitor<SyntaxNode>
    {
        private readonly bool _visitIntoStructuredTrivia;

        private int _recursionDepth;

        public virtual bool VisitIntoStructuredTrivia => _visitIntoStructuredTrivia;

        public CSharpSyntaxRewriter(bool visitIntoStructuredTrivia = false)
        {
            _visitIntoStructuredTrivia = visitIntoStructuredTrivia;
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("node")]
        public override SyntaxNode? Visit(SyntaxNode? node)
        {
            if (node != null)
            {
                _recursionDepth++;
                StackGuard.EnsureSufficientExecutionStack(_recursionDepth);
                SyntaxNode? result = ((CSharpSyntaxNode)node).Accept(this);
                _recursionDepth--;
                return result;
            }
            return null;
        }

        public virtual SyntaxToken VisitToken(SyntaxToken token)
        {
            GreenNode node = token.Node;
            if (node == null)
            {
                return token;
            }
            GreenNode leadingTriviaCore = node.GetLeadingTriviaCore();
            GreenNode trailingTriviaCore = node.GetTrailingTriviaCore();
            if (leadingTriviaCore != null)
            {
                SyntaxTriviaList trivia = VisitList(new SyntaxTriviaList(in token, leadingTriviaCore));
                if (trailingTriviaCore != null)
                {
                    int index = ((!leadingTriviaCore.IsList) ? 1 : leadingTriviaCore.SlotCount);
                    SyntaxTriviaList trivia2 = VisitList(new SyntaxTriviaList(token, trailingTriviaCore, token.Position + node.FullWidth - trailingTriviaCore.FullWidth, index));
                    if (trivia.Node != leadingTriviaCore)
                    {
                        token = token.WithLeadingTrivia(trivia);
                    }
                    if (trivia2.Node == trailingTriviaCore)
                    {
                        return token;
                    }
                    return token.WithTrailingTrivia(trivia2);
                }
                if (trivia.Node == leadingTriviaCore)
                {
                    return token;
                }
                return token.WithLeadingTrivia(trivia);
            }
            if (trailingTriviaCore != null)
            {
                SyntaxTriviaList trivia3 = VisitList(new SyntaxTriviaList(in token, trailingTriviaCore, token.Position + node.FullWidth - trailingTriviaCore.FullWidth));
                if (trivia3.Node == trailingTriviaCore)
                {
                    return token;
                }
                return token.WithTrailingTrivia(trivia3);
            }
            return token;
        }

        public virtual SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
        {
            if (VisitIntoStructuredTrivia && trivia.HasStructure)
            {
                CSharpSyntaxNode cSharpSyntaxNode = (CSharpSyntaxNode)trivia.GetStructure();
                StructuredTriviaSyntax structuredTriviaSyntax = (StructuredTriviaSyntax)Visit(cSharpSyntaxNode);
                if (structuredTriviaSyntax != cSharpSyntaxNode)
                {
                    if (structuredTriviaSyntax != null)
                    {
                        return SyntaxFactory.Trivia(structuredTriviaSyntax);
                    }
                    return default(SyntaxTrivia);
                }
            }
            return trivia;
        }

        public virtual SyntaxList<TNode> VisitList<TNode>(SyntaxList<TNode> list) where TNode : SyntaxNode
        {
            SyntaxListBuilder syntaxListBuilder = null;
            int i = 0;
            for (int count = list.Count; i < count; i++)
            {
                TNode val = list[i];
                TNode val2 = VisitListElement(val);
                if (val != val2 && syntaxListBuilder == null)
                {
                    syntaxListBuilder = new SyntaxListBuilder(count);
                    syntaxListBuilder.AddRange(list, 0, i);
                }
                if (syntaxListBuilder != null && val2 != null && !val2.IsKind(SyntaxKind.None))
                {
                    syntaxListBuilder.Add(val2);
                }
            }
            if (syntaxListBuilder != null)
            {
                return syntaxListBuilder.ToList();
            }
            return list;
        }

        public virtual TNode? VisitListElement<TNode>(TNode? node) where TNode : SyntaxNode
        {
            return (TNode)Visit(node);
        }

        public virtual SeparatedSyntaxList<TNode> VisitList<TNode>(SeparatedSyntaxList<TNode> list) where TNode : SyntaxNode
        {
            int count = list.Count;
            int separatorCount = list.SeparatorCount;
            SeparatedSyntaxListBuilder<TNode> separatedSyntaxListBuilder = default(SeparatedSyntaxListBuilder<TNode>);
            int i;
            for (i = 0; i < separatorCount; i++)
            {
                TNode val = list[i];
                TNode val2 = VisitListElement(val);
                SyntaxToken separator = list.GetSeparator(i);
                SyntaxToken separatorToken = VisitListSeparator(separator);
                if (separatedSyntaxListBuilder.IsNull && (val != val2 || separator != separatorToken))
                {
                    separatedSyntaxListBuilder = new SeparatedSyntaxListBuilder<TNode>(count);
                    separatedSyntaxListBuilder.AddRange(in list, i);
                }
                if (separatedSyntaxListBuilder.IsNull)
                {
                    continue;
                }
                if (val2 != null)
                {
                    separatedSyntaxListBuilder.Add(val2);
                    if (separatorToken.RawKind == 0)
                    {
                        throw new InvalidOperationException(CodeAnalysisResources.SeparatorIsExpected);
                    }
                    separatedSyntaxListBuilder.AddSeparator(in separatorToken);
                }
                else if (val2 == null)
                {
                    throw new InvalidOperationException(CodeAnalysisResources.ElementIsExpected);
                }
            }
            if (i < count)
            {
                TNode val3 = list[i];
                TNode val4 = VisitListElement(val3);
                if (separatedSyntaxListBuilder.IsNull && val3 != val4)
                {
                    separatedSyntaxListBuilder = new SeparatedSyntaxListBuilder<TNode>(count);
                    separatedSyntaxListBuilder.AddRange(in list, i);
                }
                if (!separatedSyntaxListBuilder.IsNull && val4 != null)
                {
                    separatedSyntaxListBuilder.Add(val4);
                }
            }
            if (!separatedSyntaxListBuilder.IsNull)
            {
                return separatedSyntaxListBuilder.ToList();
            }
            return list;
        }

        public virtual SyntaxToken VisitListSeparator(SyntaxToken separator)
        {
            return VisitToken(separator);
        }

        public virtual SyntaxTokenList VisitList(SyntaxTokenList list)
        {
            SyntaxTokenListBuilder syntaxTokenListBuilder = null;
            int count = list.Count;
            int num = -1;
            SyntaxTokenList.Enumerator enumerator = list.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SyntaxToken current = enumerator.Current;
                num++;
                SyntaxToken syntaxToken = VisitToken(current);
                if (current != syntaxToken && syntaxTokenListBuilder == null)
                {
                    syntaxTokenListBuilder = new SyntaxTokenListBuilder(count);
                    syntaxTokenListBuilder.Add(list, 0, num);
                }
                if (syntaxTokenListBuilder != null && syntaxToken.Kind() != 0)
                {
                    syntaxTokenListBuilder.Add(syntaxToken);
                }
            }
            return syntaxTokenListBuilder?.ToList() ?? list;
        }

        public virtual SyntaxTriviaList VisitList(SyntaxTriviaList list)
        {
            int count = list.Count;
            if (count != 0)
            {
                SyntaxTriviaListBuilder syntaxTriviaListBuilder = null;
                int num = -1;
                SyntaxTriviaList.Enumerator enumerator = list.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SyntaxTrivia current = enumerator.Current;
                    num++;
                    SyntaxTrivia syntaxTrivia = VisitListElement(current);
                    if (syntaxTrivia != current && syntaxTriviaListBuilder == null)
                    {
                        syntaxTriviaListBuilder = new SyntaxTriviaListBuilder(count);
                        syntaxTriviaListBuilder.Add(in list, 0, num);
                    }
                    if (syntaxTriviaListBuilder != null && syntaxTrivia.Kind() != 0)
                    {
                        syntaxTriviaListBuilder.Add(syntaxTrivia);
                    }
                }
                if (syntaxTriviaListBuilder != null)
                {
                    return syntaxTriviaListBuilder.ToList();
                }
            }
            return list;
        }

        public virtual SyntaxTrivia VisitListElement(SyntaxTrivia element)
        {
            return VisitTrivia(element);
        }

        public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
        {
            return node.Update(VisitToken(node.Identifier));
        }

        public override SyntaxNode? VisitQualifiedName(QualifiedNameSyntax node)
        {
            return node.Update(((NameSyntax)Visit(node.Left)) ?? throw new ArgumentNullException("left"), VisitToken(node.DotToken), ((SimpleNameSyntax)Visit(node.Right)) ?? throw new ArgumentNullException("right"));
        }

        public override SyntaxNode? VisitGenericName(GenericNameSyntax node)
        {
            return node.Update(VisitToken(node.Identifier), ((TypeArgumentListSyntax)Visit(node.TypeArgumentList)) ?? throw new ArgumentNullException("typeArgumentList"));
        }

        public override SyntaxNode? VisitTypeArgumentList(TypeArgumentListSyntax node)
        {
            return node.Update(VisitToken(node.LessThanToken), VisitList(node.Arguments), VisitToken(node.GreaterThanToken));
        }

        public override SyntaxNode? VisitAliasQualifiedName(AliasQualifiedNameSyntax node)
        {
            return node.Update(((IdentifierNameSyntax)Visit(node.Alias)) ?? throw new ArgumentNullException("alias"), VisitToken(node.ColonColonToken), ((SimpleNameSyntax)Visit(node.Name)) ?? throw new ArgumentNullException("name"));
        }

        public override SyntaxNode? VisitPredefinedType(PredefinedTypeSyntax node)
        {
            return node.Update(VisitToken(node.Keyword));
        }

        public override SyntaxNode? VisitArrayType(ArrayTypeSyntax node)
        {
            return node.Update(((TypeSyntax)Visit(node.ElementType)) ?? throw new ArgumentNullException("elementType"), VisitList(node.RankSpecifiers));
        }

        public override SyntaxNode? VisitArrayRankSpecifier(ArrayRankSpecifierSyntax node)
        {
            return node.Update(VisitToken(node.OpenBracketToken), VisitList(node.Sizes), VisitToken(node.CloseBracketToken));
        }

        public override SyntaxNode? VisitPointerType(PointerTypeSyntax node)
        {
            return node.Update(((TypeSyntax)Visit(node.ElementType)) ?? throw new ArgumentNullException("elementType"), VisitToken(node.AsteriskToken));
        }

        public override SyntaxNode? VisitFunctionPointerType(FunctionPointerTypeSyntax node)
        {
            return node.Update(VisitToken(node.DelegateKeyword), VisitToken(node.AsteriskToken), (FunctionPointerCallingConventionSyntax)Visit(node.CallingConvention), ((FunctionPointerParameterListSyntax)Visit(node.ParameterList)) ?? throw new ArgumentNullException("parameterList"));
        }

        public override SyntaxNode? VisitFunctionPointerParameterList(FunctionPointerParameterListSyntax node)
        {
            return node.Update(VisitToken(node.LessThanToken), VisitList(node.Parameters), VisitToken(node.GreaterThanToken));
        }

        public override SyntaxNode? VisitFunctionPointerCallingConvention(FunctionPointerCallingConventionSyntax node)
        {
            return node.Update(VisitToken(node.ManagedOrUnmanagedKeyword), (FunctionPointerUnmanagedCallingConventionListSyntax)Visit(node.UnmanagedCallingConventionList));
        }

        public override SyntaxNode? VisitFunctionPointerUnmanagedCallingConventionList(FunctionPointerUnmanagedCallingConventionListSyntax node)
        {
            return node.Update(VisitToken(node.OpenBracketToken), VisitList(node.CallingConventions), VisitToken(node.CloseBracketToken));
        }

        public override SyntaxNode? VisitFunctionPointerUnmanagedCallingConvention(FunctionPointerUnmanagedCallingConventionSyntax node)
        {
            return node.Update(VisitToken(node.Name));
        }

        public override SyntaxNode? VisitNullableType(NullableTypeSyntax node)
        {
            return node.Update(((TypeSyntax)Visit(node.ElementType)) ?? throw new ArgumentNullException("elementType"), VisitToken(node.QuestionToken));
        }

        public override SyntaxNode? VisitTupleType(TupleTypeSyntax node)
        {
            return node.Update(VisitToken(node.OpenParenToken), VisitList(node.Elements), VisitToken(node.CloseParenToken));
        }

        public override SyntaxNode? VisitTupleElement(TupleElementSyntax node)
        {
            return node.Update(((TypeSyntax)Visit(node.Type)) ?? throw new ArgumentNullException("type"), VisitToken(node.Identifier));
        }

        public override SyntaxNode? VisitOmittedTypeArgument(OmittedTypeArgumentSyntax node)
        {
            return node.Update(VisitToken(node.OmittedTypeArgumentToken));
        }

        public override SyntaxNode? VisitRefType(RefTypeSyntax node)
        {
            return node.Update(VisitToken(node.RefKeyword), VisitToken(node.ReadOnlyKeyword), ((TypeSyntax)Visit(node.Type)) ?? throw new ArgumentNullException("type"));
        }

        public override SyntaxNode? VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
        {
            return node.Update(VisitToken(node.OpenParenToken), ((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"), VisitToken(node.CloseParenToken));
        }

        public override SyntaxNode? VisitTupleExpression(TupleExpressionSyntax node)
        {
            return node.Update(VisitToken(node.OpenParenToken), VisitList(node.Arguments), VisitToken(node.CloseParenToken));
        }

        public override SyntaxNode? VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
            return node.Update(VisitToken(node.OperatorToken), ((ExpressionSyntax)Visit(node.Operand)) ?? throw new ArgumentNullException("operand"));
        }

        public override SyntaxNode? VisitAwaitExpression(AwaitExpressionSyntax node)
        {
            return node.Update(VisitToken(node.AwaitKeyword), ((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"));
        }

        public override SyntaxNode? VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        {
            return node.Update(((ExpressionSyntax)Visit(node.Operand)) ?? throw new ArgumentNullException("operand"), VisitToken(node.OperatorToken));
        }

        public override SyntaxNode? VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            return node.Update(((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"), VisitToken(node.OperatorToken), ((SimpleNameSyntax)Visit(node.Name)) ?? throw new ArgumentNullException("name"));
        }

        public override SyntaxNode? VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
        {
            return node.Update(((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"), VisitToken(node.OperatorToken), ((ExpressionSyntax)Visit(node.WhenNotNull)) ?? throw new ArgumentNullException("whenNotNull"));
        }

        public override SyntaxNode? VisitMemberBindingExpression(MemberBindingExpressionSyntax node)
        {
            return node.Update(VisitToken(node.OperatorToken), ((SimpleNameSyntax)Visit(node.Name)) ?? throw new ArgumentNullException("name"));
        }

        public override SyntaxNode? VisitElementBindingExpression(ElementBindingExpressionSyntax node)
        {
            return node.Update(((BracketedArgumentListSyntax)Visit(node.ArgumentList)) ?? throw new ArgumentNullException("argumentList"));
        }

        public override SyntaxNode? VisitRangeExpression(RangeExpressionSyntax node)
        {
            return node.Update((ExpressionSyntax)Visit(node.LeftOperand), VisitToken(node.OperatorToken), (ExpressionSyntax)Visit(node.RightOperand));
        }

        public override SyntaxNode? VisitImplicitElementAccess(ImplicitElementAccessSyntax node)
        {
            return node.Update(((BracketedArgumentListSyntax)Visit(node.ArgumentList)) ?? throw new ArgumentNullException("argumentList"));
        }

        public override SyntaxNode? VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            return node.Update(((ExpressionSyntax)Visit(node.Left)) ?? throw new ArgumentNullException("left"), VisitToken(node.OperatorToken), ((ExpressionSyntax)Visit(node.Right)) ?? throw new ArgumentNullException("right"));
        }

        public override SyntaxNode? VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            return node.Update(((ExpressionSyntax)Visit(node.Left)) ?? throw new ArgumentNullException("left"), VisitToken(node.OperatorToken), ((ExpressionSyntax)Visit(node.Right)) ?? throw new ArgumentNullException("right"));
        }

        public override SyntaxNode? VisitConditionalExpression(ConditionalExpressionSyntax node)
        {
            return node.Update(((ExpressionSyntax)Visit(node.Condition)) ?? throw new ArgumentNullException("condition"), VisitToken(node.QuestionToken), ((ExpressionSyntax)Visit(node.WhenTrue)) ?? throw new ArgumentNullException("whenTrue"), VisitToken(node.ColonToken), ((ExpressionSyntax)Visit(node.WhenFalse)) ?? throw new ArgumentNullException("whenFalse"));
        }

        public override SyntaxNode? VisitThisExpression(ThisExpressionSyntax node)
        {
            return node.Update(VisitToken(node.Token));
        }

        public override SyntaxNode? VisitBaseExpression(BaseExpressionSyntax node)
        {
            return node.Update(VisitToken(node.Token));
        }

        public override SyntaxNode? VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            return node.Update(VisitToken(node.Token));
        }

        public override SyntaxNode? VisitMakeRefExpression(MakeRefExpressionSyntax node)
        {
            return node.Update(VisitToken(node.Keyword), VisitToken(node.OpenParenToken), ((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"), VisitToken(node.CloseParenToken));
        }

        public override SyntaxNode? VisitRefTypeExpression(RefTypeExpressionSyntax node)
        {
            return node.Update(VisitToken(node.Keyword), VisitToken(node.OpenParenToken), ((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"), VisitToken(node.CloseParenToken));
        }

        public override SyntaxNode? VisitRefValueExpression(RefValueExpressionSyntax node)
        {
            return node.Update(VisitToken(node.Keyword), VisitToken(node.OpenParenToken), ((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"), VisitToken(node.Comma), ((TypeSyntax)Visit(node.Type)) ?? throw new ArgumentNullException("type"), VisitToken(node.CloseParenToken));
        }

        public override SyntaxNode? VisitCheckedExpression(CheckedExpressionSyntax node)
        {
            return node.Update(VisitToken(node.Keyword), VisitToken(node.OpenParenToken), ((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"), VisitToken(node.CloseParenToken));
        }

        public override SyntaxNode? VisitDefaultExpression(DefaultExpressionSyntax node)
        {
            return node.Update(VisitToken(node.Keyword), VisitToken(node.OpenParenToken), ((TypeSyntax)Visit(node.Type)) ?? throw new ArgumentNullException("type"), VisitToken(node.CloseParenToken));
        }

        public override SyntaxNode? VisitTypeOfExpression(TypeOfExpressionSyntax node)
        {
            return node.Update(VisitToken(node.Keyword), VisitToken(node.OpenParenToken), ((TypeSyntax)Visit(node.Type)) ?? throw new ArgumentNullException("type"), VisitToken(node.CloseParenToken));
        }

        public override SyntaxNode? VisitSizeOfExpression(SizeOfExpressionSyntax node)
        {
            return node.Update(VisitToken(node.Keyword), VisitToken(node.OpenParenToken), ((TypeSyntax)Visit(node.Type)) ?? throw new ArgumentNullException("type"), VisitToken(node.CloseParenToken));
        }

        public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            return node.Update(((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"), ((ArgumentListSyntax)Visit(node.ArgumentList)) ?? throw new ArgumentNullException("argumentList"));
        }

        public override SyntaxNode? VisitElementAccessExpression(ElementAccessExpressionSyntax node)
        {
            return node.Update(((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"), ((BracketedArgumentListSyntax)Visit(node.ArgumentList)) ?? throw new ArgumentNullException("argumentList"));
        }

        public override SyntaxNode? VisitArgumentList(ArgumentListSyntax node)
        {
            return node.Update(VisitToken(node.OpenParenToken), VisitList(node.Arguments), VisitToken(node.CloseParenToken));
        }

        public override SyntaxNode? VisitBracketedArgumentList(BracketedArgumentListSyntax node)
        {
            return node.Update(VisitToken(node.OpenBracketToken), VisitList(node.Arguments), VisitToken(node.CloseBracketToken));
        }

        public override SyntaxNode? VisitArgument(ArgumentSyntax node)
        {
            return node.Update((NameColonSyntax)Visit(node.NameColon), VisitToken(node.RefKindKeyword), ((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"));
        }

        public override SyntaxNode? VisitNameColon(NameColonSyntax node)
        {
            return node.Update(((IdentifierNameSyntax)Visit(node.Name)) ?? throw new ArgumentNullException("name"), VisitToken(node.ColonToken));
        }

        public override SyntaxNode? VisitDeclarationExpression(DeclarationExpressionSyntax node)
        {
            return node.Update(((TypeSyntax)Visit(node.Type)) ?? throw new ArgumentNullException("type"), ((VariableDesignationSyntax)Visit(node.Designation)) ?? throw new ArgumentNullException("designation"));
        }

        public override SyntaxNode? VisitCastExpression(CastExpressionSyntax node)
        {
            return node.Update(VisitToken(node.OpenParenToken), ((TypeSyntax)Visit(node.Type)) ?? throw new ArgumentNullException("type"), VisitToken(node.CloseParenToken), ((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"));
        }

        public override SyntaxNode? VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
        {
            return node.Update(VisitList(node.Modifiers), VisitToken(node.DelegateKeyword), (ParameterListSyntax)Visit(node.ParameterList), ((BlockSyntax)Visit(node.Block)) ?? throw new ArgumentNullException("block"), (ExpressionSyntax)Visit(node.ExpressionBody));
        }

        public override SyntaxNode? VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), ((ParameterSyntax)Visit(node.Parameter)) ?? throw new ArgumentNullException("parameter"), VisitToken(node.ArrowToken), (BlockSyntax)Visit(node.Block), (ExpressionSyntax)Visit(node.ExpressionBody));
        }

        public override SyntaxNode? VisitRefExpression(RefExpressionSyntax node)
        {
            return node.Update(VisitToken(node.RefKeyword), ((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"));
        }

        public override SyntaxNode? VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), ((ParameterListSyntax)Visit(node.ParameterList)) ?? throw new ArgumentNullException("parameterList"), VisitToken(node.ArrowToken), (BlockSyntax)Visit(node.Block), (ExpressionSyntax)Visit(node.ExpressionBody));
        }

        public override SyntaxNode? VisitInitializerExpression(InitializerExpressionSyntax node)
        {
            return node.Update(VisitToken(node.OpenBraceToken), VisitList(node.Expressions), VisitToken(node.CloseBraceToken));
        }

        public override SyntaxNode? VisitImplicitObjectCreationExpression(ImplicitObjectCreationExpressionSyntax node)
        {
            return node.Update(VisitToken(node.NewKeyword), ((ArgumentListSyntax)Visit(node.ArgumentList)) ?? throw new ArgumentNullException("argumentList"), (InitializerExpressionSyntax)Visit(node.Initializer));
        }

        public override SyntaxNode? VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            return node.Update(VisitToken(node.NewKeyword), ((TypeSyntax)Visit(node.Type)) ?? throw new ArgumentNullException("type"), (ArgumentListSyntax)Visit(node.ArgumentList), (InitializerExpressionSyntax)Visit(node.Initializer));
        }

        public override SyntaxNode? VisitWithExpression(WithExpressionSyntax node)
        {
            return node.Update(((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"), VisitToken(node.WithKeyword), ((InitializerExpressionSyntax)Visit(node.Initializer)) ?? throw new ArgumentNullException("initializer"));
        }

        public override SyntaxNode? VisitAnonymousObjectMemberDeclarator(AnonymousObjectMemberDeclaratorSyntax node)
        {
            return node.Update((NameEqualsSyntax)Visit(node.NameEquals), ((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"));
        }

        public override SyntaxNode? VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node)
        {
            return node.Update(VisitToken(node.NewKeyword), VisitToken(node.OpenBraceToken), VisitList(node.Initializers), VisitToken(node.CloseBraceToken));
        }

        public override SyntaxNode? VisitArrayCreationExpression(ArrayCreationExpressionSyntax node)
        {
            return node.Update(VisitToken(node.NewKeyword), ((ArrayTypeSyntax)Visit(node.Type)) ?? throw new ArgumentNullException("type"), (InitializerExpressionSyntax)Visit(node.Initializer));
        }

        public override SyntaxNode? VisitImplicitArrayCreationExpression(ImplicitArrayCreationExpressionSyntax node)
        {
            return node.Update(VisitToken(node.NewKeyword), VisitToken(node.OpenBracketToken), VisitList(node.Commas), VisitToken(node.CloseBracketToken), ((InitializerExpressionSyntax)Visit(node.Initializer)) ?? throw new ArgumentNullException("initializer"));
        }

        public override SyntaxNode? VisitStackAllocArrayCreationExpression(StackAllocArrayCreationExpressionSyntax node)
        {
            return node.Update(VisitToken(node.StackAllocKeyword), ((TypeSyntax)Visit(node.Type)) ?? throw new ArgumentNullException("type"), (InitializerExpressionSyntax)Visit(node.Initializer));
        }

        public override SyntaxNode? VisitImplicitStackAllocArrayCreationExpression(ImplicitStackAllocArrayCreationExpressionSyntax node)
        {
            return node.Update(VisitToken(node.StackAllocKeyword), VisitToken(node.OpenBracketToken), VisitToken(node.CloseBracketToken), ((InitializerExpressionSyntax)Visit(node.Initializer)) ?? throw new ArgumentNullException("initializer"));
        }

        public override SyntaxNode? VisitQueryExpression(QueryExpressionSyntax node)
        {
            return node.Update(((FromClauseSyntax)Visit(node.FromClause)) ?? throw new ArgumentNullException("fromClause"), ((QueryBodySyntax)Visit(node.Body)) ?? throw new ArgumentNullException("body"));
        }

        public override SyntaxNode? VisitQueryBody(QueryBodySyntax node)
        {
            return node.Update(VisitList(node.Clauses), ((SelectOrGroupClauseSyntax)Visit(node.SelectOrGroup)) ?? throw new ArgumentNullException("selectOrGroup"), (QueryContinuationSyntax)Visit(node.Continuation));
        }

        public override SyntaxNode? VisitFromClause(FromClauseSyntax node)
        {
            return node.Update(VisitToken(node.FromKeyword), (TypeSyntax)Visit(node.Type), VisitToken(node.Identifier), VisitToken(node.InKeyword), ((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"));
        }

        public override SyntaxNode? VisitLetClause(LetClauseSyntax node)
        {
            return node.Update(VisitToken(node.LetKeyword), VisitToken(node.Identifier), VisitToken(node.EqualsToken), ((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"));
        }

        public override SyntaxNode? VisitJoinClause(JoinClauseSyntax node)
        {
            return node.Update(VisitToken(node.JoinKeyword), (TypeSyntax)Visit(node.Type), VisitToken(node.Identifier), VisitToken(node.InKeyword), ((ExpressionSyntax)Visit(node.InExpression)) ?? throw new ArgumentNullException("inExpression"), VisitToken(node.OnKeyword), ((ExpressionSyntax)Visit(node.LeftExpression)) ?? throw new ArgumentNullException("leftExpression"), VisitToken(node.EqualsKeyword), ((ExpressionSyntax)Visit(node.RightExpression)) ?? throw new ArgumentNullException("rightExpression"), (JoinIntoClauseSyntax)Visit(node.Into));
        }

        public override SyntaxNode? VisitJoinIntoClause(JoinIntoClauseSyntax node)
        {
            return node.Update(VisitToken(node.IntoKeyword), VisitToken(node.Identifier));
        }

        public override SyntaxNode? VisitWhereClause(WhereClauseSyntax node)
        {
            return node.Update(VisitToken(node.WhereKeyword), ((ExpressionSyntax)Visit(node.Condition)) ?? throw new ArgumentNullException("condition"));
        }

        public override SyntaxNode? VisitOrderByClause(OrderByClauseSyntax node)
        {
            return node.Update(VisitToken(node.OrderByKeyword), VisitList(node.Orderings));
        }

        public override SyntaxNode? VisitOrdering(OrderingSyntax node)
        {
            return node.Update(((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"), VisitToken(node.AscendingOrDescendingKeyword));
        }

        public override SyntaxNode? VisitSelectClause(SelectClauseSyntax node)
        {
            return node.Update(VisitToken(node.SelectKeyword), ((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"));
        }

        public override SyntaxNode? VisitGroupClause(GroupClauseSyntax node)
        {
            return node.Update(VisitToken(node.GroupKeyword), ((ExpressionSyntax)Visit(node.GroupExpression)) ?? throw new ArgumentNullException("groupExpression"), VisitToken(node.ByKeyword), ((ExpressionSyntax)Visit(node.ByExpression)) ?? throw new ArgumentNullException("byExpression"));
        }

        public override SyntaxNode? VisitQueryContinuation(QueryContinuationSyntax node)
        {
            return node.Update(VisitToken(node.IntoKeyword), VisitToken(node.Identifier), ((QueryBodySyntax)Visit(node.Body)) ?? throw new ArgumentNullException("body"));
        }

        public override SyntaxNode? VisitOmittedArraySizeExpression(OmittedArraySizeExpressionSyntax node)
        {
            return node.Update(VisitToken(node.OmittedArraySizeExpressionToken));
        }

        public override SyntaxNode? VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node)
        {
            return node.Update(VisitToken(node.StringStartToken), VisitList(node.Contents), VisitToken(node.StringEndToken));
        }

        public override SyntaxNode? VisitIsPatternExpression(IsPatternExpressionSyntax node)
        {
            return node.Update(((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"), VisitToken(node.IsKeyword), ((PatternSyntax)Visit(node.Pattern)) ?? throw new ArgumentNullException("pattern"));
        }

        public override SyntaxNode? VisitThrowExpression(ThrowExpressionSyntax node)
        {
            return node.Update(VisitToken(node.ThrowKeyword), ((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"));
        }

        public override SyntaxNode? VisitWhenClause(WhenClauseSyntax node)
        {
            return node.Update(VisitToken(node.WhenKeyword), ((ExpressionSyntax)Visit(node.Condition)) ?? throw new ArgumentNullException("condition"));
        }

        public override SyntaxNode? VisitDiscardPattern(DiscardPatternSyntax node)
        {
            return node.Update(VisitToken(node.UnderscoreToken));
        }

        public override SyntaxNode? VisitDeclarationPattern(DeclarationPatternSyntax node)
        {
            return node.Update(((TypeSyntax)Visit(node.Type)) ?? throw new ArgumentNullException("type"), ((VariableDesignationSyntax)Visit(node.Designation)) ?? throw new ArgumentNullException("designation"));
        }

        public override SyntaxNode? VisitVarPattern(VarPatternSyntax node)
        {
            return node.Update(VisitToken(node.VarKeyword), ((VariableDesignationSyntax)Visit(node.Designation)) ?? throw new ArgumentNullException("designation"));
        }

        public override SyntaxNode? VisitRecursivePattern(RecursivePatternSyntax node)
        {
            return node.Update((TypeSyntax)Visit(node.Type), (PositionalPatternClauseSyntax)Visit(node.PositionalPatternClause), (PropertyPatternClauseSyntax)Visit(node.PropertyPatternClause), (VariableDesignationSyntax)Visit(node.Designation));
        }

        public override SyntaxNode? VisitPositionalPatternClause(PositionalPatternClauseSyntax node)
        {
            return node.Update(VisitToken(node.OpenParenToken), VisitList(node.Subpatterns), VisitToken(node.CloseParenToken));
        }

        public override SyntaxNode? VisitPropertyPatternClause(PropertyPatternClauseSyntax node)
        {
            return node.Update(VisitToken(node.OpenBraceToken), VisitList(node.Subpatterns), VisitToken(node.CloseBraceToken));
        }

        public override SyntaxNode? VisitSubpattern(SubpatternSyntax node)
        {
            return node.Update((NameColonSyntax)Visit(node.NameColon), ((PatternSyntax)Visit(node.Pattern)) ?? throw new ArgumentNullException("pattern"));
        }

        public override SyntaxNode? VisitConstantPattern(ConstantPatternSyntax node)
        {
            return node.Update(((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"));
        }

        public override SyntaxNode? VisitParenthesizedPattern(ParenthesizedPatternSyntax node)
        {
            return node.Update(VisitToken(node.OpenParenToken), ((PatternSyntax)Visit(node.Pattern)) ?? throw new ArgumentNullException("pattern"), VisitToken(node.CloseParenToken));
        }

        public override SyntaxNode? VisitRelationalPattern(RelationalPatternSyntax node)
        {
            return node.Update(VisitToken(node.OperatorToken), ((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"));
        }

        public override SyntaxNode? VisitTypePattern(TypePatternSyntax node)
        {
            return node.Update(((TypeSyntax)Visit(node.Type)) ?? throw new ArgumentNullException("type"));
        }

        public override SyntaxNode? VisitBinaryPattern(BinaryPatternSyntax node)
        {
            return node.Update(((PatternSyntax)Visit(node.Left)) ?? throw new ArgumentNullException("left"), VisitToken(node.OperatorToken), ((PatternSyntax)Visit(node.Right)) ?? throw new ArgumentNullException("right"));
        }

        public override SyntaxNode? VisitUnaryPattern(UnaryPatternSyntax node)
        {
            return node.Update(VisitToken(node.OperatorToken), ((PatternSyntax)Visit(node.Pattern)) ?? throw new ArgumentNullException("pattern"));
        }

        public override SyntaxNode? VisitInterpolatedStringText(InterpolatedStringTextSyntax node)
        {
            return node.Update(VisitToken(node.TextToken));
        }

        public override SyntaxNode? VisitInterpolation(InterpolationSyntax node)
        {
            return node.Update(VisitToken(node.OpenBraceToken), ((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"), (InterpolationAlignmentClauseSyntax)Visit(node.AlignmentClause), (InterpolationFormatClauseSyntax)Visit(node.FormatClause), VisitToken(node.CloseBraceToken));
        }

        public override SyntaxNode? VisitInterpolationAlignmentClause(InterpolationAlignmentClauseSyntax node)
        {
            return node.Update(VisitToken(node.CommaToken), ((ExpressionSyntax)Visit(node.Value)) ?? throw new ArgumentNullException("value"));
        }

        public override SyntaxNode? VisitInterpolationFormatClause(InterpolationFormatClauseSyntax node)
        {
            return node.Update(VisitToken(node.ColonToken), VisitToken(node.FormatStringToken));
        }

        public override SyntaxNode? VisitGlobalStatement(GlobalStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), ((StatementSyntax)Visit(node.Statement)) ?? throw new ArgumentNullException("statement"));
        }

        public override SyntaxNode? VisitBlock(BlockSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitToken(node.OpenBraceToken), VisitList(node.Statements), VisitToken(node.CloseBraceToken));
        }

        public override SyntaxNode? VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), ((TypeSyntax)Visit(node.ReturnType)) ?? throw new ArgumentNullException("returnType"), VisitToken(node.Identifier), (TypeParameterListSyntax)Visit(node.TypeParameterList), ((ParameterListSyntax)Visit(node.ParameterList)) ?? throw new ArgumentNullException("parameterList"), VisitList(node.ConstraintClauses), (BlockSyntax)Visit(node.Body), (ArrowExpressionClauseSyntax)Visit(node.ExpressionBody), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitToken(node.AwaitKeyword), VisitToken(node.UsingKeyword), VisitList(node.Modifiers), ((VariableDeclarationSyntax)Visit(node.Declaration)) ?? throw new ArgumentNullException("declaration"), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            return node.Update(((TypeSyntax)Visit(node.Type)) ?? throw new ArgumentNullException("type"), VisitList(node.Variables));
        }

        public override SyntaxNode? VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            return node.Update(VisitToken(node.Identifier), (BracketedArgumentListSyntax)Visit(node.ArgumentList), (EqualsValueClauseSyntax)Visit(node.Initializer));
        }

        public override SyntaxNode? VisitEqualsValueClause(EqualsValueClauseSyntax node)
        {
            return node.Update(VisitToken(node.EqualsToken), ((ExpressionSyntax)Visit(node.Value)) ?? throw new ArgumentNullException("value"));
        }

        public override SyntaxNode? VisitSingleVariableDesignation(SingleVariableDesignationSyntax node)
        {
            return node.Update(VisitToken(node.Identifier));
        }

        public override SyntaxNode? VisitDiscardDesignation(DiscardDesignationSyntax node)
        {
            return node.Update(VisitToken(node.UnderscoreToken));
        }

        public override SyntaxNode? VisitParenthesizedVariableDesignation(ParenthesizedVariableDesignationSyntax node)
        {
            return node.Update(VisitToken(node.OpenParenToken), VisitList(node.Variables), VisitToken(node.CloseParenToken));
        }

        public override SyntaxNode? VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), ((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitEmptyStatement(EmptyStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitLabeledStatement(LabeledStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitToken(node.Identifier), VisitToken(node.ColonToken), ((StatementSyntax)Visit(node.Statement)) ?? throw new ArgumentNullException("statement"));
        }

        public override SyntaxNode? VisitGotoStatement(GotoStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitToken(node.GotoKeyword), VisitToken(node.CaseOrDefaultKeyword), (ExpressionSyntax)Visit(node.Expression), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitBreakStatement(BreakStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitToken(node.BreakKeyword), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitContinueStatement(ContinueStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitToken(node.ContinueKeyword), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitReturnStatement(ReturnStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitToken(node.ReturnKeyword), (ExpressionSyntax)Visit(node.Expression), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitThrowStatement(ThrowStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitToken(node.ThrowKeyword), (ExpressionSyntax)Visit(node.Expression), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitYieldStatement(YieldStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitToken(node.YieldKeyword), VisitToken(node.ReturnOrBreakKeyword), (ExpressionSyntax)Visit(node.Expression), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitWhileStatement(WhileStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitToken(node.WhileKeyword), VisitToken(node.OpenParenToken), ((ExpressionSyntax)Visit(node.Condition)) ?? throw new ArgumentNullException("condition"), VisitToken(node.CloseParenToken), ((StatementSyntax)Visit(node.Statement)) ?? throw new ArgumentNullException("statement"));
        }

        public override SyntaxNode? VisitDoStatement(DoStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitToken(node.DoKeyword), ((StatementSyntax)Visit(node.Statement)) ?? throw new ArgumentNullException("statement"), VisitToken(node.WhileKeyword), VisitToken(node.OpenParenToken), ((ExpressionSyntax)Visit(node.Condition)) ?? throw new ArgumentNullException("condition"), VisitToken(node.CloseParenToken), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitForStatement(ForStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitToken(node.ForKeyword), VisitToken(node.OpenParenToken), (VariableDeclarationSyntax)Visit(node.Declaration), VisitList(node.Initializers), VisitToken(node.FirstSemicolonToken), (ExpressionSyntax)Visit(node.Condition), VisitToken(node.SecondSemicolonToken), VisitList(node.Incrementors), VisitToken(node.CloseParenToken), ((StatementSyntax)Visit(node.Statement)) ?? throw new ArgumentNullException("statement"));
        }

        public override SyntaxNode? VisitForEachStatement(ForEachStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitToken(node.AwaitKeyword), VisitToken(node.ForEachKeyword), VisitToken(node.OpenParenToken), ((TypeSyntax)Visit(node.Type)) ?? throw new ArgumentNullException("type"), VisitToken(node.Identifier), VisitToken(node.InKeyword), ((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"), VisitToken(node.CloseParenToken), ((StatementSyntax)Visit(node.Statement)) ?? throw new ArgumentNullException("statement"));
        }

        public override SyntaxNode? VisitForEachVariableStatement(ForEachVariableStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitToken(node.AwaitKeyword), VisitToken(node.ForEachKeyword), VisitToken(node.OpenParenToken), ((ExpressionSyntax)Visit(node.Variable)) ?? throw new ArgumentNullException("variable"), VisitToken(node.InKeyword), ((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"), VisitToken(node.CloseParenToken), ((StatementSyntax)Visit(node.Statement)) ?? throw new ArgumentNullException("statement"));
        }

        public override SyntaxNode? VisitUsingStatement(UsingStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitToken(node.AwaitKeyword), VisitToken(node.UsingKeyword), VisitToken(node.OpenParenToken), (VariableDeclarationSyntax)Visit(node.Declaration), (ExpressionSyntax)Visit(node.Expression), VisitToken(node.CloseParenToken), ((StatementSyntax)Visit(node.Statement)) ?? throw new ArgumentNullException("statement"));
        }

        public override SyntaxNode? VisitFixedStatement(FixedStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitToken(node.FixedKeyword), VisitToken(node.OpenParenToken), ((VariableDeclarationSyntax)Visit(node.Declaration)) ?? throw new ArgumentNullException("declaration"), VisitToken(node.CloseParenToken), ((StatementSyntax)Visit(node.Statement)) ?? throw new ArgumentNullException("statement"));
        }

        public override SyntaxNode? VisitCheckedStatement(CheckedStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitToken(node.Keyword), ((BlockSyntax)Visit(node.Block)) ?? throw new ArgumentNullException("block"));
        }

        public override SyntaxNode? VisitUnsafeStatement(UnsafeStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitToken(node.UnsafeKeyword), ((BlockSyntax)Visit(node.Block)) ?? throw new ArgumentNullException("block"));
        }

        public override SyntaxNode? VisitLockStatement(LockStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitToken(node.LockKeyword), VisitToken(node.OpenParenToken), ((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"), VisitToken(node.CloseParenToken), ((StatementSyntax)Visit(node.Statement)) ?? throw new ArgumentNullException("statement"));
        }

        public override SyntaxNode? VisitIfStatement(IfStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitToken(node.IfKeyword), VisitToken(node.OpenParenToken), ((ExpressionSyntax)Visit(node.Condition)) ?? throw new ArgumentNullException("condition"), VisitToken(node.CloseParenToken), ((StatementSyntax)Visit(node.Statement)) ?? throw new ArgumentNullException("statement"), (ElseClauseSyntax)Visit(node.Else));
        }

        public override SyntaxNode? VisitElseClause(ElseClauseSyntax node)
        {
            return node.Update(VisitToken(node.ElseKeyword), ((StatementSyntax)Visit(node.Statement)) ?? throw new ArgumentNullException("statement"));
        }

        public override SyntaxNode? VisitSwitchStatement(SwitchStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitToken(node.SwitchKeyword), VisitToken(node.OpenParenToken), ((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"), VisitToken(node.CloseParenToken), VisitToken(node.OpenBraceToken), VisitList(node.Sections), VisitToken(node.CloseBraceToken));
        }

        public override SyntaxNode? VisitSwitchSection(SwitchSectionSyntax node)
        {
            return node.Update(VisitList(node.Labels), VisitList(node.Statements));
        }

        public override SyntaxNode? VisitCasePatternSwitchLabel(CasePatternSwitchLabelSyntax node)
        {
            return node.Update(VisitToken(node.Keyword), ((PatternSyntax)Visit(node.Pattern)) ?? throw new ArgumentNullException("pattern"), (WhenClauseSyntax)Visit(node.WhenClause), VisitToken(node.ColonToken));
        }

        public override SyntaxNode? VisitCaseSwitchLabel(CaseSwitchLabelSyntax node)
        {
            return node.Update(VisitToken(node.Keyword), ((ExpressionSyntax)Visit(node.Value)) ?? throw new ArgumentNullException("value"), VisitToken(node.ColonToken));
        }

        public override SyntaxNode? VisitDefaultSwitchLabel(DefaultSwitchLabelSyntax node)
        {
            return node.Update(VisitToken(node.Keyword), VisitToken(node.ColonToken));
        }

        public override SyntaxNode? VisitSwitchExpression(SwitchExpressionSyntax node)
        {
            return node.Update(((ExpressionSyntax)Visit(node.GoverningExpression)) ?? throw new ArgumentNullException("governingExpression"), VisitToken(node.SwitchKeyword), VisitToken(node.OpenBraceToken), VisitList(node.Arms), VisitToken(node.CloseBraceToken));
        }

        public override SyntaxNode? VisitSwitchExpressionArm(SwitchExpressionArmSyntax node)
        {
            return node.Update(((PatternSyntax)Visit(node.Pattern)) ?? throw new ArgumentNullException("pattern"), (WhenClauseSyntax)Visit(node.WhenClause), VisitToken(node.EqualsGreaterThanToken), ((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"));
        }

        public override SyntaxNode? VisitTryStatement(TryStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitToken(node.TryKeyword), ((BlockSyntax)Visit(node.Block)) ?? throw new ArgumentNullException("block"), VisitList(node.Catches), (FinallyClauseSyntax)Visit(node.Finally));
        }

        public override SyntaxNode? VisitCatchClause(CatchClauseSyntax node)
        {
            return node.Update(VisitToken(node.CatchKeyword), (CatchDeclarationSyntax)Visit(node.Declaration), (CatchFilterClauseSyntax)Visit(node.Filter), ((BlockSyntax)Visit(node.Block)) ?? throw new ArgumentNullException("block"));
        }

        public override SyntaxNode? VisitCatchDeclaration(CatchDeclarationSyntax node)
        {
            return node.Update(VisitToken(node.OpenParenToken), ((TypeSyntax)Visit(node.Type)) ?? throw new ArgumentNullException("type"), VisitToken(node.Identifier), VisitToken(node.CloseParenToken));
        }

        public override SyntaxNode? VisitCatchFilterClause(CatchFilterClauseSyntax node)
        {
            return node.Update(VisitToken(node.WhenKeyword), VisitToken(node.OpenParenToken), ((ExpressionSyntax)Visit(node.FilterExpression)) ?? throw new ArgumentNullException("filterExpression"), VisitToken(node.CloseParenToken));
        }

        public override SyntaxNode? VisitFinallyClause(FinallyClauseSyntax node)
        {
            return node.Update(VisitToken(node.FinallyKeyword), ((BlockSyntax)Visit(node.Block)) ?? throw new ArgumentNullException("block"));
        }

        public override SyntaxNode? VisitCompilationUnit(CompilationUnitSyntax node)
        {
            return node.Update(VisitList(node.Externs), VisitList(node.Usings), VisitList(node.AttributeLists), VisitList(node.Members), VisitToken(node.EndOfFileToken));
        }

        public override SyntaxNode? VisitExternAliasDirective(ExternAliasDirectiveSyntax node)
        {
            return node.Update(VisitToken(node.ExternKeyword), VisitToken(node.AliasKeyword), VisitToken(node.Identifier), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitUsingDirective(UsingDirectiveSyntax node)
        {
            return node.Update(VisitToken(node.GlobalKeyword), VisitToken(node.UsingKeyword), VisitToken(node.StaticKeyword), (NameEqualsSyntax)Visit(node.Alias), ((NameSyntax)Visit(node.Name)) ?? throw new ArgumentNullException("name"), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.NamespaceKeyword), ((NameSyntax)Visit(node.Name)) ?? throw new ArgumentNullException("name"), VisitToken(node.OpenBraceToken), VisitList(node.Externs), VisitList(node.Usings), VisitList(node.Members), VisitToken(node.CloseBraceToken), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitAttributeList(AttributeListSyntax node)
        {
            return node.Update(VisitToken(node.OpenBracketToken), (AttributeTargetSpecifierSyntax)Visit(node.Target), VisitList(node.Attributes), VisitToken(node.CloseBracketToken));
        }

        public override SyntaxNode? VisitAttributeTargetSpecifier(AttributeTargetSpecifierSyntax node)
        {
            return node.Update(VisitToken(node.Identifier), VisitToken(node.ColonToken));
        }

        public override SyntaxNode? VisitAttribute(AttributeSyntax node)
        {
            return node.Update(((NameSyntax)Visit(node.Name)) ?? throw new ArgumentNullException("name"), (AttributeArgumentListSyntax)Visit(node.ArgumentList));
        }

        public override SyntaxNode? VisitAttributeArgumentList(AttributeArgumentListSyntax node)
        {
            return node.Update(VisitToken(node.OpenParenToken), VisitList(node.Arguments), VisitToken(node.CloseParenToken));
        }

        public override SyntaxNode? VisitAttributeArgument(AttributeArgumentSyntax node)
        {
            return node.Update((NameEqualsSyntax)Visit(node.NameEquals), (NameColonSyntax)Visit(node.NameColon), ((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"));
        }

        public override SyntaxNode? VisitNameEquals(NameEqualsSyntax node)
        {
            return node.Update(((IdentifierNameSyntax)Visit(node.Name)) ?? throw new ArgumentNullException("name"), VisitToken(node.EqualsToken));
        }

        public override SyntaxNode? VisitTypeParameterList(TypeParameterListSyntax node)
        {
            return node.Update(VisitToken(node.LessThanToken), VisitList(node.Parameters), VisitToken(node.GreaterThanToken));
        }

        public override SyntaxNode? VisitTypeParameter(TypeParameterSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitToken(node.VarianceKeyword), VisitToken(node.Identifier));
        }

        public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.Keyword), VisitToken(node.Identifier), (TypeParameterListSyntax)Visit(node.TypeParameterList), (BaseListSyntax)Visit(node.BaseList), VisitList(node.ConstraintClauses), VisitToken(node.OpenBraceToken), VisitList(node.Members), VisitToken(node.CloseBraceToken), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitStructDeclaration(StructDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.Keyword), VisitToken(node.Identifier), (TypeParameterListSyntax)Visit(node.TypeParameterList), (BaseListSyntax)Visit(node.BaseList), VisitList(node.ConstraintClauses), VisitToken(node.OpenBraceToken), VisitList(node.Members), VisitToken(node.CloseBraceToken), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.Keyword), VisitToken(node.Identifier), (TypeParameterListSyntax)Visit(node.TypeParameterList), (BaseListSyntax)Visit(node.BaseList), VisitList(node.ConstraintClauses), VisitToken(node.OpenBraceToken), VisitList(node.Members), VisitToken(node.CloseBraceToken), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitRecordDeclaration(RecordDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.Keyword), VisitToken(node.ClassOrStructKeyword), VisitToken(node.Identifier), (TypeParameterListSyntax)Visit(node.TypeParameterList), (ParameterListSyntax)Visit(node.ParameterList), (BaseListSyntax)Visit(node.BaseList), VisitList(node.ConstraintClauses), VisitToken(node.OpenBraceToken), VisitList(node.Members), VisitToken(node.CloseBraceToken), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.EnumKeyword), VisitToken(node.Identifier), (BaseListSyntax)Visit(node.BaseList), VisitToken(node.OpenBraceToken), VisitList(node.Members), VisitToken(node.CloseBraceToken), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitDelegateDeclaration(DelegateDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.DelegateKeyword), ((TypeSyntax)Visit(node.ReturnType)) ?? throw new ArgumentNullException("returnType"), VisitToken(node.Identifier), (TypeParameterListSyntax)Visit(node.TypeParameterList), ((ParameterListSyntax)Visit(node.ParameterList)) ?? throw new ArgumentNullException("parameterList"), VisitList(node.ConstraintClauses), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.Identifier), (EqualsValueClauseSyntax)Visit(node.EqualsValue));
        }

        public override SyntaxNode? VisitBaseList(BaseListSyntax node)
        {
            return node.Update(VisitToken(node.ColonToken), VisitList(node.Types));
        }

        public override SyntaxNode? VisitSimpleBaseType(SimpleBaseTypeSyntax node)
        {
            return node.Update(((TypeSyntax)Visit(node.Type)) ?? throw new ArgumentNullException("type"));
        }

        public override SyntaxNode? VisitPrimaryConstructorBaseType(PrimaryConstructorBaseTypeSyntax node)
        {
            return node.Update(((TypeSyntax)Visit(node.Type)) ?? throw new ArgumentNullException("type"), ((ArgumentListSyntax)Visit(node.ArgumentList)) ?? throw new ArgumentNullException("argumentList"));
        }

        public override SyntaxNode? VisitTypeParameterConstraintClause(TypeParameterConstraintClauseSyntax node)
        {
            return node.Update(VisitToken(node.WhereKeyword), ((IdentifierNameSyntax)Visit(node.Name)) ?? throw new ArgumentNullException("name"), VisitToken(node.ColonToken), VisitList(node.Constraints));
        }

        public override SyntaxNode? VisitConstructorConstraint(ConstructorConstraintSyntax node)
        {
            return node.Update(VisitToken(node.NewKeyword), VisitToken(node.OpenParenToken), VisitToken(node.CloseParenToken));
        }

        public override SyntaxNode? VisitClassOrStructConstraint(ClassOrStructConstraintSyntax node)
        {
            return node.Update(VisitToken(node.ClassOrStructKeyword), VisitToken(node.QuestionToken));
        }

        public override SyntaxNode? VisitTypeConstraint(TypeConstraintSyntax node)
        {
            return node.Update(((TypeSyntax)Visit(node.Type)) ?? throw new ArgumentNullException("type"));
        }

        public override SyntaxNode? VisitDefaultConstraint(DefaultConstraintSyntax node)
        {
            return node.Update(VisitToken(node.DefaultKeyword));
        }

        public override SyntaxNode? VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), ((VariableDeclarationSyntax)Visit(node.Declaration)) ?? throw new ArgumentNullException("declaration"), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.EventKeyword), ((VariableDeclarationSyntax)Visit(node.Declaration)) ?? throw new ArgumentNullException("declaration"), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitExplicitInterfaceSpecifier(ExplicitInterfaceSpecifierSyntax node)
        {
            return node.Update(((NameSyntax)Visit(node.Name)) ?? throw new ArgumentNullException("name"), VisitToken(node.DotToken));
        }

        public override SyntaxNode? VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), ((TypeSyntax)Visit(node.ReturnType)) ?? throw new ArgumentNullException("returnType"), (ExplicitInterfaceSpecifierSyntax)Visit(node.ExplicitInterfaceSpecifier), VisitToken(node.Identifier), (TypeParameterListSyntax)Visit(node.TypeParameterList), ((ParameterListSyntax)Visit(node.ParameterList)) ?? throw new ArgumentNullException("parameterList"), VisitList(node.ConstraintClauses), (BlockSyntax)Visit(node.Body), (ArrowExpressionClauseSyntax)Visit(node.ExpressionBody), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitOperatorDeclaration(OperatorDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), ((TypeSyntax)Visit(node.ReturnType)) ?? throw new ArgumentNullException("returnType"), VisitToken(node.OperatorKeyword), VisitToken(node.OperatorToken), ((ParameterListSyntax)Visit(node.ParameterList)) ?? throw new ArgumentNullException("parameterList"), (BlockSyntax)Visit(node.Body), (ArrowExpressionClauseSyntax)Visit(node.ExpressionBody), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.ImplicitOrExplicitKeyword), VisitToken(node.OperatorKeyword), ((TypeSyntax)Visit(node.Type)) ?? throw new ArgumentNullException("type"), ((ParameterListSyntax)Visit(node.ParameterList)) ?? throw new ArgumentNullException("parameterList"), (BlockSyntax)Visit(node.Body), (ArrowExpressionClauseSyntax)Visit(node.ExpressionBody), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.Identifier), ((ParameterListSyntax)Visit(node.ParameterList)) ?? throw new ArgumentNullException("parameterList"), (ConstructorInitializerSyntax)Visit(node.Initializer), (BlockSyntax)Visit(node.Body), (ArrowExpressionClauseSyntax)Visit(node.ExpressionBody), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitConstructorInitializer(ConstructorInitializerSyntax node)
        {
            return node.Update(VisitToken(node.ColonToken), VisitToken(node.ThisOrBaseKeyword), ((ArgumentListSyntax)Visit(node.ArgumentList)) ?? throw new ArgumentNullException("argumentList"));
        }

        public override SyntaxNode? VisitDestructorDeclaration(DestructorDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.TildeToken), VisitToken(node.Identifier), ((ParameterListSyntax)Visit(node.ParameterList)) ?? throw new ArgumentNullException("parameterList"), (BlockSyntax)Visit(node.Body), (ArrowExpressionClauseSyntax)Visit(node.ExpressionBody), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), ((TypeSyntax)Visit(node.Type)) ?? throw new ArgumentNullException("type"), (ExplicitInterfaceSpecifierSyntax)Visit(node.ExplicitInterfaceSpecifier), VisitToken(node.Identifier), (AccessorListSyntax)Visit(node.AccessorList), (ArrowExpressionClauseSyntax)Visit(node.ExpressionBody), (EqualsValueClauseSyntax)Visit(node.Initializer), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitArrowExpressionClause(ArrowExpressionClauseSyntax node)
        {
            return node.Update(VisitToken(node.ArrowToken), ((ExpressionSyntax)Visit(node.Expression)) ?? throw new ArgumentNullException("expression"));
        }

        public override SyntaxNode? VisitEventDeclaration(EventDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.EventKeyword), ((TypeSyntax)Visit(node.Type)) ?? throw new ArgumentNullException("type"), (ExplicitInterfaceSpecifierSyntax)Visit(node.ExplicitInterfaceSpecifier), VisitToken(node.Identifier), (AccessorListSyntax)Visit(node.AccessorList), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitIndexerDeclaration(IndexerDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), ((TypeSyntax)Visit(node.Type)) ?? throw new ArgumentNullException("type"), (ExplicitInterfaceSpecifierSyntax)Visit(node.ExplicitInterfaceSpecifier), VisitToken(node.ThisKeyword), ((BracketedParameterListSyntax)Visit(node.ParameterList)) ?? throw new ArgumentNullException("parameterList"), (AccessorListSyntax)Visit(node.AccessorList), (ArrowExpressionClauseSyntax)Visit(node.ExpressionBody), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitAccessorList(AccessorListSyntax node)
        {
            return node.Update(VisitToken(node.OpenBraceToken), VisitList(node.Accessors), VisitToken(node.CloseBraceToken));
        }

        public override SyntaxNode? VisitAccessorDeclaration(AccessorDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), VisitToken(node.Keyword), (BlockSyntax)Visit(node.Body), (ArrowExpressionClauseSyntax)Visit(node.ExpressionBody), VisitToken(node.SemicolonToken));
        }

        public override SyntaxNode? VisitParameterList(ParameterListSyntax node)
        {
            return node.Update(VisitToken(node.OpenParenToken), VisitList(node.Parameters), VisitToken(node.CloseParenToken));
        }

        public override SyntaxNode? VisitBracketedParameterList(BracketedParameterListSyntax node)
        {
            return node.Update(VisitToken(node.OpenBracketToken), VisitList(node.Parameters), VisitToken(node.CloseBracketToken));
        }

        public override SyntaxNode? VisitParameter(ParameterSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (TypeSyntax)Visit(node.Type), VisitToken(node.Identifier), (EqualsValueClauseSyntax)Visit(node.Default));
        }

        public override SyntaxNode? VisitFunctionPointerParameter(FunctionPointerParameterSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), ((TypeSyntax)Visit(node.Type)) ?? throw new ArgumentNullException("type"));
        }

        public override SyntaxNode? VisitIncompleteMember(IncompleteMemberSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (TypeSyntax)Visit(node.Type));
        }

        public override SyntaxNode? VisitSkippedTokensTrivia(SkippedTokensTriviaSyntax node)
        {
            return node.Update(VisitList(node.Tokens));
        }

        public override SyntaxNode? VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax node)
        {
            return node.Update(VisitList(node.Content), VisitToken(node.EndOfComment));
        }

        public override SyntaxNode? VisitTypeCref(TypeCrefSyntax node)
        {
            return node.Update(((TypeSyntax)Visit(node.Type)) ?? throw new ArgumentNullException("type"));
        }

        public override SyntaxNode? VisitQualifiedCref(QualifiedCrefSyntax node)
        {
            return node.Update(((TypeSyntax)Visit(node.Container)) ?? throw new ArgumentNullException("container"), VisitToken(node.DotToken), ((MemberCrefSyntax)Visit(node.Member)) ?? throw new ArgumentNullException("member"));
        }

        public override SyntaxNode? VisitNameMemberCref(NameMemberCrefSyntax node)
        {
            return node.Update(((TypeSyntax)Visit(node.Name)) ?? throw new ArgumentNullException("name"), (CrefParameterListSyntax)Visit(node.Parameters));
        }

        public override SyntaxNode? VisitIndexerMemberCref(IndexerMemberCrefSyntax node)
        {
            return node.Update(VisitToken(node.ThisKeyword), (CrefBracketedParameterListSyntax)Visit(node.Parameters));
        }

        public override SyntaxNode? VisitOperatorMemberCref(OperatorMemberCrefSyntax node)
        {
            return node.Update(VisitToken(node.OperatorKeyword), VisitToken(node.OperatorToken), (CrefParameterListSyntax)Visit(node.Parameters));
        }

        public override SyntaxNode? VisitConversionOperatorMemberCref(ConversionOperatorMemberCrefSyntax node)
        {
            return node.Update(VisitToken(node.ImplicitOrExplicitKeyword), VisitToken(node.OperatorKeyword), ((TypeSyntax)Visit(node.Type)) ?? throw new ArgumentNullException("type"), (CrefParameterListSyntax)Visit(node.Parameters));
        }

        public override SyntaxNode? VisitCrefParameterList(CrefParameterListSyntax node)
        {
            return node.Update(VisitToken(node.OpenParenToken), VisitList(node.Parameters), VisitToken(node.CloseParenToken));
        }

        public override SyntaxNode? VisitCrefBracketedParameterList(CrefBracketedParameterListSyntax node)
        {
            return node.Update(VisitToken(node.OpenBracketToken), VisitList(node.Parameters), VisitToken(node.CloseBracketToken));
        }

        public override SyntaxNode? VisitCrefParameter(CrefParameterSyntax node)
        {
            return node.Update(VisitToken(node.RefKindKeyword), ((TypeSyntax)Visit(node.Type)) ?? throw new ArgumentNullException("type"));
        }

        public override SyntaxNode? VisitXmlElement(XmlElementSyntax node)
        {
            return node.Update(((XmlElementStartTagSyntax)Visit(node.StartTag)) ?? throw new ArgumentNullException("startTag"), VisitList(node.Content), ((XmlElementEndTagSyntax)Visit(node.EndTag)) ?? throw new ArgumentNullException("endTag"));
        }

        public override SyntaxNode? VisitXmlElementStartTag(XmlElementStartTagSyntax node)
        {
            return node.Update(VisitToken(node.LessThanToken), ((XmlNameSyntax)Visit(node.Name)) ?? throw new ArgumentNullException("name"), VisitList(node.Attributes), VisitToken(node.GreaterThanToken));
        }

        public override SyntaxNode? VisitXmlElementEndTag(XmlElementEndTagSyntax node)
        {
            return node.Update(VisitToken(node.LessThanSlashToken), ((XmlNameSyntax)Visit(node.Name)) ?? throw new ArgumentNullException("name"), VisitToken(node.GreaterThanToken));
        }

        public override SyntaxNode? VisitXmlEmptyElement(XmlEmptyElementSyntax node)
        {
            return node.Update(VisitToken(node.LessThanToken), ((XmlNameSyntax)Visit(node.Name)) ?? throw new ArgumentNullException("name"), VisitList(node.Attributes), VisitToken(node.SlashGreaterThanToken));
        }

        public override SyntaxNode? VisitXmlName(XmlNameSyntax node)
        {
            return node.Update((XmlPrefixSyntax)Visit(node.Prefix), VisitToken(node.LocalName));
        }

        public override SyntaxNode? VisitXmlPrefix(XmlPrefixSyntax node)
        {
            return node.Update(VisitToken(node.Prefix), VisitToken(node.ColonToken));
        }

        public override SyntaxNode? VisitXmlTextAttribute(XmlTextAttributeSyntax node)
        {
            return node.Update(((XmlNameSyntax)Visit(node.Name)) ?? throw new ArgumentNullException("name"), VisitToken(node.EqualsToken), VisitToken(node.StartQuoteToken), VisitList(node.TextTokens), VisitToken(node.EndQuoteToken));
        }

        public override SyntaxNode? VisitXmlCrefAttribute(XmlCrefAttributeSyntax node)
        {
            return node.Update(((XmlNameSyntax)Visit(node.Name)) ?? throw new ArgumentNullException("name"), VisitToken(node.EqualsToken), VisitToken(node.StartQuoteToken), ((CrefSyntax)Visit(node.Cref)) ?? throw new ArgumentNullException("cref"), VisitToken(node.EndQuoteToken));
        }

        public override SyntaxNode? VisitXmlNameAttribute(XmlNameAttributeSyntax node)
        {
            return node.Update(((XmlNameSyntax)Visit(node.Name)) ?? throw new ArgumentNullException("name"), VisitToken(node.EqualsToken), VisitToken(node.StartQuoteToken), ((IdentifierNameSyntax)Visit(node.Identifier)) ?? throw new ArgumentNullException("identifier"), VisitToken(node.EndQuoteToken));
        }

        public override SyntaxNode? VisitXmlText(XmlTextSyntax node)
        {
            return node.Update(VisitList(node.TextTokens));
        }

        public override SyntaxNode? VisitXmlCDataSection(XmlCDataSectionSyntax node)
        {
            return node.Update(VisitToken(node.StartCDataToken), VisitList(node.TextTokens), VisitToken(node.EndCDataToken));
        }

        public override SyntaxNode? VisitXmlProcessingInstruction(XmlProcessingInstructionSyntax node)
        {
            return node.Update(VisitToken(node.StartProcessingInstructionToken), ((XmlNameSyntax)Visit(node.Name)) ?? throw new ArgumentNullException("name"), VisitList(node.TextTokens), VisitToken(node.EndProcessingInstructionToken));
        }

        public override SyntaxNode? VisitXmlComment(XmlCommentSyntax node)
        {
            return node.Update(VisitToken(node.LessThanExclamationMinusMinusToken), VisitList(node.TextTokens), VisitToken(node.MinusMinusGreaterThanToken));
        }

        public override SyntaxNode? VisitIfDirectiveTrivia(IfDirectiveTriviaSyntax node)
        {
            return node.Update(VisitToken(node.HashToken), VisitToken(node.IfKeyword), ((ExpressionSyntax)Visit(node.Condition)) ?? throw new ArgumentNullException("condition"), VisitToken(node.EndOfDirectiveToken), node.IsActive, node.BranchTaken, node.ConditionValue);
        }

        public override SyntaxNode? VisitElifDirectiveTrivia(ElifDirectiveTriviaSyntax node)
        {
            return node.Update(VisitToken(node.HashToken), VisitToken(node.ElifKeyword), ((ExpressionSyntax)Visit(node.Condition)) ?? throw new ArgumentNullException("condition"), VisitToken(node.EndOfDirectiveToken), node.IsActive, node.BranchTaken, node.ConditionValue);
        }

        public override SyntaxNode? VisitElseDirectiveTrivia(ElseDirectiveTriviaSyntax node)
        {
            return node.Update(VisitToken(node.HashToken), VisitToken(node.ElseKeyword), VisitToken(node.EndOfDirectiveToken), node.IsActive, node.BranchTaken);
        }

        public override SyntaxNode? VisitEndIfDirectiveTrivia(EndIfDirectiveTriviaSyntax node)
        {
            return node.Update(VisitToken(node.HashToken), VisitToken(node.EndIfKeyword), VisitToken(node.EndOfDirectiveToken), node.IsActive);
        }

        public override SyntaxNode? VisitRegionDirectiveTrivia(RegionDirectiveTriviaSyntax node)
        {
            return node.Update(VisitToken(node.HashToken), VisitToken(node.RegionKeyword), VisitToken(node.EndOfDirectiveToken), node.IsActive);
        }

        public override SyntaxNode? VisitEndRegionDirectiveTrivia(EndRegionDirectiveTriviaSyntax node)
        {
            return node.Update(VisitToken(node.HashToken), VisitToken(node.EndRegionKeyword), VisitToken(node.EndOfDirectiveToken), node.IsActive);
        }

        public override SyntaxNode? VisitErrorDirectiveTrivia(ErrorDirectiveTriviaSyntax node)
        {
            return node.Update(VisitToken(node.HashToken), VisitToken(node.ErrorKeyword), VisitToken(node.EndOfDirectiveToken), node.IsActive);
        }

        public override SyntaxNode? VisitWarningDirectiveTrivia(WarningDirectiveTriviaSyntax node)
        {
            return node.Update(VisitToken(node.HashToken), VisitToken(node.WarningKeyword), VisitToken(node.EndOfDirectiveToken), node.IsActive);
        }

        public override SyntaxNode? VisitBadDirectiveTrivia(BadDirectiveTriviaSyntax node)
        {
            return node.Update(VisitToken(node.HashToken), VisitToken(node.Identifier), VisitToken(node.EndOfDirectiveToken), node.IsActive);
        }

        public override SyntaxNode? VisitDefineDirectiveTrivia(DefineDirectiveTriviaSyntax node)
        {
            return node.Update(VisitToken(node.HashToken), VisitToken(node.DefineKeyword), VisitToken(node.Name), VisitToken(node.EndOfDirectiveToken), node.IsActive);
        }

        public override SyntaxNode? VisitUndefDirectiveTrivia(UndefDirectiveTriviaSyntax node)
        {
            return node.Update(VisitToken(node.HashToken), VisitToken(node.UndefKeyword), VisitToken(node.Name), VisitToken(node.EndOfDirectiveToken), node.IsActive);
        }

        public override SyntaxNode? VisitLineDirectiveTrivia(LineDirectiveTriviaSyntax node)
        {
            return node.Update(VisitToken(node.HashToken), VisitToken(node.LineKeyword), VisitToken(node.Line), VisitToken(node.File), VisitToken(node.EndOfDirectiveToken), node.IsActive);
        }

        public override SyntaxNode? VisitPragmaWarningDirectiveTrivia(PragmaWarningDirectiveTriviaSyntax node)
        {
            return node.Update(VisitToken(node.HashToken), VisitToken(node.PragmaKeyword), VisitToken(node.WarningKeyword), VisitToken(node.DisableOrRestoreKeyword), VisitList(node.ErrorCodes), VisitToken(node.EndOfDirectiveToken), node.IsActive);
        }

        public override SyntaxNode? VisitPragmaChecksumDirectiveTrivia(PragmaChecksumDirectiveTriviaSyntax node)
        {
            return node.Update(VisitToken(node.HashToken), VisitToken(node.PragmaKeyword), VisitToken(node.ChecksumKeyword), VisitToken(node.File), VisitToken(node.Guid), VisitToken(node.Bytes), VisitToken(node.EndOfDirectiveToken), node.IsActive);
        }

        public override SyntaxNode? VisitReferenceDirectiveTrivia(ReferenceDirectiveTriviaSyntax node)
        {
            return node.Update(VisitToken(node.HashToken), VisitToken(node.ReferenceKeyword), VisitToken(node.File), VisitToken(node.EndOfDirectiveToken), node.IsActive);
        }

        public override SyntaxNode? VisitLoadDirectiveTrivia(LoadDirectiveTriviaSyntax node)
        {
            return node.Update(VisitToken(node.HashToken), VisitToken(node.LoadKeyword), VisitToken(node.File), VisitToken(node.EndOfDirectiveToken), node.IsActive);
        }

        public override SyntaxNode? VisitShebangDirectiveTrivia(ShebangDirectiveTriviaSyntax node)
        {
            return node.Update(VisitToken(node.HashToken), VisitToken(node.ExclamationToken), VisitToken(node.EndOfDirectiveToken), node.IsActive);
        }

        public override SyntaxNode? VisitNullableDirectiveTrivia(NullableDirectiveTriviaSyntax node)
        {
            return node.Update(VisitToken(node.HashToken), VisitToken(node.NullableKeyword), VisitToken(node.SettingToken), VisitToken(node.TargetToken), VisitToken(node.EndOfDirectiveToken), node.IsActive);
        }
    }
}
