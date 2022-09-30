#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ErrorDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {
        public override SyntaxToken HashToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ErrorDirectiveTriviaSyntax)base.Green).hashToken, base.Position, 0);

        public SyntaxToken ErrorKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ErrorDirectiveTriviaSyntax)base.Green).errorKeyword, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxToken EndOfDirectiveToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ErrorDirectiveTriviaSyntax)base.Green).endOfDirectiveToken, GetChildPosition(2), GetChildIndex(2));

        public override bool IsActive => ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ErrorDirectiveTriviaSyntax)base.Green).IsActive;

        internal ErrorDirectiveTriviaSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitErrorDirectiveTrivia(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitErrorDirectiveTrivia(this);

        public ErrorDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken errorKeyword, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken != HashToken || errorKeyword != ErrorKeyword || endOfDirectiveToken != EndOfDirectiveToken)
            {
                ErrorDirectiveTriviaSyntax errorDirectiveTriviaSyntax = SyntaxFactory.ErrorDirectiveTrivia(hashToken, errorKeyword, endOfDirectiveToken, isActive);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return errorDirectiveTriviaSyntax;
                }
                return errorDirectiveTriviaSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken)
        {
            return WithHashToken(hashToken);
        }

        public new ErrorDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken)
        {
            return Update(hashToken, ErrorKeyword, EndOfDirectiveToken, IsActive);
        }

        public ErrorDirectiveTriviaSyntax WithErrorKeyword(SyntaxToken errorKeyword)
        {
            return Update(HashToken, errorKeyword, EndOfDirectiveToken, IsActive);
        }

        internal override DirectiveTriviaSyntax WithEndOfDirectiveTokenCore(SyntaxToken endOfDirectiveToken)
        {
            return WithEndOfDirectiveToken(endOfDirectiveToken);
        }

        public new ErrorDirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken)
        {
            return Update(HashToken, ErrorKeyword, endOfDirectiveToken, IsActive);
        }

        public ErrorDirectiveTriviaSyntax WithIsActive(bool isActive)
        {
            return Update(HashToken, ErrorKeyword, EndOfDirectiveToken, isActive);
        }
    }
}
