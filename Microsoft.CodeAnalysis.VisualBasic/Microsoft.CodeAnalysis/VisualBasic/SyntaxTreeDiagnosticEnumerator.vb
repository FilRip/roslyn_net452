Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Structure SyntaxTreeDiagnosticEnumerator
		Private ReadOnly _tree As SyntaxTree

		Private _stack As SyntaxTreeDiagnosticEnumerator.NodeIteration()

		Private _count As Integer

		Private _current As Diagnostic

		Private _position As Integer

		Public ReadOnly Property Current As Diagnostic
			Get
				Return Me._current
			End Get
		End Property

		Friend Sub New(ByVal tree As SyntaxTree, ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByVal position As Integer, ByVal inDocumentationComment As Boolean)
			Me = New SyntaxTreeDiagnosticEnumerator()
			If (node Is Nothing OrElse Not node.ContainsDiagnostics) Then
				Me._tree = Nothing
				Me._stack = Nothing
				Me._count = 0
			Else
				Me._tree = tree
				ReDim Me._stack(7)
				Me.Push(node, inDocumentationComment)
			End If
			Me._current = Nothing
			Me._position = position
		End Sub

		Public Function MoveNext() As Boolean
			Dim flag As Boolean
			While True
			Label0:
				If (Me._count > 0) Then
					Dim num As Integer = Me._stack(Me._count - 1).diagnosticIndex
					Dim greenNode As Microsoft.CodeAnalysis.GreenNode = Me._stack(Me._count - 1).node
					Dim diagnostics As Microsoft.CodeAnalysis.DiagnosticInfo() = greenNode.GetDiagnostics()
					Dim flag1 As Boolean = Me._stack(Me._count - 1).inDocumentationComment
					If (diagnostics Is Nothing OrElse num >= CInt(diagnostics.Length) - 1) Then
						Dim num1 As Integer = Me._stack(Me._count - 1).slotIndex
						flag1 = If(flag1, True, greenNode.RawKind = 710)
						While num1 < greenNode.SlotCount - 1
							num1 = num1 + 1
							Dim slot As Microsoft.CodeAnalysis.GreenNode = greenNode.GetSlot(num1)
							If (slot Is Nothing) Then
								Continue While
							End If
							If (slot.ContainsDiagnostics) Then
								Me._stack(Me._count - 1).slotIndex = num1
								Me.Push(slot, flag1)
								GoTo Label0
							Else
								Me._position += slot.FullWidth
							End If
						End While
						If (greenNode.SlotCount = 0) Then
							Me._position += greenNode.Width
						End If
						Me.Pop()
					Else
						num = num + 1
						Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = diagnostics(num)
						If (flag1) Then
							diagnosticInfo = ErrorFactory.ErrorInfo(ERRID.WRN_XMLDocParseError1, New [Object]() { diagnosticInfo })
						End If
						Dim leadingTriviaWidth As Integer = Me._position
						If (Not greenNode.IsToken) Then
							leadingTriviaWidth += greenNode.GetLeadingTriviaWidth()
						End If
						Me._current = New VBDiagnostic(diagnosticInfo, Me._tree.GetLocation(New TextSpan(leadingTriviaWidth, greenNode.Width)), False)
						Me._stack(Me._count - 1).diagnosticIndex = num
						flag = True
						Exit While
					End If
				Else
					flag = False
					Exit While
				End If
			End While
			Return flag
		End Function

		Private Sub Pop()
			Me._count = Me._count - 1
		End Sub

		Private Sub Push(ByVal node As GreenNode, ByVal inDocumentationComment As Boolean)
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			If (syntaxToken IsNot Nothing) Then
				Me.PushToken(syntaxToken, inDocumentationComment)
				Return
			End If
			Me.PushNode(node, inDocumentationComment)
		End Sub

		Private Sub PushNode(ByVal node As GreenNode, ByVal inDocumentationComment As Boolean)
			If (Me._count >= CInt(Me._stack.Length)) Then
				Dim nodeIterationArray(CInt(Me._stack.Length) * 2 - 1 + 1 - 1) As SyntaxTreeDiagnosticEnumerator.NodeIteration
				Array.Copy(Me._stack, nodeIterationArray, CInt(Me._stack.Length))
				Me._stack = nodeIterationArray
			End If
			Me._stack(Me._count) = New SyntaxTreeDiagnosticEnumerator.NodeIteration(node, inDocumentationComment)
			Me._count = Me._count + 1
		End Sub

		Private Sub PushToken(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal inDocumentationComment As Boolean)
			Dim trailingTrivia As GreenNode = token.GetTrailingTrivia()
			If (trailingTrivia IsNot Nothing) Then
				Me.Push(trailingTrivia, inDocumentationComment)
			End If
			Me.PushNode(token, inDocumentationComment)
			Dim leadingTrivia As GreenNode = token.GetLeadingTrivia()
			If (leadingTrivia IsNot Nothing) Then
				Me.Push(leadingTrivia, inDocumentationComment)
			End If
		End Sub

		Private Structure NodeIteration
			Friend ReadOnly node As GreenNode

			Friend diagnosticIndex As Integer

			Friend slotIndex As Integer

			Friend ReadOnly inDocumentationComment As Boolean

			Friend Sub New(ByVal node As GreenNode, ByVal inDocumentationComment As Boolean)
				Me = New SyntaxTreeDiagnosticEnumerator.NodeIteration() With
				{
					.node = node,
					.slotIndex = -1,
					.diagnosticIndex = -1,
					.inDocumentationComment = inDocumentationComment
				}
			End Sub
		End Structure
	End Structure
End Namespace