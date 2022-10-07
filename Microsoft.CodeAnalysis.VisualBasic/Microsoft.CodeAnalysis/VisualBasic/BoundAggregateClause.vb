Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundAggregateClause
		Inherits BoundQueryClauseBase
		Private ReadOnly _CapturedGroupOpt As BoundQueryClauseBase

		Private ReadOnly _GroupPlaceholderOpt As BoundRValuePlaceholder

		Private ReadOnly _UnderlyingExpression As BoundExpression

		Public ReadOnly Property CapturedGroupOpt As BoundQueryClauseBase
			Get
				Return Me._CapturedGroupOpt
			End Get
		End Property

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Return ImmutableArray.Create(Of BoundNode)(Me.CapturedGroupOpt, Me.UnderlyingExpression)
			End Get
		End Property

		Public Overrides ReadOnly Property ExpressionSymbol As Symbol
			Get
				Return Me.UnderlyingExpression.ExpressionSymbol
			End Get
		End Property

		Public ReadOnly Property GroupPlaceholderOpt As BoundRValuePlaceholder
			Get
				Return Me._GroupPlaceholderOpt
			End Get
		End Property

		Public Overrides ReadOnly Property ResultKind As LookupResultKind
			Get
				Return Me.UnderlyingExpression.ResultKind
			End Get
		End Property

		Public ReadOnly Property UnderlyingExpression As BoundExpression
			Get
				Return Me._UnderlyingExpression
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal capturedGroupOpt As BoundQueryClauseBase, ByVal groupPlaceholderOpt As BoundRValuePlaceholder, ByVal underlyingExpression As BoundExpression, ByVal rangeVariables As ImmutableArray(Of RangeVariableSymbol), ByVal compoundVariableType As TypeSymbol, ByVal binders As ImmutableArray(Of Binder), ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.AggregateClause, syntax, rangeVariables, compoundVariableType, binders, type, If(hasErrors OrElse capturedGroupOpt.NonNullAndHasErrors() OrElse groupPlaceholderOpt.NonNullAndHasErrors(), True, underlyingExpression.NonNullAndHasErrors()))
			Me._CapturedGroupOpt = capturedGroupOpt
			Me._GroupPlaceholderOpt = groupPlaceholderOpt
			Me._UnderlyingExpression = underlyingExpression
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitAggregateClause(Me)
		End Function

		Public Function Update(ByVal capturedGroupOpt As BoundQueryClauseBase, ByVal groupPlaceholderOpt As BoundRValuePlaceholder, ByVal underlyingExpression As BoundExpression, ByVal rangeVariables As ImmutableArray(Of RangeVariableSymbol), ByVal compoundVariableType As TypeSymbol, ByVal binders As ImmutableArray(Of Binder), ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundAggregateClause
			Dim boundAggregateClause As Microsoft.CodeAnalysis.VisualBasic.BoundAggregateClause
			If (capturedGroupOpt <> Me.CapturedGroupOpt OrElse groupPlaceholderOpt <> Me.GroupPlaceholderOpt OrElse underlyingExpression <> Me.UnderlyingExpression OrElse rangeVariables <> MyBase.RangeVariables OrElse CObj(compoundVariableType) <> CObj(MyBase.CompoundVariableType) OrElse binders <> MyBase.Binders OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundAggregateClause1 As Microsoft.CodeAnalysis.VisualBasic.BoundAggregateClause = New Microsoft.CodeAnalysis.VisualBasic.BoundAggregateClause(MyBase.Syntax, capturedGroupOpt, groupPlaceholderOpt, underlyingExpression, rangeVariables, compoundVariableType, binders, type, MyBase.HasErrors)
				boundAggregateClause1.CopyAttributes(Me)
				boundAggregateClause = boundAggregateClause1
			Else
				boundAggregateClause = Me
			End If
			Return boundAggregateClause
		End Function
	End Class
End Namespace