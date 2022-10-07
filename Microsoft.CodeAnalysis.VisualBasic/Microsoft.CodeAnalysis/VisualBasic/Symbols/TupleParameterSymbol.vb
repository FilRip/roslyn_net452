Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class TupleParameterSymbol
		Inherits WrappedParameterSymbol
		Private ReadOnly _container As Symbol

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._container
			End Get
		End Property

		Public Sub New(ByVal container As Symbol, ByVal underlyingParameter As ParameterSymbol)
			MyBase.New(underlyingParameter)
			Me._container = container
		End Sub

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Return Me.Equals(TryCast(obj, TupleParameterSymbol))
		End Function

		Public Function Equals(ByVal other As TupleParameterSymbol) As Boolean
			If (CObj(other) = CObj(Me)) Then
				Return True
			End If
			If (other Is Nothing OrElse Not (Me._container = other._container)) Then
				Return False
			End If
			Return Me._underlyingParameter = other._underlyingParameter
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Me._underlyingParameter.GetHashCode()
		End Function
	End Class
End Namespace