#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public abstract class AnonymousFunctionExpressionSyntax : ExpressionSyntax
    {
        public CSharpSyntaxNode Body => (CSharpSyntaxNode)(Block ?? ((object)ExpressionBody));

        public abstract SyntaxToken AsyncKeyword { get; }

        public abstract SyntaxTokenList Modifiers { get; }

        public abstract BlockSyntax? Block { get; }

        public abstract ExpressionSyntax? ExpressionBody { get; }

        public AnonymousFunctionExpressionSyntax WithBody(CSharpSyntaxNode body)
        {
            if (!(body is BlockSyntax block))
            {
                return WithExpressionBody((ExpressionSyntax)body).WithBlock(null);
            }
            return WithBlock(block).WithExpressionBody(null);
        }

        public AnonymousFunctionExpressionSyntax WithAsyncKeyword(SyntaxToken asyncKeyword)
        {
            return WithAsyncKeywordCore(asyncKeyword);
        }

        internal abstract AnonymousFunctionExpressionSyntax WithAsyncKeywordCore(SyntaxToken asyncKeyword);

        internal AnonymousFunctionExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public AnonymousFunctionExpressionSyntax WithModifiers(SyntaxTokenList modifiers)
        {
            return WithModifiersCore(modifiers);
        }

        internal abstract AnonymousFunctionExpressionSyntax WithModifiersCore(SyntaxTokenList modifiers);

        public AnonymousFunctionExpressionSyntax AddModifiers(params SyntaxToken[] items)
        {
            return AddModifiersCore(items);
        }

        internal abstract AnonymousFunctionExpressionSyntax AddModifiersCore(params SyntaxToken[] items);

        public AnonymousFunctionExpressionSyntax WithBlock(BlockSyntax? block)
        {
            return WithBlockCore(block);
        }

        internal abstract AnonymousFunctionExpressionSyntax WithBlockCore(BlockSyntax? block);

        public AnonymousFunctionExpressionSyntax AddBlockAttributeLists(params AttributeListSyntax[] items)
        {
            return AddBlockAttributeListsCore(items);
        }

        internal abstract AnonymousFunctionExpressionSyntax AddBlockAttributeListsCore(params AttributeListSyntax[] items);

        public AnonymousFunctionExpressionSyntax AddBlockStatements(params StatementSyntax[] items)
        {
            return AddBlockStatementsCore(items);
        }

        internal abstract AnonymousFunctionExpressionSyntax AddBlockStatementsCore(params StatementSyntax[] items);

        public AnonymousFunctionExpressionSyntax WithExpressionBody(ExpressionSyntax? expressionBody)
        {
            return WithExpressionBodyCore(expressionBody);
        }

        internal abstract AnonymousFunctionExpressionSyntax WithExpressionBodyCore(ExpressionSyntax? expressionBody);
    }
}
