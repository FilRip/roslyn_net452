Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ExitStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax
		Public ReadOnly Property BlockKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax)._blockKeyword, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property ExitKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax)._exitKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal exitKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax(kind, errors, annotations, exitKeyword, blockKeyword), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitExitStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitExitStatement(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal exitKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal blockKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExitStatementSyntax
			Dim exitStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExitStatementSyntax
			If (kind <> MyBase.Kind() OrElse exitKeyword <> Me.ExitKeyword OrElse blockKeyword <> Me.BlockKeyword) Then
				Dim exitStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExitStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ExitStatement(kind, exitKeyword, blockKeyword)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				exitStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, exitStatementSyntax1, exitStatementSyntax1.WithAnnotations(annotations))
			Else
				exitStatementSyntax = Me
			End If
			Return exitStatementSyntax
		End Function

		Public Function WithBlockKeyword(ByVal blockKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExitStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.ExitKeyword, blockKeyword)
		End Function

		Public Function WithExitKeyword(ByVal exitKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExitStatementSyntax
			Return Me.Update(MyBase.Kind(), exitKeyword, Me.BlockKeyword)
		End Function
	End Class
End Namespace