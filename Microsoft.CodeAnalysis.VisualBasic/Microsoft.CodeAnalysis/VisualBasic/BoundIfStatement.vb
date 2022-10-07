Imports Microsoft.CodeAnalysis
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundIfStatement
		Inherits BoundStatement
		Private ReadOnly _Condition As BoundExpression

		Private ReadOnly _Consequence As BoundStatement

		Private ReadOnly _AlternativeOpt As BoundStatement

		Public ReadOnly Property AlternativeOpt As BoundStatement
			Get
				Return Me._AlternativeOpt
			End Get
		End Property

		Public ReadOnly Property Condition As BoundExpression
			Get
				Return Me._Condition
			End Get
		End Property

		Public ReadOnly Property Consequence As BoundStatement
			Get
				Return Me._Consequence
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal condition As BoundExpression, ByVal consequence As BoundStatement, ByVal alternativeOpt As BoundStatement, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.IfStatement, syntax, If(hasErrors OrElse condition.NonNullAndHasErrors() OrElse consequence.NonNullAndHasErrors(), True, alternativeOpt.NonNullAndHasErrors()))
			Me._Condition = condition
			Me._Consequence = consequence
			Me._AlternativeOpt = alternativeOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitIfStatement(Me)
		End Function

		Public Function Update(ByVal condition As BoundExpression, ByVal consequence As BoundStatement, ByVal alternativeOpt As BoundStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundIfStatement
			Dim boundIfStatement As Microsoft.CodeAnalysis.VisualBasic.BoundIfStatement
			If (condition <> Me.Condition OrElse consequence <> Me.Consequence OrElse alternativeOpt <> Me.AlternativeOpt) Then
				Dim boundIfStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundIfStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundIfStatement(MyBase.Syntax, condition, consequence, alternativeOpt, MyBase.HasErrors)
				boundIfStatement1.CopyAttributes(Me)
				boundIfStatement = boundIfStatement1
			Else
				boundIfStatement = Me
			End If
			Return boundIfStatement
		End Function
	End Class
End Namespace