#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class WhenClauseSyntax : CSharpSyntaxNode
    {
        private ExpressionSyntax? condition;

        public SyntaxToken WhenKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.WhenClauseSyntax)base.Green).whenKeyword, base.Position, 0);

        public ExpressionSyntax Condition => GetRed(ref condition, 1);

        internal WhenClauseSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitWhenClause(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitWhenClause(this);

        public WhenClauseSyntax Update(SyntaxToken whenKeyword, ExpressionSyntax condition)
        {
            if (whenKeyword != WhenKeyword || condition != Condition)
            {
                WhenClauseSyntax whenClauseSyntax = SyntaxFactory.WhenClause(whenKeyword, condition);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return whenClauseSyntax;
                }
                return whenClauseSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public WhenClauseSyntax WithWhenKeyword(SyntaxToken whenKeyword)
        {
            return Update(whenKeyword, Condition);
        }

        public WhenClauseSyntax WithCondition(ExpressionSyntax condition)
        {
            return Update(WhenKeyword, condition);
        }
    }
}
