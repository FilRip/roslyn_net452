#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ParenthesizedLambdaExpressionSyntax : LambdaExpressionSyntax
    {
        private SyntaxNode? attributeLists;

        private ParameterListSyntax? parameterList;

        private BlockSyntax? block;

        private ExpressionSyntax? expressionBody;

        public override SyntaxToken AsyncKeyword => Modifiers.FirstOrDefault(SyntaxKind.AsyncKeyword);

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public override SyntaxTokenList Modifiers
        {
            get
            {
                GreenNode slot = base.Green.GetSlot(1);
                if (slot == null)
                {
                    return default(SyntaxTokenList);
                }
                return new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1));
            }
        }

        public ParameterListSyntax ParameterList => GetRed(ref parameterList, 2);

        public override SyntaxToken ArrowToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ParenthesizedLambdaExpressionSyntax)base.Green).arrowToken, GetChildPosition(3), GetChildIndex(3));

        public override BlockSyntax? Block => GetRed(ref block, 4);

        public override ExpressionSyntax? ExpressionBody => GetRed(ref expressionBody, 5);

        public new ParenthesizedLambdaExpressionSyntax WithBody(CSharpSyntaxNode body)
        {
            if (!(body is BlockSyntax blockSyntax))
            {
                return WithExpressionBody((ExpressionSyntax)body).WithBlock(null);
            }
            return WithBlock(blockSyntax).WithExpressionBody(null);
        }

        public ParenthesizedLambdaExpressionSyntax Update(SyntaxToken asyncKeyword, ParameterListSyntax parameterList, SyntaxToken arrowToken, CSharpSyntaxNode body)
        {
            if (!(body is BlockSyntax blockSyntax))
            {
                return Update(asyncKeyword, parameterList, arrowToken, null, (ExpressionSyntax)body);
            }
            return Update(asyncKeyword, parameterList, arrowToken, blockSyntax, null);
        }

        internal override AnonymousFunctionExpressionSyntax WithAsyncKeywordCore(SyntaxToken asyncKeyword)
        {
            return WithAsyncKeyword(asyncKeyword);
        }

        public new ParenthesizedLambdaExpressionSyntax WithAsyncKeyword(SyntaxToken asyncKeyword)
        {
            return Update(asyncKeyword, ParameterList, ArrowToken, Block, ExpressionBody);
        }

        public ParenthesizedLambdaExpressionSyntax Update(SyntaxToken asyncKeyword, ParameterListSyntax parameterList, SyntaxToken arrowToken, BlockSyntax? block, ExpressionSyntax? expressionBody)
        {
            return Update(SyntaxFactory.TokenList(asyncKeyword), parameterList, arrowToken, block, expressionBody);
        }

        public ParenthesizedLambdaExpressionSyntax Update(SyntaxTokenList modifiers, ParameterListSyntax parameterList, SyntaxToken arrowToken, BlockSyntax? block, ExpressionSyntax? expressionBody)
        {
            return Update(AttributeLists, modifiers, parameterList, arrowToken, block, expressionBody);
        }

        internal ParenthesizedLambdaExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                2 => GetRed(ref parameterList, 2),
                4 => GetRed(ref block, 4),
                5 => GetRed(ref expressionBody, 5),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                2 => parameterList,
                4 => block,
                5 => expressionBody,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitParenthesizedLambdaExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitParenthesizedLambdaExpression(this);

        public ParenthesizedLambdaExpressionSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, ParameterListSyntax parameterList, SyntaxToken arrowToken, BlockSyntax? block, ExpressionSyntax? expressionBody)
        {
            if (attributeLists != AttributeLists || modifiers != Modifiers || parameterList != ParameterList || arrowToken != ArrowToken || block != Block || expressionBody != ExpressionBody)
            {
                ParenthesizedLambdaExpressionSyntax parenthesizedLambdaExpressionSyntax = SyntaxFactory.ParenthesizedLambdaExpression(attributeLists, modifiers, parameterList, arrowToken, block, expressionBody);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return parenthesizedLambdaExpressionSyntax;
                }
                return parenthesizedLambdaExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override LambdaExpressionSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new ParenthesizedLambdaExpressionSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, Modifiers, ParameterList, ArrowToken, Block, ExpressionBody);
        }

        internal override AnonymousFunctionExpressionSyntax WithModifiersCore(SyntaxTokenList modifiers)
        {
            return WithModifiers(modifiers);
        }

        public new ParenthesizedLambdaExpressionSyntax WithModifiers(SyntaxTokenList modifiers)
        {
            return Update(AttributeLists, modifiers, ParameterList, ArrowToken, Block, ExpressionBody);
        }

        public ParenthesizedLambdaExpressionSyntax WithParameterList(ParameterListSyntax parameterList)
        {
            return Update(AttributeLists, Modifiers, parameterList, ArrowToken, Block, ExpressionBody);
        }

        internal override LambdaExpressionSyntax WithArrowTokenCore(SyntaxToken arrowToken)
        {
            return WithArrowToken(arrowToken);
        }

        public new ParenthesizedLambdaExpressionSyntax WithArrowToken(SyntaxToken arrowToken)
        {
            return Update(AttributeLists, Modifiers, ParameterList, arrowToken, Block, ExpressionBody);
        }

        internal override AnonymousFunctionExpressionSyntax WithBlockCore(BlockSyntax? block)
        {
            return WithBlock(block);
        }

        public new ParenthesizedLambdaExpressionSyntax WithBlock(BlockSyntax? block)
        {
            return Update(AttributeLists, Modifiers, ParameterList, ArrowToken, block, ExpressionBody);
        }

        internal override AnonymousFunctionExpressionSyntax WithExpressionBodyCore(ExpressionSyntax? expressionBody)
        {
            return WithExpressionBody(expressionBody);
        }

        public new ParenthesizedLambdaExpressionSyntax WithExpressionBody(ExpressionSyntax? expressionBody)
        {
            return Update(AttributeLists, Modifiers, ParameterList, ArrowToken, Block, expressionBody);
        }

        internal override LambdaExpressionSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new ParenthesizedLambdaExpressionSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }

        internal override AnonymousFunctionExpressionSyntax AddModifiersCore(params SyntaxToken[] items)
        {
            return AddModifiers(items);
        }

        public new ParenthesizedLambdaExpressionSyntax AddModifiers(params SyntaxToken[] items)
        {
            return WithModifiers(Modifiers.AddRange(items));
        }

        public ParenthesizedLambdaExpressionSyntax AddParameterListParameters(params ParameterSyntax[] items)
        {
            return WithParameterList(ParameterList.WithParameters(ParameterList.Parameters.AddRange(items)));
        }

        internal override AnonymousFunctionExpressionSyntax AddBlockAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddBlockAttributeLists(items);
        }

        public new ParenthesizedLambdaExpressionSyntax AddBlockAttributeLists(params AttributeListSyntax[] items)
        {
            BlockSyntax blockSyntax = Block ?? SyntaxFactory.Block();
            return WithBlock(blockSyntax.WithAttributeLists(blockSyntax.AttributeLists.AddRange(items)));
        }

        internal override AnonymousFunctionExpressionSyntax AddBlockStatementsCore(params StatementSyntax[] items)
        {
            return AddBlockStatements(items);
        }

        public new ParenthesizedLambdaExpressionSyntax AddBlockStatements(params StatementSyntax[] items)
        {
            BlockSyntax blockSyntax = Block ?? SyntaxFactory.Block();
            return WithBlock(blockSyntax.WithStatements(blockSyntax.Statements.AddRange(items)));
        }
    }
}
