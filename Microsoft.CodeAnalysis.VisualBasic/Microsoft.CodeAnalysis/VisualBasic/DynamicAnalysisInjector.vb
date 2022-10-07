Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class DynamicAnalysisInjector
		Inherits CompoundInstrumenter
		Private ReadOnly _method As MethodSymbol

		Private ReadOnly _methodBody As BoundStatement

		Private ReadOnly _createPayloadForMethodsSpanningSingleFile As MethodSymbol

		Private ReadOnly _createPayloadForMethodsSpanningMultipleFiles As MethodSymbol

		Private ReadOnly _spansBuilder As ArrayBuilder(Of SourceSpan)

		Private _dynamicAnalysisSpans As ImmutableArray(Of SourceSpan)

		Private ReadOnly _methodEntryInstrumentation As BoundStatement

		Private ReadOnly _payloadType As ArrayTypeSymbol

		Private ReadOnly _methodPayload As LocalSymbol

		Private ReadOnly _diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag

		Private ReadOnly _debugDocumentProvider As DebugDocumentProvider

		Private ReadOnly _methodBodyFactory As SyntheticBoundNodeFactory

		Public ReadOnly Property DynamicAnalysisSpans As ImmutableArray(Of SourceSpan)
			Get
				Return Me._dynamicAnalysisSpans
			End Get
		End Property

		Private Sub New(ByVal method As MethodSymbol, ByVal methodBody As BoundStatement, ByVal methodBodyFactory As SyntheticBoundNodeFactory, ByVal createPayloadForMethodsSpanningSingleFile As MethodSymbol, ByVal createPayloadForMethodsSpanningMultipleFiles As MethodSymbol, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal debugDocumentProvider As Microsoft.CodeAnalysis.CodeGen.DebugDocumentProvider, ByVal previous As Instrumenter)
			MyBase.New(previous)
			Me._dynamicAnalysisSpans = ImmutableArray(Of SourceSpan).Empty
			Me._createPayloadForMethodsSpanningSingleFile = createPayloadForMethodsSpanningSingleFile
			Me._createPayloadForMethodsSpanningMultipleFiles = createPayloadForMethodsSpanningMultipleFiles
			Me._method = method
			Me._methodBody = methodBody
			Me._spansBuilder = ArrayBuilder(Of SourceSpan).GetInstance()
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = methodBodyFactory.SpecialType(SpecialType.System_Boolean)
			Me._payloadType = ArrayTypeSymbol.CreateVBArray(typeSymbol, ImmutableArray(Of CustomModifier).Empty, 1, methodBodyFactory.Compilation.Assembly)
			Me._methodPayload = methodBodyFactory.SynthesizedLocal(Me._payloadType, SynthesizedLocalKind.InstrumentationPayload, methodBody.Syntax)
			Me._diagnostics = diagnostics
			Me._debugDocumentProvider = debugDocumentProvider
			Me._methodBodyFactory = methodBodyFactory
			Dim syntax As SyntaxNode = methodBody.Syntax
			If (Not method.IsImplicitlyDeclared) Then
				Me._methodEntryInstrumentation = Me.AddAnalysisPoint(syntax, DynamicAnalysisInjector.SkipAttributes(syntax), methodBodyFactory)
			End If
		End Sub

		Private Function AddAnalysisPoint(ByVal syntaxForSpan As SyntaxNode, ByVal alternateSpan As TextSpan, ByVal statementFactory As SyntheticBoundNodeFactory) As BoundStatement
			Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = syntaxForSpan.SyntaxTree
			Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
			Return Me.AddAnalysisPoint(syntaxForSpan, syntaxTree.GetMappedLineSpan(alternateSpan, cancellationToken), statementFactory)
		End Function

		Private Function AddAnalysisPoint(ByVal syntaxForSpan As SyntaxNode, ByVal statementFactory As SyntheticBoundNodeFactory) As BoundStatement
			Return Me.AddAnalysisPoint(syntaxForSpan, syntaxForSpan.GetLocation().GetMappedLineSpan(), statementFactory)
		End Function

		Private Function AddAnalysisPoint(ByVal syntaxForSpan As SyntaxNode, ByVal span As FileLinePositionSpan, ByVal statementFactory As SyntheticBoundNodeFactory) As BoundStatement
			Dim count As Integer = Me._spansBuilder.Count
			Dim sourceSpans As ArrayBuilder(Of SourceSpan) = Me._spansBuilder
			Dim sourceDocument As DebugSourceDocument = DynamicAnalysisInjector.GetSourceDocument(Me._debugDocumentProvider, syntaxForSpan, span)
			Dim startLinePosition As LinePosition = span.StartLinePosition
			Dim line As Integer = startLinePosition.Line
			startLinePosition = span.StartLinePosition
			Dim character As Integer = startLinePosition.Character
			startLinePosition = span.EndLinePosition
			Dim num As Integer = startLinePosition.Line
			startLinePosition = span.EndLinePosition
			sourceSpans.Add(New SourceSpan(sourceDocument, line, character, num, startLinePosition.Character))
			Dim boundArrayAccess As Microsoft.CodeAnalysis.VisualBasic.BoundArrayAccess = statementFactory.ArrayAccess(statementFactory.Local(Me._methodPayload, False), True, ImmutableArray.Create(Of BoundExpression)(statementFactory.Literal(count)))
			Return statementFactory.Assignment(boundArrayAccess, statementFactory.Literal(True))
		End Function

		Private Function AddDynamicAnalysis(ByVal original As Microsoft.CodeAnalysis.VisualBasic.BoundStatement, ByVal rewritten As Microsoft.CodeAnalysis.VisualBasic.BoundStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			boundStatement = If(original.WasCompilerGenerated, rewritten, Me.CollectDynamicAnalysis(original, rewritten))
			Return boundStatement
		End Function

		Private Function CollectDynamicAnalysis(ByVal original As Microsoft.CodeAnalysis.VisualBasic.BoundStatement, ByVal rewritten As Microsoft.CodeAnalysis.VisualBasic.BoundStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim syntheticBoundNodeFactory As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = New Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory(Me._methodBodyFactory.TopLevelMethod, Me._method, original.Syntax, Me._methodBodyFactory.CompilationState, Me._diagnostics)
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.AddAnalysisPoint(DynamicAnalysisInjector.SyntaxForSpan(original), syntheticBoundNodeFactory)
			If (rewritten Is Nothing) Then
				Return boundStatement
			End If
			Return syntheticBoundNodeFactory.StatementList(boundStatement, rewritten)
		End Function

		Public Overrides Function CreateBlockPrologue(ByVal trueOriginal As BoundBlock, ByVal original As BoundBlock, ByRef synthesizedLocal As LocalSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim boundStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = MyBase.CreateBlockPrologue(trueOriginal, original, synthesizedLocal)
			If (Me._methodBody <> trueOriginal) Then
				boundStatement = boundStatement1
			Else
				Me._dynamicAnalysisSpans = Me._spansBuilder.ToImmutableAndFree()
				Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol.CreateVBArray(Me._payloadType, ImmutableArray(Of CustomModifier).Empty, 1, Me._methodBodyFactory.Compilation.Assembly)
				Dim boundStatement2 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me._methodBodyFactory.Assignment(Me._methodBodyFactory.Local(Me._methodPayload, True), Me._methodBodyFactory.ArrayAccess(Me._methodBodyFactory.InstrumentationPayloadRoot(0, arrayTypeSymbol, False), False, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(Me._methodBodyFactory.MethodDefIndex(Me._method))))
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me._methodBodyFactory.ModuleVersionId(False)
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me._methodBodyFactory.MethodDefIndex(Me._method)
				Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me._methodBodyFactory.ArrayAccess(Me._methodBodyFactory.InstrumentationPayloadRoot(0, arrayTypeSymbol, False), True, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(Me._methodBodyFactory.MethodDefIndex(Me._method)))
				Dim createPayloadStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = DynamicAnalysisInjector.GetCreatePayloadStatement(Me._dynamicAnalysisSpans, Me._methodBody.Syntax, Me._methodPayload, Me._createPayloadForMethodsSpanningSingleFile, Me._createPayloadForMethodsSpanningMultipleFiles, boundExpression, boundExpression1, boundExpression2, Me._methodBodyFactory, Me._debugDocumentProvider)
				Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me._methodBodyFactory.Binary(BinaryOperatorKind.Equals, Me._methodBodyFactory.SpecialType(SpecialType.System_Boolean), Me._methodBodyFactory.Local(Me._methodPayload, False), Me._methodBodyFactory.Null(Me._payloadType))
				Dim boundStatement3 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me._methodBodyFactory.[If](boundExpression3, createPayloadStatement)
				synthesizedLocal = Me._methodPayload
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement).GetInstance(If(boundStatement1 Is Nothing, 3, 4))
				instance.Add(boundStatement2)
				instance.Add(boundStatement3)
				If (Me._methodEntryInstrumentation IsNot Nothing) Then
					instance.Add(Me._methodEntryInstrumentation)
				End If
				If (boundStatement1 IsNot Nothing) Then
					instance.Add(boundStatement1)
				End If
				boundStatement = Me._methodBodyFactory.StatementList(instance.ToImmutableAndFree())
			End If
			Return boundStatement
		End Function

		Public Overrides Function CreateSelectStatementPrologue(ByVal original As BoundSelectStatement) As BoundStatement
			Return Me.AddDynamicAnalysis(original, MyBase.CreateSelectStatementPrologue(original))
		End Function

		Public Overrides Function CreateUsingStatementPrologue(ByVal original As BoundUsingStatement) As BoundStatement
			Return Me.AddDynamicAnalysis(original, MyBase.CreateUsingStatementPrologue(original))
		End Function

		Public Overrides Function CreateWithStatementPrologue(ByVal original As BoundWithStatement) As BoundStatement
			Return Me.AddDynamicAnalysis(original, MyBase.CreateWithStatementPrologue(original))
		End Function

		Private Shared Function GetCreatePayloadOverload(ByVal compilation As VisualBasicCompilation, ByVal overload As WellKnownMember, ByVal syntax As SyntaxNode, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As MethodSymbol
			Return DirectCast(Binder.GetWellKnownTypeMember(compilation, overload, syntax, diagnostics), MethodSymbol)
		End Function

		Private Shared Function GetCreatePayloadStatement(ByVal dynamicAnalysisSpans As ImmutableArray(Of SourceSpan), ByVal methodBodySyntax As SyntaxNode, ByVal methodPayload As LocalSymbol, ByVal createPayloadForMethodsSpanningSingleFile As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal createPayloadForMethodsSpanningMultipleFiles As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal mvid As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal methodToken As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal payloadSlot As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal methodBodyFactory As SyntheticBoundNodeFactory, ByVal debugDocumentProvider As Microsoft.CodeAnalysis.CodeGen.DebugDocumentProvider) As BoundExpressionStatement
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (Not dynamicAnalysisSpans.IsEmpty) Then
				Dim instance As PooledHashSet(Of DebugSourceDocument) = PooledHashSet(Of DebugSourceDocument).GetInstance()
				Dim boundExpressions As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).GetInstance()
				Dim enumerator As ImmutableArray(Of SourceSpan).Enumerator = dynamicAnalysisSpans.GetEnumerator()
				While enumerator.MoveNext()
					Dim document As DebugSourceDocument = enumerator.Current.Document
					If (Not instance.Add(document)) Then
						Continue While
					End If
					boundExpressions.Add(methodBodyFactory.SourceDocumentIndex(document))
				End While
				instance.Free()
				If (boundExpressions.Count <> 1) Then
					methodSymbol = createPayloadForMethodsSpanningMultipleFiles
					boundExpression = methodBodyFactory.Array(methodBodyFactory.SpecialType(SpecialType.System_Int32), boundExpressions.ToImmutable())
				Else
					methodSymbol = createPayloadForMethodsSpanningSingleFile
					boundExpression = boundExpressions.[Single]()
				End If
				boundExpressions.Free()
			Else
				methodSymbol = createPayloadForMethodsSpanningSingleFile
				boundExpression = methodBodyFactory.SourceDocumentIndex(DynamicAnalysisInjector.GetSourceDocument(debugDocumentProvider, methodBodySyntax))
			End If
			Return methodBodyFactory.Assignment(methodBodyFactory.Local(methodPayload, True), methodBodyFactory.[Call](Nothing, methodSymbol, New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { mvid, methodToken, boundExpression, payloadSlot, methodBodyFactory.Literal(dynamicAnalysisSpans.Length) }))
		End Function

		Private Shared Function GetSourceDocument(ByVal debugDocumentProvider As Microsoft.CodeAnalysis.CodeGen.DebugDocumentProvider, ByVal syntax As SyntaxNode) As DebugSourceDocument
			Return DynamicAnalysisInjector.GetSourceDocument(debugDocumentProvider, syntax, syntax.GetLocation().GetMappedLineSpan())
		End Function

		Private Shared Function GetSourceDocument(ByVal debugDocumentProvider As Microsoft.CodeAnalysis.CodeGen.DebugDocumentProvider, ByVal syntax As SyntaxNode, ByVal span As FileLinePositionSpan) As DebugSourceDocument
			Dim path As String = span.Path
			If (path.Length = 0) Then
				path = syntax.SyntaxTree.FilePath
			End If
			Return debugDocumentProvider(path, "")
		End Function

		Private Shared Function HasValidMappedLineSpan(ByVal syntax As SyntaxNode) As Boolean
			Return syntax.GetLocation().GetMappedLineSpan().IsValid
		End Function

		Public Overrides Function InstrumentAddHandlerStatement(ByVal original As BoundAddHandlerStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.AddDynamicAnalysis(original, MyBase.InstrumentAddHandlerStatement(original, rewritten))
		End Function

		Public Overrides Function InstrumentContinueStatement(ByVal original As BoundContinueStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.AddDynamicAnalysis(original, MyBase.InstrumentContinueStatement(original, rewritten))
		End Function

		Public Overrides Function InstrumentDoLoopStatementEntryOrConditionalGotoStart(ByVal original As BoundDoLoopStatement, ByVal ifConditionGotoStartOpt As Microsoft.CodeAnalysis.VisualBasic.BoundStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim boundStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = MyBase.InstrumentDoLoopStatementEntryOrConditionalGotoStart(original, ifConditionGotoStartOpt)
			boundStatement = If(original.ConditionOpt Is Nothing, boundStatement1, Me.AddDynamicAnalysis(original, boundStatement1))
			Return boundStatement
		End Function

		Public Overrides Function InstrumentEndStatement(ByVal original As BoundEndStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.AddDynamicAnalysis(original, MyBase.InstrumentEndStatement(original, rewritten))
		End Function

		Public Overrides Function InstrumentExitStatement(ByVal original As BoundExitStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.AddDynamicAnalysis(original, MyBase.InstrumentExitStatement(original, rewritten))
		End Function

		Public Overrides Function InstrumentExpressionStatement(ByVal original As BoundExpressionStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.AddDynamicAnalysis(original, MyBase.InstrumentExpressionStatement(original, rewritten))
		End Function

		Public Overrides Function InstrumentFieldOrPropertyInitializer(ByVal original As BoundFieldOrPropertyInitializer, ByVal rewritten As BoundStatement, ByVal symbolIndex As Integer, ByVal createTemporary As Boolean) As BoundStatement
			Return Me.AddDynamicAnalysis(original, MyBase.InstrumentFieldOrPropertyInitializer(original, rewritten, symbolIndex, createTemporary))
		End Function

		Public Overrides Function InstrumentForEachLoopInitialization(ByVal original As BoundForEachStatement, ByVal initialization As BoundStatement) As BoundStatement
			Return Me.AddDynamicAnalysis(original, MyBase.InstrumentForEachLoopInitialization(original, initialization))
		End Function

		Public Overrides Function InstrumentForLoopInitialization(ByVal original As BoundForToStatement, ByVal initialization As BoundStatement) As BoundStatement
			Return Me.AddDynamicAnalysis(original, MyBase.InstrumentForLoopInitialization(original, initialization))
		End Function

		Public Overrides Function InstrumentGotoStatement(ByVal original As BoundGotoStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.AddDynamicAnalysis(original, MyBase.InstrumentGotoStatement(original, rewritten))
		End Function

		Public Overrides Function InstrumentIfStatementConditionalGoto(ByVal original As BoundIfStatement, ByVal condGoto As BoundStatement) As BoundStatement
			Return Me.AddDynamicAnalysis(original, MyBase.InstrumentIfStatementConditionalGoto(original, condGoto))
		End Function

		Public Overrides Function InstrumentLocalInitialization(ByVal original As BoundLocalDeclaration, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.AddDynamicAnalysis(original, MyBase.InstrumentLocalInitialization(original, rewritten))
		End Function

		Public Overrides Function InstrumentOnErrorStatement(ByVal original As BoundOnErrorStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.AddDynamicAnalysis(original, MyBase.InstrumentOnErrorStatement(original, rewritten))
		End Function

		Public Overrides Function InstrumentRaiseEventStatement(ByVal original As BoundRaiseEventStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.AddDynamicAnalysis(original, MyBase.InstrumentRaiseEventStatement(original, rewritten))
		End Function

		Public Overrides Function InstrumentRemoveHandlerStatement(ByVal original As BoundRemoveHandlerStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.AddDynamicAnalysis(original, MyBase.InstrumentRemoveHandlerStatement(original, rewritten))
		End Function

		Public Overrides Function InstrumentResumeStatement(ByVal original As BoundResumeStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.AddDynamicAnalysis(original, MyBase.InstrumentResumeStatement(original, rewritten))
		End Function

		Public Overrides Function InstrumentReturnStatement(ByVal original As BoundReturnStatement, ByVal rewritten As Microsoft.CodeAnalysis.VisualBasic.BoundStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim boundStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = MyBase.InstrumentReturnStatement(original, rewritten)
			If (original.IsEndOfMethodReturn()) Then
				boundStatement = boundStatement1
			Else
				boundStatement = If(original.ExpressionOpt Is Nothing, Me.AddDynamicAnalysis(original, boundStatement1), Me.CollectDynamicAnalysis(original, boundStatement1))
			End If
			Return boundStatement
		End Function

		Public Overrides Function InstrumentStopStatement(ByVal original As BoundStopStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.AddDynamicAnalysis(original, MyBase.InstrumentStopStatement(original, rewritten))
		End Function

		Public Overrides Function InstrumentSyncLockObjectCapture(ByVal original As BoundSyncLockStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.AddDynamicAnalysis(original, MyBase.InstrumentSyncLockObjectCapture(original, rewritten))
		End Function

		Public Overrides Function InstrumentThrowStatement(ByVal original As BoundThrowStatement, ByVal rewritten As BoundStatement) As BoundStatement
			Return Me.AddDynamicAnalysis(original, MyBase.InstrumentThrowStatement(original, rewritten))
		End Function

		Public Overrides Function InstrumentWhileStatementConditionalGotoStart(ByVal original As BoundWhileStatement, ByVal ifConditionGotoStart As BoundStatement) As BoundStatement
			Return Me.AddDynamicAnalysis(original, MyBase.InstrumentWhileStatementConditionalGotoStart(original, ifConditionGotoStart))
		End Function

		Private Shared Function IsExcludedFromCodeCoverage(ByVal method As MethodSymbol) As Boolean
			Dim flag As Boolean
			Dim nullable As Nullable(Of Integer)
			Dim nullable1 As Nullable(Of Boolean)
			Dim nullable2 As Nullable(Of SymbolKind)
			Dim nullable3 As Nullable(Of Integer)
			Dim nullable4 As Nullable(Of Boolean)
			Dim nullable5 As Nullable(Of Integer)
			Dim nullable6 As Nullable(Of Boolean)
			Dim containingType As NamedTypeSymbol = method.ContainingType
			While True
				If (containingType Is Nothing) Then
					Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = method.ContainingNonLambdaMember()
					If (If(symbol IsNot Nothing, symbol.Kind = SymbolKind.Method, False)) Then
						method = DirectCast(symbol, MethodSymbol)
						If (Not method.IsDirectlyExcludedFromCodeCoverage) Then
							Dim associatedSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = method.AssociatedSymbol
							If (associatedSymbol IsNot Nothing) Then
								nullable2 = New Nullable(Of SymbolKind)(associatedSymbol.Kind)
							Else
								nullable2 = Nothing
							End If
							Dim nullable7 As Nullable(Of SymbolKind) = nullable2
							If (nullable7.HasValue) Then
								nullable3 = New Nullable(Of Integer)(nullable7.GetValueOrDefault())
							Else
								nullable = Nothing
								nullable3 = nullable
							End If
							Dim nullable8 As Nullable(Of Integer) = nullable3
							If (nullable8.HasValue) Then
								nullable4 = New Nullable(Of Boolean)(nullable8.GetValueOrDefault() = 15)
							Else
								nullable1 = Nothing
								nullable4 = nullable1
							End If
							nullable1 = nullable4
							If (Not nullable1.GetValueOrDefault()) Then
								If (nullable7.HasValue) Then
									nullable5 = New Nullable(Of Integer)(nullable7.GetValueOrDefault())
								Else
									nullable = Nothing
									nullable5 = nullable
								End If
								nullable8 = nullable5
								If (nullable8.HasValue) Then
									nullable6 = New Nullable(Of Boolean)(nullable8.GetValueOrDefault() = 5)
								Else
									nullable1 = Nothing
									nullable6 = nullable1
								End If
								nullable1 = nullable6
								If (nullable1.GetValueOrDefault() AndAlso DirectCast(associatedSymbol, EventSymbol).IsDirectlyExcludedFromCodeCoverage) Then
									flag = True
									Exit While
								End If
							ElseIf (DirectCast(associatedSymbol, PropertySymbol).IsDirectlyExcludedFromCodeCoverage) Then
								flag = True
								Exit While
							End If
						Else
							flag = True
							Exit While
						End If
					End If
					flag = False
					Exit While
				ElseIf (Not containingType.IsDirectlyExcludedFromCodeCoverage) Then
					containingType = containingType.ContainingType
				Else
					flag = True
					Exit While
				End If
			End While
			Return flag
		End Function

		Private Shared Function SkipAttributes(ByVal syntax As SyntaxNode) As TextSpan
			Dim span As TextSpan
			Select Case syntax.Kind()
				Case SyntaxKind.SubBlock
				Case SyntaxKind.FunctionBlock
					Dim subOrFunctionStatement As MethodStatementSyntax = DirectCast(syntax, MethodBlockSyntax).SubOrFunctionStatement
					span = DynamicAnalysisInjector.SkipAttributes(syntax, subOrFunctionStatement.AttributeLists, subOrFunctionStatement.Modifiers, subOrFunctionStatement.SubOrFunctionKeyword)
					Exit Select
				Case SyntaxKind.ConstructorBlock
					Dim subNewStatement As SubNewStatementSyntax = DirectCast(syntax, ConstructorBlockSyntax).SubNewStatement
					span = DynamicAnalysisInjector.SkipAttributes(syntax, subNewStatement.AttributeLists, subNewStatement.Modifiers, subNewStatement.SubKeyword)
					Exit Select
				Case SyntaxKind.OperatorBlock
					Dim operatorStatement As OperatorStatementSyntax = DirectCast(syntax, OperatorBlockSyntax).OperatorStatement
					span = DynamicAnalysisInjector.SkipAttributes(syntax, operatorStatement.AttributeLists, operatorStatement.Modifiers, operatorStatement.OperatorKeyword)
					Exit Select
				Case SyntaxKind.GetAccessorBlock
				Case SyntaxKind.SetAccessorBlock
					Dim accessorStatement As AccessorStatementSyntax = DirectCast(syntax, AccessorBlockSyntax).AccessorStatement
					span = DynamicAnalysisInjector.SkipAttributes(syntax, accessorStatement.AttributeLists, accessorStatement.Modifiers, accessorStatement.AccessorKeyword)
					Exit Select
				Case SyntaxKind.AddHandlerAccessorBlock
				Case SyntaxKind.RemoveHandlerAccessorBlock
				Case SyntaxKind.RaiseEventAccessorBlock
				Label0:
					span = syntax.Span
					Exit Select
				Case SyntaxKind.PropertyBlock
					Dim propertyStatement As PropertyStatementSyntax = DirectCast(syntax, PropertyBlockSyntax).PropertyStatement
					span = DynamicAnalysisInjector.SkipAttributes(syntax, propertyStatement.AttributeLists, propertyStatement.Modifiers, propertyStatement.PropertyKeyword)
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return span
		End Function

		Private Shared Function SkipAttributes(ByVal syntax As SyntaxNode, ByVal attributes As SyntaxList(Of AttributeListSyntax), ByVal modifiers As SyntaxTokenList, ByVal keyword As SyntaxToken) As Microsoft.CodeAnalysis.Text.TextSpan
			Dim textSpan As Microsoft.CodeAnalysis.Text.TextSpan
			Dim span As Microsoft.CodeAnalysis.Text.TextSpan = syntax.Span
			If (attributes.Count <= 0) Then
				textSpan = span
			Else
				Dim textSpan1 As Microsoft.CodeAnalysis.Text.TextSpan = If(modifiers.Node IsNot Nothing, modifiers.Span, keyword.Span)
				textSpan = New Microsoft.CodeAnalysis.Text.TextSpan(textSpan1.Start, span.Length - (textSpan1.Start - span.Start))
			End If
			Return textSpan
		End Function

		Private Shared Function SyntaxForSpan(ByVal statement As BoundStatement) As SyntaxNode
			Dim syntax As SyntaxNode
			Dim kind As BoundKind = statement.Kind
			If (kind <= BoundKind.IfStatement) Then
				If (kind = BoundKind.LocalDeclaration) Then
					Dim initializerOpt As BoundExpression = DirectCast(statement, BoundLocalDeclaration).InitializerOpt
					If (initializerOpt Is Nothing) Then
						syntax = statement.Syntax
						Return syntax
					End If
					syntax = initializerOpt.Syntax
				ElseIf (CByte(kind) - CByte(BoundKind.FieldInitializer) <= CByte(BoundKind.OmittedArgument)) Then
					Dim equalsValueSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax = TryCast(statement.Syntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax)
					If (equalsValueSyntax Is Nothing) Then
						Dim asNewClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsNewClauseSyntax = TryCast(statement.Syntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.AsNewClauseSyntax)
						If (asNewClauseSyntax Is Nothing) Then
							syntax = statement.Syntax
							Return syntax
						End If
						syntax = asNewClauseSyntax._newExpression
					Else
						syntax = equalsValueSyntax.Value
					End If
				Else
					If (kind <> BoundKind.IfStatement) Then
						syntax = statement.Syntax
						Return syntax
					End If
					syntax = DirectCast(statement, BoundIfStatement).Condition.Syntax
				End If
			ElseIf (kind <= BoundKind.ForEachStatement) Then
				If (kind = BoundKind.SelectStatement) Then
					syntax = DirectCast(statement, BoundSelectStatement).ExpressionStatement.Expression.Syntax
				Else
					Select Case kind
						Case BoundKind.DoLoopStatement
							syntax = DirectCast(statement, BoundDoLoopStatement).ConditionOpt.Syntax
							Exit Select
						Case BoundKind.WhileStatement
							syntax = DirectCast(statement, BoundWhileStatement).Condition.Syntax
							Exit Select
						Case BoundKind.ForToUserDefinedOperators
							syntax = statement.Syntax
							Return syntax
						Case BoundKind.ForToStatement
							syntax = DirectCast(statement, BoundForToStatement).InitialValue.Syntax
							Exit Select
						Case BoundKind.ForEachStatement
							syntax = DirectCast(statement, BoundForEachStatement).Collection.Syntax
							Exit Select
						Case Else
							syntax = statement.Syntax
							Return syntax
					End Select
				End If
			ElseIf (kind = BoundKind.UsingStatement) Then
				Dim boundUsingStatement As Microsoft.CodeAnalysis.VisualBasic.BoundUsingStatement = DirectCast(statement, Microsoft.CodeAnalysis.VisualBasic.BoundUsingStatement)
				Dim resourceExpressionOpt As BoundNode = boundUsingStatement.ResourceExpressionOpt
				If (resourceExpressionOpt Is Nothing) Then
					resourceExpressionOpt = boundUsingStatement
				End If
				syntax = resourceExpressionOpt.Syntax
			Else
				If (kind <> BoundKind.SyncLockStatement) Then
					syntax = statement.Syntax
					Return syntax
				End If
				syntax = DirectCast(statement, BoundSyncLockStatement).LockExpression.Syntax
			End If
			Return syntax
		End Function

		Public Shared Function TryCreate(ByVal method As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal methodBody As BoundStatement, ByVal methodBodyFactory As SyntheticBoundNodeFactory, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal debugDocumentProvider As Microsoft.CodeAnalysis.CodeGen.DebugDocumentProvider, ByVal previous As Instrumenter) As Microsoft.CodeAnalysis.VisualBasic.DynamicAnalysisInjector
			Dim dynamicAnalysisInjector As Microsoft.CodeAnalysis.VisualBasic.DynamicAnalysisInjector
			If (method.IsImplicitlyDeclared AndAlso Not method.IsAnyConstructor()) Then
				dynamicAnalysisInjector = Nothing
			ElseIf (Not Microsoft.CodeAnalysis.VisualBasic.DynamicAnalysisInjector.HasValidMappedLineSpan(methodBody.Syntax)) Then
				dynamicAnalysisInjector = Nothing
			ElseIf (Not Microsoft.CodeAnalysis.VisualBasic.DynamicAnalysisInjector.IsExcludedFromCodeCoverage(method)) Then
				Dim createPayloadOverload As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Microsoft.CodeAnalysis.VisualBasic.DynamicAnalysisInjector.GetCreatePayloadOverload(methodBodyFactory.Compilation, WellKnownMember.Microsoft_CodeAnalysis_Runtime_Instrumentation__CreatePayloadForMethodsSpanningSingleFile, methodBody.Syntax, diagnostics)
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Microsoft.CodeAnalysis.VisualBasic.DynamicAnalysisInjector.GetCreatePayloadOverload(methodBodyFactory.Compilation, WellKnownMember.Microsoft_CodeAnalysis_Runtime_Instrumentation__CreatePayloadForMethodsSpanningMultipleFiles, methodBody.Syntax, diagnostics)
				If (createPayloadOverload Is Nothing OrElse methodSymbol Is Nothing) Then
					dynamicAnalysisInjector = Nothing
				ElseIf (method.Equals(createPayloadOverload) OrElse method.Equals(methodSymbol)) Then
					dynamicAnalysisInjector = Nothing
				Else
					dynamicAnalysisInjector = New Microsoft.CodeAnalysis.VisualBasic.DynamicAnalysisInjector(method, methodBody, methodBodyFactory, createPayloadOverload, methodSymbol, diagnostics, debugDocumentProvider, previous)
				End If
			Else
				dynamicAnalysisInjector = Nothing
			End If
			Return dynamicAnalysisInjector
		End Function
	End Class
End Namespace