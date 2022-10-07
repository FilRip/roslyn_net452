Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.ComponentModel
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public MustInherit Class VisualBasicSyntaxTree
		Inherits SyntaxTree
		Private _lineDirectiveMap As VisualBasicLineDirectiveMap

		Friend ReadOnly Shared Dummy As VisualBasicSyntaxTree

		Friend ReadOnly Shared DummyReference As SyntaxReference

		Private _lazyWarningStateMap As VisualBasicWarningStateMap

		Private _lazySymbolsMap As VisualBasicSyntaxTree.ConditionalSymbolsMap

		Private ReadOnly Property ConditionalSymbols As VisualBasicSyntaxTree.ConditionalSymbolsMap
			Get
				If (Me._lazySymbolsMap = VisualBasicSyntaxTree.ConditionalSymbolsMap.Uninitialized) Then
					Interlocked.CompareExchange(Of VisualBasicSyntaxTree.ConditionalSymbolsMap)(Me._lazySymbolsMap, VisualBasicSyntaxTree.ConditionalSymbolsMap.Create(Me.GetRoot(CancellationToken.None), Me.Options), VisualBasicSyntaxTree.ConditionalSymbolsMap.Uninitialized)
				End If
				Return Me._lazySymbolsMap
			End Get
		End Property

		Friend ReadOnly Property HasReferenceDirectives As Boolean
			Get
				If (Me.Options.Kind <> SourceCodeKind.Script) Then
					Return False
				End If
				Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
				Return Me.GetCompilationUnitRoot(cancellationToken).GetReferenceDirectives().Count > 0
			End Get
		End Property

		Friend Overridable ReadOnly Property IsMyTemplate As Boolean
			Get
				Return False
			End Get
		End Property

		Public Shadows MustOverride ReadOnly Property Options As VisualBasicParseOptions

		Protected Overrides ReadOnly Property OptionsCore As ParseOptions
			Get
				Return Me.Options
			End Get
		End Property

		Shared Sub New()
			VisualBasicSyntaxTree.Dummy = New VisualBasicSyntaxTree.DummySyntaxTree()
			Dim dummy As VisualBasicSyntaxTree = VisualBasicSyntaxTree.Dummy
			Dim dummy1 As VisualBasicSyntaxTree = VisualBasicSyntaxTree.Dummy
			Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
			VisualBasicSyntaxTree.DummyReference = dummy.GetReference(dummy1.GetRoot(cancellationToken))
		End Sub

		Protected Sub New()
			MyBase.New()
			Me._lazySymbolsMap = VisualBasicSyntaxTree.ConditionalSymbolsMap.Uninitialized
		End Sub

		Protected Function CloneNodeAsRoot(Of T As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)(ByVal node As T) As T
			Return SyntaxNode.CloneNodeAsRoot(Of T)(node, Me)
		End Function

		Public Shared Function Create(ByVal root As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode, Optional ByVal options As VisualBasicParseOptions = Nothing, Optional ByVal path As String = "", Optional ByVal encoding As System.Text.Encoding = Nothing, Optional ByVal diagnosticOptions As ImmutableDictionary(Of String, ReportDiagnostic) = Nothing) As SyntaxTree
			If (root Is Nothing) Then
				Throw New ArgumentNullException("root")
			End If
			Return New VisualBasicSyntaxTree.ParsedSyntaxTree(Nothing, encoding, SourceHashAlgorithm.Sha1, path, If(options, VisualBasicParseOptions.[Default]), root, False, diagnosticOptions, True)
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		Public Shared Function Create(ByVal root As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode, ByVal options As VisualBasicParseOptions, ByVal path As String, ByVal encoding As System.Text.Encoding) As SyntaxTree
			Return VisualBasicSyntaxTree.Create(root, options, path, encoding, Nothing)
		End Function

		Friend Shared Function CreateWithoutClone(ByVal root As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode) As SyntaxTree
			Return New VisualBasicSyntaxTree.ParsedSyntaxTree(Nothing, Nothing, SourceHashAlgorithm.Sha1, "", VisualBasicParseOptions.[Default], root, False, Nothing, False)
		End Function

		Friend Function EnumerateDiagnostics(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByVal position As Integer, ByVal InDocumentationComment As Boolean) As IEnumerable(Of Diagnostic)
			Return New VisualBasicSyntaxTree.VB$StateMachine_34_EnumerateDiagnostics(-2) With
			{
				.$VB$Me = Me,
				.$P_node = node,
				.$P_position = position,
				.$P_InDocumentationComment = InDocumentationComment
			}
		End Function

		Public Overrides Function GetChangedSpans(ByVal oldTree As SyntaxTree) As IList(Of TextSpan)
			If (oldTree Is Nothing) Then
				Throw New ArgumentNullException("oldTree")
			End If
			Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
			Dim root As SyntaxNode = oldTree.GetRoot(cancellationToken)
			cancellationToken = New System.Threading.CancellationToken()
			Return SyntaxDiffer.GetPossiblyDifferentTextSpans(root, Me.GetRoot(cancellationToken))
		End Function

		Public Overrides Function GetChanges(ByVal oldTree As SyntaxTree) As IList(Of TextChange)
			If (oldTree Is Nothing) Then
				Throw New ArgumentNullException("oldTree")
			End If
			Return SyntaxDiffer.GetTextChanges(oldTree, Me)
		End Function

		Public Function GetCompilationUnitRoot(Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax
			Return DirectCast(Me.GetRoot(cancellationToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax)
		End Function

		Public Overrides Function GetDiagnostics(ByVal node As SyntaxNode) As IEnumerable(Of Diagnostic)
			If (node Is Nothing) Then
				Throw New ArgumentNullException("node")
			End If
			Return Me.GetDiagnostics(DirectCast(node.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode).Position, Me.InDocumentationComment(node))
		End Function

		Public Overrides Function GetDiagnostics(ByVal token As Microsoft.CodeAnalysis.SyntaxToken) As IEnumerable(Of Diagnostic)
			Return Me.GetDiagnostics(DirectCast(token.Node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken), token.Position, Me.InDocumentationComment(token))
		End Function

		Public Overrides Function GetDiagnostics(ByVal trivia As Microsoft.CodeAnalysis.SyntaxTrivia) As IEnumerable(Of Diagnostic)
			Return Me.GetDiagnostics(DirectCast(trivia.UnderlyingNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), trivia.Position, Me.InDocumentationComment(trivia))
		End Function

		Public Overrides Function GetDiagnostics(ByVal nodeOrToken As SyntaxNodeOrToken) As IEnumerable(Of Diagnostic)
			Return Me.GetDiagnostics(DirectCast(nodeOrToken.UnderlyingNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), nodeOrToken.Position, Me.InDocumentationComment(nodeOrToken))
		End Function

		Public Overrides Function GetDiagnostics(Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IEnumerable(Of Diagnostic)
			Return Me.GetDiagnostics(Me.GetRoot(cancellationToken).VbGreen, 0, False)
		End Function

		Friend Function GetDiagnostics(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByVal position As Integer, ByVal InDocumentationComment As Boolean) As IEnumerable(Of Diagnostic)
			Dim diagnostics As IEnumerable(Of Diagnostic)
			If (node Is Nothing) Then
				Throw New InvalidOperationException()
			End If
			diagnostics = If(Not node.ContainsDiagnostics, SpecializedCollections.EmptyEnumerable(Of Diagnostic)(), Me.EnumerateDiagnostics(node, position, InDocumentationComment))
			Return diagnostics
		End Function

		Private Function GetLinePosition(ByVal position As Integer) As LinePosition
			Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
			Return Me.GetText(cancellationToken).Lines.GetLinePosition(position)
		End Function

		Public Overrides Function GetLineSpan(ByVal span As TextSpan, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As FileLinePositionSpan
			Return New FileLinePositionSpan(Me.FilePath, Me.GetLinePosition(span.Start), Me.GetLinePosition(span.[End]))
		End Function

		Public Overrides Function GetLineVisibility(ByVal position As Integer, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As LineVisibility
			If (Me._lineDirectiveMap Is Nothing) Then
				Interlocked.CompareExchange(Of VisualBasicLineDirectiveMap)(Me._lineDirectiveMap, New VisualBasicLineDirectiveMap(Me), Nothing)
			End If
			Return Me._lineDirectiveMap.GetLineVisibility(Me.GetText(cancellationToken), position)
		End Function

		Public Overrides Function GetLocation(ByVal span As TextSpan) As Location
			Dim embeddedTreeLocation As Location
			If (Me.IsEmbeddedSyntaxTree()) Then
				embeddedTreeLocation = New Microsoft.CodeAnalysis.VisualBasic.EmbeddedTreeLocation(Me.GetEmbeddedKind(), span)
			ElseIf (Not Me.IsMyTemplate) Then
				embeddedTreeLocation = New SourceLocation(Me, span)
			Else
				embeddedTreeLocation = New MyTemplateLocation(Me, span)
			End If
			Return embeddedTreeLocation
		End Function

		Public Overrides Function GetMappedLineSpan(ByVal span As TextSpan, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As FileLinePositionSpan
			If (Me._lineDirectiveMap Is Nothing) Then
				Interlocked.CompareExchange(Of VisualBasicLineDirectiveMap)(Me._lineDirectiveMap, New VisualBasicLineDirectiveMap(Me), Nothing)
			End If
			Return Me._lineDirectiveMap.TranslateSpan(Me.GetText(cancellationToken), Me.FilePath, span)
		End Function

		Friend Overrides Function GetMappedLineSpanAndVisibility(ByVal span As TextSpan, ByRef isHiddenPosition As Boolean) As FileLinePositionSpan
			If (Me._lineDirectiveMap Is Nothing) Then
				Interlocked.CompareExchange(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VisualBasicLineDirectiveMap)(Me._lineDirectiveMap, New Microsoft.CodeAnalysis.VisualBasic.Syntax.VisualBasicLineDirectiveMap(Me), Nothing)
			End If
			Dim visualBasicLineDirectiveMap As Microsoft.CodeAnalysis.VisualBasic.Syntax.VisualBasicLineDirectiveMap = Me._lineDirectiveMap
			Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
			Return visualBasicLineDirectiveMap.TranslateSpanAndVisibility(Me.GetText(cancellationToken), Me.FilePath, span, isHiddenPosition)
		End Function

		Friend Function GetPreprocessingSymbolInfo(ByVal identifierNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax) As VisualBasicPreprocessingSymbolInfo
			Dim valueText As String = identifierNode.Identifier.ValueText
			Dim conditionalSymbols As VisualBasicSyntaxTree.ConditionalSymbolsMap = Me.ConditionalSymbols
			If (conditionalSymbols Is Nothing) Then
				Return VisualBasicPreprocessingSymbolInfo.None
			End If
			Return conditionalSymbols.GetPreprocessingSymbolInfo(valueText, identifierNode)
		End Function

		Public MustOverride Shadows Function GetRoot(Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode

		Public Overridable Shadows Function GetRootAsync(Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Task(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = Nothing
			Return Task.FromResult(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)(If(Me.TryGetRoot(visualBasicSyntaxNode), visualBasicSyntaxNode, Me.GetRoot(cancellationToken)))
		End Function

		Protected Overrides Async Function GetRootAsyncCore(ByVal cancellationToken As System.Threading.CancellationToken) As Task(Of Microsoft.CodeAnalysis.SyntaxNode)
			Dim configuredTaskAwaitable As ConfiguredTaskAwaitable(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode) = Me.GetRootAsync(cancellationToken).ConfigureAwait(False)
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = Await configuredTaskAwaitable
			Return syntaxNode
		End Function

		Protected Overrides Function GetRootCore(ByVal CancellationToken As System.Threading.CancellationToken) As SyntaxNode
			Return Me.GetRoot(CancellationToken)
		End Function

		Friend Function GetWarningState(ByVal id As String, ByVal position As Integer) As ReportDiagnostic
			If (Me._lazyWarningStateMap Is Nothing) Then
				Interlocked.CompareExchange(Of VisualBasicWarningStateMap)(Me._lazyWarningStateMap, New VisualBasicWarningStateMap(Me), Nothing)
			End If
			Return Me._lazyWarningStateMap.GetWarningState(id, position)
		End Function

		Public Overrides Function HasHiddenRegions() As Boolean
			If (Me._lineDirectiveMap Is Nothing) Then
				Interlocked.CompareExchange(Of VisualBasicLineDirectiveMap)(Me._lineDirectiveMap, New VisualBasicLineDirectiveMap(Me), Nothing)
			End If
			Return Me._lineDirectiveMap.HasAnyHiddenRegions()
		End Function

		Private Function InDocumentationComment(ByVal node As SyntaxNode) As Boolean
			Dim flag As Boolean = False
			While node IsNot Nothing AndAlso SyntaxFacts.IsXmlSyntax(node.Kind())
				flag = True
				node = node.Parent
			End While
			If (Not flag OrElse node Is Nothing) Then
				Return False
			End If
			Return node.IsKind(SyntaxKind.DocumentationCommentTrivia)
		End Function

		Private Function InDocumentationComment(ByVal node As SyntaxNodeOrToken) As Boolean
			Dim flag As Boolean
			flag = If(Not node.IsToken, Me.InDocumentationComment(node.AsNode()), Me.InDocumentationComment(node.AsToken()))
			Return flag
		End Function

		Private Function InDocumentationComment(ByVal token As Microsoft.CodeAnalysis.SyntaxToken) As Boolean
			Return Me.InDocumentationComment(token.Parent)
		End Function

		Private Function InDocumentationComment(ByVal trivia As Microsoft.CodeAnalysis.SyntaxTrivia) As Boolean
			Return Me.InDocumentationComment(trivia.Token)
		End Function

		Friend Function IsAnyPreprocessorSymbolDefined(ByVal conditionalSymbolNames As IEnumerable(Of String), ByVal atNode As SyntaxNodeOrToken) As Boolean
			Dim flag As Boolean
			Dim enumerator As IEnumerator(Of String) = Nothing
			Dim conditionalSymbols As VisualBasicSyntaxTree.ConditionalSymbolsMap = Me.ConditionalSymbols
			If (conditionalSymbols IsNot Nothing) Then
				Using enumerator
					enumerator = conditionalSymbolNames.GetEnumerator()
					While enumerator.MoveNext()
						If (Not conditionalSymbols.IsConditionalSymbolDefined(enumerator.Current, atNode)) Then
							Continue While
						End If
						flag = True
						Return flag
					End While
				End Using
				flag = False
			Else
				flag = False
			End If
			Return flag
		End Function

		Public Overrides Function IsEquivalentTo(ByVal tree As SyntaxTree, Optional ByVal topLevel As Boolean = False) As Boolean
			Return Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.AreEquivalent(Me, tree, topLevel)
		End Function

		Public Shared Function ParseText(ByVal text As String, Optional ByVal options As VisualBasicParseOptions = Nothing, Optional ByVal path As String = "", Optional ByVal encoding As System.Text.Encoding = Nothing, Optional ByVal diagnosticOptions As ImmutableDictionary(Of String, ReportDiagnostic) = Nothing, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SyntaxTree
			Return VisualBasicSyntaxTree.ParseText(text, False, options, path, encoding, diagnosticOptions, cancellationToken)
		End Function

		Friend Shared Function ParseText(ByVal text As String, ByVal isMyTemplate As Boolean, Optional ByVal options As VisualBasicParseOptions = Nothing, Optional ByVal path As String = "", Optional ByVal encoding As System.Text.Encoding = Nothing, Optional ByVal diagnosticOptions As ImmutableDictionary(Of String, ReportDiagnostic) = Nothing, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SyntaxTree
			Return VisualBasicSyntaxTree.ParseText(SourceText.From(text, encoding, SourceHashAlgorithm.Sha1), isMyTemplate, options, path, diagnosticOptions, cancellationToken)
		End Function

		Public Shared Function ParseText(ByVal text As SourceText, Optional ByVal options As VisualBasicParseOptions = Nothing, Optional ByVal path As String = "", Optional ByVal diagnosticOptions As ImmutableDictionary(Of String, ReportDiagnostic) = Nothing, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SyntaxTree
			Return VisualBasicSyntaxTree.ParseText(text, False, options, path, diagnosticOptions, cancellationToken)
		End Function

		Friend Shared Function ParseText(ByVal text As SourceText, ByVal isMyTemplate As Boolean, Optional ByVal parseOptions As VisualBasicParseOptions = Nothing, Optional ByVal path As String = "", Optional ByVal diagnosticOptions As ImmutableDictionary(Of String, ReportDiagnostic) = Nothing, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SyntaxTree
			Dim compilationUnitSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax
			If (text Is Nothing) Then
				Throw New ArgumentNullException("text")
			End If
			parseOptions = If(parseOptions, VisualBasicParseOptions.[Default])
			Using parser As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser(text, parseOptions, cancellationToken)
				compilationUnitSyntax = parser.ParseCompilationUnit()
			End Using
			Dim compilationUnitSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax = DirectCast(compilationUnitSyntax.CreateRed(Nothing, 0), Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax)
			Return New VisualBasicSyntaxTree.ParsedSyntaxTree(text, text.Encoding, text.ChecksumAlgorithm, path, parseOptions, compilationUnitSyntax1, isMyTemplate, diagnosticOptions, True)
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		Public Shared Function ParseText(ByVal text As String, ByVal options As VisualBasicParseOptions, ByVal path As String, ByVal encoding As System.Text.Encoding, ByVal cancellationToken As System.Threading.CancellationToken) As SyntaxTree
			Return VisualBasicSyntaxTree.ParseText(text, options, path, encoding, Nothing, cancellationToken)
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		Public Shared Function ParseText(ByVal text As SourceText, ByVal options As VisualBasicParseOptions, ByVal path As String, ByVal cancellationToken As System.Threading.CancellationToken) As SyntaxTree
			Return VisualBasicSyntaxTree.ParseText(text, options, path, DirectCast(Nothing, ImmutableDictionary(Of String, ReportDiagnostic)), cancellationToken)
		End Function

		Friend Shared Function ParseTextLazy(ByVal text As SourceText, Optional ByVal options As VisualBasicParseOptions = Nothing, Optional ByVal path As String = "") As SyntaxTree
			Return New VisualBasicSyntaxTree.LazySyntaxTree(text, If(options, VisualBasicParseOptions.[Default]), path, Nothing)
		End Function

		Public MustOverride Function TryGetRoot(ByRef root As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode) As Boolean

		Protected Overrides Function TryGetRootCore(ByRef root As SyntaxNode) As Boolean
			Dim flag As Boolean
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = Nothing
			If (Not Me.TryGetRoot(visualBasicSyntaxNode)) Then
				root = Nothing
				flag = False
			Else
				root = visualBasicSyntaxNode
				flag = True
			End If
			Return flag
		End Function

		Public Overrides Function WithChangedText(ByVal newText As Microsoft.CodeAnalysis.Text.SourceText) As Microsoft.CodeAnalysis.SyntaxTree
			Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree
			Dim sourceText As Microsoft.CodeAnalysis.Text.SourceText = Nothing
			syntaxTree = If(Not Me.TryGetText(sourceText), Me.WithChanges(newText, New TextChangeRange() { New TextChangeRange(New TextSpan(0, Me.Length), newText.Length) }), Me.WithChanges(newText, newText.GetChangeRanges(sourceText).ToArray()))
			Return syntaxTree
		End Function

		Private Function WithChanges(ByVal newText As SourceText, ByVal changes As TextChangeRange()) As SyntaxTree
			Dim blender As Scanner
			Dim compilationUnitSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax
			If (changes Is Nothing) Then
				Throw New ArgumentNullException("changes")
			End If
			If (CInt(changes.Length) <> 1 OrElse Not (changes(0).Span = New TextSpan(0, Me.Length)) OrElse changes(0).NewLength <> newText.Length) Then
				blender = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Blender(newText, changes, Me, Me.Options)
			Else
				blender = New Scanner(newText, Me.Options, False)
			End If
			Using blender
				compilationUnitSyntax = (New Parser(blender)).ParseCompilationUnit()
			End Using
			Dim compilationUnitSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax = DirectCast(compilationUnitSyntax.CreateRed(Nothing, 0), Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax)
			Return New VisualBasicSyntaxTree.ParsedSyntaxTree(newText, newText.Encoding, newText.ChecksumAlgorithm, Me.FilePath, Me.Options, compilationUnitSyntax1, False, Me.DiagnosticOptions, True)
		End Function

		Private Class ConditionalSymbolsMap
			Private ReadOnly _conditionalsMap As ImmutableDictionary(Of String, Stack(Of Tuple(Of CConst, Integer)))

			Friend ReadOnly Shared Uninitialized As VisualBasicSyntaxTree.ConditionalSymbolsMap

			Shared Sub New()
				VisualBasicSyntaxTree.ConditionalSymbolsMap.Uninitialized = New VisualBasicSyntaxTree.ConditionalSymbolsMap()
			End Sub

			Private Sub New()
				MyBase.New()
			End Sub

			Private Sub New(ByVal conditionalsMap As ImmutableDictionary(Of String, Stack(Of Tuple(Of CConst, Integer))))
				MyBase.New()
				Me._conditionalsMap = conditionalsMap
			End Sub

			Friend Shared Function Create(ByVal syntaxRoot As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode, ByVal options As VisualBasicParseOptions) As VisualBasicSyntaxTree.ConditionalSymbolsMap
				Dim strs As ImmutableDictionary(Of String, Stack(Of Tuple(Of CConst, Integer))) = (New VisualBasicSyntaxTree.ConditionalSymbolsMap.ConditionalSymbolsMapBuilder()).Build(syntaxRoot, options)
				If (strs Is Nothing) Then
					Return Nothing
				End If
				Return New VisualBasicSyntaxTree.ConditionalSymbolsMap(strs)
			End Function

			Friend Function GetPreprocessingSymbolInfo(ByVal conditionalSymbolName As String, ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.VisualBasicPreprocessingSymbolInfo
				Dim visualBasicPreprocessingSymbolInfo As Microsoft.CodeAnalysis.VisualBasic.VisualBasicPreprocessingSymbolInfo
				Dim preprocessorSymbolValue As CConst = Me.GetPreprocessorSymbolValue(conditionalSymbolName, node)
				If (preprocessorSymbolValue IsNot Nothing) Then
					Dim str As String = Me._conditionalsMap.Keys.First(Function(key As String) CaseInsensitiveComparison.Equals(key, conditionalSymbolName))
					visualBasicPreprocessingSymbolInfo = New Microsoft.CodeAnalysis.VisualBasic.VisualBasicPreprocessingSymbolInfo(New PreprocessingSymbol(str), RuntimeHelpers.GetObjectValue(preprocessorSymbolValue.ValueAsObject), True)
				Else
					visualBasicPreprocessingSymbolInfo = Microsoft.CodeAnalysis.VisualBasic.VisualBasicPreprocessingSymbolInfo.None
				End If
				Return visualBasicPreprocessingSymbolInfo
			End Function

			Private Function GetPreprocessorSymbolValue(ByVal conditionalSymbolName As String, ByVal node As SyntaxNodeOrToken) As CConst
				Dim item1 As CConst
				Dim enumerator As Stack(Of Tuple(Of CConst, Integer)).Enumerator = New Stack(Of Tuple(Of CConst, Integer)).Enumerator()
				Dim tuples As Stack(Of Tuple(Of CConst, Integer)) = Nothing
				If (Me._conditionalsMap.TryGetValue(conditionalSymbolName, tuples)) Then
					Dim spanStart As Integer = node.SpanStart
					Try
						enumerator = tuples.GetEnumerator()
						While enumerator.MoveNext()
							Dim current As Tuple(Of CConst, Integer) = enumerator.Current
							If (current.Item2 >= spanStart) Then
								Continue While
							End If
							item1 = current.Item1
							Return item1
						End While
					Finally
						DirectCast(enumerator, IDisposable).Dispose()
					End Try
				End If
				item1 = Nothing
				Return item1
			End Function

			Friend Function IsConditionalSymbolDefined(ByVal conditionalSymbolName As String, ByVal node As SyntaxNodeOrToken) As Boolean
				Dim value As Boolean
				If (conditionalSymbolName IsNot Nothing) Then
					Dim preprocessorSymbolValue As CConst = Me.GetPreprocessorSymbolValue(conditionalSymbolName, node)
					If (preprocessorSymbolValue IsNot Nothing AndAlso Not preprocessorSymbolValue.IsBad) Then
						Select Case preprocessorSymbolValue.SpecialType
							Case SpecialType.System_Boolean
								value = DirectCast(preprocessorSymbolValue, CConst(Of Boolean)).Value
								Exit Select
							Case SpecialType.System_Char
							Case SpecialType.System_Decimal
							Case SpecialType.System_Single
							Case SpecialType.System_Double
								value = False
								Return value
							Case SpecialType.System_SByte
								value = DirectCast(preprocessorSymbolValue, CConst(Of SByte)).Value <> 0
								Exit Select
							Case SpecialType.System_Byte
								value = DirectCast(preprocessorSymbolValue, CConst(Of Byte)).Value <> 0
								Exit Select
							Case SpecialType.System_Int16
								value = DirectCast(preprocessorSymbolValue, CConst(Of Short)).Value <> 0
								Exit Select
							Case SpecialType.System_UInt16
								value = DirectCast(preprocessorSymbolValue, CConst(Of UShort)).Value <> 0
								Exit Select
							Case SpecialType.System_Int32
								value = DirectCast(preprocessorSymbolValue, CConst(Of Integer)).Value <> 0
								Exit Select
							Case SpecialType.System_UInt32
								value = CULng(DirectCast(preprocessorSymbolValue, CConst(Of UInteger)).Value) <> CLng(0)
								Exit Select
							Case SpecialType.System_Int64
								value = DirectCast(preprocessorSymbolValue, CConst(Of Long)).Value <> CLng(0)
								Exit Select
							Case SpecialType.System_UInt64
								value = [Decimal].Compare(New [Decimal](DirectCast(preprocessorSymbolValue, CConst(Of ULong)).Value), [Decimal].Zero) <> 0
								Exit Select
							Case SpecialType.System_String
								value = True
								Exit Select
							Case Else
								value = False
								Return value
						End Select
					Else
						value = False
						Return value
					End If
				Else
					value = False
					Return value
				End If
				Return value
			End Function

			Private Class ConditionalSymbolsMapBuilder
				Private _conditionalsMap As Dictionary(Of String, Stack(Of Tuple(Of CConst, Integer)))

				Private _preprocessorState As Scanner.PreprocessorState

				Public Sub New()
					MyBase.New()
				End Sub

				Friend Function Build(ByVal root As SyntaxNodeOrToken, ByVal options As VisualBasicParseOptions) As ImmutableDictionary(Of String, Stack(Of Tuple(Of CConst, Integer)))
					Me._conditionalsMap = New Dictionary(Of String, Stack(Of Tuple(Of CConst, Integer)))(CaseInsensitiveComparison.Comparer)
					Dim preprocessorConstants As ImmutableDictionary(Of String, CConst) = Scanner.GetPreprocessorConstants(options)
					Me.ProcessCommandLinePreprocessorSymbols(preprocessorConstants)
					Me._preprocessorState = New Scanner.PreprocessorState(preprocessorConstants)
					Me.ProcessSourceDirectives(root.GetDirectives(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax)(Nothing))
					If (Not Me._conditionalsMap.Any()) Then
						Return Nothing
					End If
					Return ImmutableDictionary.CreateRange(Of String, Stack(Of Tuple(Of CConst, Integer)))(CaseInsensitiveComparison.Comparer, Me._conditionalsMap)
				End Function

				Private Sub ProcessCommandLinePreprocessorSymbols(ByVal preprocessorSymbolsMap As ImmutableDictionary(Of String, CConst))
					Dim enumerator As ImmutableDictionary(Of String, CConst).Enumerator = New ImmutableDictionary(Of String, CConst).Enumerator()
					Try
						enumerator = preprocessorSymbolsMap.GetEnumerator()
						While enumerator.MoveNext()
							Dim current As KeyValuePair(Of String, CConst) = enumerator.Current
							Me.ProcessConditionalSymbolDefinition(current.Key, current.Value, 0)
						End While
					Finally
						DirectCast(enumerator, IDisposable).Dispose()
					End Try
				End Sub

				Private Sub ProcessConditionalSymbolDefinition(ByVal name As String, ByVal value As CConst, ByVal position As Integer)
					Dim tuples As Stack(Of Tuple(Of CConst, Integer)) = Nothing
					If (Not Me._conditionalsMap.TryGetValue(name, tuples)) Then
						tuples = New Stack(Of Tuple(Of CConst, Integer))()
						Me._conditionalsMap.Add(name, tuples)
					End If
					tuples.Push(Tuple.Create(Of CConst, Integer)(value, position))
				End Sub

				Private Sub ProcessDirective(ByVal directive As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax)
					Dim green As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax
					If (directive.Kind() <> SyntaxKind.ConstDirectiveTrivia) Then
						Dim preprocessorState As Scanner.PreprocessorState = Me._preprocessorState
						green = DirectCast(directive.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax)
						Me._preprocessorState = Scanner.ApplyDirective(preprocessorState, green)
					Else
						Dim symbolsMap As ImmutableDictionary(Of String, CConst) = Me._preprocessorState.SymbolsMap
						Dim preprocessorState1 As Scanner.PreprocessorState = Me._preprocessorState
						green = DirectCast(directive.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax)
						Me._preprocessorState = Scanner.ApplyDirective(preprocessorState1, green)
						Dim strs As ImmutableDictionary(Of String, CConst) = Me._preprocessorState.SymbolsMap
						If (symbolsMap <> strs) Then
							Dim valueText As String = DirectCast(directive, Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstDirectiveTriviaSyntax).Name.ValueText
							Me.ProcessConditionalSymbolDefinition(valueText, strs(valueText), directive.SpanStart)
							Return
						End If
					End If
				End Sub

				Private Sub ProcessSourceDirectives(ByVal directives As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax))
					Dim enumerator As IEnumerator(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax) = Nothing
					Try
						enumerator = directives.GetEnumerator()
						While enumerator.MoveNext()
							Me.ProcessDirective(enumerator.Current)
						End While
					Finally
						If (enumerator IsNot Nothing) Then
							enumerator.Dispose()
						End If
					End Try
				End Sub
			End Class
		End Class

		Friend Class DummySyntaxTree
			Inherits VisualBasicSyntaxTree
			Private ReadOnly _node As Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax

			Public Overrides ReadOnly Property DiagnosticOptions As ImmutableDictionary(Of String, ReportDiagnostic)
				Get
					Throw ExceptionUtilities.Unreachable
				End Get
			End Property

			Public Overrides ReadOnly Property Encoding As System.Text.Encoding
				Get
					Return System.Text.Encoding.UTF8
				End Get
			End Property

			Public Overrides ReadOnly Property FilePath As String
				Get
					Return [String].Empty
				End Get
			End Property

			Public Overrides ReadOnly Property HasCompilationUnitRoot As Boolean
				Get
					Return True
				End Get
			End Property

			Public Overrides ReadOnly Property Length As Integer
				Get
					Return 0
				End Get
			End Property

			Public Overrides ReadOnly Property Options As VisualBasicParseOptions
				Get
					Return VisualBasicParseOptions.[Default]
				End Get
			End Property

			Public Sub New()
				MyBase.New()
				Me._node = MyBase.CloneNodeAsRoot(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax)(Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ParseCompilationUnit([String].Empty, 0, Nothing))
			End Sub

			Public Overrides Function GetReference(ByVal node As SyntaxNode) As SyntaxReference
				Return New SimpleSyntaxReference(Me, node)
			End Function

			Public Overrides Function GetRoot(Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
				Return Me._node
			End Function

			Public Overrides Function GetText(Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SourceText
				Return SourceText.From([String].Empty, System.Text.Encoding.UTF8, SourceHashAlgorithm.Sha1)
			End Function

			Public Overrides Function ToString() As String
				Return [String].Empty
			End Function

			Public Overrides Function TryGetRoot(ByRef root As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode) As Boolean
				root = Me._node
				Return True
			End Function

			Public Overrides Function TryGetText(ByRef text As SourceText) As Boolean
				text = SourceText.From([String].Empty, System.Text.Encoding.UTF8, SourceHashAlgorithm.Sha1)
				Return True
			End Function

			Public Overrides Function WithChangedText(ByVal newText As SourceText) As SyntaxTree
				Throw New InvalidOperationException()
			End Function

			Public Overrides Function WithDiagnosticOptions(ByVal options As ImmutableDictionary(Of String, ReportDiagnostic)) As SyntaxTree
				Throw ExceptionUtilities.Unreachable
			End Function

			Public Overrides Function WithFilePath(ByVal path As String) As SyntaxTree
				Return Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.SyntaxTree(Me._node, Me.Options, path, Nothing)
			End Function

			Public Overrides Function WithRootAndOptions(ByVal root As SyntaxNode, ByVal options As ParseOptions) As SyntaxTree
				Return Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.SyntaxTree(root, options, Me.FilePath, Nothing)
			End Function
		End Class

		Private NotInheritable Class LazySyntaxTree
			Inherits VisualBasicSyntaxTree
			Private ReadOnly _text As SourceText

			Private ReadOnly _options As VisualBasicParseOptions

			Private ReadOnly _path As String

			Private ReadOnly _diagnosticOptions As ImmutableDictionary(Of String, ReportDiagnostic)

			Private _lazyRoot As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode

			Public Overrides ReadOnly Property DiagnosticOptions As ImmutableDictionary(Of String, ReportDiagnostic)
				Get
					Return Me._diagnosticOptions
				End Get
			End Property

			Public Overrides ReadOnly Property Encoding As System.Text.Encoding
				Get
					Return Me._text.Encoding
				End Get
			End Property

			Public Overrides ReadOnly Property FilePath As String
				Get
					Return Me._path
				End Get
			End Property

			Public Overrides ReadOnly Property HasCompilationUnitRoot As Boolean
				Get
					Return True
				End Get
			End Property

			Friend Overrides ReadOnly Property IsMyTemplate As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property Length As Integer
				Get
					Return Me._text.Length
				End Get
			End Property

			Public Overrides ReadOnly Property Options As VisualBasicParseOptions
				Get
					Return Me._options
				End Get
			End Property

			Friend Sub New(ByVal text As SourceText, ByVal options As VisualBasicParseOptions, ByVal path As String, ByVal diagnosticOptions As ImmutableDictionary(Of String, ReportDiagnostic))
				MyBase.New()
				Me._text = text
				Me._options = options
				Me._path = If(path, [String].Empty)
				Me._diagnosticOptions = If(diagnosticOptions, SyntaxTree.EmptyDiagnosticOptions)
			End Sub

			Public Overrides Function GetReference(ByVal node As SyntaxNode) As SyntaxReference
				Return New SimpleSyntaxReference(Me, node)
			End Function

			Public Overrides Function GetRoot(Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
				If (Me._lazyRoot Is Nothing) Then
					Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ParseSyntaxTree(Me._text, Me._options, Me._path, cancellationToken)
					Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = MyBase.CloneNodeAsRoot(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)(DirectCast(syntaxTree.GetRoot(cancellationToken), Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode))
					Interlocked.CompareExchange(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)(Me._lazyRoot, visualBasicSyntaxNode, Nothing)
				End If
				Return Me._lazyRoot
			End Function

			Public Overrides Function GetText(Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SourceText
				Return Me._text
			End Function

			Public Overrides Function TryGetRoot(ByRef root As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode) As Boolean
				root = Me._lazyRoot
				Return root IsNot Nothing
			End Function

			Public Overrides Function TryGetText(ByRef text As SourceText) As Boolean
				text = Me._text
				Return True
			End Function

			Public Overrides Function WithDiagnosticOptions(ByVal options As ImmutableDictionary(Of String, ReportDiagnostic)) As SyntaxTree
				Dim lazySyntaxTree As SyntaxTree
				If (options Is Nothing) Then
					options = SyntaxTree.EmptyDiagnosticOptions
				End If
				If (Not [Object].ReferenceEquals(Me._diagnosticOptions, options)) Then
					Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = Nothing
					If (Not Me.TryGetRoot(visualBasicSyntaxNode)) Then
						lazySyntaxTree = New VisualBasicSyntaxTree.LazySyntaxTree(Me._text, Me._options, Me._path, options)
					Else
						lazySyntaxTree = New VisualBasicSyntaxTree.ParsedSyntaxTree(Me._text, Me._text.Encoding, Me._text.ChecksumAlgorithm, Me._path, Me._options, visualBasicSyntaxNode, False, options, True)
					End If
				Else
					lazySyntaxTree = Me
				End If
				Return lazySyntaxTree
			End Function

			Public Overrides Function WithFilePath(ByVal path As String) As SyntaxTree
				Dim lazySyntaxTree As SyntaxTree
				If (Not [String].Equals(Me._path, path)) Then
					Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = Nothing
					If (Not Me.TryGetRoot(visualBasicSyntaxNode)) Then
						lazySyntaxTree = New VisualBasicSyntaxTree.LazySyntaxTree(Me._text, Me._options, path, Me._diagnosticOptions)
					Else
						lazySyntaxTree = New VisualBasicSyntaxTree.ParsedSyntaxTree(Me._text, Me._text.Encoding, Me._text.ChecksumAlgorithm, path, Me._options, visualBasicSyntaxNode, False, Me._diagnosticOptions, True)
					End If
				Else
					lazySyntaxTree = Me
				End If
				Return lazySyntaxTree
			End Function

			Public Overrides Function WithRootAndOptions(ByVal root As SyntaxNode, ByVal options As ParseOptions) As SyntaxTree
				Dim parsedSyntaxTree As SyntaxTree
				If (Me._lazyRoot <> root OrElse Me._options <> options) Then
					parsedSyntaxTree = New VisualBasicSyntaxTree.ParsedSyntaxTree(Nothing, Me._text.Encoding, Me._text.ChecksumAlgorithm, Me._path, DirectCast(options, VisualBasicParseOptions), DirectCast(root, Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode), False, Me._diagnosticOptions, True)
				Else
					parsedSyntaxTree = Me
				End If
				Return parsedSyntaxTree
			End Function
		End Class

		Private Class ParsedSyntaxTree
			Inherits VisualBasicSyntaxTree
			Private ReadOnly _options As VisualBasicParseOptions

			Private ReadOnly _path As String

			Private ReadOnly _root As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode

			Private ReadOnly _hasCompilationUnitRoot As Boolean

			Private ReadOnly _isMyTemplate As Boolean

			Private ReadOnly _encodingOpt As System.Text.Encoding

			Private ReadOnly _checksumAlgorithm As SourceHashAlgorithm

			Private ReadOnly _diagnosticOptions As ImmutableDictionary(Of String, ReportDiagnostic)

			Private _lazyText As SourceText

			Public Overrides ReadOnly Property DiagnosticOptions As ImmutableDictionary(Of String, ReportDiagnostic)
				Get
					Return Me._diagnosticOptions
				End Get
			End Property

			Public Overrides ReadOnly Property Encoding As System.Text.Encoding
				Get
					Return Me._encodingOpt
				End Get
			End Property

			Public Overrides ReadOnly Property FilePath As String
				Get
					Return Me._path
				End Get
			End Property

			Public Overrides ReadOnly Property HasCompilationUnitRoot As Boolean
				Get
					Return Me._hasCompilationUnitRoot
				End Get
			End Property

			Friend Overrides ReadOnly Property IsMyTemplate As Boolean
				Get
					Return Me._isMyTemplate
				End Get
			End Property

			Public Overrides ReadOnly Property Length As Integer
				Get
					Return Me._root.FullSpan.Length
				End Get
			End Property

			Public Overrides ReadOnly Property Options As VisualBasicParseOptions
				Get
					Return Me._options
				End Get
			End Property

			Friend Sub New(ByVal textOpt As SourceText, ByVal encodingOpt As System.Text.Encoding, ByVal checksumAlgorithm As SourceHashAlgorithm, ByVal path As String, ByVal options As VisualBasicParseOptions, ByVal syntaxRoot As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode, ByVal isMyTemplate As Boolean, ByVal diagnosticOptions As ImmutableDictionary(Of String, ReportDiagnostic), Optional ByVal cloneRoot As Boolean = True)
				MyBase.New()
				Me._lazyText = textOpt
				Dim encoding As Object = encodingOpt
				If (encoding Is Nothing) Then
					If (textOpt IsNot Nothing) Then
						encoding = textOpt.Encoding
					Else
						encoding = Nothing
					End If
				End If
				Me._encodingOpt = encoding
				Me._checksumAlgorithm = checksumAlgorithm
				Me._options = options
				Me._path = If(path, [String].Empty)
				Me._root = If(cloneRoot, MyBase.CloneNodeAsRoot(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)(syntaxRoot), syntaxRoot)
				Me._hasCompilationUnitRoot = syntaxRoot.Kind() = SyntaxKind.CompilationUnit
				Me._isMyTemplate = isMyTemplate
				Me._diagnosticOptions = If(diagnosticOptions, SyntaxTree.EmptyDiagnosticOptions)
			End Sub

			Public Overrides Function GetReference(ByVal node As SyntaxNode) As SyntaxReference
				Return New SimpleSyntaxReference(Me, node)
			End Function

			Public Overrides Function GetRoot(Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
				Return Me._root
			End Function

			Public Overrides Function GetRootAsync(Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Task(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)
				Return Task.FromResult(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)(Me._root)
			End Function

			Public Overrides Function GetText(Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SourceText
				If (Me._lazyText Is Nothing) Then
					Dim text As SourceText = Me.GetRoot(cancellationToken).GetText(Me._encodingOpt, Me._checksumAlgorithm)
					Interlocked.CompareExchange(Of SourceText)(Me._lazyText, text, Nothing)
				End If
				Return Me._lazyText
			End Function

			Public Overrides Function TryGetRoot(ByRef root As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode) As Boolean
				root = Me._root
				Return True
			End Function

			Public Overrides Function TryGetText(ByRef text As SourceText) As Boolean
				text = Me._lazyText
				Return text IsNot Nothing
			End Function

			Public Overrides Function WithDiagnosticOptions(ByVal options As ImmutableDictionary(Of String, ReportDiagnostic)) As Microsoft.CodeAnalysis.SyntaxTree
				Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree
				If (options Is Nothing) Then
					options = Microsoft.CodeAnalysis.SyntaxTree.EmptyDiagnosticOptions
				End If
				syntaxTree = If(Not [Object].ReferenceEquals(Me._diagnosticOptions, options), New VisualBasicSyntaxTree.ParsedSyntaxTree(Me._lazyText, Me._encodingOpt, Me._checksumAlgorithm, Me._path, Me._options, Me._root, Me._isMyTemplate, options, True), Me)
				Return syntaxTree
			End Function

			Public Overrides Function WithFilePath(ByVal path As String) As Microsoft.CodeAnalysis.SyntaxTree
				Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree
				syntaxTree = If(Not [String].Equals(Me._path, path), New VisualBasicSyntaxTree.ParsedSyntaxTree(Me._lazyText, Me._encodingOpt, Me._checksumAlgorithm, path, Me._options, Me._root, Me._isMyTemplate, Me._diagnosticOptions, True), Me)
				Return syntaxTree
			End Function

			Public Overrides Function WithRootAndOptions(ByVal root As SyntaxNode, ByVal options As ParseOptions) As Microsoft.CodeAnalysis.SyntaxTree
				Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree
				syntaxTree = If(Me._root <> root OrElse Me._options <> options, New VisualBasicSyntaxTree.ParsedSyntaxTree(Nothing, Me._encodingOpt, Me._checksumAlgorithm, Me._path, DirectCast(options, VisualBasicParseOptions), DirectCast(root, Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode), Me._isMyTemplate, Me._diagnosticOptions, True), Me)
				Return syntaxTree
			End Function
		End Class
	End Class
End Namespace