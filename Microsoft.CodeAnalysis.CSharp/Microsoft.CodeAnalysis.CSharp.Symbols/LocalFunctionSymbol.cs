using System.Collections.Immutable;
using System.Threading;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public sealed class LocalFunctionSymbol : SourceMethodSymbolWithAttributes
    {
        private readonly Binder _binder;

        private readonly Symbol _containingSymbol;

        private readonly DeclarationModifiers _declarationModifiers;

        private readonly ImmutableArray<SourceMethodTypeParameterSymbol> _typeParameters;

        private readonly RefKind _refKind;

        private ImmutableArray<ParameterSymbol> _lazyParameters;

        private bool _lazyIsVarArg;

        private ImmutableArray<ImmutableArray<TypeWithAnnotations>> _lazyTypeParameterConstraintTypes;

        private ImmutableArray<TypeParameterConstraintKind> _lazyTypeParameterConstraintKinds;

        private TypeWithAnnotations.Boxed? _lazyReturnType;

        private TypeWithAnnotations.Boxed? _lazyIteratorElementType;

        private readonly BindingDiagnosticBag _declarationDiagnostics;

        internal Binder ScopeBinder { get; }

        internal override Binder ParameterBinder => _binder;

        internal LocalFunctionStatementSyntax Syntax => (LocalFunctionStatementSyntax)syntaxReferenceOpt.GetSyntax();

        public override bool RequiresInstanceReceiver => false;

        public override bool IsVararg
        {
            get
            {
                ComputeParameters();
                return _lazyIsVarArg;
            }
        }

        public override ImmutableArray<ParameterSymbol> Parameters
        {
            get
            {
                ComputeParameters();
                return _lazyParameters;
            }
        }

        public override TypeWithAnnotations ReturnTypeWithAnnotations
        {
            get
            {
                ComputeReturnType();
                return _lazyReturnType!.Value;
            }
        }

        public override RefKind RefKind => _refKind;

        public override bool ReturnsVoid => base.ReturnType.IsVoidType();

        public override int Arity => TypeParameters.Length;

        public override ImmutableArray<TypeWithAnnotations> TypeArgumentsWithAnnotations => GetTypeParametersAsTypeArguments();

        public override ImmutableArray<TypeParameterSymbol> TypeParameters => _typeParameters.Cast<SourceMethodTypeParameterSymbol, TypeParameterSymbol>();

        public override bool IsExtensionMethod
        {
            get
            {
                ParameterSyntax parameterSyntax = Syntax.ParameterList.Parameters.FirstOrDefault();
                if (parameterSyntax != null && !parameterSyntax.IsArgList)
                {
                    return parameterSyntax.Modifiers.Any(SyntaxKind.ThisKeyword);
                }
                return false;
            }
        }

        internal override TypeWithAnnotations IteratorElementTypeWithAnnotations
        {
            get
            {
                return _lazyIteratorElementType?.Value ?? default(TypeWithAnnotations);
            }
            set
            {
                Interlocked.CompareExchange(ref _lazyIteratorElementType, new TypeWithAnnotations.Boxed(value), TypeWithAnnotations.Boxed.Sentinel);
            }
        }

        internal override bool IsIterator => _lazyIteratorElementType != null;

        public override MethodKind MethodKind => MethodKind.LocalFunction;

        public sealed override Symbol ContainingSymbol => _containingSymbol;

        public override string Name => Syntax.Identifier.ValueText ?? "";

        public SyntaxToken NameToken => Syntax.Identifier;

        internal override Binder SignatureBinder => _binder;

        public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => ImmutableArray<MethodSymbol>.Empty;

        public override ImmutableArray<Location> Locations => ImmutableArray.Create(Syntax.Identifier.GetLocation());

        internal override bool GenerateDebugInfo => true;

        public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

        internal override CallingConvention CallingConvention => CallingConvention.Default;

        public override Symbol? AssociatedSymbol => null;

        public override Accessibility DeclaredAccessibility => ModifierUtils.EffectiveAccessibility(_declarationModifiers);

        public override bool IsAsync => (_declarationModifiers & DeclarationModifiers.Async) != 0;

        public override bool IsStatic => (_declarationModifiers & DeclarationModifiers.Static) != 0;

        public override bool IsVirtual => (_declarationModifiers & DeclarationModifiers.Virtual) != 0;

        public override bool IsOverride => (_declarationModifiers & DeclarationModifiers.Override) != 0;

        public override bool IsAbstract => (_declarationModifiers & DeclarationModifiers.Abstract) != 0;

        public override bool IsSealed => (_declarationModifiers & DeclarationModifiers.Sealed) != 0;

        public override bool IsExtern => (_declarationModifiers & DeclarationModifiers.Extern) != 0;

        public bool IsUnsafe => (_declarationModifiers & DeclarationModifiers.Unsafe) != 0;

        internal bool IsExpressionBodied
        {
            get
            {
                LocalFunctionStatementSyntax syntax = Syntax;
                if (syntax != null && syntax.Body == null)
                {
                    return syntax.ExpressionBody != null;
                }
                return false;
            }
        }

        internal override bool IsDeclaredReadOnly => false;

        internal override bool IsInitOnly => false;

        public LocalFunctionSymbol(Binder binder, Symbol containingSymbol, LocalFunctionStatementSyntax syntax)
            : base(syntax.GetReference())
        {
            _containingSymbol = containingSymbol;
            _declarationDiagnostics = new BindingDiagnosticBag();
            _declarationModifiers = DeclarationModifiers.Private | syntax.Modifiers.ToDeclarationModifiers(_declarationDiagnostics.DiagnosticBag);
            if (SyntaxFacts.HasYieldOperations(syntax.Body))
            {
                _lazyIteratorElementType = TypeWithAnnotations.Boxed.Sentinel;
            }
            this.CheckUnsafeModifier(_declarationModifiers, _declarationDiagnostics);
            ScopeBinder = binder;
            binder = binder.WithUnsafeRegionIfNecessary(syntax.Modifiers);
            if (syntax.TypeParameterList != null)
            {
                binder = new WithMethodTypeParametersBinder(this, binder);
                _typeParameters = MakeTypeParameters(_declarationDiagnostics);
            }
            else
            {
                _typeParameters = ImmutableArray<SourceMethodTypeParameterSymbol>.Empty;
                Symbol.ReportErrorIfHasConstraints(syntax.ConstraintClauses, _declarationDiagnostics.DiagnosticBag);
            }
            if (IsExtensionMethod)
            {
                _declarationDiagnostics.Add(ErrorCode.ERR_BadExtensionAgg, Locations[0]);
            }
            SeparatedSyntaxList<ParameterSyntax>.Enumerator enumerator = syntax.ParameterList.Parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSyntax current = enumerator.Current;
                ReportAttributesDisallowed(current.AttributeLists, _declarationDiagnostics);
            }
            if (syntax.ReturnType.Kind() == SyntaxKind.RefType)
            {
                if (((RefTypeSyntax)syntax.ReturnType).ReadOnlyKeyword.Kind() == SyntaxKind.ReadOnlyKeyword)
                {
                    _refKind = RefKind.In;
                }
                else
                {
                    _refKind = RefKind.Ref;
                }
            }
            else
            {
                _refKind = RefKind.None;
            }
            _binder = binder;
        }

        internal void GetDeclarationDiagnostics(BindingDiagnosticBag addTo)
        {
            ImmutableArray<SourceMethodTypeParameterSymbol>.Enumerator enumerator = _typeParameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.ForceComplete(null, default(CancellationToken));
            }
            ComputeParameters();
            ImmutableArray<ParameterSymbol>.Enumerator enumerator2 = _lazyParameters.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                enumerator2.Current.ForceComplete(null, default(CancellationToken));
            }
            ComputeReturnType();
            GetAttributes();
            GetReturnTypeAttributes();
            AsyncMethodChecks(_declarationDiagnostics);
            addTo.AddRange(_declarationDiagnostics, allowMismatchInDependencyAccumulation: true);
            BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance(withDiagnostics: false, addTo.AccumulatesDependencies);
            if (base.IsEntryPointCandidate && !IsGenericMethod && ContainingSymbol is SynthesizedSimpleProgramEntryPointSymbol && DeclaringCompilation.HasEntryPointSignature(this, instance).IsCandidate)
            {
                addTo.Add(ErrorCode.WRN_MainIgnored, Syntax.Identifier.GetLocation(), this);
            }
            addTo.AddRangeAndFree(instance);
        }

        internal override void AddDeclarationDiagnostics(BindingDiagnosticBag diagnostics)
        {
            _declarationDiagnostics.AddRange(diagnostics);
        }

        private void ComputeParameters()
        {
            if (_lazyParameters != null)
            {
                return;
            }
            BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance(_declarationDiagnostics);
            ImmutableArray<ParameterSymbol> immutableArray = ParameterHelpers.MakeParameters(_binder, this, Syntax.ParameterList, out SyntaxToken arglistToken, instance, allowRefOrOut: true, allowThis: true, addRefReadOnlyModifier: false);
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            ParameterHelpers.EnsureIsReadOnlyAttributeExists(declaringCompilation, immutableArray, instance, modifyCompilation: false);
            ParameterHelpers.EnsureNativeIntegerAttributeExists(declaringCompilation, immutableArray, instance, modifyCompilation: false);
            ParameterHelpers.EnsureNullableAttributeExists(declaringCompilation, this, immutableArray, instance, modifyCompilation: false);
            bool flag = arglistToken.Kind() == SyntaxKind.ArgListKeyword;
            if (flag)
            {
                instance.Add(ErrorCode.ERR_IllegalVarArgs, arglistToken.GetLocation());
            }
            lock (_declarationDiagnostics)
            {
                if (_lazyParameters != null)
                {
                    instance.Free();
                    return;
                }
                _declarationDiagnostics.AddRangeAndFree(instance);
                _lazyIsVarArg = flag;
                _lazyParameters = immutableArray;
            }
        }

        internal void ComputeReturnType()
        {
            if (_lazyReturnType != null)
            {
                return;
            }
            BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance(_declarationDiagnostics);
            TypeSyntax returnType = Syntax.ReturnType;
            TypeWithAnnotations value = _binder.BindType(returnType.SkipRef(), instance);
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            if (declaringCompilation != null)
            {
                Location location = returnType.Location;
                if (_refKind == RefKind.In)
                {
                    declaringCompilation.EnsureIsReadOnlyAttributeExists(instance, location, modifyCompilation: false);
                }
                if (value.Type.ContainsNativeInteger())
                {
                    declaringCompilation.EnsureNativeIntegerAttributeExists(instance, location, modifyCompilation: false);
                }
                if (declaringCompilation.ShouldEmitNullableAttributes(this) && value.NeedsNullableAttribute())
                {
                    declaringCompilation.EnsureNullableAttributeExists(instance, location, modifyCompilation: false);
                }
            }
            if (value.IsRestrictedType(ignoreSpanLikeTypes: true))
            {
                instance.Add(ErrorCode.ERR_MethodReturnCantBeRefAny, returnType.Location, value.Type);
            }
            lock (_declarationDiagnostics)
            {
                if (_lazyReturnType != null)
                {
                    instance.Free();
                    return;
                }
                _declarationDiagnostics.AddRangeAndFree(instance);
                Interlocked.CompareExchange(ref _lazyReturnType, new TypeWithAnnotations.Boxed(value), null);
            }
        }

        internal override OneOrMany<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
        {
            return OneOrMany.Create(Syntax.AttributeLists);
        }

        protected override void NoteAttributesComplete(bool forReturnType)
        {
        }

        internal override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
        {
            return false;
        }

        internal override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
        {
            return false;
        }

        internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override bool TryGetThisParameter(out ParameterSymbol? thisParameter)
        {
            thisParameter = null;
            return true;
        }

        private void ReportAttributesDisallowed(SyntaxList<AttributeListSyntax> attributes, BindingDiagnosticBag diagnostics)
        {
            CSDiagnosticInfo featureAvailabilityDiagnosticInfo = MessageID.IDS_FeatureLocalFunctionAttributes.GetFeatureAvailabilityDiagnosticInfo((CSharpParseOptions)syntaxReferenceOpt.SyntaxTree.Options);
            if (featureAvailabilityDiagnosticInfo != null)
            {
                SyntaxList<AttributeListSyntax>.Enumerator enumerator = attributes.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    AttributeListSyntax current = enumerator.Current;
                    diagnostics.Add(featureAvailabilityDiagnosticInfo, current.Location);
                }
            }
        }

        private ImmutableArray<SourceMethodTypeParameterSymbol> MakeTypeParameters(BindingDiagnosticBag diagnostics)
        {
            ArrayBuilder<SourceMethodTypeParameterSymbol> instance = ArrayBuilder<SourceMethodTypeParameterSymbol>.GetInstance();
            SeparatedSyntaxList<TypeParameterSyntax> separatedSyntaxList = Syntax.TypeParameterList?.Parameters ?? default(SeparatedSyntaxList<TypeParameterSyntax>);
            for (int i = 0; i < separatedSyntaxList.Count; i++)
            {
                TypeParameterSyntax typeParameterSyntax = separatedSyntaxList[i];
                if (typeParameterSyntax.VarianceKeyword.Kind() != 0)
                {
                    diagnostics.Add(ErrorCode.ERR_IllegalVarianceSyntax, typeParameterSyntax.VarianceKeyword.GetLocation());
                }
                ReportAttributesDisallowed(typeParameterSyntax.AttributeLists, diagnostics);
                SyntaxToken identifier = typeParameterSyntax.Identifier;
                Location location = identifier.GetLocation();
                string text = identifier.ValueText ?? "";
                ArrayBuilder<SourceMethodTypeParameterSymbol>.Enumerator enumerator = instance.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SourceMethodTypeParameterSymbol current = enumerator.Current;
                    if (text == current.Name)
                    {
                        diagnostics.Add(ErrorCode.ERR_DuplicateTypeParameter, location, text);
                        break;
                    }
                }
                SourceMemberContainerTypeSymbol.ReportTypeNamedRecord(identifier.Text, DeclaringCompilation, diagnostics.DiagnosticBag, location);
                TypeParameterSymbol typeParameterSymbol = ContainingSymbol.FindEnclosingTypeParameter(text);
                if ((object)typeParameterSymbol != null)
                {
                    ErrorCode code = ((typeParameterSymbol.ContainingSymbol.Kind != SymbolKind.Method) ? ErrorCode.WRN_TypeParameterSameAsOuterTypeParameter : ErrorCode.WRN_TypeParameterSameAsOuterMethodTypeParameter);
                    diagnostics.Add(code, location, text, typeParameterSymbol.ContainingSymbol);
                }
                SourceMethodTypeParameterSymbol item = new SourceMethodTypeParameterSymbol(this, text, i, ImmutableArray.Create(location), ImmutableArray.Create(typeParameterSyntax.GetReference()));
                instance.Add(item);
            }
            return instance.ToImmutableAndFree();
        }

        public override ImmutableArray<ImmutableArray<TypeWithAnnotations>> GetTypeParameterConstraintTypes()
        {
            if (_lazyTypeParameterConstraintTypes.IsDefault)
            {
                GetTypeParameterConstraintKinds();
                LocalFunctionStatementSyntax syntax = Syntax;
                BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance(_declarationDiagnostics);
                ImmutableArray<ImmutableArray<TypeWithAnnotations>> lazyTypeParameterConstraintTypes = this.MakeTypeParameterConstraintTypes(_binder, TypeParameters, syntax.TypeParameterList, syntax.ConstraintClauses, instance);
                lock (_declarationDiagnostics)
                {
                    if (_lazyTypeParameterConstraintTypes.IsDefault)
                    {
                        _declarationDiagnostics.AddRange(instance);
                        _lazyTypeParameterConstraintTypes = lazyTypeParameterConstraintTypes;
                    }
                }
                instance.Free();
            }
            return _lazyTypeParameterConstraintTypes;
        }

        public override ImmutableArray<TypeParameterConstraintKind> GetTypeParameterConstraintKinds()
        {
            if (_lazyTypeParameterConstraintKinds.IsDefault)
            {
                LocalFunctionStatementSyntax syntax = Syntax;
                ImmutableArray<TypeParameterConstraintKind> value = this.MakeTypeParameterConstraintKinds(_binder, TypeParameters, syntax.TypeParameterList, syntax.ConstraintClauses);
                ImmutableInterlocked.InterlockedInitialize(ref _lazyTypeParameterConstraintKinds, value);
            }
            return _lazyTypeParameterConstraintKinds;
        }

        internal override bool IsNullableAnalysisEnabled()
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override int GetHashCode()
        {
            return Syntax.GetHashCode();
        }

        public sealed override bool Equals(Symbol symbol, TypeCompareKind compareKind)
        {
            if ((object)this == symbol)
            {
                return true;
            }
            return (symbol as LocalFunctionSymbol)?.Syntax == Syntax;
        }
    }
}
