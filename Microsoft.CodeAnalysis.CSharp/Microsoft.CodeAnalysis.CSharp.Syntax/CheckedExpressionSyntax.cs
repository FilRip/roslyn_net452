#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class CheckedExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? expression;

        public SyntaxToken Keyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CheckedExpressionSyntax)base.Green).keyword, base.Position, 0);

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CheckedExpressionSyntax)base.Green).openParenToken, GetChildPosition(1), GetChildIndex(1));

        public ExpressionSyntax Expression => GetRed(ref expression, 2);

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CheckedExpressionSyntax)base.Green).closeParenToken, GetChildPosition(3), GetChildIndex(3));

        internal CheckedExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 2)
            {
                return null;
            }
            return GetRed(ref expression, 2);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 2)
            {
                return null;
            }
            return expression;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitCheckedExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitCheckedExpression(this);

        public CheckedExpressionSyntax Update(SyntaxToken keyword, SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken)
        {
            if (keyword != Keyword || openParenToken != OpenParenToken || expression != Expression || closeParenToken != CloseParenToken)
            {
                CheckedExpressionSyntax checkedExpressionSyntax = SyntaxFactory.CheckedExpression(Kind(), keyword, openParenToken, expression, closeParenToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return checkedExpressionSyntax;
                }
                return checkedExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public CheckedExpressionSyntax WithKeyword(SyntaxToken keyword)
        {
            return Update(keyword, OpenParenToken, Expression, CloseParenToken);
        }

        public CheckedExpressionSyntax WithOpenParenToken(SyntaxToken openParenToken)
        {
            return Update(Keyword, openParenToken, Expression, CloseParenToken);
        }

        public CheckedExpressionSyntax WithExpression(ExpressionSyntax expression)
        {
            return Update(Keyword, OpenParenToken, expression, CloseParenToken);
        }

        public CheckedExpressionSyntax WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return Update(Keyword, OpenParenToken, Expression, closeParenToken);
        }
    }
}
