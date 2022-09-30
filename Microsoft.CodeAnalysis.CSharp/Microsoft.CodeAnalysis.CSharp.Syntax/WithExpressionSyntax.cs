#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class WithExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? expression;

        private InitializerExpressionSyntax? initializer;

        public ExpressionSyntax Expression => GetRedAtZero(ref expression);

        public SyntaxToken WithKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.WithExpressionSyntax)base.Green).withKeyword, GetChildPosition(1), GetChildIndex(1));

        public InitializerExpressionSyntax Initializer => GetRed(ref initializer, 2);

        internal WithExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref expression),
                2 => GetRed(ref initializer, 2),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => expression,
                2 => initializer,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitWithExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitWithExpression(this);

        public WithExpressionSyntax Update(ExpressionSyntax expression, SyntaxToken withKeyword, InitializerExpressionSyntax initializer)
        {
            if (expression != Expression || withKeyword != WithKeyword || initializer != Initializer)
            {
                WithExpressionSyntax withExpressionSyntax = SyntaxFactory.WithExpression(expression, withKeyword, initializer);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return withExpressionSyntax;
                }
                return withExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public WithExpressionSyntax WithExpression(ExpressionSyntax expression)
        {
            return Update(expression, WithKeyword, Initializer);
        }

        public WithExpressionSyntax WithWithKeyword(SyntaxToken withKeyword)
        {
            return Update(Expression, withKeyword, Initializer);
        }

        public WithExpressionSyntax WithInitializer(InitializerExpressionSyntax initializer)
        {
            return Update(Expression, WithKeyword, initializer);
        }

        public WithExpressionSyntax AddInitializerExpressions(params ExpressionSyntax[] items)
        {
            return WithInitializer(Initializer.WithExpressions(Initializer.Expressions.AddRange(items)));
        }
    }
}
