namespace Microsoft.CodeAnalysis
{
    public interface ISyntaxReceiver
    {
        void OnVisitSyntaxNode(SyntaxNode syntaxNode);
    }
}
