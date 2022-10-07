Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public MustInherit Class CastExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
		Friend _expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Friend _type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax

		Public ReadOnly Property CloseParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return Me.GetCloseParenTokenCore()
			End Get
		End Property

		Public ReadOnly Property CommaToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return Me.GetCommaTokenCore()
			End Get
		End Property

		Public ReadOnly Property Expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return Me.GetExpressionCore()
			End Get
		End Property

		Public ReadOnly Property Keyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return Me.GetKeywordCore()
			End Get
		End Property

		Public ReadOnly Property OpenParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return Me.GetOpenParenTokenCore()
			End Get
		End Property

		Public ReadOnly Property Type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax
			Get
				Return Me.GetTypeCore()
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Overridable Function GetCloseParenTokenCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CastExpressionSyntax)._closeParenToken, Me.GetChildPosition(5), MyBase.GetChildIndex(5))
		End Function

		Friend Overridable Function GetCommaTokenCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CastExpressionSyntax)._commaToken, Me.GetChildPosition(3), MyBase.GetChildIndex(3))
		End Function

		Friend Overridable Function GetExpressionCore() As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._expression, 2)
		End Function

		Friend Overridable Function GetKeywordCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CastExpressionSyntax)._keyword, MyBase.Position, 0)
		End Function

		Friend Overridable Function GetOpenParenTokenCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CastExpressionSyntax)._openParenToken, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
		End Function

		Friend Overridable Function GetTypeCore() As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax
			Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)(Me._type, 4)
		End Function

		Public Function WithCloseParenToken(ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CastExpressionSyntax
			Return Me.WithCloseParenTokenCore(closeParenToken)
		End Function

		Friend MustOverride Function WithCloseParenTokenCore(ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CastExpressionSyntax

		Public Function WithCommaToken(ByVal commaToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CastExpressionSyntax
			Return Me.WithCommaTokenCore(commaToken)
		End Function

		Friend MustOverride Function WithCommaTokenCore(ByVal commaToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CastExpressionSyntax

		Public Function WithExpression(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CastExpressionSyntax
			Return Me.WithExpressionCore(expression)
		End Function

		Friend MustOverride Function WithExpressionCore(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CastExpressionSyntax

		Public Function WithKeyword(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CastExpressionSyntax
			Return Me.WithKeywordCore(keyword)
		End Function

		Friend MustOverride Function WithKeywordCore(ByVal keyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CastExpressionSyntax

		Public Function WithOpenParenToken(ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CastExpressionSyntax
			Return Me.WithOpenParenTokenCore(openParenToken)
		End Function

		Friend MustOverride Function WithOpenParenTokenCore(ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CastExpressionSyntax

		Public Function WithType(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CastExpressionSyntax
			Return Me.WithTypeCore(type)
		End Function

		Friend MustOverride Function WithTypeCore(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CastExpressionSyntax
	End Class
End Namespace