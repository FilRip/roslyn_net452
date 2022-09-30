#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class NameEqualsSyntax : CSharpSyntaxNode
    {
        private IdentifierNameSyntax? name;

        public IdentifierNameSyntax Name => GetRedAtZero(ref name);

        public SyntaxToken EqualsToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.NameEqualsSyntax)base.Green).equalsToken, GetChildPosition(1), GetChildIndex(1));

        internal NameEqualsSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return GetRedAtZero(ref name);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return name;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitNameEquals(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitNameEquals(this);

        public NameEqualsSyntax Update(IdentifierNameSyntax name, SyntaxToken equalsToken)
        {
            if (name != Name || equalsToken != EqualsToken)
            {
                NameEqualsSyntax nameEqualsSyntax = SyntaxFactory.NameEquals(name, equalsToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return nameEqualsSyntax;
                }
                return nameEqualsSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public NameEqualsSyntax WithName(IdentifierNameSyntax name)
        {
            return Update(name, EqualsToken);
        }

        public NameEqualsSyntax WithEqualsToken(SyntaxToken equalsToken)
        {
            return Update(Name, equalsToken);
        }
    }
}
