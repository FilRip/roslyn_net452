#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class AccessorListSyntax : CSharpSyntaxNode
    {
        private SyntaxNode? accessors;

        public SyntaxToken OpenBraceToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AccessorListSyntax)base.Green).openBraceToken, base.Position, 0);

        public SyntaxList<AccessorDeclarationSyntax> Accessors => new SyntaxList<AccessorDeclarationSyntax>(GetRed(ref accessors, 1));

        public SyntaxToken CloseBraceToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AccessorListSyntax)base.Green).closeBraceToken, GetChildPosition(2), GetChildIndex(2));

        internal AccessorListSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return GetRed(ref accessors, 1);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return accessors;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitAccessorList(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitAccessorList(this);

        public AccessorListSyntax Update(SyntaxToken openBraceToken, SyntaxList<AccessorDeclarationSyntax> accessors, SyntaxToken closeBraceToken)
        {
            if (openBraceToken != OpenBraceToken || accessors != Accessors || closeBraceToken != CloseBraceToken)
            {
                AccessorListSyntax accessorListSyntax = SyntaxFactory.AccessorList(openBraceToken, accessors, closeBraceToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return accessorListSyntax;
                }
                return accessorListSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public AccessorListSyntax WithOpenBraceToken(SyntaxToken openBraceToken)
        {
            return Update(openBraceToken, Accessors, CloseBraceToken);
        }

        public AccessorListSyntax WithAccessors(SyntaxList<AccessorDeclarationSyntax> accessors)
        {
            return Update(OpenBraceToken, accessors, CloseBraceToken);
        }

        public AccessorListSyntax WithCloseBraceToken(SyntaxToken closeBraceToken)
        {
            return Update(OpenBraceToken, Accessors, closeBraceToken);
        }

        public AccessorListSyntax AddAccessors(params AccessorDeclarationSyntax[] items)
        {
            return WithAccessors(Accessors.AddRange(items));
        }
    }
}
