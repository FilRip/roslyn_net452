Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class SyntaxTreeSemanticModel
		Inherits VBSemanticModel
		Private ReadOnly _compilation As VisualBasicCompilation

		Private ReadOnly _sourceModule As SourceModuleSymbol

		Private ReadOnly _syntaxTree As Microsoft.CodeAnalysis.SyntaxTree

		Private ReadOnly _binderFactory As BinderFactory

		Private ReadOnly _ignoresAccessibility As Boolean

		<TupleElementNames(New String() { "binder", "ignoresAccessibility" })>
		Private ReadOnly _semanticModelCache As ConcurrentDictionary(Of ValueTuple(Of Binder, Boolean), MemberSemanticModel)

		<TupleElementNames(New String() { "binder", "ignoresAccessibility" })>
		Private ReadOnly _methodBodySemanticModelCreator As Func(Of ValueTuple(Of Binder, Boolean), MemberSemanticModel)

		<TupleElementNames(New String() { "binder", "ignoresAccessibility" })>
		Private ReadOnly _initializerSemanticModelCreator As Func(Of ValueTuple(Of Binder, Boolean), MemberSemanticModel)

		<TupleElementNames(New String() { "binder", "ignoresAccessibility" })>
		Private ReadOnly _attributeSemanticModelCreator As Func(Of ValueTuple(Of Binder, Boolean), MemberSemanticModel)

		Public Overrides ReadOnly Property Compilation As VisualBasicCompilation
			Get
				Return Me._compilation
			End Get
		End Property

		Friend Overrides ReadOnly Property ContainingModelOrSelf As SemanticModel
			Get
				Return Me
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IgnoresAccessibility As Boolean
			Get
				Return Me._ignoresAccessibility
			End Get
		End Property

		Public Overrides ReadOnly Property IsSpeculativeSemanticModel As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property OriginalPositionForSpeculation As Integer
			Get
				Return 0
			End Get
		End Property

		Public Overrides ReadOnly Property ParentModel As SemanticModel
			Get
				Return Nothing
			End Get
		End Property

		Friend Overrides ReadOnly Property Root As SyntaxNode
			Get
				Return DirectCast(Me._syntaxTree.GetRoot(New CancellationToken()), VisualBasicSyntaxNode)
			End Get
		End Property

		Public Overrides ReadOnly Property SyntaxTree As Microsoft.CodeAnalysis.SyntaxTree
			Get
				Return Me._syntaxTree
			End Get
		End Property

		Friend Sub New(ByVal compilation As VisualBasicCompilation, ByVal sourceModule As SourceModuleSymbol, ByVal syntaxTree As Microsoft.CodeAnalysis.SyntaxTree, Optional ByVal ignoreAccessibility As Boolean = False)
			MyBase.New()
			Me._semanticModelCache = New ConcurrentDictionary(Of ValueTuple(Of Binder, Boolean), MemberSemanticModel)()
			Me._methodBodySemanticModelCreator = Function(key As ValueTuple(Of Binder, Boolean)) MethodBodySemanticModel.Create(Me, DirectCast(key.Item1, SubOrFunctionBodyBinder), key.Item2)
			Me._initializerSemanticModelCreator = Function(key As ValueTuple(Of Binder, Boolean)) InitializerSemanticModel.Create(Me, DirectCast(key.Item1, DeclarationInitializerBinder), key.Item2)
			Me._attributeSemanticModelCreator = Function(key As ValueTuple(Of Binder, Boolean)) AttributeSemanticModel.Create(Me, DirectCast(key.Item1, AttributeBinder), key.Item2)
			Me._compilation = compilation
			Me._sourceModule = sourceModule
			Me._syntaxTree = syntaxTree
			Me._ignoresAccessibility = ignoreAccessibility
			Me._binderFactory = New BinderFactory(sourceModule, syntaxTree)
		End Sub

		Public Overrides Function AnalyzeControlFlow(ByVal firstStatement As StatementSyntax, ByVal lastStatement As StatementSyntax) As ControlFlowAnalysis
			Return New VisualBasicControlFlowAnalysis(If(Me.ValidateRegionDefiningStatementsRange(firstStatement, lastStatement), Me.CreateRegionAnalysisContext(firstStatement, lastStatement), Me.CreateFailedRegionAnalysisContext()))
		End Function

		Public Overrides Function AnalyzeDataFlow(ByVal firstStatement As StatementSyntax, ByVal lastStatement As StatementSyntax) As DataFlowAnalysis
			Return New VisualBasicDataFlowAnalysis(If(Me.ValidateRegionDefiningStatementsRange(firstStatement, lastStatement), Me.CreateRegionAnalysisContext(firstStatement, lastStatement), Me.CreateFailedRegionAnalysisContext()))
		End Function

		Public Overrides Function AnalyzeDataFlow(ByVal expression As ExpressionSyntax) As DataFlowAnalysis
			Return New VisualBasicDataFlowAnalysis(If(Me.ValidateRegionDefiningExpression(expression), Me.CreateRegionAnalysisContext(expression), Me.CreateFailedRegionAnalysisContext()))
		End Function

		Private Sub AssertNodeInTree(ByVal node As VisualBasicSyntaxNode, ByVal argName As String)
			If (node Is Nothing) Then
				Throw New ArgumentNullException(argName)
			End If
			If (Not MyBase.IsInTree(node)) Then
				Throw New ArgumentException([String].Concat(argName, VBResources.NotWithinTree))
			End If
		End Sub

		<Conditional("DEBUG")>
		Private Sub CheckSucceededFlagInAnalyzeDataFlow(ByVal expression As ExpressionSyntax, ByVal result As VisualBasicDataFlowAnalysis, ByVal context As RegionAnalysisContext)
			If (Not result.Succeeded AndAlso Not result.InvalidRegionDetectedInternal AndAlso Not context.Failed) Then
				Dim parent As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = expression.Parent
				If (expression.Kind() <> SyntaxKind.IdentifierName OrElse parent Is Nothing OrElse parent.Kind() <> SyntaxKind.SimpleMemberAccessExpression OrElse DirectCast(parent, MemberAccessExpressionSyntax).Expression <> expression) Then
					If (expression.Kind() = SyntaxKind.NumericLiteralExpression AndAlso parent IsNot Nothing AndAlso parent.Kind() = SyntaxKind.SimpleArgument AndAlso Not DirectCast(parent, SimpleArgumentSyntax).IsNamed) Then
						Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = parent.Parent
						If (visualBasicSyntaxNode IsNot Nothing AndAlso visualBasicSyntaxNode.Kind() = SyntaxKind.ArgumentList) Then
							Dim parent1 As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = visualBasicSyntaxNode.Parent
							If (parent1 IsNot Nothing AndAlso parent1.Kind() = SyntaxKind.ModifiedIdentifier) Then
								Dim visualBasicSyntaxNode1 As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = parent1.Parent
								If (visualBasicSyntaxNode1 IsNot Nothing AndAlso visualBasicSyntaxNode1.Kind() = SyntaxKind.VariableDeclarator AndAlso DirectCast(visualBasicSyntaxNode1, VariableDeclaratorSyntax).Initializer IsNot Nothing) Then
									Return
								End If
							End If
						End If
					End If
					Throw ExceptionUtilities.Unreachable
				End If
			End If
		End Sub

		Private Function CheckSymbolLocationsAgainstSyntax(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, ByVal nodeToCheck As VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim enumerator As ImmutableArray(Of Location).Enumerator = symbol.Locations.GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As Location = enumerator.Current
					If (current.SourceTree = Me.SyntaxTree AndAlso nodeToCheck.Span.Contains(current.SourceSpan)) Then
						namedTypeSymbol = symbol
						Exit While
					End If
				Else
					namedTypeSymbol = Nothing
					Exit While
				End If
			End While
			Return namedTypeSymbol
		End Function

		Public Overrides Function ClassifyConversion(ByVal expression As ExpressionSyntax, ByVal destination As ITypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Conversion
			Dim conversion As Microsoft.CodeAnalysis.VisualBasic.Conversion
			MyBase.CheckSyntaxNode(expression)
			If (destination Is Nothing) Then
				Throw New ArgumentNullException("destination")
			End If
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = destination.EnsureVbSymbolOrNothing(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)("destination")
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(expression)
			conversion = If(memberSemanticModel IsNot Nothing, memberSemanticModel.ClassifyConversion(expression, typeSymbol), New Microsoft.CodeAnalysis.VisualBasic.Conversion(New KeyValuePair(Of ConversionKind, MethodSymbol)()))
			Return conversion
		End Function

		Private Function CreateFailedRegionAnalysisContext() As RegionAnalysisContext
			Return New RegionAnalysisContext(Me.Compilation)
		End Function

		Private Function CreateRegionAnalysisContext(ByVal expression As ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.RegionAnalysisContext
			Dim regionAnalysisContext As Microsoft.CodeAnalysis.VisualBasic.RegionAnalysisContext
			Dim span As TextSpan = expression.Span
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(expression)
			If (memberSemanticModel IsNot Nothing) Then
				Dim boundRoot As BoundNode = memberSemanticModel.GetBoundRoot()
				Dim upperBoundNode As BoundNode = memberSemanticModel.GetUpperBoundNode(expression)
				regionAnalysisContext = New Microsoft.CodeAnalysis.VisualBasic.RegionAnalysisContext(Me.Compilation, memberSemanticModel.MemberSymbol, boundRoot, upperBoundNode, upperBoundNode, span)
			Else
				Dim boundBadStatement As Microsoft.CodeAnalysis.VisualBasic.BoundBadStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundBadStatement(expression, ImmutableArray(Of BoundNode).Empty, False)
				regionAnalysisContext = New Microsoft.CodeAnalysis.VisualBasic.RegionAnalysisContext(Me.Compilation, Nothing, boundBadStatement, boundBadStatement, boundBadStatement, span)
			End If
			Return regionAnalysisContext
		End Function

		Private Function CreateRegionAnalysisContext(ByVal firstStatement As StatementSyntax, ByVal lastStatement As StatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.RegionAnalysisContext
			Dim regionAnalysisContext As Microsoft.CodeAnalysis.VisualBasic.RegionAnalysisContext
			Dim textSpan As Microsoft.CodeAnalysis.Text.TextSpan = Microsoft.CodeAnalysis.Text.TextSpan.FromBounds(firstStatement.SpanStart, lastStatement.Span.[End])
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(firstStatement)
			If (memberSemanticModel IsNot Nothing) Then
				Dim boundRoot As Microsoft.CodeAnalysis.VisualBasic.BoundNode = memberSemanticModel.GetBoundRoot()
				Dim upperBoundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = memberSemanticModel.GetUpperBoundNode(firstStatement)
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = memberSemanticModel.GetUpperBoundNode(lastStatement)
				regionAnalysisContext = New Microsoft.CodeAnalysis.VisualBasic.RegionAnalysisContext(Me.Compilation, memberSemanticModel.MemberSymbol, boundRoot, upperBoundNode, boundNode, textSpan)
			Else
				Dim boundBadStatement As Microsoft.CodeAnalysis.VisualBasic.BoundBadStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundBadStatement(firstStatement, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundNode).Empty, False)
				regionAnalysisContext = New Microsoft.CodeAnalysis.VisualBasic.RegionAnalysisContext(Me.Compilation, Nothing, boundBadStatement, boundBadStatement, boundBadStatement, textSpan)
			End If
			Return regionAnalysisContext
		End Function

		Friend Overrides Function GetAggregateClauseSymbolInfoWorker(ByVal node As AggregateClauseSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.VisualBasic.AggregateClauseSymbolInfo
			Dim aggregateClauseSymbolInfo As Microsoft.CodeAnalysis.VisualBasic.AggregateClauseSymbolInfo
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(node)
			aggregateClauseSymbolInfo = If(memberSemanticModel Is Nothing, New Microsoft.CodeAnalysis.VisualBasic.AggregateClauseSymbolInfo(SymbolInfo.None, SymbolInfo.None), memberSemanticModel.GetAggregateClauseSymbolInfoWorker(node, cancellationToken))
			Return aggregateClauseSymbolInfo
		End Function

		Friend Overrides Function GetAttributeMemberGroup(ByVal attribute As AttributeSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of Symbol)
			Dim empty As ImmutableArray(Of Symbol)
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(attribute)
			If (memberSemanticModel Is Nothing) Then
				empty = ImmutableArray(Of Symbol).Empty
			Else
				empty = memberSemanticModel.GetAttributeMemberGroup(attribute, cancellationToken)
			End If
			Return empty
		End Function

		Friend Overrides Function GetAttributeSymbolInfo(ByVal attribute As AttributeSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.SymbolInfo
			Dim symbolInfo As Microsoft.CodeAnalysis.SymbolInfo
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(attribute)
			symbolInfo = If(memberSemanticModel Is Nothing, Microsoft.CodeAnalysis.SymbolInfo.None, memberSemanticModel.GetAttributeSymbolInfo(attribute, cancellationToken))
			Return symbolInfo
		End Function

		Friend Overrides Function GetAttributeTypeInfo(ByVal attribute As AttributeSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.VisualBasic.VisualBasicTypeInfo
			Dim visualBasicTypeInfo As Microsoft.CodeAnalysis.VisualBasic.VisualBasicTypeInfo
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(attribute)
			visualBasicTypeInfo = If(memberSemanticModel Is Nothing, Microsoft.CodeAnalysis.VisualBasic.VisualBasicTypeInfo.None, memberSemanticModel.GetAttributeTypeInfo(attribute, cancellationToken))
			Return visualBasicTypeInfo
		End Function

		Friend Overrides Function GetAwaitExpressionInfoWorker(ByVal awaitExpression As AwaitExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.VisualBasic.AwaitExpressionInfo
			Dim awaitExpressionInfo As Microsoft.CodeAnalysis.VisualBasic.AwaitExpressionInfo
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(awaitExpression)
			awaitExpressionInfo = If(memberSemanticModel Is Nothing, New Microsoft.CodeAnalysis.VisualBasic.AwaitExpressionInfo(), memberSemanticModel.GetAwaitExpressionInfoWorker(awaitExpression, cancellationToken))
			Return awaitExpressionInfo
		End Function

		Friend Overrides Function GetCollectionInitializerAddSymbolInfo(ByVal collectionInitializer As ObjectCreationExpressionSyntax, ByVal node As ExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.SymbolInfo
			Dim symbolInfo As Microsoft.CodeAnalysis.SymbolInfo
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(collectionInitializer)
			symbolInfo = If(memberSemanticModel Is Nothing, Microsoft.CodeAnalysis.SymbolInfo.None, memberSemanticModel.GetCollectionInitializerAddSymbolInfo(collectionInitializer, node, cancellationToken))
			Return symbolInfo
		End Function

		Friend Overrides Function GetCollectionRangeVariableSymbolInfoWorker(ByVal node As CollectionRangeVariableSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.VisualBasic.CollectionRangeVariableSymbolInfo
			Dim collectionRangeVariableSymbolInfo As Microsoft.CodeAnalysis.VisualBasic.CollectionRangeVariableSymbolInfo
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(node)
			collectionRangeVariableSymbolInfo = If(memberSemanticModel Is Nothing, Microsoft.CodeAnalysis.VisualBasic.CollectionRangeVariableSymbolInfo.None, memberSemanticModel.GetCollectionRangeVariableSymbolInfoWorker(node, cancellationToken))
			Return collectionRangeVariableSymbolInfo
		End Function

		Private Function GetCrefOrNameAttributeReferenceSymbols(ByVal node As VisualBasicSyntaxNode, ByVal preserveAlias As Boolean, <Out> ByRef typeParameters As ImmutableArray(Of Symbol)) As ImmutableArray(Of Symbol)
			Dim symbols As ImmutableArray(Of Symbol)
			Dim immutableAndFree As ImmutableArray(Of Symbol)
			Dim flag As Boolean
			Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol)
			typeParameters = ImmutableArray(Of Symbol).Empty
			If (node.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlString) Then
				Dim parent As VisualBasicSyntaxNode = node.Parent
				Dim baseXmlAttributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.BaseXmlAttributeSyntax = Nothing
				While True
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = parent.Kind()
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DocumentationCommentTrivia) Then
						Exit While
					End If
					If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlCrefAttribute) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
						baseXmlAttributeSyntax = DirectCast(parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.BaseXmlAttributeSyntax)
					End If
					parent = parent.Parent
				End While
				If (baseXmlAttributeSyntax IsNot Nothing) Then
					Dim flag1 As Boolean = baseXmlAttributeSyntax.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlCrefAttribute
					Dim parentTrivia As SyntaxTrivia = DirectCast(parent, DocumentationCommentTriviaSyntax).ParentTrivia
					If (parentTrivia.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None) Then
						symbols = New ImmutableArray(Of Symbol)()
					ElseIf (parentTrivia.Token.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None) Then
						Dim binderForPosition As Binder = Me._binderFactory.GetBinderForPosition(node, node.SpanStart)
						binderForPosition = SemanticModelBinder.Mark(binderForPosition, Me.IgnoresAccessibility)
						If (Not flag1) Then
							Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)
							discarded = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
							symbols = binderForPosition.BindXmlNameAttributeValue(identifierNameSyntax, discarded)
						Else
							If (node.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CrefReference) Then
								flag = If(node.Parent Is Nothing, False, node.Parent.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CrefReference)
								Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)
								discarded = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
								immutableAndFree = binderForPosition.BindInsideCrefAttributeValue(typeSyntax, preserveAlias, Nothing, discarded)
							Else
								flag = True
								Dim crefReferenceSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefReferenceSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefReferenceSyntax)
								discarded = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
								immutableAndFree = binderForPosition.BindInsideCrefAttributeValue(crefReferenceSyntax, preserveAlias, Nothing, discarded)
							End If
							If (flag) Then
								Dim instance As ArrayBuilder(Of Symbol) = Nothing
								Dim instance1 As ArrayBuilder(Of Symbol) = Nothing
								Dim length As Integer = immutableAndFree.Length - 1
								Dim num As Integer = 0
								Do
									Dim item As Symbol = immutableAndFree(num)
									If (item.Kind = SymbolKind.TypeParameter) Then
										If (instance Is Nothing) Then
											instance = ArrayBuilder(Of Symbol).GetInstance(num)
											instance1 = ArrayBuilder(Of Symbol).GetInstance()
											instance.AddRange(immutableAndFree, num)
										End If
										instance1.Add(DirectCast(item, TypeParameterSymbol))
									ElseIf (instance IsNot Nothing) Then
										instance.Add(item)
									End If
									num = num + 1
								Loop While num <= length
								If (instance IsNot Nothing) Then
									immutableAndFree = instance.ToImmutableAndFree()
									typeParameters = instance1.ToImmutableAndFree()
								End If
							End If
							symbols = immutableAndFree
						End If
					Else
						symbols = New ImmutableArray(Of Symbol)()
					End If
				Else
					symbols = New ImmutableArray(Of Symbol)()
				End If
			Else
				symbols = New ImmutableArray(Of Symbol)()
			End If
			Return symbols
		End Function

		Friend Overrides Function GetCrefReferenceSymbolInfo(ByVal crefReference As CrefReferenceSyntax, ByVal options As VBSemanticModel.SymbolInfoOptions, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SymbolInfo
			MyBase.ValidateSymbolInfoOptions(options)
			Return Me.GetSymbolInfoForCrefOrNameAttributeReference(crefReference, options)
		End Function

		Public Overrides Function GetDeclarationDiagnostics(Optional ByVal span As Nullable(Of TextSpan) = Nothing, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of Diagnostic)
			Return Me._compilation.GetDiagnosticsForSyntaxTree(CompilationStage.[Declare], Me._syntaxTree, span, False, cancellationToken)
		End Function

		Public Shadows Function GetDeclaredSymbol(ByVal declarationSyntax As DelegateStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			If (declarationSyntax Is Nothing) Then
				Throw New ArgumentNullException("declarationSyntax")
			End If
			If (Not MyBase.IsInTree(declarationSyntax)) Then
				Throw New ArgumentException(VBResources.DeclarationSyntaxNotWithinTree)
			End If
			Dim namedTypeBinder As Binder = Me._binderFactory.GetNamedTypeBinder(declarationSyntax)
			If (namedTypeBinder Is Nothing OrElse Not TypeOf namedTypeBinder Is Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder) Then
				namedTypeSymbol = Nothing
			Else
				namedTypeSymbol = Me.CheckSymbolLocationsAgainstSyntax(namedTypeBinder.ContainingType, declarationSyntax)
			End If
			Return namedTypeSymbol
		End Function

		Public Overrides Function GetDeclaredSymbol(ByVal declarationSyntax As TypeStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As INamedTypeSymbol
			Dim namedTypeSymbol As INamedTypeSymbol
			If (declarationSyntax Is Nothing) Then
				Throw New ArgumentNullException("declarationSyntax")
			End If
			If (Not MyBase.IsInTree(declarationSyntax)) Then
				Throw New ArgumentException(VBResources.DeclarationSyntaxNotWithinTree)
			End If
			Dim namedTypeBinder As Binder = Me._binderFactory.GetNamedTypeBinder(declarationSyntax)
			If (namedTypeBinder Is Nothing OrElse Not TypeOf namedTypeBinder Is Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder) Then
				namedTypeSymbol = Nothing
			Else
				namedTypeSymbol = Me.CheckSymbolLocationsAgainstSyntax(namedTypeBinder.ContainingType, declarationSyntax)
			End If
			Return namedTypeSymbol
		End Function

		Public Overrides Function GetDeclaredSymbol(ByVal declarationSyntax As EnumStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As INamedTypeSymbol
			Dim namedTypeSymbol As INamedTypeSymbol
			If (declarationSyntax Is Nothing) Then
				Throw New ArgumentNullException("declarationSyntax")
			End If
			If (Not MyBase.IsInTree(declarationSyntax)) Then
				Throw New ArgumentException(VBResources.DeclarationSyntaxNotWithinTree)
			End If
			Dim namedTypeBinder As Binder = Me._binderFactory.GetNamedTypeBinder(declarationSyntax)
			If (namedTypeBinder Is Nothing OrElse Not TypeOf namedTypeBinder Is Microsoft.CodeAnalysis.VisualBasic.NamedTypeBinder) Then
				namedTypeSymbol = Nothing
			Else
				namedTypeSymbol = Me.CheckSymbolLocationsAgainstSyntax(namedTypeBinder.ContainingType, declarationSyntax)
			End If
			Return namedTypeSymbol
		End Function

		Public Overrides Function GetDeclaredSymbol(ByVal declarationSyntax As NamespaceStatementSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As INamespaceSymbol
			Dim containingNamespaceOrType As INamespaceSymbol
			If (declarationSyntax Is Nothing) Then
				Throw New ArgumentNullException("declarationSyntax")
			End If
			If (Not MyBase.IsInTree(declarationSyntax)) Then
				Throw New ArgumentException(VBResources.DeclarationSyntaxNotWithinTree)
			End If
			Dim parent As NamespaceBlockSyntax = TryCast(declarationSyntax.Parent, NamespaceBlockSyntax)
			If (parent IsNot Nothing) Then
				Dim namespaceBinder As Binder = Me._binderFactory.GetNamespaceBinder(parent)
				If (namespaceBinder Is Nothing OrElse Not TypeOf namespaceBinder Is Microsoft.CodeAnalysis.VisualBasic.NamespaceBinder) Then
					containingNamespaceOrType = Nothing
					Return containingNamespaceOrType
				End If
				containingNamespaceOrType = DirectCast(namespaceBinder.ContainingNamespaceOrType, NamespaceSymbol)
				Return containingNamespaceOrType
			End If
			containingNamespaceOrType = Nothing
			Return containingNamespaceOrType
		End Function

		Friend Overrides Function GetDeclaredSymbol(ByVal declarationSyntax As MethodBaseSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ISymbol
			Dim declaredSymbol As ISymbol
			If (declarationSyntax Is Nothing) Then
				Throw New ArgumentNullException("declarationSyntax")
			End If
			If (Not MyBase.IsInTree(declarationSyntax)) Then
				Throw New ArgumentException(VBResources.DeclarationSyntaxNotWithinTree)
			End If
			If (declarationSyntax.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement OrElse declarationSyntax.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement) Then
				declaredSymbol = Me.GetDeclaredSymbol(DirectCast(declarationSyntax, DelegateStatementSyntax), cancellationToken)
			Else
				Dim parent As StatementSyntax = TryCast(declarationSyntax.Parent, StatementSyntax)
				If (parent IsNot Nothing) Then
					Dim typeBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax = Nothing
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = parent.Kind()
					If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock) <= 4) Then
						typeBlockSyntax = TryCast(parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax)
					Else
						Select Case syntaxKind
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock
								typeBlockSyntax = TryCast(parent.Parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax)
								If (typeBlockSyntax IsNot Nothing OrElse parent.Parent Is Nothing) Then
									Exit Select
								End If
								Dim rootNamespace As INamespaceSymbol = Nothing
								Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = parent.Parent.Kind()
								If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit) Then
									rootNamespace = Me._sourceModule.RootNamespace
								ElseIf (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock) Then
									rootNamespace = MyBase.GetDeclaredSymbol(DirectCast(parent.Parent, NamespaceBlockSyntax), cancellationToken)
								End If
								If (rootNamespace Is Nothing) Then
									Exit Select
								End If
								Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(rootNamespace.GetMembers("<invalid-global-code>").SingleOrDefault(), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
								If (namedTypeSymbol Is Nothing) Then
									Exit Select
								End If
								declaredSymbol = SourceMethodSymbol.FindSymbolFromSyntax(declarationSyntax, Me._syntaxTree, namedTypeSymbol)
								Return declaredSymbol
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorBlock
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorBlock
								If (parent.Parent Is Nothing) Then
									Exit Select
								End If
								typeBlockSyntax = TryCast(parent.Parent.Parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax)
								Exit Select
							Case Else
								declaredSymbol = Nothing
								Return declaredSymbol
						End Select
					End If
					If (typeBlockSyntax IsNot Nothing) Then
						Dim declaredSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(Me.GetDeclaredSymbol(typeBlockSyntax.BlockStatement, cancellationToken), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
						If (declaredSymbol1 Is Nothing) Then
							GoTo Label1
						End If
						declaredSymbol = SourceMethodSymbol.FindSymbolFromSyntax(declarationSyntax, Me._syntaxTree, declaredSymbol1)
						Return declaredSymbol
					End If
				End If
			Label1:
				declaredSymbol = Nothing
			End If
			Return declaredSymbol
		End Function

		Public Overrides Function GetDeclaredSymbol(ByVal parameter As ParameterSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IParameterSymbol
			Dim parameterSymbol As IParameterSymbol
			Dim associatedSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol
			If (parameter Is Nothing) Then
				Throw New ArgumentNullException("parameter")
			End If
			Dim parent As ParameterListSyntax = TryCast(parameter.Parent, ParameterListSyntax)
			If (parent IsNot Nothing) Then
				Dim methodBaseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax = TryCast(parent.Parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax)
				If (methodBaseSyntax IsNot Nothing) Then
					Dim declaredSymbol As ISymbol = Me.GetDeclaredSymbol(methodBaseSyntax, cancellationToken)
					If (declaredSymbol IsNot Nothing) Then
						Dim kind As SymbolKind = declaredSymbol.Kind
						If (kind <= SymbolKind.Method) Then
							If (kind = SymbolKind.[Event]) Then
								Dim eventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol = DirectCast(declaredSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol)
								Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = TryCast(eventSymbol.Type, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
								If (type IsNot Nothing) Then
									associatedSymbol = type.AssociatedSymbol
								Else
									associatedSymbol = Nothing
								End If
								If (CObj(associatedSymbol) <> CObj(eventSymbol)) Then
									parameterSymbol = Nothing
									Return parameterSymbol
								Else
									parameterSymbol = type.DelegateInvokeMethod.Parameters.GetParameterSymbol(parameter)
									Return parameterSymbol
								End If
							Else
								If (kind <> SymbolKind.Method) Then
									parameterSymbol = Nothing
									Return parameterSymbol
								End If
								parameterSymbol = DirectCast(declaredSymbol, MethodSymbol).Parameters.GetParameterSymbol(parameter)
								Return parameterSymbol
							End If
						ElseIf (kind = SymbolKind.NamedType) Then
							Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(declaredSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
							If (namedTypeSymbol.DelegateInvokeMethod Is Nothing) Then
								parameterSymbol = Nothing
								Return parameterSymbol
							End If
							parameterSymbol = namedTypeSymbol.DelegateInvokeMethod.Parameters.GetParameterSymbol(parameter)
							Return parameterSymbol
						Else
							If (kind <> SymbolKind.[Property]) Then
								parameterSymbol = Nothing
								Return parameterSymbol
							End If
							parameterSymbol = DirectCast(declaredSymbol, PropertySymbol).Parameters.GetParameterSymbol(parameter)
							Return parameterSymbol
						End If
					ElseIf (TypeOf methodBaseSyntax Is LambdaHeaderSyntax) Then
						Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(methodBaseSyntax)
						If (memberSemanticModel Is Nothing) Then
							parameterSymbol = Nothing
							Return parameterSymbol
						End If
						parameterSymbol = memberSemanticModel.GetDeclaredSymbol(parameter, cancellationToken)
						Return parameterSymbol
					End If
				End If
			End If
			parameterSymbol = Nothing
			Return parameterSymbol
		End Function

		Public Overrides Function GetDeclaredSymbol(ByVal typeParameter As TypeParameterSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ITypeParameterSymbol
			Dim typeParameterSymbol As ITypeParameterSymbol
			If (typeParameter Is Nothing) Then
				Throw New ArgumentNullException("typeParameter")
			End If
			If (Not MyBase.IsInTree(typeParameter)) Then
				Throw New ArgumentException(VBResources.TypeParameterNotWithinTree)
			End If
			Dim declaredSymbol As ISymbol = Nothing
			Dim parent As TypeParameterListSyntax = TryCast(typeParameter.Parent, TypeParameterListSyntax)
			If (parent IsNot Nothing AndAlso parent.Parent IsNot Nothing) Then
				If (TypeOf parent.Parent Is MethodStatementSyntax) Then
					declaredSymbol = MyBase.GetDeclaredSymbol(DirectCast(parent.Parent, MethodStatementSyntax), cancellationToken)
				ElseIf (TypeOf parent.Parent Is TypeStatementSyntax) Then
					declaredSymbol = Me.GetDeclaredSymbol(DirectCast(parent.Parent, TypeStatementSyntax), cancellationToken)
				ElseIf (TypeOf parent.Parent Is DelegateStatementSyntax) Then
					declaredSymbol = Me.GetDeclaredSymbol(DirectCast(parent.Parent, DelegateStatementSyntax), cancellationToken)
				End If
				If (declaredSymbol IsNot Nothing) Then
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = TryCast(declaredSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
					If (namedTypeSymbol Is Nothing) Then
						Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = TryCast(declaredSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						If (methodSymbol Is Nothing) Then
							typeParameterSymbol = Nothing
							Return typeParameterSymbol
						End If
						typeParameterSymbol = Me.GetTypeParameterSymbol(methodSymbol.TypeParameters, typeParameter)
						Return typeParameterSymbol
					Else
						typeParameterSymbol = Me.GetTypeParameterSymbol(namedTypeSymbol.TypeParameters, typeParameter)
						Return typeParameterSymbol
					End If
				End If
			End If
			typeParameterSymbol = Nothing
			Return typeParameterSymbol
		End Function

		Public Overrides Function GetDeclaredSymbol(ByVal declarationSyntax As EnumMemberDeclarationSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IFieldSymbol
			Dim fieldSymbol As IFieldSymbol
			If (declarationSyntax Is Nothing) Then
				Throw New ArgumentNullException("declarationSyntax")
			End If
			If (Not MyBase.IsInTree(declarationSyntax)) Then
				Throw New ArgumentException(VBResources.DeclarationSyntaxNotWithinTree)
			End If
			Dim parent As EnumBlockSyntax = DirectCast(declarationSyntax.Parent, EnumBlockSyntax)
			If (parent IsNot Nothing) Then
				Dim declaredSymbol As NamedTypeSymbol = DirectCast(Me.GetDeclaredSymbol(parent.EnumStatement, cancellationToken), NamedTypeSymbol)
				If (declaredSymbol Is Nothing) Then
					fieldSymbol = Nothing
					Return fieldSymbol
				End If
				fieldSymbol = DirectCast(SourceFieldSymbol.FindFieldOrWithEventsSymbolFromSyntax(declarationSyntax.Identifier, Me._syntaxTree, declaredSymbol), Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol)
				Return fieldSymbol
			End If
			fieldSymbol = Nothing
			Return fieldSymbol
		End Function

		Public Overrides Function GetDeclaredSymbol(ByVal declarationSyntax As ModifiedIdentifierSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ISymbol
			Dim declaredSymbol As ISymbol
			Dim parameterSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel
			If (declarationSyntax Is Nothing) Then
				Throw New ArgumentNullException("declarationSyntax")
			End If
			If (Not MyBase.IsInTree(declarationSyntax)) Then
				Throw New ArgumentException(VBResources.DeclarationSyntaxNotWithinTree)
			End If
			Dim parent As VisualBasicSyntaxNode = declarationSyntax.Parent
			Dim fieldDeclarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax = Nothing
			If (parent IsNot Nothing) Then
				fieldDeclarationSyntax = TryCast(parent.Parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax)
			End If
			Dim typeBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax = Nothing
			If (fieldDeclarationSyntax IsNot Nothing) Then
				typeBlockSyntax = TryCast(fieldDeclarationSyntax.Parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeBlockSyntax)
			End If
			If (typeBlockSyntax IsNot Nothing) Then
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(Me.GetDeclaredSymbol(typeBlockSyntax.BlockStatement, cancellationToken), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				If (namedTypeSymbol Is Nothing) Then
					parameterSyntax = TryCast(parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax)
					If (parameterSyntax Is Nothing) Then
						memberSemanticModel = Me.GetMemberSemanticModel(declarationSyntax)
						declaredSymbol = If(memberSemanticModel Is Nothing, MyBase.GetDeclaredSymbol(declarationSyntax, cancellationToken), memberSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken))
					Else
						declaredSymbol = Me.GetDeclaredSymbol(parameterSyntax, cancellationToken)
					End If
					Return declaredSymbol
				End If
				declaredSymbol = SourceFieldSymbol.FindFieldOrWithEventsSymbolFromSyntax(declarationSyntax.Identifier, Me._syntaxTree, namedTypeSymbol)
				Return declaredSymbol
			End If
			parameterSyntax = TryCast(parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax)
			If (parameterSyntax Is Nothing) Then
				memberSemanticModel = Me.GetMemberSemanticModel(declarationSyntax)
				declaredSymbol = If(memberSemanticModel Is Nothing, MyBase.GetDeclaredSymbol(declarationSyntax, cancellationToken), memberSemanticModel.GetDeclaredSymbol(declarationSyntax, cancellationToken))
			Else
				declaredSymbol = Me.GetDeclaredSymbol(parameterSyntax, cancellationToken)
			End If
			Return declaredSymbol
		End Function

		Public Overrides Function GetDeclaredSymbol(ByVal fieldInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldInitializerSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IPropertySymbol
			Dim propertySymbol As IPropertySymbol
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(fieldInitializerSyntax)
			propertySymbol = If(memberSemanticModel Is Nothing, MyBase.GetDeclaredSymbol(fieldInitializerSyntax, cancellationToken), memberSemanticModel.GetDeclaredSymbol(fieldInitializerSyntax, cancellationToken))
			Return propertySymbol
		End Function

		Public Overrides Function GetDeclaredSymbol(ByVal anonymousObjectCreationExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AnonymousObjectCreationExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As INamedTypeSymbol
			Dim namedTypeSymbol As INamedTypeSymbol
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(anonymousObjectCreationExpressionSyntax)
			namedTypeSymbol = If(memberSemanticModel Is Nothing, MyBase.GetDeclaredSymbol(anonymousObjectCreationExpressionSyntax, cancellationToken), memberSemanticModel.GetDeclaredSymbol(anonymousObjectCreationExpressionSyntax, cancellationToken))
			Return namedTypeSymbol
		End Function

		Public Overrides Function GetDeclaredSymbol(ByVal rangeVariableSyntax As ExpressionRangeVariableSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IRangeVariableSymbol
			Dim rangeVariableSymbol As IRangeVariableSymbol
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(rangeVariableSyntax)
			rangeVariableSymbol = If(memberSemanticModel Is Nothing, MyBase.GetDeclaredSymbol(rangeVariableSyntax, cancellationToken), memberSemanticModel.GetDeclaredSymbol(rangeVariableSyntax, cancellationToken))
			Return rangeVariableSymbol
		End Function

		Public Overrides Function GetDeclaredSymbol(ByVal rangeVariableSyntax As CollectionRangeVariableSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IRangeVariableSymbol
			Dim rangeVariableSymbol As IRangeVariableSymbol
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(rangeVariableSyntax)
			rangeVariableSymbol = If(memberSemanticModel Is Nothing, MyBase.GetDeclaredSymbol(rangeVariableSyntax, cancellationToken), memberSemanticModel.GetDeclaredSymbol(rangeVariableSyntax, cancellationToken))
			Return rangeVariableSymbol
		End Function

		Public Overrides Function GetDeclaredSymbol(ByVal rangeVariableSyntax As AggregationRangeVariableSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IRangeVariableSymbol
			Dim rangeVariableSymbol As IRangeVariableSymbol
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(rangeVariableSyntax)
			rangeVariableSymbol = If(memberSemanticModel Is Nothing, MyBase.GetDeclaredSymbol(rangeVariableSyntax, cancellationToken), memberSemanticModel.GetDeclaredSymbol(rangeVariableSyntax, cancellationToken))
			Return rangeVariableSymbol
		End Function

		Public Overrides Function GetDeclaredSymbol(ByVal declarationSyntax As SimpleImportsClauseSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As IAliasSymbol
			Dim [alias] As IAliasSymbol
			If (declarationSyntax Is Nothing) Then
				Throw New ArgumentNullException("declarationSyntax")
			End If
			If (Not MyBase.IsInTree(declarationSyntax)) Then
				Throw New ArgumentException(VBResources.DeclarationSyntaxNotWithinTree)
			End If
			If (declarationSyntax.[Alias] IsNot Nothing) Then
				Dim valueText As String = declarationSyntax.[Alias].Identifier.ValueText
				If (Not [String].IsNullOrEmpty(valueText)) Then
					Dim aliasImportsOpt As IReadOnlyDictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.AliasAndImportsClausePosition) = Me._sourceModule.TryGetSourceFile(Me.SyntaxTree).AliasImportsOpt
					Dim aliasAndImportsClausePosition As Microsoft.CodeAnalysis.VisualBasic.AliasAndImportsClausePosition = New Microsoft.CodeAnalysis.VisualBasic.AliasAndImportsClausePosition()
					If (aliasImportsOpt IsNot Nothing AndAlso aliasImportsOpt.TryGetValue(valueText, aliasAndImportsClausePosition)) Then
						Dim enumerator As ImmutableArray(Of Location).Enumerator = aliasAndImportsClausePosition.[Alias].Locations.GetEnumerator()
						While enumerator.MoveNext()
							Dim current As Location = enumerator.Current
							If (Not current.IsInSource OrElse current.SourceTree <> Me._syntaxTree OrElse Not declarationSyntax.Span.Contains(current.SourceSpan)) Then
								Continue While
							End If
							[alias] = aliasAndImportsClausePosition.[Alias]
							Return [alias]
						End While
						Dim enclosingBinder As Binder = Me.GetEnclosingBinder(declarationSyntax.SpanStart)
						Dim namespaceOrTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceOrTypeSymbol = enclosingBinder.BindNamespaceOrTypeSyntax(declarationSyntax.Name, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded, False)
						If (namespaceOrTypeSymbol Is Nothing) Then
							GoTo Label1
						End If
						[alias] = New AliasSymbol(enclosingBinder.Compilation, enclosingBinder.ContainingNamespaceOrType, valueText, namespaceOrTypeSymbol, declarationSyntax.GetLocation())
						Return [alias]
					End If
				End If
			Label1:
				[alias] = Nothing
			Else
				[alias] = Nothing
			End If
			Return [alias]
		End Function

		Friend Overrides Function GetDeclaredSymbols(ByVal declarationSyntax As FieldDeclarationSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of ISymbol)
			If (declarationSyntax Is Nothing) Then
				Throw New ArgumentNullException("declarationSyntax")
			End If
			If (Not MyBase.IsInTree(declarationSyntax)) Then
				Throw New ArgumentException(VBResources.DeclarationSyntaxNotWithinTree)
			End If
			Dim symbols As ArrayBuilder(Of ISymbol) = New ArrayBuilder(Of ISymbol)()
			Dim enumerator As SeparatedSyntaxList(Of VariableDeclaratorSyntax).Enumerator = declarationSyntax.Declarators.GetEnumerator()
			While enumerator.MoveNext()
				Dim enumerator1 As SeparatedSyntaxList(Of ModifiedIdentifierSyntax).Enumerator = enumerator.Current.Names.GetEnumerator()
				While enumerator1.MoveNext()
					Dim declaredSymbol As IFieldSymbol = TryCast(Me.GetDeclaredSymbol(enumerator1.Current, cancellationToken), IFieldSymbol)
					If (declaredSymbol Is Nothing) Then
						Continue While
					End If
					symbols.Add(declaredSymbol)
				End While
			End While
			Return symbols.ToImmutableAndFree()
		End Function

		Public Overrides Function GetDiagnostics(Optional ByVal span As Nullable(Of TextSpan) = Nothing, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of Diagnostic)
			Return Me._compilation.GetDiagnosticsForSyntaxTree(CompilationStage.Compile, Me._syntaxTree, span, True, cancellationToken)
		End Function

		Friend Overrides Function GetEnclosingBinder(ByVal position As Integer) As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(position)
			binder = If(memberSemanticModel Is Nothing, SemanticModelBinder.Mark(Me._binderFactory.GetBinderForPosition(MyBase.FindInitialNodeFromPosition(position), position), Me.IgnoresAccessibility), memberSemanticModel.GetEnclosingBinder(position))
			Return binder
		End Function

		Friend Overrides Function GetExpressionConstantValue(ByVal node As ExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ConstantValue
			Dim expressionConstantValue As ConstantValue
			node = SyntaxFactory.GetStandaloneExpression(node)
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(node)
			If (memberSemanticModel Is Nothing) Then
				expressionConstantValue = Nothing
			Else
				expressionConstantValue = memberSemanticModel.GetExpressionConstantValue(node, cancellationToken)
			End If
			Return expressionConstantValue
		End Function

		Friend Overrides Function GetExpressionMemberGroup(ByVal node As ExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of Symbol)
			Dim empty As ImmutableArray(Of Symbol)
			node = SyntaxFactory.GetStandaloneExpression(node)
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(node)
			If (memberSemanticModel Is Nothing) Then
				empty = ImmutableArray(Of Symbol).Empty
			Else
				empty = memberSemanticModel.GetExpressionMemberGroup(node, cancellationToken)
			End If
			Return empty
		End Function

		Friend Overrides Function GetExpressionSymbolInfo(ByVal node As ExpressionSyntax, ByVal options As VBSemanticModel.SymbolInfoOptions, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.SymbolInfo
			Dim expressionSymbolInfo As Microsoft.CodeAnalysis.SymbolInfo
			MyBase.ValidateSymbolInfoOptions(options)
			node = SyntaxFactory.GetStandaloneExpression(node)
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(node)
			If (memberSemanticModel IsNot Nothing) Then
				expressionSymbolInfo = memberSemanticModel.GetExpressionSymbolInfo(node, options, cancellationToken)
				If (expressionSymbolInfo.IsEmpty AndAlso SyntaxFacts.IsInNamespaceOrTypeContext(node)) Then
					Dim symbolInfo As Microsoft.CodeAnalysis.SymbolInfo = Me.TryBindNamespaceOrTypeAsExpression(node, options)
					If (Not symbolInfo.IsEmpty) Then
						expressionSymbolInfo = symbolInfo
					End If
				End If
			ElseIf (SyntaxFacts.IsImplementedMember(node)) Then
				expressionSymbolInfo = Me.GetImplementedMemberSymbolInfo(DirectCast(node, QualifiedNameSyntax), options)
			ElseIf (SyntaxFacts.IsHandlesEvent(node)) Then
				expressionSymbolInfo = Me.GetHandlesEventSymbolInfo(DirectCast(node.Parent, HandlesClauseItemSyntax), options)
			ElseIf (SyntaxFacts.IsHandlesContainer(node)) Then
				Dim parent As VisualBasicSyntaxNode = node.Parent
				If (parent.Kind() <> SyntaxKind.HandlesClauseItem) Then
					parent = parent.Parent
				End If
				expressionSymbolInfo = Me.GetHandlesContainerSymbolInfo(DirectCast(parent, HandlesClauseItemSyntax), options)
			ElseIf (SyntaxFacts.IsHandlesProperty(node)) Then
				expressionSymbolInfo = Me.GetHandlesPropertySymbolInfo(DirectCast(node.Parent.Parent, HandlesClauseItemSyntax), options)
			ElseIf (Not VBSemanticModel.IsInCrefOrNameAttributeInterior(node)) Then
				expressionSymbolInfo = If(Not SyntaxFacts.IsInNamespaceOrTypeContext(node), Microsoft.CodeAnalysis.SymbolInfo.None, Me.GetTypeOrNamespaceSymbolInfoNotInMember(DirectCast(node, TypeSyntax), options))
			Else
				expressionSymbolInfo = Me.GetSymbolInfoForCrefOrNameAttributeReference(node, options)
			End If
			Return expressionSymbolInfo
		End Function

		Friend Overrides Function GetExpressionTypeInfo(ByVal node As ExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As VisualBasicTypeInfo
			Dim expressionTypeInfo As VisualBasicTypeInfo
			node = SyntaxFactory.GetStandaloneExpression(node)
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(node)
			If (memberSemanticModel IsNot Nothing) Then
				expressionTypeInfo = memberSemanticModel.GetExpressionTypeInfo(node, cancellationToken)
			ElseIf (SyntaxFacts.IsImplementedMember(node)) Then
				expressionTypeInfo = Me.GetImplementedMemberTypeInfo(DirectCast(node, QualifiedNameSyntax))
			ElseIf (SyntaxFacts.IsHandlesEvent(node)) Then
				expressionTypeInfo = Me.GetHandlesEventTypeInfo(DirectCast(node, IdentifierNameSyntax))
			ElseIf (SyntaxFacts.IsHandlesContainer(node)) Then
				Dim parent As VisualBasicSyntaxNode = node.Parent
				If (parent.Kind() <> SyntaxKind.HandlesClauseItem) Then
					parent = parent.Parent
				End If
				expressionTypeInfo = Me.GetHandlesContainerTypeInfo(DirectCast(parent, HandlesClauseItemSyntax))
			ElseIf (Not SyntaxFacts.IsHandlesProperty(node)) Then
				If (Not VBSemanticModel.IsInCrefOrNameAttributeInterior(node)) Then
					If (Not SyntaxFacts.IsInNamespaceOrTypeContext(node)) Then
						GoTo Label1
					End If
					expressionTypeInfo = Me.GetTypeOrNamespaceTypeInfoNotInMember(DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax))
					Return expressionTypeInfo
				Else
					Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)
					If (typeSyntax Is Nothing) Then
						GoTo Label1
					End If
					expressionTypeInfo = Me.GetTypeInfoForCrefOrNameAttributeReference(typeSyntax)
					Return expressionTypeInfo
				End If
			Label1:
				expressionTypeInfo = VisualBasicTypeInfo.None
			Else
				expressionTypeInfo = Me.GetHandlesPropertyTypeInfo(DirectCast(node.Parent.Parent, HandlesClauseItemSyntax))
			End If
			Return expressionTypeInfo
		End Function

		Friend Overrides Function GetForEachStatementInfoWorker(ByVal node As ForEachBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.ForEachStatementInfo
			Dim forEachStatementInfo As Microsoft.CodeAnalysis.VisualBasic.ForEachStatementInfo
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(node)
			forEachStatementInfo = If(memberSemanticModel Is Nothing, New Microsoft.CodeAnalysis.VisualBasic.ForEachStatementInfo(), memberSemanticModel.GetForEachStatementInfoWorker(node))
			Return forEachStatementInfo
		End Function

		Private Function GetHandledEventOrContainerSymbolsAndResultKind(ByVal eventSymbolBuilder As ArrayBuilder(Of Symbol), ByVal containerSymbolBuilder As ArrayBuilder(Of Symbol), ByVal propertySymbolBuilder As ArrayBuilder(Of Symbol), ByVal handlesClause As HandlesClauseItemSyntax) As Microsoft.CodeAnalysis.VisualBasic.LookupResultKind
			Dim lookupResultKind As Microsoft.CodeAnalysis.VisualBasic.LookupResultKind = Microsoft.CodeAnalysis.VisualBasic.LookupResultKind.Good
			Dim enclosingBinder As Binder = Me.GetEnclosingBinder(handlesClause.SpanStart)
			Dim parent As MethodStatementSyntax = TryCast(handlesClause.Parent.Parent, MethodStatementSyntax)
			If (parent IsNot Nothing) Then
				Dim declaredSymbol As IMethodSymbol = MyBase.GetDeclaredSymbol(parent, New CancellationToken())
				If (declaredSymbol IsNot Nothing) Then
					DirectCast(declaredSymbol, SourceMemberMethodSymbol).BindSingleHandlesClause(handlesClause, enclosingBinder, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded, eventSymbolBuilder, containerSymbolBuilder, propertySymbolBuilder, lookupResultKind)
				End If
			End If
			Return lookupResultKind
		End Function

		Private Function GetHandlesContainerSymbolInfo(ByVal handlesClause As HandlesClauseItemSyntax, ByVal options As VBSemanticModel.SymbolInfoOptions) As SymbolInfo
			Dim instance As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance()
			Dim handledEventOrContainerSymbolsAndResultKind As LookupResultKind = Me.GetHandledEventOrContainerSymbolsAndResultKind(Nothing, instance, Nothing, handlesClause)
			Dim symbols As ImmutableArray(Of Symbol) = MyBase.RemoveErrorTypesAndDuplicates(instance, options)
			instance.Free()
			Return SymbolInfoFactory.Create(symbols, handledEventOrContainerSymbolsAndResultKind)
		End Function

		Private Function GetHandlesContainerTypeInfo(ByVal memberName As HandlesClauseItemSyntax) As VisualBasicTypeInfo
			Return VisualBasicTypeInfo.None
		End Function

		Private Function GetHandlesEventSymbolInfo(ByVal handlesClause As HandlesClauseItemSyntax, ByVal options As VBSemanticModel.SymbolInfoOptions) As SymbolInfo
			Dim instance As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance()
			Dim handledEventOrContainerSymbolsAndResultKind As LookupResultKind = Me.GetHandledEventOrContainerSymbolsAndResultKind(instance, Nothing, Nothing, handlesClause)
			Dim symbols As ImmutableArray(Of Symbol) = MyBase.RemoveErrorTypesAndDuplicates(instance, options)
			instance.Free()
			Return SymbolInfoFactory.Create(symbols, handledEventOrContainerSymbolsAndResultKind)
		End Function

		Private Function GetHandlesEventTypeInfo(ByVal memberName As IdentifierNameSyntax) As VisualBasicTypeInfo
			Return VisualBasicTypeInfo.None
		End Function

		Private Function GetHandlesPropertySymbolInfo(ByVal handlesClause As HandlesClauseItemSyntax, ByVal options As VBSemanticModel.SymbolInfoOptions) As SymbolInfo
			Dim instance As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance()
			Dim handledEventOrContainerSymbolsAndResultKind As LookupResultKind = Me.GetHandledEventOrContainerSymbolsAndResultKind(Nothing, Nothing, instance, handlesClause)
			Dim symbols As ImmutableArray(Of Symbol) = MyBase.RemoveErrorTypesAndDuplicates(instance, options)
			instance.Free()
			Return SymbolInfoFactory.Create(symbols, handledEventOrContainerSymbolsAndResultKind)
		End Function

		Private Function GetHandlesPropertyTypeInfo(ByVal memberName As HandlesClauseItemSyntax) As VisualBasicTypeInfo
			Return VisualBasicTypeInfo.None
		End Function

		Private Function GetImplementedMemberAndResultKind(ByVal symbolBuilder As ArrayBuilder(Of Symbol), ByVal memberName As QualifiedNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.LookupResultKind
			Dim lookupResultKind As Microsoft.CodeAnalysis.VisualBasic.LookupResultKind = Microsoft.CodeAnalysis.VisualBasic.LookupResultKind.Good
			Dim enclosingBinder As Binder = Me.GetEnclosingBinder(memberName.SpanStart)
			Dim parent As MethodBaseSyntax = TryCast(memberName.Parent.Parent, MethodBaseSyntax)
			If (parent IsNot Nothing) Then
				Dim declaredSymbol As ISymbol = Me.GetDeclaredSymbol(parent, New CancellationToken())
				If (declaredSymbol IsNot Nothing) Then
					Dim kind As SymbolKind = declaredSymbol.Kind
					If (kind = SymbolKind.[Event]) Then
						ImplementsHelper.FindExplicitlyImplementedMember(Of EventSymbol)(DirectCast(declaredSymbol, EventSymbol), DirectCast(declaredSymbol, EventSymbol).ContainingType, memberName, enclosingBinder, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded, symbolBuilder, lookupResultKind)
					ElseIf (kind = SymbolKind.Method) Then
						ImplementsHelper.FindExplicitlyImplementedMember(Of MethodSymbol)(DirectCast(declaredSymbol, MethodSymbol), DirectCast(declaredSymbol, MethodSymbol).ContainingType, memberName, enclosingBinder, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded, symbolBuilder, lookupResultKind)
					Else
						If (kind <> SymbolKind.[Property]) Then
							Throw ExceptionUtilities.UnexpectedValue(declaredSymbol.Kind)
						End If
						ImplementsHelper.FindExplicitlyImplementedMember(Of PropertySymbol)(DirectCast(declaredSymbol, PropertySymbol), DirectCast(declaredSymbol, PropertySymbol).ContainingType, memberName, enclosingBinder, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded, symbolBuilder, lookupResultKind)
					End If
				End If
			End If
			Return lookupResultKind
		End Function

		Private Function GetImplementedMemberSymbolInfo(ByVal memberName As QualifiedNameSyntax, ByVal options As VBSemanticModel.SymbolInfoOptions) As SymbolInfo
			Dim instance As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance()
			Dim implementedMemberAndResultKind As LookupResultKind = Me.GetImplementedMemberAndResultKind(instance, memberName)
			Dim symbols As ImmutableArray(Of Symbol) = MyBase.RemoveErrorTypesAndDuplicates(instance, options)
			instance.Free()
			Return SymbolInfoFactory.Create(symbols, implementedMemberAndResultKind)
		End Function

		Private Function GetImplementedMemberTypeInfo(ByVal memberName As QualifiedNameSyntax) As VisualBasicTypeInfo
			Return VisualBasicTypeInfo.None
		End Function

		Friend Overrides Function GetInvokeSummaryForRaiseEvent(ByVal node As RaiseEventStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.BoundNodeSummary
			Dim boundNodeSummary As Microsoft.CodeAnalysis.VisualBasic.BoundNodeSummary
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(node)
			boundNodeSummary = If(memberSemanticModel Is Nothing, New Microsoft.CodeAnalysis.VisualBasic.BoundNodeSummary(), memberSemanticModel.GetInvokeSummaryForRaiseEvent(node))
			Return boundNodeSummary
		End Function

		Friend Overrides Function GetLetClauseSymbolInfo(ByVal node As ExpressionRangeVariableSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.SymbolInfo
			Dim symbolInfo As Microsoft.CodeAnalysis.SymbolInfo
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(node)
			symbolInfo = If(memberSemanticModel Is Nothing, Microsoft.CodeAnalysis.SymbolInfo.None, memberSemanticModel.GetLetClauseSymbolInfo(node, cancellationToken))
			Return symbolInfo
		End Function

		Public Function GetMemberSemanticModel(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder) As MemberSemanticModel
			Dim orAdd As MemberSemanticModel
			If (TypeOf binder Is MethodBodyBinder) Then
				orAdd = Me._semanticModelCache.GetOrAdd(New ValueTuple(Of Microsoft.CodeAnalysis.VisualBasic.Binder, Boolean)(binder, Me.IgnoresAccessibility), Me._methodBodySemanticModelCreator)
			ElseIf (TypeOf binder Is DeclarationInitializerBinder) Then
				orAdd = Me._semanticModelCache.GetOrAdd(New ValueTuple(Of Microsoft.CodeAnalysis.VisualBasic.Binder, Boolean)(binder, Me.IgnoresAccessibility), Me._initializerSemanticModelCreator)
			ElseIf (TypeOf binder Is AttributeBinder) Then
				orAdd = Me._semanticModelCache.GetOrAdd(New ValueTuple(Of Microsoft.CodeAnalysis.VisualBasic.Binder, Boolean)(binder, Me.IgnoresAccessibility), Me._attributeSemanticModelCreator)
			ElseIf (Not TypeOf binder Is TopLevelCodeBinder) Then
				orAdd = Nothing
			Else
				orAdd = Me._semanticModelCache.GetOrAdd(New ValueTuple(Of Microsoft.CodeAnalysis.VisualBasic.Binder, Boolean)(binder, Me.IgnoresAccessibility), Me._methodBodySemanticModelCreator)
			End If
			Return orAdd
		End Function

		Friend Function GetMemberSemanticModel(ByVal position As Integer) As MemberSemanticModel
			Dim binderForPosition As Binder = Me._binderFactory.GetBinderForPosition(MyBase.FindInitialNodeFromPosition(position), position)
			Return Me.GetMemberSemanticModel(binderForPosition)
		End Function

		Friend Function GetMemberSemanticModel(ByVal node As SyntaxNode) As MemberSemanticModel
			Return Me.GetMemberSemanticModel(node.SpanStart)
		End Function

		Public Overrides Function GetMethodBodyDiagnostics(Optional ByVal span As Nullable(Of TextSpan) = Nothing, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of Diagnostic)
			Return Me._compilation.GetDiagnosticsForSyntaxTree(CompilationStage.Compile, Me._syntaxTree, span, False, cancellationToken)
		End Function

		Friend Overrides Function GetOperationWorker(ByVal node As VisualBasicSyntaxNode, ByVal cancellationToken As System.Threading.CancellationToken) As IOperation
			Dim operationWorker As IOperation
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel
			Dim methodBlockBaseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax)
			memberSemanticModel = If(methodBlockBaseSyntax Is Nothing, Me.GetMemberSemanticModel(node), Me.GetMemberSemanticModel(methodBlockBaseSyntax.BlockStatement.EndPosition))
			If (memberSemanticModel Is Nothing) Then
				operationWorker = Nothing
			Else
				operationWorker = memberSemanticModel.GetOperationWorker(node, cancellationToken)
			End If
			Return operationWorker
		End Function

		Friend Overrides Function GetOrderingSymbolInfo(ByVal node As OrderingSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.SymbolInfo
			Dim symbolInfo As Microsoft.CodeAnalysis.SymbolInfo
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(node)
			symbolInfo = If(memberSemanticModel Is Nothing, Microsoft.CodeAnalysis.SymbolInfo.None, memberSemanticModel.GetOrderingSymbolInfo(node, cancellationToken))
			Return symbolInfo
		End Function

		Friend Overrides Function GetQueryClauseSymbolInfo(ByVal node As QueryClauseSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Microsoft.CodeAnalysis.SymbolInfo
			Dim symbolInfo As Microsoft.CodeAnalysis.SymbolInfo
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(node)
			symbolInfo = If(memberSemanticModel Is Nothing, Microsoft.CodeAnalysis.SymbolInfo.None, memberSemanticModel.GetQueryClauseSymbolInfo(node, cancellationToken))
			Return symbolInfo
		End Function

		Private Function GetSymbolInfoForCrefOrNameAttributeReference(ByVal node As VisualBasicSyntaxNode, ByVal options As VBSemanticModel.SymbolInfoOptions) As Microsoft.CodeAnalysis.SymbolInfo
			Dim symbolInfo As Microsoft.CodeAnalysis.SymbolInfo
			Dim symbols As ImmutableArray(Of Symbol) = New ImmutableArray(Of Symbol)()
			Dim crefOrNameAttributeReferenceSymbols As ImmutableArray(Of Symbol) = Me.GetCrefOrNameAttributeReferenceSymbols(node, CInt((options And VBSemanticModel.SymbolInfoOptions.ResolveAliases)) = 0, symbols)
			If (Not crefOrNameAttributeReferenceSymbols.IsDefaultOrEmpty) Then
				If (crefOrNameAttributeReferenceSymbols.Length = 1) Then
					Dim symbolInfoForSymbol As Microsoft.CodeAnalysis.SymbolInfo = MyBase.GetSymbolInfoForSymbol(crefOrNameAttributeReferenceSymbols(0), options)
					If (symbolInfoForSymbol.CandidateReason = CandidateReason.None) Then
						symbolInfo = symbolInfoForSymbol
						Return symbolInfo
					End If
					crefOrNameAttributeReferenceSymbols = ImmutableArray(Of Symbol).Empty
				End If
				Dim instance As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance()
				instance.AddRange(crefOrNameAttributeReferenceSymbols)
				Dim symbols1 As ImmutableArray(Of Symbol) = MyBase.RemoveErrorTypesAndDuplicates(instance, options)
				instance.Free()
				symbolInfo = If(symbols1.Length <> 0, SymbolInfoFactory.Create(symbols1, If(symbols1.Length = 1, LookupResultKind.Good, LookupResultKind.Ambiguous)), SymbolInfoFactory.Create(symbols1, LookupResultKind.Empty))
			Else
				symbolInfo = If(Not symbols.IsDefaultOrEmpty, SymbolInfoFactory.Create(symbols, LookupResultKind.NotReferencable), Microsoft.CodeAnalysis.SymbolInfo.None)
			End If
			Return symbolInfo
		End Function

		Public Overrides Function GetSyntaxDiagnostics(Optional ByVal span As Nullable(Of TextSpan) = Nothing, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of Diagnostic)
			Return Me._compilation.GetDiagnosticsForSyntaxTree(CompilationStage.Parse, Me._syntaxTree, span, False, cancellationToken)
		End Function

		Private Function GetTypeInfoForCrefOrNameAttributeReference(ByVal name As TypeSyntax) As VisualBasicTypeInfo
			Dim none As VisualBasicTypeInfo
			Dim item As Symbol
			Dim kind As SymbolKind
			Dim symbols As ImmutableArray(Of Symbol) = New ImmutableArray(Of Symbol)()
			Dim crefOrNameAttributeReferenceSymbols As ImmutableArray(Of Symbol) = Me.GetCrefOrNameAttributeReferenceSymbols(name, False, symbols)
			If (crefOrNameAttributeReferenceSymbols.IsDefaultOrEmpty) Then
				crefOrNameAttributeReferenceSymbols = symbols
				If (Not crefOrNameAttributeReferenceSymbols.IsDefaultOrEmpty) Then
					If (crefOrNameAttributeReferenceSymbols.Length <= 1) Then
						item = crefOrNameAttributeReferenceSymbols(0)
						kind = item.Kind
						none = If(kind = SymbolKind.ArrayType OrElse kind = SymbolKind.NamedType OrElse kind = SymbolKind.TypeParameter, MyBase.GetTypeInfoForSymbol(item), VisualBasicTypeInfo.None)
					Else
						none = VisualBasicTypeInfo.None
					End If
					Return none
				End If
				none = VisualBasicTypeInfo.None
				Return none
			End If
			If (crefOrNameAttributeReferenceSymbols.Length <= 1) Then
				item = crefOrNameAttributeReferenceSymbols(0)
				kind = item.Kind
				none = If(kind = SymbolKind.ArrayType OrElse kind = SymbolKind.NamedType OrElse kind = SymbolKind.TypeParameter, MyBase.GetTypeInfoForSymbol(item), VisualBasicTypeInfo.None)
			Else
				none = VisualBasicTypeInfo.None
			End If
			Return none
		End Function

		Private Function GetTypeOrNamespaceSymbolInfoNotInMember(ByVal expression As TypeSyntax, ByVal options As VBSemanticModel.SymbolInfoOptions) As Microsoft.CodeAnalysis.SymbolInfo
			Dim current As ISymbol
			Dim typeOrNamespaceSymbolNotInMember As Symbol = Me.GetTypeOrNamespaceSymbolNotInMember(expression)
			If (typeOrNamespaceSymbolNotInMember.Kind = SymbolKind.[Namespace] AndAlso expression.Parent IsNot Nothing AndAlso expression.Parent.Kind() = SyntaxKind.QualifiedName AndAlso DirectCast(expression.Parent, QualifiedNameSyntax).Left = expression) Then
				Dim namespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = DirectCast(typeOrNamespaceSymbolNotInMember, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol)
				If (CInt(namespaceSymbol.NamespaceKind) = 0) Then
					Dim typeOrNamespaceSymbolInfoNotInMember As Microsoft.CodeAnalysis.SymbolInfo = Me.GetTypeOrNamespaceSymbolInfoNotInMember(DirectCast(expression.Parent, QualifiedNameSyntax), 0)
					If (Not typeOrNamespaceSymbolInfoNotInMember.IsEmpty) Then
						Dim namespaceSymbols As SmallDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol, Boolean) = New SmallDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol, Boolean)()
						If (typeOrNamespaceSymbolInfoNotInMember.Symbol Is Nothing) Then
							Dim enumerator As ImmutableArray(Of ISymbol).Enumerator = typeOrNamespaceSymbolInfoNotInMember.CandidateSymbols.GetEnumerator()
							Do
								If (Not enumerator.MoveNext()) Then
									GoTo Label0
								End If
								current = enumerator.Current
							Loop While Binder.AddReceiverNamespaces(namespaceSymbols, DirectCast(current, Symbol), Me.Compilation)
							namespaceSymbols = Nothing
						ElseIf (Not Binder.AddReceiverNamespaces(namespaceSymbols, DirectCast(typeOrNamespaceSymbolInfoNotInMember.Symbol, Symbol), Me.Compilation)) Then
							namespaceSymbols = Nothing
						End If
					Label0:
						If (namespaceSymbols IsNot Nothing AndAlso namespaceSymbols.Count() < namespaceSymbol.ConstituentNamespaces.Length) Then
							typeOrNamespaceSymbolNotInMember = DirectCast(namespaceSymbol, MergedNamespaceSymbol).Shrink(DirectCast(namespaceSymbols.Keys, IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol)))
						End If
					End If
				End If
			End If
			Dim symbolInfoForSymbol As Microsoft.CodeAnalysis.SymbolInfo = MyBase.GetSymbolInfoForSymbol(typeOrNamespaceSymbolNotInMember, options)
			If (symbolInfoForSymbol.IsEmpty) Then
				Dim symbolInfo As Microsoft.CodeAnalysis.SymbolInfo = Me.TryBindNamespaceOrTypeAsExpression(expression, options)
				If (Not symbolInfo.IsEmpty) Then
					symbolInfoForSymbol = symbolInfo
				End If
			End If
			Return symbolInfoForSymbol
		End Function

		Private Function GetTypeOrNamespaceSymbolNotInMember(ByVal expression As TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Symbol
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol
			Dim enclosingBinder As Binder = Me.GetEnclosingBinder(expression.SpanStart)
			symbol = If(Not SyntaxFacts.IsInTypeOnlyContext(expression), enclosingBinder.BindNamespaceOrTypeOrAliasSyntax(expression, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded, False), enclosingBinder.BindTypeOrAliasSyntax(expression, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded, False))
			Return symbol
		End Function

		Private Function GetTypeOrNamespaceTypeInfoNotInMember(ByVal expression As TypeSyntax) As VisualBasicTypeInfo
			Return MyBase.GetTypeInfoForSymbol(Me.GetTypeOrNamespaceSymbolNotInMember(expression))
		End Function

		Private Function GetTypeParameterSymbol(ByVal parameters As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol), ByVal parameter As TypeParameterSyntax) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol
			Dim typeParameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol
			Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol).Enumerator = parameters.GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol = enumerator.Current
					Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.Location).Enumerator = current.Locations.GetEnumerator()
					While enumerator1.MoveNext()
						Dim location As Microsoft.CodeAnalysis.Location = enumerator1.Current
						If (Not location.IsInSource OrElse location.SourceTree <> Me._syntaxTree OrElse Not parameter.Span.Contains(location.SourceSpan)) Then
							Continue While
						End If
						typeParameterSymbol = current
						Return typeParameterSymbol
					End While
				Else
					typeParameterSymbol = Nothing
					Exit While
				End If
			End While
			Return typeParameterSymbol
		End Function

		Private Shared Function IsExpressionInValidContext(ByVal expression As ExpressionSyntax) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = expression
			While True
				Dim parent As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = visualBasicSyntaxNode.Parent
				If (parent IsNot Nothing) Then
					If (Not TypeOf parent Is ExpressionSyntax) Then
						syntaxKind = parent.Kind()
						If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NextStatement) Then
							Select Case syntaxKind
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectMemberInitializer
									GoTo Label2
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectCollectionInitializer
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferredFieldInitializer
									flag = True
									Return flag
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer
									If (DirectCast(parent, NamedFieldInitializerSyntax).Name <> visualBasicSyntaxNode) Then
										GoTo Label2
									End If
									flag = False
									Return flag
								Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsValue
									parent = parent.Parent
									If (parent IsNot Nothing) Then
										Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = parent.Kind()
										If (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration) Then
											If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariableDeclarator) Then
												Dim localDeclarationStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.LocalDeclarationStatementSyntax = TryCast(parent.Parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.LocalDeclarationStatementSyntax)
												If (localDeclarationStatementSyntax IsNot Nothing) Then
													Dim enumerator As SyntaxTokenList.Enumerator = localDeclarationStatementSyntax.Modifiers.GetEnumerator()
													While enumerator.MoveNext()
														If (enumerator.Current.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstKeyword) Then
															Continue While
														End If
														flag = False
														Return flag
													End While
												End If
												flag = True
												Return flag
											ElseIf (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter) Then
												flag = True
												Return flag
											End If
										End If
										flag = False
										Return flag
									Else
										flag = True
										Return flag
									End If
								Case Else
									If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GoToStatement) Then
										Exit Select
									Else
										GoTo Label3
									End If
							End Select
							flag = False
							Exit While
						ElseIf (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RangeArgument) Then
							If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventStatement) Then
								flag = False
								Exit While
							Else
								Select Case syntaxKind
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArgumentList
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleArgument
										Exit Select
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OmittedArgument
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndAddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterSingleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RightShiftAssignmentStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReDimStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RedimClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharacterLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FalseLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DateLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MeExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MyClassExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeOfIsExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotEqualsExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanOrEqualExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsNotExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndAlsoExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UnaryMinusExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddressOfExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TernaryConditionalExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineSubLambdaExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineSubLambdaExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionLambdaHeader Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OmittedArgument
										flag = True
										Return flag
									Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RangeArgument
										If (DirectCast(parent, RangeArgumentSyntax).LowerBound <> visualBasicSyntaxNode) Then
											Exit Select
										End If
										flag = False
										Return flag
									Case Else
										flag = True
										Return flag
								End Select
							End If
						ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlDeclarationOption) Then
							flag = False
							Exit While
						ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NameColonEquals) Then
							flag = False
							Exit While
						Else
							flag = True
							Return flag
						End If
					ElseIf (parent.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlElementEndTag) Then
						flag = False
						Exit While
					End If
				Label2:
					visualBasicSyntaxNode = parent
				Else
					flag = True
					Exit While
				End If
			End While
			Return flag
		Label3:
			If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NextStatement) Then
				flag = False
				Return flag
			Else
				flag = True
				Return flag
			End If
		End Function

		Private Shared Function IsNodeInsideAttributeArguments(ByVal node As VisualBasicSyntaxNode) As Boolean
			Dim flag As Boolean
			While True
				If (node Is Nothing) Then
					flag = False
					Exit While
				ElseIf (node.Kind() <> SyntaxKind.Attribute) Then
					node = node.Parent
				Else
					flag = True
					Exit While
				End If
			End While
			Return flag
		End Function

		Private Function IsNotUppermostForBlock(ByVal forBlockOrStatement As VisualBasicSyntaxNode) As Boolean
			Dim count As Boolean
			Dim controlVariables As SeparatedSyntaxList(Of ExpressionSyntax)
			Dim forOrForEachBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachBlockSyntax = TryCast(forBlockOrStatement, Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachBlockSyntax)
			If (forOrForEachBlockSyntax IsNot Nothing) Then
				Dim nextStatement As NextStatementSyntax = forOrForEachBlockSyntax.NextStatement
				If (nextStatement Is Nothing) Then
					Dim num As Integer = 1
					While forOrForEachBlockSyntax.Statements.Count <> 0
						Dim forOrForEachBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachBlockSyntax = TryCast(forOrForEachBlockSyntax.Statements.Last(), Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachBlockSyntax)
						If (forOrForEachBlockSyntax1 IsNot Nothing) Then
							num = num + 1
							nextStatement = forOrForEachBlockSyntax1.NextStatement
							If (nextStatement Is Nothing) Then
								forOrForEachBlockSyntax = forOrForEachBlockSyntax1
							Else
								controlVariables = nextStatement.ControlVariables
								count = controlVariables.Count <> num
								Return count
							End If
						Else
							count = True
							Return count
						End If
					End While
					count = True
				Else
					controlVariables = nextStatement.ControlVariables
					count = controlVariables.Count > 1
				End If
			Else
				count = False
			End If
			Return count
		End Function

		Private Function TryBindNamespaceOrTypeAsExpression(ByVal node As ExpressionSyntax, ByVal options As VBSemanticModel.SymbolInfoOptions) As SymbolInfo
			Dim none As SymbolInfo
			Dim enclosingBinder As Binder = Me.GetEnclosingBinder(node.SpanStart)
			If (enclosingBinder IsNot Nothing) Then
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = enclosingBinder.BindExpression(node, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded)
				Dim symbolInfoForNode As SymbolInfo = MyBase.GetSymbolInfoForNode(options, New BoundNodeSummary(boundExpression, boundExpression, Nothing), Nothing)
				If (symbolInfoForNode.GetAllSymbols().IsDefaultOrEmpty) Then
					none = SymbolInfo.None
					Return none
				End If
				none = SymbolInfoFactory.Create(symbolInfoForNode.GetAllSymbols(), LookupResultKind.NotATypeOrNamespace)
				Return none
			End If
			none = SymbolInfo.None
			Return none
		End Function

		Friend Overrides Function TryGetSpeculativeSemanticModelCore(ByVal parentModel As SyntaxTreeSemanticModel, ByVal position As Integer, ByVal type As TypeSyntax, ByVal bindingOption As SpeculativeBindingOption, <Out> ByRef speculativeModel As SemanticModel) As Boolean
			Dim flag As Boolean
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(position)
			If (memberSemanticModel Is Nothing) Then
				Dim speculativeBinderForExpression As Binder = MyBase.GetSpeculativeBinderForExpression(position, type, bindingOption)
				If (speculativeBinderForExpression Is Nothing) Then
					speculativeModel = Nothing
					flag = False
				Else
					speculativeModel = SpeculativeSyntaxTreeSemanticModel.Create(Me, type, speculativeBinderForExpression, position, bindingOption)
					flag = True
				End If
			Else
				flag = memberSemanticModel.TryGetSpeculativeSemanticModelCore(parentModel, position, type, bindingOption, speculativeModel)
			End If
			Return flag
		End Function

		Friend Overrides Function TryGetSpeculativeSemanticModelCore(ByVal parentModel As SyntaxTreeSemanticModel, ByVal position As Integer, ByVal rangeArgument As RangeArgumentSyntax, <Out> ByRef speculativeModel As SemanticModel) As Boolean
			Dim flag As Boolean
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(position)
			If (memberSemanticModel Is Nothing) Then
				speculativeModel = Nothing
				flag = False
			Else
				flag = memberSemanticModel.TryGetSpeculativeSemanticModelCore(parentModel, position, rangeArgument, speculativeModel)
			End If
			Return flag
		End Function

		Friend Overrides Function TryGetSpeculativeSemanticModelCore(ByVal parentModel As SyntaxTreeSemanticModel, ByVal position As Integer, ByVal statement As ExecutableStatementSyntax, <Out> ByRef speculativeModel As SemanticModel) As Boolean
			Dim flag As Boolean
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(position)
			If (memberSemanticModel Is Nothing) Then
				speculativeModel = Nothing
				flag = False
			Else
				flag = memberSemanticModel.TryGetSpeculativeSemanticModelCore(parentModel, position, statement, speculativeModel)
			End If
			Return flag
		End Function

		Friend Overrides Function TryGetSpeculativeSemanticModelCore(ByVal parentModel As SyntaxTreeSemanticModel, ByVal position As Integer, ByVal initializer As EqualsValueSyntax, <Out> ByRef speculativeModel As SemanticModel) As Boolean
			Dim flag As Boolean
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(position)
			If (memberSemanticModel Is Nothing) Then
				speculativeModel = Nothing
				flag = False
			Else
				flag = memberSemanticModel.TryGetSpeculativeSemanticModelCore(parentModel, position, initializer, speculativeModel)
			End If
			Return flag
		End Function

		Friend Overrides Function TryGetSpeculativeSemanticModelForMethodBodyCore(ByVal parentModel As SyntaxTreeSemanticModel, ByVal position As Integer, ByVal method As MethodBlockBaseSyntax, <Out> ByRef speculativeModel As SemanticModel) As Boolean
			Dim flag As Boolean
			Dim memberSemanticModel As Microsoft.CodeAnalysis.VisualBasic.MemberSemanticModel = Me.GetMemberSemanticModel(position)
			If (memberSemanticModel Is Nothing) Then
				speculativeModel = Nothing
				flag = False
			Else
				flag = memberSemanticModel.TryGetSpeculativeSemanticModelForMethodBodyCore(parentModel, position, method, speculativeModel)
			End If
			Return flag
		End Function

		Private Function ValidateRegionDefiningExpression(ByVal expression As ExpressionSyntax) As Boolean
			Dim flag As Boolean
			Me.AssertNodeInTree(expression, "expression")
			If (expression.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PredefinedType OrElse SyntaxFacts.IsInNamespaceOrTypeContext(expression)) Then
				flag = False
			ElseIf (SyntaxFactory.GetStandaloneExpression(expression) = expression) Then
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = expression.Kind()
				If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IdentifierLabel) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement)) Then
					flag = False
				Else
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CollectionInitializer) Then
						Dim parent As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = expression.Parent
						If (parent IsNot Nothing) Then
							Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = parent.Kind()
							If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectCollectionInitializer) Then
								If (DirectCast(parent, ObjectCollectionInitializerSyntax).Initializer <> expression) Then
									GoTo Label1
								End If
								flag = False
								Return flag
							ElseIf (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayCreationExpression) Then
								If (DirectCast(parent, ArrayCreationExpressionSyntax).Initializer <> expression) Then
									GoTo Label1
								End If
								flag = False
								Return flag
							ElseIf (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CollectionInitializer) Then
								parent = parent.Parent
								If (parent IsNot Nothing AndAlso parent.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CollectionInitializer) Then
									Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = parent
									parent = parent.Parent
									If (parent IsNot Nothing AndAlso parent.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectCollectionInitializer AndAlso DirectCast(parent, ObjectCollectionInitializerSyntax).Initializer = visualBasicSyntaxNode) Then
										GoTo Label1
									End If
								End If
								flag = False
								Return flag
							End If
						End If
					End If
				Label1:
					flag = If(Not SyntaxTreeSemanticModel.IsExpressionInValidContext(expression) OrElse SyntaxTreeSemanticModel.IsNodeInsideAttributeArguments(expression), False, True)
				End If
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function ValidateRegionDefiningStatementsRange(ByVal firstStatement As StatementSyntax, ByVal lastStatement As StatementSyntax) As Boolean
			Dim flag As Boolean
			Me.AssertNodeInTree(firstStatement, "firstStatement")
			Me.AssertNodeInTree(lastStatement, "lastStatement")
			If (firstStatement.Parent Is Nothing OrElse firstStatement.Parent <> lastStatement.Parent) Then
				Throw New ArgumentException("statements not within the same statement list")
			End If
			If (firstStatement.SpanStart > lastStatement.SpanStart) Then
				Throw New ArgumentException("first statement does not precede last statement")
			End If
			If (Not TypeOf firstStatement Is ExecutableStatementSyntax OrElse Not TypeOf lastStatement Is ExecutableStatementSyntax) Then
				flag = False
			ElseIf (Me.IsNotUppermostForBlock(firstStatement)) Then
				flag = False
			ElseIf (firstStatement = lastStatement OrElse Not Me.IsNotUppermostForBlock(lastStatement)) Then
				flag = If(SyntaxTreeSemanticModel.IsNodeInsideAttributeArguments(firstStatement) OrElse firstStatement <> lastStatement AndAlso SyntaxTreeSemanticModel.IsNodeInsideAttributeArguments(lastStatement), False, True)
			Else
				flag = False
			End If
			Return flag
		End Function
	End Class
End Namespace