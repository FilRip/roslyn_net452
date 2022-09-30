#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ConstantPatternSyntax : PatternSyntax
    {
        private ExpressionSyntax? expression;

        public ExpressionSyntax Expression => GetRedAtZero(ref expression);

        internal ConstantPatternSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return GetRedAtZero(ref expression);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return expression;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitConstantPattern(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitConstantPattern(this);

        public ConstantPatternSyntax Update(ExpressionSyntax expression)
        {
            if (expression != Expression)
            {
                ConstantPatternSyntax constantPatternSyntax = SyntaxFactory.ConstantPattern(expression);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return constantPatternSyntax;
                }
                return constantPatternSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public ConstantPatternSyntax WithExpression(ExpressionSyntax expression)
        {
            return Update(expression);
        }
    }
}
