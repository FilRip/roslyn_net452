Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class NameOfExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
		Friend _argument As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property Argument As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._argument, 2)
			End Get
		End Property

		Public ReadOnly Property CloseParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameOfExpressionSyntax)._closeParenToken, Me.GetChildPosition(3), MyBase.GetChildIndex(3))
			End Get
		End Property

		Public ReadOnly Property NameOfKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameOfExpressionSyntax)._nameOfKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property OpenParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameOfExpressionSyntax)._openParenToken, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal nameOfKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal argument As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal closeParenToken As PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameOfExpressionSyntax(kind, errors, annotations, nameOfKeyword, openParenToken, DirectCast(argument.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), closeParenToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitNameOfExpression(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitNameOfExpression(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 2) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._argument
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim argument As SyntaxNode
			If (i <> 2) Then
				argument = Nothing
			Else
				argument = Me.Argument
			End If
			Return argument
		End Function

		Public Function Update(ByVal nameOfKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal argument As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameOfExpressionSyntax
			Dim nameOfExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameOfExpressionSyntax
			If (nameOfKeyword <> Me.NameOfKeyword OrElse openParenToken <> Me.OpenParenToken OrElse argument <> Me.Argument OrElse closeParenToken <> Me.CloseParenToken) Then
				Dim nameOfExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameOfExpressionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.NameOfExpression(nameOfKeyword, openParenToken, argument, closeParenToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				nameOfExpressionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, nameOfExpressionSyntax1, nameOfExpressionSyntax1.WithAnnotations(annotations))
			Else
				nameOfExpressionSyntax = Me
			End If
			Return nameOfExpressionSyntax
		End Function

		Public Function WithArgument(ByVal argument As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameOfExpressionSyntax
			Return Me.Update(Me.NameOfKeyword, Me.OpenParenToken, argument, Me.CloseParenToken)
		End Function

		Public Function WithCloseParenToken(ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameOfExpressionSyntax
			Return Me.Update(Me.NameOfKeyword, Me.OpenParenToken, Me.Argument, closeParenToken)
		End Function

		Public Function WithNameOfKeyword(ByVal nameOfKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameOfExpressionSyntax
			Return Me.Update(nameOfKeyword, Me.OpenParenToken, Me.Argument, Me.CloseParenToken)
		End Function

		Public Function WithOpenParenToken(ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NameOfExpressionSyntax
			Return Me.Update(Me.NameOfKeyword, openParenToken, Me.Argument, Me.CloseParenToken)
		End Function
	End Class
End Namespace