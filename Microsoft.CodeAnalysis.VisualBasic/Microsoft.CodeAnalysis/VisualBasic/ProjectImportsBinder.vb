Imports Microsoft.CodeAnalysis
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class ProjectImportsBinder
		Inherits Binder
		Private ReadOnly _tree As Microsoft.CodeAnalysis.SyntaxTree

		Friend Overrides ReadOnly Property SuppressObsoleteDiagnostics As Boolean
			Get
				Return True
			End Get
		End Property

		Public Sub New(ByVal containingBinder As Binder, ByVal tree As Microsoft.CodeAnalysis.SyntaxTree)
			MyBase.New(containingBinder)
			Me._tree = tree
		End Sub

		Public Overrides Function GetSyntaxReference(ByVal node As VisualBasicSyntaxNode) As SyntaxReference
			Return Me._tree.GetReference(node)
		End Function
	End Class
End Namespace