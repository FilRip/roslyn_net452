Imports Microsoft.CodeAnalysis
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundForToUserDefinedOperators
		Inherits BoundNode
		Private ReadOnly _LeftOperandPlaceholder As BoundRValuePlaceholder

		Private ReadOnly _RightOperandPlaceholder As BoundRValuePlaceholder

		Private ReadOnly _Addition As BoundUserDefinedBinaryOperator

		Private ReadOnly _Subtraction As BoundUserDefinedBinaryOperator

		Private ReadOnly _LessThanOrEqual As BoundExpression

		Private ReadOnly _GreaterThanOrEqual As BoundExpression

		Public ReadOnly Property Addition As BoundUserDefinedBinaryOperator
			Get
				Return Me._Addition
			End Get
		End Property

		Public ReadOnly Property GreaterThanOrEqual As BoundExpression
			Get
				Return Me._GreaterThanOrEqual
			End Get
		End Property

		Public ReadOnly Property LeftOperandPlaceholder As BoundRValuePlaceholder
			Get
				Return Me._LeftOperandPlaceholder
			End Get
		End Property

		Public ReadOnly Property LessThanOrEqual As BoundExpression
			Get
				Return Me._LessThanOrEqual
			End Get
		End Property

		Public ReadOnly Property RightOperandPlaceholder As BoundRValuePlaceholder
			Get
				Return Me._RightOperandPlaceholder
			End Get
		End Property

		Public ReadOnly Property Subtraction As BoundUserDefinedBinaryOperator
			Get
				Return Me._Subtraction
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal leftOperandPlaceholder As BoundRValuePlaceholder, ByVal rightOperandPlaceholder As BoundRValuePlaceholder, ByVal addition As BoundUserDefinedBinaryOperator, ByVal subtraction As BoundUserDefinedBinaryOperator, ByVal lessThanOrEqual As BoundExpression, ByVal greaterThanOrEqual As BoundExpression, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.ForToUserDefinedOperators, syntax, If(hasErrors OrElse leftOperandPlaceholder.NonNullAndHasErrors() OrElse rightOperandPlaceholder.NonNullAndHasErrors() OrElse addition.NonNullAndHasErrors() OrElse subtraction.NonNullAndHasErrors() OrElse lessThanOrEqual.NonNullAndHasErrors(), True, greaterThanOrEqual.NonNullAndHasErrors()))
			Me._LeftOperandPlaceholder = leftOperandPlaceholder
			Me._RightOperandPlaceholder = rightOperandPlaceholder
			Me._Addition = addition
			Me._Subtraction = subtraction
			Me._LessThanOrEqual = lessThanOrEqual
			Me._GreaterThanOrEqual = greaterThanOrEqual
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitForToUserDefinedOperators(Me)
		End Function

		Public Function Update(ByVal leftOperandPlaceholder As BoundRValuePlaceholder, ByVal rightOperandPlaceholder As BoundRValuePlaceholder, ByVal addition As BoundUserDefinedBinaryOperator, ByVal subtraction As BoundUserDefinedBinaryOperator, ByVal lessThanOrEqual As BoundExpression, ByVal greaterThanOrEqual As BoundExpression) As BoundForToUserDefinedOperators
			Dim boundForToUserDefinedOperator As BoundForToUserDefinedOperators
			If (leftOperandPlaceholder <> Me.LeftOperandPlaceholder OrElse rightOperandPlaceholder <> Me.RightOperandPlaceholder OrElse addition <> Me.Addition OrElse subtraction <> Me.Subtraction OrElse lessThanOrEqual <> Me.LessThanOrEqual OrElse greaterThanOrEqual <> Me.GreaterThanOrEqual) Then
				Dim boundForToUserDefinedOperator1 As BoundForToUserDefinedOperators = New BoundForToUserDefinedOperators(MyBase.Syntax, leftOperandPlaceholder, rightOperandPlaceholder, addition, subtraction, lessThanOrEqual, greaterThanOrEqual, MyBase.HasErrors)
				boundForToUserDefinedOperator1.CopyAttributes(Me)
				boundForToUserDefinedOperator = boundForToUserDefinedOperator1
			Else
				boundForToUserDefinedOperator = Me
			End If
			Return boundForToUserDefinedOperator
		End Function
	End Class
End Namespace