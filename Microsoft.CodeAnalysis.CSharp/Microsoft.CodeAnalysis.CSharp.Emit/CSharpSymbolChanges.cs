using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Emit
{
    internal sealed class CSharpSymbolChanges : SymbolChanges
    {
        public CSharpSymbolChanges(DefinitionMap definitionMap, IEnumerable<SemanticEdit> edits, Func<ISymbol, bool> isAddedSymbol)
            : base(definitionMap, edits, isAddedSymbol)
        {
        }

        protected override ISymbolInternal? GetISymbolInternalOrNull(ISymbol symbol)
        {
            return (symbol as Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.Symbol)?.UnderlyingSymbol;
        }
    }
}
