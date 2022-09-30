#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class AnonymousObjectMemberDeclaratorSyntax : CSharpSyntaxNode
    {
        private NameEqualsSyntax? nameEquals;

        private ExpressionSyntax? expression;

        public NameEqualsSyntax? NameEquals => GetRedAtZero(ref nameEquals);

        public ExpressionSyntax Expression => GetRed(ref expression, 1);

        internal AnonymousObjectMemberDeclaratorSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref nameEquals),
                1 => GetRed(ref expression, 1),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => nameEquals,
                1 => expression,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitAnonymousObjectMemberDeclarator(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitAnonymousObjectMemberDeclarator(this);

        public AnonymousObjectMemberDeclaratorSyntax Update(NameEqualsSyntax? nameEquals, ExpressionSyntax expression)
        {
            if (nameEquals != NameEquals || expression != Expression)
            {
                AnonymousObjectMemberDeclaratorSyntax anonymousObjectMemberDeclaratorSyntax = SyntaxFactory.AnonymousObjectMemberDeclarator(nameEquals, expression);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return anonymousObjectMemberDeclaratorSyntax;
                }
                return anonymousObjectMemberDeclaratorSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public AnonymousObjectMemberDeclaratorSyntax WithNameEquals(NameEqualsSyntax? nameEquals)
        {
            return Update(nameEquals, Expression);
        }

        public AnonymousObjectMemberDeclaratorSyntax WithExpression(ExpressionSyntax expression)
        {
            return Update(NameEquals, expression);
        }
    }
}
