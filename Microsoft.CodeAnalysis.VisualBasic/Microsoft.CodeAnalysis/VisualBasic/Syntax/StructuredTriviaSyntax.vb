Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public MustInherit Class StructuredTriviaSyntax
		Inherits VisualBasicSyntaxNode
		Implements IStructuredTriviaSyntax
		Private _parentTrivia As SyntaxTrivia

		Public Overrides ReadOnly Property ParentTrivia As SyntaxTrivia Implements IStructuredTriviaSyntax.ParentTrivia
			Get
				Return Me._parentTrivia
			End Get
		End Property

		Friend Sub New(ByVal green As Microsoft.CodeAnalysis.GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, startLocation, If(parent IsNot Nothing, parent.SyntaxTree, Nothing))
		End Sub

		Friend Shared Function Create(ByVal trivia As SyntaxTrivia) As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructuredTriviaSyntax
			Dim parent As VisualBasicSyntaxNode = DirectCast(trivia.Token.Parent, VisualBasicSyntaxNode)
			Dim position As Integer = trivia.Position
			Dim structuredTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.StructuredTriviaSyntax = DirectCast(trivia.UnderlyingNode.CreateRed(parent, position), Microsoft.CodeAnalysis.VisualBasic.Syntax.StructuredTriviaSyntax)
			structuredTriviaSyntax._parentTrivia = trivia
			Return structuredTriviaSyntax
		End Function
	End Class
End Namespace