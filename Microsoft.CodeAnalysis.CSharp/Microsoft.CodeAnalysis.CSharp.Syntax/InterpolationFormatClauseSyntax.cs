#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class InterpolationFormatClauseSyntax : CSharpSyntaxNode
    {
        public SyntaxToken ColonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.InterpolationFormatClauseSyntax)base.Green).colonToken, base.Position, 0);

        public SyntaxToken FormatStringToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.InterpolationFormatClauseSyntax)base.Green).formatStringToken, GetChildPosition(1), GetChildIndex(1));

        internal InterpolationFormatClauseSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitInterpolationFormatClause(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitInterpolationFormatClause(this);

        public InterpolationFormatClauseSyntax Update(SyntaxToken colonToken, SyntaxToken formatStringToken)
        {
            if (colonToken != ColonToken || formatStringToken != FormatStringToken)
            {
                InterpolationFormatClauseSyntax interpolationFormatClauseSyntax = SyntaxFactory.InterpolationFormatClause(colonToken, formatStringToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return interpolationFormatClauseSyntax;
                }
                return interpolationFormatClauseSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public InterpolationFormatClauseSyntax WithColonToken(SyntaxToken colonToken)
        {
            return Update(colonToken, FormatStringToken);
        }

        public InterpolationFormatClauseSyntax WithFormatStringToken(SyntaxToken formatStringToken)
        {
            return Update(ColonToken, formatStringToken);
        }
    }
}
