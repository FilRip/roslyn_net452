Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundSelectStatement
		Inherits BoundStatement
		Private ReadOnly _ExpressionStatement As BoundExpressionStatement

		Private ReadOnly _ExprPlaceholderOpt As BoundRValuePlaceholder

		Private ReadOnly _CaseBlocks As ImmutableArray(Of BoundCaseBlock)

		Private ReadOnly _RecommendSwitchTable As Boolean

		Private ReadOnly _ExitLabel As LabelSymbol

		Public ReadOnly Property CaseBlocks As ImmutableArray(Of BoundCaseBlock)
			Get
				Return Me._CaseBlocks
			End Get
		End Property

		Public ReadOnly Property ExitLabel As LabelSymbol
			Get
				Return Me._ExitLabel
			End Get
		End Property

		Public ReadOnly Property ExpressionStatement As BoundExpressionStatement
			Get
				Return Me._ExpressionStatement
			End Get
		End Property

		Public ReadOnly Property ExprPlaceholderOpt As BoundRValuePlaceholder
			Get
				Return Me._ExprPlaceholderOpt
			End Get
		End Property

		Public ReadOnly Property RecommendSwitchTable As Boolean
			Get
				Return Me._RecommendSwitchTable
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal expressionStatement As BoundExpressionStatement, ByVal exprPlaceholderOpt As BoundRValuePlaceholder, ByVal caseBlocks As ImmutableArray(Of BoundCaseBlock), ByVal recommendSwitchTable As Boolean, ByVal exitLabel As LabelSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.SelectStatement, syntax, If(hasErrors OrElse expressionStatement.NonNullAndHasErrors() OrElse exprPlaceholderOpt.NonNullAndHasErrors(), True, caseBlocks.NonNullAndHasErrors()))
			Me._ExpressionStatement = expressionStatement
			Me._ExprPlaceholderOpt = exprPlaceholderOpt
			Me._CaseBlocks = caseBlocks
			Me._RecommendSwitchTable = recommendSwitchTable
			Me._ExitLabel = exitLabel
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitSelectStatement(Me)
		End Function

		Public Function Update(ByVal expressionStatement As BoundExpressionStatement, ByVal exprPlaceholderOpt As BoundRValuePlaceholder, ByVal caseBlocks As ImmutableArray(Of BoundCaseBlock), ByVal recommendSwitchTable As Boolean, ByVal exitLabel As LabelSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundSelectStatement
			Dim boundSelectStatement As Microsoft.CodeAnalysis.VisualBasic.BoundSelectStatement
			If (expressionStatement <> Me.ExpressionStatement OrElse exprPlaceholderOpt <> Me.ExprPlaceholderOpt OrElse caseBlocks <> Me.CaseBlocks OrElse recommendSwitchTable <> Me.RecommendSwitchTable OrElse CObj(exitLabel) <> CObj(Me.ExitLabel)) Then
				Dim boundSelectStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundSelectStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundSelectStatement(MyBase.Syntax, expressionStatement, exprPlaceholderOpt, caseBlocks, recommendSwitchTable, exitLabel, MyBase.HasErrors)
				boundSelectStatement1.CopyAttributes(Me)
				boundSelectStatement = boundSelectStatement1
			Else
				boundSelectStatement = Me
			End If
			Return boundSelectStatement
		End Function
	End Class
End Namespace