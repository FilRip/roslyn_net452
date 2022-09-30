#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class TypeConstraintSyntax : TypeParameterConstraintSyntax
    {
        private TypeSyntax? type;

        public TypeSyntax Type => GetRedAtZero(ref type);

        internal TypeConstraintSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitTypeConstraint(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitTypeConstraint(this);

        public TypeConstraintSyntax Update(TypeSyntax type)
        {
            if (type != Type)
            {
                TypeConstraintSyntax typeConstraintSyntax = SyntaxFactory.TypeConstraint(type);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return typeConstraintSyntax;
                }
                return typeConstraintSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public TypeConstraintSyntax WithType(TypeSyntax type)
        {
            return Update(type);
        }
    }
}
