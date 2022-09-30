using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.CodeAnalysis
{
    public static class ModifierInfoExtensions
    {
        public static bool AnyRequired<TypeSymbol>(this ImmutableArray<ModifierInfo<TypeSymbol>> modifiers) where TypeSymbol : class
        {
            if (!modifiers.IsDefaultOrEmpty)
            {
                return modifiers.Any((ModifierInfo<TypeSymbol> m) => !m.IsOptional);
            }
            return false;
        }
    }
}
