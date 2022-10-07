Imports Microsoft.CodeAnalysis
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundThrowStatement
		Inherits BoundStatement
		Private ReadOnly _ExpressionOpt As BoundExpression

		Public ReadOnly Property ExpressionOpt As BoundExpression
			Get
				Return Me._ExpressionOpt
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal expressionOpt As BoundExpression, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.ThrowStatement, syntax, If(hasErrors, True, expressionOpt.NonNullAndHasErrors()))
			Me._ExpressionOpt = expressionOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitThrowStatement(Me)
		End Function

		Public Function Update(ByVal expressionOpt As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundThrowStatement
			Dim boundThrowStatement As Microsoft.CodeAnalysis.VisualBasic.BoundThrowStatement
			If (expressionOpt = Me.ExpressionOpt) Then
				boundThrowStatement = Me
			Else
				Dim boundThrowStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundThrowStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundThrowStatement(MyBase.Syntax, expressionOpt, MyBase.HasErrors)
				boundThrowStatement1.CopyAttributes(Me)
				boundThrowStatement = boundThrowStatement1
			End If
			Return boundThrowStatement
		End Function
	End Class
End Namespace