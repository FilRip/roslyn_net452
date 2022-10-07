Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundUserDefinedShortCircuitingOperator
		Inherits BoundExpression
		Private ReadOnly _LeftOperand As BoundExpression

		Private ReadOnly _LeftOperandPlaceholder As BoundRValuePlaceholder

		Private ReadOnly _LeftTest As BoundExpression

		Private ReadOnly _BitwiseOperator As BoundUserDefinedBinaryOperator

		Public ReadOnly Property BitwiseOperator As BoundUserDefinedBinaryOperator
			Get
				Return Me._BitwiseOperator
			End Get
		End Property

		Public ReadOnly Property LeftOperand As BoundExpression
			Get
				Return Me._LeftOperand
			End Get
		End Property

		Public ReadOnly Property LeftOperandPlaceholder As BoundRValuePlaceholder
			Get
				Return Me._LeftOperandPlaceholder
			End Get
		End Property

		Public ReadOnly Property LeftTest As BoundExpression
			Get
				Return Me._LeftTest
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal leftOperand As BoundExpression, ByVal leftOperandPlaceholder As BoundRValuePlaceholder, ByVal leftTest As BoundExpression, ByVal bitwiseOperator As BoundUserDefinedBinaryOperator, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.UserDefinedShortCircuitingOperator, syntax, type, If(hasErrors OrElse leftOperand.NonNullAndHasErrors() OrElse leftOperandPlaceholder.NonNullAndHasErrors() OrElse leftTest.NonNullAndHasErrors(), True, bitwiseOperator.NonNullAndHasErrors()))
			Me._LeftOperand = leftOperand
			Me._LeftOperandPlaceholder = leftOperandPlaceholder
			Me._LeftTest = leftTest
			Me._BitwiseOperator = bitwiseOperator
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitUserDefinedShortCircuitingOperator(Me)
		End Function

		Public Function Update(ByVal leftOperand As BoundExpression, ByVal leftOperandPlaceholder As BoundRValuePlaceholder, ByVal leftTest As BoundExpression, ByVal bitwiseOperator As BoundUserDefinedBinaryOperator, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedShortCircuitingOperator
			Dim boundUserDefinedShortCircuitingOperator As Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedShortCircuitingOperator
			If (leftOperand <> Me.LeftOperand OrElse leftOperandPlaceholder <> Me.LeftOperandPlaceholder OrElse leftTest <> Me.LeftTest OrElse bitwiseOperator <> Me.BitwiseOperator OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundUserDefinedShortCircuitingOperator1 As Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedShortCircuitingOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedShortCircuitingOperator(MyBase.Syntax, leftOperand, leftOperandPlaceholder, leftTest, bitwiseOperator, type, MyBase.HasErrors)
				boundUserDefinedShortCircuitingOperator1.CopyAttributes(Me)
				boundUserDefinedShortCircuitingOperator = boundUserDefinedShortCircuitingOperator1
			Else
				boundUserDefinedShortCircuitingOperator = Me
			End If
			Return boundUserDefinedShortCircuitingOperator
		End Function
	End Class
End Namespace