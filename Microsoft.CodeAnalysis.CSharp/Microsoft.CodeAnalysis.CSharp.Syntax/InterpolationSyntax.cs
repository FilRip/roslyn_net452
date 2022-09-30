#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class InterpolationSyntax : InterpolatedStringContentSyntax
    {
        private ExpressionSyntax? expression;

        private InterpolationAlignmentClauseSyntax? alignmentClause;

        private InterpolationFormatClauseSyntax? formatClause;

        public SyntaxToken OpenBraceToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.InterpolationSyntax)base.Green).openBraceToken, base.Position, 0);

        public ExpressionSyntax Expression => GetRed(ref expression, 1);

        public InterpolationAlignmentClauseSyntax? AlignmentClause => GetRed(ref alignmentClause, 2);

        public InterpolationFormatClauseSyntax? FormatClause => GetRed(ref formatClause, 3);

        public SyntaxToken CloseBraceToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.InterpolationSyntax)base.Green).closeBraceToken, GetChildPosition(4), GetChildIndex(4));

        internal InterpolationSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                1 => GetRed(ref expression, 1),
                2 => GetRed(ref alignmentClause, 2),
                3 => GetRed(ref formatClause, 3),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                1 => expression,
                2 => alignmentClause,
                3 => formatClause,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitInterpolation(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitInterpolation(this);

        public InterpolationSyntax Update(SyntaxToken openBraceToken, ExpressionSyntax expression, InterpolationAlignmentClauseSyntax? alignmentClause, InterpolationFormatClauseSyntax? formatClause, SyntaxToken closeBraceToken)
        {
            if (openBraceToken != OpenBraceToken || expression != Expression || alignmentClause != AlignmentClause || formatClause != FormatClause || closeBraceToken != CloseBraceToken)
            {
                InterpolationSyntax interpolationSyntax = SyntaxFactory.Interpolation(openBraceToken, expression, alignmentClause, formatClause, closeBraceToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return interpolationSyntax;
                }
                return interpolationSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public InterpolationSyntax WithOpenBraceToken(SyntaxToken openBraceToken)
        {
            return Update(openBraceToken, Expression, AlignmentClause, FormatClause, CloseBraceToken);
        }

        public InterpolationSyntax WithExpression(ExpressionSyntax expression)
        {
            return Update(OpenBraceToken, expression, AlignmentClause, FormatClause, CloseBraceToken);
        }

        public InterpolationSyntax WithAlignmentClause(InterpolationAlignmentClauseSyntax? alignmentClause)
        {
            return Update(OpenBraceToken, Expression, alignmentClause, FormatClause, CloseBraceToken);
        }

        public InterpolationSyntax WithFormatClause(InterpolationFormatClauseSyntax? formatClause)
        {
            return Update(OpenBraceToken, Expression, AlignmentClause, formatClause, CloseBraceToken);
        }

        public InterpolationSyntax WithCloseBraceToken(SyntaxToken closeBraceToken)
        {
            return Update(OpenBraceToken, Expression, AlignmentClause, FormatClause, closeBraceToken);
        }
    }
}
