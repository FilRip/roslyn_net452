Imports Microsoft.CodeAnalysis
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public Structure CollectionRangeVariableSymbolInfo
		Friend ReadOnly Shared None As CollectionRangeVariableSymbolInfo

		Public ReadOnly Property AsClauseConversion As SymbolInfo

		Public ReadOnly Property SelectMany As SymbolInfo

		Public ReadOnly Property ToQueryableCollectionConversion As SymbolInfo

		Shared Sub New()
			CollectionRangeVariableSymbolInfo.None = New CollectionRangeVariableSymbolInfo(SymbolInfo.None, SymbolInfo.None, SymbolInfo.None)
		End Sub

		Friend Sub New(ByVal toQueryableCollectionConversion As SymbolInfo, ByVal asClauseConversion As SymbolInfo, ByVal selectMany As SymbolInfo)
			Me = New CollectionRangeVariableSymbolInfo()
			Me.ToQueryableCollectionConversion = toQueryableCollectionConversion
			Me.AsClauseConversion = asClauseConversion
			Me.SelectMany = selectMany
		End Sub
	End Structure
End Namespace