Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundBinaryConditionalExpression
		Inherits BoundExpression
		Private ReadOnly _TestExpression As BoundExpression

		Private ReadOnly _ConvertedTestExpression As BoundExpression

		Private ReadOnly _TestExpressionPlaceholder As BoundRValuePlaceholder

		Private ReadOnly _ElseExpression As BoundExpression

		Private ReadOnly _ConstantValueOpt As ConstantValue

		Public Overrides ReadOnly Property ConstantValueOpt As ConstantValue
			Get
				Return Me._ConstantValueOpt
			End Get
		End Property

		Public ReadOnly Property ConvertedTestExpression As BoundExpression
			Get
				Return Me._ConvertedTestExpression
			End Get
		End Property

		Public ReadOnly Property ElseExpression As BoundExpression
			Get
				Return Me._ElseExpression
			End Get
		End Property

		Public ReadOnly Property TestExpression As BoundExpression
			Get
				Return Me._TestExpression
			End Get
		End Property

		Public ReadOnly Property TestExpressionPlaceholder As BoundRValuePlaceholder
			Get
				Return Me._TestExpressionPlaceholder
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal testExpression As BoundExpression, ByVal convertedTestExpression As BoundExpression, ByVal testExpressionPlaceholder As BoundRValuePlaceholder, ByVal elseExpression As BoundExpression, ByVal constantValueOpt As ConstantValue, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.BinaryConditionalExpression, syntax, type, If(hasErrors OrElse testExpression.NonNullAndHasErrors() OrElse convertedTestExpression.NonNullAndHasErrors() OrElse testExpressionPlaceholder.NonNullAndHasErrors(), True, elseExpression.NonNullAndHasErrors()))
			Me._TestExpression = testExpression
			Me._ConvertedTestExpression = convertedTestExpression
			Me._TestExpressionPlaceholder = testExpressionPlaceholder
			Me._ElseExpression = elseExpression
			Me._ConstantValueOpt = constantValueOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitBinaryConditionalExpression(Me)
		End Function

		Public Function Update(ByVal testExpression As BoundExpression, ByVal convertedTestExpression As BoundExpression, ByVal testExpressionPlaceholder As BoundRValuePlaceholder, ByVal elseExpression As BoundExpression, ByVal constantValueOpt As ConstantValue, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryConditionalExpression
			Dim boundBinaryConditionalExpression As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryConditionalExpression
			If (testExpression <> Me.TestExpression OrElse convertedTestExpression <> Me.ConvertedTestExpression OrElse testExpressionPlaceholder <> Me.TestExpressionPlaceholder OrElse elseExpression <> Me.ElseExpression OrElse CObj(constantValueOpt) <> CObj(Me.ConstantValueOpt) OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundBinaryConditionalExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryConditionalExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundBinaryConditionalExpression(MyBase.Syntax, testExpression, convertedTestExpression, testExpressionPlaceholder, elseExpression, constantValueOpt, type, MyBase.HasErrors)
				boundBinaryConditionalExpression1.CopyAttributes(Me)
				boundBinaryConditionalExpression = boundBinaryConditionalExpression1
			Else
				boundBinaryConditionalExpression = Me
			End If
			Return boundBinaryConditionalExpression
		End Function
	End Class
End Namespace