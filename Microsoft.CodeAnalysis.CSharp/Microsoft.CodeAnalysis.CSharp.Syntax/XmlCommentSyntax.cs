#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class XmlCommentSyntax : XmlNodeSyntax
    {
        public SyntaxToken LessThanExclamationMinusMinusToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.XmlCommentSyntax)base.Green).lessThanExclamationMinusMinusToken, base.Position, 0);

        public SyntaxTokenList TextTokens
        {
            get
            {
                GreenNode slot = base.Green.GetSlot(1);
                if (slot == null)
                {
                    return default(SyntaxTokenList);
                }
                return new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1));
            }
        }

        public SyntaxToken MinusMinusGreaterThanToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.XmlCommentSyntax)base.Green).minusMinusGreaterThanToken, GetChildPosition(2), GetChildIndex(2));

        internal XmlCommentSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitXmlComment(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitXmlComment(this);

        public XmlCommentSyntax Update(SyntaxToken lessThanExclamationMinusMinusToken, SyntaxTokenList textTokens, SyntaxToken minusMinusGreaterThanToken)
        {
            if (lessThanExclamationMinusMinusToken != LessThanExclamationMinusMinusToken || textTokens != TextTokens || minusMinusGreaterThanToken != MinusMinusGreaterThanToken)
            {
                XmlCommentSyntax xmlCommentSyntax = SyntaxFactory.XmlComment(lessThanExclamationMinusMinusToken, textTokens, minusMinusGreaterThanToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return xmlCommentSyntax;
                }
                return xmlCommentSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public XmlCommentSyntax WithLessThanExclamationMinusMinusToken(SyntaxToken lessThanExclamationMinusMinusToken)
        {
            return Update(lessThanExclamationMinusMinusToken, TextTokens, MinusMinusGreaterThanToken);
        }

        public XmlCommentSyntax WithTextTokens(SyntaxTokenList textTokens)
        {
            return Update(LessThanExclamationMinusMinusToken, textTokens, MinusMinusGreaterThanToken);
        }

        public XmlCommentSyntax WithMinusMinusGreaterThanToken(SyntaxToken minusMinusGreaterThanToken)
        {
            return Update(LessThanExclamationMinusMinusToken, TextTokens, minusMinusGreaterThanToken);
        }

        public XmlCommentSyntax AddTextTokens(params SyntaxToken[] items)
        {
            return WithTextTokens(TextTokens.AddRange(items));
        }
    }
}
