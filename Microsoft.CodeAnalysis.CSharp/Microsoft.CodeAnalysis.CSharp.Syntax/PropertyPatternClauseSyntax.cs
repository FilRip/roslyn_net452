#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class PropertyPatternClauseSyntax : CSharpSyntaxNode
    {
        private SyntaxNode? subpatterns;

        public SyntaxToken OpenBraceToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.PropertyPatternClauseSyntax)base.Green).openBraceToken, base.Position, 0);

        public SeparatedSyntaxList<SubpatternSyntax> Subpatterns
        {
            get
            {
                SyntaxNode red = GetRed(ref subpatterns, 1);
                if (red == null)
                {
                    return default(SeparatedSyntaxList<SubpatternSyntax>);
                }
                return new SeparatedSyntaxList<SubpatternSyntax>(red, GetChildIndex(1));
            }
        }

        public SyntaxToken CloseBraceToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.PropertyPatternClauseSyntax)base.Green).closeBraceToken, GetChildPosition(2), GetChildIndex(2));

        internal PropertyPatternClauseSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return GetRed(ref subpatterns, 1);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return subpatterns;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitPropertyPatternClause(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitPropertyPatternClause(this);

        public PropertyPatternClauseSyntax Update(SyntaxToken openBraceToken, SeparatedSyntaxList<SubpatternSyntax> subpatterns, SyntaxToken closeBraceToken)
        {
            if (openBraceToken != OpenBraceToken || subpatterns != Subpatterns || closeBraceToken != CloseBraceToken)
            {
                PropertyPatternClauseSyntax propertyPatternClauseSyntax = SyntaxFactory.PropertyPatternClause(openBraceToken, subpatterns, closeBraceToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return propertyPatternClauseSyntax;
                }
                return propertyPatternClauseSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public PropertyPatternClauseSyntax WithOpenBraceToken(SyntaxToken openBraceToken)
        {
            return Update(openBraceToken, Subpatterns, CloseBraceToken);
        }

        public PropertyPatternClauseSyntax WithSubpatterns(SeparatedSyntaxList<SubpatternSyntax> subpatterns)
        {
            return Update(OpenBraceToken, subpatterns, CloseBraceToken);
        }

        public PropertyPatternClauseSyntax WithCloseBraceToken(SyntaxToken closeBraceToken)
        {
            return Update(OpenBraceToken, Subpatterns, closeBraceToken);
        }

        public PropertyPatternClauseSyntax AddSubpatterns(params SubpatternSyntax[] items)
        {
            return WithSubpatterns(Subpatterns.AddRange(items));
        }
    }
}
