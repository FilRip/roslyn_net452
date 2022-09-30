namespace Microsoft.CodeAnalysis
{
    public readonly struct GeneratorSyntaxContext
    {
        public SyntaxNode Node { get; }

        public SemanticModel SemanticModel { get; }

        internal GeneratorSyntaxContext(SyntaxNode node, SemanticModel semanticModel)
        {
            Node = node;
            SemanticModel = semanticModel;
        }
    }
}
