#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ArrayRankSpecifierSyntax : CSharpSyntaxNode
    {
        private SyntaxNode? sizes;

        public int Rank => Sizes.Count;

        public SyntaxToken OpenBracketToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ArrayRankSpecifierSyntax)base.Green).openBracketToken, base.Position, 0);

        public SeparatedSyntaxList<ExpressionSyntax> Sizes
        {
            get
            {
                SyntaxNode red = GetRed(ref sizes, 1);
                if (red == null)
                {
                    return default(SeparatedSyntaxList<ExpressionSyntax>);
                }
                return new SeparatedSyntaxList<ExpressionSyntax>(red, GetChildIndex(1));
            }
        }

        public SyntaxToken CloseBracketToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ArrayRankSpecifierSyntax)base.Green).closeBracketToken, GetChildPosition(2), GetChildIndex(2));

        internal ArrayRankSpecifierSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return GetRed(ref sizes, 1);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return sizes;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitArrayRankSpecifier(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitArrayRankSpecifier(this);

        public ArrayRankSpecifierSyntax Update(SyntaxToken openBracketToken, SeparatedSyntaxList<ExpressionSyntax> sizes, SyntaxToken closeBracketToken)
        {
            if (openBracketToken != OpenBracketToken || sizes != Sizes || closeBracketToken != CloseBracketToken)
            {
                ArrayRankSpecifierSyntax arrayRankSpecifierSyntax = SyntaxFactory.ArrayRankSpecifier(openBracketToken, sizes, closeBracketToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return arrayRankSpecifierSyntax;
                }
                return arrayRankSpecifierSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public ArrayRankSpecifierSyntax WithOpenBracketToken(SyntaxToken openBracketToken)
        {
            return Update(openBracketToken, Sizes, CloseBracketToken);
        }

        public ArrayRankSpecifierSyntax WithSizes(SeparatedSyntaxList<ExpressionSyntax> sizes)
        {
            return Update(OpenBracketToken, sizes, CloseBracketToken);
        }

        public ArrayRankSpecifierSyntax WithCloseBracketToken(SyntaxToken closeBracketToken)
        {
            return Update(OpenBracketToken, Sizes, closeBracketToken);
        }

        public ArrayRankSpecifierSyntax AddSizes(params ExpressionSyntax[] items)
        {
            return WithSizes(Sizes.AddRange(items));
        }
    }
}
