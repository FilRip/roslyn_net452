Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ExpressionStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax
		Friend _expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property Expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._expression)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionStatementSyntax(kind, errors, annotations, DirectCast(expression.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitExpressionStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitExpressionStatement(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 0) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._expression
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim expression As SyntaxNode
			If (i <> 0) Then
				expression = Nothing
			Else
				expression = Me.Expression
			End If
			Return expression
		End Function

		Public Function Update(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionStatementSyntax
			Dim expressionStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionStatementSyntax
			If (expression = Me.Expression) Then
				expressionStatementSyntax = Me
			Else
				Dim expressionStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ExpressionStatement(expression)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				expressionStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, expressionStatementSyntax1, expressionStatementSyntax1.WithAnnotations(annotations))
			End If
			Return expressionStatementSyntax
		End Function

		Public Function WithExpression(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionStatementSyntax
			Return Me.Update(expression)
		End Function
	End Class
End Namespace