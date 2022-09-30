#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class IsPatternExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? expression;

        private PatternSyntax? pattern;

        public ExpressionSyntax Expression => GetRedAtZero(ref expression);

        public SyntaxToken IsKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.IsPatternExpressionSyntax)base.Green).isKeyword, GetChildPosition(1), GetChildIndex(1));

        public PatternSyntax Pattern => GetRed(ref pattern, 2);

        internal IsPatternExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref expression),
                2 => GetRed(ref pattern, 2),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => expression,
                2 => pattern,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitIsPatternExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitIsPatternExpression(this);

        public IsPatternExpressionSyntax Update(ExpressionSyntax expression, SyntaxToken isKeyword, PatternSyntax pattern)
        {
            if (expression != Expression || isKeyword != IsKeyword || pattern != Pattern)
            {
                IsPatternExpressionSyntax isPatternExpressionSyntax = SyntaxFactory.IsPatternExpression(expression, isKeyword, pattern);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return isPatternExpressionSyntax;
                }
                return isPatternExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public IsPatternExpressionSyntax WithExpression(ExpressionSyntax expression)
        {
            return Update(expression, IsKeyword, Pattern);
        }

        public IsPatternExpressionSyntax WithIsKeyword(SyntaxToken isKeyword)
        {
            return Update(Expression, isKeyword, Pattern);
        }

        public IsPatternExpressionSyntax WithPattern(PatternSyntax pattern)
        {
            return Update(Expression, IsKeyword, pattern);
        }
    }
}
