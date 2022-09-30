#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class UnsafeStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;

        private BlockSyntax? block;

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public SyntaxToken UnsafeKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.UnsafeStatementSyntax)base.Green).unsafeKeyword, GetChildPosition(1), GetChildIndex(1));

        public BlockSyntax Block => GetRed(ref block, 2);

        public UnsafeStatementSyntax Update(SyntaxToken unsafeKeyword, BlockSyntax block)
        {
            return Update(AttributeLists, unsafeKeyword, block);
        }

        internal UnsafeStatementSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitUnsafeStatement(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitUnsafeStatement(this);

        public UnsafeStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken unsafeKeyword, BlockSyntax block)
        {
            if (attributeLists != AttributeLists || unsafeKeyword != UnsafeKeyword || block != Block)
            {
                UnsafeStatementSyntax unsafeStatementSyntax = SyntaxFactory.UnsafeStatement(attributeLists, unsafeKeyword, block);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return unsafeStatementSyntax;
                }
                return unsafeStatementSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new UnsafeStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, UnsafeKeyword, Block);
        }

        public UnsafeStatementSyntax WithUnsafeKeyword(SyntaxToken unsafeKeyword)
        {
            return Update(AttributeLists, unsafeKeyword, Block);
        }

        public UnsafeStatementSyntax WithBlock(BlockSyntax block)
        {
            return Update(AttributeLists, UnsafeKeyword, block);
        }

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new UnsafeStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }

        public UnsafeStatementSyntax AddBlockAttributeLists(params AttributeListSyntax[] items)
        {
            return WithBlock(Block.WithAttributeLists(Block.AttributeLists.AddRange(items)));
        }

        public UnsafeStatementSyntax AddBlockStatements(params StatementSyntax[] items)
        {
            return WithBlock(Block.WithStatements(Block.Statements.AddRange(items)));
        }
    }
}
