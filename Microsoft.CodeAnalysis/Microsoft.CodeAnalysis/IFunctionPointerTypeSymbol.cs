using System;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public interface IFunctionPointerTypeSymbol : ITypeSymbol, INamespaceOrTypeSymbol, ISymbol, IEquatable<ISymbol?>
    {
        IMethodSymbol Signature { get; }
    }
}
