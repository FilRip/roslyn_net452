Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class TopLevelCodeBinder
		Inherits SubOrFunctionBodyBinder
		Public Overrides ReadOnly Property IsInQuery As Boolean
			Get
				Return False
			End Get
		End Property

		Public Sub New(ByVal scriptInitializer As MethodSymbol, ByVal containingBinder As Binder)
			MyBase.New(scriptInitializer, scriptInitializer.Syntax, containingBinder)
		End Sub

		Public Overrides Function GetLocalForFunctionValue() As LocalSymbol
			Return Nothing
		End Function
	End Class
End Namespace