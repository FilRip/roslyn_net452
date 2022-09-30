using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal class SourceComplexParameterSymbol : SourceParameterSymbol, IAttributeTargetSymbol
    {
        [Flags()]
        private enum ParameterSyntaxKind : byte
        {
            Regular = 0,
            ParamsParameter = 1,
            ExtensionThisParameter = 2,
            DefaultParameter = 4
        }

        private readonly SyntaxReference _syntaxRef;

        private readonly ParameterSyntaxKind _parameterSyntaxKind;

        private CustomAttributesBag<CSharpAttributeData> _lazyCustomAttributesBag;

        private ThreeState _lazyHasOptionalAttribute;

        protected ConstantValue _lazyDefaultSyntaxValue;

        private Binder ParameterBinderOpt => (ContainingSymbol as SourceMethodSymbolWithAttributes)?.ParameterBinder;

        internal sealed override SyntaxReference SyntaxReference => _syntaxRef;

        private ParameterSyntax CSharpSyntaxNode => (ParameterSyntax)(_syntaxRef?.GetSyntax());

        public override bool IsDiscard => false;

        internal sealed override ConstantValue ExplicitDefaultConstantValue => DefaultSyntaxValue ?? DefaultValueFromAttributes;

        internal sealed override ConstantValue DefaultValueFromAttributes
        {
            get
            {
                ParameterEarlyWellKnownAttributeData earlyDecodedWellKnownAttributeData = GetEarlyDecodedWellKnownAttributeData();
                if (earlyDecodedWellKnownAttributeData == null || !(earlyDecodedWellKnownAttributeData.DefaultParameterValue != ConstantValue.Unset))
                {
                    return null;
                }
                return earlyDecodedWellKnownAttributeData.DefaultParameterValue;
            }
        }

        internal sealed override bool IsIDispatchConstant => GetDecodedWellKnownAttributeData()?.HasIDispatchConstantAttribute ?? false;

        internal override bool IsIUnknownConstant => GetDecodedWellKnownAttributeData()?.HasIUnknownConstantAttribute ?? false;

        private bool HasCallerLineNumberAttribute => GetEarlyDecodedWellKnownAttributeData()?.HasCallerLineNumberAttribute ?? false;

        private bool HasCallerFilePathAttribute => GetEarlyDecodedWellKnownAttributeData()?.HasCallerFilePathAttribute ?? false;

        private bool HasCallerMemberNameAttribute => GetEarlyDecodedWellKnownAttributeData()?.HasCallerMemberNameAttribute ?? false;

        internal sealed override bool IsCallerLineNumber => HasCallerLineNumberAttribute;

        internal sealed override bool IsCallerFilePath
        {
            get
            {
                if (!HasCallerLineNumberAttribute)
                {
                    return HasCallerFilePathAttribute;
                }
                return false;
            }
        }

        internal sealed override bool IsCallerMemberName
        {
            get
            {
                if (!HasCallerLineNumberAttribute && !HasCallerFilePathAttribute)
                {
                    return HasCallerMemberNameAttribute;
                }
                return false;
            }
        }

        internal override FlowAnalysisAnnotations FlowAnalysisAnnotations => DecodeFlowAnalysisAttributes(GetDecodedWellKnownAttributeData());

        internal override ImmutableHashSet<string> NotNullIfParameterNotNull => GetDecodedWellKnownAttributeData()?.NotNullIfParameterNotNull ?? ImmutableHashSet<string>.Empty;

        internal bool HasEnumeratorCancellationAttribute => GetDecodedWellKnownAttributeData()?.HasEnumeratorCancellationAttribute ?? false;

        private ConstantValue DefaultSyntaxValue
        {
            get
            {
                if (state.NotePartComplete(CompletionPart.Members))
                {
                    BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
                    Interlocked.CompareExchange(ref _lazyDefaultSyntaxValue, MakeDefaultExpression(instance, out var binder, out var parameterEqualsValue), ConstantValue.Unset);
                    state.NotePartComplete(CompletionPart.TypeMembers);
                    if (parameterEqualsValue != null)
                    {
                        if (binder != null)
                        {
                            SyntaxNode defaultValueSyntaxForIsNullableAnalysisEnabled = GetDefaultValueSyntaxForIsNullableAnalysisEnabled(CSharpSyntaxNode);
                            if (defaultValueSyntaxForIsNullableAnalysisEnabled != null)
                            {
                                NullableWalker.AnalyzeIfNeeded(binder, parameterEqualsValue, defaultValueSyntaxForIsNullableAnalysisEnabled, instance.DiagnosticBag);
                            }
                        }
                        if (!_lazyDefaultSyntaxValue.IsBad)
                        {
                            VerifyParamDefaultValueMatchesAttributeIfAny(_lazyDefaultSyntaxValue, parameterEqualsValue.Value.Syntax, instance);
                        }
                    }
                    AddDeclarationDiagnostics(instance);
                    instance.Free();
                    state.NotePartComplete(CompletionPart.SynthesizedExplicitImplementations);
                }
                state.SpinWaitComplete(CompletionPart.TypeMembers, default(CancellationToken));
                return _lazyDefaultSyntaxValue;
            }
        }

        public override string MetadataName
        {
            get
            {
                if (!(ContainingSymbol is SourceOrdinaryMethodSymbol sourceOrdinaryMethodSymbol))
                {
                    return base.MetadataName;
                }
                SourceOrdinaryMethodSymbol sourcePartialDefinition = sourceOrdinaryMethodSymbol.SourcePartialDefinition;
                if ((object)sourcePartialDefinition == null)
                {
                    return base.MetadataName;
                }
                return sourcePartialDefinition.Parameters[Ordinal].MetadataName;
            }
        }

        protected virtual IAttributeTargetSymbol AttributeOwner => this;

        IAttributeTargetSymbol IAttributeTargetSymbol.AttributesOwner => AttributeOwner;

        AttributeLocation IAttributeTargetSymbol.DefaultAttributeLocation => AttributeLocation.Parameter;

        AttributeLocation IAttributeTargetSymbol.AllowedAttributeLocations
        {
            get
            {
                if (SynthesizedRecordPropertySymbol.HaveCorrespondingSynthesizedRecordPropertySymbol(this))
                {
                    return AttributeLocation.Field | AttributeLocation.Property | AttributeLocation.Parameter;
                }
                return AttributeLocation.Parameter;
            }
        }

        private SourceParameterSymbol BoundAttributesSource
        {
            get
            {
                if (!(ContainingSymbol is SourceOrdinaryMethodSymbol sourceOrdinaryMethodSymbol))
                {
                    return null;
                }
                SourceOrdinaryMethodSymbol sourcePartialImplementation = sourceOrdinaryMethodSymbol.SourcePartialImplementation;
                if ((object)sourcePartialImplementation == null)
                {
                    return null;
                }
                return (SourceParameterSymbol)sourcePartialImplementation.Parameters[Ordinal];
            }
        }

        internal sealed override SyntaxList<AttributeListSyntax> AttributeDeclarationList => CSharpSyntaxNode?.AttributeLists ?? default(SyntaxList<AttributeListSyntax>);

        internal override bool HasDefaultArgumentSyntax => (_parameterSyntaxKind & ParameterSyntaxKind.DefaultParameter) != 0;

        internal sealed override bool HasOptionalAttribute
        {
            get
            {
                if (_lazyHasOptionalAttribute == ThreeState.Unknown)
                {
                    SourceParameterSymbol boundAttributesSource = BoundAttributesSource;
                    if ((object)boundAttributesSource != null)
                    {
                        _lazyHasOptionalAttribute = boundAttributesSource.HasOptionalAttribute.ToThreeState();
                    }
                    else if (!GetAttributes().Any())
                    {
                        _lazyHasOptionalAttribute = ThreeState.False;
                    }
                }
                return _lazyHasOptionalAttribute.Value();
            }
        }

        internal override bool IsMetadataOptional
        {
            get
            {
                if (!HasDefaultArgumentSyntax)
                {
                    return HasOptionalAttribute;
                }
                return true;
            }
        }

        internal sealed override bool IsMetadataIn
        {
            get
            {
                if (!base.IsMetadataIn)
                {
                    return GetDecodedWellKnownAttributeData()?.HasInAttribute ?? false;
                }
                return true;
            }
        }

        internal sealed override bool IsMetadataOut
        {
            get
            {
                if (!base.IsMetadataOut)
                {
                    return GetDecodedWellKnownAttributeData()?.HasOutAttribute ?? false;
                }
                return true;
            }
        }

        internal sealed override MarshalPseudoCustomAttributeData MarshallingInformation => GetDecodedWellKnownAttributeData()?.MarshallingInformation;

        public override bool IsParams => (_parameterSyntaxKind & ParameterSyntaxKind.ParamsParameter) != 0;

        internal override bool IsExtensionMethodThis => (_parameterSyntaxKind & ParameterSyntaxKind.ExtensionThisParameter) != 0;

        public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

        internal SourceComplexParameterSymbol(Symbol owner, int ordinal, TypeWithAnnotations parameterType, RefKind refKind, string name, ImmutableArray<Location> locations, SyntaxReference syntaxRef, bool isParams, bool isExtensionMethodThis)
            : base(owner, parameterType, ordinal, refKind, name, locations)
        {
            _lazyHasOptionalAttribute = ThreeState.Unknown;
            _syntaxRef = syntaxRef;
            if (isParams)
            {
                _parameterSyntaxKind |= ParameterSyntaxKind.ParamsParameter;
            }
            if (isExtensionMethodThis)
            {
                _parameterSyntaxKind |= ParameterSyntaxKind.ExtensionThisParameter;
            }
            ParameterSyntax cSharpSyntaxNode = CSharpSyntaxNode;
            if (cSharpSyntaxNode != null && cSharpSyntaxNode.Default != null)
            {
                _parameterSyntaxKind |= ParameterSyntaxKind.DefaultParameter;
            }
            _lazyDefaultSyntaxValue = ConstantValue.Unset;
        }

        private static FlowAnalysisAnnotations DecodeFlowAnalysisAttributes(ParameterWellKnownAttributeData attributeData)
        {
            if (attributeData == null)
            {
                return FlowAnalysisAnnotations.None;
            }
            FlowAnalysisAnnotations flowAnalysisAnnotations = FlowAnalysisAnnotations.None;
            if (attributeData.HasAllowNullAttribute)
            {
                flowAnalysisAnnotations |= FlowAnalysisAnnotations.AllowNull;
            }
            if (attributeData.HasDisallowNullAttribute)
            {
                flowAnalysisAnnotations |= FlowAnalysisAnnotations.DisallowNull;
            }
            bool? maybeNullWhenAttribute;
            if (attributeData.HasMaybeNullAttribute)
            {
                flowAnalysisAnnotations |= FlowAnalysisAnnotations.MaybeNull;
            }
            else
            {
                maybeNullWhenAttribute = attributeData.MaybeNullWhenAttribute;
                if (maybeNullWhenAttribute.HasValue)
                {
                    bool valueOrDefault = maybeNullWhenAttribute.GetValueOrDefault();
                    flowAnalysisAnnotations |= (valueOrDefault ? FlowAnalysisAnnotations.MaybeNullWhenTrue : FlowAnalysisAnnotations.MaybeNullWhenFalse);
                }
            }
            if (attributeData.HasNotNullAttribute)
            {
                flowAnalysisAnnotations |= FlowAnalysisAnnotations.NotNull;
            }
            else
            {
                maybeNullWhenAttribute = attributeData.NotNullWhenAttribute;
                if (maybeNullWhenAttribute.HasValue)
                {
                    bool valueOrDefault2 = maybeNullWhenAttribute.GetValueOrDefault();
                    flowAnalysisAnnotations |= (valueOrDefault2 ? FlowAnalysisAnnotations.NotNullWhenTrue : FlowAnalysisAnnotations.NotNullWhenFalse);
                }
            }
            maybeNullWhenAttribute = attributeData.DoesNotReturnIfAttribute;
            if (maybeNullWhenAttribute.HasValue)
            {
                bool valueOrDefault3 = maybeNullWhenAttribute.GetValueOrDefault();
                flowAnalysisAnnotations |= (valueOrDefault3 ? FlowAnalysisAnnotations.DoesNotReturnIfTrue : FlowAnalysisAnnotations.DoesNotReturnIfFalse);
            }
            return flowAnalysisAnnotations;
        }

        internal static SyntaxNode? GetDefaultValueSyntaxForIsNullableAnalysisEnabled(ParameterSyntax? parameterSyntax)
        {
            return parameterSyntax?.Default?.Value;
        }

        private Binder GetBinder(SyntaxNode syntax)
        {
            Binder binder = ParameterBinderOpt;
            if (binder == null)
            {
                binder = DeclaringCompilation.GetBinderFactory(syntax.SyntaxTree).GetBinder(syntax);
            }
            return binder;
        }

        private void NullableAnalyzeParameterDefaultValueFromAttributes()
        {
            ParameterSyntax cSharpSyntaxNode = CSharpSyntaxNode;
            if (cSharpSyntaxNode == null)
            {
                return;
            }
            SyntaxNode node = cSharpSyntaxNode.AttributeLists.Node;
            if (node != null && NullableWalker.NeedsAnalysis(DeclaringCompilation, node))
            {
                ConstantValue defaultValueFromAttributes = DefaultValueFromAttributes;
                if (!(defaultValueFromAttributes == null) && !defaultValueFromAttributes.IsBad)
                {
                    Binder binder = GetBinder(cSharpSyntaxNode);
                    BoundParameterEqualsValue node2 = new BoundParameterEqualsValue(cSharpSyntaxNode, this, ImmutableArray<LocalSymbol>.Empty, new BoundLiteral(cSharpSyntaxNode, defaultValueFromAttributes, base.Type));
                    BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance(withDiagnostics: true, withDependencies: false);
                    NullableWalker.AnalyzeIfNeeded(binder, node2, cSharpSyntaxNode, instance.DiagnosticBag);
                    AddDeclarationDiagnostics(instance);
                    instance.Free();
                }
            }
        }

        private ConstantValue MakeDefaultExpression(BindingDiagnosticBag diagnostics, out Binder? binder, out BoundParameterEqualsValue? parameterEqualsValue)
        {
            binder = null;
            parameterEqualsValue = null;
            ParameterSyntax cSharpSyntaxNode = CSharpSyntaxNode;
            if (cSharpSyntaxNode == null)
            {
                return null;
            }
            EqualsValueClauseSyntax @default = cSharpSyntaxNode.Default;
            if (@default == null)
            {
                return null;
            }
            binder = GetBinder(@default);
            Binder binder2 = binder!.CreateBinderForParameterDefaultValue(this, @default);
            parameterEqualsValue = binder2.BindParameterDefaultValue(@default, this, diagnostics, out var valueBeforeConversion);
            if (valueBeforeConversion.HasErrors)
            {
                return ConstantValue.Bad;
            }
            BoundExpression boundExpression = parameterEqualsValue!.Value;
            if (ParameterHelpers.ReportDefaultParameterErrors(binder, ContainingSymbol, cSharpSyntaxNode, this, valueBeforeConversion, boundExpression, diagnostics))
            {
                return ConstantValue.Bad;
            }
            if (boundExpression.ConstantValue == null && boundExpression.Kind == BoundKind.Conversion && ((BoundConversion)boundExpression).ConversionKind != ConversionKind.DefaultLiteral && parameterType.Type.IsNullableType())
            {
                boundExpression = binder!.GenerateConversionForAssignment(parameterType.Type.GetNullableUnderlyingType(), valueBeforeConversion, diagnostics, isDefaultParameter: true);
            }
            return boundExpression.ConstantValue ?? ConstantValue.Null;
        }

        internal virtual OneOrMany<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
        {
            SyntaxList<AttributeListSyntax> attributeDeclarationList = AttributeDeclarationList;
            if (!(ContainingSymbol is SourceOrdinaryMethodSymbol sourceOrdinaryMethodSymbol))
            {
                return OneOrMany.Create(attributeDeclarationList);
            }
            SourceOrdinaryMethodSymbol otherPartOfPartial = sourceOrdinaryMethodSymbol.OtherPartOfPartial;
            SyntaxList<AttributeListSyntax> syntaxList = (((object)otherPartOfPartial == null) ? default(SyntaxList<AttributeListSyntax>) : ((SourceParameterSymbol)otherPartOfPartial.Parameters[Ordinal]).AttributeDeclarationList);
            if (attributeDeclarationList.Equals(default(SyntaxList<AttributeListSyntax>)))
            {
                return OneOrMany.Create(syntaxList);
            }
            if (syntaxList.Equals(default(SyntaxList<AttributeListSyntax>)))
            {
                return OneOrMany.Create(attributeDeclarationList);
            }
            return OneOrMany.Create(ImmutableArray.Create(attributeDeclarationList, syntaxList));
        }

        internal ParameterWellKnownAttributeData GetDecodedWellKnownAttributeData()
        {
            CustomAttributesBag<CSharpAttributeData> customAttributesBag = _lazyCustomAttributesBag;
            if (customAttributesBag == null || !customAttributesBag.IsDecodedWellKnownAttributeDataComputed)
            {
                customAttributesBag = GetAttributesBag();
            }
            return (ParameterWellKnownAttributeData)customAttributesBag.DecodedWellKnownAttributeData;
        }

        internal ParameterEarlyWellKnownAttributeData GetEarlyDecodedWellKnownAttributeData()
        {
            CustomAttributesBag<CSharpAttributeData> customAttributesBag = _lazyCustomAttributesBag;
            if (customAttributesBag == null || !customAttributesBag.IsEarlyDecodedWellKnownAttributeDataComputed)
            {
                customAttributesBag = GetAttributesBag();
            }
            return (ParameterEarlyWellKnownAttributeData)customAttributesBag.EarlyDecodedWellKnownAttributeData;
        }

        internal sealed override CustomAttributesBag<CSharpAttributeData> GetAttributesBag()
        {
            if (_lazyCustomAttributesBag == null || !_lazyCustomAttributesBag.IsSealed)
            {
                SourceParameterSymbol boundAttributesSource = BoundAttributesSource;
                bool flag;
                if ((object)boundAttributesSource != null)
                {
                    CustomAttributesBag<CSharpAttributeData> attributesBag = boundAttributesSource.GetAttributesBag();
                    flag = Interlocked.CompareExchange(ref _lazyCustomAttributesBag, attributesBag, null) == null;
                }
                else
                {
                    OneOrMany<SyntaxList<AttributeListSyntax>> attributeDeclarations = GetAttributeDeclarations();
                    flag = LoadAndValidateAttributes(attributeDeclarations, ref _lazyCustomAttributesBag, AttributeLocation.None, earlyDecodingOnly: false, ParameterBinderOpt);
                }
                if (flag)
                {
                    NullableAnalyzeParameterDefaultValueFromAttributes();
                    state.NotePartComplete(CompletionPart.Attributes);
                }
            }
            return _lazyCustomAttributesBag;
        }

        internal override void EarlyDecodeWellKnownAttributeType(NamedTypeSymbol attributeType, AttributeSyntax attributeSyntax)
        {
            if (CSharpAttributeData.IsTargetEarlyAttribute(attributeType, attributeSyntax, AttributeDescription.OptionalAttribute))
            {
                _lazyHasOptionalAttribute = ThreeState.True;
            }
        }

        internal override void PostEarlyDecodeWellKnownAttributeTypes()
        {
            if (_lazyHasOptionalAttribute == ThreeState.Unknown)
            {
                _lazyHasOptionalAttribute = ThreeState.False;
            }
            base.PostEarlyDecodeWellKnownAttributeTypes();
        }

        internal override CSharpAttributeData EarlyDecodeWellKnownAttribute(ref EarlyDecodeWellKnownAttributeArguments<EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation> arguments)
        {
            if (CSharpAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.DefaultParameterValueAttribute))
            {
                return EarlyDecodeAttributeForDefaultParameterValue(AttributeDescription.DefaultParameterValueAttribute, ref arguments);
            }
            if (CSharpAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.DecimalConstantAttribute))
            {
                return EarlyDecodeAttributeForDefaultParameterValue(AttributeDescription.DecimalConstantAttribute, ref arguments);
            }
            if (CSharpAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.DateTimeConstantAttribute))
            {
                return EarlyDecodeAttributeForDefaultParameterValue(AttributeDescription.DateTimeConstantAttribute, ref arguments);
            }
            if (!IsOnPartialImplementation(arguments.AttributeSyntax))
            {
                if (CSharpAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.CallerLineNumberAttribute))
                {
                    arguments.GetOrCreateData<ParameterEarlyWellKnownAttributeData>().HasCallerLineNumberAttribute = true;
                }
                else if (CSharpAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.CallerFilePathAttribute))
                {
                    arguments.GetOrCreateData<ParameterEarlyWellKnownAttributeData>().HasCallerFilePathAttribute = true;
                }
                else if (CSharpAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.CallerMemberNameAttribute))
                {
                    arguments.GetOrCreateData<ParameterEarlyWellKnownAttributeData>().HasCallerMemberNameAttribute = true;
                }
            }
            return base.EarlyDecodeWellKnownAttribute(ref arguments);
        }

        private CSharpAttributeData EarlyDecodeAttributeForDefaultParameterValue(AttributeDescription description, ref EarlyDecodeWellKnownAttributeArguments<EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation> arguments)
        {
            CSharpAttributeData attribute = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, out bool generatedDiagnostics);
            ConstantValue defaultParameterValue;
            if (attribute.HasErrors)
            {
                defaultParameterValue = ConstantValue.Bad;
                generatedDiagnostics = true;
            }
            else
            {
                defaultParameterValue = DecodeDefaultParameterValueAttribute(description, attribute, arguments.AttributeSyntax, diagnose: false, null);
            }
            ParameterEarlyWellKnownAttributeData orCreateData = arguments.GetOrCreateData<ParameterEarlyWellKnownAttributeData>();
            if (orCreateData.DefaultParameterValue == ConstantValue.Unset)
            {
                orCreateData.DefaultParameterValue = defaultParameterValue;
            }
            if (generatedDiagnostics)
            {
                return null;
            }
            return attribute;
        }

        internal override void DecodeWellKnownAttribute(ref DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments)
        {
            CSharpAttributeData attribute = arguments.Attribute;
            BindingDiagnosticBag bindingDiagnosticBag = (BindingDiagnosticBag)arguments.Diagnostics;
            if (attribute.IsTargetAttribute(this, AttributeDescription.DefaultParameterValueAttribute))
            {
                DecodeDefaultParameterValueAttribute(AttributeDescription.DefaultParameterValueAttribute, ref arguments);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.DecimalConstantAttribute))
            {
                DecodeDefaultParameterValueAttribute(AttributeDescription.DecimalConstantAttribute, ref arguments);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.DateTimeConstantAttribute))
            {
                DecodeDefaultParameterValueAttribute(AttributeDescription.DateTimeConstantAttribute, ref arguments);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.OptionalAttribute))
            {
                if (HasDefaultArgumentSyntax)
                {
                    bindingDiagnosticBag.Add(ErrorCode.ERR_DefaultValueUsedWithAttributes, arguments.AttributeSyntaxOpt!.Name.Location);
                }
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.ParamArrayAttribute))
            {
                bindingDiagnosticBag.Add(ErrorCode.ERR_ExplicitParamArray, arguments.AttributeSyntaxOpt!.Name.Location);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.InAttribute))
            {
                arguments.GetOrCreateData<ParameterWellKnownAttributeData>().HasInAttribute = true;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.OutAttribute))
            {
                arguments.GetOrCreateData<ParameterWellKnownAttributeData>().HasOutAttribute = true;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.MarshalAsAttribute))
            {
                MarshalAsAttributeDecoder<ParameterWellKnownAttributeData, AttributeSyntax, CSharpAttributeData, AttributeLocation>.Decode(ref arguments, AttributeTargets.Parameter, MessageProvider.Instance);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.IDispatchConstantAttribute))
            {
                arguments.GetOrCreateData<ParameterWellKnownAttributeData>().HasIDispatchConstantAttribute = true;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.IUnknownConstantAttribute))
            {
                arguments.GetOrCreateData<ParameterWellKnownAttributeData>().HasIUnknownConstantAttribute = true;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.CallerLineNumberAttribute))
            {
                ValidateCallerLineNumberAttribute(arguments.AttributeSyntaxOpt, bindingDiagnosticBag);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.CallerFilePathAttribute))
            {
                ValidateCallerFilePathAttribute(arguments.AttributeSyntaxOpt, bindingDiagnosticBag);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.CallerMemberNameAttribute))
            {
                ValidateCallerMemberNameAttribute(arguments.AttributeSyntaxOpt, bindingDiagnosticBag);
            }
            else if (!ReportExplicitUseOfReservedAttributes(in arguments, ReservedAttributes.DynamicAttribute | ReservedAttributes.IsReadOnlyAttribute | ReservedAttributes.IsUnmanagedAttribute | ReservedAttributes.IsByRefLikeAttribute | ReservedAttributes.TupleElementNamesAttribute | ReservedAttributes.NullableAttribute | ReservedAttributes.NativeIntegerAttribute))
            {
                if (attribute.IsTargetAttribute(this, AttributeDescription.AllowNullAttribute))
                {
                    arguments.GetOrCreateData<ParameterWellKnownAttributeData>().HasAllowNullAttribute = true;
                }
                else if (attribute.IsTargetAttribute(this, AttributeDescription.DisallowNullAttribute))
                {
                    arguments.GetOrCreateData<ParameterWellKnownAttributeData>().HasDisallowNullAttribute = true;
                }
                else if (attribute.IsTargetAttribute(this, AttributeDescription.MaybeNullAttribute))
                {
                    arguments.GetOrCreateData<ParameterWellKnownAttributeData>().HasMaybeNullAttribute = true;
                }
                else if (attribute.IsTargetAttribute(this, AttributeDescription.MaybeNullWhenAttribute))
                {
                    arguments.GetOrCreateData<ParameterWellKnownAttributeData>().MaybeNullWhenAttribute = DecodeMaybeNullWhenOrNotNullWhenOrDoesNotReturnIfAttribute(AttributeDescription.MaybeNullWhenAttribute, attribute);
                }
                else if (attribute.IsTargetAttribute(this, AttributeDescription.NotNullAttribute))
                {
                    arguments.GetOrCreateData<ParameterWellKnownAttributeData>().HasNotNullAttribute = true;
                }
                else if (attribute.IsTargetAttribute(this, AttributeDescription.NotNullWhenAttribute))
                {
                    arguments.GetOrCreateData<ParameterWellKnownAttributeData>().NotNullWhenAttribute = DecodeMaybeNullWhenOrNotNullWhenOrDoesNotReturnIfAttribute(AttributeDescription.NotNullWhenAttribute, attribute);
                }
                else if (attribute.IsTargetAttribute(this, AttributeDescription.DoesNotReturnIfAttribute))
                {
                    arguments.GetOrCreateData<ParameterWellKnownAttributeData>().DoesNotReturnIfAttribute = DecodeMaybeNullWhenOrNotNullWhenOrDoesNotReturnIfAttribute(AttributeDescription.DoesNotReturnIfAttribute, attribute);
                }
                else if (attribute.IsTargetAttribute(this, AttributeDescription.NotNullIfNotNullAttribute))
                {
                    arguments.GetOrCreateData<ParameterWellKnownAttributeData>().AddNotNullIfParameterNotNull(attribute.DecodeNotNullIfNotNullAttribute());
                }
                else if (attribute.IsTargetAttribute(this, AttributeDescription.EnumeratorCancellationAttribute))
                {
                    arguments.GetOrCreateData<ParameterWellKnownAttributeData>().HasEnumeratorCancellationAttribute = true;
                    ValidateCancellationTokenAttribute(arguments.AttributeSyntaxOpt, (BindingDiagnosticBag)arguments.Diagnostics);
                }
            }
        }

        private static bool? DecodeMaybeNullWhenOrNotNullWhenOrDoesNotReturnIfAttribute(AttributeDescription description, CSharpAttributeData attribute)
        {
            ImmutableArray<TypedConstant> commonConstructorArguments = attribute.CommonConstructorArguments;
            if (commonConstructorArguments.Length != 1 || !commonConstructorArguments[0].TryDecodeValue<bool>(SpecialType.System_Boolean, out var value))
            {
                return null;
            }
            return value;
        }

        private void DecodeDefaultParameterValueAttribute(AttributeDescription description, ref DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments)
        {
            CSharpAttributeData attribute = arguments.Attribute;
            AttributeSyntax attributeSyntaxOpt = arguments.AttributeSyntaxOpt;
            BindingDiagnosticBag bindingDiagnosticBag = (BindingDiagnosticBag)arguments.Diagnostics;
            ConstantValue constantValue = DecodeDefaultParameterValueAttribute(description, attribute, attributeSyntaxOpt, diagnose: true, bindingDiagnosticBag);
            if (!constantValue.IsBad)
            {
                VerifyParamDefaultValueMatchesAttributeIfAny(constantValue, attributeSyntaxOpt, bindingDiagnosticBag);
            }
        }

        private void VerifyParamDefaultValueMatchesAttributeIfAny(ConstantValue value, SyntaxNode syntax, BindingDiagnosticBag diagnostics)
        {
            ParameterEarlyWellKnownAttributeData earlyDecodedWellKnownAttributeData = GetEarlyDecodedWellKnownAttributeData();
            if (earlyDecodedWellKnownAttributeData != null)
            {
                ConstantValue defaultParameterValue = earlyDecodedWellKnownAttributeData.DefaultParameterValue;
                if (defaultParameterValue != ConstantValue.Unset && value != defaultParameterValue)
                {
                    diagnostics.Add(ErrorCode.ERR_ParamDefaultValueDiffersFromAttribute, syntax.Location);
                }
            }
        }

        private ConstantValue DecodeDefaultParameterValueAttribute(AttributeDescription description, CSharpAttributeData attribute, AttributeSyntax node, bool diagnose, BindingDiagnosticBag diagnosticsOpt)
        {
            if (description.Equals(AttributeDescription.DefaultParameterValueAttribute))
            {
                return DecodeDefaultParameterValueAttribute(attribute, node, diagnose, diagnosticsOpt);
            }
            if (description.Equals(AttributeDescription.DecimalConstantAttribute))
            {
                return attribute.DecodeDecimalConstantValue();
            }
            return attribute.DecodeDateTimeConstantValue();
        }

        private ConstantValue DecodeDefaultParameterValueAttribute(CSharpAttributeData attribute, AttributeSyntax node, bool diagnose, BindingDiagnosticBag diagnosticsOpt)
        {
            if (HasDefaultArgumentSyntax)
            {
                if (diagnose)
                {
                    diagnosticsOpt.Add(ErrorCode.ERR_DefaultValueUsedWithAttributes, node.Name.Location);
                }
                return ConstantValue.Bad;
            }
            TypedConstant typedConstant = attribute.CommonConstructorArguments[0];
            SpecialType st = ((typedConstant.Kind == TypedConstantKind.Enum) ? ((NamedTypeSymbol)typedConstant.TypeInternal).EnumUnderlyingType.SpecialType : typedConstant.TypeInternal!.SpecialType);
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            ConstantValueTypeDiscriminator constantValueTypeDiscriminator = ConstantValue.GetDiscriminator(st);
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagnosticsOpt, ContainingAssembly);
            if (constantValueTypeDiscriminator == ConstantValueTypeDiscriminator.Bad)
            {
                if (typedConstant.Kind == TypedConstantKind.Array || typedConstant.ValueInternal != null)
                {
                    if (diagnose)
                    {
                        diagnosticsOpt.Add(ErrorCode.ERR_DefaultValueBadValueType, node.Name.Location, typedConstant.TypeInternal);
                    }
                    return ConstantValue.Bad;
                }
                if (!base.Type.IsReferenceType)
                {
                    if (diagnose)
                    {
                        diagnosticsOpt.Add(ErrorCode.ERR_DefaultValueTypeMustMatch, node.Name.Location);
                    }
                    return ConstantValue.Bad;
                }
                constantValueTypeDiscriminator = ConstantValueTypeDiscriminator.Nothing;
            }
            else if (!declaringCompilation.Conversions.ClassifyConversionFromType((TypeSymbol)typedConstant.TypeInternal, base.Type, ref useSiteInfo).Kind.IsImplicitConversion())
            {
                if (diagnose)
                {
                    diagnosticsOpt.Add(ErrorCode.ERR_DefaultValueTypeMustMatch, node.Name.Location);
                    diagnosticsOpt.Add(node.Name.Location, useSiteInfo);
                }
                return ConstantValue.Bad;
            }
            if (diagnose)
            {
                diagnosticsOpt.Add(node.Name.Location, useSiteInfo);
            }
            return ConstantValue.Create(typedConstant.ValueInternal, constantValueTypeDiscriminator);
        }

        private bool IsValidCallerInfoContext(AttributeSyntax node)
        {
            if (!ContainingSymbol.IsExplicitInterfaceImplementation() && !ContainingSymbol.IsOperator())
            {
                return !IsOnPartialImplementation(node);
            }
            return false;
        }

        private bool IsOnPartialImplementation(AttributeSyntax node)
        {
            if (!(ContainingSymbol is MethodSymbol methodSymbol))
            {
                return false;
            }
            MethodSymbol methodSymbol2 = (methodSymbol.IsPartialImplementation() ? methodSymbol : methodSymbol.PartialImplementationPart);
            if ((object)methodSymbol2 == null)
            {
                return false;
            }
            if (!(node.Parent!.Parent!.Parent is ParameterListSyntax parameterListSyntax))
            {
                return false;
            }
            if (!(parameterListSyntax.Parent is MethodDeclarationSyntax methodDeclarationSyntax))
            {
                return false;
            }
            ImmutableArray<SyntaxReference>.Enumerator enumerator = methodSymbol2.DeclaringSyntaxReferences.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.GetSyntax() == methodDeclarationSyntax)
                {
                    return true;
                }
            }
            return false;
        }

        private void ValidateCallerLineNumberAttribute(AttributeSyntax node, BindingDiagnosticBag diagnostics)
        {
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, ContainingAssembly);
            if (!IsValidCallerInfoContext(node))
            {
                diagnostics.Add(ErrorCode.WRN_CallerLineNumberParamForUnconsumedLocation, node.Name.Location, CSharpSyntaxNode.Identifier.ValueText);
            }
            else if (!declaringCompilation.Conversions.HasCallerLineNumberConversion(TypeWithAnnotations.Type, ref useSiteInfo))
            {
                TypeSymbol specialType = declaringCompilation.GetSpecialType(SpecialType.System_Int32);
                diagnostics.Add(ErrorCode.ERR_NoConversionForCallerLineNumberParam, node.Name.Location, specialType, TypeWithAnnotations.Type);
            }
            else if (!base.HasExplicitDefaultValue && !ContainingSymbol.IsPartialImplementation())
            {
                diagnostics.Add(ErrorCode.ERR_BadCallerLineNumberParamWithoutDefaultValue, node.Name.Location);
            }
            diagnostics.Add(node.Name.Location, useSiteInfo);
        }

        private void ValidateCallerFilePathAttribute(AttributeSyntax node, BindingDiagnosticBag diagnostics)
        {
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, ContainingAssembly);
            if (!IsValidCallerInfoContext(node))
            {
                diagnostics.Add(ErrorCode.WRN_CallerFilePathParamForUnconsumedLocation, node.Name.Location, CSharpSyntaxNode.Identifier.ValueText);
            }
            else if (!declaringCompilation.Conversions.HasCallerInfoStringConversion(TypeWithAnnotations.Type, ref useSiteInfo))
            {
                TypeSymbol specialType = declaringCompilation.GetSpecialType(SpecialType.System_String);
                diagnostics.Add(ErrorCode.ERR_NoConversionForCallerFilePathParam, node.Name.Location, specialType, TypeWithAnnotations.Type);
            }
            else if (!base.HasExplicitDefaultValue && !ContainingSymbol.IsPartialImplementation())
            {
                diagnostics.Add(ErrorCode.ERR_BadCallerFilePathParamWithoutDefaultValue, node.Name.Location);
            }
            else if (HasCallerLineNumberAttribute)
            {
                diagnostics.Add(ErrorCode.WRN_CallerLineNumberPreferredOverCallerFilePath, node.Name.Location, CSharpSyntaxNode.Identifier.ValueText);
            }
            diagnostics.Add(node.Name.Location, useSiteInfo);
        }

        private void ValidateCallerMemberNameAttribute(AttributeSyntax node, BindingDiagnosticBag diagnostics)
        {
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, ContainingAssembly);
            if (!IsValidCallerInfoContext(node))
            {
                diagnostics.Add(ErrorCode.WRN_CallerMemberNameParamForUnconsumedLocation, node.Name.Location, CSharpSyntaxNode.Identifier.ValueText);
            }
            else if (!declaringCompilation.Conversions.HasCallerInfoStringConversion(TypeWithAnnotations.Type, ref useSiteInfo))
            {
                TypeSymbol specialType = declaringCompilation.GetSpecialType(SpecialType.System_String);
                diagnostics.Add(ErrorCode.ERR_NoConversionForCallerMemberNameParam, node.Name.Location, specialType, TypeWithAnnotations.Type);
            }
            else if (!base.HasExplicitDefaultValue && !ContainingSymbol.IsPartialImplementation())
            {
                diagnostics.Add(ErrorCode.ERR_BadCallerMemberNameParamWithoutDefaultValue, node.Name.Location);
            }
            else if (HasCallerLineNumberAttribute)
            {
                diagnostics.Add(ErrorCode.WRN_CallerLineNumberPreferredOverCallerMemberName, node.Name.Location, CSharpSyntaxNode.Identifier.ValueText);
            }
            else if (HasCallerFilePathAttribute)
            {
                diagnostics.Add(ErrorCode.WRN_CallerFilePathPreferredOverCallerMemberName, node.Name.Location, CSharpSyntaxNode.Identifier.ValueText);
            }
            diagnostics.Add(node.Name.Location, useSiteInfo);
        }

        private void ValidateCancellationTokenAttribute(AttributeSyntax node, BindingDiagnosticBag diagnostics)
        {
            if (needsReporting())
            {
                diagnostics.Add(ErrorCode.WRN_UnconsumedEnumeratorCancellationAttributeUsage, node.Name.Location, CSharpSyntaxNode.Identifier.ValueText);
            }
            bool needsReporting()
            {
                if (!base.Type.Equals(DeclaringCompilation.GetWellKnownType(WellKnownType.System_Threading_CancellationToken)))
                {
                    return true;
                }
                if (ContainingSymbol is MethodSymbol methodSymbol && methodSymbol.IsAsync && methodSymbol.ReturnType.OriginalDefinition.Equals(DeclaringCompilation.GetWellKnownType(WellKnownType.System_Collections_Generic_IAsyncEnumerable_T)))
                {
                    return false;
                }
                return true;
            }
        }

        internal override void PostDecodeWellKnownAttributes(ImmutableArray<CSharpAttributeData> boundAttributes, ImmutableArray<AttributeSyntax> allAttributeSyntaxNodes, BindingDiagnosticBag diagnostics, AttributeLocation symbolPart, WellKnownAttributeData decodedData)
        {
            ParameterWellKnownAttributeData parameterWellKnownAttributeData = (ParameterWellKnownAttributeData)decodedData;
            if (parameterWellKnownAttributeData != null)
            {
                switch (RefKind)
                {
                    case RefKind.Ref:
                        if (parameterWellKnownAttributeData.HasOutAttribute && !parameterWellKnownAttributeData.HasInAttribute)
                        {
                            diagnostics.Add(ErrorCode.ERR_OutAttrOnRefParam, Locations[0]);
                        }
                        break;
                    case RefKind.Out:
                        if (parameterWellKnownAttributeData.HasInAttribute)
                        {
                            diagnostics.Add(ErrorCode.ERR_InAttrOnOutParam, Locations[0]);
                        }
                        break;
                    case RefKind.In:
                        if (parameterWellKnownAttributeData.HasOutAttribute)
                        {
                            diagnostics.Add(ErrorCode.ERR_OutAttrOnInParam, Locations[0]);
                        }
                        break;
                }
            }
            base.PostDecodeWellKnownAttributes(boundAttributes, allAttributeSyntaxNodes, diagnostics, symbolPart, decodedData);
        }

        internal override void ForceComplete(SourceLocation locationOpt, CancellationToken cancellationToken)
        {
            GetAttributes();
            _ = ExplicitDefaultConstantValue;
            state.SpinWaitComplete(CompletionPart.ComplexParameterSymbolAll, cancellationToken);
        }
    }
}
