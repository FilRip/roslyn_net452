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
    public sealed class CasePatternSwitchLabelSyntax : SwitchLabelSyntax
    {
        private PatternSyntax? pattern;

        private WhenClauseSyntax? whenClause;

        public override SyntaxToken Keyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CasePatternSwitchLabelSyntax)base.Green).keyword, base.Position, 0);

        public PatternSyntax Pattern => GetRed(ref pattern, 1);

        public WhenClauseSyntax? WhenClause => GetRed(ref whenClause, 2);

        public override SyntaxToken ColonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CasePatternSwitchLabelSyntax)base.Green).colonToken, GetChildPosition(3), GetChildIndex(3));

        internal CasePatternSwitchLabelSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                1 => GetRed(ref pattern, 1),
                2 => GetRed(ref whenClause, 2),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                1 => pattern,
                2 => whenClause,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitCasePatternSwitchLabel(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitCasePatternSwitchLabel(this);

        public CasePatternSwitchLabelSyntax Update(SyntaxToken keyword, PatternSyntax pattern, WhenClauseSyntax? whenClause, SyntaxToken colonToken)
        {
            if (keyword != Keyword || pattern != Pattern || whenClause != WhenClause || colonToken != ColonToken)
            {
                CasePatternSwitchLabelSyntax casePatternSwitchLabelSyntax = SyntaxFactory.CasePatternSwitchLabel(keyword, pattern, whenClause, colonToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return casePatternSwitchLabelSyntax;
                }
                return casePatternSwitchLabelSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override SwitchLabelSyntax WithKeywordCore(SyntaxToken keyword)
        {
            return WithKeyword(keyword);
        }

        public new CasePatternSwitchLabelSyntax WithKeyword(SyntaxToken keyword)
        {
            return Update(keyword, Pattern, WhenClause, ColonToken);
        }

        public CasePatternSwitchLabelSyntax WithPattern(PatternSyntax pattern)
        {
            return Update(Keyword, pattern, WhenClause, ColonToken);
        }

        public CasePatternSwitchLabelSyntax WithWhenClause(WhenClauseSyntax? whenClause)
        {
            return Update(Keyword, Pattern, whenClause, ColonToken);
        }

        internal override SwitchLabelSyntax WithColonTokenCore(SyntaxToken colonToken)
        {
            return WithColonToken(colonToken);
        }

        public new CasePatternSwitchLabelSyntax WithColonToken(SyntaxToken colonToken)
        {
            return Update(Keyword, Pattern, WhenClause, colonToken);
        }
    }
}
