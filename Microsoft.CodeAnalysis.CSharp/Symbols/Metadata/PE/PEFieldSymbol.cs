// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.DocumentationComments;
using Microsoft.CodeAnalysis.CSharp.Emit;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols.Metadata.PE
{
    /// <summary>
    /// The class to represent all fields imported from a PE/module.
    /// </summary>
    internal sealed class PEFieldSymbol : FieldSymbol
    {
        private struct PackedFlags
        {
            // Layout:
            // |..............................|vvvvv|
            //
            // f = FlowAnalysisAnnotations. 5 bits (4 value bits + 1 completion bit).

            private const int HasDisallowNullAttribute = 0x1 << 0;
            private const int HasAllowNullAttribute = 0x1 << 1;
            private const int HasMaybeNullAttribute = 0x1 << 2;
            private const int HasNotNullAttribute = 0x1 << 3;
            private const int FlowAnalysisAnnotationsCompletionBit = 0x1 << 4;

            private int _bits;

            public bool SetFlowAnalysisAnnotations(FlowAnalysisAnnotations value)
            {

                int bitsToSet = FlowAnalysisAnnotationsCompletionBit;
                if ((value & FlowAnalysisAnnotations.DisallowNull) != 0) bitsToSet |= PackedFlags.HasDisallowNullAttribute;
                if ((value & FlowAnalysisAnnotations.AllowNull) != 0) bitsToSet |= PackedFlags.HasAllowNullAttribute;
                if ((value & FlowAnalysisAnnotations.MaybeNull) != 0) bitsToSet |= PackedFlags.HasMaybeNullAttribute;
                if ((value & FlowAnalysisAnnotations.NotNull) != 0) bitsToSet |= PackedFlags.HasNotNullAttribute;

                return ThreadSafeFlagOperations.Set(ref _bits, bitsToSet);
            }

            public bool TryGetFlowAnalysisAnnotations(out FlowAnalysisAnnotations value)
            {
                int theBits = _bits; // Read this.bits once to ensure the consistency of the value and completion flags.
                value = FlowAnalysisAnnotations.None;
                if ((theBits & PackedFlags.HasDisallowNullAttribute) != 0) value |= FlowAnalysisAnnotations.DisallowNull;
                if ((theBits & PackedFlags.HasAllowNullAttribute) != 0) value |= FlowAnalysisAnnotations.AllowNull;
                if ((theBits & PackedFlags.HasMaybeNullAttribute) != 0) value |= FlowAnalysisAnnotations.MaybeNull;
                if ((theBits & PackedFlags.HasNotNullAttribute) != 0) value |= FlowAnalysisAnnotations.NotNull;

                var result = (theBits & FlowAnalysisAnnotationsCompletionBit) != 0;
                return result;
            }
        }

        private readonly FieldDefinitionHandle _handle;
        private readonly string _name;
        private readonly FieldAttributes _flags;
        private readonly PENamedTypeSymbol _containingType;
        private bool _lazyIsVolatile;
        private ImmutableArray<CSharpAttributeData> _lazyCustomAttributes;
        private ConstantValue _lazyConstantValue = Microsoft.CodeAnalysis.ConstantValue.Unset; // Indicates an uninitialized ConstantValue
        private Tuple<CultureInfo, string> _lazyDocComment;
        private CachedUseSiteInfo<AssemblySymbol> _lazyCachedUseSiteInfo = CachedUseSiteInfo<AssemblySymbol>.Uninitialized;

        private ObsoleteAttributeData _lazyObsoleteAttributeData = ObsoleteAttributeData.Uninitialized;

        private TypeWithAnnotations.Boxed _lazyType;
        private int _lazyFixedSize;
        private NamedTypeSymbol _lazyFixedImplementationType;
        private PEEventSymbol _associatedEventOpt;
        private PackedFlags _packedFlags;

        internal PEFieldSymbol(
            PEModuleSymbol moduleSymbol,
            PENamedTypeSymbol containingType,
            FieldDefinitionHandle fieldDef)
        {

            _handle = fieldDef;
            _containingType = containingType;
            _packedFlags = new PackedFlags();

            try
            {
                moduleSymbol.Module.GetFieldDefPropsOrThrow(fieldDef, out _name, out _flags);
            }
            catch (BadImageFormatException)
            {
                _name ??= string.Empty;

                _lazyCachedUseSiteInfo.Initialize(new CSDiagnosticInfo(ErrorCode.ERR_BindToBogus, this));
            }
        }

        public override Symbol ContainingSymbol
        {
            get
            {
                return _containingType;
            }
        }

        public override NamedTypeSymbol ContainingType
        {
            get
            {
                return _containingType;
            }
        }

        public override string Name
        {
            get
            {
                return _name;
            }
        }

        internal FieldAttributes Flags
        {
            get
            {
                return _flags;
            }
        }

        internal override bool HasSpecialName
        {
            get
            {
                return (_flags & FieldAttributes.SpecialName) != 0;
            }
        }

        internal override bool HasRuntimeSpecialName
        {
            get
            {
                return (_flags & FieldAttributes.RTSpecialName) != 0;
            }
        }

        internal override bool IsNotSerialized
        {
            get
            {
                return (_flags & FieldAttributes.NotSerialized) != 0;
            }
        }

        internal override MarshalPseudoCustomAttributeData MarshallingInformation
        {
            get
            {
                // the compiler doesn't need full marshalling information, just the unmanaged type or descriptor
                return null;
            }
        }

        internal override bool IsMarshalledExplicitly
        {
            get
            {
                return ((_flags & FieldAttributes.HasFieldMarshal) != 0);
            }
        }

        internal override UnmanagedType MarshallingType
        {
            get
            {
                if ((_flags & FieldAttributes.HasFieldMarshal) == 0)
                {
                    return 0;
                }

                return _containingType.ContainingPEModule.Module.GetMarshallingType(_handle);
            }
        }

        internal override ImmutableArray<byte> MarshallingDescriptor
        {
            get
            {
                if ((_flags & FieldAttributes.HasFieldMarshal) == 0)
                {
                    return default;
                }

                return _containingType.ContainingPEModule.Module.GetMarshallingDescriptor(_handle);
            }
        }

        internal override int? TypeLayoutOffset
        {
            get
            {
                return _containingType.ContainingPEModule.Module.GetFieldOffset(_handle);
            }
        }

        internal FieldDefinitionHandle Handle
        {
            get
            {
                return _handle;
            }
        }

        /// <summary>
        /// Mark this field as the backing field of a field-like event.
        /// The caller will also ensure that it is excluded from the member list of
        /// the containing type (as it would be in source).
        /// </summary>
        internal void SetAssociatedEvent(PEEventSymbol eventSymbol)
        {

            // This should always be true in valid metadata - there should only
            // be one event with a given name in a given type.
            // No locking required since this method will only be called by the thread that created
            // the field symbol (and will be called before the field symbol is added to the containing 
            // type members and available to other threads).
            _associatedEventOpt ??= eventSymbol;
        }

        private void EnsureSignatureIsLoaded()
        {
            if (_lazyType == null)
            {
                var moduleSymbol = _containingType.ContainingPEModule;
                TypeSymbol typeSymbol = (new MetadataDecoder(moduleSymbol, _containingType)).DecodeFieldSignature(_handle, out ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers);
                ImmutableArray<CustomModifier> customModifiersArray = CSharpCustomModifier.Convert(customModifiers);

                typeSymbol = DynamicTypeDecoder.TransformType(typeSymbol, customModifiersArray.Length, _handle, moduleSymbol);
                typeSymbol = NativeIntegerTypeDecoder.TransformType(typeSymbol, _handle, moduleSymbol);

                // We start without annotations
                var type = TypeWithAnnotations.Create(typeSymbol, customModifiers: customModifiersArray);

                // Decode nullable before tuple types to avoid converting between
                // NamedTypeSymbol and TupleTypeSymbol unnecessarily.
                type = NullableTypeDecoder.TransformType(type, _handle, moduleSymbol, accessSymbol: this, nullableContext: _containingType);
                type = TupleTypeDecoder.DecodeTupleTypesIfApplicable(type, _handle, moduleSymbol);

                _lazyIsVolatile = customModifiersArray.Any(m => !m.IsOptional && m.Modifier.SpecialType == SpecialType.System_Runtime_CompilerServices_IsVolatile);

                if (customModifiersArray.IsEmpty && IsFixedBuffer(out int fixedSize, out TypeSymbol fixedElementType))
                {
                    _lazyFixedSize = fixedSize;
                    _lazyFixedImplementationType = type.Type as NamedTypeSymbol;
                    type = TypeWithAnnotations.Create(new PointerTypeSymbol(TypeWithAnnotations.Create(fixedElementType)));
                }

                Interlocked.CompareExchange(ref _lazyType, new TypeWithAnnotations.Boxed(type), null);
            }
        }

        private bool IsFixedBuffer(out int fixedSize, out TypeSymbol fixedElementType)
        {
            fixedSize = 0;
            fixedElementType = null;

            PEModuleSymbol containingPEModule = this.ContainingPEModule;
            if (containingPEModule.Module.HasFixedBufferAttribute(_handle, out string elementTypeName, out int bufferSize))
            {
                var decoder = new MetadataDecoder(containingPEModule);
                var elementType = decoder.GetTypeSymbolForSerializedType(elementTypeName);
                if (elementType.FixedBufferElementSizeInBytes() != 0)
                {
                    fixedSize = bufferSize;
                    fixedElementType = elementType;
                    return true;
                }
            }

            return false;
        }

        private PEModuleSymbol ContainingPEModule
        {
            get
            {
                return ((PENamespaceSymbol)ContainingNamespace).ContainingPEModule;
            }
        }

        internal override TypeWithAnnotations GetFieldType(ConsList<FieldSymbol> fieldsBeingBound)
        {
            EnsureSignatureIsLoaded();
            return _lazyType.Value;
        }

        public override FlowAnalysisAnnotations FlowAnalysisAnnotations
        {
            get
            {
                if (!_packedFlags.TryGetFlowAnalysisAnnotations(out FlowAnalysisAnnotations value))
                {
                    value = DecodeFlowAnalysisAttributes(_containingType.ContainingPEModule.Module, _handle);
                    _packedFlags.SetFlowAnalysisAnnotations(value);
                }
                return value;
            }
        }

        private static FlowAnalysisAnnotations DecodeFlowAnalysisAttributes(PEModule module, FieldDefinitionHandle handle)
        {
            FlowAnalysisAnnotations annotations = FlowAnalysisAnnotations.None;
            if (module.HasAttribute(handle, AttributeDescription.AllowNullAttribute)) annotations |= FlowAnalysisAnnotations.AllowNull;
            if (module.HasAttribute(handle, AttributeDescription.DisallowNullAttribute)) annotations |= FlowAnalysisAnnotations.DisallowNull;
            if (module.HasAttribute(handle, AttributeDescription.MaybeNullAttribute)) annotations |= FlowAnalysisAnnotations.MaybeNull;
            if (module.HasAttribute(handle, AttributeDescription.NotNullAttribute)) annotations |= FlowAnalysisAnnotations.NotNull;
            return annotations;
        }

        public override bool IsFixedSizeBuffer
        {
            get
            {
                EnsureSignatureIsLoaded();
                return _lazyFixedImplementationType is not null;
            }
        }

        public override int FixedSize
        {
            get
            {
                EnsureSignatureIsLoaded();
                return _lazyFixedSize;
            }
        }

        internal override NamedTypeSymbol FixedImplementationType(PEModuleBuilder emitModule)
        {
            EnsureSignatureIsLoaded();
            return _lazyFixedImplementationType;
        }

        public override Symbol AssociatedSymbol
        {
            get
            {
                return _associatedEventOpt;
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                return (_flags & FieldAttributes.InitOnly) != 0;
            }
        }

        public override bool IsVolatile
        {
            get
            {
                EnsureSignatureIsLoaded();
                return _lazyIsVolatile;
            }
        }

        public override bool IsConst
        {
            get
            {
                return (_flags & FieldAttributes.Literal) != 0 || GetConstantValue(ConstantFieldsInProgress.Empty, earlyDecodingWellKnownAttributes: false) != null;
            }
        }

        internal override ConstantValue GetConstantValue(ConstantFieldsInProgress inProgress, bool earlyDecodingWellKnownAttributes)
        {
            if (_lazyConstantValue == Microsoft.CodeAnalysis.ConstantValue.Unset)
            {
                ConstantValue value = null;

                if ((_flags & FieldAttributes.Literal) != 0)
                {
                    value = _containingType.ContainingPEModule.Module.GetConstantFieldValue(_handle);
                }

                // If this is a Decimal, the constant value may come from DecimalConstantAttribute

                if (this.Type.SpecialType == SpecialType.System_Decimal)
                {

                    if (_containingType.ContainingPEModule.Module.HasDecimalConstantAttribute(Handle, out ConstantValue defaultValue))
                    {
                        value = defaultValue;
                    }
                }

                Interlocked.CompareExchange(
                    ref _lazyConstantValue,
                    value,
                    Microsoft.CodeAnalysis.ConstantValue.Unset);
            }

            return _lazyConstantValue;
        }

        public override ImmutableArray<Location> Locations
        {
            get
            {
                return _containingType.ContainingPEModule.MetadataLocation.Cast<MetadataLocation, Location>();
            }
        }

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get
            {
                return ImmutableArray<SyntaxReference>.Empty;
            }
        }

        public override Accessibility DeclaredAccessibility
        {
            get
            {
                var access = (_flags & FieldAttributes.FieldAccessMask) switch
                {
                    FieldAttributes.Assembly => Accessibility.Internal,
                    FieldAttributes.FamORAssem => Accessibility.ProtectedOrInternal,
                    FieldAttributes.FamANDAssem => Accessibility.ProtectedAndInternal,
                    FieldAttributes.Private or FieldAttributes.PrivateScope => Accessibility.Private,
                    FieldAttributes.Public => Accessibility.Public,
                    FieldAttributes.Family => Accessibility.Protected,
                    _ => Accessibility.Private,
                };
                return access;
            }
        }

        public override bool IsStatic
        {
            get
            {
                return (_flags & FieldAttributes.Static) != 0;
            }
        }

        public override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            if (_lazyCustomAttributes.IsDefault)
            {
                var containingPEModuleSymbol = (PEModuleSymbol)this.ContainingModule;

                if (FilterOutDecimalConstantAttribute())
                {
                    // filter out DecimalConstantAttribute
                    var attributes = containingPEModuleSymbol.GetCustomAttributesForToken(
                        _handle,
                        out _,
                        AttributeDescription.DecimalConstantAttribute);

                    ImmutableInterlocked.InterlockedInitialize(ref _lazyCustomAttributes, attributes);
                }
                else
                {
                    containingPEModuleSymbol.LoadCustomAttributes(_handle, ref _lazyCustomAttributes);
                }
            }
            return _lazyCustomAttributes;
        }

        private bool FilterOutDecimalConstantAttribute()
        {
            ConstantValue value;
            return this.Type.SpecialType == SpecialType.System_Decimal &&
                   (value = GetConstantValue(ConstantFieldsInProgress.Empty, earlyDecodingWellKnownAttributes: false)) is not null &&
                   value.Discriminator == ConstantValueTypeDiscriminator.Decimal;
        }

        internal override IEnumerable<CSharpAttributeData> GetCustomAttributesToEmit(PEModuleBuilder moduleBuilder)
        {
            foreach (CSharpAttributeData attribute in GetAttributes())
            {
                yield return attribute;
            }

            // Yield hidden attributes last, order might be important.
            if (FilterOutDecimalConstantAttribute())
            {
                var containingPEModuleSymbol = _containingType.ContainingPEModule;
                yield return new PEAttributeData(containingPEModuleSymbol,
                                          containingPEModuleSymbol.Module.FindLastTargetAttribute(_handle, AttributeDescription.DecimalConstantAttribute).Handle);
            }
        }

        public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default)
        {
            return PEDocumentationCommentUtils.GetDocumentationComment(this, _containingType.ContainingPEModule, preferredCulture, cancellationToken, ref _lazyDocComment);
        }

        internal override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
        {
            AssemblySymbol primaryDependency = PrimaryDependency;

            if (!_lazyCachedUseSiteInfo.IsInitialized)
            {
                UseSiteInfo<AssemblySymbol> result = new(primaryDependency);
                CalculateUseSiteDiagnostic(ref result);
                _lazyCachedUseSiteInfo.Initialize(primaryDependency, result);
            }

            return _lazyCachedUseSiteInfo.ToUseSiteInfo(primaryDependency);
        }

        internal override ObsoleteAttributeData ObsoleteAttributeData
        {
            get
            {
                ObsoleteAttributeHelpers.InitializeObsoleteDataFromMetadata(ref _lazyObsoleteAttributeData, _handle, (PEModuleSymbol)(this.ContainingModule), ignoreByRefLikeMarker: false);
                return _lazyObsoleteAttributeData;
            }
        }

        internal sealed override CSharpCompilation DeclaringCompilation // perf, not correctness
        {
            get { return null; }
        }
    }
}
