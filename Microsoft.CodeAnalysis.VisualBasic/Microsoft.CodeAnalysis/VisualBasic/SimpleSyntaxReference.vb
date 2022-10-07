Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Text
Imports System
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class SimpleSyntaxReference
		Inherits SyntaxReference
		Private ReadOnly _tree As Microsoft.CodeAnalysis.SyntaxTree

		Private ReadOnly _node As SyntaxNode

		Public Overrides ReadOnly Property Span As TextSpan
			Get
				Return Me._node.Span
			End Get
		End Property

		Public Overrides ReadOnly Property SyntaxTree As Microsoft.CodeAnalysis.SyntaxTree
			Get
				Return Me._tree
			End Get
		End Property

		Friend Sub New(ByVal tree As Microsoft.CodeAnalysis.SyntaxTree, ByVal node As SyntaxNode)
			MyBase.New()
			Me._tree = tree
			Me._node = node
		End Sub

		Public Overrides Function GetSyntax(Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SyntaxNode
			Return Me._node
		End Function
	End Class
End Namespace