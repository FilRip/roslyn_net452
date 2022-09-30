using System;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public interface IAliasSymbol : ISymbol, IEquatable<ISymbol?>
    {
        INamespaceOrTypeSymbol Target { get; }
    }
}
