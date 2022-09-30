namespace Microsoft.CodeAnalysis.CSharp
{
    public abstract class MergedNamespaceOrTypeDeclaration : Declaration
    {
        protected MergedNamespaceOrTypeDeclaration(string name)
            : base(name)
        {
        }
    }
}
