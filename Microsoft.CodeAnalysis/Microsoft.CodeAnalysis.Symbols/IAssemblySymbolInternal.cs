using System;

#nullable enable

namespace Microsoft.CodeAnalysis.Symbols
{
    public interface IAssemblySymbolInternal : ISymbolInternal
    {
        Version? AssemblyVersionPattern { get; }

        AssemblyIdentity Identity { get; }

        IAssemblySymbolInternal CorLibrary { get; }
    }
}
