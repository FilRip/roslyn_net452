Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundTernaryConditionalExpression
		Inherits BoundExpression
		Private ReadOnly _Condition As BoundExpression

		Private ReadOnly _WhenTrue As BoundExpression

		Private ReadOnly _WhenFalse As BoundExpression

		Private ReadOnly _ConstantValueOpt As ConstantValue

		Public ReadOnly Property Condition As BoundExpression
			Get
				Return Me._Condition
			End Get
		End Property

		Public Overrides ReadOnly Property ConstantValueOpt As ConstantValue
			Get
				Return Me._ConstantValueOpt
			End Get
		End Property

		Public ReadOnly Property WhenFalse As BoundExpression
			Get
				Return Me._WhenFalse
			End Get
		End Property

		Public ReadOnly Property WhenTrue As BoundExpression
			Get
				Return Me._WhenTrue
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal condition As BoundExpression, ByVal whenTrue As BoundExpression, ByVal whenFalse As BoundExpression, ByVal constantValueOpt As ConstantValue, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.TernaryConditionalExpression, syntax, type, If(hasErrors OrElse condition.NonNullAndHasErrors() OrElse whenTrue.NonNullAndHasErrors(), True, whenFalse.NonNullAndHasErrors()))
			Me._Condition = condition
			Me._WhenTrue = whenTrue
			Me._WhenFalse = whenFalse
			Me._ConstantValueOpt = constantValueOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitTernaryConditionalExpression(Me)
		End Function

		Public Function Update(ByVal condition As BoundExpression, ByVal whenTrue As BoundExpression, ByVal whenFalse As BoundExpression, ByVal constantValueOpt As ConstantValue, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundTernaryConditionalExpression
			Dim boundTernaryConditionalExpression As Microsoft.CodeAnalysis.VisualBasic.BoundTernaryConditionalExpression
			If (condition <> Me.Condition OrElse whenTrue <> Me.WhenTrue OrElse whenFalse <> Me.WhenFalse OrElse CObj(constantValueOpt) <> CObj(Me.ConstantValueOpt) OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundTernaryConditionalExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundTernaryConditionalExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundTernaryConditionalExpression(MyBase.Syntax, condition, whenTrue, whenFalse, constantValueOpt, type, MyBase.HasErrors)
				boundTernaryConditionalExpression1.CopyAttributes(Me)
				boundTernaryConditionalExpression = boundTernaryConditionalExpression1
			Else
				boundTernaryConditionalExpression = Me
			End If
			Return boundTernaryConditionalExpression
		End Function
	End Class
End Namespace