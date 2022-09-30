using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public abstract class SourcePropertySymbolBase : PropertySymbol, IAttributeTargetSymbol
    {
        [Flags()]
        private enum Flags : byte
        {
            IsExpressionBodied = 1,
            IsAutoProperty = 2,
            IsExplicitInterfaceImplementation = 4,
            HasInitializer = 8
        }

        protected const string DefaultIndexerName = "Item";

        private readonly SourceMemberContainerTypeSymbol _containingType;

        private readonly string _name;

        private readonly SyntaxReference _syntaxRef;

        protected readonly DeclarationModifiers _modifiers;

        private ImmutableArray<CustomModifier> _lazyRefCustomModifiers;

        private readonly SourcePropertyAccessorSymbol? _getMethod;

        private readonly SourcePropertyAccessorSymbol? _setMethod;

        private readonly TypeSymbol _explicitInterfaceType;

        private ImmutableArray<PropertySymbol> _lazyExplicitInterfaceImplementations;

        private readonly Flags _propertyFlags;

        private readonly RefKind _refKind;

        private SymbolCompletionState _state;

        private ImmutableArray<ParameterSymbol> _lazyParameters;

        private TypeWithAnnotations.Boxed _lazyType;

        private string _lazySourceName;

        private string _lazyDocComment;

        private string _lazyExpandedDocComment;

        private OverriddenOrHiddenMembersResult _lazyOverriddenOrHiddenMembers;

        private SynthesizedSealedPropertyAccessor _lazySynthesizedSealedAccessor;

        private CustomAttributesBag<CSharpAttributeData> _lazyCustomAttributesBag;

        public Location Location { get; }

        protected abstract Location TypeLocation { get; }

        internal sealed override ImmutableArray<string> NotNullMembers => GetDecodedWellKnownAttributeData()?.NotNullMembers ?? ImmutableArray<string>.Empty;

        internal sealed override ImmutableArray<string> NotNullWhenTrueMembers => GetDecodedWellKnownAttributeData()?.NotNullWhenTrueMembers ?? ImmutableArray<string>.Empty;

        internal sealed override ImmutableArray<string> NotNullWhenFalseMembers => GetDecodedWellKnownAttributeData()?.NotNullWhenFalseMembers ?? ImmutableArray<string>.Empty;

        internal bool IsExpressionBodied => (_propertyFlags & Flags.IsExpressionBodied) != 0;

        public sealed override RefKind RefKind => _refKind;

        public sealed override TypeWithAnnotations TypeWithAnnotations
        {
            get
            {
                EnsureSignature();
                return _lazyType.Value;
            }
        }

        internal bool HasPointerType
        {
            get
            {
                if (_lazyType != null)
                {
                    return _lazyType.Value.DefaultType.IsPointerOrFunctionPointer();
                }
                return HasPointerTypeSyntactically;
            }
        }

        protected abstract bool HasPointerTypeSyntactically { get; }

        public override string Name => _name;

        internal string SourceName
        {
            get
            {
                if (_lazySourceName == null)
                {
                    SyntaxList<AttributeListSyntax> attributeLists = ((IndexerDeclarationSyntax)CSharpSyntaxNode).AttributeLists;
                    string text = null;
                    CustomAttributesBag<CSharpAttributeData> lazyCustomAttributesBag = null;
                    LoadAndValidateAttributes(OneOrMany.Create(attributeLists), ref lazyCustomAttributesBag, AttributeLocation.None, earlyDecodingOnly: true);
                    if (lazyCustomAttributesBag != null)
                    {
                        PropertyEarlyWellKnownAttributeData propertyEarlyWellKnownAttributeData = (PropertyEarlyWellKnownAttributeData)lazyCustomAttributesBag.EarlyDecodedWellKnownAttributeData;
                        if (propertyEarlyWellKnownAttributeData != null)
                        {
                            text = propertyEarlyWellKnownAttributeData.IndexerName;
                        }
                    }
                    text = text ?? "Item";
                    InterlockedOperations.Initialize(ref _lazySourceName, text);
                }
                return _lazySourceName;
            }
        }

        public override string MetadataName => SourceName.Replace(" ", "");

        public override Symbol ContainingSymbol => _containingType;

        public override NamedTypeSymbol ContainingType => _containingType;

        public override ImmutableArray<Location> Locations => ImmutableArray.Create(Location);

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray.Create(_syntaxRef);

        public override bool IsAbstract => (_modifiers & DeclarationModifiers.Abstract) != 0;

        public override bool IsExtern => (_modifiers & DeclarationModifiers.Extern) != 0;

        public override bool IsStatic => (_modifiers & DeclarationModifiers.Static) != 0;

        internal bool IsFixed => false;

        public override bool IsIndexer => (_modifiers & DeclarationModifiers.Indexer) != 0;

        public override bool IsOverride => (_modifiers & DeclarationModifiers.Override) != 0;

        public override bool IsSealed => (_modifiers & DeclarationModifiers.Sealed) != 0;

        public override bool IsVirtual => (_modifiers & DeclarationModifiers.Virtual) != 0;

        internal bool IsNew => (_modifiers & DeclarationModifiers.New) != 0;

        internal bool HasReadOnlyModifier => (_modifiers & DeclarationModifiers.ReadOnly) != 0;

        public sealed override MethodSymbol? GetMethod => _getMethod;

        public sealed override MethodSymbol? SetMethod => _setMethod;

        internal override CallingConvention CallingConvention
        {
            get
            {
                if (!IsStatic)
                {
                    return CallingConvention.HasThis;
                }
                return CallingConvention.Default;
            }
        }

        public sealed override ImmutableArray<ParameterSymbol> Parameters
        {
            get
            {
                EnsureSignature();
                return _lazyParameters;
            }
        }

        internal override bool IsExplicitInterfaceImplementation => (_propertyFlags & Flags.IsExplicitInterfaceImplementation) != 0;

        public sealed override ImmutableArray<PropertySymbol> ExplicitInterfaceImplementations
        {
            get
            {
                if (IsExplicitInterfaceImplementation)
                {
                    EnsureSignature();
                }
                return _lazyExplicitInterfaceImplementations;
            }
        }

        public sealed override ImmutableArray<CustomModifier> RefCustomModifiers
        {
            get
            {
                EnsureSignature();
                return _lazyRefCustomModifiers;
            }
        }

        public override Accessibility DeclaredAccessibility => ModifierUtils.EffectiveAccessibility(_modifiers);

        public bool HasSkipLocalsInitAttribute => GetDecodedWellKnownAttributeData()?.HasSkipLocalsInitAttribute ?? false;

        internal bool IsAutoPropertyWithGetAccessor
        {
            get
            {
                if (IsAutoProperty)
                {
                    return (object)_getMethod != null;
                }
                return false;
            }
        }

        protected bool IsAutoProperty => (_propertyFlags & Flags.IsAutoProperty) != 0;

        internal SynthesizedBackingFieldSymbol BackingField { get; }

        internal override bool MustCallMethodsDirectly => false;

        internal SyntaxReference SyntaxReference => _syntaxRef;

        internal CSharpSyntaxNode CSharpSyntaxNode => (CSharpSyntaxNode)_syntaxRef.GetSyntax();

        internal SyntaxTree SyntaxTree => _syntaxRef.SyntaxTree;

        internal override OverriddenOrHiddenMembersResult OverriddenOrHiddenMembers
        {
            get
            {
                if (_lazyOverriddenOrHiddenMembers == null)
                {
                    Interlocked.CompareExchange(ref _lazyOverriddenOrHiddenMembers, this.MakeOverriddenOrHiddenMembers(), null);
                }
                return _lazyOverriddenOrHiddenMembers;
            }
        }

        internal SynthesizedSealedPropertyAccessor SynthesizedSealedAccessorOpt
        {
            get
            {
                bool flag = (object)GetMethod != null;
                bool flag2 = (object)SetMethod != null;
                if (!IsSealed || (flag && flag2))
                {
                    return null;
                }
                if ((object)_lazySynthesizedSealedAccessor == null)
                {
                    Interlocked.CompareExchange(ref _lazySynthesizedSealedAccessor, MakeSynthesizedSealedAccessor(), null);
                }
                return _lazySynthesizedSealedAccessor;
            }
        }

        public abstract SyntaxList<AttributeListSyntax> AttributeDeclarationSyntaxList { get; }

        public abstract IAttributeTargetSymbol AttributesOwner { get; }

        IAttributeTargetSymbol IAttributeTargetSymbol.AttributesOwner => AttributesOwner;

        AttributeLocation IAttributeTargetSymbol.DefaultAttributeLocation => AttributeLocation.Property;

        AttributeLocation IAttributeTargetSymbol.AllowedAttributeLocations
        {
            get
            {
                if (!IsAutoPropertyWithGetAccessor)
                {
                    return AttributeLocation.Property;
                }
                return AttributeLocation.Field | AttributeLocation.Property;
            }
        }

        internal sealed override bool IsDirectlyExcludedFromCodeCoverage => GetDecodedWellKnownAttributeData()?.HasExcludeFromCodeCoverageAttribute ?? false;

        internal override bool HasSpecialName => GetDecodedWellKnownAttributeData()?.HasSpecialNameAttribute ?? false;

        internal override ObsoleteAttributeData ObsoleteAttributeData
        {
            get
            {
                if (!_containingType.AnyMemberHasAttributes)
                {
                    return null;
                }
                CustomAttributesBag<CSharpAttributeData> lazyCustomAttributesBag = _lazyCustomAttributesBag;
                if (lazyCustomAttributesBag != null && lazyCustomAttributesBag.IsEarlyDecodedWellKnownAttributeDataComputed)
                {
                    return ((PropertyEarlyWellKnownAttributeData)lazyCustomAttributesBag.EarlyDecodedWellKnownAttributeData)?.ObsoleteAttributeData;
                }
                return ObsoleteAttributeData.Uninitialized;
            }
        }

        internal bool HasDisallowNull => GetDecodedWellKnownAttributeData()?.HasDisallowNullAttribute ?? false;

        internal bool HasAllowNull => GetDecodedWellKnownAttributeData()?.HasAllowNullAttribute ?? false;

        internal bool HasMaybeNull => GetDecodedWellKnownAttributeData()?.HasMaybeNullAttribute ?? false;

        internal bool HasNotNull => GetDecodedWellKnownAttributeData()?.HasNotNullAttribute ?? false;

        internal SourceAttributeData DisallowNullAttributeIfExists => FindAttribute(AttributeDescription.DisallowNullAttribute);

        internal SourceAttributeData AllowNullAttributeIfExists => FindAttribute(AttributeDescription.AllowNullAttribute);

        internal SourceAttributeData MaybeNullAttributeIfExists => FindAttribute(AttributeDescription.MaybeNullAttribute);

        internal SourceAttributeData NotNullAttributeIfExists => FindAttribute(AttributeDescription.NotNullAttribute);

        internal ImmutableArray<SourceAttributeData> MemberNotNullAttributeIfExists => FindAttributes(AttributeDescription.MemberNotNullAttribute);

        internal ImmutableArray<SourceAttributeData> MemberNotNullWhenAttributeIfExists => FindAttributes(AttributeDescription.MemberNotNullWhenAttribute);

        internal sealed override bool RequiresCompletion => true;

        public SourcePropertySymbolBase(SourceMemberContainerTypeSymbol containingType, CSharpSyntaxNode syntax, bool hasGetAccessor, bool hasSetAccessor, bool isExplicitInterfaceImplementation, TypeSymbol? explicitInterfaceType, string? aliasQualifierOpt, DeclarationModifiers modifiers, bool hasInitializer, bool isAutoProperty, bool isExpressionBodied, bool isInitOnly, RefKind refKind, string memberName, SyntaxList<AttributeListSyntax> indexerNameAttributeLists, Location location, BindingDiagnosticBag diagnostics)
        {
            _syntaxRef = syntax.GetReference();
            Location = location;
            _containingType = containingType;
            _refKind = refKind;
            _modifiers = modifiers;
            _explicitInterfaceType = explicitInterfaceType;
            if (isExplicitInterfaceImplementation)
            {
                _propertyFlags |= Flags.IsExplicitInterfaceImplementation;
            }
            else
            {
                _lazyExplicitInterfaceImplementations = ImmutableArray<PropertySymbol>.Empty;
            }
            bool isIndexer = IsIndexer;
            isAutoProperty = isAutoProperty && (!containingType.IsInterface || IsStatic) && !IsAbstract && !IsExtern && !isIndexer;
            if (isAutoProperty)
            {
                _propertyFlags |= Flags.IsAutoProperty;
            }
            if (hasInitializer)
            {
                _propertyFlags |= Flags.HasInitializer;
            }
            if (isExpressionBodied)
            {
                _propertyFlags |= Flags.IsExpressionBodied;
            }
            if (isIndexer)
            {
                if (indexerNameAttributeLists.Count == 0 || isExplicitInterfaceImplementation)
                {
                    _lazySourceName = memberName;
                }
                _name = ExplicitInterfaceHelpers.GetMemberName("this[]", _explicitInterfaceType, aliasQualifierOpt);
            }
            else
            {
                _name = (_lazySourceName = memberName);
            }
            if ((isAutoProperty && hasGetAccessor) || hasInitializer)
            {
                string name = GeneratedNames.MakeBackingFieldName(_name);
                BackingField = new SynthesizedBackingFieldSymbol(this, name, (hasGetAccessor && !hasSetAccessor) || isInitOnly, IsStatic, hasInitializer);
            }
            if (hasGetAccessor)
            {
                _getMethod = CreateGetAccessorSymbol(isAutoProperty, diagnostics);
            }
            if (hasSetAccessor)
            {
                _setMethod = CreateSetAccessorSymbol(isAutoProperty, diagnostics);
            }
        }

        private void EnsureSignatureGuarded(BindingDiagnosticBag diagnostics)
        {
            PropertySymbol propertySymbol = null;
            _lazyRefCustomModifiers = ImmutableArray<CustomModifier>.Empty;
            (TypeWithAnnotations, ImmutableArray<ParameterSymbol>) tuple = MakeParametersAndBindType(diagnostics);
            TypeWithAnnotations item = tuple.Item1;
            _lazyParameters = tuple.Item2;
            _lazyType = new TypeWithAnnotations.Boxed(item);
            bool isExplicitInterfaceImplementation = IsExplicitInterfaceImplementation;
            if (isExplicitInterfaceImplementation || IsOverride)
            {
                bool alsoCopyParamsModifier = false;
                PropertySymbol propertySymbol2;
                if (!isExplicitInterfaceImplementation)
                {
                    alsoCopyParamsModifier = true;
                    propertySymbol2 = base.OverriddenProperty;
                }
                else
                {
                    CSharpSyntaxNode cSharpSyntaxNode = CSharpSyntaxNode;
                    string interfacePropertyName = (IsIndexer ? "this[]" : ((PropertyDeclarationSyntax)cSharpSyntaxNode).Identifier.ValueText);
                    propertySymbol = this.FindExplicitlyImplementedProperty(_explicitInterfaceType, interfacePropertyName, GetExplicitInterfaceSpecifier(), diagnostics);
                    this.FindExplicitlyImplementedMemberVerification(propertySymbol, diagnostics);
                    propertySymbol2 = propertySymbol;
                }
                if ((object)propertySymbol2 != null)
                {
                    _lazyRefCustomModifiers = ((_refKind != 0) ? propertySymbol2.RefCustomModifiers : ImmutableArray<CustomModifier>.Empty);
                    TypeWithAnnotations typeWithAnnotations = propertySymbol2.TypeWithAnnotations;
                    if (item.Type.Equals(typeWithAnnotations.Type, TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds | TypeCompareKind.IgnoreDynamic | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes))
                    {
                        item = item.WithTypeAndModifiers(CustomModifierUtils.CopyTypeCustomModifiers(typeWithAnnotations.Type, item.Type, ContainingAssembly), typeWithAnnotations.CustomModifiers);
                        _lazyType = new TypeWithAnnotations.Boxed(item);
                    }
                    _lazyParameters = CustomModifierUtils.CopyParameterCustomModifiers(propertySymbol2.Parameters, _lazyParameters, alsoCopyParamsModifier);
                }
            }
            else if (_refKind == RefKind.In)
            {
                NamedTypeSymbol wellKnownType = Binder.GetWellKnownType(DeclaringCompilation, WellKnownType.System_Runtime_InteropServices_InAttribute, diagnostics, TypeLocation);
                _lazyRefCustomModifiers = ImmutableArray.Create(CSharpCustomModifier.CreateRequired(wellKnownType));
            }
            _lazyExplicitInterfaceImplementations = (((object)propertySymbol == null) ? ImmutableArray<PropertySymbol>.Empty : ImmutableArray.Create(propertySymbol));
        }

        private void CheckInitializer(bool isAutoProperty, bool isInterface, bool isStatic, Location location, BindingDiagnosticBag diagnostics)
        {
            if (isInterface && !isStatic)
            {
                diagnostics.Add(ErrorCode.ERR_InstancePropertyInitializerInInterface, location, this);
            }
            else if (!isAutoProperty)
            {
                diagnostics.Add(ErrorCode.ERR_InitializerOnNonAutoProperty, location, this);
            }
        }

        private void EnsureSignature()
        {
            if (_state.HasComplete(CompletionPart.FinishBaseType))
            {
                return;
            }
            lock (_syntaxRef)
            {
                if (_state.NotePartComplete(CompletionPart.StartBaseType))
                {
                    BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
                    try
                    {
                        EnsureSignatureGuarded(instance);
                        AddDeclarationDiagnostics(instance);
                        return;
                    }
                    finally
                    {
                        _state.NotePartComplete(CompletionPart.FinishBaseType);
                        instance.Free();
                    }
                }
            }
        }

        internal override LexicalSortKey GetLexicalSortKey()
        {
            return new LexicalSortKey(Location, DeclaringCompilation);
        }

        public abstract SourcePropertyAccessorSymbol CreateGetAccessorSymbol(bool isAutoPropertyAccessor, BindingDiagnosticBag diagnostics);

        public abstract SourcePropertyAccessorSymbol CreateSetAccessorSymbol(bool isAutoPropertyAccessor, BindingDiagnosticBag diagnostics);

        internal override void AfterAddingTypeMembersChecks(ConversionsBase conversions, BindingDiagnosticBag diagnostics)
        {
            bool isExplicitInterfaceImplementation = IsExplicitInterfaceImplementation;
            CheckAccessibility(Location, diagnostics, isExplicitInterfaceImplementation);
            CheckModifiers(isExplicitInterfaceImplementation, Location, IsIndexer, diagnostics);
            if ((_propertyFlags & Flags.HasInitializer) != 0)
            {
                CheckInitializer(IsAutoProperty, ContainingType.IsInterface, IsStatic, Location, diagnostics);
            }
            if (IsAutoPropertyWithGetAccessor)
            {
                if (!IsStatic)
                {
                    MethodSymbol setMethod = SetMethod;
                    if ((object)setMethod != null && !setMethod.IsInitOnly)
                    {
                        if (ContainingType.IsReadOnly)
                        {
                            diagnostics.Add(ErrorCode.ERR_AutoPropsInRoStruct, Location);
                        }
                        else if (HasReadOnlyModifier)
                        {
                            diagnostics.Add(ErrorCode.ERR_AutoPropertyWithSetterCantBeReadOnly, Location, this);
                        }
                    }
                }
                Binder.ReportUseSiteDiagnosticForSynthesizedAttribute(DeclaringCompilation, WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor, diagnostics, Location);
                if (RefKind != 0)
                {
                    diagnostics.Add(ErrorCode.ERR_AutoPropertyCannotBeRefReturning, Location, this);
                }
                if ((object)SetMethod == null && !base.IsReadOnly)
                {
                    diagnostics.Add(ErrorCode.ERR_AutoPropertyMustOverrideSet, Location, this);
                }
            }
            if (!IsExpressionBodied)
            {
                bool flag = (object)GetMethod != null;
                bool flag2 = (object)SetMethod != null;
                if (flag && flag2)
                {
                    if (_refKind != 0)
                    {
                        diagnostics.Add(ErrorCode.ERR_RefPropertyCannotHaveSetAccessor, _setMethod!.Locations[0], _setMethod);
                    }
                    else if (_getMethod!.LocalAccessibility != 0 && _setMethod!.LocalAccessibility != 0)
                    {
                        diagnostics.Add(ErrorCode.ERR_DuplicatePropertyAccessMods, Location, this);
                    }
                    else if (_getMethod!.LocalDeclaredReadOnly && _setMethod!.LocalDeclaredReadOnly)
                    {
                        diagnostics.Add(ErrorCode.ERR_DuplicatePropertyReadOnlyMods, Location, this);
                    }
                    else if (IsAbstract)
                    {
                        CheckAbstractPropertyAccessorNotPrivate(_getMethod, diagnostics);
                        CheckAbstractPropertyAccessorNotPrivate(_setMethod, diagnostics);
                    }
                }
                else
                {
                    if (!flag && !flag2)
                    {
                        diagnostics.Add(ErrorCode.ERR_PropertyWithNoAccessors, Location, this);
                    }
                    else if (RefKind != 0)
                    {
                        if (!flag)
                        {
                            diagnostics.Add(ErrorCode.ERR_RefPropertyMustHaveGetAccessor, Location, this);
                        }
                    }
                    else if (!flag && IsAutoProperty)
                    {
                        diagnostics.Add(ErrorCode.ERR_AutoPropertyMustHaveGetAccessor, _setMethod!.Locations[0], _setMethod);
                    }
                    if (!IsOverride)
                    {
                        SourcePropertyAccessorSymbol sourcePropertyAccessorSymbol = _getMethod ?? _setMethod;
                        if ((object)sourcePropertyAccessorSymbol != null)
                        {
                            if (sourcePropertyAccessorSymbol.LocalAccessibility != 0)
                            {
                                diagnostics.Add(ErrorCode.ERR_AccessModMissingAccessor, Location, this);
                            }
                            if (sourcePropertyAccessorSymbol.LocalDeclaredReadOnly)
                            {
                                diagnostics.Add(ErrorCode.ERR_ReadOnlyModMissingAccessor, Location, this);
                            }
                        }
                    }
                }
                CheckAccessibilityMoreRestrictive(_getMethod, diagnostics);
                CheckAccessibilityMoreRestrictive(_setMethod, diagnostics);
            }
            PropertySymbol propertySymbol = ExplicitInterfaceImplementations.FirstOrDefault();
            if ((object)propertySymbol != null)
            {
                CheckExplicitImplementationAccessor(GetMethod, propertySymbol.GetMethod, propertySymbol, diagnostics);
                CheckExplicitImplementationAccessor(SetMethod, propertySymbol.SetMethod, propertySymbol, diagnostics);
            }
            Location typeLocation = TypeLocation;
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            if ((object)_explicitInterfaceType != null)
            {
                ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier = GetExplicitInterfaceSpecifier();
                _explicitInterfaceType.CheckAllConstraints(declaringCompilation, conversions, new SourceLocation(explicitInterfaceSpecifier.Name), diagnostics);
                if ((object)propertySymbol != null)
                {
                    TypeSymbol.CheckNullableReferenceTypeMismatchOnImplementingMember(ContainingType, this, propertySymbol, isExplicit: true, diagnostics);
                }
            }
            if (_refKind == RefKind.In)
            {
                declaringCompilation.EnsureIsReadOnlyAttributeExists(diagnostics, typeLocation, modifyCompilation: true);
            }
            ParameterHelpers.EnsureIsReadOnlyAttributeExists(declaringCompilation, Parameters, diagnostics, modifyCompilation: true);
            if (base.Type.ContainsNativeInteger())
            {
                declaringCompilation.EnsureNativeIntegerAttributeExists(diagnostics, typeLocation, modifyCompilation: true);
            }
            ParameterHelpers.EnsureNativeIntegerAttributeExists(declaringCompilation, Parameters, diagnostics, modifyCompilation: true);
            if (declaringCompilation.ShouldEmitNullableAttributes(this) && TypeWithAnnotations.NeedsNullableAttribute())
            {
                declaringCompilation.EnsureNullableAttributeExists(diagnostics, typeLocation, modifyCompilation: true);
            }
            ParameterHelpers.EnsureNullableAttributeExists(declaringCompilation, this, Parameters, diagnostics, modifyCompilation: true);
        }

        private void CheckAccessibility(Location location, BindingDiagnosticBag diagnostics, bool isExplicitInterfaceImplementation)
        {
            CSDiagnosticInfo cSDiagnosticInfo = ModifierUtils.CheckAccessibility(_modifiers, this, isExplicitInterfaceImplementation);
            if (cSDiagnosticInfo != null)
            {
                diagnostics.Add(new CSDiagnostic(cSDiagnosticInfo, location));
            }
        }

        private void CheckModifiers(bool isExplicitInterfaceImplementation, Location location, bool isIndexer, BindingDiagnosticBag diagnostics)
        {
            bool flag = isExplicitInterfaceImplementation && ContainingType.IsInterface;
            if (DeclaredAccessibility == Accessibility.Private && (IsVirtual || (IsAbstract && !flag) || IsOverride))
            {
                diagnostics.Add(ErrorCode.ERR_VirtualPrivate, location, this);
            }
            else if (IsStatic && (IsOverride || IsVirtual || IsAbstract))
            {
                diagnostics.Add(ErrorCode.ERR_StaticNotVirtual, location, this);
            }
            else if (IsStatic && HasReadOnlyModifier)
            {
                diagnostics.Add(ErrorCode.ERR_StaticMemberCantBeReadOnly, location, this);
            }
            else if (IsOverride && (IsNew || IsVirtual))
            {
                diagnostics.Add(ErrorCode.ERR_OverrideNotNew, location, this);
            }
            else if (IsSealed && !IsOverride && !(IsAbstract && flag))
            {
                diagnostics.Add(ErrorCode.ERR_SealedNonOverride, location, this);
            }
            else if (IsAbstract && ContainingType.TypeKind == TypeKind.Struct)
            {
                diagnostics.Add(ErrorCode.ERR_BadMemberFlag, location, SyntaxFacts.GetText(SyntaxKind.AbstractKeyword));
            }
            else if (IsVirtual && ContainingType.TypeKind == TypeKind.Struct)
            {
                diagnostics.Add(ErrorCode.ERR_BadMemberFlag, location, SyntaxFacts.GetText(SyntaxKind.VirtualKeyword));
            }
            else if (IsAbstract && IsExtern)
            {
                diagnostics.Add(ErrorCode.ERR_AbstractAndExtern, location, this);
            }
            else if (IsAbstract && IsSealed && !flag)
            {
                diagnostics.Add(ErrorCode.ERR_AbstractAndSealed, location, this);
            }
            else if (IsAbstract && IsVirtual)
            {
                diagnostics.Add(ErrorCode.ERR_AbstractNotVirtual, location, Kind.Localize(), this);
            }
            else if (ContainingType.IsSealed && DeclaredAccessibility.HasProtected() && !IsOverride)
            {
                diagnostics.Add(AccessCheck.GetProtectedMemberInSealedTypeError(ContainingType), location, this);
            }
            else if (ContainingType.IsStatic && !IsStatic)
            {
                ErrorCode code = (isIndexer ? ErrorCode.ERR_IndexerInStaticClass : ErrorCode.ERR_InstanceMemberInStaticClass);
                diagnostics.Add(code, location, this);
            }
        }

        private void CheckAccessibilityMoreRestrictive(SourcePropertyAccessorSymbol accessor, BindingDiagnosticBag diagnostics)
        {
            if ((object)accessor != null && !IsAccessibilityMoreRestrictive(DeclaredAccessibility, accessor.LocalAccessibility))
            {
                diagnostics.Add(ErrorCode.ERR_InvalidPropertyAccessMod, accessor.Locations[0], accessor, this);
            }
        }

        private static bool IsAccessibilityMoreRestrictive(Accessibility property, Accessibility accessor)
        {
            if (accessor == Accessibility.NotApplicable)
            {
                return true;
            }
            if (accessor < property)
            {
                if (accessor == Accessibility.Protected)
                {
                    return property != Accessibility.Internal;
                }
                return true;
            }
            return false;
        }

        private static void CheckAbstractPropertyAccessorNotPrivate(SourcePropertyAccessorSymbol accessor, BindingDiagnosticBag diagnostics)
        {
            if (accessor.LocalAccessibility == Accessibility.Private)
            {
                diagnostics.Add(ErrorCode.ERR_PrivateAbstractAccessor, accessor.Locations[0], accessor);
            }
        }

        public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            return SourceDocumentationCommentUtils.GetAndCacheDocumentationComment(this, expandIncludes, ref expandIncludes ? ref _lazyExpandedDocComment : ref _lazyDocComment);
        }

        private void CheckExplicitImplementationAccessor(MethodSymbol thisAccessor, MethodSymbol otherAccessor, PropertySymbol explicitlyImplementedProperty, BindingDiagnosticBag diagnostics)
        {
            bool flag = (object)thisAccessor != null;
            bool flag2 = otherAccessor.IsImplementable();
            if (flag2 && !flag)
            {
                diagnostics.Add(ErrorCode.ERR_ExplicitPropertyMissingAccessor, Location, this, otherAccessor);
            }
            else if (!flag2 && flag)
            {
                diagnostics.Add(ErrorCode.ERR_ExplicitPropertyAddingAccessor, thisAccessor.Locations[0], thisAccessor, explicitlyImplementedProperty);
            }
            else if (TypeSymbol.HaveInitOnlyMismatch(thisAccessor, otherAccessor))
            {
                diagnostics.Add(ErrorCode.ERR_ExplicitPropertyMismatchInitOnly, thisAccessor.Locations[0], thisAccessor, otherAccessor);
            }
        }

        private SynthesizedSealedPropertyAccessor MakeSynthesizedSealedAccessor()
        {
            if ((object)GetMethod != null)
            {
                MethodSymbol ownOrInheritedSetMethod = this.GetOwnOrInheritedSetMethod();
                if ((object)ownOrInheritedSetMethod != null)
                {
                    return new SynthesizedSealedPropertyAccessor(this, ownOrInheritedSetMethod);
                }
                return null;
            }
            if ((object)SetMethod != null)
            {
                MethodSymbol ownOrInheritedGetMethod = this.GetOwnOrInheritedGetMethod();
                if ((object)ownOrInheritedGetMethod != null)
                {
                    return new SynthesizedSealedPropertyAccessor(this, ownOrInheritedGetMethod);
                }
                return null;
            }
            return null;
        }

        private CustomAttributesBag<CSharpAttributeData> GetAttributesBag()
        {
            CustomAttributesBag<CSharpAttributeData> lazyCustomAttributesBag = _lazyCustomAttributesBag;
            if (lazyCustomAttributesBag != null && lazyCustomAttributesBag.IsSealed)
            {
                return lazyCustomAttributesBag;
            }
            BackingField?.GetAttributes();
            if (LoadAndValidateAttributes(OneOrMany.Create(AttributeDeclarationSyntaxList), ref _lazyCustomAttributesBag))
            {
                _state.NotePartComplete(CompletionPart.Attributes);
            }
            return _lazyCustomAttributesBag;
        }

        public sealed override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            return GetAttributesBag().Attributes;
        }

        private PropertyWellKnownAttributeData GetDecodedWellKnownAttributeData()
        {
            CustomAttributesBag<CSharpAttributeData> customAttributesBag = _lazyCustomAttributesBag;
            if (customAttributesBag == null || !customAttributesBag.IsDecodedWellKnownAttributeDataComputed)
            {
                customAttributesBag = GetAttributesBag();
            }
            return (PropertyWellKnownAttributeData)customAttributesBag.DecodedWellKnownAttributeData;
        }

        internal PropertyEarlyWellKnownAttributeData GetEarlyDecodedWellKnownAttributeData()
        {
            CustomAttributesBag<CSharpAttributeData> customAttributesBag = _lazyCustomAttributesBag;
            if (customAttributesBag == null || !customAttributesBag.IsEarlyDecodedWellKnownAttributeDataComputed)
            {
                customAttributesBag = GetAttributesBag();
            }
            return (PropertyEarlyWellKnownAttributeData)customAttributesBag.EarlyDecodedWellKnownAttributeData;
        }

        internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
            base.AddSynthesizedAttributes(moduleBuilder, ref attributes);
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            TypeWithAnnotations typeWithAnnotations = TypeWithAnnotations;
            if (typeWithAnnotations.Type.ContainsDynamic())
            {
                Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.SynthesizeDynamicAttribute(typeWithAnnotations.Type, typeWithAnnotations.CustomModifiers.Length + RefCustomModifiers.Length, _refKind));
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
            if (base.ReturnsByRefReadonly)
            {
                Symbol.AddSynthesizedAttribute(ref attributes, moduleBuilder.SynthesizeIsReadOnlyAttribute(this));
            }
        }

        internal override CSharpAttributeData EarlyDecodeWellKnownAttribute(ref EarlyDecodeWellKnownAttributeArguments<EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation> arguments)
        {
            if (Symbol.EarlyDecodeDeprecatedOrExperimentalOrObsoleteAttribute(ref arguments, out var attributeData, out var obsoleteData))
            {
                if (obsoleteData != null)
                {
                    arguments.GetOrCreateData<PropertyEarlyWellKnownAttributeData>().ObsoleteAttributeData = obsoleteData;
                }
                return attributeData;
            }
            if (CSharpAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.IndexerNameAttribute))
            {
                attributeData = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, out var generatedDiagnostics);
                if (!attributeData.HasErrors)
                {
                    string text = attributeData.CommonConstructorArguments[0].DecodeValue<string>(SpecialType.System_String);
                    if (text != null)
                    {
                        arguments.GetOrCreateData<PropertyEarlyWellKnownAttributeData>().IndexerName = text;
                    }
                    if (!generatedDiagnostics)
                    {
                        return attributeData;
                    }
                }
                return null;
            }
            return base.EarlyDecodeWellKnownAttribute(ref arguments);
        }

        internal override void DecodeWellKnownAttribute(ref DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments)
        {
            BindingDiagnosticBag bindingDiagnosticBag = (BindingDiagnosticBag)arguments.Diagnostics;
            CSharpAttributeData attribute = arguments.Attribute;
            if (attribute.IsTargetAttribute(this, AttributeDescription.IndexerNameAttribute))
            {
                ValidateIndexerNameAttribute(attribute, arguments.AttributeSyntaxOpt, bindingDiagnosticBag);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.SpecialNameAttribute))
            {
                arguments.GetOrCreateData<PropertyWellKnownAttributeData>().HasSpecialNameAttribute = true;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.ExcludeFromCodeCoverageAttribute))
            {
                arguments.GetOrCreateData<PropertyWellKnownAttributeData>().HasExcludeFromCodeCoverageAttribute = true;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.SkipLocalsInitAttribute))
            {
                CSharpAttributeData.DecodeSkipLocalsInitAttribute<PropertyWellKnownAttributeData>(DeclaringCompilation, ref arguments);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.DynamicAttribute))
            {
                bindingDiagnosticBag.Add(ErrorCode.ERR_ExplicitDynamicAttr, arguments.AttributeSyntaxOpt!.Location);
            }
            else if (!ReportExplicitUseOfReservedAttributes(in arguments, ReservedAttributes.DynamicAttribute | ReservedAttributes.IsReadOnlyAttribute | ReservedAttributes.IsUnmanagedAttribute | ReservedAttributes.IsByRefLikeAttribute | ReservedAttributes.TupleElementNamesAttribute | ReservedAttributes.NullableAttribute | ReservedAttributes.NativeIntegerAttribute))
            {
                if (attribute.IsTargetAttribute(this, AttributeDescription.DisallowNullAttribute))
                {
                    arguments.GetOrCreateData<PropertyWellKnownAttributeData>().HasDisallowNullAttribute = true;
                }
                else if (attribute.IsTargetAttribute(this, AttributeDescription.AllowNullAttribute))
                {
                    arguments.GetOrCreateData<PropertyWellKnownAttributeData>().HasAllowNullAttribute = true;
                }
                else if (attribute.IsTargetAttribute(this, AttributeDescription.MaybeNullAttribute))
                {
                    arguments.GetOrCreateData<PropertyWellKnownAttributeData>().HasMaybeNullAttribute = true;
                }
                else if (attribute.IsTargetAttribute(this, AttributeDescription.NotNullAttribute))
                {
                    arguments.GetOrCreateData<PropertyWellKnownAttributeData>().HasNotNullAttribute = true;
                }
                else if (attribute.IsTargetAttribute(this, AttributeDescription.MemberNotNullAttribute))
                {
                    MessageID.IDS_FeatureMemberNotNull.CheckFeatureAvailability(bindingDiagnosticBag, arguments.AttributeSyntaxOpt);
                    CSharpAttributeData.DecodeMemberNotNullAttribute<PropertyWellKnownAttributeData>(ContainingType, ref arguments);
                }
                else if (attribute.IsTargetAttribute(this, AttributeDescription.MemberNotNullWhenAttribute))
                {
                    MessageID.IDS_FeatureMemberNotNull.CheckFeatureAvailability(bindingDiagnosticBag, arguments.AttributeSyntaxOpt);
                    CSharpAttributeData.DecodeMemberNotNullWhenAttribute<PropertyWellKnownAttributeData>(ContainingType, ref arguments);
                }
            }
        }

        private SourceAttributeData FindAttribute(AttributeDescription attributeDescription)
        {
            return (SourceAttributeData)GetAttributes().First((CSharpAttributeData a) => a.IsTargetAttribute(this, attributeDescription));
        }

        private ImmutableArray<SourceAttributeData> FindAttributes(AttributeDescription attributeDescription)
        {
            return (from a in GetAttributes()
                    where a.IsTargetAttribute(this, attributeDescription)
                    select a).Cast<SourceAttributeData>().ToImmutableArray();
        }

        internal override void PostDecodeWellKnownAttributes(ImmutableArray<CSharpAttributeData> boundAttributes, ImmutableArray<AttributeSyntax> allAttributeSyntaxNodes, BindingDiagnosticBag diagnostics, AttributeLocation symbolPart, WellKnownAttributeData decodedData)
        {
            base.PostDecodeWellKnownAttributes(boundAttributes, allAttributeSyntaxNodes, diagnostics, symbolPart, decodedData);
        }

        private void ValidateIndexerNameAttribute(CSharpAttributeData attribute, AttributeSyntax node, BindingDiagnosticBag diagnostics)
        {
            if (!IsIndexer || IsExplicitInterfaceImplementation)
            {
                diagnostics.Add(ErrorCode.ERR_BadIndexerNameAttr, node.Name.Location, node.GetErrorDisplayName());
                return;
            }
            string text = attribute.CommonConstructorArguments[0].DecodeValue<string>(SpecialType.System_String);
            if (text == null || !SyntaxFacts.IsValidIdentifier(text))
            {
                diagnostics.Add(ErrorCode.ERR_BadArgumentToAttribute, node.ArgumentList!.Arguments[0].Location, node.GetErrorDisplayName());
            }
        }

        internal sealed override bool HasComplete(CompletionPart part)
        {
            return _state.HasComplete(part);
        }

        internal override void ForceComplete(SourceLocation locationOpt, CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                CompletionPart nextIncompletePart = _state.NextIncompletePart;
                switch (nextIncompletePart)
                {
                    case CompletionPart.Attributes:
                        GetAttributes();
                        break;
                    case CompletionPart.StartBaseType:
                    case CompletionPart.FinishBaseType:
                        EnsureSignature();
                        break;
                    case CompletionPart.StartInterfaces:
                    case CompletionPart.FinishInterfaces:
                        if (_state.NotePartComplete(CompletionPart.StartInterfaces))
                        {
                            if (Parameters.Length > 0)
                            {
                                BindingDiagnosticBag instance2 = BindingDiagnosticBag.GetInstance();
                                TypeConversions conversions2 = new TypeConversions(ContainingAssembly.CorLibrary);
                                ImmutableArray<ParameterSymbol>.Enumerator enumerator = Parameters.GetEnumerator();
                                while (enumerator.MoveNext())
                                {
                                    ParameterSymbol current = enumerator.Current;
                                    current.ForceComplete(locationOpt, cancellationToken);
                                    current.Type.CheckAllConstraints(DeclaringCompilation, conversions2, current.Locations[0], instance2);
                                }
                                AddDeclarationDiagnostics(instance2);
                                instance2.Free();
                            }
                            DeclaringCompilation.SymbolDeclaredEvent(this);
                            _state.NotePartComplete(CompletionPart.FinishInterfaces);
                        }
                        else
                        {
                            _state.SpinWaitComplete(CompletionPart.FinishInterfaces, cancellationToken);
                        }
                        break;
                    case CompletionPart.EnumUnderlyingType:
                    case CompletionPart.TypeArguments:
                        if (_state.NotePartComplete(CompletionPart.EnumUnderlyingType))
                        {
                            BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
                            TypeConversions conversions = new TypeConversions(ContainingAssembly.CorLibrary);
                            base.Type.CheckAllConstraints(DeclaringCompilation, conversions, Location, instance);
                            ValidatePropertyType(instance);
                            AddDeclarationDiagnostics(instance);
                            _state.NotePartComplete(CompletionPart.TypeArguments);
                            instance.Free();
                        }
                        else
                        {
                            _state.SpinWaitComplete(CompletionPart.TypeArguments, cancellationToken);
                        }
                        break;
                    case CompletionPart.None:
                        return;
                    default:
                        _state.NotePartComplete(CompletionPart.NamespaceSymbolAll | CompletionPart.ReturnTypeAttributes | CompletionPart.Parameters | CompletionPart.Type | CompletionPart.TypeParameters | CompletionPart.TypeMembers | CompletionPart.SynthesizedExplicitImplementations | CompletionPart.StartMemberChecks | CompletionPart.FinishMemberChecks);
                        break;
                }
                _state.SpinWaitComplete(nextIncompletePart, cancellationToken);
            }
        }

        protected virtual void ValidatePropertyType(BindingDiagnosticBag diagnostics)
        {
            TypeSymbol type = base.Type;
            if (type.IsRestrictedType(ignoreSpanLikeTypes: true))
            {
                diagnostics.Add(ErrorCode.ERR_FieldCantBeRefAny, TypeLocation, type);
            }
            else if (IsAutoPropertyWithGetAccessor && type.IsRefLikeType && (IsStatic || !ContainingType.IsRefLikeType))
            {
                diagnostics.Add(ErrorCode.ERR_FieldAutoPropCantBeByRefLike, TypeLocation, type);
            }
        }

        protected abstract (TypeWithAnnotations Type, ImmutableArray<ParameterSymbol> Parameters) MakeParametersAndBindType(BindingDiagnosticBag diagnostics);

        protected static ExplicitInterfaceSpecifierSyntax? GetExplicitInterfaceSpecifier(SyntaxNode syntax)
        {
            return (syntax as BasePropertyDeclarationSyntax)?.ExplicitInterfaceSpecifier;
        }

        internal ExplicitInterfaceSpecifierSyntax? GetExplicitInterfaceSpecifier()
        {
            return GetExplicitInterfaceSpecifier(CSharpSyntaxNode);
        }
    }
}
