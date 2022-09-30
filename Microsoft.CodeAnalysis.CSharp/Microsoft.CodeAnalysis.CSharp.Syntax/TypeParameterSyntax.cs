#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class TypeParameterSyntax : CSharpSyntaxNode
    {
        private SyntaxNode? attributeLists;

        public SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public SyntaxToken VarianceKeyword
        {
            get
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken varianceKeyword = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeParameterSyntax)base.Green).varianceKeyword;
                if (varianceKeyword == null)
                {
                    return default(SyntaxToken);
                }
                return new SyntaxToken(this, varianceKeyword, GetChildPosition(1), GetChildIndex(1));
            }
        }

        public SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeParameterSyntax)base.Green).identifier, GetChildPosition(2), GetChildIndex(2));

        internal TypeParameterSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return GetRedAtZero(ref attributeLists);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return attributeLists;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitTypeParameter(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitTypeParameter(this);

        public TypeParameterSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken varianceKeyword, SyntaxToken identifier)
        {
            if (attributeLists != AttributeLists || varianceKeyword != VarianceKeyword || identifier != Identifier)
            {
                TypeParameterSyntax typeParameterSyntax = SyntaxFactory.TypeParameter(attributeLists, varianceKeyword, identifier);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return typeParameterSyntax;
                }
                return typeParameterSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public TypeParameterSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, VarianceKeyword, Identifier);
        }

        public TypeParameterSyntax WithVarianceKeyword(SyntaxToken varianceKeyword)
        {
            return Update(AttributeLists, varianceKeyword, Identifier);
        }

        public TypeParameterSyntax WithIdentifier(SyntaxToken identifier)
        {
            return Update(AttributeLists, VarianceKeyword, identifier);
        }

        public TypeParameterSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }
    }
}
