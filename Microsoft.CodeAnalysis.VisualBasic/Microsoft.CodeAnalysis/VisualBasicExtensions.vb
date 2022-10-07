Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis
	Public Module VisualBasicExtensions
		<Extension>
		Public Function Any(Of TNode As SyntaxNode)(ByVal list As SyntaxList(Of TNode), ByVal kind As SyntaxKind) As Boolean
			Return list.IndexOf(CInt(kind)) >= 0
		End Function

		<Extension>
		Public Function Any(Of TNode As SyntaxNode)(ByVal list As SeparatedSyntaxList(Of TNode), ByVal kind As SyntaxKind) As Boolean
			Return list.IndexOf(CInt(kind)) >= 0
		End Function

		<Extension>
		Public Function Any(ByVal list As SyntaxTriviaList, ByVal kind As SyntaxKind) As Boolean
			Return list.IndexOf(CInt(kind)) >= 0
		End Function

		<Extension>
		Public Function Any(ByVal list As SyntaxTokenList, ByVal kind As SyntaxKind) As Boolean
			Return list.IndexOf(CInt(kind)) >= 0
		End Function

		<Extension>
		Friend Function First(ByVal list As SyntaxTokenList, ByVal kind As SyntaxKind) As SyntaxToken
			Dim num As Integer = list.IndexOf(CInt(kind))
			If (num < 0) Then
				Throw New InvalidOperationException()
			End If
			Return list(num)
		End Function

		<Extension>
		Friend Function FirstOrDefault(ByVal list As SyntaxTokenList, ByVal kind As SyntaxKind) As SyntaxToken
			Dim num As Integer = list.IndexOf(CInt(kind))
			If (num < 0) Then
				Return New SyntaxToken()
			End If
			Return list(num)
		End Function

		<Extension>
		Public Function IndexOf(Of TNode As SyntaxNode)(ByVal list As SyntaxList(Of TNode), ByVal kind As SyntaxKind) As Integer
			Return list.IndexOf(CInt(kind))
		End Function

		<Extension>
		Public Function IndexOf(Of TNode As SyntaxNode)(ByVal list As SeparatedSyntaxList(Of TNode), ByVal kind As SyntaxKind) As Integer
			Return list.IndexOf(CInt(kind))
		End Function

		<Extension>
		Public Function IndexOf(ByVal list As SyntaxTriviaList, ByVal kind As SyntaxKind) As Integer
			Return list.IndexOf(CInt(kind))
		End Function

		<Extension>
		Public Function IndexOf(ByVal list As SyntaxTokenList, ByVal kind As SyntaxKind) As Integer
			Return list.IndexOf(CInt(kind))
		End Function

		<Extension>
		Public Function IsKind(ByVal trivia As SyntaxTrivia, ByVal kind As SyntaxKind) As Boolean
			Return trivia.RawKind = CInt(kind)
		End Function

		<Extension>
		Public Function IsKind(ByVal token As SyntaxToken, ByVal kind As SyntaxKind) As Boolean
			Return token.RawKind = CInt(kind)
		End Function

		<Extension>
		Public Function IsKind(ByVal node As SyntaxNode, ByVal kind As SyntaxKind) As Boolean
			If (node Is Nothing) Then
				Return False
			End If
			Return node.RawKind = CInt(kind)
		End Function

		<Extension>
		Public Function IsKind(ByVal nodeOrToken As SyntaxNodeOrToken, ByVal kind As SyntaxKind) As Boolean
			Return nodeOrToken.RawKind = CInt(kind)
		End Function
	End Module
End Namespace