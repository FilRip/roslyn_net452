Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Generic

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Friend Class KeywordTable
		Private ReadOnly Shared s_keywords As Dictionary(Of String, SyntaxKind)

		Private ReadOnly Shared s_keywordProperties As Dictionary(Of UShort, KeywordTable.KeywordDescription)

		Shared Sub New()
			KeywordTable.s_keywords = New Dictionary(Of String, SyntaxKind)(CaseInsensitiveComparison.Comparer)
			KeywordTable.s_keywordProperties = New Dictionary(Of UShort, KeywordTable.KeywordDescription)()
			Dim numArray() As UInt16 = { 413, 0, 414, 0, 415, 0, 416, 1027, 417, 1027, 418, 0, 421, 0, 422, 0, 423, 0, 424, 0, 425, 0, 426, 0, 427, 0, 428, 0, 429, 0, 432, 0, 433, 0, 434, 0, 435, 0, 436, 0, 437, 0, 438, 0, 439, 0, 440, 0, 441, 0, 443, 256, 444, 256, 445, 0, 446, 0, 447, 0, 448, 0, 449, 256, 450, 256, 453, 256, 454, 0, 455, 0, 456, 0, 457, 0, 458, 0, 459, 0, 460, 0, 461, 0, 462, 0, 463, 0, 464, 1024, 465, 0, 466, 0, 467, 0, 468, 0, 469, 0, 470, 0, 471, 0, 474, 0, 475, 0, 476, 0, 477, 0, 478, 0, 479, 0, 480, 0, 481, 0, 482, 256, 483, 0, 484, 0, 485, 0, 486, 1024, 487, 0, 488, 1024, 489, 0, 490, 0, 491, 0, 492, 1029, 495, 1285, 496, 1536, 497, 0, 498, 1029, 499, 0, 500, 0, 501, 0, 502, 1033, 503, 0, 504, 0, 505, 0, 506, 0, 507, 0, 778, 0, 508, 0, 509, 256, 510, 0, 511, 0, 512, 4, 513, 0, 516, 0, 517, 0, 518, 0, 519, 256, 520, 1024, 521, 256, 522, 0, 523, 0, 524, 1026, 525, 1026, 526, 0, 527, 0, 528, 0, 529, 0, 530, 256, 531, 0, 532, 0, 533, 0, 534, 0, 537, 0, 538, 0, 442, 0, 539, 0, 540, 1024, 541, 0, 542, 0, 543, 0, 544, 256, 545, 1536, 546, 0, 547, 0, 548, 0, 549, 0, 550, 0, 551, 0, 552, 1024, 553, 0, 554, 0, 555, 0, 558, 0, 559, 0, 560, 1024, 561, 0, 562, 1024, 563, 0, 564, 0, 565, 256, 566, 0, 567, 256, 568, 256, 569, 256, 570, 256, 571, 0, 572, 0, 573, 256, 574, 0, 575, 0, 578, 0, 579, 1025, 584, 1536, 585, 0, 586, 0, 587, 1024, 588, 0, 589, 0, 590, 0, 591, 1024, 592, 0, 593, 0, 594, 1024, 595, 0, 596, 1536, 599, 0, 600, 1024, 601, 0, 602, 0, 603, 0, 604, 1536, 605, 1536, 606, 0, 607, 1024, 608, 0, 609, 0, 610, 1536, 611, 0, 612, 0, 613, 0, 614, 1536, 615, 0, 616, 0, 617, 0, 620, 1536, 621, 0, 623, 0, 622, 1536, 624, 0, 625, 0, 626, 0, 627, 1536, 630, 0, 631, 14, 632, 0, 633, 0, 580, 0, 581, 0, 628, 0, 582, 0, 583, 0, 636, 1024, 638, 1031, 641, 0, 642, 1024, 643, 1024, 644, 0, 645, 1024, 647, 1035, 648, 1032, 649, 1032, 651, 1035, 653, 1029, 654, 1029, 655, 1029, 656, 1029, 657, 1029, 658, 5, 659, 1034, 662, 1037, 663, 0, 664, 7, 665, 11, 666, 8, 667, 8, 668, 11, 669, 10, 670, 13, 671, 6, 672, 6, 673, 6, 674, 6, 689, 1024 }
			Dim length As Integer = CInt(numArray.Length) - 1
			For i As Integer = 0 To length Step 2
				Dim num As UShort = numArray(i + 1)
				KeywordTable.AddKeyword(DirectCast(numArray(i), SyntaxKind), ' 
				' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.Syntax.KeywordTable::.cctor()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Void .cctor()
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Expression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 1019
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 946
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 141
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(IList`1 , Boolean ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2787
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(IList`1 ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2798
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2687
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 2150
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 126
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
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(Statement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1060
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(MethodDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1916
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(MethodDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1841
				'    à ..WriteInternal(IMemberDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseLanguageWriter.cs:ligne 447
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com


		Public Sub New()
			MyBase.New()
		End Sub

		Private Shared Sub AddKeyword(ByVal Token As SyntaxKind, ByVal New7To8 As Boolean, ByVal Precedence As OperatorPrecedence, ByVal isQueryClause As Boolean, ByVal canFollowExpr As Boolean)
			Dim keywordDescription As KeywordTable.KeywordDescription = New KeywordTable.KeywordDescription(New7To8, Precedence, isQueryClause, canFollowExpr)
			KeywordTable.s_keywordProperties.Add(Token, keywordDescription)
			Dim text As String = SyntaxFacts.GetText(Token)
			KeywordTable.s_keywords.Add(text, Token)
		End Sub

		Friend Shared Function CanFollowExpression(ByVal kind As SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim keywordDescription As KeywordTable.KeywordDescription = New KeywordTable.KeywordDescription()
			flag = If(Not KeywordTable.s_keywordProperties.TryGetValue(kind, keywordDescription), False, keywordDescription.kdCanFollowExpr)
			Return flag
		End Function

		Private Shared Function EnsureHalfWidth(ByVal s As String) As String
			Dim chrArray As Char() = Nothing
			Dim length As Integer = s.Length - 1
			Dim num As Integer = 0
			Do
				Dim chr As Char = s(num)
				If (SyntaxFacts.IsFullWidth(chr)) Then
					chr = SyntaxFacts.MakeHalfWidth(chr)
					If (chrArray Is Nothing) Then
						ReDim chrArray(s.Length - 1 + 1 - 1)
						Dim num1 As Integer = num - 1
						For i As Integer = 0 To num1
							chrArray(i) = s(i)
						Next

					End If
					chrArray(num) = chr
				ElseIf (chrArray IsNot Nothing) Then
					chrArray(num) = chr
				End If
				num = num + 1
			Loop While num <= length
			Return If(chrArray Is Nothing, s, New [String](chrArray))
		End Function

		Friend Shared Function IsQueryClause(ByVal kind As SyntaxKind) As Boolean
			Dim flag As Boolean
			Dim keywordDescription As KeywordTable.KeywordDescription = New KeywordTable.KeywordDescription()
			flag = If(Not KeywordTable.s_keywordProperties.TryGetValue(kind, keywordDescription), False, keywordDescription.kdIsQueryClause)
			Return flag
		End Function

		Friend Shared Function TokenOfString(ByVal tokenName As String) As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			tokenName = KeywordTable.EnsureHalfWidth(tokenName)
			If (Not KeywordTable.s_keywords.TryGetValue(tokenName, syntaxKind)) Then
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierToken
			End If
			Return syntaxKind
		End Function

		Friend Shared Function TokenOpPrec(ByVal kind As SyntaxKind) As Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorPrecedence
			Dim operatorPrecedence As Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorPrecedence
			Dim keywordDescription As KeywordTable.KeywordDescription = New KeywordTable.KeywordDescription()
			operatorPrecedence = If(Not KeywordTable.s_keywordProperties.TryGetValue(kind, keywordDescription), Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorPrecedence.PrecedenceNone, keywordDescription.kdOperPrec)
			Return operatorPrecedence
		End Function

		Public Structure KeywordDescription
			Public kdOperPrec As OperatorPrecedence

			Public kdNew7To8kwd As Boolean

			Public kdIsQueryClause As Boolean

			Public kdCanFollowExpr As Boolean

			Public Sub New(ByVal New7To8 As Boolean, ByVal Precedence As OperatorPrecedence, ByVal isQueryClause As Boolean, ByVal canFollowExpr As Boolean)
				Me = New KeywordTable.KeywordDescription() With
				{
					.kdNew7To8kwd = New7To8,
					.kdOperPrec = Precedence,
					.kdIsQueryClause = isQueryClause,
					.kdCanFollowExpr = canFollowExpr
				}
			End Sub
		End Structure
	End Class
End Namespace