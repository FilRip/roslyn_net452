#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public abstract class BaseMethodDeclarationSyntax : MemberDeclarationSyntax
    {
        public abstract override SyntaxList<AttributeListSyntax> AttributeLists { get; }

        public abstract override SyntaxTokenList Modifiers { get; }

        public abstract ParameterListSyntax ParameterList { get; }

        public abstract BlockSyntax? Body { get; }

        public abstract ArrowExpressionClauseSyntax? ExpressionBody { get; }

        public abstract SyntaxToken SemicolonToken { get; }

        internal BaseMethodDeclarationSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public BaseMethodDeclarationSyntax WithParameterList(ParameterListSyntax parameterList)
        {
            return WithParameterListCore(parameterList);
        }

        internal abstract BaseMethodDeclarationSyntax WithParameterListCore(ParameterListSyntax parameterList);

        public BaseMethodDeclarationSyntax AddParameterListParameters(params ParameterSyntax[] items)
        {
            return AddParameterListParametersCore(items);
        }

        internal abstract BaseMethodDeclarationSyntax AddParameterListParametersCore(params ParameterSyntax[] items);

        public BaseMethodDeclarationSyntax WithBody(BlockSyntax? body)
        {
            return WithBodyCore(body);
        }

        internal abstract BaseMethodDeclarationSyntax WithBodyCore(BlockSyntax? body);

        public BaseMethodDeclarationSyntax AddBodyAttributeLists(params AttributeListSyntax[] items)
        {
            return AddBodyAttributeListsCore(items);
        }

        internal abstract BaseMethodDeclarationSyntax AddBodyAttributeListsCore(params AttributeListSyntax[] items);

        public BaseMethodDeclarationSyntax AddBodyStatements(params StatementSyntax[] items)
        {
            return AddBodyStatementsCore(items);
        }

        internal abstract BaseMethodDeclarationSyntax AddBodyStatementsCore(params StatementSyntax[] items);

        public BaseMethodDeclarationSyntax WithExpressionBody(ArrowExpressionClauseSyntax? expressionBody)
        {
            return WithExpressionBodyCore(expressionBody);
        }

        internal abstract BaseMethodDeclarationSyntax WithExpressionBodyCore(ArrowExpressionClauseSyntax? expressionBody);

        public BaseMethodDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken)
        {
            return WithSemicolonTokenCore(semicolonToken);
        }

        internal abstract BaseMethodDeclarationSyntax WithSemicolonTokenCore(SyntaxToken semicolonToken);

        public new BaseMethodDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return (BaseMethodDeclarationSyntax)WithAttributeListsCore(attributeLists);
        }

        public new BaseMethodDeclarationSyntax WithModifiers(SyntaxTokenList modifiers)
        {
            return (BaseMethodDeclarationSyntax)WithModifiersCore(modifiers);
        }

        public new BaseMethodDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return (BaseMethodDeclarationSyntax)AddAttributeListsCore(items);
        }

        public new BaseMethodDeclarationSyntax AddModifiers(params SyntaxToken[] items)
        {
            return (BaseMethodDeclarationSyntax)AddModifiersCore(items);
        }
    }
}
