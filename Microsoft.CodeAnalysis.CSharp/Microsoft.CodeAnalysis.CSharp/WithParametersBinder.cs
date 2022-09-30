using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class WithParametersBinder : Binder
    {
        private readonly ImmutableArray<ParameterSymbol> _parameters;

        internal WithParametersBinder(ImmutableArray<ParameterSymbol> parameters, Binder next)
            : base(next)
        {
            _parameters = parameters;
        }

        protected override void AddLookupSymbolsInfoInSingleBinder(LookupSymbolsInfo result, LookupOptions options, Binder originalBinder)
        {
            if (!options.CanConsiderLocals())
            {
                return;
            }
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = _parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                if (originalBinder.CanAddLookupSymbolInfo(current, options, result, null))
                {
                    result.AddSymbol(current, current.Name, 0);
                }
            }
        }

        internal override void LookupSymbolsInSingleBinder(LookupResult result, string name, int arity, ConsList<TypeSymbol> basesBeingResolved, LookupOptions options, Binder originalBinder, bool diagnose, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if ((options & (LookupOptions.NamespaceAliasesOnly | LookupOptions.MustBeInvocableIfMember)) != 0)
            {
                return;
            }
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = _parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                if (current.Name == name)
                {
                    result.MergeEqual(originalBinder.CheckViability(current, arity, options, null, diagnose, ref useSiteInfo));
                }
            }
        }
    }
}
