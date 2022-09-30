#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class DefineDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {
        public override SyntaxToken HashToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DefineDirectiveTriviaSyntax)base.Green).hashToken, base.Position, 0);

        public SyntaxToken DefineKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DefineDirectiveTriviaSyntax)base.Green).defineKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken Name => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DefineDirectiveTriviaSyntax)base.Green).name, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxToken EndOfDirectiveToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DefineDirectiveTriviaSyntax)base.Green).endOfDirectiveToken, GetChildPosition(3), GetChildIndex(3));

        public override bool IsActive => ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DefineDirectiveTriviaSyntax)base.Green).IsActive;

        internal DefineDirectiveTriviaSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitDefineDirectiveTrivia(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitDefineDirectiveTrivia(this);

        public DefineDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken defineKeyword, SyntaxToken name, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken != HashToken || defineKeyword != DefineKeyword || name != Name || endOfDirectiveToken != EndOfDirectiveToken)
            {
                DefineDirectiveTriviaSyntax defineDirectiveTriviaSyntax = SyntaxFactory.DefineDirectiveTrivia(hashToken, defineKeyword, name, endOfDirectiveToken, isActive);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return defineDirectiveTriviaSyntax;
                }
                return defineDirectiveTriviaSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken)
        {
            return WithHashToken(hashToken);
        }

        public new DefineDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken)
        {
            return Update(hashToken, DefineKeyword, Name, EndOfDirectiveToken, IsActive);
        }

        public DefineDirectiveTriviaSyntax WithDefineKeyword(SyntaxToken defineKeyword)
        {
            return Update(HashToken, defineKeyword, Name, EndOfDirectiveToken, IsActive);
        }

        public DefineDirectiveTriviaSyntax WithName(SyntaxToken name)
        {
            return Update(HashToken, DefineKeyword, name, EndOfDirectiveToken, IsActive);
        }

        internal override DirectiveTriviaSyntax WithEndOfDirectiveTokenCore(SyntaxToken endOfDirectiveToken)
        {
            return WithEndOfDirectiveToken(endOfDirectiveToken);
        }

        public new DefineDirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken)
        {
            return Update(HashToken, DefineKeyword, Name, endOfDirectiveToken, IsActive);
        }

        public DefineDirectiveTriviaSyntax WithIsActive(bool isActive)
        {
            return Update(HashToken, DefineKeyword, Name, EndOfDirectiveToken, isActive);
        }
    }
}
