Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class BadDirectiveTriviaSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
		Public Shadows ReadOnly Property HashToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BadDirectiveTriviaSyntax)._hashToken, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal hashToken As PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BadDirectiveTriviaSyntax(kind, errors, annotations, hashToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitBadDirectiveTrivia(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitBadDirectiveTrivia(Me)
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

		Public Function Update(ByVal hashToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.BadDirectiveTriviaSyntax
			Dim badDirectiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.BadDirectiveTriviaSyntax
			If (hashToken = Me.HashToken) Then
				badDirectiveTriviaSyntax = Me
			Else
				Dim badDirectiveTriviaSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.BadDirectiveTriviaSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.BadDirectiveTrivia(hashToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				badDirectiveTriviaSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, badDirectiveTriviaSyntax1, badDirectiveTriviaSyntax1.WithAnnotations(annotations))
			End If
			Return badDirectiveTriviaSyntax
		End Function

		Public Shadows Function WithHashToken(ByVal hashToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.BadDirectiveTriviaSyntax
			Return Me.Update(hashToken)
		End Function

		Friend Overrides Function WithHashTokenCore(ByVal hashToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
			Return Me.WithHashToken(hashToken)
		End Function
	End Class
End Namespace