#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class CaseSwitchLabelSyntax : SwitchLabelSyntax
    {
        private ExpressionSyntax? value;

        public override SyntaxToken Keyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CaseSwitchLabelSyntax)base.Green).keyword, base.Position, 0);

        public ExpressionSyntax Value => GetRed(ref value, 1);

        public override SyntaxToken ColonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CaseSwitchLabelSyntax)base.Green).colonToken, GetChildPosition(2), GetChildIndex(2));

        internal CaseSwitchLabelSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return GetRed(ref value, 1);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return value;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitCaseSwitchLabel(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitCaseSwitchLabel(this);

        public CaseSwitchLabelSyntax Update(SyntaxToken keyword, ExpressionSyntax value, SyntaxToken colonToken)
        {
            if (keyword != Keyword || value != Value || colonToken != ColonToken)
            {
                CaseSwitchLabelSyntax caseSwitchLabelSyntax = SyntaxFactory.CaseSwitchLabel(keyword, value, colonToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return caseSwitchLabelSyntax;
                }
                return caseSwitchLabelSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override SwitchLabelSyntax WithKeywordCore(SyntaxToken keyword)
        {
            return WithKeyword(keyword);
        }

        public new CaseSwitchLabelSyntax WithKeyword(SyntaxToken keyword)
        {
            return Update(keyword, Value, ColonToken);
        }

        public CaseSwitchLabelSyntax WithValue(ExpressionSyntax value)
        {
            return Update(Keyword, value, ColonToken);
        }

        internal override SwitchLabelSyntax WithColonTokenCore(SyntaxToken colonToken)
        {
            return WithColonToken(colonToken);
        }

        public new CaseSwitchLabelSyntax WithColonToken(SyntaxToken colonToken)
        {
            return Update(Keyword, Value, colonToken);
        }
    }
}
