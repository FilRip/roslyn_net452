#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class MemberAccessExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? expression;

        private SimpleNameSyntax? name;

        public ExpressionSyntax Expression => GetRedAtZero(ref expression);

        public SyntaxToken OperatorToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.MemberAccessExpressionSyntax)base.Green).operatorToken, GetChildPosition(1), GetChildIndex(1));

        public SimpleNameSyntax Name => GetRed(ref name, 2);

        internal MemberAccessExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref expression),
                2 => GetRed(ref name, 2),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => expression,
                2 => name,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitMemberAccessExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitMemberAccessExpression(this);

        public MemberAccessExpressionSyntax Update(ExpressionSyntax expression, SyntaxToken operatorToken, SimpleNameSyntax name)
        {
            if (expression != Expression || operatorToken != OperatorToken || name != Name)
            {
                MemberAccessExpressionSyntax memberAccessExpressionSyntax = SyntaxFactory.MemberAccessExpression(Kind(), expression, operatorToken, name);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return memberAccessExpressionSyntax;
                }
                return memberAccessExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public MemberAccessExpressionSyntax WithExpression(ExpressionSyntax expression)
        {
            return Update(expression, OperatorToken, Name);
        }

        public MemberAccessExpressionSyntax WithOperatorToken(SyntaxToken operatorToken)
        {
            return Update(Expression, operatorToken, Name);
        }

        public MemberAccessExpressionSyntax WithName(SimpleNameSyntax name)
        {
            return Update(Expression, OperatorToken, name);
        }
    }
}
