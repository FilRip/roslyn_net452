using System;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public interface IDiscardSymbol : ISymbol, IEquatable<ISymbol?>
    {
        ITypeSymbol Type { get; }

        NullableAnnotation NullableAnnotation { get; }
    }
}
