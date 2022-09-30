#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class DefaultConstraintSyntax : TypeParameterConstraintSyntax
    {
        public SyntaxToken DefaultKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DefaultConstraintSyntax)base.Green).defaultKeyword, base.Position, 0);

        internal DefaultConstraintSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return null;
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return null;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitDefaultConstraint(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitDefaultConstraint(this);

        public DefaultConstraintSyntax Update(SyntaxToken defaultKeyword)
        {
            if (defaultKeyword != DefaultKeyword)
            {
                DefaultConstraintSyntax defaultConstraintSyntax = SyntaxFactory.DefaultConstraint(defaultKeyword);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return defaultConstraintSyntax;
                }
                return defaultConstraintSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public DefaultConstraintSyntax WithDefaultKeyword(SyntaxToken defaultKeyword)
        {
            return Update(defaultKeyword);
        }
    }
}
