using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class InMethodBinder : LocalScopeBinder
    {
        private MultiDictionary<string, ParameterSymbol> _lazyParameterMap;

        private readonly MethodSymbol _methodSymbol;

        private SmallDictionary<string, Symbol> _lazyDefinitionMap;

        private TypeWithAnnotations.Boxed _iteratorElementType;

        internal override uint LocalScopeDepth => 1u;

        protected override bool InExecutableBinder => true;

        internal override Symbol ContainingMemberOrLambda => _methodSymbol;

        internal override bool IsInMethodBody => true;

        internal override bool IsNestedFunctionBinder => _methodSymbol.MethodKind == MethodKind.LocalFunction;

        internal override bool IsDirectlyInIterator => _methodSymbol.IsIterator;

        internal override bool IsIndirectlyInIterator => IsDirectlyInIterator;

        internal override GeneratedLabelSymbol BreakLabel => null;

        internal override GeneratedLabelSymbol ContinueLabel => null;

        public InMethodBinder(MethodSymbol owner, Binder enclosing)
            : base(enclosing, enclosing.Flags & ~BinderFlags.AllClearedAtExecutableCodeBoundary)
        {
            _methodSymbol = owner;
        }

        private static void RecordDefinition<T>(SmallDictionary<string, Symbol> declarationMap, ImmutableArray<T> definitions) where T : Symbol
        {
            ImmutableArray<T>.Enumerator enumerator = definitions.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (!declarationMap.ContainsKey(current.Name))
                {
                    declarationMap.Add(current.Name, current);
                }
            }
        }

        protected override SourceLocalSymbol LookupLocal(SyntaxToken nameToken)
        {
            return null;
        }

        protected override LocalFunctionSymbol LookupLocalFunction(SyntaxToken nameToken)
        {
            return null;
        }

        protected override void ValidateYield(YieldStatementSyntax node, BindingDiagnosticBag diagnostics)
        {
        }

        internal override TypeWithAnnotations GetIteratorElementType()
        {
            RefKind refKind = _methodSymbol.RefKind;
            TypeSymbol returnType = _methodSymbol.ReturnType;
            if (!IsDirectlyInIterator)
            {
                TypeWithAnnotations iteratorElementTypeFromReturnType = GetIteratorElementTypeFromReturnType(base.Compilation, refKind, returnType, null, null);
                if (iteratorElementTypeFromReturnType.IsDefault)
                {
                    return TypeWithAnnotations.Create(CreateErrorType());
                }
                return iteratorElementTypeFromReturnType;
            }
            if (_iteratorElementType == null)
            {
                TypeWithAnnotations value = GetIteratorElementTypeFromReturnType(base.Compilation, refKind, returnType, null, null);
                if (value.IsDefault)
                {
                    value = TypeWithAnnotations.Create(CreateErrorType());
                }
                Interlocked.CompareExchange(ref _iteratorElementType, new TypeWithAnnotations.Boxed(value), null);
            }
            return _iteratorElementType.Value;
        }

        internal static TypeWithAnnotations GetIteratorElementTypeFromReturnType(CSharpCompilation compilation, RefKind refKind, TypeSymbol returnType, Location errorLocation, BindingDiagnosticBag diagnostics)
        {
            if (refKind == RefKind.None && returnType.Kind == SymbolKind.NamedType)
            {
                TypeSymbol originalDefinition = returnType.OriginalDefinition;
                switch (originalDefinition.SpecialType)
                {
                    case SpecialType.System_Collections_IEnumerable:
                    case SpecialType.System_Collections_IEnumerator:
                        {
                            NamedTypeSymbol specialType = compilation.GetSpecialType(SpecialType.System_Object);
                            if (diagnostics != null)
                            {
                                Binder.ReportUseSite(specialType, diagnostics, errorLocation);
                            }
                            return TypeWithAnnotations.Create(specialType);
                        }
                    case SpecialType.System_Collections_Generic_IEnumerable_T:
                    case SpecialType.System_Collections_Generic_IEnumerator_T:
                        return ((NamedTypeSymbol)returnType).TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[0];
                }
                if (TypeSymbol.Equals(originalDefinition, compilation.GetWellKnownType(WellKnownType.System_Collections_Generic_IAsyncEnumerable_T), TypeCompareKind.ConsiderEverything) || TypeSymbol.Equals(originalDefinition, compilation.GetWellKnownType(WellKnownType.System_Collections_Generic_IAsyncEnumerator_T), TypeCompareKind.ConsiderEverything))
                {
                    return ((NamedTypeSymbol)returnType).TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[0];
                }
            }
            return default(TypeWithAnnotations);
        }

        internal static bool IsAsyncStreamInterface(CSharpCompilation compilation, RefKind refKind, TypeSymbol returnType)
        {
            if (refKind == RefKind.None && returnType.Kind == SymbolKind.NamedType)
            {
                TypeSymbol originalDefinition = returnType.OriginalDefinition;
                if (TypeSymbol.Equals(originalDefinition, compilation.GetWellKnownType(WellKnownType.System_Collections_Generic_IAsyncEnumerable_T), TypeCompareKind.ConsiderEverything) || TypeSymbol.Equals(originalDefinition, compilation.GetWellKnownType(WellKnownType.System_Collections_Generic_IAsyncEnumerator_T), TypeCompareKind.ConsiderEverything))
                {
                    return true;
                }
            }
            return false;
        }

        internal override void LookupSymbolsInSingleBinder(LookupResult result, string name, int arity, ConsList<TypeSymbol> basesBeingResolved, LookupOptions options, Binder originalBinder, bool diagnose, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (_methodSymbol.ParameterCount == 0 || (options & LookupOptions.NamespaceAliasesOnly) != 0)
            {
                return;
            }
            MultiDictionary<string, ParameterSymbol> multiDictionary = _lazyParameterMap;
            if (multiDictionary == null)
            {
                ImmutableArray<ParameterSymbol> parameters = _methodSymbol.Parameters;
                multiDictionary = new MultiDictionary<string, ParameterSymbol>(parameters.Length, EqualityComparer<string>.Default);
                ImmutableArray<ParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ParameterSymbol current = enumerator.Current;
                    multiDictionary.Add(current.Name, current);
                }
                _lazyParameterMap = multiDictionary;
            }
            foreach (ParameterSymbol item in multiDictionary[name])
            {
                result.MergeEqual(originalBinder.CheckViability(item, arity, options, null, diagnose, ref useSiteInfo));
            }
        }

        protected override void AddLookupSymbolsInfoInSingleBinder(LookupSymbolsInfo result, LookupOptions options, Binder originalBinder)
        {
            if (!options.CanConsiderMembers())
            {
                return;
            }
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = _methodSymbol.Parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                if (originalBinder.CanAddLookupSymbolInfo(current, options, result, null))
                {
                    result.AddSymbol(current, current.Name, 0);
                }
            }
        }

        private static bool ReportConflictWithParameter(Symbol parameter, Symbol newSymbol, string name, Location newLocation, BindingDiagnosticBag diagnostics)
        {
            SymbolKind kind = parameter.Kind;
            SymbolKind symbolKind = newSymbol?.Kind ?? SymbolKind.Parameter;
            if (symbolKind == SymbolKind.ErrorType)
            {
                return true;
            }
            if (kind == SymbolKind.Parameter)
            {
                switch (symbolKind)
                {
                    case SymbolKind.Local:
                    case SymbolKind.Parameter:
                        diagnostics.Add(ErrorCode.ERR_LocalIllegallyOverrides, newLocation, name);
                        return true;
                    case SymbolKind.Method:
                        if (((MethodSymbol)newSymbol).MethodKind != MethodKind.LocalFunction)
                        {
                            break;
                        }
                        goto case SymbolKind.Local;
                    case SymbolKind.TypeParameter:
                        return false;
                    case SymbolKind.RangeVariable:
                        diagnostics.Add(ErrorCode.ERR_QueryRangeVariableOverrides, newLocation, name);
                        return true;
                }
            }
            if (kind == SymbolKind.TypeParameter)
            {
                switch (symbolKind)
                {
                    case SymbolKind.Local:
                    case SymbolKind.Parameter:
                        diagnostics.Add(ErrorCode.ERR_LocalSameNameAsTypeParam, newLocation, name);
                        return true;
                    case SymbolKind.Method:
                        if (((MethodSymbol)newSymbol).MethodKind != MethodKind.LocalFunction)
                        {
                            break;
                        }
                        goto case SymbolKind.Local;
                    case SymbolKind.TypeParameter:
                        return false;
                    case SymbolKind.RangeVariable:
                        diagnostics.Add(ErrorCode.ERR_QueryRangeVariableSameAsTypeParam, newLocation, name);
                        return true;
                }
            }
            diagnostics.Add(ErrorCode.ERR_InternalError, newLocation);
            return true;
        }

        internal override bool EnsureSingleDefinition(Symbol symbol, string name, Location location, BindingDiagnosticBag diagnostics)
        {
            ImmutableArray<ParameterSymbol> parameters = _methodSymbol.Parameters;
            ImmutableArray<TypeParameterSymbol> typeParameters = _methodSymbol.TypeParameters;
            if (parameters.IsEmpty && typeParameters.IsEmpty)
            {
                return false;
            }
            SmallDictionary<string, Symbol> smallDictionary = _lazyDefinitionMap;
            if (smallDictionary == null)
            {
                smallDictionary = new SmallDictionary<string, Symbol>();
                RecordDefinition(smallDictionary, parameters);
                RecordDefinition(smallDictionary, typeParameters);
                _lazyDefinitionMap = smallDictionary;
            }
            if (smallDictionary.TryGetValue(name, out var value))
            {
                return ReportConflictWithParameter(value, symbol, name, location, diagnostics);
            }
            return false;
        }
    }
}
