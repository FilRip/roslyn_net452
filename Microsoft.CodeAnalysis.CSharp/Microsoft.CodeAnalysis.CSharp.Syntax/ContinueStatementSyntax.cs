#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ContinueStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public SyntaxToken ContinueKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ContinueStatementSyntax)base.Green).continueKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken SemicolonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ContinueStatementSyntax)base.Green).semicolonToken, GetChildPosition(2), GetChildIndex(2));

        public ContinueStatementSyntax Update(SyntaxToken continueKeyword, SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, continueKeyword, semicolonToken);
        }

        internal ContinueStatementSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitContinueStatement(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitContinueStatement(this);

        public ContinueStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken continueKeyword, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || continueKeyword != ContinueKeyword || semicolonToken != SemicolonToken)
            {
                ContinueStatementSyntax continueStatementSyntax = SyntaxFactory.ContinueStatement(attributeLists, continueKeyword, semicolonToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return continueStatementSyntax;
                }
                return continueStatementSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new ContinueStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, ContinueKeyword, SemicolonToken);
        }

        public ContinueStatementSyntax WithContinueKeyword(SyntaxToken continueKeyword)
        {
            return Update(AttributeLists, continueKeyword, SemicolonToken);
        }

        public ContinueStatementSyntax WithSemicolonToken(SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, ContinueKeyword, semicolonToken);
        }

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new ContinueStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }
    }
}
