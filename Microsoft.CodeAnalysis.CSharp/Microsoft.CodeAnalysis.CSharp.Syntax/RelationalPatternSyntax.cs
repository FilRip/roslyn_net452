#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class RelationalPatternSyntax : PatternSyntax
    {
        private ExpressionSyntax? expression;

        public SyntaxToken OperatorToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.RelationalPatternSyntax)base.Green).operatorToken, base.Position, 0);

        public ExpressionSyntax Expression => GetRed(ref expression, 1);

        internal RelationalPatternSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitRelationalPattern(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitRelationalPattern(this);

        public RelationalPatternSyntax Update(SyntaxToken operatorToken, ExpressionSyntax expression)
        {
            if (operatorToken != OperatorToken || expression != Expression)
            {
                RelationalPatternSyntax relationalPatternSyntax = SyntaxFactory.RelationalPattern(operatorToken, expression);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return relationalPatternSyntax;
                }
                return relationalPatternSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public RelationalPatternSyntax WithOperatorToken(SyntaxToken operatorToken)
        {
            return Update(operatorToken, Expression);
        }

        public RelationalPatternSyntax WithExpression(ExpressionSyntax expression)
        {
            return Update(OperatorToken, expression);
        }
    }
}
