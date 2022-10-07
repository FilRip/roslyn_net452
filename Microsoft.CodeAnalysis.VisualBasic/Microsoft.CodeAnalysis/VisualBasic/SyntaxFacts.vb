Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Globalization
Imports System.Linq
Imports System.Reflection
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public Class SyntaxFacts
		Private Const s_fullwidth As Integer = 65248

		Friend Const CHARACTER_TABULATION As Char = Strings.ChrW(9)

		Friend Const LINE_FEED As Char = Strings.ChrW(10)

		Friend Const CARRIAGE_RETURN As Char = Strings.ChrW(13)

		Friend Const SPACE As Char = Strings.ChrW(32)

		Friend Const NO_BREAK_SPACE As Char = Strings.ChrW(160)

		Friend Const IDEOGRAPHIC_SPACE As Char = Strings.ChrW(12288)

		Friend Const LINE_SEPARATOR As Char = Strings.ChrW(8232)

		Friend Const PARAGRAPH_SEPARATOR As Char = Strings.ChrW(8233)

		Friend Const NEXT_LINE As Char = Strings.ChrW(133)

		Friend Const LEFT_SINGLE_QUOTATION_MARK As Char = "‘"C

		Friend Const RIGHT_SINGLE_QUOTATION_MARK As Char = "’"c

		Friend Const LEFT_DOUBLE_QUOTATION_MARK As Char = ChrW(&H201C)

		Friend Const RIGHT_DOUBLE_QUOTATION_MARK As Char = ChrW(&H201D)

		Friend Const FULLWIDTH_APOSTROPHE As Char = ChrW(s_fullwidth + AscW("'"c))

		Friend Const FULLWIDTH_QUOTATION_MARK As Char = ChrW(s_fullwidth + AscW(""""c))

		Friend Const FULLWIDTH_DIGIT_ZERO As Char = "０"C

		Friend Const FULLWIDTH_DIGIT_ONE As Char = "１"C

		Friend Const FULLWIDTH_DIGIT_SEVEN As Char = "７"C

		Friend Const FULLWIDTH_DIGIT_NINE As Char = "９"C

		Friend Const FULLWIDTH_LOW_LINE As Char = Strings.ChrW(65343)

		Friend Const FULLWIDTH_COLON As Char = "："C

		Friend Const FULLWIDTH_SOLIDUS As Char = "／"C

		Friend Const FULLWIDTH_HYPHEN_MINUS As Char = "－"C

		Friend Const FULLWIDTH_PLUS_SIGN As Char = "＋"C

		Friend Const FULLWIDTH_NUMBER_SIGN As Char = "＃"C

		Friend Const FULLWIDTH_EQUALS_SIGN As Char = "＝"C

		Friend Const FULLWIDTH_LESS_THAN_SIGN As Char = "＜"C

		Friend Const FULLWIDTH_GREATER_THAN_SIGN As Char = "＞"C

		Friend Const FULLWIDTH_LEFT_PARENTHESIS As Char = "（"C

		Friend Const FULLWIDTH_LEFT_SQUARE_BRACKET As Char = "［"C

		Friend Const FULLWIDTH_RIGHT_SQUARE_BRACKET As Char = "］"C

		Friend Const FULLWIDTH_LEFT_CURLY_BRACKET As Char = "｛"C

		Friend Const FULLWIDTH_RIGHT_CURLY_BRACKET As Char = "｝"C

		Friend Const FULLWIDTH_AMPERSAND As Char = "＆"C

		Friend Const FULLWIDTH_DOLLAR_SIGN As Char = "＄"C

		Friend Const FULLWIDTH_QUESTION_MARK As Char = "？"C

		Friend Const FULLWIDTH_FULL_STOP As Char = "．"C

		Friend Const FULLWIDTH_COMMA As Char = "，"C

		Friend Const FULLWIDTH_PERCENT_SIGN As Char = "％"C

		Friend Const FULLWIDTH_LATIN_CAPITAL_LETTER_B As Char = "Ｂ"C

		Friend Const FULLWIDTH_LATIN_CAPITAL_LETTER_H As Char = "Ｈ"C

		Friend Const FULLWIDTH_LATIN_CAPITAL_LETTER_O As Char = "Ｏ"C

		Friend Const FULLWIDTH_LATIN_CAPITAL_LETTER_E As Char = "Ｅ"C

		Friend Const FULLWIDTH_LATIN_CAPITAL_LETTER_A As Char = "Ａ"C

		Friend Const FULLWIDTH_LATIN_CAPITAL_LETTER_F As Char = "Ｆ"C

		Friend Const FULLWIDTH_LATIN_CAPITAL_LETTER_C As Char = "Ｃ"C

		Friend Const FULLWIDTH_LATIN_CAPITAL_LETTER_P As Char = "Ｐ"C

		Friend Const FULLWIDTH_LATIN_CAPITAL_LETTER_M As Char = "Ｍ"C

		Friend Const FULLWIDTH_LATIN_SMALL_LETTER_B As Char = "ｂ"C

		Friend Const FULLWIDTH_LATIN_SMALL_LETTER_H As Char = "ｈ"C

		Friend Const FULLWIDTH_LATIN_SMALL_LETTER_O As Char = "ｏ"C

		Friend Const FULLWIDTH_LATIN_SMALL_LETTER_E As Char = "ｅ"C

		Friend Const FULLWIDTH_LATIN_SMALL_LETTER_A As Char = "ａ"C

		Friend Const FULLWIDTH_LATIN_SMALL_LETTER_F As Char = "ｆ"C

		Friend Const FULLWIDTH_LATIN_SMALL_LETTER_C As Char = "ｃ"C

		Friend Const FULLWIDTH_LATIN_SMALL_LETTER_P As Char = "ｐ"C

		Friend Const FULLWIDTH_LATIN_SMALL_LETTER_M As Char = "ｍ"C

		Friend Const FULLWIDTH_LEFT_PARENTHESIS_STRING As String = "（"

		Friend Const FULLWIDTH_RIGHT_PARENTHESIS_STRING As String = "）"

		Friend Const FULLWIDTH_LEFT_CURLY_BRACKET_STRING As String = "｛"

		Friend Const FULLWIDTH_RIGHT_CURLY_BRACKET_STRING As String = "｝"

		Friend Const FULLWIDTH_FULL_STOP_STRING As String = "．"

		Friend Const FULLWIDTH_COMMA_STRING As String = "，"

		Friend Const FULLWIDTH_EQUALS_SIGN_STRING As String = "＝"

		Friend Const FULLWIDTH_PLUS_SIGN_STRING As String = "＋"

		Friend Const FULLWIDTH_HYPHEN_MINUS_STRING As String = "－"

		Friend Const FULLWIDTH_ASTERISK_STRING As String = "＊"

		Friend Const FULLWIDTH_SOLIDUS_STRING As String = "／"

		Friend Const FULLWIDTH_REVERSE_SOLIDUS_STRING As String = "＼"

		Friend Const FULLWIDTH_COLON_STRING As String = "："

		Friend Const FULLWIDTH_CIRCUMFLEX_ACCENT_STRING As String = "＾"

		Friend Const FULLWIDTH_AMPERSAND_STRING As String = "＆"

		Friend Const FULLWIDTH_NUMBER_SIGN_STRING As String = "＃"

		Friend Const FULLWIDTH_EXCLAMATION_MARK_STRING As String = "！"

		Friend Const FULLWIDTH_QUESTION_MARK_STRING As String = "？"

		Friend Const FULLWIDTH_COMMERCIAL_AT_STRING As String = "＠"

		Friend Const FULLWIDTH_LESS_THAN_SIGN_STRING As String = "＜"

		Friend Const FULLWIDTH_GREATER_THAN_SIGN_STRING As String = "＞"

		Private ReadOnly Shared s_isIDChar As Boolean()

		Friend ReadOnly Shared DaysToMonth365 As Integer()

		Friend ReadOnly Shared DaysToMonth366 As Integer()

		Private ReadOnly Shared s_reservedKeywords As SyntaxKind()

		Private ReadOnly Shared s_contextualKeywords As SyntaxKind()

		Private ReadOnly Shared s_punctuationKinds As SyntaxKind()

		Private ReadOnly Shared s_preprocessorKeywords As SyntaxKind()

		Private ReadOnly Shared s_contextualKeywordToSyntaxKindMap As Dictionary(Of String, SyntaxKind)

		Private ReadOnly Shared s_preprocessorKeywordToSyntaxKindMap As Dictionary(Of String, SyntaxKind)

		Public ReadOnly Shared Property EqualityComparer As IEqualityComparer(Of SyntaxKind)

		Shared Sub New()
			SyntaxFacts.s_isIDChar = New [Boolean]() { False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, False, True, True, True, True, True, True, True, True, True, True, False, False, False, False, False, False, False, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, False, False, False, False, True, False, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, True, False, False, False, False, False }
			SyntaxFacts.DaysToMonth365 = New Int32() { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365 }
			SyntaxFacts.DaysToMonth366 = New Int32() { 0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366 }
			SyntaxFacts.EqualityComparer = New SyntaxFacts.SyntaxKindEqualityComparer()
			SyntaxFacts.s_reservedKeywords = New SyntaxKind() {GetType(< PrivateImplementationDetails >).GetField("A416ED6B57862C846BD34C23A062A36B97E312E953DE035F83DBB5B45E96877F").FieldHandle}
			SyntaxFacts.s_contextualKeywords = New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("8CE85D11E66E201354AC77AF934C2D1BF295FB05D51EFBE290D86F2B0F6DAF96").FieldHandle }
			SyntaxFacts.s_punctuationKinds = New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("EBCBAD171D5D772119D5487FBC6B24A004F1190F6F2BD769C3BADA8F5B6499F9").FieldHandle }
			SyntaxFacts.s_preprocessorKeywords = New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("BF389238507231A043B5D241C4DF2257065582705FB60D675B03B60049075199").FieldHandle }
			SyntaxFacts.s_contextualKeywordToSyntaxKindMap = New Dictionary(Of String, SyntaxKind)(CaseInsensitiveComparison.Comparer) From
			{
				{ "aggregate", SyntaxKind.AggregateKeyword },
				{ "all", SyntaxKind.AllKeyword },
				{ "ansi", SyntaxKind.AnsiKeyword },
				{ "ascending", SyntaxKind.AscendingKeyword },
				{ "assembly", SyntaxKind.AssemblyKeyword },
				{ "auto", SyntaxKind.AutoKeyword },
				{ "binary", SyntaxKind.BinaryKeyword },
				{ "by", SyntaxKind.ByKeyword },
				{ "compare", SyntaxKind.CompareKeyword },
				{ "custom", SyntaxKind.CustomKeyword },
				{ "descending", SyntaxKind.DescendingKeyword },
				{ "disable", SyntaxKind.DisableKeyword },
				{ "distinct", SyntaxKind.DistinctKeyword },
				{ "enable", SyntaxKind.EnableKeyword },
				{ "equals", SyntaxKind.EqualsKeyword },
				{ "explicit", SyntaxKind.ExplicitKeyword },
				{ "externalsource", SyntaxKind.ExternalSourceKeyword },
				{ "externalchecksum", SyntaxKind.ExternalChecksumKeyword },
				{ "from", SyntaxKind.FromKeyword },
				{ "group", SyntaxKind.GroupKeyword },
				{ "infer", SyntaxKind.InferKeyword },
				{ "into", SyntaxKind.IntoKeyword },
				{ "isfalse", SyntaxKind.IsFalseKeyword },
				{ "istrue", SyntaxKind.IsTrueKeyword },
				{ "join", SyntaxKind.JoinKeyword },
				{ "key", SyntaxKind.KeyKeyword },
				{ "mid", SyntaxKind.MidKeyword },
				{ "off", SyntaxKind.OffKeyword },
				{ "order", SyntaxKind.OrderKeyword },
				{ "out", SyntaxKind.OutKeyword },
				{ "preserve", SyntaxKind.PreserveKeyword },
				{ "region", SyntaxKind.RegionKeyword },
				{ "r", SyntaxKind.ReferenceKeyword },
				{ "skip", SyntaxKind.SkipKeyword },
				{ "strict", SyntaxKind.StrictKeyword },
				{ "take", SyntaxKind.TakeKeyword },
				{ "text", SyntaxKind.TextKeyword },
				{ "unicode", SyntaxKind.UnicodeKeyword },
				{ "until", SyntaxKind.UntilKeyword },
				{ "warning", SyntaxKind.WarningKeyword },
				{ "where", SyntaxKind.WhereKeyword },
				{ "type", SyntaxKind.TypeKeyword },
				{ "xml", SyntaxKind.XmlKeyword },
				{ "async", SyntaxKind.AsyncKeyword },
				{ "await", SyntaxKind.AwaitKeyword },
				{ "iterator", SyntaxKind.IteratorKeyword },
				{ "yield", SyntaxKind.YieldKeyword }
			}
			SyntaxFacts.s_preprocessorKeywordToSyntaxKindMap = New Dictionary(Of String, SyntaxKind)(CaseInsensitiveComparison.Comparer) From
			{
				{ "if", SyntaxKind.IfKeyword },
				{ "elseif", SyntaxKind.ElseIfKeyword },
				{ "else", SyntaxKind.ElseKeyword },
				{ "endif", SyntaxKind.EndIfKeyword },
				{ "region", SyntaxKind.RegionKeyword },
				{ "end", SyntaxKind.EndKeyword },
				{ "const", SyntaxKind.ConstKeyword },
				{ "externalsource", SyntaxKind.ExternalSourceKeyword },
				{ "externalchecksum", SyntaxKind.ExternalChecksumKeyword },
				{ "r", SyntaxKind.ReferenceKeyword },
				{ "enable", SyntaxKind.EnableKeyword },
				{ "disable", SyntaxKind.DisableKeyword }
			}
		End Sub

		Public Sub New()
			MyBase.New()
		End Sub

		Public Shared Function AllowsLeadingImplicitLineContinuation(ByVal token As SyntaxToken) As Boolean
			Dim parent As Boolean
			If (token.Parent Is Nothing) Then
				Throw New ArgumentException("'token' must be parented by a SyntaxNode.")
			End If
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = token.Kind()
			Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = token.Parent.Kind()
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinKeyword) Then
				If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword) Then
					If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AscendingKeyword) Then
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregateKeyword) Then
							GoTo Label6
						End If
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AscendingKeyword) Then
							parent = True
							Return parent
						End If
						parent = False
						Return parent
					ElseIf (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DescendingKeyword AndAlso syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DistinctKeyword) Then
						Select Case syntaxKind
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FromKeyword
								Exit Select
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GroupKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntoKeyword
								parent = True
								Return parent
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsFalseKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsTrueKeyword
								parent = False
								Return parent
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinKeyword
								parent = syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GroupJoinClause
								Return parent
							Case Else
								parent = False
								Return parent
						End Select
					Else
						parent = True
						Return parent
					End If
				Label6:
					Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = TryCast(token.Parent, QueryClauseSyntax)
					If (visualBasicSyntaxNode IsNot Nothing) Then
						visualBasicSyntaxNode = visualBasicSyntaxNode.Parent
						While visualBasicSyntaxNode IsNot Nothing
							If (Not TypeOf visualBasicSyntaxNode Is QueryExpressionSyntax) Then
								visualBasicSyntaxNode = visualBasicSyntaxNode.Parent
							ElseIf (visualBasicSyntaxNode.GetLocation().SourceSpan.Start >= token.SpanStart) Then
								parent = False
								Return parent
							Else
								parent = True
								Return parent
							End If
						End While
						parent = False
						Return parent
					Else
						parent = False
						Return parent
					End If
				Else
					If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LetKeyword) Then
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword) Then
							parent = TypeOf token.Parent Is QueryClauseSyntax
							Return parent
						End If
						parent = False
						Return parent
					Else
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LetKeyword) Then
							parent = TypeOf token.Parent Is QueryClauseSyntax
							Return parent
						End If
						parent = False
						Return parent
					End If
					parent = TypeOf token.Parent Is QueryClauseSyntax
					Return parent
				End If
			ElseIf (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhereKeyword) Then
				If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseBraceToken) Then
					If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanToken AndAlso syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanToken) Then
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PercentGreaterThanToken) Then
							parent = True
							Return parent
						End If
						parent = False
						Return parent
					End If
					parent = syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributeList
					Return parent
				Else
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseParenToken) Then
						parent = True
						Return parent
					End If
					If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseBraceToken) Then
						parent = False
						Return parent
					End If
					If (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Interpolation) Then
						parent = True
						Return parent
					Else
						parent = False
						Return parent
					End If
				End If
				parent = True
				Return parent
			ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkipKeyword) Then
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrderKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkipKeyword) Then
					parent = True
					Return parent
				End If
			ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TakeKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhereKeyword) Then
				parent = True
				Return parent
			End If
			parent = False
			Return parent
		End Function

		Public Shared Function AllowsTrailingImplicitLineContinuation(ByVal token As SyntaxToken) As Boolean
			Dim parent As Boolean
			If (token.Parent Is Nothing) Then
				Throw New ArgumentException("'token' must be parented by a SyntaxNode.")
			End If
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = token.Kind()
			Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = token.Parent.Kind()
			If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken) Then
				parent = True
			ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MinusToken OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PlusToken) Then
				parent = TypeOf token.Parent Is BinaryExpressionSyntax
			ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanToken AndAlso syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause) Then
				parent = False
			ElseIf (SyntaxFacts.IsBinaryExpressionOperatorToken(syntaxKind) OrElse SyntaxFacts.IsAssignmentStatementOperatorToken(syntaxKind)) Then
				parent = True
			Else
				If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsKeyword) Then
					If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlKeyword) Then
						If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DotToken) Then
							If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken) Then
								parent = True
								Return parent
							End If
							If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken) Then
								If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DotToken) Then
									GoTo Label2
								End If
								If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer) Then
									parent = False
									Return parent
								ElseIf (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleMemberAccessExpression) Then
									parent = If(DirectCast(token.Parent, MemberAccessExpressionSyntax).Expression IsNot Nothing, True, token.Parent.Parent.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer)
									Return parent
								ElseIf (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlElementAccessExpression AndAlso syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlAttributeAccessExpression AndAlso syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlDescendantAccessExpression) Then
									parent = True
									Return parent
								ElseIf (DirectCast(token.Parent, XmlMemberAccessExpressionSyntax).Base Is Nothing) Then
									parent = False
									Return parent
								Else
									parent = token.GetNextToken(False, False, False, False).Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DotToken
									Return parent
								End If
							ElseIf (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Interpolation) Then
								parent = True
								Return parent
							Else
								parent = False
								Return parent
							End If
						ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ColonToken) Then
							parent = syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlPrefix
							Return parent
						ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ColonEqualsToken) Then
							parent = True
							Return parent
						Else
							Select Case syntaxKind
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoubleQuoteToken
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanExclamationMinusMinusToken
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanQuestionToken
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BeginCDataToken
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNameToken
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlTextLiteralToken
									parent = True
									Return parent
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SlashGreaterThanToken
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MinusMinusGreaterThanToken
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QuestionGreaterThanToken
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndCDataToken
									Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax = TryCast(token.Parent.Parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)
									While xmlNodeSyntax IsNot Nothing
										If (xmlNodeSyntax.EndPosition <= token.EndPosition) Then
											xmlNodeSyntax = TryCast(xmlNodeSyntax.Parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)
										Else
											parent = True
											Return parent
										End If
									End While
									parent = False
									Return parent
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanPercentEqualsToken
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PercentGreaterThanToken
									Exit Select
								Case Else
									GoTo Label2
							End Select
						End If
						parent = True
						Return parent
					ElseIf (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkipKeyword) Then
						If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TakeKeyword) Then
							GoTo Label6
						End If
						parent = syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TakeWhileClause
						Return parent
					Else
						Select Case syntaxKind
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FromKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntoKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinKeyword
								parent = True
								Return parent
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GroupKeyword
								parent = syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GroupJoinClause
								Return parent
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsFalseKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsTrueKeyword
								GoTo Label2
							Case Else
								If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkipKeyword) Then
									parent = syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkipWhileClause
									Return parent
								Else
									GoTo Label2
								End If
						End Select
					End If
					parent = True
					Return parent
				Else
					If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword) Then
						If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InKeyword) Then
							If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetXmlNamespaceKeyword) Then
								parent = True
								Return parent
							End If
							If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InKeyword) Then
								GoTo Label2
							End If
							parent = If(syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CollectionRangeVariable, True, syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForEachStatement)
							Return parent
						ElseIf (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LetKeyword) Then
							If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OfKeyword) Then
								parent = True
								Return parent
							End If
							If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword) Then
								parent = TypeOf token.Parent Is QueryClauseSyntax
								Return parent
							End If
							GoTo Label2
						Else
							parent = TypeOf token.Parent Is QueryClauseSyntax
							Return parent
						End If
						parent = True
						Return parent
					ElseIf (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithKeyword) Then
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregateKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsKeyword) Then
							parent = True
							Return parent
						End If
						GoTo Label2
					Else
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword) Then
							parent = TypeOf token.Parent Is QueryClauseSyntax
							Return parent
						End If
						If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithKeyword) Then
							GoTo Label2
						End If
						parent = syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectMemberInitializer
						Return parent
					End If
					parent = TypeOf token.Parent Is QueryClauseSyntax
					Return parent
				End If
			Label2:
				parent = False
			End If
			Return parent
		Label6:
			If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhereKeyword) Then
				parent = True
				Return parent
			End If
			If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlKeyword) Then
				parent = True
				Return parent
			End If
			GoTo Label2
		End Function

		Friend Shared Function BeginOfBlockStatementIfAny(ByVal node As Microsoft.CodeAnalysis.SyntaxNode) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax = Nothing
			Dim statementSyntaxes As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax) = New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)()
			Dim statementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax = Nothing
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = New Microsoft.CodeAnalysis.SyntaxToken()
			If (Not SyntaxFacts.IsBlockStatement(node, statementSyntax, syntaxToken, statementSyntaxes, statementSyntax1) OrElse statementSyntax Is Nothing) Then
				syntaxNode = node
			Else
				syntaxNode = statementSyntax
			End If
			Return syntaxNode
		End Function

		Friend Shared Function BeginsBaseLiteral(ByVal c As Char) As Boolean
			If (c = "H"C Or c = "O"C Or c = "B"C Or c = "h"C Or c = "o"C Or c = "b"C) Then
				Return True
			End If
			Return If(Not SyntaxFacts.IsFullWidth(c), False, c = "Ｈ"C Or c = "ｈ"C) Or c = "Ｏ"C Or c = "ｏ"C Or c = "Ｂ"C Or c = "ｂ"C
		End Function

		Friend Shared Function BeginsExponent(ByVal c As Char) As Boolean
			Return c = "E"C Or c = "e"C Or c = "Ｅ"C Or c = "ｅ"C
		End Function

		Friend Shared Function CanStartSpecifierDeclaration(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionKeyword) Then
				If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateKeyword) Then
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionKeyword) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				Else
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateKeyword) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				End If
			ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword) Then
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword) Then
					flag = True
					Return flag
				End If
				flag = False
				Return flag
			ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword) Then
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword) Then
					flag = True
					Return flag
				End If
				flag = False
				Return flag
			ElseIf (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword AndAlso syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierToken) Then
				flag = False
				Return flag
			End If
			flag = True
			Return flag
		End Function

		Public Shared Function GetAccessorStatementKind(ByVal keyword As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = keyword
			If (syntaxKind1 <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetKeyword) Then
				If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerKeyword) Then
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerStatement
				Else
					If (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetKeyword) Then
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
						Return syntaxKind
					End If
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement
				End If
			ElseIf (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventKeyword) Then
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorStatement
			ElseIf (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerKeyword) Then
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerStatement
			Else
				If (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetKeyword) Then
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
					Return syntaxKind
				End If
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement
			End If
			Return syntaxKind
		End Function

		Public Shared Function GetBaseTypeStatementKind(ByVal keyword As SyntaxKind) As SyntaxKind
			If (keyword = SyntaxKind.EnumKeyword) Then
				Return SyntaxKind.EnumStatement
			End If
			Return SyntaxFacts.GetTypeStatementKind(keyword)
		End Function

		Public Shared Function GetBinaryExpression(ByVal keyword As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = keyword
			If (syntaxKind1 <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModKeyword) Then
				If (syntaxKind1 <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsKeyword) Then
					If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndKeyword) Then
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndExpression
					ElseIf (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndAlsoKeyword) Then
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndAlsoExpression
					Else
						If (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsKeyword) Then
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
							Return syntaxKind
						End If
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsExpression
					End If
				ElseIf (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsNotKeyword) Then
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsNotExpression
				ElseIf (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LikeKeyword) Then
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LikeExpression
				Else
					If (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModKeyword) Then
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
						Return syntaxKind
					End If
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuloExpression
				End If
			ElseIf (syntaxKind1 > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword) Then
				Select Case syntaxKind1
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AmpersandToken
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConcatenateExpression
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndAddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRemoveHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSyncLockStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterSingleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterMultipleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubNewStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeywordEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsPropertyEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesClauseItem Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IncompleteMember Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FieldDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariableDeclarator Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleAsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsNewClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectMemberInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectCollectionInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferredFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionalKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrElseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverloadsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParamArrayKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PartialKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrivateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PublicKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReadOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReDimKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.REMKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ResumeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReturnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShadowsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SharedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShortKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StaticKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StepKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StopKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StringKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ThenKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ThrowKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ToKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TrueKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryCastKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeOfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UIntegerKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ULongKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UShortKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhenKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WideningKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WriteOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GosubKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariantKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WendKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AllKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnsiKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AscendingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AssemblyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AutoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BinaryKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompareKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CustomKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DescendingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DisableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DistinctKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExplicitKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalSourceKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalChecksumKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FromKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GroupKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsFalseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsTrueKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OffKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrderKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OutKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PreserveKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RegionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkipKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StrictKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TakeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TextKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UnicodeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UntilKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WarningKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhereKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsyncKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AwaitKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IteratorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.YieldKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExclamationToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AtToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HashToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AmpersandToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseParenToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseBraceToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SemicolonToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DotToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ColonToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LabelStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsValue Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModifiedIdentifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LabelStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GoToStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParamArrayKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanToken
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
						Return syntaxKind
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsteriskToken
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiplyExpression
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PlusToken
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddExpression
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MinusToken
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubtractExpression
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SlashToken
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DivideExpression
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanToken
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanExpression
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanEqualsToken
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanOrEqualExpression
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanGreaterThanToken
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotEqualsExpression
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsExpression
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanToken
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanExpression
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanEqualsToken
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanOrEqualExpression
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BackslashToken
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntegerDivideExpression
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaretToken
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExponentiateExpression
						Exit Select
					Case Else
						If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanLessThanToken) Then
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LeftShiftExpression
							Exit Select
						ElseIf (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanGreaterThanToken) Then
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RightShiftExpression
							Exit Select
						Else
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
							Return syntaxKind
						End If
				End Select
			ElseIf (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword) Then
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrExpression
			ElseIf (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrElseKeyword) Then
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrElseExpression
			Else
				If (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword) Then
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
					Return syntaxKind
				End If
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExclusiveOrExpression
			End If
			Return syntaxKind
		End Function

		Public Shared Function GetBlockName(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As String
			Dim str As String
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectBlock) Then
				If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineIfBlock) Then
					Select Case syntaxKind
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseIfBlock
							str = "Else If"
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseBlock
							str = "Else"
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IfStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseIfStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Attribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributesStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NextLabel Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueForStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseStatement
							Throw New ArgumentOutOfRangeException("kind")
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryBlock
							str = "Try"
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CatchBlock
							str = "Catch"
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FinallyBlock
							str = "Finally"
							Exit Select
						Case Else
							If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectBlock) Then
								str = "Select"
								Exit Select
							Else
								Throw New ArgumentOutOfRangeException("kind")
							End If
					End Select
				Else
					Select Case syntaxKind
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileBlock
							str = "While"
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayRankSpecifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Attribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributesStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrintStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsValue Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModifiedIdentifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayRankSpecifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributeList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Attribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributeTarget Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributesStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExpressionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrintStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileBlock
							Throw New ArgumentOutOfRangeException("kind")
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock
							str = "Using"
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockBlock
							str = "SyncLock"
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithBlock
							str = "With"
							Exit Select
						Case Else
							If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineIfBlock) Then
								str = "If"
								Exit Select
							Else
								Throw New ArgumentOutOfRangeException("kind")
							End If
					End Select
				End If
			ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForBlock) Then
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseBlock) Then
					str = "Case"
				Else
					If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForBlock) Then
						Throw New ArgumentOutOfRangeException("kind")
					End If
					str = "For"
				End If
			ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForEachBlock) Then
				str = "For Each"
			Else
				If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleDoLoopBlock) > 4) Then
					Throw New ArgumentOutOfRangeException("kind")
				End If
				str = "Do Loop"
			End If
			Return str
		End Function

		Public Shared Function GetContextualKeywordKind(ByVal text As String) As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			text = SyntaxFacts.MakeHalfWidthIdentifier(text)
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
			If (Not SyntaxFacts.s_contextualKeywordToSyntaxKindMap.TryGetValue(text, syntaxKind)) Then
				Return Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
			End If
			Return syntaxKind
		End Function

		Public Shared Function GetContextualKeywordKinds() As IEnumerable(Of SyntaxKind)
			Return SyntaxFacts.s_contextualKeywords
		End Function

		Public Shared Function GetInstanceExpression(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MeKeyword) Then
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MeExpression
			ElseIf (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MyBaseKeyword) Then
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MyBaseExpression
			Else
				syntaxKind = If(syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MyClassKeyword, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MyClassExpression, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None)
			End If
			Return syntaxKind
		End Function

		Public Shared Function GetKeywordKind(ByVal text As String) As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			text = SyntaxFacts.MakeHalfWidthIdentifier(text)
			Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = KeywordTable.TokenOfString(text)
			syntaxKind = If(syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierToken OrElse SyntaxFacts.IsContextualKeyword(syntaxKind1), Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None, syntaxKind1)
			Return syntaxKind
		End Function

		Public Shared Function GetKeywordKinds() As IEnumerable(Of SyntaxKind)
			Return SyntaxFacts.GetReservedKeywordKinds().Concat(SyntaxFacts.GetContextualKeywordKinds())
		End Function

		Public Shared Function GetLiteralExpression(ByVal token As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = token
			If (syntaxKind1 <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword) Then
				If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FalseKeyword) Then
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FalseLiteralExpression
				Else
					If (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword) Then
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
						Return syntaxKind
					End If
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingLiteralExpression
				End If
			ElseIf (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TrueKeyword) Then
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TrueLiteralExpression
			Else
				Select Case syntaxKind1
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntegerLiteralToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FloatingLiteralToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DecimalLiteralToken
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NumericLiteralExpression
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DateLiteralToken
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DateLiteralExpression
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StringLiteralToken
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StringLiteralExpression
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharacterLiteralToken
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharacterLiteralExpression
						Exit Select
					Case Else
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
						Return syntaxKind
				End Select
			End If
			Return syntaxKind
		End Function

		Public Shared Function GetOperatorKind(ByVal operatorMetadataName As String) As SyntaxKind
			Dim operatorInfo As OverloadResolution.OperatorInfo = OverloadResolution.GetOperatorInfo(operatorMetadataName)
			If (operatorInfo.ParamCount = 0) Then
				Return SyntaxKind.None
			End If
			Return OverloadResolution.GetOperatorTokenKind(operatorInfo)
		End Function

		Public Shared Function GetPreprocessorKeywordKind(ByVal text As String) As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			text = SyntaxFacts.MakeHalfWidthIdentifier(text)
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
			If (Not SyntaxFacts.s_preprocessorKeywordToSyntaxKindMap.TryGetValue(text, syntaxKind)) Then
				Return Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
			End If
			Return syntaxKind
		End Function

		Public Shared Function GetPreprocessorKeywordKinds() As IEnumerable(Of SyntaxKind)
			Return SyntaxFacts.s_preprocessorKeywords
		End Function

		Public Shared Function GetPunctuationKinds() As IEnumerable(Of SyntaxKind)
			Return SyntaxFacts.s_punctuationKinds
		End Function

		Public Shared Function GetReservedKeywordKinds() As IEnumerable(Of SyntaxKind)
			Return SyntaxFacts.s_reservedKeywords
		End Function

		Public Shared Function GetText(ByVal kind As SyntaxKind) As String
			Dim empty As String
			Select Case kind
				Case SyntaxKind.AddHandlerKeyword
					empty = "AddHandler"
					Exit Select
				Case SyntaxKind.AddressOfKeyword
					empty = "AddressOf"
					Exit Select
				Case SyntaxKind.AliasKeyword
					empty = "Alias"
					Exit Select
				Case SyntaxKind.AndKeyword
					empty = "And"
					Exit Select
				Case SyntaxKind.AndAlsoKeyword
					empty = "AndAlso"
					Exit Select
				Case SyntaxKind.AsKeyword
					empty = "As"
					Exit Select
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.ConcatenateAssignmentStatement Or SyntaxKind.GetXmlNamespaceExpression Or SyntaxKind.SimpleMemberAccessExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlEmptyElement Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlString Or SyntaxKind.AndKeyword Or SyntaxKind.AndAlsoKeyword Or SyntaxKind.AsKeyword
				Case SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.MidExpression Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlPrefixName Or SyntaxKind.AndKeyword
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.PrintStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineElseClause Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.MidExpression Or SyntaxKind.AddHandlerStatement Or SyntaxKind.RaiseEventStatement Or SyntaxKind.ReDimStatement Or SyntaxKind.RedimClause Or SyntaxKind.GetXmlNamespaceExpression Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.XmlDescendantAccessExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.AnonymousObjectCreationExpression Or SyntaxKind.CollectionInitializer Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlPrefixName Or SyntaxKind.XmlBracketedName Or SyntaxKind.XmlComment Or SyntaxKind.XmlCDataSection Or SyntaxKind.ArrayType Or SyntaxKind.PredefinedType Or SyntaxKind.AndKeyword Or SyntaxKind.AsKeyword Or SyntaxKind.ByRefKeyword Or SyntaxKind.ByValKeyword Or SyntaxKind.CaseKeyword Or SyntaxKind.CBoolKeyword
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueDoStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineIfPart Or SyntaxKind.SingleLineElseClause Or SyntaxKind.MultiLineIfBlock Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.ConcatenateAssignmentStatement Or SyntaxKind.MidExpression Or SyntaxKind.CallStatement Or SyntaxKind.AddHandlerStatement Or SyntaxKind.RemoveHandlerStatement Or SyntaxKind.RaiseEventStatement Or SyntaxKind.WithStatement Or SyntaxKind.ReDimStatement Or SyntaxKind.ReDimPreserveStatement Or SyntaxKind.RedimClause Or SyntaxKind.EraseStatement Or SyntaxKind.GetXmlNamespaceExpression Or SyntaxKind.SimpleMemberAccessExpression Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.XmlElementAccessExpression Or SyntaxKind.XmlDescendantAccessExpression Or SyntaxKind.XmlAttributeAccessExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.ObjectCreationExpression Or SyntaxKind.AnonymousObjectCreationExpression Or SyntaxKind.ArrayCreationExpression Or SyntaxKind.CollectionInitializer Or SyntaxKind.CTypeExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlEmptyElement Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlString Or SyntaxKind.XmlPrefixName Or SyntaxKind.XmlName Or SyntaxKind.XmlBracketedName Or SyntaxKind.XmlPrefix Or SyntaxKind.XmlComment Or SyntaxKind.XmlProcessingInstruction Or SyntaxKind.XmlCDataSection Or SyntaxKind.XmlEmbeddedExpression Or SyntaxKind.ArrayType Or SyntaxKind.NullableType Or SyntaxKind.PredefinedType Or SyntaxKind.IdentifierName Or SyntaxKind.AndKeyword Or SyntaxKind.AndAlsoKeyword Or SyntaxKind.AsKeyword Or SyntaxKind.BooleanKeyword Or SyntaxKind.ByRefKeyword Or SyntaxKind.ByteKeyword Or SyntaxKind.ByValKeyword Or SyntaxKind.CallKeyword Or SyntaxKind.CaseKeyword Or SyntaxKind.CatchKeyword Or SyntaxKind.CBoolKeyword Or SyntaxKind.CByteKeyword
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.ConcatenateAssignmentStatement Or SyntaxKind.NotEqualsExpression Or SyntaxKind.LessThanExpression Or SyntaxKind.LessThanOrEqualExpression Or SyntaxKind.GreaterThanOrEqualExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlEmptyElement Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlString Or SyntaxKind.CTypeKeyword Or SyntaxKind.CUIntKeyword Or SyntaxKind.CULngKeyword
				Case SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.MidExpression Or SyntaxKind.NotEqualsExpression Or SyntaxKind.GreaterThanExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlPrefixName Or SyntaxKind.CTypeKeyword
				Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.NewConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Attribute Or SyntaxKind.UsingBlock Or SyntaxKind.NextLabel Or SyntaxKind.ResumeStatement Or SyntaxKind.CaseEqualsClause Or SyntaxKind.RaiseEventStatement Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.NothingLiteralExpression Or SyntaxKind.NotEqualsExpression Or SyntaxKind.OrExpression Or SyntaxKind.AddressOfExpression Or SyntaxKind.MultiLineSubLambdaExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlComment Or SyntaxKind.GenericName Or SyntaxKind.CrefSignaturePart Or SyntaxKind.CTypeKeyword Or SyntaxKind.DeclareKeyword Or SyntaxKind.ElseKeyword
				Case SyntaxKind.List Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.EventBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.NextLabel Or SyntaxKind.StopStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.CaseEqualsClause Or SyntaxKind.CaseNotEqualsClause Or SyntaxKind.RaiseEventStatement Or SyntaxKind.WithStatement Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.TrueLiteralExpression Or SyntaxKind.NothingLiteralExpression Or SyntaxKind.ParenthesizedExpression Or SyntaxKind.NotEqualsExpression Or SyntaxKind.LessThanExpression Or SyntaxKind.OrExpression Or SyntaxKind.ExclusiveOrExpression Or SyntaxKind.AddressOfExpression Or SyntaxKind.BinaryConditionalExpression Or SyntaxKind.MultiLineSubLambdaExpression Or SyntaxKind.SubLambdaHeader Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlEmptyElement Or SyntaxKind.XmlComment Or SyntaxKind.XmlProcessingInstruction Or SyntaxKind.GenericName Or SyntaxKind.QualifiedName Or SyntaxKind.CrefSignaturePart Or SyntaxKind.CrefOperatorReference Or SyntaxKind.CTypeKeyword Or SyntaxKind.CUIntKeyword Or SyntaxKind.DeclareKeyword Or SyntaxKind.DefaultKeyword Or SyntaxKind.ElseKeyword Or SyntaxKind.ElseIfKeyword
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.OptionStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineElseClause Or SyntaxKind.MultiLineIfBlock Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.SelectStatement Or SyntaxKind.ForBlock Or SyntaxKind.MidExpression Or SyntaxKind.CallStatement Or SyntaxKind.RaiseEventStatement Or SyntaxKind.WithStatement Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.XmlElementAccessExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.ObjectCreationExpression Or SyntaxKind.ArrayCreationExpression Or SyntaxKind.NotEqualsExpression Or SyntaxKind.LessThanExpression Or SyntaxKind.GreaterThanExpression Or SyntaxKind.IsExpression Or SyntaxKind.OrExpression Or SyntaxKind.ExclusiveOrExpression Or SyntaxKind.AndAlsoExpression Or SyntaxKind.UnaryPlusExpression Or SyntaxKind.QueryExpression Or SyntaxKind.CollectionRangeVariable Or SyntaxKind.VariableNameEquals Or SyntaxKind.FunctionAggregation Or SyntaxKind.LetClause Or SyntaxKind.AggregateClause Or SyntaxKind.SkipWhileClause Or SyntaxKind.TakeWhileClause Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlEmptyElement Or SyntaxKind.XmlPrefixName Or SyntaxKind.XmlName Or SyntaxKind.XmlComment Or SyntaxKind.XmlProcessingInstruction Or SyntaxKind.ArrayType Or SyntaxKind.NullableType Or SyntaxKind.AndKeyword Or SyntaxKind.AndAlsoKeyword Or SyntaxKind.BooleanKeyword Or SyntaxKind.ByValKeyword Or SyntaxKind.CallKeyword Or SyntaxKind.CBoolKeyword Or SyntaxKind.CByteKeyword Or SyntaxKind.CTypeKeyword Or SyntaxKind.CUIntKeyword Or SyntaxKind.CUShortKeyword Or SyntaxKind.DeclareKeyword Or SyntaxKind.DefaultKeyword Or SyntaxKind.DirectCastKeyword Or SyntaxKind.DoKeyword Or SyntaxKind.GetTypeKeyword Or SyntaxKind.GetXmlNamespaceKeyword Or SyntaxKind.HandlesKeyword Or SyntaxKind.IfKeyword Or SyntaxKind.InKeyword Or SyntaxKind.InheritsKeyword Or SyntaxKind.IsKeyword
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.EventStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.PrintStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineElseClause Or SyntaxKind.FinallyStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectStatement Or SyntaxKind.SyncLockStatement Or SyntaxKind.WhileStatement Or SyntaxKind.ForEachBlock Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.MidExpression Or SyntaxKind.AddHandlerStatement Or SyntaxKind.RaiseEventStatement Or SyntaxKind.ReDimStatement Or SyntaxKind.RedimClause Or SyntaxKind.GetXmlNamespaceExpression Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.XmlDescendantAccessExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.AnonymousObjectCreationExpression Or SyntaxKind.CollectionInitializer Or SyntaxKind.NotEqualsExpression Or SyntaxKind.LessThanOrEqualExpression Or SyntaxKind.GreaterThanExpression Or SyntaxKind.IsNotExpression Or SyntaxKind.OrExpression Or SyntaxKind.AndExpression Or SyntaxKind.AndAlsoExpression Or SyntaxKind.UnaryMinusExpression Or SyntaxKind.QueryExpression Or SyntaxKind.ExpressionRangeVariable Or SyntaxKind.VariableNameEquals Or SyntaxKind.GroupAggregation Or SyntaxKind.LetClause Or SyntaxKind.DistinctClause Or SyntaxKind.SkipWhileClause Or SyntaxKind.SkipClause Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlPrefixName Or SyntaxKind.XmlBracketedName Or SyntaxKind.XmlComment Or SyntaxKind.XmlCDataSection Or SyntaxKind.ArrayType Or SyntaxKind.PredefinedType Or SyntaxKind.AndKeyword Or SyntaxKind.AsKeyword Or SyntaxKind.ByRefKeyword Or SyntaxKind.ByValKeyword Or SyntaxKind.CaseKeyword Or SyntaxKind.CBoolKeyword Or SyntaxKind.CTypeKeyword Or SyntaxKind.CULngKeyword Or SyntaxKind.DateKeyword Or SyntaxKind.DeclareKeyword Or SyntaxKind.DelegateKeyword Or SyntaxKind.DirectCastKeyword Or SyntaxKind.DoubleKeyword Or SyntaxKind.GetTypeKeyword Or SyntaxKind.GlobalKeyword Or SyntaxKind.HandlesKeyword Or SyntaxKind.ImplementsKeyword Or SyntaxKind.InKeyword Or SyntaxKind.IntegerKeyword Or SyntaxKind.IsKeyword
				Case SyntaxKind.EmptyStatement Or SyntaxKind.NotKeyword
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.EndRemoveHandlerStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PrivateKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.PublicKeyword
				Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.NotKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OverridesKeyword
				Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.StepKeyword
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.OptionStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OrElseKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword
				Case 576
				Case SyntaxKind.List Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.AddHandlerAccessorBlock Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.DistinctKeyword
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.FunctionBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.RemoveHandlerAccessorBlock Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.PublicKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.DistinctKeyword
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.NewConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.NotKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.PreserveKeyword
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.AddHandlerAccessorStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.StructureKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AscendingKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.RegionKeyword
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.EndRemoveHandlerStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.EndSyncLockStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.ModuleBlock Or SyntaxKind.StructureBlock Or SyntaxKind.InterfaceBlock Or SyntaxKind.ClassBlock Or SyntaxKind.EnumBlock Or SyntaxKind.InheritsStatement Or SyntaxKind.ImplementsStatement Or SyntaxKind.ModuleStatement Or SyntaxKind.StructureStatement Or SyntaxKind.InterfaceStatement Or SyntaxKind.ClassStatement Or SyntaxKind.EnumStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.SubBlock Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.GetAccessorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.AddHandlerAccessorBlock Or SyntaxKind.RemoveHandlerAccessorBlock Or SyntaxKind.RaiseEventAccessorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.EventBlock Or SyntaxKind.ParameterList Or SyntaxKind.SubStatement Or SyntaxKind.FunctionStatement Or SyntaxKind.SubNewStatement Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.EventStatement Or SyntaxKind.OperatorStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.AddHandlerAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.RaiseEventAccessorStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.HandlesClause Or SyntaxKind.KeywordEventContainer Or SyntaxKind.WithEventsEventContainer Or SyntaxKind.WithEventsPropertyEventContainer Or SyntaxKind.HandlesClauseItem Or SyntaxKind.IncompleteMember Or SyntaxKind.FieldDeclaration Or SyntaxKind.VariableDeclarator Or SyntaxKind.SimpleAsClause Or SyntaxKind.AsNewClause Or SyntaxKind.ObjectMemberInitializer Or SyntaxKind.ObjectCollectionInitializer Or SyntaxKind.InferredFieldInitializer Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OrElseKeyword Or SyntaxKind.OverloadsKeyword Or SyntaxKind.OverridableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PrivateKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.PublicKeyword Or SyntaxKind.RaiseEventKeyword Or SyntaxKind.ReadOnlyKeyword Or SyntaxKind.ReDimKeyword Or SyntaxKind.REMKeyword Or SyntaxKind.RemoveHandlerKeyword Or SyntaxKind.ResumeKeyword Or SyntaxKind.ReturnKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.StaticKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.StructureKeyword Or SyntaxKind.SubKeyword Or SyntaxKind.SyncLockKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ThrowKeyword Or SyntaxKind.ToKeyword Or SyntaxKind.TrueKeyword Or SyntaxKind.TryKeyword Or SyntaxKind.TryCastKeyword Or SyntaxKind.TypeOfKeyword Or SyntaxKind.UIntegerKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UShortKeyword Or SyntaxKind.UsingKeyword Or SyntaxKind.WhenKeyword Or SyntaxKind.WhileKeyword Or SyntaxKind.WideningKeyword Or SyntaxKind.WithKeyword Or SyntaxKind.WithEventsKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AscendingKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.AutoKeyword Or SyntaxKind.BinaryKeyword Or SyntaxKind.ByKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.DisableKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.EnableKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.ExplicitKeyword Or SyntaxKind.ExternalSourceKeyword Or SyntaxKind.ExternalChecksumKeyword Or SyntaxKind.FromKeyword Or SyntaxKind.GroupKeyword Or SyntaxKind.InferKeyword Or SyntaxKind.IntoKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OffKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OutKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.RegionKeyword Or SyntaxKind.SkipKeyword Or SyntaxKind.StrictKeyword Or SyntaxKind.TakeKeyword Or SyntaxKind.TextKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.UntilKeyword Or SyntaxKind.WarningKeyword Or SyntaxKind.WhereKeyword Or SyntaxKind.TypeKeyword Or SyntaxKind.XmlKeyword Or SyntaxKind.AsyncKeyword Or SyntaxKind.AwaitKeyword Or SyntaxKind.IteratorKeyword Or SyntaxKind.YieldKeyword Or SyntaxKind.ExclamationToken Or SyntaxKind.AtToken Or SyntaxKind.CommaToken Or SyntaxKind.HashToken Or SyntaxKind.AmpersandToken
				Case SyntaxKind.NamedFieldInitializer Or SyntaxKind.NotKeyword
				Case SyntaxKind.EndFunctionStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.UsingBlock Or SyntaxKind.LabelStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.OpenBraceToken Or SyntaxKind.EqualsToken
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.LabelStatement Or SyntaxKind.GoToStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken
				Case SyntaxKind.EndOfFileToken
				Case SyntaxKind.EmptyToken
				Case SyntaxKind.List Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.OptionStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.SlashGreaterThanToken
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.NotKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.OpenParenToken Or SyntaxKind.PlusToken Or SyntaxKind.DotToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.SlashGreaterThanToken
				Case SyntaxKind.EndOfXmlToken
				Case SyntaxKind.BadToken
				Case SyntaxKind.XmlNameToken
				Case SyntaxKind.XmlTextLiteralToken
				Case SyntaxKind.XmlEntityLiteralToken
				Case SyntaxKind.DocumentationCommentLineBreakToken
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.ModuleBlock Or SyntaxKind.ImplementsStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.UsingBlock Or SyntaxKind.WithBlock Or SyntaxKind.NextLabel Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.ElseStatement Or SyntaxKind.NotKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.ReadOnlyKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ToKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UsingKeyword Or SyntaxKind.OpenParenToken Or SyntaxKind.PlusToken Or SyntaxKind.DotToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.PlusEqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.BeginCDataToken Or SyntaxKind.XmlEntityLiteralToken
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.EndSyncLockStatement Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.ModuleBlock Or SyntaxKind.StructureBlock Or SyntaxKind.InheritsStatement Or SyntaxKind.ImplementsStatement Or SyntaxKind.ModuleStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.WithBlock Or SyntaxKind.LocalDeclarationStatement Or SyntaxKind.NextLabel Or SyntaxKind.StopStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineIfPart Or SyntaxKind.ElseStatement Or SyntaxKind.TryBlock Or SyntaxKind.CatchBlock Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PrivateKeyword Or SyntaxKind.RaiseEventKeyword Or SyntaxKind.ReadOnlyKeyword Or SyntaxKind.ReDimKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.StructureKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ThrowKeyword Or SyntaxKind.ToKeyword Or SyntaxKind.TrueKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UShortKeyword Or SyntaxKind.UsingKeyword Or SyntaxKind.WhenKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.DotToken Or SyntaxKind.SlashToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.BackslashToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.AsteriskEqualsToken Or SyntaxKind.PlusEqualsToken Or SyntaxKind.MinusEqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.QuestionToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanSlashToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.PercentGreaterThanToken Or SyntaxKind.BeginCDataToken Or SyntaxKind.EndCDataToken Or SyntaxKind.XmlEntityLiteralToken Or SyntaxKind.DocumentationCommentLineBreakToken
				Case SyntaxKind.IdentifierToken
				Case SyntaxKind.IntegerLiteralToken
				Case SyntaxKind.FloatingLiteralToken
				Case SyntaxKind.DecimalLiteralToken
				Case SyntaxKind.DateLiteralToken
				Case SyntaxKind.StringLiteralToken
				Case SyntaxKind.CharacterLiteralToken
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken
				Case SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.OpenBraceToken Or SyntaxKind.DateLiteralToken
				Case SyntaxKind.SkippedTokensTrivia
				Case SyntaxKind.DocumentationCommentTrivia
				Case SyntaxKind.XmlCrefAttribute
				Case SyntaxKind.XmlNameAttribute
				Case SyntaxKind.ConditionalAccessExpression
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.NewConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.NotKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.OpenParenToken Or SyntaxKind.PlusToken Or SyntaxKind.DotToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.XmlNameAttribute
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectBlock Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AscendingKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.DotToken Or SyntaxKind.SlashToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression
				Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.NewConstraint Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.Attribute Or SyntaxKind.PrintStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.SelectStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.OpenBraceToken Or SyntaxKind.PlusToken Or SyntaxKind.ColonToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.XmlNameAttribute
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.SelectStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OrElseKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.AutoKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.ColonToken Or SyntaxKind.LessThanToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.PrintStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OverloadsKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.BinaryKeyword Or SyntaxKind.OpenParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.PlusToken Or SyntaxKind.DotToken Or SyntaxKind.ColonToken Or SyntaxKind.LessThanEqualsToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlNameAttribute
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.SubBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.OnErrorResumeNextStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectBlock Or SyntaxKind.SelectStatement Or SyntaxKind.CaseBlock Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OrElseKeyword Or SyntaxKind.OverloadsKeyword Or SyntaxKind.OverridableKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AscendingKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.AutoKeyword Or SyntaxKind.BinaryKeyword Or SyntaxKind.ByKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.AsteriskToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.DotToken Or SyntaxKind.SlashToken Or SyntaxKind.ColonToken Or SyntaxKind.LessThanToken Or SyntaxKind.LessThanEqualsToken Or SyntaxKind.LessThanGreaterThanToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlCrefAttribute Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression
				Case SyntaxKind.EndFunctionStatement Or SyntaxKind.FunctionBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.UsingBlock Or SyntaxKind.NotKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.EqualsToken Or SyntaxKind.DateLiteralToken
				Case SyntaxKind.List Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.FunctionBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.UsingBlock Or SyntaxKind.WithBlock Or SyntaxKind.FinallyStatement Or SyntaxKind.CaseElseBlock Or SyntaxKind.NotKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.OpenParenToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.CharacterLiteralToken
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.GetAccessorBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.WithBlock Or SyntaxKind.LocalDeclarationStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.CaseElseBlock Or SyntaxKind.CaseStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PrivateKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.DisableKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.BackslashToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken
				Case SyntaxKind.EndFunctionStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.FunctionBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.UsingBlock Or SyntaxKind.LabelStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.CaseElseStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.OpenBraceToken Or SyntaxKind.EqualsToken Or SyntaxKind.DateLiteralToken
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.AddHandlerAccessorBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.LabelStatement Or SyntaxKind.GoToStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.CaseElseStatement Or SyntaxKind.ElseCaseClause Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.SkippedTokensTrivia
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.FunctionBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.RemoveHandlerAccessorBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.UsingBlock Or SyntaxKind.WithBlock Or SyntaxKind.LabelStatement Or SyntaxKind.IdentifierLabel Or SyntaxKind.FinallyStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.CaseElseBlock Or SyntaxKind.CaseElseStatement Or SyntaxKind.SimpleCaseClause Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.PublicKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.OpenParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.CaretToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.DocumentationCommentTrivia
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.EndRemoveHandlerStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.GetAccessorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.AddHandlerAccessorBlock Or SyntaxKind.RemoveHandlerAccessorBlock Or SyntaxKind.RaiseEventAccessorBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.WithBlock Or SyntaxKind.LocalDeclarationStatement Or SyntaxKind.LabelStatement Or SyntaxKind.GoToStatement Or SyntaxKind.IdentifierLabel Or SyntaxKind.NumericLabel Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.OnErrorResumeNextStatement Or SyntaxKind.CaseElseBlock Or SyntaxKind.CaseStatement Or SyntaxKind.CaseElseStatement Or SyntaxKind.ElseCaseClause Or SyntaxKind.SimpleCaseClause Or SyntaxKind.RangeCaseClause Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PrivateKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.PublicKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.DisableKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.EnableKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.AsteriskToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.BackslashToken Or SyntaxKind.CaretToken Or SyntaxKind.ColonEqualsToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlCrefAttribute
				Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.NewConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Attribute Or SyntaxKind.UsingBlock Or SyntaxKind.NextLabel Or SyntaxKind.ResumeStatement Or SyntaxKind.CaseEqualsClause Or SyntaxKind.NotKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.PlusToken Or SyntaxKind.EqualsToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.XmlNameAttribute
				Case SyntaxKind.WhitespaceTrivia
				Case SyntaxKind.EndOfLineTrivia
				Case SyntaxKind.CommentTrivia
				Case SyntaxKind.DisabledTextTrivia
				Case SyntaxKind.ConstDirectiveTrivia
				Case SyntaxKind.IfDirectiveTrivia
				Case SyntaxKind.ElseIfDirectiveTrivia
				Case SyntaxKind.ElseDirectiveTrivia
				Case SyntaxKind.EndIfDirectiveTrivia
				Case SyntaxKind.RegionDirectiveTrivia
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.EventStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.SyncLockStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OpenParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.EventStatement Or SyntaxKind.OperatorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueDoStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.OnErrorResumeNextStatement Or SyntaxKind.SyncLockStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.StaticKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OffKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OutKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.AsteriskToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.QuestionToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.StatementTerminatorToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.EmptyToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlCrefAttribute Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.IfDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.ElseDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia Or SyntaxKind.RegionDirectiveTrivia
				Case SyntaxKind.EndRegionDirectiveTrivia
				Case SyntaxKind.ExternalSourceDirectiveTrivia
				Case SyntaxKind.EndExternalSourceDirectiveTrivia
				Case SyntaxKind.ExternalChecksumDirectiveTrivia
				Case SyntaxKind.EnableWarningDirectiveTrivia
				Case SyntaxKind.DisableWarningDirectiveTrivia
				Case SyntaxKind.ReferenceDirectiveTrivia
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.SubBlock Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.EventStatement Or SyntaxKind.OperatorStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.AddHandlerAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.RaiseEventAccessorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueDoStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineIfPart Or SyntaxKind.SingleLineElseClause Or SyntaxKind.MultiLineIfBlock Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.OnErrorResumeNextStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectBlock Or SyntaxKind.SelectStatement Or SyntaxKind.CaseBlock Or SyntaxKind.SyncLockStatement Or SyntaxKind.WhileStatement Or SyntaxKind.ForBlock Or SyntaxKind.ForEachBlock Or SyntaxKind.ForStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OrElseKeyword Or SyntaxKind.OverloadsKeyword Or SyntaxKind.OverridableKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.StaticKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.StructureKeyword Or SyntaxKind.SubKeyword Or SyntaxKind.SyncLockKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AscendingKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.AutoKeyword Or SyntaxKind.BinaryKeyword Or SyntaxKind.ByKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OffKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OutKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.RegionKeyword Or SyntaxKind.SkipKeyword Or SyntaxKind.StrictKeyword Or SyntaxKind.TakeKeyword Or SyntaxKind.TextKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.AsteriskToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.DotToken Or SyntaxKind.SlashToken Or SyntaxKind.ColonToken Or SyntaxKind.LessThanToken Or SyntaxKind.LessThanEqualsToken Or SyntaxKind.LessThanGreaterThanToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.QuestionToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.StatementTerminatorToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.EmptyToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanSlashToken Or SyntaxKind.LessThanExclamationMinusMinusToken Or SyntaxKind.MinusMinusGreaterThanToken Or SyntaxKind.LessThanQuestionToken Or SyntaxKind.QuestionGreaterThanToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlCrefAttribute Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.IfDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.ElseDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia Or SyntaxKind.RegionDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.ExternalSourceDirectiveTrivia Or SyntaxKind.EndExternalSourceDirectiveTrivia Or SyntaxKind.ExternalChecksumDirectiveTrivia Or SyntaxKind.EnableWarningDirectiveTrivia Or SyntaxKind.DisableWarningDirectiveTrivia Or SyntaxKind.ReferenceDirectiveTrivia
				Case SyntaxKind.EndFunctionStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.FunctionBlock Or SyntaxKind.DeclareSubStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.UsingBlock Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ForStepClause Or SyntaxKind.NotKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.ConstDirectiveTrivia
				Case SyntaxKind.BadDirectiveTrivia
				Case SyntaxKind.ImportAliasClause
				Case SyntaxKind.NameColonEquals
				Case SyntaxKind.SimpleDoLoopBlock
				Case SyntaxKind.DoWhileLoopBlock
				Case SyntaxKind.DoUntilLoopBlock
				Case SyntaxKind.DoLoopWhileBlock
				Case SyntaxKind.DoLoopUntilBlock
				Case SyntaxKind.List Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.OptionStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.InheritsStatement Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.EventBlock Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.HandlesClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.NextLabel Or SyntaxKind.StopStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.ElseStatement Or SyntaxKind.TryBlock Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.CaseEqualsClause Or SyntaxKind.CaseNotEqualsClause Or SyntaxKind.ForStepClause Or SyntaxKind.ForEachStatement Or SyntaxKind.MidAssignmentStatement Or SyntaxKind.AddAssignmentStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.RaiseEventKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ThrowKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UShortKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.ExplicitKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.RegionKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.UntilKeyword Or SyntaxKind.IteratorKeyword Or SyntaxKind.YieldKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.AsteriskEqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.PercentGreaterThanToken Or SyntaxKind.XmlEntityLiteralToken Or SyntaxKind.DocumentationCommentLineBreakToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression Or SyntaxKind.WhitespaceTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.IfDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.ExternalSourceDirectiveTrivia Or SyntaxKind.BadDirectiveTrivia Or SyntaxKind.DoLoopUntilBlock
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.ModuleBlock Or SyntaxKind.ImplementsStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.NewConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.KeywordEventContainer Or SyntaxKind.VariableDeclarator Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.UsingBlock Or SyntaxKind.WithBlock Or SyntaxKind.NextLabel Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.ElseStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.CaseElseBlock Or SyntaxKind.CaseEqualsClause Or SyntaxKind.CaseLessThanClause Or SyntaxKind.SyncLockStatement Or SyntaxKind.WhileStatement Or SyntaxKind.ForStepClause Or SyntaxKind.NextStatement Or SyntaxKind.MidAssignmentStatement Or SyntaxKind.SubtractAssignmentStatement Or SyntaxKind.NotKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.ReadOnlyKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ToKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UsingKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.ExternalSourceKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.WarningKeyword Or SyntaxKind.IteratorKeyword Or SyntaxKind.ExclamationToken Or SyntaxKind.OpenParenToken Or SyntaxKind.PlusToken Or SyntaxKind.DotToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.PlusEqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.BeginCDataToken Or SyntaxKind.XmlEntityLiteralToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.XmlNameAttribute Or SyntaxKind.EndOfLineTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.EndExternalSourceDirectiveTrivia Or SyntaxKind.ImportAliasClause Or SyntaxKind.DoLoopUntilBlock
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.EndSyncLockStatement Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.ModuleBlock Or SyntaxKind.StructureBlock Or SyntaxKind.InheritsStatement Or SyntaxKind.ImplementsStatement Or SyntaxKind.ModuleStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.GetAccessorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.EventBlock Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.AddHandlerAccessorStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.HandlesClause Or SyntaxKind.KeywordEventContainer Or SyntaxKind.WithEventsEventContainer Or SyntaxKind.VariableDeclarator Or SyntaxKind.SimpleAsClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.WithBlock Or SyntaxKind.LocalDeclarationStatement Or SyntaxKind.NextLabel Or SyntaxKind.StopStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineIfPart Or SyntaxKind.ElseStatement Or SyntaxKind.TryBlock Or SyntaxKind.CatchBlock Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectBlock Or SyntaxKind.CaseElseBlock Or SyntaxKind.CaseStatement Or SyntaxKind.CaseEqualsClause Or SyntaxKind.CaseNotEqualsClause Or SyntaxKind.CaseLessThanClause Or SyntaxKind.CaseLessThanOrEqualClause Or SyntaxKind.SyncLockStatement Or SyntaxKind.WhileStatement Or SyntaxKind.ForStepClause Or SyntaxKind.ForEachStatement Or SyntaxKind.NextStatement Or SyntaxKind.UsingStatement Or SyntaxKind.MidAssignmentStatement Or SyntaxKind.AddAssignmentStatement Or SyntaxKind.SubtractAssignmentStatement Or SyntaxKind.MultiplyAssignmentStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PrivateKeyword Or SyntaxKind.RaiseEventKeyword Or SyntaxKind.ReadOnlyKeyword Or SyntaxKind.ReDimKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.StructureKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ThrowKeyword Or SyntaxKind.ToKeyword Or SyntaxKind.TrueKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UShortKeyword Or SyntaxKind.UsingKeyword Or SyntaxKind.WhenKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AscendingKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.DisableKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.ExplicitKeyword Or SyntaxKind.ExternalSourceKeyword Or SyntaxKind.ExternalChecksumKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.RegionKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.UntilKeyword Or SyntaxKind.WarningKeyword Or SyntaxKind.WhereKeyword Or SyntaxKind.IteratorKeyword Or SyntaxKind.YieldKeyword Or SyntaxKind.ExclamationToken Or SyntaxKind.AtToken Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.DotToken Or SyntaxKind.SlashToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.BackslashToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.AsteriskEqualsToken Or SyntaxKind.PlusEqualsToken Or SyntaxKind.MinusEqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.QuestionToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanSlashToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.PercentGreaterThanToken Or SyntaxKind.BeginCDataToken Or SyntaxKind.EndCDataToken Or SyntaxKind.XmlEntityLiteralToken Or SyntaxKind.DocumentationCommentLineBreakToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression Or SyntaxKind.WhitespaceTrivia Or SyntaxKind.EndOfLineTrivia Or SyntaxKind.ColonTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.IfDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.ElseDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.ExternalSourceDirectiveTrivia Or SyntaxKind.EndExternalSourceDirectiveTrivia Or SyntaxKind.ExternalChecksumDirectiveTrivia Or SyntaxKind.BadDirectiveTrivia Or SyntaxKind.ImportAliasClause Or SyntaxKind.NameColonEquals Or SyntaxKind.DoLoopUntilBlock
				Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.NamespaceBlock Or SyntaxKind.InterfaceBlock Or SyntaxKind.StructureStatement Or SyntaxKind.NewConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.ParameterList Or SyntaxKind.DeclareSubStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.WithEventsPropertyEventContainer Or SyntaxKind.AsNewClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.Attribute Or SyntaxKind.PrintStatement Or SyntaxKind.UsingBlock Or SyntaxKind.LabelStatement Or SyntaxKind.NextLabel Or SyntaxKind.EndStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.SingleLineElseClause Or SyntaxKind.ElseIfBlock Or SyntaxKind.ElseStatement Or SyntaxKind.FinallyBlock Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.SelectStatement Or SyntaxKind.CaseElseStatement Or SyntaxKind.CaseEqualsClause Or SyntaxKind.ForStepClause Or SyntaxKind.MidAssignmentStatement Or SyntaxKind.DivideAssignmentStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.REMKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.TryKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.WhileKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.FromKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.SkipKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.TypeKeyword Or SyntaxKind.IteratorKeyword Or SyntaxKind.CommaToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.PlusToken Or SyntaxKind.ColonToken Or SyntaxKind.EqualsToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.SlashEqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanExclamationMinusMinusToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.EndOfXmlToken Or SyntaxKind.XmlEntityLiteralToken Or SyntaxKind.IdentifierToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.XmlNameAttribute Or SyntaxKind.CommentTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.EnableWarningDirectiveTrivia Or SyntaxKind.SimpleDoLoopBlock Or SyntaxKind.DoLoopUntilBlock
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.OptionStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.InterfaceBlock Or SyntaxKind.ClassBlock Or SyntaxKind.InheritsStatement Or SyntaxKind.StructureStatement Or SyntaxKind.InterfaceStatement Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.AddHandlerAccessorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.EventBlock Or SyntaxKind.ParameterList Or SyntaxKind.SubStatement Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.HandlesClause Or SyntaxKind.WithEventsPropertyEventContainer Or SyntaxKind.HandlesClauseItem Or SyntaxKind.AsNewClause Or SyntaxKind.ObjectMemberInitializer Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.LabelStatement Or SyntaxKind.GoToStatement Or SyntaxKind.NextLabel Or SyntaxKind.StopStatement Or SyntaxKind.EndStatement Or SyntaxKind.ExitDoStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineElseClause Or SyntaxKind.MultiLineIfBlock Or SyntaxKind.ElseIfBlock Or SyntaxKind.ElseBlock Or SyntaxKind.ElseStatement Or SyntaxKind.TryBlock Or SyntaxKind.FinallyBlock Or SyntaxKind.TryStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.SelectStatement Or SyntaxKind.CaseElseStatement Or SyntaxKind.ElseCaseClause Or SyntaxKind.CaseEqualsClause Or SyntaxKind.CaseNotEqualsClause Or SyntaxKind.ForBlock Or SyntaxKind.ForStepClause Or SyntaxKind.ForEachStatement Or SyntaxKind.MidAssignmentStatement Or SyntaxKind.AddAssignmentStatement Or SyntaxKind.DivideAssignmentStatement Or SyntaxKind.IntegerDivideAssignmentStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OrElseKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.RaiseEventKeyword Or SyntaxKind.REMKeyword Or SyntaxKind.RemoveHandlerKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ThrowKeyword Or SyntaxKind.TryKeyword Or SyntaxKind.TryCastKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UShortKeyword Or SyntaxKind.WhileKeyword Or SyntaxKind.WideningKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.AutoKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.ExplicitKeyword Or SyntaxKind.FromKeyword Or SyntaxKind.GroupKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OffKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.RegionKeyword Or SyntaxKind.SkipKeyword Or SyntaxKind.StrictKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.UntilKeyword Or SyntaxKind.TypeKeyword Or SyntaxKind.XmlKeyword Or SyntaxKind.IteratorKeyword Or SyntaxKind.YieldKeyword Or SyntaxKind.CommaToken Or SyntaxKind.HashToken Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.ColonToken Or SyntaxKind.LessThanToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.AsteriskEqualsToken Or SyntaxKind.SlashEqualsToken Or SyntaxKind.BackslashEqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.StatementTerminatorToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanExclamationMinusMinusToken Or SyntaxKind.MinusMinusGreaterThanToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.PercentGreaterThanToken Or SyntaxKind.EndOfXmlToken Or SyntaxKind.BadToken Or SyntaxKind.XmlEntityLiteralToken Or SyntaxKind.DocumentationCommentLineBreakToken Or SyntaxKind.IdentifierToken Or SyntaxKind.IntegerLiteralToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression Or SyntaxKind.WhitespaceTrivia Or SyntaxKind.CommentTrivia Or SyntaxKind.LineContinuationTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.IfDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia Or SyntaxKind.RegionDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.ExternalSourceDirectiveTrivia Or SyntaxKind.EnableWarningDirectiveTrivia Or SyntaxKind.DisableWarningDirectiveTrivia Or SyntaxKind.BadDirectiveTrivia Or SyntaxKind.SimpleDoLoopBlock Or SyntaxKind.DoWhileLoopBlock Or SyntaxKind.DoLoopUntilBlock
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.NamespaceBlock Or SyntaxKind.ModuleBlock Or SyntaxKind.InterfaceBlock Or SyntaxKind.EnumBlock Or SyntaxKind.ImplementsStatement Or SyntaxKind.StructureStatement Or SyntaxKind.ClassStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.FunctionBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.RemoveHandlerAccessorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.ParameterList Or SyntaxKind.FunctionStatement Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.EventStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.KeywordEventContainer Or SyntaxKind.WithEventsPropertyEventContainer Or SyntaxKind.IncompleteMember Or SyntaxKind.VariableDeclarator Or SyntaxKind.AsNewClause Or SyntaxKind.ObjectCollectionInitializer Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.PrintStatement Or SyntaxKind.UsingBlock Or SyntaxKind.WithBlock Or SyntaxKind.LabelStatement Or SyntaxKind.IdentifierLabel Or SyntaxKind.NextLabel Or SyntaxKind.EndStatement Or SyntaxKind.ExitForStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineElseClause Or SyntaxKind.ElseIfBlock Or SyntaxKind.IfStatement Or SyntaxKind.ElseStatement Or SyntaxKind.FinallyBlock Or SyntaxKind.CatchStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectStatement Or SyntaxKind.CaseElseBlock Or SyntaxKind.CaseElseStatement Or SyntaxKind.SimpleCaseClause Or SyntaxKind.CaseEqualsClause Or SyntaxKind.CaseLessThanClause Or SyntaxKind.CaseGreaterThanOrEqualClause Or SyntaxKind.SyncLockStatement Or SyntaxKind.WhileStatement Or SyntaxKind.ForEachBlock Or SyntaxKind.ForStepClause Or SyntaxKind.NextStatement Or SyntaxKind.ThrowStatement Or SyntaxKind.MidAssignmentStatement Or SyntaxKind.SubtractAssignmentStatement Or SyntaxKind.DivideAssignmentStatement Or SyntaxKind.ExponentiateAssignmentStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OverloadsKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.PublicKeyword Or SyntaxKind.ReadOnlyKeyword Or SyntaxKind.REMKeyword Or SyntaxKind.ResumeKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.SubKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ToKeyword Or SyntaxKind.TryKeyword Or SyntaxKind.TypeOfKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UsingKeyword Or SyntaxKind.WhileKeyword Or SyntaxKind.WithKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.BinaryKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.ExternalSourceKeyword Or SyntaxKind.FromKeyword Or SyntaxKind.InferKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.SkipKeyword Or SyntaxKind.TakeKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.WarningKeyword Or SyntaxKind.TypeKeyword Or SyntaxKind.AsyncKeyword Or SyntaxKind.IteratorKeyword Or SyntaxKind.ExclamationToken Or SyntaxKind.CommaToken Or SyntaxKind.AmpersandToken Or SyntaxKind.OpenParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.PlusToken Or SyntaxKind.DotToken Or SyntaxKind.ColonToken Or SyntaxKind.LessThanEqualsToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.CaretToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.PlusEqualsToken Or SyntaxKind.SlashEqualsToken Or SyntaxKind.CaretEqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanExclamationMinusMinusToken Or SyntaxKind.LessThanQuestionToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.BeginCDataToken Or SyntaxKind.EndOfXmlToken Or SyntaxKind.XmlNameToken Or SyntaxKind.XmlEntityLiteralToken Or SyntaxKind.IdentifierToken Or SyntaxKind.FloatingLiteralToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlNameAttribute Or SyntaxKind.EndOfLineTrivia Or SyntaxKind.CommentTrivia Or SyntaxKind.DocumentationCommentExteriorTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.EndExternalSourceDirectiveTrivia Or SyntaxKind.EnableWarningDirectiveTrivia Or SyntaxKind.ReferenceDirectiveTrivia Or SyntaxKind.ImportAliasClause Or SyntaxKind.SimpleDoLoopBlock Or SyntaxKind.DoUntilLoopBlock Or SyntaxKind.DoLoopUntilBlock
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.EndRemoveHandlerStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.EndSyncLockStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.ModuleBlock Or SyntaxKind.StructureBlock Or SyntaxKind.InterfaceBlock Or SyntaxKind.ClassBlock Or SyntaxKind.EnumBlock Or SyntaxKind.InheritsStatement Or SyntaxKind.ImplementsStatement Or SyntaxKind.ModuleStatement Or SyntaxKind.StructureStatement Or SyntaxKind.InterfaceStatement Or SyntaxKind.ClassStatement Or SyntaxKind.EnumStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.SubBlock Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.GetAccessorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.AddHandlerAccessorBlock Or SyntaxKind.RemoveHandlerAccessorBlock Or SyntaxKind.RaiseEventAccessorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.EventBlock Or SyntaxKind.ParameterList Or SyntaxKind.SubStatement Or SyntaxKind.FunctionStatement Or SyntaxKind.SubNewStatement Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.EventStatement Or SyntaxKind.OperatorStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.AddHandlerAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.RaiseEventAccessorStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.HandlesClause Or SyntaxKind.KeywordEventContainer Or SyntaxKind.WithEventsEventContainer Or SyntaxKind.WithEventsPropertyEventContainer Or SyntaxKind.HandlesClauseItem Or SyntaxKind.IncompleteMember Or SyntaxKind.FieldDeclaration Or SyntaxKind.VariableDeclarator Or SyntaxKind.SimpleAsClause Or SyntaxKind.AsNewClause Or SyntaxKind.ObjectMemberInitializer Or SyntaxKind.ObjectCollectionInitializer Or SyntaxKind.InferredFieldInitializer Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.WithBlock Or SyntaxKind.LocalDeclarationStatement Or SyntaxKind.LabelStatement Or SyntaxKind.GoToStatement Or SyntaxKind.IdentifierLabel Or SyntaxKind.NumericLabel Or SyntaxKind.NextLabel Or SyntaxKind.StopStatement Or SyntaxKind.EndStatement Or SyntaxKind.ExitDoStatement Or SyntaxKind.ExitForStatement Or SyntaxKind.ExitSubStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueDoStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineIfPart Or SyntaxKind.SingleLineElseClause Or SyntaxKind.MultiLineIfBlock Or SyntaxKind.ElseIfBlock Or SyntaxKind.ElseBlock Or SyntaxKind.IfStatement Or SyntaxKind.ElseIfStatement Or SyntaxKind.ElseStatement Or SyntaxKind.TryBlock Or SyntaxKind.CatchBlock Or SyntaxKind.FinallyBlock Or SyntaxKind.TryStatement Or SyntaxKind.CatchStatement Or SyntaxKind.CatchFilterClause Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.OnErrorResumeNextStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectBlock Or SyntaxKind.SelectStatement Or SyntaxKind.CaseBlock Or SyntaxKind.CaseElseBlock Or SyntaxKind.CaseStatement Or SyntaxKind.CaseElseStatement Or SyntaxKind.ElseCaseClause Or SyntaxKind.SimpleCaseClause Or SyntaxKind.RangeCaseClause Or SyntaxKind.CaseEqualsClause Or SyntaxKind.CaseNotEqualsClause Or SyntaxKind.CaseLessThanClause Or SyntaxKind.CaseLessThanOrEqualClause Or SyntaxKind.CaseGreaterThanOrEqualClause Or SyntaxKind.CaseGreaterThanClause Or SyntaxKind.SyncLockStatement Or SyntaxKind.WhileStatement Or SyntaxKind.ForBlock Or SyntaxKind.ForEachBlock Or SyntaxKind.ForStatement Or SyntaxKind.ForStepClause Or SyntaxKind.ForEachStatement Or SyntaxKind.NextStatement Or SyntaxKind.UsingStatement Or SyntaxKind.ThrowStatement Or SyntaxKind.SimpleAssignmentStatement Or SyntaxKind.MidAssignmentStatement Or SyntaxKind.AddAssignmentStatement Or SyntaxKind.SubtractAssignmentStatement Or SyntaxKind.MultiplyAssignmentStatement Or SyntaxKind.DivideAssignmentStatement Or SyntaxKind.IntegerDivideAssignmentStatement Or SyntaxKind.ExponentiateAssignmentStatement Or SyntaxKind.LeftShiftAssignmentStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OrElseKeyword Or SyntaxKind.OverloadsKeyword Or SyntaxKind.OverridableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PrivateKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.PublicKeyword Or SyntaxKind.RaiseEventKeyword Or SyntaxKind.ReadOnlyKeyword Or SyntaxKind.ReDimKeyword Or SyntaxKind.REMKeyword Or SyntaxKind.RemoveHandlerKeyword Or SyntaxKind.ResumeKeyword Or SyntaxKind.ReturnKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.StaticKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.StructureKeyword Or SyntaxKind.SubKeyword Or SyntaxKind.SyncLockKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ThrowKeyword Or SyntaxKind.ToKeyword Or SyntaxKind.TrueKeyword Or SyntaxKind.TryKeyword Or SyntaxKind.TryCastKeyword Or SyntaxKind.TypeOfKeyword Or SyntaxKind.UIntegerKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UShortKeyword Or SyntaxKind.UsingKeyword Or SyntaxKind.WhenKeyword Or SyntaxKind.WhileKeyword Or SyntaxKind.WideningKeyword Or SyntaxKind.WithKeyword Or SyntaxKind.WithEventsKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AscendingKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.AutoKeyword Or SyntaxKind.BinaryKeyword Or SyntaxKind.ByKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.DisableKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.EnableKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.ExplicitKeyword Or SyntaxKind.ExternalSourceKeyword Or SyntaxKind.ExternalChecksumKeyword Or SyntaxKind.FromKeyword Or SyntaxKind.GroupKeyword Or SyntaxKind.InferKeyword Or SyntaxKind.IntoKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OffKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OutKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.RegionKeyword Or SyntaxKind.SkipKeyword Or SyntaxKind.StrictKeyword Or SyntaxKind.TakeKeyword Or SyntaxKind.TextKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.UntilKeyword Or SyntaxKind.WarningKeyword Or SyntaxKind.WhereKeyword Or SyntaxKind.TypeKeyword Or SyntaxKind.XmlKeyword Or SyntaxKind.AsyncKeyword Or SyntaxKind.AwaitKeyword Or SyntaxKind.IteratorKeyword Or SyntaxKind.YieldKeyword Or SyntaxKind.ExclamationToken Or SyntaxKind.AtToken Or SyntaxKind.CommaToken Or SyntaxKind.HashToken Or SyntaxKind.AmpersandToken Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.AsteriskToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.DotToken Or SyntaxKind.SlashToken Or SyntaxKind.ColonToken Or SyntaxKind.LessThanToken Or SyntaxKind.LessThanEqualsToken Or SyntaxKind.LessThanGreaterThanToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.BackslashToken Or SyntaxKind.CaretToken Or SyntaxKind.ColonEqualsToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.AsteriskEqualsToken Or SyntaxKind.PlusEqualsToken Or SyntaxKind.MinusEqualsToken Or SyntaxKind.SlashEqualsToken Or SyntaxKind.BackslashEqualsToken Or SyntaxKind.CaretEqualsToken Or SyntaxKind.LessThanLessThanToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.QuestionToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.StatementTerminatorToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.EmptyToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanSlashToken Or SyntaxKind.LessThanExclamationMinusMinusToken Or SyntaxKind.MinusMinusGreaterThanToken Or SyntaxKind.LessThanQuestionToken Or SyntaxKind.QuestionGreaterThanToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.PercentGreaterThanToken Or SyntaxKind.BeginCDataToken Or SyntaxKind.EndCDataToken Or SyntaxKind.EndOfXmlToken Or SyntaxKind.BadToken Or SyntaxKind.XmlNameToken Or SyntaxKind.XmlTextLiteralToken Or SyntaxKind.XmlEntityLiteralToken Or SyntaxKind.DocumentationCommentLineBreakToken Or SyntaxKind.IdentifierToken Or SyntaxKind.IntegerLiteralToken Or SyntaxKind.FloatingLiteralToken Or SyntaxKind.DecimalLiteralToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlCrefAttribute Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression Or SyntaxKind.WhitespaceTrivia Or SyntaxKind.EndOfLineTrivia Or SyntaxKind.ColonTrivia Or SyntaxKind.CommentTrivia Or SyntaxKind.LineContinuationTrivia Or SyntaxKind.DocumentationCommentExteriorTrivia Or SyntaxKind.DisabledTextTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.IfDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.ElseDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia Or SyntaxKind.RegionDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.ExternalSourceDirectiveTrivia Or SyntaxKind.EndExternalSourceDirectiveTrivia Or SyntaxKind.ExternalChecksumDirectiveTrivia Or SyntaxKind.EnableWarningDirectiveTrivia Or SyntaxKind.DisableWarningDirectiveTrivia Or SyntaxKind.ReferenceDirectiveTrivia Or SyntaxKind.BadDirectiveTrivia Or SyntaxKind.ImportAliasClause Or SyntaxKind.NameColonEquals Or SyntaxKind.SimpleDoLoopBlock Or SyntaxKind.DoWhileLoopBlock Or SyntaxKind.DoUntilLoopBlock Or SyntaxKind.DoLoopWhileBlock Or SyntaxKind.DoLoopUntilBlock
				Case 768
				Case SyntaxKind.List Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword
				Case SyntaxKind.SimpleDoStatement
				Case SyntaxKind.DoWhileStatement
				Case SyntaxKind.DoUntilStatement
				Case SyntaxKind.SimpleLoopStatement
				Case SyntaxKind.LoopWhileStatement
				Case SyntaxKind.LoopUntilStatement
				Case SyntaxKind.WhileClause
				Case SyntaxKind.UntilClause
				Case SyntaxKind.NameOfExpression
				Case SyntaxKind.InterpolatedStringExpression
				Case SyntaxKind.InterpolatedStringText
				Case SyntaxKind.Interpolation
				Case SyntaxKind.InterpolationAlignmentClause
				Case SyntaxKind.InterpolationFormatClause
				Label0:
					empty = [String].Empty
					Exit Select
				Case SyntaxKind.BooleanKeyword
					empty = "Boolean"
					Exit Select
				Case SyntaxKind.ByRefKeyword
					empty = "ByRef"
					Exit Select
				Case SyntaxKind.ByteKeyword
					empty = "Byte"
					Exit Select
				Case SyntaxKind.ByValKeyword
					empty = "ByVal"
					Exit Select
				Case SyntaxKind.CallKeyword
					empty = "Call"
					Exit Select
				Case SyntaxKind.CaseKeyword
					empty = "Case"
					Exit Select
				Case SyntaxKind.CatchKeyword
					empty = "Catch"
					Exit Select
				Case SyntaxKind.CBoolKeyword
					empty = "CBool"
					Exit Select
				Case SyntaxKind.CByteKeyword
					empty = "CByte"
					Exit Select
				Case SyntaxKind.CCharKeyword
					empty = "CChar"
					Exit Select
				Case SyntaxKind.CDateKeyword
					empty = "CDate"
					Exit Select
				Case SyntaxKind.CDecKeyword
					empty = "CDec"
					Exit Select
				Case SyntaxKind.CDblKeyword
					empty = "CDbl"
					Exit Select
				Case SyntaxKind.CharKeyword
					empty = "Char"
					Exit Select
				Case SyntaxKind.CIntKeyword
					empty = "CInt"
					Exit Select
				Case SyntaxKind.ClassKeyword
					empty = "Class"
					Exit Select
				Case SyntaxKind.CLngKeyword
					empty = "CLng"
					Exit Select
				Case SyntaxKind.CObjKeyword
					empty = "CObj"
					Exit Select
				Case SyntaxKind.ConstKeyword
					empty = "Const"
					Exit Select
				Case SyntaxKind.ReferenceKeyword
					empty = "R"
					Exit Select
				Case SyntaxKind.ContinueKeyword
					empty = "Continue"
					Exit Select
				Case SyntaxKind.CSByteKeyword
					empty = "CSByte"
					Exit Select
				Case SyntaxKind.CShortKeyword
					empty = "CShort"
					Exit Select
				Case SyntaxKind.CSngKeyword
					empty = "CSng"
					Exit Select
				Case SyntaxKind.CStrKeyword
					empty = "CStr"
					Exit Select
				Case SyntaxKind.CTypeKeyword
					empty = "CType"
					Exit Select
				Case SyntaxKind.CUIntKeyword
					empty = "CUInt"
					Exit Select
				Case SyntaxKind.CULngKeyword
					empty = "CULng"
					Exit Select
				Case SyntaxKind.CUShortKeyword
					empty = "CUShort"
					Exit Select
				Case SyntaxKind.DateKeyword
					empty = "Date"
					Exit Select
				Case SyntaxKind.DecimalKeyword
					empty = "Decimal"
					Exit Select
				Case SyntaxKind.DeclareKeyword
					empty = "Declare"
					Exit Select
				Case SyntaxKind.DefaultKeyword
					empty = "Default"
					Exit Select
				Case SyntaxKind.DelegateKeyword
					empty = "Delegate"
					Exit Select
				Case SyntaxKind.DimKeyword
					empty = "Dim"
					Exit Select
				Case SyntaxKind.DirectCastKeyword
					empty = "DirectCast"
					Exit Select
				Case SyntaxKind.DoKeyword
					empty = "Do"
					Exit Select
				Case SyntaxKind.DoubleKeyword
					empty = "Double"
					Exit Select
				Case SyntaxKind.EachKeyword
					empty = "Each"
					Exit Select
				Case SyntaxKind.ElseKeyword
					empty = "Else"
					Exit Select
				Case SyntaxKind.ElseIfKeyword
					empty = "ElseIf"
					Exit Select
				Case SyntaxKind.EndKeyword
					empty = "End"
					Exit Select
				Case SyntaxKind.EnumKeyword
					empty = "Enum"
					Exit Select
				Case SyntaxKind.EraseKeyword
					empty = "Erase"
					Exit Select
				Case SyntaxKind.ErrorKeyword
					empty = "Error"
					Exit Select
				Case SyntaxKind.EventKeyword
					empty = "Event"
					Exit Select
				Case SyntaxKind.ExitKeyword
					empty = "Exit"
					Exit Select
				Case SyntaxKind.FalseKeyword
					empty = "False"
					Exit Select
				Case SyntaxKind.FinallyKeyword
					empty = "Finally"
					Exit Select
				Case SyntaxKind.ForKeyword
					empty = "For"
					Exit Select
				Case SyntaxKind.FriendKeyword
					empty = "Friend"
					Exit Select
				Case SyntaxKind.FunctionKeyword
					empty = "Function"
					Exit Select
				Case SyntaxKind.GetKeyword
					empty = "Get"
					Exit Select
				Case SyntaxKind.GetTypeKeyword
					empty = "GetType"
					Exit Select
				Case SyntaxKind.GetXmlNamespaceKeyword
					empty = "GetXmlNamespace"
					Exit Select
				Case SyntaxKind.GlobalKeyword
					empty = "Global"
					Exit Select
				Case SyntaxKind.GoToKeyword
					empty = "GoTo"
					Exit Select
				Case SyntaxKind.HandlesKeyword
					empty = "Handles"
					Exit Select
				Case SyntaxKind.IfKeyword
					empty = "If"
					Exit Select
				Case SyntaxKind.ImplementsKeyword
					empty = "Implements"
					Exit Select
				Case SyntaxKind.ImportsKeyword
					empty = "Imports"
					Exit Select
				Case SyntaxKind.InKeyword
					empty = "In"
					Exit Select
				Case SyntaxKind.InheritsKeyword
					empty = "Inherits"
					Exit Select
				Case SyntaxKind.IntegerKeyword
					empty = "Integer"
					Exit Select
				Case SyntaxKind.InterfaceKeyword
					empty = "Interface"
					Exit Select
				Case SyntaxKind.IsKeyword
					empty = "Is"
					Exit Select
				Case SyntaxKind.IsNotKeyword
					empty = "IsNot"
					Exit Select
				Case SyntaxKind.LetKeyword
					empty = "Let"
					Exit Select
				Case SyntaxKind.LibKeyword
					empty = "Lib"
					Exit Select
				Case SyntaxKind.LikeKeyword
					empty = "Like"
					Exit Select
				Case SyntaxKind.LongKeyword
					empty = "Long"
					Exit Select
				Case SyntaxKind.LoopKeyword
					empty = "Loop"
					Exit Select
				Case SyntaxKind.MeKeyword
					empty = "Me"
					Exit Select
				Case SyntaxKind.ModKeyword
					empty = "Mod"
					Exit Select
				Case SyntaxKind.ModuleKeyword
					empty = "Module"
					Exit Select
				Case SyntaxKind.MustInheritKeyword
					empty = "MustInherit"
					Exit Select
				Case SyntaxKind.MustOverrideKeyword
					empty = "MustOverride"
					Exit Select
				Case SyntaxKind.MyBaseKeyword
					empty = "MyBase"
					Exit Select
				Case SyntaxKind.MyClassKeyword
					empty = "MyClass"
					Exit Select
				Case SyntaxKind.NamespaceKeyword
					empty = "Namespace"
					Exit Select
				Case SyntaxKind.NarrowingKeyword
					empty = "Narrowing"
					Exit Select
				Case SyntaxKind.NextKeyword
					empty = "Next"
					Exit Select
				Case SyntaxKind.NewKeyword
					empty = "New"
					Exit Select
				Case SyntaxKind.NotKeyword
					empty = "Not"
					Exit Select
				Case SyntaxKind.NothingKeyword
					empty = "Nothing"
					Exit Select
				Case SyntaxKind.NotInheritableKeyword
					empty = "NotInheritable"
					Exit Select
				Case SyntaxKind.NotOverridableKeyword
					empty = "NotOverridable"
					Exit Select
				Case SyntaxKind.ObjectKeyword
					empty = "Object"
					Exit Select
				Case SyntaxKind.OfKeyword
					empty = "Of"
					Exit Select
				Case SyntaxKind.OnKeyword
					empty = "On"
					Exit Select
				Case SyntaxKind.OperatorKeyword
					empty = "Operator"
					Exit Select
				Case SyntaxKind.OptionKeyword
					empty = "Option"
					Exit Select
				Case SyntaxKind.OptionalKeyword
					empty = "Optional"
					Exit Select
				Case SyntaxKind.OrKeyword
					empty = "Or"
					Exit Select
				Case SyntaxKind.OrElseKeyword
					empty = "OrElse"
					Exit Select
				Case SyntaxKind.OverloadsKeyword
					empty = "Overloads"
					Exit Select
				Case SyntaxKind.OverridableKeyword
					empty = "Overridable"
					Exit Select
				Case SyntaxKind.OverridesKeyword
					empty = "Overrides"
					Exit Select
				Case SyntaxKind.ParamArrayKeyword
					empty = "ParamArray"
					Exit Select
				Case SyntaxKind.PartialKeyword
					empty = "Partial"
					Exit Select
				Case SyntaxKind.PrivateKeyword
					empty = "Private"
					Exit Select
				Case SyntaxKind.PropertyKeyword
					empty = "Property"
					Exit Select
				Case SyntaxKind.ProtectedKeyword
					empty = "Protected"
					Exit Select
				Case SyntaxKind.PublicKeyword
					empty = "Public"
					Exit Select
				Case SyntaxKind.RaiseEventKeyword
					empty = "RaiseEvent"
					Exit Select
				Case SyntaxKind.ReadOnlyKeyword
					empty = "ReadOnly"
					Exit Select
				Case SyntaxKind.ReDimKeyword
					empty = "ReDim"
					Exit Select
				Case SyntaxKind.REMKeyword
					empty = "REM"
					Exit Select
				Case SyntaxKind.RemoveHandlerKeyword
					empty = "RemoveHandler"
					Exit Select
				Case SyntaxKind.ResumeKeyword
					empty = "Resume"
					Exit Select
				Case SyntaxKind.ReturnKeyword
					empty = "Return"
					Exit Select
				Case SyntaxKind.SByteKeyword
					empty = "SByte"
					Exit Select
				Case SyntaxKind.SelectKeyword
					empty = "Select"
					Exit Select
				Case SyntaxKind.SetKeyword
					empty = "Set"
					Exit Select
				Case SyntaxKind.ShadowsKeyword
					empty = "Shadows"
					Exit Select
				Case SyntaxKind.SharedKeyword
					empty = "Shared"
					Exit Select
				Case SyntaxKind.ShortKeyword
					empty = "Short"
					Exit Select
				Case SyntaxKind.SingleKeyword
					empty = "Single"
					Exit Select
				Case SyntaxKind.StaticKeyword
					empty = "Static"
					Exit Select
				Case SyntaxKind.StepKeyword
					empty = "Step"
					Exit Select
				Case SyntaxKind.StopKeyword
					empty = "Stop"
					Exit Select
				Case SyntaxKind.StringKeyword
					empty = "String"
					Exit Select
				Case SyntaxKind.StructureKeyword
					empty = "Structure"
					Exit Select
				Case SyntaxKind.SubKeyword
					empty = "Sub"
					Exit Select
				Case SyntaxKind.SyncLockKeyword
					empty = "SyncLock"
					Exit Select
				Case SyntaxKind.ThenKeyword
					empty = "Then"
					Exit Select
				Case SyntaxKind.ThrowKeyword
					empty = "Throw"
					Exit Select
				Case SyntaxKind.ToKeyword
					empty = "To"
					Exit Select
				Case SyntaxKind.TrueKeyword
					empty = "True"
					Exit Select
				Case SyntaxKind.TryKeyword
					empty = "Try"
					Exit Select
				Case SyntaxKind.TryCastKeyword
					empty = "TryCast"
					Exit Select
				Case SyntaxKind.TypeOfKeyword
					empty = "TypeOf"
					Exit Select
				Case SyntaxKind.UIntegerKeyword
					empty = "UInteger"
					Exit Select
				Case SyntaxKind.ULongKeyword
					empty = "ULong"
					Exit Select
				Case SyntaxKind.UShortKeyword
					empty = "UShort"
					Exit Select
				Case SyntaxKind.UsingKeyword
					empty = "Using"
					Exit Select
				Case SyntaxKind.WhenKeyword
					empty = "When"
					Exit Select
				Case SyntaxKind.WhileKeyword
					empty = "While"
					Exit Select
				Case SyntaxKind.WideningKeyword
					empty = "Widening"
					Exit Select
				Case SyntaxKind.WithKeyword
					empty = "With"
					Exit Select
				Case SyntaxKind.WithEventsKeyword
					empty = "WithEvents"
					Exit Select
				Case SyntaxKind.WriteOnlyKeyword
					empty = "WriteOnly"
					Exit Select
				Case SyntaxKind.XorKeyword
					empty = "Xor"
					Exit Select
				Case SyntaxKind.EndIfKeyword
					empty = "EndIf"
					Exit Select
				Case SyntaxKind.GosubKeyword
					empty = "Gosub"
					Exit Select
				Case SyntaxKind.VariantKeyword
					empty = "Variant"
					Exit Select
				Case SyntaxKind.WendKeyword
					empty = "Wend"
					Exit Select
				Case SyntaxKind.AggregateKeyword
					empty = "Aggregate"
					Exit Select
				Case SyntaxKind.AllKeyword
					empty = "All"
					Exit Select
				Case SyntaxKind.AnsiKeyword
					empty = "Ansi"
					Exit Select
				Case SyntaxKind.AscendingKeyword
					empty = "Ascending"
					Exit Select
				Case SyntaxKind.AssemblyKeyword
					empty = "Assembly"
					Exit Select
				Case SyntaxKind.AutoKeyword
					empty = "Auto"
					Exit Select
				Case SyntaxKind.BinaryKeyword
					empty = "Binary"
					Exit Select
				Case SyntaxKind.ByKeyword
					empty = "By"
					Exit Select
				Case SyntaxKind.CompareKeyword
					empty = "Compare"
					Exit Select
				Case SyntaxKind.CustomKeyword
					empty = "Custom"
					Exit Select
				Case SyntaxKind.DescendingKeyword
					empty = "Descending"
					Exit Select
				Case SyntaxKind.DisableKeyword
					empty = "Disable"
					Exit Select
				Case SyntaxKind.DistinctKeyword
					empty = "Distinct"
					Exit Select
				Case SyntaxKind.EnableKeyword
					empty = "Enable"
					Exit Select
				Case SyntaxKind.EqualsKeyword
					empty = "Equals"
					Exit Select
				Case SyntaxKind.ExplicitKeyword
					empty = "Explicit"
					Exit Select
				Case SyntaxKind.ExternalSourceKeyword
					empty = "ExternalSource"
					Exit Select
				Case SyntaxKind.ExternalChecksumKeyword
					empty = "ExternalChecksum"
					Exit Select
				Case SyntaxKind.FromKeyword
					empty = "From"
					Exit Select
				Case SyntaxKind.GroupKeyword
					empty = "Group"
					Exit Select
				Case SyntaxKind.InferKeyword
					empty = "Infer"
					Exit Select
				Case SyntaxKind.IntoKeyword
					empty = "Into"
					Exit Select
				Case SyntaxKind.IsFalseKeyword
					empty = "IsFalse"
					Exit Select
				Case SyntaxKind.IsTrueKeyword
					empty = "IsTrue"
					Exit Select
				Case SyntaxKind.JoinKeyword
					empty = "Join"
					Exit Select
				Case SyntaxKind.KeyKeyword
					empty = "Key"
					Exit Select
				Case SyntaxKind.MidKeyword
					empty = "Mid"
					Exit Select
				Case SyntaxKind.OffKeyword
					empty = "Off"
					Exit Select
				Case SyntaxKind.OrderKeyword
					empty = "Order"
					Exit Select
				Case SyntaxKind.OutKeyword
					empty = "Out"
					Exit Select
				Case SyntaxKind.PreserveKeyword
					empty = "Preserve"
					Exit Select
				Case SyntaxKind.RegionKeyword
					empty = "Region"
					Exit Select
				Case SyntaxKind.SkipKeyword
					empty = "Skip"
					Exit Select
				Case SyntaxKind.StrictKeyword
					empty = "Strict"
					Exit Select
				Case SyntaxKind.TakeKeyword
					empty = "Take"
					Exit Select
				Case SyntaxKind.TextKeyword
					empty = "Text"
					Exit Select
				Case SyntaxKind.UnicodeKeyword
					empty = "Unicode"
					Exit Select
				Case SyntaxKind.UntilKeyword
					empty = "Until"
					Exit Select
				Case SyntaxKind.WarningKeyword
					empty = "Warning"
					Exit Select
				Case SyntaxKind.WhereKeyword
					empty = "Where"
					Exit Select
				Case SyntaxKind.TypeKeyword
					empty = "Type"
					Exit Select
				Case SyntaxKind.XmlKeyword
					empty = "xml"
					Exit Select
				Case SyntaxKind.AsyncKeyword
					empty = "Async"
					Exit Select
				Case SyntaxKind.AwaitKeyword
					empty = "Await"
					Exit Select
				Case SyntaxKind.IteratorKeyword
					empty = "Iterator"
					Exit Select
				Case SyntaxKind.YieldKeyword
					empty = "Yield"
					Exit Select
				Case SyntaxKind.ExclamationToken
					empty = "!"
					Exit Select
				Case SyntaxKind.AtToken
					empty = "@"
					Exit Select
				Case SyntaxKind.CommaToken
					empty = ","
					Exit Select
				Case SyntaxKind.HashToken
					empty = "#"
					Exit Select
				Case SyntaxKind.AmpersandToken
					empty = "&"
					Exit Select
				Case SyntaxKind.SingleQuoteToken
					empty = "'"
					Exit Select
				Case SyntaxKind.OpenParenToken
					empty = "("
					Exit Select
				Case SyntaxKind.CloseParenToken
					empty = ")"
					Exit Select
				Case SyntaxKind.OpenBraceToken
					empty = "{"
					Exit Select
				Case SyntaxKind.CloseBraceToken
					empty = "}"
					Exit Select
				Case SyntaxKind.SemicolonToken
					empty = ";"
					Exit Select
				Case SyntaxKind.AsteriskToken
					empty = "*"
					Exit Select
				Case SyntaxKind.PlusToken
					empty = "+"
					Exit Select
				Case SyntaxKind.MinusToken
					empty = "-"
					Exit Select
				Case SyntaxKind.DotToken
					empty = "."
					Exit Select
				Case SyntaxKind.SlashToken
					empty = "/"
					Exit Select
				Case SyntaxKind.ColonToken
					empty = ":"
					Exit Select
				Case SyntaxKind.LessThanToken
					empty = "<"
					Exit Select
				Case SyntaxKind.LessThanEqualsToken
					empty = "<="
					Exit Select
				Case SyntaxKind.LessThanGreaterThanToken
					empty = "<>"
					Exit Select
				Case SyntaxKind.EqualsToken
					empty = "="
					Exit Select
				Case SyntaxKind.GreaterThanToken
					empty = ">"
					Exit Select
				Case SyntaxKind.GreaterThanEqualsToken
					empty = ">="
					Exit Select
				Case SyntaxKind.BackslashToken
					empty = "\"
					Exit Select
				Case SyntaxKind.CaretToken
					empty = "^"
					Exit Select
				Case SyntaxKind.ColonEqualsToken
					empty = ":="
					Exit Select
				Case SyntaxKind.AmpersandEqualsToken
					empty = "&="
					Exit Select
				Case SyntaxKind.AsteriskEqualsToken
					empty = "*="
					Exit Select
				Case SyntaxKind.PlusEqualsToken
					empty = "+="
					Exit Select
				Case SyntaxKind.MinusEqualsToken
					empty = "-="
					Exit Select
				Case SyntaxKind.SlashEqualsToken
					empty = "/="
					Exit Select
				Case SyntaxKind.BackslashEqualsToken
					empty = "\="
					Exit Select
				Case SyntaxKind.CaretEqualsToken
					empty = "^="
					Exit Select
				Case SyntaxKind.LessThanLessThanToken
					empty = "<<"
					Exit Select
				Case SyntaxKind.GreaterThanGreaterThanToken
					empty = ">>"
					Exit Select
				Case SyntaxKind.LessThanLessThanEqualsToken
					empty = "<<="
					Exit Select
				Case SyntaxKind.GreaterThanGreaterThanEqualsToken
					empty = ">>="
					Exit Select
				Case SyntaxKind.QuestionToken
					empty = "?"
					Exit Select
				Case SyntaxKind.DoubleQuoteToken
					empty = """"
					Exit Select
				Case SyntaxKind.StatementTerminatorToken
					empty = "" & VbCrLf & ""
					Exit Select
				Case SyntaxKind.SlashGreaterThanToken
					empty = "/>"
					Exit Select
				Case SyntaxKind.LessThanSlashToken
					empty = "</"
					Exit Select
				Case SyntaxKind.LessThanExclamationMinusMinusToken
					empty = "<!--"
					Exit Select
				Case SyntaxKind.MinusMinusGreaterThanToken
					empty = "-->"
					Exit Select
				Case SyntaxKind.LessThanQuestionToken
					empty = "<?"
					Exit Select
				Case SyntaxKind.QuestionGreaterThanToken
					empty = "?>"
					Exit Select
				Case SyntaxKind.LessThanPercentEqualsToken
					empty = "<%="
					Exit Select
				Case SyntaxKind.PercentGreaterThanToken
					empty = "%>"
					Exit Select
				Case SyntaxKind.BeginCDataToken
					empty = "<![CDATA["
					Exit Select
				Case SyntaxKind.EndCDataToken
					empty = "]]>"
					Exit Select
				Case SyntaxKind.ColonTrivia
					empty = ":"
					Exit Select
				Case SyntaxKind.LineContinuationTrivia
					empty = "_" & VbCrLf & ""
					Exit Select
				Case SyntaxKind.DocumentationCommentExteriorTrivia
					empty = "'''"
					Exit Select
				Case SyntaxKind.NameOfKeyword
					empty = "NameOf"
					Exit Select
				Case SyntaxKind.DollarSignDoubleQuoteToken
					empty = "$"""
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return empty
		End Function

		Public Shared Function GetText(ByVal accessibility As Microsoft.CodeAnalysis.Accessibility) As String
			Dim empty As String
			Select Case accessibility
				Case Microsoft.CodeAnalysis.Accessibility.NotApplicable
					empty = [String].Empty
					Exit Select
				Case Microsoft.CodeAnalysis.Accessibility.[Private]
					empty = SyntaxFacts.GetText(SyntaxKind.PrivateKeyword)
					Exit Select
				Case Microsoft.CodeAnalysis.Accessibility.ProtectedAndInternal
					empty = [String].Concat(SyntaxFacts.GetText(SyntaxKind.PrivateKeyword), " ", SyntaxFacts.GetText(SyntaxKind.ProtectedKeyword))
					Exit Select
				Case Microsoft.CodeAnalysis.Accessibility.[Protected]
					empty = SyntaxFacts.GetText(SyntaxKind.ProtectedKeyword)
					Exit Select
				Case Microsoft.CodeAnalysis.Accessibility.Internal
					empty = SyntaxFacts.GetText(SyntaxKind.FriendKeyword)
					Exit Select
				Case Microsoft.CodeAnalysis.Accessibility.ProtectedOrInternal
					empty = [String].Concat(SyntaxFacts.GetText(SyntaxKind.ProtectedKeyword), " ", SyntaxFacts.GetText(SyntaxKind.FriendKeyword))
					Exit Select
				Case Microsoft.CodeAnalysis.Accessibility.[Public]
					empty = SyntaxFacts.GetText(SyntaxKind.PublicKeyword)
					Exit Select
				Case Else
					empty = Nothing
					Exit Select
			End Select
			Return empty
		End Function

		Public Shared Function GetTypeStatementKind(ByVal keyword As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = keyword
			If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassKeyword) Then
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassStatement
			ElseIf (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceKeyword) Then
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceStatement
			Else
				syntaxKind = If(syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureStatement, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None)
			End If
			Return syntaxKind
		End Function

		Public Shared Function GetUnaryExpression(ByVal token As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = token
			If (syntaxKind1 <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword) Then
				If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddressOfKeyword) Then
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddressOfExpression
				Else
					If (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword) Then
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
						Return syntaxKind
					End If
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotExpression
				End If
			ElseIf (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PlusToken) Then
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UnaryPlusExpression
			Else
				If (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MinusToken) Then
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
					Return syntaxKind
				End If
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UnaryMinusExpression
			End If
			Return syntaxKind
		End Function

		Friend Shared Function InBlockInterior(ByVal possibleBlock As SyntaxNode, ByVal position As Integer) As Boolean
			Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
			Return SyntaxFacts.InBlockInterior(possibleBlock, position, statementSyntaxes)
		End Function

		Friend Shared Function InBlockInterior(ByVal possibleBlock As SyntaxNode, ByVal position As Integer, ByRef body As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Boolean
			Dim flag As Boolean
			Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax = Nothing
			Dim statementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax = Nothing
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = New Microsoft.CodeAnalysis.SyntaxToken()
			If (Not SyntaxFacts.IsBlockStatement(possibleBlock, statementSyntax, syntaxToken, body, statementSyntax1)) Then
				flag = False
			Else
				Dim flag1 As Boolean = True
				Dim flag2 As Boolean = True
				flag1 = If(syntaxToken.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None OrElse syntaxToken.Width <= 0, Not SyntaxFacts.InOrBeforeSpanOrEffectiveTrailingOfNode(statementSyntax, position), position >= syntaxToken.SpanStart)
				If (statementSyntax1 Is Nothing) Then
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = possibleBlock.Kind()
					If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfStatement AndAlso syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineElseClause) Then
						Dim lastToken As Microsoft.CodeAnalysis.SyntaxToken = possibleBlock.GetLastToken(True, False, False, False)
						Dim nextToken As Microsoft.CodeAnalysis.SyntaxToken = lastToken.GetNextToken(False, False, False, False)
						lastToken = New Microsoft.CodeAnalysis.SyntaxToken()
						If (nextToken <> lastToken) Then
							flag2 = position < nextToken.SpanStart
						End If
					ElseIf (body.Count > 0) Then
						flag2 = SyntaxFacts.InOrBeforeSpanOrEffectiveTrailingOfNode(body(body.Count - 1), position)
					End If
				ElseIf (statementSyntax1.Width > 0) Then
					flag2 = SyntaxFacts.InOrBeforeSpanOrEffectiveTrailingOfNode(statementSyntax1, position)
				End If
				flag = If(Not flag1, False, flag2)
			End If
			Return flag
		End Function

		Friend Shared Function InLambdaInterior(ByVal possibleLambda As SyntaxNode, ByVal position As Integer) As Boolean
			Dim flag As Boolean
			Dim flag1 As Boolean
			Dim flag2 As Boolean
			Dim closeParenToken As SyntaxToken
			Dim span As TextSpan
			Select Case possibleLambda.Kind()
				Case SyntaxKind.SingleLineFunctionLambdaExpression
				Case SyntaxKind.SingleLineSubLambdaExpression
					Dim singleLineLambdaExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineLambdaExpressionSyntax = DirectCast(possibleLambda, Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineLambdaExpressionSyntax)
					Dim parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax = singleLineLambdaExpressionSyntax.SubOrFunctionHeader.ParameterList
					If (parameterList Is Nothing OrElse parameterList.CloseParenToken.IsMissing) Then
						span = singleLineLambdaExpressionSyntax.SubOrFunctionHeader.Span
						flag1 = position >= span.[End]
					Else
						closeParenToken = parameterList.CloseParenToken
						flag1 = position >= closeParenToken.SpanStart
					End If
					span = singleLineLambdaExpressionSyntax.Body.Span
					flag2 = position <= span.[End]
					Exit Select
				Case SyntaxKind.EndFunctionStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.FunctionBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.MidExpression Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.DateLiteralExpression Or SyntaxKind.NotEqualsExpression Or SyntaxKind.GreaterThanExpression Or SyntaxKind.AddressOfExpression
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.AddHandlerAccessorBlock Or SyntaxKind.MidExpression Or SyntaxKind.CallStatement Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.TrueLiteralExpression Or SyntaxKind.DateLiteralExpression Or SyntaxKind.NotEqualsExpression Or SyntaxKind.LessThanExpression Or SyntaxKind.GreaterThanExpression Or SyntaxKind.IsExpression Or SyntaxKind.AddressOfExpression Or SyntaxKind.BinaryConditionalExpression
					flag = False
					Return flag
				Case SyntaxKind.MultiLineFunctionLambdaExpression
				Case SyntaxKind.MultiLineSubLambdaExpression
					Dim multiLineLambdaExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineLambdaExpressionSyntax = DirectCast(possibleLambda, Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineLambdaExpressionSyntax)
					Dim parameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterListSyntax = multiLineLambdaExpressionSyntax.SubOrFunctionHeader.ParameterList
					If (parameterListSyntax Is Nothing OrElse parameterListSyntax.CloseParenToken.IsMissing) Then
						span = multiLineLambdaExpressionSyntax.SubOrFunctionHeader.Span
						flag1 = position >= span.[End]
					Else
						closeParenToken = parameterListSyntax.CloseParenToken
						flag1 = position >= closeParenToken.SpanStart
					End If
					flag2 = position < multiLineLambdaExpressionSyntax.EndSubOrFunctionStatement.SpanStart
					Exit Select
				Case Else
					flag = False
					Return flag
			End Select
			flag = If(Not flag1, False, flag2)
			Return flag
		End Function

		Private Shared Function InOrBeforeSpanOrEffectiveTrailingOfNode(ByVal node As SyntaxNode, ByVal position As Integer) As Boolean
			If (position < node.SpanStart) Then
				Return True
			End If
			Return SyntaxFacts.InSpanOrEffectiveTrailingOfNode(node, position)
		End Function

		Friend Shared Function InSpanOrEffectiveTrailingOfNode(ByVal node As SyntaxNode, ByVal position As Integer) As Boolean
			Dim flag As Boolean
			Dim span As TextSpan = node.Span
			If (span.Contains(position)) Then
				flag = True
			ElseIf (position < span.[End] OrElse position >= node.FullSpan.[End]) Then
				flag = False
			Else
				Dim enumerator As SyntaxTriviaList.Enumerator = node.GetTrailingTrivia().GetEnumerator()
				While enumerator.MoveNext()
					Dim current As SyntaxTrivia = enumerator.Current
					If (current.Kind() = SyntaxKind.EndOfLineTrivia OrElse current.Kind() = SyntaxKind.ColonTrivia) Then
						Exit While
					End If
					If (Not current.FullSpan.Contains(position)) Then
						Continue While
					End If
					flag = True
					Return flag
				End While
				flag = False
			End If
			Return flag
		End Function

		Friend Shared Function IntegralLiteralCharacterValue(ByVal Digit As Char) As Byte
			Dim num As Byte
			If (SyntaxFacts.IsFullWidth(Digit)) Then
				Digit = SyntaxFacts.MakeHalfWidth(Digit)
			End If
			Dim num1 As Integer = Digit
			If (Not SyntaxFacts.IsDecimalDigit(Digit)) Then
				num = If(Digit < "A"C OrElse Digit > "F"C, CByte((num1 + -87)), CByte((num1 + -55)))
			Else
				num = CByte((num1 - 48))
			End If
			Return num
		End Function

		Public Shared Function IsAccessibilityModifier(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FriendKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrivateKeyword OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List), True, False)
			Return flag
		End Function

		Public Shared Function IsAccessorBlock(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.GetAccessorBlock) > 4, False, True)
		End Function

		Public Shared Function IsAccessorStatement(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement) <= CUShort((Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement)) OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorStatement, True, False)
			Return flag
		End Function

		Public Shared Function IsAccessorStatementAccessorKeyword(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetKeyword) Then
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetKeyword) Then
					flag = True
					Return flag
				End If
				flag = False
				Return flag
			ElseIf (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventKeyword AndAlso syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerKeyword AndAlso syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetKeyword) Then
				flag = False
				Return flag
			End If
			flag = True
			Return flag
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use IsAccessorStatementAccessorKeyword instead.", True)>
		Public Shared Function IsAccessorStatementKeyword(ByVal kind As SyntaxKind) As Boolean
			Return SyntaxFacts.IsAccessorStatementAccessorKeyword(kind)
		End Function

		Public Shared Function IsAddRemoveHandlerStatement(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.AddHandlerStatement) > CUShort(SyntaxKind.List), False, True)
		End Function

		Public Shared Function IsAddRemoveHandlerStatementAddHandlerOrRemoveHandlerKeyword(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerKeyword, True, False)
			Return flag
		End Function

		Public Shared Function IsAddressOfOperand(ByVal node As ExpressionSyntax) As Boolean
			Dim parent As VisualBasicSyntaxNode = node.Parent
			If (parent Is Nothing) Then
				Return False
			End If
			Return parent.Kind() = SyntaxKind.AddressOfExpression
		End Function

		Public Shared Function IsAnyToken(ByVal kind As SyntaxKind) As Boolean
			If (kind < SyntaxKind.AddHandlerKeyword) Then
				Return False
			End If
			Return kind <= SyntaxKind.CharacterLiteralToken
		End Function

		Public Shared Function IsAssignmentStatement(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleAssignmentStatement) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement) OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RightShiftAssignmentStatement) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List), True, False)
			Return flag
		End Function

		Public Shared Function IsAssignmentStatementOperatorToken(ByVal kind As SyntaxKind) As Boolean
			Dim flag As Boolean
			Select Case kind
				Case SyntaxKind.EqualsToken
				Case SyntaxKind.AmpersandEqualsToken
				Case SyntaxKind.AsteriskEqualsToken
				Case SyntaxKind.PlusEqualsToken
				Case SyntaxKind.MinusEqualsToken
				Case SyntaxKind.SlashEqualsToken
				Case SyntaxKind.BackslashEqualsToken
				Case SyntaxKind.CaretEqualsToken
				Case SyntaxKind.LessThanLessThanEqualsToken
				Case SyntaxKind.GreaterThanGreaterThanEqualsToken
					flag = True
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
				Label0:
					flag = False
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return flag
		End Function

		Public Shared Function IsAttributeName(ByVal node As SyntaxNode) As Boolean
			Dim flag As Boolean
			Dim parent As SyntaxNode = node
			While True
				If (parent IsNot Nothing) Then
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = parent.Kind()
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Attribute) Then
						Dim attributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax = DirectCast(parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax)
						If (attributeSyntax.Name <> node) Then
							Dim name As QualifiedNameSyntax = TryCast(attributeSyntax.Name, QualifiedNameSyntax)
							flag = If(name Is Nothing, False, name.Right = node)
							Exit While
						Else
							flag = True
							Exit While
						End If
					ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierName OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QualifiedName) Then
						parent = parent.Parent
					Else
						flag = False
						Exit While
					End If
				Else
					flag = False
					Exit While
				End If
			End While
			Return flag
		End Function

		Public Shared Function IsAttributeTargetAttributeModifier(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AssemblyKeyword, True, False)
			Return flag
		End Function

		Friend Shared Function IsBinaryDigit(ByVal c As Char) As Boolean
			Return c >= "0"C And c <= "1"C Or c >= "０"C And c <= "１"C
		End Function

		Public Shared Function IsBinaryExpression(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddExpression) <= 4 OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExponentiateExpression) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement), True, False)
			Return flag
		End Function

		Public Shared Function IsBinaryExpressionOperatorToken(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LikeKeyword) Then
				If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsKeyword) Then
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsNotKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LikeKeyword) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				Else
					If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndKeyword) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List) OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsKeyword) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				End If
			ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrElseKeyword) Then
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModKeyword OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
					flag = True
					Return flag
				End If
				flag = False
				Return flag
			ElseIf (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword) Then
				Select Case syntaxKind
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AmpersandToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsteriskToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PlusToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MinusToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SlashToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanEqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanGreaterThanToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanEqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BackslashToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaretToken
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndAddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRemoveHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSyncLockStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterSingleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterMultipleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubNewStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeywordEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsPropertyEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesClauseItem Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IncompleteMember Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FieldDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariableDeclarator Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleAsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsNewClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectMemberInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectCollectionInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferredFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionalKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrElseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverloadsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParamArrayKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PartialKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrivateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PublicKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReadOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReDimKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.REMKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ResumeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReturnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShadowsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SharedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShortKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StaticKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StepKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StopKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StringKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ThenKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ThrowKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ToKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TrueKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryCastKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeOfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UIntegerKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ULongKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UShortKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhenKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WideningKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WriteOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GosubKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariantKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WendKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AllKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnsiKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AscendingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AssemblyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AutoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BinaryKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompareKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CustomKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DescendingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DisableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DistinctKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExplicitKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalSourceKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalChecksumKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FromKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GroupKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsFalseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsTrueKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OffKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrderKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OutKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PreserveKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RegionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkipKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StrictKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TakeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TextKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UnicodeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UntilKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WarningKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhereKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsyncKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AwaitKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IteratorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.YieldKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExclamationToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AtToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HashToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AmpersandToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseParenToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseBraceToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SemicolonToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DotToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ColonToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LabelStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsValue Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModifiedIdentifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LabelStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GoToStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParamArrayKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanToken
						flag = False
						Return flag
					Case Else
						If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanLessThanToken) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
							flag = False
							Return flag
						Else
							Exit Select
						End If
				End Select
			End If
			flag = True
			Return flag
		End Function

		Friend Shared Function IsBlockStatement(ByVal possibleBlock As SyntaxNode, ByRef beginStatement As StatementSyntax, ByRef beginTerminator As SyntaxToken, ByRef body As SyntaxList(Of StatementSyntax), ByRef endStatement As StatementSyntax) As Boolean
			Dim flag As Boolean
			beginTerminator = New SyntaxToken()
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = possibleBlock.Kind()
			If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FinallyBlock) Then
				If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseBlock) Then
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseElseBlock) Then
						GoTo Label2
					End If
					If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForBlock) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
						Dim forOrForEachBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachBlockSyntax = DirectCast(possibleBlock, Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachBlockSyntax)
						beginStatement = forOrForEachBlockSyntax.ForOrForEachStatement
						body = forOrForEachBlockSyntax.Statements
						endStatement = forOrForEachBlockSyntax.NextStatement
						flag = True
						Return flag
					Else
						If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleDoLoopBlock) > 4) Then
							flag = False
							Return flag
						End If
						Dim doLoopBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.DoLoopBlockSyntax = DirectCast(possibleBlock, Microsoft.CodeAnalysis.VisualBasic.Syntax.DoLoopBlockSyntax)
						beginStatement = doLoopBlockSyntax.DoStatement
						body = doLoopBlockSyntax.Statements
						endStatement = doLoopBlockSyntax.LoopStatement
						flag = True
						Return flag
					End If
				Else
					If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectBlock) Then
						GoTo Label4
					End If
					Dim selectBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectBlockSyntax = DirectCast(possibleBlock, Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectBlockSyntax)
					beginStatement = selectBlockSyntax.SelectStatement
					body = New SyntaxList(Of StatementSyntax)()
					endStatement = selectBlockSyntax.EndSelectStatement
					flag = True
					Return flag
				End If
			Label2:
				Dim caseBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseBlockSyntax = DirectCast(possibleBlock, Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseBlockSyntax)
				beginStatement = caseBlockSyntax.CaseStatement
				body = caseBlockSyntax.Statements
				endStatement = Nothing
				flag = True
			ElseIf (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock) Then
				Select Case syntaxKind
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileBlock
						Dim whileBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileBlockSyntax = DirectCast(possibleBlock, Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileBlockSyntax)
						beginStatement = whileBlockSyntax.WhileStatement
						body = whileBlockSyntax.Statements
						endStatement = whileBlockSyntax.EndWhileStatement
						flag = True
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayRankSpecifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Attribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributesStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrintStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsValue Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModifiedIdentifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayRankSpecifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributeList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Attribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributeTarget Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributesStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExpressionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrintStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileBlock
						flag = False
						Return flag
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock
						Dim usingBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingBlockSyntax = DirectCast(possibleBlock, Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingBlockSyntax)
						beginStatement = usingBlockSyntax.UsingStatement
						body = usingBlockSyntax.Statements
						endStatement = usingBlockSyntax.EndUsingStatement
						flag = True
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockBlock
						Dim syncLockBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockBlockSyntax = DirectCast(possibleBlock, Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockBlockSyntax)
						beginStatement = syncLockBlockSyntax.SyncLockStatement
						body = syncLockBlockSyntax.Statements
						endStatement = syncLockBlockSyntax.EndSyncLockStatement
						flag = True
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithBlock
						Dim withBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.WithBlockSyntax = DirectCast(possibleBlock, Microsoft.CodeAnalysis.VisualBasic.Syntax.WithBlockSyntax)
						beginStatement = withBlockSyntax.WithStatement
						body = withBlockSyntax.Statements
						endStatement = withBlockSyntax.EndWithStatement
						flag = True
						Exit Select
					Case Else
						Select Case syntaxKind
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfStatement
								Dim singleLineIfStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineIfStatementSyntax = DirectCast(possibleBlock, Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineIfStatementSyntax)
								beginStatement = Nothing
								beginTerminator = singleLineIfStatementSyntax.ThenKeyword
								body = singleLineIfStatementSyntax.Statements
								endStatement = Nothing
								flag = True

							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfPart
								flag = False
								Return flag
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineElseClause
								Dim singleLineElseClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineElseClauseSyntax = DirectCast(possibleBlock, Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineElseClauseSyntax)
								beginStatement = Nothing
								beginTerminator = singleLineElseClauseSyntax.ElseKeyword
								body = singleLineElseClauseSyntax.Statements
								endStatement = Nothing
								flag = True

							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineIfBlock
								Dim multiLineIfBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineIfBlockSyntax = DirectCast(possibleBlock, Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineIfBlockSyntax)
								beginStatement = multiLineIfBlockSyntax.IfStatement
								body = multiLineIfBlockSyntax.Statements
								endStatement = multiLineIfBlockSyntax.EndIfStatement
								flag = True

							Case Else
								Select Case syntaxKind
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseIfBlock
										Dim elseIfBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfBlockSyntax = DirectCast(possibleBlock, Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfBlockSyntax)
										beginStatement = elseIfBlockSyntax.ElseIfStatement
										body = elseIfBlockSyntax.Statements
										endStatement = Nothing
										flag = True

									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseBlock
										Dim elseBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseBlockSyntax = DirectCast(possibleBlock, Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseBlockSyntax)
										beginStatement = elseBlockSyntax.ElseStatement
										body = elseBlockSyntax.Statements
										endStatement = Nothing
										flag = True

									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IfStatement
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseIfStatement
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseStatement
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Attribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributesStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NextLabel Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueForStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseStatement
										flag = False
										Return flag
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryBlock
										Dim tryBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TryBlockSyntax = DirectCast(possibleBlock, Microsoft.CodeAnalysis.VisualBasic.Syntax.TryBlockSyntax)
										beginStatement = tryBlockSyntax.TryStatement
										body = tryBlockSyntax.Statements
										endStatement = tryBlockSyntax.EndTryStatement
										flag = True

									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CatchBlock
										Dim catchBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchBlockSyntax = DirectCast(possibleBlock, Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchBlockSyntax)
										beginStatement = catchBlockSyntax.CatchStatement
										body = catchBlockSyntax.Statements
										endStatement = Nothing
										flag = True

									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FinallyBlock
										Dim finallyBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyBlockSyntax = DirectCast(possibleBlock, Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyBlockSyntax)
										beginStatement = finallyBlockSyntax.FinallyStatement
										body = finallyBlockSyntax.Statements
										endStatement = Nothing
										flag = True

									Case Else
										flag = False
										Return flag
								End Select

						End Select

				End Select
			Else
				Select Case syntaxKind
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock
						Dim namespaceBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceBlockSyntax = DirectCast(possibleBlock, Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceBlockSyntax)
						beginStatement = namespaceBlockSyntax.NamespaceStatement
						body = namespaceBlockSyntax.Members
						endStatement = namespaceBlockSyntax.EndNamespaceStatement
						flag = True
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceStatement
						flag = False
						Return flag
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureBlock
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceBlock
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassBlock
						Dim typeBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax = DirectCast(possibleBlock, Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax)
						beginStatement = typeBlockSyntax.BlockStatement
						body = typeBlockSyntax.Members
						endStatement = typeBlockSyntax.EndBlockStatement
						flag = True
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumBlock
						Dim enumBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumBlockSyntax = DirectCast(possibleBlock, Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumBlockSyntax)
						beginStatement = enumBlockSyntax.EnumStatement
						body = enumBlockSyntax.Members
						endStatement = enumBlockSyntax.EndEnumStatement
						flag = True
						Exit Select
					Case Else
						Select Case syntaxKind
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorBlock
								Dim methodBlockBaseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax = DirectCast(possibleBlock, Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax)
								beginStatement = methodBlockBaseSyntax.BlockStatement
								body = methodBlockBaseSyntax.Statements
								endStatement = methodBlockBaseSyntax.EndBlockStatement
								flag = True

							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock
								Dim propertyBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyBlockSyntax = DirectCast(possibleBlock, Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyBlockSyntax)
								beginStatement = propertyBlockSyntax.PropertyStatement
								body = New SyntaxList(Of StatementSyntax)()
								endStatement = propertyBlockSyntax.EndPropertyStatement
								flag = True

							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock
								Dim eventBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventBlockSyntax = DirectCast(possibleBlock, Microsoft.CodeAnalysis.VisualBasic.Syntax.EventBlockSyntax)
								beginStatement = eventBlockSyntax.EventStatement
								body = New SyntaxList(Of StatementSyntax)()
								endStatement = eventBlockSyntax.EndEventStatement
								flag = True

							Case Else
								flag = False
								Return flag
						End Select

				End Select
			End If
			Return flag
		Label4:
			If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseBlock) Then
				GoTo Label2
			End If
			flag = False
			Return flag
		End Function

		Public Shared Function IsCaseBlock(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseBlock OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseElseBlock, True, False)
			Return flag
		End Function

		Public Shared Function IsCaseStatement(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.CaseStatement) > CUShort(SyntaxKind.List), False, True)
		End Function

		Public Shared Function IsColon(ByVal c As Char) As Boolean
			If (c = ":"C) Then
				Return True
			End If
			Return c = "："C
		End Function

		Friend Shared Function IsConnectorPunctuation(ByVal c As Char) As Boolean
			Return CharUnicodeInfo.GetUnicodeCategory(c) = UnicodeCategory.ConnectorPunctuation
		End Function

		Public Shared Function IsContextualKeyword(ByVal kind As SyntaxKind) As Boolean
			If (kind = SyntaxKind.ReferenceKeyword) Then
				Return True
			End If
			If (584 > CInt(kind)) Then
				Return False
			End If
			Return kind <= SyntaxKind.YieldKeyword
		End Function

		Public Shared Function IsContinueStatement(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.ContinueWhileStatement) > CUShort(SyntaxKind.EmptyStatement), False, True)
		End Function

		Public Shared Function IsContinueStatementBlockKeyword(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword, True, False)
			Return flag
		End Function

		Public Shared Function IsCrefOperatorReferenceOperatorToken(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword) Then
				If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CTypeKeyword) Then
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LikeKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				Else
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CTypeKeyword) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				End If
			ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword) Then
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword) Then
					flag = True
					Return flag
				End If
				flag = False
				Return flag
			ElseIf (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsFalseKeyword) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
				Select Case syntaxKind
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AmpersandToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsteriskToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PlusToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MinusToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SlashToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanEqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanGreaterThanToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanEqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BackslashToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaretToken
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndAddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRemoveHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSyncLockStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterSingleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterMultipleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubNewStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeywordEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsPropertyEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesClauseItem Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IncompleteMember Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FieldDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariableDeclarator Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleAsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsNewClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectMemberInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectCollectionInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferredFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionalKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrElseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverloadsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParamArrayKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PartialKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrivateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PublicKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReadOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReDimKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.REMKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ResumeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReturnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShadowsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SharedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShortKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StaticKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StepKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StopKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StringKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ThenKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ThrowKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ToKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TrueKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryCastKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeOfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UIntegerKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ULongKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UShortKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhenKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WideningKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WriteOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GosubKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariantKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WendKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AllKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnsiKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AscendingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AssemblyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AutoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BinaryKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompareKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CustomKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DescendingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DisableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DistinctKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExplicitKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalSourceKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalChecksumKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FromKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GroupKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsFalseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsTrueKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OffKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrderKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OutKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PreserveKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RegionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkipKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StrictKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TakeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TextKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UnicodeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UntilKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WarningKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhereKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsyncKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AwaitKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IteratorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.YieldKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExclamationToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AtToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HashToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AmpersandToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseParenToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseBraceToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SemicolonToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DotToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ColonToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LabelStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsValue Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModifiedIdentifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LabelStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GoToStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParamArrayKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanToken
						flag = False
						Return flag
					Case Else
						If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanLessThanToken) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
							flag = False
							Return flag
						Else
							Exit Select
						End If
				End Select
			End If
			flag = True
			Return flag
		End Function

		Public Shared Function IsCrefSignaturePartModifier(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByRefKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByValKeyword, True, False)
			Return flag
		End Function

		Friend Shared Function IsDateSeparatorCharacter(ByVal c As Char) As Boolean
			Return c = "/"C Or c = "-"C Or c = "／"C Or c = "－"C
		End Function

		Friend Shared Function IsDecimalDigit(ByVal c As Char) As Boolean
			Return c >= "0"C And c <= "9"C Or c >= "０"C And c <= "９"C
		End Function

		Public Shared Function IsDeclareStatement(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.DeclareSubStatement) > CUShort(SyntaxKind.List), False, True)
		End Function

		Public Shared Function IsDeclareStatementCharsetKeyword(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnsiKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AutoKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UnicodeKeyword, True, False)
			Return flag
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use IsDeclareStatementSubOrFunctionKeyword instead.", True)>
		Public Shared Function IsDeclareStatementKeyword(ByVal kind As SyntaxKind) As Boolean
			Return SyntaxFacts.IsDeclareStatementSubOrFunctionKeyword(kind)
		End Function

		Public Shared Function IsDeclareStatementSubOrFunctionKeyword(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword, True, False)
			Return flag
		End Function

		Public Shared Function IsDelegateStatement(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.DelegateSubStatement) > CUShort(SyntaxKind.List), False, True)
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use IsDelegateStatementSubOrFunctionKeyword instead.", True)>
		Public Shared Function IsDelegateStatementKeyword(ByVal kind As SyntaxKind) As Boolean
			Return SyntaxFacts.IsDelegateStatementSubOrFunctionKeyword(kind)
		End Function

		Public Shared Function IsDelegateStatementSubOrFunctionKeyword(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword, True, False)
			Return flag
		End Function

		Public Shared Function IsDoLoopBlock(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.SimpleDoLoopBlock) > 4, False, True)
		End Function

		Public Shared Function IsDoStatement(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.SimpleDoStatement) > CUShort(SyntaxKind.EmptyStatement), False, True)
		End Function

		Friend Shared Function IsDoubleQuote(c As Char) As Boolean
			' // Besides the half width and full width ", we also check for Unicode
			' // LEFT DOUBLE QUOTATION MARK and RIGHT DOUBLE QUOTATION MARK because
			' // IME editors paste them in. This isn't really technically correct
			' // because we ignore the left-ness or right-ness, but see VS 170991
			Return c = """"c OrElse (c >= LEFT_DOUBLE_QUOTATION_MARK AndAlso (c = FULLWIDTH_QUOTATION_MARK Or c = LEFT_DOUBLE_QUOTATION_MARK Or c = RIGHT_DOUBLE_QUOTATION_MARK))
		End Function

		Friend Shared Function IsEndBlockLoopOrNextStatement(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndAddHandlerStatement) OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NextStatement OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleLoopStatement) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement), True, False)
			Return flag
		End Function

		Public Shared Function IsEndBlockStatement(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.EndIfStatement) > CUShort(SyntaxKind.EndAddHandlerStatement), False, True)
		End Function

		Public Shared Function IsEndBlockStatementBlockKeyword(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceKeyword) Then
				If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventKeyword) Then
					If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassKeyword) Then
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventKeyword) Then
							flag = True
							Return flag
						End If
						flag = False
						Return flag
					Else
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassKeyword) Then
							flag = True
							Return flag
						End If
						flag = False
						Return flag
					End If
				ElseIf (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IfKeyword) Then
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceKeyword) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				Else
					If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionKeyword) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List) OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IfKeyword) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				End If
			ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerKeyword) Then
				If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword) Then
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerKeyword) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				Else
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				End If
			ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword) Then
				If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List) OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword) Then
					flag = True
					Return flag
				End If
				flag = False
				Return flag
			ElseIf (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List) AndAlso syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryKeyword) Then
				Select Case syntaxKind
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithKeyword
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhenKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WideningKeyword
						flag = False
						Return flag
					Case Else
						flag = False
						Return flag
				End Select
			End If
			flag = True
			Return flag
		End Function

		Public Shared Function IsExitStatement(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.ExitDoStatement) > CUShort(SyntaxKind.EndSelectStatement), False, True)
		End Function

		Public Shared Function IsExitStatementBlockKeyword(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword) Then
				If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForKeyword) Then
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				Else
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForKeyword) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				End If
			ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword) Then
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword) Then
					flag = True
					Return flag
				End If
				flag = False
				Return flag
			ElseIf (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword AndAlso syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryKeyword AndAlso syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword) Then
				flag = False
				Return flag
			End If
			flag = True
			Return flag
		End Function

		Friend Shared Function IsFullWidth(ByVal c As Char) As Boolean
			Return c > Strings.ChrW(65280) And c < "｟"C
		End Function

		Friend Shared Function IsHalfWidth(ByVal c As Char) As Boolean
			If (c < "!"C) Then
				Return False
			End If
			Return c <= "~"C
		End Function

		Friend Shared Function IsHandlesContainer(ByVal node As SyntaxNode) As Boolean
			Return TypeOf node Is WithEventsEventContainerSyntax
		End Function

		Friend Shared Function IsHandlesEvent(ByVal node As SyntaxNode) As Boolean
			Dim parent As SyntaxNode = node.Parent
			If (parent Is Nothing OrElse Not parent.IsKind(SyntaxKind.HandlesClauseItem)) Then
				Return False
			End If
			Return TypeOf node Is IdentifierNameSyntax
		End Function

		Friend Shared Function IsHandlesProperty(ByVal node As SyntaxNode) As Boolean
			Dim parent As SyntaxNode = node.Parent
			If (parent Is Nothing OrElse Not parent.IsKind(SyntaxKind.WithEventsPropertyEventContainer)) Then
				Return False
			End If
			Return TypeOf node Is IdentifierNameSyntax
		End Function

		Public Shared Function IsHash(ByVal c As Char) As Boolean
			If (c = "#"C) Then
				Return True
			End If
			Return c = "＃"C
		End Function

		Friend Shared Function IsHexDigit(ByVal c As Char) As Boolean
			If (SyntaxFacts.IsDecimalDigit(c) OrElse c >= "a"C And c <= "f"C OrElse c >= "A"C And c <= "F"C OrElse c >= "ａ"C And c <= "ｆ"C) Then
				Return True
			End If
			Return c >= "Ａ"C And c <= "Ｆ"C
		End Function

		Friend Shared Function IsHighSurrogate(ByVal c As Char) As Boolean
			Return [Char].IsHighSurrogate(c)
		End Function

		Public Shared Function IsIdentifierPartCharacter(ByVal c As Char) As Boolean
			Dim flag As Boolean
			flag = If(c >= Strings.ChrW(128), SyntaxFacts.IsWideIdentifierCharacter(c), SyntaxFacts.IsNarrowIdentifierCharacter(Convert.ToUInt16(c)))
			Return flag
		End Function

		Public Shared Function IsIdentifierStartCharacter(ByVal c As Char) As Boolean
			Dim unicodeCategory As System.Globalization.UnicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c)
			If (SyntaxFacts.IsPropAlpha(unicodeCategory) OrElse SyntaxFacts.IsPropLetterDigit(unicodeCategory)) Then
				Return True
			End If
			Return SyntaxFacts.IsPropConnectorPunctuation(unicodeCategory)
		End Function

		Public Shared Function IsIfDirectiveTrivia(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.IfDirectiveTrivia) > CUShort(SyntaxKind.List), False, True)
		End Function

		Public Shared Function IsIfDirectiveTriviaIfOrElseIfKeyword(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseIfKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IfKeyword, True, False)
			Return flag
		End Function

		Friend Shared Function IsImplementedMember(ByVal node As SyntaxNode) As Boolean
			Dim parent As SyntaxNode = node.Parent
			If (parent Is Nothing) Then
				Return False
			End If
			Return parent.IsKind(SyntaxKind.ImplementsClause)
		End Function

		Public Shared Function IsInNamespaceOrTypeContext(ByVal node As SyntaxNode) As Boolean
			Dim name As Boolean
			If (node Is Nothing) Then
				name = False
				Return name
			ElseIf (TypeOf node Is TypeSyntax) Then
				Dim parent As SyntaxNode = node.Parent
				If (parent IsNot Nothing) Then
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = parent.Kind()
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause) Then
						name = DirectCast(parent, SimpleImportsClauseSyntax).Name = node
						Return name
					ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceStatement) Then
						name = DirectCast(parent, NamespaceStatementSyntax).Name = node
						Return name
					ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QualifiedName) Then
						Dim qualifiedNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax = DirectCast(parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax)
						If (qualifiedNameSyntax.Parent IsNot Nothing AndAlso qualifiedNameSyntax.Parent.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsClause) Then
							GoTo Label2
						End If
						name = DirectCast(parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax).Left = node
						Return name
					End If
				End If
			Label2:
				Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
				If (expressionSyntax Is Nothing) Then
					name = False
					Return name
				End If
				name = SyntaxFacts.IsInTypeOnlyContext(expressionSyntax)
			Else
				name = False
			End If
			Return name
		End Function

		Public Shared Function IsInstanceExpression(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MeKeyword OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MyBaseKeyword) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List), True, False)
			Return flag
		End Function

		Public Shared Function IsInTypeOnlyContext(ByVal node As ExpressionSyntax) As Boolean
			Dim name As Boolean
			If (TypeOf node Is TypeSyntax) Then
				Dim parent As VisualBasicSyntaxNode = node.Parent
				If (parent IsNot Nothing) Then
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = parent.Kind()
					If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeOfIsNotExpression) Then
						If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsNewClause) Then
							If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
								name = True
								Return name
							ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeConstraint) Then
								name = True
								Return name
							Else
								If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleAsClause) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
									GoTo Label5
								End If
								name = DirectCast(parent, AsClauseSyntax).Type() = node
								Return name
							End If
						ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Attribute) Then
							name = DirectCast(parent, AttributeSyntax).Name = node
							Return name
						ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetTypeExpression) Then
							name = DirectCast(parent, GetTypeExpressionSyntax).Type = node
							Return name
						Else
							If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeOfIsExpression) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
								GoTo Label5
							End If
							name = DirectCast(parent, TypeOfExpressionSyntax).Type = node
							Return name
						End If
					ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NullableType) Then
						Select Case syntaxKind
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectCreationExpression
								name = DirectCast(parent, ObjectCreationExpressionSyntax).Type = node
								Return name
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnonymousObjectCreationExpression
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RightShiftAssignmentStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConcatenateAssignmentStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReDimStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReDimPreserveStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetXmlNamespaceExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleMemberAccessExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InvocationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectCreationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnonymousObjectCreationExpression
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DictionaryAccessExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InvocationExpression
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CollectionInitializer
								Exit Select
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayCreationExpression
								name = DirectCast(parent, ArrayCreationExpressionSyntax).Type = node
								Return name
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CTypeExpression
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DirectCastExpression
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryCastExpression
								name = DirectCast(parent, CastExpressionSyntax).Type = node
								Return name
							Case Else
								If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayType) Then
									name = DirectCast(parent, ArrayTypeSyntax).ElementType = node
									Return name
								ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NullableType) Then
									name = DirectCast(parent, NullableTypeSyntax).ElementType = node
									Return name
								Else
									Exit Select
								End If
						End Select
					ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeArgumentList) Then
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QualifiedName) Then
							Dim qualifiedNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax = DirectCast(parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax)
							If (qualifiedNameSyntax.Parent Is Nothing OrElse qualifiedNameSyntax.Parent.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsClause) Then
								name = qualifiedNameSyntax.Right = node
								Return name
							Else
								name = qualifiedNameSyntax.Left = node
								Return name
							End If
						Else
							If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeArgumentList) Then
								GoTo Label5
							End If
							name = True
							Return name
						End If
					ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CrefSignaturePart) Then
						name = True
						Return name
					Else
						If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypedTupleElement) Then
							GoTo Label5
						End If
						name = DirectCast(parent, TypedTupleElementSyntax).Type = node
						Return name
					End If
				End If
			Label5:
				name = False
			Else
				name = False
			End If
			Return name
		End Function

		Public Shared Function IsInvocationOrAddressOfOperand(ByVal node As ExpressionSyntax) As Boolean
			If (SyntaxFacts.IsInvoked(node)) Then
				Return True
			End If
			Return SyntaxFacts.IsAddressOfOperand(node)
		End Function

		Public Shared Function IsInvoked(ByVal node As ExpressionSyntax) As Boolean
			node = SyntaxFactory.GetStandaloneExpression(node)
			Dim parent As InvocationExpressionSyntax = TryCast(node.Parent, InvocationExpressionSyntax)
			If (parent Is Nothing) Then
				Return False
			End If
			Return parent.Expression = node
		End Function

		Public Shared Function IsKeywordEventContainerKeyword(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MeKeyword OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MyBaseKeyword) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List), True, False)
			Return flag
		End Function

		Public Shared Function IsKeywordKind(ByVal kind As SyntaxKind) As Boolean
			Dim flag As Boolean
			Select Case kind
				Case SyntaxKind.AddHandlerKeyword
				Case SyntaxKind.AddressOfKeyword
				Case SyntaxKind.AliasKeyword
				Case SyntaxKind.AndKeyword
				Case SyntaxKind.AndAlsoKeyword
				Case SyntaxKind.AsKeyword
				Case SyntaxKind.BooleanKeyword
				Case SyntaxKind.ByRefKeyword
				Case SyntaxKind.ByteKeyword
				Case SyntaxKind.ByValKeyword
				Case SyntaxKind.CallKeyword
				Case SyntaxKind.CaseKeyword
				Case SyntaxKind.CatchKeyword
				Case SyntaxKind.CBoolKeyword
				Case SyntaxKind.CByteKeyword
				Case SyntaxKind.CCharKeyword
				Case SyntaxKind.CDateKeyword
				Case SyntaxKind.CDecKeyword
				Case SyntaxKind.CDblKeyword
				Case SyntaxKind.CharKeyword
				Case SyntaxKind.CIntKeyword
				Case SyntaxKind.ClassKeyword
				Case SyntaxKind.CLngKeyword
				Case SyntaxKind.CObjKeyword
				Case SyntaxKind.ConstKeyword
				Case SyntaxKind.ReferenceKeyword
				Case SyntaxKind.ContinueKeyword
				Case SyntaxKind.CSByteKeyword
				Case SyntaxKind.CShortKeyword
				Case SyntaxKind.CSngKeyword
				Case SyntaxKind.CStrKeyword
				Case SyntaxKind.CTypeKeyword
				Case SyntaxKind.CUIntKeyword
				Case SyntaxKind.CULngKeyword
				Case SyntaxKind.CUShortKeyword
				Case SyntaxKind.DateKeyword
				Case SyntaxKind.DecimalKeyword
				Case SyntaxKind.DeclareKeyword
				Case SyntaxKind.DefaultKeyword
				Case SyntaxKind.DelegateKeyword
				Case SyntaxKind.DimKeyword
				Case SyntaxKind.DirectCastKeyword
				Case SyntaxKind.DoKeyword
				Case SyntaxKind.DoubleKeyword
				Case SyntaxKind.EachKeyword
				Case SyntaxKind.ElseKeyword
				Case SyntaxKind.ElseIfKeyword
				Case SyntaxKind.EndKeyword
				Case SyntaxKind.EnumKeyword
				Case SyntaxKind.EraseKeyword
				Case SyntaxKind.ErrorKeyword
				Case SyntaxKind.EventKeyword
				Case SyntaxKind.ExitKeyword
				Case SyntaxKind.FalseKeyword
				Case SyntaxKind.FinallyKeyword
				Case SyntaxKind.ForKeyword
				Case SyntaxKind.FriendKeyword
				Case SyntaxKind.FunctionKeyword
				Case SyntaxKind.GetKeyword
				Case SyntaxKind.GetTypeKeyword
				Case SyntaxKind.GetXmlNamespaceKeyword
				Case SyntaxKind.GlobalKeyword
				Case SyntaxKind.GoToKeyword
				Case SyntaxKind.HandlesKeyword
				Case SyntaxKind.IfKeyword
				Case SyntaxKind.ImplementsKeyword
				Case SyntaxKind.ImportsKeyword
				Case SyntaxKind.InKeyword
				Case SyntaxKind.InheritsKeyword
				Case SyntaxKind.IntegerKeyword
				Case SyntaxKind.InterfaceKeyword
				Case SyntaxKind.IsKeyword
				Case SyntaxKind.IsNotKeyword
				Case SyntaxKind.LetKeyword
				Case SyntaxKind.LibKeyword
				Case SyntaxKind.LikeKeyword
				Case SyntaxKind.LongKeyword
				Case SyntaxKind.LoopKeyword
				Case SyntaxKind.MeKeyword
				Case SyntaxKind.ModKeyword
				Case SyntaxKind.ModuleKeyword
				Case SyntaxKind.MustInheritKeyword
				Case SyntaxKind.MustOverrideKeyword
				Case SyntaxKind.MyBaseKeyword
				Case SyntaxKind.MyClassKeyword
				Case SyntaxKind.NamespaceKeyword
				Case SyntaxKind.NarrowingKeyword
				Case SyntaxKind.NextKeyword
				Case SyntaxKind.NewKeyword
				Case SyntaxKind.NotKeyword
				Case SyntaxKind.NothingKeyword
				Case SyntaxKind.NotInheritableKeyword
				Case SyntaxKind.NotOverridableKeyword
				Case SyntaxKind.ObjectKeyword
				Case SyntaxKind.OfKeyword
				Case SyntaxKind.OnKeyword
				Case SyntaxKind.OperatorKeyword
				Case SyntaxKind.OptionKeyword
				Case SyntaxKind.OptionalKeyword
				Case SyntaxKind.OrKeyword
				Case SyntaxKind.OrElseKeyword
				Case SyntaxKind.OverloadsKeyword
				Case SyntaxKind.OverridableKeyword
				Case SyntaxKind.OverridesKeyword
				Case SyntaxKind.ParamArrayKeyword
				Case SyntaxKind.PartialKeyword
				Case SyntaxKind.PrivateKeyword
				Case SyntaxKind.PropertyKeyword
				Case SyntaxKind.ProtectedKeyword
				Case SyntaxKind.PublicKeyword
				Case SyntaxKind.RaiseEventKeyword
				Case SyntaxKind.ReadOnlyKeyword
				Case SyntaxKind.ReDimKeyword
				Case SyntaxKind.REMKeyword
				Case SyntaxKind.RemoveHandlerKeyword
				Case SyntaxKind.ResumeKeyword
				Case SyntaxKind.ReturnKeyword
				Case SyntaxKind.SByteKeyword
				Case SyntaxKind.SelectKeyword
				Case SyntaxKind.SetKeyword
				Case SyntaxKind.ShadowsKeyword
				Case SyntaxKind.SharedKeyword
				Case SyntaxKind.ShortKeyword
				Case SyntaxKind.SingleKeyword
				Case SyntaxKind.StaticKeyword
				Case SyntaxKind.StepKeyword
				Case SyntaxKind.StopKeyword
				Case SyntaxKind.StringKeyword
				Case SyntaxKind.StructureKeyword
				Case SyntaxKind.SubKeyword
				Case SyntaxKind.SyncLockKeyword
				Case SyntaxKind.ThenKeyword
				Case SyntaxKind.ThrowKeyword
				Case SyntaxKind.ToKeyword
				Case SyntaxKind.TrueKeyword
				Case SyntaxKind.TryKeyword
				Case SyntaxKind.TryCastKeyword
				Case SyntaxKind.TypeOfKeyword
				Case SyntaxKind.UIntegerKeyword
				Case SyntaxKind.ULongKeyword
				Case SyntaxKind.UShortKeyword
				Case SyntaxKind.UsingKeyword
				Case SyntaxKind.WhenKeyword
				Case SyntaxKind.WhileKeyword
				Case SyntaxKind.WideningKeyword
				Case SyntaxKind.WithKeyword
				Case SyntaxKind.WithEventsKeyword
				Case SyntaxKind.WriteOnlyKeyword
				Case SyntaxKind.XorKeyword
				Case SyntaxKind.EndIfKeyword
				Case SyntaxKind.GosubKeyword
				Case SyntaxKind.VariantKeyword
				Case SyntaxKind.WendKeyword
				Case SyntaxKind.AggregateKeyword
				Case SyntaxKind.AllKeyword
				Case SyntaxKind.AnsiKeyword
				Case SyntaxKind.AscendingKeyword
				Case SyntaxKind.AssemblyKeyword
				Case SyntaxKind.AutoKeyword
				Case SyntaxKind.BinaryKeyword
				Case SyntaxKind.ByKeyword
				Case SyntaxKind.CompareKeyword
				Case SyntaxKind.CustomKeyword
				Case SyntaxKind.DescendingKeyword
				Case SyntaxKind.DisableKeyword
				Case SyntaxKind.DistinctKeyword
				Case SyntaxKind.EnableKeyword
				Case SyntaxKind.EqualsKeyword
				Case SyntaxKind.ExplicitKeyword
				Case SyntaxKind.ExternalSourceKeyword
				Case SyntaxKind.ExternalChecksumKeyword
				Case SyntaxKind.FromKeyword
				Case SyntaxKind.GroupKeyword
				Case SyntaxKind.InferKeyword
				Case SyntaxKind.IntoKeyword
				Case SyntaxKind.IsFalseKeyword
				Case SyntaxKind.IsTrueKeyword
				Case SyntaxKind.JoinKeyword
				Case SyntaxKind.KeyKeyword
				Case SyntaxKind.MidKeyword
				Case SyntaxKind.OffKeyword
				Case SyntaxKind.OrderKeyword
				Case SyntaxKind.OutKeyword
				Case SyntaxKind.PreserveKeyword
				Case SyntaxKind.RegionKeyword
				Case SyntaxKind.SkipKeyword
				Case SyntaxKind.StrictKeyword
				Case SyntaxKind.TakeKeyword
				Case SyntaxKind.TextKeyword
				Case SyntaxKind.UnicodeKeyword
				Case SyntaxKind.UntilKeyword
				Case SyntaxKind.WarningKeyword
				Case SyntaxKind.WhereKeyword
				Case SyntaxKind.TypeKeyword
				Case SyntaxKind.XmlKeyword
				Case SyntaxKind.AsyncKeyword
				Case SyntaxKind.AwaitKeyword
				Case SyntaxKind.IteratorKeyword
				Case SyntaxKind.YieldKeyword
				Case SyntaxKind.NameOfKeyword
					flag = True
					Exit Select
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.ConcatenateAssignmentStatement Or SyntaxKind.GetXmlNamespaceExpression Or SyntaxKind.SimpleMemberAccessExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlEmptyElement Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlString Or SyntaxKind.AndKeyword Or SyntaxKind.AndAlsoKeyword Or SyntaxKind.AsKeyword
				Case SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.MidExpression Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlPrefixName Or SyntaxKind.AndKeyword
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.PrintStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineElseClause Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.MidExpression Or SyntaxKind.AddHandlerStatement Or SyntaxKind.RaiseEventStatement Or SyntaxKind.ReDimStatement Or SyntaxKind.RedimClause Or SyntaxKind.GetXmlNamespaceExpression Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.XmlDescendantAccessExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.AnonymousObjectCreationExpression Or SyntaxKind.CollectionInitializer Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlPrefixName Or SyntaxKind.XmlBracketedName Or SyntaxKind.XmlComment Or SyntaxKind.XmlCDataSection Or SyntaxKind.ArrayType Or SyntaxKind.PredefinedType Or SyntaxKind.AndKeyword Or SyntaxKind.AsKeyword Or SyntaxKind.ByRefKeyword Or SyntaxKind.ByValKeyword Or SyntaxKind.CaseKeyword Or SyntaxKind.CBoolKeyword
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueDoStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineIfPart Or SyntaxKind.SingleLineElseClause Or SyntaxKind.MultiLineIfBlock Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.ConcatenateAssignmentStatement Or SyntaxKind.MidExpression Or SyntaxKind.CallStatement Or SyntaxKind.AddHandlerStatement Or SyntaxKind.RemoveHandlerStatement Or SyntaxKind.RaiseEventStatement Or SyntaxKind.WithStatement Or SyntaxKind.ReDimStatement Or SyntaxKind.ReDimPreserveStatement Or SyntaxKind.RedimClause Or SyntaxKind.EraseStatement Or SyntaxKind.GetXmlNamespaceExpression Or SyntaxKind.SimpleMemberAccessExpression Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.XmlElementAccessExpression Or SyntaxKind.XmlDescendantAccessExpression Or SyntaxKind.XmlAttributeAccessExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.ObjectCreationExpression Or SyntaxKind.AnonymousObjectCreationExpression Or SyntaxKind.ArrayCreationExpression Or SyntaxKind.CollectionInitializer Or SyntaxKind.CTypeExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlEmptyElement Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlString Or SyntaxKind.XmlPrefixName Or SyntaxKind.XmlName Or SyntaxKind.XmlBracketedName Or SyntaxKind.XmlPrefix Or SyntaxKind.XmlComment Or SyntaxKind.XmlProcessingInstruction Or SyntaxKind.XmlCDataSection Or SyntaxKind.XmlEmbeddedExpression Or SyntaxKind.ArrayType Or SyntaxKind.NullableType Or SyntaxKind.PredefinedType Or SyntaxKind.IdentifierName Or SyntaxKind.AndKeyword Or SyntaxKind.AndAlsoKeyword Or SyntaxKind.AsKeyword Or SyntaxKind.BooleanKeyword Or SyntaxKind.ByRefKeyword Or SyntaxKind.ByteKeyword Or SyntaxKind.ByValKeyword Or SyntaxKind.CallKeyword Or SyntaxKind.CaseKeyword Or SyntaxKind.CatchKeyword Or SyntaxKind.CBoolKeyword Or SyntaxKind.CByteKeyword
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.ConcatenateAssignmentStatement Or SyntaxKind.NotEqualsExpression Or SyntaxKind.LessThanExpression Or SyntaxKind.LessThanOrEqualExpression Or SyntaxKind.GreaterThanOrEqualExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlEmptyElement Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlString Or SyntaxKind.CTypeKeyword Or SyntaxKind.CUIntKeyword Or SyntaxKind.CULngKeyword
				Case SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.MidExpression Or SyntaxKind.NotEqualsExpression Or SyntaxKind.GreaterThanExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlPrefixName Or SyntaxKind.CTypeKeyword
				Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.NewConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Attribute Or SyntaxKind.UsingBlock Or SyntaxKind.NextLabel Or SyntaxKind.ResumeStatement Or SyntaxKind.CaseEqualsClause Or SyntaxKind.RaiseEventStatement Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.NothingLiteralExpression Or SyntaxKind.NotEqualsExpression Or SyntaxKind.OrExpression Or SyntaxKind.AddressOfExpression Or SyntaxKind.MultiLineSubLambdaExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlComment Or SyntaxKind.GenericName Or SyntaxKind.CrefSignaturePart Or SyntaxKind.CTypeKeyword Or SyntaxKind.DeclareKeyword Or SyntaxKind.ElseKeyword
				Case SyntaxKind.List Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.EventBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.NextLabel Or SyntaxKind.StopStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.CaseEqualsClause Or SyntaxKind.CaseNotEqualsClause Or SyntaxKind.RaiseEventStatement Or SyntaxKind.WithStatement Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.TrueLiteralExpression Or SyntaxKind.NothingLiteralExpression Or SyntaxKind.ParenthesizedExpression Or SyntaxKind.NotEqualsExpression Or SyntaxKind.LessThanExpression Or SyntaxKind.OrExpression Or SyntaxKind.ExclusiveOrExpression Or SyntaxKind.AddressOfExpression Or SyntaxKind.BinaryConditionalExpression Or SyntaxKind.MultiLineSubLambdaExpression Or SyntaxKind.SubLambdaHeader Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlEmptyElement Or SyntaxKind.XmlComment Or SyntaxKind.XmlProcessingInstruction Or SyntaxKind.GenericName Or SyntaxKind.QualifiedName Or SyntaxKind.CrefSignaturePart Or SyntaxKind.CrefOperatorReference Or SyntaxKind.CTypeKeyword Or SyntaxKind.CUIntKeyword Or SyntaxKind.DeclareKeyword Or SyntaxKind.DefaultKeyword Or SyntaxKind.ElseKeyword Or SyntaxKind.ElseIfKeyword
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.OptionStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineElseClause Or SyntaxKind.MultiLineIfBlock Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.SelectStatement Or SyntaxKind.ForBlock Or SyntaxKind.MidExpression Or SyntaxKind.CallStatement Or SyntaxKind.RaiseEventStatement Or SyntaxKind.WithStatement Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.XmlElementAccessExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.ObjectCreationExpression Or SyntaxKind.ArrayCreationExpression Or SyntaxKind.NotEqualsExpression Or SyntaxKind.LessThanExpression Or SyntaxKind.GreaterThanExpression Or SyntaxKind.IsExpression Or SyntaxKind.OrExpression Or SyntaxKind.ExclusiveOrExpression Or SyntaxKind.AndAlsoExpression Or SyntaxKind.UnaryPlusExpression Or SyntaxKind.QueryExpression Or SyntaxKind.CollectionRangeVariable Or SyntaxKind.VariableNameEquals Or SyntaxKind.FunctionAggregation Or SyntaxKind.LetClause Or SyntaxKind.AggregateClause Or SyntaxKind.SkipWhileClause Or SyntaxKind.TakeWhileClause Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlEmptyElement Or SyntaxKind.XmlPrefixName Or SyntaxKind.XmlName Or SyntaxKind.XmlComment Or SyntaxKind.XmlProcessingInstruction Or SyntaxKind.ArrayType Or SyntaxKind.NullableType Or SyntaxKind.AndKeyword Or SyntaxKind.AndAlsoKeyword Or SyntaxKind.BooleanKeyword Or SyntaxKind.ByValKeyword Or SyntaxKind.CallKeyword Or SyntaxKind.CBoolKeyword Or SyntaxKind.CByteKeyword Or SyntaxKind.CTypeKeyword Or SyntaxKind.CUIntKeyword Or SyntaxKind.CUShortKeyword Or SyntaxKind.DeclareKeyword Or SyntaxKind.DefaultKeyword Or SyntaxKind.DirectCastKeyword Or SyntaxKind.DoKeyword Or SyntaxKind.GetTypeKeyword Or SyntaxKind.GetXmlNamespaceKeyword Or SyntaxKind.HandlesKeyword Or SyntaxKind.IfKeyword Or SyntaxKind.InKeyword Or SyntaxKind.InheritsKeyword Or SyntaxKind.IsKeyword
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.EventStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.PrintStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineElseClause Or SyntaxKind.FinallyStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectStatement Or SyntaxKind.SyncLockStatement Or SyntaxKind.WhileStatement Or SyntaxKind.ForEachBlock Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.MidExpression Or SyntaxKind.AddHandlerStatement Or SyntaxKind.RaiseEventStatement Or SyntaxKind.ReDimStatement Or SyntaxKind.RedimClause Or SyntaxKind.GetXmlNamespaceExpression Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.XmlDescendantAccessExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.AnonymousObjectCreationExpression Or SyntaxKind.CollectionInitializer Or SyntaxKind.NotEqualsExpression Or SyntaxKind.LessThanOrEqualExpression Or SyntaxKind.GreaterThanExpression Or SyntaxKind.IsNotExpression Or SyntaxKind.OrExpression Or SyntaxKind.AndExpression Or SyntaxKind.AndAlsoExpression Or SyntaxKind.UnaryMinusExpression Or SyntaxKind.QueryExpression Or SyntaxKind.ExpressionRangeVariable Or SyntaxKind.VariableNameEquals Or SyntaxKind.GroupAggregation Or SyntaxKind.LetClause Or SyntaxKind.DistinctClause Or SyntaxKind.SkipWhileClause Or SyntaxKind.SkipClause Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlPrefixName Or SyntaxKind.XmlBracketedName Or SyntaxKind.XmlComment Or SyntaxKind.XmlCDataSection Or SyntaxKind.ArrayType Or SyntaxKind.PredefinedType Or SyntaxKind.AndKeyword Or SyntaxKind.AsKeyword Or SyntaxKind.ByRefKeyword Or SyntaxKind.ByValKeyword Or SyntaxKind.CaseKeyword Or SyntaxKind.CBoolKeyword Or SyntaxKind.CTypeKeyword Or SyntaxKind.CULngKeyword Or SyntaxKind.DateKeyword Or SyntaxKind.DeclareKeyword Or SyntaxKind.DelegateKeyword Or SyntaxKind.DirectCastKeyword Or SyntaxKind.DoubleKeyword Or SyntaxKind.GetTypeKeyword Or SyntaxKind.GlobalKeyword Or SyntaxKind.HandlesKeyword Or SyntaxKind.ImplementsKeyword Or SyntaxKind.InKeyword Or SyntaxKind.IntegerKeyword Or SyntaxKind.IsKeyword
				Case SyntaxKind.EmptyStatement Or SyntaxKind.NotKeyword
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.EndRemoveHandlerStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PrivateKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.PublicKeyword
				Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.NotKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OverridesKeyword
				Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.StepKeyword
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.OptionStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OrElseKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword
				Case 576
				Case SyntaxKind.List Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.AddHandlerAccessorBlock Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.DistinctKeyword
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.FunctionBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.RemoveHandlerAccessorBlock Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.PublicKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.DistinctKeyword
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.NewConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.NotKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.PreserveKeyword
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.AddHandlerAccessorStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.StructureKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AscendingKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.RegionKeyword
				Case SyntaxKind.ExclamationToken
				Case SyntaxKind.AtToken
				Case SyntaxKind.CommaToken
				Case SyntaxKind.HashToken
				Case SyntaxKind.AmpersandToken
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.EndRemoveHandlerStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.EndSyncLockStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.ModuleBlock Or SyntaxKind.StructureBlock Or SyntaxKind.InterfaceBlock Or SyntaxKind.ClassBlock Or SyntaxKind.EnumBlock Or SyntaxKind.InheritsStatement Or SyntaxKind.ImplementsStatement Or SyntaxKind.ModuleStatement Or SyntaxKind.StructureStatement Or SyntaxKind.InterfaceStatement Or SyntaxKind.ClassStatement Or SyntaxKind.EnumStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.SubBlock Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.GetAccessorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.AddHandlerAccessorBlock Or SyntaxKind.RemoveHandlerAccessorBlock Or SyntaxKind.RaiseEventAccessorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.EventBlock Or SyntaxKind.ParameterList Or SyntaxKind.SubStatement Or SyntaxKind.FunctionStatement Or SyntaxKind.SubNewStatement Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.EventStatement Or SyntaxKind.OperatorStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.AddHandlerAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.RaiseEventAccessorStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.HandlesClause Or SyntaxKind.KeywordEventContainer Or SyntaxKind.WithEventsEventContainer Or SyntaxKind.WithEventsPropertyEventContainer Or SyntaxKind.HandlesClauseItem Or SyntaxKind.IncompleteMember Or SyntaxKind.FieldDeclaration Or SyntaxKind.VariableDeclarator Or SyntaxKind.SimpleAsClause Or SyntaxKind.AsNewClause Or SyntaxKind.ObjectMemberInitializer Or SyntaxKind.ObjectCollectionInitializer Or SyntaxKind.InferredFieldInitializer Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OrElseKeyword Or SyntaxKind.OverloadsKeyword Or SyntaxKind.OverridableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PrivateKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.PublicKeyword Or SyntaxKind.RaiseEventKeyword Or SyntaxKind.ReadOnlyKeyword Or SyntaxKind.ReDimKeyword Or SyntaxKind.REMKeyword Or SyntaxKind.RemoveHandlerKeyword Or SyntaxKind.ResumeKeyword Or SyntaxKind.ReturnKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.StaticKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.StructureKeyword Or SyntaxKind.SubKeyword Or SyntaxKind.SyncLockKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ThrowKeyword Or SyntaxKind.ToKeyword Or SyntaxKind.TrueKeyword Or SyntaxKind.TryKeyword Or SyntaxKind.TryCastKeyword Or SyntaxKind.TypeOfKeyword Or SyntaxKind.UIntegerKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UShortKeyword Or SyntaxKind.UsingKeyword Or SyntaxKind.WhenKeyword Or SyntaxKind.WhileKeyword Or SyntaxKind.WideningKeyword Or SyntaxKind.WithKeyword Or SyntaxKind.WithEventsKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AscendingKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.AutoKeyword Or SyntaxKind.BinaryKeyword Or SyntaxKind.ByKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.DisableKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.EnableKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.ExplicitKeyword Or SyntaxKind.ExternalSourceKeyword Or SyntaxKind.ExternalChecksumKeyword Or SyntaxKind.FromKeyword Or SyntaxKind.GroupKeyword Or SyntaxKind.InferKeyword Or SyntaxKind.IntoKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OffKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OutKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.RegionKeyword Or SyntaxKind.SkipKeyword Or SyntaxKind.StrictKeyword Or SyntaxKind.TakeKeyword Or SyntaxKind.TextKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.UntilKeyword Or SyntaxKind.WarningKeyword Or SyntaxKind.WhereKeyword Or SyntaxKind.TypeKeyword Or SyntaxKind.XmlKeyword Or SyntaxKind.AsyncKeyword Or SyntaxKind.AwaitKeyword Or SyntaxKind.IteratorKeyword Or SyntaxKind.YieldKeyword Or SyntaxKind.ExclamationToken Or SyntaxKind.AtToken Or SyntaxKind.CommaToken Or SyntaxKind.HashToken Or SyntaxKind.AmpersandToken
				Case SyntaxKind.NamedFieldInitializer Or SyntaxKind.NotKeyword
				Case SyntaxKind.SingleQuoteToken
				Case SyntaxKind.OpenParenToken
				Case SyntaxKind.CloseParenToken
				Case SyntaxKind.OpenBraceToken
				Case SyntaxKind.CloseBraceToken
				Case SyntaxKind.SemicolonToken
				Case SyntaxKind.AsteriskToken
				Case SyntaxKind.PlusToken
				Case SyntaxKind.MinusToken
				Case SyntaxKind.DotToken
				Case SyntaxKind.SlashToken
				Case SyntaxKind.ColonToken
				Case SyntaxKind.LessThanToken
				Case SyntaxKind.LessThanEqualsToken
				Case SyntaxKind.LessThanGreaterThanToken
				Case SyntaxKind.EqualsToken
				Case SyntaxKind.GreaterThanToken
				Case SyntaxKind.GreaterThanEqualsToken
				Case SyntaxKind.BackslashToken
				Case SyntaxKind.EndFunctionStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.UsingBlock Or SyntaxKind.LabelStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.OpenBraceToken Or SyntaxKind.EqualsToken
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.LabelStatement Or SyntaxKind.GoToStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken
				Case SyntaxKind.CaretToken
				Case SyntaxKind.ColonEqualsToken
				Case SyntaxKind.AmpersandEqualsToken
				Case SyntaxKind.AsteriskEqualsToken
				Case SyntaxKind.PlusEqualsToken
				Case SyntaxKind.MinusEqualsToken
				Case SyntaxKind.SlashEqualsToken
				Case SyntaxKind.BackslashEqualsToken
				Case SyntaxKind.CaretEqualsToken
				Case SyntaxKind.LessThanLessThanToken
				Case SyntaxKind.GreaterThanGreaterThanToken
				Case SyntaxKind.LessThanLessThanEqualsToken
				Case SyntaxKind.GreaterThanGreaterThanEqualsToken
				Case SyntaxKind.QuestionToken
				Case SyntaxKind.DoubleQuoteToken
				Case SyntaxKind.StatementTerminatorToken
				Case SyntaxKind.EndOfFileToken
				Case SyntaxKind.EmptyToken
				Case SyntaxKind.SlashGreaterThanToken
				Case SyntaxKind.List Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.OptionStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.SlashGreaterThanToken
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.NotKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.OpenParenToken Or SyntaxKind.PlusToken Or SyntaxKind.DotToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.SlashGreaterThanToken
				Case SyntaxKind.LessThanSlashToken
				Case SyntaxKind.LessThanExclamationMinusMinusToken
				Case SyntaxKind.MinusMinusGreaterThanToken
				Case SyntaxKind.LessThanQuestionToken
				Case SyntaxKind.QuestionGreaterThanToken
				Case SyntaxKind.LessThanPercentEqualsToken
				Case SyntaxKind.PercentGreaterThanToken
				Case SyntaxKind.BeginCDataToken
				Case SyntaxKind.EndCDataToken
				Case SyntaxKind.EndOfXmlToken
				Case SyntaxKind.BadToken
				Case SyntaxKind.XmlNameToken
				Case SyntaxKind.XmlTextLiteralToken
				Case SyntaxKind.XmlEntityLiteralToken
				Case SyntaxKind.DocumentationCommentLineBreakToken
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.ModuleBlock Or SyntaxKind.ImplementsStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.UsingBlock Or SyntaxKind.WithBlock Or SyntaxKind.NextLabel Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.ElseStatement Or SyntaxKind.NotKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.ReadOnlyKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ToKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UsingKeyword Or SyntaxKind.OpenParenToken Or SyntaxKind.PlusToken Or SyntaxKind.DotToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.PlusEqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.BeginCDataToken Or SyntaxKind.XmlEntityLiteralToken
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.EndSyncLockStatement Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.ModuleBlock Or SyntaxKind.StructureBlock Or SyntaxKind.InheritsStatement Or SyntaxKind.ImplementsStatement Or SyntaxKind.ModuleStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.WithBlock Or SyntaxKind.LocalDeclarationStatement Or SyntaxKind.NextLabel Or SyntaxKind.StopStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineIfPart Or SyntaxKind.ElseStatement Or SyntaxKind.TryBlock Or SyntaxKind.CatchBlock Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PrivateKeyword Or SyntaxKind.RaiseEventKeyword Or SyntaxKind.ReadOnlyKeyword Or SyntaxKind.ReDimKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.StructureKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ThrowKeyword Or SyntaxKind.ToKeyword Or SyntaxKind.TrueKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UShortKeyword Or SyntaxKind.UsingKeyword Or SyntaxKind.WhenKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.DotToken Or SyntaxKind.SlashToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.BackslashToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.AsteriskEqualsToken Or SyntaxKind.PlusEqualsToken Or SyntaxKind.MinusEqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.QuestionToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanSlashToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.PercentGreaterThanToken Or SyntaxKind.BeginCDataToken Or SyntaxKind.EndCDataToken Or SyntaxKind.XmlEntityLiteralToken Or SyntaxKind.DocumentationCommentLineBreakToken
				Case SyntaxKind.IdentifierToken
				Case SyntaxKind.IntegerLiteralToken
				Case SyntaxKind.FloatingLiteralToken
				Case SyntaxKind.DecimalLiteralToken
				Case SyntaxKind.DateLiteralToken
				Case SyntaxKind.StringLiteralToken
				Case SyntaxKind.CharacterLiteralToken
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken
				Case SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.OpenBraceToken Or SyntaxKind.DateLiteralToken
				Case SyntaxKind.SkippedTokensTrivia
				Case SyntaxKind.DocumentationCommentTrivia
				Case SyntaxKind.XmlCrefAttribute
				Case SyntaxKind.XmlNameAttribute
				Case SyntaxKind.ConditionalAccessExpression
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.NewConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.NotKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.OpenParenToken Or SyntaxKind.PlusToken Or SyntaxKind.DotToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.XmlNameAttribute
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectBlock Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AscendingKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.DotToken Or SyntaxKind.SlashToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression
				Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.NewConstraint Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.Attribute Or SyntaxKind.PrintStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.SelectStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.OpenBraceToken Or SyntaxKind.PlusToken Or SyntaxKind.ColonToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.XmlNameAttribute
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.SelectStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OrElseKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.AutoKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.ColonToken Or SyntaxKind.LessThanToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.PrintStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OverloadsKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.BinaryKeyword Or SyntaxKind.OpenParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.PlusToken Or SyntaxKind.DotToken Or SyntaxKind.ColonToken Or SyntaxKind.LessThanEqualsToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlNameAttribute
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.SubBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.OnErrorResumeNextStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectBlock Or SyntaxKind.SelectStatement Or SyntaxKind.CaseBlock Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OrElseKeyword Or SyntaxKind.OverloadsKeyword Or SyntaxKind.OverridableKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AscendingKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.AutoKeyword Or SyntaxKind.BinaryKeyword Or SyntaxKind.ByKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.AsteriskToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.DotToken Or SyntaxKind.SlashToken Or SyntaxKind.ColonToken Or SyntaxKind.LessThanToken Or SyntaxKind.LessThanEqualsToken Or SyntaxKind.LessThanGreaterThanToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlCrefAttribute Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression
				Case SyntaxKind.EndFunctionStatement Or SyntaxKind.FunctionBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.UsingBlock Or SyntaxKind.NotKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.EqualsToken Or SyntaxKind.DateLiteralToken
				Case SyntaxKind.List Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.FunctionBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.UsingBlock Or SyntaxKind.WithBlock Or SyntaxKind.FinallyStatement Or SyntaxKind.CaseElseBlock Or SyntaxKind.NotKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.OpenParenToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.CharacterLiteralToken
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.GetAccessorBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.WithBlock Or SyntaxKind.LocalDeclarationStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.CaseElseBlock Or SyntaxKind.CaseStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PrivateKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.DisableKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.BackslashToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken
				Case SyntaxKind.EndFunctionStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.FunctionBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.UsingBlock Or SyntaxKind.LabelStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.CaseElseStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.OpenBraceToken Or SyntaxKind.EqualsToken Or SyntaxKind.DateLiteralToken
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.AddHandlerAccessorBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.LabelStatement Or SyntaxKind.GoToStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.CaseElseStatement Or SyntaxKind.ElseCaseClause Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.SkippedTokensTrivia
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.FunctionBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.RemoveHandlerAccessorBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.UsingBlock Or SyntaxKind.WithBlock Or SyntaxKind.LabelStatement Or SyntaxKind.IdentifierLabel Or SyntaxKind.FinallyStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.CaseElseBlock Or SyntaxKind.CaseElseStatement Or SyntaxKind.SimpleCaseClause Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.PublicKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.OpenParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.CaretToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.DocumentationCommentTrivia
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.EndRemoveHandlerStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.GetAccessorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.AddHandlerAccessorBlock Or SyntaxKind.RemoveHandlerAccessorBlock Or SyntaxKind.RaiseEventAccessorBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.WithBlock Or SyntaxKind.LocalDeclarationStatement Or SyntaxKind.LabelStatement Or SyntaxKind.GoToStatement Or SyntaxKind.IdentifierLabel Or SyntaxKind.NumericLabel Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.OnErrorResumeNextStatement Or SyntaxKind.CaseElseBlock Or SyntaxKind.CaseStatement Or SyntaxKind.CaseElseStatement Or SyntaxKind.ElseCaseClause Or SyntaxKind.SimpleCaseClause Or SyntaxKind.RangeCaseClause Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PrivateKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.PublicKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.DisableKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.EnableKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.AsteriskToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.BackslashToken Or SyntaxKind.CaretToken Or SyntaxKind.ColonEqualsToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlCrefAttribute
				Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.NewConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Attribute Or SyntaxKind.UsingBlock Or SyntaxKind.NextLabel Or SyntaxKind.ResumeStatement Or SyntaxKind.CaseEqualsClause Or SyntaxKind.NotKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.PlusToken Or SyntaxKind.EqualsToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.XmlNameAttribute
				Case SyntaxKind.WhitespaceTrivia
				Case SyntaxKind.EndOfLineTrivia
				Case SyntaxKind.ColonTrivia
				Case SyntaxKind.CommentTrivia
				Case SyntaxKind.LineContinuationTrivia
				Case SyntaxKind.DocumentationCommentExteriorTrivia
				Case SyntaxKind.DisabledTextTrivia
				Case SyntaxKind.ConstDirectiveTrivia
				Case SyntaxKind.IfDirectiveTrivia
				Case SyntaxKind.ElseIfDirectiveTrivia
				Case SyntaxKind.ElseDirectiveTrivia
				Case SyntaxKind.EndIfDirectiveTrivia
				Case SyntaxKind.RegionDirectiveTrivia
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.EventStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.SyncLockStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OpenParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.EventStatement Or SyntaxKind.OperatorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueDoStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.OnErrorResumeNextStatement Or SyntaxKind.SyncLockStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.StaticKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OffKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OutKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.AsteriskToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.QuestionToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.StatementTerminatorToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.EmptyToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlCrefAttribute Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.IfDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.ElseDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia Or SyntaxKind.RegionDirectiveTrivia
				Case SyntaxKind.EndRegionDirectiveTrivia
				Case SyntaxKind.ExternalSourceDirectiveTrivia
				Case SyntaxKind.EndExternalSourceDirectiveTrivia
				Case SyntaxKind.ExternalChecksumDirectiveTrivia
				Case SyntaxKind.EnableWarningDirectiveTrivia
				Case SyntaxKind.DisableWarningDirectiveTrivia
				Case SyntaxKind.ReferenceDirectiveTrivia
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.SubBlock Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.EventStatement Or SyntaxKind.OperatorStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.AddHandlerAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.RaiseEventAccessorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueDoStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineIfPart Or SyntaxKind.SingleLineElseClause Or SyntaxKind.MultiLineIfBlock Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.OnErrorResumeNextStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectBlock Or SyntaxKind.SelectStatement Or SyntaxKind.CaseBlock Or SyntaxKind.SyncLockStatement Or SyntaxKind.WhileStatement Or SyntaxKind.ForBlock Or SyntaxKind.ForEachBlock Or SyntaxKind.ForStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OrElseKeyword Or SyntaxKind.OverloadsKeyword Or SyntaxKind.OverridableKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.StaticKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.StructureKeyword Or SyntaxKind.SubKeyword Or SyntaxKind.SyncLockKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AscendingKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.AutoKeyword Or SyntaxKind.BinaryKeyword Or SyntaxKind.ByKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OffKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OutKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.RegionKeyword Or SyntaxKind.SkipKeyword Or SyntaxKind.StrictKeyword Or SyntaxKind.TakeKeyword Or SyntaxKind.TextKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.AsteriskToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.DotToken Or SyntaxKind.SlashToken Or SyntaxKind.ColonToken Or SyntaxKind.LessThanToken Or SyntaxKind.LessThanEqualsToken Or SyntaxKind.LessThanGreaterThanToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.QuestionToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.StatementTerminatorToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.EmptyToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanSlashToken Or SyntaxKind.LessThanExclamationMinusMinusToken Or SyntaxKind.MinusMinusGreaterThanToken Or SyntaxKind.LessThanQuestionToken Or SyntaxKind.QuestionGreaterThanToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlCrefAttribute Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.IfDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.ElseDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia Or SyntaxKind.RegionDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.ExternalSourceDirectiveTrivia Or SyntaxKind.EndExternalSourceDirectiveTrivia Or SyntaxKind.ExternalChecksumDirectiveTrivia Or SyntaxKind.EnableWarningDirectiveTrivia Or SyntaxKind.DisableWarningDirectiveTrivia Or SyntaxKind.ReferenceDirectiveTrivia
				Case SyntaxKind.EndFunctionStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.FunctionBlock Or SyntaxKind.DeclareSubStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.UsingBlock Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ForStepClause Or SyntaxKind.NotKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.ConstDirectiveTrivia
				Case SyntaxKind.BadDirectiveTrivia
				Case SyntaxKind.ImportAliasClause
				Case SyntaxKind.NameColonEquals
				Case SyntaxKind.SimpleDoLoopBlock
				Case SyntaxKind.DoWhileLoopBlock
				Case SyntaxKind.DoUntilLoopBlock
				Case SyntaxKind.DoLoopWhileBlock
				Case SyntaxKind.DoLoopUntilBlock
				Case SyntaxKind.List Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.OptionStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.InheritsStatement Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.EventBlock Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.HandlesClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.NextLabel Or SyntaxKind.StopStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.ElseStatement Or SyntaxKind.TryBlock Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.CaseEqualsClause Or SyntaxKind.CaseNotEqualsClause Or SyntaxKind.ForStepClause Or SyntaxKind.ForEachStatement Or SyntaxKind.MidAssignmentStatement Or SyntaxKind.AddAssignmentStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.RaiseEventKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ThrowKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UShortKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.ExplicitKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.RegionKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.UntilKeyword Or SyntaxKind.IteratorKeyword Or SyntaxKind.YieldKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.AsteriskEqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.PercentGreaterThanToken Or SyntaxKind.XmlEntityLiteralToken Or SyntaxKind.DocumentationCommentLineBreakToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression Or SyntaxKind.WhitespaceTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.IfDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.ExternalSourceDirectiveTrivia Or SyntaxKind.BadDirectiveTrivia Or SyntaxKind.DoLoopUntilBlock
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.ModuleBlock Or SyntaxKind.ImplementsStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.NewConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.KeywordEventContainer Or SyntaxKind.VariableDeclarator Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.UsingBlock Or SyntaxKind.WithBlock Or SyntaxKind.NextLabel Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.ElseStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.CaseElseBlock Or SyntaxKind.CaseEqualsClause Or SyntaxKind.CaseLessThanClause Or SyntaxKind.SyncLockStatement Or SyntaxKind.WhileStatement Or SyntaxKind.ForStepClause Or SyntaxKind.NextStatement Or SyntaxKind.MidAssignmentStatement Or SyntaxKind.SubtractAssignmentStatement Or SyntaxKind.NotKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.ReadOnlyKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ToKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UsingKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.ExternalSourceKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.WarningKeyword Or SyntaxKind.IteratorKeyword Or SyntaxKind.ExclamationToken Or SyntaxKind.OpenParenToken Or SyntaxKind.PlusToken Or SyntaxKind.DotToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.PlusEqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.BeginCDataToken Or SyntaxKind.XmlEntityLiteralToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.XmlNameAttribute Or SyntaxKind.EndOfLineTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.EndExternalSourceDirectiveTrivia Or SyntaxKind.ImportAliasClause Or SyntaxKind.DoLoopUntilBlock
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.EndSyncLockStatement Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.ModuleBlock Or SyntaxKind.StructureBlock Or SyntaxKind.InheritsStatement Or SyntaxKind.ImplementsStatement Or SyntaxKind.ModuleStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.GetAccessorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.EventBlock Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.AddHandlerAccessorStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.HandlesClause Or SyntaxKind.KeywordEventContainer Or SyntaxKind.WithEventsEventContainer Or SyntaxKind.VariableDeclarator Or SyntaxKind.SimpleAsClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.WithBlock Or SyntaxKind.LocalDeclarationStatement Or SyntaxKind.NextLabel Or SyntaxKind.StopStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineIfPart Or SyntaxKind.ElseStatement Or SyntaxKind.TryBlock Or SyntaxKind.CatchBlock Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectBlock Or SyntaxKind.CaseElseBlock Or SyntaxKind.CaseStatement Or SyntaxKind.CaseEqualsClause Or SyntaxKind.CaseNotEqualsClause Or SyntaxKind.CaseLessThanClause Or SyntaxKind.CaseLessThanOrEqualClause Or SyntaxKind.SyncLockStatement Or SyntaxKind.WhileStatement Or SyntaxKind.ForStepClause Or SyntaxKind.ForEachStatement Or SyntaxKind.NextStatement Or SyntaxKind.UsingStatement Or SyntaxKind.MidAssignmentStatement Or SyntaxKind.AddAssignmentStatement Or SyntaxKind.SubtractAssignmentStatement Or SyntaxKind.MultiplyAssignmentStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PrivateKeyword Or SyntaxKind.RaiseEventKeyword Or SyntaxKind.ReadOnlyKeyword Or SyntaxKind.ReDimKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.StructureKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ThrowKeyword Or SyntaxKind.ToKeyword Or SyntaxKind.TrueKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UShortKeyword Or SyntaxKind.UsingKeyword Or SyntaxKind.WhenKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AscendingKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.DisableKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.ExplicitKeyword Or SyntaxKind.ExternalSourceKeyword Or SyntaxKind.ExternalChecksumKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.RegionKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.UntilKeyword Or SyntaxKind.WarningKeyword Or SyntaxKind.WhereKeyword Or SyntaxKind.IteratorKeyword Or SyntaxKind.YieldKeyword Or SyntaxKind.ExclamationToken Or SyntaxKind.AtToken Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.DotToken Or SyntaxKind.SlashToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.BackslashToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.AsteriskEqualsToken Or SyntaxKind.PlusEqualsToken Or SyntaxKind.MinusEqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.QuestionToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanSlashToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.PercentGreaterThanToken Or SyntaxKind.BeginCDataToken Or SyntaxKind.EndCDataToken Or SyntaxKind.XmlEntityLiteralToken Or SyntaxKind.DocumentationCommentLineBreakToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression Or SyntaxKind.WhitespaceTrivia Or SyntaxKind.EndOfLineTrivia Or SyntaxKind.ColonTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.IfDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.ElseDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.ExternalSourceDirectiveTrivia Or SyntaxKind.EndExternalSourceDirectiveTrivia Or SyntaxKind.ExternalChecksumDirectiveTrivia Or SyntaxKind.BadDirectiveTrivia Or SyntaxKind.ImportAliasClause Or SyntaxKind.NameColonEquals Or SyntaxKind.DoLoopUntilBlock
				Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.NamespaceBlock Or SyntaxKind.InterfaceBlock Or SyntaxKind.StructureStatement Or SyntaxKind.NewConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.ParameterList Or SyntaxKind.DeclareSubStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.WithEventsPropertyEventContainer Or SyntaxKind.AsNewClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.Attribute Or SyntaxKind.PrintStatement Or SyntaxKind.UsingBlock Or SyntaxKind.LabelStatement Or SyntaxKind.NextLabel Or SyntaxKind.EndStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.SingleLineElseClause Or SyntaxKind.ElseIfBlock Or SyntaxKind.ElseStatement Or SyntaxKind.FinallyBlock Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.SelectStatement Or SyntaxKind.CaseElseStatement Or SyntaxKind.CaseEqualsClause Or SyntaxKind.ForStepClause Or SyntaxKind.MidAssignmentStatement Or SyntaxKind.DivideAssignmentStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.REMKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.TryKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.WhileKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.FromKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.SkipKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.TypeKeyword Or SyntaxKind.IteratorKeyword Or SyntaxKind.CommaToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.PlusToken Or SyntaxKind.ColonToken Or SyntaxKind.EqualsToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.SlashEqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanExclamationMinusMinusToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.EndOfXmlToken Or SyntaxKind.XmlEntityLiteralToken Or SyntaxKind.IdentifierToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.XmlNameAttribute Or SyntaxKind.CommentTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.EnableWarningDirectiveTrivia Or SyntaxKind.SimpleDoLoopBlock Or SyntaxKind.DoLoopUntilBlock
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.OptionStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.InterfaceBlock Or SyntaxKind.ClassBlock Or SyntaxKind.InheritsStatement Or SyntaxKind.StructureStatement Or SyntaxKind.InterfaceStatement Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.AddHandlerAccessorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.EventBlock Or SyntaxKind.ParameterList Or SyntaxKind.SubStatement Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.HandlesClause Or SyntaxKind.WithEventsPropertyEventContainer Or SyntaxKind.HandlesClauseItem Or SyntaxKind.AsNewClause Or SyntaxKind.ObjectMemberInitializer Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.LabelStatement Or SyntaxKind.GoToStatement Or SyntaxKind.NextLabel Or SyntaxKind.StopStatement Or SyntaxKind.EndStatement Or SyntaxKind.ExitDoStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineElseClause Or SyntaxKind.MultiLineIfBlock Or SyntaxKind.ElseIfBlock Or SyntaxKind.ElseBlock Or SyntaxKind.ElseStatement Or SyntaxKind.TryBlock Or SyntaxKind.FinallyBlock Or SyntaxKind.TryStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.SelectStatement Or SyntaxKind.CaseElseStatement Or SyntaxKind.ElseCaseClause Or SyntaxKind.CaseEqualsClause Or SyntaxKind.CaseNotEqualsClause Or SyntaxKind.ForBlock Or SyntaxKind.ForStepClause Or SyntaxKind.ForEachStatement Or SyntaxKind.MidAssignmentStatement Or SyntaxKind.AddAssignmentStatement Or SyntaxKind.DivideAssignmentStatement Or SyntaxKind.IntegerDivideAssignmentStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OrElseKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.RaiseEventKeyword Or SyntaxKind.REMKeyword Or SyntaxKind.RemoveHandlerKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ThrowKeyword Or SyntaxKind.TryKeyword Or SyntaxKind.TryCastKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UShortKeyword Or SyntaxKind.WhileKeyword Or SyntaxKind.WideningKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.AutoKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.ExplicitKeyword Or SyntaxKind.FromKeyword Or SyntaxKind.GroupKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OffKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.RegionKeyword Or SyntaxKind.SkipKeyword Or SyntaxKind.StrictKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.UntilKeyword Or SyntaxKind.TypeKeyword Or SyntaxKind.XmlKeyword Or SyntaxKind.IteratorKeyword Or SyntaxKind.YieldKeyword Or SyntaxKind.CommaToken Or SyntaxKind.HashToken Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.ColonToken Or SyntaxKind.LessThanToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.AsteriskEqualsToken Or SyntaxKind.SlashEqualsToken Or SyntaxKind.BackslashEqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.StatementTerminatorToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanExclamationMinusMinusToken Or SyntaxKind.MinusMinusGreaterThanToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.PercentGreaterThanToken Or SyntaxKind.EndOfXmlToken Or SyntaxKind.BadToken Or SyntaxKind.XmlEntityLiteralToken Or SyntaxKind.DocumentationCommentLineBreakToken Or SyntaxKind.IdentifierToken Or SyntaxKind.IntegerLiteralToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression Or SyntaxKind.WhitespaceTrivia Or SyntaxKind.CommentTrivia Or SyntaxKind.LineContinuationTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.IfDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia Or SyntaxKind.RegionDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.ExternalSourceDirectiveTrivia Or SyntaxKind.EnableWarningDirectiveTrivia Or SyntaxKind.DisableWarningDirectiveTrivia Or SyntaxKind.BadDirectiveTrivia Or SyntaxKind.SimpleDoLoopBlock Or SyntaxKind.DoWhileLoopBlock Or SyntaxKind.DoLoopUntilBlock
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.NamespaceBlock Or SyntaxKind.ModuleBlock Or SyntaxKind.InterfaceBlock Or SyntaxKind.EnumBlock Or SyntaxKind.ImplementsStatement Or SyntaxKind.StructureStatement Or SyntaxKind.ClassStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.FunctionBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.RemoveHandlerAccessorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.ParameterList Or SyntaxKind.FunctionStatement Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.EventStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.KeywordEventContainer Or SyntaxKind.WithEventsPropertyEventContainer Or SyntaxKind.IncompleteMember Or SyntaxKind.VariableDeclarator Or SyntaxKind.AsNewClause Or SyntaxKind.ObjectCollectionInitializer Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.PrintStatement Or SyntaxKind.UsingBlock Or SyntaxKind.WithBlock Or SyntaxKind.LabelStatement Or SyntaxKind.IdentifierLabel Or SyntaxKind.NextLabel Or SyntaxKind.EndStatement Or SyntaxKind.ExitForStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineElseClause Or SyntaxKind.ElseIfBlock Or SyntaxKind.IfStatement Or SyntaxKind.ElseStatement Or SyntaxKind.FinallyBlock Or SyntaxKind.CatchStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectStatement Or SyntaxKind.CaseElseBlock Or SyntaxKind.CaseElseStatement Or SyntaxKind.SimpleCaseClause Or SyntaxKind.CaseEqualsClause Or SyntaxKind.CaseLessThanClause Or SyntaxKind.CaseGreaterThanOrEqualClause Or SyntaxKind.SyncLockStatement Or SyntaxKind.WhileStatement Or SyntaxKind.ForEachBlock Or SyntaxKind.ForStepClause Or SyntaxKind.NextStatement Or SyntaxKind.ThrowStatement Or SyntaxKind.MidAssignmentStatement Or SyntaxKind.SubtractAssignmentStatement Or SyntaxKind.DivideAssignmentStatement Or SyntaxKind.ExponentiateAssignmentStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OverloadsKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.PublicKeyword Or SyntaxKind.ReadOnlyKeyword Or SyntaxKind.REMKeyword Or SyntaxKind.ResumeKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.SubKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ToKeyword Or SyntaxKind.TryKeyword Or SyntaxKind.TypeOfKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UsingKeyword Or SyntaxKind.WhileKeyword Or SyntaxKind.WithKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.BinaryKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.ExternalSourceKeyword Or SyntaxKind.FromKeyword Or SyntaxKind.InferKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.SkipKeyword Or SyntaxKind.TakeKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.WarningKeyword Or SyntaxKind.TypeKeyword Or SyntaxKind.AsyncKeyword Or SyntaxKind.IteratorKeyword Or SyntaxKind.ExclamationToken Or SyntaxKind.CommaToken Or SyntaxKind.AmpersandToken Or SyntaxKind.OpenParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.PlusToken Or SyntaxKind.DotToken Or SyntaxKind.ColonToken Or SyntaxKind.LessThanEqualsToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.CaretToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.PlusEqualsToken Or SyntaxKind.SlashEqualsToken Or SyntaxKind.CaretEqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanExclamationMinusMinusToken Or SyntaxKind.LessThanQuestionToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.BeginCDataToken Or SyntaxKind.EndOfXmlToken Or SyntaxKind.XmlNameToken Or SyntaxKind.XmlEntityLiteralToken Or SyntaxKind.IdentifierToken Or SyntaxKind.FloatingLiteralToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlNameAttribute Or SyntaxKind.EndOfLineTrivia Or SyntaxKind.CommentTrivia Or SyntaxKind.DocumentationCommentExteriorTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.EndExternalSourceDirectiveTrivia Or SyntaxKind.EnableWarningDirectiveTrivia Or SyntaxKind.ReferenceDirectiveTrivia Or SyntaxKind.ImportAliasClause Or SyntaxKind.SimpleDoLoopBlock Or SyntaxKind.DoUntilLoopBlock Or SyntaxKind.DoLoopUntilBlock
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.EndRemoveHandlerStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.EndSyncLockStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.ModuleBlock Or SyntaxKind.StructureBlock Or SyntaxKind.InterfaceBlock Or SyntaxKind.ClassBlock Or SyntaxKind.EnumBlock Or SyntaxKind.InheritsStatement Or SyntaxKind.ImplementsStatement Or SyntaxKind.ModuleStatement Or SyntaxKind.StructureStatement Or SyntaxKind.InterfaceStatement Or SyntaxKind.ClassStatement Or SyntaxKind.EnumStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.SubBlock Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.GetAccessorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.AddHandlerAccessorBlock Or SyntaxKind.RemoveHandlerAccessorBlock Or SyntaxKind.RaiseEventAccessorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.EventBlock Or SyntaxKind.ParameterList Or SyntaxKind.SubStatement Or SyntaxKind.FunctionStatement Or SyntaxKind.SubNewStatement Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.EventStatement Or SyntaxKind.OperatorStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.AddHandlerAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.RaiseEventAccessorStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.HandlesClause Or SyntaxKind.KeywordEventContainer Or SyntaxKind.WithEventsEventContainer Or SyntaxKind.WithEventsPropertyEventContainer Or SyntaxKind.HandlesClauseItem Or SyntaxKind.IncompleteMember Or SyntaxKind.FieldDeclaration Or SyntaxKind.VariableDeclarator Or SyntaxKind.SimpleAsClause Or SyntaxKind.AsNewClause Or SyntaxKind.ObjectMemberInitializer Or SyntaxKind.ObjectCollectionInitializer Or SyntaxKind.InferredFieldInitializer Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.WithBlock Or SyntaxKind.LocalDeclarationStatement Or SyntaxKind.LabelStatement Or SyntaxKind.GoToStatement Or SyntaxKind.IdentifierLabel Or SyntaxKind.NumericLabel Or SyntaxKind.NextLabel Or SyntaxKind.StopStatement Or SyntaxKind.EndStatement Or SyntaxKind.ExitDoStatement Or SyntaxKind.ExitForStatement Or SyntaxKind.ExitSubStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueDoStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineIfPart Or SyntaxKind.SingleLineElseClause Or SyntaxKind.MultiLineIfBlock Or SyntaxKind.ElseIfBlock Or SyntaxKind.ElseBlock Or SyntaxKind.IfStatement Or SyntaxKind.ElseIfStatement Or SyntaxKind.ElseStatement Or SyntaxKind.TryBlock Or SyntaxKind.CatchBlock Or SyntaxKind.FinallyBlock Or SyntaxKind.TryStatement Or SyntaxKind.CatchStatement Or SyntaxKind.CatchFilterClause Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.OnErrorResumeNextStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectBlock Or SyntaxKind.SelectStatement Or SyntaxKind.CaseBlock Or SyntaxKind.CaseElseBlock Or SyntaxKind.CaseStatement Or SyntaxKind.CaseElseStatement Or SyntaxKind.ElseCaseClause Or SyntaxKind.SimpleCaseClause Or SyntaxKind.RangeCaseClause Or SyntaxKind.CaseEqualsClause Or SyntaxKind.CaseNotEqualsClause Or SyntaxKind.CaseLessThanClause Or SyntaxKind.CaseLessThanOrEqualClause Or SyntaxKind.CaseGreaterThanOrEqualClause Or SyntaxKind.CaseGreaterThanClause Or SyntaxKind.SyncLockStatement Or SyntaxKind.WhileStatement Or SyntaxKind.ForBlock Or SyntaxKind.ForEachBlock Or SyntaxKind.ForStatement Or SyntaxKind.ForStepClause Or SyntaxKind.ForEachStatement Or SyntaxKind.NextStatement Or SyntaxKind.UsingStatement Or SyntaxKind.ThrowStatement Or SyntaxKind.SimpleAssignmentStatement Or SyntaxKind.MidAssignmentStatement Or SyntaxKind.AddAssignmentStatement Or SyntaxKind.SubtractAssignmentStatement Or SyntaxKind.MultiplyAssignmentStatement Or SyntaxKind.DivideAssignmentStatement Or SyntaxKind.IntegerDivideAssignmentStatement Or SyntaxKind.ExponentiateAssignmentStatement Or SyntaxKind.LeftShiftAssignmentStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OrElseKeyword Or SyntaxKind.OverloadsKeyword Or SyntaxKind.OverridableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PrivateKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.PublicKeyword Or SyntaxKind.RaiseEventKeyword Or SyntaxKind.ReadOnlyKeyword Or SyntaxKind.ReDimKeyword Or SyntaxKind.REMKeyword Or SyntaxKind.RemoveHandlerKeyword Or SyntaxKind.ResumeKeyword Or SyntaxKind.ReturnKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.StaticKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.StructureKeyword Or SyntaxKind.SubKeyword Or SyntaxKind.SyncLockKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ThrowKeyword Or SyntaxKind.ToKeyword Or SyntaxKind.TrueKeyword Or SyntaxKind.TryKeyword Or SyntaxKind.TryCastKeyword Or SyntaxKind.TypeOfKeyword Or SyntaxKind.UIntegerKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UShortKeyword Or SyntaxKind.UsingKeyword Or SyntaxKind.WhenKeyword Or SyntaxKind.WhileKeyword Or SyntaxKind.WideningKeyword Or SyntaxKind.WithKeyword Or SyntaxKind.WithEventsKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AscendingKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.AutoKeyword Or SyntaxKind.BinaryKeyword Or SyntaxKind.ByKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.DisableKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.EnableKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.ExplicitKeyword Or SyntaxKind.ExternalSourceKeyword Or SyntaxKind.ExternalChecksumKeyword Or SyntaxKind.FromKeyword Or SyntaxKind.GroupKeyword Or SyntaxKind.InferKeyword Or SyntaxKind.IntoKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OffKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OutKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.RegionKeyword Or SyntaxKind.SkipKeyword Or SyntaxKind.StrictKeyword Or SyntaxKind.TakeKeyword Or SyntaxKind.TextKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.UntilKeyword Or SyntaxKind.WarningKeyword Or SyntaxKind.WhereKeyword Or SyntaxKind.TypeKeyword Or SyntaxKind.XmlKeyword Or SyntaxKind.AsyncKeyword Or SyntaxKind.AwaitKeyword Or SyntaxKind.IteratorKeyword Or SyntaxKind.YieldKeyword Or SyntaxKind.ExclamationToken Or SyntaxKind.AtToken Or SyntaxKind.CommaToken Or SyntaxKind.HashToken Or SyntaxKind.AmpersandToken Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.AsteriskToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.DotToken Or SyntaxKind.SlashToken Or SyntaxKind.ColonToken Or SyntaxKind.LessThanToken Or SyntaxKind.LessThanEqualsToken Or SyntaxKind.LessThanGreaterThanToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken Or SyntaxKind.GreaterThanEqualsToken Or SyntaxKind.BackslashToken Or SyntaxKind.CaretToken Or SyntaxKind.ColonEqualsToken Or SyntaxKind.AmpersandEqualsToken Or SyntaxKind.AsteriskEqualsToken Or SyntaxKind.PlusEqualsToken Or SyntaxKind.MinusEqualsToken Or SyntaxKind.SlashEqualsToken Or SyntaxKind.BackslashEqualsToken Or SyntaxKind.CaretEqualsToken Or SyntaxKind.LessThanLessThanToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.QuestionToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.StatementTerminatorToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.EmptyToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanSlashToken Or SyntaxKind.LessThanExclamationMinusMinusToken Or SyntaxKind.MinusMinusGreaterThanToken Or SyntaxKind.LessThanQuestionToken Or SyntaxKind.QuestionGreaterThanToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.PercentGreaterThanToken Or SyntaxKind.BeginCDataToken Or SyntaxKind.EndCDataToken Or SyntaxKind.EndOfXmlToken Or SyntaxKind.BadToken Or SyntaxKind.XmlNameToken Or SyntaxKind.XmlTextLiteralToken Or SyntaxKind.XmlEntityLiteralToken Or SyntaxKind.DocumentationCommentLineBreakToken Or SyntaxKind.IdentifierToken Or SyntaxKind.IntegerLiteralToken Or SyntaxKind.FloatingLiteralToken Or SyntaxKind.DecimalLiteralToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlCrefAttribute Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression Or SyntaxKind.WhitespaceTrivia Or SyntaxKind.EndOfLineTrivia Or SyntaxKind.ColonTrivia Or SyntaxKind.CommentTrivia Or SyntaxKind.LineContinuationTrivia Or SyntaxKind.DocumentationCommentExteriorTrivia Or SyntaxKind.DisabledTextTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.IfDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.ElseDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia Or SyntaxKind.RegionDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.ExternalSourceDirectiveTrivia Or SyntaxKind.EndExternalSourceDirectiveTrivia Or SyntaxKind.ExternalChecksumDirectiveTrivia Or SyntaxKind.EnableWarningDirectiveTrivia Or SyntaxKind.DisableWarningDirectiveTrivia Or SyntaxKind.ReferenceDirectiveTrivia Or SyntaxKind.BadDirectiveTrivia Or SyntaxKind.ImportAliasClause Or SyntaxKind.NameColonEquals Or SyntaxKind.SimpleDoLoopBlock Or SyntaxKind.DoWhileLoopBlock Or SyntaxKind.DoUntilLoopBlock Or SyntaxKind.DoLoopWhileBlock Or SyntaxKind.DoLoopUntilBlock
				Case 768
				Case SyntaxKind.List Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword
				Case SyntaxKind.SimpleDoStatement
				Case SyntaxKind.DoWhileStatement
				Case SyntaxKind.DoUntilStatement
				Case SyntaxKind.SimpleLoopStatement
				Case SyntaxKind.LoopWhileStatement
				Case SyntaxKind.LoopUntilStatement
				Case SyntaxKind.WhileClause
				Case SyntaxKind.UntilClause
				Label0:
					flag = False
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return flag
		End Function

		Public Shared Function IsLabel(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.IdentifierLabel) > CUShort(SyntaxKind.EmptyStatement), False, True)
		End Function

		Public Shared Function IsLabelLabelToken(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NextKeyword OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierToken) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List), True, False)
			Return flag
		End Function

		Public Shared Function IsLabelStatementLabelToken(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.IdentifierToken) > CUShort(SyntaxKind.List), False, True)
		End Function

		Public Shared Function IsLambdaHeader(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.SubLambdaHeader) > CUShort(SyntaxKind.List), False, True)
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use IsLambdaHeaderSubOrFunctionKeyword instead.", True)>
		Public Shared Function IsLambdaHeaderKeyword(ByVal kind As SyntaxKind) As Boolean
			Return SyntaxFacts.IsLambdaHeaderSubOrFunctionKeyword(kind)
		End Function

		Public Shared Function IsLambdaHeaderSubOrFunctionKeyword(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword, True, False)
			Return flag
		End Function

		Public Shared Function IsLanguagePunctuation(ByVal kind As SyntaxKind) As Boolean
			If (Not SyntaxFacts.IsPunctuation(kind)) Then
				Return False
			End If
			Return Not SyntaxFacts.IsPreprocessorPunctuation(kind)
		End Function

		Friend Shared Function IsLeftCurlyBracket(ByVal c As Char) As Boolean
			If (c = "{"C) Then
				Return True
			End If
			Return c = "｛"C
		End Function

		Friend Shared Function IsLetterC(ByVal ch As Char) As Boolean
			Return ch = "c"C Or ch = "C"C Or ch = "Ｃ"C Or ch = "ｃ"C
		End Function

		Public Shared Function IsLiteralExpression(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharacterLiteralExpression) <= 4 OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StringLiteralExpression) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List), True, False)
			Return flag
		End Function

		Public Shared Function IsLiteralExpressionToken(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword) Then
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FalseKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword) Then
					flag = True
					Return flag
				End If
				flag = False
				Return flag
			ElseIf (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TrueKeyword AndAlso CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntegerLiteralToken) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement)) Then
				flag = False
				Return flag
			End If
			flag = True
			Return flag
		End Function

		Public Shared Function IsLoopStatement(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.SimpleLoopStatement) > CUShort(SyntaxKind.EmptyStatement), False, True)
		End Function

		Friend Shared Function IsLowSurrogate(ByVal c As Char) As Boolean
			Return [Char].IsLowSurrogate(c)
		End Function

		Public Shared Function IsMemberAccessExpression(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.SimpleMemberAccessExpression) > CUShort(SyntaxKind.List), False, True)
		End Function

		Public Shared Function IsMemberAccessExpressionOperatorToken(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExclamationToken OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DotToken, True, False)
			Return flag
		End Function

		Public Shared Function IsMethodBlock(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.SubBlock) > CUShort(SyntaxKind.List), False, True)
		End Function

		Public Shared Function IsMethodStatement(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.SubStatement) > CUShort(SyntaxKind.List), False, True)
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use IsMethodStatementSubOrFunctionKeyword instead.", True)>
		Public Shared Function IsMethodStatementKeyword(ByVal kind As SyntaxKind) As Boolean
			Return SyntaxFacts.IsMethodStatementSubOrFunctionKeyword(kind)
		End Function

		Public Shared Function IsMethodStatementSubOrFunctionKeyword(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword, True, False)
			Return flag
		End Function

		Public Shared Function IsMultiLineLambdaExpression(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.MultiLineFunctionLambdaExpression) > CUShort(SyntaxKind.List), False, True)
		End Function

		Public Shared Function IsName(ByVal kind As SyntaxKind) As Boolean
			If (kind = SyntaxKind.IdentifierName OrElse kind = SyntaxKind.GenericName OrElse kind = SyntaxKind.QualifiedName) Then
				Return True
			End If
			Return kind = SyntaxKind.GlobalName
		End Function

		Public Shared Function IsNamedArgumentName(ByVal node As SyntaxNode) As Boolean
			Dim flag As Boolean
			If (node.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierName) Then
				Dim parent As NameColonEqualsSyntax = TryCast(node.Parent, NameColonEqualsSyntax)
				If (parent IsNot Nothing) Then
					Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = parent.Parent.Parent
					If (visualBasicSyntaxNode Is Nothing OrElse Not visualBasicSyntaxNode.IsKind(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArgumentList)) Then
						flag = False
					Else
						Dim parent1 As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = visualBasicSyntaxNode.Parent
						If (parent1 IsNot Nothing) Then
							Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = parent1.Kind()
							flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventStatement OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InvocationExpression) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List), True, False)
						Else
							flag = False
						End If
					End If
				Else
					flag = False
				End If
			Else
				flag = False
			End If
			Return flag
		End Function

		Public Shared Function IsNamespaceMemberDeclaration(ByVal kind As SyntaxKind) As Boolean
			If (kind = SyntaxKind.ClassStatement OrElse kind = SyntaxKind.InterfaceStatement OrElse kind = SyntaxKind.StructureStatement OrElse kind = SyntaxKind.EnumStatement OrElse kind = SyntaxKind.ModuleStatement OrElse kind = SyntaxKind.NamespaceStatement OrElse kind = SyntaxKind.DelegateFunctionStatement) Then
				Return True
			End If
			Return kind = SyntaxKind.DelegateSubStatement
		End Function

		Friend Shared Function IsNarrowIdentifierCharacter(ByVal c As UShort) As Boolean
			Return SyntaxFacts.s_isIDChar(c)
		End Function

		Public Shared Function IsNewLine(ByVal c As Char) As Boolean
			If (13 = c OrElse 10 = c) Then
				Return True
			End If
			If (c < Strings.ChrW(133)) Then
				Return False
			End If
			If (133 = c OrElse 8232 = c) Then
				Return True
			End If
			Return 8233 = c
		End Function

		Friend Shared Function IsOctalDigit(ByVal c As Char) As Boolean
			Return c >= "0"C And c <= "7"C Or c >= "０"C And c <= "７"C
		End Function

		Public Shared Function IsOnErrorGoToStatement(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.OnErrorGoToZeroStatement) > CUShort(SyntaxKind.EmptyStatement), False, True)
		End Function

		Public Shared Function IsOperator(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LikeKeyword) Then
				If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DirectCastKeyword) Then
					Select Case syntaxKind
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndAlsoKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CBoolKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CByteKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CCharKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CDateKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CDecKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CDblKeyword
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
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsValue Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RightShiftAssignmentStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConcatenateAssignmentStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetXmlNamespaceExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleMemberAccessExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlElementEndTag Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlEmptyElement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlAttribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlString Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndAlsoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DictionaryAccessExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlElementEndTag Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlPrefixName Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BooleanKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByRefKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByteKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByValKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CallKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CatchKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayRankSpecifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Attribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributesStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrintStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueForStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineElseClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RightShiftAssignmentStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReDimStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RedimClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetXmlNamespaceExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DictionaryAccessExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlDescendantAccessExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InvocationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnonymousObjectCreationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CollectionInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlElementEndTag Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlAttribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlPrefixName Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlBracketedName Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlComment Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlCDataSection Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayType Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PredefinedType Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByRefKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByValKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CBoolKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsValue Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModifiedIdentifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayRankSpecifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributeList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Attribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributeTarget Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributesStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExpressionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrintStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueDoStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueForStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReturnStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfPart Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineElseClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineIfBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RightShiftAssignmentStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConcatenateAssignmentStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CallStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReDimStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReDimPreserveStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RedimClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EraseStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetXmlNamespaceExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleMemberAccessExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DictionaryAccessExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlElementAccessExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlDescendantAccessExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlAttributeAccessExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InvocationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectCreationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnonymousObjectCreationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayCreationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CollectionInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CTypeExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlElementEndTag Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlEmptyElement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlAttribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlString Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlPrefixName Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlName Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlBracketedName Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlPrefix Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlComment Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlProcessingInstruction Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlCDataSection Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlEmbeddedExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayType Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NullableType Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PredefinedType Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierName Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndAlsoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BooleanKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByRefKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByValKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CallKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CatchKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CBoolKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CByteKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReferenceKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueKeyword
							flag = False
							Return flag
						Case Else
							If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CUShortKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DirectCastKeyword) Then
								Exit Select
							End If
							flag = False
							Return flag
					End Select
				ElseIf (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsKeyword) Then
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsNotKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LikeKeyword) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				Else
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetTypeKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsKeyword) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				End If
			ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeOfKeyword) Then
				If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword) Then
					If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List) OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryCastKeyword) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				Else
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModKeyword OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewKeyword) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				End If
			ElseIf (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsTrueKeyword) Then
				Select Case syntaxKind
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExclamationToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AmpersandToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsteriskToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PlusToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MinusToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DotToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SlashToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanEqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanGreaterThanToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanEqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BackslashToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaretToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AmpersandEqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsteriskEqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PlusEqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MinusEqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SlashEqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BackslashEqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaretEqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanLessThanToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanGreaterThanToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanLessThanEqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanGreaterThanEqualsToken
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AtToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HashToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndAddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRemoveHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSyncLockStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterSingleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterMultipleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubNewStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeywordEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsPropertyEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesClauseItem Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IncompleteMember Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FieldDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariableDeclarator Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleAsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsNewClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectMemberInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectCollectionInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferredFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionalKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrElseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverloadsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParamArrayKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PartialKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrivateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PublicKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReadOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReDimKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.REMKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ResumeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReturnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShadowsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SharedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShortKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StaticKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StepKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StopKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StringKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ThenKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ThrowKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ToKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TrueKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryCastKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeOfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UIntegerKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ULongKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UShortKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhenKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WideningKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WriteOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GosubKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariantKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WendKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AllKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnsiKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AscendingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AssemblyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AutoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BinaryKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompareKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CustomKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DescendingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DisableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DistinctKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExplicitKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalSourceKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalChecksumKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FromKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GroupKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsFalseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsTrueKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OffKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrderKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OutKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PreserveKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RegionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkipKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StrictKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TakeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TextKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UnicodeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UntilKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WarningKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhereKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsyncKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AwaitKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IteratorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.YieldKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExclamationToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AtToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HashToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AmpersandToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseParenToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseBraceToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SemicolonToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ColonToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LabelStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsValue Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModifiedIdentifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LabelStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GoToStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParamArrayKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ColonEqualsToken
						flag = False
						Return flag
					Case Else
						If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NameOfKeyword) Then
							flag = False
							Return flag
						Else
							Exit Select
						End If
				End Select
			Else
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsFalseKeyword) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
					flag = True
					Return flag
				End If
				flag = False
				Return flag
			End If
			flag = True
			Return flag
		End Function

		Public Shared Function IsOperatorStatementOperatorToken(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword) Then
				If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CTypeKeyword) Then
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LikeKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				Else
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CTypeKeyword) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				End If
			ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword) Then
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword) Then
					flag = True
					Return flag
				End If
				flag = False
				Return flag
			ElseIf (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsFalseKeyword) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
				Select Case syntaxKind
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AmpersandToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsteriskToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PlusToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MinusToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SlashToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanEqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanGreaterThanToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanEqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BackslashToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaretToken
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndAddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRemoveHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSyncLockStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterSingleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterMultipleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubNewStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeywordEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsPropertyEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesClauseItem Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IncompleteMember Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FieldDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariableDeclarator Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleAsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsNewClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectMemberInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectCollectionInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferredFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionalKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrElseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverloadsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParamArrayKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PartialKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrivateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PublicKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReadOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReDimKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.REMKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ResumeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReturnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShadowsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SharedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShortKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StaticKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StepKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StopKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StringKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ThenKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ThrowKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ToKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TrueKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryCastKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeOfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UIntegerKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ULongKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UShortKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhenKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WideningKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WriteOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GosubKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariantKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WendKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AllKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnsiKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AscendingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AssemblyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AutoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BinaryKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompareKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CustomKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DescendingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DisableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DistinctKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExplicitKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalSourceKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalChecksumKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FromKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GroupKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsFalseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsTrueKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OffKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrderKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OutKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PreserveKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RegionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkipKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StrictKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TakeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TextKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UnicodeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UntilKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WarningKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhereKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsyncKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AwaitKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IteratorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.YieldKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExclamationToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AtToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HashToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AmpersandToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseParenToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseBraceToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SemicolonToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DotToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ColonToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LabelStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsValue Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModifiedIdentifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LabelStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GoToStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParamArrayKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanToken
						flag = False
						Return flag
					Case Else
						If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanLessThanToken) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
							flag = False
							Return flag
						Else
							Exit Select
						End If
				End Select
			End If
			flag = True
			Return flag
		End Function

		Public Shared Function IsOptionStatementNameKeyword(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExplicitKeyword) Then
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompareKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExplicitKeyword) Then
					flag = True
					Return flag
				End If
				flag = False
				Return flag
			ElseIf (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferKeyword AndAlso syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StrictKeyword) Then
				flag = False
				Return flag
			End If
			flag = True
			Return flag
		End Function

		Public Shared Function IsOptionStatementValueKeyword(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BinaryKeyword) Then
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BinaryKeyword) Then
					flag = True
					Return flag
				End If
				flag = False
				Return flag
			ElseIf (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OffKeyword AndAlso syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TextKeyword) Then
				flag = False
				Return flag
			End If
			flag = True
			Return flag
		End Function

		Public Shared Function IsOrdering(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.AscendingOrdering) > CUShort(SyntaxKind.List), False, True)
		End Function

		Public Shared Function IsOrderingAscendingOrDescendingKeyword(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AscendingKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DescendingKeyword, True, False)
			Return flag
		End Function

		Public Shared Function IsPartitionClause(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.SkipClause) > CUShort(SyntaxKind.List), False, True)
		End Function

		Public Shared Function IsPartitionClauseSkipOrTakeKeyword(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkipKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TakeKeyword, True, False)
			Return flag
		End Function

		Public Shared Function IsPartitionWhileClause(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.SkipWhileClause) > CUShort(SyntaxKind.List), False, True)
		End Function

		Public Shared Function IsPartitionWhileClauseSkipOrTakeKeyword(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkipKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TakeKeyword, True, False)
			Return flag
		End Function

		Public Shared Function IsPredefinedCastExpressionKeyword(ByVal kind As SyntaxKind) As Boolean
			Dim flag As Boolean
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
				Case SyntaxKind.CUIntKeyword
				Case SyntaxKind.CULngKeyword
				Case SyntaxKind.CUShortKeyword
					flag = True
					Exit Select
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.Attribute Or SyntaxKind.AttributesStatement Or SyntaxKind.PrintStatement Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineElseClause Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.MidExpression Or SyntaxKind.AddHandlerStatement Or SyntaxKind.RaiseEventStatement Or SyntaxKind.ReDimStatement Or SyntaxKind.RedimClause Or SyntaxKind.GetXmlNamespaceExpression Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.XmlDescendantAccessExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.AnonymousObjectCreationExpression Or SyntaxKind.CollectionInitializer Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlPrefixName Or SyntaxKind.XmlBracketedName Or SyntaxKind.XmlComment Or SyntaxKind.XmlCDataSection Or SyntaxKind.ArrayType Or SyntaxKind.PredefinedType Or SyntaxKind.AndKeyword Or SyntaxKind.AsKeyword Or SyntaxKind.ByRefKeyword Or SyntaxKind.ByValKeyword Or SyntaxKind.CaseKeyword Or SyntaxKind.CBoolKeyword
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueDoStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineIfPart Or SyntaxKind.SingleLineElseClause Or SyntaxKind.MultiLineIfBlock Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.ConcatenateAssignmentStatement Or SyntaxKind.MidExpression Or SyntaxKind.CallStatement Or SyntaxKind.AddHandlerStatement Or SyntaxKind.RemoveHandlerStatement Or SyntaxKind.RaiseEventStatement Or SyntaxKind.WithStatement Or SyntaxKind.ReDimStatement Or SyntaxKind.ReDimPreserveStatement Or SyntaxKind.RedimClause Or SyntaxKind.EraseStatement Or SyntaxKind.GetXmlNamespaceExpression Or SyntaxKind.SimpleMemberAccessExpression Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.XmlElementAccessExpression Or SyntaxKind.XmlDescendantAccessExpression Or SyntaxKind.XmlAttributeAccessExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.ObjectCreationExpression Or SyntaxKind.AnonymousObjectCreationExpression Or SyntaxKind.ArrayCreationExpression Or SyntaxKind.CollectionInitializer Or SyntaxKind.CTypeExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlEmptyElement Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlString Or SyntaxKind.XmlPrefixName Or SyntaxKind.XmlName Or SyntaxKind.XmlBracketedName Or SyntaxKind.XmlPrefix Or SyntaxKind.XmlComment Or SyntaxKind.XmlProcessingInstruction Or SyntaxKind.XmlCDataSection Or SyntaxKind.XmlEmbeddedExpression Or SyntaxKind.ArrayType Or SyntaxKind.NullableType Or SyntaxKind.PredefinedType Or SyntaxKind.IdentifierName Or SyntaxKind.AndKeyword Or SyntaxKind.AndAlsoKeyword Or SyntaxKind.AsKeyword Or SyntaxKind.BooleanKeyword Or SyntaxKind.ByRefKeyword Or SyntaxKind.ByteKeyword Or SyntaxKind.ByValKeyword Or SyntaxKind.CallKeyword Or SyntaxKind.CaseKeyword Or SyntaxKind.CatchKeyword Or SyntaxKind.CBoolKeyword Or SyntaxKind.CByteKeyword
				Case SyntaxKind.CharKeyword
				Case SyntaxKind.ClassKeyword
				Case SyntaxKind.ConstKeyword
				Case SyntaxKind.ReferenceKeyword
				Case SyntaxKind.ContinueKeyword
				Case SyntaxKind.CTypeKeyword
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.ConcatenateAssignmentStatement Or SyntaxKind.NotEqualsExpression Or SyntaxKind.LessThanExpression Or SyntaxKind.LessThanOrEqualExpression Or SyntaxKind.GreaterThanOrEqualExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlEmptyElement Or SyntaxKind.XmlAttribute Or SyntaxKind.XmlString Or SyntaxKind.CTypeKeyword Or SyntaxKind.CUIntKeyword Or SyntaxKind.CULngKeyword
				Case SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.MidExpression Or SyntaxKind.NotEqualsExpression Or SyntaxKind.GreaterThanExpression Or SyntaxKind.XmlElementEndTag Or SyntaxKind.XmlPrefixName Or SyntaxKind.CTypeKeyword
				Label0:
					flag = False
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return flag
		End Function

		Public Shared Function IsPredefinedType(ByVal kind As SyntaxKind) As Boolean
			Return SyntaxFacts.IsPredefinedTypeKeyword(kind)
		End Function

		Friend Shared Function IsPredefinedTypeKeyword(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntegerKeyword) Then
				If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharKeyword) Then
					If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DateKeyword) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List) OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoubleKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntegerKeyword) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				Else
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BooleanKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByteKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharKeyword) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				End If
			ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword) Then
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LongKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword) Then
					flag = True
					Return flag
				End If
				flag = False
				Return flag
			ElseIf (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShortKeyword) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List) AndAlso syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StringKeyword AndAlso CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UIntegerKeyword) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement)) Then
				flag = False
				Return flag
			End If
			flag = True
			Return flag
		End Function

		Friend Shared Function IsPredefinedTypeOrVariant(ByVal kind As SyntaxKind) As Boolean
			If (SyntaxFacts.IsPredefinedTypeKeyword(kind)) Then
				Return True
			End If
			Return kind = SyntaxKind.VariantKeyword
		End Function

		Public Shared Function IsPreprocessorDirective(ByVal kind As SyntaxKind) As Boolean
			Dim flag As Boolean
			Select Case kind
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
					flag = True
					Exit Select
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.EventStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.SyncLockStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OpenParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.EventStatement Or SyntaxKind.OperatorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueDoStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.OnErrorResumeNextStatement Or SyntaxKind.SyncLockStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.StaticKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OffKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OutKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.AsteriskToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.QuestionToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.StatementTerminatorToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.EmptyToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlCrefAttribute Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.IfDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.ElseDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia Or SyntaxKind.RegionDirectiveTrivia
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.SubBlock Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.EventStatement Or SyntaxKind.OperatorStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.AddHandlerAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.RaiseEventAccessorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueDoStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineIfPart Or SyntaxKind.SingleLineElseClause Or SyntaxKind.MultiLineIfBlock Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.OnErrorResumeNextStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectBlock Or SyntaxKind.SelectStatement Or SyntaxKind.CaseBlock Or SyntaxKind.SyncLockStatement Or SyntaxKind.WhileStatement Or SyntaxKind.ForBlock Or SyntaxKind.ForEachBlock Or SyntaxKind.ForStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OrElseKeyword Or SyntaxKind.OverloadsKeyword Or SyntaxKind.OverridableKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.StaticKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.StructureKeyword Or SyntaxKind.SubKeyword Or SyntaxKind.SyncLockKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AscendingKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.AutoKeyword Or SyntaxKind.BinaryKeyword Or SyntaxKind.ByKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OffKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OutKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.RegionKeyword Or SyntaxKind.SkipKeyword Or SyntaxKind.StrictKeyword Or SyntaxKind.TakeKeyword Or SyntaxKind.TextKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.AsteriskToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.DotToken Or SyntaxKind.SlashToken Or SyntaxKind.ColonToken Or SyntaxKind.LessThanToken Or SyntaxKind.LessThanEqualsToken Or SyntaxKind.LessThanGreaterThanToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.QuestionToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.StatementTerminatorToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.EmptyToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanSlashToken Or SyntaxKind.LessThanExclamationMinusMinusToken Or SyntaxKind.MinusMinusGreaterThanToken Or SyntaxKind.LessThanQuestionToken Or SyntaxKind.QuestionGreaterThanToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlCrefAttribute Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.IfDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.ElseDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia Or SyntaxKind.RegionDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.ExternalSourceDirectiveTrivia Or SyntaxKind.EndExternalSourceDirectiveTrivia Or SyntaxKind.ExternalChecksumDirectiveTrivia Or SyntaxKind.EnableWarningDirectiveTrivia Or SyntaxKind.DisableWarningDirectiveTrivia Or SyntaxKind.ReferenceDirectiveTrivia
				Case SyntaxKind.EndFunctionStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.FunctionBlock Or SyntaxKind.DeclareSubStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.UsingBlock Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ForStepClause Or SyntaxKind.NotKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.ConstDirectiveTrivia
				Label0:
					flag = False
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return flag
		End Function

		Public Shared Function IsPreprocessorKeyword(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfKeyword) Then
				If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndKeyword) Then
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IfKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfKeyword) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				Else
					If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstKeyword) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List) OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseKeyword) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement)) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				End If
			ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnableKeyword) Then
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DisableKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnableKeyword) Then
					flag = True
					Return flag
				End If
				flag = False
				Return flag
			ElseIf (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalSourceKeyword) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List) AndAlso syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RegionKeyword) Then
				flag = False
				Return flag
			End If
			flag = True
			Return flag
		End Function

		Public Shared Function IsPreprocessorPunctuation(ByVal kind As SyntaxKind) As Boolean
			Return kind = SyntaxKind.HashToken
		End Function

		Friend Shared Function IsPropAlpha(ByVal CharacterProperties As UnicodeCategory) As Boolean
			Return CharacterProperties <= UnicodeCategory.OtherLetter
		End Function

		Friend Shared Function IsPropAlphaNumeric(ByVal CharacterProperties As UnicodeCategory) As Boolean
			Return CharacterProperties <= UnicodeCategory.DecimalDigitNumber
		End Function

		Friend Shared Function IsPropCombining(ByVal CharacterProperties As UnicodeCategory) As Boolean
			If (CharacterProperties < UnicodeCategory.NonSpacingMark) Then
				Return False
			End If
			Return CharacterProperties <= UnicodeCategory.EnclosingMark
		End Function

		Friend Shared Function IsPropConnectorPunctuation(ByVal CharacterProperties As UnicodeCategory) As Boolean
			Return CharacterProperties = UnicodeCategory.ConnectorPunctuation
		End Function

		Friend Shared Function IsPropLetterDigit(ByVal CharacterProperties As UnicodeCategory) As Boolean
			Return CharacterProperties = UnicodeCategory.LetterNumber
		End Function

		Friend Shared Function IsPropOtherFormat(ByVal CharacterProperties As UnicodeCategory) As Boolean
			Return CharacterProperties = UnicodeCategory.Format
		End Function

		Public Shared Function IsPunctuation(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			Select Case syntaxKind
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExclamationToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AtToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HashToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AmpersandToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseParenToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseBraceToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SemicolonToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsteriskToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PlusToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MinusToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DotToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SlashToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ColonToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanEqualsToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanGreaterThanToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanEqualsToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BackslashToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaretToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ColonEqualsToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AmpersandEqualsToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsteriskEqualsToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PlusEqualsToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MinusEqualsToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SlashEqualsToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BackslashEqualsToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaretEqualsToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanLessThanToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanGreaterThanToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanLessThanEqualsToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanGreaterThanEqualsToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QuestionToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoubleQuoteToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StatementTerminatorToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOfFileToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SlashGreaterThanToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanSlashToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanExclamationMinusMinusToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MinusMinusGreaterThanToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanQuestionToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QuestionGreaterThanToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanPercentEqualsToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PercentGreaterThanToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BeginCDataToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndCDataToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOfXmlToken
				Label0:
					flag = True
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndAddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRemoveHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSyncLockStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterSingleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterMultipleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubNewStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeywordEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsPropertyEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesClauseItem Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IncompleteMember Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FieldDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariableDeclarator Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleAsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsNewClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectMemberInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectCollectionInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferredFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionalKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrElseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverloadsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParamArrayKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PartialKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrivateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PublicKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReadOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReDimKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.REMKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ResumeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReturnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShadowsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SharedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShortKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StaticKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StepKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StopKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StringKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ThenKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ThrowKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ToKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TrueKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryCastKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeOfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UIntegerKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ULongKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UShortKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhenKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WideningKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WriteOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GosubKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariantKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WendKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AllKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnsiKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AscendingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AssemblyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AutoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BinaryKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompareKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CustomKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DescendingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DisableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DistinctKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExplicitKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalSourceKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalChecksumKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FromKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GroupKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsFalseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsTrueKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OffKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrderKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OutKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PreserveKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RegionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkipKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StrictKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TakeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TextKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UnicodeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UntilKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WarningKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhereKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsyncKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AwaitKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IteratorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.YieldKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExclamationToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AtToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HashToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AmpersandToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LabelStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsValue Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModifiedIdentifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LabelStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GoToStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParamArrayKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsValue Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Attribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributeTarget Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueForStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReturnStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StepKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StopKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PlusToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MinusToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanGreaterThanToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanLessThanEqualsToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SlashGreaterThanToken
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Attribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributesStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueForStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StepKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StringKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PlusToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DotToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanGreaterThanToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanGreaterThanEqualsToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SlashGreaterThanToken
				Label1:
					flag = False
					Exit Select
				Case Else
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DollarSignDoubleQuoteToken OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOfInterpolatedStringToken) Then
						GoTo Label0
					Else
						GoTo Label1
					End If
			End Select
			Return flag
		End Function

		Public Shared Function IsPunctuationOrKeyword(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If((syntaxKind < Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerKeyword OrElse syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOfXmlToken) AndAlso syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NameOfKeyword AndAlso syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DollarSignDoubleQuoteToken AndAlso syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOfInterpolatedStringToken, False, True)
			Return flag
		End Function

		Public Shared Function IsReDimStatement(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.ReDimStatement) > CUShort(SyntaxKind.List), False, True)
		End Function

		Public Shared Function IsRelationalCaseClause(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseEqualsClause) <= CUShort((Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement)) OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseGreaterThanOrEqualClause) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List), True, False)
			Return flag
		End Function

		Public Shared Function IsRelationalCaseClauseOperatorToken(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.LessThanToken) > CUShort(SyntaxKind.EndIfStatement), False, True)
		End Function

		Public Shared Function IsRelationalOperator(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.LessThanToken) > CUShort(SyntaxKind.EndIfStatement), False, True)
		End Function

		Public Shared Function IsReservedKeyword(ByVal kind As SyntaxKind) As Boolean
			If (CUShort((CUShort(kind) - CUShort(SyntaxKind.AddHandlerKeyword))) <= 170) Then
				Return True
			End If
			Return kind = SyntaxKind.NameOfKeyword
		End Function

		Public Shared Function IsReservedTupleElementName(ByVal elementName As String) As Boolean
			Return TupleTypeSymbol.IsElementNameReserved(elementName) <> -1
		End Function

		Public Shared Function IsResumeStatement(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.ResumeStatement) > CUShort(SyntaxKind.EmptyStatement), False, True)
		End Function

		Friend Shared Function IsRightCurlyBracket(ByVal c As Char) As Boolean
			If (c = "}"C) Then
				Return True
			End If
			Return c = "｝"C
		End Function

		Public Shared Function IsSingleLineLambdaExpression(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineFunctionLambdaExpression OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineSubLambdaExpression, True, False)
			Return flag
		End Function

		Friend Shared Function IsSingleQuote(ByVal c As Char) As Boolean
			If (c = "'"C) Then
				Return True
			End If
			If (c < "‘"C) Then
				Return False
			End If
			Return c = "＇"C Or c = "‘"C Or c = "’"C
		End Function

		Friend Shared Function IsSpaceSeparator(ByVal c As Char) As Boolean
			Return CharUnicodeInfo.GetUnicodeCategory(c) = UnicodeCategory.SpaceSeparator
		End Function

		Public Shared Function IsSpecialConstraint(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.NewConstraint) > CUShort(SyntaxKind.EmptyStatement), False, True)
		End Function

		Public Shared Function IsSpecialConstraintConstraintKeyword(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword, True, False)
			Return flag
		End Function

		Friend Shared Function IsSpecifier(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReadOnlyKeyword) Then
				If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FriendKeyword) Then
					If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DefaultKeyword) Then
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DimKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FriendKeyword) Then
							flag = True
							Return flag
						End If
						flag = False
						Return flag
					Else
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DefaultKeyword) Then
							flag = True
							Return flag
						End If
						flag = False
						Return flag
					End If
				ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NarrowingKeyword) Then
					If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MustInheritKeyword) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List) OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NarrowingKeyword) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				ElseIf (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
					Select Case syntaxKind
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverloadsKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridableKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PartialKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrivateKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PublicKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReadOnlyKeyword
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParamArrayKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndAddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRemoveHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParamArrayKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PartialKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrivateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PublicKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventKeyword
							flag = False
							Return flag
						Case Else
							flag = False
							Return flag
					End Select
				End If
			ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsKeyword) Then
				If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StaticKeyword) Then
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WideningKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsKeyword) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				Else
					If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShadowsKeyword) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List) OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StaticKeyword) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				End If
			ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CustomKeyword) Then
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WriteOnlyKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CustomKeyword) Then
					flag = True
					Return flag
				End If
				flag = False
				Return flag
			ElseIf (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsyncKeyword AndAlso syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IteratorKeyword) Then
				flag = False
				Return flag
			End If
			flag = True
			Return flag
		End Function

		Public Shared Function IsStopOrEndStatement(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StopStatement OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStatement, True, False)
			Return flag
		End Function

		Public Shared Function IsStopOrEndStatementStopOrEndKeyword(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StopKeyword, True, False)
			Return flag
		End Function

		Friend Shared Function IsSurrogate(ByVal c As Char) As Boolean
			Return [Char].IsSurrogate(c)
		End Function

		Friend Shared Function IsSyntaxTrivia(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhitespaceTrivia) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement) OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConflictMarkerTrivia, True, False)
			Return flag
		End Function

		Friend Shared Function IsTerminator(ByVal kind As SyntaxKind) As Boolean
			If (kind = SyntaxKind.StatementTerminatorToken OrElse kind = SyntaxKind.ColonToken) Then
				Return True
			End If
			Return kind = SyntaxKind.EndOfFileToken
		End Function

		Public Shared Function IsTrivia(ByVal this As SyntaxKind) As Boolean
			Return SyntaxFacts.IsSyntaxTrivia(this)
		End Function

		Public Shared Function IsTypeOfExpression(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.TypeOfIsExpression) > CUShort(SyntaxKind.List), False, True)
		End Function

		Public Shared Function IsTypeOfExpressionOperatorToken(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsNotKeyword, True, False)
			Return flag
		End Function

		Public Shared Function IsTypeParameterVarianceKeyword(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OutKeyword, True, False)
			Return flag
		End Function

		Public Shared Function IsUnaryExpression(ByVal kind As SyntaxKind) As Boolean
			Dim flag As Boolean
			flag = If(CUShort(kind) - CUShort(SyntaxKind.UnaryPlusExpression) > CUShort((SyntaxKind.List Or SyntaxKind.EmptyStatement)), False, True)
			Return flag
		End Function

		Public Shared Function IsUnaryExpressionOperatorToken(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddressOfKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PlusToken) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List), True, False)
			Return flag
		End Function

		Public Shared Function IsUnderscore(ByVal c As Char) As Boolean
			Return c = Strings.ChrW(95)
		End Function

		Public Shared Function IsValidIdentifier(ByVal name As String) As Boolean
			Dim flag As Boolean
			If ([String].IsNullOrEmpty(name)) Then
				flag = False
			ElseIf (SyntaxFacts.IsIdentifierStartCharacter(name(0))) Then
				Dim length As Integer = name.Length - 1
				Dim num As Integer = 1
				While num <= length
					If (SyntaxFacts.IsIdentifierPartCharacter(name(num))) Then
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

		Public Shared Function IsWhileOrUntilClause(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.WhileClause) > CUShort(SyntaxKind.List), False, True)
		End Function

		Public Shared Function IsWhileOrUntilClauseWhileOrUntilKeyword(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UntilKeyword, True, False)
			Return flag
		End Function

		Public Shared Function IsWhitespace(ByVal c As Char) As Boolean
			If (32 = c OrElse 9 = c) Then
				Return True
			End If
			If (c <= Strings.ChrW(128)) Then
				Return False
			End If
			Return SyntaxFacts.IsWhitespaceNotAscii(c)
		End Function

		Friend Shared Function IsWhitespaceNotAscii(ByVal ch As Char) As Boolean
			Dim flag As Boolean
			Dim chr As Char = ch
			flag = If(chr = Strings.ChrW(160) OrElse chr = Strings.ChrW(12288) OrElse chr >= Strings.ChrW(8192) AndAlso chr <= Strings.ChrW(8203), True, CharUnicodeInfo.GetUnicodeCategory(ch) = UnicodeCategory.SpaceSeparator)
			Return flag
		End Function

		Friend Shared Function IsWideIdentifierCharacter(ByVal c As Char) As Boolean
			Dim unicodeCategory As System.Globalization.UnicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c)
			If (SyntaxFacts.IsPropAlphaNumeric(unicodeCategory) OrElse SyntaxFacts.IsPropLetterDigit(unicodeCategory) OrElse SyntaxFacts.IsPropConnectorPunctuation(unicodeCategory) OrElse SyntaxFacts.IsPropCombining(unicodeCategory)) Then
				Return True
			End If
			Return SyntaxFacts.IsPropOtherFormat(unicodeCategory)
		End Function

		Friend Shared Function IsWithinPreprocessorConditionalExpression(ByVal node As SyntaxNode) As Boolean
			Dim value As Boolean
			Dim parent As SyntaxNode = node.Parent
			While True
				If (parent IsNot Nothing) Then
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = parent.Kind()
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstDirectiveTrivia) Then
						value = DirectCast(parent, ConstDirectiveTriviaSyntax).Value = node
						Exit While
					ElseIf (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IfDirectiveTrivia) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
						node = parent
						parent = node.Parent
					Else
						value = DirectCast(parent, IfDirectiveTriviaSyntax).Condition = node
						Exit While
					End If
				Else
					value = False
					Exit While
				End If
			End While
			Return value
		End Function

		Public Shared Function IsXmlCrefAttributeEndQuoteToken(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoubleQuoteToken, True, False)
			Return flag
		End Function

		Public Shared Function IsXmlCrefAttributeStartQuoteToken(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoubleQuoteToken, True, False)
			Return flag
		End Function

		Public Shared Function IsXmlMemberAccessExpression(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.XmlElementAccessExpression) > CUShort(SyntaxKind.EmptyStatement), False, True)
		End Function

		Public Shared Function IsXmlMemberAccessExpressionToken2(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AtToken OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DotToken, True, False)
			Return flag
		End Function

		Public Shared Function IsXmlNameAttributeEndQuoteToken(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoubleQuoteToken, True, False)
			Return flag
		End Function

		Public Shared Function IsXmlNameAttributeStartQuoteToken(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoubleQuoteToken, True, False)
			Return flag
		End Function

		Public Shared Function IsXmlStringEndQuoteToken(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoubleQuoteToken, True, False)
			Return flag
		End Function

		Public Shared Function IsXmlStringStartQuoteToken(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoubleQuoteToken, True, False)
			Return flag
		End Function

		Friend Shared Function IsXmlSyntax(ByVal kind As SyntaxKind) As Boolean
			Dim flag As Boolean
			Select Case kind
				Case SyntaxKind.XmlDocument
				Case SyntaxKind.XmlDeclaration
				Case SyntaxKind.XmlDeclarationOption
				Case SyntaxKind.XmlElement
				Case SyntaxKind.XmlText
				Case SyntaxKind.XmlElementStartTag
				Case SyntaxKind.XmlElementEndTag
				Case SyntaxKind.XmlEmptyElement
				Case SyntaxKind.XmlAttribute
				Case SyntaxKind.XmlString
				Case SyntaxKind.XmlName
				Case SyntaxKind.XmlBracketedName
				Case SyntaxKind.XmlPrefix
				Case SyntaxKind.XmlComment
				Case SyntaxKind.XmlCDataSection
				Case SyntaxKind.XmlEmbeddedExpression
					flag = True
					Exit Select
				Case SyntaxKind.XmlPrefixName
				Case SyntaxKind.XmlProcessingInstruction
				Label0:
					flag = False
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return flag
		End Function

		Public Shared Function IsXmlTextToken(ByVal kind As SyntaxKind) As Boolean
			Return If(CUShort(kind) - CUShort(SyntaxKind.XmlTextLiteralToken) > CUShort(SyntaxKind.EmptyStatement), False, True)
		End Function

		Public Shared Function IsXmlWhitespace(ByVal c As Char) As Boolean
			If (32 = c OrElse 9 = c) Then
				Return True
			End If
			If (c <= Strings.ChrW(128)) Then
				Return False
			End If
			Return XmlCharType.IsWhiteSpace(c)
		End Function

		Friend Shared Function MakeFullWidth(ByVal c As Char) As Char
			Return Convert.ToChar(Convert.ToUInt16(c) + 65248)
		End Function

		Friend Shared Function MakeHalfWidth(ByVal c As Char) As Char
			Return Convert.ToChar(Convert.ToUInt16(c) - 65248)
		End Function

		Public Shared Function MakeHalfWidthIdentifier(ByVal text As String) As String
			Dim str As String
			If (text IsNot Nothing) Then
				Dim chrArray As Char() = Nothing
				Dim length As Integer = text.Length - 1
				Dim num As Integer = 0
				Do
					Dim chr As Char = text(num)
					If (SyntaxFacts.IsFullWidth(chr)) Then
						If (chrArray Is Nothing) Then
							ReDim chrArray(text.Length - 1 + 1 - 1)
							text.CopyTo(0, chrArray, 0, num)
						End If
						chrArray(num) = SyntaxFacts.MakeHalfWidth(chr)
					ElseIf (chrArray IsNot Nothing) Then
						chrArray(num) = chr
					End If
					num = num + 1
				Loop While num <= length
				str = If(chrArray Is Nothing, text, New [String](chrArray))
			Else
				str = text
			End If
			Return str
		End Function

		Friend Shared Function MatchOneOrAnother(ByVal ch As Char, ByVal one As Char, ByVal another As Char) As Boolean
			Return ch = one Or ch = another
		End Function

		Friend Shared Function MatchOneOrAnotherOrFullwidth(ByVal ch As Char, ByVal one As Char, ByVal another As Char) As Boolean
			If (SyntaxFacts.IsFullWidth(ch)) Then
				ch = SyntaxFacts.MakeHalfWidth(ch)
			End If
			Return ch = one Or ch = another
		End Function

		Friend Shared Function ReturnFullWidthOrSelf(ByVal c As Char) As Char
			Dim chr As Char
			chr = If(Not SyntaxFacts.IsHalfWidth(c), c, SyntaxFacts.MakeFullWidth(c))
			Return chr
		End Function

		Friend Shared Function SupportsContinueStatement(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileBlock OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForBlock) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List) OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleDoLoopBlock) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement), True, False)
			Return flag
		End Function

		Friend Shared Function SupportsExitStatement(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = kind
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryBlock) Then
				If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock) Then
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileBlock OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryBlock) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				Else
					If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List) OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				End If
			ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForEachBlock) Then
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectBlock OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForBlock) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
					flag = True
					Return flag
				End If
				flag = False
				Return flag
			ElseIf (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineSubLambdaExpression) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement) AndAlso CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleDoLoopBlock) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement)) Then
				flag = False
				Return flag
			End If
			flag = True
			Return flag
		End Function

		Public Shared Function VarianceKindFromToken(ByVal token As SyntaxToken) As Microsoft.CodeAnalysis.VarianceKind
			Dim varianceKind As Microsoft.CodeAnalysis.VarianceKind
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = token.Kind()
			If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InKeyword) Then
				varianceKind = Microsoft.CodeAnalysis.VarianceKind.[In]
			Else
				varianceKind = If(syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OutKeyword, Microsoft.CodeAnalysis.VarianceKind.None, Microsoft.CodeAnalysis.VarianceKind.Out)
			End If
			Return varianceKind
		End Function

		Private NotInheritable Class SyntaxKindEqualityComparer
			Implements IEqualityComparer(Of SyntaxKind)
			Public Sub New()
				MyBase.New()
			End Sub

			Public Function Equals(ByVal x As SyntaxKind, ByVal y As SyntaxKind) As Boolean Implements IEqualityComparer(Of SyntaxKind).Equals
				Return x = y
			End Function

			Public Function GetHashCode(ByVal obj As SyntaxKind) As Integer Implements IEqualityComparer(Of SyntaxKind).GetHashCode
				Return CInt(obj)
			End Function
		End Class
	End Class
End Namespace