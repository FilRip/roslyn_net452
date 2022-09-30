using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.Collections;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class StateMachineRewriter<TProxy>
	{
		internal abstract class StateMachineMethodToClassRewriter : MethodToClassRewriter<TProxy>
		{
			protected struct StateInfo
			{
				public readonly int Number;

				public readonly GeneratedLabelSymbol ResumeLabel;

				public StateInfo(int stateNumber, GeneratedLabelSymbol resumeLabel)
				{
					this = default(StateInfo);
					Number = stateNumber;
					ResumeLabel = resumeLabel;
				}
			}

			protected internal readonly SyntheticBoundNodeFactory F;

			protected int NextState;

			protected readonly FieldSymbol StateField;

			protected readonly LocalSymbol CachedState;

			protected Dictionary<LabelSymbol, List<int>> Dispatches;

			protected readonly Dictionary<int, int> FinalizerStateMap;

			private bool _hasFinalizerState;

			private int _currentFinalizerState;

			private readonly IReadOnlySet<Symbol> _hoistedVariables;

			private readonly SynthesizedLocalOrdinalsDispenser _synthesizedLocalOrdinals;

			private readonly int _nextFreeHoistedLocalSlot;

			protected abstract string ResumeLabelName { get; }

			protected override TypeSubstitution TypeMap => F.CurrentType.TypeSubstitution;

			protected override MethodSymbol CurrentMethod => F.CurrentMethod;

			protected override MethodSymbol TopLevelMethod => F.TopLevelMethod;

			public StateMachineMethodToClassRewriter(SyntheticBoundNodeFactory F, FieldSymbol stateField, IReadOnlySet<Symbol> hoistedVariables, Dictionary<Symbol, TProxy> initialProxies, SynthesizedLocalOrdinalsDispenser synthesizedLocalOrdinals, VariableSlotAllocator slotAllocatorOpt, int nextFreeHoistedLocalSlot, BindingDiagnosticBag diagnostics)
				: base(slotAllocatorOpt, F.CompilationState, diagnostics, preserveOriginalLocals: false)
			{
				NextState = 0;
				Dispatches = new Dictionary<LabelSymbol, List<int>>();
				FinalizerStateMap = new Dictionary<int, int>();
				_hasFinalizerState = true;
				_currentFinalizerState = -1;
				_hoistedVariables = null;
				this.F = F;
				StateField = stateField;
				CachedState = F.SynthesizedLocal(F.SpecialType(SpecialType.System_Int32), SynthesizedLocalKind.StateMachineCachedState, F.Syntax);
				_hoistedVariables = hoistedVariables;
				_synthesizedLocalOrdinals = synthesizedLocalOrdinals;
				_nextFreeHoistedLocalSlot = nextFreeHoistedLocalSlot;
				foreach (KeyValuePair<Symbol, TProxy> initialProxy in initialProxies)
				{
					Proxies.Add(initialProxy.Key, initialProxy.Value);
				}
			}

			protected abstract BoundStatement GenerateReturn(bool finished);

			internal override BoundExpression FramePointer(SyntaxNode syntax, NamedTypeSymbol frameClass)
			{
				SyntaxNode syntax2 = F.Syntax;
				F.Syntax = syntax;
				BoundMeReference result = F.Me();
				F.Syntax = syntax2;
				return result;
			}

			protected StateInfo AddState()
			{
				int nextState = NextState;
				NextState++;
				if (Dispatches == null)
				{
					Dispatches = new Dictionary<LabelSymbol, List<int>>();
				}
				if (!_hasFinalizerState)
				{
					_currentFinalizerState = NextState;
					NextState++;
					_hasFinalizerState = true;
				}
				GeneratedLabelSymbol generatedLabelSymbol = F.GenerateLabel(ResumeLabelName);
				Dispatches.Add(generatedLabelSymbol, new List<int> { nextState });
				FinalizerStateMap.Add(nextState, _currentFinalizerState);
				return new StateInfo(nextState, generatedLabelSymbol);
			}

			protected BoundStatement Dispatch()
			{
				return F.Select(F.Local(CachedState, isLValue: false), from kv in Dispatches
					orderby kv.Value[0]
					select F.SwitchSection(kv.Value, F.Goto(kv.Key)));
			}

			public override BoundNode Visit(BoundNode node)
			{
				if (node == null)
				{
					return node;
				}
				SyntaxNode syntax = F.Syntax;
				F.Syntax = node.Syntax;
				BoundNode result = base.Visit(node);
				F.Syntax = syntax;
				return result;
			}

			public override BoundNode VisitBlock(BoundBlock node)
			{
				return PossibleStateMachineScope(node.Locals, base.VisitBlock(node));
			}

			private BoundNode PossibleStateMachineScope(ImmutableArray<LocalSymbol> locals, BoundNode wrapped)
			{
				if (locals.IsEmpty)
				{
					return wrapped;
				}
				ArrayBuilder<FieldSymbol> instance = ArrayBuilder<FieldSymbol>.GetInstance();
				ImmutableArray<LocalSymbol>.Enumerator enumerator = locals.GetEnumerator();
				while (enumerator.MoveNext())
				{
					LocalSymbol current = enumerator.Current;
					if (NeedsProxy(current) && !current.IsByRef && (current.SynthesizedKind == SynthesizedLocalKind.UserDefined || current.SynthesizedKind == SynthesizedLocalKind.LambdaDisplayClass))
					{
						TProxy value = default(TProxy);
						if (Proxies.TryGetValue(current, out value))
						{
							AddProxyFieldsForStateMachineScope(value, instance);
						}
					}
				}
				BoundNode boundNode = wrapped;
				if (instance.Count > 0)
				{
					boundNode = MakeStateMachineScope(instance.ToImmutable(), (BoundStatement)boundNode);
				}
				instance.Free();
				return boundNode;
			}

			internal BoundBlock MakeStateMachineScope(ImmutableArray<FieldSymbol> hoistedLocals, BoundStatement statement)
			{
				return F.Block(BoundNodeExtensions.MakeCompilerGenerated(new BoundStateMachineScope(F.Syntax, hoistedLocals, statement)));
			}

			internal bool TryUnwrapBoundStateMachineScope(ref BoundStatement statement, out ImmutableArray<FieldSymbol> hoistedLocals)
			{
				if (statement.Kind == BoundKind.Block)
				{
					ImmutableArray<BoundStatement> statements = ((BoundBlock)statement).Statements;
					if (statements.Length == 1 && statements[0].Kind == BoundKind.StateMachineScope)
					{
						BoundStateMachineScope boundStateMachineScope = (BoundStateMachineScope)statements[0];
						statement = boundStateMachineScope.Statement;
						hoistedLocals = boundStateMachineScope.Fields;
						return true;
					}
				}
				hoistedLocals = ImmutableArray<FieldSymbol>.Empty;
				return false;
			}

			private bool NeedsProxy(Symbol localOrParameter)
			{
				return _hoistedVariables.Contains(localOrParameter);
			}

			internal abstract void AddProxyFieldsForStateMachineScope(TProxy proxy, ArrayBuilder<FieldSymbol> proxyFields);

			public override BoundNode VisitTryStatement(BoundTryStatement node)
			{
				Dictionary<LabelSymbol, List<int>> dictionary = Dispatches;
				int currentFinalizerState = _currentFinalizerState;
				bool hasFinalizerState = _hasFinalizerState;
				Dispatches = null;
				_currentFinalizerState = -1;
				_hasFinalizerState = false;
				BoundBlock boundBlock = F.Block((BoundStatement)Visit(node.TryBlock));
				GeneratedLabelSymbol generatedLabelSymbol = null;
				if (Dispatches != null)
				{
					generatedLabelSymbol = F.GenerateLabel("tryDispatch");
					if (_hasFinalizerState)
					{
						GeneratedLabelSymbol generatedLabelSymbol2 = F.GenerateLabel("finalizer");
						Dispatches.Add(generatedLabelSymbol2, new List<int> { _currentFinalizerState });
						GeneratedLabelSymbol generatedLabelSymbol3 = F.GenerateLabel("skipFinalizer");
						boundBlock = F.Block(SyntheticBoundNodeFactory.HiddenSequencePoint(), Dispatch(), F.Goto(generatedLabelSymbol3), F.Label(generatedLabelSymbol2), F.Assignment(F.Field(F.Me(), StateField, isLValue: true), F.AssignmentExpression(F.Local(CachedState, isLValue: true), F.Literal(StateMachineStates.NotStartedStateMachine))), GenerateReturn(finished: false), F.Label(generatedLabelSymbol3), boundBlock);
					}
					else
					{
						boundBlock = F.Block(SyntheticBoundNodeFactory.HiddenSequencePoint(), Dispatch(), boundBlock);
					}
					if (dictionary == null)
					{
						dictionary = new Dictionary<LabelSymbol, List<int>>();
					}
					dictionary.Add(generatedLabelSymbol, new List<int>(from kv in Dispatches.Values
						from n in kv
						select new VB_0024AnonymousType_1<List<int>, int>(kv, n) into _0024VB_0024It
						orderby _0024VB_0024It.n
						select _0024VB_0024It.n));
				}
				_hasFinalizerState = hasFinalizerState;
				_currentFinalizerState = currentFinalizerState;
				Dispatches = dictionary;
				ImmutableArray<BoundCatchBlock> catchBlocks = VisitList(node.CatchBlocks);
				BoundBlock finallyBlockOpt = ((node.FinallyBlockOpt == null) ? null : F.Block(SyntheticBoundNodeFactory.HiddenSequencePoint(), F.If(F.IntLessThan(F.Local(CachedState, isLValue: false), F.Literal(StateMachineStates.FirstUnusedState)), (BoundBlock)Visit(node.FinallyBlockOpt)), SyntheticBoundNodeFactory.HiddenSequencePoint()));
				BoundStatement boundStatement = node.Update(boundBlock, catchBlocks, finallyBlockOpt, node.ExitLabelOpt);
				if ((object)generatedLabelSymbol != null)
				{
					boundStatement = F.Block(SyntheticBoundNodeFactory.HiddenSequencePoint(), F.Label(generatedLabelSymbol), boundStatement);
				}
				return boundStatement;
			}

			public override BoundNode VisitMeReference(BoundMeReference node)
			{
				return MaterializeProxy(node, Proxies[TopLevelMethod.MeParameter]);
			}

			public override BoundNode VisitMyClassReference(BoundMyClassReference node)
			{
				return MaterializeProxy(node, Proxies[TopLevelMethod.MeParameter]);
			}

			public override BoundNode VisitMyBaseReference(BoundMyBaseReference node)
			{
				return MaterializeProxy(node, Proxies[TopLevelMethod.MeParameter]);
			}

			private LocalSymbol TryRewriteLocal(LocalSymbol local)
			{
				if (NeedsProxy(local))
				{
					return null;
				}
				LocalSymbol value = null;
				if (!LocalMap.TryGetValue(local, out value))
				{
					TypeSymbol typeSymbol = VisitType(local.Type);
					if (TypeSymbol.Equals(typeSymbol, local.Type, TypeCompareKind.ConsiderEverything))
					{
						value = local;
					}
					else
					{
						value = LocalSymbol.Create(local, typeSymbol);
						LocalMap.Add(local, value);
					}
				}
				return value;
			}

			public override BoundNode VisitCatchBlock(BoundCatchBlock node)
			{
				LocalSymbol localOpt = null;
				LocalSymbol localOpt2 = node.LocalOpt;
				if ((object)localOpt2 != null)
				{
					localOpt = TryRewriteLocal(localOpt2);
				}
				BoundExpression exceptionSourceOpt = (BoundExpression)Visit(node.ExceptionSourceOpt);
				BoundExpression errorLineNumberOpt = (BoundExpression)Visit(node.ErrorLineNumberOpt);
				BoundExpression exceptionFilterOpt = (BoundExpression)Visit(node.ExceptionFilterOpt);
				BoundBlock body = (BoundBlock)Visit(node.Body);
				return node.Update(localOpt, exceptionSourceOpt, errorLineNumberOpt, exceptionFilterOpt, body, node.IsSynthesizedAsyncCatchAll);
			}

			public sealed override BoundNode VisitUserDefinedConversion(BoundUserDefinedConversion node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitUserDefinedShortCircuitingOperator(BoundUserDefinedShortCircuitingOperator node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitUserDefinedBinaryOperator(BoundUserDefinedBinaryOperator node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitUserDefinedUnaryOperator(BoundUserDefinedUnaryOperator node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitBadExpression(BoundBadExpression node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitBadVariable(BoundBadVariable node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitParenthesized(BoundParenthesized node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitUnboundLambda(UnboundLambda node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitAttribute(BoundAttribute node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitAnonymousTypeCreationExpression(BoundAnonymousTypeCreationExpression node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitAnonymousTypePropertyAccess(BoundAnonymousTypePropertyAccess node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitAnonymousTypeFieldInitializer(BoundAnonymousTypeFieldInitializer node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitLateMemberAccess(BoundLateMemberAccess node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitLateInvocation(BoundLateInvocation node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitLateAddressOfOperator(BoundLateAddressOfOperator node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitLateBoundArgumentSupportingAssignmentWithCapture(BoundLateBoundArgumentSupportingAssignmentWithCapture node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitRangeVariable(BoundRangeVariable node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitGroupTypeInferenceLambda(GroupTypeInferenceLambda node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitLambda(BoundLambda node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitQueryLambda(BoundQueryLambda node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitXmlNamespace(BoundXmlNamespace node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitXmlName(BoundXmlName node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitXmlDocument(BoundXmlDocument node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitXmlDeclaration(BoundXmlDeclaration node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitXmlProcessingInstruction(BoundXmlProcessingInstruction node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitXmlComment(BoundXmlComment node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitXmlAttribute(BoundXmlAttribute node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitXmlElement(BoundXmlElement node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitXmlMemberAccess(BoundXmlMemberAccess node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitXmlCData(BoundXmlCData node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitXmlEmbeddedExpression(BoundXmlEmbeddedExpression node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitByRefArgumentPlaceholder(BoundByRefArgumentPlaceholder node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitByRefArgumentWithCopyBack(BoundByRefArgumentWithCopyBack node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitCompoundAssignmentTargetPlaceholder(BoundCompoundAssignmentTargetPlaceholder node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitWithLValueExpressionPlaceholder(BoundWithLValueExpressionPlaceholder node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitWithRValueExpressionPlaceholder(BoundWithRValueExpressionPlaceholder node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitMethodGroup(BoundMethodGroup node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitPropertyGroup(BoundPropertyGroup node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitValueTypeMeReference(BoundValueTypeMeReference node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitObjectInitializerExpression(BoundObjectInitializerExpression node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitCollectionInitializerExpression(BoundCollectionInitializerExpression node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitPropertyAccess(BoundPropertyAccess node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitArrayLiteral(BoundArrayLiteral node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitNullableIsTrueOperator(BoundNullableIsTrueOperator node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitNewT(BoundNewT node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitDup(BoundDup node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitOmittedArgument(BoundOmittedArgument node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitTypeArguments(BoundTypeArguments node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitTypeOrValueExpression(BoundTypeOrValueExpression node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public sealed override BoundNode VisitAddressOfOperator(BoundAddressOfOperator node)
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		protected readonly BoundStatement Body;

		protected readonly MethodSymbol Method;

		protected readonly BindingDiagnosticBag Diagnostics;

		protected readonly SyntheticBoundNodeFactory F;

		protected readonly SynthesizedContainer StateMachineType;

		protected readonly VariableSlotAllocator SlotAllocatorOpt;

		protected readonly SynthesizedLocalOrdinalsDispenser SynthesizedLocalOrdinals;

		protected FieldSymbol StateField;

		protected Dictionary<Symbol, TProxy> nonReusableLocalProxies;

		protected int nextFreeHoistedLocalSlot;

		protected IReadOnlySet<Symbol> hoistedVariables;

		protected Dictionary<Symbol, TProxy> InitialParameters;

		protected abstract bool PreserveInitialParameterValues { get; }

		internal abstract TypeSubstitution TypeMap { get; }

		protected StateMachineRewriter(BoundStatement body, MethodSymbol method, StateMachineTypeSymbol stateMachineType, VariableSlotAllocator slotAllocatorOpt, TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
		{
			Body = body;
			Method = method;
			StateMachineType = stateMachineType;
			SlotAllocatorOpt = slotAllocatorOpt;
			Diagnostics = diagnostics;
			SynthesizedLocalOrdinals = new SynthesizedLocalOrdinalsDispenser();
			nonReusableLocalProxies = new Dictionary<Symbol, TProxy>();
			F = new SyntheticBoundNodeFactory(method, method, method.ContainingType, body.Syntax, compilationState, diagnostics);
		}

		protected abstract void GenerateControlFields();

		protected abstract void InitializeStateMachine(ArrayBuilder<BoundStatement> bodyBuilder, NamedTypeSymbol frameType, LocalSymbol stateMachineLocal);

		protected abstract BoundStatement GenerateStateMachineCreation(LocalSymbol stateMachineVariable, NamedTypeSymbol frameType);

		protected abstract void GenerateMethodImplementations();

		protected BoundBlock Rewrite()
		{
			F.OpenNestedType(StateMachineType);
			F.CompilationState.StateMachineImplementationClass[Method] = StateMachineType;
			GenerateControlFields();
			if (PreserveInitialParameterValues)
			{
				InitialParameters = new Dictionary<Symbol, TProxy>();
			}
			IteratorAndAsyncCaptureWalker.Result captured = IteratorAndAsyncCaptureWalker.Analyze(new FlowAnalysisInfo(F.CompilationState.Compilation, Method, Body), Diagnostics.DiagnosticBag);
			CreateNonReusableLocalProxies(captured, ref nextFreeHoistedLocalSlot);
			hoistedVariables = new OrderedSet<Symbol>(captured.CapturedLocals);
			GenerateMethodImplementations();
			return GenerateKickoffMethodBody();
		}

		private BoundBlock GenerateKickoffMethodBody()
		{
			F.CurrentMethod = Method;
			ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
			instance.Add(SyntheticBoundNodeFactory.HiddenSequencePoint());
			NamedTypeSymbol namedTypeSymbol = (Method.IsGenericMethod ? StateMachineType.Construct(Method.TypeArguments) : StateMachineType);
			LocalSymbol localSymbol = F.SynthesizedLocal(namedTypeSymbol);
			InitializeStateMachine(instance, namedTypeSymbol, localSymbol);
			Dictionary<Symbol, TProxy> dictionary = (PreserveInitialParameterValues ? InitialParameters : nonReusableLocalProxies);
			ArrayBuilder<BoundExpression> instance2 = ArrayBuilder<BoundExpression>.GetInstance();
			if (!Method.IsShared && (object)Method.MeParameter != null)
			{
				TProxy value = default(TProxy);
				if (dictionary.TryGetValue(Method.MeParameter, out value))
				{
					InitializeParameterWithProxy(Method.MeParameter, value, localSymbol, instance2);
				}
			}
			ImmutableArray<ParameterSymbol>.Enumerator enumerator = Method.Parameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ParameterSymbol current = enumerator.Current;
				TProxy value2 = default(TProxy);
				if (dictionary.TryGetValue(current, out value2))
				{
					InitializeParameterWithProxy(current, value2, localSymbol, instance2);
				}
			}
			if (instance2.Count > 0)
			{
				instance.Add(F.ExpressionStatement(F.Sequence(instance2.ToArray())));
			}
			instance2.Free();
			instance.Add(GenerateStateMachineCreation(localSymbol, namedTypeSymbol));
			return F.Block(ImmutableArray.Create(localSymbol), instance.ToImmutableAndFree());
		}

		private void CreateNonReusableLocalProxies(IteratorAndAsyncCaptureWalker.Result captured, ref int nextFreeHoistedLocalSlot)
		{
			TypeSubstitution typeSubstitution = StateMachineType.TypeSubstitution;
			bool flag = F.Compilation.Options.OptimizationLevel == OptimizationLevel.Debug && SlotAllocatorOpt != null;
			this.nextFreeHoistedLocalSlot = (flag ? SlotAllocatorOpt.PreviousHoistedLocalSlotCount : 0);
			ArrayBuilder<Symbol>.Enumerator enumerator = captured.CapturedLocals.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				switch (current.Kind)
				{
				case SymbolKind.Local:
				{
					LocalSymbol localSymbol = (LocalSymbol)current;
					if (!localSymbol.IsConst && localSymbol.SynthesizedKind != SynthesizedLocalKind.ConditionalBranchDiscriminator)
					{
						CaptureLocalSymbol(typeSubstitution, (LocalSymbol)current, captured.ByRefLocalsInitializers);
					}
					break;
				}
				case SymbolKind.Parameter:
					CaptureParameterSymbol(typeSubstitution, (ParameterSymbol)current);
					break;
				}
			}
		}

		protected TProxy CaptureParameterSymbol(TypeSubstitution typeMap, ParameterSymbol parameter)
		{
			TProxy value = default(TProxy);
			if (nonReusableLocalProxies.TryGetValue(parameter, out value))
			{
				return value;
			}
			if (parameter.IsMe)
			{
				string name = parameter.ContainingSymbol.ContainingType.Name;
				bool flag = name.StartsWith("_Closure$__", StringComparison.Ordinal);
				value = CreateParameterCapture(F.StateMachineField(Method.ContainingType, Method, flag ? GeneratedNames.MakeStateMachineCapturedClosureMeName(name) : GeneratedNames.MakeStateMachineCapturedMeName(), Accessibility.Internal), parameter);
				nonReusableLocalProxies.Add(parameter, value);
				if (PreserveInitialParameterValues)
				{
					TProxy value2 = (TypeSymbolExtensions.IsStructureType(Method.ContainingType) ? CreateParameterCapture(F.StateMachineField(Method.ContainingType, Method, GeneratedNames.MakeIteratorParameterProxyName(GeneratedNames.MakeStateMachineCapturedMeName()), Accessibility.Internal), parameter) : nonReusableLocalProxies[parameter]);
					InitialParameters.Add(parameter, value2);
				}
			}
			else
			{
				TypeSymbol type = parameter.Type.InternalSubstituteTypeParameters(typeMap).Type;
				value = CreateParameterCapture(F.StateMachineField(type, Method, GeneratedNames.MakeStateMachineParameterName(parameter.Name), Accessibility.Internal), parameter);
				nonReusableLocalProxies.Add(parameter, value);
				if (PreserveInitialParameterValues)
				{
					InitialParameters.Add(parameter, CreateParameterCapture(F.StateMachineField(type, Method, GeneratedNames.MakeIteratorParameterProxyName(parameter.Name), Accessibility.Internal), parameter));
				}
			}
			return value;
		}

		protected TProxy CaptureLocalSymbol(TypeSubstitution typeMap, LocalSymbol local, Dictionary<LocalSymbol, BoundExpression> initializers)
		{
			TProxy value = default(TProxy);
			if (nonReusableLocalProxies.TryGetValue(local, out value))
			{
				return value;
			}
			if (local.IsByRef)
			{
				value = CreateByRefLocalCapture(typeMap, local, initializers);
				nonReusableLocalProxies.Add(local, value);
				return value;
			}
			TypeSymbol type = local.Type.InternalSubstituteTypeParameters(typeMap).Type;
			LocalDebugId localDebugId = LocalDebugId.None;
			int num = -1;
			if (!local.SynthesizedKind.IsSlotReusable(F.Compilation.Options.OptimizationLevel))
			{
				SyntaxNode declaratorSyntax = local.GetDeclaratorSyntax();
				int syntaxOffset = Method.CalculateLocalSyntaxOffset(declaratorSyntax.SpanStart, declaratorSyntax.SyntaxTree);
				int ordinal = SynthesizedLocalOrdinals.AssignLocalOrdinal(local.SynthesizedKind, syntaxOffset);
				localDebugId = new LocalDebugId(syntaxOffset, ordinal);
				int slotIndex = -1;
				if (SlotAllocatorOpt != null && SlotAllocatorOpt.TryGetPreviousHoistedLocalSlotIndex(declaratorSyntax, F.CompilationState.ModuleBuilderOpt.Translate(type, declaratorSyntax, Diagnostics.DiagnosticBag), local.SynthesizedKind, localDebugId, Diagnostics.DiagnosticBag, out slotIndex))
				{
					num = slotIndex;
				}
			}
			if (num == -1)
			{
				num = nextFreeHoistedLocalSlot;
				nextFreeHoistedLocalSlot++;
			}
			value = CreateByValLocalCapture(MakeHoistedFieldForLocal(local, type, num, localDebugId), local);
			nonReusableLocalProxies.Add(local, value);
			return value;
		}

		protected abstract void InitializeParameterWithProxy(ParameterSymbol parameter, TProxy proxy, LocalSymbol stateMachineVariable, ArrayBuilder<BoundExpression> initializers);

		protected abstract TProxy CreateByValLocalCapture(FieldSymbol field, LocalSymbol local);

		protected abstract TProxy CreateParameterCapture(FieldSymbol field, ParameterSymbol parameter);

		protected virtual TProxy CreateByRefLocalCapture(TypeSubstitution typeMap, LocalSymbol local, Dictionary<LocalSymbol, BoundExpression> initializers)
		{
			throw ExceptionUtilities.Unreachable;
		}

		protected FieldSymbol MakeHoistedFieldForLocal(LocalSymbol local, TypeSymbol localType, int slotIndex, LocalDebugId id)
		{
			string name = local.SynthesizedKind switch
			{
				SynthesizedLocalKind.LambdaDisplayClass => "$VB$ResumableLocal_$VB$Closure_$" + Microsoft.VisualBasic.CompilerServices.Conversions.ToString(slotIndex), 
				SynthesizedLocalKind.UserDefined => "$VB$ResumableLocal_" + local.Name + "$" + Microsoft.VisualBasic.CompilerServices.Conversions.ToString(slotIndex), 
				SynthesizedLocalKind.With => "$W" + Microsoft.VisualBasic.CompilerServices.Conversions.ToString(slotIndex), 
				_ => "$S" + Microsoft.VisualBasic.CompilerServices.Conversions.ToString(slotIndex), 
			};
			return F.StateMachineField(localType, Method, name, new LocalSlotDebugInfo(local.SynthesizedKind, id), slotIndex, Accessibility.Internal);
		}

		internal virtual bool EnsureAllSymbolsAndSignature()
		{
			if (TypeSymbolExtensions.IsErrorType(Method.ReturnType))
			{
				return true;
			}
			ImmutableArray<ParameterSymbol>.Enumerator enumerator = Method.Parameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ParameterSymbol current = enumerator.Current;
				if (current.IsByRef || TypeSymbolExtensions.IsErrorType(current.Type))
				{
					return true;
				}
			}
			return false;
		}

		internal Symbol EnsureSpecialType(SpecialType type, BindingDiagnosticBag bag)
		{
			return Binder.GetSpecialType(F.Compilation, type, Body.Syntax, bag);
		}

		internal Symbol EnsureWellKnownType(WellKnownType type, BindingDiagnosticBag bag)
		{
			return Binder.GetWellKnownType(F.Compilation, type, Body.Syntax, bag);
		}

		internal Symbol EnsureSpecialMember(SpecialMember member, BindingDiagnosticBag bag)
		{
			return Binder.GetSpecialTypeMember(F.Compilation.Assembly, member, Body.Syntax, bag);
		}

		internal Symbol EnsureWellKnownMember(WellKnownMember member, BindingDiagnosticBag bag)
		{
			return Binder.GetWellKnownTypeMember(F.Compilation, member, Body.Syntax, bag);
		}

		internal void EnsureSpecialPropertyGetter(SpecialMember member, BindingDiagnosticBag bag)
		{
			PropertySymbol propertySymbol = (PropertySymbol)EnsureSpecialMember(member, bag);
			if ((object)propertySymbol != null)
			{
				MethodSymbol getMethod = propertySymbol.GetMethod;
				if ((object)getMethod == null)
				{
					Binder.ReportDiagnostic(bag, Body.Syntax, ERRID.ERR_NoGetProperty1, CustomSymbolDisplayFormatter.QualifiedName(propertySymbol));
				}
				else
				{
					UseSiteInfo<AssemblySymbol> useSiteInfo = getMethod.GetUseSiteInfo();
					Binder.ReportUseSite(bag, Body.Syntax, useSiteInfo);
				}
			}
		}

		internal SynthesizedMethod OpenMethodImplementation(WellKnownMember interfaceMethod, string name, Accessibility accessibility, bool hasMethodBodyDependency = false, PropertySymbol associatedProperty = null)
		{
			MethodSymbol methodToImplement = F.WellKnownMember<MethodSymbol>(interfaceMethod);
			return OpenMethodImplementation(methodToImplement, name, accessibility, hasMethodBodyDependency, associatedProperty);
		}

		internal SynthesizedMethod OpenMethodImplementation(SpecialMember interfaceMethod, string name, Accessibility accessibility, bool hasMethodBodyDependency = false, PropertySymbol associatedProperty = null)
		{
			MethodSymbol methodToImplement = (MethodSymbol)F.SpecialMember(interfaceMethod);
			return OpenMethodImplementation(methodToImplement, name, accessibility, hasMethodBodyDependency, associatedProperty);
		}

		internal SynthesizedMethod OpenMethodImplementation(NamedTypeSymbol interfaceType, SpecialMember interfaceMethod, string name, Accessibility accessibility, bool hasMethodBodyDependency = false, PropertySymbol associatedProperty = null)
		{
			MethodSymbol methodToImplement = SymbolExtensions.AsMember((MethodSymbol)F.SpecialMember(interfaceMethod), interfaceType);
			return OpenMethodImplementation(methodToImplement, name, accessibility, hasMethodBodyDependency, associatedProperty);
		}

		private SynthesizedMethod OpenMethodImplementation(MethodSymbol methodToImplement, string methodName, Accessibility accessibility, bool hasMethodBodyDependency = false, PropertySymbol associatedProperty = null)
		{
			SynthesizedStateMachineDebuggerNonUserCodeMethod synthesizedStateMachineDebuggerNonUserCodeMethod = new SynthesizedStateMachineDebuggerNonUserCodeMethod((StateMachineTypeSymbol)F.CurrentType, methodName, methodToImplement, F.Syntax, accessibility, hasMethodBodyDependency, associatedProperty);
			F.AddMethod(F.CurrentType, synthesizedStateMachineDebuggerNonUserCodeMethod);
			F.CurrentMethod = synthesizedStateMachineDebuggerNonUserCodeMethod;
			return synthesizedStateMachineDebuggerNonUserCodeMethod;
		}

		internal MethodSymbol OpenPropertyImplementation(SpecialMember interfaceProperty, string name, Accessibility accessibility)
		{
			MethodSymbol getMethod = ((PropertySymbol)F.SpecialMember(interfaceProperty)).GetMethod;
			return OpenPropertyImplementation(getMethod, name, accessibility);
		}

		internal MethodSymbol OpenPropertyImplementation(NamedTypeSymbol interfaceType, SpecialMember interfaceMethod, string name, Accessibility accessibility)
		{
			MethodSymbol getterToImplement = SymbolExtensions.AsMember(((PropertySymbol)F.SpecialMember(interfaceMethod)).GetMethod, interfaceType);
			return OpenPropertyImplementation(getterToImplement, name, accessibility);
		}

		private MethodSymbol OpenPropertyImplementation(MethodSymbol getterToImplement, string name, Accessibility accessibility)
		{
			SynthesizedStateMachineProperty synthesizedStateMachineProperty = new SynthesizedStateMachineProperty((StateMachineTypeSymbol)F.CurrentType, name, getterToImplement, F.Syntax, accessibility);
			F.AddProperty(F.CurrentType, synthesizedStateMachineProperty);
			MethodSymbol getMethod = synthesizedStateMachineProperty.GetMethod;
			F.AddMethod(F.CurrentType, getMethod);
			F.CurrentMethod = getMethod;
			return getMethod;
		}

		internal void CloseMethod(BoundStatement body)
		{
			F.CloseMethod(RewriteBodyIfNeeded(body, F.TopLevelMethod, F.CurrentMethod));
		}

		internal virtual BoundStatement RewriteBodyIfNeeded(BoundStatement body, MethodSymbol topMethod, MethodSymbol currentMethod)
		{
			return body;
		}

		internal SynthesizedMethod OpenMoveNextMethodImplementation(WellKnownMember interfaceMethod, Accessibility accessibility)
		{
			MethodSymbol methodToImplement = F.WellKnownMember<MethodSymbol>(interfaceMethod);
			return OpenMoveNextMethodImplementation(methodToImplement, accessibility);
		}

		internal SynthesizedMethod OpenMoveNextMethodImplementation(SpecialMember interfaceMethod, Accessibility accessibility)
		{
			MethodSymbol methodToImplement = (MethodSymbol)F.SpecialMember(interfaceMethod);
			return OpenMoveNextMethodImplementation(methodToImplement, accessibility);
		}

		private SynthesizedMethod OpenMoveNextMethodImplementation(MethodSymbol methodToImplement, Accessibility accessibility)
		{
			SynthesizedStateMachineMoveNextMethod synthesizedStateMachineMoveNextMethod = new SynthesizedStateMachineMoveNextMethod((StateMachineTypeSymbol)F.CurrentType, methodToImplement, F.Syntax, accessibility);
			F.AddMethod(F.CurrentType, synthesizedStateMachineMoveNextMethod);
			F.CurrentMethod = synthesizedStateMachineMoveNextMethod;
			return synthesizedStateMachineMoveNextMethod;
		}
	}
}
