using System.Collections.Immutable;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Symbols;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class WithClassTypeParametersBinder : WithTypeParametersBinder
    {
        private readonly NamedTypeSymbol _namedType;

        private MultiDictionary<string, TypeParameterSymbol> _lazyTypeParameterMap;

        protected override MultiDictionary<string, TypeParameterSymbol> TypeParameterMap
        {
            get
            {
                if (_lazyTypeParameterMap == null)
                {
                    MultiDictionary<string, TypeParameterSymbol> multiDictionary = new MultiDictionary<string, TypeParameterSymbol>();
                    ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = _namedType.TypeParameters.GetEnumerator();
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

        internal WithClassTypeParametersBinder(NamedTypeSymbol container, Binder next)
            : base(next)
        {
            _namedType = container;
        }

        internal override bool IsAccessibleHelper(Symbol symbol, TypeSymbol accessThroughType, out bool failedThroughTypeCheck, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ConsList<TypeSymbol> basesBeingResolved)
        {
            return IsSymbolAccessibleConditional(symbol, _namedType, accessThroughType, out failedThroughTypeCheck, ref useSiteInfo, basesBeingResolved);
        }

        protected override void AddLookupSymbolsInfoInSingleBinder(LookupSymbolsInfo result, LookupOptions options, Binder originalBinder)
        {
            if (!CanConsiderTypeParameters(options))
            {
                return;
            }
            ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = _namedType.TypeParameters.GetEnumerator();
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
