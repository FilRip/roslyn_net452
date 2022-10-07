Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class SymbolsInProgress(Of T As Symbol)
		Private ReadOnly _symbols As ImmutableSetWithInsertionOrder(Of T)

		Friend ReadOnly Shared Empty As SymbolsInProgress(Of T)

		Shared Sub New()
			SymbolsInProgress(Of T).Empty = New SymbolsInProgress(Of T)(ImmutableSetWithInsertionOrder(Of T).Empty)
		End Sub

		Private Sub New(ByVal fields As ImmutableSetWithInsertionOrder(Of T))
			MyBase.New()
			Me._symbols = fields
		End Sub

		Friend Function Add(ByVal symbol As T) As SymbolsInProgress(Of T)
			Return New SymbolsInProgress(Of T)(Me._symbols.Add(symbol))
		End Function

		Friend Function Contains(ByVal symbol As T) As Boolean
			Return Me._symbols.Contains(symbol)
		End Function
	End Class
End Namespace