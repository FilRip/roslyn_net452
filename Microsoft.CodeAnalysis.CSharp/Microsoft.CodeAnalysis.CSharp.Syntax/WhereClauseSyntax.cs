#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class WhereClauseSyntax : QueryClauseSyntax
    {
        private ExpressionSyntax? condition;

        public SyntaxToken WhereKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.WhereClauseSyntax)base.Green).whereKeyword, base.Position, 0);

        public ExpressionSyntax Condition => GetRed(ref condition, 1);

        internal WhereClauseSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return GetRed(ref condition, 1);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return condition;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitWhereClause(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitWhereClause(this);

        public WhereClauseSyntax Update(SyntaxToken whereKeyword, ExpressionSyntax condition)
        {
            if (whereKeyword != WhereKeyword || condition != Condition)
            {
                WhereClauseSyntax whereClauseSyntax = SyntaxFactory.WhereClause(whereKeyword, condition);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return whereClauseSyntax;
                }
                return whereClauseSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public WhereClauseSyntax WithWhereKeyword(SyntaxToken whereKeyword)
        {
            return Update(whereKeyword, Condition);
        }

        public WhereClauseSyntax WithCondition(ExpressionSyntax condition)
        {
            return Update(WhereKeyword, condition);
        }
    }
}
