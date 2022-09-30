using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.PooledObjects;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal abstract class SynthesizedParameterSymbolBase : ParameterSymbol
    {
        private readonly MethodSymbol? _container;

        private readonly TypeWithAnnotations _type;

        private readonly int _ordinal;

        private readonly string _name;

        private readonly RefKind _refKind;

        public override TypeWithAnnotations TypeWithAnnotations => _type;

        public override RefKind RefKind => _refKind;

        public sealed override bool IsDiscard => false;

        internal override bool IsMetadataIn => RefKind == RefKind.In;

        internal override bool IsMetadataOut => RefKind == RefKind.Out;

        public override string Name => _name;

        public abstract override ImmutableArray<CustomModifier> RefCustomModifiers { get; }

        public override int Ordinal => _ordinal;

        public override bool IsParams => false;

        internal override bool IsMetadataOptional => false;

        public override bool IsImplicitlyDeclared => true;

        internal override ConstantValue? ExplicitDefaultConstantValue => null;

        internal override bool IsIDispatchConstant => false;

        internal override bool IsIUnknownConstant => false;

        internal override bool IsCallerLineNumber => false;

        internal override bool IsCallerFilePath => false;

        internal override bool IsCallerMemberName => false;

        internal override FlowAnalysisAnnotations FlowAnalysisAnnotations => FlowAnalysisAnnotations.None;

        internal override ImmutableHashSet<string> NotNullIfParameterNotNull => ImmutableHashSet<string>.Empty;

        public override Symbol? ContainingSymbol => _container;

        public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        public SynthesizedParameterSymbolBase(MethodSymbol? container, TypeWithAnnotations type, int ordinal, RefKind refKind, string name = "")
        {
            _container = container;
            _type = type;
            _ordinal = ordinal;
            _refKind = refKind;
            _name = name;
        }

        internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            TypeWithAnnotations typeWithAnnotations = TypeWithAnnotations;
            if (typeWithAnnotations.Type.ContainsDynamic() && declaringCompilation.HasDynamicEmitAttributes(BindingDiagnosticBag.Discarded, Location.None) && declaringCompilation.CanEmitBoolean())
            {
                Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.SynthesizeDynamicAttribute(typeWithAnnotations.Type, typeWithAnnotations.CustomModifiers.Length + RefCustomModifiers.Length, RefKind));
            }
            if (typeWithAnnotations.Type.ContainsNativeInteger())
            {
                Symbol.AddSynthesizedAttribute(ref attributes, moduleBuilder.SynthesizeNativeIntegerAttribute(this, typeWithAnnotations.Type));
            }
            if (typeWithAnnotations.Type.ContainsTupleNames() && declaringCompilation.HasTupleNamesAttributes(BindingDiagnosticBag.Discarded, Location.None) && declaringCompilation.CanEmitSpecialType(SpecialType.System_String))
            {
                Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.SynthesizeTupleNamesAttribute(typeWithAnnotations.Type));
            }
            if (declaringCompilation.ShouldEmitNullableAttributes(this))
            {
                Symbol.AddSynthesizedAttribute(ref attributes, moduleBuilder.SynthesizeNullableAttributeIfNecessary(this, GetNullableContextValue(), typeWithAnnotations));
            }
            if (RefKind == RefKind.In)
            {
                Symbol.AddSynthesizedAttribute(ref attributes, moduleBuilder.SynthesizeIsReadOnlyAttribute(this));
            }
        }
    }
}
