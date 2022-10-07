Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundWhileStatement
		Inherits BoundLoopStatement
		Private ReadOnly _Condition As BoundExpression

		Private ReadOnly _Body As BoundStatement

		Public ReadOnly Property Body As BoundStatement
			Get
				Return Me._Body
			End Get
		End Property

		Public ReadOnly Property Condition As BoundExpression
			Get
				Return Me._Condition
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal condition As BoundExpression, ByVal body As BoundStatement, ByVal continueLabel As LabelSymbol, ByVal exitLabel As LabelSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.WhileStatement, syntax, continueLabel, exitLabel, If(hasErrors OrElse condition.NonNullAndHasErrors(), True, body.NonNullAndHasErrors()))
			Me._Condition = condition
			Me._Body = body
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitWhileStatement(Me)
		End Function

		Public Function Update(ByVal condition As BoundExpression, ByVal body As BoundStatement, ByVal continueLabel As LabelSymbol, ByVal exitLabel As LabelSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundWhileStatement
			Dim boundWhileStatement As Microsoft.CodeAnalysis.VisualBasic.BoundWhileStatement
			If (condition <> Me.Condition OrElse body <> Me.Body OrElse CObj(continueLabel) <> CObj(MyBase.ContinueLabel) OrElse CObj(exitLabel) <> CObj(MyBase.ExitLabel)) Then
				Dim boundWhileStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundWhileStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundWhileStatement(MyBase.Syntax, condition, body, continueLabel, exitLabel, MyBase.HasErrors)
				boundWhileStatement1.CopyAttributes(Me)
				boundWhileStatement = boundWhileStatement1
			Else
				boundWhileStatement = Me
			End If
			Return boundWhileStatement
		End Function
	End Class
End Namespace