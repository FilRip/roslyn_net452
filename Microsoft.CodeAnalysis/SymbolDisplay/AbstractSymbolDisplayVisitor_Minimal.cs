// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.SymbolDisplay
{
    public abstract partial class AbstractSymbolDisplayVisitor : SymbolVisitor
    {
        protected abstract bool ShouldRestrictMinimallyQualifyLookupToNamespacesAndTypes();

        protected bool IsMinimizing
        {
            get { return this.semanticModelOpt != null; }
        }

        protected bool NameBoundSuccessfullyToSameSymbol(INamedTypeSymbol symbol)
        {
            ImmutableArray<ISymbol> normalSymbols = ShouldRestrictMinimallyQualifyLookupToNamespacesAndTypes()
                ? semanticModelOpt.LookupNamespacesAndTypes(positionOpt, name: symbol.Name)
                : semanticModelOpt.LookupSymbols(positionOpt, name: symbol.Name);
            ISymbol normalSymbol = SingleSymbolWithArity(normalSymbols, symbol.Arity);

            if (normalSymbol == null)
            {
                return false;
            }

            // Binding normally ended up with the right symbol.  We can definitely use the
            // simplified name.
            if (normalSymbol.Equals(symbol.OriginalDefinition))
            {
                return true;
            }

            // Binding normally failed.  We may be in a "Color Color" situation where 'Color'
            // will bind to the field, but we could still allow simplification here.
            ImmutableArray<ISymbol> typeOnlySymbols = semanticModelOpt.LookupNamespacesAndTypes(positionOpt, name: symbol.Name);
            ISymbol typeOnlySymbol = SingleSymbolWithArity(typeOnlySymbols, symbol.Arity);

            if (typeOnlySymbol == null)
            {
                return false;
            }

            var type1 = GetSymbolType(normalSymbol);
            var type2 = GetSymbolType(typeOnlySymbol);

            return
                type1 != null &&
                type2 != null &&
                type1.Equals(type2) &&
                typeOnlySymbol.Equals(symbol.OriginalDefinition);
        }

        private static ISymbol SingleSymbolWithArity(ImmutableArray<ISymbol> candidates, int desiredArity)
        {
            ISymbol singleSymbol = null;
            foreach (ISymbol candidate in candidates)
            {
                var arity = candidate.Kind switch
                {
                    SymbolKind.NamedType => ((INamedTypeSymbol)candidate).Arity,
                    SymbolKind.Method => ((IMethodSymbol)candidate).Arity,
                    _ => 0,
                };
                if (arity == desiredArity)
                {
                    if (singleSymbol == null)
                    {
                        singleSymbol = candidate;
                    }
                    else
                    {
                        singleSymbol = null;
                        break;
                    }
                }
            }
            return singleSymbol;
        }

        protected static ITypeSymbol GetSymbolType(ISymbol symbol)
        {
            if (symbol is ILocalSymbol localSymbol)
            {
                return localSymbol.Type;
            }

            if (symbol is IFieldSymbol fieldSymbol)
            {
                return fieldSymbol.Type;
            }

            if (symbol is IPropertySymbol propertySymbol)
            {
                return propertySymbol.Type;
            }

            if (symbol is IParameterSymbol parameterSymbol)
            {
                return parameterSymbol.Type;
            }

            if (symbol is IAliasSymbol aliasSymbol)
            {
                return aliasSymbol.Target as ITypeSymbol;
            }

            return symbol as ITypeSymbol;
        }
    }
}
