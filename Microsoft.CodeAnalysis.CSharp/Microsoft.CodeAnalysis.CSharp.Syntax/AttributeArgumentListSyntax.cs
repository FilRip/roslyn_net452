#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class AttributeArgumentListSyntax : CSharpSyntaxNode
    {
        private SyntaxNode? arguments;

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeArgumentListSyntax)base.Green).openParenToken, base.Position, 0);

        public SeparatedSyntaxList<AttributeArgumentSyntax> Arguments
        {
            get
            {
                SyntaxNode red = GetRed(ref arguments, 1);
                if (red == null)
                {
                    return default(SeparatedSyntaxList<AttributeArgumentSyntax>);
                }
                return new SeparatedSyntaxList<AttributeArgumentSyntax>(red, GetChildIndex(1));
            }
        }

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AttributeArgumentListSyntax)base.Green).closeParenToken, GetChildPosition(2), GetChildIndex(2));

        internal AttributeArgumentListSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return GetRed(ref arguments, 1);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return arguments;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitAttributeArgumentList(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitAttributeArgumentList(this);

        public AttributeArgumentListSyntax Update(SyntaxToken openParenToken, SeparatedSyntaxList<AttributeArgumentSyntax> arguments, SyntaxToken closeParenToken)
        {
            if (openParenToken != OpenParenToken || arguments != Arguments || closeParenToken != CloseParenToken)
            {
                AttributeArgumentListSyntax attributeArgumentListSyntax = SyntaxFactory.AttributeArgumentList(openParenToken, arguments, closeParenToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return attributeArgumentListSyntax;
                }
                return attributeArgumentListSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public AttributeArgumentListSyntax WithOpenParenToken(SyntaxToken openParenToken)
        {
            return Update(openParenToken, Arguments, CloseParenToken);
        }

        public AttributeArgumentListSyntax WithArguments(SeparatedSyntaxList<AttributeArgumentSyntax> arguments)
        {
            return Update(OpenParenToken, arguments, CloseParenToken);
        }

        public AttributeArgumentListSyntax WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return Update(OpenParenToken, Arguments, closeParenToken);
        }

        public AttributeArgumentListSyntax AddArguments(params AttributeArgumentSyntax[] items)
        {
            return WithArguments(Arguments.AddRange(items));
        }
    }
}
