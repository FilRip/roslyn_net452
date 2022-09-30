#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class TryStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;

        private BlockSyntax? block;

        private SyntaxNode? catches;

        private FinallyClauseSyntax? @finally;

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public SyntaxToken TryKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TryStatementSyntax)base.Green).tryKeyword, GetChildPosition(1), GetChildIndex(1));

        public BlockSyntax Block => GetRed(ref block, 2);

        public SyntaxList<CatchClauseSyntax> Catches => new SyntaxList<CatchClauseSyntax>(GetRed(ref catches, 3));

        public FinallyClauseSyntax? Finally => GetRed(ref @finally, 4);

        public TryStatementSyntax Update(SyntaxToken tryKeyword, BlockSyntax block, SyntaxList<CatchClauseSyntax> catches, FinallyClauseSyntax @finally)
        {
            return Update(AttributeLists, tryKeyword, block, catches, @finally);
        }

        internal TryStatementSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                2 => GetRed(ref block, 2),
                3 => GetRed(ref catches, 3),
                4 => GetRed(ref @finally, 4),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                2 => block,
                3 => catches,
                4 => @finally,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitTryStatement(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitTryStatement(this);

        public TryStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken tryKeyword, BlockSyntax block, SyntaxList<CatchClauseSyntax> catches, FinallyClauseSyntax? @finally)
        {
            if (attributeLists != AttributeLists || tryKeyword != TryKeyword || block != Block || catches != Catches || @finally != Finally)
            {
                TryStatementSyntax tryStatementSyntax = SyntaxFactory.TryStatement(attributeLists, tryKeyword, block, catches, @finally);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return tryStatementSyntax;
                }
                return tryStatementSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new TryStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, TryKeyword, Block, Catches, Finally);
        }

        public TryStatementSyntax WithTryKeyword(SyntaxToken tryKeyword)
        {
            return Update(AttributeLists, tryKeyword, Block, Catches, Finally);
        }

        public TryStatementSyntax WithBlock(BlockSyntax block)
        {
            return Update(AttributeLists, TryKeyword, block, Catches, Finally);
        }

        public TryStatementSyntax WithCatches(SyntaxList<CatchClauseSyntax> catches)
        {
            return Update(AttributeLists, TryKeyword, Block, catches, Finally);
        }

        public TryStatementSyntax WithFinally(FinallyClauseSyntax? @finally)
        {
            return Update(AttributeLists, TryKeyword, Block, Catches, @finally);
        }

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new TryStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }

        public TryStatementSyntax AddBlockAttributeLists(params AttributeListSyntax[] items)
        {
            return WithBlock(Block.WithAttributeLists(Block.AttributeLists.AddRange(items)));
        }

        public TryStatementSyntax AddBlockStatements(params StatementSyntax[] items)
        {
            return WithBlock(Block.WithStatements(Block.Statements.AddRange(items)));
        }

        public TryStatementSyntax AddCatches(params CatchClauseSyntax[] items)
        {
            return WithCatches(Catches.AddRange(items));
        }
    }
}
