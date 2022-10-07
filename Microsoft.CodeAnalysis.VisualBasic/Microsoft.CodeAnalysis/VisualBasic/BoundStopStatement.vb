Imports Microsoft.CodeAnalysis
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundStopStatement
		Inherits BoundStatement
		Public Sub New(ByVal syntax As SyntaxNode, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.StopStatement, syntax, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode)
			MyBase.New(BoundKind.StopStatement, syntax)
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitStopStatement(Me)
		End Function
	End Class
End Namespace