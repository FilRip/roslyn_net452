Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundQueryClause
		Inherits BoundQueryClauseBase
		Private ReadOnly _UnderlyingExpression As BoundExpression

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Return ImmutableArray.Create(Of BoundNode)(Me.UnderlyingExpression)
			End Get
		End Property

		Public Overrides ReadOnly Property ExpressionSymbol As Symbol
			Get
				Return Me.UnderlyingExpression.ExpressionSymbol
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

		Public Sub New(ByVal syntax As SyntaxNode, ByVal underlyingExpression As BoundExpression, ByVal rangeVariables As ImmutableArray(Of RangeVariableSymbol), ByVal compoundVariableType As TypeSymbol, ByVal binders As ImmutableArray(Of Binder), ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.QueryClause, syntax, rangeVariables, compoundVariableType, binders, type, If(hasErrors, True, underlyingExpression.NonNullAndHasErrors()))
			Me._UnderlyingExpression = underlyingExpression
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitQueryClause(Me)
		End Function

		Public Function Update(ByVal underlyingExpression As BoundExpression, ByVal rangeVariables As ImmutableArray(Of RangeVariableSymbol), ByVal compoundVariableType As TypeSymbol, ByVal binders As ImmutableArray(Of Binder), ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundQueryClause
			Dim boundQueryClause As Microsoft.CodeAnalysis.VisualBasic.BoundQueryClause
			If (underlyingExpression <> Me.UnderlyingExpression OrElse rangeVariables <> MyBase.RangeVariables OrElse CObj(compoundVariableType) <> CObj(MyBase.CompoundVariableType) OrElse binders <> MyBase.Binders OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundQueryClause1 As Microsoft.CodeAnalysis.VisualBasic.BoundQueryClause = New Microsoft.CodeAnalysis.VisualBasic.BoundQueryClause(MyBase.Syntax, underlyingExpression, rangeVariables, compoundVariableType, binders, type, MyBase.HasErrors)
				boundQueryClause1.CopyAttributes(Me)
				boundQueryClause = boundQueryClause1
			Else
				boundQueryClause = Me
			End If
			Return boundQueryClause
		End Function
	End Class
End Namespace