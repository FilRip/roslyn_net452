using System.Collections.Immutable;
using System.Globalization;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.RuntimeMembers;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SourceOrdinaryMethodSymbol : SourceOrdinaryMethodSymbolBase
    {
        private readonly TypeSymbol _explicitInterfaceType;

        private readonly bool _isExpressionBodied;

        private readonly bool _hasAnyBody;

        private readonly RefKind _refKind;

        private bool _lazyIsVararg;

        private ImmutableArray<ImmutableArray<TypeWithAnnotations>> _lazyTypeParameterConstraintTypes;

        private ImmutableArray<TypeParameterConstraintKind> _lazyTypeParameterConstraintKinds;

        private SourceOrdinaryMethodSymbol _otherPartOfPartial;

        protected override Location ReturnTypeLocation => GetSyntax().ReturnType.Location;

        protected override TypeSymbol ExplicitInterfaceType => _explicitInterfaceType;

        protected override bool HasAnyBody => _hasAnyBody;

        public override bool IsVararg
        {
            get
            {
                LazyMethodChecks();
                return _lazyIsVararg;
            }
        }

        public override RefKind RefKind => _refKind;

        internal SourceOrdinaryMethodSymbol OtherPartOfPartial => _otherPartOfPartial;

        internal bool IsPartialDefinition
        {
            get
            {
                if (base.IsPartial && !_hasAnyBody)
                {
                    return !base.HasExternModifier;
                }
                return false;
            }
        }

        internal bool IsPartialImplementation
        {
            get
            {
                if (base.IsPartial)
                {
                    if (!_hasAnyBody)
                    {
                        return base.HasExternModifier;
                    }
                    return true;
                }
                return false;
            }
        }

        internal bool IsPartialWithoutImplementation
        {
            get
            {
                if (IsPartialDefinition)
                {
                    return (object)_otherPartOfPartial == null;
                }
                return false;
            }
        }

        internal SourceOrdinaryMethodSymbol SourcePartialDefinition
        {
            get
            {
                if (!IsPartialImplementation)
                {
                    return null;
                }
                return _otherPartOfPartial;
            }
        }

        internal SourceOrdinaryMethodSymbol SourcePartialImplementation
        {
            get
            {
                if (!IsPartialDefinition)
                {
                    return null;
                }
                return _otherPartOfPartial;
            }
        }

        public override MethodSymbol PartialDefinitionPart => SourcePartialDefinition;

        public override MethodSymbol PartialImplementationPart => SourcePartialImplementation;

        public sealed override bool IsExtern
        {
            get
            {
                if (!IsPartialDefinition)
                {
                    return base.HasExternModifier;
                }
                return _otherPartOfPartial?.IsExtern ?? false;
            }
        }

        protected override SourceMemberMethodSymbol BoundAttributesSource => SourcePartialDefinition;

        private SyntaxList<AttributeListSyntax> AttributeDeclarationSyntaxList
        {
            get
            {
                if (ContainingType is SourceMemberContainerTypeSymbol sourceMemberContainerTypeSymbol && sourceMemberContainerTypeSymbol.AnyMemberHasAttributes)
                {
                    return GetSyntax().AttributeLists;
                }
                return default(SyntaxList<AttributeListSyntax>);
            }
        }

        internal override bool IsExpressionBodied => _isExpressionBodied;

        internal override bool GenerateDebugInfo
        {
            get
            {
                if (!IsAsync)
                {
                    return !IsIterator;
                }
                return false;
            }
        }

        public static SourceOrdinaryMethodSymbol CreateMethodSymbol(NamedTypeSymbol containingType, Binder bodyBinder, MethodDeclarationSyntax syntax, bool isNullableAnalysisEnabled, BindingDiagnosticBag diagnostics)
        {
            ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier = syntax.ExplicitInterfaceSpecifier;
            SyntaxToken token = syntax.Identifier;
            string memberNameAndInterfaceSymbol = ExplicitInterfaceHelpers.GetMemberNameAndInterfaceSymbol(bodyBinder, explicitInterfaceSpecifier, token.ValueText, diagnostics, out TypeSymbol explicitInterfaceTypeOpt, out string aliasQualifierOpt);
            SourceLocation location = new SourceLocation(in token);
            MethodKind methodKind = ((explicitInterfaceSpecifier == null) ? MethodKind.Ordinary : MethodKind.ExplicitInterfaceImplementation);
            return new SourceOrdinaryMethodSymbol(containingType, explicitInterfaceTypeOpt, memberNameAndInterfaceSymbol, location, syntax, methodKind, isNullableAnalysisEnabled, diagnostics);
        }

        private SourceOrdinaryMethodSymbol(
            NamedTypeSymbol containingType,
            TypeSymbol explicitInterfaceType,
            string name,
            Location location,
            MethodDeclarationSyntax syntax,
            MethodKind methodKind,
            bool isNullableAnalysisEnabled,
            BindingDiagnosticBag diagnostics) :
            base(containingType,
                 name,
                 location,
                 syntax,
                 methodKind,
                 isIterator: SyntaxFacts.HasYieldOperations(syntax.Body),
                 isExtensionMethod: syntax.ParameterList.Parameters.FirstOrDefault() is ParameterSyntax firstParam &&
                                    !firstParam.IsArgList &&
                                    firstParam.Modifiers.Any(SyntaxKind.ThisKeyword),
                 isPartial: syntax.Modifiers.IndexOf(SyntaxKind.PartialKeyword) < 0,
                 hasBody: syntax.Body != null || syntax.ExpressionBody != null,
                 isNullableAnalysisEnabled: isNullableAnalysisEnabled,
                 diagnostics)
        {
            _explicitInterfaceType = explicitInterfaceType;

            bool hasBlockBody = syntax.Body != null;
            _isExpressionBodied = !hasBlockBody && syntax.ExpressionBody != null;
            bool hasBody = hasBlockBody || _isExpressionBodied;
            _hasAnyBody = hasBody;
            _refKind = syntax.ReturnType.GetRefKind();

            CheckForBlockAndExpressionBody(
                syntax.Body, syntax.ExpressionBody, syntax, diagnostics);
        }

        protected override ImmutableArray<TypeParameterSymbol> MakeTypeParameters(CSharpSyntaxNode node, BindingDiagnosticBag diagnostics)
        {
            MethodDeclarationSyntax methodDeclarationSyntax = (MethodDeclarationSyntax)node;
            if (methodDeclarationSyntax.Arity == 0)
            {
                Symbol.ReportErrorIfHasConstraints(methodDeclarationSyntax.ConstraintClauses, diagnostics.DiagnosticBag);
                return ImmutableArray<TypeParameterSymbol>.Empty;
            }
            return MakeTypeParameters(methodDeclarationSyntax, diagnostics);
        }

        protected override (TypeWithAnnotations ReturnType, ImmutableArray<ParameterSymbol> Parameters, bool IsVararg, ImmutableArray<TypeParameterConstraintClause> DeclaredConstraintsForOverrideOrImplementation) MakeParametersAndBindReturnType(BindingDiagnosticBag diagnostics)
        {
            MethodDeclarationSyntax syntax = GetSyntax();
            Binder binder = DeclaringCompilation.GetBinderFactory(syntax.SyntaxTree).GetBinder(syntax.ReturnType, syntax, this).WithAdditionalFlagsAndContainingMemberOrLambda(BinderFlags.SuppressConstraintChecks, this);
            ParameterListSyntax parameterList = syntax.ParameterList;
            bool addRefReadOnlyModifier = IsVirtual || IsAbstract;
            ImmutableArray<ParameterSymbol> item = ParameterHelpers.MakeParameters(binder, this, parameterList, out SyntaxToken arglistToken, diagnostics, allowRefOrOut: true, allowThis: true, addRefReadOnlyModifier);
            _lazyIsVararg = arglistToken.Kind() == SyntaxKind.ArgListKeyword;
            TypeSyntax syntax2 = syntax.ReturnType.SkipRef(out RefKind refKind);
            TypeWithAnnotations typeWithAnnotations = binder.BindType(syntax2, diagnostics);
            if (typeWithAnnotations.IsRestrictedType(ignoreSpanLikeTypes: true) && (typeWithAnnotations.SpecialType != SpecialType.System_TypedReference || (ContainingType.SpecialType != SpecialType.System_TypedReference && ContainingType.SpecialType != SpecialType.System_ArgIterator)))
            {
                diagnostics.Add(ErrorCode.ERR_MethodReturnCantBeRefAny, syntax.ReturnType.Location, typeWithAnnotations.Type);
            }
            ImmutableArray<TypeParameterConstraintClause> immutableArray = default(ImmutableArray<TypeParameterConstraintClause>);
            if (Arity != 0 && (syntax.ExplicitInterfaceSpecifier != null || IsOverride))
            {
                if (syntax.ConstraintClauses.Count > 0)
                {
                    Binder.CheckFeatureAvailability(syntax.SyntaxTree, MessageID.IDS_OverrideWithConstraints, diagnostics, syntax.ConstraintClauses[0].WhereKeyword.GetLocation());
                    immutableArray = binder.WithAdditionalFlags(BinderFlags.SuppressConstraintChecks | BinderFlags.GenericConstraintsClause).BindTypeParameterConstraintClauses(this, TypeParameters, syntax.TypeParameterList, syntax.ConstraintClauses, diagnostics, performOnlyCycleSafeValidation: false, isForOverride: true);
                }
                ImmutableArray<ParameterSymbol>.Enumerator enumerator = item.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    forceMethodTypeParameters(enumerator.Current.TypeWithAnnotations, this, immutableArray);
                }
                forceMethodTypeParameters(typeWithAnnotations, this, immutableArray);
            }
            return (typeWithAnnotations, item, _lazyIsVararg, immutableArray);
            static void forceMethodTypeParameters(TypeWithAnnotations type, SourceOrdinaryMethodSymbol method, ImmutableArray<TypeParameterConstraintClause> declaredConstraints)
            {
                type.VisitType(null, delegate (TypeWithAnnotations type, (SourceOrdinaryMethodSymbol method, ImmutableArray<TypeParameterConstraintClause> declaredConstraints) args, bool unused2)
                {
                    if (type.DefaultType is TypeParameterSymbol typeParameterSymbol && (object)typeParameterSymbol.DeclaringMethod == args.method)
                    {
                        bool asValueType = args.declaredConstraints.IsDefault || (args.declaredConstraints[typeParameterSymbol.Ordinal].Constraints & (TypeParameterConstraintKind.ReferenceType | TypeParameterConstraintKind.Default)) == 0;
                        type.TryForceResolve(asValueType);
                    }
                    return false;
                }, null, (method, declaredConstraints), canDigThroughNullable: false, useDefaultType: true);
            }
        }

        protected override void ExtensionMethodChecks(BindingDiagnosticBag diagnostics)
        {
            if (!IsExtensionMethod)
            {
                return;
            }
            MethodDeclarationSyntax syntax = GetSyntax();
            Location location = locations[0];
            TypeWithAnnotations typeWithAnnotations = Parameters[0].TypeWithAnnotations;
            RefKind refKind = Parameters[0].RefKind;
            if (!typeWithAnnotations.Type.IsValidExtensionParameterType())
            {
                Location location2 = syntax.ParameterList.Parameters[0].Type!.Location;
                diagnostics.Add(ErrorCode.ERR_BadTypeforThis, location2, typeWithAnnotations.Type);
                return;
            }
            if (refKind == RefKind.Ref && !typeWithAnnotations.Type.IsValueType)
            {
                diagnostics.Add(ErrorCode.ERR_RefExtensionMustBeValueTypeOrConstrainedToOne, location, Name);
                return;
            }
            if (refKind == RefKind.In && typeWithAnnotations.TypeKind != TypeKind.Struct)
            {
                diagnostics.Add(ErrorCode.ERR_InExtensionMustBeValueType, location, Name);
                return;
            }
            if ((object)ContainingType.ContainingType != null)
            {
                diagnostics.Add(ErrorCode.ERR_ExtensionMethodsDecl, location, ContainingType.Name);
                return;
            }
            if (!ContainingType.IsScriptClass && (!ContainingType.IsStatic || ContainingType.Arity != 0))
            {
                Location location3 = ((syntax.Parent is TypeDeclarationSyntax typeDeclarationSyntax) ? typeDeclarationSyntax.Identifier : syntax.Identifier).GetLocation();
                diagnostics.Add(ErrorCode.ERR_BadExtensionAgg, location3);
                return;
            }
            if (!IsStatic)
            {
                diagnostics.Add(ErrorCode.ERR_BadExtensionMeth, location);
                return;
            }
            Symbol wellKnownTypeMember = Binder.GetWellKnownTypeMember(DeclaringCompilation, WellKnownMember.System_Runtime_CompilerServices_ExtensionAttribute__ctor, out UseSiteInfo<AssemblySymbol> useSiteInfo);
            Location location4 = syntax.ParameterList.Parameters[0].Modifiers.FirstOrDefault(SyntaxKind.ThisKeyword).GetLocation();
            if ((object)wellKnownTypeMember == null)
            {
                MemberDescriptor descriptor = WellKnownMembers.GetDescriptor(WellKnownMember.System_Runtime_CompilerServices_ExtensionAttribute__ctor);
                diagnostics.Add(ErrorCode.ERR_ExtensionAttrNotFound, location4, descriptor.DeclaringTypeMetadataName);
            }
            else
            {
                diagnostics.Add(useSiteInfo, location4);
            }
        }

        protected override MethodSymbol FindExplicitlyImplementedMethod(BindingDiagnosticBag diagnostics)
        {
            MethodDeclarationSyntax syntax = GetSyntax();
            return this.FindExplicitlyImplementedMethod(_explicitInterfaceType, syntax.Identifier.ValueText, syntax.ExplicitInterfaceSpecifier, diagnostics);
        }

        internal MethodDeclarationSyntax GetSyntax()
        {
            return (MethodDeclarationSyntax)syntaxReferenceOpt.GetSyntax();
        }

        protected override void CompleteAsyncMethodChecksBetweenStartAndFinish()
        {
            if (IsPartialDefinition)
            {
                DeclaringCompilation.SymbolDeclaredEvent(this);
            }
        }

        public override ImmutableArray<ImmutableArray<TypeWithAnnotations>> GetTypeParameterConstraintTypes()
        {
            if (_lazyTypeParameterConstraintTypes.IsDefault)
            {
                GetTypeParameterConstraintKinds();
                BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
                MethodDeclarationSyntax syntax = GetSyntax();
                Binder binder = DeclaringCompilation.GetBinderFactory(syntax.SyntaxTree).GetBinder(syntax.ReturnType, syntax, this);
                ImmutableArray<ImmutableArray<TypeWithAnnotations>> value = this.MakeTypeParameterConstraintTypes(binder, TypeParameters, syntax.TypeParameterList, syntax.ConstraintClauses, instance);
                if (ImmutableInterlocked.InterlockedInitialize(ref _lazyTypeParameterConstraintTypes, value))
                {
                    AddDeclarationDiagnostics(instance);
                }
                instance.Free();
            }
            return _lazyTypeParameterConstraintTypes;
        }

        public override ImmutableArray<TypeParameterConstraintKind> GetTypeParameterConstraintKinds()
        {
            if (_lazyTypeParameterConstraintKinds.IsDefault)
            {
                MethodDeclarationSyntax syntax = GetSyntax();
                Binder binder = DeclaringCompilation.GetBinderFactory(syntax.SyntaxTree).GetBinder(syntax.ReturnType, syntax, this);
                ImmutableArray<TypeParameterConstraintKind> value = this.MakeTypeParameterConstraintKinds(binder, TypeParameters, syntax.TypeParameterList, syntax.ConstraintClauses);
                ImmutableInterlocked.InterlockedInitialize(ref _lazyTypeParameterConstraintKinds, value);
            }
            return _lazyTypeParameterConstraintKinds;
        }

        protected override int GetParameterCountFromSyntax()
        {
            return GetSyntax().ParameterList.ParameterCount;
        }

        internal static void InitializePartialMethodParts(SourceOrdinaryMethodSymbol definition, SourceOrdinaryMethodSymbol implementation)
        {
            definition._otherPartOfPartial = implementation;
            implementation._otherPartOfPartial = definition;
        }

        public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            ref string lazyXmlText = ref expandIncludes ? ref lazyExpandedDocComment : ref lazyDocComment;
            return SourceDocumentationCommentUtils.GetAndCacheDocumentationComment(SourcePartialImplementation ?? this, expandIncludes, ref lazyXmlText);
        }

        internal override OneOrMany<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
        {
            if ((object)SourcePartialImplementation != null)
            {
                return OneOrMany.Create(ImmutableArray.Create(AttributeDeclarationSyntaxList, SourcePartialImplementation.AttributeDeclarationSyntaxList));
            }
            return OneOrMany.Create(AttributeDeclarationSyntaxList);
        }

        protected override DeclarationModifiers MakeDeclarationModifiers(DeclarationModifiers allowedModifiers, BindingDiagnosticBag diagnostics)
        {
            return ModifierUtils.MakeAndCheckNontypeMemberModifiers(GetSyntax().Modifiers, DeclarationModifiers.None, allowedModifiers, Locations[0], diagnostics, out bool modifierErrors);
        }

        private ImmutableArray<TypeParameterSymbol> MakeTypeParameters(MethodDeclarationSyntax syntax, BindingDiagnosticBag diagnostics)
        {
            OverriddenMethodTypeParameterMapBase overriddenMethodTypeParameterMapBase = null;
            if (IsOverride)
            {
                overriddenMethodTypeParameterMapBase = new OverriddenMethodTypeParameterMap(this);
            }
            else if (IsExplicitInterfaceImplementation)
            {
                overriddenMethodTypeParameterMapBase = new ExplicitInterfaceMethodTypeParameterMap(this);
            }
            SeparatedSyntaxList<TypeParameterSyntax> parameters = syntax.TypeParameterList!.Parameters;
            ArrayBuilder<TypeParameterSymbol> instance = ArrayBuilder<TypeParameterSymbol>.GetInstance();
            for (int i = 0; i < parameters.Count; i++)
            {
                TypeParameterSyntax typeParameterSyntax = parameters[i];
                if (typeParameterSyntax.VarianceKeyword.Kind() != 0)
                {
                    diagnostics.Add(ErrorCode.ERR_IllegalVarianceSyntax, typeParameterSyntax.VarianceKeyword.GetLocation());
                }
                SyntaxToken identifier = typeParameterSyntax.Identifier;
                Location location = identifier.GetLocation();
                string valueText = identifier.ValueText;
                for (int j = 0; j < instance.Count; j++)
                {
                    if (valueText == instance[j].Name)
                    {
                        diagnostics.Add(ErrorCode.ERR_DuplicateTypeParameter, location, valueText);
                        break;
                    }
                }
                SourceMemberContainerTypeSymbol.ReportTypeNamedRecord(identifier.Text, DeclaringCompilation, diagnostics.DiagnosticBag, location);
                TypeParameterSymbol typeParameterSymbol = ContainingType.FindEnclosingTypeParameter(valueText);
                if ((object)typeParameterSymbol != null)
                {
                    diagnostics.Add(ErrorCode.WRN_TypeParameterSameAsOuterTypeParameter, location, valueText, typeParameterSymbol.ContainingType);
                }
                ImmutableArray<SyntaxReference> syntaxRefs = ImmutableArray.Create(typeParameterSyntax.GetReference());
                ImmutableArray<Location> immutableArray = ImmutableArray.Create(location);
                TypeParameterSymbol item = ((overriddenMethodTypeParameterMapBase != null) ? new SourceOverridingMethodTypeParameterSymbol(overriddenMethodTypeParameterMapBase, valueText, i, immutableArray, syntaxRefs) : ((SourceTypeParameterSymbolBase)new SourceMethodTypeParameterSymbol(this, valueText, i, immutableArray, syntaxRefs)));
                instance.Add(item);
            }
            return instance.ToImmutableAndFree();
        }

        internal override void ForceComplete(SourceLocation locationOpt, CancellationToken cancellationToken)
        {
            SourcePartialImplementation?.ForceComplete(locationOpt, cancellationToken);
            base.ForceComplete(locationOpt, cancellationToken);
        }

        internal override bool IsDefinedInSourceTree(SyntaxTree tree, TextSpan? definedWithinSpan, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!base.IsDefinedInSourceTree(tree, definedWithinSpan, cancellationToken))
            {
                return SourcePartialImplementation?.IsDefinedInSourceTree(tree, definedWithinSpan, cancellationToken) ?? false;
            }
            return true;
        }

        protected override void CheckConstraintsForExplicitInterfaceType(ConversionsBase conversions, BindingDiagnosticBag diagnostics)
        {
            if ((object)_explicitInterfaceType != null)
            {
                MethodDeclarationSyntax syntax = GetSyntax();
                _explicitInterfaceType.CheckAllConstraints(DeclaringCompilation, conversions, new SourceLocation(syntax.ExplicitInterfaceSpecifier!.Name), diagnostics);
            }
        }

        protected override void PartialMethodChecks(BindingDiagnosticBag diagnostics)
        {
            SourceOrdinaryMethodSymbol sourcePartialImplementation = SourcePartialImplementation;
            if ((object)sourcePartialImplementation != null)
            {
                PartialMethodChecks(this, sourcePartialImplementation, diagnostics);
            }
        }

        private static void PartialMethodChecks(SourceOrdinaryMethodSymbol definition, SourceOrdinaryMethodSymbol implementation, BindingDiagnosticBag diagnostics)
        {
            MethodSymbol methodSymbol = definition.ConstructIfGeneric(TypeMap.TypeParametersAsTypeSymbolsWithIgnoredAnnotations(implementation.TypeParameters));
            bool flag = methodSymbol.ReturnTypeWithAnnotations.Equals(implementation.ReturnTypeWithAnnotations, TypeCompareKind.AllIgnoreOptions);
            if (!flag && !SourceMemberContainerTypeSymbol.IsOrContainsErrorType(implementation.ReturnType) && !SourceMemberContainerTypeSymbol.IsOrContainsErrorType(definition.ReturnType))
            {
                diagnostics.Add(ErrorCode.ERR_PartialMethodReturnTypeDifference, implementation.Locations[0]);
            }
            if (definition.RefKind != implementation.RefKind)
            {
                diagnostics.Add(ErrorCode.ERR_PartialMethodRefReturnDifference, implementation.Locations[0]);
            }
            if (definition.IsStatic != implementation.IsStatic)
            {
                diagnostics.Add(ErrorCode.ERR_PartialMethodStaticDifference, implementation.Locations[0]);
            }
            if (definition.IsDeclaredReadOnly != implementation.IsDeclaredReadOnly)
            {
                diagnostics.Add(ErrorCode.ERR_PartialMethodReadOnlyDifference, implementation.Locations[0]);
            }
            if (definition.IsExtensionMethod != implementation.IsExtensionMethod)
            {
                diagnostics.Add(ErrorCode.ERR_PartialMethodExtensionDifference, implementation.Locations[0]);
            }
            if (definition.IsUnsafe != implementation.IsUnsafe && definition.CompilationAllowsUnsafe())
            {
                diagnostics.Add(ErrorCode.ERR_PartialMethodUnsafeDifference, implementation.Locations[0]);
            }
            if (definition.IsParams() != implementation.IsParams())
            {
                diagnostics.Add(ErrorCode.ERR_PartialMethodParamsDifference, implementation.Locations[0]);
            }
            if (definition.HasExplicitAccessModifier != implementation.HasExplicitAccessModifier || definition.DeclaredAccessibility != implementation.DeclaredAccessibility)
            {
                diagnostics.Add(ErrorCode.ERR_PartialMethodAccessibilityDifference, implementation.Locations[0]);
            }
            if (definition.IsVirtual != implementation.IsVirtual || definition.IsOverride != implementation.IsOverride || definition.IsSealed != implementation.IsSealed || definition.IsNew != implementation.IsNew)
            {
                diagnostics.Add(ErrorCode.ERR_PartialMethodExtendedModDifference, implementation.Locations[0]);
            }
            PartialMethodConstraintsChecks(definition, implementation, diagnostics);
            SourceMemberContainerTypeSymbol.CheckValidNullableMethodOverride(implementation.DeclaringCompilation, methodSymbol, implementation, diagnostics, delegate (BindingDiagnosticBag diagnostics, MethodSymbol implementedMethod, MethodSymbol implementingMethod, bool topLevel, bool returnTypesEqual)
            {
                if (returnTypesEqual)
                {
                    diagnostics.Add(ErrorCode.WRN_NullabilityMismatchInReturnTypeOnPartial, implementingMethod.Locations[0]);
                }
            }, delegate (BindingDiagnosticBag diagnostics, MethodSymbol implementedMethod, MethodSymbol implementingMethod, ParameterSymbol implementingParameter, bool blameAttributes, bool arg)
            {
                diagnostics.Add(ErrorCode.WRN_NullabilityMismatchInParameterTypeOnPartial, implementingMethod.Locations[0], new FormattedSymbol(implementingParameter, SymbolDisplayFormat.ShortFormat));
            }, flag);
        }

        private static void PartialMethodConstraintsChecks(SourceOrdinaryMethodSymbol definition, SourceOrdinaryMethodSymbol implementation, BindingDiagnosticBag diagnostics)
        {
            ImmutableArray<TypeParameterSymbol> typeParameters = definition.TypeParameters;
            int length = typeParameters.Length;
            if (length == 0)
            {
                return;
            }
            ImmutableArray<TypeParameterSymbol> typeParameters2 = implementation.TypeParameters;
            ImmutableArray<TypeWithAnnotations> to = IndexedTypeParameterSymbol.Take(length);
            TypeMap typeMap = new TypeMap(typeParameters, to, allowAlpha: true);
            TypeMap typeMap2 = new TypeMap(typeParameters2, to, allowAlpha: true);
            for (int i = 0; i < length; i++)
            {
                TypeParameterSymbol typeParameter = typeParameters[i];
                TypeParameterSymbol typeParameterSymbol = typeParameters2[i];
                if (!MemberSignatureComparer.HaveSameConstraints(typeParameter, typeMap, typeParameterSymbol, typeMap2))
                {
                    diagnostics.Add(ErrorCode.ERR_PartialMethodInconsistentConstraints, implementation.Locations[0], implementation, typeParameterSymbol.Name);
                }
                else if (!MemberSignatureComparer.HaveSameNullabilityInConstraints(typeParameter, typeMap, typeParameterSymbol, typeMap2))
                {
                    diagnostics.Add(ErrorCode.WRN_NullabilityMismatchInConstraintsOnPartialImplementation, implementation.Locations[0], implementation, typeParameterSymbol.Name);
                }
            }
        }

        internal override bool CallsAreOmitted(SyntaxTree syntaxTree)
        {
            if (IsPartialWithoutImplementation)
            {
                return true;
            }
            return base.CallsAreOmitted(syntaxTree);
        }
    }
}
