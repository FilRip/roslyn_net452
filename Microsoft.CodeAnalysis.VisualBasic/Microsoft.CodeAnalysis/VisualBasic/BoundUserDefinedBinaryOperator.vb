Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundUserDefinedBinaryOperator
		Inherits BoundExpression
		Private ReadOnly _OperatorKind As BinaryOperatorKind

		Private ReadOnly _UnderlyingExpression As BoundExpression

		Private ReadOnly _Checked As Boolean

		Public ReadOnly Property [Call] As BoundCall
			Get
				Return DirectCast(Me.UnderlyingExpression, BoundCall)
			End Get
		End Property

		Public ReadOnly Property Checked As Boolean
			Get
				Return Me._Checked
			End Get
		End Property

		Public ReadOnly Property Left As BoundExpression
			Get
				Return Me.[Call].Arguments(0)
			End Get
		End Property

		Public ReadOnly Property OperatorKind As BinaryOperatorKind
			Get
				Return Me._OperatorKind
			End Get
		End Property

		Public ReadOnly Property Right As BoundExpression
			Get
				Return Me.[Call].Arguments(1)
			End Get
		End Property

		Public ReadOnly Property UnderlyingExpression As BoundExpression
			Get
				Return Me._UnderlyingExpression
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal operatorKind As BinaryOperatorKind, ByVal underlyingExpression As BoundExpression, ByVal checked As Boolean, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.UserDefinedBinaryOperator, syntax, type, If(hasErrors, True, underlyingExpression.NonNullAndHasErrors()))
			Me._OperatorKind = operatorKind
			Me._UnderlyingExpression = underlyingExpression
			Me._Checked = checked
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitUserDefinedBinaryOperator(Me)
		End Function

		Public Function Update(ByVal operatorKind As BinaryOperatorKind, ByVal underlyingExpression As BoundExpression, ByVal checked As Boolean, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedBinaryOperator
			Dim boundUserDefinedBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedBinaryOperator
			If (operatorKind <> Me.OperatorKind OrElse underlyingExpression <> Me.UnderlyingExpression OrElse checked <> Me.Checked OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundUserDefinedBinaryOperator1 As Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedBinaryOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedBinaryOperator(MyBase.Syntax, operatorKind, underlyingExpression, checked, type, MyBase.HasErrors)
				boundUserDefinedBinaryOperator1.CopyAttributes(Me)
				boundUserDefinedBinaryOperator = boundUserDefinedBinaryOperator1
			Else
				boundUserDefinedBinaryOperator = Me
			End If
			Return boundUserDefinedBinaryOperator
		End Function
	End Class
End Namespace