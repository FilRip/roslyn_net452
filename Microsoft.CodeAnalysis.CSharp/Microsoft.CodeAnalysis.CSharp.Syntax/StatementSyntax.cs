#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public abstract class StatementSyntax : CSharpSyntaxNode
    {
        public abstract SyntaxList<AttributeListSyntax> AttributeLists { get; }

        internal StatementSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public StatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeListsCore(attributeLists);
        }

        internal abstract StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists);

        public StatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return AddAttributeListsCore(items);
        }

        internal abstract StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items);
    }
}
