#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ImplicitStackAllocArrayCreationExpressionSyntax : ExpressionSyntax
    {
        private InitializerExpressionSyntax? initializer;

        public SyntaxToken StackAllocKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ImplicitStackAllocArrayCreationExpressionSyntax)base.Green).stackAllocKeyword, base.Position, 0);

        public SyntaxToken OpenBracketToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ImplicitStackAllocArrayCreationExpressionSyntax)base.Green).openBracketToken, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken CloseBracketToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ImplicitStackAllocArrayCreationExpressionSyntax)base.Green).closeBracketToken, GetChildPosition(2), GetChildIndex(2));

        public InitializerExpressionSyntax Initializer => GetRed(ref initializer, 3);

        internal ImplicitStackAllocArrayCreationExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 3)
            {
                return null;
            }
            return GetRed(ref initializer, 3);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 3)
            {
                return null;
            }
            return initializer;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitImplicitStackAllocArrayCreationExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitImplicitStackAllocArrayCreationExpression(this);

        public ImplicitStackAllocArrayCreationExpressionSyntax Update(SyntaxToken stackAllocKeyword, SyntaxToken openBracketToken, SyntaxToken closeBracketToken, InitializerExpressionSyntax initializer)
        {
            if (stackAllocKeyword != StackAllocKeyword || openBracketToken != OpenBracketToken || closeBracketToken != CloseBracketToken || initializer != Initializer)
            {
                ImplicitStackAllocArrayCreationExpressionSyntax implicitStackAllocArrayCreationExpressionSyntax = SyntaxFactory.ImplicitStackAllocArrayCreationExpression(stackAllocKeyword, openBracketToken, closeBracketToken, initializer);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return implicitStackAllocArrayCreationExpressionSyntax;
                }
                return implicitStackAllocArrayCreationExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public ImplicitStackAllocArrayCreationExpressionSyntax WithStackAllocKeyword(SyntaxToken stackAllocKeyword)
        {
            return Update(stackAllocKeyword, OpenBracketToken, CloseBracketToken, Initializer);
        }

        public ImplicitStackAllocArrayCreationExpressionSyntax WithOpenBracketToken(SyntaxToken openBracketToken)
        {
            return Update(StackAllocKeyword, openBracketToken, CloseBracketToken, Initializer);
        }

        public ImplicitStackAllocArrayCreationExpressionSyntax WithCloseBracketToken(SyntaxToken closeBracketToken)
        {
            return Update(StackAllocKeyword, OpenBracketToken, closeBracketToken, Initializer);
        }

        public ImplicitStackAllocArrayCreationExpressionSyntax WithInitializer(InitializerExpressionSyntax initializer)
        {
            return Update(StackAllocKeyword, OpenBracketToken, CloseBracketToken, initializer);
        }

        public ImplicitStackAllocArrayCreationExpressionSyntax AddInitializerExpressions(params ExpressionSyntax[] items)
        {
            return WithInitializer(Initializer.WithExpressions(Initializer.Expressions.AddRange(items)));
        }
    }
}
