using System.Reflection;

#nullable enable

namespace Microsoft.CodeAnalysis.Symbols
{
    public interface ISourceAssemblySymbolInternal : IAssemblySymbolInternal, ISymbolInternal
    {
        AssemblyFlags AssemblyFlags { get; }

        string? SignatureKey { get; }

        AssemblyHashAlgorithm HashAlgorithm { get; }

        bool InternalsAreVisible { get; }
    }
}
