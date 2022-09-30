using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit
{
	internal sealed class VisualBasicSymbolChanges : SymbolChanges
	{
		public VisualBasicSymbolChanges(DefinitionMap definitionMap, IEnumerable<SemanticEdit> edits, Func<ISymbol, bool> isAddedSymbol)
			: base(definitionMap, edits, isAddedSymbol)
		{
		}

		protected override ISymbolInternal GetISymbolInternalOrNull(ISymbol symbol)
		{
			return symbol as Symbol;
		}
	}
}
