Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Friend Class SyntaxReplacer
		Public Sub New()
			MyBase.New()
		End Sub

		Private Shared Function GetItemNotListElementException() As InvalidOperationException
			Return New InvalidOperationException(CodeAnalysisResources.MissingListItem)
		End Function

		Public Shared Function InsertNodeInList(ByVal root As SyntaxNode, ByVal nodeInList As SyntaxNode, ByVal nodesToInsert As IEnumerable(Of SyntaxNode), ByVal insertBefore As Boolean) As SyntaxNode
			Return (New SyntaxReplacer.NodeListEditor(nodeInList, nodesToInsert, If(insertBefore, SyntaxReplacer.ListEditKind.InsertBefore, SyntaxReplacer.ListEditKind.InsertAfter))).Visit(root)
		End Function

		Public Shared Function InsertTokenInList(ByVal root As SyntaxNode, ByVal tokenInList As SyntaxToken, ByVal newTokens As IEnumerable(Of SyntaxToken), ByVal insertBefore As Boolean) As SyntaxNode
			Return (New SyntaxReplacer.TokenListEditor(tokenInList, newTokens, If(insertBefore, SyntaxReplacer.ListEditKind.InsertBefore, SyntaxReplacer.ListEditKind.InsertAfter))).Visit(root)
		End Function

		Public Shared Function InsertTriviaInList(ByVal root As SyntaxNode, ByVal triviaInList As SyntaxTrivia, ByVal newTrivia As IEnumerable(Of SyntaxTrivia), ByVal insertBefore As Boolean) As SyntaxNode
			Return (New SyntaxReplacer.TriviaListEditor(triviaInList, newTrivia, If(insertBefore, SyntaxReplacer.ListEditKind.InsertBefore, SyntaxReplacer.ListEditKind.InsertAfter))).Visit(root)
		End Function

		Public Shared Function InsertTriviaInList(ByVal root As SyntaxToken, ByVal triviaInList As SyntaxTrivia, ByVal newTrivia As IEnumerable(Of SyntaxTrivia), ByVal insertBefore As Boolean) As SyntaxToken
			Return (New SyntaxReplacer.TriviaListEditor(triviaInList, newTrivia, If(insertBefore, SyntaxReplacer.ListEditKind.InsertBefore, SyntaxReplacer.ListEditKind.InsertAfter))).VisitToken(root)
		End Function

		Friend Shared Function Replace(Of TNode As Microsoft.CodeAnalysis.SyntaxNode)(ByVal root As Microsoft.CodeAnalysis.SyntaxNode, Optional ByVal nodes As IEnumerable(Of TNode) = Nothing, Optional ByVal computeReplacementNode As Func(Of TNode, TNode, Microsoft.CodeAnalysis.SyntaxNode) = Nothing, Optional ByVal tokens As IEnumerable(Of SyntaxToken) = Nothing, Optional ByVal computeReplacementToken As Func(Of SyntaxToken, SyntaxToken, SyntaxToken) = Nothing, Optional ByVal trivia As IEnumerable(Of SyntaxTrivia) = Nothing, Optional ByVal computeReplacementTrivia As Func(Of SyntaxTrivia, SyntaxTrivia, SyntaxTrivia) = Nothing) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim replacer As SyntaxReplacer.Replacer(Of TNode) = New SyntaxReplacer.Replacer(Of TNode)(nodes, computeReplacementNode, tokens, computeReplacementToken, trivia, computeReplacementTrivia)
			syntaxNode = If(Not replacer.HasWork, root, replacer.Visit(root))
			Return syntaxNode
		End Function

		Friend Shared Function Replace(ByVal root As Microsoft.CodeAnalysis.SyntaxToken, Optional ByVal nodes As IEnumerable(Of SyntaxNode) = Nothing, Optional ByVal computeReplacementNode As Func(Of SyntaxNode, SyntaxNode, SyntaxNode) = Nothing, Optional ByVal tokens As IEnumerable(Of Microsoft.CodeAnalysis.SyntaxToken) = Nothing, Optional ByVal computeReplacementToken As Func(Of Microsoft.CodeAnalysis.SyntaxToken, Microsoft.CodeAnalysis.SyntaxToken, Microsoft.CodeAnalysis.SyntaxToken) = Nothing, Optional ByVal trivia As IEnumerable(Of SyntaxTrivia) = Nothing, Optional ByVal computeReplacementTrivia As Func(Of SyntaxTrivia, SyntaxTrivia, SyntaxTrivia) = Nothing) As Microsoft.CodeAnalysis.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken
			Dim replacer As SyntaxReplacer.Replacer(Of SyntaxNode) = New SyntaxReplacer.Replacer(Of SyntaxNode)(nodes, computeReplacementNode, tokens, computeReplacementToken, trivia, computeReplacementTrivia)
			syntaxToken = If(Not replacer.HasWork, root, replacer.VisitToken(root))
			Return syntaxToken
		End Function

		Public Shared Function ReplaceNodeInList(ByVal root As SyntaxNode, ByVal originalNode As SyntaxNode, ByVal newNodes As IEnumerable(Of SyntaxNode)) As SyntaxNode
			Return (New SyntaxReplacer.NodeListEditor(originalNode, newNodes, SyntaxReplacer.ListEditKind.Replace)).Visit(root)
		End Function

		Public Shared Function ReplaceTokenInList(ByVal root As SyntaxNode, ByVal tokenInList As SyntaxToken, ByVal newTokens As IEnumerable(Of SyntaxToken)) As SyntaxNode
			Return (New SyntaxReplacer.TokenListEditor(tokenInList, newTokens, SyntaxReplacer.ListEditKind.Replace)).Visit(root)
		End Function

		Public Shared Function ReplaceTriviaInList(ByVal root As SyntaxNode, ByVal triviaInList As SyntaxTrivia, ByVal newTrivia As IEnumerable(Of SyntaxTrivia)) As SyntaxNode
			Return (New SyntaxReplacer.TriviaListEditor(triviaInList, newTrivia, SyntaxReplacer.ListEditKind.Replace)).Visit(root)
		End Function

		Public Shared Function ReplaceTriviaInList(ByVal root As SyntaxToken, ByVal triviaInList As SyntaxTrivia, ByVal newTrivia As IEnumerable(Of SyntaxTrivia)) As SyntaxToken
			Return (New SyntaxReplacer.TriviaListEditor(triviaInList, newTrivia, SyntaxReplacer.ListEditKind.Replace)).VisitToken(root)
		End Function

		Private Class BaseListEditor
			Inherits VisualBasicSyntaxRewriter
			Private ReadOnly _elementSpan As TextSpan

			Protected ReadOnly _editKind As SyntaxReplacer.ListEditKind

			Private ReadOnly _visitTrivia As Boolean

			Private ReadOnly _visitIntoStructuredTrivia As Boolean

			Public Overrides ReadOnly Property VisitIntoStructuredTrivia As Boolean
				Get
					Return Me._visitIntoStructuredTrivia
				End Get
			End Property

			Public Sub New(ByVal elementSpan As TextSpan, ByVal editKind As SyntaxReplacer.ListEditKind, ByVal visitTrivia As Boolean, ByVal visitIntoStructuredTrivia As Boolean)
				MyBase.New(False)
				Me._elementSpan = elementSpan
				Me._editKind = editKind
				Me._visitTrivia = visitTrivia Or visitIntoStructuredTrivia
				Me._visitIntoStructuredTrivia = visitIntoStructuredTrivia
			End Sub

			Private Function ShouldVisit(ByVal span As TextSpan) As Boolean
				Return span.IntersectsWith(Me._elementSpan)
			End Function

			Public Overrides Function Visit(ByVal node As Microsoft.CodeAnalysis.SyntaxNode) As Microsoft.CodeAnalysis.SyntaxNode
				Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = node
				If (node IsNot Nothing AndAlso Me.ShouldVisit(node.FullSpan)) Then
					syntaxNode = MyBase.Visit(node)
				End If
				Return syntaxNode
			End Function

			Public Overrides Function VisitListElement(ByVal element As SyntaxTrivia) As SyntaxTrivia
				Dim syntaxTrivium As SyntaxTrivia = element
				If (Me._visitIntoStructuredTrivia AndAlso element.HasStructure AndAlso Me.ShouldVisit(element.FullSpan)) Then
					syntaxTrivium = MyBase.VisitTrivia(element)
				End If
				Return syntaxTrivium
			End Function

			Public Overrides Function VisitToken(ByVal token As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.SyntaxToken
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = token
				If (Me._visitTrivia AndAlso Me.ShouldVisit(token.FullSpan)) Then
					syntaxToken = MyBase.VisitToken(token)
				End If
				Return syntaxToken
			End Function
		End Class

		Private Enum ListEditKind
			InsertBefore
			InsertAfter
			Replace
		End Enum

		Private Class NodeListEditor
			Inherits SyntaxReplacer.BaseListEditor
			Private ReadOnly _originalNode As SyntaxNode

			Private ReadOnly _replacementNodes As IEnumerable(Of SyntaxNode)

			Public Sub New(ByVal originalNode As SyntaxNode, ByVal replacementNodes As IEnumerable(Of SyntaxNode), ByVal editKind As SyntaxReplacer.ListEditKind)
				MyBase.New(originalNode.FullSpan, editKind, False, originalNode.IsPartOfStructuredTrivia())
				Me._originalNode = originalNode
				Me._replacementNodes = replacementNodes
			End Sub

			Public Overrides Function Visit(ByVal node As SyntaxNode) As SyntaxNode
				If (node = Me._originalNode) Then
					Throw SyntaxReplacer.GetItemNotListElementException()
				End If
				Return MyBase.Visit(node)
			End Function

			Public Overrides Function VisitList(Of TNode As SyntaxNode)(ByVal list As SeparatedSyntaxList(Of TNode)) As SeparatedSyntaxList(Of TNode)
				Dim tNodes As SeparatedSyntaxList(Of TNode)
				If (TypeOf Me._originalNode Is TNode) Then
					Dim num As Integer = list.IndexOf(DirectCast(Me._originalNode, TNode))
					If (num >= 0 AndAlso num < list.Count) Then
						Select Case Me._editKind
							Case SyntaxReplacer.ListEditKind.InsertBefore
								tNodes = list.InsertRange(num, Me._replacementNodes.Cast(Of TNode)())
								Return tNodes
							Case SyntaxReplacer.ListEditKind.InsertAfter
								tNodes = list.InsertRange(num + 1, Me._replacementNodes.Cast(Of TNode)())
								Return tNodes
							Case SyntaxReplacer.ListEditKind.Replace
								tNodes = list.ReplaceRange(DirectCast(Me._originalNode, TNode), Me._replacementNodes.Cast(Of TNode)())
								Return tNodes
						End Select
					End If
				End If
				tNodes = MyBase.VisitList(Of TNode)(list)
				Return tNodes
			End Function

			Public Overrides Function VisitList(Of TNode As SyntaxNode)(ByVal list As SyntaxList(Of TNode)) As SyntaxList(Of TNode)
				Dim tNodes As SyntaxList(Of TNode)
				If (TypeOf Me._originalNode Is TNode) Then
					Dim num As Integer = list.IndexOf(DirectCast(Me._originalNode, TNode))
					If (num >= 0 AndAlso num < list.Count) Then
						Select Case Me._editKind
							Case SyntaxReplacer.ListEditKind.InsertBefore
								tNodes = list.InsertRange(num, Me._replacementNodes.Cast(Of TNode)())
								Return tNodes
							Case SyntaxReplacer.ListEditKind.InsertAfter
								tNodes = list.InsertRange(num + 1, Me._replacementNodes.Cast(Of TNode)())
								Return tNodes
							Case SyntaxReplacer.ListEditKind.Replace
								tNodes = list.ReplaceRange(DirectCast(Me._originalNode, TNode), Me._replacementNodes.Cast(Of TNode)())
								Return tNodes
						End Select
					End If
				End If
				tNodes = MyBase.VisitList(Of TNode)(list)
				Return tNodes
			End Function
		End Class

		Private Class Replacer(Of TNode As SyntaxNode)
			Inherits VisualBasicSyntaxRewriter
			Private ReadOnly _computeReplacementNode As Func(Of TNode, TNode, SyntaxNode)

			Private ReadOnly _computeReplacementToken As Func(Of SyntaxToken, SyntaxToken, SyntaxToken)

			Private ReadOnly _computeReplacementTrivia As Func(Of SyntaxTrivia, SyntaxTrivia, SyntaxTrivia)

			Private ReadOnly _nodeSet As HashSet(Of SyntaxNode)

			Private ReadOnly _tokenSet As HashSet(Of SyntaxToken)

			Private ReadOnly _triviaSet As HashSet(Of SyntaxTrivia)

			Private ReadOnly _spanSet As HashSet(Of TextSpan)

			Private ReadOnly _totalSpan As TextSpan

			Private ReadOnly _visitStructuredTrivia As Boolean

			Private ReadOnly _shouldVisitTrivia As Boolean

			Private ReadOnly Shared s_noNodes As HashSet(Of SyntaxNode)

			Private ReadOnly Shared s_noTokens As HashSet(Of SyntaxToken)

			Private ReadOnly Shared s_noTrivia As HashSet(Of SyntaxTrivia)

			Public ReadOnly Property HasWork As Boolean
				Get
					Return Me._nodeSet.Count + Me._tokenSet.Count + Me._triviaSet.Count > 0
				End Get
			End Property

			Public Overrides ReadOnly Property VisitIntoStructuredTrivia As Boolean
				Get
					Return Me._visitStructuredTrivia
				End Get
			End Property

			Shared Sub New()
				SyntaxReplacer.Replacer(Of TNode).s_noNodes = New HashSet(Of SyntaxNode)()
				SyntaxReplacer.Replacer(Of TNode).s_noTokens = New HashSet(Of SyntaxToken)()
				SyntaxReplacer.Replacer(Of TNode).s_noTrivia = New HashSet(Of SyntaxTrivia)()
			End Sub

			Public Sub New(ByVal nodes As IEnumerable(Of TNode), ByVal computeReplacementNode As Func(Of TNode, TNode, SyntaxNode), ByVal tokens As IEnumerable(Of SyntaxToken), ByVal computeReplacementToken As Func(Of SyntaxToken, SyntaxToken, SyntaxToken), ByVal trivia As IEnumerable(Of SyntaxTrivia), ByVal computeReplacementTrivia As Func(Of SyntaxTrivia, SyntaxTrivia, SyntaxTrivia))
				MyBase.New(False)
				Dim fullSpan As Func(Of SyntaxNode, TextSpan)
				Dim func As Func(Of SyntaxToken, TextSpan)
				Dim fullSpan1 As Func(Of SyntaxTrivia, TextSpan)
				Dim func1 As Func(Of SyntaxNode, Boolean)
				Dim flag As Boolean
				Dim flag1 As Boolean
				Dim func2 As Func(Of SyntaxToken, Boolean)
				Dim func3 As Func(Of SyntaxTrivia, Boolean)
				Me._computeReplacementNode = computeReplacementNode
				Me._computeReplacementToken = computeReplacementToken
				Me._computeReplacementTrivia = computeReplacementTrivia
				Me._nodeSet = If(nodes IsNot Nothing, New HashSet(Of SyntaxNode)(nodes), SyntaxReplacer.Replacer(Of TNode).s_noNodes)
				Me._tokenSet = If(tokens IsNot Nothing, New HashSet(Of SyntaxToken)(tokens), SyntaxReplacer.Replacer(Of TNode).s_noTokens)
				Me._triviaSet = If(trivia IsNot Nothing, New HashSet(Of SyntaxTrivia)(trivia), SyntaxReplacer.Replacer(Of TNode).s_noTrivia)
				Dim syntaxNodes As HashSet(Of SyntaxNode) = Me._nodeSet
				If (SyntaxReplacer.Replacer(Of TNode)._Closure$__.$I11-0 Is Nothing) Then
					fullSpan = Function(n As SyntaxNode) n.FullSpan
					SyntaxReplacer.Replacer(Of TNode)._Closure$__.$I11-0 = fullSpan
				Else
					fullSpan = SyntaxReplacer.Replacer(Of TNode)._Closure$__.$I11-0
				End If
				Dim textSpans As IEnumerable(Of TextSpan) = syntaxNodes.[Select](Of TextSpan)(fullSpan)
				Dim syntaxTokens As HashSet(Of SyntaxToken) = Me._tokenSet
				If (SyntaxReplacer.Replacer(Of TNode)._Closure$__.$I11-1 Is Nothing) Then
					func = Function(t As SyntaxToken) t.FullSpan
					SyntaxReplacer.Replacer(Of TNode)._Closure$__.$I11-1 = func
				Else
					func = SyntaxReplacer.Replacer(Of TNode)._Closure$__.$I11-1
				End If
				Dim textSpans1 As IEnumerable(Of TextSpan) = textSpans.Concat(syntaxTokens.[Select](Of TextSpan)(func))
				Dim syntaxTrivias As HashSet(Of SyntaxTrivia) = Me._triviaSet
				If (SyntaxReplacer.Replacer(Of TNode)._Closure$__.$I11-2 Is Nothing) Then
					fullSpan1 = Function(t As SyntaxTrivia) t.FullSpan
					SyntaxReplacer.Replacer(Of TNode)._Closure$__.$I11-2 = fullSpan1
				Else
					fullSpan1 = SyntaxReplacer.Replacer(Of TNode)._Closure$__.$I11-2
				End If
				Me._spanSet = New HashSet(Of TextSpan)(textSpans1.Concat(syntaxTrivias.[Select](Of TextSpan)(fullSpan1)))
				Me._totalSpan = SyntaxReplacer.Replacer(Of TNode).ComputeTotalSpan(Me._spanSet)
				Dim syntaxNodes1 As HashSet(Of SyntaxNode) = Me._nodeSet
				If (SyntaxReplacer.Replacer(Of TNode)._Closure$__.$I11-3 Is Nothing) Then
					func1 = Function(n As SyntaxNode) n.IsPartOfStructuredTrivia()
					SyntaxReplacer.Replacer(Of TNode)._Closure$__.$I11-3 = func1
				Else
					func1 = SyntaxReplacer.Replacer(Of TNode)._Closure$__.$I11-3
				End If
				If (Not syntaxNodes1.Any(func1)) Then
					Dim syntaxTokens1 As HashSet(Of SyntaxToken) = Me._tokenSet
					If (SyntaxReplacer.Replacer(Of TNode)._Closure$__.$I11-4 Is Nothing) Then
						func2 = Function(t As SyntaxToken) t.IsPartOfStructuredTrivia()
						SyntaxReplacer.Replacer(Of TNode)._Closure$__.$I11-4 = func2
					Else
						func2 = SyntaxReplacer.Replacer(Of TNode)._Closure$__.$I11-4
					End If
					If (syntaxTokens1.Any(func2)) Then
						flag = True
						Me._visitStructuredTrivia = flag
						flag1 = If(Me._triviaSet.Count > 0, True, Me._visitStructuredTrivia)
						Me._shouldVisitTrivia = flag1
						Return
					End If
					Dim syntaxTrivias1 As HashSet(Of SyntaxTrivia) = Me._triviaSet
					If (SyntaxReplacer.Replacer(Of TNode)._Closure$__.$I11-5 Is Nothing) Then
						func3 = Function(t As SyntaxTrivia) t.IsPartOfStructuredTrivia()
						SyntaxReplacer.Replacer(Of TNode)._Closure$__.$I11-5 = func3
					Else
						func3 = SyntaxReplacer.Replacer(Of TNode)._Closure$__.$I11-5
					End If
					flag = syntaxTrivias1.Any(func3)
					Me._visitStructuredTrivia = flag
					flag1 = If(Me._triviaSet.Count > 0, True, Me._visitStructuredTrivia)
					Me._shouldVisitTrivia = flag1
					Return
				End If
				flag = True
				Me._visitStructuredTrivia = flag
				flag1 = If(Me._triviaSet.Count > 0, True, Me._visitStructuredTrivia)
				Me._shouldVisitTrivia = flag1
			End Sub

			Private Shared Function ComputeTotalSpan(ByVal spans As IEnumerable(Of TextSpan)) As TextSpan
				Dim enumerator As IEnumerator(Of TextSpan) = Nothing
				Dim flag As Boolean = True
				Dim start As Integer = 0
				Using [end] As Integer = 0
					enumerator = spans.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As TextSpan = enumerator.Current
						If (Not flag) Then
							start = Math.Min(start, current.Start)
							[end] = Math.Max([end], current.[End])
						Else
							flag = False
							start = current.Start
							[end] = current.[End]
						End If
					End While
				End Using
				Return New TextSpan(start, [end] - start)
			End Function

			Private Function ShouldVisit(ByVal span As TextSpan) As Boolean
				Dim flag As Boolean
				Dim enumerator As HashSet(Of TextSpan).Enumerator = New HashSet(Of TextSpan).Enumerator()
				If (span.IntersectsWith(Me._totalSpan)) Then
					Try
						enumerator = Me._spanSet.GetEnumerator()
						While enumerator.MoveNext()
							If (Not span.IntersectsWith(enumerator.Current)) Then
								Continue While
							End If
							flag = True
							Return flag
						End While
					Finally
						DirectCast(enumerator, IDisposable).Dispose()
					End Try
					flag = False
				Else
					flag = False
				End If
				Return flag
			End Function

			Public Overrides Function Visit(ByVal node As Microsoft.CodeAnalysis.SyntaxNode) As Microsoft.CodeAnalysis.SyntaxNode
				Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = node
				If (node IsNot Nothing) Then
					If (Me.ShouldVisit(node.FullSpan)) Then
						syntaxNode = MyBase.Visit(node)
					End If
					If (Me._nodeSet.Contains(node) AndAlso Me._computeReplacementNode IsNot Nothing) Then
						syntaxNode = Me._computeReplacementNode(DirectCast(node, TNode), DirectCast(syntaxNode, TNode))
					End If
				End If
				Return syntaxNode
			End Function

			Public Overrides Function VisitListElement(ByVal trivia As SyntaxTrivia) As SyntaxTrivia
				Dim syntaxTrivium As SyntaxTrivia = trivia
				If (Me.VisitIntoStructuredTrivia AndAlso trivia.HasStructure AndAlso Me.ShouldVisit(trivia.FullSpan)) Then
					syntaxTrivium = Me.VisitTrivia(trivia)
				End If
				If (Me._triviaSet.Contains(trivia) AndAlso Me._computeReplacementTrivia IsNot Nothing) Then
					syntaxTrivium = Me._computeReplacementTrivia(trivia, syntaxTrivium)
				End If
				Return syntaxTrivium
			End Function

			Public Overrides Function VisitToken(ByVal token As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.SyntaxToken
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = token
				If (Me._shouldVisitTrivia AndAlso Me.ShouldVisit(token.FullSpan)) Then
					syntaxToken = MyBase.VisitToken(token)
				End If
				If (Me._tokenSet.Contains(token) AndAlso Me._computeReplacementToken IsNot Nothing) Then
					syntaxToken = Me._computeReplacementToken(token, syntaxToken)
				End If
				Return syntaxToken
			End Function
		End Class

		Private Class TokenListEditor
			Inherits SyntaxReplacer.BaseListEditor
			Private ReadOnly _originalToken As SyntaxToken

			Private ReadOnly _newTokens As IEnumerable(Of SyntaxToken)

			Public Sub New(ByVal originalToken As SyntaxToken, ByVal newTokens As IEnumerable(Of SyntaxToken), ByVal editKind As SyntaxReplacer.ListEditKind)
				MyBase.New(originalToken.FullSpan, editKind, False, originalToken.IsPartOfStructuredTrivia())
				Me._originalToken = originalToken
				Me._newTokens = newTokens
			End Sub

			Public Overrides Function VisitList(ByVal list As SyntaxTokenList) As SyntaxTokenList
				Dim syntaxTokenLists As SyntaxTokenList
				Dim num As Integer = list.IndexOf(Me._originalToken)
				If (num >= 0 AndAlso num < list.Count) Then
					Select Case Me._editKind
						Case SyntaxReplacer.ListEditKind.InsertBefore
							syntaxTokenLists = list.InsertRange(num, Me._newTokens)
							Return syntaxTokenLists
						Case SyntaxReplacer.ListEditKind.InsertAfter
							syntaxTokenLists = list.InsertRange(num + 1, Me._newTokens)
							Return syntaxTokenLists
						Case SyntaxReplacer.ListEditKind.Replace
							syntaxTokenLists = list.ReplaceRange(Me._originalToken, Me._newTokens)
							Return syntaxTokenLists
					End Select
				End If
				syntaxTokenLists = MyBase.VisitList(list)
				Return syntaxTokenLists
			End Function

			Public Overrides Function VisitToken(ByVal token As SyntaxToken) As SyntaxToken
				If (token = Me._originalToken) Then
					Throw SyntaxReplacer.GetItemNotListElementException()
				End If
				Return MyBase.VisitToken(token)
			End Function
		End Class

		Private Class TriviaListEditor
			Inherits SyntaxReplacer.BaseListEditor
			Private ReadOnly _originalTrivia As SyntaxTrivia

			Private ReadOnly _newTrivia As IEnumerable(Of SyntaxTrivia)

			Public Sub New(ByVal originalTrivia As SyntaxTrivia, ByVal newTrivia As IEnumerable(Of SyntaxTrivia), ByVal editKind As SyntaxReplacer.ListEditKind)
				MyBase.New(originalTrivia.FullSpan, editKind, True, originalTrivia.IsPartOfStructuredTrivia())
				Me._originalTrivia = originalTrivia
				Me._newTrivia = newTrivia
			End Sub

			Public Overrides Function VisitList(ByVal list As SyntaxTriviaList) As SyntaxTriviaList
				Dim syntaxTriviaLists As SyntaxTriviaList
				Dim num As Integer = list.IndexOf(Me._originalTrivia)
				If (num >= 0 AndAlso num < list.Count) Then
					Select Case Me._editKind
						Case SyntaxReplacer.ListEditKind.InsertBefore
							syntaxTriviaLists = list.InsertRange(num, Me._newTrivia)
							Return syntaxTriviaLists
						Case SyntaxReplacer.ListEditKind.InsertAfter
							syntaxTriviaLists = list.InsertRange(num + 1, Me._newTrivia)
							Return syntaxTriviaLists
						Case SyntaxReplacer.ListEditKind.Replace
							syntaxTriviaLists = list.ReplaceRange(Me._originalTrivia, Me._newTrivia)
							Return syntaxTriviaLists
					End Select
				End If
				syntaxTriviaLists = MyBase.VisitList(list)
				Return syntaxTriviaLists
			End Function
		End Class
	End Class
End Namespace