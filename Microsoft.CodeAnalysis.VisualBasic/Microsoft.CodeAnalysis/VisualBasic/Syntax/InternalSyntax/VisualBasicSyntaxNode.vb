Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	<DebuggerDisplay("{GetDebuggerDisplay(), nq}")>
	Friend MustInherit Class VisualBasicSyntaxNode
		Inherits GreenNode
		Private ReadOnly Shared s_structuresTable As ConditionalWeakTable(Of SyntaxNode, Dictionary(Of Microsoft.CodeAnalysis.SyntaxTrivia, SyntaxNode))

		Protected Property _slotCount As Integer
			Get
				Return MyBase.SlotCount
			End Get
			Set(ByVal value As Integer)
				MyBase.SlotCount = value
			End Set
		End Property

		Friend ReadOnly Property ContextualKind As SyntaxKind
			Get
				Return DirectCast(CUShort(Me.RawContextualKind), SyntaxKind)
			End Get
		End Property

		Public Overrides ReadOnly Property IsDirective As Boolean
			Get
				Return TypeOf Me Is Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax
			End Get
		End Property

		Public Overrides ReadOnly Property IsDocumentationCommentTrivia As Boolean
			Get
				Return Me.Kind = SyntaxKind.DocumentationCommentTrivia
			End Get
		End Property

		Public Overrides ReadOnly Property IsSkippedTokensTrivia As Boolean
			Get
				Return Me.Kind = SyntaxKind.SkippedTokensTrivia
			End Get
		End Property

		Public Overrides ReadOnly Property IsStructuredTrivia As Boolean
			Get
				Return TypeOf Me Is Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructuredTriviaSyntax
			End Get
		End Property

		Friend ReadOnly Property Kind As SyntaxKind
			Get
				Return DirectCast(CUShort(MyBase.RawKind), SyntaxKind)
			End Get
		End Property

		Public Overrides ReadOnly Property KindText As String
			Get
				Return Me.Kind.ToString()
			End Get
		End Property

		Public Overrides ReadOnly Property Language As String
			Get
				Return "Visual Basic"
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode.s_structuresTable = New ConditionalWeakTable(Of SyntaxNode, Dictionary(Of Microsoft.CodeAnalysis.SyntaxTrivia, SyntaxNode))()
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
		End Sub

		Protected Sub New(ByVal kind As SyntaxKind)
			MyBase.New(kind)
			GreenStats.NoteGreen(Me)
		End Sub

		Protected Sub New(ByVal kind As SyntaxKind, ByVal width As Integer)
			MyBase.New(kind, width)
			GreenStats.NoteGreen(Me)
		End Sub

		Protected Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo())
			MyBase.New(kind, errors)
			GreenStats.NoteGreen(Me)
		End Sub

		Protected Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal width As Integer)
			MyBase.New(kind, errors, width)
			GreenStats.NoteGreen(Me)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal diagnostics As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation())
			MyBase.New(kind, diagnostics, annotations)
			GreenStats.NoteGreen(Me)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal diagnostics As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal fullWidth As Integer)
			MyBase.New(kind, diagnostics, annotations, fullWidth)
			GreenStats.NoteGreen(Me)
		End Sub

		Public Overridable Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitVisualBasicSyntaxNode(Me)
		End Function

		Friend Overridable Sub AddSyntaxErrors(ByVal accumulatedErrors As List(Of DiagnosticInfo))
			If (MyBase.GetDiagnostics() IsNot Nothing) Then
				accumulatedErrors.AddRange(MyBase.GetDiagnostics())
			End If
			Dim slotCount As Integer = MyBase.SlotCount
			If (slotCount <> 0) Then
				Dim num As Integer = slotCount - 1
				For i As Integer = 0 To num
					Dim slot As GreenNode = Me.GetSlot(i)
					If (slot IsNot Nothing AndAlso slot.ContainsDiagnostics) Then
						DirectCast(slot, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode).AddSyntaxErrors(accumulatedErrors)
					End If
				Next

			End If
		End Sub

		Public Overrides Function CreateSeparator(Of TNode As SyntaxNode)(ByVal element As SyntaxNode) As Microsoft.CodeAnalysis.SyntaxToken
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken
			If (element.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinCondition) Then
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndKeyword
			End If
			Return Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.Token(syntaxKind, Nothing)
		End Function

		Private Function GetDebuggerDisplay() As String
			Dim fullString As String = Me.ToFullString()
			If (fullString.Length > 400) Then
				fullString = fullString.Substring(0, 400)
			End If
			Dim kind As SyntaxKind = Me.Kind
			Return [String].Concat(kind.ToString(), ":", fullString)
		End Function

		Friend Function GetFirstToken() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Return DirectCast(MyBase.GetFirstTerminal(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
		End Function

		Friend Function GetLastToken() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Return DirectCast(MyBase.GetLastTerminal(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
		End Function

		Friend Overridable Function GetLeadingTrivia() As GreenNode
			Dim leadingTrivia As GreenNode
			Dim firstToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.GetFirstToken()
			If (firstToken Is Nothing) Then
				leadingTrivia = Nothing
			Else
				leadingTrivia = firstToken.GetLeadingTrivia()
			End If
			Return leadingTrivia
		End Function

		Public Overrides Function GetLeadingTriviaCore() As GreenNode
			Return Me.GetLeadingTrivia()
		End Function

		Protected Overrides Function GetSlotCount() As Integer
			Throw ExceptionUtilities.Unreachable
		End Function

		Public Overrides Function GetStructure(ByVal trivia As Microsoft.CodeAnalysis.SyntaxTrivia) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (trivia.HasStructure) Then
				Dim parent As Microsoft.CodeAnalysis.SyntaxNode = trivia.Token.Parent
				If (parent IsNot Nothing) Then
					Dim syntaxNode1 As Microsoft.CodeAnalysis.SyntaxNode = Nothing
					Dim orCreateValue As Dictionary(Of Microsoft.CodeAnalysis.SyntaxTrivia, Microsoft.CodeAnalysis.SyntaxNode) = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode.s_structuresTable.GetOrCreateValue(parent)
					SyncLock orCreateValue
						If (Not orCreateValue.TryGetValue(trivia, syntaxNode1)) Then
							syntaxNode1 = Microsoft.CodeAnalysis.VisualBasic.Syntax.StructuredTriviaSyntax.Create(trivia)
							orCreateValue.Add(trivia, syntaxNode1)
						End If
					End SyncLock
					syntaxNode = syntaxNode1
				Else
					syntaxNode = Microsoft.CodeAnalysis.VisualBasic.Syntax.StructuredTriviaSyntax.Create(trivia)
				End If
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overridable Function GetSyntaxErrors() As IList(Of DiagnosticInfo)
			Dim diagnosticInfos As IList(Of DiagnosticInfo)
			If (MyBase.ContainsDiagnostics) Then
				Dim diagnosticInfos1 As List(Of DiagnosticInfo) = New List(Of DiagnosticInfo)()
				Me.AddSyntaxErrors(diagnosticInfos1)
				diagnosticInfos = diagnosticInfos1
			Else
				diagnosticInfos = Nothing
			End If
			Return diagnosticInfos
		End Function

		Friend Overridable Function GetTrailingTrivia() As GreenNode
			Dim trailingTrivia As GreenNode
			Dim lastToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = Me.GetLastToken()
			If (lastToken Is Nothing) Then
				trailingTrivia = Nothing
			Else
				trailingTrivia = lastToken.GetTrailingTrivia()
			End If
			Return trailingTrivia
		End Function

		Public Overrides Function GetTrailingTriviaCore() As GreenNode
			Return Me.GetTrailingTrivia()
		End Function

		Friend Shared Function IsEquivalentTo(ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Boolean
			Dim flag As Boolean
			If (left <> right) Then
				flag = If(left Is Nothing OrElse right Is Nothing, False, left.IsEquivalentTo(right))
			Else
				flag = True
			End If
			Return flag
		End Function

		Public Overrides Function IsTriviaWithEndOfLine() As Boolean
			If (Me.Kind = SyntaxKind.EndOfLineTrivia) Then
				Return True
			End If
			Return Me.Kind = SyntaxKind.CommentTrivia
		End Function

		Friend Function MatchesFactoryContext(ByVal context As ISyntaxFactoryContext) As Boolean
			If (context.IsWithinAsyncMethodOrLambda <> MyBase.ParsedInAsync) Then
				Return False
			End If
			Return context.IsWithinIteratorContext = MyBase.ParsedInIterator
		End Function

		Protected Sub SetFactoryContext(ByVal context As ISyntaxFactoryContext)
			If (context.IsWithinAsyncMethodOrLambda) Then
				MyBase.SetFlags(GreenNode.NodeFlags.FactoryContextIsInAsync)
			End If
			If (context.IsWithinIteratorContext) Then
				MyBase.SetFlags(GreenNode.NodeFlags.FactoryContextIsInQuery)
			End If
		End Sub
	End Class
End Namespace