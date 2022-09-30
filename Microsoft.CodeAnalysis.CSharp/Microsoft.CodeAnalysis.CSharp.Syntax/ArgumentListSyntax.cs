#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ArgumentListSyntax : BaseArgumentListSyntax
    {
        private SyntaxNode? arguments;

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ArgumentListSyntax)base.Green).openParenToken, base.Position, 0);

        public override SeparatedSyntaxList<ArgumentSyntax> Arguments
        {
            get
            {
                SyntaxNode red = GetRed(ref arguments, 1);
                if (red == null)
                {
                    return default(SeparatedSyntaxList<ArgumentSyntax>);
                }
                return new SeparatedSyntaxList<ArgumentSyntax>(red, GetChildIndex(1));
            }
        }

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ArgumentListSyntax)base.Green).closeParenToken, GetChildPosition(2), GetChildIndex(2));

        internal ArgumentListSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitArgumentList(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitArgumentList(this);

        public ArgumentListSyntax Update(SyntaxToken openParenToken, SeparatedSyntaxList<ArgumentSyntax> arguments, SyntaxToken closeParenToken)
        {
            if (openParenToken != OpenParenToken || arguments != Arguments || closeParenToken != CloseParenToken)
            {
                ArgumentListSyntax argumentListSyntax = SyntaxFactory.ArgumentList(openParenToken, arguments, closeParenToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return argumentListSyntax;
                }
                return argumentListSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public ArgumentListSyntax WithOpenParenToken(SyntaxToken openParenToken)
        {
            return Update(openParenToken, Arguments, CloseParenToken);
        }

        internal override BaseArgumentListSyntax WithArgumentsCore(SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            return WithArguments(arguments);
        }

        public new ArgumentListSyntax WithArguments(SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            return Update(OpenParenToken, arguments, CloseParenToken);
        }

        public ArgumentListSyntax WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return Update(OpenParenToken, Arguments, closeParenToken);
        }

        internal override BaseArgumentListSyntax AddArgumentsCore(params ArgumentSyntax[] items)
        {
            return AddArguments(items);
        }

        public new ArgumentListSyntax AddArguments(params ArgumentSyntax[] items)
        {
            return WithArguments(Arguments.AddRange(items));
        }
    }
}
