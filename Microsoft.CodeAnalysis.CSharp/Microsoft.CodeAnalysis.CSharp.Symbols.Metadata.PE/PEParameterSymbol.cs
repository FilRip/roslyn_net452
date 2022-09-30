using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols.Metadata.PE
{
    internal class PEParameterSymbol : ParameterSymbol
    {
        [Flags()]
        private enum WellKnownAttributeFlags
        {
            HasIDispatchConstantAttribute = 1,
            HasIUnknownConstantAttribute = 2,
            HasCallerFilePathAttribute = 4,
            HasCallerLineNumberAttribute = 8,
            HasCallerMemberNameAttribute = 0x10,
            IsCallerFilePath = 0x20,
            IsCallerLineNumber = 0x40,
            IsCallerMemberName = 0x80
        }

        private struct PackedFlags
        {
            private const int WellKnownAttributeDataOffset = 0;

            private const int WellKnownAttributeCompletionFlagOffset = 8;

            private const int RefKindOffset = 16;

            private const int FlowAnalysisAnnotationsOffset = 20;

            private const int RefKindMask = 3;

            private const int WellKnownAttributeDataMask = 255;

            private const int WellKnownAttributeCompletionFlagMask = 255;

            private const int FlowAnalysisAnnotationsMask = 255;

            private const int HasNameInMetadataBit = 262144;

            private const int FlowAnalysisAnnotationsCompletionBit = 524288;

            private const int AllWellKnownAttributesCompleteNoData = 65280;

            private int _bits;

            public RefKind RefKind => (RefKind)((uint)(_bits >> 16) & 3u);

            public bool HasNameInMetadata => (_bits & 0x40000) != 0;

            public PackedFlags(RefKind refKind, bool attributesAreComplete, bool hasNameInMetadata)
            {
                int num = (int)((uint)(refKind & RefKind.In) << 16);
                int num2 = (attributesAreComplete ? 65280 : 0);
                int num3 = (hasNameInMetadata ? 262144 : 0);
                _bits = num | num2 | num3;
            }

            public bool SetWellKnownAttribute(WellKnownAttributeFlags flag, bool value)
            {
                int num = (int)flag << 8;
                if (value)
                {
                    num |= (int)flag;
                }
                ThreadSafeFlagOperations.Set(ref _bits, num);
                return value;
            }

            public bool TryGetWellKnownAttribute(WellKnownAttributeFlags flag, out bool value)
            {
                int bits = _bits;
                value = ((uint)bits & (uint)flag) != 0;
                return (bits & ((int)flag << 8)) != 0;
            }

            public bool SetFlowAnalysisAnnotations(FlowAnalysisAnnotations value)
            {
                int toSet = 0x80000 | ((int)(value & (FlowAnalysisAnnotations.MaybeNull | FlowAnalysisAnnotations.NotNull | FlowAnalysisAnnotations.DoesNotReturn | FlowAnalysisAnnotations.AllowNull | FlowAnalysisAnnotations.DisallowNull)) << 20);
                return ThreadSafeFlagOperations.Set(ref _bits, toSet);
            }

            public bool TryGetFlowAnalysisAnnotations(out FlowAnalysisAnnotations value)
            {
                int bits = _bits;
                value = (FlowAnalysisAnnotations)((bits >> 20) & 0xFF);
                return (bits & 0x80000) != 0;
            }
        }

        private sealed class PEParameterSymbolWithCustomModifiers : PEParameterSymbol
        {
            private readonly ImmutableArray<CustomModifier> _refCustomModifiers;

            public override ImmutableArray<CustomModifier> RefCustomModifiers => _refCustomModifiers;

            public PEParameterSymbolWithCustomModifiers(PEModuleSymbol moduleSymbol, Symbol containingSymbol, int ordinal, bool isByRef, ImmutableArray<ModifierInfo<TypeSymbol>> refCustomModifiers, TypeWithAnnotations type, ParameterHandle handle, Symbol nullableContext, out bool isBad)
                : base(moduleSymbol, containingSymbol, ordinal, isByRef, type, handle, nullableContext, refCustomModifiers.NullToEmpty().Length + type.CustomModifiers.Length, out isBad)
            {
                _refCustomModifiers = CSharpCustomModifier.Convert(refCustomModifiers);
            }
        }

        private readonly Symbol _containingSymbol;

        private readonly string _name;

        private readonly TypeWithAnnotations _typeWithAnnotations;

        private readonly ParameterHandle _handle;

        private readonly ParameterAttributes _flags;

        private readonly PEModuleSymbol _moduleSymbol;

        private ImmutableArray<CSharpAttributeData> _lazyCustomAttributes;

        private ConstantValue _lazyDefaultValue = ConstantValue.Unset;

        private ThreeState _lazyIsParams;

        private ImmutableArray<CSharpAttributeData> _lazyHiddenAttributes;

        private readonly ushort _ordinal;

        private PackedFlags _packedFlags;

        private bool HasNameInMetadata => _packedFlags.HasNameInMetadata;

        public override RefKind RefKind => _packedFlags.RefKind;

        public override string Name => _name;

        public override string MetadataName
        {
            get
            {
                if (!HasNameInMetadata)
                {
                    return string.Empty;
                }
                return _name;
            }
        }

        internal ParameterAttributes Flags => _flags;

        public override int Ordinal => _ordinal;

        public override bool IsDiscard => false;

        internal ParameterHandle Handle => _handle;

        public override Symbol ContainingSymbol => _containingSymbol;

        internal override bool HasMetadataConstantValue => (_flags & ParameterAttributes.HasDefault) != 0;

        internal override ConstantValue ExplicitDefaultConstantValue
        {
            get
            {
                if (_lazyDefaultValue == ConstantValue.Unset)
                {
                    ConstantValue value = ImportConstantValue(!IsMetadataOptional);
                    Interlocked.CompareExchange(ref _lazyDefaultValue, value, ConstantValue.Unset);
                }
                return _lazyDefaultValue;
            }
        }

        internal override bool IsMetadataOptional => (_flags & ParameterAttributes.Optional) != 0;

        internal override bool IsIDispatchConstant
        {
            get
            {
                if (!_packedFlags.TryGetWellKnownAttribute(WellKnownAttributeFlags.HasIDispatchConstantAttribute, out var value))
                {
                    return _packedFlags.SetWellKnownAttribute(WellKnownAttributeFlags.HasIDispatchConstantAttribute, _moduleSymbol.Module.HasAttribute(_handle, AttributeDescription.IDispatchConstantAttribute));
                }
                return value;
            }
        }

        internal override bool IsIUnknownConstant
        {
            get
            {
                if (!_packedFlags.TryGetWellKnownAttribute(WellKnownAttributeFlags.HasIUnknownConstantAttribute, out var value))
                {
                    return _packedFlags.SetWellKnownAttribute(WellKnownAttributeFlags.HasIUnknownConstantAttribute, _moduleSymbol.Module.HasAttribute(_handle, AttributeDescription.IUnknownConstantAttribute));
                }
                return value;
            }
        }

        private bool HasCallerLineNumberAttribute
        {
            get
            {
                if (!_packedFlags.TryGetWellKnownAttribute(WellKnownAttributeFlags.HasCallerLineNumberAttribute, out var value))
                {
                    return _packedFlags.SetWellKnownAttribute(WellKnownAttributeFlags.HasCallerLineNumberAttribute, _moduleSymbol.Module.HasAttribute(_handle, AttributeDescription.CallerLineNumberAttribute));
                }
                return value;
            }
        }

        private bool HasCallerFilePathAttribute
        {
            get
            {
                if (!_packedFlags.TryGetWellKnownAttribute(WellKnownAttributeFlags.HasCallerFilePathAttribute, out var value))
                {
                    return _packedFlags.SetWellKnownAttribute(WellKnownAttributeFlags.HasCallerFilePathAttribute, _moduleSymbol.Module.HasAttribute(_handle, AttributeDescription.CallerFilePathAttribute));
                }
                return value;
            }
        }

        private bool HasCallerMemberNameAttribute
        {
            get
            {
                if (!_packedFlags.TryGetWellKnownAttribute(WellKnownAttributeFlags.HasCallerMemberNameAttribute, out var value))
                {
                    return _packedFlags.SetWellKnownAttribute(WellKnownAttributeFlags.HasCallerMemberNameAttribute, _moduleSymbol.Module.HasAttribute(_handle, AttributeDescription.CallerMemberNameAttribute));
                }
                return value;
            }
        }

        internal override bool IsCallerLineNumber
        {
            get
            {
                if (!_packedFlags.TryGetWellKnownAttribute(WellKnownAttributeFlags.IsCallerLineNumber, out var value))
                {
                    CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                    bool value2 = HasCallerLineNumberAttribute && new TypeConversions(ContainingAssembly).HasCallerLineNumberConversion(base.Type, ref useSiteInfo);
                    return _packedFlags.SetWellKnownAttribute(WellKnownAttributeFlags.IsCallerLineNumber, value2);
                }
                return value;
            }
        }

        internal override bool IsCallerFilePath
        {
            get
            {
                if (!_packedFlags.TryGetWellKnownAttribute(WellKnownAttributeFlags.IsCallerFilePath, out var value))
                {
                    CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                    bool value2 = !HasCallerLineNumberAttribute && HasCallerFilePathAttribute && new TypeConversions(ContainingAssembly).HasCallerInfoStringConversion(base.Type, ref useSiteInfo);
                    return _packedFlags.SetWellKnownAttribute(WellKnownAttributeFlags.IsCallerFilePath, value2);
                }
                return value;
            }
        }

        internal override bool IsCallerMemberName
        {
            get
            {
                if (!_packedFlags.TryGetWellKnownAttribute(WellKnownAttributeFlags.IsCallerMemberName, out var value))
                {
                    CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                    bool value2 = !HasCallerLineNumberAttribute && !HasCallerFilePathAttribute && HasCallerMemberNameAttribute && new TypeConversions(ContainingAssembly).HasCallerInfoStringConversion(base.Type, ref useSiteInfo);
                    return _packedFlags.SetWellKnownAttribute(WellKnownAttributeFlags.IsCallerMemberName, value2);
                }
                return value;
            }
        }

        internal override FlowAnalysisAnnotations FlowAnalysisAnnotations
        {
            get
            {
                if (!_packedFlags.TryGetFlowAnalysisAnnotations(out var value))
                {
                    value = DecodeFlowAnalysisAttributes(_moduleSymbol.Module, _handle);
                    _packedFlags.SetFlowAnalysisAnnotations(value);
                }
                return value;
            }
        }

        internal override ImmutableHashSet<string> NotNullIfParameterNotNull => _moduleSymbol.Module.GetStringValuesOfNotNullIfNotNullAttribute(_handle);

        public override TypeWithAnnotations TypeWithAnnotations => _typeWithAnnotations;

        public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

        internal override bool IsMetadataIn => (_flags & ParameterAttributes.In) != 0;

        internal override bool IsMetadataOut => (_flags & ParameterAttributes.Out) != 0;

        internal override bool IsMarshalledExplicitly => (_flags & ParameterAttributes.HasFieldMarshal) != 0;

        internal override MarshalPseudoCustomAttributeData MarshallingInformation => null;

        internal override ImmutableArray<byte> MarshallingDescriptor
        {
            get
            {
                if ((_flags & ParameterAttributes.HasFieldMarshal) == 0)
                {
                    return default(ImmutableArray<byte>);
                }
                return _moduleSymbol.Module.GetMarshallingDescriptor(_handle);
            }
        }

        internal override UnmanagedType MarshallingType
        {
            get
            {
                if ((_flags & ParameterAttributes.HasFieldMarshal) == 0)
                {
                    return 0;
                }
                return _moduleSymbol.Module.GetMarshallingType(_handle);
            }
        }

        public override bool IsParams
        {
            get
            {
                if (!_lazyIsParams.HasValue())
                {
                    _lazyIsParams = _moduleSymbol.Module.HasParamsAttribute(_handle).ToThreeState();
                }
                return _lazyIsParams.Value();
            }
        }

        public override ImmutableArray<Location> Locations => _containingSymbol.Locations;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        internal sealed override CSharpCompilation DeclaringCompilation => null;

        internal static PEParameterSymbol Create(PEModuleSymbol moduleSymbol, PEMethodSymbol containingSymbol, bool isContainingSymbolVirtual, int ordinal, ParamInfo<TypeSymbol> parameterInfo, Symbol nullableContext, bool isReturn, out bool isBad)
        {
            return Create(moduleSymbol, containingSymbol, isContainingSymbolVirtual, ordinal, parameterInfo.IsByRef, parameterInfo.RefCustomModifiers, parameterInfo.Type, parameterInfo.Handle, nullableContext, parameterInfo.CustomModifiers, isReturn, out isBad);
        }

        internal static PEParameterSymbol Create(PEModuleSymbol moduleSymbol, PEPropertySymbol containingSymbol, bool isContainingSymbolVirtual, int ordinal, ParameterHandle handle, ParamInfo<TypeSymbol> parameterInfo, Symbol nullableContext, out bool isBad)
        {
            return Create(moduleSymbol, containingSymbol, isContainingSymbolVirtual, ordinal, parameterInfo.IsByRef, parameterInfo.RefCustomModifiers, parameterInfo.Type, handle, nullableContext, parameterInfo.CustomModifiers, isReturn: false, out isBad);
        }

        private PEParameterSymbol(PEModuleSymbol moduleSymbol, Symbol containingSymbol, int ordinal, bool isByRef, TypeWithAnnotations typeWithAnnotations, ParameterHandle handle, Symbol nullableContext, int countOfCustomModifiers, out bool isBad)
        {
            isBad = false;
            _moduleSymbol = moduleSymbol;
            _containingSymbol = containingSymbol;
            _ordinal = (ushort)ordinal;
            _handle = handle;
            RefKind refKind = RefKind.None;
            if (handle.IsNil)
            {
                refKind = (isByRef ? RefKind.Ref : RefKind.None);
                byte? nullableContextValue = nullableContext.GetNullableContextValue();
                if (nullableContextValue.HasValue)
                {
                    typeWithAnnotations = NullableTypeDecoder.TransformType(typeWithAnnotations, nullableContextValue.GetValueOrDefault(), default(ImmutableArray<byte>));
                }
                _lazyCustomAttributes = ImmutableArray<CSharpAttributeData>.Empty;
                _lazyHiddenAttributes = ImmutableArray<CSharpAttributeData>.Empty;
                _lazyDefaultValue = null;
                _lazyIsParams = ThreeState.False;
            }
            else
            {
                try
                {
                    moduleSymbol.Module.GetParamPropsOrThrow(handle, out _name, out _flags);
                }
                catch (BadImageFormatException)
                {
                    isBad = true;
                }
                if (isByRef)
                {
                    refKind = (((_flags & (ParameterAttributes.In | ParameterAttributes.Out)) == ParameterAttributes.Out) ? RefKind.Out : ((!moduleSymbol.Module.HasIsReadOnlyAttribute(handle)) ? RefKind.Ref : RefKind.In));
                }
                TypeSymbol type = DynamicTypeDecoder.TransformType(typeWithAnnotations.Type, countOfCustomModifiers, handle, moduleSymbol, refKind);
                type = NativeIntegerTypeDecoder.TransformType(type, handle, moduleSymbol);
                typeWithAnnotations = typeWithAnnotations.WithTypeAndModifiers(type, typeWithAnnotations.CustomModifiers);
                Symbol accessSymbol = ((containingSymbol.Kind == SymbolKind.Property) ? containingSymbol.ContainingSymbol : containingSymbol);
                typeWithAnnotations = NullableTypeDecoder.TransformType(typeWithAnnotations, handle, moduleSymbol, accessSymbol, nullableContext);
                typeWithAnnotations = TupleTypeDecoder.DecodeTupleTypesIfApplicable(typeWithAnnotations, handle, moduleSymbol);
            }
            _typeWithAnnotations = typeWithAnnotations;
            bool flag = !string.IsNullOrEmpty(_name);
            if (!flag)
            {
                _name = "value";
            }
            _packedFlags = new PackedFlags(refKind, handle.IsNil, flag);
        }

        private static PEParameterSymbol Create(PEModuleSymbol moduleSymbol, Symbol containingSymbol, bool isContainingSymbolVirtual, int ordinal, bool isByRef, ImmutableArray<ModifierInfo<TypeSymbol>> refCustomModifiers, TypeSymbol type, ParameterHandle handle, Symbol nullableContext, ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers, bool isReturn, out bool isBad)
        {
            TypeWithAnnotations typeWithAnnotations = TypeWithAnnotations.Create(type, NullableAnnotation.Oblivious, CSharpCustomModifier.Convert(customModifiers));
            PEParameterSymbol pEParameterSymbol = ((customModifiers.IsDefaultOrEmpty && refCustomModifiers.IsDefaultOrEmpty) ? new PEParameterSymbol(moduleSymbol, containingSymbol, ordinal, isByRef, typeWithAnnotations, handle, nullableContext, 0, out isBad) : new PEParameterSymbolWithCustomModifiers(moduleSymbol, containingSymbol, ordinal, isByRef, refCustomModifiers, typeWithAnnotations, handle, nullableContext, out isBad));
            bool flag = pEParameterSymbol.RefCustomModifiers.HasInAttributeModifier();
            if (isReturn)
            {
                isBad |= pEParameterSymbol.RefKind == RefKind.In != flag;
            }
            else if (pEParameterSymbol.RefKind == RefKind.In)
            {
                isBad |= isContainingSymbolVirtual != flag;
            }
            else if (flag)
            {
                isBad = true;
            }
            return pEParameterSymbol;
        }

        internal ConstantValue ImportConstantValue(bool ignoreAttributes = false)
        {
            ConstantValue constantValue = null;
            if ((_flags & ParameterAttributes.HasDefault) != 0)
            {
                constantValue = _moduleSymbol.Module.GetParamDefaultValue(_handle);
            }
            if (constantValue == null && !ignoreAttributes)
            {
                constantValue = GetDefaultDecimalOrDateTimeValue();
            }
            return constantValue;
        }

        private ConstantValue GetDefaultDecimalOrDateTimeValue()
        {
            if (_moduleSymbol.Module.HasDateTimeConstantAttribute(_handle, out ConstantValue defaultValue))
            {
                return defaultValue;
            }
            _moduleSymbol.Module.HasDecimalConstantAttribute(_handle, out defaultValue);
            return defaultValue;
        }

        private static FlowAnalysisAnnotations DecodeFlowAnalysisAttributes(PEModule module, ParameterHandle handle)
        {
            FlowAnalysisAnnotations flowAnalysisAnnotations = FlowAnalysisAnnotations.None;
            if (module.HasAttribute(handle, AttributeDescription.AllowNullAttribute))
            {
                flowAnalysisAnnotations |= FlowAnalysisAnnotations.AllowNull;
            }
            if (module.HasAttribute(handle, AttributeDescription.DisallowNullAttribute))
            {
                flowAnalysisAnnotations |= FlowAnalysisAnnotations.DisallowNull;
            }
            if (module.HasAttribute(handle, AttributeDescription.MaybeNullAttribute))
            {
                flowAnalysisAnnotations |= FlowAnalysisAnnotations.MaybeNull;
            }
            else if (module.HasMaybeNullWhenOrNotNullWhenOrDoesNotReturnIfAttribute(handle, AttributeDescription.MaybeNullWhenAttribute, out bool when))
            {
                flowAnalysisAnnotations |= (when ? FlowAnalysisAnnotations.MaybeNullWhenTrue : FlowAnalysisAnnotations.MaybeNullWhenFalse);
            }
            if (module.HasAttribute(handle, AttributeDescription.NotNullAttribute))
            {
                flowAnalysisAnnotations |= FlowAnalysisAnnotations.NotNull;
            }
            else if (module.HasMaybeNullWhenOrNotNullWhenOrDoesNotReturnIfAttribute(handle, AttributeDescription.NotNullWhenAttribute, out bool when2))
            {
                flowAnalysisAnnotations |= (when2 ? FlowAnalysisAnnotations.NotNullWhenTrue : FlowAnalysisAnnotations.NotNullWhenFalse);
            }
            if (module.HasMaybeNullWhenOrNotNullWhenOrDoesNotReturnIfAttribute(handle, AttributeDescription.DoesNotReturnIfAttribute, out var when3))
            {
                flowAnalysisAnnotations |= (when3 ? FlowAnalysisAnnotations.DoesNotReturnIfTrue : FlowAnalysisAnnotations.DoesNotReturnIfFalse);
            }
            return flowAnalysisAnnotations;
        }

        public override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            if (_lazyCustomAttributes.IsDefault)
            {
                PEModuleSymbol pEModuleSymbol = (PEModuleSymbol)ContainingModule;
                bool flag = !_lazyIsParams.HasValue() || _lazyIsParams.Value();
                ConstantValue explicitDefaultConstantValue = ExplicitDefaultConstantValue;
                AttributeDescription filterOut = default(AttributeDescription);
                if ((object)explicitDefaultConstantValue != null)
                {
                    if (explicitDefaultConstantValue.Discriminator == ConstantValueTypeDiscriminator.DateTime)
                    {
                        filterOut = AttributeDescription.DateTimeConstantAttribute;
                    }
                    else if (explicitDefaultConstantValue.Discriminator == ConstantValueTypeDiscriminator.Decimal)
                    {
                        filterOut = AttributeDescription.DecimalConstantAttribute;
                    }
                }
                bool flag2 = RefKind == RefKind.In;
                if (flag || filterOut.Signatures != null || flag2)
                {
                    ImmutableArray<CSharpAttributeData> customAttributesForToken = pEModuleSymbol.GetCustomAttributesForToken(_handle, out CustomAttributeHandle filteredOutAttribute, flag ? AttributeDescription.ParamArrayAttribute : default(AttributeDescription), out CustomAttributeHandle filteredOutAttribute2, filterOut, out CustomAttributeHandle filteredOutAttribute3, flag2 ? AttributeDescription.IsReadOnlyAttribute : default(AttributeDescription), out CustomAttributeHandle filteredOutAttribute4, default(AttributeDescription));
                    if (!filteredOutAttribute.IsNil || !filteredOutAttribute2.IsNil)
                    {
                        ArrayBuilder<CSharpAttributeData> instance = ArrayBuilder<CSharpAttributeData>.GetInstance();
                        if (!filteredOutAttribute.IsNil)
                        {
                            instance.Add(new PEAttributeData(pEModuleSymbol, filteredOutAttribute));
                        }
                        if (!filteredOutAttribute2.IsNil)
                        {
                            instance.Add(new PEAttributeData(pEModuleSymbol, filteredOutAttribute2));
                        }
                        ImmutableInterlocked.InterlockedInitialize(ref _lazyHiddenAttributes, instance.ToImmutableAndFree());
                    }
                    else
                    {
                        ImmutableInterlocked.InterlockedInitialize(ref _lazyHiddenAttributes, ImmutableArray<CSharpAttributeData>.Empty);
                    }
                    if (!_lazyIsParams.HasValue())
                    {
                        _lazyIsParams = (!filteredOutAttribute.IsNil).ToThreeState();
                    }
                    ImmutableInterlocked.InterlockedInitialize(ref _lazyCustomAttributes, customAttributesForToken);
                }
                else
                {
                    ImmutableInterlocked.InterlockedInitialize(ref _lazyHiddenAttributes, ImmutableArray<CSharpAttributeData>.Empty);
                    pEModuleSymbol.LoadCustomAttributes(_handle, ref _lazyCustomAttributes);
                }
            }
            return _lazyCustomAttributes;
        }

        internal override IEnumerable<CSharpAttributeData> GetCustomAttributesToEmit(PEModuleBuilder moduleBuilder)
        {
            ImmutableArray<CSharpAttributeData>.Enumerator enumerator = GetAttributes().GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
            enumerator = _lazyHiddenAttributes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }

        public sealed override bool Equals(Symbol other, TypeCompareKind compareKind)
        {
            if (!(other is NativeIntegerParameterSymbol nativeIntegerParameterSymbol))
            {
                return base.Equals(other, compareKind);
            }
            return nativeIntegerParameterSymbol.Equals(this, compareKind);
        }
    }
}
