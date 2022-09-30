using System.Collections.Generic;
using System.Threading;

using Microsoft.CodeAnalysis.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public class ModuleCompilationState<TNamedTypeSymbol, TMethodSymbol> : CommonModuleCompilationState where TNamedTypeSymbol : class, INamedTypeSymbolInternal where TMethodSymbol : class, IMethodSymbolInternal
    {
        private Dictionary<TMethodSymbol, TNamedTypeSymbol>? _lazyStateMachineTypes;

        public void SetStateMachineType(TMethodSymbol method, TNamedTypeSymbol stateMachineClass)
        {
            if (_lazyStateMachineTypes == null)
            {
                Interlocked.CompareExchange(ref _lazyStateMachineTypes, new Dictionary<TMethodSymbol, TNamedTypeSymbol>(), null);
            }
            lock (_lazyStateMachineTypes)
            {
                _lazyStateMachineTypes!.Add(method, stateMachineClass);
            }
        }

        public bool TryGetStateMachineType(TMethodSymbol method, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out TNamedTypeSymbol? stateMachineType)
        {
            stateMachineType = null;
            if (_lazyStateMachineTypes != null)
            {
                return _lazyStateMachineTypes!.TryGetValue(method, out stateMachineType);
            }
            return false;
        }
    }
}
