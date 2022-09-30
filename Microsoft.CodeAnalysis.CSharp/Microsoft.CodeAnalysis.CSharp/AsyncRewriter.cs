using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal class AsyncRewriter : StateMachineRewriter
    {
        private sealed class AsyncIteratorRewriter : AsyncRewriter
        {
            private FieldSymbol _promiseOfValueOrEndField;

            private FieldSymbol _currentField;

            private FieldSymbol _disposeModeField;

            private FieldSymbol _combinedTokensField;

            private readonly bool _isEnumerable;

            protected override bool PreserveInitialParameterValuesAndThreadId => _isEnumerable;

            internal AsyncIteratorRewriter(BoundStatement body, MethodSymbol method, int methodOrdinal, AsyncStateMachine stateMachineType, VariableSlotAllocator slotAllocatorOpt, TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
                : base(body, method, methodOrdinal, stateMachineType, slotAllocatorOpt, compilationState, diagnostics)
            {
                _isEnumerable = method.IsAsyncReturningIAsyncEnumerable(method.DeclaringCompilation);
            }

            protected override void VerifyPresenceOfRequiredAPIs(BindingDiagnosticBag bag)
            {
                base.VerifyPresenceOfRequiredAPIs(bag);
                if (_isEnumerable)
                {
                    EnsureWellKnownMember(WellKnownMember.System_Collections_Generic_IAsyncEnumerable_T__GetAsyncEnumerator, bag);
                    EnsureWellKnownMember(WellKnownMember.System_Threading_CancellationToken__Equals, bag);
                    EnsureWellKnownMember(WellKnownMember.System_Threading_CancellationTokenSource__CreateLinkedTokenSource, bag);
                    EnsureWellKnownMember(WellKnownMember.System_Threading_CancellationTokenSource__Token, bag);
                    EnsureWellKnownMember(WellKnownMember.System_Threading_CancellationTokenSource__Dispose, bag);
                }
                EnsureWellKnownMember(WellKnownMember.System_Collections_Generic_IAsyncEnumerator_T__MoveNextAsync, bag);
                EnsureWellKnownMember(WellKnownMember.System_Collections_Generic_IAsyncEnumerator_T__get_Current, bag);
                EnsureWellKnownMember(WellKnownMember.System_IAsyncDisposable__DisposeAsync, bag);
                EnsureWellKnownMember(WellKnownMember.System_Threading_Tasks_ValueTask_T__ctorSourceAndToken, bag);
                EnsureWellKnownMember(WellKnownMember.System_Threading_Tasks_ValueTask_T__ctorValue, bag);
                EnsureWellKnownMember(WellKnownMember.System_Threading_Tasks_ValueTask__ctor, bag);
                EnsureWellKnownMember(WellKnownMember.System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__GetResult, bag);
                EnsureWellKnownMember(WellKnownMember.System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__GetStatus, bag);
                EnsureWellKnownMember(WellKnownMember.System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__get_Version, bag);
                EnsureWellKnownMember(WellKnownMember.System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__OnCompleted, bag);
                EnsureWellKnownMember(WellKnownMember.System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__Reset, bag);
                EnsureWellKnownMember(WellKnownMember.System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__SetException, bag);
                EnsureWellKnownMember(WellKnownMember.System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__SetResult, bag);
                EnsureWellKnownMember(WellKnownMember.System_Threading_Tasks_Sources_IValueTaskSource_T__GetResult, bag);
                EnsureWellKnownMember(WellKnownMember.System_Threading_Tasks_Sources_IValueTaskSource_T__GetStatus, bag);
                EnsureWellKnownMember(WellKnownMember.System_Threading_Tasks_Sources_IValueTaskSource_T__OnCompleted, bag);
                EnsureWellKnownMember(WellKnownMember.System_Threading_Tasks_Sources_IValueTaskSource__GetResult, bag);
                EnsureWellKnownMember(WellKnownMember.System_Threading_Tasks_Sources_IValueTaskSource__GetStatus, bag);
                EnsureWellKnownMember(WellKnownMember.System_Threading_Tasks_Sources_IValueTaskSource__OnCompleted, bag);
            }

            protected override void GenerateMethodImplementations()
            {
                base.GenerateMethodImplementations();
                if (_isEnumerable)
                {
                    GenerateIAsyncEnumerableImplementation_GetAsyncEnumerator();
                }
                GenerateIAsyncEnumeratorImplementation_MoveNextAsync();
                GenerateIAsyncEnumeratorImplementation_Current();
                GenerateIValueTaskSourceBoolImplementation_GetResult();
                GenerateIValueTaskSourceBoolImplementation_GetStatus();
                GenerateIValueTaskSourceBoolImplementation_OnCompleted();
                GenerateIValueTaskSourceImplementation_GetResult();
                GenerateIValueTaskSourceImplementation_GetStatus();
                GenerateIValueTaskSourceImplementation_OnCompleted();
                GenerateIAsyncDisposable_DisposeAsync();
            }

            protected override void GenerateControlFields()
            {
                base.GenerateControlFields();
                NamedTypeSymbol namedTypeSymbol = F.SpecialType(SpecialType.System_Boolean);
                _promiseOfValueOrEndField = F.StateMachineField(F.WellKnownType(WellKnownType.System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T).Construct(namedTypeSymbol), GeneratedNames.MakeAsyncIteratorPromiseOfValueOrEndFieldName(), isPublic: true);
                TypeSymbol iteratorElementType = ((AsyncStateMachine)stateMachineType).IteratorElementType;
                _currentField = F.StateMachineField(iteratorElementType, GeneratedNames.MakeIteratorCurrentFieldName());
                _disposeModeField = F.StateMachineField(namedTypeSymbol, GeneratedNames.MakeDisposeModeFieldName());
                if (_isEnumerable && method.Parameters.Any((ParameterSymbol p) => p.IsSourceParameterWithEnumeratorCancellationAttribute()))
                {
                    _combinedTokensField = F.StateMachineField(F.WellKnownType(WellKnownType.System_Threading_CancellationTokenSource), GeneratedNames.MakeAsyncIteratorCombinedTokensFieldName());
                }
            }

            protected override void GenerateConstructor()
            {
                F.CurrentFunction = stateMachineType.Constructor;
                ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
                instance.Add(F.BaseInitialization());
                instance.Add(GenerateCreateAndAssignBuilder());
                instance.Add(F.Assignment(F.InstanceField(stateField), F.Parameter(F.CurrentFunction!.Parameters[0])));
                BoundExpression boundExpression = MakeCurrentThreadId();
                if (boundExpression != null && (object)initialThreadIdField != null)
                {
                    instance.Add(F.Assignment(F.InstanceField(initialThreadIdField), boundExpression));
                }
                instance.Add(F.Return());
                F.CloseMethod(F.Block(instance.ToImmutableAndFree()));
                instance = null;
            }

            private BoundExpressionStatement GenerateCreateAndAssignBuilder()
            {
                return F.Assignment(F.InstanceField(_builderField), F.StaticCall(null, _asyncMethodBuilderMemberCollection.CreateBuilder));
            }

            protected override void InitializeStateMachine(ArrayBuilder<BoundStatement> bodyBuilder, NamedTypeSymbol frameType, LocalSymbol stateMachineLocal)
            {
                int value = (_isEnumerable ? (-2) : (-3));
                bodyBuilder.Add(F.Assignment(F.Local(stateMachineLocal), F.New(stateMachineType.Constructor.AsMember(frameType), F.Literal(value))));
            }

            protected override BoundStatement InitializeParameterField(MethodSymbol getEnumeratorMethod, ParameterSymbol parameter, BoundExpression resultParameter, BoundExpression parameterProxy)
            {
                if ((object)_combinedTokensField != null && parameter.IsSourceParameterWithEnumeratorCancellationAttribute() && parameter.Type.Equals(F.Compilation.GetWellKnownType(WellKnownType.System_Threading_CancellationToken), TypeCompareKind.ConsiderEverything))
                {
                    BoundParameter boundParameter = F.Parameter(getEnumeratorMethod.Parameters[0]);
                    BoundFieldAccess boundFieldAccess = F.Field(F.This(), _combinedTokensField);
                    return F.If(F.Call(parameterProxy, WellKnownMember.System_Threading_CancellationToken__Equals, F.Default(parameterProxy.Type)), F.Assignment(resultParameter, boundParameter), F.If(F.LogicalOr(F.Call(boundParameter, WellKnownMember.System_Threading_CancellationToken__Equals, parameterProxy), F.Call(boundParameter, WellKnownMember.System_Threading_CancellationToken__Equals, F.Default(boundParameter.Type))), F.Assignment(resultParameter, parameterProxy), F.Block(F.Assignment(boundFieldAccess, F.StaticCall(WellKnownMember.System_Threading_CancellationTokenSource__CreateLinkedTokenSource, parameterProxy, boundParameter)), F.Assignment(resultParameter, F.Property(boundFieldAccess, WellKnownMember.System_Threading_CancellationTokenSource__Token)))));
                }
                return F.Assignment(resultParameter, parameterProxy);
            }

            protected override BoundStatement GenerateStateMachineCreation(LocalSymbol stateMachineVariable, NamedTypeSymbol frameType, IReadOnlyDictionary<Symbol, CapturedSymbolReplacement> proxies)
            {
                ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
                instance.Add(GenerateParameterStorage(stateMachineVariable, proxies));
                instance.Add(F.Return(F.Local(stateMachineVariable)));
                return F.Block(instance.ToImmutableAndFree());
            }

            private void GenerateIAsyncEnumeratorImplementation_MoveNextAsync()
            {
                NamedTypeSymbol newOwner = F.WellKnownType(WellKnownType.System_Collections_Generic_IAsyncEnumerator_T).Construct(_currentField.Type);
                MethodSymbol methodSymbol = F.WellKnownMethod(WellKnownMember.System_Collections_Generic_IAsyncEnumerator_T__MoveNextAsync).AsMember(newOwner);
                NamedTypeSymbol newOwner2 = (NamedTypeSymbol)_promiseOfValueOrEndField.Type;
                MethodSymbol methodSymbol2 = F.WellKnownMethod(WellKnownMember.System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__GetStatus).AsMember(newOwner2);
                MethodSymbol methodSymbol3 = F.WellKnownMethod(WellKnownMember.System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__GetResult).AsMember(newOwner2);
                NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)methodSymbol.ReturnType;
                MethodSymbol ctor = F.WellKnownMethod(WellKnownMember.System_Threading_Tasks_ValueTask_T__ctorValue).AsMember(namedTypeSymbol);
                MethodSymbol ctor2 = F.WellKnownMethod(WellKnownMember.System_Threading_Tasks_ValueTask_T__ctorSourceAndToken).AsMember(namedTypeSymbol);
                OpenMethodImplementation(methodSymbol);
                GetPartsForStartingMachine(out var callReset, out var instSymbol, out var instAssignment, out var startCall, out var promise_get_Version);
                BoundStatement boundStatement = F.If(F.IntEqual(F.InstanceField(stateField), F.Literal(-2)), F.Return(F.Default(namedTypeSymbol)));
                LocalSymbol localSymbol = F.SynthesizedLocal(F.SpecialType(SpecialType.System_Int16));
                BoundLocal boundLocal = F.Local(localSymbol);
                BoundExpressionStatement boundExpressionStatement = F.Assignment(boundLocal, F.Call(F.Field(F.This(), _promiseOfValueOrEndField), promise_get_Version));
                BoundStatement boundStatement2 = F.If(F.IntEqual(F.Call(F.Field(F.This(), _promiseOfValueOrEndField), methodSymbol2, boundLocal), F.Literal(1)), F.Return(F.New(ctor, F.Call(F.Field(F.This(), _promiseOfValueOrEndField), methodSymbol3, boundLocal))));
                BoundReturnStatement boundReturnStatement = F.Return(F.New(ctor2, F.This(), boundLocal));
                F.CloseMethod(F.Block(ImmutableArray.Create(instSymbol, localSymbol), boundStatement, callReset, instAssignment, startCall, boundExpressionStatement, boundStatement2, boundReturnStatement));
            }

            private void GetPartsForStartingMachine(out BoundExpressionStatement callReset, out LocalSymbol instSymbol, out BoundStatement instAssignment, out BoundExpressionStatement startCall, out MethodSymbol promise_get_Version)
            {
                BoundFieldAccess receiver = F.InstanceField(_promiseOfValueOrEndField);
                MethodSymbol methodSymbol = (MethodSymbol)F.WellKnownMethod(WellKnownMember.System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__Reset, isOptional: true).SymbolAsMember((NamedTypeSymbol)_promiseOfValueOrEndField.Type);
                callReset = F.ExpressionStatement(F.Call(receiver, methodSymbol));
                MethodSymbol methodSymbol2 = _asyncMethodBuilderMemberCollection.Start.Construct(stateMachineType);
                instSymbol = F.SynthesizedLocal(stateMachineType);
                BoundLocal boundLocal = F.Local(instSymbol);
                instAssignment = F.Assignment(boundLocal, F.This());
                startCall = F.ExpressionStatement(F.Call(F.InstanceField(_builderField), methodSymbol2, ImmutableArray.Create((BoundExpression)boundLocal)));
                promise_get_Version = F.WellKnownMethod(WellKnownMember.System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__get_Version).AsMember((NamedTypeSymbol)_promiseOfValueOrEndField.Type);
            }

            private void GenerateIAsyncDisposable_DisposeAsync()
            {
                MethodSymbol methodSymbol = F.WellKnownMethod(WellKnownMember.System_IAsyncDisposable__DisposeAsync);
                OpenMethodImplementation(methodSymbol);
                TypeSymbol returnType = methodSymbol.ReturnType;
                GetPartsForStartingMachine(out var callReset, out var instSymbol, out var instAssignment, out var startCall, out var promise_get_Version);
                BoundStatement boundStatement = F.If(F.IntGreaterThanOrEqual(F.InstanceField(stateField), F.Literal(-1)), F.Throw(F.New(F.WellKnownType(WellKnownType.System_NotSupportedException))));
                BoundStatement boundStatement2 = F.If(F.IntEqual(F.InstanceField(stateField), F.Literal(-2)), F.Return(F.Default(returnType)));
                MethodSymbol ctor = F.WellKnownMethod(WellKnownMember.System_Threading_Tasks_ValueTask__ctor).AsMember((NamedTypeSymbol)methodSymbol.ReturnType);
                BoundReturnStatement boundReturnStatement = F.Return(F.New(ctor, F.This(), F.Call(F.InstanceField(_promiseOfValueOrEndField), promise_get_Version)));
                F.CloseMethod(F.Block(ImmutableArray.Create(instSymbol), boundStatement, boundStatement2, F.Assignment(F.InstanceField(_disposeModeField), F.Literal(value: true)), callReset, instAssignment, startCall, boundReturnStatement));
            }

            private void GenerateIAsyncEnumeratorImplementation_Current()
            {
                NamedTypeSymbol newOwner = F.WellKnownType(WellKnownType.System_Collections_Generic_IAsyncEnumerator_T).Construct(_currentField.Type);
                MethodSymbol getterToImplement = F.WellKnownMethod(WellKnownMember.System_Collections_Generic_IAsyncEnumerator_T__get_Current).AsMember(newOwner);
                OpenPropertyImplementation(getterToImplement);
                F.CloseMethod(F.Block(F.Return(F.InstanceField(_currentField))));
            }

            private void GenerateIValueTaskSourceBoolImplementation_GetResult()
            {
                NamedTypeSymbol newOwner = F.WellKnownType(WellKnownType.System_Threading_Tasks_Sources_IValueTaskSource_T).Construct(F.SpecialType(SpecialType.System_Boolean));
                MethodSymbol methodSymbol = F.WellKnownMethod(WellKnownMember.System_Threading_Tasks_Sources_IValueTaskSource_T__GetResult).AsMember(newOwner);
                MethodSymbol methodSymbol2 = F.WellKnownMethod(WellKnownMember.System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__GetResult).AsMember((NamedTypeSymbol)_promiseOfValueOrEndField.Type);
                OpenMethodImplementation(methodSymbol);
                F.CloseMethod(F.Return(F.Call(F.InstanceField(_promiseOfValueOrEndField), methodSymbol2, F.Parameter(methodSymbol.Parameters[0]))));
            }

            private void GenerateIValueTaskSourceBoolImplementation_GetStatus()
            {
                NamedTypeSymbol newOwner = F.WellKnownType(WellKnownType.System_Threading_Tasks_Sources_IValueTaskSource_T).Construct(F.SpecialType(SpecialType.System_Boolean));
                MethodSymbol methodSymbol = F.WellKnownMethod(WellKnownMember.System_Threading_Tasks_Sources_IValueTaskSource_T__GetStatus).AsMember(newOwner);
                MethodSymbol methodSymbol2 = F.WellKnownMethod(WellKnownMember.System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__GetStatus).AsMember((NamedTypeSymbol)_promiseOfValueOrEndField.Type);
                OpenMethodImplementation(methodSymbol);
                F.CloseMethod(F.Return(F.Call(F.InstanceField(_promiseOfValueOrEndField), methodSymbol2, F.Parameter(methodSymbol.Parameters[0]))));
            }

            private void GenerateIValueTaskSourceBoolImplementation_OnCompleted()
            {
                NamedTypeSymbol newOwner = F.WellKnownType(WellKnownType.System_Threading_Tasks_Sources_IValueTaskSource_T).Construct(F.SpecialType(SpecialType.System_Boolean));
                MethodSymbol methodSymbol = F.WellKnownMethod(WellKnownMember.System_Threading_Tasks_Sources_IValueTaskSource_T__OnCompleted).AsMember(newOwner);
                MethodSymbol methodSymbol2 = F.WellKnownMethod(WellKnownMember.System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__OnCompleted).AsMember((NamedTypeSymbol)_promiseOfValueOrEndField.Type);
                OpenMethodImplementation(methodSymbol);
                F.CloseMethod(F.Block(F.ExpressionStatement(F.Call(F.InstanceField(_promiseOfValueOrEndField), methodSymbol2, F.Parameter(methodSymbol.Parameters[0]), F.Parameter(methodSymbol.Parameters[1]), F.Parameter(methodSymbol.Parameters[2]), F.Parameter(methodSymbol.Parameters[3]))), F.Return()));
            }

            private void GenerateIValueTaskSourceImplementation_GetResult()
            {
                MethodSymbol methodSymbol = F.WellKnownMethod(WellKnownMember.System_Threading_Tasks_Sources_IValueTaskSource__GetResult);
                MethodSymbol methodSymbol2 = F.WellKnownMethod(WellKnownMember.System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__GetResult).AsMember((NamedTypeSymbol)_promiseOfValueOrEndField.Type);
                OpenMethodImplementation(methodSymbol);
                F.CloseMethod(F.Block(F.ExpressionStatement(F.Call(F.InstanceField(_promiseOfValueOrEndField), methodSymbol2, F.Parameter(methodSymbol.Parameters[0]))), F.Return()));
            }

            private void GenerateIValueTaskSourceImplementation_GetStatus()
            {
                MethodSymbol methodSymbol = F.WellKnownMethod(WellKnownMember.System_Threading_Tasks_Sources_IValueTaskSource__GetStatus);
                MethodSymbol methodSymbol2 = F.WellKnownMethod(WellKnownMember.System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__GetStatus).AsMember((NamedTypeSymbol)_promiseOfValueOrEndField.Type);
                OpenMethodImplementation(methodSymbol);
                F.CloseMethod(F.Return(F.Call(F.InstanceField(_promiseOfValueOrEndField), methodSymbol2, F.Parameter(methodSymbol.Parameters[0]))));
            }

            private void GenerateIValueTaskSourceImplementation_OnCompleted()
            {
                MethodSymbol methodSymbol = F.WellKnownMethod(WellKnownMember.System_Threading_Tasks_Sources_IValueTaskSource__OnCompleted);
                MethodSymbol methodSymbol2 = F.WellKnownMethod(WellKnownMember.System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__OnCompleted).AsMember((NamedTypeSymbol)_promiseOfValueOrEndField.Type);
                OpenMethodImplementation(methodSymbol);
                F.CloseMethod(F.Block(F.ExpressionStatement(F.Call(F.InstanceField(_promiseOfValueOrEndField), methodSymbol2, F.Parameter(methodSymbol.Parameters[0]), F.Parameter(methodSymbol.Parameters[1]), F.Parameter(methodSymbol.Parameters[2]), F.Parameter(methodSymbol.Parameters[3]))), F.Return()));
            }

            private void GenerateIAsyncEnumerableImplementation_GetAsyncEnumerator()
            {
                NamedTypeSymbol newOwner = F.WellKnownType(WellKnownType.System_Collections_Generic_IAsyncEnumerable_T).Construct(_currentField.Type);
                MethodSymbol getEnumeratorMethod = F.WellKnownMethod(WellKnownMember.System_Collections_Generic_IAsyncEnumerable_T__GetAsyncEnumerator).AsMember(newOwner);
                BoundExpression managedThreadId = null;
                GenerateIteratorGetEnumerator(getEnumeratorMethod, ref managedThreadId, -3);
            }

            protected override void GenerateResetInstance(ArrayBuilder<BoundStatement> builder, int initialState)
            {
                builder.Add(F.Assignment(F.Field(F.This(), stateField), F.Literal(initialState)));
                builder.Add(GenerateCreateAndAssignBuilder());
                builder.Add(F.Assignment(F.InstanceField(_disposeModeField), F.Literal(value: false)));
            }

            protected override void GenerateMoveNext(SynthesizedImplementationMethod moveNextMethod)
            {
                MethodSymbol methodSymbol = F.WellKnownMethod(WellKnownMember.System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__SetResult, isOptional: true);
                if ((object)methodSymbol != null)
                {
                    methodSymbol = (MethodSymbol)methodSymbol.SymbolAsMember((NamedTypeSymbol)_promiseOfValueOrEndField.Type);
                }
                MethodSymbol methodSymbol2 = F.WellKnownMethod(WellKnownMember.System_Threading_Tasks_Sources_ManualResetValueTaskSourceCore_T__SetException, isOptional: true);
                if ((object)methodSymbol2 != null)
                {
                    methodSymbol2 = (MethodSymbol)methodSymbol2.SymbolAsMember((NamedTypeSymbol)_promiseOfValueOrEndField.Type);
                }
                new AsyncIteratorMethodToStateMachineRewriter(method, _methodOrdinal, _asyncMethodBuilderMemberCollection, new AsyncIteratorInfo(_promiseOfValueOrEndField, _combinedTokensField, _currentField, _disposeModeField, methodSymbol, methodSymbol2), F, stateField, _builderField, hoistedVariables, nonReusableLocalProxies, synthesizedLocalOrdinals, slotAllocatorOpt, nextFreeHoistedLocalSlot, diagnostics).GenerateMoveNext(body, moveNextMethod);
            }
        }

        private class AwaitDetector : BoundTreeWalkerWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator
        {
            private bool _sawAwait;

            public static bool ContainsAwait(BoundNode node)
            {
                AwaitDetector awaitDetector = new AwaitDetector();
                awaitDetector.Visit(node);
                return awaitDetector._sawAwait;
            }

            public override BoundNode VisitAwaitExpression(BoundAwaitExpression node)
            {
                _sawAwait = true;
                return null;
            }
        }

        private readonly AsyncMethodBuilderMemberCollection _asyncMethodBuilderMemberCollection;

        private readonly bool _constructedSuccessfully;

        private readonly int _methodOrdinal;

        private FieldSymbol _builderField;

        protected override bool PreserveInitialParameterValuesAndThreadId => false;

        private AsyncRewriter(BoundStatement body, MethodSymbol method, int methodOrdinal, AsyncStateMachine stateMachineType, VariableSlotAllocator slotAllocatorOpt, TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
            : base(body, method, stateMachineType, slotAllocatorOpt, compilationState, diagnostics)
        {
            _constructedSuccessfully = AsyncMethodBuilderMemberCollection.TryCreate(F, method, base.stateMachineType.TypeMap, out _asyncMethodBuilderMemberCollection);
            _methodOrdinal = methodOrdinal;
        }

        internal static BoundStatement Rewrite(BoundStatement bodyWithAwaitLifted, MethodSymbol method, int methodOrdinal, VariableSlotAllocator slotAllocatorOpt, TypeCompilationState compilationState, BindingDiagnosticBag diagnostics, out AsyncStateMachine stateMachineType)
        {
            if (!method.IsAsync)
            {
                stateMachineType = null;
                return bodyWithAwaitLifted;
            }
            CSharpCompilation declaringCompilation = method.DeclaringCompilation;
            bool flag = method.IsAsyncReturningIAsyncEnumerable(declaringCompilation) || method.IsAsyncReturningIAsyncEnumerator(declaringCompilation);
            if (flag && !method.IsIterator)
            {
                bool flag2 = AwaitDetector.ContainsAwait(bodyWithAwaitLifted);
                diagnostics.Add(flag2 ? ErrorCode.ERR_PossibleAsyncIteratorWithoutYield : ErrorCode.ERR_PossibleAsyncIteratorWithoutYieldOrAwait, method.Locations[0], method.ReturnTypeWithAnnotations);
                stateMachineType = null;
                return bodyWithAwaitLifted;
            }
            TypeKind typeKind = ((compilationState.Compilation.Options.EnableEditAndContinue || method.IsIterator) ? TypeKind.Class : TypeKind.Struct);
            stateMachineType = new AsyncStateMachine(slotAllocatorOpt, compilationState, method, methodOrdinal, typeKind);
            compilationState.ModuleBuilderOpt!.CompilationState.SetStateMachineType(method, stateMachineType);
            AsyncRewriter asyncRewriter = (flag ? new AsyncIteratorRewriter(bodyWithAwaitLifted, method, methodOrdinal, stateMachineType, slotAllocatorOpt, compilationState, diagnostics) : new AsyncRewriter(bodyWithAwaitLifted, method, methodOrdinal, stateMachineType, slotAllocatorOpt, compilationState, diagnostics));
            if (!asyncRewriter.VerifyPresenceOfRequiredAPIs())
            {
                return bodyWithAwaitLifted;
            }
            try
            {
                return asyncRewriter.Rewrite();
            }
            catch (SyntheticBoundNodeFactory.MissingPredefinedMember missingPredefinedMember)
            {
                diagnostics.Add(missingPredefinedMember.Diagnostic);
                return new BoundBadStatement(bodyWithAwaitLifted.Syntax, ImmutableArray.Create((BoundNode)bodyWithAwaitLifted), hasErrors: true);
            }
        }

        protected bool VerifyPresenceOfRequiredAPIs()
        {
            BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance(withDiagnostics: true, diagnostics.AccumulatesDependencies);
            VerifyPresenceOfRequiredAPIs(instance);
            bool num = instance.HasAnyErrors();
            if (!num)
            {
                diagnostics.AddDependencies(instance);
            }
            else
            {
                diagnostics.AddRange(instance);
            }
            instance.Free();
            if (!num)
            {
                return _constructedSuccessfully;
            }
            return false;
        }

        protected virtual void VerifyPresenceOfRequiredAPIs(BindingDiagnosticBag bag)
        {
            EnsureWellKnownMember(WellKnownMember.System_Runtime_CompilerServices_IAsyncStateMachine_MoveNext, bag);
            EnsureWellKnownMember(WellKnownMember.System_Runtime_CompilerServices_IAsyncStateMachine_SetStateMachine, bag);
        }

        private Symbol EnsureWellKnownMember(WellKnownMember member, BindingDiagnosticBag bag)
        {
            return Binder.GetWellKnownTypeMember(F.Compilation, member, bag, body.Syntax.Location);
        }

        protected override void GenerateControlFields()
        {
            stateField = F.StateMachineField(F.SpecialType(SpecialType.System_Int32), GeneratedNames.MakeStateMachineStateFieldName(), isPublic: true);
            _builderField = F.StateMachineField(_asyncMethodBuilderMemberCollection.BuilderType, GeneratedNames.AsyncBuilderFieldName(), isPublic: true);
        }

        protected override void GenerateMethodImplementations()
        {
            MethodSymbol methodToImplement = F.WellKnownMethod(WellKnownMember.System_Runtime_CompilerServices_IAsyncStateMachine_MoveNext);
            MethodSymbol methodToImplement2 = F.WellKnownMethod(WellKnownMember.System_Runtime_CompilerServices_IAsyncStateMachine_SetStateMachine);
            SynthesizedImplementationMethod moveNextMethod = OpenMoveNextMethodImplementation(methodToImplement);
            GenerateMoveNext(moveNextMethod);
            OpenMethodImplementation(methodToImplement2, "SetStateMachine");
            if (F.CurrentType!.TypeKind == TypeKind.Class)
            {
                F.CloseMethod(F.Return());
            }
            else
            {
                F.CloseMethod(F.Block(F.ExpressionStatement(F.Call(F.Field(F.This(), _builderField), _asyncMethodBuilderMemberCollection.SetStateMachine, new BoundExpression[1] { F.Parameter(F.CurrentFunction!.Parameters[0]) })), F.Return()));
            }
            GenerateConstructor();
        }

        protected virtual void GenerateConstructor()
        {
            if (stateMachineType.TypeKind == TypeKind.Class)
            {
                F.CurrentFunction = stateMachineType.Constructor;
                F.CloseMethod(F.Block(ImmutableArray.Create(F.BaseInitialization(), F.Return())));
            }
        }

        protected override void InitializeStateMachine(ArrayBuilder<BoundStatement> bodyBuilder, NamedTypeSymbol frameType, LocalSymbol stateMachineLocal)
        {
            if (frameType.TypeKind == TypeKind.Class)
            {
                bodyBuilder.Add(F.Assignment(F.Local(stateMachineLocal), F.New(frameType.InstanceConstructors[0])));
            }
        }

        protected override BoundStatement GenerateStateMachineCreation(LocalSymbol stateMachineVariable, NamedTypeSymbol frameType, IReadOnlyDictionary<Symbol, CapturedSymbolReplacement> proxies)
        {
            if (!AsyncMethodBuilderMemberCollection.TryCreate(F, method, null, out var collection))
            {
                return new BoundBadStatement(F.Syntax, ImmutableArray<BoundNode>.Empty, hasErrors: true);
            }
            ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
            instance.Add(F.Assignment(F.Field(F.Local(stateMachineVariable), _builderField.AsMember(frameType)), F.StaticCall(null, collection.CreateBuilder)));
            instance.Add(GenerateParameterStorage(stateMachineVariable, proxies));
            instance.Add(F.Assignment(F.Field(F.Local(stateMachineVariable), stateField.AsMember(frameType)), F.Literal(-1)));
            MethodSymbol methodSymbol = collection.Start.Construct(frameType);
            if (collection.CheckGenericMethodConstraints)
            {
                ConstraintsHelper.CheckConstraintsArgs args = new ConstraintsHelper.CheckConstraintsArgs(F.Compilation, F.Compilation.Conversions, includeNullability: false, F.Syntax.Location, diagnostics);
                methodSymbol.CheckConstraints(in args);
            }
            instance.Add(F.ExpressionStatement(F.Call(F.Field(F.Local(stateMachineVariable), _builderField.AsMember(frameType)), methodSymbol, ImmutableArray.Create((BoundExpression)F.Local(stateMachineVariable)))));
            instance.Add(method.IsAsyncReturningVoid() ? F.Return() : F.Return(F.Property(F.Field(F.Local(stateMachineVariable), _builderField.AsMember(frameType)), collection.Task)));
            return F.Block(instance.ToImmutableAndFree());
        }

        protected virtual void GenerateMoveNext(SynthesizedImplementationMethod moveNextMethod)
        {
            new AsyncMethodToStateMachineRewriter(method, _methodOrdinal, _asyncMethodBuilderMemberCollection, F, stateField, _builderField, hoistedVariables, nonReusableLocalProxies, synthesizedLocalOrdinals, slotAllocatorOpt, nextFreeHoistedLocalSlot, diagnostics).GenerateMoveNext(body, moveNextMethod);
        }
    }
}
