Imports Microsoft.CodeAnalysis
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundEndStatement
		Inherits BoundStatement
		Public Sub New(ByVal syntax As SyntaxNode, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.EndStatement, syntax, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode)
			MyBase.New(BoundKind.EndStatement, syntax)
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitEndStatement(Me)
		End Function
	End Class
End Namespace