#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class PositionalPatternClauseSyntax : CSharpSyntaxNode
    {
        private SyntaxNode? subpatterns;

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.PositionalPatternClauseSyntax)base.Green).openParenToken, base.Position, 0);

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

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.PositionalPatternClauseSyntax)base.Green).closeParenToken, GetChildPosition(2), GetChildIndex(2));

        internal PositionalPatternClauseSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitPositionalPatternClause(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitPositionalPatternClause(this);

        public PositionalPatternClauseSyntax Update(SyntaxToken openParenToken, SeparatedSyntaxList<SubpatternSyntax> subpatterns, SyntaxToken closeParenToken)
        {
            if (openParenToken != OpenParenToken || subpatterns != Subpatterns || closeParenToken != CloseParenToken)
            {
                PositionalPatternClauseSyntax positionalPatternClauseSyntax = SyntaxFactory.PositionalPatternClause(openParenToken, subpatterns, closeParenToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return positionalPatternClauseSyntax;
                }
                return positionalPatternClauseSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public PositionalPatternClauseSyntax WithOpenParenToken(SyntaxToken openParenToken)
        {
            return Update(openParenToken, Subpatterns, CloseParenToken);
        }

        public PositionalPatternClauseSyntax WithSubpatterns(SeparatedSyntaxList<SubpatternSyntax> subpatterns)
        {
            return Update(OpenParenToken, subpatterns, CloseParenToken);
        }

        public PositionalPatternClauseSyntax WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return Update(OpenParenToken, Subpatterns, closeParenToken);
        }

        public PositionalPatternClauseSyntax AddSubpatterns(params SubpatternSyntax[] items)
        {
            return WithSubpatterns(Subpatterns.AddRange(items));
        }
    }
}
