Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class BinaryConditionalExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
		Friend _firstExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Friend _secondExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property CloseParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryConditionalExpressionSyntax)._closeParenToken, Me.GetChildPosition(5), MyBase.GetChildIndex(5))
			End Get
		End Property

		Public ReadOnly Property CommaToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryConditionalExpressionSyntax)._commaToken, Me.GetChildPosition(3), MyBase.GetChildIndex(3))
			End Get
		End Property

		Public ReadOnly Property FirstExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._firstExpression, 2)
			End Get
		End Property

		Public ReadOnly Property IfKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryConditionalExpressionSyntax)._ifKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property OpenParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryConditionalExpressionSyntax)._openParenToken, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property SecondExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._secondExpression, 4)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal ifKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal firstExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal commaToken As PunctuationSyntax, ByVal secondExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal closeParenToken As PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryConditionalExpressionSyntax(kind, errors, annotations, ifKeyword, openParenToken, DirectCast(firstExpression.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), commaToken, DirectCast(secondExpression.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), closeParenToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitBinaryConditionalExpression(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitBinaryConditionalExpression(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 2) Then
				syntaxNode = Me._firstExpression
			ElseIf (num = 4) Then
				syntaxNode = Me._secondExpression
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim firstExpression As SyntaxNode
			Dim num As Integer = i
			If (num = 2) Then
				firstExpression = Me.FirstExpression
			ElseIf (num = 4) Then
				firstExpression = Me.SecondExpression
			Else
				firstExpression = Nothing
			End If
			Return firstExpression
		End Function

		Public Function Update(ByVal ifKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal firstExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal commaToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal secondExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryConditionalExpressionSyntax
			Dim binaryConditionalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryConditionalExpressionSyntax
			If (ifKeyword <> Me.IfKeyword OrElse openParenToken <> Me.OpenParenToken OrElse firstExpression <> Me.FirstExpression OrElse commaToken <> Me.CommaToken OrElse secondExpression <> Me.SecondExpression OrElse closeParenToken <> Me.CloseParenToken) Then
				Dim binaryConditionalExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryConditionalExpressionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.BinaryConditionalExpression(ifKeyword, openParenToken, firstExpression, commaToken, secondExpression, closeParenToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				binaryConditionalExpressionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, binaryConditionalExpressionSyntax1, binaryConditionalExpressionSyntax1.WithAnnotations(annotations))
			Else
				binaryConditionalExpressionSyntax = Me
			End If
			Return binaryConditionalExpressionSyntax
		End Function

		Public Function WithCloseParenToken(ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryConditionalExpressionSyntax
			Return Me.Update(Me.IfKeyword, Me.OpenParenToken, Me.FirstExpression, Me.CommaToken, Me.SecondExpression, closeParenToken)
		End Function

		Public Function WithCommaToken(ByVal commaToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryConditionalExpressionSyntax
			Return Me.Update(Me.IfKeyword, Me.OpenParenToken, Me.FirstExpression, commaToken, Me.SecondExpression, Me.CloseParenToken)
		End Function

		Public Function WithFirstExpression(ByVal firstExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryConditionalExpressionSyntax
			Return Me.Update(Me.IfKeyword, Me.OpenParenToken, firstExpression, Me.CommaToken, Me.SecondExpression, Me.CloseParenToken)
		End Function

		Public Function WithIfKeyword(ByVal ifKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryConditionalExpressionSyntax
			Return Me.Update(ifKeyword, Me.OpenParenToken, Me.FirstExpression, Me.CommaToken, Me.SecondExpression, Me.CloseParenToken)
		End Function

		Public Function WithOpenParenToken(ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryConditionalExpressionSyntax
			Return Me.Update(Me.IfKeyword, openParenToken, Me.FirstExpression, Me.CommaToken, Me.SecondExpression, Me.CloseParenToken)
		End Function

		Public Function WithSecondExpression(ByVal secondExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryConditionalExpressionSyntax
			Return Me.Update(Me.IfKeyword, Me.OpenParenToken, Me.FirstExpression, Me.CommaToken, secondExpression, Me.CloseParenToken)
		End Function
	End Class
End Namespace