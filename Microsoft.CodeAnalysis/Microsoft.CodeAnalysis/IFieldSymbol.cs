using System;
using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public interface IFieldSymbol : ISymbol, IEquatable<ISymbol?>
    {
        ISymbol? AssociatedSymbol { get; }

        bool IsConst { get; }

        bool IsReadOnly { get; }

        bool IsVolatile { get; }

        bool IsFixedSizeBuffer { get; }

        ITypeSymbol Type { get; }

        NullableAnnotation NullableAnnotation { get; }

        [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, "ConstantValue")]
        bool HasConstantValue
        {
            [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, "ConstantValue")]
            get;
        }

        object? ConstantValue { get; }

        ImmutableArray<CustomModifier> CustomModifiers { get; }

        new IFieldSymbol OriginalDefinition { get; }

        IFieldSymbol? CorrespondingTupleField { get; }

        bool IsExplicitlyNamedTupleElement { get; }
    }
}
