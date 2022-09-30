#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ElementBindingExpressionSyntax : ExpressionSyntax
    {
        private BracketedArgumentListSyntax? argumentList;

        public BracketedArgumentListSyntax ArgumentList => GetRedAtZero(ref argumentList);

        internal ElementBindingExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitElementBindingExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitElementBindingExpression(this);

        public ElementBindingExpressionSyntax Update(BracketedArgumentListSyntax argumentList)
        {
            if (argumentList != ArgumentList)
            {
                ElementBindingExpressionSyntax elementBindingExpressionSyntax = SyntaxFactory.ElementBindingExpression(argumentList);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return elementBindingExpressionSyntax;
                }
                return elementBindingExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public ElementBindingExpressionSyntax WithArgumentList(BracketedArgumentListSyntax argumentList)
        {
            return Update(argumentList);
        }

        public ElementBindingExpressionSyntax AddArgumentListArguments(params ArgumentSyntax[] items)
        {
            return WithArgumentList(ArgumentList.WithArguments(ArgumentList.Arguments.AddRange(items)));
        }
    }
}
