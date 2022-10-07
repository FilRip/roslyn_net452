Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class CTypeExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.CastExpressionSyntax
		Public Shadows ReadOnly Property CloseParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CTypeExpressionSyntax)._closeParenToken, Me.GetChildPosition(5), MyBase.GetChildIndex(5))
			End Get
		End Property

		Public Shadows ReadOnly Property CommaToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CTypeExpressionSyntax)._commaToken, Me.GetChildPosition(3), MyBase.GetChildIndex(3))
			End Get
		End Property

		Public Shadows ReadOnly Property Expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._expression, 2)
			End Get
		End Property

		Public Shadows ReadOnly Property Keyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CTypeExpressionSyntax)._keyword, MyBase.Position, 0)
			End Get
		End Property

		Public Shadows ReadOnly Property OpenParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CTypeExpressionSyntax)._openParenToken, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public Shadows ReadOnly Property Type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)(Me._type, 4)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal keyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal commaToken As PunctuationSyntax, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax, ByVal closeParenToken As PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CTypeExpressionSyntax(kind, errors, annotations, keyword, openParenToken, DirectCast(expression.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), commaToken, DirectCast(type.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax), closeParenToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitCTypeExpression(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitCTypeExpression(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 2) Then
				syntaxNode = Me._expression
			ElseIf (num = 4) Then
				syntaxNode = Me._type
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetCloseParenTokenCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return Me.CloseParenToken
		End Function

		Friend Overrides Function GetCommaTokenCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return Me.CommaToken
		End Function

		Friend Overrides Function GetExpressionCore() As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Return Me.Expression
		End Function

		Friend Overrides Function GetKeywordCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return Me.Keyword
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim expression As SyntaxNode
			Dim num As Integer = i
			If (num = 2) Then
				expression = Me.Expression
			ElseIf (num = 4) Then
				expression = Me.Type
			Else
				expression = Nothing
			End If
			Return expression
		End Function

		Friend Overrides Function GetOpenParenTokenCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return Me.OpenParenToken
		End Function

		Friend Overrides Function GetTypeCore() As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax
			Return Me.Type
		End Function

		Public Function Update(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal commaToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax, ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CTypeExpressionSyntax
			Dim cTypeExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CTypeExpressionSyntax
			If (keyword <> Me.Keyword OrElse openParenToken <> Me.OpenParenToken OrElse expression <> Me.Expression OrElse commaToken <> Me.CommaToken OrElse type <> Me.Type OrElse closeParenToken <> Me.CloseParenToken) Then
				Dim cTypeExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.CTypeExpressionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.CTypeExpression(keyword, openParenToken, expression, commaToken, type, closeParenToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				cTypeExpressionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, cTypeExpressionSyntax1, cTypeExpressionSyntax1.WithAnnotations(annotations))
			Else
				cTypeExpressionSyntax = Me
			End If
			Return cTypeExpressionSyntax
		End Function

		Public Shadows Function WithCloseParenToken(ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CTypeExpressionSyntax
			Return Me.Update(Me.Keyword, Me.OpenParenToken, Me.Expression, Me.CommaToken, Me.Type, closeParenToken)
		End Function

		Friend Overrides Function WithCloseParenTokenCore(ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CastExpressionSyntax
			Return Me.WithCloseParenToken(closeParenToken)
		End Function

		Public Shadows Function WithCommaToken(ByVal commaToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CTypeExpressionSyntax
			Return Me.Update(Me.Keyword, Me.OpenParenToken, Me.Expression, commaToken, Me.Type, Me.CloseParenToken)
		End Function

		Friend Overrides Function WithCommaTokenCore(ByVal commaToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CastExpressionSyntax
			Return Me.WithCommaToken(commaToken)
		End Function

		Public Shadows Function WithExpression(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CTypeExpressionSyntax
			Return Me.Update(Me.Keyword, Me.OpenParenToken, expression, Me.CommaToken, Me.Type, Me.CloseParenToken)
		End Function

		Friend Overrides Function WithExpressionCore(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CastExpressionSyntax
			Return Me.WithExpression(expression)
		End Function

		Public Shadows Function WithKeyword(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CTypeExpressionSyntax
			Return Me.Update(keyword, Me.OpenParenToken, Me.Expression, Me.CommaToken, Me.Type, Me.CloseParenToken)
		End Function

		Friend Overrides Function WithKeywordCore(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CastExpressionSyntax
			Return Me.WithKeyword(keyword)
		End Function

		Public Shadows Function WithOpenParenToken(ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CTypeExpressionSyntax
			Return Me.Update(Me.Keyword, openParenToken, Me.Expression, Me.CommaToken, Me.Type, Me.CloseParenToken)
		End Function

		Friend Overrides Function WithOpenParenTokenCore(ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CastExpressionSyntax
			Return Me.WithOpenParenToken(openParenToken)
		End Function

		Public Shadows Function WithType(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CTypeExpressionSyntax
			Return Me.Update(Me.Keyword, Me.OpenParenToken, Me.Expression, Me.CommaToken, type, Me.CloseParenToken)
		End Function

		Friend Overrides Function WithTypeCore(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CastExpressionSyntax
			Return Me.WithType(type)
		End Function
	End Class
End Namespace