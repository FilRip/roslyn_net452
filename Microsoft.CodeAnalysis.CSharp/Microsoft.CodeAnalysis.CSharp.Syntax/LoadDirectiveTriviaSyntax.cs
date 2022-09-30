#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class LoadDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {
        public override SyntaxToken HashToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.LoadDirectiveTriviaSyntax)base.Green).hashToken, base.Position, 0);

        public SyntaxToken LoadKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.LoadDirectiveTriviaSyntax)base.Green).loadKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken File => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.LoadDirectiveTriviaSyntax)base.Green).file, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxToken EndOfDirectiveToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.LoadDirectiveTriviaSyntax)base.Green).endOfDirectiveToken, GetChildPosition(3), GetChildIndex(3));

        public override bool IsActive => ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.LoadDirectiveTriviaSyntax)base.Green).IsActive;

        internal LoadDirectiveTriviaSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitLoadDirectiveTrivia(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitLoadDirectiveTrivia(this);

        public LoadDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken loadKeyword, SyntaxToken file, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken != HashToken || loadKeyword != LoadKeyword || file != File || endOfDirectiveToken != EndOfDirectiveToken)
            {
                LoadDirectiveTriviaSyntax loadDirectiveTriviaSyntax = SyntaxFactory.LoadDirectiveTrivia(hashToken, loadKeyword, file, endOfDirectiveToken, isActive);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return loadDirectiveTriviaSyntax;
                }
                return loadDirectiveTriviaSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken)
        {
            return WithHashToken(hashToken);
        }

        public new LoadDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken)
        {
            return Update(hashToken, LoadKeyword, File, EndOfDirectiveToken, IsActive);
        }

        public LoadDirectiveTriviaSyntax WithLoadKeyword(SyntaxToken loadKeyword)
        {
            return Update(HashToken, loadKeyword, File, EndOfDirectiveToken, IsActive);
        }

        public LoadDirectiveTriviaSyntax WithFile(SyntaxToken file)
        {
            return Update(HashToken, LoadKeyword, file, EndOfDirectiveToken, IsActive);
        }

        internal override DirectiveTriviaSyntax WithEndOfDirectiveTokenCore(SyntaxToken endOfDirectiveToken)
        {
            return WithEndOfDirectiveToken(endOfDirectiveToken);
        }

        public new LoadDirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken)
        {
            return Update(HashToken, LoadKeyword, File, endOfDirectiveToken, IsActive);
        }

        public LoadDirectiveTriviaSyntax WithIsActive(bool isActive)
        {
            return Update(HashToken, LoadKeyword, File, EndOfDirectiveToken, isActive);
        }
    }
}
