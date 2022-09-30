#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class InterpolatedStringExpressionSyntax : ExpressionSyntax
    {
        private SyntaxNode? contents;

        public SyntaxToken StringStartToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax)base.Green).stringStartToken, base.Position, 0);

        public SyntaxList<InterpolatedStringContentSyntax> Contents => new SyntaxList<InterpolatedStringContentSyntax>(GetRed(ref contents, 1));

        public SyntaxToken StringEndToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax)base.Green).stringEndToken, GetChildPosition(2), GetChildIndex(2));

        internal InterpolatedStringExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return GetRed(ref contents, 1);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return contents;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitInterpolatedStringExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitInterpolatedStringExpression(this);

        public InterpolatedStringExpressionSyntax Update(SyntaxToken stringStartToken, SyntaxList<InterpolatedStringContentSyntax> contents, SyntaxToken stringEndToken)
        {
            if (stringStartToken != StringStartToken || contents != Contents || stringEndToken != StringEndToken)
            {
                InterpolatedStringExpressionSyntax interpolatedStringExpressionSyntax = SyntaxFactory.InterpolatedStringExpression(stringStartToken, contents, stringEndToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return interpolatedStringExpressionSyntax;
                }
                return interpolatedStringExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public InterpolatedStringExpressionSyntax WithStringStartToken(SyntaxToken stringStartToken)
        {
            return Update(stringStartToken, Contents, StringEndToken);
        }

        public InterpolatedStringExpressionSyntax WithContents(SyntaxList<InterpolatedStringContentSyntax> contents)
        {
            return Update(StringStartToken, contents, StringEndToken);
        }

        public InterpolatedStringExpressionSyntax WithStringEndToken(SyntaxToken stringEndToken)
        {
            return Update(StringStartToken, Contents, stringEndToken);
        }

        public InterpolatedStringExpressionSyntax AddContents(params InterpolatedStringContentSyntax[] items)
        {
            return WithContents(Contents.AddRange(items));
        }
    }
}
