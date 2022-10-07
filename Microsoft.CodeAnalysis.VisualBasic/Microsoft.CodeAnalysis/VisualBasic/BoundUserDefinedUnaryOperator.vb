Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundUserDefinedUnaryOperator
		Inherits BoundExpression
		Private ReadOnly _OperatorKind As UnaryOperatorKind

		Private ReadOnly _UnderlyingExpression As BoundExpression

		Public ReadOnly Property [Call] As BoundCall
			Get
				Return DirectCast(Me.UnderlyingExpression, BoundCall)
			End Get
		End Property

		Public ReadOnly Property Operand As BoundExpression
			Get
				Return Me.[Call].Arguments(0)
			End Get
		End Property

		Public ReadOnly Property OperatorKind As UnaryOperatorKind
			Get
				Return Me._OperatorKind
			End Get
		End Property

		Public ReadOnly Property UnderlyingExpression As BoundExpression
			Get
				Return Me._UnderlyingExpression
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal operatorKind As UnaryOperatorKind, ByVal underlyingExpression As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.UserDefinedUnaryOperator, syntax, type, If(hasErrors, True, underlyingExpression.NonNullAndHasErrors()))
			Me._OperatorKind = operatorKind
			Me._UnderlyingExpression = underlyingExpression
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitUserDefinedUnaryOperator(Me)
		End Function

		Public Function Update(ByVal operatorKind As UnaryOperatorKind, ByVal underlyingExpression As BoundExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedUnaryOperator
			Dim boundUserDefinedUnaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedUnaryOperator
			If (operatorKind <> Me.OperatorKind OrElse underlyingExpression <> Me.UnderlyingExpression OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundUserDefinedUnaryOperator1 As Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedUnaryOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedUnaryOperator(MyBase.Syntax, operatorKind, underlyingExpression, type, MyBase.HasErrors)
				boundUserDefinedUnaryOperator1.CopyAttributes(Me)
				boundUserDefinedUnaryOperator = boundUserDefinedUnaryOperator1
			Else
				boundUserDefinedUnaryOperator = Me
			End If
			Return boundUserDefinedUnaryOperator
		End Function
	End Class
End Namespace