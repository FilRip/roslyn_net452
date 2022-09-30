#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class CheckedStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;

        private BlockSyntax? block;

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public SyntaxToken Keyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CheckedStatementSyntax)base.Green).keyword, GetChildPosition(1), GetChildIndex(1));

        public BlockSyntax Block => GetRed(ref block, 2);

        public CheckedStatementSyntax Update(SyntaxToken keyword, BlockSyntax block)
        {
            return Update(AttributeLists, keyword, block);
        }

        internal CheckedStatementSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                2 => GetRed(ref block, 2),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                2 => block,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitCheckedStatement(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitCheckedStatement(this);

        public CheckedStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken keyword, BlockSyntax block)
        {
            if (attributeLists != AttributeLists || keyword != Keyword || block != Block)
            {
                CheckedStatementSyntax checkedStatementSyntax = SyntaxFactory.CheckedStatement(Kind(), attributeLists, keyword, block);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return checkedStatementSyntax;
                }
                return checkedStatementSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new CheckedStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, Keyword, Block);
        }

        public CheckedStatementSyntax WithKeyword(SyntaxToken keyword)
        {
            return Update(AttributeLists, keyword, Block);
        }

        public CheckedStatementSyntax WithBlock(BlockSyntax block)
        {
            return Update(AttributeLists, Keyword, block);
        }

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new CheckedStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }

        public CheckedStatementSyntax AddBlockAttributeLists(params AttributeListSyntax[] items)
        {
            return WithBlock(Block.WithAttributeLists(Block.AttributeLists.AddRange(items)));
        }

        public CheckedStatementSyntax AddBlockStatements(params StatementSyntax[] items)
        {
            return WithBlock(Block.WithStatements(Block.Statements.AddRange(items)));
        }
    }
}
