using System.Collections.Immutable;

using Microsoft.CodeAnalysis.PooledObjects;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedParameterSymbol : SynthesizedParameterSymbolBase
    {
        public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

        internal override MarshalPseudoCustomAttributeData? MarshallingInformation => null;

        private SynthesizedParameterSymbol(MethodSymbol? container, TypeWithAnnotations type, int ordinal, RefKind refKind, string name)
            : base(container, type, ordinal, refKind, name)
        {
        }

        public static ParameterSymbol Create(MethodSymbol? container, TypeWithAnnotations type, int ordinal, RefKind refKind, string name = "", ImmutableArray<CustomModifier> refCustomModifiers = default(ImmutableArray<CustomModifier>), SourceComplexParameterSymbol? baseParameterForAttributes = null)
        {
            if (refCustomModifiers.IsDefaultOrEmpty && (object)baseParameterForAttributes == null)
            {
                return new SynthesizedParameterSymbol(container, type, ordinal, refKind, name);
            }
            return new SynthesizedComplexParameterSymbol(container, type, ordinal, refKind, name, refCustomModifiers.NullToEmpty(), baseParameterForAttributes);
        }

        internal static ImmutableArray<ParameterSymbol> DeriveParameters(MethodSymbol sourceMethod, MethodSymbol destinationMethod)
        {
            ArrayBuilder<ParameterSymbol> instance = ArrayBuilder<ParameterSymbol>.GetInstance();
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = sourceMethod.Parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                instance.Add(Create(destinationMethod, current.TypeWithAnnotations, current.Ordinal, current.RefKind, current.Name, current.RefCustomModifiers));
            }
            return instance.ToImmutableAndFree();
        }
    }
}
