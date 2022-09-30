#nullable enable

namespace Microsoft.CodeAnalysis.Syntax.InternalSyntax
{
    public static class GreenNodeExtensions
    {
        public static SyntaxList<T> ToGreenList<T>(this SyntaxNode? node) where T : GreenNode
        {
            return node?.Green.ToGreenList<T>() ?? default(SyntaxList<T>);
        }

        public static SeparatedSyntaxList<T> ToGreenSeparatedList<T>(this SyntaxNode? node) where T : GreenNode
        {
            if (node == null)
            {
                return default(SeparatedSyntaxList<T>);
            }
            return new SeparatedSyntaxList<T>(node!.Green.ToGreenList<T>());
        }

        public static SyntaxList<T> ToGreenList<T>(this GreenNode? node) where T : GreenNode
        {
            return new SyntaxList<T>(node);
        }
    }
}
