#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public abstract class BaseFieldDeclarationSyntax : MemberDeclarationSyntax
    {
        public abstract override SyntaxList<AttributeListSyntax> AttributeLists { get; }

        public abstract override SyntaxTokenList Modifiers { get; }

        public abstract VariableDeclarationSyntax Declaration { get; }

        public abstract SyntaxToken SemicolonToken { get; }

        internal BaseFieldDeclarationSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public BaseFieldDeclarationSyntax WithDeclaration(VariableDeclarationSyntax declaration)
        {
            return WithDeclarationCore(declaration);
        }

        internal abstract BaseFieldDeclarationSyntax WithDeclarationCore(VariableDeclarationSyntax declaration);

        public BaseFieldDeclarationSyntax AddDeclarationVariables(params VariableDeclaratorSyntax[] items)
        {
            return AddDeclarationVariablesCore(items);
        }

        internal abstract BaseFieldDeclarationSyntax AddDeclarationVariablesCore(params VariableDeclaratorSyntax[] items);

        public BaseFieldDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken)
        {
            return WithSemicolonTokenCore(semicolonToken);
        }

        internal abstract BaseFieldDeclarationSyntax WithSemicolonTokenCore(SyntaxToken semicolonToken);

        public new BaseFieldDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return (BaseFieldDeclarationSyntax)WithAttributeListsCore(attributeLists);
        }

        public new BaseFieldDeclarationSyntax WithModifiers(SyntaxTokenList modifiers)
        {
            return (BaseFieldDeclarationSyntax)WithModifiersCore(modifiers);
        }

        public new BaseFieldDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return (BaseFieldDeclarationSyntax)AddAttributeListsCore(items);
        }

        public new BaseFieldDeclarationSyntax AddModifiers(params SyntaxToken[] items)
        {
            return (BaseFieldDeclarationSyntax)AddModifiersCore(items);
        }
    }
}
