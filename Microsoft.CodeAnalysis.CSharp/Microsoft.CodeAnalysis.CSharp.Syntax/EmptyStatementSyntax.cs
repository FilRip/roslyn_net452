#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class EmptyStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public SyntaxToken SemicolonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.EmptyStatementSyntax)base.Green).semicolonToken, GetChildPosition(1), GetChildIndex(1));

        public EmptyStatementSyntax Update(SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, semicolonToken);
        }

        internal EmptyStatementSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return GetRedAtZero(ref attributeLists);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return attributeLists;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitEmptyStatement(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitEmptyStatement(this);

        public EmptyStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || semicolonToken != SemicolonToken)
            {
                EmptyStatementSyntax emptyStatementSyntax = SyntaxFactory.EmptyStatement(attributeLists, semicolonToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return emptyStatementSyntax;
                }
                return emptyStatementSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new EmptyStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, SemicolonToken);
        }

        public EmptyStatementSyntax WithSemicolonToken(SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, semicolonToken);
        }

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new EmptyStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }
    }
}
