Imports Microsoft.CodeAnalysis
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundYieldStatement
		Inherits BoundStatement
		Private ReadOnly _Expression As BoundExpression

		Public ReadOnly Property Expression As BoundExpression
			Get
				Return Me._Expression
			End Get
		End Property

		Friend Sub New(ByVal syntax As SyntaxNode, ByVal expression As BoundExpression, ByVal hasErrors As Boolean, ByVal returnTypeIsBeingInferred As Boolean)
			MyBase.New(BoundKind.YieldStatement, syntax, If(hasErrors, True, expression.NonNullAndHasErrors()))
			Me._Expression = expression
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal expression As BoundExpression, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.YieldStatement, syntax, If(hasErrors, True, expression.NonNullAndHasErrors()))
			Me._Expression = expression
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitYieldStatement(Me)
		End Function

		Public Function Update(ByVal expression As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundYieldStatement
			Dim boundYieldStatement As Microsoft.CodeAnalysis.VisualBasic.BoundYieldStatement
			If (expression = Me.Expression) Then
				boundYieldStatement = Me
			Else
				Dim boundYieldStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundYieldStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundYieldStatement(MyBase.Syntax, expression, MyBase.HasErrors)
				boundYieldStatement1.CopyAttributes(Me)
				boundYieldStatement = boundYieldStatement1
			End If
			Return boundYieldStatement
		End Function
	End Class
End Namespace