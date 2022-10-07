Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class XmlEmbeddedExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax
		Friend _expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property Expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._expression, 1)
			End Get
		End Property

		Public ReadOnly Property LessThanPercentEqualsToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmbeddedExpressionSyntax)._lessThanPercentEqualsToken, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property PercentGreaterThanToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmbeddedExpressionSyntax)._percentGreaterThanToken, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal lessThanPercentEqualsToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal percentGreaterThanToken As PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmbeddedExpressionSyntax(kind, errors, annotations, lessThanPercentEqualsToken, DirectCast(expression.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), percentGreaterThanToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitXmlEmbeddedExpression(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitXmlEmbeddedExpression(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._expression
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim expression As SyntaxNode
			If (i <> 1) Then
				expression = Nothing
			Else
				expression = Me.Expression
			End If
			Return expression
		End Function

		Public Function Update(ByVal lessThanPercentEqualsToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal percentGreaterThanToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlEmbeddedExpressionSyntax
			Dim xmlEmbeddedExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlEmbeddedExpressionSyntax
			If (lessThanPercentEqualsToken <> Me.LessThanPercentEqualsToken OrElse expression <> Me.Expression OrElse percentGreaterThanToken <> Me.PercentGreaterThanToken) Then
				Dim xmlEmbeddedExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlEmbeddedExpressionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.XmlEmbeddedExpression(lessThanPercentEqualsToken, expression, percentGreaterThanToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				xmlEmbeddedExpressionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, xmlEmbeddedExpressionSyntax1, xmlEmbeddedExpressionSyntax1.WithAnnotations(annotations))
			Else
				xmlEmbeddedExpressionSyntax = Me
			End If
			Return xmlEmbeddedExpressionSyntax
		End Function

		Public Function WithExpression(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlEmbeddedExpressionSyntax
			Return Me.Update(Me.LessThanPercentEqualsToken, expression, Me.PercentGreaterThanToken)
		End Function

		Public Function WithLessThanPercentEqualsToken(ByVal lessThanPercentEqualsToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlEmbeddedExpressionSyntax
			Return Me.Update(lessThanPercentEqualsToken, Me.Expression, Me.PercentGreaterThanToken)
		End Function

		Public Function WithPercentGreaterThanToken(ByVal percentGreaterThanToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlEmbeddedExpressionSyntax
			Return Me.Update(Me.LessThanPercentEqualsToken, Me.Expression, percentGreaterThanToken)
		End Function
	End Class
End Namespace