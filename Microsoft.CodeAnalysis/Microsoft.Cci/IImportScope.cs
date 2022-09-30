using System.Collections.Immutable;

namespace Microsoft.Cci
{
    public interface IImportScope
    {
        IImportScope Parent { get; }

        ImmutableArray<UsedNamespaceOrType> GetUsedNamespaces();
    }
}
