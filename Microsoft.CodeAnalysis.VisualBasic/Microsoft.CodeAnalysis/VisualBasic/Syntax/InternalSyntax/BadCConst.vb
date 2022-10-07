Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend Class BadCConst
		Inherits CConst
		Public Overrides ReadOnly Property SpecialType As Microsoft.CodeAnalysis.SpecialType
			Get
				Return Microsoft.CodeAnalysis.SpecialType.None
			End Get
		End Property

		Public Overrides ReadOnly Property ValueAsObject As Object
			Get
				Return Nothing
			End Get
		End Property

		Public Sub New(ByVal id As ERRID)
			MyBase.New(id, New [Object](-1) {})
		End Sub

		Public Sub New(ByVal id As ERRID, ByVal ParamArray args As Object())
			MyBase.New(id, args)
		End Sub

		Public Overrides Function WithError(ByVal id As ERRID) As CConst
			Throw ExceptionUtilities.Unreachable
		End Function
	End Class
End Namespace