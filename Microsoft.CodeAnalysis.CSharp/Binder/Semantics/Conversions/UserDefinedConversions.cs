// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp
{
    public partial class ConversionsBase
    {
        private static TypeSymbol GetUnderlyingEffectiveType(TypeSymbol type, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            // Spec 6.4.4: User-defined implicit conversions 
            // Spec 6.4.5: User-defined explicit conversions 
            // 
            // Determine the types S0 and T0. 
            //   * If S or T are nullable types, let Su and Tu be their underlying types, otherwise let Su and Tu be S and T, respectively. 
            //   * If Su or Tu are type parameters, S0 and T0 are their effective base types, otherwise S0 and T0 are equal to Su and Tu, respectively.

            if (type is not null)
            {
                type = type.StrippedType();

                if (type.IsTypeParameter())
                {
                    type = ((TypeParameterSymbol)type).EffectiveBaseClass(ref useSiteInfo);
                }
            }

            return type;
        }

        public static void AddTypesParticipatingInUserDefinedConversion(ArrayBuilder<NamedTypeSymbol> result, TypeSymbol type, bool includeBaseTypes, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (type is null)
            {
                return;
            }

            // CONSIDER: These sets are usually small; if they are large then this is an O(n^2)
            // CONSIDER: algorithm. We could use a hash table instead to build up the set.


            // optimization:
            bool excludeExisting = result.Count > 0;

            if (type.IsClassType() || type.IsStructType())
            {
                var namedType = (NamedTypeSymbol)type;
                if (!excludeExisting || !HasIdentityConversionToAny(namedType, result))
                {
                    result.Add(namedType);
                }
            }

            if (!includeBaseTypes)
            {
                return;
            }

            NamedTypeSymbol t = type.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
            while (t is not null)
            {
                if (!excludeExisting || !HasIdentityConversionToAny(t, result))
                {
                    result.Add(t);
                }

                t = t.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
            }
        }
    }
}
