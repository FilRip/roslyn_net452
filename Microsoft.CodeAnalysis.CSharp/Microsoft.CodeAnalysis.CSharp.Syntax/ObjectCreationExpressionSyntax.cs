#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ObjectCreationExpressionSyntax : BaseObjectCreationExpressionSyntax
    {
        private TypeSyntax? type;

        private ArgumentListSyntax? argumentList;

        private InitializerExpressionSyntax? initializer;

        public override SyntaxToken NewKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ObjectCreationExpressionSyntax)base.Green).newKeyword, base.Position, 0);

        public TypeSyntax Type => GetRed(ref type, 1);

        public override ArgumentListSyntax? ArgumentList => GetRed(ref argumentList, 2);

        public override InitializerExpressionSyntax? Initializer => GetRed(ref initializer, 3);

        internal ObjectCreationExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                1 => GetRed(ref type, 1),
                2 => GetRed(ref argumentList, 2),
                3 => GetRed(ref initializer, 3),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                1 => type,
                2 => argumentList,
                3 => initializer,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitObjectCreationExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitObjectCreationExpression(this);

        public ObjectCreationExpressionSyntax Update(SyntaxToken newKeyword, TypeSyntax type, ArgumentListSyntax? argumentList, InitializerExpressionSyntax? initializer)
        {
            if (newKeyword != NewKeyword || type != Type || argumentList != ArgumentList || initializer != Initializer)
            {
                ObjectCreationExpressionSyntax objectCreationExpressionSyntax = SyntaxFactory.ObjectCreationExpression(newKeyword, type, argumentList, initializer);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return objectCreationExpressionSyntax;
                }
                return objectCreationExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override BaseObjectCreationExpressionSyntax WithNewKeywordCore(SyntaxToken newKeyword)
        {
            return WithNewKeyword(newKeyword);
        }

        public new ObjectCreationExpressionSyntax WithNewKeyword(SyntaxToken newKeyword)
        {
            return Update(newKeyword, Type, ArgumentList, Initializer);
        }

        public ObjectCreationExpressionSyntax WithType(TypeSyntax type)
        {
            return Update(NewKeyword, type, ArgumentList, Initializer);
        }

        internal override BaseObjectCreationExpressionSyntax WithArgumentListCore(ArgumentListSyntax? argumentList)
        {
            return WithArgumentList(argumentList);
        }

        public new ObjectCreationExpressionSyntax WithArgumentList(ArgumentListSyntax? argumentList)
        {
            return Update(NewKeyword, Type, argumentList, Initializer);
        }

        internal override BaseObjectCreationExpressionSyntax WithInitializerCore(InitializerExpressionSyntax? initializer)
        {
            return WithInitializer(initializer);
        }

        public new ObjectCreationExpressionSyntax WithInitializer(InitializerExpressionSyntax? initializer)
        {
            return Update(NewKeyword, Type, ArgumentList, initializer);
        }

        internal override BaseObjectCreationExpressionSyntax AddArgumentListArgumentsCore(params ArgumentSyntax[] items)
        {
            return AddArgumentListArguments(items);
        }

        public new ObjectCreationExpressionSyntax AddArgumentListArguments(params ArgumentSyntax[] items)
        {
            ArgumentListSyntax argumentListSyntax = ArgumentList ?? SyntaxFactory.ArgumentList();
            return WithArgumentList(argumentListSyntax.WithArguments(argumentListSyntax.Arguments.AddRange(items)));
        }
    }
}
