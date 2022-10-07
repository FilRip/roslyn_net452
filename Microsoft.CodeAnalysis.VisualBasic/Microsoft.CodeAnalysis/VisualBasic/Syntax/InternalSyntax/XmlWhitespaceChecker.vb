Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend Class XmlWhitespaceChecker
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxRewriter
		Private _options As XmlWhitespaceChecker.WhiteSpaceOptions

		Public Sub New()
			MyBase.New()
		End Sub

		Public Overrides Function VisitSyntaxToken(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim leadingTrivia As Microsoft.CodeAnalysis.GreenNode
			Dim trailingTrivia As Microsoft.CodeAnalysis.GreenNode
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			Dim kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax
			If (token IsNot Nothing) Then
				Dim flag As Boolean = False
				leadingTrivia = Nothing
				trailingTrivia = Nothing
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = token.Kind
				If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanSlashToken) Then
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlKeyword OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ColonToken) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List) OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanSlashToken) Then
						GoTo Label1
					End If
					GoTo Label0
				ElseIf (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanQuestionToken AndAlso syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanPercentEqualsToken AndAlso syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNameToken) Then
					GoTo Label0
				End If
			Label1:
				leadingTrivia = token.GetLeadingTrivia()
				If ((Me._options._triviaCheck And XmlWhitespaceChecker.TriviaCheck.ProhibitLeadingTrivia) = XmlWhitespaceChecker.TriviaCheck.ProhibitLeadingTrivia) Then
					syntaxList = MyBase.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(leadingTrivia))
					Dim node As Microsoft.CodeAnalysis.GreenNode = syntaxList.Node
					If (node <> leadingTrivia) Then
						flag = True
						leadingTrivia = node
					End If
				End If
				trailingTrivia = token.GetTrailingTrivia()
				If ((Me._options._triviaCheck And XmlWhitespaceChecker.TriviaCheck.ProhibitTrailingTrivia) = XmlWhitespaceChecker.TriviaCheck.ProhibitTrailingTrivia) Then
					syntaxList = MyBase.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(trailingTrivia))
					Dim greenNode As Microsoft.CodeAnalysis.GreenNode = syntaxList.Node
					If (greenNode <> trailingTrivia) Then
						flag = True
						trailingTrivia = greenNode
					End If
				End If
			Label0:
				If (flag) Then
					kind = token.Kind
					If (kind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanSlashToken) Then
						If (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanQuestionToken OrElse kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanPercentEqualsToken) Then
							punctuationSyntax1 = DirectCast(token, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
							punctuationSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax(punctuationSyntax1.Kind, punctuationSyntax1.GetDiagnostics(), punctuationSyntax1.GetAnnotations(), punctuationSyntax1.Text, leadingTrivia, trailingTrivia)
							Return punctuationSyntax
						End If
						If (kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNameToken) Then
							GoTo Label3
						End If
						Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = DirectCast(token, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)
						punctuationSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax(xmlNameTokenSyntax.Kind, xmlNameTokenSyntax.GetDiagnostics(), xmlNameTokenSyntax.GetAnnotations(), xmlNameTokenSyntax.Text, leadingTrivia, trailingTrivia, xmlNameTokenSyntax.PossibleKeywordKind)
						Return punctuationSyntax
					Else
						If (kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlKeyword) Then
							GoTo Label5
						End If
						Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(token, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
						punctuationSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax(keywordSyntax.Kind, keywordSyntax.GetDiagnostics(), keywordSyntax.GetAnnotations(), keywordSyntax.Text, leadingTrivia, trailingTrivia)
						Return punctuationSyntax
					End If
					punctuationSyntax1 = DirectCast(token, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
					punctuationSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax(punctuationSyntax1.Kind, punctuationSyntax1.GetDiagnostics(), punctuationSyntax1.GetAnnotations(), punctuationSyntax1.Text, leadingTrivia, trailingTrivia)
					Return punctuationSyntax
				End If
			Label3:
				punctuationSyntax = token
			Else
				punctuationSyntax = Nothing
			End If
			Return punctuationSyntax
		Label5:
			If (CUShort(kind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ColonToken) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List) OrElse kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanSlashToken) Then
				punctuationSyntax1 = DirectCast(token, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
				punctuationSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax(punctuationSyntax1.Kind, punctuationSyntax1.GetDiagnostics(), punctuationSyntax1.GetAnnotations(), punctuationSyntax1.Text, leadingTrivia, trailingTrivia)
				Return punctuationSyntax
			End If
			GoTo Label3
		End Function

		Public Overrides Function VisitSyntaxTrivia(ByVal trivia As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Dim syntaxTrivium As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			syntaxTrivium = If(trivia.Kind = SyntaxKind.WhitespaceTrivia OrElse trivia.Kind = SyntaxKind.EndOfLineTrivia, DirectCast(trivia.AddError(ErrorFactory.ErrorInfo(ERRID.ERR_IllegalXmlWhiteSpace)), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia), trivia)
			Return syntaxTrivium
		End Function

		Public Overrides Function VisitXmlAttribute(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim whiteSpaceOption As XmlWhitespaceChecker.WhiteSpaceOptions = Me._options
			Me._options._parentKind = SyntaxKind.XmlAttribute
			Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)
			If (node.Name <> xmlNodeSyntax) Then
				flag = True
			End If
			Me._options = whiteSpaceOption
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlNodeSyntax, node.EqualsToken, node.Value))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlBracketedName(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim whiteSpaceOption As XmlWhitespaceChecker.WhiteSpaceOptions = Me._options
			Me._options._parentKind = SyntaxKind.XmlBracketedName
			Me._options._triviaCheck = XmlWhitespaceChecker.TriviaCheck.ProhibitTrailingTrivia
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.VisitSyntaxToken(node.LessThanToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.LessThanToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)
			If (node.Name <> xmlNodeSyntax) Then
				flag = True
			End If
			Me._options._triviaCheck = XmlWhitespaceChecker.TriviaCheck.ProhibitLeadingTrivia
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.VisitSyntaxToken(node.GreaterThanToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.GreaterThanToken <> punctuationSyntax1) Then
				flag = True
			End If
			Me._options = whiteSpaceOption
			visualBasicSyntaxNode = If(Not flag, node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlBracketedName(punctuationSyntax, DirectCast(xmlNodeSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax), punctuationSyntax1))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlCrefAttribute(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCrefAttributeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim whiteSpaceOption As XmlWhitespaceChecker.WhiteSpaceOptions = Me._options
			Me._options._parentKind = SyntaxKind.XmlCrefAttribute
			Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax)
			If (node.Name <> xmlNameSyntax) Then
				flag = True
			End If
			Me._options = whiteSpaceOption
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCrefAttributeSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlNameSyntax, node.EqualsToken, node.StartQuoteToken, node.Reference, node.EndQuoteToken))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlDeclaration(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim whiteSpaceOption As XmlWhitespaceChecker.WhiteSpaceOptions = Me._options
			Me._options._triviaCheck = XmlWhitespaceChecker.TriviaCheck.ProhibitTrailingTrivia
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.Visit(node.LessThanQuestionToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node._lessThanQuestionToken <> punctuationSyntax) Then
				flag = True
			End If
			Me._options._triviaCheck = XmlWhitespaceChecker.TriviaCheck.ProhibitLeadingTrivia
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(Me.Visit(node.XmlKeyword), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (node._xmlKeyword <> keywordSyntax) Then
				flag = True
			End If
			Me._options = whiteSpaceOption
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), punctuationSyntax, keywordSyntax, node.Version, node.Encoding, node.Standalone, node.QuestionGreaterThanToken))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlElementEndTag(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim whiteSpaceOption As XmlWhitespaceChecker.WhiteSpaceOptions = Me._options
			Me._options._parentKind = SyntaxKind.XmlElementStartTag
			Me._options._triviaCheck = XmlWhitespaceChecker.TriviaCheck.ProhibitTrailingTrivia
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.VisitSyntaxToken(node.LessThanSlashToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.LessThanSlashToken <> punctuationSyntax) Then
				flag = True
			End If
			Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax)
			If (node.Name <> xmlNameSyntax) Then
				flag = True
			End If
			Me._options = whiteSpaceOption
			visualBasicSyntaxNode = If(Not flag, node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlElementEndTag(punctuationSyntax, xmlNameSyntax, node.GreaterThanToken))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlElementStartTag(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim whiteSpaceOption As XmlWhitespaceChecker.WhiteSpaceOptions = Me._options
			Me._options._parentKind = SyntaxKind.XmlElementStartTag
			Me._options._triviaCheck = XmlWhitespaceChecker.TriviaCheck.ProhibitTrailingTrivia
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.VisitSyntaxToken(node.LessThanToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.LessThanToken <> punctuationSyntax) Then
				flag = True
			End If
			Me._options._triviaCheck = XmlWhitespaceChecker.TriviaCheck.ProhibitLeadingTrivia
			Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)
			If (node.Name <> xmlNodeSyntax) Then
				flag = True
			End If
			Me._options._triviaCheck = XmlWhitespaceChecker.TriviaCheck.None
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) = MyBase.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)(node.Attributes)
			If (node.Attributes.Node <> syntaxList.Node) Then
				flag = True
			End If
			Me._options = whiteSpaceOption
			visualBasicSyntaxNode = If(Not flag, node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlElementStartTag(punctuationSyntax, xmlNodeSyntax, syntaxList, node.GreaterThanToken))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlEmptyElement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmptyElementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim whiteSpaceOption As XmlWhitespaceChecker.WhiteSpaceOptions = Me._options
			Me._options._parentKind = SyntaxKind.XmlElementStartTag
			Me._options._triviaCheck = XmlWhitespaceChecker.TriviaCheck.ProhibitTrailingTrivia
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.VisitSyntaxToken(node.LessThanToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.LessThanToken <> punctuationSyntax) Then
				flag = True
			End If
			Me._options._triviaCheck = XmlWhitespaceChecker.TriviaCheck.ProhibitLeadingTrivia
			Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)
			If (node.Name <> xmlNodeSyntax) Then
				flag = True
			End If
			Me._options._triviaCheck = XmlWhitespaceChecker.TriviaCheck.None
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) = MyBase.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)(node.Attributes)
			If (node.Attributes.Node <> syntaxList.Node) Then
				flag = True
			End If
			Me._options = whiteSpaceOption
			visualBasicSyntaxNode = If(Not flag, node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlEmptyElement(punctuationSyntax, xmlNodeSyntax, syntaxList, node.SlashGreaterThanToken))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlName(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim localName As XmlNameTokenSyntax
			Dim flag As Boolean = False
			Dim xmlPrefixSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax = DirectCast(Me.Visit(node.Prefix), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax)
			If (node.Prefix <> xmlPrefixSyntax) Then
				flag = True
			End If
			Dim whiteSpaceOption As XmlWhitespaceChecker.WhiteSpaceOptions = Me._options
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me._options._parentKind
			If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlAttribute) Then
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlBracketedName) Then
					Me._options._triviaCheck = XmlWhitespaceChecker.TriviaCheck.ProhibitLeadingTrivia Or XmlWhitespaceChecker.TriviaCheck.ProhibitTrailingTrivia
					If (Me._options._triviaCheck = XmlWhitespaceChecker.TriviaCheck.None) Then
						localName = node.LocalName
					Else
						localName = DirectCast(Me.VisitSyntaxToken(node.LocalName), XmlNameTokenSyntax)
						If (node.LocalName <> localName) Then
							flag = True
						End If
					End If
					Me._options = whiteSpaceOption
					visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlPrefixSyntax, localName))
					Return visualBasicSyntaxNode
				Else
					If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlCrefAttribute) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
						If (node.Prefix Is Nothing) Then
							Me._options._triviaCheck = XmlWhitespaceChecker.TriviaCheck.None
						Else
							Me._options._triviaCheck = XmlWhitespaceChecker.TriviaCheck.ProhibitLeadingTrivia
						End If
						If (Me._options._triviaCheck = XmlWhitespaceChecker.TriviaCheck.None) Then
							localName = node.LocalName
						Else
							localName = DirectCast(Me.VisitSyntaxToken(node.LocalName), XmlNameTokenSyntax)
							If (node.LocalName <> localName) Then
								flag = True
							End If
						End If
						Me._options = whiteSpaceOption
						visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlPrefixSyntax, localName))
						Return visualBasicSyntaxNode
					End If
					Me._options._triviaCheck = XmlWhitespaceChecker.TriviaCheck.ProhibitLeadingTrivia
					If (Me._options._triviaCheck = XmlWhitespaceChecker.TriviaCheck.None) Then
						localName = node.LocalName
					Else
						localName = DirectCast(Me.VisitSyntaxToken(node.LocalName), XmlNameTokenSyntax)
						If (node.LocalName <> localName) Then
							flag = True
						End If
					End If
					Me._options = whiteSpaceOption
					visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlPrefixSyntax, localName))
					Return visualBasicSyntaxNode
				End If
			End If
			If (node.Prefix Is Nothing) Then
				Me._options._triviaCheck = XmlWhitespaceChecker.TriviaCheck.None
			Else
				Me._options._triviaCheck = XmlWhitespaceChecker.TriviaCheck.ProhibitLeadingTrivia
			End If
			If (Me._options._triviaCheck = XmlWhitespaceChecker.TriviaCheck.None) Then
				localName = node.LocalName
			Else
				localName = DirectCast(Me.VisitSyntaxToken(node.LocalName), XmlNameTokenSyntax)
				If (node.LocalName <> localName) Then
					flag = True
				End If
			End If
			Me._options = whiteSpaceOption
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlPrefixSyntax, localName))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlNameAttribute(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameAttributeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim whiteSpaceOption As XmlWhitespaceChecker.WhiteSpaceOptions = Me._options
			Me._options._parentKind = SyntaxKind.XmlNameAttribute
			Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax = DirectCast(Me.Visit(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax)
			If (node.Name <> xmlNameSyntax) Then
				flag = True
			End If
			Me._options = whiteSpaceOption
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameAttributeSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlNameSyntax, node.EqualsToken, node.StartQuoteToken, node.Reference, node.EndQuoteToken))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlPrefix(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim whiteSpaceOption As XmlWhitespaceChecker.WhiteSpaceOptions = Me._options
			Me._options._triviaCheck = If(Me._options._parentKind = SyntaxKind.XmlAttribute, XmlWhitespaceChecker.TriviaCheck.ProhibitTrailingTrivia, XmlWhitespaceChecker.TriviaCheck.ProhibitLeadingTrivia Or XmlWhitespaceChecker.TriviaCheck.ProhibitTrailingTrivia)
			Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = DirectCast(Me.VisitSyntaxToken(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)
			If (node.Name <> xmlNameTokenSyntax) Then
				flag = True
			End If
			Me._options._triviaCheck = XmlWhitespaceChecker.TriviaCheck.ProhibitLeadingTrivia Or XmlWhitespaceChecker.TriviaCheck.ProhibitTrailingTrivia
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.VisitSyntaxToken(node.ColonToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.ColonToken <> punctuationSyntax) Then
				flag = True
			End If
			Me._options = whiteSpaceOption
			visualBasicSyntaxNode = If(Not flag, node, New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax(node.Kind, node.GetDiagnostics(), node.GetAnnotations(), xmlNameTokenSyntax, punctuationSyntax))
			Return visualBasicSyntaxNode
		End Function

		Public Overrides Function VisitXmlProcessingInstruction(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim xmlProcessingInstructionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim flag As Boolean = False
			Dim whiteSpaceOption As XmlWhitespaceChecker.WhiteSpaceOptions = Me._options
			Me._options._triviaCheck = XmlWhitespaceChecker.TriviaCheck.ProhibitTrailingTrivia
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(Me.VisitSyntaxToken(node.LessThanQuestionToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (node.LessThanQuestionToken <> punctuationSyntax) Then
				flag = True
			End If
			Me._options._triviaCheck = XmlWhitespaceChecker.TriviaCheck.ProhibitLeadingTrivia
			Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = DirectCast(Me.VisitSyntaxToken(node.Name), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)
			If (node.Name <> xmlNameTokenSyntax) Then
				flag = True
			End If
			Me._options = whiteSpaceOption
			If (Not flag) Then
				xmlProcessingInstructionSyntax = node
			Else
				Dim kind As SyntaxKind = node.Kind
				Dim diagnostics As DiagnosticInfo() = node.GetDiagnostics()
				Dim annotations As SyntaxAnnotation() = node.GetAnnotations()
				Dim textTokens As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of XmlTextTokenSyntax) = node.TextTokens
				xmlProcessingInstructionSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax(kind, diagnostics, annotations, punctuationSyntax, xmlNameTokenSyntax, textTokens.Node, node.QuestionGreaterThanToken)
			End If
			Return xmlProcessingInstructionSyntax
		End Function

		<Flags>
		Friend Enum TriviaCheck
			None
			ProhibitLeadingTrivia
			ProhibitTrailingTrivia
		End Enum

		Friend Structure WhiteSpaceOptions
			Friend _parentKind As SyntaxKind

			Friend _triviaCheck As XmlWhitespaceChecker.TriviaCheck
		End Structure
	End Class
End Namespace