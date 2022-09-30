using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public abstract class SynthesizedFieldSymbolBase : FieldSymbol
    {
        private readonly NamedTypeSymbol _containingType;

        private readonly string _name;

        private readonly DeclarationModifiers _modifiers;

        internal abstract bool SuppressDynamicAttribute { get; }

        public override FlowAnalysisAnnotations FlowAnalysisAnnotations => FlowAnalysisAnnotations.None;

        public override string Name => _name;

        public override Symbol AssociatedSymbol => null;

        public override bool IsReadOnly => (_modifiers & DeclarationModifiers.ReadOnly) != 0;

        public override bool IsVolatile => false;

        public override bool IsConst => false;

        internal override bool IsNotSerialized => false;

        internal override MarshalPseudoCustomAttributeData MarshallingInformation => null;

        internal override int? TypeLayoutOffset => null;

        internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

        public override Symbol ContainingSymbol => _containingType;

        public override NamedTypeSymbol ContainingType => _containingType;

        public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        public override Accessibility DeclaredAccessibility => ModifierUtils.EffectiveAccessibility(_modifiers);

        public override bool IsStatic => (_modifiers & DeclarationModifiers.Static) != 0;

        internal override bool HasSpecialName => HasRuntimeSpecialName;

        internal override bool HasRuntimeSpecialName => Name == "value__";

        public override bool IsImplicitlyDeclared => true;

        public SynthesizedFieldSymbolBase(NamedTypeSymbol containingType, string name, bool isPublic, bool isReadOnly, bool isStatic)
        {
            _containingType = containingType;
            _name = name;
            _modifiers = (isPublic ? DeclarationModifiers.Public : DeclarationModifiers.Private) | (isReadOnly ? DeclarationModifiers.ReadOnly : DeclarationModifiers.None) | (isStatic ? DeclarationModifiers.Static : DeclarationModifiers.None);
        }

        internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
            base.AddSynthesizedAttributes(moduleBuilder, ref attributes);
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            TypeWithAnnotations typeWithAnnotations = base.TypeWithAnnotations;
            TypeSymbol type = typeWithAnnotations.Type;
            if (!_containingType.IsImplicitlyDeclared)
            {
                Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor));
            }
            if (!SuppressDynamicAttribute && type.ContainsDynamic() && declaringCompilation.HasDynamicEmitAttributes(BindingDiagnosticBag.Discarded, Location.None) && declaringCompilation.CanEmitBoolean())
            {
                Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.SynthesizeDynamicAttribute(type, typeWithAnnotations.CustomModifiers.Length));
            }
            if (type.ContainsNativeInteger())
            {
                Symbol.AddSynthesizedAttribute(ref attributes, moduleBuilder.SynthesizeNativeIntegerAttribute(this, type));
            }
            if (type.ContainsTupleNames() && declaringCompilation.HasTupleNamesAttributes(BindingDiagnosticBag.Discarded, Location.None) && declaringCompilation.CanEmitSpecialType(SpecialType.System_String))
            {
                Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.SynthesizeTupleNamesAttribute(base.Type));
            }
            if (declaringCompilation.ShouldEmitNullableAttributes(this))
            {
                Symbol.AddSynthesizedAttribute(ref attributes, moduleBuilder.SynthesizeNullableAttributeIfNecessary(this, ContainingType.GetNullableContextValue(), typeWithAnnotations));
            }
        }

        internal abstract override TypeWithAnnotations GetFieldType(ConsList<FieldSymbol> fieldsBeingBound);

        internal override ConstantValue GetConstantValue(ConstantFieldsInProgress inProgress, bool earlyDecodingWellKnownAttributes)
        {
            return null;
        }
    }
}
