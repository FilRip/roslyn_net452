using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class IteratorRewriter : StateMachineRewriter<FieldSymbol>
	{
		private class IteratorMethodToClassRewriter : StateMachineMethodToClassRewriter
		{
			private readonly FieldSymbol _current;

			private LabelSymbol _exitLabel;

			private LocalSymbol _methodValue;

			private int _tryNestingLevel;

			protected override bool IsInExpressionLambda => false;

			protected override string ResumeLabelName => "iteratorLabel";

			internal IteratorMethodToClassRewriter(MethodSymbol method, SyntheticBoundNodeFactory F, FieldSymbol state, FieldSymbol current, IReadOnlySet<Symbol> hoistedVariables, Dictionary<Symbol, FieldSymbol> localProxies, SynthesizedLocalOrdinalsDispenser SynthesizedLocalOrdinals, VariableSlotAllocator slotAllocatorOpt, int nextFreeHoistedLocalSlot, BindingDiagnosticBag diagnostics)
				: base(F, state, hoistedVariables, localProxies, SynthesizedLocalOrdinals, slotAllocatorOpt, nextFreeHoistedLocalSlot, diagnostics)
			{
				_current = current;
			}

			public void GenerateMoveNextAndDispose(BoundStatement Body, SynthesizedMethod moveNextMethod, SynthesizedMethod disposeMethod)
			{
				F.CurrentMethod = moveNextMethod;
				GeneratedLabelSymbol resumeLabel = AddState().ResumeLabel;
				_methodValue = F.SynthesizedLocal(F.CurrentMethod.ReturnType, SynthesizedLocalKind.AsyncMethodReturnValue, F.Syntax);
				BoundStatement boundStatement = (BoundStatement)Visit(Body);
				F.CloseMethod(F.Block(ImmutableArray.Create(_methodValue, CachedState), SyntheticBoundNodeFactory.HiddenSequencePoint(), F.Assignment(F.Local(CachedState, isLValue: true), F.Field(F.Me(), StateField, isLValue: false)), Dispatch(), GenerateReturn(finished: true), F.Label(resumeLabel), F.Assignment(F.Field(F.Me(), StateField, isLValue: true), F.AssignmentExpression(F.Local(CachedState, isLValue: true), F.Literal(StateMachineStates.NotStartedStateMachine))), boundStatement, HandleReturn()));
				_exitLabel = null;
				_methodValue = null;
				F.CurrentMethod = disposeMethod;
				GeneratedLabelSymbol generatedLabelSymbol = F.GenerateLabel("break");
				BoundCaseBlock[] array = (from _0024VB_0024It in FinalizerStateMap.Where((KeyValuePair<int, int> ft) => ft.Value != -1).GroupBy((KeyValuePair<int, int> ft) => ft.Value, (KeyValuePair<int, int> ft) => ft.Key, (int Value, IEnumerable<int> _0024VB_0024ItAnonymous) => new VB_0024AnonymousType_0<int, IEnumerable<int>>(Value, _0024VB_0024ItAnonymous))
					select F.SwitchSection(new List<int>(_0024VB_0024It.Group), F.Assignment(F.Field(F.Me(), StateField, isLValue: true), F.Literal(_0024VB_0024It.Value)), F.Goto(generatedLabelSymbol))).ToArray();
				if (array.Length > 0)
				{
					F.CloseMethod(F.Block(F.Select(F.Field(F.Me(), StateField, isLValue: false), array), F.Assignment(F.Field(F.Me(), StateField, isLValue: true), F.Literal(StateMachineStates.NotStartedStateMachine)), F.Label(generatedLabelSymbol), F.ExpressionStatement(F.Call(F.Me(), moveNextMethod)), F.Return()));
				}
				else
				{
					F.CloseMethod(F.Return());
				}
			}

			private BoundStatement HandleReturn()
			{
				if ((object)_exitLabel == null)
				{
					return F.StatementList();
				}
				return F.Block(SyntheticBoundNodeFactory.HiddenSequencePoint(), F.Assignment(F.Local(_methodValue, isLValue: true), F.Literal(value: true)), F.Label(_exitLabel), F.Return(F.Local(_methodValue, isLValue: false)));
			}

			protected override BoundStatement GenerateReturn(bool finished)
			{
				BoundLiteral boundLiteral = F.Literal(!finished);
				if (_tryNestingLevel == 0)
				{
					return F.Return(boundLiteral);
				}
				if ((object)_exitLabel == null)
				{
					_exitLabel = F.GenerateLabel("exitLabel");
				}
				return F.Block(F.Assignment(F.Local(_methodValue, isLValue: true), boundLiteral), F.Goto(_exitLabel));
			}

			public override BoundNode VisitTryStatement(BoundTryStatement node)
			{
				_tryNestingLevel++;
				BoundNode result = base.VisitTryStatement(node);
				_tryNestingLevel--;
				return result;
			}

			public override BoundNode VisitReturnStatement(BoundReturnStatement node)
			{
				return GenerateReturn(finished: true);
			}

			public override BoundNode VisitYieldStatement(BoundYieldStatement node)
			{
				StateInfo stateInfo = AddState();
				return F.SequencePoint(node.Syntax, F.Block(F.Assignment(F.Field(F.Me(), _current, isLValue: true), (BoundExpression)Visit(node.Expression)), F.Assignment(F.Field(F.Me(), StateField, isLValue: true), F.AssignmentExpression(F.Local(CachedState, isLValue: true), F.Literal(stateInfo.Number))), GenerateReturn(finished: false), F.Label(stateInfo.ResumeLabel), F.Assignment(F.Field(F.Me(), StateField, isLValue: true), F.AssignmentExpression(F.Local(CachedState, isLValue: true), F.Literal(StateMachineStates.NotStartedStateMachine)))));
			}

			internal override void AddProxyFieldsForStateMachineScope(FieldSymbol proxy, ArrayBuilder<FieldSymbol> proxyFields)
			{
				proxyFields.Add(proxy);
			}

			protected override BoundNode MaterializeProxy(BoundExpression origExpression, FieldSymbol proxy)
			{
				SyntaxNode syntax = F.Syntax;
				BoundExpression boundExpression = FramePointer(syntax, proxy.ContainingType);
				FieldSymbol f = proxy.AsMember((NamedTypeSymbol)boundExpression.Type);
				return F.Field(boundExpression, f, origExpression.IsLValue);
			}
		}

		private readonly TypeSymbol _elementType;

		private readonly bool _isEnumerable;

		private FieldSymbol _currentField;

		private FieldSymbol _initialThreadIdField;

		protected override bool PreserveInitialParameterValues => _isEnumerable;

		internal override TypeSubstitution TypeMap => StateMachineType.TypeSubstitution;

		public IteratorRewriter(BoundStatement body, MethodSymbol method, bool isEnumerable, IteratorStateMachine stateMachineType, VariableSlotAllocator slotAllocatorOpt, TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
			: base(body, method, (StateMachineTypeSymbol)stateMachineType, slotAllocatorOpt, compilationState, diagnostics)
		{
			_isEnumerable = isEnumerable;
			TypeSymbol returnType = method.ReturnType;
			if (SymbolExtensions.GetArity(returnType) == 0)
			{
				_elementType = method.ContainingAssembly.GetSpecialType(SpecialType.System_Object);
			}
			else
			{
				_elementType = ((NamedTypeSymbol)returnType).TypeArgumentsNoUseSiteDiagnostics.Single().InternalSubstituteTypeParameters(TypeMap).Type;
			}
		}

		internal static BoundBlock Rewrite(BoundBlock body, MethodSymbol method, int methodOrdinal, VariableSlotAllocator slotAllocatorOpt, TypeCompilationState compilationState, BindingDiagnosticBag diagnostics, out IteratorStateMachine stateMachineType)
		{
			if (body.HasErrors | !method.IsIterator)
			{
				return body;
			}
			TypeSymbol returnType = method.ReturnType;
			SpecialType specialType = method.ReturnType.OriginalDefinition.SpecialType;
			bool isEnumerable = specialType == SpecialType.System_Collections_Generic_IEnumerable_T || specialType == SpecialType.System_Collections_IEnumerable;
			TypeSymbol valueTypeSymbol = ((!method.ReturnType.IsDefinition) ? ((NamedTypeSymbol)returnType).TypeArgumentsNoUseSiteDiagnostics[0] : method.ContainingAssembly.GetSpecialType(SpecialType.System_Object));
			stateMachineType = new IteratorStateMachine(slotAllocatorOpt, compilationState, method, methodOrdinal, valueTypeSymbol, isEnumerable);
			compilationState.ModuleBuilderOpt.CompilationState.SetStateMachineType(method, stateMachineType);
			IteratorRewriter iteratorRewriter = new IteratorRewriter(body, method, isEnumerable, stateMachineType, slotAllocatorOpt, compilationState, diagnostics);
			if (iteratorRewriter.EnsureAllSymbolsAndSignature())
			{
				return body;
			}
			return iteratorRewriter.Rewrite();
		}

		internal override bool EnsureAllSymbolsAndSignature()
		{
			if (base.EnsureAllSymbolsAndSignature() || Method.IsSub || TypeSymbolExtensions.IsErrorType(_elementType))
			{
				return true;
			}
			BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance(withDiagnostics: true, Diagnostics.AccumulatesDependencies);
			EnsureSpecialType(SpecialType.System_Object, instance);
			EnsureSpecialType(SpecialType.System_Boolean, instance);
			EnsureSpecialType(SpecialType.System_Int32, instance);
			EnsureSpecialType(SpecialType.System_IDisposable, instance);
			EnsureSpecialMember(SpecialMember.System_IDisposable__Dispose, instance);
			EnsureSpecialType(SpecialType.System_Collections_IEnumerator, instance);
			EnsureSpecialPropertyGetter(SpecialMember.System_Collections_IEnumerator__Current, instance);
			EnsureSpecialMember(SpecialMember.System_Collections_IEnumerator__MoveNext, instance);
			EnsureSpecialMember(SpecialMember.System_Collections_IEnumerator__Reset, instance);
			EnsureSpecialType(SpecialType.System_Collections_Generic_IEnumerator_T, instance);
			EnsureSpecialPropertyGetter(SpecialMember.System_Collections_Generic_IEnumerator_T__Current, instance);
			if (_isEnumerable)
			{
				EnsureSpecialType(SpecialType.System_Collections_IEnumerable, instance);
				EnsureSpecialMember(SpecialMember.System_Collections_IEnumerable__GetEnumerator, instance);
				EnsureSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T, instance);
				EnsureSpecialMember(SpecialMember.System_Collections_Generic_IEnumerable_T__GetEnumerator, instance);
			}
			bool num = instance.HasAnyErrors();
			if (num)
			{
				Diagnostics.AddRange(instance);
			}
			else
			{
				Diagnostics.AddDependencies(instance);
			}
			instance.Free();
			return num;
		}

		protected override void GenerateControlFields()
		{
			StateField = F.StateMachineField(F.SpecialType(SpecialType.System_Int32), Method, GeneratedNames.MakeStateMachineStateFieldName(), Accessibility.Public);
			_currentField = F.StateMachineField(_elementType, Method, GeneratedNames.MakeIteratorCurrentFieldName(), Accessibility.Public);
			_initialThreadIdField = (_isEnumerable ? F.StateMachineField(F.SpecialType(SpecialType.System_Int32), Method, GeneratedNames.MakeIteratorInitialThreadIdName(), Accessibility.Public) : null);
		}

		protected override void GenerateMethodImplementations()
		{
			BoundExpression right = null;
			SynthesizedMethod disposeMethod = OpenMethodImplementation(SpecialMember.System_IDisposable__Dispose, "Dispose", Accessibility.Private, hasMethodBodyDependency: true);
			SynthesizedMethod synthesizedMethod = OpenMoveNextMethodImplementation(SpecialMember.System_Collections_IEnumerator__MoveNext, Accessibility.Private);
			GenerateMoveNextAndDispose(synthesizedMethod, disposeMethod);
			F.CurrentMethod = synthesizedMethod;
			if (_isEnumerable)
			{
				SynthesizedMethod method = OpenMethodImplementation(F.SpecialType(SpecialType.System_Collections_Generic_IEnumerable_T).Construct(_elementType), SpecialMember.System_Collections_Generic_IEnumerable_T__GetEnumerator, "GetEnumerator", Accessibility.Private);
				ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
				LocalSymbol localSymbol = F.SynthesizedLocal(StateMachineType);
				MethodSymbol methodSymbol = null;
				PropertySymbol propertySymbol = F.WellKnownMember<PropertySymbol>(WellKnownMember.System_Environment__CurrentManagedThreadId, isOptional: true);
				if ((object)propertySymbol != null)
				{
					methodSymbol = propertySymbol.GetMethod;
				}
				right = (((object)methodSymbol == null) ? F.Property(F.Property(WellKnownMember.System_Threading_Thread__CurrentThread), WellKnownMember.System_Threading_Thread__ManagedThreadId) : F.Call(null, methodSymbol));
				GeneratedLabelSymbol generatedLabelSymbol = F.GenerateLabel("thisInitialized");
				instance.Add(F.If(F.LogicalAndAlso(F.IntEqual(F.Field(F.Me(), StateField, isLValue: false), F.Literal(StateMachineStates.FinishedStateMachine)), F.IntEqual(F.Field(F.Me(), _initialThreadIdField, isLValue: false), right)), F.Block(F.Assignment(F.Field(F.Me(), StateField, isLValue: true), F.Literal(StateMachineStates.FirstUnusedState)), F.Assignment(F.Local(localSymbol, isLValue: true), F.Me()), (Method.IsShared || Method.MeParameter.Type.IsReferenceType) ? ((BoundStatement)F.Goto(generatedLabelSymbol)) : ((BoundStatement)F.StatementList())), F.Assignment(F.Local(localSymbol, isLValue: true), F.New(StateMachineType.Constructor, F.Literal(0)))));
				Dictionary<Symbol, FieldSymbol> initialParameters = InitialParameters;
				Dictionary<Symbol, FieldSymbol> dictionary = nonReusableLocalProxies;
				if (!Method.IsShared)
				{
					FieldSymbol value = null;
					if (dictionary.TryGetValue(Method.MeParameter, out value))
					{
						instance.Add(F.Assignment(F.Field(F.Local(localSymbol, isLValue: true), value.AsMember(StateMachineType), isLValue: true), F.Field(F.Me(), initialParameters[Method.MeParameter].AsMember(F.CurrentType), isLValue: false)));
					}
				}
				instance.Add(F.Label(generatedLabelSymbol));
				ImmutableArray<ParameterSymbol>.Enumerator enumerator = Method.Parameters.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ParameterSymbol current = enumerator.Current;
					FieldSymbol value2 = null;
					if (dictionary.TryGetValue(current, out value2))
					{
						instance.Add(F.Assignment(F.Field(F.Local(localSymbol, isLValue: true), value2.AsMember(StateMachineType), isLValue: true), F.Field(F.Me(), initialParameters[current].AsMember(F.CurrentType), isLValue: false)));
					}
				}
				instance.Add(F.Return(F.Local(localSymbol, isLValue: false)));
				F.CloseMethod(F.Block(ImmutableArray.Create(localSymbol), instance.ToImmutableAndFree()));
				OpenMethodImplementation(SpecialMember.System_Collections_IEnumerable__GetEnumerator, "IEnumerable.GetEnumerator", Accessibility.Private);
				F.CloseMethod(F.Return(F.Call(F.Me(), method)));
			}
			OpenPropertyImplementation(F.SpecialType(SpecialType.System_Collections_Generic_IEnumerator_T).Construct(_elementType), SpecialMember.System_Collections_Generic_IEnumerator_T__Current, "Current", Accessibility.Private);
			F.CloseMethod(F.Return(F.Field(F.Me(), _currentField, isLValue: false)));
			OpenMethodImplementation(SpecialMember.System_Collections_IEnumerator__Reset, "Reset", Accessibility.Private);
			F.CloseMethod(F.Throw(F.New(F.WellKnownType(WellKnownType.System_NotSupportedException))));
			OpenPropertyImplementation(SpecialMember.System_Collections_IEnumerator__Current, "IEnumerator.Current", Accessibility.Private);
			F.CloseMethod(F.Return(F.Field(F.Me(), _currentField, isLValue: false)));
			F.CurrentMethod = StateMachineType.Constructor;
			ArrayBuilder<BoundStatement> instance2 = ArrayBuilder<BoundStatement>.GetInstance();
			instance2.Add(F.BaseInitialization());
			instance2.Add(F.Assignment(F.Field(F.Me(), StateField, isLValue: true), F.Parameter(F.CurrentMethod.Parameters[0]).MakeRValue()));
			if (_isEnumerable)
			{
				instance2.Add(F.Assignment(F.Field(F.Me(), _initialThreadIdField, isLValue: true), right));
			}
			instance2.Add(F.Return());
			F.CloseMethod(F.Block(instance2.ToImmutableAndFree()));
			instance2 = null;
		}

		protected override BoundStatement GenerateStateMachineCreation(LocalSymbol stateMachineVariable, NamedTypeSymbol frameType)
		{
			return F.Return(F.Local(stateMachineVariable, isLValue: false));
		}

		protected override void InitializeStateMachine(ArrayBuilder<BoundStatement> bodyBuilder, NamedTypeSymbol frameType, LocalSymbol stateMachineLocal)
		{
			int value = (_isEnumerable ? StateMachineStates.FinishedStateMachine : StateMachineStates.FirstUnusedState);
			bodyBuilder.Add(F.Assignment(F.Local(stateMachineLocal, isLValue: true), F.New(SymbolExtensions.AsMember(StateMachineType.Constructor, frameType), F.Literal(value))));
		}

		private void GenerateMoveNextAndDispose(SynthesizedMethod moveNextMethod, SynthesizedMethod disposeMethod)
		{
			new IteratorMethodToClassRewriter(Method, F, StateField, _currentField, hoistedVariables, nonReusableLocalProxies, SynthesizedLocalOrdinals, SlotAllocatorOpt, nextFreeHoistedLocalSlot, Diagnostics).GenerateMoveNextAndDispose(Body, moveNextMethod, disposeMethod);
		}

		protected override FieldSymbol CreateByValLocalCapture(FieldSymbol field, LocalSymbol local)
		{
			return field;
		}

		protected override FieldSymbol CreateParameterCapture(FieldSymbol field, ParameterSymbol parameter)
		{
			return field;
		}

		protected override void InitializeParameterWithProxy(ParameterSymbol parameter, FieldSymbol proxy, LocalSymbol stateMachineVariable, ArrayBuilder<BoundExpression> initializers)
		{
			NamedTypeSymbol newOwner = (Method.IsGenericMethod ? StateMachineType.Construct(Method.TypeArguments) : StateMachineType);
			BoundExpression right = (parameter.IsMe ? ((BoundExpression)F.Me()) : ((BoundExpression)F.Parameter(parameter).MakeRValue()));
			initializers.Add(F.AssignmentExpression(F.Field(F.Local(stateMachineVariable, isLValue: true), proxy.AsMember(newOwner), isLValue: true), right));
		}
	}
}
