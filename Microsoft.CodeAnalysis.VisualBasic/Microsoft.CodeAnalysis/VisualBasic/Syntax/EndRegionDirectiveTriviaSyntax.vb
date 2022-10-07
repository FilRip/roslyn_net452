Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class EndRegionDirectiveTriviaSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
		Public ReadOnly Property EndKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndRegionDirectiveTriviaSyntax)._endKeyword, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public Shadows ReadOnly Property HashToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndRegionDirectiveTriviaSyntax)._hashToken, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property RegionKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndRegionDirectiveTriviaSyntax)._regionKeyword, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal hashToken As PunctuationSyntax, ByVal endKeyword As KeywordSyntax, ByVal regionKeyword As KeywordSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndRegionDirectiveTriviaSyntax(kind, errors, annotations, hashToken, endKeyword, regionKeyword), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitEndRegionDirectiveTrivia(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitEndRegionDirectiveTrivia(Me)
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

		Public Function Update(ByVal hashToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal endKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal regionKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndRegionDirectiveTriviaSyntax
			Dim endRegionDirectiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndRegionDirectiveTriviaSyntax
			If (hashToken <> Me.HashToken OrElse endKeyword <> Me.EndKeyword OrElse regionKeyword <> Me.RegionKeyword) Then
				Dim endRegionDirectiveTriviaSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndRegionDirectiveTriviaSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.EndRegionDirectiveTrivia(hashToken, endKeyword, regionKeyword)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				endRegionDirectiveTriviaSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, endRegionDirectiveTriviaSyntax1, endRegionDirectiveTriviaSyntax1.WithAnnotations(annotations))
			Else
				endRegionDirectiveTriviaSyntax = Me
			End If
			Return endRegionDirectiveTriviaSyntax
		End Function

		Public Function WithEndKeyword(ByVal endKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndRegionDirectiveTriviaSyntax
			Return Me.Update(Me.HashToken, endKeyword, Me.RegionKeyword)
		End Function

		Public Shadows Function WithHashToken(ByVal hashToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndRegionDirectiveTriviaSyntax
			Return Me.Update(hashToken, Me.EndKeyword, Me.RegionKeyword)
		End Function

		Friend Overrides Function WithHashTokenCore(ByVal hashToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
			Return Me.WithHashToken(hashToken)
		End Function

		Public Function WithRegionKeyword(ByVal regionKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndRegionDirectiveTriviaSyntax
			Return Me.Update(Me.HashToken, Me.EndKeyword, regionKeyword)
		End Function
	End Class
End Namespace