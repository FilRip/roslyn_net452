#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class QueryExpressionSyntax : ExpressionSyntax
    {
        private FromClauseSyntax? fromClause;

        private QueryBodySyntax? body;

        public FromClauseSyntax FromClause => GetRedAtZero(ref fromClause);

        public QueryBodySyntax Body => GetRed(ref body, 1);

        internal QueryExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref fromClause),
                1 => GetRed(ref body, 1),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => fromClause,
                1 => body,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitQueryExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitQueryExpression(this);

        public QueryExpressionSyntax Update(FromClauseSyntax fromClause, QueryBodySyntax body)
        {
            if (fromClause != FromClause || body != Body)
            {
                QueryExpressionSyntax queryExpressionSyntax = SyntaxFactory.QueryExpression(fromClause, body);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return queryExpressionSyntax;
                }
                return queryExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public QueryExpressionSyntax WithFromClause(FromClauseSyntax fromClause)
        {
            return Update(fromClause, Body);
        }

        public QueryExpressionSyntax WithBody(QueryBodySyntax body)
        {
            return Update(FromClause, body);
        }

        public QueryExpressionSyntax AddBodyClauses(params QueryClauseSyntax[] items)
        {
            return WithBody(Body.WithClauses(Body.Clauses.AddRange(items)));
        }
    }
}
