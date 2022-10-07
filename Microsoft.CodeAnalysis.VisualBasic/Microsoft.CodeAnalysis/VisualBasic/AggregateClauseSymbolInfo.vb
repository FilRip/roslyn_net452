Imports Microsoft.CodeAnalysis
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public Structure AggregateClauseSymbolInfo
		Public ReadOnly Property Select1 As SymbolInfo

		Public ReadOnly Property Select2 As SymbolInfo

		Friend Sub New(ByVal select1 As SymbolInfo)
			Me = New AggregateClauseSymbolInfo()
			Me.Select1 = select1
			Me.Select2 = SymbolInfo.None
		End Sub

		Friend Sub New(ByVal select1 As SymbolInfo, ByVal select2 As SymbolInfo)
			Me = New AggregateClauseSymbolInfo()
			Me.Select1 = select1
			Me.Select2 = select2
		End Sub
	End Structure
End Namespace