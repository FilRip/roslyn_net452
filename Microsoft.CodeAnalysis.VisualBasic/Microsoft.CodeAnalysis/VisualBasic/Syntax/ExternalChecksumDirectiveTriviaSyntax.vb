Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ExternalChecksumDirectiveTriviaSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
		Public ReadOnly Property Checksum As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax)._checksum, Me.GetChildPosition(7), MyBase.GetChildIndex(7))
			End Get
		End Property

		Public ReadOnly Property CloseParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax)._closeParenToken, Me.GetChildPosition(8), MyBase.GetChildIndex(8))
			End Get
		End Property

		Public ReadOnly Property ExternalChecksumKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax)._externalChecksumKeyword, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property ExternalSource As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax)._externalSource, Me.GetChildPosition(3), MyBase.GetChildIndex(3))
			End Get
		End Property

		Public ReadOnly Property FirstCommaToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax)._firstCommaToken, Me.GetChildPosition(4), MyBase.GetChildIndex(4))
			End Get
		End Property

		Public ReadOnly Property Guid As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax)._guid, Me.GetChildPosition(5), MyBase.GetChildIndex(5))
			End Get
		End Property

		Public Shadows ReadOnly Property HashToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax)._hashToken, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property OpenParenToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax)._openParenToken, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Public ReadOnly Property SecondCommaToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax)._secondCommaToken, Me.GetChildPosition(6), MyBase.GetChildIndex(6))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal hashToken As PunctuationSyntax, ByVal externalChecksumKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal externalSource As StringLiteralTokenSyntax, ByVal firstCommaToken As PunctuationSyntax, ByVal guid As StringLiteralTokenSyntax, ByVal secondCommaToken As PunctuationSyntax, ByVal checksum As StringLiteralTokenSyntax, ByVal closeParenToken As PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax(kind, errors, annotations, hashToken, externalChecksumKeyword, openParenToken, externalSource, firstCommaToken, guid, secondCommaToken, checksum, closeParenToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitExternalChecksumDirectiveTrivia(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitExternalChecksumDirectiveTrivia(Me)
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

		Public Function Update(ByVal hashToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal externalChecksumKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal externalSource As Microsoft.CodeAnalysis.SyntaxToken, ByVal firstCommaToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal guid As Microsoft.CodeAnalysis.SyntaxToken, ByVal secondCommaToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal checksum As Microsoft.CodeAnalysis.SyntaxToken, ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalChecksumDirectiveTriviaSyntax
			Dim externalChecksumDirectiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalChecksumDirectiveTriviaSyntax
			If (hashToken <> Me.HashToken OrElse externalChecksumKeyword <> Me.ExternalChecksumKeyword OrElse openParenToken <> Me.OpenParenToken OrElse externalSource <> Me.ExternalSource OrElse firstCommaToken <> Me.FirstCommaToken OrElse guid <> Me.Guid OrElse secondCommaToken <> Me.SecondCommaToken OrElse checksum <> Me.Checksum OrElse closeParenToken <> Me.CloseParenToken) Then
				Dim externalChecksumDirectiveTriviaSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalChecksumDirectiveTriviaSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ExternalChecksumDirectiveTrivia(hashToken, externalChecksumKeyword, openParenToken, externalSource, firstCommaToken, guid, secondCommaToken, checksum, closeParenToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				externalChecksumDirectiveTriviaSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, externalChecksumDirectiveTriviaSyntax1, externalChecksumDirectiveTriviaSyntax1.WithAnnotations(annotations))
			Else
				externalChecksumDirectiveTriviaSyntax = Me
			End If
			Return externalChecksumDirectiveTriviaSyntax
		End Function

		Public Function WithChecksum(ByVal checksum As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalChecksumDirectiveTriviaSyntax
			Return Me.Update(Me.HashToken, Me.ExternalChecksumKeyword, Me.OpenParenToken, Me.ExternalSource, Me.FirstCommaToken, Me.Guid, Me.SecondCommaToken, checksum, Me.CloseParenToken)
		End Function

		Public Function WithCloseParenToken(ByVal closeParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalChecksumDirectiveTriviaSyntax
			Return Me.Update(Me.HashToken, Me.ExternalChecksumKeyword, Me.OpenParenToken, Me.ExternalSource, Me.FirstCommaToken, Me.Guid, Me.SecondCommaToken, Me.Checksum, closeParenToken)
		End Function

		Public Function WithExternalChecksumKeyword(ByVal externalChecksumKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalChecksumDirectiveTriviaSyntax
			Return Me.Update(Me.HashToken, externalChecksumKeyword, Me.OpenParenToken, Me.ExternalSource, Me.FirstCommaToken, Me.Guid, Me.SecondCommaToken, Me.Checksum, Me.CloseParenToken)
		End Function

		Public Function WithExternalSource(ByVal externalSource As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalChecksumDirectiveTriviaSyntax
			Return Me.Update(Me.HashToken, Me.ExternalChecksumKeyword, Me.OpenParenToken, externalSource, Me.FirstCommaToken, Me.Guid, Me.SecondCommaToken, Me.Checksum, Me.CloseParenToken)
		End Function

		Public Function WithFirstCommaToken(ByVal firstCommaToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalChecksumDirectiveTriviaSyntax
			Return Me.Update(Me.HashToken, Me.ExternalChecksumKeyword, Me.OpenParenToken, Me.ExternalSource, firstCommaToken, Me.Guid, Me.SecondCommaToken, Me.Checksum, Me.CloseParenToken)
		End Function

		Public Function WithGuid(ByVal guid As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalChecksumDirectiveTriviaSyntax
			Return Me.Update(Me.HashToken, Me.ExternalChecksumKeyword, Me.OpenParenToken, Me.ExternalSource, Me.FirstCommaToken, guid, Me.SecondCommaToken, Me.Checksum, Me.CloseParenToken)
		End Function

		Public Shadows Function WithHashToken(ByVal hashToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalChecksumDirectiveTriviaSyntax
			Return Me.Update(hashToken, Me.ExternalChecksumKeyword, Me.OpenParenToken, Me.ExternalSource, Me.FirstCommaToken, Me.Guid, Me.SecondCommaToken, Me.Checksum, Me.CloseParenToken)
		End Function

		Friend Overrides Function WithHashTokenCore(ByVal hashToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
			Return Me.WithHashToken(hashToken)
		End Function

		Public Function WithOpenParenToken(ByVal openParenToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalChecksumDirectiveTriviaSyntax
			Return Me.Update(Me.HashToken, Me.ExternalChecksumKeyword, openParenToken, Me.ExternalSource, Me.FirstCommaToken, Me.Guid, Me.SecondCommaToken, Me.Checksum, Me.CloseParenToken)
		End Function

		Public Function WithSecondCommaToken(ByVal secondCommaToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalChecksumDirectiveTriviaSyntax
			Return Me.Update(Me.HashToken, Me.ExternalChecksumKeyword, Me.OpenParenToken, Me.ExternalSource, Me.FirstCommaToken, Me.Guid, secondCommaToken, Me.Checksum, Me.CloseParenToken)
		End Function
	End Class
End Namespace