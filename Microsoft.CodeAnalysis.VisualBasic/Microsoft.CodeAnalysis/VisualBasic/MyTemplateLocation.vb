Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Text
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class MyTemplateLocation
		Inherits VBLocation
		Private ReadOnly _span As TextSpan

		Private ReadOnly _tree As SyntaxTree

		Public Overrides ReadOnly Property Kind As LocationKind
			Get
				Return LocationKind.None
			End Get
		End Property

		Friend Overrides ReadOnly Property PossiblyEmbeddedOrMySourceSpan As TextSpan
			Get
				Return Me._span
			End Get
		End Property

		Friend Overrides ReadOnly Property PossiblyEmbeddedOrMySourceTree As SyntaxTree
			Get
				Return Me._tree
			End Get
		End Property

		Public Sub New(ByVal tree As SyntaxTree, ByVal span As TextSpan)
			MyBase.New()
			Me._span = span
			Me._tree = tree
		End Sub

		Public Function Equals(ByVal other As MyTemplateLocation) As Boolean
			Dim flag As Boolean
			If (CObj(Me) <> CObj(other)) Then
				flag = If(other Is Nothing OrElse Me._tree <> other._tree, False, other._span.Equals(Me._span))
			Else
				flag = True
			End If
			Return flag
		End Function

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Return Me.Equals(TryCast(obj, MyTemplateLocation))
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Me._span.GetHashCode()
		End Function
	End Class
End Namespace