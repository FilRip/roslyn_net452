Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ExternalSourceDirectiveTriviaSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
		Public ReadOnly Property CloseParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax)._closeParenToken, Me.GetChildPosition(6), MyBase.GetChildIndex(6))
			End Get
		End Property

		Public ReadOnly Property CommaToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax)._commaToken, Me.GetChildPosition(4), MyBase.GetChildIndex(4))
			End Get
		End Property

		Public ReadOnly Property ExternalSource As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax)._externalSource, Me.GetChildPosition(3), MyBase.GetChildIndex(3))
			End Get
		End Property

		Public ReadOnly Property ExternalSourceKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax)._externalSourceKeyword, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public Shadows ReadOnly Property HashToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax)._hashToken, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property LineStart As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax)._lineStart, Me.GetChildPosition(5), MyBase.GetChildIndex(5))
			End Get
		End Property

		Public ReadOnly Property OpenParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax)._openParenToken, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal hashToken As PunctuationSyntax, ByVal externalSourceKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal externalSource As StringLiteralTokenSyntax, ByVal commaToken As PunctuationSyntax, ByVal lineStart As IntegerLiteralTokenSyntax, ByVal closeParenToken As PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax(kind, errors, annotations, hashToken, externalSourceKeyword, openParenToken, externalSource, commaToken, lineStart, closeParenToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitExternalSourceDirectiveTrivia(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitExternalSourceDirectiveTrivia(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetHashTokenCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return Me.HashToken
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal hashToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal externalSourceKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal externalSource As Microsoft.CodeAnalysis.SyntaxToken, ByVal commaToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal lineStart As Microsoft.CodeAnalysis.SyntaxToken, ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalSourceDirectiveTriviaSyntax
			Dim externalSourceDirectiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalSourceDirectiveTriviaSyntax
			If (hashToken <> Me.HashToken OrElse externalSourceKeyword <> Me.ExternalSourceKeyword OrElse openParenToken <> Me.OpenParenToken OrElse externalSource <> Me.ExternalSource OrElse commaToken <> Me.CommaToken OrElse lineStart <> Me.LineStart OrElse closeParenToken <> Me.CloseParenToken) Then
				Dim externalSourceDirectiveTriviaSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalSourceDirectiveTriviaSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ExternalSourceDirectiveTrivia(hashToken, externalSourceKeyword, openParenToken, externalSource, commaToken, lineStart, closeParenToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				externalSourceDirectiveTriviaSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, externalSourceDirectiveTriviaSyntax1, externalSourceDirectiveTriviaSyntax1.WithAnnotations(annotations))
			Else
				externalSourceDirectiveTriviaSyntax = Me
			End If
			Return externalSourceDirectiveTriviaSyntax
		End Function

		Public Function WithCloseParenToken(ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalSourceDirectiveTriviaSyntax
			Return Me.Update(Me.HashToken, Me.ExternalSourceKeyword, Me.OpenParenToken, Me.ExternalSource, Me.CommaToken, Me.LineStart, closeParenToken)
		End Function

		Public Function WithCommaToken(ByVal commaToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalSourceDirectiveTriviaSyntax
			Return Me.Update(Me.HashToken, Me.ExternalSourceKeyword, Me.OpenParenToken, Me.ExternalSource, commaToken, Me.LineStart, Me.CloseParenToken)
		End Function

		Public Function WithExternalSource(ByVal externalSource As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalSourceDirectiveTriviaSyntax
			Return Me.Update(Me.HashToken, Me.ExternalSourceKeyword, Me.OpenParenToken, externalSource, Me.CommaToken, Me.LineStart, Me.CloseParenToken)
		End Function

		Public Function WithExternalSourceKeyword(ByVal externalSourceKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalSourceDirectiveTriviaSyntax
			Return Me.Update(Me.HashToken, externalSourceKeyword, Me.OpenParenToken, Me.ExternalSource, Me.CommaToken, Me.LineStart, Me.CloseParenToken)
		End Function

		Public Shadows Function WithHashToken(ByVal hashToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalSourceDirectiveTriviaSyntax
			Return Me.Update(hashToken, Me.ExternalSourceKeyword, Me.OpenParenToken, Me.ExternalSource, Me.CommaToken, Me.LineStart, Me.CloseParenToken)
		End Function

		Friend Overrides Function WithHashTokenCore(ByVal hashToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
			Return Me.WithHashToken(hashToken)
		End Function

		Public Function WithLineStart(ByVal lineStart As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalSourceDirectiveTriviaSyntax
			Return Me.Update(Me.HashToken, Me.ExternalSourceKeyword, Me.OpenParenToken, Me.ExternalSource, Me.CommaToken, lineStart, Me.CloseParenToken)
		End Function

		Public Function WithOpenParenToken(ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalSourceDirectiveTriviaSyntax
			Return Me.Update(Me.HashToken, Me.ExternalSourceKeyword, openParenToken, Me.ExternalSource, Me.CommaToken, Me.LineStart, Me.CloseParenToken)
		End Function
	End Class
End Namespace