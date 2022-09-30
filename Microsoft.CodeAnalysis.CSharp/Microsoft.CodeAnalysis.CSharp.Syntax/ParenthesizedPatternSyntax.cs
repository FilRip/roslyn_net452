#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ParenthesizedPatternSyntax : PatternSyntax
    {
        private PatternSyntax? pattern;

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ParenthesizedPatternSyntax)base.Green).openParenToken, base.Position, 0);

        public PatternSyntax Pattern => GetRed(ref pattern, 1);

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ParenthesizedPatternSyntax)base.Green).closeParenToken, GetChildPosition(2), GetChildIndex(2));

        internal ParenthesizedPatternSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return GetRed(ref pattern, 1);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return pattern;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitParenthesizedPattern(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitParenthesizedPattern(this);

        public ParenthesizedPatternSyntax Update(SyntaxToken openParenToken, PatternSyntax pattern, SyntaxToken closeParenToken)
        {
            if (openParenToken != OpenParenToken || pattern != Pattern || closeParenToken != CloseParenToken)
            {
                ParenthesizedPatternSyntax parenthesizedPatternSyntax = SyntaxFactory.ParenthesizedPattern(openParenToken, pattern, closeParenToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return parenthesizedPatternSyntax;
                }
                return parenthesizedPatternSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public ParenthesizedPatternSyntax WithOpenParenToken(SyntaxToken openParenToken)
        {
            return Update(openParenToken, Pattern, CloseParenToken);
        }

        public ParenthesizedPatternSyntax WithPattern(PatternSyntax pattern)
        {
            return Update(OpenParenToken, pattern, CloseParenToken);
        }

        public ParenthesizedPatternSyntax WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return Update(OpenParenToken, Pattern, closeParenToken);
        }
    }
}
