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
    public sealed class SubpatternSyntax : CSharpSyntaxNode
    {
        private NameColonSyntax? nameColon;

        private PatternSyntax? pattern;

        public NameColonSyntax? NameColon => GetRedAtZero(ref nameColon);

        public PatternSyntax Pattern => GetRed(ref pattern, 1);

        internal SubpatternSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref nameColon),
                1 => GetRed(ref pattern, 1),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => nameColon,
                1 => pattern,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitSubpattern(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitSubpattern(this);

        public SubpatternSyntax Update(NameColonSyntax? nameColon, PatternSyntax pattern)
        {
            if (nameColon != NameColon || pattern != Pattern)
            {
                SubpatternSyntax subpatternSyntax = SyntaxFactory.Subpattern(nameColon, pattern);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return subpatternSyntax;
                }
                return subpatternSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public SubpatternSyntax WithNameColon(NameColonSyntax? nameColon)
        {
            return Update(nameColon, Pattern);
        }

        public SubpatternSyntax WithPattern(PatternSyntax pattern)
        {
            return Update(NameColon, pattern);
        }
    }
}
