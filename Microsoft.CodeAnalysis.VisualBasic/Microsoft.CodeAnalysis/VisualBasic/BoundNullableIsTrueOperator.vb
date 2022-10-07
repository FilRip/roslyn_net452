Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundNullableIsTrueOperator
		Inherits BoundExpression
		Implements IBoundInvalidNode
		Private ReadOnly _Operand As BoundExpression

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Return ImmutableArray.Create(Of BoundNode)(Me.Operand)
			End Get
		End Property

		ReadOnly Property IBoundInvalidNode_InvalidNodeChildren As ImmutableArray(Of BoundNode) Implements IBoundInvalidNode.InvalidNodeChildren
			Get
				Return ImmutableArray.Create(Of BoundNode)(Me.Operand)
			End Get
		End Property

		Public ReadOnly Property Operand As BoundExpression
			Get
				Return Me._Operand
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal operand As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.NullableIsTrueOperator, syntax, type, If(hasErrors, True, operand.NonNullAndHasErrors()))
			Me._Operand = operand
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitNullableIsTrueOperator(Me)
		End Function

		Public Function Update(ByVal operand As BoundExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundNullableIsTrueOperator
			Dim boundNullableIsTrueOperator As Microsoft.CodeAnalysis.VisualBasic.BoundNullableIsTrueOperator
			If (operand <> Me.Operand OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundNullableIsTrueOperator1 As Microsoft.CodeAnalysis.VisualBasic.BoundNullableIsTrueOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundNullableIsTrueOperator(MyBase.Syntax, operand, type, MyBase.HasErrors)
				boundNullableIsTrueOperator1.CopyAttributes(Me)
				boundNullableIsTrueOperator = boundNullableIsTrueOperator1
			Else
				boundNullableIsTrueOperator = Me
			End If
			Return boundNullableIsTrueOperator
		End Function
	End Class
End Namespace