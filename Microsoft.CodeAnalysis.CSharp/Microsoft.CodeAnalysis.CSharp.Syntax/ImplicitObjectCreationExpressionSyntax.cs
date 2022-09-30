using System;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ImplicitObjectCreationExpressionSyntax : BaseObjectCreationExpressionSyntax
    {
        private ArgumentListSyntax? argumentList;

        private InitializerExpressionSyntax? initializer;

        public override SyntaxToken NewKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ImplicitObjectCreationExpressionSyntax)base.Green).newKeyword, base.Position, 0);

        public override ArgumentListSyntax ArgumentList => GetRed(ref argumentList, 1);

        public override InitializerExpressionSyntax? Initializer => GetRed(ref initializer, 2);

        internal ImplicitObjectCreationExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                1 => GetRed(ref argumentList, 1),
                2 => GetRed(ref initializer, 2),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                1 => argumentList,
                2 => initializer,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitImplicitObjectCreationExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitImplicitObjectCreationExpression(this);

        public ImplicitObjectCreationExpressionSyntax Update(SyntaxToken newKeyword, ArgumentListSyntax argumentList, InitializerExpressionSyntax? initializer)
        {
            if (newKeyword != NewKeyword || argumentList != ArgumentList || initializer != Initializer)
            {
                ImplicitObjectCreationExpressionSyntax implicitObjectCreationExpressionSyntax = SyntaxFactory.ImplicitObjectCreationExpression(newKeyword, argumentList, initializer);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return implicitObjectCreationExpressionSyntax;
                }
                return implicitObjectCreationExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override BaseObjectCreationExpressionSyntax WithNewKeywordCore(SyntaxToken newKeyword)
        {
            return WithNewKeyword(newKeyword);
        }

        public new ImplicitObjectCreationExpressionSyntax WithNewKeyword(SyntaxToken newKeyword)
        {
            return Update(newKeyword, ArgumentList, Initializer);
        }

        internal override BaseObjectCreationExpressionSyntax WithArgumentListCore(ArgumentListSyntax? argumentList)
        {
            return WithArgumentList(argumentList ?? throw new ArgumentNullException("argumentList"));
        }

        public new ImplicitObjectCreationExpressionSyntax WithArgumentList(ArgumentListSyntax argumentList)
        {
            return Update(NewKeyword, argumentList, Initializer);
        }

        internal override BaseObjectCreationExpressionSyntax WithInitializerCore(InitializerExpressionSyntax? initializer)
        {
            return WithInitializer(initializer);
        }

        public new ImplicitObjectCreationExpressionSyntax WithInitializer(InitializerExpressionSyntax? initializer)
        {
            return Update(NewKeyword, ArgumentList, initializer);
        }

        internal override BaseObjectCreationExpressionSyntax AddArgumentListArgumentsCore(params ArgumentSyntax[] items)
        {
            return AddArgumentListArguments(items);
        }

        public new ImplicitObjectCreationExpressionSyntax AddArgumentListArguments(params ArgumentSyntax[] items)
        {
            return WithArgumentList(ArgumentList.WithArguments(ArgumentList.Arguments.AddRange(items)));
        }
    }
}
