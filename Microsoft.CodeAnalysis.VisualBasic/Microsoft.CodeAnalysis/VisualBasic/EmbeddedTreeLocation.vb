Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class EmbeddedTreeLocation
		Inherits VBLocation
		Friend ReadOnly _embeddedKind As EmbeddedSymbolKind

		Friend ReadOnly _span As TextSpan

		Friend Overrides ReadOnly Property EmbeddedKind As EmbeddedSymbolKind
			Get
				Return Me._embeddedKind
			End Get
		End Property

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
				Return EmbeddedSymbolManager.GetEmbeddedTree(Me._embeddedKind)
			End Get
		End Property

		Public Sub New(ByVal embeddedKind As EmbeddedSymbolKind, ByVal span As TextSpan)
			MyBase.New()
			Me._embeddedKind = embeddedKind
			Me._span = span
		End Sub

		Public Function Equals(ByVal other As EmbeddedTreeLocation) As Boolean
			Dim flag As Boolean
			If (CObj(Me) <> CObj(other)) Then
				flag = If(other Is Nothing OrElse other.EmbeddedKind <> Me._embeddedKind, False, other._span.Equals(Me._span))
			Else
				flag = True
			End If
			Return flag
		End Function

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Return Me.Equals(TryCast(obj, EmbeddedTreeLocation))
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Hash.Combine(Me._embeddedKind.GetHashCode(), Me._span.GetHashCode())
		End Function
	End Class
End Namespace