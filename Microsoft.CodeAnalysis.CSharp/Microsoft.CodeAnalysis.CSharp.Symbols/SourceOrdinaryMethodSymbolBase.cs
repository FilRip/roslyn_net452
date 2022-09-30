using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal abstract class SourceOrdinaryMethodSymbolBase : SourceMemberMethodSymbol
    {
        private readonly ImmutableArray<TypeParameterSymbol> _typeParameters;

        private readonly string _name;

        private ImmutableArray<MethodSymbol> _lazyExplicitInterfaceImplementations;

        private ImmutableArray<CustomModifier> _lazyRefCustomModifiers;

        private ImmutableArray<ParameterSymbol> _lazyParameters;

        private TypeWithAnnotations _lazyReturnType;

        private const DeclarationModifiers PartialMethodExtendedModifierMask = DeclarationModifiers.Sealed | DeclarationModifiers.New | DeclarationModifiers.Extern | DeclarationModifiers.Virtual | DeclarationModifiers.Override;

        public override bool ReturnsVoid
        {
            get
            {
                LazyMethodChecks();
                return base.ReturnsVoid;
            }
        }

        protected abstract Location ReturnTypeLocation { get; }

        protected abstract TypeSymbol ExplicitInterfaceType { get; }

        protected abstract bool HasAnyBody { get; }

        public override ImmutableArray<TypeParameterSymbol> TypeParameters => _typeParameters;

        public override ImmutableArray<Location> Locations => locations;

        internal sealed override int ParameterCount
        {
            get
            {
                if (!_lazyParameters.IsDefault)
                {
                    return _lazyParameters.Length;
                }
                return GetParameterCountFromSyntax();
            }
        }

        public sealed override ImmutableArray<ParameterSymbol> Parameters
        {
            get
            {
                LazyMethodChecks();
                return _lazyParameters;
            }
        }

        public sealed override TypeWithAnnotations ReturnTypeWithAnnotations
        {
            get
            {
                LazyMethodChecks();
                return _lazyReturnType;
            }
        }

        internal override bool IsExplicitInterfaceImplementation => MethodKind == MethodKind.ExplicitInterfaceImplementation;

        public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations
        {
            get
            {
                LazyMethodChecks();
                return _lazyExplicitInterfaceImplementations;
            }
        }

        public override ImmutableArray<CustomModifier> RefCustomModifiers
        {
            get
            {
                LazyMethodChecks();
                return _lazyRefCustomModifiers;
            }
        }

        public override string Name => _name;

        protected abstract override SourceMemberMethodSymbol BoundAttributesSource { get; }

        internal bool HasExplicitAccessModifier { get; }

        internal bool HasExtendedPartialModifier => (DeclarationModifiers & (DeclarationModifiers.Sealed | DeclarationModifiers.New | DeclarationModifiers.Extern | DeclarationModifiers.Virtual | DeclarationModifiers.Override)) != 0;

        protected SourceOrdinaryMethodSymbolBase(NamedTypeSymbol containingType, string name, Location location, CSharpSyntaxNode syntax, MethodKind methodKind, bool isIterator, bool isExtensionMethod, bool isPartial, bool hasBody, bool isNullableAnalysisEnabled, BindingDiagnosticBag diagnostics)
            : base(containingType, syntax.GetReference(), location, isIterator)
        {
            _name = name;
            (DeclarationModifiers, bool) tuple = MakeModifiers(methodKind, isPartial, hasBody, location, diagnostics);
            DeclarationModifiers item = tuple.Item1;
            HasExplicitAccessModifier = tuple.Item2;
            bool isMetadataVirtualIgnoringModifiers = methodKind == MethodKind.ExplicitInterfaceImplementation;
            MakeFlags(methodKind, item, returnsVoid: false, isExtensionMethod, isNullableAnalysisEnabled, isMetadataVirtualIgnoringModifiers);
            _typeParameters = MakeTypeParameters(syntax, diagnostics);
            CheckFeatureAvailabilityAndRuntimeSupport(syntax, location, hasBody, diagnostics);
            if (hasBody)
            {
                CheckModifiersForBody(location, diagnostics);
            }
            CSDiagnosticInfo cSDiagnosticInfo = ModifierUtils.CheckAccessibility(DeclarationModifiers, this, methodKind == MethodKind.ExplicitInterfaceImplementation);
            if (cSDiagnosticInfo != null)
            {
                diagnostics.Add(cSDiagnosticInfo, location);
            }
        }

        protected abstract ImmutableArray<TypeParameterSymbol> MakeTypeParameters(CSharpSyntaxNode node, BindingDiagnosticBag diagnostics);

        protected override void MethodChecks(BindingDiagnosticBag diagnostics)
        {
            bool isVararg;
            ImmutableArray<TypeParameterConstraintClause> immutableArray;
            (_lazyReturnType, _lazyParameters, isVararg, immutableArray) = MakeParametersAndBindReturnType(diagnostics);
            SetReturnsVoid(_lazyReturnType.IsVoidType());
            CheckEffectiveAccessibility(_lazyReturnType, _lazyParameters, diagnostics);
            Location location = locations[0];
            if (Name == "Finalize" && ParameterCount == 0 && Arity == 0 && ReturnsVoid)
            {
                diagnostics.Add(ErrorCode.WRN_FinalizeMethod, location);
            }
            ExtensionMethodChecks(diagnostics);
            if (base.IsPartial)
            {
                if (MethodKind == MethodKind.ExplicitInterfaceImplementation)
                {
                    diagnostics.Add(ErrorCode.ERR_PartialMethodNotExplicit, location);
                }
                if (!ContainingType.IsPartial())
                {
                    diagnostics.Add(ErrorCode.ERR_PartialMethodOnlyInPartialClass, location);
                }
            }
            if (!base.IsPartial)
            {
                LazyAsyncMethodChecks(CancellationToken.None);
            }
            _lazyRefCustomModifiers = ImmutableArray<CustomModifier>.Empty;
            MethodSymbol methodSymbol = null;
            if (MethodKind != MethodKind.ExplicitInterfaceImplementation)
            {
                _lazyExplicitInterfaceImplementations = ImmutableArray<MethodSymbol>.Empty;
                if (IsOverride)
                {
                    methodSymbol = base.OverriddenMethod;
                    if ((object)methodSymbol != null)
                    {
                        CustomModifierUtils.CopyMethodCustomModifiers(methodSymbol, this, out _lazyReturnType, out _lazyRefCustomModifiers, out _lazyParameters, alsoCopyParamsModifier: true);
                    }
                }
                else if (RefKind == RefKind.In)
                {
                    NamedTypeSymbol wellKnownType = Binder.GetWellKnownType(DeclaringCompilation, WellKnownType.System_Runtime_InteropServices_InAttribute, diagnostics, ReturnTypeLocation);
                    _lazyRefCustomModifiers = ImmutableArray.Create(CSharpCustomModifier.CreateRequired(wellKnownType));
                }
            }
            else if ((object)ExplicitInterfaceType != null)
            {
                methodSymbol = FindExplicitlyImplementedMethod(diagnostics);
                if ((object)methodSymbol != null)
                {
                    _lazyExplicitInterfaceImplementations = ImmutableArray.Create(methodSymbol);
                    CustomModifierUtils.CopyMethodCustomModifiers(methodSymbol, this, out _lazyReturnType, out _lazyRefCustomModifiers, out _lazyParameters, alsoCopyParamsModifier: false);
                    this.FindExplicitlyImplementedMemberVerification(methodSymbol, diagnostics);
                    TypeSymbol.CheckNullableReferenceTypeMismatchOnImplementingMember(ContainingType, this, methodSymbol, isExplicit: true, diagnostics);
                }
                else
                {
                    _lazyExplicitInterfaceImplementations = ImmutableArray<MethodSymbol>.Empty;
                }
            }
            if (!immutableArray.IsDefault && (object)methodSymbol != null)
            {
                for (int i = 0; i < immutableArray.Length; i++)
                {
                    TypeParameterSymbol typeParameterSymbol = _typeParameters[i];
                    TypeParameterConstraintKind typeParameterConstraintKind = immutableArray[i].Constraints & (TypeParameterConstraintKind.ReferenceType | TypeParameterConstraintKind.ValueType | TypeParameterConstraintKind.Default);
                    ErrorCode code;
                    if (typeParameterConstraintKind != TypeParameterConstraintKind.ReferenceType)
                    {
                        if (typeParameterConstraintKind != TypeParameterConstraintKind.ValueType)
                        {
                            if (typeParameterConstraintKind != TypeParameterConstraintKind.Default || (!typeParameterSymbol.IsReferenceType && !typeParameterSymbol.IsValueType))
                            {
                                continue;
                            }
                            code = ErrorCode.ERR_OverrideDefaultConstraintNotSatisfied;
                        }
                        else
                        {
                            if (typeParameterSymbol.IsNonNullableValueType())
                            {
                                continue;
                            }
                            code = ErrorCode.ERR_OverrideValConstraintNotSatisfied;
                        }
                    }
                    else
                    {
                        if (typeParameterSymbol.IsReferenceType)
                        {
                            continue;
                        }
                        code = ErrorCode.ERR_OverrideRefConstraintNotSatisfied;
                    }
                    diagnostics.Add(code, typeParameterSymbol.Locations[0], this, typeParameterSymbol, methodSymbol.TypeParameters[i], methodSymbol);
                }
            }
            CheckModifiers(MethodKind == MethodKind.ExplicitInterfaceImplementation, isVararg, HasAnyBody, location, diagnostics);
        }

        protected abstract (TypeWithAnnotations ReturnType, ImmutableArray<ParameterSymbol> Parameters, bool IsVararg, ImmutableArray<TypeParameterConstraintClause> DeclaredConstraintsForOverrideOrImplementation) MakeParametersAndBindReturnType(BindingDiagnosticBag diagnostics);

        protected abstract void ExtensionMethodChecks(BindingDiagnosticBag diagnostics);

        protected abstract MethodSymbol FindExplicitlyImplementedMethod(BindingDiagnosticBag diagnostics);

        protected sealed override void LazyAsyncMethodChecks(CancellationToken cancellationToken)
        {
            if (!IsAsync)
            {
                CompleteAsyncMethodChecks(null, cancellationToken);
                return;
            }
            BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
            AsyncMethodChecks(instance);
            CompleteAsyncMethodChecks(instance, cancellationToken);
            instance.Free();
        }

        private void CompleteAsyncMethodChecks(BindingDiagnosticBag diagnosticsOpt, CancellationToken cancellationToken)
        {
            if (state.NotePartComplete(CompletionPart.Members))
            {
                if (diagnosticsOpt != null)
                {
                    AddDeclarationDiagnostics(diagnosticsOpt);
                }
                CompleteAsyncMethodChecksBetweenStartAndFinish();
                state.NotePartComplete(CompletionPart.TypeMembers);
            }
            else
            {
                state.SpinWaitComplete(CompletionPart.TypeMembers, cancellationToken);
            }
        }

        protected abstract void CompleteAsyncMethodChecksBetweenStartAndFinish();

        protected abstract int GetParameterCountFromSyntax();

        public abstract override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken));

        internal abstract override OneOrMany<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations();

        private (DeclarationModifiers mods, bool hasExplicitAccessMod) MakeModifiers(MethodKind methodKind, bool isPartial, bool hasBody, Location location, BindingDiagnosticBag diagnostics)
        {
            bool isInterface = ContainingType.IsInterface;
            bool flag = methodKind == MethodKind.ExplicitInterfaceImplementation;
            DeclarationModifiers declarationModifiers = ((isInterface && isPartial && !flag) ? DeclarationModifiers.Public : DeclarationModifiers.Private);
            DeclarationModifiers declarationModifiers2 = DeclarationModifiers.Partial | DeclarationModifiers.Unsafe;
            DeclarationModifiers declarationModifiers3 = DeclarationModifiers.None;
            if (!flag)
            {
                declarationModifiers2 |= DeclarationModifiers.AccessibilityMask | DeclarationModifiers.Abstract | DeclarationModifiers.Sealed | DeclarationModifiers.Static | DeclarationModifiers.New | DeclarationModifiers.Virtual;
                if (!isInterface)
                {
                    declarationModifiers2 |= DeclarationModifiers.Override;
                }
                else
                {
                    declarationModifiers = DeclarationModifiers.None;
                    declarationModifiers3 |= DeclarationModifiers.AccessibilityMask | DeclarationModifiers.Abstract | DeclarationModifiers.Sealed | DeclarationModifiers.Static | DeclarationModifiers.Extern | DeclarationModifiers.Partial | DeclarationModifiers.Virtual | DeclarationModifiers.Async;
                }
            }
            else if (isInterface)
            {
                declarationModifiers2 |= DeclarationModifiers.Abstract;
            }
            declarationModifiers2 |= DeclarationModifiers.Extern | DeclarationModifiers.Async;
            if (ContainingType.IsStructType())
            {
                declarationModifiers2 |= DeclarationModifiers.ReadOnly;
            }
            DeclarationModifiers declarationModifiers4 = MakeDeclarationModifiers(declarationModifiers2, diagnostics);
            bool item;
            if ((declarationModifiers4 & DeclarationModifiers.AccessibilityMask) == 0)
            {
                item = false;
                declarationModifiers4 |= declarationModifiers;
            }
            else
            {
                item = true;
            }
            this.CheckUnsafeModifier(declarationModifiers4, diagnostics);
            ModifierUtils.ReportDefaultInterfaceImplementationModifiers(hasBody, declarationModifiers4, declarationModifiers3, location, diagnostics);
            declarationModifiers4 = AddImpliedModifiers(declarationModifiers4, isInterface, methodKind, hasBody);
            return (declarationModifiers4, item);
        }

        protected abstract DeclarationModifiers MakeDeclarationModifiers(DeclarationModifiers allowedModifiers, BindingDiagnosticBag diagnostics);

        private static DeclarationModifiers AddImpliedModifiers(DeclarationModifiers mods, bool containingTypeIsInterface, MethodKind methodKind, bool hasBody)
        {
            if (containingTypeIsInterface)
            {
                mods = ModifierUtils.AdjustModifiersForAnInterfaceMember(mods, hasBody, methodKind == MethodKind.ExplicitInterfaceImplementation);
            }
            else if (methodKind == MethodKind.ExplicitInterfaceImplementation)
            {
                mods = (mods & ~DeclarationModifiers.AccessibilityMask) | DeclarationModifiers.Private;
            }
            return mods;
        }

        private void CheckModifiers(bool isExplicitInterfaceImplementation, bool isVararg, bool hasBody, Location location, BindingDiagnosticBag diagnostics)
        {
            bool flag = isExplicitInterfaceImplementation && ContainingType.IsInterface;
            if (base.IsPartial && HasExplicitAccessModifier)
            {
                Binder.CheckFeatureAvailability(SyntaxNode, MessageID.IDS_FeatureExtendedPartialMethods, diagnostics, location);
            }
            if (base.IsPartial && IsAbstract)
            {
                diagnostics.Add(ErrorCode.ERR_PartialMethodInvalidModifier, location);
            }
            else if (base.IsPartial && !HasExplicitAccessModifier && !ReturnsVoid)
            {
                diagnostics.Add(ErrorCode.ERR_PartialMethodWithNonVoidReturnMustHaveAccessMods, location, this);
            }
            else if (base.IsPartial && !HasExplicitAccessModifier && HasExtendedPartialModifier)
            {
                diagnostics.Add(ErrorCode.ERR_PartialMethodWithExtendedModMustHaveAccessMods, location, this);
            }
            else if (base.IsPartial && !HasExplicitAccessModifier && Parameters.Any((ParameterSymbol p) => p.RefKind == RefKind.Out))
            {
                diagnostics.Add(ErrorCode.ERR_PartialMethodWithOutParamMustHaveAccessMods, location, this);
            }
            else if (DeclaredAccessibility == Accessibility.Private && (IsVirtual || (IsAbstract && !flag) || IsOverride))
            {
                diagnostics.Add(ErrorCode.ERR_VirtualPrivate, location, this);
            }
            else if (IsStatic && (IsOverride || IsVirtual || IsAbstract))
            {
                diagnostics.Add(ErrorCode.ERR_StaticNotVirtual, location, this);
            }
            else if (IsOverride && (base.IsNew || IsVirtual))
            {
                diagnostics.Add(ErrorCode.ERR_OverrideNotNew, location, this);
            }
            else if (IsSealed && !IsOverride && (!flag || !IsAbstract))
            {
                diagnostics.Add(ErrorCode.ERR_SealedNonOverride, location, this);
            }
            else if (IsSealed && ContainingType.TypeKind == TypeKind.Struct)
            {
                diagnostics.Add(ErrorCode.ERR_BadMemberFlag, location, SyntaxFacts.GetText(SyntaxKind.SealedKeyword));
            }
            else if (_lazyReturnType.IsStatic)
            {
                diagnostics.Add(ErrorFacts.GetStaticClassReturnCode(ContainingType.IsInterfaceType()), location, _lazyReturnType.Type);
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
            else if (IsAbstract && ContainingType.TypeKind == TypeKind.Struct)
            {
                diagnostics.Add(ErrorCode.ERR_BadMemberFlag, location, SyntaxFacts.GetText(SyntaxKind.AbstractKeyword));
            }
            else if (IsVirtual && ContainingType.TypeKind == TypeKind.Struct)
            {
                diagnostics.Add(ErrorCode.ERR_BadMemberFlag, location, SyntaxFacts.GetText(SyntaxKind.VirtualKeyword));
            }
            else if (IsStatic && IsDeclaredReadOnly)
            {
                diagnostics.Add(ErrorCode.ERR_StaticMemberCantBeReadOnly, location, this);
            }
            else if (IsAbstract && !ContainingType.IsAbstract && (ContainingType.TypeKind == TypeKind.Class || ContainingType.TypeKind == TypeKind.Submission))
            {
                diagnostics.Add(ErrorCode.ERR_AbstractInConcreteClass, location, this, ContainingType);
            }
            else if (IsVirtual && ContainingType.IsSealed)
            {
                diagnostics.Add(ErrorCode.ERR_NewVirtualInSealed, location, this, ContainingType);
            }
            else if (!hasBody && IsAsync)
            {
                diagnostics.Add(ErrorCode.ERR_BadAsyncLacksBody, location);
            }
            else if (!hasBody && !IsExtern && !IsAbstract && !base.IsPartial && !IsExpressionBodied)
            {
                diagnostics.Add(ErrorCode.ERR_ConcreteMissingBody, location, this);
            }
            else if (ContainingType.IsSealed && DeclaredAccessibility.HasProtected() && !IsOverride)
            {
                diagnostics.Add(AccessCheck.GetProtectedMemberInSealedTypeError(ContainingType), location, this);
            }
            else if (ContainingType.IsStatic && !IsStatic)
            {
                diagnostics.Add(ErrorCode.ERR_InstanceMemberInStaticClass, location, Name);
            }
            else if (isVararg && (IsGenericMethod || ContainingType.IsGenericType || (_lazyParameters.Length > 0 && _lazyParameters[_lazyParameters.Length - 1].IsParams)))
            {
                diagnostics.Add(ErrorCode.ERR_BadVarargs, location);
            }
            else if (isVararg && IsAsync)
            {
                diagnostics.Add(ErrorCode.ERR_VarargsAsync, location);
            }
        }

        internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
            base.AddSynthesizedAttributes(moduleBuilder, ref attributes);
            if (IsExtensionMethod)
            {
                CSharpCompilation declaringCompilation = DeclaringCompilation;
                Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_ExtensionAttribute__ctor));
            }
        }

        internal override void AfterAddingTypeMembersChecks(ConversionsBase conversions, BindingDiagnosticBag diagnostics)
        {
            base.AfterAddingTypeMembersChecks(conversions, diagnostics);
            Location returnTypeLocation = ReturnTypeLocation;
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            CheckConstraintsForExplicitInterfaceType(conversions, diagnostics);
            base.ReturnType.CheckAllConstraints(declaringCompilation, conversions, Locations[0], diagnostics);
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = Parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                current.Type.CheckAllConstraints(declaringCompilation, conversions, current.Locations[0], diagnostics);
            }
            PartialMethodChecks(diagnostics);
            if (RefKind == RefKind.In)
            {
                declaringCompilation.EnsureIsReadOnlyAttributeExists(diagnostics, returnTypeLocation, modifyCompilation: true);
            }
            ParameterHelpers.EnsureIsReadOnlyAttributeExists(declaringCompilation, Parameters, diagnostics, modifyCompilation: true);
            if (base.ReturnType.ContainsNativeInteger())
            {
                declaringCompilation.EnsureNativeIntegerAttributeExists(diagnostics, returnTypeLocation, modifyCompilation: true);
            }
            ParameterHelpers.EnsureNativeIntegerAttributeExists(declaringCompilation, Parameters, diagnostics, modifyCompilation: true);
            if (declaringCompilation.ShouldEmitNullableAttributes(this) && ReturnTypeWithAnnotations.NeedsNullableAttribute())
            {
                declaringCompilation.EnsureNullableAttributeExists(diagnostics, returnTypeLocation, modifyCompilation: true);
            }
            ParameterHelpers.EnsureNullableAttributeExists(declaringCompilation, this, Parameters, diagnostics, modifyCompilation: true);
        }

        protected abstract void CheckConstraintsForExplicitInterfaceType(ConversionsBase conversions, BindingDiagnosticBag diagnostics);

        protected abstract void PartialMethodChecks(BindingDiagnosticBag diagnostics);
    }
}
