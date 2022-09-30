#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class BlockSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;

        private SyntaxNode? statements;

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public SyntaxToken OpenBraceToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BlockSyntax)base.Green).openBraceToken, GetChildPosition(1), GetChildIndex(1));

        public SyntaxList<StatementSyntax> Statements => new SyntaxList<StatementSyntax>(GetRed(ref statements, 2));

        public SyntaxToken CloseBraceToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BlockSyntax)base.Green).closeBraceToken, GetChildPosition(3), GetChildIndex(3));

        public BlockSyntax Update(SyntaxToken openBraceToken, SyntaxList<StatementSyntax> statements, SyntaxToken closeBraceToken)
        {
            return Update(AttributeLists, openBraceToken, statements, closeBraceToken);
        }

        internal BlockSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                2 => GetRed(ref statements, 2),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                2 => statements,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitBlock(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitBlock(this);

        public BlockSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken openBraceToken, SyntaxList<StatementSyntax> statements, SyntaxToken closeBraceToken)
        {
            if (attributeLists != AttributeLists || openBraceToken != OpenBraceToken || statements != Statements || closeBraceToken != CloseBraceToken)
            {
                BlockSyntax blockSyntax = SyntaxFactory.Block(attributeLists, openBraceToken, statements, closeBraceToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return blockSyntax;
                }
                return blockSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new BlockSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, OpenBraceToken, Statements, CloseBraceToken);
        }

        public BlockSyntax WithOpenBraceToken(SyntaxToken openBraceToken)
        {
            return Update(AttributeLists, openBraceToken, Statements, CloseBraceToken);
        }

        public BlockSyntax WithStatements(SyntaxList<StatementSyntax> statements)
        {
            return Update(AttributeLists, OpenBraceToken, statements, CloseBraceToken);
        }

        public BlockSyntax WithCloseBraceToken(SyntaxToken closeBraceToken)
        {
            return Update(AttributeLists, OpenBraceToken, Statements, closeBraceToken);
        }

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new BlockSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }

        public BlockSyntax AddStatements(params StatementSyntax[] items)
        {
            return WithStatements(Statements.AddRange(items));
        }
    }
}
