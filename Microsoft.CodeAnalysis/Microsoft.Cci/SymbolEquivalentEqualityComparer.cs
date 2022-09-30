using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis.Symbols;

#nullable enable

namespace Microsoft.Cci
{
    public sealed class SymbolEquivalentEqualityComparer : IEqualityComparer<IReference?>, IEqualityComparer<INamespace?>
    {
        public static readonly SymbolEquivalentEqualityComparer Instance = new SymbolEquivalentEqualityComparer();

        private SymbolEquivalentEqualityComparer()
        {
        }

        public bool Equals(IReference? x, IReference? y)
        {
            if (x == y)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            ISymbolInternal internalSymbol = x!.GetInternalSymbol();
            ISymbolInternal internalSymbol2 = y!.GetInternalSymbol();
            if (internalSymbol != null && internalSymbol2 != null)
            {
                return internalSymbol.Equals(internalSymbol2);
            }
            return false;
        }

        public int GetHashCode(IReference? obj)
        {
            ISymbolInternal symbolInternal = obj?.GetInternalSymbol();
            if (symbolInternal != null)
            {
                return symbolInternal.GetHashCode();
            }
            if (obj == null)
            {
                return 0;
            }
            return RuntimeHelpers.GetHashCode(obj);
        }

        public bool Equals(INamespace? x, INamespace? y)
        {
            if (x == y)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            INamespaceSymbolInternal internalSymbol = x!.GetInternalSymbol();
            INamespaceSymbolInternal internalSymbol2 = y!.GetInternalSymbol();
            if (internalSymbol != null && internalSymbol2 != null)
            {
                return internalSymbol.Equals(internalSymbol2);
            }
            return false;
        }

        public int GetHashCode(INamespace? obj)
        {
            INamespaceSymbolInternal namespaceSymbolInternal = obj?.GetInternalSymbol();
            if (namespaceSymbolInternal != null)
            {
                return namespaceSymbolInternal.GetHashCode();
            }
            if (obj == null)
            {
                return 0;
            }
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
