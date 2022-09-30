#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class SelectClauseSyntax : SelectOrGroupClauseSyntax
    {
        private ExpressionSyntax? expression;

        public SyntaxToken SelectKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SelectClauseSyntax)base.Green).selectKeyword, base.Position, 0);

        public ExpressionSyntax Expression => GetRed(ref expression, 1);

        internal SelectClauseSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return GetRed(ref expression, 1);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return expression;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitSelectClause(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitSelectClause(this);

        public SelectClauseSyntax Update(SyntaxToken selectKeyword, ExpressionSyntax expression)
        {
            if (selectKeyword != SelectKeyword || expression != Expression)
            {
                SelectClauseSyntax selectClauseSyntax = SyntaxFactory.SelectClause(selectKeyword, expression);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return selectClauseSyntax;
                }
                return selectClauseSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public SelectClauseSyntax WithSelectKeyword(SyntaxToken selectKeyword)
        {
            return Update(selectKeyword, Expression);
        }

        public SelectClauseSyntax WithExpression(ExpressionSyntax expression)
        {
            return Update(SelectKeyword, expression);
        }
    }
}
