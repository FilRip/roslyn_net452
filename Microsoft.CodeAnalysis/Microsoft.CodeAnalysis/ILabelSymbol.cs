using System;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public interface ILabelSymbol : ISymbol, IEquatable<ISymbol?>
    {
        IMethodSymbol ContainingMethod { get; }
    }
}
