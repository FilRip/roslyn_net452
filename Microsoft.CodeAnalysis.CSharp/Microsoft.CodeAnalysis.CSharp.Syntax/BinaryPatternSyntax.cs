#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class BinaryPatternSyntax : PatternSyntax
    {
        private PatternSyntax? left;

        private PatternSyntax? right;

        public PatternSyntax Left => GetRedAtZero(ref left);

        public SyntaxToken OperatorToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BinaryPatternSyntax)base.Green).operatorToken, GetChildPosition(1), GetChildIndex(1));

        public PatternSyntax Right => GetRed(ref right, 2);

        internal BinaryPatternSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref left),
                2 => GetRed(ref right, 2),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => left,
                2 => right,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitBinaryPattern(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitBinaryPattern(this);

        public BinaryPatternSyntax Update(PatternSyntax left, SyntaxToken operatorToken, PatternSyntax right)
        {
            if (left != Left || operatorToken != OperatorToken || right != Right)
            {
                BinaryPatternSyntax binaryPatternSyntax = SyntaxFactory.BinaryPattern(Kind(), left, operatorToken, right);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return binaryPatternSyntax;
                }
                return binaryPatternSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public BinaryPatternSyntax WithLeft(PatternSyntax left)
        {
            return Update(left, OperatorToken, Right);
        }

        public BinaryPatternSyntax WithOperatorToken(SyntaxToken operatorToken)
        {
            return Update(Left, operatorToken, Right);
        }

        public BinaryPatternSyntax WithRight(PatternSyntax right)
        {
            return Update(Left, OperatorToken, right);
        }
    }
}
