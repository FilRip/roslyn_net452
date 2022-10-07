Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public MustInherit Class DirectiveTriviaSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.StructuredTriviaSyntax
		Private ReadOnly Shared s_hasDirectivesFunction As Func(Of Microsoft.CodeAnalysis.SyntaxToken, Boolean)

		Public ReadOnly Property HashToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return Me.GetHashTokenCore()
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax.s_hasDirectivesFunction = Function(n As Microsoft.CodeAnalysis.SyntaxToken) n.ContainsDirectives
		End Sub

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Overridable Function GetHashTokenCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax)._hashToken, MyBase.Position, 0)
		End Function

		Public Function GetNextDirective(Optional ByVal predicate As Func(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax, Boolean) = Nothing) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
			Dim directiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
			Dim token As Microsoft.CodeAnalysis.SyntaxToken = MyBase.ParentTrivia.Token
			Dim flag As Boolean = False
			While True
				If (token.Kind() <> SyntaxKind.None) Then
					Dim enumerator As SyntaxTriviaList.Enumerator = token.LeadingTrivia.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Microsoft.CodeAnalysis.SyntaxTrivia = enumerator.Current
						If (Not flag) Then
							If (current.UnderlyingNode <> MyBase.Green) Then
								Continue While
							End If
							flag = True
						Else
							If (Not current.IsDirective) Then
								Continue While
							End If
							Dim [structure] As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax = DirectCast(current.GetStructure(), Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax)
							If (predicate IsNot Nothing AndAlso Not predicate([structure])) Then
								Continue While
							End If
							directiveTriviaSyntax = [structure]
							Return directiveTriviaSyntax
						End If
					End While
					token = token.GetNextToken(Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax.s_hasDirectivesFunction, Nothing)
				Else
					directiveTriviaSyntax = Nothing
					Exit While
				End If
			End While
			Return directiveTriviaSyntax
		End Function

		Private Function GetNextPossiblyRelatedDirective() As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
			Dim directiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
			Dim nextDirective As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax = Me
			While True
			Label1:
				If (nextDirective IsNot Nothing) Then
					nextDirective = nextDirective.GetNextDirective(Nothing)
					If (nextDirective Is Nothing) Then
						Exit While
					End If
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = nextDirective.Kind()
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IfDirectiveTrivia) Then
						While nextDirective IsNot Nothing
							If (nextDirective.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfDirectiveTrivia) Then
								nextDirective = nextDirective.GetNextRelatedDirective()
							Else
								GoTo Label1
							End If
						End While
					ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RegionDirectiveTrivia) Then
						While nextDirective IsNot Nothing
							If (nextDirective.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRegionDirectiveTrivia) Then
								nextDirective = nextDirective.GetNextRelatedDirective()
							Else
								GoTo Label1
							End If
						End While
					Else
						Exit While
					End If
				Else
					directiveTriviaSyntax = Nothing
					Return directiveTriviaSyntax
				End If
			End While
			directiveTriviaSyntax = nextDirective
			Return directiveTriviaSyntax
		End Function

		Private Function GetNextRelatedDirective() As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
			Dim directiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
			Dim nextPossiblyRelatedDirective As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax = Me
			Select Case nextPossiblyRelatedDirective.Kind()
				Case SyntaxKind.IfDirectiveTrivia
					While nextPossiblyRelatedDirective IsNot Nothing
						If (CUShort(nextPossiblyRelatedDirective.Kind()) - CUShort(SyntaxKind.ElseIfDirectiveTrivia) > CUShort(SyntaxKind.EmptyStatement)) Then
							nextPossiblyRelatedDirective = nextPossiblyRelatedDirective.GetNextPossiblyRelatedDirective()
						Else
							directiveTriviaSyntax = nextPossiblyRelatedDirective
							Return directiveTriviaSyntax
						End If
					End While
					GoTo Label0
				Case SyntaxKind.ElseIfDirectiveTrivia
					While nextPossiblyRelatedDirective IsNot Nothing
						If (CUShort(nextPossiblyRelatedDirective.Kind()) - CUShort(SyntaxKind.ElseDirectiveTrivia) > CUShort(SyntaxKind.List)) Then
							nextPossiblyRelatedDirective = nextPossiblyRelatedDirective.GetNextPossiblyRelatedDirective()
						Else
							directiveTriviaSyntax = nextPossiblyRelatedDirective
							Return directiveTriviaSyntax
						End If
					End While
					GoTo Label0
				Case SyntaxKind.ElseDirectiveTrivia
					While nextPossiblyRelatedDirective IsNot Nothing
						If (nextPossiblyRelatedDirective.Kind() <> SyntaxKind.EndIfDirectiveTrivia) Then
							nextPossiblyRelatedDirective = nextPossiblyRelatedDirective.GetNextPossiblyRelatedDirective()
						Else
							directiveTriviaSyntax = nextPossiblyRelatedDirective
							Return directiveTriviaSyntax
						End If
					End While
					GoTo Label0
				Case SyntaxKind.EndIfDirectiveTrivia
				Label0:
					directiveTriviaSyntax = Nothing
					Exit Select
				Case SyntaxKind.RegionDirectiveTrivia
					While nextPossiblyRelatedDirective IsNot Nothing
						If (nextPossiblyRelatedDirective.Kind() <> SyntaxKind.EndRegionDirectiveTrivia) Then
							nextPossiblyRelatedDirective = nextPossiblyRelatedDirective.GetNextPossiblyRelatedDirective()
						Else
							directiveTriviaSyntax = nextPossiblyRelatedDirective
							Return directiveTriviaSyntax
						End If
					End While
					GoTo Label0
				Case Else
					GoTo Label0
			End Select
			Return directiveTriviaSyntax
		End Function

		Public Function GetPreviousDirective(Optional ByVal predicate As Func(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax, Boolean) = Nothing) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
			Dim directiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
			Dim token As Microsoft.CodeAnalysis.SyntaxToken = MyBase.ParentTrivia.Token
			Dim flag As Boolean = False
			While True
				If (token.Kind() <> SyntaxKind.None) Then
					Dim enumerator As SyntaxTriviaList.Reversed.Enumerator = token.LeadingTrivia.Reverse().GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Microsoft.CodeAnalysis.SyntaxTrivia = enumerator.Current
						If (Not flag) Then
							If (current.UnderlyingNode <> MyBase.Green) Then
								Continue While
							End If
							flag = True
						Else
							If (Not current.IsDirective) Then
								Continue While
							End If
							Dim [structure] As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax = DirectCast(current.GetStructure(), Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax)
							If (predicate IsNot Nothing AndAlso Not predicate([structure])) Then
								Continue While
							End If
							directiveTriviaSyntax = [structure]
							Return directiveTriviaSyntax
						End If
					End While
					token = token.GetPreviousToken(Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax.s_hasDirectivesFunction, Nothing)
				Else
					directiveTriviaSyntax = Nothing
					Exit While
				End If
			End While
			Return directiveTriviaSyntax
		End Function

		Private Function GetPreviousPossiblyRelatedDirective() As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
			Dim directiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
			Dim previousDirective As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax = Me
			While True
			Label1:
				If (previousDirective IsNot Nothing) Then
					previousDirective = previousDirective.GetPreviousDirective(Nothing)
					If (previousDirective Is Nothing) Then
						Exit While
					End If
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = previousDirective.Kind()
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfDirectiveTrivia) Then
						While previousDirective IsNot Nothing
							If (previousDirective.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IfDirectiveTrivia) Then
								previousDirective = previousDirective.GetPreviousRelatedDirective()
							Else
								GoTo Label1
							End If
						End While
					ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRegionDirectiveTrivia) Then
						While previousDirective IsNot Nothing
							If (previousDirective.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RegionDirectiveTrivia) Then
								previousDirective = previousDirective.GetPreviousRelatedDirective()
							Else
								GoTo Label1
							End If
						End While
					Else
						Exit While
					End If
				Else
					directiveTriviaSyntax = Nothing
					Return directiveTriviaSyntax
				End If
			End While
			directiveTriviaSyntax = previousDirective
			Return directiveTriviaSyntax
		End Function

		Private Function GetPreviousRelatedDirective() As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
			Dim directiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
			Dim previousPossiblyRelatedDirective As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax = Me
			Select Case previousPossiblyRelatedDirective.Kind()
				Case SyntaxKind.ElseIfDirectiveTrivia
					While previousPossiblyRelatedDirective IsNot Nothing
						If (previousPossiblyRelatedDirective.Kind() <> SyntaxKind.IfDirectiveTrivia) Then
							previousPossiblyRelatedDirective = previousPossiblyRelatedDirective.GetPreviousPossiblyRelatedDirective()
						Else
							directiveTriviaSyntax = previousPossiblyRelatedDirective
							Return directiveTriviaSyntax
						End If
					End While
					GoTo Label0
				Case SyntaxKind.ElseDirectiveTrivia
					While previousPossiblyRelatedDirective IsNot Nothing
						If (CUShort(previousPossiblyRelatedDirective.Kind()) - CUShort(SyntaxKind.IfDirectiveTrivia) > CUShort(SyntaxKind.List)) Then
							previousPossiblyRelatedDirective = previousPossiblyRelatedDirective.GetPreviousPossiblyRelatedDirective()
						Else
							directiveTriviaSyntax = previousPossiblyRelatedDirective
							Return directiveTriviaSyntax
						End If
					End While
					GoTo Label0
				Case SyntaxKind.EndIfDirectiveTrivia
					While previousPossiblyRelatedDirective IsNot Nothing
						If (CUShort(previousPossiblyRelatedDirective.Kind()) - CUShort(SyntaxKind.IfDirectiveTrivia) > CUShort(SyntaxKind.EmptyStatement)) Then
							previousPossiblyRelatedDirective = previousPossiblyRelatedDirective.GetPreviousPossiblyRelatedDirective()
						Else
							directiveTriviaSyntax = previousPossiblyRelatedDirective
							Return directiveTriviaSyntax
						End If
					End While
					GoTo Label0
				Case SyntaxKind.RegionDirectiveTrivia
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.EventStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.SyncLockStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OpenParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.EventStatement Or SyntaxKind.OperatorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.ArrayRankSpecifier Or SyntaxKind.AttributeList Or SyntaxKind.ExitFunctionStatement Or SyntaxKind.ExitOperatorStatement Or SyntaxKind.ExitPropertyStatement Or SyntaxKind.ExitTryStatement Or SyntaxKind.ExitSelectStatement Or SyntaxKind.ExitWhileStatement Or SyntaxKind.ContinueWhileStatement Or SyntaxKind.ContinueDoStatement Or SyntaxKind.FinallyStatement Or SyntaxKind.ErrorStatement Or SyntaxKind.OnErrorGoToZeroStatement Or SyntaxKind.OnErrorGoToMinusOneStatement Or SyntaxKind.OnErrorGoToLabelStatement Or SyntaxKind.OnErrorResumeNextStatement Or SyntaxKind.SyncLockStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.StaticKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OffKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OutKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenParenToken Or SyntaxKind.CloseParenToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.SemicolonToken Or SyntaxKind.AsteriskToken Or SyntaxKind.GreaterThanGreaterThanToken Or SyntaxKind.LessThanLessThanEqualsToken Or SyntaxKind.GreaterThanGreaterThanEqualsToken Or SyntaxKind.QuestionToken Or SyntaxKind.DoubleQuoteToken Or SyntaxKind.StatementTerminatorToken Or SyntaxKind.EndOfFileToken Or SyntaxKind.EmptyToken Or SyntaxKind.DateLiteralToken Or SyntaxKind.StringLiteralToken Or SyntaxKind.CharacterLiteralToken Or SyntaxKind.SkippedTokensTrivia Or SyntaxKind.DocumentationCommentTrivia Or SyntaxKind.XmlCrefAttribute Or SyntaxKind.ConstDirectiveTrivia Or SyntaxKind.IfDirectiveTrivia Or SyntaxKind.ElseIfDirectiveTrivia Or SyntaxKind.ElseDirectiveTrivia Or SyntaxKind.EndIfDirectiveTrivia Or SyntaxKind.RegionDirectiveTrivia
				Label0:
					directiveTriviaSyntax = Nothing
					Exit Select
				Case SyntaxKind.EndRegionDirectiveTrivia
					While previousPossiblyRelatedDirective IsNot Nothing
						If (previousPossiblyRelatedDirective.Kind() <> SyntaxKind.RegionDirectiveTrivia) Then
							previousPossiblyRelatedDirective = previousPossiblyRelatedDirective.GetPreviousPossiblyRelatedDirective()
						Else
							directiveTriviaSyntax = previousPossiblyRelatedDirective
							Return directiveTriviaSyntax
						End If
					End While
					GoTo Label0
				Case Else
					GoTo Label0
			End Select
			Return directiveTriviaSyntax
		End Function

		Public Function GetRelatedDirectives() As List(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax)
			Dim directiveTriviaSyntaxes As List(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax) = New List(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax)()
			Me.GetRelatedDirectives(directiveTriviaSyntaxes)
			Return directiveTriviaSyntaxes
		End Function

		Private Sub GetRelatedDirectives(ByVal list As List(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax))
			list.Clear()
			Dim previousRelatedDirective As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax = Me.GetPreviousRelatedDirective()
			While previousRelatedDirective IsNot Nothing
				list.Add(previousRelatedDirective)
				previousRelatedDirective = previousRelatedDirective.GetPreviousRelatedDirective()
			End While
			list.Reverse()
			list.Add(Me)
			Dim nextRelatedDirective As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax = Me.GetNextRelatedDirective()
			While nextRelatedDirective IsNot Nothing
				list.Add(nextRelatedDirective)
				nextRelatedDirective = nextRelatedDirective.GetNextRelatedDirective()
			End While
		End Sub

		Public Function WithHashToken(ByVal hashToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
			Return Me.WithHashTokenCore(hashToken)
		End Function

		Friend MustOverride Function WithHashTokenCore(ByVal hashToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax
	End Class
End Namespace