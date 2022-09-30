#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public abstract class MemberDeclarationSyntax : CSharpSyntaxNode
    {
        public abstract SyntaxList<AttributeListSyntax> AttributeLists { get; }

        public abstract SyntaxTokenList Modifiers { get; }

        internal MemberDeclarationSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public MemberDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeListsCore(attributeLists);
        }

        internal abstract MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists);

        public MemberDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return AddAttributeListsCore(items);
        }

        internal abstract MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items);

        public MemberDeclarationSyntax WithModifiers(SyntaxTokenList modifiers)
        {
            return WithModifiersCore(modifiers);
        }

        internal abstract MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers);

        public MemberDeclarationSyntax AddModifiers(params SyntaxToken[] items)
        {
            return AddModifiersCore(items);
        }

        internal abstract MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items);
    }
}
