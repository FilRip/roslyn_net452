Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class IteratorRewriter
		Inherits StateMachineRewriter(Of FieldSymbol)
		Private ReadOnly _elementType As TypeSymbol

		Private ReadOnly _isEnumerable As Boolean

		Private _currentField As FieldSymbol

		Private _initialThreadIdField As FieldSymbol

		Protected Overrides ReadOnly Property PreserveInitialParameterValues As Boolean
			Get
				Return Me._isEnumerable
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeMap As TypeSubstitution
			Get
				Return Me.StateMachineType.TypeSubstitution
			End Get
		End Property

		Public Sub New(ByVal body As BoundStatement, ByVal method As MethodSymbol, ByVal isEnumerable As Boolean, ByVal stateMachineType As IteratorStateMachine, ByVal slotAllocatorOpt As VariableSlotAllocator, ByVal compilationState As TypeCompilationState, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			MyBase.New(body, method, stateMachineType, slotAllocatorOpt, compilationState, diagnostics)
			Me._isEnumerable = isEnumerable
			Dim returnType As TypeSymbol = method.ReturnType
			If (returnType.GetArity() = 0) Then
				Me._elementType = method.ContainingAssembly.GetSpecialType(SpecialType.System_Object)
				Return
			End If
			Me._elementType = DirectCast(returnType, NamedTypeSymbol).TypeArgumentsNoUseSiteDiagnostics.[Single]().InternalSubstituteTypeParameters(Me.TypeMap).Type
		End Sub

		Protected Overrides Function CreateByValLocalCapture(ByVal field As FieldSymbol, ByVal local As LocalSymbol) As FieldSymbol
			Return field
		End Function

		Protected Overrides Function CreateParameterCapture(ByVal field As FieldSymbol, ByVal parameter As ParameterSymbol) As FieldSymbol
			Return field
		End Function

		Friend Overrides Function EnsureAllSymbolsAndSignature() As Boolean
			Dim flag As Boolean
			If (MyBase.EnsureAllSymbolsAndSignature() OrElse Me.Method.IsSub OrElse Me._elementType.IsErrorType()) Then
				flag = True
			Else
				Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance(True, Me.Diagnostics.AccumulatesDependencies)
				MyBase.EnsureSpecialType(SpecialType.System_Object, instance)
				MyBase.EnsureSpecialType(SpecialType.System_Boolean, instance)
				MyBase.EnsureSpecialType(SpecialType.System_Int32, instance)
				MyBase.EnsureSpecialType(SpecialType.System_IDisposable, instance)
				MyBase.EnsureSpecialMember(SpecialMember.System_IDisposable__Dispose, instance)
				MyBase.EnsureSpecialType(SpecialType.System_Collections_IEnumerator, instance)
				MyBase.EnsureSpecialPropertyGetter(SpecialMember.System_Collections_IEnumerator__Current, instance)
				MyBase.EnsureSpecialMember(SpecialMember.System_Collections_IEnumerator__MoveNext, instance)
				MyBase.EnsureSpecialMember(SpecialMember.System_Collections_IEnumerator__Reset, instance)
				MyBase.EnsureSpecialType(SpecialType.System_Collections_Generic_IEnumerator_T, instance)
				MyBase.EnsureSpecialPropertyGetter(SpecialMember.System_Collections_Generic_IEnumerator_T__Current, instance)
				If (Me._isEnumerable) Then
					MyBase.EnsureSpecialType(SpecialType.System_Collections_IEnumerable, instance)
					MyBase.EnsureSpecialMember(SpecialMember.System_Collections_IEnumerable__GetEnumerator, instance)
					MyBase.EnsureSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T, instance)
					MyBase.EnsureSpecialMember(SpecialMember.System_Collections_Generic_IEnumerable_T__GetEnumerator, instance)
				End If
				Dim flag1 As Boolean = instance.HasAnyErrors()
				If (Not flag1) Then
					Me.Diagnostics.AddDependencies(instance, False)
				Else
					Me.Diagnostics.AddRange(instance, False)
				End If
				instance.Free()
				flag = flag1
			End If
			Return flag
		End Function

		Protected Overrides Sub GenerateControlFields()
			Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol
			Me.StateField = Me.F.StateMachineField(Me.F.SpecialType(SpecialType.System_Int32), Me.Method, GeneratedNames.MakeStateMachineStateFieldName(), Accessibility.[Public])
			Me._currentField = Me.F.StateMachineField(Me._elementType, Me.Method, GeneratedNames.MakeIteratorCurrentFieldName(), Accessibility.[Public])
			If (Me._isEnumerable) Then
				fieldSymbol = Me.F.StateMachineField(Me.F.SpecialType(SpecialType.System_Int32), Me.Method, GeneratedNames.MakeIteratorInitialThreadIdName(), Accessibility.[Public])
			Else
				fieldSymbol = Nothing
			End If
			Me._initialThreadIdField = fieldSymbol
		End Sub

		Protected Overrides Sub GenerateMethodImplementations()
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
			Dim synthesizedMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedMethod = MyBase.OpenMethodImplementation(SpecialMember.System_IDisposable__Dispose, "Dispose", Accessibility.[Private], True, Nothing)
			Dim synthesizedMethod1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedMethod = MyBase.OpenMoveNextMethodImplementation(SpecialMember.System_Collections_IEnumerator__MoveNext, Accessibility.[Private])
			Me.GenerateMoveNextAndDispose(synthesizedMethod1, synthesizedMethod)
			Me.F.CurrentMethod = synthesizedMethod1
			If (Me._isEnumerable) Then
				Dim synthesizedMethod2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedMethod = MyBase.OpenMethodImplementation(Me.F.SpecialType(SpecialType.System_Collections_Generic_IEnumerable_T).Construct(New TypeSymbol() { Me._elementType }), SpecialMember.System_Collections_Generic_IEnumerable_T__GetEnumerator, "GetEnumerator", Accessibility.[Private], False, Nothing)
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement).GetInstance()
				Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Me.F.SynthesizedLocal(Me.StateMachineType, SynthesizedLocalKind.LoweringTemp, Nothing)
				Dim getMethod As MethodSymbol = Nothing
				Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = Me.F.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)(WellKnownMember.System_Environment__CurrentManagedThreadId, True)
				If (propertySymbol IsNot Nothing) Then
					getMethod = propertySymbol.GetMethod
				End If
				If (getMethod Is Nothing) Then
					boundExpression = Me.F.[Property](Me.F.[Property](WellKnownMember.System_Threading_Thread__CurrentThread), WellKnownMember.System_Threading_Thread__ManagedThreadId)
				Else
					boundExpression = Me.F.[Call](Nothing, getMethod)
				End If
				Dim generatedLabelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol = Me.F.GenerateLabel("thisInitialized")
				Dim boundStatements As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = instance
				Dim f As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = Me.F
				Dim boundBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator = Me.F.LogicalAndAlso(Me.F.IntEqual(Me.F.Field(Me.F.[Me](), Me.StateField, False), Me.F.Literal(StateMachineStates.FinishedStateMachine)), Me.F.IntEqual(Me.F.Field(Me.F.[Me](), Me._initialThreadIdField, False), boundExpression))
				Dim syntheticBoundNodeFactory As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = Me.F
				Dim boundStatementArray() As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = { Me.F.Assignment(Me.F.Field(Me.F.[Me](), Me.StateField, True), Me.F.Literal(StateMachineStates.FirstUnusedState)), Me.F.Assignment(Me.F.Local(localSymbol, True), Me.F.[Me]()), Nothing }
				If (Me.Method.IsShared OrElse Me.Method.MeParameter.Type.IsReferenceType) Then
					boundStatement = Me.F.[Goto](generatedLabelSymbol, True)
				Else
					boundStatement = Me.F.StatementList()
				End If
				boundStatementArray(2) = boundStatement
				boundStatements.Add(f.[If](boundBinaryOperator, syntheticBoundNodeFactory.Block(boundStatementArray), Me.F.Assignment(Me.F.Local(localSymbol, True), Me.F.[New](Me.StateMachineType.Constructor, New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { Me.F.Literal(0) }))))
				Dim initialParameters As Dictionary(Of Symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol) = Me.InitialParameters
				Dim symbols As Dictionary(Of Symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol) = Me.nonReusableLocalProxies
				If (Not Me.Method.IsShared) Then
					Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = Nothing
					If (symbols.TryGetValue(Me.Method.MeParameter, fieldSymbol)) Then
						instance.Add(Me.F.Assignment(Me.F.Field(Me.F.Local(localSymbol, True), fieldSymbol.AsMember(Me.StateMachineType), True), Me.F.Field(Me.F.[Me](), initialParameters(Me.Method.MeParameter).AsMember(Me.F.CurrentType), False)))
					End If
				End If
				instance.Add(Me.F.Label(generatedLabelSymbol))
				Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = Me.Method.Parameters.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As ParameterSymbol = enumerator.Current
					Dim fieldSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = Nothing
					If (Not symbols.TryGetValue(current, fieldSymbol1)) Then
						Continue While
					End If
					instance.Add(Me.F.Assignment(Me.F.Field(Me.F.Local(localSymbol, True), fieldSymbol1.AsMember(Me.StateMachineType), True), Me.F.Field(Me.F.[Me](), initialParameters(current).AsMember(Me.F.CurrentType), False)))
				End While
				instance.Add(Me.F.[Return](Me.F.Local(localSymbol, False)))
				Me.F.CloseMethod(Me.F.Block(ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)(localSymbol), instance.ToImmutableAndFree()))
				MyBase.OpenMethodImplementation(SpecialMember.System_Collections_IEnumerable__GetEnumerator, "IEnumerable.GetEnumerator", Accessibility.[Private], False, Nothing)
				Me.F.CloseMethod(Me.F.[Return](Me.F.[Call](Me.F.[Me](), synthesizedMethod2)))
			End If
			MyBase.OpenPropertyImplementation(Me.F.SpecialType(SpecialType.System_Collections_Generic_IEnumerator_T).Construct(New TypeSymbol() { Me._elementType }), SpecialMember.System_Collections_Generic_IEnumerator_T__Current, "Current", Accessibility.[Private])
			Me.F.CloseMethod(Me.F.[Return](Me.F.Field(Me.F.[Me](), Me._currentField, False)))
			MyBase.OpenMethodImplementation(SpecialMember.System_Collections_IEnumerator__Reset, "Reset", Accessibility.[Private], False, Nothing)
			Me.F.CloseMethod(Me.F.[Throw](Me.F.[New](Me.F.WellKnownType(WellKnownType.System_NotSupportedException))))
			MyBase.OpenPropertyImplementation(SpecialMember.System_Collections_IEnumerator__Current, "IEnumerator.Current", Accessibility.[Private])
			Me.F.CloseMethod(Me.F.[Return](Me.F.Field(Me.F.[Me](), Me._currentField, False)))
			Me.F.CurrentMethod = Me.StateMachineType.Constructor
			Dim instance1 As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement).GetInstance()
			instance1.Add(Me.F.BaseInitialization(New Microsoft.CodeAnalysis.VisualBasic.BoundExpression(-1) {}))
			Dim f1 As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = Me.F
			Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = Me.F.Field(Me.F.[Me](), Me.StateField, True)
			Dim syntheticBoundNodeFactory1 As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = Me.F
			Dim parameters As ImmutableArray(Of ParameterSymbol) = Me.F.CurrentMethod.Parameters
			instance1.Add(f1.Assignment(boundFieldAccess, syntheticBoundNodeFactory1.Parameter(parameters(0)).MakeRValue()))
			If (Me._isEnumerable) Then
				instance1.Add(Me.F.Assignment(Me.F.Field(Me.F.[Me](), Me._initialThreadIdField, True), boundExpression))
			End If
			instance1.Add(Me.F.[Return](Nothing))
			Me.F.CloseMethod(Me.F.Block(instance1.ToImmutableAndFree()))
			instance1 = Nothing
		End Sub

		Private Sub GenerateMoveNextAndDispose(ByVal moveNextMethod As SynthesizedMethod, ByVal disposeMethod As SynthesizedMethod)
			Dim iteratorMethodToClassRewriter As IteratorRewriter.IteratorMethodToClassRewriter = New IteratorRewriter.IteratorMethodToClassRewriter(Me.Method, Me.F, Me.StateField, Me._currentField, Me.hoistedVariables, Me.nonReusableLocalProxies, Me.SynthesizedLocalOrdinals, Me.SlotAllocatorOpt, Me.nextFreeHoistedLocalSlot, Me.Diagnostics)
			iteratorMethodToClassRewriter.GenerateMoveNextAndDispose(Me.Body, moveNextMethod, disposeMethod)
		End Sub

		Protected Overrides Function GenerateStateMachineCreation(ByVal stateMachineVariable As LocalSymbol, ByVal frameType As NamedTypeSymbol) As BoundStatement
			Return Me.F.[Return](Me.F.Local(stateMachineVariable, False))
		End Function

		Protected Overrides Sub InitializeParameterWithProxy(ByVal parameter As ParameterSymbol, ByVal proxy As FieldSymbol, ByVal stateMachineVariable As LocalSymbol, ByVal initializers As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression))
			Dim stateMachineType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (Me.Method.IsGenericMethod) Then
				stateMachineType = Me.StateMachineType.Construct(Me.Method.TypeArguments)
			Else
				stateMachineType = Me.StateMachineType
			End If
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = stateMachineType
			If (parameter.IsMe) Then
				boundExpression = Me.F.[Me]()
			Else
				boundExpression = Me.F.Parameter(parameter).MakeRValue()
			End If
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = boundExpression
			initializers.Add(Me.F.AssignmentExpression(Me.F.Field(Me.F.Local(stateMachineVariable, True), proxy.AsMember(namedTypeSymbol), True), boundExpression1))
		End Sub

		Protected Overrides Sub InitializeStateMachine(ByVal bodyBuilder As ArrayBuilder(Of BoundStatement), ByVal frameType As NamedTypeSymbol, ByVal stateMachineLocal As LocalSymbol)
			Dim num As Integer = If(Me._isEnumerable, StateMachineStates.FinishedStateMachine, StateMachineStates.FirstUnusedState)
			bodyBuilder.Add(Me.F.Assignment(Me.F.Local(stateMachineLocal, True), Me.F.[New](Me.StateMachineType.Constructor.AsMember(frameType), New BoundExpression() { Me.F.Literal(num) })))
		End Sub

		Friend Shared Function Rewrite(ByVal body As Microsoft.CodeAnalysis.VisualBasic.BoundBlock, ByVal method As MethodSymbol, ByVal methodOrdinal As Integer, ByVal slotAllocatorOpt As VariableSlotAllocator, ByVal compilationState As TypeCompilationState, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, <Out> ByRef stateMachineType As IteratorStateMachine) As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim item As TypeSymbol
			If (Not (body.HasErrors Or Not method.IsIterator)) Then
				Dim returnType As TypeSymbol = method.ReturnType
				Dim specialType As Microsoft.CodeAnalysis.SpecialType = method.ReturnType.OriginalDefinition.SpecialType
				Dim flag As Boolean = If(specialType = Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_IEnumerable_T, True, specialType = Microsoft.CodeAnalysis.SpecialType.System_Collections_IEnumerable)
				If (Not method.ReturnType.IsDefinition) Then
					item = DirectCast(returnType, NamedTypeSymbol).TypeArgumentsNoUseSiteDiagnostics(0)
				Else
					item = method.ContainingAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Object)
				End If
				stateMachineType = New IteratorStateMachine(slotAllocatorOpt, compilationState, method, methodOrdinal, item, flag)
				compilationState.ModuleBuilderOpt.CompilationState.SetStateMachineType(method, stateMachineType)
				Dim iteratorRewriter As Microsoft.CodeAnalysis.VisualBasic.IteratorRewriter = New Microsoft.CodeAnalysis.VisualBasic.IteratorRewriter(body, method, flag, stateMachineType, slotAllocatorOpt, compilationState, diagnostics)
				boundBlock = If(Not iteratorRewriter.EnsureAllSymbolsAndSignature(), iteratorRewriter.Rewrite(), body)
			Else
				boundBlock = body
			End If
			Return boundBlock
		End Function

		Private Class IteratorMethodToClassRewriter
			Inherits StateMachineRewriter(Of FieldSymbol).StateMachineMethodToClassRewriter
			Private ReadOnly _current As FieldSymbol

			Private _exitLabel As LabelSymbol

			Private _methodValue As LocalSymbol

			Private _tryNestingLevel As Integer

			Protected Overrides ReadOnly Property IsInExpressionLambda As Boolean
				Get
					Return False
				End Get
			End Property

			Protected Overrides ReadOnly Property ResumeLabelName As String
				Get
					Return "iteratorLabel"
				End Get
			End Property

			Friend Sub New(ByVal method As MethodSymbol, ByVal F As SyntheticBoundNodeFactory, ByVal state As FieldSymbol, ByVal current As FieldSymbol, ByVal hoistedVariables As IReadOnlySet(Of Symbol), ByVal localProxies As Dictionary(Of Symbol, FieldSymbol), ByVal SynthesizedLocalOrdinals As SynthesizedLocalOrdinalsDispenser, ByVal slotAllocatorOpt As VariableSlotAllocator, ByVal nextFreeHoistedLocalSlot As Integer, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
				MyBase.New(F, state, hoistedVariables, localProxies, SynthesizedLocalOrdinals, slotAllocatorOpt, nextFreeHoistedLocalSlot, diagnostics)
				Me._current = current
			End Sub

			Friend Overrides Sub AddProxyFieldsForStateMachineScope(ByVal proxy As FieldSymbol, ByVal proxyFields As ArrayBuilder(Of FieldSymbol))
				proxyFields.Add(proxy)
			End Sub

			Public Sub GenerateMoveNextAndDispose(ByVal Body As Microsoft.CodeAnalysis.VisualBasic.BoundStatement, ByVal moveNextMethod As SynthesizedMethod, ByVal disposeMethod As SynthesizedMethod)
				Dim func As Func(Of KeyValuePair(Of Integer, Integer), Boolean)
				Dim func1 As Func(Of KeyValuePair(Of Integer, Integer), Integer)
				Dim key As Func(Of KeyValuePair(Of Integer, Integer), Integer)
				Dim variable
				Me.F.CurrentMethod = moveNextMethod
				Dim resumeLabel As Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol = MyBase.AddState().ResumeLabel
				Me._methodValue = Me.F.SynthesizedLocal(Me.F.CurrentMethod.ReturnType, SynthesizedLocalKind.AsyncMethodReturnValue, Me.F.Syntax)
				Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = DirectCast(Me.Visit(Body), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
				Me.F.CloseMethod(Me.F.Block(ImmutableArray.Create(Of LocalSymbol)(Me._methodValue, Me.CachedState), New Microsoft.CodeAnalysis.VisualBasic.BoundStatement() { SyntheticBoundNodeFactory.HiddenSequencePoint(Nothing), Me.F.Assignment(Me.F.Local(Me.CachedState, True), Me.F.Field(Me.F.[Me](), Me.StateField, False)), MyBase.Dispatch(), Me.GenerateReturn(True), Me.F.Label(resumeLabel), Me.F.Assignment(Me.F.Field(Me.F.[Me](), Me.StateField, True), Me.F.AssignmentExpression(Me.F.Local(Me.CachedState, True), Me.F.Literal(StateMachineStates.NotStartedStateMachine))), boundStatement, Me.HandleReturn() }))
				Me._exitLabel = Nothing
				Me._methodValue = Nothing
				Me.F.CurrentMethod = disposeMethod
				Dim generatedLabelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol = Me.F.GenerateLabel("break")
				Dim finalizerStateMap As Dictionary(Of Integer, Integer) = Me.FinalizerStateMap
				If (IteratorRewriter.IteratorMethodToClassRewriter._Closure$__.$I5-0 Is Nothing) Then
					func = Function(ft As KeyValuePair(Of Integer, Integer)) ft.Value <> -1
					IteratorRewriter.IteratorMethodToClassRewriter._Closure$__.$I5-0 = func
				Else
					func = IteratorRewriter.IteratorMethodToClassRewriter._Closure$__.$I5-0
				End If
				Dim keyValuePairs As IEnumerable(Of KeyValuePair(Of Integer, Integer)) = finalizerStateMap.Where(func)
				If (IteratorRewriter.IteratorMethodToClassRewriter._Closure$__.$I5-1 Is Nothing) Then
					func1 = Function(ft As KeyValuePair(Of Integer, Integer)) ft.Value
					IteratorRewriter.IteratorMethodToClassRewriter._Closure$__.$I5-1 = func1
				Else
					func1 = IteratorRewriter.IteratorMethodToClassRewriter._Closure$__.$I5-1
				End If
				If (IteratorRewriter.IteratorMethodToClassRewriter._Closure$__.$I5-2 Is Nothing) Then
					key = Function(ft As KeyValuePair(Of Integer, Integer)) ft.Key
					IteratorRewriter.IteratorMethodToClassRewriter._Closure$__.$I5-2 = key
				Else
					key = IteratorRewriter.IteratorMethodToClassRewriter._Closure$__.$I5-2
				End If
				If (IteratorRewriter.IteratorMethodToClassRewriter._Closure$__.$I5-3 Is Nothing) Then
					variable = Function(Value As Integer, argument1 As IEnumerable(Of Integer)) New With { Key .Value = Value, Key .Group = argument1 }
					IteratorRewriter.IteratorMethodToClassRewriter._Closure$__.$I5-3 = variable
				Else
					variable = IteratorRewriter.IteratorMethodToClassRewriter._Closure$__.$I5-3
				End If
				Dim array As BoundCaseBlock() = keyValuePairs.GroupBy(func1, key, variable).[Select](Function(argument0) Me.F.SwitchSection(New List(Of Integer)(argument0.Group), New Microsoft.CodeAnalysis.VisualBasic.BoundStatement() { Me.F.Assignment(Me.F.Field(Me.F.[Me](), Me.StateField, True), Me.F.Literal(argument0.Value)), Me.F.[Goto](generatedLabelSymbol, True) })).ToArray()
				If (CInt(array.Length) <= 0) Then
					Me.F.CloseMethod(Me.F.[Return](Nothing))
					Return
				End If
				Me.F.CloseMethod(Me.F.Block(New Microsoft.CodeAnalysis.VisualBasic.BoundStatement() { Me.F.[Select](Me.F.Field(Me.F.[Me](), Me.StateField, False), array), Me.F.Assignment(Me.F.Field(Me.F.[Me](), Me.StateField, True), Me.F.Literal(StateMachineStates.NotStartedStateMachine)), Me.F.Label(generatedLabelSymbol), Me.F.ExpressionStatement(Me.F.[Call](Me.F.[Me](), moveNextMethod)), Me.F.[Return](Nothing) }))
			End Sub

			Protected Overrides Function GenerateReturn(ByVal finished As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
				Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
				Dim boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral = Me.F.Literal(Not finished)
				If (Me._tryNestingLevel <> 0) Then
					If (Me._exitLabel Is Nothing) Then
						Me._exitLabel = Me.F.GenerateLabel("exitLabel")
					End If
					boundStatement = Me.F.Block(New Microsoft.CodeAnalysis.VisualBasic.BoundStatement() { Me.F.Assignment(Me.F.Local(Me._methodValue, True), boundLiteral), Me.F.[Goto](Me._exitLabel, True) })
				Else
					boundStatement = Me.F.[Return](boundLiteral)
				End If
				Return boundStatement
			End Function

			Private Function HandleReturn() As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
				Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
				If (Me._exitLabel IsNot Nothing) Then
					boundStatement = Me.F.Block(New Microsoft.CodeAnalysis.VisualBasic.BoundStatement() { SyntheticBoundNodeFactory.HiddenSequencePoint(Nothing), Me.F.Assignment(Me.F.Local(Me._methodValue, True), Me.F.Literal(True)), Me.F.Label(Me._exitLabel), Me.F.[Return](Me.F.Local(Me._methodValue, False)) })
				Else
					boundStatement = Me.F.StatementList()
				End If
				Return boundStatement
			End Function

			Protected Overrides Function MaterializeProxy(ByVal origExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal proxy As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol) As BoundNode
				Dim syntax As SyntaxNode = Me.F.Syntax
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.FramePointer(syntax, proxy.ContainingType)
				Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = proxy.AsMember(DirectCast(boundExpression.Type, NamedTypeSymbol))
				Return Me.F.Field(boundExpression, fieldSymbol, origExpression.IsLValue)
			End Function

			Public Overrides Function VisitReturnStatement(ByVal node As BoundReturnStatement) As BoundNode
				Return Me.GenerateReturn(True)
			End Function

			Public Overrides Function VisitTryStatement(ByVal node As BoundTryStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Me._tryNestingLevel = Me._tryNestingLevel + 1
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = MyBase.VisitTryStatement(node)
				Me._tryNestingLevel = Me._tryNestingLevel - 1
				Return boundNode
			End Function

			Public Overrides Function VisitYieldStatement(ByVal node As BoundYieldStatement) As BoundNode
				Dim stateInfo As StateMachineRewriter(Of FieldSymbol).StateMachineMethodToClassRewriter.StateInfo = MyBase.AddState()
				Return Me.F.SequencePoint(node.Syntax, Me.F.Block(New BoundStatement() { Me.F.Assignment(Me.F.Field(Me.F.[Me](), Me._current, True), DirectCast(Me.Visit(node.Expression), BoundExpression)), Me.F.Assignment(Me.F.Field(Me.F.[Me](), Me.StateField, True), Me.F.AssignmentExpression(Me.F.Local(Me.CachedState, True), Me.F.Literal(stateInfo.Number))), Me.GenerateReturn(False), Me.F.Label(stateInfo.ResumeLabel), Me.F.Assignment(Me.F.Field(Me.F.[Me](), Me.StateField, True), Me.F.AssignmentExpression(Me.F.Local(Me.CachedState, True), Me.F.Literal(StateMachineStates.NotStartedStateMachine))) }))
			End Function
		End Class
	End Class
End Namespace