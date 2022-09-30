using System.Collections.Immutable;
using System.Globalization;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal abstract class SourceEventSymbol : EventSymbol, IAttributeTargetSymbol
    {
        private readonly Location _location;

        private readonly SyntaxReference _syntaxRef;

        private readonly DeclarationModifiers _modifiers;

        internal readonly SourceMemberContainerTypeSymbol containingType;

        private SymbolCompletionState _state;

        private CustomAttributesBag<CSharpAttributeData>? _lazyCustomAttributesBag;

        private string? _lazyDocComment;

        private string? _lazyExpandedDocComment;

        private OverriddenOrHiddenMembersResult? _lazyOverriddenOrHiddenMembers;

        private ThreeState _lazyIsWindowsRuntimeEvent;

        internal sealed override bool RequiresCompletion => true;

        public abstract override string Name { get; }

        public abstract override MethodSymbol? AddMethod { get; }

        public abstract override MethodSymbol? RemoveMethod { get; }

        public abstract override ImmutableArray<EventSymbol> ExplicitInterfaceImplementations { get; }

        public abstract override TypeWithAnnotations TypeWithAnnotations { get; }

        public sealed override Symbol ContainingSymbol => containingType;

        public override NamedTypeSymbol ContainingType => containingType;

        public sealed override ImmutableArray<Location> Locations => ImmutableArray.Create(_location);

        public sealed override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray.Create(_syntaxRef);

        internal SyntaxList<AttributeListSyntax> AttributeDeclarationSyntaxList
        {
            get
            {
                if (containingType.AnyMemberHasAttributes)
                {
                    CSharpSyntaxNode cSharpSyntaxNode = CSharpSyntaxNode;
                    if (cSharpSyntaxNode != null)
                    {
                        return cSharpSyntaxNode.Kind() switch
                        {
                            SyntaxKind.EventDeclaration => ((EventDeclarationSyntax)cSharpSyntaxNode).AttributeLists,
                            SyntaxKind.VariableDeclarator => ((EventFieldDeclarationSyntax)cSharpSyntaxNode.Parent!.Parent).AttributeLists,
                            _ => throw ExceptionUtilities.UnexpectedValue(cSharpSyntaxNode.Kind()),
                        };
                    }
                }
                return default(SyntaxList<AttributeListSyntax>);
            }
        }

        IAttributeTargetSymbol IAttributeTargetSymbol.AttributesOwner => this;

        AttributeLocation IAttributeTargetSymbol.DefaultAttributeLocation => AttributeLocation.Event;

        AttributeLocation IAttributeTargetSymbol.AllowedAttributeLocations => AllowedAttributeLocations;

        protected abstract AttributeLocation AllowedAttributeLocations { get; }

        internal override ObsoleteAttributeData? ObsoleteAttributeData
        {
            get
            {
                if (!containingType.AnyMemberHasAttributes)
                {
                    return null;
                }
                CustomAttributesBag<CSharpAttributeData> lazyCustomAttributesBag = _lazyCustomAttributesBag;
                if (lazyCustomAttributesBag != null && lazyCustomAttributesBag.IsEarlyDecodedWellKnownAttributeDataComputed)
                {
                    return ((CommonEventEarlyWellKnownAttributeData)lazyCustomAttributesBag.EarlyDecodedWellKnownAttributeData)?.ObsoleteAttributeData;
                }
                return Microsoft.CodeAnalysis.ObsoleteAttributeData.Uninitialized;
            }
        }

        internal sealed override bool IsDirectlyExcludedFromCodeCoverage => GetDecodedWellKnownAttributeData()?.HasExcludeFromCodeCoverageAttribute ?? false;

        internal sealed override bool HasSpecialName => GetDecodedWellKnownAttributeData()?.HasSpecialNameAttribute ?? false;

        public bool HasSkipLocalsInitAttribute => GetDecodedWellKnownAttributeData()?.HasSkipLocalsInitAttribute ?? false;

        public sealed override bool IsAbstract => (_modifiers & DeclarationModifiers.Abstract) != 0;

        public sealed override bool IsExtern => (_modifiers & DeclarationModifiers.Extern) != 0;

        public sealed override bool IsStatic => (_modifiers & DeclarationModifiers.Static) != 0;

        public sealed override bool IsOverride => (_modifiers & DeclarationModifiers.Override) != 0;

        public sealed override bool IsSealed => (_modifiers & DeclarationModifiers.Sealed) != 0;

        public sealed override bool IsVirtual => (_modifiers & DeclarationModifiers.Virtual) != 0;

        internal bool IsReadOnly => (_modifiers & DeclarationModifiers.ReadOnly) != 0;

        public sealed override Accessibility DeclaredAccessibility => ModifierUtils.EffectiveAccessibility(_modifiers);

        internal sealed override bool MustCallMethodsDirectly => false;

        internal SyntaxReference SyntaxReference => _syntaxRef;

        internal CSharpSyntaxNode CSharpSyntaxNode => (CSharpSyntaxNode)_syntaxRef.GetSyntax();

        internal SyntaxTree SyntaxTree => _syntaxRef.SyntaxTree;

        internal bool IsNew => (_modifiers & DeclarationModifiers.New) != 0;

        internal DeclarationModifiers Modifiers => _modifiers;

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

        public sealed override bool IsWindowsRuntimeEvent
        {
            get
            {
                if (!_lazyIsWindowsRuntimeEvent.HasValue())
                {
                    _lazyIsWindowsRuntimeEvent = ComputeIsWindowsRuntimeEvent().ToThreeState();
                }
                return _lazyIsWindowsRuntimeEvent.Value();
            }
        }

        internal SourceEventSymbol(SourceMemberContainerTypeSymbol containingType, CSharpSyntaxNode syntax, SyntaxTokenList modifiers, bool isFieldLike, ExplicitInterfaceSpecifierSyntax? interfaceSpecifierSyntaxOpt, SyntaxToken nameTokenSyntax, BindingDiagnosticBag diagnostics)
        {
            _location = nameTokenSyntax.GetLocation();
            this.containingType = containingType;
            _syntaxRef = syntax.GetReference();
            bool flag = interfaceSpecifierSyntaxOpt != null;
            _modifiers = MakeModifiers(modifiers, flag, isFieldLike, _location, diagnostics, out var _);
            CheckAccessibility(_location, diagnostics, flag);
        }

        internal sealed override bool HasComplete(CompletionPart part)
        {
            return _state.HasComplete(part);
        }

        internal override void ForceComplete(SourceLocation? locationOpt, CancellationToken cancellationToken)
        {
            _state.DefaultForceComplete(this, cancellationToken);
        }

        internal override LexicalSortKey GetLexicalSortKey()
        {
            return new LexicalSortKey(_location, DeclaringCompilation);
        }

        private CustomAttributesBag<CSharpAttributeData> GetAttributesBag()
        {
            if ((_lazyCustomAttributesBag == null || !_lazyCustomAttributesBag!.IsSealed) && LoadAndValidateAttributes(OneOrMany.Create(AttributeDeclarationSyntaxList), ref _lazyCustomAttributesBag))
            {
                DeclaringCompilation.SymbolDeclaredEvent(this);
                _state.NotePartComplete(CompletionPart.Attributes);
            }
            return _lazyCustomAttributesBag;
        }

        public sealed override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            return GetAttributesBag().Attributes;
        }

        protected CommonEventWellKnownAttributeData GetDecodedWellKnownAttributeData()
        {
            CustomAttributesBag<CSharpAttributeData> customAttributesBag = _lazyCustomAttributesBag;
            if (customAttributesBag == null || !customAttributesBag.IsDecodedWellKnownAttributeDataComputed)
            {
                customAttributesBag = GetAttributesBag();
            }
            return (CommonEventWellKnownAttributeData)customAttributesBag.DecodedWellKnownAttributeData;
        }

        internal CommonEventEarlyWellKnownAttributeData GetEarlyDecodedWellKnownAttributeData()
        {
            CustomAttributesBag<CSharpAttributeData> customAttributesBag = _lazyCustomAttributesBag;
            if (customAttributesBag == null || !customAttributesBag.IsEarlyDecodedWellKnownAttributeDataComputed)
            {
                customAttributesBag = GetAttributesBag();
            }
            return (CommonEventEarlyWellKnownAttributeData)customAttributesBag.EarlyDecodedWellKnownAttributeData;
        }

        internal override CSharpAttributeData? EarlyDecodeWellKnownAttribute(ref EarlyDecodeWellKnownAttributeArguments<EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation> arguments)
        {
            if (Symbol.EarlyDecodeDeprecatedOrExperimentalOrObsoleteAttribute(ref arguments, out var attributeData, out var obsoleteData))
            {
                if (obsoleteData != null)
                {
                    arguments.GetOrCreateData<CommonEventEarlyWellKnownAttributeData>().ObsoleteAttributeData = obsoleteData;
                }
                return attributeData;
            }
            return base.EarlyDecodeWellKnownAttribute(ref arguments);
        }

        internal sealed override void DecodeWellKnownAttribute(ref DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments)
        {
            CSharpAttributeData attribute = arguments.Attribute;
            _ = (BindingDiagnosticBag)arguments.Diagnostics;
            if (attribute.IsTargetAttribute(this, AttributeDescription.SpecialNameAttribute))
            {
                arguments.GetOrCreateData<CommonEventWellKnownAttributeData>().HasSpecialNameAttribute = true;
            }
            else if (!ReportExplicitUseOfReservedAttributes(in arguments, ReservedAttributes.TupleElementNamesAttribute | ReservedAttributes.NullableAttribute | ReservedAttributes.NativeIntegerAttribute))
            {
                if (attribute.IsTargetAttribute(this, AttributeDescription.ExcludeFromCodeCoverageAttribute))
                {
                    arguments.GetOrCreateData<CommonEventWellKnownAttributeData>().HasExcludeFromCodeCoverageAttribute = true;
                }
                else if (attribute.IsTargetAttribute(this, AttributeDescription.SkipLocalsInitAttribute))
                {
                    CSharpAttributeData.DecodeSkipLocalsInitAttribute<CommonEventWellKnownAttributeData>(DeclaringCompilation, ref arguments);
                }
            }
        }

        internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData>? attributes)
        {
            base.AddSynthesizedAttributes(moduleBuilder, ref attributes);
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            TypeWithAnnotations typeWithAnnotations = TypeWithAnnotations;
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
                Symbol.AddSynthesizedAttribute(ref attributes, DeclaringCompilation.SynthesizeTupleNamesAttribute(typeWithAnnotations.Type));
            }
            if (declaringCompilation.ShouldEmitNullableAttributes(this))
            {
                Symbol.AddSynthesizedAttribute(ref attributes, moduleBuilder.SynthesizeNullableAttributeIfNecessary(this, containingType.GetNullableContextValue(), typeWithAnnotations));
            }
        }

        private void CheckAccessibility(Location location, BindingDiagnosticBag diagnostics, bool isExplicitInterfaceImplementation)
        {
            CSDiagnosticInfo cSDiagnosticInfo = ModifierUtils.CheckAccessibility(_modifiers, this, isExplicitInterfaceImplementation);
            if (cSDiagnosticInfo != null)
            {
                diagnostics.Add(new CSDiagnostic(cSDiagnosticInfo, location));
            }
        }

        private DeclarationModifiers MakeModifiers(SyntaxTokenList modifiers, bool explicitInterfaceImplementation, bool isFieldLike, Location location, BindingDiagnosticBag diagnostics, out bool modifierErrors)
        {
            bool isInterface = ContainingType.IsInterface;
            DeclarationModifiers defaultAccess = ((isInterface && !explicitInterfaceImplementation) ? DeclarationModifiers.Public : DeclarationModifiers.Private);
            DeclarationModifiers declarationModifiers = DeclarationModifiers.None;
            DeclarationModifiers declarationModifiers2 = DeclarationModifiers.Unsafe;
            if (!explicitInterfaceImplementation)
            {
                declarationModifiers2 |= DeclarationModifiers.AccessibilityMask | DeclarationModifiers.Abstract | DeclarationModifiers.Sealed | DeclarationModifiers.Static | DeclarationModifiers.New | DeclarationModifiers.Virtual;
                if (!isInterface)
                {
                    declarationModifiers2 |= DeclarationModifiers.Override;
                }
                else
                {
                    defaultAccess = DeclarationModifiers.None;
                    declarationModifiers2 |= DeclarationModifiers.Extern;
                    declarationModifiers |= DeclarationModifiers.AccessibilityMask | DeclarationModifiers.Abstract | DeclarationModifiers.Sealed | DeclarationModifiers.Static | DeclarationModifiers.Extern | DeclarationModifiers.Virtual;
                }
            }
            else if (isInterface)
            {
                declarationModifiers2 |= DeclarationModifiers.Abstract;
            }
            if (ContainingType.IsStructType())
            {
                declarationModifiers2 |= DeclarationModifiers.ReadOnly;
            }
            if (!isInterface)
            {
                declarationModifiers2 |= DeclarationModifiers.Extern;
            }
            DeclarationModifiers declarationModifiers3 = ModifierUtils.MakeAndCheckNontypeMemberModifiers(modifiers, defaultAccess, declarationModifiers2, location, diagnostics, out modifierErrors);
            this.CheckUnsafeModifier(declarationModifiers3, diagnostics);
            ModifierUtils.ReportDefaultInterfaceImplementationModifiers(!isFieldLike, declarationModifiers3, declarationModifiers, location, diagnostics);
            if (isInterface)
            {
                declarationModifiers3 = ModifierUtils.AdjustModifiersForAnInterfaceMember(declarationModifiers3, !isFieldLike, explicitInterfaceImplementation);
            }
            return declarationModifiers3;
        }

        protected void CheckModifiersAndType(BindingDiagnosticBag diagnostics)
        {
            Location location = Locations[0];
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, ContainingAssembly);
            bool flag = ContainingType.IsInterface && IsExplicitInterfaceImplementation;
            if (DeclaredAccessibility == Accessibility.Private && (IsVirtual || (IsAbstract && !flag) || IsOverride))
            {
                diagnostics.Add(ErrorCode.ERR_VirtualPrivate, location, this);
            }
            else if (IsStatic && (IsOverride || IsVirtual || IsAbstract))
            {
                diagnostics.Add(ErrorCode.ERR_StaticNotVirtual, location, this);
            }
            else if (IsReadOnly && IsStatic)
            {
                diagnostics.Add(ErrorCode.ERR_StaticMemberCantBeReadOnly, location, this);
            }
            else if (IsReadOnly && base.HasAssociatedField)
            {
                diagnostics.Add(ErrorCode.ERR_FieldLikeEventCantBeReadOnly, location, this);
            }
            else if (IsOverride && (IsNew || IsVirtual))
            {
                diagnostics.Add(ErrorCode.ERR_OverrideNotNew, location, this);
            }
            else if (IsSealed && !IsOverride && (!flag || !IsAbstract))
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
                diagnostics.Add(ErrorCode.ERR_InstanceMemberInStaticClass, location, Name);
            }
            else if (!base.Type.IsVoidType())
            {
                if (!this.IsNoMoreVisibleThan(base.Type, ref useSiteInfo) && (CSharpSyntaxNode as EventDeclarationSyntax)?.ExplicitInterfaceSpecifier == null)
                {
                    diagnostics.Add(ErrorCode.ERR_BadVisEventType, location, this, base.Type);
                }
                else if (!base.Type.IsDelegateType() && !base.Type.IsErrorType())
                {
                    diagnostics.Add(ErrorCode.ERR_EventNotDelegate, location, this);
                }
                else if (IsAbstract && !ContainingType.IsAbstract && (ContainingType.TypeKind == TypeKind.Class || ContainingType.TypeKind == TypeKind.Submission))
                {
                    diagnostics.Add(ErrorCode.ERR_AbstractInConcreteClass, location, this, ContainingType);
                }
                else if (IsVirtual && ContainingType.IsSealed)
                {
                    diagnostics.Add(ErrorCode.ERR_NewVirtualInSealed, location, this, ContainingType);
                }
            }
            diagnostics.Add(location, useSiteInfo);
        }

        public override string GetDocumentationCommentXml(CultureInfo? preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            return SourceDocumentationCommentUtils.GetAndCacheDocumentationComment(this, expandIncludes, ref expandIncludes ? ref _lazyExpandedDocComment : ref _lazyDocComment);
        }

        protected static void CopyEventCustomModifiers(EventSymbol eventWithCustomModifiers, ref TypeWithAnnotations type, AssemblySymbol containingAssembly)
        {
            TypeSymbol type2 = eventWithCustomModifiers.Type;
            if (type.Type.Equals(type2, TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds | TypeCompareKind.IgnoreDynamic | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes))
            {
                type = type.WithTypeAndModifiers(CustomModifierUtils.CopyTypeCustomModifiers(type2, type.Type, containingAssembly), eventWithCustomModifiers.TypeWithAnnotations.CustomModifiers);
            }
        }

        private bool ComputeIsWindowsRuntimeEvent()
        {
            ImmutableArray<EventSymbol> explicitInterfaceImplementations = ExplicitInterfaceImplementations;
            if (!explicitInterfaceImplementations.IsEmpty)
            {
                return explicitInterfaceImplementations[0].IsWindowsRuntimeEvent;
            }
            if (containingType.IsInterfaceType())
            {
                return this.IsCompilationOutputWinMdObj();
            }
            EventSymbol overriddenEvent = base.OverriddenEvent;
            if ((object)overriddenEvent != null)
            {
                return overriddenEvent.IsWindowsRuntimeEvent;
            }
            bool flag = false;
            foreach (NamedTypeSymbol key in containingType.InterfacesAndTheirBaseInterfacesNoUseSiteDiagnostics.Keys)
            {
                ImmutableArray<Symbol>.Enumerator enumerator2 = key.GetMembers(Name).GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    Symbol current = enumerator2.Current;
                    if (current.Kind == SymbolKind.Event && current.IsImplementableInterfaceMember() && this == containingType.FindImplementationForInterfaceMemberInNonInterface(current, ignoreImplementationInInterfacesIfResultIsNotReady: true))
                    {
                        flag = true;
                        if (((EventSymbol)current).IsWindowsRuntimeEvent)
                        {
                            return true;
                        }
                    }
                }
            }
            if (flag)
            {
                return false;
            }
            return this.IsCompilationOutputWinMdObj();
        }

        internal static string GetAccessorName(string eventName, bool isAdder)
        {
            return (isAdder ? "add_" : "remove_") + eventName;
        }

        protected TypeWithAnnotations BindEventType(Binder binder, TypeSyntax typeSyntax, BindingDiagnosticBag diagnostics)
        {
            binder = binder.WithAdditionalFlagsAndContainingMemberOrLambda(BinderFlags.SuppressConstraintChecks | BinderFlags.SuppressUnsafeDiagnostics, this);
            return binder.BindType(typeSyntax, diagnostics);
        }

        internal override void AfterAddingTypeMembersChecks(ConversionsBase conversions, BindingDiagnosticBag diagnostics)
        {
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            Location location = Locations[0];
            CheckModifiersAndType(diagnostics);
            base.Type.CheckAllConstraints(declaringCompilation, conversions, location, diagnostics);
            if (base.Type.ContainsNativeInteger())
            {
                declaringCompilation.EnsureNativeIntegerAttributeExists(diagnostics, location, modifyCompilation: true);
            }
            if (declaringCompilation.ShouldEmitNullableAttributes(this) && TypeWithAnnotations.NeedsNullableAttribute())
            {
                declaringCompilation.EnsureNullableAttributeExists(diagnostics, location, modifyCompilation: true);
            }
        }
    }
}
