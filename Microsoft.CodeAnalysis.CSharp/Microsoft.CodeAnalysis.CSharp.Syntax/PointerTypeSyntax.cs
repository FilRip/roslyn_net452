#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class PointerTypeSyntax : TypeSyntax
    {
        private TypeSyntax? elementType;

        public TypeSyntax ElementType => GetRedAtZero(ref elementType);

        public SyntaxToken AsteriskToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.PointerTypeSyntax)base.Green).asteriskToken, GetChildPosition(1), GetChildIndex(1));

        internal PointerTypeSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return GetRedAtZero(ref elementType);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return elementType;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitPointerType(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitPointerType(this);

        public PointerTypeSyntax Update(TypeSyntax elementType, SyntaxToken asteriskToken)
        {
            if (elementType != ElementType || asteriskToken != AsteriskToken)
            {
                PointerTypeSyntax pointerTypeSyntax = SyntaxFactory.PointerType(elementType, asteriskToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return pointerTypeSyntax;
                }
                return pointerTypeSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public PointerTypeSyntax WithElementType(TypeSyntax elementType)
        {
            return Update(elementType, AsteriskToken);
        }

        public PointerTypeSyntax WithAsteriskToken(SyntaxToken asteriskToken)
        {
            return Update(ElementType, asteriskToken);
        }
    }
}
