Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class SkippedTokensTriviaSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.StructuredTriviaSyntax
		Implements ISkippedTokensTriviaSyntax
		ReadOnly Property ISkippedTokensTriviaSyntax_Tokens As SyntaxTokenList Implements ISkippedTokensTriviaSyntax.Tokens
			Get
				Return Me.Tokens
			End Get
		End Property

		Public ReadOnly Property Tokens As SyntaxTokenList
			Get
				Dim syntaxTokenLists As SyntaxTokenList
				Dim green As GreenNode = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SkippedTokensTriviaSyntax)._tokens
				syntaxTokenLists = If(green Is Nothing, New SyntaxTokenList(), New SyntaxTokenList(Me, green, MyBase.Position, 0))
				Return syntaxTokenLists
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal tokens As GreenNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SkippedTokensTriviaSyntax(kind, errors, annotations, tokens), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitSkippedTokensTrivia(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitSkippedTokensTrivia(Me)
		End Sub

		Public Function AddTokens(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SkippedTokensTriviaSyntax
			Return Me.WithTokens(Me.Tokens.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal tokens As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SkippedTokensTriviaSyntax
			Dim skippedTokensTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SkippedTokensTriviaSyntax
			If (tokens = Me.Tokens) Then
				skippedTokensTriviaSyntax = Me
			Else
				Dim skippedTokensTriviaSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.SkippedTokensTriviaSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.SkippedTokensTrivia(tokens)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				skippedTokensTriviaSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, skippedTokensTriviaSyntax1, skippedTokensTriviaSyntax1.WithAnnotations(annotations))
			End If
			Return skippedTokensTriviaSyntax
		End Function

		Public Function WithTokens(ByVal tokens As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SkippedTokensTriviaSyntax
			Return Me.Update(tokens)
		End Function
	End Class
End Namespace