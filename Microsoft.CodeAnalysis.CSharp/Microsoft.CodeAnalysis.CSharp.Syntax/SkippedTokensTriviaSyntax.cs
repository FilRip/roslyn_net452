#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class SkippedTokensTriviaSyntax : StructuredTriviaSyntax, ISkippedTokensTriviaSyntax
    {
        public SyntaxTokenList Tokens
        {
            get
            {
                GreenNode slot = base.Green.GetSlot(0);
                if (slot == null)
                {
                    return default(SyntaxTokenList);
                }
                return new SyntaxTokenList(this, slot, base.Position, 0);
            }
        }

        internal SkippedTokensTriviaSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitSkippedTokensTrivia(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitSkippedTokensTrivia(this);

        public SkippedTokensTriviaSyntax Update(SyntaxTokenList tokens)
        {
            if (tokens != Tokens)
            {
                SkippedTokensTriviaSyntax skippedTokensTriviaSyntax = SyntaxFactory.SkippedTokensTrivia(tokens);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return skippedTokensTriviaSyntax;
                }
                return skippedTokensTriviaSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public SkippedTokensTriviaSyntax WithTokens(SyntaxTokenList tokens)
        {
            return Update(tokens);
        }

        public SkippedTokensTriviaSyntax AddTokens(params SyntaxToken[] items)
        {
            return WithTokens(Tokens.AddRange(items));
        }
    }
}
