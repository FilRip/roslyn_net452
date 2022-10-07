Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class LambdaRewriter
		Inherits MethodToClassRewriter(Of FieldSymbol)
		Private ReadOnly _analysis As LambdaRewriter.Analysis

		Private ReadOnly _topLevelMethod As MethodSymbol

		Private ReadOnly _topLevelMethodOrdinal As Integer

		Private _lazyStaticLambdaFrame As LambdaFrame

		Private ReadOnly _frames As Dictionary(Of BoundNode, LambdaFrame)

		Private ReadOnly _framePointers As Dictionary(Of NamedTypeSymbol, Symbol)

		Private _currentMethod As MethodSymbol

		Private _currentFrameThis As ParameterSymbol

		Private ReadOnly _lambdaDebugInfoBuilder As ArrayBuilder(Of LambdaDebugInfo)

		Private _delegateRelaxationIdDispenser As Integer

		Private _synthesizedFieldNameIdDispenser As Integer

		Private _innermostFramePointer As Symbol

		Private _currentLambdaBodyTypeSubstitution As TypeSubstitution

		Private _currentTypeParameters As ImmutableArray(Of TypeParameterSymbol)

		Private _meProxyDeferredInit As BoundExpression

		Private _meIsInitialized As Boolean

		Private _meProxyDeferredInitDone As Boolean

		Private _inExpressionLambda As Boolean

		Private _reported_ERR_CannotUseOnErrorGotoWithClosure As Boolean

		Private _rewrittenNodes As HashSet(Of BoundNode)

		Protected Overrides ReadOnly Property CurrentMethod As MethodSymbol
			Get
				Return Me._currentMethod
			End Get
		End Property

		Protected Overrides ReadOnly Property IsInExpressionLambda As Boolean
			Get
				Return Me._inExpressionLambda
			End Get
		End Property

		Protected Overrides ReadOnly Property TopLevelMethod As MethodSymbol
			Get
				Return Me._topLevelMethod
			End Get
		End Property

		Protected Overrides ReadOnly Property TypeMap As TypeSubstitution
			Get
				Return Me._currentLambdaBodyTypeSubstitution
			End Get
		End Property

		Private Sub New(ByVal analysis As LambdaRewriter.Analysis, ByVal method As MethodSymbol, ByVal methodOrdinal As Integer, ByVal lambdaDebugInfoBuilder As ArrayBuilder(Of LambdaDebugInfo), ByVal delegateRelaxationIdDispenser As Integer, ByVal slotAllocatorOpt As VariableSlotAllocator, ByVal compilationState As TypeCompilationState, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			MyBase.New(slotAllocatorOpt, compilationState, diagnostics, method.PreserveOriginalLocals)
			Me._frames = New Dictionary(Of BoundNode, LambdaFrame)()
			Me._framePointers = New Dictionary(Of NamedTypeSymbol, Symbol)()
			Me._rewrittenNodes = Nothing
			Me._topLevelMethod = method
			Me._topLevelMethodOrdinal = methodOrdinal
			Me._lambdaDebugInfoBuilder = lambdaDebugInfoBuilder
			Me._delegateRelaxationIdDispenser = delegateRelaxationIdDispenser
			Me._currentMethod = method
			Me._analysis = analysis
			Me._currentTypeParameters = Me._topLevelMethod.TypeParameters
			Me._inExpressionLambda = False
			If (Not method.IsShared) Then
				Me._innermostFramePointer = method.MeParameter
				Me._framePointers(method.ContainingType) = method.MeParameter
			End If
			Me._currentFrameThis = method.MeParameter
			Me._synthesizedFieldNameIdDispenser = 1
		End Sub

		Friend Shared Function ConstructFrameType(Of T As TypeSymbol)(ByVal type As LambdaFrame, ByVal typeArguments As ImmutableArray(Of T)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			If (Not type.CanConstruct) Then
				namedTypeSymbol = type
			Else
				namedTypeSymbol = type.Construct(StaticCast(Of TypeSymbol).From(Of T)(typeArguments))
			End If
			Return namedTypeSymbol
		End Function

		Private Function FrameOfType(ByVal syntax As SyntaxNode, ByVal frameType As NamedTypeSymbol) As BoundExpression
			Return Me.FramePointer(syntax, frameType.OriginalDefinition)
		End Function

		Friend Overrides Function FramePointer(ByVal syntax As SyntaxNode, ByVal frameClass As NamedTypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (Me._currentFrameThis Is Nothing OrElse Not TypeSymbol.Equals(Me._currentFrameThis.Type, frameClass, TypeCompareKind.ConsiderEverything)) Then
				Dim item As Symbol = Me._framePointers(frameClass)
				Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = Nothing
				If (Not Me.Proxies.TryGetValue(item, fieldSymbol)) Then
					Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = DirectCast(item, Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)
					boundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(syntax, localSymbol, False, localSymbol.Type)
				Else
					Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.FramePointer(syntax, fieldSymbol.ContainingType)
					Dim fieldSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = fieldSymbol.AsMember(DirectCast(boundExpression.Type, NamedTypeSymbol))
					boundLocal = New BoundFieldAccess(syntax, boundExpression, fieldSymbol1, False, fieldSymbol1.Type, False)
				End If
			Else
				boundLocal = New BoundMeReference(syntax, frameClass)
			End If
			Return boundLocal
		End Function

		Private Function GetClosureId(ByVal syntax As SyntaxNode, ByVal closureDebugInfo As ArrayBuilder(Of Microsoft.CodeAnalysis.CodeGen.ClosureDebugInfo)) As Microsoft.CodeAnalysis.CodeGen.DebugId
			Dim debugId As Microsoft.CodeAnalysis.CodeGen.DebugId
			Dim debugId1 As Microsoft.CodeAnalysis.CodeGen.DebugId
			debugId = If(Me.SlotAllocatorOpt Is Nothing OrElse Not Me.SlotAllocatorOpt.TryGetPreviousClosure(syntax, debugId1), New Microsoft.CodeAnalysis.CodeGen.DebugId(closureDebugInfo.Count, Me.CompilationState.ModuleBuilderOpt.CurrentGenerationOrdinal), debugId1)
			Dim num As Integer = Me._topLevelMethod.CalculateLocalSyntaxOffset(syntax.SpanStart, syntax.SyntaxTree)
			closureDebugInfo.Add(New Microsoft.CodeAnalysis.CodeGen.ClosureDebugInfo(num, debugId))
			Return debugId
		End Function

		Private Function GetFrameForScope(ByVal copyConstructor As Boolean, ByVal captured As Symbol, ByVal scope As BoundNode, ByVal closureDebugInfo As ArrayBuilder(Of Microsoft.CodeAnalysis.CodeGen.ClosureDebugInfo), ByRef delegateRelaxationIdDispenser As Integer) As Microsoft.CodeAnalysis.VisualBasic.LambdaFrame
			Dim topLevelMethodId As DebugId
			Dim closureId As DebugId
			Dim synthesizedKind As Boolean
			Dim lambdaFrame As Microsoft.CodeAnalysis.VisualBasic.LambdaFrame = Nothing
			If (Not Me._frames.TryGetValue(scope, lambdaFrame)) Then
				Dim syntax As SyntaxNode = scope.Syntax
				Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = TryCast(captured, Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal)
				If (synthesizedLocal IsNot Nothing) Then
					synthesizedKind = synthesizedLocal.SynthesizedKind = SynthesizedLocalKind.DelegateRelaxationReceiver
				Else
					synthesizedKind = False
				End If
				Dim flag As Boolean = synthesizedKind
				If (Not flag) Then
					topLevelMethodId = Me.GetTopLevelMethodId()
					closureId = Me.GetClosureId(syntax, closureDebugInfo)
				Else
					Dim currentGenerationOrdinal As Integer = Me.CompilationState.ModuleBuilderOpt.CurrentGenerationOrdinal
					topLevelMethodId = New DebugId(Me._topLevelMethodOrdinal, currentGenerationOrdinal)
					closureId = New DebugId(delegateRelaxationIdDispenser, currentGenerationOrdinal)
					delegateRelaxationIdDispenser = delegateRelaxationIdDispenser + 1
				End If
				lambdaFrame = New Microsoft.CodeAnalysis.VisualBasic.LambdaFrame(Me._topLevelMethod, syntax, topLevelMethodId, closureId, If(Not copyConstructor, False, Not Me._analysis.symbolsCapturedWithoutCopyCtor.Contains(captured)), False, flag)
				Me._frames(scope) = lambdaFrame
				Me.CompilationState.ModuleBuilderOpt.AddSynthesizedDefinition(Me._topLevelMethod.ContainingType, lambdaFrame.GetCciAdapter())
			End If
			Return lambdaFrame
		End Function

		Private Function GetLambdaId(ByVal syntax As SyntaxNode, ByVal closureKind As Microsoft.CodeAnalysis.VisualBasic.ClosureKind, ByVal closureOrdinal As Integer) As Microsoft.CodeAnalysis.CodeGen.DebugId
			Dim lambdaExpressionLambdaBody As SyntaxNode
			Dim flag As Boolean
			Dim debugId As Microsoft.CodeAnalysis.CodeGen.DebugId
			Dim debugId1 As Microsoft.CodeAnalysis.CodeGen.DebugId
			Dim lambdaExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaExpressionSyntax = TryCast(syntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaExpressionSyntax)
			If (lambdaExpressionSyntax IsNot Nothing) Then
				lambdaExpressionLambdaBody = LambdaUtilities.GetLambdaExpressionLambdaBody(lambdaExpressionSyntax)
				flag = True
			ElseIf (syntax.IsKind(SyntaxKind.AddressOfExpression)) Then
				lambdaExpressionLambdaBody = syntax
				flag = False
			ElseIf (Not LambdaUtilities.IsNonUserCodeQueryLambda(syntax)) Then
				lambdaExpressionLambdaBody = syntax
				flag = True
			Else
				lambdaExpressionLambdaBody = syntax
				flag = False
			End If
			debugId = If(Me.SlotAllocatorOpt Is Nothing OrElse Not Me.SlotAllocatorOpt.TryGetPreviousLambda(lambdaExpressionLambdaBody, flag, debugId1), New Microsoft.CodeAnalysis.CodeGen.DebugId(Me._lambdaDebugInfoBuilder.Count, Me.CompilationState.ModuleBuilderOpt.CurrentGenerationOrdinal), debugId1)
			Dim num As Integer = Me._topLevelMethod.CalculateLocalSyntaxOffset(lambdaExpressionLambdaBody.SpanStart, lambdaExpressionLambdaBody.SyntaxTree)
			Me._lambdaDebugInfoBuilder.Add(New LambdaDebugInfo(num, debugId, closureOrdinal))
			Return debugId
		End Function

		Private Function GetStaticFrame(ByVal lambda As BoundNode, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.LambdaFrame
			Dim debugId As Microsoft.CodeAnalysis.CodeGen.DebugId
			If (Me._lazyStaticLambdaFrame Is Nothing) Then
				Dim isGenericMethod As Boolean = Not Me.TopLevelMethod.IsGenericMethod
				If (isGenericMethod) Then
					Me._lazyStaticLambdaFrame = Me.CompilationState.staticLambdaFrame
				End If
				If (Me._lazyStaticLambdaFrame Is Nothing) Then
					debugId = If(Not isGenericMethod, Me.GetTopLevelMethodId(), New Microsoft.CodeAnalysis.CodeGen.DebugId(-1, Me.CompilationState.ModuleBuilderOpt.CurrentGenerationOrdinal))
					Dim debugId1 As Microsoft.CodeAnalysis.CodeGen.DebugId = New Microsoft.CodeAnalysis.CodeGen.DebugId()
					Me._lazyStaticLambdaFrame = New Microsoft.CodeAnalysis.VisualBasic.LambdaFrame(Me._topLevelMethod, lambda.Syntax, debugId, debugId1, False, True, False)
					If (isGenericMethod) Then
						Me.CompilationState.staticLambdaFrame = Me._lazyStaticLambdaFrame
					End If
					Dim lambdaFrame As Microsoft.CodeAnalysis.VisualBasic.LambdaFrame = Me._lazyStaticLambdaFrame
					Me.CompilationState.ModuleBuilderOpt.AddSynthesizedDefinition(Me._topLevelMethod.ContainingType, lambdaFrame.GetCciAdapter())
					Dim syntax As SyntaxNode = lambda.Syntax
					Me.CompilationState.AddSynthesizedMethod(lambdaFrame.Constructor, Me.MakeFrameCtor(lambdaFrame, diagnostics))
					Dim syntheticBoundNodeFactory As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = New Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory(lambdaFrame.SharedConstructor, lambdaFrame.SharedConstructor, syntax, Me.CompilationState, diagnostics)
					Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = syntheticBoundNodeFactory.Block(New BoundStatement() { syntheticBoundNodeFactory.Assignment(syntheticBoundNodeFactory.Field(Nothing, lambdaFrame.SingletonCache, True), syntheticBoundNodeFactory.[New](lambdaFrame.Constructor)), syntheticBoundNodeFactory.[Return](Nothing) })
					Me.CompilationState.AddSynthesizedMethod(lambdaFrame.SharedConstructor, boundBlock)
				End If
			End If
			Return Me._lazyStaticLambdaFrame
		End Function

		Private Function GetTopLevelMethodId() As DebugId
			Dim nullable As Nullable(Of DebugId)
			Dim methodId As Nullable(Of DebugId)
			Dim slotAllocatorOpt As VariableSlotAllocator = Me.SlotAllocatorOpt
			If (slotAllocatorOpt IsNot Nothing) Then
				methodId = slotAllocatorOpt.MethodId
			Else
				nullable = Nothing
				methodId = nullable
			End If
			Dim nullable1 As Nullable(Of DebugId) = methodId
			nullable = methodId
			If (nullable.HasValue) Then
				Return nullable1.GetValueOrDefault()
			End If
			Return New DebugId(Me._topLevelMethodOrdinal, Me.CompilationState.ModuleBuilderOpt.CurrentGenerationOrdinal)
		End Function

		Private Sub InitVariableProxy(ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode, ByVal originalSymbol As Symbol, ByVal framePointer As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, ByVal frameType As NamedTypeSymbol, ByVal prologue As ArrayBuilder(Of BoundExpression))
			Dim boundLocal As BoundExpression
			Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = Nothing
			If (Me.Proxies.TryGetValue(originalSymbol, fieldSymbol) AndAlso Not Me._analysis.declaredInsideExpressionLambda.Contains(originalSymbol)) Then
				Dim kind As SymbolKind = originalSymbol.Kind
				If (kind = SymbolKind.Local) Then
					Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = DirectCast(originalSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)
					Dim localSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Nothing
					If (Not Me.LocalMap.TryGetValue(localSymbol, localSymbol1)) Then
						localSymbol1 = localSymbol
					End If
					boundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(syntaxNode, localSymbol1, False, localSymbol1.Type)
				Else
					If (kind <> SymbolKind.Parameter) Then
						Throw ExceptionUtilities.UnexpectedValue(originalSymbol.Kind)
					End If
					Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = DirectCast(originalSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol)
					Dim parameterSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = Nothing
					If (Not Me.ParameterMap.TryGetValue(parameterSymbol, parameterSymbol1)) Then
						parameterSymbol1 = parameterSymbol
					End If
					boundLocal = New BoundParameter(syntaxNode, parameterSymbol1, False, parameterSymbol1.Type)
				End If
				Dim fieldSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = fieldSymbol.AsMember(frameType)
				Dim boundAssignmentOperator As BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(syntaxNode, New BoundFieldAccess(syntaxNode, New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(syntaxNode, framePointer, frameType), fieldSymbol1, True, fieldSymbol1.Type, False), boundLocal, True, fieldSymbol1.Type, False)
				prologue.Add(boundAssignmentOperator)
			End If
		End Sub

		Private Shared Function InLoopOrLambda(ByVal lambdaSyntax As SyntaxNode, ByVal scopeSyntax As SyntaxNode) As Boolean
			Dim flag As Boolean
			Dim parent As SyntaxNode = lambdaSyntax.Parent
			While True
				If (parent Is Nothing OrElse parent = scopeSyntax) Then
					flag = False
					Return flag
				Else
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = parent.Kind()
					If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForEachBlock) Then
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileBlock OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ForBlock) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
							Exit While
						End If
					ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineFunctionLambdaExpression OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineSubLambdaExpression) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement) OrElse CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleDoLoopBlock) <= 4) Then
						Exit While
					End If
					parent = parent.Parent
				End If
			End While
			flag = True
			Return flag
		End Function

		Private Function IntroduceFrame(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundNode, ByVal frame As LambdaFrame, ByVal F As Func(Of ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression), ArrayBuilder(Of LocalSymbol), Microsoft.CodeAnalysis.VisualBasic.BoundNode), Optional ByVal origLambda As LambdaSymbol = Nothing) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim enumerator As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator()
			Dim empty As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = LambdaRewriter.ConstructFrameType(Of TypeParameterSymbol)(frame, Me._currentTypeParameters)
			Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(Me._topLevelMethod, namedTypeSymbol, SynthesizedLocalKind.LambdaDisplayClass, frame.ScopeSyntax, False)
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).GetInstance()
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = frame.Constructor.AsMember(namedTypeSymbol)
			Dim syntax As SyntaxNode = node.Syntax
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(syntax, synthesizedLocal, namedTypeSymbol)
			If (methodSymbol.Parameters.IsEmpty) Then
				empty = ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Empty
			Else
				empty = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundLocal)
			End If
			Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = empty
			Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
			instance.Add(New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(syntax, boundLocal, New BoundObjectCreationExpression(syntax, methodSymbol, boundExpressions, Nothing, namedTypeSymbol, False, bitVector), True, namedTypeSymbol, False))
			Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = Nothing
			If (Me._innermostFramePointer IsNot Nothing) Then
				Me.Proxies.TryGetValue(Me._innermostFramePointer, fieldSymbol)
				If (Me._analysis.needsParentFrame.Contains(node)) Then
					Dim lambdaCapturedVariable As Microsoft.CodeAnalysis.VisualBasic.LambdaCapturedVariable = Microsoft.CodeAnalysis.VisualBasic.LambdaCapturedVariable.Create(frame, Me._innermostFramePointer, Me._synthesizedFieldNameIdDispenser)
					Dim fieldSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = lambdaCapturedVariable.AsMember(namedTypeSymbol)
					Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess(syntax, New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(syntax, synthesizedLocal, namedTypeSymbol), fieldSymbol1, True, fieldSymbol1.Type, False)
					Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.FrameOfType(syntax, TryCast(fieldSymbol1.Type, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol))
					Dim boundAssignmentOperator As Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(syntax, boundFieldAccess, boundExpression, True, boundFieldAccess.Type, False)
					If (Me._innermostFramePointer.Kind <> SymbolKind.Parameter OrElse Me._topLevelMethod.MethodKind <> MethodKind.Constructor OrElse CObj(Me._topLevelMethod) <> CObj(Me._currentMethod) OrElse Me._meIsInitialized) Then
						instance.Add(boundAssignmentOperator)
					Else
						Me._meProxyDeferredInit = boundAssignmentOperator
					End If
					Me.CompilationState.ModuleBuilderOpt.AddSynthesizedDefinition(frame, lambdaCapturedVariable.GetCciAdapter())
					Me.Proxies(Me._innermostFramePointer) = lambdaCapturedVariable
				End If
			End If
			If (origLambda IsNot Nothing) Then
				Dim enumerator1 As ImmutableArray(Of ParameterSymbol).Enumerator = origLambda.Parameters.GetEnumerator()
				While enumerator1.MoveNext()
					Me.InitVariableProxy(syntax, enumerator1.Current, synthesizedLocal, namedTypeSymbol, instance)
				End While
			ElseIf (Not Me._analysis.blockParent.ContainsKey(node)) Then
				Dim enumerator2 As ImmutableArray(Of ParameterSymbol).Enumerator = Me._topLevelMethod.Parameters.GetEnumerator()
				While enumerator2.MoveNext()
					Me.InitVariableProxy(syntax, enumerator2.Current, synthesizedLocal, namedTypeSymbol, instance)
				End While
			End If
			If (Me.PreserveOriginalLocals) Then
				Try
					enumerator = Me._analysis.capturedVariables.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator.Current
						If (current.Kind <> SymbolKind.Local) Then
							Continue While
						End If
						Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Nothing
						If (Not Me._analysis.variableScope.TryGetValue(current, boundNode) OrElse boundNode <> node) Then
							Continue While
						End If
						Me.InitVariableProxy(syntax, current, synthesizedLocal, namedTypeSymbol, instance)
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
			End If
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Me._innermostFramePointer
			Me._innermostFramePointer = synthesizedLocal
			Dim localSymbols As ArrayBuilder(Of LocalSymbol) = ArrayBuilder(Of LocalSymbol).GetInstance()
			localSymbols.Add(synthesizedLocal)
			Me._framePointers.Add(frame, synthesizedLocal)
			Dim boundNode1 As Microsoft.CodeAnalysis.VisualBasic.BoundNode = F(instance, localSymbols)
			Me._framePointers.Remove(frame)
			Me._innermostFramePointer = symbol
			If (Me._innermostFramePointer IsNot Nothing) Then
				If (fieldSymbol IsNot Nothing) Then
					Me.Proxies(Me._innermostFramePointer) = fieldSymbol
					Return boundNode1
				End If
				Me.Proxies.Remove(Me._innermostFramePointer)
			End If
			Return boundNode1
		End Function

		Private Function IsLegalBranch(ByVal gotoBlock As Microsoft.CodeAnalysis.VisualBasic.BoundNode, ByVal labelBlock As Microsoft.CodeAnalysis.VisualBasic.BoundNode) As Boolean
			Dim flag As Boolean
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = gotoBlock
			While True
				If (labelBlock = boundNode) Then
					flag = True
					Exit While
				ElseIf (boundNode Is Nothing OrElse Not Me._analysis.blockParent.TryGetValue(boundNode, boundNode)) Then
					Dim boundNode1 As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Me.LowestCommonAncestor(gotoBlock, labelBlock)
					boundNode = labelBlock
					While boundNode <> boundNode1
						If (Not Me._analysis.containsLiftingLambda.Contains(boundNode)) Then
							Me._analysis.blockParent.TryGetValue(boundNode, boundNode)
						Else
							flag = False
							Return flag
						End If
					End While
					flag = True
					Exit While
				End If
			End While
			Return flag
		End Function

		Private Function LowestCommonAncestor(ByVal gotoBlock As BoundNode, ByVal labelBlock As BoundNode) As BoundNode
			Dim boundNodes As HashSet(Of BoundNode) = New HashSet(Of BoundNode)()
			boundNodes.Add(gotoBlock)
			While Me._analysis.blockParent.TryGetValue(gotoBlock, gotoBlock)
				boundNodes.Add(gotoBlock)
			End While
			Dim item As BoundNode = labelBlock
			While Not boundNodes.Contains(item)
				item = Me._analysis.blockParent(item)
			End While
			Return item
		End Function

		Private Function MakeFrameCtor(ByVal frame As LambdaFrame, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As BoundBlock
			Dim constructor As MethodSymbol = frame.Constructor
			Dim syntax As SyntaxNode = constructor.Syntax
			Dim instance As ArrayBuilder(Of BoundStatement) = ArrayBuilder(Of BoundStatement).GetInstance()
			instance.Add(MethodCompiler.BindDefaultConstructorInitializer(constructor, diagnostics))
			If (Not constructor.Parameters.IsEmpty) Then
				Dim item As ParameterSymbol = constructor.Parameters(0)
				Dim boundParameter As Microsoft.CodeAnalysis.VisualBasic.BoundParameter = New Microsoft.CodeAnalysis.VisualBasic.BoundParameter(syntax, item, frame)
				Dim specialType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = frame.ContainingAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Boolean)
				Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = specialType.GetUseSiteInfo()
				diagnostics.Add(useSiteInfo, syntax.GetLocation())
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = frame.ContainingAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Object)
				Dim boundBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator(syntax, BinaryOperatorKind.[Is], New BoundDirectCast(syntax, boundParameter.MakeRValue(), ConversionKind.WideningReference, namedTypeSymbol, False), New BoundLiteral(syntax, ConstantValue.[Nothing], namedTypeSymbol), False, specialType, False)
				Dim generatedLabelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol("Done")
				instance.Add(New BoundConditionalGoto(syntax, boundBinaryOperator, True, generatedLabelSymbol, False))
				Dim boundParameter1 As Microsoft.CodeAnalysis.VisualBasic.BoundParameter = New Microsoft.CodeAnalysis.VisualBasic.BoundParameter(syntax, constructor.MeParameter, frame)
				Dim enumerator As ArrayBuilder(Of LambdaCapturedVariable).Enumerator = frame.CapturedLocals.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As LambdaCapturedVariable = enumerator.Current
					Dim type As TypeSymbol = current.Type
					Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess(syntax, boundParameter1, current, True, current.Type, False)
					Dim boundFieldAccess1 As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess(syntax, boundParameter, current, False, current.Type, False)
					Dim boundAssignmentOperator As Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(syntax, boundFieldAccess, boundFieldAccess1, True, type, False)
					instance.Add(New BoundExpressionStatement(syntax, boundAssignmentOperator, False))
				End While
				instance.Add(New BoundLabelStatement(syntax, generatedLabelSymbol))
			End If
			instance.Add(New BoundReturnStatement(syntax, Nothing, Nothing, Nothing, False))
			Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
			Return New BoundBlock(syntax, statementSyntaxes, ImmutableArray(Of LocalSymbol).Empty, instance.ToImmutableAndFree(), False)
		End Function

		Private Sub MakeFrames(ByVal closureDebugInfo As ArrayBuilder(Of Microsoft.CodeAnalysis.CodeGen.ClosureDebugInfo))
			Dim enumerator As HashSet(Of Symbol).Enumerator = New HashSet(Of Symbol).Enumerator()
			Dim enumerator1 As Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.BoundNode, Microsoft.CodeAnalysis.VisualBasic.LambdaFrame).ValueCollection.Enumerator = New Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.BoundNode, Microsoft.CodeAnalysis.VisualBasic.LambdaFrame).ValueCollection.Enumerator()
			Dim flag As Boolean = Me._analysis.seenBackBranches
			Try
				enumerator = Me._analysis.capturedVariables.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Symbol = enumerator.Current
					Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Nothing
					If (Not Me._analysis.variableScope.TryGetValue(current, boundNode) OrElse Me._analysis.declaredInsideExpressionLambda.Contains(current)) Then
						Continue While
					End If
					Dim frameForScope As Microsoft.CodeAnalysis.VisualBasic.LambdaFrame = Me.GetFrameForScope(flag, current, boundNode, closureDebugInfo, Me._delegateRelaxationIdDispenser)
					Dim lambdaCapturedVariable As Microsoft.CodeAnalysis.VisualBasic.LambdaCapturedVariable = Microsoft.CodeAnalysis.VisualBasic.LambdaCapturedVariable.Create(frameForScope, current, Me._synthesizedFieldNameIdDispenser)
					Me.Proxies.Add(current, lambdaCapturedVariable)
					frameForScope.CapturedLocals.Add(lambdaCapturedVariable)
				End While
			Finally
				DirectCast(enumerator, IDisposable).Dispose()
			End Try
			Try
				enumerator1 = Me._frames.Values.GetEnumerator()
				While enumerator1.MoveNext()
					Dim lambdaFrame As Microsoft.CodeAnalysis.VisualBasic.LambdaFrame = enumerator1.Current
					Me.CompilationState.AddSynthesizedMethod(lambdaFrame.Constructor, Me.MakeFrameCtor(lambdaFrame, Me.Diagnostics))
				End While
			Finally
				DirectCast(enumerator1, IDisposable).Dispose()
			End Try
		End Sub

		Protected Overrides Function MaterializeProxy(ByVal origExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal proxy As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.FramePointer(origExpression.Syntax, proxy.ContainingType)
			Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = proxy.AsMember(DirectCast(boundExpression.Type, NamedTypeSymbol))
			Return New BoundFieldAccess(origExpression.Syntax, boundExpression, fieldSymbol, origExpression.IsLValue, fieldSymbol.Type, False)
		End Function

		Private Function OptimizeMethodCallForDelegateInvoke(ByVal node As BoundCall) As BoundCall
			Dim method As MethodSymbol = node.Method
			Dim receiverOpt As BoundExpression = node.ReceiverOpt
			Dim compoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = New CompoundUseSiteInfo(Of AssemblySymbol)(Me.Diagnostics, Me.CompilationState.Compilation.Assembly)
			If (method.MethodKind = MethodKind.DelegateInvoke AndAlso method.ContainingType.IsAnonymousType AndAlso receiverOpt.Kind = BoundKind.DelegateCreationExpression AndAlso Conversions.ClassifyMethodConversionForLambdaOrAnonymousDelegate(method, DirectCast(receiverOpt, Microsoft.CodeAnalysis.VisualBasic.BoundDelegateCreationExpression).Method, compoundUseSiteInfo) = MethodConversionKind.Identity) Then
				Me.Diagnostics.Add(node, compoundUseSiteInfo)
				Dim boundDelegateCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundDelegateCreationExpression = DirectCast(receiverOpt, Microsoft.CodeAnalysis.VisualBasic.BoundDelegateCreationExpression)
				If (Not boundDelegateCreationExpression.Method.IsReducedExtensionMethod) Then
					method = boundDelegateCreationExpression.Method
					receiverOpt = boundDelegateCreationExpression.ReceiverOpt
					node = node.Update(method, Nothing, receiverOpt, node.Arguments, node.DefaultArguments, Nothing, False, node.SuppressObjectClone, node.Type)
				End If
			End If
			Return node
		End Function

		Public Shared Function Rewrite(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundBlock, ByVal method As MethodSymbol, ByVal methodOrdinal As Integer, ByVal lambdaDebugInfoBuilder As ArrayBuilder(Of LambdaDebugInfo), ByVal closureDebugInfoBuilder As ArrayBuilder(Of ClosureDebugInfo), ByRef delegateRelaxationIdDispenser As Integer, ByVal slotAllocatorOpt As VariableSlotAllocator, ByVal CompilationState As TypeCompilationState, ByVal symbolsCapturedWithoutCopyCtor As ISet(Of Symbol), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal rewrittenNodes As HashSet(Of BoundNode)) As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim analysi As Microsoft.CodeAnalysis.VisualBasic.LambdaRewriter.Analysis = Microsoft.CodeAnalysis.VisualBasic.LambdaRewriter.Analysis.AnalyzeMethodBody(node, method, symbolsCapturedWithoutCopyCtor, diagnostics)
			If (analysi.seenLambda) Then
				Dim lambdaRewriter As Microsoft.CodeAnalysis.VisualBasic.LambdaRewriter = New Microsoft.CodeAnalysis.VisualBasic.LambdaRewriter(analysi, method, methodOrdinal, lambdaDebugInfoBuilder, delegateRelaxationIdDispenser, slotAllocatorOpt, CompilationState, diagnostics)
				analysi.ComputeLambdaScopesAndFrameCaptures()
				lambdaRewriter.MakeFrames(closureDebugInfoBuilder)
				Dim boundBlock1 As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = DirectCast(lambdaRewriter.Visit(node), Microsoft.CodeAnalysis.VisualBasic.BoundBlock)
				delegateRelaxationIdDispenser = lambdaRewriter._delegateRelaxationIdDispenser
				boundBlock = boundBlock1
			Else
				boundBlock = node
			End If
			Return boundBlock
		End Function

		Private Function RewriteCatch(ByVal node As BoundCatchBlock, ByVal prologue As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression), ByVal newLocals As ArrayBuilder(Of LocalSymbol)) As BoundCatchBlock
			Dim item As LocalSymbol = Nothing
			If (newLocals.Count <> 0) Then
				item = newLocals(0)
			ElseIf (node.LocalOpt IsNot Nothing) Then
				Dim localOpt As LocalSymbol = node.LocalOpt
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = MyBase.VisitType(localOpt.Type)
				If (Not Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol.Equals(typeSymbol, localOpt.Type, TypeCompareKind.ConsiderEverything)) Then
					Dim flag As Boolean = False
					item = MethodToClassRewriter(Of FieldSymbol).CreateReplacementLocalOrReturnSelf(localOpt, typeSymbol, False, flag)
					Me.LocalMap.Add(localOpt, item)
				Else
					item = localOpt
				End If
			End If
			Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ExceptionSourceOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			If (prologue.Count <> 0) Then
				boundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(boundSequence.Syntax, ImmutableArray(Of LocalSymbol).Empty, prologue.ToImmutable(), boundSequence, boundSequence.Type, False)
			End If
			newLocals.Free()
			prologue.Free()
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ErrorLineNumberOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ExceptionFilterOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = DirectCast(Me.Visit(node.Body), Microsoft.CodeAnalysis.VisualBasic.BoundBlock)
			Return node.Update(item, boundSequence, boundExpression, boundExpression1, boundBlock, node.IsSynthesizedAsyncCatchAll)
		End Function

		Private Function RewriteLambda(ByVal node As BoundLambda, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal convertToExpressionTree As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim item As InstanceTypeSymbol
			Dim closureOrdinal As Integer
			Dim closureKind As Microsoft.CodeAnalysis.VisualBasic.ClosureKind
			Dim lambdaId As DebugId
			Dim topLevelMethodId As DebugId
			Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (Not (convertToExpressionTree Or Me._inExpressionLambda)) Then
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Nothing
				If (Me._analysis.lambdaScopes.TryGetValue(node.LambdaSymbol, boundNode)) Then
					item = Me._frames(boundNode)
					closureKind = Microsoft.CodeAnalysis.VisualBasic.ClosureKind.General
					closureOrdinal = Me._frames(boundNode).ClosureOrdinal
				ElseIf (Me._analysis.capturedVariablesByLambda(node.LambdaSymbol).Count <> 0) Then
					item = DirectCast(Me._topLevelMethod.ContainingType, InstanceTypeSymbol)
					closureKind = Microsoft.CodeAnalysis.VisualBasic.ClosureKind.ThisOnly
					closureOrdinal = -2
				Else
					item = Me.GetStaticFrame(node, Me.Diagnostics)
					closureKind = Microsoft.CodeAnalysis.VisualBasic.ClosureKind.[Static]
					closureOrdinal = -1
				End If
				If (node.LambdaSymbol.SynthesizedKind <> SynthesizedLambdaKind.DelegateRelaxationStub) Then
					lambdaId = Me.GetLambdaId(node.Syntax, closureKind, closureOrdinal)
					topLevelMethodId = Me.GetTopLevelMethodId()
				Else
					Me._delegateRelaxationIdDispenser = Me._delegateRelaxationIdDispenser + 1
					Dim currentGenerationOrdinal As Integer = Me.CompilationState.ModuleBuilderOpt.CurrentGenerationOrdinal
					lambdaId = New DebugId(Me._delegateRelaxationIdDispenser, currentGenerationOrdinal)
					topLevelMethodId = New DebugId(Me._topLevelMethodOrdinal, currentGenerationOrdinal)
				End If
				Dim synthesizedLambdaMethod As Microsoft.CodeAnalysis.VisualBasic.SynthesizedLambdaMethod = New Microsoft.CodeAnalysis.VisualBasic.SynthesizedLambdaMethod(item, closureKind, Me._topLevelMethod, topLevelMethodId, node, lambdaId, Me.Diagnostics)
				Me.CompilationState.ModuleBuilderOpt.AddSynthesizedDefinition(item, synthesizedLambdaMethod.GetCciAdapter())
				Dim parameters As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol) = node.LambdaSymbol.Parameters
				Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol).Enumerator = parameters.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = enumerator.Current
					Dim parameterMap As Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol) = Me.ParameterMap
					parameters = synthesizedLambdaMethod.Parameters
					parameterMap.Add(current, parameters(current.Ordinal))
				End While
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me._currentMethod
				Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = Me._currentFrameThis
				Dim typeParameterSymbols As ImmutableArray(Of TypeParameterSymbol) = Me._currentTypeParameters
				Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Me._innermostFramePointer
				Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Me._currentLambdaBodyTypeSubstitution
				Dim lambdaFrame As Microsoft.CodeAnalysis.VisualBasic.LambdaFrame = TryCast(item, Microsoft.CodeAnalysis.VisualBasic.LambdaFrame)
				Me._currentMethod = synthesizedLambdaMethod
				If (closureKind <> Microsoft.CodeAnalysis.VisualBasic.ClosureKind.[Static]) Then
					Me._currentFrameThis = synthesizedLambdaMethod.MeParameter
					Me._innermostFramePointer = Nothing
					Me._framePointers.TryGetValue(item, Me._innermostFramePointer)
				Else
					Me._innermostFramePointer = Nothing
					Me._currentFrameThis = Nothing
				End If
				If (lambdaFrame Is Nothing) Then
					Me._currentTypeParameters = synthesizedLambdaMethod.TypeParameters
					Me._currentLambdaBodyTypeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Create(Me._topLevelMethod, Me._topLevelMethod.TypeParameters, Me._currentMethod.TypeArguments, False)
				Else
					Me._currentTypeParameters = item.TypeParameters
					Me._currentLambdaBodyTypeSubstitution = lambdaFrame.TypeMap
				End If
				Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.RewriteLambdaAsMethod(synthesizedLambdaMethod, node)
				Me.CompilationState.AddSynthesizedMethod(synthesizedLambdaMethod, boundStatement)
				Me._currentMethod = methodSymbol
				Me._currentFrameThis = parameterSymbol
				Me._currentTypeParameters = typeParameterSymbols
				Me._innermostFramePointer = symbol
				Me._currentLambdaBodyTypeSubstitution = typeSubstitution
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = item
				If (lambdaFrame IsNot Nothing) Then
					namedTypeSymbol = LambdaRewriter.ConstructFrameType(Of TypeParameterSymbol)(lambdaFrame, Me._currentTypeParameters)
				End If
				If (closureKind = Microsoft.CodeAnalysis.VisualBasic.ClosureKind.[Static]) Then
					Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = lambdaFrame.SingletonCache.AsMember(namedTypeSymbol)
					boundFieldAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess(node.Syntax, Nothing, fieldSymbol, False, fieldSymbol.Type, False)
				Else
					boundFieldAccess = Me.FrameOfType(node.Syntax, namedTypeSymbol)
				End If
				Dim methodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = synthesizedLambdaMethod.AsMember(namedTypeSymbol)
				If (methodSymbol1.IsGenericMethod) Then
					methodSymbol1 = methodSymbol1.Construct(StaticCast(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol).From(Of TypeParameterSymbol)(Me._currentTypeParameters))
				End If
				Dim boundDelegateCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundDelegateCreationExpression(node.Syntax, boundFieldAccess, methodSymbol1, Nothing, Nothing, Nothing, type, False)
				If (If(closureKind <> Microsoft.CodeAnalysis.VisualBasic.ClosureKind.[Static] OrElse Me.CurrentMethod.MethodKind = MethodKind.StaticConstructor, False, Not methodSymbol1.IsGenericMethod) OrElse If(boundNode Is Nothing OrElse boundNode = Me._analysis.blockParent(node.Body), False, LambdaRewriter.InLoopOrLambda(node.Syntax, boundNode.Syntax))) Then
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = If(lambdaFrame Is Nothing, type, type.InternalSubstituteTypeParameters(lambdaFrame.TypeMap).Type)
					Dim str As String = GeneratedNames.MakeLambdaCacheFieldName(If(closureKind = Microsoft.CodeAnalysis.VisualBasic.ClosureKind.General, -1, topLevelMethodId.Ordinal), topLevelMethodId.Generation, lambdaId.Ordinal, lambdaId.Generation, node.LambdaSymbol.SynthesizedKind)
					Dim synthesizedLambdaCacheFieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = New Microsoft.CodeAnalysis.VisualBasic.SynthesizedLambdaCacheFieldSymbol(item, node.LambdaSymbol, typeSymbol, str, Me._topLevelMethod, Accessibility.[Public], False, closureKind = Microsoft.CodeAnalysis.VisualBasic.ClosureKind.[Static], False)
					Me.CompilationState.ModuleBuilderOpt.AddSynthesizedDefinition(item, synthesizedLambdaCacheFieldSymbol.GetCciAdapter())
					Dim syntheticBoundNodeFactory As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = New Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory(Me._topLevelMethod, Me._currentMethod, node.Syntax, Me.CompilationState, Me.Diagnostics)
					Dim fieldSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = synthesizedLambdaCacheFieldSymbol.AsMember(namedTypeSymbol)
					Dim boundFieldAccess1 As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = syntheticBoundNodeFactory.Field(boundFieldAccess, fieldSymbol1, True)
					Dim boundFieldAccess2 As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = syntheticBoundNodeFactory.Field(boundFieldAccess, fieldSymbol1, False)
					boundDelegateCreationExpression = syntheticBoundNodeFactory.Conditional(syntheticBoundNodeFactory.ObjectReferenceEqual(boundFieldAccess2, syntheticBoundNodeFactory.Null(boundFieldAccess2.Type)), syntheticBoundNodeFactory.AssignmentExpression(boundFieldAccess1, boundDelegateCreationExpression), boundFieldAccess2, boundFieldAccess2.Type)
				End If
				boundExpression = boundDelegateCreationExpression
			Else
				Dim flag As Boolean = Me._inExpressionLambda
				Me._inExpressionLambda = True
				Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = DirectCast(Me.Visit(node.Body), Microsoft.CodeAnalysis.VisualBasic.BoundBlock)
				node = node.Update(node.LambdaSymbol, boundBlock, node.Diagnostics, node.LambdaBinderOpt, node.DelegateRelaxation, node.MethodConversionKind)
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node
				If (Not flag) Then
					Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = type.ExpressionTargetDelegate(Me.CompilationState.Compilation)
					boundExpression1 = ExpressionLambdaRewriter.RewriteLambda(node, Me._currentMethod, namedTypeSymbol1, Me.CompilationState, Me.TypeMap, Me.Diagnostics, Me._rewrittenNodes, MyBase.RecursionDepth)
				End If
				Me._inExpressionLambda = flag
				boundExpression = boundExpression1
			End If
			Return boundExpression
		End Function

		Public Function RewriteLambdaAsMethod(ByVal method As SynthesizedLambdaMethod, ByVal lambda As BoundLambda) As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim syntax As SyntaxNode = lambda.Syntax
			Dim body As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = lambda.Body
			Dim lambdaFrame As Microsoft.CodeAnalysis.VisualBasic.LambdaFrame = Nothing
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = Nothing
			boundBlock = If(Not Me._frames.TryGetValue(body, lambdaFrame), MyBase.RewriteBlock(body), DirectCast(Me.IntroduceFrame(body, lambdaFrame, Function(prologue As ArrayBuilder(Of BoundExpression), newLocals As ArrayBuilder(Of LocalSymbol)) MyBase.RewriteBlock(body, prologue, newLocals), lambda.LambdaSymbol), Microsoft.CodeAnalysis.VisualBasic.BoundBlock))
			Dim stateMachineTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.StateMachineTypeSymbol = Nothing
			Dim variableSlotAllocator As Microsoft.CodeAnalysis.CodeGen.VariableSlotAllocator = Me.CompilationState.ModuleBuilderOpt.TryCreateVariableSlotAllocator(method, method.TopLevelMethod, Me.Diagnostics.DiagnosticBag)
			Return Rewriter.RewriteIteratorAndAsync(boundBlock, method, -1, Me.CompilationState, Me.Diagnostics, variableSlotAllocator, stateMachineTypeSymbol)
		End Function

		Private Function RewriteStatementList(ByVal node As BoundStatementList, ByVal prologue As ArrayBuilder(Of BoundExpression), ByVal newLocals As ArrayBuilder(Of LocalSymbol)) As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement).GetInstance()
			Dim enumerator As ArrayBuilder(Of BoundExpression).Enumerator = prologue.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As BoundExpression = enumerator.Current
				instance.Add(New BoundExpressionStatement(current.Syntax, current, False))
			End While
			prologue.Free()
			Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement).Enumerator = node.Statements.GetEnumerator()
			While enumerator1.MoveNext()
				Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = DirectCast(Me.Visit(enumerator1.Current), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
				If (boundStatement Is Nothing) Then
					Continue While
				End If
				instance.Add(boundStatement)
			End While
			If (newLocals.Count <> 0) Then
				Dim syntax As SyntaxNode = node.Syntax
				Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
				boundBlock = New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(syntax, statementSyntaxes, newLocals.ToImmutableAndFree(), instance.ToImmutableAndFree(), False)
			Else
				newLocals.Free()
				boundBlock = node.Update(instance.ToImmutableAndFree())
			End If
			Return boundBlock
		End Function

		Public Overrides Function VisitBlock(ByVal node As BoundBlock) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim lambdaFrame As Microsoft.CodeAnalysis.VisualBasic.LambdaFrame = Nothing
			If (Not Me._frames.TryGetValue(node, lambdaFrame)) Then
				boundNode = MyBase.RewriteBlock(node)
			Else
				boundNode = Me.IntroduceFrame(node, lambdaFrame, Function(prologue As ArrayBuilder(Of BoundExpression), newLocals As ArrayBuilder(Of LocalSymbol)) MyBase.RewriteBlock(node, prologue, newLocals), Nothing)
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitCall(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundCall) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode1 As Microsoft.CodeAnalysis.VisualBasic.BoundNode = MyBase.VisitCall(node)
			If (boundNode1.Kind <> BoundKind.[Call]) Then
				boundNode = boundNode1
			Else
				Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundCall = DirectCast(boundNode1, Microsoft.CodeAnalysis.VisualBasic.BoundCall)
				boundCall = Me.OptimizeMethodCallForDelegateInvoke(boundCall)
				If (CObj(Me._currentMethod) = CObj(Me._topLevelMethod)) Then
					Dim receiverOpt As BoundExpression = node.ReceiverOpt
					If (node.Method.MethodKind = MethodKind.Constructor AndAlso receiverOpt IsNot Nothing AndAlso receiverOpt.IsInstanceReference()) Then
						Me._meIsInitialized = True
						If (Me._meProxyDeferredInit Is Nothing) Then
							GoTo Label1
						End If
						Me._meProxyDeferredInitDone = True
						boundNode = LocalRewriter.GenerateSequenceValueSideEffects(Me._currentMethod, boundCall, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of BoundExpression)(Me._meProxyDeferredInit))
						Return boundNode
					End If
				End If
			Label1:
				boundNode = boundCall
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitCatchBlock(ByVal node As BoundCatchBlock) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim lambdaFrame As Microsoft.CodeAnalysis.VisualBasic.LambdaFrame = Nothing
			If (Not Me._frames.TryGetValue(node, lambdaFrame)) Then
				boundNode = Me.RewriteCatch(node, ArrayBuilder(Of BoundExpression).GetInstance(), ArrayBuilder(Of LocalSymbol).GetInstance())
			Else
				boundNode = Me.IntroduceFrame(node, lambdaFrame, Function(prologue As ArrayBuilder(Of BoundExpression), newLocals As ArrayBuilder(Of LocalSymbol)) Me.RewriteCatch(node, prologue, newLocals), Nothing)
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitConversion(ByVal conversion As BoundConversion) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim operand As BoundLambda = TryCast(conversion.Operand, BoundLambda)
			If (operand IsNot Nothing) Then
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.RewriteLambda(operand, MyBase.VisitType(conversion.Type), (conversion.ConversionKind And ConversionKind.ConvertedToExpressionTree) <> ConversionKind.DelegateRelaxationLevelNone)
				If (Me._inExpressionLambda) Then
					boundExpression = conversion.Update(boundExpression, conversion.ConversionKind, conversion.Checked, conversion.ExplicitCastInCode, conversion.ConstantValueOpt, conversion.ExtendedInfoOpt, conversion.Type)
				End If
				boundNode = boundExpression
			Else
				boundNode = MyBase.VisitConversion(conversion)
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitDirectCast(ByVal node As BoundDirectCast) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim operand As BoundLambda = TryCast(node.Operand, BoundLambda)
			If (operand IsNot Nothing) Then
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.RewriteLambda(operand, MyBase.VisitType(node.Type), (node.ConversionKind And ConversionKind.ConvertedToExpressionTree) <> ConversionKind.DelegateRelaxationLevelNone)
				If (Me._inExpressionLambda) Then
					boundExpression = node.Update(boundExpression, node.ConversionKind, node.SuppressVirtualCalls, node.ConstantValueOpt, node.RelaxationLambdaOpt, node.Type)
				End If
				boundNode = boundExpression
			Else
				boundNode = MyBase.VisitDirectCast(node)
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitGotoStatement(ByVal node As BoundGotoStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim label As LabelSymbol = node.Label
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Nothing
			If (label IsNot Nothing AndAlso Me._analysis.labelBlock.TryGetValue(node.Label, boundNode)) Then
				Dim boundNode1 As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Nothing
				If (Me._analysis.gotoBlock.TryGetValue(node, boundNode1) AndAlso Not Me.IsLegalBranch(boundNode1, boundNode)) Then
					Dim generatedUnstructuredExceptionHandlingResumeLabel As Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedUnstructuredExceptionHandlingResumeLabel = TryCast(label, Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedUnstructuredExceptionHandlingResumeLabel)
					If (generatedUnstructuredExceptionHandlingResumeLabel Is Nothing) Then
						Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = node.Syntax.Kind()
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnErrorGoToLabelStatement OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ResumeLabelStatement) Then
							Me.Diagnostics.Add(ERRID.ERR_CannotGotoNonScopeBlocksWithClosure, node.Syntax.GetLocation(), New [Object]() { node.Syntax.ToString(), [String].Empty, label.Name })
						Else
							Me.Diagnostics.Add(ERRID.ERR_CannotGotoNonScopeBlocksWithClosure, node.Syntax.GetLocation(), New [Object]() { "Goto ", label.Name, label.Name })
						End If
					ElseIf (Not Me._reported_ERR_CannotUseOnErrorGotoWithClosure) Then
						Me._reported_ERR_CannotUseOnErrorGotoWithClosure = True
						Me.Diagnostics.Add(ERRID.ERR_CannotUseOnErrorGotoWithClosure, generatedUnstructuredExceptionHandlingResumeLabel.ResumeStatement.GetLocation(), New [Object]() { generatedUnstructuredExceptionHandlingResumeLabel.ResumeStatement.ToString() })
					End If
					node = New BoundGotoStatement(node.Syntax, node.Label, node.LabelExpressionOpt, True)
				End If
			End If
			Return MyBase.VisitGotoStatement(node)
		End Function

		Public Overrides Function VisitLambda(ByVal node As BoundLambda) As BoundNode
			Throw ExceptionUtilities.Unreachable
		End Function

		Public Overrides Function VisitLoweredConditionalAccess(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundLoweredConditionalAccess) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundLoweredConditionalAccess As Microsoft.CodeAnalysis.VisualBasic.BoundLoweredConditionalAccess = DirectCast(MyBase.VisitLoweredConditionalAccess(node), Microsoft.CodeAnalysis.VisualBasic.BoundLoweredConditionalAccess)
			boundNode = If(boundLoweredConditionalAccess.CaptureReceiver OrElse node.ReceiverOrCondition.Type.IsBooleanType() OrElse node.ReceiverOrCondition.Kind = boundLoweredConditionalAccess.ReceiverOrCondition.Kind, boundLoweredConditionalAccess, boundLoweredConditionalAccess.Update(boundLoweredConditionalAccess.ReceiverOrCondition, True, boundLoweredConditionalAccess.PlaceholderId, boundLoweredConditionalAccess.WhenNotNull, boundLoweredConditionalAccess.WhenNullOpt, boundLoweredConditionalAccess.Type))
			Return boundNode
		End Function

		Public Overrides Function VisitMeReference(ByVal node As BoundMeReference) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			If (Me._currentMethod = Me._topLevelMethod) Then
				boundNode = node
			ElseIf (Not Me._topLevelMethod.IsShared) Then
				boundNode = Me.FramePointer(node.Syntax, TryCast(node.Type, NamedTypeSymbol))
			Else
				boundNode = node
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitMyBaseReference(ByVal node As BoundMyBaseReference) As BoundNode
			If (CObj(Me._currentMethod) = CObj(Me._topLevelMethod)) Then
				Return node
			End If
			If (CObj(Me._currentMethod.ContainingType) = CObj(Me._topLevelMethod.ContainingType)) Then
				Return New BoundMyBaseReference(node.Syntax, node.Type)
			End If
			Return Me.FramePointer(node.Syntax, Me._topLevelMethod.ContainingType)
		End Function

		Public Overrides Function VisitMyClassReference(ByVal node As BoundMyClassReference) As BoundNode
			If (CObj(Me._currentMethod) = CObj(Me._topLevelMethod)) Then
				Return node
			End If
			If (CObj(Me._currentMethod.ContainingType) = CObj(Me._topLevelMethod.ContainingType)) Then
				Return New BoundMyClassReference(node.Syntax, node.Type)
			End If
			Return Me.FramePointer(node.Syntax, Me._topLevelMethod.ContainingType)
		End Function

		Public Overrides Function VisitRangeVariable(ByVal node As BoundRangeVariable) As BoundNode
			Throw ExceptionUtilities.Unreachable
		End Function

		Public Overrides Function VisitSequence(ByVal node As BoundSequence) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim lambdaFrame As Microsoft.CodeAnalysis.VisualBasic.LambdaFrame = Nothing
			If (Not Me._frames.TryGetValue(node, lambdaFrame)) Then
				boundNode = MyBase.RewriteSequence(node)
			Else
				boundNode = Me.IntroduceFrame(node, lambdaFrame, Function(prologue As ArrayBuilder(Of BoundExpression), newLocals As ArrayBuilder(Of LocalSymbol)) MyBase.RewriteSequence(node, prologue, newLocals), Nothing)
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitStatementList(ByVal node As BoundStatementList) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim lambdaFrame As Microsoft.CodeAnalysis.VisualBasic.LambdaFrame = Nothing
			boundNode = If(Not Me._frames.TryGetValue(node, lambdaFrame), MyBase.VisitStatementList(node), Me.IntroduceFrame(node, lambdaFrame, Function(prologue As ArrayBuilder(Of BoundExpression), newLocals As ArrayBuilder(Of LocalSymbol)) Me.RewriteStatementList(node, prologue, newLocals), Nothing))
			Return boundNode
		End Function

		Public Overrides Function VisitTryCast(ByVal node As BoundTryCast) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim operand As BoundLambda = TryCast(node.Operand, BoundLambda)
			If (operand IsNot Nothing) Then
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.RewriteLambda(operand, MyBase.VisitType(node.Type), (node.ConversionKind And ConversionKind.ConvertedToExpressionTree) <> ConversionKind.DelegateRelaxationLevelNone)
				If (Me._inExpressionLambda) Then
					boundExpression = node.Update(boundExpression, node.ConversionKind, node.ConstantValueOpt, node.RelaxationLambdaOpt, node.Type)
				End If
				boundNode = boundExpression
			Else
				boundNode = MyBase.VisitTryCast(node)
			End If
			Return boundNode
		End Function

		Friend NotInheritable Class Analysis
			Inherits BoundTreeWalkerWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator
			Private ReadOnly _diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag

			Private ReadOnly _method As MethodSymbol

			Private _currentParent As MethodSymbol

			Private _currentBlock As BoundNode

			Friend seenLambda As Boolean

			Friend seenBackBranches As Boolean

			Friend blockParent As Dictionary(Of BoundNode, BoundNode)

			Friend lambdaParent As Dictionary(Of LambdaSymbol, MethodSymbol)

			Friend variableScope As Dictionary(Of Symbol, BoundNode)

			Friend labelBlock As Dictionary(Of LabelSymbol, BoundNode)

			Friend gotoBlock As Dictionary(Of BoundGotoStatement, BoundNode)

			Friend containsLiftingLambda As HashSet(Of BoundNode)

			Friend needsParentFrame As HashSet(Of BoundNode)

			Friend lambdaScopes As Dictionary(Of LambdaSymbol, BoundNode)

			Friend capturedVariables As HashSet(Of Symbol)

			Friend capturedVariablesByLambda As MultiDictionary(Of LambdaSymbol, Symbol)

			Friend ReadOnly declaredInsideExpressionLambda As HashSet(Of Symbol)

			Private _inExpressionLambda As Boolean

			Friend ReadOnly symbolsCapturedWithoutCopyCtor As ISet(Of Symbol)

			Private Sub New(ByVal method As MethodSymbol, ByVal symbolsCapturedWithoutCopyCtor As ISet(Of Symbol), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
				MyBase.New()
				Me.seenLambda = False
				Me.seenBackBranches = False
				Me.blockParent = New Dictionary(Of BoundNode, BoundNode)()
				Me.lambdaParent = New Dictionary(Of LambdaSymbol, MethodSymbol)(ReferenceEqualityComparer.Instance)
				Me.variableScope = New Dictionary(Of Symbol, BoundNode)(ReferenceEqualityComparer.Instance)
				Me.labelBlock = New Dictionary(Of LabelSymbol, BoundNode)(ReferenceEqualityComparer.Instance)
				Me.gotoBlock = New Dictionary(Of BoundGotoStatement, BoundNode)()
				Me.containsLiftingLambda = New HashSet(Of BoundNode)()
				Me.capturedVariables = New HashSet(Of Symbol)(ReferenceEqualityComparer.Instance)
				Me.capturedVariablesByLambda = New MultiDictionary(Of LambdaSymbol, Symbol)(ReferenceEqualityComparer.Instance)
				Me.declaredInsideExpressionLambda = New HashSet(Of Symbol)(ReferenceEqualityComparer.Instance)
				Me._currentParent = method
				Me._method = method
				Me.symbolsCapturedWithoutCopyCtor = symbolsCapturedWithoutCopyCtor
				Me._diagnostics = diagnostics
				Me._inExpressionLambda = False
			End Sub

			Private Sub Analyze(ByVal node As BoundNode)
				If (node IsNot Nothing) Then
					Me._currentBlock = node
					If (Me._method IsNot Nothing) Then
						Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = Me._method.Parameters.GetEnumerator()
						While enumerator.MoveNext()
							Dim current As ParameterSymbol = enumerator.Current
							Me.variableScope.Add(current, Me._currentBlock)
							If (Not Me._inExpressionLambda) Then
								Continue While
							End If
							Me.declaredInsideExpressionLambda.Add(current)
						End While
					End If
					Me.Visit(node)
				End If
			End Sub

			Public Shared Function AnalyzeMethodBody(ByVal node As BoundBlock, ByVal method As MethodSymbol, ByVal symbolsCapturedWithoutCtor As ISet(Of Symbol), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As LambdaRewriter.Analysis
				Dim analysi As LambdaRewriter.Analysis = New LambdaRewriter.Analysis(method, symbolsCapturedWithoutCtor, diagnostics)
				analysi.Analyze(node)
				Return analysi
			End Function

			Private Function BlockDepth(ByVal node As BoundNode) As Integer
				Dim num As Integer = -1
				Do
					If (node Is Nothing) Then
						Exit Do
					End If
					num = num + 1
				Loop While Me.blockParent.TryGetValue(node, node)
				Return num
			End Function

			Friend Sub ComputeLambdaScopesAndFrameCaptures()
				Dim enumerator As Dictionary(Of LambdaSymbol, MultiDictionary(Of LambdaSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbol).ValueSet).Enumerator = New Dictionary(Of LambdaSymbol, MultiDictionary(Of LambdaSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbol).ValueSet).Enumerator()
				Dim enumerator1 As MultiDictionary(Of LambdaSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbol).ValueSet.Enumerator = New MultiDictionary(Of LambdaSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbol).ValueSet.Enumerator()
				Dim num As Integer
				Me.lambdaScopes = New Dictionary(Of LambdaSymbol, Microsoft.CodeAnalysis.VisualBasic.BoundNode)(ReferenceEqualityComparer.Instance)
				Me.needsParentFrame = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.BoundNode)()
				Try
					enumerator = Me.capturedVariablesByLambda.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As KeyValuePair(Of LambdaSymbol, MultiDictionary(Of LambdaSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbol).ValueSet) = enumerator.Current
						Dim num1 As Integer = -1
						Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Nothing
						Dim num2 As Integer = 2147483647
						Dim boundNode1 As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Nothing
						Try
							enumerator1 = current.Value.GetEnumerator()
							While enumerator1.MoveNext()
								Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator1.Current
								Dim boundNode2 As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Nothing
								num = If(Me.variableScope.TryGetValue(symbol, boundNode2), Me.BlockDepth(boundNode2), -1)
								If (num > num1) Then
									num1 = num
									boundNode = boundNode2
								End If
								If (num >= num2) Then
									Continue While
								End If
								num2 = num
								boundNode1 = boundNode2
							End While
						Finally
							DirectCast(enumerator1, IDisposable).Dispose()
						End Try
						If (boundNode Is Nothing) Then
							Continue While
						End If
						Me.lambdaScopes.Add(current.Key, boundNode)
						While boundNode <> boundNode1
							Me.needsParentFrame.Add(boundNode)
							Me.blockParent.TryGetValue(boundNode, boundNode)
						End While
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
			End Sub

			Private Shared Function MayParticipateInIllegalBranch(ByVal node As BoundGotoStatement) As Boolean
				Return Not node.WasCompilerGenerated
			End Function

			Public Sub PopBlock(ByVal previousBlock As BoundNode)
				Me._currentBlock = previousBlock
			End Sub

			Public Function PushBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundNode, ByVal locals As ImmutableArray(Of LocalSymbol)) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				If (Not locals.IsEmpty) Then
					Dim boundNode1 As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Me._currentBlock
					Me._currentBlock = node
					If (Me._currentBlock <> boundNode1) Then
						Me.blockParent.Add(Me._currentBlock, boundNode1)
					End If
					Dim enumerator As ImmutableArray(Of LocalSymbol).Enumerator = locals.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As LocalSymbol = enumerator.Current
						Me.variableScope.Add(current, Me._currentBlock)
						If (Not Me._inExpressionLambda) Then
							Continue While
						End If
						Me.declaredInsideExpressionLambda.Add(current)
					End While
					boundNode = boundNode1
				Else
					boundNode = Me._currentBlock
				End If
				Return boundNode
			End Function

			Private Sub RecordCaptureInIntermediateBlocks(ByVal variableOrParameter As Symbol)
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Me._currentBlock
				Dim boundNode1 As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Nothing
				Me.variableScope.TryGetValue(variableOrParameter, boundNode1)
				Me.containsLiftingLambda.Add(boundNode)
				While boundNode <> boundNode1 AndAlso boundNode IsNot Nothing
					If (Not Me.blockParent.TryGetValue(boundNode, boundNode)) Then
						Continue While
					End If
					Me.containsLiftingLambda.Add(boundNode)
				End While
			End Sub

			Private Sub ReferenceVariable(ByVal variableOrParameter As Symbol, ByVal syntax As SyntaxNode)
				If (Me._currentParent.MethodKind = MethodKind.AnonymousFunction AndAlso (variableOrParameter.Kind <> SymbolKind.Local OrElse Not DirectCast(variableOrParameter, LocalSymbol).IsConst)) Then
					Dim containingSymbol As Symbol = variableOrParameter.ContainingSymbol
					Dim item As MethodSymbol = Me._currentParent
					Dim flag As Boolean = False
					If (item IsNot Nothing AndAlso item <> containingSymbol) Then
						Me.capturedVariables.Add(variableOrParameter)
						flag = True
						Me.RecordCaptureInIntermediateBlocks(variableOrParameter)
						Do
							Dim lambdaSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LambdaSymbol = DirectCast(item, Microsoft.CodeAnalysis.VisualBasic.Symbols.LambdaSymbol)
							Me.capturedVariablesByLambda.Add(lambdaSymbol, variableOrParameter)
							item = Me.lambdaParent(lambdaSymbol)
						Loop While item.MethodKind = MethodKind.AnonymousFunction AndAlso item <> containingSymbol
					End If
					If (flag) Then
						Me.VerifyCaptured(variableOrParameter, syntax)
					End If
				End If
			End Sub

			Private Sub VerifyCaptured(ByVal variableOrParameter As Symbol, ByVal syntax As SyntaxNode)
				Dim type As TypeSymbol
				Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = TryCast(variableOrParameter, Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol)
				If (parameterSymbol Is Nothing) Then
					Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = DirectCast(variableOrParameter, Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)
					type = localSymbol.Type
					If (localSymbol.IsByRef) Then
						Throw ExceptionUtilities.UnexpectedValue(localSymbol.IsByRef)
					End If
				Else
					type = parameterSymbol.Type
				End If
				If (type.IsRestrictedType()) Then
					If (Binder.IsTopMostEnclosingLambdaAQueryLambda(Me._currentParent, variableOrParameter.ContainingSymbol)) Then
						Me._diagnostics.Add(ERRID.ERR_CannotLiftRestrictedTypeQuery, syntax.GetLocation(), New [Object]() { type })
						Return
					End If
					Me._diagnostics.Add(ERRID.ERR_CannotLiftRestrictedTypeLambda, syntax.GetLocation(), New [Object]() { type })
				End If
			End Sub

			Public Overrides Function VisitBlock(ByVal node As BoundBlock) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Me.PushBlock(node, node.Locals)
				Dim boundNode1 As Microsoft.CodeAnalysis.VisualBasic.BoundNode = MyBase.VisitBlock(node)
				Me.PopBlock(boundNode)
				Return boundNode1
			End Function

			Public Overrides Function VisitCatchBlock(ByVal node As BoundCatchBlock) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				If (node.LocalOpt IsNot Nothing) Then
					Dim boundNode1 As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Me.PushBlock(node, ImmutableArray.Create(Of LocalSymbol)(node.LocalOpt))
					Dim boundNode2 As Microsoft.CodeAnalysis.VisualBasic.BoundNode = MyBase.VisitCatchBlock(node)
					Me.PopBlock(boundNode1)
					boundNode = boundNode2
				Else
					boundNode = MyBase.VisitCatchBlock(node)
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitConditionalGoto(ByVal node As BoundConditionalGoto) As BoundNode
				If (Me.labelBlock.ContainsKey(node.Label)) Then
					Me.seenBackBranches = True
				End If
				Return MyBase.VisitConditionalGoto(node)
			End Function

			Public Overrides Function VisitConversion(ByVal conversion As BoundConversion) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim operand As BoundLambda = TryCast(conversion.Operand, BoundLambda)
				boundNode = If(operand IsNot Nothing, Me.VisitLambda(operand, (conversion.ConversionKind And ConversionKind.ConvertedToExpressionTree) <> ConversionKind.DelegateRelaxationLevelNone), MyBase.VisitConversion(conversion))
				Return boundNode
			End Function

			Public Overrides Function VisitDirectCast(ByVal node As BoundDirectCast) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim operand As BoundLambda = TryCast(node.Operand, BoundLambda)
				boundNode = If(operand IsNot Nothing, Me.VisitLambda(operand, (node.ConversionKind And ConversionKind.ConvertedToExpressionTree) <> ConversionKind.DelegateRelaxationLevelNone), MyBase.VisitDirectCast(node))
				Return boundNode
			End Function

			Public Overrides Function VisitGotoStatement(ByVal node As BoundGotoStatement) As BoundNode
				If (Me.labelBlock.ContainsKey(node.Label)) Then
					Me.seenBackBranches = True
				End If
				If (LambdaRewriter.Analysis.MayParticipateInIllegalBranch(node)) Then
					Me.gotoBlock.Add(node, Me._currentBlock)
				End If
				Return MyBase.VisitGotoStatement(node)
			End Function

			Public Overrides Function VisitLabelStatement(ByVal node As BoundLabelStatement) As BoundNode
				Me.labelBlock.Add(node.Label, Me._currentBlock)
				Return MyBase.VisitLabelStatement(node)
			End Function

			Public Overrides Function VisitLambda(ByVal node As BoundLambda) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Private Function VisitLambda(ByVal node As BoundLambda, ByVal convertToExpressionTree As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Me.seenLambda = True
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me._currentParent
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Me._currentBlock
				Me._currentParent = node.LambdaSymbol
				Me._currentBlock = node.Body
				Me.blockParent.Add(Me._currentBlock, boundNode)
				Me.lambdaParent.Add(node.LambdaSymbol, methodSymbol)
				Dim flag As Boolean = Me._inExpressionLambda
				Me._inExpressionLambda = If(Me._inExpressionLambda, True, convertToExpressionTree)
				Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = node.LambdaSymbol.Parameters.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As ParameterSymbol = enumerator.Current
					Me.variableScope.Add(current, Me._currentBlock)
					If (Not Me._inExpressionLambda) Then
						Continue While
					End If
					Me.declaredInsideExpressionLambda.Add(current)
				End While
				Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol).Enumerator = node.Body.Locals.GetEnumerator()
				While enumerator1.MoveNext()
					Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = enumerator1.Current
					Me.variableScope.Add(localSymbol, Me._currentBlock)
					If (Not Me._inExpressionLambda) Then
						Continue While
					End If
					Me.declaredInsideExpressionLambda.Add(localSymbol)
				End While
				Me._inExpressionLambda = flag
				Me._currentParent = methodSymbol
				Me._currentBlock = boundNode
				Return MyBase.VisitBlock(node.Body)
			End Function

			Public Overrides Function VisitLocal(ByVal node As BoundLocal) As BoundNode
				Me.ReferenceVariable(node.LocalSymbol, node.Syntax)
				Return MyBase.VisitLocal(node)
			End Function

			Public Overrides Function VisitMeReference(ByVal node As BoundMeReference) As BoundNode
				Me.ReferenceVariable(Me._method.MeParameter, node.Syntax)
				Return MyBase.VisitMeReference(node)
			End Function

			Public Overrides Function VisitMethodGroup(ByVal node As BoundMethodGroup) As BoundNode
				Return Nothing
			End Function

			Public Overrides Function VisitMyBaseReference(ByVal node As BoundMyBaseReference) As BoundNode
				Me.ReferenceVariable(Me._method.MeParameter, node.Syntax)
				Return MyBase.VisitMyBaseReference(node)
			End Function

			Public Overrides Function VisitMyClassReference(ByVal node As BoundMyClassReference) As BoundNode
				Me.ReferenceVariable(Me._method.MeParameter, node.Syntax)
				Return MyBase.VisitMyClassReference(node)
			End Function

			Public Overrides Function VisitParameter(ByVal node As BoundParameter) As BoundNode
				Me.ReferenceVariable(node.ParameterSymbol, node.Syntax)
				Return MyBase.VisitParameter(node)
			End Function

			Public Overrides Function VisitRangeVariable(ByVal node As BoundRangeVariable) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Public Overrides Function VisitSequence(ByVal node As BoundSequence) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Me.PushBlock(node, node.Locals)
				Dim boundNode1 As Microsoft.CodeAnalysis.VisualBasic.BoundNode = MyBase.VisitSequence(node)
				Me.PopBlock(boundNode)
				Return boundNode1
			End Function

			Public Overrides Function VisitTryCast(ByVal node As BoundTryCast) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim operand As BoundLambda = TryCast(node.Operand, BoundLambda)
				boundNode = If(operand IsNot Nothing, Me.VisitLambda(operand, (node.ConversionKind And ConversionKind.ConvertedToExpressionTree) <> ConversionKind.DelegateRelaxationLevelNone), MyBase.VisitTryCast(node))
				Return boundNode
			End Function
		End Class
	End Class
End Namespace