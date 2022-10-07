Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Text

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend Class Scanner
		Implements IDisposable
		Private _isScanningDirective As Boolean

		Protected _scannerPreprocessorState As Scanner.PreprocessorState

		Private ReadOnly Shared s_charProperties As UShort()

		Private Const s_CHARPROP_LENGTH As Integer = 384

		Friend Const MAX_CACHED_TOKENSIZE As Integer = 42

		Private ReadOnly Shared s_scanNoTriviaFunc As Scanner.ScanTriviaFunc

		Private ReadOnly _scanSingleLineTriviaFunc As Scanner.ScanTriviaFunc

		Protected _lineBufferOffset As Integer

		Private _endOfTerminatorTrivia As Integer

		Friend Const BadTokenCountLimit As Integer = 200

		Private _badTokenCount As Integer

		Private ReadOnly _sbPooled As PooledStringBuilder

		Private ReadOnly _sb As StringBuilder

		Private ReadOnly _triviaListPool As SyntaxListPool

		Private ReadOnly _options As VisualBasicParseOptions

		Private ReadOnly _stringTable As StringTable

		Private ReadOnly _quickTokenTable As TextKeyedCache(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)

		Public Const TABLE_LIMIT As Integer = 512

		Private ReadOnly Shared s_keywordKindFactory As Func(Of String, SyntaxKind)

		Private ReadOnly Shared s_keywordsObjsPool As ObjectPool(Of CachingIdentityFactory(Of String, SyntaxKind))

		Private ReadOnly _KeywordsObjs As CachingIdentityFactory(Of String, SyntaxKind)

		Private ReadOnly Shared s_idTablePool As ObjectPool(Of CachingFactory(Of Scanner.TokenParts, IdentifierTokenSyntax))

		Private ReadOnly _idTable As CachingFactory(Of Scanner.TokenParts, IdentifierTokenSyntax)

		Private ReadOnly Shared s_kwTablePool As ObjectPool(Of CachingFactory(Of Scanner.TokenParts, KeywordSyntax))

		Private ReadOnly _kwTable As CachingFactory(Of Scanner.TokenParts, KeywordSyntax)

		Private ReadOnly Shared s_punctTablePool As ObjectPool(Of CachingFactory(Of Scanner.TokenParts, PunctuationSyntax))

		Private ReadOnly _punctTable As CachingFactory(Of Scanner.TokenParts, PunctuationSyntax)

		Private ReadOnly Shared s_literalTablePool As ObjectPool(Of CachingFactory(Of Scanner.TokenParts, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken))

		Private ReadOnly _literalTable As CachingFactory(Of Scanner.TokenParts, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)

		Private ReadOnly Shared s_wslTablePool As ObjectPool(Of CachingFactory(Of SyntaxListBuilder, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)))

		Private ReadOnly _wslTable As CachingFactory(Of SyntaxListBuilder, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode))

		Private ReadOnly Shared s_wsTablePool As ObjectPool(Of CachingFactory(Of Scanner.TriviaKey, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia))

		Private ReadOnly _wsTable As CachingFactory(Of Scanner.TriviaKey, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia)

		Private ReadOnly _isScanningForExpressionCompiler As Boolean

		Private _isDisposed As Boolean

		Private ReadOnly Shared s_conflictMarkerLength As Integer

		Private _curPage As Scanner.Page

		Private ReadOnly _pages As Scanner.Page()

		Private Const s_PAGE_NUM_SHIFT As Integer = 2

		Private Const s_PAGE_NUM As Integer = 4

		Private Const s_PAGE_NUM_MASK As Integer = 3

		Private Const s_PAGE_SHIFT As Integer = 11

		Private Const s_PAGE_SIZE As Integer = 2048

		Private Const s_PAGE_MASK As Integer = 2047

		Private Const s_NOT_PAGE_MASK As Integer = -2048

		Private ReadOnly _buffer As SourceText

		Private ReadOnly _bufferLen As Integer

		Private _builder As StringBuilder

		Private ReadOnly Shared s_triviaKeyHasher As Func(Of Scanner.TriviaKey, Integer)

		Private ReadOnly Shared s_triviaKeyEquality As Func(Of Scanner.TriviaKey, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia, Boolean)

		Private ReadOnly Shared s_singleSpaceWhitespaceTrivia As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia

		Private ReadOnly Shared s_fourSpacesWhitespaceTrivia As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia

		Private ReadOnly Shared s_eightSpacesWhitespaceTrivia As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia

		Private ReadOnly Shared s_twelveSpacesWhitespaceTrivia As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia

		Private ReadOnly Shared s_sixteenSpacesWhitespaceTrivia As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia

		Private ReadOnly Shared s_wsListKeyHasher As Func(Of SyntaxListBuilder, Integer)

		Private ReadOnly Shared s_wsListKeyEquality As Func(Of SyntaxListBuilder, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), Boolean)

		Private ReadOnly Shared s_wsListFactory As Func(Of SyntaxListBuilder, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode))

		Private ReadOnly Shared s_tokenKeyHasher As Func(Of Scanner.TokenParts, Integer)

		Private ReadOnly Shared s_tokenKeyEquality As Func(Of Scanner.TokenParts, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, Boolean)

		Private ReadOnly Shared s_crLfTrivia As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia

		Private ReadOnly _simpleEof As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken

		Public Const MaxTokensLookAheadBeyondEOL As Integer = 4

		Public Const MaxCharsLookBehind As Integer = 1

		Private _prevToken As Scanner.ScannerToken

		Protected _currentToken As Scanner.ScannerToken

		Private ReadOnly _tokens As List(Of Scanner.ScannerToken)

		Private _IsScanningXmlDoc As Boolean

		Private _endOfXmlInsteadOfLastDocCommentLineBreak As Boolean

		Private _isStartingFirstXmlDocLine As Boolean

		Private _doNotRequireXmlDocCommentPrefix As Boolean

		Private ReadOnly Shared s_xmlAmpToken As XmlTextTokenSyntax

		Private ReadOnly Shared s_xmlAposToken As XmlTextTokenSyntax

		Private ReadOnly Shared s_xmlGtToken As XmlTextTokenSyntax

		Private ReadOnly Shared s_xmlLtToken As XmlTextTokenSyntax

		Private ReadOnly Shared s_xmlQuotToken As XmlTextTokenSyntax

		Private ReadOnly Shared s_docCommentCrLfToken As XmlTextTokenSyntax

		Friend Property IsScanningXmlDoc As Boolean
			Get
				Return Me._IsScanningXmlDoc
			End Get
			Private Set(ByVal value As Boolean)
				Me._IsScanningXmlDoc = value
			End Set
		End Property

		Friend ReadOnly Property LastToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Get
				Dim innerTokenObject As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
				Dim count As Integer = Me._tokens.Count
				If (count <= 0) Then
					innerTokenObject = If(Me._currentToken.InnerTokenObject Is Nothing, Me._prevToken.InnerTokenObject, Me._currentToken.InnerTokenObject)
				Else
					innerTokenObject = Me._tokens(count - 1).InnerTokenObject
				End If
				Return innerTokenObject
			End Get
		End Property

		Friend ReadOnly Property Options As VisualBasicParseOptions
			Get
				Return Me._options
			End Get
		End Property

		Friend ReadOnly Property PrevToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Get
				Return Me._prevToken.InnerTokenObject
			End Get
		End Property

		Private ReadOnly Property ShouldReportXmlError As Boolean
			Get
				If (Not Me._IsScanningXmlDoc) Then
					Return True
				End If
				Return Me._options.DocumentationMode = DocumentationMode.Diagnose
			End Get
		End Property

		Shared Sub New()
			Scanner.s_charProperties = New UInt16() { 512, 512, 512, 512, 512, 512, 512, 512, 512, 1, 128, 512, 512, 64, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 1, 512, 512, 512, 8, 8, 8, 512, 16, 16, 32, 32, 16, 32, 16, 32, 256, 256, 256, 256, 256, 256, 256, 256, 256, 256, 512, 512, 512, 512, 512, 16, 8, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 4, 2, 2, 2, 2, 2, 2, 2, 2, 512, 32, 512, 32, 4, 512, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 4, 2, 2, 2, 2, 2, 2, 2, 2, 16, 512, 16, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 2, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 2, 512, 512, 512, 512, 2, 512, 512, 512, 512, 512, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 512, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 512, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 }
			Scanner.s_scanNoTriviaFunc = Function() New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
			Scanner.s_keywordKindFactory = Function(spelling As String) KeywordTable.TokenOfString(spelling)
			Scanner.s_keywordsObjsPool = CachingIdentityFactory(Of String, SyntaxKind).CreatePool(512, Scanner.s_keywordKindFactory)
			Scanner.s_idTablePool = New ObjectPool(Of CachingFactory(Of Scanner.TokenParts, IdentifierTokenSyntax))(Function() New CachingFactory(Of Scanner.TokenParts, IdentifierTokenSyntax)(512, Nothing, Scanner.s_tokenKeyHasher, Scanner.s_tokenKeyEquality))
			Scanner.s_kwTablePool = New ObjectPool(Of CachingFactory(Of Scanner.TokenParts, KeywordSyntax))(Function() New CachingFactory(Of Scanner.TokenParts, KeywordSyntax)(512, Nothing, Scanner.s_tokenKeyHasher, Scanner.s_tokenKeyEquality))
			Scanner.s_punctTablePool = New ObjectPool(Of CachingFactory(Of Scanner.TokenParts, PunctuationSyntax))(Function() New CachingFactory(Of Scanner.TokenParts, PunctuationSyntax)(512, Nothing, Scanner.s_tokenKeyHasher, Scanner.s_tokenKeyEquality))
			Scanner.s_literalTablePool = New ObjectPool(Of CachingFactory(Of Scanner.TokenParts, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken))(Function() New CachingFactory(Of Scanner.TokenParts, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(512, Nothing, Scanner.s_tokenKeyHasher, Scanner.s_tokenKeyEquality))
			Scanner.s_wslTablePool = New ObjectPool(Of CachingFactory(Of SyntaxListBuilder, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)))(Function() New CachingFactory(Of SyntaxListBuilder, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode))(512, Scanner.s_wsListFactory, Scanner.s_wsListKeyHasher, Scanner.s_wsListKeyEquality))
			Scanner.s_wsTablePool = New ObjectPool(Of CachingFactory(Of Scanner.TriviaKey, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia))(Function() Scanner.CreateWsTable())
			Scanner.s_conflictMarkerLength = "<<<<<<<".Length
			Scanner.s_triviaKeyHasher = Function(key As Scanner.TriviaKey) RuntimeHelpers.GetHashCode(key.spelling) Xor CInt(key.kind)
			Scanner.s_triviaKeyEquality = Function(key As Scanner.TriviaKey, value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia)
				If (CObj(key.spelling) <> CObj(value.Text)) Then
					Return False
				End If
				Return key.kind = value.Kind
			End Function
			Scanner.s_singleSpaceWhitespaceTrivia = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.WhitespaceTrivia(" ")
			Scanner.s_fourSpacesWhitespaceTrivia = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.WhitespaceTrivia("    ")
			Scanner.s_eightSpacesWhitespaceTrivia = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.WhitespaceTrivia("        ")
			Scanner.s_twelveSpacesWhitespaceTrivia = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.WhitespaceTrivia("            ")
			Scanner.s_sixteenSpacesWhitespaceTrivia = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.WhitespaceTrivia("                ")
			Scanner.s_wsListKeyHasher = Function(builder As SyntaxListBuilder)
				Dim num1 As Integer = 0
				Dim num2 As Integer = builder.Count - 1
				Dim num3 As Integer = 0
				Do
					num1 = num1 << 1 Xor RuntimeHelpers.GetHashCode(builder(num3))
					num3 = num3 + 1
				Loop While num3 <= num2
				Return num1
			End Function
			Scanner.s_wsListKeyEquality = Function(builder As SyntaxListBuilder, list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode))
				Dim flag As Boolean
				If (builder.Count = list.Count) Then
					Dim count As Integer = builder.Count - 1
					Dim num As Integer = 0
					While num <= count
						If (builder(num) = list.ItemUntyped(num)) Then
							num = num + 1
						Else
							flag = False
							Return flag
						End If
					End While
					flag = True
				Else
					flag = False
				End If
				Return flag
			End Function
			Scanner.s_wsListFactory = Function(builder As SyntaxListBuilder) builder.ToList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
			Scanner.s_tokenKeyHasher = Function(key As Scanner.TokenParts)
				Dim hashCode As Integer = RuntimeHelpers.GetHashCode(key.spelling)
				Dim greenNode As Microsoft.CodeAnalysis.GreenNode = key.pTrivia
				If (greenNode IsNot Nothing) Then
					hashCode = hashCode Xor RuntimeHelpers.GetHashCode(greenNode) << 1
				End If
				greenNode = key.fTrivia
				If (greenNode IsNot Nothing) Then
					hashCode = hashCode Xor RuntimeHelpers.GetHashCode(greenNode)
				End If
				Return hashCode
			End Function
			Scanner.s_tokenKeyEquality = Function(x As Scanner.TokenParts, y As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) If(y Is Nothing OrElse CObj(x.spelling) <> CObj(y.Text) OrElse x.fTrivia <> y.GetTrailingTrivia() OrElse x.pTrivia <> y.GetLeadingTrivia(), False, True)
			Scanner.s_crLfTrivia = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EndOfLineTrivia("" & VbCrLf & "")
			Scanner.s_xmlAmpToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlEntityLiteralToken("&amp;", "&", Nothing, Nothing)
			Scanner.s_xmlAposToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlEntityLiteralToken("&apos;", "'", Nothing, Nothing)
			Scanner.s_xmlGtToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlEntityLiteralToken("&gt;", ">", Nothing, Nothing)
			Scanner.s_xmlLtToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlEntityLiteralToken("&lt;", "<", Nothing, Nothing)
			Scanner.s_xmlQuotToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlEntityLiteralToken("&quot;", """", Nothing, Nothing)
			Scanner.s_docCommentCrLfToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.DocumentationCommentLineBreakToken("" & VbCrLf & "", "" & VbCrLf & "", Nothing, Nothing)
		End Sub

		Friend Sub New(ByVal textToScan As SourceText, ByVal options As VisualBasicParseOptions, Optional ByVal isScanningForExpressionCompiler As Boolean = False)
			MyBase.New()
			Me._isScanningDirective = False
			Me._scanSingleLineTriviaFunc = New Scanner.ScanTriviaFunc(AddressOf Me.ScanSingleLineTrivia)
			Me._sbPooled = PooledStringBuilder.GetInstance()
			Me._sb = Me._sbPooled.Builder
			Me._triviaListPool = New SyntaxListPool()
			Me._stringTable = StringTable.GetInstance()
			Me._quickTokenTable = TextKeyedCache(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken).GetInstance()
			Me._KeywordsObjs = Scanner.s_keywordsObjsPool.Allocate()
			Me._idTable = Scanner.s_idTablePool.Allocate()
			Me._kwTable = Scanner.s_kwTablePool.Allocate()
			Me._punctTable = Scanner.s_punctTablePool.Allocate()
			Me._literalTable = Scanner.s_literalTablePool.Allocate()
			Me._wslTable = Scanner.s_wslTablePool.Allocate()
			Me._wsTable = Scanner.s_wsTablePool.Allocate()
			ReDim Me._pages(3)
			Me._simpleEof = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Token(Nothing, SyntaxKind.EndOfFileToken, Nothing, [String].Empty)
			Me._tokens = New List(Of Scanner.ScannerToken)()
			Me._IsScanningXmlDoc = False
			Me._isStartingFirstXmlDocLine = False
			Me._doNotRequireXmlDocCommentPrefix = False
			Me._lineBufferOffset = 0
			Me._buffer = textToScan
			Me._bufferLen = textToScan.Length
			Me._curPage = Me.GetPage(0)
			Me._options = options
			Me._scannerPreprocessorState = New Scanner.PreprocessorState(Scanner.GetPreprocessorConstants(options))
			Me._isScanningForExpressionCompiler = isScanningForExpressionCompiler
		End Sub

		Private Sub AbandonAllTokens()
			Me.RevertState(Me._currentToken)
			Me._tokens.Clear()
			Me._currentToken = Me._currentToken.[With](ScannerState.VB, Nothing)
		End Sub

		Private Sub AbandonPeekedTokens()
			If (Me._tokens.Count <> 0) Then
				Me.RevertState(Me._tokens(0))
				Me._tokens.Clear()
			End If
		End Sub

		Private Sub AdvanceChar(Optional ByVal howFar As Integer = 1)
			Me._lineBufferOffset += howFar
		End Sub

		Friend Shared Function ApplyDirective(ByVal preprocessorState As Scanner.PreprocessorState, ByRef statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax) As Scanner.PreprocessorState
			Select Case statement.Kind
				Case SyntaxKind.ConstDirectiveTrivia
					Dim conditionalStack As ImmutableStack(Of Scanner.ConditionalState) = preprocessorState.ConditionalStack
					If (conditionalStack.Count() <> 0 AndAlso conditionalStack.Peek().BranchTaken <> Scanner.ConditionalState.BranchTakenState.Taken) Then
						Return preprocessorState
					End If
					preprocessorState = preprocessorState.InterpretConstDirective(statement)
					Return preprocessorState
				Case SyntaxKind.IfDirectiveTrivia
					preprocessorState = preprocessorState.InterpretIfDirective(statement)
					Return preprocessorState
				Case SyntaxKind.ElseIfDirectiveTrivia
					preprocessorState = preprocessorState.InterpretElseIfDirective(statement)
					Return preprocessorState
				Case SyntaxKind.ElseDirectiveTrivia
					preprocessorState = preprocessorState.InterpretElseDirective(statement)
					Return preprocessorState
				Case SyntaxKind.EndIfDirectiveTrivia
					preprocessorState = preprocessorState.InterpretEndIfDirective(statement)
					Return preprocessorState
				Case SyntaxKind.RegionDirectiveTrivia
					preprocessorState = preprocessorState.InterpretRegionDirective(statement)
					Return preprocessorState
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.EventStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.SyncLockStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OpenParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.EventStatement Or SyntaxKind.OperatorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueDoStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.OnErrorResumeNextStatement Or SyntaxKind.SyncLockStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.StaticKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OffKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OutKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.AsteriskToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.QuestionToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.StatementTerminatorToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.EmptyToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlCrefAttribute Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.IfDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.ElseDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia Or SyntaxKind.RegionDirectiveTrivia
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.SubBlock Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.EventStatement Or SyntaxKind.OperatorStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.AddHandlerAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.RaiseEventAccessorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueDoStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineIfPart Or SyntaxKind.SingleLineElseClause Or SyntaxKind.MultiLineIfBlock Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.OnErrorResumeNextStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectBlock Or SyntaxKind.SelectStatement Or SyntaxKind.CaseBlock Or SyntaxKind.SyncLockStatement Or SyntaxKind.WhileStatement Or SyntaxKind.ForBlock Or SyntaxKind.ForEachBlock Or SyntaxKind.ForStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OrElseKeyword Or SyntaxKind.OverloadsKeyword Or SyntaxKind.OverridableKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.StaticKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.StructureKeyword Or SyntaxKind.SubKeyword Or SyntaxKind.SyncLockKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AscendingKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.AutoKeyword Or SyntaxKind.BinaryKeyword Or SyntaxKind.ByKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OffKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OutKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.RegionKeyword Or SyntaxKind.SkipKeyword Or SyntaxKind.StrictKeyword Or SyntaxKind.TakeKeyword Or SyntaxKind.TextKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.AsteriskToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.DotToken Or SyntaxKind.SlashToken Or SyntaxKind.ColonToken Or SyntaxKind.LessThanToken Or SyntaxKind.LessThanEqualsToken Or SyntaxKind.LessThanGreaterThanToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.QuestionToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.StatementTerminatorToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.EmptyToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanSlashToken Or SyntaxKind.LessThanExclamationMinusMinusToken Or SyntaxKind.MinusMinusGreaterThanToken Or SyntaxKind.LessThanQuestionToken Or SyntaxKind.QuestionGreaterThanToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlCrefAttribute Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.IfDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.ElseDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia Or SyntaxKind.RegionDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.ExternalSourceDirectiveTrivia Or SyntaxKind.EndExternalSourceDirectiveTrivia Or SyntaxKind.ExternalChecksumDirectiveTrivia Or SyntaxKind.EnableWarningDirectiveTrivia Or SyntaxKind.DisableWarningDirectiveTrivia Or SyntaxKind.ReferenceDirectiveTrivia
				Case SyntaxKind.EndFunctionStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.FunctionBlock Or SyntaxKind.DeclareSubStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.UsingBlock Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ForStepClause Or SyntaxKind.NotKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.ConstDirectiveTrivia
					Throw ExceptionUtilities.UnexpectedValue(statement.Kind)
				Case SyntaxKind.EndRegionDirectiveTrivia
					preprocessorState = preprocessorState.InterpretEndRegionDirective(statement)
					Return preprocessorState
				Case SyntaxKind.ExternalSourceDirectiveTrivia
					preprocessorState = preprocessorState.InterpretExternalSourceDirective(statement)
					Return preprocessorState
				Case SyntaxKind.EndExternalSourceDirectiveTrivia
					preprocessorState = preprocessorState.InterpretEndExternalSourceDirective(statement)
					Return preprocessorState
				Case SyntaxKind.ExternalChecksumDirectiveTrivia
				Case SyntaxKind.EnableWarningDirectiveTrivia
				Case SyntaxKind.DisableWarningDirectiveTrivia
				Case SyntaxKind.ReferenceDirectiveTrivia
				Case SyntaxKind.BadDirectiveTrivia
					Return preprocessorState
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(statement.Kind)
			End Select
		End Function

		Protected Shared Function ApplyDirectives(ByVal preprocessorState As Scanner.PreprocessorState, ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Scanner.PreprocessorState
			If (node.ContainsDirectives) Then
				preprocessorState = Scanner.ApplyDirectivesRecursive(preprocessorState, node)
			End If
			Return preprocessorState
		End Function

		Private Shared Function ApplyDirectivesRecursive(ByVal preprocessorState As Scanner.PreprocessorState, ByVal node As GreenNode) As Scanner.PreprocessorState
			Dim preprocessorState1 As Scanner.PreprocessorState
			Dim directiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax)
			If (directiveTriviaSyntax Is Nothing) Then
				Dim slotCount As Integer = node.SlotCount
				If (slotCount <= 0) Then
					Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
					Dim leadingTrivia As GreenNode = syntaxToken.GetLeadingTrivia()
					If (leadingTrivia IsNot Nothing AndAlso leadingTrivia.ContainsDirectives) Then
						preprocessorState = Scanner.ApplyDirectivesRecursive(preprocessorState, leadingTrivia)
					End If
					leadingTrivia = syntaxToken.GetTrailingTrivia()
					If (leadingTrivia IsNot Nothing AndAlso leadingTrivia.ContainsDirectives) Then
						preprocessorState = Scanner.ApplyDirectivesRecursive(preprocessorState, leadingTrivia)
					End If
					preprocessorState1 = preprocessorState
				Else
					Dim num As Integer = slotCount - 1
					Dim num1 As Integer = 0
					Do
						Dim slot As GreenNode = node.GetSlot(num1)
						If (slot IsNot Nothing AndAlso slot.ContainsDirectives) Then
							preprocessorState = Scanner.ApplyDirectivesRecursive(preprocessorState, slot)
						End If
						num1 = num1 + 1
					Loop While num1 <= num
					preprocessorState1 = preprocessorState
				End If
			Else
				Dim directiveTriviaSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax = directiveTriviaSyntax
				preprocessorState = Scanner.ApplyDirective(preprocessorState, directiveTriviaSyntax1)
				preprocessorState1 = preprocessorState
			End If
			Return preprocessorState1
		End Function

		Private Shared Function CanCache(ByVal trivia As SyntaxListBuilder) As Boolean
			Dim flag As Boolean
			Dim count As Integer = trivia.Count - 1
			Dim num As Integer = 0
			While True
				If (num <= count) Then
					Dim rawKind As Integer = trivia(num).RawKind
					If (rawKind - 729 <= 1 OrElse rawKind - 733 <= 1) Then
						num = num + 1
					Else
						flag = False
						Exit While
					End If
				Else
					flag = True
					Exit While
				End If
			End While
			Return flag
		End Function

		Private Function CanGet() As Boolean
			Return Me._lineBufferOffset < Me._bufferLen
		End Function

		Private Function CanGet(ByVal num As Integer) As Boolean
			Return Me._lineBufferOffset + num < Me._bufferLen
		End Function

		Private Function CheckFeatureAvailability(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal feature As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			If (Not Me.CheckFeatureAvailability(feature)) Then
				Dim visualBasicRequiredLanguageVersion As Microsoft.CodeAnalysis.VisualBasic.VisualBasicRequiredLanguageVersion = New Microsoft.CodeAnalysis.VisualBasic.VisualBasicRequiredLanguageVersion(feature.GetLanguageVersion())
				Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_LanguageVersion, New [Object]() { Me._options.LanguageVersion.GetErrorName(), ErrorFactory.ErrorInfo(feature.GetResourceId()), visualBasicRequiredLanguageVersion })
				syntaxToken = DirectCast(token.AddError(diagnosticInfo), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			Else
				syntaxToken = token
			End If
			Return syntaxToken
		End Function

		Friend Function CheckFeatureAvailability(ByVal feature As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature) As Boolean
			Return Scanner.CheckFeatureAvailability(Me.Options, feature)
		End Function

		Private Shared Function CheckFeatureAvailability(ByVal parseOptions As VisualBasicParseOptions, ByVal feature As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature) As Boolean
			Dim flag As Boolean
			Dim featureFlag As String = feature.GetFeatureFlag()
			flag = If(featureFlag Is Nothing, feature.GetLanguageVersion() <= parseOptions.LanguageVersion, parseOptions.Features.ContainsKey(featureFlag))
			Return flag
		End Function

		Private Function CreateOffsetRestorePoint() As Scanner.LineBufferAndEndOfTerminatorOffsets
			Return New Scanner.LineBufferAndEndOfTerminatorOffsets(Me)
		End Function

		Friend Function CreateRestorePoint() As Scanner.RestorePoint
			Return New Scanner.RestorePoint(Me)
		End Function

		Private Shared Function CreateWsTable() As CachingFactory(Of Scanner.TriviaKey, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia)
			Dim cachingFactory As CachingFactory(Of Scanner.TriviaKey, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia) = New CachingFactory(Of Scanner.TriviaKey, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia)(512, Nothing, Scanner.s_triviaKeyHasher, Scanner.s_triviaKeyEquality)
			cachingFactory.Add(New Scanner.TriviaKey(" ", SyntaxKind.WhitespaceTrivia), Scanner.s_singleSpaceWhitespaceTrivia)
			cachingFactory.Add(New Scanner.TriviaKey("    ", SyntaxKind.WhitespaceTrivia), Scanner.s_fourSpacesWhitespaceTrivia)
			cachingFactory.Add(New Scanner.TriviaKey("        ", SyntaxKind.WhitespaceTrivia), Scanner.s_eightSpacesWhitespaceTrivia)
			cachingFactory.Add(New Scanner.TriviaKey("            ", SyntaxKind.WhitespaceTrivia), Scanner.s_twelveSpacesWhitespaceTrivia)
			cachingFactory.Add(New Scanner.TriviaKey("                ", SyntaxKind.WhitespaceTrivia), Scanner.s_sixteenSpacesWhitespaceTrivia)
			Return cachingFactory
		End Function

		Friend Sub Dispose() Implements IDisposable.Dispose
			If (Not Me._isDisposed) Then
				Me._isDisposed = True
				Me._KeywordsObjs.Free()
				Me._quickTokenTable.Free()
				Me._stringTable.Free()
				Me._sbPooled.Free()
				Scanner.s_idTablePool.Free(Me._idTable)
				Scanner.s_kwTablePool.Free(Me._kwTable)
				Scanner.s_punctTablePool.Free(Me._punctTable)
				Scanner.s_literalTablePool.Free(Me._literalTable)
				Scanner.s_wslTablePool.Free(Me._wslTable)
				Scanner.s_wsTablePool.Free(Me._wsTable)
				Dim pageArray As Scanner.Page() = Me._pages
				Dim num As Integer = 0
				Do
					Dim page As Scanner.Page = pageArray(num)
					If (page IsNot Nothing) Then
						page.Free()
					End If
					num = num + 1
				Loop While num < CInt(pageArray.Length)
				Array.Clear(Me._pages, 0, CInt(Me._pages.Length))
			End If
		End Sub

		Private Sub EatThroughLine()
			While Me.CanGet()
				Dim chr As Char = Me.Peek()
				If (SyntaxFacts.IsNewLine(chr)) Then
					Me.EatThroughLineBreak(chr)
					Return
				End If
				Me.AdvanceChar(1)
			End While
		End Sub

		Private Sub EatThroughLineBreak(ByVal StartCharacter As Char)
			Me.AdvanceChar(Me.LengthOfLineBreak(StartCharacter, 0))
		End Sub

		Private Sub EatWhitespace()
			Me.AdvanceChar(1)
			While Me.CanGet() AndAlso SyntaxFacts.IsWhitespace(Me.Peek())
				Me.AdvanceChar(1)
			End While
		End Sub

		Friend Sub ForceScanningXmlDocMode()
			Me.IsScanningXmlDoc = True
			Me._isStartingFirstXmlDocLine = False
			Me._doNotRequireXmlDocCommentPrefix = True
		End Sub

		Friend Function GetChar() As String
			Return Me.Intern(Me.Peek())
		End Function

		Friend Overridable Function GetCurrentSyntaxNode() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Nothing
		End Function

		Friend Function GetCurrentToken() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim innerTokenObject As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me._currentToken.InnerTokenObject
			If (innerTokenObject Is Nothing) Then
				Dim state As ScannerState = Me._currentToken.State
				innerTokenObject = Me.GetScannerToken(state)
				Me._currentToken = Me._currentToken.[With](state, innerTokenObject)
			End If
			Return innerTokenObject
		End Function

		Private Shared Function GetDecimalValue(ByVal text As String, <Out> ByRef value As [Decimal]) As Boolean
			Return [Decimal].TryParse(text, NumberStyles.AllowDecimalPoint Or NumberStyles.AllowExponent, CultureInfo.InvariantCulture, value)
		End Function

		Friend Function GetDisabledTextAt(ByVal span As TextSpan) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			If (span.Start < 0 OrElse span.[End] > Me._bufferLen) Then
				Throw New ArgumentOutOfRangeException("span")
			End If
			Dim syntaxTrivium As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.DisabledTextTrivia(Me.GetTextNotInterned(span.Start, span.Length))
			Return syntaxTrivium
		End Function

		Private Shared Function GetFullWidth(ByVal token As Scanner.ScannerToken, ByVal tk As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Integer
			Dim num As Integer
			num = If(tk.Width <> 0 OrElse Not SyntaxFacts.IsTerminator(tk.Kind), tk.FullWidth, token.EndOfTerminatorTrivia - token.Position)
			Return num
		End Function

		Private Function GetNextChar() As String
			Dim chr As String = Me.GetChar()
			Me._lineBufferOffset = Me._lineBufferOffset + 1
			Return chr
		End Function

		Private Function GetNextToken(Optional ByVal allowLeadingMultilineTrivia As Boolean = False) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim quickScanResult As Scanner.QuickScanResult = Me.QuickScanToken(allowLeadingMultilineTrivia)
			If (quickScanResult.Succeeded) Then
				Dim syntaxToken2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me._quickTokenTable.FindItem(quickScanResult.Chars, quickScanResult.Start, quickScanResult.Length, quickScanResult.HashCode)
				If (syntaxToken2 Is Nothing) Then
					syntaxToken1 = Me.ScanNextToken(allowLeadingMultilineTrivia)
					If (quickScanResult.Succeeded) Then
						Me._quickTokenTable.AddItem(quickScanResult.Chars, quickScanResult.Start, quickScanResult.Length, quickScanResult.HashCode, syntaxToken1)
					End If
					syntaxToken = syntaxToken1
					Return syntaxToken
				End If
				Me.AdvanceChar(quickScanResult.Length)
				If (quickScanResult.TerminatorLength <> 0) Then
					Me._endOfTerminatorTrivia = Me._lineBufferOffset
					Me._lineBufferOffset -= quickScanResult.TerminatorLength
				End If
				syntaxToken = syntaxToken2
				Return syntaxToken
			End If
			syntaxToken1 = Me.ScanNextToken(allowLeadingMultilineTrivia)
			If (quickScanResult.Succeeded) Then
				Me._quickTokenTable.AddItem(quickScanResult.Chars, quickScanResult.Start, quickScanResult.Length, quickScanResult.HashCode, syntaxToken1)
			End If
			syntaxToken = syntaxToken1
			Return syntaxToken
		End Function

		Friend Sub GetNextTokenInState(ByVal state As ScannerState)
			Me._prevToken = Me._currentToken
			If (Me._tokens.Count = 0) Then
				Me._currentToken = New Scanner.ScannerToken(Me._scannerPreprocessorState, Me._lineBufferOffset, Me._endOfTerminatorTrivia, Nothing, state)
				Return
			End If
			Me._currentToken = Me._tokens(0)
			Me._tokens.RemoveAt(0)
			Me.ResetCurrentToken(state)
		End Sub

		Private Function GetPage(ByVal position As Integer) As Scanner.Page
			Dim num As Integer = position >> 11 And 3
			Dim instance As Scanner.Page = Me._pages(num)
			Dim num1 As Integer = position And -2048
			If (instance Is Nothing) Then
				instance = Scanner.Page.GetInstance()
				Me._pages(num) = instance
			End If
			If (instance._pageStart <> num1) Then
				Me._buffer.CopyTo(num1, instance._arr, 0, Math.Min(Me._bufferLen - num1, 2048))
				instance._pageStart = num1
			End If
			Me._curPage = instance
			Return instance
		End Function

		Friend Shared Function GetPreprocessorConstants(ByVal options As VisualBasicParseOptions) As ImmutableDictionary(Of String, CConst)
			Dim immutable As ImmutableDictionary(Of String, CConst)
			If (Not options.PreprocessorSymbols.IsDefaultOrEmpty) Then
				Dim strs As ImmutableDictionary(Of String, CConst).Builder = ImmutableDictionary.CreateBuilder(Of String, CConst)(CaseInsensitiveComparison.Comparer)
				Dim enumerator As ImmutableArray(Of KeyValuePair(Of String, Object)).Enumerator = options.PreprocessorSymbols.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As KeyValuePair(Of String, Object) = enumerator.Current
					strs(current.Key) = CConst.CreateChecked(RuntimeHelpers.GetObjectValue(current.Value))
				End While
				immutable = strs.ToImmutable()
			Else
				immutable = ImmutableDictionary(Of String, CConst).Empty
			End If
			Return immutable
		End Function

		Private Function GetScannerToken(ByVal state As ScannerState) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim nextToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Nothing
			Select Case state
				Case ScannerState.VB
					nextToken = Me.GetNextToken(False)
					Exit Select
				Case ScannerState.VBAllowLeadingMultilineTrivia
					nextToken = Me.GetNextToken(Not Me._isScanningDirective)
					Exit Select
				Case ScannerState.Misc
					nextToken = Me.ScanXmlMisc()
					Exit Select
				Case ScannerState.DocType
				Case ScannerState.Element
				Case ScannerState.EndElement
					nextToken = Me.ScanXmlElement(state)
					Exit Select
				Case ScannerState.SingleQuotedString
					nextToken = Me.ScanXmlStringSingle()
					Exit Select
				Case ScannerState.SmartSingleQuotedString
					nextToken = Me.ScanXmlStringSmartSingle()
					Exit Select
				Case ScannerState.QuotedString
					nextToken = Me.ScanXmlStringDouble()
					Exit Select
				Case ScannerState.SmartQuotedString
					nextToken = Me.ScanXmlStringSmartDouble()
					Exit Select
				Case ScannerState.UnQuotedString
					nextToken = Me.ScanXmlStringUnQuoted()
					Exit Select
				Case ScannerState.Content
					nextToken = Me.ScanXmlContent()
					Exit Select
				Case ScannerState.CData
					nextToken = Me.ScanXmlCData()
					Exit Select
				Case ScannerState.StartProcessingInstruction
				Case ScannerState.ProcessingInstruction
					nextToken = Me.ScanXmlPIData(state)
					Exit Select
				Case ScannerState.Comment
					nextToken = Me.ScanXmlComment()
					Exit Select
				Case ScannerState.InterpolatedStringPunctuation
					nextToken = Me.ScanInterpolatedStringPunctuation()
					Exit Select
				Case ScannerState.InterpolatedStringContent
					nextToken = Me.ScanInterpolatedStringContent()
					Exit Select
				Case ScannerState.InterpolatedStringFormatString
					nextToken = Me.ScanInterpolatedStringFormatString()
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(state)
			End Select
			Return nextToken
		End Function

		Private Function GetScratch() As StringBuilder
			Return Me._sb
		End Function

		Friend Shared Function GetScratchText(ByVal sb As StringBuilder) As String
			Dim str As String
			str = If(sb.Length <> 1 OrElse sb(0) <> Strings.ChrW(32), sb.ToString(), " ")
			sb.Clear()
			Return str
		End Function

		Private Shared Function GetScratchText(ByVal sb As StringBuilder, ByVal text As String) As String
			Dim str As String
			str = If(Not StringTable.TextEquals(text, sb), sb.ToString(), text)
			sb.Clear()
			Return str
		End Function

		Friend Function GetScratchTextInterned(ByVal sb As StringBuilder) As String
			Dim str As String = Me._stringTable.Add(sb)
			sb.Clear()
			Return str
		End Function

		Private Function GetText(ByVal length As Integer) As String
			Dim nextChar As String
			If (length <> 1) Then
				Dim text As String = Me.GetText(Me._lineBufferOffset, length)
				Me.AdvanceChar(length)
				nextChar = text
			Else
				nextChar = Me.GetNextChar()
			End If
			Return nextChar
		End Function

		Friend Function GetText(ByVal start As Integer, ByVal length As Integer) As String
			Dim str As String
			Dim page As Scanner.Page = Me._curPage
			Dim num As Integer = start And 2047
			str = If(page._pageStart <> (start And -2048) OrElse num + length >= 2048, Me.GetTextSlow(start, length, False), Me.Intern(page._arr, num, length))
			Return str
		End Function

		Private Function GetTextNotInterned(ByVal length As Integer) As String
			Dim nextChar As String
			If (length <> 1) Then
				Dim textNotInterned As String = Me.GetTextNotInterned(Me._lineBufferOffset, length)
				Me.AdvanceChar(length)
				nextChar = textNotInterned
			Else
				nextChar = Me.GetNextChar()
			End If
			Return nextChar
		End Function

		Friend Function GetTextNotInterned(ByVal start As Integer, ByVal length As Integer) As String
			Dim textSlow As String
			Dim page As Scanner.Page = Me._curPage
			Dim num As Integer = start And 2047
			If (page._pageStart <> (start And -2048) OrElse num + length >= 2048) Then
				textSlow = Me.GetTextSlow(start, length, True)
			Else
				Dim chrArray As Char() = page._arr
				textSlow = If(length <> 2 OrElse chrArray(num) <> Strings.ChrW(13) OrElse chrArray(num + 1) <> Strings.ChrW(10), New [String](chrArray, num, length), "" & VbCrLf & "")
			End If
			Return textSlow
		End Function

		Private Function GetTextSlow(ByVal start As Integer, ByVal length As Integer, Optional ByVal suppressInterning As Boolean = False) As String
			Dim str As String
			Dim str1 As String
			Dim num As Integer = start And 2047
			Dim page As Scanner.Page = Me.GetPage(start)
			If (num + length >= 2048) Then
				If (Me._builder Is Nothing) Then
					Me._builder = New StringBuilder(Math.Min(length, 1024))
				End If
				Dim num1 As Integer = Math.Min(length, 2048 - num)
				Me._builder.Append(page._arr, num, num1)
				Dim num2 As Integer = num1
				length -= num1
				start += num1
				Do
					page = Me.GetPage(start)
					num1 = Math.Min(length, 2048)
					Me._builder.Append(page._arr, 0, num1)
					num2 += num1
					length -= num1
					start += num1
				Loop While length > 0
				str1 = If(Not suppressInterning, Me._stringTable.Add(Me._builder), Me._builder.ToString())
				If (str1.Length >= 1024) Then
					Me._builder = Nothing
				Else
					Me._builder.Clear()
				End If
				str = str1
			Else
				str = If(Not suppressInterning, Me.Intern(page._arr, num, length), New [String](page._arr, num, length))
			End If
			Return str
		End Function

		Private Function GetTokenAndAddToQueue(ByVal state As ScannerState) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim num As Integer = Me._lineBufferOffset
			Dim num1 As Integer = Me._endOfTerminatorTrivia
			Dim preprocessorState As Scanner.PreprocessorState = Me._scannerPreprocessorState
			Dim scannerToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.GetScannerToken(state)
			Me._tokens.Add(New Scanner.ScannerToken(preprocessorState, num, num1, scannerToken, state))
			Return scannerToken
		End Function

		Private Function GetWhitespaceLength(ByVal len As Integer) As Integer
			While Me.CanGet(len) AndAlso SyntaxFacts.IsWhitespace(Me.Peek(len))
				len = len + 1
			End While
			Return len
		End Function

		Private Function GetXmlWhitespaceLength(ByVal len As Integer) As Integer
			While Me.CanGet(len) AndAlso SyntaxFacts.IsXmlWhitespace(Me.Peek(len))
				len = len + 1
			End While
			Return len
		End Function

		Friend Function Intern(ByVal s As String, ByVal start As Integer, ByVal length As Integer) As String
			Return Me._stringTable.Add(s, start, length)
		End Function

		Friend Function Intern(ByVal s As Char(), ByVal start As Integer, ByVal length As Integer) As String
			Return Me._stringTable.Add(s, start, length)
		End Function

		Friend Function Intern(ByVal ch As Char) As String
			Return Me._stringTable.Add(ch)
		End Function

		Friend Function Intern(ByVal arr As Char()) As String
			Return Me._stringTable.Add(New [String](arr))
		End Function

		Private Function IsAfterWhitespace() As Boolean
			Dim flag As Boolean
			flag = If(Me._lineBufferOffset <> 0, SyntaxFacts.IsWhitespace(Me.Peek(-1)), True)
			Return flag
		End Function

		Private Function IsAtNewLine() As Boolean
			If (Me._lineBufferOffset = 0) Then
				Return True
			End If
			Return SyntaxFacts.IsNewLine(Me.Peek(-1))
		End Function

		Private Shared Function IsBlankLine(ByVal tList As SyntaxListBuilder) As Boolean
			Dim flag As Boolean
			Dim count As Integer = tList.Count
			If (count = 0 OrElse tList(count - 1).RawKind <> 730) Then
				flag = False
			Else
				Dim num As Integer = count - 2
				Dim num1 As Integer = 0
				While num1 <= num
					If (tList(num1).RawKind = 729) Then
						num1 = num1 + 1
					Else
						flag = False
						Return flag
					End If
				End While
				flag = True
			End If
			Return flag
		End Function

		Private Function IsColonAndNotColonEquals(ByVal ch As Char, ByVal offset As Integer) As Boolean
			If (Not SyntaxFacts.IsColon(ch)) Then
				Return False
			End If
			Dim num As Integer = offset + 1
			Return Not Me.TrySkipFollowingEquals(num)
		End Function

		Private Function IsConflictMarkerTrivia() As Boolean
			Dim flag As Boolean
			If (Me.CanGet()) Then
				Dim chr As Char = Me.Peek()
				If (chr = "<"C OrElse chr = ">"C OrElse chr = "="C) Then
					Dim num As Integer = Me._lineBufferOffset
					Dim sourceText As Microsoft.CodeAnalysis.Text.SourceText = Me._buffer
					If (num = 0 OrElse SyntaxFacts.IsNewLine(sourceText(num - 1))) Then
						Dim item As Char = Me._buffer(num)
						If (num + Scanner.s_conflictMarkerLength > sourceText.Length) Then
							flag = False
							Return flag
						End If
						Dim sConflictMarkerLength As Integer = Scanner.s_conflictMarkerLength - 1
						Dim num1 As Integer = 0
						While num1 <= sConflictMarkerLength
							If (sourceText(num + num1) = item) Then
								num1 = num1 + 1
							Else
								flag = False
								Return flag
							End If
						End While
						If (item <> "="C) Then
							flag = If(num + Scanner.s_conflictMarkerLength >= sourceText.Length, False, sourceText(num + Scanner.s_conflictMarkerLength) = Strings.ChrW(32))
							Return flag
						Else
							flag = True
							Return flag
						End If
					End If
				End If
			End If
			flag = False
			Return flag
		End Function

		Friend Shared Function IsContextualKeyword(ByVal t As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal ParamArray kinds As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind()) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
			flag = If(Not Scanner.TryTokenAsKeyword(t, syntaxKind), False, Array.IndexOf(Of Microsoft.CodeAnalysis.VisualBasic.SyntaxKind)(kinds, syntaxKind) >= 0)
			Return flag
		End Function

		Friend Shared Function IsIdentifier(ByVal spelling As String) As Boolean
			Dim flag As Boolean
			Dim length As Integer = spelling.Length
			If (length <> 0) Then
				Dim chr As Char = spelling(0)
				If (SyntaxFacts.IsIdentifierStartCharacter(chr)) Then
					If (Not SyntaxFacts.IsConnectorPunctuation(chr) OrElse length <> 1) Then
						Dim num As Integer = length - 1
						Dim num1 As Integer = 1
						While num1 <= num
							If (SyntaxFacts.IsIdentifierPartCharacter(spelling(num1))) Then
								num1 = num1 + 1
							Else
								flag = False
								Return flag
							End If
						End While
					Else
						flag = False
						Return flag
					End If
				End If
				flag = True
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function IsIdentifierStartCharacter(ByVal c As Char) As Boolean
			If (Me._isScanningForExpressionCompiler AndAlso c = "$"C) Then
				Return True
			End If
			Return SyntaxFacts.IsIdentifierStartCharacter(c)
		End Function

		Private Function IsInterpolatedStringPunctuation(Optional ByVal offset As Integer = 0) As Boolean
			Dim flag As Boolean
			If (Me.CanGet(offset)) Then
				Dim chr As Char = Me.Peek(offset)
				If (SyntaxFacts.IsLeftCurlyBracket(chr)) Then
					flag = If(Not Me.CanGet(offset + 1), True, Not SyntaxFacts.IsLeftCurlyBracket(Me.Peek(offset + 1)))
				ElseIf (SyntaxFacts.IsRightCurlyBracket(chr)) Then
					flag = If(Not Me.CanGet(offset + 1), True, Not SyntaxFacts.IsRightCurlyBracket(Me.Peek(offset + 1)))
				ElseIf (Not SyntaxFacts.IsDoubleQuote(chr)) Then
					flag = False
				Else
					flag = If(Not Me.CanGet(offset + 1), True, Not SyntaxFacts.IsDoubleQuote(Me.Peek(offset + 1)))
				End If
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function LengthOfLineBreak(ByVal StartCharacter As Char, Optional ByVal here As Integer = 0) As Integer
			Dim num As Integer
			num = If(StartCharacter <> Strings.ChrW(13) OrElse Not Me.NextIs(here + 1, Strings.ChrW(10)), 1, 2)
			Return num
		End Function

		Private Function MakeAmpersandEqualsToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal length As Integer) As PunctuationSyntax
			Return Me.MakePunctuationToken(precedingTrivia, length, SyntaxKind.AmpersandEqualsToken)
		End Function

		Private Function MakeAmpersandToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal charIsFullWidth As Boolean) As PunctuationSyntax
			Dim str As String = If(charIsFullWidth, "＆", "&")
			Me.AdvanceChar(1)
			Return Me.MakePunctuationToken(precedingTrivia, str, SyntaxKind.AmpersandToken)
		End Function

		Private Function MakeAsteriskEqualsToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal length As Integer) As PunctuationSyntax
			Return Me.MakePunctuationToken(precedingTrivia, length, SyntaxKind.AsteriskEqualsToken)
		End Function

		Private Function MakeAsteriskToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal charIsFullWidth As Boolean) As PunctuationSyntax
			Dim str As String = If(charIsFullWidth, "＊", "*")
			Me.AdvanceChar(1)
			Return Me.MakePunctuationToken(precedingTrivia, str, SyntaxKind.AsteriskToken)
		End Function

		Private Function MakeAtToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal charIsFullWidth As Boolean) As PunctuationSyntax
			Dim str As String = If(charIsFullWidth, "＠", "@")
			Me.AdvanceChar(1)
			Return Me.MakePunctuationToken(precedingTrivia, str, SyntaxKind.AtToken)
		End Function

		Private Function MakeBackSlashEqualsToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal length As Integer) As PunctuationSyntax
			Return Me.MakePunctuationToken(precedingTrivia, length, SyntaxKind.BackslashEqualsToken)
		End Function

		Private Function MakeBackslashToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal charIsFullWidth As Boolean) As PunctuationSyntax
			Dim str As String = If(charIsFullWidth, "＼", "\")
			Me.AdvanceChar(1)
			Return Me.MakePunctuationToken(precedingTrivia, str, SyntaxKind.BackslashToken)
		End Function

		Private Function MakeBadToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal length As Integer, ByVal errId As Microsoft.CodeAnalysis.VisualBasic.ERRID) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim textNotInterned As String = Me.GetTextNotInterned(length)
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = Me.ScanSingleLineTrivia()
			Return DirectCast(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.BadToken(SyntaxSubKind.None, textNotInterned, precedingTrivia.Node, syntaxList.Node).AddError(ErrorFactory.ErrorInfo(errId)), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
		End Function

		Private Function MakeCaretEqualsToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal length As Integer) As PunctuationSyntax
			Return Me.MakePunctuationToken(precedingTrivia, length, SyntaxKind.CaretEqualsToken)
		End Function

		Private Function MakeCaretToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal charIsFullWidth As Boolean) As PunctuationSyntax
			Dim str As String = If(charIsFullWidth, "＾", "^")
			Me.AdvanceChar(1)
			Return Me.MakePunctuationToken(precedingTrivia, str, SyntaxKind.CaretToken)
		End Function

		Private Function MakeCharacterLiteralToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal value As Char, ByVal length As Integer) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim text As String = Me.GetText(length)
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = Me.ScanSingleLineTrivia()
			Dim tokenPart As Scanner.TokenParts = New Scanner.TokenParts(precedingTrivia, syntaxList, text)
			Dim syntaxToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Nothing
			If (Not Me._literalTable.TryGetValue(tokenPart, syntaxToken1)) Then
				syntaxToken1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.CharacterLiteralToken(text, value, precedingTrivia.Node, syntaxList.Node)
				Me._literalTable.Add(tokenPart, syntaxToken1)
				syntaxToken = syntaxToken1
			Else
				syntaxToken = syntaxToken1
			End If
			Return syntaxToken
		End Function

		Private Function MakeCloseBraceToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal charIsFullWidth As Boolean) As PunctuationSyntax
			Dim str As String = If(charIsFullWidth, "｝", "}")
			Me.AdvanceChar(1)
			Return Me.MakePunctuationToken(precedingTrivia, str, SyntaxKind.CloseBraceToken)
		End Function

		Private Function MakeCloseParenToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal charIsFullWidth As Boolean) As PunctuationSyntax
			Dim str As String = If(charIsFullWidth, "）", ")")
			Me.AdvanceChar(1)
			Return Me.MakePunctuationToken(precedingTrivia, str, SyntaxKind.CloseParenToken)
		End Function

		Private Function MakeColonEqualsToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal length As Integer) As PunctuationSyntax
			Return Me.MakePunctuationToken(precedingTrivia, length, SyntaxKind.ColonEqualsToken)
		End Function

		Private Function MakeColonToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal charIsFullWidth As Boolean) As PunctuationSyntax
			Me.AdvanceChar(Me._endOfTerminatorTrivia - Me._lineBufferOffset)
			Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.ColonToken
		End Function

		Friend Function MakeColonTrivia(ByVal text As String) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Dim syntaxTrivium As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia = Nothing
			Dim triviaKey As Scanner.TriviaKey = New Scanner.TriviaKey(text, SyntaxKind.ColonTrivia)
			If (Not Me._wsTable.TryGetValue(triviaKey, syntaxTrivium)) Then
				syntaxTrivium = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.ColonTrivia(text)
				Me._wsTable.Add(triviaKey, syntaxTrivium)
			End If
			Return syntaxTrivium
		End Function

		Private Function MakeCommaToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal charIsFullWidth As Boolean) As PunctuationSyntax
			Dim str As String = If(charIsFullWidth, "，", ",")
			Me.AdvanceChar(1)
			Return Me.MakePunctuationToken(precedingTrivia, str, SyntaxKind.CommaToken)
		End Function

		Friend Shared Function MakeCommentTrivia(ByVal text As String) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.SyntaxTrivia(SyntaxKind.CommentTrivia, text)
		End Function

		Private Function MakeDateLiteralToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal value As DateTime, ByVal length As Integer) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim text As String = Me.GetText(length)
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = Me.ScanSingleLineTrivia()
			Dim tokenPart As Scanner.TokenParts = New Scanner.TokenParts(precedingTrivia, syntaxList, text)
			Dim syntaxToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Nothing
			If (Not Me._literalTable.TryGetValue(tokenPart, syntaxToken1)) Then
				syntaxToken1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.DateLiteralToken(text, value, precedingTrivia.Node, syntaxList.Node)
				Me._literalTable.Add(tokenPart, syntaxToken1)
				syntaxToken = syntaxToken1
			Else
				syntaxToken = syntaxToken1
			End If
			Return syntaxToken
		End Function

		Private Function MakeDecimalLiteralToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal typeCharacter As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter, ByVal decimalValue As [Decimal], ByVal length As Integer) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim text As String = Me.GetText(length)
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = Me.ScanSingleLineTrivia()
			Dim tokenPart As Scanner.TokenParts = New Scanner.TokenParts(precedingTrivia, syntaxList, text)
			Dim syntaxToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Nothing
			If (Not Me._literalTable.TryGetValue(tokenPart, syntaxToken1)) Then
				syntaxToken1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.DecimalLiteralToken(text, typeCharacter, decimalValue, precedingTrivia.Node, syntaxList.Node)
				Me._literalTable.Add(tokenPart, syntaxToken1)
				syntaxToken = syntaxToken1
			Else
				syntaxToken = syntaxToken1
			End If
			Return syntaxToken
		End Function

		Private Function MakeDocCommentLineBreakToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal TokenWidth As Integer) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax
			Dim xmlTextTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax
			Dim text As String = Me.GetText(TokenWidth)
			xmlTextTokenSyntax = If(precedingTrivia.Node IsNot Nothing OrElse EmbeddedOperators.CompareString(text, "" & VbCrLf & "", False) <> 0, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.DocumentationCommentLineBreakToken(text, "" & VbCrLf & "", precedingTrivia.Node, Nothing), Scanner.s_docCommentCrLfToken)
			Return xmlTextTokenSyntax
		End Function

		Friend Function MakeDocumentationCommentExteriorTrivia(ByVal text As String) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Dim syntaxTrivium As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia = Nothing
			Dim triviaKey As Scanner.TriviaKey = New Scanner.TriviaKey(text, SyntaxKind.DocumentationCommentExteriorTrivia)
			If (Not Me._wsTable.TryGetValue(triviaKey, syntaxTrivium)) Then
				syntaxTrivium = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.DocumentationCommentExteriorTrivia(text)
				Me._wsTable.Add(triviaKey, syntaxTrivium)
			End If
			Return syntaxTrivium
		End Function

		Private Function MakeDotToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal charIsFullWidth As Boolean) As PunctuationSyntax
			Dim str As String = If(charIsFullWidth, "．", ".")
			Me.AdvanceChar(1)
			Return Me.MakePunctuationToken(precedingTrivia, str, SyntaxKind.DotToken)
		End Function

		Private Function MakeEmptyToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As PunctuationSyntax
			Return Me.MakePunctuationToken(precedingTrivia, "", SyntaxKind.EmptyToken)
		End Function

		Private Function MakeEndOfInterpolatedStringToken() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Token(Nothing, SyntaxKind.EndOfInterpolatedStringToken, Nothing, [String].Empty)
		End Function

		Friend Function MakeEndOfLineTrivia(ByVal text As String) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Dim syntaxTrivium As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia = Nothing
			Dim triviaKey As Scanner.TriviaKey = New Scanner.TriviaKey(text, SyntaxKind.EndOfLineTrivia)
			If (Not Me._wsTable.TryGetValue(triviaKey, syntaxTrivium)) Then
				syntaxTrivium = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EndOfLineTrivia(text)
				Me._wsTable.Add(triviaKey, syntaxTrivium)
			End If
			Return syntaxTrivium
		End Function

		Friend Function MakeEndOfLineTriviaCRLF() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Me.AdvanceChar(2)
			Return Scanner.s_crLfTrivia
		End Function

		Private Shared Function MakeEofToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Token(precedingTrivia.Node, SyntaxKind.EndOfFileToken, Nothing, [String].Empty)
		End Function

		Private Function MakeEofToken() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Return Me._simpleEof
		End Function

		Private Function MakeEqualsToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal charIsFullWidth As Boolean) As PunctuationSyntax
			Dim str As String = If(charIsFullWidth, "＝", "=")
			Me.AdvanceChar(1)
			Return Me.MakePunctuationToken(precedingTrivia, str, SyntaxKind.EqualsToken)
		End Function

		Private Function MakeExclamationToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal charIsFullWidth As Boolean) As PunctuationSyntax
			Dim str As String = If(charIsFullWidth, "！", "!")
			Me.AdvanceChar(1)
			Return Me.MakePunctuationToken(precedingTrivia, str, SyntaxKind.ExclamationToken)
		End Function

		Private Function MakeFloatingLiteralToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal typeCharacter As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter, ByVal floatingValue As Double, ByVal length As Integer) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim text As String = Me.GetText(length)
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = Me.ScanSingleLineTrivia()
			Dim tokenPart As Scanner.TokenParts = New Scanner.TokenParts(precedingTrivia, syntaxList, text)
			Dim syntaxToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Nothing
			If (Not Me._literalTable.TryGetValue(tokenPart, syntaxToken1)) Then
				syntaxToken1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.FloatingLiteralToken(text, typeCharacter, floatingValue, precedingTrivia.Node, syntaxList.Node)
				Me._literalTable.Add(tokenPart, syntaxToken1)
				syntaxToken = syntaxToken1
			Else
				syntaxToken = syntaxToken1
			End If
			Return syntaxToken
		End Function

		Private Function MakeGreaterThanEqualsToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal length As Integer) As PunctuationSyntax
			Return Me.MakePunctuationToken(precedingTrivia, length, SyntaxKind.GreaterThanEqualsToken)
		End Function

		Private Function MakeGreaterThanGreaterThanEqualsToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal length As Integer) As PunctuationSyntax
			Return Me.MakePunctuationToken(precedingTrivia, length, SyntaxKind.GreaterThanGreaterThanEqualsToken)
		End Function

		Private Function MakeGreaterThanGreaterThanToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal length As Integer) As PunctuationSyntax
			Return Me.MakePunctuationToken(precedingTrivia, length, SyntaxKind.GreaterThanGreaterThanToken)
		End Function

		Private Function MakeGreaterThanToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal charIsFullWidth As Boolean) As PunctuationSyntax
			Dim str As String = If(charIsFullWidth, "＞", ">")
			Me.AdvanceChar(1)
			Return Me.MakePunctuationToken(precedingTrivia, str, SyntaxKind.GreaterThanToken)
		End Function

		Private Function MakeHashToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal charIsFullWidth As Boolean) As PunctuationSyntax
			Dim str As String = If(charIsFullWidth, "＃", "#")
			Me.AdvanceChar(1)
			Return Me.MakePunctuationToken(precedingTrivia, str, SyntaxKind.HashToken)
		End Function

		Private Function MakeIdentifier(ByVal spelling As String, ByVal contextualKind As SyntaxKind, ByVal isBracketed As Boolean, ByVal BaseSpelling As String, ByVal TypeCharacter As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter, ByVal leadingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As IdentifierTokenSyntax
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = Me.ScanSingleLineTrivia()
			Return Me.MakeIdentifier(spelling, contextualKind, isBracketed, BaseSpelling, TypeCharacter, leadingTrivia, syntaxList)
		End Function

		Friend Function MakeIdentifier(ByVal keyword As KeywordSyntax) As IdentifierTokenSyntax
			Return Me.MakeIdentifier(keyword.Text, keyword.Kind, False, keyword.Text, TypeCharacter.None, keyword.GetLeadingTrivia(), keyword.GetTrailingTrivia())
		End Function

		Private Function MakeIdentifier(ByVal spelling As String, ByVal contextualKind As SyntaxKind, ByVal isBracketed As Boolean, ByVal BaseSpelling As String, ByVal TypeCharacter As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter, ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal followingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax
			Dim tokenPart As Scanner.TokenParts = New Scanner.TokenParts(precedingTrivia, followingTrivia, spelling)
			Dim identifierTokenSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = Nothing
			If (Not Me._idTable.TryGetValue(tokenPart, identifierTokenSyntax1)) Then
				identifierTokenSyntax1 = If(contextualKind <> SyntaxKind.IdentifierToken OrElse isBracketed OrElse TypeCharacter <> Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.None, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Identifier(spelling, contextualKind, isBracketed, BaseSpelling, TypeCharacter, precedingTrivia.Node, followingTrivia.Node), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Identifier(spelling, precedingTrivia.Node, followingTrivia.Node))
				Me._idTable.Add(tokenPart, identifierTokenSyntax1)
				identifierTokenSyntax = identifierTokenSyntax1
			Else
				identifierTokenSyntax = identifierTokenSyntax1
			End If
			Return identifierTokenSyntax
		End Function

		Private Function MakeIntegerLiteralToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal base As LiteralBase, ByVal typeCharacter As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter, ByVal integralValue As ULong, ByVal length As Integer) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim text As String = Me.GetText(length)
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = Me.ScanSingleLineTrivia()
			Dim tokenPart As Scanner.TokenParts = New Scanner.TokenParts(precedingTrivia, syntaxList, text)
			Dim syntaxToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Nothing
			If (Not Me._literalTable.TryGetValue(tokenPart, syntaxToken1)) Then
				syntaxToken1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.IntegerLiteralToken(text, base, typeCharacter, integralValue, precedingTrivia.Node, syntaxList.Node)
				Me._literalTable.Add(tokenPart, syntaxToken1)
				syntaxToken = syntaxToken1
			Else
				syntaxToken = syntaxToken1
			End If
			Return syntaxToken
		End Function

		Private Function MakeKeyword(ByVal tokenType As SyntaxKind, ByVal spelling As String, ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As KeywordSyntax
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = Me.ScanSingleLineTrivia()
			Return Me.MakeKeyword(tokenType, spelling, precedingTrivia, syntaxList)
		End Function

		Friend Function MakeKeyword(ByVal identifier As IdentifierTokenSyntax) As KeywordSyntax
			Return Me.MakeKeyword(identifier.PossibleKeywordKind, identifier.Text, identifier.GetLeadingTrivia(), identifier.GetTrailingTrivia())
		End Function

		Friend Function MakeKeyword(ByVal xmlName As XmlNameTokenSyntax) As KeywordSyntax
			Return Me.MakeKeyword(xmlName.PossibleKeywordKind, xmlName.Text, xmlName.GetLeadingTrivia(), xmlName.GetTrailingTrivia())
		End Function

		Private Function MakeKeyword(ByVal tokenType As SyntaxKind, ByVal spelling As String, ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal followingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax
			Dim tokenPart As Scanner.TokenParts = New Scanner.TokenParts(precedingTrivia, followingTrivia, spelling)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			If (Not Me._kwTable.TryGetValue(tokenPart, keywordSyntax1)) Then
				keywordSyntax1 = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax(tokenType, spelling, precedingTrivia.Node, followingTrivia.Node)
				Me._kwTable.Add(tokenPart, keywordSyntax1)
				keywordSyntax = keywordSyntax1
			Else
				keywordSyntax = keywordSyntax1
			End If
			Return keywordSyntax
		End Function

		Private Function MakeLessThanEqualsToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal length As Integer) As PunctuationSyntax
			Return Me.MakePunctuationToken(precedingTrivia, length, SyntaxKind.LessThanEqualsToken)
		End Function

		Private Function MakeLessThanGreaterThanToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal length As Integer) As PunctuationSyntax
			Return Me.MakePunctuationToken(precedingTrivia, length, SyntaxKind.LessThanGreaterThanToken)
		End Function

		Private Function MakeLessThanLessThanEqualsToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal length As Integer) As PunctuationSyntax
			Return Me.MakePunctuationToken(precedingTrivia, length, SyntaxKind.LessThanLessThanEqualsToken)
		End Function

		Private Function MakeLessThanLessThanToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal length As Integer) As PunctuationSyntax
			Return Me.MakePunctuationToken(precedingTrivia, length, SyntaxKind.LessThanLessThanToken)
		End Function

		Private Function MakeLessThanToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal charIsFullWidth As Boolean) As PunctuationSyntax
			Dim str As String = If(charIsFullWidth, "＜", "<")
			Me.AdvanceChar(1)
			Return Me.MakePunctuationToken(precedingTrivia, str, SyntaxKind.LessThanToken)
		End Function

		Friend Function MakeLineContinuationTrivia(ByVal text As String) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Dim syntaxTrivium As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia = Nothing
			Dim triviaKey As Scanner.TriviaKey = New Scanner.TriviaKey(text, SyntaxKind.LineContinuationTrivia)
			If (Not Me._wsTable.TryGetValue(triviaKey, syntaxTrivium)) Then
				syntaxTrivium = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.LineContinuationTrivia(text)
				Me._wsTable.Add(triviaKey, syntaxTrivium)
			End If
			Return syntaxTrivium
		End Function

		Private Function MakeMinusEqualsToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal length As Integer) As PunctuationSyntax
			Return Me.MakePunctuationToken(precedingTrivia, length, SyntaxKind.MinusEqualsToken)
		End Function

		Private Function MakeMinusToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal charIsFullWidth As Boolean) As PunctuationSyntax
			Dim str As String = If(charIsFullWidth, "－", "-")
			Me.AdvanceChar(1)
			Return Me.MakePunctuationToken(precedingTrivia, str, SyntaxKind.MinusToken)
		End Function

		Private Shared Function MakeMissingToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal kind As SyntaxKind) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(kind)
			If (precedingTrivia.Any()) Then
				syntaxToken = DirectCast(syntaxToken.WithLeadingTrivia(precedingTrivia.Node), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			End If
			Return syntaxToken
		End Function

		Private Function MakeOpenBraceToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal charIsFullWidth As Boolean) As PunctuationSyntax
			Dim str As String = If(charIsFullWidth, "｛", "{")
			Me.AdvanceChar(1)
			Return Me.MakePunctuationToken(precedingTrivia, str, SyntaxKind.OpenBraceToken)
		End Function

		Private Function MakeOpenParenToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal charIsFullWidth As Boolean) As PunctuationSyntax
			Dim str As String = If(charIsFullWidth, "（", "(")
			Me.AdvanceChar(1)
			Return Me.MakePunctuationToken(precedingTrivia, str, SyntaxKind.OpenParenToken)
		End Function

		Private Function MakePlusEqualsToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal length As Integer) As PunctuationSyntax
			Return Me.MakePunctuationToken(precedingTrivia, length, SyntaxKind.PlusEqualsToken)
		End Function

		Private Function MakePlusToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal charIsFullWidth As Boolean) As PunctuationSyntax
			Dim str As String = If(charIsFullWidth, "＋", "+")
			Me.AdvanceChar(1)
			Return Me.MakePunctuationToken(precedingTrivia, str, SyntaxKind.PlusToken)
		End Function

		Friend Function MakePunctuationToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal spelling As String, ByVal kind As SyntaxKind) As PunctuationSyntax
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = Me.ScanSingleLineTrivia()
			Return Me.MakePunctuationToken(kind, spelling, precedingTrivia, syntaxList)
		End Function

		Private Function MakePunctuationToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal length As Integer, ByVal kind As SyntaxKind) As PunctuationSyntax
			Dim text As String = Me.GetText(length)
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = Me.ScanSingleLineTrivia()
			Return Me.MakePunctuationToken(kind, text, precedingTrivia, syntaxList)
		End Function

		Friend Function MakePunctuationToken(ByVal kind As SyntaxKind, ByVal spelling As String, ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal followingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax
			Dim tokenPart As Scanner.TokenParts = New Scanner.TokenParts(precedingTrivia, followingTrivia, spelling)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			If (Not Me._punctTable.TryGetValue(tokenPart, punctuationSyntax1)) Then
				punctuationSyntax1 = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax(kind, spelling, precedingTrivia.Node, followingTrivia.Node)
				Me._punctTable.Add(tokenPart, punctuationSyntax1)
				punctuationSyntax = punctuationSyntax1
			Else
				punctuationSyntax = punctuationSyntax1
			End If
			Return punctuationSyntax
		End Function

		Private Function MakeQuestionToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal charIsFullWidth As Boolean) As PunctuationSyntax
			Dim str As String = If(charIsFullWidth, "？", "?")
			Me.AdvanceChar(1)
			Return Me.MakePunctuationToken(precedingTrivia, str, SyntaxKind.QuestionToken)
		End Function

		Private Function MakeSlashEqualsToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal length As Integer) As PunctuationSyntax
			Return Me.MakePunctuationToken(precedingTrivia, length, SyntaxKind.SlashEqualsToken)
		End Function

		Private Function MakeSlashToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal charIsFullWidth As Boolean) As PunctuationSyntax
			Dim str As String = If(charIsFullWidth, "／", "/")
			Me.AdvanceChar(1)
			Return Me.MakePunctuationToken(precedingTrivia, str, SyntaxKind.SlashToken)
		End Function

		Private Function MakeStatementTerminatorToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal width As Integer) As PunctuationSyntax
			Me.AdvanceChar(width)
			Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.StatementTerminatorToken
		End Function

		Friend Function MakeTriviaArray(ByVal builder As SyntaxListBuilder) As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			If (builder.Count <> 0) Then
				syntaxList = If(Not Scanner.CanCache(builder), builder.ToList(), Me._wslTable.GetOrMakeValue(builder))
			Else
				syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
			End If
			Return syntaxList
		End Function

		Friend Function MakeWhiteSpaceTrivia(ByVal text As String) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Dim syntaxTrivium As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia = Nothing
			Dim triviaKey As Scanner.TriviaKey = New Scanner.TriviaKey(text, SyntaxKind.WhitespaceTrivia)
			If (Not Me._wsTable.TryGetValue(triviaKey, syntaxTrivium)) Then
				syntaxTrivium = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.WhitespaceTrivia(text)
				Me._wsTable.Add(triviaKey, syntaxTrivium)
			End If
			Return syntaxTrivium
		End Function

		Friend Overridable Sub MoveToNextSyntaxNode()
			Me._prevToken = New Scanner.ScannerToken()
			Me._scannerPreprocessorState = Me._currentToken.PreprocessorState
			Me.ResetTokens()
		End Sub

		Friend Overridable Sub MoveToNextSyntaxNodeInTrivia()
			Me._prevToken = New Scanner.ScannerToken()
		End Sub

		Private Function NextAre(ByVal chars As String) As Boolean
			Return Me.NextAre(0, chars)
		End Function

		Private Function NextAre(ByVal offset As Integer, ByVal chars As String) As Boolean
			Dim flag As Boolean
			Dim length As Integer = chars.Length
			If (Me.CanGet(offset + length - 1)) Then
				Dim num As Integer = length - 1
				Dim num1 As Integer = 0
				While num1 <= num
					If (chars(num1) = Me.Peek(offset + num1)) Then
						num1 = num1 + 1
					Else
						flag = False
						Return flag
					End If
				End While
				flag = True
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function NextIs(ByVal offset As Integer, ByVal c As Char) As Boolean
			If (Not Me.CanGet(offset)) Then
				Return False
			End If
			Return Me.Peek(offset) = c
		End Function

		Private Function Peek(ByVal skip As Integer) As Char
			Dim num As Integer = Me._lineBufferOffset
			Dim page As Scanner.Page = Me._curPage
			num += skip
			Dim chr As Char = page._arr(num And 2047)
			If (page._pageStart <> (num And -2048)) Then
				chr = Me.GetPage(num)._arr(num And 2047)
			End If
			Return chr
		End Function

		Friend Function Peek() As Char
			Dim page As Scanner.Page = Me._curPage
			Dim num As Integer = Me._lineBufferOffset
			Dim chr As Char = page._arr(num And 2047)
			If (page._pageStart <> (num And -2048)) Then
				chr = Me.GetPage(num)._arr(num And 2047)
			End If
			Return chr
		End Function

		Friend Function PeekNextToken(ByVal state As ScannerState) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim innerTokenObject As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			If (Me._tokens.Count > 0) Then
				Dim item As Scanner.ScannerToken = Me._tokens(0)
				If (item.State = state) Then
					innerTokenObject = item.InnerTokenObject
					Return innerTokenObject
				End If
				Me.AbandonPeekedTokens()
			End If
			Me.GetCurrentToken()
			innerTokenObject = Me.GetTokenAndAddToQueue(state)
			Return innerTokenObject
		End Function

		Private Function PeekStartComment(ByVal i As Integer) As Integer
			Dim num As Integer
			If (Me.CanGet(i)) Then
				Dim chr As Char = Me.Peek(i)
				If (SyntaxFacts.IsSingleQuote(chr)) Then
					num = 1
					Return num
				ElseIf (SyntaxFacts.MatchOneOrAnotherOrFullwidth(chr, "R"C, "r"C) AndAlso Me.CanGet(i + 2) AndAlso SyntaxFacts.MatchOneOrAnotherOrFullwidth(Me.Peek(i + 1), "E"C, "e"C) AndAlso SyntaxFacts.MatchOneOrAnotherOrFullwidth(Me.Peek(i + 2), "M"C, "m"C)) Then
					If (Not Me.CanGet(i + 3) OrElse SyntaxFacts.IsNewLine(Me.Peek(i + 3))) Then
						num = 3
						Return num
					Else
						If (SyntaxFacts.IsIdentifierPartCharacter(Me.Peek(i + 3))) Then
							num = 0
							Return num
						End If
						num = 4
						Return num
					End If
				End If
			End If
			num = 0
			Return num
		End Function

		Friend Function PeekToken(ByVal tokenOffset As Integer, ByVal state As ScannerState) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			If (tokenOffset = 0) Then
				currentToken = Me.GetCurrentToken()
			ElseIf (tokenOffset <> 1) Then
				Dim num As Integer = tokenOffset - 1
				If (num = Me._tokens.Count) Then
					currentToken = Me.GetTokenAndAddToQueue(state)
				ElseIf (num >= Me._tokens.Count OrElse Me._tokens(num).State <> state) Then
					Me.RevertState(Me._tokens(num))
					Me._tokens.RemoveRange(num, Me._tokens.Count - num)
					currentToken = Me.GetTokenAndAddToQueue(state)
				Else
					currentToken = Me._tokens(num).InnerTokenObject
				End If
			Else
				currentToken = Me.PeekNextToken(state)
			End If
			Return currentToken
		End Function

		Private Sub ProcessDirective(ByVal directiveTrivia As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax, ByVal tList As SyntaxListBuilder)
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
			Dim directiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax = directiveTrivia
			Dim preprocessorState As Scanner.PreprocessorState = Scanner.ApplyDirective(Me._scannerPreprocessorState, directiveTriviaSyntax)
			Me._scannerPreprocessorState = preprocessorState
			Dim conditionalStack As ImmutableStack(Of Scanner.ConditionalState) = preprocessorState.ConditionalStack
			If (conditionalStack.Count() <> 0 AndAlso conditionalStack.Peek().BranchTaken <> Scanner.ConditionalState.BranchTakenState.Taken) Then
				syntaxList = Me.SkipConditionalCompilationSection()
			End If
			If (directiveTriviaSyntax <> directiveTrivia) Then
				directiveTrivia = directiveTriviaSyntax
			End If
			tList.Add(directiveTrivia)
			If (syntaxList.Node IsNot Nothing) Then
				tList.AddRange(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(syntaxList)
			End If
		End Sub

		Public Function QuickScanToken(ByVal allowLeadingMultilineTrivia As Boolean) As Scanner.QuickScanResult
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Scanner/QuickScanResult Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Scanner::QuickScanToken(System.Boolean)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Scanner/QuickScanResult QuickScanToken(System.Boolean)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Friend Function RecoverFromMissingConditionalEnds(ByVal eof As PunctuationSyntax, <Out> ByRef notClosedIfDirectives As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax), <Out> ByRef notClosedRegionDirectives As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax), <Out> ByRef haveRegionDirectives As Boolean, <Out> ByRef notClosedExternalSourceDirective As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax) As PunctuationSyntax
			notClosedIfDirectives = Nothing
			notClosedRegionDirectives = Nothing
			If (Me._scannerPreprocessorState.ConditionalStack.Count() > 0) Then
				Dim enumerator As ImmutableStack(Of Scanner.ConditionalState).Enumerator = Me._scannerPreprocessorState.ConditionalStack.GetEnumerator()
				While enumerator.MoveNext()
					Dim ifDirective As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax = enumerator.Current.IfDirective
					If (ifDirective Is Nothing) Then
						Continue While
					End If
					If (notClosedIfDirectives Is Nothing) Then
						notClosedIfDirectives = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax).GetInstance()
					End If
					notClosedIfDirectives.Add(ifDirective)
				End While
				If (notClosedIfDirectives Is Nothing) Then
					eof = Parser.ReportSyntaxError(Of PunctuationSyntax)(eof, ERRID.ERR_LbExpectedEndIf)
				End If
			End If
			If (Me._scannerPreprocessorState.RegionDirectiveStack.Count() > 0) Then
				notClosedRegionDirectives = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax).GetInstance()
				notClosedRegionDirectives.AddRange(Me._scannerPreprocessorState.RegionDirectiveStack)
			End If
			haveRegionDirectives = Me._scannerPreprocessorState.HaveSeenRegionDirectives
			notClosedExternalSourceDirective = Me._scannerPreprocessorState.ExternalSourceDirective
			Return eof
		End Function

		Private Function RemainingLength() As Integer
			Return Me._bufferLen - Me._lineBufferOffset
		End Function

		Friend Sub RescanTrailingColonAsToken(ByRef prevToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByRef currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			Dim innerTokenObject As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me._prevToken.InnerTokenObject
			Me.AbandonAllTokens()
			Me.RevertState(Me._prevToken)
			Dim scannerState As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState.VB
			innerTokenObject = DirectCast(innerTokenObject.WithTrailingTrivia(Nothing), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			Dim fullWidth As Integer = innerTokenObject.FullWidth
			Me._lineBufferOffset += fullWidth
			Me._endOfTerminatorTrivia = Me._lineBufferOffset
			Me._prevToken = Me._prevToken.[With](scannerState, innerTokenObject)
			prevToken = innerTokenObject
			Me._currentToken = New Scanner.ScannerToken(Me._scannerPreprocessorState, Me._lineBufferOffset, Me._endOfTerminatorTrivia, Nothing, scannerState)
			Dim syntaxListBuilder As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxListBuilder = Me._triviaListPool.Allocate()
			Me.ScanSingleLineTrivia(syntaxListBuilder)
			Dim item As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia = DirectCast(syntaxListBuilder(syntaxListBuilder.Count - 1), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia)
			syntaxListBuilder.RemoveLast()
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = Me.MakeTriviaArray(syntaxListBuilder)
			Me._triviaListPool.Free(syntaxListBuilder)
			Me._lineBufferOffset = Me._endOfTerminatorTrivia
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = Me.ScanSingleLineTrivia()
			innerTokenObject = Me.MakePunctuationToken(SyntaxKind.ColonToken, item.Text, syntaxList, syntaxList1)
			Me._currentToken = Me._currentToken.[With](scannerState, innerTokenObject)
			currentToken = innerTokenObject
		End Sub

		Friend Sub ResetCurrentToken(ByVal state As ScannerState)
			If (state <> Me._currentToken.State) Then
				If (Me._currentToken.State = ScannerState.VB AndAlso state = ScannerState.Content) Then
					Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.GetCurrentToken()
					Me.AbandonAllTokens()
					Dim position As Integer = Me._currentToken.Position + currentToken.GetLeadingTriviaWidth()
					Me._lineBufferOffset = position
					Dim scannerToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.GetScannerToken(state)
					scannerToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.AddLeadingTrivia(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(scannerToken, currentToken.GetLeadingTrivia())
					Me._currentToken = Me._currentToken.[With](state, scannerToken)
					Return
				End If
				Me.AbandonAllTokens()
				Me._currentToken = Me._currentToken.[With](state, Nothing)
			End If
		End Sub

		Private Sub ResetLineBufferOffset()
			Me._lineBufferOffset = Me._currentToken.Position
			Me._endOfTerminatorTrivia = Me._lineBufferOffset
		End Sub

		Private Sub ResetTokens()
			Me._tokens.Clear()
			Me._currentToken = New Scanner.ScannerToken(Me._scannerPreprocessorState, Me._lineBufferOffset, Me._endOfTerminatorTrivia, Nothing, ScannerState.VB)
		End Sub

		Private Sub RestoreTokens(ByVal tokens As Scanner.ScannerToken())
			Me._tokens.Clear()
			If (tokens IsNot Nothing) Then
				Me._tokens.AddRange(tokens)
			End If
		End Sub

		Private Sub RevertState(ByVal revertTo As Scanner.ScannerToken)
			Me._lineBufferOffset = revertTo.Position
			Me._endOfTerminatorTrivia = revertTo.EndOfTerminatorTrivia
			Me._scannerPreprocessorState = revertTo.PreprocessorState
		End Sub

		Private Function SaveAndClearTokens() As Scanner.ScannerToken()
			Dim scannerTokenArray As Scanner.ScannerToken()
			If (Me._tokens.Count <> 0) Then
				Dim array As Scanner.ScannerToken() = Me._tokens.ToArray()
				Me._tokens.Clear()
				scannerTokenArray = array
			Else
				scannerTokenArray = Nothing
			End If
			Return scannerTokenArray
		End Function

		Private Function ScanBracketedIdentifier(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim num As Integer = 1
			Dim num1 As Integer = num
			Dim flag As Boolean = False
			If (Me.CanGet(num1)) Then
				Dim chr As Char = Me.Peek(num1)
				If (Not Me.IsIdentifierStartCharacter(chr) OrElse SyntaxFacts.IsConnectorPunctuation(chr) AndAlso (Not Me.CanGet(num1 + 1) OrElse Not SyntaxFacts.IsIdentifierPartCharacter(Me.Peek(num1 + 1)))) Then
					flag = True
				End If
				While Me.CanGet(num1)
					Dim chr1 As Char = Me.Peek(num1)
					If (chr1 = "]"C OrElse chr1 = "］"C) Then
						Dim num2 As Integer = num1 - num
						If (num2 <= 0 OrElse flag) Then
							syntaxToken = Me.MakeBadToken(precedingTrivia, num1 + 1, ERRID.ERR_ExpectedIdentifier)
							Return syntaxToken
						Else
							Dim text As String = Me.GetText(num2 + 2)
							Dim str As String = text.Substring(1, num2)
							syntaxToken = Me.MakeIdentifier(text, SyntaxKind.IdentifierToken, True, str, TypeCharacter.None, precedingTrivia)
							Return syntaxToken
						End If
					Else
						If (SyntaxFacts.IsNewLine(chr1)) Then
							Exit While
						End If
						If (SyntaxFacts.IsIdentifierPartCharacter(chr1)) Then
							num1 = num1 + 1
						Else
							flag = True
							Exit While
						End If
					End If
				End While
				syntaxToken = If(num1 <= 1, Me.MakeBadToken(precedingTrivia, num1, ERRID.ERR_ExpectedIdentifier), Me.MakeBadToken(precedingTrivia, num1, ERRID.ERR_MissingEndBrack))
			Else
				syntaxToken = Me.MakeBadToken(precedingTrivia, num1, ERRID.ERR_MissingEndBrack)
			End If
			Return syntaxToken
		End Function

		Private Function ScanColonAsStatementTerminator(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal charIsFullWidth As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			syntaxToken = If(Me._lineBufferOffset >= Me._endOfTerminatorTrivia, Me.MakeEmptyToken(precedingTrivia), Me.MakeColonToken(precedingTrivia, charIsFullWidth))
			Return syntaxToken
		End Function

		Private Function ScanColonAsTrivia() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Return Me.MakeColonTrivia(Me.GetText(1))
		End Function

		Private Function ScanComment() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Dim syntaxTrivium As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Dim num As Integer = Me.PeekStartComment(0)
			If (num <= 0) Then
				syntaxTrivium = Nothing
			Else
				Dim flag As Boolean = Me.StartsXmlDoc(0)
				While Me.CanGet(num) AndAlso Not SyntaxFacts.IsNewLine(Me.Peek(num))
					num = num + 1
				End While
				Dim syntaxTrivium1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia = Scanner.MakeCommentTrivia(Me.GetTextNotInterned(num))
				If (flag AndAlso Me._options.DocumentationMode >= DocumentationMode.Diagnose) Then
					syntaxTrivium1 = syntaxTrivium1.WithDiagnostics(New DiagnosticInfo() { ErrorFactory.ErrorInfo(ERRID.WRN_XMLDocNotFirstOnLine) })
				End If
				syntaxTrivium = syntaxTrivium1
			End If
			Return syntaxTrivium
		End Function

		Private Function ScanCommentIfAny(ByVal tList As SyntaxListBuilder) As Boolean
			Dim flag As Boolean
			If (Me.CanGet()) Then
				Dim syntaxTrivium As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia = Me.ScanComment()
				If (syntaxTrivium Is Nothing) Then
					flag = False
					Return flag
				End If
				tList.Add(syntaxTrivium)
				flag = True
				Return flag
			End If
			flag = False
			Return flag
		End Function

		Private Sub ScanConflictMarker(ByVal tList As SyntaxListBuilder)
			Dim chr As Char = Me.Peek()
			Me.ScanConflictMarkerHeader(tList)
			Me.ScanConflictMarkerEndOfLine(tList)
			If (chr = "="C) Then
				Me.ScanConflictMarkerDisabledText(tList)
			End If
		End Sub

		Private Sub ScanConflictMarkerDisabledText(ByVal tList As SyntaxListBuilder)
			Dim num As Integer = Me._lineBufferOffset
			While Me.CanGet() AndAlso (Me.Peek() <> ">"C OrElse Not Me.IsConflictMarkerTrivia())
				Me.AdvanceChar(1)
			End While
			Dim num1 As Integer = Me._lineBufferOffset - num
			If (num1 > 0) Then
				tList.Add(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.DisabledTextTrivia(Me.GetText(num, num1)))
			End If
		End Sub

		Private Sub ScanConflictMarkerEndOfLine(ByVal tList As SyntaxListBuilder)
			Dim num As Integer = Me._lineBufferOffset
			While Me.CanGet() AndAlso SyntaxFacts.IsNewLine(Me.Peek())
				Me.AdvanceChar(1)
			End While
			Dim num1 As Integer = Me._lineBufferOffset - num
			If (num1 > 0) Then
				tList.Add(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EndOfLineTrivia(Me.GetText(num, num1)))
			End If
		End Sub

		Private Sub ScanConflictMarkerHeader(ByVal tList As SyntaxListBuilder)
			Dim num As Integer = Me._lineBufferOffset
			While Me.CanGet() AndAlso Not SyntaxFacts.IsNewLine(Me.Peek())
				Me.AdvanceChar(1)
			End While
			Dim syntaxTrivium As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.ConflictMarkerTrivia(Me.GetText(num, Me._lineBufferOffset - num))
			tList.Add(DirectCast(syntaxTrivium.SetDiagnostics(New DiagnosticInfo() { ErrorFactory.ErrorInfo(ERRID.ERR_Merge_conflict_marker_encountered) }), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia))
		End Sub

		Private Function ScanDateLiteral(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim num As Integer = 0
			Dim num1 As Integer = 0
			Dim num2 As Integer = 0
			Dim num3 As Integer = 0
			Dim num4 As Integer = 0
			Dim num5 As Integer = 0
			Dim num6 As Integer = 0
			Dim whitespaceLength As Integer = 1
			Dim flag As Boolean = False
			Dim flag1 As Boolean = False
			Dim flag2 As Boolean = False
			Dim flag3 As Boolean = False
			Dim flag4 As Boolean = False
			Dim flag5 As Boolean = False
			Dim flag6 As Boolean = False
			Dim flag7 As Boolean = False
			Dim flag8 As Boolean = False
			Dim daysToMonth365 As Integer() = Nothing
			Dim flag9 As Boolean = False
			whitespaceLength = Me.GetWhitespaceLength(whitespaceLength)
			Dim num7 As Integer = whitespaceLength
			If (Me.ScanIntLiteral(num, whitespaceLength)) Then
				If (Me.CanGet(whitespaceLength) AndAlso SyntaxFacts.IsDateSeparatorCharacter(Me.Peek(whitespaceLength))) Then
					Dim num8 As Integer = whitespaceLength
					flag = True
					whitespaceLength = whitespaceLength + 1
					If (whitespaceLength - num7 <> 5) Then
						num2 = num
						If (Not Me.ScanIntLiteral(num3, whitespaceLength)) Then
							GoTo Label1
						ElseIf (Me.CanGet(whitespaceLength) AndAlso SyntaxFacts.IsDateSeparatorCharacter(Me.Peek(whitespaceLength))) Then
							If (Me.Peek(whitespaceLength) = Me.Peek(num8)) Then
								flag1 = True
								whitespaceLength = whitespaceLength + 1
								Dim num9 As Integer = whitespaceLength
								If (Not Me.ScanIntLiteral(num1, whitespaceLength)) Then
									GoTo Label1
								End If
								If (whitespaceLength - num9 = 2) Then
									flag8 = True
								End If
							Else
								GoTo Label1
							End If
						End If
					Else
						flag1 = True
						flag9 = True
						num1 = num
						If (Not Me.ScanIntLiteral(num2, whitespaceLength)) Then
							GoTo Label1
						ElseIf (Me.CanGet(whitespaceLength) AndAlso SyntaxFacts.IsDateSeparatorCharacter(Me.Peek(whitespaceLength))) Then
							If (Me.Peek(whitespaceLength) = Me.Peek(num8)) Then
								whitespaceLength = whitespaceLength + 1
								If (Me.ScanIntLiteral(num3, whitespaceLength)) Then
									GoTo Label2
								End If
								GoTo Label1
							Else
								GoTo Label1
							End If
						End If
					End If
				Label2:
					whitespaceLength = Me.GetWhitespaceLength(whitespaceLength)
					GoTo Label0
				Else
					GoTo Label0
				End If
			Label1:
				While Me.CanGet(whitespaceLength)
					Dim chr As Char = Me.Peek(whitespaceLength)
					If (SyntaxFacts.IsHash(chr) OrElse SyntaxFacts.IsNewLine(chr)) Then
						Exit While
					End If
					whitespaceLength = whitespaceLength + 1
				End While
				If (Not Me.CanGet(whitespaceLength) OrElse SyntaxFacts.IsNewLine(Me.Peek(whitespaceLength))) Then
					syntaxToken = Nothing
				Else
					whitespaceLength = whitespaceLength + 1
					syntaxToken = Me.MakeBadToken(precedingTrivia, whitespaceLength, ERRID.ERR_InvalidDate)
				End If
			Else
				syntaxToken = Nothing
			End If
			Return syntaxToken
			If (Not flag) Then
				flag2 = True
				num4 = num
			ElseIf (Me.ScanIntLiteral(num4, whitespaceLength)) Then
				flag2 = True
			End If
			If (flag2) Then
				If (Me.CanGet(whitespaceLength) AndAlso SyntaxFacts.IsColon(Me.Peek(whitespaceLength))) Then
					whitespaceLength = whitespaceLength + 1
					If (Me.ScanIntLiteral(num5, whitespaceLength)) Then
						flag3 = True
						If (Me.CanGet(whitespaceLength) AndAlso SyntaxFacts.IsColon(Me.Peek(whitespaceLength))) Then
							flag4 = True
							whitespaceLength = whitespaceLength + 1
							If (Not Me.ScanIntLiteral(num6, whitespaceLength)) Then
								GoTo Label1
							End If
						End If
					Else
						GoTo Label1
					End If
				End If
				whitespaceLength = Me.GetWhitespaceLength(whitespaceLength)
				If (Me.CanGet(whitespaceLength)) Then
					If (Me.Peek(whitespaceLength) = "A"C OrElse Me.Peek(whitespaceLength) = "Ａ"C OrElse Me.Peek(whitespaceLength) = "a"C OrElse Me.Peek(whitespaceLength) = "ａ"C) Then
						flag5 = True
						whitespaceLength = whitespaceLength + 1
					ElseIf (Me.Peek(whitespaceLength) = "P"C OrElse Me.Peek(whitespaceLength) = "Ｐ"C OrElse Me.Peek(whitespaceLength) = "p"C OrElse Me.Peek(whitespaceLength) = "ｐ"C) Then
						flag6 = True
						whitespaceLength = whitespaceLength + 1
					End If
					If (Me.CanGet(whitespaceLength) AndAlso (flag5 OrElse flag6)) Then
						If (Me.Peek(whitespaceLength) <> "M"C AndAlso Me.Peek(whitespaceLength) <> "Ｍ"C AndAlso Me.Peek(whitespaceLength) <> "m"C AndAlso Me.Peek(whitespaceLength) <> "ｍ"C) Then
							GoTo Label1
						End If
						whitespaceLength = Me.GetWhitespaceLength(whitespaceLength + 1)
					End If
				End If
				If (Not flag3 AndAlso Not flag5 AndAlso Not flag6) Then
					GoTo Label1
				End If
			End If
			If (Not Me.CanGet(whitespaceLength) OrElse Not SyntaxFacts.IsHash(Me.Peek(whitespaceLength))) Then
				GoTo Label1
			End If
			whitespaceLength = whitespaceLength + 1
			If (Not flag) Then
				num2 = 1
				num3 = 1
				num1 = 1
				daysToMonth365 = SyntaxFacts.DaysToMonth365
			Else
				If (num2 < 1 OrElse num2 > 12) Then
					flag7 = True
				End If
				If (Not flag1) Then
					flag7 = True
					num1 = 1
				End If
				daysToMonth365 = If(num1 Mod 4 <> 0 OrElse num1 Mod 100 = 0 AndAlso num1 Mod 400 <> 0, SyntaxFacts.DaysToMonth365, SyntaxFacts.DaysToMonth366)
				If (num3 < 1 OrElse Not flag7 AndAlso num3 > daysToMonth365(num2) - daysToMonth365(num2 - 1)) Then
					flag7 = True
				End If
				If (flag8) Then
					flag7 = True
				End If
				If (num1 < 1 OrElse num1 > 9999) Then
					flag7 = True
				End If
			End If
			If (Not flag2) Then
				num4 = 0
				num5 = 0
				num6 = 0
			Else
				If (flag5 OrElse flag6) Then
					If (num4 < 1 OrElse num4 > 12) Then
						flag7 = True
					End If
					If (flag5) Then
						num4 = num4 Mod 12
					ElseIf (flag6) Then
						num4 += 12
						If (num4 = 24) Then
							num4 = 12
						End If
					End If
				ElseIf (num4 < 0 OrElse num4 > 23) Then
					flag7 = True
				End If
				If (Not flag3) Then
					num5 = 0
				ElseIf (num5 < 0 OrElse num5 > 59) Then
					flag7 = True
				End If
				If (Not flag4) Then
					num6 = 0
				ElseIf (num6 < 0 OrElse num6 > 59) Then
					flag7 = True
				End If
			End If
			If (flag7) Then
				syntaxToken = Me.MakeBadToken(precedingTrivia, whitespaceLength, ERRID.ERR_InvalidDate)
				Return syntaxToken
			Else
				Dim dateTime As System.DateTime = New System.DateTime(num1, num2, num3, num4, num5, num6)
				Dim syntaxToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.MakeDateLiteralToken(precedingTrivia, dateTime, whitespaceLength)
				If (flag9) Then
					syntaxToken1 = Parser.CheckFeatureAvailability(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(Feature.YearFirstDateLiterals, syntaxToken1, Me.Options.LanguageVersion)
				End If
				syntaxToken = syntaxToken1
				Return syntaxToken
			End If
		End Function

		Private Function ScanIdentifierOrKeyword(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim chr As Char = Me.Peek()
			If (Me.CanGet(1)) Then
				Dim chr1 As Char = Me.Peek(1)
				If (Not SyntaxFacts.IsConnectorPunctuation(chr) OrElse SyntaxFacts.IsIdentifierPartCharacter(chr1)) Then
					GoTo Label1
				End If
				syntaxToken = Me.MakeBadToken(precedingTrivia, 1, ERRID.ERR_ExpectedIdentifier)
				Return syntaxToken
			End If
		Label1:
			Dim num As Integer = 1
			While Me.CanGet(num)
				chr = Me.Peek(num)
				Dim num1 As UShort = Convert.ToUInt16(chr)
				If ((num1 >= 128 OrElse Not SyntaxFacts.IsNarrowIdentifierCharacter(num1)) AndAlso Not SyntaxFacts.IsWideIdentifierCharacter(chr)) Then
					Exit While
				End If
				num = num + 1
			End While
			Dim typeCharacter As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.None
			If (Me.CanGet(num)) Then
				chr = Me.Peek(num)
				While True
					Select Case chr
						Case "!"C
							GoTo Label2
						Case """"C
						Label8:
							If (Not SyntaxFacts.IsFullWidth(chr)) Then
								GoTo Label7
							End If
							chr = SyntaxFacts.MakeHalfWidth(chr)
							Continue While
						Case "#"C
							GoTo Label3
						Case "$"C
							GoTo Label4
						Case "%"C
							GoTo Label5
						Case "&"C
							GoTo Label6
						Case Else
							If (chr = "@"C) Then
								GoTo Label9
							End If
							GoTo Label8
					End Select
				End While
			Label9:
				typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.[Decimal]
				num = num + 1
			End If
		Label7:
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierToken
			Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierToken
			Dim text As String = Me.GetText(num)
			Dim str As String = If(typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.None, text, Me.Intern(text, 0, num - 1))
			If (typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.None) Then
				syntaxKind = Me.TokenOfStringCached(text)
				If (SyntaxFacts.IsContextualKeyword(syntaxKind)) Then
					syntaxKind1 = syntaxKind
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierToken
				End If
			ElseIf (Me.TokenOfStringCached(str) = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidKeyword) Then
				syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidKeyword
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierToken
			End If
			If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierToken) Then
				syntaxToken = Me.MakeIdentifier(text, syntaxKind1, False, str, typeCharacter, precedingTrivia)
			Else
				syntaxToken = Me.MakeKeyword(syntaxKind, text, precedingTrivia)
			End If
			Return syntaxToken
		Label2:
			If (Me.CanGet(num + 1)) Then
				Dim chr2 As Char = Me.Peek(num + 1)
				If (Me.IsIdentifierStartCharacter(chr2) OrElse SyntaxFacts.MatchOneOrAnotherOrFullwidth(chr2, "["C, "]"C)) Then
					GoTo Label7
				End If
			End If
			typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.[Single]
			num = num + 1
			GoTo Label7
		Label3:
			typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.[Double]
			num = num + 1
			GoTo Label7
		Label4:
			typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.[String]
			num = num + 1
			GoTo Label7
		Label5:
			typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.[Integer]
			num = num + 1
			GoTo Label7
		Label6:
			typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.[Long]
			num = num + 1
			GoTo Label7
		End Function

		Private Function ScanInterpolatedStringContent() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			syntaxToken = If(Not Me.IsInterpolatedStringPunctuation(0), Me.ScanInterpolatedStringText(False), Me.ScanInterpolatedStringPunctuation())
			Return syntaxToken
		End Function

		Private Function ScanInterpolatedStringFormatString() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			syntaxToken = If(Not Me.IsInterpolatedStringPunctuation(0), Me.ScanInterpolatedStringText(True), Me.ScanInterpolatedStringPunctuation())
			Return syntaxToken
		End Function

		Private Function ScanInterpolatedStringPunctuation() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim num As Integer
			Dim flag As Boolean
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim text As String
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			Dim syntaxList2 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			If (Me.CanGet()) Then
				Dim whitespaceLength As Integer = Me.GetWhitespaceLength(0)
				Dim num1 As Integer = whitespaceLength
				If (Me.CanGet(num1)) Then
					Dim chr As Char = Me.Peek(num1)
					If (chr <= "}"C) Then
						If (chr > ","C) Then
							If (chr = ":"C) Then
								syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ColonToken
								num = 1
								flag = False
								visualBasicSyntaxNode = Me.ScanWhitespace(whitespaceLength)
								text = Me.GetText(num)
								If (flag) Then
									syntaxList2 = Me.ScanSingleLineTrivia()
								Else
									syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
									syntaxList2 = syntaxList1
								End If
								syntaxList = syntaxList2
								syntaxToken = Me.MakePunctuationToken(syntaxKind, text, visualBasicSyntaxNode, syntaxList.Node)
								Return syntaxToken
							End If
							If (chr = "{"C) Then
								syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken
								num = 1
								flag = True
								visualBasicSyntaxNode = Me.ScanWhitespace(whitespaceLength)
								text = Me.GetText(num)
								If (flag) Then
									syntaxList2 = Me.ScanSingleLineTrivia()
								Else
									syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
									syntaxList2 = syntaxList1
								End If
								syntaxList = syntaxList2
								syntaxToken = Me.MakePunctuationToken(syntaxKind, text, visualBasicSyntaxNode, syntaxList.Node)
								Return syntaxToken
							End If
							If (chr = "}"C) Then
								syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseBraceToken
								num = 1
								flag = False
								visualBasicSyntaxNode = Me.ScanWhitespace(whitespaceLength)
								text = Me.GetText(num)
								If (flag) Then
									syntaxList2 = Me.ScanSingleLineTrivia()
								Else
									syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
									syntaxList2 = syntaxList1
								End If
								syntaxList = syntaxList2
								syntaxToken = Me.MakePunctuationToken(syntaxKind, text, visualBasicSyntaxNode, syntaxList.Node)
								Return syntaxToken
							End If
						Else
							If (chr = "$"C) Then
								If (Not Me.CanGet(num1 + 1) OrElse Not SyntaxFacts.IsDoubleQuote(Me.Peek(num1 + 1))) Then
									Throw ExceptionUtilities.Unreachable
								End If
								syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DollarSignDoubleQuoteToken
								num = 2
								flag = False
								visualBasicSyntaxNode = Me.ScanWhitespace(whitespaceLength)
								text = Me.GetText(num)
								If (flag) Then
									syntaxList2 = Me.ScanSingleLineTrivia()
								Else
									syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
									syntaxList2 = syntaxList1
								End If
								syntaxList = syntaxList2
								syntaxToken = Me.MakePunctuationToken(syntaxKind, text, visualBasicSyntaxNode, syntaxList.Node)
								Return syntaxToken
							End If
							If (chr = ","C) Then
								syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken
								num = 1
								flag = True
								visualBasicSyntaxNode = Me.ScanWhitespace(whitespaceLength)
								text = Me.GetText(num)
								If (flag) Then
									syntaxList2 = Me.ScanSingleLineTrivia()
								Else
									syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
									syntaxList2 = syntaxList1
								End If
								syntaxList = syntaxList2
								syntaxToken = Me.MakePunctuationToken(syntaxKind, text, visualBasicSyntaxNode, syntaxList.Node)
								Return syntaxToken
							End If
						End If
					ElseIf (chr > "，"C) Then
						If (chr = "："C) Then
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ColonToken
							num = 1
							flag = False
							visualBasicSyntaxNode = Me.ScanWhitespace(whitespaceLength)
							text = Me.GetText(num)
							If (flag) Then
								syntaxList2 = Me.ScanSingleLineTrivia()
							Else
								syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
								syntaxList2 = syntaxList1
							End If
							syntaxList = syntaxList2
							syntaxToken = Me.MakePunctuationToken(syntaxKind, text, visualBasicSyntaxNode, syntaxList.Node)
							Return syntaxToken
						End If
						If (chr = "｛"C) Then
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken
							num = 1
							flag = True
							visualBasicSyntaxNode = Me.ScanWhitespace(whitespaceLength)
							text = Me.GetText(num)
							If (flag) Then
								syntaxList2 = Me.ScanSingleLineTrivia()
							Else
								syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
								syntaxList2 = syntaxList1
							End If
							syntaxList = syntaxList2
							syntaxToken = Me.MakePunctuationToken(syntaxKind, text, visualBasicSyntaxNode, syntaxList.Node)
							Return syntaxToken
						End If
						If (chr = "｝"C) Then
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseBraceToken
							num = 1
							flag = False
							visualBasicSyntaxNode = Me.ScanWhitespace(whitespaceLength)
							text = Me.GetText(num)
							If (flag) Then
								syntaxList2 = Me.ScanSingleLineTrivia()
							Else
								syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
								syntaxList2 = syntaxList1
							End If
							syntaxList = syntaxList2
							syntaxToken = Me.MakePunctuationToken(syntaxKind, text, visualBasicSyntaxNode, syntaxList.Node)
							Return syntaxToken
						End If
					Else
						If (chr = "＄"C) Then
							If (Not Me.CanGet(num1 + 1) OrElse Not SyntaxFacts.IsDoubleQuote(Me.Peek(num1 + 1))) Then
								Throw ExceptionUtilities.Unreachable
							End If
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DollarSignDoubleQuoteToken
							num = 2
							flag = False
							visualBasicSyntaxNode = Me.ScanWhitespace(whitespaceLength)
							text = Me.GetText(num)
							If (flag) Then
								syntaxList2 = Me.ScanSingleLineTrivia()
							Else
								syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
								syntaxList2 = syntaxList1
							End If
							syntaxList = syntaxList2
							syntaxToken = Me.MakePunctuationToken(syntaxKind, text, visualBasicSyntaxNode, syntaxList.Node)
							Return syntaxToken
						End If
						If (chr = "，"C) Then
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken
							num = 1
							flag = True
							visualBasicSyntaxNode = Me.ScanWhitespace(whitespaceLength)
							text = Me.GetText(num)
							If (flag) Then
								syntaxList2 = Me.ScanSingleLineTrivia()
							Else
								syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
								syntaxList2 = syntaxList1
							End If
							syntaxList = syntaxList2
							syntaxToken = Me.MakePunctuationToken(syntaxKind, text, visualBasicSyntaxNode, syntaxList.Node)
							Return syntaxToken
						End If
					End If
					If (SyntaxFacts.IsDoubleQuote(chr)) Then
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoubleQuoteToken
						num = 1
						flag = True
						visualBasicSyntaxNode = Me.ScanWhitespace(whitespaceLength)
						text = Me.GetText(num)
						If (flag) Then
							syntaxList2 = Me.ScanSingleLineTrivia()
						Else
							syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
							syntaxList2 = syntaxList1
						End If
						syntaxList = syntaxList2
						syntaxToken = Me.MakePunctuationToken(syntaxKind, text, visualBasicSyntaxNode, syntaxList.Node)
						Return syntaxToken
					End If
					syntaxToken = Me.MakeEndOfInterpolatedStringToken()
				Else
					syntaxToken = Me.MakeEndOfInterpolatedStringToken()
				End If
			Else
				syntaxToken = Me.MakeEndOfInterpolatedStringToken()
			End If
			Return syntaxToken
		End Function

		Private Function ScanInterpolatedStringText(ByVal scanTrailingWhitespaceAsTrivia As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			If (Me.CanGet()) Then
				Dim num As Integer = 0
				Dim num1 As Integer = 0
				Dim scratch As StringBuilder = Me.GetScratch()
				While Me.CanGet(num)
					Dim chr As Char = Me.Peek(num)
					If (SyntaxFacts.IsLeftCurlyBracket(chr)) Then
						If (Not Me.CanGet(num + 1) OrElse Not SyntaxFacts.IsLeftCurlyBracket(Me.Peek(num + 1))) Then
							Exit While
						End If
						scratch.Append("{{")
						num += 2
						num1 = 0
					ElseIf (SyntaxFacts.IsRightCurlyBracket(chr)) Then
						If (Not Me.CanGet(num + 1) OrElse Not SyntaxFacts.IsRightCurlyBracket(Me.Peek(num + 1))) Then
							Exit While
						End If
						scratch.Append("}}")
						num += 2
						num1 = 0
					ElseIf (Not SyntaxFacts.IsDoubleQuote(chr)) Then
						If (SyntaxFacts.IsNewLine(chr) AndAlso scanTrailingWhitespaceAsTrivia) Then
							Exit While
						End If
						If (Not SyntaxFacts.IsWhitespace(chr) OrElse Not scanTrailingWhitespaceAsTrivia) Then
							scratch.Append(chr)
							num = num + 1
							num1 = 0
						Else
							scratch.Append(chr)
							num = num + 1
							num1 = num1 + 1
						End If
					Else
						If (Not Me.CanGet(num + 1) OrElse Not SyntaxFacts.IsDoubleQuote(Me.Peek(num + 1))) Then
							Exit While
						End If
						scratch.Append(""""C)
						num += 2
						num1 = 0
					End If
				End While
				If (num1 > 0) Then
					num -= num1
					Dim length As StringBuilder = scratch
					length.Length = length.Length - num1
				End If
				Dim str As String = If(num > 0, Me.GetTextNotInterned(num), [String].Empty)
				Dim scratchText As String = Scanner.GetScratchText(scratch, str)
				syntaxToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.InterpolatedStringTextToken(str, scratchText, Nothing, Me.ScanWhitespace(num1))
			Else
				syntaxToken = Me.MakeEndOfInterpolatedStringToken()
			End If
			Return syntaxToken
		End Function

		Private Function ScanIntLiteral(ByRef ReturnValue As Integer, ByRef here As Integer) As Boolean
			Dim flag As Boolean
			If (Me.CanGet(here)) Then
				Dim chr As Char = Me.Peek(here)
				If (SyntaxFacts.IsDecimalDigit(chr)) Then
					Dim num As Integer = SyntaxFacts.IntegralLiteralCharacterValue(chr)
					here = here + 1
					While Me.CanGet(here)
						chr = Me.Peek(here)
						If (Not SyntaxFacts.IsDecimalDigit(chr)) Then
							Exit While
						End If
						Dim num1 As Byte = SyntaxFacts.IntegralLiteralCharacterValue(chr)
						If (num < 214748364 OrElse num = 214748364 AndAlso num1 < 8) Then
							num = num * 10 + num1
							here = here + 1
						Else
							flag = False
							Return flag
						End If
					End While
					ReturnValue = num
					flag = True
				Else
					flag = False
				End If
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function ScanLeadingTrivia() As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			Dim syntaxListBuilder As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxListBuilder = Me._triviaListPool.Allocate()
			Me.ScanWhitespaceAndLineContinuations(syntaxListBuilder)
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = Me.MakeTriviaArray(syntaxListBuilder)
			Me._triviaListPool.Free(syntaxListBuilder)
			Return syntaxList
		End Function

		Private Function ScanLeftAngleBracket(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal charIsFullWidth As Boolean, ByVal scanTrailingTrivia As Scanner.ScanTriviaFunc) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim whitespaceLength As Integer = 1
			If (Not charIsFullWidth AndAlso Me.CanGet(whitespaceLength)) Then
				Dim chr As Char = Me.Peek(whitespaceLength)
				If (chr <> "!"C) Then
					If (chr = "/"C) Then
						syntaxToken = Me.XmlMakeBeginEndElementToken(precedingTrivia, Me._scanSingleLineTriviaFunc)
						Return syntaxToken
					Else
						If (chr <> "?"C) Then
							GoTo Label1
						End If
						syntaxToken = Me.XmlMakeBeginProcessingInstructionToken(precedingTrivia, scanTrailingTrivia)
						Return syntaxToken
					End If
				ElseIf (Me.CanGet(whitespaceLength + 2)) Then
					Dim chr1 As Char = Me.Peek(whitespaceLength + 1)
					If (chr1 = "-"C) Then
						If (Not Me.CanGet(whitespaceLength + 3) OrElse Me.Peek(whitespaceLength + 2) <> "-"C) Then
							GoTo Label1
						End If
						syntaxToken = Me.XmlMakeBeginCommentToken(precedingTrivia, scanTrailingTrivia)
						Return syntaxToken
					ElseIf (chr1 = "["C) Then
						If (Not Me.NextAre(whitespaceLength + 2, "CDATA[")) Then
							GoTo Label1
						End If
						syntaxToken = Me.XmlMakeBeginCDataToken(precedingTrivia, scanTrailingTrivia)
						Return syntaxToken
					End If
				End If
			End If
		Label1:
			whitespaceLength = Me.GetWhitespaceLength(whitespaceLength)
			If (Me.CanGet(whitespaceLength)) Then
				Dim chr2 As Char = Me.Peek(whitespaceLength)
				If (chr2 = "="C OrElse chr2 = "＝"C) Then
					whitespaceLength = whitespaceLength + 1
					syntaxToken = Me.MakeLessThanEqualsToken(precedingTrivia, whitespaceLength)
					Return syntaxToken
				ElseIf (chr2 = ">"C OrElse chr2 = "＞"C) Then
					whitespaceLength = whitespaceLength + 1
					syntaxToken = Me.MakeLessThanGreaterThanToken(precedingTrivia, whitespaceLength)
					Return syntaxToken
				ElseIf (chr2 = "<"C OrElse chr2 = "＜"C) Then
					whitespaceLength = whitespaceLength + 1
					If (Me.CanGet(whitespaceLength)) Then
						chr2 = Me.Peek(whitespaceLength)
						If (chr2 = "%"C OrElse chr2 = "％"C) Then
							syntaxToken = Me.MakeLessThanToken(precedingTrivia, charIsFullWidth)
							Return syntaxToken
						End If
						If (Not Me.TrySkipFollowingEquals(whitespaceLength)) Then
							syntaxToken = Me.MakeLessThanLessThanToken(precedingTrivia, whitespaceLength)
							Return syntaxToken
						Else
							syntaxToken = Me.MakeLessThanLessThanEqualsToken(precedingTrivia, whitespaceLength)
							Return syntaxToken
						End If
					End If
				End If
			End If
			syntaxToken = Me.MakeLessThanToken(precedingTrivia, charIsFullWidth)
			Return syntaxToken
		End Function

		Private Function ScanLineContinuation(ByVal tList As SyntaxListBuilder) As Boolean
			Dim flag As Boolean
			Dim chr As Char = Strings.ChrW(0)
			If (Not Me.TryGet(0, chr)) Then
				flag = False
			ElseIf (Not Me.IsAfterWhitespace()) Then
				flag = False
			ElseIf (SyntaxFacts.IsUnderscore(chr)) Then
				Dim whitespaceLength As Integer = Me.GetWhitespaceLength(1)
				Me.TryGet(whitespaceLength, chr)
				Dim flag1 As Boolean = SyntaxFacts.IsSingleQuote(chr)
				Dim flag2 As Boolean = SyntaxFacts.IsNewLine(chr)
				If (flag1 OrElse flag2 OrElse Not Me.CanGet(whitespaceLength)) Then
					tList.Add(Me.MakeLineContinuationTrivia(Me.GetText(1)))
					If (whitespaceLength > 1) Then
						tList.Add(Me.MakeWhiteSpaceTrivia(Me.GetText(whitespaceLength - 1)))
					End If
					If (flag1) Then
						Dim syntaxTrivium As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia = Me.ScanComment()
						If (Not Me.CheckFeatureAvailability(Feature.CommentsAfterLineContinuation)) Then
							syntaxTrivium = syntaxTrivium.WithDiagnostics(New DiagnosticInfo() { ErrorFactory.ErrorInfo(ERRID.ERR_CommentsAfterLineContinuationNotAvailable1, New [Object]() { New VisualBasicRequiredLanguageVersion(Feature.CommentsAfterLineContinuation.GetLanguageVersion()) }) })
						End If
						tList.Add(syntaxTrivium)
						If (Me.CanGet()) Then
							chr = Me.Peek()
							flag2 = SyntaxFacts.IsNewLine(chr)
						End If
					End If
					If (flag2) Then
						Dim num As Integer = Me.SkipLineBreak(chr, 0)
						whitespaceLength = Me.GetWhitespaceLength(num)
						Dim num1 As Integer = whitespaceLength - num
						If (Me.PeekStartComment(whitespaceLength) = 0 AndAlso Me.CanGet(whitespaceLength) AndAlso Not SyntaxFacts.IsNewLine(Me.Peek(whitespaceLength))) Then
							tList.Add(Me.MakeEndOfLineTrivia(Me.GetText(num)))
							If (num1 > 0) Then
								tList.Add(Me.MakeWhiteSpaceTrivia(Me.GetText(num1)))
							End If
						End If
					End If
					flag = True
				Else
					flag = False
				End If
			Else
				flag = False
			End If
			Return flag
		End Function

		Friend Function ScanMultilineTrivia() As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			If (Me.CanGet()) Then
				Dim chr As Char = Me.Peek()
				If (chr <= ":"C OrElse chr > "~"C OrElse chr = "'"C OrElse chr = Strings.ChrW(95) OrElse chr = "R"C OrElse chr = "r"C OrElse chr = "<"C OrElse chr = "="C OrElse chr = ">"C) Then
					Dim syntaxListBuilder As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxListBuilder = Me._triviaListPool.Allocate()
					While Me.TryScanSinglePieceOfMultilineTrivia(syntaxListBuilder)
					End While
					Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = Me.MakeTriviaArray(syntaxListBuilder)
					Me._triviaListPool.Free(syntaxListBuilder)
					syntaxList = syntaxList1
				Else
					syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
				End If
			Else
				syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
			End If
			Return syntaxList
		End Function

		Private Function ScanNewlineAsStatementTerminator(ByVal startCharacter As Char, ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			If (Me._lineBufferOffset >= Me._endOfTerminatorTrivia) Then
				syntaxToken = Me.MakeEmptyToken(precedingTrivia)
			Else
				Dim num As Integer = Me.LengthOfLineBreak(startCharacter, 0)
				syntaxToken = Me.MakeStatementTerminatorToken(precedingTrivia, num)
			End If
			Return syntaxToken
		End Function

		Private Function ScanNewlineAsTrivia(ByVal StartCharacter As Char) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Dim syntaxTrivium As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			syntaxTrivium = If(Me.LengthOfLineBreak(StartCharacter, 0) <> 2, Me.MakeEndOfLineTrivia(Me.GetNextChar()), Me.MakeEndOfLineTriviaCRLF())
			Return syntaxTrivium
		End Function

		Private Function ScanNextCharAsToken(ByVal leadingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			If (Me.CanGet()) Then
				Me._badTokenCount = Me._badTokenCount + 1
				syntaxToken = If(Me._badTokenCount >= 200, Me.MakeBadToken(leadingTrivia, Me.RemainingLength(), ERRID.ERR_IllegalChar), Me.MakeBadToken(leadingTrivia, If(Not SyntaxFacts.IsHighSurrogate(Me.Peek()) OrElse Not Me.CanGet(1) OrElse Not SyntaxFacts.IsLowSurrogate(Me.Peek(1)), 1, 2), ERRID.ERR_IllegalChar))
			Else
				syntaxToken = Scanner.MakeEofToken(leadingTrivia)
			End If
			Return syntaxToken
		End Function

		Private Function ScanNextToken(ByVal allowLeadingMultilineTrivia As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			Dim syntaxToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			If (Not allowLeadingMultilineTrivia) Then
				syntaxList = Me.ScanLeadingTrivia()
				If (Me.PeekStartComment(0) <= 0) Then
					syntaxToken1 = If(Me.TryScanToken(syntaxList), Me.ScanNextCharAsToken(syntaxList))
					If (Me._lineBufferOffset > Me._endOfTerminatorTrivia) Then
						Me._endOfTerminatorTrivia = Me._lineBufferOffset
					End If
					syntaxToken = syntaxToken1
					Return syntaxToken
				End If
				syntaxToken = Me.MakeEmptyToken(syntaxList)
				Return syntaxToken
			Else
				syntaxList = Me.ScanMultilineTrivia()
			End If
			syntaxToken1 = If(Me.TryScanToken(syntaxList), Me.ScanNextCharAsToken(syntaxList))
			If (Me._lineBufferOffset > Me._endOfTerminatorTrivia) Then
				Me._endOfTerminatorTrivia = Me._lineBufferOffset
			End If
			syntaxToken = syntaxToken1
			Return syntaxToken
		End Function

		Private Function ScanNumericLiteral(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim num As Integer
			Dim flag As Boolean = False
			Dim num1 As ULong = 0L
			Dim num2 As Double = 0
			Dim num3 As [Decimal] = New Decimal()
			Dim syntaxToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim [single] As Single
			Dim num4 As ULong
			Dim num5 As Integer
			Dim num6 As ULong
			Dim num7 As Integer = 0
			Dim flag1 As Boolean = False
			Dim flag2 As Boolean = False
			Dim literalBase As Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralBase = Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralBase.[Decimal]
			Dim numericLiteralKind As Scanner.NumericLiteralKind = Scanner.NumericLiteralKind.Integral
			Dim chr As Char = Me.Peek()
			If (chr = "&"C OrElse chr = "＆"C) Then
				num7 = num7 + 1
				chr = If(Me.CanGet(num7), Me.Peek(num7), Strings.ChrW(0))
				While True
					If (chr > "O"C) Then
						If (chr = "b"C) Then
							Exit While
						End If
						If (chr = "h"C) Then
							GoTo Label1
						End If
						If (chr = "o"C) Then
							GoTo Label2
						End If
					Else
						If (chr = "B"C) Then
							Exit While
						End If
						If (chr = "H"C) Then
							GoTo Label1
						End If
						If (chr = "O"C) Then
							GoTo Label2
						End If
					End If
					If (Not SyntaxFacts.IsFullWidth(chr)) Then
						Throw ExceptionUtilities.UnexpectedValue(chr)
					End If
					chr = SyntaxFacts.MakeHalfWidth(chr)
				End While
				num7 = num7 + 1
				num = num7
				literalBase = Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralBase.Binary
				If (Me.CanGet(num7) AndAlso Me.Peek(num7) = Strings.ChrW(95)) Then
					flag2 = True
				End If
				While Me.CanGet(num7)
					chr = Me.Peek(num7)
					If (Not SyntaxFacts.IsBinaryDigit(chr) AndAlso chr <> Strings.ChrW(95)) Then
						Exit While
					End If
					If (chr = Strings.ChrW(95)) Then
						flag1 = True
					End If
					num7 = num7 + 1
				End While
				flag = flag Or Me.Peek(num7 - 1) = Strings.ChrW(95)
			Else
				num = num7
				flag = If(Not Me.CanGet(num7), False, Me.Peek(num7) = Strings.ChrW(95))
				While Me.CanGet(num7)
					chr = Me.Peek(num7)
					If (Not SyntaxFacts.IsDecimalDigit(chr) AndAlso chr <> Strings.ChrW(95)) Then
						Exit While
					End If
					If (chr = Strings.ChrW(95)) Then
						flag1 = True
					End If
					num7 = num7 + 1
				End While
				If (num7 <> num) Then
					flag = flag Or Me.Peek(num7 - 1) = Strings.ChrW(95)
				End If
			End If
		Label0:
			Dim num8 As Integer = num7
			If (literalBase = Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralBase.[Decimal] AndAlso Me.CanGet(num7)) Then
				chr = Me.Peek(num7)
				If (chr = "."C Or chr = "．"C AndAlso Me.CanGet(num7 + 1) AndAlso SyntaxFacts.IsDecimalDigit(Me.Peek(num7 + 1))) Then
					num7 += 2
					While Me.CanGet(num7)
						chr = Me.Peek(num7)
						If (Not SyntaxFacts.IsDecimalDigit(chr) AndAlso chr <> Strings.ChrW(95)) Then
							Exit While
						End If
						num7 = num7 + 1
					End While
					flag = flag Or Me.Peek(num7 - 1) = Strings.ChrW(95)
					numericLiteralKind = Scanner.NumericLiteralKind.Float
				End If
				If (Me.CanGet(num7) AndAlso SyntaxFacts.BeginsExponent(Me.Peek(num7))) Then
					num7 = num7 + 1
					If (Me.CanGet(num7)) Then
						chr = Me.Peek(num7)
						If (SyntaxFacts.MatchOneOrAnotherOrFullwidth(chr, "+"C, "-"C)) Then
							num7 = num7 + 1
						End If
					End If
					If (Me.CanGet(num7) AndAlso SyntaxFacts.IsDecimalDigit(Me.Peek(num7))) Then
						GoTo Label4
					End If
					syntaxToken = Me.MakeBadToken(precedingTrivia, num7, ERRID.ERR_InvalidLiteralExponent)
					Return syntaxToken
				End If
			End If
		Label15:
			Dim num9 As Integer = num7
			Dim typeCharacter As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.None
			If (Me.CanGet(num7)) Then
				chr = Me.Peek(num7)
				While True
					If (chr > "L"C) Then
						If (chr > "f"C) Then
							If (chr = "i"C) Then
								GoTo Label6
							End If
							If (chr = "l"C) Then
								GoTo Label7
							End If
							Select Case chr
								Case "r"C
									GoTo Label11
								Case "s"C
									Exit Select
								Case "u"C
									GoTo Label8
								Case Else
									GoTo Label12
							End Select
						Else
							Select Case chr
								Case "R"C
									GoTo Label11
								Case "S"C
									Exit Select
								Case "T"C
									GoTo Label12
								Case "U"C
									GoTo Label8
								Case Else
									If (chr = "d"C) Then
										GoTo Label9
									End If
									If (chr = "f"C) Then
										GoTo Label10
									End If
									GoTo Label12
							End Select
						End If
						If (numericLiteralKind = Scanner.NumericLiteralKind.Float) Then
							GoTo Label5
						End If
						typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.ShortLiteral
						num7 = num7 + 1
						GoTo Label5
					ElseIf (chr > "D"C) Then
						If (chr = "F"C) Then
							GoTo Label10
						End If
						If (chr = "I"C) Then
							GoTo Label6
						End If
						If (chr = "L"C) Then
							GoTo Label7
						End If
					Else
						Select Case chr
							Case "!"C
								If (literalBase <> Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralBase.[Decimal]) Then
									GoTo Label5
								End If
								typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.[Single]
								numericLiteralKind = Scanner.NumericLiteralKind.Float
								num7 = num7 + 1
								GoTo Label5
							Case """"C
							Case "$"C
								Exit Select
							Case "#"C
								If (literalBase <> Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralBase.[Decimal]) Then
									GoTo Label5
								End If
								typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.[Double]
								numericLiteralKind = Scanner.NumericLiteralKind.Float
								num7 = num7 + 1
								GoTo Label5
							Case "%"C
								If (numericLiteralKind = Scanner.NumericLiteralKind.Float) Then
									GoTo Label5
								End If
								typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.[Integer]
								num7 = num7 + 1
								GoTo Label5
							Case "&"C
								If (numericLiteralKind = Scanner.NumericLiteralKind.Float) Then
									GoTo Label5
								End If
								typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.[Long]
								num7 = num7 + 1
								GoTo Label5
							Case Else
								If (chr = "@"C) Then
									If (literalBase <> Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralBase.[Decimal]) Then
										GoTo Label5
									End If
									typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.[Decimal]
									numericLiteralKind = Scanner.NumericLiteralKind.[Decimal]
									num7 = num7 + 1
									GoTo Label5
								Else
									If (chr = "D"C) Then
										GoTo Label9
									End If
									Exit Select
								End If
						End Select
					End If
				Label12:
					If (Not SyntaxFacts.IsFullWidth(chr)) Then
						GoTo Label5
					End If
					chr = SyntaxFacts.MakeHalfWidth(chr)
				End While
			Label8:
				If (numericLiteralKind <> Scanner.NumericLiteralKind.Float AndAlso Me.CanGet(num7 + 1)) Then
					Dim chr1 As Char = Me.Peek(num7 + 1)
					If (SyntaxFacts.MatchOneOrAnotherOrFullwidth(chr1, "S"C, "s"C)) Then
						typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.UShortLiteral
						num7 += 2
					ElseIf (SyntaxFacts.MatchOneOrAnotherOrFullwidth(chr1, "I"C, "i"C)) Then
						typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.UIntegerLiteral
						num7 += 2
					ElseIf (SyntaxFacts.MatchOneOrAnotherOrFullwidth(chr1, "L"C, "l"C)) Then
						typeCharacter = Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.ULongLiteral
						num7 += 2
					End If
				End If
			End If
		Label5:
			Dim decimalValue As Boolean = False
			If (numericLiteralKind <> Scanner.NumericLiteralKind.Integral) Then
				Dim scratch As StringBuilder = Me.GetScratch()
				Dim num10 As Integer = num9 - 1
				Dim num11 As Integer = 0
				Do
					Dim chr2 As Char = Me.Peek(num11)
					If (chr2 <> Strings.ChrW(95)) Then
						scratch.Append(If(SyntaxFacts.IsFullWidth(chr2), SyntaxFacts.MakeHalfWidth(chr2), chr2))
					End If
					num11 = num11 + 1
				Loop While num11 <= num10
				Dim scratchTextInterned As String = Me.GetScratchTextInterned(scratch)
				If (numericLiteralKind = Scanner.NumericLiteralKind.[Decimal]) Then
					decimalValue = Not Scanner.GetDecimalValue(scratchTextInterned, num3)
				ElseIf (typeCharacter <> Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.[Single] AndAlso typeCharacter <> Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeCharacter.SingleLiteral) Then
					If (Not RealParser.TryParseDouble(scratchTextInterned, num2)) Then
						decimalValue = True
					End If
				ElseIf (RealParser.TryParseFloat(scratchTextInterned, [single])) Then
					num2 = CDbl([single])
				Else
					decimalValue = True
				End If
			Else
				If (num = num8) Then
					syntaxToken = Me.MakeBadToken(precedingTrivia, num7, ERRID.ERR_Syntax)
					Return syntaxToken
				End If
				num1 = CULng(0)
				If (literalBase <> Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralBase.[Decimal]) Then
					If (literalBase = Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralBase.Hexadecimal) Then
						num5 = 4
					Else
						num5 = If(literalBase = Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralBase.Octal, 3, 1)
					End If
					Dim num12 As Integer = num5
					If (literalBase = Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralBase.Hexadecimal) Then
						num6 = -1152921504606846976L
					Else
						num6 = CULng(If(literalBase = Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralBase.Octal, -2305843009213693952L, -9223372036854775808L))
					End If
					Dim num13 As ULong = num6
					Dim num14 As Integer = num8 - 1
					For i As Integer = num To num14
						Dim chr3 As Char = Me.Peek(i)
						If (chr3 <> Strings.ChrW(95)) Then
							If ([Decimal].Compare(New [Decimal](num1 And num13), [Decimal].Zero) <> 0) Then
								decimalValue = True
							End If
							num1 = ' 
							' Current member / type: Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Scanner::ScanNumericLiteral(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList`1<Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode>)
							' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
							' 
							' Product version: 2019.1.118.0
							' Exception in: Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken ScanNumericLiteral(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode>)
							' 
							' La référence d'objet n'est pas définie à une instance d'un objet.
							'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Expression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 1019
							'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 946
							'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 141
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 827
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 822
							'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 998
							'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 141
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2183
							'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 84
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(IEnumerable ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2177
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2139
							'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2137
							'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
							'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(IfStatement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 108
							'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 78
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(IEnumerable ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2177
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2139
							'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2137
							'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
							'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 1339
							'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 102
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(IEnumerable ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2177
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2139
							'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2137
							'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
							'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(IfStatement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 108
							'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 78
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(IEnumerable ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2177
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2139
							'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2137
							'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
							'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(IfStatement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 119
							'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 78
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(IEnumerable ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2177
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2139
							'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2137
							'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(Statement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1060
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(MethodDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1916
							'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(MethodDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1841
							'    à ..WriteInternal(IMemberDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseLanguageWriter.cs:ligne 447
							' 
							' mailto: JustDecompilePublicFeedback@telerik.com


		Private Function ScanRightAngleBracket(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal charIsFullWidth As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim whitespaceLength As Integer = 1
			whitespaceLength = Me.GetWhitespaceLength(whitespaceLength)
			If (Me.CanGet(whitespaceLength)) Then
				Dim chr As Char = Me.Peek(whitespaceLength)
				If (chr = "="C OrElse chr = "＝"C) Then
					whitespaceLength = whitespaceLength + 1
					syntaxToken = Me.MakeGreaterThanEqualsToken(precedingTrivia, whitespaceLength)
					Return syntaxToken
				Else
					If (chr <> ">"C AndAlso chr <> "＞"C) Then
						syntaxToken = Me.MakeGreaterThanToken(precedingTrivia, charIsFullWidth)
						Return syntaxToken
					End If
					whitespaceLength = whitespaceLength + 1
					If (Not Me.TrySkipFollowingEquals(whitespaceLength)) Then
						syntaxToken = Me.MakeGreaterThanGreaterThanToken(precedingTrivia, whitespaceLength)
						Return syntaxToken
					Else
						syntaxToken = Me.MakeGreaterThanGreaterThanEqualsToken(precedingTrivia, whitespaceLength)
						Return syntaxToken
					End If
				End If
			End If
			syntaxToken = Me.MakeGreaterThanToken(precedingTrivia, charIsFullWidth)
			Return syntaxToken
		End Function

		Friend Function ScanSingleLineTrivia() As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			Dim syntaxListBuilder As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxListBuilder = Me._triviaListPool.Allocate()
			Me.ScanSingleLineTrivia(syntaxListBuilder)
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = Me.MakeTriviaArray(syntaxListBuilder)
			Me._triviaListPool.Free(syntaxListBuilder)
			Return syntaxList
		End Function

		Private Sub ScanSingleLineTrivia(ByVal tList As SyntaxListBuilder)
			If (Me.IsScanningXmlDoc) Then
				Me.ScanSingleLineTriviaInXmlDoc(tList)
				Return
			End If
			Me.ScanWhitespaceAndLineContinuations(tList)
			Me.ScanCommentIfAny(tList)
			Me.ScanTerminatorTrivia(tList)
		End Sub

		Private Function ScanSingleLineTrivia(ByVal includeFollowingBlankLines As Boolean) As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			Dim lineBufferAndEndOfTerminatorOffset As Scanner.LineBufferAndEndOfTerminatorOffsets
			Dim syntaxListBuilder As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxListBuilder = Me._triviaListPool.Allocate()
			Me.ScanSingleLineTrivia(syntaxListBuilder)
			If (includeFollowingBlankLines AndAlso Scanner.IsBlankLine(syntaxListBuilder)) Then
				Dim syntaxListBuilder1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxListBuilder = Me._triviaListPool.Allocate()
				While True
					lineBufferAndEndOfTerminatorOffset = Me.CreateOffsetRestorePoint()
					Me._lineBufferOffset = Me._endOfTerminatorTrivia
					Me.ScanSingleLineTrivia(syntaxListBuilder1)
					If (Not Scanner.IsBlankLine(syntaxListBuilder1)) Then
						Exit While
					End If
					Dim count As Integer = syntaxListBuilder1.Count - 1
					Dim num As Integer = 0
					Do
						syntaxListBuilder.Add(syntaxListBuilder1(num))
						num = num + 1
					Loop While num <= count
					syntaxListBuilder1.Clear()
				End While
				lineBufferAndEndOfTerminatorOffset.Restore()
				Me._triviaListPool.Free(syntaxListBuilder1)
			End If
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode) = syntaxListBuilder.ToList()
			Me._triviaListPool.Free(syntaxListBuilder)
			Return list
		End Function

		Private Sub ScanSingleLineTriviaInXmlDoc(ByVal tList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxListBuilder)
			If (Me.CanGet()) Then
				Dim chr As Char = Me.Peek()
				Select Case chr
					Case Strings.ChrW(9)
					Case Strings.ChrW(10)
					Case Strings.ChrW(13)
					Label0:
						Dim lineBufferAndEndOfTerminatorOffset As Scanner.LineBufferAndEndOfTerminatorOffsets = Me.CreateOffsetRestorePoint()
						Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = Me._triviaListPool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
						If (Not Me.ScanXmlTriviaInXmlDoc(chr, syntaxListBuilder)) Then
							Me._triviaListPool.Free(syntaxListBuilder)
							lineBufferAndEndOfTerminatorOffset.Restore()
							Return
						End If
						Dim count As Integer = syntaxListBuilder.Count - 1
						Dim num As Integer = 0
						Do
							tList.Add(syntaxListBuilder(num))
							num = num + 1
						Loop While num <= count
						Me._triviaListPool.Free(syntaxListBuilder)
						Exit Select
					Case Strings.ChrW(11)
					Case Strings.ChrW(12)
						Exit Select
					Case Else
						If (chr <> Strings.ChrW(32)) Then
							Exit Select
						Else
							GoTo Label0
						End If
				End Select
			End If
		End Sub

		Private Function ScanStringLiteral(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			Dim num As Integer = 1
			If (Me.CanGet(3) AndAlso SyntaxFacts.IsDoubleQuote(Me.Peek(2))) Then
				If (Not SyntaxFacts.IsDoubleQuote(Me.Peek(1))) Then
					If (Not SyntaxFacts.IsLetterC(Me.Peek(3))) Then
						GoTo Label1
					End If
					syntaxToken = Me.MakeCharacterLiteralToken(precedingTrivia, Me.Peek(1), 4)
					Return syntaxToken
				Else
					If (Not SyntaxFacts.IsDoubleQuote(Me.Peek(3)) OrElse Not Me.CanGet(4) OrElse Not SyntaxFacts.IsLetterC(Me.Peek(4))) Then
						GoTo Label1
					End If
					syntaxToken = Me.MakeCharacterLiteralToken(precedingTrivia, """"C, 5)
					Return syntaxToken
				End If
			End If
		Label1:
			If (Not Me.CanGet(2) OrElse Not SyntaxFacts.IsDoubleQuote(Me.Peek(1)) OrElse Not SyntaxFacts.IsLetterC(Me.Peek(2))) Then
				Dim flag As Boolean = False
				Dim scratch As StringBuilder = Me.GetScratch()
				While Me.CanGet(num)
					Dim chr As Char = Me.Peek(num)
					If (Not SyntaxFacts.IsDoubleQuote(chr)) Then
						If (SyntaxFacts.IsNewLine(chr)) Then
							If (Me._isScanningDirective) Then
								Exit While
							End If
							flag = True
						End If
						scratch.Append(chr)
						num = num + 1
					Else
						If (Me.CanGet(num + 1)) Then
							chr = Me.Peek(num + 1)
							If (SyntaxFacts.IsDoubleQuote(chr)) Then
								scratch.Append(""""C)
								num += 2
								Continue While
							ElseIf (SyntaxFacts.IsLetterC(chr)) Then
								scratch.Clear()
								syntaxToken = Me.MakeBadToken(precedingTrivia, num + 2, ERRID.ERR_IllegalCharConstant)
								Return syntaxToken
							End If
						End If
						num = num + 1
						Dim textNotInterned As String = Me.GetTextNotInterned(num)
						syntaxList = Me.ScanSingleLineTrivia()
						Dim syntaxToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.StringLiteralToken(textNotInterned, Scanner.GetScratchText(scratch), precedingTrivia.Node, syntaxList.Node)
						If (flag) Then
							syntaxToken1 = Parser.CheckFeatureAvailability(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(Feature.MultilineStringLiterals, syntaxToken1, Me.Options.LanguageVersion)
						End If
						syntaxToken = syntaxToken1
						Return syntaxToken
					End If
				End While
				Dim str As String = Me.GetTextNotInterned(num)
				syntaxList = Me.ScanSingleLineTrivia()
				syntaxToken = DirectCast(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.StringLiteralToken(str, Scanner.GetScratchText(scratch), precedingTrivia.Node, syntaxList.Node).SetDiagnostics(New DiagnosticInfo() { ErrorFactory.ErrorInfo(ERRID.ERR_UnterminatedStringLiteral) }), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			Else
				syntaxToken = Me.MakeBadToken(precedingTrivia, 3, ERRID.ERR_IllegalCharConstant)
			End If
			Return syntaxToken
		End Function

		Private Function ScanSurrogatePair(ByVal c1 As Char, ByVal Here As Integer) As Scanner.XmlCharResult
			Dim xmlCharResult As Scanner.XmlCharResult
			If (SyntaxFacts.IsHighSurrogate(c1) AndAlso Me.CanGet(Here + 1)) Then
				Dim chr As Char = Me.Peek(Here + 1)
				If (Not SyntaxFacts.IsLowSurrogate(chr)) Then
					xmlCharResult = New Scanner.XmlCharResult()
					Return xmlCharResult
				End If
				xmlCharResult = New Scanner.XmlCharResult(c1, chr)
				Return xmlCharResult
			End If
			xmlCharResult = New Scanner.XmlCharResult()
			Return xmlCharResult
		End Function

		Private Sub ScanTerminatorTrivia(ByVal tList As SyntaxListBuilder)
			If (Me.CanGet()) Then
				Dim chr As Char = Me.Peek()
				Dim num As Integer = Me._lineBufferOffset
				If (SyntaxFacts.IsNewLine(chr)) Then
					tList.Add(Me.ScanNewlineAsTrivia(chr))
				ElseIf (Me.IsColonAndNotColonEquals(chr, 0)) Then
					tList.Add(Me.ScanColonAsTrivia())
					While True
						Dim whitespaceLength As Integer = Me.GetWhitespaceLength(0)
						If (Not Me.CanGet(whitespaceLength)) Then
							Exit While
						End If
						chr = Me.Peek(whitespaceLength)
						If (Not Me.IsColonAndNotColonEquals(chr, whitespaceLength)) Then
							Exit While
						End If
						If (whitespaceLength > 0) Then
							tList.Add(Me.MakeWhiteSpaceTrivia(Me.GetText(whitespaceLength)))
						End If
						num = Me._lineBufferOffset
						tList.Add(Me.ScanColonAsTrivia())
					End While
				End If
				Me._endOfTerminatorTrivia = Me._lineBufferOffset
				Me._lineBufferOffset = num
			End If
		End Sub

		Private Function ScanTokenCommon(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal ch As Char, ByVal fullWidth As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim num As Integer = 1
			Dim chr As Char = ch
			If (chr > Strings.ChrW(8232)) Then
				If (chr = Strings.ChrW(8233)) Then
					GoTo Label0
				End If
				If (chr = "＄"C) Then
					GoTo Label1
				End If
				GoTo Label2
			Else
				Select Case chr
					Case Strings.ChrW(9)
					Case Strings.ChrW(32)
					Case "'"C
						syntaxToken = Nothing
						Exit Select
					Case Strings.ChrW(10)
					Case Strings.ChrW(13)
						syntaxToken = Me.ScanNewlineAsStatementTerminator(ch, precedingTrivia)
						Exit Select
					Case Strings.ChrW(11)
					Case Strings.ChrW(12)
					Case Strings.ChrW(14)
					Case Strings.ChrW(15)
					Case Strings.ChrW(16)
					Case Strings.ChrW(17)
					Case Strings.ChrW(18)
					Case Strings.ChrW(19)
					Case Strings.ChrW(20)
					Case Strings.ChrW(21)
					Case Strings.ChrW(22)
					Case Strings.ChrW(23)
					Case Strings.ChrW(24)
					Case Strings.ChrW(25)
					Case Strings.ChrW(26)
					Case Strings.ChrW(27)
					Case Strings.ChrW(28)
					Case Strings.ChrW(29)
					Case Strings.ChrW(30)
					Case Strings.ChrW(31)
					Case ";"C
					Case "]"C
					Case Strings.ChrW(96)
					Case "|"C
					Case "~"C
					Case Strings.ChrW(127)
					Case Strings.ChrW(128)
					Case Strings.ChrW(129)
					Case Strings.ChrW(130)
					Case Strings.ChrW(131)
					Case Strings.ChrW(132)
						GoTo Label2
					Case "!"C
						syntaxToken = Me.MakeExclamationToken(precedingTrivia, fullWidth)
						Exit Select
					Case """"C
						syntaxToken = Me.ScanStringLiteral(precedingTrivia)
						Exit Select
					Case "#"C
						Dim syntaxToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.ScanDateLiteral(precedingTrivia)
						If (syntaxToken1 Is Nothing) Then
							syntaxToken1 = Me.MakeHashToken(precedingTrivia, fullWidth)
						End If
						syntaxToken = syntaxToken1
						Exit Select
					Case "$"C
						GoTo Label1
					Case "%"C
						If (Not Me.NextIs(1, ">"C)) Then
							GoTo Label2
						End If
						syntaxToken = Me.XmlMakeEndEmbeddedToken(precedingTrivia, Me._scanSingleLineTriviaFunc)
						Exit Select
					Case "&"C
						If (Me.CanGet(1) AndAlso SyntaxFacts.BeginsBaseLiteral(Me.Peek(1))) Then
							syntaxToken = Me.ScanNumericLiteral(precedingTrivia)
							Exit Select
						ElseIf (Not Me.TrySkipFollowingEquals(num)) Then
							syntaxToken = Me.MakeAmpersandToken(precedingTrivia, fullWidth)
							Exit Select
						Else
							syntaxToken = Me.MakeAmpersandEqualsToken(precedingTrivia, num)
							Exit Select
						End If
					Case "("C
						syntaxToken = Me.MakeOpenParenToken(precedingTrivia, fullWidth)
						Exit Select
					Case ")"C
						syntaxToken = Me.MakeCloseParenToken(precedingTrivia, fullWidth)
						Exit Select
					Case "*"C
						If (Not Me.TrySkipFollowingEquals(num)) Then
							syntaxToken = Me.MakeAsteriskToken(precedingTrivia, fullWidth)
							Exit Select
						Else
							syntaxToken = Me.MakeAsteriskEqualsToken(precedingTrivia, num)
							Exit Select
						End If
					Case "+"C
						If (Not Me.TrySkipFollowingEquals(num)) Then
							syntaxToken = Me.MakePlusToken(precedingTrivia, fullWidth)
							Exit Select
						Else
							syntaxToken = Me.MakePlusEqualsToken(precedingTrivia, num)
							Exit Select
						End If
					Case ","C
						syntaxToken = Me.MakeCommaToken(precedingTrivia, fullWidth)
						Exit Select
					Case "-"C
						If (Not Me.TrySkipFollowingEquals(num)) Then
							syntaxToken = Me.MakeMinusToken(precedingTrivia, fullWidth)
							Exit Select
						Else
							syntaxToken = Me.MakeMinusEqualsToken(precedingTrivia, num)
							Exit Select
						End If
					Case "."C
						If (Not Me.CanGet(1) OrElse Not SyntaxFacts.IsDecimalDigit(Me.Peek(1))) Then
							syntaxToken = Me.MakeDotToken(precedingTrivia, fullWidth)
							Exit Select
						Else
							syntaxToken = Me.ScanNumericLiteral(precedingTrivia)
							Exit Select
						End If
					Case "/"C
						If (Not Me.TrySkipFollowingEquals(num)) Then
							syntaxToken = Me.MakeSlashToken(precedingTrivia, fullWidth)
							Exit Select
						Else
							syntaxToken = Me.MakeSlashEqualsToken(precedingTrivia, num)
							Exit Select
						End If
					Case "0"C
					Case "1"C
					Case "2"C
					Case "3"C
					Case "4"C
					Case "5"C
					Case "6"C
					Case "7"C
					Case "8"C
					Case "9"C
						syntaxToken = Me.ScanNumericLiteral(precedingTrivia)
						Exit Select
					Case ":"C
						If (Not Me.TrySkipFollowingEquals(num)) Then
							syntaxToken = Me.ScanColonAsStatementTerminator(precedingTrivia, fullWidth)
							Exit Select
						Else
							syntaxToken = Me.MakeColonEqualsToken(precedingTrivia, num)
							Exit Select
						End If
					Case "<"C
						syntaxToken = Me.ScanLeftAngleBracket(precedingTrivia, fullWidth, Me._scanSingleLineTriviaFunc)
						Exit Select
					Case "="C
						syntaxToken = Me.MakeEqualsToken(precedingTrivia, fullWidth)
						Exit Select
					Case ">"C
						syntaxToken = Me.ScanRightAngleBracket(precedingTrivia, fullWidth)
						Exit Select
					Case "?"C
						syntaxToken = Me.MakeQuestionToken(precedingTrivia, fullWidth)
						Exit Select
					Case "@"C
						syntaxToken = Me.MakeAtToken(precedingTrivia, fullWidth)
						Exit Select
					Case "A"C
						If (Not Me.NextAre(1, "s ")) Then
							syntaxToken = Me.ScanIdentifierOrKeyword(precedingTrivia)
							Exit Select
						Else
							Me.AdvanceChar(2)
							syntaxToken = Me.MakeKeyword(SyntaxKind.AsKeyword, "As", precedingTrivia)
							Exit Select
						End If
					Case "B"C
					Case "C"C
					Case "D"C
					Case "F"C
					Case "G"C
					Case "H"C
					Case "J"C
					Case "K"C
					Case "L"C
					Case "M"C
					Case "N"C
					Case "O"C
					Case "P"C
					Case "Q"C
					Case "R"C
					Case "S"C
					Case "T"C
					Case "U"C
					Case "V"C
					Case "W"C
					Case "X"C
					Case "Y"C
					Case "Z"C
						syntaxToken = Me.ScanIdentifierOrKeyword(precedingTrivia)
						Exit Select
					Case "E"C
						If (Not Me.NextAre(1, "nd ")) Then
							syntaxToken = Me.ScanIdentifierOrKeyword(precedingTrivia)
							Exit Select
						Else
							Me.AdvanceChar(3)
							syntaxToken = Me.MakeKeyword(SyntaxKind.EndKeyword, "End", precedingTrivia)
							Exit Select
						End If
					Case "I"C
						If (Not Me.NextAre(1, "f ")) Then
							syntaxToken = Me.ScanIdentifierOrKeyword(precedingTrivia)
							Exit Select
						Else
							Me.AdvanceChar(2)
							syntaxToken = Me.MakeKeyword(SyntaxKind.IfKeyword, "If", precedingTrivia)
							Exit Select
						End If
					Case "["C
						syntaxToken = Me.ScanBracketedIdentifier(precedingTrivia)
						Exit Select
					Case "\"C
						If (Not Me.TrySkipFollowingEquals(num)) Then
							syntaxToken = Me.MakeBackslashToken(precedingTrivia, fullWidth)
							Exit Select
						Else
							syntaxToken = Me.MakeBackSlashEqualsToken(precedingTrivia, num)
							Exit Select
						End If
					Case Strings.ChrW(94)
						If (Not Me.TrySkipFollowingEquals(num)) Then
							syntaxToken = Me.MakeCaretToken(precedingTrivia, fullWidth)
							Exit Select
						Else
							syntaxToken = Me.MakeCaretEqualsToken(precedingTrivia, num)
							Exit Select
						End If
					Case Strings.ChrW(95)
						If (Not Me.CanGet(1) OrElse Not SyntaxFacts.IsIdentifierPartCharacter(Me.Peek(1))) Then
							Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedIdentifier
							Dim whitespaceLength As Integer = Me.GetWhitespaceLength(1)
							If (Not Me.CanGet(whitespaceLength) OrElse SyntaxFacts.IsNewLine(Me.Peek(whitespaceLength)) OrElse Me.PeekStartComment(whitespaceLength) > 0) Then
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_LineContWithCommentOrNoPrecSpace
							End If
							syntaxToken = Me.MakeBadToken(precedingTrivia, 1, eRRID)
							Exit Select
						Else
							syntaxToken = Me.ScanIdentifierOrKeyword(precedingTrivia)
							Exit Select
						End If
					Case "a"C
					Case "b"C
					Case "c"C
					Case "d"C
					Case "e"C
					Case "f"C
					Case "g"C
					Case "h"C
					Case "i"C
					Case "j"C
					Case "k"C
					Case "l"C
					Case "m"C
					Case "n"C
					Case "o"C
					Case "p"C
					Case "q"C
					Case "r"C
					Case "s"C
					Case "t"C
					Case "u"C
					Case "v"C
					Case "w"C
					Case "x"C
					Case "y"C
					Case "z"C
						syntaxToken = Me.ScanIdentifierOrKeyword(precedingTrivia)
						Exit Select
					Case "{"C
						syntaxToken = Me.MakeOpenBraceToken(precedingTrivia, fullWidth)
						Exit Select
					Case "}"C
						syntaxToken = Me.MakeCloseBraceToken(precedingTrivia, fullWidth)
						Exit Select
					Case Strings.ChrW(133)
						GoTo Label0
					Case Else
						If (chr = Strings.ChrW(8232)) Then
							GoTo Label0
						End If
						GoTo Label2
				End Select
			End If
			Return syntaxToken
		Label0:
			If (fullWidth) Then
				GoTo Label2
			End If
			syntaxToken = Me.ScanNewlineAsStatementTerminator(ch, precedingTrivia)
			Return syntaxToken
		Label1:
			If (Not fullWidth AndAlso Me.CanGet(1) AndAlso SyntaxFacts.IsDoubleQuote(Me.Peek(1))) Then
				syntaxToken = Me.MakePunctuationToken(precedingTrivia, 2, SyntaxKind.DollarSignDoubleQuoteToken)
				Return syntaxToken
			End If
			If (Me.IsIdentifierStartCharacter(ch)) Then
				syntaxToken = Me.ScanIdentifierOrKeyword(precedingTrivia)
				Return syntaxToken
			ElseIf (fullWidth) Then
				syntaxToken = Nothing
				Return syntaxToken
			ElseIf (SyntaxFacts.IsDoubleQuote(ch)) Then
				syntaxToken = Me.ScanStringLiteral(precedingTrivia)
				Return syntaxToken
			ElseIf (Not SyntaxFacts.IsFullWidth(ch)) Then
				syntaxToken = Nothing
				Return syntaxToken
			Else
				ch = SyntaxFacts.MakeHalfWidth(ch)
				syntaxToken = Me.ScanTokenFullWidth(precedingTrivia, ch)
				Return syntaxToken
			End If
		End Function

		Private Function ScanTokenFullWidth(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal ch As Char) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Return Me.ScanTokenCommon(precedingTrivia, ch, True)
		End Function

		Private Function ScanWhitespace(Optional ByVal len As Integer = 0) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			len = Me.GetWhitespaceLength(len)
			If (len <= 0) Then
				visualBasicSyntaxNode = Nothing
			Else
				visualBasicSyntaxNode = Me.MakeWhiteSpaceTrivia(Me.GetText(len))
			End If
			Return visualBasicSyntaxNode
		End Function

		Private Sub ScanWhitespaceAndLineContinuations(ByVal tList As SyntaxListBuilder)
			If (Me.CanGet() AndAlso SyntaxFacts.IsWhitespace(Me.Peek())) Then
				tList.Add(Me.ScanWhitespace(1))
				While Me.ScanLineContinuation(tList)
				End While
			End If
		End Sub

		Friend Function ScanXmlCData() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
			If (Me.IsScanningXmlDoc AndAlso Me.IsAtNewLine()) Then
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Me.ScanXmlDocTrivia()
				If (visualBasicSyntaxNode Is Nothing) Then
					syntaxToken = Me.MakeEofToken()
					Return syntaxToken
				End If
				syntaxList = visualBasicSyntaxNode
			End If
			Dim scratch As StringBuilder = Me.GetScratch()
			Dim length As Integer = 0
			While Me.CanGet(length)
				Dim chr As Char = Me.Peek(length)
				If (chr = Strings.ChrW(10) OrElse chr = Strings.ChrW(13)) Then
					length = Me.SkipLineBreak(chr, length)
					scratch.Append(Strings.ChrW(10))
					syntaxToken = Me.XmlMakeCDataToken(syntaxList, length, scratch)
					Return syntaxToken
				Else
					If (chr = "]"C) Then
						If (Me.NextAre(length + 1, "]>")) Then
							If (length = 0) Then
								syntaxToken = Me.XmlMakeEndCDataToken(syntaxList)
								Return syntaxToken
							Else
								syntaxToken = Me.XmlMakeCDataToken(syntaxList, length, scratch)
								Return syntaxToken
							End If
						End If
					End If
					Dim xmlCharResult As Scanner.XmlCharResult = Me.ScanXmlChar(length)
					If (xmlCharResult.Length <> 0) Then
						xmlCharResult.AppendTo(scratch)
						length += xmlCharResult.Length
					ElseIf (length <= 0) Then
						syntaxToken = Me.XmlMakeBadToken(syntaxList, 1, ERRID.ERR_IllegalChar)
						Return syntaxToken
					Else
						syntaxToken = Me.XmlMakeCDataToken(syntaxList, length, scratch)
						Return syntaxToken
					End If
				End If
			End While
			GoTo Label2
			Return syntaxToken
		Label2:
			If (length <= 0) Then
				syntaxToken = Scanner.MakeEofToken(syntaxList)
				Return syntaxToken
			Else
				syntaxToken = Me.XmlMakeCDataToken(syntaxList, length, scratch)
				Return syntaxToken
			End If
		End Function

		Private Function ScanXmlChar(ByVal Here As Integer) As Scanner.XmlCharResult
			Dim xmlCharResult As Scanner.XmlCharResult
			Dim chr As Char = Me.Peek(Here)
			If (XmlCharacterGlobalHelpers.isValidUtf16(chr)) Then
				xmlCharResult = If(SyntaxFacts.IsSurrogate(chr), Me.ScanSurrogatePair(chr, Here), New Scanner.XmlCharResult(chr))
			Else
				xmlCharResult = New Scanner.XmlCharResult()
			End If
			Return xmlCharResult
		End Function

		Private Function ScanXmlCharRef(ByRef index As Integer) As Scanner.XmlCharResult
			Dim xmlCharResult As Scanner.XmlCharResult
			If (Me.CanGet(index)) Then
				Dim stringBuilder As System.Text.StringBuilder = New System.Text.StringBuilder()
				Dim num As Integer = index
				Dim chr As Char = Me.Peek(num)
				If (chr <> "x"C) Then
					While Me.CanGet(num)
						chr = Me.Peek(num)
						If (Not XmlCharType.IsDigit(chr)) Then
							Exit While
						End If
						stringBuilder.Append(chr)
						num = num + 1
					End While
					If (stringBuilder.Length <= 0) Then
						GoTo Label1
					End If
					Dim uTF16 As Scanner.XmlCharResult = XmlCharacterGlobalHelpers.DecToUTF16(stringBuilder)
					If (uTF16.Length <> 0) Then
						index = num
					End If
					xmlCharResult = uTF16
					Return xmlCharResult
				Else
					num = num + 1
					While Me.CanGet(num)
						chr = Me.Peek(num)
						If (Not XmlCharType.IsHexDigit(chr)) Then
							Exit While
						End If
						stringBuilder.Append(chr)
						num = num + 1
					End While
					If (stringBuilder.Length <= 0) Then
						GoTo Label1
					End If
					Dim uTF161 As Scanner.XmlCharResult = XmlCharacterGlobalHelpers.HexToUTF16(stringBuilder)
					If (uTF161.Length <> 0) Then
						index = num
					End If
					xmlCharResult = uTF161
					Return xmlCharResult
				End If
			Label1:
				xmlCharResult = New Scanner.XmlCharResult()
			Else
				xmlCharResult = New Scanner.XmlCharResult()
			End If
			Return xmlCharResult
		End Function

		Friend Function ScanXmlComment() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
			If (Me.IsScanningXmlDoc AndAlso Me.IsAtNewLine()) Then
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Me.ScanXmlDocTrivia()
				If (visualBasicSyntaxNode Is Nothing) Then
					syntaxToken = Me.MakeEofToken()
					Return syntaxToken
				End If
				syntaxList = visualBasicSyntaxNode
			End If
			Dim length As Integer = 0
			While Me.CanGet(length)
				Dim chr As Char = Me.Peek(length)
				If (chr = Strings.ChrW(10) OrElse chr = Strings.ChrW(13)) Then
					syntaxToken = Me.XmlMakeCommentToken(syntaxList, length + Me.LengthOfLineBreak(chr, length))
					Return syntaxToken
				Else
					If (chr = "-"C) Then
						If (Me.NextIs(length + 1, "-"C)) Then
							If (length > 0) Then
								syntaxToken = Me.XmlMakeCommentToken(syntaxList, length)
								Return syntaxToken
							ElseIf (Me.CanGet(length + 2)) Then
								chr = Me.Peek(length + 2)
								length += 2
								If (chr = ">"C) Then
									syntaxToken = Me.XmlMakeEndCommentToken(syntaxList)
									Return syntaxToken
								Else
									syntaxToken = Me.XmlMakeCommentToken(syntaxList, 2)
									Return syntaxToken
								End If
							End If
						End If
					End If
					Dim xmlCharResult As Scanner.XmlCharResult = Me.ScanXmlChar(length)
					If (xmlCharResult.Length <> 0) Then
						length += xmlCharResult.Length
					ElseIf (length <= 0) Then
						syntaxToken = Me.XmlMakeBadToken(syntaxList, 1, ERRID.ERR_IllegalChar)
						Return syntaxToken
					Else
						syntaxToken = Me.XmlMakeCommentToken(syntaxList, length)
						Return syntaxToken
					End If
				End If
			End While
			GoTo Label2
			Return syntaxToken
		Label2:
			If (length <= 0) Then
				syntaxToken = Scanner.MakeEofToken(syntaxList)
				Return syntaxToken
			Else
				syntaxToken = Me.XmlMakeCommentToken(syntaxList, length)
				Return syntaxToken
			End If
		End Function

		Friend Function ScanXmlContent() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			If (Not Me.IsScanningXmlDoc) Then
				Dim length As Integer = 0
				Dim flag As Boolean = True
				If (Me._lineBufferOffset > 0) Then
					Dim chr As Char = Me.Peek(-1)
					If (chr <> ">"C AndAlso Not XmlCharType.IsWhiteSpace(chr)) Then
						flag = False
					End If
				End If
				Dim scratch As StringBuilder = Me.GetScratch()
				While Me.CanGet(length)
					Dim chr1 As Char = Me.Peek(length)
					If (chr1 <= "&"C) Then
						Select Case chr1
							Case Strings.ChrW(9)
							Label1:
								scratch.Append(chr1)
								length = length + 1
								Continue While
							Case Strings.ChrW(10)
							Case Strings.ChrW(13)
								length = Me.SkipLineBreak(chr1, length)
								scratch.Append(Strings.ChrW(10))
								Continue While
							Case Strings.ChrW(11)
							Case Strings.ChrW(12)
								Exit Select
							Case Else
								Select Case chr1
									Case Strings.ChrW(32)
										GoTo Label1
									Case "&"C
										If (length = 0) Then
											syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
											syntaxToken = Me.ScanXmlReference(syntaxList)
											Return syntaxToken
										Else
											syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
											syntaxToken = Me.XmlMakeTextLiteralToken(syntaxList, length, scratch)
											Return syntaxToken
										End If
								End Select

						End Select
					ElseIf (chr1 = "<"C) Then
						Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
						If (length <> 0) Then
							If (flag) Then
								scratch.Clear()
								length = 0
								syntaxList1 = Me.ScanXmlTrivia(Me.Peek())
							Else
								syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
								syntaxToken = Me.XmlMakeTextLiteralToken(syntaxList, length, scratch)
								Return syntaxToken
							End If
						End If
						If (Me.CanGet(1)) Then
							Dim chr2 As Char = Me.Peek(1)
							If (chr2 <= "%"C) Then
								If (chr2 <> "!"C) Then
									If (chr2 = "%"C) Then
										If (Me.NextIs(2, "="C)) Then
											syntaxToken = Me.XmlMakeBeginEmbeddedToken(syntaxList1)
											Return syntaxToken
										End If
									End If
								ElseIf (Me.CanGet(2)) Then
									Dim chr3 As Char = Me.Peek(2)
									If (chr3 = "-"C) Then
										If (Me.NextIs(3, "-"C)) Then
											syntaxToken = Me.XmlMakeBeginCommentToken(syntaxList1, Scanner.s_scanNoTriviaFunc)
											Return syntaxToken
										End If
									ElseIf (chr3 <> "D"C) Then
										If (chr3 = "["C) Then
											If (Me.NextAre(3, "CDATA[")) Then
												syntaxToken = Me.XmlMakeBeginCDataToken(syntaxList1, Scanner.s_scanNoTriviaFunc)
												Return syntaxToken
											End If
										End If
									ElseIf (Me.NextAre(3, "OCTYPE")) Then
										syntaxToken = Me.XmlMakeBeginDTDToken(syntaxList1)
										Return syntaxToken
									End If
								End If
							ElseIf (chr2 = "/"C) Then
								syntaxToken = Me.XmlMakeBeginEndElementToken(syntaxList1, Scanner.s_scanNoTriviaFunc)
								Return syntaxToken
							ElseIf (chr2 = "?"C) Then
								syntaxToken = Me.XmlMakeBeginProcessingInstructionToken(syntaxList1, Scanner.s_scanNoTriviaFunc)
								Return syntaxToken
							End If
						End If
						syntaxToken = Me.XmlMakeLessToken(syntaxList1)
						Return syntaxToken
					ElseIf (chr1 = "]"C) Then
						If (Me.NextAre(length + 1, "]>")) Then
							If (length = 0) Then
								syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
								syntaxToken = Me.XmlMakeTextLiteralToken(syntaxList, 3, ERRID.ERR_XmlEndCDataNotAllowedInContent)
								Return syntaxToken
							Else
								syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
								syntaxToken = Me.XmlMakeTextLiteralToken(syntaxList, length, scratch)
								Return syntaxToken
							End If
						End If
					End If
					flag = False
					Dim xmlCharResult As Scanner.XmlCharResult = Me.ScanXmlChar(length)
					If (xmlCharResult.Length <> 0) Then
						xmlCharResult.AppendTo(scratch)
						length += xmlCharResult.Length
					ElseIf (length <= 0) Then
						syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
						syntaxToken = Me.XmlMakeBadToken(syntaxList, 1, ERRID.ERR_IllegalChar)
						Return syntaxToken
					Else
						syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
						syntaxToken = Me.XmlMakeTextLiteralToken(syntaxList, length, scratch)
						Return syntaxToken
					End If
				End While
				If (length <= 0) Then
					syntaxToken = Me.MakeEofToken()
				Else
					syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
					syntaxToken = Me.XmlMakeTextLiteralToken(syntaxList, length, scratch)
				End If
			Else
				syntaxToken = Me.ScanXmlContentInXmlDoc()
			End If
			Return syntaxToken
		End Function

		Private Function ScanXmlContentInXmlDoc() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
			If (Me.IsAtNewLine() OrElse Me._isStartingFirstXmlDocLine) Then
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Me.ScanXmlDocTrivia()
				Me._isStartingFirstXmlDocLine = False
				If (visualBasicSyntaxNode Is Nothing) Then
					syntaxToken = Me.MakeEofToken()
					Return syntaxToken
				End If
				syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(visualBasicSyntaxNode)
			End If
			Dim length As Integer = 0
			Dim scratch As StringBuilder = Me.GetScratch()
			While Me.CanGet(length)
				Dim chr As Char = Me.Peek(length)
				If (chr <= Strings.ChrW(32)) Then
					Select Case chr
						Case Strings.ChrW(9)
						Label3:
							scratch.Append(chr)
							length = length + 1
							Continue While
						Case Strings.ChrW(10)
						Case Strings.ChrW(13)
							If (length = 0) Then
								length = Me.SkipLineBreak(chr, length)
								If (Me._endOfXmlInsteadOfLastDocCommentLineBreak) Then
									Dim num As Integer = length
									If (Not Me.TrySkipXmlDocMarker(num)) Then
										Me.ResetLineBufferOffset()
										syntaxToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Token(Nothing, SyntaxKind.EndOfXmlToken, Nothing, [String].Empty)
										Return syntaxToken
									End If
								End If
								syntaxToken = Me.MakeDocCommentLineBreakToken(syntaxList, length)
								Return syntaxToken
							Else
								syntaxToken = Me.XmlMakeTextLiteralToken(syntaxList, length, scratch)
								Return syntaxToken
							End If
						Case Strings.ChrW(11)
						Case Strings.ChrW(12)
							Exit Select
						Case Else
							If (chr = Strings.ChrW(32)) Then
								GoTo Label3
							End If
							Exit Select
					End Select
				ElseIf (chr = "&"C) Then
					If (length = 0) Then
						syntaxToken = Me.ScanXmlReference(syntaxList)
						Return syntaxToken
					Else
						syntaxToken = Me.XmlMakeTextLiteralToken(syntaxList, length, scratch)
						Return syntaxToken
					End If
				ElseIf (chr <> "<"C) Then
					If (chr = "]"C) Then
						If (Me.NextAre(length + 1, "]>")) Then
							If (length = 0) Then
								syntaxToken = Me.XmlMakeTextLiteralToken(syntaxList, 3, ERRID.ERR_XmlEndCDataNotAllowedInContent)
								Return syntaxToken
							Else
								syntaxToken = Me.XmlMakeTextLiteralToken(syntaxList, length, scratch)
								Return syntaxToken
							End If
						End If
					End If
				ElseIf (length = 0) Then
					If (Me.CanGet(1)) Then
						Dim chr1 As Char = Me.Peek(1)
						If (chr1 = "!"C) Then
							If (Me.CanGet(2)) Then
								Dim chr2 As Char = Me.Peek(2)
								If (chr2 = "-"C) Then
									If (Me.NextIs(3, "-"C)) Then
										syntaxToken = Me.XmlMakeBeginCommentToken(syntaxList, Scanner.s_scanNoTriviaFunc)
										Return syntaxToken
									End If
								ElseIf (chr2 <> "D"C) Then
									If (chr2 = "["C) Then
										If (Me.NextAre(3, "CDATA[")) Then
											syntaxToken = Me.XmlMakeBeginCDataToken(syntaxList, Scanner.s_scanNoTriviaFunc)
											Return syntaxToken
										End If
									End If
								ElseIf (Me.NextAre(3, "OCTYPE")) Then
									syntaxToken = Me.XmlMakeBeginDTDToken(syntaxList)
									Return syntaxToken
								End If
							End If
						ElseIf (chr1 = "/"C) Then
							syntaxToken = Me.XmlMakeBeginEndElementToken(syntaxList, Scanner.s_scanNoTriviaFunc)
							Return syntaxToken
						ElseIf (chr1 = "?"C) Then
							syntaxToken = Me.XmlMakeBeginProcessingInstructionToken(syntaxList, Scanner.s_scanNoTriviaFunc)
							Return syntaxToken
						End If
					End If
					syntaxToken = Me.XmlMakeLessToken(syntaxList)
					Return syntaxToken
				Else
					syntaxToken = Me.XmlMakeTextLiteralToken(syntaxList, length, scratch)
					Return syntaxToken
				End If
				Dim xmlCharResult As Scanner.XmlCharResult = Me.ScanXmlChar(length)
				If (xmlCharResult.Length <> 0) Then
					xmlCharResult.AppendTo(scratch)
					length += xmlCharResult.Length
				ElseIf (length <= 0) Then
					syntaxToken = Me.XmlMakeBadToken(syntaxList, 1, ERRID.ERR_IllegalChar)
					Return syntaxToken
				Else
					syntaxToken = Me.XmlMakeTextLiteralToken(syntaxList, length, scratch)
					Return syntaxToken
				End If
			End While
			GoTo Label2
			Return syntaxToken
		Label2:
			If (length <= 0) Then
				syntaxToken = Scanner.MakeEofToken(syntaxList)
				Return syntaxToken
			Else
				syntaxToken = Me.XmlMakeTextLiteralToken(syntaxList, length, scratch)
				Return syntaxToken
			End If
		End Function

		Private Function ScanXmlDocTrivia() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim num As Integer = 0
			If (Not Me.TrySkipXmlDocMarker(num)) Then
				visualBasicSyntaxNode = Nothing
			Else
				visualBasicSyntaxNode = Me.MakeDocumentationCommentExteriorTrivia(Me.GetText(num))
			End If
			Return visualBasicSyntaxNode
		End Function

		Friend Function ScanXmlElement(Optional state As ScannerState = ScannerState.Element) As SyntaxToken
			Debug.Assert(state = ScannerState.Element OrElse state = ScannerState.EndElement OrElse state = ScannerState.DocType)

			' SHIM
			If IsScanningXmlDoc Then
				Return ScanXmlElementInXmlDoc(state)
			End If

			' // Only legal tokens
			' //  QName
			' //  /
			' //  >
			' //  =
			' //  Whitespace

			Dim leadingTrivia As CoreInternalSyntax.SyntaxList(Of VisualBasicSyntaxNode) = Nothing

			While CanGet()
				Dim c As Char = Peek()

				Select Case (c)
					' // Whitespace
					' //  S    ::=    (#x20 | #x9 | #xD | #xA)+
					Case CARRIAGE_RETURN, LINE_FEED
						' we should not visit this place twice
						Debug.Assert(leadingTrivia.Node Is Nothing)

						Dim offsets = CreateOffsetRestorePoint()
						leadingTrivia = ScanXmlTrivia(c)

						If ScanXmlForPossibleStatement(state) Then
							offsets.Restore()
							Return SyntaxFactory.Token(Nothing, SyntaxKind.EndOfXmlToken, Nothing, String.Empty)
						End If

					Case " "c, CHARACTER_TABULATION
						' we should not visit this place twice
						Debug.Assert(leadingTrivia.Node Is Nothing)
						leadingTrivia = ScanXmlTrivia(c)

					Case "/"c
						If CanGet(1) AndAlso Peek(1) = ">" Then
							Return XmlMakeEndEmptyElementToken(leadingTrivia)
						End If
						Return XmlMakeDivToken(leadingTrivia)

					Case ">"c
						' TODO: this will not consume trailing trivia
						' consider cases where this is the last element in the literal.
						Return XmlMakeGreaterToken(leadingTrivia)

					Case "="c
						Return XmlMakeEqualsToken(leadingTrivia)

					Case "'"c, LEFT_SINGLE_QUOTATION_MARK, RIGHT_SINGLE_QUOTATION_MARK
						Return XmlMakeSingleQuoteToken(leadingTrivia, c, isOpening:=True)

					Case """"c, LEFT_DOUBLE_QUOTATION_MARK, RIGHT_DOUBLE_QUOTATION_MARK
						Return XmlMakeDoubleQuoteToken(leadingTrivia, c, isOpening:=True)

					Case "<"c
						If CanGet(1) Then
							Dim ch As Char = Peek(1)
							Select Case ch
								Case "!"c
									If CanGet(2) Then
										Select Case (Peek(2))
											Case "-"c
												If NextIs(3, "-"c) Then
													Return XmlMakeBeginCommentToken(leadingTrivia, s_scanNoTriviaFunc)
												End If
											Case "["c
												If NextAre(3, "CDATA[") Then
													Return XmlMakeBeginCDataToken(leadingTrivia, s_scanNoTriviaFunc)
												End If
											Case "D"c
												If NextAre(3, "OCTYPE") Then
													Return XmlMakeBeginDTDToken(leadingTrivia)
												End If
										End Select
									End If
									Return XmlLessThanExclamationToken(state, leadingTrivia)
								Case "%"c
									If NextIs(2, "="c) Then
										Return XmlMakeBeginEmbeddedToken(leadingTrivia)
									End If
								Case "?"c
									Return XmlMakeBeginProcessingInstructionToken(leadingTrivia, s_scanNoTriviaFunc)
								Case "/"c
									Return XmlMakeBeginEndElementToken(leadingTrivia, s_scanNoTriviaFunc)
							End Select
						End If

						Return XmlMakeLessToken(leadingTrivia)

					Case "?"c

						If NextIs(1, ">"c) Then
							' // Create token for the '?>' termination sequence
							Return XmlMakeEndProcessingInstructionToken(leadingTrivia)
						End If

						Return XmlMakeBadToken(leadingTrivia, 1, ERRID.ERR_IllegalXmlNameChar)

					Case "("c
						Return XmlMakeLeftParenToken(leadingTrivia)

					Case ")"c
						Return XmlMakeRightParenToken(leadingTrivia)

					Case "!"c, ";"c, "#"c, ","c, "}"c
						Return XmlMakeBadToken(leadingTrivia, 1, ERRID.ERR_IllegalXmlNameChar)

					Case ":"c
						Return XmlMakeColonToken(leadingTrivia)

					Case "["c
						Return XmlMakeOpenBracketToken(state, leadingTrivia)

					Case "]"c
						Return XmlMakeCloseBracketToken(state, leadingTrivia)

					Case Else
						' // Because of weak scanning of QName, this state must always handle
						' //    '=' | '\'' | '"'| '/' | '>' | '<' | '?'

						Return ScanXmlNcName(leadingTrivia)

				End Select
			End While
			Return MakeEofToken(leadingTrivia)
		End Function

		Private Function ScanXmlElementInXmlDoc(ByVal state As ScannerState) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim lineBufferAndEndOfTerminatorOffset As Scanner.LineBufferAndEndOfTerminatorOffsets
			Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			Dim flag As Boolean
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
			If (Me.IsAtNewLine() AndAlso Not Me._doNotRequireXmlDocCommentPrefix) Then
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Me.ScanXmlDocTrivia()
				If (visualBasicSyntaxNode Is Nothing) Then
					syntaxToken = Me.MakeEofToken()
					Return syntaxToken
				End If
				syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(visualBasicSyntaxNode)
			End If
			While True
				If (Not Me.CanGet()) Then
					syntaxToken = Scanner.MakeEofToken(syntaxList)
					Exit While
				ElseIf (syntaxList.Any() OrElse Not Me.IsAtNewLine() OrElse Me._doNotRequireXmlDocCommentPrefix) Then
					Dim chr As Char = Me.Peek()
					If (chr <= "?"C) Then
						Select Case chr
							Case Strings.ChrW(9)
							Case Strings.ChrW(10)
							Case Strings.ChrW(13)
								lineBufferAndEndOfTerminatorOffset = Me.CreateOffsetRestorePoint()
								syntaxListBuilder = Me._triviaListPool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
								flag = Me.ScanXmlTriviaInXmlDoc(chr, syntaxListBuilder)
								syntaxList = syntaxListBuilder.ToList()
								Me._triviaListPool.Free(syntaxListBuilder)
								If (flag) Then
									Continue While
								End If
								lineBufferAndEndOfTerminatorOffset.Restore()
								syntaxToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Token(syntaxList.Node, SyntaxKind.EndOfXmlToken, Nothing, [String].Empty)
								Return syntaxToken
							Case Strings.ChrW(11)
							Case Strings.ChrW(12)
								Exit Select
							Case Else
								Select Case chr
									Case Strings.ChrW(32)
										lineBufferAndEndOfTerminatorOffset = Me.CreateOffsetRestorePoint()
										syntaxListBuilder = Me._triviaListPool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
										flag = Me.ScanXmlTriviaInXmlDoc(chr, syntaxListBuilder)
										syntaxList = syntaxListBuilder.ToList()
										Me._triviaListPool.Free(syntaxListBuilder)
										If (flag) Then
											Continue While
										End If
										lineBufferAndEndOfTerminatorOffset.Restore()
										syntaxToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Token(syntaxList.Node, SyntaxKind.EndOfXmlToken, Nothing, [String].Empty)
										Return syntaxToken
									Case "!"C
									Case "#"C
									Case ","C
										syntaxToken = Me.XmlMakeBadToken(syntaxList, 1, ERRID.ERR_IllegalXmlNameChar)
										Return syntaxToken
									Case """"C
										syntaxToken = Me.XmlMakeDoubleQuoteToken(syntaxList, chr, True)
										Return syntaxToken
									Case "$"C
									Case "%"C
									Case "&"C
									Case "*"C
									Case "+"C
									Case "-"C
									Case "."C
										Exit Select
									Case "'"C
										syntaxToken = Me.XmlMakeSingleQuoteToken(syntaxList, chr, True)
										Return syntaxToken
									Case "("C
										syntaxToken = Me.XmlMakeLeftParenToken(syntaxList)
										Return syntaxToken
									Case ")"C
										syntaxToken = Me.XmlMakeRightParenToken(syntaxList)
										Return syntaxToken
									Case "/"C
										If (Not Me.NextIs(1, ">"C)) Then
											syntaxToken = Me.XmlMakeDivToken(syntaxList)
											Return syntaxToken
										Else
											syntaxToken = Me.XmlMakeEndEmptyElementToken(syntaxList)
											Return syntaxToken
										End If
									Case Else
										Select Case chr
											Case ":"C
												syntaxToken = Me.XmlMakeColonToken(syntaxList)
												Return syntaxToken
											Case ";"C
												syntaxToken = Me.XmlMakeBadToken(syntaxList, 1, ERRID.ERR_IllegalXmlNameChar)
												Return syntaxToken
											Case "<"C
												If (Me.CanGet(1)) Then
													Dim chr1 As Char = Me.Peek(1)
													If (chr1 = "!"C) Then
														If (Me.CanGet(2)) Then
															Dim chr2 As Char = Me.Peek(2)
															If (chr2 = "-"C) Then
																If (Me.NextIs(3, "-"C)) Then
																	syntaxToken = Me.XmlMakeBeginCommentToken(syntaxList, Scanner.s_scanNoTriviaFunc)
																	Return syntaxToken
																End If
															ElseIf (chr2 <> "D"C) Then
																If (chr2 = "["C) Then
																	If (Me.NextAre(3, "CDATA[")) Then
																		syntaxToken = Me.XmlMakeBeginCDataToken(syntaxList, Scanner.s_scanNoTriviaFunc)
																		Return syntaxToken
																	End If
																End If
															ElseIf (Me.NextAre(3, "OCTYPE")) Then
																syntaxToken = Me.XmlMakeBeginDTDToken(syntaxList)
																Return syntaxToken
															End If
														End If
														syntaxToken = Me.XmlLessThanExclamationToken(state, syntaxList)
														Return syntaxToken
													ElseIf (chr1 = "/"C) Then
														syntaxToken = Me.XmlMakeBeginEndElementToken(syntaxList, Scanner.s_scanNoTriviaFunc)
														Return syntaxToken
													ElseIf (chr1 = "?"C) Then
														syntaxToken = Me.XmlMakeBeginProcessingInstructionToken(syntaxList, Scanner.s_scanNoTriviaFunc)
														Return syntaxToken
													End If
												End If
												syntaxToken = Me.XmlMakeLessToken(syntaxList)
												Return syntaxToken
											Case "="C
												syntaxToken = Me.XmlMakeEqualsToken(syntaxList)
												Return syntaxToken
											Case ">"C
												syntaxToken = Me.XmlMakeGreaterToken(syntaxList)
												Return syntaxToken
											Case "?"C
												If (Not Me.NextIs(1, ">"C)) Then
													syntaxToken = Me.MakeQuestionToken(syntaxList, False)
													Return syntaxToken
												Else
													syntaxToken = Me.XmlMakeEndProcessingInstructionToken(syntaxList)
													Return syntaxToken
												End If
										End Select

								End Select

						End Select
					ElseIf (chr > "]"C) Then
						If (chr = "}"C) Then
							syntaxToken = Me.XmlMakeBadToken(syntaxList, 1, ERRID.ERR_IllegalXmlNameChar)
							Return syntaxToken
						End If
						Select Case chr
							Case "‘"C
							Case "’"C
								syntaxToken = Me.XmlMakeSingleQuoteToken(syntaxList, chr, True)
								Return syntaxToken
							Case "“"C
							Case "”"C
								syntaxToken = Me.XmlMakeDoubleQuoteToken(syntaxList, chr, True)
								Return syntaxToken
						End Select
					ElseIf (chr = "["C) Then
						syntaxToken = Me.XmlMakeOpenBracketToken(state, syntaxList)
						Exit While
					ElseIf (chr = "]"C) Then
						syntaxToken = Me.XmlMakeCloseBracketToken(state, syntaxList)
						Exit While
					End If
					syntaxToken = Me.ScanXmlNcName(syntaxList)
					Exit While
				Else
					syntaxToken = Scanner.MakeEofToken(syntaxList)
					Exit While
				End If
			End While
			Return syntaxToken
		End Function

		Private Function ScanXmlForPossibleStatement(ByVal state As ScannerState) As Boolean
			Dim flag As Boolean
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim kind As Boolean
			Dim chr As Char
			If (Me.CanGet()) Then
				kind = False
				Dim lineBufferAndEndOfTerminatorOffset As Scanner.LineBufferAndEndOfTerminatorOffsets = Me.CreateOffsetRestorePoint()
				chr = Me.Peek()
				If (chr > "<"C) Then
					If (chr = "＃"C) Then
						GoTo Label0
					End If
					If (chr = "＜"C) Then
						GoTo Label1
					End If
				Else
					If (chr = "#"C) Then
						GoTo Label0
					End If
					If (chr = "<"C) Then
						GoTo Label1
					End If
				End If
				If (Not SyntaxFacts.IsSingleQuote(chr) OrElse Me.LastToken.Kind = SyntaxKind.EqualsToken) Then
					syntaxToken = Me.ScanXmlNcName(Me.ScanSingleLineTrivia())
					Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = TryCast(syntaxToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)
					If (xmlNameTokenSyntax IsNot Nothing AndAlso Not syntaxToken.IsMissing) Then
						If (state <> ScannerState.EndElement) Then
							syntaxToken = Me.ScanNextToken(False)
							If (xmlNameTokenSyntax.PossibleKeywordKind <> SyntaxKind.XmlNameToken) Then
								kind = If(syntaxToken.Kind = SyntaxKind.IdentifierToken, True, syntaxToken.IsKeyword)
							Else
								kind = syntaxToken.Kind = SyntaxKind.OpenParenToken
							End If
						Else
							kind = If(syntaxToken.Kind = SyntaxKind.XmlNameToken, True, Me.LastToken.Kind = SyntaxKind.XmlNameToken)
						End If
					End If
				Else
					kind = True
				End If
			Label2:
				lineBufferAndEndOfTerminatorOffset.Restore()
				flag = kind
			Else
				flag = False
			End If
			Return flag
		Label0:
			Me.AdvanceChar(1)
			syntaxToken = Me.ScanNextToken(False)
			kind = syntaxToken.IsKeyword
			GoTo Label2
		Label1:
			Me.AdvanceChar(1)
			syntaxToken = Me.ScanXmlNcName(Me.ScanSingleLineTrivia())
			Dim xmlNameTokenSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = TryCast(syntaxToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)
			If (xmlNameTokenSyntax1 IsNot Nothing AndAlso Not xmlNameTokenSyntax1.IsMissing AndAlso xmlNameTokenSyntax1.PossibleKeywordKind <> SyntaxKind.XmlNameToken) Then
				Me.ScanSingleLineTrivia()
				chr = Me.Peek()
				kind = If(chr = "("C, True, chr = "（"C)
				GoTo Label2
			Else
				GoTo Label2
			End If
		End Function

		Friend Function ScanXmlMisc() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
			While True
				If (Me.CanGet()) Then
					Dim chr As Char = Me.Peek()
					Select Case chr
						Case Strings.ChrW(9)
						Case Strings.ChrW(10)
						Case Strings.ChrW(13)
						Label1:
							syntaxList = Me.ScanXmlTrivia(chr)
							Continue While
						Case Strings.ChrW(11)
						Case Strings.ChrW(12)
							syntaxToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Token(syntaxList.Node, SyntaxKind.EndOfXmlToken, Nothing, [String].Empty)
							Return syntaxToken
						Case Else
							If (chr = Strings.ChrW(32)) Then
								GoTo Label1
							End If
							If (chr = "<"C) Then
								If (Me.CanGet(1)) Then
									Dim chr1 As Char = Me.Peek(1)
									If (chr1 <> "!"C) Then
										If (chr1 <> "%"C) Then
											If (chr1 = "?"C) Then
												syntaxToken = Me.XmlMakeBeginProcessingInstructionToken(syntaxList, Scanner.s_scanNoTriviaFunc)
												Return syntaxToken
											End If
										ElseIf (Me.NextIs(2, "="C)) Then
											syntaxToken = Me.XmlMakeBeginEmbeddedToken(syntaxList)
											Return syntaxToken
										End If
									ElseIf (Me.NextAre(2, "--")) Then
										syntaxToken = Me.XmlMakeBeginCommentToken(syntaxList, Scanner.s_scanNoTriviaFunc)
										Return syntaxToken
									ElseIf (Me.NextAre(2, "DOCTYPE")) Then
										syntaxToken = Me.XmlMakeBeginDTDToken(syntaxList)
										Return syntaxToken
									End If
								End If
								syntaxToken = Me.XmlMakeLessToken(syntaxList)
							Else
								syntaxToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Token(syntaxList.Node, SyntaxKind.EndOfXmlToken, Nothing, [String].Empty)
								Return syntaxToken
							End If

					End Select
				Else
					syntaxToken = Scanner.MakeEofToken(syntaxList)
					Exit While
				End If
			End While
			Return syntaxToken
		End Function

		Private Function ScanXmlNcName(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax
			Dim objArray As [Object]()
			Dim diagnosticInfoArray As DiagnosticInfo()
			Dim length As Integer = 0
			Dim flag As Boolean = False
			Dim flag1 As Boolean = True
			Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None
			Dim num As Integer = 0
			Dim str As String = Nothing
			While Me.CanGet(length)
				Dim chr As Char = Me.Peek(length)
				If (chr > """"C) Then
					Select Case chr
						Case "'"C
						Case "("C
						Case ")"C
						Case ","C
						Case "/"C
							If (length <> 0) Then
								xmlNameTokenSyntax = Me.XmlMakeXmlNCNameToken(precedingTrivia, length)
								If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
									ReDim diagnosticInfoArray(0)
									objArray = New [Object]() { str, [String].Format("&H{0:X}", num) }
									diagnosticInfoArray(0) = ErrorFactory.ErrorInfo(eRRID, objArray)
									xmlNameTokenSyntax = xmlNameTokenSyntax.WithDiagnostics(diagnosticInfoArray)
								End If
								syntaxToken = xmlNameTokenSyntax
							ElseIf (Not flag) Then
								syntaxToken = Scanner.MakeMissingToken(precedingTrivia, SyntaxKind.XmlNameToken)
							Else
								syntaxToken = Me.XmlMakeBadToken(precedingTrivia, 1, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalChar)
							End If
							Return syntaxToken
						Case "*"C
						Case "+"C
						Case "-"C
						Case "."C
							Exit Select
						Case Else
							Select Case chr
								Case ":"C
								Case ";"C
								Case "<"C
								Case "="C
								Case ">"C
								Case "?"C
									If (length <> 0) Then
										xmlNameTokenSyntax = Me.XmlMakeXmlNCNameToken(precedingTrivia, length)
										If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
											ReDim diagnosticInfoArray(0)
											objArray = New [Object]() { str, [String].Format("&H{0:X}", num) }
											diagnosticInfoArray(0) = ErrorFactory.ErrorInfo(eRRID, objArray)
											xmlNameTokenSyntax = xmlNameTokenSyntax.WithDiagnostics(diagnosticInfoArray)
										End If
										syntaxToken = xmlNameTokenSyntax
									ElseIf (Not flag) Then
										syntaxToken = Scanner.MakeMissingToken(precedingTrivia, SyntaxKind.XmlNameToken)
									Else
										syntaxToken = Me.XmlMakeBadToken(precedingTrivia, 1, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalChar)
									End If
									Return syntaxToken
								Case Else
									If (chr = "}"C) Then
										If (length <> 0) Then
											xmlNameTokenSyntax = Me.XmlMakeXmlNCNameToken(precedingTrivia, length)
											If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
												ReDim diagnosticInfoArray(0)
												objArray = New [Object]() { str, [String].Format("&H{0:X}", num) }
												diagnosticInfoArray(0) = ErrorFactory.ErrorInfo(eRRID, objArray)
												xmlNameTokenSyntax = xmlNameTokenSyntax.WithDiagnostics(diagnosticInfoArray)
											End If
											syntaxToken = xmlNameTokenSyntax
										ElseIf (Not flag) Then
											syntaxToken = Scanner.MakeMissingToken(precedingTrivia, SyntaxKind.XmlNameToken)
										Else
											syntaxToken = Me.XmlMakeBadToken(precedingTrivia, 1, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalChar)
										End If
										Return syntaxToken
									End If

							End Select

					End Select
				Else
					Select Case chr
						Case Strings.ChrW(9)
						Case Strings.ChrW(10)
						Case Strings.ChrW(13)
							If (length <> 0) Then
								xmlNameTokenSyntax = Me.XmlMakeXmlNCNameToken(precedingTrivia, length)
								If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
									ReDim diagnosticInfoArray(0)
									objArray = New [Object]() { str, [String].Format("&H{0:X}", num) }
									diagnosticInfoArray(0) = ErrorFactory.ErrorInfo(eRRID, objArray)
									xmlNameTokenSyntax = xmlNameTokenSyntax.WithDiagnostics(diagnosticInfoArray)
								End If
								syntaxToken = xmlNameTokenSyntax
							ElseIf (Not flag) Then
								syntaxToken = Scanner.MakeMissingToken(precedingTrivia, SyntaxKind.XmlNameToken)
							Else
								syntaxToken = Me.XmlMakeBadToken(precedingTrivia, 1, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalChar)
							End If
							Return syntaxToken
						Case Strings.ChrW(11)
						Case Strings.ChrW(12)
							Exit Select
						Case Else
							If (chr = Strings.ChrW(32) OrElse chr = """"C) Then
								If (length <> 0) Then
									xmlNameTokenSyntax = Me.XmlMakeXmlNCNameToken(precedingTrivia, length)
									If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
										ReDim diagnosticInfoArray(0)
										objArray = New [Object]() { str, [String].Format("&H{0:X}", num) }
										diagnosticInfoArray(0) = ErrorFactory.ErrorInfo(eRRID, objArray)
										xmlNameTokenSyntax = xmlNameTokenSyntax.WithDiagnostics(diagnosticInfoArray)
									End If
									syntaxToken = xmlNameTokenSyntax
								ElseIf (Not flag) Then
									syntaxToken = Scanner.MakeMissingToken(precedingTrivia, SyntaxKind.XmlNameToken)
								Else
									syntaxToken = Me.XmlMakeBadToken(precedingTrivia, 1, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalChar)
								End If
								Return syntaxToken
							End If
							Exit Select
					End Select
				End If
				Dim xmlCharResult As Scanner.XmlCharResult = Me.ScanXmlChar(length)
				If (xmlCharResult.Length <> 0) Then
					If (eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
						If (xmlCharResult.Length <> 1) Then
							Dim unicode As Integer = XmlCharacterGlobalHelpers.UTF16ToUnicode(xmlCharResult)
							If (unicode < 65536 OrElse unicode > 983039) Then
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalXmlNameChar
								str = New [String](New [Char]() { xmlCharResult.Char1, xmlCharResult.Char2 })
								num = unicode
							End If
						Else
							If (Not flag1) Then
								eRRID = If(Not XmlCharacterGlobalHelpers.isNameChar(xmlCharResult.Char1), Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalXmlNameChar, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None)
							Else
								eRRID = If(Not XmlCharacterGlobalHelpers.isStartNameChar(xmlCharResult.Char1), Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalXmlStartNameChar, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None)
								flag1 = False
							End If
							If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
								str = Convert.ToString(xmlCharResult.Char1)
								num = Convert.ToInt32(xmlCharResult.Char1)
							End If
						End If
					End If
					length += xmlCharResult.Length
				Else
					flag = True
					Exit While
				End If
			End While
			If (length <> 0) Then
				xmlNameTokenSyntax = Me.XmlMakeXmlNCNameToken(precedingTrivia, length)
				If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
					ReDim diagnosticInfoArray(0)
					objArray = New [Object]() { str, [String].Format("&H{0:X}", num) }
					diagnosticInfoArray(0) = ErrorFactory.ErrorInfo(eRRID, objArray)
					xmlNameTokenSyntax = xmlNameTokenSyntax.WithDiagnostics(diagnosticInfoArray)
				End If
				syntaxToken = xmlNameTokenSyntax
			ElseIf (Not flag) Then
				syntaxToken = Scanner.MakeMissingToken(precedingTrivia, SyntaxKind.XmlNameToken)
			Else
				syntaxToken = Me.XmlMakeBadToken(precedingTrivia, 1, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_IllegalChar)
			End If
			Return syntaxToken
		End Function

		Friend Function ScanXmlPIData(ByVal state As ScannerState) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			If (Not Me.IsScanningXmlDoc) Then
				Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = Me._triviaListPool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
				If (state = ScannerState.StartProcessingInstruction AndAlso Me.CanGet()) Then
					Dim chr As Char = Me.Peek()
					Select Case chr
						Case Strings.ChrW(9)
						Case Strings.ChrW(10)
						Case Strings.ChrW(13)
						Label0:
							syntaxListBuilder.AddRange(Me.ScanXmlTrivia(chr))
							Exit Select
						Case Strings.ChrW(11)
						Case Strings.ChrW(12)
							Exit Select
						Case Else
							If (chr <> Strings.ChrW(32)) Then
								Exit Select
							Else
								GoTo Label0
							End If
					End Select
				End If
				Dim length As Integer = 0
				While True
					If (Me.CanGet(length)) Then
						Dim chr1 As Char = Me.Peek(length)
						If (chr1 = Strings.ChrW(10) OrElse chr1 = Strings.ChrW(13)) Then
							syntaxToken1 = Me.XmlMakeProcessingInstructionToken(syntaxListBuilder.ToList(), length + Me.LengthOfLineBreak(chr1, length))
							Exit While
						Else
							If (chr1 = "?"C) Then
								If (Me.NextIs(length + 1, ">"C)) Then
									If (length = 0) Then
										syntaxToken1 = Me.XmlMakeEndProcessingInstructionToken(syntaxListBuilder.ToList())
										Exit While
									Else
										syntaxToken1 = Me.XmlMakeProcessingInstructionToken(syntaxListBuilder.ToList(), length)
										Exit While
									End If
								End If
							End If
							Dim xmlCharResult As Scanner.XmlCharResult = Me.ScanXmlChar(length)
							If (xmlCharResult.Length > 0) Then
								length += xmlCharResult.Length
							ElseIf (length = 0) Then
								syntaxToken1 = Me.XmlMakeBadToken(syntaxListBuilder.ToList(), 1, ERRID.ERR_IllegalChar)
								Exit While
							Else
								syntaxToken1 = Me.XmlMakeProcessingInstructionToken(syntaxListBuilder.ToList(), length)
								Exit While
							End If
						End If
					ElseIf (length <= 0) Then
						syntaxToken1 = Scanner.MakeEofToken(syntaxListBuilder.ToList())
						Exit While
					Else
						syntaxToken1 = Me.XmlMakeProcessingInstructionToken(syntaxListBuilder.ToList(), length)
						Exit While
					End If
				End While
				Me._triviaListPool.Free(syntaxListBuilder)
				syntaxToken = syntaxToken1
			Else
				syntaxToken = Me.ScanXmlPIDataInXmlDoc(state)
			End If
			Return syntaxToken
		End Function

		Friend Function ScanXmlPIDataInXmlDoc(ByVal state As ScannerState) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim lineBufferAndEndOfTerminatorOffset As Scanner.LineBufferAndEndOfTerminatorOffsets
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = Me._triviaListPool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
			If (Me.IsAtNewLine()) Then
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Me.ScanXmlDocTrivia()
				If (visualBasicSyntaxNode Is Nothing) Then
					syntaxToken = Me.MakeEofToken()
					Return syntaxToken
				End If
				syntaxListBuilder.Add(visualBasicSyntaxNode)
			End If
			If (state = ScannerState.StartProcessingInstruction AndAlso Me.CanGet()) Then
				Dim chr As Char = Me.Peek()
				Select Case chr
					Case Strings.ChrW(9)
					Case Strings.ChrW(10)
					Case Strings.ChrW(13)
						lineBufferAndEndOfTerminatorOffset = Me.CreateOffsetRestorePoint()
						If (Me.ScanXmlTriviaInXmlDoc(chr, syntaxListBuilder)) Then
							Exit Select
						End If
						lineBufferAndEndOfTerminatorOffset.Restore()
						list = syntaxListBuilder.ToList()
						syntaxToken1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Token(list.Node, SyntaxKind.EndOfXmlToken, Nothing, [String].Empty)
						Me._triviaListPool.Free(syntaxListBuilder)
						syntaxToken = syntaxToken1
						Return syntaxToken
					Case Strings.ChrW(11)
					Case Strings.ChrW(12)
						Exit Select
					Case Else
						If (chr <> Strings.ChrW(32)) Then
							Exit Select
						Else
							lineBufferAndEndOfTerminatorOffset = Me.CreateOffsetRestorePoint()
							If (Me.ScanXmlTriviaInXmlDoc(chr, syntaxListBuilder)) Then
								Exit Select
							End If
							lineBufferAndEndOfTerminatorOffset.Restore()
							list = syntaxListBuilder.ToList()
							syntaxToken1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Token(list.Node, SyntaxKind.EndOfXmlToken, Nothing, [String].Empty)
							Me._triviaListPool.Free(syntaxListBuilder)
							syntaxToken = syntaxToken1
							Return syntaxToken
						End If
				End Select
			End If
			Dim length As Integer = 0
			While Me.CanGet(length)
				Dim chr1 As Char = Me.Peek(length)
				If (chr1 = Strings.ChrW(10) OrElse chr1 = Strings.ChrW(13)) Then
					syntaxToken1 = Me.XmlMakeProcessingInstructionToken(syntaxListBuilder.ToList(), length + Me.LengthOfLineBreak(chr1, length))
					Me._triviaListPool.Free(syntaxListBuilder)
					syntaxToken = syntaxToken1
					Return syntaxToken
				Else
					If (chr1 = "?"C) Then
						If (Me.NextIs(length + 1, ">"C)) Then
							If (length = 0) Then
								syntaxToken1 = Me.XmlMakeEndProcessingInstructionToken(syntaxListBuilder.ToList())
								Me._triviaListPool.Free(syntaxListBuilder)
								syntaxToken = syntaxToken1
								Return syntaxToken
							Else
								syntaxToken1 = Me.XmlMakeProcessingInstructionToken(syntaxListBuilder.ToList(), length)
								Me._triviaListPool.Free(syntaxListBuilder)
								syntaxToken = syntaxToken1
								Return syntaxToken
							End If
						End If
					End If
					Dim xmlCharResult As Scanner.XmlCharResult = Me.ScanXmlChar(length)
					If (xmlCharResult.Length > 0) Then
						length += xmlCharResult.Length
					ElseIf (length = 0) Then
						syntaxToken1 = Me.XmlMakeBadToken(syntaxListBuilder.ToList(), 1, ERRID.ERR_IllegalChar)
						Me._triviaListPool.Free(syntaxListBuilder)
						syntaxToken = syntaxToken1
						Return syntaxToken
					Else
						syntaxToken1 = Me.XmlMakeProcessingInstructionToken(syntaxListBuilder.ToList(), length)
						Me._triviaListPool.Free(syntaxListBuilder)
						syntaxToken = syntaxToken1
						Return syntaxToken
					End If
				End If
			End While
			GoTo Label4
			Me._triviaListPool.Free(syntaxListBuilder)
			syntaxToken = syntaxToken1
			Return syntaxToken
		Label4:
			If (length <= 0) Then
				syntaxToken1 = Scanner.MakeEofToken(syntaxListBuilder.ToList())
				Me._triviaListPool.Free(syntaxListBuilder)
				syntaxToken = syntaxToken1
				Return syntaxToken
			Else
				syntaxToken1 = Me.XmlMakeProcessingInstructionToken(syntaxListBuilder.ToList(), length)
				Me._triviaListPool.Free(syntaxListBuilder)
				syntaxToken = syntaxToken1
				Return syntaxToken
			End If
		End Function

		Private Function ScanXmlReference(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax
			Dim xmlTextTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim diagnosticInfoArray As Microsoft.CodeAnalysis.DiagnosticInfo()
			Dim xmlTextTokenSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax
			If (Me.CanGet(1)) Then
				Dim chr As Char = Me.Peek(1)
				If (chr <= "a"C) Then
					If (chr = "#"C) Then
						Dim num As Integer = 2
						Dim xmlCharResult As Scanner.XmlCharResult = Me.ScanXmlCharRef(num)
						If (xmlCharResult.Length = 0) Then
							xmlTextTokenSyntax1 = Me.XmlMakeEntityLiteralToken(precedingTrivia, 1, "")
							diagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_XmlEntityReference)
							diagnosticInfoArray = New Microsoft.CodeAnalysis.DiagnosticInfo() { diagnosticInfo }
							xmlTextTokenSyntax = DirectCast(xmlTextTokenSyntax1.SetDiagnostics(diagnosticInfoArray), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax)
							Return xmlTextTokenSyntax
						End If
						Dim str As String = Nothing
						If (xmlCharResult.Length = 1) Then
							str = Me.Intern(xmlCharResult.Char1)
						ElseIf (xmlCharResult.Length = 2) Then
							str = Me.Intern(New [Char]() { xmlCharResult.Char1, xmlCharResult.Char2 })
						End If
						If (Not Me.CanGet(num) OrElse Me.Peek(num) <> ";"C) Then
							Dim xmlTextTokenSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax = Me.XmlMakeEntityLiteralToken(precedingTrivia, num, str)
							Dim diagnosticInfo1 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_ExpectedSColon)
							xmlTextTokenSyntax = DirectCast(xmlTextTokenSyntax2.SetDiagnostics(New Microsoft.CodeAnalysis.DiagnosticInfo() { diagnosticInfo1 }), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax)
							Return xmlTextTokenSyntax
						Else
							xmlTextTokenSyntax = Me.XmlMakeEntityLiteralToken(precedingTrivia, num + 1, str)
							Return xmlTextTokenSyntax
						End If
					ElseIf (chr = "a"C) Then
						If (Not Me.CanGet(4) OrElse Not Me.NextAre(2, "mp")) Then
							If (Not Me.CanGet(5) OrElse Not Me.NextAre(2, "pos")) Then
								xmlTextTokenSyntax1 = Me.XmlMakeEntityLiteralToken(precedingTrivia, 1, "")
								diagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_XmlEntityReference)
								diagnosticInfoArray = New Microsoft.CodeAnalysis.DiagnosticInfo() { diagnosticInfo }
								xmlTextTokenSyntax = DirectCast(xmlTextTokenSyntax1.SetDiagnostics(diagnosticInfoArray), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax)
								Return xmlTextTokenSyntax
							End If
							If (Me.Peek(5) <> ";"C) Then
								Dim xmlTextTokenSyntax3 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax = Me.XmlMakeEntityLiteralToken(precedingTrivia, 5, "'")
								Dim diagnosticInfo2 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_ExpectedSColon)
								xmlTextTokenSyntax = DirectCast(xmlTextTokenSyntax3.SetDiagnostics(New Microsoft.CodeAnalysis.DiagnosticInfo() { diagnosticInfo2 }), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax)
								Return xmlTextTokenSyntax
							Else
								xmlTextTokenSyntax = Me.XmlMakeAposLiteralToken(precedingTrivia)
								Return xmlTextTokenSyntax
							End If
						ElseIf (Me.Peek(4) <> ";"C) Then
							Dim xmlTextTokenSyntax4 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax = Me.XmlMakeEntityLiteralToken(precedingTrivia, 4, "&")
							Dim diagnosticInfo3 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_ExpectedSColon)
							xmlTextTokenSyntax = DirectCast(xmlTextTokenSyntax4.SetDiagnostics(New Microsoft.CodeAnalysis.DiagnosticInfo() { diagnosticInfo3 }), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax)
							Return xmlTextTokenSyntax
						Else
							xmlTextTokenSyntax = Me.XmlMakeAmpLiteralToken(precedingTrivia)
							Return xmlTextTokenSyntax
						End If
					End If
				ElseIf (chr = "g"C) Then
					If (Not Me.CanGet(3) OrElse Not Me.NextIs(2, "t"C)) Then
						xmlTextTokenSyntax1 = Me.XmlMakeEntityLiteralToken(precedingTrivia, 1, "")
						diagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_XmlEntityReference)
						diagnosticInfoArray = New Microsoft.CodeAnalysis.DiagnosticInfo() { diagnosticInfo }
						xmlTextTokenSyntax = DirectCast(xmlTextTokenSyntax1.SetDiagnostics(diagnosticInfoArray), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax)
						Return xmlTextTokenSyntax
					End If
					If (Me.Peek(3) <> ";"C) Then
						Dim xmlTextTokenSyntax5 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax = Me.XmlMakeEntityLiteralToken(precedingTrivia, 3, ">")
						Dim diagnosticInfo4 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_ExpectedSColon)
						xmlTextTokenSyntax = DirectCast(xmlTextTokenSyntax5.SetDiagnostics(New Microsoft.CodeAnalysis.DiagnosticInfo() { diagnosticInfo4 }), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax)
						Return xmlTextTokenSyntax
					Else
						xmlTextTokenSyntax = Me.XmlMakeGtLiteralToken(precedingTrivia)
						Return xmlTextTokenSyntax
					End If
				ElseIf (chr = "l"C) Then
					If (Not Me.CanGet(3) OrElse Not Me.NextIs(2, "t"C)) Then
						xmlTextTokenSyntax1 = Me.XmlMakeEntityLiteralToken(precedingTrivia, 1, "")
						diagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_XmlEntityReference)
						diagnosticInfoArray = New Microsoft.CodeAnalysis.DiagnosticInfo() { diagnosticInfo }
						xmlTextTokenSyntax = DirectCast(xmlTextTokenSyntax1.SetDiagnostics(diagnosticInfoArray), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax)
						Return xmlTextTokenSyntax
					End If
					If (Me.Peek(3) <> ";"C) Then
						Dim xmlTextTokenSyntax6 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax = Me.XmlMakeEntityLiteralToken(precedingTrivia, 3, "<")
						Dim diagnosticInfo5 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_ExpectedSColon)
						xmlTextTokenSyntax = DirectCast(xmlTextTokenSyntax6.SetDiagnostics(New Microsoft.CodeAnalysis.DiagnosticInfo() { diagnosticInfo5 }), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax)
						Return xmlTextTokenSyntax
					Else
						xmlTextTokenSyntax = Me.XmlMakeLtLiteralToken(precedingTrivia)
						Return xmlTextTokenSyntax
					End If
				ElseIf (chr = "q"C) Then
					If (Not Me.CanGet(5) OrElse Not Me.NextAre(2, "uot")) Then
						xmlTextTokenSyntax1 = Me.XmlMakeEntityLiteralToken(precedingTrivia, 1, "")
						diagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_XmlEntityReference)
						diagnosticInfoArray = New Microsoft.CodeAnalysis.DiagnosticInfo() { diagnosticInfo }
						xmlTextTokenSyntax = DirectCast(xmlTextTokenSyntax1.SetDiagnostics(diagnosticInfoArray), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax)
						Return xmlTextTokenSyntax
					End If
					If (Me.Peek(5) <> ";"C) Then
						Dim xmlTextTokenSyntax7 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax = Me.XmlMakeEntityLiteralToken(precedingTrivia, 5, """")
						Dim diagnosticInfo6 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_ExpectedSColon)
						xmlTextTokenSyntax = DirectCast(xmlTextTokenSyntax7.SetDiagnostics(New Microsoft.CodeAnalysis.DiagnosticInfo() { diagnosticInfo6 }), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax)
						Return xmlTextTokenSyntax
					Else
						xmlTextTokenSyntax = Me.XmlMakeQuotLiteralToken(precedingTrivia)
						Return xmlTextTokenSyntax
					End If
				End If
			End If
			xmlTextTokenSyntax1 = Me.XmlMakeEntityLiteralToken(precedingTrivia, 1, "")
			diagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_XmlEntityReference)
			diagnosticInfoArray = New Microsoft.CodeAnalysis.DiagnosticInfo() { diagnosticInfo }
			xmlTextTokenSyntax = DirectCast(xmlTextTokenSyntax1.SetDiagnostics(diagnosticInfoArray), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax)
			Return xmlTextTokenSyntax
		End Function

		Friend Function ScanXmlString(ByVal terminatingChar As Char, ByVal altTerminatingChar As Char, ByVal isSingle As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = Me._triviaListPool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
			If (Me.IsScanningXmlDoc AndAlso Me.IsAtNewLine()) Then
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Me.ScanXmlDocTrivia()
				If (visualBasicSyntaxNode Is Nothing) Then
					syntaxToken = Me.MakeEofToken()
					Me._triviaListPool.Free(syntaxListBuilder)
					Return syntaxToken
				End If
				syntaxListBuilder.Add(visualBasicSyntaxNode)
			End If
			Dim length As Integer = 0
			Dim scratch As StringBuilder = Me.GetScratch()
			While Me.CanGet(length)
				Dim chr As Char = Me.Peek(length)
				If (Not (chr = terminatingChar Or chr = altTerminatingChar)) Then
					Select Case chr
						Case Strings.ChrW(9)
							scratch.Append(Strings.ChrW(32))
							length = length + 1
							Continue While
						Case Strings.ChrW(10)
						Case Strings.ChrW(13)
							length = Me.SkipLineBreak(chr, length)
							scratch.Append(Strings.ChrW(32))
							syntaxToken = Me.XmlMakeAttributeDataToken(syntaxListBuilder, length, scratch)
							Me._triviaListPool.Free(syntaxListBuilder)
							Return syntaxToken
						Case Strings.ChrW(11)
						Case Strings.ChrW(12)
						Label3:
							Dim xmlCharResult As Scanner.XmlCharResult = Me.ScanXmlChar(length)
							If (xmlCharResult.Length <> 0) Then
								xmlCharResult.AppendTo(scratch)
								length += xmlCharResult.Length
								Continue While
							ElseIf (length <= 0) Then
								syntaxToken = Me.XmlMakeBadToken(syntaxListBuilder, 1, ERRID.ERR_IllegalChar)
								Me._triviaListPool.Free(syntaxListBuilder)
								Return syntaxToken
							Else
								syntaxToken = Me.XmlMakeAttributeDataToken(syntaxListBuilder, length, scratch)
								Me._triviaListPool.Free(syntaxListBuilder)
								Return syntaxToken
							End If
						Case Else
							If (chr = "&"C) Then
								If (length <= 0) Then
									syntaxToken = Me.ScanXmlReference(syntaxListBuilder)
									Me._triviaListPool.Free(syntaxListBuilder)
									Return syntaxToken
								Else
									syntaxToken = Me.XmlMakeAttributeDataToken(syntaxListBuilder, length, scratch)
									Me._triviaListPool.Free(syntaxListBuilder)
									Return syntaxToken
								End If
							ElseIf (chr <> "<"C) Then
								GoTo Label3
							ElseIf (length > 0) Then
								syntaxToken = Me.XmlMakeAttributeDataToken(syntaxListBuilder, length, scratch)
								Me._triviaListPool.Free(syntaxListBuilder)
								Return syntaxToken
							ElseIf (Not Me.NextAre(1, "%=")) Then
								Dim syntaxToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.SingleQuoteToken)
								If (syntaxListBuilder.Count > 0) Then
									Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = syntaxListBuilder.ToList()
									syntaxToken1 = DirectCast(syntaxToken1.WithLeadingTrivia(list.Node), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
								End If
								Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(If(isSingle, ERRID.ERR_ExpectedSQuote, ERRID.ERR_ExpectedQuote))
								syntaxToken = DirectCast(syntaxToken1.SetDiagnostics(New Microsoft.CodeAnalysis.DiagnosticInfo() { diagnosticInfo }), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
								Me._triviaListPool.Free(syntaxListBuilder)
								Return syntaxToken
							Else
								Dim xmlTextTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax = Me.XmlMakeAttributeDataToken(syntaxListBuilder, 3, "<%=")
								Dim diagnosticInfo1 As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_QuotedEmbeddedExpression)
								syntaxToken = DirectCast(xmlTextTokenSyntax.SetDiagnostics(New Microsoft.CodeAnalysis.DiagnosticInfo() { diagnosticInfo1 }), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
								Me._triviaListPool.Free(syntaxListBuilder)
								Return syntaxToken
							End If
					End Select
				ElseIf (length > 0) Then
					syntaxToken = Me.XmlMakeAttributeDataToken(syntaxListBuilder, length, scratch)
					Me._triviaListPool.Free(syntaxListBuilder)
					Return syntaxToken
				ElseIf (Not isSingle) Then
					syntaxToken = Me.XmlMakeDoubleQuoteToken(syntaxListBuilder, chr, False)
					Me._triviaListPool.Free(syntaxListBuilder)
					Return syntaxToken
				Else
					syntaxToken = Me.XmlMakeSingleQuoteToken(syntaxListBuilder, chr, False)
					Me._triviaListPool.Free(syntaxListBuilder)
					Return syntaxToken
				End If
			End While
			GoTo Label2
			Me._triviaListPool.Free(syntaxListBuilder)
			Return syntaxToken
		Label2:
			If (length <= 0) Then
				syntaxToken = Scanner.MakeEofToken(syntaxListBuilder)
				Me._triviaListPool.Free(syntaxListBuilder)
				Return syntaxToken
			Else
				syntaxToken = Me.XmlMakeAttributeDataToken(syntaxListBuilder, length, scratch)
				Me._triviaListPool.Free(syntaxListBuilder)
				Return syntaxToken
			End If
		End Function

		Friend Function ScanXmlStringDouble() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Return Me.ScanXmlString(""""C, """"C, False)
		End Function

		Friend Function ScanXmlStringSingle() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Return Me.ScanXmlString("'"C, "'"C, True)
		End Function

		Friend Function ScanXmlStringSmartDouble() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Return Me.ScanXmlString("”"C, "“"C, False)
		End Function

		Friend Function ScanXmlStringSmartSingle() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Return Me.ScanXmlString("’"C, "‘"C, True)
		End Function

		Friend Function ScanXmlStringUnQuoted() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			If (Me.CanGet()) Then
				Dim length As Integer = 0
				Dim scratch As StringBuilder = Me.GetScratch()
				While True
					If (Me.CanGet(length)) Then
						Dim chr As Char = Me.Peek(length)
						If (chr <= Strings.ChrW(32)) Then
							Select Case chr
								Case Strings.ChrW(9)
								Case Strings.ChrW(10)
								Case Strings.ChrW(13)
									GoTo Label1
								Case Strings.ChrW(11)
								Case Strings.ChrW(12)
									Exit Select
								Case Else
									If (chr = Strings.ChrW(32)) Then
										GoTo Label1
									End If
									Exit Select
							End Select
						ElseIf (chr = "&"C) Then
							If (length <= 0) Then
								syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
								syntaxToken = Me.ScanXmlReference(syntaxList)
								Return syntaxToken
							Else
								syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
								syntaxToken = Me.XmlMakeAttributeDataToken(syntaxList, length, scratch)
								Return syntaxToken
							End If
						ElseIf (chr <> "/"C) Then
							Select Case chr
								Case "<"C
								Case ">"C
								Case "?"C
									If (length = 0) Then
										syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
										syntaxToken = Scanner.MakeMissingToken(syntaxList, SyntaxKind.SingleQuoteToken)
										Return syntaxToken
									Else
										syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
										syntaxToken = Me.XmlMakeAttributeDataToken(syntaxList, length, scratch)
										Return syntaxToken
									End If
							End Select
						ElseIf (Me.NextIs(length + 1, ">"C)) Then
							If (length = 0) Then
								syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
								syntaxToken = Scanner.MakeMissingToken(syntaxList, SyntaxKind.SingleQuoteToken)
								Return syntaxToken
							Else
								syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
								syntaxToken = Me.XmlMakeAttributeDataToken(syntaxList, length, scratch)
								Return syntaxToken
							End If
						End If
						Dim xmlCharResult As Scanner.XmlCharResult = Me.ScanXmlChar(length)
						If (xmlCharResult.Length <> 0) Then
							xmlCharResult.AppendTo(scratch)
							length += xmlCharResult.Length
						ElseIf (length <= 0) Then
							syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
							syntaxToken = Me.XmlMakeBadToken(syntaxList, 1, ERRID.ERR_IllegalChar)
							Return syntaxToken
						Else
							syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
							syntaxToken = Me.XmlMakeAttributeDataToken(syntaxList, length, scratch)
							Return syntaxToken
						End If
					Else
						syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
						syntaxToken = Me.XmlMakeAttributeDataToken(syntaxList, length, scratch)
						Return syntaxToken
					End If
				End While
			Label1:
				If (length <= 0) Then
					syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
					syntaxToken = Scanner.MakeMissingToken(syntaxList, SyntaxKind.SingleQuoteToken)
				Else
					syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
					syntaxToken = Me.XmlMakeAttributeDataToken(syntaxList, length, scratch)
				End If
			Else
				syntaxToken = Me.MakeEofToken()
			End If
			Return syntaxToken
		End Function

		Private Function ScanXmlTrivia(ByVal c As Char) As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			Dim syntaxListBuilder As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxListBuilder = Me._triviaListPool.Allocate()
			Dim num As Integer = 0
			While True
				If (c = Strings.ChrW(32) OrElse c = Strings.ChrW(9)) Then
					num = num + 1
				Else
					If (c <> Strings.ChrW(13) AndAlso c <> Strings.ChrW(10)) Then
						Exit While
					End If
					If (num > 0) Then
						syntaxListBuilder.Add(Me.MakeWhiteSpaceTrivia(Me.GetText(num)))
						num = 0
					End If
					syntaxListBuilder.Add(Me.ScanNewlineAsTrivia(c))
				End If
				If (Not Me.CanGet(num)) Then
					Exit While
				End If
				c = Me.Peek(num)
			End While
			If (num > 0) Then
				syntaxListBuilder.Add(Me.MakeWhiteSpaceTrivia(Me.GetText(num)))
				num = 0
			End If
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode) = syntaxListBuilder.ToList()
			Me._triviaListPool.Free(syntaxListBuilder)
			Return list
		End Function

		Private Function ScanXmlTriviaInXmlDoc(ByVal c As Char, ByVal triviaList As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As Boolean
			Dim flag As Boolean
			Dim num As Integer = 0
			While True
				If (c = Strings.ChrW(32) OrElse c = Strings.ChrW(9)) Then
					num = num + 1
				ElseIf (Not SyntaxFacts.IsNewLine(c)) Then
					If (num > 0) Then
						triviaList.Add(Me.MakeWhiteSpaceTrivia(Me.GetText(num)))
					End If
					flag = True
					Exit While
				Else
					If (num > 0) Then
						triviaList.Add(Me.MakeWhiteSpaceTrivia(Me.GetText(num)))
						num = 0
					End If
					Dim lineBufferAndEndOfTerminatorOffset As Scanner.LineBufferAndEndOfTerminatorOffsets = Me.CreateOffsetRestorePoint()
					Dim syntaxTrivium As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia = Me.ScanNewlineAsTrivia(c)
					Dim xmlWhitespaceLength As Integer = Me.GetXmlWhitespaceLength(0)
					If (Not Me.TrySkipXmlDocMarker(xmlWhitespaceLength)) Then
						lineBufferAndEndOfTerminatorOffset.Restore()
						flag = False
						Exit While
					Else
						triviaList.Add(syntaxTrivium)
						triviaList.Add(Me.MakeDocumentationCommentExteriorTrivia(Me.GetText(xmlWhitespaceLength)))
					End If
				End If
				c = Me.Peek(num)
			End While
			Return flag
		End Function

		Private Function ScanXmlWhitespace(Optional ByVal len As Integer = 0) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			len = Me.GetXmlWhitespaceLength(len)
			If (len <= 0) Then
				visualBasicSyntaxNode = Nothing
			Else
				visualBasicSyntaxNode = Me.MakeWhiteSpaceTrivia(Me.GetText(len))
			End If
			Return visualBasicSyntaxNode
		End Function

		Private Function SkipConditionalCompilationSection() As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			Dim num As Integer = 0
			Dim start As Integer = -1
			Dim length As Integer = 0
			While True
				Dim nextConditionalLine As TextSpan = Me.SkipToNextConditionalLine()
				If (start < 0) Then
					start = nextConditionalLine.Start
				End If
				length += nextConditionalLine.Length
				Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.GetCurrentToken()
				Dim kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = currentToken.Kind
				If (kind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOfFileToken) Then
					If (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BadToken OrElse kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DateLiteralToken) Then
						length += Me.GetCurrentToken().FullWidth
						Me.GetNextTokenInState(ScannerState.VB)
						Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me.GetCurrentToken().Kind
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StatementTerminatorToken OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ColonToken) Then
							Me.GetNextTokenInState(ScannerState.VB)
							Continue While
						End If
					End If
				ElseIf (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HashToken) Then
					Dim kind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me.PeekToken(1, ScannerState.VB).Kind
					Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.PeekToken(2, ScannerState.VB)
					If (num = 0) Then
						If (kind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndKeyword) Then
							If (Not Scanner.IsContextualKeyword(syntaxToken, New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalSourceKeyword, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RegionKeyword })) Then
								Exit While
							End If
						End If
						If (kind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfKeyword OrElse kind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseIfKeyword OrElse kind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseKeyword) Then
							Exit While
						End If
					End If
					If (kind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfKeyword) Then
						If (kind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndKeyword) Then
							If (Not Scanner.IsContextualKeyword(syntaxToken, New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalSourceKeyword, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RegionKeyword })) Then
								GoTo Label1
							End If
						End If
						If (kind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IfKeyword) Then
							num = num + 1
							GoTo Label0
						Else
							GoTo Label0
						End If
					End If
				Label1:
					num = num - 1
				Label0:
					If (num < 0) Then
						Exit While
					End If
					length += Me.GetCurrentToken().FullWidth
					Me.GetNextTokenInState(ScannerState.VB)
					If (kind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StatementTerminatorToken OrElse kind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ColonToken) Then
						Me.GetNextTokenInState(ScannerState.VB)
						Continue While
					End If
				ElseIf (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOfFileToken) Then
					Exit While
				End If
				Throw ExceptionUtilities.UnexpectedValue(currentToken.Kind)
			End While
			syntaxList = If(length <= 0, New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(Nothing), New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(Me.GetDisabledTextAt(New TextSpan(start, length))))
			Return syntaxList
		End Function

		Private Function SkipLineBreak(ByVal StartCharacter As Char, ByVal index As Integer) As Integer
			Return index + Me.LengthOfLineBreak(StartCharacter, index)
		End Function

		Public Function SkipToNextConditionalLine() As TextSpan
			Me.ResetLineBufferOffset()
			Dim num As Integer = Me._lineBufferOffset
			Dim prevToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.PrevToken
			If (Not Me.IsAtNewLine() OrElse Me.PrevToken IsNot Nothing AndAlso Me.PrevToken.EndsWithEndOfLineOrColonTrivia()) Then
				Me.EatThroughLine()
			End If
			Dim num1 As Integer = Me._lineBufferOffset
			While Me.CanGet()
				Dim chr As Char = Me.Peek()
				Select Case chr
					Case Strings.ChrW(9)
					Case Strings.ChrW(32)
						Me.EatWhitespace()
						Continue While
					Case Strings.ChrW(10)
					Case Strings.ChrW(13)
						Me.EatThroughLineBreak(chr)
						num1 = Me._lineBufferOffset
						Continue While
					Case Strings.ChrW(11)
					Case Strings.ChrW(12)
					Case Strings.ChrW(14)
					Case Strings.ChrW(15)
					Case Strings.ChrW(16)
					Case Strings.ChrW(17)
					Case Strings.ChrW(18)
					Case Strings.ChrW(19)
					Case Strings.ChrW(20)
					Case Strings.ChrW(21)
					Case Strings.ChrW(22)
					Case Strings.ChrW(23)
					Case Strings.ChrW(24)
					Case Strings.ChrW(25)
					Case Strings.ChrW(26)
					Case Strings.ChrW(27)
					Case Strings.ChrW(28)
					Case Strings.ChrW(29)
					Case Strings.ChrW(30)
					Case Strings.ChrW(31)
					Case "!"C
					Case """"C
					Case "$"C
					Case "%"C
					Case "&"C
					Case "("C
					Case ")"C
					Case "*"C
					Case "+"C
					Case ","C
					Case "-"C
					Case "."C
					Case "/"C
					Case "0"C
					Case "1"C
					Case "2"C
					Case "3"C
					Case "4"C
					Case "5"C
					Case "6"C
					Case "7"C
					Case "8"C
					Case "9"C
					Case ":"C
					Case ";"C
					Case "<"C
					Case "="C
					Case ">"C
					Case "?"C
					Case "@"C
					Case "["C
					Case "\"C
					Case "]"C
					Case Strings.ChrW(94)
					Case Strings.ChrW(96)
						If (SyntaxFacts.IsWhitespace(chr)) Then
							Me.EatWhitespace()
							Continue While
						ElseIf (Not SyntaxFacts.IsNewLine(chr)) Then
							Me.EatThroughLine()
							num1 = Me._lineBufferOffset
							Continue While
						Else
							Me.EatThroughLineBreak(chr)
							num1 = Me._lineBufferOffset
							Continue While
						End If
					Case "#"C
						Exit Select
					Case "'"C
					Case "A"C
					Case "B"C
					Case "C"C
					Case "D"C
					Case "E"C
					Case "F"C
					Case "G"C
					Case "H"C
					Case "I"C
					Case "J"C
					Case "K"C
					Case "L"C
					Case "M"C
					Case "N"C
					Case "O"C
					Case "P"C
					Case "Q"C
					Case "R"C
					Case "S"C
					Case "T"C
					Case "U"C
					Case "V"C
					Case "W"C
					Case "X"C
					Case "Y"C
					Case "Z"C
					Case Strings.ChrW(95)
					Case "a"C
					Case "b"C
					Case "c"C
					Case "d"C
					Case "e"C
					Case "f"C
					Case "g"C
					Case "h"C
					Case "i"C
					Case "j"C
					Case "k"C
					Case "l"C
					Case "m"C
					Case "n"C
					Case "o"C
					Case "p"C
					Case "q"C
					Case "r"C
					Case "s"C
					Case "t"C
					Case "u"C
					Case "v"C
					Case "w"C
					Case "x"C
					Case "y"C
					Case "z"C
						Me.EatThroughLine()
						num1 = Me._lineBufferOffset
						Continue While
					Case Else
						If (chr = "＃"C) Then
							Me._lineBufferOffset = num1
							Me.ResetTokens()
							Return TextSpan.FromBounds(num, num1)
						End If
						GoTo Label0
				End Select
			End While
			Me._lineBufferOffset = num1
			Me.ResetTokens()
			Return TextSpan.FromBounds(num, num1)
		End Function

		Private Function StartsDirective(ByVal here As Integer) As Boolean
			Dim flag As Boolean
			flag = If(Not Me.CanGet(here), False, SyntaxFacts.IsHash(Me.Peek(here)))
			Return flag
		End Function

		Private Function StartsXmlDoc(ByVal here As Integer) As Boolean
			If (Me._options.DocumentationMode < DocumentationMode.Parse OrElse Not Me.CanGet(here + 3) OrElse Not SyntaxFacts.IsSingleQuote(Me.Peek(here)) OrElse Not SyntaxFacts.IsSingleQuote(Me.Peek(here + 1)) OrElse Not SyntaxFacts.IsSingleQuote(Me.Peek(here + 2))) Then
				Return False
			End If
			Return Not SyntaxFacts.IsSingleQuote(Me.Peek(here + 3))
		End Function

		Private Function TokenOfStringCached(ByVal spelling As String) As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			syntaxKind = If(spelling.Length <= 16, Me._KeywordsObjs.GetOrMakeValue(spelling), Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierToken)
			Return syntaxKind
		End Function

		Friend Sub TransitionFromVBToXml(ByVal state As ScannerState, ByVal toCompare As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByRef toRemove As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByRef toAdd As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode))
			Dim innerTokenObject As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me._prevToken.InnerTokenObject
			Me.AbandonAllTokens()
			Me.RevertState(Me._prevToken)
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(innerTokenObject.GetTrailingTrivia())
			Dim count As Integer = syntaxList.Count - Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxNodeExtensions.GetLengthOfCommonEnd(syntaxList, toCompare)
			toRemove = syntaxList.GetEndOfTrivia(count)
			Dim startOfTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = syntaxList.GetStartOfTrivia(count)
			innerTokenObject = DirectCast(innerTokenObject.WithTrailingTrivia(startOfTrivia.Node), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			Dim fullWidth As Integer = Scanner.GetFullWidth(Me._prevToken, innerTokenObject)
			Me._lineBufferOffset += fullWidth
			Me._endOfTerminatorTrivia = Me._lineBufferOffset
			toAdd = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
			Me._prevToken = Me._prevToken.[With](Me._prevToken.State, innerTokenObject)
			Me._currentToken = New Scanner.ScannerToken(Me._scannerPreprocessorState, Me._lineBufferOffset, Me._endOfTerminatorTrivia, Nothing, state)
		End Sub

		Friend Sub TransitionFromXmlToVB(ByVal toCompare As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByRef toRemove As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByRef toAdd As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode))
			Dim innerTokenObject As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me._prevToken.InnerTokenObject
			Dim flag As Boolean = If(Me._currentToken.InnerTokenObject Is Nothing, False, Me._currentToken.InnerTokenObject.Kind = SyntaxKind.EndOfXmlToken)
			Me.AbandonAllTokens()
			Me.RevertState(Me._prevToken)
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(innerTokenObject.GetTrailingTrivia())
			Dim count As Integer = syntaxList.Count - Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxNodeExtensions.GetLengthOfCommonEnd(syntaxList, toCompare)
			toRemove = syntaxList.GetEndOfTrivia(count)
			Dim startOfTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = syntaxList.GetStartOfTrivia(count)
			innerTokenObject = DirectCast(innerTokenObject.WithTrailingTrivia(startOfTrivia.Node), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			Dim fullWidth As Integer = Scanner.GetFullWidth(Me._prevToken, innerTokenObject)
			Me._lineBufferOffset += fullWidth
			Me._endOfTerminatorTrivia = Me._lineBufferOffset
			toAdd = Me.ScanSingleLineTrivia(flag)
			Dim scannerState As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState.VB
			innerTokenObject = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.AddTrailingTrivia(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(innerTokenObject, toAdd.Node)
			Me._prevToken = Me._prevToken.[With](scannerState, innerTokenObject)
			Me._currentToken = New Scanner.ScannerToken(Me._scannerPreprocessorState, Me._lineBufferOffset, Me._endOfTerminatorTrivia, Nothing, scannerState)
		End Sub

		Friend Overridable Function TryCrumbleOnce() As Boolean
			Return False
		End Function

		Private Function TryGet(ByVal num As Integer, ByRef ch As Char) As Boolean
			Dim flag As Boolean
			If (Not Me.CanGet(num)) Then
				flag = False
			Else
				ch = Me.Peek(num)
				flag = True
			End If
			Return flag
		End Function

		Friend Shared Function TryIdentifierAsContextualKeyword(ByVal id As IdentifierTokenSyntax, ByRef k As SyntaxKind) As Boolean
			Dim flag As Boolean
			If (id.PossibleKeywordKind = SyntaxKind.IdentifierToken) Then
				flag = False
			Else
				k = id.PossibleKeywordKind
				flag = True
			End If
			Return flag
		End Function

		Friend Function TryIdentifierAsContextualKeyword(ByVal id As IdentifierTokenSyntax, ByRef k As KeywordSyntax) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierToken
			If (Not Scanner.TryIdentifierAsContextualKeyword(id, syntaxKind)) Then
				flag = False
			Else
				k = Me.MakeKeyword(id)
				flag = True
			End If
			Return flag
		End Function

		Private Function TryScanDirective(ByVal tList As SyntaxListBuilder) As Boolean
			If (Me.CanGet() AndAlso SyntaxFacts.IsWhitespace(Me.Peek())) Then
				tList.Add(Me.ScanWhitespace(0))
			End If
			Dim restorePoint As Scanner.RestorePoint = Me.CreateRestorePoint()
			Me._isScanningDirective = True
			Me.GetNextTokenInState(ScannerState.VB)
			Dim currentSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax = TryCast(Me.GetCurrentSyntaxNode(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax)
			If (currentSyntaxNode Is Nothing) Then
				Dim parser As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser(Me)
				currentSyntaxNode = parser.ParseConditionalCompilationStatement()
				currentSyntaxNode = parser.ConsumeStatementTerminatorAfterDirective(currentSyntaxNode)
			Else
				Me.MoveToNextSyntaxNodeInTrivia()
				Me.GetNextTokenInState(ScannerState.VB)
			End If
			Me.ProcessDirective(currentSyntaxNode, tList)
			Me.ResetLineBufferOffset()
			restorePoint.RestoreTokens(True)
			Me._isScanningDirective = False
			Return True
		End Function

		Private Function TryScanSinglePieceOfMultilineTrivia(ByVal tList As SyntaxListBuilder) As Boolean
			Dim flag As Boolean
			If (Not Me.CanGet()) Then
				flag = False
			Else
				Dim flag1 As Boolean = Me.IsAtNewLine()
				If (flag1) Then
					If (Me.StartsXmlDoc(0)) Then
						flag = Me.TryScanXmlDocComment(tList)
						Return flag
					ElseIf (Not Me.StartsDirective(0)) Then
						If (Not Me.IsConflictMarkerTrivia()) Then
							GoTo Label1
						End If
						Me.ScanConflictMarker(tList)
						flag = True
						Return flag
					Else
						flag = Me.TryScanDirective(tList)
						Return flag
					End If
				End If
			Label1:
				Dim chr As Char = Me.Peek()
				If (SyntaxFacts.IsWhitespace(chr)) Then
					Dim whitespaceLength As Integer = Me.GetWhitespaceLength(1)
					If (flag1) Then
						If (Not Me.StartsXmlDoc(whitespaceLength)) Then
							If (Not Me.StartsDirective(whitespaceLength)) Then
								GoTo Label2
							End If
							flag = Me.TryScanDirective(tList)
							Return flag
						Else
							flag = Me.TryScanXmlDocComment(tList)
							Return flag
						End If
					End If
				Label2:
					tList.Add(Me.MakeWhiteSpaceTrivia(Me.GetText(whitespaceLength)))
					flag = True
				ElseIf (SyntaxFacts.IsNewLine(chr)) Then
					tList.Add(Me.ScanNewlineAsTrivia(chr))
					flag = True
				ElseIf (SyntaxFacts.IsUnderscore(chr)) Then
					flag = Me.ScanLineContinuation(tList)
				ElseIf (Not Me.IsColonAndNotColonEquals(chr, 0)) Then
					flag = Me.ScanCommentIfAny(tList)
				Else
					tList.Add(Me.ScanColonAsTrivia())
					flag = True
				End If
			End If
			Return flag
		End Function

		Private Function TryScanToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			syntaxToken = If(Not Me.CanGet(), Scanner.MakeEofToken(precedingTrivia), Me.ScanTokenCommon(precedingTrivia, Me.Peek(), False))
			Return syntaxToken
		End Function

		Private Function TryScanXmlDocComment(ByVal tList As SyntaxListBuilder) As Boolean
			If (Me.CanGet() AndAlso SyntaxFacts.IsWhitespace(Me.Peek())) Then
				tList.Add(Me.ScanWhitespace(0))
			End If
			Dim restorePoint As Scanner.RestorePoint = Me.CreateRestorePoint()
			Me.GetNextTokenInState(ScannerState.Content)
			Dim currentSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DocumentationCommentTriviaSyntax = TryCast(Me.GetCurrentSyntaxNode(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DocumentationCommentTriviaSyntax)
			If (currentSyntaxNode Is Nothing) Then
				Dim parser As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser(Me)
				Me.IsScanningXmlDoc = True
				Me._isStartingFirstXmlDocLine = True
				Me._endOfXmlInsteadOfLastDocCommentLineBreak = True
				Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) = parser.ParseXmlContent(ScannerState.Content)
				Me._endOfXmlInsteadOfLastDocCommentLineBreak = False
				If (syntaxList.Count = 0 AndAlso parser.CurrentToken.Kind = SyntaxKind.EndOfXmlToken) Then
					Me.ResetLineBufferOffset()
					restorePoint.RestoreTokens(False)
					Me._isStartingFirstXmlDocLine = True
				End If
				syntaxList = parser.ParseRestOfDocCommentContent(syntaxList)
				Me.IsScanningXmlDoc = False
				Me.ResetLineBufferOffset()
				currentSyntaxNode = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.DocumentationCommentTrivia(syntaxList)
				If (Me.Options.DocumentationMode < DocumentationMode.Diagnose) Then
					currentSyntaxNode.ClearFlags(GreenNode.NodeFlags.ContainsDiagnostics)
				End If
			Else
				Me.MoveToNextSyntaxNodeInTrivia()
			End If
			restorePoint.RestoreTokens(True)
			tList.Add(currentSyntaxNode)
			Return True
		End Function

		Private Function TrySkipFollowingEquals(ByRef index As Integer) As Boolean
			Dim flag As Boolean
			Dim num As Integer = index
			While True
				If (Me.CanGet(num)) Then
					Dim chr As Char = Me.Peek(num)
					num = num + 1
					If (Not SyntaxFacts.IsWhitespace(chr)) Then
						If (chr = "="C OrElse chr = "＝"C) Then
							index = num
							flag = True
							Exit While
						Else
							flag = False
							Exit While
						End If
					End If
				Else
					flag = False
					Exit While
				End If
			End While
			Return flag
		End Function

		Private Function TrySkipXmlDocMarker(ByRef len As Integer) As Boolean
			Dim flag As Boolean
			Dim num As Integer = len
			While Me.CanGet(num) AndAlso SyntaxFacts.IsWhitespace(Me.Peek(num))
				num = num + 1
			End While
			If (Not Me.StartsXmlDoc(num)) Then
				flag = False
			Else
				len = num + 3
				flag = True
			End If
			Return flag
		End Function

		Friend Function TryTokenAsContextualKeyword(ByVal t As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByRef k As KeywordSyntax) As Boolean
			Dim flag As Boolean
			If (t IsNot Nothing) Then
				flag = If(t.Kind <> SyntaxKind.IdentifierToken, False, Me.TryIdentifierAsContextualKeyword(DirectCast(t, IdentifierTokenSyntax), k))
			Else
				flag = False
			End If
			Return flag
		End Function

		Friend Shared Function TryTokenAsKeyword(ByVal t As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByRef kind As SyntaxKind) As Boolean
			Dim flag As Boolean
			If (t Is Nothing) Then
				flag = False
			ElseIf (Not t.IsKeyword) Then
				flag = If(t.Kind <> SyntaxKind.IdentifierToken, False, Scanner.TryIdentifierAsContextualKeyword(DirectCast(t, IdentifierTokenSyntax), kind))
			Else
				kind = t.Kind
				flag = True
			End If
			Return flag
		End Function

		Private Function XmlLessThanExclamationToken(ByVal state As ScannerState, ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As BadTokenSyntax
			Return Me.XmlMakeBadToken(SyntaxSubKind.LessThanExclamationToken, precedingTrivia, 2, If(state = ScannerState.DocType, ERRID.ERR_DTDNotSupported, ERRID.ERR_Syntax))
		End Function

		Private Function XmlMakeAmpLiteralToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As XmlTextTokenSyntax
			Me.AdvanceChar(5)
			If (precedingTrivia.Node Is Nothing) Then
				Return Scanner.s_xmlAmpToken
			End If
			Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlEntityLiteralToken("&amp;", "&", precedingTrivia.Node, Nothing)
		End Function

		Private Function XmlMakeAposLiteralToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As XmlTextTokenSyntax
			Me.AdvanceChar(6)
			If (precedingTrivia.Node Is Nothing) Then
				Return Scanner.s_xmlAposToken
			End If
			Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlEntityLiteralToken("&apos;", "'", precedingTrivia.Node, Nothing)
		End Function

		Private Function XmlMakeAttributeDataToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal TokenWidth As Integer, ByVal Value As String) As XmlTextTokenSyntax
			Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlTextLiteralToken(Me.GetTextNotInterned(TokenWidth), Value, precedingTrivia.Node, Nothing)
		End Function

		Private Function XmlMakeAttributeDataToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal TokenWidth As Integer, ByVal Scratch As StringBuilder) As XmlTextTokenSyntax
			Return Me.XmlMakeTextLiteralToken(precedingTrivia, TokenWidth, Scratch)
		End Function

		Private Function XmlMakeBadToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal length As Integer, ByVal id As ERRID) As BadTokenSyntax
			Return Me.XmlMakeBadToken(SyntaxSubKind.None, precedingTrivia, length, id)
		End Function

		Private Function XmlMakeBadToken(ByVal subkind As SyntaxSubKind, ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal length As Integer, ByVal id As ERRID) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BadTokenSyntax
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim textNotInterned As String = Me.GetTextNotInterned(length)
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Me.ScanXmlWhitespace(0)
			Dim badTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BadTokenSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.BadToken(subkind, textNotInterned, precedingTrivia.Node, visualBasicSyntaxNode)
			If (CInt(id) - CInt(ERRID.ERR_IllegalXmlStartNameChar) > 1) Then
				diagnosticInfo = ErrorFactory.ErrorInfo(id)
			Else
				If (id = ERRID.ERR_IllegalXmlNameChar AndAlso (precedingTrivia.Any() OrElse Me.PrevToken Is Nothing OrElse Me.PrevToken.HasTrailingTrivia OrElse Me.PrevToken.Kind = SyntaxKind.LessThanToken OrElse Me.PrevToken.Kind = SyntaxKind.LessThanSlashToken OrElse Me.PrevToken.Kind = SyntaxKind.LessThanQuestionToken)) Then
					id = ERRID.ERR_IllegalXmlStartNameChar
				End If
				Dim chr As Char = textNotInterned(0)
				Dim unicode As Integer = XmlCharacterGlobalHelpers.UTF16ToUnicode(New Scanner.XmlCharResult(chr))
				diagnosticInfo = ErrorFactory.ErrorInfo(id, New [Object]() { chr, [String].Format("&H{0:X}", unicode) })
			End If
			Return DirectCast(badTokenSyntax.SetDiagnostics(New Microsoft.CodeAnalysis.DiagnosticInfo() { diagnosticInfo }), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BadTokenSyntax)
		End Function

		Private Function XmlMakeBeginCDataToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal scanTrailingTrivia As Scanner.ScanTriviaFunc) As PunctuationSyntax
			Me.AdvanceChar(9)
			Return Me.MakePunctuationToken(SyntaxKind.BeginCDataToken, "<![CDATA[", precedingTrivia, scanTrailingTrivia())
		End Function

		Private Function XmlMakeBeginCommentToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal scanTrailingTrivia As Scanner.ScanTriviaFunc) As PunctuationSyntax
			Me.AdvanceChar(4)
			Return Me.MakePunctuationToken(SyntaxKind.LessThanExclamationMinusMinusToken, "<!--", precedingTrivia, scanTrailingTrivia())
		End Function

		Private Function XmlMakeBeginDTDToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As BadTokenSyntax
			Return Me.XmlMakeBadToken(SyntaxSubKind.BeginDocTypeToken, precedingTrivia, 9, ERRID.ERR_DTDNotSupported)
		End Function

		Private Function XmlMakeBeginEmbeddedToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As PunctuationSyntax
			Me.AdvanceChar(3)
			Return Me.MakePunctuationToken(SyntaxKind.LessThanPercentEqualsToken, "<%=", precedingTrivia, New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)())
		End Function

		Private Function XmlMakeBeginEndElementToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal scanTrailingTrivia As Scanner.ScanTriviaFunc) As PunctuationSyntax
			Me.AdvanceChar(2)
			Return Me.MakePunctuationToken(SyntaxKind.LessThanSlashToken, "</", precedingTrivia, scanTrailingTrivia())
		End Function

		Private Function XmlMakeBeginProcessingInstructionToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal scanTrailingTrivia As Scanner.ScanTriviaFunc) As PunctuationSyntax
			Me.AdvanceChar(2)
			Return Me.MakePunctuationToken(SyntaxKind.LessThanQuestionToken, "<?", precedingTrivia, scanTrailingTrivia())
		End Function

		Private Function XmlMakeCDataToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal TokenWidth As Integer, ByVal scratch As StringBuilder) As XmlTextTokenSyntax
			Return Me.XmlMakeTextLiteralToken(precedingTrivia, TokenWidth, scratch)
		End Function

		Private Function XmlMakeCloseBracketToken(ByVal state As ScannerState, ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As BadTokenSyntax
			Return Me.XmlMakeBadToken(SyntaxSubKind.CloseBracketToken, precedingTrivia, 1, If(state = ScannerState.DocType, ERRID.ERR_DTDNotSupported, ERRID.ERR_IllegalXmlNameChar))
		End Function

		Private Function XmlMakeColonToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As PunctuationSyntax
			Me.AdvanceChar(1)
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Me.ScanXmlWhitespace(0)
			Return Me.MakePunctuationToken(SyntaxKind.ColonToken, ":", precedingTrivia, visualBasicSyntaxNode)
		End Function

		Private Function XmlMakeCommentToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal TokenWidth As Integer) As XmlTextTokenSyntax
			Dim textNotInterned As String = Me.GetTextNotInterned(TokenWidth)
			Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlTextLiteralToken(textNotInterned, textNotInterned, precedingTrivia.Node, Nothing)
		End Function

		Private Function XmlMakeDivToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As PunctuationSyntax
			Me.AdvanceChar(1)
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Me.ScanXmlWhitespace(0)
			Return Me.MakePunctuationToken(SyntaxKind.SlashToken, "/", precedingTrivia, visualBasicSyntaxNode)
		End Function

		Private Function XmlMakeDoubleQuoteToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal spelling As Char, ByVal isOpening As Boolean) As PunctuationSyntax
			Me.AdvanceChar(1)
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = Nothing
			If (Not isOpening) Then
				greenNode = Me.ScanXmlWhitespace(0)
			End If
			Return Me.MakePunctuationToken(SyntaxKind.DoubleQuoteToken, Me.Intern(spelling), precedingTrivia, greenNode)
		End Function

		Private Function XmlMakeEndCDataToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As PunctuationSyntax
			Me.AdvanceChar(3)
			Return Me.MakePunctuationToken(SyntaxKind.EndCDataToken, "]]>", precedingTrivia, New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)())
		End Function

		Private Function XmlMakeEndCommentToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As PunctuationSyntax
			Me.AdvanceChar(3)
			Return Me.MakePunctuationToken(SyntaxKind.MinusMinusGreaterThanToken, "-->", precedingTrivia, New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)())
		End Function

		Private Function XmlMakeEndEmbeddedToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal scanTrailingTrivia As Scanner.ScanTriviaFunc) As PunctuationSyntax
			Dim text As String
			If (Me.Peek() <> "%"C) Then
				text = Me.GetText(2)
			Else
				Me.AdvanceChar(2)
				text = "%>"
			End If
			Return Me.MakePunctuationToken(SyntaxKind.PercentGreaterThanToken, text, precedingTrivia, scanTrailingTrivia())
		End Function

		Private Function XmlMakeEndEmptyElementToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As PunctuationSyntax
			Me.AdvanceChar(2)
			Return Me.MakePunctuationToken(SyntaxKind.SlashGreaterThanToken, "/>", precedingTrivia, New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)())
		End Function

		Private Function XmlMakeEndProcessingInstructionToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As PunctuationSyntax
			Me.AdvanceChar(2)
			Return Me.MakePunctuationToken(SyntaxKind.QuestionGreaterThanToken, "?>", precedingTrivia, New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)())
		End Function

		Private Function XmlMakeEntityLiteralToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal TokenWidth As Integer, ByVal Value As String) As XmlTextTokenSyntax
			Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlEntityLiteralToken(Me.GetText(TokenWidth), Value, precedingTrivia.Node, Nothing)
		End Function

		Private Function XmlMakeEqualsToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As PunctuationSyntax
			Me.AdvanceChar(1)
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Me.ScanXmlWhitespace(0)
			Return Me.MakePunctuationToken(SyntaxKind.EqualsToken, "=", precedingTrivia, visualBasicSyntaxNode)
		End Function

		Private Function XmlMakeGreaterToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As PunctuationSyntax
			Me.AdvanceChar(1)
			Return Me.MakePunctuationToken(SyntaxKind.GreaterThanToken, ">", precedingTrivia, New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)())
		End Function

		Private Function XmlMakeGtLiteralToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As XmlTextTokenSyntax
			Me.AdvanceChar(4)
			If (precedingTrivia.Node Is Nothing) Then
				Return Scanner.s_xmlGtToken
			End If
			Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlEntityLiteralToken("&gt;", "&", precedingTrivia.Node, Nothing)
		End Function

		Private Function XmlMakeLeftParenToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As PunctuationSyntax
			Me.AdvanceChar(1)
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Me.ScanXmlWhitespace(0)
			Return Me.MakePunctuationToken(SyntaxKind.OpenParenToken, "(", precedingTrivia, visualBasicSyntaxNode)
		End Function

		Private Function XmlMakeLessToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As PunctuationSyntax
			Me.AdvanceChar(1)
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Me.ScanXmlWhitespace(0)
			Return Me.MakePunctuationToken(SyntaxKind.LessThanToken, "<", precedingTrivia, visualBasicSyntaxNode)
		End Function

		Private Function XmlMakeLtLiteralToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As XmlTextTokenSyntax
			Me.AdvanceChar(4)
			If (precedingTrivia.Node Is Nothing) Then
				Return Scanner.s_xmlLtToken
			End If
			Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlEntityLiteralToken("&lt;", "<", precedingTrivia.Node, Nothing)
		End Function

		Private Function XmlMakeOpenBracketToken(ByVal state As ScannerState, ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As BadTokenSyntax
			Return Me.XmlMakeBadToken(SyntaxSubKind.OpenBracketToken, precedingTrivia, 1, If(state = ScannerState.DocType, ERRID.ERR_DTDNotSupported, ERRID.ERR_IllegalXmlNameChar))
		End Function

		Private Function XmlMakeProcessingInstructionToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal TokenWidth As Integer) As XmlTextTokenSyntax
			Dim textNotInterned As String = Me.GetTextNotInterned(TokenWidth)
			Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlTextLiteralToken(textNotInterned, textNotInterned, precedingTrivia.Node, Nothing)
		End Function

		Private Function XmlMakeQuotLiteralToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As XmlTextTokenSyntax
			Me.AdvanceChar(6)
			If (precedingTrivia.Node Is Nothing) Then
				Return Scanner.s_xmlQuotToken
			End If
			Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlEntityLiteralToken("&quot;", """", precedingTrivia.Node, Nothing)
		End Function

		Private Function XmlMakeRightParenToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As PunctuationSyntax
			Me.AdvanceChar(1)
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Me.ScanXmlWhitespace(0)
			Return Me.MakePunctuationToken(SyntaxKind.CloseParenToken, ")", precedingTrivia, visualBasicSyntaxNode)
		End Function

		Private Function XmlMakeSingleQuoteToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal spelling As Char, ByVal isOpening As Boolean) As PunctuationSyntax
			Me.AdvanceChar(1)
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = Nothing
			If (Not isOpening) Then
				greenNode = Me.ScanXmlWhitespace(0)
			End If
			Return Me.MakePunctuationToken(SyntaxKind.SingleQuoteToken, Me.Intern(spelling), precedingTrivia, greenNode)
		End Function

		Private Function XmlMakeTextLiteralToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal TokenWidth As Integer, ByVal Scratch As StringBuilder) As XmlTextTokenSyntax
			Dim textNotInterned As String = Me.GetTextNotInterned(TokenWidth)
			Dim scratchText As String = Scanner.GetScratchText(Scratch, textNotInterned)
			Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlTextLiteralToken(textNotInterned, scratchText, precedingTrivia.Node, Nothing)
		End Function

		Private Function XmlMakeTextLiteralToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal TokenWidth As Integer, ByVal err As ERRID) As XmlTextTokenSyntax
			Dim textNotInterned As String = Me.GetTextNotInterned(TokenWidth)
			Return DirectCast(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlTextLiteralToken(textNotInterned, textNotInterned, precedingTrivia.Node, Nothing).SetDiagnostics(New DiagnosticInfo() { ErrorFactory.ErrorInfo(err) }), XmlTextTokenSyntax)
		End Function

		Private Function XmlMakeXmlNCNameToken(ByVal precedingTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal TokenWidth As Integer) As XmlNameTokenSyntax
			Dim text As String = Me.GetText(TokenWidth)
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNameToken
			If (text.Length = 3 AndAlso [String].Equals(text, "xml", StringComparison.Ordinal)) Then
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlKeyword
			End If
			If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNameToken) Then
				syntaxKind = Me.TokenOfStringCached(text)
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierToken) Then
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNameToken
				End If
			End If
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Me.ScanXmlWhitespace(0)
			Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlNameToken(text, syntaxKind, precedingTrivia.Node, visualBasicSyntaxNode)
		End Function

		Private Enum AccumulatorState
			Initial
			InitialAllowLeadingMultilineTrivia
			Ident
			TypeChar
			FollowingWhite
			Punctuation
			CompoundPunctStart
			CR
			Done
			Bad
		End Enum

		<Flags>
		Private Enum CharFlags As UShort
			White = 1
			Letter = 2
			IdentOnly = 4
			TypeChar = 8
			Punct = 16
			CompoundPunctStart = 32
			CR = 64
			LF = 128
			Digit = 256
			Complex = 512
		End Enum

		Friend Class ConditionalState
			Private ReadOnly _branchTaken As Scanner.ConditionalState.BranchTakenState

			Private ReadOnly _elseSeen As Boolean

			Private ReadOnly _ifDirective As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax

			Friend ReadOnly Property BranchTaken As Scanner.ConditionalState.BranchTakenState
				Get
					Return Me._branchTaken
				End Get
			End Property

			Friend ReadOnly Property ElseSeen As Boolean
				Get
					Return Me._elseSeen
				End Get
			End Property

			Friend ReadOnly Property IfDirective As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax
				Get
					Return Me._ifDirective
				End Get
			End Property

			Friend Sub New(ByVal branchTaken As Scanner.ConditionalState.BranchTakenState, ByVal elseSeen As Boolean, ByVal ifDirective As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax)
				MyBase.New()
				Me._branchTaken = branchTaken
				Me._elseSeen = elseSeen
				Me._ifDirective = ifDirective
			End Sub

			Public Enum BranchTakenState As Byte
				NotTaken
				Taken
				AlreadyTaken
			End Enum
		End Class

		Private Structure LineBufferAndEndOfTerminatorOffsets
			Private ReadOnly _scanner As Scanner

			Private ReadOnly _lineBufferOffset As Integer

			Private ReadOnly _endOfTerminatorTrivia As Integer

			Public Sub New(ByVal scanner As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Scanner)
				Me = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Scanner.LineBufferAndEndOfTerminatorOffsets() With
				{
					._scanner = scanner,
					._lineBufferOffset = scanner._lineBufferOffset,
					._endOfTerminatorTrivia = scanner._endOfTerminatorTrivia
				}
			End Sub

			Public Sub Restore()
				Me._scanner._lineBufferOffset = Me._lineBufferOffset
				Me._scanner._endOfTerminatorTrivia = Me._endOfTerminatorTrivia
			End Sub
		End Structure

		Private Enum NumericLiteralKind
			Integral
			Float
			[Decimal]
		End Enum

		Private Class Page
			Friend _pageStart As Integer

			Friend ReadOnly _arr As Char()

			Private ReadOnly _pool As ObjectPool(Of Scanner.Page)

			Private ReadOnly Shared s_poolInstance As ObjectPool(Of Scanner.Page)

			Shared Sub New()
				Scanner.Page.s_poolInstance = Scanner.Page.CreatePool()
			End Sub

			Private Sub New(ByVal pool As ObjectPool(Of Scanner.Page))
				MyBase.New()
				Me._pageStart = -1
				ReDim Me._arr(2047)
				Me._pool = pool
			End Sub

			Private Shared Function CreatePool() As ObjectPool(Of Scanner.Page)
				Dim objectPool As ObjectPool(Of Scanner.Page) = Nothing
				objectPool = New ObjectPool(Of Scanner.Page)(Function() New Scanner.Page(Me.$VB$Local_pool), 128)
				Return objectPool
			End Function

			Friend Sub Free()
				Me._pageStart = -1
				Me._pool.Free(Me)
			End Sub

			Friend Shared Function GetInstance() As Scanner.Page
				Return Scanner.Page.s_poolInstance.Allocate()
			End Function
		End Class

		Friend NotInheritable Class PreprocessorState
			Private ReadOnly _symbols As ImmutableDictionary(Of String, CConst)

			Private ReadOnly _conditionals As ImmutableStack(Of Scanner.ConditionalState)

			Private ReadOnly _regionDirectives As ImmutableStack(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax)

			Private ReadOnly _haveSeenRegionDirectives As Boolean

			Private ReadOnly _externalSourceDirective As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax

			Friend ReadOnly Property ConditionalStack As ImmutableStack(Of Scanner.ConditionalState)
				Get
					Return Me._conditionals
				End Get
			End Property

			Friend ReadOnly Property ExternalSourceDirective As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax
				Get
					Return Me._externalSourceDirective
				End Get
			End Property

			Friend ReadOnly Property HaveSeenRegionDirectives As Boolean
				Get
					Return Me._haveSeenRegionDirectives
				End Get
			End Property

			Friend ReadOnly Property RegionDirectiveStack As ImmutableStack(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax)
				Get
					Return Me._regionDirectives
				End Get
			End Property

			Friend ReadOnly Property SymbolsMap As ImmutableDictionary(Of String, CConst)
				Get
					Return Me._symbols
				End Get
			End Property

			Friend Sub New(ByVal symbols As ImmutableDictionary(Of String, CConst))
				MyBase.New()
				Me._symbols = symbols
				Me._conditionals = ImmutableStack.Create(Of Scanner.ConditionalState)()
				Me._regionDirectives = ImmutableStack.Create(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax)()
			End Sub

			Private Sub New(ByVal symbols As ImmutableDictionary(Of String, CConst), ByVal conditionals As ImmutableStack(Of Scanner.ConditionalState), ByVal regionDirectives As ImmutableStack(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax), ByVal haveSeenRegionDirectives As Boolean, ByVal externalSourceDirective As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax)
				MyBase.New()
				Me._symbols = symbols
				Me._conditionals = conditionals
				Me._regionDirectives = regionDirectives
				Me._haveSeenRegionDirectives = haveSeenRegionDirectives
				Me._externalSourceDirective = externalSourceDirective
			End Sub

			Friend Function InterpretConstDirective(ByRef statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax) As Scanner.PreprocessorState
				Dim constDirectiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstDirectiveTriviaSyntax = DirectCast(statement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstDirectiveTriviaSyntax)
				Dim cConst As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst = ExpressionEvaluator.EvaluateExpression(constDirectiveTriviaSyntax.Value, Me._symbols)
				Dim errorId As ERRID = cConst.ErrorId
				If (errorId <> ERRID.ERR_None) Then
					statement = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax)(statement, errorId, cConst.ErrorArgs)
				End If
				Return Me.SetSymbol(constDirectiveTriviaSyntax.Name.IdentifierText, cConst)
			End Function

			Friend Function InterpretElseDirective(ByRef statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax) As Scanner.PreprocessorState
				Dim preprocessorState As Scanner.PreprocessorState
				Dim conditionalStates As ImmutableStack(Of Scanner.ConditionalState) = Me._conditionals
				If (conditionalStates.Count() <> 0) Then
					Dim conditionalState As Scanner.ConditionalState = conditionalStates.Peek()
					conditionalStates = conditionalStates.Pop()
					If (conditionalState.ElseSeen) Then
						statement = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax)(statement, ERRID.ERR_LbElseNoMatchingIf)
					End If
					Dim branchTaken As Scanner.ConditionalState.BranchTakenState = conditionalState.BranchTaken
					If (branchTaken = Scanner.ConditionalState.BranchTakenState.Taken) Then
						branchTaken = Scanner.ConditionalState.BranchTakenState.AlreadyTaken
					ElseIf (branchTaken = Scanner.ConditionalState.BranchTakenState.NotTaken) Then
						branchTaken = Scanner.ConditionalState.BranchTakenState.Taken
					End If
					conditionalState = New Scanner.ConditionalState(branchTaken, True, conditionalState.IfDirective)
					preprocessorState = Me.WithConditionals(conditionalStates.Push(conditionalState))
				Else
					statement = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax)(statement, ERRID.ERR_LbElseNoMatchingIf)
					preprocessorState = Me.WithConditionals(conditionalStates.Push(New Scanner.ConditionalState(Scanner.ConditionalState.BranchTakenState.Taken, True, Nothing)))
				End If
				Return preprocessorState
			End Function

			Friend Function InterpretElseIfDirective(ByRef statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax) As Scanner.PreprocessorState
				Dim conditionalState As Scanner.ConditionalState
				Dim conditionalStates As ImmutableStack(Of Scanner.ConditionalState) = Me._conditionals
				If (conditionalStates.Count() <> 0) Then
					conditionalState = conditionalStates.Peek()
					conditionalStates = conditionalStates.Pop()
					If (conditionalState.ElseSeen) Then
						statement = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax)(statement, ERRID.ERR_LbElseifAfterElse)
					End If
				Else
					statement = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax)(statement, ERRID.ERR_LbBadElseif)
					conditionalState = New Scanner.ConditionalState(Scanner.ConditionalState.BranchTakenState.NotTaken, False, Nothing)
				End If
				Dim cConst As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst = ExpressionEvaluator.EvaluateCondition(DirectCast(statement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax).Condition, Me._symbols)
				Dim errorId As ERRID = cConst.ErrorId
				If (errorId <> ERRID.ERR_None) Then
					statement = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax)(statement, errorId, cConst.ErrorArgs)
				End If
				Dim branchTaken As Scanner.ConditionalState.BranchTakenState = conditionalState.BranchTaken
				If (branchTaken = Scanner.ConditionalState.BranchTakenState.Taken) Then
					branchTaken = Scanner.ConditionalState.BranchTakenState.AlreadyTaken
				ElseIf (branchTaken = Scanner.ConditionalState.BranchTakenState.NotTaken AndAlso Not cConst.IsBad AndAlso cConst.IsBooleanTrue) Then
					branchTaken = Scanner.ConditionalState.BranchTakenState.Taken
				End If
				conditionalState = New Scanner.ConditionalState(branchTaken, conditionalState.ElseSeen, DirectCast(statement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax))
				Return Me.WithConditionals(conditionalStates.Push(conditionalState))
			End Function

			Friend Function InterpretEndExternalSourceDirective(ByRef statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax) As Scanner.PreprocessorState
				Dim preprocessorState As Scanner.PreprocessorState
				If (Me._externalSourceDirective IsNot Nothing) Then
					preprocessorState = Me.WithExternalSource(Nothing)
				Else
					statement = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax)(statement, ERRID.ERR_EndExternalSource)
					preprocessorState = Me
				End If
				Return preprocessorState
			End Function

			Friend Function InterpretEndIfDirective(ByRef statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax) As Scanner.PreprocessorState
				Dim preprocessorState As Scanner.PreprocessorState
				If (Me._conditionals.Count() <> 0) Then
					preprocessorState = Me.WithConditionals(Me._conditionals.Pop())
				Else
					statement = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax)(statement, ERRID.ERR_LbNoMatchingIf)
					preprocessorState = Me
				End If
				Return preprocessorState
			End Function

			Friend Function InterpretEndRegionDirective(ByRef statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax) As Scanner.PreprocessorState
				Dim preprocessorState As Scanner.PreprocessorState
				If (Me._regionDirectives.Count() <> 0) Then
					preprocessorState = Me.WithRegions(Me._regionDirectives.Pop())
				Else
					statement = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax)(statement, ERRID.ERR_EndRegionNoRegion)
					preprocessorState = Me
				End If
				Return preprocessorState
			End Function

			Friend Function InterpretExternalSourceDirective(ByRef statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax) As Scanner.PreprocessorState
				Dim preprocessorState As Scanner.PreprocessorState
				Dim externalSourceDirectiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax = DirectCast(statement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax)
				If (Me._externalSourceDirective Is Nothing) Then
					preprocessorState = Me.WithExternalSource(externalSourceDirectiveTriviaSyntax)
				Else
					statement = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax)(statement, ERRID.ERR_NestedExternalSource)
					preprocessorState = Me
				End If
				Return preprocessorState
			End Function

			Friend Function InterpretIfDirective(ByRef statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax) As Scanner.PreprocessorState
				Dim cConst As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst = ExpressionEvaluator.EvaluateCondition(DirectCast(statement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax).Condition, Me._symbols)
				Dim errorId As ERRID = cConst.ErrorId
				If (errorId <> ERRID.ERR_None) Then
					statement = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax)(statement, errorId, cConst.ErrorArgs)
				End If
				Dim branchTakenState As Scanner.ConditionalState.BranchTakenState = If(cConst.IsBad OrElse cConst.IsBooleanTrue, Scanner.ConditionalState.BranchTakenState.Taken, Scanner.ConditionalState.BranchTakenState.NotTaken)
				Return Me.WithConditionals(Me._conditionals.Push(New Scanner.ConditionalState(branchTakenState, False, DirectCast(statement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax))))
			End Function

			Friend Function InterpretRegionDirective(ByRef statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax) As Scanner.PreprocessorState
				Dim regionDirectiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax = DirectCast(statement, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax)
				Return Me.WithRegions(Me._regionDirectives.Push(regionDirectiveTriviaSyntax))
			End Function

			Friend Function IsEquivalentTo(ByVal other As Scanner.PreprocessorState) As Boolean
				Dim flag As Boolean
				If (Me._conditionals.Count() > 0 OrElse Me._symbols.Count > 0 OrElse Me._externalSourceDirective IsNot Nothing OrElse other._conditionals.Count() > 0 OrElse other._symbols.Count > 0 OrElse other._externalSourceDirective IsNot Nothing) Then
					flag = False
				ElseIf (Me._regionDirectives.Count() = other._regionDirectives.Count()) Then
					flag = If(Me._haveSeenRegionDirectives = other._haveSeenRegionDirectives, True, False)
				Else
					flag = False
				End If
				Return flag
			End Function

			Private Function SetSymbol(ByVal name As String, ByVal value As CConst) As Scanner.PreprocessorState
				Dim strs As ImmutableDictionary(Of String, CConst) = Me._symbols.SetItem(name, value)
				Return New Scanner.PreprocessorState(strs, Me._conditionals, Me._regionDirectives, Me._haveSeenRegionDirectives, Me._externalSourceDirective)
			End Function

			Private Function WithConditionals(ByVal conditionals As ImmutableStack(Of Scanner.ConditionalState)) As Scanner.PreprocessorState
				Return New Scanner.PreprocessorState(Me._symbols, conditionals, Me._regionDirectives, Me._haveSeenRegionDirectives, Me._externalSourceDirective)
			End Function

			Private Function WithExternalSource(ByVal externalSource As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax) As Scanner.PreprocessorState
				Return New Scanner.PreprocessorState(Me._symbols, Me._conditionals, Me._regionDirectives, Me._haveSeenRegionDirectives, externalSource)
			End Function

			Private Function WithRegions(ByVal regions As ImmutableStack(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax)) As Scanner.PreprocessorState
				Return New Scanner.PreprocessorState(Me._symbols, Me._conditionals, regions, If(Me._haveSeenRegionDirectives, True, regions.Count() > 0), Me._externalSourceDirective)
			End Function
		End Class

		Public Structure QuickScanResult
			Public ReadOnly Chars As Char()

			Public ReadOnly Start As Integer

			Public ReadOnly Length As Integer

			Public ReadOnly HashCode As Integer

			Public ReadOnly TerminatorLength As Byte

			Public ReadOnly Property Succeeded As Boolean
				Get
					Return Me.Length > 0
				End Get
			End Property

			Public Sub New(ByVal start As Integer, ByVal length As Integer, ByVal chars As Char(), ByVal hashCode As Integer, ByVal terminatorLength As Byte)
				Me = New Scanner.QuickScanResult() With
				{
					.Start = start,
					.Length = length,
					.Chars = chars,
					.HashCode = hashCode,
					.TerminatorLength = terminatorLength
				}
			End Sub
		End Structure

		Friend Structure RestorePoint
			Private ReadOnly _scanner As Scanner

			Private ReadOnly _currentToken As Scanner.ScannerToken

			Private ReadOnly _prevToken As Scanner.ScannerToken

			Private ReadOnly _tokens As Scanner.ScannerToken()

			Private ReadOnly _lineBufferOffset As Integer

			Private ReadOnly _endOfTerminatorTrivia As Integer

			Private ReadOnly _scannerPreprocessorState As Scanner.PreprocessorState

			Friend Sub New(ByVal scanner As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Scanner)
				Me = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Scanner.RestorePoint() With
				{
					._scanner = scanner,
					._currentToken = scanner._currentToken,
					._prevToken = scanner._prevToken,
					._tokens = scanner.SaveAndClearTokens(),
					._lineBufferOffset = scanner._lineBufferOffset,
					._endOfTerminatorTrivia = scanner._endOfTerminatorTrivia,
					._scannerPreprocessorState = scanner._scannerPreprocessorState
				}
			End Sub

			Friend Sub Restore()
				Me._scanner._currentToken = Me._currentToken
				Me._scanner._prevToken = Me._prevToken
				Me._scanner.RestoreTokens(Me._tokens)
				Me._scanner._lineBufferOffset = Me._lineBufferOffset
				Me._scanner._endOfTerminatorTrivia = Me._endOfTerminatorTrivia
				Me._scanner._scannerPreprocessorState = Me._scannerPreprocessorState
			End Sub

			Friend Sub RestoreTokens(ByVal includeLookAhead As Boolean)
				Dim scannerTokenArray As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Scanner.ScannerToken()
				Me._scanner._currentToken = Me._currentToken
				Me._scanner._prevToken = Me._prevToken
				Dim scanner As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Scanner = Me._scanner
				If (includeLookAhead) Then
					scannerTokenArray = Me._tokens
				Else
					scannerTokenArray = Nothing
				End If
				scanner.RestoreTokens(scannerTokenArray)
			End Sub
		End Structure

		Protected Structure ScannerToken
			Public ReadOnly InnerTokenObject As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken

			Public ReadOnly Position As Integer

			Public ReadOnly EndOfTerminatorTrivia As Integer

			Public ReadOnly State As ScannerState

			Public ReadOnly PreprocessorState As Scanner.PreprocessorState

			Friend Sub New(ByVal preprocessorState As Scanner.PreprocessorState, ByVal lineBufferOffset As Integer, ByVal endOfTerminatorTrivia As Integer, ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal state As ScannerState)
				Me = New Scanner.ScannerToken() With
				{
					.PreprocessorState = preprocessorState,
					.Position = lineBufferOffset,
					.EndOfTerminatorTrivia = endOfTerminatorTrivia,
					.InnerTokenObject = token,
					.State = state
				}
			End Sub

			Friend Function [With](ByVal state As ScannerState, ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Scanner.ScannerToken
				Return New Scanner.ScannerToken(Me.PreprocessorState, Me.Position, Me.EndOfTerminatorTrivia, token, state)
			End Function

			Friend Function [With](ByVal preprocessorState As Scanner.PreprocessorState) As Scanner.ScannerToken
				Return New Scanner.ScannerToken(preprocessorState, Me.Position, Me.EndOfTerminatorTrivia, Me.InnerTokenObject, Me.State)
			End Function
		End Structure

		Private Delegate Function ScanTriviaFunc() As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)

		Private Structure TokenParts
			Friend ReadOnly spelling As String

			Friend ReadOnly pTrivia As GreenNode

			Friend ReadOnly fTrivia As GreenNode

			Friend Sub New(ByVal pTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal fTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal spelling As String)
				Me = New Scanner.TokenParts() With
				{
					.spelling = spelling,
					.pTrivia = pTrivia.Node,
					.fTrivia = fTrivia.Node
				}
			End Sub
		End Structure

		Private Structure TriviaKey
			Public ReadOnly spelling As String

			Public ReadOnly kind As SyntaxKind

			Public Sub New(ByVal spelling As String, ByVal kind As SyntaxKind)
				Me = New Scanner.TriviaKey() With
				{
					.spelling = spelling,
					.kind = kind
				}
			End Sub
		End Structure

		Friend Structure XmlCharResult
			Friend ReadOnly Length As Integer

			Friend ReadOnly Char1 As Char

			Friend ReadOnly Char2 As Char

			Friend Sub New(ByVal ch As Char)
				Me = New Scanner.XmlCharResult() With
				{
					.Length = 1,
					.Char1 = ch
				}
			End Sub

			Friend Sub New(ByVal ch1 As Char, ByVal ch2 As Char)
				Me = New Scanner.XmlCharResult() With
				{
					.Length = 2,
					.Char1 = ch1,
					.Char2 = ch2
				}
			End Sub

			Friend Sub AppendTo(ByVal list As StringBuilder)
				list.Append(Me.Char1)
				If (Me.Length = 2) Then
					list.Append(Me.Char2)
				End If
			End Sub
		End Structure
	End Class
End Namespace