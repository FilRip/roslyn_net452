#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class FinallyClauseSyntax : CSharpSyntaxNode
    {
        private BlockSyntax? block;

        public SyntaxToken FinallyKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.FinallyClauseSyntax)base.Green).finallyKeyword, base.Position, 0);

        public BlockSyntax Block => GetRed(ref block, 1);

        internal FinallyClauseSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return GetRed(ref block, 1);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return block;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitFinallyClause(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitFinallyClause(this);

        public FinallyClauseSyntax Update(SyntaxToken finallyKeyword, BlockSyntax block)
        {
            if (finallyKeyword != FinallyKeyword || block != Block)
            {
                FinallyClauseSyntax finallyClauseSyntax = SyntaxFactory.FinallyClause(finallyKeyword, block);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return finallyClauseSyntax;
                }
                return finallyClauseSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public FinallyClauseSyntax WithFinallyKeyword(SyntaxToken finallyKeyword)
        {
            return Update(finallyKeyword, Block);
        }

        public FinallyClauseSyntax WithBlock(BlockSyntax block)
        {
            return Update(FinallyKeyword, block);
        }

        public FinallyClauseSyntax AddBlockAttributeLists(params AttributeListSyntax[] items)
        {
            return WithBlock(Block.WithAttributeLists(Block.AttributeLists.AddRange(items)));
        }

        public FinallyClauseSyntax AddBlockStatements(params StatementSyntax[] items)
        {
            return WithBlock(Block.WithStatements(Block.Statements.AddRange(items)));
        }
    }
}
