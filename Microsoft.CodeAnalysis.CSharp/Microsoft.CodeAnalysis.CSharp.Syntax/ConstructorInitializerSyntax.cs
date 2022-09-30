#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ConstructorInitializerSyntax : CSharpSyntaxNode
    {
        private ArgumentListSyntax? argumentList;

        public SyntaxToken ColonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ConstructorInitializerSyntax)base.Green).colonToken, base.Position, 0);

        public SyntaxToken ThisOrBaseKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ConstructorInitializerSyntax)base.Green).thisOrBaseKeyword, GetChildPosition(1), GetChildIndex(1));

        public ArgumentListSyntax ArgumentList => GetRed(ref argumentList, 2);

        internal ConstructorInitializerSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 2)
            {
                return null;
            }
            return GetRed(ref argumentList, 2);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 2)
            {
                return null;
            }
            return argumentList;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitConstructorInitializer(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitConstructorInitializer(this);

        public ConstructorInitializerSyntax Update(SyntaxToken colonToken, SyntaxToken thisOrBaseKeyword, ArgumentListSyntax argumentList)
        {
            if (colonToken != ColonToken || thisOrBaseKeyword != ThisOrBaseKeyword || argumentList != ArgumentList)
            {
                ConstructorInitializerSyntax constructorInitializerSyntax = SyntaxFactory.ConstructorInitializer(Kind(), colonToken, thisOrBaseKeyword, argumentList);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return constructorInitializerSyntax;
                }
                return constructorInitializerSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public ConstructorInitializerSyntax WithColonToken(SyntaxToken colonToken)
        {
            return Update(colonToken, ThisOrBaseKeyword, ArgumentList);
        }

        public ConstructorInitializerSyntax WithThisOrBaseKeyword(SyntaxToken thisOrBaseKeyword)
        {
            return Update(ColonToken, thisOrBaseKeyword, ArgumentList);
        }

        public ConstructorInitializerSyntax WithArgumentList(ArgumentListSyntax argumentList)
        {
            return Update(ColonToken, ThisOrBaseKeyword, argumentList);
        }

        public ConstructorInitializerSyntax AddArgumentListArguments(params ArgumentSyntax[] items)
        {
            return WithArgumentList(ArgumentList.WithArguments(ArgumentList.Arguments.AddRange(items)));
        }
    }
}
