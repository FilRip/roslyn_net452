namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public readonly struct BlendedNode
    {
        internal readonly Microsoft.CodeAnalysis.CSharp.CSharpSyntaxNode Node;

        internal readonly SyntaxToken Token;

        internal readonly Blender Blender;

        public BlendedNode(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxNode node, SyntaxToken token, Blender blender)
        {
            Node = node;
            Token = token;
            Blender = blender;
        }
    }
}
