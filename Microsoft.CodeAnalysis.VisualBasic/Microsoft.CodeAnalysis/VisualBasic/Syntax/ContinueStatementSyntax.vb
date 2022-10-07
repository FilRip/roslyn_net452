Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ContinueStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax
		Public ReadOnly Property BlockKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax)._blockKeyword, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property ContinueKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax)._continueKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal continueKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax(kind, errors, annotations, continueKeyword, blockKeyword), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitContinueStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitContinueStatement(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal continueKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal blockKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ContinueStatementSyntax
			Dim continueStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ContinueStatementSyntax
			If (kind <> MyBase.Kind() OrElse continueKeyword <> Me.ContinueKeyword OrElse blockKeyword <> Me.BlockKeyword) Then
				Dim continueStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ContinueStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ContinueStatement(kind, continueKeyword, blockKeyword)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				continueStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, continueStatementSyntax1, continueStatementSyntax1.WithAnnotations(annotations))
			Else
				continueStatementSyntax = Me
			End If
			Return continueStatementSyntax
		End Function

		Public Function WithBlockKeyword(ByVal blockKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ContinueStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.ContinueKeyword, blockKeyword)
		End Function

		Public Function WithContinueKeyword(ByVal continueKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ContinueStatementSyntax
			Return Me.Update(MyBase.Kind(), continueKeyword, Me.BlockKeyword)
		End Function
	End Class
End Namespace