#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class EqualsValueClauseSyntax : CSharpSyntaxNode
    {
        private ExpressionSyntax? value;

        public SyntaxToken EqualsToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.EqualsValueClauseSyntax)base.Green).equalsToken, base.Position, 0);

        public ExpressionSyntax Value => GetRed(ref value, 1);

        internal EqualsValueClauseSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return GetRed(ref value, 1);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return value;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitEqualsValueClause(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitEqualsValueClause(this);

        public EqualsValueClauseSyntax Update(SyntaxToken equalsToken, ExpressionSyntax value)
        {
            if (equalsToken != EqualsToken || value != Value)
            {
                EqualsValueClauseSyntax equalsValueClauseSyntax = SyntaxFactory.EqualsValueClause(equalsToken, value);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return equalsValueClauseSyntax;
                }
                return equalsValueClauseSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public EqualsValueClauseSyntax WithEqualsToken(SyntaxToken equalsToken)
        {
            return Update(equalsToken, Value);
        }

        public EqualsValueClauseSyntax WithValue(ExpressionSyntax value)
        {
            return Update(EqualsToken, value);
        }
    }
}
