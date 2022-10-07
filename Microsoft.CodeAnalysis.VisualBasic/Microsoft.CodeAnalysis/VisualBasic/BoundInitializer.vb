Imports Microsoft.CodeAnalysis
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class BoundInitializer
		Inherits BoundStatement
		Protected Sub New(ByVal kind As BoundKind, ByVal syntax As SyntaxNode, ByVal hasErrors As Boolean)
			MyBase.New(kind, syntax, hasErrors)
		End Sub

		Protected Sub New(ByVal kind As BoundKind, ByVal syntax As SyntaxNode)
			MyBase.New(kind, syntax)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.Initializer, syntax, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode)
			MyBase.New(BoundKind.Initializer, syntax)
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitInitializer(Me)
		End Function
	End Class
End Namespace