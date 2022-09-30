using System;

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
    public sealed class AnonymousMethodExpressionSyntax : AnonymousFunctionExpressionSyntax
    {
        private ParameterListSyntax? parameterList;

        private BlockSyntax? block;

        private ExpressionSyntax? expressionBody;

        public override SyntaxToken AsyncKeyword => Modifiers.FirstOrDefault(SyntaxKind.AsyncKeyword);

        public override SyntaxTokenList Modifiers
        {
            get
            {
                GreenNode slot = base.Green.GetSlot(0);
                if (slot == null)
                {
                    return default(SyntaxTokenList);
                }
                return new SyntaxTokenList(this, slot, base.Position, 0);
            }
        }

        public SyntaxToken DelegateKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AnonymousMethodExpressionSyntax)base.Green).delegateKeyword, GetChildPosition(1), GetChildIndex(1));

        public ParameterListSyntax? ParameterList => GetRed(ref parameterList, 2);

        public override BlockSyntax Block => GetRed(ref block, 3);

        public override ExpressionSyntax? ExpressionBody => GetRed(ref expressionBody, 4);

        public new AnonymousMethodExpressionSyntax WithBody(CSharpSyntaxNode body)
        {
            if (!(body is BlockSyntax blockSyntax))
            {
                return WithExpressionBody((ExpressionSyntax)body).WithBlock(null);
            }
            return WithBlock(blockSyntax).WithExpressionBody(null);
        }

        public AnonymousMethodExpressionSyntax Update(SyntaxToken asyncKeyword, SyntaxToken delegateKeyword, ParameterListSyntax parameterList, CSharpSyntaxNode body)
        {
            if (!(body is BlockSyntax blockSyntax))
            {
                return Update(asyncKeyword, delegateKeyword, parameterList, null, (ExpressionSyntax)body);
            }
            return Update(asyncKeyword, delegateKeyword, parameterList, blockSyntax, null);
        }

        internal override AnonymousFunctionExpressionSyntax WithAsyncKeywordCore(SyntaxToken asyncKeyword)
        {
            return WithAsyncKeyword(asyncKeyword);
        }

        public new AnonymousMethodExpressionSyntax WithAsyncKeyword(SyntaxToken asyncKeyword)
        {
            return Update(asyncKeyword, DelegateKeyword, ParameterList, Block, ExpressionBody);
        }

        public AnonymousMethodExpressionSyntax Update(SyntaxToken asyncKeyword, SyntaxToken delegateKeyword, ParameterListSyntax parameterList, BlockSyntax block, ExpressionSyntax expressionBody)
        {
            return Update(SyntaxFactory.TokenList(asyncKeyword), delegateKeyword, parameterList, block, expressionBody);
        }

        internal AnonymousMethodExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                2 => GetRed(ref parameterList, 2),
                3 => GetRed(ref block, 3),
                4 => GetRed(ref expressionBody, 4),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                2 => parameterList,
                3 => block,
                4 => expressionBody,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitAnonymousMethodExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitAnonymousMethodExpression(this);

        public AnonymousMethodExpressionSyntax Update(SyntaxTokenList modifiers, SyntaxToken delegateKeyword, ParameterListSyntax? parameterList, BlockSyntax block, ExpressionSyntax? expressionBody)
        {
            if (modifiers != Modifiers || delegateKeyword != DelegateKeyword || parameterList != ParameterList || block != Block || expressionBody != ExpressionBody)
            {
                AnonymousMethodExpressionSyntax anonymousMethodExpressionSyntax = SyntaxFactory.AnonymousMethodExpression(modifiers, delegateKeyword, parameterList, block, expressionBody);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return anonymousMethodExpressionSyntax;
                }
                return anonymousMethodExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override AnonymousFunctionExpressionSyntax WithModifiersCore(SyntaxTokenList modifiers)
        {
            return WithModifiers(modifiers);
        }

        public new AnonymousMethodExpressionSyntax WithModifiers(SyntaxTokenList modifiers)
        {
            return Update(modifiers, DelegateKeyword, ParameterList, Block, ExpressionBody);
        }

        public AnonymousMethodExpressionSyntax WithDelegateKeyword(SyntaxToken delegateKeyword)
        {
            return Update(Modifiers, delegateKeyword, ParameterList, Block, ExpressionBody);
        }

        public AnonymousMethodExpressionSyntax WithParameterList(ParameterListSyntax? parameterList)
        {
            return Update(Modifiers, DelegateKeyword, parameterList, Block, ExpressionBody);
        }

        internal override AnonymousFunctionExpressionSyntax WithBlockCore(BlockSyntax? block)
        {
            return WithBlock(block ?? throw new ArgumentNullException("block"));
        }

        public new AnonymousMethodExpressionSyntax WithBlock(BlockSyntax block)
        {
            return Update(Modifiers, DelegateKeyword, ParameterList, block, ExpressionBody);
        }

        internal override AnonymousFunctionExpressionSyntax WithExpressionBodyCore(ExpressionSyntax? expressionBody)
        {
            return WithExpressionBody(expressionBody);
        }

        public new AnonymousMethodExpressionSyntax WithExpressionBody(ExpressionSyntax? expressionBody)
        {
            return Update(Modifiers, DelegateKeyword, ParameterList, Block, expressionBody);
        }

        internal override AnonymousFunctionExpressionSyntax AddModifiersCore(params SyntaxToken[] items)
        {
            return AddModifiers(items);
        }

        public new AnonymousMethodExpressionSyntax AddModifiers(params SyntaxToken[] items)
        {
            return WithModifiers(Modifiers.AddRange(items));
        }

        public AnonymousMethodExpressionSyntax AddParameterListParameters(params ParameterSyntax[] items)
        {
            ParameterListSyntax parameterListSyntax = ParameterList ?? SyntaxFactory.ParameterList();
            return WithParameterList(parameterListSyntax.WithParameters(parameterListSyntax.Parameters.AddRange(items)));
        }

        internal override AnonymousFunctionExpressionSyntax AddBlockAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddBlockAttributeLists(items);
        }

        public new AnonymousMethodExpressionSyntax AddBlockAttributeLists(params AttributeListSyntax[] items)
        {
            return WithBlock(Block.WithAttributeLists(Block.AttributeLists.AddRange(items)));
        }

        internal override AnonymousFunctionExpressionSyntax AddBlockStatementsCore(params StatementSyntax[] items)
        {
            return AddBlockStatements(items);
        }

        public new AnonymousMethodExpressionSyntax AddBlockStatements(params StatementSyntax[] items)
        {
            return WithBlock(Block.WithStatements(Block.Statements.AddRange(items)));
        }
    }
}
