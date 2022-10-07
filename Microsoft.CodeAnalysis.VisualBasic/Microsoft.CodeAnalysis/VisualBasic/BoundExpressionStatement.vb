Imports Microsoft.CodeAnalysis
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundExpressionStatement
		Inherits BoundStatement
		Private ReadOnly _Expression As BoundExpression

		Public ReadOnly Property Expression As BoundExpression
			Get
				Return Me._Expression
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal expression As BoundExpression, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.ExpressionStatement, syntax, If(hasErrors, True, expression.NonNullAndHasErrors()))
			Me._Expression = expression
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitExpressionStatement(Me)
		End Function

		Public Function Update(ByVal expression As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement
			Dim boundExpressionStatement As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement
			If (expression = Me.Expression) Then
				boundExpressionStatement = Me
			Else
				Dim boundExpressionStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement(MyBase.Syntax, expression, MyBase.HasErrors)
				boundExpressionStatement1.CopyAttributes(Me)
				boundExpressionStatement = boundExpressionStatement1
			End If
			Return boundExpressionStatement
		End Function
	End Class
End Namespace