Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundQueryLambda
		Inherits BoundExpression
		Private ReadOnly _LambdaSymbol As SynthesizedLambdaSymbol

		Private ReadOnly _RangeVariables As ImmutableArray(Of RangeVariableSymbol)

		Private ReadOnly _Expression As BoundExpression

		Private ReadOnly _ExprIsOperandOfConditionalBranch As Boolean

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Return ImmutableArray.Create(Of BoundNode)(Me.Expression)
			End Get
		End Property

		Public ReadOnly Property Expression As BoundExpression
			Get
				Return Me._Expression
			End Get
		End Property

		Public ReadOnly Property ExprIsOperandOfConditionalBranch As Boolean
			Get
				Return Me._ExprIsOperandOfConditionalBranch
			End Get
		End Property

		Public ReadOnly Property LambdaSymbol As SynthesizedLambdaSymbol
			Get
				Return Me._LambdaSymbol
			End Get
		End Property

		Public ReadOnly Property RangeVariables As ImmutableArray(Of RangeVariableSymbol)
			Get
				Return Me._RangeVariables
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal lambdaSymbol As SynthesizedLambdaSymbol, ByVal rangeVariables As ImmutableArray(Of RangeVariableSymbol), ByVal expression As BoundExpression, ByVal exprIsOperandOfConditionalBranch As Boolean, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.QueryLambda, syntax, Nothing, If(hasErrors, True, expression.NonNullAndHasErrors()))
			Me._LambdaSymbol = lambdaSymbol
			Me._RangeVariables = rangeVariables
			Me._Expression = expression
			Me._ExprIsOperandOfConditionalBranch = exprIsOperandOfConditionalBranch
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitQueryLambda(Me)
		End Function

		Public Function Update(ByVal lambdaSymbol As SynthesizedLambdaSymbol, ByVal rangeVariables As ImmutableArray(Of RangeVariableSymbol), ByVal expression As BoundExpression, ByVal exprIsOperandOfConditionalBranch As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundQueryLambda
			Dim boundQueryLambda As Microsoft.CodeAnalysis.VisualBasic.BoundQueryLambda
			If (CObj(lambdaSymbol) <> CObj(Me.LambdaSymbol) OrElse rangeVariables <> Me.RangeVariables OrElse expression <> Me.Expression OrElse exprIsOperandOfConditionalBranch <> Me.ExprIsOperandOfConditionalBranch) Then
				Dim boundQueryLambda1 As Microsoft.CodeAnalysis.VisualBasic.BoundQueryLambda = New Microsoft.CodeAnalysis.VisualBasic.BoundQueryLambda(MyBase.Syntax, lambdaSymbol, rangeVariables, expression, exprIsOperandOfConditionalBranch, MyBase.HasErrors)
				boundQueryLambda1.CopyAttributes(Me)
				boundQueryLambda = boundQueryLambda1
			Else
				boundQueryLambda = Me
			End If
			Return boundQueryLambda
		End Function
	End Class
End Namespace