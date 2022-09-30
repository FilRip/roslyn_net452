#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class CatchClauseSyntax : CSharpSyntaxNode
    {
        private CatchDeclarationSyntax? declaration;

        private CatchFilterClauseSyntax? filter;

        private BlockSyntax? block;

        public SyntaxToken CatchKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CatchClauseSyntax)base.Green).catchKeyword, base.Position, 0);

        public CatchDeclarationSyntax? Declaration => GetRed(ref declaration, 1);

        public CatchFilterClauseSyntax? Filter => GetRed(ref filter, 2);

        public BlockSyntax Block => GetRed(ref block, 3);

        internal CatchClauseSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                1 => GetRed(ref declaration, 1),
                2 => GetRed(ref filter, 2),
                3 => GetRed(ref block, 3),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                1 => declaration,
                2 => filter,
                3 => block,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitCatchClause(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitCatchClause(this);

        public CatchClauseSyntax Update(SyntaxToken catchKeyword, CatchDeclarationSyntax? declaration, CatchFilterClauseSyntax? filter, BlockSyntax block)
        {
            if (catchKeyword != CatchKeyword || declaration != Declaration || filter != Filter || block != Block)
            {
                CatchClauseSyntax catchClauseSyntax = SyntaxFactory.CatchClause(catchKeyword, declaration, filter, block);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return catchClauseSyntax;
                }
                return catchClauseSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public CatchClauseSyntax WithCatchKeyword(SyntaxToken catchKeyword)
        {
            return Update(catchKeyword, Declaration, Filter, Block);
        }

        public CatchClauseSyntax WithDeclaration(CatchDeclarationSyntax? declaration)
        {
            return Update(CatchKeyword, declaration, Filter, Block);
        }

        public CatchClauseSyntax WithFilter(CatchFilterClauseSyntax? filter)
        {
            return Update(CatchKeyword, Declaration, filter, Block);
        }

        public CatchClauseSyntax WithBlock(BlockSyntax block)
        {
            return Update(CatchKeyword, Declaration, Filter, block);
        }

        public CatchClauseSyntax AddBlockAttributeLists(params AttributeListSyntax[] items)
        {
            return WithBlock(Block.WithAttributeLists(Block.AttributeLists.AddRange(items)));
        }

        public CatchClauseSyntax AddBlockStatements(params StatementSyntax[] items)
        {
            return WithBlock(Block.WithStatements(Block.Statements.AddRange(items)));
        }
    }
}
