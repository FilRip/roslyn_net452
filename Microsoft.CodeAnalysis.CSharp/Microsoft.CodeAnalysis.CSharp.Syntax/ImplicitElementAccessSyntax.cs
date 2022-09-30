#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ImplicitElementAccessSyntax : ExpressionSyntax
    {
        private BracketedArgumentListSyntax? argumentList;

        public BracketedArgumentListSyntax ArgumentList => GetRedAtZero(ref argumentList);

        internal ImplicitElementAccessSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return GetRedAtZero(ref argumentList);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return argumentList;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitImplicitElementAccess(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitImplicitElementAccess(this);

        public ImplicitElementAccessSyntax Update(BracketedArgumentListSyntax argumentList)
        {
            if (argumentList != ArgumentList)
            {
                ImplicitElementAccessSyntax implicitElementAccessSyntax = SyntaxFactory.ImplicitElementAccess(argumentList);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return implicitElementAccessSyntax;
                }
                return implicitElementAccessSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public ImplicitElementAccessSyntax WithArgumentList(BracketedArgumentListSyntax argumentList)
        {
            return Update(argumentList);
        }

        public ImplicitElementAccessSyntax AddArgumentListArguments(params ArgumentSyntax[] items)
        {
            return WithArgumentList(ArgumentList.WithArguments(ArgumentList.Arguments.AddRange(items)));
        }
    }
}
