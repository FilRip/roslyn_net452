#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class DefaultExpressionSyntax : ExpressionSyntax
    {
        private TypeSyntax? type;

        public SyntaxToken Keyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DefaultExpressionSyntax)base.Green).keyword, base.Position, 0);

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DefaultExpressionSyntax)base.Green).openParenToken, GetChildPosition(1), GetChildIndex(1));

        public TypeSyntax Type => GetRed(ref type, 2);

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DefaultExpressionSyntax)base.Green).closeParenToken, GetChildPosition(3), GetChildIndex(3));

        internal DefaultExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 2)
            {
                return null;
            }
            return GetRed(ref type, 2);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 2)
            {
                return null;
            }
            return type;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitDefaultExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitDefaultExpression(this);

        public DefaultExpressionSyntax Update(SyntaxToken keyword, SyntaxToken openParenToken, TypeSyntax type, SyntaxToken closeParenToken)
        {
            if (keyword != Keyword || openParenToken != OpenParenToken || type != Type || closeParenToken != CloseParenToken)
            {
                DefaultExpressionSyntax defaultExpressionSyntax = SyntaxFactory.DefaultExpression(keyword, openParenToken, type, closeParenToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return defaultExpressionSyntax;
                }
                return defaultExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public DefaultExpressionSyntax WithKeyword(SyntaxToken keyword)
        {
            return Update(keyword, OpenParenToken, Type, CloseParenToken);
        }

        public DefaultExpressionSyntax WithOpenParenToken(SyntaxToken openParenToken)
        {
            return Update(Keyword, openParenToken, Type, CloseParenToken);
        }

        public DefaultExpressionSyntax WithType(TypeSyntax type)
        {
            return Update(Keyword, OpenParenToken, type, CloseParenToken);
        }

        public DefaultExpressionSyntax WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return Update(Keyword, OpenParenToken, Type, closeParenToken);
        }
    }
}
