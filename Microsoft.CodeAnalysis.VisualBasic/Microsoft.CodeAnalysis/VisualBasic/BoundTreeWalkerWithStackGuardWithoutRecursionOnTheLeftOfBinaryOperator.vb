Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class BoundTreeWalkerWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator
		Inherits BoundTreeWalkerWithStackGuard
		Protected Sub New()
			MyBase.New()
		End Sub

		Protected Sub New(ByVal recursionDepth As Integer)
			MyBase.New(recursionDepth)
		End Sub

		Public NotOverridable Overrides Function VisitBinaryOperator(ByVal node As BoundBinaryOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			If (node.Left.Kind = BoundKind.BinaryOperator) Then
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).GetInstance()
				instance.Push(node.Right)
				Dim left As BoundBinaryOperator = DirectCast(node.Left, BoundBinaryOperator)
				instance.Push(left.Right)
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = left.Left
				While boundExpression.Kind = BoundKind.BinaryOperator
					left = DirectCast(boundExpression, BoundBinaryOperator)
					instance.Push(left.Right)
					boundExpression = left.Left
				End While
				Me.Visit(boundExpression)
				While instance.Count > 0
					Me.Visit(instance.Pop())
				End While
				instance.Free()
				boundNode = Nothing
			Else
				boundNode = MyBase.VisitBinaryOperator(node)
			End If
			Return boundNode
		End Function
	End Class
End Namespace