Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class BoundTreeRewriterWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator
		Inherits BoundTreeRewriterWithStackGuard
		Protected Sub New()
			MyBase.New()
		End Sub

		Protected Sub New(ByVal recursionDepth As Integer)
			MyBase.New(recursionDepth)
		End Sub

		Public NotOverridable Overrides Function VisitBinaryOperator(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim left As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Left
			If (left.Kind = BoundKind.BinaryOperator) Then
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator).GetInstance()
				instance.Push(node)
				Dim boundBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator = DirectCast(left, Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator)
				While True
					instance.Push(boundBinaryOperator)
					left = boundBinaryOperator.Left
					If (left.Kind <> BoundKind.BinaryOperator) Then
						Exit While
					End If
					boundBinaryOperator = DirectCast(left, Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator)
				End While
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(left), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				Do
					boundBinaryOperator = instance.Pop()
					Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(boundBinaryOperator.Right), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(boundBinaryOperator.Type)
					boundExpression = boundBinaryOperator.Update(boundBinaryOperator.OperatorKind, boundExpression, boundExpression1, boundBinaryOperator.Checked, boundBinaryOperator.ConstantValueOpt, typeSymbol)
				Loop While instance.Count > 0
				instance.Free()
				boundNode = boundExpression
			Else
				boundNode = MyBase.VisitBinaryOperator(node)
			End If
			Return boundNode
		End Function
	End Class
End Namespace