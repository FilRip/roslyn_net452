Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ConditionalAccessExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
		Friend _expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Friend _whenNotNull As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property Expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._expression)
			End Get
		End Property

		Public ReadOnly Property QuestionMarkToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax)._questionMarkToken, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property WhenNotNull As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._whenNotNull, 2)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal questionMarkToken As PunctuationSyntax, ByVal whenNotNull As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax(kind, errors, annotations, If(expression IsNot Nothing, DirectCast(expression.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), Nothing), questionMarkToken, DirectCast(whenNotNull.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitConditionalAccessExpression(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitConditionalAccessExpression(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._expression
			ElseIf (num = 2) Then
				syntaxNode = Me._whenNotNull
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim expression As SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				expression = Me.Expression
			ElseIf (num = 2) Then
				expression = Me.WhenNotNull
			Else
				expression = Nothing
			End If
			Return expression
		End Function

		Public Function Update(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal questionMarkToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal whenNotNull As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConditionalAccessExpressionSyntax
			Dim conditionalAccessExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConditionalAccessExpressionSyntax
			If (expression <> Me.Expression OrElse questionMarkToken <> Me.QuestionMarkToken OrElse whenNotNull <> Me.WhenNotNull) Then
				Dim conditionalAccessExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConditionalAccessExpressionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ConditionalAccessExpression(expression, questionMarkToken, whenNotNull)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				conditionalAccessExpressionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, conditionalAccessExpressionSyntax1, conditionalAccessExpressionSyntax1.WithAnnotations(annotations))
			Else
				conditionalAccessExpressionSyntax = Me
			End If
			Return conditionalAccessExpressionSyntax
		End Function

		Public Function WithExpression(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConditionalAccessExpressionSyntax
			Return Me.Update(expression, Me.QuestionMarkToken, Me.WhenNotNull)
		End Function

		Public Function WithQuestionMarkToken(ByVal questionMarkToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConditionalAccessExpressionSyntax
			Return Me.Update(Me.Expression, questionMarkToken, Me.WhenNotNull)
		End Function

		Public Function WithWhenNotNull(ByVal whenNotNull As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConditionalAccessExpressionSyntax
			Return Me.Update(Me.Expression, Me.QuestionMarkToken, whenNotNull)
		End Function
	End Class
End Namespace