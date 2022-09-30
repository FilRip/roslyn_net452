#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public abstract class BasePropertyDeclarationSyntax : MemberDeclarationSyntax
    {
        public abstract override SyntaxList<AttributeListSyntax> AttributeLists { get; }

        public abstract override SyntaxTokenList Modifiers { get; }

        public abstract TypeSyntax Type { get; }

        public abstract ExplicitInterfaceSpecifierSyntax? ExplicitInterfaceSpecifier { get; }

        public abstract AccessorListSyntax? AccessorList { get; }

        internal BasePropertyDeclarationSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public BasePropertyDeclarationSyntax WithType(TypeSyntax type)
        {
            return WithTypeCore(type);
        }

        internal abstract BasePropertyDeclarationSyntax WithTypeCore(TypeSyntax type);

        public BasePropertyDeclarationSyntax WithExplicitInterfaceSpecifier(ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier)
        {
            return WithExplicitInterfaceSpecifierCore(explicitInterfaceSpecifier);
        }

        internal abstract BasePropertyDeclarationSyntax WithExplicitInterfaceSpecifierCore(ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier);

        public BasePropertyDeclarationSyntax WithAccessorList(AccessorListSyntax? accessorList)
        {
            return WithAccessorListCore(accessorList);
        }

        internal abstract BasePropertyDeclarationSyntax WithAccessorListCore(AccessorListSyntax? accessorList);

        public BasePropertyDeclarationSyntax AddAccessorListAccessors(params AccessorDeclarationSyntax[] items)
        {
            return AddAccessorListAccessorsCore(items);
        }

        internal abstract BasePropertyDeclarationSyntax AddAccessorListAccessorsCore(params AccessorDeclarationSyntax[] items);

        public new BasePropertyDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return (BasePropertyDeclarationSyntax)WithAttributeListsCore(attributeLists);
        }

        public new BasePropertyDeclarationSyntax WithModifiers(SyntaxTokenList modifiers)
        {
            return (BasePropertyDeclarationSyntax)WithModifiersCore(modifiers);
        }

        public new BasePropertyDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return (BasePropertyDeclarationSyntax)AddAttributeListsCore(items);
        }

        public new BasePropertyDeclarationSyntax AddModifiers(params SyntaxToken[] items)
        {
            return (BasePropertyDeclarationSyntax)AddModifiersCore(items);
        }
    }
}
