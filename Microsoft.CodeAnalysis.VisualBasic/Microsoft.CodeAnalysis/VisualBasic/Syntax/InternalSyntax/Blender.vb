Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Generic
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class Blender
		Inherits Scanner
		Private ReadOnly _nodeStack As Stack(Of GreenNode)

		Private ReadOnly _change As TextChangeRange

		Private ReadOnly _affectedRange As TextChangeRange

		Private _currentNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode

		Private _curNodeStart As Integer

		Private _curNodeLength As Integer

		Private ReadOnly _baseTreeRoot As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode

		Private _currentPreprocessorState As Scanner.PreprocessorState

		Private _nextPreprocessorStateGetter As Blender.NextPreprocessorStateGetter

		Friend Sub New(ByVal newText As SourceText, ByVal changes As TextChangeRange(), ByVal baseTreeRoot As SyntaxTree, ByVal options As VisualBasicParseOptions)
			MyBase.New(newText, options, False)
			Me._nodeStack = New Stack(Of GreenNode)()
			Me._currentPreprocessorState = Me._scannerPreprocessorState
			Me._nextPreprocessorStateGetter = New Blender.NextPreprocessorStateGetter()
			Me._baseTreeRoot = baseTreeRoot.GetVisualBasicRoot(New CancellationToken())
			Me._currentNode = Me._baseTreeRoot.VbGreen
			Me._curNodeStart = 0
			Me._curNodeLength = 0
			Me.TryCrumbleOnce()
			If (Me._currentNode IsNot Nothing) Then
				Me._change = TextChangeRange.Collapse(changes)
				Dim nearestStatements As Microsoft.CodeAnalysis.Text.TextSpan = Blender.ExpandToNearestStatements(Me._baseTreeRoot, Blender.ExpandByLookAheadAndBehind(Me._baseTreeRoot, Me._change.Span))
				Dim textSpan As Microsoft.CodeAnalysis.Text.TextSpan = nearestStatements
				Dim length As Integer = nearestStatements.Length
				Dim span As Microsoft.CodeAnalysis.Text.TextSpan = Me._change.Span
				Me._affectedRange = New TextChangeRange(textSpan, length - span.Length + Me._change.NewLength)
			End If
		End Sub

		Private Function CanReuseNode(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Boolean
			Dim flag As Boolean
			If (node Is Nothing) Then
				flag = False
			ElseIf (node.SlotCount = 0) Then
				flag = False
			ElseIf (node.ContainsDiagnostics) Then
				flag = False
			ElseIf (node.ContainsAnnotations) Then
				flag = False
			ElseIf (node.Kind <> SyntaxKind.IfStatement) Then
				Dim textSpan As Microsoft.CodeAnalysis.Text.TextSpan = New Microsoft.CodeAnalysis.Text.TextSpan(Me._curNodeStart, Me._curNodeLength)
				If (Me._affectedRange.Span.Length <> 0) Then
					If (Not textSpan.OverlapsWith(Me._affectedRange.Span)) Then
						GoTo Label1
					End If
					flag = False
					Return flag
				Else
					If (Not textSpan.Contains(Me._affectedRange.Span.Start)) Then
						GoTo Label1
					End If
					flag = False
					Return flag
				End If
				If (node.ContainsDirectives AndAlso Not TypeOf node Is Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax) Then
					flag = Me._scannerPreprocessorState.IsEquivalentTo(Me._currentPreprocessorState)
				ElseIf (Me._currentToken.State = ScannerState.VBAllowLeadingMultilineTrivia OrElse Not Me.ContainsLeadingLineBreaks(node)) Then
					flag = If(Not Me._currentNode.IsMissing, True, False)
				Else
					flag = False
				End If
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function ContainsLeadingLineBreaks(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Boolean
			Dim flag As Boolean
			Dim leadingTrivia As GreenNode = node.GetLeadingTrivia()
			If (leadingTrivia IsNot Nothing) Then
				If (leadingTrivia.RawKind <> 730) Then
					Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList = TryCast(leadingTrivia, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList)
					If (syntaxList IsNot Nothing) Then
						Dim slotCount As Integer = syntaxList.SlotCount - 1
						Dim num As Integer = 0
						While num <= slotCount
							If (leadingTrivia.GetSlot(num).RawKind <> 730) Then
								num = num + 1
							Else
								flag = True
								Return flag
							End If
						End While
					End If
				Else
					flag = True
					Return flag
				End If
			End If
			flag = False
			Return flag
		End Function

		Private Shared Function ExpandByLookAheadAndBehind(ByVal root As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode, ByVal span As TextSpan) As TextSpan
			Dim fullWidth As Integer = root.FullWidth
			Dim position As Integer = Math.Min(span.Start, Math.Max(0, fullWidth - 1))
			Dim [end] As Integer = span.[End]
			If (position > 0) Then
				Dim num As Integer = 0
				Do
					Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = root.FindTokenInternal(position)
					If (syntaxToken.Kind() = SyntaxKind.None) Then
						Exit Do
					End If
					position = syntaxToken.Position
					If (position = 0) Then
						Exit Do
					End If
					position = position - 1
					num = num + 1
				Loop While num <= 4
			End If
			If ([end] < fullWidth) Then
				[end] = [end] + 1
			End If
			Return TextSpan.FromBounds(position, [end])
		End Function

		Private Shared Function ExpandToNearestStatements(ByVal root As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode, ByVal span As Microsoft.CodeAnalysis.Text.TextSpan) As Microsoft.CodeAnalysis.Text.TextSpan
			Dim textSpan As Microsoft.CodeAnalysis.Text.TextSpan
			Dim textSpan1 As Microsoft.CodeAnalysis.Text.TextSpan = New Microsoft.CodeAnalysis.Text.TextSpan(0, root.FullWidth)
			Dim textSpan2 As Microsoft.CodeAnalysis.Text.TextSpan = Blender.NearestStatementThatContainsPosition(root, span.Start, textSpan1)
			If (span.Length <> 0) Then
				Dim textSpan3 As Microsoft.CodeAnalysis.Text.TextSpan = Blender.NearestStatementThatContainsPosition(root, span.[End] - 1, textSpan1)
				textSpan = Microsoft.CodeAnalysis.Text.TextSpan.FromBounds(textSpan2.Start, textSpan3.[End])
			Else
				textSpan = textSpan2
			End If
			Return textSpan
		End Function

		Private Function GetCurrentNode(ByVal position As Integer) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim oldTree As Integer = Me.MapNewPositionToOldTree(position)
			If (oldTree <> -1) Then
				Do
					If (Me._curNodeStart > oldTree) Then
						visualBasicSyntaxNode = Nothing
						Return visualBasicSyntaxNode
					ElseIf (Me._curNodeStart + Me._curNodeLength > oldTree) Then
						If (Me._curNodeStart <> oldTree OrElse Not Me.CanReuseNode(Me._currentNode)) Then
							Continue Do
						End If
						visualBasicSyntaxNode = Me._currentNode
						Return visualBasicSyntaxNode
					ElseIf (Not Me.TryPopNode()) Then
						visualBasicSyntaxNode = Nothing
						Return visualBasicSyntaxNode
					Else
						GoTo Label1
					End If
				Loop While Me.TryCrumbleOnce()
				visualBasicSyntaxNode = Nothing
			Else
				visualBasicSyntaxNode = Nothing
			End If
			Return visualBasicSyntaxNode
		End Function

		Friend Overrides Function GetCurrentSyntaxNode() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim currentNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			If (Me._currentNode IsNot Nothing) Then
				Dim position As Integer = Me._currentToken.Position
				Dim start As Integer = Me._affectedRange.Span.Start
				Dim textChangeRange As Microsoft.CodeAnalysis.Text.TextChangeRange = Me._affectedRange
				If (Not (New TextSpan(start, textChangeRange.NewLength)).Contains(position)) Then
					currentNode = Me.GetCurrentNode(position)
				Else
					currentNode = Nothing
				End If
			Else
				currentNode = Nothing
			End If
			Return currentNode
		End Function

		Private Shared Function IsStatementLike(ByVal node As SyntaxNode) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = node.Kind()
			If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineElseClause) Then
				If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseIfBlock) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List) AndAlso CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CatchBlock) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
					flag = TypeOf node Is Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax
					Return flag
				End If
				flag = node.GetTrailingTrivia().Any(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOfLineTrivia)
				Return flag
			Else
				If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfStatement AndAlso syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineElseClause) Then
					flag = TypeOf node Is Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax
					Return flag
				End If
				flag = False
				Return flag
			End If
			flag = TypeOf node Is Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax
			Return flag
		End Function

		Private Function MapNewPositionToOldTree(ByVal position As Integer) As Integer
			Dim num As Integer
			If (position >= Me._change.Span.Start) Then
				num = If(position < Me._change.Span.Start + Me._change.NewLength, -1, position - Me._change.NewLength + Me._change.Span.Length)
			Else
				num = position
			End If
			Return num
		End Function

		Friend Overrides Sub MoveToNextSyntaxNode()
			If (Me._currentNode IsNot Nothing) Then
				Dim preprocessorState As Scanner.PreprocessorState = Me._nextPreprocessorStateGetter.State()
				Me._lineBufferOffset = Me._currentToken.Position + Me._curNodeLength
				If (Me._currentNode.ContainsDirectives) Then
					Me._currentToken = Me._currentToken.[With](preprocessorState)
				End If
				MyBase.MoveToNextSyntaxNode()
				Me.TryPopNode()
			End If
		End Sub

		Friend Overrides Sub MoveToNextSyntaxNodeInTrivia()
			If (Me._currentNode IsNot Nothing) Then
				Me._lineBufferOffset += Me._curNodeLength
				MyBase.MoveToNextSyntaxNodeInTrivia()
				Me.TryPopNode()
			End If
		End Sub

		Private Shared Function NearestStatementThatContainsPosition(ByVal node As Microsoft.CodeAnalysis.SyntaxNode, ByVal position As Integer, ByVal rootFullSpan As Microsoft.CodeAnalysis.Text.TextSpan) As Microsoft.CodeAnalysis.Text.TextSpan
			Dim textSpan As Microsoft.CodeAnalysis.Text.TextSpan
			If (Not node.FullSpan.Contains(position)) Then
				textSpan = New Microsoft.CodeAnalysis.Text.TextSpan(position, 0)
			ElseIf (node.Kind() = SyntaxKind.CompilationUnit OrElse Blender.IsStatementLike(node)) Then
				While True
					Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = node.ChildThatContainsPosition(position).AsNode()
					If (syntaxNode Is Nothing OrElse Not Blender.IsStatementLike(syntaxNode)) Then
						Exit While
					End If
					node = syntaxNode
				End While
				textSpan = node.FullSpan
			Else
				textSpan = rootFullSpan
			End If
			Return textSpan
		End Function

		Private Shared Sub PushChildReverse(ByVal stack As Stack(Of GreenNode), ByVal child As GreenNode)
			If (child IsNot Nothing) Then
				If (child.IsList) Then
					Blender.PushReverseNonterminal(stack, child)
					Return
				End If
				stack.Push(child)
			End If
		End Sub

		Private Shared Sub PushReverseNonterminal(ByVal stack As Stack(Of GreenNode), ByVal nonterminal As GreenNode)
			Dim slotCount As Integer = nonterminal.SlotCount
			Dim num As Integer = slotCount
			For i As Integer = 1 To num
				Blender.PushChildReverse(stack, nonterminal.GetSlot(slotCount - i))
			Next

		End Sub

		Private Shared Sub PushReverseTerminal(ByVal stack As Stack(Of GreenNode), ByVal tk As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			Dim trailingTrivia As GreenNode = tk.GetTrailingTrivia()
			If (trailingTrivia IsNot Nothing) Then
				Blender.PushChildReverse(stack, trailingTrivia)
			End If
			Blender.PushChildReverse(stack, DirectCast(tk.WithLeadingTrivia(Nothing).WithTrailingTrivia(Nothing), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken))
			trailingTrivia = tk.GetLeadingTrivia()
			If (trailingTrivia IsNot Nothing) Then
				Blender.PushChildReverse(stack, trailingTrivia)
			End If
		End Sub

		Private Shared Function ShouldCrumble(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Boolean
			Dim flag As Boolean
			If (Not TypeOf node Is Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructuredTriviaSyntax) Then
				Dim kind As SyntaxKind = node.Kind
				If (kind = SyntaxKind.EnumBlock) Then
					flag = False
				Else
					flag = If(kind = SyntaxKind.SingleLineIfStatement OrElse kind = SyntaxKind.SingleLineElseClause, False, True)
				End If
			Else
				flag = False
			End If
			Return flag
		End Function

		Friend Overrides Function TryCrumbleOnce() As Boolean
			Dim flag As Boolean
			If (Me._currentNode IsNot Nothing) Then
				If (Me._currentNode.SlotCount <> 0) Then
					If (Not Blender.ShouldCrumble(Me._currentNode)) Then
						flag = False
						Return flag
					End If
					Blender.PushReverseNonterminal(Me._nodeStack, Me._currentNode)
				Else
					If (Not Me._currentNode.ContainsStructuredTrivia) Then
						flag = False
						Return flag
					End If
					Blender.PushReverseTerminal(Me._nodeStack, DirectCast(Me._currentNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken))
				End If
				Me._curNodeLength = 0
				Me._nextPreprocessorStateGetter = New Blender.NextPreprocessorStateGetter()
				flag = Me.TryPopNode()
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function TryPopNode() As Boolean
			Dim flag As Boolean
			If (Me._nodeStack.Count <= 0) Then
				Me._currentNode = Nothing
				flag = False
			Else
				Dim greenNode As Microsoft.CodeAnalysis.GreenNode = Me._nodeStack.Pop()
				Me._currentNode = DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
				Me._curNodeStart += Me._curNodeLength
				Me._curNodeLength = greenNode.FullWidth
				If (Me._nextPreprocessorStateGetter.Valid) Then
					Me._currentPreprocessorState = Me._nextPreprocessorStateGetter.State()
				End If
				Me._nextPreprocessorStateGetter = New Blender.NextPreprocessorStateGetter(Me._currentPreprocessorState, DirectCast(greenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode))
				flag = True
			End If
			Return flag
		End Function

		Private Structure NextPreprocessorStateGetter
			Private ReadOnly _state As Scanner.PreprocessorState

			Private ReadOnly _node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode

			Private _nextState As Scanner.PreprocessorState

			Public ReadOnly Property Valid As Boolean
				Get
					Return Me._node IsNot Nothing
				End Get
			End Property

			Public Sub New(ByVal state As Scanner.PreprocessorState, ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
				Me = New Blender.NextPreprocessorStateGetter() With
				{
					._state = state,
					._node = node,
					._nextState = Nothing
				}
			End Sub

			Public Function State() As Scanner.PreprocessorState
				If (Me._nextState Is Nothing) Then
					Me._nextState = Scanner.ApplyDirectives(Me._state, Me._node)
				End If
				Return Me._nextState
			End Function
		End Structure
	End Class
End Namespace