#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class BadDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {
        public override SyntaxToken HashToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BadDirectiveTriviaSyntax)base.Green).hashToken, base.Position, 0);

        public SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BadDirectiveTriviaSyntax)base.Green).identifier, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxToken EndOfDirectiveToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BadDirectiveTriviaSyntax)base.Green).endOfDirectiveToken, GetChildPosition(2), GetChildIndex(2));

        public override bool IsActive => ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BadDirectiveTriviaSyntax)base.Green).IsActive;

        internal BadDirectiveTriviaSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitBadDirectiveTrivia(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitBadDirectiveTrivia(this);

        public BadDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken identifier, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken != HashToken || identifier != Identifier || endOfDirectiveToken != EndOfDirectiveToken)
            {
                BadDirectiveTriviaSyntax badDirectiveTriviaSyntax = SyntaxFactory.BadDirectiveTrivia(hashToken, identifier, endOfDirectiveToken, isActive);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return badDirectiveTriviaSyntax;
                }
                return badDirectiveTriviaSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken)
        {
            return WithHashToken(hashToken);
        }

        public new BadDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken)
        {
            return Update(hashToken, Identifier, EndOfDirectiveToken, IsActive);
        }

        public BadDirectiveTriviaSyntax WithIdentifier(SyntaxToken identifier)
        {
            return Update(HashToken, identifier, EndOfDirectiveToken, IsActive);
        }

        internal override DirectiveTriviaSyntax WithEndOfDirectiveTokenCore(SyntaxToken endOfDirectiveToken)
        {
            return WithEndOfDirectiveToken(endOfDirectiveToken);
        }

        public new BadDirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken)
        {
            return Update(HashToken, Identifier, endOfDirectiveToken, IsActive);
        }

        public BadDirectiveTriviaSyntax WithIsActive(bool isActive)
        {
            return Update(HashToken, Identifier, EndOfDirectiveToken, isActive);
        }
    }
}
