Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Friend Class SyntaxNormalizer
		Inherits VisualBasicSyntaxRewriter
		Private ReadOnly _consideredSpan As TextSpan

		Private ReadOnly _indentWhitespace As String

		Private ReadOnly _eolTrivia As SyntaxTrivia

		Private ReadOnly _useElasticTrivia As Boolean

		Private ReadOnly _useDefaultCasing As Boolean

		Private _isInStructuredTrivia As Boolean

		Private _previousToken As SyntaxToken

		Private _afterLineBreak As Boolean

		Private _afterIndentation As Boolean

		Private ReadOnly _lineBreaksAfterToken As Dictionary(Of SyntaxToken, Integer)

		Private ReadOnly _lastStatementsInBlocks As HashSet(Of SyntaxNode)

		Private _indentationDepth As Integer

		Private _indentations As ArrayBuilder(Of SyntaxTrivia)

		Private Sub New(ByVal consideredSpan As TextSpan, ByVal indentWhitespace As String, ByVal eolWhitespace As String, ByVal useElasticTrivia As Boolean, ByVal useDefaultCasing As Boolean)
			MyBase.New(True)
			Me._lineBreaksAfterToken = New Dictionary(Of SyntaxToken, Integer)()
			Me._lastStatementsInBlocks = New HashSet(Of SyntaxNode)()
			Me._consideredSpan = consideredSpan
			Me._indentWhitespace = indentWhitespace
			Me._useElasticTrivia = useElasticTrivia
			Me._eolTrivia = If(useElasticTrivia, SyntaxFactory.ElasticEndOfLine(eolWhitespace), SyntaxFactory.EndOfLine(eolWhitespace))
			Me._useDefaultCasing = useDefaultCasing
			Me._afterLineBreak = True
		End Sub

		Private Sub AddLinebreaksAfterElementsIfNeeded(Of TNode As SyntaxNode)(ByVal list As SyntaxList(Of TNode), ByVal linebreaksBetweenElements As Integer, ByVal linebreaksAfterLastElement As Integer)
			Dim count As Integer = list.Count - 1
			Dim num As Integer = count
			For i As Integer = 0 To num
				Dim item As TNode = list(i)
				If (DirectCast(item, SyntaxNode).Kind() <> SyntaxKind.LabelStatement) Then
					Me.AddLinebreaksAfterTokenIfNeeded(item.GetLastToken(False, False, False, False), If(i = count, linebreaksAfterLastElement, linebreaksBetweenElements))
				Else
					Me._lineBreaksAfterToken(item.GetLastToken(False, False, False, False)) = 1
				End If
			Next

		End Sub

		Private Sub AddLinebreaksAfterTokenIfNeeded(ByVal node As SyntaxToken, ByVal linebreaksAfterToken As Integer)
			If (Not Me.EndsWithColonSeparator(node)) Then
				Me._lineBreaksAfterToken(node) = linebreaksAfterToken
			End If
		End Sub

		Private Function EndsInLineBreak(ByVal trivia As SyntaxTrivia) As Boolean
			Dim flag As Boolean
			If (trivia.Kind() = SyntaxKind.EndOfLineTrivia) Then
				flag = True
			ElseIf (trivia.Kind() <> SyntaxKind.DisabledTextTrivia) Then
				flag = If(Not trivia.HasStructure OrElse Not trivia.GetStructure().GetLastToken(False, False, False, False).HasTrailingTrivia OrElse trivia.GetStructure().GetLastToken(False, False, False, False).TrailingTrivia.Last().Kind() <> SyntaxKind.EndOfLineTrivia, False, True)
			Else
				Dim fullString As String = trivia.ToFullString()
				flag = If(fullString.Length <= 0, False, SyntaxNormalizer.IsNewLineChar(fullString.Last()))
			End If
			Return flag
		End Function

		Private Function EndsWithColonSeparator(ByVal node As SyntaxToken) As Boolean
			If (Not node.HasTrailingTrivia) Then
				Return False
			End If
			Dim trailingTrivia As SyntaxTriviaList = node.TrailingTrivia
			Return trailingTrivia.Last().Kind() = SyntaxKind.ColonTrivia
		End Function

		Private Sub Free()
			If (Me._indentations IsNot Nothing) Then
				Me._indentations.Free()
			End If
		End Sub

		Private Function GetEndOfLine() As SyntaxTrivia
			Return Me._eolTrivia
		End Function

		Private Function GetIndentation(ByVal count As Integer) As SyntaxTrivia
			Dim str As String
			Dim num As Integer = count + 1
			If (Me._indentations IsNot Nothing) Then
				Me._indentations.EnsureCapacity(num)
			Else
				Me._indentations = ArrayBuilder(Of SyntaxTrivia).GetInstance(num)
			End If
			Dim num1 As Integer = count
			Dim num2 As Integer = Me._indentations.Count
			Do
				If (num2 = 0) Then
					str = ""
				Else
					Dim item As SyntaxTrivia = Me._indentations(num2 - 1)
					str = [String].Concat(item.ToString(), Me._indentWhitespace)
				End If
				Dim str1 As String = str
				Me._indentations.Add(If(Me._useElasticTrivia, SyntaxFactory.ElasticWhitespace(str1), SyntaxFactory.Whitespace(str1)))
				num2 = num2 + 1
			Loop While num2 <= num1
			Return Me._indentations(count)
		End Function

		Private Function GetIndentationDepth() As Integer
			Return Me._indentationDepth
		End Function

		Private Function GetIndentationDepth(ByVal trivia As SyntaxTrivia) As Integer
			Dim num As Integer
			num = If(Not SyntaxFacts.IsPreprocessorDirective(trivia.Kind()), Me.GetIndentationDepth(), 0)
			Return num
		End Function

		Private Function GetNextRelevantToken(ByVal token As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken
			Dim func As Func(Of Microsoft.CodeAnalysis.SyntaxToken, Boolean)
			Dim func1 As Func(Of SyntaxTrivia, Boolean)
			If (SyntaxNormalizer._Closure$__.$I36-0 Is Nothing) Then
				func = Function(t As Microsoft.CodeAnalysis.SyntaxToken) t.Kind() <> SyntaxKind.None
				SyntaxNormalizer._Closure$__.$I36-0 = func
			Else
				func = SyntaxNormalizer._Closure$__.$I36-0
			End If
			If (SyntaxNormalizer._Closure$__.$I36-1 Is Nothing) Then
				func1 = Function(t As SyntaxTrivia) False
				SyntaxNormalizer._Closure$__.$I36-1 = func1
			Else
				func1 = SyntaxNormalizer._Closure$__.$I36-1
			End If
			Dim nextToken As Microsoft.CodeAnalysis.SyntaxToken = token.GetNextToken(func, func1)
			syntaxToken = If(Not Me._consideredSpan.Contains(nextToken.FullSpan), New Microsoft.CodeAnalysis.SyntaxToken(), nextToken)
			Return syntaxToken
		End Function

		Private Function GetSpace() As SyntaxTrivia
			If (Not Me._useElasticTrivia) Then
				Return SyntaxFactory.Space
			End If
			Return SyntaxFactory.ElasticSpace
		End Function

		Private Function IsLastTokenOnLine(ByVal token As SyntaxToken) As Boolean
			If (token.HasTrailingTrivia AndAlso token.TrailingTrivia.Last().Kind() = SyntaxKind.ColonTrivia) Then
				Return True
			End If
			If (token.Parent Is Nothing) Then
				Return False
			End If
			Return token.Parent.GetLastToken(False, False, False, False) = token
		End Function

		Private Shared Function IsNewLineChar(ByVal ch As Char) As Boolean
			If (EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(ch), "" & VbCrLf & "", False) = 0 OrElse EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(ch), "", False) = 0 OrElse EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(ch), "\u0085", False) = 0 OrElse EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(ch), "\u2028", False) = 0) Then
				Return True
			End If
			Return EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(ch), "\u2029", False) = 0
		End Function

		Private Function LineBreaksBetween(ByVal currentToken As SyntaxToken, ByVal nextToken As SyntaxToken) As Integer
			Dim num As Integer
			If (currentToken.Kind() = SyntaxKind.None OrElse nextToken.Kind() = SyntaxKind.None) Then
				num = 0
			Else
				Dim num1 As Integer = 0
				num = If(Not Me._lineBreaksAfterToken.TryGetValue(currentToken, num1), 0, num1)
			End If
			Return num
		End Function

		Private Sub MarkLastStatementIfNeeded(Of TNode As SyntaxNode)(ByVal list As SyntaxList(Of TNode))
			If (list.Any()) Then
				Me._lastStatementsInBlocks.Add(DirectCast(list.Last(), SyntaxNode))
			End If
		End Sub

		Private Function NeedsIndentAfterLineBreak(ByVal trivia As SyntaxTrivia) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = trivia.Kind()
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DocumentationCommentTrivia OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommentTrivia OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DocumentationCommentExteriorTrivia, True, False)
			Return flag
		End Function

		Private Function NeedsLineBreakAfter(ByVal trivia As SyntaxTrivia) As Boolean
			Return trivia.Kind() = SyntaxKind.CommentTrivia
		End Function

		Private Function NeedsLineBreakBefore(ByVal trivia As SyntaxTrivia) As Boolean
			Dim flag As Boolean
			flag = If(trivia.Kind() <> SyntaxKind.DocumentationCommentExteriorTrivia, False, True)
			Return flag
		End Function

		Private Function NeedsLineBreakBetween(ByVal trivia As SyntaxTrivia, ByVal nextTrivia As SyntaxTrivia, ByVal isTrailingTrivia As Boolean) As Boolean
			Dim flag As Boolean
			If (Not Me.EndsInLineBreak(trivia)) Then
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = nextTrivia.Kind()
				If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement) Then
					Select Case syntaxKind
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommentTrivia
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DocumentationCommentExteriorTrivia
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstDirectiveTrivia
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IfDirectiveTrivia
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseIfDirectiveTrivia
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseDirectiveTrivia
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfDirectiveTrivia
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RegionDirectiveTrivia
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRegionDirectiveTrivia
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalSourceDirectiveTrivia
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndExternalSourceDirectiveTrivia
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalChecksumDirectiveTrivia
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnableWarningDirectiveTrivia
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DisableWarningDirectiveTrivia
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReferenceDirectiveTrivia
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BadDirectiveTrivia
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LineContinuationTrivia
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DisabledTextTrivia
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterSingleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayRankSpecifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FinallyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnErrorGoToZeroStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnErrorGoToLabelStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SharedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WriteOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariantKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsFalseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrderKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SemicolonToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanGreaterThanToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanGreaterThanEqualsToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoubleQuoteToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOfFileToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DateLiteralToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharacterLiteralToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DocumentationCommentTrivia Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstDirectiveTrivia Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseIfDirectiveTrivia Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfDirectiveTrivia
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterSingleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterMultipleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsValue Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModifiedIdentifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayRankSpecifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributeList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueDoStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FinallyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ErrorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnErrorGoToZeroStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnErrorGoToMinusOneStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnErrorGoToLabelStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnErrorResumeNextStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShadowsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SharedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShortKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StaticKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WriteOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GosubKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariantKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WendKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsFalseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsTrueKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OffKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrderKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OutKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseParenToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SemicolonToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsteriskToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanGreaterThanToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanLessThanEqualsToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanGreaterThanEqualsToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QuestionToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoubleQuoteToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StatementTerminatorToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOfFileToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DateLiteralToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StringLiteralToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharacterLiteralToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkippedTokensTrivia Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DocumentationCommentTrivia Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlCrefAttribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstDirectiveTrivia Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IfDirectiveTrivia Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseIfDirectiveTrivia Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseDirectiveTrivia Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfDirectiveTrivia Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RegionDirectiveTrivia
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterSingleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterMultipleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsValue Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModifiedIdentifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayRankSpecifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributeList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Attribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributeTarget Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributesStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExpressionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrintStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueDoStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueForStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReturnStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfPart Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineElseClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineIfBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FinallyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ErrorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnErrorGoToZeroStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnErrorGoToMinusOneStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnErrorGoToLabelStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnErrorResumeNextStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ResumeStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ResumeLabelStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ResumeNextStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForEachBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionalKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrElseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverloadsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShadowsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SharedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShortKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StaticKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StepKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StopKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StringKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WriteOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GosubKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariantKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WendKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AllKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnsiKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AscendingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AssemblyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AutoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BinaryKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsFalseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsTrueKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OffKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrderKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OutKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PreserveKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RegionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkipKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StrictKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TakeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TextKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseParenToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SemicolonToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsteriskToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PlusToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MinusToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DotToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SlashToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ColonToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanEqualsToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanGreaterThanToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanGreaterThanToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanLessThanEqualsToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanGreaterThanEqualsToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QuestionToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DoubleQuoteToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StatementTerminatorToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOfFileToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SlashGreaterThanToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanSlashToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanExclamationMinusMinusToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MinusMinusGreaterThanToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanQuestionToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QuestionGreaterThanToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DateLiteralToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StringLiteralToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharacterLiteralToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkippedTokensTrivia Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DocumentationCommentTrivia Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlCrefAttribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNameAttribute Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConditionalAccessExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstDirectiveTrivia Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IfDirectiveTrivia Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseIfDirectiveTrivia Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseDirectiveTrivia Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfDirectiveTrivia Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RegionDirectiveTrivia Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRegionDirectiveTrivia Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalSourceDirectiveTrivia Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndExternalSourceDirectiveTrivia Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalChecksumDirectiveTrivia Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnableWarningDirectiveTrivia Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DisableWarningDirectiveTrivia Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReferenceDirectiveTrivia
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForStepClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ThenKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompareKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsFalseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UnicodeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanGreaterThanToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanPercentEqualsToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DateLiteralToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstDirectiveTrivia
							flag = False
							Return flag
						Case Else
							flag = False
							Return flag
					End Select
				End If
				flag = Not isTrailingTrivia
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function NeedsSeparator(ByVal token As SyntaxToken, ByVal nextToken As SyntaxToken) As Boolean
			Dim flag As Boolean
			If (token.Kind() = SyntaxKind.EndOfFileToken) Then
				flag = False
			ElseIf (token.Parent Is Nothing OrElse nextToken.Kind() = SyntaxKind.None) Then
				flag = False
			ElseIf (nextToken.Parent.Kind() = SyntaxKind.SingleLineFunctionLambdaExpression) Then
				flag = True
			ElseIf (nextToken.Kind() = SyntaxKind.EndOfFileToken) Then
				flag = False
			ElseIf (TypeOf token.Parent Is UnaryExpressionSyntax AndAlso token.Kind() <> SyntaxKind.NotKeyword AndAlso token.Kind() <> SyntaxKind.AddressOfKeyword) Then
				flag = False
			ElseIf (TypeOf token.Parent Is BinaryExpressionSyntax OrElse TypeOf nextToken.Parent Is BinaryExpressionSyntax) Then
				flag = True
			ElseIf (token.Kind() = SyntaxKind.OpenParenToken) Then
				flag = False
			ElseIf (nextToken.Kind() = SyntaxKind.CloseParenToken) Then
				flag = False
			ElseIf (token.Kind() <> SyntaxKind.CommaToken AndAlso nextToken.Kind() = SyntaxKind.OpenParenToken) Then
				flag = False
			ElseIf (token.Kind() = SyntaxKind.CommaToken AndAlso (nextToken.Kind() = SyntaxKind.EmptyToken OrElse token.Parent.Kind() = SyntaxKind.InterpolationAlignmentClause) OrElse nextToken.Kind() = SyntaxKind.CommaToken) Then
				flag = False
			ElseIf (token.Kind() = SyntaxKind.DotToken) Then
				flag = False
			ElseIf (nextToken.Kind() <> SyntaxKind.DotToken OrElse nextToken.Parent.Kind() = SyntaxKind.NamedFieldInitializer) Then
				If (nextToken.Kind() = SyntaxKind.ColonToken) Then
					If (token.Parent.Kind() <> SyntaxKind.LabelStatement) Then
						If (nextToken.Parent.Kind() <> SyntaxKind.InterpolationFormatClause) Then
							GoTo Label1
						End If
						flag = False
						Return flag
					Else
						flag = False
						Return flag
					End If
				End If
				If (token.Kind() = SyntaxKind.OpenBraceToken OrElse nextToken.Kind() = SyntaxKind.CloseBraceToken) Then
					flag = False
				ElseIf (token.Kind() = SyntaxKind.ColonEqualsToken OrElse nextToken.Kind() = SyntaxKind.ColonEqualsToken) Then
					flag = False
				ElseIf (SyntaxFacts.IsRelationalCaseClause(token.Parent.Kind()) OrElse SyntaxFacts.IsRelationalCaseClause(nextToken.Parent.Kind())) Then
					flag = True
				ElseIf (token.Kind() = SyntaxKind.GreaterThanToken AndAlso token.Parent.Kind() = SyntaxKind.AttributeList) Then
					flag = True
				ElseIf (token.Kind() = SyntaxKind.LessThanToken OrElse nextToken.Kind() = SyntaxKind.GreaterThanToken OrElse token.Kind() = SyntaxKind.LessThanSlashToken OrElse token.Kind() = SyntaxKind.GreaterThanToken OrElse nextToken.Kind() = SyntaxKind.LessThanSlashToken) Then
					flag = False
				ElseIf (token.Kind() = SyntaxKind.ColonToken AndAlso token.Parent.Kind() = SyntaxKind.XmlPrefix OrElse nextToken.Kind() = SyntaxKind.ColonToken AndAlso nextToken.Parent.Kind() = SyntaxKind.XmlPrefix) Then
					flag = False
				ElseIf (nextToken.Kind() = SyntaxKind.SlashGreaterThanToken) Then
					flag = False
				ElseIf (token.Kind() = SyntaxKind.LessThanExclamationMinusMinusToken OrElse nextToken.Kind() = SyntaxKind.MinusMinusGreaterThanToken) Then
					flag = False
				ElseIf (token.Kind() = SyntaxKind.LessThanQuestionToken) Then
					flag = False
				ElseIf (token.Kind() = SyntaxKind.BeginCDataToken OrElse nextToken.Kind() = SyntaxKind.EndCDataToken) Then
					flag = False
				ElseIf (token.Kind() = SyntaxKind.ColonToken AndAlso token.Parent.Kind() = SyntaxKind.AttributeTarget OrElse nextToken.Kind() = SyntaxKind.ColonToken AndAlso nextToken.Parent.Kind() = SyntaxKind.AttributeTarget) Then
					flag = False
				ElseIf (token.Kind() = SyntaxKind.EqualsToken AndAlso (token.Parent.Kind() = SyntaxKind.XmlAttribute OrElse token.Parent.Kind() = SyntaxKind.XmlCrefAttribute OrElse token.Parent.Kind() = SyntaxKind.XmlNameAttribute OrElse token.Parent.Kind() = SyntaxKind.XmlDeclaration) OrElse nextToken.Kind() = SyntaxKind.EqualsToken AndAlso (nextToken.Parent.Kind() = SyntaxKind.XmlAttribute OrElse nextToken.Parent.Kind() = SyntaxKind.XmlCrefAttribute OrElse nextToken.Parent.Kind() = SyntaxKind.XmlNameAttribute OrElse nextToken.Parent.Kind() = SyntaxKind.XmlDeclaration)) Then
					flag = False
				ElseIf (token.Kind() = SyntaxKind.DoubleQuoteToken OrElse nextToken.Kind() = SyntaxKind.DoubleQuoteToken) Then
					flag = False
				ElseIf (token.Kind() = SyntaxKind.AtToken AndAlso token.Parent.Kind() = SyntaxKind.XmlAttributeAccessExpression) Then
					flag = False
				ElseIf (token.Kind() = SyntaxKind.SingleQuoteToken OrElse nextToken.Kind() = SyntaxKind.SingleQuoteToken) Then
					flag = False
				ElseIf (nextToken.Kind() = SyntaxKind.QuestionToken) Then
					flag = False
				ElseIf (token.Kind() = SyntaxKind.HashToken AndAlso TypeOf token.Parent Is DirectiveTriviaSyntax) Then
					flag = False
				ElseIf (token.Parent.Kind() = SyntaxKind.RegionDirectiveTrivia AndAlso nextToken.Kind() = SyntaxKind.StringLiteralToken AndAlso [String].IsNullOrEmpty(nextToken.ValueText)) Then
					flag = False
				ElseIf (token.Kind() = SyntaxKind.XmlTextLiteralToken OrElse token.Kind() = SyntaxKind.DocumentationCommentLineBreakToken) Then
					flag = False
				ElseIf (token.Kind() <> SyntaxKind.DollarSignDoubleQuoteToken) Then
					flag = If(token.Kind() = SyntaxKind.InterpolatedStringTextToken OrElse nextToken.Kind() = SyntaxKind.InterpolatedStringTextToken, False, True)
				Else
					flag = False
				End If
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function NeedsSeparatorBetween(ByVal trivia As SyntaxTrivia) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = trivia.Kind()
			flag = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhitespaceTrivia OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LineContinuationTrivia) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List), False, Not SyntaxFacts.IsPreprocessorDirective(trivia.Kind()))
			Return flag
		End Function

		Friend Shared Function Normalize(Of TNode As SyntaxNode)(ByVal node As TNode, ByVal indentWhitespace As String, ByVal eolWhitespace As String, ByVal useElasticTrivia As Boolean, ByVal useDefaultCasing As Boolean) As SyntaxNode
			Dim syntaxNormalizer As Microsoft.CodeAnalysis.VisualBasic.Syntax.SyntaxNormalizer = New Microsoft.CodeAnalysis.VisualBasic.Syntax.SyntaxNormalizer(node.FullSpan, indentWhitespace, eolWhitespace, useElasticTrivia, useDefaultCasing)
			Dim tNode1 As TNode = DirectCast(syntaxNormalizer.Visit(DirectCast(node, SyntaxNode)), TNode)
			syntaxNormalizer.Free()
			Return DirectCast(tNode1, SyntaxNode)
		End Function

		Friend Shared Function Normalize(ByVal token As Microsoft.CodeAnalysis.SyntaxToken, ByVal indentWhitespace As String, ByVal eolWhitespace As String, ByVal useElasticTrivia As Boolean, ByVal useDefaultCasing As Boolean) As Microsoft.CodeAnalysis.SyntaxToken
			Dim syntaxNormalizer As Microsoft.CodeAnalysis.VisualBasic.Syntax.SyntaxNormalizer = New Microsoft.CodeAnalysis.VisualBasic.Syntax.SyntaxNormalizer(token.FullSpan, indentWhitespace, eolWhitespace, useElasticTrivia, useDefaultCasing)
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = syntaxNormalizer.VisitToken(token)
			syntaxNormalizer.Free()
			Return syntaxToken
		End Function

		Friend Shared Function Normalize(ByVal trivia As SyntaxTriviaList, ByVal indentWhitespace As String, ByVal eolWhitespace As String, ByVal useElasticTrivia As Boolean, ByVal useDefaultCasing As Boolean) As SyntaxTriviaList
			Dim syntaxNormalizer As Microsoft.CodeAnalysis.VisualBasic.Syntax.SyntaxNormalizer = New Microsoft.CodeAnalysis.VisualBasic.Syntax.SyntaxNormalizer(trivia.FullSpan, indentWhitespace, eolWhitespace, useElasticTrivia, useDefaultCasing)
			Dim syntaxTriviaLists As SyntaxTriviaList = syntaxNormalizer.RewriteTrivia(trivia, syntaxNormalizer.GetIndentationDepth(), False, False, False, 0, 0)
			syntaxNormalizer.Free()
			Return syntaxTriviaLists
		End Function

		Private Function RewriteTrivia(ByVal triviaList As SyntaxTriviaList, ByVal depth As Integer, ByVal isTrailing As Boolean, ByVal mustBeIndented As Boolean, ByVal mustHaveSeparator As Boolean, ByVal lineBreaksAfter As Integer, ByVal lineBreaksBefore As Integer) As SyntaxTriviaList
			Dim syntaxTriviaLists As SyntaxTriviaList
			Dim flag As Boolean
			Dim flag1 As Boolean
			Dim instance As ArrayBuilder(Of SyntaxTrivia) = ArrayBuilder(Of SyntaxTrivia).GetInstance()
			Try
				Dim num As Integer = lineBreaksBefore
				Dim num1 As Integer = 1
				Do
					instance.Add(Me.GetEndOfLine())
					Me._afterLineBreak = True
					Me._afterIndentation = False
					num1 = num1 + 1
				Loop While num1 <= num
				Dim enumerator As SyntaxTriviaList.Enumerator = triviaList.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As SyntaxTrivia = enumerator.Current
					If (current.Kind() = SyntaxKind.WhitespaceTrivia OrElse current.Kind() = SyntaxKind.EndOfLineTrivia OrElse current.Kind() = SyntaxKind.LineContinuationTrivia OrElse current.FullWidth = 0) Then
						Continue While
					End If
					Dim parent As SyntaxNode = current.Token.Parent
					If (current.Kind() = SyntaxKind.ColonTrivia AndAlso parent IsNot Nothing AndAlso parent.Kind() = SyntaxKind.LabelStatement OrElse parent IsNot Nothing AndAlso parent.Parent IsNot Nothing AndAlso parent.Parent.Kind() = SyntaxKind.CrefReference) Then
						flag = False
					ElseIf (instance.Count <= 0 OrElse Not Me.NeedsSeparatorBetween(instance.Last()) OrElse Me.EndsInLineBreak(instance.Last())) Then
						flag = If(instance.Count <> 0, False, isTrailing)
					Else
						flag = True
					End If
					Dim flag2 As Boolean = flag
					If (Me.NeedsLineBreakBefore(current)) Then
						flag1 = True
					Else
						flag1 = If(instance.Count <= 0, False, Me.NeedsLineBreakBetween(instance.Last(), current, isTrailing))
					End If
					If (flag1 AndAlso Not Me._afterLineBreak) Then
						instance.Add(Me.GetEndOfLine())
						Me._afterLineBreak = True
						Me._afterIndentation = False
					End If
					If (Me._afterLineBreak And Not isTrailing) Then
						If (Not Me._afterIndentation AndAlso Me.NeedsIndentAfterLineBreak(current)) Then
							instance.Add(Me.GetIndentation(Me.GetIndentationDepth(current)))
							Me._afterIndentation = True
						End If
					ElseIf (flag2) Then
						instance.Add(Me.GetSpace())
						Me._afterLineBreak = False
						Me._afterIndentation = False
					End If
					If (Not current.HasStructure) Then
						If (current.Kind() = SyntaxKind.DocumentationCommentExteriorTrivia) Then
							current = SyntaxFactory.DocumentationCommentExteriorTrivia(SyntaxFacts.GetText(SyntaxKind.DocumentationCommentExteriorTrivia))
						End If
						instance.Add(current)
					Else
						instance.Add(Me.VisitStructuredTrivia(current))
					End If
					If (Not Me.NeedsLineBreakAfter(current)) Then
						Me._afterLineBreak = Me.EndsInLineBreak(current)
					Else
						If (isTrailing) Then
							Continue While
						End If
						instance.Add(Me.GetEndOfLine())
						Me._afterLineBreak = True
						Me._afterIndentation = False
					End If
				End While
				If (lineBreaksAfter > 0) Then
					If (instance.Count > 0 AndAlso Me.EndsInLineBreak(instance.Last())) Then
						lineBreaksAfter = lineBreaksAfter - 1
					End If
					Dim num2 As Integer = lineBreaksAfter - 1
					For i As Integer = 0 To num2
						instance.Add(Me.GetEndOfLine())
						Me._afterLineBreak = True
						Me._afterIndentation = False
					Next

				ElseIf (mustHaveSeparator) Then
					instance.Add(Me.GetSpace())
					Me._afterLineBreak = False
					Me._afterIndentation = False
				End If
				If (mustBeIndented) Then
					instance.Add(Me.GetIndentation(depth))
					Me._afterIndentation = True
					Me._afterLineBreak = False
				End If
				If (instance.Count <> 0) Then
					syntaxTriviaLists = If(instance.Count <> 1, SyntaxFactory.TriviaList(instance), SyntaxFactory.TriviaList(instance.First()))
				Else
					syntaxTriviaLists = If(Not Me._useElasticTrivia, New SyntaxTriviaList(), SyntaxFactory.TriviaList(SyntaxFactory.ElasticMarker))
				End If
			Finally
				instance.Free()
			End Try
			Return syntaxTriviaLists
		End Function

		Public Overrides Function VisitAccessorBlock(ByVal node As AccessorBlockSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.BlockStatement.GetLastToken(False, False, False, False), 1)
			Me.AddLinebreaksAfterElementsIfNeeded(Of StatementSyntax)(node.Statements, 1, 1)
			Me.MarkLastStatementIfNeeded(Of StatementSyntax)(node.Statements)
			Return MyBase.VisitAccessorBlock(node)
		End Function

		Public Overrides Function VisitAccessorStatement(ByVal node As AccessorStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitAccessorStatement(node)
			Me._indentationDepth = Me._indentationDepth + 1
			Return syntaxNode
		End Function

		Public Overrides Function VisitAttributeList(ByVal node As AttributeListSyntax) As SyntaxNode
			If (node.Parent Is Nothing OrElse node.Parent.Kind() <> SyntaxKind.Parameter AndAlso node.Parent.Kind() <> SyntaxKind.SimpleAsClause) Then
				Me.AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(False, False, False, False), 1)
			End If
			Return MyBase.VisitAttributeList(node)
		End Function

		Public Overrides Function VisitBadDirectiveTrivia(ByVal node As BadDirectiveTriviaSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(False, False, False, False), 1)
			Return MyBase.VisitBadDirectiveTrivia(node)
		End Function

		Public Overrides Function VisitCaseBlock(ByVal node As CaseBlockSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.CaseStatement.GetLastToken(False, False, False, False), 1)
			Me.AddLinebreaksAfterElementsIfNeeded(Of StatementSyntax)(node.Statements, 1, 1)
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitCaseBlock(node)
			Me._indentationDepth = Me._indentationDepth - 1
			Return syntaxNode
		End Function

		Public Overrides Function VisitCaseStatement(ByVal node As CaseStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitCaseStatement(node)
			Me._indentationDepth = Me._indentationDepth + 1
			Return syntaxNode
		End Function

		Public Overrides Function VisitCatchBlock(ByVal node As CatchBlockSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.CatchStatement.GetLastToken(False, False, False, False), 1)
			Me.AddLinebreaksAfterElementsIfNeeded(Of StatementSyntax)(node.Statements, 1, 1)
			Return MyBase.VisitCatchBlock(node)
		End Function

		Public Overrides Function VisitCatchStatement(ByVal node As CatchStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Me._indentationDepth = Me._indentationDepth - 1
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitCatchStatement(node)
			Me._indentationDepth = Me._indentationDepth + 1
			Return syntaxNode
		End Function

		Public Overrides Function VisitClassBlock(ByVal node As ClassBlockSyntax) As SyntaxNode
			Me.VisitTypeBlockSyntax(node)
			Return MyBase.VisitClassBlock(node)
		End Function

		Public Overrides Function VisitClassStatement(ByVal node As ClassStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitClassStatement(node)
			Me._indentationDepth = Me._indentationDepth + 1
			Return syntaxNode
		End Function

		Public Overrides Function VisitCompilationUnit(ByVal node As CompilationUnitSyntax) As SyntaxNode
			Dim flag As Boolean = node.[Imports].Any()
			Dim flag1 As Boolean = node.Members.Any()
			Dim flag2 As Boolean = node.Attributes.Any()
			If (flag OrElse flag2 OrElse flag1) Then
				Me.AddLinebreaksAfterElementsIfNeeded(Of OptionStatementSyntax)(node.Options, 1, 2)
			Else
				Me.AddLinebreaksAfterElementsIfNeeded(Of OptionStatementSyntax)(node.Options, 1, 1)
			End If
			If (flag2 OrElse flag1) Then
				Me.AddLinebreaksAfterElementsIfNeeded(Of ImportsStatementSyntax)(node.[Imports], 1, 2)
			Else
				Me.AddLinebreaksAfterElementsIfNeeded(Of ImportsStatementSyntax)(node.[Imports], 1, 1)
			End If
			If (Not flag1) Then
				Me.AddLinebreaksAfterElementsIfNeeded(Of AttributesStatementSyntax)(node.Attributes, 1, 1)
			Else
				Me.AddLinebreaksAfterElementsIfNeeded(Of AttributesStatementSyntax)(node.Attributes, 1, 2)
			End If
			Me.AddLinebreaksAfterElementsIfNeeded(Of StatementSyntax)(node.Members, 2, 1)
			Return MyBase.VisitCompilationUnit(node)
		End Function

		Public Overrides Function VisitConstDirectiveTrivia(ByVal node As ConstDirectiveTriviaSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(False, False, False, False), 1)
			Return MyBase.VisitConstDirectiveTrivia(node)
		End Function

		Public Overrides Function VisitConstructorBlock(ByVal node As ConstructorBlockSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.BlockStatement.GetLastToken(False, False, False, False), 1)
			Me.AddLinebreaksAfterElementsIfNeeded(Of StatementSyntax)(node.Statements, 1, 1)
			Me.MarkLastStatementIfNeeded(Of StatementSyntax)(node.Statements)
			Return MyBase.VisitConstructorBlock(node)
		End Function

		Public Overrides Function VisitDisableWarningDirectiveTrivia(ByVal node As DisableWarningDirectiveTriviaSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(False, False, False, False), 1)
			Return MyBase.VisitDisableWarningDirectiveTrivia(node)
		End Function

		Public Overrides Function VisitDoLoopBlock(ByVal node As DoLoopBlockSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.DoStatement.GetLastToken(False, False, False, False), 1)
			Me.AddLinebreaksAfterElementsIfNeeded(Of StatementSyntax)(node.Statements, 1, 1)
			Me.MarkLastStatementIfNeeded(Of StatementSyntax)(node.Statements)
			If (Not Me._lastStatementsInBlocks.Contains(node)) Then
				Me.AddLinebreaksAfterTokenIfNeeded(node.LoopStatement.GetLastToken(False, False, False, False), 2)
			Else
				Me.AddLinebreaksAfterTokenIfNeeded(node.LoopStatement.GetLastToken(False, False, False, False), 1)
			End If
			Return MyBase.VisitDoLoopBlock(node)
		End Function

		Public Overrides Function VisitDoStatement(ByVal node As DoStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitDoStatement(node)
			Me._indentationDepth = Me._indentationDepth + 1
			Return syntaxNode
		End Function

		Public Overrides Function VisitElseBlock(ByVal node As ElseBlockSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.ElseStatement.GetLastToken(False, False, False, False), 1)
			Me.AddLinebreaksAfterElementsIfNeeded(Of StatementSyntax)(node.Statements, 1, 1)
			Me.MarkLastStatementIfNeeded(Of StatementSyntax)(node.Statements)
			Return MyBase.VisitElseBlock(node)
		End Function

		Public Overrides Function VisitElseDirectiveTrivia(ByVal node As ElseDirectiveTriviaSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(False, False, False, False), 1)
			Return MyBase.VisitElseDirectiveTrivia(node)
		End Function

		Public Overrides Function VisitElseIfBlock(ByVal node As ElseIfBlockSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.ElseIfStatement.GetLastToken(False, False, False, False), 1)
			Me.AddLinebreaksAfterElementsIfNeeded(Of StatementSyntax)(node.Statements, 1, 1)
			Me.MarkLastStatementIfNeeded(Of StatementSyntax)(node.Statements)
			Return MyBase.VisitElseIfBlock(node)
		End Function

		Public Overrides Function VisitElseIfStatement(ByVal node As ElseIfStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Me._indentationDepth = Me._indentationDepth - 1
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitElseIfStatement(node)
			Me._indentationDepth = Me._indentationDepth + 1
			Return syntaxNode
		End Function

		Public Overrides Function VisitElseStatement(ByVal node As ElseStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Me._indentationDepth = Me._indentationDepth - 1
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitElseStatement(node)
			Me._indentationDepth = Me._indentationDepth + 1
			Return syntaxNode
		End Function

		Public Overrides Function VisitEnableWarningDirectiveTrivia(ByVal node As EnableWarningDirectiveTriviaSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(False, False, False, False), 1)
			Return MyBase.VisitEnableWarningDirectiveTrivia(node)
		End Function

		Public Overrides Function VisitEndBlockStatement(ByVal node As EndBlockStatementSyntax) As SyntaxNode
			Me._indentationDepth = Me._indentationDepth - 1
			Return MyBase.VisitEndBlockStatement(node)
		End Function

		Public Overrides Function VisitEndExternalSourceDirectiveTrivia(ByVal node As EndExternalSourceDirectiveTriviaSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(False, False, False, False), 1)
			Return MyBase.VisitEndExternalSourceDirectiveTrivia(node)
		End Function

		Public Overrides Function VisitEndIfDirectiveTrivia(ByVal node As EndIfDirectiveTriviaSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(False, False, False, False), 1)
			Return MyBase.VisitEndIfDirectiveTrivia(node)
		End Function

		Public Overrides Function VisitEndRegionDirectiveTrivia(ByVal node As EndRegionDirectiveTriviaSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(False, False, False, False), 1)
			Return MyBase.VisitEndRegionDirectiveTrivia(node)
		End Function

		Public Overrides Function VisitEnumBlock(ByVal node As EnumBlockSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.EnumStatement.GetLastToken(False, False, False, False), 1)
			Me.AddLinebreaksAfterElementsIfNeeded(Of StatementSyntax)(node.Members, 1, 1)
			Return MyBase.VisitEnumBlock(node)
		End Function

		Public Overrides Function VisitEnumStatement(ByVal node As EnumStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitEnumStatement(node)
			Me._indentationDepth = Me._indentationDepth + 1
			Return syntaxNode
		End Function

		Public Overrides Function VisitEventBlock(ByVal node As EventBlockSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.EventStatement.GetLastToken(False, False, False, False), 1)
			Me.AddLinebreaksAfterElementsIfNeeded(Of AccessorBlockSyntax)(node.Accessors, 2, 1)
			Return MyBase.VisitEventBlock(node)
		End Function

		Public Overrides Function VisitEventStatement(ByVal node As EventStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitEventStatement(node)
			If (node.Parent IsNot Nothing AndAlso node.Parent.Kind() = SyntaxKind.EventBlock) Then
				Me._indentationDepth = Me._indentationDepth + 1
			End If
			Return syntaxNode
		End Function

		Public Overrides Function VisitExternalChecksumDirectiveTrivia(ByVal node As ExternalChecksumDirectiveTriviaSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(False, False, False, False), 1)
			Return MyBase.VisitExternalChecksumDirectiveTrivia(node)
		End Function

		Public Overrides Function VisitExternalSourceDirectiveTrivia(ByVal node As ExternalSourceDirectiveTriviaSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(False, False, False, False), 1)
			Return MyBase.VisitExternalSourceDirectiveTrivia(node)
		End Function

		Public Overrides Function VisitFinallyBlock(ByVal node As FinallyBlockSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.FinallyStatement.GetLastToken(False, False, False, False), 1)
			Me.AddLinebreaksAfterElementsIfNeeded(Of StatementSyntax)(node.Statements, 1, 1)
			Me.MarkLastStatementIfNeeded(Of StatementSyntax)(node.Statements)
			Return MyBase.VisitFinallyBlock(node)
		End Function

		Public Overrides Function VisitFinallyStatement(ByVal node As FinallyStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Me._indentationDepth = Me._indentationDepth - 1
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitFinallyStatement(node)
			Me._indentationDepth = Me._indentationDepth + 1
			Return syntaxNode
		End Function

		Public Overrides Function VisitForBlock(ByVal node As ForBlockSyntax) As SyntaxNode
			Me.VisitForOrForEachBlock(node)
			Return MyBase.VisitForBlock(node)
		End Function

		Public Overrides Function VisitForEachBlock(ByVal node As ForEachBlockSyntax) As SyntaxNode
			Me.VisitForOrForEachBlock(node)
			Return MyBase.VisitForEachBlock(node)
		End Function

		Public Overrides Function VisitForEachStatement(ByVal node As ForEachStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitForEachStatement(node)
			Me._indentationDepth = Me._indentationDepth + 1
			Return syntaxNode
		End Function

		Private Sub VisitForOrForEachBlock(ByVal node As ForOrForEachBlockSyntax)
			Me.AddLinebreaksAfterTokenIfNeeded(node.ForOrForEachStatement.GetLastToken(False, False, False, False), 1)
			Me.AddLinebreaksAfterElementsIfNeeded(Of StatementSyntax)(node.Statements, 1, 1)
			Me.MarkLastStatementIfNeeded(Of StatementSyntax)(node.Statements)
			If (node.NextStatement IsNot Nothing) Then
				If (Not Me._lastStatementsInBlocks.Contains(node)) Then
					Me.AddLinebreaksAfterTokenIfNeeded(node.NextStatement.GetLastToken(False, False, False, False), 2)
					Return
				End If
				Me.AddLinebreaksAfterTokenIfNeeded(node.NextStatement.GetLastToken(False, False, False, False), 1)
			End If
		End Sub

		Public Overrides Function VisitForStatement(ByVal node As ForStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitForStatement(node)
			Me._indentationDepth = Me._indentationDepth + 1
			Return syntaxNode
		End Function

		Public Overrides Function VisitIfDirectiveTrivia(ByVal node As IfDirectiveTriviaSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(False, False, False, False), 1)
			Return MyBase.VisitIfDirectiveTrivia(node)
		End Function

		Public Overrides Function VisitIfStatement(ByVal node As IfStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitIfStatement(node)
			Me._indentationDepth = Me._indentationDepth + 1
			Return syntaxNode
		End Function

		Public Overrides Function VisitInterfaceBlock(ByVal node As InterfaceBlockSyntax) As SyntaxNode
			Me.VisitTypeBlockSyntax(node)
			Return MyBase.VisitInterfaceBlock(node)
		End Function

		Public Overrides Function VisitInterfaceStatement(ByVal node As InterfaceStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitInterfaceStatement(node)
			Me._indentationDepth = Me._indentationDepth + 1
			Return syntaxNode
		End Function

		Public Overrides Function VisitLabelStatement(ByVal node As LabelStatementSyntax) As SyntaxNode
			Dim num As Integer = Me._indentationDepth
			Me._indentationDepth = 0
			Me._indentationDepth = num
			Return MyBase.VisitLabelStatement(node)
		End Function

		Public Overrides Function VisitLoopStatement(ByVal node As LoopStatementSyntax) As SyntaxNode
			Me._indentationDepth = Me._indentationDepth - 1
			Return MyBase.VisitLoopStatement(node)
		End Function

		Public Overrides Function VisitMethodBlock(ByVal node As MethodBlockSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.BlockStatement.GetLastToken(False, False, False, False), 1)
			Me.AddLinebreaksAfterElementsIfNeeded(Of StatementSyntax)(node.Statements, 1, 1)
			Me.MarkLastStatementIfNeeded(Of StatementSyntax)(node.Statements)
			Return MyBase.VisitMethodBlock(node)
		End Function

		Public Overrides Function VisitMethodStatement(ByVal node As MethodStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitMethodStatement(node)
			If (node.Parent IsNot Nothing AndAlso (node.Parent.Kind() = SyntaxKind.SubBlock OrElse node.Parent.Kind() = SyntaxKind.FunctionBlock)) Then
				Me._indentationDepth = Me._indentationDepth + 1
			End If
			Return syntaxNode
		End Function

		Public Overrides Function VisitModuleBlock(ByVal node As ModuleBlockSyntax) As SyntaxNode
			Me.VisitTypeBlockSyntax(node)
			Return MyBase.VisitModuleBlock(node)
		End Function

		Public Overrides Function VisitModuleStatement(ByVal node As ModuleStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitModuleStatement(node)
			Me._indentationDepth = Me._indentationDepth + 1
			Return syntaxNode
		End Function

		Public Overrides Function VisitMultiLineIfBlock(ByVal node As MultiLineIfBlockSyntax) As SyntaxNode
			Dim ifStatement As VisualBasicSyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.IfStatement.GetLastToken(False, False, False, False), 1)
			Me.AddLinebreaksAfterElementsIfNeeded(Of StatementSyntax)(node.Statements, 1, 1)
			Me.MarkLastStatementIfNeeded(Of StatementSyntax)(node.Statements)
			If (Not node.Statements.Any()) Then
				ifStatement = node.IfStatement
			Else
				ifStatement = node.Statements.Last()
			End If
			Dim enumerator As SyntaxList(Of ElseIfBlockSyntax).Enumerator = node.ElseIfBlocks.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As ElseIfBlockSyntax = enumerator.Current
				Me.AddLinebreaksAfterTokenIfNeeded(ifStatement.GetLastToken(False, False, False, False), 1)
				ifStatement = current
			End While
			If (node.ElseBlock IsNot Nothing) Then
				Me.AddLinebreaksAfterTokenIfNeeded(ifStatement.GetLastToken(False, False, False, False), 1)
			End If
			If (Me._lastStatementsInBlocks.Contains(node)) Then
				Me.AddLinebreaksAfterTokenIfNeeded(node.EndIfStatement.GetLastToken(False, False, False, False), 1)
			Else
				Me.AddLinebreaksAfterTokenIfNeeded(node.EndIfStatement.GetLastToken(False, False, False, False), 2)
			End If
			Return MyBase.VisitMultiLineIfBlock(node)
		End Function

		Public Overrides Function VisitMultiLineLambdaExpression(ByVal node As MultiLineLambdaExpressionSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.SubOrFunctionHeader.GetLastToken(False, False, False, False), 1)
			Me.AddLinebreaksAfterElementsIfNeeded(Of StatementSyntax)(node.Statements, 1, 1)
			Me.MarkLastStatementIfNeeded(Of StatementSyntax)(node.Statements)
			Me._indentationDepth = Me._indentationDepth + 1
			Return MyBase.VisitMultiLineLambdaExpression(node)
		End Function

		Public Overrides Function VisitNamespaceBlock(ByVal node As NamespaceBlockSyntax) As SyntaxNode
			If (node.Members.Count <= 0) Then
				Me.AddLinebreaksAfterTokenIfNeeded(node.NamespaceStatement.GetLastToken(False, False, False, False), 1)
			Else
				If (node.Members(0).Kind() = SyntaxKind.NamespaceBlock) Then
					Me.AddLinebreaksAfterTokenIfNeeded(node.NamespaceStatement.GetLastToken(False, False, False, False), 1)
				Else
					Me.AddLinebreaksAfterTokenIfNeeded(node.NamespaceStatement.GetLastToken(False, False, False, False), 2)
				End If
				Me.AddLinebreaksAfterElementsIfNeeded(Of StatementSyntax)(node.Members, 2, 1)
			End If
			Return MyBase.VisitNamespaceBlock(node)
		End Function

		Public Overrides Function VisitNamespaceStatement(ByVal node As NamespaceStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitNamespaceStatement(node)
			Me._indentationDepth = Me._indentationDepth + 1
			Return syntaxNode
		End Function

		Public Overrides Function VisitNextStatement(ByVal node As NextStatementSyntax) As SyntaxNode
			Dim count As Integer = node.ControlVariables.Count
			If (count = 0) Then
				count = 1
			End If
			Me._indentationDepth -= count
			Return MyBase.VisitNextStatement(node)
		End Function

		Public Overrides Function VisitOperatorBlock(ByVal node As OperatorBlockSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.BlockStatement.GetLastToken(False, False, False, False), 1)
			Me.AddLinebreaksAfterElementsIfNeeded(Of StatementSyntax)(node.Statements, 1, 1)
			Me.MarkLastStatementIfNeeded(Of StatementSyntax)(node.Statements)
			Return MyBase.VisitOperatorBlock(node)
		End Function

		Public Overrides Function VisitOperatorStatement(ByVal node As OperatorStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitOperatorStatement(node)
			Me._indentationDepth = Me._indentationDepth + 1
			Return syntaxNode
		End Function

		Public Overrides Function VisitPropertyBlock(ByVal node As PropertyBlockSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.PropertyStatement.GetLastToken(False, False, False, False), 1)
			Me.AddLinebreaksAfterElementsIfNeeded(Of AccessorBlockSyntax)(node.Accessors, 2, 1)
			Return MyBase.VisitPropertyBlock(node)
		End Function

		Public Overrides Function VisitPropertyStatement(ByVal node As PropertyStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitPropertyStatement(node)
			If (node.Parent IsNot Nothing AndAlso node.Parent.Kind() = SyntaxKind.PropertyBlock) Then
				Me._indentationDepth = Me._indentationDepth + 1
			End If
			Return syntaxNode
		End Function

		Public Overrides Function VisitReferenceDirectiveTrivia(ByVal node As ReferenceDirectiveTriviaSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(False, False, False, False), 1)
			Return MyBase.VisitReferenceDirectiveTrivia(node)
		End Function

		Public Overrides Function VisitRegionDirectiveTrivia(ByVal node As RegionDirectiveTriviaSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.GetLastToken(False, False, False, False), 1)
			Return MyBase.VisitRegionDirectiveTrivia(node)
		End Function

		Public Overrides Function VisitSelectBlock(ByVal node As SelectBlockSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.SelectStatement.GetLastToken(False, False, False, False), 1)
			If (Me._lastStatementsInBlocks.Contains(node)) Then
				Me.AddLinebreaksAfterTokenIfNeeded(node.EndSelectStatement.GetLastToken(False, False, False, False), 1)
			Else
				Me.AddLinebreaksAfterTokenIfNeeded(node.EndSelectStatement.GetLastToken(False, False, False, False), 2)
			End If
			Return MyBase.VisitSelectBlock(node)
		End Function

		Public Overrides Function VisitSelectStatement(ByVal node As SelectStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitSelectStatement(node)
			Me._indentationDepth = Me._indentationDepth + 1
			Return syntaxNode
		End Function

		Public Overrides Function VisitStructureBlock(ByVal node As StructureBlockSyntax) As SyntaxNode
			Me.VisitTypeBlockSyntax(node)
			Return MyBase.VisitStructureBlock(node)
		End Function

		Private Function VisitStructuredTrivia(ByVal trivia As SyntaxTrivia) As SyntaxTrivia
			Dim flag As Boolean = Me._isInStructuredTrivia
			Me._isInStructuredTrivia = True
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me._previousToken
			Me._previousToken = New Microsoft.CodeAnalysis.SyntaxToken()
			Me._isInStructuredTrivia = flag
			Me._previousToken = syntaxToken
			Return Me.VisitTrivia(trivia)
		End Function

		Public Overrides Function VisitStructureStatement(ByVal node As StructureStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitStructureStatement(node)
			Me._indentationDepth = Me._indentationDepth + 1
			Return syntaxNode
		End Function

		Public Overrides Function VisitSubNewStatement(ByVal node As SubNewStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitSubNewStatement(node)
			Me._indentationDepth = Me._indentationDepth + 1
			Return syntaxNode
		End Function

		Public Overrides Function VisitSyncLockBlock(ByVal node As SyncLockBlockSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.SyncLockStatement.GetLastToken(False, False, False, False), 1)
			Me.AddLinebreaksAfterElementsIfNeeded(Of StatementSyntax)(node.Statements, 1, 1)
			If (Not Me._lastStatementsInBlocks.Contains(node)) Then
				Me.AddLinebreaksAfterTokenIfNeeded(node.EndSyncLockStatement.GetLastToken(False, False, False, False), 2)
			Else
				Me.AddLinebreaksAfterTokenIfNeeded(node.EndSyncLockStatement.GetLastToken(False, False, False, False), 1)
			End If
			Return MyBase.VisitSyncLockBlock(node)
		End Function

		Public Overrides Function VisitSyncLockStatement(ByVal node As SyncLockStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitSyncLockStatement(node)
			Me._indentationDepth = Me._indentationDepth + 1
			Return syntaxNode
		End Function

		Public Overrides Function VisitToken(ByVal token As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken
			Dim syntaxToken1 As Microsoft.CodeAnalysis.SyntaxToken
			If (token.Kind() <> SyntaxKind.None) Then
				Try
					syntaxToken1 = If(Not Me._useDefaultCasing OrElse Not token.IsKeyword(), token, SyntaxFactory.Token(token.Kind(), Nothing))
					Dim indentationDepth As Integer = Me.GetIndentationDepth()
					Dim num As Integer = Me.LineBreaksBetween(Me._previousToken, token)
					Dim flag As Boolean = num > 0
					If (num > 0 AndAlso Me.IsLastTokenOnLine(Me._previousToken)) Then
						num = num - 1
					End If
					syntaxToken1 = syntaxToken1.WithLeadingTrivia(Me.RewriteTrivia(token.LeadingTrivia, indentationDepth, False, flag, False, 0, num))
					Dim nextRelevantToken As Microsoft.CodeAnalysis.SyntaxToken = Me.GetNextRelevantToken(token)
					Me._afterIndentation = False
					Dim num1 As Integer = If(Me.LineBreaksBetween(token, nextRelevantToken) > 0, 1, 0)
					Dim flag1 As Boolean = If(num1 > 0, False, Me.NeedsSeparator(token, nextRelevantToken))
					syntaxToken1 = syntaxToken1.WithTrailingTrivia(Me.RewriteTrivia(token.TrailingTrivia, 0, True, False, flag1, num1, 0))
					If (syntaxToken1.Kind() = SyntaxKind.DocumentationCommentLineBreakToken) Then
						Me._afterLineBreak = True
					ElseIf (syntaxToken1.Kind() = SyntaxKind.XmlTextLiteralToken AndAlso syntaxToken1.TrailingTrivia.Count = 0 AndAlso SyntaxNormalizer.IsNewLineChar(syntaxToken1.ValueText.Last())) Then
						Me._afterLineBreak = True
					End If
					syntaxToken = syntaxToken1
				Finally
					Me._previousToken = token
				End Try
			Else
				syntaxToken = token
			End If
			Return syntaxToken
		End Function

		Public Overrides Function VisitTryBlock(ByVal node As TryBlockSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.TryStatement.GetLastToken(False, False, False, False), 1)
			Me.AddLinebreaksAfterElementsIfNeeded(Of StatementSyntax)(node.Statements, 1, 1)
			Me.MarkLastStatementIfNeeded(Of StatementSyntax)(node.Statements)
			If (Me._lastStatementsInBlocks.Contains(node)) Then
				Me.AddLinebreaksAfterTokenIfNeeded(node.EndTryStatement.GetLastToken(False, False, False, False), 1)
			Else
				Me.AddLinebreaksAfterTokenIfNeeded(node.EndTryStatement.GetLastToken(False, False, False, False), 2)
			End If
			Return MyBase.VisitTryBlock(node)
		End Function

		Public Overrides Function VisitTryStatement(ByVal node As TryStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitTryStatement(node)
			Me._indentationDepth = Me._indentationDepth + 1
			Return syntaxNode
		End Function

		Private Sub VisitTypeBlockSyntax(ByVal node As TypeBlockSyntax)
			Dim count As Boolean = node.[Implements].Count > 0
			If (node.[Inherits].Count > 0 OrElse count OrElse node.Members.Count <= 0) Then
				Me.AddLinebreaksAfterTokenIfNeeded(node.BlockStatement.GetLastToken(False, False, False, False), 1)
			Else
				Me.AddLinebreaksAfterTokenIfNeeded(node.BlockStatement.GetLastToken(False, False, False, False), 2)
			End If
			If (Not count) Then
				Me.AddLinebreaksAfterElementsIfNeeded(Of InheritsStatementSyntax)(node.[Inherits], 1, 2)
			Else
				Me.AddLinebreaksAfterElementsIfNeeded(Of InheritsStatementSyntax)(node.[Inherits], 1, 1)
			End If
			Me.AddLinebreaksAfterElementsIfNeeded(Of ImplementsStatementSyntax)(node.[Implements], 1, 2)
			If (node.Kind() = SyntaxKind.InterfaceBlock) Then
				Me.AddLinebreaksAfterElementsIfNeeded(Of StatementSyntax)(node.Members, 2, 2)
				Return
			End If
			Me.AddLinebreaksAfterElementsIfNeeded(Of StatementSyntax)(node.Members, 2, 1)
		End Sub

		Public Overrides Function VisitUsingBlock(ByVal node As UsingBlockSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.UsingStatement.GetLastToken(False, False, False, False), 1)
			Me.AddLinebreaksAfterElementsIfNeeded(Of StatementSyntax)(node.Statements, 1, 1)
			Me.MarkLastStatementIfNeeded(Of StatementSyntax)(node.Statements)
			If (Me._lastStatementsInBlocks.Contains(node)) Then
				Me.AddLinebreaksAfterTokenIfNeeded(node.EndUsingStatement.GetLastToken(False, False, False, False), 1)
			Else
				Me.AddLinebreaksAfterTokenIfNeeded(node.EndUsingStatement.GetLastToken(False, False, False, False), 2)
			End If
			Return MyBase.VisitUsingBlock(node)
		End Function

		Public Overrides Function VisitUsingStatement(ByVal node As UsingStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitUsingStatement(node)
			Me._indentationDepth = Me._indentationDepth + 1
			Return syntaxNode
		End Function

		Public Overrides Function VisitWhileBlock(ByVal node As WhileBlockSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.WhileStatement.GetLastToken(False, False, False, False), 1)
			Me.AddLinebreaksAfterElementsIfNeeded(Of StatementSyntax)(node.Statements, 1, 1)
			Me.MarkLastStatementIfNeeded(Of StatementSyntax)(node.Statements)
			If (Not Me._lastStatementsInBlocks.Contains(node)) Then
				Me.AddLinebreaksAfterTokenIfNeeded(node.EndWhileStatement.GetLastToken(False, False, False, False), 2)
			End If
			Return MyBase.VisitWhileBlock(node)
		End Function

		Public Overrides Function VisitWhileStatement(ByVal node As WhileStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitWhileStatement(node)
			Me._indentationDepth = Me._indentationDepth + 1
			Return syntaxNode
		End Function

		Public Overrides Function VisitWithBlock(ByVal node As WithBlockSyntax) As SyntaxNode
			Me.AddLinebreaksAfterTokenIfNeeded(node.WithStatement.GetLastToken(False, False, False, False), 1)
			Me.AddLinebreaksAfterElementsIfNeeded(Of StatementSyntax)(node.Statements, 1, 1)
			Me.MarkLastStatementIfNeeded(Of StatementSyntax)(node.Statements)
			Return MyBase.VisitWithBlock(node)
		End Function

		Public Overrides Function VisitWithStatement(ByVal node As WithStatementSyntax) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = MyBase.VisitWithStatement(node)
			Me._indentationDepth = Me._indentationDepth + 1
			Return syntaxNode
		End Function
	End Class
End Namespace