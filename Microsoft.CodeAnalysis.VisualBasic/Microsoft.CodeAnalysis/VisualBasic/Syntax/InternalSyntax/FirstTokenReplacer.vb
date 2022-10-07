Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend Class FirstTokenReplacer
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxRewriter
		Private ReadOnly _newItem As Func(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)

		Private _isFirst As Boolean

		Private Sub New(ByVal newItem As Func(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken))
			MyBase.New()
			Me._isFirst = True
			Me._newItem = newItem
		End Sub

		Friend Shared Function Replace(Of TTree As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal root As TTree, ByVal newItem As Func(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)) As TTree
			Return DirectCast((New FirstTokenReplacer(newItem)).Visit(DirectCast(root, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)), TTree)
		End Function

		Public Overrides Function Visit(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			If (node Is Nothing) Then
				visualBasicSyntaxNode = Nothing
			ElseIf (Me._isFirst) Then
				Me._isFirst = False
				visualBasicSyntaxNode = MyBase.Visit(node)
			Else
				visualBasicSyntaxNode = node
			End If
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitSyntaxToken(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Return Me._newItem(token)
		End Function
	End Class
End Namespace