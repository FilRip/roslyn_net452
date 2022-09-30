using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public readonly struct DeclarationInfo
    {
        public SyntaxNode DeclaredNode { get; }

        public ImmutableArray<SyntaxNode> ExecutableCodeBlocks { get; }

        public ISymbol? DeclaredSymbol { get; }

        public DeclarationInfo(SyntaxNode declaredNode, ImmutableArray<SyntaxNode> executableCodeBlocks, ISymbol? declaredSymbol)
        {
            DeclaredNode = declaredNode;
            ExecutableCodeBlocks = executableCodeBlocks;
            DeclaredSymbol = declaredSymbol;
        }
    }
}
