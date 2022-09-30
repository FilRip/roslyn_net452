#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class StackAllocArrayCreationExpressionSyntax : ExpressionSyntax
    {
        private TypeSyntax? type;

        private InitializerExpressionSyntax? initializer;

        public SyntaxToken StackAllocKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.StackAllocArrayCreationExpressionSyntax)base.Green).stackAllocKeyword, base.Position, 0);

        public TypeSyntax Type => GetRed(ref type, 1);

        public InitializerExpressionSyntax? Initializer => GetRed(ref initializer, 2);

        public StackAllocArrayCreationExpressionSyntax Update(SyntaxToken stackAllocKeyword, TypeSyntax type)
        {
            return Update(StackAllocKeyword, type, Initializer);
        }

        internal StackAllocArrayCreationExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                1 => GetRed(ref type, 1),
                2 => GetRed(ref initializer, 2),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                1 => type,
                2 => initializer,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitStackAllocArrayCreationExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitStackAllocArrayCreationExpression(this);

        public StackAllocArrayCreationExpressionSyntax Update(SyntaxToken stackAllocKeyword, TypeSyntax type, InitializerExpressionSyntax? initializer)
        {
            if (stackAllocKeyword != StackAllocKeyword || type != Type || initializer != Initializer)
            {
                StackAllocArrayCreationExpressionSyntax stackAllocArrayCreationExpressionSyntax = SyntaxFactory.StackAllocArrayCreationExpression(stackAllocKeyword, type, initializer);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return stackAllocArrayCreationExpressionSyntax;
                }
                return stackAllocArrayCreationExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public StackAllocArrayCreationExpressionSyntax WithStackAllocKeyword(SyntaxToken stackAllocKeyword)
        {
            return Update(stackAllocKeyword, Type, Initializer);
        }

        public StackAllocArrayCreationExpressionSyntax WithType(TypeSyntax type)
        {
            return Update(StackAllocKeyword, type, Initializer);
        }

        public StackAllocArrayCreationExpressionSyntax WithInitializer(InitializerExpressionSyntax? initializer)
        {
            return Update(StackAllocKeyword, Type, initializer);
        }
    }
}
