#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class RegionDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {
        public override SyntaxToken HashToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax)base.Green).hashToken, base.Position, 0);

        public SyntaxToken RegionKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax)base.Green).regionKeyword, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxToken EndOfDirectiveToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax)base.Green).endOfDirectiveToken, GetChildPosition(2), GetChildIndex(2));

        public override bool IsActive => ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax)base.Green).IsActive;

        internal RegionDirectiveTriviaSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitRegionDirectiveTrivia(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitRegionDirectiveTrivia(this);

        public RegionDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken regionKeyword, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken != HashToken || regionKeyword != RegionKeyword || endOfDirectiveToken != EndOfDirectiveToken)
            {
                RegionDirectiveTriviaSyntax regionDirectiveTriviaSyntax = SyntaxFactory.RegionDirectiveTrivia(hashToken, regionKeyword, endOfDirectiveToken, isActive);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return regionDirectiveTriviaSyntax;
                }
                return regionDirectiveTriviaSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken)
        {
            return WithHashToken(hashToken);
        }

        public new RegionDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken)
        {
            return Update(hashToken, RegionKeyword, EndOfDirectiveToken, IsActive);
        }

        public RegionDirectiveTriviaSyntax WithRegionKeyword(SyntaxToken regionKeyword)
        {
            return Update(HashToken, regionKeyword, EndOfDirectiveToken, IsActive);
        }

        internal override DirectiveTriviaSyntax WithEndOfDirectiveTokenCore(SyntaxToken endOfDirectiveToken)
        {
            return WithEndOfDirectiveToken(endOfDirectiveToken);
        }

        public new RegionDirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken)
        {
            return Update(HashToken, RegionKeyword, endOfDirectiveToken, IsActive);
        }

        public RegionDirectiveTriviaSyntax WithIsActive(bool isActive)
        {
            return Update(HashToken, RegionKeyword, EndOfDirectiveToken, isActive);
        }
    }
}
