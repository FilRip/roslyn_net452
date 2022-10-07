Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend Module SyntaxNodeExtensions
		<Extension>
		Friend Function AddError(Of TSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal node As TSyntax, ByVal errorId As ERRID) As TSyntax
			Return DirectCast(node.AddError(ErrorFactory.ErrorInfo(errorId)), TSyntax)
		End Function

		<Extension>
		Friend Function AddLeadingSyntax(Of TSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal node As TSyntax, ByVal unexpected As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode), ByVal errorId As ERRID) As TSyntax
			Dim tSyntax1 As TSyntax
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(errorId)
			tSyntax1 = If(unexpected.Node Is Nothing, DirectCast(node.AddError(diagnosticInfo), TSyntax), node.AddLeadingTrivia(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxNodeExtensions.CreateSkippedTrivia(unexpected.Node, True, True, diagnosticInfo)))
			Return tSyntax1
		End Function

		<Extension>
		Friend Function AddLeadingSyntax(Of TSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal node As TSyntax, ByVal unexpected As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal errorId As ERRID) As TSyntax
			Return node.AddLeadingSyntax(DirectCast(unexpected, GreenNode), errorId)
		End Function

		<Extension>
		Friend Function AddLeadingSyntax(Of TSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal node As TSyntax, ByVal unexpected As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)) As TSyntax
			Return node.AddLeadingSyntax(unexpected.Node)
		End Function

		<Extension>
		Friend Function AddLeadingSyntax(Of TSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal node As TSyntax, ByVal unexpected As GreenNode, ByVal errorId As ERRID) As TSyntax
			Dim tSyntax1 As TSyntax
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(errorId)
			tSyntax1 = If(unexpected Is Nothing, DirectCast(node.AddError(diagnosticInfo), TSyntax), node.AddLeadingTrivia(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxNodeExtensions.CreateSkippedTrivia(unexpected, False, False, diagnosticInfo)))
			Return tSyntax1
		End Function

		<Extension>
		Friend Function AddLeadingSyntax(Of TSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal node As TSyntax, ByVal unexpected As GreenNode) As TSyntax
			Dim tSyntax1 As TSyntax
			tSyntax1 = If(unexpected Is Nothing, node, node.AddLeadingTrivia(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxNodeExtensions.CreateSkippedTrivia(unexpected, True, False, Nothing)))
			Return tSyntax1
		End Function

		<Extension>
		Private Function AddLeadingTrivia(Of TSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal node As TSyntax, ByVal trivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As TSyntax
			Dim tSyntax1 As TSyntax
			Dim tSyntax2 As TSyntax
			If (node Is Nothing) Then
				Throw New ArgumentNullException("node")
			End If
			If (trivia.Any()) Then
				Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
				If (syntaxToken Is Nothing) Then
					tSyntax2 = FirstTokenReplacer.Replace(Of TSyntax)(node, Function(t As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.AddLeadingTrivia(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(t, trivia))
				Else
					If (Not Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxNodeExtensions.IsMissingToken(syntaxToken)) Then
						syntaxToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.AddLeadingTrivia(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(syntaxToken, trivia)
					Else
						Dim startOfTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = trivia.GetStartOfTrivia()
						Dim endOfTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = trivia.GetEndOfTrivia()
						syntaxToken = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.AddLeadingTrivia(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(syntaxToken, startOfTrivia).AddTrailingTrivia(endOfTrivia)
					End If
					tSyntax2 = DirectCast(syntaxToken, TSyntax)
				End If
				tSyntax1 = tSyntax2
			Else
				tSyntax1 = node
			End If
			Return tSyntax1
		End Function

		<Extension>
		Friend Function AddTrailingSyntax(Of TSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal node As TSyntax, ByVal unexpected As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken), ByVal errorId As ERRID) As TSyntax
			Dim tSyntax1 As TSyntax
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(errorId)
			tSyntax1 = If(unexpected.Node Is Nothing, DirectCast(node.AddError(diagnosticInfo), TSyntax), node.AddTrailingTrivia(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxNodeExtensions.CreateSkippedTrivia(unexpected.Node, True, True, diagnosticInfo)))
			Return tSyntax1
		End Function

		<Extension>
		Friend Function AddTrailingSyntax(Of TSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal node As TSyntax, ByVal unexpected As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal errorId As ERRID) As TSyntax
			Return node.AddTrailingSyntax(DirectCast(unexpected, GreenNode), errorId)
		End Function

		<Extension>
		Friend Function AddTrailingSyntax(Of TSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal node As TSyntax, ByVal unexpected As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)) As TSyntax
			Return node.AddTrailingSyntax(unexpected.Node)
		End Function

		<Extension>
		Friend Function AddTrailingSyntax(Of TSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal node As TSyntax, ByVal unexpected As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As TSyntax
			Return node.AddTrailingSyntax(DirectCast(unexpected, GreenNode))
		End Function

		<Extension>
		Friend Function AddTrailingSyntax(Of TSyntax As GreenNode)(ByVal node As TSyntax, ByVal unexpected As GreenNode, ByVal errorId As ERRID) As TSyntax
			Dim tSyntax1 As TSyntax
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(errorId)
			tSyntax1 = If(unexpected Is Nothing, DirectCast(node.AddError(diagnosticInfo), TSyntax), node.AddTrailingTrivia(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxNodeExtensions.CreateSkippedTrivia(unexpected, False, False, diagnosticInfo)))
			Return tSyntax1
		End Function

		<Extension>
		Friend Function AddTrailingSyntax(Of TSyntax As GreenNode)(ByVal node As TSyntax, ByVal unexpected As GreenNode) As TSyntax
			Dim tSyntax1 As TSyntax
			If (unexpected Is Nothing) Then
				tSyntax1 = node
			Else
				Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode) = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxNodeExtensions.CreateSkippedTrivia(unexpected, True, False, Nothing)
				tSyntax1 = node.AddTrailingTrivia(syntaxList)
			End If
			Return tSyntax1
		End Function

		<Extension>
		Friend Function AddTrailingTrivia(Of TSyntax As GreenNode)(ByVal node As TSyntax, ByVal trivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)) As TSyntax
			Dim tSyntax1 As TSyntax
			If (node Is Nothing) Then
				Throw New ArgumentNullException("node")
			End If
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			tSyntax1 = If(syntaxToken Is Nothing, LastTokenReplacer.Replace(Of TSyntax)(node, Function(t As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.AddTrailingTrivia(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(t, trivia)), DirectCast(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.AddTrailingTrivia(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(syntaxToken, trivia), TSyntax))
			Return tSyntax1
		End Function

		Private Sub ClearConditionalAccessStack(ByVal conditionalAccessStack As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax))
			If (conditionalAccessStack IsNot Nothing) Then
				conditionalAccessStack.Clear()
			End If
		End Sub

		Friend Sub CollectConstituentTokensAndDiagnostics(ByVal this As GreenNode, ByVal tokenListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken), ByVal nonTokenDiagnostics As IList(Of DiagnosticInfo))
			If (this IsNot Nothing) Then
				If (this.IsToken) Then
					tokenListBuilder.Add(DirectCast(this, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken))
					Return
				End If
				Dim diagnostics As DiagnosticInfo() = this.GetDiagnostics()
				If (diagnostics IsNot Nothing AndAlso CInt(diagnostics.Length) > 0) Then
					Dim diagnosticInfoArray As DiagnosticInfo() = diagnostics
					For i As Integer = 0 To CInt(diagnosticInfoArray.Length) Step 1
						nonTokenDiagnostics.Add(diagnosticInfoArray(i))
					Next

				End If
				Dim slotCount As Integer = this.SlotCount - 1
				For j As Integer = 0 To slotCount
					Dim slot As GreenNode = this.GetSlot(j)
					If (slot IsNot Nothing) Then
						Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxNodeExtensions.CollectConstituentTokensAndDiagnostics(slot, tokenListBuilder, nonTokenDiagnostics)
					End If
				Next

			End If
		End Sub

		<Extension>
		Friend Function ContainsCommentTrivia(ByVal this As GreenNode) As Boolean
			Dim flag As Boolean
			If (this IsNot Nothing) Then
				Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(this)
				Dim count As Integer = syntaxList.Count - 1
				Dim num As Integer = 0
				While num <= count
					If (syntaxList.ItemUntyped(num).RawKind <> 732) Then
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
			Return flag
		End Function

		<Extension>
		Friend Function ContainsWhitespaceTrivia(ByVal this As GreenNode) As Boolean
			Dim flag As Boolean
			If (this IsNot Nothing) Then
				Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(this)
				Dim count As Integer = syntaxList.Count - 1
				Dim num As Integer = 0
				While num <= count
					Dim rawKind As Integer = syntaxList.ItemUntyped(num).RawKind
					If (rawKind = 729 OrElse rawKind = 730) Then
						flag = True
						Return flag
					Else
						num = num + 1
					End If
				End While
				flag = False
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function CreateSkippedTrivia(ByVal node As GreenNode, ByVal preserveDiagnostics As Boolean, ByVal addDiagnosticToFirstTokenOnly As Boolean, ByVal addDiagnostic As DiagnosticInfo) As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)
			Dim triviaList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)
			If (node.RawKind <> 709) Then
				Dim diagnosticInfos As IList(Of DiagnosticInfo) = New List(Of DiagnosticInfo)()
				Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) = SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken).Create()
				Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxNodeExtensions.CollectConstituentTokensAndDiagnostics(node, syntaxListBuilder, diagnosticInfos)
				If (Not preserveDiagnostics) Then
					diagnosticInfos.Clear()
				End If
				If (addDiagnostic IsNot Nothing) Then
					diagnosticInfos.Add(addDiagnostic)
				End If
				Dim skippedTriviaBuilder As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxNodeExtensions.SkippedTriviaBuilder = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxNodeExtensions.SkippedTriviaBuilder(preserveDiagnostics, addDiagnosticToFirstTokenOnly, diagnosticInfos)
				Dim count As Integer = syntaxListBuilder.Count - 1
				Dim num As Integer = 0
				Do
					Dim item As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = syntaxListBuilder(num)
					skippedTriviaBuilder.AddToken(item, num = 0, num = syntaxListBuilder.Count - 1)
					num = num + 1
				Loop While num <= count
				triviaList = skippedTriviaBuilder.GetTriviaList()
			Else
				If (addDiagnostic IsNot Nothing) Then
					node = node.AddError(addDiagnostic)
				End If
				triviaList = node
			End If
			Return triviaList
		End Function

		<Extension>
		Friend Function ExtractAnonymousTypeMemberName(ByVal input As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByRef isNameDictionaryAccess As Boolean, ByRef isRejectedXmlName As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim conditionalAccessExpressionSyntaxes As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax) = Nothing
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = (' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxNodeExtensions::ExtractAnonymousTypeMemberName(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax,System.Boolean&,System.Boolean&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken ExtractAnonymousTypeMemberName(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax,System.Boolean&,System.Boolean&)
			' 
			' The unary opperator AddressReference is not supported in VisualBasic
			'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.ToString(UnaryOperator ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 876
			'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 1268
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 138
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Expression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2623
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2656
			'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 2150
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 126
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
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(Statement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1060
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(MethodDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1916
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(MethodDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1841
			'    à ..WriteInternal(IMemberDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseLanguageWriter.cs:ligne 447
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com


		<Extension>
		Private Function ExtractAnonymousTypeMemberName(ByRef conditionalAccessStack As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax), ByVal input As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByRef isNameDictionaryAccess As Boolean, ByRef isRejectedXmlName As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim localName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			While True
				Dim kind As SyntaxKind = input.Kind
				If (kind <= SyntaxKind.XmlName) Then
					Select Case kind
						Case SyntaxKind.SimpleMemberAccessExpression
						Case SyntaxKind.DictionaryAccessExpression
							Dim memberAccessExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax = DirectCast(input, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax)
							Dim expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = If(memberAccessExpressionSyntax.Expression, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxNodeExtensions.PopAndGetConditionalAccessReceiver(conditionalAccessStack))
							If (input.Kind <> SyntaxKind.SimpleMemberAccessExpression OrElse expression Is Nothing OrElse CUShort(expression.Kind) - CUShort(SyntaxKind.XmlElementAccessExpression) > CUShort(SyntaxKind.List)) Then
								Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxNodeExtensions.ClearConditionalAccessStack(conditionalAccessStack)
								isNameDictionaryAccess = input.Kind = SyntaxKind.DictionaryAccessExpression
								input = memberAccessExpressionSyntax.Name
								Continue While
							Else
								input = expression
								Continue While
							End If
						Case SyntaxKind.XmlElementAccessExpression
						Case SyntaxKind.XmlDescendantAccessExpression
						Case SyntaxKind.XmlAttributeAccessExpression
							Dim xmlMemberAccessExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax = DirectCast(input, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax)
							Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxNodeExtensions.ClearConditionalAccessStack(conditionalAccessStack)
							input = xmlMemberAccessExpressionSyntax.Name
							Continue While
						Case SyntaxKind.InvocationExpression
							Dim invocationExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InvocationExpressionSyntax = DirectCast(input, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InvocationExpressionSyntax)
							Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = If(invocationExpressionSyntax.Expression, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxNodeExtensions.PopAndGetConditionalAccessReceiver(conditionalAccessStack))
							If (expressionSyntax Is Nothing) Then
								localName = Nothing
								Return localName
							End If
							If (invocationExpressionSyntax.ArgumentList Is Nothing OrElse invocationExpressionSyntax.ArgumentList.Arguments.Count = 0) Then
								input = expressionSyntax
								Continue While
							Else
								If (invocationExpressionSyntax.ArgumentList.Arguments.Count <> 1 OrElse CUShort(expressionSyntax.Kind) - CUShort(SyntaxKind.XmlElementAccessExpression) > CUShort(SyntaxKind.List)) Then
									localName = Nothing
									Return localName
								End If
								input = expressionSyntax
								Continue While
							End If
						Case Else
							If (kind = SyntaxKind.XmlName) Then
								Exit Select
							Else
								localName = Nothing
								Return localName
							End If
					End Select
					Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax = DirectCast(input, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax)
					If (Scanner.IsIdentifier(xmlNameSyntax.LocalName.ToString())) Then
						localName = xmlNameSyntax.LocalName
						Return localName
					Else
						isRejectedXmlName = True
						localName = Nothing
						Return localName
					End If
				ElseIf (kind = SyntaxKind.XmlBracketedName) Then
					input = DirectCast(input, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax).Name
				ElseIf (kind = SyntaxKind.IdentifierName) Then
					localName = DirectCast(input, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax).Identifier
					Return localName
				ElseIf (kind = SyntaxKind.ConditionalAccessExpression) Then
					Dim conditionalAccessExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax = DirectCast(input, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax)
					If (conditionalAccessStack Is Nothing) Then
						conditionalAccessStack = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax).GetInstance()
					End If
					conditionalAccessStack.Push(conditionalAccessExpressionSyntax)
					input = conditionalAccessExpressionSyntax.WhenNotNull
				Else
					Exit While
				End If
			End While
			localName = Nothing
			Return localName
		End Function

		<Extension>
		Friend Function GetEndOfTrivia(ByVal trivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			Return trivia.GetEndOfTrivia(trivia.GetIndexOfEndOfTrivia())
		End Function

		<Extension>
		Friend Function GetEndOfTrivia(ByVal trivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal indexOfEnd As Integer) As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			If (indexOfEnd = 0) Then
				list = trivia
			ElseIf (indexOfEnd <> trivia.Count) Then
				Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode).Create()
				Dim count As Integer = trivia.Count - 1
				Dim num As Integer = indexOfEnd
				Do
					syntaxListBuilder.Add(trivia(num))
					num = num + 1
				Loop While num <= count
				list = syntaxListBuilder.ToList()
			Else
				list = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
			End If
			Return list
		End Function

		<Extension>
		Private Function GetIndexAfterLastSkippedToken(ByVal trivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As Integer
			Dim num As Integer
			Dim count As Integer = trivia.Count - 1
			While True
				If (count < 0) Then
					num = 0
					Exit While
				ElseIf (trivia(count).Kind <> SyntaxKind.SkippedTokensTrivia) Then
					count += -1
				Else
					num = count + 1
					Exit While
				End If
			End While
			Return num
		End Function

		<Extension>
		Private Function GetIndexOfEndOfTrivia(ByVal trivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As Integer
			Dim num As Integer
			Dim count As Integer = trivia.Count
			If (count > 0) Then
				Dim num1 As Integer = count - 1
				Select Case trivia(num1).Kind
					Case SyntaxKind.EndOfLineTrivia
						If (num1 <= 0) Then
							num = num1
							Exit Select
						Else
							Dim kind As SyntaxKind = trivia(num1 - 1).Kind
							If (kind = SyntaxKind.CommentTrivia) Then
								num = num1 - 1
								Exit Select
							ElseIf (kind <> SyntaxKind.LineContinuationTrivia) Then
								num = num1
								Exit Select
							Else
								num = count
								Exit Select
							End If
						End If
					Case SyntaxKind.ColonTrivia
						num = num1
						Exit Select
					Case SyntaxKind.CommentTrivia
					Case SyntaxKind.DocumentationCommentExteriorTrivia
					Case SyntaxKind.DisabledTextTrivia
					Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.EventStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.SyncLockStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OpenParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia
					Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.EventStatement Or SyntaxKind.OperatorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueDoStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.OnErrorResumeNextStatement Or SyntaxKind.SyncLockStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.StaticKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OffKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OutKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.AsteriskToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.QuestionToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.StatementTerminatorToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.EmptyToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlCrefAttribute Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.IfDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.ElseDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia Or SyntaxKind.RegionDirectiveTrivia
					Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.SubBlock Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.EventStatement Or SyntaxKind.OperatorStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.AddHandlerAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.RaiseEventAccessorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.Attribute Or SyntaxKind.AttributeTarget Or SyntaxKind.AttributesStatement Or SyntaxKind.ExpressionStatement Or SyntaxKind.PrintStatement Or SyntaxKind.WhileBlock Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueDoStatement Or SyntaxKind.ContinueForStatement Or SyntaxKind.ReturnStatement Or SyntaxKind.SingleLineIfStatement Or SyntaxKind.SingleLineIfPart Or SyntaxKind.SingleLineElseClause Or SyntaxKind.MultiLineIfBlock Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.OnErrorResumeNextStatement Or SyntaxKind.ResumeStatement Or SyntaxKind.ResumeLabelStatement Or SyntaxKind.ResumeNextStatement Or SyntaxKind.SelectBlock Or SyntaxKind.SelectStatement Or SyntaxKind.CaseBlock Or SyntaxKind.SyncLockStatement Or SyntaxKind.WhileStatement Or SyntaxKind.ForBlock Or SyntaxKind.ForEachBlock Or SyntaxKind.ForStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OrElseKeyword Or SyntaxKind.OverloadsKeyword Or SyntaxKind.OverridableKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.StaticKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.StructureKeyword Or SyntaxKind.SubKeyword Or SyntaxKind.SyncLockKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AscendingKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.AutoKeyword Or SyntaxKind.BinaryKeyword Or SyntaxKind.ByKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OffKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OutKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.RegionKeyword Or SyntaxKind.SkipKeyword Or SyntaxKind.StrictKeyword Or SyntaxKind.TakeKeyword Or SyntaxKind.TextKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.AsteriskToken Or SyntaxKind.PlusToken Or SyntaxKind.MinusToken Or SyntaxKind.DotToken Or SyntaxKind.SlashToken Or SyntaxKind.ColonToken Or SyntaxKind.LessThanToken Or SyntaxKind.LessThanEqualsToken Or SyntaxKind.LessThanGreaterThanToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.QuestionToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.StatementTerminatorToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.EmptyToken Or SyntaxKind.SlashGreaterThanToken Or SyntaxKind.LessThanSlashToken Or SyntaxKind.LessThanExclamationMinusMinusToken Or SyntaxKind.MinusMinusGreaterThanToken Or SyntaxKind.LessThanQuestionToken Or SyntaxKind.QuestionGreaterThanToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlCrefAttribute Or SyntaxKind.XmlNameAttribute Or SyntaxKind.ConditionalAccessExpression Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.IfDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.ElseDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia Or SyntaxKind.RegionDirectiveTrivia Or SyntaxKind.EndRegionDirectiveTrivia Or SyntaxKind.ExternalSourceDirectiveTrivia Or SyntaxKind.EndExternalSourceDirectiveTrivia Or SyntaxKind.ExternalChecksumDirectiveTrivia Or SyntaxKind.EnableWarningDirectiveTrivia Or SyntaxKind.DisableWarningDirectiveTrivia Or SyntaxKind.ReferenceDirectiveTrivia
					Case SyntaxKind.EndFunctionStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.FunctionBlock Or SyntaxKind.DeclareSubStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.UsingBlock Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ForStepClause Or SyntaxKind.NotKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanPercentEqualsToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.ConstDirectiveTrivia
						num = count
						Return num
					Case SyntaxKind.LineContinuationTrivia
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
						Throw ExceptionUtilities.UnexpectedValue(trivia(num1).Kind)
					Case Else
						num = count
						Return num
				End Select
			Else
				num = count
				Return num
			End If
			Return num
		End Function

		Friend Function GetLengthOfCommonEnd(ByVal trivia1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal trivia2 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As Integer
			Dim count As Integer = trivia1.Count
			Dim num As Integer = trivia2.Count
			Dim indexAfterLastSkippedToken As Integer = trivia1.GetIndexAfterLastSkippedToken()
			Dim indexAfterLastSkippedToken1 As Integer = trivia2.GetIndexAfterLastSkippedToken()
			Return Math.Min(count - indexAfterLastSkippedToken, num - indexAfterLastSkippedToken1)
		End Function

		<Extension>
		Friend Function GetStartOfTrivia(ByVal trivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)) As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			Return trivia.GetStartOfTrivia(trivia.GetIndexOfEndOfTrivia())
		End Function

		<Extension>
		Friend Function GetStartOfTrivia(ByVal trivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode), ByVal indexOfEnd As Integer) As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			If (indexOfEnd = 0) Then
				list = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)()
			ElseIf (indexOfEnd <> trivia.Count) Then
				Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode).Create()
				Dim num As Integer = indexOfEnd - 1
				Dim num1 As Integer = 0
				Do
					syntaxListBuilder.Add(trivia(num1))
					num1 = num1 + 1
				Loop While num1 <= num
				list = syntaxListBuilder.ToList()
			Else
				list = trivia
			End If
			Return list
		End Function

		Friend Function IsExecutableStatementOrItsPart(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Boolean
			Dim flag As Boolean
			If (Not TypeOf node Is Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExecutableStatementSyntax) Then
				Dim kind As SyntaxKind = node.Kind
				If (kind <= SyntaxKind.CaseElseStatement) Then
					If (kind > SyntaxKind.CatchStatement) Then
						If (kind = SyntaxKind.FinallyStatement OrElse kind = SyntaxKind.SelectStatement OrElse CUShort(kind) - CUShort(SyntaxKind.CaseStatement) <= CUShort(SyntaxKind.List)) Then
							GoTo Label1
						End If
						flag = False
						Return flag
					Else
						If (CUShort(kind) - CUShort(SyntaxKind.IfStatement) <= CUShort(SyntaxKind.EmptyStatement) OrElse CUShort(kind) - CUShort(SyntaxKind.TryStatement) <= CUShort(SyntaxKind.List)) Then
							GoTo Label1
						End If
						flag = False
						Return flag
					End If
				ElseIf (kind > SyntaxKind.WhileStatement) Then
					Select Case kind
						Case SyntaxKind.ForStatement
						Case SyntaxKind.ForEachStatement
						Case SyntaxKind.UsingStatement
							Exit Select
						Case SyntaxKind.ForStepClause
						Case SyntaxKind.NextStatement
							flag = False
							Return flag
						Case Else
							If (kind = SyntaxKind.WithStatement OrElse CUShort(kind) - CUShort(SyntaxKind.SimpleDoStatement) <= CUShort(SyntaxKind.EmptyStatement)) Then
								Exit Select
							Else
								flag = False
								Return flag
							End If
					End Select
				Else
					If (kind = SyntaxKind.SyncLockStatement OrElse kind = SyntaxKind.WhileStatement) Then
						GoTo Label1
					End If
					flag = False
					Return flag
				End If
			Label1:
				flag = True
			Else
				flag = True
			End If
			Return flag
		End Function

		Private Function IsMissingToken(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Boolean
			If (token.Width <> 0) Then
				Return False
			End If
			Return token.Kind <> SyntaxKind.EmptyToken
		End Function

		Private Function PopAndGetConditionalAccessReceiver(ByVal conditionalAccessStack As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			If (conditionalAccessStack Is Nothing OrElse conditionalAccessStack.Count = 0) Then
				expression = Nothing
			Else
				expression = conditionalAccessStack.Pop().Expression
			End If
			Return expression
		End Function

		Private Function TriviaListContainsStructuredTrivia(ByVal triviaList As GreenNode) As Boolean
			Dim flag As Boolean
			If (triviaList IsNot Nothing) Then
				Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(triviaList)
				Dim count As Integer = syntaxList.Count - 1
				Dim num As Integer = 0
				While True
					If (num <= count) Then
						Dim rawKind As Integer = syntaxList.ItemUntyped(num).RawKind
						If (rawKind = 378 OrElse rawKind = 709) Then
							Exit While
						End If
						Select Case rawKind
							Case 736
							Case 737
							Case 738
							Case 739
							Case 740
							Case 741
							Case 744
							Case 745
							Case 746
							Case 747
							Case 748
							Case 749
							Case 750
							Case 753
								Exit Select
							Case 742
							Case 743
							Case 751
							Case 752
							Label1:
								num = num + 1
								Continue While
							Case Else
								GoTo Label1
						End Select
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

		Private Class SkippedTriviaBuilder
			Private _triviaListBuilder As SyntaxListBuilder(Of GreenNode)

			Private ReadOnly _skippedTokensBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)

			Private ReadOnly _preserveExistingDiagnostics As Boolean

			Private _addDiagnosticsToFirstTokenOnly As Boolean

			Private _diagnosticsToAdd As IEnumerable(Of DiagnosticInfo)

			Public Sub New(ByVal preserveExistingDiagnostics As Boolean, ByVal addDiagnosticsToFirstTokenOnly As Boolean, ByVal diagnosticsToAdd As IEnumerable(Of DiagnosticInfo))
				MyBase.New()
				Me._triviaListBuilder = SyntaxListBuilder(Of GreenNode).Create()
				Me._skippedTokensBuilder = SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken).Create()
				Me._addDiagnosticsToFirstTokenOnly = addDiagnosticsToFirstTokenOnly
				Me._preserveExistingDiagnostics = preserveExistingDiagnostics
				Me._diagnosticsToAdd = diagnosticsToAdd
			End Sub

			Public Sub AddToken(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal isFirst As Boolean, ByVal isLast As Boolean)
				Dim isMissing As Boolean = token.IsMissing
				If (token.HasLeadingTrivia AndAlso (isFirst OrElse isMissing OrElse Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxNodeExtensions.TriviaListContainsStructuredTrivia(token.GetLeadingTrivia()))) Then
					Me.FinishInProgressTokens()
					Me.AddTrivia(token.GetLeadingTrivia())
					token = DirectCast(token.WithLeadingTrivia(Nothing), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
				End If
				If (Not Me._preserveExistingDiagnostics) Then
					token = token.WithoutDiagnostics()
				End If
				Dim trailingTrivia As GreenNode = Nothing
				If (token.HasTrailingTrivia AndAlso (isLast OrElse isMissing OrElse Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxNodeExtensions.TriviaListContainsStructuredTrivia(token.GetTrailingTrivia()))) Then
					trailingTrivia = token.GetTrailingTrivia()
					token = DirectCast(token.WithTrailingTrivia(Nothing), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
				End If
				If (Not isMissing) Then
					Me._skippedTokensBuilder.Add(token)
				ElseIf (token.ContainsDiagnostics) Then
					If (Me._diagnosticsToAdd Is Nothing) Then
						Me._diagnosticsToAdd = token.GetDiagnostics()
					Else
						Me._diagnosticsToAdd = Me._diagnosticsToAdd.Concat(token.GetDiagnostics())
					End If
					Me._addDiagnosticsToFirstTokenOnly = True
				End If
				If (trailingTrivia IsNot Nothing) Then
					Me.FinishInProgressTokens()
					Me.AddTrivia(trailingTrivia)
				End If
				If (isFirst AndAlso Me._addDiagnosticsToFirstTokenOnly) Then
					Me.FinishInProgressTokens()
				End If
			End Sub

			Private Sub AddTrivia(ByVal trivia As GreenNode)
				Me.FinishInProgressTokens()
				Me._triviaListBuilder.AddRange(trivia)
			End Sub

			Private Sub FinishInProgressTokens()
				Dim enumerator As IEnumerator(Of DiagnosticInfo) = Nothing
				If (Me._skippedTokensBuilder.Count > 0) Then
					Dim greenNode As Microsoft.CodeAnalysis.GreenNode = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.SkippedTokensTrivia(Me._skippedTokensBuilder.ToList())
					If (Me._diagnosticsToAdd IsNot Nothing) Then
						Using enumerator
							enumerator = Me._diagnosticsToAdd.GetEnumerator()
							While enumerator.MoveNext()
								greenNode = greenNode.AddError(enumerator.Current)
							End While
						End Using
						Me._diagnosticsToAdd = Nothing
					End If
					Me._triviaListBuilder.Add(greenNode)
					Me._skippedTokensBuilder.Clear()
				End If
			End Sub

			Public Function GetTriviaList() As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
				Me.FinishInProgressTokens()
				If (Me._diagnosticsToAdd IsNot Nothing AndAlso Me._diagnosticsToAdd.Any() AndAlso Me._triviaListBuilder.Count > 0) Then
					Me._triviaListBuilder(Me._triviaListBuilder.Count - 1) = Me._triviaListBuilder(Me._triviaListBuilder.Count - 1).WithAdditionalDiagnostics(Me._diagnosticsToAdd.ToArray())
				End If
				Return Me._triviaListBuilder.ToList()
			End Function
		End Class
	End Module
End Namespace