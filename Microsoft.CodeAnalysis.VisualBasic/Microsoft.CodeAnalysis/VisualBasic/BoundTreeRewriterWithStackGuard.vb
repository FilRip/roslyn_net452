Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class BoundTreeRewriterWithStackGuard
		Inherits BoundTreeRewriter
		Private _recursionDepth As Integer

		Protected ReadOnly Property RecursionDepth As Integer
			Get
				Return Me._recursionDepth
			End Get
		End Property

		Protected Sub New()
			MyBase.New()
		End Sub

		Protected Sub New(ByVal recursionDepth As Integer)
			MyBase.New()
			Me._recursionDepth = recursionDepth
		End Sub

		Public Overrides Function Visit(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundNode) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			If (boundExpression Is Nothing) Then
				boundNode = MyBase.Visit(node)
			Else
				boundNode = MyBase.VisitExpressionWithStackGuard(Me._recursionDepth, boundExpression)
			End If
			Return boundNode
		End Function

		Protected NotOverridable Overrides Function VisitExpressionWithoutStackGuard(ByVal node As BoundExpression) As BoundExpression
			Return DirectCast(MyBase.Visit(node), BoundExpression)
		End Function

		Protected Function VisitExpressionWithStackGuard(ByVal expression As BoundExpression) As BoundExpression
			Return MyBase.VisitExpressionWithStackGuard(Me._recursionDepth, expression)
		End Function
	End Class
End Namespace