#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public abstract class BaseTypeDeclarationSyntax : MemberDeclarationSyntax
    {
        public abstract SyntaxToken Identifier { get; }

        public abstract BaseListSyntax? BaseList { get; }

        public abstract SyntaxToken OpenBraceToken { get; }

        public abstract SyntaxToken CloseBraceToken { get; }

        public abstract SyntaxToken SemicolonToken { get; }

        internal BaseTypeDeclarationSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public BaseTypeDeclarationSyntax WithIdentifier(SyntaxToken identifier)
        {
            return WithIdentifierCore(identifier);
        }

        internal abstract BaseTypeDeclarationSyntax WithIdentifierCore(SyntaxToken identifier);

        public BaseTypeDeclarationSyntax WithBaseList(BaseListSyntax? baseList)
        {
            return WithBaseListCore(baseList);
        }

        internal abstract BaseTypeDeclarationSyntax WithBaseListCore(BaseListSyntax? baseList);

        public BaseTypeDeclarationSyntax AddBaseListTypes(params BaseTypeSyntax[] items)
        {
            return AddBaseListTypesCore(items);
        }

        internal abstract BaseTypeDeclarationSyntax AddBaseListTypesCore(params BaseTypeSyntax[] items);

        public BaseTypeDeclarationSyntax WithOpenBraceToken(SyntaxToken openBraceToken)
        {
            return WithOpenBraceTokenCore(openBraceToken);
        }

        internal abstract BaseTypeDeclarationSyntax WithOpenBraceTokenCore(SyntaxToken openBraceToken);

        public BaseTypeDeclarationSyntax WithCloseBraceToken(SyntaxToken closeBraceToken)
        {
            return WithCloseBraceTokenCore(closeBraceToken);
        }

        internal abstract BaseTypeDeclarationSyntax WithCloseBraceTokenCore(SyntaxToken closeBraceToken);

        public BaseTypeDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken)
        {
            return WithSemicolonTokenCore(semicolonToken);
        }

        internal abstract BaseTypeDeclarationSyntax WithSemicolonTokenCore(SyntaxToken semicolonToken);

        public new BaseTypeDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return (BaseTypeDeclarationSyntax)WithAttributeListsCore(attributeLists);
        }

        public new BaseTypeDeclarationSyntax WithModifiers(SyntaxTokenList modifiers)
        {
            return (BaseTypeDeclarationSyntax)WithModifiersCore(modifiers);
        }

        public new BaseTypeDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return (BaseTypeDeclarationSyntax)AddAttributeListsCore(items);
        }

        public new BaseTypeDeclarationSyntax AddModifiers(params SyntaxToken[] items)
        {
            return (BaseTypeDeclarationSyntax)AddModifiersCore(items);
        }
    }
}
