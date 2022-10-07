Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend Class LastTokenReplacer
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxRewriter
		Private ReadOnly _newItem As Func(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)

		Private _skipCnt As Integer

		Private Sub New(ByVal newItem As Func(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken))
			MyBase.New()
			Me._newItem = newItem
		End Sub

		Friend Shared Function Replace(Of TTree As GreenNode)(ByVal root As TTree, ByVal newItem As Func(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)) As TTree
			Return DirectCast((New LastTokenReplacer(newItem)).VisitGreen(DirectCast(root, GreenNode)), TTree)
		End Function

		Public Overrides Function Visit(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return DirectCast(Me.VisitGreen(node), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
		End Function

		Private Function VisitGreen(ByVal node As Microsoft.CodeAnalysis.GreenNode) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode1 As Microsoft.CodeAnalysis.GreenNode
			If (node Is Nothing) Then
				greenNode = Nothing
			ElseIf (Me._skipCnt <> 0) Then
				Me._skipCnt = Me._skipCnt - 1
				greenNode = node
			ElseIf (node.IsToken) Then
				greenNode = MyBase.Visit(DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken))
			Else
				Dim slotCount As Integer = 0
				Dim num As Integer = node.SlotCount - 1
				Dim num1 As Integer = 0
				Do
					Dim slot As Microsoft.CodeAnalysis.GreenNode = node.GetSlot(num1)
					If (slot IsNot Nothing) Then
						If (Not slot.IsList) Then
							slotCount = slotCount + 1
						Else
							slotCount += slot.SlotCount
						End If
					End If
					num1 = num1 + 1
				Loop While num1 <= num
				If (slotCount <> 0) Then
					Dim num2 As Integer = Me._skipCnt
					Me._skipCnt = slotCount - 1
					If (Not node.IsList) Then
						greenNode1 = MyBase.Visit(DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode))
					Else
						greenNode1 = MyBase.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node)).Node
					End If
					Me._skipCnt = num2
					greenNode = greenNode1
				Else
					greenNode = node
				End If
			End If
			Return greenNode
		End Function

		Public Overrides Function VisitSyntaxToken(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Return Me._newItem(token)
		End Function
	End Class
End Namespace