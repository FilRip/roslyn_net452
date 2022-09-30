#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public abstract class BaseParameterSyntax : CSharpSyntaxNode
    {
        public abstract SyntaxList<AttributeListSyntax> AttributeLists { get; }

        public abstract SyntaxTokenList Modifiers { get; }

        public abstract TypeSyntax? Type { get; }

        internal BaseParameterSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public BaseParameterSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeListsCore(attributeLists);
        }

        internal abstract BaseParameterSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists);

        public BaseParameterSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return AddAttributeListsCore(items);
        }

        internal abstract BaseParameterSyntax AddAttributeListsCore(params AttributeListSyntax[] items);

        public BaseParameterSyntax WithModifiers(SyntaxTokenList modifiers)
        {
            return WithModifiersCore(modifiers);
        }

        internal abstract BaseParameterSyntax WithModifiersCore(SyntaxTokenList modifiers);

        public BaseParameterSyntax AddModifiers(params SyntaxToken[] items)
        {
            return AddModifiersCore(items);
        }

        internal abstract BaseParameterSyntax AddModifiersCore(params SyntaxToken[] items);

        public BaseParameterSyntax WithType(TypeSyntax? type)
        {
            return WithTypeCore(type);
        }

        internal abstract BaseParameterSyntax WithTypeCore(TypeSyntax? type);
    }
}
