Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ElseStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax
		Public ReadOnly Property ElseKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax)._elseKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal elseKeyword As KeywordSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax(kind, errors, annotations, elseKeyword), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitElseStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitElseStatement(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal elseKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseStatementSyntax
			Dim elseStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseStatementSyntax
			If (elseKeyword = Me.ElseKeyword) Then
				elseStatementSyntax = Me
			Else
				Dim elseStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ElseStatement(elseKeyword)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				elseStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, elseStatementSyntax1, elseStatementSyntax1.WithAnnotations(annotations))
			End If
			Return elseStatementSyntax
		End Function

		Public Function WithElseKeyword(ByVal elseKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseStatementSyntax
			Return Me.Update(elseKeyword)
		End Function
	End Class
End Namespace