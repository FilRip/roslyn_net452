Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class CompilationUnitContext
		Inherits NamespaceBlockContext
		Private _optionStmts As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax)

		Private _importsStmts As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsStatementSyntax)

		Private _attributeStmts As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributesStatementSyntax)

		Private _state As SyntaxKind

		Friend Overrides ReadOnly Property IsWithinAsyncMethodOrLambda As Boolean
			Get
				Return MyBase.Parser.IsScript
			End Get
		End Property

		Friend Sub New(ByVal parser As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser)
			MyBase.New(SyntaxKind.CompilationUnit, Nothing, Nothing)
			MyBase.Parser = parser
			Me._statements = Me._parser._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)()
			Me._state = SyntaxKind.OptionStatement
		End Sub

		Friend Overrides Function CreateBlockSyntax(ByVal endStmt As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Function CreateCompilationUnit(ByVal optionalTerminator As PunctuationSyntax, ByVal notClosedIfDirectives As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax), ByVal notClosedRegionDirectives As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax), ByVal haveRegionDirectives As Boolean, ByVal notClosedExternalSourceDirective As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)
			If (Me._state <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None) Then
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me._state
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement) Then
					syntaxList = MyBase.Body()
					Me._optionStmts = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax)(syntaxList.Node)
				ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement) Then
					syntaxList = MyBase.Body()
					Me._importsStmts = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsStatementSyntax)(syntaxList.Node)
				ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributesStatement) Then
					syntaxList = MyBase.Body()
					Me._attributeStmts = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributesStatementSyntax)(syntaxList.Node)
				End If
				Me._state = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
			End If
			Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) = MyBase.Body()
			Dim compilationUnitSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax = MyBase.SyntaxFactory.CompilationUnit(Me._optionStmts, Me._importsStmts, Me._attributeStmts, syntaxList1, optionalTerminator)
			Dim flag As Boolean = If(Not haveRegionDirectives, True, MyBase.Parser.CheckFeatureAvailability(Feature.RegionsEverywhere))
			If (notClosedIfDirectives IsNot Nothing OrElse notClosedRegionDirectives IsNot Nothing OrElse notClosedExternalSourceDirective IsNot Nothing OrElse Not flag) Then
				compilationUnitSyntax = CompilationUnitContext.DiagnosticRewriter.Rewrite(compilationUnitSyntax, notClosedIfDirectives, notClosedRegionDirectives, flag, notClosedExternalSourceDirective, MyBase.Parser)
				If (notClosedIfDirectives IsNot Nothing) Then
					notClosedIfDirectives.Free()
				End If
				If (notClosedRegionDirectives IsNot Nothing) Then
					notClosedRegionDirectives.Free()
				End If
			End If
			MyBase.FreeStatements()
			Return compilationUnitSyntax
		End Function

		Friend Overrides Function ProcessSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)
			While True
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me._state
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement) Then
					If (node.Kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement) Then
						syntaxList = MyBase.Body()
						Me._optionStmts = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax)(syntaxList.Node)
						Me._state = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement
					Else
						MyBase.Add(node)
						blockContext = Me
						Exit While
					End If
				ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement) Then
					If (node.Kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement) Then
						syntaxList = MyBase.Body()
						Me._importsStmts = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsStatementSyntax)(syntaxList.Node)
						Me._state = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributesStatement
					Else
						MyBase.Add(node)
						blockContext = Me
						Exit While
					End If
				ElseIf (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributesStatement) Then
					If (Me._parser.IsScript) Then
						Dim blockContext1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext = MyBase.TryProcessExecutableStatement(node)
						If (blockContext1 IsNot Nothing) Then
							blockContext = blockContext1
							Exit While
						End If
					End If
					blockContext = MyBase.ProcessSyntax(node)
					Exit While
				ElseIf (node.Kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AttributesStatement) Then
					syntaxList = MyBase.Body()
					Me._attributeStmts = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributesStatementSyntax)(syntaxList.Node)
					Me._state = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
				Else
					MyBase.Add(node)
					blockContext = Me
					Exit While
				End If
			End While
			Return blockContext
		End Function

		Friend Overrides Function TryLinkSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByRef newContext As BlockContext) As BlockContext.LinkResult
			Dim linkResult As BlockContext.LinkResult
			linkResult = If(Not Me._parser.IsScript, MyBase.TryLinkSyntax(node, newContext), MyBase.TryLinkStatement(node, newContext))
			Return linkResult
		End Function

		Private Class DiagnosticRewriter
			Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxRewriter
			Private _notClosedIfDirectives As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax)

			Private _notClosedRegionDirectives As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax)

			Private _notClosedExternalSourceDirective As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax

			Private _regionsAreAllowedEverywhere As Boolean

			Private _parser As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser

			Private _declarationBlocksBeingVisited As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)

			Private _parentsOfRegionDirectivesAwaitingClosure As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)

			Private _tokenWithDirectivesBeingVisited As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken

			Private Sub New()
				MyBase.New()
				Me._notClosedIfDirectives = Nothing
				Me._notClosedRegionDirectives = Nothing
				Me._notClosedExternalSourceDirective = Nothing
			End Sub

			Private Shared Function IsValidContainingBlockForRegionInVB12(ByVal containingBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Boolean
				Dim flag As Boolean
				flag = If(CUShort(containingBlock.Kind) - CUShort(SyntaxKind.SubBlock) > CUShort(SyntaxKind.EndSelectStatement), True, False)
				Return flag
			End Function

			Public Shared Function Rewrite(ByVal compilationUnit As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax, ByVal notClosedIfDirectives As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax), ByVal notClosedRegionDirectives As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax), ByVal regionsAreAllowedEverywhere As Boolean, ByVal notClosedExternalSourceDirective As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax, ByVal parser As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax
				Dim diagnosticRewriter As CompilationUnitContext.DiagnosticRewriter = New CompilationUnitContext.DiagnosticRewriter()
				If (notClosedIfDirectives IsNot Nothing) Then
					diagnosticRewriter._notClosedIfDirectives = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax)(ReferenceEqualityComparer.Instance)
					Dim enumerator As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax).Enumerator = notClosedIfDirectives.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax = enumerator.Current
						diagnosticRewriter._notClosedIfDirectives.Add(current)
					End While
				End If
				If (notClosedRegionDirectives IsNot Nothing) Then
					diagnosticRewriter._notClosedRegionDirectives = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax)(ReferenceEqualityComparer.Instance)
					Dim enumerator1 As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax).Enumerator = notClosedRegionDirectives.GetEnumerator()
					While enumerator1.MoveNext()
						Dim regionDirectiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax = enumerator1.Current
						diagnosticRewriter._notClosedRegionDirectives.Add(regionDirectiveTriviaSyntax)
					End While
				End If
				diagnosticRewriter._parser = parser
				diagnosticRewriter._regionsAreAllowedEverywhere = regionsAreAllowedEverywhere
				If (Not regionsAreAllowedEverywhere) Then
					diagnosticRewriter._declarationBlocksBeingVisited = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode).GetInstance()
					diagnosticRewriter._parentsOfRegionDirectivesAwaitingClosure = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode).GetInstance()
				End If
				diagnosticRewriter._notClosedExternalSourceDirective = notClosedExternalSourceDirective
				Dim compilationUnitSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax = DirectCast(diagnosticRewriter.Visit(compilationUnit), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax)
				If (Not regionsAreAllowedEverywhere) Then
					diagnosticRewriter._declarationBlocksBeingVisited.Free()
					diagnosticRewriter._parentsOfRegionDirectivesAwaitingClosure.Free()
				End If
				Return compilationUnitSyntax
			End Function

			Private Function VerifyRegionPlacement(ByVal original As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByVal rewritten As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
				Dim item As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Me._declarationBlocksBeingVisited.Peek()
				If (Me._declarationBlocksBeingVisited.Count > 1) Then
					If (Me._tokenWithDirectivesBeingVisited = item.GetFirstToken()) Then
						Dim leadingTrivia As GreenNode = Me._tokenWithDirectivesBeingVisited.GetLeadingTrivia()
						If (leadingTrivia IsNot Nothing AndAlso IReadOnlyListExtensions.Contains(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)((New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(leadingTrivia)).Nodes, original, Nothing)) Then
							item = Me._declarationBlocksBeingVisited(Me._declarationBlocksBeingVisited.Count - 2)
						End If
					ElseIf (Me._tokenWithDirectivesBeingVisited = item.GetLastToken()) Then
						Dim trailingTrivia As GreenNode = Me._tokenWithDirectivesBeingVisited.GetTrailingTrivia()
						If (trailingTrivia IsNot Nothing AndAlso IReadOnlyListExtensions.Contains(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)((New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(trailingTrivia)).Nodes, original, Nothing)) Then
							item = Me._declarationBlocksBeingVisited(Me._declarationBlocksBeingVisited.Count - 2)
						End If
					End If
				End If
				Dim flag As Boolean = Not CompilationUnitContext.DiagnosticRewriter.IsValidContainingBlockForRegionInVB12(item)
				If (original.Kind = SyntaxKind.RegionDirectiveTrivia) Then
					Me._parentsOfRegionDirectivesAwaitingClosure.Push(item)
				ElseIf (Me._parentsOfRegionDirectivesAwaitingClosure.Count > 0) Then
					Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = Me._parentsOfRegionDirectivesAwaitingClosure.Pop()
					If (visualBasicSyntaxNode <> item AndAlso CompilationUnitContext.DiagnosticRewriter.IsValidContainingBlockForRegionInVB12(visualBasicSyntaxNode)) Then
						flag = True
					End If
				End If
				If (flag) Then
					rewritten = Me._parser.ReportFeatureUnavailable(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(Feature.RegionsEverywhere, rewritten)
				End If
				Return rewritten
			End Function

			Public Overrides Function Visit(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
				visualBasicSyntaxNode = If(node Is Nothing OrElse Not node.ContainsDirectives, node, node.Accept(Me))
				Return visualBasicSyntaxNode
			End Function

			Public Overrides Function VisitAccessorBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
				If (Me._declarationBlocksBeingVisited IsNot Nothing) Then
					Me._declarationBlocksBeingVisited.Push(node)
				End If
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = MyBase.VisitAccessorBlock(node)
				If (Me._declarationBlocksBeingVisited IsNot Nothing) Then
					Me._declarationBlocksBeingVisited.Pop()
				End If
				Return visualBasicSyntaxNode
			End Function

			Public Overrides Function VisitClassBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
				If (Me._declarationBlocksBeingVisited IsNot Nothing) Then
					Me._declarationBlocksBeingVisited.Push(node)
				End If
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = MyBase.VisitClassBlock(node)
				If (Me._declarationBlocksBeingVisited IsNot Nothing) Then
					Me._declarationBlocksBeingVisited.Pop()
				End If
				Return visualBasicSyntaxNode
			End Function

			Public Overrides Function VisitCompilationUnit(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
				If (Me._declarationBlocksBeingVisited IsNot Nothing) Then
					Me._declarationBlocksBeingVisited.Push(node)
				End If
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = MyBase.VisitCompilationUnit(node)
				If (Me._declarationBlocksBeingVisited IsNot Nothing) Then
					Me._declarationBlocksBeingVisited.Pop()
				End If
				Return visualBasicSyntaxNode
			End Function

			Public Overrides Function VisitConstructorBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstructorBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
				If (Me._declarationBlocksBeingVisited IsNot Nothing) Then
					Me._declarationBlocksBeingVisited.Push(node)
				End If
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = MyBase.VisitConstructorBlock(node)
				If (Me._declarationBlocksBeingVisited IsNot Nothing) Then
					Me._declarationBlocksBeingVisited.Pop()
				End If
				Return visualBasicSyntaxNode
			End Function

			Public Overrides Function VisitEndRegionDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndRegionDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = MyBase.VisitEndRegionDirectiveTrivia(node)
				If (Not Me._regionsAreAllowedEverywhere) Then
					visualBasicSyntaxNode = Me.VerifyRegionPlacement(node, visualBasicSyntaxNode)
				End If
				Return visualBasicSyntaxNode
			End Function

			Public Overrides Function VisitEnumBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
				If (Me._declarationBlocksBeingVisited IsNot Nothing) Then
					Me._declarationBlocksBeingVisited.Push(node)
				End If
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = MyBase.VisitEnumBlock(node)
				If (Me._declarationBlocksBeingVisited IsNot Nothing) Then
					Me._declarationBlocksBeingVisited.Pop()
				End If
				Return visualBasicSyntaxNode
			End Function

			Public Overrides Function VisitEventBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
				If (Me._declarationBlocksBeingVisited IsNot Nothing) Then
					Me._declarationBlocksBeingVisited.Push(node)
				End If
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = MyBase.VisitEventBlock(node)
				If (Me._declarationBlocksBeingVisited IsNot Nothing) Then
					Me._declarationBlocksBeingVisited.Pop()
				End If
				Return visualBasicSyntaxNode
			End Function

			Public Overrides Function VisitExternalSourceDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = MyBase.VisitExternalSourceDirectiveTrivia(node)
				If (Me._notClosedExternalSourceDirective = node) Then
					visualBasicSyntaxNode = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(visualBasicSyntaxNode, ERRID.ERR_ExpectedEndExternalSource)
				End If
				Return visualBasicSyntaxNode
			End Function

			Public Overrides Function VisitIfDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = MyBase.VisitIfDirectiveTrivia(node)
				If (Me._notClosedIfDirectives IsNot Nothing AndAlso Me._notClosedIfDirectives.Contains(node)) Then
					visualBasicSyntaxNode = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(visualBasicSyntaxNode, ERRID.ERR_LbExpectedEndIf)
				End If
				Return visualBasicSyntaxNode
			End Function

			Public Overrides Function VisitInterfaceBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
				If (Me._declarationBlocksBeingVisited IsNot Nothing) Then
					Me._declarationBlocksBeingVisited.Push(node)
				End If
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = MyBase.VisitInterfaceBlock(node)
				If (Me._declarationBlocksBeingVisited IsNot Nothing) Then
					Me._declarationBlocksBeingVisited.Pop()
				End If
				Return visualBasicSyntaxNode
			End Function

			Public Overrides Function VisitMethodBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
				If (Me._declarationBlocksBeingVisited IsNot Nothing) Then
					Me._declarationBlocksBeingVisited.Push(node)
				End If
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = MyBase.VisitMethodBlock(node)
				If (Me._declarationBlocksBeingVisited IsNot Nothing) Then
					Me._declarationBlocksBeingVisited.Pop()
				End If
				Return visualBasicSyntaxNode
			End Function

			Public Overrides Function VisitModuleBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
				If (Me._declarationBlocksBeingVisited IsNot Nothing) Then
					Me._declarationBlocksBeingVisited.Push(node)
				End If
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = MyBase.VisitModuleBlock(node)
				If (Me._declarationBlocksBeingVisited IsNot Nothing) Then
					Me._declarationBlocksBeingVisited.Pop()
				End If
				Return visualBasicSyntaxNode
			End Function

			Public Overrides Function VisitNamespaceBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
				If (Me._declarationBlocksBeingVisited IsNot Nothing) Then
					Me._declarationBlocksBeingVisited.Push(node)
				End If
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = MyBase.VisitNamespaceBlock(node)
				If (Me._declarationBlocksBeingVisited IsNot Nothing) Then
					Me._declarationBlocksBeingVisited.Pop()
				End If
				Return visualBasicSyntaxNode
			End Function

			Public Overrides Function VisitOperatorBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
				If (Me._declarationBlocksBeingVisited IsNot Nothing) Then
					Me._declarationBlocksBeingVisited.Push(node)
				End If
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = MyBase.VisitOperatorBlock(node)
				If (Me._declarationBlocksBeingVisited IsNot Nothing) Then
					Me._declarationBlocksBeingVisited.Pop()
				End If
				Return visualBasicSyntaxNode
			End Function

			Public Overrides Function VisitPropertyBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
				If (Me._declarationBlocksBeingVisited IsNot Nothing) Then
					Me._declarationBlocksBeingVisited.Push(node)
				End If
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = MyBase.VisitPropertyBlock(node)
				If (Me._declarationBlocksBeingVisited IsNot Nothing) Then
					Me._declarationBlocksBeingVisited.Pop()
				End If
				Return visualBasicSyntaxNode
			End Function

			Public Overrides Function VisitRegionDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = MyBase.VisitRegionDirectiveTrivia(node)
				If (Me._notClosedRegionDirectives IsNot Nothing AndAlso Me._notClosedRegionDirectives.Contains(node)) Then
					visualBasicSyntaxNode = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Parser.ReportSyntaxError(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(visualBasicSyntaxNode, ERRID.ERR_ExpectedEndRegion)
				ElseIf (Not Me._regionsAreAllowedEverywhere) Then
					visualBasicSyntaxNode = Me.VerifyRegionPlacement(node, visualBasicSyntaxNode)
				End If
				Return visualBasicSyntaxNode
			End Function

			Public Overrides Function VisitStructureBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
				If (Me._declarationBlocksBeingVisited IsNot Nothing) Then
					Me._declarationBlocksBeingVisited.Push(node)
				End If
				Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = MyBase.VisitStructureBlock(node)
				If (Me._declarationBlocksBeingVisited IsNot Nothing) Then
					Me._declarationBlocksBeingVisited.Pop()
				End If
				Return visualBasicSyntaxNode
			End Function

			Public Overrides Function VisitSyntaxToken(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
				Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
				Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
				If (token Is Nothing OrElse Not token.ContainsDirectives) Then
					syntaxToken = token
				Else
					Me._tokenWithDirectivesBeingVisited = token
					Dim leadingTrivia As Microsoft.CodeAnalysis.GreenNode = token.GetLeadingTrivia()
					Dim trailingTrivia As Microsoft.CodeAnalysis.GreenNode = token.GetTrailingTrivia()
					If (leadingTrivia IsNot Nothing) Then
						syntaxList = MyBase.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(leadingTrivia))
						Dim node As Microsoft.CodeAnalysis.GreenNode = syntaxList.Node
						If (leadingTrivia <> node) Then
							token = DirectCast(token.WithLeadingTrivia(node), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
						End If
					End If
					If (trailingTrivia IsNot Nothing) Then
						syntaxList = MyBase.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(trailingTrivia))
						Dim greenNode As Microsoft.CodeAnalysis.GreenNode = syntaxList.Node
						If (trailingTrivia <> greenNode) Then
							token = DirectCast(token.WithTrailingTrivia(greenNode), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
						End If
					End If
					Me._tokenWithDirectivesBeingVisited = Nothing
					syntaxToken = token
				End If
				Return syntaxToken
			End Function
		End Class
	End Class
End Namespace