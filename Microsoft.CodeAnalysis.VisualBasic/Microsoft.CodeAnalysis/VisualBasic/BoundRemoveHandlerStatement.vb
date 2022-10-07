Imports Microsoft.CodeAnalysis
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundRemoveHandlerStatement
		Inherits BoundAddRemoveHandlerStatement
		Public Sub New(ByVal syntax As SyntaxNode, ByVal eventAccess As BoundExpression, ByVal handler As BoundExpression, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.RemoveHandlerStatement, syntax, eventAccess, handler, If(hasErrors OrElse eventAccess.NonNullAndHasErrors(), True, handler.NonNullAndHasErrors()))
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitRemoveHandlerStatement(Me)
		End Function

		Public Function Update(ByVal eventAccess As BoundExpression, ByVal handler As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundRemoveHandlerStatement
			Dim boundRemoveHandlerStatement As Microsoft.CodeAnalysis.VisualBasic.BoundRemoveHandlerStatement
			If (eventAccess <> MyBase.EventAccess OrElse handler <> MyBase.Handler) Then
				Dim boundRemoveHandlerStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundRemoveHandlerStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundRemoveHandlerStatement(MyBase.Syntax, eventAccess, handler, MyBase.HasErrors)
				boundRemoveHandlerStatement1.CopyAttributes(Me)
				boundRemoveHandlerStatement = boundRemoveHandlerStatement1
			Else
				boundRemoveHandlerStatement = Me
			End If
			Return boundRemoveHandlerStatement
		End Function
	End Class
End Namespace