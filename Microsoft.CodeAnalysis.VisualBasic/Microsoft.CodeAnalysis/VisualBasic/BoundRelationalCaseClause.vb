Imports Microsoft.CodeAnalysis
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundRelationalCaseClause
		Inherits BoundSingleValueCaseClause
		Private ReadOnly _OperatorKind As BinaryOperatorKind

		Public ReadOnly Property OperatorKind As BinaryOperatorKind
			Get
				Return Me._OperatorKind
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal operatorKind As BinaryOperatorKind, ByVal valueOpt As BoundExpression, ByVal conditionOpt As BoundExpression, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.RelationalCaseClause, syntax, valueOpt, conditionOpt, If(hasErrors OrElse valueOpt.NonNullAndHasErrors(), True, conditionOpt.NonNullAndHasErrors()))
			Me._OperatorKind = operatorKind
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitRelationalCaseClause(Me)
		End Function

		Public Function Update(ByVal operatorKind As BinaryOperatorKind, ByVal valueOpt As BoundExpression, ByVal conditionOpt As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundRelationalCaseClause
			Dim boundRelationalCaseClause As Microsoft.CodeAnalysis.VisualBasic.BoundRelationalCaseClause
			If (operatorKind <> Me.OperatorKind OrElse valueOpt <> MyBase.ValueOpt OrElse conditionOpt <> MyBase.ConditionOpt) Then
				Dim boundRelationalCaseClause1 As Microsoft.CodeAnalysis.VisualBasic.BoundRelationalCaseClause = New Microsoft.CodeAnalysis.VisualBasic.BoundRelationalCaseClause(MyBase.Syntax, operatorKind, valueOpt, conditionOpt, MyBase.HasErrors)
				boundRelationalCaseClause1.CopyAttributes(Me)
				boundRelationalCaseClause = boundRelationalCaseClause1
			Else
				boundRelationalCaseClause = Me
			End If
			Return boundRelationalCaseClause
		End Function
	End Class
End Namespace