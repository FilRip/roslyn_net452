Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class EndBlockStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclarationStatementSyntax
		Public ReadOnly Property BlockKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)._blockKeyword, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property EndKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)._endKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal endKeyword As KeywordSyntax, ByVal blockKeyword As KeywordSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(kind, errors, annotations, endKeyword, blockKeyword), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitEndBlockStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitEndBlockStatement(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal endKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal blockKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			If (kind <> MyBase.Kind() OrElse endKeyword <> Me.EndKeyword OrElse blockKeyword <> Me.BlockKeyword) Then
				Dim endBlockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.EndBlockStatement(kind, endKeyword, blockKeyword)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				endBlockStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, endBlockStatementSyntax1, endBlockStatementSyntax1.WithAnnotations(annotations))
			Else
				endBlockStatementSyntax = Me
			End If
			Return endBlockStatementSyntax
		End Function

		Public Function WithBlockKeyword(ByVal blockKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.EndKeyword, blockKeyword)
		End Function

		Public Function WithEndKeyword(ByVal endKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Return Me.Update(MyBase.Kind(), endKeyword, Me.BlockKeyword)
		End Function
	End Class
End Namespace