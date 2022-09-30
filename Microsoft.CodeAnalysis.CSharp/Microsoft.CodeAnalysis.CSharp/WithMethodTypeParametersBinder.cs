using System.Collections.Immutable;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Symbols;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class WithMethodTypeParametersBinder : WithTypeParametersBinder
    {
        private readonly MethodSymbol _methodSymbol;

        private MultiDictionary<string, TypeParameterSymbol> _lazyTypeParameterMap;

        protected override bool InExecutableBinder => false;

        internal override Symbol ContainingMemberOrLambda => _methodSymbol;

        protected override MultiDictionary<string, TypeParameterSymbol> TypeParameterMap
        {
            get
            {
                if (_lazyTypeParameterMap == null)
                {
                    MultiDictionary<string, TypeParameterSymbol> multiDictionary = new MultiDictionary<string, TypeParameterSymbol>();
                    ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = _methodSymbol.TypeParameters.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        TypeParameterSymbol current = enumerator.Current;
                        multiDictionary.Add(current.Name, current);
                    }
                    Interlocked.CompareExchange(ref _lazyTypeParameterMap, multiDictionary, null);
                }
                return _lazyTypeParameterMap;
            }
        }

        protected override LookupOptions LookupMask => LookupOptions.NamespaceAliasesOnly | LookupOptions.MustNotBeMethodTypeParameter;

        internal WithMethodTypeParametersBinder(MethodSymbol methodSymbol, Binder next)
            : base(next)
        {
            _methodSymbol = methodSymbol;
        }

        protected override void AddLookupSymbolsInfoInSingleBinder(LookupSymbolsInfo result, LookupOptions options, Binder originalBinder)
        {
            if (!CanConsiderTypeParameters(options))
            {
                return;
            }
            ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = _methodSymbol.TypeParameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TypeParameterSymbol current = enumerator.Current;
                if (originalBinder.CanAddLookupSymbolInfo(current, options, result, null))
                {
                    result.AddSymbol(current, current.Name, 0);
                }
            }
        }
    }
}
