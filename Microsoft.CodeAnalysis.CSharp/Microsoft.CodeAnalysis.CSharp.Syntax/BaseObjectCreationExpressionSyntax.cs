#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public abstract class BaseObjectCreationExpressionSyntax : ExpressionSyntax
    {
        public abstract SyntaxToken NewKeyword { get; }

        public abstract ArgumentListSyntax? ArgumentList { get; }

        public abstract InitializerExpressionSyntax? Initializer { get; }

        internal BaseObjectCreationExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public BaseObjectCreationExpressionSyntax WithNewKeyword(SyntaxToken newKeyword)
        {
            return WithNewKeywordCore(newKeyword);
        }

        internal abstract BaseObjectCreationExpressionSyntax WithNewKeywordCore(SyntaxToken newKeyword);

        public BaseObjectCreationExpressionSyntax WithArgumentList(ArgumentListSyntax? argumentList)
        {
            return WithArgumentListCore(argumentList);
        }

        internal abstract BaseObjectCreationExpressionSyntax WithArgumentListCore(ArgumentListSyntax? argumentList);

        public BaseObjectCreationExpressionSyntax AddArgumentListArguments(params ArgumentSyntax[] items)
        {
            return AddArgumentListArgumentsCore(items);
        }

        internal abstract BaseObjectCreationExpressionSyntax AddArgumentListArgumentsCore(params ArgumentSyntax[] items);

        public BaseObjectCreationExpressionSyntax WithInitializer(InitializerExpressionSyntax? initializer)
        {
            return WithInitializerCore(initializer);
        }

        internal abstract BaseObjectCreationExpressionSyntax WithInitializerCore(InitializerExpressionSyntax? initializer);
    }
}
