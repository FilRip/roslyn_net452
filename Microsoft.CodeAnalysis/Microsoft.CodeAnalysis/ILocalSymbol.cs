using System;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public interface ILocalSymbol : ISymbol, IEquatable<ISymbol?>
    {
        ITypeSymbol Type { get; }

        NullableAnnotation NullableAnnotation { get; }

        bool IsConst { get; }

        bool IsRef { get; }

        RefKind RefKind { get; }

        bool HasConstantValue { get; }

        object? ConstantValue { get; }

        bool IsFunctionValue { get; }

        bool IsFixed { get; }
    }
}
