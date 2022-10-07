Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class SymbolsInProgressBinder(Of T As Symbol)
		Inherits Binder
		Protected ReadOnly inProgress As SymbolsInProgress(Of T)

		Protected Sub New(ByVal inProgress As SymbolsInProgress(Of T), ByVal [next] As Binder)
			MyBase.New([next])
			Me.inProgress = inProgress
		End Sub
	End Class
End Namespace