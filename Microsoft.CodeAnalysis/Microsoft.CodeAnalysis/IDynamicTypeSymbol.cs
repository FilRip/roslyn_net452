using System;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public interface IDynamicTypeSymbol : ITypeSymbol, INamespaceOrTypeSymbol, ISymbol, IEquatable<ISymbol?>
    {
    }
}
