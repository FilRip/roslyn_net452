#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ArrayTypeSyntax : TypeSyntax
    {
        private TypeSyntax? elementType;

        private SyntaxNode? rankSpecifiers;

        public TypeSyntax ElementType => GetRedAtZero(ref elementType);

        public SyntaxList<ArrayRankSpecifierSyntax> RankSpecifiers => new SyntaxList<ArrayRankSpecifierSyntax>(GetRed(ref rankSpecifiers, 1));

        internal ArrayTypeSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref elementType),
                1 => GetRed(ref rankSpecifiers, 1),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => elementType,
                1 => rankSpecifiers,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitArrayType(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitArrayType(this);

        public ArrayTypeSyntax Update(TypeSyntax elementType, SyntaxList<ArrayRankSpecifierSyntax> rankSpecifiers)
        {
            if (elementType != ElementType || rankSpecifiers != RankSpecifiers)
            {
                ArrayTypeSyntax arrayTypeSyntax = SyntaxFactory.ArrayType(elementType, rankSpecifiers);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return arrayTypeSyntax;
                }
                return arrayTypeSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public ArrayTypeSyntax WithElementType(TypeSyntax elementType)
        {
            return Update(elementType, RankSpecifiers);
        }

        public ArrayTypeSyntax WithRankSpecifiers(SyntaxList<ArrayRankSpecifierSyntax> rankSpecifiers)
        {
            return Update(ElementType, rankSpecifiers);
        }

        public ArrayTypeSyntax AddRankSpecifiers(params ArrayRankSpecifierSyntax[] items)
        {
            return WithRankSpecifiers(RankSpecifiers.AddRange(items));
        }
    }
}
