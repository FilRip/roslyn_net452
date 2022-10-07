Imports Microsoft.CodeAnalysis
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundRangeCaseClause
		Inherits BoundCaseClause
		Private ReadOnly _LowerBoundOpt As BoundExpression

		Private ReadOnly _UpperBoundOpt As BoundExpression

		Private ReadOnly _LowerBoundConditionOpt As BoundExpression

		Private ReadOnly _UpperBoundConditionOpt As BoundExpression

		Public ReadOnly Property LowerBoundConditionOpt As BoundExpression
			Get
				Return Me._LowerBoundConditionOpt
			End Get
		End Property

		Public ReadOnly Property LowerBoundOpt As BoundExpression
			Get
				Return Me._LowerBoundOpt
			End Get
		End Property

		Public ReadOnly Property UpperBoundConditionOpt As BoundExpression
			Get
				Return Me._UpperBoundConditionOpt
			End Get
		End Property

		Public ReadOnly Property UpperBoundOpt As BoundExpression
			Get
				Return Me._UpperBoundOpt
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal lowerBoundOpt As BoundExpression, ByVal upperBoundOpt As BoundExpression, ByVal lowerBoundConditionOpt As BoundExpression, ByVal upperBoundConditionOpt As BoundExpression, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.RangeCaseClause, syntax, If(hasErrors OrElse lowerBoundOpt.NonNullAndHasErrors() OrElse upperBoundOpt.NonNullAndHasErrors() OrElse lowerBoundConditionOpt.NonNullAndHasErrors(), True, upperBoundConditionOpt.NonNullAndHasErrors()))
			Me._LowerBoundOpt = lowerBoundOpt
			Me._UpperBoundOpt = upperBoundOpt
			Me._LowerBoundConditionOpt = lowerBoundConditionOpt
			Me._UpperBoundConditionOpt = upperBoundConditionOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitRangeCaseClause(Me)
		End Function

		Public Function Update(ByVal lowerBoundOpt As BoundExpression, ByVal upperBoundOpt As BoundExpression, ByVal lowerBoundConditionOpt As BoundExpression, ByVal upperBoundConditionOpt As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundRangeCaseClause
			Dim boundRangeCaseClause As Microsoft.CodeAnalysis.VisualBasic.BoundRangeCaseClause
			If (lowerBoundOpt <> Me.LowerBoundOpt OrElse upperBoundOpt <> Me.UpperBoundOpt OrElse lowerBoundConditionOpt <> Me.LowerBoundConditionOpt OrElse upperBoundConditionOpt <> Me.UpperBoundConditionOpt) Then
				Dim boundRangeCaseClause1 As Microsoft.CodeAnalysis.VisualBasic.BoundRangeCaseClause = New Microsoft.CodeAnalysis.VisualBasic.BoundRangeCaseClause(MyBase.Syntax, lowerBoundOpt, upperBoundOpt, lowerBoundConditionOpt, upperBoundConditionOpt, MyBase.HasErrors)
				boundRangeCaseClause1.CopyAttributes(Me)
				boundRangeCaseClause = boundRangeCaseClause1
			Else
				boundRangeCaseClause = Me
			End If
			Return boundRangeCaseClause
		End Function
	End Class
End Namespace