#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ElseDirectiveTriviaSyntax : BranchingDirectiveTriviaSyntax
    {
        public override SyntaxToken HashToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ElseDirectiveTriviaSyntax)base.Green).hashToken, base.Position, 0);

        public SyntaxToken ElseKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ElseDirectiveTriviaSyntax)base.Green).elseKeyword, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxToken EndOfDirectiveToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ElseDirectiveTriviaSyntax)base.Green).endOfDirectiveToken, GetChildPosition(2), GetChildIndex(2));

        public override bool IsActive => ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ElseDirectiveTriviaSyntax)base.Green).IsActive;

        public override bool BranchTaken => ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ElseDirectiveTriviaSyntax)base.Green).BranchTaken;

        internal ElseDirectiveTriviaSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitElseDirectiveTrivia(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitElseDirectiveTrivia(this);

        public ElseDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken elseKeyword, SyntaxToken endOfDirectiveToken, bool isActive, bool branchTaken)
        {
            if (hashToken != HashToken || elseKeyword != ElseKeyword || endOfDirectiveToken != EndOfDirectiveToken)
            {
                ElseDirectiveTriviaSyntax elseDirectiveTriviaSyntax = SyntaxFactory.ElseDirectiveTrivia(hashToken, elseKeyword, endOfDirectiveToken, isActive, branchTaken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return elseDirectiveTriviaSyntax;
                }
                return elseDirectiveTriviaSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken)
        {
            return WithHashToken(hashToken);
        }

        public new ElseDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken)
        {
            return Update(hashToken, ElseKeyword, EndOfDirectiveToken, IsActive, BranchTaken);
        }

        public ElseDirectiveTriviaSyntax WithElseKeyword(SyntaxToken elseKeyword)
        {
            return Update(HashToken, elseKeyword, EndOfDirectiveToken, IsActive, BranchTaken);
        }

        internal override DirectiveTriviaSyntax WithEndOfDirectiveTokenCore(SyntaxToken endOfDirectiveToken)
        {
            return WithEndOfDirectiveToken(endOfDirectiveToken);
        }

        public new ElseDirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken)
        {
            return Update(HashToken, ElseKeyword, endOfDirectiveToken, IsActive, BranchTaken);
        }

        public ElseDirectiveTriviaSyntax WithIsActive(bool isActive)
        {
            return Update(HashToken, ElseKeyword, EndOfDirectiveToken, isActive, BranchTaken);
        }

        public ElseDirectiveTriviaSyntax WithBranchTaken(bool branchTaken)
        {
            return Update(HashToken, ElseKeyword, EndOfDirectiveToken, IsActive, branchTaken);
        }
    }
}
