using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public abstract class SourceParameterSymbolBase : ParameterSymbol
    {
        private readonly Symbol _containingSymbol;

        private readonly ushort _ordinal;

        public sealed override int Ordinal => _ordinal;

        public sealed override Symbol ContainingSymbol => _containingSymbol;

        public sealed override AssemblySymbol ContainingAssembly => _containingSymbol.ContainingAssembly;

        internal abstract ConstantValue DefaultValueFromAttributes { get; }

        public SourceParameterSymbolBase(Symbol containingSymbol, int ordinal)
        {
            _ordinal = (ushort)ordinal;
            _containingSymbol = containingSymbol;
        }

        public sealed override bool Equals(Symbol obj, TypeCompareKind compareKind)
        {
            if ((object)obj == this)
            {
                return true;
            }
            if (obj is NativeIntegerParameterSymbol nativeIntegerParameterSymbol)
            {
                return nativeIntegerParameterSymbol.Equals(this, compareKind);
            }
            if (obj is SourceParameterSymbolBase sourceParameterSymbolBase && sourceParameterSymbolBase.Ordinal == Ordinal)
            {
                return sourceParameterSymbolBase._containingSymbol.Equals(_containingSymbol, compareKind);
            }
            return false;
        }

        public sealed override int GetHashCode()
        {
            return Hash.Combine(_containingSymbol.GetHashCode(), Ordinal);
        }

        internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
            base.AddSynthesizedAttributes(moduleBuilder, ref attributes);
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            if (IsParams)
            {
                Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_ParamArrayAttribute__ctor));
            }
            ConstantValue explicitDefaultConstantValue = ExplicitDefaultConstantValue;
            if (explicitDefaultConstantValue != null && explicitDefaultConstantValue.SpecialType == SpecialType.System_Decimal && DefaultValueFromAttributes == null)
            {
                Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.SynthesizeDecimalConstantAttribute(explicitDefaultConstantValue.DecimalValue));
            }
            TypeWithAnnotations typeWithAnnotations = TypeWithAnnotations;
            if (typeWithAnnotations.Type.ContainsDynamic())
            {
                Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.SynthesizeDynamicAttribute(typeWithAnnotations.Type, typeWithAnnotations.CustomModifiers.Length + RefCustomModifiers.Length, RefKind));
            }
            if (typeWithAnnotations.Type.ContainsNativeInteger())
            {
                Symbol.AddSynthesizedAttribute(ref attributes, moduleBuilder.SynthesizeNativeIntegerAttribute(this, typeWithAnnotations.Type));
            }
            if (typeWithAnnotations.Type.ContainsTupleNames())
            {
                Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.SynthesizeTupleNamesAttribute(typeWithAnnotations.Type));
            }
            if (RefKind == RefKind.In)
            {
                Symbol.AddSynthesizedAttribute(ref attributes, moduleBuilder.SynthesizeIsReadOnlyAttribute(this));
            }
            if (declaringCompilation.ShouldEmitNullableAttributes(this))
            {
                Symbol.AddSynthesizedAttribute(ref attributes, moduleBuilder.SynthesizeNullableAttributeIfNecessary(this, GetNullableContextValue(), typeWithAnnotations));
            }
        }

        internal abstract ParameterSymbol WithCustomModifiersAndParams(TypeSymbol newType, ImmutableArray<CustomModifier> newCustomModifiers, ImmutableArray<CustomModifier> newRefCustomModifiers, bool newIsParams);
    }
}
