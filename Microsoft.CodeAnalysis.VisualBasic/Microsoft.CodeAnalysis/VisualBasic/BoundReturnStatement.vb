Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundReturnStatement
		Inherits BoundStatement
		Private ReadOnly _ExpressionOpt As BoundExpression

		Private ReadOnly _FunctionLocalOpt As LocalSymbol

		Private ReadOnly _ExitLabelOpt As LabelSymbol

		Public ReadOnly Property ExitLabelOpt As LabelSymbol
			Get
				Return Me._ExitLabelOpt
			End Get
		End Property

		Public ReadOnly Property ExpressionOpt As BoundExpression
			Get
				Return Me._ExpressionOpt
			End Get
		End Property

		Public ReadOnly Property FunctionLocalOpt As LocalSymbol
			Get
				Return Me._FunctionLocalOpt
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal expressionOpt As BoundExpression, ByVal functionLocalOpt As LocalSymbol, ByVal exitLabelOpt As LabelSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.ReturnStatement, syntax, If(hasErrors, True, expressionOpt.NonNullAndHasErrors()))
			Me._ExpressionOpt = expressionOpt
			Me._FunctionLocalOpt = functionLocalOpt
			Me._ExitLabelOpt = exitLabelOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitReturnStatement(Me)
		End Function

		Friend Function IsEndOfMethodReturn() As Boolean
			Return Me.ExitLabelOpt Is Nothing
		End Function

		Public Function Update(ByVal expressionOpt As BoundExpression, ByVal functionLocalOpt As LocalSymbol, ByVal exitLabelOpt As LabelSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundReturnStatement
			Dim boundReturnStatement As Microsoft.CodeAnalysis.VisualBasic.BoundReturnStatement
			If (expressionOpt <> Me.ExpressionOpt OrElse CObj(functionLocalOpt) <> CObj(Me.FunctionLocalOpt) OrElse CObj(exitLabelOpt) <> CObj(Me.ExitLabelOpt)) Then
				Dim boundReturnStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundReturnStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundReturnStatement(MyBase.Syntax, expressionOpt, functionLocalOpt, exitLabelOpt, MyBase.HasErrors)
				boundReturnStatement1.CopyAttributes(Me)
				boundReturnStatement = boundReturnStatement1
			Else
				boundReturnStatement = Me
			End If
			Return boundReturnStatement
		End Function
	End Class
End Namespace