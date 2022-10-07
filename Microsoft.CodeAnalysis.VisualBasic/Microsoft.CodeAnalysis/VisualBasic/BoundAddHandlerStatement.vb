Imports Microsoft.CodeAnalysis
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundAddHandlerStatement
		Inherits BoundAddRemoveHandlerStatement
		Public Sub New(ByVal syntax As SyntaxNode, ByVal eventAccess As BoundExpression, ByVal handler As BoundExpression, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.AddHandlerStatement, syntax, eventAccess, handler, If(hasErrors OrElse eventAccess.NonNullAndHasErrors(), True, handler.NonNullAndHasErrors()))
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitAddHandlerStatement(Me)
		End Function

		Public Function Update(ByVal eventAccess As BoundExpression, ByVal handler As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundAddHandlerStatement
			Dim boundAddHandlerStatement As Microsoft.CodeAnalysis.VisualBasic.BoundAddHandlerStatement
			If (eventAccess <> MyBase.EventAccess OrElse handler <> MyBase.Handler) Then
				Dim boundAddHandlerStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundAddHandlerStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundAddHandlerStatement(MyBase.Syntax, eventAccess, handler, MyBase.HasErrors)
				boundAddHandlerStatement1.CopyAttributes(Me)
				boundAddHandlerStatement = boundAddHandlerStatement1
			Else
				boundAddHandlerStatement = Me
			End If
			Return boundAddHandlerStatement
		End Function
	End Class
End Namespace