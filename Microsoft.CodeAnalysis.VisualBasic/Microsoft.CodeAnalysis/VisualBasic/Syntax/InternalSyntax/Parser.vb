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
Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend Class Parser
		Implements ISyntaxFactoryContext, IDisposable
		Private _allowLeadingMultilineTrivia As Boolean

		Private _hadImplicitLineContinuation As Boolean

		Private _hadLineContinuationComment As Boolean

		Private _possibleFirstStatementOnLine As Parser.PossibleFirstStatementKind

		Private _recursionDepth As Integer

		Private _evaluatingConditionCompilationExpression As Boolean

		Private ReadOnly _scanner As Scanner

		Private ReadOnly _cancellationToken As CancellationToken

		Friend ReadOnly _pool As SyntaxListPool

		Private ReadOnly _syntaxFactory As ContextAwareSyntaxFactory

		Private ReadOnly _disposeScanner As Boolean

		Private _context As BlockContext

		Private _isInMethodDeclarationHeader As Boolean

		Private _isInAsyncMethodDeclarationHeader As Boolean

		Private _isInIteratorMethodDeclarationHeader As Boolean

		Private _currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken

		Private ReadOnly Shared s_isTokenOrKeywordFunc As Func(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, SyntaxKind(), Boolean)

		Friend ReadOnly Property Context As BlockContext
			Get
				Return Me._context
			End Get
		End Property

		Friend ReadOnly Property CurrentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Get
				Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me._currentToken
				If (syntaxToken Is Nothing) Then
					syntaxToken = Me._scanner.GetCurrentToken()
					Me._allowLeadingMultilineTrivia = False
					Me._currentToken = syntaxToken
				End If
				Return syntaxToken
			End Get
		End Property

		Friend ReadOnly Property IsScript As Boolean
			Get
				Return Me._scanner.Options.Kind = SourceCodeKind.Script
			End Get
		End Property

		Public ReadOnly Property IsWithinAsyncMethodOrLambda As Boolean Implements ISyntaxFactoryContext.IsWithinAsyncMethodOrLambda
			Get
				If (Me._isInMethodDeclarationHeader) Then
					Return Me._isInAsyncMethodDeclarationHeader
				End If
				Return Me.Context.IsWithinAsyncMethodOrLambda
			End Get
		End Property

		Public ReadOnly Property IsWithinIteratorContext As Boolean Implements ISyntaxFactoryContext.IsWithinIteratorContext
			Get
				If (Me._isInMethodDeclarationHeader) Then
					Return Me._isInIteratorMethodDeclarationHeader
				End If
				Return Me.Context.IsWithinIteratorMethodOrLambdaOrProperty
			End Get
		End Property

		Private ReadOnly Property PrevToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Get
				Return Me._scanner.PrevToken
			End Get
		End Property

		Friend ReadOnly Property SyntaxFactory As ContextAwareSyntaxFactory
			Get
				Return Me._syntaxFactory
			End Get
		End Property

		Shared Sub New()
			Parser.s_isTokenOrKeywordFunc = New Func(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, SyntaxKind(), Boolean)(AddressOf Parser.IsTokenOrKeyword)
		End Sub

		Friend Sub New(ByVal text As SourceText, ByVal options As VisualBasicParseOptions, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing)
			MyClass.New(New Scanner(text, options, False))
			Me._disposeScanner = True
			Me._cancellationToken = cancellationToken
		End Sub

		Friend Sub New(ByVal scanner As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Scanner)
			MyBase.New()
			Me._allowLeadingMultilineTrivia = True
			Me._hadImplicitLineContinuation = False
			Me._hadLineContinuationComment = False
			Me._possibleFirstStatementOnLine = Parser.PossibleFirstStatementKind.Yes
			Me._pool = New SyntaxListPool()
			Me._context = Nothing
			Me._scanner = scanner
			Me._context = New CompilationUnitContext(Me)
			Me._syntaxFactory = New ContextAwareSyntaxFactory(Me)
		End Sub

		Private Shared Function AdjustTriviaForMissingTokens(Of T As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal node As T) As T
			Dim t1 As T
			If (node.ContainsDiagnostics) Then
				t1 = If(node.GetLastTerminal().FullWidth = 0, Parser.AdjustTriviaForMissingTokensCore(Of T)(node), node)
			Else
				t1 = node
			End If
			Return t1
		End Function

		Private Shared Function AdjustTriviaForMissingTokensCore(Of T As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal node As T) As T
			Dim variable As Parser._Closure$__111-0(Of T) = Nothing
			Dim t1 As T
			variable = New Parser._Closure$__111-0(Of T)(variable)
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = node.CreateRed(Nothing, 0)
			variable.$VB$Local_lastNonZeroWidthToken = Parser.GetLastNZWToken(syntaxNode)
			variable.$VB$Local_lastZeroWidthToken = Parser.GetLastToken(syntaxNode)
			Dim trailingTrivia As SyntaxTriviaList = variable.$VB$Local_lastNonZeroWidthToken.TrailingTrivia
			Dim count As Integer = trailingTrivia.Count
			Dim enumerator As SyntaxTriviaList.Enumerator = trailingTrivia.GetEnumerator()
			While enumerator.MoveNext() AndAlso enumerator.Current.Kind() = SyntaxKind.WhitespaceTrivia
				count = count - 1
			End While
			If (count <> 0) Then
				Dim syntaxTriviaArray(trailingTrivia.Count - count - 1 + 1 - 1) As Microsoft.CodeAnalysis.SyntaxTrivia
				trailingTrivia.CopyTo(0, syntaxTriviaArray, 0, CInt(syntaxTriviaArray.Length))
				variable.$VB$Local_nonZwTokenReplacement = variable.$VB$Local_lastNonZeroWidthToken.WithTrailingTrivia(syntaxTriviaArray)
				Dim syntaxTriviaLists As SyntaxTriviaList = variable.$VB$Local_lastZeroWidthToken.TrailingTrivia
				Dim syntaxTriviaArray1(count + syntaxTriviaLists.Count - 1 + 1 - 1) As Microsoft.CodeAnalysis.SyntaxTrivia
				trailingTrivia.CopyTo(trailingTrivia.Count - count, syntaxTriviaArray1, 0, count)
				syntaxTriviaLists.CopyTo(0, syntaxTriviaArray1, count, syntaxTriviaLists.Count)
				variable.$VB$Local_lastTokenReplacement = variable.$VB$Local_lastZeroWidthToken.WithTrailingTrivia(syntaxTriviaArray1)
				syntaxNode = syntaxNode.ReplaceTokens(DirectCast((New Microsoft.CodeAnalysis.SyntaxToken() { variable.$VB$Local_lastNonZeroWidthToken, variable.$VB$Local_lastZeroWidthToken }), IEnumerable(Of Microsoft.CodeAnalysis.SyntaxToken)), Function(oldToken As Microsoft.CodeAnalysis.SyntaxToken, newToken As Microsoft.CodeAnalysis.SyntaxToken) If(oldToken <> Me.$VB$Local_lastNonZeroWidthToken, If(oldToken <> Me.$VB$Local_lastZeroWidthToken, newToken, Me.$VB$Local_lastTokenReplacement), Me.$VB$Local_nonZwTokenReplacement))
				node = DirectCast(syntaxNode.Green, T)
				t1 = node
			Else
				t1 = node
			End If
			Return t1
		End Function

		Private Function BeginsGeneric(Optional ByVal nonArrayName As Boolean = False, Optional ByVal allowGenericsWithoutOf As Boolean = False) As Boolean
			Dim flag As Boolean
			If (Me.CurrentToken.Kind = SyntaxKind.OpenParenToken) Then
				If (Not nonArrayName) Then
					Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.PeekPastStatementTerminator()
					If (syntaxToken.Kind = SyntaxKind.OfKeyword) Then
						flag = True
						Return flag
					ElseIf (allowGenericsWithoutOf AndAlso SyntaxFacts.IsPredefinedTypeOrVariant(syntaxToken.Kind)) Then
						Dim kind As SyntaxKind = Me.PeekToken(2).Kind
						If (kind <> SyntaxKind.CommaToken AndAlso kind <> SyntaxKind.CloseParenToken) Then
							flag = False
							Return flag
						End If
						flag = True
						Return flag
					End If
				Else
					flag = True
					Return flag
				End If
			End If
			flag = False
			Return flag
		End Function

		Private Function CanEndExecutableStatement(ByVal t As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Boolean
			If (Me.CanFollowStatement(t)) Then
				Return True
			End If
			Return t.Kind = SyntaxKind.ElseKeyword
		End Function

		Private Function CanFollowExpression(ByVal t As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
			If (t.Kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierToken OrElse Not Parser.TryIdentifierAsContextualKeyword(t, syntaxKind)) Then
				flag = If(KeywordTable.CanFollowExpression(t.Kind), True, Me.IsValidStatementTerminator(t))
			Else
				flag = KeywordTable.CanFollowExpression(syntaxKind)
			End If
			Return flag
		End Function

		Private Function CanFollowStatement(ByVal T As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Boolean
			If (If(Me.Context.IsWithinSingleLineLambda, Me.CanFollowExpression(T), Me.IsValidStatementTerminator(T))) Then
				Return True
			End If
			Return T.Kind = SyntaxKind.ElseKeyword
		End Function

		Friend Function CanFollowStatementButIsNotSelectFollowingExpression(ByVal nextToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Boolean
			Dim flag As Boolean
			If (Me.Context.IsWithinSingleLineLambda) Then
				flag = If(Not Me.CanFollowExpression(nextToken), False, nextToken.Kind <> SyntaxKind.SelectKeyword)
			Else
				flag = Me.IsValidStatementTerminator(nextToken)
			End If
			If (flag) Then
				Return True
			End If
			If (Not Me.Context.IsLineIf) Then
				Return False
			End If
			Return nextToken.Kind = SyntaxKind.ElseKeyword
		End Function

		Private Function CanStartConsequenceExpression(ByVal kind As SyntaxKind, ByVal qualified As Boolean) As Boolean
			If (kind = SyntaxKind.DotToken OrElse kind = SyntaxKind.ExclamationToken) Then
				Return True
			End If
			If (Not qualified) Then
				Return False
			End If
			Return kind = SyntaxKind.OpenParenToken
		End Function

		Private Shared Function CanTokenStartTypeName(ByVal Token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Boolean
			Dim flag As Boolean
			If (Not SyntaxFacts.IsPredefinedTypeOrVariant(Token.Kind)) Then
				Dim kind As SyntaxKind = Token.Kind
				flag = If(kind = SyntaxKind.GlobalKeyword OrElse kind = SyntaxKind.IdentifierToken, True, False)
			Else
				flag = True
			End If
			Return flag
		End Function

		Private Shared Function CanUseInTryGetToken(ByVal kind As SyntaxKind) As Boolean
			If (SyntaxFacts.IsTerminator(kind)) Then
				Return False
			End If
			Return kind <> SyntaxKind.EmptyToken
		End Function

		Private Function CheckFeatureAvailability(Of TNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal feature As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature, ByVal node As TNode) As TNode
			Return Parser.CheckFeatureAvailability(Of TNode)(feature, node, Me._scanner.Options.LanguageVersion)
		End Function

		Friend Shared Function CheckFeatureAvailability(Of TNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal feature As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature, ByVal node As TNode, ByVal languageVersion As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion) As TNode
			Dim tNode1 As TNode
			tNode1 = If(Not Parser.CheckFeatureAvailability(languageVersion, feature), Parser.ReportFeatureUnavailable(Of TNode)(feature, node, languageVersion), node)
			Return tNode1
		End Function

		Friend Function CheckFeatureAvailability(ByVal feature As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature) As Boolean
			Return Parser.CheckFeatureAvailability(Me._scanner.Options.LanguageVersion, feature)
		End Function

		Friend Shared Function CheckFeatureAvailability(ByVal languageVersion As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion, ByVal feature As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature) As Boolean
			Return feature.GetLanguageVersion() <= languageVersion
		End Function

		Friend Shared Function CheckFeatureAvailability(ByVal diagnosticsOpt As DiagnosticBag, ByVal location As Microsoft.CodeAnalysis.Location, ByVal languageVersion As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion, ByVal feature As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature) As Boolean
			Dim flag As Boolean
			If (Parser.CheckFeatureAvailability(languageVersion, feature)) Then
				flag = True
			Else
				If (diagnosticsOpt IsNot Nothing) Then
					Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(feature.GetResourceId())
					Dim visualBasicRequiredLanguageVersion As Microsoft.CodeAnalysis.VisualBasic.VisualBasicRequiredLanguageVersion = New Microsoft.CodeAnalysis.VisualBasic.VisualBasicRequiredLanguageVersion(feature.GetLanguageVersion())
					diagnosticsOpt.Add(ERRID.ERR_LanguageVersion, location, New [Object]() { languageVersion.GetErrorName(), diagnosticInfo, visualBasicRequiredLanguageVersion })
				End If
				flag = False
			End If
			Return flag
		End Function

		Friend Shared Function CheckFeatureAvailability(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal location As Microsoft.CodeAnalysis.Location, ByVal languageVersion As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion, ByVal feature As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature) As Boolean
			Return Parser.CheckFeatureAvailability(diagnostics.DiagnosticBag, location, languageVersion, feature)
		End Function

		Private Function CheckForEndOfExpression(Of T As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByRef syntax As T) As Boolean
			Dim flag As Boolean
			If (Me.CurrentToken.IsBinaryOperator() OrElse Me.CurrentToken.Kind = SyntaxKind.DotToken) Then
				syntax = Parser.ReportSyntaxError(Of T)(syntax, ERRID.ERR_ExpectedEndOfExpression)
				flag = False
			Else
				flag = True
			End If
			Return flag
		End Function

		Friend Sub ConsumeColonInSingleLineExpression()
			Me.ConsumedStatementTerminator(False)
			Me.GetNextToken(ScannerState.VB)
		End Sub

		Friend Sub ConsumedStatementTerminator(ByVal allowLeadingMultilineTrivia As Boolean)
			Me.ConsumedStatementTerminator(allowLeadingMultilineTrivia, If(allowLeadingMultilineTrivia, Parser.PossibleFirstStatementKind.Yes, Parser.PossibleFirstStatementKind.No))
		End Sub

		Private Sub ConsumedStatementTerminator(ByVal allowLeadingMultilineTrivia As Boolean, ByVal possibleFirstStatementOnLine As Parser.PossibleFirstStatementKind)
			Me._allowLeadingMultilineTrivia = allowLeadingMultilineTrivia
			Me._possibleFirstStatementOnLine = possibleFirstStatementOnLine
		End Sub

		Friend Sub ConsumeStatementTerminator(ByVal colonAsSeparator As Boolean)
			Dim kind As SyntaxKind = Me.CurrentToken.Kind
			If (kind = SyntaxKind.ColonToken) Then
				If (colonAsSeparator) Then
					Me.ConsumedStatementTerminator(False)
					Me.GetNextToken(ScannerState.VB)
					Return
				End If
				Me.ConsumedStatementTerminator(True, Parser.PossibleFirstStatementKind.IfPrecededByLineBreak)
				Me.GetNextToken(ScannerState.VB)
			Else
				If (kind = SyntaxKind.StatementTerminatorToken) Then
					Me.ConsumedStatementTerminator(True)
					Me.GetNextToken(ScannerState.VB)
					Return
				End If
				If (kind = SyntaxKind.EndOfFileToken) Then
					Me.ConsumedStatementTerminator(True)
					Return
				End If
			End If
		End Sub

		Friend Function ConsumeStatementTerminatorAfterDirective(ByRef stmt As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax
			If (Me.CurrentToken.Kind <> SyntaxKind.StatementTerminatorToken OrElse Me.CurrentToken.HasLeadingTrivia) Then
				Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = Me.ResyncAndConsumeStatementTerminator()
				If (syntaxList.Node IsNot Nothing) Then
					If (stmt.Kind = SyntaxKind.BadDirectiveTrivia) Then
						stmt = stmt.AddTrailingSyntax(syntaxList)
					Else
						stmt = stmt.AddTrailingSyntax(syntaxList, ERRID.ERR_ExpectedEOS)
					End If
				End If
			Else
				Me.GetNextToken(ScannerState.VB)
			End If
			Return stmt
		End Function

		Friend Function ConsumeUnexpectedTokens(Of TNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal node As TNode) As TNode
			Dim tNode1 As TNode
			If (Me.CurrentToken.Kind <> SyntaxKind.EndOfFileToken) Then
				Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken).Create()
				While Me.CurrentToken.Kind <> SyntaxKind.EndOfFileToken
					syntaxListBuilder.Add(Me.CurrentToken)
					Me.GetNextToken(ScannerState.VB)
				End While
				tNode1 = node.AddTrailingSyntax(syntaxListBuilder.ToList(), ERRID.ERR_Syntax)
			Else
				tNode1 = node
			End If
			Return tNode1
		End Function

		Private Function CreateForInsufficientStack(Of TNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByRef restorePoint As Scanner.RestorePoint, ByVal result As TNode) As TNode
			restorePoint.Restore()
			Me.GetNextToken(ScannerState.VB)
			Dim syntaxListBuilder As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxListBuilder = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxListBuilder(4)
			While Me.CurrentToken.Kind <> SyntaxKind.EndOfFileToken
				syntaxListBuilder.Add(Me.CurrentToken)
				Me.GetNextToken(ScannerState.VB)
			End While
			Return result.AddLeadingSyntax(syntaxListBuilder.ToList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(), ERRID.ERR_TooLongOrComplexExpression)
		End Function

		Private Function CreateMissingXmlAttribute() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax
			Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = DirectCast(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.XmlNameToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.ColonToken)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.EqualsToken)
			Return Me.SyntaxFactory.XmlAttribute(Me.SyntaxFactory.XmlName(Me.SyntaxFactory.XmlPrefix(xmlNameTokenSyntax, punctuationSyntax), xmlNameTokenSyntax), punctuationSyntax1, Me.CreateMissingXmlString())
		End Function

		Private Function CreateMissingXmlString() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.DoubleQuoteToken)
			Return Me.SyntaxFactory.XmlString(punctuationSyntax, New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)(), punctuationSyntax)
		End Function

		Private Function CreateXmlElement(ByVal contexts As List(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlContext), ByVal endElement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
			Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
			Dim xmlContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlContext
			Dim num As Integer = contexts.MatchEndElement(endElement.Name)
			If (num < 0) Then
				Dim text As String = ""
				Dim str As String = ""
				Dim text1 As String = ""
				xmlContext = contexts.Peek(0)
				Dim name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax = xmlContext.StartElement.Name
				If (name.Kind = SyntaxKind.XmlName) Then
					Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax = DirectCast(name, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax)
					If (xmlNameSyntax.Prefix IsNot Nothing) Then
						text = xmlNameSyntax.Prefix.Name.Text
						str = ":"
					End If
					text1 = xmlNameSyntax.LocalName.Text
				End If
				endElement = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax)(endElement, ERRID.ERR_MismatchedXmlEndTag, New [Object]() { text, str, text1 })
				xmlContext = contexts.Peek(0)
				xmlNodeSyntax = xmlContext.CreateElement(endElement, ErrorFactory.ErrorInfo(ERRID.ERR_MissingXmlEndTag))
			Else
				Dim count As Integer = contexts.Count - 1
				Do
					Dim xmlElementEndTagSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax = Me.SyntaxFactory.XmlElementEndTag(DirectCast(Parser.HandleUnexpectedToken(SyntaxKind.LessThanSlashToken), PunctuationSyntax), Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax)(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlName(Nothing, Me.SyntaxFactory.XmlNameToken("", SyntaxKind.XmlNameToken, Nothing, Nothing)), ERRID.ERR_ExpectedXmlName), DirectCast(Parser.HandleUnexpectedToken(SyntaxKind.GreaterThanToken), PunctuationSyntax))
					xmlContext = contexts.Peek(0)
					Dim xmlNodeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax = xmlContext.CreateElement(xmlElementEndTagSyntax, ErrorFactory.ErrorInfo(ERRID.ERR_MissingXmlEndTag))
					contexts.Pop()
					If (contexts.Count <= 0) Then
						Exit Do
					End If
					contexts.Peek(0).Add(xmlNodeSyntax1)
					count = count - 1
				Loop While count > num
				If (Not endElement.IsMissing) Then
					xmlContext = contexts.Peek(0)
					xmlNodeSyntax = xmlContext.CreateElement(endElement)
				Else
					xmlContext = contexts.Peek(0)
					xmlNodeSyntax = xmlContext.CreateElement(endElement, ErrorFactory.ErrorInfo(ERRID.ERR_MissingXmlEndTag))
				End If
			End If
			contexts.Pop()
			Return xmlNodeSyntax
		End Function

		Friend Sub Dispose() Implements IDisposable.Dispose
			If (Me._disposeScanner) Then
				Me._scanner.Dispose()
			End If
		End Sub

		Private Function ElementNameIsOneFromTheList(ByVal xmlElementName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax, ByVal ParamArray names As String()) As Boolean
			Dim flag As Boolean
			If (xmlElementName Is Nothing OrElse xmlElementName.Kind <> SyntaxKind.XmlName) Then
				flag = False
			Else
				Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax = DirectCast(xmlElementName, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax)
				If (xmlNameSyntax.Prefix Is Nothing) Then
					Dim strArray As String() = names
					Dim num As Integer = 0
					While num < CInt(strArray.Length)
						Dim str As String = strArray(num)
						If (Not DocumentationCommentXmlNames.ElementEquals(xmlNameSyntax.LocalName.Text, str, True)) Then
							num = num + 1
						Else
							flag = True
							Return flag
						End If
					End While
					flag = False
				Else
					flag = False
				End If
			End If
			Return flag
		End Function

		Private Shared Function GetArgumentAsExpression(ByVal arg As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			If (arg.Kind <> SyntaxKind.SimpleArgument) Then
				expressionSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression().AddLeadingSyntax(arg, ERRID.ERR_ExpectedExpression)
			Else
				Dim simpleArgumentSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax = DirectCast(arg, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax)
				Dim expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = simpleArgumentSyntax.Expression
				If (simpleArgumentSyntax.NameColonEquals IsNot Nothing) Then
					expression = expression.AddLeadingSyntax(SyntaxList.List(simpleArgumentSyntax.NameColonEquals.Name, simpleArgumentSyntax.NameColonEquals.ColonEqualsToken), ERRID.ERR_IllegalOperandInIIFName)
				End If
				expressionSyntax = expression
			End If
			Return expressionSyntax
		End Function

		Friend Shared Function GetBinaryOperatorHelper(ByVal t As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As SyntaxKind
			Return SyntaxFacts.GetBinaryExpression(t.Kind)
		End Function

		Private Function GetClosingRightBrace() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)()
			If (Me.CurrentToken.Kind <> SyntaxKind.CloseBraceToken) Then
				syntaxList = Me.ResyncAt(New SyntaxKind() { SyntaxKind.CloseBraceToken })
			End If
			Me.TryEatNewLineAndGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CloseBraceToken, punctuationSyntax, True, ScannerState.VB)
			If (syntaxList.Node IsNot Nothing) Then
				punctuationSyntax = punctuationSyntax.AddLeadingSyntax(syntaxList, ERRID.ERR_ExpectedRbrace)
			End If
			Return punctuationSyntax
		End Function

		Private Function GetCurrentSyntaxNodeIfApplicable(<Out> ByRef curSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim linkResult As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext.LinkResult
			Dim blockContext1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext = Me._context
			Do
				curSyntaxNode = Me._scanner.GetCurrentSyntaxNode()
				If (curSyntaxNode IsNot Nothing) Then
					linkResult = If(TypeOf curSyntaxNode Is Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax OrElse curSyntaxNode.Kind = SyntaxKind.DocumentationCommentTrivia, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext.LinkResult.NotUsed, blockContext1.TryLinkSyntax(curSyntaxNode, blockContext1))
				Else
					linkResult = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext.LinkResult.NotUsed
				End If
			Loop While linkResult = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext.LinkResult.Crumble AndAlso Me._scanner.TryCrumbleOnce()
			If ((linkResult And Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext.LinkResult.Used) <> Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext.LinkResult.Used) Then
				blockContext = Nothing
			Else
				blockContext = blockContext1
			End If
			Return blockContext
		End Function

		Private Shared Function GetEndStatementKindFromKeyword(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			If (syntaxKind1 <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleKeyword) Then
				If (syntaxKind1 <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventKeyword) Then
					If (syntaxKind1 <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassKeyword) Then
						If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerKeyword) Then
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndAddHandlerStatement
						Else
							If (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassKeyword) Then
								syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
								Return syntaxKind
							End If
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement
						End If
					ElseIf (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumKeyword) Then
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement
					Else
						If (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventKeyword) Then
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
							Return syntaxKind
						End If
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement
					End If
				ElseIf (syntaxKind1 <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetKeyword) Then
					If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionKeyword) Then
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement
					Else
						If (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetKeyword) Then
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
							Return syntaxKind
						End If
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement
					End If
				ElseIf (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IfKeyword) Then
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement
				ElseIf (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceKeyword) Then
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement
				Else
					If (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleKeyword) Then
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
						Return syntaxKind
					End If
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement
				End If
			ElseIf (syntaxKind1 > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerKeyword) Then
				If (syntaxKind1 > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetKeyword) Then
					Select Case syntaxKind1
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SharedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StepKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrElseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SharedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShortKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StepKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StopKeyword
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
							Return syntaxKind
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSubStatement
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockKeyword
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSyncLockStatement
							Exit Select
						Case Else
							If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryKeyword) Then
								syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndTryStatement
								Exit Select
							Else
								Select Case syntaxKind1
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingKeyword
										syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement

									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhenKeyword
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WideningKeyword
										syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
										Return syntaxKind
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword
										syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWhileStatement

									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithKeyword
										syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement

									Case Else
										syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
										Return syntaxKind
								End Select
							End If

					End Select
				ElseIf (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword) Then
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement
				Else
					If (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetKeyword) Then
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
						Return syntaxKind
					End If
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement
				End If
			ElseIf (syntaxKind1 <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword) Then
				If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceKeyword) Then
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement
				Else
					If (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword) Then
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
						Return syntaxKind
					End If
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement
				End If
			ElseIf (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword) Then
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndPropertyStatement
			ElseIf (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventKeyword) Then
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement
			Else
				If (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerKeyword) Then
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
					Return syntaxKind
				End If
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRemoveHandlerStatement
			End If
			Return syntaxKind
		End Function

		Private Function GetLabelSyntaxForIdentifierOrLineNumber(ByVal labelName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax
			If (labelName.Kind <> SyntaxKind.IntegerLiteralToken) Then
				Return Me.SyntaxFactory.IdentifierLabel(labelName)
			End If
			Return Me.SyntaxFactory.NumericLabel(labelName)
		End Function

		Private Shared Function GetLastNZWToken(ByVal node As SyntaxNode) As Microsoft.CodeAnalysis.SyntaxToken
		Label0:
			Dim reverseds As Microsoft.CodeAnalysis.ChildSyntaxList.Reversed = node.ChildNodesAndTokens().Reverse()
			Dim enumerator As Microsoft.CodeAnalysis.ChildSyntaxList.Reversed.Enumerator = reverseds.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As SyntaxNodeOrToken = enumerator.Current
				If (current.FullWidth = 0) Then
					Continue While
				End If
				node = current.AsNode()
				If (node IsNot Nothing) Then
					GoTo Label0
				End If
				Return current.AsToken()
			End While
			Throw ExceptionUtilities.Unreachable
		End Function

		Private Shared Function GetLastToken(ByVal node As SyntaxNode) As Microsoft.CodeAnalysis.SyntaxToken
			Dim syntaxNodeOrToken As Microsoft.CodeAnalysis.SyntaxNodeOrToken
			Do
				syntaxNodeOrToken = node.ChildNodesAndTokens().Last()
				node = syntaxNodeOrToken.AsNode()
			Loop While node IsNot Nothing
			Return syntaxNodeOrToken.AsToken()
		End Function

		Friend Sub GetNextSyntaxNode()
			Me._scanner.MoveToNextSyntaxNode()
			Me._currentToken = Nothing
		End Sub

		Friend Sub GetNextToken(Optional ByVal state As ScannerState = 0)
			If (Me._allowLeadingMultilineTrivia AndAlso state = ScannerState.VB) Then
				state = ScannerState.VBAllowLeadingMultilineTrivia
			End If
			Me._scanner.GetNextTokenInState(state)
			Me._currentToken = Nothing
		End Sub

		Private Function GetTokenAsAssemblyOrModuleKeyword(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax
			If (token.Kind <> SyntaxKind.ModuleKeyword) Then
				Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
				Me.TryTokenAsContextualKeyword(token, SyntaxKind.AssemblyKeyword, keywordSyntax1)
				keywordSyntax = keywordSyntax1
			Else
				keywordSyntax = DirectCast(token, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			End If
			Return keywordSyntax
		End Function

		Private Shared Function GetUnexpectedTokenErrorId(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Microsoft.CodeAnalysis.VisualBasic.ERRID
			Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsKeyword) Then
				If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanToken) Then
					If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinKeyword) Then
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntoKeyword) Then
							eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedInto
							Return eRRID
						Else
							If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinKeyword) Then
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_Syntax
								Return eRRID
							End If
							eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedJoin
							Return eRRID
						End If
					ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WarningKeyword) Then
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedWarningKeyword
						Return eRRID
					Else
						Select Case syntaxKind
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedComma
								Return eRRID
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HashToken
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AmpersandToken
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndAddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRemoveHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSyncLockStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterSingleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterMultipleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubNewStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeywordEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsPropertyEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesClauseItem Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IncompleteMember Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FieldDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariableDeclarator Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleAsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsNewClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectMemberInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectCollectionInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferredFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionalKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrElseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverloadsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParamArrayKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PartialKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrivateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PublicKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReadOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReDimKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.REMKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ResumeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReturnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShadowsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SharedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShortKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StaticKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StepKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StopKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StringKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ThenKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ThrowKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ToKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TrueKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryCastKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeOfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UIntegerKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ULongKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UShortKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhenKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WideningKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WriteOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GosubKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariantKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WendKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AllKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnsiKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AscendingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AssemblyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AutoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BinaryKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompareKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CustomKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DescendingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DisableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DistinctKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExplicitKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalSourceKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalChecksumKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FromKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GroupKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsFalseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsTrueKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OffKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrderKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OutKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PreserveKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RegionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkipKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StrictKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TakeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TextKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UnicodeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UntilKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WarningKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhereKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsyncKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AwaitKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IteratorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.YieldKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExclamationToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AtToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HashToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AmpersandToken
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsteriskToken
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PlusToken
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ColonToken
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanEqualsToken
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanGreaterThanToken
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_Syntax
								Return eRRID
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedSQuote
								Return eRRID
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedLparen
								Return eRRID
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseParenToken
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedRparen
								Return eRRID
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedLbrace
								Return eRRID
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseBraceToken
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedRbrace
								Return eRRID
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SemicolonToken
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedSColon
								Return eRRID
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MinusToken
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedMinus
								Return eRRID
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DotToken
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedDot
								Return eRRID
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SlashToken
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedDiv
								Return eRRID
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanToken
								Exit Select
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedEQ
								Return eRRID
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanToken
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedGreater
								Return eRRID
							Case Else
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_Syntax
								Return eRRID
						End Select
					End If
				ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNameToken) Then
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoubleQuoteToken) Then
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedQuote
						Return eRRID
					Else
						Select Case syntaxKind
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanSlashToken
								Exit Select
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanExclamationMinusMinusToken
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanQuestionToken
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BeginCDataToken
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOfXmlToken
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BadToken
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_Syntax
								Return eRRID
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MinusMinusGreaterThanToken
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedXmlEndComment
								Return eRRID
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QuestionGreaterThanToken
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedXmlEndPI
								Return eRRID
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanPercentEqualsToken
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedXmlBeginEmbedded
								Return eRRID
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PercentGreaterThanToken
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedXmlEndEmbedded
								Return eRRID
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndCDataToken
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedXmlEndCData
								Return eRRID
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNameToken
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedXmlName
								Return eRRID
							Case Else
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_Syntax
								Return eRRID
						End Select
					End If
				ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierToken) Then
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedIdentifier
					Return eRRID
				ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntegerLiteralToken) Then
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedIntLiteral
					Return eRRID
				Else
					If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StringLiteralToken) Then
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_Syntax
						Return eRRID
					End If
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedStringLiteral
					Return eRRID
				End If
				eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedLT
			ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LibKeyword) Then
				If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InKeyword) Then
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsKeyword) Then
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedAs
					Else
						If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InKeyword) Then
							eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_Syntax
							Return eRRID
						End If
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedIn
					End If
				ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsKeyword) Then
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_MissingIsInTypeOf
				Else
					If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LibKeyword) Then
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_Syntax
						Return eRRID
					End If
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_MissingLibInDeclare
				End If
			ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OfKeyword) Then
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NextKeyword) Then
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_MissingNext
				Else
					If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OfKeyword) Then
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_Syntax
						Return eRRID
					End If
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_OfExpected
				End If
			ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword) Then
				eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedOn
			ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByKeyword) Then
				eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedBy
			Else
				If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsKeyword) Then
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_Syntax
					Return eRRID
				End If
				eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedEquals
			End If
			Return eRRID
		End Function

		Private Shared Function HandleUnexpectedKeyword(ByVal kind As SyntaxKind) As KeywordSyntax
			Dim unexpectedTokenErrorId As ERRID = Parser.GetUnexpectedTokenErrorId(kind)
			Return Parser.ReportSyntaxError(Of KeywordSyntax)(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(kind), unexpectedTokenErrorId)
		End Function

		Private Shared Function HandleUnexpectedToken(ByVal kind As SyntaxKind) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim unexpectedTokenErrorId As ERRID = Parser.GetUnexpectedTokenErrorId(kind)
			Return Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(kind), unexpectedTokenErrorId)
		End Function

		Private Shared Function IsAsciiColonTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Boolean
			If (node.Kind <> SyntaxKind.ColonTrivia) Then
				Return False
			End If
			Return EmbeddedOperators.CompareString(node.ToString(), ":", False) = 0
		End Function

		Private Function IsContinuableEOL(Optional ByVal i As Integer = 0) As Boolean
			Dim flag As Boolean
			flag = If(Me.PeekToken(i).Kind <> SyntaxKind.StatementTerminatorToken OrElse Me.PeekToken(i + 1).Kind = SyntaxKind.EmptyToken, False, True)
			Return flag
		End Function

		Private Function IsContinuableQueryOperator(ByVal pToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
			If (Parser.TryTokenAsKeyword(pToken, syntaxKind)) Then
				Dim flag1 As Boolean = KeywordTable.IsQueryClause(syntaxKind)
				If (flag1 AndAlso syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword AndAlso Me.PeekToken(2).Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseKeyword) Then
					flag1 = False
				End If
				flag = flag1
			Else
				flag = False
			End If
			Return flag
		End Function

		Friend Shared Function IsDeclarationStatement(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumStatement) Then
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceStatement OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleStatement) <= 4) Then
					flag = True
					Return flag
				End If
				flag = False
				Return flag
			ElseIf (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement) AndAlso CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement) > 4) Then
				flag = False
				Return flag
			End If
			flag = True
			Return flag
		End Function

		Friend Function IsFirstStatementOnLine(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Boolean
			Dim flag As Boolean
			If (Me._possibleFirstStatementOnLine <> Parser.PossibleFirstStatementKind.No) Then
				If (node.HasLeadingTrivia) Then
					Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node.GetLeadingTrivia())
					Dim count As Integer = syntaxList.Count - 1
					While True
						If (count < 0) Then
							GoTo Label1
						End If
						Dim kind As SyntaxKind = syntaxList(count).Kind
						If (kind = SyntaxKind.DocumentationCommentTrivia) Then
							Exit While
						End If
						Select Case kind
							Case SyntaxKind.EndOfLineTrivia
							Case SyntaxKind.ConstDirectiveTrivia
							Case SyntaxKind.IfDirectiveTrivia
							Case SyntaxKind.ElseIfDirectiveTrivia
							Case SyntaxKind.ElseDirectiveTrivia
							Case SyntaxKind.EndIfDirectiveTrivia
							Case SyntaxKind.RegionDirectiveTrivia
							Case SyntaxKind.EndRegionDirectiveTrivia
							Case SyntaxKind.ExternalSourceDirectiveTrivia
							Case SyntaxKind.EndExternalSourceDirectiveTrivia
							Case SyntaxKind.ExternalChecksumDirectiveTrivia
							Case SyntaxKind.EnableWarningDirectiveTrivia
							Case SyntaxKind.DisableWarningDirectiveTrivia
							Case SyntaxKind.ReferenceDirectiveTrivia
							Case SyntaxKind.BadDirectiveTrivia
								Exit Select
							Case SyntaxKind.ColonTrivia
							Case SyntaxKind.CommentTrivia
							Case SyntaxKind.LineContinuationTrivia
							Case SyntaxKind.DocumentationCommentExteriorTrivia
							Case SyntaxKind.DisabledTextTrivia
							Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.EventStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.SyncLockStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OpenParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia
							Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.EventStatement Or SyntaxKind.OperatorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueDoStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.OnErrorResumeNextStatement Or SyntaxKind.SyncLockStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.StaticKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OffKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OutKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.AsteriskToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.QuestionToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.StatementTerminatorToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.EmptyToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlCrefAttribute Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.IfDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.ElseDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia Or SyntaxKind.RegionDirectiveTrivia
							Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.SubBlock Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.EventStatement Or SyntaxKind.OperatorStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.AddHandlerAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.RaiseEventAccessorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueDoStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineIfPart Or SyntaxKind.SingleLineElseClause Or SyntaxKind.MultiLineIfBlock Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.OnErrorResumeNextStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectBlock Or SyntaxKind.SelectStatement Or SyntaxKind.CaseBlock Or SyntaxKind.SyncLockStatement Or SyntaxKind.WhileStatement Or SyntaxKind.ForBlock Or SyntaxKind.ForEachBlock Or SyntaxKind.ForStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OrElseKeyword Or SyntaxKind.OverloadsKeyword Or SyntaxKind.OverridableKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.StaticKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.StructureKeyword Or SyntaxKind.SubKeyword Or SyntaxKind.SyncLockKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AscendingKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.AutoKeyword Or SyntaxKind.BinaryKeyword Or SyntaxKind.ByKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OffKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OutKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.RegionKeyword Or SyntaxKind.SkipKeyword Or SyntaxKind.StrictKeyword Or SyntaxKind.TakeKeyword Or SyntaxKind.TextKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.AsteriskToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.DotToken Or SyntaxKind.SlashToken Or SyntaxKind.ColonToken Or SyntaxKind.LessThanToken Or SyntaxKind.LessThanEqualsToken Or SyntaxKind.LessThanGreaterThanToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.QuestionToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.StatementTerminatorToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.EmptyToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanSlashToken Or SyntaxKind.LessThanExclamationMinusMinusToken Or SyntaxKind.MinusMinusGreaterThanToken Or SyntaxKind.LessThanQuestionToken Or SyntaxKind.QuestionGreaterThanToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlCrefAttribute Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.IfDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.ElseDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia Or SyntaxKind.RegionDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.ExternalSourceDirectiveTrivia Or SyntaxKind.EndExternalSourceDirectiveTrivia Or SyntaxKind.ExternalChecksumDirectiveTrivia Or SyntaxKind.EnableWarningDirectiveTrivia Or SyntaxKind.DisableWarningDirectiveTrivia Or SyntaxKind.ReferenceDirectiveTrivia
							Case SyntaxKind.EndFunctionStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.FunctionBlock Or SyntaxKind.DeclareSubStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.UsingBlock Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ForStepClause Or SyntaxKind.NotKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.ConstDirectiveTrivia
							Label2:
								If (kind = SyntaxKind.WhitespaceTrivia OrElse kind = SyntaxKind.LineContinuationTrivia) Then
									count += -1
									Continue While
								Else
									flag = False
									Return flag
								End If
							Case Else
								GoTo Label2
						End Select
					End While
					flag = True
					Return flag
				End If
			Label1:
				flag = Me._possibleFirstStatementOnLine = Parser.PossibleFirstStatementKind.Yes
			Else
				flag = False
			End If
			Return flag
		End Function

		Friend Function IsNextStatementInsideLambda(ByVal context As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext, ByVal lambdaContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext, ByVal allowLeadingMultilineTrivia As Boolean) As Boolean
			Dim flag As Boolean
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim syntaxKindArray As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind()
			Dim flag1 As Boolean
			Me._allowLeadingMultilineTrivia = allowLeadingMultilineTrivia
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me.PeekEndStatement(1)
			If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None) Then
				Dim blockContext1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext = context.FindNearest(Function(c As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext) c.KindEndsBlock(syntaxKind))
				If (blockContext1 Is Nothing OrElse blockContext1.Level >= lambdaContext.Level) Then
					flag = True
					Return flag
				End If
				flag = False
				Return flag
			ElseIf (Not Me.PeekDeclarationStatement(1)) Then
				Dim kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me.PeekToken(1).Kind
				If (kind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseIfKeyword) Then
					If (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FinallyKeyword) Then
						syntaxKindArray = New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryBlock, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CatchBlock }
						blockContext = context.FindNearest(syntaxKindArray)
						flag1 = If(blockContext Is Nothing, True, blockContext.Level >= lambdaContext.Level)
						flag = flag1
						Return flag
					End If
					If (kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanToken) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				Else
					If (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CatchKeyword) Then
						syntaxKindArray = New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryBlock, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CatchBlock }
						blockContext = context.FindNearest(syntaxKindArray)
						flag1 = If(blockContext Is Nothing, True, blockContext.Level >= lambdaContext.Level)
						flag = flag1
						Return flag
					End If
					If (CUShort(kind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseKeyword) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
						flag = True
						Return flag
					End If
					Dim blockContext2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext = context.FindNearest(New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfStatement, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineIfBlock })
					flag = If(blockContext2 Is Nothing, True, blockContext2.Level >= lambdaContext.Level)
					Return flag
				End If
				syntaxKindArray = New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryBlock, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CatchBlock }
				blockContext = context.FindNearest(syntaxKindArray)
				flag1 = If(blockContext Is Nothing, True, blockContext.Level >= lambdaContext.Level)
				flag = flag1
				Return flag
			Else
				flag = False
				Return flag
			End If
			flag = True
			Return flag
		End Function

		Private Shared Function IsToken(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal ParamArray kinds As SyntaxKind()) As Boolean
			Return kinds.Contains(token.Kind)
		End Function

		Private Shared Function IsTokenOrKeyword(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal kinds As SyntaxKind()) As Boolean
			Dim flag As Boolean
			flag = If(token.Kind <> SyntaxKind.IdentifierToken, Parser.IsToken(token, kinds), Scanner.IsContextualKeyword(token, kinds))
			Return flag
		End Function

		Private Shared Function IsValidOperatorForConditionalCompilationExpr(ByVal t As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Boolean
			Dim flag As Boolean
			Dim kind As SyntaxKind = t.Kind
			If (kind <= SyntaxKind.NotKeyword) Then
				If (CUShort(kind) - CUShort(SyntaxKind.AndKeyword) <= CUShort(SyntaxKind.List) OrElse kind = SyntaxKind.ModKeyword OrElse kind = SyntaxKind.NotKeyword) Then
					flag = True
					Return flag
				End If
				flag = False
				Return flag
			ElseIf (kind > SyntaxKind.XorKeyword) Then
				Select Case kind
					Case SyntaxKind.AmpersandToken
					Case SyntaxKind.AsteriskToken
					Case SyntaxKind.PlusToken
					Case SyntaxKind.MinusToken
					Case SyntaxKind.SlashToken
					Case SyntaxKind.LessThanToken
					Case SyntaxKind.LessThanEqualsToken
					Case SyntaxKind.LessThanGreaterThanToken
					Case SyntaxKind.EqualsToken
					Case SyntaxKind.GreaterThanToken
					Case SyntaxKind.GreaterThanEqualsToken
					Case SyntaxKind.BackslashToken
					Case SyntaxKind.CaretToken
						Exit Select
					Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.EndRemoveHandlerStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.EndSyncLockStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.ModuleBlock Or SyntaxKind.StructureBlock Or SyntaxKind.InterfaceBlock Or SyntaxKind.ClassBlock Or SyntaxKind.EnumBlock Or SyntaxKind.InheritsStatement Or SyntaxKind.ImplementsStatement Or SyntaxKind.ModuleStatement Or SyntaxKind.StructureStatement Or SyntaxKind.InterfaceStatement Or SyntaxKind.ClassStatement Or SyntaxKind.EnumStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.SubBlock Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.GetAccessorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.AddHandlerAccessorBlock Or SyntaxKind.RemoveHandlerAccessorBlock Or SyntaxKind.RaiseEventAccessorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.EventBlock Or SyntaxKind.ParameterList Or SyntaxKind.SubStatement Or SyntaxKind.FunctionStatement Or SyntaxKind.SubNewStatement Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.EventStatement Or SyntaxKind.OperatorStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.AddHandlerAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.RaiseEventAccessorStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.HandlesClause Or SyntaxKind.KeywordEventContainer Or SyntaxKind.WithEventsEventContainer Or SyntaxKind.WithEventsPropertyEventContainer Or SyntaxKind.HandlesClauseItem Or SyntaxKind.IncompleteMember Or SyntaxKind.FieldDeclaration Or SyntaxKind.VariableDeclarator Or SyntaxKind.SimpleAsClause Or SyntaxKind.AsNewClause Or SyntaxKind.ObjectMemberInitializer Or SyntaxKind.ObjectCollectionInitializer Or SyntaxKind.InferredFieldInitializer Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OrElseKeyword Or SyntaxKind.OverloadsKeyword Or SyntaxKind.OverridableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PrivateKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.PublicKeyword Or SyntaxKind.RaiseEventKeyword Or SyntaxKind.ReadOnlyKeyword Or SyntaxKind.ReDimKeyword Or SyntaxKind.REMKeyword Or SyntaxKind.RemoveHandlerKeyword Or SyntaxKind.ResumeKeyword Or SyntaxKind.ReturnKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.StaticKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.StructureKeyword Or SyntaxKind.SubKeyword Or SyntaxKind.SyncLockKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ThrowKeyword Or SyntaxKind.ToKeyword Or SyntaxKind.TrueKeyword Or SyntaxKind.TryKeyword Or SyntaxKind.TryCastKeyword Or SyntaxKind.TypeOfKeyword Or SyntaxKind.UIntegerKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UShortKeyword Or SyntaxKind.UsingKeyword Or SyntaxKind.WhenKeyword Or SyntaxKind.WhileKeyword Or SyntaxKind.WideningKeyword Or SyntaxKind.WithKeyword Or SyntaxKind.WithEventsKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AscendingKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.AutoKeyword Or SyntaxKind.BinaryKeyword Or SyntaxKind.ByKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.DisableKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.EnableKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.ExplicitKeyword Or SyntaxKind.ExternalSourceKeyword Or SyntaxKind.ExternalChecksumKeyword Or SyntaxKind.FromKeyword Or SyntaxKind.GroupKeyword Or SyntaxKind.InferKeyword Or SyntaxKind.IntoKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OffKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OutKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.RegionKeyword Or SyntaxKind.SkipKeyword Or SyntaxKind.StrictKeyword Or SyntaxKind.TakeKeyword Or SyntaxKind.TextKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.UntilKeyword Or SyntaxKind.WarningKeyword Or SyntaxKind.WhereKeyword Or SyntaxKind.TypeKeyword Or SyntaxKind.XmlKeyword Or SyntaxKind.AsyncKeyword Or SyntaxKind.AwaitKeyword Or SyntaxKind.IteratorKeyword Or SyntaxKind.YieldKeyword Or SyntaxKind.ExclamationToken Or SyntaxKind.AtToken Or SyntaxKind.CommaToken Or SyntaxKind.HashToken Or SyntaxKind.AmpersandToken
					Case SyntaxKind.NamedFieldInitializer Or SyntaxKind.NotKeyword
					Case SyntaxKind.SingleQuoteToken
					Case SyntaxKind.OpenParenToken
					Case SyntaxKind.CloseParenToken
					Case SyntaxKind.OpenBraceToken
					Case SyntaxKind.CloseBraceToken
					Case SyntaxKind.SemicolonToken
					Case SyntaxKind.DotToken
					Case SyntaxKind.ColonToken
					Case SyntaxKind.EndFunctionStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.UsingBlock Or SyntaxKind.LabelStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.OpenBraceToken Or SyntaxKind.EqualsToken
					Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.LabelStatement Or SyntaxKind.GoToStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken
						flag = False
						Return flag
					Case Else
						If (CUShort(kind) - CUShort(SyntaxKind.LessThanLessThanToken) > CUShort(SyntaxKind.List)) Then
							flag = False
							Return flag
						Else
							Exit Select
						End If
				End Select
			Else
				If (CUShort(kind) - CUShort(SyntaxKind.OrKeyword) <= CUShort(SyntaxKind.List) OrElse kind = SyntaxKind.XorKeyword) Then
					flag = True
					Return flag
				End If
				flag = False
				Return flag
			End If
			flag = True
			Return flag
		End Function

		Friend Function IsValidStatementTerminator(ByVal t As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Boolean
			Return SyntaxFacts.IsTerminator(t.Kind)
		End Function

		Private Shared Function IsValidXmlQualifiedNameToken(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Boolean
			If (token.Kind = SyntaxKind.IdentifierToken) Then
				Return True
			End If
			Return TypeOf token Is KeywordSyntax
		End Function

		Private Function MakeAssignmentStatement(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim assignmentStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Select Case operatorToken.Kind
				Case SyntaxKind.EqualsToken
					assignmentStatementSyntax = Me.SyntaxFactory.SimpleAssignmentStatement(left, operatorToken, right)
					Exit Select
				Case SyntaxKind.GreaterThanToken
				Case SyntaxKind.GreaterThanEqualsToken
				Case SyntaxKind.BackslashToken
				Case SyntaxKind.EndFunctionStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.UsingBlock Or SyntaxKind.LabelStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.OpenBraceToken Or SyntaxKind.EqualsToken
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.LabelStatement Or SyntaxKind.GoToStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken
				Case SyntaxKind.CaretToken
				Case SyntaxKind.ColonEqualsToken
				Case SyntaxKind.LessThanLessThanToken
				Case SyntaxKind.GreaterThanGreaterThanToken
					Throw ExceptionUtilities.UnexpectedValue(operatorToken.Kind)
				Case SyntaxKind.AmpersandEqualsToken
					assignmentStatementSyntax = Me.SyntaxFactory.ConcatenateAssignmentStatement(left, operatorToken, right)
					Exit Select
				Case SyntaxKind.AsteriskEqualsToken
					assignmentStatementSyntax = Me.SyntaxFactory.MultiplyAssignmentStatement(left, operatorToken, right)
					Exit Select
				Case SyntaxKind.PlusEqualsToken
					assignmentStatementSyntax = Me.SyntaxFactory.AddAssignmentStatement(left, operatorToken, right)
					Exit Select
				Case SyntaxKind.MinusEqualsToken
					assignmentStatementSyntax = Me.SyntaxFactory.SubtractAssignmentStatement(left, operatorToken, right)
					Exit Select
				Case SyntaxKind.SlashEqualsToken
					assignmentStatementSyntax = Me.SyntaxFactory.DivideAssignmentStatement(left, operatorToken, right)
					Exit Select
				Case SyntaxKind.BackslashEqualsToken
					assignmentStatementSyntax = Me.SyntaxFactory.IntegerDivideAssignmentStatement(left, operatorToken, right)
					Exit Select
				Case SyntaxKind.CaretEqualsToken
					assignmentStatementSyntax = Me.SyntaxFactory.ExponentiateAssignmentStatement(left, operatorToken, right)
					Exit Select
				Case SyntaxKind.LessThanLessThanEqualsToken
					assignmentStatementSyntax = Me.SyntaxFactory.LeftShiftAssignmentStatement(left, operatorToken, right)
					Exit Select
				Case SyntaxKind.GreaterThanGreaterThanEqualsToken
					assignmentStatementSyntax = Me.SyntaxFactory.RightShiftAssignmentStatement(left, operatorToken, right)
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(operatorToken.Kind)
			End Select
			Return assignmentStatementSyntax
		End Function

		Private Function MakeCallStatementExpression(ByVal expr As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			If (expr.Kind = SyntaxKind.ConditionalAccessExpression) Then
				Dim conditionalAccessExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax = DirectCast(expr, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax)
				Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.MakeCallStatementExpression(conditionalAccessExpressionSyntax.WhenNotNull)
				If (conditionalAccessExpressionSyntax.WhenNotNull <> expressionSyntax) Then
					expr = Me.SyntaxFactory.ConditionalAccessExpression(conditionalAccessExpressionSyntax.Expression, conditionalAccessExpressionSyntax.QuestionMarkToken, expressionSyntax)
				End If
			ElseIf (expr.Kind <> SyntaxKind.InvocationExpression) Then
				expr = Me.SyntaxFactory.InvocationExpression(expr, Nothing)
			End If
			Return expr
		End Function

		Private Function MakeInvocationExpression(ByVal target As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			If (target.Kind = SyntaxKind.ConditionalAccessExpression) Then
				Dim conditionalAccessExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax = DirectCast(target, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax)
				Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.MakeInvocationExpression(conditionalAccessExpressionSyntax.WhenNotNull)
				If (conditionalAccessExpressionSyntax.WhenNotNull <> expressionSyntax) Then
					target = Me.SyntaxFactory.ConditionalAccessExpression(conditionalAccessExpressionSyntax.Expression, conditionalAccessExpressionSyntax.QuestionMarkToken, expressionSyntax)
				End If
			ElseIf (target.Kind <> SyntaxKind.InvocationExpression) Then
				If (Me.CanEndExecutableStatement(Me.CurrentToken) OrElse Me.CurrentToken.Kind = SyntaxKind.BadToken OrElse target.Kind = SyntaxKind.PredefinedCastExpression) Then
					target = Me.SyntaxFactory.InvocationExpression(target, Nothing)
				Else
					Dim greenNode As Microsoft.CodeAnalysis.GreenNode = Nothing
					Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax) = Me.ParseArguments(greenNode, False, False)
					Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.CloseParenToken)
					If (greenNode IsNot Nothing) Then
						punctuationSyntax = punctuationSyntax.AddLeadingSyntax(greenNode)
					End If
					Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax = Me.SyntaxFactory.ArgumentList(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.OpenParenToken), separatedSyntaxList, punctuationSyntax)
					target = Me.SyntaxFactory.InvocationExpression(target, Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)(argumentListSyntax, ERRID.ERR_ObsoleteArgumentsNeedParens))
				End If
			End If
			Return target
		End Function

		Private Shared Function MergeTokenText(ByVal firstToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal secondToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As String
			Dim instance As PooledStringBuilder = PooledStringBuilder.GetInstance()
			Dim stringWriter As System.IO.StringWriter = New System.IO.StringWriter(instance)
			firstToken.WriteTo(stringWriter)
			secondToken.WriteTo(stringWriter)
			Dim leadingTriviaWidth As Integer = firstToken.GetLeadingTriviaWidth()
			Dim trailingTriviaWidth As Integer = secondToken.GetTrailingTriviaWidth()
			Dim fullWidth As Integer = firstToken.FullWidth + secondToken.FullWidth
			Return instance.ToStringAndFree(leadingTriviaWidth, fullWidth - leadingTriviaWidth - trailingTriviaWidth)
		End Function

		Private Shared Function MergeTokenText(ByVal firstToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal secondToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal thirdToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As String
			Dim instance As PooledStringBuilder = PooledStringBuilder.GetInstance()
			Dim stringWriter As System.IO.StringWriter = New System.IO.StringWriter(instance)
			firstToken.WriteTo(stringWriter)
			secondToken.WriteTo(stringWriter)
			thirdToken.WriteTo(stringWriter)
			Dim leadingTriviaWidth As Integer = firstToken.GetLeadingTriviaWidth()
			Dim trailingTriviaWidth As Integer = thirdToken.GetTrailingTriviaWidth()
			Dim fullWidth As Integer = firstToken.FullWidth + secondToken.FullWidth + thirdToken.FullWidth
			Return instance.ToStringAndFree(leadingTriviaWidth, fullWidth - leadingTriviaWidth - trailingTriviaWidth)
		End Function

		Private Function MissingAggregationRangeVariables() As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax)
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax)()
			separatedSyntaxListBuilder.Add(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.AggregationRangeVariable(Nothing, Me.SyntaxFactory.FunctionAggregation(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier(), Nothing, Nothing, Nothing)))
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax) = separatedSyntaxListBuilder.ToList()
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax)(separatedSyntaxListBuilder)
			Return list
		End Function

		Private Function MustEndStatement(ByVal t As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Boolean
			Return Me.IsValidStatementTerminator(t)
		End Function

		Private Function NextLineStartsWith(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			If (Me.CurrentToken.IsEndOfLine) Then
				Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.PeekToken(1)
				If (syntaxToken.Kind = kind) Then
					flag = True
					Return flag
				ElseIf (syntaxToken.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierToken) Then
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
					If (Not Parser.TryIdentifierAsContextualKeyword(syntaxToken, syntaxKind) OrElse syntaxKind <> kind) Then
						flag = False
						Return flag
					End If
					flag = True
					Return flag
				End If
			End If
			flag = False
			Return flag
		End Function

		Private Function NextLineStartsWithStatementTerminator(Optional ByVal offset As Integer = 0) As Boolean
			Dim kind As SyntaxKind = Me.PeekToken(offset + 1).Kind
			If (kind = SyntaxKind.EmptyToken) Then
				Return True
			End If
			Return kind = SyntaxKind.EndOfFileToken
		End Function

		Private Function ParseAggregateClause(ByVal AggregateKw As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregateClauseSyntax
			Me.TryEatNewLine(ScannerState.VB)
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax) = Me.ParseFromControlVars()
			Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax) = Me._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax)()
			Me.ParseMoreQueryOperators(syntaxListBuilder)
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax) = syntaxListBuilder.ToList()
			Me._pool.Free(syntaxListBuilder)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Dim separatedSyntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax)()
			If (Not Me.TryEatNewLineAndGetContextualKeyword(SyntaxKind.IntoKeyword, keywordSyntax, True)) Then
				separatedSyntaxList1 = Me.MissingAggregationRangeVariables()
			Else
				Me.TryEatNewLine(ScannerState.VB)
				separatedSyntaxList1 = Me.ParseAggregateList(False, False)
			End If
			Return Me.SyntaxFactory.AggregateClause(AggregateKw, separatedSyntaxList, list, keywordSyntax, separatedSyntaxList1)
		End Function

		Private Function ParseAggregateList(ByVal AllowGroupName As Boolean, ByVal IsGroupJoinProjection As Boolean) As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax)
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax)()
			While True
				Dim aggregationRangeVariableSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax = Me.ParseAggregateListInitializer(AllowGroupName)
				If (aggregationRangeVariableSyntax.ContainsDiagnostics) Then
					aggregationRangeVariableSyntax = If(Not IsGroupJoinProjection, Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax)(aggregationRangeVariableSyntax, New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("B0BF54917E6AAF810C991346916DFAA00F6C9022B200D38278F2561719FDAC7F").FieldHandle }), Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax)(aggregationRangeVariableSyntax, New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("EC677C5C8FAAA489D7D0A07F32669F102F163A3C47063A7DE25294FD9CF5AE97").FieldHandle }))
				End If
				separatedSyntaxListBuilder.Add(aggregationRangeVariableSyntax)
				If (Me.CurrentToken.Kind <> SyntaxKind.CommaToken) Then
					Exit While
				End If
				Dim currentToken As PunctuationSyntax = DirectCast(Me.CurrentToken, PunctuationSyntax)
				Me.GetNextToken(ScannerState.VB)
				Me.TryEatNewLine(ScannerState.VB)
				separatedSyntaxListBuilder.AddSeparator(currentToken)
			End While
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax) = separatedSyntaxListBuilder.ToList()
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax)(separatedSyntaxListBuilder)
			Return list
		End Function

		Private Function ParseAggregateListInitializer(ByVal AllowGroupName As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax
			Dim modifiedIdentifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax = Nothing
			Dim currentToken As PunctuationSyntax = Nothing
			If ((Me.CurrentToken.Kind = SyntaxKind.IdentifierToken OrElse Me.CurrentToken.IsKeyword) AndAlso Me.PeekToken(1).Kind = SyntaxKind.EqualsToken OrElse Me.PeekToken(1).Kind = SyntaxKind.QuestionToken AndAlso Me.PeekToken(2).Kind = SyntaxKind.EqualsToken) Then
				modifiedIdentifierSyntax = Me.ParseSimpleIdentifierAsModifiedIdentifier()
				currentToken = DirectCast(Me.CurrentToken, PunctuationSyntax)
				Me.GetNextToken(ScannerState.VB)
				Me.TryEatNewLine(ScannerState.VB)
			End If
			Dim aggregationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationSyntax = Nothing
			If (Me.CurrentToken.Kind = SyntaxKind.IdentifierToken OrElse Me.CurrentToken.IsKeyword) Then
				Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
				If (Not Me.TryTokenAsContextualKeyword(Me.CurrentToken, SyntaxKind.GroupKeyword, keywordSyntax) OrElse Me.PeekToken(1).Kind = SyntaxKind.OpenParenToken) Then
					aggregationSyntax = Me.ParseAggregationExpression()
				Else
					If (Not AllowGroupName) Then
						keywordSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(keywordSyntax, ERRID.ERR_UnexpectedGroup)
					End If
					aggregationSyntax = Me.SyntaxFactory.GroupAggregation(keywordSyntax)
					Me.CheckForEndOfExpression(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationSyntax)(aggregationSyntax)
					Me.GetNextToken(ScannerState.VB)
				End If
			Else
				Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier()
				identifierTokenSyntax = If(Not AllowGroupName, Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)(identifierTokenSyntax, ERRID.ERR_ExpectedIdentifier), Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)(identifierTokenSyntax, ERRID.ERR_ExpectedIdentifierOrGroup))
				aggregationSyntax = Me.SyntaxFactory.FunctionAggregation(identifierTokenSyntax, Nothing, Nothing, Nothing)
			End If
			Dim variableNameEqualsSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax = Nothing
			If (modifiedIdentifierSyntax IsNot Nothing AndAlso currentToken IsNot Nothing) Then
				variableNameEqualsSyntax = Me.SyntaxFactory.VariableNameEquals(modifiedIdentifierSyntax, Nothing, currentToken)
			End If
			Return Me.SyntaxFactory.AggregationRangeVariable(variableNameEqualsSyntax, aggregationSyntax)
		End Function

		Private Function ParseAggregateQueryExpression(ByVal AggregateKw As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryExpressionSyntax
			Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax) = Me._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax)()
			syntaxListBuilder.Add(Me.ParseAggregateClause(AggregateKw))
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax) = syntaxListBuilder.ToList()
			Me._pool.Free(syntaxListBuilder)
			Return Me.SyntaxFactory.QueryExpression(list)
		End Function

		Private Function ParseAggregationExpression() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationSyntax
			Dim aggregationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationSyntax
			If (Me.CurrentToken.Kind = SyntaxKind.IdentifierToken) Then
				Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
				If (currentToken.PossibleKeywordKind <> SyntaxKind.GroupKeyword) Then
					GoTo Label1
				End If
				currentToken = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)(currentToken, ERRID.ERR_InvalidUseOfKeyword)
				Me.GetNextToken(ScannerState.VB)
				aggregationSyntax = Me.SyntaxFactory.FunctionAggregation(currentToken, Nothing, Nothing, Nothing)
				Return aggregationSyntax
			End If
		Label1:
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = Me.ParseIdentifier()
			Dim functionAggregationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FunctionAggregationSyntax = Nothing
			If (identifierTokenSyntax.ContainsDiagnostics OrElse Me.CurrentToken.Kind <> SyntaxKind.OpenParenToken) Then
				functionAggregationSyntax = Me.SyntaxFactory.FunctionAggregation(identifierTokenSyntax, Nothing, Nothing, Nothing)
				Me.CheckForEndOfExpression(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FunctionAggregationSyntax)(functionAggregationSyntax)
			Else
				Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
				Me.GetNextToken(ScannerState.VB)
				Me.TryEatNewLine(ScannerState.VB)
				Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Nothing
				If (Me.CurrentToken.Kind <> SyntaxKind.CloseParenToken) Then
					expressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
				End If
				Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				If (Me.TryEatNewLineAndGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CloseParenToken, punctuationSyntax1, True, ScannerState.VB) AndAlso expressionSyntax IsNot Nothing) Then
					Me.CheckForEndOfExpression(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax)
				End If
				functionAggregationSyntax = Me.SyntaxFactory.FunctionAggregation(identifierTokenSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax1)
			End If
			aggregationSyntax = functionAggregationSyntax
			Return aggregationSyntax
		End Function

		Private Function ParseAnachronisticEndIfDirective(ByVal hashToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.EndKeyword)
			keywordSyntax = keywordSyntax.AddLeadingSyntax(currentToken, ERRID.ERR_ObsoleteEndIf)
			Return Me.SyntaxFactory.EndIfDirectiveTrivia(hashToken, keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.IfKeyword))
		End Function

		Private Function ParseAnachronisticStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID = 0
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = Me.ResyncAt()
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.EndKeyword)
			Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax = Nothing
			Select Case currentToken.Kind
				Case SyntaxKind.EndIfKeyword
					statementSyntax = Me.SyntaxFactory.EndIfStatement(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.IfKeyword))
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ObsoleteEndIf
					Return statementSyntax.AddTrailingSyntax(syntaxList, eRRID)
				Case SyntaxKind.GosubKeyword
					statementSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EmptyStatement()
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ObsoleteGosub
					Return statementSyntax.AddTrailingSyntax(syntaxList, eRRID)
				Case SyntaxKind.VariantKeyword
					Return statementSyntax.AddTrailingSyntax(syntaxList, eRRID)
				Case SyntaxKind.WendKeyword
					statementSyntax = Me.SyntaxFactory.EndWhileStatement(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.WhileKeyword))
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ObsoleteWhileWend
					Return statementSyntax.AddTrailingSyntax(syntaxList, eRRID)
				Case Else
					Return statementSyntax.AddTrailingSyntax(syntaxList, eRRID)
			End Select
		End Function

		Private Function ParseArgument(Optional ByVal RedimOrNewParent As Boolean = False) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax
			Dim argumentSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
			If (expressionSyntax.ContainsDiagnostics) Then
				expressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax, New SyntaxKind() { SyntaxKind.CommaToken, SyntaxKind.CloseParenToken })
			End If
			If (Not RedimOrNewParent OrElse Me.CurrentToken.Kind <> SyntaxKind.ToKeyword) Then
				argumentSyntax = Me.SyntaxFactory.SimpleArgument(Nothing, expressionSyntax)
			Else
				Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
				Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = expressionSyntax
				Me.GetNextToken(ScannerState.VB)
				expressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
				argumentSyntax = Me.SyntaxFactory.RangeArgument(expressionSyntax1, currentToken, expressionSyntax)
			End If
			Return argumentSyntax
		End Function

		Private Function ParseArgumentList() As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax)
			Dim argumentSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax)()
			While True
				Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Nothing
				Dim currentToken As KeywordSyntax = Nothing
				Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
				If (expressionSyntax1.ContainsDiagnostics) Then
					expressionSyntax1 = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax1, New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("F024E2AB7DA64013A1A2267C220FDAE990421241EADCFCECC3689EEC695F3B70").FieldHandle })
				ElseIf (Me.CurrentToken.Kind = SyntaxKind.ToKeyword) Then
					currentToken = DirectCast(Me.CurrentToken, KeywordSyntax)
					expressionSyntax = expressionSyntax1
					Me.GetNextToken(ScannerState.VB)
					expressionSyntax1 = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
				End If
				If (expressionSyntax1.ContainsDiagnostics OrElse currentToken IsNot Nothing AndAlso expressionSyntax.ContainsDiagnostics) Then
					expressionSyntax1 = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax1, New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("F024E2AB7DA64013A1A2267C220FDAE990421241EADCFCECC3689EEC695F3B70").FieldHandle })
				End If
				If (currentToken IsNot Nothing) Then
					argumentSyntax = Me.SyntaxFactory.RangeArgument(expressionSyntax, currentToken, expressionSyntax1)
				Else
					argumentSyntax = Me.SyntaxFactory.SimpleArgument(Nothing, expressionSyntax1)
				End If
				separatedSyntaxListBuilder.Add(argumentSyntax)
				Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax, False, ScannerState.VB)) Then
					Exit While
				End If
				separatedSyntaxListBuilder.AddSeparator(punctuationSyntax)
			End While
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax) = separatedSyntaxListBuilder.ToList()
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax)(separatedSyntaxListBuilder)
			Return list
		End Function

		Private Function ParseArguments(ByRef unexpected As GreenNode, Optional ByVal RedimOrNewParent As Boolean = False, Optional ByVal attributeListParent As Boolean = False) As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax)
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax)()
			Dim flag As Boolean = Me._scanner.Options.LanguageVersion.AllowNonTrailingNamedArguments()
			Dim flag1 As Boolean = False
			While True
				Dim flag2 As Boolean = False
				If ((Me.CurrentToken.Kind = SyntaxKind.IdentifierToken OrElse Me.CurrentToken.IsKeyword) AndAlso Me.PeekToken(1).Kind = SyntaxKind.ColonEqualsToken) Then
					flag1 = True
					flag2 = True
				End If
				Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				If (flag2) Then
					If (Not attributeListParent) Then
						Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax = Me.ParseIdentifierNameAllowingKeyword()
						Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
						Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.ColonEqualsToken, punctuationSyntax, False, ScannerState.VB)
						Dim simpleArgumentSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax = Me.SyntaxFactory.SimpleArgument(Me.SyntaxFactory.NameColonEquals(identifierNameSyntax, punctuationSyntax), Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False))
						separatedSyntaxListBuilder.Add(simpleArgumentSyntax)
					Else
						Me.ParseNamedArguments(separatedSyntaxListBuilder)
						Exit While
					End If
				ElseIf (Me.CurrentToken.Kind = SyntaxKind.CommaToken) Then
					Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, currentToken, False, ScannerState.VB)
					Dim argumentSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax = Parser.ReportNonTrailingNamedArgumentIfNeeded(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.OmittedArgument(), flag1, flag)
					separatedSyntaxListBuilder.Add(argumentSyntax)
					separatedSyntaxListBuilder.AddSeparator(currentToken)
					Continue While
				ElseIf (Me.CurrentToken.Kind <> SyntaxKind.CloseParenToken) Then
					Dim argumentSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax = Me.ParseArgument(RedimOrNewParent)
					argumentSyntax1 = Parser.ReportNonTrailingNamedArgumentIfNeeded(argumentSyntax1, flag1, flag)
					separatedSyntaxListBuilder.Add(argumentSyntax1)
				Else
					If (separatedSyntaxListBuilder.Count <= 0) Then
						Exit While
					End If
					Dim argumentSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax = Parser.ReportNonTrailingNamedArgumentIfNeeded(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.OmittedArgument(), flag1, flag)
					separatedSyntaxListBuilder.Add(argumentSyntax2)
					Exit While
				End If
				If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, currentToken, False, ScannerState.VB)) Then
					If (Me.CurrentToken.Kind = SyntaxKind.CloseParenToken OrElse Me.MustEndStatement(Me.CurrentToken)) Then
						Exit While
					End If
					Dim node As GreenNode = Me.ResyncAt(New SyntaxKind() { SyntaxKind.CommaToken, SyntaxKind.CloseParenToken }).Node
					If (node IsNot Nothing) Then
						node = Parser.ReportSyntaxError(Of GreenNode)(node, ERRID.ERR_ArgumentSyntax)
					End If
					If (Me.CurrentToken.Kind <> SyntaxKind.CommaToken) Then
						unexpected = node
						Exit While
					Else
						currentToken = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
						currentToken = currentToken.AddLeadingSyntax(node)
						separatedSyntaxListBuilder.AddSeparator(currentToken)
						Me.GetNextToken(ScannerState.VB)
					End If
				Else
					separatedSyntaxListBuilder.AddSeparator(currentToken)
				End If
			End While
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax) = separatedSyntaxListBuilder.ToList()
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax)(separatedSyntaxListBuilder)
			Return list
		End Function

		Private Function ParseArrayModifiedIdentifier(ByVal elementType As IdentifierTokenSyntax, ByVal optionalNullable As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, ByVal allowExplicitSizes As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax
			Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax = Nothing
			Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax) = New SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax)()
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Dim flag As Boolean = False
			Do
				Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)()
				Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax)()
				Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.OpenParenToken, punctuationSyntax, False, ScannerState.VB)
				If (Me.CurrentToken.Kind = SyntaxKind.CommaToken) Then
					syntaxList = Me.ParseSeparators(SyntaxKind.CommaToken)
				ElseIf (Me.CurrentToken.Kind <> SyntaxKind.CloseParenToken) Then
					separatedSyntaxList = Me.ParseArgumentList()
				End If
				Me.TryEatNewLineAndGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CloseParenToken, punctuationSyntax1, True, ScannerState.VB)
				If (syntaxListBuilder.IsNull) Then
					syntaxListBuilder = Me._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax)()
				End If
				If (separatedSyntaxList.Count = 0) Then
					syntaxListBuilder.Add(Me.SyntaxFactory.ArrayRankSpecifier(punctuationSyntax, syntaxList, punctuationSyntax1))
				ElseIf (flag) Then
					punctuationSyntax1 = punctuationSyntax1.AddLeadingSyntax(separatedSyntaxList.Node, ERRID.ERR_NoConstituentArraySizes)
					syntaxListBuilder.Add(Me.SyntaxFactory.ArrayRankSpecifier(punctuationSyntax, syntaxList, punctuationSyntax1))
				Else
					argumentListSyntax = Me.SyntaxFactory.ArgumentList(punctuationSyntax, separatedSyntaxList, punctuationSyntax1)
					If (Not allowExplicitSizes) Then
						argumentListSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)(argumentListSyntax, ERRID.ERR_NoExplicitArraySizes)
					End If
				End If
				flag = True
			Loop While Me.CurrentToken.Kind = SyntaxKind.OpenParenToken
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax) = syntaxListBuilder.ToList()
			Me._pool.Free(syntaxListBuilder)
			Return Me.SyntaxFactory.ModifiedIdentifier(elementType, optionalNullable, argumentListSyntax, list)
		End Function

		Private Function ParseArrayRankSpecifiers(Optional ByVal errorForExplicitArraySizes As ERRID = 30638) As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax)
			Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax) = New SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax)()
			Do
				Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)()
				Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax)()
				Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.OpenParenToken, punctuationSyntax, False, ScannerState.VB)
				If (Me.CurrentToken.Kind = SyntaxKind.CommaToken) Then
					syntaxList = Me.ParseSeparators(SyntaxKind.CommaToken)
				ElseIf (Me.CurrentToken.Kind <> SyntaxKind.CloseParenToken) Then
					separatedSyntaxList = Me.ParseArgumentList()
				End If
				Me.TryEatNewLineAndGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CloseParenToken, punctuationSyntax1, True, ScannerState.VB)
				If (syntaxListBuilder.IsNull) Then
					syntaxListBuilder = Me._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax)()
				End If
				If (separatedSyntaxList.Count <> 0) Then
					punctuationSyntax1 = punctuationSyntax1.AddLeadingSyntax(separatedSyntaxList.Node, errorForExplicitArraySizes)
				End If
				Dim arrayRankSpecifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax = Me.SyntaxFactory.ArrayRankSpecifier(punctuationSyntax, syntaxList, punctuationSyntax1)
				syntaxListBuilder.Add(arrayRankSpecifierSyntax)
			Loop While Me.CurrentToken.Kind = SyntaxKind.OpenParenToken
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax) = syntaxListBuilder.ToList()
			Me._pool.Free(syntaxListBuilder)
			Return list
		End Function

		Private Function ParseAssignmentInitializer(ByVal anonymousTypeInitializer As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldInitializerSyntax
			Dim fieldInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldInitializerSyntax
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = Nothing
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			If (anonymousTypeInitializer AndAlso Me.TryTokenAsContextualKeyword(Me.CurrentToken, SyntaxKind.KeyKeyword, keywordSyntax)) Then
				Me.GetNextToken(ScannerState.VB)
			End If
			If (Me.CurrentToken.Kind <> SyntaxKind.DotToken) Then
				If (anonymousTypeInitializer) Then
					GoTo Label1
				End If
				currentToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.DotToken)
				identifierTokenSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier()
				identifierTokenSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)(identifierTokenSyntax, ERRID.ERR_ExpectedQualifiedNameInInit)
				punctuationSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.EqualsToken)
			Else
				currentToken = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
				Me.GetNextToken(ScannerState.VB)
				identifierTokenSyntax = Me.ParseIdentifierAllowingKeyword()
				If (675 = CInt(Me.CurrentToken.Kind)) Then
					identifierTokenSyntax = identifierTokenSyntax.AddTrailingSyntax(Me.CurrentToken)
					identifierTokenSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)(identifierTokenSyntax, ERRID.ERR_NullableTypeInferenceNotSupported)
					Me.GetNextToken(ScannerState.VB)
				End If
				If (Me.CurrentToken.Kind <> SyntaxKind.EqualsToken) Then
					punctuationSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.EqualsToken)
					punctuationSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(punctuationSyntax, ERRID.ERR_ExpectedAssignmentOperatorInInit)
				Else
					punctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
					Me.GetNextToken(ScannerState.VB)
					Me.TryEatNewLine(ScannerState.VB)
				End If
			End If
			expressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
			fieldInitializerSyntax = Me.SyntaxFactory.NamedFieldInitializer(keywordSyntax, currentToken, Me.SyntaxFactory.IdentifierName(identifierTokenSyntax), punctuationSyntax, expressionSyntax)
			Return fieldInitializerSyntax
		Label1:
			expressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
			Dim flag As Boolean = False
			Dim flag1 As Boolean = False
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = expressionSyntax.ExtractAnonymousTypeMemberName(flag, flag1)
			If (syntaxToken Is Nothing OrElse syntaxToken.IsMissing) Then
				Dim kind As SyntaxKind = expressionSyntax.Kind
				If (kind = SyntaxKind.CharacterLiteralExpression OrElse CUShort(kind) - CUShort(SyntaxKind.NumericLiteralExpression) <= CUShort(SyntaxKind.List) OrElse kind = SyntaxKind.StringLiteralExpression) Then
					expressionSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax, ERRID.ERR_AnonymousTypeExpectedIdentifier)
				ElseIf (expressionSyntax.Kind <> SyntaxKind.EqualsExpression OrElse DirectCast(expressionSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax).Left.Kind <> SyntaxKind.IdentifierName) Then
					Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = Me.ResyncAt(New SyntaxKind() { SyntaxKind.CommaToken, SyntaxKind.CloseBraceToken })
					expressionSyntax = If(Not flag1, Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax, ERRID.ERR_AnonymousTypeFieldNameInference), Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax, ERRID.ERR_AnonTypeFieldXMLNameInference))
					expressionSyntax = expressionSyntax.AddTrailingSyntax(syntaxList)
				Else
					expressionSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax, ERRID.ERR_AnonymousTypeNameWithoutPeriod)
				End If
			End If
			fieldInitializerSyntax = Me.SyntaxFactory.InferredFieldInitializer(keywordSyntax, expressionSyntax)
			Return fieldInitializerSyntax
		End Function

		Private Function ParseAssignmentOrInvocationStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseTerm(False, False)
			If (expressionSyntax.ContainsDiagnostics) Then
				expressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax, New SyntaxKind() { SyntaxKind.EqualsToken })
			End If
			If (Not SyntaxFacts.IsAssignmentStatementOperatorToken(Me.CurrentToken.Kind)) Then
				statementSyntax = Me.SyntaxFactory.ExpressionStatement(Me.MakeInvocationExpression(expressionSyntax))
			Else
				Dim currentToken As PunctuationSyntax = DirectCast(Me.CurrentToken, PunctuationSyntax)
				Me.GetNextToken(ScannerState.VB)
				Me.TryEatNewLine(ScannerState.VB)
				Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
				If (expressionSyntax1.ContainsDiagnostics) Then
					expressionSyntax1 = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax1)
				End If
				statementSyntax = Me.MakeAssignmentStatement(expressionSyntax, currentToken, expressionSyntax1)
			End If
			Return statementSyntax
		End Function

		Private Function ParseAssignmentStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			If (Me.CurrentToken.Kind = SyntaxKind.SetKeyword AndAlso (Me.IsValidStatementTerminator(Me.PeekToken(1)) OrElse Me.PeekToken(1).Kind = SyntaxKind.OpenParenToken)) Then
				If (Not Me.Context.IsWithin(New SyntaxKind() { SyntaxKind.SetAccessorBlock, SyntaxKind.GetAccessorBlock })) Then
					currentToken = Me.CurrentToken
					Me.GetNextToken(ScannerState.VB)
					statementSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EmptyStatement().AddTrailingSyntax(currentToken, ERRID.ERR_ObsoleteLetSetNotNeeded)
					Return statementSyntax
				End If
				Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
				statementSyntax = Me.ParsePropertyOrEventAccessor(SyntaxKind.SetAccessorStatement, syntaxList, New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)())
				Return statementSyntax
			End If
			currentToken = Me.CurrentToken
			Me.GetNextToken(ScannerState.VB)
			statementSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EmptyStatement().AddTrailingSyntax(currentToken, ERRID.ERR_ObsoleteLetSetNotNeeded)
			Return statementSyntax
		End Function

		Private Function ParseAttributeLists(ByVal allowFileLevelAttributes As Boolean) As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax
			Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Me._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeSyntax)()
			Do
				Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.LessThanToken, punctuationSyntax1, False, ScannerState.VB)
				While True
					Dim attributeTargetSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeTargetSyntax = Nothing
					Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax = Nothing
					If (allowFileLevelAttributes) Then
						Dim tokenAsAssemblyOrModuleKeyword As KeywordSyntax = Me.GetTokenAsAssemblyOrModuleKeyword(Me.CurrentToken)
						If (tokenAsAssemblyOrModuleKeyword IsNot Nothing) Then
							Me.GetNextToken(ScannerState.VB)
							If (Me.CurrentToken.Kind <> SyntaxKind.ColonToken) Then
								punctuationSyntax = DirectCast(Parser.HandleUnexpectedToken(SyntaxKind.ColonToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
							Else
								Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Nothing
								Dim syntaxToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Nothing
								Me.RescanTrailingColonAsToken(syntaxToken, syntaxToken1)
								Me.GetNextToken(ScannerState.VB)
								tokenAsAssemblyOrModuleKeyword = Me.GetTokenAsAssemblyOrModuleKeyword(syntaxToken)
								punctuationSyntax = DirectCast(syntaxToken1, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
							End If
						Else
							tokenAsAssemblyOrModuleKeyword = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.AssemblyKeyword)
							tokenAsAssemblyOrModuleKeyword = Parser.ReportSyntaxError(Of KeywordSyntax)(tokenAsAssemblyOrModuleKeyword, ERRID.ERR_FileAttributeNotAssemblyOrModule)
							punctuationSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.ColonToken)
						End If
						attributeTargetSyntax = Me.SyntaxFactory.AttributeTarget(tokenAsAssemblyOrModuleKeyword, punctuationSyntax)
					End If
					Me.ResetCurrentToken(ScannerState.VB)
					Dim flag As Boolean = False
					Dim nameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax = Me.ParseName(False, True, False, True, False, False, False, flag, False)
					If (Me.BeginsGeneric(False, False)) Then
						nameSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax)(nameSyntax, ERRID.ERR_GenericArgsOnAttributeSpecifier)
						nameSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax)(nameSyntax, New SyntaxKind() { SyntaxKind.GreaterThanToken })
					ElseIf (Me.CurrentToken.Kind = SyntaxKind.OpenParenToken) Then
						argumentListSyntax = Me.ParseParenthesizedArguments(False, True)
					End If
					Dim attributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeSyntax = Me.SyntaxFactory.Attribute(attributeTargetSyntax, nameSyntax, argumentListSyntax)
					separatedSyntaxListBuilder.Add(attributeSyntax)
					Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
					If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax2, False, ScannerState.VB)) Then
						Exit While
					End If
					separatedSyntaxListBuilder.AddSeparator(punctuationSyntax2)
				End While
				Me.ResetCurrentToken(ScannerState.VB)
				Dim punctuationSyntax3 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				If (Me.TryEatNewLineAndGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.GreaterThanToken, punctuationSyntax3, True, ScannerState.VB) AndAlso Not allowFileLevelAttributes AndAlso Me.IsContinuableEOL(0)) Then
					Me.TryEatNewLine(ScannerState.VB)
				End If
				syntaxListBuilder.Add(Me.SyntaxFactory.AttributeList(punctuationSyntax1, separatedSyntaxListBuilder.ToList(), punctuationSyntax3))
				separatedSyntaxListBuilder.Clear()
			Loop While Me.CurrentToken.Kind = SyntaxKind.LessThanToken
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = syntaxListBuilder.ToList()
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeSyntax)(separatedSyntaxListBuilder)
			Me._pool.Free(syntaxListBuilder)
			Return list
		End Function

		Private Function ParseAwaitExpression(Optional ByVal awaitKeyword As KeywordSyntax = Nothing) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AwaitExpressionSyntax
			If (awaitKeyword Is Nothing) Then
				Me.TryIdentifierAsContextualKeyword(Me.CurrentToken, awaitKeyword)
			End If
			Me.GetNextToken(ScannerState.VB)
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseTerm(False, False)
			Return Me.SyntaxFactory.AwaitExpression(awaitKeyword, expressionSyntax)
		End Function

		Private Function ParseAwaitStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionStatementSyntax
			Dim awaitExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AwaitExpressionSyntax = Me.ParseAwaitExpression(Nothing)
			If (awaitExpressionSyntax.ContainsDiagnostics) Then
				awaitExpressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AwaitExpressionSyntax)(awaitExpressionSyntax)
			End If
			Return Me.SyntaxFactory.ExpressionStatement(awaitExpressionSyntax)
		End Function

		Private Shared Function ParseBadDirective(ByVal hashToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BadDirectiveTriviaSyntax
			Dim badDirectiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BadDirectiveTriviaSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.BadDirectiveTrivia(hashToken)
			If (Not badDirectiveTriviaSyntax.ContainsDiagnostics) Then
				badDirectiveTriviaSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BadDirectiveTriviaSyntax)(badDirectiveTriviaSyntax, ERRID.ERR_ExpectedConditionalDirective)
			End If
			Return badDirectiveTriviaSyntax
		End Function

		Private Function ParseBinaryOperator() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.CurrentToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Nothing
			If (Me.CurrentToken.Kind = SyntaxKind.GreaterThanToken AndAlso Me.PeekToken(1).Kind = SyntaxKind.LessThanToken) Then
				syntaxToken = Me.PeekToken(1)
				currentToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.LessThanGreaterThanToken)
			ElseIf (Me.CurrentToken.Kind = SyntaxKind.EqualsToken) Then
				If (Me.PeekToken(1).Kind = SyntaxKind.GreaterThanToken) Then
					syntaxToken = Me.PeekToken(1)
					currentToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.GreaterThanEqualsToken)
				ElseIf (Me.PeekToken(1).Kind = SyntaxKind.LessThanToken) Then
					syntaxToken = Me.PeekToken(1)
					currentToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.LessThanEqualsToken)
				End If
			End If
			If (syntaxToken IsNot Nothing) Then
				currentToken = currentToken.AddLeadingSyntax(SyntaxList.List(Me.CurrentToken, syntaxToken), ERRID.ERR_ExpectedRelational)
				Me.GetNextToken(ScannerState.VB)
			End If
			Me.GetNextToken(ScannerState.VB)
			Me.TryEatNewLine(ScannerState.VB)
			Return currentToken
		End Function

		Private Function ParseBracketedXmlQualifiedName() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax
			Dim xmlBracketedNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.ResetCurrentToken(ScannerState.Content)
			If (Not Me.TryGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.LessThanToken, punctuationSyntax)) Then
				Me.ResetCurrentToken(ScannerState.VB)
				xmlBracketedNameSyntax = Me.ReportExpectedXmlBracketedName()
			Else
				Me.ResetCurrentToken(ScannerState.Element)
				Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax = Me.ParseXmlQualifiedName(False, False, ScannerState.Element, ScannerState.Element)
				Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.GreaterThanToken, punctuationSyntax1, ScannerState.VB)
				Dim vB As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax = Me.SyntaxFactory.XmlBracketedName(punctuationSyntax, DirectCast(xmlNodeSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax), punctuationSyntax1)
				vB = Me.TransitionFromXmlToVB(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax)(Parser.AdjustTriviaForMissingTokens(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax)(vB))
				xmlBracketedNameSyntax = DirectCast((New XmlWhitespaceChecker()).Visit(vB), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax)
			End If
			Return xmlBracketedNameSyntax
		End Function

		Private Function ParseCallStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CallStatementSyntax
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.MakeCallStatementExpression(Me.ParseVariable())
			If (expressionSyntax.ContainsDiagnostics) Then
				expressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax)
			End If
			Return Me.SyntaxFactory.CallStatement(currentToken, expressionSyntax)
		End Function

		Private Function ParseCaseStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax
			Dim caseStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax
			Dim caseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseClauseSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseClauseSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseClauseSyntax)()
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			If (Me.CurrentToken.Kind <> SyntaxKind.ElseKeyword) Then
				While True
					Dim kind As SyntaxKind = Me.CurrentToken.Kind
					If (kind = SyntaxKind.IsKeyword OrElse SyntaxFacts.IsRelationalOperator(kind)) Then
						Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
						Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(SyntaxKind.IsKeyword, keywordSyntax1, False, ScannerState.VB)
						If (Not SyntaxFacts.IsRelationalOperator(Me.CurrentToken.Kind)) Then
							Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.EqualsToken), ERRID.ERR_ExpectedRelational)
							caseClauseSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax)(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.RelationalCaseClause(SyntaxKind.CaseEqualsClause, keywordSyntax1, punctuationSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression()))
						Else
							Dim currentToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
							Me.GetNextToken(ScannerState.VB)
							Me.TryEatNewLine(ScannerState.VB)
							Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
							If (expressionSyntax.ContainsDiagnostics) Then
								expressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax)
							End If
							caseClauseSyntax = Me.SyntaxFactory.RelationalCaseClause(Parser.RelationalOperatorKindToCaseKind(currentToken1.Kind), keywordSyntax1, currentToken1, expressionSyntax)
						End If
					Else
						Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
						If (expressionSyntax1.ContainsDiagnostics) Then
							expressionSyntax1 = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax1, New SyntaxKind() { SyntaxKind.ToKeyword })
						End If
						Dim keywordSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
						If (Not Me.TryGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(SyntaxKind.ToKeyword, keywordSyntax2)) Then
							caseClauseSyntax = Me.SyntaxFactory.SimpleCaseClause(expressionSyntax1)
						Else
							Dim expressionSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
							If (expressionSyntax2.ContainsDiagnostics) Then
								expressionSyntax2 = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax2)
							End If
							caseClauseSyntax = Me.SyntaxFactory.RangeCaseClause(expressionSyntax1, keywordSyntax2, expressionSyntax2)
						End If
					End If
					separatedSyntaxListBuilder.Add(caseClauseSyntax)
					Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
					If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax1, False, ScannerState.VB)) Then
						Exit While
					End If
					separatedSyntaxListBuilder.AddSeparator(punctuationSyntax1)
				End While
			Else
				keywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
				Dim elseCaseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseCaseClauseSyntax = Me.SyntaxFactory.ElseCaseClause(keywordSyntax)
				separatedSyntaxListBuilder.Add(elseCaseClauseSyntax)
			End If
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseClauseSyntax) = separatedSyntaxListBuilder.ToList()
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseClauseSyntax)(separatedSyntaxListBuilder)
			caseStatementSyntax = If(keywordSyntax IsNot Nothing, Me.SyntaxFactory.CaseElseStatement(currentToken, list), Me.SyntaxFactory.CaseStatement(currentToken, list))
			Return caseStatementSyntax
		End Function

		Private Function ParseCast() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CastExpressionSyntax
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Dim kind As SyntaxKind = currentToken.Kind
			Me.GetNextToken(ScannerState.VB)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.OpenParenToken, punctuationSyntax, True, ScannerState.VB)
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
			If (expressionSyntax.ContainsDiagnostics) Then
				expressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax, New SyntaxKind() { SyntaxKind.CommaToken, SyntaxKind.CloseParenToken })
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax1, False, ScannerState.VB)) Then
				punctuationSyntax1 = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.CommaToken), ERRID.ERR_SyntaxInCastOp)
			End If
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = Me.ParseGeneralType(False)
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.TryEatNewLineAndGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CloseParenToken, punctuationSyntax2, True, ScannerState.VB)
			Dim castExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CastExpressionSyntax = Nothing
			If (kind = SyntaxKind.CTypeKeyword) Then
				castExpressionSyntax = Me.SyntaxFactory.CTypeExpression(currentToken, punctuationSyntax, expressionSyntax, punctuationSyntax1, typeSyntax, punctuationSyntax2)
			ElseIf (kind = SyntaxKind.DirectCastKeyword) Then
				castExpressionSyntax = Me.SyntaxFactory.DirectCastExpression(currentToken, punctuationSyntax, expressionSyntax, punctuationSyntax1, typeSyntax, punctuationSyntax2)
			Else
				If (kind <> SyntaxKind.TryCastKeyword) Then
					Throw ExceptionUtilities.UnexpectedValue(kind)
				End If
				castExpressionSyntax = Me.SyntaxFactory.TryCastExpression(currentToken, punctuationSyntax, expressionSyntax, punctuationSyntax1, typeSyntax, punctuationSyntax2)
			End If
			Return castExpressionSyntax
		End Function

		Private Function ParseCastExpression() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.OpenParenToken, punctuationSyntax, True, ScannerState.VB)
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.TryEatNewLineAndGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CloseParenToken, punctuationSyntax1, True, ScannerState.VB)
			Return Me.SyntaxFactory.PredefinedCastExpression(currentToken, punctuationSyntax, expressionSyntax, punctuationSyntax1)
		End Function

		Private Function ParseCatch() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchStatementSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax = Nothing
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = Nothing
			If (Me.CurrentToken.Kind = SyntaxKind.IdentifierToken) Then
				Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = Me.ParseIdentifier()
				If (identifierTokenSyntax.Kind <> SyntaxKind.None) Then
					identifierNameSyntax = Me.SyntaxFactory.IdentifierName(identifierTokenSyntax)
				End If
				Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
				Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = Nothing
				If (Me.TryGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(SyntaxKind.AsKeyword, keywordSyntax)) Then
					Dim flag As Boolean = False
					typeSyntax = Me.ParseTypeName(False, False, flag)
					If (typeSyntax.ContainsDiagnostics) Then
						typeSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)(typeSyntax, New SyntaxKind() { SyntaxKind.WhenKeyword })
					End If
					simpleAsClauseSyntax = Me.SyntaxFactory.SimpleAsClause(keywordSyntax, New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)(), typeSyntax)
				End If
			End If
			Dim catchFilterClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchFilterClauseSyntax = Nothing
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Nothing
			If (Me.TryGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(SyntaxKind.WhenKeyword, keywordSyntax1)) Then
				expressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
				catchFilterClauseSyntax = Me.SyntaxFactory.CatchFilterClause(keywordSyntax1, expressionSyntax)
			End If
			Return Me.SyntaxFactory.CatchStatement(currentToken, identifierNameSyntax, simpleAsClauseSyntax, catchFilterClauseSyntax)
		End Function

		Private Function ParseCharLiteral() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim literalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax = Me.SyntaxFactory.CharacterLiteralExpression(Me.CurrentToken)
			Me.GetNextToken(ScannerState.VB)
			Return literalExpressionSyntax
		End Function

		Private Function ParseCollectionInitializer() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax
			Dim collectionInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			If (Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.OpenBraceToken, punctuationSyntax, True, ScannerState.VB)) Then
				Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)()
				If (Me.CurrentToken.Kind <> SyntaxKind.CloseBraceToken) Then
					Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)()
					While True
						Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
						If (expressionSyntax.ContainsDiagnostics) Then
							expressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax, New SyntaxKind() { SyntaxKind.CommaToken, SyntaxKind.CloseBraceToken })
						End If
						separatedSyntaxListBuilder.Add(expressionSyntax)
						Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
						If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax1, False, ScannerState.VB)) Then
							Exit While
						End If
						separatedSyntaxListBuilder.AddSeparator(punctuationSyntax1)
					End While
					list = separatedSyntaxListBuilder.ToList()
					Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(separatedSyntaxListBuilder)
				End If
				Dim closingRightBrace As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Me.GetClosingRightBrace()
				collectionInitializerSyntax = Me.SyntaxFactory.CollectionInitializer(punctuationSyntax, list, closingRightBrace)
			Else
				Dim syntaxFactory As ContextAwareSyntaxFactory = Me.SyntaxFactory
				Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode)()
				collectionInitializerSyntax = syntaxFactory.CollectionInitializer(punctuationSyntax, separatedSyntaxList, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.CloseBraceToken))
			End If
			Return collectionInitializerSyntax
		End Function

		Friend Function ParseCompilationUnit() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax
			Return Me.ParseWithStackGuard(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax)(New Func(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax)(AddressOf Me.ParseCompilationUnitCore), Function()
				Dim syntaxFactory As ContextAwareSyntaxFactory = Me.SyntaxFactory
				Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
				Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode) = syntaxList
				syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
				Dim syntaxList2 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode) = syntaxList
				syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
				Dim syntaxList3 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode) = syntaxList
				syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
				Return syntaxFactory.CompilationUnit(syntaxList1, syntaxList2, syntaxList3, syntaxList, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EndOfFileToken())
			End Function)
		End Function

		Friend Function ParseCompilationUnitCore() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax
			Dim compilationUnitContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitContext = DirectCast(Me._context, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitContext)
			Me.GetNextToken(ScannerState.VB)
			While True
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Nothing
				Dim currentSyntaxNodeIfApplicable As BlockContext = Me.GetCurrentSyntaxNodeIfApplicable(visualBasicSyntaxNode)
				If (currentSyntaxNodeIfApplicable Is Nothing) Then
					Me.ResetCurrentToken(If(Me._allowLeadingMultilineTrivia, ScannerState.VBAllowLeadingMultilineTrivia, ScannerState.VB))
					If (Me.CurrentToken.IsEndOfParse) Then
						Exit While
					End If
					Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax = Parser.AdjustTriviaForMissingTokens(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(Me._context.Parse())
					Me._context = Me._context.LinkSyntax(statementSyntax)
					Me._context = Me._context.ResyncAndProcessStatementTerminator(statementSyntax, Nothing)
				Else
					Me._context = currentSyntaxNodeIfApplicable
					Dim visualBasicSyntaxNode1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = visualBasicSyntaxNode.LastTriviaIfAny()
					If (visualBasicSyntaxNode1 Is Nothing) Then
						Dim labelStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelStatementSyntax = TryCast(visualBasicSyntaxNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelStatementSyntax)
						If (labelStatementSyntax IsNot Nothing AndAlso labelStatementSyntax.ColonToken.Kind = SyntaxKind.ColonToken) Then
							Me.ConsumedStatementTerminator(True, Parser.PossibleFirstStatementKind.IfPrecededByLineBreak)
						End If
					ElseIf (visualBasicSyntaxNode1.Kind = SyntaxKind.EndOfLineTrivia) Then
						Me.ConsumedStatementTerminator(True)
						Me.ResetCurrentToken(ScannerState.VBAllowLeadingMultilineTrivia)
					ElseIf (visualBasicSyntaxNode1.Kind = SyntaxKind.ColonTrivia) Then
						Me.ConsumedStatementTerminator(True, Parser.PossibleFirstStatementKind.IfPrecededByLineBreak)
						Me.ResetCurrentToken(If(Me._allowLeadingMultilineTrivia, ScannerState.VBAllowLeadingMultilineTrivia, ScannerState.VB))
					End If
				End If
			End While
			Me._context.RecoverFromMissingEnd(compilationUnitContext)
			Dim currentToken As PunctuationSyntax = DirectCast(Me.CurrentToken, PunctuationSyntax)
			Dim ifDirectiveTriviaSyntaxes As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax) = Nothing
			Dim regionDirectiveTriviaSyntaxes As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax) = Nothing
			Dim flag As Boolean = False
			Dim externalSourceDirectiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax = Nothing
			currentToken = Me._scanner.RecoverFromMissingConditionalEnds(currentToken, ifDirectiveTriviaSyntaxes, regionDirectiveTriviaSyntaxes, flag, externalSourceDirectiveTriviaSyntax)
			Return compilationUnitContext.CreateCompilationUnit(currentToken, ifDirectiveTriviaSyntaxes, regionDirectiveTriviaSyntaxes, flag, externalSourceDirectiveTriviaSyntax)
		End Function

		Friend Function ParseConditionalCompilationExpression() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim flag As Boolean = Me._evaluatingConditionCompilationExpression
			Me._evaluatingConditionCompilationExpression = True
			Me._evaluatingConditionCompilationExpression = flag
			Return Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
		End Function

		Friend Function ParseConditionalCompilationStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax
			Dim directiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax
			Dim directiveTriviaSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax
			Dim possibleKeywordKind As SyntaxKind
			If (Me.CurrentToken.Kind = SyntaxKind.DateLiteralToken OrElse Me.CurrentToken.Kind = SyntaxKind.BadToken) Then
				Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.HashToken).AddLeadingSyntax(New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(Me.CurrentToken))
				Me.GetNextToken(ScannerState.VB)
				directiveTriviaSyntax = Parser.ParseBadDirective(punctuationSyntax)
			Else
				directiveTriviaSyntax1 = Nothing
				currentToken = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
				Me.GetNextToken(ScannerState.VB)
				Dim kind As SyntaxKind = Me.CurrentToken.Kind
				If (kind <= SyntaxKind.EndKeyword) Then
					If (kind = SyntaxKind.ConstKeyword) Then
						directiveTriviaSyntax1 = Me.ParseConstDirective(currentToken)
					Else
						Select Case kind
							Case SyntaxKind.ElseKeyword
								directiveTriviaSyntax1 = Me.ParseElseDirective(currentToken)
								Exit Select
							Case SyntaxKind.ElseIfKeyword
								directiveTriviaSyntax1 = Me.ParseElseIfDirective(currentToken)
								Exit Select
							Case SyntaxKind.EndKeyword
								directiveTriviaSyntax1 = Me.ParseEndDirective(currentToken)
								Exit Select
							Case Else
								GoTo Label0
						End Select
					End If
				ElseIf (kind = SyntaxKind.IfKeyword) Then
					directiveTriviaSyntax1 = Me.ParseIfDirective(currentToken, Nothing)
				ElseIf (kind = SyntaxKind.EndIfKeyword) Then
					directiveTriviaSyntax1 = Me.ParseAnachronisticEndIfDirective(currentToken)
				Else
					If (kind <> SyntaxKind.IdentifierToken) Then
						GoTo Label0
					End If
					possibleKeywordKind = DirectCast(Me.CurrentToken, IdentifierTokenSyntax).PossibleKeywordKind
					If (possibleKeywordKind > SyntaxKind.DisableKeyword) Then
						Select Case possibleKeywordKind
							Case SyntaxKind.EnableKeyword
								GoTo Label3
							Case SyntaxKind.EqualsKeyword
							Case SyntaxKind.ExplicitKeyword
								Exit Select
							Case SyntaxKind.ExternalSourceKeyword
								directiveTriviaSyntax1 = Me.ParseExternalSourceDirective(currentToken)
								GoTo Label2
							Case SyntaxKind.ExternalChecksumKeyword
								directiveTriviaSyntax1 = Me.ParseExternalChecksumDirective(currentToken)
								GoTo Label2
							Case Else
								If (possibleKeywordKind = SyntaxKind.RegionKeyword) Then
									directiveTriviaSyntax1 = Me.ParseRegionDirective(currentToken)
									GoTo Label2
								Else
									Exit Select
								End If
						End Select
					Else
						If (possibleKeywordKind <> SyntaxKind.ReferenceKeyword) Then
							GoTo Label4
						End If
						directiveTriviaSyntax1 = Me.ParseReferenceDirective(currentToken)
						GoTo Label2
					End If
				Label5:
					directiveTriviaSyntax1 = Parser.ParseBadDirective(currentToken)
				End If
			Label2:
				directiveTriviaSyntax = directiveTriviaSyntax1
			End If
			Return directiveTriviaSyntax
		Label0:
			directiveTriviaSyntax1 = Parser.ParseBadDirective(currentToken)
			GoTo Label2
		Label3:
			directiveTriviaSyntax1 = Me.ParseWarningDirective(currentToken)
			GoTo Label2
		Label4:
			If (possibleKeywordKind = SyntaxKind.DisableKeyword) Then
				GoTo Label3
			End If
			GoTo Label5
		End Function

		Private Function ParseConstDirective(ByVal hashToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstDirectiveTriviaSyntax
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = Me.ParseIdentifier()
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)()
			If (identifierTokenSyntax.ContainsDiagnostics) Then
				syntaxList = Me.ResyncAt(New SyntaxKind() { SyntaxKind.EqualsToken })
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.EqualsToken, punctuationSyntax, ScannerState.VB)
			If (syntaxList.Node IsNot Nothing) Then
				punctuationSyntax = punctuationSyntax.AddLeadingSyntax(syntaxList, ERRID.ERR_Syntax)
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseConditionalCompilationExpression()
			If (expressionSyntax.ContainsDiagnostics) Then
				expressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax)
			End If
			Return Me.SyntaxFactory.ConstDirectiveTrivia(hashToken, currentToken, identifierTokenSyntax, punctuationSyntax, expressionSyntax)
		End Function

		Private Function ParseConstraintSyntax() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax
			Dim currentToken As KeywordSyntax
			Dim constraintSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax = Nothing
			If (Me.CurrentToken.Kind = SyntaxKind.NewKeyword) Then
				currentToken = DirectCast(Me.CurrentToken, KeywordSyntax)
				constraintSyntax = Me.SyntaxFactory.NewConstraint(currentToken)
				Me.GetNextToken(ScannerState.VB)
			ElseIf (Me.CurrentToken.Kind = SyntaxKind.ClassKeyword) Then
				currentToken = DirectCast(Me.CurrentToken, KeywordSyntax)
				constraintSyntax = Me.SyntaxFactory.ClassConstraint(currentToken)
				Me.GetNextToken(ScannerState.VB)
			ElseIf (Me.CurrentToken.Kind <> SyntaxKind.StructureKeyword) Then
				Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = Nothing
				If (Not Parser.CanTokenStartTypeName(Me.CurrentToken)) Then
					diagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_BadConstraintSyntax)
				End If
				Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = Me.ParseGeneralType(False)
				If (diagnosticInfo IsNot Nothing) Then
					typeSyntax = DirectCast(typeSyntax.AddError(diagnosticInfo), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)
				End If
				constraintSyntax = Me.SyntaxFactory.TypeConstraint(typeSyntax)
			Else
				currentToken = DirectCast(Me.CurrentToken, KeywordSyntax)
				constraintSyntax = Me.SyntaxFactory.StructureConstraint(currentToken)
				Me.GetNextToken(ScannerState.VB)
			End If
			Return constraintSyntax
		End Function

		Private Function ParseContinueStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = 0
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Dim kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me.CurrentToken.Kind
			If (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoKeyword) Then
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueDoStatement
				keywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
			ElseIf (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForKeyword) Then
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueForStatement
				keywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
			ElseIf (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword) Then
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueWhileStatement
				keywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
			Else
				Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext = Me.Context.FindNearest(New Func(Of Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, Boolean)(AddressOf SyntaxFacts.SupportsContinueStatement))
				If (blockContext IsNot Nothing) Then
					Dim blockKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = blockContext.BlockKind
					If (blockKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileBlock) Then
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueWhileStatement
						keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword)
					ElseIf (CUShort(blockKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForBlock) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueForStatement
						keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForKeyword)
					ElseIf (CUShort(blockKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleDoLoopBlock) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueDoStatement
						keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoKeyword)
					End If
				End If
				If (keywordSyntax Is Nothing) Then
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueDoStatement
					keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoKeyword)
				End If
				keywordSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(keywordSyntax, ERRID.ERR_ExpectedContinueKind)
			End If
			Return Me.SyntaxFactory.ContinueStatement(syntaxKind, currentToken, keywordSyntax)
		End Function

		Private Function ParseCustomEventDefinition(ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			If (Me.PeekToken(1).Kind = SyntaxKind.EventKeyword) Then
				keywordSyntax = Me._scanner.MakeKeyword(DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax))
				Me.GetNextToken(ScannerState.VB)
				Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
				Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = Me.ParseIdentifier()
				Dim currentToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
				Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = Nothing
				Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = Nothing
				If (Me.CurrentToken.Kind <> SyntaxKind.AsKeyword) Then
					Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax = Nothing
					If (Me.TryRejectGenericParametersForMemberDecl(typeParameterListSyntax)) Then
						identifierTokenSyntax = identifierTokenSyntax.AddTrailingSyntax(typeParameterListSyntax)
					End If
					If (Me.CurrentToken.Kind = SyntaxKind.OpenParenToken) Then
						Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
						Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
						Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax) = Me.ParseParameters(punctuationSyntax, punctuationSyntax1)
						identifierTokenSyntax = identifierTokenSyntax.AddTrailingSyntax(Me.SyntaxFactory.ParameterList(punctuationSyntax, separatedSyntaxList, punctuationSyntax1))
					End If
					If (Me.CurrentToken.Kind <> SyntaxKind.AsKeyword) Then
						currentToken1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.AsKeyword)
					Else
						currentToken1 = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
						currentToken1 = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(currentToken1, ERRID.ERR_EventsCantBeFunctions)
						Me.GetNextToken(ScannerState.VB)
						currentToken1 = currentToken1.AddTrailingSyntax(Me.ResyncAt(New SyntaxKind() { SyntaxKind.ImplementsKeyword }))
					End If
					Dim syntaxFactory As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContextAwareSyntaxFactory = Me.SyntaxFactory
					syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)()
					simpleAsClauseSyntax = syntaxFactory.SimpleAsClause(currentToken1, syntaxList, Me.SyntaxFactory.IdentifierName(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier()))
				Else
					currentToken1 = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
					Me.GetNextToken(ScannerState.VB)
					typeSyntax = Me.ParseGeneralType(False)
					If (typeSyntax.ContainsDiagnostics) Then
						typeSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)(typeSyntax)
					End If
					Dim contextAwareSyntaxFactory As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContextAwareSyntaxFactory = Me.SyntaxFactory
					syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)()
					simpleAsClauseSyntax = contextAwareSyntaxFactory.SimpleAsClause(currentToken1, syntaxList, typeSyntax)
				End If
				Dim implementsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax = Nothing
				If (Me.CurrentToken.Kind = SyntaxKind.ImplementsKeyword) Then
					implementsClauseSyntax = Me.ParseImplementsList()
				End If
				statementSyntax = Me.SyntaxFactory.EventStatement(attributes, modifiers, keywordSyntax, currentToken, identifierTokenSyntax, Nothing, simpleAsClauseSyntax, implementsClauseSyntax)
			Else
				statementSyntax = Me.ParseVarDeclStatement(attributes, modifiers)
			End If
			Return statementSyntax
		End Function

		Private Function ParseDateLiteral() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim literalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax = Me.SyntaxFactory.DateLiteralExpression(Me.CurrentToken)
			Me.GetNextToken(ScannerState.VB)
			Return literalExpressionSyntax
		End Function

		Friend Function ParseDeclarationStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim flag As Boolean = Me._hadImplicitLineContinuation
			Dim flag1 As Boolean = Me._hadLineContinuationComment
			Try
				Me._hadImplicitLineContinuation = False
				Me._hadLineContinuationComment = False
				Dim statementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax = Me.ParseDeclarationStatementInternal()
				If (Me._hadImplicitLineContinuation) Then
					Dim statementSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax = statementSyntax1
					statementSyntax1 = Me.CheckFeatureAvailability(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(Feature.LineContinuation, statementSyntax1)
					If (statementSyntax2 = statementSyntax1 AndAlso Me._hadLineContinuationComment) Then
						statementSyntax1 = Me.CheckFeatureAvailability(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(Feature.LineContinuationComments, statementSyntax1)
					End If
				End If
				statementSyntax = statementSyntax1
			Finally
				Me._hadImplicitLineContinuation = flag
				Me._hadLineContinuationComment = flag1
			End Try
			Return statementSyntax
		End Function

		Friend Function ParseDeclarationStatementInternal() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)
			Me._cancellationToken.ThrowIfCancellationRequested()
			Dim kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me.CurrentToken.Kind
			If (kind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword) Then
				If (kind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DimKeyword) Then
					If (kind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceKeyword) Then
						Select Case kind
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleKeyword
								Exit Select
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MustInheritKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MustOverrideKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NarrowingKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverloadsKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridableKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PartialKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrivateKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PublicKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReadOnlyKeyword
								statementSyntax = Me.ParseSpecifierDeclaration()
								Return statementSyntax
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MyBaseKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MyClassKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NextKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OfKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionalKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrElseKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParamArrayKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndAddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRemoveHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParamArrayKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PartialKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrivateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PublicKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReDimKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.REMKeyword
								statementSyntax = Me.ParseStatementInMethodBodyInternal()
								Return statementSyntax
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceKeyword
								syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
								syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)()
								statementSyntax = Me.ParseNamespaceStatement(syntaxList, syntaxList1)
								Return statementSyntax
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword
								syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
								syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)()
								statementSyntax = Me.ParseOperatorStatement(syntaxList, syntaxList1)
								Return statementSyntax
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionKeyword
								syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
								syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)()
								statementSyntax = Me.ParseOptionStatement(syntaxList, syntaxList1)
								Return statementSyntax
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword
								syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
								syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)()
								statementSyntax = Me.ParsePropertyDefinition(syntaxList, syntaxList1)
								Return statementSyntax
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventKeyword
								syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
								syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)()
								statementSyntax = Me.ParsePropertyOrEventAccessor(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorStatement, syntaxList, syntaxList1)
								Return statementSyntax
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerKeyword
								syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
								syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)()
								statementSyntax = Me.ParsePropertyOrEventAccessor(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement, syntaxList, syntaxList1)
								Return statementSyntax
							Case Else
								Select Case kind
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetKeyword
										syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
										syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)()
										statementSyntax = Me.ParsePropertyOrEventAccessor(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement, syntaxList, syntaxList1)
										Return statementSyntax
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShadowsKeyword
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SharedKeyword
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StaticKeyword
										statementSyntax = Me.ParseSpecifierDeclaration()
										Return statementSyntax
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShortKeyword
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleKeyword
										statementSyntax = Me.ParseStatementInMethodBodyInternal()
										Return statementSyntax
									Case Else
										If (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword) Then
											GoTo Label7
										End If
										statementSyntax = Me.ParseStatementInMethodBodyInternal()
										Return statementSyntax
								End Select

						End Select
					Else
						Select Case kind
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndKeyword
								statementSyntax = Me.ParseGroupEndStatement()
								Return statementSyntax
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumKeyword
								syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
								syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)()
								statementSyntax = Me.ParseEnumStatement(syntaxList, syntaxList1)
								Return statementSyntax
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EraseKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ErrorKeyword
								statementSyntax = Me.ParseStatementInMethodBodyInternal()
								Return statementSyntax
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventKeyword
								syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
								syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)()
								statementSyntax = Me.ParseEventDefinition(syntaxList, syntaxList1)
								Return statementSyntax
							Case Else
								Select Case kind
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FriendKeyword
										statementSyntax = Me.ParseSpecifierDeclaration()
										Return statementSyntax
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionKeyword
										syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
										syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)()
										statementSyntax = Me.ParseFunctionStatement(syntaxList, syntaxList1)
										Return statementSyntax
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetKeyword
										syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
										syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)()
										statementSyntax = Me.ParsePropertyOrEventAccessor(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement, syntaxList, syntaxList1)
										Return statementSyntax
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetTypeKeyword
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetXmlNamespaceKeyword
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GoToKeyword
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesKeyword
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IfKeyword
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InKeyword
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntegerKeyword
										statementSyntax = Me.ParseStatementInMethodBodyInternal()
										Return statementSyntax
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GlobalKeyword
										Dim statementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax = Me.ParsePossibleDeclarationStatement()
										If (statementSyntax1 Is Nothing) Then
											statementSyntax = Me.ParseStatementInMethodBodyInternal()
											Return statementSyntax
										Else
											statementSyntax = statementSyntax1
											Return statementSyntax
										End If
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsKeyword
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsKeyword
										syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
										syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)()
										statementSyntax = Me.ParseInheritsImplementsStatement(syntaxList, syntaxList1)
										Return statementSyntax
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsKeyword
										syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
										syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)()
										statementSyntax = Me.ParseImportsStatement(syntaxList, syntaxList1)
										Return statementSyntax
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceKeyword
										Exit Select
									Case Else
										statementSyntax = Me.ParseStatementInMethodBodyInternal()
										Return statementSyntax
								End Select

						End Select
					End If
				ElseIf (kind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassKeyword) Then
					If (kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerKeyword) Then
						GoTo Label8
					End If
					syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
					syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)()
					statementSyntax = Me.ParsePropertyOrEventAccessor(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorStatement, syntaxList, syntaxList1)
					Return statementSyntax
				ElseIf (kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstKeyword) Then
					Select Case kind
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareKeyword
							syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
							syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)()
							statementSyntax = Me.ParseProcDeclareStatement(syntaxList, syntaxList1)
							Return statementSyntax
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DefaultKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DimKeyword
							statementSyntax = Me.ParseSpecifierDeclaration()
							Return statementSyntax
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateKeyword
							syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
							syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)()
							statementSyntax = Me.ParseDelegateStatement(syntaxList, syntaxList1)
							Return statementSyntax
						Case Else
							statementSyntax = Me.ParseStatementInMethodBodyInternal()
							Return statementSyntax
					End Select
				Else
					statementSyntax = Me.ParseSpecifierDeclaration()
					Return statementSyntax
				End If
			Label7:
				syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
				syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)()
				statementSyntax = Me.ParseTypeStatement(syntaxList, syntaxList1)
			ElseIf (kind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WriteOnlyKeyword) Then
				If (kind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WideningKeyword) Then
					If (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsKeyword OrElse kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WriteOnlyKeyword) Then
						statementSyntax = Me.ParseSpecifierDeclaration()
						Return statementSyntax
					End If
					statementSyntax = Me.ParseStatementInMethodBodyInternal()
					Return statementSyntax
				Else
					If (kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword) Then
						If (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WideningKeyword) Then
							statementSyntax = Me.ParseSpecifierDeclaration()
							Return statementSyntax
						End If
						statementSyntax = Me.ParseStatementInMethodBodyInternal()
						Return statementSyntax
					End If
					syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
					syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)()
					statementSyntax = Me.ParseSubStatement(syntaxList, syntaxList1)
				End If
			ElseIf (kind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StatementTerminatorToken) Then
				Select Case kind
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ColonToken
					Label5:
						statementSyntax = Me.ParseStatementInMethodBodyInternal()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanToken
						Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.PeekToken(1)
						If (Me.IsContinuableEOL(1)) Then
							syntaxToken = Me.PeekToken(2)
						End If
						Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
						If (Not Parser.TryTokenAsKeyword(syntaxToken, syntaxKind) OrElse syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AssemblyKeyword AndAlso syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleKeyword) Then
							statementSyntax = Me.ParseSpecifierDeclaration()
							Exit Select
						Else
							Dim syntaxList2 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Me.ParseAttributeLists(True)
							statementSyntax = Me.SyntaxFactory.AttributesStatement(syntaxList2)
							Exit Select
						End If
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanEqualsToken
						statementSyntax = Me.ParseStatementInMethodBodyInternal()
						Return statementSyntax
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanGreaterThanToken
						statementSyntax = Me.ParseSpecifierDeclaration(Me.ParseEmptyAttributeLists())
						Exit Select
					Case Else
						If (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StatementTerminatorToken) Then
							GoTo Label5
						End If
						statementSyntax = Me.ParseStatementInMethodBodyInternal()
						Return statementSyntax
				End Select
			ElseIf (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyToken) Then
				statementSyntax = Me.ParseEmptyStatement()
			ElseIf (kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierToken) Then
				If (kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntegerLiteralToken) Then
					statementSyntax = Me.ParseStatementInMethodBodyInternal()
					Return statementSyntax
				End If
				If (Not Me.IsFirstStatementOnLine(Me.CurrentToken)) Then
					statementSyntax = Me.ReportUnrecognizedStatementError(ERRID.ERR_Syntax)
				Else
					statementSyntax = Me.ParseLabel()
				End If
			ElseIf (Me.Context.BlockKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumBlock) Then
				Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
				If (Parser.TryIdentifierAsContextualKeyword(Me.CurrentToken, syntaxKind1)) Then
					If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CustomKeyword) Then
						syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
						syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)()
						statementSyntax = Me.ParseCustomEventDefinition(syntaxList, syntaxList1)
						Return statementSyntax
					ElseIf (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeKeyword) Then
						If (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsyncKeyword AndAlso syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IteratorKeyword) Then
							GoTo Label3
						End If
						statementSyntax = Me.ParseSpecifierDeclaration()
						Return statementSyntax
					Else
						statementSyntax = Me.ReportUnrecognizedStatementError(ERRID.ERR_ObsoleteStructureNotType)
						Return statementSyntax
					End If
				End If
			Label3:
				Dim statementSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax = Me.ParsePossibleDeclarationStatement()
				If (statementSyntax2 IsNot Nothing) Then
					statementSyntax = statementSyntax2
				ElseIf (Me.Context.BlockKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit) Then
					statementSyntax = Me.ParseStatementInMethodBodyInternal()
				ElseIf (Not Me.ShouldParseAsLabel()) Then
					statementSyntax = Me.ReportUnrecognizedStatementError(ERRID.ERR_ExpectedDeclaration)
				Else
					statementSyntax = Me.ParseLabel()
				End If
			Else
				syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
				statementSyntax = Me.ParseEnumMemberOrLabel(syntaxList)
			End If
			Return statementSyntax
		Label8:
			If (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassKeyword) Then
				GoTo Label7
			End If
			statementSyntax = Me.ParseStatementInMethodBodyInternal()
			Return statementSyntax
		End Function

		Private Sub ParseDeclareLibClause(ByRef libKeyword As KeywordSyntax, ByRef libraryName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax, ByRef optionalAliasKeyword As KeywordSyntax, ByRef optionalAliasName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax)
			libKeyword = Nothing
			optionalAliasKeyword = Nothing
			If (Not Me.VerifyExpectedToken(Of KeywordSyntax)(SyntaxKind.LibKeyword, libKeyword, ScannerState.VB)) Then
				libraryName = Me.SyntaxFactory.StringLiteralExpression(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingStringLiteral())
			Else
				libraryName = Me.ParseStringLiteral()
				If (libraryName.ContainsDiagnostics) Then
					libraryName = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax)(libraryName, New SyntaxKind() { SyntaxKind.AliasKeyword, SyntaxKind.OpenParenToken })
				End If
			End If
			If (Me.CurrentToken.Kind = SyntaxKind.AliasKeyword) Then
				optionalAliasKeyword = DirectCast(Me.CurrentToken, KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
				optionalAliasName = Me.ParseStringLiteral()
				If (optionalAliasName.ContainsDiagnostics) Then
					optionalAliasName = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax)(optionalAliasName, New SyntaxKind() { SyntaxKind.OpenParenToken })
				End If
			End If
		End Sub

		Private Function ParseDecLiteral() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim literalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax = Me.SyntaxFactory.NumericLiteralExpression(Me.CurrentToken)
			Me.GetNextToken(ScannerState.VB)
			Return literalExpressionSyntax
		End Function

		Private Function ParseDelegateStatement(ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = Nothing
			Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax = Nothing
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax = Nothing
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = Nothing
			Dim handlesClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax = Nothing
			Dim implementsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax = Nothing
			Me.GetNextToken(ScannerState.VB)
			Dim kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me.CurrentToken.Kind
			If (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionKeyword) Then
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement
				keywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
				Me.ParseFunctionOrDelegateStatement(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement, identifierTokenSyntax, typeParameterListSyntax, parameterListSyntax, simpleAsClauseSyntax, handlesClauseSyntax, implementsClauseSyntax)
			ElseIf (kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword) Then
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement
				keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword)
				keywordSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(keywordSyntax, ERRID.ERR_ExpectedSubOrFunction)
				Me.ParseSubOrDelegateStatement(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement, identifierTokenSyntax, typeParameterListSyntax, parameterListSyntax, handlesClauseSyntax, implementsClauseSyntax)
			Else
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement
				keywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
				Me.ParseSubOrDelegateStatement(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement, identifierTokenSyntax, typeParameterListSyntax, parameterListSyntax, handlesClauseSyntax, implementsClauseSyntax)
			End If
			Dim delegateStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax = Me.SyntaxFactory.DelegateStatement(syntaxKind, attributes, modifiers, currentToken, keywordSyntax, identifierTokenSyntax, typeParameterListSyntax, parameterListSyntax, simpleAsClauseSyntax)
			If (handlesClauseSyntax IsNot Nothing) Then
				delegateStatementSyntax = delegateStatementSyntax.AddTrailingSyntax(handlesClauseSyntax)
			End If
			If (implementsClauseSyntax IsNot Nothing) Then
				delegateStatementSyntax = delegateStatementSyntax.AddTrailingSyntax(implementsClauseSyntax)
			End If
			Return delegateStatementSyntax
		End Function

		Private Function ParseDoStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim whileOrUntilClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax = Nothing
			Me.TryParseOptionalWhileOrUntilClause(currentToken, whileOrUntilClauseSyntax)
			If (whileOrUntilClauseSyntax IsNot Nothing) Then
				syntaxKind = If(whileOrUntilClauseSyntax.Kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileClause, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoUntilStatement, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoWhileStatement)
			Else
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleDoStatement
			End If
			Return Me.SyntaxFactory.DoStatement(syntaxKind, currentToken, whileOrUntilClauseSyntax)
		End Function

		Private Function ParseElseDirective(ByVal hashToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax
			Dim directiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			If (Me.CurrentToken.Kind = SyntaxKind.IfKeyword) Then
				directiveTriviaSyntax = Me.ParseIfDirective(hashToken, currentToken)
			Else
				directiveTriviaSyntax = Me.SyntaxFactory.ElseDirectiveTrivia(hashToken, currentToken)
			End If
			Return directiveTriviaSyntax
		End Function

		Private Function ParseElseIfDirective(ByVal hashToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax
			Return Me.ParseIfDirective(hashToken, Nothing)
		End Function

		Private Function ParseElseIfStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			If (Me.CurrentToken.Kind = SyntaxKind.ElseIfKeyword) Then
				currentToken = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
			ElseIf (Me.CurrentToken.Kind = SyntaxKind.ElseKeyword) Then
				Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
				If (Me.Context.IsSingleLine) Then
					statementSyntax = Me.SyntaxFactory.ElseStatement(keywordSyntax)
					Return statementSyntax
				End If
				Dim currentToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
				currentToken = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax(SyntaxKind.ElseIfKeyword, Parser.MergeTokenText(keywordSyntax, currentToken1), keywordSyntax.GetLeadingTrivia(), currentToken1.GetTrailingTrivia())
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
			If (expressionSyntax.ContainsDiagnostics) Then
				expressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax, New SyntaxKind() { SyntaxKind.ThenKeyword })
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Me.TryGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(SyntaxKind.ThenKeyword, keywordSyntax1)
			statementSyntax = Me.SyntaxFactory.ElseIfStatement(currentToken, expressionSyntax, keywordSyntax1)
			Return statementSyntax
		End Function

		Private Function ParseElseStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Return Me.SyntaxFactory.ElseStatement(currentToken)
		End Function

		Private Function ParseEmptyAttributeLists() As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)
			Dim syntaxTrivium As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.CurrentToken
			Dim text As String = currentToken.Text
			Dim length As Integer = currentToken.Text.Length
			Dim str As String = text.Substring(0, 1)
			Dim str1 As String = text.Substring(length - 1, 1)
			If (length > 2) Then
				syntaxTrivium = Me._scanner.MakeWhiteSpaceTrivia(text.Substring(1, length - 2))
			Else
				syntaxTrivium = Nothing
			End If
			Dim syntaxTrivium1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia = syntaxTrivium
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Me._scanner.MakePunctuationToken(SyntaxKind.LessThanToken, str, currentToken.GetLeadingTrivia(), syntaxTrivium1)
			Dim scanner As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Scanner = Me._scanner
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)()
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = scanner.MakePunctuationToken(SyntaxKind.GreaterThanToken, str1, syntaxList, currentToken.GetTrailingTrivia())
			Me.GetNextToken(ScannerState.VB)
			Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Me._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeSyntax)()
			Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax = Me.SyntaxFactory.IdentifierName(Parser.ReportSyntaxError(Of IdentifierTokenSyntax)(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier(), ERRID.ERR_ExpectedIdentifier))
			Dim attributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeSyntax = Me.SyntaxFactory.Attribute(Nothing, identifierNameSyntax, Nothing)
			separatedSyntaxListBuilder.Add(attributeSyntax)
			syntaxListBuilder.Add(Me.SyntaxFactory.AttributeList(punctuationSyntax, separatedSyntaxListBuilder.ToList(), punctuationSyntax1))
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = syntaxListBuilder.ToList()
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeSyntax)(separatedSyntaxListBuilder)
			Me._pool.Free(syntaxListBuilder)
			Return list
		End Function

		Private Function ParseEmptyStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EmptyStatementSyntax
			Dim currentToken As PunctuationSyntax = DirectCast(Me.CurrentToken, PunctuationSyntax)
			Me.GetNextToken(ScannerState.VB)
			Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EmptyStatement(currentToken)
		End Function

		Private Function ParseEndDirective(ByVal hashToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim directiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax = Nothing
			If (Me.CurrentToken.Kind = SyntaxKind.IfKeyword) Then
				Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
				directiveTriviaSyntax = Me.SyntaxFactory.EndIfDirectiveTrivia(hashToken, currentToken, keywordSyntax)
			ElseIf (Me.CurrentToken.Kind = SyntaxKind.IdentifierToken) Then
				Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
				If (identifierTokenSyntax.PossibleKeywordKind = SyntaxKind.RegionKeyword) Then
					Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Me._scanner.MakeKeyword(DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax))
					Me.GetNextToken(ScannerState.VB)
					directiveTriviaSyntax = Me.SyntaxFactory.EndRegionDirectiveTrivia(hashToken, currentToken, keywordSyntax1)
				ElseIf (identifierTokenSyntax.PossibleKeywordKind = SyntaxKind.ExternalSourceKeyword) Then
					Dim keywordSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Me._scanner.MakeKeyword(DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax))
					Me.GetNextToken(ScannerState.VB)
					directiveTriviaSyntax = Me.SyntaxFactory.EndExternalSourceDirectiveTrivia(hashToken, currentToken, keywordSyntax2)
				End If
			End If
			If (directiveTriviaSyntax Is Nothing) Then
				hashToken = hashToken.AddTrailingSyntax(currentToken, ERRID.ERR_Syntax)
				directiveTriviaSyntax = Me.SyntaxFactory.BadDirectiveTrivia(hashToken)
			End If
			Return directiveTriviaSyntax
		End Function

		Private Function ParseEndStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			If (Not Me.CanFollowStatementButIsNotSelectFollowingExpression(Me.PeekToken(1))) Then
				statementSyntax = Me.ParseGroupEndStatement()
			Else
				statementSyntax = Me.ParseStopOrEndStatement()
			End If
			Return statementSyntax
		End Function

		Private Function ParseEnumMemberOrLabel(ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			If (attributes.Any() OrElse Not Me.ShouldParseAsLabel()) Then
				Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = Me.ParseIdentifier()
				If (identifierTokenSyntax.ContainsDiagnostics) Then
					identifierTokenSyntax = identifierTokenSyntax.AddTrailingSyntax(Me.ResyncAt(New SyntaxKind() { SyntaxKind.EqualsToken }))
				End If
				Dim equalsValueSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax = Nothing
				Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Nothing
				If (Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.EqualsToken, punctuationSyntax, False, ScannerState.VB)) Then
					expressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
					If (expressionSyntax.ContainsDiagnostics) Then
						expressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax)
					End If
					equalsValueSyntax = Me.SyntaxFactory.EqualsValue(punctuationSyntax, expressionSyntax)
				End If
				statementSyntax = Me.SyntaxFactory.EnumMemberDeclaration(attributes, identifierTokenSyntax, equalsValueSyntax)
			Else
				statementSyntax = Me.ParseLabel()
			End If
			Return statementSyntax
		End Function

		Private Function ParseEnumStatement(Optional ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Nothing, Optional ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax) = Nothing) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumStatementSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Dim asClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax = Nothing
			Me.GetNextToken(ScannerState.VB)
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = Me.ParseIdentifier()
			If (Me.CurrentToken.Kind = SyntaxKind.OpenParenToken) Then
				Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax)(Me.ParseGenericParameters(), ERRID.ERR_GenericParamsOnInvalidMember)
				identifierTokenSyntax = identifierTokenSyntax.AddTrailingSyntax(typeParameterListSyntax)
			End If
			If (identifierTokenSyntax.ContainsDiagnostics) Then
				identifierTokenSyntax = identifierTokenSyntax.AddTrailingSyntax(Me.ResyncAt(New SyntaxKind() { SyntaxKind.AsKeyword }))
			End If
			If (Me.CurrentToken.Kind = SyntaxKind.AsKeyword) Then
				Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
				Dim flag As Boolean = False
				Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = Me.ParseTypeName(False, False, flag)
				If (typeSyntax.ContainsDiagnostics) Then
					typeSyntax = typeSyntax.AddTrailingSyntax(Me.ResyncAt())
				End If
				asClauseSyntax = Me.SyntaxFactory.SimpleAsClause(keywordSyntax, New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)(), typeSyntax)
			End If
			Return Me.SyntaxFactory.EnumStatement(attributes, modifiers, currentToken, identifierTokenSyntax, asClauseSyntax)
		End Function

		Private Function ParseErase() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EraseStatementSyntax
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) = Me.ParseVariableList()
			Return Me.SyntaxFactory.EraseStatement(currentToken, separatedSyntaxList)
		End Function

		Private Function ParseError() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ErrorStatementSyntax
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
			If (expressionSyntax.ContainsDiagnostics) Then
				expressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax)
			End If
			Return Me.SyntaxFactory.ErrorStatement(currentToken, expressionSyntax)
		End Function

		Private Function ParseEventDefinition(ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = Me.ParseIdentifier()
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax = Nothing
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax)()
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = Nothing
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = Nothing
			If (Me.CurrentToken.Kind <> SyntaxKind.AsKeyword) Then
				Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax = Nothing
				If (Me.TryRejectGenericParametersForMemberDecl(typeParameterListSyntax)) Then
					identifierTokenSyntax = identifierTokenSyntax.AddTrailingSyntax(typeParameterListSyntax)
				End If
				If (Me.CurrentToken.Kind = SyntaxKind.OpenParenToken) Then
					separatedSyntaxList = Me.ParseParameters(punctuationSyntax, punctuationSyntax1)
				End If
				If (Me.CurrentToken.Kind = SyntaxKind.AsKeyword) Then
					If (punctuationSyntax1 Is Nothing) Then
						identifierTokenSyntax = identifierTokenSyntax.AddTrailingSyntax(Me.ResyncAt(New SyntaxKind() { SyntaxKind.ImplementsKeyword }), ERRID.ERR_EventsCantBeFunctions)
					Else
						punctuationSyntax1 = punctuationSyntax1.AddTrailingSyntax(Me.ResyncAt(New SyntaxKind() { SyntaxKind.ImplementsKeyword }), ERRID.ERR_EventsCantBeFunctions)
					End If
				End If
			Else
				keywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
				typeSyntax = Me.ParseGeneralType(False)
				If (typeSyntax.ContainsDiagnostics) Then
					typeSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)(typeSyntax)
				End If
				simpleAsClauseSyntax = Me.SyntaxFactory.SimpleAsClause(keywordSyntax, New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)(), typeSyntax)
			End If
			If (punctuationSyntax IsNot Nothing) Then
				parameterListSyntax = Me.SyntaxFactory.ParameterList(punctuationSyntax, separatedSyntaxList, punctuationSyntax1)
			End If
			Dim implementsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax = Nothing
			If (Me.CurrentToken.Kind = SyntaxKind.ImplementsKeyword) Then
				implementsClauseSyntax = Me.ParseImplementsList()
			End If
			Return Me.SyntaxFactory.EventStatement(attributes, modifiers, Nothing, currentToken, identifierTokenSyntax, parameterListSyntax, simpleAsClauseSyntax, implementsClauseSyntax)
		End Function

		Friend Function ParseExecutableStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim func As Func(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)
			Dim func1 As Func(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = New Func(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(AddressOf Me.ParseExecutableStatementCore)
			If (Parser._Closure$__.$I119-0 Is Nothing) Then
				func = Function() Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EmptyStatement()
				Parser._Closure$__.$I119-0 = func
			Else
				func = Parser._Closure$__.$I119-0
			End If
			Return Me.ParseWithStackGuard(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(func1, func)
		End Function

		Private Function ParseExecutableStatementCore() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim item As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim compilationUnitContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitContext = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitContext(Me)
			Dim syntaxFactory As ContextAwareSyntaxFactory = Me.SyntaxFactory
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)()
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode) = syntaxList
			syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)()
			Dim methodStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax = syntaxFactory.SubStatement(syntaxList1, syntaxList, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.SubKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier(), Nothing, Nothing, Nothing, Nothing, Nothing)
			Dim methodBlockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockContext = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockContext(SyntaxKind.SubBlock, methodStatementSyntax, compilationUnitContext)
			Me.GetNextToken(ScannerState.VB)
			Me._context = methodBlockContext
			Do
				Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax = Me._context.Parse()
				Me._context = Me._context.LinkSyntax(statementSyntax)
				Me._context = Me._context.ResyncAndProcessStatementTerminator(statementSyntax, Nothing)
			Loop While Me._context.Level > methodBlockContext.Level AndAlso Not Me.CurrentToken.IsEndOfParse
			Me._context.RecoverFromMissingEnd(methodBlockContext)
			If (methodBlockContext.Statements.Count <= 0) Then
				Dim methodBlockBaseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockBaseSyntax = DirectCast(compilationUnitContext.Statements(0), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockBaseSyntax)
				If (Not methodBlockBaseSyntax.Statements.Any()) Then
					item = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)(methodBlockBaseSyntax.[End], ERRID.ERR_InvInsideEndsProc)
				Else
					item = methodBlockBaseSyntax.Statements(0)
				End If
			Else
				item = methodBlockContext.Statements(0)
			End If
			Return item
		End Function

		Private Function ParseExitStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = 0
			Dim statementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Dim kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me.CurrentToken.Kind
			If (kind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword) Then
				If (kind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForKeyword) Then
					If (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoKeyword) Then
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitDoStatement
						keywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
						Me.GetNextToken(ScannerState.VB)
					Else
						If (kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForKeyword) Then
							GoTo Label0
						End If
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitForStatement
						keywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
						Me.GetNextToken(ScannerState.VB)
					End If
				ElseIf (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionKeyword) Then
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitFunctionStatement
					keywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
					Me.GetNextToken(ScannerState.VB)
				Else
					If (kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword) Then
						GoTo Label0
					End If
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitPropertyStatement
					keywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
					Me.GetNextToken(ScannerState.VB)
				End If
			ElseIf (kind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword) Then
				If (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword) Then
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitSelectStatement
					keywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
					Me.GetNextToken(ScannerState.VB)
				Else
					If (kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword) Then
						GoTo Label0
					End If
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitSubStatement
					keywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
					Me.GetNextToken(ScannerState.VB)
				End If
			ElseIf (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryKeyword) Then
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitTryStatement
				keywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
			Else
				If (kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword) Then
					GoTo Label0
				End If
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitWhileStatement
				keywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
			End If
			statementSyntax = Me.SyntaxFactory.ExitStatement(syntaxKind, currentToken, keywordSyntax)
			Return statementSyntax
		Label0:
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext = Me.Context.FindNearest(New Func(Of Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, Boolean)(AddressOf SyntaxFacts.SupportsExitStatement))
			If (blockContext IsNot Nothing) Then
				Dim blockKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = blockContext.BlockKind
				If (blockKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileBlock) Then
					Select Case blockKind
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitSubStatement
							keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitFunctionStatement
							keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionKeyword)
							Exit Select
						Case Else
							If (blockKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock) Then
								syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitPropertyStatement
								keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword)
								Exit Select
							ElseIf (blockKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileBlock) Then
								syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitWhileStatement
								keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword)
								Exit Select
							Else
								Exit Select
							End If
					End Select
				ElseIf (blockKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectBlock) Then
					If (blockKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryBlock) Then
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitTryStatement
						keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryKeyword)
					ElseIf (blockKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectBlock) Then
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitSelectStatement
						keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword)
					End If
				ElseIf (CUShort(blockKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForBlock) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitForStatement
					keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForKeyword)
				ElseIf (CUShort(blockKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleDoLoopBlock) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitDoStatement
					keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoKeyword)
				End If
			End If
			Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None
			Dim kind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me.CurrentToken.Kind
			If (kind1 > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword) Then
				If (kind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventKeyword OrElse kind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerKeyword) Then
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExitEventMemberNotInvalid
					If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
						statementSyntax1 = Me.SyntaxFactory.ReturnStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReturnKeyword), Nothing).AddLeadingSyntax(SyntaxList.List(currentToken, Me.CurrentToken), eRRID)
						Me.GetNextToken(ScannerState.VB)
						statementSyntax = statementSyntax1
						Return statementSyntax
					End If
					If (keywordSyntax Is Nothing) Then
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitSubStatement
						keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword)
					End If
					keywordSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedExitKind)
					statementSyntax = Me.SyntaxFactory.ExitStatement(syntaxKind, currentToken, keywordSyntax)
					Return statementSyntax
				End If
				If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
					statementSyntax1 = Me.SyntaxFactory.ReturnStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReturnKeyword), Nothing).AddLeadingSyntax(SyntaxList.List(currentToken, Me.CurrentToken), eRRID)
					Me.GetNextToken(ScannerState.VB)
					statementSyntax = statementSyntax1
					Return statementSyntax
				End If
				If (keywordSyntax Is Nothing) Then
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitSubStatement
					keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword)
				End If
				keywordSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedExitKind)
				statementSyntax = Me.SyntaxFactory.ExitStatement(syntaxKind, currentToken, keywordSyntax)
				Return statementSyntax
			Else
				If (kind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerKeyword) Then
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExitEventMemberNotInvalid
					If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
						statementSyntax1 = Me.SyntaxFactory.ReturnStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReturnKeyword), Nothing).AddLeadingSyntax(SyntaxList.List(currentToken, Me.CurrentToken), eRRID)
						Me.GetNextToken(ScannerState.VB)
						statementSyntax = statementSyntax1
						Return statementSyntax
					End If
					If (keywordSyntax Is Nothing) Then
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitSubStatement
						keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword)
					End If
					keywordSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedExitKind)
					statementSyntax = Me.SyntaxFactory.ExitStatement(syntaxKind, currentToken, keywordSyntax)
					Return statementSyntax
				End If
				If (kind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword) Then
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExitOperatorNotValid
					If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
						statementSyntax1 = Me.SyntaxFactory.ReturnStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReturnKeyword), Nothing).AddLeadingSyntax(SyntaxList.List(currentToken, Me.CurrentToken), eRRID)
						Me.GetNextToken(ScannerState.VB)
						statementSyntax = statementSyntax1
						Return statementSyntax
					End If
					If (keywordSyntax Is Nothing) Then
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitSubStatement
						keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword)
					End If
					keywordSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedExitKind)
					statementSyntax = Me.SyntaxFactory.ExitStatement(syntaxKind, currentToken, keywordSyntax)
					Return statementSyntax
				Else
					If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
						statementSyntax1 = Me.SyntaxFactory.ReturnStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReturnKeyword), Nothing).AddLeadingSyntax(SyntaxList.List(currentToken, Me.CurrentToken), eRRID)
						Me.GetNextToken(ScannerState.VB)
						statementSyntax = statementSyntax1
						Return statementSyntax
					End If
					If (keywordSyntax Is Nothing) Then
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitSubStatement
						keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword)
					End If
					keywordSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedExitKind)
					statementSyntax = Me.SyntaxFactory.ExitStatement(syntaxKind, currentToken, keywordSyntax)
					Return statementSyntax
				End If
			End If
			eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExitEventMemberNotInvalid
			If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
				statementSyntax1 = Me.SyntaxFactory.ReturnStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReturnKeyword), Nothing).AddLeadingSyntax(SyntaxList.List(currentToken, Me.CurrentToken), eRRID)
				Me.GetNextToken(ScannerState.VB)
				statementSyntax = statementSyntax1
				Return statementSyntax
			End If
			If (keywordSyntax Is Nothing) Then
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitSubStatement
				keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword)
			End If
			keywordSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedExitKind)
			statementSyntax = Me.SyntaxFactory.ExitStatement(syntaxKind, currentToken, keywordSyntax)
			Return statementSyntax
		End Function

		Friend Function ParseExpression(Optional ByVal pendingPrecedence As OperatorPrecedence = 0, Optional ByVal bailIfFirstTokenRejected As Boolean = False) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim func As Func(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			Dim func1 As Func(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) = Function() Me.ParseExpressionCore(pendingPrecedence, bailIfFirstTokenRejected)
			If (Parser._Closure$__.$I15-1 Is Nothing) Then
				func = Function() Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression()
				Parser._Closure$__.$I15-1 = func
			Else
				func = Parser._Closure$__.$I15-1
			End If
			Return Me.ParseWithStackGuard(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(func1, func)
		End Function

		Private Function ParseExpressionBlockStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
			If (expressionSyntax.ContainsDiagnostics) Then
				expressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax)
			End If
			Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax = Nothing
			Dim kind As SyntaxKind = currentToken.Kind
			If (kind = SyntaxKind.SyncLockKeyword) Then
				statementSyntax = Me.SyntaxFactory.SyncLockStatement(currentToken, expressionSyntax)
			ElseIf (kind = SyntaxKind.WhileKeyword) Then
				statementSyntax = Me.SyntaxFactory.WhileStatement(currentToken, expressionSyntax)
			ElseIf (kind = SyntaxKind.WithKeyword) Then
				statementSyntax = Me.SyntaxFactory.WithStatement(currentToken, expressionSyntax)
			End If
			Return statementSyntax
		End Function

		Private Function ParseExpressionCore(Optional ByVal pendingPrecedence As Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorPrecedence = 0, Optional ByVal bailIfFirstTokenRejected As Boolean = False) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Try
				Me._recursionDepth = Me._recursionDepth + 1
				StackGuard.EnsureSufficientExecutionStack(Me._recursionDepth)
				expressionSyntax1 = Nothing
				Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.CurrentToken
				If (Not Me._evaluatingConditionCompilationExpression OrElse Parser.StartsValidConditionalCompilationExpr(currentToken)) Then
					Dim kind As SyntaxKind = currentToken.Kind
					If (kind <= SyntaxKind.NotKeyword) Then
						If (kind = SyntaxKind.AddressOfKeyword) Then
							Me.GetNextToken(ScannerState.VB)
							Dim expressionSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorPrecedence.PrecedenceNegate, False)
							expressionSyntax1 = Me.SyntaxFactory.AddressOfExpression(currentToken, expressionSyntax2)
						Else
							If (kind <> SyntaxKind.NotKeyword) Then
								GoTo Label0
							End If
							Me.GetNextToken(ScannerState.VB)
							Dim expressionSyntax3 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorPrecedence.PrecedenceNot, False)
							expressionSyntax1 = Me.SyntaxFactory.NotExpression(currentToken, expressionSyntax3)
						End If
					ElseIf (kind = SyntaxKind.PlusToken) Then
						Me.GetNextToken(ScannerState.VB)
						Dim expressionSyntax4 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorPrecedence.PrecedenceNegate, False)
						expressionSyntax1 = Me.SyntaxFactory.UnaryPlusExpression(currentToken, expressionSyntax4)
					Else
						If (kind <> SyntaxKind.MinusToken) Then
							GoTo Label0
						End If
						Me.GetNextToken(ScannerState.VB)
						Dim expressionSyntax5 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorPrecedence.PrecedenceNegate, False)
						expressionSyntax1 = Me.SyntaxFactory.UnaryMinusExpression(currentToken, expressionSyntax5)
					End If
				Label2:
					If (302 <> CInt(expressionSyntax1.Kind)) Then
						While Me.CurrentToken.IsBinaryOperator()
							If (Not Me._evaluatingConditionCompilationExpression OrElse Parser.IsValidOperatorForConditionalCompilationExpr(Me.CurrentToken)) Then
								Dim operatorPrecedence As Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorPrecedence = KeywordTable.TokenOpPrec(Me.CurrentToken.Kind)
								If (operatorPrecedence <= pendingPrecedence) Then
									Exit While
								End If
								Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.ParseBinaryOperator()
								Dim expressionSyntax6 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(operatorPrecedence, False)
								expressionSyntax1 = Me.SyntaxFactory.BinaryExpression(Parser.GetBinaryOperatorHelper(syntaxToken), expressionSyntax1, syntaxToken, expressionSyntax6)
							Else
								expressionSyntax1 = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax1, ERRID.ERR_BadCCExpression)
								Exit While
							End If
						End While
					End If
					expressionSyntax = expressionSyntax1
				ElseIf (Not bailIfFirstTokenRejected) Then
					expressionSyntax1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression().AddTrailingSyntax(currentToken, ERRID.ERR_BadCCExpression)
					Me.GetNextToken(ScannerState.VB)
					expressionSyntax = expressionSyntax1
				Else
					expressionSyntax = Nothing
				End If
			Finally
				Me._recursionDepth = Me._recursionDepth - 1
			End Try
			Return expressionSyntax
		Label0:
			expressionSyntax1 = Me.ParseTerm(bailIfFirstTokenRejected, False)
			If (expressionSyntax1 Is Nothing) Then
				expressionSyntax = Nothing
				Return expressionSyntax
			Else
				GoTo Label2
			End If
		End Function

		Private Function ParseExternalChecksumDirective(ByVal hashToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax
			Dim guid As System.Guid
			Dim currentToken As IdentifierTokenSyntax = DirectCast(Me.CurrentToken, IdentifierTokenSyntax)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Me._scanner.MakeKeyword(currentToken)
			Me.GetNextToken(ScannerState.VB)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.OpenParenToken, punctuationSyntax, ScannerState.VB)
			Dim stringLiteralTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax = Nothing
			Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax)(SyntaxKind.StringLiteralToken, stringLiteralTokenSyntax, ScannerState.VB)
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax2, ScannerState.VB)
			Dim stringLiteralTokenSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax = Nothing
			Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax)(SyntaxKind.StringLiteralToken, stringLiteralTokenSyntax1, ScannerState.VB)
			If (Not stringLiteralTokenSyntax1.IsMissing AndAlso Not System.Guid.TryParse(stringLiteralTokenSyntax1.ValueText, guid)) Then
				stringLiteralTokenSyntax1 = stringLiteralTokenSyntax1.WithDiagnostics(New DiagnosticInfo() { ErrorFactory.ErrorInfo(ERRID.WRN_BadGUIDFormatExtChecksum) })
			End If
			Dim punctuationSyntax3 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax3, ScannerState.VB)
			Dim stringLiteralTokenSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax = Nothing
			Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax)(SyntaxKind.StringLiteralToken, stringLiteralTokenSyntax2, ScannerState.VB)
			If (Not stringLiteralTokenSyntax2.IsMissing) Then
				Dim valueText As String = stringLiteralTokenSyntax2.ValueText
				If (valueText.Length Mod 2 = 0) Then
					Dim str As String = valueText
					Dim num As Integer = 0
					While num < str.Length
						If (SyntaxFacts.IsHexDigit(str(num))) Then
							num = num + 1
						Else
							stringLiteralTokenSyntax2 = stringLiteralTokenSyntax2.WithDiagnostics(New DiagnosticInfo() { ErrorFactory.ErrorInfo(ERRID.WRN_BadChecksumValExtChecksum) })
							Exit While
						End If
					End While
				Else
					stringLiteralTokenSyntax2 = stringLiteralTokenSyntax2.WithDiagnostics(New DiagnosticInfo() { ErrorFactory.ErrorInfo(ERRID.WRN_BadChecksumValExtChecksum) })
				End If
			End If
			Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CloseParenToken, punctuationSyntax1, ScannerState.VB)
			Return Me.SyntaxFactory.ExternalChecksumDirectiveTrivia(hashToken, keywordSyntax, punctuationSyntax, stringLiteralTokenSyntax, punctuationSyntax2, stringLiteralTokenSyntax1, punctuationSyntax3, stringLiteralTokenSyntax2, punctuationSyntax1)
		End Function

		Private Sub ParseExternalID(ByVal builder As SyntaxListBuilder(Of GreenNode))
			If (Me.CurrentToken.Kind = SyntaxKind.XmlNameToken) Then
				Dim currentToken As XmlNameTokenSyntax = DirectCast(Me.CurrentToken, XmlNameTokenSyntax)
				Dim str As String = currentToken.ToString()
				If (EmbeddedOperators.CompareString(str, "SYSTEM", False) = 0) Then
					builder.Add(currentToken)
					Me.GetNextToken(ScannerState.DocType)
					builder.Add(Me.ParseXmlString(ScannerState.DocType))
					Return
				End If
				If (EmbeddedOperators.CompareString(str, "PUBLIC", False) <> 0) Then
					Return
				End If
				builder.Add(currentToken)
				Me.GetNextToken(ScannerState.DocType)
				builder.Add(Me.ParseXmlString(ScannerState.DocType))
				builder.Add(Me.ParseXmlString(ScannerState.DocType))
			End If
		End Sub

		Private Function ParseExternalSourceDirective(ByVal hashToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax
			Dim currentToken As IdentifierTokenSyntax = DirectCast(Me.CurrentToken, IdentifierTokenSyntax)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Me._scanner.MakeKeyword(currentToken)
			Me.GetNextToken(ScannerState.VB)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.OpenParenToken, punctuationSyntax, ScannerState.VB)
			Dim stringLiteralTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax = Nothing
			Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax)(SyntaxKind.StringLiteralToken, stringLiteralTokenSyntax, ScannerState.VB)
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax2, ScannerState.VB)
			Dim integerLiteralTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IntegerLiteralTokenSyntax = Nothing
			Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IntegerLiteralTokenSyntax)(SyntaxKind.IntegerLiteralToken, integerLiteralTokenSyntax, ScannerState.VB)
			Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CloseParenToken, punctuationSyntax1, ScannerState.VB)
			Return Me.SyntaxFactory.ExternalSourceDirectiveTrivia(hashToken, keywordSyntax, punctuationSyntax, stringLiteralTokenSyntax, punctuationSyntax2, integerLiteralTokenSyntax, punctuationSyntax1)
		End Function

		Private Sub ParseFieldOrPropertyAsClauseAndInitializer(ByVal isProperty As Boolean, ByVal allowAsNewWith As Boolean, ByRef optionalAsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax, ByRef optionalInitializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax)
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax = Nothing
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = Nothing
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Dim node As GreenNode = Nothing
			If (Me.CurrentToken.Kind = SyntaxKind.AsKeyword) Then
				currentToken = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
				Dim objectCollectionInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCollectionInitializerSyntax = Nothing
				If (Me.CurrentToken.Kind <> SyntaxKind.NewKeyword) Then
					If (isProperty AndAlso Me.CurrentToken.Kind = SyntaxKind.LessThanToken) Then
						node = Me.ParseAttributeLists(False).Node
					End If
					typeSyntax = Me.ParseGeneralType(False)
					If (typeSyntax.ContainsDiagnostics) Then
						typeSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)(typeSyntax, New SyntaxKind() { SyntaxKind.CommaToken, SyntaxKind.EqualsToken })
					End If
					optionalAsClause = Me.SyntaxFactory.SimpleAsClause(currentToken, node, typeSyntax)
				Else
					keywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
					Me.GetNextToken(ScannerState.VB)
					If (isProperty AndAlso Me.CurrentToken.Kind = SyntaxKind.LessThanToken) Then
						node = Me.ParseAttributeLists(False).Node
					End If
					If (Me.CurrentToken.Kind <> SyntaxKind.WithKeyword) Then
						Dim flag As Boolean = False
						typeSyntax = Me.ParseTypeName(False, False, flag)
						If (Me.CurrentToken.Kind = SyntaxKind.OpenParenToken) Then
							argumentListSyntax = Me.ParseParenthesizedArguments(False, False)
						End If
						If (isProperty) Then
							Me.TryEatNewLineIfFollowedBy(SyntaxKind.FromKeyword)
						End If
						If (Me.TryTokenAsContextualKeyword(Me.CurrentToken, SyntaxKind.FromKeyword, keywordSyntax1)) Then
							Me.GetNextToken(ScannerState.VB)
							objectCollectionInitializerSyntax = Me.ParseObjectCollectionInitializer(keywordSyntax1)
						End If
						optionalAsClause = Me.SyntaxFactory.AsNewClause(currentToken, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCreationExpressionSyntax(SyntaxKind.ObjectCreationExpression, keywordSyntax, node, typeSyntax, argumentListSyntax, objectCollectionInitializerSyntax))
					Else
						optionalAsClause = Nothing
					End If
				End If
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			If (keywordSyntax IsNot Nothing) Then
				Dim objectMemberInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax = Nothing
				If (Me.CurrentToken.Kind = SyntaxKind.WithKeyword) Then
					Dim currentToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
					If (keywordSyntax1 IsNot Nothing) Then
						currentToken1 = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(currentToken1, ERRID.ERR_CantCombineInitializers)
						optionalAsClause = optionalAsClause.AddTrailingSyntax(currentToken1)
						Me.GetNextToken(ScannerState.VB)
						Return
					End If
					objectMemberInitializerSyntax = Me.ParseObjectInitializerList(typeSyntax Is Nothing, allowAsNewWith)
					Dim keywordSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
					If (Me.CurrentToken.Kind = SyntaxKind.IdentifierToken AndAlso Me.TryIdentifierAsContextualKeyword(Me.CurrentToken, keywordSyntax2) AndAlso keywordSyntax2.Kind = SyntaxKind.FromKeyword) Then
						objectMemberInitializerSyntax = objectMemberInitializerSyntax.AddTrailingSyntax(keywordSyntax2, ERRID.ERR_CantCombineInitializers)
						Me.GetNextToken(ScannerState.VB)
					End If
					Dim objectCreationExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NewExpressionSyntax = Nothing
					If (typeSyntax IsNot Nothing) Then
						objectCreationExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCreationExpressionSyntax(SyntaxKind.ObjectCreationExpression, keywordSyntax, node, typeSyntax, argumentListSyntax, objectMemberInitializerSyntax)
					Else
						If (Not allowAsNewWith) Then
							currentToken1 = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(currentToken1, ERRID.ERR_UnrecognizedTypeKeyword)
						End If
						objectCreationExpressionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax(SyntaxKind.AnonymousObjectCreationExpression, keywordSyntax, Nothing, objectMemberInitializerSyntax)
					End If
					optionalAsClause = Me.SyntaxFactory.AsNewClause(currentToken, objectCreationExpressionSyntax)
				End If
			ElseIf (Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.EqualsToken, punctuationSyntax, False, ScannerState.VB)) Then
				Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
				optionalInitializer = Me.SyntaxFactory.EqualsValue(punctuationSyntax, expressionSyntax)
				If (optionalInitializer.ContainsDiagnostics) Then
					optionalInitializer = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax)(optionalInitializer, New SyntaxKind() { SyntaxKind.CommaToken })
					Return
				End If
			End If
		End Sub

		Private Function ParseFinally() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Return Me.SyntaxFactory.FinallyStatement(currentToken)
		End Function

		Private Function ParseFltLiteral() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim literalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax = Me.SyntaxFactory.NumericLiteralExpression(Me.CurrentToken)
			Me.GetNextToken(ScannerState.VB)
			Return literalExpressionSyntax
		End Function

		Private Function ParseForEachStatement(ByVal forKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal eachKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Me.ParseForLoopControlVariable()
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Nothing
			If (visualBasicSyntaxNode.ContainsDiagnostics) Then
				visualBasicSyntaxNode = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(visualBasicSyntaxNode, New SyntaxKind() { SyntaxKind.InKeyword })
			End If
			Me.TryEatNewLineIfFollowedBy(SyntaxKind.InKeyword)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(SyntaxKind.InKeyword, keywordSyntax, False, ScannerState.VB)) Then
				keywordSyntax = DirectCast(Parser.HandleUnexpectedToken(SyntaxKind.InKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				expressionSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression().AddTrailingSyntax(Me.ResyncAt(New SyntaxKind() { SyntaxKind.ToKeyword }), ERRID.ERR_Syntax)
			Else
				expressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
				If (expressionSyntax.ContainsDiagnostics) Then
					expressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax)
				End If
			End If
			Return Me.SyntaxFactory.ForEachStatement(forKeyword, eachKeyword, visualBasicSyntaxNode, keywordSyntax, expressionSyntax)
		End Function

		Private Function ParseForLoopControlVariable() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			If (Me.CurrentToken.Kind <> SyntaxKind.IdentifierToken) Then
				visualBasicSyntaxNode = Me.ParseVariable()
			Else
				Dim kind As SyntaxKind = Me.PeekToken(1).Kind
				If (kind <> SyntaxKind.AsKeyword) Then
					If (kind = SyntaxKind.OpenParenToken) Then
						Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Nothing
						Dim num As Integer = Me.PeekAheadFor(Of SyntaxKind())(Parser.s_isTokenOrKeywordFunc, New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("8C3F66DC34C68D84B4A7EB85074243DFCA233E7142F2F0494915C4EB3876825A").FieldHandle }, syntaxToken)
						If (syntaxToken Is Nothing OrElse syntaxToken.Kind <> SyntaxKind.AsKeyword OrElse Me.PeekToken(num - 1).Kind <> SyntaxKind.CloseParenToken) Then
							visualBasicSyntaxNode = Me.ParseVariable()
							Return visualBasicSyntaxNode
						End If
						visualBasicSyntaxNode = Me.ParseForLoopVariableDeclaration()
						Return visualBasicSyntaxNode
					ElseIf (kind = SyntaxKind.QuestionToken) Then
						GoTo Label1
					End If
					visualBasicSyntaxNode = Me.ParseVariable()
					Return visualBasicSyntaxNode
				End If
			Label1:
				visualBasicSyntaxNode = Me.ParseForLoopVariableDeclaration()
			End If
			Return visualBasicSyntaxNode
		End Function

		Private Function ParseForLoopVariableDeclaration() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax
			Dim modifiedIdentifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax = Me.ParseModifiedIdentifier(True, False)
			If (modifiedIdentifierSyntax.ContainsDiagnostics AndAlso Me.PeekAheadFor(New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("8C3F66DC34C68D84B4A7EB85074243DFCA233E7142F2F0494915C4EB3876825A").FieldHandle }) = SyntaxKind.AsKeyword) Then
				modifiedIdentifierSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax)(modifiedIdentifierSyntax, New SyntaxKind() { SyntaxKind.AsKeyword })
			End If
			Dim currentToken As KeywordSyntax = Nothing
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = Nothing
			Dim asClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax = Nothing
			If (Me.CurrentToken.Kind = SyntaxKind.AsKeyword) Then
				currentToken = DirectCast(Me.CurrentToken, KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
				typeSyntax = Me.ParseGeneralType(False)
				asClauseSyntax = Me.SyntaxFactory.SimpleAsClause(currentToken, New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)(), typeSyntax)
			End If
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax)()
			separatedSyntaxListBuilder.Add(modifiedIdentifierSyntax)
			Dim variableDeclaratorSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax = Me.SyntaxFactory.VariableDeclarator(separatedSyntaxListBuilder.ToList(), asClauseSyntax, Nothing)
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax)(separatedSyntaxListBuilder)
			Return variableDeclaratorSyntax
		End Function

		Private Function ParseForStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			If (Not Me.TryGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(SyntaxKind.EachKeyword, keywordSyntax)) Then
				statementSyntax = Me.ParseForStatement(currentToken)
			Else
				statementSyntax = Me.ParseForEachStatement(currentToken, keywordSyntax)
			End If
			Return statementSyntax
		End Function

		Private Function ParseForStatement(ByVal forKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Me.ParseForLoopControlVariable()
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Nothing
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Nothing
			If (visualBasicSyntaxNode.ContainsDiagnostics) Then
				visualBasicSyntaxNode = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(visualBasicSyntaxNode, New SyntaxKind() { SyntaxKind.EqualsToken, SyntaxKind.ToKeyword })
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.EqualsToken, punctuationSyntax, False, ScannerState.VB)) Then
				punctuationSyntax = DirectCast(Parser.HandleUnexpectedToken(SyntaxKind.EqualsToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
				expressionSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression().AddTrailingSyntax(Me.ResyncAt(New SyntaxKind() { SyntaxKind.ToKeyword }), ERRID.ERR_Syntax)
			Else
				expressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
				If (expressionSyntax.ContainsDiagnostics) Then
					expressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax, New SyntaxKind() { SyntaxKind.ToKeyword })
				End If
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			If (Not Me.TryGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(SyntaxKind.ToKeyword, keywordSyntax)) Then
				keywordSyntax = DirectCast(Parser.HandleUnexpectedToken(SyntaxKind.ToKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				expressionSyntax1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression().AddTrailingSyntax(Me.ResyncAt(New SyntaxKind() { SyntaxKind.ToKeyword }))
			Else
				expressionSyntax1 = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
				If (expressionSyntax1.ContainsDiagnostics) Then
					expressionSyntax1 = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax1, New SyntaxKind() { SyntaxKind.StepKeyword })
				End If
			End If
			Dim forStepClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax = Nothing
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Dim expressionSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Nothing
			If (Me.TryGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(SyntaxKind.StepKeyword, keywordSyntax1)) Then
				expressionSyntax2 = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
				If (expressionSyntax2.ContainsDiagnostics) Then
					expressionSyntax2 = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax2)
				End If
				forStepClauseSyntax = Me.SyntaxFactory.ForStepClause(keywordSyntax1, expressionSyntax2)
			End If
			Return Me.SyntaxFactory.ForStatement(forKeyword, visualBasicSyntaxNode, punctuationSyntax, expressionSyntax, keywordSyntax, expressionSyntax1, forStepClauseSyntax)
		End Function

		Private Function ParseFromControlVars() As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax)
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax)()
			While True
				Dim modifiedIdentifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax = Me.ParseNullableModifiedIdentifier()
				If (modifiedIdentifierSyntax.ContainsDiagnostics) Then
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me.PeekAheadFor(New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("309DCA8F63E598582A4836BA101576447C59167DDA55354C8D05E5487084CBCD").FieldHandle })
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken) Then
						modifiedIdentifierSyntax = modifiedIdentifierSyntax.AddTrailingSyntax(Me.ResyncAt(New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { syntaxKind }))
					End If
				End If
				If (Me.CurrentToken.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QuestionToken AndAlso (Me.PeekToken(1).Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InKeyword OrElse Me.PeekToken(1).Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken)) Then
					Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.CurrentToken
					modifiedIdentifierSyntax = modifiedIdentifierSyntax.AddTrailingSyntax(Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(currentToken, ERRID.ERR_NullableTypeInferenceNotSupported))
					Me.GetNextToken(ScannerState.VB)
				End If
				Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = Nothing
				Dim flag As Boolean = False
				If (Me.CurrentToken.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsKeyword) Then
					Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
					Me.GetNextToken(ScannerState.VB)
					If (Me.CurrentToken.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InKeyword) Then
						flag = True
					End If
					Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = Me.ParseGeneralType(False)
					simpleAsClauseSyntax = Me.SyntaxFactory.SimpleAsClause(keywordSyntax, New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)(), typeSyntax)
					If (typeSyntax.ContainsDiagnostics) Then
						Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me.PeekAheadFor(New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("D73EC862EA2DFA1E569D367C052B1C6EB5FB5E9CA942E926081558B6EC7C13DE").FieldHandle })
						If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsKeyword OrElse syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InKeyword OrElse syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken) Then
							simpleAsClauseSyntax = simpleAsClauseSyntax.AddTrailingSyntax(Me.ResyncAt(New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { syntaxKind1 }))
						End If
					End If
				End If
				Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
				Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Nothing
				If (Me.TryEatNewLineAndGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InKeyword, keywordSyntax1, True, ScannerState.VB)) Then
					If (Not flag) Then
						Me.TryEatNewLine(ScannerState.VB)
					End If
					expressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
				End If
				If (expressionSyntax Is Nothing OrElse expressionSyntax.ContainsDiagnostics) Then
					expressionSyntax = If(expressionSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression())
					Dim syntaxKind2 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me.PeekAheadFor(New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("985E90C91B60AFC57C95B6862CD842D734F1E65DE0BADA5FB77B4DF14C9C1C10").FieldHandle })
					If (syntaxKind2 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken) Then
						expressionSyntax = expressionSyntax.AddTrailingSyntax(Me.ResyncAt(New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { syntaxKind2 }))
					End If
				End If
				Dim collectionRangeVariableSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax = Me.SyntaxFactory.CollectionRangeVariable(modifiedIdentifierSyntax, simpleAsClauseSyntax, keywordSyntax1, expressionSyntax)
				separatedSyntaxListBuilder.Add(collectionRangeVariableSyntax)
				Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken, punctuationSyntax, False, ScannerState.VB)) Then
					Exit While
				End If
				separatedSyntaxListBuilder.AddSeparator(punctuationSyntax)
			End While
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax) = separatedSyntaxListBuilder.ToList()
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax)(separatedSyntaxListBuilder)
			Return list
		End Function

		Private Function ParseFromOperator(ByVal FromKw As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FromClauseSyntax
			Me.TryEatNewLine(ScannerState.VB)
			Return Me.SyntaxFactory.FromClause(FromKw, Me.ParseFromControlVars())
		End Function

		Private Function ParseFromQueryExpression(ByVal fromKw As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryExpressionSyntax
			Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax) = Me._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax)()
			syntaxListBuilder.Add(Me.ParseFromOperator(fromKw))
			Me.ParseMoreQueryOperators(syntaxListBuilder)
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax) = syntaxListBuilder.ToList()
			Me._pool.Free(syntaxListBuilder)
			Return Me.SyntaxFactory.QueryExpression(list)
		End Function

		Private Sub ParseFunctionOrDelegateStatement(ByVal kind As SyntaxKind, ByRef ident As IdentifierTokenSyntax, ByRef optionalGenericParams As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax, ByRef optionalParameters As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByRef asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByRef handlesClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax, ByRef implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax)
			If (Me.CurrentToken.Kind <> SyntaxKind.NewKeyword) Then
				ident = Me.ParseIdentifier()
				If (ident.ContainsDiagnostics) Then
					ident = ident.AddTrailingSyntax(Me.ResyncAt(New SyntaxKind() { SyntaxKind.OpenParenToken, SyntaxKind.AsKeyword }))
				End If
			Else
				ident = Me.ParseIdentifierAllowingKeyword()
				ident = Parser.ReportSyntaxError(Of IdentifierTokenSyntax)(ident, ERRID.ERR_ConstructorFunction)
			End If
			If (Me.BeginsGeneric(False, False)) Then
				optionalGenericParams = Me.ParseGenericParameters()
			End If
			optionalParameters = Me.ParseParameterList()
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = Nothing
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
			Dim currentToken As KeywordSyntax = Nothing
			If (Me.CurrentToken.Kind = SyntaxKind.AsKeyword) Then
				currentToken = DirectCast(Me.CurrentToken, KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
				If (Me.CurrentToken.Kind = SyntaxKind.LessThanToken) Then
					syntaxList = Me.ParseAttributeLists(False)
				End If
				typeSyntax = Me.ParseGeneralType(False)
				If (typeSyntax.ContainsDiagnostics) Then
					typeSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)(typeSyntax)
				End If
				asClause = Me.SyntaxFactory.SimpleAsClause(currentToken, syntaxList, typeSyntax)
			End If
			If (Me.CurrentToken.Kind = SyntaxKind.HandlesKeyword) Then
				handlesClause = Me.ParseHandlesList()
				If (kind = SyntaxKind.DelegateFunctionStatement) Then
					handlesClause = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax)(handlesClause, ERRID.ERR_DelegateCantHandleEvents)
					Return
				End If
			ElseIf (Me.CurrentToken.Kind = SyntaxKind.ImplementsKeyword) Then
				implementsClause = Me.ParseImplementsList()
				If (kind = SyntaxKind.DelegateFunctionStatement) Then
					implementsClause = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax)(implementsClause, ERRID.ERR_DelegateCantImplement)
				End If
			End If
		End Sub

		Private Function ParseFunctionOrSubLambdaHeader(<Out> ByRef isMultiLine As Boolean, Optional ByVal parseModifiers As Boolean = False) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim flag As Boolean = Me._isInMethodDeclarationHeader
			Me._isInMethodDeclarationHeader = True
			Dim flag1 As Boolean = Me._isInAsyncMethodDeclarationHeader
			Dim flag2 As Boolean = Me._isInIteratorMethodDeclarationHeader
			If (Not parseModifiers) Then
				syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)()
				Me._isInAsyncMethodDeclarationHeader = False
				Me._isInIteratorMethodDeclarationHeader = False
			Else
				syntaxList = Me.ParseSpecifiers()
				Me._isInAsyncMethodDeclarationHeader = syntaxList.Any(630)
				Me._isInIteratorMethodDeclarationHeader = syntaxList.Any(632)
			End If
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax = Nothing
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax)()
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			isMultiLine = False
			If (Me.CurrentToken.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken) Then
				Me.TryRejectGenericParametersForMemberDecl(typeParameterListSyntax)
			End If
			If (Me.CurrentToken.Kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken) Then
				punctuationSyntax = DirectCast(Parser.HandleUnexpectedToken(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
				punctuationSyntax1 = DirectCast(Parser.HandleUnexpectedToken(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			Else
				separatedSyntaxList = Me.ParseParameters(punctuationSyntax, punctuationSyntax1)
			End If
			If (typeParameterListSyntax IsNot Nothing) Then
				punctuationSyntax = punctuationSyntax.AddLeadingSyntax(typeParameterListSyntax, ERRID.ERR_GenericParamsOnInvalidMember)
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = Nothing
			Dim syntaxList2 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			If (Me.CurrentToken.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsKeyword) Then
				keywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
				If (Me.CurrentToken.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanToken) Then
					syntaxList2 = Me.ParseAttributeLists(False)
					keywordSyntax = keywordSyntax.AddTrailingSyntax(syntaxList2.Node, ERRID.ERR_AttributeOnLambdaReturnType)
				End If
				Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = Me.ParseGeneralType(False)
				If (typeSyntax.ContainsDiagnostics) Then
					typeSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)(typeSyntax)
				End If
				Dim syntaxFactory As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContextAwareSyntaxFactory = Me.SyntaxFactory
				syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)()
				simpleAsClauseSyntax = syntaxFactory.SimpleAsClause(keywordSyntax, syntaxList1, typeSyntax)
				isMultiLine = True
			End If
			If (currentToken.Kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionKeyword AndAlso simpleAsClauseSyntax IsNot Nothing) Then
				punctuationSyntax1 = punctuationSyntax1.AddTrailingSyntax(simpleAsClauseSyntax, ERRID.ERR_ExpectedEOS)
				simpleAsClauseSyntax = Nothing
			End If
			isMultiLine = If(isMultiLine, True, Me.CurrentToken.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StatementTerminatorToken)
			Me._isInMethodDeclarationHeader = flag
			Me._isInAsyncMethodDeclarationHeader = flag1
			Me._isInIteratorMethodDeclarationHeader = flag2
			Dim contextAwareSyntaxFactory As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContextAwareSyntaxFactory = Me.SyntaxFactory
			syntaxKind = If(currentToken.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionKeyword, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionLambdaHeader, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubLambdaHeader)
			syntaxList1 = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)()
			Return contextAwareSyntaxFactory.LambdaHeader(syntaxKind, syntaxList1, syntaxList, currentToken, Me.SyntaxFactory.ParameterList(punctuationSyntax, separatedSyntaxList, punctuationSyntax1), simpleAsClauseSyntax)
		End Function

		Private Function ParseFunctionStatement(ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim flag As Boolean = Me._isInMethodDeclarationHeader
			Me._isInMethodDeclarationHeader = True
			Dim flag1 As Boolean = Me._isInAsyncMethodDeclarationHeader
			Dim flag2 As Boolean = Me._isInIteratorMethodDeclarationHeader
			Me._isInAsyncMethodDeclarationHeader = modifiers.Any(630)
			Me._isInIteratorMethodDeclarationHeader = modifiers.Any(632)
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = Nothing
			Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax = Nothing
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax = Nothing
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = Nothing
			Dim handlesClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax = Nothing
			Dim implementsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax = Nothing
			Me.ParseFunctionOrDelegateStatement(SyntaxKind.FunctionStatement, identifierTokenSyntax, typeParameterListSyntax, parameterListSyntax, simpleAsClauseSyntax, handlesClauseSyntax, implementsClauseSyntax)
			Me._isInMethodDeclarationHeader = flag
			Me._isInAsyncMethodDeclarationHeader = flag1
			Me._isInIteratorMethodDeclarationHeader = flag2
			Return Me.SyntaxFactory.FunctionStatement(attributes, modifiers, currentToken, identifierTokenSyntax, typeParameterListSyntax, parameterListSyntax, simpleAsClauseSyntax, handlesClauseSyntax, implementsClauseSyntax)
		End Function

		Friend Function ParseGeneralType(Optional ByVal allowEmptyGenericArguments As Boolean = False) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax
			Dim typeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.CurrentToken
			If (Not Me._evaluatingConditionCompilationExpression OrElse SyntaxFacts.IsPredefinedTypeOrVariant(currentToken.Kind)) Then
				Dim flag As Boolean = False
				typeSyntax1 = Me.ParseTypeName(False, allowEmptyGenericArguments, flag)
				If (Me.CurrentToken.Kind = SyntaxKind.OpenParenToken) Then
					Dim typeSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = typeSyntax1
					Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax) = Me.ParseArrayRankSpecifiers(ERRID.ERR_NoExplicitArraySizes)
					If (flag) Then
						syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax)(Parser.ReportSyntaxError(Of GreenNode)(syntaxList.Node, ERRID.ERR_ArrayOfRawGenericInvalid))
					End If
					typeSyntax1 = Me.SyntaxFactory.ArrayType(typeSyntax2, syntaxList)
				End If
				typeSyntax = typeSyntax1
			Else
				Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier().AddTrailingSyntax(currentToken, ERRID.ERR_BadTypeInCCExpression)
				typeSyntax1 = Me.SyntaxFactory.IdentifierName(identifierTokenSyntax)
				Me.GetNextToken(ScannerState.VB)
				typeSyntax = typeSyntax1
			End If
			Return typeSyntax
		End Function

		Private Function ParseGenericArguments(ByRef allowEmptyGenericArguments As Boolean, ByRef AllowNonEmptyGenericArguments As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			Me.GetNextToken(ScannerState.VB)
			Me.TryEatNewLine(ScannerState.VB)
			Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(SyntaxKind.OfKeyword, keywordSyntax, True, ScannerState.VB)
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)()
			While True
				Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = Nothing
				If (Me.CurrentToken.Kind <> SyntaxKind.CommaToken AndAlso Me.CurrentToken.Kind <> SyntaxKind.CloseParenToken) Then
					typeSyntax = Me.ParseGeneralType(False)
					If (Not AllowNonEmptyGenericArguments) Then
						typeSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)(typeSyntax, ERRID.ERR_TypeParamMissingCommaOrRParen)
					Else
						allowEmptyGenericArguments = False
					End If
				ElseIf (Not allowEmptyGenericArguments) Then
					typeSyntax = Me.ParseGeneralType(False)
				Else
					typeSyntax = Me.SyntaxFactory.IdentifierName(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier())
					AllowNonEmptyGenericArguments = False
				End If
				If (typeSyntax.ContainsDiagnostics) Then
					typeSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)(typeSyntax, New SyntaxKind() { SyntaxKind.CloseParenToken, SyntaxKind.CommaToken })
				End If
				separatedSyntaxListBuilder.Add(typeSyntax)
				Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax1, False, ScannerState.VB)) Then
					Exit While
				End If
				separatedSyntaxListBuilder.AddSeparator(punctuationSyntax1)
			End While
			If (currentToken IsNot Nothing) Then
				Me.TryEatNewLineAndGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CloseParenToken, punctuationSyntax, True, ScannerState.VB)
			End If
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax) = separatedSyntaxListBuilder.ToList()
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)(separatedSyntaxListBuilder)
			Return Me.SyntaxFactory.TypeArgumentList(currentToken, keywordSyntax, list, punctuationSyntax)
		End Function

		Private Function ParseGenericParameters() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.OpenParenToken, punctuationSyntax, False, ScannerState.VB)
			Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(SyntaxKind.OfKeyword, keywordSyntax, True, ScannerState.VB)
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax)()
			While True
				Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = Nothing
				Dim currentToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
				If (Me.CurrentToken.Kind <> SyntaxKind.InKeyword) Then
					Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
					If (Me.TryTokenAsContextualKeyword(Me.CurrentToken, SyntaxKind.OutKeyword, keywordSyntax1)) Then
						Dim identifierTokenSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
						Me.GetNextToken(ScannerState.VB)
						Me.TryEatNewLineIfFollowedBy(SyntaxKind.CloseParenToken)
						If (Me.CurrentToken.Kind = SyntaxKind.CloseParenToken OrElse Me.CurrentToken.Kind = SyntaxKind.CommaToken OrElse Me.CurrentToken.Kind = SyntaxKind.AsKeyword) Then
							identifierTokenSyntax = identifierTokenSyntax1
							currentToken1 = Nothing
						Else
							keywordSyntax1 = Me.CheckFeatureAvailability(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(Feature.CoContraVariance, keywordSyntax1)
							currentToken1 = keywordSyntax1
						End If
					End If
				Else
					currentToken1 = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
					currentToken1 = Me.CheckFeatureAvailability(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(Feature.CoContraVariance, currentToken1)
					Me.GetNextToken(ScannerState.VB)
				End If
				If (identifierTokenSyntax Is Nothing) Then
					identifierTokenSyntax = Me.ParseIdentifier()
				End If
				Dim typeParameterConstraintClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax = Nothing
				currentToken = Nothing
				If (Me.CurrentToken.Kind = SyntaxKind.AsKeyword) Then
					currentToken = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
					Me.GetNextToken(ScannerState.VB)
					Dim punctuationSyntax3 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
					If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.OpenBraceToken, punctuationSyntax3, False, ScannerState.VB)) Then
						Dim constraintSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax = Me.ParseConstraintSyntax()
						If (constraintSyntax.ContainsDiagnostics) Then
							constraintSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax)(constraintSyntax, New SyntaxKind() { SyntaxKind.CloseParenToken })
						End If
						typeParameterConstraintClauseSyntax = Me.SyntaxFactory.TypeParameterSingleConstraintClause(currentToken, constraintSyntax)
					Else
						Dim separatedSyntaxListBuilder1 As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax)()
						While True
							Dim constraintSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax = Me.ParseConstraintSyntax()
							If (constraintSyntax1.ContainsDiagnostics) Then
								constraintSyntax1 = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax)(constraintSyntax1, New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("AF4CA0617393AB927DA48E7F0EE887EEF07B9BCFBACA3AD5376EE79470D75622").FieldHandle })
							End If
							separatedSyntaxListBuilder1.Add(constraintSyntax1)
							punctuationSyntax2 = Nothing
							If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax2, False, ScannerState.VB)) Then
								Exit While
							End If
							separatedSyntaxListBuilder1.AddSeparator(punctuationSyntax2)
						End While
						Dim punctuationSyntax4 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
						Me.TryEatNewLineAndGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CloseBraceToken, punctuationSyntax4, True, ScannerState.VB)
						Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax) = separatedSyntaxListBuilder1.ToList()
						Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax)(separatedSyntaxListBuilder1)
						typeParameterConstraintClauseSyntax = Me.SyntaxFactory.TypeParameterMultipleConstraintClause(currentToken, punctuationSyntax3, list, punctuationSyntax4)
					End If
				End If
				Dim typeParameterSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax = Me.SyntaxFactory.TypeParameter(currentToken1, identifierTokenSyntax, typeParameterConstraintClauseSyntax)
				separatedSyntaxListBuilder.Add(typeParameterSyntax)
				punctuationSyntax2 = Nothing
				If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax2, False, ScannerState.VB)) Then
					Exit While
				End If
				separatedSyntaxListBuilder.AddSeparator(punctuationSyntax2)
			End While
			If (punctuationSyntax IsNot Nothing AndAlso Not Me.TryEatNewLineAndGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CloseParenToken, punctuationSyntax1, False, ScannerState.VB)) Then
				punctuationSyntax1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.CloseParenToken)
				punctuationSyntax1 = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(punctuationSyntax1, If(currentToken Is Nothing, ERRID.ERR_TypeParamMissingAsCommaOrRParen, ERRID.ERR_TypeParamMissingCommaOrRParen))
			End If
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax) = separatedSyntaxListBuilder.ToList()
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax)(separatedSyntaxListBuilder)
			Return Me.SyntaxFactory.TypeParameterList(punctuationSyntax, keywordSyntax, separatedSyntaxList, punctuationSyntax1)
		End Function

		Private Function ParseGetType() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetTypeExpressionSyntax
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.OpenParenToken, punctuationSyntax, True, ScannerState.VB)
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = Me.ParseGeneralType(True)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.TryEatNewLineAndGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CloseParenToken, punctuationSyntax1, True, ScannerState.VB)
			Return Me.SyntaxFactory.GetTypeExpression(currentToken, punctuationSyntax, typeSyntax, punctuationSyntax1)
		End Function

		Private Function ParseGetXmlNamespace() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim xmlNamespaceExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			If (Me.CurrentToken.Kind <> SyntaxKind.OpenParenToken) Then
				Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Parser.HandleUnexpectedToken(SyntaxKind.OpenParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
				Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.CloseParenToken)
				xmlNamespaceExpression = Me.SyntaxFactory.GetXmlNamespaceExpression(currentToken, punctuationSyntax, Nothing, punctuationSyntax1)
			Else
				Me.ResetCurrentToken(ScannerState.Element)
				Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.OpenParenToken, punctuationSyntax2, ScannerState.Element)
				Dim xmlPrefixNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax = Nothing
				If (Me.CurrentToken.Kind = SyntaxKind.XmlNameToken) Then
					xmlPrefixNameSyntax = Me.SyntaxFactory.XmlPrefixName(DirectCast(Me.CurrentToken, XmlNameTokenSyntax))
					Me.GetNextToken(ScannerState.Element)
				End If
				Dim punctuationSyntax3 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CloseParenToken, punctuationSyntax3, ScannerState.VB)
				Dim getXmlNamespaceExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetXmlNamespaceExpressionSyntax = Me.SyntaxFactory.GetXmlNamespaceExpression(currentToken, punctuationSyntax2, xmlPrefixNameSyntax, punctuationSyntax3)
				xmlNamespaceExpression = Me.TransitionFromXmlToVB(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetXmlNamespaceExpressionSyntax)(Parser.AdjustTriviaForMissingTokens(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetXmlNamespaceExpressionSyntax)(getXmlNamespaceExpressionSyntax))
			End If
			Return xmlNamespaceExpression
		End Function

		Private Function ParseGotoStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.ParseLabelReference()
			Return Me.SyntaxFactory.GoToStatement(currentToken, Me.GetLabelSyntaxForIdentifierOrLineNumber(syntaxToken))
		End Function

		Private Function ParseGroupByExpression(ByVal groupKw As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupByClauseSyntax
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax)()
			If (Not Me.TryEatNewLineAndGetContextualKeyword(SyntaxKind.ByKeyword, keywordSyntax, False)) Then
				Me.TryEatNewLine(ScannerState.VB)
				separatedSyntaxList = Me.ParseSelectList()
			End If
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax)()
			If (keywordSyntax IsNot Nothing OrElse Me.TryEatNewLineAndGetContextualKeyword(SyntaxKind.ByKeyword, keywordSyntax, True)) Then
				Me.TryEatNewLine(ScannerState.VB)
				list = Me.ParseSelectList()
			Else
				Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax)()
				separatedSyntaxListBuilder.Add(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.ExpressionRangeVariable(Nothing, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression()))
				list = separatedSyntaxListBuilder.ToList()
				Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax)(separatedSyntaxListBuilder)
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Dim separatedSyntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax)()
			If (Not Me.TryEatNewLineAndGetContextualKeyword(SyntaxKind.IntoKeyword, keywordSyntax1, True)) Then
				separatedSyntaxList1 = Me.MissingAggregationRangeVariables()
			Else
				Me.TryEatNewLine(ScannerState.VB)
				separatedSyntaxList1 = Me.ParseAggregateList(True, False)
			End If
			Return Me.SyntaxFactory.GroupByClause(groupKw, separatedSyntaxList, keywordSyntax, list, keywordSyntax1, separatedSyntaxList1)
		End Function

		Private Function ParseGroupEndStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Dim syntaxToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.PeekToken(1)
			If (Me.IsValidStatementTerminator(syntaxToken1)) Then
				syntaxToken = Nothing
			Else
				syntaxToken = syntaxToken1
			End If
			Dim syntaxToken2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = syntaxToken
			Dim endStatementKindFromKeyword As SyntaxKind = Parser.GetEndStatementKindFromKeyword(syntaxToken1.Kind)
			If (endStatementKindFromKeyword <> SyntaxKind.None) Then
				Me.GetNextToken(ScannerState.VB)
				Me.GetNextToken(ScannerState.VB)
				statementSyntax = Me.SyntaxFactory.EndBlockStatement(endStatementKindFromKeyword, currentToken, DirectCast(syntaxToken2, KeywordSyntax))
			Else
				statementSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax)(Me.ParseStopOrEndStatement(), ERRID.ERR_UnrecognizedEnd)
			End If
			Return statementSyntax
		End Function

		Private Function ParseHandlerStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = If(currentToken.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerKeyword, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerStatement, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerStatement)
			Me.GetNextToken(ScannerState.VB)
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
			If (expressionSyntax.ContainsDiagnostics) Then
				expressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax, New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken })
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken, punctuationSyntax, True, ScannerState.VB)
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
			If (expressionSyntax1.ContainsDiagnostics) Then
				expressionSyntax1 = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax1)
			End If
			Return Me.SyntaxFactory.AddRemoveHandlerStatement(syntaxKind, currentToken, expressionSyntax, punctuationSyntax, expressionSyntax1)
		End Function

		Private Function ParseHandlesList() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax
			Dim eventContainerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventContainerSyntax
			Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax)()
			Me.GetNextToken(ScannerState.VB)
			While True
				If (Me.CurrentToken.Kind = SyntaxKind.MyBaseKeyword OrElse Me.CurrentToken.Kind = SyntaxKind.MyClassKeyword OrElse Me.CurrentToken.Kind = SyntaxKind.MeKeyword) Then
					eventContainerSyntax = Me.SyntaxFactory.KeywordEventContainer(DirectCast(Me.CurrentToken, KeywordSyntax))
					Me.GetNextToken(ScannerState.VB)
				ElseIf (Me.CurrentToken.Kind <> SyntaxKind.GlobalKeyword) Then
					eventContainerSyntax = Me.SyntaxFactory.WithEventsEventContainer(Me.ParseIdentifier())
				Else
					eventContainerSyntax = Me.SyntaxFactory.WithEventsEventContainer(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier())
					eventContainerSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventContainerSyntax)(eventContainerSyntax, ERRID.ERR_NoGlobalInHandles)
				End If
				Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.DotToken, punctuationSyntax, True, ScannerState.VB)) Then
					identifierNameSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.IdentifierName(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier())
				Else
					identifierNameSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.IdentifierName(Me.ParseIdentifierAllowingKeyword())
					Dim withEventsEventContainerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsEventContainerSyntax = TryCast(eventContainerSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsEventContainerSyntax)
					Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
					If (withEventsEventContainerSyntax IsNot Nothing AndAlso Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.DotToken, punctuationSyntax1, True, ScannerState.VB)) Then
						eventContainerSyntax = Me.SyntaxFactory.WithEventsPropertyEventContainer(withEventsEventContainerSyntax, punctuationSyntax, identifierNameSyntax)
						punctuationSyntax = punctuationSyntax1
						identifierNameSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.IdentifierName(Me.ParseIdentifierAllowingKeyword())
					End If
				End If
				Dim handlesClauseItemSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax = Me.SyntaxFactory.HandlesClauseItem(eventContainerSyntax, punctuationSyntax, identifierNameSyntax)
				If ((eventContainerSyntax.ContainsDiagnostics OrElse punctuationSyntax.ContainsDiagnostics OrElse identifierNameSyntax.ContainsDiagnostics) AndAlso Me.CurrentToken.Kind <> SyntaxKind.CommaToken) Then
					handlesClauseItemSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax)(handlesClauseItemSyntax, New SyntaxKind() { SyntaxKind.CommaToken })
				End If
				separatedSyntaxListBuilder.Add(handlesClauseItemSyntax)
				Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax2, False, ScannerState.VB)) Then
					Exit While
				End If
				separatedSyntaxListBuilder.AddSeparator(punctuationSyntax2)
			End While
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax) = separatedSyntaxListBuilder.ToList()
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax)(separatedSyntaxListBuilder)
			Return Me.SyntaxFactory.HandlesClause(currentToken, list)
		End Function

		Private Function ParseIdentifier() As IdentifierTokenSyntax
			Dim currentToken As IdentifierTokenSyntax
			If (Me.CurrentToken.Kind = SyntaxKind.IdentifierToken) Then
				currentToken = DirectCast(Me.CurrentToken, IdentifierTokenSyntax)
				If (currentToken.ContextualKind = SyntaxKind.AwaitKeyword AndAlso Me.IsWithinAsyncMethodOrLambda OrElse currentToken.ContextualKind = SyntaxKind.YieldKeyword AndAlso Me.IsWithinIteratorContext) Then
					currentToken = Parser.ReportSyntaxError(Of IdentifierTokenSyntax)(currentToken, ERRID.ERR_InvalidUseOfKeyword)
				End If
				Me.GetNextToken(ScannerState.VB)
			ElseIf (Not Me.CurrentToken.IsKeyword) Then
				currentToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier()
				If (Me.CurrentToken.Kind <> SyntaxKind.BadToken OrElse EmbeddedOperators.CompareString(Me.CurrentToken.Text, "_", False) <> 0) Then
					currentToken = Parser.ReportSyntaxError(Of IdentifierTokenSyntax)(currentToken, ERRID.ERR_ExpectedIdentifier)
				Else
					currentToken = currentToken.AddLeadingSyntax(Me.CurrentToken, ERRID.ERR_ExpectedIdentifier)
					Me.GetNextToken(ScannerState.VB)
				End If
			Else
				Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				currentToken = Me._scanner.MakeIdentifier(keywordSyntax)
				currentToken = Parser.ReportSyntaxError(Of IdentifierTokenSyntax)(currentToken, ERRID.ERR_InvalidUseOfKeyword)
				Me.GetNextToken(ScannerState.VB)
			End If
			Return currentToken
		End Function

		Private Function ParseIdentifierAllowingKeyword() As IdentifierTokenSyntax
			Dim currentToken As IdentifierTokenSyntax
			If (Me.CurrentToken.Kind = SyntaxKind.IdentifierToken) Then
				currentToken = DirectCast(Me.CurrentToken, IdentifierTokenSyntax)
				Me.GetNextToken(ScannerState.VB)
			ElseIf (Not Me.CurrentToken.IsKeyword) Then
				currentToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier()
				currentToken = Parser.ReportSyntaxError(Of IdentifierTokenSyntax)(currentToken, ERRID.ERR_ExpectedIdentifier)
			Else
				Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				currentToken = Me._scanner.MakeIdentifier(keywordSyntax)
				Me.GetNextToken(ScannerState.VB)
			End If
			Return currentToken
		End Function

		Private Function ParseIdentifierNameAllowingKeyword() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = Me.ParseIdentifierAllowingKeyword()
			Return Me.SyntaxFactory.IdentifierName(identifierTokenSyntax)
		End Function

		Private Function ParseIfDirective(ByVal hashToken As PunctuationSyntax, ByVal elseKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax
			Dim ifDirectiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseConditionalCompilationExpression()
			If (expressionSyntax.ContainsDiagnostics) Then
				expressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax)
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			If (Me.CurrentToken.Kind = SyntaxKind.ThenKeyword) Then
				keywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
			End If
			ifDirectiveTriviaSyntax = If(currentToken.Kind <> SyntaxKind.IfKeyword OrElse elseKeyword IsNot Nothing, Me.SyntaxFactory.ElseIfDirectiveTrivia(hashToken, elseKeyword, currentToken, expressionSyntax, keywordSyntax), Me.SyntaxFactory.IfDirectiveTrivia(hashToken, elseKeyword, currentToken, expressionSyntax, keywordSyntax))
			Return ifDirectiveTriviaSyntax
		End Function

		Private Function ParseIfExpression() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim withSeparators As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			If (Me.CurrentToken.Kind <> SyntaxKind.OpenParenToken) Then
				expressionSyntax = Me.SyntaxFactory.BinaryConditionalExpression(currentToken, DirectCast(Parser.HandleUnexpectedToken(SyntaxKind.OpenParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression(), DirectCast(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.CommaToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression(), DirectCast(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.CloseParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax))
			Else
				Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax = Me.ParseParenthesizedArguments(False, False)
				Dim arguments As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax) = argumentListSyntax.Arguments
				Select Case arguments.Count
					Case 0
						expressionSyntax = Me.SyntaxFactory.BinaryConditionalExpression(currentToken, argumentListSyntax.OpenParenToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression().WithDiagnostics(New DiagnosticInfo() { ErrorFactory.ErrorInfo(ERRID.ERR_IllegalOperandInIIFCount) }), DirectCast(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.CommaToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression(), argumentListSyntax.CloseParenToken)
						Exit Select
					Case 1
						expressionSyntax = Me.SyntaxFactory.BinaryConditionalExpression(currentToken, argumentListSyntax.OpenParenToken, Parser.GetArgumentAsExpression(arguments(0)), DirectCast(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.CommaToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression().WithDiagnostics(New DiagnosticInfo() { ErrorFactory.ErrorInfo(ERRID.ERR_IllegalOperandInIIFCount) }), argumentListSyntax.CloseParenToken)
						Exit Select
					Case 2
						Dim syntaxFactory As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContextAwareSyntaxFactory = Me.SyntaxFactory
						Dim openParenToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = argumentListSyntax.OpenParenToken
						Dim argumentAsExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Parser.GetArgumentAsExpression(arguments(0))
						withSeparators = arguments.GetWithSeparators()
						expressionSyntax = syntaxFactory.BinaryConditionalExpression(currentToken, openParenToken, argumentAsExpression, DirectCast(withSeparators(1), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax), Parser.GetArgumentAsExpression(arguments(1)), argumentListSyntax.CloseParenToken)
						Exit Select
					Case 3
						Dim contextAwareSyntaxFactory As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContextAwareSyntaxFactory = Me.SyntaxFactory
						Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = argumentListSyntax.OpenParenToken
						Dim argumentAsExpression1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Parser.GetArgumentAsExpression(arguments(0))
						withSeparators = arguments.GetWithSeparators()
						Dim item As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(withSeparators(1), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
						Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Parser.GetArgumentAsExpression(arguments(1))
						withSeparators = arguments.GetWithSeparators()
						expressionSyntax = contextAwareSyntaxFactory.TernaryConditionalExpression(currentToken, punctuationSyntax, argumentAsExpression1, item, expressionSyntax1, DirectCast(withSeparators(3), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax), Parser.GetArgumentAsExpression(arguments(2)), argumentListSyntax.CloseParenToken)
						Exit Select
					Case Else
						Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = arguments.GetWithSeparators()
						Dim visualBasicSyntaxNodeArray(syntaxList.Count - 5 - 1 + 1 - 1) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
						Dim count As Integer = syntaxList.Count - 1
						Dim num As Integer = 5
						Do
							visualBasicSyntaxNodeArray(num - 5) = syntaxList(num)
							num = num + 1
						Loop While num <= count
						expressionSyntax = Me.SyntaxFactory.TernaryConditionalExpression(currentToken, argumentListSyntax.OpenParenToken, Parser.GetArgumentAsExpression(arguments(0)), DirectCast(syntaxList(1), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax), Parser.GetArgumentAsExpression(arguments(1)), DirectCast(syntaxList(3), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax), Parser.GetArgumentAsExpression(arguments(2)), argumentListSyntax.CloseParenToken.AddLeadingSyntax(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList.List(ArrayElement(Of GreenNode).MakeElementArray(visualBasicSyntaxNodeArray)), ERRID.ERR_IllegalOperandInIIFCount))
						Exit Select
				End Select
			End If
			Return expressionSyntax
		End Function

		Private Function ParseIfStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
			If (expressionSyntax.ContainsDiagnostics) Then
				expressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax, New SyntaxKind() { SyntaxKind.ThenKeyword })
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Me.TryGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(SyntaxKind.ThenKeyword, keywordSyntax)
			Return Me.SyntaxFactory.IfStatement(currentToken, expressionSyntax, keywordSyntax)
		End Function

		Private Function ParseImplementsList() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax)()
			Me.GetNextToken(ScannerState.VB)
			While True
				Dim flag As Boolean = False
				Dim qualifiedNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax = DirectCast(Me.ParseName(True, True, True, True, True, True, False, flag, False), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax)
				separatedSyntaxListBuilder.Add(qualifiedNameSyntax)
				Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax, False, ScannerState.VB)) Then
					Exit While
				End If
				separatedSyntaxListBuilder.AddSeparator(punctuationSyntax)
			End While
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax) = separatedSyntaxListBuilder.ToList()
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax)(separatedSyntaxListBuilder)
			Return Me.SyntaxFactory.ImplementsClause(currentToken, list)
		End Function

		Private Function ParseImportsStatement(ByVal Attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax), ByVal Specifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsStatementSyntax
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Me.ReportModifiersOnStatementError(Attributes, Specifiers, DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax))
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsClauseSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsClauseSyntax)()
			Me.GetNextToken(ScannerState.VB)
			While True
				separatedSyntaxListBuilder.Add(Me.ParseOneImportsDirective())
				Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax, False, ScannerState.VB)) Then
					Exit While
				End If
				separatedSyntaxListBuilder.AddSeparator(punctuationSyntax)
			End While
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsClauseSyntax) = separatedSyntaxListBuilder.ToList()
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsClauseSyntax)(separatedSyntaxListBuilder)
			Return Me.SyntaxFactory.ImportsStatement(keywordSyntax, list)
		End Function

		Private Function ParseInheritsImplementsStatement(ByVal Attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax), ByVal Specifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsOrImplementsStatementSyntax
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Me.ReportModifiersOnStatementError(Attributes, Specifiers, DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax))
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)()
			Me.GetNextToken(ScannerState.VB)
			While True
				Dim flag As Boolean = False
				Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = Me.ParseTypeName(True, False, flag)
				If (typeSyntax.ContainsDiagnostics) Then
					typeSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)(typeSyntax, New SyntaxKind() { SyntaxKind.CommaToken })
				End If
				separatedSyntaxListBuilder.Add(typeSyntax)
				Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax, False, ScannerState.VB)) Then
					Exit While
				End If
				separatedSyntaxListBuilder.AddSeparator(punctuationSyntax)
			End While
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax) = separatedSyntaxListBuilder.ToList()
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)(separatedSyntaxListBuilder)
			Dim inheritsOrImplementsStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsOrImplementsStatementSyntax = Nothing
			Dim kind As SyntaxKind = keywordSyntax.Kind
			If (kind = SyntaxKind.ImplementsKeyword) Then
				inheritsOrImplementsStatementSyntax = Me.SyntaxFactory.ImplementsStatement(keywordSyntax, list)
			Else
				If (kind <> SyntaxKind.InheritsKeyword) Then
					Throw ExceptionUtilities.UnexpectedValue(keywordSyntax.Kind)
				End If
				inheritsOrImplementsStatementSyntax = Me.SyntaxFactory.InheritsStatement(keywordSyntax, list)
			End If
			Return inheritsOrImplementsStatementSyntax
		End Function

		Private Function ParseInnerJoinOrGroupJoinExpression(ByVal groupKw As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal joinKw As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinClauseSyntax
			Dim joinClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinClauseSyntax
			Me.TryEatNewLine(ScannerState.VB)
			Dim collectionRangeVariableSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax = Me.ParseJoinControlVar()
			Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinClauseSyntax) = Me._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinClauseSyntax)()
			While True
				Dim joinClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinClauseSyntax = Me.ParseOptionalJoinOperator()
				If (joinClauseSyntax1 Is Nothing) Then
					Exit While
				End If
				syntaxListBuilder.Add(joinClauseSyntax1)
			End While
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax)()
			If (Not Me.TryEatNewLineAndGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(SyntaxKind.OnKeyword, keywordSyntax, True, ScannerState.VB)) Then
				Dim joinConditionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax = Me.SyntaxFactory.JoinCondition(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.EqualsKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression())
				separatedSyntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax)(joinConditionSyntax)
			Else
				Me.TryEatNewLine(ScannerState.VB)
				separatedSyntaxList = Me.ParseJoinPredicateExpression()
			End If
			Dim separatedSyntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax)(collectionRangeVariableSyntax)
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinClauseSyntax) = syntaxListBuilder.ToList()
			Me._pool.Free(syntaxListBuilder)
			If (groupKw IsNot Nothing) Then
				Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
				Dim separatedSyntaxList2 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax)()
				If (Not Me.TryEatNewLineAndGetContextualKeyword(SyntaxKind.IntoKeyword, keywordSyntax1, True)) Then
					separatedSyntaxList2 = Me.MissingAggregationRangeVariables()
				Else
					Me.TryEatNewLine(ScannerState.VB)
					separatedSyntaxList2 = Me.ParseAggregateList(True, True)
				End If
				joinClauseSyntax = Me.SyntaxFactory.GroupJoinClause(groupKw, joinKw, separatedSyntaxList1, list, keywordSyntax, separatedSyntaxList, keywordSyntax1, separatedSyntaxList2)
			Else
				joinClauseSyntax = Me.SyntaxFactory.SimpleJoinClause(joinKw, separatedSyntaxList1, list, keywordSyntax, separatedSyntaxList)
			End If
			Return joinClauseSyntax
		End Function

		Private Sub ParseInternalSubSet(ByVal builder As SyntaxListBuilder(Of GreenNode))
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)()
			If (Me.CurrentToken.Kind <> SyntaxKind.BadToken OrElse DirectCast(Me.CurrentToken, BadTokenSyntax).SubKind <> SyntaxSubKind.OpenBracketToken) Then
				syntaxList = Me.ResyncAt(ScannerState.DocType, New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("BD5CBF415E8757D997DDB85E45EB3057281480E21FCED76A337979996A7A1E7D").FieldHandle })
				If (syntaxList.Node IsNot Nothing) Then
					builder.Add(syntaxList.Node)
				End If
			End If
			If (Me.CurrentToken.Kind = SyntaxKind.BadToken AndAlso DirectCast(Me.CurrentToken, BadTokenSyntax).SubKind = SyntaxSubKind.OpenBracketToken) Then
				builder.Add(Me.CurrentToken)
				Me.GetNextToken(ScannerState.DocType)
				If (Me.CurrentToken.Kind = SyntaxKind.BadToken AndAlso DirectCast(Me.CurrentToken, BadTokenSyntax).SubKind = SyntaxSubKind.LessThanExclamationToken) Then
					builder.Add(Me.CurrentToken)
					Me.GetNextToken(ScannerState.DocType)
					Me.ParseXmlMarkupDecl(builder)
				End If
				If (Me.CurrentToken.Kind <> SyntaxKind.BadToken OrElse DirectCast(Me.CurrentToken, BadTokenSyntax).SubKind <> SyntaxSubKind.CloseBracketToken) Then
					syntaxList = Me.ResyncAt(ScannerState.DocType, New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("BD5CBF415E8757D997DDB85E45EB3057281480E21FCED76A337979996A7A1E7D").FieldHandle })
					If (syntaxList.Node IsNot Nothing) Then
						builder.Add(syntaxList.Node)
					End If
				End If
				builder.Add(Me.CurrentToken)
				Me.GetNextToken(ScannerState.DocType)
			End If
		End Sub

		Private Function ParseInterpolatedStringExpression() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax
			Dim interpolatedStringExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax
			Dim interpolatedStringContentSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringContentSyntax
			Me.ResetCurrentToken(ScannerState.InterpolatedStringPunctuation)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			Me.GetNextToken(ScannerState.InterpolatedStringContent)
			Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringContentSyntax) = Me._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringContentSyntax)()
			Dim syntaxListBuilder1 As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = New SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)()
			While True
				Dim kind As SyntaxKind = Me.CurrentToken.Kind
				If (kind <= SyntaxKind.CloseBraceToken) Then
					If (kind = SyntaxKind.OpenBraceToken) Then
						interpolatedStringContentSyntax = Me.ParseInterpolatedStringInterpolation()
					ElseIf (kind = SyntaxKind.CloseBraceToken) Then
						If (syntaxListBuilder1.IsNull) Then
							syntaxListBuilder1 = Me._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)()
						End If
						syntaxListBuilder1.Add(Me.CurrentToken)
						Me.GetNextToken(ScannerState.InterpolatedStringContent)
						Continue While
					Else
						Exit While
					End If
				ElseIf (kind = SyntaxKind.DoubleQuoteToken) Then
					currentToken = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
					Me.GetNextToken(ScannerState.VB)
					If (Not syntaxListBuilder1.IsNull) Then
						currentToken = currentToken.AddLeadingSyntax(Me._pool.ToListAndFree(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(syntaxListBuilder1), ERRID.ERR_Syntax)
						syntaxListBuilder1 = New SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)()
					End If
					interpolatedStringExpressionSyntax = Me.SyntaxFactory.InterpolatedStringExpression(punctuationSyntax, Me._pool.ToListAndFree(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringContentSyntax)(syntaxListBuilder), currentToken)
					Return Me.CheckFeatureAvailability(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax)(Feature.InterpolatedStrings, interpolatedStringExpressionSyntax)
				ElseIf (kind = SyntaxKind.InterpolatedStringTextToken) Then
					Dim interpolatedStringTextTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextTokenSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextTokenSyntax)
					Me.GetNextToken(ScannerState.InterpolatedStringPunctuation)
					interpolatedStringContentSyntax = Me.SyntaxFactory.InterpolatedStringText(interpolatedStringTextTokenSyntax)
				ElseIf (kind = SyntaxKind.EndOfInterpolatedStringToken) Then
					currentToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.DoubleQuoteToken)
					Me.GetNextToken(ScannerState.VB)
					If (Not syntaxListBuilder1.IsNull) Then
						currentToken = currentToken.AddLeadingSyntax(Me._pool.ToListAndFree(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(syntaxListBuilder1), ERRID.ERR_Syntax)
						syntaxListBuilder1 = New SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)()
					End If
					interpolatedStringExpressionSyntax = Me.SyntaxFactory.InterpolatedStringExpression(punctuationSyntax, Me._pool.ToListAndFree(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringContentSyntax)(syntaxListBuilder), currentToken)
					Return Me.CheckFeatureAvailability(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax)(Feature.InterpolatedStrings, interpolatedStringExpressionSyntax)
				Else
					Exit While
				End If
				If (Not syntaxListBuilder1.IsNull) Then
					interpolatedStringContentSyntax = interpolatedStringContentSyntax.AddLeadingSyntax(Me._pool.ToListAndFree(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(syntaxListBuilder1), ERRID.ERR_Syntax)
					syntaxListBuilder1 = New SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)()
				End If
				syntaxListBuilder.Add(interpolatedStringContentSyntax)
			End While
			currentToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.DoubleQuoteToken)
			If (Not syntaxListBuilder1.IsNull) Then
				currentToken = currentToken.AddLeadingSyntax(Me._pool.ToListAndFree(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(syntaxListBuilder1), ERRID.ERR_Syntax)
				syntaxListBuilder1 = New SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)()
			End If
			interpolatedStringExpressionSyntax = Me.SyntaxFactory.InterpolatedStringExpression(punctuationSyntax, Me._pool.ToListAndFree(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringContentSyntax)(syntaxListBuilder), currentToken)
			Return Me.CheckFeatureAvailability(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax)(Feature.InterpolatedStrings, interpolatedStringExpressionSyntax)
		End Function

		Private Function ParseInterpolatedStringInterpolation() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationSyntax
			Dim colonToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim interpolationAlignmentClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax
			Dim interpolationFormatClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax
			Dim integerLiteralTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IntegerLiteralTokenSyntax
			Dim interpolatedStringTextTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextTokenSyntax
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Dim str As String = Nothing
			Dim currentToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			Me.GetNextToken(ScannerState.VB)
			If (Me.CurrentToken.Kind <> SyntaxKind.ColonToken) Then
				colonToken = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
				If (Me.CurrentToken.Kind = SyntaxKind.ColonToken) Then
					colonToken = DirectCast(Me.RemoveTrailingColonTriviaAndConvertToColonToken(colonToken, punctuationSyntax1, str), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
				End If
			Else
				currentToken1 = DirectCast(Parser.RemoveTrailingColonTriviaAndConvertToColonToken(currentToken1, punctuationSyntax1, str), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
				colonToken = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression(), ERRID.ERR_ExpectedExpression)
			End If
			If (Me.CurrentToken.Kind <> SyntaxKind.CommaToken) Then
				interpolationAlignmentClauseSyntax = Nothing
			Else
				Dim colonToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
				Me.GetNextToken(ScannerState.VB)
				If (Me.CurrentToken.Kind = SyntaxKind.ColonToken) Then
					colonToken1 = DirectCast(Parser.RemoveTrailingColonTriviaAndConvertToColonToken(colonToken1, punctuationSyntax1, str), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
				End If
				If (Me.CurrentToken.Kind = SyntaxKind.MinusToken OrElse Me.CurrentToken.Kind = SyntaxKind.PlusToken) Then
					punctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
					Me.GetNextToken(ScannerState.VB)
					If (Me.CurrentToken.Kind = SyntaxKind.ColonToken) Then
						punctuationSyntax = DirectCast(Parser.RemoveTrailingColonTriviaAndConvertToColonToken(punctuationSyntax, punctuationSyntax1, str), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
					End If
				Else
					punctuationSyntax = Nothing
				End If
				If (Me.CurrentToken.Kind <> SyntaxKind.IntegerLiteralToken) Then
					integerLiteralTokenSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IntegerLiteralTokenSyntax)(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIntegerLiteralToken(), ERRID.ERR_ExpectedIntLiteral)
				Else
					integerLiteralTokenSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IntegerLiteralTokenSyntax)
					Me.GetNextToken(ScannerState.VB)
					If (Me.CurrentToken.Kind = SyntaxKind.ColonToken) Then
						integerLiteralTokenSyntax = DirectCast(Parser.RemoveTrailingColonTriviaAndConvertToColonToken(integerLiteralTokenSyntax, punctuationSyntax1, str), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IntegerLiteralTokenSyntax)
					End If
				End If
				Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.SyntaxFactory.NumericLiteralExpression(integerLiteralTokenSyntax)
				If (punctuationSyntax IsNot Nothing) Then
					expressionSyntax = Me.SyntaxFactory.UnaryExpression(If(punctuationSyntax.Kind = SyntaxKind.PlusToken, SyntaxKind.UnaryPlusExpression, SyntaxKind.UnaryMinusExpression), punctuationSyntax, expressionSyntax)
				End If
				interpolationAlignmentClauseSyntax = Me.SyntaxFactory.InterpolationAlignmentClause(colonToken1, expressionSyntax)
			End If
			If (Me.CurrentToken.Kind <> SyntaxKind.ColonToken OrElse punctuationSyntax1 Is Nothing) Then
				interpolationFormatClauseSyntax = Nothing
				If (Me.CurrentToken.Kind = SyntaxKind.ColonToken) Then
					Me.GetNextToken(ScannerState.InterpolatedStringFormatString)
				End If
			Else
				Me.GetNextToken(ScannerState.InterpolatedStringFormatString)
				If (Me.CurrentToken.Kind = SyntaxKind.InterpolatedStringTextToken) Then
					interpolatedStringTextTokenSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextTokenSyntax)
					Me.GetNextToken(ScannerState.InterpolatedStringPunctuation)
					If (str IsNot Nothing) Then
						interpolatedStringTextTokenSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.InterpolatedStringTextToken([String].Concat(str, interpolatedStringTextTokenSyntax.Text), [String].Concat(str, interpolatedStringTextTokenSyntax.Value), interpolatedStringTextTokenSyntax.GetLeadingTrivia(), interpolatedStringTextTokenSyntax.GetTrailingTrivia())
					End If
				ElseIf (str Is Nothing) Then
					interpolatedStringTextTokenSyntax = Nothing
				Else
					interpolatedStringTextTokenSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.InterpolatedStringTextToken(str, str, Nothing, Nothing)
				End If
				If (interpolatedStringTextTokenSyntax Is Nothing) Then
					interpolatedStringTextTokenSyntax = DirectCast(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.InterpolatedStringTextToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextTokenSyntax)
					interpolatedStringTextTokenSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextTokenSyntax)(interpolatedStringTextTokenSyntax, ERRID.ERR_Syntax)
				ElseIf (interpolatedStringTextTokenSyntax.GetTrailingTrivia() IsNot Nothing) Then
					interpolatedStringTextTokenSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextTokenSyntax)(interpolatedStringTextTokenSyntax, ERRID.ERR_InterpolationFormatWhitespace)
				End If
				interpolationFormatClauseSyntax = Me.SyntaxFactory.InterpolationFormatClause(punctuationSyntax1, interpolatedStringTextTokenSyntax)
			End If
			If (Me.CurrentToken.Kind = SyntaxKind.CloseBraceToken) Then
				Me.ResetCurrentToken(ScannerState.InterpolatedStringPunctuation)
				currentToken = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
				Me.GetNextToken(ScannerState.InterpolatedStringContent)
			ElseIf (Me.CurrentToken.Kind <> SyntaxKind.EndOfInterpolatedStringToken) Then
				If (Not Me.IsValidStatementTerminator(Me.CurrentToken)) Then
					Me.ResetCurrentToken(ScannerState.InterpolatedStringFormatString)
				End If
				currentToken = DirectCast(Parser.HandleUnexpectedToken(SyntaxKind.CloseBraceToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
				If (Me.CurrentToken.Kind = SyntaxKind.InterpolatedStringTextToken) Then
					Me.ResetCurrentToken(ScannerState.InterpolatedStringContent)
					Me.GetNextToken(ScannerState.InterpolatedStringContent)
				End If
			Else
				Me.GetNextToken(ScannerState.VB)
				currentToken = DirectCast(Parser.HandleUnexpectedToken(SyntaxKind.CloseBraceToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			End If
			Return Me.SyntaxFactory.Interpolation(currentToken1, colonToken, interpolationAlignmentClauseSyntax, interpolationFormatClauseSyntax, currentToken)
		End Function

		Private Function ParseIntLiteral() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim literalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax = Me.SyntaxFactory.NumericLiteralExpression(Me.CurrentToken)
			Me.GetNextToken(ScannerState.VB)
			Return literalExpressionSyntax
		End Function

		Private Function ParseJoinControlVar() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax
			Dim modifiedIdentifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax = Me.ParseNullableModifiedIdentifier()
			If (modifiedIdentifierSyntax.ContainsDiagnostics) Then
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me.PeekAheadFor(New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("652BDEB827CD3F1992D0E0536A4BCCCC28F14EFC3D1F06643D6978FDDE89879A").FieldHandle })
				If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InKeyword) Then
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InKeyword) Then
						GoTo Label1
					End If
					GoTo Label0
				ElseIf (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword AndAlso syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GroupKeyword AndAlso syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinKeyword) Then
					GoTo Label0
				End If
			Label1:
				Dim syntaxKindArray() As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = { syntaxKind }
				modifiedIdentifierSyntax = modifiedIdentifierSyntax.AddTrailingSyntax(Me.ResyncAt(syntaxKindArray))
			End If
		Label0:
			If (Me.CurrentToken.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QuestionToken AndAlso Me.PeekToken(1).Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InKeyword) Then
				Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.CurrentToken
				modifiedIdentifierSyntax = modifiedIdentifierSyntax.AddTrailingSyntax(Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(currentToken, ERRID.ERR_NullableTypeInferenceNotSupported))
				Me.GetNextToken(ScannerState.VB)
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = Nothing
			If (Me.CurrentToken.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsKeyword) Then
				Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
				Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = Me.ParseGeneralType(False)
				simpleAsClauseSyntax = Me.SyntaxFactory.SimpleAsClause(keywordSyntax, New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)(), typeSyntax)
				If (typeSyntax.ContainsDiagnostics) Then
					Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me.PeekAheadFor(New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("E53C30D708D7D500EA238A38F47B4756C483D9E266EA9773FF66EFD88EB96D6D").FieldHandle })
					If (syntaxKind1 <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword) Then
						If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InKeyword OrElse syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword) Then
							GoTo Label3
						End If
						GoTo Label2
					ElseIf (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GroupKeyword AndAlso syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinKeyword AndAlso syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken) Then
						GoTo Label2
					End If
				Label3:
					Dim syntaxKindArray1() As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = { syntaxKind1 }
					simpleAsClauseSyntax = simpleAsClauseSyntax.AddTrailingSyntax(Me.ResyncAt(syntaxKindArray1))
				End If
			End If
		Label2:
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Nothing
			If (Me.TryEatNewLineAndGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InKeyword, keywordSyntax1, True, ScannerState.VB)) Then
				Me.TryEatNewLine(ScannerState.VB)
				expressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
			End If
			If (expressionSyntax Is Nothing OrElse expressionSyntax.ContainsDiagnostics) Then
				expressionSyntax = If(expressionSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression())
				Dim syntaxKind2 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me.PeekAheadFor(New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("985E90C91B60AFC57C95B6862CD842D734F1E65DE0BADA5FB77B4DF14C9C1C10").FieldHandle })
				If (syntaxKind2 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken) Then
					expressionSyntax = expressionSyntax.AddTrailingSyntax(Me.ResyncAt(New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { syntaxKind2 }))
				End If
			End If
			Return Me.SyntaxFactory.CollectionRangeVariable(modifiedIdentifierSyntax, simpleAsClauseSyntax, keywordSyntax1, expressionSyntax)
		End Function

		Private Function ParseJoinPredicateExpression() As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax)
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax)()
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Do
				Dim joinConditionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax = Nothing
				If (Me.CurrentToken.Kind = SyntaxKind.StatementTerminatorToken) Then
					joinConditionSyntax = Me.SyntaxFactory.JoinCondition(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.EqualsKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression())
					joinConditionSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax)(joinConditionSyntax, ERRID.ERR_ExpectedExpression)
				Else
					Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceRelational, False)
					If (expressionSyntax.ContainsDiagnostics) Then
						expressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax, New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("E6A28E29154F8B51DEC46BBA1B83E9B8A57147D01318CE542EA96BD1EAD5054C").FieldHandle })
					End If
					Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
					Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Nothing
					expressionSyntax1 = If(Not Me.TryGetContextualKeywordAndEatNewLine(SyntaxKind.EqualsKeyword, keywordSyntax1, True), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression(), Me.ParseExpressionCore(OperatorPrecedence.PrecedenceRelational, False))
					joinConditionSyntax = Me.SyntaxFactory.JoinCondition(expressionSyntax, keywordSyntax1, expressionSyntax1)
				End If
				If (joinConditionSyntax.ContainsDiagnostics) Then
					joinConditionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax)(joinConditionSyntax, New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("A1F5CAF92BC45B07811901B77F4FDE7FDB2256E085F58CC1DFAB1229F4A0CB82").FieldHandle })
				End If
				If (separatedSyntaxListBuilder.Count > 0) Then
					separatedSyntaxListBuilder.AddSeparator(keywordSyntax)
				End If
				separatedSyntaxListBuilder.Add(joinConditionSyntax)
			Loop While Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(SyntaxKind.AndKeyword, keywordSyntax, False, ScannerState.VB)
			If (Me.CurrentToken.Kind = SyntaxKind.AndAlsoKeyword OrElse Me.CurrentToken.Kind = SyntaxKind.OrKeyword OrElse Me.CurrentToken.Kind = SyntaxKind.OrElseKeyword) Then
				separatedSyntaxListBuilder(separatedSyntaxListBuilder.Count - 1) = separatedSyntaxListBuilder(separatedSyntaxListBuilder.Count - 1).AddTrailingSyntax(DirectCast(Me.CurrentToken, GreenNode), ERRID.ERR_ExpectedAnd)
				Me.GetNextToken(ScannerState.VB)
			End If
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax) = separatedSyntaxListBuilder.ToList()
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax)(separatedSyntaxListBuilder)
			If (list.Node.ContainsDiagnostics) Then
				Dim node As GreenNode = list.Node
				node = Me.ResyncAt(Of GreenNode)(node, New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("DA0954000C545638BB436B25326A98E0C6C92EB3AAE045D5CF3FD3A0DF80E1A0").FieldHandle })
				list = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax)(New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax)(node))
			End If
			Return list
		End Function

		Private Function ParseLabel() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelStatementSyntax
			Dim labelStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelStatementSyntax
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.ParseLabelReference()
			If (syntaxToken.Kind <> SyntaxKind.IntegerLiteralToken OrElse Me.CurrentToken.Kind = SyntaxKind.ColonToken) Then
				Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(syntaxToken.GetTrailingTrivia())
				Dim num As Integer = -1
				Dim count As Integer = syntaxList.Count - 1
				Dim num1 As Integer = 0
				While num1 <= count
					If (syntaxList(num1).Kind <> SyntaxKind.ColonTrivia) Then
						num1 = num1 + 1
					Else
						num = num1
						Exit While
					End If
				End While
				Dim startOfTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = syntaxList.GetStartOfTrivia(num)
				syntaxToken = DirectCast(syntaxToken.WithTrailingTrivia(startOfTrivia.Node), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
				Dim item As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia = DirectCast(syntaxList(num), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia)
				syntaxList = syntaxList.GetEndOfTrivia(num + 1)
				Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax(SyntaxKind.ColonToken, item.Text, Nothing, syntaxList.Node)
				labelStatementSyntax = Me.SyntaxFactory.LabelStatement(syntaxToken, punctuationSyntax)
			Else
				labelStatementSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelStatementSyntax)(Me.SyntaxFactory.LabelStatement(syntaxToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.ColonToken)), ERRID.ERR_ObsoleteLineNumbersAreLabels)
			End If
			Return labelStatementSyntax
		End Function

		Private Function ParseLabelReference() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.CurrentToken
			If (currentToken.Kind = SyntaxKind.IdentifierToken) Then
				If (DirectCast(currentToken, IdentifierTokenSyntax).TypeCharacter = TypeCharacter.None) Then
					syntaxToken = Me.ParseIdentifier()
				Else
					currentToken = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(currentToken, ERRID.ERR_NoTypecharInLabel)
					Me.GetNextToken(ScannerState.VB)
					syntaxToken = currentToken
				End If
			ElseIf (currentToken.Kind <> SyntaxKind.IntegerLiteralToken) Then
				currentToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier()
				currentToken = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(currentToken, ERRID.ERR_ExpectedIdentifier)
				syntaxToken = currentToken
			Else
				Dim integerLiteralTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IntegerLiteralTokenSyntax = DirectCast(currentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IntegerLiteralTokenSyntax)
				If (Not integerLiteralTokenSyntax.ContainsDiagnostics) Then
					If (integerLiteralTokenSyntax.TypeSuffix <> TypeCharacter.None) Then
						integerLiteralTokenSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IntegerLiteralTokenSyntax)(integerLiteralTokenSyntax, ERRID.ERR_Syntax)
					Else
						Dim num As ULong = Microsoft.VisualBasic.CompilerServices.Conversions.ToULong(integerLiteralTokenSyntax.ObjectValue)
						If (TypeOf integerLiteralTokenSyntax Is IntegerLiteralTokenSyntax(Of Integer)) Then
							num = CULng(CUInt(num))
						End If
						integerLiteralTokenSyntax = New IntegerLiteralTokenSyntax(Of ULong)(SyntaxKind.IntegerLiteralToken, integerLiteralTokenSyntax.ToString(), integerLiteralTokenSyntax.GetLeadingTrivia(), integerLiteralTokenSyntax.GetTrailingTrivia(), integerLiteralTokenSyntax.Base, TypeCharacter.None, num)
					End If
				End If
				Me.GetNextToken(ScannerState.VB)
				syntaxToken = integerLiteralTokenSyntax
			End If
			Return syntaxToken
		End Function

		Private Function ParseLambda(ByVal parseModifiers As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim singleLineLambdaContext As MethodBlockContext
			Dim flag As Boolean = False
			Dim lambdaHeaderSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax = Me.ParseFunctionOrSubLambdaHeader(flag, parseModifiers)
			lambdaHeaderSyntax = Parser.AdjustTriviaForMissingTokens(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax)(lambdaHeaderSyntax)
			If (lambdaHeaderSyntax.Kind <> SyntaxKind.FunctionLambdaHeader OrElse flag) Then
				Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext = Me._context
				If (Not flag) Then
					singleLineLambdaContext = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaContext(lambdaHeaderSyntax, blockContext)
				Else
					singleLineLambdaContext = New LambdaContext(lambdaHeaderSyntax, blockContext)
				End If
				Me._context = singleLineLambdaContext
				If (flag OrElse Me.CurrentToken.Kind = SyntaxKind.ColonToken) Then
					Me._context = Me._context.ResyncAndProcessStatementTerminator(lambdaHeaderSyntax, singleLineLambdaContext)
				End If
				Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax = Nothing
				While Me._context.Level >= singleLineLambdaContext.Level
					If (Not Me.CurrentToken.IsEndOfParse) Then
						statementSyntax = Me._context.Parse()
						statementSyntax = Parser.AdjustTriviaForMissingTokens(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(statementSyntax)
						Dim flag1 As Boolean = Parser.IsDeclarationStatement(statementSyntax.Kind)
						If (Not flag1) Then
							Me._context = Me._context.LinkSyntax(statementSyntax)
							If (Me._context.Level < singleLineLambdaContext.Level) Then
								Exit While
							End If
						Else
							Me._context.Add(Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(statementSyntax, ERRID.ERR_InvInsideEndsProc))
						End If
						Me._context = Me._context.ResyncAndProcessStatementTerminator(statementSyntax, singleLineLambdaContext)
						statementSyntax = Nothing
						If (Not flag1) Then
							Continue While
						End If
						If (Me._context.Level < singleLineLambdaContext.Level) Then
							Exit While
						End If
						Me._context = Me._context.EndLambda()
						Exit While
					Else
						Me._context = Me._context.EndLambda()
						Exit While
					End If
				End While
				expressionSyntax = DirectCast(singleLineLambdaContext.CreateBlockSyntax(statementSyntax), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			Else
				Dim singleLineLambdaContext1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaContext = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaContext(lambdaHeaderSyntax, Me._context)
				Me._context = singleLineLambdaContext1
				expressionSyntax = Me.SyntaxFactory.SingleLineLambdaExpression(SyntaxKind.SingleLineFunctionLambdaExpression, lambdaHeaderSyntax, Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False))
				expressionSyntax = Parser.AdjustTriviaForMissingTokens(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax)
				If (lambdaHeaderSyntax.Modifiers.Any(632)) Then
					expressionSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax, ERRID.ERR_BadIteratorExpressionLambda)
				End If
				Me._context = singleLineLambdaContext1.PrevBlock
			End If
			If (flag) Then
				expressionSyntax = Me.CheckFeatureAvailability(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(Feature.StatementLambdas, expressionSyntax)
			End If
			Return expressionSyntax
		End Function

		Private Function ParseLetList() As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax)
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax)()
			While True
				Dim modifiedIdentifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax = Me.ParseNullableModifiedIdentifier()
				If (modifiedIdentifierSyntax.ContainsDiagnostics) Then
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me.PeekAheadFor(New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("309DCA8F63E598582A4836BA101576447C59167DDA55354C8D05E5487084CBCD").FieldHandle })
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken) Then
						modifiedIdentifierSyntax = modifiedIdentifierSyntax.AddTrailingSyntax(Me.ResyncAt(New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { syntaxKind }))
					End If
				End If
				If (Me.CurrentToken.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QuestionToken AndAlso (Me.PeekToken(1).Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InKeyword OrElse Me.PeekToken(1).Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken)) Then
					Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.CurrentToken
					modifiedIdentifierSyntax = modifiedIdentifierSyntax.AddTrailingSyntax(Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(currentToken, ERRID.ERR_NullableTypeInferenceNotSupported))
					Me.GetNextToken(ScannerState.VB)
				End If
				Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = Nothing
				Dim flag As Boolean = False
				If (Me.CurrentToken.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsKeyword) Then
					Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
					Me.GetNextToken(ScannerState.VB)
					If (Me.CurrentToken.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InKeyword) Then
						flag = True
					End If
					Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = Me.ParseGeneralType(False)
					simpleAsClauseSyntax = Me.SyntaxFactory.SimpleAsClause(keywordSyntax, New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)(), typeSyntax)
					If (typeSyntax.ContainsDiagnostics) Then
						Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me.PeekAheadFor(New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("D73EC862EA2DFA1E569D367C052B1C6EB5FB5E9CA942E926081558B6EC7C13DE").FieldHandle })
						If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsKeyword OrElse syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InKeyword OrElse syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken) Then
							simpleAsClauseSyntax = simpleAsClauseSyntax.AddTrailingSyntax(Me.ResyncAt(New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { syntaxKind1 }))
						End If
					End If
				End If
				Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Nothing
				If (Me.TryGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken, punctuationSyntax)) Then
					If (Not flag) Then
						Me.TryEatNewLine(ScannerState.VB)
					End If
					expressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
				Else
					punctuationSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken)
					punctuationSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(punctuationSyntax, ERRID.ERR_ExpectedAssignmentOperator)
				End If
				If (expressionSyntax Is Nothing OrElse expressionSyntax.ContainsDiagnostics) Then
					expressionSyntax = If(expressionSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression())
					Dim syntaxKind2 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me.PeekAheadFor(New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("985E90C91B60AFC57C95B6862CD842D734F1E65DE0BADA5FB77B4DF14C9C1C10").FieldHandle })
					If (syntaxKind2 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken) Then
						expressionSyntax = expressionSyntax.AddTrailingSyntax(Me.ResyncAt(New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { syntaxKind2 }))
					End If
				End If
				Dim expressionRangeVariableSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax = Me.SyntaxFactory.ExpressionRangeVariable(Me.SyntaxFactory.VariableNameEquals(modifiedIdentifierSyntax, simpleAsClauseSyntax, punctuationSyntax), expressionSyntax)
				separatedSyntaxListBuilder.Add(expressionRangeVariableSyntax)
				Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken, punctuationSyntax1, False, ScannerState.VB)) Then
					Exit While
				End If
				separatedSyntaxListBuilder.AddSeparator(punctuationSyntax1)
			End While
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax) = separatedSyntaxListBuilder.ToList()
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax)(separatedSyntaxListBuilder)
			Return list
		End Function

		Private Function ParseLetOperator(ByVal LetKw As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LetClauseSyntax
			Me.TryEatNewLine(ScannerState.VB)
			Return Me.SyntaxFactory.LetClause(LetKw, Me.ParseLetList())
		End Function

		Private Function ParseLoopStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim whileOrUntilClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax = Nothing
			Me.TryParseOptionalWhileOrUntilClause(currentToken, whileOrUntilClauseSyntax)
			If (whileOrUntilClauseSyntax IsNot Nothing) Then
				syntaxKind = If(whileOrUntilClauseSyntax.Kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileClause, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LoopUntilStatement, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LoopWhileStatement)
			Else
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleLoopStatement
			End If
			Return Me.SyntaxFactory.LoopStatement(syntaxKind, currentToken, whileOrUntilClauseSyntax)
		End Function

		Private Function ParseMid() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax
			Dim currentToken As IdentifierTokenSyntax = DirectCast(Me.CurrentToken, IdentifierTokenSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.OpenParenToken, punctuationSyntax, False, ScannerState.VB)
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax)()
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			separatedSyntaxListBuilder.Add(Me.ParseArgument(False))
			If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax1, False, ScannerState.VB)) Then
				Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax1, ScannerState.VB)
			End If
			separatedSyntaxListBuilder.AddSeparator(punctuationSyntax1)
			separatedSyntaxListBuilder.Add(Me.ParseArgument(False))
			If (Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax1, False, ScannerState.VB)) Then
				separatedSyntaxListBuilder.AddSeparator(punctuationSyntax1)
				separatedSyntaxListBuilder.Add(Me.ParseArgument(False))
			End If
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax) = separatedSyntaxListBuilder.ToList()
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax)(separatedSyntaxListBuilder)
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.TryEatNewLineAndGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CloseParenToken, punctuationSyntax2, True, ScannerState.VB)
			Dim punctuationSyntax3 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.EqualsToken, punctuationSyntax3, ScannerState.VB)
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
			If (expressionSyntax.ContainsDiagnostics) Then
				expressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax)
			End If
			Return Me.SyntaxFactory.MidAssignmentStatement(Me.SyntaxFactory.MidExpression(currentToken, Me.SyntaxFactory.ArgumentList(punctuationSyntax, list, punctuationSyntax2)), punctuationSyntax3, expressionSyntax)
		End Function

		Private Function ParseModifiedIdentifier(ByVal AllowExplicitArraySizes As Boolean, ByVal checkForCustom As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax
			Dim modifiedIdentifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)
			Dim prevToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.PrevToken
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.CurrentToken
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Dim flag As Boolean = False
			If (checkForCustom) Then
				Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
				If (Me.TryTokenAsContextualKeyword(currentToken, SyntaxKind.CustomKeyword, keywordSyntax)) Then
					Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.PeekToken(1)
					flag = If(SyntaxFacts.IsSpecifier(syntaxToken.Kind), True, SyntaxFacts.CanStartSpecifierDeclaration(syntaxToken.Kind))
				End If
			End If
			If (Not SyntaxFacts.IsSpecifier(currentToken.Kind)) Then
				identifierTokenSyntax = Me.ParseNullableIdentifier(punctuationSyntax)
				If (flag) Then
					identifierTokenSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)(identifierTokenSyntax, ERRID.ERR_InvalidUseOfCustomModifier)
				End If
			Else
				If (prevToken IsNot Nothing AndAlso prevToken.IsEndOfLine) Then
					identifierTokenSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier()
					identifierTokenSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)(identifierTokenSyntax, ERRID.ERR_ExpectedIdentifier)
					Dim syntaxFactory As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContextAwareSyntaxFactory = Me.SyntaxFactory
					syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)()
					modifiedIdentifierSyntax = syntaxFactory.ModifiedIdentifier(identifierTokenSyntax, Nothing, Nothing, syntaxList)
					Return modifiedIdentifierSyntax
				End If
				Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax) = Me.ParseSpecifiers()
				identifierTokenSyntax = Me.ParseNullableIdentifier(punctuationSyntax).AddLeadingSyntax(syntaxList1.Node, ERRID.ERR_ExtraSpecifiers)
			End If
			If (Me.CurrentToken.Kind <> SyntaxKind.OpenParenToken) Then
				Dim contextAwareSyntaxFactory As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContextAwareSyntaxFactory = Me.SyntaxFactory
				syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)()
				modifiedIdentifierSyntax = contextAwareSyntaxFactory.ModifiedIdentifier(identifierTokenSyntax, punctuationSyntax, Nothing, syntaxList)
			Else
				modifiedIdentifierSyntax = Me.ParseArrayModifiedIdentifier(identifierTokenSyntax, punctuationSyntax, AllowExplicitArraySizes)
			End If
			Return modifiedIdentifierSyntax
		End Function

		Private Sub ParseMoreQueryOperators(ByRef operators As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax))
			While True
				If (operators.Count > 0 AndAlso operators(operators.Count - 1).ContainsDiagnostics) Then
					Dim item As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax = operators(operators.Count - 1)
					Dim fieldHandle() As SyntaxKind = { GetType(<PrivateImplementationDetails>).GetField("36CC3C6C3092DABA26D9E9E428330FBEE77FE294E62242CDA669549C793E069B").FieldHandle }
					operators(operators.Count - 1) = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax)(item, fieldHandle)
				End If
				Dim queryClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax = Me.ParseNextQueryOperator()
				If (queryClauseSyntax Is Nothing) Then
					Exit While
				End If
				operators.Add(queryClauseSyntax)
			End While
		End Sub

		Friend Function ParseName(ByVal requireQualification As Boolean, ByVal allowGlobalNameSpace As Boolean, ByVal allowGenericArguments As Boolean, ByVal allowGenericsWithoutOf As Boolean, Optional ByVal nonArrayName As Boolean = False, Optional ByVal disallowGenericArgumentsOnLastQualifiedName As Boolean = False, Optional ByVal allowEmptyGenericArguments As Boolean = False, Optional ByRef allowedEmptyGenericArguments As Boolean = False, Optional ByVal isNameInNamespaceDeclaration As Boolean = False) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax
			Dim flag As Boolean = True
			Dim nameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax = Nothing
			If (Me.CurrentToken.Kind <> SyntaxKind.GlobalKeyword) Then
				nameSyntax = Me.ParseSimpleName(allowGenericArguments, allowGenericsWithoutOf, disallowGenericArgumentsOnLastQualifiedName, nonArrayName, False, allowEmptyGenericArguments, flag)
			Else
				nameSyntax = Me.SyntaxFactory.GlobalName(DirectCast(Me.CurrentToken, KeywordSyntax))
				If (isNameInNamespaceDeclaration) Then
					nameSyntax = Me.CheckFeatureAvailability(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax)(Feature.GlobalNamespace, nameSyntax)
				End If
				Me.GetNextToken(ScannerState.VB)
				If (Not allowGlobalNameSpace) Then
					nameSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax)(nameSyntax, ERRID.ERR_NoGlobalExpectedIdentifier)
				End If
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			While Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.DotToken, punctuationSyntax, False, ScannerState.VB)
				nameSyntax = Me.SyntaxFactory.QualifiedName(nameSyntax, punctuationSyntax, Me.ParseSimpleName(allowGenericArguments, allowGenericsWithoutOf, disallowGenericArgumentsOnLastQualifiedName, nonArrayName, True, allowEmptyGenericArguments, flag))
			End While
			If (requireQualification AndAlso punctuationSyntax Is Nothing) Then
				nameSyntax = Me.SyntaxFactory.QualifiedName(nameSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.DotToken), Me.SyntaxFactory.IdentifierName(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier()))
				nameSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax)(nameSyntax, ERRID.ERR_ExpectedDot)
			End If
			allowedEmptyGenericArguments = Not flag
			Return nameSyntax
		End Function

		Private Sub ParseNamedArguments(ByVal arguments As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax))
			Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax
			Dim simpleArgumentSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax
			While True
				Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				Dim flag As Boolean = False
				If ((Me.CurrentToken.Kind = SyntaxKind.IdentifierToken OrElse Me.CurrentToken.IsKeyword) AndAlso Me.PeekToken(1).Kind = SyntaxKind.ColonEqualsToken) Then
					identifierNameSyntax = Me.ParseIdentifierNameAllowingKeyword()
					Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.ColonEqualsToken, punctuationSyntax, False, ScannerState.VB)
				Else
					identifierNameSyntax = Me.SyntaxFactory.IdentifierName(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier())
					punctuationSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.ColonEqualsToken)
					flag = True
				End If
				If (flag) Then
					identifierNameSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)(identifierNameSyntax, ERRID.ERR_ExpectedNamedArgumentInAttributeList)
				End If
				simpleArgumentSyntax = Me.SyntaxFactory.SimpleArgument(Me.SyntaxFactory.NameColonEquals(identifierNameSyntax, punctuationSyntax), Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False))
				If (Me.CurrentToken.Kind <> SyntaxKind.CommaToken) Then
					If (Me.CurrentToken.Kind = SyntaxKind.CloseParenToken OrElse Me.MustEndStatement(Me.CurrentToken)) Then
						Exit While
					End If
					simpleArgumentSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax)(simpleArgumentSyntax, ERRID.ERR_ArgumentSyntax)
					simpleArgumentSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax)(simpleArgumentSyntax, New SyntaxKind() { SyntaxKind.CommaToken, SyntaxKind.CloseParenToken })
					If (Me.CurrentToken.Kind <> SyntaxKind.CommaToken) Then
						arguments.Add(simpleArgumentSyntax)
						Return
					End If
				End If
				Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax1, False, ScannerState.VB)
				arguments.Add(simpleArgumentSyntax)
				arguments.AddSeparator(punctuationSyntax1)
			End While
			arguments.Add(simpleArgumentSyntax)
		End Sub

		Private Function ParseNameOf() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameOfExpressionSyntax
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Me.CheckFeatureAvailability(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(Feature.NameOfExpressions, DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax))
			Me.GetNextToken(ScannerState.VB)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.OpenParenToken, punctuationSyntax, True, ScannerState.VB)
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ValidateNameOfArgument(Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False), True)
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.TryEatNewLineAndGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CloseParenToken, punctuationSyntax1, True, ScannerState.VB)
			Return Me.SyntaxFactory.NameOfExpression(keywordSyntax, punctuationSyntax, expressionSyntax, punctuationSyntax1)
		End Function

		Private Function ParseNamespaceStatement(ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax), ByVal Specifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Me.ReportModifiersOnStatementError(ERRID.ERR_SpecifiersInvalidOnInheritsImplOpt, attributes, Specifiers, DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax))
			If (Me.IsScript) Then
				keywordSyntax = keywordSyntax.AddError(ERRID.ERR_NamespaceNotAllowedInScript)
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)()
			Me.GetNextToken(ScannerState.VB)
			Dim flag As Boolean = False
			Dim nameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax = Me.ParseName(False, True, False, True, False, False, False, flag, True)
			If (nameSyntax.ContainsDiagnostics) Then
				syntaxList = Me.ResyncAt()
			End If
			Dim namespaceStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax = Me.SyntaxFactory.NamespaceStatement(keywordSyntax, nameSyntax)
			If (syntaxList.Node IsNot Nothing) Then
				namespaceStatementSyntax = namespaceStatementSyntax.AddTrailingSyntax(syntaxList)
			End If
			Return namespaceStatementSyntax
		End Function

		Private Function ParseNewExpression() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			If (Me.CurrentToken.Kind <> SyntaxKind.WithKeyword) Then
				Dim flag As Boolean = False
				Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = Me.ParseTypeName(False, False, flag)
				If (typeSyntax.ContainsDiagnostics) Then
					typeSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)(typeSyntax, New SyntaxKind() { SyntaxKind.OpenParenToken })
				End If
				Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax = Nothing
				If (Me.CurrentToken.Kind = SyntaxKind.OpenParenToken) Then
					argumentListSyntax = Me.ParseParenthesizedArguments(True, False)
					Dim flag1 As Boolean = False
					Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax)()
					If (Me.CurrentToken.Kind = SyntaxKind.OpenParenToken) Then
						syntaxList1 = Me.ParseArrayRankSpecifiers(ERRID.ERR_NoConstituentArraySizes)
						flag1 = True
					ElseIf (Me.CurrentToken.Kind = SyntaxKind.OpenBraceToken) Then
						If (Me.TryReinterpretAsArraySpecifier(argumentListSyntax, syntaxList1)) Then
							argumentListSyntax = Nothing
						End If
						flag1 = True
					End If
					If (Not flag1) Then
						GoTo Label1
					End If
					Dim collectionInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax = Me.ParseCollectionInitializer()
					Dim syntaxFactory As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContextAwareSyntaxFactory = Me.SyntaxFactory
					syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)()
					expressionSyntax = syntaxFactory.ArrayCreationExpression(currentToken, syntaxList, typeSyntax, argumentListSyntax, syntaxList1, collectionInitializerSyntax)
					Return expressionSyntax
				End If
			Label1:
				Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
				If (Me.TryTokenAsContextualKeyword(Me.CurrentToken, SyntaxKind.FromKeyword, keywordSyntax) AndAlso (Me.PeekToken(1).Kind = SyntaxKind.OpenBraceToken OrElse Me.PeekToken(1).Kind = SyntaxKind.StatementTerminatorToken)) Then
					Me.GetNextToken(ScannerState.VB)
					Dim objectCollectionInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCollectionInitializerSyntax = Me.ParseObjectCollectionInitializer(keywordSyntax)
					If (Me.CurrentToken.Kind = SyntaxKind.WithKeyword) Then
						objectCollectionInitializerSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCollectionInitializerSyntax)(objectCollectionInitializerSyntax, ERRID.ERR_CantCombineInitializers)
					End If
					Dim contextAwareSyntaxFactory As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContextAwareSyntaxFactory = Me.SyntaxFactory
					syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)()
					expressionSyntax = contextAwareSyntaxFactory.ObjectCreationExpression(currentToken, syntaxList, typeSyntax, argumentListSyntax, objectCollectionInitializerSyntax)
				ElseIf (Me.CurrentToken.Kind <> SyntaxKind.WithKeyword) Then
					Dim syntaxFactory1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContextAwareSyntaxFactory = Me.SyntaxFactory
					syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)()
					expressionSyntax = syntaxFactory1.ObjectCreationExpression(currentToken, syntaxList, typeSyntax, argumentListSyntax, Nothing)
				Else
					Dim objectMemberInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax = Me.ParseObjectInitializerList(False, True)
					If (Me.CurrentToken.Kind = SyntaxKind.WithKeyword) Then
						objectMemberInitializerSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax)(objectMemberInitializerSyntax, ERRID.ERR_CantCombineInitializers)
					End If
					If (Me.TryTokenAsContextualKeyword(Me.CurrentToken, SyntaxKind.FromKeyword, keywordSyntax) AndAlso Me.PeekToken(1).Kind = SyntaxKind.OpenBraceToken) Then
						objectMemberInitializerSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax)(objectMemberInitializerSyntax, ERRID.ERR_CantCombineInitializers)
					End If
					Dim contextAwareSyntaxFactory1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContextAwareSyntaxFactory = Me.SyntaxFactory
					syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)()
					expressionSyntax = contextAwareSyntaxFactory1.ObjectCreationExpression(currentToken, syntaxList, typeSyntax, argumentListSyntax, objectMemberInitializerSyntax)
				End If
			Else
				Dim objectMemberInitializerSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax = Me.ParseObjectInitializerList(True, True)
				Dim syntaxFactory2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContextAwareSyntaxFactory = Me.SyntaxFactory
				syntaxList = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)()
				expressionSyntax = syntaxFactory2.AnonymousObjectCreationExpression(currentToken, syntaxList, objectMemberInitializerSyntax1)
			End If
			Return expressionSyntax
		End Function

		Private Function ParseNextQueryOperator() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax
			Dim queryClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.CurrentToken
			If (currentToken.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StatementTerminatorToken) Then
				If (Not Me.NextLineStartsWithStatementTerminator(0) AndAlso Me.IsContinuableQueryOperator(Me.PeekToken(1))) Then
					GoTo Label1
				End If
				queryClauseSyntax = Nothing
				Return queryClauseSyntax
			End If
		Label8:
			Dim kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = currentToken.Kind
			If (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LetKeyword) Then
				Me.GetNextToken(ScannerState.VB)
				queryClauseSyntax = Me.ParseLetOperator(DirectCast(currentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax))
			ElseIf (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword) Then
				Me.GetNextToken(ScannerState.VB)
				Me.TryEatNewLine(ScannerState.VB)
				Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax) = Me.ParseSelectList()
				queryClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.SelectClause(DirectCast(currentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax), separatedSyntaxList)
			Else
				If (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierToken) Then
					keywordSyntax = Nothing
					If (Me.TryTokenAsContextualKeyword(currentToken, keywordSyntax)) Then
						Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = keywordSyntax.Kind
						If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GroupKeyword) Then
							If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DistinctKeyword) Then
								If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregateKeyword) Then
									Me.GetNextToken(ScannerState.VB)
									queryClauseSyntax = Me.ParseAggregateClause(keywordSyntax)
									Return queryClauseSyntax
								Else
									If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DistinctKeyword) Then
										GoTo Label2
									End If
									Me.GetNextToken(ScannerState.VB)
									If (Me.CurrentToken.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StatementTerminatorToken) Then
										syntaxToken = Me.PeekToken(1)
										Dim kind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = syntaxToken.Kind
										If (kind1 <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken) Then
											If (kind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExclamationToken OrElse kind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken) Then
												GoTo Label7
											End If
											GoTo Label6
										ElseIf (kind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DotToken AndAlso kind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QuestionToken) Then
											GoTo Label6
										End If
									Label7:
										Me.TryEatNewLine(ScannerState.VB)
									End If
									queryClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.DistinctClause(keywordSyntax)
									Return queryClauseSyntax
								End If
							ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FromKeyword) Then
								Me.GetNextToken(ScannerState.VB)
								queryClauseSyntax = Me.ParseFromOperator(keywordSyntax)
								Return queryClauseSyntax
							Else
								If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GroupKeyword) Then
									GoTo Label2
								End If
								Me.GetNextToken(ScannerState.VB)
								Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
								If (Not Me.TryGetContextualKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinKeyword, keywordSyntax1, False)) Then
									queryClauseSyntax = Me.ParseGroupByExpression(keywordSyntax)
									Return queryClauseSyntax
								Else
									queryClauseSyntax = Me.ParseInnerJoinOrGroupJoinExpression(keywordSyntax, keywordSyntax1)
									Return queryClauseSyntax
								End If
							End If
						ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrderKeyword) Then
							If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinKeyword) Then
								Me.GetNextToken(ScannerState.VB)
								queryClauseSyntax = Me.ParseInnerJoinOrGroupJoinExpression(Nothing, keywordSyntax)
								Return queryClauseSyntax
							Else
								If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrderKeyword) Then
									GoTo Label2
								End If
								Me.GetNextToken(ScannerState.VB)
								Dim keywordSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
								Me.TryGetContextualKeywordAndEatNewLine(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByKeyword, keywordSyntax2, True)
								Me.TryEatNewLine(ScannerState.VB)
								Dim separatedSyntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax) = Me.ParseOrderByList()
								queryClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.OrderByClause(keywordSyntax, keywordSyntax2, separatedSyntaxList1)
								Return queryClauseSyntax
							End If
						ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkipKeyword) Then
							Me.GetNextToken(ScannerState.VB)
							If (Me.CurrentToken.Kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword) Then
								Me.TryEatNewLineIfNotFollowedBy(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword)
								queryClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.SkipClause(keywordSyntax, Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False))
								Return queryClauseSyntax
							Else
								Dim currentToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
								Me.GetNextToken(ScannerState.VB)
								Me.TryEatNewLine(ScannerState.VB)
								queryClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.SkipWhileClause(keywordSyntax, currentToken1, Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False))
								Return queryClauseSyntax
							End If
						ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TakeKeyword) Then
							Me.GetNextToken(ScannerState.VB)
							If (Me.CurrentToken.Kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword) Then
								Me.TryEatNewLineIfNotFollowedBy(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword)
								queryClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.TakeClause(keywordSyntax, Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False))
								Return queryClauseSyntax
							Else
								Dim currentToken2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
								Me.GetNextToken(ScannerState.VB)
								Me.TryEatNewLine(ScannerState.VB)
								queryClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.TakeWhileClause(keywordSyntax, currentToken2, Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False))
								Return queryClauseSyntax
							End If
						Else
							If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhereKeyword) Then
								GoTo Label2
							End If
							Me.GetNextToken(ScannerState.VB)
							Me.TryEatNewLine(ScannerState.VB)
							queryClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.WhereClause(keywordSyntax, Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False))
							Return queryClauseSyntax
						End If
					Else
						queryClauseSyntax = Nothing
						Return queryClauseSyntax
					End If
				End If
			Label2:
				queryClauseSyntax = Nothing
			End If
			Return queryClauseSyntax
		Label1:
			Me.TryEatNewLine(ScannerState.VB)
			currentToken = Me.CurrentToken
			GoTo Label8
		Label6:
			If (syntaxToken.IsBinaryOperator()) Then
				Me.TryEatNewLine(ScannerState.VB)
				queryClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.DistinctClause(keywordSyntax)
				Return queryClauseSyntax
			Else
				queryClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.DistinctClause(keywordSyntax)
				Return queryClauseSyntax
			End If
		End Function

		Private Function ParseNextStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax
			Dim nextStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			If (Not Me.CanFollowStatement(Me.CurrentToken)) Then
				Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)()
				Dim context As BlockContext = Me.Context
				While True
					Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseVariable()
					If (expressionSyntax.ContainsDiagnostics) Then
						expressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax)
					End If
					If (context IsNot Nothing AndAlso context.BlockKind <> SyntaxKind.ForBlock AndAlso context.BlockKind <> SyntaxKind.ForEachBlock) Then
						expressionSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax, ERRID.ERR_ExtraNextVariable)
						context = Nothing
					End If
					separatedSyntaxListBuilder.Add(expressionSyntax)
					Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
					If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax, False, ScannerState.VB)) Then
						Exit While
					End If
					separatedSyntaxListBuilder.AddSeparator(punctuationSyntax)
					If (context IsNot Nothing) Then
						context = context.PrevBlock
					End If
				End While
				Dim nextStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax = Me.SyntaxFactory.NextStatement(currentToken, separatedSyntaxListBuilder.ToList())
				Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(separatedSyntaxListBuilder)
				nextStatementSyntax = nextStatementSyntax1
			Else
				nextStatementSyntax = Me.SyntaxFactory.NextStatement(currentToken, New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode)())
			End If
			Return nextStatementSyntax
		End Function

		Private Function ParseNullableIdentifier(ByRef optionalNullable As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = Me.ParseIdentifier()
			If (675 = CInt(Me.CurrentToken.Kind) AndAlso Not identifierTokenSyntax.ContainsDiagnostics) Then
				optionalNullable = DirectCast(Me.CurrentToken, PunctuationSyntax)
				Me.GetNextToken(ScannerState.VB)
			End If
			Return identifierTokenSyntax
		End Function

		Private Function ParseNullableModifiedIdentifier() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = Me.ParseNullableIdentifier(punctuationSyntax)
			Return Me.SyntaxFactory.ModifiedIdentifier(identifierTokenSyntax, punctuationSyntax, Nothing, New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)())
		End Function

		Private Function ParseObjectCollectionInitializer(ByVal fromKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCollectionInitializerSyntax
			fromKeyword = Me.CheckFeatureAvailability(Of KeywordSyntax)(Feature.CollectionInitializers, fromKeyword)
			If (Me.CurrentToken.Kind = SyntaxKind.StatementTerminatorToken AndAlso Me.PeekToken(1).Kind = SyntaxKind.OpenBraceToken) Then
				Me.TryEatNewLine(ScannerState.VB)
			End If
			Dim collectionInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax = Me.ParseCollectionInitializer()
			Return Me.SyntaxFactory.ObjectCollectionInitializer(fromKeyword, collectionInitializerSyntax)
		End Function

		Private Function ParseObjectInitializerList(Optional ByVal anonymousTypeInitializer As Boolean = False, Optional ByVal anonymousTypesAllowedHere As Boolean = True) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax
			Dim objectMemberInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			If (anonymousTypeInitializer AndAlso Not anonymousTypesAllowedHere) Then
				currentToken = Parser.ReportSyntaxError(Of KeywordSyntax)(currentToken, ERRID.ERR_UnrecognizedTypeKeyword)
			End If
			Me.GetNextToken(ScannerState.VB)
			If (Me.PeekPastStatementTerminator().Kind = SyntaxKind.OpenBraceToken) Then
				Me.TryEatNewLine(ScannerState.VB)
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			If (Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.OpenBraceToken, punctuationSyntax, True, ScannerState.VB)) Then
				Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldInitializerSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldInitializerSyntax)()
				If (Me.CurrentToken.Kind = SyntaxKind.CloseBraceToken OrElse Me.CurrentToken.Kind = SyntaxKind.StatementTerminatorToken OrElse Me.CurrentToken.Kind = SyntaxKind.ColonToken) Then
					punctuationSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(punctuationSyntax, If(anonymousTypeInitializer, ERRID.ERR_AnonymousTypeNeedField, ERRID.ERR_InitializerExpected))
				Else
					Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldInitializerSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldInitializerSyntax)()
					While True
						Dim fieldInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldInitializerSyntax = Me.ParseAssignmentInitializer(anonymousTypeInitializer)
						If (fieldInitializerSyntax.ContainsDiagnostics) Then
							fieldInitializerSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldInitializerSyntax)(fieldInitializerSyntax, New SyntaxKind() { SyntaxKind.CommaToken, SyntaxKind.CloseBraceToken })
						End If
						separatedSyntaxListBuilder.Add(fieldInitializerSyntax)
						Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
						If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax1, False, ScannerState.VB)) Then
							Exit While
						End If
						separatedSyntaxListBuilder.AddSeparator(punctuationSyntax1)
					End While
					list = separatedSyntaxListBuilder.ToList()
					Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldInitializerSyntax)(separatedSyntaxListBuilder)
				End If
				Dim closingRightBrace As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Me.GetClosingRightBrace()
				objectMemberInitializerSyntax = Me.SyntaxFactory.ObjectMemberInitializer(currentToken, punctuationSyntax, list, closingRightBrace)
			Else
				Dim syntaxFactory As ContextAwareSyntaxFactory = Me.SyntaxFactory
				Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode)()
				objectMemberInitializerSyntax = syntaxFactory.ObjectMemberInitializer(currentToken, punctuationSyntax, separatedSyntaxList, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.CloseBraceToken))
			End If
			Return objectMemberInitializerSyntax
		End Function

		Private Function ParseOneImportsDirective() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsClauseSyntax
			Dim xmlAttributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax
			Dim flag As Boolean
			Dim vB As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsClauseSyntax = Nothing
			If (Me.CurrentToken.Kind = SyntaxKind.LessThanToken) Then
				Me.ResetCurrentToken(ScannerState.Element)
				Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				If (Not Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.LessThanToken, punctuationSyntax, ScannerState.Element)) Then
					xmlAttributeSyntax = Me.CreateMissingXmlAttribute()
					Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = Me.ResyncAt(ScannerState.Element, New SyntaxKind() { SyntaxKind.GreaterThanToken })
					xmlAttributeSyntax = xmlAttributeSyntax.AddTrailingSyntax(syntaxList)
				Else
					xmlAttributeSyntax = If(Me.CurrentToken.Kind <> SyntaxKind.XmlNameToken OrElse EmbeddedOperators.CompareString(Me.CurrentToken.ToFullString(), "xmlns", False) <> 0 OrElse punctuationSyntax.HasTrailingTrivia, Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax)(Me.CreateMissingXmlAttribute(), ERRID.ERR_ExpectedXmlns), DirectCast(Me.ParseXmlAttribute(False, False, Nothing), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax))
					Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = Me.ResyncAt(ScannerState.Element, New SyntaxKind() { SyntaxKind.GreaterThanToken })
					If (syntaxList1.Any()) Then
						xmlAttributeSyntax = xmlAttributeSyntax.AddTrailingSyntax(syntaxList1, ERRID.ERR_ExpectedGreater)
					End If
				End If
				Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.GreaterThanToken, punctuationSyntax1, ScannerState.Element)
				vB = Me.SyntaxFactory.XmlNamespaceImportsClause(punctuationSyntax, xmlAttributeSyntax, punctuationSyntax1)
				vB = Parser.AdjustTriviaForMissingTokens(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsClauseSyntax)(vB)
				vB = Me.TransitionFromXmlToVB(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsClauseSyntax)(vB)
			ElseIf ((Me.CurrentToken.Kind <> SyntaxKind.IdentifierToken OrElse Me.PeekToken(1).Kind <> SyntaxKind.EqualsToken) AndAlso Me.CurrentToken.Kind <> SyntaxKind.EqualsToken) Then
				flag = False
				Dim nameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax = Me.ParseName(False, False, True, True, False, False, False, flag, False)
				vB = Me.SyntaxFactory.SimpleImportsClause(Nothing, nameSyntax)
			Else
				Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = Me.ParseIdentifier()
				If (identifierTokenSyntax.TypeCharacter <> TypeCharacter.None) Then
					identifierTokenSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)(identifierTokenSyntax, ERRID.ERR_NoTypecharInAlias)
				End If
				Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
				Me.GetNextToken(ScannerState.VB)
				Me.TryEatNewLine(ScannerState.VB)
				flag = False
				Dim nameSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax = Me.ParseName(False, False, True, True, False, False, False, flag, False)
				vB = Me.SyntaxFactory.SimpleImportsClause(Me.SyntaxFactory.ImportAliasClause(identifierTokenSyntax, currentToken), nameSyntax1)
			End If
			If (vB.ContainsDiagnostics AndAlso Me.CurrentToken.Kind <> SyntaxKind.CommaToken) Then
				vB = vB.AddTrailingSyntax(Me.ResyncAt(New SyntaxKind() { SyntaxKind.CommaToken }))
			End If
			Return vB
		End Function

		Private Function ParseOnErrorGoto(ByVal onKeyword As KeywordSyntax, ByVal errorKeyword As KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax
			Dim labelSyntaxForIdentifierOrLineNumber As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.PeekToken(1)
			If (syntaxToken.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntegerLiteralToken AndAlso EmbeddedOperators.CompareString(syntaxToken.ValueText, "0", False) = 0) Then
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnErrorGoToZeroStatement
				labelSyntaxForIdentifierOrLineNumber = Me.SyntaxFactory.NumericLabel(syntaxToken)
				Me.GetNextToken(ScannerState.VB)
				Me.GetNextToken(ScannerState.VB)
			ElseIf (syntaxToken.Kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MinusToken OrElse Me.PeekToken(2).Kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntegerLiteralToken OrElse EmbeddedOperators.CompareString(Me.PeekToken(2).ValueText, "1", False) <> 0) Then
				Me.GetNextToken(ScannerState.VB)
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnErrorGoToLabelStatement
				labelSyntaxForIdentifierOrLineNumber = Me.GetLabelSyntaxForIdentifierOrLineNumber(Me.ParseLabelReference())
			Else
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnErrorGoToMinusOneStatement
				punctuationSyntax = DirectCast(syntaxToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
				labelSyntaxForIdentifierOrLineNumber = Me.SyntaxFactory.NumericLabel(Me.PeekToken(2))
				Me.GetNextToken(ScannerState.VB)
				Me.GetNextToken(ScannerState.VB)
				Me.GetNextToken(ScannerState.VB)
			End If
			Return Me.SyntaxFactory.OnErrorGoToStatement(syntaxKind, onKeyword, errorKeyword, currentToken, punctuationSyntax, labelSyntaxForIdentifierOrLineNumber)
		End Function

		Private Function ParseOnErrorResumeNext(ByVal onKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal errorKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorResumeNextStatementSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Me.GetNextToken(ScannerState.VB)
			Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(SyntaxKind.NextKeyword, keywordSyntax, ScannerState.VB)
			Return Me.SyntaxFactory.OnErrorResumeNextStatement(onKeyword, errorKeyword, currentToken, keywordSyntax)
		End Function

		Private Function ParseOnErrorStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			If (Me.CurrentToken.Kind <> SyntaxKind.ErrorKeyword) Then
				currentToken = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.ErrorKeyword), ERRID.ERR_ObsoleteOnGotoGosub)
				currentToken = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(currentToken, New SyntaxKind() { SyntaxKind.GoToKeyword, SyntaxKind.ResumeKeyword })
			Else
				currentToken = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
			End If
			If (Me.CurrentToken.Kind = SyntaxKind.ResumeKeyword) Then
				statement = Me.ParseOnErrorResumeNext(keywordSyntax, currentToken)
			ElseIf (Me.CurrentToken.Kind <> SyntaxKind.GoToKeyword) Then
				Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.GoToKeyword)
				If (Not currentToken.ContainsDiagnostics) Then
					keywordSyntax1 = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(keywordSyntax1, ERRID.ERR_ExpectedResumeOrGoto)
				End If
				statement = Me.SyntaxFactory.OnErrorGoToStatement(SyntaxKind.OnErrorGoToLabelStatement, keywordSyntax, currentToken, keywordSyntax1, Nothing, Me.SyntaxFactory.IdentifierLabel(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier()))
			Else
				statement = Me.ParseOnErrorGoto(keywordSyntax, currentToken)
			End If
			Return statement
		End Function

		Private Function ParseOperatorStatement(ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorStatementSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			If (Not Me.TryTokenAsContextualKeyword(Me.CurrentToken, keywordSyntax1)) Then
				currentToken = Me.CurrentToken
			Else
				currentToken = keywordSyntax1
			End If
			Dim kind As SyntaxKind = currentToken.Kind
			If (Not SyntaxFacts.IsOperatorStatementOperatorToken(kind)) Then
				Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.PlusToken)
				If (SyntaxFacts.IsOperator(kind)) Then
					currentToken = syntaxToken.AddTrailingSyntax(currentToken, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_OperatorNotOverloadable)
					Me.GetNextToken(ScannerState.VB)
				ElseIf (kind = SyntaxKind.OpenParenToken OrElse Me.IsValidStatementTerminator(currentToken)) Then
					currentToken = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(syntaxToken, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_UnknownOperator)
				Else
					currentToken = syntaxToken.AddTrailingSyntax(currentToken, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_UnknownOperator)
					Me.GetNextToken(ScannerState.VB)
				End If
			Else
				Me.GetNextToken(ScannerState.VB)
			End If
			Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax = Nothing
			If (Me.TryRejectGenericParametersForMemberDecl(typeParameterListSyntax)) Then
				currentToken = currentToken.AddTrailingSyntax(typeParameterListSyntax)
			End If
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax = Nothing
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax)()
			Dim flag As Boolean = False
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			If (Me.CurrentToken.Kind <> SyntaxKind.OpenParenToken) Then
				flag = True
				currentToken = currentToken.AddTrailingSyntax(Me.ResyncAt(New SyntaxKind() { SyntaxKind.OpenParenToken, SyntaxKind.AsKeyword }))
			End If
			If (Me.CurrentToken.Kind = SyntaxKind.OpenParenToken) Then
				separatedSyntaxList = Me.ParseParameters(punctuationSyntax, punctuationSyntax1)
			End If
			If (flag) Then
				punctuationSyntax = If(punctuationSyntax IsNot Nothing, Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(punctuationSyntax, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedLparen), DirectCast(Parser.HandleUnexpectedToken(SyntaxKind.OpenParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax))
				If (punctuationSyntax1 Is Nothing) Then
					punctuationSyntax1 = DirectCast(Parser.HandleUnexpectedToken(SyntaxKind.CloseParenToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
				End If
			End If
			If (punctuationSyntax IsNot Nothing) Then
				parameterListSyntax = Me.SyntaxFactory.ParameterList(punctuationSyntax, separatedSyntaxList, punctuationSyntax1)
			End If
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = Nothing
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = Nothing
			Dim currentToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			If (Me.CurrentToken.Kind = SyntaxKind.AsKeyword) Then
				currentToken1 = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
				If (Me.CurrentToken.Kind = SyntaxKind.LessThanToken) Then
					syntaxList = Me.ParseAttributeLists(False)
				End If
				typeSyntax = Me.ParseGeneralType(False)
				If (typeSyntax.ContainsDiagnostics) Then
					typeSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)(typeSyntax)
				End If
				simpleAsClauseSyntax = Me.SyntaxFactory.SimpleAsClause(currentToken1, syntaxList, typeSyntax)
			End If
			Dim operatorStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorStatementSyntax = Me.SyntaxFactory.OperatorStatement(attributes, modifiers, keywordSyntax, currentToken, parameterListSyntax, simpleAsClauseSyntax)
			Dim syntaxToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Nothing
			Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None
			If (Me.CurrentToken.Kind = SyntaxKind.HandlesKeyword) Then
				syntaxToken1 = Me.CurrentToken
				Me.GetNextToken(ScannerState.VB)
				eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidHandles
			ElseIf (Me.CurrentToken.Kind = SyntaxKind.ImplementsKeyword) Then
				syntaxToken1 = Me.CurrentToken
				Me.GetNextToken(ScannerState.VB)
				eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidImplements
			End If
			If (syntaxToken1 IsNot Nothing) Then
				operatorStatementSyntax = operatorStatementSyntax.AddTrailingSyntax(syntaxToken1, eRRID)
			End If
			Return operatorStatementSyntax
		End Function

		Private Function ParseOptionalJoinOperator() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinClauseSyntax
			Dim joinClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinClauseSyntax
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			If (Not Me.TryEatNewLineAndGetContextualKeyword(SyntaxKind.JoinKeyword, keywordSyntax, False)) Then
				Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
				If (Not Me.TryEatNewLineAndGetContextualKeyword(SyntaxKind.GroupKeyword, keywordSyntax1, False)) Then
					joinClauseSyntax = Nothing
				Else
					Me.TryGetContextualKeyword(SyntaxKind.JoinKeyword, keywordSyntax, True)
					joinClauseSyntax = Me.ParseInnerJoinOrGroupJoinExpression(keywordSyntax1, keywordSyntax)
				End If
			Else
				joinClauseSyntax = Me.ParseInnerJoinOrGroupJoinExpression(Nothing, keywordSyntax)
			End If
			Return joinClauseSyntax
		End Function

		Private Function ParseOptionStatement(ByVal Attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax), ByVal Specifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim optionStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax
			Dim kind As SyntaxKind
			Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Me.ReportModifiersOnStatementError(Attributes, Specifiers, DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax))
			Me.GetNextToken(ScannerState.VB)
			If (Not Me.TryTokenAsContextualKeyword(Me.CurrentToken, keywordSyntax)) Then
				keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.StrictKeyword)
				eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedForOptionStmt
			Else
				kind = keywordSyntax.Kind
				If (kind > SyntaxKind.ExplicitKeyword) Then
					If (kind = SyntaxKind.InferKeyword OrElse kind = SyntaxKind.StrictKeyword) Then
						GoTo Label3
					End If
					If (kind = SyntaxKind.TextKeyword) Then
						keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.CompareKeyword)
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedOptionCompare
						optionStatementSyntax = Me.SyntaxFactory.OptionStatement(keywordSyntax1, keywordSyntax, currentToken)
						If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
							optionStatementSyntax = optionStatementSyntax.AddTrailingSyntax(Me.ResyncAt(), eRRID)
						End If
						Return optionStatementSyntax
					End If
					keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.StrictKeyword)
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedForOptionStmt
					optionStatementSyntax = Me.SyntaxFactory.OptionStatement(keywordSyntax1, keywordSyntax, currentToken)
					If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
						optionStatementSyntax = optionStatementSyntax.AddTrailingSyntax(Me.ResyncAt(), eRRID)
					End If
					Return optionStatementSyntax
				Else
					If (kind = SyntaxKind.BinaryKeyword) Then
						keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.CompareKeyword)
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedOptionCompare
						optionStatementSyntax = Me.SyntaxFactory.OptionStatement(keywordSyntax1, keywordSyntax, currentToken)
						If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
							optionStatementSyntax = optionStatementSyntax.AddTrailingSyntax(Me.ResyncAt(), eRRID)
						End If
						Return optionStatementSyntax
					End If
					If (kind <> SyntaxKind.CompareKeyword) Then
						GoTo Label4
					End If
					Me.GetNextToken(ScannerState.VB)
					If (Not Me.TryTokenAsContextualKeyword(Me.CurrentToken, currentToken)) Then
						currentToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.BinaryKeyword)
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidOptionCompare
						optionStatementSyntax = Me.SyntaxFactory.OptionStatement(keywordSyntax1, keywordSyntax, currentToken)
						If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
							optionStatementSyntax = optionStatementSyntax.AddTrailingSyntax(Me.ResyncAt(), eRRID)
						End If
						Return optionStatementSyntax
					ElseIf (currentToken.Kind = SyntaxKind.TextKeyword) Then
						Me.GetNextToken(ScannerState.VB)
						optionStatementSyntax = Me.SyntaxFactory.OptionStatement(keywordSyntax1, keywordSyntax, currentToken)
						If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
							optionStatementSyntax = optionStatementSyntax.AddTrailingSyntax(Me.ResyncAt(), eRRID)
						End If
						Return optionStatementSyntax
					ElseIf (currentToken.Kind <> SyntaxKind.BinaryKeyword) Then
						currentToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.BinaryKeyword)
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidOptionCompare
						optionStatementSyntax = Me.SyntaxFactory.OptionStatement(keywordSyntax1, keywordSyntax, currentToken)
						If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
							optionStatementSyntax = optionStatementSyntax.AddTrailingSyntax(Me.ResyncAt(), eRRID)
						End If
						Return optionStatementSyntax
					Else
						Me.GetNextToken(ScannerState.VB)
						optionStatementSyntax = Me.SyntaxFactory.OptionStatement(keywordSyntax1, keywordSyntax, currentToken)
						If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
							optionStatementSyntax = optionStatementSyntax.AddTrailingSyntax(Me.ResyncAt(), eRRID)
						End If
						Return optionStatementSyntax
					End If
				End If
			Label3:
				Me.GetNextToken(ScannerState.VB)
				If (Me.CurrentToken.Kind = SyntaxKind.OnKeyword) Then
					currentToken = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
					Me.GetNextToken(ScannerState.VB)
				ElseIf (Me.TryTokenAsContextualKeyword(Me.CurrentToken, currentToken) AndAlso currentToken.Kind = SyntaxKind.OffKeyword) Then
					Me.GetNextToken(ScannerState.VB)
				ElseIf (Not Me.IsValidStatementTerminator(Me.CurrentToken)) Then
					If (keywordSyntax.Kind <> SyntaxKind.StrictKeyword) Then
						eRRID = If(keywordSyntax.Kind <> SyntaxKind.ExplicitKeyword, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidOptionInfer, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidOptionExplicit)
					Else
						eRRID = If(currentToken Is Nothing OrElse currentToken.Kind <> SyntaxKind.CustomKeyword, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidOptionStrict, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidOptionStrictCustom)
					End If
					currentToken = Nothing
				End If
			End If
			optionStatementSyntax = Me.SyntaxFactory.OptionStatement(keywordSyntax1, keywordSyntax, currentToken)
			If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
				optionStatementSyntax = optionStatementSyntax.AddTrailingSyntax(Me.ResyncAt(), eRRID)
			End If
			Return optionStatementSyntax
		Label4:
			If (kind = SyntaxKind.ExplicitKeyword) Then
				GoTo Label3
			End If
			keywordSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.StrictKeyword)
			eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedForOptionStmt
			optionStatementSyntax = Me.SyntaxFactory.OptionStatement(keywordSyntax1, keywordSyntax, currentToken)
			If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
				optionStatementSyntax = optionStatementSyntax.AddTrailingSyntax(Me.ResyncAt(), eRRID)
			End If
			Return optionStatementSyntax
		End Function

		Private Function ParseOrderByList() As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax)
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax)()
			While True
				Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
				If (expressionSyntax.ContainsDiagnostics) Then
					expressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax, New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("5CD5515250FFF8C85B2B05BEE91D70651B5E7B35BD61DEC5ECD69A59B753F272").FieldHandle })
				End If
				Dim orderingSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax = Nothing
				Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
				If (Not Me.TryEatNewLineAndGetContextualKeyword(SyntaxKind.DescendingKeyword, keywordSyntax, False)) Then
					Me.TryEatNewLineAndGetContextualKeyword(SyntaxKind.AscendingKeyword, keywordSyntax, False)
					orderingSyntax = Me.SyntaxFactory.AscendingOrdering(expressionSyntax, keywordSyntax)
				Else
					orderingSyntax = Me.SyntaxFactory.DescendingOrdering(expressionSyntax, keywordSyntax)
				End If
				separatedSyntaxListBuilder.Add(orderingSyntax)
				Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				If (Not Me.TryEatNewLineAndGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax, False, ScannerState.VB)) Then
					Exit While
				End If
				Me.TryEatNewLine(ScannerState.VB)
				separatedSyntaxListBuilder.AddSeparator(punctuationSyntax)
			End While
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax) = separatedSyntaxListBuilder.ToList()
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax)(separatedSyntaxListBuilder)
			Return list
		End Function

		Private Function ParseParameter(ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax
			Dim modifiedIdentifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax = Me.ParseModifiedIdentifier(False, False)
			If (modifiedIdentifierSyntax.ContainsDiagnostics AndAlso Me.PeekAheadFor(New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("2B97283034DB4ADF165678CD3D2AC2F3869D8E1E52C8DE1AC8EA4077F956A4F9").FieldHandle }) = SyntaxKind.AsKeyword) Then
				modifiedIdentifierSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax)(modifiedIdentifierSyntax, New SyntaxKind() { SyntaxKind.AsKeyword })
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = Nothing
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			If (Me.TryGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(SyntaxKind.AsKeyword, keywordSyntax)) Then
				Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = Me.ParseGeneralType(False)
				simpleAsClauseSyntax = Me.SyntaxFactory.SimpleAsClause(keywordSyntax, New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)(), typeSyntax)
				If (simpleAsClauseSyntax.ContainsDiagnostics) Then
					simpleAsClauseSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)(simpleAsClauseSyntax, New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("693C8DEB6B3535DD638089176E0159015A0BA2CE2B40083388240B8869C63AC3").FieldHandle })
				End If
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Nothing
			If (Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.EqualsToken, punctuationSyntax, False, ScannerState.VB)) Then
				If (Not modifiers.Any() OrElse Not modifiers.Any(523)) Then
					punctuationSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(punctuationSyntax, ERRID.ERR_DefaultValueForNonOptionalParam)
				End If
				expressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
			ElseIf (modifiers.Any() AndAlso modifiers.Any(523)) Then
				punctuationSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.EqualsToken), ERRID.ERR_ObsoleteOptionalWithoutValue)
				expressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
			End If
			Dim equalsValueSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax = Nothing
			If (expressionSyntax IsNot Nothing) Then
				If (expressionSyntax.ContainsDiagnostics) Then
					expressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax, New SyntaxKind() { SyntaxKind.CommaToken, SyntaxKind.CloseParenToken })
				End If
				equalsValueSyntax = Me.SyntaxFactory.EqualsValue(punctuationSyntax, expressionSyntax)
			End If
			Return Me.SyntaxFactory.Parameter(attributes, modifiers, modifiedIdentifierSyntax, simpleAsClauseSyntax, equalsValueSyntax)
		End Function

		Friend Function ParseParameterList() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax
			If (Me.CurrentToken.Kind <> SyntaxKind.OpenParenToken) Then
				parameterListSyntax = Nothing
			Else
				Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax) = Me.ParseParameters(punctuationSyntax, punctuationSyntax1)
				parameterListSyntax = Me.SyntaxFactory.ParameterList(punctuationSyntax, separatedSyntaxList, punctuationSyntax1)
			End If
			Return parameterListSyntax
		End Function

		Private Function ParseParameters(ByRef openParen As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, ByRef closeParen As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax) As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax)
			Dim parameterSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax)
			Dim flag As Boolean
			Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.OpenParenToken, openParen, False, ScannerState.VB)
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax)()
			If (Me.CurrentToken.Kind <> SyntaxKind.CloseParenToken) Then
				While True
					Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
					If (Me.CurrentToken.Kind = SyntaxKind.LessThanToken) Then
						syntaxList = Me.ParseAttributeLists(False)
					End If
					Dim parameterSpecifier As ParameterSpecifiers = 0
					Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax) = Me.ParseParameterSpecifiers(parameterSpecifier)
					parameterSyntax = Me.ParseParameter(syntaxList, syntaxList1)
					If (parameterSyntax.ContainsDiagnostics) Then
						parameterSyntax = parameterSyntax.AddTrailingSyntax(Me.ResyncAt(New SyntaxKind() { SyntaxKind.CommaToken, SyntaxKind.CloseParenToken }))
					End If
					Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
					If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax, False, ScannerState.VB)) Then
						If (Me.CurrentToken.Kind = SyntaxKind.CloseParenToken OrElse Me.MustEndStatement(Me.CurrentToken)) Then
							Exit While
						End If
						If (Me.IsContinuableEOL(0) AndAlso Me.PeekToken(1).Kind = SyntaxKind.CloseParenToken) Then
							separatedSyntaxListBuilder.Add(parameterSyntax)
							flag = Me.TryEatNewLineAndGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CloseParenToken, closeParen, True, ScannerState.VB)
							list = separatedSyntaxListBuilder.ToList()
							Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax)(separatedSyntaxListBuilder)
							Return list
						End If
						parameterSyntax = parameterSyntax.AddTrailingSyntax(Me.ResyncAt(New SyntaxKind() { SyntaxKind.CommaToken, SyntaxKind.CloseParenToken }), ERRID.ERR_InvalidParameterSyntax)
						If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax, False, ScannerState.VB)) Then
							separatedSyntaxListBuilder.Add(parameterSyntax)
							flag = Me.TryEatNewLineAndGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CloseParenToken, closeParen, True, ScannerState.VB)
							list = separatedSyntaxListBuilder.ToList()
							Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax)(separatedSyntaxListBuilder)
							Return list
						End If
					End If
					separatedSyntaxListBuilder.Add(parameterSyntax)
					separatedSyntaxListBuilder.AddSeparator(punctuationSyntax)
				End While
				separatedSyntaxListBuilder.Add(parameterSyntax)
			End If
			flag = Me.TryEatNewLineAndGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CloseParenToken, closeParen, True, ScannerState.VB)
			list = separatedSyntaxListBuilder.ToList()
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax)(separatedSyntaxListBuilder)
			Return list
		End Function

		Private Function ParseParameterSpecifiers(ByRef specifiers As ParameterSpecifiers) As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)
			' 
			' Current member / type: Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList`1<Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax> Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser::ParseParameterSpecifiers(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSpecifiers&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax> ParseParameterSpecifiers(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSpecifiers&)
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

		Friend Function ParseParenthesizedArguments(Optional ByVal RedimOrNewParent As Boolean = False, Optional ByVal attributeListParent As Boolean = False) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax)()
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.OpenParenToken, punctuationSyntax, False, ScannerState.VB)
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = Nothing
			separatedSyntaxList = Me.ParseArguments(greenNode, RedimOrNewParent, attributeListParent)
			If (Not Me.TryEatNewLineAndGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CloseParenToken, currentToken, False, ScannerState.VB)) Then
				If (Me.PeekAheadFor(New SyntaxKind() { SyntaxKind.OpenParenToken, SyntaxKind.CloseParenToken }) <> SyntaxKind.CloseParenToken) Then
					currentToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.CloseParenToken)
					currentToken = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(currentToken, ERRID.ERR_ExpectedRparen)
				Else
					Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = Me.ResyncAt(New SyntaxKind() { SyntaxKind.CloseParenToken })
					currentToken = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
					currentToken = currentToken.AddLeadingSyntax(syntaxList)
					Me.GetNextToken(ScannerState.VB)
				End If
			End If
			If (greenNode IsNot Nothing) Then
				currentToken = currentToken.AddLeadingSyntax(greenNode)
			End If
			Return Me.SyntaxFactory.ArgumentList(punctuationSyntax, separatedSyntaxList, currentToken)
		End Function

		Private Function ParseParenthesizedExpressionOrTupleLiteral() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.OpenParenToken, punctuationSyntax, False, ScannerState.VB)
			If (Me.CurrentToken.Kind <> SyntaxKind.IdentifierToken OrElse Me.PeekToken(1).Kind <> SyntaxKind.ColonEqualsToken) Then
				Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
				If (Me.CurrentToken.Kind <> SyntaxKind.CommaToken) Then
					Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
					Me.TryEatNewLineAndGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CloseParenToken, punctuationSyntax1, True, ScannerState.VB)
					expressionSyntax = Me.SyntaxFactory.ParenthesizedExpression(punctuationSyntax, expressionSyntax1, punctuationSyntax1)
				Else
					Dim simpleArgumentSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax = Me.SyntaxFactory.SimpleArgument(Nothing, expressionSyntax1)
					expressionSyntax = Me.ParseTheRestOfTupleLiteral(punctuationSyntax, simpleArgumentSyntax)
				End If
			Else
				Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax = Me.ParseIdentifierNameAllowingKeyword()
				Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.ColonEqualsToken, punctuationSyntax2, False, ScannerState.VB)
				Dim nameColonEqualsSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameColonEqualsSyntax = Me.SyntaxFactory.NameColonEquals(identifierNameSyntax, punctuationSyntax2)
				Dim simpleArgumentSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax = Me.SyntaxFactory.SimpleArgument(nameColonEqualsSyntax, Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False))
				expressionSyntax = Me.ParseTheRestOfTupleLiteral(punctuationSyntax, simpleArgumentSyntax1)
			End If
			Return expressionSyntax
		End Function

		Private Function ParseParenthesizedQualifier(ByVal Term As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, Optional ByVal RedimOrNewParent As Boolean = False) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax = Me.ParseParenthesizedArguments(RedimOrNewParent, False)
			Return Me.SyntaxFactory.InvocationExpression(Term, argumentListSyntax)
		End Function

		Private Function ParsePossibleDeclarationStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim kind As SyntaxKind = Me.PeekToken(1).Kind
			If (SyntaxFacts.CanStartSpecifierDeclaration(kind) OrElse SyntaxFacts.IsSpecifier(kind)) Then
				Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.CurrentToken
				Me.GetNextToken(ScannerState.VB)
				statementSyntax = Me.ParseSpecifierDeclaration().AddLeadingSyntax(currentToken, ERRID.ERR_ExpectedDeclaration)
			Else
				statementSyntax = Nothing
			End If
			Return statementSyntax
		End Function

		Private Function ParsePostFixExpression(ByVal RedimOrNewParent As Boolean, ByVal term As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			While True
				Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.CurrentToken
				Dim flag As Boolean = If(term Is Nothing, False, term.Kind = SyntaxKind.SingleLineSubLambdaExpression)
				If (currentToken.Kind = SyntaxKind.DotToken) Then
					If (flag) Then
						term = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(term, ERRID.ERR_SubRequiresParenthesesDot)
					End If
					term = Me.ParseQualifiedExpr(term)
				ElseIf (currentToken.Kind = SyntaxKind.ExclamationToken) Then
					If (flag) Then
						term = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(term, ERRID.ERR_SubRequiresParenthesesBang)
					End If
					term = Me.ParseQualifiedExpr(term)
				ElseIf (currentToken.Kind <> SyntaxKind.OpenParenToken) Then
					If (currentToken.Kind <> SyntaxKind.QuestionToken OrElse Not Me.CanStartConsequenceExpression(Me.PeekToken(1).Kind, True)) Then
						Exit While
					End If
					Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(currentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
					punctuationSyntax = Me.CheckFeatureAvailability(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(Feature.NullPropagatingOperator, punctuationSyntax)
					Me.GetNextToken(ScannerState.VB)
					If (flag) Then
						Dim kind As SyntaxKind = Me.CurrentToken.Kind
						If (kind = SyntaxKind.ExclamationToken) Then
							term = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(term, ERRID.ERR_SubRequiresParenthesesBang)
						ElseIf (kind = SyntaxKind.OpenParenToken) Then
							term = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(term, ERRID.ERR_SubRequiresParenthesesLParen)
						Else
							If (kind <> SyntaxKind.DotToken) Then
								Throw ExceptionUtilities.Unreachable
							End If
							term = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(term, ERRID.ERR_SubRequiresParenthesesDot)
						End If
					End If
					term = Me.SyntaxFactory.ConditionalAccessExpression(term, punctuationSyntax, Me.ParsePostFixExpression(RedimOrNewParent, Nothing))
				Else
					If (flag) Then
						term = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(term, ERRID.ERR_SubRequiresParenthesesLParen)
					End If
					term = Me.ParseParenthesizedQualifier(term, RedimOrNewParent)
				End If
			End While
			Return term
		End Function

		Private Function ParsePotentialQuery(ByVal contextualKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim flag As Boolean = False
			Dim num As Integer = 1
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.PeekToken(num)
			If (syntaxToken IsNot Nothing AndAlso syntaxToken.IsEndOfLine AndAlso Not Me.NextLineStartsWithStatementTerminator(1)) Then
				num = num + 1
				syntaxToken = Me.PeekToken(num)
				flag = True
			End If
			If (syntaxToken Is Nothing OrElse syntaxToken.Kind <> SyntaxKind.IdentifierToken AndAlso Not syntaxToken.IsKeyword) Then
				expressionSyntax = Nothing
			Else
				Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
				If (flag OrElse syntaxToken.IsKeyword OrElse syntaxToken.Kind = SyntaxKind.IdentifierToken AndAlso Me.TryTokenAsContextualKeyword(syntaxToken, keywordSyntax)) Then
					num = num + 1
					syntaxToken = Me.PeekToken(num)
					If (syntaxToken IsNot Nothing) Then
						If (syntaxToken.Kind = SyntaxKind.StatementTerminatorToken) Then
							syntaxToken = Me.PeekToken(num + 1)
							If (syntaxToken IsNot Nothing AndAlso syntaxToken.Kind = SyntaxKind.InKeyword) Then
								GoTo Label1
							End If
							expressionSyntax = Nothing
							Return expressionSyntax
						ElseIf (syntaxToken.Kind = SyntaxKind.QuestionToken) Then
							num = num + 1
							syntaxToken = Me.PeekToken(num)
						End If
					End If
				Label1:
					If (syntaxToken IsNot Nothing) Then
						If (syntaxToken.Kind = SyntaxKind.InKeyword OrElse syntaxToken.Kind = SyntaxKind.AsKeyword OrElse Not flag AndAlso syntaxToken.Kind = SyntaxKind.EqualsToken) Then
							GoTo Label2
						End If
						expressionSyntax = Nothing
						Return expressionSyntax
					Else
						expressionSyntax = Nothing
						Return expressionSyntax
					End If
				End If
			Label2:
				If (contextualKeyword.Kind <> SyntaxKind.FromKeyword) Then
					Me.GetNextToken(ScannerState.VB)
					expressionSyntax = Me.ParseAggregateQueryExpression(contextualKeyword)
				Else
					Me.GetNextToken(ScannerState.VB)
					expressionSyntax = Me.ParseFromQueryExpression(contextualKeyword)
				End If
			End If
			Return expressionSyntax
		End Function

		Private Function ParsePrintStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PrintStatementSyntax
			Dim currentToken As PunctuationSyntax = DirectCast(Me.CurrentToken, PunctuationSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
			Dim printStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PrintStatementSyntax = Me.SyntaxFactory.PrintStatement(currentToken, expressionSyntax)
			If (Me.PeekToken(1).Kind <> SyntaxKind.EndOfFileToken OrElse Me._scanner.Options.Kind = SourceCodeKind.Regular) Then
				printStatementSyntax = printStatementSyntax.AddError(ERRID.ERR_UnexpectedExpressionStatement)
			End If
			Return printStatementSyntax
		End Function

		Private Function ParseProcDeclareStatement(ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Dim keywordSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			If (Me.TryTokenAsContextualKeyword(Me.CurrentToken, keywordSyntax1)) Then
				Dim kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = keywordSyntax1.Kind
				If (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnsiKeyword OrElse kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AutoKeyword OrElse kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UnicodeKeyword) Then
					keywordSyntax2 = keywordSyntax1
					Me.GetNextToken(ScannerState.VB)
				End If
			End If
			If (Me.CurrentToken.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword) Then
				currentToken = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement
			ElseIf (Me.CurrentToken.Kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionKeyword) Then
				currentToken = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword), ERRID.ERR_ExpectedSubFunction)
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement
			Else
				currentToken = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement
			End If
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = Me.ParseIdentifier()
			If (identifierTokenSyntax.ContainsDiagnostics) Then
				identifierTokenSyntax = identifierTokenSyntax.AddTrailingSyntax(Me.ResyncAt(New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LibKeyword, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken }))
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)()
			Dim flag As Boolean = False
			If (Me.CurrentToken.Kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LibKeyword) Then
				If (Me.PeekAheadFor(New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LibKeyword }) <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LibKeyword) Then
					syntaxList = Me.ResyncAt(New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AliasKeyword, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken })
					flag = True
				Else
					syntaxList = Me.ResyncAt(New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LibKeyword })
				End If
			End If
			Dim keywordSyntax3 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Dim literalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax = Nothing
			Dim keywordSyntax4 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Dim literalExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax = Nothing
			Me.ParseDeclareLibClause(keywordSyntax3, literalExpressionSyntax, keywordSyntax4, literalExpressionSyntax1)
			If (syntaxList.Node IsNot Nothing) Then
				keywordSyntax3 = If(Not flag, keywordSyntax3.AddLeadingSyntax(syntaxList, ERRID.ERR_MissingLibInDeclare), keywordSyntax3.AddLeadingSyntax(syntaxList))
			End If
			Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax = Nothing
			If (Me.TryRejectGenericParametersForMemberDecl(typeParameterListSyntax)) Then
				If (literalExpressionSyntax1 Is Nothing) Then
					literalExpressionSyntax = literalExpressionSyntax.AddTrailingSyntax(typeParameterListSyntax)
				Else
					literalExpressionSyntax1 = literalExpressionSyntax1.AddTrailingSyntax(typeParameterListSyntax)
				End If
			End If
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax = Nothing
			parameterListSyntax = Me.ParseParameterList()
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = Nothing
			If (currentToken.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionKeyword AndAlso Me.CurrentToken.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsKeyword) Then
				Dim currentToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
				Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
				If (Me.CurrentToken.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanToken) Then
					syntaxList1 = Me.ParseAttributeLists(False)
				End If
				Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = Me.ParseGeneralType(False)
				If (typeSyntax.ContainsDiagnostics) Then
					typeSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)(typeSyntax)
				End If
				simpleAsClauseSyntax = Me.SyntaxFactory.SimpleAsClause(currentToken1, syntaxList1, typeSyntax)
			End If
			Return Me.SyntaxFactory.DeclareStatement(syntaxKind, attributes, modifiers, keywordSyntax, keywordSyntax2, currentToken, identifierTokenSyntax, keywordSyntax3, literalExpressionSyntax, keywordSyntax4, literalExpressionSyntax1, parameterListSyntax, simpleAsClauseSyntax)
		End Function

		Private Function ParsePropertyDefinition(ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			If (Me.CurrentToken.Kind = SyntaxKind.GetKeyword OrElse Me.CurrentToken.Kind = SyntaxKind.SetKeyword OrElse Me.CurrentToken.Kind = SyntaxKind.LetKeyword) Then
				identifierTokenSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)(Me.ParseIdentifierAllowingKeyword(), ERRID.ERR_ObsoletePropertyGetLetSet)
				If (Me.CurrentToken.Kind = SyntaxKind.IdentifierToken) Then
					identifierTokenSyntax = identifierTokenSyntax.AddTrailingSyntax(Me.ParseIdentifier())
				End If
			Else
				identifierTokenSyntax = Me.ParseIdentifier()
			End If
			Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax = Nothing
			If (Me.TryRejectGenericParametersForMemberDecl(typeParameterListSyntax)) Then
				identifierTokenSyntax = identifierTokenSyntax.AddTrailingSyntax(typeParameterListSyntax)
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax)()
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax = Nothing
			If (Me.CurrentToken.Kind = SyntaxKind.OpenParenToken) Then
				separatedSyntaxList = Me.ParseParameters(punctuationSyntax, punctuationSyntax1)
				parameterListSyntax = Me.SyntaxFactory.ParameterList(punctuationSyntax, separatedSyntaxList, punctuationSyntax1)
			ElseIf (identifierTokenSyntax.ContainsDiagnostics) Then
				identifierTokenSyntax = identifierTokenSyntax.AddTrailingSyntax(Me.ResyncAt(New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("CFF090327DA163B5088503D95322C8E7B8AAA9303557AEA570CAAD2CEE9CD997").FieldHandle }))
			End If
			Dim asClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax = Nothing
			Dim equalsValueSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax = Nothing
			Me.ParseFieldOrPropertyAsClauseAndInitializer(True, False, asClauseSyntax, equalsValueSyntax)
			Dim implementsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax = Nothing
			If (Me.CurrentToken.Kind = SyntaxKind.ImplementsKeyword) Then
				implementsClauseSyntax = Me.ParseImplementsList()
			End If
			Dim propertyStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax = Me.SyntaxFactory.PropertyStatement(attributes, modifiers, currentToken, identifierTokenSyntax, parameterListSyntax, asClauseSyntax, equalsValueSyntax, implementsClauseSyntax)
			If (Me.CurrentToken.Kind <> SyntaxKind.EndOfFileToken) Then
				Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.PeekToken(1)
				If (syntaxToken.Kind <> SyntaxKind.GetKeyword AndAlso syntaxToken.Kind <> SyntaxKind.SetKeyword AndAlso Me.Context.BlockKind <> SyntaxKind.InterfaceBlock AndAlso Not propertyStatementSyntax.Modifiers.Any(505)) Then
					Dim propertyStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax = propertyStatementSyntax
					propertyStatementSyntax = Me.CheckFeatureAvailability(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax)(Feature.AutoProperties, propertyStatementSyntax)
					If (propertyStatementSyntax = propertyStatementSyntax1 AndAlso propertyStatementSyntax.Modifiers.Any(538)) Then
						propertyStatementSyntax = Me.CheckFeatureAvailability(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax)(Feature.ReadonlyAutoProperties, propertyStatementSyntax)
					End If
				End If
			End If
			Return propertyStatementSyntax
		End Function

		Private Function ParsePropertyOrEventAccessor(ByVal accessorKind As SyntaxKind, ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			If (Not Me.IsFirstStatementOnLine(Me.CurrentToken)) Then
				currentToken = Parser.ReportSyntaxError(Of KeywordSyntax)(currentToken, ERRID.ERR_MethodMustBeFirstStatementOnLine)
			End If
			Me.GetNextToken(ScannerState.VB)
			Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax = Nothing
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax = Nothing
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax)()
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.TryRejectGenericParametersForMemberDecl(typeParameterListSyntax)
			If (typeParameterListSyntax IsNot Nothing) Then
				currentToken = currentToken.AddTrailingSyntax(typeParameterListSyntax)
			End If
			If (currentToken.Kind <> SyntaxKind.GetKeyword AndAlso Me.CurrentToken.Kind = SyntaxKind.OpenParenToken) Then
				separatedSyntaxList = Me.ParseParameters(punctuationSyntax, punctuationSyntax1)
				parameterListSyntax = Me.SyntaxFactory.ParameterList(punctuationSyntax, separatedSyntaxList, punctuationSyntax1)
			End If
			If (modifiers.Any() AndAlso (currentToken.Kind = SyntaxKind.AddHandlerKeyword OrElse currentToken.Kind = SyntaxKind.RemoveHandlerKeyword OrElse currentToken.Kind = SyntaxKind.RaiseEventKeyword)) Then
				Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
				currentToken = Me.ReportModifiersOnStatementError(ERRID.ERR_SpecifiersInvOnEventMethod, syntaxList, modifiers, currentToken)
				modifiers = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)()
			End If
			Return Me.SyntaxFactory.AccessorStatement(accessorKind, attributes, modifiers, currentToken, parameterListSyntax)
		End Function

		Private Function ParseQualifiedExpr(ByVal Term As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
			Dim xmlBracketedNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			Dim prevToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.PrevToken
			Me.GetNextToken(ScannerState.VB)
			If (currentToken.Kind <> SyntaxKind.ExclamationToken) Then
				If (Me.CurrentToken.IsEndOfLine AndAlso Not Me.CurrentToken.IsEndOfParse) Then
					If (prevToken IsNot Nothing AndAlso prevToken.Kind <> SyntaxKind.StatementTerminatorToken) Then
						GoTo Label1
					End If
					Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier(), ERRID.ERR_ExpectedIdentifier)
					expressionSyntax = Me.SyntaxFactory.SimpleMemberAccessExpression(Term, currentToken, Me.SyntaxFactory.IdentifierName(identifierTokenSyntax))
					Return expressionSyntax
				End If
			Label2:
				Dim kind As SyntaxKind = Me.CurrentToken.Kind
				If (kind = SyntaxKind.AtToken) Then
					Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
					If (punctuationSyntax.HasTrailingTrivia) Then
						Me.GetNextToken(ScannerState.VB)
						punctuationSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(punctuationSyntax, ERRID.ERR_ExpectedXmlName)
						punctuationSyntax = punctuationSyntax.AddTrailingSyntax(Me.ResyncAt())
						xmlNodeSyntax = Me.SyntaxFactory.XmlName(Nothing, DirectCast(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.XmlNameToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax))
					ElseIf (Me.PeekNextToken(ScannerState.VB).Kind <> SyntaxKind.LessThanToken) Then
						Me.GetNextToken(ScannerState.VB)
						xmlNodeSyntax = Me.ParseXmlQualifiedNameVB()
						If (xmlNodeSyntax.HasLeadingTrivia) Then
							punctuationSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(punctuationSyntax, ERRID.ERR_ExpectedXmlName)
							punctuationSyntax.AddTrailingSyntax(xmlNodeSyntax)
							punctuationSyntax.AddTrailingSyntax(Me.ResyncAt())
							xmlNodeSyntax = Me.SyntaxFactory.XmlName(Nothing, DirectCast(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.XmlNameToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax))
						End If
					Else
						Me.GetNextToken(ScannerState.Element)
						xmlNodeSyntax = Me.ParseBracketedXmlQualifiedName()
					End If
					expressionSyntax = Me.SyntaxFactory.XmlMemberAccessExpression(SyntaxKind.XmlAttributeAccessExpression, Term, currentToken, punctuationSyntax, Nothing, xmlNodeSyntax)
				ElseIf (kind <> SyntaxKind.DotToken) Then
					If (kind = SyntaxKind.LessThanToken) Then
						Dim xmlBracketedNameSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax = Me.ParseBracketedXmlQualifiedName()
						expressionSyntax = Me.SyntaxFactory.XmlMemberAccessExpression(SyntaxKind.XmlElementAccessExpression, Term, currentToken, Nothing, Nothing, xmlBracketedNameSyntax1)
					Else
						Dim simpleNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax = Me.ParseSimpleNameExpressionAllowingKeywordAndTypeArguments()
						expressionSyntax = Me.SyntaxFactory.SimpleMemberAccessExpression(Term, currentToken, simpleNameSyntax)
					End If
				ElseIf (Me.PeekToken(1).Kind <> SyntaxKind.DotToken) Then
					If (Me.CurrentToken.Kind <> SyntaxKind.AtToken) Then
						expressionSyntax1 = Me.SyntaxFactory.SimpleMemberAccessExpression(Term, currentToken, Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.IdentifierName(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier()), ERRID.ERR_ExpectedIdentifier))
					Else
						Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = DirectCast(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.XmlNameToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)
						expressionSyntax1 = Me.SyntaxFactory.XmlMemberAccessExpression(SyntaxKind.XmlAttributeAccessExpression, Term, currentToken, Nothing, Nothing, Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax)(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlName(Nothing, xmlNameTokenSyntax), ERRID.ERR_ExpectedXmlName))
					End If
					expressionSyntax = expressionSyntax1
				Else
					Dim currentToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
					Me.GetNextToken(ScannerState.VB)
					Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
					Me.TryGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.DotToken, punctuationSyntax1)
					Me.TryEatNewLineIfFollowedBy(SyntaxKind.LessThanToken)
					xmlBracketedNameSyntax = If(Me.CurrentToken.Kind <> SyntaxKind.LessThanToken, Me.ReportExpectedXmlBracketedName(), Me.ParseBracketedXmlQualifiedName())
					expressionSyntax = Me.SyntaxFactory.XmlMemberAccessExpression(SyntaxKind.XmlDescendantAccessExpression, Term, currentToken, currentToken1, punctuationSyntax1, xmlBracketedNameSyntax)
				End If
			Else
				Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax = Me.ParseIdentifierNameAllowingKeyword()
				expressionSyntax = Me.SyntaxFactory.DictionaryAccessExpression(Term, currentToken, identifierNameSyntax)
			End If
			Return expressionSyntax
		Label1:
			If (Not Me.NextLineStartsWithStatementTerminator(0)) Then
				Me.TryEatNewLineIfNotFollowedBy(SyntaxKind.DotToken)
				GoTo Label2
			Else
				GoTo Label2
			End If
		End Function

		Private Function ParseRaiseEventStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RaiseEventStatementSyntax
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax = Me.ParseIdentifierNameAllowingKeyword()
			If (identifierNameSyntax.ContainsDiagnostics) Then
				identifierNameSyntax = identifierNameSyntax.AddTrailingSyntax(Me.ResyncAt())
			End If
			Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax = Nothing
			If (Me.CurrentToken.Kind = SyntaxKind.OpenParenToken) Then
				argumentListSyntax = Me.ParseParenthesizedArguments(False, False)
			End If
			Return Me.SyntaxFactory.RaiseEventStatement(currentToken, identifierNameSyntax, argumentListSyntax)
		End Function

		Private Function ParseRedimStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim redimClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RedimClauseSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			If (Me.CurrentToken.Kind = SyntaxKind.IdentifierToken AndAlso Me.TryIdentifierAsContextualKeyword(Me.CurrentToken, keywordSyntax) AndAlso keywordSyntax.Kind = SyntaxKind.PreserveKeyword) Then
				keywordSyntax1 = keywordSyntax
				Me.GetNextToken(ScannerState.VB)
			End If
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RedimClauseSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RedimClauseSyntax)()
			While True
				Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseTerm(False, True)
				If (expressionSyntax.ContainsDiagnostics) Then
					expressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax)
				End If
				If (expressionSyntax.Kind <> SyntaxKind.InvocationExpression) Then
					Dim syntaxFactory As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContextAwareSyntaxFactory = Me.SyntaxFactory
					Dim contextAwareSyntaxFactory As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContextAwareSyntaxFactory = Me.SyntaxFactory
					Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.OpenParenToken)
					Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of GreenNode)()
					redimClauseSyntax = syntaxFactory.RedimClause(expressionSyntax, contextAwareSyntaxFactory.ArgumentList(punctuationSyntax, separatedSyntaxList, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.CloseParenToken)))
				Else
					Dim invocationExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InvocationExpressionSyntax = DirectCast(expressionSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InvocationExpressionSyntax)
					redimClauseSyntax = Me.SyntaxFactory.RedimClause(invocationExpressionSyntax.Expression, invocationExpressionSyntax.ArgumentList)
					Dim diagnostics As DiagnosticInfo() = invocationExpressionSyntax.GetDiagnostics()
					If (diagnostics IsNot Nothing AndAlso CInt(diagnostics.Length) > 0) Then
						redimClauseSyntax = redimClauseSyntax.WithDiagnostics(diagnostics)
					End If
				End If
				separatedSyntaxListBuilder.Add(redimClauseSyntax)
				Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax1, False, ScannerState.VB)) Then
					Exit While
				End If
				separatedSyntaxListBuilder.AddSeparator(punctuationSyntax1)
			End While
			Dim reDimStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax = If(keywordSyntax1 Is Nothing, Me.SyntaxFactory.ReDimStatement(currentToken, keywordSyntax1, separatedSyntaxListBuilder.ToList()), Me.SyntaxFactory.ReDimPreserveStatement(currentToken, keywordSyntax1, separatedSyntaxListBuilder.ToList()))
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RedimClauseSyntax)(separatedSyntaxListBuilder)
			If (Me.CurrentToken.Kind = SyntaxKind.AsKeyword) Then
				reDimStatementSyntax = reDimStatementSyntax.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_ObsoleteRedimAs)
				Me.GetNextToken(ScannerState.VB)
			End If
			Return reDimStatementSyntax
		End Function

		Private Function ParseReferenceDirective(ByVal hashToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax
			Dim currentToken As IdentifierTokenSyntax = DirectCast(Me.CurrentToken, IdentifierTokenSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Me._scanner.MakeKeyword(currentToken)
			If (Not Me.IsScript) Then
				keywordSyntax = keywordSyntax.AddError(ERRID.ERR_ReferenceDirectiveOnlyAllowedInScripts)
			End If
			Dim stringLiteralTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax = Nothing
			Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax)(SyntaxKind.StringLiteralToken, stringLiteralTokenSyntax, ScannerState.VB)
			Return Me.SyntaxFactory.ReferenceDirectiveTrivia(hashToken, keywordSyntax, stringLiteralTokenSyntax)
		End Function

		Private Function ParseRegionDirective(ByVal hashToken As PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax
			Dim currentToken As IdentifierTokenSyntax = DirectCast(Me.CurrentToken, IdentifierTokenSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Me._scanner.MakeKeyword(currentToken)
			Dim stringLiteralTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax = Nothing
			Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax)(SyntaxKind.StringLiteralToken, stringLiteralTokenSyntax, ScannerState.VB)
			Return Me.SyntaxFactory.RegionDirectiveTrivia(hashToken, keywordSyntax, stringLiteralTokenSyntax)
		End Function

		Friend Function ParseRestOfDocCommentContent(ByVal nodesSoFar As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)) As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)
			Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) = Me._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)()
			Dim nodes As GreenNode() = nodesSoFar.Nodes
			Dim num As Integer = 0
			Do
				syntaxListBuilder.Add(DirectCast(nodes(num), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax))
				num = num + 1
			Loop While num < CInt(nodes.Length)
			If (Me.CurrentToken.Kind = SyntaxKind.EndOfXmlToken) Then
				Me.GetNextToken(ScannerState.Content)
				If (Me.CurrentToken.Kind = SyntaxKind.DocumentationCommentLineBreakToken) Then
					Dim xmlNodeSyntaxArray As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax() = Me.ParseXmlContent(ScannerState.Content).Nodes
					For i As Integer = 0 To CInt(xmlNodeSyntaxArray.Length) Step 1
						syntaxListBuilder.Add(xmlNodeSyntaxArray(i))
					Next

				End If
			End If
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) = syntaxListBuilder.ToList()
			Me._pool.Free(syntaxListBuilder)
			Return list
		End Function

		Private Function ParseResumeStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax
			Dim resumeStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Nothing
			Me.GetNextToken(ScannerState.VB)
			If (Me.IsValidStatementTerminator(Me.CurrentToken)) Then
				resumeStatementSyntax = Me.SyntaxFactory.ResumeStatement(currentToken, Nothing)
			ElseIf (Me.CurrentToken.Kind <> SyntaxKind.NextKeyword) Then
				syntaxToken = Me.ParseLabelReference()
				resumeStatementSyntax = Me.SyntaxFactory.ResumeLabelStatement(currentToken, Me.GetLabelSyntaxForIdentifierOrLineNumber(syntaxToken))
			Else
				Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
				resumeStatementSyntax = Me.SyntaxFactory.ResumeNextStatement(currentToken, Me.SyntaxFactory.NextLabel(keywordSyntax))
			End If
			Return resumeStatementSyntax
		End Function

		Private Function ParseReturnStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReturnStatementSyntax
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.CurrentToken
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, True)
			If (expressionSyntax Is Nothing) Then
				If (Not Me.CanFollowStatement(Me.CurrentToken)) Then
					expressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
				End If
			ElseIf (expressionSyntax.ContainsDiagnostics) Then
				expressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax)
			End If
			Return Me.SyntaxFactory.ReturnStatement(currentToken, expressionSyntax)
		End Function

		Private Function ParseSelectList() As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax)
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax)()
			While True
				Dim expressionRangeVariableSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax = Me.ParseSelectListInitializer()
				If (expressionRangeVariableSyntax.ContainsDiagnostics) Then
					expressionRangeVariableSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax)(expressionRangeVariableSyntax, New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("B0BF54917E6AAF810C991346916DFAA00F6C9022B200D38278F2561719FDAC7F").FieldHandle })
				End If
				separatedSyntaxListBuilder.Add(expressionRangeVariableSyntax)
				If (Me.CurrentToken.Kind <> SyntaxKind.CommaToken) Then
					Exit While
				End If
				Dim currentToken As PunctuationSyntax = DirectCast(Me.CurrentToken, PunctuationSyntax)
				Me.GetNextToken(ScannerState.VB)
				Me.TryEatNewLine(ScannerState.VB)
				separatedSyntaxListBuilder.AddSeparator(currentToken)
			End While
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax) = separatedSyntaxListBuilder.ToList()
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax)(separatedSyntaxListBuilder)
			Return list
		End Function

		Private Function ParseSelectListInitializer() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax
			Dim variableNameEqualsSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax = Nothing
			If ((Me.CurrentToken.Kind = SyntaxKind.IdentifierToken OrElse Me.CurrentToken.IsKeyword) AndAlso Me.PeekToken(1).Kind = SyntaxKind.EqualsToken OrElse Me.PeekToken(1).Kind = SyntaxKind.QuestionToken AndAlso Me.PeekToken(2).Kind = SyntaxKind.EqualsToken) Then
				Dim modifiedIdentifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax = Nothing
				Dim currentToken As PunctuationSyntax = Nothing
				modifiedIdentifierSyntax = Me.ParseSimpleIdentifierAsModifiedIdentifier()
				currentToken = DirectCast(Me.CurrentToken, PunctuationSyntax)
				Me.GetNextToken(ScannerState.VB)
				Me.TryEatNewLine(ScannerState.VB)
				variableNameEqualsSyntax = Me.SyntaxFactory.VariableNameEquals(modifiedIdentifierSyntax, Nothing, currentToken)
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
			Return Me.SyntaxFactory.ExpressionRangeVariable(variableNameEqualsSyntax, expressionSyntax)
		End Function

		Private Function ParseSelectStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Me.TryGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(SyntaxKind.CaseKeyword, keywordSyntax)
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
			If (expressionSyntax.ContainsDiagnostics) Then
				expressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax)
			End If
			Return Me.SyntaxFactory.SelectStatement(currentToken, keywordSyntax, expressionSyntax)
		End Function

		Private Function ParseSeparators(ByVal kind As SyntaxKind) As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of PunctuationSyntax)
			Dim syntaxListBuilder As SyntaxListBuilder(Of PunctuationSyntax) = Me._pool.Allocate(Of PunctuationSyntax)()
			While Me.CurrentToken.Kind = kind
				Dim currentToken As PunctuationSyntax = DirectCast(Me.CurrentToken, PunctuationSyntax)
				Me.GetNextToken(ScannerState.VB)
				Me.TryEatNewLine(ScannerState.VB)
				syntaxListBuilder.Add(currentToken)
			End While
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of PunctuationSyntax) = syntaxListBuilder.ToList()
			Me._pool.Free(syntaxListBuilder)
			Return list
		End Function

		Private Function ParseSimpleIdentifierAsModifiedIdentifier() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = Me.ParseIdentifier()
			If (Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
				Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.CurrentToken
				identifierTokenSyntax = identifierTokenSyntax.AddTrailingSyntax(Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(currentToken, ERRID.ERR_NullableTypeInferenceNotSupported))
				Me.GetNextToken(ScannerState.VB)
			End If
			Return Me.SyntaxFactory.ModifiedIdentifier(identifierTokenSyntax, Nothing, Nothing, New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)())
		End Function

		Private Function ParseSimpleName(ByVal allowGenericArguments As Boolean, ByVal allowGenericsWithoutOf As Boolean, ByVal disallowGenericArgumentsOnLastQualifiedName As Boolean, ByVal nonArrayName As Boolean, ByVal allowKeyword As Boolean, ByRef allowEmptyGenericArguments As Boolean, ByRef allowNonEmptyGenericArguments As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax
			Dim simpleNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = If(allowKeyword, Me.ParseIdentifierAllowingKeyword(), Me.ParseIdentifier())
			Dim typeArgumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax = Nothing
			If (allowGenericArguments AndAlso Me.BeginsGeneric(nonArrayName, allowGenericsWithoutOf)) Then
				typeArgumentListSyntax = Me.ParseGenericArguments(allowEmptyGenericArguments, allowNonEmptyGenericArguments)
			End If
			If (typeArgumentListSyntax Is Nothing) Then
				simpleNameSyntax = Me.SyntaxFactory.IdentifierName(identifierTokenSyntax)
			ElseIf (Not disallowGenericArgumentsOnLastQualifiedName OrElse Me.CurrentToken.Kind = SyntaxKind.DotToken OrElse typeArgumentListSyntax.ContainsDiagnostics) Then
				simpleNameSyntax = Me.SyntaxFactory.GenericName(identifierTokenSyntax, typeArgumentListSyntax)
			Else
				identifierTokenSyntax = identifierTokenSyntax.AddTrailingSyntax(typeArgumentListSyntax, ERRID.ERR_TypeArgsUnexpected)
				simpleNameSyntax = Me.SyntaxFactory.IdentifierName(identifierTokenSyntax)
			End If
			Return simpleNameSyntax
		End Function

		Private Function ParseSimpleNameExpressionAllowingKeywordAndTypeArguments() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax
			Dim simpleNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = Me.ParseIdentifierAllowingKeyword()
			If (Me._evaluatingConditionCompilationExpression OrElse Not Me.BeginsGeneric(False, True)) Then
				simpleNameSyntax = Me.SyntaxFactory.IdentifierName(identifierTokenSyntax)
			Else
				Dim flag As Boolean = False
				Dim flag1 As Boolean = True
				Dim typeArgumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax = Me.ParseGenericArguments(flag, flag1)
				simpleNameSyntax = Me.SyntaxFactory.GenericName(identifierTokenSyntax, typeArgumentListSyntax)
			End If
			Return simpleNameSyntax
		End Function

		Private Function ParseSpecifierDeclaration() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
			If (Me.CurrentToken.Kind = SyntaxKind.LessThanToken) Then
				syntaxList = Me.ParseAttributeLists(False)
			End If
			Return Me.ParseSpecifierDeclaration(syntaxList)
		End Function

		Private Function ParseSpecifierDeclaration(ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Return Me.ParseSpecifierDeclaration(attributes, Me.ParseSpecifiers())
		End Function

		Private Function ParseSpecifierDeclaration(ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim statementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax = Nothing
			Dim kind As SyntaxKind = Me.CurrentToken.Kind
			If (kind <= SyntaxKind.ModuleKeyword) Then
				If (kind > SyntaxKind.EnumKeyword) Then
					If (kind <= SyntaxKind.FunctionKeyword) Then
						If (kind = SyntaxKind.EventKeyword) Then
							statementSyntax1 = Me.ParseEventDefinition(attributes, modifiers)
						Else
							If (kind <> SyntaxKind.FunctionKeyword) Then
								GoTo Label2
							End If
							statementSyntax1 = Me.ParseFunctionStatement(attributes, modifiers)
						End If
					ElseIf (kind = SyntaxKind.GetKeyword) Then
						statementSyntax1 = Me.ParsePropertyOrEventAccessor(SyntaxKind.GetAccessorStatement, attributes, modifiers)
					Else
						Select Case kind
							Case SyntaxKind.ImplementsKeyword
							Case SyntaxKind.InheritsKeyword
								statementSyntax1 = Me.ParseInheritsImplementsStatement(attributes, modifiers)
								Exit Select
							Case SyntaxKind.ImportsKeyword
								statementSyntax1 = Me.ParseImportsStatement(attributes, modifiers)
								Exit Select
							Case SyntaxKind.InKeyword
							Case SyntaxKind.IntegerKeyword
								GoTo Label2
							Case SyntaxKind.InterfaceKeyword
								statementSyntax1 = Me.ParseTypeStatement(attributes, modifiers)
								statementSyntax = statementSyntax1
								Return statementSyntax
							Case Else
								If (kind = SyntaxKind.ModuleKeyword) Then
									statementSyntax1 = Me.ParseTypeStatement(attributes, modifiers)
									statementSyntax = statementSyntax1
									Return statementSyntax
								End If
								GoTo Label2
						End Select
					End If
				ElseIf (kind <= SyntaxKind.ClassKeyword) Then
					If (kind <> SyntaxKind.AddHandlerKeyword) Then
						GoTo Label12
					End If
					statementSyntax1 = Me.ParsePropertyOrEventAccessor(SyntaxKind.AddHandlerAccessorStatement, attributes, modifiers)
				ElseIf (kind = SyntaxKind.DeclareKeyword) Then
					statementSyntax1 = Me.ParseProcDeclareStatement(attributes, modifiers)
				ElseIf (kind = SyntaxKind.DelegateKeyword) Then
					statementSyntax1 = Me.ParseDelegateStatement(attributes, modifiers)
				Else
					If (kind <> SyntaxKind.EnumKeyword) Then
						GoTo Label2
					End If
					statementSyntax1 = Me.ParseEnumStatement(attributes, modifiers)
				End If
			ElseIf (kind > SyntaxKind.RaiseEventKeyword) Then
				If (kind > SyntaxKind.SetKeyword) Then
					If (kind = SyntaxKind.StructureKeyword) Then
						statementSyntax1 = Me.ParseTypeStatement(attributes, modifiers)
						statementSyntax = statementSyntax1
						Return statementSyntax
					End If
					If (kind = SyntaxKind.SubKeyword) Then
						statementSyntax1 = Me.ParseSubStatement(attributes, modifiers)
					Else
						If (kind <> SyntaxKind.IdentifierToken) Then
							GoTo Label2
						End If
						If (Me.Context.BlockKind <> SyntaxKind.EnumBlock OrElse modifiers.Any()) Then
							Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
							If (Me.TryIdentifierAsContextualKeyword(Me.CurrentToken, keywordSyntax)) Then
								If (keywordSyntax.Kind = SyntaxKind.CustomKeyword) Then
									statementSyntax = Me.ParseCustomEventDefinition(attributes, modifiers)
									Return statementSyntax
								End If
								If (keywordSyntax.Kind <> SyntaxKind.TypeKeyword OrElse Me.PeekToken(1).Kind <> SyntaxKind.IdentifierToken OrElse Not Me.IsValidStatementTerminator(Me.PeekToken(2)) OrElse Not modifiers.AnyAndOnly(New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("D0C4B2B40BACF131BB7A80B58B31F5EAB9AEA97EE880CBC79D48E6328433FC35").FieldHandle })) Then
									GoTo Label6
								End If
								statementSyntax1 = Me.ReportUnrecognizedStatementError(ERRID.ERR_ObsoleteStructureNotType, attributes, modifiers, False, False)
								statementSyntax = statementSyntax1
								Return statementSyntax
							End If
						Label6:
							statementSyntax1 = Me.ParseVarDeclStatement(attributes, modifiers)
						Else
							statementSyntax1 = Me.ParseEnumMemberOrLabel(attributes)
						End If
					End If
				ElseIf (kind = SyntaxKind.RemoveHandlerKeyword) Then
					statementSyntax1 = Me.ParsePropertyOrEventAccessor(SyntaxKind.RemoveHandlerAccessorStatement, attributes, modifiers)
				Else
					If (kind <> SyntaxKind.SetKeyword) Then
						GoTo Label2
					End If
					statementSyntax1 = Me.ParsePropertyOrEventAccessor(SyntaxKind.SetAccessorStatement, attributes, modifiers)
				End If
			ElseIf (kind <= SyntaxKind.OperatorKeyword) Then
				If (kind = SyntaxKind.NamespaceKeyword) Then
					statementSyntax1 = Me.ParseNamespaceStatement(attributes, modifiers)
				Else
					If (kind <> SyntaxKind.OperatorKeyword) Then
						GoTo Label2
					End If
					statementSyntax1 = Me.ParseOperatorStatement(attributes, modifiers)
				End If
			ElseIf (kind = SyntaxKind.OptionKeyword) Then
				statementSyntax1 = Me.ParseOptionStatement(attributes, modifiers)
			ElseIf (kind = SyntaxKind.PropertyKeyword) Then
				statementSyntax1 = Me.ParsePropertyDefinition(attributes, modifiers)
			Else
				If (kind <> SyntaxKind.RaiseEventKeyword) Then
					GoTo Label2
				End If
				statementSyntax1 = Me.ParsePropertyOrEventAccessor(SyntaxKind.RaiseEventAccessorStatement, attributes, modifiers)
			End If
			statementSyntax = statementSyntax1
			Return statementSyntax
		Label2:
			Dim blockKind As SyntaxKind = Me.Context.BlockKind
			If (blockKind <= SyntaxKind.NamespaceBlock) Then
				If (blockKind = SyntaxKind.CompilationUnit OrElse blockKind = SyntaxKind.NamespaceBlock) Then
					GoTo Label14
				End If
				statementSyntax1 = Me.ParseVarDeclStatement(attributes, modifiers)
				statementSyntax = statementSyntax1
				Return statementSyntax
			ElseIf (CUShort(blockKind) - CUShort(SyntaxKind.ModuleBlock) > 4 AndAlso blockKind <> SyntaxKind.PropertyBlock) Then
				statementSyntax1 = Me.ParseVarDeclStatement(attributes, modifiers)
				statementSyntax = statementSyntax1
				Return statementSyntax
			End If
			If (attributes.Any() AndAlso Not modifiers.Any()) Then
				statementSyntax1 = Me.ReportUnrecognizedStatementError(ERRID.ERR_StandaloneAttribute, attributes, modifiers, False, False)
				statementSyntax = statementSyntax1
				Return statementSyntax
			ElseIf (Not modifiers.Any() OrElse Not Me.CurrentToken.IsKeyword) Then
				statementSyntax1 = Me.ReportUnrecognizedStatementError(ERRID.ERR_ExpectedIdentifier, attributes, modifiers, True, False)
				statementSyntax = statementSyntax1
				Return statementSyntax
			Else
				statementSyntax1 = Me.ReportUnrecognizedStatementError(ERRID.ERR_InvalidUseOfKeyword, attributes, modifiers, False, True)
				statementSyntax = statementSyntax1
				Return statementSyntax
			End If
		Label12:
			If (kind = SyntaxKind.ClassKeyword) Then
				statementSyntax1 = Me.ParseTypeStatement(attributes, modifiers)
				statementSyntax = statementSyntax1
				Return statementSyntax
			End If
			GoTo Label2
		End Function

		Private Function ParseSpecifiers() As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax) = Me._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)()
			While True
				Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None
				Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.CurrentToken
				Dim kind As SyntaxKind = currentToken.Kind
				If (kind <= SyntaxKind.NotInheritableKeyword) Then
					If (kind <= SyntaxKind.FriendKeyword) Then
						If (kind <= SyntaxKind.DefaultKeyword) Then
							If (kind <> SyntaxKind.ConstKeyword AndAlso kind <> SyntaxKind.DefaultKeyword) Then
								Exit While
							End If
						ElseIf (kind <> SyntaxKind.DimKeyword AndAlso kind <> SyntaxKind.FriendKeyword) Then
							Exit While
						End If
					ElseIf (kind <= SyntaxKind.MustOverrideKeyword) Then
						If (kind <> SyntaxKind.MustInheritKeyword AndAlso kind <> SyntaxKind.MustOverrideKeyword) Then
							Exit While
						End If
					ElseIf (kind <> SyntaxKind.NarrowingKeyword AndAlso kind <> SyntaxKind.NotInheritableKeyword) Then
						Exit While
					End If
				ElseIf (kind <= SyntaxKind.StaticKeyword) Then
					If (kind <= SyntaxKind.ReadOnlyKeyword) Then
						If (kind <> SyntaxKind.NotOverridableKeyword) Then
							Select Case kind
								Case SyntaxKind.OverloadsKeyword
								Case SyntaxKind.OverridableKeyword
								Case SyntaxKind.OverridesKeyword
								Case SyntaxKind.PartialKeyword
								Case SyntaxKind.PrivateKeyword
								Case SyntaxKind.ProtectedKeyword
								Case SyntaxKind.PublicKeyword
								Case SyntaxKind.ReadOnlyKeyword
									Exit Select
								Case SyntaxKind.ParamArrayKeyword
								Case SyntaxKind.PropertyKeyword
								Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.EndRemoveHandlerStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PrivateKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.PublicKeyword
								Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.NotKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OverridesKeyword
								Case SyntaxKind.RaiseEventKeyword
									list = syntaxListBuilder.ToList()
									Me._pool.Free(syntaxListBuilder)
									Return list
								Case Else
									list = syntaxListBuilder.ToList()
									Me._pool.Free(syntaxListBuilder)
									Return list
							End Select
						End If
					ElseIf (CUShort(kind) - CUShort(SyntaxKind.ShadowsKeyword) > CUShort(SyntaxKind.List) AndAlso kind <> SyntaxKind.StaticKeyword) Then
						Exit While
					End If
				ElseIf (kind <= SyntaxKind.WithEventsKeyword) Then
					If (kind <> SyntaxKind.WideningKeyword AndAlso kind <> SyntaxKind.WithEventsKeyword) Then
						Exit While
					End If
				ElseIf (kind <> SyntaxKind.WriteOnlyKeyword) Then
					If (kind <> SyntaxKind.IdentifierToken) Then
						Exit While
					End If
					Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
					If (Not Me.TryTokenAsContextualKeyword(Me.CurrentToken, keywordSyntax)) Then
						Exit While
					End If
					If (keywordSyntax.Kind <> SyntaxKind.CustomKeyword) Then
						If (keywordSyntax.Kind <> SyntaxKind.AsyncKeyword AndAlso keywordSyntax.Kind <> SyntaxKind.IteratorKeyword) Then
							Exit While
						End If
						Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.PeekToken(1)
						If (Not SyntaxFacts.IsSpecifier(syntaxToken.Kind) AndAlso Not SyntaxFacts.CanStartSpecifierDeclaration(syntaxToken.Kind)) Then
							Exit While
						End If
						currentToken = keywordSyntax
						currentToken = Me.CheckFeatureAvailability(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(If(keywordSyntax.Kind = SyntaxKind.AsyncKeyword, Feature.AsyncExpressions, Feature.Iterators), currentToken)
					Else
						Dim syntaxToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.PeekToken(1)
						If (syntaxToken1.Kind = SyntaxKind.EventKeyword OrElse Not SyntaxFacts.IsSpecifier(syntaxToken1.Kind) AndAlso Not SyntaxFacts.CanStartSpecifierDeclaration(syntaxToken1.Kind)) Then
							Exit While
						End If
						currentToken = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(keywordSyntax, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidUseOfCustomModifier)
					End If
				End If
				Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(currentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
					keywordSyntax1 = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(keywordSyntax1, eRRID)
				End If
				syntaxListBuilder.Add(keywordSyntax1)
				Me.GetNextToken(ScannerState.VB)
			End While
			list = syntaxListBuilder.ToList()
			Me._pool.Free(syntaxListBuilder)
			Return list
		End Function

		Friend Function ParseStatementInMethodBody() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim flag As Boolean = Me._hadImplicitLineContinuation
			Dim flag1 As Boolean = Me._hadLineContinuationComment
			Try
				Me._hadImplicitLineContinuation = False
				Me._hadLineContinuationComment = False
				Dim statementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax = Me.ParseStatementInMethodBodyInternal()
				If (Me._hadImplicitLineContinuation) Then
					Dim statementSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax = statementSyntax1
					statementSyntax1 = Me.CheckFeatureAvailability(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(Feature.LineContinuation, statementSyntax1)
					If (statementSyntax2 = statementSyntax1 AndAlso Me._hadLineContinuationComment) Then
						statementSyntax1 = Me.CheckFeatureAvailability(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(Feature.LineContinuationComments, statementSyntax1)
					End If
				End If
				statementSyntax = statementSyntax1
			Finally
				Me._hadImplicitLineContinuation = flag
				Me._hadLineContinuationComment = flag1
			End Try
			Return statementSyntax
		End Function

		Private Function ParseStatementInMethodBodyCore() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Me._cancellationToken.ThrowIfCancellationRequested()
			Dim kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me.CurrentToken.Kind
			If (kind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyToken) Then
				Select Case kind
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerKeyword
						statementSyntax = Me.ParseHandlerStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddressOfKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AliasKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndAlsoKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsValue Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RightShiftAssignmentStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConcatenateAssignmentStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetXmlNamespaceExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleMemberAccessExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlElementEndTag Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlEmptyElement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlAttribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlString Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndAlsoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DictionaryAccessExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlElementEndTag Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlPrefixName Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByRefKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByValKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayRankSpecifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Attribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributesStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrintStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueForStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineElseClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RightShiftAssignmentStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReDimStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RedimClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetXmlNamespaceExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DictionaryAccessExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlDescendantAccessExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InvocationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnonymousObjectCreationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CollectionInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlElementEndTag Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlAttribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlPrefixName Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlBracketedName Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlComment Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlCDataSection Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayType Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PredefinedType Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByRefKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByValKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CBoolKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsValue Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModifiedIdentifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayRankSpecifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributeList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Attribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributeTarget Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributesStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExpressionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrintStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueDoStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueForStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReturnStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfPart Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineElseClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineIfBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RightShiftAssignmentStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConcatenateAssignmentStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CallStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReDimStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReDimPreserveStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RedimClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EraseStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetXmlNamespaceExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleMemberAccessExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DictionaryAccessExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlElementAccessExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlDescendantAccessExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlAttributeAccessExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InvocationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectCreationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnonymousObjectCreationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayCreationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CollectionInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CTypeExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlElementEndTag Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlEmptyElement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlAttribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlString Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlPrefixName Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlName Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlBracketedName Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlPrefix Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlComment Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlProcessingInstruction Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlCDataSection Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlEmbeddedExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayType Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NullableType Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PredefinedType Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierName Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndAlsoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BooleanKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByRefKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByValKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CallKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CatchKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CBoolKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CByteKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReferenceKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsValue Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FinallyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ErrorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RightShiftAssignmentStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConcatenateAssignmentStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotEqualsExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanOrEqualExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanOrEqualExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlElementEndTag Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlEmptyElement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlAttribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlString Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CTypeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CUIntKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CULngKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnErrorGoToZeroStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotEqualsExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlElementEndTag Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlPrefixName Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CTypeKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EachKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Attribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NextLabel Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ResumeStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseEqualsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharacterLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotEqualsExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddressOfExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineSubLambdaExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlElementEndTag Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlComment Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GenericName Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CrefSignaturePart Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CTypeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsValue Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Attribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributeTarget Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NextLabel Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StopStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ResumeStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ResumeLabelStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseEqualsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseNotEqualsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharacterLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TrueLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParenthesizedExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotEqualsExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExclusiveOrExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddressOfExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BinaryConditionalExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineSubLambdaExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubLambdaHeader Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlElementEndTag Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlEmptyElement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlComment Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlProcessingInstruction Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GenericName Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QualifiedName Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CrefSignaturePart Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CrefOperatorReference Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CTypeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CUIntKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DefaultKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseIfKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FalseKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsValue Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModifiedIdentifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Attribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributeTarget Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrintStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueForStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReturnStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineElseClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineIfBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnErrorGoToZeroStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnErrorGoToMinusOneStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ResumeStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ResumeLabelStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CallStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DictionaryAccessExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlElementAccessExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InvocationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectCreationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayCreationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotEqualsExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExclusiveOrExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndAlsoExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UnaryPlusExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QueryExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CollectionRangeVariable Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariableNameEquals Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionAggregation Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LetClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregateClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkipWhileClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TakeWhileClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlElementEndTag Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlEmptyElement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlPrefixName Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlName Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlComment Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlProcessingInstruction Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayType Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NullableType Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndAlsoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BooleanKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByValKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CallKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CBoolKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CTypeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CUIntKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CUShortKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DefaultKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DirectCastKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetTypeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetXmlNamespaceKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterSingleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayRankSpecifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Attribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributesStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrintStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueForStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineElseClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FinallyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnErrorGoToZeroStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnErrorGoToLabelStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ResumeStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ResumeNextStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForEachBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RightShiftAssignmentStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReDimStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RedimClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetXmlNamespaceExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DictionaryAccessExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlDescendantAccessExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InvocationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnonymousObjectCreationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CollectionInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotEqualsExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanOrEqualExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsNotExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndAlsoExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UnaryMinusExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QueryExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExpressionRangeVariable Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariableNameEquals Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GroupAggregation Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LetClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DistinctClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkipWhileClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkipClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlElementEndTag Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlAttribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlPrefixName Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlBracketedName Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlComment Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlCDataSection Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayType Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PredefinedType Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByRefKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByValKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CBoolKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CTypeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CULngKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DirectCastKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoubleKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetTypeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GlobalKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntegerKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsNotKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LibKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LikeKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OfKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionalKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrElseKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParamArrayKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndAddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRemoveHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParamArrayKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PartialKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrivateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PublicKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.REMKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StepKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SharedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StepKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrElseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SharedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShortKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StepKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StopKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ThenKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ToKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TrueKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeOfKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhenKeyword
					Case 576
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregateKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AllKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnsiKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AscendingKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AssemblyKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AutoKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BinaryKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompareKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CustomKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DescendingKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DisableKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DistinctKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParamArrayKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GosubKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompareKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CustomKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DistinctKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndAddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterSingleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PartialKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PublicKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WriteOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariantKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompareKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DescendingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DistinctKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnableKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExplicitKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalSourceKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalChecksumKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FromKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GroupKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntoKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsFalseKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsTrueKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeyKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OffKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrderKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OutKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PreserveKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RegionKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StepKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StringKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WriteOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnsiKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsFalseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PreserveKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionalKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShadowsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StepKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StopKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StringKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WriteOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AllKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnsiKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AscendingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsFalseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsTrueKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PreserveKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RegionKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkipKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StrictKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TakeKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TextKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UnicodeKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UntilKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WarningKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhereKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsyncKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AwaitKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IteratorKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.YieldKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AtToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HashToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AmpersandToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndAddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRemoveHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSyncLockStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterSingleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterMultipleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubNewStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeywordEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsPropertyEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesClauseItem Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IncompleteMember Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FieldDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariableDeclarator Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleAsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsNewClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectMemberInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectCollectionInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferredFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionalKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrElseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverloadsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParamArrayKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PartialKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrivateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PublicKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReadOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReDimKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.REMKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ResumeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReturnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShadowsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SharedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShortKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StaticKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StepKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StopKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StringKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ThenKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ThrowKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ToKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TrueKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryCastKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeOfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UIntegerKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ULongKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UShortKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhenKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WideningKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WriteOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GosubKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariantKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WendKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AllKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnsiKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AscendingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AssemblyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AutoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BinaryKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompareKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CustomKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DescendingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DisableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DistinctKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExplicitKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalSourceKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalChecksumKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FromKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GroupKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsFalseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsTrueKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OffKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrderKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OutKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PreserveKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RegionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkipKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StrictKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TakeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TextKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UnicodeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UntilKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WarningKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhereKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsyncKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AwaitKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IteratorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.YieldKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExclamationToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AtToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HashToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AmpersandToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseParenToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseBraceToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SemicolonToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsteriskToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PlusToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MinusToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SlashToken
						If (Me.CanFollowStatement(Me.CurrentToken)) Then
						Else
							statementSyntax = Me.ReportUnrecognizedStatementError(ERRID.ERR_Syntax)
							Return statementSyntax
						End If
						statementSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EmptyStatement()
						Return statementSyntax
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BooleanKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByteKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CBoolKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CByteKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CCharKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CDateKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CDecKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CDblKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CIntKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CLngKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CObjKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CSByteKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CShortKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CSngKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CStrKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CTypeKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CUIntKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CULngKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CUShortKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DateKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DecimalKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DirectCastKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoubleKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetTypeKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetXmlNamespaceKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GlobalKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntegerKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LongKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MeKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MyBaseKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MyClassKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShortKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StringKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryCastKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UIntegerKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ULongKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UShortKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariantKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExclamationToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DotToken
						statementSyntax = Me.ParseAssignmentOrInvocationStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CallKeyword
						statementSyntax = Me.ParseCallStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseKeyword
						statementSyntax = Me.ParseCaseStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CatchKeyword
						statementSyntax = Me.ParseCatch()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword
						statementSyntax = Me.ParseDeclarationStatementInternal()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DefaultKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DimKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FriendKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MustInheritKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MustOverrideKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NarrowingKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverloadsKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridableKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PartialKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrivateKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PublicKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReadOnlyKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShadowsKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SharedKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StaticKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WideningKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WriteOnlyKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanToken
						Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
						If (Me.CurrentToken.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanToken) Then
							syntaxList = Me.ParseAttributeLists(False)
						End If
						Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax) = Me.ParseSpecifiers()
						If (Not syntaxList1.Any(New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DimKeyword, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstKeyword })) Then
							Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me.CurrentToken.Kind
							If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceKeyword) Then
								If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumKeyword) Then
									If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceKeyword) Then
										GoTo Label6
									End If
									GoTo Label5
								Else
									If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumKeyword) Then
										GoTo Label6
									End If
									GoTo Label5
								End If
							ElseIf (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword) Then
								If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword) Then
									GoTo Label6
								End If
								If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierToken) Then
									Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
									If (Parser.TryIdentifierAsContextualKeyword(Me.CurrentToken, syntaxKind1) AndAlso syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CustomKeyword AndAlso Me.PeekToken(1).Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventKeyword) Then
										statementSyntax = Me.ParseSpecifierDeclaration(syntaxList, syntaxList1)
										Exit Select
									Else
										GoTo Label5
									End If
								Else
									GoTo Label5
								End If
							Else
								If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword) Then
									GoTo Label6
								End If
								GoTo Label5
							End If
						Label6:
							statementSyntax = Me.ParseSpecifierDeclaration(syntaxList, syntaxList1)
							Exit Select
						End If
					Label5:
						statementSyntax = Me.ParseVarDeclStatement(syntaxList, syntaxList1)
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueKeyword
						statementSyntax = Me.ParseContinueStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoKeyword
						statementSyntax = Me.ParseDoStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseKeyword
						If (Me.PeekToken(1).Kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IfKeyword) Then
							statementSyntax = Me.ParseElseStatement()
							Exit Select
						Else
							statementSyntax = Me.ParseElseIfStatement()
							Exit Select
						End If
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseIfKeyword
						statementSyntax = Me.ParseElseIfStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndKeyword
						statementSyntax = Me.ParseEndStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EraseKeyword
						statementSyntax = Me.ParseErase()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ErrorKeyword
						statementSyntax = Me.ParseError()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitKeyword
						statementSyntax = Me.ParseExitStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FinallyKeyword
						statementSyntax = Me.ParseFinally()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForKeyword
						statementSyntax = Me.ParseForStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetKeyword
						If (Me.IsValidStatementTerminator(Me.PeekToken(1)) OrElse Me.PeekToken(1).Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken) Then
							If (Me.Context.IsWithin(New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorBlock })) Then
								Dim syntaxList2 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
								statementSyntax = Me.ParsePropertyOrEventAccessor(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement, syntaxList2, New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)())
								Exit Select
							End If
						End If
						statementSyntax = Me.ReportUnrecognizedStatementError(ERRID.ERR_ObsoleteGetStatement)
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GoToKeyword
						statementSyntax = Me.ParseGotoStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IfKeyword
						statementSyntax = Me.ParseIfStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LetKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetKeyword
						statementSyntax = Me.ParseAssignmentStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LoopKeyword
						statementSyntax = Me.ParseLoopStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NextKeyword
						statementSyntax = Me.ParseNextStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword
						statementSyntax = Me.ParseOnErrorStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventKeyword
						statementSyntax = Me.ParseRaiseEventStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReDimKeyword
						statementSyntax = Me.ParseRedimStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ResumeKeyword
						statementSyntax = Me.ParseResumeStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReturnKeyword
						statementSyntax = Me.ParseReturnStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword
						statementSyntax = Me.ParseSelectStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StopKeyword
						statementSyntax = Me.ParseStopOrEndStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockKeyword
						statementSyntax = Me.ParseExpressionBlockStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ThrowKeyword
						statementSyntax = Me.ParseThrowStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryKeyword
						statementSyntax = Me.ParseTry()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingKeyword
						statementSyntax = Me.ParseUsingStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithKeyword
						statementSyntax = Me.ParseExpressionBlockStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WendKeyword
						statementSyntax = Me.ParseAnachronisticStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GosubKeyword
						statementSyntax = Me.ParseAnachronisticStatement()
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ColonToken
						statementSyntax = Me.ReportUnrecognizedStatementError(ERRID.ERR_Syntax)
						Return statementSyntax
					Case Else
						Select Case kind
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QuestionToken
								If (Not Me.CanStartConsequenceExpression(Me.PeekToken(1).Kind, False)) Then
									statementSyntax = Me.ParsePrintStatement()
								Else
									statementSyntax = Me.ParseAssignmentOrInvocationStatement()
								End If

							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoubleQuoteToken
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOfFileToken
								If (Me.CanFollowStatement(Me.CurrentToken)) Then
								Else
									statementSyntax = Me.ReportUnrecognizedStatementError(ERRID.ERR_Syntax)
									Return statementSyntax
								End If
								statementSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EmptyStatement()
								Return statementSyntax
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StatementTerminatorToken
								statementSyntax = Me.ReportUnrecognizedStatementError(ERRID.ERR_Syntax)
								Return statementSyntax
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyToken
								statementSyntax = Me.ParseEmptyStatement()

							Case Else
								If (Me.CanFollowStatement(Me.CurrentToken)) Then
								Else
									statementSyntax = Me.ReportUnrecognizedStatementError(ERRID.ERR_Syntax)
									Return statementSyntax
								End If
								statementSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EmptyStatement()
								Return statementSyntax
						End Select

				End Select
			ElseIf (kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierToken) Then
				If (kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntegerLiteralToken) Then
					If (Me.CanFollowStatement(Me.CurrentToken)) Then
					Else
						statementSyntax = Me.ReportUnrecognizedStatementError(ERRID.ERR_Syntax)
						Return statementSyntax
					End If
					statementSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EmptyStatement()
					Return statementSyntax
				End If
				If (Not Me.IsFirstStatementOnLine(Me.CurrentToken)) Then
					statementSyntax = Me.ReportUnrecognizedStatementError(ERRID.ERR_Syntax)
					Return statementSyntax
				End If
				statementSyntax = Me.ParseLabel()
			ElseIf (Not Me.ShouldParseAsLabel()) Then
				Dim syntaxKind2 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
				If (Parser.TryIdentifierAsContextualKeyword(Me.CurrentToken, syntaxKind2)) Then
					If (syntaxKind2 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidKeyword) Then
						If (Me.PeekToken(1).Kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken) Then
							GoTo Label3
						End If
						statementSyntax = Me.ParseMid()
						Return statementSyntax
					ElseIf (syntaxKind2 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CustomKeyword AndAlso Me.PeekToken(1).Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventKeyword) Then
						statementSyntax = Me.ParseSpecifierDeclaration()
						Return statementSyntax
					ElseIf (syntaxKind2 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsyncKeyword OrElse syntaxKind2 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IteratorKeyword) Then
						Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.PeekToken(1)
						If (Not SyntaxFacts.IsSpecifier(syntaxToken.Kind) AndAlso Not SyntaxFacts.CanStartSpecifierDeclaration(syntaxToken.Kind)) Then
							GoTo Label3
						End If
						statementSyntax = Me.ParseSpecifierDeclaration()
						Return statementSyntax
					ElseIf (syntaxKind2 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AwaitKeyword OrElse Not Me.Context.IsWithinAsyncMethodOrLambda) Then
						If (syntaxKind2 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.YieldKeyword OrElse Not Me.Context.IsWithinIteratorMethodOrLambdaOrProperty) Then
							GoTo Label3
						End If
						statementSyntax = Me.ParseYieldStatement()
						Return statementSyntax
					Else
						statementSyntax = Me.ParseAwaitStatement()
						Return statementSyntax
					End If
				End If
			Label3:
				statementSyntax = Me.ParseAssignmentOrInvocationStatement()
			Else
				statementSyntax = Me.ParseLabel()
			End If
			Return statementSyntax
			statementSyntax = Me.ReportUnrecognizedStatementError(ERRID.ERR_Syntax)
			Return statementSyntax
		End Function

		Friend Function ParseStatementInMethodBodyInternal() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Try
				Me._recursionDepth = Me._recursionDepth + 1
				StackGuard.EnsureSufficientExecutionStack(Me._recursionDepth)
				statementSyntax = Me.ParseStatementInMethodBodyCore()
			Finally
				Me._recursionDepth = Me._recursionDepth - 1
			End Try
			Return statementSyntax
		End Function

		Private Function ParseStopOrEndStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Return Me.SyntaxFactory.StopOrEndStatement(If(currentToken.Kind = SyntaxKind.StopKeyword, SyntaxKind.StopStatement, SyntaxKind.EndStatement), currentToken)
		End Function

		Private Function ParseStringLiteral() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Nothing
			Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(SyntaxKind.StringLiteralToken, syntaxToken, ScannerState.VB)
			Return Me.SyntaxFactory.StringLiteralExpression(syntaxToken)
		End Function

		Private Sub ParseSubOrDelegateStatement(ByVal kind As SyntaxKind, ByRef ident As IdentifierTokenSyntax, ByRef optionalGenericParams As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax, ByRef optionalParameters As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByRef handlesClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax, ByRef implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax)
			If (kind <> SyntaxKind.SubNewStatement) Then
				ident = Me.ParseIdentifier()
				If (ident.ContainsDiagnostics) Then
					ident = ident.AddTrailingSyntax(Me.ResyncAt(New SyntaxKind() { SyntaxKind.OpenParenToken, SyntaxKind.OfKeyword }))
				End If
			End If
			If (Me.BeginsGeneric(False, False)) Then
				If (kind <> SyntaxKind.SubNewStatement) Then
					optionalGenericParams = Me.ParseGenericParameters()
				Else
					optionalGenericParams = Me.ReportGenericParamsDisallowedError(ERRID.ERR_GenericParamsOnInvalidMember)
				End If
			End If
			optionalParameters = Me.ParseParameterList()
			If (Me.CurrentToken.Kind = SyntaxKind.HandlesKeyword) Then
				handlesClause = Me.ParseHandlesList()
				If (kind = SyntaxKind.DelegateSubStatement) Then
					handlesClause = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax)(handlesClause, ERRID.ERR_DelegateCantHandleEvents)
					Return
				End If
			ElseIf (Me.CurrentToken.Kind = SyntaxKind.ImplementsKeyword) Then
				implementsClause = Me.ParseImplementsList()
				If (kind = SyntaxKind.DelegateSubStatement) Then
					implementsClause = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax)(implementsClause, ERRID.ERR_DelegateCantImplement)
				End If
			End If
		End Sub

		Private Function ParseSubStatement(ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBaseSyntax
			Dim methodBaseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBaseSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim flag As Boolean = Me._isInMethodDeclarationHeader
			Me._isInMethodDeclarationHeader = True
			Dim flag1 As Boolean = Me._isInAsyncMethodDeclarationHeader
			Dim flag2 As Boolean = Me._isInIteratorMethodDeclarationHeader
			Me._isInAsyncMethodDeclarationHeader = modifiers.Any(630)
			Me._isInIteratorMethodDeclarationHeader = modifiers.Any(632)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = Nothing
			Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax = Nothing
			Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax = Nothing
			Dim handlesClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax = Nothing
			Dim implementsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax = Nothing
			If (Me.CurrentToken.Kind = SyntaxKind.NewKeyword) Then
				keywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				Me.GetNextToken(ScannerState.VB)
			End If
			Me.ParseSubOrDelegateStatement(If(keywordSyntax Is Nothing, SyntaxKind.SubStatement, SyntaxKind.SubNewStatement), identifierTokenSyntax, typeParameterListSyntax, parameterListSyntax, handlesClauseSyntax, implementsClauseSyntax)
			Me._isInMethodDeclarationHeader = flag
			Me._isInAsyncMethodDeclarationHeader = flag1
			Me._isInIteratorMethodDeclarationHeader = flag2
			If (keywordSyntax IsNot Nothing) Then
				If (handlesClauseSyntax IsNot Nothing) Then
					keywordSyntax = keywordSyntax.AddError(ERRID.ERR_NewCannotHandleEvents)
				End If
				If (implementsClauseSyntax IsNot Nothing) Then
					keywordSyntax = keywordSyntax.AddError(ERRID.ERR_ImplementsOnNew)
				End If
				If (typeParameterListSyntax IsNot Nothing) Then
					keywordSyntax = keywordSyntax.AddTrailingSyntax(typeParameterListSyntax)
				End If
				methodBaseSyntax = Me.SyntaxFactory.SubNewStatement(attributes, modifiers, currentToken, keywordSyntax, parameterListSyntax).AddTrailingSyntax(handlesClauseSyntax).AddTrailingSyntax(implementsClauseSyntax)
			Else
				methodBaseSyntax = Me.SyntaxFactory.SubStatement(attributes, modifiers, currentToken, identifierTokenSyntax, typeParameterListSyntax, parameterListSyntax, Nothing, handlesClauseSyntax, implementsClauseSyntax)
			End If
			Return methodBaseSyntax
		End Function

		Private Function ParseTerm(Optional ByVal BailIfFirstTokenRejected As Boolean = False, Optional ByVal RedimOrNewParent As Boolean = False) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim flag As Boolean
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Nothing
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.CurrentToken
			Dim kind As SyntaxKind = currentToken.Kind
			If (kind > SyntaxKind.StringKeyword) Then
				If (kind <= SyntaxKind.LessThanToken) Then
					If (kind <= SyntaxKind.ExclamationToken) Then
						Select Case kind
							Case SyntaxKind.SubKeyword
								expressionSyntax1 = Me.ParseLambda(False)
								If (Not Me._evaluatingConditionCompilationExpression) Then
									expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
								End If
								If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
									expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
									Me.GetNextToken(ScannerState.VB)
								End If
								expressionSyntax = expressionSyntax1
								Return expressionSyntax
							Case SyntaxKind.SyncLockKeyword
							Case SyntaxKind.ThenKeyword
							Case SyntaxKind.ThrowKeyword
							Case SyntaxKind.ToKeyword
							Case SyntaxKind.TryKeyword
								GoTo Label0
							Case SyntaxKind.TrueKeyword
								expressionSyntax1 = Me.SyntaxFactory.TrueLiteralExpression(Me.CurrentToken)
								Me.GetNextToken(ScannerState.VB)
								If (Not Me._evaluatingConditionCompilationExpression) Then
									expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
								End If
								If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
									expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
									Me.GetNextToken(ScannerState.VB)
								End If
								expressionSyntax = expressionSyntax1
								Return expressionSyntax
							Case SyntaxKind.TryCastKeyword
								expressionSyntax1 = Me.ParseCast()
								If (Not Me._evaluatingConditionCompilationExpression) Then
									expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
								End If
								If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
									expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
									Me.GetNextToken(ScannerState.VB)
								End If
								expressionSyntax = expressionSyntax1
								Return expressionSyntax
							Case SyntaxKind.TypeOfKeyword
								expressionSyntax1 = Me.ParseTypeOf()
								If (Not Me._evaluatingConditionCompilationExpression) Then
									expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
								End If
								If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
									expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
									Me.GetNextToken(ScannerState.VB)
								End If
								expressionSyntax = expressionSyntax1
								Return expressionSyntax
							Case SyntaxKind.UIntegerKeyword
							Case SyntaxKind.ULongKeyword
							Case SyntaxKind.UShortKeyword
								flag = False
								expressionSyntax1 = Me.ParseTypeName(False, False, flag)
								If (Not Me._evaluatingConditionCompilationExpression) Then
									expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
								End If
								If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
									expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
									Me.GetNextToken(ScannerState.VB)
								End If
								expressionSyntax = expressionSyntax1
								Return expressionSyntax
							Case Else
								If (kind = SyntaxKind.VariantKeyword) Then
									flag = False
									expressionSyntax1 = Me.ParseTypeName(False, False, flag)
									If (Not Me._evaluatingConditionCompilationExpression) Then
										expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
									End If
									If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
										expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
										Me.GetNextToken(ScannerState.VB)
									End If
									expressionSyntax = expressionSyntax1
									Return expressionSyntax
								End If
								If (kind = SyntaxKind.ExclamationToken) Then
									expressionSyntax1 = Me.ParseQualifiedExpr(Nothing)
									If (Not Me._evaluatingConditionCompilationExpression) Then
										expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
									End If
									If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
										expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
										Me.GetNextToken(ScannerState.VB)
									End If
									expressionSyntax = expressionSyntax1
									Return expressionSyntax
								Else
									GoTo Label0
								End If
						End Select
					ElseIf (kind > SyntaxKind.OpenBraceToken) Then
						If (kind <> SyntaxKind.DotToken) Then
							GoTo Label8
						End If
						expressionSyntax1 = Me.ParseQualifiedExpr(Nothing)
						If (Not Me._evaluatingConditionCompilationExpression) Then
							expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
						End If
						If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
							expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
							Me.GetNextToken(ScannerState.VB)
						End If
						expressionSyntax = expressionSyntax1
						Return expressionSyntax
					ElseIf (kind = SyntaxKind.OpenParenToken) Then
						expressionSyntax1 = Me.ParseParenthesizedExpressionOrTupleLiteral()
						If (Not Me._evaluatingConditionCompilationExpression) Then
							expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
						End If
						If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
							expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
							Me.GetNextToken(ScannerState.VB)
						End If
						expressionSyntax = expressionSyntax1
						Return expressionSyntax
					Else
						If (kind <> SyntaxKind.OpenBraceToken) Then
							GoTo Label0
						End If
						expressionSyntax1 = Me.ParseCollectionInitializer()
						If (Not Me._evaluatingConditionCompilationExpression) Then
							expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
						End If
						If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
							expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
							Me.GetNextToken(ScannerState.VB)
						End If
						expressionSyntax = expressionSyntax1
						Return expressionSyntax
					End If
				ElseIf (kind <= SyntaxKind.LessThanQuestionToken) Then
					If (kind = SyntaxKind.LessThanGreaterThanToken OrElse CUShort(kind) - CUShort(SyntaxKind.LessThanSlashToken) <= CUShort(SyntaxKind.List) OrElse kind = SyntaxKind.LessThanQuestionToken) Then
						GoTo Label7
					End If
					GoTo Label0
				ElseIf (kind <= SyntaxKind.CharacterLiteralToken) Then
					If (kind = SyntaxKind.BeginCDataToken) Then
						GoTo Label7
					End If
					Select Case kind
						Case SyntaxKind.IdentifierToken
							Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
							If (Me.TryIdentifierAsContextualKeyword(currentToken, keywordSyntax)) Then
								If (keywordSyntax.Kind = SyntaxKind.FromKeyword OrElse keywordSyntax.Kind = SyntaxKind.AggregateKeyword) Then
									expressionSyntax1 = Me.ParsePotentialQuery(keywordSyntax)
									If (expressionSyntax1 IsNot Nothing) Then
										If (Not Me._evaluatingConditionCompilationExpression) Then
											expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
										End If
										If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
											expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
											Me.GetNextToken(ScannerState.VB)
										End If
										expressionSyntax = expressionSyntax1
										Return expressionSyntax
									End If
								ElseIf (keywordSyntax.Kind = SyntaxKind.AsyncKeyword OrElse keywordSyntax.Kind = SyntaxKind.IteratorKeyword) Then
									Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.PeekToken(1)
									If (syntaxToken.Kind = SyntaxKind.IdentifierToken) Then
										Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
										If (Me.TryTokenAsContextualKeyword(syntaxToken, keywordSyntax1) AndAlso keywordSyntax1.Kind <> keywordSyntax.Kind AndAlso (keywordSyntax1.Kind = SyntaxKind.AsyncKeyword OrElse keywordSyntax1.Kind = SyntaxKind.IteratorKeyword)) Then
											syntaxToken = Me.PeekToken(2)
										End If
									End If
									If (syntaxToken.Kind = SyntaxKind.SubKeyword OrElse syntaxToken.Kind = SyntaxKind.FunctionKeyword) Then
										expressionSyntax1 = Me.ParseLambda(True)
										If (Not Me._evaluatingConditionCompilationExpression) Then
											expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
										End If
										If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
											expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
											Me.GetNextToken(ScannerState.VB)
										End If
										expressionSyntax = expressionSyntax1
										Return expressionSyntax
									End If
								ElseIf (Me.Context.IsWithinAsyncMethodOrLambda AndAlso keywordSyntax.Kind = SyntaxKind.AwaitKeyword) Then
									expressionSyntax1 = Me.ParseAwaitExpression(keywordSyntax)
									If (Not Me._evaluatingConditionCompilationExpression) Then
										expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
									End If
									If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
										expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
										Me.GetNextToken(ScannerState.VB)
									End If
									expressionSyntax = expressionSyntax1
									Return expressionSyntax
								End If
							End If
							expressionSyntax1 = Me.ParseSimpleNameExpressionAllowingKeywordAndTypeArguments()
							If (Not Me._evaluatingConditionCompilationExpression) Then
								expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
							End If
							If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
								expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
								Me.GetNextToken(ScannerState.VB)
							End If
							expressionSyntax = expressionSyntax1
							Return expressionSyntax
						Case SyntaxKind.IntegerLiteralToken
							expressionSyntax1 = Me.ParseIntLiteral()
							If (Not Me._evaluatingConditionCompilationExpression) Then
								expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
							End If
							If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
								expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
								Me.GetNextToken(ScannerState.VB)
							End If
							expressionSyntax = expressionSyntax1
							Return expressionSyntax
						Case SyntaxKind.FloatingLiteralToken
							expressionSyntax1 = Me.ParseFltLiteral()
							If (Not Me._evaluatingConditionCompilationExpression) Then
								expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
							End If
							If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
								expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
								Me.GetNextToken(ScannerState.VB)
							End If
							expressionSyntax = expressionSyntax1
							Return expressionSyntax
						Case SyntaxKind.DecimalLiteralToken
							expressionSyntax1 = Me.ParseDecLiteral()
							If (Not Me._evaluatingConditionCompilationExpression) Then
								expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
							End If
							If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
								expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
								Me.GetNextToken(ScannerState.VB)
							End If
							expressionSyntax = expressionSyntax1
							Return expressionSyntax
						Case SyntaxKind.DateLiteralToken
							expressionSyntax1 = Me.ParseDateLiteral()
							If (Not Me._evaluatingConditionCompilationExpression) Then
								expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
							End If
							If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
								expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
								Me.GetNextToken(ScannerState.VB)
							End If
							expressionSyntax = expressionSyntax1
							Return expressionSyntax
						Case SyntaxKind.StringLiteralToken
							expressionSyntax1 = Me.ParseStringLiteral()
							If (Not Me._evaluatingConditionCompilationExpression) Then
								expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
							End If
							If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
								expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
								Me.GetNextToken(ScannerState.VB)
							End If
							expressionSyntax = expressionSyntax1
							Return expressionSyntax
						Case SyntaxKind.CharacterLiteralToken
							expressionSyntax1 = Me.ParseCharLiteral()
							If (Not Me._evaluatingConditionCompilationExpression) Then
								expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
							End If
							If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
								expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
								Me.GetNextToken(ScannerState.VB)
							End If
							expressionSyntax = expressionSyntax1
							Return expressionSyntax
						Case Else
							GoTo Label0
					End Select
				ElseIf (kind = SyntaxKind.NameOfKeyword) Then
					expressionSyntax1 = Me.ParseNameOf()
					If (Not Me._evaluatingConditionCompilationExpression) Then
						expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
					End If
					If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
						expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
						Me.GetNextToken(ScannerState.VB)
					End If
					expressionSyntax = expressionSyntax1
					Return expressionSyntax
				Else
					If (kind <> SyntaxKind.DollarSignDoubleQuoteToken) Then
						GoTo Label0
					End If
					expressionSyntax1 = Me.ParseInterpolatedStringExpression()
					If (Not Me._evaluatingConditionCompilationExpression) Then
						expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
					End If
					If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
						expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
						Me.GetNextToken(ScannerState.VB)
					End If
					expressionSyntax = expressionSyntax1
					Return expressionSyntax
				End If
			Label7:
				If (Parser.TokenContainsFullWidthChars(currentToken)) Then
					GoTo Label10
				End If
				expressionSyntax1 = Me.ParseXmlExpression()
			ElseIf (kind <= SyntaxKind.MyBaseKeyword) Then
				If (kind <= SyntaxKind.IfKeyword) Then
					Select Case kind
						Case SyntaxKind.BooleanKeyword
						Case SyntaxKind.ByteKeyword
						Case SyntaxKind.CharKeyword
						Case SyntaxKind.DateKeyword
						Case SyntaxKind.DecimalKeyword
						Case SyntaxKind.DoubleKeyword
							flag = False
							expressionSyntax1 = Me.ParseTypeName(False, False, flag)
							If (Not Me._evaluatingConditionCompilationExpression) Then
								expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
							End If
							If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
								expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
								Me.GetNextToken(ScannerState.VB)
							End If
							expressionSyntax = expressionSyntax1
							Return expressionSyntax
						Case SyntaxKind.ByRefKeyword
						Case SyntaxKind.ByValKeyword
						Case SyntaxKind.CallKeyword
						Case SyntaxKind.CaseKeyword
						Case SyntaxKind.CatchKeyword
						Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.PrintStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineElseClause Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.MidExpression Or SyntaxKind.AddHandlerStatement Or SyntaxKind.RaiseEventStatement Or SyntaxKind.ReDimStatement Or SyntaxKind.RedimClause Or SyntaxKind.GetXmlNamespaceExpression Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.XmlDescendantAccessExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.AnonymousObjectCreationExpression Or SyntaxKind.CollectionInitializer Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlPrefixName Or SyntaxKind.XmlBracketedName Or SyntaxKind.XmlComment Or SyntaxKind.XmlCDataSection Or SyntaxKind.ArrayType Or SyntaxKind.PredefinedType Or SyntaxKind.AndKeyword Or SyntaxKind.AsKeyword Or SyntaxKind.ByRefKeyword Or SyntaxKind.ByValKeyword Or SyntaxKind.CaseKeyword Or SyntaxKind.CBoolKeyword
						Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueDoStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineIfPart Or SyntaxKind.SingleLineElseClause Or SyntaxKind.MultiLineIfBlock Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.ConcatenateAssignmentStatement Or SyntaxKind.MidExpression Or SyntaxKind.CallStatement Or SyntaxKind.AddHandlerStatement Or SyntaxKind.RemoveHandlerStatement Or SyntaxKind.RaiseEventStatement Or SyntaxKind.WithStatement Or SyntaxKind.ReDimStatement Or SyntaxKind.ReDimPreserveStatement Or SyntaxKind.RedimClause Or SyntaxKind.EraseStatement Or SyntaxKind.GetXmlNamespaceExpression Or SyntaxKind.SimpleMemberAccessExpression Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.XmlElementAccessExpression Or SyntaxKind.XmlDescendantAccessExpression Or SyntaxKind.XmlAttributeAccessExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.ObjectCreationExpression Or SyntaxKind.AnonymousObjectCreationExpression Or SyntaxKind.ArrayCreationExpression Or SyntaxKind.CollectionInitializer Or SyntaxKind.CTypeExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlEmptyElement Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlString Or SyntaxKind.XmlPrefixName Or SyntaxKind.XmlName Or SyntaxKind.XmlBracketedName Or SyntaxKind.XmlPrefix Or SyntaxKind.XmlComment Or SyntaxKind.XmlProcessingInstruction Or SyntaxKind.XmlCDataSection Or SyntaxKind.XmlEmbeddedExpression Or SyntaxKind.ArrayType Or SyntaxKind.NullableType Or SyntaxKind.PredefinedType Or SyntaxKind.IdentifierName Or SyntaxKind.AndKeyword Or SyntaxKind.AndAlsoKeyword Or SyntaxKind.AsKeyword Or SyntaxKind.BooleanKeyword Or SyntaxKind.ByRefKeyword Or SyntaxKind.ByteKeyword Or SyntaxKind.ByValKeyword Or SyntaxKind.CallKeyword Or SyntaxKind.CaseKeyword Or SyntaxKind.CatchKeyword Or SyntaxKind.CBoolKeyword Or SyntaxKind.CByteKeyword
						Case SyntaxKind.ClassKeyword
						Case SyntaxKind.ConstKeyword
						Case SyntaxKind.ReferenceKeyword
						Case SyntaxKind.ContinueKeyword
						Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.ConcatenateAssignmentStatement Or SyntaxKind.NotEqualsExpression Or SyntaxKind.LessThanExpression Or SyntaxKind.LessThanOrEqualExpression Or SyntaxKind.GreaterThanOrEqualExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlEmptyElement Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlString Or SyntaxKind.CTypeKeyword Or SyntaxKind.CUIntKeyword Or SyntaxKind.CULngKeyword
						Case SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.MidExpression Or SyntaxKind.NotEqualsExpression Or SyntaxKind.GreaterThanExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlPrefixName Or SyntaxKind.CTypeKeyword
						Case SyntaxKind.DeclareKeyword
						Case SyntaxKind.DefaultKeyword
						Case SyntaxKind.DelegateKeyword
						Case SyntaxKind.DimKeyword
						Case SyntaxKind.DoKeyword
							GoTo Label0
						Case SyntaxKind.CBoolKeyword
						Case SyntaxKind.CByteKeyword
						Case SyntaxKind.CCharKeyword
						Case SyntaxKind.CDateKeyword
						Case SyntaxKind.CDecKeyword
						Case SyntaxKind.CDblKeyword
						Case SyntaxKind.CIntKeyword
						Case SyntaxKind.CLngKeyword
						Case SyntaxKind.CObjKeyword
						Case SyntaxKind.CSByteKeyword
						Case SyntaxKind.CShortKeyword
						Case SyntaxKind.CSngKeyword
						Case SyntaxKind.CStrKeyword
						Case SyntaxKind.CUIntKeyword
						Case SyntaxKind.CULngKeyword
						Case SyntaxKind.CUShortKeyword
							expressionSyntax1 = Me.ParseCastExpression()
							Exit Select
						Case SyntaxKind.CTypeKeyword
						Case SyntaxKind.DirectCastKeyword
							expressionSyntax1 = Me.ParseCast()
							If (Not Me._evaluatingConditionCompilationExpression) Then
								expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
							End If
							If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
								expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
								Me.GetNextToken(ScannerState.VB)
							End If
							expressionSyntax = expressionSyntax1
							Return expressionSyntax
						Case Else
							Select Case kind
								Case SyntaxKind.FalseKeyword
									expressionSyntax1 = Me.SyntaxFactory.FalseLiteralExpression(Me.CurrentToken)
									Me.GetNextToken(ScannerState.VB)

								Case SyntaxKind.FinallyKeyword
								Case SyntaxKind.ForKeyword
								Case SyntaxKind.FriendKeyword
								Case SyntaxKind.GetKeyword
									GoTo Label0
								Case SyntaxKind.FunctionKeyword
									expressionSyntax1 = Me.ParseLambda(False)
									If (Not Me._evaluatingConditionCompilationExpression) Then
										expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
									End If
									If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
										expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
										Me.GetNextToken(ScannerState.VB)
									End If
									expressionSyntax = expressionSyntax1
									Return expressionSyntax
								Case SyntaxKind.GetTypeKeyword
									expressionSyntax1 = Me.ParseGetType()

								Case SyntaxKind.GetXmlNamespaceKeyword
									expressionSyntax1 = Me.ParseGetXmlNamespace()

								Case SyntaxKind.GlobalKeyword
									expressionSyntax1 = Me.SyntaxFactory.GlobalName(DirectCast(currentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax))
									Me.GetNextToken(ScannerState.VB)
									If (Me.CurrentToken.Kind = SyntaxKind.DotToken) Then
										If (Not Me._evaluatingConditionCompilationExpression) Then
											expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
										End If
										If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
											expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
											Me.GetNextToken(ScannerState.VB)
										End If
										expressionSyntax = expressionSyntax1
										Return expressionSyntax
									End If
									expressionSyntax1 = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax1, ERRID.ERR_ExpectedDotAfterGlobalNameSpace)

								Case Else
									If (kind = SyntaxKind.IfKeyword) Then
										expressionSyntax1 = Me.ParseIfExpression()
									Else
										GoTo Label0
									End If

							End Select

					End Select
				ElseIf (kind <= SyntaxKind.LongKeyword) Then
					If (kind = SyntaxKind.IntegerKeyword OrElse kind = SyntaxKind.LongKeyword) Then
						flag = False
						expressionSyntax1 = Me.ParseTypeName(False, False, flag)
						If (Not Me._evaluatingConditionCompilationExpression) Then
							expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
						End If
						If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
							expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
							Me.GetNextToken(ScannerState.VB)
						End If
						expressionSyntax = expressionSyntax1
						Return expressionSyntax
					End If
					GoTo Label0
				ElseIf (kind = SyntaxKind.MeKeyword) Then
					expressionSyntax1 = Me.SyntaxFactory.MeExpression(DirectCast(currentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax))
					Me.GetNextToken(ScannerState.VB)
				Else
					If (kind <> SyntaxKind.MyBaseKeyword) Then
						GoTo Label0
					End If
					expressionSyntax1 = Me.SyntaxFactory.MyBaseExpression(DirectCast(currentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax))
					Me.GetNextToken(ScannerState.VB)
					If (Me.CurrentToken.Kind <> SyntaxKind.DotToken) Then
						expressionSyntax1 = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax1, ERRID.ERR_ExpectedDotAfterMyBase)
					End If
				End If
			ElseIf (kind > SyntaxKind.NothingKeyword) Then
				If (kind > SyntaxKind.SByteKeyword) Then
					If (CUShort(kind) - CUShort(SyntaxKind.ShortKeyword) <= CUShort(SyntaxKind.List) OrElse kind = SyntaxKind.StringKeyword) Then
						flag = False
						expressionSyntax1 = Me.ParseTypeName(False, False, flag)
						If (Not Me._evaluatingConditionCompilationExpression) Then
							expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
						End If
						If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
							expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
							Me.GetNextToken(ScannerState.VB)
						End If
						expressionSyntax = expressionSyntax1
						Return expressionSyntax
					End If
					GoTo Label0
				Else
					If (kind = SyntaxKind.ObjectKeyword OrElse kind = SyntaxKind.SByteKeyword) Then
						flag = False
						expressionSyntax1 = Me.ParseTypeName(False, False, flag)
						If (Not Me._evaluatingConditionCompilationExpression) Then
							expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
						End If
						If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
							expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
							Me.GetNextToken(ScannerState.VB)
						End If
						expressionSyntax = expressionSyntax1
						Return expressionSyntax
					End If
					GoTo Label0
				End If
			ElseIf (kind = SyntaxKind.MyClassKeyword) Then
				expressionSyntax1 = Me.SyntaxFactory.MyClassExpression(DirectCast(currentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax))
				Me.GetNextToken(ScannerState.VB)
				If (Me.CurrentToken.Kind <> SyntaxKind.DotToken) Then
					expressionSyntax1 = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax1, ERRID.ERR_ExpectedDotAfterMyClass)
				End If
			ElseIf (kind = SyntaxKind.NewKeyword) Then
				expressionSyntax1 = Me.ParseNewExpression()
			Else
				If (kind <> SyntaxKind.NothingKeyword) Then
					GoTo Label0
				End If
				expressionSyntax1 = Me.SyntaxFactory.NothingLiteralExpression(Me.CurrentToken)
				Me.GetNextToken(ScannerState.VB)
			End If
			If (Not Me._evaluatingConditionCompilationExpression) Then
				expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
			End If
			If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
				expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
				Me.GetNextToken(ScannerState.VB)
			End If
			expressionSyntax = expressionSyntax1
			Return expressionSyntax
		Label0:
			If (currentToken.Kind <> SyntaxKind.QuestionToken OrElse Not Me.CanStartConsequenceExpression(Me.PeekToken(1).Kind, False)) Then
				If (BailIfFirstTokenRejected) Then
					expressionSyntax = Nothing
					Return expressionSyntax
				End If
				expressionSyntax1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression()
				expressionSyntax1 = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax1, ERRID.ERR_ExpectedExpression)
				If (Not Me._evaluatingConditionCompilationExpression) Then
					expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
				End If
				If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
					expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
					Me.GetNextToken(ScannerState.VB)
				End If
				expressionSyntax = expressionSyntax1
				Return expressionSyntax
			Else
				Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Me.CheckFeatureAvailability(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(Feature.NullPropagatingOperator, DirectCast(currentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax))
				Me.GetNextToken(ScannerState.VB)
				expressionSyntax1 = Me.SyntaxFactory.ConditionalAccessExpression(expressionSyntax1, punctuationSyntax, Me.ParsePostFixExpression(RedimOrNewParent, Nothing))
				If (Not Me._evaluatingConditionCompilationExpression) Then
					expressionSyntax1 = Me.ParsePostFixExpression(RedimOrNewParent, expressionSyntax1)
				End If
				If (Me.CurrentToken IsNot Nothing AndAlso Me.CurrentToken.Kind = SyntaxKind.QuestionToken) Then
					expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_NullableCharNotSupported)
					Me.GetNextToken(ScannerState.VB)
				End If
				expressionSyntax = expressionSyntax1
				Return expressionSyntax
			End If
			Return expressionSyntax
		Label8:
			If (kind = SyntaxKind.LessThanToken) Then
				GoTo Label7
			End If
			GoTo Label0
		Label10:
			If (Not BailIfFirstTokenRejected) Then
				expressionSyntax1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression()
				expressionSyntax1 = expressionSyntax1.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_FullWidthAsXmlDelimiter)
				Me.GetNextToken(ScannerState.VB)
				expressionSyntax = expressionSyntax1
				Return expressionSyntax
			Else
				expressionSyntax = Nothing
				Return expressionSyntax
			End If
		End Function

		Private Function ParseTheRestOfTupleLiteral(ByVal openParen As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, ByVal firstArgument As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax)()
			separatedSyntaxListBuilder.Add(firstArgument)
			While Me.CurrentToken.Kind = SyntaxKind.CommaToken
				Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax, False, ScannerState.VB)
				separatedSyntaxListBuilder.AddSeparator(punctuationSyntax)
				Dim nameColonEqualsSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameColonEqualsSyntax = Nothing
				If (Me.CurrentToken.Kind = SyntaxKind.IdentifierToken AndAlso Me.PeekToken(1).Kind = SyntaxKind.ColonEqualsToken) Then
					Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax = Me.ParseIdentifierNameAllowingKeyword()
					Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
					Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.ColonEqualsToken, punctuationSyntax1, False, ScannerState.VB)
					nameColonEqualsSyntax = Me.SyntaxFactory.NameColonEquals(identifierNameSyntax, punctuationSyntax1)
				End If
				Dim simpleArgumentSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax = Me.SyntaxFactory.SimpleArgument(nameColonEqualsSyntax, Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False))
				separatedSyntaxListBuilder.Add(simpleArgumentSyntax)
			End While
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.TryEatNewLineAndGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CloseParenToken, punctuationSyntax2, True, ScannerState.VB)
			If (separatedSyntaxListBuilder.Count < 2) Then
				separatedSyntaxListBuilder.AddSeparator(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.CommaToken))
				Dim identifierNameSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax = Me.SyntaxFactory.IdentifierName(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier())
				identifierNameSyntax1 = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)(identifierNameSyntax1, ERRID.ERR_TupleTooFewElements)
				separatedSyntaxListBuilder.Add(Me.SyntaxFactory.SimpleArgument(Nothing, identifierNameSyntax1))
			End If
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax) = separatedSyntaxListBuilder.ToList()
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax)(separatedSyntaxListBuilder)
			Dim tupleExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax = Me.SyntaxFactory.TupleExpression(openParen, list, punctuationSyntax2)
			Return Me.CheckFeatureAvailability(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax)(Feature.Tuples, tupleExpressionSyntax)
		End Function

		Private Function ParseThrowStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ThrowStatementSyntax
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, True)
			If (expressionSyntax IsNot Nothing AndAlso expressionSyntax.ContainsDiagnostics) Then
				expressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax)
			End If
			Return Me.SyntaxFactory.ThrowStatement(currentToken, expressionSyntax)
		End Function

		Private Function ParseTry() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Return Me.SyntaxFactory.TryStatement(currentToken)
		End Function

		Private Function ParseTupleType(ByVal openParen As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax
			Dim tupleElementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleElementSyntax
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleElementSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleElementSyntax)()
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = Nothing
			While True
				Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = Nothing
				Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
				If (Me.CurrentToken.Kind = SyntaxKind.IdentifierToken AndAlso (DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax).TypeCharacter <> TypeCharacter.None OrElse Me.PeekNextToken(ScannerState.VB).Kind = SyntaxKind.AsKeyword)) Then
					identifierTokenSyntax = Me.ParseIdentifier()
					Me.TryGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(SyntaxKind.AsKeyword, keywordSyntax)
				End If
				Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = Nothing
				If (keywordSyntax IsNot Nothing OrElse identifierTokenSyntax Is Nothing) Then
					typeSyntax = Me.ParseGeneralType(False)
				End If
				If (identifierTokenSyntax Is Nothing) Then
					tupleElementSyntax = Me.SyntaxFactory.TypedTupleElement(typeSyntax)
				Else
					Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = Nothing
					If (keywordSyntax IsNot Nothing) Then
						simpleAsClauseSyntax = Me.SyntaxFactory.SimpleAsClause(keywordSyntax, New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode)(), typeSyntax)
					End If
					tupleElementSyntax = Me.SyntaxFactory.NamedTupleElement(identifierTokenSyntax, simpleAsClauseSyntax)
				End If
				separatedSyntaxListBuilder.Add(tupleElementSyntax)
				Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, currentToken, False, ScannerState.VB)) Then
					If (Me.CurrentToken.Kind = SyntaxKind.CloseParenToken OrElse Me.MustEndStatement(Me.CurrentToken)) Then
						Exit While
					End If
					Dim node As Microsoft.CodeAnalysis.GreenNode = Me.ResyncAt(New SyntaxKind() { SyntaxKind.CommaToken, SyntaxKind.CloseParenToken }).Node
					If (node IsNot Nothing AndAlso Not tupleElementSyntax.ContainsDiagnostics) Then
						node = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.GreenNode)(node, ERRID.ERR_ArgumentSyntax)
					End If
					If (Me.CurrentToken.Kind <> SyntaxKind.CommaToken) Then
						greenNode = node
						Exit While
					Else
						currentToken = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
						currentToken = currentToken.AddLeadingSyntax(node)
						separatedSyntaxListBuilder.AddSeparator(currentToken)
						Me.GetNextToken(ScannerState.VB)
					End If
				Else
					separatedSyntaxListBuilder.AddSeparator(currentToken)
				End If
			End While
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.TryEatNewLineAndGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CloseParenToken, punctuationSyntax, True, ScannerState.VB)
			If (greenNode IsNot Nothing) Then
				punctuationSyntax = punctuationSyntax.AddLeadingSyntax(greenNode)
			End If
			If (separatedSyntaxListBuilder.Count < 2) Then
				separatedSyntaxListBuilder.AddSeparator(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.CommaToken))
				Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax = Me.SyntaxFactory.IdentifierName(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier())
				identifierNameSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)(identifierNameSyntax, ERRID.ERR_TupleTooFewElements)
				separatedSyntaxListBuilder.Add(Me._syntaxFactory.TypedTupleElement(identifierNameSyntax))
			End If
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleElementSyntax) = separatedSyntaxListBuilder.ToList()
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleElementSyntax)(separatedSyntaxListBuilder)
			Dim tupleTypeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleTypeSyntax = Me.SyntaxFactory.TupleType(openParen, list, punctuationSyntax)
			Return Me.CheckFeatureAvailability(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleTypeSyntax)(Feature.Tuples, tupleTypeSyntax)
		End Function

		Friend Function ParseTypeName(Optional ByVal nonArrayName As Boolean = False, Optional ByVal allowEmptyGenericArguments As Boolean = False, Optional ByRef allowedEmptyGenericArguments As Boolean = False) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax
			Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID
			Dim kind As SyntaxKind
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.CurrentToken
			Dim prevToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.PrevToken
			Dim typeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = Nothing
			Dim nameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax = Nothing
			If (Not SyntaxFacts.IsPredefinedTypeKeyword(currentToken.Kind)) Then
				kind = currentToken.Kind
				If (kind > SyntaxKind.VariantKeyword) Then
					If (kind <> SyntaxKind.OpenParenToken) Then
						GoTo Label4
					End If
					Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
					Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.OpenParenToken, punctuationSyntax, False, ScannerState.VB)
					typeSyntax1 = Me.ParseTupleType(punctuationSyntax)
					GoTo Label0
				Else
					If (kind = SyntaxKind.GlobalKeyword) Then
						GoTo Label2
					End If
					If (kind <> SyntaxKind.VariantKeyword) Then
						If (currentToken.Kind = SyntaxKind.NewKeyword AndAlso Me.PeekToken(1).Kind = SyntaxKind.IdentifierToken) Then
							eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidNewInType
						ElseIf (currentToken.Kind <> SyntaxKind.OpenBraceToken OrElse prevToken Is Nothing OrElse prevToken.Kind <> SyntaxKind.NewKeyword) Then
							eRRID = If(Not currentToken.IsKeyword, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_UnrecognizedType, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_UnrecognizedTypeKeyword)
						Else
							eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_UnrecognizedTypeOrWith
						End If
						typeSyntax1 = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)(Me.SyntaxFactory.IdentifierName(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier()), eRRID)
						typeSyntax = typeSyntax1
						Return typeSyntax
					End If
					nameSyntax = Me.SyntaxFactory.IdentifierName(Me._scanner.MakeIdentifier(DirectCast(currentToken, KeywordSyntax)))
					nameSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax)(nameSyntax, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ObsoleteObjectNotVariant)
					GoTo Label3
				End If
				If (currentToken.Kind = SyntaxKind.NewKeyword AndAlso Me.PeekToken(1).Kind = SyntaxKind.IdentifierToken) Then
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidNewInType
				ElseIf (currentToken.Kind <> SyntaxKind.OpenBraceToken OrElse prevToken Is Nothing OrElse prevToken.Kind <> SyntaxKind.NewKeyword) Then
					eRRID = If(Not currentToken.IsKeyword, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_UnrecognizedType, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_UnrecognizedTypeKeyword)
				Else
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_UnrecognizedTypeOrWith
				End If
				typeSyntax1 = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)(Me.SyntaxFactory.IdentifierName(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier()), eRRID)
				typeSyntax = typeSyntax1
				Return typeSyntax
			Else
				typeSyntax1 = Me.SyntaxFactory.PredefinedType(DirectCast(currentToken, KeywordSyntax))
			End If
		Label3:
			Me.GetNextToken(ScannerState.VB)
		Label0:
			If (typeSyntax1 Is Nothing) Then
				typeSyntax1 = nameSyntax
			End If
			If (675 = CInt(Me.CurrentToken.Kind)) Then
				If (Me._evaluatingConditionCompilationExpression) Then
					typeSyntax1 = typeSyntax1.AddTrailingSyntax(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadNullTypeInCCExpression)
					Me.GetNextToken(ScannerState.VB)
					typeSyntax = typeSyntax1
					Return typeSyntax
				End If
				If (allowedEmptyGenericArguments) Then
					typeSyntax1 = Me.ReportUnrecognizedTypeInGeneric(typeSyntax1)
				End If
				Dim currentToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
				Dim nullableTypeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NullableTypeSyntax = Me.SyntaxFactory.NullableType(typeSyntax1, currentToken1)
				Me.GetNextToken(ScannerState.VB)
				typeSyntax1 = nullableTypeSyntax
			End If
			typeSyntax = typeSyntax1
			Return typeSyntax
		Label2:
			nameSyntax = Me.ParseName(False, True, True, True, nonArrayName, False, allowEmptyGenericArguments, allowedEmptyGenericArguments, False)
			GoTo Label0
		Label4:
			If (kind = SyntaxKind.IdentifierToken) Then
				GoTo Label2
			End If
			If (currentToken.Kind = SyntaxKind.NewKeyword AndAlso Me.PeekToken(1).Kind = SyntaxKind.IdentifierToken) Then
				eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidNewInType
			ElseIf (currentToken.Kind <> SyntaxKind.OpenBraceToken OrElse prevToken Is Nothing OrElse prevToken.Kind <> SyntaxKind.NewKeyword) Then
				eRRID = If(Not currentToken.IsKeyword, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_UnrecognizedType, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_UnrecognizedTypeKeyword)
			Else
				eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_UnrecognizedTypeOrWith
			End If
			typeSyntax1 = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)(Me.SyntaxFactory.IdentifierName(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier()), eRRID)
			typeSyntax = typeSyntax1
			Return typeSyntax
		End Function

		Private Function ParseTypeOf() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceRelational, False)
			If (expressionSyntax.ContainsDiagnostics) Then
				expressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax, New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsKeyword, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsNotKeyword })
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.CurrentToken
			If (syntaxToken.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsKeyword OrElse syntaxToken.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsNotKeyword) Then
				keywordSyntax = DirectCast(syntaxToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
				If (keywordSyntax.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsNotKeyword) Then
					keywordSyntax = Me.CheckFeatureAvailability(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(Feature.TypeOfIsNot, keywordSyntax)
				End If
				Me.GetNextToken(ScannerState.VB)
				Me.TryEatNewLine(ScannerState.VB)
			Else
				keywordSyntax = DirectCast(Parser.HandleUnexpectedToken(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			End If
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = Me.ParseGeneralType(False)
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = If(keywordSyntax.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsNotKeyword, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeOfIsNotExpression, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeOfIsExpression)
			Return Me.SyntaxFactory.TypeOfExpression(syntaxKind, currentToken, expressionSyntax, keywordSyntax, typeSyntax)
		End Function

		Private Function ParseTypeStatement(Optional ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = Nothing, Optional ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax) = Nothing) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeStatementSyntax
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax = Nothing
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = currentToken.Kind
			If (kind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceKeyword) Then
				If (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassKeyword) Then
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassStatement
				Else
					If (kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceKeyword) Then
						Throw ExceptionUtilities.UnexpectedValue(currentToken.Kind)
					End If
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceStatement
				End If
			ElseIf (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleKeyword) Then
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleStatement
			Else
				If (kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword) Then
					Throw ExceptionUtilities.UnexpectedValue(currentToken.Kind)
				End If
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureStatement
			End If
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = Me.ParseIdentifier()
			If (identifierTokenSyntax.ContainsDiagnostics) Then
				identifierTokenSyntax = identifierTokenSyntax.AddTrailingSyntax(Me.ResyncAt(New Microsoft.CodeAnalysis.VisualBasic.SyntaxKind() { Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OfKeyword, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken }))
			End If
			If (Me.CurrentToken.Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken) Then
				If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleStatement) Then
					typeParameterListSyntax = Me.ParseGenericParameters()
				Else
					identifierTokenSyntax = identifierTokenSyntax.AddTrailingSyntax(Me.ReportGenericParamsDisallowedError(ERRID.ERR_ModulesCannotBeGeneric))
				End If
			End If
			Dim typeStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.TypeStatement(syntaxKind, attributes, modifiers, currentToken, identifierTokenSyntax, typeParameterListSyntax)
			If ((syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleStatement OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceStatement) AndAlso typeStatementSyntax.Modifiers.Any(530)) Then
				typeStatementSyntax = Me.CheckFeatureAvailability(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeStatementSyntax)(If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleStatement, Feature.PartialModules, Feature.PartialInterfaces), typeStatementSyntax)
			End If
			Return typeStatementSyntax
		End Function

		Private Function ParseUsingStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax
			Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Nothing
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax)()
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.PeekToken(1)
			If (syntaxToken.Kind = SyntaxKind.AsKeyword OrElse syntaxToken.Kind = SyntaxKind.EqualsToken OrElse syntaxToken.Kind = SyntaxKind.CommaToken OrElse syntaxToken.Kind = SyntaxKind.QuestionToken) Then
				separatedSyntaxList = Me.ParseVariableDeclaration(True)
			Else
				expressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
			End If
			Return Me.SyntaxFactory.UsingStatement(currentToken, expressionSyntax, separatedSyntaxList)
		End Function

		Private Function ParseVarDeclStatement(ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax)
			Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim statementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID
			Dim flag As Boolean = False
			Dim blockKind As SyntaxKind = Me.Context.BlockKind
			If (blockKind <= SyntaxKind.NamespaceBlock) Then
				If (blockKind = SyntaxKind.CompilationUnit OrElse blockKind = SyntaxKind.NamespaceBlock) Then
					GoTo Label1
				End If
				separatedSyntaxList = Me.ParseVariableDeclaration(Not flag)
				If (Not flag) Then
					statementSyntax = Me.SyntaxFactory.LocalDeclarationStatement(modifiers, separatedSyntaxList)
					If (attributes.Any()) Then
						statementSyntax = If(Not modifiers.Any(551), statementSyntax.AddLeadingSyntax(attributes.Node, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_LocalsCannotHaveAttributes), statementSyntax.AddLeadingSyntax(attributes.Node))
					End If
				Else
					statementSyntax = Me.SyntaxFactory.FieldDeclaration(attributes, modifiers, separatedSyntaxList)
				End If
				If (Not modifiers.Any()) Then
					statementSyntax1 = statementSyntax
					eRRID = If(attributes.Any(), Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_StandaloneAttribute, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedSpecifier)
					statementSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(statementSyntax1, eRRID)
				End If
				Return statementSyntax
			ElseIf (CUShort(blockKind) - CUShort(SyntaxKind.ModuleBlock) > 4 AndAlso blockKind <> SyntaxKind.PropertyBlock) Then
				separatedSyntaxList = Me.ParseVariableDeclaration(Not flag)
				If (Not flag) Then
					statementSyntax = Me.SyntaxFactory.LocalDeclarationStatement(modifiers, separatedSyntaxList)
					If (attributes.Any()) Then
						statementSyntax = If(Not modifiers.Any(551), statementSyntax.AddLeadingSyntax(attributes.Node, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_LocalsCannotHaveAttributes), statementSyntax.AddLeadingSyntax(attributes.Node))
					End If
				Else
					statementSyntax = Me.SyntaxFactory.FieldDeclaration(attributes, modifiers, separatedSyntaxList)
				End If
				If (Not modifiers.Any()) Then
					statementSyntax1 = statementSyntax
					eRRID = If(attributes.Any(), Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_StandaloneAttribute, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedSpecifier)
					statementSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(statementSyntax1, eRRID)
				End If
				Return statementSyntax
			End If
		Label1:
			flag = True
			separatedSyntaxList = Me.ParseVariableDeclaration(Not flag)
			If (Not flag) Then
				statementSyntax = Me.SyntaxFactory.LocalDeclarationStatement(modifiers, separatedSyntaxList)
				If (attributes.Any()) Then
					statementSyntax = If(Not modifiers.Any(551), statementSyntax.AddLeadingSyntax(attributes.Node, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_LocalsCannotHaveAttributes), statementSyntax.AddLeadingSyntax(attributes.Node))
				End If
			Else
				statementSyntax = Me.SyntaxFactory.FieldDeclaration(attributes, modifiers, separatedSyntaxList)
			End If
			If (Not modifiers.Any()) Then
				statementSyntax1 = statementSyntax
				eRRID = If(attributes.Any(), Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_StandaloneAttribute, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpectedSpecifier)
				statementSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(statementSyntax1, eRRID)
			End If
			Return statementSyntax
		End Function

		Private Function ParseVariable() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Return Me.ParseExpressionCore(OperatorPrecedence.PrecedenceRelational, False)
		End Function

		Private Function ParseVariableDeclaration(ByVal allowAsNewWith As Boolean) As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax)()
			Dim flag As Boolean = True
			Dim separatedSyntaxListBuilder1 As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax)()
			While True
				separatedSyntaxListBuilder1.Clear()
				While True
					Dim modifiedIdentifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax = Me.ParseModifiedIdentifier(True, flag)
					flag = False
					If (modifiedIdentifierSyntax.ContainsDiagnostics) Then
						modifiedIdentifierSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax)(modifiedIdentifierSyntax, New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("971BE6A7D29BCF799AEF73EBD98A30A9FDD2335AEBF2D305769A66C7D1018485").FieldHandle })
					End If
					separatedSyntaxListBuilder1.Add(modifiedIdentifierSyntax)
					punctuationSyntax = Nothing
					If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax, False, ScannerState.VB)) Then
						Exit While
					End If
					separatedSyntaxListBuilder1.AddSeparator(punctuationSyntax)
				End While
				Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax) = separatedSyntaxListBuilder1.ToList()
				Dim asClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax = Nothing
				Dim equalsValueSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax = Nothing
				Me.ParseFieldOrPropertyAsClauseAndInitializer(False, allowAsNewWith, asClauseSyntax, equalsValueSyntax)
				Dim variableDeclaratorSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax = Me.SyntaxFactory.VariableDeclarator(list, asClauseSyntax, equalsValueSyntax)
				separatedSyntaxListBuilder.Add(variableDeclaratorSyntax)
				punctuationSyntax = Nothing
				If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax, False, ScannerState.VB)) Then
					Exit While
				End If
				separatedSyntaxListBuilder.AddSeparator(punctuationSyntax)
			End While
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax)(separatedSyntaxListBuilder1)
			Dim separatedSyntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax) = separatedSyntaxListBuilder.ToList()
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax)(separatedSyntaxListBuilder)
			Return separatedSyntaxList
		End Function

		Private Function ParseVariableList() As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)()
			While True
				separatedSyntaxListBuilder.Add(Me.ParseVariable())
				Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
				If (Not Me.TryGetTokenAndEatNewLine(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax, False, ScannerState.VB)) Then
					Exit While
				End If
				separatedSyntaxListBuilder.AddSeparator(punctuationSyntax)
			End While
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) = separatedSyntaxListBuilder.ToList()
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(separatedSyntaxListBuilder)
			Return list
		End Function

		Private Function ParseWarningDirective(ByVal hashToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax
			Dim currentToken As IdentifierTokenSyntax = DirectCast(Me.CurrentToken, IdentifierTokenSyntax)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Me._scanner.MakeKeyword(currentToken)
			Me.GetNextToken(ScannerState.VB)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Me.TryGetContextualKeyword(SyntaxKind.WarningKeyword, keywordSyntax1, True)
			If (keywordSyntax1.ContainsDiagnostics) Then
				keywordSyntax1 = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(keywordSyntax1)
			End If
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)()
			If (Not SyntaxFacts.IsTerminator(Me.CurrentToken.Kind)) Then
				While True
					Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax = Me.SyntaxFactory.IdentifierName(Me.ParseIdentifier())
					If (identifierNameSyntax.ContainsDiagnostics) Then
						identifierNameSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)(identifierNameSyntax, New SyntaxKind() { SyntaxKind.CommaToken })
					ElseIf (identifierNameSyntax.Identifier.TypeCharacter <> TypeCharacter.None) Then
						identifierNameSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)(identifierNameSyntax, ERRID.ERR_TypecharNotallowed)
					End If
					separatedSyntaxListBuilder.Add(identifierNameSyntax)
					Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
					If (Not Me.TryGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.CommaToken, punctuationSyntax)) Then
						If (SyntaxFacts.IsTerminator(Me.CurrentToken.Kind)) Then
							Exit While
						End If
						punctuationSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.CommaToken)
						punctuationSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(punctuationSyntax, ERRID.ERR_ExpectedComma)
					End If
					If (punctuationSyntax.ContainsDiagnostics) Then
						punctuationSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(punctuationSyntax)
					End If
					separatedSyntaxListBuilder.AddSeparator(punctuationSyntax)
				End While
			End If
			Dim directiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax = Nothing
			If (keywordSyntax.Kind = SyntaxKind.EnableKeyword) Then
				directiveTriviaSyntax = Me.SyntaxFactory.EnableWarningDirectiveTrivia(hashToken, keywordSyntax, keywordSyntax1, separatedSyntaxListBuilder.ToList())
			ElseIf (keywordSyntax.Kind = SyntaxKind.DisableKeyword) Then
				directiveTriviaSyntax = Me.SyntaxFactory.DisableWarningDirectiveTrivia(hashToken, keywordSyntax, keywordSyntax1, separatedSyntaxListBuilder.ToList())
			End If
			If (directiveTriviaSyntax IsNot Nothing) Then
				directiveTriviaSyntax = Me.CheckFeatureAvailability(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax)(Feature.WarningDirectives, directiveTriviaSyntax)
			End If
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)(separatedSyntaxListBuilder)
			Return directiveTriviaSyntax
		End Function

		Private Function ParseWithStackGuard(Of TNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal parseFunc As Func(Of TNode), ByVal defaultFunc As Func(Of TNode)) As TNode
			Dim tNode1 As TNode
			Dim restorePoint As Scanner.RestorePoint = Me._scanner.CreateRestorePoint()
			Try
				tNode1 = parseFunc()
			Catch insufficientExecutionStackException As System.InsufficientExecutionStackException
				ProjectData.SetProjectError(insufficientExecutionStackException)
				tNode1 = Me.CreateForInsufficientStack(Of TNode)(restorePoint, defaultFunc())
				ProjectData.ClearProjectError()
			End Try
			Return tNode1
		End Function

		Friend Function ParseXmlAttribute(ByVal requireLeadingWhitespace As Boolean, ByVal AllowNameAsExpression As Boolean, ByVal xmlElementName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
			Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
			Dim xmlNodeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax = Nothing
			If (Me.CurrentToken.Kind = SyntaxKind.XmlNameToken OrElse AllowNameAsExpression AndAlso Me.CurrentToken.Kind = SyntaxKind.LessThanPercentEqualsToken OrElse Me.CurrentToken.Kind = SyntaxKind.EqualsToken OrElse Me.CurrentToken.Kind = SyntaxKind.SingleQuoteToken OrElse Me.CurrentToken.Kind = SyntaxKind.DoubleQuoteToken) Then
				Dim xmlNodeSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax = Me.ParseXmlQualifiedName(requireLeadingWhitespace, True, ScannerState.Element, ScannerState.Element)
				If (Me.CurrentToken.Kind = SyntaxKind.EqualsToken) Then
					Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
					Me.GetNextToken(ScannerState.Element)
					Dim xmlNodeSyntax3 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax = Nothing
					If (Me.CurrentToken.Kind = SyntaxKind.LessThanPercentEqualsToken) Then
						xmlNodeSyntax3 = Me.ParseXmlEmbedded(ScannerState.Element)
						xmlNodeSyntax1 = Me.SyntaxFactory.XmlAttribute(xmlNodeSyntax2, currentToken, xmlNodeSyntax3)
					ElseIf (Not Me._scanner.IsScanningXmlDoc OrElse Not Me.TryParseXmlCrefAttributeValue(xmlNodeSyntax2, currentToken, xmlNodeSyntax1) AndAlso Not Me.TryParseXmlNameAttributeValue(xmlNodeSyntax2, currentToken, xmlNodeSyntax1, xmlElementName)) Then
						xmlNodeSyntax3 = Me.ParseXmlString(ScannerState.Element)
						xmlNodeSyntax1 = Me.SyntaxFactory.XmlAttribute(xmlNodeSyntax2, currentToken, xmlNodeSyntax3)
					End If
				ElseIf (xmlNodeSyntax2.Kind <> SyntaxKind.XmlEmbeddedExpression) Then
					If (Me.CurrentToken.Kind = SyntaxKind.SingleQuoteToken OrElse Me.CurrentToken.Kind = SyntaxKind.DoubleQuoteToken) Then
						xmlNodeSyntax = Me.ParseXmlString(ScannerState.Element)
					Else
						Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.SingleQuoteToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
						xmlNodeSyntax = Me.SyntaxFactory.XmlString(punctuationSyntax, New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)(), punctuationSyntax)
					End If
					xmlNodeSyntax1 = Me.SyntaxFactory.XmlAttribute(xmlNodeSyntax2, DirectCast(Parser.HandleUnexpectedToken(SyntaxKind.EqualsToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax), xmlNodeSyntax)
				Else
					xmlNodeSyntax1 = xmlNodeSyntax2
				End If
			End If
			Return xmlNodeSyntax1
		End Function

		Private Function ParseXmlAttributes(ByVal requireLeadingWhitespace As Boolean, ByVal xmlElementName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)
			Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) = Me._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)()
			While True
				Dim kind As SyntaxKind = Me.CurrentToken.Kind
				If (kind <= SyntaxKind.EqualsToken) Then
					If (kind <> SyntaxKind.SingleQuoteToken AndAlso kind <> SyntaxKind.EqualsToken) Then
						Exit While
					End If
				ElseIf (kind <> SyntaxKind.DoubleQuoteToken AndAlso kind <> SyntaxKind.LessThanPercentEqualsToken AndAlso kind <> SyntaxKind.XmlNameToken) Then
					Exit While
				End If
				Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax = Me.ParseXmlAttribute(requireLeadingWhitespace, True, xmlElementName)
				requireLeadingWhitespace = Not xmlNodeSyntax.HasTrailingTrivia
				syntaxListBuilder.Add(xmlNodeSyntax)
			End While
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) = syntaxListBuilder.ToList()
			Me._pool.Free(syntaxListBuilder)
			Return list
		End Function

		Private Function ParseXmlCData(ByVal nextState As ScannerState) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCDataSectionSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			Me.GetNextToken(ScannerState.CData)
			Dim syntaxListBuilder As SyntaxListBuilder(Of XmlTextTokenSyntax) = Me._pool.Allocate(Of XmlTextTokenSyntax)()
			While Me.CurrentToken.Kind = SyntaxKind.XmlTextLiteralToken OrElse Me.CurrentToken.Kind = SyntaxKind.DocumentationCommentLineBreakToken
				syntaxListBuilder.Add(DirectCast(Me.CurrentToken, XmlTextTokenSyntax))
				Me.GetNextToken(ScannerState.CData)
			End While
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.EndCDataToken, punctuationSyntax, nextState)
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of XmlTextTokenSyntax) = syntaxListBuilder.ToList()
			Me._pool.Free(syntaxListBuilder)
			Return Me.SyntaxFactory.XmlCDataSection(currentToken, list, punctuationSyntax)
		End Function

		Private Function ParseXmlComment(ByVal nextState As ScannerState) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			Me.GetNextToken(ScannerState.Comment)
			Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax) = Me._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax)()
			While Me.CurrentToken.Kind = SyntaxKind.XmlTextLiteralToken OrElse Me.CurrentToken.Kind = SyntaxKind.DocumentationCommentLineBreakToken
				Dim xmlTextTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax)
				If (xmlTextTokenSyntax.Text.Length = 2 AndAlso EmbeddedOperators.CompareString(xmlTextTokenSyntax.Text, "--", False) = 0) Then
					xmlTextTokenSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax)(xmlTextTokenSyntax, ERRID.ERR_IllegalXmlCommentChar)
				End If
				syntaxListBuilder.Add(xmlTextTokenSyntax)
				Me.GetNextToken(ScannerState.Comment)
			End While
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.MinusMinusGreaterThanToken, punctuationSyntax, nextState)
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax) = syntaxListBuilder.ToList()
			Me._pool.Free(syntaxListBuilder)
			Return Me.SyntaxFactory.XmlComment(currentToken, list, punctuationSyntax)
		End Function

		Friend Function ParseXmlContent(ByVal state As ScannerState) As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)
			Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
			Dim kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)
			Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) = Me._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)()
			Dim xmlWhitespaceChecker As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlWhitespaceChecker = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlWhitespaceChecker()
			While True
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me.CurrentToken.Kind
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanToken) Then
					xmlNodeSyntax = Me.ParseXmlElement(ScannerState.Content)
				Else
					Select Case syntaxKind
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOfFileToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOfXmlToken
							list = syntaxListBuilder.ToList()
							Me._pool.Free(syntaxListBuilder)
							Return list
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SlashGreaterThanToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsValue Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Attribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributeTarget Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueForStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReturnStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StepKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StopKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PlusToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MinusToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanGreaterThanToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanLessThanEqualsToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SlashGreaterThanToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Attribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributesStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueForStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StepKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StringKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PlusToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DotToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanGreaterThanToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanGreaterThanEqualsToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SlashGreaterThanToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MinusMinusGreaterThanToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QuestionGreaterThanToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PercentGreaterThanToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndCDataToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNameToken
						Label1:
							If (state <> ScannerState.Content) Then
								list = syntaxListBuilder.ToList()
								Me._pool.Free(syntaxListBuilder)
								Return list
							End If
							xmlNodeSyntax = Me.ResyncXmlContent()
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanSlashToken
							xmlNodeSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax)(Me.ParseXmlElementEndTag(ScannerState.Content), ERRID.ERR_XmlEndElementNoMatchingStart)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanExclamationMinusMinusToken
							xmlNodeSyntax = Me.ParseXmlComment(ScannerState.Content)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanQuestionToken
							xmlNodeSyntax = Me.ParseXmlProcessingInstruction(ScannerState.Content, xmlWhitespaceChecker)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanPercentEqualsToken
							xmlNodeSyntax = Me.ParseXmlEmbedded(ScannerState.Content)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BeginCDataToken
							xmlNodeSyntax = Me.ParseXmlCData(ScannerState.Content)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BadToken
							If (DirectCast(Me.CurrentToken, BadTokenSyntax).SubKind <> SyntaxSubKind.BeginDocTypeToken) Then
								GoTo Label1
							End If
							Dim greenNode As Microsoft.CodeAnalysis.GreenNode = Me.ParseXmlDocType(ScannerState.Element)
							xmlNodeSyntax = Me.SyntaxFactory.XmlText(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlTextLiteralToken))
							xmlNodeSyntax = xmlNodeSyntax.AddLeadingSyntax(greenNode, ERRID.ERR_DTDNotSupported)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlTextLiteralToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlEntityLiteralToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DocumentationCommentLineBreakToken
							Dim syntaxListBuilder1 As SyntaxListBuilder(Of XmlTextTokenSyntax) = Me._pool.Allocate(Of XmlTextTokenSyntax)()
							Do
								syntaxListBuilder1.Add(DirectCast(Me.CurrentToken, XmlTextTokenSyntax))
								Me.GetNextToken(ScannerState.Content)
								kind = Me.CurrentToken.Kind
							Loop While kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlTextLiteralToken OrElse kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlEntityLiteralToken OrElse kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DocumentationCommentLineBreakToken
							Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of XmlTextTokenSyntax) = syntaxListBuilder1.ToList()
							Me._pool.Free(syntaxListBuilder1)
							xmlNodeSyntax = Me.SyntaxFactory.XmlText(syntaxList)
							Exit Select
						Case Else
							GoTo Label1
					End Select
				End If
				syntaxListBuilder.Add(xmlNodeSyntax)
			End While
			list = syntaxListBuilder.ToList()
			Me._pool.Free(syntaxListBuilder)
			Return list
		End Function

		Private Function ParseXmlDeclaration() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax
			Dim xmlDeclarationOptionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax
			Dim xmlDeclarationOptionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax
			Dim xmlDeclarationOptionSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			Me.GetNextToken(ScannerState.Element)
			Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = Nothing
			Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)(SyntaxKind.XmlNameToken, xmlNameTokenSyntax, ScannerState.Element)
			Dim num As Integer = 0
			Dim num1 As Integer = 0
			Dim flag As Boolean = False
			Dim flag1 As Boolean = False
			Dim flag2 As Boolean = False
			Dim visualBasicSyntaxNodeArray(3) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim num2 As Integer = 0
			visualBasicSyntaxNodeArray(num2) = Me._scanner.MakeKeyword(xmlNameTokenSyntax)
			num2 = num2 + 1
			While True
				Dim kind As SyntaxKind = Me.CurrentToken.Kind
				If (kind = SyntaxKind.LessThanPercentEqualsToken) Then
					xmlDeclarationOptionSyntax = Me.ParseXmlDeclarationOption()
					visualBasicSyntaxNodeArray(num2 - 1) = visualBasicSyntaxNodeArray(num2 - 1).AddTrailingSyntax(xmlDeclarationOptionSyntax)
				Else
					If (kind <> SyntaxKind.XmlNameToken) Then
						Exit While
					End If
					Dim currentToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)
					Dim str As String = currentToken1.ToString()
					If (EmbeddedOperators.CompareString(str, "version", False) = 0) Then
						xmlDeclarationOptionSyntax = Me.ParseXmlDeclarationOption()
						If (Not flag) Then
							flag = True
							If (flag1 OrElse flag2) Then
								xmlDeclarationOptionSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax)(xmlDeclarationOptionSyntax, ERRID.ERR_VersionMustBeFirstInXmlDecl, New [Object]() { "", "", currentToken1.ToString() })
							End If
							If (xmlDeclarationOptionSyntax.Value.TextTokens.Node Is Nothing OrElse EmbeddedOperators.CompareString(xmlDeclarationOptionSyntax.Value.TextTokens.Node.ToFullString(), "1.0", False) <> 0) Then
								xmlDeclarationOptionSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax)(xmlDeclarationOptionSyntax, ERRID.ERR_InvalidAttributeValue1, New [Object]() { "1.0" })
							End If
							visualBasicSyntaxNodeArray(num2) = xmlDeclarationOptionSyntax
							num2 = num2 + 1
						Else
							xmlDeclarationOptionSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax)(xmlDeclarationOptionSyntax, ERRID.ERR_DuplicateXmlAttribute, New [Object]() { currentToken1.ToString() })
							visualBasicSyntaxNodeArray(num2 - 1) = visualBasicSyntaxNodeArray(num2 - 1).AddTrailingSyntax(xmlDeclarationOptionSyntax)
						End If
					ElseIf (EmbeddedOperators.CompareString(str, "encoding", False) = 0) Then
						xmlDeclarationOptionSyntax = Me.ParseXmlDeclarationOption()
						If (Not flag1) Then
							flag1 = True
							If (flag2) Then
								xmlDeclarationOptionSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax)(xmlDeclarationOptionSyntax, ERRID.ERR_AttributeOrder, New [Object]() { "encoding", "standalone" })
								visualBasicSyntaxNodeArray(num2 - 1) = visualBasicSyntaxNodeArray(num2 - 1).AddTrailingSyntax(xmlDeclarationOptionSyntax)
							ElseIf (flag) Then
								num = num2
								visualBasicSyntaxNodeArray(num2) = xmlDeclarationOptionSyntax
								num2 = num2 + 1
							Else
								visualBasicSyntaxNodeArray(num2 - 1) = visualBasicSyntaxNodeArray(num2 - 1).AddTrailingSyntax(xmlDeclarationOptionSyntax)
							End If
						Else
							xmlDeclarationOptionSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax)(xmlDeclarationOptionSyntax, ERRID.ERR_DuplicateXmlAttribute, New [Object]() { currentToken1.ToString() })
							visualBasicSyntaxNodeArray(num2 - 1) = visualBasicSyntaxNodeArray(num2 - 1).AddTrailingSyntax(xmlDeclarationOptionSyntax)
						End If
					ElseIf (EmbeddedOperators.CompareString(str, "standalone", False) = 0) Then
						xmlDeclarationOptionSyntax = Me.ParseXmlDeclarationOption()
						If (Not flag2) Then
							flag2 = True
							If (flag) Then
								Dim str1 As String = If(xmlDeclarationOptionSyntax.Value.TextTokens.Node IsNot Nothing, xmlDeclarationOptionSyntax.Value.TextTokens.Node.ToFullString(), "")
								If (EmbeddedOperators.CompareString(str1, "yes", False) <> 0 AndAlso EmbeddedOperators.CompareString(str1, "no", False) <> 0) Then
									xmlDeclarationOptionSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax)(xmlDeclarationOptionSyntax, ERRID.ERR_InvalidAttributeValue2, New [Object]() { "yes", "no" })
								End If
								num1 = num2
								visualBasicSyntaxNodeArray(num2) = xmlDeclarationOptionSyntax
								num2 = num2 + 1
							Else
								visualBasicSyntaxNodeArray(num2 - 1) = visualBasicSyntaxNodeArray(num2 - 1).AddTrailingSyntax(xmlDeclarationOptionSyntax)
							End If
						Else
							xmlDeclarationOptionSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax)(xmlDeclarationOptionSyntax, ERRID.ERR_DuplicateXmlAttribute, New [Object]() { currentToken1.ToString() })
							visualBasicSyntaxNodeArray(num2 - 1) = visualBasicSyntaxNodeArray(num2 - 1).AddTrailingSyntax(xmlDeclarationOptionSyntax)
						End If
					Else
						xmlDeclarationOptionSyntax = Me.ParseXmlDeclarationOption()
						xmlDeclarationOptionSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax)(xmlDeclarationOptionSyntax, ERRID.ERR_IllegalAttributeInXmlDecl, New [Object]() { "", "", xmlDeclarationOptionSyntax.Name.ToString() })
						visualBasicSyntaxNodeArray(num2 - 1) = visualBasicSyntaxNodeArray(num2 - 1).AddTrailingSyntax(xmlDeclarationOptionSyntax)
					End If
				End If
			End While
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)()
			If (Me.CurrentToken.Kind <> SyntaxKind.QuestionGreaterThanToken) Then
				syntaxList = Me.ResyncAt(ScannerState.Element, New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("B831730E37B9B4785D03788336CEFD8A2D45F51B3A785D7C2F5EC5F7B26B1618").FieldHandle })
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.QuestionGreaterThanToken, punctuationSyntax, ScannerState.Content)
			If (syntaxList.Node IsNot Nothing) Then
				punctuationSyntax = punctuationSyntax.AddLeadingSyntax(syntaxList, ERRID.ERR_ExpectedXmlName)
			End If
			If (visualBasicSyntaxNodeArray(1) Is Nothing) Then
				Dim xmlDeclarationOptionSyntax3 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax = Me.SyntaxFactory.XmlDeclarationOption(DirectCast(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.XmlNameToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.EqualsToken), Me.CreateMissingXmlString())
				visualBasicSyntaxNodeArray(1) = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax)(xmlDeclarationOptionSyntax3, ERRID.ERR_MissingVersionInXmlDecl)
			End If
			Dim syntaxFactory As ContextAwareSyntaxFactory = Me.SyntaxFactory
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = currentToken
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = TryCast(visualBasicSyntaxNodeArray(0), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Dim xmlDeclarationOptionSyntax4 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax = TryCast(visualBasicSyntaxNodeArray(1), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax)
			If (num = 0) Then
				xmlDeclarationOptionSyntax1 = Nothing
			Else
				xmlDeclarationOptionSyntax1 = TryCast(visualBasicSyntaxNodeArray(num), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax)
			End If
			If (num1 = 0) Then
				xmlDeclarationOptionSyntax2 = Nothing
			Else
				xmlDeclarationOptionSyntax2 = TryCast(visualBasicSyntaxNodeArray(num1), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax)
			End If
			Return syntaxFactory.XmlDeclaration(punctuationSyntax1, keywordSyntax, xmlDeclarationOptionSyntax4, xmlDeclarationOptionSyntax1, xmlDeclarationOptionSyntax2, punctuationSyntax)
		End Function

		Private Function ParseXmlDeclarationOption() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax
			Dim flag As Boolean
			Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = Nothing
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Dim xmlStringSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax = Nothing
			flag = If(Me.PrevToken.GetTrailingTrivia().ContainsWhitespaceTrivia(), True, Me.CurrentToken.GetLeadingTrivia().ContainsWhitespaceTrivia())
			Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)(SyntaxKind.XmlNameToken, xmlNameTokenSyntax, ScannerState.Element)
			If (Not flag) Then
				xmlNameTokenSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)(xmlNameTokenSyntax, ERRID.ERR_ExpectedXmlWhiteSpace)
			End If
			If (Me.CurrentToken.Kind = SyntaxKind.LessThanPercentEqualsToken) Then
				Dim xmlEmbeddedExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmbeddedExpressionSyntax = Me.ParseXmlEmbedded(ScannerState.Element)
				xmlNameTokenSyntax = xmlNameTokenSyntax.AddTrailingSyntax(xmlEmbeddedExpressionSyntax, ERRID.ERR_EmbeddedExpression)
			End If
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)()
			If (Not Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.EqualsToken, punctuationSyntax, ScannerState.Element)) Then
				syntaxList = Me.ResyncAt(ScannerState.Element, New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("748BA153C06C5F9029EE9AC2B18BBBC865C4F050035892DD502A2C6C0978F010").FieldHandle })
				punctuationSyntax = punctuationSyntax.AddTrailingSyntax(syntaxList)
			End If
			Dim kind As SyntaxKind = Me.CurrentToken.Kind
			If (kind = SyntaxKind.SingleQuoteToken OrElse kind = SyntaxKind.DoubleQuoteToken) Then
				xmlStringSyntax = Me.ParseXmlString(ScannerState.Element)
			ElseIf (kind = SyntaxKind.LessThanPercentEqualsToken) Then
				Dim xmlEmbeddedExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmbeddedExpressionSyntax = Me.ParseXmlEmbedded(ScannerState.Element)
				xmlStringSyntax = Me.CreateMissingXmlString().AddLeadingSyntax(xmlEmbeddedExpressionSyntax1, ERRID.ERR_EmbeddedExpression)
			Else
				xmlStringSyntax = Me.CreateMissingXmlString()
			End If
			Return Me.SyntaxFactory.XmlDeclarationOption(xmlNameTokenSyntax, punctuationSyntax, xmlStringSyntax)
		End Function

		Private Function ParseXmlDocType(ByVal enclosingState As ScannerState) As GreenNode
			Dim syntaxListBuilder As SyntaxListBuilder(Of GreenNode) = SyntaxListBuilder(Of GreenNode).Create()
			syntaxListBuilder.Add(DirectCast(Me.CurrentToken, BadTokenSyntax))
			Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = Nothing
			Me.GetNextToken(ScannerState.DocType)
			Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)(SyntaxKind.XmlNameToken, xmlNameTokenSyntax, ScannerState.DocType)
			syntaxListBuilder.Add(xmlNameTokenSyntax)
			Me.ParseExternalID(syntaxListBuilder)
			Me.ParseInternalSubSet(syntaxListBuilder)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.GreaterThanToken, punctuationSyntax, enclosingState)
			syntaxListBuilder.Add(punctuationSyntax)
			Return syntaxListBuilder.ToList().Node
		End Function

		Private Function ParseXmlDocument() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
			Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
			Dim xmlNodeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
			Dim xmlWhitespaceChecker As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlWhitespaceChecker = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlWhitespaceChecker()
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.PeekNextToken(ScannerState.Element)
			If (syntaxToken.Kind <> SyntaxKind.XmlNameToken OrElse DirectCast(syntaxToken, XmlNameTokenSyntax).PossibleKeywordKind <> SyntaxKind.XmlKeyword) Then
				xmlNodeSyntax = Me.ParseXmlProcessingInstruction(ScannerState.VB, xmlWhitespaceChecker)
			Else
				Dim xmlDeclarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax = Me.ParseXmlDeclaration()
				xmlDeclarationSyntax = DirectCast(xmlWhitespaceChecker.Visit(xmlDeclarationSyntax), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax)
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = xmlDeclarationSyntax
				Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) = Me.ParseXmlMisc(True, xmlWhitespaceChecker, visualBasicSyntaxNode)
				xmlDeclarationSyntax = DirectCast(visualBasicSyntaxNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax)
				Dim kind As SyntaxKind = Me.CurrentToken.Kind
				If (kind = SyntaxKind.LessThanToken) Then
					xmlNodeSyntax1 = Me.ParseXmlElement(ScannerState.Misc)
				ElseIf (kind = SyntaxKind.LessThanPercentEqualsToken) Then
					xmlNodeSyntax1 = Me.ParseXmlEmbedded(ScannerState.Misc)
				Else
					Dim syntaxFactory As ContextAwareSyntaxFactory = Me.SyntaxFactory
					Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Parser.HandleUnexpectedToken(SyntaxKind.LessThanToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
					Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax = Me.SyntaxFactory.XmlName(Nothing, DirectCast(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.XmlNameToken), XmlNameTokenSyntax))
					Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)()
					xmlNodeSyntax1 = syntaxFactory.XmlEmptyElement(punctuationSyntax, xmlNameSyntax, syntaxList1, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.SlashGreaterThanToken))
				End If
				visualBasicSyntaxNode = xmlNodeSyntax1
				Dim syntaxList2 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) = Me.ParseXmlMisc(False, xmlWhitespaceChecker, visualBasicSyntaxNode)
				xmlNodeSyntax1 = DirectCast(visualBasicSyntaxNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)
				xmlNodeSyntax = Me.SyntaxFactory.XmlDocument(xmlDeclarationSyntax, syntaxList, xmlNodeSyntax1, syntaxList2)
			End If
			Return xmlNodeSyntax
		End Function

		Private Function ParseXmlElement(ByVal enclosingState As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
			Dim xmlElementEndTagSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax
			Dim kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax = Nothing
			Dim xmlContexts As List(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlContext) = New List(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlContext)()
			Dim scannerState As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState = enclosingState
			Dim xmlWhitespaceChecker As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlWhitespaceChecker = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlWhitespaceChecker()
			While True
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me.CurrentToken.Kind
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanToken) Then
					If (Me.PeekNextToken(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState.Element).Kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SlashToken) Then
						GoTo Label0
					End If
					xmlNodeSyntax = Me.ParseXmlElementStartTag(scannerState)
					xmlNodeSyntax = DirectCast(xmlWhitespaceChecker.Visit(xmlNodeSyntax), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)
					If (xmlNodeSyntax.Kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlElementStartTag) Then
						GoTo Label2
					End If
					Dim xmlElementStartTagSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax = DirectCast(xmlNodeSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax)
					xmlContexts.Push(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlContext(Me._pool, xmlElementStartTagSyntax))
					scannerState = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState.Content
					Continue While
				Else
					Select Case syntaxKind
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanSlashToken
							GoTo Label0
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanExclamationMinusMinusToken
							xmlNodeSyntax = Me.ParseXmlComment(scannerState)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MinusMinusGreaterThanToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QuestionGreaterThanToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PercentGreaterThanToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndCDataToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOfXmlToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNameToken
							If (xmlContexts.Count > 0) Then
								While True
									xmlElementEndTagSyntax = Me.ParseXmlElementEndTag(scannerState)
									xmlNodeSyntax = Me.CreateXmlElement(xmlContexts, xmlElementEndTagSyntax)
									If (xmlContexts.Count <= 0) Then
										Exit While
									End If
									xmlContexts.Peek(0).Add(xmlNodeSyntax)
								End While
							End If
							Me.ResetCurrentToken(enclosingState)
							Return xmlNodeSyntax
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanQuestionToken
							xmlNodeSyntax = Me.ParseXmlProcessingInstruction(scannerState, xmlWhitespaceChecker)
							xmlNodeSyntax = DirectCast(xmlWhitespaceChecker.Visit(xmlNodeSyntax), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanPercentEqualsToken
							xmlNodeSyntax = Me.ParseXmlEmbedded(scannerState)
							If (xmlContexts.Count <> 0) Then
								Exit Select
							End If
							xmlNodeSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)(xmlNodeSyntax, ERRID.ERR_EmbeddedExpression)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BeginCDataToken
							xmlNodeSyntax = Me.ParseXmlCData(scannerState)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BadToken
							If (DirectCast(Me.CurrentToken, BadTokenSyntax).SubKind <> SyntaxSubKind.BeginDocTypeToken) Then
								If (xmlContexts.Count > 0) Then
									While True
										xmlElementEndTagSyntax = Me.ParseXmlElementEndTag(scannerState)
										xmlNodeSyntax = Me.CreateXmlElement(xmlContexts, xmlElementEndTagSyntax)
										If (xmlContexts.Count <= 0) Then
											Exit While
										End If
										xmlContexts.Peek(0).Add(xmlNodeSyntax)
									End While
								End If
								Me.ResetCurrentToken(enclosingState)
								Return xmlNodeSyntax
							End If
							Dim greenNode As Microsoft.CodeAnalysis.GreenNode = Me.ParseXmlDocType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState.Element)
							xmlNodeSyntax = Me.SyntaxFactory.XmlText(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlTextLiteralToken))
							xmlNodeSyntax = xmlNodeSyntax.AddLeadingSyntax(greenNode, ERRID.ERR_DTDNotSupported)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlTextLiteralToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlEntityLiteralToken
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DocumentationCommentLineBreakToken
							Dim syntaxListBuilder As SyntaxListBuilder(Of XmlTextTokenSyntax) = Me._pool.Allocate(Of XmlTextTokenSyntax)()
							Do
								syntaxListBuilder.Add(DirectCast(Me.CurrentToken, XmlTextTokenSyntax))
								Me.GetNextToken(scannerState)
								kind = Me.CurrentToken.Kind
							Loop While kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlTextLiteralToken OrElse kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlEntityLiteralToken OrElse kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DocumentationCommentLineBreakToken
							Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of XmlTextTokenSyntax) = syntaxListBuilder.ToList()
							Me._pool.Free(syntaxListBuilder)
							xmlNodeSyntax = Me.SyntaxFactory.XmlText(list)
							Exit Select
						Case Else
							If (xmlContexts.Count > 0) Then
								While True
									xmlElementEndTagSyntax = Me.ParseXmlElementEndTag(scannerState)
									xmlNodeSyntax = Me.CreateXmlElement(xmlContexts, xmlElementEndTagSyntax)
									If (xmlContexts.Count <= 0) Then
										Exit While
									End If
									xmlContexts.Peek(0).Add(xmlNodeSyntax)
								End While
							End If
							Me.ResetCurrentToken(enclosingState)
							Return xmlNodeSyntax
					End Select
				End If
			Label2:
				If (xmlContexts.Count <= 0) Then
					Exit While
				End If
				xmlContexts.Peek(0).Add(xmlNodeSyntax)
			End While
			If (xmlContexts.Count > 0) Then
				While True
					xmlElementEndTagSyntax = Me.ParseXmlElementEndTag(scannerState)
					xmlNodeSyntax = Me.CreateXmlElement(xmlContexts, xmlElementEndTagSyntax)
					If (xmlContexts.Count <= 0) Then
						Exit While
					End If
					xmlContexts.Peek(0).Add(xmlNodeSyntax)
				End While
			End If
			Me.ResetCurrentToken(enclosingState)
			Return xmlNodeSyntax
		Label0:
			xmlElementEndTagSyntax = Me.ParseXmlElementEndTag(scannerState)
			xmlElementEndTagSyntax = DirectCast(xmlWhitespaceChecker.Visit(xmlElementEndTagSyntax), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax)
			If (xmlContexts.Count <= 0) Then
				Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanToken)
				Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = DirectCast(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNameToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)
				Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax = Me.SyntaxFactory.XmlName(Nothing, xmlNameTokenSyntax)
				Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanToken)
				Dim syntaxFactory As ContextAwareSyntaxFactory = Me.SyntaxFactory
				Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.GreenNode)()
				Dim xmlElementStartTagSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax = syntaxFactory.XmlElementStartTag(punctuationSyntax, xmlNameSyntax, syntaxList, punctuationSyntax1)
				xmlContexts.Push(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlContext(Me._pool, xmlElementStartTagSyntax1))
				Dim xmlContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlContext = xmlContexts.Peek(0)
				xmlNodeSyntax = xmlContext.CreateElement(xmlElementEndTagSyntax)
				xmlNodeSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)(xmlNodeSyntax, ERRID.ERR_XmlEndElementNoMatchingStart)
				xmlContexts.Pop()
				GoTo Label2
			Else
				xmlNodeSyntax = Me.CreateXmlElement(xmlContexts, xmlElementEndTagSyntax)
				GoTo Label2
			End If
		End Function

		Private Function ParseXmlElementEndTag(ByVal nextState As ScannerState) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax = Nothing
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)()
			If (Me.CurrentToken.Kind <> SyntaxKind.LessThanSlashToken) Then
				syntaxList = Me.ResyncAt(ScannerState.Content, New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("FFCDD6D0AD034CE36D8568C73BE4F7C3BB1A8D3810E395A15BB9FF2A53355743").FieldHandle })
			End If
			If (Not Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.LessThanSlashToken, punctuationSyntax, ScannerState.EndElement) AndAlso Me.CurrentToken.Kind = SyntaxKind.LessThanToken) Then
				Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
				Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.PeekNextToken(ScannerState.EndElement)
				If (syntaxToken.Kind = SyntaxKind.SlashToken) Then
					punctuationSyntax = If(Not (currentToken.HasTrailingTrivia Or syntaxToken.HasLeadingTrivia), DirectCast(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.Token(currentToken.GetLeadingTrivia(), SyntaxKind.LessThanSlashToken, syntaxToken.GetTrailingTrivia(), [String].Concat(currentToken.Text, syntaxToken.Text)), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax), punctuationSyntax.AddLeadingSyntax(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList.List(currentToken, syntaxToken), ERRID.ERR_IllegalXmlWhiteSpace))
					Me.GetNextToken(ScannerState.EndElement)
					Me.GetNextToken(ScannerState.EndElement)
				End If
			End If
			If (syntaxList.Node IsNot Nothing) Then
				punctuationSyntax = If(Not syntaxList.Node.ContainsDiagnostics, punctuationSyntax.AddLeadingSyntax(syntaxList, ERRID.ERR_ExpectedLT), punctuationSyntax.AddLeadingSyntax(syntaxList))
			End If
			If (Me.CurrentToken.Kind = SyntaxKind.XmlNameToken) Then
				xmlNameSyntax = DirectCast(Me.ParseXmlQualifiedName(False, False, ScannerState.EndElement, ScannerState.EndElement), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax)
			End If
			Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.GreaterThanToken, punctuationSyntax1, nextState)
			Return Me.SyntaxFactory.XmlElementEndTag(punctuationSyntax, xmlNameSyntax, punctuationSyntax1)
		End Function

		Private Function ParseXmlElementStartTag(ByVal enclosingState As ScannerState) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
			Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			Me.GetNextToken(ScannerState.Element)
			Dim xmlNodeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax = Me.ParseXmlQualifiedName(False, True, ScannerState.Element, ScannerState.Element)
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) = Me.ParseXmlAttributes(Not xmlNodeSyntax1.HasTrailingTrivia, xmlNodeSyntax1)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Dim currentToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Dim kind As SyntaxKind = Me.CurrentToken.Kind
			If (kind = SyntaxKind.SlashToken) Then
				If (Me.PeekNextToken(ScannerState.Element).Kind <> SyntaxKind.GreaterThanToken) Then
					xmlNodeSyntax = Me.ResyncXmlElement(enclosingState, currentToken, xmlNodeSyntax1, syntaxList)
				Else
					Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.CurrentToken
					Me.GetNextToken(ScannerState.Element)
					punctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
					Me.GetNextToken(enclosingState)
					Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList.List(syntaxToken, punctuationSyntax))
					currentToken1 = (New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax(SyntaxKind.SlashGreaterThanToken, "", Nothing, Nothing)).AddLeadingSyntax(syntaxList1, ERRID.ERR_IllegalXmlWhiteSpace)
					xmlNodeSyntax = Me.SyntaxFactory.XmlEmptyElement(currentToken, xmlNodeSyntax1, syntaxList, currentToken1)
				End If
			ElseIf (kind = SyntaxKind.GreaterThanToken) Then
				punctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
				Me.GetNextToken(ScannerState.Content)
				xmlNodeSyntax = Me.SyntaxFactory.XmlElementStartTag(currentToken, xmlNodeSyntax1, syntaxList, punctuationSyntax)
			ElseIf (kind = SyntaxKind.SlashGreaterThanToken) Then
				currentToken1 = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
				Me.GetNextToken(enclosingState)
				xmlNodeSyntax = Me.SyntaxFactory.XmlEmptyElement(currentToken, xmlNodeSyntax1, syntaxList, currentToken1)
			Else
				xmlNodeSyntax = Me.ResyncXmlElement(enclosingState, currentToken, xmlNodeSyntax1, syntaxList)
			End If
			Return xmlNodeSyntax
		End Function

		Private Function ParseXmlEmbedded(ByVal enclosingState As ScannerState) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmbeddedExpressionSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			Me.GetNextToken(enclosingState)
			currentToken = Me.TransitionFromXmlToVB(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(currentToken)
			Me.TryEatNewLine(ScannerState.VB)
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			If (Not Me.TryEatNewLineAndGetToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.PercentGreaterThanToken, punctuationSyntax, False, enclosingState)) Then
				Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = Me._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)()
				Me.ResyncAt(syntaxListBuilder, ScannerState.VB, New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("768B15A40BADF92A63317E1EC58B2A4DC25BBCAB7B3FEEE26DE2904A93E00852").FieldHandle })
				If (Me.CurrentToken.Kind <> SyntaxKind.PercentGreaterThanToken) Then
					punctuationSyntax = DirectCast(Parser.HandleUnexpectedToken(SyntaxKind.PercentGreaterThanToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
				Else
					punctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
					Me.GetNextToken(enclosingState)
				End If
				Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = syntaxListBuilder.ToList()
				Me._pool.Free(syntaxListBuilder)
				If (list.Node IsNot Nothing) Then
					punctuationSyntax = punctuationSyntax.AddLeadingSyntax(list, ERRID.ERR_Syntax)
				End If
			End If
			Dim xmlEmbeddedExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmbeddedExpressionSyntax = Me.SyntaxFactory.XmlEmbeddedExpression(currentToken, expressionSyntax, punctuationSyntax)
			Return Me.TransitionFromVBToXml(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmbeddedExpressionSyntax)(enclosingState, Parser.AdjustTriviaForMissingTokens(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmbeddedExpressionSyntax)(xmlEmbeddedExpressionSyntax))
		End Function

		Private Function ParseXmlExpression() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
			Me.ResetCurrentToken(ScannerState.Content)
			Dim vB As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax = Nothing
			vB = If(Me.CurrentToken.Kind <> SyntaxKind.LessThanQuestionToken, Me.ParseXmlElement(ScannerState.VB), Me.ParseXmlDocument())
			vB = Parser.AdjustTriviaForMissingTokens(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)(vB)
			vB = Me.TransitionFromXmlToVB(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)(vB)
			Return vB
		End Function

		Private Sub ParseXmlMarkupDecl(ByVal builder As SyntaxListBuilder(Of GreenNode))
			While True
				Dim kind As SyntaxKind = Me.CurrentToken.Kind
				If (kind <= SyntaxKind.LessThanExclamationMinusMinusToken) Then
					If (kind = SyntaxKind.GreaterThanToken) Then
						builder.Add(Me.CurrentToken)
						Me.GetNextToken(ScannerState.DocType)
						Return
					End If
					If (kind = SyntaxKind.EndOfFileToken) Then
						Exit While
					End If
					If (kind = SyntaxKind.LessThanExclamationMinusMinusToken) Then
						builder.Add(Me.ParseXmlComment(ScannerState.DocType))
						Continue While
					End If
				ElseIf (kind = SyntaxKind.LessThanQuestionToken) Then
					Dim xmlProcessingInstructionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax = Me.ParseXmlProcessingInstruction(ScannerState.DocType, Nothing)
					builder.Add(xmlProcessingInstructionSyntax)
					Continue While
				Else
					If (kind = SyntaxKind.EndOfXmlToken) Then
						Exit While
					End If
					If (kind <> SyntaxKind.BadToken) Then
						GoTo Label0
					End If
					builder.Add(Me.CurrentToken)
					Dim currentToken As BadTokenSyntax = DirectCast(Me.CurrentToken, BadTokenSyntax)
					Me.GetNextToken(ScannerState.DocType)
					If (currentToken.SubKind = SyntaxSubKind.LessThanExclamationToken) Then
						Me.ParseXmlMarkupDecl(builder)
						Continue While
					End If
				End If
			Label0:
				builder.Add(Me.CurrentToken)
				Me.GetNextToken(ScannerState.DocType)
			End While
		End Sub

		Private Function ParseXmlMisc(ByVal IsProlog As Boolean, ByVal whitespaceChecker As XmlWhitespaceChecker, ByRef outerNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) = Me._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)()
			While True
				Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax = Nothing
				Dim kind As SyntaxKind = Me.CurrentToken.Kind
				If (kind = SyntaxKind.LessThanExclamationMinusMinusToken) Then
					xmlNodeSyntax = Me.ParseXmlComment(ScannerState.Misc)
				ElseIf (kind = SyntaxKind.LessThanQuestionToken) Then
					xmlNodeSyntax = Me.ParseXmlProcessingInstruction(ScannerState.Misc, whitespaceChecker)
				Else
					If (kind <> SyntaxKind.BadToken) Then
						Exit While
					End If
					Dim currentToken As BadTokenSyntax = DirectCast(Me.CurrentToken, BadTokenSyntax)
					If (currentToken.SubKind <> SyntaxSubKind.BeginDocTypeToken) Then
						greenNode = currentToken
						Me.GetNextToken(ScannerState.Misc)
					Else
						greenNode = Me.ParseXmlDocType(ScannerState.Misc)
					End If
					Dim count As Integer = syntaxListBuilder.Count
					If (count <= 0) Then
						outerNode = outerNode.AddTrailingSyntax(greenNode, ERRID.ERR_DTDNotSupported)
					Else
						syntaxListBuilder(count - 1) = syntaxListBuilder(count - 1).AddTrailingSyntax(greenNode, ERRID.ERR_DTDNotSupported)
					End If
				End If
				If (xmlNodeSyntax IsNot Nothing) Then
					syntaxListBuilder.Add(xmlNodeSyntax)
				End If
			End While
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) = syntaxListBuilder.ToList()
			Me._pool.Free(syntaxListBuilder)
			Return list
		End Function

		Private Function ParseXmlProcessingInstruction(ByVal nextState As ScannerState, ByVal whitespaceChecker As XmlWhitespaceChecker) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			Me.GetNextToken(ScannerState.Element)
			Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = Nothing
			If (Not Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)(SyntaxKind.XmlNameToken, xmlNameTokenSyntax, ScannerState.StartProcessingInstruction)) Then
				Me.ResetCurrentToken(ScannerState.StartProcessingInstruction)
			End If
			If (xmlNameTokenSyntax.Text.Length = 3 AndAlso [String].Equals(xmlNameTokenSyntax.Text, "xml", StringComparison.OrdinalIgnoreCase)) Then
				xmlNameTokenSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)(xmlNameTokenSyntax, ERRID.ERR_IllegalProcessingInstructionName, New [Object]() { xmlNameTokenSyntax.Text })
			End If
			Dim xmlTextTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax = Nothing
			Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax) = Me._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax)()
			If (Me.CurrentToken.Kind = SyntaxKind.XmlTextLiteralToken OrElse Me.CurrentToken.Kind = SyntaxKind.DocumentationCommentLineBreakToken) Then
				xmlTextTokenSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax)
				If (Not xmlNameTokenSyntax.IsMissing AndAlso Not xmlNameTokenSyntax.GetTrailingTrivia().ContainsWhitespaceTrivia() AndAlso Not xmlTextTokenSyntax.GetLeadingTrivia().ContainsWhitespaceTrivia()) Then
					xmlTextTokenSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax)(xmlTextTokenSyntax, ERRID.ERR_ExpectedXmlWhiteSpace)
				End If
				While True
					syntaxListBuilder.Add(xmlTextTokenSyntax)
					Me.GetNextToken(ScannerState.ProcessingInstruction)
					If (Me.CurrentToken.Kind <> SyntaxKind.XmlTextLiteralToken AndAlso Me.CurrentToken.Kind <> SyntaxKind.DocumentationCommentLineBreakToken) Then
						Exit While
					End If
					xmlTextTokenSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextTokenSyntax)
				End While
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Nothing
			Me.VerifyExpectedToken(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(SyntaxKind.QuestionGreaterThanToken, punctuationSyntax, nextState)
			Dim xmlProcessingInstructionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax = Me.SyntaxFactory.XmlProcessingInstruction(currentToken, xmlNameTokenSyntax, syntaxListBuilder.ToList(), punctuationSyntax)
			xmlProcessingInstructionSyntax = DirectCast(whitespaceChecker.Visit(xmlProcessingInstructionSyntax), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax)
			Me._pool.Free(syntaxListBuilder)
			Return xmlProcessingInstructionSyntax
		End Function

		Private Function ParseXmlQualifiedName(ByVal requireLeadingWhitespace As Boolean, ByVal allowExpr As Boolean, ByVal stateForName As ScannerState, ByVal nextState As ScannerState) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
			Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
			Dim kind As SyntaxKind = Me.CurrentToken.Kind
			If (kind = SyntaxKind.LessThanPercentEqualsToken) Then
				If (Not allowExpr) Then
					Me.ResetCurrentToken(nextState)
					xmlNodeSyntax = Me.ReportExpectedXmlName()
					Return xmlNodeSyntax
				End If
				xmlNodeSyntax = Me.ParseXmlEmbedded(nextState)
				Return xmlNodeSyntax
			Else
				If (kind <> SyntaxKind.XmlNameToken) Then
					Me.ResetCurrentToken(nextState)
					xmlNodeSyntax = Me.ReportExpectedXmlName()
					Return xmlNodeSyntax
				End If
				xmlNodeSyntax = Me.ParseXmlQualifiedName(requireLeadingWhitespace, stateForName, nextState)
				Return xmlNodeSyntax
			End If
			Me.ResetCurrentToken(nextState)
			xmlNodeSyntax = Me.ReportExpectedXmlName()
			Return xmlNodeSyntax
		End Function

		Private Function ParseXmlQualifiedName(ByVal requireLeadingWhitespace As Boolean, ByVal stateForName As ScannerState, ByVal nextState As ScannerState) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
			Dim flag As Boolean
			If (Not requireLeadingWhitespace) Then
				flag = False
			Else
				flag = If(Me.PrevToken.GetTrailingTrivia().ContainsWhitespaceTrivia(), True, Me.CurrentToken.GetLeadingTrivia().ContainsWhitespaceTrivia())
			End If
			Dim flag1 As Boolean = flag
			Dim currentToken As XmlNameTokenSyntax = DirectCast(Me.CurrentToken, XmlNameTokenSyntax)
			Me.GetNextToken(stateForName)
			If (requireLeadingWhitespace AndAlso Not flag1) Then
				currentToken = Parser.ReportSyntaxError(Of XmlNameTokenSyntax)(currentToken, ERRID.ERR_ExpectedXmlWhiteSpace)
			End If
			Dim xmlPrefixSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax = Nothing
			If (Me.CurrentToken.Kind = SyntaxKind.ColonToken) Then
				Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
				Me.GetNextToken(stateForName)
				xmlPrefixSyntax = Me.SyntaxFactory.XmlPrefix(currentToken, punctuationSyntax)
				If (Me.CurrentToken.Kind <> SyntaxKind.XmlNameToken) Then
					currentToken = Parser.ReportSyntaxError(Of XmlNameTokenSyntax)(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlNameToken("", SyntaxKind.XmlNameToken, Nothing, Nothing), ERRID.ERR_ExpectedXmlName)
				Else
					currentToken = DirectCast(Me.CurrentToken, XmlNameTokenSyntax)
					Me.GetNextToken(stateForName)
					If (punctuationSyntax.HasTrailingTrivia OrElse currentToken.HasLeadingTrivia) Then
						currentToken = Parser.ReportSyntaxError(Of XmlNameTokenSyntax)(currentToken, ERRID.ERR_ExpectedXmlName)
					End If
				End If
			End If
			Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax = Me.SyntaxFactory.XmlName(xmlPrefixSyntax, currentToken)
			Me.ResetCurrentToken(nextState)
			Return xmlNameSyntax
		End Function

		Private Function ParseXmlQualifiedNameVB() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax
			Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax
			If (Parser.IsValidXmlQualifiedNameToken(Me.CurrentToken)) Then
				Dim xmlNameToken As XmlNameTokenSyntax = Me.ToXmlNameToken(Me.CurrentToken)
				Me.GetNextToken(ScannerState.VB)
				Dim xmlPrefixSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax = Nothing
				Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(xmlNameToken.GetTrailingTrivia())
				If (syntaxList.Count > 0 AndAlso Parser.IsAsciiColonTrivia(syntaxList(0))) Then
					syntaxList = syntaxList.GetStartOfTrivia(syntaxList.Count - 1)
					xmlNameToken = Me.SyntaxFactory.XmlNameToken(xmlNameToken.Text, xmlNameToken.PossibleKeywordKind, xmlNameToken.GetLeadingTrivia(), syntaxList.Node).WithDiagnostics(xmlNameToken.GetDiagnostics())
					Me.ResetCurrentToken(ScannerState.Element)
					Dim currentToken As PunctuationSyntax = DirectCast(Me.CurrentToken, PunctuationSyntax)
					Me.GetNextToken(ScannerState.Element)
					currentToken = Me.TransitionFromXmlToVB(Of PunctuationSyntax)(currentToken)
					xmlPrefixSyntax = Me.SyntaxFactory.XmlPrefix(xmlNameToken, currentToken)
					xmlNameToken = Nothing
					If (syntaxList.Count = 0 AndAlso Not currentToken.HasTrailingTrivia AndAlso Parser.IsValidXmlQualifiedNameToken(Me.CurrentToken)) Then
						xmlNameToken = Me.ToXmlNameToken(Me.CurrentToken)
						Me.GetNextToken(ScannerState.VB)
					End If
					If (xmlNameToken Is Nothing) Then
						xmlNameToken = Parser.ReportSyntaxError(Of XmlNameTokenSyntax)(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlNameToken("", SyntaxKind.XmlNameToken, Nothing, Nothing), ERRID.ERR_ExpectedXmlName)
					End If
				End If
				xmlNameSyntax = Me.SyntaxFactory.XmlName(xmlPrefixSyntax, xmlNameToken)
			Else
				xmlNameSyntax = Me.ReportExpectedXmlName()
			End If
			Return xmlNameSyntax
		End Function

		Friend Function ParseXmlString(ByVal nextState As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax
			Dim xmlStringSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax
			Dim scannerState As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax
			If (Me.CurrentToken.Kind = SyntaxKind.SingleQuoteToken) Then
				scannerState = If(EmbeddedOperators.CompareString(Me.CurrentToken.Text, "'", False) = 0, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState.SingleQuotedString, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState.SmartSingleQuotedString)
				currentToken = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
				Me.GetNextToken(scannerState)
			ElseIf (Me.CurrentToken.Kind <> SyntaxKind.DoubleQuoteToken) Then
				scannerState = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState.UnQuotedString
				currentToken = DirectCast(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.SingleQuoteToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
				currentToken = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(currentToken, ERRID.ERR_StartAttributeValue)
				Me.ResetCurrentToken(scannerState)
			Else
				scannerState = If(EmbeddedOperators.CompareString(Me.CurrentToken.Text, """", False) = 0, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState.QuotedString, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState.SmartQuotedString)
				currentToken = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
				Me.GetNextToken(scannerState)
			End If
			Dim syntaxListBuilder As SyntaxListBuilder(Of XmlTextTokenSyntax) = Me._pool.Allocate(Of XmlTextTokenSyntax)()
			While True
				Dim kind As SyntaxKind = Me.CurrentToken.Kind
				If (kind = SyntaxKind.SingleQuoteToken OrElse kind = SyntaxKind.DoubleQuoteToken) Then
					Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
					Me.GetNextToken(nextState)
					Dim xmlStringSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax = Me.SyntaxFactory.XmlString(currentToken, syntaxListBuilder.ToList(), punctuationSyntax)
					Me._pool.Free(syntaxListBuilder)
					xmlStringSyntax = xmlStringSyntax1
					Exit While
				ElseIf (CUShort(kind) - CUShort(SyntaxKind.XmlTextLiteralToken) <= CUShort(SyntaxKind.EmptyStatement)) Then
					syntaxListBuilder.Add(DirectCast(Me.CurrentToken, XmlTextTokenSyntax))
					Me.GetNextToken(scannerState)
				Else
					Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Parser.HandleUnexpectedToken(currentToken.Kind)
					Dim xmlStringSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax = Me.SyntaxFactory.XmlString(currentToken, syntaxListBuilder.ToList(), DirectCast(syntaxToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax))
					Me._pool.Free(syntaxListBuilder)
					xmlStringSyntax = xmlStringSyntax2
					Exit While
				End If
			End While
			Return xmlStringSyntax
		End Function

		Private Function ParseYieldStatement() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.YieldStatementSyntax
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			Me.TryIdentifierAsContextualKeyword(Me.CurrentToken, keywordSyntax)
			keywordSyntax = Me.CheckFeatureAvailability(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)(Feature.Iterators, keywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
			Return Me.SyntaxFactory.YieldStatement(keywordSyntax, expressionSyntax)
		End Function

		Private Function PeekAheadFor(ByVal ParamArray kinds As SyntaxKind()) As SyntaxKind
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Nothing
			Me.PeekAheadFor(Of SyntaxKind())(Parser.s_isTokenOrKeywordFunc, kinds, syntaxToken)
			If (syntaxToken Is Nothing) Then
				Return SyntaxKind.None
			End If
			Return syntaxToken.Kind
		End Function

		Private Function PeekAheadFor(Of TArg)(ByVal predicate As Func(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, TArg, Boolean), ByVal arg As TArg, <Out> ByRef token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Integer
			Dim num As Integer
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.CurrentToken
			Dim num1 As Integer = 0
			While True
				If (Me.IsValidStatementTerminator(currentToken)) Then
					token = Nothing
					num = 0
					Exit While
				ElseIf (Not predicate(currentToken, arg)) Then
					num1 = num1 + 1
					currentToken = Me.PeekToken(num1)
				Else
					token = currentToken
					num = num1
					Exit While
				End If
			End While
			Return num
		End Function

		Private Function PeekAheadForToken(ByVal ParamArray kinds As SyntaxKind()) As Integer
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Nothing
			Return Me.PeekAheadFor(Of SyntaxKind())(Parser.s_isTokenOrKeywordFunc, kinds, syntaxToken)
		End Function

		Private Function PeekDeclarationStatement(ByVal i As Integer) As Boolean
			Dim flag As Boolean
			While True
				Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.PeekToken(i)
				Dim kind As SyntaxKind = syntaxToken.Kind
				If (kind <= SyntaxKind.ReadOnlyKeyword) Then
					If (kind <= SyntaxKind.EventKeyword) Then
						If (kind > SyntaxKind.DelegateKeyword) Then
							If (kind = SyntaxKind.EnumKeyword OrElse kind = SyntaxKind.EventKeyword) Then
								Exit While
							End If
							flag = False
							Return flag
						Else
							If (kind = SyntaxKind.ClassKeyword) Then
								Exit While
							End If
							Select Case kind
								Case SyntaxKind.DeclareKeyword
								Case SyntaxKind.DelegateKeyword
									flag = True
									Return flag
								Case SyntaxKind.DefaultKeyword
									Exit Select
								Case Else
									flag = False
									Return flag
							End Select
						End If
					ElseIf (kind > SyntaxKind.GetKeyword) Then
						If (kind = SyntaxKind.InterfaceKeyword) Then
							Exit While
						End If
						Select Case kind
							Case SyntaxKind.ModuleKeyword
							Case SyntaxKind.NamespaceKeyword
								flag = True
								Return flag
							Case SyntaxKind.MustInheritKeyword
							Case SyntaxKind.MustOverrideKeyword
							Case SyntaxKind.NarrowingKeyword
								Exit Select
							Case SyntaxKind.MyBaseKeyword
							Case SyntaxKind.MyClassKeyword
								flag = False
								Return flag
							Case Else
								Select Case kind
									Case SyntaxKind.NotInheritableKeyword
									Case SyntaxKind.NotOverridableKeyword
									Case SyntaxKind.OverloadsKeyword
									Case SyntaxKind.OverridableKeyword
									Case SyntaxKind.OverridesKeyword
									Case SyntaxKind.PartialKeyword
									Case SyntaxKind.PrivateKeyword
									Case SyntaxKind.ProtectedKeyword
									Case SyntaxKind.PublicKeyword
									Case SyntaxKind.ReadOnlyKeyword
										Exit Select
									Case SyntaxKind.ObjectKeyword
									Case SyntaxKind.OfKeyword
									Case SyntaxKind.OnKeyword
									Case SyntaxKind.OptionKeyword
									Case SyntaxKind.OptionalKeyword
									Case SyntaxKind.OrKeyword
									Case SyntaxKind.OrElseKeyword
									Case SyntaxKind.ParamArrayKeyword
									Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.EndRemoveHandlerStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PrivateKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.PublicKeyword
									Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.NotKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OverridesKeyword
									Case SyntaxKind.RaiseEventKeyword
										flag = False
										Return flag
									Case SyntaxKind.OperatorKeyword
									Case SyntaxKind.PropertyKeyword
										flag = True
										Return flag
									Case Else
										flag = False
										Return flag
								End Select

						End Select
					Else
						If (kind = SyntaxKind.FriendKeyword) Then
							GoTo Label3
						End If
						If (CUShort(kind) - CUShort(SyntaxKind.FunctionKeyword) <= CUShort(SyntaxKind.List)) Then
							Exit While
						End If
						flag = False
						Return flag
					End If
				ElseIf (kind <= SyntaxKind.WithEventsKeyword) Then
					If (kind > SyntaxKind.StructureKeyword) Then
						If (kind = SyntaxKind.SubKeyword) Then
							Exit While
						End If
						If (kind = SyntaxKind.WideningKeyword OrElse kind = SyntaxKind.WithEventsKeyword) Then
							GoTo Label3
						End If
						flag = False
						Return flag
					Else
						Select Case kind
							Case SyntaxKind.SetKeyword
								flag = True
								Return flag
							Case SyntaxKind.ShadowsKeyword
							Case SyntaxKind.SharedKeyword
							Case SyntaxKind.StaticKeyword
								Exit Select
							Case SyntaxKind.ShortKeyword
							Case SyntaxKind.SingleKeyword
								flag = False
								Return flag
							Case Else
								If (kind = SyntaxKind.StructureKeyword) Then
									flag = True
									Return flag
								End If
								flag = False
								Return flag
						End Select
					End If
				ElseIf (kind <= SyntaxKind.CustomKeyword) Then
					If (kind = SyntaxKind.WriteOnlyKeyword OrElse kind = SyntaxKind.CustomKeyword) Then
						GoTo Label3
					End If
					flag = False
					Return flag
				ElseIf (kind <> SyntaxKind.AsyncKeyword AndAlso kind <> SyntaxKind.IteratorKeyword) Then
					If (kind <> SyntaxKind.IdentifierToken) Then
						flag = False
						Return flag
					End If
					Dim possibleKeywordKind As SyntaxKind = DirectCast(syntaxToken, IdentifierTokenSyntax).PossibleKeywordKind
					If (possibleKeywordKind <> SyntaxKind.CustomKeyword AndAlso possibleKeywordKind <> SyntaxKind.AsyncKeyword AndAlso possibleKeywordKind <> SyntaxKind.IteratorKeyword) Then
						flag = False
						Return flag
					End If
				End If
			Label3:
				i = i + 1
			End While
			flag = True
			Return flag
		End Function

		Private Function PeekEndStatement(ByVal i As Integer) As SyntaxKind
			Dim endStatementKindFromKeyword As SyntaxKind
			Dim kind As SyntaxKind = Me.PeekToken(i).Kind
			If (kind <= SyntaxKind.LoopKeyword) Then
				If (kind = SyntaxKind.EndKeyword) Then
					endStatementKindFromKeyword = Parser.GetEndStatementKindFromKeyword(Me.PeekToken(i + 1).Kind)
				Else
					If (kind <> SyntaxKind.LoopKeyword) Then
						endStatementKindFromKeyword = SyntaxKind.None
						Return endStatementKindFromKeyword
					End If
					endStatementKindFromKeyword = SyntaxKind.SimpleLoopStatement
				End If
			ElseIf (kind = SyntaxKind.NextKeyword) Then
				endStatementKindFromKeyword = SyntaxKind.NextStatement
			ElseIf (kind = SyntaxKind.EndIfKeyword) Then
				endStatementKindFromKeyword = SyntaxKind.EndIfStatement
			Else
				If (kind <> SyntaxKind.WendKeyword) Then
					endStatementKindFromKeyword = SyntaxKind.None
					Return endStatementKindFromKeyword
				End If
				endStatementKindFromKeyword = SyntaxKind.EndWhileStatement
			End If
			Return endStatementKindFromKeyword
		End Function

		Friend Function PeekNextToken(Optional ByVal state As ScannerState = 0) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			If (Me._allowLeadingMultilineTrivia AndAlso state = ScannerState.VB) Then
				state = ScannerState.VBAllowLeadingMultilineTrivia
			End If
			Return Me._scanner.PeekNextToken(state)
		End Function

		Private Function PeekPastStatementTerminator() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.PeekToken(1)
			If (syntaxToken1.Kind = SyntaxKind.StatementTerminatorToken) Then
				Dim syntaxToken2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.PeekToken(2)
				If (syntaxToken2.Kind = SyntaxKind.EmptyToken) Then
					syntaxToken = syntaxToken1
					Return syntaxToken
				End If
				syntaxToken = syntaxToken2
				Return syntaxToken
			End If
			syntaxToken = syntaxToken1
			Return syntaxToken
		End Function

		Private Function PeekToken(ByVal offset As Integer) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Return Me._scanner.PeekToken(offset, If(Me._allowLeadingMultilineTrivia, ScannerState.VBAllowLeadingMultilineTrivia, ScannerState.VB))
		End Function

		Private Shared Function RelationalOperatorKindToCaseKind(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Select Case kind
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanToken
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseLessThanClause
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanEqualsToken
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseLessThanOrEqualClause
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanGreaterThanToken
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseNotEqualsClause
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseEqualsClause
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanToken
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseGreaterThanClause
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanEqualsToken
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseGreaterThanOrEqualClause
					Exit Select
				Case Else
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
					Exit Select
			End Select
			Return syntaxKind
		End Function

		Private Shared Function RemoveTrailingColonTriviaAndConvertToColonToken(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, <Out> ByRef colonToken As PunctuationSyntax, <Out> ByRef excessText As String) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim node As GreenNode
			Dim startOfTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			If (token.HasTrailingTrivia) Then
				Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(token.GetTrailingTrivia())
				Dim num As Integer = -1
				If (syntaxList.Count = 1) Then
					num = 0
					node = Nothing
					excessText = Nothing
				ElseIf (syntaxList(0).Kind <> SyntaxKind.ColonTrivia) Then
					Dim count As Integer = syntaxList.Count - 1
					Dim num1 As Integer = 0
					While num1 <= count
						If (syntaxList(num1).Kind <> SyntaxKind.ColonTrivia) Then
							num1 = num1 + 1
						Else
							num = num1
							Exit While
						End If
					End While
					startOfTrivia = syntaxList.GetStartOfTrivia(num)
					node = startOfTrivia.Node
					If (num <> syntaxList.Count - 1) Then
						startOfTrivia = syntaxList.GetEndOfTrivia(num + 1)
						excessText = startOfTrivia.Node.ToFullString()
					Else
						excessText = Nothing
					End If
				Else
					num = 0
					node = Nothing
					startOfTrivia = syntaxList.GetEndOfTrivia(1)
					excessText = startOfTrivia.Node.ToFullString()
				End If
				Dim item As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia = DirectCast(syntaxList(num), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia)
				colonToken = New PunctuationSyntax(SyntaxKind.ColonToken, item.Text, Nothing, Nothing)
				syntaxToken = DirectCast(token.WithTrailingTrivia(node), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			Else
				colonToken = Nothing
				excessText = Nothing
				syntaxToken = token
			End If
			Return syntaxToken
		End Function

		Private Function RemoveTrailingColonTriviaAndConvertToColonToken(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, <Out> ByRef colonToken As PunctuationSyntax, <Out> ByRef excessText As String) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim lastToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = node.GetLastToken()
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Parser.RemoveTrailingColonTriviaAndConvertToColonToken(lastToken, colonToken, excessText)
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = LastTokenReplacer.Replace(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(node, Function(t As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
				If (t <> lastToken) Then
					Return t
				End If
				Return syntaxToken
			End Function)
			If (visualBasicSyntaxNode = node) Then
				colonToken = Nothing
				excessText = Nothing
			End If
			Return visualBasicSyntaxNode
		End Function

		Private Function ReportExpectedXmlBracketedName() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Parser.HandleUnexpectedToken(SyntaxKind.LessThanToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax = Me.ReportExpectedXmlName()
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.GreaterThanToken)
			Return Me.SyntaxFactory.XmlBracketedName(punctuationSyntax, xmlNameSyntax, punctuationSyntax1)
		End Function

		Private Function ReportExpectedXmlName() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax
			Return Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax)(Me.SyntaxFactory.XmlName(Nothing, Me.SyntaxFactory.XmlNameToken("", SyntaxKind.XmlNameToken, Nothing, Nothing)), ERRID.ERR_ExpectedXmlName)
		End Function

		Private Shared Function ReportFeatureUnavailable(Of TNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal feature As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature, ByVal node As TNode, ByVal languageVersion As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion) As TNode
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(feature.GetResourceId())
			Dim visualBasicRequiredLanguageVersion As Microsoft.CodeAnalysis.VisualBasic.VisualBasicRequiredLanguageVersion = New Microsoft.CodeAnalysis.VisualBasic.VisualBasicRequiredLanguageVersion(feature.GetLanguageVersion())
			Return Parser.ReportSyntaxError(Of TNode)(node, ERRID.ERR_LanguageVersion, New [Object]() { languageVersion.GetErrorName(), diagnosticInfo, visualBasicRequiredLanguageVersion })
		End Function

		Friend Function ReportFeatureUnavailable(Of TNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal feature As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature, ByVal node As TNode) As TNode
			Return Parser.ReportFeatureUnavailable(Of TNode)(feature, node, Me._scanner.Options.LanguageVersion)
		End Function

		Private Function ReportGenericArgumentsDisallowedError(ByVal errid As Microsoft.CodeAnalysis.VisualBasic.ERRID) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax
			Dim flag As Boolean = True
			Dim flag1 As Boolean = True
			Dim typeArgumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax = Me.ParseGenericArguments(flag, flag1)
			If (typeArgumentListSyntax.CloseParenToken.IsMissing) Then
				typeArgumentListSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax)(typeArgumentListSyntax)
			End If
			typeArgumentListSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax)(typeArgumentListSyntax, errid)
			Return typeArgumentListSyntax
		End Function

		Private Function ReportGenericParamsDisallowedError(ByVal errid As Microsoft.CodeAnalysis.VisualBasic.ERRID) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax
			Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax = Me.ParseGenericParameters()
			If (typeParameterListSyntax.CloseParenToken.IsMissing) Then
				typeParameterListSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax)(typeParameterListSyntax)
			End If
			typeParameterListSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax)(typeParameterListSyntax, errid)
			typeParameterListSyntax = Parser.AdjustTriviaForMissingTokens(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax)(typeParameterListSyntax)
			Return typeParameterListSyntax
		End Function

		Private Function ReportModifiersOnStatementError(ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax), ByVal keyword As KeywordSyntax) As KeywordSyntax
			Return Me.ReportModifiersOnStatementError(ERRID.ERR_SpecifiersInvalidOnInheritsImplOpt, attributes, modifiers, keyword)
		End Function

		Private Function ReportModifiersOnStatementError(ByVal errorId As ERRID, ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax), ByVal keyword As KeywordSyntax) As KeywordSyntax
			If (modifiers.Any()) Then
				keyword = keyword.AddLeadingSyntax(modifiers.Node, errorId)
			End If
			If (attributes.Any()) Then
				keyword = keyword.AddLeadingSyntax(attributes.Node, errorId)
			End If
			Return keyword
		End Function

		Private Shared Function ReportNonTrailingNamedArgumentIfNeeded(ByVal argument As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax, ByVal seenNames As Boolean, ByVal allowNonTrailingNamedArguments As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax
			Dim argumentSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax
			argumentSyntax = If(Not seenNames OrElse allowNonTrailingNamedArguments, argument, Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax)(argument, ERRID.ERR_ExpectedNamedArgument, New [Object]() { New VisualBasicRequiredLanguageVersion(Feature.NonTrailingNamedArguments.GetLanguageVersion()) }))
			Return argumentSyntax
		End Function

		Friend Shared Function ReportSyntaxError(Of T As GreenNode)(ByVal syntax As T, ByVal ErrorId As ERRID) As T
			Return DirectCast(syntax.AddError(ErrorFactory.ErrorInfo(ErrorId)), T)
		End Function

		Friend Shared Function ReportSyntaxError(Of T As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal syntax As T, ByVal ErrorId As ERRID, ByVal ParamArray args As Object()) As T
			Return DirectCast(syntax.AddError(ErrorFactory.ErrorInfo(ErrorId, args)), T)
		End Function

		Private Sub ReportSyntaxErrorForLanguageFeature(ByVal Errid As Microsoft.CodeAnalysis.VisualBasic.ERRID, ByVal Start As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal Feature As UInteger, ByVal wszVersion As String)
		End Sub

		Private Function ReportUnrecognizedStatementError(ByVal ErrorId As ERRID) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)()
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)()
			Return Me.ReportUnrecognizedStatementError(ErrorId, syntaxList, syntaxList1, False, False)
		End Function

		Private Function ReportUnrecognizedStatementError(ByVal ErrorId As ERRID, ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax), ByVal modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax), Optional ByVal createMissingIdentifier As Boolean = False, Optional ByVal forceErrorOnFirstToken As Boolean = False) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = Me.ResyncAt()
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = Nothing
			If (createMissingIdentifier) Then
				identifierTokenSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingIdentifier()
				identifierTokenSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)(identifierTokenSyntax, ErrorId)
			End If
			If (modifiers.Any() OrElse attributes.Any()) Then
				statementSyntax = Me.SyntaxFactory.IncompleteMember(attributes, modifiers, identifierTokenSyntax)
				statementSyntax = If(Not forceErrorOnFirstToken, statementSyntax.AddTrailingSyntax(syntaxList), statementSyntax.AddTrailingSyntax(syntaxList, ErrorId))
			Else
				statementSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.EmptyStatement()
				statementSyntax = If(syntaxList.ContainsDiagnostics(), statementSyntax.AddTrailingSyntax(syntaxList), statementSyntax.AddTrailingSyntax(syntaxList, ErrorId))
			End If
			If (Not statementSyntax.ContainsDiagnostics) Then
				statementSyntax = statementSyntax.AddError(ErrorId)
			End If
			Return statementSyntax
		End Function

		Private Function ReportUnrecognizedTypeInGeneric(ByVal typeName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax
			Dim kind As SyntaxKind = typeName.Kind
			If (kind = SyntaxKind.GenericName) Then
				typeName = Me.ReportUnrecognizedTypeInGeneric(DirectCast(typeName, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax))
			ElseIf (kind = SyntaxKind.QualifiedName) Then
				Dim qualifiedNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax = DirectCast(typeName, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax)
				Dim right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax = TryCast(qualifiedNameSyntax.Right, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax)
				If (right Is Nothing) Then
					Dim nameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax = DirectCast(Me.ReportUnrecognizedTypeInGeneric(qualifiedNameSyntax.Left), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax)
					typeName = Me.SyntaxFactory.QualifiedName(nameSyntax, qualifiedNameSyntax.DotToken, qualifiedNameSyntax.Right)
				Else
					right = Me.ReportUnrecognizedTypeInGeneric(right)
					typeName = Me.SyntaxFactory.QualifiedName(qualifiedNameSyntax.Left, qualifiedNameSyntax.DotToken, right)
				End If
			End If
			Return typeName
		End Function

		Private Function ReportUnrecognizedTypeInGeneric(ByVal genericName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax
			Dim typeArgumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax = genericName.TypeArgumentList
			typeArgumentList = Me.SyntaxFactory.TypeArgumentList(typeArgumentList.OpenParenToken, typeArgumentList.OfKeyword, typeArgumentList.Arguments, Parser.ReportSyntaxError(Of PunctuationSyntax)(typeArgumentList.CloseParenToken, ERRID.ERR_UnrecognizedType))
			genericName = Me.SyntaxFactory.GenericName(genericName.Identifier, typeArgumentList)
			Return genericName
		End Function

		Private Sub RescanTrailingColonAsToken(ByRef prevToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByRef currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			Me._scanner.RescanTrailingColonAsToken(prevToken, currentToken)
			Me._currentToken = currentToken
		End Sub

		Private Sub ResetCurrentToken(ByVal state As ScannerState)
			Me._scanner.ResetCurrentToken(state)
			Me._currentToken = Nothing
		End Sub

		Private Function ResyncAndConsumeStatementTerminator() As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = Me._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)()
			While Me.CurrentToken.Kind <> SyntaxKind.EndOfFileToken AndAlso Me.CurrentToken.Kind <> SyntaxKind.StatementTerminatorToken
				syntaxListBuilder.Add(Me.CurrentToken)
				Me.GetNextToken(ScannerState.VB)
			End While
			If (Me.CurrentToken.Kind = SyntaxKind.StatementTerminatorToken) Then
				If (Me.CurrentToken.HasLeadingTrivia) Then
					syntaxListBuilder.Add(Me.CurrentToken)
				End If
				Me.GetNextToken(ScannerState.VB)
			End If
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = syntaxListBuilder.ToList()
			Me._pool.Free(syntaxListBuilder)
			Return list
		End Function

		Private Sub ResyncAt(ByVal skippedTokens As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken), ByVal state As ScannerState, ByVal resyncTokens As SyntaxKind())
			While Me.CurrentToken.Kind <> SyntaxKind.EndOfFileToken
				If (state.IsVBState()) Then
					If (Me.IsValidStatementTerminator(Me.CurrentToken)) Then
						Exit While
					End If
					If (Me.CurrentToken.Kind = SyntaxKind.EmptyToken) Then
						Return
					End If
				ElseIf (Me.CurrentToken.Kind = SyntaxKind.EndOfXmlToken OrElse Me.CurrentToken.Kind = SyntaxKind.EndOfInterpolatedStringToken) Then
					Exit While
				End If
				If (Parser.IsTokenOrKeyword(Me.CurrentToken, resyncTokens)) Then
					Exit While
				End If
				skippedTokens.Add(Me.CurrentToken)
				Me.GetNextToken(state)
			End While
		End Sub

		Private Function ResyncAt(ByVal state As ScannerState, ByVal resyncTokens As SyntaxKind()) As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = Me._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)()
			Me.ResyncAt(syntaxListBuilder, state, resyncTokens)
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = syntaxListBuilder.ToList()
			Me._pool.Free(syntaxListBuilder)
			Return list
		End Function

		Friend Function ResyncAt() As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			Return Me.ResyncAt(ScannerState.VB, Array.Empty(Of SyntaxKind)())
		End Function

		Friend Function ResyncAt(ByVal resyncTokens As SyntaxKind()) As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			Return Me.ResyncAt(ScannerState.VB, resyncTokens)
		End Function

		Private Function ResyncAt(Of T As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal syntax As T, ByVal state As ScannerState, ByVal ParamArray resyncTokens As SyntaxKind()) As T
			Return syntax.AddTrailingSyntax(Me.ResyncAt(state, resyncTokens))
		End Function

		Private Function ResyncAt(Of T As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal syntax As T) As T
			Return syntax.AddTrailingSyntax(Me.ResyncAt())
		End Function

		Private Function ResyncAt(Of T As GreenNode)(ByVal syntax As T, ByVal ParamArray resyncTokens As SyntaxKind()) As T
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = Me.ResyncAt(resyncTokens)
			Return syntax.AddTrailingSyntax(syntaxList.Node)
		End Function

		Private Function ResyncXmlContent() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
			Dim xmlTextSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextSyntax
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = Me.ResyncAt(ScannerState.Content, New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("B887586B9562536EA96166B718EA69090AC4EE74E4480ECA6BB04C746F3AFA9D").FieldHandle })
			Dim kind As SyntaxKind = Me.CurrentToken.Kind
			If (kind = SyntaxKind.XmlTextLiteralToken OrElse kind = SyntaxKind.DocumentationCommentLineBreakToken OrElse kind = SyntaxKind.XmlEntityLiteralToken) Then
				xmlTextSyntax = Me.SyntaxFactory.XmlText(Me.CurrentToken)
				Me.GetNextToken(ScannerState.Content)
			Else
				xmlTextSyntax = Me.SyntaxFactory.XmlText(Parser.HandleUnexpectedToken(SyntaxKind.XmlTextLiteralToken))
			End If
			xmlTextSyntax = If(Not xmlTextSyntax.ContainsDiagnostics, xmlTextSyntax.AddLeadingSyntax(syntaxList, ERRID.ERR_Syntax), xmlTextSyntax.AddLeadingSyntax(syntaxList))
			Return xmlTextSyntax
		End Function

		Private Function ResyncXmlElement(ByVal state As ScannerState, ByVal lessThan As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, ByVal Name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax, ByVal attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
			Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = Me.ResyncAt(ScannerState.Element, New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("4A374453DD0865BEBEA1387A38FFD4F3D341FBD1259E49EEC76A28E066D45E70").FieldHandle })
			Dim kind As SyntaxKind = Me.CurrentToken.Kind
			If (kind = SyntaxKind.GreaterThanToken) Then
				currentToken = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
				Me.GetNextToken(ScannerState.Content)
				If (syntaxList.Node IsNot Nothing) Then
					currentToken = currentToken.AddLeadingSyntax(syntaxList, ERRID.ERR_ExpectedGreater)
				End If
				xmlNodeSyntax = Me.SyntaxFactory.XmlElementStartTag(lessThan, Name, attributes, currentToken)
			ElseIf (kind <> SyntaxKind.SlashGreaterThanToken) Then
				currentToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingPunctuation(SyntaxKind.GreaterThanToken)
				If (syntaxList.Node IsNot Nothing) Then
					currentToken = currentToken.AddLeadingSyntax(syntaxList, ERRID.ERR_Syntax)
				ElseIf (attributes.Node Is Nothing OrElse Not attributes.Node.ContainsDiagnostics) Then
					currentToken = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)(currentToken, ERRID.ERR_ExpectedGreater)
				End If
				xmlNodeSyntax = Me.SyntaxFactory.XmlElementStartTag(lessThan, Name, attributes, currentToken)
			Else
				Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
				If (syntaxList.Node IsNot Nothing) Then
					punctuationSyntax = punctuationSyntax.AddLeadingSyntax(syntaxList, ERRID.ERR_ExpectedGreater)
				End If
				Me.GetNextToken(state)
				xmlNodeSyntax = Me.SyntaxFactory.XmlEmptyElement(lessThan, Name, attributes, punctuationSyntax)
			End If
			Return xmlNodeSyntax
		End Function

		Private Function ShouldParseAsLabel() As Boolean
			If (Not Me.IsFirstStatementOnLine(Me.CurrentToken)) Then
				Return False
			End If
			Return Me.PeekToken(1).Kind = SyntaxKind.ColonToken
		End Function

		Private Shared Function StartsValidConditionalCompilationExpr(ByVal t As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Boolean
			Dim flag As Boolean
			Dim kind As SyntaxKind = t.Kind
			If (kind <= SyntaxKind.TrueKeyword) Then
				If (kind > SyntaxKind.FalseKeyword) Then
					If (kind = SyntaxKind.IfKeyword OrElse CUShort(kind) - CUShort(SyntaxKind.NotKeyword) <= CUShort(SyntaxKind.List) OrElse kind = SyntaxKind.TrueKeyword) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				Else
					Select Case kind
						Case SyntaxKind.CBoolKeyword
						Case SyntaxKind.CByteKeyword
						Case SyntaxKind.CCharKeyword
						Case SyntaxKind.CDateKeyword
						Case SyntaxKind.CDecKeyword
						Case SyntaxKind.CDblKeyword
						Case SyntaxKind.CIntKeyword
						Case SyntaxKind.CLngKeyword
						Case SyntaxKind.CObjKeyword
						Case SyntaxKind.CSByteKeyword
						Case SyntaxKind.CShortKeyword
						Case SyntaxKind.CSngKeyword
						Case SyntaxKind.CStrKeyword
						Case SyntaxKind.CTypeKeyword
						Case SyntaxKind.CUIntKeyword
						Case SyntaxKind.CULngKeyword
						Case SyntaxKind.CUShortKeyword
						Case SyntaxKind.DirectCastKeyword
							Exit Select
						Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.PrintStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineElseClause Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.MidExpression Or SyntaxKind.AddHandlerStatement Or SyntaxKind.RaiseEventStatement Or SyntaxKind.ReDimStatement Or SyntaxKind.RedimClause Or SyntaxKind.GetXmlNamespaceExpression Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.XmlDescendantAccessExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.AnonymousObjectCreationExpression Or SyntaxKind.CollectionInitializer Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlPrefixName Or SyntaxKind.XmlBracketedName Or SyntaxKind.XmlComment Or SyntaxKind.XmlCDataSection Or SyntaxKind.ArrayType Or SyntaxKind.PredefinedType Or SyntaxKind.AndKeyword Or SyntaxKind.AsKeyword Or SyntaxKind.ByRefKeyword Or SyntaxKind.ByValKeyword Or SyntaxKind.CaseKeyword Or SyntaxKind.CBoolKeyword
						Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueDoStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineIfPart Or SyntaxKind.SingleLineElseClause Or SyntaxKind.MultiLineIfBlock Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.ConcatenateAssignmentStatement Or SyntaxKind.MidExpression Or SyntaxKind.CallStatement Or SyntaxKind.AddHandlerStatement Or SyntaxKind.RemoveHandlerStatement Or SyntaxKind.RaiseEventStatement Or SyntaxKind.WithStatement Or SyntaxKind.ReDimStatement Or SyntaxKind.ReDimPreserveStatement Or SyntaxKind.RedimClause Or SyntaxKind.EraseStatement Or SyntaxKind.GetXmlNamespaceExpression Or SyntaxKind.SimpleMemberAccessExpression Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.XmlElementAccessExpression Or SyntaxKind.XmlDescendantAccessExpression Or SyntaxKind.XmlAttributeAccessExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.ObjectCreationExpression Or SyntaxKind.AnonymousObjectCreationExpression Or SyntaxKind.ArrayCreationExpression Or SyntaxKind.CollectionInitializer Or SyntaxKind.CTypeExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlEmptyElement Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlString Or SyntaxKind.XmlPrefixName Or SyntaxKind.XmlName Or SyntaxKind.XmlBracketedName Or SyntaxKind.XmlPrefix Or SyntaxKind.XmlComment Or SyntaxKind.XmlProcessingInstruction Or SyntaxKind.XmlCDataSection Or SyntaxKind.XmlEmbeddedExpression Or SyntaxKind.ArrayType Or SyntaxKind.NullableType Or SyntaxKind.PredefinedType Or SyntaxKind.IdentifierName Or SyntaxKind.AndKeyword Or SyntaxKind.AndAlsoKeyword Or SyntaxKind.AsKeyword Or SyntaxKind.BooleanKeyword Or SyntaxKind.ByRefKeyword Or SyntaxKind.ByteKeyword Or SyntaxKind.ByValKeyword Or SyntaxKind.CallKeyword Or SyntaxKind.CaseKeyword Or SyntaxKind.CatchKeyword Or SyntaxKind.CBoolKeyword Or SyntaxKind.CByteKeyword
						Case SyntaxKind.CharKeyword
						Case SyntaxKind.ClassKeyword
						Case SyntaxKind.ConstKeyword
						Case SyntaxKind.ReferenceKeyword
						Case SyntaxKind.ContinueKeyword
						Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.ConcatenateAssignmentStatement Or SyntaxKind.NotEqualsExpression Or SyntaxKind.LessThanExpression Or SyntaxKind.LessThanOrEqualExpression Or SyntaxKind.GreaterThanOrEqualExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlEmptyElement Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlString Or SyntaxKind.CTypeKeyword Or SyntaxKind.CUIntKeyword Or SyntaxKind.CULngKeyword
						Case SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.MidExpression Or SyntaxKind.NotEqualsExpression Or SyntaxKind.GreaterThanExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlPrefixName Or SyntaxKind.CTypeKeyword
						Case SyntaxKind.DateKeyword
						Case SyntaxKind.DecimalKeyword
						Case SyntaxKind.DeclareKeyword
						Case SyntaxKind.DefaultKeyword
						Case SyntaxKind.DelegateKeyword
						Case SyntaxKind.DimKeyword
							flag = False
							Return flag
						Case Else
							If (kind = SyntaxKind.FalseKeyword) Then
								Exit Select
							End If
							flag = False
							Return flag
					End Select
				End If
			ElseIf (kind <= SyntaxKind.OpenParenToken) Then
				If (kind = SyntaxKind.TryCastKeyword OrElse kind = SyntaxKind.OpenParenToken) Then
					flag = True
					Return flag
				End If
				flag = False
				Return flag
			ElseIf (CUShort(kind) - CUShort(SyntaxKind.PlusToken) > CUShort(SyntaxKind.List) AndAlso kind <> SyntaxKind.StatementTerminatorToken AndAlso CUShort(kind) - CUShort(SyntaxKind.IdentifierToken) > CUShort(SyntaxKind.EndUsingStatement)) Then
				flag = False
				Return flag
			End If
			flag = True
			Return flag
		End Function

		Private Shared Function TokenContainsFullWidthChars(ByVal tk As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Boolean
			Dim flag As Boolean
			Dim text As String = tk.Text
			Dim num As Integer = 0
			While True
				If (num >= text.Length) Then
					flag = False
					Exit While
				ElseIf (Not SyntaxFacts.IsFullWidth(text(num))) Then
					num = num + 1
				Else
					flag = True
					Exit While
				End If
			End While
			Return flag
		End Function

		Private Function ToXmlNameToken(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax
			Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax
			If (token.Kind <> SyntaxKind.IdentifierToken) Then
				xmlNameTokenSyntax = Me.SyntaxFactory.XmlNameToken(token.Text, token.Kind, token.GetLeadingTrivia(), token.GetTrailingTrivia())
			Else
				Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(token, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
				Dim xmlNameTokenSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = Me.SyntaxFactory.XmlNameToken(identifierTokenSyntax.Text, identifierTokenSyntax.PossibleKeywordKind, token.GetLeadingTrivia(), token.GetTrailingTrivia())
				xmlNameTokenSyntax1 = If(Not identifierTokenSyntax.IsBracketed, Parser.VerifyXmlNameToken(xmlNameTokenSyntax1), Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)(xmlNameTokenSyntax1, ERRID.ERR_ExpectedXmlName))
				xmlNameTokenSyntax = xmlNameTokenSyntax1
			End If
			Return xmlNameTokenSyntax
		End Function

		Private Function TransitionFromVBToXml(Of T As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal state As ScannerState, ByVal node As T) As T
			node = LastTokenReplacer.Replace(Of T)(node, Function(token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
				Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(token.GetTrailingTrivia())
				Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
				Dim syntaxList2 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
				Me._scanner.TransitionFromVBToXml(state, syntaxList, syntaxList1, syntaxList2)
				syntaxList = syntaxList.GetStartOfTrivia(syntaxList.Count - syntaxList1.Count)
				token = DirectCast(token.WithTrailingTrivia(syntaxList.Node), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
				token = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.AddTrailingTrivia(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(token, syntaxList2)
				Return token
			End Function)
			Me._currentToken = Nothing
			Return node
		End Function

		Private Function TransitionFromXmlToVB(Of T As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal node As T) As T
			node = LastTokenReplacer.Replace(Of T)(node, Function(token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
				Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(token.GetTrailingTrivia())
				Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
				Dim syntaxList2 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
				Me._scanner.TransitionFromXmlToVB(syntaxList, syntaxList1, syntaxList2)
				syntaxList = syntaxList.GetStartOfTrivia(syntaxList.Count - syntaxList1.Count)
				token = DirectCast(token.WithTrailingTrivia(syntaxList.Node), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
				token = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.AddTrailingTrivia(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(token, syntaxList2)
				Return token
			End Function)
			Me._currentToken = Nothing
			Return node
		End Function

		Private Function TryEatNewLine(Optional ByVal state As ScannerState = 0) As Boolean
			Dim flag As Boolean
			If (Me.CurrentToken.Kind <> SyntaxKind.StatementTerminatorToken OrElse Me.PeekEndStatement(1) <> SyntaxKind.None OrElse Me._evaluatingConditionCompilationExpression OrElse Me.NextLineStartsWithStatementTerminator(0)) Then
				flag = False
			Else
				Me._hadImplicitLineContinuation = True
				If (Me.PrevToken.GetTrailingTrivia().ContainsCommentTrivia()) Then
					Me._hadLineContinuationComment = True
				End If
				Me.GetNextToken(state)
				flag = True
			End If
			Return flag
		End Function

		Private Function TryEatNewLineAndGetContextualKeyword(ByVal kind As SyntaxKind, ByRef keyword As KeywordSyntax, Optional ByVal createIfMissing As Boolean = False) As Boolean
			Dim flag As Boolean
			If (Me.TryGetContextualKeyword(kind, keyword, createIfMissing)) Then
				flag = True
			ElseIf (Me.CurrentToken.Kind <> SyntaxKind.StatementTerminatorToken OrElse Not Me.TryTokenAsContextualKeyword(Me.PeekToken(1), kind, keyword)) Then
				If (createIfMissing) Then
					keyword = Parser.HandleUnexpectedKeyword(kind)
				End If
				flag = False
			Else
				Me.TryEatNewLine(ScannerState.VB)
				Me.GetNextToken(ScannerState.VB)
				flag = True
			End If
			Return flag
		End Function

		Private Function TryEatNewLineAndGetToken(Of T As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(ByVal kind As SyntaxKind, ByRef token As T, Optional ByVal createIfMissing As Boolean = False, Optional ByVal state As ScannerState = 0) As Boolean
			Dim flag As Boolean
			If (Me.CurrentToken.Kind = kind) Then
				token = DirectCast(Me.CurrentToken, T)
				Me.GetNextToken(state)
				flag = True
			ElseIf (Not Me.TryEatNewLineIfFollowedBy(kind)) Then
				If (createIfMissing) Then
					token = DirectCast(Parser.HandleUnexpectedToken(kind), T)
				End If
				flag = False
			Else
				token = DirectCast(Me.CurrentToken, T)
				Me.GetNextToken(state)
				flag = True
			End If
			Return flag
		End Function

		Private Function TryEatNewLineIfFollowedBy(ByVal kind As SyntaxKind) As Boolean
			Dim flag As Boolean
			flag = If(Not Me.NextLineStartsWith(kind), False, Me.TryEatNewLine(ScannerState.VB))
			Return flag
		End Function

		Private Function TryEatNewLineIfNotFollowedBy(ByVal kind As SyntaxKind) As Boolean
			Dim flag As Boolean
			flag = If(Me.NextLineStartsWith(kind), False, Me.TryEatNewLine(ScannerState.VB))
			Return flag
		End Function

		Private Function TryGetContextualKeyword(ByVal kind As SyntaxKind, ByRef keyword As KeywordSyntax, Optional ByVal createIfMissing As Boolean = False) As Boolean
			Dim flag As Boolean
			If (Not Me.TryTokenAsContextualKeyword(Me.CurrentToken, kind, keyword)) Then
				If (createIfMissing) Then
					keyword = Parser.HandleUnexpectedKeyword(kind)
				End If
				flag = False
			Else
				Me.GetNextToken(ScannerState.VB)
				flag = True
			End If
			Return flag
		End Function

		Private Function TryGetContextualKeywordAndEatNewLine(ByVal kind As SyntaxKind, ByRef keyword As KeywordSyntax, Optional ByVal createIfMissing As Boolean = False) As Boolean
			Dim flag As Boolean = Me.TryGetContextualKeyword(kind, keyword, createIfMissing)
			If (flag) Then
				Me.TryEatNewLine(ScannerState.VB)
			End If
			Return flag
		End Function

		Private Function TryGetToken(Of T As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(ByVal kind As SyntaxKind, ByRef token As T) As Boolean
			Dim flag As Boolean
			If (Me.CurrentToken.Kind <> kind) Then
				flag = False
			Else
				token = DirectCast(Me.CurrentToken, T)
				Me.GetNextToken(ScannerState.VB)
				flag = True
			End If
			Return flag
		End Function

		Private Function TryGetTokenAndEatNewLine(Of T As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(ByVal kind As SyntaxKind, ByRef token As T, Optional ByVal createIfMissing As Boolean = False, Optional ByVal state As ScannerState = 0) As Boolean
			Dim flag As Boolean
			If (Me.CurrentToken.Kind <> kind) Then
				If (createIfMissing) Then
					token = DirectCast(Parser.HandleUnexpectedToken(kind), T)
				End If
				flag = False
			Else
				token = DirectCast(Me.CurrentToken, T)
				Me.GetNextToken(state)
				If (Me.CurrentToken.Kind = SyntaxKind.StatementTerminatorToken) Then
					Me.TryEatNewLine(state)
				End If
				flag = True
			End If
			Return flag
		End Function

		Private Shared Function TryIdentifierAsContextualKeyword(ByVal id As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByRef kind As SyntaxKind) As Boolean
			Return Scanner.TryIdentifierAsContextualKeyword(DirectCast(id, IdentifierTokenSyntax), kind)
		End Function

		Private Function TryIdentifierAsContextualKeyword(ByVal id As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByRef k As KeywordSyntax) As Boolean
			Return Me._scanner.TryIdentifierAsContextualKeyword(DirectCast(id, IdentifierTokenSyntax), k)
		End Function

		Friend Function TryParseCrefOperatorName() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			If (Not Me.TryTokenAsContextualKeyword(Me.CurrentToken, keywordSyntax1)) Then
				currentToken = Me.CurrentToken
			Else
				currentToken = keywordSyntax1
			End If
			If (Not SyntaxFacts.IsOperatorStatementOperatorToken(currentToken.Kind)) Then
				currentToken = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.PlusToken), ERRID.ERR_UnknownOperator)
			Else
				Me.GetNextToken(ScannerState.VB)
			End If
			Return Me.SyntaxFactory.CrefOperatorReference(keywordSyntax, currentToken)
		End Function

		Friend Function TryParseCrefOptionallyQualifiedName() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax
			Dim flag As Boolean
			Dim flag1 As Boolean
			Dim nameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax = Nothing
			If (Me.CurrentToken.Kind = SyntaxKind.GlobalKeyword) Then
				nameSyntax = Me.SyntaxFactory.GlobalName(DirectCast(Me.CurrentToken, KeywordSyntax))
				Me.GetNextToken(ScannerState.VB)
			ElseIf (Me.CurrentToken.Kind = SyntaxKind.ObjectKeyword) Then
				nameSyntax = Me.SyntaxFactory.IdentifierName(Me._scanner.MakeIdentifier(DirectCast(Me.CurrentToken, KeywordSyntax)))
				Me.GetNextToken(ScannerState.VB)
			ElseIf (Me.CurrentToken.Kind <> SyntaxKind.OperatorKeyword) Then
				If (Me.CurrentToken.Kind = SyntaxKind.NewKeyword) Then
					flag = False
					flag1 = True
					typeSyntax = Me.ParseSimpleName(False, False, True, False, True, flag, flag1)
					Return typeSyntax
				End If
				flag1 = False
				flag = True
				nameSyntax = Me.ParseSimpleName(True, False, False, False, False, flag1, flag)
			Else
				typeSyntax = Me.TryParseCrefOperatorName()
				Return typeSyntax
			End If
			While Me.CurrentToken.Kind = SyntaxKind.DotToken
				Dim currentToken As PunctuationSyntax = DirectCast(Me.CurrentToken, PunctuationSyntax)
				Me.GetNextToken(ScannerState.VB)
				If (Me.CurrentToken.Kind <> SyntaxKind.OperatorKeyword) Then
					flag = False
					flag1 = True
					Dim simpleNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax = Me.ParseSimpleName(True, False, False, False, True, flag, flag1)
					nameSyntax = Me.SyntaxFactory.QualifiedName(nameSyntax, currentToken, simpleNameSyntax)
				Else
					Dim crefOperatorReferenceSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax = Me.TryParseCrefOperatorName()
					typeSyntax = Me.SyntaxFactory.QualifiedCrefOperatorReference(nameSyntax, currentToken, crefOperatorReferenceSyntax)
					Return typeSyntax
				End If
			End While
			typeSyntax = nameSyntax
			Return typeSyntax
		End Function

		Friend Function TryParseCrefReference() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = Me.TryParseCrefOptionallyQualifiedName()
			Dim crefSignatureSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax = Nothing
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = Nothing
			If (Me.CurrentToken.Kind = SyntaxKind.OpenParenToken AndAlso typeSyntax.Kind <> SyntaxKind.PredefinedType) Then
				crefSignatureSyntax = Me.TryParseCrefReferenceSignature()
				If (Me.CurrentToken.Kind = SyntaxKind.AsKeyword) Then
					Dim currentToken As KeywordSyntax = DirectCast(Me.CurrentToken, KeywordSyntax)
					Me.GetNextToken(ScannerState.VB)
					Dim typeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = Me.ParseGeneralType(False)
					simpleAsClauseSyntax = Me.SyntaxFactory.SimpleAsClause(currentToken, New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)(), typeSyntax1)
				End If
			End If
			Dim crefReferenceSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax = Me.SyntaxFactory.CrefReference(typeSyntax, crefSignatureSyntax, simpleAsClauseSyntax)
			If (crefReferenceSyntax.ContainsDiagnostics) Then
				crefReferenceSyntax.ClearFlags(GreenNode.NodeFlags.ContainsDiagnostics)
			End If
			Return crefReferenceSyntax
		End Function

		Friend Function TryParseCrefReferenceSignature() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			Me.GetNextToken(ScannerState.VB)
			Dim separatedSyntaxListBuilder As SeparatedSyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignaturePartSyntax) = Me._pool.AllocateSeparated(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignaturePartSyntax)()
			Dim flag As Boolean = True
			While True
				currentToken = Me.CurrentToken
				If (currentToken.Kind <> SyntaxKind.CloseParenToken AndAlso currentToken.Kind <> SyntaxKind.CommaToken AndAlso Not flag) Then
					currentToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(SyntaxKind.CloseParenToken)
				End If
				If (currentToken.Kind = SyntaxKind.CloseParenToken) Then
					Exit While
				End If
				If (Not flag) Then
					separatedSyntaxListBuilder.AddSeparator(Me.CurrentToken)
					Me.GetNextToken(ScannerState.VB)
				Else
					flag = False
				End If
				Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
				While Me.CurrentToken.Kind = SyntaxKind.ByValKeyword OrElse Me.CurrentToken.Kind = SyntaxKind.ByRefKeyword
					keywordSyntax = If(keywordSyntax IsNot Nothing, keywordSyntax.AddTrailingSyntax(Me.CurrentToken, ERRID.ERR_InvalidParameterSyntax), DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax))
					Me.GetNextToken(ScannerState.VB)
				End While
				Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = Me.ParseGeneralType(False)
				separatedSyntaxListBuilder.Add(Me.SyntaxFactory.CrefSignaturePart(keywordSyntax, typeSyntax))
			End While
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(currentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (Not currentToken.IsMissing) Then
				Me.GetNextToken(ScannerState.VB)
			End If
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignaturePartSyntax) = separatedSyntaxListBuilder.ToList()
			Me._pool.Free(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignaturePartSyntax)(separatedSyntaxListBuilder)
			Return Me.SyntaxFactory.CrefSignature(punctuationSyntax, list, punctuationSyntax1)
		End Function

		Private Function TryParseOptionalWhileOrUntilClause(ByVal precedingKeyword As KeywordSyntax, ByRef optionalWhileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			If (Me.CanFollowStatement(Me.CurrentToken)) Then
				flag = False
			Else
				Dim currentToken As KeywordSyntax = Nothing
				If (Me.CurrentToken.Kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword) Then
					Me.TryTokenAsContextualKeyword(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UntilKeyword, currentToken)
				Else
					currentToken = DirectCast(Me.CurrentToken, KeywordSyntax)
				End If
				If (currentToken Is Nothing) Then
					If (precedingKeyword.Kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoKeyword) Then
						syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileClause
						currentToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword)
					Else
						syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UntilClause
						currentToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UntilKeyword)
					End If
					Dim whileOrUntilClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax = Me.SyntaxFactory.WhileOrUntilClause(syntaxKind1, currentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression())
					optionalWhileOrUntilClause = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax)(Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax)(whileOrUntilClauseSyntax), ERRID.ERR_Syntax)
					flag = True
				Else
					Me.GetNextToken(ScannerState.VB)
					Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ParseExpressionCore(OperatorPrecedence.PrecedenceNone, False)
					If (expressionSyntax.ContainsDiagnostics) Then
						expressionSyntax = Me.ResyncAt(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax)
					End If
					syntaxKind = If(currentToken.Kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UntilClause, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileClause)
					optionalWhileOrUntilClause = Me.SyntaxFactory.WhileOrUntilClause(syntaxKind, currentToken, expressionSyntax)
					flag = True
				End If
			End If
			Return flag
		End Function

		Private Function TryParseXmlCrefAttributeValue(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax, ByVal equals As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, <Out> ByRef crefAttribute As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) As Boolean
			Dim flag As Boolean
			Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax
			Dim scannerState As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax
			Dim crefReferenceSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax
			Dim kind As SyntaxKind
			Dim scannerState1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState
			If (name.Kind = SyntaxKind.XmlName) Then
				xmlNameSyntax = DirectCast(name, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax)
				If (xmlNameSyntax.Prefix IsNot Nothing OrElse Not DocumentationCommentXmlNames.AttributeEquals(xmlNameSyntax.LocalName.Text, "cref")) Then
					flag = False
				Else
					If (Me.CurrentToken.Kind <> SyntaxKind.SingleQuoteToken) Then
						If (Me.CurrentToken.Kind = SyntaxKind.DoubleQuoteToken) Then
							GoTo Label1
						End If
						flag = False
						Return flag
					Else
						scannerState = If(EmbeddedOperators.CompareString(Me.CurrentToken.Text, "'", False) = 0, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState.SingleQuotedString, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState.SmartSingleQuotedString)
						currentToken = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
					End If
				Label5:
					Dim restorePoint As Scanner.RestorePoint = Me._scanner.CreateRestorePoint()
					Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.PeekNextToken(scannerState)
					If (syntaxToken.Kind = SyntaxKind.XmlTextLiteralToken OrElse syntaxToken.Kind = SyntaxKind.XmlEntityLiteralToken) Then
						Dim str As String = syntaxToken.Text.Trim()
						If (str.Length < 2 OrElse EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(str(0)), ":", False) = 0 OrElse EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(str(1)), ":", False) <> 0) Then
							Me.GetNextToken(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState.VB)
							If (Not SyntaxFacts.IsPredefinedTypeKeyword(Me.CurrentToken.Kind)) Then
								crefReferenceSyntax = Me.TryParseCrefReference()
							Else
								Dim predefinedTypeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedTypeSyntax = Me.SyntaxFactory.PredefinedType(DirectCast(Me.CurrentToken, KeywordSyntax))
								Me.GetNextToken(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState.VB)
								crefReferenceSyntax = Me.SyntaxFactory.CrefReference(predefinedTypeSyntax, Nothing, Nothing)
							End If
							If (crefReferenceSyntax IsNot Nothing) Then
								Me.ResetCurrentToken(scannerState)
								While True
									kind = Me.CurrentToken.Kind
									If (kind <= SyntaxKind.DoubleQuoteToken) Then
										GoTo Label2
									End If
									If (kind = SyntaxKind.EndOfFileToken OrElse kind = SyntaxKind.EndOfXmlToken) Then
										Exit While
									End If
									If (CUShort(kind) - CUShort(SyntaxKind.XmlTextLiteralToken) > CUShort(SyntaxKind.List)) Then
										GoTo Label4
									End If
									Dim currentToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.CurrentToken
									If (Parser.TriviaChecker.HasInvalidTrivia(currentToken1)) Then
										GoTo Label4
									End If
									crefReferenceSyntax = crefReferenceSyntax.AddTrailingSyntax(currentToken1)
									crefReferenceSyntax.ClearFlags(GreenNode.NodeFlags.ContainsDiagnostics)
									Me.GetNextToken(scannerState)
								End While
								Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingToken(currentToken.Kind), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
								crefAttribute = Me.SyntaxFactory.XmlCrefAttribute(xmlNameSyntax, equals, currentToken, crefReferenceSyntax, punctuationSyntax)
								flag = True
								Return flag
							End If
						End If
					End If
				Label4:
					restorePoint.Restore()
					Me.ResetCurrentToken(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState.Element)
					flag = False
				End If
			Else
				flag = False
			End If
			Return flag
		Label1:
			scannerState1 = If(EmbeddedOperators.CompareString(Me.CurrentToken.Text, """", False) = 0, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState.QuotedString, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState.SmartQuotedString)
			scannerState = scannerState1
			currentToken = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			GoTo Label5
		Label2:
			If (kind <> SyntaxKind.SingleQuoteToken AndAlso kind <> SyntaxKind.DoubleQuoteToken) Then
				GoTo Label4
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			Me.GetNextToken(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState.Element)
			crefAttribute = Me.SyntaxFactory.XmlCrefAttribute(xmlNameSyntax, equals, currentToken, crefReferenceSyntax, punctuationSyntax1)
			flag = True
			Return flag
		End Function

		Private Function TryParseXmlNameAttributeValue(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax, ByVal equals As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, <Out> ByRef nameAttribute As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax, ByVal xmlElementName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) As Boolean
			Dim flag As Boolean
			Dim scannerState As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax
			Dim scannerState1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState
			If (name.Kind = SyntaxKind.XmlName) Then
				Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax = DirectCast(name, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax)
				If (xmlNameSyntax.Prefix IsNot Nothing OrElse Not DocumentationCommentXmlNames.AttributeEquals(xmlNameSyntax.LocalName.Text, "name")) Then
					flag = False
				ElseIf (Me.ElementNameIsOneFromTheList(xmlElementName, New [String]() { "param", "paramref", "typeparam", "typeparamref" })) Then
					If (Me.CurrentToken.Kind <> SyntaxKind.SingleQuoteToken) Then
						If (Me.CurrentToken.Kind = SyntaxKind.DoubleQuoteToken) Then
							GoTo Label1
						End If
						flag = False
						Return flag
					Else
						scannerState = If(EmbeddedOperators.CompareString(Me.CurrentToken.Text, "'", False) = 0, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState.SingleQuotedString, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState.SmartSingleQuotedString)
						currentToken = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
					End If
				Label3:
					Dim restorePoint As Scanner.RestorePoint = Me._scanner.CreateRestorePoint()
					Me.GetNextToken(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState.VB)
					Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.CurrentToken
					If (syntaxToken.Kind <> SyntaxKind.IdentifierToken) Then
						If (Not syntaxToken.IsKeyword) Then
							GoTo Label2
						End If
						syntaxToken = Me._scanner.MakeIdentifier(DirectCast(Me.CurrentToken, KeywordSyntax))
					End If
					If (Not syntaxToken.ContainsDiagnostics AndAlso Not Parser.TriviaChecker.HasInvalidTrivia(syntaxToken)) Then
						Me.GetNextToken(scannerState)
						Dim currentToken1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.CurrentToken
						If (currentToken1.Kind <> SyntaxKind.SingleQuoteToken AndAlso currentToken1.Kind <> SyntaxKind.DoubleQuoteToken) Then
							GoTo Label2
						End If
						Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
						Me.GetNextToken(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState.Element)
						nameAttribute = Me.SyntaxFactory.XmlNameAttribute(xmlNameSyntax, equals, currentToken, Me.SyntaxFactory.IdentifierName(DirectCast(syntaxToken, IdentifierTokenSyntax)), punctuationSyntax)
						flag = True
						Return flag
					End If
				Label2:
					restorePoint.Restore()
					Me.ResetCurrentToken(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState.Element)
					flag = False
				Else
					flag = False
				End If
			Else
				flag = False
			End If
			Return flag
		Label1:
			scannerState1 = If(EmbeddedOperators.CompareString(Me.CurrentToken.Text, """", False) = 0, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState.QuotedString, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ScannerState.SmartQuotedString)
			scannerState = scannerState1
			currentToken = DirectCast(Me.CurrentToken, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			GoTo Label3
		End Function

		Private Function TryReinterpretAsArraySpecifier(ByVal argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax, ByRef arrayModifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax)) As Boolean
			Dim syntaxListBuilder As SyntaxListBuilder(Of PunctuationSyntax) = Me._pool.Allocate(Of PunctuationSyntax)()
			Dim flag As Boolean = True
			Dim arguments As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax) = argumentList.Arguments
			Dim count As Integer = arguments.Count - 1
			Dim num As Integer = 0
			While num <= count
				If (arguments(num).Kind = SyntaxKind.OmittedArgument) Then
					num = num + 1
				Else
					flag = False
					Exit While
				End If
			End While
			If (flag) Then
				Dim withSeparators As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode) = arguments.GetWithSeparators()
				Dim separatorCount As Integer = arguments.SeparatorCount - 1
				Dim num1 As Integer = 0
				Do
					syntaxListBuilder.Add(DirectCast(withSeparators(2 * num1 + 1), PunctuationSyntax))
					num1 = num1 + 1
				Loop While num1 <= separatorCount
				arrayModifiers = Me.SyntaxFactory.ArrayRankSpecifier(argumentList.OpenParenToken, syntaxListBuilder.ToList(), argumentList.CloseParenToken)
			End If
			Me._pool.Free(syntaxListBuilder)
			Return flag
		End Function

		Private Function TryRejectGenericParametersForMemberDecl(ByRef genericParams As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax) As Boolean
			Dim flag As Boolean
			If (Me.BeginsGeneric(False, False)) Then
				genericParams = Me.ReportGenericParamsDisallowedError(ERRID.ERR_GenericParamsOnInvalidMember)
				flag = True
			Else
				genericParams = Nothing
				flag = False
			End If
			Return flag
		End Function

		Private Function TryTokenAsContextualKeyword(ByVal t As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal kind As SyntaxKind, ByRef k As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax) As Boolean
			Dim flag As Boolean
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = Nothing
			If (Not Me._scanner.TryTokenAsContextualKeyword(t, keywordSyntax) OrElse keywordSyntax.Kind <> kind) Then
				flag = False
			Else
				k = keywordSyntax
				flag = True
			End If
			Return flag
		End Function

		Private Function TryTokenAsContextualKeyword(ByVal t As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByRef k As KeywordSyntax) As Boolean
			Return Me._scanner.TryTokenAsContextualKeyword(t, k)
		End Function

		Private Shared Function TryTokenAsKeyword(ByVal t As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByRef kind As SyntaxKind) As Boolean
			Return Scanner.TryTokenAsKeyword(t, kind)
		End Function

		Private Function ValidateNameOfArgument(ByVal argument As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal isTopLevel As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID
			Dim kind As SyntaxKind = argument.Kind
			If (CUShort(kind) - CUShort(SyntaxKind.MeExpression) > CUShort(SyntaxKind.EmptyStatement)) Then
				If (kind = SyntaxKind.SimpleMemberAccessExpression) Then
					Dim memberAccessExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax = DirectCast(argument, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax)
					If (memberAccessExpressionSyntax.Expression IsNot Nothing) Then
						Dim expressionSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = Me.ValidateNameOfArgument(memberAccessExpressionSyntax.Expression, False)
						If (expressionSyntax2 <> memberAccessExpressionSyntax.Expression) Then
							memberAccessExpressionSyntax = Me.SyntaxFactory.SimpleMemberAccessExpression(expressionSyntax2, memberAccessExpressionSyntax.OperatorToken, memberAccessExpressionSyntax.Name)
						End If
					End If
					expressionSyntax = memberAccessExpressionSyntax
					Return expressionSyntax
				Else
					Select Case kind
						Case SyntaxKind.NullableType
						Case SyntaxKind.PredefinedType
						Case SyntaxKind.GlobalName
							Exit Select
						Case SyntaxKind.IdentifierName
						Case SyntaxKind.GenericName
							expressionSyntax = argument
							Return expressionSyntax
						Case SyntaxKind.QualifiedName
							expressionSyntax1 = argument
							eRRID = If(isTopLevel, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpressionDoesntHaveName, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidNameOfSubExpression)
							expressionSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax1, eRRID)
							Return expressionSyntax
						Case Else
							expressionSyntax1 = argument
							eRRID = If(isTopLevel, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpressionDoesntHaveName, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_InvalidNameOfSubExpression)
							expressionSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(expressionSyntax1, eRRID)
							Return expressionSyntax
					End Select
				End If
			End If
			expressionSyntax = If(Not isTopLevel, argument, Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(argument, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ExpressionDoesntHaveName))
			Return expressionSyntax
		End Function

		Private Function VerifyExpectedToken(Of T As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(ByVal kind As SyntaxKind, ByRef token As T, Optional ByVal state As ScannerState = 0) As Boolean
			Dim flag As Boolean
			Dim currentToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.CurrentToken
			If (currentToken.Kind <> kind) Then
				token = DirectCast(Parser.HandleUnexpectedToken(kind), T)
				flag = False
			Else
				token = DirectCast(currentToken, T)
				Me.GetNextToken(state)
				flag = True
			End If
			Return flag
		End Function

		Private Shared Function VerifyXmlNameToken(ByVal tk As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax
			Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax
			Dim num As Integer
			Dim valueText As String = tk.ValueText
			If (Not [String].IsNullOrEmpty(valueText)) Then
				If (XmlCharacterGlobalHelpers.isStartNameChar(valueText(0))) Then
					Dim str As String = valueText
					num = 0
					While num < str.Length
						Dim chr As Char = str(num)
						If (XmlCharacterGlobalHelpers.isNameChar(chr)) Then
							num = num + 1
						Else
							Dim objArray() As [Object] = { chr, Nothing }
							Dim num1 As Integer = Convert.ToInt32(chr)
							objArray(1) = [String].Concat("0x", num1.ToString("X4"))
							xmlNameTokenSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)(tk, ERRID.ERR_IllegalXmlNameChar, objArray)
							Return xmlNameTokenSyntax
						End If
					End While
				Else
					Dim chr1 As Char = valueText(0)
					Dim objArray1() As [Object] = { chr1, Nothing }
					num = Convert.ToInt32(chr1)
					objArray1(1) = [String].Concat("0x", num.ToString("X4"))
					xmlNameTokenSyntax = Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)(tk, ERRID.ERR_IllegalXmlStartNameChar, objArray1)
					Return xmlNameTokenSyntax
				End If
			End If
			xmlNameTokenSyntax = tk
			Return xmlNameTokenSyntax
		End Function

		Private Enum PossibleFirstStatementKind
			No
			Yes
			IfPrecededByLineBreak
		End Enum

		Private Class TriviaChecker
			Private Sub New()
				MyBase.New()
			End Sub

			Public Shared Function HasInvalidTrivia(ByVal node As GreenNode) As Boolean
				Return Parser.TriviaChecker.SyntaxNodeOrTokenHasInvalidTrivia(node)
			End Function

			Private Shared Function IsInvalidTrivia(ByVal node As GreenNode) As Boolean
				Dim flag As Boolean
				If (node IsNot Nothing) Then
					Dim rawKind As Integer = node.RawKind
					If (rawKind = 1) Then
						Dim slotCount As Integer = node.SlotCount - 1
						Dim num As Integer = 0
						While num <= slotCount
							If (Not Parser.TriviaChecker.IsInvalidTrivia(node.GetSlot(num))) Then
								num = num + 1
							Else
								flag = True
								Return flag
							End If
						End While
					ElseIf (rawKind = 709) Then
						If (Not Parser.TriviaChecker.SyntaxNodeOrTokenHasInvalidTrivia(DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SkippedTokensTriviaSyntax).Tokens.Node)) Then
							flag = False
							Return flag
						End If
						flag = True
						Return flag
					ElseIf (rawKind = 729) Then
						Dim text As String = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia).Text
						Dim num1 As Integer = 0
						While num1 < text.Length
							Dim chr As Char = text(num1)
							If (chr = Strings.ChrW(32) OrElse chr = Strings.ChrW(9)) Then
								num1 = num1 + 1
							Else
								flag = True
								Return flag
							End If
						End While
					Else
						flag = True
						Return flag
					End If
				End If
				flag = False
				Return flag
			End Function

			Private Shared Function SyntaxNodeHasInvalidTrivia(ByVal node As GreenNode) As Boolean
				Dim flag As Boolean
				Dim slotCount As Integer = node.SlotCount - 1
				Dim num As Integer = 0
				While True
					If (num > slotCount) Then
						flag = False
						Exit While
					ElseIf (Not Parser.TriviaChecker.SyntaxNodeOrTokenHasInvalidTrivia(node.GetSlot(num))) Then
						num = num + 1
					Else
						flag = True
						Exit While
					End If
				End While
				Return flag
			End Function

			Private Shared Function SyntaxNodeOrTokenHasInvalidTrivia(ByVal node As GreenNode) As Boolean
				Dim flag As Boolean
				If (node IsNot Nothing) Then
					Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
					If (syntaxToken Is Nothing) Then
						If (Not Parser.TriviaChecker.SyntaxNodeHasInvalidTrivia(node)) Then
							flag = False
							Return flag
						End If
						flag = True
						Return flag
					Else
						If (Not Parser.TriviaChecker.IsInvalidTrivia(syntaxToken.GetLeadingTrivia()) AndAlso Not Parser.TriviaChecker.IsInvalidTrivia(syntaxToken.GetTrailingTrivia())) Then
							flag = False
							Return flag
						End If
						flag = True
						Return flag
					End If
				End If
				flag = False
				Return flag
			End Function
		End Class
	End Class
End Namespace