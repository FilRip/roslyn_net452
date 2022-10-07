Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class MemberAccessExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
		Friend _expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Friend _name As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleNameSyntax

		Public ReadOnly Property Expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._expression)
			End Get
		End Property

		Public ReadOnly Property Name As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleNameSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleNameSyntax)(Me._name, 2)
			End Get
		End Property

		Public ReadOnly Property OperatorToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax)._operatorToken, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleNameSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax(kind, errors, annotations, If(expression IsNot Nothing, DirectCast(expression.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), Nothing), operatorToken, DirectCast(name.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitMemberAccessExpression(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitMemberAccessExpression(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._expression
			ElseIf (num = 2) Then
				syntaxNode = Me._name
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
				expression = Me.Name
			Else
				expression = Nothing
			End If
			Return expression
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax
			Dim memberAccessExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax
			If (kind <> MyBase.Kind() OrElse expression <> Me.Expression OrElse operatorToken <> Me.OperatorToken OrElse name <> Me.Name) Then
				Dim memberAccessExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.MemberAccessExpression(kind, expression, operatorToken, name)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				memberAccessExpressionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, memberAccessExpressionSyntax1, memberAccessExpressionSyntax1.WithAnnotations(annotations))
			Else
				memberAccessExpressionSyntax = Me
			End If
			Return memberAccessExpressionSyntax
		End Function

		Public Function WithExpression(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax
			Return Me.Update(MyBase.Kind(), expression, Me.OperatorToken, Me.Name)
		End Function

		Public Function WithName(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax
			Return Me.Update(MyBase.Kind(), Me.Expression, Me.OperatorToken, name)
		End Function

		Public Function WithOperatorToken(ByVal operatorToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax
			Return Me.Update(MyBase.Kind(), Me.Expression, operatorToken, Me.Name)
		End Function
	End Class
End Namespace