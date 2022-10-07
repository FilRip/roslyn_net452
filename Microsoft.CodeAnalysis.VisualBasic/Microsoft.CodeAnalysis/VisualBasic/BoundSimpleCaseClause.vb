Imports Microsoft.CodeAnalysis
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundSimpleCaseClause
		Inherits BoundSingleValueCaseClause
		Public Sub New(ByVal syntax As SyntaxNode, ByVal valueOpt As BoundExpression, ByVal conditionOpt As BoundExpression, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.SimpleCaseClause, syntax, valueOpt, conditionOpt, If(hasErrors OrElse valueOpt.NonNullAndHasErrors(), True, conditionOpt.NonNullAndHasErrors()))
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitSimpleCaseClause(Me)
		End Function

		Public Function Update(ByVal valueOpt As BoundExpression, ByVal conditionOpt As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundSimpleCaseClause
			Dim boundSimpleCaseClause As Microsoft.CodeAnalysis.VisualBasic.BoundSimpleCaseClause
			If (valueOpt <> MyBase.ValueOpt OrElse conditionOpt <> MyBase.ConditionOpt) Then
				Dim boundSimpleCaseClause1 As Microsoft.CodeAnalysis.VisualBasic.BoundSimpleCaseClause = New Microsoft.CodeAnalysis.VisualBasic.BoundSimpleCaseClause(MyBase.Syntax, valueOpt, conditionOpt, MyBase.HasErrors)
				boundSimpleCaseClause1.CopyAttributes(Me)
				boundSimpleCaseClause = boundSimpleCaseClause1
			Else
				boundSimpleCaseClause = Me
			End If
			Return boundSimpleCaseClause
		End Function
	End Class
End Namespace