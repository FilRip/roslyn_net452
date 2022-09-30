#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class BreakStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public SyntaxToken BreakKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BreakStatementSyntax)base.Green).breakKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken SemicolonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BreakStatementSyntax)base.Green).semicolonToken, GetChildPosition(2), GetChildIndex(2));

        public BreakStatementSyntax Update(SyntaxToken breakKeyword, SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, breakKeyword, semicolonToken);
        }

        internal BreakStatementSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitBreakStatement(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitBreakStatement(this);

        public BreakStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken breakKeyword, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || breakKeyword != BreakKeyword || semicolonToken != SemicolonToken)
            {
                BreakStatementSyntax breakStatementSyntax = SyntaxFactory.BreakStatement(attributeLists, breakKeyword, semicolonToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return breakStatementSyntax;
                }
                return breakStatementSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new BreakStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, BreakKeyword, SemicolonToken);
        }

        public BreakStatementSyntax WithBreakKeyword(SyntaxToken breakKeyword)
        {
            return Update(AttributeLists, breakKeyword, SemicolonToken);
        }

        public BreakStatementSyntax WithSemicolonToken(SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, BreakKeyword, semicolonToken);
        }

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new BreakStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }
    }
}
