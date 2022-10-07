Imports Microsoft.CodeAnalysis
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class BoundCaseClause
		Inherits BoundNode
		Protected Sub New(ByVal kind As BoundKind, ByVal syntax As SyntaxNode, ByVal hasErrors As Boolean)
			MyBase.New(kind, syntax, hasErrors)
		End Sub

		Protected Sub New(ByVal kind As BoundKind, ByVal syntax As SyntaxNode)
			MyBase.New(kind, syntax)
		End Sub
	End Class
End Namespace