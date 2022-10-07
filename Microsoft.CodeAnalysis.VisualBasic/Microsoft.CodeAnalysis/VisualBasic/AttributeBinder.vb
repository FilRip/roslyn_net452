Imports Microsoft.CodeAnalysis
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class AttributeBinder
		Inherits Binder
		Private ReadOnly _root As VisualBasicSyntaxNode

		Friend Overrides ReadOnly Property IsDefaultInstancePropertyAllowed As Boolean
			Get
				Return False
			End Get
		End Property

		Friend ReadOnly Property Root As VisualBasicSyntaxNode
			Get
				Return Me._root
			End Get
		End Property

		Public Sub New(ByVal containingBinder As Binder, ByVal tree As Microsoft.CodeAnalysis.SyntaxTree, Optional ByVal node As VisualBasicSyntaxNode = Nothing)
			MyBase.New(containingBinder, tree)
			Me._root = node
		End Sub

		Public Overrides Function GetBinder(ByVal node As SyntaxNode) As Binder
			Return Nothing
		End Function
	End Class
End Namespace