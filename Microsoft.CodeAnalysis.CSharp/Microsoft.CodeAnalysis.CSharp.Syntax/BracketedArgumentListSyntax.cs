#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class BracketedArgumentListSyntax : BaseArgumentListSyntax
    {
        private SyntaxNode? arguments;

        public SyntaxToken OpenBracketToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BracketedArgumentListSyntax)base.Green).openBracketToken, base.Position, 0);

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

        public SyntaxToken CloseBracketToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BracketedArgumentListSyntax)base.Green).closeBracketToken, GetChildPosition(2), GetChildIndex(2));

        internal BracketedArgumentListSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitBracketedArgumentList(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitBracketedArgumentList(this);

        public BracketedArgumentListSyntax Update(SyntaxToken openBracketToken, SeparatedSyntaxList<ArgumentSyntax> arguments, SyntaxToken closeBracketToken)
        {
            if (openBracketToken != OpenBracketToken || arguments != Arguments || closeBracketToken != CloseBracketToken)
            {
                BracketedArgumentListSyntax bracketedArgumentListSyntax = SyntaxFactory.BracketedArgumentList(openBracketToken, arguments, closeBracketToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return bracketedArgumentListSyntax;
                }
                return bracketedArgumentListSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public BracketedArgumentListSyntax WithOpenBracketToken(SyntaxToken openBracketToken)
        {
            return Update(openBracketToken, Arguments, CloseBracketToken);
        }

        internal override BaseArgumentListSyntax WithArgumentsCore(SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            return WithArguments(arguments);
        }

        public new BracketedArgumentListSyntax WithArguments(SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            return Update(OpenBracketToken, arguments, CloseBracketToken);
        }

        public BracketedArgumentListSyntax WithCloseBracketToken(SyntaxToken closeBracketToken)
        {
            return Update(OpenBracketToken, Arguments, closeBracketToken);
        }

        internal override BaseArgumentListSyntax AddArgumentsCore(params ArgumentSyntax[] items)
        {
            return AddArguments(items);
        }

        public new BracketedArgumentListSyntax AddArguments(params ArgumentSyntax[] items)
        {
            return WithArguments(Arguments.AddRange(items));
        }
    }
}
