#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class LetClauseSyntax : QueryClauseSyntax
    {
        private ExpressionSyntax? expression;

        public SyntaxToken LetKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.LetClauseSyntax)base.Green).letKeyword, base.Position, 0);

        public SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.LetClauseSyntax)base.Green).identifier, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken EqualsToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.LetClauseSyntax)base.Green).equalsToken, GetChildPosition(2), GetChildIndex(2));

        public ExpressionSyntax Expression => GetRed(ref expression, 3);

        internal LetClauseSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 3)
            {
                return null;
            }
            return GetRed(ref expression, 3);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 3)
            {
                return null;
            }
            return expression;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitLetClause(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitLetClause(this);

        public LetClauseSyntax Update(SyntaxToken letKeyword, SyntaxToken identifier, SyntaxToken equalsToken, ExpressionSyntax expression)
        {
            if (letKeyword != LetKeyword || identifier != Identifier || equalsToken != EqualsToken || expression != Expression)
            {
                LetClauseSyntax letClauseSyntax = SyntaxFactory.LetClause(letKeyword, identifier, equalsToken, expression);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return letClauseSyntax;
                }
                return letClauseSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public LetClauseSyntax WithLetKeyword(SyntaxToken letKeyword)
        {
            return Update(letKeyword, Identifier, EqualsToken, Expression);
        }

        public LetClauseSyntax WithIdentifier(SyntaxToken identifier)
        {
            return Update(LetKeyword, identifier, EqualsToken, Expression);
        }

        public LetClauseSyntax WithEqualsToken(SyntaxToken equalsToken)
        {
            return Update(LetKeyword, Identifier, equalsToken, Expression);
        }

        public LetClauseSyntax WithExpression(ExpressionSyntax expression)
        {
            return Update(LetKeyword, Identifier, EqualsToken, expression);
        }
    }
}
