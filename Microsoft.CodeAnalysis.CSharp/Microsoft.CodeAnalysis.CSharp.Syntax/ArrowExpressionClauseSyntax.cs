#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ArrowExpressionClauseSyntax : CSharpSyntaxNode
    {
        private ExpressionSyntax? expression;

        public SyntaxToken ArrowToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ArrowExpressionClauseSyntax)base.Green).arrowToken, base.Position, 0);

        public ExpressionSyntax Expression => GetRed(ref expression, 1);

        internal ArrowExpressionClauseSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitArrowExpressionClause(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitArrowExpressionClause(this);

        public ArrowExpressionClauseSyntax Update(SyntaxToken arrowToken, ExpressionSyntax expression)
        {
            if (arrowToken != ArrowToken || expression != Expression)
            {
                ArrowExpressionClauseSyntax arrowExpressionClauseSyntax = SyntaxFactory.ArrowExpressionClause(arrowToken, expression);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return arrowExpressionClauseSyntax;
                }
                return arrowExpressionClauseSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public ArrowExpressionClauseSyntax WithArrowToken(SyntaxToken arrowToken)
        {
            return Update(arrowToken, Expression);
        }

        public ArrowExpressionClauseSyntax WithExpression(ExpressionSyntax expression)
        {
            return Update(ArrowToken, expression);
        }
    }
}
