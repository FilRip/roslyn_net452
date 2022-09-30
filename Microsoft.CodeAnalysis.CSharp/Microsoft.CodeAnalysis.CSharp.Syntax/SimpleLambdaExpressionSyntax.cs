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

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class SimpleLambdaExpressionSyntax : LambdaExpressionSyntax
    {
        private SyntaxNode? attributeLists;

        private ParameterSyntax? parameter;

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

        public ParameterSyntax Parameter => GetRed(ref parameter, 2);

        public override SyntaxToken ArrowToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SimpleLambdaExpressionSyntax)base.Green).arrowToken, GetChildPosition(3), GetChildIndex(3));

        public override BlockSyntax? Block => GetRed(ref block, 4);

        public override ExpressionSyntax? ExpressionBody => GetRed(ref expressionBody, 5);

        public new SimpleLambdaExpressionSyntax WithBody(CSharpSyntaxNode body)
        {
            if (!(body is BlockSyntax blockSyntax))
            {
                return WithExpressionBody((ExpressionSyntax)body).WithBlock(null);
            }
            return WithBlock(blockSyntax).WithExpressionBody(null);
        }

        public SimpleLambdaExpressionSyntax Update(SyntaxToken asyncKeyword, ParameterSyntax parameter, SyntaxToken arrowToken, CSharpSyntaxNode body)
        {
            if (!(body is BlockSyntax blockSyntax))
            {
                return Update(asyncKeyword, parameter, arrowToken, null, (ExpressionSyntax)body);
            }
            return Update(asyncKeyword, parameter, arrowToken, blockSyntax, null);
        }

        internal override AnonymousFunctionExpressionSyntax WithAsyncKeywordCore(SyntaxToken asyncKeyword)
        {
            return WithAsyncKeyword(asyncKeyword);
        }

        public new SimpleLambdaExpressionSyntax WithAsyncKeyword(SyntaxToken asyncKeyword)
        {
            return Update(asyncKeyword, Parameter, ArrowToken, Block, ExpressionBody);
        }

        public SimpleLambdaExpressionSyntax Update(SyntaxToken asyncKeyword, ParameterSyntax parameter, SyntaxToken arrowToken, BlockSyntax? block, ExpressionSyntax? expressionBody)
        {
            return Update(SyntaxFactory.TokenList(asyncKeyword), parameter, arrowToken, block, expressionBody);
        }

        public SimpleLambdaExpressionSyntax Update(SyntaxTokenList modifiers, ParameterSyntax parameter, SyntaxToken arrowToken, BlockSyntax? block, ExpressionSyntax? expressionBody)
        {
            return Update(AttributeLists, modifiers, parameter, arrowToken, block, expressionBody);
        }

        internal SimpleLambdaExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                2 => GetRed(ref parameter, 2),
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
                2 => parameter,
                4 => block,
                5 => expressionBody,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitSimpleLambdaExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitSimpleLambdaExpression(this);

        public SimpleLambdaExpressionSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, ParameterSyntax parameter, SyntaxToken arrowToken, BlockSyntax? block, ExpressionSyntax? expressionBody)
        {
            if (attributeLists != AttributeLists || modifiers != Modifiers || parameter != Parameter || arrowToken != ArrowToken || block != Block || expressionBody != ExpressionBody)
            {
                SimpleLambdaExpressionSyntax simpleLambdaExpressionSyntax = SyntaxFactory.SimpleLambdaExpression(attributeLists, modifiers, parameter, arrowToken, block, expressionBody);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return simpleLambdaExpressionSyntax;
                }
                return simpleLambdaExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override LambdaExpressionSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new SimpleLambdaExpressionSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, Modifiers, Parameter, ArrowToken, Block, ExpressionBody);
        }

        internal override AnonymousFunctionExpressionSyntax WithModifiersCore(SyntaxTokenList modifiers)
        {
            return WithModifiers(modifiers);
        }

        public new SimpleLambdaExpressionSyntax WithModifiers(SyntaxTokenList modifiers)
        {
            return Update(AttributeLists, modifiers, Parameter, ArrowToken, Block, ExpressionBody);
        }

        public SimpleLambdaExpressionSyntax WithParameter(ParameterSyntax parameter)
        {
            return Update(AttributeLists, Modifiers, parameter, ArrowToken, Block, ExpressionBody);
        }

        internal override LambdaExpressionSyntax WithArrowTokenCore(SyntaxToken arrowToken)
        {
            return WithArrowToken(arrowToken);
        }

        public new SimpleLambdaExpressionSyntax WithArrowToken(SyntaxToken arrowToken)
        {
            return Update(AttributeLists, Modifiers, Parameter, arrowToken, Block, ExpressionBody);
        }

        internal override AnonymousFunctionExpressionSyntax WithBlockCore(BlockSyntax? block)
        {
            return WithBlock(block);
        }

        public new SimpleLambdaExpressionSyntax WithBlock(BlockSyntax? block)
        {
            return Update(AttributeLists, Modifiers, Parameter, ArrowToken, block, ExpressionBody);
        }

        internal override AnonymousFunctionExpressionSyntax WithExpressionBodyCore(ExpressionSyntax? expressionBody)
        {
            return WithExpressionBody(expressionBody);
        }

        public new SimpleLambdaExpressionSyntax WithExpressionBody(ExpressionSyntax? expressionBody)
        {
            return Update(AttributeLists, Modifiers, Parameter, ArrowToken, Block, expressionBody);
        }

        internal override LambdaExpressionSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new SimpleLambdaExpressionSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }

        internal override AnonymousFunctionExpressionSyntax AddModifiersCore(params SyntaxToken[] items)
        {
            return AddModifiers(items);
        }

        public new SimpleLambdaExpressionSyntax AddModifiers(params SyntaxToken[] items)
        {
            return WithModifiers(Modifiers.AddRange(items));
        }

        public SimpleLambdaExpressionSyntax AddParameterAttributeLists(params AttributeListSyntax[] items)
        {
            return WithParameter(Parameter.WithAttributeLists(Parameter.AttributeLists.AddRange(items)));
        }

        public SimpleLambdaExpressionSyntax AddParameterModifiers(params SyntaxToken[] items)
        {
            return WithParameter(Parameter.WithModifiers(Parameter.Modifiers.AddRange(items)));
        }

        internal override AnonymousFunctionExpressionSyntax AddBlockAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddBlockAttributeLists(items);
        }

        public new SimpleLambdaExpressionSyntax AddBlockAttributeLists(params AttributeListSyntax[] items)
        {
            BlockSyntax blockSyntax = Block ?? SyntaxFactory.Block();
            return WithBlock(blockSyntax.WithAttributeLists(blockSyntax.AttributeLists.AddRange(items)));
        }

        internal override AnonymousFunctionExpressionSyntax AddBlockStatementsCore(params StatementSyntax[] items)
        {
            return AddBlockStatements(items);
        }

        public new SimpleLambdaExpressionSyntax AddBlockStatements(params StatementSyntax[] items)
        {
            BlockSyntax blockSyntax = Block ?? SyntaxFactory.Block();
            return WithBlock(blockSyntax.WithStatements(blockSyntax.Statements.AddRange(items)));
        }
    }
}
