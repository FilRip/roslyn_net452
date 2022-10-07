Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class InvocationExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
		Friend _expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Friend _argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax

		Public ReadOnly Property ArgumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)(Me._argumentList, 1)
			End Get
		End Property

		Public ReadOnly Property Expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._expression)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InvocationExpressionSyntax(kind, errors, annotations, If(expression IsNot Nothing, DirectCast(expression.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), Nothing), If(argumentList IsNot Nothing, DirectCast(argumentList.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax), Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitInvocationExpression(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitInvocationExpression(Me)
		End Sub

		Public Function AddArgumentListArguments(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax
			Return Me.WithArgumentList(If(Me.ArgumentList IsNot Nothing, Me.ArgumentList, Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ArgumentList()).AddArguments(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._expression
			ElseIf (num = 1) Then
				syntaxNode = Me._argumentList
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
			ElseIf (num = 1) Then
				expression = Me.ArgumentList
			Else
				expression = Nothing
			End If
			Return expression
		End Function

		Public Function Update(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax
			Dim invocationExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax
			If (expression <> Me.Expression OrElse argumentList <> Me.ArgumentList) Then
				Dim invocationExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.InvocationExpression(expression, argumentList)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				invocationExpressionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, invocationExpressionSyntax1, invocationExpressionSyntax1.WithAnnotations(annotations))
			Else
				invocationExpressionSyntax = Me
			End If
			Return invocationExpressionSyntax
		End Function

		Public Function WithArgumentList(ByVal argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax
			Return Me.Update(Me.Expression, argumentList)
		End Function

		Public Function WithExpression(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax
			Return Me.Update(expression, Me.ArgumentList)
		End Function
	End Class
End Namespace