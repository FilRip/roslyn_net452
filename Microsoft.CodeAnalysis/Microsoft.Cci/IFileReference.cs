using System.Collections.Immutable;
using System.Reflection;

#nullable enable

namespace Microsoft.Cci
{
    public interface IFileReference
    {
        bool HasMetadata { get; }

        string? FileName { get; }

        ImmutableArray<byte> GetHashValue(AssemblyHashAlgorithm algorithmId);
    }
}
