#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class TypePatternSyntax : PatternSyntax
    {
        private TypeSyntax? type;

        public TypeSyntax Type => GetRedAtZero(ref type);

        internal TypePatternSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitTypePattern(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitTypePattern(this);

        public TypePatternSyntax Update(TypeSyntax type)
        {
            if (type != Type)
            {
                TypePatternSyntax typePatternSyntax = SyntaxFactory.TypePattern(type);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return typePatternSyntax;
                }
                return typePatternSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public TypePatternSyntax WithType(TypeSyntax type)
        {
            return Update(type);
        }
    }
}
