using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public class SourcePropertyAccessorSymbol : SourceMemberMethodSymbol
    {
        private readonly SourcePropertySymbolBase _property;

        private ImmutableArray<ParameterSymbol> _lazyParameters;

        private TypeWithAnnotations _lazyReturnType;

        private ImmutableArray<CustomModifier> _lazyRefCustomModifiers;

        private ImmutableArray<MethodSymbol> _lazyExplicitInterfaceImplementations;

        private string _lazyName;

        private readonly bool _isAutoPropertyAccessor;

        private readonly bool _isExpressionBodied;

        private readonly bool _usesInit;

        internal sealed override bool IsExpressionBodied => _isExpressionBodied;

        internal sealed override ImmutableArray<string> NotNullMembers => _property.NotNullMembers.Concat(base.NotNullMembers);

        internal sealed override ImmutableArray<string> NotNullWhenTrueMembers => _property.NotNullWhenTrueMembers.Concat(base.NotNullWhenTrueMembers);

        internal sealed override ImmutableArray<string> NotNullWhenFalseMembers => _property.NotNullWhenFalseMembers.Concat(base.NotNullWhenFalseMembers);

        public sealed override Accessibility DeclaredAccessibility
        {
            get
            {
                Accessibility localAccessibility = LocalAccessibility;
                if (localAccessibility != 0)
                {
                    return localAccessibility;
                }
                return _property.DeclaredAccessibility;
            }
        }

        public sealed override Symbol AssociatedSymbol => _property;

        public sealed override bool IsVararg => false;

        public sealed override bool ReturnsVoid => base.ReturnType.IsVoidType();

        public sealed override ImmutableArray<ParameterSymbol> Parameters
        {
            get
            {
                LazyMethodChecks();
                return _lazyParameters;
            }
        }

        public sealed override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

        public sealed override RefKind RefKind => _property.RefKind;

        public sealed override TypeWithAnnotations ReturnTypeWithAnnotations
        {
            get
            {
                LazyMethodChecks();
                return _lazyReturnType;
            }
        }

        public sealed override FlowAnalysisAnnotations ReturnTypeFlowAnalysisAnnotations
        {
            get
            {
                if (MethodKind == MethodKind.PropertySet)
                {
                    return FlowAnalysisAnnotations.None;
                }
                FlowAnalysisAnnotations flowAnalysisAnnotations = FlowAnalysisAnnotations.None;
                if (_property.HasMaybeNull)
                {
                    flowAnalysisAnnotations |= FlowAnalysisAnnotations.MaybeNull;
                }
                if (_property.HasNotNull)
                {
                    flowAnalysisAnnotations |= FlowAnalysisAnnotations.NotNull;
                }
                return flowAnalysisAnnotations;
            }
        }

        public sealed override ImmutableHashSet<string> ReturnNotNullIfParameterNotNull => ImmutableHashSet<string>.Empty;

        public sealed override ImmutableArray<CustomModifier> RefCustomModifiers
        {
            get
            {
                LazyMethodChecks();
                return _lazyRefCustomModifiers;
            }
        }

        internal Accessibility LocalAccessibility => ModifierUtils.EffectiveAccessibility(DeclarationModifiers);

        internal bool LocalDeclaredReadOnly => (DeclarationModifiers & DeclarationModifiers.ReadOnly) != 0;

        internal sealed override bool IsDeclaredReadOnly
        {
            get
            {
                if (LocalDeclaredReadOnly || (_property.HasReadOnlyModifier && base.IsValidReadOnlyTarget))
                {
                    return true;
                }
                if (!((CSharpParseOptions)base.SyntaxTree.Options).IsFeatureEnabled(MessageID.IDS_FeatureReadOnlyMembers))
                {
                    return false;
                }
                if (!(DeclaringCompilation.GetWellKnownTypeMember(WellKnownMember.System_Runtime_CompilerServices_IsReadOnlyAttribute__ctor) != null) && (DeclaringCompilation.Options.OutputKind == OutputKind.NetModule || !(DeclaringCompilation.GetWellKnownType(WellKnownType.System_Runtime_CompilerServices_IsReadOnlyAttribute) is MissingMetadataTypeSymbol)))
                {
                    return false;
                }
                if (ContainingType.IsStructType() && !_property.IsStatic && _isAutoPropertyAccessor)
                {
                    return MethodKind == MethodKind.PropertyGet;
                }
                return false;
            }
        }

        internal sealed override bool IsInitOnly
        {
            get
            {
                if (!IsStatic)
                {
                    return _usesInit;
                }
                return false;
            }
        }

        internal sealed override bool IsExplicitInterfaceImplementation => _property.IsExplicitInterfaceImplementation;

        public sealed override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations
        {
            get
            {
                if (_lazyExplicitInterfaceImplementations.IsDefault)
                {
                    PropertySymbol propertySymbol = (IsExplicitInterfaceImplementation ? _property.ExplicitInterfaceImplementations.FirstOrDefault() : null);
                    ImmutableArray<MethodSymbol> value;
                    if ((object)propertySymbol == null)
                    {
                        value = ImmutableArray<MethodSymbol>.Empty;
                    }
                    else
                    {
                        MethodSymbol methodSymbol = ((MethodKind == MethodKind.PropertyGet) ? propertySymbol.GetMethod : propertySymbol.SetMethod);
                        value = (((object)methodSymbol == null) ? ImmutableArray<MethodSymbol>.Empty : ImmutableArray.Create(methodSymbol));
                    }
                    ImmutableInterlocked.InterlockedInitialize(ref _lazyExplicitInterfaceImplementations, value);
                }
                return _lazyExplicitInterfaceImplementations;
            }
        }

        public sealed override string Name
        {
            get
            {
                if (_lazyName == null)
                {
                    bool flag = MethodKind == MethodKind.PropertyGet;
                    string text = null;
                    if (IsExplicitInterfaceImplementation)
                    {
                        PropertySymbol propertySymbol = _property.ExplicitInterfaceImplementations.FirstOrDefault();
                        if ((object)propertySymbol != null)
                        {
                            MethodSymbol methodSymbol = (flag ? propertySymbol.GetMethod : propertySymbol.SetMethod);
                            text = ExplicitInterfaceHelpers.GetMemberName(((object)methodSymbol != null) ? methodSymbol.Name : GetAccessorName(propertySymbol.MetadataName, flag, _property.IsCompilationOutputWinMdObj()), aliasQualifierOpt: _property.GetExplicitInterfaceSpecifier()?.Name.GetAliasQualifierOpt(), explicitInterfaceTypeOpt: propertySymbol.ContainingType);
                        }
                    }
                    else if (IsOverride)
                    {
                        MethodSymbol overriddenMethod = base.OverriddenMethod;
                        if ((object)overriddenMethod != null)
                        {
                            text = overriddenMethod.Name;
                        }
                    }
                    if (text == null)
                    {
                        text = GetAccessorName(_property.SourceName, flag, _property.IsCompilationOutputWinMdObj());
                    }
                    InterlockedOperations.Initialize(ref _lazyName, text);
                }
                return _lazyName;
            }
        }

        public sealed override bool IsImplicitlyDeclared
        {
            get
            {
                SyntaxKind syntaxKind = GetSyntax().Kind();
                if (syntaxKind - 8896 <= SyntaxKind.List || syntaxKind == SyntaxKind.ArrowExpressionClause || syntaxKind == SyntaxKind.InitAccessorDeclaration)
                {
                    return false;
                }
                return true;
            }
        }

        internal sealed override bool GenerateDebugInfo => true;

        public sealed override bool AreLocalsZeroed
        {
            get
            {
                if (!_property.HasSkipLocalsInitAttribute)
                {
                    return base.AreLocalsZeroed;
                }
                return false;
            }
        }

        public static SourcePropertyAccessorSymbol CreateAccessorSymbol(NamedTypeSymbol containingType, SourcePropertySymbol property, DeclarationModifiers propertyModifiers, AccessorDeclarationSyntax syntax, bool isAutoPropertyAccessor, BindingDiagnosticBag diagnostics)
        {
            MethodKind methodKind = ((syntax.Kind() == SyntaxKind.GetAccessorDeclaration) ? MethodKind.PropertyGet : MethodKind.PropertySet);
            bool hasBody = syntax.Body != null;
            bool hasExpressionBody = syntax.ExpressionBody != null;
            bool isNullableAnalysisEnabled = containingType.DeclaringCompilation.IsNullableAnalysisEnabledIn(syntax);
            Symbol.CheckForBlockAndExpressionBody(syntax.Body, syntax.ExpressionBody, syntax, diagnostics);
            return new SourcePropertyAccessorSymbol(containingType, property, propertyModifiers, syntax.Keyword.GetLocation(), syntax, hasBody, hasExpressionBody, SyntaxFacts.HasYieldOperations(syntax.Body), syntax.Modifiers, methodKind, syntax.Keyword.IsKind(SyntaxKind.InitKeyword), isAutoPropertyAccessor, isNullableAnalysisEnabled, diagnostics);
        }

        public static SourcePropertyAccessorSymbol CreateAccessorSymbol(NamedTypeSymbol containingType, SourcePropertySymbol property, DeclarationModifiers propertyModifiers, ArrowExpressionClauseSyntax syntax, BindingDiagnosticBag diagnostics)
        {
            bool isNullableAnalysisEnabled = containingType.DeclaringCompilation.IsNullableAnalysisEnabledIn(syntax);
            return new SourcePropertyAccessorSymbol(containingType, property, propertyModifiers, syntax.Expression.GetLocation(), syntax, isNullableAnalysisEnabled, diagnostics);
        }

        public static SourcePropertyAccessorSymbol CreateAccessorSymbol(bool isGetMethod, bool usesInit, NamedTypeSymbol containingType, SynthesizedRecordPropertySymbol property, DeclarationModifiers propertyModifiers, Location location, CSharpSyntaxNode syntax, BindingDiagnosticBag diagnostics)
        {
            MethodKind methodKind = (isGetMethod ? MethodKind.PropertyGet : MethodKind.PropertySet);
            return new SourcePropertyAccessorSymbol(containingType, property, propertyModifiers, location, syntax, hasBody: false, hasExpressionBody: false, isIterator: false, default(SyntaxTokenList), methodKind, usesInit, isAutoPropertyAccessor: true, isNullableAnalysisEnabled: false, diagnostics);
        }

        public static SourcePropertyAccessorSymbol CreateAccessorSymbol(NamedTypeSymbol containingType, SynthesizedRecordEqualityContractProperty property, DeclarationModifiers propertyModifiers, Location location, CSharpSyntaxNode syntax, BindingDiagnosticBag diagnostics)
        {
            return new SynthesizedRecordEqualityContractProperty.GetAccessorSymbol(containingType, property, propertyModifiers, location, syntax, diagnostics);
        }

        private SourcePropertyAccessorSymbol(NamedTypeSymbol containingType, SourcePropertySymbol property, DeclarationModifiers propertyModifiers, Location location, ArrowExpressionClauseSyntax syntax, bool isNullableAnalysisEnabled, BindingDiagnosticBag diagnostics)
            : base(containingType, syntax.GetReference(), location, isIterator: false)
        {
            _property = property;
            _isAutoPropertyAccessor = false;
            _isExpressionBodied = true;
            DeclarationModifiers accessorModifiers = GetAccessorModifiers(propertyModifiers);
            MakeFlags(MethodKind.PropertyGet, accessorModifiers, returnsVoid: false, isExtensionMethod: false, isNullableAnalysisEnabled, property.IsExplicitInterfaceImplementation);
            CheckFeatureAvailabilityAndRuntimeSupport(syntax, location, hasBody: true, diagnostics);
            CheckModifiersForBody(location, diagnostics);
            CSDiagnosticInfo cSDiagnosticInfo = ModifierUtils.CheckAccessibility(DeclarationModifiers, this, property.IsExplicitInterfaceImplementation);
            if (cSDiagnosticInfo != null)
            {
                diagnostics.Add(cSDiagnosticInfo, location);
            }
            CheckModifiers(location, hasBody: true, isAutoPropertyOrExpressionBodied: true, diagnostics);
        }

        protected SourcePropertyAccessorSymbol(NamedTypeSymbol containingType, SourcePropertySymbolBase property, DeclarationModifiers propertyModifiers, Location location, CSharpSyntaxNode syntax, bool hasBody, bool hasExpressionBody, bool isIterator, SyntaxTokenList modifiers, MethodKind methodKind, bool usesInit, bool isAutoPropertyAccessor, bool isNullableAnalysisEnabled, BindingDiagnosticBag diagnostics)
            : base(containingType, syntax.GetReference(), location, isIterator)
        {
            _property = property;
            _isAutoPropertyAccessor = isAutoPropertyAccessor;
            _isExpressionBodied = !hasBody && hasExpressionBody;
            _usesInit = usesInit;
            if (_usesInit)
            {
                Binder.CheckFeatureAvailability(syntax, MessageID.IDS_FeatureInitOnlySetters, diagnostics, location);
            }
            DeclarationModifiers declarationModifiers = MakeModifiers(modifiers, property.IsExplicitInterfaceImplementation, hasBody || hasExpressionBody, location, diagnostics, out var modifierErrors);
            declarationModifiers |= GetAccessorModifiers(propertyModifiers) & ~DeclarationModifiers.AccessibilityMask;
            if ((declarationModifiers & DeclarationModifiers.Private) != 0)
            {
                declarationModifiers &= ~DeclarationModifiers.Virtual;
            }
            MakeFlags(methodKind, declarationModifiers, returnsVoid: false, isExtensionMethod: false, isNullableAnalysisEnabled, property.IsExplicitInterfaceImplementation);
            CheckFeatureAvailabilityAndRuntimeSupport(syntax, location, hasBody || hasExpressionBody || isAutoPropertyAccessor, diagnostics);
            if (hasBody || hasExpressionBody)
            {
                CheckModifiersForBody(location, diagnostics);
            }
            CSDiagnosticInfo cSDiagnosticInfo = ModifierUtils.CheckAccessibility(DeclarationModifiers, this, property.IsExplicitInterfaceImplementation);
            if (cSDiagnosticInfo != null)
            {
                diagnostics.Add(cSDiagnosticInfo, location);
            }
            if (!modifierErrors)
            {
                CheckModifiers(location, hasBody || hasExpressionBody, isAutoPropertyAccessor, diagnostics);
            }
        }

        private static DeclarationModifiers GetAccessorModifiers(DeclarationModifiers propertyModifiers)
        {
            return propertyModifiers & ~(DeclarationModifiers.ReadOnly | DeclarationModifiers.Indexer);
        }

        protected sealed override void MethodChecks(BindingDiagnosticBag diagnostics)
        {
            _lazyParameters = ComputeParameters(diagnostics);
            _lazyReturnType = ComputeReturnType(diagnostics);
            _lazyRefCustomModifiers = ImmutableArray<CustomModifier>.Empty;
            ImmutableArray<MethodSymbol> explicitInterfaceImplementations = ExplicitInterfaceImplementations;
            if (explicitInterfaceImplementations.Length > 0)
            {
                CustomModifierUtils.CopyMethodCustomModifiers(explicitInterfaceImplementations[0], this, out _lazyReturnType, out _lazyRefCustomModifiers, out _lazyParameters, alsoCopyParamsModifier: false);
            }
            else if (IsOverride)
            {
                MethodSymbol overriddenMethod = base.OverriddenMethod;
                if ((object)overriddenMethod != null)
                {
                    CustomModifierUtils.CopyMethodCustomModifiers(overriddenMethod, this, out _lazyReturnType, out _lazyRefCustomModifiers, out _lazyParameters, alsoCopyParamsModifier: true);
                }
            }
            else if (!_lazyReturnType.IsVoidType())
            {
                PropertySymbol property = _property;
                TypeWithAnnotations typeWithAnnotations = property.TypeWithAnnotations;
                _lazyReturnType = _lazyReturnType.WithTypeAndModifiers(CustomModifierUtils.CopyTypeCustomModifiers(typeWithAnnotations.Type, _lazyReturnType.Type, ContainingAssembly), typeWithAnnotations.CustomModifiers);
                _lazyRefCustomModifiers = property.RefCustomModifiers;
            }
        }

        public sealed override ImmutableArray<ImmutableArray<TypeWithAnnotations>> GetTypeParameterConstraintTypes()
        {
            return ImmutableArray<ImmutableArray<TypeWithAnnotations>>.Empty;
        }

        public sealed override ImmutableArray<TypeParameterConstraintKind> GetTypeParameterConstraintKinds()
        {
            return ImmutableArray<TypeParameterConstraintKind>.Empty;
        }

        private TypeWithAnnotations ComputeReturnType(BindingDiagnosticBag diagnostics)
        {
            if (MethodKind == MethodKind.PropertyGet)
            {
                TypeWithAnnotations typeWithAnnotations = _property.TypeWithAnnotations;
                if (typeWithAnnotations.Type.IsStatic)
                {
                    diagnostics.Add(ErrorFacts.GetStaticClassReturnCode(ContainingType.IsInterfaceType()), locations[0], typeWithAnnotations.Type);
                }
                return typeWithAnnotations;
            }
            TypeWithAnnotations result = TypeWithAnnotations.Create(GetBinder().GetSpecialType(SpecialType.System_Void, diagnostics, GetSyntax()));
            if (IsInitOnly)
            {
                ImmutableArray<CustomModifier> customModifiers = ImmutableArray.Create(CSharpCustomModifier.CreateRequired(Binder.GetWellKnownType(DeclaringCompilation, WellKnownType.System_Runtime_CompilerServices_IsExternalInit, diagnostics, locations[0])));
                result = result.WithModifiers(customModifiers);
            }
            return result;
        }

        private Binder GetBinder()
        {
            CSharpSyntaxNode syntax = GetSyntax();
            return DeclaringCompilation.GetBinderFactory(syntax.SyntaxTree).GetBinder(syntax);
        }

        private DeclarationModifiers MakeModifiers(SyntaxTokenList modifiers, bool isExplicitInterfaceImplementation, bool hasBody, Location location, BindingDiagnosticBag diagnostics, out bool modifierErrors)
        {
            DeclarationModifiers declarationModifiers = ((!isExplicitInterfaceImplementation) ? DeclarationModifiers.AccessibilityMask : DeclarationModifiers.None);
            if (ContainingType.IsStructType())
            {
                declarationModifiers |= DeclarationModifiers.ReadOnly;
            }
            DeclarationModifiers defaultInterfaceImplementationModifiers = DeclarationModifiers.None;
            if (ContainingType.IsInterface && !isExplicitInterfaceImplementation)
            {
                defaultInterfaceImplementationModifiers = DeclarationModifiers.AccessibilityMask;
            }
            DeclarationModifiers declarationModifiers2 = ModifierUtils.MakeAndCheckNontypeMemberModifiers(modifiers, DeclarationModifiers.None, declarationModifiers, location, diagnostics, out modifierErrors);
            ModifierUtils.ReportDefaultInterfaceImplementationModifiers(hasBody, declarationModifiers2, defaultInterfaceImplementationModifiers, location, diagnostics);
            return declarationModifiers2;
        }

        private void CheckModifiers(Location location, bool hasBody, bool isAutoPropertyOrExpressionBodied, BindingDiagnosticBag diagnostics)
        {
            Accessibility localAccessibility = LocalAccessibility;
            if (IsAbstract && !ContainingType.IsAbstract && (ContainingType.TypeKind == TypeKind.Class || ContainingType.TypeKind == TypeKind.Submission))
            {
                diagnostics.Add(ErrorCode.ERR_AbstractInConcreteClass, location, this, ContainingType);
            }
            else if (IsVirtual && ContainingType.IsSealed && ContainingType.TypeKind != TypeKind.Struct)
            {
                diagnostics.Add(ErrorCode.ERR_NewVirtualInSealed, location, this, ContainingType);
            }
            else if (!hasBody && !IsExtern && !IsAbstract && !isAutoPropertyOrExpressionBodied)
            {
                diagnostics.Add(ErrorCode.ERR_ConcreteMissingBody, location, this);
            }
            else if (ContainingType.IsSealed && localAccessibility.HasProtected() && !IsOverride)
            {
                diagnostics.Add(AccessCheck.GetProtectedMemberInSealedTypeError(ContainingType), location, this);
            }
            else if (LocalDeclaredReadOnly && _property.HasReadOnlyModifier)
            {
                diagnostics.Add(ErrorCode.ERR_InvalidPropertyReadOnlyMods, location, _property);
            }
            else if (LocalDeclaredReadOnly && IsStatic)
            {
                diagnostics.Add(ErrorCode.ERR_StaticMemberCantBeReadOnly, location, this);
            }
            else if (LocalDeclaredReadOnly && IsInitOnly)
            {
                diagnostics.Add(ErrorCode.ERR_InitCannotBeReadonly, location, _property);
            }
            else if (LocalDeclaredReadOnly && _isAutoPropertyAccessor && MethodKind == MethodKind.PropertySet)
            {
                diagnostics.Add(ErrorCode.ERR_AutoSetterCantBeReadOnly, location, this);
            }
            else if (_usesInit && IsStatic)
            {
                diagnostics.Add(ErrorCode.ERR_BadInitAccessor, location);
            }
        }

        internal static string GetAccessorName(string propertyName, bool getNotSet, bool isWinMdOutput)
        {
            return (getNotSet ? "get_" : (isWinMdOutput ? "put_" : "set_")) + propertyName;
        }

        internal CSharpSyntaxNode GetSyntax()
        {
            return (CSharpSyntaxNode)syntaxReferenceOpt.GetSyntax();
        }

        internal sealed override OneOrMany<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
        {
            CSharpSyntaxNode syntax = GetSyntax();
            SyntaxKind syntaxKind = syntax.Kind();
            if (syntaxKind - 8896 <= SyntaxKind.List || syntaxKind == SyntaxKind.InitAccessorDeclaration)
            {
                return OneOrMany.Create(((AccessorDeclarationSyntax)syntax).AttributeLists);
            }
            return base.GetAttributeDeclarations();
        }

        private ImmutableArray<ParameterSymbol> ComputeParameters(BindingDiagnosticBag diagnostics)
        {
            bool flag = MethodKind == MethodKind.PropertyGet;
            ImmutableArray<ParameterSymbol> parameters = _property.Parameters;
            int num = parameters.Length + ((!flag) ? 1 : 0);
            if (num == 0)
            {
                return ImmutableArray<ParameterSymbol>.Empty;
            }
            ArrayBuilder<ParameterSymbol> instance = ArrayBuilder<ParameterSymbol>.GetInstance(num);
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SourceParameterSymbol sourceParameterSymbol = (SourceParameterSymbol)enumerator.Current;
                instance.Add(new SourceClonedParameterSymbol(sourceParameterSymbol, this, sourceParameterSymbol.Ordinal, suppressOptional: false));
            }
            if (!flag)
            {
                TypeWithAnnotations typeWithAnnotations = _property.TypeWithAnnotations;
                if (typeWithAnnotations.IsStatic)
                {
                    diagnostics.Add(ErrorFacts.GetStaticClassParameterCode(ContainingType.IsInterfaceType()), locations[0], typeWithAnnotations.Type);
                }
                instance.Add(new SynthesizedAccessorValueParameterSymbol(this, typeWithAnnotations, instance.Count));
            }
            return instance.ToImmutableAndFree();
        }

        internal sealed override void AddSynthesizedReturnTypeAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
            base.AddSynthesizedReturnTypeAttributes(moduleBuilder, ref attributes);
            FlowAnalysisAnnotations returnTypeFlowAnalysisAnnotations = ReturnTypeFlowAnalysisAnnotations;
            if ((returnTypeFlowAnalysisAnnotations & FlowAnalysisAnnotations.MaybeNull) != 0)
            {
                Symbol.AddSynthesizedAttribute(ref attributes, new SynthesizedAttributeData(_property.MaybeNullAttributeIfExists));
            }
            if ((returnTypeFlowAnalysisAnnotations & FlowAnalysisAnnotations.NotNull) != 0)
            {
                Symbol.AddSynthesizedAttribute(ref attributes, new SynthesizedAttributeData(_property.NotNullAttributeIfExists));
            }
        }

        internal sealed override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
            base.AddSynthesizedAttributes(moduleBuilder, ref attributes);
            if (_isAutoPropertyAccessor)
            {
                CSharpCompilation declaringCompilation = DeclaringCompilation;
                Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor));
            }
            if (!NotNullMembers.IsEmpty)
            {
                ImmutableArray<SourceAttributeData>.Enumerator enumerator = _property.MemberNotNullAttributeIfExists.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SourceAttributeData current = enumerator.Current;
                    Symbol.AddSynthesizedAttribute(ref attributes, new SynthesizedAttributeData(current));
                }
            }
            if (!NotNullWhenTrueMembers.IsEmpty || !NotNullWhenFalseMembers.IsEmpty)
            {
                ImmutableArray<SourceAttributeData>.Enumerator enumerator = _property.MemberNotNullWhenAttributeIfExists.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SourceAttributeData current2 = enumerator.Current;
                    Symbol.AddSynthesizedAttribute(ref attributes, new SynthesizedAttributeData(current2));
                }
            }
        }
    }
}
