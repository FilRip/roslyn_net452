#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class InitializerExpressionSyntax : ExpressionSyntax
    {
        private SyntaxNode? expressions;

        public SyntaxToken OpenBraceToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.InitializerExpressionSyntax)base.Green).openBraceToken, base.Position, 0);

        public SeparatedSyntaxList<ExpressionSyntax> Expressions
        {
            get
            {
                SyntaxNode red = GetRed(ref expressions, 1);
                if (red == null)
                {
                    return default(SeparatedSyntaxList<ExpressionSyntax>);
                }
                return new SeparatedSyntaxList<ExpressionSyntax>(red, GetChildIndex(1));
            }
        }

        public SyntaxToken CloseBraceToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.InitializerExpressionSyntax)base.Green).closeBraceToken, GetChildPosition(2), GetChildIndex(2));

        internal InitializerExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return GetRed(ref expressions, 1);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return expressions;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitInitializerExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitInitializerExpression(this);

        public InitializerExpressionSyntax Update(SyntaxToken openBraceToken, SeparatedSyntaxList<ExpressionSyntax> expressions, SyntaxToken closeBraceToken)
        {
            if (openBraceToken != OpenBraceToken || expressions != Expressions || closeBraceToken != CloseBraceToken)
            {
                InitializerExpressionSyntax initializerExpressionSyntax = SyntaxFactory.InitializerExpression(Kind(), openBraceToken, expressions, closeBraceToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return initializerExpressionSyntax;
                }
                return initializerExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public InitializerExpressionSyntax WithOpenBraceToken(SyntaxToken openBraceToken)
        {
            return Update(openBraceToken, Expressions, CloseBraceToken);
        }

        public InitializerExpressionSyntax WithExpressions(SeparatedSyntaxList<ExpressionSyntax> expressions)
        {
            return Update(OpenBraceToken, expressions, CloseBraceToken);
        }

        public InitializerExpressionSyntax WithCloseBraceToken(SyntaxToken closeBraceToken)
        {
            return Update(OpenBraceToken, Expressions, closeBraceToken);
        }

        public InitializerExpressionSyntax AddExpressions(params ExpressionSyntax[] items)
        {
            return WithExpressions(Expressions.AddRange(items));
        }
    }
}
