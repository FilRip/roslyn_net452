using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class LambdaRewriter : MethodToClassRewriter<FieldSymbol>
	{
		internal sealed class Analysis : BoundTreeWalkerWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator
		{
			private readonly BindingDiagnosticBag _diagnostics;

			private readonly MethodSymbol _method;

			private MethodSymbol _currentParent;

			private BoundNode _currentBlock;

			internal bool seenLambda;

			internal bool seenBackBranches;

			internal Dictionary<BoundNode, BoundNode> blockParent;

			internal Dictionary<LambdaSymbol, MethodSymbol> lambdaParent;

			internal Dictionary<Symbol, BoundNode> variableScope;

			internal Dictionary<LabelSymbol, BoundNode> labelBlock;

			internal Dictionary<BoundGotoStatement, BoundNode> gotoBlock;

			internal HashSet<BoundNode> containsLiftingLambda;

			internal HashSet<BoundNode> needsParentFrame;

			internal Dictionary<LambdaSymbol, BoundNode> lambdaScopes;

			internal HashSet<Symbol> capturedVariables;

			internal MultiDictionary<LambdaSymbol, Symbol> capturedVariablesByLambda;

			internal readonly HashSet<Symbol> declaredInsideExpressionLambda;

			private bool _inExpressionLambda;

			internal readonly ISet<Symbol> symbolsCapturedWithoutCopyCtor;

			private Analysis(MethodSymbol method, ISet<Symbol> symbolsCapturedWithoutCopyCtor, BindingDiagnosticBag diagnostics)
			{
				seenLambda = false;
				seenBackBranches = false;
				blockParent = new Dictionary<BoundNode, BoundNode>();
				lambdaParent = new Dictionary<LambdaSymbol, MethodSymbol>(ReferenceEqualityComparer.Instance);
				variableScope = new Dictionary<Symbol, BoundNode>(ReferenceEqualityComparer.Instance);
				labelBlock = new Dictionary<LabelSymbol, BoundNode>(ReferenceEqualityComparer.Instance);
				gotoBlock = new Dictionary<BoundGotoStatement, BoundNode>();
				containsLiftingLambda = new HashSet<BoundNode>();
				capturedVariables = new HashSet<Symbol>(ReferenceEqualityComparer.Instance);
				capturedVariablesByLambda = new MultiDictionary<LambdaSymbol, Symbol>(ReferenceEqualityComparer.Instance);
				declaredInsideExpressionLambda = new HashSet<Symbol>(ReferenceEqualityComparer.Instance);
				_currentParent = method;
				_method = method;
				this.symbolsCapturedWithoutCopyCtor = symbolsCapturedWithoutCopyCtor;
				_diagnostics = diagnostics;
				_inExpressionLambda = false;
			}

			public static Analysis AnalyzeMethodBody(BoundBlock node, MethodSymbol method, ISet<Symbol> symbolsCapturedWithoutCtor, BindingDiagnosticBag diagnostics)
			{
				Analysis analysis = new Analysis(method, symbolsCapturedWithoutCtor, diagnostics);
				analysis.Analyze(node);
				return analysis;
			}

			private void Analyze(BoundNode node)
			{
				if (node == null)
				{
					return;
				}
				_currentBlock = node;
				if ((object)_method != null)
				{
					ImmutableArray<ParameterSymbol>.Enumerator enumerator = _method.Parameters.GetEnumerator();
					while (enumerator.MoveNext())
					{
						ParameterSymbol current = enumerator.Current;
						variableScope.Add(current, _currentBlock);
						if (_inExpressionLambda)
						{
							declaredInsideExpressionLambda.Add(current);
						}
					}
				}
				Visit(node);
			}

			internal void ComputeLambdaScopesAndFrameCaptures()
			{
				lambdaScopes = new Dictionary<LambdaSymbol, BoundNode>(ReferenceEqualityComparer.Instance);
				needsParentFrame = new HashSet<BoundNode>();
				foreach (KeyValuePair<LambdaSymbol, MultiDictionary<LambdaSymbol, Symbol>.ValueSet> capturedVariablesByLambdum in capturedVariablesByLambda)
				{
					int num = -1;
					BoundNode value = null;
					int num2 = int.MaxValue;
					BoundNode boundNode = null;
					foreach (Symbol item in capturedVariablesByLambdum.Value)
					{
						BoundNode value2 = null;
						int num3 = (variableScope.TryGetValue(item, out value2) ? BlockDepth(value2) : (-1));
						if (num3 > num)
						{
							num = num3;
							value = value2;
						}
						if (num3 < num2)
						{
							num2 = num3;
							boundNode = value2;
						}
					}
					if (value != null)
					{
						lambdaScopes.Add(capturedVariablesByLambdum.Key, value);
						while (value != boundNode)
						{
							needsParentFrame.Add(value);
							blockParent.TryGetValue(value, out value);
						}
					}
				}
			}

			private int BlockDepth(BoundNode node)
			{
				int num = -1;
				while (node != null)
				{
					num++;
					if (!blockParent.TryGetValue(node, out node))
					{
						break;
					}
				}
				return num;
			}

			public BoundNode PushBlock(BoundNode node, ImmutableArray<LocalSymbol> locals)
			{
				if (locals.IsEmpty)
				{
					return _currentBlock;
				}
				BoundNode currentBlock = _currentBlock;
				_currentBlock = node;
				if (_currentBlock != currentBlock)
				{
					blockParent.Add(_currentBlock, currentBlock);
				}
				ImmutableArray<LocalSymbol>.Enumerator enumerator = locals.GetEnumerator();
				while (enumerator.MoveNext())
				{
					LocalSymbol current = enumerator.Current;
					variableScope.Add(current, _currentBlock);
					if (_inExpressionLambda)
					{
						declaredInsideExpressionLambda.Add(current);
					}
				}
				return currentBlock;
			}

			public void PopBlock(BoundNode previousBlock)
			{
				_currentBlock = previousBlock;
			}

			public override BoundNode VisitCatchBlock(BoundCatchBlock node)
			{
				if ((object)node.LocalOpt == null)
				{
					return base.VisitCatchBlock(node);
				}
				BoundNode previousBlock = PushBlock(node, ImmutableArray.Create(node.LocalOpt));
				BoundNode result = base.VisitCatchBlock(node);
				PopBlock(previousBlock);
				return result;
			}

			public override BoundNode VisitBlock(BoundBlock node)
			{
				BoundNode previousBlock = PushBlock(node, node.Locals);
				BoundNode result = base.VisitBlock(node);
				PopBlock(previousBlock);
				return result;
			}

			public override BoundNode VisitSequence(BoundSequence node)
			{
				BoundNode previousBlock = PushBlock(node, node.Locals);
				BoundNode result = base.VisitSequence(node);
				PopBlock(previousBlock);
				return result;
			}

			public override BoundNode VisitLambda(BoundLambda node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			private BoundNode VisitLambda(BoundLambda node, bool convertToExpressionTree)
			{
				seenLambda = true;
				MethodSymbol currentParent = _currentParent;
				BoundNode currentBlock = _currentBlock;
				_currentParent = node.LambdaSymbol;
				_currentBlock = node.Body;
				blockParent.Add(_currentBlock, currentBlock);
				lambdaParent.Add(node.LambdaSymbol, currentParent);
				bool inExpressionLambda = _inExpressionLambda;
				_inExpressionLambda = _inExpressionLambda || convertToExpressionTree;
				ImmutableArray<ParameterSymbol>.Enumerator enumerator = node.LambdaSymbol.Parameters.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ParameterSymbol current = enumerator.Current;
					variableScope.Add(current, _currentBlock);
					if (_inExpressionLambda)
					{
						declaredInsideExpressionLambda.Add(current);
					}
				}
				ImmutableArray<LocalSymbol>.Enumerator enumerator2 = node.Body.Locals.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					LocalSymbol current2 = enumerator2.Current;
					variableScope.Add(current2, _currentBlock);
					if (_inExpressionLambda)
					{
						declaredInsideExpressionLambda.Add(current2);
					}
				}
				BoundNode result = base.VisitBlock(node.Body);
				_inExpressionLambda = inExpressionLambda;
				_currentParent = currentParent;
				_currentBlock = currentBlock;
				return result;
			}

			public override BoundNode VisitTryCast(BoundTryCast node)
			{
				if (!(node.Operand is BoundLambda node2))
				{
					return base.VisitTryCast(node);
				}
				return VisitLambda(node2, (node.ConversionKind & ConversionKind.ConvertedToExpressionTree) != 0);
			}

			public override BoundNode VisitDirectCast(BoundDirectCast node)
			{
				if (!(node.Operand is BoundLambda node2))
				{
					return base.VisitDirectCast(node);
				}
				return VisitLambda(node2, (node.ConversionKind & ConversionKind.ConvertedToExpressionTree) != 0);
			}

			public override BoundNode VisitConversion(BoundConversion conversion)
			{
				if (!(conversion.Operand is BoundLambda node))
				{
					return base.VisitConversion(conversion);
				}
				return VisitLambda(node, (conversion.ConversionKind & ConversionKind.ConvertedToExpressionTree) != 0);
			}

			private void RecordCaptureInIntermediateBlocks(Symbol variableOrParameter)
			{
				BoundNode value = _currentBlock;
				BoundNode value2 = null;
				variableScope.TryGetValue(variableOrParameter, out value2);
				containsLiftingLambda.Add(value);
				while (value != value2 && value != null)
				{
					if (blockParent.TryGetValue(value, out value))
					{
						containsLiftingLambda.Add(value);
					}
				}
			}

			private void ReferenceVariable(Symbol variableOrParameter, SyntaxNode syntax)
			{
				if (_currentParent.MethodKind != 0 || (variableOrParameter.Kind == SymbolKind.Local && ((LocalSymbol)variableOrParameter).IsConst))
				{
					return;
				}
				Symbol containingSymbol = variableOrParameter.ContainingSymbol;
				MethodSymbol methodSymbol = _currentParent;
				bool flag = false;
				if ((object)methodSymbol != null && methodSymbol != containingSymbol)
				{
					capturedVariables.Add(variableOrParameter);
					flag = true;
					RecordCaptureInIntermediateBlocks(variableOrParameter);
					do
					{
						LambdaSymbol lambdaSymbol = (LambdaSymbol)methodSymbol;
						capturedVariablesByLambda.Add(lambdaSymbol, variableOrParameter);
						methodSymbol = lambdaParent[lambdaSymbol];
					}
					while (methodSymbol.MethodKind == MethodKind.AnonymousFunction && (object)methodSymbol != containingSymbol);
				}
				if (flag)
				{
					VerifyCaptured(variableOrParameter, syntax);
				}
			}

			private void VerifyCaptured(Symbol variableOrParameter, SyntaxNode syntax)
			{
				TypeSymbol type;
				if (variableOrParameter is ParameterSymbol parameterSymbol)
				{
					type = parameterSymbol.Type;
				}
				else
				{
					LocalSymbol localSymbol = (LocalSymbol)variableOrParameter;
					type = localSymbol.Type;
					if (localSymbol.IsByRef)
					{
						throw ExceptionUtilities.UnexpectedValue(localSymbol.IsByRef);
					}
				}
				if (TypeSymbolExtensions.IsRestrictedType(type))
				{
					if (Binder.IsTopMostEnclosingLambdaAQueryLambda(_currentParent, variableOrParameter.ContainingSymbol))
					{
						_diagnostics.Add(ERRID.ERR_CannotLiftRestrictedTypeQuery, syntax.GetLocation(), type);
					}
					else
					{
						_diagnostics.Add(ERRID.ERR_CannotLiftRestrictedTypeLambda, syntax.GetLocation(), type);
					}
				}
			}

			public override BoundNode VisitMeReference(BoundMeReference node)
			{
				ReferenceVariable(_method.MeParameter, node.Syntax);
				return base.VisitMeReference(node);
			}

			public override BoundNode VisitMyClassReference(BoundMyClassReference node)
			{
				ReferenceVariable(_method.MeParameter, node.Syntax);
				return base.VisitMyClassReference(node);
			}

			public override BoundNode VisitMyBaseReference(BoundMyBaseReference node)
			{
				ReferenceVariable(_method.MeParameter, node.Syntax);
				return base.VisitMyBaseReference(node);
			}

			public override BoundNode VisitParameter(BoundParameter node)
			{
				ReferenceVariable(node.ParameterSymbol, node.Syntax);
				return base.VisitParameter(node);
			}

			public override BoundNode VisitLocal(BoundLocal node)
			{
				ReferenceVariable(node.LocalSymbol, node.Syntax);
				return base.VisitLocal(node);
			}

			public override BoundNode VisitRangeVariable(BoundRangeVariable node)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public override BoundNode VisitLabelStatement(BoundLabelStatement node)
			{
				labelBlock.Add(node.Label, _currentBlock);
				return base.VisitLabelStatement(node);
			}

			public override BoundNode VisitConditionalGoto(BoundConditionalGoto node)
			{
				if (labelBlock.ContainsKey(node.Label))
				{
					seenBackBranches = true;
				}
				return base.VisitConditionalGoto(node);
			}

			private static bool MayParticipateInIllegalBranch(BoundGotoStatement node)
			{
				return !node.WasCompilerGenerated;
			}

			public override BoundNode VisitGotoStatement(BoundGotoStatement node)
			{
				if (labelBlock.ContainsKey(node.Label))
				{
					seenBackBranches = true;
				}
				if (MayParticipateInIllegalBranch(node))
				{
					gotoBlock.Add(node, _currentBlock);
				}
				return base.VisitGotoStatement(node);
			}

			public override BoundNode VisitMethodGroup(BoundMethodGroup node)
			{
				return null;
			}
		}

		private readonly Analysis _analysis;

		private readonly MethodSymbol _topLevelMethod;

		private readonly int _topLevelMethodOrdinal;

		private LambdaFrame _lazyStaticLambdaFrame;

		private readonly Dictionary<BoundNode, LambdaFrame> _frames;

		private readonly Dictionary<NamedTypeSymbol, Symbol> _framePointers;

		private MethodSymbol _currentMethod;

		private ParameterSymbol _currentFrameThis;

		private readonly ArrayBuilder<LambdaDebugInfo> _lambdaDebugInfoBuilder;

		private int _delegateRelaxationIdDispenser;

		private int _synthesizedFieldNameIdDispenser;

		private Symbol _innermostFramePointer;

		private TypeSubstitution _currentLambdaBodyTypeSubstitution;

		private ImmutableArray<TypeParameterSymbol> _currentTypeParameters;

		private BoundExpression _meProxyDeferredInit;

		private bool _meIsInitialized;

		private bool _meProxyDeferredInitDone;

		private bool _inExpressionLambda;

		private bool _reported_ERR_CannotUseOnErrorGotoWithClosure;

		private HashSet<BoundNode> _rewrittenNodes;

		protected override MethodSymbol CurrentMethod => _currentMethod;

		protected override MethodSymbol TopLevelMethod => _topLevelMethod;

		protected override TypeSubstitution TypeMap => _currentLambdaBodyTypeSubstitution;

		protected override bool IsInExpressionLambda => _inExpressionLambda;

		private LambdaRewriter(Analysis analysis, MethodSymbol method, int methodOrdinal, ArrayBuilder<LambdaDebugInfo> lambdaDebugInfoBuilder, int delegateRelaxationIdDispenser, VariableSlotAllocator slotAllocatorOpt, TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
			: base(slotAllocatorOpt, compilationState, diagnostics, method.PreserveOriginalLocals)
		{
			_frames = new Dictionary<BoundNode, LambdaFrame>();
			_framePointers = new Dictionary<NamedTypeSymbol, Symbol>();
			_rewrittenNodes = null;
			_topLevelMethod = method;
			_topLevelMethodOrdinal = methodOrdinal;
			_lambdaDebugInfoBuilder = lambdaDebugInfoBuilder;
			_delegateRelaxationIdDispenser = delegateRelaxationIdDispenser;
			_currentMethod = method;
			_analysis = analysis;
			_currentTypeParameters = _topLevelMethod.TypeParameters;
			_inExpressionLambda = false;
			if (!method.IsShared)
			{
				_innermostFramePointer = method.MeParameter;
				_framePointers[method.ContainingType] = method.MeParameter;
			}
			_currentFrameThis = method.MeParameter;
			_synthesizedFieldNameIdDispenser = 1;
		}

		public static BoundBlock Rewrite(BoundBlock node, MethodSymbol method, int methodOrdinal, ArrayBuilder<LambdaDebugInfo> lambdaDebugInfoBuilder, ArrayBuilder<ClosureDebugInfo> closureDebugInfoBuilder, ref int delegateRelaxationIdDispenser, VariableSlotAllocator slotAllocatorOpt, TypeCompilationState CompilationState, ISet<Symbol> symbolsCapturedWithoutCopyCtor, BindingDiagnosticBag diagnostics, HashSet<BoundNode> rewrittenNodes)
		{
			Analysis analysis = Analysis.AnalyzeMethodBody(node, method, symbolsCapturedWithoutCopyCtor, diagnostics);
			if (!analysis.seenLambda)
			{
				return node;
			}
			LambdaRewriter lambdaRewriter = new LambdaRewriter(analysis, method, methodOrdinal, lambdaDebugInfoBuilder, delegateRelaxationIdDispenser, slotAllocatorOpt, CompilationState, diagnostics);
			analysis.ComputeLambdaScopesAndFrameCaptures();
			lambdaRewriter.MakeFrames(closureDebugInfoBuilder);
			BoundBlock result = (BoundBlock)lambdaRewriter.Visit(node);
			delegateRelaxationIdDispenser = lambdaRewriter._delegateRelaxationIdDispenser;
			return result;
		}

		private void MakeFrames(ArrayBuilder<ClosureDebugInfo> closureDebugInfo)
		{
			bool seenBackBranches = _analysis.seenBackBranches;
			foreach (Symbol capturedVariable in _analysis.capturedVariables)
			{
				BoundNode value = null;
				if (_analysis.variableScope.TryGetValue(capturedVariable, out value) && !_analysis.declaredInsideExpressionLambda.Contains(capturedVariable))
				{
					LambdaFrame frameForScope = GetFrameForScope(seenBackBranches, capturedVariable, value, closureDebugInfo, ref _delegateRelaxationIdDispenser);
					LambdaCapturedVariable lambdaCapturedVariable = LambdaCapturedVariable.Create(frameForScope, capturedVariable, ref _synthesizedFieldNameIdDispenser);
					Proxies.Add(capturedVariable, lambdaCapturedVariable);
					frameForScope.CapturedLocals.Add(lambdaCapturedVariable);
				}
			}
			foreach (LambdaFrame value2 in _frames.Values)
			{
				CompilationState.AddSynthesizedMethod(value2.Constructor, MakeFrameCtor(value2, Diagnostics));
			}
		}

		private LambdaFrame GetFrameForScope(bool copyConstructor, Symbol captured, BoundNode scope, ArrayBuilder<ClosureDebugInfo> closureDebugInfo, ref int delegateRelaxationIdDispenser)
		{
			LambdaFrame value = null;
			if (!_frames.TryGetValue(scope, out value))
			{
				SyntaxNode syntax = scope.Syntax;
				SynthesizedLocal obj = captured as SynthesizedLocal;
				bool flag = (object)obj != null && obj.SynthesizedKind == SynthesizedLocalKind.DelegateRelaxationReceiver;
				DebugId methodId;
				DebugId closureId;
				if (flag)
				{
					int currentGenerationOrdinal = CompilationState.ModuleBuilderOpt.CurrentGenerationOrdinal;
					methodId = new DebugId(_topLevelMethodOrdinal, currentGenerationOrdinal);
					closureId = new DebugId(delegateRelaxationIdDispenser, currentGenerationOrdinal);
					delegateRelaxationIdDispenser++;
				}
				else
				{
					methodId = GetTopLevelMethodId();
					closureId = GetClosureId(syntax, closureDebugInfo);
				}
				value = new LambdaFrame(_topLevelMethod, syntax, methodId, closureId, copyConstructor && !_analysis.symbolsCapturedWithoutCopyCtor.Contains(captured), isStatic: false, flag);
				_frames[scope] = value;
				CompilationState.ModuleBuilderOpt.AddSynthesizedDefinition(_topLevelMethod.ContainingType, value.GetCciAdapter());
			}
			return value;
		}

		private LambdaFrame GetStaticFrame(BoundNode lambda, BindingDiagnosticBag diagnostics)
		{
			if ((object)_lazyStaticLambdaFrame == null)
			{
				bool flag = !TopLevelMethod.IsGenericMethod;
				if (flag)
				{
					_lazyStaticLambdaFrame = CompilationState.staticLambdaFrame;
				}
				if ((object)_lazyStaticLambdaFrame == null)
				{
					_lazyStaticLambdaFrame = new LambdaFrame(methodId: (!flag) ? GetTopLevelMethodId() : new DebugId(-1, CompilationState.ModuleBuilderOpt.CurrentGenerationOrdinal), topLevelMethod: _topLevelMethod, scopeSyntaxOpt: lambda.Syntax, closureId: default(DebugId), copyConstructor: false, isStatic: true, isDelegateRelaxationFrame: false);
					if (flag)
					{
						CompilationState.staticLambdaFrame = _lazyStaticLambdaFrame;
					}
					LambdaFrame lazyStaticLambdaFrame = _lazyStaticLambdaFrame;
					CompilationState.ModuleBuilderOpt.AddSynthesizedDefinition(_topLevelMethod.ContainingType, lazyStaticLambdaFrame.GetCciAdapter());
					SyntaxNode syntax = lambda.Syntax;
					CompilationState.AddSynthesizedMethod(lazyStaticLambdaFrame.Constructor, MakeFrameCtor(lazyStaticLambdaFrame, diagnostics));
					SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(lazyStaticLambdaFrame.SharedConstructor, lazyStaticLambdaFrame.SharedConstructor, syntax, CompilationState, diagnostics);
					BoundBlock body = syntheticBoundNodeFactory.Block(syntheticBoundNodeFactory.Assignment(syntheticBoundNodeFactory.Field(null, lazyStaticLambdaFrame.SingletonCache, isLValue: true), syntheticBoundNodeFactory.New(lazyStaticLambdaFrame.Constructor)), syntheticBoundNodeFactory.Return());
					CompilationState.AddSynthesizedMethod(lazyStaticLambdaFrame.SharedConstructor, body);
				}
			}
			return _lazyStaticLambdaFrame;
		}

		private BoundExpression FrameOfType(SyntaxNode syntax, NamedTypeSymbol frameType)
		{
			return FramePointer(syntax, frameType.OriginalDefinition);
		}

		internal override BoundExpression FramePointer(SyntaxNode syntax, NamedTypeSymbol frameClass)
		{
			if ((object)_currentFrameThis != null && TypeSymbol.Equals(_currentFrameThis.Type, frameClass, TypeCompareKind.ConsiderEverything))
			{
				return new BoundMeReference(syntax, frameClass);
			}
			Symbol symbol = _framePointers[frameClass];
			FieldSymbol value = null;
			if (Proxies.TryGetValue(symbol, out value))
			{
				BoundExpression boundExpression = FramePointer(syntax, value.ContainingType);
				FieldSymbol fieldSymbol = value.AsMember((NamedTypeSymbol)boundExpression.Type);
				return new BoundFieldAccess(syntax, boundExpression, fieldSymbol, isLValue: false, fieldSymbol.Type);
			}
			LocalSymbol localSymbol = (LocalSymbol)symbol;
			return new BoundLocal(syntax, localSymbol, isLValue: false, localSymbol.Type);
		}

		protected override BoundNode MaterializeProxy(BoundExpression origExpression, FieldSymbol proxy)
		{
			BoundExpression boundExpression = FramePointer(origExpression.Syntax, proxy.ContainingType);
			FieldSymbol fieldSymbol = proxy.AsMember((NamedTypeSymbol)boundExpression.Type);
			return new BoundFieldAccess(origExpression.Syntax, boundExpression, fieldSymbol, origExpression.IsLValue, fieldSymbol.Type);
		}

		private BoundBlock MakeFrameCtor(LambdaFrame frame, BindingDiagnosticBag diagnostics)
		{
			MethodSymbol constructor = frame.Constructor;
			SyntaxNode syntax = constructor.Syntax;
			ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
			instance.Add(MethodCompiler.BindDefaultConstructorInitializer(constructor, diagnostics));
			if (!constructor.Parameters.IsEmpty)
			{
				ParameterSymbol parameterSymbol = constructor.Parameters[0];
				BoundParameter boundParameter = new BoundParameter(syntax, parameterSymbol, frame);
				NamedTypeSymbol specialType = frame.ContainingAssembly.GetSpecialType(SpecialType.System_Boolean);
				UseSiteInfo<AssemblySymbol> useSiteInfo = specialType.GetUseSiteInfo();
				diagnostics.Add(useSiteInfo, syntax.GetLocation());
				NamedTypeSymbol specialType2 = frame.ContainingAssembly.GetSpecialType(SpecialType.System_Object);
				BoundBinaryOperator condition = new BoundBinaryOperator(syntax, BinaryOperatorKind.Is, new BoundDirectCast(syntax, boundParameter.MakeRValue(), ConversionKind.WideningReference, specialType2), new BoundLiteral(syntax, ConstantValue.Nothing, specialType2), @checked: false, specialType);
				GeneratedLabelSymbol label = new GeneratedLabelSymbol("Done");
				BoundConditionalGoto item = new BoundConditionalGoto(syntax, condition, jumpIfTrue: true, label);
				instance.Add(item);
				ParameterSymbol meParameter = constructor.MeParameter;
				BoundParameter receiverOpt = new BoundParameter(syntax, meParameter, frame);
				ArrayBuilder<LambdaCapturedVariable>.Enumerator enumerator = frame.CapturedLocals.GetEnumerator();
				while (enumerator.MoveNext())
				{
					LambdaCapturedVariable current = enumerator.Current;
					TypeSymbol type = current.Type;
					BoundFieldAccess left = new BoundFieldAccess(syntax, receiverOpt, current, isLValue: true, current.Type);
					BoundFieldAccess right = new BoundFieldAccess(syntax, boundParameter, current, isLValue: false, current.Type);
					BoundAssignmentOperator expression = new BoundAssignmentOperator(syntax, left, right, suppressObjectClone: true, type);
					instance.Add(new BoundExpressionStatement(syntax, expression));
				}
				instance.Add(new BoundLabelStatement(syntax, label));
			}
			instance.Add(new BoundReturnStatement(syntax, null, null, null));
			return new BoundBlock(syntax, default(SyntaxList<StatementSyntax>), ImmutableArray<LocalSymbol>.Empty, instance.ToImmutableAndFree());
		}

		internal static NamedTypeSymbol ConstructFrameType<T>(LambdaFrame type, ImmutableArray<T> typeArguments) where T : TypeSymbol
		{
			if (type.CanConstruct)
			{
				return type.Construct(StaticCast<TypeSymbol>.From(typeArguments));
			}
			return type;
		}

		private BoundNode IntroduceFrame(BoundNode node, LambdaFrame frame, Func<ArrayBuilder<BoundExpression>, ArrayBuilder<LocalSymbol>, BoundNode> F, LambdaSymbol origLambda = null)
		{
			NamedTypeSymbol namedTypeSymbol = ConstructFrameType(frame, _currentTypeParameters);
			SynthesizedLocal synthesizedLocal = new SynthesizedLocal(_topLevelMethod, namedTypeSymbol, SynthesizedLocalKind.LambdaDisplayClass, frame.ScopeSyntax);
			ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
			MethodSymbol methodSymbol = SymbolExtensions.AsMember(frame.Constructor, namedTypeSymbol);
			SyntaxNode syntax = node.Syntax;
			BoundLocal boundLocal = new BoundLocal(syntax, synthesizedLocal, namedTypeSymbol);
			ImmutableArray<BoundExpression> arguments = (methodSymbol.Parameters.IsEmpty ? ImmutableArray<BoundExpression>.Empty : ImmutableArray.Create((BoundExpression)boundLocal));
			instance.Add(new BoundAssignmentOperator(syntax, boundLocal, new BoundObjectCreationExpression(syntax, methodSymbol, arguments, null, namedTypeSymbol), suppressObjectClone: true, namedTypeSymbol));
			FieldSymbol value = null;
			if ((object)_innermostFramePointer != null)
			{
				Proxies.TryGetValue(_innermostFramePointer, out value);
				if (_analysis.needsParentFrame.Contains(node))
				{
					LambdaCapturedVariable lambdaCapturedVariable = LambdaCapturedVariable.Create(frame, _innermostFramePointer, ref _synthesizedFieldNameIdDispenser);
					FieldSymbol fieldSymbol = lambdaCapturedVariable.AsMember(namedTypeSymbol);
					BoundExpression boundExpression = new BoundFieldAccess(syntax, new BoundLocal(syntax, synthesizedLocal, namedTypeSymbol), fieldSymbol, isLValue: true, fieldSymbol.Type);
					BoundExpression right = FrameOfType(syntax, fieldSymbol.Type as NamedTypeSymbol);
					BoundAssignmentOperator boundAssignmentOperator = new BoundAssignmentOperator(syntax, boundExpression, right, suppressObjectClone: true, boundExpression.Type);
					if (_innermostFramePointer.Kind == SymbolKind.Parameter && _topLevelMethod.MethodKind == MethodKind.Constructor && (object)_topLevelMethod == _currentMethod && !_meIsInitialized)
					{
						_meProxyDeferredInit = boundAssignmentOperator;
					}
					else
					{
						instance.Add(boundAssignmentOperator);
					}
					CompilationState.ModuleBuilderOpt.AddSynthesizedDefinition(frame, lambdaCapturedVariable.GetCciAdapter());
					Proxies[_innermostFramePointer] = lambdaCapturedVariable;
				}
			}
			if ((object)origLambda != null)
			{
				ImmutableArray<ParameterSymbol>.Enumerator enumerator = origLambda.Parameters.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ParameterSymbol current = enumerator.Current;
					InitVariableProxy(syntax, current, synthesizedLocal, namedTypeSymbol, instance);
				}
			}
			else if (!_analysis.blockParent.ContainsKey(node))
			{
				ImmutableArray<ParameterSymbol>.Enumerator enumerator2 = _topLevelMethod.Parameters.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					ParameterSymbol current2 = enumerator2.Current;
					InitVariableProxy(syntax, current2, synthesizedLocal, namedTypeSymbol, instance);
				}
			}
			if (PreserveOriginalLocals)
			{
				foreach (Symbol capturedVariable in _analysis.capturedVariables)
				{
					if (capturedVariable.Kind == SymbolKind.Local)
					{
						BoundNode value2 = null;
						if (_analysis.variableScope.TryGetValue(capturedVariable, out value2) && value2 == node)
						{
							InitVariableProxy(syntax, capturedVariable, synthesizedLocal, namedTypeSymbol, instance);
						}
					}
				}
			}
			Symbol innermostFramePointer = _innermostFramePointer;
			_innermostFramePointer = synthesizedLocal;
			ArrayBuilder<LocalSymbol> instance2 = ArrayBuilder<LocalSymbol>.GetInstance();
			instance2.Add(synthesizedLocal);
			_framePointers.Add(frame, synthesizedLocal);
			BoundNode result = F(instance, instance2);
			_framePointers.Remove(frame);
			_innermostFramePointer = innermostFramePointer;
			if ((object)_innermostFramePointer != null)
			{
				if ((object)value != null)
				{
					Proxies[_innermostFramePointer] = value;
					return result;
				}
				Proxies.Remove(_innermostFramePointer);
			}
			return result;
		}

		private void InitVariableProxy(SyntaxNode syntaxNode, Symbol originalSymbol, LocalSymbol framePointer, NamedTypeSymbol frameType, ArrayBuilder<BoundExpression> prologue)
		{
			FieldSymbol value = null;
			if (!Proxies.TryGetValue(originalSymbol, out value) || _analysis.declaredInsideExpressionLambda.Contains(originalSymbol))
			{
				return;
			}
			BoundExpression right;
			switch (originalSymbol.Kind)
			{
			case SymbolKind.Parameter:
			{
				ParameterSymbol parameterSymbol = (ParameterSymbol)originalSymbol;
				ParameterSymbol value3 = null;
				if (!ParameterMap.TryGetValue(parameterSymbol, out value3))
				{
					value3 = parameterSymbol;
				}
				right = new BoundParameter(syntaxNode, value3, isLValue: false, value3.Type);
				break;
			}
			case SymbolKind.Local:
			{
				LocalSymbol localSymbol = (LocalSymbol)originalSymbol;
				LocalSymbol value2 = null;
				if (!LocalMap.TryGetValue(localSymbol, out value2))
				{
					value2 = localSymbol;
				}
				right = new BoundLocal(syntaxNode, value2, isLValue: false, value2.Type);
				break;
			}
			default:
				throw ExceptionUtilities.UnexpectedValue(originalSymbol.Kind);
			}
			FieldSymbol fieldSymbol = value.AsMember(frameType);
			BoundExpression item = new BoundAssignmentOperator(syntaxNode, new BoundFieldAccess(syntaxNode, new BoundLocal(syntaxNode, framePointer, frameType), fieldSymbol, isLValue: true, fieldSymbol.Type), right, suppressObjectClone: true, fieldSymbol.Type);
			prologue.Add(item);
		}

		public override BoundNode VisitMeReference(BoundMeReference node)
		{
			if (_currentMethod == _topLevelMethod)
			{
				return node;
			}
			if (_topLevelMethod.IsShared)
			{
				return node;
			}
			return FramePointer(node.Syntax, node.Type as NamedTypeSymbol);
		}

		public override BoundNode VisitMyClassReference(BoundMyClassReference node)
		{
			if ((object)_currentMethod != _topLevelMethod)
			{
				if ((object)_currentMethod.ContainingType != _topLevelMethod.ContainingType)
				{
					return FramePointer(node.Syntax, _topLevelMethod.ContainingType);
				}
				return new BoundMyClassReference(node.Syntax, node.Type);
			}
			return node;
		}

		public override BoundNode VisitMyBaseReference(BoundMyBaseReference node)
		{
			if ((object)_currentMethod != _topLevelMethod)
			{
				if ((object)_currentMethod.ContainingType != _topLevelMethod.ContainingType)
				{
					return FramePointer(node.Syntax, _topLevelMethod.ContainingType);
				}
				return new BoundMyBaseReference(node.Syntax, node.Type);
			}
			return node;
		}

		public override BoundNode VisitRangeVariable(BoundRangeVariable node)
		{
			throw ExceptionUtilities.Unreachable;
		}

		private BoundStatement RewriteStatementList(BoundStatementList node, ArrayBuilder<BoundExpression> prologue, ArrayBuilder<LocalSymbol> newLocals)
		{
			ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
			ArrayBuilder<BoundExpression>.Enumerator enumerator = prologue.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundExpression current = enumerator.Current;
				instance.Add(new BoundExpressionStatement(current.Syntax, current));
			}
			prologue.Free();
			ImmutableArray<BoundStatement>.Enumerator enumerator2 = node.Statements.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				BoundStatement current2 = enumerator2.Current;
				BoundStatement boundStatement = (BoundStatement)Visit(current2);
				if (boundStatement != null)
				{
					instance.Add(boundStatement);
				}
			}
			if (newLocals.Count == 0)
			{
				newLocals.Free();
				return node.Update(instance.ToImmutableAndFree());
			}
			return new BoundBlock(node.Syntax, default(SyntaxList<StatementSyntax>), newLocals.ToImmutableAndFree(), instance.ToImmutableAndFree());
		}

		public override BoundNode VisitBlock(BoundBlock node)
		{
			LambdaFrame value = null;
			if (_frames.TryGetValue(node, out value))
			{
				return IntroduceFrame(node, value, (ArrayBuilder<BoundExpression> prologue, ArrayBuilder<LocalSymbol> newLocals) => RewriteBlock(node, prologue, newLocals));
			}
			return RewriteBlock(node);
		}

		public override BoundNode VisitSequence(BoundSequence node)
		{
			LambdaFrame value = null;
			if (_frames.TryGetValue(node, out value))
			{
				return IntroduceFrame(node, value, (ArrayBuilder<BoundExpression> prologue, ArrayBuilder<LocalSymbol> newLocals) => RewriteSequence(node, prologue, newLocals));
			}
			return RewriteSequence(node);
		}

		public override BoundNode VisitCatchBlock(BoundCatchBlock node)
		{
			LambdaFrame value = null;
			if (_frames.TryGetValue(node, out value))
			{
				return IntroduceFrame(node, value, (ArrayBuilder<BoundExpression> prologue, ArrayBuilder<LocalSymbol> newLocals) => RewriteCatch(node, prologue, newLocals));
			}
			return RewriteCatch(node, ArrayBuilder<BoundExpression>.GetInstance(), ArrayBuilder<LocalSymbol>.GetInstance());
		}

		private BoundCatchBlock RewriteCatch(BoundCatchBlock node, ArrayBuilder<BoundExpression> prologue, ArrayBuilder<LocalSymbol> newLocals)
		{
			LocalSymbol localSymbol = null;
			if (newLocals.Count != 0)
			{
				localSymbol = newLocals[0];
			}
			else if ((object)node.LocalOpt != null)
			{
				LocalSymbol localOpt = node.LocalOpt;
				TypeSymbol typeSymbol = VisitType(localOpt.Type);
				if (TypeSymbol.Equals(typeSymbol, localOpt.Type, TypeCompareKind.ConsiderEverything))
				{
					localSymbol = localOpt;
				}
				else
				{
					bool wasReplaced = false;
					localSymbol = MethodToClassRewriter<FieldSymbol>.CreateReplacementLocalOrReturnSelf(localOpt, typeSymbol, onlyReplaceIfFunctionValue: false, out wasReplaced);
					LocalMap.Add(localOpt, localSymbol);
				}
			}
			BoundExpression boundExpression = (BoundExpression)Visit(node.ExceptionSourceOpt);
			if (prologue.Count != 0)
			{
				boundExpression = new BoundSequence(boundExpression.Syntax, ImmutableArray<LocalSymbol>.Empty, prologue.ToImmutable(), boundExpression, boundExpression.Type);
			}
			newLocals.Free();
			prologue.Free();
			BoundExpression errorLineNumberOpt = (BoundExpression)Visit(node.ErrorLineNumberOpt);
			BoundExpression exceptionFilterOpt = (BoundExpression)Visit(node.ExceptionFilterOpt);
			BoundBlock body = (BoundBlock)Visit(node.Body);
			return node.Update(localSymbol, boundExpression, errorLineNumberOpt, exceptionFilterOpt, body, node.IsSynthesizedAsyncCatchAll);
		}

		public override BoundNode VisitStatementList(BoundStatementList node)
		{
			LambdaFrame value = null;
			if (_frames.TryGetValue(node, out value))
			{
				return IntroduceFrame(node, value, (ArrayBuilder<BoundExpression> prologue, ArrayBuilder<LocalSymbol> newLocals) => RewriteStatementList(node, prologue, newLocals));
			}
			return base.VisitStatementList(node);
		}

		public BoundBlock RewriteLambdaAsMethod(SynthesizedLambdaMethod method, BoundLambda lambda)
		{
			_ = lambda.Syntax;
			BoundBlock body = lambda.Body;
			LambdaFrame value = null;
			BoundBlock boundBlock = null;
			boundBlock = ((!_frames.TryGetValue(body, out value)) ? RewriteBlock(body) : ((BoundBlock)IntroduceFrame(body, value, (ArrayBuilder<BoundExpression> prologue, ArrayBuilder<LocalSymbol> newLocals) => RewriteBlock(body, prologue, newLocals), lambda.LambdaSymbol)));
			StateMachineTypeSymbol stateMachineTypeOpt = null;
			VariableSlotAllocator slotAllocatorOpt = CompilationState.ModuleBuilderOpt.TryCreateVariableSlotAllocator(method, method.TopLevelMethod, Diagnostics.DiagnosticBag);
			return Rewriter.RewriteIteratorAndAsync(boundBlock, method, -1, CompilationState, Diagnostics, slotAllocatorOpt, out stateMachineTypeOpt);
		}

		public override BoundNode VisitTryCast(BoundTryCast node)
		{
			if (!(node.Operand is BoundLambda node2))
			{
				return base.VisitTryCast(node);
			}
			BoundExpression boundExpression = RewriteLambda(node2, VisitType(node.Type), (node.ConversionKind & ConversionKind.ConvertedToExpressionTree) != 0);
			if (_inExpressionLambda)
			{
				boundExpression = node.Update(boundExpression, node.ConversionKind, node.ConstantValueOpt, node.RelaxationLambdaOpt, node.Type);
			}
			return boundExpression;
		}

		public override BoundNode VisitDirectCast(BoundDirectCast node)
		{
			if (!(node.Operand is BoundLambda node2))
			{
				return base.VisitDirectCast(node);
			}
			BoundExpression boundExpression = RewriteLambda(node2, VisitType(node.Type), (node.ConversionKind & ConversionKind.ConvertedToExpressionTree) != 0);
			if (_inExpressionLambda)
			{
				boundExpression = node.Update(boundExpression, node.ConversionKind, node.SuppressVirtualCalls, node.ConstantValueOpt, node.RelaxationLambdaOpt, node.Type);
			}
			return boundExpression;
		}

		public override BoundNode VisitConversion(BoundConversion conversion)
		{
			if (!(conversion.Operand is BoundLambda node))
			{
				return base.VisitConversion(conversion);
			}
			BoundExpression boundExpression = RewriteLambda(node, VisitType(conversion.Type), (conversion.ConversionKind & ConversionKind.ConvertedToExpressionTree) != 0);
			if (_inExpressionLambda)
			{
				boundExpression = conversion.Update(boundExpression, conversion.ConversionKind, conversion.Checked, conversion.ExplicitCastInCode, conversion.ConstantValueOpt, conversion.ExtendedInfoOpt, conversion.Type);
			}
			return boundExpression;
		}

		private DebugId GetTopLevelMethodId()
		{
			DebugId? debugId;
			DebugId? debugId2 = (debugId = SlotAllocatorOpt?.MethodId);
			if (!debugId2.HasValue)
			{
				return new DebugId(_topLevelMethodOrdinal, CompilationState.ModuleBuilderOpt.CurrentGenerationOrdinal);
			}
			return debugId.GetValueOrDefault();
		}

		private DebugId GetClosureId(SyntaxNode syntax, ArrayBuilder<ClosureDebugInfo> closureDebugInfo)
		{
			DebugId closureId;
			DebugId debugId = ((SlotAllocatorOpt == null || !SlotAllocatorOpt.TryGetPreviousClosure(syntax, out closureId)) ? new DebugId(closureDebugInfo.Count, CompilationState.ModuleBuilderOpt.CurrentGenerationOrdinal) : closureId);
			int syntaxOffset = _topLevelMethod.CalculateLocalSyntaxOffset(syntax.SpanStart, syntax.SyntaxTree);
			closureDebugInfo.Add(new ClosureDebugInfo(syntaxOffset, debugId));
			return debugId;
		}

		private DebugId GetLambdaId(SyntaxNode syntax, ClosureKind closureKind, int closureOrdinal)
		{
			SyntaxNode syntaxNode;
			bool isLambdaBody;
			if (syntax is LambdaExpressionSyntax lambda)
			{
				syntaxNode = LambdaUtilities.GetLambdaExpressionLambdaBody(lambda);
				isLambdaBody = true;
			}
			else if (Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(syntax, SyntaxKind.AddressOfExpression))
			{
				syntaxNode = syntax;
				isLambdaBody = false;
			}
			else if (LambdaUtilities.IsNonUserCodeQueryLambda(syntax))
			{
				syntaxNode = syntax;
				isLambdaBody = false;
			}
			else
			{
				syntaxNode = syntax;
				isLambdaBody = true;
			}
			DebugId lambdaId;
			DebugId debugId = ((SlotAllocatorOpt == null || !SlotAllocatorOpt.TryGetPreviousLambda(syntaxNode, isLambdaBody, out lambdaId)) ? new DebugId(_lambdaDebugInfoBuilder.Count, CompilationState.ModuleBuilderOpt.CurrentGenerationOrdinal) : lambdaId);
			int syntaxOffset = _topLevelMethod.CalculateLocalSyntaxOffset(syntaxNode.SpanStart, syntaxNode.SyntaxTree);
			_lambdaDebugInfoBuilder.Add(new LambdaDebugInfo(syntaxOffset, debugId, closureOrdinal));
			return debugId;
		}

		private BoundExpression RewriteLambda(BoundLambda node, TypeSymbol type, bool convertToExpressionTree)
		{
			if (convertToExpressionTree | _inExpressionLambda)
			{
				bool inExpressionLambda = _inExpressionLambda;
				_inExpressionLambda = true;
				BoundBlock body = (BoundBlock)Visit(node.Body);
				node = node.Update(node.LambdaSymbol, body, node.Diagnostics, node.LambdaBinderOpt, node.DelegateRelaxation, node.MethodConversionKind);
				BoundExpression result = node;
				if (!inExpressionLambda)
				{
					NamedTypeSymbol delegateType = TypeSymbolExtensions.ExpressionTargetDelegate(type, CompilationState.Compilation);
					result = ExpressionLambdaRewriter.RewriteLambda(node, _currentMethod, delegateType, CompilationState, TypeMap, Diagnostics, _rewrittenNodes, base.RecursionDepth);
				}
				_inExpressionLambda = inExpressionLambda;
				return result;
			}
			BoundNode value = null;
			InstanceTypeSymbol instanceTypeSymbol;
			ClosureKind closureKind;
			int closureOrdinal;
			if (_analysis.lambdaScopes.TryGetValue(node.LambdaSymbol, out value))
			{
				instanceTypeSymbol = _frames[value];
				closureKind = ClosureKind.General;
				closureOrdinal = _frames[value].ClosureOrdinal;
			}
			else if (_analysis.capturedVariablesByLambda[node.LambdaSymbol].Count == 0)
			{
				instanceTypeSymbol = GetStaticFrame(node, Diagnostics);
				closureKind = ClosureKind.Static;
				closureOrdinal = -1;
			}
			else
			{
				instanceTypeSymbol = (InstanceTypeSymbol)_topLevelMethod.ContainingType;
				closureKind = ClosureKind.ThisOnly;
				closureOrdinal = -2;
			}
			DebugId lambdaId;
			DebugId topLevelMethodId;
			if (node.LambdaSymbol.SynthesizedKind == SynthesizedLambdaKind.DelegateRelaxationStub)
			{
				_delegateRelaxationIdDispenser++;
				int currentGenerationOrdinal = CompilationState.ModuleBuilderOpt.CurrentGenerationOrdinal;
				lambdaId = new DebugId(_delegateRelaxationIdDispenser, currentGenerationOrdinal);
				topLevelMethodId = new DebugId(_topLevelMethodOrdinal, currentGenerationOrdinal);
			}
			else
			{
				lambdaId = GetLambdaId(node.Syntax, closureKind, closureOrdinal);
				topLevelMethodId = GetTopLevelMethodId();
			}
			SynthesizedLambdaMethod synthesizedLambdaMethod = new SynthesizedLambdaMethod(instanceTypeSymbol, closureKind, _topLevelMethod, topLevelMethodId, node, lambdaId, Diagnostics);
			CompilationState.ModuleBuilderOpt.AddSynthesizedDefinition(instanceTypeSymbol, synthesizedLambdaMethod.GetCciAdapter());
			ImmutableArray<ParameterSymbol>.Enumerator enumerator = node.LambdaSymbol.Parameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ParameterSymbol current = enumerator.Current;
				ParameterMap.Add(current, synthesizedLambdaMethod.Parameters[current.Ordinal]);
			}
			MethodSymbol currentMethod = _currentMethod;
			ParameterSymbol currentFrameThis = _currentFrameThis;
			ImmutableArray<TypeParameterSymbol> currentTypeParameters = _currentTypeParameters;
			Symbol innermostFramePointer = _innermostFramePointer;
			TypeSubstitution currentLambdaBodyTypeSubstitution = _currentLambdaBodyTypeSubstitution;
			LambdaFrame lambdaFrame = instanceTypeSymbol as LambdaFrame;
			_currentMethod = synthesizedLambdaMethod;
			if (closureKind == ClosureKind.Static)
			{
				_innermostFramePointer = null;
				_currentFrameThis = null;
			}
			else
			{
				_currentFrameThis = synthesizedLambdaMethod.MeParameter;
				_innermostFramePointer = null;
				_framePointers.TryGetValue(instanceTypeSymbol, out _innermostFramePointer);
			}
			if ((object)lambdaFrame != null)
			{
				_currentTypeParameters = instanceTypeSymbol.TypeParameters;
				_currentLambdaBodyTypeSubstitution = lambdaFrame.TypeMap;
			}
			else
			{
				_currentTypeParameters = synthesizedLambdaMethod.TypeParameters;
				_currentLambdaBodyTypeSubstitution = TypeSubstitution.Create(_topLevelMethod, _topLevelMethod.TypeParameters, _currentMethod.TypeArguments);
			}
			BoundStatement body2 = RewriteLambdaAsMethod(synthesizedLambdaMethod, node);
			CompilationState.AddSynthesizedMethod(synthesizedLambdaMethod, body2);
			_currentMethod = currentMethod;
			_currentFrameThis = currentFrameThis;
			_currentTypeParameters = currentTypeParameters;
			_innermostFramePointer = innermostFramePointer;
			_currentLambdaBodyTypeSubstitution = currentLambdaBodyTypeSubstitution;
			NamedTypeSymbol namedTypeSymbol = instanceTypeSymbol;
			if ((object)lambdaFrame != null)
			{
				namedTypeSymbol = ConstructFrameType(lambdaFrame, _currentTypeParameters);
			}
			BoundExpression boundExpression;
			if (closureKind != 0)
			{
				boundExpression = FrameOfType(node.Syntax, namedTypeSymbol);
			}
			else
			{
				FieldSymbol fieldSymbol = lambdaFrame.SingletonCache.AsMember(namedTypeSymbol);
				boundExpression = new BoundFieldAccess(node.Syntax, null, fieldSymbol, isLValue: false, fieldSymbol.Type);
			}
			MethodSymbol methodSymbol = synthesizedLambdaMethod.AsMember(namedTypeSymbol);
			if (methodSymbol.IsGenericMethod)
			{
				methodSymbol = methodSymbol.Construct(StaticCast<TypeSymbol>.From(_currentTypeParameters));
			}
			BoundExpression boundExpression2 = new BoundDelegateCreationExpression(node.Syntax, boundExpression, methodSymbol, null, null, null, type);
			bool num = closureKind == ClosureKind.Static && CurrentMethod.MethodKind != MethodKind.StaticConstructor && !methodSymbol.IsGenericMethod;
			bool flag = value != null && value != _analysis.blockParent[node.Body] && InLoopOrLambda(node.Syntax, value.Syntax);
			if (num || flag)
			{
				TypeSymbol type2 = (((object)lambdaFrame == null) ? type : type.InternalSubstituteTypeParameters(lambdaFrame.TypeMap).Type);
				string name = GeneratedNames.MakeLambdaCacheFieldName((closureKind == ClosureKind.General) ? (-1) : topLevelMethodId.Ordinal, topLevelMethodId.Generation, lambdaId.Ordinal, lambdaId.Generation, node.LambdaSymbol.SynthesizedKind);
				FieldSymbol fieldSymbol2 = new SynthesizedLambdaCacheFieldSymbol(instanceTypeSymbol, node.LambdaSymbol, type2, name, _topLevelMethod, Accessibility.Public, isReadOnly: false, closureKind == ClosureKind.Static);
				CompilationState.ModuleBuilderOpt.AddSynthesizedDefinition(instanceTypeSymbol, fieldSymbol2.GetCciAdapter());
				SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(_topLevelMethod, _currentMethod, node.Syntax, CompilationState, Diagnostics);
				FieldSymbol f = fieldSymbol2.AsMember(namedTypeSymbol);
				BoundFieldAccess left = syntheticBoundNodeFactory.Field(boundExpression, f, isLValue: true);
				BoundFieldAccess boundFieldAccess = syntheticBoundNodeFactory.Field(boundExpression, f, isLValue: false);
				boundExpression2 = syntheticBoundNodeFactory.Conditional(syntheticBoundNodeFactory.ObjectReferenceEqual(boundFieldAccess, syntheticBoundNodeFactory.Null(boundFieldAccess.Type)), syntheticBoundNodeFactory.AssignmentExpression(left, boundExpression2), boundFieldAccess, boundFieldAccess.Type);
			}
			return boundExpression2;
		}

		private static bool InLoopOrLambda(SyntaxNode lambdaSyntax, SyntaxNode scopeSyntax)
		{
			SyntaxNode parent = lambdaSyntax.Parent;
			while (parent != null && parent != scopeSyntax)
			{
				switch (VisualBasicExtensions.Kind(parent))
				{
				case SyntaxKind.WhileBlock:
				case SyntaxKind.ForBlock:
				case SyntaxKind.ForEachBlock:
				case SyntaxKind.SingleLineFunctionLambdaExpression:
				case SyntaxKind.SingleLineSubLambdaExpression:
				case SyntaxKind.MultiLineFunctionLambdaExpression:
				case SyntaxKind.MultiLineSubLambdaExpression:
				case SyntaxKind.SimpleDoLoopBlock:
				case SyntaxKind.DoWhileLoopBlock:
				case SyntaxKind.DoUntilLoopBlock:
				case SyntaxKind.DoLoopWhileBlock:
				case SyntaxKind.DoLoopUntilBlock:
					return true;
				}
				parent = parent.Parent;
			}
			return false;
		}

		public override BoundNode VisitLambda(BoundLambda node)
		{
			throw ExceptionUtilities.Unreachable;
		}

		private BoundNode LowestCommonAncestor(BoundNode gotoBlock, BoundNode labelBlock)
		{
			HashSet<BoundNode> hashSet = new HashSet<BoundNode>();
			hashSet.Add(gotoBlock);
			while (_analysis.blockParent.TryGetValue(gotoBlock, out gotoBlock))
			{
				hashSet.Add(gotoBlock);
			}
			BoundNode boundNode = labelBlock;
			while (!hashSet.Contains(boundNode))
			{
				boundNode = _analysis.blockParent[boundNode];
			}
			return boundNode;
		}

		private bool IsLegalBranch(BoundNode gotoBlock, BoundNode labelBlock)
		{
			BoundNode value = gotoBlock;
			do
			{
				if (labelBlock == value)
				{
					return true;
				}
			}
			while (value != null && _analysis.blockParent.TryGetValue(value, out value));
			BoundNode boundNode = LowestCommonAncestor(gotoBlock, labelBlock);
			value = labelBlock;
			while (true)
			{
				if (value == boundNode)
				{
					return true;
				}
				if (_analysis.containsLiftingLambda.Contains(value))
				{
					break;
				}
				_analysis.blockParent.TryGetValue(value, out value);
			}
			return false;
		}

		public override BoundNode VisitGotoStatement(BoundGotoStatement node)
		{
			LabelSymbol label = node.Label;
			BoundNode value = null;
			if ((object)label != null && _analysis.labelBlock.TryGetValue(node.Label, out value))
			{
				BoundNode value2 = null;
				if (_analysis.gotoBlock.TryGetValue(node, out value2) && !IsLegalBranch(value2, value))
				{
					if (label is GeneratedUnstructuredExceptionHandlingResumeLabel generatedUnstructuredExceptionHandlingResumeLabel)
					{
						if (!_reported_ERR_CannotUseOnErrorGotoWithClosure)
						{
							_reported_ERR_CannotUseOnErrorGotoWithClosure = true;
							Diagnostics.Add(ERRID.ERR_CannotUseOnErrorGotoWithClosure, generatedUnstructuredExceptionHandlingResumeLabel.ResumeStatement.GetLocation(), generatedUnstructuredExceptionHandlingResumeLabel.ResumeStatement.ToString());
						}
					}
					else
					{
						SyntaxKind syntaxKind = VisualBasicExtensions.Kind(node.Syntax);
						if (syntaxKind == SyntaxKind.OnErrorGoToLabelStatement || syntaxKind == SyntaxKind.ResumeLabelStatement)
						{
							Diagnostics.Add(ERRID.ERR_CannotGotoNonScopeBlocksWithClosure, node.Syntax.GetLocation(), node.Syntax.ToString(), string.Empty, label.Name);
						}
						else
						{
							Diagnostics.Add(ERRID.ERR_CannotGotoNonScopeBlocksWithClosure, node.Syntax.GetLocation(), "Goto ", label.Name, label.Name);
						}
					}
					node = new BoundGotoStatement(node.Syntax, node.Label, node.LabelExpressionOpt, hasErrors: true);
				}
			}
			return base.VisitGotoStatement(node);
		}

		public override BoundNode VisitCall(BoundCall node)
		{
			BoundNode boundNode = base.VisitCall(node);
			if (boundNode.Kind == BoundKind.Call)
			{
				BoundCall node2 = (BoundCall)boundNode;
				node2 = OptimizeMethodCallForDelegateInvoke(node2);
				if ((object)_currentMethod == _topLevelMethod)
				{
					BoundExpression receiverOpt = node.ReceiverOpt;
					if (node.Method.MethodKind == MethodKind.Constructor && receiverOpt != null && BoundExpressionExtensions.IsInstanceReference(receiverOpt))
					{
						_meIsInitialized = true;
						if (_meProxyDeferredInit != null)
						{
							_meProxyDeferredInitDone = true;
							return LocalRewriter.GenerateSequenceValueSideEffects(_currentMethod, node2, ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create(_meProxyDeferredInit));
						}
					}
				}
				return node2;
			}
			return boundNode;
		}

		public override BoundNode VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node)
		{
			BoundLoweredConditionalAccess boundLoweredConditionalAccess = (BoundLoweredConditionalAccess)base.VisitLoweredConditionalAccess(node);
			if (!boundLoweredConditionalAccess.CaptureReceiver && !TypeSymbolExtensions.IsBooleanType(node.ReceiverOrCondition.Type) && node.ReceiverOrCondition.Kind != boundLoweredConditionalAccess.ReceiverOrCondition.Kind)
			{
				return boundLoweredConditionalAccess.Update(boundLoweredConditionalAccess.ReceiverOrCondition, captureReceiver: true, boundLoweredConditionalAccess.PlaceholderId, boundLoweredConditionalAccess.WhenNotNull, boundLoweredConditionalAccess.WhenNullOpt, boundLoweredConditionalAccess.Type);
			}
			return boundLoweredConditionalAccess;
		}

		private BoundCall OptimizeMethodCallForDelegateInvoke(BoundCall node)
		{
			MethodSymbol method = node.Method;
			BoundExpression receiverOpt = node.ReceiverOpt;
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(Diagnostics, CompilationState.Compilation.Assembly);
			if (method.MethodKind == MethodKind.DelegateInvoke && method.ContainingType.IsAnonymousType && receiverOpt.Kind == BoundKind.DelegateCreationExpression && Conversions.ClassifyMethodConversionForLambdaOrAnonymousDelegate(method, ((BoundDelegateCreationExpression)receiverOpt).Method, ref useSiteInfo) == MethodConversionKind.Identity)
			{
				Diagnostics.Add(node, useSiteInfo);
				BoundDelegateCreationExpression boundDelegateCreationExpression = (BoundDelegateCreationExpression)receiverOpt;
				if (!boundDelegateCreationExpression.Method.IsReducedExtensionMethod)
				{
					method = boundDelegateCreationExpression.Method;
					receiverOpt = boundDelegateCreationExpression.ReceiverOpt;
					node = node.Update(method, null, receiverOpt, node.Arguments, node.DefaultArguments, null, isLValue: false, node.SuppressObjectClone, node.Type);
				}
			}
			return node;
		}
	}
}
