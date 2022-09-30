using Microsoft.CodeAnalysis.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public class CSharpSyntaxRewriter : CSharpSyntaxVisitor<CSharpSyntaxNode>
    {
        protected readonly bool VisitIntoStructuredTrivia;

        public CSharpSyntaxRewriter(bool visitIntoStructuredTrivia = false)
        {
            VisitIntoStructuredTrivia = visitIntoStructuredTrivia;
        }

        public override CSharpSyntaxNode VisitToken(SyntaxToken token)
        {
            Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CSharpSyntaxNode> syntaxList = VisitList(token.LeadingTrivia);
            Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CSharpSyntaxNode> syntaxList2 = VisitList(token.TrailingTrivia);
            if (syntaxList != token.LeadingTrivia || syntaxList2 != token.TrailingTrivia)
            {
                if (syntaxList != token.LeadingTrivia)
                {
                    token = token.TokenWithLeadingTrivia(syntaxList.Node);
                }
                if (syntaxList2 != token.TrailingTrivia)
                {
                    token = token.TokenWithTrailingTrivia(syntaxList2.Node);
                }
            }
            return token;
        }

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<TNode> VisitList<TNode>(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<TNode> list) where TNode : CSharpSyntaxNode
        {
            SyntaxListBuilder syntaxListBuilder = null;
            int i = 0;
            for (int count = list.Count; i < count; i++)
            {
                TNode val = list[i];
                CSharpSyntaxNode cSharpSyntaxNode = Visit(val);
                if (val != cSharpSyntaxNode && syntaxListBuilder == null)
                {
                    syntaxListBuilder = new SyntaxListBuilder(count);
                    syntaxListBuilder.AddRange(list, 0, i);
                }
                syntaxListBuilder?.Add(cSharpSyntaxNode);
            }
            if (syntaxListBuilder != null)
            {
                return syntaxListBuilder.ToList();
            }
            return list;
        }

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TNode> VisitList<TNode>(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TNode> list) where TNode : CSharpSyntaxNode
        {
            Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CSharpSyntaxNode> syntaxList = list.GetWithSeparators();
            Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CSharpSyntaxNode> syntaxList2 = VisitList(syntaxList);
            if (syntaxList2 != syntaxList)
            {
                return syntaxList2.AsSeparatedList<TNode>();
            }
            return list;
        }

        public override CSharpSyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.Identifier));
        }

        public override CSharpSyntaxNode VisitQualifiedName(QualifiedNameSyntax node)
        {
            return node.Update((NameSyntax)Visit(node.Left), (SyntaxToken)Visit(node.DotToken), (SimpleNameSyntax)Visit(node.Right));
        }

        public override CSharpSyntaxNode VisitGenericName(GenericNameSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.Identifier), (TypeArgumentListSyntax)Visit(node.TypeArgumentList));
        }

        public override CSharpSyntaxNode VisitTypeArgumentList(TypeArgumentListSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.LessThanToken), VisitList(node.Arguments), (SyntaxToken)Visit(node.GreaterThanToken));
        }

        public override CSharpSyntaxNode VisitAliasQualifiedName(AliasQualifiedNameSyntax node)
        {
            return node.Update((IdentifierNameSyntax)Visit(node.Alias), (SyntaxToken)Visit(node.ColonColonToken), (SimpleNameSyntax)Visit(node.Name));
        }

        public override CSharpSyntaxNode VisitPredefinedType(PredefinedTypeSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.Keyword));
        }

        public override CSharpSyntaxNode VisitArrayType(ArrayTypeSyntax node)
        {
            return node.Update((TypeSyntax)Visit(node.ElementType), VisitList(node.RankSpecifiers));
        }

        public override CSharpSyntaxNode VisitArrayRankSpecifier(ArrayRankSpecifierSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OpenBracketToken), VisitList(node.Sizes), (SyntaxToken)Visit(node.CloseBracketToken));
        }

        public override CSharpSyntaxNode VisitPointerType(PointerTypeSyntax node)
        {
            return node.Update((TypeSyntax)Visit(node.ElementType), (SyntaxToken)Visit(node.AsteriskToken));
        }

        public override CSharpSyntaxNode VisitFunctionPointerType(FunctionPointerTypeSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.DelegateKeyword), (SyntaxToken)Visit(node.AsteriskToken), (FunctionPointerCallingConventionSyntax)Visit(node.CallingConvention), (FunctionPointerParameterListSyntax)Visit(node.ParameterList));
        }

        public override CSharpSyntaxNode VisitFunctionPointerParameterList(FunctionPointerParameterListSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.LessThanToken), VisitList(node.Parameters), (SyntaxToken)Visit(node.GreaterThanToken));
        }

        public override CSharpSyntaxNode VisitFunctionPointerCallingConvention(FunctionPointerCallingConventionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.ManagedOrUnmanagedKeyword), (FunctionPointerUnmanagedCallingConventionListSyntax)Visit(node.UnmanagedCallingConventionList));
        }

        public override CSharpSyntaxNode VisitFunctionPointerUnmanagedCallingConventionList(FunctionPointerUnmanagedCallingConventionListSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OpenBracketToken), VisitList(node.CallingConventions), (SyntaxToken)Visit(node.CloseBracketToken));
        }

        public override CSharpSyntaxNode VisitFunctionPointerUnmanagedCallingConvention(FunctionPointerUnmanagedCallingConventionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.Name));
        }

        public override CSharpSyntaxNode VisitNullableType(NullableTypeSyntax node)
        {
            return node.Update((TypeSyntax)Visit(node.ElementType), (SyntaxToken)Visit(node.QuestionToken));
        }

        public override CSharpSyntaxNode VisitTupleType(TupleTypeSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OpenParenToken), VisitList(node.Elements), (SyntaxToken)Visit(node.CloseParenToken));
        }

        public override CSharpSyntaxNode VisitTupleElement(TupleElementSyntax node)
        {
            return node.Update((TypeSyntax)Visit(node.Type), (SyntaxToken)Visit(node.Identifier));
        }

        public override CSharpSyntaxNode VisitOmittedTypeArgument(OmittedTypeArgumentSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OmittedTypeArgumentToken));
        }

        public override CSharpSyntaxNode VisitRefType(RefTypeSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.RefKeyword), (SyntaxToken)Visit(node.ReadOnlyKeyword), (TypeSyntax)Visit(node.Type));
        }

        public override CSharpSyntaxNode VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OpenParenToken), (ExpressionSyntax)Visit(node.Expression), (SyntaxToken)Visit(node.CloseParenToken));
        }

        public override CSharpSyntaxNode VisitTupleExpression(TupleExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OpenParenToken), VisitList(node.Arguments), (SyntaxToken)Visit(node.CloseParenToken));
        }

        public override CSharpSyntaxNode VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OperatorToken), (ExpressionSyntax)Visit(node.Operand));
        }

        public override CSharpSyntaxNode VisitAwaitExpression(AwaitExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.AwaitKeyword), (ExpressionSyntax)Visit(node.Expression));
        }

        public override CSharpSyntaxNode VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        {
            return node.Update((ExpressionSyntax)Visit(node.Operand), (SyntaxToken)Visit(node.OperatorToken));
        }

        public override CSharpSyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            return node.Update((ExpressionSyntax)Visit(node.Expression), (SyntaxToken)Visit(node.OperatorToken), (SimpleNameSyntax)Visit(node.Name));
        }

        public override CSharpSyntaxNode VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
        {
            return node.Update((ExpressionSyntax)Visit(node.Expression), (SyntaxToken)Visit(node.OperatorToken), (ExpressionSyntax)Visit(node.WhenNotNull));
        }

        public override CSharpSyntaxNode VisitMemberBindingExpression(MemberBindingExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OperatorToken), (SimpleNameSyntax)Visit(node.Name));
        }

        public override CSharpSyntaxNode VisitElementBindingExpression(ElementBindingExpressionSyntax node)
        {
            return node.Update((BracketedArgumentListSyntax)Visit(node.ArgumentList));
        }

        public override CSharpSyntaxNode VisitRangeExpression(RangeExpressionSyntax node)
        {
            return node.Update((ExpressionSyntax)Visit(node.LeftOperand), (SyntaxToken)Visit(node.OperatorToken), (ExpressionSyntax)Visit(node.RightOperand));
        }

        public override CSharpSyntaxNode VisitImplicitElementAccess(ImplicitElementAccessSyntax node)
        {
            return node.Update((BracketedArgumentListSyntax)Visit(node.ArgumentList));
        }

        public override CSharpSyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            return node.Update((ExpressionSyntax)Visit(node.Left), (SyntaxToken)Visit(node.OperatorToken), (ExpressionSyntax)Visit(node.Right));
        }

        public override CSharpSyntaxNode VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            return node.Update((ExpressionSyntax)Visit(node.Left), (SyntaxToken)Visit(node.OperatorToken), (ExpressionSyntax)Visit(node.Right));
        }

        public override CSharpSyntaxNode VisitConditionalExpression(ConditionalExpressionSyntax node)
        {
            return node.Update((ExpressionSyntax)Visit(node.Condition), (SyntaxToken)Visit(node.QuestionToken), (ExpressionSyntax)Visit(node.WhenTrue), (SyntaxToken)Visit(node.ColonToken), (ExpressionSyntax)Visit(node.WhenFalse));
        }

        public override CSharpSyntaxNode VisitThisExpression(ThisExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.Token));
        }

        public override CSharpSyntaxNode VisitBaseExpression(BaseExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.Token));
        }

        public override CSharpSyntaxNode VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.Token));
        }

        public override CSharpSyntaxNode VisitMakeRefExpression(MakeRefExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.Keyword), (SyntaxToken)Visit(node.OpenParenToken), (ExpressionSyntax)Visit(node.Expression), (SyntaxToken)Visit(node.CloseParenToken));
        }

        public override CSharpSyntaxNode VisitRefTypeExpression(RefTypeExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.Keyword), (SyntaxToken)Visit(node.OpenParenToken), (ExpressionSyntax)Visit(node.Expression), (SyntaxToken)Visit(node.CloseParenToken));
        }

        public override CSharpSyntaxNode VisitRefValueExpression(RefValueExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.Keyword), (SyntaxToken)Visit(node.OpenParenToken), (ExpressionSyntax)Visit(node.Expression), (SyntaxToken)Visit(node.Comma), (TypeSyntax)Visit(node.Type), (SyntaxToken)Visit(node.CloseParenToken));
        }

        public override CSharpSyntaxNode VisitCheckedExpression(CheckedExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.Keyword), (SyntaxToken)Visit(node.OpenParenToken), (ExpressionSyntax)Visit(node.Expression), (SyntaxToken)Visit(node.CloseParenToken));
        }

        public override CSharpSyntaxNode VisitDefaultExpression(DefaultExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.Keyword), (SyntaxToken)Visit(node.OpenParenToken), (TypeSyntax)Visit(node.Type), (SyntaxToken)Visit(node.CloseParenToken));
        }

        public override CSharpSyntaxNode VisitTypeOfExpression(TypeOfExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.Keyword), (SyntaxToken)Visit(node.OpenParenToken), (TypeSyntax)Visit(node.Type), (SyntaxToken)Visit(node.CloseParenToken));
        }

        public override CSharpSyntaxNode VisitSizeOfExpression(SizeOfExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.Keyword), (SyntaxToken)Visit(node.OpenParenToken), (TypeSyntax)Visit(node.Type), (SyntaxToken)Visit(node.CloseParenToken));
        }

        public override CSharpSyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            return node.Update((ExpressionSyntax)Visit(node.Expression), (ArgumentListSyntax)Visit(node.ArgumentList));
        }

        public override CSharpSyntaxNode VisitElementAccessExpression(ElementAccessExpressionSyntax node)
        {
            return node.Update((ExpressionSyntax)Visit(node.Expression), (BracketedArgumentListSyntax)Visit(node.ArgumentList));
        }

        public override CSharpSyntaxNode VisitArgumentList(ArgumentListSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OpenParenToken), VisitList(node.Arguments), (SyntaxToken)Visit(node.CloseParenToken));
        }

        public override CSharpSyntaxNode VisitBracketedArgumentList(BracketedArgumentListSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OpenBracketToken), VisitList(node.Arguments), (SyntaxToken)Visit(node.CloseBracketToken));
        }

        public override CSharpSyntaxNode VisitArgument(ArgumentSyntax node)
        {
            return node.Update((NameColonSyntax)Visit(node.NameColon), (SyntaxToken)Visit(node.RefKindKeyword), (ExpressionSyntax)Visit(node.Expression));
        }

        public override CSharpSyntaxNode VisitNameColon(NameColonSyntax node)
        {
            return node.Update((IdentifierNameSyntax)Visit(node.Name), (SyntaxToken)Visit(node.ColonToken));
        }

        public override CSharpSyntaxNode VisitDeclarationExpression(DeclarationExpressionSyntax node)
        {
            return node.Update((TypeSyntax)Visit(node.Type), (VariableDesignationSyntax)Visit(node.Designation));
        }

        public override CSharpSyntaxNode VisitCastExpression(CastExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OpenParenToken), (TypeSyntax)Visit(node.Type), (SyntaxToken)Visit(node.CloseParenToken), (ExpressionSyntax)Visit(node.Expression));
        }

        public override CSharpSyntaxNode VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
        {
            return node.Update(VisitList(node.Modifiers), (SyntaxToken)Visit(node.DelegateKeyword), (ParameterListSyntax)Visit(node.ParameterList), (BlockSyntax)Visit(node.Block), (ExpressionSyntax)Visit(node.ExpressionBody));
        }

        public override CSharpSyntaxNode VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (ParameterSyntax)Visit(node.Parameter), (SyntaxToken)Visit(node.ArrowToken), (BlockSyntax)Visit(node.Block), (ExpressionSyntax)Visit(node.ExpressionBody));
        }

        public override CSharpSyntaxNode VisitRefExpression(RefExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.RefKeyword), (ExpressionSyntax)Visit(node.Expression));
        }

        public override CSharpSyntaxNode VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (ParameterListSyntax)Visit(node.ParameterList), (SyntaxToken)Visit(node.ArrowToken), (BlockSyntax)Visit(node.Block), (ExpressionSyntax)Visit(node.ExpressionBody));
        }

        public override CSharpSyntaxNode VisitInitializerExpression(InitializerExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OpenBraceToken), VisitList(node.Expressions), (SyntaxToken)Visit(node.CloseBraceToken));
        }

        public override CSharpSyntaxNode VisitImplicitObjectCreationExpression(ImplicitObjectCreationExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.NewKeyword), (ArgumentListSyntax)Visit(node.ArgumentList), (InitializerExpressionSyntax)Visit(node.Initializer));
        }

        public override CSharpSyntaxNode VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.NewKeyword), (TypeSyntax)Visit(node.Type), (ArgumentListSyntax)Visit(node.ArgumentList), (InitializerExpressionSyntax)Visit(node.Initializer));
        }

        public override CSharpSyntaxNode VisitWithExpression(WithExpressionSyntax node)
        {
            return node.Update((ExpressionSyntax)Visit(node.Expression), (SyntaxToken)Visit(node.WithKeyword), (InitializerExpressionSyntax)Visit(node.Initializer));
        }

        public override CSharpSyntaxNode VisitAnonymousObjectMemberDeclarator(AnonymousObjectMemberDeclaratorSyntax node)
        {
            return node.Update((NameEqualsSyntax)Visit(node.NameEquals), (ExpressionSyntax)Visit(node.Expression));
        }

        public override CSharpSyntaxNode VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.NewKeyword), (SyntaxToken)Visit(node.OpenBraceToken), VisitList(node.Initializers), (SyntaxToken)Visit(node.CloseBraceToken));
        }

        public override CSharpSyntaxNode VisitArrayCreationExpression(ArrayCreationExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.NewKeyword), (ArrayTypeSyntax)Visit(node.Type), (InitializerExpressionSyntax)Visit(node.Initializer));
        }

        public override CSharpSyntaxNode VisitImplicitArrayCreationExpression(ImplicitArrayCreationExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.NewKeyword), (SyntaxToken)Visit(node.OpenBracketToken), VisitList(node.Commas), (SyntaxToken)Visit(node.CloseBracketToken), (InitializerExpressionSyntax)Visit(node.Initializer));
        }

        public override CSharpSyntaxNode VisitStackAllocArrayCreationExpression(StackAllocArrayCreationExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.StackAllocKeyword), (TypeSyntax)Visit(node.Type), (InitializerExpressionSyntax)Visit(node.Initializer));
        }

        public override CSharpSyntaxNode VisitImplicitStackAllocArrayCreationExpression(ImplicitStackAllocArrayCreationExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.StackAllocKeyword), (SyntaxToken)Visit(node.OpenBracketToken), (SyntaxToken)Visit(node.CloseBracketToken), (InitializerExpressionSyntax)Visit(node.Initializer));
        }

        public override CSharpSyntaxNode VisitQueryExpression(QueryExpressionSyntax node)
        {
            return node.Update((FromClauseSyntax)Visit(node.FromClause), (QueryBodySyntax)Visit(node.Body));
        }

        public override CSharpSyntaxNode VisitQueryBody(QueryBodySyntax node)
        {
            return node.Update(VisitList(node.Clauses), (SelectOrGroupClauseSyntax)Visit(node.SelectOrGroup), (QueryContinuationSyntax)Visit(node.Continuation));
        }

        public override CSharpSyntaxNode VisitFromClause(FromClauseSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.FromKeyword), (TypeSyntax)Visit(node.Type), (SyntaxToken)Visit(node.Identifier), (SyntaxToken)Visit(node.InKeyword), (ExpressionSyntax)Visit(node.Expression));
        }

        public override CSharpSyntaxNode VisitLetClause(LetClauseSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.LetKeyword), (SyntaxToken)Visit(node.Identifier), (SyntaxToken)Visit(node.EqualsToken), (ExpressionSyntax)Visit(node.Expression));
        }

        public override CSharpSyntaxNode VisitJoinClause(JoinClauseSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.JoinKeyword), (TypeSyntax)Visit(node.Type), (SyntaxToken)Visit(node.Identifier), (SyntaxToken)Visit(node.InKeyword), (ExpressionSyntax)Visit(node.InExpression), (SyntaxToken)Visit(node.OnKeyword), (ExpressionSyntax)Visit(node.LeftExpression), (SyntaxToken)Visit(node.EqualsKeyword), (ExpressionSyntax)Visit(node.RightExpression), (JoinIntoClauseSyntax)Visit(node.Into));
        }

        public override CSharpSyntaxNode VisitJoinIntoClause(JoinIntoClauseSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.IntoKeyword), (SyntaxToken)Visit(node.Identifier));
        }

        public override CSharpSyntaxNode VisitWhereClause(WhereClauseSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.WhereKeyword), (ExpressionSyntax)Visit(node.Condition));
        }

        public override CSharpSyntaxNode VisitOrderByClause(OrderByClauseSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OrderByKeyword), VisitList(node.Orderings));
        }

        public override CSharpSyntaxNode VisitOrdering(OrderingSyntax node)
        {
            return node.Update((ExpressionSyntax)Visit(node.Expression), (SyntaxToken)Visit(node.AscendingOrDescendingKeyword));
        }

        public override CSharpSyntaxNode VisitSelectClause(SelectClauseSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.SelectKeyword), (ExpressionSyntax)Visit(node.Expression));
        }

        public override CSharpSyntaxNode VisitGroupClause(GroupClauseSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.GroupKeyword), (ExpressionSyntax)Visit(node.GroupExpression), (SyntaxToken)Visit(node.ByKeyword), (ExpressionSyntax)Visit(node.ByExpression));
        }

        public override CSharpSyntaxNode VisitQueryContinuation(QueryContinuationSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.IntoKeyword), (SyntaxToken)Visit(node.Identifier), (QueryBodySyntax)Visit(node.Body));
        }

        public override CSharpSyntaxNode VisitOmittedArraySizeExpression(OmittedArraySizeExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OmittedArraySizeExpressionToken));
        }

        public override CSharpSyntaxNode VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.StringStartToken), VisitList(node.Contents), (SyntaxToken)Visit(node.StringEndToken));
        }

        public override CSharpSyntaxNode VisitIsPatternExpression(IsPatternExpressionSyntax node)
        {
            return node.Update((ExpressionSyntax)Visit(node.Expression), (SyntaxToken)Visit(node.IsKeyword), (PatternSyntax)Visit(node.Pattern));
        }

        public override CSharpSyntaxNode VisitThrowExpression(ThrowExpressionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.ThrowKeyword), (ExpressionSyntax)Visit(node.Expression));
        }

        public override CSharpSyntaxNode VisitWhenClause(WhenClauseSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.WhenKeyword), (ExpressionSyntax)Visit(node.Condition));
        }

        public override CSharpSyntaxNode VisitDiscardPattern(DiscardPatternSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.UnderscoreToken));
        }

        public override CSharpSyntaxNode VisitDeclarationPattern(DeclarationPatternSyntax node)
        {
            return node.Update((TypeSyntax)Visit(node.Type), (VariableDesignationSyntax)Visit(node.Designation));
        }

        public override CSharpSyntaxNode VisitVarPattern(VarPatternSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.VarKeyword), (VariableDesignationSyntax)Visit(node.Designation));
        }

        public override CSharpSyntaxNode VisitRecursivePattern(RecursivePatternSyntax node)
        {
            return node.Update((TypeSyntax)Visit(node.Type), (PositionalPatternClauseSyntax)Visit(node.PositionalPatternClause), (PropertyPatternClauseSyntax)Visit(node.PropertyPatternClause), (VariableDesignationSyntax)Visit(node.Designation));
        }

        public override CSharpSyntaxNode VisitPositionalPatternClause(PositionalPatternClauseSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OpenParenToken), VisitList(node.Subpatterns), (SyntaxToken)Visit(node.CloseParenToken));
        }

        public override CSharpSyntaxNode VisitPropertyPatternClause(PropertyPatternClauseSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OpenBraceToken), VisitList(node.Subpatterns), (SyntaxToken)Visit(node.CloseBraceToken));
        }

        public override CSharpSyntaxNode VisitSubpattern(SubpatternSyntax node)
        {
            return node.Update((NameColonSyntax)Visit(node.NameColon), (PatternSyntax)Visit(node.Pattern));
        }

        public override CSharpSyntaxNode VisitConstantPattern(ConstantPatternSyntax node)
        {
            return node.Update((ExpressionSyntax)Visit(node.Expression));
        }

        public override CSharpSyntaxNode VisitParenthesizedPattern(ParenthesizedPatternSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OpenParenToken), (PatternSyntax)Visit(node.Pattern), (SyntaxToken)Visit(node.CloseParenToken));
        }

        public override CSharpSyntaxNode VisitRelationalPattern(RelationalPatternSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OperatorToken), (ExpressionSyntax)Visit(node.Expression));
        }

        public override CSharpSyntaxNode VisitTypePattern(TypePatternSyntax node)
        {
            return node.Update((TypeSyntax)Visit(node.Type));
        }

        public override CSharpSyntaxNode VisitBinaryPattern(BinaryPatternSyntax node)
        {
            return node.Update((PatternSyntax)Visit(node.Left), (SyntaxToken)Visit(node.OperatorToken), (PatternSyntax)Visit(node.Right));
        }

        public override CSharpSyntaxNode VisitUnaryPattern(UnaryPatternSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OperatorToken), (PatternSyntax)Visit(node.Pattern));
        }

        public override CSharpSyntaxNode VisitInterpolatedStringText(InterpolatedStringTextSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.TextToken));
        }

        public override CSharpSyntaxNode VisitInterpolation(InterpolationSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OpenBraceToken), (ExpressionSyntax)Visit(node.Expression), (InterpolationAlignmentClauseSyntax)Visit(node.AlignmentClause), (InterpolationFormatClauseSyntax)Visit(node.FormatClause), (SyntaxToken)Visit(node.CloseBraceToken));
        }

        public override CSharpSyntaxNode VisitInterpolationAlignmentClause(InterpolationAlignmentClauseSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.CommaToken), (ExpressionSyntax)Visit(node.Value));
        }

        public override CSharpSyntaxNode VisitInterpolationFormatClause(InterpolationFormatClauseSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.ColonToken), (SyntaxToken)Visit(node.FormatStringToken));
        }

        public override CSharpSyntaxNode VisitGlobalStatement(GlobalStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (StatementSyntax)Visit(node.Statement));
        }

        public override CSharpSyntaxNode VisitBlock(BlockSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), (SyntaxToken)Visit(node.OpenBraceToken), VisitList(node.Statements), (SyntaxToken)Visit(node.CloseBraceToken));
        }

        public override CSharpSyntaxNode VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (TypeSyntax)Visit(node.ReturnType), (SyntaxToken)Visit(node.Identifier), (TypeParameterListSyntax)Visit(node.TypeParameterList), (ParameterListSyntax)Visit(node.ParameterList), VisitList(node.ConstraintClauses), (BlockSyntax)Visit(node.Body), (ArrowExpressionClauseSyntax)Visit(node.ExpressionBody), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), (SyntaxToken)Visit(node.AwaitKeyword), (SyntaxToken)Visit(node.UsingKeyword), VisitList(node.Modifiers), (VariableDeclarationSyntax)Visit(node.Declaration), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            return node.Update((TypeSyntax)Visit(node.Type), VisitList(node.Variables));
        }

        public override CSharpSyntaxNode VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.Identifier), (BracketedArgumentListSyntax)Visit(node.ArgumentList), (EqualsValueClauseSyntax)Visit(node.Initializer));
        }

        public override CSharpSyntaxNode VisitEqualsValueClause(EqualsValueClauseSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.EqualsToken), (ExpressionSyntax)Visit(node.Value));
        }

        public override CSharpSyntaxNode VisitSingleVariableDesignation(SingleVariableDesignationSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.Identifier));
        }

        public override CSharpSyntaxNode VisitDiscardDesignation(DiscardDesignationSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.UnderscoreToken));
        }

        public override CSharpSyntaxNode VisitParenthesizedVariableDesignation(ParenthesizedVariableDesignationSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OpenParenToken), VisitList(node.Variables), (SyntaxToken)Visit(node.CloseParenToken));
        }

        public override CSharpSyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), (ExpressionSyntax)Visit(node.Expression), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitEmptyStatement(EmptyStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitLabeledStatement(LabeledStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), (SyntaxToken)Visit(node.Identifier), (SyntaxToken)Visit(node.ColonToken), (StatementSyntax)Visit(node.Statement));
        }

        public override CSharpSyntaxNode VisitGotoStatement(GotoStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), (SyntaxToken)Visit(node.GotoKeyword), (SyntaxToken)Visit(node.CaseOrDefaultKeyword), (ExpressionSyntax)Visit(node.Expression), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitBreakStatement(BreakStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), (SyntaxToken)Visit(node.BreakKeyword), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitContinueStatement(ContinueStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), (SyntaxToken)Visit(node.ContinueKeyword), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitReturnStatement(ReturnStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), (SyntaxToken)Visit(node.ReturnKeyword), (ExpressionSyntax)Visit(node.Expression), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitThrowStatement(ThrowStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), (SyntaxToken)Visit(node.ThrowKeyword), (ExpressionSyntax)Visit(node.Expression), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitYieldStatement(YieldStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), (SyntaxToken)Visit(node.YieldKeyword), (SyntaxToken)Visit(node.ReturnOrBreakKeyword), (ExpressionSyntax)Visit(node.Expression), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitWhileStatement(WhileStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), (SyntaxToken)Visit(node.WhileKeyword), (SyntaxToken)Visit(node.OpenParenToken), (ExpressionSyntax)Visit(node.Condition), (SyntaxToken)Visit(node.CloseParenToken), (StatementSyntax)Visit(node.Statement));
        }

        public override CSharpSyntaxNode VisitDoStatement(DoStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), (SyntaxToken)Visit(node.DoKeyword), (StatementSyntax)Visit(node.Statement), (SyntaxToken)Visit(node.WhileKeyword), (SyntaxToken)Visit(node.OpenParenToken), (ExpressionSyntax)Visit(node.Condition), (SyntaxToken)Visit(node.CloseParenToken), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitForStatement(ForStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), (SyntaxToken)Visit(node.ForKeyword), (SyntaxToken)Visit(node.OpenParenToken), (VariableDeclarationSyntax)Visit(node.Declaration), VisitList(node.Initializers), (SyntaxToken)Visit(node.FirstSemicolonToken), (ExpressionSyntax)Visit(node.Condition), (SyntaxToken)Visit(node.SecondSemicolonToken), VisitList(node.Incrementors), (SyntaxToken)Visit(node.CloseParenToken), (StatementSyntax)Visit(node.Statement));
        }

        public override CSharpSyntaxNode VisitForEachStatement(ForEachStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), (SyntaxToken)Visit(node.AwaitKeyword), (SyntaxToken)Visit(node.ForEachKeyword), (SyntaxToken)Visit(node.OpenParenToken), (TypeSyntax)Visit(node.Type), (SyntaxToken)Visit(node.Identifier), (SyntaxToken)Visit(node.InKeyword), (ExpressionSyntax)Visit(node.Expression), (SyntaxToken)Visit(node.CloseParenToken), (StatementSyntax)Visit(node.Statement));
        }

        public override CSharpSyntaxNode VisitForEachVariableStatement(ForEachVariableStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), (SyntaxToken)Visit(node.AwaitKeyword), (SyntaxToken)Visit(node.ForEachKeyword), (SyntaxToken)Visit(node.OpenParenToken), (ExpressionSyntax)Visit(node.Variable), (SyntaxToken)Visit(node.InKeyword), (ExpressionSyntax)Visit(node.Expression), (SyntaxToken)Visit(node.CloseParenToken), (StatementSyntax)Visit(node.Statement));
        }

        public override CSharpSyntaxNode VisitUsingStatement(UsingStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), (SyntaxToken)Visit(node.AwaitKeyword), (SyntaxToken)Visit(node.UsingKeyword), (SyntaxToken)Visit(node.OpenParenToken), (VariableDeclarationSyntax)Visit(node.Declaration), (ExpressionSyntax)Visit(node.Expression), (SyntaxToken)Visit(node.CloseParenToken), (StatementSyntax)Visit(node.Statement));
        }

        public override CSharpSyntaxNode VisitFixedStatement(FixedStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), (SyntaxToken)Visit(node.FixedKeyword), (SyntaxToken)Visit(node.OpenParenToken), (VariableDeclarationSyntax)Visit(node.Declaration), (SyntaxToken)Visit(node.CloseParenToken), (StatementSyntax)Visit(node.Statement));
        }

        public override CSharpSyntaxNode VisitCheckedStatement(CheckedStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), (SyntaxToken)Visit(node.Keyword), (BlockSyntax)Visit(node.Block));
        }

        public override CSharpSyntaxNode VisitUnsafeStatement(UnsafeStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), (SyntaxToken)Visit(node.UnsafeKeyword), (BlockSyntax)Visit(node.Block));
        }

        public override CSharpSyntaxNode VisitLockStatement(LockStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), (SyntaxToken)Visit(node.LockKeyword), (SyntaxToken)Visit(node.OpenParenToken), (ExpressionSyntax)Visit(node.Expression), (SyntaxToken)Visit(node.CloseParenToken), (StatementSyntax)Visit(node.Statement));
        }

        public override CSharpSyntaxNode VisitIfStatement(IfStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), (SyntaxToken)Visit(node.IfKeyword), (SyntaxToken)Visit(node.OpenParenToken), (ExpressionSyntax)Visit(node.Condition), (SyntaxToken)Visit(node.CloseParenToken), (StatementSyntax)Visit(node.Statement), (ElseClauseSyntax)Visit(node.Else));
        }

        public override CSharpSyntaxNode VisitElseClause(ElseClauseSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.ElseKeyword), (StatementSyntax)Visit(node.Statement));
        }

        public override CSharpSyntaxNode VisitSwitchStatement(SwitchStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), (SyntaxToken)Visit(node.SwitchKeyword), (SyntaxToken)Visit(node.OpenParenToken), (ExpressionSyntax)Visit(node.Expression), (SyntaxToken)Visit(node.CloseParenToken), (SyntaxToken)Visit(node.OpenBraceToken), VisitList(node.Sections), (SyntaxToken)Visit(node.CloseBraceToken));
        }

        public override CSharpSyntaxNode VisitSwitchSection(SwitchSectionSyntax node)
        {
            return node.Update(VisitList(node.Labels), VisitList(node.Statements));
        }

        public override CSharpSyntaxNode VisitCasePatternSwitchLabel(CasePatternSwitchLabelSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.Keyword), (PatternSyntax)Visit(node.Pattern), (WhenClauseSyntax)Visit(node.WhenClause), (SyntaxToken)Visit(node.ColonToken));
        }

        public override CSharpSyntaxNode VisitCaseSwitchLabel(CaseSwitchLabelSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.Keyword), (ExpressionSyntax)Visit(node.Value), (SyntaxToken)Visit(node.ColonToken));
        }

        public override CSharpSyntaxNode VisitDefaultSwitchLabel(DefaultSwitchLabelSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.Keyword), (SyntaxToken)Visit(node.ColonToken));
        }

        public override CSharpSyntaxNode VisitSwitchExpression(SwitchExpressionSyntax node)
        {
            return node.Update((ExpressionSyntax)Visit(node.GoverningExpression), (SyntaxToken)Visit(node.SwitchKeyword), (SyntaxToken)Visit(node.OpenBraceToken), VisitList(node.Arms), (SyntaxToken)Visit(node.CloseBraceToken));
        }

        public override CSharpSyntaxNode VisitSwitchExpressionArm(SwitchExpressionArmSyntax node)
        {
            return node.Update((PatternSyntax)Visit(node.Pattern), (WhenClauseSyntax)Visit(node.WhenClause), (SyntaxToken)Visit(node.EqualsGreaterThanToken), (ExpressionSyntax)Visit(node.Expression));
        }

        public override CSharpSyntaxNode VisitTryStatement(TryStatementSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), (SyntaxToken)Visit(node.TryKeyword), (BlockSyntax)Visit(node.Block), VisitList(node.Catches), (FinallyClauseSyntax)Visit(node.Finally));
        }

        public override CSharpSyntaxNode VisitCatchClause(CatchClauseSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.CatchKeyword), (CatchDeclarationSyntax)Visit(node.Declaration), (CatchFilterClauseSyntax)Visit(node.Filter), (BlockSyntax)Visit(node.Block));
        }

        public override CSharpSyntaxNode VisitCatchDeclaration(CatchDeclarationSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OpenParenToken), (TypeSyntax)Visit(node.Type), (SyntaxToken)Visit(node.Identifier), (SyntaxToken)Visit(node.CloseParenToken));
        }

        public override CSharpSyntaxNode VisitCatchFilterClause(CatchFilterClauseSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.WhenKeyword), (SyntaxToken)Visit(node.OpenParenToken), (ExpressionSyntax)Visit(node.FilterExpression), (SyntaxToken)Visit(node.CloseParenToken));
        }

        public override CSharpSyntaxNode VisitFinallyClause(FinallyClauseSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.FinallyKeyword), (BlockSyntax)Visit(node.Block));
        }

        public override CSharpSyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
        {
            return node.Update(VisitList(node.Externs), VisitList(node.Usings), VisitList(node.AttributeLists), VisitList(node.Members), (SyntaxToken)Visit(node.EndOfFileToken));
        }

        public override CSharpSyntaxNode VisitExternAliasDirective(ExternAliasDirectiveSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.ExternKeyword), (SyntaxToken)Visit(node.AliasKeyword), (SyntaxToken)Visit(node.Identifier), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitUsingDirective(UsingDirectiveSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.GlobalKeyword), (SyntaxToken)Visit(node.UsingKeyword), (SyntaxToken)Visit(node.StaticKeyword), (NameEqualsSyntax)Visit(node.Alias), (NameSyntax)Visit(node.Name), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (SyntaxToken)Visit(node.NamespaceKeyword), (NameSyntax)Visit(node.Name), (SyntaxToken)Visit(node.OpenBraceToken), VisitList(node.Externs), VisitList(node.Usings), VisitList(node.Members), (SyntaxToken)Visit(node.CloseBraceToken), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitAttributeList(AttributeListSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OpenBracketToken), (AttributeTargetSpecifierSyntax)Visit(node.Target), VisitList(node.Attributes), (SyntaxToken)Visit(node.CloseBracketToken));
        }

        public override CSharpSyntaxNode VisitAttributeTargetSpecifier(AttributeTargetSpecifierSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.Identifier), (SyntaxToken)Visit(node.ColonToken));
        }

        public override CSharpSyntaxNode VisitAttribute(AttributeSyntax node)
        {
            return node.Update((NameSyntax)Visit(node.Name), (AttributeArgumentListSyntax)Visit(node.ArgumentList));
        }

        public override CSharpSyntaxNode VisitAttributeArgumentList(AttributeArgumentListSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OpenParenToken), VisitList(node.Arguments), (SyntaxToken)Visit(node.CloseParenToken));
        }

        public override CSharpSyntaxNode VisitAttributeArgument(AttributeArgumentSyntax node)
        {
            return node.Update((NameEqualsSyntax)Visit(node.NameEquals), (NameColonSyntax)Visit(node.NameColon), (ExpressionSyntax)Visit(node.Expression));
        }

        public override CSharpSyntaxNode VisitNameEquals(NameEqualsSyntax node)
        {
            return node.Update((IdentifierNameSyntax)Visit(node.Name), (SyntaxToken)Visit(node.EqualsToken));
        }

        public override CSharpSyntaxNode VisitTypeParameterList(TypeParameterListSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.LessThanToken), VisitList(node.Parameters), (SyntaxToken)Visit(node.GreaterThanToken));
        }

        public override CSharpSyntaxNode VisitTypeParameter(TypeParameterSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), (SyntaxToken)Visit(node.VarianceKeyword), (SyntaxToken)Visit(node.Identifier));
        }

        public override CSharpSyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (SyntaxToken)Visit(node.Keyword), (SyntaxToken)Visit(node.Identifier), (TypeParameterListSyntax)Visit(node.TypeParameterList), (BaseListSyntax)Visit(node.BaseList), VisitList(node.ConstraintClauses), (SyntaxToken)Visit(node.OpenBraceToken), VisitList(node.Members), (SyntaxToken)Visit(node.CloseBraceToken), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitStructDeclaration(StructDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (SyntaxToken)Visit(node.Keyword), (SyntaxToken)Visit(node.Identifier), (TypeParameterListSyntax)Visit(node.TypeParameterList), (BaseListSyntax)Visit(node.BaseList), VisitList(node.ConstraintClauses), (SyntaxToken)Visit(node.OpenBraceToken), VisitList(node.Members), (SyntaxToken)Visit(node.CloseBraceToken), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (SyntaxToken)Visit(node.Keyword), (SyntaxToken)Visit(node.Identifier), (TypeParameterListSyntax)Visit(node.TypeParameterList), (BaseListSyntax)Visit(node.BaseList), VisitList(node.ConstraintClauses), (SyntaxToken)Visit(node.OpenBraceToken), VisitList(node.Members), (SyntaxToken)Visit(node.CloseBraceToken), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitRecordDeclaration(RecordDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (SyntaxToken)Visit(node.Keyword), (SyntaxToken)Visit(node.ClassOrStructKeyword), (SyntaxToken)Visit(node.Identifier), (TypeParameterListSyntax)Visit(node.TypeParameterList), (ParameterListSyntax)Visit(node.ParameterList), (BaseListSyntax)Visit(node.BaseList), VisitList(node.ConstraintClauses), (SyntaxToken)Visit(node.OpenBraceToken), VisitList(node.Members), (SyntaxToken)Visit(node.CloseBraceToken), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (SyntaxToken)Visit(node.EnumKeyword), (SyntaxToken)Visit(node.Identifier), (BaseListSyntax)Visit(node.BaseList), (SyntaxToken)Visit(node.OpenBraceToken), VisitList(node.Members), (SyntaxToken)Visit(node.CloseBraceToken), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitDelegateDeclaration(DelegateDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (SyntaxToken)Visit(node.DelegateKeyword), (TypeSyntax)Visit(node.ReturnType), (SyntaxToken)Visit(node.Identifier), (TypeParameterListSyntax)Visit(node.TypeParameterList), (ParameterListSyntax)Visit(node.ParameterList), VisitList(node.ConstraintClauses), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (SyntaxToken)Visit(node.Identifier), (EqualsValueClauseSyntax)Visit(node.EqualsValue));
        }

        public override CSharpSyntaxNode VisitBaseList(BaseListSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.ColonToken), VisitList(node.Types));
        }

        public override CSharpSyntaxNode VisitSimpleBaseType(SimpleBaseTypeSyntax node)
        {
            return node.Update((TypeSyntax)Visit(node.Type));
        }

        public override CSharpSyntaxNode VisitPrimaryConstructorBaseType(PrimaryConstructorBaseTypeSyntax node)
        {
            return node.Update((TypeSyntax)Visit(node.Type), (ArgumentListSyntax)Visit(node.ArgumentList));
        }

        public override CSharpSyntaxNode VisitTypeParameterConstraintClause(TypeParameterConstraintClauseSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.WhereKeyword), (IdentifierNameSyntax)Visit(node.Name), (SyntaxToken)Visit(node.ColonToken), VisitList(node.Constraints));
        }

        public override CSharpSyntaxNode VisitConstructorConstraint(ConstructorConstraintSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.NewKeyword), (SyntaxToken)Visit(node.OpenParenToken), (SyntaxToken)Visit(node.CloseParenToken));
        }

        public override CSharpSyntaxNode VisitClassOrStructConstraint(ClassOrStructConstraintSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.ClassOrStructKeyword), (SyntaxToken)Visit(node.QuestionToken));
        }

        public override CSharpSyntaxNode VisitTypeConstraint(TypeConstraintSyntax node)
        {
            return node.Update((TypeSyntax)Visit(node.Type));
        }

        public override CSharpSyntaxNode VisitDefaultConstraint(DefaultConstraintSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.DefaultKeyword));
        }

        public override CSharpSyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (VariableDeclarationSyntax)Visit(node.Declaration), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (SyntaxToken)Visit(node.EventKeyword), (VariableDeclarationSyntax)Visit(node.Declaration), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitExplicitInterfaceSpecifier(ExplicitInterfaceSpecifierSyntax node)
        {
            return node.Update((NameSyntax)Visit(node.Name), (SyntaxToken)Visit(node.DotToken));
        }

        public override CSharpSyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (TypeSyntax)Visit(node.ReturnType), (ExplicitInterfaceSpecifierSyntax)Visit(node.ExplicitInterfaceSpecifier), (SyntaxToken)Visit(node.Identifier), (TypeParameterListSyntax)Visit(node.TypeParameterList), (ParameterListSyntax)Visit(node.ParameterList), VisitList(node.ConstraintClauses), (BlockSyntax)Visit(node.Body), (ArrowExpressionClauseSyntax)Visit(node.ExpressionBody), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitOperatorDeclaration(OperatorDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (TypeSyntax)Visit(node.ReturnType), (SyntaxToken)Visit(node.OperatorKeyword), (SyntaxToken)Visit(node.OperatorToken), (ParameterListSyntax)Visit(node.ParameterList), (BlockSyntax)Visit(node.Body), (ArrowExpressionClauseSyntax)Visit(node.ExpressionBody), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (SyntaxToken)Visit(node.ImplicitOrExplicitKeyword), (SyntaxToken)Visit(node.OperatorKeyword), (TypeSyntax)Visit(node.Type), (ParameterListSyntax)Visit(node.ParameterList), (BlockSyntax)Visit(node.Body), (ArrowExpressionClauseSyntax)Visit(node.ExpressionBody), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (SyntaxToken)Visit(node.Identifier), (ParameterListSyntax)Visit(node.ParameterList), (ConstructorInitializerSyntax)Visit(node.Initializer), (BlockSyntax)Visit(node.Body), (ArrowExpressionClauseSyntax)Visit(node.ExpressionBody), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitConstructorInitializer(ConstructorInitializerSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.ColonToken), (SyntaxToken)Visit(node.ThisOrBaseKeyword), (ArgumentListSyntax)Visit(node.ArgumentList));
        }

        public override CSharpSyntaxNode VisitDestructorDeclaration(DestructorDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (SyntaxToken)Visit(node.TildeToken), (SyntaxToken)Visit(node.Identifier), (ParameterListSyntax)Visit(node.ParameterList), (BlockSyntax)Visit(node.Body), (ArrowExpressionClauseSyntax)Visit(node.ExpressionBody), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (TypeSyntax)Visit(node.Type), (ExplicitInterfaceSpecifierSyntax)Visit(node.ExplicitInterfaceSpecifier), (SyntaxToken)Visit(node.Identifier), (AccessorListSyntax)Visit(node.AccessorList), (ArrowExpressionClauseSyntax)Visit(node.ExpressionBody), (EqualsValueClauseSyntax)Visit(node.Initializer), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitArrowExpressionClause(ArrowExpressionClauseSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.ArrowToken), (ExpressionSyntax)Visit(node.Expression));
        }

        public override CSharpSyntaxNode VisitEventDeclaration(EventDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (SyntaxToken)Visit(node.EventKeyword), (TypeSyntax)Visit(node.Type), (ExplicitInterfaceSpecifierSyntax)Visit(node.ExplicitInterfaceSpecifier), (SyntaxToken)Visit(node.Identifier), (AccessorListSyntax)Visit(node.AccessorList), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitIndexerDeclaration(IndexerDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (TypeSyntax)Visit(node.Type), (ExplicitInterfaceSpecifierSyntax)Visit(node.ExplicitInterfaceSpecifier), (SyntaxToken)Visit(node.ThisKeyword), (BracketedParameterListSyntax)Visit(node.ParameterList), (AccessorListSyntax)Visit(node.AccessorList), (ArrowExpressionClauseSyntax)Visit(node.ExpressionBody), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitAccessorList(AccessorListSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OpenBraceToken), VisitList(node.Accessors), (SyntaxToken)Visit(node.CloseBraceToken));
        }

        public override CSharpSyntaxNode VisitAccessorDeclaration(AccessorDeclarationSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (SyntaxToken)Visit(node.Keyword), (BlockSyntax)Visit(node.Body), (ArrowExpressionClauseSyntax)Visit(node.ExpressionBody), (SyntaxToken)Visit(node.SemicolonToken));
        }

        public override CSharpSyntaxNode VisitParameterList(ParameterListSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OpenParenToken), VisitList(node.Parameters), (SyntaxToken)Visit(node.CloseParenToken));
        }

        public override CSharpSyntaxNode VisitBracketedParameterList(BracketedParameterListSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OpenBracketToken), VisitList(node.Parameters), (SyntaxToken)Visit(node.CloseBracketToken));
        }

        public override CSharpSyntaxNode VisitParameter(ParameterSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (TypeSyntax)Visit(node.Type), (SyntaxToken)Visit(node.Identifier), (EqualsValueClauseSyntax)Visit(node.Default));
        }

        public override CSharpSyntaxNode VisitFunctionPointerParameter(FunctionPointerParameterSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (TypeSyntax)Visit(node.Type));
        }

        public override CSharpSyntaxNode VisitIncompleteMember(IncompleteMemberSyntax node)
        {
            return node.Update(VisitList(node.AttributeLists), VisitList(node.Modifiers), (TypeSyntax)Visit(node.Type));
        }

        public override CSharpSyntaxNode VisitSkippedTokensTrivia(SkippedTokensTriviaSyntax node)
        {
            return node.Update(VisitList(node.Tokens));
        }

        public override CSharpSyntaxNode VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax node)
        {
            return node.Update(VisitList(node.Content), (SyntaxToken)Visit(node.EndOfComment));
        }

        public override CSharpSyntaxNode VisitTypeCref(TypeCrefSyntax node)
        {
            return node.Update((TypeSyntax)Visit(node.Type));
        }

        public override CSharpSyntaxNode VisitQualifiedCref(QualifiedCrefSyntax node)
        {
            return node.Update((TypeSyntax)Visit(node.Container), (SyntaxToken)Visit(node.DotToken), (MemberCrefSyntax)Visit(node.Member));
        }

        public override CSharpSyntaxNode VisitNameMemberCref(NameMemberCrefSyntax node)
        {
            return node.Update((TypeSyntax)Visit(node.Name), (CrefParameterListSyntax)Visit(node.Parameters));
        }

        public override CSharpSyntaxNode VisitIndexerMemberCref(IndexerMemberCrefSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.ThisKeyword), (CrefBracketedParameterListSyntax)Visit(node.Parameters));
        }

        public override CSharpSyntaxNode VisitOperatorMemberCref(OperatorMemberCrefSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OperatorKeyword), (SyntaxToken)Visit(node.OperatorToken), (CrefParameterListSyntax)Visit(node.Parameters));
        }

        public override CSharpSyntaxNode VisitConversionOperatorMemberCref(ConversionOperatorMemberCrefSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.ImplicitOrExplicitKeyword), (SyntaxToken)Visit(node.OperatorKeyword), (TypeSyntax)Visit(node.Type), (CrefParameterListSyntax)Visit(node.Parameters));
        }

        public override CSharpSyntaxNode VisitCrefParameterList(CrefParameterListSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OpenParenToken), VisitList(node.Parameters), (SyntaxToken)Visit(node.CloseParenToken));
        }

        public override CSharpSyntaxNode VisitCrefBracketedParameterList(CrefBracketedParameterListSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.OpenBracketToken), VisitList(node.Parameters), (SyntaxToken)Visit(node.CloseBracketToken));
        }

        public override CSharpSyntaxNode VisitCrefParameter(CrefParameterSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.RefKindKeyword), (TypeSyntax)Visit(node.Type));
        }

        public override CSharpSyntaxNode VisitXmlElement(XmlElementSyntax node)
        {
            return node.Update((XmlElementStartTagSyntax)Visit(node.StartTag), VisitList(node.Content), (XmlElementEndTagSyntax)Visit(node.EndTag));
        }

        public override CSharpSyntaxNode VisitXmlElementStartTag(XmlElementStartTagSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.LessThanToken), (XmlNameSyntax)Visit(node.Name), VisitList(node.Attributes), (SyntaxToken)Visit(node.GreaterThanToken));
        }

        public override CSharpSyntaxNode VisitXmlElementEndTag(XmlElementEndTagSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.LessThanSlashToken), (XmlNameSyntax)Visit(node.Name), (SyntaxToken)Visit(node.GreaterThanToken));
        }

        public override CSharpSyntaxNode VisitXmlEmptyElement(XmlEmptyElementSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.LessThanToken), (XmlNameSyntax)Visit(node.Name), VisitList(node.Attributes), (SyntaxToken)Visit(node.SlashGreaterThanToken));
        }

        public override CSharpSyntaxNode VisitXmlName(XmlNameSyntax node)
        {
            return node.Update((XmlPrefixSyntax)Visit(node.Prefix), (SyntaxToken)Visit(node.LocalName));
        }

        public override CSharpSyntaxNode VisitXmlPrefix(XmlPrefixSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.Prefix), (SyntaxToken)Visit(node.ColonToken));
        }

        public override CSharpSyntaxNode VisitXmlTextAttribute(XmlTextAttributeSyntax node)
        {
            return node.Update((XmlNameSyntax)Visit(node.Name), (SyntaxToken)Visit(node.EqualsToken), (SyntaxToken)Visit(node.StartQuoteToken), VisitList(node.TextTokens), (SyntaxToken)Visit(node.EndQuoteToken));
        }

        public override CSharpSyntaxNode VisitXmlCrefAttribute(XmlCrefAttributeSyntax node)
        {
            return node.Update((XmlNameSyntax)Visit(node.Name), (SyntaxToken)Visit(node.EqualsToken), (SyntaxToken)Visit(node.StartQuoteToken), (CrefSyntax)Visit(node.Cref), (SyntaxToken)Visit(node.EndQuoteToken));
        }

        public override CSharpSyntaxNode VisitXmlNameAttribute(XmlNameAttributeSyntax node)
        {
            return node.Update((XmlNameSyntax)Visit(node.Name), (SyntaxToken)Visit(node.EqualsToken), (SyntaxToken)Visit(node.StartQuoteToken), (IdentifierNameSyntax)Visit(node.Identifier), (SyntaxToken)Visit(node.EndQuoteToken));
        }

        public override CSharpSyntaxNode VisitXmlText(XmlTextSyntax node)
        {
            return node.Update(VisitList(node.TextTokens));
        }

        public override CSharpSyntaxNode VisitXmlCDataSection(XmlCDataSectionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.StartCDataToken), VisitList(node.TextTokens), (SyntaxToken)Visit(node.EndCDataToken));
        }

        public override CSharpSyntaxNode VisitXmlProcessingInstruction(XmlProcessingInstructionSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.StartProcessingInstructionToken), (XmlNameSyntax)Visit(node.Name), VisitList(node.TextTokens), (SyntaxToken)Visit(node.EndProcessingInstructionToken));
        }

        public override CSharpSyntaxNode VisitXmlComment(XmlCommentSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.LessThanExclamationMinusMinusToken), VisitList(node.TextTokens), (SyntaxToken)Visit(node.MinusMinusGreaterThanToken));
        }

        public override CSharpSyntaxNode VisitIfDirectiveTrivia(IfDirectiveTriviaSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.HashToken), (SyntaxToken)Visit(node.IfKeyword), (ExpressionSyntax)Visit(node.Condition), (SyntaxToken)Visit(node.EndOfDirectiveToken), node.IsActive, node.BranchTaken, node.ConditionValue);
        }

        public override CSharpSyntaxNode VisitElifDirectiveTrivia(ElifDirectiveTriviaSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.HashToken), (SyntaxToken)Visit(node.ElifKeyword), (ExpressionSyntax)Visit(node.Condition), (SyntaxToken)Visit(node.EndOfDirectiveToken), node.IsActive, node.BranchTaken, node.ConditionValue);
        }

        public override CSharpSyntaxNode VisitElseDirectiveTrivia(ElseDirectiveTriviaSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.HashToken), (SyntaxToken)Visit(node.ElseKeyword), (SyntaxToken)Visit(node.EndOfDirectiveToken), node.IsActive, node.BranchTaken);
        }

        public override CSharpSyntaxNode VisitEndIfDirectiveTrivia(EndIfDirectiveTriviaSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.HashToken), (SyntaxToken)Visit(node.EndIfKeyword), (SyntaxToken)Visit(node.EndOfDirectiveToken), node.IsActive);
        }

        public override CSharpSyntaxNode VisitRegionDirectiveTrivia(RegionDirectiveTriviaSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.HashToken), (SyntaxToken)Visit(node.RegionKeyword), (SyntaxToken)Visit(node.EndOfDirectiveToken), node.IsActive);
        }

        public override CSharpSyntaxNode VisitEndRegionDirectiveTrivia(EndRegionDirectiveTriviaSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.HashToken), (SyntaxToken)Visit(node.EndRegionKeyword), (SyntaxToken)Visit(node.EndOfDirectiveToken), node.IsActive);
        }

        public override CSharpSyntaxNode VisitErrorDirectiveTrivia(ErrorDirectiveTriviaSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.HashToken), (SyntaxToken)Visit(node.ErrorKeyword), (SyntaxToken)Visit(node.EndOfDirectiveToken), node.IsActive);
        }

        public override CSharpSyntaxNode VisitWarningDirectiveTrivia(WarningDirectiveTriviaSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.HashToken), (SyntaxToken)Visit(node.WarningKeyword), (SyntaxToken)Visit(node.EndOfDirectiveToken), node.IsActive);
        }

        public override CSharpSyntaxNode VisitBadDirectiveTrivia(BadDirectiveTriviaSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.HashToken), (SyntaxToken)Visit(node.Identifier), (SyntaxToken)Visit(node.EndOfDirectiveToken), node.IsActive);
        }

        public override CSharpSyntaxNode VisitDefineDirectiveTrivia(DefineDirectiveTriviaSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.HashToken), (SyntaxToken)Visit(node.DefineKeyword), (SyntaxToken)Visit(node.Name), (SyntaxToken)Visit(node.EndOfDirectiveToken), node.IsActive);
        }

        public override CSharpSyntaxNode VisitUndefDirectiveTrivia(UndefDirectiveTriviaSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.HashToken), (SyntaxToken)Visit(node.UndefKeyword), (SyntaxToken)Visit(node.Name), (SyntaxToken)Visit(node.EndOfDirectiveToken), node.IsActive);
        }

        public override CSharpSyntaxNode VisitLineDirectiveTrivia(LineDirectiveTriviaSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.HashToken), (SyntaxToken)Visit(node.LineKeyword), (SyntaxToken)Visit(node.Line), (SyntaxToken)Visit(node.File), (SyntaxToken)Visit(node.EndOfDirectiveToken), node.IsActive);
        }

        public override CSharpSyntaxNode VisitPragmaWarningDirectiveTrivia(PragmaWarningDirectiveTriviaSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.HashToken), (SyntaxToken)Visit(node.PragmaKeyword), (SyntaxToken)Visit(node.WarningKeyword), (SyntaxToken)Visit(node.DisableOrRestoreKeyword), VisitList(node.ErrorCodes), (SyntaxToken)Visit(node.EndOfDirectiveToken), node.IsActive);
        }

        public override CSharpSyntaxNode VisitPragmaChecksumDirectiveTrivia(PragmaChecksumDirectiveTriviaSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.HashToken), (SyntaxToken)Visit(node.PragmaKeyword), (SyntaxToken)Visit(node.ChecksumKeyword), (SyntaxToken)Visit(node.File), (SyntaxToken)Visit(node.Guid), (SyntaxToken)Visit(node.Bytes), (SyntaxToken)Visit(node.EndOfDirectiveToken), node.IsActive);
        }

        public override CSharpSyntaxNode VisitReferenceDirectiveTrivia(ReferenceDirectiveTriviaSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.HashToken), (SyntaxToken)Visit(node.ReferenceKeyword), (SyntaxToken)Visit(node.File), (SyntaxToken)Visit(node.EndOfDirectiveToken), node.IsActive);
        }

        public override CSharpSyntaxNode VisitLoadDirectiveTrivia(LoadDirectiveTriviaSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.HashToken), (SyntaxToken)Visit(node.LoadKeyword), (SyntaxToken)Visit(node.File), (SyntaxToken)Visit(node.EndOfDirectiveToken), node.IsActive);
        }

        public override CSharpSyntaxNode VisitShebangDirectiveTrivia(ShebangDirectiveTriviaSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.HashToken), (SyntaxToken)Visit(node.ExclamationToken), (SyntaxToken)Visit(node.EndOfDirectiveToken), node.IsActive);
        }

        public override CSharpSyntaxNode VisitNullableDirectiveTrivia(NullableDirectiveTriviaSyntax node)
        {
            return node.Update((SyntaxToken)Visit(node.HashToken), (SyntaxToken)Visit(node.NullableKeyword), (SyntaxToken)Visit(node.SettingToken), (SyntaxToken)Visit(node.TargetToken), (SyntaxToken)Visit(node.EndOfDirectiveToken), node.IsActive);
        }
    }
}
