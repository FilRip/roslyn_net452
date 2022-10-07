Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ElseCaseClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseClauseSyntax
		Public ReadOnly Property ElseKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseCaseClauseSyntax)._elseKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal elseKeyword As KeywordSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseCaseClauseSyntax(kind, errors, annotations, elseKeyword), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitElseCaseClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitElseCaseClause(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal elseKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseCaseClauseSyntax
			Dim elseCaseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseCaseClauseSyntax
			If (elseKeyword = Me.ElseKeyword) Then
				elseCaseClauseSyntax = Me
			Else
				Dim elseCaseClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseCaseClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ElseCaseClause(elseKeyword)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				elseCaseClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, elseCaseClauseSyntax1, elseCaseClauseSyntax1.WithAnnotations(annotations))
			End If
			Return elseCaseClauseSyntax
		End Function

		Public Function WithElseKeyword(ByVal elseKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseCaseClauseSyntax
			Return Me.Update(elseKeyword)
		End Function
	End Class
End Namespace