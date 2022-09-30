#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ThrowStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;

        private ExpressionSyntax? expression;

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public SyntaxToken ThrowKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ThrowStatementSyntax)base.Green).throwKeyword, GetChildPosition(1), GetChildIndex(1));

        public ExpressionSyntax? Expression => GetRed(ref expression, 2);

        public SyntaxToken SemicolonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ThrowStatementSyntax)base.Green).semicolonToken, GetChildPosition(3), GetChildIndex(3));

        public ThrowStatementSyntax Update(SyntaxToken throwKeyword, ExpressionSyntax expression, SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, throwKeyword, expression, semicolonToken);
        }

        internal ThrowStatementSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                2 => GetRed(ref expression, 2),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                2 => expression,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitThrowStatement(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitThrowStatement(this);

        public ThrowStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken throwKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || throwKeyword != ThrowKeyword || expression != Expression || semicolonToken != SemicolonToken)
            {
                ThrowStatementSyntax throwStatementSyntax = SyntaxFactory.ThrowStatement(attributeLists, throwKeyword, expression, semicolonToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return throwStatementSyntax;
                }
                return throwStatementSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new ThrowStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, ThrowKeyword, Expression, SemicolonToken);
        }

        public ThrowStatementSyntax WithThrowKeyword(SyntaxToken throwKeyword)
        {
            return Update(AttributeLists, throwKeyword, Expression, SemicolonToken);
        }

        public ThrowStatementSyntax WithExpression(ExpressionSyntax? expression)
        {
            return Update(AttributeLists, ThrowKeyword, expression, SemicolonToken);
        }

        public ThrowStatementSyntax WithSemicolonToken(SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, ThrowKeyword, Expression, semicolonToken);
        }

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new ThrowStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }
    }
}
