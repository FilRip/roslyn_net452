Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class EndIfDirectiveTriviaSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
		Public ReadOnly Property EndKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndIfDirectiveTriviaSyntax)._endKeyword, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public Shadows ReadOnly Property HashToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndIfDirectiveTriviaSyntax)._hashToken, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property IfKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndIfDirectiveTriviaSyntax)._ifKeyword, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal hashToken As PunctuationSyntax, ByVal endKeyword As KeywordSyntax, ByVal ifKeyword As KeywordSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndIfDirectiveTriviaSyntax(kind, errors, annotations, hashToken, endKeyword, ifKeyword), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitEndIfDirectiveTrivia(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitEndIfDirectiveTrivia(Me)
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

		Public Function Update(ByVal hashToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal endKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal ifKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndIfDirectiveTriviaSyntax
			Dim endIfDirectiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndIfDirectiveTriviaSyntax
			If (hashToken <> Me.HashToken OrElse endKeyword <> Me.EndKeyword OrElse ifKeyword <> Me.IfKeyword) Then
				Dim endIfDirectiveTriviaSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndIfDirectiveTriviaSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.EndIfDirectiveTrivia(hashToken, endKeyword, ifKeyword)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				endIfDirectiveTriviaSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, endIfDirectiveTriviaSyntax1, endIfDirectiveTriviaSyntax1.WithAnnotations(annotations))
			Else
				endIfDirectiveTriviaSyntax = Me
			End If
			Return endIfDirectiveTriviaSyntax
		End Function

		Public Function WithEndKeyword(ByVal endKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndIfDirectiveTriviaSyntax
			Return Me.Update(Me.HashToken, endKeyword, Me.IfKeyword)
		End Function

		Public Shadows Function WithHashToken(ByVal hashToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndIfDirectiveTriviaSyntax
			Return Me.Update(hashToken, Me.EndKeyword, Me.IfKeyword)
		End Function

		Friend Overrides Function WithHashTokenCore(ByVal hashToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
			Return Me.WithHashToken(hashToken)
		End Function

		Public Function WithIfKeyword(ByVal ifKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndIfDirectiveTriviaSyntax
			Return Me.Update(Me.HashToken, Me.EndKeyword, ifKeyword)
		End Function
	End Class
End Namespace