#nullable enable

namespace Microsoft.CodeAnalysis.Syntax
{
    public static class SyntaxListBuilderExtensions
    {
        public static SyntaxTokenList ToTokenList(this SyntaxListBuilder? builder)
        {
            if (builder == null || builder!.Count == 0)
            {
                return default(SyntaxTokenList);
            }
            return new SyntaxTokenList(null, builder!.ToListNode(), 0, 0);
        }

        public static SyntaxList<SyntaxNode> ToList(this SyntaxListBuilder? builder)
        {
            GreenNode greenNode = builder?.ToListNode();
            if (greenNode == null)
            {
                return default(SyntaxList<SyntaxNode>);
            }
            return new SyntaxList<SyntaxNode>(greenNode.CreateRed());
        }

        public static SyntaxList<TNode> ToList<TNode>(this SyntaxListBuilder? builder) where TNode : SyntaxNode
        {
            GreenNode greenNode = builder?.ToListNode();
            if (greenNode == null)
            {
                return default(SyntaxList<TNode>);
            }
            return new SyntaxList<TNode>(greenNode.CreateRed());
        }

        public static SeparatedSyntaxList<TNode> ToSeparatedList<TNode>(this SyntaxListBuilder? builder) where TNode : SyntaxNode
        {
            GreenNode greenNode = builder?.ToListNode();
            if (greenNode == null)
            {
                return default(SeparatedSyntaxList<TNode>);
            }
            return new SeparatedSyntaxList<TNode>(new SyntaxNodeOrTokenList(greenNode.CreateRed(), 0));
        }
    }
}
