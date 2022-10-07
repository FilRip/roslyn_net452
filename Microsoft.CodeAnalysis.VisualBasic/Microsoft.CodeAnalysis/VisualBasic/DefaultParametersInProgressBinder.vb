Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class DefaultParametersInProgressBinder
		Inherits SymbolsInProgressBinder(Of ParameterSymbol)
		Friend Overrides ReadOnly Property DefaultParametersInProgress As SymbolsInProgress(Of ParameterSymbol)
			Get
				Return Me.inProgress
			End Get
		End Property

		Friend Sub New(ByVal inProgress As SymbolsInProgress(Of ParameterSymbol), ByVal [next] As Binder)
			MyBase.New(inProgress, [next])
		End Sub
	End Class
End Namespace