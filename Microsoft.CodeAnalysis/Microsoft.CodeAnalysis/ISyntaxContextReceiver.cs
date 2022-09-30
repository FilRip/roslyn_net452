namespace Microsoft.CodeAnalysis
{
    public interface ISyntaxContextReceiver
    {
        void OnVisitSyntaxNode(GeneratorSyntaxContext context);
    }
}
