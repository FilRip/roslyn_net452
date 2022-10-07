Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Generic

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit
	Friend NotInheritable Class VisualBasicSymbolChanges
		Inherits SymbolChanges
		Public Sub New(ByVal definitionMap As Microsoft.CodeAnalysis.Emit.DefinitionMap, ByVal edits As IEnumerable(Of SemanticEdit), ByVal isAddedSymbol As Func(Of ISymbol, Boolean))
			MyBase.New(definitionMap, edits, isAddedSymbol)
		End Sub

		Protected Overrides Function GetISymbolInternalOrNull(ByVal symbol As ISymbol) As ISymbolInternal
			Return TryCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbol)
		End Function
	End Class
End Namespace