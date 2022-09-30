#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public abstract class LambdaExpressionSyntax : AnonymousFunctionExpressionSyntax
    {
        public abstract SyntaxList<AttributeListSyntax> AttributeLists { get; }

        public abstract SyntaxToken ArrowToken { get; }

        public new LambdaExpressionSyntax WithBody(CSharpSyntaxNode body)
        {
            if (!(body is BlockSyntax block))
            {
                return WithExpressionBody((ExpressionSyntax)body).WithBlock(null);
            }
            return WithBlock(block).WithExpressionBody(null);
        }

        public new LambdaExpressionSyntax WithAsyncKeyword(SyntaxToken asyncKeyword)
        {
            return (LambdaExpressionSyntax)WithAsyncKeywordCore(asyncKeyword);
        }

        internal LambdaExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public LambdaExpressionSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeListsCore(attributeLists);
        }

        internal abstract LambdaExpressionSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists);

        public LambdaExpressionSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return AddAttributeListsCore(items);
        }

        internal abstract LambdaExpressionSyntax AddAttributeListsCore(params AttributeListSyntax[] items);

        public LambdaExpressionSyntax WithArrowToken(SyntaxToken arrowToken)
        {
            return WithArrowTokenCore(arrowToken);
        }

        internal abstract LambdaExpressionSyntax WithArrowTokenCore(SyntaxToken arrowToken);

        public new LambdaExpressionSyntax WithModifiers(SyntaxTokenList modifiers)
        {
            return (LambdaExpressionSyntax)WithModifiersCore(modifiers);
        }

        public new LambdaExpressionSyntax WithBlock(BlockSyntax? block)
        {
            return (LambdaExpressionSyntax)WithBlockCore(block);
        }

        public new LambdaExpressionSyntax WithExpressionBody(ExpressionSyntax? expressionBody)
        {
            return (LambdaExpressionSyntax)WithExpressionBodyCore(expressionBody);
        }

        public new LambdaExpressionSyntax AddModifiers(params SyntaxToken[] items)
        {
            return (LambdaExpressionSyntax)AddModifiersCore(items);
        }

        public new AnonymousFunctionExpressionSyntax AddBlockAttributeLists(params AttributeListSyntax[] items)
        {
            return AddBlockAttributeListsCore(items);
        }

        public new AnonymousFunctionExpressionSyntax AddBlockStatements(params StatementSyntax[] items)
        {
            return AddBlockStatementsCore(items);
        }
    }
}
