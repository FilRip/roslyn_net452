#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class CastExpressionSyntax : ExpressionSyntax
    {
        private TypeSyntax? type;

        private ExpressionSyntax? expression;

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CastExpressionSyntax)base.Green).openParenToken, base.Position, 0);

        public TypeSyntax Type => GetRed(ref type, 1);

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CastExpressionSyntax)base.Green).closeParenToken, GetChildPosition(2), GetChildIndex(2));

        public ExpressionSyntax Expression => GetRed(ref expression, 3);

        internal CastExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                1 => GetRed(ref type, 1),
                3 => GetRed(ref expression, 3),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                1 => type,
                3 => expression,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitCastExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitCastExpression(this);

        public CastExpressionSyntax Update(SyntaxToken openParenToken, TypeSyntax type, SyntaxToken closeParenToken, ExpressionSyntax expression)
        {
            if (openParenToken != OpenParenToken || type != Type || closeParenToken != CloseParenToken || expression != Expression)
            {
                CastExpressionSyntax castExpressionSyntax = SyntaxFactory.CastExpression(openParenToken, type, closeParenToken, expression);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return castExpressionSyntax;
                }
                return castExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public CastExpressionSyntax WithOpenParenToken(SyntaxToken openParenToken)
        {
            return Update(openParenToken, Type, CloseParenToken, Expression);
        }

        public CastExpressionSyntax WithType(TypeSyntax type)
        {
            return Update(OpenParenToken, type, CloseParenToken, Expression);
        }

        public CastExpressionSyntax WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return Update(OpenParenToken, Type, closeParenToken, Expression);
        }

        public CastExpressionSyntax WithExpression(ExpressionSyntax expression)
        {
            return Update(OpenParenToken, Type, CloseParenToken, expression);
        }
    }
}
