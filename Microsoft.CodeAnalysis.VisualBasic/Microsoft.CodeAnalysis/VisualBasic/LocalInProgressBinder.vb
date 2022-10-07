Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class LocalInProgressBinder
		Inherits Binder
		Private ReadOnly _symbols As ConsList(Of LocalSymbol)

		Public Overrides ReadOnly Property ImplicitlyTypedLocalsBeingBound As ConsList(Of LocalSymbol)
			Get
				Return Me._symbols
			End Get
		End Property

		Public Sub New(ByVal containingBinder As Binder, ByVal symbol As LocalSymbol)
			MyBase.New(containingBinder)
			Me._symbols = New ConsList(Of LocalSymbol)(symbol, containingBinder.ImplicitlyTypedLocalsBeingBound)
		End Sub
	End Class
End Namespace