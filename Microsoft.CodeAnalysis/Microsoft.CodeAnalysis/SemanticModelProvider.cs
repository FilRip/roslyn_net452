namespace Microsoft.CodeAnalysis
{
    public abstract class SemanticModelProvider
    {
        public abstract SemanticModel GetSemanticModel(SyntaxTree tree, Compilation compilation, bool ignoreAccessibility = false);
    }
}
