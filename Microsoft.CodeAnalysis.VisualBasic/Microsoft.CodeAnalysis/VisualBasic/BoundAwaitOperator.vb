Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundAwaitOperator
		Inherits BoundExpression
		Private ReadOnly _Operand As BoundExpression

		Private ReadOnly _AwaitableInstancePlaceholder As BoundRValuePlaceholder

		Private ReadOnly _GetAwaiter As BoundExpression

		Private ReadOnly _AwaiterInstancePlaceholder As BoundLValuePlaceholder

		Private ReadOnly _IsCompleted As BoundExpression

		Private ReadOnly _GetResult As BoundExpression

		Public ReadOnly Property AwaitableInstancePlaceholder As BoundRValuePlaceholder
			Get
				Return Me._AwaitableInstancePlaceholder
			End Get
		End Property

		Public ReadOnly Property AwaiterInstancePlaceholder As BoundLValuePlaceholder
			Get
				Return Me._AwaiterInstancePlaceholder
			End Get
		End Property

		Public ReadOnly Property GetAwaiter As BoundExpression
			Get
				Return Me._GetAwaiter
			End Get
		End Property

		Public ReadOnly Property GetResult As BoundExpression
			Get
				Return Me._GetResult
			End Get
		End Property

		Public ReadOnly Property IsCompleted As BoundExpression
			Get
				Return Me._IsCompleted
			End Get
		End Property

		Public ReadOnly Property Operand As BoundExpression
			Get
				Return Me._Operand
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal operand As BoundExpression, ByVal awaitableInstancePlaceholder As BoundRValuePlaceholder, ByVal getAwaiter As BoundExpression, ByVal awaiterInstancePlaceholder As BoundLValuePlaceholder, ByVal isCompleted As BoundExpression, ByVal getResult As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.AwaitOperator, syntax, type, If(hasErrors OrElse operand.NonNullAndHasErrors() OrElse awaitableInstancePlaceholder.NonNullAndHasErrors() OrElse getAwaiter.NonNullAndHasErrors() OrElse awaiterInstancePlaceholder.NonNullAndHasErrors() OrElse isCompleted.NonNullAndHasErrors(), True, getResult.NonNullAndHasErrors()))
			Me._Operand = operand
			Me._AwaitableInstancePlaceholder = awaitableInstancePlaceholder
			Me._GetAwaiter = getAwaiter
			Me._AwaiterInstancePlaceholder = awaiterInstancePlaceholder
			Me._IsCompleted = isCompleted
			Me._GetResult = getResult
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitAwaitOperator(Me)
		End Function

		Public Function Update(ByVal operand As BoundExpression, ByVal awaitableInstancePlaceholder As BoundRValuePlaceholder, ByVal getAwaiter As BoundExpression, ByVal awaiterInstancePlaceholder As BoundLValuePlaceholder, ByVal isCompleted As BoundExpression, ByVal getResult As BoundExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundAwaitOperator
			Dim boundAwaitOperator As Microsoft.CodeAnalysis.VisualBasic.BoundAwaitOperator
			If (operand <> Me.Operand OrElse awaitableInstancePlaceholder <> Me.AwaitableInstancePlaceholder OrElse getAwaiter <> Me.GetAwaiter OrElse awaiterInstancePlaceholder <> Me.AwaiterInstancePlaceholder OrElse isCompleted <> Me.IsCompleted OrElse getResult <> Me.GetResult OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundAwaitOperator1 As Microsoft.CodeAnalysis.VisualBasic.BoundAwaitOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundAwaitOperator(MyBase.Syntax, operand, awaitableInstancePlaceholder, getAwaiter, awaiterInstancePlaceholder, isCompleted, getResult, type, MyBase.HasErrors)
				boundAwaitOperator1.CopyAttributes(Me)
				boundAwaitOperator = boundAwaitOperator1
			Else
				boundAwaitOperator = Me
			End If
			Return boundAwaitOperator
		End Function
	End Class
End Namespace