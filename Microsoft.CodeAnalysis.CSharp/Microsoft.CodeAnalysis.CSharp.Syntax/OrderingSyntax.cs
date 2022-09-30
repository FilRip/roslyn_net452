#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class OrderingSyntax : CSharpSyntaxNode
    {
        private ExpressionSyntax? expression;

        public ExpressionSyntax Expression => GetRedAtZero(ref expression);

        public SyntaxToken AscendingOrDescendingKeyword
        {
            get
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken ascendingOrDescendingKeyword = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.OrderingSyntax)base.Green).ascendingOrDescendingKeyword;
                if (ascendingOrDescendingKeyword == null)
                {
                    return default(SyntaxToken);
                }
                return new SyntaxToken(this, ascendingOrDescendingKeyword, GetChildPosition(1), GetChildIndex(1));
            }
        }

        internal OrderingSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitOrdering(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitOrdering(this);

        public OrderingSyntax Update(ExpressionSyntax expression, SyntaxToken ascendingOrDescendingKeyword)
        {
            if (expression != Expression || ascendingOrDescendingKeyword != AscendingOrDescendingKeyword)
            {
                OrderingSyntax orderingSyntax = SyntaxFactory.Ordering(Kind(), expression, ascendingOrDescendingKeyword);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return orderingSyntax;
                }
                return orderingSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public OrderingSyntax WithExpression(ExpressionSyntax expression)
        {
            return Update(expression, AscendingOrDescendingKeyword);
        }

        public OrderingSyntax WithAscendingOrDescendingKeyword(SyntaxToken ascendingOrDescendingKeyword)
        {
            return Update(Expression, ascendingOrDescendingKeyword);
        }
    }
}
