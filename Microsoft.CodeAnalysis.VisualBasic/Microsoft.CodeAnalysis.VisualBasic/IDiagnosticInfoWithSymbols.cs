using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal interface IDiagnosticInfoWithSymbols
	{
		void GetAssociatedSymbols(ArrayBuilder<Symbol> builder);
	}
}
