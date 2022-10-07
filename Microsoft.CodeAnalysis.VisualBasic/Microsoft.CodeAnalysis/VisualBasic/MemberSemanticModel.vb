Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Collections
Imports Microsoft.CodeAnalysis.Operations
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class MemberSemanticModel
		Inherits VBSemanticModel
		Private ReadOnly _root As SyntaxNode

		Private ReadOnly _rootBinder As Binder

		Private ReadOnly _containingSemanticModelOpt As SyntaxTreeSemanticModel

		Private ReadOnly _parentSemanticModelOpt As SyntaxTreeSemanticModel

		Private ReadOnly _speculatedPosition As Integer

		Private ReadOnly _ignoresAccessibility As Boolean

		Private ReadOnly _operationFactory As Lazy(Of VisualBasicOperationFactory)

		Private ReadOnly _rwLock As ReaderWriterLockSlim

		Private ReadOnly _guardedBoundNodeMap As SmallDictionary(Of SyntaxNode, ImmutableArray(Of BoundNode))

		Private ReadOnly _guardedIOperationNodeMap As Dictionary(Of SyntaxNode, IOperation)

		Private ReadOnly _guardedQueryBindersMap As Dictionary(Of SyntaxNode, ImmutableArray(Of Binder))

		Private ReadOnly _guardedAnonymousTypeBinderMap As Dictionary(Of FieldInitializerSyntax, Binder.AnonymousTypeFieldInitializerBinder)

		Public Overrides ReadOnly Property Compilation As VisualBasicCompilation
			Get
				Return Me.RootBinder.Compilation
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ContainingModelOrSelf As SemanticModel
			Get
				Dim vBSemanticModel As Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel = Me._containingSemanticModelOpt
				If (vBSemanticModel Is Nothing) Then
					vBSemanticModel = Me
				End If
				Return vBSemanticModel
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IgnoresAccessibility As Boolean
			Get
				Return Me._ignoresAccessibility
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsSpeculativeSemanticModel As Boolean
			Get
				Return Me._parentSemanticModelOpt IsNot Nothing
			End Get
		End Property

		Friend ReadOnly Property MemberSymbol As Symbol
			Get
				Return Me.RootBinder.ContainingMember
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property OriginalPositionForSpeculation As Integer
			Get
				Return Me._speculatedPosition
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property ParentModel As SemanticModel
			Get
				Return Me._parentSemanticModelOpt
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property Root As SyntaxNode
			Get
				Return Me._root
			End Get
		End Property

		Friend ReadOnly Property RootBinder As Binder
			Get
				Return Me._rootBinder
			End Get
		End Property

		Public Overrides ReadOnly Property SyntaxTree As Microsoft.CodeAnalysis.SyntaxTree
			Get
				Return Me.Root.SyntaxTree
			End Get
		End Property

		Friend Sub New(ByVal root As SyntaxNode, ByVal rootBinder As Binder, ByVal containingSemanticModelOpt As SyntaxTreeSemanticModel, ByVal parentSemanticModelOpt As SyntaxTreeSemanticModel, ByVal speculatedPosition As Integer, Optional ByVal ignoreAccessibility As Boolean = False)
			MyBase.New()
			Me._rwLock = New ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion)
			Me._guardedBoundNodeMap = New SmallDictionary(Of SyntaxNode, ImmutableArray(Of BoundNode))(ReferenceEqualityComparer.Instance)
			Me._guardedIOperationNodeMap = New Dictionary(Of SyntaxNode, IOperation)()
			Me._guardedQueryBindersMap = New Dictionary(Of SyntaxNode, ImmutableArray(Of Binder))()
			Me._guardedAnonymousTypeBinderMap = New Dictionary(Of FieldInitializerSyntax, Binder.AnonymousTypeFieldInitializerBinder)()
			Me._root = root
			Me._ignoresAccessibility = ignoreAccessibility
			Me._rootBinder = SemanticModelBinder.Mark(rootBinder, ignoreAccessibility)
			Me._containingSemanticModelOpt = containingSemanticModelOpt
			Me._parentSemanticModelOpt = parentSemanticModelOpt
			Me._speculatedPosition = speculatedPosition
			Me._operationFactory = New Lazy(Of VisualBasicOperationFactory)(Function() New VisualBasicOperationFactory(Me))
		End Sub

		<Conditional("DEBUG")>
		Private Sub AssertIfShouldHaveFound(ByVal node As VisualBasicSyntaxNode)
		End Sub

		Friend Sub CacheBoundNodes(ByVal boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode, Optional ByVal thisSyntaxNodeOnly As SyntaxNode = Nothing)
			Me._rwLock.EnterWriteLock()
			Try
				MemberSemanticModel.SemanticModelMapsBuilder.GuardedCacheBoundNodes(boundNode, Me, Me._guardedBoundNodeMap, thisSyntaxNodeOnly)
			Finally
				Me._rwLock.ExitWriteLock()
			End Try
		End Sub

		Public Overrides Function ClassifyConversion(ByVal expression As ExpressionSyntax, ByVal destination As ITypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Conversion
			Dim conversion As Microsoft.CodeAnalysis.VisualBasic.Conversion
			MyBase.CheckSyntaxNode(expression)
			expression = SyntaxFactory.GetStandaloneExpression(expression)
			If (destination Is Nothing) Then
				Throw New ArgumentNullException("destination")
			End If
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = destination.EnsureVbSymbolOrNothing(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)("destination")
			Dim lowerBoundNode As BoundExpression = TryCast(Me.GetLowerBoundNode(expression), BoundExpression)
			If (lowerBoundNode Is Nothing OrElse typeSymbol.IsErrorType()) Then
				conversion = New Microsoft.CodeAnalysis.VisualBasic.Conversion(New KeyValuePair(Of ConversionKind, MethodSymbol)())
			Else
				Dim kind As BoundKind = lowerBoundNode.Kind
				If (kind = BoundKind.ArrayCreation) Then
					Dim arrayLiteralOpt As BoundArrayLiteral = DirectCast(lowerBoundNode, BoundArrayCreation).ArrayLiteralOpt
					If (arrayLiteralOpt IsNot Nothing) Then
						lowerBoundNode = arrayLiteralOpt
					End If
				ElseIf (kind = BoundKind.Lambda) Then
					Dim lambdaSymbol As SourceLambdaSymbol = TryCast(DirectCast(lowerBoundNode, BoundLambda).LambdaSymbol, SourceLambdaSymbol)
					If (lambdaSymbol IsNot Nothing) Then
						lowerBoundNode = lambdaSymbol.UnboundLambda
					End If
				End If
				Dim enclosingBinder As Binder = Me.GetEnclosingBinder(lowerBoundNode.Syntax)
				Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
				conversion = New Microsoft.CodeAnalysis.VisualBasic.Conversion(Conversions.ClassifyConversion(lowerBoundNode, typeSymbol, enclosingBinder, discarded))
			End If
			Return conversion
		End Function

		Private Sub EnsureFullyBoundIfImplicitVariablesAllowed()
			If (Me.RootBinder.ImplicitVariableDeclarationAllowed AndAlso Not Me.RootBinder.AllImplicitVariableDeclarationsAreHandled) Then
				Me._rwLock.EnterWriteLock()
				Try
					If (Not Me.RootBinder.AllImplicitVariableDeclarationsAreHandled) Then
						Me.GuardedIncrementalBind(Me.Root, Me.RootBinder)
						Me.RootBinder.DisallowFurtherImplicitVariableDeclaration(Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded)
					End If
				Finally
					Me._rwLock.ExitWriteLock()
				End Try
			End If
		End Sub

		Private Function GetAggregateClauseLambdaBinder(ByVal aggregate As AggregateClauseSyntax, ByVal position As Integer) As Binder
			Dim collectionRangeVariablesLambdaBinder As Binder = Nothing
			If (SyntaxFacts.InSpanOrEffectiveTrailingOfNode(aggregate, position)) Then
				If (Not aggregate.IntoKeyword.IsMissing AndAlso aggregate.IntoKeyword.SpanStart <= position) Then
					Dim queryClauseLambdaBinders As ImmutableArray(Of Binder) = Me.GetQueryClauseLambdaBinders(aggregate)
					If (Not queryClauseLambdaBinders.IsEmpty) Then
						collectionRangeVariablesLambdaBinder = queryClauseLambdaBinders.Last()
					End If
				ElseIf (aggregate.AggregateKeyword.SpanStart <= position) Then
					collectionRangeVariablesLambdaBinder = Me.GetCollectionRangeVariablesLambdaBinder(aggregate.Variables, position)
					If (collectionRangeVariablesLambdaBinder Is Nothing) Then
						Dim binders As ImmutableArray(Of Binder) = Me.GetQueryClauseLambdaBinders(aggregate)
						If (Not binders.IsDefault AndAlso binders.Length = 2) Then
							collectionRangeVariablesLambdaBinder = binders(0)
						End If
					End If
				End If
			End If
			Return collectionRangeVariablesLambdaBinder
		End Function

		Friend Overrides Function GetAggregateClauseSymbolInfoWorker(ByVal node As AggregateClauseSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.VisualBasic.AggregateClauseSymbolInfo
			Dim aggregateClauseSymbolInfo As Microsoft.CodeAnalysis.VisualBasic.AggregateClauseSymbolInfo
			Dim lowerBoundNode As BoundNode = Me.GetLowerBoundNode(node)
			If (lowerBoundNode Is Nothing OrElse lowerBoundNode.Kind <> BoundKind.AggregateClause) Then
				aggregateClauseSymbolInfo = New Microsoft.CodeAnalysis.VisualBasic.AggregateClauseSymbolInfo(SymbolInfo.None, SymbolInfo.None)
			ElseIf (Not TypeOf DirectCast(lowerBoundNode, BoundAggregateClause).UnderlyingExpression Is BoundQueryClauseBase) Then
				Dim symbolInfoForNode As SymbolInfo = MyBase.GetSymbolInfoForNode(VBSemanticModel.SymbolInfoOptions.DefaultOptions, New BoundNodeSummary(lowerBoundNode, lowerBoundNode, Nothing), Nothing)
				Dim boundQueryClause As Microsoft.CodeAnalysis.VisualBasic.BoundQueryClause = DirectCast(MemberSemanticModel.CompilerGeneratedNodeFinder.FindIn(lowerBoundNode, node, BoundKind.QueryClause), Microsoft.CodeAnalysis.VisualBasic.BoundQueryClause)
				If (boundQueryClause Is Nothing) Then
					aggregateClauseSymbolInfo = New Microsoft.CodeAnalysis.VisualBasic.AggregateClauseSymbolInfo(symbolInfoForNode)
				Else
					aggregateClauseSymbolInfo = If(Not TypeOf boundQueryClause.UnderlyingExpression Is BoundQueryClauseBase, New Microsoft.CodeAnalysis.VisualBasic.AggregateClauseSymbolInfo(MyBase.GetSymbolInfoForNode(VBSemanticModel.SymbolInfoOptions.DefaultOptions, New BoundNodeSummary(boundQueryClause, boundQueryClause, Nothing), Nothing), symbolInfoForNode), New Microsoft.CodeAnalysis.VisualBasic.AggregateClauseSymbolInfo(SymbolInfo.None, symbolInfoForNode))
				End If
			Else
				aggregateClauseSymbolInfo = New Microsoft.CodeAnalysis.VisualBasic.AggregateClauseSymbolInfo(SymbolInfo.None, SymbolInfo.None)
			End If
			Return aggregateClauseSymbolInfo
		End Function

		Friend Overrides Function GetAttributeMemberGroup(ByVal attribute As AttributeSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of Symbol)
			Return MyBase.GetMemberGroupForNode(Me.GetBoundNodeSummary(attribute), Nothing)
		End Function

		Friend Overrides Function GetAttributeSymbolInfo(ByVal attribute As AttributeSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SymbolInfo
			Return MyBase.GetSymbolInfoForNode(VBSemanticModel.SymbolInfoOptions.DefaultOptions, Me.GetBoundNodeSummary(attribute), Nothing)
		End Function

		Friend Overrides Function GetAttributeTypeInfo(ByVal attribute As AttributeSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As VisualBasicTypeInfo
			Return MyBase.GetTypeInfoForNode(Me.GetBoundNodeSummary(attribute))
		End Function

		Friend Overrides Function GetAwaitExpressionInfoWorker(ByVal awaitExpression As AwaitExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.VisualBasic.AwaitExpressionInfo
			Dim awaitExpressionInfo As Microsoft.CodeAnalysis.VisualBasic.AwaitExpressionInfo
			Dim lowerBoundNode As BoundNode = Me.GetLowerBoundNode(awaitExpression)
			If (lowerBoundNode Is Nothing OrElse lowerBoundNode.Kind <> BoundKind.AwaitOperator) Then
				awaitExpressionInfo = New Microsoft.CodeAnalysis.VisualBasic.AwaitExpressionInfo()
			Else
				Dim boundAwaitOperator As Microsoft.CodeAnalysis.VisualBasic.BoundAwaitOperator = DirectCast(lowerBoundNode, Microsoft.CodeAnalysis.VisualBasic.BoundAwaitOperator)
				awaitExpressionInfo = New Microsoft.CodeAnalysis.VisualBasic.AwaitExpressionInfo(TryCast(boundAwaitOperator.GetAwaiter.ExpressionSymbol, MethodSymbol), TryCast(boundAwaitOperator.IsCompleted.ExpressionSymbol, PropertySymbol), TryCast(boundAwaitOperator.GetResult.ExpressionSymbol, MethodSymbol))
			End If
			Return awaitExpressionInfo
		End Function

		Protected Function GetBindableParent(ByVal node As VisualBasicSyntaxNode) As VisualBasicSyntaxNode
			Dim standaloneExpression As VisualBasicSyntaxNode
			Dim parent As VisualBasicSyntaxNode = node.Parent
			If (parent Is Nothing OrElse node = Me.Root) Then
				standaloneExpression = Nothing
			Else
				Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = TryCast(parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
				If (expressionSyntax Is Nothing) Then
					Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax = TryCast(parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
					If (statementSyntax Is Nothing OrElse Not MemberSemanticModel.IsStandaloneStatement(statementSyntax)) Then
						Dim attributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax = TryCast(parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax)
						If (attributeSyntax Is Nothing) Then
							standaloneExpression = Nothing
						Else
							standaloneExpression = attributeSyntax
						End If
					Else
						standaloneExpression = statementSyntax
					End If
				Else
					standaloneExpression = SyntaxFactory.GetStandaloneExpression(expressionSyntax)
				End If
			End If
			Return standaloneExpression
		End Function

		Private Function GetBindingRoot(ByVal node As SyntaxNode) As SyntaxNode
			Dim root As SyntaxNode
			Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax = Nothing
			While node <> Me.Root
				If (statementSyntax Is Nothing) Then
					Dim statementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
					If (statementSyntax1 IsNot Nothing AndAlso MemberSemanticModel.IsStandaloneStatement(statementSyntax1)) Then
						statementSyntax = statementSyntax1
					End If
				End If
				If (node.Kind() <> SyntaxKind.DocumentationCommentTrivia) Then
					If (node.IsLambdaExpressionSyntax()) Then
						statementSyntax = Nothing
					End If
					node = node.Parent
				Else
					node = DirectCast(DirectCast(node, DocumentationCommentTriviaSyntax).ParentTrivia.Token.Parent, VisualBasicSyntaxNode)
				End If
			End While
			If (statementSyntax Is Nothing) Then
				root = Me.Root
			Else
				root = statementSyntax
			End If
			Return root
		End Function

		Private Function GetBoundLambda(ByVal lambda As LambdaExpressionSyntax) As BoundLambda
			Return DirectCast(Me.GetLowerBoundNode(lambda), BoundLambda)
		End Function

		Friend Function GetBoundNodes(ByVal node As SyntaxNode) As ImmutableArray(Of BoundNode)
			Dim empty As ImmutableArray(Of BoundNode)
			Dim boundNodes As ImmutableArray(Of BoundNode) = New ImmutableArray(Of BoundNode)()
			Me.EnsureFullyBoundIfImplicitVariablesAllowed()
			Me._rwLock.EnterReadLock()
			Try
				boundNodes = Me.GuardedGetBoundNodesFromMap(node)
			Finally
				Me._rwLock.ExitReadLock()
			End Try
			If (Not boundNodes.IsDefault) Then
				empty = boundNodes
			ElseIf (Not MemberSemanticModel.IsNonExpressionCollectionInitializer(node)) Then
				Dim bindingRoot As SyntaxNode = Me.GetBindingRoot(node)
				Dim enclosingBinder As Binder = Me.GetEnclosingBinder(bindingRoot)
				Me._rwLock.EnterWriteLock()
				Try
					boundNodes = Me.GuardedGetBoundNodesFromMap(node)
					If (boundNodes.IsDefault) Then
						Me.GuardedIncrementalBind(bindingRoot, enclosingBinder)
					End If
					boundNodes = Me.GuardedGetBoundNodesFromMap(node)
					If (Not boundNodes.IsDefault) Then
						empty = boundNodes
						Return empty
					End If
				Finally
					Me._rwLock.ExitWriteLock()
				End Try
				If (TypeOf node Is ExpressionSyntax OrElse TypeOf node Is StatementSyntax) Then
					Dim incrementalBinder As MemberSemanticModel.IncrementalBinder = New MemberSemanticModel.IncrementalBinder(Me, Me.GetEnclosingBinder(node))
					Me._rwLock.EnterWriteLock()
					Try
						boundNodes = Me.GuardedGetBoundNodesFromMap(node)
						If (boundNodes.IsDefault) Then
							MemberSemanticModel.SemanticModelMapsBuilder.GuardedCacheBoundNodes(Me.Bind(incrementalBinder, node, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded), Me, Me._guardedBoundNodeMap, node)
						End If
						boundNodes = Me.GuardedGetBoundNodesFromMap(node)
						If (Not boundNodes.IsDefault) Then
							empty = boundNodes
							Return empty
						End If
					Finally
						Me._rwLock.ExitWriteLock()
					End Try
				End If
				empty = ImmutableArray(Of BoundNode).Empty
			Else
				empty = ImmutableArray(Of BoundNode).Empty
			End If
			Return empty
		End Function

		Friend Function GetBoundNodeSummary(ByVal node As VisualBasicSyntaxNode) As BoundNodeSummary
			Dim lowerBoundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim upperBoundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Me.GetUpperBoundNode(node)
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Me.GetLowerBoundNode(node)
			Dim bindableParent As VisualBasicSyntaxNode = Me.GetBindableParent(node)
			If (bindableParent Is Nothing) Then
				lowerBoundNode = Nothing
			Else
				lowerBoundNode = Me.GetLowerBoundNode(bindableParent)
			End If
			Return New BoundNodeSummary(boundNode, upperBoundNode, lowerBoundNode)
		End Function

		Friend Overridable Function GetBoundRoot() As BoundNode
			Return Me.GetUpperBoundNode(Me.Root)
		End Function

		Friend Overrides Function GetCollectionInitializerAddSymbolInfo(ByVal collectionInitializer As ObjectCreationExpressionSyntax, ByVal node As ExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SymbolInfo
			Dim none As SymbolInfo
			Dim lowerBoundNode As BoundCollectionInitializerExpression = TryCast(Me.GetLowerBoundNode(collectionInitializer.Initializer), BoundCollectionInitializerExpression)
			If (lowerBoundNode IsNot Nothing) Then
				Dim initializers As ImmutableArray(Of BoundExpression) = lowerBoundNode.Initializers
				Dim expressionSyntaxes As SeparatedSyntaxList(Of ExpressionSyntax) = DirectCast(collectionInitializer.Initializer, ObjectCollectionInitializerSyntax).Initializer.Initializers
				Dim item As BoundExpression = initializers(expressionSyntaxes.IndexOf(node))
				If (Not item.WasCompilerGenerated) Then
					none = SymbolInfo.None
					Return none
				End If
				none = MyBase.GetSymbolInfoForNode(VBSemanticModel.SymbolInfoOptions.DefaultOptions, New BoundNodeSummary(item, item, Nothing), Nothing)
				Return none
			End If
			none = SymbolInfo.None
			Return none
		End Function

		Private Function GetCollectionRangeVariablesLambdaBinder(ByVal variables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax), ByVal position As Integer) As Binder
			Dim item As Binder = Nothing
			Dim count As Integer = variables.Count - 1
			Dim num As Integer = 0
			While num <= count
				Dim collectionRangeVariableSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax = variables(num)
				If (SyntaxFacts.InSpanOrEffectiveTrailingOfNode(collectionRangeVariableSyntax, position) OrElse position < collectionRangeVariableSyntax.SpanStart) Then
					If (num <= 0 AndAlso (collectionRangeVariableSyntax.Parent.Kind() = SyntaxKind.AggregateClause OrElse collectionRangeVariableSyntax.Parent.Parent Is Nothing OrElse collectionRangeVariableSyntax.Parent.Parent.Kind() = SyntaxKind.QueryExpression AndAlso DirectCast(collectionRangeVariableSyntax.Parent.Parent, QueryExpressionSyntax).Clauses.FirstOrDefault() = collectionRangeVariableSyntax.Parent)) Then
						Exit While
					End If
					Dim queryClauseLambdaBinders As ImmutableArray(Of Binder) = Me.GetQueryClauseLambdaBinders(collectionRangeVariableSyntax)
					If (queryClauseLambdaBinders.IsEmpty) Then
						Exit While
					End If
					item = queryClauseLambdaBinders(0)
					Exit While
				Else
					num = num + 1
				End If
			End While
			Return item
		End Function

		Friend Overrides Function GetCollectionRangeVariableSymbolInfoWorker(ByVal node As CollectionRangeVariableSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.VisualBasic.CollectionRangeVariableSymbolInfo
			Dim collectionRangeVariableSymbolInfo As Microsoft.CodeAnalysis.VisualBasic.CollectionRangeVariableSymbolInfo
			Dim boundNodes As ImmutableArray(Of BoundNode) = Me.GetBoundNodes(node)
			Dim item As BoundNode = Nothing
			Dim length As Integer = boundNodes.Length - 1
			Dim num As Integer = 0
			While num <= length
				If (boundNodes(num).Kind = BoundKind.QueryClause OrElse boundNodes(num).Kind = BoundKind.QueryableSource) Then
					item = boundNodes(num)
					Exit While
				Else
					num = num + 1
				End If
			End While
			If (item IsNot Nothing) Then
				Dim none As Microsoft.CodeAnalysis.SymbolInfo = Microsoft.CodeAnalysis.SymbolInfo.None
				Dim symbolInfoForNode As Microsoft.CodeAnalysis.SymbolInfo = Microsoft.CodeAnalysis.SymbolInfo.None
				Dim symbolInfo As Microsoft.CodeAnalysis.SymbolInfo = Microsoft.CodeAnalysis.SymbolInfo.None
				If (item.Kind = BoundKind.QueryClause) Then
					symbolInfo = MyBase.GetSymbolInfoForNode(VBSemanticModel.SymbolInfoOptions.DefaultOptions, New BoundNodeSummary(item, item, Nothing), Nothing)
					item = Me.GetLowerBoundNode(node)
				End If
				If (item Is Nothing OrElse item.Kind <> BoundKind.QueryableSource) Then
					symbolInfo = Microsoft.CodeAnalysis.SymbolInfo.None
				Else
					Dim boundQueryableSource As Microsoft.CodeAnalysis.VisualBasic.BoundQueryableSource = DirectCast(item, Microsoft.CodeAnalysis.VisualBasic.BoundQueryableSource)
					Select Case boundQueryableSource.Source.Kind
						Case BoundKind.QuerySource
							symbolInfoForNode = Microsoft.CodeAnalysis.SymbolInfo.None
							none = Microsoft.CodeAnalysis.SymbolInfo.None
							Exit Select
						Case BoundKind.ToQueryableCollectionConversion
							symbolInfoForNode = Microsoft.CodeAnalysis.SymbolInfo.None
							none = MyBase.GetSymbolInfoForNode(VBSemanticModel.SymbolInfoOptions.DefaultOptions, New BoundNodeSummary(boundQueryableSource.Source, boundQueryableSource.Source, Nothing), Nothing)
							Exit Select
						Case BoundKind.QueryableSource
							Throw ExceptionUtilities.UnexpectedValue(boundQueryableSource.Source.Kind)
						Case BoundKind.QueryClause
							symbolInfoForNode = MyBase.GetSymbolInfoForNode(VBSemanticModel.SymbolInfoOptions.DefaultOptions, New BoundNodeSummary(boundQueryableSource.Source, boundQueryableSource.Source, Nothing), Nothing)
							Dim boundToQueryableCollectionConversion As Microsoft.CodeAnalysis.VisualBasic.BoundToQueryableCollectionConversion = DirectCast(MemberSemanticModel.CompilerGeneratedNodeFinder.FindIn(DirectCast(boundQueryableSource.Source, BoundQueryClause).UnderlyingExpression, node.Expression, BoundKind.ToQueryableCollectionConversion), Microsoft.CodeAnalysis.VisualBasic.BoundToQueryableCollectionConversion)
							If (boundToQueryableCollectionConversion IsNot Nothing) Then
								none = MyBase.GetSymbolInfoForNode(VBSemanticModel.SymbolInfoOptions.DefaultOptions, New BoundNodeSummary(boundToQueryableCollectionConversion, boundToQueryableCollectionConversion, Nothing), Nothing)
								Exit Select
							Else
								none = Microsoft.CodeAnalysis.SymbolInfo.None
								Exit Select
							End If
						Case Else
							Throw ExceptionUtilities.UnexpectedValue(boundQueryableSource.Source.Kind)
					End Select
				End If
				collectionRangeVariableSymbolInfo = New Microsoft.CodeAnalysis.VisualBasic.CollectionRangeVariableSymbolInfo(none, symbolInfoForNode, symbolInfo)
			Else
				collectionRangeVariableSymbolInfo = Microsoft.CodeAnalysis.VisualBasic.CollectionRangeVariableSymbolInfo.None
			End If
			Return collectionRangeVariableSymbolInfo
		End Function

		Friend Overrides Function GetCrefReferenceSymbolInfo(ByVal crefReference As CrefReferenceSyntax, ByVal options As VBSemanticModel.SymbolInfoOptions, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SymbolInfo
			Return SymbolInfo.None
		End Function

		Public Overrides Function GetDeclarationDiagnostics(Optional ByVal span As Nullable(Of TextSpan) = Nothing, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of Diagnostic)
			Throw New NotSupportedException()
		End Function

		Public Overrides Function GetDeclaredSymbol(ByVal declarationSyntax As TypeStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As INamedTypeSymbol
			Return Nothing
		End Function

		Public Overrides Function GetDeclaredSymbol(ByVal declarationSyntax As EnumStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As INamedTypeSymbol
			Return Nothing
		End Function

		Public Overrides Function GetDeclaredSymbol(ByVal declarationSyntax As NamespaceStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As INamespaceSymbol
			Return Nothing
		End Function

		Friend Overrides Function GetDeclaredSymbol(ByVal declarationSyntax As MethodBaseSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ISymbol
			Return Nothing
		End Function

		Public Overrides Function GetDeclaredSymbol(ByVal parameter As ParameterSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IParameterSymbol
			Dim parameterSymbol As IParameterSymbol
			Dim parent As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = parameter.Parent
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = parameter.Parent
			If (parent IsNot Nothing AndAlso parent.Kind() = SyntaxKind.ParameterList) Then
				Dim lambdaHeaderSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax = TryCast(parent.Parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax)
				If (lambdaHeaderSyntax IsNot Nothing) Then
					Dim lambdaExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaExpressionSyntax = TryCast(lambdaHeaderSyntax.Parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaExpressionSyntax)
					If (lambdaExpressionSyntax IsNot Nothing) Then
						Dim lowerBoundNode As BoundLambda = TryCast(Me.GetLowerBoundNode(lambdaExpressionSyntax), BoundLambda)
						If (lowerBoundNode IsNot Nothing) Then
							Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol).Enumerator = lowerBoundNode.LambdaSymbol.Parameters.GetEnumerator()
							While enumerator.MoveNext()
								Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = enumerator.Current
								Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.Location).Enumerator = current.Locations.GetEnumerator()
								While enumerator1.MoveNext()
									Dim location As Microsoft.CodeAnalysis.Location = enumerator1.Current
									If (Not parameter.Span.Contains(location.SourceSpan)) Then
										Continue While
									End If
									parameterSymbol = current
									Return parameterSymbol
								End While
							End While
						End If
					End If
				End If
			End If
			parameterSymbol = Nothing
			Return parameterSymbol
		End Function

		Public Overrides Function GetDeclaredSymbol(ByVal declarationSyntax As SimpleImportsClauseSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IAliasSymbol
			Return Nothing
		End Function

		Public Overrides Function GetDeclaredSymbol(ByVal typeParameter As TypeParameterSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ITypeParameterSymbol
			Return Nothing
		End Function

		Public Overrides Function GetDeclaredSymbol(ByVal declarationSyntax As EnumMemberDeclarationSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IFieldSymbol
			Return Nothing
		End Function

		Public Overrides Function GetDeclaredSymbol(ByVal identifierSyntax As ModifiedIdentifierSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ISymbol
			Dim declaredSymbol As ISymbol
			If (identifierSyntax Is Nothing) Then
				Throw New ArgumentNullException("identifierSyntax")
			End If
			If (Not MyBase.IsInTree(identifierSyntax)) Then
				Throw New ArgumentException(VBResources.IdentifierSyntaxNotWithinSyntaxTree)
			End If
			Dim parent As VisualBasicSyntaxNode = identifierSyntax.Parent
			If (parent IsNot Nothing) Then
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = parent.Kind()
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CollectionRangeVariable) Then
					declaredSymbol = Me.GetDeclaredSymbol(DirectCast(parent, CollectionRangeVariableSyntax), cancellationToken)
					Return declaredSymbol
				ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariableNameEquals) Then
					parent = parent.Parent
					If (parent IsNot Nothing) Then
						Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = parent.Kind()
						If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExpressionRangeVariable) Then
							declaredSymbol = Me.GetDeclaredSymbol(DirectCast(parent, ExpressionRangeVariableSyntax), cancellationToken)
							Return declaredSymbol
						Else
							If (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregationRangeVariable) Then
								declaredSymbol = MyBase.GetDeclaredSymbol(identifierSyntax, cancellationToken)
								Return declaredSymbol
							End If
							declaredSymbol = Me.GetDeclaredSymbol(DirectCast(parent, AggregationRangeVariableSyntax), cancellationToken)
							Return declaredSymbol
						End If
					End If
				End If
			End If
			declaredSymbol = MyBase.GetDeclaredSymbol(identifierSyntax, cancellationToken)
			Return declaredSymbol
		End Function

		Public Overrides Function GetDeclaredSymbol(ByVal anonymousObjectCreationExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AnonymousObjectCreationExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As INamedTypeSymbol
			Dim type As INamedTypeSymbol
			If (anonymousObjectCreationExpressionSyntax Is Nothing) Then
				Throw New ArgumentNullException("anonymousObjectCreationExpressionSyntax")
			End If
			If (Not MyBase.IsInTree(anonymousObjectCreationExpressionSyntax)) Then
				Throw New ArgumentException(VBResources.AnonymousObjectCreationExpressionSyntaxNotWithinTree)
			End If
			Dim lowerBoundNode As BoundExpression = TryCast(Me.GetLowerBoundNode(anonymousObjectCreationExpressionSyntax), BoundExpression)
			If (lowerBoundNode IsNot Nothing) Then
				type = TryCast(lowerBoundNode.Type, AnonymousTypeManager.AnonymousTypePublicSymbol)
			Else
				type = Nothing
			End If
			Return type
		End Function

		Public Overrides Function GetDeclaredSymbol(ByVal fieldInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldInitializerSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IPropertySymbol
			Dim item As IPropertySymbol
			If (fieldInitializerSyntax Is Nothing) Then
				Throw New ArgumentNullException("fieldInitializerSyntax")
			End If
			If (Not MyBase.IsInTree(fieldInitializerSyntax)) Then
				Throw New ArgumentException(VBResources.FieldInitializerSyntaxNotWithinSyntaxTree)
			End If
			Dim parent As ObjectMemberInitializerSyntax = TryCast(fieldInitializerSyntax.Parent, ObjectMemberInitializerSyntax)
			If (parent IsNot Nothing) Then
				Dim anonymousObjectCreationExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AnonymousObjectCreationExpressionSyntax = TryCast(parent.Parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.AnonymousObjectCreationExpressionSyntax)
				If (anonymousObjectCreationExpressionSyntax IsNot Nothing) Then
					Dim lowerBoundNode As BoundExpression = TryCast(Me.GetLowerBoundNode(anonymousObjectCreationExpressionSyntax), BoundExpression)
					If (lowerBoundNode IsNot Nothing) Then
						Dim type As AnonymousTypeManager.AnonymousTypePublicSymbol = TryCast(lowerBoundNode.Type, AnonymousTypeManager.AnonymousTypePublicSymbol)
						If (type IsNot Nothing) Then
							Dim num As Integer = parent.Initializers.IndexOf(fieldInitializerSyntax)
							item = type.Properties(num)
						Else
							item = Nothing
						End If
					Else
						item = Nothing
					End If
				Else
					item = Nothing
				End If
			Else
				item = Nothing
			End If
			Return item
		End Function

		Public Overrides Function GetDeclaredSymbol(ByVal rangeVariableSyntax As CollectionRangeVariableSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IRangeVariableSymbol
			Dim declaredSymbol As IRangeVariableSymbol
			If (rangeVariableSyntax Is Nothing) Then
				Throw New ArgumentNullException("rangeVariableSyntax")
			End If
			If (Not MyBase.IsInTree(rangeVariableSyntax)) Then
				Throw New ArgumentException(VBResources.IdentifierSyntaxNotWithinSyntaxTree)
			End If
			Dim lowerBoundNode As BoundNode = Me.GetLowerBoundNode(rangeVariableSyntax)
			If (lowerBoundNode IsNot Nothing AndAlso lowerBoundNode.Kind = BoundKind.QueryableSource) Then
				Dim boundQueryableSource As Microsoft.CodeAnalysis.VisualBasic.BoundQueryableSource = DirectCast(lowerBoundNode, Microsoft.CodeAnalysis.VisualBasic.BoundQueryableSource)
				If (boundQueryableSource.RangeVariableOpt Is Nothing) Then
					declaredSymbol = MyBase.GetDeclaredSymbol(rangeVariableSyntax, cancellationToken)
					Return declaredSymbol
				End If
				declaredSymbol = boundQueryableSource.RangeVariableOpt
				Return declaredSymbol
			End If
			declaredSymbol = MyBase.GetDeclaredSymbol(rangeVariableSyntax, cancellationToken)
			Return declaredSymbol
		End Function

		Public Overrides Function GetDeclaredSymbol(ByVal rangeVariableSyntax As ExpressionRangeVariableSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IRangeVariableSymbol
			Dim declaredSymbol As IRangeVariableSymbol
			If (rangeVariableSyntax Is Nothing) Then
				Throw New ArgumentNullException("rangeVariableSyntax")
			End If
			If (Not MyBase.IsInTree(rangeVariableSyntax)) Then
				Throw New ArgumentException(VBResources.IdentifierSyntaxNotWithinSyntaxTree)
			End If
			Dim lowerBoundNode As BoundNode = Me.GetLowerBoundNode(rangeVariableSyntax)
			If (lowerBoundNode Is Nothing OrElse lowerBoundNode.Kind <> BoundKind.RangeVariableAssignment) Then
				declaredSymbol = MyBase.GetDeclaredSymbol(rangeVariableSyntax, cancellationToken)
			Else
				declaredSymbol = DirectCast(lowerBoundNode, BoundRangeVariableAssignment).RangeVariable
			End If
			Return declaredSymbol
		End Function

		Public Overrides Function GetDeclaredSymbol(ByVal rangeVariableSyntax As AggregationRangeVariableSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IRangeVariableSymbol
			Dim declaredSymbol As IRangeVariableSymbol
			If (rangeVariableSyntax Is Nothing) Then
				Throw New ArgumentNullException("rangeVariableSyntax")
			End If
			If (Not MyBase.IsInTree(rangeVariableSyntax)) Then
				Throw New ArgumentException(VBResources.IdentifierSyntaxNotWithinSyntaxTree)
			End If
			Dim lowerBoundNode As BoundNode = Me.GetLowerBoundNode(rangeVariableSyntax)
			If (lowerBoundNode Is Nothing OrElse lowerBoundNode.Kind <> BoundKind.RangeVariableAssignment) Then
				declaredSymbol = MyBase.GetDeclaredSymbol(rangeVariableSyntax, cancellationToken)
			Else
				declaredSymbol = DirectCast(lowerBoundNode, BoundRangeVariableAssignment).RangeVariable
			End If
			Return declaredSymbol
		End Function

		Friend Overrides Function GetDeclaredSymbols(ByVal declarationSyntax As FieldDeclarationSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of ISymbol)
			Return ImmutableArray.Create(Of ISymbol)()
		End Function

		Public Overrides Function GetDiagnostics(Optional ByVal span As Nullable(Of TextSpan) = Nothing, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of Diagnostic)
			Throw New NotSupportedException()
		End Function

		Friend NotOverridable Overrides Function GetEnclosingBinder(ByVal position As Integer) As Binder
			Return SemanticModelBinder.Mark(Me.GetEnclosingBinderInternal(Me.RootBinder, Me.Root, MyBase.FindInitialNodeFromPosition(position), position), Me.IgnoresAccessibility)
		End Function

		Private Function GetEnclosingBinder(ByVal node As SyntaxNode) As Binder
			Return SemanticModelBinder.Mark(Me.GetEnclosingBinderInternal(Me.RootBinder, Me.Root, node, node.SpanStart), Me.IgnoresAccessibility)
		End Function

		Private Function GetEnclosingBinderInternal(ByVal memberBinder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal binderRoot As SyntaxNode, ByVal node As SyntaxNode, ByVal position As Integer) As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim enclosingBinderInternal As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Nothing
			Me.EnsureFullyBoundIfImplicitVariablesAllowed()
			Dim parent As SyntaxNode = node
			While True
				Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
				If (parent.Kind() <> SyntaxKind.DocumentationCommentTrivia) Then
					If (SyntaxFacts.InBlockInterior(parent, position, statementSyntaxes)) Then
						binder = memberBinder.GetBinder(statementSyntaxes)
						If (binder IsNot Nothing) Then
							enclosingBinderInternal = binder
							Exit While
						End If
					ElseIf (SyntaxFacts.InLambdaInterior(parent, position)) Then
						If (parent <> binderRoot) Then
							Dim lambdaBodyBinder As Microsoft.CodeAnalysis.VisualBasic.LambdaBodyBinder = Me.GetLambdaBodyBinder(DirectCast(parent, LambdaExpressionSyntax))
							If (lambdaBodyBinder IsNot Nothing) Then
								If ((parent.Kind() = SyntaxKind.MultiLineFunctionLambdaExpression OrElse parent.Kind() = SyntaxKind.MultiLineSubLambdaExpression) AndAlso DirectCast(parent, MultiLineLambdaExpressionSyntax).SubOrFunctionHeader.FullSpan.Contains(position)) Then
									enclosingBinderInternal = lambdaBodyBinder
									Exit While
								Else
									binder = Me.GetEnclosingBinderInternal(lambdaBodyBinder, lambdaBodyBinder.Root, node, position)
									If (binder IsNot Nothing) Then
										enclosingBinderInternal = binder
										Exit While
									End If
								End If
							End If
						ElseIf (parent.Kind() = SyntaxKind.MultiLineFunctionLambdaExpression OrElse parent.Kind() = SyntaxKind.MultiLineSubLambdaExpression) Then
							binder = memberBinder.GetBinder(DirectCast(parent, MultiLineLambdaExpressionSyntax).Statements)
							If (binder IsNot Nothing) Then
								enclosingBinderInternal = binder
								Exit While
							End If
						ElseIf (parent.Kind() = SyntaxKind.SingleLineSubLambdaExpression) Then
							binder = memberBinder.GetBinder(DirectCast(parent, SingleLineLambdaExpressionSyntax).Statements)
							If (binder IsNot Nothing) Then
								enclosingBinderInternal = binder
								Exit While
							End If
						End If
					ElseIf (Me.InQueryInterior(parent, position, binder)) Then
						enclosingBinderInternal = binder
						Exit While
					ElseIf (Me.InAnonymousTypeInitializerInterior(parent, position, binder)) Then
						enclosingBinderInternal = binder
						Exit While
					ElseIf (MemberSemanticModel.InWithStatementExpressionInterior(parent)) Then
						parent = parent.Parent.Parent
						If (parent <> binderRoot) Then
							parent = parent.Parent
						Else
							enclosingBinderInternal = memberBinder
							Exit While
						End If
					End If
					binder = memberBinder.GetBinder(parent)
					If (binder IsNot Nothing) Then
						enclosingBinderInternal = binder
						Exit While
					ElseIf (parent <> binderRoot) Then
						parent = parent.Parent
					Else
						enclosingBinderInternal = memberBinder
						Exit While
					End If
				Else
					Dim token As SyntaxToken = DirectCast(parent, DocumentationCommentTriviaSyntax).ParentTrivia.Token
					enclosingBinderInternal = Me.GetEnclosingBinderInternal(memberBinder, binderRoot, DirectCast(token.Parent, VisualBasicSyntaxNode), position)
					Exit While
				End If
			End While
			Return enclosingBinderInternal
		End Function

		Friend Overrides Function GetExpressionConstantValue(ByVal node As ExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ConstantValue
			Dim constantValueForNode As ConstantValue
			If (Me.IsSpeculativeSemanticModel) Then
				node = SyntaxFactory.GetStandaloneExpression(node)
			End If
			If (node.EnclosingStructuredTrivia() Is Nothing) Then
				constantValueForNode = MyBase.GetConstantValueForNode(Me.GetBoundNodeSummary(node))
			Else
				constantValueForNode = Nothing
			End If
			Return constantValueForNode
		End Function

		Friend Overrides Function GetExpressionMemberGroup(ByVal node As ExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of Symbol)
			Dim memberGroupForNode As ImmutableArray(Of Symbol)
			If (Me.IsSpeculativeSemanticModel) Then
				node = SyntaxFactory.GetStandaloneExpression(node)
			End If
			If (node.EnclosingStructuredTrivia() Is Nothing) Then
				memberGroupForNode = MyBase.GetMemberGroupForNode(Me.GetBoundNodeSummary(node), Nothing)
			Else
				memberGroupForNode = ImmutableArray(Of Symbol).Empty
			End If
			Return memberGroupForNode
		End Function

		Friend Overrides Function GetExpressionSymbolInfo(ByVal node As ExpressionSyntax, ByVal options As VBSemanticModel.SymbolInfoOptions, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.SymbolInfo
			Dim symbolInfo As Microsoft.CodeAnalysis.SymbolInfo
			MyBase.ValidateSymbolInfoOptions(options)
			If (Me.IsSpeculativeSemanticModel) Then
				node = SyntaxFactory.GetStandaloneExpression(node)
			End If
			symbolInfo = If(node.EnclosingStructuredTrivia() Is Nothing, MyBase.GetSymbolInfoForNode(options, Me.GetBoundNodeSummary(node), Nothing), Microsoft.CodeAnalysis.SymbolInfo.None)
			Return symbolInfo
		End Function

		Friend Overrides Function GetExpressionTypeInfo(ByVal node As ExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.VisualBasic.VisualBasicTypeInfo
			Dim visualBasicTypeInfo As Microsoft.CodeAnalysis.VisualBasic.VisualBasicTypeInfo
			If (Me.IsSpeculativeSemanticModel) Then
				node = SyntaxFactory.GetStandaloneExpression(node)
			End If
			visualBasicTypeInfo = If(node.EnclosingStructuredTrivia() Is Nothing, MyBase.GetTypeInfoForNode(Me.GetBoundNodeSummary(node)), Microsoft.CodeAnalysis.VisualBasic.VisualBasicTypeInfo.None)
			Return visualBasicTypeInfo
		End Function

		Friend Shared Function GetForEachStatementInfo(ByVal boundForEach As BoundForEachStatement, ByVal compilation As VisualBasicCompilation, <Out> ByRef getEnumeratorArguments As ImmutableArray(Of BoundExpression), <Out> ByRef getEnumeratorDefaultArguments As BitVector, <Out> ByRef moveNextArguments As ImmutableArray(Of BoundExpression), <Out> ByRef moveNextDefaultArguments As BitVector, <Out> ByRef currentArguments As ImmutableArray(Of BoundExpression), <Out> ByRef currentDefaultArguments As BitVector) As ForEachStatementInfo
			Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol)
			Dim specialTypeMember As IMethodSymbol
			getEnumeratorArguments = New ImmutableArray(Of BoundExpression)()
			moveNextArguments = New ImmutableArray(Of BoundExpression)()
			currentArguments = New ImmutableArray(Of BoundExpression)()
			Dim enumeratorInfo As ForEachEnumeratorInfo = boundForEach.EnumeratorInfo
			Dim method As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
			If (enumeratorInfo.GetEnumerator IsNot Nothing AndAlso enumeratorInfo.GetEnumerator.Kind = BoundKind.[Call]) Then
				Dim getEnumerator As BoundCall = DirectCast(enumeratorInfo.GetEnumerator, BoundCall)
				method = getEnumerator.Method
				getEnumeratorArguments = getEnumerator.Arguments
				getEnumeratorDefaultArguments = getEnumerator.DefaultArguments
			End If
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
			If (enumeratorInfo.MoveNext IsNot Nothing AndAlso enumeratorInfo.MoveNext.Kind = BoundKind.[Call]) Then
				Dim moveNext As BoundCall = DirectCast(enumeratorInfo.MoveNext, BoundCall)
				methodSymbol = moveNext.Method
				moveNextArguments = moveNext.Arguments
				moveNextDefaultArguments = moveNext.DefaultArguments
			End If
			Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = Nothing
			If (enumeratorInfo.Current IsNot Nothing AndAlso enumeratorInfo.Current.Kind = BoundKind.PropertyAccess) Then
				Dim current As BoundPropertyAccess = DirectCast(enumeratorInfo.Current, BoundPropertyAccess)
				propertySymbol = current.PropertySymbol
				currentArguments = current.Arguments
				currentDefaultArguments = current.DefaultArguments
			End If
			Dim conversion As Microsoft.CodeAnalysis.VisualBasic.Conversion = New Microsoft.CodeAnalysis.VisualBasic.Conversion()
			Dim conversion1 As Microsoft.CodeAnalysis.VisualBasic.Conversion = New Microsoft.CodeAnalysis.VisualBasic.Conversion()
			Dim elementType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = enumeratorInfo.ElementType
			If (elementType IsNot Nothing AndAlso Not elementType.IsErrorType()) Then
				If (propertySymbol IsNot Nothing AndAlso Not propertySymbol.Type.IsErrorType()) Then
					Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = propertySymbol.Type
					discarded = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
					conversion = New Microsoft.CodeAnalysis.VisualBasic.Conversion(Conversions.ClassifyConversion(type, elementType, discarded))
				End If
				Dim currentConversion As BoundExpression = enumeratorInfo.CurrentConversion
				If (currentConversion IsNot Nothing AndAlso Not currentConversion.Type.IsErrorType()) Then
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = currentConversion.Type
					discarded = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
					conversion1 = New Microsoft.CodeAnalysis.VisualBasic.Conversion(Conversions.ClassifyConversion(elementType, typeSymbol, discarded))
				End If
			End If
			Dim collection As BoundExpression = boundForEach.Collection
			If (collection.Kind = BoundKind.Conversion) Then
				Dim boundConversion As Microsoft.CodeAnalysis.VisualBasic.BoundConversion = DirectCast(collection, Microsoft.CodeAnalysis.VisualBasic.BoundConversion)
				If (Not boundConversion.ExplicitCastInCode) Then
					collection = boundConversion.Operand
				End If
			End If
			Dim methodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = method
			Dim methodSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = methodSymbol
			Dim propertySymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = propertySymbol
			If (enumeratorInfo.NeedToDispose OrElse collection.Type IsNot Nothing AndAlso collection.Type.IsArrayType()) Then
				specialTypeMember = DirectCast(compilation.GetSpecialTypeMember(SpecialMember.System_IDisposable__Dispose), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			Else
				specialTypeMember = Nothing
			End If
			Return New ForEachStatementInfo(methodSymbol1, methodSymbol2, propertySymbol1, specialTypeMember, elementType, conversion1, conversion)
		End Function

		Friend Overrides Function GetForEachStatementInfoWorker(ByVal node As ForEachBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.ForEachStatementInfo
			Dim forEachStatementInfo As Microsoft.CodeAnalysis.VisualBasic.ForEachStatementInfo
			Dim upperBoundNode As BoundForEachStatement = DirectCast(Me.GetUpperBoundNode(node), BoundForEachStatement)
			If (upperBoundNode Is Nothing) Then
				forEachStatementInfo = New Microsoft.CodeAnalysis.VisualBasic.ForEachStatementInfo()
			Else
				Dim compilation As VisualBasicCompilation = Me.Compilation
				Dim boundExpressions As ImmutableArray(Of BoundExpression) = New ImmutableArray(Of BoundExpression)()
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				Dim boundExpressions1 As ImmutableArray(Of BoundExpression) = New ImmutableArray(Of BoundExpression)()
				Dim bitVector1 As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				Dim boundExpressions2 As ImmutableArray(Of BoundExpression) = New ImmutableArray(Of BoundExpression)()
				Dim bitVector2 As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				forEachStatementInfo = MemberSemanticModel.GetForEachStatementInfo(upperBoundNode, compilation, boundExpressions, bitVector, boundExpressions1, bitVector1, boundExpressions2, bitVector2)
			End If
			Return forEachStatementInfo
		End Function

		Private Function GetFromClauseLambdaBinder(ByVal from As FromClauseSyntax, ByVal position As Integer) As Binder
			Dim collectionRangeVariablesLambdaBinder As Binder = Nothing
			If (SyntaxFacts.InSpanOrEffectiveTrailingOfNode(from, position)) Then
				collectionRangeVariablesLambdaBinder = Me.GetCollectionRangeVariablesLambdaBinder(from.Variables, position)
			End If
			Return collectionRangeVariablesLambdaBinder
		End Function

		Private Function GetFunctionAggregationLambdaBinder(ByVal func As FunctionAggregationSyntax, ByVal position As Integer) As Binder
			Dim item As Binder = Nothing
			If (Not func.OpenParenToken.IsMissing AndAlso func.OpenParenToken.SpanStart <= position AndAlso (func.CloseParenToken.IsMissing AndAlso SyntaxFacts.InSpanOrEffectiveTrailingOfNode(func, position) OrElse position < func.CloseParenToken.SpanStart)) Then
				Dim queryClauseLambdaBinders As ImmutableArray(Of Binder) = Me.GetQueryClauseLambdaBinders(func)
				If (Not queryClauseLambdaBinders.IsDefaultOrEmpty) Then
					item = queryClauseLambdaBinders(0)
				End If
			End If
			Return item
		End Function

		Private Function GetGroupByClauseLambdaBinder(ByVal groupBy As GroupByClauseSyntax, ByVal position As Integer) As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Nothing
			If (SyntaxFacts.InSpanOrEffectiveTrailingOfNode(groupBy, position)) Then
				Dim queryClauseLambdaBinders As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Binder) = Me.GetQueryClauseLambdaBinders(groupBy)
				If (Not queryClauseLambdaBinders.IsEmpty) Then
					If (position >= groupBy.ByKeyword.SpanStart) Then
						binder = If(position >= groupBy.IntoKeyword.SpanStart, queryClauseLambdaBinders.Last(), queryClauseLambdaBinders(0))
					Else
						binder = If(queryClauseLambdaBinders.Length > 2, queryClauseLambdaBinders(1), queryClauseLambdaBinders(0))
					End If
				End If
			End If
			Return binder
		End Function

		Private Function GetGroupJoinClauseLambdaBinder(ByVal join As GroupJoinClauseSyntax, ByVal position As Integer) As Binder
			Dim joinClauseLambdaBinder As Binder = Nothing
			If (SyntaxFacts.InSpanOrEffectiveTrailingOfNode(join, position)) Then
				If (join.IntoKeyword.IsMissing OrElse join.IntoKeyword.SpanStart > position) Then
					joinClauseLambdaBinder = Me.GetJoinClauseLambdaBinder(join, position)
				Else
					Dim queryClauseLambdaBinders As ImmutableArray(Of Binder) = Me.GetQueryClauseLambdaBinders(join)
					If (Not queryClauseLambdaBinders.IsDefault AndAlso queryClauseLambdaBinders.Length = 3) Then
						joinClauseLambdaBinder = queryClauseLambdaBinders(2)
					End If
				End If
			End If
			Return joinClauseLambdaBinder
		End Function

		Friend Overrides Function GetInvokeSummaryForRaiseEvent(ByVal node As RaiseEventStatementSyntax) As BoundNodeSummary
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode1 As Microsoft.CodeAnalysis.VisualBasic.BoundNode = MemberSemanticModel.UnwrapRaiseEvent(Me.GetUpperBoundNode(node))
			Dim boundNode2 As Microsoft.CodeAnalysis.VisualBasic.BoundNode = MemberSemanticModel.UnwrapRaiseEvent(Me.GetLowerBoundNode(node))
			Dim bindableParent As VisualBasicSyntaxNode = Me.GetBindableParent(node)
			If (bindableParent Is Nothing) Then
				boundNode = Nothing
			Else
				boundNode = MemberSemanticModel.UnwrapRaiseEvent(Me.GetLowerBoundNode(bindableParent))
			End If
			Return New BoundNodeSummary(boundNode2, boundNode1, boundNode)
		End Function

		Private Function GetJoinClauseLambdaBinder(ByVal join As JoinClauseSyntax, ByVal position As Integer) As Binder
			Dim item As Binder = Nothing
			If (Not join.OnKeyword.IsMissing AndAlso join.OnKeyword.SpanStart <= position AndAlso SyntaxFacts.InSpanOrEffectiveTrailingOfNode(join, position)) Then
				Dim queryClauseLambdaBinders As ImmutableArray(Of Binder) = Me.GetQueryClauseLambdaBinders(join)
				If (Not queryClauseLambdaBinders.IsEmpty) Then
					item = queryClauseLambdaBinders(0)
				End If
			End If
			Return item
		End Function

		Private Function GetLambdaBodyBinder(ByVal lambda As LambdaExpressionSyntax) As LambdaBodyBinder
			Dim lambdaBinderOpt As LambdaBodyBinder
			Dim boundLambda As Microsoft.CodeAnalysis.VisualBasic.BoundLambda = Me.GetBoundLambda(lambda)
			If (boundLambda Is Nothing) Then
				lambdaBinderOpt = Nothing
			Else
				lambdaBinderOpt = boundLambda.LambdaBinderOpt
			End If
			Return lambdaBinderOpt
		End Function

		Private Function GetLetClauseLambdaBinder(ByVal [let] As LetClauseSyntax, ByVal position As Integer) As Binder
			Dim current As ExpressionRangeVariableSyntax
			Dim item As Binder = Nothing
			If (SyntaxFacts.InSpanOrEffectiveTrailingOfNode([let], position)) Then
				Dim enumerator As SeparatedSyntaxList(Of ExpressionRangeVariableSyntax).Enumerator = [let].Variables.GetEnumerator()
				Do
					If (Not enumerator.MoveNext()) Then
						Return item
					End If
					current = enumerator.Current
				Loop While Not SyntaxFacts.InSpanOrEffectiveTrailingOfNode(current, position) AndAlso position >= current.SpanStart
				Dim queryClauseLambdaBinders As ImmutableArray(Of Binder) = Me.GetQueryClauseLambdaBinders(current)
				If (Not queryClauseLambdaBinders.IsEmpty) Then
					item = queryClauseLambdaBinders(0)
				End If
			End If
			Return item
		End Function

		Friend Overrides Function GetLetClauseSymbolInfo(ByVal node As ExpressionRangeVariableSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SymbolInfo
			Dim none As SymbolInfo
			Dim upperBoundNode As BoundNode = Me.GetUpperBoundNode(node)
			If (upperBoundNode Is Nothing OrElse upperBoundNode.Kind <> BoundKind.QueryClause) Then
				none = SymbolInfo.None
			Else
				none = If(DirectCast(upperBoundNode, BoundQueryClause).UnderlyingExpression.Kind <> BoundKind.QueryClause, MyBase.GetSymbolInfoForNode(VBSemanticModel.SymbolInfoOptions.DefaultOptions, New BoundNodeSummary(upperBoundNode, upperBoundNode, Nothing), Nothing), SymbolInfo.None)
			End If
			Return none
		End Function

		Friend Function GetLowerBoundNode(ByVal node As VisualBasicSyntaxNode) As BoundNode
			Dim item As BoundNode
			Dim boundNodes As ImmutableArray(Of BoundNode) = Me.GetBoundNodes(node)
			If (boundNodes.Length <> 0) Then
				item = boundNodes(boundNodes.Length - 1)
			Else
				item = Nothing
			End If
			Return item
		End Function

		Public Overrides Function GetMethodBodyDiagnostics(Optional ByVal span As Nullable(Of TextSpan) = Nothing, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of Diagnostic)
			Throw New NotSupportedException()
		End Function

		Friend Overrides Function GetOperationWorker(ByVal node As VisualBasicSyntaxNode, ByVal cancellationToken As System.Threading.CancellationToken) As IOperation
			Dim operation As IOperation
			Dim operation1 As IOperation
			Dim operation2 As IOperation
			Dim operation3 As IOperation
			Dim operation4 As IOperation = Nothing
			Try
				Me._rwLock.EnterReadLock()
				If (Me._guardedIOperationNodeMap.Count > 0) Then
					If (Me._guardedIOperationNodeMap.TryGetValue(node, operation4)) Then
						operation3 = operation4
					Else
						operation3 = Nothing
					End If
					operation = operation3
					Return operation
				End If
			Finally
				Me._rwLock.ExitReadLock()
			End Try
			Dim boundRoot As BoundNode = Me.GetBoundRoot()
			Dim operation5 As IOperation = Me._operationFactory.Value.Create(boundRoot)
			Try
				Me._rwLock.EnterWriteLock()
				If (Me._guardedIOperationNodeMap.Count <= 0) Then
					Microsoft.CodeAnalysis.Operation.SetParentOperation(Of IOperation)(operation5, Nothing)
					OperationMapBuilder.AddToMap(operation5, Me._guardedIOperationNodeMap)
					If (Me._guardedIOperationNodeMap.TryGetValue(node, operation4)) Then
						operation1 = operation4
					Else
						operation1 = Nothing
					End If
					operation = operation1
				Else
					If (Me._guardedIOperationNodeMap.TryGetValue(node, operation4)) Then
						operation2 = operation4
					Else
						operation2 = Nothing
					End If
					operation = operation2
				End If
			Finally
				Me._rwLock.ExitWriteLock()
			End Try
			Return operation
		End Function

		Friend Overrides Function GetOrderingSymbolInfo(ByVal node As OrderingSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.SymbolInfo
			Dim symbolInfo As Microsoft.CodeAnalysis.SymbolInfo
			Dim lowerBoundNode As BoundNode = Me.GetLowerBoundNode(node)
			symbolInfo = If(lowerBoundNode Is Nothing OrElse lowerBoundNode.Kind <> BoundKind.Ordering, Microsoft.CodeAnalysis.SymbolInfo.None, MyBase.GetSymbolInfoForNode(VBSemanticModel.SymbolInfoOptions.DefaultOptions, New BoundNodeSummary(lowerBoundNode, lowerBoundNode, Nothing), Nothing))
			Return symbolInfo
		End Function

		Private Function GetQueryClauseLambdaBinders(ByVal node As VisualBasicSyntaxNode) As ImmutableArray(Of Binder)
			Dim binders As ImmutableArray(Of Binder)
			Dim binders1 As ImmutableArray(Of Binder) = New ImmutableArray(Of Binder)()
			Me._rwLock.EnterReadLock()
			Try
				If (Me._guardedQueryBindersMap.TryGetValue(node, binders1)) Then
					binders = binders1
					Return binders
				End If
			Finally
				Me._rwLock.ExitReadLock()
			End Try
			Dim upperBoundNode As BoundNode = Me.GetUpperBoundNode(node)
			Me._rwLock.EnterWriteLock()
			Try
				If (Not Me._guardedQueryBindersMap.TryGetValue(node, binders1)) Then
					If (upperBoundNode IsNot Nothing AndAlso upperBoundNode.Kind = BoundKind.NoOpStatement) Then
						Dim hasErrors As Boolean = upperBoundNode.HasErrors
					End If
					Me._guardedQueryBindersMap.Add(node, New ImmutableArray(Of Binder)())
					binders = New ImmutableArray(Of Binder)()
				Else
					binders = binders1
				End If
			Finally
				Me._rwLock.ExitWriteLock()
			End Try
			Return binders
		End Function

		Friend Overrides Function GetQueryClauseSymbolInfo(ByVal node As QueryClauseSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SymbolInfo
			Dim none As SymbolInfo
			Dim lowerBoundNode As BoundNode
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = node.Kind()
			If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FromClause) Then
				lowerBoundNode = Me.GetLowerBoundNode(node)
				If (lowerBoundNode Is Nothing OrElse lowerBoundNode.Kind <> BoundKind.QueryClause) Then
					none = SymbolInfo.None
				Else
					none = If(syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectClause OrElse DirectCast(lowerBoundNode, BoundQueryClause).UnderlyingExpression.Kind <> BoundKind.QueryClause, MyBase.GetSymbolInfoForNode(VBSemanticModel.SymbolInfoOptions.DefaultOptions, New BoundNodeSummary(lowerBoundNode, lowerBoundNode, Nothing), Nothing), SymbolInfo.None)
				End If
			Else
				If (DirectCast(node, FromClauseSyntax).Variables.Count < 2 AndAlso node.Parent IsNot Nothing AndAlso node.Parent.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.QueryExpression) Then
					Dim parent As QueryExpressionSyntax = DirectCast(node.Parent, QueryExpressionSyntax)
					If (parent.Clauses.Count = 1 AndAlso parent.Clauses(0) = node) Then
						lowerBoundNode = Me.GetLowerBoundNode(parent)
						If (lowerBoundNode IsNot Nothing AndAlso lowerBoundNode.Kind = BoundKind.QueryExpression) Then
							Dim boundQueryExpression As Microsoft.CodeAnalysis.VisualBasic.BoundQueryExpression = DirectCast(lowerBoundNode, Microsoft.CodeAnalysis.VisualBasic.BoundQueryExpression)
							If (boundQueryExpression.LastOperator.Syntax <> node) Then
								GoTo Label1
							End If
							none = MyBase.GetSymbolInfoForNode(VBSemanticModel.SymbolInfoOptions.DefaultOptions, New BoundNodeSummary(boundQueryExpression.LastOperator, boundQueryExpression.LastOperator, Nothing), Nothing)
							Return none
						End If
					End If
				End If
			Label1:
				none = SymbolInfo.None
			End If
			Return none
		End Function

		Private Function GetSingleLambdaClauseLambdaBinder(ByVal operatorSyntax As QueryClauseSyntax, ByVal operatorKeyWord As SyntaxToken, ByVal position As Integer) As Binder
			Dim item As Binder
			If (operatorKeyWord.SpanStart <= position AndAlso SyntaxFacts.InSpanOrEffectiveTrailingOfNode(operatorSyntax, position)) Then
				Dim queryClauseLambdaBinders As ImmutableArray(Of Binder) = Me.GetQueryClauseLambdaBinders(operatorSyntax)
				If (queryClauseLambdaBinders.IsDefaultOrEmpty) Then
					item = Nothing
					Return item
				End If
				item = queryClauseLambdaBinders(0)
				Return item
			End If
			item = Nothing
			Return item
		End Function

		Public Overrides Function GetSyntaxDiagnostics(Optional ByVal span As Nullable(Of TextSpan) = Nothing, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of Diagnostic)
			Throw New NotSupportedException()
		End Function

		Friend Function GetUpperBoundNode(ByVal node As SyntaxNode) As BoundNode
			Dim item As BoundNode
			Dim boundNodes As ImmutableArray(Of BoundNode) = Me.GetBoundNodes(node)
			If (boundNodes.Length <> 0) Then
				item = boundNodes(0)
			Else
				item = Nothing
			End If
			Return item
		End Function

		Private Function GuardedGetBoundNodesFromMap(ByVal node As SyntaxNode) As ImmutableArray(Of BoundNode)
			Dim boundNodes As ImmutableArray(Of BoundNode) = New ImmutableArray(Of BoundNode)()
			If (Me._guardedBoundNodeMap.TryGetValue(node, boundNodes)) Then
				Return boundNodes
			End If
			Return New ImmutableArray(Of BoundNode)()
		End Function

		Private Sub GuardedIncrementalBind(ByVal bindingRoot As SyntaxNode, ByVal enclosingBinder As Binder)
			If (Not Me._guardedBoundNodeMap.ContainsKey(bindingRoot)) Then
				Dim incrementalBinder As MemberSemanticModel.IncrementalBinder = New MemberSemanticModel.IncrementalBinder(Me, enclosingBinder)
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Me.Bind(incrementalBinder, bindingRoot, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded)
				If (boundNode IsNot Nothing) Then
					MemberSemanticModel.SemanticModelMapsBuilder.GuardedCacheBoundNodes(boundNode, Me, Me._guardedBoundNodeMap, Nothing)
					If (Not Me._guardedBoundNodeMap.ContainsKey(bindingRoot)) Then
						Me._guardedBoundNodeMap.Add(bindingRoot, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundNode)(boundNode))
					End If
				End If
			End If
		End Sub

		Private Function InAnonymousTypeInitializerInterior(ByVal node As SyntaxNode, ByVal position As Integer, <Out> ByRef binder As Microsoft.CodeAnalysis.VisualBasic.Binder) As Boolean
			Dim flag As Boolean
			binder = Nothing
			If ((node.Kind() = SyntaxKind.InferredFieldInitializer OrElse node.Kind() = SyntaxKind.NamedFieldInitializer) AndAlso node.Parent IsNot Nothing AndAlso node.Parent.Kind() = SyntaxKind.ObjectMemberInitializer AndAlso node.Parent.Parent IsNot Nothing AndAlso node.Parent.Parent.Kind() = SyntaxKind.AnonymousObjectCreationExpression) Then
				Dim fieldInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldInitializerSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldInitializerSyntax)
				If (SyntaxFacts.InSpanOrEffectiveTrailingOfNode(fieldInitializerSyntax, position)) Then
					Dim anonymousTypeFieldInitializerBinder As Microsoft.CodeAnalysis.VisualBasic.Binder.AnonymousTypeFieldInitializerBinder = Nothing
					Me._rwLock.EnterReadLock()
					Try
						If (Me._guardedAnonymousTypeBinderMap.TryGetValue(fieldInitializerSyntax, anonymousTypeFieldInitializerBinder)) Then
							binder = anonymousTypeFieldInitializerBinder
							flag = binder IsNot Nothing
							Return flag
						End If
					Finally
						Me._rwLock.ExitReadLock()
					End Try
					Dim upperBoundNode As BoundNode = Me.GetUpperBoundNode(fieldInitializerSyntax.Parent.Parent)
					Me._rwLock.EnterReadLock()
					Try
						If (Me._guardedAnonymousTypeBinderMap.TryGetValue(fieldInitializerSyntax, anonymousTypeFieldInitializerBinder)) Then
							binder = anonymousTypeFieldInitializerBinder
							flag = binder IsNot Nothing
							Return flag
						ElseIf (upperBoundNode IsNot Nothing AndAlso upperBoundNode.Kind = BoundKind.NoOpStatement) Then
							Dim hasErrors As Boolean = upperBoundNode.HasErrors
						End If
					Finally
						Me._rwLock.ExitReadLock()
					End Try
				End If
			End If
			flag = False
			Return flag
		End Function

		Private Function InQueryInterior(ByVal node As SyntaxNode, ByVal position As Integer, <Out> ByRef binder As Microsoft.CodeAnalysis.VisualBasic.Binder) As Boolean
			binder = Nothing
			Select Case node.Kind()
				Case SyntaxKind.FunctionAggregation
					binder = Me.GetFunctionAggregationLambdaBinder(DirectCast(node, FunctionAggregationSyntax), position)
					Return binder IsNot Nothing
				Case SyntaxKind.GroupAggregation
				Case SyntaxKind.DistinctClause
				Case SyntaxKind.SkipClause
				Case SyntaxKind.TakeClause
				Case SyntaxKind.JoinCondition
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.InterfaceBlock Or SyntaxKind.ClassBlock Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.AddHandlerAccessorBlock Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.HandlesClause Or SyntaxKind.WithEventsPropertyEventContainer Or SyntaxKind.HandlesClauseItem Or SyntaxKind.MidExpression Or SyntaxKind.CallStatement Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.TrueLiteralExpression Or SyntaxKind.DateLiteralExpression Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.XmlElementAccessExpression Or SyntaxKind.DirectCastExpression Or SyntaxKind.TryCastExpression Or SyntaxKind.SubtractExpression Or SyntaxKind.MultiplyExpression Or SyntaxKind.NotEqualsExpression Or SyntaxKind.LessThanExpression Or SyntaxKind.GreaterThanExpression Or SyntaxKind.IsExpression Or SyntaxKind.AddressOfExpression Or SyntaxKind.BinaryConditionalExpression Or SyntaxKind.QueryExpression Or SyntaxKind.CollectionRangeVariable Or SyntaxKind.VariableNameEquals Or SyntaxKind.FunctionAggregation Or SyntaxKind.GroupByClause Or SyntaxKind.JoinCondition Or SyntaxKind.OrderByClause
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.NamespaceBlock Or SyntaxKind.ModuleBlock Or SyntaxKind.InterfaceBlock Or SyntaxKind.EnumBlock Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.FunctionBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.RemoveHandlerAccessorBlock Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.EventStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.KeywordEventContainer Or SyntaxKind.WithEventsPropertyEventContainer Or SyntaxKind.IncompleteMember Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.MidExpression Or SyntaxKind.AddHandlerStatement Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.FalseLiteralExpression Or SyntaxKind.DateLiteralExpression Or SyntaxKind.GetXmlNamespaceExpression Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.XmlDescendantAccessExpression Or SyntaxKind.DirectCastExpression Or SyntaxKind.PredefinedCastExpression Or SyntaxKind.SubtractExpression Or SyntaxKind.DivideExpression Or SyntaxKind.NotEqualsExpression Or SyntaxKind.LessThanOrEqualExpression Or SyntaxKind.GreaterThanExpression Or SyntaxKind.IsNotExpression Or SyntaxKind.AddressOfExpression Or SyntaxKind.TernaryConditionalExpression Or SyntaxKind.SingleLineSubLambdaExpression Or SyntaxKind.QueryExpression Or SyntaxKind.ExpressionRangeVariable Or SyntaxKind.VariableNameEquals Or SyntaxKind.GroupAggregation Or SyntaxKind.GroupByClause Or SyntaxKind.SimpleJoinClause Or SyntaxKind.OrderByClause
				Case SyntaxKind.AscendingOrdering
				Case SyntaxKind.DescendingOrdering
					Return binder IsNot Nothing
				Case SyntaxKind.FromClause
					binder = Me.GetFromClauseLambdaBinder(DirectCast(node, FromClauseSyntax), position)
					Return binder IsNot Nothing
				Case SyntaxKind.LetClause
					binder = Me.GetLetClauseLambdaBinder(DirectCast(node, LetClauseSyntax), position)
					Return binder IsNot Nothing
				Case SyntaxKind.AggregateClause
					binder = Me.GetAggregateClauseLambdaBinder(DirectCast(node, AggregateClauseSyntax), position)
					Return binder IsNot Nothing
				Case SyntaxKind.WhereClause
					Dim whereClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.WhereClauseSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.WhereClauseSyntax)
					binder = Me.GetSingleLambdaClauseLambdaBinder(whereClauseSyntax, whereClauseSyntax.WhereKeyword, position)
					Return binder IsNot Nothing
				Case SyntaxKind.SkipWhileClause
				Case SyntaxKind.TakeWhileClause
					Dim partitionWhileClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionWhileClauseSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionWhileClauseSyntax)
					binder = Me.GetSingleLambdaClauseLambdaBinder(partitionWhileClauseSyntax, partitionWhileClauseSyntax.WhileKeyword, position)
					Return binder IsNot Nothing
				Case SyntaxKind.GroupByClause
					binder = Me.GetGroupByClauseLambdaBinder(DirectCast(node, GroupByClauseSyntax), position)
					Return binder IsNot Nothing
				Case SyntaxKind.SimpleJoinClause
					binder = Me.GetJoinClauseLambdaBinder(DirectCast(node, SimpleJoinClauseSyntax), position)
					Return binder IsNot Nothing
				Case SyntaxKind.GroupJoinClause
					binder = Me.GetGroupJoinClauseLambdaBinder(DirectCast(node, GroupJoinClauseSyntax), position)
					Return binder IsNot Nothing
				Case SyntaxKind.OrderByClause
					Dim orderByClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderByClauseSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderByClauseSyntax)
					binder = Me.GetSingleLambdaClauseLambdaBinder(orderByClauseSyntax, If(orderByClauseSyntax.ByKeyword.IsMissing, orderByClauseSyntax.OrderKeyword, orderByClauseSyntax.ByKeyword), position)
					Return binder IsNot Nothing
				Case SyntaxKind.SelectClause
					Dim selectClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectClauseSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectClauseSyntax)
					binder = Me.GetSingleLambdaClauseLambdaBinder(selectClauseSyntax, selectClauseSyntax.SelectKeyword, position)
					Return binder IsNot Nothing
				Case Else
					Return binder IsNot Nothing
			End Select
		End Function

		Private Shared Function InWithStatementExpressionInterior(ByVal node As SyntaxNode) As Boolean
			Dim flag As Boolean
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			If (expressionSyntax IsNot Nothing) Then
				Dim parent As VisualBasicSyntaxNode = expressionSyntax.Parent
				If (parent Is Nothing OrElse parent.Kind() <> SyntaxKind.WithStatement) Then
					flag = False
					Return flag
				End If
				parent = parent.Parent
				flag = If(parent Is Nothing OrElse parent.Kind() <> SyntaxKind.WithBlock, False, parent.Parent IsNot Nothing)
				Return flag
			End If
			flag = False
			Return flag
		End Function

		Private Shared Function IsNonExpressionCollectionInitializer(ByVal syntax As SyntaxNode) As Boolean
			Dim flag As Boolean
			Dim parent As SyntaxNode = syntax.Parent
			If (syntax.Kind() = SyntaxKind.CollectionInitializer AndAlso parent IsNot Nothing) Then
				If (parent.Kind() <> SyntaxKind.ObjectCollectionInitializer) Then
					If (parent.Kind() <> SyntaxKind.CollectionInitializer) Then
						flag = False
						Return flag
					End If
					parent = parent.Parent
					flag = If(parent Is Nothing, False, parent.Kind() = SyntaxKind.ObjectCollectionInitializer)
					Return flag
				Else
					flag = True
					Return flag
				End If
			End If
			flag = False
			Return flag
		End Function

		Private Shared Function IsStandaloneStatement(ByVal node As StatementSyntax) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = node.Kind()
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EraseStatement) Then
				Select Case syntaxKind
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement
						flag = True
						Return flag
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement
					Case 4
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWhileStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndAddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndTryStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndAddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRemoveHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSyncLockStatement
					Case 32
					Case 33
					Case 34
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement
					Case 36
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit
					Case 40
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSubStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndPropertyStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndAddHandlerStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRemoveHandlerStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWhileStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndTryStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSyncLockStatement
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement
						flag = False
						Return flag
					Case Else
						If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
							flag = False
							Return flag
						End If
						Select Case syntaxKind
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubNewStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IncompleteMember
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FieldDeclaration
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IfStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseIfStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CatchStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FinallyStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseElseStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForEachStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NextStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithStatement
								flag = False
								Return flag
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExpressionStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrintStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LocalDeclarationStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LabelStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GoToStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StopStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitDoStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitForStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitSubStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitFunctionStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitOperatorStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitPropertyStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitTryStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitSelectStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitWhileStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueWhileStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueDoStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ContinueForStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReturnStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineIfBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ErrorStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnErrorGoToZeroStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnErrorGoToMinusOneStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnErrorGoToLabelStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnErrorResumeNextStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ResumeStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ResumeLabelStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ResumeNextStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForEachBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ThrowStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleAssignmentStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidAssignmentStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddAssignmentStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubtractAssignmentStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiplyAssignmentStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DivideAssignmentStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntegerDivideAssignmentStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExponentiateAssignmentStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LeftShiftAssignmentStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RightShiftAssignmentStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConcatenateAssignmentStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CallStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReDimStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReDimPreserveStatement
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EraseStatement
								flag = True
								Return flag
						End Select

				End Select
			ElseIf (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.YieldStatement) Then
				If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleDoLoopBlock) <= 4) Then
					flag = True
					Return flag
				End If
				If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleDoStatement) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement)) Then
					flag = False
					Return flag
				End If
			Else
				If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubLambdaHeader) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
					flag = False
					Return flag
				End If
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.YieldStatement) Then
					flag = True
					Return flag
				End If
			End If
			Throw ExceptionUtilities.UnexpectedValue(node.Kind())
		End Function

		Friend NotOverridable Overrides Function TryGetSpeculativeSemanticModelCore(ByVal parentModel As SyntaxTreeSemanticModel, ByVal position As Integer, ByVal type As TypeSyntax, ByVal bindingOption As SpeculativeBindingOption, <Out> ByRef speculativeModel As SemanticModel) As Boolean
			Dim flag As Boolean
			Dim speculativeBinderForExpression As Binder = MyBase.GetSpeculativeBinderForExpression(position, type, bindingOption)
			If (speculativeBinderForExpression IsNot Nothing) Then
				speculativeModel = New SpeculativeMemberSemanticModel(parentModel, type, speculativeBinderForExpression, position)
				flag = True
			Else
				speculativeModel = Nothing
				flag = False
			End If
			Return flag
		End Function

		Friend NotOverridable Overrides Function TryGetSpeculativeSemanticModelCore(ByVal parentModel As SyntaxTreeSemanticModel, ByVal position As Integer, ByVal rangeArgument As RangeArgumentSyntax, <Out> ByRef speculativeModel As SemanticModel) As Boolean
			Dim flag As Boolean
			Dim enclosingBinder As Binder = Me.GetEnclosingBinder(position)
			If (enclosingBinder IsNot Nothing) Then
				enclosingBinder = SpeculativeBinder.Create(enclosingBinder)
				speculativeModel = New SpeculativeMemberSemanticModel(parentModel, rangeArgument, enclosingBinder, position)
				flag = True
			Else
				speculativeModel = Nothing
				flag = False
			End If
			Return flag
		End Function

		Private Shared Function UnwrapRaiseEvent(ByVal node As BoundNode) As BoundNode
			Dim eventInvocation As BoundNode
			Dim boundRaiseEventStatement As Microsoft.CodeAnalysis.VisualBasic.BoundRaiseEventStatement = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.BoundRaiseEventStatement)
			If (boundRaiseEventStatement Is Nothing) Then
				eventInvocation = node
			Else
				eventInvocation = boundRaiseEventStatement.EventInvocation
			End If
			Return eventInvocation
		End Function

		Private Class CompilerGeneratedNodeFinder
			Inherits BoundTreeWalkerWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator
			Private ReadOnly _targetSyntax As VisualBasicSyntaxNode

			Private ReadOnly _targetBoundKind As BoundKind

			Private _found As BoundNode

			Private Sub New(ByVal targetSyntax As VisualBasicSyntaxNode, ByVal targetBoundKind As BoundKind)
				MyBase.New()
				Me._targetSyntax = targetSyntax
				Me._targetBoundKind = targetBoundKind
			End Sub

			Protected Overrides Function ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException() As Boolean
				Return False
			End Function

			Public Shared Function FindIn(ByVal context As BoundNode, ByVal targetSyntax As VisualBasicSyntaxNode, ByVal targetBoundKind As BoundKind) As BoundNode
				Dim compilerGeneratedNodeFinder As MemberSemanticModel.CompilerGeneratedNodeFinder = New MemberSemanticModel.CompilerGeneratedNodeFinder(targetSyntax, targetBoundKind)
				compilerGeneratedNodeFinder.Visit(context)
				Return compilerGeneratedNodeFinder._found
			End Function

			Public Overrides Function Visit(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundNode) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				If (node Is Nothing OrElse Me._found IsNot Nothing) Then
					boundNode = Nothing
				ElseIf (Not node.WasCompilerGenerated OrElse node.Syntax <> Me._targetSyntax OrElse node.Kind <> Me._targetBoundKind) Then
					boundNode = MyBase.Visit(node)
				Else
					Me._found = node
					boundNode = Nothing
				End If
				Return boundNode
			End Function
		End Class

		Friend Class IncrementalBinder
			Inherits Binder
			Private ReadOnly _binding As MemberSemanticModel

			Friend Sub New(ByVal binding As MemberSemanticModel, ByVal [next] As Binder)
				MyBase.New([next])
				Me._binding = binding
			End Sub

			Public Overrides Function BindStatement(ByVal node As StatementSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
				Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
				Dim boundNodes As ImmutableArray(Of BoundNode) = Me._binding.GuardedGetBoundNodesFromMap(node)
				boundStatement = If(Not boundNodes.IsDefault, DirectCast(boundNodes.First(), Microsoft.CodeAnalysis.VisualBasic.BoundStatement), MyBase.BindStatement(node, diagnostics))
				Return boundStatement
			End Function

			Public Overrides Function GetBinder(ByVal node As SyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Binder
				Dim incrementalBinder As Microsoft.CodeAnalysis.VisualBasic.Binder
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = MyBase.ContainingBinder.GetBinder(node)
				If (binder Is Nothing) Then
					incrementalBinder = Nothing
				Else
					incrementalBinder = New MemberSemanticModel.IncrementalBinder(Me._binding, binder)
				End If
				Return incrementalBinder
			End Function

			Public Overrides Function GetBinder(ByVal list As SyntaxList(Of StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Binder
				Dim incrementalBinder As Microsoft.CodeAnalysis.VisualBasic.Binder
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = MyBase.ContainingBinder.GetBinder(list)
				If (binder Is Nothing) Then
					incrementalBinder = Nothing
				Else
					incrementalBinder = New MemberSemanticModel.IncrementalBinder(Me._binding, binder)
				End If
				Return incrementalBinder
			End Function
		End Class

		Private Class SemanticModelMapsBuilder
			Inherits BoundTreeWalkerWithStackGuard
			Private ReadOnly _semanticModel As MemberSemanticModel

			Private ReadOnly _thisSyntaxNodeOnly As SyntaxNode

			Private _placeholderReplacementMap As Dictionary(Of BoundValuePlaceholderBase, BoundExpression)

			Private ReadOnly _nodeCache As OrderPreservingMultiDictionary(Of SyntaxNode, BoundNode)

			Private Sub New(ByVal semanticModel As MemberSemanticModel, ByVal thisSyntaxNodeOnly As SyntaxNode, ByVal nodeCache As OrderPreservingMultiDictionary(Of SyntaxNode, BoundNode))
				MyBase.New()
				Me._semanticModel = semanticModel
				Me._thisSyntaxNodeOnly = thisSyntaxNodeOnly
				Me._nodeCache = nodeCache
			End Sub

			Protected Overrides Function ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException() As Boolean
				Return False
			End Function

			Public Shared Sub GuardedCacheBoundNodes(ByVal root As Microsoft.CodeAnalysis.VisualBasic.BoundNode, ByVal semanticModel As MemberSemanticModel, ByVal nodeCache As SmallDictionary(Of SyntaxNode, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundNode)), Optional ByVal thisSyntaxNodeOnly As SyntaxNode = Nothing)
				Dim enumerator As Dictionary(Of SyntaxNode, OrderPreservingMultiDictionary(Of SyntaxNode, Microsoft.CodeAnalysis.VisualBasic.BoundNode).ValueSet).KeyCollection.Enumerator = New Dictionary(Of SyntaxNode, OrderPreservingMultiDictionary(Of SyntaxNode, Microsoft.CodeAnalysis.VisualBasic.BoundNode).ValueSet).KeyCollection.Enumerator()
				Dim instance As OrderPreservingMultiDictionary(Of SyntaxNode, Microsoft.CodeAnalysis.VisualBasic.BoundNode) = OrderPreservingMultiDictionary(Of SyntaxNode, Microsoft.CodeAnalysis.VisualBasic.BoundNode).GetInstance()
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = (New MemberSemanticModel.SemanticModelMapsBuilder(semanticModel, thisSyntaxNodeOnly, instance)).Visit(root)
				Try
					enumerator = instance.Keys.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As SyntaxNode = enumerator.Current
						If (nodeCache.ContainsKey(current)) Then
							Continue While
						End If
						nodeCache(current) = instance(current)
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
				instance.Free()
			End Sub

			Public Function RecordNode(ByVal node As BoundNode, Optional ByVal allowCompilerGenerated As Boolean = False) As Boolean
				Dim flag As Boolean
				If (allowCompilerGenerated OrElse Not node.WasCompilerGenerated) Then
					Dim kind As BoundKind = node.Kind
					If (kind <> BoundKind.Conversion) Then
						If (kind <> BoundKind.UnboundLambda) Then
							GoTo Label1
						End If
						flag = False
						Return flag
					ElseIf (Not allowCompilerGenerated) Then
						Dim boundConversion As Microsoft.CodeAnalysis.VisualBasic.BoundConversion = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.BoundConversion)
						If (boundConversion.ExplicitCastInCode OrElse Not boundConversion.Operand.WasCompilerGenerated OrElse CByte(boundConversion.Operand.Kind) - CByte(BoundKind.WithLValueExpressionPlaceholder) > CByte(BoundKind.WithLValueExpressionPlaceholder)) Then
							GoTo Label1
						End If
						flag = False
						Return flag
					End If
				Label1:
					flag = If(Me._thisSyntaxNodeOnly Is Nothing OrElse node.Syntax = Me._thisSyntaxNodeOnly, True, False)
				Else
					flag = False
				End If
				Return flag
			End Function

			Public Overrides Function Visit(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundNode) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				If (node IsNot Nothing) Then
					If (Me.RecordNode(node, False)) Then
						Me._nodeCache.Add(node.Syntax, node)
					End If
					boundNode = MyBase.Visit(node)
				Else
					boundNode = Nothing
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitAggregateClause(ByVal node As BoundAggregateClause) As BoundNode
				If (Me.RecordNode(node, False)) Then
					Me._semanticModel._guardedQueryBindersMap(node.Syntax) = node.Binders
				End If
				Return MyBase.VisitAggregateClause(node)
			End Function

			Public Overrides Function VisitAnonymousTypeFieldInitializer(ByVal node As BoundAnonymousTypeFieldInitializer) As BoundNode
				If (Me.RecordNode(node, True)) Then
					Dim syntax As FieldInitializerSyntax = TryCast(node.Syntax, FieldInitializerSyntax)
					If (syntax IsNot Nothing) Then
						Me._semanticModel._guardedAnonymousTypeBinderMap(syntax) = node.Binder
					End If
				End If
				Return MyBase.VisitAnonymousTypeFieldInitializer(node)
			End Function

			Public Overrides Function VisitAssignmentOperator(ByVal node As BoundAssignmentOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				If (node.LeftOnTheRightOpt IsNot Nothing) Then
					If (Me._placeholderReplacementMap Is Nothing) Then
						Me._placeholderReplacementMap = New Dictionary(Of BoundValuePlaceholderBase, BoundExpression)()
					End If
					Me._placeholderReplacementMap.Add(node.LeftOnTheRightOpt, node.Left)
					Me.Visit(node.Right)
					Me._placeholderReplacementMap.Remove(node.LeftOnTheRightOpt)
					boundNode = Nothing
				Else
					boundNode = MyBase.VisitAssignmentOperator(node)
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitAttribute(ByVal node As BoundAttribute) As BoundNode
				Me.VisitList(Of BoundExpression)(node.ConstructorArguments)
				Dim enumerator As ImmutableArray(Of BoundExpression).Enumerator = node.NamedArguments.GetEnumerator()
				While enumerator.MoveNext()
					Me.Visit(enumerator.Current)
				End While
				Return Nothing
			End Function

			Public Overrides Function VisitBinaryOperator(ByVal node As BoundBinaryOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				If (node.Left.Kind = BoundKind.BinaryOperator) Then
					Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).GetInstance()
					instance.Push(node.Right)
					Dim left As BoundBinaryOperator = DirectCast(node.Left, BoundBinaryOperator)
					If (Me.RecordNode(left, False)) Then
						Me._nodeCache.Add(left.Syntax, left)
					End If
					instance.Push(left.Right)
					Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = left.Left
					While boundExpression.Kind = BoundKind.BinaryOperator
						left = DirectCast(boundExpression, BoundBinaryOperator)
						If (Me.RecordNode(left, False)) Then
							Me._nodeCache.Add(left.Syntax, left)
						End If
						instance.Push(left.Right)
						boundExpression = left.Left
					End While
					Me.Visit(boundExpression)
					While instance.Count > 0
						Me.Visit(instance.Pop())
					End While
					instance.Free()
					boundNode = Nothing
				Else
					boundNode = MyBase.VisitBinaryOperator(node)
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitByRefArgumentPlaceholder(ByVal node As BoundByRefArgumentPlaceholder) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				boundNode = If(Me._placeholderReplacementMap Is Nothing OrElse Not Me._placeholderReplacementMap.TryGetValue(node, boundExpression), MyBase.VisitByRefArgumentPlaceholder(node), Me.Visit(boundExpression))
				Return boundNode
			End Function

			Public Overrides Function VisitByRefArgumentWithCopyBack(ByVal node As BoundByRefArgumentWithCopyBack) As BoundNode
				If (Me._placeholderReplacementMap Is Nothing) Then
					Me._placeholderReplacementMap = New Dictionary(Of BoundValuePlaceholderBase, BoundExpression)()
				End If
				Me._placeholderReplacementMap.Add(node.InPlaceholder, node.OriginalArgument)
				Me.Visit(node.InConversion)
				Me._placeholderReplacementMap.Remove(node.InPlaceholder)
				Return Nothing
			End Function

			Public Overrides Function VisitCall(ByVal node As BoundCall) As BoundNode
				Me.Visit(node.ReceiverOpt)
				Dim methodGroupOpt As BoundMethodGroup = node.MethodGroupOpt
				If (methodGroupOpt IsNot Nothing) Then
					If (methodGroupOpt.Syntax <> node.Syntax) Then
						Me.Visit(methodGroupOpt)
					ElseIf (node.Method.IsShared) Then
						Me.Visit(methodGroupOpt.ReceiverOpt)
					End If
				End If
				Me.VisitList(Of BoundExpression)(node.Arguments)
				Return Nothing
			End Function

			Public Overrides Function VisitCollectionInitializerExpression(ByVal node As BoundCollectionInitializerExpression) As BoundNode
				Return Me.VisitObjectInitializerExpressionBase(node)
			End Function

			Public Overrides Function VisitCompoundAssignmentTargetPlaceholder(ByVal node As BoundCompoundAssignmentTargetPlaceholder) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				boundNode = If(Me._placeholderReplacementMap Is Nothing OrElse Not Me._placeholderReplacementMap.TryGetValue(node, boundExpression), MyBase.VisitCompoundAssignmentTargetPlaceholder(node), Me.Visit(boundExpression))
				Return boundNode
			End Function

			Public Overrides Function VisitConversion(ByVal node As BoundConversion) As BoundNode
				Return Me.Visit(node.Operand)
			End Function

			Public Overrides Function VisitDelegateCreationExpression(ByVal node As BoundDelegateCreationExpression) As BoundNode
				Me.Visit(node.ReceiverOpt)
				Dim methodGroupOpt As BoundMethodGroup = node.MethodGroupOpt
				If (methodGroupOpt IsNot Nothing) Then
					If (methodGroupOpt.Syntax <> node.Syntax) Then
						Me.Visit(methodGroupOpt)
					ElseIf (node.Method.IsShared) Then
						Me.Visit(methodGroupOpt.ReceiverOpt)
					End If
				End If
				Return Nothing
			End Function

			Public Overrides Function VisitDirectCast(ByVal node As BoundDirectCast) As BoundNode
				Return Me.Visit(node.Operand)
			End Function

			Public Overrides Function VisitLateInvocation(ByVal node As BoundLateInvocation) As BoundNode
				MyBase.VisitLateInvocation(node)
				Dim member As BoundLateMemberAccess = TryCast(node.Member, BoundLateMemberAccess)
				If (member IsNot Nothing AndAlso member.ReceiverOpt Is Nothing AndAlso node.MethodOrPropertyGroupOpt IsNot Nothing) Then
					Me.Visit(node.MethodOrPropertyGroupOpt.ReceiverOpt)
				End If
				Return Nothing
			End Function

			Public Overrides Function VisitObjectInitializerExpression(ByVal node As BoundObjectInitializerExpression) As BoundNode
				Return Me.VisitObjectInitializerExpressionBase(node)
			End Function

			Private Function VisitObjectInitializerExpressionBase(ByVal node As BoundObjectInitializerExpressionBase) As BoundNode
				Me.VisitList(Of BoundExpression)(node.Initializers)
				Return Nothing
			End Function

			Public Overrides Function VisitPropertyAccess(ByVal node As BoundPropertyAccess) As BoundNode
				Me.Visit(node.ReceiverOpt)
				Dim propertyGroupOpt As BoundPropertyGroup = node.PropertyGroupOpt
				If (propertyGroupOpt IsNot Nothing) Then
					If (propertyGroupOpt.Syntax <> node.Syntax) Then
						Me.Visit(node.PropertyGroupOpt)
					ElseIf (node.PropertySymbol.IsShared) Then
						Me.Visit(propertyGroupOpt.ReceiverOpt)
					End If
				End If
				Me.VisitList(Of BoundExpression)(node.Arguments)
				Return Nothing
			End Function

			Public Overrides Function VisitQueryClause(ByVal node As BoundQueryClause) As BoundNode
				If (Me.RecordNode(node, False)) Then
					Me._semanticModel._guardedQueryBindersMap(node.Syntax) = node.Binders
				End If
				Return MyBase.VisitQueryClause(node)
			End Function

			Public Overrides Function VisitTryCast(ByVal node As BoundTryCast) As BoundNode
				Return Me.Visit(node.Operand)
			End Function

			Public Overrides Function VisitTypeExpression(ByVal node As BoundTypeExpression) As BoundNode
				Me.Visit(node.UnevaluatedReceiverOpt)
				Return MyBase.VisitTypeExpression(node)
			End Function

			Public Overrides Function VisitUnboundLambda(ByVal node As UnboundLambda) As BoundNode
				Return Me.Visit(node.BindForErrorRecovery())
			End Function
		End Class
	End Class
End Namespace