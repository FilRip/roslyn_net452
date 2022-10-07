Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend Class CConst(Of T)
		Inherits CConst
		Private ReadOnly _specialType As Microsoft.CodeAnalysis.SpecialType

		Private ReadOnly _value As T

		Public Overrides ReadOnly Property SpecialType As Microsoft.CodeAnalysis.SpecialType
			Get
				Return Me._specialType
			End Get
		End Property

		Public ReadOnly Property Value As T
			Get
				Return Me._value
			End Get
		End Property

		Public Overrides ReadOnly Property ValueAsObject As Object
			Get
				Return Me._value
			End Get
		End Property

		Friend Sub New(ByVal value As T, ByVal specialType As Microsoft.CodeAnalysis.SpecialType)
			MyBase.New()
			Me._value = value
			Me._specialType = specialType
		End Sub

		Private Sub New(ByVal value As T, ByVal specialType As Microsoft.CodeAnalysis.SpecialType, ByVal id As ERRID)
			MyBase.New(id, New [Object](-1) {})
			Me._value = value
			Me._specialType = specialType
		End Sub

		Public Overrides Function WithError(ByVal id As ERRID) As CConst
			Return New CConst(Of T)(Me._value, Me._specialType, id)
		End Function
	End Class
End Namespace