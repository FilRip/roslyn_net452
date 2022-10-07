Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundQueryExpression
		Inherits BoundExpression
		Private ReadOnly _LastOperator As BoundQueryClauseBase

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Return ImmutableArray.Create(Of BoundNode)(Me.LastOperator)
			End Get
		End Property

		Public ReadOnly Property LastOperator As BoundQueryClauseBase
			Get
				Return Me._LastOperator
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal lastOperator As BoundQueryClauseBase, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.QueryExpression, syntax, type, If(hasErrors, True, lastOperator.NonNullAndHasErrors()))
			Me._LastOperator = lastOperator
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitQueryExpression(Me)
		End Function

		Public Function Update(ByVal lastOperator As BoundQueryClauseBase, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundQueryExpression
			Dim boundQueryExpression As Microsoft.CodeAnalysis.VisualBasic.BoundQueryExpression
			If (lastOperator <> Me.LastOperator OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundQueryExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundQueryExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundQueryExpression(MyBase.Syntax, lastOperator, type, MyBase.HasErrors)
				boundQueryExpression1.CopyAttributes(Me)
				boundQueryExpression = boundQueryExpression1
			Else
				boundQueryExpression = Me
			End If
			Return boundQueryExpression
		End Function
	End Class
End Namespace