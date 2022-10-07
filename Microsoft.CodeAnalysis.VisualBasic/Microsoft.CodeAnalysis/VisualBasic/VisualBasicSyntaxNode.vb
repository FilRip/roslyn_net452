Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.IO
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public MustInherit Class VisualBasicSyntaxNode
		Inherits SyntaxNode
		Friend Shared EmptyErrorCollection As ReadOnlyCollection(Of Diagnostic)

		Public ReadOnly Property IsDirective As Boolean
			Get
				Return MyBase.Green.IsDirective
			End Get
		End Property

		Public Overrides ReadOnly Property Language As String
			Get
				Return "Visual Basic"
			End Get
		End Property

		Friend Shadows ReadOnly Property Parent As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
			Get
				Return DirectCast(MyBase.Parent, Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)
			End Get
		End Property

		Public Shadows ReadOnly Property SpanStart As Integer
			Get
				Return MyBase.Position + MyBase.Green.GetLeadingTriviaWidth()
			End Get
		End Property

		Friend Shadows ReadOnly Property SyntaxTree As Microsoft.CodeAnalysis.SyntaxTree
			Get
				If (Me._syntaxTree Is Nothing) Then
					Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.SyntaxNode) = ArrayBuilder(Of Microsoft.CodeAnalysis.SyntaxNode).GetInstance()
					Dim syntaxTree1 As Microsoft.CodeAnalysis.SyntaxTree = Nothing
					Dim parent As Microsoft.CodeAnalysis.SyntaxNode = Me
					Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = Nothing
					While parent IsNot Nothing
						syntaxTree1 = parent._syntaxTree
						If (syntaxTree1 IsNot Nothing) Then
							Exit While
						End If
						syntaxNode = parent
						instance.Push(parent)
						parent = syntaxNode.Parent
					End While
					If (syntaxTree1 Is Nothing) Then
						syntaxTree1 = VisualBasicSyntaxTree.CreateWithoutClone(DirectCast(syntaxNode, Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode))
					End If
					While instance.Count > 0
						Dim syntaxTree2 As Microsoft.CodeAnalysis.SyntaxTree = Interlocked.CompareExchange(Of Microsoft.CodeAnalysis.SyntaxTree)(instance.Pop()._syntaxTree, syntaxTree1, Nothing)
						If (syntaxTree2 Is Nothing) Then
							Continue While
						End If
						syntaxTree1 = syntaxTree2
					End While
					instance.Free()
				End If
				Return Me._syntaxTree
			End Get
		End Property

		Protected Overrides ReadOnly Property SyntaxTreeCore As Microsoft.CodeAnalysis.SyntaxTree
			Get
				Return Me.SyntaxTree
			End Get
		End Property

		Friend ReadOnly Property VbGreen As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Get
				Return DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode.EmptyErrorCollection = New ReadOnlyCollection(Of Diagnostic)(Array.Empty(Of Diagnostic)())
		End Sub

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal position As Integer)
			MyBase.New(green, parent, position)
		End Sub

		Friend Sub New(ByVal green As GreenNode, ByVal position As Integer, ByVal syntaxTree As Microsoft.CodeAnalysis.SyntaxTree)
			MyBase.New(green, Nothing, position)
			Me._syntaxTree = syntaxTree
		End Sub

		Public MustOverride Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult

		Public MustOverride Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)

		Friend Function AddError(ByVal err As DiagnosticInfo) As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
			Dim diagnostics As DiagnosticInfo()
			If (MyBase.Green.GetDiagnostics() IsNot Nothing) Then
				diagnostics = MyBase.Green.GetDiagnostics()
				Dim length As Integer = CInt(diagnostics.Length)
				diagnostics = DirectCast(Utils.CopyArray(diagnostics, New DiagnosticInfo(length + 1 - 1) {}), DiagnosticInfo())
				diagnostics(length) = err
			Else
				diagnostics = New DiagnosticInfo() { err }
			End If
			Return DirectCast(MyBase.Green.SetDiagnostics(diagnostics).CreateRed(Nothing, 0), Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)
		End Function

		Private Shared Function CreateSyntaxError(ByVal tree As Microsoft.CodeAnalysis.SyntaxTree, ByVal nodeOrToken As SyntaxNodeOrToken, ByVal errorInfo As Microsoft.CodeAnalysis.DiagnosticInfo) As Diagnostic
			Dim sourceLocation As Microsoft.CodeAnalysis.Location
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = errorInfo
			If (tree Is Nothing) Then
				sourceLocation = New Microsoft.CodeAnalysis.SourceLocation(tree, nodeOrToken.Span)
			Else
				sourceLocation = tree.GetLocation(nodeOrToken.Span)
			End If
			Return New VBDiagnostic(diagnosticInfo, sourceLocation, False)
		End Function

		Private Shared Function CreateSyntaxError(ByVal tree As Microsoft.CodeAnalysis.SyntaxTree, ByVal nodeOrToken As Microsoft.CodeAnalysis.SyntaxTrivia, ByVal errorInfo As Microsoft.CodeAnalysis.DiagnosticInfo) As Diagnostic
			Dim sourceLocation As Microsoft.CodeAnalysis.Location
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = errorInfo
			If (tree Is Nothing) Then
				sourceLocation = New Microsoft.CodeAnalysis.SourceLocation(tree, nodeOrToken.Span)
			Else
				sourceLocation = tree.GetLocation(nodeOrToken.Span)
			End If
			Return New VBDiagnostic(diagnosticInfo, sourceLocation, False)
		End Function

		Public Shared Function DeserializeFrom(ByVal stream As System.IO.Stream, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (stream Is Nothing) Then
				Throw New ArgumentNullException("stream")
			End If
			If (Not stream.CanRead) Then
				Throw New InvalidOperationException(CodeAnalysisResources.TheStreamCannotBeReadFrom)
			End If
			Using objectReader As Roslyn.Utilities.ObjectReader = Roslyn.Utilities.ObjectReader.TryGetReader(stream, True, cancellationToken)
				If (objectReader Is Nothing) Then
					Throw New ArgumentException(CodeAnalysisResources.Stream_contains_invalid_data, "stream")
				End If
				syntaxNode = DirectCast(objectReader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode).CreateRed(Nothing, 0)
			End Using
			Return syntaxNode
		End Function

		Friend Shared Function DoGetSyntaxErrors(ByVal tree As Microsoft.CodeAnalysis.SyntaxTree, ByVal nodeOrToken As SyntaxNodeOrToken) As ReadOnlyCollection(Of Diagnostic)
			Dim diagnostics As ReadOnlyCollection(Of Diagnostic)
			If (nodeOrToken.ContainsDiagnostics) Then
				Dim syntaxNodeOrTokens As Stack(Of SyntaxNodeOrToken) = New Stack(Of SyntaxNodeOrToken)()
				Dim diagnostics1 As List(Of Diagnostic) = New List(Of Diagnostic)()
				syntaxNodeOrTokens.Push(nodeOrToken)
				While syntaxNodeOrTokens.Count > 0
					nodeOrToken = syntaxNodeOrTokens.Pop()
					Dim underlyingNode As GreenNode = nodeOrToken.UnderlyingNode
					If (underlyingNode.ContainsDiagnostics) Then
						Dim diagnosticInfoArray As Microsoft.CodeAnalysis.DiagnosticInfo() = DirectCast(underlyingNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode).GetDiagnostics()
						If (diagnosticInfoArray IsNot Nothing) Then
							Dim length As Integer = CInt(diagnosticInfoArray.Length) - 1
							For i As Integer = 0 To length
								Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = diagnosticInfoArray(i)
								diagnostics1.Add(Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode.CreateSyntaxError(tree, nodeOrToken, diagnosticInfo))
							Next

						End If
					End If
					If (nodeOrToken.IsToken) Then
						If (Not nodeOrToken.IsToken) Then
							Continue While
						End If
						Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode.ProcessTrivia(tree, diagnostics1, syntaxNodeOrTokens, nodeOrToken.GetLeadingTrivia())
						Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode.ProcessTrivia(tree, diagnostics1, syntaxNodeOrTokens, nodeOrToken.GetTrailingTrivia())
					Else
						Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode.PushNodesWithErrors(syntaxNodeOrTokens, nodeOrToken.ChildNodesAndTokens())
					End If
				End While
				diagnostics = New ReadOnlyCollection(Of Diagnostic)(diagnostics1)
			Else
				diagnostics = Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode.EmptyErrorCollection
			End If
			Return diagnostics
		End Function

		Public Shadows Function FindToken(ByVal position As Integer, Optional ByVal findInsideTrivia As Boolean = False) As Microsoft.CodeAnalysis.SyntaxToken
			Return MyBase.FindToken(position, findInsideTrivia)
		End Function

		Public Shadows Function FindTrivia(ByVal textPosition As Integer, Optional ByVal findInsideTrivia As Boolean = False) As Microsoft.CodeAnalysis.SyntaxTrivia
			Return MyBase.FindTrivia(textPosition, findInsideTrivia)
		End Function

		Public Shadows Function GetDiagnostics() As IEnumerable(Of Diagnostic)
			Return Me.SyntaxTree.GetDiagnostics(Me)
		End Function

		Public Function GetDirectives(Optional ByVal filter As Func(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax, Boolean) = Nothing) As IList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax)
			Return Me.GetDirectives(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax)(filter)
		End Function

		Public Function GetFirstDirective(Optional ByVal predicate As Func(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax, Boolean) = Nothing) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
			Dim directiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
			Dim enumerator As ChildSyntaxList.Enumerator = MyBase.ChildNodesAndTokens().GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As SyntaxNodeOrToken = enumerator.Current
					If (current.ContainsDirectives) Then
						If (Not current.IsNode) Then
							Dim enumerator1 As SyntaxTriviaList.Enumerator = current.AsToken().LeadingTrivia.GetEnumerator()
							While enumerator1.MoveNext()
								Dim syntaxTrivium As Microsoft.CodeAnalysis.SyntaxTrivia = enumerator1.Current
								If (Not syntaxTrivium.IsDirective) Then
									Continue While
								End If
								Dim [structure] As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax = DirectCast(syntaxTrivium.GetStructure(), Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax)
								If (predicate IsNot Nothing AndAlso Not predicate([structure])) Then
									Continue While
								End If
								directiveTriviaSyntax = [structure]
								Return directiveTriviaSyntax
							End While
						Else
							Dim firstDirective As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax = DirectCast(current.AsNode(), Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode).GetFirstDirective(predicate)
							If (firstDirective IsNot Nothing) Then
								directiveTriviaSyntax = firstDirective
								Exit While
							End If
						End If
					End If
				Else
					directiveTriviaSyntax = Nothing
					Exit While
				End If
			End While
			Return directiveTriviaSyntax
		End Function

		Public Shadows Function GetFirstToken(Optional ByVal includeZeroWidth As Boolean = False, Optional ByVal includeSkipped As Boolean = False, Optional ByVal includeDirectives As Boolean = False, Optional ByVal includeDocumentationComments As Boolean = False) As Microsoft.CodeAnalysis.SyntaxToken
			Return MyBase.GetFirstToken(includeZeroWidth, includeSkipped, includeDirectives, includeDocumentationComments)
		End Function

		Public Function GetLastDirective(Optional ByVal predicate As Func(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax, Boolean) = Nothing) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
			Dim directiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
			Dim enumerator As ChildSyntaxList.Reversed.Enumerator = MyBase.ChildNodesAndTokens().Reverse().GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As SyntaxNodeOrToken = enumerator.Current
					If (current.ContainsDirectives) Then
						If (Not current.IsNode) Then
							Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = current.AsToken()
							Dim enumerator1 As SyntaxTriviaList.Reversed.Enumerator = syntaxToken.LeadingTrivia.Reverse().GetEnumerator()
							While enumerator1.MoveNext()
								Dim syntaxTrivium As Microsoft.CodeAnalysis.SyntaxTrivia = enumerator1.Current
								If (Not syntaxTrivium.IsDirective) Then
									Continue While
								End If
								Dim [structure] As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax = DirectCast(syntaxTrivium.GetStructure(), Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax)
								If (predicate IsNot Nothing AndAlso Not predicate([structure])) Then
									Continue While
								End If
								directiveTriviaSyntax = [structure]
								Return directiveTriviaSyntax
							End While
						Else
							Dim lastDirective As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax = DirectCast(current.AsNode(), Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode).GetLastDirective(predicate)
							If (lastDirective IsNot Nothing) Then
								directiveTriviaSyntax = lastDirective
								Exit While
							End If
						End If
					End If
				Else
					directiveTriviaSyntax = Nothing
					Exit While
				End If
			End While
			Return directiveTriviaSyntax
		End Function

		Public Shadows Function GetLastToken(Optional ByVal includeZeroWidth As Boolean = False, Optional ByVal includeSkipped As Boolean = False, Optional ByVal includeDirectives As Boolean = False, Optional ByVal includeDocumentationComments As Boolean = False) As Microsoft.CodeAnalysis.SyntaxToken
			Return MyBase.GetLastToken(includeZeroWidth, includeSkipped, includeDirectives, includeDocumentationComments)
		End Function

		Public Shadows Function GetLeadingTrivia() As SyntaxTriviaList
			Return Me.GetFirstToken(True, False, False, False).LeadingTrivia
		End Function

		Public Shadows Function GetLocation() As Microsoft.CodeAnalysis.Location
			Dim sourceLocation As Microsoft.CodeAnalysis.Location
			If (Me.SyntaxTree IsNot Nothing) Then
				Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = Me.SyntaxTree
				If (Not syntaxTree.IsEmbeddedSyntaxTree()) Then
					If (Not syntaxTree.IsMyTemplate()) Then
						sourceLocation = New Microsoft.CodeAnalysis.SourceLocation(Me)
						Return sourceLocation
					End If
					sourceLocation = New MyTemplateLocation(syntaxTree, MyBase.Span)
					Return sourceLocation
				Else
					sourceLocation = New EmbeddedTreeLocation(syntaxTree.GetEmbeddedKind(), MyBase.Span)
					Return sourceLocation
				End If
			End If
			sourceLocation = New Microsoft.CodeAnalysis.SourceLocation(Me)
			Return sourceLocation
		End Function

		Friend Shadows Function GetReference() As SyntaxReference
			Return Me.SyntaxTree.GetReference(Me)
		End Function

		Friend Function GetSyntaxErrors(ByVal tree As Microsoft.CodeAnalysis.SyntaxTree) As ReadOnlyCollection(Of Diagnostic)
			Return Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode.DoGetSyntaxErrors(tree, Me)
		End Function

		Public Shadows Function GetTrailingTrivia() As SyntaxTriviaList
			Return Me.GetLastToken(True, False, False, False).TrailingTrivia
		End Function

		Protected Overrides Function InsertNodesInListCore(ByVal nodeInList As SyntaxNode, ByVal nodesToInsert As IEnumerable(Of SyntaxNode), ByVal insertBefore As Boolean) As SyntaxNode
			Return Microsoft.CodeAnalysis.SyntaxNodeExtensions.AsRootOfNewTreeWithOptionsFrom(SyntaxReplacer.InsertNodeInList(Me, nodeInList, nodesToInsert, insertBefore), Me.SyntaxTree)
		End Function

		Protected Overrides Function InsertTokensInListCore(ByVal originalToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal newTokens As IEnumerable(Of Microsoft.CodeAnalysis.SyntaxToken), ByVal insertBefore As Boolean) As SyntaxNode
			Return Microsoft.CodeAnalysis.SyntaxNodeExtensions.AsRootOfNewTreeWithOptionsFrom(SyntaxReplacer.InsertTokenInList(Me, originalToken, newTokens, insertBefore), Me.SyntaxTree)
		End Function

		Protected Overrides Function InsertTriviaInListCore(ByVal originalTrivia As Microsoft.CodeAnalysis.SyntaxTrivia, ByVal newTrivia As IEnumerable(Of Microsoft.CodeAnalysis.SyntaxTrivia), ByVal insertBefore As Boolean) As SyntaxNode
			Return Microsoft.CodeAnalysis.SyntaxNodeExtensions.AsRootOfNewTreeWithOptionsFrom(SyntaxReplacer.InsertTriviaInList(Me, originalTrivia, newTrivia, insertBefore), Me.SyntaxTree)
		End Function

		Protected Overrides Function IsEquivalentToCore(ByVal node As SyntaxNode, Optional ByVal topLevel As Boolean = False) As Boolean
			Return Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.AreEquivalent(Me, DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode), topLevel)
		End Function

		Public Function Kind() As SyntaxKind
			Return DirectCast(CUShort(MyBase.Green.RawKind), SyntaxKind)
		End Function

		Protected Overrides Function NormalizeWhitespaceCore(ByVal indentation As String, ByVal eol As String, ByVal elasticTrivia As Boolean) As SyntaxNode
			Return Microsoft.CodeAnalysis.SyntaxNodeExtensions.AsRootOfNewTreeWithOptionsFrom(SyntaxNormalizer.Normalize(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)(Me, indentation, eol, elasticTrivia, False), Me.SyntaxTree)
		End Function

		Private Shared Sub ProcessTrivia(ByVal tree As Microsoft.CodeAnalysis.SyntaxTree, ByVal errorList As List(Of Diagnostic), ByVal stack As Stack(Of SyntaxNodeOrToken), ByVal nodes As SyntaxTriviaList)
			Dim enumerator As SyntaxTriviaList.Enumerator = nodes.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As Microsoft.CodeAnalysis.SyntaxTrivia = enumerator.Current
				If (Not current.UnderlyingNode.ContainsDiagnostics) Then
					Continue While
				End If
				If (Not current.HasStructure) Then
					Dim diagnostics As Microsoft.CodeAnalysis.DiagnosticInfo() = DirectCast(current.UnderlyingNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode).GetDiagnostics()
					If (diagnostics Is Nothing) Then
						Continue While
					End If
					Dim length As Integer = CInt(diagnostics.Length) - 1
					For i As Integer = 0 To length
						Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = diagnostics(i)
						errorList.Add(Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode.CreateSyntaxError(tree, current, diagnosticInfo))
					Next

				Else
					stack.Push(DirectCast(current.GetStructure(), Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode))
				End If
			End While
		End Sub

		Private Shared Sub PushNodesWithErrors(ByVal stack As Stack(Of SyntaxNodeOrToken), ByVal nodes As ChildSyntaxList)
			Dim enumerator As ChildSyntaxList.Enumerator = nodes.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As SyntaxNodeOrToken = enumerator.Current
				If (Not current.ContainsDiagnostics) Then
					Continue While
				End If
				stack.Push(current)
			End While
		End Sub

		Protected Overrides Function RemoveNodesCore(ByVal nodes As IEnumerable(Of SyntaxNode), ByVal options As SyntaxRemoveOptions) As SyntaxNode
			Return Microsoft.CodeAnalysis.SyntaxNodeExtensions.AsRootOfNewTreeWithOptionsFrom(SyntaxNodeRemover.RemoveNodes(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)(Me, nodes, options), Me.SyntaxTree)
		End Function

		Protected Overrides Function ReplaceCore(Of TNode As SyntaxNode)(Optional ByVal nodes As IEnumerable(Of TNode) = Nothing, Optional ByVal computeReplacementNode As Func(Of TNode, TNode, SyntaxNode) = Nothing, Optional ByVal tokens As IEnumerable(Of Microsoft.CodeAnalysis.SyntaxToken) = Nothing, Optional ByVal computeReplacementToken As Func(Of Microsoft.CodeAnalysis.SyntaxToken, Microsoft.CodeAnalysis.SyntaxToken, Microsoft.CodeAnalysis.SyntaxToken) = Nothing, Optional ByVal trivia As IEnumerable(Of Microsoft.CodeAnalysis.SyntaxTrivia) = Nothing, Optional ByVal computeReplacementTrivia As Func(Of Microsoft.CodeAnalysis.SyntaxTrivia, Microsoft.CodeAnalysis.SyntaxTrivia, Microsoft.CodeAnalysis.SyntaxTrivia) = Nothing) As SyntaxNode
			Return Microsoft.CodeAnalysis.SyntaxNodeExtensions.AsRootOfNewTreeWithOptionsFrom(SyntaxReplacer.Replace(Of TNode)(Me, nodes, computeReplacementNode, tokens, computeReplacementToken, trivia, computeReplacementTrivia), Me.SyntaxTree)
		End Function

		Protected Overrides Function ReplaceNodeInListCore(ByVal originalNode As SyntaxNode, ByVal replacementNodes As IEnumerable(Of SyntaxNode)) As SyntaxNode
			Return Microsoft.CodeAnalysis.SyntaxNodeExtensions.AsRootOfNewTreeWithOptionsFrom(SyntaxReplacer.ReplaceNodeInList(Me, originalNode, replacementNodes), Me.SyntaxTree)
		End Function

		Protected Overrides Function ReplaceTokenInListCore(ByVal originalToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal newTokens As IEnumerable(Of Microsoft.CodeAnalysis.SyntaxToken)) As SyntaxNode
			Return Microsoft.CodeAnalysis.SyntaxNodeExtensions.AsRootOfNewTreeWithOptionsFrom(SyntaxReplacer.ReplaceTokenInList(Me, originalToken, newTokens), Me.SyntaxTree)
		End Function

		Protected Overrides Function ReplaceTriviaInListCore(ByVal originalTrivia As Microsoft.CodeAnalysis.SyntaxTrivia, ByVal newTrivia As IEnumerable(Of Microsoft.CodeAnalysis.SyntaxTrivia)) As SyntaxNode
			Return Microsoft.CodeAnalysis.SyntaxNodeExtensions.AsRootOfNewTreeWithOptionsFrom(SyntaxReplacer.ReplaceTriviaInList(Me, originalTrivia, newTrivia), Me.SyntaxTree)
		End Function

		Friend Overrides Function ShouldCreateWeakList() As Boolean
			Return TypeOf Me Is Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax
		End Function
	End Class
End Namespace