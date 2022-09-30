#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class TypeArgumentListSyntax : CSharpSyntaxNode
    {
        private SyntaxNode? arguments;

        public SyntaxToken LessThanToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeArgumentListSyntax)base.Green).lessThanToken, base.Position, 0);

        public SeparatedSyntaxList<TypeSyntax> Arguments
        {
            get
            {
                SyntaxNode red = GetRed(ref arguments, 1);
                if (red == null)
                {
                    return default(SeparatedSyntaxList<TypeSyntax>);
                }
                return new SeparatedSyntaxList<TypeSyntax>(red, GetChildIndex(1));
            }
        }

        public SyntaxToken GreaterThanToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeArgumentListSyntax)base.Green).greaterThanToken, GetChildPosition(2), GetChildIndex(2));

        internal TypeArgumentListSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitTypeArgumentList(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitTypeArgumentList(this);

        public TypeArgumentListSyntax Update(SyntaxToken lessThanToken, SeparatedSyntaxList<TypeSyntax> arguments, SyntaxToken greaterThanToken)
        {
            if (lessThanToken != LessThanToken || arguments != Arguments || greaterThanToken != GreaterThanToken)
            {
                TypeArgumentListSyntax typeArgumentListSyntax = SyntaxFactory.TypeArgumentList(lessThanToken, arguments, greaterThanToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return typeArgumentListSyntax;
                }
                return typeArgumentListSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public TypeArgumentListSyntax WithLessThanToken(SyntaxToken lessThanToken)
        {
            return Update(lessThanToken, Arguments, GreaterThanToken);
        }

        public TypeArgumentListSyntax WithArguments(SeparatedSyntaxList<TypeSyntax> arguments)
        {
            return Update(LessThanToken, arguments, GreaterThanToken);
        }

        public TypeArgumentListSyntax WithGreaterThanToken(SyntaxToken greaterThanToken)
        {
            return Update(LessThanToken, Arguments, greaterThanToken);
        }

        public TypeArgumentListSyntax AddArguments(params TypeSyntax[] items)
        {
            return WithArguments(Arguments.AddRange(items));
        }
    }
}
