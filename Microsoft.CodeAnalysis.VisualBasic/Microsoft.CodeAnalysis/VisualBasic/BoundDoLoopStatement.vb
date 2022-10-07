Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundDoLoopStatement
		Inherits BoundLoopStatement
		Private ReadOnly _TopConditionOpt As BoundExpression

		Private ReadOnly _BottomConditionOpt As BoundExpression

		Private ReadOnly _TopConditionIsUntil As Boolean

		Private ReadOnly _BottomConditionIsUntil As Boolean

		Private ReadOnly _Body As BoundStatement

		Public ReadOnly Property Body As BoundStatement
			Get
				Return Me._Body
			End Get
		End Property

		Public ReadOnly Property BottomConditionIsUntil As Boolean
			Get
				Return Me._BottomConditionIsUntil
			End Get
		End Property

		Public ReadOnly Property BottomConditionOpt As BoundExpression
			Get
				Return Me._BottomConditionOpt
			End Get
		End Property

		Public ReadOnly Property ConditionIsTop As Boolean
			Get
				Return Me.TopConditionOpt IsNot Nothing
			End Get
		End Property

		Public ReadOnly Property ConditionIsUntil As Boolean
			Get
				Dim flag As Boolean
				flag = If(Me.TopConditionOpt Is Nothing, Me.BottomConditionIsUntil, Me.TopConditionIsUntil)
				Return flag
			End Get
		End Property

		Public ReadOnly Property ConditionOpt As BoundExpression
			Get
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				boundExpression = If(Me.TopConditionOpt Is Nothing, Me.BottomConditionOpt, Me.TopConditionOpt)
				Return boundExpression
			End Get
		End Property

		Public ReadOnly Property TopConditionIsUntil As Boolean
			Get
				Return Me._TopConditionIsUntil
			End Get
		End Property

		Public ReadOnly Property TopConditionOpt As BoundExpression
			Get
				Return Me._TopConditionOpt
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal topConditionOpt As BoundExpression, ByVal bottomConditionOpt As BoundExpression, ByVal topConditionIsUntil As Boolean, ByVal bottomConditionIsUntil As Boolean, ByVal body As BoundStatement, ByVal continueLabel As LabelSymbol, ByVal exitLabel As LabelSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.DoLoopStatement, syntax, continueLabel, exitLabel, If(hasErrors OrElse topConditionOpt.NonNullAndHasErrors() OrElse bottomConditionOpt.NonNullAndHasErrors(), True, body.NonNullAndHasErrors()))
			Me._TopConditionOpt = topConditionOpt
			Me._BottomConditionOpt = bottomConditionOpt
			Me._TopConditionIsUntil = topConditionIsUntil
			Me._BottomConditionIsUntil = bottomConditionIsUntil
			Me._Body = body
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitDoLoopStatement(Me)
		End Function

		Public Function Update(ByVal topConditionOpt As BoundExpression, ByVal bottomConditionOpt As BoundExpression, ByVal topConditionIsUntil As Boolean, ByVal bottomConditionIsUntil As Boolean, ByVal body As BoundStatement, ByVal continueLabel As LabelSymbol, ByVal exitLabel As LabelSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundDoLoopStatement
			Dim boundDoLoopStatement As Microsoft.CodeAnalysis.VisualBasic.BoundDoLoopStatement
			If (topConditionOpt <> Me.TopConditionOpt OrElse bottomConditionOpt <> Me.BottomConditionOpt OrElse topConditionIsUntil <> Me.TopConditionIsUntil OrElse bottomConditionIsUntil <> Me.BottomConditionIsUntil OrElse body <> Me.Body OrElse CObj(continueLabel) <> CObj(MyBase.ContinueLabel) OrElse CObj(exitLabel) <> CObj(MyBase.ExitLabel)) Then
				Dim boundDoLoopStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundDoLoopStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundDoLoopStatement(MyBase.Syntax, topConditionOpt, bottomConditionOpt, topConditionIsUntil, bottomConditionIsUntil, body, continueLabel, exitLabel, MyBase.HasErrors)
				boundDoLoopStatement1.CopyAttributes(Me)
				boundDoLoopStatement = boundDoLoopStatement1
			Else
				boundDoLoopStatement = Me
			End If
			Return boundDoLoopStatement
		End Function
	End Class
End Namespace