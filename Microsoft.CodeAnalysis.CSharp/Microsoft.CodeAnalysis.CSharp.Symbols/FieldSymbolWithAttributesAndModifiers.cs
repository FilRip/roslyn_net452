using System;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public abstract class FieldSymbolWithAttributesAndModifiers : FieldSymbol, IAttributeTargetSymbol
    {
        private CustomAttributesBag<CSharpAttributeData> _lazyCustomAttributesBag;

        protected SymbolCompletionState state;

        internal abstract Location ErrorLocation { get; }

        protected abstract DeclarationModifiers Modifiers { get; }

        protected abstract SyntaxList<AttributeListSyntax> AttributeDeclarationSyntaxList { get; }

        protected abstract IAttributeTargetSymbol AttributeOwner { get; }

        IAttributeTargetSymbol IAttributeTargetSymbol.AttributesOwner => AttributeOwner;

        AttributeLocation IAttributeTargetSymbol.DefaultAttributeLocation => AttributeLocation.Field;

        AttributeLocation IAttributeTargetSymbol.AllowedAttributeLocations => AttributeLocation.Field;

        public sealed override bool IsStatic => (Modifiers & DeclarationModifiers.Static) != 0;

        public sealed override bool IsReadOnly => (Modifiers & DeclarationModifiers.ReadOnly) != 0;

        public sealed override Accessibility DeclaredAccessibility => ModifierUtils.EffectiveAccessibility(Modifiers);

        public sealed override bool IsConst => (Modifiers & DeclarationModifiers.Const) != 0;

        public sealed override bool IsVolatile => (Modifiers & DeclarationModifiers.Volatile) != 0;

        public sealed override bool IsFixedSizeBuffer => (Modifiers & DeclarationModifiers.Fixed) != 0;

        internal sealed override ObsoleteAttributeData ObsoleteAttributeData
        {
            get
            {
                if (!((SourceMemberContainerTypeSymbol)ContainingType).AnyMemberHasAttributes)
                {
                    return null;
                }
                CustomAttributesBag<CSharpAttributeData> lazyCustomAttributesBag = _lazyCustomAttributesBag;
                if (lazyCustomAttributesBag != null && lazyCustomAttributesBag.IsEarlyDecodedWellKnownAttributeDataComputed)
                {
                    return ((CommonFieldEarlyWellKnownAttributeData)lazyCustomAttributesBag.EarlyDecodedWellKnownAttributeData)?.ObsoleteAttributeData;
                }
                return ObsoleteAttributeData.Uninitialized;
            }
        }

        public override FlowAnalysisAnnotations FlowAnalysisAnnotations => DecodeFlowAnalysisAttributes(GetDecodedWellKnownAttributeData());

        internal sealed override bool HasSpecialName
        {
            get
            {
                if (HasRuntimeSpecialName)
                {
                    return true;
                }
                return GetDecodedWellKnownAttributeData()?.HasSpecialNameAttribute ?? false;
            }
        }

        internal sealed override bool IsNotSerialized => GetDecodedWellKnownAttributeData()?.HasNonSerializedAttribute ?? false;

        internal sealed override MarshalPseudoCustomAttributeData MarshallingInformation => GetDecodedWellKnownAttributeData()?.MarshallingInformation;

        internal sealed override int? TypeLayoutOffset => GetDecodedWellKnownAttributeData()?.Offset;

        internal sealed override bool HasComplete(CompletionPart part)
        {
            return state.HasComplete(part);
        }

        public sealed override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            return GetAttributesBag().Attributes;
        }

        private CustomAttributesBag<CSharpAttributeData> GetAttributesBag()
        {
            CustomAttributesBag<CSharpAttributeData> lazyCustomAttributesBag = _lazyCustomAttributesBag;
            if (lazyCustomAttributesBag != null && lazyCustomAttributesBag.IsSealed)
            {
                return lazyCustomAttributesBag;
            }
            if (LoadAndValidateAttributes(OneOrMany.Create(AttributeDeclarationSyntaxList), ref _lazyCustomAttributesBag))
            {
                state.NotePartComplete(CompletionPart.Attributes);
            }
            return _lazyCustomAttributesBag;
        }

        protected FieldWellKnownAttributeData GetDecodedWellKnownAttributeData()
        {
            CustomAttributesBag<CSharpAttributeData> customAttributesBag = _lazyCustomAttributesBag;
            if (customAttributesBag == null || !customAttributesBag.IsDecodedWellKnownAttributeDataComputed)
            {
                customAttributesBag = GetAttributesBag();
            }
            return (FieldWellKnownAttributeData)customAttributesBag.DecodedWellKnownAttributeData;
        }

        internal sealed override CSharpAttributeData EarlyDecodeWellKnownAttribute(ref EarlyDecodeWellKnownAttributeArguments<EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation> arguments)
        {
            if (Symbol.EarlyDecodeDeprecatedOrExperimentalOrObsoleteAttribute(ref arguments, out var attributeData, out var obsoleteData))
            {
                if (obsoleteData != null)
                {
                    arguments.GetOrCreateData<CommonFieldEarlyWellKnownAttributeData>().ObsoleteAttributeData = obsoleteData;
                }
                return attributeData;
            }
            return base.EarlyDecodeWellKnownAttribute(ref arguments);
        }

        internal override void DecodeWellKnownAttribute(ref DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments)
        {
            BindingDiagnosticBag bindingDiagnosticBag = (BindingDiagnosticBag)arguments.Diagnostics;
            CSharpAttributeData attribute = arguments.Attribute;
            if (attribute.IsTargetAttribute(this, AttributeDescription.SpecialNameAttribute))
            {
                arguments.GetOrCreateData<FieldWellKnownAttributeData>().HasSpecialNameAttribute = true;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.NonSerializedAttribute))
            {
                arguments.GetOrCreateData<FieldWellKnownAttributeData>().HasNonSerializedAttribute = true;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.FieldOffsetAttribute))
            {
                if (IsStatic || IsConst)
                {
                    bindingDiagnosticBag.Add(ErrorCode.ERR_StructOffsetOnBadField, arguments.AttributeSyntaxOpt!.Name.Location, arguments.AttributeSyntaxOpt!.GetErrorDisplayName());
                    return;
                }
                int num = attribute.CommonConstructorArguments[0].DecodeValue<int>(SpecialType.System_Int32);
                if (num < 0)
                {
                    CSharpSyntaxNode attributeArgumentSyntax = attribute.GetAttributeArgumentSyntax(0, arguments.AttributeSyntaxOpt);
                    bindingDiagnosticBag.Add(ErrorCode.ERR_InvalidAttributeArgument, attributeArgumentSyntax.Location, arguments.AttributeSyntaxOpt!.GetErrorDisplayName());
                    num = 0;
                }
                arguments.GetOrCreateData<FieldWellKnownAttributeData>().SetFieldOffset(num);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.MarshalAsAttribute))
            {
                MarshalAsAttributeDecoder<FieldWellKnownAttributeData, AttributeSyntax, CSharpAttributeData, AttributeLocation>.Decode(ref arguments, AttributeTargets.Field, MessageProvider.Instance);
            }
            else if (!ReportExplicitUseOfReservedAttributes(in arguments, ReservedAttributes.DynamicAttribute | ReservedAttributes.IsReadOnlyAttribute | ReservedAttributes.IsUnmanagedAttribute | ReservedAttributes.IsByRefLikeAttribute | ReservedAttributes.TupleElementNamesAttribute | ReservedAttributes.NullableAttribute | ReservedAttributes.NativeIntegerAttribute))
            {
                if (attribute.IsTargetAttribute(this, AttributeDescription.DateTimeConstantAttribute))
                {
                    VerifyConstantValueMatches(attribute.DecodeDateTimeConstantValue(), ref arguments);
                }
                else if (attribute.IsTargetAttribute(this, AttributeDescription.DecimalConstantAttribute))
                {
                    VerifyConstantValueMatches(attribute.DecodeDecimalConstantValue(), ref arguments);
                }
                else if (attribute.IsTargetAttribute(this, AttributeDescription.AllowNullAttribute))
                {
                    arguments.GetOrCreateData<FieldWellKnownAttributeData>().HasAllowNullAttribute = true;
                }
                else if (attribute.IsTargetAttribute(this, AttributeDescription.DisallowNullAttribute))
                {
                    arguments.GetOrCreateData<FieldWellKnownAttributeData>().HasDisallowNullAttribute = true;
                }
                else if (attribute.IsTargetAttribute(this, AttributeDescription.MaybeNullAttribute))
                {
                    arguments.GetOrCreateData<FieldWellKnownAttributeData>().HasMaybeNullAttribute = true;
                }
                else if (attribute.IsTargetAttribute(this, AttributeDescription.NotNullAttribute))
                {
                    arguments.GetOrCreateData<FieldWellKnownAttributeData>().HasNotNullAttribute = true;
                }
            }
        }

        private static FlowAnalysisAnnotations DecodeFlowAnalysisAttributes(FieldWellKnownAttributeData attributeData)
        {
            FlowAnalysisAnnotations flowAnalysisAnnotations = FlowAnalysisAnnotations.None;
            if (attributeData != null)
            {
                if (attributeData.HasAllowNullAttribute)
                {
                    flowAnalysisAnnotations |= FlowAnalysisAnnotations.AllowNull;
                }
                if (attributeData.HasDisallowNullAttribute)
                {
                    flowAnalysisAnnotations |= FlowAnalysisAnnotations.DisallowNull;
                }
                if (attributeData.HasMaybeNullAttribute)
                {
                    flowAnalysisAnnotations |= FlowAnalysisAnnotations.MaybeNull;
                }
                if (attributeData.HasNotNullAttribute)
                {
                    flowAnalysisAnnotations |= FlowAnalysisAnnotations.NotNull;
                }
            }
            return flowAnalysisAnnotations;
        }

        private void VerifyConstantValueMatches(ConstantValue attrValue, ref DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments)
        {
            if (attrValue.IsBad)
            {
                return;
            }
            FieldWellKnownAttributeData orCreateData = arguments.GetOrCreateData<FieldWellKnownAttributeData>();
            BindingDiagnosticBag bindingDiagnosticBag = (BindingDiagnosticBag)arguments.Diagnostics;
            ConstantValue constantValue;
            if (IsConst)
            {
                if (base.Type.SpecialType == SpecialType.System_Decimal)
                {
                    constantValue = GetConstantValue(ConstantFieldsInProgress.Empty, earlyDecodingWellKnownAttributes: false);
                    if ((object)constantValue != null && !constantValue.IsBad && constantValue != attrValue)
                    {
                        bindingDiagnosticBag.Add(ErrorCode.ERR_FieldHasMultipleDistinctConstantValues, arguments.AttributeSyntaxOpt!.Location);
                    }
                }
                else
                {
                    bindingDiagnosticBag.Add(ErrorCode.ERR_FieldHasMultipleDistinctConstantValues, arguments.AttributeSyntaxOpt!.Location);
                }
                if (orCreateData.ConstValue == Microsoft.CodeAnalysis.ConstantValue.Unset)
                {
                    orCreateData.ConstValue = attrValue;
                }
                return;
            }
            constantValue = orCreateData.ConstValue;
            if (constantValue != Microsoft.CodeAnalysis.ConstantValue.Unset)
            {
                if (constantValue != attrValue)
                {
                    bindingDiagnosticBag.Add(ErrorCode.ERR_FieldHasMultipleDistinctConstantValues, arguments.AttributeSyntaxOpt!.Location);
                }
            }
            else
            {
                orCreateData.ConstValue = attrValue;
            }
        }

        internal override void PostDecodeWellKnownAttributes(ImmutableArray<CSharpAttributeData> boundAttributes, ImmutableArray<AttributeSyntax> allAttributeSyntaxNodes, BindingDiagnosticBag diagnostics, AttributeLocation symbolPart, WellKnownAttributeData decodedData)
        {
            if ((((FieldWellKnownAttributeData)decodedData)?.Offset).HasValue)
            {
                if (ContainingType.Layout.Kind != LayoutKind.Explicit)
                {
                    int index = boundAttributes.IndexOfAttribute(this, AttributeDescription.FieldOffsetAttribute);
                    diagnostics.Add(ErrorCode.ERR_StructOffsetOnBadStruct, allAttributeSyntaxNodes[index].Name.Location);
                }
            }
            else if (!IsStatic && !IsConst && ContainingType.Layout.Kind == LayoutKind.Explicit)
            {
                diagnostics.Add(ErrorCode.ERR_MissingStructOffset, ErrorLocation, AttributeOwner);
            }
            base.PostDecodeWellKnownAttributes(boundAttributes, allAttributeSyntaxNodes, diagnostics, symbolPart, decodedData);
        }

        internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
            base.AddSynthesizedAttributes(moduleBuilder, ref attributes);
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            TypeWithAnnotations typeWithAnnotations = base.TypeWithAnnotations;
            if (typeWithAnnotations.Type.ContainsDynamic())
            {
                Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.SynthesizeDynamicAttribute(typeWithAnnotations.Type, typeWithAnnotations.CustomModifiers.Length));
            }
            if (typeWithAnnotations.Type.ContainsNativeInteger())
            {
                Symbol.AddSynthesizedAttribute(ref attributes, moduleBuilder.SynthesizeNativeIntegerAttribute(this, typeWithAnnotations.Type));
            }
            if (typeWithAnnotations.Type.ContainsTupleNames())
            {
                Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.SynthesizeTupleNamesAttribute(typeWithAnnotations.Type));
            }
            if (declaringCompilation.ShouldEmitNullableAttributes(this))
            {
                Symbol.AddSynthesizedAttribute(ref attributes, moduleBuilder.SynthesizeNullableAttributeIfNecessary(this, ContainingType.GetNullableContextValue(), typeWithAnnotations));
            }
        }
    }
}
