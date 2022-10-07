Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class VBSemanticModel
		Inherits SemanticModel
		Public Shadows MustOverride ReadOnly Property Compilation As VisualBasicCompilation

		Protected NotOverridable Overrides ReadOnly Property CompilationCore As Microsoft.CodeAnalysis.Compilation
			Get
				Return Me.Compilation
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Language As String
			Get
				Return "Visual Basic"
			End Get
		End Property

		Public ReadOnly Property OptionCompareText As Boolean
			Get
				Return Me.GetEnclosingBinder(Me.Root.SpanStart).OptionCompareText
			End Get
		End Property

		Public ReadOnly Property OptionExplicit As Boolean
			Get
				Return Me.GetEnclosingBinder(Me.Root.SpanStart).OptionExplicit
			End Get
		End Property

		Public ReadOnly Property OptionInfer As Boolean
			Get
				Return Me.GetEnclosingBinder(Me.Root.SpanStart).OptionInfer
			End Get
		End Property

		Public ReadOnly Property OptionStrict As Microsoft.CodeAnalysis.VisualBasic.OptionStrict
			Get
				Return Me.GetEnclosingBinder(Me.Root.SpanStart).OptionStrict
			End Get
		End Property

		Public Shadows MustOverride ReadOnly Property ParentModel As SemanticModel

		Protected NotOverridable Overrides ReadOnly Property ParentModelCore As SemanticModel
			Get
				Return Me.ParentModel
			End Get
		End Property

		Friend Shadows MustOverride ReadOnly Property Root As SyntaxNode

		Protected NotOverridable Overrides ReadOnly Property RootCore As SyntaxNode
			Get
				Return Me.Root
			End Get
		End Property

		Public Shadows MustOverride ReadOnly Property SyntaxTree As Microsoft.CodeAnalysis.SyntaxTree

		Protected NotOverridable Overrides ReadOnly Property SyntaxTreeCore As Microsoft.CodeAnalysis.SyntaxTree
			Get
				Return Me.SyntaxTree
			End Get
		End Property

		Protected Sub New()
			MyBase.New()
		End Sub

		Private Sub AddLookupSymbolsInfo(ByVal position As Integer, ByVal info As LookupSymbolsInfo, Optional ByVal container As NamespaceOrTypeSymbol = Nothing, Optional ByVal options As LookupOptions = 0)
			Me.CheckPosition(position)
			Dim enclosingBinder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me.GetEnclosingBinder(position)
			If (enclosingBinder IsNot Nothing) Then
				If (container Is Nothing) Then
					enclosingBinder.AddLookupSymbolsInfo(info, options)
					Return
				End If
				enclosingBinder.AddMemberLookupSymbolsInfo(info, container, options)
			End If
		End Sub

		Private Sub AddSymbolsFromDiagnosticInfo(ByVal symbolsBuilder As ArrayBuilder(Of Symbol), ByVal diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo)
			Dim diagnosticInfoWithSymbol As IDiagnosticInfoWithSymbols = TryCast(diagnosticInfo, IDiagnosticInfoWithSymbols)
			If (diagnosticInfoWithSymbol IsNot Nothing) Then
				diagnosticInfoWithSymbol.GetAssociatedSymbols(symbolsBuilder)
			End If
		End Sub

		Private Sub AdjustSymbolsForObjectCreation(ByVal boundNodes As BoundNodeSummary, ByVal binderOpt As Microsoft.CodeAnalysis.VisualBasic.Binder, ByRef bindingSymbols As ImmutableArray(Of Symbol), ByVal memberGroupBuilder As ArrayBuilder(Of Symbol), ByRef resultKind As LookupResultKind)
			Dim constructor As MethodSymbol = Nothing
			Dim lowestBoundNode As BoundNode = boundNodes.LowestBoundNode
			Dim lowestBoundNodeOfSyntacticParent As BoundNode = boundNodes.LowestBoundNodeOfSyntacticParent
			Dim syntax As SyntaxNode = lowestBoundNodeOfSyntacticParent.Syntax
			If (syntax IsNot Nothing AndAlso lowestBoundNode IsNot Nothing AndAlso syntax = lowestBoundNode.Syntax.Parent AndAlso syntax.Kind() = SyntaxKind.Attribute AndAlso DirectCast(syntax, AttributeSyntax).Name = lowestBoundNode.Syntax) Then
				Dim symbols As ImmutableArray(Of Symbol) = VBSemanticModel.UnwrapAliases(bindingSymbols)
				If (symbols.Length = 1 AndAlso TypeOf symbols(0) Is TypeSymbol) Then
					Dim item As NamedTypeSymbol = TryCast(DirectCast(symbols(0), TypeSymbol), NamedTypeSymbol)
					Dim kind As BoundKind = lowestBoundNodeOfSyntacticParent.Kind
					If (kind = BoundKind.BadExpression) Then
						Dim boundBadExpression As Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression = DirectCast(lowestBoundNodeOfSyntacticParent, Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression)
						resultKind = LookupResult.WorseResultKind(DirectCast(CInt(resultKind), LookupResultKind), boundBadExpression.ResultKind)
					Else
						If (kind <> BoundKind.Attribute) Then
							Throw ExceptionUtilities.UnexpectedValue(lowestBoundNodeOfSyntacticParent.Kind)
						End If
						Dim boundAttribute As Microsoft.CodeAnalysis.VisualBasic.BoundAttribute = DirectCast(lowestBoundNodeOfSyntacticParent, Microsoft.CodeAnalysis.VisualBasic.BoundAttribute)
						constructor = boundAttribute.Constructor
						resultKind = LookupResult.WorseResultKind(DirectCast(CInt(resultKind), LookupResultKind), boundAttribute.ResultKind)
					End If
					Me.AdjustSymbolsForObjectCreation(lowestBoundNode, item, constructor, binderOpt, bindingSymbols, memberGroupBuilder, resultKind)
				End If
			End If
		End Sub

		Private Sub AdjustSymbolsForObjectCreation(ByVal lowestBoundNode As BoundNode, ByVal namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, ByVal constructor As MethodSymbol, ByVal binderOpt As Microsoft.CodeAnalysis.VisualBasic.Binder, ByRef bindingSymbols As ImmutableArray(Of Symbol), ByVal memberGroupBuilder As ArrayBuilder(Of Symbol), ByRef resultKind As LookupResultKind)
			Dim empty As ImmutableArray(Of MethodSymbol)
			Dim coClassType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			If (namedTypeSymbol IsNot Nothing) Then
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = If(binderOpt, Me.GetEnclosingBinder(lowestBoundNode.Syntax.SpanStart))
				If (binder Is Nothing) Then
					empty = ImmutableArray(Of MethodSymbol).Empty
				Else
					If (namedTypeSymbol.IsInterface) Then
						coClassType = TryCast(namedTypeSymbol.CoClassType, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
					Else
						coClassType = Nothing
					End If
					Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = coClassType
					Dim binder1 As Microsoft.CodeAnalysis.VisualBasic.Binder = binder
					Dim namedTypeSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = If(namedTypeSymbol1, namedTypeSymbol)
					Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
					empty = binder1.GetAccessibleConstructors(namedTypeSymbol2, discarded)
					Dim instanceConstructors As ImmutableArray(Of MethodSymbol) = namedTypeSymbol.InstanceConstructors
					If (Not System.Linq.ImmutableArrayExtensions.Any(Of MethodSymbol)(empty) AndAlso System.Linq.ImmutableArrayExtensions.Any(Of MethodSymbol)(instanceConstructors)) Then
						empty = instanceConstructors
					End If
				End If
				If (constructor IsNot Nothing) Then
					bindingSymbols = ImmutableArray.Create(Of Symbol)(constructor)
				ElseIf (empty.Length <> 0) Then
					bindingSymbols = StaticCast(Of Symbol).From(Of MethodSymbol)(empty)
					resultKind = LookupResult.WorseResultKind(DirectCast(CInt(resultKind), LookupResultKind), LookupResultKind.OverloadResolutionFailure)
				End If
				memberGroupBuilder.AddRange(Of MethodSymbol)(empty)
			End If
		End Sub

		Public Overridable Function AnalyzeControlFlow(ByVal firstStatement As StatementSyntax, ByVal lastStatement As StatementSyntax) As ControlFlowAnalysis
			Throw New NotSupportedException()
		End Function

		Public Overridable Function AnalyzeControlFlow(ByVal statement As StatementSyntax) As ControlFlowAnalysis
			Return Me.AnalyzeControlFlow(statement, statement)
		End Function

		Protected NotOverridable Overrides Function AnalyzeControlFlowCore(ByVal firstStatement As SyntaxNode, ByVal lastStatement As SyntaxNode) As ControlFlowAnalysis
			Return Me.AnalyzeControlFlow(VBSemanticModel.SafeCastArgument(Of StatementSyntax)(firstStatement, "firstStatement"), VBSemanticModel.SafeCastArgument(Of StatementSyntax)(lastStatement, "lastStatement"))
		End Function

		Protected NotOverridable Overrides Function AnalyzeControlFlowCore(ByVal statement As SyntaxNode) As ControlFlowAnalysis
			Return Me.AnalyzeControlFlow(VBSemanticModel.SafeCastArgument(Of StatementSyntax)(statement, "statement"))
		End Function

		Public Overridable Function AnalyzeDataFlow(ByVal expression As ExpressionSyntax) As DataFlowAnalysis
			Throw New NotSupportedException()
		End Function

		Public Overridable Function AnalyzeDataFlow(ByVal firstStatement As StatementSyntax, ByVal lastStatement As StatementSyntax) As DataFlowAnalysis
			Throw New NotSupportedException()
		End Function

		Public Overridable Function AnalyzeDataFlow(ByVal statement As StatementSyntax) As DataFlowAnalysis
			Return Me.AnalyzeDataFlow(statement, statement)
		End Function

		Protected NotOverridable Overrides Function AnalyzeDataFlowCore(ByVal firstStatement As SyntaxNode, ByVal lastStatement As SyntaxNode) As DataFlowAnalysis
			Return Me.AnalyzeDataFlow(VBSemanticModel.SafeCastArgument(Of StatementSyntax)(firstStatement, "firstStatement"), VBSemanticModel.SafeCastArgument(Of StatementSyntax)(lastStatement, "lastStatement"))
		End Function

		Protected NotOverridable Overrides Function AnalyzeDataFlowCore(ByVal statementOrExpression As SyntaxNode) As DataFlowAnalysis
			Dim dataFlowAnalysi As DataFlowAnalysis
			If (statementOrExpression Is Nothing) Then
				Throw New ArgumentNullException("statementOrExpression")
			End If
			If (Not TypeOf statementOrExpression Is ExecutableStatementSyntax) Then
				If (Not TypeOf statementOrExpression Is ExpressionSyntax) Then
					Throw New ArgumentException(VBResources.StatementOrExpressionIsNotAValidType)
				End If
				dataFlowAnalysi = Me.AnalyzeDataFlow(DirectCast(statementOrExpression, ExpressionSyntax))
			Else
				dataFlowAnalysi = Me.AnalyzeDataFlow(DirectCast(statementOrExpression, StatementSyntax))
			End If
			Return dataFlowAnalysi
		End Function

		Private Sub AppendSymbolsWithName(ByVal results As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbol), ByVal name As String, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal container As NamespaceOrTypeSymbol, ByVal options As LookupOptions, ByVal info As LookupSymbolsInfo)
			Dim arityEnumerable As AbstractLookupSymbolsInfo(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).IArityEnumerable = Nothing
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Nothing
			If (info.TryGetAritiesAndUniqueSymbol(name, arityEnumerable, symbol)) Then
				If (symbol IsNot Nothing) Then
					results.Add(symbol)
					Return
				End If
				If (arityEnumerable IsNot Nothing) Then
					Me.LookupSymbols(binder, container, name, arityEnumerable, options, results)
					Return
				End If
				Me.LookupSymbols(binder, container, name, 0, options, results)
			End If
		End Sub

		Friend Overridable Function Bind(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal node As SyntaxNode, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (expressionSyntax Is Nothing) Then
				Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
				If (statementSyntax Is Nothing) Then
					boundNode = Nothing
				Else
					boundNode = binder.BindStatement(statementSyntax, diagnostics)
				End If
			Else
				boundNode = binder.BindNamespaceOrTypeOrExpressionSyntaxForSemanticModel(expressionSyntax, diagnostics)
			End If
			Return boundNode
		End Function

		Friend Function CanGetSemanticInfo(ByVal node As VisualBasicSyntaxNode, Optional ByVal allowNamedArgumentName As Boolean = False) As Boolean
			Dim flag As Boolean
			Dim flag1 As Boolean
			If (node.Kind() = SyntaxKind.XmlName) Then
				flag = False
			ElseIf (node.EnclosingStructuredTrivia() Is Nothing) Then
				If (node.IsMissing) Then
					flag1 = False
				Else
					flag1 = If((Not TypeOf node Is ExpressionSyntax OrElse Not allowNamedArgumentName AndAlso SyntaxFacts.IsNamedArgumentName(node)) AndAlso Not TypeOf node Is AttributeSyntax AndAlso Not TypeOf node Is QueryClauseSyntax AndAlso Not TypeOf node Is ExpressionRangeVariableSyntax, TypeOf node Is OrderingSyntax, True)
				End If
				flag = flag1
			Else
				flag = VBSemanticModel.IsInCrefOrNameAttributeInterior(node)
			End If
			Return flag
		End Function

		Private Sub CheckModelAndSyntaxNodeToSpeculate(ByVal node As VisualBasicSyntaxNode)
			If (node Is Nothing) Then
				Throw New ArgumentNullException("node")
			End If
			If (Me.IsSpeculativeSemanticModel) Then
				Throw New InvalidOperationException(VBResources.ChainingSpeculativeModelIsNotSupported)
			End If
			If (Me.Compilation.ContainsSyntaxTree(node.SyntaxTree)) Then
				Throw New ArgumentException(VBResources.SpeculatedSyntaxNodeCannotBelongToCurrentCompilation)
			End If
		End Sub

		Protected Sub CheckPosition(ByVal position As Integer)
			Dim flag As Boolean
			Dim num As Integer = Me.Root.Position
			Dim endPosition As Integer = Me.Root.EndPosition
			If (position <> endPosition) Then
				flag = False
			Else
				Dim fullSpan As TextSpan = Me.SyntaxTree.GetRoot(New CancellationToken()).FullSpan
				flag = position = fullSpan.[End]
			End If
			Dim flag1 As Boolean = flag
			If ((num > position OrElse position >= endPosition) AndAlso Not flag1 AndAlso (num <> endPosition OrElse position <> endPosition)) Then
				Throw New ArgumentException(VBResources.PositionIsNotWithinSyntax)
			End If
		End Sub

		Friend Sub CheckSyntaxNode(ByVal node As VisualBasicSyntaxNode)
			If (node Is Nothing) Then
				Throw New ArgumentNullException("node")
			End If
			If (Not Me.IsInTree(node)) Then
				Throw New ArgumentException(VBResources.NodeIsNotWithinSyntaxTree)
			End If
		End Sub

		Public MustOverride Function ClassifyConversion(ByVal expression As ExpressionSyntax, ByVal destination As ITypeSymbol) As Conversion

		Public Function ClassifyConversion(ByVal position As Integer, ByVal expression As ExpressionSyntax, ByVal destination As ITypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Conversion
			Dim conversion As Microsoft.CodeAnalysis.VisualBasic.Conversion
			Dim keyValuePair As KeyValuePair(Of ConversionKind, MethodSymbol)
			If (destination Is Nothing) Then
				Throw New ArgumentNullException("destination")
			End If
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = destination.EnsureVbSymbolOrNothing(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)("destination")
			Me.CheckPosition(position)
			Dim enclosingBinder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me.GetEnclosingBinder(position)
			If (enclosingBinder IsNot Nothing) Then
				enclosingBinder = SpeculativeBinder.Create(enclosingBinder)
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = enclosingBinder.BindValue(expression, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded, False)
				If (boundExpression Is Nothing OrElse typeSymbol.IsErrorType()) Then
					keyValuePair = New KeyValuePair(Of ConversionKind, MethodSymbol)()
					conversion = New Microsoft.CodeAnalysis.VisualBasic.Conversion(keyValuePair)
					Return conversion
				End If
				Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
				conversion = New Microsoft.CodeAnalysis.VisualBasic.Conversion(Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyConversion(boundExpression, typeSymbol, enclosingBinder, discarded))
				Return conversion
			End If
			keyValuePair = New KeyValuePair(Of ConversionKind, MethodSymbol)()
			conversion = New Microsoft.CodeAnalysis.VisualBasic.Conversion(keyValuePair)
			Return conversion
		End Function

		Friend Overrides Sub ComputeDeclarationsInNode(ByVal node As SyntaxNode, ByVal associatedSymbol As ISymbol, ByVal getSymbol As Boolean, ByVal builder As ArrayBuilder(Of DeclarationInfo), ByVal cancellationToken As System.Threading.CancellationToken, Optional ByVal levelsToCompute As Nullable(Of Integer) = Nothing)
			VisualBasicDeclarationComputer.ComputeDeclarationsInNode(Me, node, getSymbol, builder, cancellationToken, Nothing)
		End Sub

		Friend Overrides Sub ComputeDeclarationsInSpan(ByVal span As TextSpan, ByVal getSymbol As Boolean, ByVal builder As ArrayBuilder(Of DeclarationInfo), ByVal cancellationToken As System.Threading.CancellationToken)
			VisualBasicDeclarationComputer.ComputeDeclarationsInSpan(Me, span, getSymbol, builder, cancellationToken)
		End Sub

		Friend Function FindInitialNodeFromPosition(ByVal position As Integer) As SyntaxNode
			Dim root As SyntaxNode
			Dim cancellationToken As System.Threading.CancellationToken
			Dim previousToken As SyntaxToken
			Dim flag As Boolean
			Dim num As Integer = Me.Root.Position
			Dim endPosition As Integer = Me.Root.EndPosition
			If (position <> endPosition) Then
				flag = False
			Else
				Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = Me.SyntaxTree
				cancellationToken = New System.Threading.CancellationToken()
				Dim fullSpan As TextSpan = syntaxTree.GetRoot(cancellationToken).FullSpan
				flag = position = fullSpan.[End]
			End If
			Dim flag1 As Boolean = flag
			If ((num > position OrElse position >= endPosition) AndAlso Not flag1) Then
				If (num <> endPosition OrElse position <> endPosition) Then
					Throw ExceptionUtilities.Unreachable
				End If
				root = Me.Root
			Else
				If (Not flag1) Then
					previousToken = Me.Root.FindToken(position, True)
				Else
					Dim syntaxTree1 As Microsoft.CodeAnalysis.SyntaxTree = Me.SyntaxTree
					cancellationToken = New System.Threading.CancellationToken()
					previousToken = syntaxTree1.GetRoot(cancellationToken).FindToken(position, True)
				End If
				If (DirectCast(previousToken.Parent, VisualBasicSyntaxNode).EnclosingStructuredTrivia() Is Nothing OrElse Not VBSemanticModel.IsInCrefOrNameAttributeInterior(DirectCast(previousToken.Parent, VisualBasicSyntaxNode))) Then
					If (Not flag1) Then
						previousToken = Me.Root.FindToken(position, False)
					Else
						Dim syntaxTree2 As Microsoft.CodeAnalysis.SyntaxTree = Me.SyntaxTree
						cancellationToken = New System.Threading.CancellationToken()
						previousToken = syntaxTree2.GetRoot(cancellationToken).FindToken(position, False)
					End If
				End If
				If (position < previousToken.SpanStart) Then
					previousToken = previousToken.GetPreviousToken(False, False, False, False)
				End If
				If (previousToken.SpanStart < num) Then
					root = Me.Root
				ElseIf (previousToken.Parent Is Nothing) Then
					root = Me.Root
				Else
					root = DirectCast(previousToken.Parent, VisualBasicSyntaxNode)
				End If
			End If
			Return root
		End Function

		Private Function FindNamedParameter(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal argumentName As String) As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol
			Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol
			Dim parameters As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol)
			Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol).Enumerator
			Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol
			If (symbol.Kind <> SymbolKind.Method) Then
				If (symbol.Kind = SymbolKind.[Property]) Then
					parameters = DirectCast(symbol, PropertySymbol).Parameters
					enumerator = parameters.GetEnumerator()
					While enumerator.MoveNext()
						current = enumerator.Current
						If (Not CaseInsensitiveComparison.Equals(current.Name, argumentName)) Then
							Continue While
						End If
						parameterSymbol = current
						Return parameterSymbol
					End While
					parameterSymbol = Nothing
					Return parameterSymbol
				End If
				parameterSymbol = Nothing
				Return parameterSymbol
			Else
				parameters = DirectCast(symbol, MethodSymbol).Parameters
			End If
			enumerator = parameters.GetEnumerator()
			While enumerator.MoveNext()
				current = enumerator.Current
				If (Not CaseInsensitiveComparison.Equals(current.Name, argumentName)) Then
					Continue While
				End If
				parameterSymbol = current
				Return parameterSymbol
			End While
			parameterSymbol = Nothing
			Return parameterSymbol
		End Function

		Private Function FindNameParameterInfo(ByVal invocationInfosymbols As ImmutableArray(Of Symbol), ByVal arGumentName As String, ByVal reason As CandidateReason) As SymbolInfo
			Dim none As SymbolInfo
			Dim instance As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance()
			Dim enumerator As ImmutableArray(Of Symbol).Enumerator = invocationInfosymbols.GetEnumerator()
			While enumerator.MoveNext()
				Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = Me.FindNamedParameter(enumerator.Current, arGumentName)
				If (parameterSymbol Is Nothing) Then
					Continue While
				End If
				instance.Add(parameterSymbol)
			End While
			If (instance.Count <> 0) Then
				none = SymbolInfoFactory.Create(StaticCast(Of ISymbol).From(Of Symbol)(instance.ToImmutableAndFree()), reason)
			Else
				instance.Free()
				none = SymbolInfo.None
			End If
			Return none
		End Function

		Public Function GetAggregateClauseSymbolInfo(ByVal aggregateSyntax As AggregateClauseSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.VisualBasic.AggregateClauseSymbolInfo
			Dim aggregateClauseSymbolInfo As Microsoft.CodeAnalysis.VisualBasic.AggregateClauseSymbolInfo
			If (aggregateSyntax Is Nothing) Then
				Throw New ArgumentNullException("aggregateSyntax")
			End If
			If (Not Me.IsInTree(aggregateSyntax)) Then
				Throw New ArgumentException(VBResources.AggregateSyntaxNotWithinSyntaxTree)
			End If
			aggregateClauseSymbolInfo = If(aggregateSyntax.Parent Is Nothing OrElse aggregateSyntax.Parent.Kind() = SyntaxKind.QueryExpression AndAlso DirectCast(aggregateSyntax.Parent, QueryExpressionSyntax).Clauses.FirstOrDefault() = aggregateSyntax, New Microsoft.CodeAnalysis.VisualBasic.AggregateClauseSymbolInfo(SymbolInfo.None), Me.GetAggregateClauseSymbolInfoWorker(aggregateSyntax, cancellationToken))
			Return aggregateClauseSymbolInfo
		End Function

		Friend MustOverride Function GetAggregateClauseSymbolInfoWorker(ByVal node As AggregateClauseSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As AggregateClauseSymbolInfo

		Public Function GetAliasInfo(ByVal nameSyntax As IdentifierNameSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IAliasSymbol
			Dim symbol As IAliasSymbol
			Me.CheckSyntaxNode(nameSyntax)
			If (Not Me.CanGetSemanticInfo(nameSyntax, False)) Then
				symbol = Nothing
			Else
				Dim expressionSymbolInfo As SymbolInfo = Me.GetExpressionSymbolInfo(nameSyntax, VBSemanticModel.SymbolInfoOptions.PreferTypeToConstructors Or VBSemanticModel.SymbolInfoOptions.PreserveAliases, cancellationToken)
				symbol = TryCast(expressionSymbolInfo.Symbol, IAliasSymbol)
			End If
			Return symbol
		End Function

		Protected NotOverridable Overrides Function GetAliasInfoCore(ByVal node As SyntaxNode, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IAliasSymbol
			Dim aliasInfo As IAliasSymbol
			If (node Is Nothing) Then
				Throw New ArgumentNullException("node")
			End If
			Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)
			If (identifierNameSyntax Is Nothing) Then
				aliasInfo = Nothing
			Else
				aliasInfo = Me.GetAliasInfo(identifierNameSyntax, cancellationToken)
			End If
			Return aliasInfo
		End Function

		Friend MustOverride Function GetAttributeMemberGroup(ByVal attribute As AttributeSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of Symbol)

		Friend MustOverride Function GetAttributeSymbolInfo(ByVal attribute As AttributeSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SymbolInfo

		Friend MustOverride Function GetAttributeTypeInfo(ByVal attribute As AttributeSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As VisualBasicTypeInfo

		Public Function GetAwaitExpressionInfo(ByVal awaitExpression As AwaitExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.VisualBasic.AwaitExpressionInfo
			Dim awaitExpressionInfo As Microsoft.CodeAnalysis.VisualBasic.AwaitExpressionInfo
			Me.CheckSyntaxNode(awaitExpression)
			awaitExpressionInfo = If(Not Me.CanGetSemanticInfo(awaitExpression, False), New Microsoft.CodeAnalysis.VisualBasic.AwaitExpressionInfo(), Me.GetAwaitExpressionInfoWorker(awaitExpression, cancellationToken))
			Return awaitExpressionInfo
		End Function

		Friend MustOverride Function GetAwaitExpressionInfoWorker(ByVal awaitExpression As AwaitExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As AwaitExpressionInfo

		Friend MustOverride Function GetCollectionInitializerAddSymbolInfo(ByVal collectionInitializer As ObjectCreationExpressionSyntax, ByVal node As ExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SymbolInfo

		Public Function GetCollectionInitializerSymbolInfo(ByVal expression As ExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SymbolInfo
			Dim none As SymbolInfo
			Me.CheckSyntaxNode(expression)
			If (expression.Parent IsNot Nothing AndAlso expression.Parent.Kind() = SyntaxKind.CollectionInitializer AndAlso expression.Parent.Parent IsNot Nothing AndAlso expression.Parent.Parent.Kind() = SyntaxKind.ObjectCollectionInitializer AndAlso DirectCast(expression.Parent.Parent, ObjectCollectionInitializerSyntax).Initializer = expression.Parent AndAlso expression.Parent.Parent.Parent IsNot Nothing AndAlso expression.Parent.Parent.Parent.Kind() = SyntaxKind.ObjectCreationExpression AndAlso Me.CanGetSemanticInfo(expression.Parent.Parent.Parent, False)) Then
				Dim parent As ObjectCreationExpressionSyntax = DirectCast(expression.Parent.Parent.Parent, ObjectCreationExpressionSyntax)
				If (parent.Initializer <> expression.Parent.Parent) Then
					none = SymbolInfo.None
					Return none
				End If
				none = Me.GetCollectionInitializerAddSymbolInfo(parent, expression, cancellationToken)
				Return none
			End If
			none = SymbolInfo.None
			Return none
		End Function

		Public Function GetCollectionRangeVariableSymbolInfo(ByVal variableSyntax As CollectionRangeVariableSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As CollectionRangeVariableSymbolInfo
			If (variableSyntax Is Nothing) Then
				Throw New ArgumentNullException("variableSyntax")
			End If
			If (Not Me.IsInTree(variableSyntax)) Then
				Throw New ArgumentException(VBResources.VariableSyntaxNotWithinSyntaxTree)
			End If
			Return Me.GetCollectionRangeVariableSymbolInfoWorker(variableSyntax, cancellationToken)
		End Function

		Friend MustOverride Function GetCollectionRangeVariableSymbolInfoWorker(ByVal node As CollectionRangeVariableSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As CollectionRangeVariableSymbolInfo

		Public Function GetConstantValue(ByVal expression As ExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As [Optional](Of Object)
			Dim [optional] As [Optional](Of Object)
			Me.CheckSyntaxNode(expression)
			If (Me.CanGetSemanticInfo(expression, False)) Then
				Dim expressionConstantValue As ConstantValue = Me.GetExpressionConstantValue(expression, cancellationToken)
				If (expressionConstantValue Is Nothing OrElse expressionConstantValue.IsBad) Then
					[optional] = New [Optional](Of Object)()
					Return [optional]
				End If
				[optional] = New [Optional](Of Object)(RuntimeHelpers.GetObjectValue(expressionConstantValue.Value))
				Return [optional]
			End If
			[optional] = New [Optional](Of Object)()
			Return [optional]
		End Function

		Protected NotOverridable Overrides Function GetConstantValueCore(ByVal node As SyntaxNode, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As [Optional](Of Object)
			Dim [optional] As [Optional](Of Object)
			If (node Is Nothing) Then
				Throw New ArgumentNullException("node")
			End If
			[optional] = If(Not TypeOf node Is ExpressionSyntax, New [Optional](Of Object)(), Me.GetConstantValue(DirectCast(node, ExpressionSyntax), cancellationToken))
			Return [optional]
		End Function

		Friend Function GetConstantValueForNode(ByVal boundNodes As BoundNodeSummary) As ConstantValue
			Dim constantValueOpt As ConstantValue = Nothing
			Dim lowestBoundNode As BoundExpression = TryCast(boundNodes.LowestBoundNode, BoundExpression)
			If (lowestBoundNode IsNot Nothing) Then
				constantValueOpt = lowestBoundNode.ConstantValueOpt
			End If
			Return constantValueOpt
		End Function

		Public Function GetConversion(ByVal node As SyntaxNode, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Conversion
			Dim implicitConversion As Conversion
			Dim typeInfoWorker As VisualBasicTypeInfo
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (expressionSyntax Is Nothing) Then
				Dim attributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax)
				If (attributeSyntax Is Nothing) Then
					implicitConversion = VisualBasicTypeInfo.None.ImplicitConversion
				Else
					typeInfoWorker = Me.GetTypeInfoWorker(attributeSyntax, cancellationToken)
					implicitConversion = typeInfoWorker.ImplicitConversion
				End If
			Else
				typeInfoWorker = Me.GetTypeInfoWorker(expressionSyntax, cancellationToken)
				implicitConversion = typeInfoWorker.ImplicitConversion
			End If
			Return implicitConversion
		End Function

		Friend MustOverride Function GetCrefReferenceSymbolInfo(ByVal crefReference As CrefReferenceSyntax, ByVal options As VBSemanticModel.SymbolInfoOptions, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SymbolInfo

		Public Overridable Function GetDeclaredSymbol(ByVal identifierSyntax As ModifiedIdentifierSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ISymbol
			Dim symbol As ISymbol
			If (identifierSyntax Is Nothing) Then
				Throw New ArgumentNullException("identifierSyntax")
			End If
			If (Not Me.IsInTree(identifierSyntax)) Then
				Throw New ArgumentException(VBResources.IdentifierSyntaxNotWithinSyntaxTree)
			End If
			Dim enclosingBinder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me.GetEnclosingBinder(identifierSyntax.SpanStart)
			Dim blockBaseBinder As Microsoft.CodeAnalysis.VisualBasic.BlockBaseBinder = TryCast(VBSemanticModel.StripSemanticModelBinder(enclosingBinder), Microsoft.CodeAnalysis.VisualBasic.BlockBaseBinder)
			If (blockBaseBinder IsNot Nothing) Then
				Dim instance As LookupResult = LookupResult.GetInstance()
				Try
					Dim valueText As String = identifierSyntax.Identifier.ValueText
					Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
					enclosingBinder.Lookup(instance, valueText, 0, LookupOptions.[Default], discarded)
					If (instance.IsGood) Then
						Dim item As LocalSymbol = TryCast(instance.Symbols(0), LocalSymbol)
						If (item IsNot Nothing AndAlso item.IdentifierToken = identifierSyntax.Identifier) Then
							symbol = item
							Return symbol
						End If
					End If
				Finally
					instance.Free()
				End Try
				Dim enumerator As ImmutableArray(Of LocalSymbol).Enumerator = blockBaseBinder.Locals.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As LocalSymbol = enumerator.Current
					If (current.IdentifierToken <> identifierSyntax.Identifier) Then
						Continue While
					End If
					symbol = current
					Return symbol
				End While
			End If
			symbol = Nothing
			Return symbol
		End Function

		Public Function GetDeclaredSymbol(ByVal elementSyntax As TupleElementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ISymbol
			Dim symbol As ISymbol
			Dim symbol1 As ISymbol
			Me.CheckSyntaxNode(elementSyntax)
			Dim parent As TupleTypeSyntax = TryCast(elementSyntax.Parent, TupleTypeSyntax)
			If (parent Is Nothing) Then
				symbol = Nothing
			Else
				Dim tupleTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleTypeSymbol = TryCast(Me.GetSymbolInfo(parent, cancellationToken).Symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleTypeSymbol)
				If (tupleTypeSymbol IsNot Nothing) Then
					Dim tupleElements As ImmutableArray(Of FieldSymbol) = tupleTypeSymbol.TupleElements
					Dim elements As SeparatedSyntaxList(Of TupleElementSyntax) = parent.Elements
					symbol1 = System.Linq.ImmutableArrayExtensions.ElementAtOrDefault(Of FieldSymbol)(tupleElements, elements.IndexOf(elementSyntax))
				Else
					symbol1 = Nothing
				End If
				symbol = symbol1
			End If
			Return symbol
		End Function

		Public Overridable Function GetDeclaredSymbol(ByVal fieldInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldInitializerSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IPropertySymbol
			If (fieldInitializerSyntax Is Nothing) Then
				Throw New ArgumentNullException("fieldInitializerSyntax")
			End If
			If (Not Me.IsInTree(fieldInitializerSyntax)) Then
				Throw New ArgumentException(VBResources.FieldInitializerSyntaxNotWithinSyntaxTree)
			End If
			Return Nothing
		End Function

		Public Overridable Function GetDeclaredSymbol(ByVal anonymousObjectCreationExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AnonymousObjectCreationExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As INamedTypeSymbol
			If (anonymousObjectCreationExpressionSyntax Is Nothing) Then
				Throw New ArgumentNullException("anonymousObjectCreationExpressionSyntax")
			End If
			If (Not Me.IsInTree(anonymousObjectCreationExpressionSyntax)) Then
				Throw New ArgumentException(VBResources.AnonymousObjectCreationExpressionSyntaxNotWithinTree)
			End If
			Return Nothing
		End Function

		Public Overridable Function GetDeclaredSymbol(ByVal rangeVariableSyntax As ExpressionRangeVariableSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IRangeVariableSymbol
			If (rangeVariableSyntax Is Nothing) Then
				Throw New ArgumentNullException("rangeVariableSyntax")
			End If
			If (Not Me.IsInTree(rangeVariableSyntax)) Then
				Throw New ArgumentException(VBResources.RangeVariableSyntaxNotWithinSyntaxTree)
			End If
			Return Nothing
		End Function

		Public Overridable Function GetDeclaredSymbol(ByVal rangeVariableSyntax As CollectionRangeVariableSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IRangeVariableSymbol
			If (rangeVariableSyntax Is Nothing) Then
				Throw New ArgumentNullException("rangeVariableSyntax")
			End If
			If (Not Me.IsInTree(rangeVariableSyntax)) Then
				Throw New ArgumentException(VBResources.RangeVariableSyntaxNotWithinSyntaxTree)
			End If
			Return Nothing
		End Function

		Public Overridable Function GetDeclaredSymbol(ByVal rangeVariableSyntax As AggregationRangeVariableSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IRangeVariableSymbol
			If (rangeVariableSyntax Is Nothing) Then
				Throw New ArgumentNullException("rangeVariableSyntax")
			End If
			If (Not Me.IsInTree(rangeVariableSyntax)) Then
				Throw New ArgumentException(VBResources.RangeVariableSyntaxNotWithinSyntaxTree)
			End If
			Return Nothing
		End Function

		Public Overridable Function GetDeclaredSymbol(ByVal declarationSyntax As LabelStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ILabelSymbol
			Dim labelSymbol As ILabelSymbol
			If (declarationSyntax Is Nothing) Then
				Throw New ArgumentNullException("declarationSyntax")
			End If
			If (Not Me.IsInTree(declarationSyntax)) Then
				Throw New ArgumentException(VBResources.DeclarationSyntaxNotWithinSyntaxTree)
			End If
			Dim blockBaseBinder As Microsoft.CodeAnalysis.VisualBasic.BlockBaseBinder = TryCast(VBSemanticModel.StripSemanticModelBinder(Me.GetEnclosingBinder(declarationSyntax.SpanStart)), Microsoft.CodeAnalysis.VisualBasic.BlockBaseBinder)
			If (blockBaseBinder IsNot Nothing) Then
				Dim labelSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol = blockBaseBinder.LookupLabelByNameToken(declarationSyntax.LabelToken)
				If (labelSymbol1 Is Nothing) Then
					labelSymbol = Nothing
					Return labelSymbol
				End If
				labelSymbol = labelSymbol1
				Return labelSymbol
			End If
			labelSymbol = Nothing
			Return labelSymbol
		End Function

		Public MustOverride Function GetDeclaredSymbol(ByVal declarationSyntax As EnumMemberDeclarationSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IFieldSymbol

		Public MustOverride Function GetDeclaredSymbol(ByVal declarationSyntax As TypeStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As INamedTypeSymbol

		Public Function GetDeclaredSymbol(ByVal declarationSyntax As TypeBlockSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As INamedTypeSymbol
			Return Me.GetDeclaredSymbol(declarationSyntax.BlockStatement, cancellationToken)
		End Function

		Public MustOverride Function GetDeclaredSymbol(ByVal declarationSyntax As EnumStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As INamedTypeSymbol

		Public Function GetDeclaredSymbol(ByVal declarationSyntax As EnumBlockSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As INamedTypeSymbol
			Return Me.GetDeclaredSymbol(declarationSyntax.EnumStatement, cancellationToken)
		End Function

		Public MustOverride Function GetDeclaredSymbol(ByVal declarationSyntax As NamespaceStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As INamespaceSymbol

		Public Function GetDeclaredSymbol(ByVal declarationSyntax As NamespaceBlockSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As INamespaceSymbol
			Return Me.GetDeclaredSymbol(declarationSyntax.NamespaceStatement, cancellationToken)
		End Function

		Friend MustOverride Function GetDeclaredSymbol(ByVal declarationSyntax As MethodBaseSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ISymbol

		Public MustOverride Function GetDeclaredSymbol(ByVal parameter As ParameterSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IParameterSymbol

		Public MustOverride Function GetDeclaredSymbol(ByVal typeParameter As TypeParameterSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ITypeParameterSymbol

		Public Function GetDeclaredSymbol(ByVal declarationSyntax As DelegateStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As NamedTypeSymbol
			Return DirectCast(Me.GetDeclaredSymbol(DirectCast(declarationSyntax, MethodBaseSyntax), cancellationToken), NamedTypeSymbol)
		End Function

		Public Function GetDeclaredSymbol(ByVal declarationSyntax As SubNewStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IMethodSymbol
			Return DirectCast(Me.GetDeclaredSymbol(DirectCast(declarationSyntax, MethodBaseSyntax), cancellationToken), MethodSymbol)
		End Function

		Public Function GetDeclaredSymbol(ByVal declarationSyntax As MethodStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IMethodSymbol
			Return DirectCast(Me.GetDeclaredSymbol(DirectCast(declarationSyntax, MethodBaseSyntax), cancellationToken), MethodSymbol)
		End Function

		Public Function GetDeclaredSymbol(ByVal declarationSyntax As DeclareStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IMethodSymbol
			Return DirectCast(Me.GetDeclaredSymbol(DirectCast(declarationSyntax, MethodBaseSyntax), cancellationToken), MethodSymbol)
		End Function

		Public Function GetDeclaredSymbol(ByVal declarationSyntax As OperatorStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IMethodSymbol
			Return DirectCast(Me.GetDeclaredSymbol(DirectCast(declarationSyntax, MethodBaseSyntax), cancellationToken), MethodSymbol)
		End Function

		Public Function GetDeclaredSymbol(ByVal declarationSyntax As MethodBlockBaseSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IMethodSymbol
			Return DirectCast(Me.GetDeclaredSymbol(declarationSyntax.BlockStatement, cancellationToken), MethodSymbol)
		End Function

		Public Function GetDeclaredSymbol(ByVal declarationSyntax As PropertyStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IPropertySymbol
			Return DirectCast(Me.GetDeclaredSymbol(DirectCast(declarationSyntax, MethodBaseSyntax), cancellationToken), PropertySymbol)
		End Function

		Public Function GetDeclaredSymbol(ByVal declarationSyntax As EventStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IEventSymbol
			Return DirectCast(Me.GetDeclaredSymbol(DirectCast(declarationSyntax, MethodBaseSyntax), cancellationToken), EventSymbol)
		End Function

		Public Function GetDeclaredSymbol(ByVal declarationSyntax As PropertyBlockSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IPropertySymbol
			Return Me.GetDeclaredSymbol(declarationSyntax.PropertyStatement, cancellationToken)
		End Function

		Public Function GetDeclaredSymbol(ByVal declarationSyntax As EventBlockSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IEventSymbol
			Return Me.GetDeclaredSymbol(declarationSyntax.EventStatement, cancellationToken)
		End Function

		Public Function GetDeclaredSymbol(ByVal declarationSyntax As CatchStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ILocalSymbol
			Dim localSymbol As ILocalSymbol
			Dim catchBlockBinder As Microsoft.CodeAnalysis.VisualBasic.CatchBlockBinder = TryCast(VBSemanticModel.StripSemanticModelBinder(Me.GetEnclosingBinder(declarationSyntax.SpanStart)), Microsoft.CodeAnalysis.VisualBasic.CatchBlockBinder)
			If (catchBlockBinder Is Nothing) Then
				localSymbol = Nothing
			Else
				localSymbol = System.Linq.ImmutableArrayExtensions.FirstOrDefault(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)(catchBlockBinder.Locals)
			End If
			Return localSymbol
		End Function

		Public Function GetDeclaredSymbol(ByVal declarationSyntax As AccessorStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IMethodSymbol
			Return DirectCast(Me.GetDeclaredSymbol(DirectCast(declarationSyntax, MethodBaseSyntax), cancellationToken), MethodSymbol)
		End Function

		Public MustOverride Function GetDeclaredSymbol(ByVal declarationSyntax As SimpleImportsClauseSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IAliasSymbol

		Protected NotOverridable Overrides Function GetDeclaredSymbolCore(ByVal declaration As SyntaxNode, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ISymbol
			Dim declaredSymbol As ISymbol
			cancellationToken.ThrowIfCancellationRequested()
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = DirectCast(declaration, Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = visualBasicSyntaxNode.Kind()
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LabelStatement) Then
				If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter) Then
					Select Case syntaxKind
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause
							declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, SimpleImportsClauseSyntax), cancellationToken)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndAddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRemoveHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsStatement
						Case 64
						Case 65
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList
						Case 68
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterSingleConstraintClause
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterMultipleConstraintClause
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeConstraint
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSyncLockStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParameterList
						Case 100
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterSingleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsClause
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesClause
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeywordEventContainer
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsEventContainer
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsPropertyEventContainer
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesClauseItem
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IncompleteMember
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FieldDeclaration
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsClause
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesClause
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariableDeclarator
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleAsClause
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsNewClause
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectMemberInitializer
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectCollectionInitializer
							GoTo Label0
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock
							declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, NamespaceBlockSyntax), cancellationToken)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceStatement
							declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, NamespaceStatementSyntax), cancellationToken)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassBlock
							declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, TypeBlockSyntax), cancellationToken)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumBlock
							declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, EnumBlockSyntax), cancellationToken)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassStatement
							declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax), cancellationToken)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumStatement
							declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, EnumStatementSyntax), cancellationToken)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameter
							declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, TypeParameterSyntax), cancellationToken)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration
							declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, EnumMemberDeclarationSyntax), cancellationToken)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorBlock
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorBlock
							declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, MethodBlockBaseSyntax), cancellationToken)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock
							declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, PropertyBlockSyntax), cancellationToken)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock
							declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, EventBlockSyntax), cancellationToken)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionStatement
							declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, MethodStatementSyntax), cancellationToken)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubNewStatement
							declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, SubNewStatementSyntax), cancellationToken)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement
							declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax), cancellationToken)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement
							declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, DelegateStatementSyntax), cancellationToken)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement
							declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, EventStatementSyntax), cancellationToken)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement
							declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, PropertyStatementSyntax), cancellationToken)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorStatement
							declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, AccessorStatementSyntax), cancellationToken)
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferredFieldInitializer
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer
							declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, FieldInitializerSyntax), cancellationToken)
							Exit Select
						Case Else
							If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter) Then
								declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, ParameterSyntax), cancellationToken)
								Exit Select
							Else
								GoTo Label0
							End If
					End Select
				ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModifiedIdentifier) Then
					declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, ModifiedIdentifierSyntax), cancellationToken)
				Else
					If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LabelStatement) Then
						GoTo Label0
					End If
					declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, LabelStatementSyntax), cancellationToken)
				End If
			ElseIf (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnonymousObjectCreationExpression) Then
				Select Case syntaxKind
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CollectionRangeVariable
						declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, CollectionRangeVariableSyntax), cancellationToken)
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExpressionRangeVariable
						declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, ExpressionRangeVariableSyntax), cancellationToken)
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregationRangeVariable
						declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, AggregationRangeVariableSyntax), cancellationToken)
						Exit Select
					Case Else
						If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypedTupleElement) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
							declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, TupleElementSyntax), cancellationToken)
							Exit Select
						Else
							GoTo Label0
						End If
				End Select
			ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CatchStatement) Then
				declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, CatchStatementSyntax), cancellationToken)
			Else
				If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnonymousObjectCreationExpression) Then
					GoTo Label0
				End If
				declaredSymbol = Me.GetDeclaredSymbol(DirectCast(visualBasicSyntaxNode, AnonymousObjectCreationExpressionSyntax), cancellationToken)
			End If
			Return declaredSymbol
		Label0:
			Dim typeStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax = TryCast(visualBasicSyntaxNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeStatementSyntax)
			If (typeStatementSyntax Is Nothing) Then
				Dim methodBaseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax = TryCast(visualBasicSyntaxNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax)
				If (methodBaseSyntax Is Nothing) Then
					declaredSymbol = Nothing
					Return declaredSymbol
				Else
					declaredSymbol = Me.GetDeclaredSymbol(methodBaseSyntax, cancellationToken)
					Return declaredSymbol
				End If
			Else
				declaredSymbol = Me.GetDeclaredSymbol(typeStatementSyntax, cancellationToken)
				Return declaredSymbol
			End If
		End Function

		Friend MustOverride Function GetDeclaredSymbols(ByVal declarationSyntax As FieldDeclarationSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of ISymbol)

		Protected NotOverridable Overrides Function GetDeclaredSymbolsCore(ByVal declaration As SyntaxNode, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of ISymbol)
			Dim declaredSymbols As ImmutableArray(Of ISymbol)
			cancellationToken.ThrowIfCancellationRequested()
			Dim fieldDeclarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax = TryCast(declaration, Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax)
			If (fieldDeclarationSyntax Is Nothing) Then
				Dim declaredSymbolCore As ISymbol = Me.GetDeclaredSymbolCore(declaration, cancellationToken)
				declaredSymbols = If(declaredSymbolCore Is Nothing, ImmutableArray.Create(Of ISymbol)(), ImmutableArray.Create(Of ISymbol)(declaredSymbolCore))
			Else
				declaredSymbols = Me.GetDeclaredSymbols(fieldDeclarationSyntax, cancellationToken)
			End If
			Return declaredSymbols
		End Function

		Friend MustOverride Function GetEnclosingBinder(ByVal position As Integer) As Microsoft.CodeAnalysis.VisualBasic.Binder

		Public Shadows Function GetEnclosingSymbol(ByVal position As Integer, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ISymbol
			Me.CheckPosition(position)
			Dim enclosingBinder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me.GetEnclosingBinder(position)
			If (enclosingBinder Is Nothing) Then
				Return Nothing
			End If
			Return enclosingBinder.ContainingMember
		End Function

		Protected NotOverridable Overrides Function GetEnclosingSymbolCore(ByVal position As Integer, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ISymbol
			Return Me.GetEnclosingSymbol(position, cancellationToken)
		End Function

		Friend MustOverride Function GetExpressionConstantValue(ByVal node As ExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ConstantValue

		Friend MustOverride Function GetExpressionMemberGroup(ByVal node As ExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of Symbol)

		Friend MustOverride Function GetExpressionSymbolInfo(ByVal node As ExpressionSyntax, ByVal options As VBSemanticModel.SymbolInfoOptions, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SymbolInfo

		Friend MustOverride Function GetExpressionTypeInfo(ByVal node As ExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As VisualBasicTypeInfo

		Public Function GetForEachStatementInfo(ByVal node As ForEachStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.ForEachStatementInfo
			Dim forEachStatementInfo As Microsoft.CodeAnalysis.VisualBasic.ForEachStatementInfo
			forEachStatementInfo = If(node.Parent Is Nothing OrElse node.Parent.Kind() <> SyntaxKind.ForEachBlock, New Microsoft.CodeAnalysis.VisualBasic.ForEachStatementInfo(), Me.GetForEachStatementInfoWorker(DirectCast(node.Parent, ForEachBlockSyntax)))
			Return forEachStatementInfo
		End Function

		Public Function GetForEachStatementInfo(ByVal node As ForEachBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.ForEachStatementInfo
			Dim forEachStatementInfo As Microsoft.CodeAnalysis.VisualBasic.ForEachStatementInfo
			forEachStatementInfo = If(node.Kind() <> SyntaxKind.ForEachBlock, New Microsoft.CodeAnalysis.VisualBasic.ForEachStatementInfo(), Me.GetForEachStatementInfoWorker(node))
			Return forEachStatementInfo
		End Function

		Friend MustOverride Function GetForEachStatementInfoWorker(ByVal node As ForEachBlockSyntax) As ForEachStatementInfo

		Friend MustOverride Function GetInvokeSummaryForRaiseEvent(ByVal node As RaiseEventStatementSyntax) As BoundNodeSummary

		Friend MustOverride Function GetLetClauseSymbolInfo(ByVal node As ExpressionRangeVariableSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SymbolInfo

		Public Function GetMemberGroup(ByVal expression As ExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of ISymbol)
			Dim empty As ImmutableArray(Of ISymbol)
			Me.CheckSyntaxNode(expression)
			If (Not Me.CanGetSemanticInfo(expression, False)) Then
				empty = ImmutableArray(Of ISymbol).Empty
			Else
				empty = StaticCast(Of ISymbol).From(Of Symbol)(Me.GetExpressionMemberGroup(expression, cancellationToken))
			End If
			Return empty
		End Function

		Public Function GetMemberGroup(ByVal attribute As AttributeSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of ISymbol)
			Dim empty As ImmutableArray(Of ISymbol)
			Me.CheckSyntaxNode(attribute)
			If (Not Me.CanGetSemanticInfo(attribute, False)) Then
				empty = ImmutableArray(Of ISymbol).Empty
			Else
				empty = StaticCast(Of ISymbol).From(Of Symbol)(Me.GetAttributeMemberGroup(attribute, cancellationToken))
			End If
			Return empty
		End Function

		Protected NotOverridable Overrides Function GetMemberGroupCore(ByVal node As SyntaxNode, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of ISymbol)
			Return Me.GetMemberGroupForNode(node, cancellationToken)
		End Function

		Friend Function GetMemberGroupForNode(ByVal boundNodes As BoundNodeSummary, ByVal binderOpt As Microsoft.CodeAnalysis.VisualBasic.Binder) As ImmutableArray(Of Symbol)
			Dim lookupResultKind As Microsoft.CodeAnalysis.VisualBasic.LookupResultKind = Microsoft.CodeAnalysis.VisualBasic.LookupResultKind.Empty
			Dim symbols As ImmutableArray(Of Symbol) = New ImmutableArray(Of Symbol)()
			Me.GetSemanticSymbols(boundNodes, binderOpt, VBSemanticModel.SymbolInfoOptions.DefaultOptions, lookupResultKind, symbols)
			Return symbols
		End Function

		Private Function GetMemberGroupForNode(ByVal node As SyntaxNode, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of ISymbol)
			Dim empty As ImmutableArray(Of ISymbol)
			If (node Is Nothing) Then
				Throw New ArgumentNullException("node")
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (expressionSyntax Is Nothing) Then
				Dim attributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax)
				If (attributeSyntax Is Nothing) Then
					empty = ImmutableArray(Of ISymbol).Empty
				Else
					empty = Me.GetMemberGroup(attributeSyntax, cancellationToken)
				End If
			Else
				empty = Me.GetMemberGroup(expressionSyntax, cancellationToken)
			End If
			Return empty
		End Function

		Private Shared Function GetMeParameter(ByVal referenceType As TypeSymbol, ByVal containingType As TypeSymbol, ByVal containingMember As Symbol, ByRef resultKind As LookupResultKind) As ParameterSymbol
			Dim meParameterSymbol As ParameterSymbol
			Dim meParameter As ParameterSymbol
			If (containingMember Is Nothing OrElse containingType Is Nothing) Then
				resultKind = LookupResultKind.NotReferencable
				meParameterSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.MeParameterSymbol(containingMember, referenceType)
			Else
				Dim kind As SymbolKind = containingMember.Kind
				If (kind <> SymbolKind.Field AndAlso kind <> SymbolKind.Method AndAlso kind <> SymbolKind.[Property]) Then
					meParameter = New Microsoft.CodeAnalysis.VisualBasic.Symbols.MeParameterSymbol(containingMember, referenceType)
					resultKind = LookupResultKind.NotReferencable
				ElseIf (containingMember.IsShared) Then
					resultKind = LookupResultKind.MustNotBeInstance
					meParameter = New Microsoft.CodeAnalysis.VisualBasic.Symbols.MeParameterSymbol(containingMember, containingType)
				ElseIf (Not TypeSymbol.Equals(referenceType, ErrorTypeSymbol.UnknownResultType, TypeCompareKind.ConsiderEverything)) Then
					resultKind = LookupResultKind.Good
					meParameter = containingMember.GetMeParameter()
				Else
					meParameter = New Microsoft.CodeAnalysis.VisualBasic.Symbols.MeParameterSymbol(containingMember, containingType)
					resultKind = LookupResultKind.NotReferencable
				End If
				meParameterSymbol = meParameter
			End If
			Return meParameterSymbol
		End Function

		Friend Function GetMessage(ByVal position As Integer) As String
			Return [String].Format("{0}: at {1}", CObj(Me.SyntaxTree.FilePath), position)
		End Function

		Friend Function GetMessage(ByVal node As VisualBasicSyntaxNode) As String
			Dim filePath As String
			If (node IsNot Nothing) Then
				Dim str As String = Me.SyntaxTree.FilePath
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = node.Kind()
				filePath = [String].Format("{0}: {1} ({2})", CObj(str), syntaxKind.ToString(), node.Position)
			Else
				filePath = Me.SyntaxTree.FilePath
			End If
			Return filePath
		End Function

		Friend Function GetMessage(ByVal node As VisualBasicSyntaxNode, ByVal position As Integer) As String
			Dim filePath As String
			If (node IsNot Nothing) Then
				Dim str() As [Object] = { Me.SyntaxTree.FilePath, Nothing, Nothing, Nothing }
				str(1) = node.Kind().ToString()
				str(2) = node.Position
				str(3) = position
				filePath = [String].Format("{0}: {1} ({2}) at {3}", str)
			Else
				filePath = Me.SyntaxTree.FilePath
			End If
			Return filePath
		End Function

		Friend Function GetMessage(ByVal firstStatement As StatementSyntax, ByVal lastStatement As StatementSyntax) As String
			Dim str As String
			str = If(firstStatement Is Nothing OrElse lastStatement Is Nothing, Me.SyntaxTree.FilePath, [String].Format("{0}: {1} to {2}", CObj(Me.SyntaxTree.FilePath), firstStatement.Position, lastStatement.EndPosition))
			Return str
		End Function

		Friend Function GetMessage(ByVal expression As ExpressionSyntax, ByVal type As TypeSymbol) As String
			Dim filePath As String
			If (expression Is Nothing OrElse type Is Nothing) Then
				filePath = Me.SyntaxTree.FilePath
			Else
				Dim str() As [Object] = { Me.SyntaxTree.FilePath, Nothing, Nothing, Nothing, Nothing }
				str(1) = expression.Kind().ToString()
				str(2) = expression.Position
				str(3) = type.TypeKind.ToString()
				str(4) = type.Name
				filePath = [String].Format("{0}: {1} ({2}) -> {3} {4}", str)
			End If
			Return filePath
		End Function

		Friend Function GetMessage(ByVal expression As ExpressionSyntax, ByVal type As TypeSymbol, ByVal position As Integer) As String
			Dim filePath As String
			If (expression Is Nothing OrElse type Is Nothing) Then
				filePath = Me.SyntaxTree.FilePath
			Else
				Dim str() As [Object] = { Me.SyntaxTree.FilePath, Nothing, Nothing, Nothing, Nothing, Nothing }
				str(1) = expression.Kind().ToString()
				str(2) = expression.Position
				str(3) = type.TypeKind.ToString()
				str(4) = type.Name
				str(5) = position
				filePath = [String].Format("{0}: {1} ({2}) -> {3} {4} at {5}", str)
			End If
			Return filePath
		End Function

		Friend Function GetMessage(ByVal expression As ExpressionSyntax, ByVal [option] As SpeculativeBindingOption, ByVal position As Integer) As String
			Dim filePath As String
			If (expression IsNot Nothing) Then
				Dim str() As [Object] = { Me.SyntaxTree.FilePath, Nothing, Nothing, Nothing, Nothing }
				str(1) = expression.Kind().ToString()
				str(2) = expression.Position
				str(3) = position
				str(4) = [option].ToString()
				filePath = [String].Format("{0}: {1} ({2}) at {3} ({4})", str)
			Else
				filePath = Me.SyntaxTree.FilePath
			End If
			Return filePath
		End Function

		Friend Function GetMessage(ByVal name As String, ByVal [option] As LookupOptions, ByVal position As Integer) As String
			Return [String].Format("{0}: {1} at {2} ({3})", New [Object]() { Me.SyntaxTree.FilePath, name, position, [option].ToString() })
		End Function

		Friend Function GetMessage(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal position As Integer) As String
			Dim filePath As String
			If (symbol IsNot Nothing) Then
				Dim str() As [Object] = { Me.SyntaxTree.FilePath, Nothing, Nothing, Nothing }
				str(1) = symbol.Kind.ToString()
				str(2) = symbol.Name
				str(3) = position
				filePath = [String].Format("{0}: {1} {2} at {3}", str)
			Else
				filePath = Me.SyntaxTree.FilePath
			End If
			Return filePath
		End Function

		Friend Function GetMessage(ByVal stage As CompilationStage) As String
			Return [String].Format("{0} ({1})", CObj(Me.SyntaxTree.FilePath), stage.ToString())
		End Function

		Private Function GetNamedArgumentSymbolInfo(ByVal identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax, ByVal cancellationToken As System.Threading.CancellationToken) As SymbolInfo
			Dim none As SymbolInfo
			Dim valueText As String = identifierNameSyntax.Identifier.ValueText
			If (valueText.Length = 0) Then
				none = SymbolInfo.None
			ElseIf (identifierNameSyntax.Parent.Parent.Parent.Parent.Kind() <> SyntaxKind.RaiseEventStatement) Then
				Dim parent As ExpressionSyntax = DirectCast(identifierNameSyntax.Parent.Parent.Parent.Parent, ExpressionSyntax)
				Dim expressionSymbolInfo As SymbolInfo = Me.GetExpressionSymbolInfo(parent, VBSemanticModel.SymbolInfoOptions.DefaultOptions, cancellationToken)
				none = Me.FindNameParameterInfo(DirectCast(expressionSymbolInfo.GetAllSymbols(), IEnumerable).Cast(Of Symbol)().ToImmutableArray(), valueText, expressionSymbolInfo.CandidateReason)
			Else
				Dim raiseEventStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.RaiseEventStatementSyntax = DirectCast(identifierNameSyntax.Parent.Parent.Parent.Parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.RaiseEventStatementSyntax)
				none = Me.GetNamedArgumentSymbolInfoInRaiseEvent(valueText, raiseEventStatementSyntax)
			End If
			Return none
		End Function

		Private Function GetNamedArgumentSymbolInfoInRaiseEvent(ByVal argumentName As String, ByVal containingRaiseEvent As RaiseEventStatementSyntax) As SymbolInfo
			Dim invokeSummaryForRaiseEvent As BoundNodeSummary = Me.GetInvokeSummaryForRaiseEvent(containingRaiseEvent)
			Dim lookupResultKind As Microsoft.CodeAnalysis.VisualBasic.LookupResultKind = Microsoft.CodeAnalysis.VisualBasic.LookupResultKind.Empty
			Dim symbols As ImmutableArray(Of Symbol) = New ImmutableArray(Of Symbol)()
			Dim semanticSymbols As ImmutableArray(Of Symbol) = Me.GetSemanticSymbols(invokeSummaryForRaiseEvent, Nothing, VBSemanticModel.SymbolInfoOptions.DefaultOptions, lookupResultKind, symbols)
			Return Me.FindNameParameterInfo(semanticSymbols, argumentName, If(lookupResultKind = Microsoft.CodeAnalysis.VisualBasic.LookupResultKind.Good, CandidateReason.None, lookupResultKind.ToCandidateReason()))
		End Function

		Public NotOverridable Overrides Function GetNullableContext(ByVal position As Integer) As NullableContext
			Return NullableContext.ContextInherited
		End Function

		Protected Overrides Function GetOperationCore(ByVal node As SyntaxNode, ByVal cancellationToken As System.Threading.CancellationToken) As IOperation
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)
			Me.CheckSyntaxNode(visualBasicSyntaxNode)
			Return Me.GetOperationWorker(visualBasicSyntaxNode, cancellationToken)
		End Function

		Friend Overridable Function GetOperationWorker(ByVal node As VisualBasicSyntaxNode, ByVal cancellationToken As System.Threading.CancellationToken) As IOperation
			Return Nothing
		End Function

		Friend MustOverride Function GetOrderingSymbolInfo(ByVal node As OrderingSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SymbolInfo

		Public Function GetPreprocessingSymbolInfo(ByVal node As IdentifierNameSyntax) As VisualBasicPreprocessingSymbolInfo
			Dim none As VisualBasicPreprocessingSymbolInfo
			Me.CheckSyntaxNode(node)
			If (Not SyntaxFacts.IsWithinPreprocessorConditionalExpression(node)) Then
				none = VisualBasicPreprocessingSymbolInfo.None
			Else
				Dim preprocessingSymbolInfo As VisualBasicPreprocessingSymbolInfo = node.SyntaxTree.GetPreprocessingSymbolInfo(node)
				If (preprocessingSymbolInfo.Symbol Is Nothing) Then
					Dim identifier As SyntaxToken = node.Identifier
					none = New VisualBasicPreprocessingSymbolInfo(New PreprocessingSymbol(identifier.ValueText), Nothing, False)
				Else
					none = preprocessingSymbolInfo
				End If
			End If
			Return none
		End Function

		Protected NotOverridable Overrides Function GetPreprocessingSymbolInfoCore(ByVal node As SyntaxNode) As PreprocessingSymbolInfo
			Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)
			Return If(identifierNameSyntax Is Nothing, New PreprocessingSymbolInfo(), Me.GetPreprocessingSymbolInfo(identifierNameSyntax))
		End Function

		Friend MustOverride Function GetQueryClauseSymbolInfo(ByVal node As QueryClauseSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SymbolInfo

		Private Function GetSemanticSymbols(ByVal boundNodes As BoundNodeSummary, ByVal binderOpt As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal options As VBSemanticModel.SymbolInfoOptions, ByRef resultKind As LookupResultKind, ByRef memberGroup As ImmutableArray(Of Symbol)) As ImmutableArray(Of Symbol)
			' 
			' Current member / type: System.Collections.Immutable.ImmutableArray`1<Microsoft.CodeAnalysis.VisualBasic.Symbol> Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel::GetSemanticSymbols(Microsoft.CodeAnalysis.VisualBasic.BoundNodeSummary,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel/SymbolInfoOptions,Microsoft.CodeAnalysis.VisualBasic.LookupResultKind&,System.Collections.Immutable.ImmutableArray`1<Microsoft.CodeAnalysis.VisualBasic.Symbol>&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.VisualBasic.Symbol> GetSemanticSymbols(Microsoft.CodeAnalysis.VisualBasic.BoundNodeSummary,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel/SymbolInfoOptions,Microsoft.CodeAnalysis.VisualBasic.LookupResultKind&,System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.VisualBasic.Symbol>&)
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

		Private Sub GetSemanticSymbolsForLateBoundInvocation(ByVal lateInvocation As BoundLateInvocation, ByVal symbolsBuilder As ArrayBuilder(Of Symbol), ByVal memberGroupBuilder As ArrayBuilder(Of Symbol), ByRef resultKind As LookupResultKind)
			resultKind = LookupResultKind.LateBound
			Dim methodOrPropertyGroupOpt As BoundMethodOrPropertyGroup = lateInvocation.MethodOrPropertyGroupOpt
			If (methodOrPropertyGroupOpt IsNot Nothing) Then
				methodOrPropertyGroupOpt.GetExpressionSymbols(memberGroupBuilder)
				methodOrPropertyGroupOpt.GetExpressionSymbols(symbolsBuilder)
			End If
		End Sub

		Private Sub GetSemanticSymbolsForLateBoundMemberAccess(ByVal boundNodes As BoundNodeSummary, ByVal symbolsBuilder As ArrayBuilder(Of Symbol), ByVal memberGroupBuilder As ArrayBuilder(Of Symbol), ByRef resultKind As LookupResultKind)
			If (boundNodes.LowestBoundNodeOfSyntacticParent Is Nothing OrElse boundNodes.LowestBoundNodeOfSyntacticParent.Kind <> BoundKind.LateInvocation) Then
				resultKind = LookupResultKind.LateBound
				Return
			End If
			Me.GetSemanticSymbolsForLateBoundInvocation(DirectCast(boundNodes.LowestBoundNodeOfSyntacticParent, BoundLateInvocation), symbolsBuilder, memberGroupBuilder, resultKind)
		End Sub

		Private Sub GetSemanticSymbolsForMethodGroup(ByVal boundNodes As BoundNodeSummary, ByVal symbolsBuilder As ArrayBuilder(Of Symbol), ByVal memberGroupBuilder As ArrayBuilder(Of Symbol), ByRef resultKind As LookupResultKind)
			Dim lowestBoundNode As BoundMethodGroup = DirectCast(boundNodes.LowestBoundNode, BoundMethodGroup)
			resultKind = lowestBoundNode.ResultKind
			lowestBoundNode.GetExpressionSymbols(memberGroupBuilder)
			Dim flag As Boolean = False
			If (boundNodes.LowestBoundNodeOfSyntacticParent IsNot Nothing) Then
				Dim kind As BoundKind = boundNodes.LowestBoundNodeOfSyntacticParent.Kind
				If (kind <= BoundKind.[Call]) Then
					If (kind = BoundKind.BadExpression) Then
						Dim lowestBoundNodeOfSyntacticParent As BoundBadExpression = DirectCast(boundNodes.LowestBoundNodeOfSyntacticParent, BoundBadExpression)
						symbolsBuilder.AddRange(lowestBoundNodeOfSyntacticParent.Symbols.Where(Function(sym As Symbol) memberGroupBuilder.Contains(sym)))
						If (symbolsBuilder.Count > 0) Then
							resultKind = lowestBoundNodeOfSyntacticParent.ResultKind
							flag = True
						End If
					ElseIf (kind = BoundKind.[Call]) Then
						Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundCall = DirectCast(boundNodes.LowestBoundNodeOfSyntacticParent, Microsoft.CodeAnalysis.VisualBasic.BoundCall)
						symbolsBuilder.Add(boundCall.Method)
						If (CInt(boundCall.ResultKind) < CInt(resultKind)) Then
							resultKind = boundCall.ResultKind
						End If
						flag = True
					End If
				ElseIf (kind = BoundKind.DelegateCreationExpression) Then
					Dim boundDelegateCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundDelegateCreationExpression = DirectCast(boundNodes.LowestBoundNodeOfSyntacticParent, Microsoft.CodeAnalysis.VisualBasic.BoundDelegateCreationExpression)
					symbolsBuilder.Add(boundDelegateCreationExpression.Method)
					If (CInt(boundDelegateCreationExpression.ResultKind) < CInt(resultKind)) Then
						resultKind = boundDelegateCreationExpression.ResultKind
					End If
					flag = True
				ElseIf (kind = BoundKind.NameOfOperator) Then
					symbolsBuilder.AddRange(memberGroupBuilder)
					resultKind = LookupResultKind.MemberGroup
					flag = True
				End If
			End If
			If (Not flag) Then
				symbolsBuilder.AddRange(memberGroupBuilder)
				resultKind = LookupResultKind.OverloadResolutionFailure
			End If
			If (CInt(lowestBoundNode.ResultKind) < CInt(resultKind)) Then
				resultKind = lowestBoundNode.ResultKind
			End If
		End Sub

		Private Sub GetSemanticSymbolsForPropertyGroup(ByVal boundNodes As BoundNodeSummary, ByVal symbolsBuilder As ArrayBuilder(Of Symbol), ByVal memberGroupBuilder As ArrayBuilder(Of Symbol), ByRef resultKind As LookupResultKind)
			Dim lowestBoundNode As BoundPropertyGroup = DirectCast(boundNodes.LowestBoundNode, BoundPropertyGroup)
			resultKind = lowestBoundNode.ResultKind
			memberGroupBuilder.AddRange(Of PropertySymbol)(lowestBoundNode.Properties)
			Dim flag As Boolean = False
			If (boundNodes.LowestBoundNodeOfSyntacticParent IsNot Nothing) Then
				Dim kind As BoundKind = boundNodes.LowestBoundNodeOfSyntacticParent.Kind
				If (kind = BoundKind.BadExpression) Then
					Dim lowestBoundNodeOfSyntacticParent As BoundBadExpression = DirectCast(boundNodes.LowestBoundNodeOfSyntacticParent, BoundBadExpression)
					symbolsBuilder.AddRange(lowestBoundNodeOfSyntacticParent.Symbols.Where(Function(sym As Symbol) memberGroupBuilder.Contains(sym)))
					If (symbolsBuilder.Count > 0) Then
						resultKind = lowestBoundNodeOfSyntacticParent.ResultKind
						flag = True
					End If
				ElseIf (kind = BoundKind.PropertyAccess) Then
					Dim boundPropertyAccess As Microsoft.CodeAnalysis.VisualBasic.BoundPropertyAccess = TryCast(boundNodes.LowestBoundNodeOfSyntacticParent, Microsoft.CodeAnalysis.VisualBasic.BoundPropertyAccess)
					If (boundPropertyAccess IsNot Nothing) Then
						symbolsBuilder.Add(boundPropertyAccess.PropertySymbol)
						If (CInt(boundPropertyAccess.ResultKind) < CInt(resultKind)) Then
							resultKind = boundPropertyAccess.ResultKind
						End If
						flag = True
					End If
				ElseIf (kind = BoundKind.NameOfOperator) Then
					symbolsBuilder.AddRange(memberGroupBuilder)
					resultKind = LookupResultKind.MemberGroup
					flag = True
				End If
			End If
			If (Not flag) Then
				symbolsBuilder.AddRange(memberGroupBuilder)
				resultKind = LookupResultKind.OverloadResolutionFailure
			End If
			If (CInt(lowestBoundNode.ResultKind) < CInt(resultKind)) Then
				resultKind = lowestBoundNode.ResultKind
			End If
		End Sub

		Private Function GetSemanticType(ByVal boundNodes As BoundNodeSummary, ByRef convertedType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByRef conversion As Microsoft.CodeAnalysis.VisualBasic.Conversion) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			convertedType = Nothing
			conversion = New Microsoft.CodeAnalysis.VisualBasic.Conversion(Microsoft.CodeAnalysis.VisualBasic.Conversions.Identity)
			Dim lowestBoundNode As BoundExpression = TryCast(boundNodes.LowestBoundNode, BoundExpression)
			Dim highestBoundNode As BoundExpression = TryCast(boundNodes.HighestBoundNode, BoundExpression)
			If (lowestBoundNode Is Nothing) Then
				typeSymbol = Nothing
			ElseIf (boundNodes.LowestBoundNodeOfSyntacticParent Is Nothing OrElse boundNodes.LowestBoundNodeOfSyntacticParent.Syntax.Kind() <> SyntaxKind.ObjectCreationExpression OrElse DirectCast(boundNodes.LowestBoundNodeOfSyntacticParent.Syntax, ObjectCreationExpressionSyntax).Type <> lowestBoundNode.Syntax) Then
				If (lowestBoundNode.Kind <> BoundKind.ArrayCreation OrElse DirectCast(lowestBoundNode, BoundArrayCreation).ArrayLiteralOpt Is Nothing) Then
					type = If(lowestBoundNode.Kind <> BoundKind.ConvertedTupleLiteral, lowestBoundNode.Type, DirectCast(lowestBoundNode, BoundConvertedTupleLiteral).NaturalTypeOpt)
				Else
					type = Nothing
					conversion = New Microsoft.CodeAnalysis.VisualBasic.Conversion(New KeyValuePair(Of ConversionKind, MethodSymbol)(DirectCast(lowestBoundNode, BoundArrayCreation).ArrayLiteralConversion, Nothing))
				End If
				Dim flag As Boolean = False
				If (type = LocalSymbol.UseBeforeDeclarationResultType AndAlso lowestBoundNode.Kind = BoundKind.Local) Then
					flag = True
					type = DirectCast(lowestBoundNode, BoundLocal).LocalSymbol.Type
				End If
				If (highestBoundNode IsNot Nothing AndAlso highestBoundNode.Type IsNot Nothing AndAlso highestBoundNode.Type.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Error]) Then
					convertedType = highestBoundNode.Type
					If ((type Is Nothing OrElse Not type.IsSameTypeIgnoringAll(convertedType)) AndAlso highestBoundNode.Kind = BoundKind.Conversion) Then
						Dim boundConversion As Microsoft.CodeAnalysis.VisualBasic.BoundConversion = DirectCast(highestBoundNode, Microsoft.CodeAnalysis.VisualBasic.BoundConversion)
						If (Not flag OrElse type.IsErrorType()) Then
							conversion = New Microsoft.CodeAnalysis.VisualBasic.Conversion(KeyValuePairUtil.Create(Of ConversionKind, MethodSymbol)(boundConversion.ConversionKind, TryCast(boundConversion.ExpressionSymbol, MethodSymbol)))
						Else
							Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
							conversion = New Microsoft.CodeAnalysis.VisualBasic.Conversion(Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyConversion(type, convertedType, discarded))
						End If
					End If
				End If
				If (type Is Nothing AndAlso TypeOf boundNodes.LowestBoundNodeOfSyntacticParent Is BoundBadExpression) Then
					Dim syntax As SyntaxNode = boundNodes.LowestBoundNodeOfSyntacticParent.Syntax
					If (syntax IsNot Nothing AndAlso syntax = boundNodes.LowestBoundNode.Syntax.Parent AndAlso syntax.Kind() = SyntaxKind.ObjectCreationExpression AndAlso DirectCast(syntax, ObjectCreationExpressionSyntax).Type = boundNodes.LowestBoundNode.Syntax) Then
						type = DirectCast(boundNodes.LowestBoundNodeOfSyntacticParent, BoundBadExpression).Type
					End If
				End If
				If (convertedType Is Nothing) Then
					convertedType = type
				End If
				typeSymbol = type
			Else
				typeSymbol = Nothing
			End If
			Return typeSymbol
		End Function

		Public Function GetSpeculativeAliasInfo(ByVal position As Integer, ByVal nameSyntax As IdentifierNameSyntax, ByVal bindingOption As SpeculativeBindingOption) As IAliasSymbol
			Dim symbol As IAliasSymbol
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Nothing
			Dim speculativelyBoundNodeSummary As BoundNodeSummary = Me.GetSpeculativelyBoundNodeSummary(position, nameSyntax, bindingOption, binder)
			If (speculativelyBoundNodeSummary.LowestBoundNode Is Nothing) Then
				symbol = Nothing
			Else
				Dim symbolInfoForNode As SymbolInfo = Me.GetSymbolInfoForNode(VBSemanticModel.SymbolInfoOptions.PreferTypeToConstructors Or VBSemanticModel.SymbolInfoOptions.PreserveAliases, speculativelyBoundNodeSummary, binder)
				symbol = TryCast(symbolInfoForNode.Symbol, IAliasSymbol)
			End If
			Return symbol
		End Function

		Protected NotOverridable Overrides Function GetSpeculativeAliasInfoCore(ByVal position As Integer, ByVal nameSyntax As SyntaxNode, ByVal bindingOption As SpeculativeBindingOption) As IAliasSymbol
			If (Not TypeOf nameSyntax Is IdentifierNameSyntax) Then
				Return Nothing
			End If
			Return Me.GetSpeculativeAliasInfo(position, DirectCast(nameSyntax, IdentifierNameSyntax), bindingOption)
		End Function

		Private Function GetSpeculativeAttributeBinder(ByVal position As Integer, ByVal attribute As AttributeSyntax) As AttributeBinder
			Me.CheckPosition(position)
			Dim enclosingBinder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me.GetEnclosingBinder(position)
			If (enclosingBinder Is Nothing) Then
				Return Nothing
			End If
			Return BinderBuilder.CreateBinderForAttribute(enclosingBinder.SyntaxTree, enclosingBinder, attribute)
		End Function

		Friend Function GetSpeculativeBinderForExpression(ByVal position As Integer, ByVal expression As ExpressionSyntax, ByVal bindingOption As SpeculativeBindingOption) As Microsoft.CodeAnalysis.VisualBasic.SpeculativeBinder
			Dim speculativeBinder As Microsoft.CodeAnalysis.VisualBasic.SpeculativeBinder
			Dim speculativeBinder1 As Microsoft.CodeAnalysis.VisualBasic.SpeculativeBinder
			Me.CheckPosition(position)
			If (bindingOption <> SpeculativeBindingOption.BindAsTypeOrNamespace OrElse TypeOf expression Is TypeSyntax) Then
				Dim enclosingBinder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me.GetEnclosingBinder(position)
				If (enclosingBinder IsNot Nothing) Then
					speculativeBinder1 = Microsoft.CodeAnalysis.VisualBasic.SpeculativeBinder.Create(enclosingBinder)
				Else
					speculativeBinder1 = Nothing
				End If
				speculativeBinder = speculativeBinder1
			Else
				speculativeBinder = Nothing
			End If
			Return speculativeBinder
		End Function

		Public Function GetSpeculativeConstantValue(ByVal position As Integer, ByVal expression As ExpressionSyntax) As [Optional](Of Object)
			Dim [optional] As [Optional](Of Object)
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Nothing
			Dim speculativelyBoundNodeSummary As BoundNodeSummary = Me.GetSpeculativelyBoundNodeSummary(position, expression, SpeculativeBindingOption.BindAsExpression, binder)
			If (speculativelyBoundNodeSummary.LowestBoundNode IsNot Nothing) Then
				Dim constantValueForNode As ConstantValue = Me.GetConstantValueForNode(speculativelyBoundNodeSummary)
				If (constantValueForNode Is Nothing OrElse constantValueForNode.IsBad) Then
					[optional] = New [Optional](Of Object)()
					Return [optional]
				End If
				[optional] = New [Optional](Of Object)(RuntimeHelpers.GetObjectValue(constantValueForNode.Value))
				Return [optional]
			End If
			[optional] = New [Optional](Of Object)()
			Return [optional]
		End Function

		Public Function GetSpeculativeConversion(ByVal position As Integer, ByVal expression As ExpressionSyntax, ByVal bindingOption As SpeculativeBindingOption) As Conversion
			Return Me.GetSpeculativeTypeInfoWorker(position, expression, bindingOption).ImplicitConversion
		End Function

		Friend Function GetSpeculativelyBoundAttribute(ByVal position As Integer, ByVal attribute As AttributeSyntax, <Out> ByRef binder As Microsoft.CodeAnalysis.VisualBasic.Binder) As Microsoft.CodeAnalysis.VisualBasic.BoundAttribute
			Dim boundAttribute As Microsoft.CodeAnalysis.VisualBasic.BoundAttribute
			binder = Me.GetSpeculativeAttributeBinder(position, attribute)
			If (binder Is Nothing) Then
				boundAttribute = Nothing
			Else
				boundAttribute = binder.BindAttribute(attribute, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded)
			End If
			Return boundAttribute
		End Function

		Private Function GetSpeculativelyBoundAttributeSummary(ByVal position As Integer, ByVal attribute As AttributeSyntax, <Out> ByRef binder As Microsoft.CodeAnalysis.VisualBasic.Binder) As Microsoft.CodeAnalysis.VisualBasic.BoundNodeSummary
			Dim boundNodeSummary As Microsoft.CodeAnalysis.VisualBasic.BoundNodeSummary
			If (attribute Is Nothing) Then
				Throw New ArgumentNullException("attribute")
			End If
			Dim speculativelyBoundAttribute As BoundAttribute = Me.GetSpeculativelyBoundAttribute(position, attribute, binder)
			boundNodeSummary = If(speculativelyBoundAttribute Is Nothing, New Microsoft.CodeAnalysis.VisualBasic.BoundNodeSummary(), New Microsoft.CodeAnalysis.VisualBasic.BoundNodeSummary(speculativelyBoundAttribute, speculativelyBoundAttribute, Nothing))
			Return boundNodeSummary
		End Function

		Private Function GetSpeculativelyBoundNode(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal expression As ExpressionSyntax, ByVal bindingOption As SpeculativeBindingOption) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			If (bindingOption <> SpeculativeBindingOption.BindAsTypeOrNamespace) Then
				boundNode = Me.Bind(binder, expression, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded)
				boundNode = Me.MakeValueIfPossible(binder, boundNode)
			Else
				boundNode = binder.BindNamespaceOrTypeExpression(DirectCast(expression, TypeSyntax), Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded)
			End If
			Return boundNode
		End Function

		Friend Function GetSpeculativelyBoundNode(ByVal position As Integer, ByVal expression As ExpressionSyntax, ByVal bindingOption As SpeculativeBindingOption, <Out> ByRef binder As Microsoft.CodeAnalysis.VisualBasic.Binder) As BoundNode
			Dim speculativelyBoundNode As BoundNode
			binder = Me.GetSpeculativeBinderForExpression(position, expression, bindingOption)
			If (binder Is Nothing) Then
				speculativelyBoundNode = Nothing
			Else
				speculativelyBoundNode = Me.GetSpeculativelyBoundNode(binder, expression, bindingOption)
			End If
			Return speculativelyBoundNode
		End Function

		Private Function GetSpeculativelyBoundNodeSummary(ByVal position As Integer, ByVal expression As ExpressionSyntax, ByVal bindingOption As SpeculativeBindingOption, <Out> ByRef binder As Microsoft.CodeAnalysis.VisualBasic.Binder) As Microsoft.CodeAnalysis.VisualBasic.BoundNodeSummary
			Dim boundNodeSummary As Microsoft.CodeAnalysis.VisualBasic.BoundNodeSummary
			If (expression Is Nothing) Then
				Throw New ArgumentNullException("expression")
			End If
			Dim standaloneExpression As ExpressionSyntax = SyntaxFactory.GetStandaloneExpression(expression)
			Dim speculativelyBoundNode As BoundNode = Me.GetSpeculativelyBoundNode(position, standaloneExpression, bindingOption, binder)
			boundNodeSummary = If(speculativelyBoundNode Is Nothing, New Microsoft.CodeAnalysis.VisualBasic.BoundNodeSummary(), New Microsoft.CodeAnalysis.VisualBasic.BoundNodeSummary(speculativelyBoundNode, speculativelyBoundNode, Nothing))
			Return boundNodeSummary
		End Function

		Public Function GetSpeculativeMemberGroup(ByVal position As Integer, ByVal expression As ExpressionSyntax) As ImmutableArray(Of ISymbol)
			Dim empty As ImmutableArray(Of ISymbol)
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Nothing
			Dim speculativelyBoundNodeSummary As BoundNodeSummary = Me.GetSpeculativelyBoundNodeSummary(position, expression, SpeculativeBindingOption.BindAsExpression, binder)
			If (speculativelyBoundNodeSummary.LowestBoundNode Is Nothing) Then
				empty = ImmutableArray(Of ISymbol).Empty
			Else
				empty = StaticCast(Of ISymbol).From(Of Symbol)(Me.GetMemberGroupForNode(speculativelyBoundNodeSummary, Nothing))
			End If
			Return empty
		End Function

		Public Function GetSpeculativeSymbolInfo(ByVal position As Integer, ByVal expression As ExpressionSyntax, ByVal bindingOption As SpeculativeBindingOption) As Microsoft.CodeAnalysis.SymbolInfo
			Dim symbolInfo As Microsoft.CodeAnalysis.SymbolInfo
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Nothing
			Dim speculativelyBoundNodeSummary As BoundNodeSummary = Me.GetSpeculativelyBoundNodeSummary(position, expression, bindingOption, binder)
			symbolInfo = If(speculativelyBoundNodeSummary.LowestBoundNode Is Nothing, Microsoft.CodeAnalysis.SymbolInfo.None, Me.GetSymbolInfoForNode(VBSemanticModel.SymbolInfoOptions.DefaultOptions, speculativelyBoundNodeSummary, binder))
			Return symbolInfo
		End Function

		Public Function GetSpeculativeSymbolInfo(ByVal position As Integer, ByVal attribute As AttributeSyntax) As Microsoft.CodeAnalysis.SymbolInfo
			Dim symbolInfo As Microsoft.CodeAnalysis.SymbolInfo
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Nothing
			Dim speculativelyBoundAttributeSummary As BoundNodeSummary = Me.GetSpeculativelyBoundAttributeSummary(position, attribute, binder)
			symbolInfo = If(speculativelyBoundAttributeSummary.LowestBoundNode Is Nothing, Microsoft.CodeAnalysis.SymbolInfo.None, Me.GetSymbolInfoForNode(VBSemanticModel.SymbolInfoOptions.DefaultOptions, speculativelyBoundAttributeSummary, binder))
			Return symbolInfo
		End Function

		Protected NotOverridable Overrides Function GetSpeculativeSymbolInfoCore(ByVal position As Integer, ByVal expression As SyntaxNode, ByVal bindingOption As SpeculativeBindingOption) As SymbolInfo
			If (Not TypeOf expression Is ExpressionSyntax) Then
				Return New SymbolInfo()
			End If
			Return Me.GetSpeculativeSymbolInfo(position, DirectCast(expression, ExpressionSyntax), bindingOption)
		End Function

		Public Function GetSpeculativeTypeInfo(ByVal position As Integer, ByVal expression As ExpressionSyntax, ByVal bindingOption As SpeculativeBindingOption) As Microsoft.CodeAnalysis.TypeInfo
			Return Me.GetSpeculativeTypeInfoWorker(position, expression, bindingOption)
		End Function

		Protected NotOverridable Overrides Function GetSpeculativeTypeInfoCore(ByVal position As Integer, ByVal expression As SyntaxNode, ByVal bindingOption As SpeculativeBindingOption) As Microsoft.CodeAnalysis.TypeInfo
			If (Not TypeOf expression Is ExpressionSyntax) Then
				Return New Microsoft.CodeAnalysis.TypeInfo()
			End If
			Return Me.GetSpeculativeTypeInfo(position, DirectCast(expression, ExpressionSyntax), bindingOption)
		End Function

		Friend Function GetSpeculativeTypeInfoWorker(ByVal position As Integer, ByVal expression As ExpressionSyntax, ByVal bindingOption As SpeculativeBindingOption) As Microsoft.CodeAnalysis.VisualBasic.VisualBasicTypeInfo
			Dim visualBasicTypeInfo As Microsoft.CodeAnalysis.VisualBasic.VisualBasicTypeInfo
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Nothing
			Dim speculativelyBoundNodeSummary As BoundNodeSummary = Me.GetSpeculativelyBoundNodeSummary(position, expression, bindingOption, binder)
			visualBasicTypeInfo = If(speculativelyBoundNodeSummary.LowestBoundNode Is Nothing, Microsoft.CodeAnalysis.VisualBasic.VisualBasicTypeInfo.None, Me.GetTypeInfoForNode(speculativelyBoundNodeSummary))
			Return visualBasicTypeInfo
		End Function

		Public Function GetSymbolInfo(ByVal clauseSyntax As QueryClauseSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SymbolInfo
			Dim none As SymbolInfo
			Me.CheckSyntaxNode(clauseSyntax)
			If (Not Me.CanGetSemanticInfo(clauseSyntax, False)) Then
				none = SymbolInfo.None
			Else
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = clauseSyntax.Kind()
				If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LetClause) Then
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregateClause) Then
						none = SymbolInfo.None
						Return none
					Else
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrderByClause) Then
							GoTo Label1
						End If
						none = Me.GetQueryClauseSymbolInfo(clauseSyntax, cancellationToken)
						Return none
					End If
				End If
			Label1:
				none = SymbolInfo.None
			End If
			Return none
		End Function

		Public Function GetSymbolInfo(ByVal variableSyntax As ExpressionRangeVariableSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SymbolInfo
			Dim none As SymbolInfo
			Me.CheckSyntaxNode(variableSyntax)
			If (Not Me.CanGetSemanticInfo(variableSyntax, False)) Then
				none = SymbolInfo.None
			Else
				none = If(variableSyntax.Parent Is Nothing OrElse variableSyntax.Parent.Kind() <> SyntaxKind.LetClause, SymbolInfo.None, Me.GetLetClauseSymbolInfo(variableSyntax, cancellationToken))
			End If
			Return none
		End Function

		Public Function GetSymbolInfo(ByVal functionSyntax As FunctionAggregationSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SymbolInfo
			Dim none As SymbolInfo
			Me.CheckSyntaxNode(functionSyntax)
			If (Not Me.CanGetSemanticInfo(functionSyntax, False)) Then
				none = SymbolInfo.None
			Else
				If (Not Me.IsInTree(functionSyntax)) Then
					Throw New ArgumentException(VBResources.FunctionSyntaxNotWithinSyntaxTree)
				End If
				none = Me.GetSymbolInfo(DirectCast(functionSyntax, ExpressionSyntax), cancellationToken)
			End If
			Return none
		End Function

		Public Function GetSymbolInfo(ByVal orderingSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderingSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.SymbolInfo
			Dim symbolInfo As Microsoft.CodeAnalysis.SymbolInfo
			Me.CheckSyntaxNode(orderingSyntax)
			symbolInfo = If(Not Me.CanGetSemanticInfo(orderingSyntax, False), Microsoft.CodeAnalysis.SymbolInfo.None, Me.GetOrderingSymbolInfo(orderingSyntax, cancellationToken))
			Return symbolInfo
		End Function

		Public Function GetSymbolInfo(ByVal expression As ExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SymbolInfo
			Dim none As SymbolInfo
			Me.CheckSyntaxNode(expression)
			If (Not Me.CanGetSemanticInfo(expression, True)) Then
				none = SymbolInfo.None
			Else
				none = If(Not SyntaxFacts.IsNamedArgumentName(expression), Me.GetExpressionSymbolInfo(expression, VBSemanticModel.SymbolInfoOptions.DefaultOptions, cancellationToken), Me.GetNamedArgumentSymbolInfo(DirectCast(expression, IdentifierNameSyntax), cancellationToken))
			End If
			Return none
		End Function

		Public Function GetSymbolInfo(ByVal crefReference As CrefReferenceSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SymbolInfo
			Me.CheckSyntaxNode(crefReference)
			Return Me.GetCrefReferenceSymbolInfo(crefReference, VBSemanticModel.SymbolInfoOptions.DefaultOptions, cancellationToken)
		End Function

		Public Function GetSymbolInfo(ByVal attribute As AttributeSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.SymbolInfo
			Dim symbolInfo As Microsoft.CodeAnalysis.SymbolInfo
			Me.CheckSyntaxNode(attribute)
			symbolInfo = If(Not Me.CanGetSemanticInfo(attribute, False), Microsoft.CodeAnalysis.SymbolInfo.None, Me.GetAttributeSymbolInfo(attribute, cancellationToken))
			Return symbolInfo
		End Function

		Protected NotOverridable Overrides Function GetSymbolInfoCore(ByVal node As SyntaxNode, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SymbolInfo
			Return Me.GetSymbolInfoForNode(node, cancellationToken)
		End Function

		Friend Function GetSymbolInfoForNode(ByVal options As VBSemanticModel.SymbolInfoOptions, ByVal boundNodes As BoundNodeSummary, ByVal binderOpt As Microsoft.CodeAnalysis.VisualBasic.Binder) As SymbolInfo
			Dim lookupResultKind As Microsoft.CodeAnalysis.VisualBasic.LookupResultKind = Microsoft.CodeAnalysis.VisualBasic.LookupResultKind.Empty
			Dim symbols As ImmutableArray(Of Symbol) = New ImmutableArray(Of Symbol)()
			Return SymbolInfoFactory.Create(Me.GetSemanticSymbols(boundNodes, binderOpt, options, lookupResultKind, symbols), lookupResultKind)
		End Function

		Private Function GetSymbolInfoForNode(ByVal node As SyntaxNode, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.SymbolInfo
			Dim symbolInfo As Microsoft.CodeAnalysis.SymbolInfo
			If (node Is Nothing) Then
				Throw New ArgumentNullException("node")
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (expressionSyntax Is Nothing) Then
				Dim attributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax)
				If (attributeSyntax Is Nothing) Then
					Dim queryClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax)
					If (queryClauseSyntax Is Nothing) Then
						Dim expressionRangeVariableSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax)
						If (expressionRangeVariableSyntax Is Nothing) Then
							Dim orderingSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderingSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderingSyntax)
							If (orderingSyntax Is Nothing) Then
								Dim functionAggregationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.FunctionAggregationSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.FunctionAggregationSyntax)
								If (functionAggregationSyntax Is Nothing) Then
									Dim crefReferenceSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefReferenceSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefReferenceSyntax)
									symbolInfo = If(crefReferenceSyntax Is Nothing, Microsoft.CodeAnalysis.SymbolInfo.None, Me.GetSymbolInfo(crefReferenceSyntax, cancellationToken))
								Else
									symbolInfo = Me.GetSymbolInfo(functionAggregationSyntax, cancellationToken)
								End If
							Else
								symbolInfo = Me.GetSymbolInfo(orderingSyntax, cancellationToken)
							End If
						Else
							symbolInfo = Me.GetSymbolInfo(expressionRangeVariableSyntax, cancellationToken)
						End If
					Else
						symbolInfo = Me.GetSymbolInfo(queryClauseSyntax, cancellationToken)
					End If
				Else
					symbolInfo = Me.GetSymbolInfo(attributeSyntax, cancellationToken)
				End If
			Else
				symbolInfo = Me.GetSymbolInfo(expressionSyntax, cancellationToken)
			End If
			Return symbolInfo
		End Function

		Friend Function GetSymbolInfoForSymbol(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal options As VBSemanticModel.SymbolInfoOptions) As SymbolInfo
			Dim resultKind As LookupResultKind
			Dim originalDefinition As Microsoft.CodeAnalysis.VisualBasic.Symbols.ErrorTypeSymbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = TryCast(symbol.UnwrapAlias(), Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).GetInstance()
			If (typeSymbol IsNot Nothing) Then
				originalDefinition = TryCast(typeSymbol.OriginalDefinition, Microsoft.CodeAnalysis.VisualBasic.Symbols.ErrorTypeSymbol)
			Else
				originalDefinition = Nothing
			End If
			Dim errorTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ErrorTypeSymbol = originalDefinition
			If (errorTypeSymbol IsNot Nothing) Then
				resultKind = errorTypeSymbol.ResultKind
				If (resultKind <> LookupResultKind.Empty) Then
					instance.AddRange(errorTypeSymbol.CandidateSymbols)
				End If
			ElseIf (symbol.Kind <> SymbolKind.[Namespace] OrElse CInt(DirectCast(symbol, NamespaceSymbol).NamespaceKind) <> 0) Then
				instance.Add(symbol)
				resultKind = LookupResultKind.Good
			Else
				instance.AddRange(Of NamespaceSymbol)(DirectCast(symbol, NamespaceSymbol).ConstituentNamespaces)
				resultKind = LookupResultKind.Ambiguous
			End If
			Dim symbols As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = Me.RemoveErrorTypesAndDuplicates(instance, options)
			instance.Free()
			Return SymbolInfoFactory.Create(symbols, resultKind)
		End Function

		Protected Overrides Function GetTopmostNodeForDiagnosticAnalysis(ByVal symbol As ISymbol, ByVal declaringSyntax As SyntaxNode) As SyntaxNode
			Dim parent As SyntaxNode
			Select Case symbol.Kind
				Case SymbolKind.[Event]
					If (Not TypeOf declaringSyntax Is EventStatementSyntax OrElse declaringSyntax.Parent Is Nothing OrElse Not TypeOf declaringSyntax.Parent Is EventBlockSyntax) Then
						GoTo Label0
					End If
					parent = declaringSyntax.Parent
					Exit Select
				Case SymbolKind.Field
					Dim fieldDeclarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax = declaringSyntax.FirstAncestorOrSelf(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax)(Nothing, True)
					If (fieldDeclarationSyntax Is Nothing) Then
						GoTo Label0
					End If
					parent = fieldDeclarationSyntax
					Exit Select
				Case SymbolKind.Label
				Case SymbolKind.Local
				Case SymbolKind.NetModule
				Case SymbolKind.Parameter
				Case SymbolKind.PointerType
				Label0:
					parent = declaringSyntax
					Exit Select
				Case SymbolKind.Method
					If (Not TypeOf declaringSyntax Is MethodBaseSyntax OrElse declaringSyntax.Parent Is Nothing OrElse Not TypeOf declaringSyntax.Parent Is MethodBlockBaseSyntax) Then
						GoTo Label0
					End If
					parent = declaringSyntax.Parent
					Exit Select
				Case SymbolKind.NamedType
					If (Not TypeOf declaringSyntax Is TypeStatementSyntax OrElse declaringSyntax.Parent Is Nothing OrElse Not TypeOf declaringSyntax.Parent Is TypeBlockSyntax) Then
						GoTo Label0
					End If
					parent = declaringSyntax.Parent
					Exit Select
				Case SymbolKind.[Namespace]
					If (Not TypeOf declaringSyntax Is NamespaceStatementSyntax OrElse declaringSyntax.Parent Is Nothing OrElse Not TypeOf declaringSyntax.Parent Is NamespaceBlockSyntax) Then
						GoTo Label0
					End If
					parent = declaringSyntax.Parent
					Exit Select
				Case SymbolKind.[Property]
					If (Not TypeOf declaringSyntax Is PropertyStatementSyntax OrElse declaringSyntax.Parent Is Nothing OrElse Not TypeOf declaringSyntax.Parent Is PropertyBlockSyntax) Then
						GoTo Label0
					End If
					parent = declaringSyntax.Parent
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return parent
		End Function

		Public Function GetTypeInfo(ByVal expression As ExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.TypeInfo
			Return Me.GetTypeInfoWorker(expression, cancellationToken)
		End Function

		Public Function GetTypeInfo(ByVal attribute As AttributeSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.TypeInfo
			Return Me.GetTypeInfoWorker(attribute, cancellationToken)
		End Function

		Protected NotOverridable Overrides Function GetTypeInfoCore(ByVal node As SyntaxNode, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.TypeInfo
			Return Me.GetTypeInfoForNode(node, cancellationToken)
		End Function

		Friend Function GetTypeInfoForNode(ByVal boundNodes As BoundNodeSummary) As VisualBasicTypeInfo
			Dim semanticType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
			Dim conversion As Microsoft.CodeAnalysis.VisualBasic.Conversion = New Microsoft.CodeAnalysis.VisualBasic.Conversion()
			semanticType = Me.GetSemanticType(boundNodes, typeSymbol, conversion)
			Return New VisualBasicTypeInfo(semanticType, typeSymbol, conversion)
		End Function

		Private Function GetTypeInfoForNode(ByVal node As SyntaxNode, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As VisualBasicTypeInfo
			Dim typeInfoWorker As VisualBasicTypeInfo
			If (node Is Nothing) Then
				Throw New ArgumentNullException("node")
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (expressionSyntax Is Nothing) Then
				Dim attributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax)
				typeInfoWorker = If(attributeSyntax Is Nothing, VisualBasicTypeInfo.None, Me.GetTypeInfoWorker(attributeSyntax, cancellationToken))
			Else
				typeInfoWorker = Me.GetTypeInfoWorker(expressionSyntax, cancellationToken)
			End If
			Return typeInfoWorker
		End Function

		Friend Function GetTypeInfoForSymbol(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As VisualBasicTypeInfo
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = TryCast(symbol.UnwrapAlias(), Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)
			Return New VisualBasicTypeInfo(typeSymbol, typeSymbol, New Conversion(Microsoft.CodeAnalysis.VisualBasic.Conversions.Identity))
		End Function

		Friend Function GetTypeInfoWorker(ByVal expression As ExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As VisualBasicTypeInfo
			Dim none As VisualBasicTypeInfo
			Me.CheckSyntaxNode(expression)
			If (Not Me.CanGetSemanticInfo(expression, False)) Then
				none = VisualBasicTypeInfo.None
			Else
				none = If(Not SyntaxFacts.IsNamedArgumentName(expression), Me.GetExpressionTypeInfo(expression, cancellationToken), VisualBasicTypeInfo.None)
			End If
			Return none
		End Function

		Private Function GetTypeInfoWorker(ByVal attribute As AttributeSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.VisualBasic.VisualBasicTypeInfo
			Dim visualBasicTypeInfo As Microsoft.CodeAnalysis.VisualBasic.VisualBasicTypeInfo
			Me.CheckSyntaxNode(attribute)
			visualBasicTypeInfo = If(Not Me.CanGetSemanticInfo(attribute, False), Microsoft.CodeAnalysis.VisualBasic.VisualBasicTypeInfo.None, Me.GetAttributeTypeInfo(attribute, cancellationToken))
			Return visualBasicTypeInfo
		End Function

		Public Shadows Function IsAccessible(ByVal position As Integer, ByVal symbol As ISymbol) As Boolean
			Dim flag As Boolean
			Me.CheckPosition(position)
			If (symbol Is Nothing) Then
				Throw New ArgumentNullException("symbol")
			End If
			Dim symbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbol = symbol.EnsureVbSymbolOrNothing(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)("symbol")
			Dim enclosingBinder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me.GetEnclosingBinder(position)
			If (enclosingBinder Is Nothing) Then
				flag = False
			Else
				Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
				Dim basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved = New Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved()
				flag = enclosingBinder.IsAccessible(symbol1, discarded, Nothing, basesBeingResolved)
			End If
			Return flag
		End Function

		Protected NotOverridable Overrides Function IsAccessibleCore(ByVal position As Integer, ByVal symbol As ISymbol) As Boolean
			Return Me.IsAccessible(position, symbol.EnsureVbSymbolOrNothing(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)("symbol"))
		End Function

		Protected NotOverridable Overrides Function IsEventUsableAsFieldCore(ByVal position As Integer, ByVal symbol As IEventSymbol) As Boolean
			Return False
		End Function

		Friend Shared Function IsInCrefOrNameAttributeInterior(ByVal node As VisualBasicSyntaxNode) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = node.Kind()
			If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlString) Then
				Select Case syntaxKind
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PredefinedType
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierName
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GenericName
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QualifiedName
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GlobalName
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CrefReference
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CrefOperatorReference
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QualifiedCrefOperatorReference
						Exit Select
					Case Else
						flag = False
						Return flag
				End Select
			End If
			Dim parent As VisualBasicSyntaxNode = node.Parent
			Dim flag1 As Boolean = False
			While parent IsNot Nothing
				Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = parent.Kind()
				If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlAttribute) Then
					flag1 = True
					parent = parent.Parent
				ElseIf (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DocumentationCommentTrivia) Then
					If (CUShort(syntaxKind1) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlCrefAttribute) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
						parent = parent.Parent
					Else
						flag = True
						Return flag
					End If
				ElseIf (Not flag1) Then
					parent = parent.Parent
				Else
					flag = True
					Return flag
				End If
			End While
			flag = False
			Return flag
		End Function

		Friend Function IsInTree(ByVal node As SyntaxNode) As Boolean
			Return VBSemanticModel.IsUnderNode(node, Me.Root)
		End Function

		Private Shared Function IsUnderNode(ByVal node As SyntaxNode, ByVal root As SyntaxNode) As Boolean
			Dim flag As Boolean
			While True
				If (node Is Nothing) Then
					flag = False
					Exit While
				ElseIf (node = root) Then
					flag = True
					Exit While
				ElseIf (Not node.IsStructuredTrivia) Then
					node = node.Parent
				Else
					node = DirectCast(node, StructuredTriviaSyntax).ParentTrivia.Token.Parent
				End If
			End While
			Return flag
		End Function

		Public Shadows Function LookupBaseMembers(ByVal position As Integer, Optional ByVal name As String = Nothing) As ImmutableArray(Of ISymbol)
			Return StaticCast(Of ISymbol).From(Of Symbol)(Me.LookupSymbolsInternal(position, Nothing, name, LookupOptions.[Default], True))
		End Function

		Protected NotOverridable Overrides Function LookupBaseMembersCore(ByVal position As Integer, ByVal name As String) As ImmutableArray(Of ISymbol)
			Return Me.LookupBaseMembers(position, name)
		End Function

		Private Sub LookupInstanceConstructors(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal container As NamespaceOrTypeSymbol, ByVal options As LookupOptions, ByVal results As ArrayBuilder(Of Symbol))
			' 
			' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel::LookupInstanceConstructors(Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol,Microsoft.CodeAnalysis.VisualBasic.LookupOptions,Microsoft.CodeAnalysis.PooledObjects.ArrayBuilder`1<Microsoft.CodeAnalysis.VisualBasic.Symbol>)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Void LookupInstanceConstructors(Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol,Microsoft.CodeAnalysis.VisualBasic.LookupOptions,Microsoft.CodeAnalysis.PooledObjects.ArrayBuilder<Microsoft.CodeAnalysis.VisualBasic.Symbol>)
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

		End Sub

		Public Shadows Function LookupLabels(ByVal position As Integer, Optional ByVal name As String = Nothing) As ImmutableArray(Of ISymbol)
			Return StaticCast(Of ISymbol).From(Of Symbol)(Me.LookupSymbolsInternal(position, Nothing, name, LookupOptions.LabelsOnly, False))
		End Function

		Protected NotOverridable Overrides Function LookupLabelsCore(ByVal position As Integer, ByVal name As String) As ImmutableArray(Of ISymbol)
			Return Me.LookupLabels(position, name)
		End Function

		Public Shadows Function LookupNamespacesAndTypes(ByVal position As Integer, Optional ByVal container As INamespaceOrTypeSymbol = Nothing, Optional ByVal name As String = Nothing) As ImmutableArray(Of ISymbol)
			Return StaticCast(Of ISymbol).From(Of Symbol)(Me.LookupSymbolsInternal(position, VBSemanticModel.ToLanguageSpecific(container), name, LookupOptions.NamespacesOrTypesOnly, False))
		End Function

		Protected NotOverridable Overrides Function LookupNamespacesAndTypesCore(ByVal position As Integer, ByVal container As INamespaceOrTypeSymbol, ByVal name As String) As ImmutableArray(Of ISymbol)
			Return Me.LookupNamespacesAndTypes(position, VBSemanticModel.ToLanguageSpecific(container), name)
		End Function

		Public Shadows Function LookupStaticMembers(ByVal position As Integer, Optional ByVal container As INamespaceOrTypeSymbol = Nothing, Optional ByVal name As String = Nothing) As ImmutableArray(Of ISymbol)
			Return StaticCast(Of ISymbol).From(Of Symbol)(Me.LookupSymbolsInternal(position, VBSemanticModel.ToLanguageSpecific(container), name, LookupOptions.MustNotBeInstance Or LookupOptions.IgnoreExtensionMethods, False))
		End Function

		Protected NotOverridable Overrides Function LookupStaticMembersCore(ByVal position As Integer, ByVal container As INamespaceOrTypeSymbol, ByVal name As String) As ImmutableArray(Of ISymbol)
			Return Me.LookupStaticMembers(position, VBSemanticModel.ToLanguageSpecific(container), name)
		End Function

		Public Shadows Function LookupSymbols(ByVal position As Integer, Optional ByVal container As INamespaceOrTypeSymbol = Nothing, Optional ByVal name As String = Nothing, Optional ByVal includeReducedExtensionMethods As Boolean = False) As ImmutableArray(Of ISymbol)
			Dim lookupOption As LookupOptions = If(includeReducedExtensionMethods, LookupOptions.[Default], LookupOptions.IgnoreExtensionMethods)
			Return StaticCast(Of ISymbol).From(Of Symbol)(Me.LookupSymbolsInternal(position, VBSemanticModel.ToLanguageSpecific(container), name, lookupOption, False))
		End Function

		Private Sub LookupSymbols(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal container As NamespaceOrTypeSymbol, ByVal name As String, ByVal arities As AbstractLookupSymbolsInfo(Of Symbol).IArityEnumerable, ByVal options As LookupOptions, ByVal results As ArrayBuilder(Of Symbol))
			Dim enumerator As AbstractLookupSymbolsInfo(Of Symbol).ArityEnumerator = New AbstractLookupSymbolsInfo(Of Symbol).ArityEnumerator()
			Dim instance As PooledHashSet(Of Symbol) = PooledHashSet(Of Symbol).GetInstance()
			Dim symbols As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance(arities.Count)
			Try
				enumerator = arities.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Integer = enumerator.Current
					Me.LookupSymbols(binder, container, name, current, options, symbols)
					instance.UnionWith(symbols)
					symbols.Clear()
				End While
			Finally
				DirectCast(enumerator, IDisposable).Dispose()
			End Try
			symbols.Free()
			results.AddRange(instance)
			instance.Free()
		End Sub

		Private Sub LookupSymbols(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal container As NamespaceOrTypeSymbol, ByVal name As String, ByVal arity As Integer, ByVal options As LookupOptions, ByVal results As ArrayBuilder(Of Symbol))
			Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol)
			If (EmbeddedOperators.CompareString(name, ".ctor", False) = 0) Then
				Me.LookupInstanceConstructors(binder, container, options, results)
				Return
			End If
			Dim instance As LookupResult = LookupResult.GetInstance()
			Dim num As Integer = arity
			options = options Or LookupOptions.EagerlyLookupExtensionMethods
			If (options.IsAttributeTypeLookup()) Then
				discarded = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
				binder.LookupAttributeType(instance, container, name, options, discarded)
			ElseIf (container IsNot Nothing) Then
				discarded = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
				binder.LookupMember(instance, container, name, num, options, discarded)
			Else
				discarded = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
				binder.Lookup(instance, name, num, options, discarded)
			End If
			If (instance.IsGoodOrAmbiguous) Then
				If (instance.HasDiagnostic) Then
					Dim pooledHashSet As PooledHashSet(Of Symbol) = PooledHashSet(Of Symbol).GetInstance()
					Dim symbols As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance()
					Me.AddSymbolsFromDiagnosticInfo(symbols, instance.Diagnostic)
					pooledHashSet.UnionWith(symbols)
					pooledHashSet.UnionWith(instance.Symbols)
					symbols.Free()
					results.AddRange(pooledHashSet)
					pooledHashSet.Free()
				ElseIf (Not instance.HasSingleSymbol OrElse instance.SingleSymbol.Kind <> SymbolKind.[Namespace] OrElse CInt(DirectCast(instance.SingleSymbol, NamespaceSymbol).NamespaceKind) <> 0) Then
					results.AddRange(instance.Symbols)
				Else
					results.AddRange(Of NamespaceSymbol)(DirectCast(instance.SingleSymbol, NamespaceSymbol).ConstituentNamespaces)
				End If
			End If
			instance.Free()
		End Sub

		Protected NotOverridable Overrides Function LookupSymbolsCore(ByVal position As Integer, ByVal container As INamespaceOrTypeSymbol, ByVal name As String, ByVal includeReducedExtensionMethods As Boolean) As ImmutableArray(Of ISymbol)
			Return Me.LookupSymbols(position, VBSemanticModel.ToLanguageSpecific(container), name, includeReducedExtensionMethods)
		End Function

		Private Function LookupSymbolsInternal(ByVal position As Integer, ByVal container As NamespaceOrTypeSymbol, ByVal name As String, ByVal options As LookupOptions, ByVal useBaseReferenceAccessibility As Boolean) As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)
			Dim immutableAndFree As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)
			Dim enumerator As IEnumerator(Of String) = Nothing
			Dim baseTypeNoUseSiteDiagnostics As Object
			If (useBaseReferenceAccessibility) Then
				options = options Or LookupOptions.UseBaseReferenceAccessibility
			End If
			Me.CheckPosition(position)
			Dim enclosingBinder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me.GetEnclosingBinder(position)
			If (enclosingBinder IsNot Nothing) Then
				If (useBaseReferenceAccessibility) Then
					Dim containingType As NamedTypeSymbol = enclosingBinder.ContainingType
					If (containingType Is Nothing) Then
						baseTypeNoUseSiteDiagnostics = Nothing
					Else
						baseTypeNoUseSiteDiagnostics = containingType.BaseTypeNoUseSiteDiagnostics
					End If
					If (baseTypeNoUseSiteDiagnostics Is Nothing) Then
						Throw New ArgumentException("position", "Not a valid position for a call to LookupBaseMembers (must be in a type with a base type)")
					End If
					container = baseTypeNoUseSiteDiagnostics
				End If
				If (name IsNot Nothing) Then
					Dim instance As Microsoft.CodeAnalysis.VisualBasic.LookupSymbolsInfo = Microsoft.CodeAnalysis.VisualBasic.LookupSymbolsInfo.GetInstance()
					instance.FilterName = name
					Me.AddLookupSymbolsInfo(position, instance, container, options)
					Dim symbols As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).GetInstance(instance.Count)
					Me.AppendSymbolsWithName(symbols, name, enclosingBinder, container, options, instance)
					instance.Free()
					immutableAndFree = symbols.ToImmutableAndFree()
				Else
					Dim lookupSymbolsInfo As Microsoft.CodeAnalysis.VisualBasic.LookupSymbolsInfo = Microsoft.CodeAnalysis.VisualBasic.LookupSymbolsInfo.GetInstance()
					Me.AddLookupSymbolsInfo(position, lookupSymbolsInfo, container, options)
					Using instance1 As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).GetInstance(lookupSymbolsInfo.Count)
						enumerator = lookupSymbolsInfo.Names.GetEnumerator()
						While enumerator.MoveNext()
							Dim current As String = enumerator.Current
							Me.AppendSymbolsWithName(instance1, current, enclosingBinder, container, options, lookupSymbolsInfo)
						End While
					End Using
					lookupSymbolsInfo.Free()
					Dim immutableAndFree1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = instance1.ToImmutableAndFree()
					Dim symbols1 As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = Nothing
					Dim num As Integer = 0
					Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = immutableAndFree1.GetEnumerator()
					While enumerator1.MoveNext()
						Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator1.Current
						If (Not symbol.CanBeReferencedByName AndAlso (symbol.Kind <> SymbolKind.Method OrElse DirectCast(symbol, MethodSymbol).MethodKind <> MethodKind.Constructor)) Then
							If (symbols1 Is Nothing) Then
								symbols1 = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).GetInstance()
								symbols1.AddRange(immutableAndFree1, num)
							End If
						ElseIf (symbols1 IsNot Nothing) Then
							symbols1.Add(symbol)
						End If
						num = num + 1
					End While
					immutableAndFree = If(symbols1 Is Nothing, immutableAndFree1, symbols1.ToImmutableAndFree())
				End If
			Else
				immutableAndFree = ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Empty
			End If
			Return immutableAndFree
		End Function

		Private Function MakeValueIfPossible(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundNode) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			If (boundExpression IsNot Nothing) Then
				Dim bindingDiagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = New Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag(DiagnosticBag.GetInstance())
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = binder.ReclassifyAsValue(boundExpression, bindingDiagnosticBag)
				If (Not boundExpression1.HasErrors AndAlso boundExpression1.Type Is Nothing) Then
					boundExpression1 = binder.ReclassifyExpression(boundExpression1, bindingDiagnosticBag)
				End If
				bindingDiagnosticBag.Free()
				If (bindingDiagnosticBag.HasAnyErrors()) Then
					boundNode = node
					Return boundNode
				End If
				boundNode = boundExpression1
				Return boundNode
			End If
			boundNode = node
			Return boundNode
		End Function

		Friend Function RemoveErrorTypesAndDuplicates(ByVal symbolsBuilder As ArrayBuilder(Of Symbol), ByVal options As VBSemanticModel.SymbolInfoOptions) As ImmutableArray(Of Symbol)
			' 
			' Current member / type: System.Collections.Immutable.ImmutableArray`1<Microsoft.CodeAnalysis.VisualBasic.Symbol> Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel::RemoveErrorTypesAndDuplicates(Microsoft.CodeAnalysis.PooledObjects.ArrayBuilder`1<Microsoft.CodeAnalysis.VisualBasic.Symbol>,Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel/SymbolInfoOptions)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.VisualBasic.Symbol> RemoveErrorTypesAndDuplicates(Microsoft.CodeAnalysis.PooledObjects.ArrayBuilder<Microsoft.CodeAnalysis.VisualBasic.Symbol>,Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel/SymbolInfoOptions)
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

		Private Shared Function SafeCastArgument(Of T As Class)(ByVal node As SyntaxNode, ByVal argName As String) As T
			If (node Is Nothing) Then
				Throw New ArgumentNullException(argName)
			End If
			Dim t1 As T = DirectCast(TryCast(node, T), T)
			If (t1 Is Nothing) Then
				Throw New ArgumentException([String].Concat(argName, " is not an ", GetType(T).Name))
			End If
			Return t1
		End Function

		Friend Shared Function StripSemanticModelBinder(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder) As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim binder1 As Microsoft.CodeAnalysis.VisualBasic.Binder
			If (binder Is Nothing OrElse Not binder.IsSemanticModelBinder) Then
				binder1 = binder
			Else
				binder1 = If(TypeOf binder Is SemanticModelBinder, binder.ContainingBinder, binder)
			End If
			Return binder1
		End Function

		Private Shared Function ToLanguageSpecific(ByVal container As INamespaceOrTypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol
			Dim namespaceOrTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol
			If (container IsNot Nothing) Then
				Dim namespaceOrTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol = TryCast(container, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol)
				If (namespaceOrTypeSymbol1 Is Nothing) Then
					Throw New ArgumentException(VBResources.NotAVbSymbol, "container")
				End If
				namespaceOrTypeSymbol = namespaceOrTypeSymbol1
			Else
				namespaceOrTypeSymbol = Nothing
			End If
			Return namespaceOrTypeSymbol
		End Function

		Public Function TryGetSpeculativeSemanticModel(ByVal position As Integer, ByVal rangeArgument As RangeArgumentSyntax, <Out> ByRef speculativeModel As SemanticModel) As Boolean
			Me.CheckPosition(position)
			Me.CheckModelAndSyntaxNodeToSpeculate(rangeArgument)
			Return Me.TryGetSpeculativeSemanticModelCore(DirectCast(Me, SyntaxTreeSemanticModel), position, rangeArgument, speculativeModel)
		End Function

		Public Function TryGetSpeculativeSemanticModel(ByVal position As Integer, ByVal statement As ExecutableStatementSyntax, <Out> ByRef speculativeModel As SemanticModel) As Boolean
			Me.CheckPosition(position)
			Me.CheckModelAndSyntaxNodeToSpeculate(statement)
			Return Me.TryGetSpeculativeSemanticModelCore(DirectCast(Me, SyntaxTreeSemanticModel), position, statement, speculativeModel)
		End Function

		Public Function TryGetSpeculativeSemanticModel(ByVal position As Integer, ByVal initializer As EqualsValueSyntax, <Out> ByRef speculativeModel As SemanticModel) As Boolean
			Me.CheckPosition(position)
			Me.CheckModelAndSyntaxNodeToSpeculate(initializer)
			Return Me.TryGetSpeculativeSemanticModelCore(DirectCast(Me, SyntaxTreeSemanticModel), position, initializer, speculativeModel)
		End Function

		Public Function TryGetSpeculativeSemanticModel(ByVal position As Integer, ByVal attribute As AttributeSyntax, <Out> ByRef speculativeModel As SemanticModel) As Boolean
			Dim flag As Boolean
			Me.CheckPosition(position)
			Me.CheckModelAndSyntaxNodeToSpeculate(attribute)
			Dim speculativeAttributeBinder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me.GetSpeculativeAttributeBinder(position, attribute)
			If (speculativeAttributeBinder IsNot Nothing) Then
				speculativeModel = AttributeSemanticModel.CreateSpeculative(DirectCast(Me, SyntaxTreeSemanticModel), attribute, speculativeAttributeBinder, position)
				flag = True
			Else
				speculativeModel = Nothing
				flag = False
			End If
			Return flag
		End Function

		Public Function TryGetSpeculativeSemanticModel(ByVal position As Integer, ByVal type As TypeSyntax, <Out> ByRef speculativeModel As SemanticModel, Optional ByVal bindingOption As SpeculativeBindingOption = 0) As Boolean
			Me.CheckPosition(position)
			Me.CheckModelAndSyntaxNodeToSpeculate(type)
			Return Me.TryGetSpeculativeSemanticModelCore(DirectCast(Me, SyntaxTreeSemanticModel), position, type, bindingOption, speculativeModel)
		End Function

		Friend MustOverride Function TryGetSpeculativeSemanticModelCore(ByVal parentModel As SyntaxTreeSemanticModel, ByVal position As Integer, ByVal rangeArgument As RangeArgumentSyntax, <Out> ByRef speculativeModel As SemanticModel) As Boolean

		Friend MustOverride Function TryGetSpeculativeSemanticModelCore(ByVal parentModel As SyntaxTreeSemanticModel, ByVal position As Integer, ByVal statement As ExecutableStatementSyntax, <Out> ByRef speculativeModel As SemanticModel) As Boolean

		Friend MustOverride Function TryGetSpeculativeSemanticModelCore(ByVal parentModel As SyntaxTreeSemanticModel, ByVal position As Integer, ByVal initializer As EqualsValueSyntax, <Out> ByRef speculativeModel As SemanticModel) As Boolean

		Friend MustOverride Function TryGetSpeculativeSemanticModelCore(ByVal parentModel As SyntaxTreeSemanticModel, ByVal position As Integer, ByVal type As TypeSyntax, ByVal bindingOption As SpeculativeBindingOption, <Out> ByRef speculativeModel As SemanticModel) As Boolean

		Public Function TryGetSpeculativeSemanticModelForMethodBody(ByVal position As Integer, ByVal method As MethodBlockBaseSyntax, <Out> ByRef speculativeModel As SemanticModel) As Boolean
			Me.CheckPosition(position)
			Me.CheckModelAndSyntaxNodeToSpeculate(method)
			Return Me.TryGetSpeculativeSemanticModelForMethodBodyCore(DirectCast(Me, SyntaxTreeSemanticModel), position, method, speculativeModel)
		End Function

		Friend MustOverride Function TryGetSpeculativeSemanticModelForMethodBodyCore(ByVal parentModel As SyntaxTreeSemanticModel, ByVal position As Integer, ByVal method As MethodBlockBaseSyntax, <Out> ByRef speculativeModel As SemanticModel) As Boolean

		Private Shared Function UnwrapAliases(ByVal symbols As ImmutableArray(Of Symbol)) As ImmutableArray(Of Symbol)
			Dim immutableAndFree As ImmutableArray(Of Symbol)
			Dim kind As Func(Of Symbol, Boolean)
			Dim symbols1 As ImmutableArray(Of Symbol) = symbols
			If (VBSemanticModel._Closure$__.$I78-0 Is Nothing) Then
				kind = Function(sym As Symbol) sym.Kind = SymbolKind.[Alias]
				VBSemanticModel._Closure$__.$I78-0 = kind
			Else
				kind = VBSemanticModel._Closure$__.$I78-0
			End If
			If (symbols1.Any(kind)) Then
				Dim instance As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance()
				Dim enumerator As ImmutableArray(Of Symbol).Enumerator = symbols.GetEnumerator()
				While enumerator.MoveNext()
					instance.Add(enumerator.Current.UnwrapAlias())
				End While
				immutableAndFree = instance.ToImmutableAndFree()
			Else
				immutableAndFree = symbols
			End If
			Return immutableAndFree
		End Function

		Friend Sub ValidateSymbolInfoOptions(ByVal options As VBSemanticModel.SymbolInfoOptions)
		End Sub

		Friend Enum SymbolInfoOptions
			PreferTypeToConstructors = 1
			PreferConstructorsToType = 2
			ResolveAliases = 4
			DefaultOptions = 6
			PreserveAliases = 8
		End Enum
	End Class
End Namespace