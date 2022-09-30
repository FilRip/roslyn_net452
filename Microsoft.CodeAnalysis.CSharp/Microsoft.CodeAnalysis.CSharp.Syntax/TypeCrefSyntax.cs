#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class TypeCrefSyntax : CrefSyntax
    {
        private TypeSyntax? type;

        public TypeSyntax Type => GetRedAtZero(ref type);

        internal TypeCrefSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return GetRedAtZero(ref type);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return type;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitTypeCref(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitTypeCref(this);

        public TypeCrefSyntax Update(TypeSyntax type)
        {
            if (type != Type)
            {
                TypeCrefSyntax typeCrefSyntax = SyntaxFactory.TypeCref(type);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return typeCrefSyntax;
                }
                return typeCrefSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public TypeCrefSyntax WithType(TypeSyntax type)
        {
            return Update(type);
        }
    }
}
