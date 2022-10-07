Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class TypeOfExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
		Friend _expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Friend _type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax

		Public ReadOnly Property Expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._expression, 1)
			End Get
		End Property

		Public ReadOnly Property OperatorToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax)._operatorToken, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Public ReadOnly Property Type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)(Me._type, 3)
			End Get
		End Property

		Public ReadOnly Property TypeOfKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax)._typeOfKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal typeOfKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal operatorToken As KeywordSyntax, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax(kind, errors, annotations, typeOfKeyword, DirectCast(expression.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), operatorToken, DirectCast(type.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitTypeOfExpression(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitTypeOfExpression(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 1) Then
				syntaxNode = Me._expression
			ElseIf (num = 3) Then
				syntaxNode = Me._type
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim expression As SyntaxNode
			Dim num As Integer = i
			If (num = 1) Then
				expression = Me.Expression
			ElseIf (num = 3) Then
				expression = Me.Type
			Else
				expression = Nothing
			End If
			Return expression
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal typeOfKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal operatorToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeOfExpressionSyntax
			Dim typeOfExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeOfExpressionSyntax
			If (kind <> MyBase.Kind() OrElse typeOfKeyword <> Me.TypeOfKeyword OrElse expression <> Me.Expression OrElse operatorToken <> Me.OperatorToken OrElse type <> Me.Type) Then
				Dim typeOfExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeOfExpressionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.TypeOfExpression(kind, typeOfKeyword, expression, operatorToken, type)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				typeOfExpressionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, typeOfExpressionSyntax1, typeOfExpressionSyntax1.WithAnnotations(annotations))
			Else
				typeOfExpressionSyntax = Me
			End If
			Return typeOfExpressionSyntax
		End Function

		Public Function WithExpression(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeOfExpressionSyntax
			Return Me.Update(MyBase.Kind(), Me.TypeOfKeyword, expression, Me.OperatorToken, Me.Type)
		End Function

		Public Function WithOperatorToken(ByVal operatorToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeOfExpressionSyntax
			Return Me.Update(MyBase.Kind(), Me.TypeOfKeyword, Me.Expression, operatorToken, Me.Type)
		End Function

		Public Function WithType(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeOfExpressionSyntax
			Return Me.Update(MyBase.Kind(), Me.TypeOfKeyword, Me.Expression, Me.OperatorToken, type)
		End Function

		Public Function WithTypeOfKeyword(ByVal typeOfKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeOfExpressionSyntax
			Return Me.Update(MyBase.Kind(), typeOfKeyword, Me.Expression, Me.OperatorToken, Me.Type)
		End Function
	End Class
End Namespace