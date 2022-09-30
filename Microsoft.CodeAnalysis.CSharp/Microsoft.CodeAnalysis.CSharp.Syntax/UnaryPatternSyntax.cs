#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class UnaryPatternSyntax : PatternSyntax
    {
        private PatternSyntax? pattern;

        public SyntaxToken OperatorToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.UnaryPatternSyntax)base.Green).operatorToken, base.Position, 0);

        public PatternSyntax Pattern => GetRed(ref pattern, 1);

        internal UnaryPatternSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitUnaryPattern(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitUnaryPattern(this);

        public UnaryPatternSyntax Update(SyntaxToken operatorToken, PatternSyntax pattern)
        {
            if (operatorToken != OperatorToken || pattern != Pattern)
            {
                UnaryPatternSyntax unaryPatternSyntax = SyntaxFactory.UnaryPattern(operatorToken, pattern);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return unaryPatternSyntax;
                }
                return unaryPatternSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public UnaryPatternSyntax WithOperatorToken(SyntaxToken operatorToken)
        {
            return Update(operatorToken, Pattern);
        }

        public UnaryPatternSyntax WithPattern(PatternSyntax pattern)
        {
            return Update(OperatorToken, pattern);
        }
    }
}
