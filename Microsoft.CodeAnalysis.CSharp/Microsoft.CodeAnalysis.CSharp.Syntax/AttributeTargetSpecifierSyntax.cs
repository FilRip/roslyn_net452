using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class AttributeTargetSpecifierSyntax : CSharpSyntaxNode
    {
        public SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeTargetSpecifierSyntax)base.Green).identifier, base.Position, 0);

        public SyntaxToken ColonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeTargetSpecifierSyntax)base.Green).colonToken, GetChildPosition(1), GetChildIndex(1));

        internal AttributeLocation GetAttributeLocation()
        {
            return Identifier.ToAttributeLocation();
        }

        internal AttributeTargetSpecifierSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitAttributeTargetSpecifier(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitAttributeTargetSpecifier(this);

        public AttributeTargetSpecifierSyntax Update(SyntaxToken identifier, SyntaxToken colonToken)
        {
            if (identifier != Identifier || colonToken != ColonToken)
            {
                AttributeTargetSpecifierSyntax attributeTargetSpecifierSyntax = SyntaxFactory.AttributeTargetSpecifier(identifier, colonToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return attributeTargetSpecifierSyntax;
                }
                return attributeTargetSpecifierSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public AttributeTargetSpecifierSyntax WithIdentifier(SyntaxToken identifier)
        {
            return Update(identifier, ColonToken);
        }

        public AttributeTargetSpecifierSyntax WithColonToken(SyntaxToken colonToken)
        {
            return Update(Identifier, colonToken);
        }
    }
}
