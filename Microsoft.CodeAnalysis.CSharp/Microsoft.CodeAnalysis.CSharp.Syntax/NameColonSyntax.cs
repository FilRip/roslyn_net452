#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class NameColonSyntax : CSharpSyntaxNode
    {
        private IdentifierNameSyntax? name;

        public IdentifierNameSyntax Name => GetRedAtZero(ref name);

        public SyntaxToken ColonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.NameColonSyntax)base.Green).colonToken, GetChildPosition(1), GetChildIndex(1));

        internal NameColonSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitNameColon(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitNameColon(this);

        public NameColonSyntax Update(IdentifierNameSyntax name, SyntaxToken colonToken)
        {
            if (name != Name || colonToken != ColonToken)
            {
                NameColonSyntax nameColonSyntax = SyntaxFactory.NameColon(name, colonToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return nameColonSyntax;
                }
                return nameColonSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public NameColonSyntax WithName(IdentifierNameSyntax name)
        {
            return Update(name, ColonToken);
        }

        public NameColonSyntax WithColonToken(SyntaxToken colonToken)
        {
            return Update(Name, colonToken);
        }
    }
}
