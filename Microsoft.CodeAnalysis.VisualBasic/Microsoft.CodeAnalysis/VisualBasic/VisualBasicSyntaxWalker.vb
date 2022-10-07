Imports Microsoft.CodeAnalysis
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public MustInherit Class VisualBasicSyntaxWalker
		Inherits VisualBasicSyntaxVisitor
		Private _recursionDepth As Integer

		Protected ReadOnly Property Depth As SyntaxWalkerDepth

		Protected Sub New(Optional ByVal depth As SyntaxWalkerDepth = 0)
			MyBase.New()
			Me.Depth = depth
		End Sub

		Public Overrides Sub DefaultVisit(ByVal node As Microsoft.CodeAnalysis.SyntaxNode)
			Dim childSyntaxLists As ChildSyntaxList = node.ChildNodesAndTokens()
			Dim count As Integer = childSyntaxLists.Count
			Dim num As Integer = 0
			Do
				Dim item As SyntaxNodeOrToken = childSyntaxLists(num)
				num = num + 1
				Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = item.AsNode()
				If (syntaxNode Is Nothing) Then
					If (Me.Depth < SyntaxWalkerDepth.Token) Then
						Continue Do
					End If
					Me.VisitToken(item.AsToken())
				Else
					If (Me.Depth < SyntaxWalkerDepth.Node) Then
						Continue Do
					End If
					Me.Visit(syntaxNode)
				End If
			Loop While num < count
		End Sub

		Public Overrides Sub Visit(ByVal node As SyntaxNode)
			If (node IsNot Nothing) Then
				Me._recursionDepth = Me._recursionDepth + 1
				StackGuard.EnsureSufficientExecutionStack(Me._recursionDepth)
				DirectCast(node, VisualBasicSyntaxNode).Accept(Me)
				Me._recursionDepth = Me._recursionDepth - 1
			End If
		End Sub

		Public Overridable Sub VisitLeadingTrivia(ByVal token As SyntaxToken)
			If (token.HasLeadingTrivia) Then
				Dim enumerator As SyntaxTriviaList.Enumerator = token.LeadingTrivia.GetEnumerator()
				While enumerator.MoveNext()
					Me.VisitTrivia(enumerator.Current)
				End While
			End If
		End Sub

		Public Overridable Sub VisitToken(ByVal token As SyntaxToken)
			If (Me.Depth >= SyntaxWalkerDepth.Trivia) Then
				Me.VisitLeadingTrivia(token)
				Me.VisitTrailingTrivia(token)
			End If
		End Sub

		Public Overridable Sub VisitTrailingTrivia(ByVal token As SyntaxToken)
			If (token.HasTrailingTrivia) Then
				Dim enumerator As SyntaxTriviaList.Enumerator = token.TrailingTrivia.GetEnumerator()
				While enumerator.MoveNext()
					Me.VisitTrivia(enumerator.Current)
				End While
			End If
		End Sub

		Public Overridable Sub VisitTrivia(ByVal trivia As SyntaxTrivia)
			If (Me.Depth >= SyntaxWalkerDepth.StructuredTrivia AndAlso trivia.HasStructure) Then
				Me.Visit(DirectCast(trivia.GetStructure(), VisualBasicSyntaxNode))
			End If
		End Sub
	End Class
End Namespace