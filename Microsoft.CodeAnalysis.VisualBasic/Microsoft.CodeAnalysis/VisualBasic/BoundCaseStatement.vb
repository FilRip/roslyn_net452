Imports Microsoft.CodeAnalysis
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundCaseStatement
		Inherits BoundStatement
		Private ReadOnly _CaseClauses As ImmutableArray(Of BoundCaseClause)

		Private ReadOnly _ConditionOpt As BoundExpression

		Public ReadOnly Property CaseClauses As ImmutableArray(Of BoundCaseClause)
			Get
				Return Me._CaseClauses
			End Get
		End Property

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Dim boundNodes As ImmutableArray(Of BoundNode) = StaticCast(Of BoundNode).From(Of BoundCaseClause)(Me.CaseClauses)
				Return boundNodes.Add(Me.ConditionOpt)
			End Get
		End Property

		Public ReadOnly Property ConditionOpt As BoundExpression
			Get
				Return Me._ConditionOpt
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal caseClauses As ImmutableArray(Of BoundCaseClause), ByVal conditionOpt As BoundExpression, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.CaseStatement, syntax, If(hasErrors OrElse caseClauses.NonNullAndHasErrors(), True, conditionOpt.NonNullAndHasErrors()))
			Me._CaseClauses = caseClauses
			Me._ConditionOpt = conditionOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitCaseStatement(Me)
		End Function

		Public Function Update(ByVal caseClauses As ImmutableArray(Of BoundCaseClause), ByVal conditionOpt As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundCaseStatement
			Dim boundCaseStatement As Microsoft.CodeAnalysis.VisualBasic.BoundCaseStatement
			If (caseClauses <> Me.CaseClauses OrElse conditionOpt <> Me.ConditionOpt) Then
				Dim boundCaseStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundCaseStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundCaseStatement(MyBase.Syntax, caseClauses, conditionOpt, MyBase.HasErrors)
				boundCaseStatement1.CopyAttributes(Me)
				boundCaseStatement = boundCaseStatement1
			Else
				boundCaseStatement = Me
			End If
			Return boundCaseStatement
		End Function
	End Class
End Namespace