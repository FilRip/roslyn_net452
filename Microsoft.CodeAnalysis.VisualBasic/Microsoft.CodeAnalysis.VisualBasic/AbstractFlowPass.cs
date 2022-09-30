using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class AbstractFlowPass<LocalState> : BoundTreeVisitor where LocalState : AbstractFlowPass<LocalState>.AbstractLocalState
	{
		internal interface AbstractLocalState
		{
			LocalState Clone();
		}

		internal struct BlockNesting
		{
			private readonly ImmutableArray<int> _path;

			public bool IsPrefixedBy(ArrayBuilder<int> other, bool ignoreLast)
			{
				int num = other.Count;
				if (ignoreLast)
				{
					num--;
				}
				if (num <= _path.Length)
				{
					int num2 = num - 1;
					for (int i = 0; i <= num2; i++)
					{
						if (_path[i] != other[i])
						{
							return false;
						}
					}
					return true;
				}
				return false;
			}

			private BlockNesting(ArrayBuilder<int> builder)
			{
				this = default(BlockNesting);
				_path = builder.ToImmutable();
			}

			public static implicit operator BlockNesting(ArrayBuilder<int> builder)
			{
				return new BlockNesting(builder);
			}
		}

		protected struct LabelStateAndNesting
		{
			public readonly BoundLabelStatement Target;

			public readonly LocalState State;

			public readonly BlockNesting Nesting;

			public LabelStateAndNesting(BoundLabelStatement target, LocalState state, BlockNesting nesting)
			{
				this = default(LabelStateAndNesting);
				Target = target;
				State = state;
				Nesting = nesting;
			}
		}

		internal class PendingBranch
		{
			public readonly BoundStatement Branch;

			public LocalState State;

			public BlockNesting Nesting;

			public LabelSymbol Label => Branch.Kind switch
			{
				BoundKind.ConditionalGoto => ((BoundConditionalGoto)Branch).Label, 
				BoundKind.GotoStatement => ((BoundGotoStatement)Branch).Label, 
				BoundKind.ExitStatement => ((BoundExitStatement)Branch).Label, 
				BoundKind.ContinueStatement => ((BoundContinueStatement)Branch).Label, 
				_ => null, 
			};

			public PendingBranch(BoundStatement branch, LocalState state, BlockNesting nesting)
			{
				Branch = branch;
				State = state.Clone();
				Nesting = nesting;
			}
		}

		protected enum RegionPlace
		{
			Before,
			Inside,
			After
		}

		protected class SavedPending
		{
			public readonly ArrayBuilder<PendingBranch> PendingBranches;

			public readonly HashSet<LabelSymbol> LabelsSeen;

			public SavedPending(ref ArrayBuilder<PendingBranch> _pendingBranches, ref HashSet<LabelSymbol> _labelsSeen)
			{
				PendingBranches = _pendingBranches;
				LabelsSeen = _labelsSeen;
				_pendingBranches = ArrayBuilder<PendingBranch>.GetInstance();
				_labelsSeen = new HashSet<LabelSymbol>();
			}
		}

		protected enum ReadWriteContext
		{
			None,
			CompoundAssignmentTarget,
			ByRefArgument
		}

		protected readonly BoundNode _firstInRegion;

		protected readonly BoundNode _lastInRegion;

		protected readonly TextSpan _region;

		protected RegionPlace _regionPlace;

		private readonly Dictionary<BoundLoopStatement, LocalState> _loopHeadState;

		private bool _invalidRegion;

		protected readonly VisualBasicCompilation compilation;

		public Symbol symbol;

		private readonly BoundNode _methodOrInitializerMainNode;

		private readonly Dictionary<LabelSymbol, LabelStateAndNesting> _labels;

		private HashSet<LabelSymbol> _labelsSeen;

		private Dictionary<BoundValuePlaceholderBase, BoundExpression> _placeholderReplacementMap;

		internal bool backwardBranchChanged;

		private ArrayBuilder<PendingBranch> _pendingBranches;

		protected LocalState State;

		protected LocalState StateWhenTrue;

		protected LocalState StateWhenFalse;

		protected bool IsConditionalState;

		protected readonly ParameterSymbol MeParameter;

		protected readonly bool TrackUnassignments;

		private ArrayBuilder<int> _nesting;

		protected readonly DiagnosticBag diagnostics;

		private readonly bool _suppressConstantExpressions;

		protected int _recursionDepth;

		protected bool IsInside => _regionPlace == RegionPlace.Inside;

		protected bool InvalidRegionDetected => _invalidRegion;

		protected ImmutableArray<PendingBranch> PendingBranches => _pendingBranches.ToImmutable();

		protected ImmutableArray<ParameterSymbol> MethodParameters
		{
			get
			{
				if (symbol.Kind != SymbolKind.Method)
				{
					return ImmutableArray<ParameterSymbol>.Empty;
				}
				return ((MethodSymbol)symbol).Parameters;
			}
		}

		protected bool ShouldAnalyzeByRefParameters
		{
			get
			{
				if (symbol.Kind == SymbolKind.Method)
				{
					return ((MethodSymbol)symbol).Locations.Length == 1;
				}
				return false;
			}
		}

		protected MethodSymbol MethodSymbol
		{
			get
			{
				if (symbol.Kind != SymbolKind.Method)
				{
					return null;
				}
				return (MethodSymbol)symbol;
			}
		}

		protected TypeSymbol MethodReturnType
		{
			get
			{
				if (symbol.Kind != SymbolKind.Method)
				{
					return null;
				}
				return ((MethodSymbol)symbol).ReturnType;
			}
		}

		protected BoundExpression GetPlaceholderSubstitute
		{
			get
			{
				BoundExpression value = null;
				if (_placeholderReplacementMap != null && _placeholderReplacementMap.TryGetValue(placeholder, out value))
				{
					return value;
				}
				return null;
			}
		}

		protected virtual bool SuppressRedimOperandRvalueOnPreserve => false;

		protected abstract bool IntersectWith(ref LocalState self, ref LocalState other);

		protected abstract void UnionWith(ref LocalState self, ref LocalState other);

		protected bool IsInsideRegion(TextSpan span)
		{
			TextSpan region;
			if (span.Length == 0)
			{
				region = _region;
				return region.Contains(span.Start);
			}
			region = _region;
			return region.Contains(span);
		}

		protected virtual void EnterRegion()
		{
			_regionPlace = RegionPlace.Inside;
		}

		protected virtual void LeaveRegion()
		{
			_regionPlace = RegionPlace.After;
		}

		protected void SetInvalidRegion()
		{
			_invalidRegion = true;
		}

		protected AbstractFlowPass(FlowAnalysisInfo _info, bool suppressConstExpressionsSupport)
			: this(_info, default(FlowAnalysisRegionInfo), suppressConstExpressionsSupport, trackUnassignments: false)
		{
		}

		protected AbstractFlowPass(FlowAnalysisInfo _info, FlowAnalysisRegionInfo _region, bool suppressConstExpressionsSupport, bool trackUnassignments)
		{
			_invalidRegion = false;
			_labels = new Dictionary<LabelSymbol, LabelStateAndNesting>();
			_labelsSeen = new HashSet<LabelSymbol>();
			backwardBranchChanged = false;
			_pendingBranches = ArrayBuilder<PendingBranch>.GetInstance();
			diagnostics = DiagnosticBag.GetInstance();
			compilation = _info.Compilation;
			symbol = _info.Symbol;
			MeParameter = SymbolExtensions.GetMeParameter(symbol);
			_methodOrInitializerMainNode = _info.Node;
			_firstInRegion = _region.FirstInRegion;
			_lastInRegion = _region.LastInRegion;
			this._region = _region.Region;
			TrackUnassignments = trackUnassignments;
			_loopHeadState = (trackUnassignments ? new Dictionary<BoundLoopStatement, LocalState>() : null);
			_suppressConstantExpressions = suppressConstExpressionsSupport;
		}

		protected virtual void InitForScan()
		{
		}

		protected abstract LocalState ReachableState();

		protected abstract LocalState UnreachableState();

		private void SetConditionalState(LocalState _whenTrue, LocalState _whenFalse)
		{
			State = default(LocalState);
			StateWhenTrue = _whenTrue;
			StateWhenFalse = _whenFalse;
			IsConditionalState = true;
		}

		protected void SetState(LocalState _state)
		{
			State = _state;
			if (IsConditionalState)
			{
				StateWhenTrue = default(LocalState);
				StateWhenFalse = default(LocalState);
				IsConditionalState = false;
			}
		}

		protected void Split()
		{
			if (!IsConditionalState)
			{
				SetConditionalState(State, State.Clone());
			}
		}

		protected void Unsplit()
		{
			if (IsConditionalState)
			{
				IntersectWith(ref StateWhenTrue, ref StateWhenFalse);
				SetState(StateWhenTrue);
			}
		}

		protected virtual bool Scan()
		{
			diagnostics.Clear();
			_regionPlace = RegionPlace.Before;
			SetState(ReachableState());
			backwardBranchChanged = false;
			if (_nesting != null)
			{
				_nesting.Free();
			}
			_nesting = ArrayBuilder<int>.GetInstance();
			InitForScan();
			SavedPending oldPending = SavePending();
			Visit(_methodOrInitializerMainNode);
			RestorePending(oldPending);
			_labelsSeen.Clear();
			if (_firstInRegion != null)
			{
				return _regionPlace == RegionPlace.After;
			}
			return true;
		}

		protected virtual bool Analyze()
		{
			do
			{
				if (!Scan())
				{
					return false;
				}
			}
			while (backwardBranchChanged);
			return true;
		}

		protected virtual void Free()
		{
			if (_nesting != null)
			{
				_nesting.Free();
			}
			diagnostics.Free();
			_pendingBranches.Free();
		}

		private LocalState LabelState(LabelSymbol label)
		{
			LabelStateAndNesting value = default(LabelStateAndNesting);
			if (_labels.TryGetValue(label, out value))
			{
				return value.State;
			}
			return UnreachableState();
		}

		protected void SetUnreachable()
		{
			SetState(UnreachableState());
		}

		private bool IsConstantTrue(BoundExpression node)
		{
			if (_suppressConstantExpressions)
			{
				return false;
			}
			if (!node.IsConstant)
			{
				return false;
			}
			ConstantValue constantValueOpt = node.ConstantValueOpt;
			if (constantValueOpt.Discriminator != ConstantValueTypeDiscriminator.Boolean)
			{
				return false;
			}
			return constantValueOpt.BooleanValue;
		}

		private bool IsConstantFalse(BoundExpression node)
		{
			if (_suppressConstantExpressions)
			{
				return false;
			}
			if (!node.IsConstant)
			{
				return false;
			}
			ConstantValue constantValueOpt = node.ConstantValueOpt;
			if (constantValueOpt.Discriminator != ConstantValueTypeDiscriminator.Boolean)
			{
				return false;
			}
			return !constantValueOpt.BooleanValue;
		}

		private bool IsConstantNull(BoundExpression node)
		{
			if (_suppressConstantExpressions)
			{
				return false;
			}
			if (!node.IsConstant)
			{
				return false;
			}
			return node.ConstantValueOpt.IsNull;
		}

		protected static bool IsNonPrimitiveValueType(TypeSymbol type)
		{
			if (!type.IsValueType)
			{
				return false;
			}
			SpecialType specialType = type.SpecialType;
			if (specialType == SpecialType.None || (uint)(specialType - 21) <= 1u || specialType == SpecialType.System_Nullable_T)
			{
				return true;
			}
			return false;
		}

		private void LoopHead(BoundLoopStatement node)
		{
			if (TrackUnassignments)
			{
				if (_loopHeadState.TryGetValue(node, out var value))
				{
					IntersectWith(ref State, ref value);
				}
				_loopHeadState[node] = State.Clone();
			}
		}

		private void LoopTail(BoundLoopStatement node)
		{
			if (TrackUnassignments)
			{
				LocalState self = _loopHeadState[node];
				if (IntersectWith(ref self, ref State))
				{
					_loopHeadState[node] = self;
					backwardBranchChanged = true;
				}
			}
		}

		private void ResolveBreaks(LocalState breakState, LabelSymbol breakLabel)
		{
			ArrayBuilder<PendingBranch> instance = ArrayBuilder<PendingBranch>.GetInstance();
			ImmutableArray<PendingBranch>.Enumerator enumerator = PendingBranches.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PendingBranch current = enumerator.Current;
				BoundKind kind = current.Branch.Kind;
				if (kind == BoundKind.ExitStatement)
				{
					if ((current.Branch as BoundExitStatement).Label == breakLabel)
					{
						IntersectWith(ref breakState, ref current.State);
					}
					else
					{
						instance.Add(current);
					}
				}
				else
				{
					instance.Add(current);
				}
			}
			ResetPendingBranches(instance);
			SetState(breakState);
		}

		private void ResolveContinues(LabelSymbol continueLabel)
		{
			ArrayBuilder<PendingBranch> instance = ArrayBuilder<PendingBranch>.GetInstance();
			ImmutableArray<PendingBranch>.Enumerator enumerator = PendingBranches.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PendingBranch current = enumerator.Current;
				BoundKind kind = current.Branch.Kind;
				if (kind == BoundKind.ContinueStatement)
				{
					if ((current.Branch as BoundContinueStatement).Label == continueLabel)
					{
						IntersectWith(ref State, ref current.State);
					}
					else
					{
						instance.Add(current);
					}
				}
				else
				{
					instance.Add(current);
				}
			}
			ResetPendingBranches(instance);
		}

		protected virtual void NoteBranch(PendingBranch pending, BoundStatement stmt, BoundLabelStatement labelStmt)
		{
		}

		private static LabelSymbol GetBranchTargetLabel(BoundStatement branch, bool gotoOnly)
		{
			return branch.Kind switch
			{
				BoundKind.GotoStatement => ((BoundGotoStatement)branch).Label, 
				BoundKind.ConditionalGoto => ((BoundConditionalGoto)branch).Label, 
				BoundKind.ExitStatement => gotoOnly ? null : ((BoundExitStatement)branch).Label, 
				BoundKind.ReturnStatement => gotoOnly ? null : ((BoundReturnStatement)branch).ExitLabelOpt, 
				BoundKind.ContinueStatement => null, 
				BoundKind.YieldStatement => null, 
				_ => throw ExceptionUtilities.UnexpectedValue(branch.Kind), 
			};
		}

		protected virtual void ResolveBranch(PendingBranch pending, LabelSymbol label, BoundLabelStatement target, ref bool labelStateChanged)
		{
			LocalState self = LabelState(target.Label);
			NoteBranch(pending, pending.Branch, target);
			if (IntersectWith(ref self, ref pending.State))
			{
				labelStateChanged = true;
				_labels[target.Label] = new LabelStateAndNesting(target, self, _nesting);
			}
		}

		private bool ResolveBranches(BoundLabelStatement target)
		{
			bool labelStateChanged = false;
			if (PendingBranches.Length > 0)
			{
				ArrayBuilder<PendingBranch> instance = ArrayBuilder<PendingBranch>.GetInstance();
				ImmutableArray<PendingBranch>.Enumerator enumerator = PendingBranches.GetEnumerator();
				while (enumerator.MoveNext())
				{
					PendingBranch current = enumerator.Current;
					LabelSymbol branchTargetLabel = GetBranchTargetLabel(current.Branch, gotoOnly: false);
					if ((object)branchTargetLabel != null && branchTargetLabel == target.Label)
					{
						ResolveBranch(current, branchTargetLabel, target, ref labelStateChanged);
					}
					else
					{
						instance.Add(current);
					}
				}
				ResetPendingBranches(instance);
			}
			return labelStateChanged;
		}

		protected SavedPending SavePending()
		{
			return new SavedPending(ref _pendingBranches, ref _labelsSeen);
		}

		private void ResetPendingBranches(ArrayBuilder<PendingBranch> newPendingBranches)
		{
			_pendingBranches.Free();
			_pendingBranches = newPendingBranches;
		}

		protected void RestorePending(SavedPending oldPending, bool mergeLabelsSeen = false)
		{
			if (ResolveBranches(_labelsSeen))
			{
				backwardBranchChanged = true;
			}
			oldPending.PendingBranches.AddRange(PendingBranches);
			ResetPendingBranches(oldPending.PendingBranches);
			if (mergeLabelsSeen)
			{
				_labelsSeen.AddAll(oldPending.LabelsSeen);
			}
			else
			{
				_labelsSeen = oldPending.LabelsSeen;
			}
		}

		private bool ResolveBranches(HashSet<LabelSymbol> labelsFilter)
		{
			bool result = false;
			if (PendingBranches.Length > 0)
			{
				ArrayBuilder<PendingBranch> instance = ArrayBuilder<PendingBranch>.GetInstance();
				ImmutableArray<PendingBranch>.Enumerator enumerator = PendingBranches.GetEnumerator();
				bool labelStateChanged = default(bool);
				while (enumerator.MoveNext())
				{
					PendingBranch current = enumerator.Current;
					LabelSymbol labelSymbol = null;
					LabelStateAndNesting labelAndNesting = default(LabelStateAndNesting);
					if (BothBranchAndLabelArePrefixedByNesting(current, labelsFilter, ignoreLast: false, out labelSymbol, out labelAndNesting))
					{
						ResolveBranch(current, labelSymbol, labelAndNesting.Target, ref labelStateChanged);
						if (labelStateChanged)
						{
							result = true;
						}
					}
					else
					{
						instance.Add(current);
					}
				}
				ResetPendingBranches(instance);
			}
			return result;
		}

		private bool BothBranchAndLabelArePrefixedByNesting(PendingBranch branch, HashSet<LabelSymbol> labelsFilter = null, bool ignoreLast = false, out LabelSymbol labelSymbol = null, out LabelStateAndNesting labelAndNesting = default(LabelStateAndNesting))
		{
			BoundStatement branch2 = branch.Branch;
			if (branch2 != null && branch.Nesting.IsPrefixedBy(_nesting, ignoreLast))
			{
				labelSymbol = GetBranchTargetLabel(branch2, gotoOnly: true);
				if ((object)labelSymbol != null && (labelsFilter == null || labelsFilter.Contains(labelSymbol)))
				{
					return _labels.TryGetValue(labelSymbol, out labelAndNesting) && labelAndNesting.Nesting.IsPrefixedBy(_nesting, ignoreLast);
				}
			}
			return false;
		}

		protected virtual BoundNode Unimplemented(BoundNode node, string feature)
		{
			return null;
		}

		protected virtual LocalState AllBitsSet()
		{
			return default(LocalState);
		}

		protected void SetPlaceholderSubstitute(BoundValuePlaceholderBase placeholder, BoundExpression newSubstitute)
		{
			if (_placeholderReplacementMap == null)
			{
				_placeholderReplacementMap = new Dictionary<BoundValuePlaceholderBase, BoundExpression>();
			}
			_placeholderReplacementMap[placeholder] = newSubstitute;
		}

		protected void RemovePlaceholderSubstitute(BoundValuePlaceholderBase placeholder)
		{
			_placeholderReplacementMap.Remove(placeholder);
		}

		protected abstract string Dump(LocalState state);

		public sealed override BoundNode Visit(BoundNode node)
		{
			Visit(node, dontLeaveRegion: false);
			return null;
		}

		protected virtual void Visit(BoundNode node, bool dontLeaveRegion)
		{
			VisitAlways(node, dontLeaveRegion);
		}

		protected void VisitAlways(BoundNode node, bool dontLeaveRegion = false)
		{
			if (_firstInRegion == null)
			{
				VisitWithStackGuard(node);
				return;
			}
			if (node == _firstInRegion && _regionPlace == RegionPlace.Before)
			{
				EnterRegion();
			}
			VisitWithStackGuard(node);
			if (!dontLeaveRegion && node == _lastInRegion && IsInside)
			{
				LeaveRegion();
			}
		}

		[DebuggerStepThrough]
		private BoundNode VisitWithStackGuard(BoundNode node)
		{
			if (node is BoundExpression node2)
			{
				return VisitExpressionWithStackGuard(ref _recursionDepth, node2);
			}
			return base.Visit(node);
		}

		[DebuggerStepThrough]
		protected override BoundExpression VisitExpressionWithoutStackGuard(BoundExpression node)
		{
			return (BoundExpression)base.Visit(node);
		}

		protected override bool ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException()
		{
			return false;
		}

		protected virtual void VisitLvalue(BoundExpression node, bool dontLeaveRegion = false)
		{
			if (node == _firstInRegion && _regionPlace == RegionPlace.Before)
			{
				EnterRegion();
			}
			switch (node.Kind)
			{
			case BoundKind.Local:
			{
				BoundLocal boundLocal = (BoundLocal)node;
				if (boundLocal.LocalSymbol.IsByRef)
				{
					VisitRvalue(boundLocal);
				}
				break;
			}
			case BoundKind.FieldAccess:
				VisitFieldAccessInternal((BoundFieldAccess)node);
				break;
			case BoundKind.WithLValueExpressionPlaceholder:
			case BoundKind.WithRValueExpressionPlaceholder:
			case BoundKind.RValuePlaceholder:
			case BoundKind.LValuePlaceholder:
			{
				BoundExpression boundExpression = this.get_GetPlaceholderSubstitute((BoundValuePlaceholderBase)node);
				if (boundExpression != null)
				{
					VisitLvalue(boundExpression);
				}
				else
				{
					VisitRvalue(node, ReadWriteContext.None, dontLeaveRegion: true);
				}
				break;
			}
			default:
				VisitRvalue(node, ReadWriteContext.None, dontLeaveRegion: true);
				break;
			case BoundKind.MeReference:
			case BoundKind.MyBaseReference:
			case BoundKind.MyClassReference:
			case BoundKind.Parameter:
				break;
			}
			if (!dontLeaveRegion && node == _lastInRegion && IsInside)
			{
				LeaveRegion();
			}
		}

		protected void VisitCondition(BoundExpression node)
		{
			if (node == _firstInRegion && _regionPlace == RegionPlace.Before)
			{
				EnterRegion();
			}
			Visit(node, dontLeaveRegion: true);
			AdjustConditionalState(node);
			if (node == _lastInRegion && IsInside)
			{
				LeaveRegion();
			}
		}

		private void AdjustConditionalState(BoundExpression node)
		{
			if (IsConstantTrue(node))
			{
				Unsplit();
				SetConditionalState(State, UnreachableState());
			}
			else if (IsConstantFalse(node))
			{
				Unsplit();
				SetConditionalState(UnreachableState(), State);
			}
			else
			{
				Split();
			}
		}

		protected void VisitRvalue(BoundExpression node, ReadWriteContext rwContext = ReadWriteContext.None, bool dontLeaveRegion = false)
		{
			if (node == _firstInRegion && _regionPlace == RegionPlace.Before)
			{
				EnterRegion();
			}
			if (rwContext == ReadWriteContext.None)
			{
				goto IL_0049;
			}
			BoundKind kind = node.Kind;
			if (kind != BoundKind.FieldAccess)
			{
				if (kind != BoundKind.Local)
				{
					goto IL_0049;
				}
				VisitLocalInReadWriteContext((BoundLocal)node, rwContext);
			}
			else
			{
				VisitFieldAccessInReadWriteContext((BoundFieldAccess)node, rwContext);
			}
			goto IL_0051;
			IL_0051:
			Unsplit();
			if (!dontLeaveRegion && node == _lastInRegion && IsInside)
			{
				LeaveRegion();
			}
			return;
			IL_0049:
			Visit(node, dontLeaveRegion: true);
			goto IL_0051;
		}

		public override BoundNode VisitSequence(BoundSequence node)
		{
			if (!node.SideEffects.IsEmpty)
			{
				ImmutableArray<BoundExpression>.Enumerator enumerator = node.SideEffects.GetEnumerator();
				while (enumerator.MoveNext())
				{
					BoundExpression current = enumerator.Current;
					VisitExpressionAsStatement(current);
				}
			}
			if (node.ValueOpt != null)
			{
				VisitRvalue(node.ValueOpt);
			}
			return null;
		}

		public override BoundNode VisitByRefArgumentPlaceholder(BoundByRefArgumentPlaceholder node)
		{
			BoundExpression boundExpression = this.get_GetPlaceholderSubstitute((BoundValuePlaceholderBase)node);
			if (boundExpression != null)
			{
				VisitRvalue(boundExpression, ReadWriteContext.ByRefArgument);
			}
			return null;
		}

		public override BoundNode VisitByRefArgumentWithCopyBack(BoundByRefArgumentWithCopyBack node)
		{
			SetPlaceholderSubstitute(node.InPlaceholder, node.OriginalArgument);
			VisitRvalue(node.InConversion);
			RemovePlaceholderSubstitute(node.InPlaceholder);
			return null;
		}

		protected virtual void VisitStatement(BoundStatement statement)
		{
			Visit(statement);
		}

		public override BoundNode VisitLValueToRValueWrapper(BoundLValueToRValueWrapper node)
		{
			Visit(node.UnderlyingLValue);
			return null;
		}

		public override BoundNode VisitWithLValueExpressionPlaceholder(BoundWithLValueExpressionPlaceholder node)
		{
			Visit(this.get_GetPlaceholderSubstitute((BoundValuePlaceholderBase)node));
			return null;
		}

		public override BoundNode VisitWithStatement(BoundWithStatement node)
		{
			bool flag = false;
			bool flag2 = false;
			if (_firstInRegion != null && _regionPlace == RegionPlace.Before && BoundNodeFinder.ContainsNode(node.OriginalExpression, _firstInRegion, _recursionDepth, ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException()))
			{
				flag = true;
				ImmutableArray<BoundExpression>.Enumerator enumerator = node.DraftInitializers.GetEnumerator();
				while (enumerator.MoveNext())
				{
					BoundExpression boundExpression = ((BoundAssignmentOperator)enumerator.Current).Right;
					if (boundExpression.Kind == BoundKind.LValueToRValueWrapper)
					{
						boundExpression = ((BoundLValueToRValueWrapper)boundExpression).UnderlyingLValue;
					}
					flag2 = true;
					if (_firstInRegion == boundExpression || !BoundNodeFinder.ContainsNode(_firstInRegion, boundExpression, _recursionDepth, ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException()))
					{
						flag2 = false;
						break;
					}
				}
			}
			if (flag && flag2)
			{
				EnterRegion();
			}
			ImmutableArray<BoundExpression>.Enumerator enumerator2 = node.DraftInitializers.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				BoundExpression current = enumerator2.Current;
				VisitRvalue(current);
			}
			if (flag && flag2)
			{
				LeaveRegion();
			}
			VisitBlock(node.Body);
			if (flag && _regionPlace != RegionPlace.After)
			{
				SetInvalidRegion();
			}
			return null;
		}

		public override BoundNode VisitAnonymousTypeCreationExpression(BoundAnonymousTypeCreationExpression node)
		{
			ImmutableArray<BoundExpression>.Enumerator enumerator = node.Arguments.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundExpression current = enumerator.Current;
				VisitRvalue(current);
			}
			return null;
		}

		public override BoundNode VisitAnonymousTypeFieldInitializer(BoundAnonymousTypeFieldInitializer node)
		{
			VisitRvalue(node.Value);
			return null;
		}

		public override BoundNode DefaultVisit(BoundNode node)
		{
			return Unimplemented(node, "flow analysis");
		}

		protected virtual void VisitLocalInReadWriteContext(BoundLocal node, ReadWriteContext rwContext)
		{
		}

		protected virtual void VisitFieldAccessInReadWriteContext(BoundFieldAccess node, ReadWriteContext rwContext)
		{
			VisitFieldAccessInternal(node);
		}

		public override BoundNode VisitLambda(BoundLambda node)
		{
			return null;
		}

		public override BoundNode VisitQueryExpression(BoundQueryExpression node)
		{
			return null;
		}

		public override BoundNode VisitLocal(BoundLocal node)
		{
			return null;
		}

		public override BoundNode VisitRangeVariable(BoundRangeVariable node)
		{
			return null;
		}

		public override BoundNode VisitLocalDeclaration(BoundLocalDeclaration node)
		{
			if (node.InitializerOpt != null)
			{
				VisitRvalue(node.InitializerOpt);
			}
			return null;
		}

		private int IntroduceBlock()
		{
			int count = _nesting.Count;
			_nesting.Add(0);
			return count;
		}

		private void FinalizeBlock(int level)
		{
			_nesting.RemoveAt(level);
		}

		private void InitializeBlockStatement(int level, ref int index)
		{
			_nesting[level] = index;
			index++;
		}

		public override BoundNode VisitBlock(BoundBlock node)
		{
			int level = IntroduceBlock();
			int index = 0;
			ImmutableArray<BoundStatement>.Enumerator enumerator = node.Statements.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundStatement current = enumerator.Current;
				InitializeBlockStatement(level, ref index);
				VisitStatement(current);
			}
			FinalizeBlock(level);
			return null;
		}

		public override BoundNode VisitExpressionStatement(BoundExpressionStatement node)
		{
			VisitExpressionAsStatement(node.Expression);
			return null;
		}

		private void VisitExpressionAsStatement(BoundExpression node)
		{
			VisitRvalue(node);
		}

		public override BoundNode VisitLateMemberAccess(BoundLateMemberAccess node)
		{
			VisitRvalue(node.ReceiverOpt);
			return null;
		}

		public override BoundNode VisitLateInvocation(BoundLateInvocation node)
		{
			BoundExpression member = node.Member;
			Visit(node.Member);
			ImmutableArray<BoundExpression> argumentsOpt = node.ArgumentsOpt;
			if (!argumentsOpt.IsEmpty)
			{
				VisitLateBoundArguments(isByRef: member.Kind == BoundKind.LateMemberAccess, arguments: argumentsOpt);
			}
			return null;
		}

		private void VisitLateBoundArguments(ImmutableArray<BoundExpression> arguments, bool isByRef)
		{
			ImmutableArray<BoundExpression>.Enumerator enumerator = arguments.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundExpression current = enumerator.Current;
				VisitLateBoundArgument(current, isByRef);
			}
			if (isByRef)
			{
				ImmutableArray<BoundExpression>.Enumerator enumerator2 = arguments.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					BoundExpression current2 = enumerator2.Current;
					WriteArgument(current2, isOut: false);
				}
			}
		}

		protected virtual void VisitLateBoundArgument(BoundExpression arg, bool isByRef)
		{
			if (isByRef)
			{
				VisitLvalue(arg);
			}
			else
			{
				VisitRvalue(arg);
			}
		}

		public override BoundNode VisitCall(BoundCall node)
		{
			bool num = node.Method.CallsAreOmitted(node.Syntax, node.SyntaxTree);
			LocalState state = default(LocalState);
			if (num)
			{
				state = State.Clone();
				SetUnreachable();
			}
			BoundMethodGroup methodGroupOpt = node.MethodGroupOpt;
			BoundExpression receiverOpt = node.ReceiverOpt;
			MethodSymbol method = node.Method;
			if (methodGroupOpt != null && _firstInRegion == methodGroupOpt && _regionPlace == RegionPlace.Before)
			{
				EnterRegion();
			}
			if (receiverOpt != null)
			{
				VisitCallReceiver(receiverOpt, method);
			}
			else
			{
				BoundExpression boundExpression = methodGroupOpt?.ReceiverOpt;
				if (boundExpression != null && !boundExpression.WasCompilerGenerated)
				{
					BoundKind kind = boundExpression.Kind;
					if (kind != BoundKind.TypeExpression && kind != BoundKind.NamespaceExpression && kind != BoundKind.TypeOrValueExpression)
					{
						VisitUnreachableReceiver(boundExpression);
					}
				}
			}
			if (methodGroupOpt != null && _lastInRegion == methodGroupOpt && IsInside)
			{
				LeaveRegion();
			}
			VisitArguments(node.Arguments, method.Parameters);
			if (receiverOpt != null && receiverOpt.IsLValue)
			{
				WriteLValueCallReceiver(receiverOpt, method);
			}
			if (num)
			{
				SetState(state);
			}
			return null;
		}

		private void VisitCallReceiver(BoundExpression receiver, MethodSymbol method)
		{
			if (!method.IsReducedExtensionMethod || !BoundExpressionExtensions.IsValue(receiver))
			{
				VisitRvalue(receiver);
			}
			else
			{
				VisitArgument(receiver, method.CallsiteReducedFromMethod.Parameters[0]);
			}
		}

		private void WriteLValueCallReceiver(BoundExpression receiver, MethodSymbol method)
		{
			if (receiver.Type.IsReferenceType)
			{
				MethodSymbol callsiteReducedFromMethod = method.CallsiteReducedFromMethod;
				if ((object)callsiteReducedFromMethod == null || callsiteReducedFromMethod.ParameterCount == 0 || !callsiteReducedFromMethod.Parameters[0].IsByRef)
				{
					return;
				}
			}
			WriteArgument(receiver, isOut: false);
		}

		private void VisitArguments(ImmutableArray<BoundExpression> arguments, ImmutableArray<ParameterSymbol> parameters)
		{
			if (parameters.IsDefault)
			{
				ImmutableArray<BoundExpression>.Enumerator enumerator = arguments.GetEnumerator();
				while (enumerator.MoveNext())
				{
					BoundExpression current = enumerator.Current;
					VisitRvalue(current);
				}
				return;
			}
			int num = Math.Min(parameters.Length, arguments.Length);
			int num2 = num - 1;
			for (int i = 0; i <= num2; i++)
			{
				VisitArgument(arguments[i], parameters[i]);
			}
			int num3 = num - 1;
			for (int j = 0; j <= num3; j++)
			{
				if (parameters[j].IsByRef)
				{
					WriteArgument(arguments[j], parameters[j].IsOut);
				}
			}
		}

		protected virtual void WriteArgument(BoundExpression arg, bool isOut)
		{
		}

		protected virtual void VisitArgument(BoundExpression arg, ParameterSymbol p)
		{
			if (p.IsByRef)
			{
				VisitLvalue(arg);
			}
			else
			{
				VisitRvalue(arg);
			}
		}

		public override BoundNode VisitDelegateCreationExpression(BoundDelegateCreationExpression node)
		{
			BoundMethodGroup methodGroupOpt = node.MethodGroupOpt;
			if (methodGroupOpt != null && _firstInRegion == methodGroupOpt && _regionPlace == RegionPlace.Before)
			{
				EnterRegion();
			}
			BoundExpression receiverOpt = node.ReceiverOpt;
			MethodSymbol method = node.Method;
			if (receiverOpt != null)
			{
				if (!method.IsReducedExtensionMethod || !BoundExpressionExtensions.IsValue(receiverOpt))
				{
					VisitRvalue(receiverOpt);
				}
				else
				{
					VisitArgument(receiverOpt, method.CallsiteReducedFromMethod.Parameters[0]);
				}
			}
			else
			{
				BoundExpression boundExpression = methodGroupOpt?.ReceiverOpt;
				if (boundExpression != null && !boundExpression.WasCompilerGenerated)
				{
					BoundKind kind = boundExpression.Kind;
					if (kind != BoundKind.TypeExpression && kind != BoundKind.NamespaceExpression && kind != BoundKind.TypeOrValueExpression)
					{
						VisitUnreachableReceiver(boundExpression);
					}
				}
			}
			if (methodGroupOpt != null && _lastInRegion == methodGroupOpt && IsInside)
			{
				LeaveRegion();
			}
			return null;
		}

		public override BoundNode VisitBadExpression(BoundBadExpression node)
		{
			ImmutableArray<BoundExpression>.Enumerator enumerator = node.ChildBoundNodes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundExpression current = enumerator.Current;
				VisitRvalue(current);
			}
			return null;
		}

		public override BoundNode VisitBadStatement(BoundBadStatement node)
		{
			ImmutableArray<BoundNode>.Enumerator enumerator = node.ChildBoundNodes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundNode current = enumerator.Current;
				if (current is BoundStatement)
				{
					VisitStatement(current as BoundStatement);
				}
				else if (current is BoundExpression node2)
				{
					VisitExpressionAsStatement(node2);
				}
				else
				{
					Visit(current);
				}
			}
			return null;
		}

		public override BoundNode VisitBadVariable(BoundBadVariable node)
		{
			if (node.IsLValue)
			{
				VisitLvalue(node.Expression);
			}
			else
			{
				VisitRvalue(node.Expression);
			}
			return null;
		}

		public override BoundNode VisitTypeExpression(BoundTypeExpression node)
		{
			VisitUnreachableReceiver(node.UnevaluatedReceiverOpt);
			return null;
		}

		public override BoundNode VisitLiteral(BoundLiteral node)
		{
			return null;
		}

		public override BoundNode VisitConversion(BoundConversion node)
		{
			VisitRvalue(node.Operand);
			return null;
		}

		public override BoundNode VisitRelaxationLambda(BoundRelaxationLambda node)
		{
			throw ExceptionUtilities.Unreachable;
		}

		public override BoundNode VisitConvertedTupleElements(BoundConvertedTupleElements node)
		{
			throw ExceptionUtilities.Unreachable;
		}

		public override BoundNode VisitUserDefinedConversion(BoundUserDefinedConversion node)
		{
			VisitRvalue(node.UnderlyingExpression);
			return null;
		}

		public override BoundNode VisitIfStatement(BoundIfStatement node)
		{
			VisitCondition(node.Condition);
			LocalState stateWhenTrue = StateWhenTrue;
			LocalState stateWhenFalse = StateWhenFalse;
			SetState(stateWhenTrue);
			VisitStatement(node.Consequence);
			stateWhenTrue = State;
			SetState(stateWhenFalse);
			if (node.AlternativeOpt != null)
			{
				VisitStatement(node.AlternativeOpt);
			}
			IntersectWith(ref State, ref stateWhenTrue);
			return null;
		}

		public override BoundNode VisitTernaryConditionalExpression(BoundTernaryConditionalExpression node)
		{
			VisitCondition(node.Condition);
			LocalState stateWhenTrue = StateWhenTrue;
			LocalState stateWhenFalse = StateWhenFalse;
			if (IsConstantTrue(node.Condition))
			{
				SetState(stateWhenFalse);
				VisitRvalue(node.WhenFalse);
				SetState(stateWhenTrue);
				VisitRvalue(node.WhenTrue);
			}
			else if (IsConstantFalse(node.Condition))
			{
				SetState(stateWhenTrue);
				VisitRvalue(node.WhenTrue);
				SetState(stateWhenFalse);
				VisitRvalue(node.WhenFalse);
			}
			else
			{
				SetState(stateWhenTrue);
				VisitRvalue(node.WhenTrue);
				Unsplit();
				stateWhenTrue = State;
				SetState(stateWhenFalse);
				VisitRvalue(node.WhenFalse);
				Unsplit();
				IntersectWith(ref State, ref stateWhenTrue);
			}
			return null;
		}

		public override BoundNode VisitBinaryConditionalExpression(BoundBinaryConditionalExpression node)
		{
			VisitRvalue(node.TestExpression);
			if (node.TestExpression.IsConstant && node.TestExpression.ConstantValueOpt.IsNothing)
			{
				VisitRvalue(node.ElseExpression);
			}
			else
			{
				LocalState state = State.Clone();
				VisitRvalue(node.ElseExpression);
				SetState(state);
			}
			return null;
		}

		public override BoundNode VisitConditionalAccess(BoundConditionalAccess node)
		{
			VisitRvalue(node.Receiver);
			if (node.Receiver.IsConstant)
			{
				if (node.Receiver.ConstantValueOpt.IsNothing)
				{
					LocalState state = State.Clone();
					SetUnreachable();
					VisitRvalue(node.AccessExpression);
					SetState(state);
				}
				else
				{
					VisitRvalue(node.AccessExpression);
				}
			}
			else
			{
				LocalState other = State.Clone();
				VisitRvalue(node.AccessExpression);
				IntersectWith(ref State, ref other);
			}
			return null;
		}

		public override BoundNode VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node)
		{
			VisitRvalue(node.ReceiverOrCondition);
			LocalState other = State.Clone();
			VisitRvalue(node.WhenNotNull);
			IntersectWith(ref State, ref other);
			if (node.WhenNullOpt != null)
			{
				other = State.Clone();
				VisitRvalue(node.WhenNullOpt);
				IntersectWith(ref State, ref other);
			}
			return null;
		}

		public override BoundNode VisitComplexConditionalAccessReceiver(BoundComplexConditionalAccessReceiver node)
		{
			LocalState other = State.Clone();
			VisitLvalue(node.ValueTypeReceiver);
			IntersectWith(ref State, ref other);
			other = State.Clone();
			VisitRvalue(node.ReferenceTypeReceiver);
			IntersectWith(ref State, ref other);
			return null;
		}

		public override BoundNode VisitConditionalAccessReceiverPlaceholder(BoundConditionalAccessReceiverPlaceholder node)
		{
			return null;
		}

		public override BoundNode VisitReturnStatement(BoundReturnStatement node)
		{
			if (!node.IsEndOfMethodReturn())
			{
				VisitRvalue(node.ExpressionOpt);
				_pendingBranches.Add(new PendingBranch(node, State, _nesting));
				SetUnreachable();
			}
			return null;
		}

		public override BoundNode VisitYieldStatement(BoundYieldStatement node)
		{
			VisitRvalue(node.Expression);
			_pendingBranches.Add(new PendingBranch(node, State, _nesting));
			return null;
		}

		public override BoundNode VisitStopStatement(BoundStopStatement node)
		{
			return null;
		}

		public override BoundNode VisitEndStatement(BoundEndStatement node)
		{
			SetUnreachable();
			return null;
		}

		public override BoundNode VisitMeReference(BoundMeReference node)
		{
			return null;
		}

		public override BoundNode VisitParameter(BoundParameter node)
		{
			return null;
		}

		public override BoundNode VisitObjectCreationExpression(BoundObjectCreationExpression node)
		{
			ImmutableArray<BoundExpression>.Enumerator enumerator = node.Arguments.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundExpression current = enumerator.Current;
				VisitRvalue(current);
			}
			VisitObjectCreationExpressionInitializer(node.InitializerOpt);
			return null;
		}

		public override BoundNode VisitNoPiaObjectCreationExpression(BoundNoPiaObjectCreationExpression node)
		{
			VisitObjectCreationExpressionInitializer(node.InitializerOpt);
			return null;
		}

		protected virtual void VisitObjectCreationExpressionInitializer(BoundObjectInitializerExpressionBase node)
		{
			Visit(node);
		}

		private BoundNode VisitObjectInitializerExpressionBase(BoundObjectInitializerExpressionBase node)
		{
			ImmutableArray<BoundExpression>.Enumerator enumerator = node.Initializers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundExpression current = enumerator.Current;
				VisitExpressionAsStatement(current);
			}
			return null;
		}

		public override BoundNode VisitCollectionInitializerExpression(BoundCollectionInitializerExpression node)
		{
			return VisitObjectInitializerExpressionBase(node);
		}

		public override BoundNode VisitObjectInitializerExpression(BoundObjectInitializerExpression node)
		{
			return VisitObjectInitializerExpressionBase(node);
		}

		public override BoundNode VisitNewT(BoundNewT node)
		{
			Visit(node.InitializerOpt);
			return null;
		}

		public override BoundNode VisitRedimStatement(BoundRedimStatement node)
		{
			ImmutableArray<BoundRedimClause>.Enumerator enumerator = node.Clauses.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundRedimClause current = enumerator.Current;
				Visit(current);
			}
			return null;
		}

		public override BoundNode VisitEraseStatement(BoundEraseStatement node)
		{
			ImmutableArray<BoundAssignmentOperator>.Enumerator enumerator = node.Clauses.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundAssignmentOperator current = enumerator.Current;
				Visit(current);
			}
			return null;
		}

		public override BoundNode VisitRedimClause(BoundRedimClause node)
		{
			if (node.Preserve && !SuppressRedimOperandRvalueOnPreserve)
			{
				VisitRvalue(node.Operand);
			}
			VisitLvalue(node.Operand);
			ImmutableArray<BoundExpression>.Enumerator enumerator = node.Indices.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundExpression current = enumerator.Current;
				VisitRvalue(current);
			}
			return null;
		}

		public override BoundNode VisitAssignmentOperator(BoundAssignmentOperator node)
		{
			if (node.LeftOnTheRightOpt == null)
			{
				VisitLvalue(node.Left);
			}
			else
			{
				SetPlaceholderSubstitute(node.LeftOnTheRightOpt, node.Left);
			}
			VisitRvalue(node.Right);
			if (node.LeftOnTheRightOpt != null)
			{
				RemovePlaceholderSubstitute(node.LeftOnTheRightOpt);
			}
			return null;
		}

		public override BoundNode VisitMidResult(BoundMidResult node)
		{
			VisitRvalue(node.Original);
			VisitRvalue(node.Start);
			if (node.LengthOpt != null)
			{
				VisitRvalue(node.LengthOpt);
			}
			VisitRvalue(node.Source);
			return null;
		}

		public override BoundNode VisitReferenceAssignment(BoundReferenceAssignment node)
		{
			VisitRvalue(node.ByRefLocal);
			VisitRvalue(node.LValue);
			return null;
		}

		public override BoundNode VisitCompoundAssignmentTargetPlaceholder(BoundCompoundAssignmentTargetPlaceholder node)
		{
			BoundExpression boundExpression = this.get_GetPlaceholderSubstitute((BoundValuePlaceholderBase)node);
			if (boundExpression != null)
			{
				VisitRvalue(boundExpression, ReadWriteContext.CompoundAssignmentTarget);
			}
			return null;
		}

		public override BoundNode VisitFieldAccess(BoundFieldAccess node)
		{
			VisitFieldAccessInternal(node);
			return null;
		}

		private BoundNode VisitFieldAccessInternal(BoundFieldAccess node)
		{
			BoundExpression receiverOpt = node.ReceiverOpt;
			if (FieldAccessMayRequireTracking(node))
			{
				VisitLvalue(receiverOpt);
			}
			else if (node.FieldSymbol.IsShared)
			{
				VisitUnreachableReceiver(receiverOpt);
			}
			else
			{
				VisitRvalue(receiverOpt);
			}
			return null;
		}

		protected static bool FieldAccessMayRequireTracking(BoundFieldAccess fieldAccess)
		{
			if (fieldAccess.FieldSymbol.IsShared)
			{
				return false;
			}
			BoundExpression receiverOpt = fieldAccess.ReceiverOpt;
			if (receiverOpt == null)
			{
				return false;
			}
			return IsNonPrimitiveValueType(receiverOpt.Type);
		}

		public override BoundNode VisitPropertyAccess(BoundPropertyAccess node)
		{
			BoundPropertyGroup propertyGroupOpt = node.PropertyGroupOpt;
			if (propertyGroupOpt != null && _firstInRegion == propertyGroupOpt && _regionPlace == RegionPlace.Before)
			{
				EnterRegion();
			}
			if (node.ReceiverOpt != null)
			{
				VisitRvalue(node.ReceiverOpt);
			}
			else
			{
				BoundExpression boundExpression = propertyGroupOpt?.ReceiverOpt;
				if (boundExpression != null && !boundExpression.WasCompilerGenerated)
				{
					BoundKind kind = boundExpression.Kind;
					if (kind != BoundKind.TypeExpression && kind != BoundKind.NamespaceExpression && kind != BoundKind.TypeOrValueExpression)
					{
						VisitUnreachableReceiver(boundExpression);
					}
				}
			}
			if (propertyGroupOpt != null && _lastInRegion == propertyGroupOpt && IsInside)
			{
				LeaveRegion();
			}
			ImmutableArray<BoundExpression>.Enumerator enumerator = node.Arguments.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundExpression current = enumerator.Current;
				VisitRvalue(current);
			}
			return null;
		}

		private void VisitUnreachableReceiver(BoundExpression receiver)
		{
			if (receiver != null)
			{
				LocalState state = State.Clone();
				SetUnreachable();
				VisitRvalue(receiver);
				SetState(state);
			}
		}

		public override BoundNode VisitAsNewLocalDeclarations(BoundAsNewLocalDeclarations node)
		{
			VisitRvalue(node.Initializer);
			ImmutableArray<BoundLocalDeclaration>.Enumerator enumerator = node.LocalDeclarations.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundLocalDeclaration current = enumerator.Current;
				Visit(current);
			}
			return null;
		}

		public override BoundNode VisitDimStatement(BoundDimStatement node)
		{
			ImmutableArray<BoundLocalDeclarationBase>.Enumerator enumerator = node.LocalDeclarations.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundLocalDeclarationBase current = enumerator.Current;
				VisitStatement(current);
			}
			return null;
		}

		public override BoundNode VisitWhileStatement(BoundWhileStatement node)
		{
			LoopHead(node);
			VisitCondition(node.Condition);
			LocalState stateWhenTrue = StateWhenTrue;
			LocalState stateWhenFalse = StateWhenFalse;
			SetState(stateWhenTrue);
			VisitStatement(node.Body);
			ResolveContinues(node.ContinueLabel);
			LoopTail(node);
			ResolveBreaks(stateWhenFalse, node.ExitLabel);
			return null;
		}

		public override BoundNode VisitSelectStatement(BoundSelectStatement node)
		{
			VisitStatement(node.ExpressionStatement);
			ImmutableArray<BoundCaseBlock> caseBlocks = node.CaseBlocks;
			if (caseBlocks.Any())
			{
				VisitCaseBlocks(caseBlocks);
				ResolveBreaks(State, node.ExitLabel);
			}
			return null;
		}

		private void VisitCaseBlocks(ImmutableArray<BoundCaseBlock> caseBlocks)
		{
			ArrayBuilder<LocalState> instance = ArrayBuilder<LocalState>.GetInstance(caseBlocks.Length);
			bool flag = false;
			int num = 0;
			int num2 = caseBlocks.Length - 1;
			ImmutableArray<BoundCaseBlock>.Enumerator enumerator = caseBlocks.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundCaseBlock current = enumerator.Current;
				VisitStatement(current.CaseStatement);
				LocalState state = State.Clone();
				VisitStatement(current.Body);
				flag = flag || VisualBasicExtensions.Kind(current.Syntax) == SyntaxKind.CaseElseBlock;
				if (num != num2 || !flag)
				{
					instance.Add(State.Clone());
					SetState(state);
				}
				num++;
			}
			ArrayBuilder<LocalState>.Enumerator enumerator2 = instance.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				LocalState other = enumerator2.Current;
				IntersectWith(ref State, ref other);
			}
			instance.Free();
		}

		public override BoundNode VisitCaseBlock(BoundCaseBlock node)
		{
			throw ExceptionUtilities.Unreachable;
		}

		public override BoundNode VisitCaseStatement(BoundCaseStatement node)
		{
			if (node.ConditionOpt != null)
			{
				VisitRvalue(node.ConditionOpt);
			}
			else
			{
				ImmutableArray<BoundCaseClause>.Enumerator enumerator = node.CaseClauses.GetEnumerator();
				while (enumerator.MoveNext())
				{
					BoundCaseClause current = enumerator.Current;
					switch (current.Kind)
					{
					case BoundKind.RelationalCaseClause:
						VisitRelationalCaseClause((BoundRelationalCaseClause)current);
						break;
					case BoundKind.SimpleCaseClause:
						VisitSimpleCaseClause((BoundSimpleCaseClause)current);
						break;
					case BoundKind.RangeCaseClause:
						VisitRangeCaseClause((BoundRangeCaseClause)current);
						break;
					default:
						throw ExceptionUtilities.UnexpectedValue(current.Kind);
					}
				}
			}
			return null;
		}

		public override BoundNode VisitRelationalCaseClause(BoundRelationalCaseClause node)
		{
			if (node.ValueOpt != null)
			{
				VisitRvalue(node.ValueOpt);
			}
			else
			{
				VisitRvalue(node.ConditionOpt);
			}
			return null;
		}

		public override BoundNode VisitSimpleCaseClause(BoundSimpleCaseClause node)
		{
			if (node.ValueOpt != null)
			{
				VisitRvalue(node.ValueOpt);
			}
			else
			{
				VisitRvalue(node.ConditionOpt);
			}
			return null;
		}

		public override BoundNode VisitRangeCaseClause(BoundRangeCaseClause node)
		{
			if (node.LowerBoundOpt != null)
			{
				VisitRvalue(node.LowerBoundOpt);
			}
			else
			{
				VisitRvalue(node.LowerBoundConditionOpt);
			}
			if (node.UpperBoundOpt != null)
			{
				VisitRvalue(node.UpperBoundOpt);
			}
			else
			{
				VisitRvalue(node.UpperBoundConditionOpt);
			}
			return null;
		}

		protected virtual void VisitForControlInitialization(BoundForToStatement node)
		{
			VisitLvalue(node.ControlVariable);
		}

		protected virtual void VisitForControlInitialization(BoundForEachStatement node)
		{
			VisitLvalue(node.ControlVariable);
		}

		protected virtual void VisitForInitValues(BoundForToStatement node)
		{
			VisitRvalue(node.InitialValue);
			VisitRvalue(node.LimitValue);
			VisitRvalue(node.StepValue);
		}

		protected virtual void VisitForStatementVariableDeclaration(BoundForStatement node)
		{
		}

		public override BoundNode VisitForEachStatement(BoundForEachStatement node)
		{
			VisitForStatementVariableDeclaration(node);
			VisitRvalue(node.Collection);
			LoopHead(node);
			Split();
			LocalState stateWhenTrue = StateWhenTrue;
			LocalState stateWhenFalse = StateWhenFalse;
			SetState(stateWhenTrue);
			VisitForControlInitialization(node);
			VisitStatement(node.Body);
			ResolveContinues(node.ContinueLabel);
			LoopTail(node);
			ResolveBreaks(stateWhenFalse, node.ExitLabel);
			return null;
		}

		public override BoundNode VisitUsingStatement(BoundUsingStatement node)
		{
			if (node.ResourceExpressionOpt != null)
			{
				VisitRvalue(node.ResourceExpressionOpt);
			}
			else
			{
				ImmutableArray<BoundLocalDeclarationBase>.Enumerator enumerator = node.ResourceList.GetEnumerator();
				while (enumerator.MoveNext())
				{
					BoundLocalDeclarationBase current = enumerator.Current;
					Visit(current);
				}
			}
			VisitStatement(node.Body);
			return null;
		}

		public override BoundNode VisitForToStatement(BoundForToStatement node)
		{
			VisitForStatementVariableDeclaration(node);
			VisitForInitValues(node);
			VisitForControlInitialization(node);
			LoopHead(node);
			Split();
			LocalState stateWhenTrue = StateWhenTrue;
			LocalState stateWhenFalse = StateWhenFalse;
			SetState(stateWhenTrue);
			VisitStatement(node.Body);
			ResolveContinues(node.ContinueLabel);
			LoopTail(node);
			ResolveBreaks(stateWhenFalse, node.ExitLabel);
			return null;
		}

		public override BoundNode VisitTryStatement(BoundTryStatement node)
		{
			SavedPending oldPending = SavePending();
			int level = IntroduceBlock();
			int index = 0;
			LocalState tryState = State.Clone();
			InitializeBlockStatement(level, ref index);
			VisitTryBlock(node.TryBlock, node, ref tryState);
			LocalState finallyState = tryState.Clone();
			LocalState self = State;
			ImmutableArray<BoundCatchBlock>.Enumerator enumerator = node.CatchBlocks.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundCatchBlock current = enumerator.Current;
				SetState(tryState.Clone());
				InitializeBlockStatement(level, ref index);
				VisitCatchBlock(current, ref finallyState);
				IntersectWith(ref self, ref State);
			}
			if (node.FinallyBlockOpt != null)
			{
				SavedPending savedPending = SavePending();
				SetState(finallyState);
				LocalState unsetInFinally = AllBitsSet();
				InitializeBlockStatement(level, ref index);
				VisitFinallyBlock(node.FinallyBlockOpt, ref unsetInFinally);
				ArrayBuilder<PendingBranch>.Enumerator enumerator2 = savedPending.PendingBranches.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					PendingBranch current2 = enumerator2.Current;
					bool flag = current2.Branch.Kind != BoundKind.YieldStatement;
					if (flag)
					{
						LabelSymbol labelSymbol = null;
						LabelStateAndNesting labelAndNesting = default(LabelStateAndNesting);
						if (BothBranchAndLabelArePrefixedByNesting(current2, null, ignoreLast: true, out labelSymbol, out labelAndNesting))
						{
							flag = false;
						}
					}
					if (flag)
					{
						UnionWith(ref current2.State, ref State);
						if (TrackUnassignments)
						{
							IntersectWith(ref current2.State, ref unsetInFinally);
						}
					}
				}
				RestorePending(savedPending);
				UnionWith(ref self, ref State);
				if (TrackUnassignments)
				{
					IntersectWith(ref self, ref unsetInFinally);
				}
			}
			SetState(self);
			FinalizeBlock(level);
			if ((object)node.ExitLabelOpt != null)
			{
				ResolveBreaks(self, node.ExitLabelOpt);
			}
			RestorePending(oldPending);
			return null;
		}

		protected virtual void VisitTryBlock(BoundStatement tryBlock, BoundTryStatement node, ref LocalState tryState)
		{
			VisitStatement(tryBlock);
		}

		protected virtual void VisitCatchBlock(BoundCatchBlock catchBlock, ref LocalState finallyState)
		{
			if (catchBlock.ExceptionSourceOpt != null)
			{
				VisitLvalue(catchBlock.ExceptionSourceOpt);
			}
			if (catchBlock.ErrorLineNumberOpt != null)
			{
				VisitRvalue(catchBlock.ErrorLineNumberOpt);
			}
			if (catchBlock.ExceptionFilterOpt != null)
			{
				VisitRvalue(catchBlock.ExceptionFilterOpt);
			}
			VisitBlock(catchBlock.Body);
		}

		protected virtual void VisitFinallyBlock(BoundStatement finallyBlock, ref LocalState unsetInFinally)
		{
			VisitStatement(finallyBlock);
		}

		public override BoundNode VisitArrayAccess(BoundArrayAccess node)
		{
			VisitRvalue(node.Expression);
			ImmutableArray<BoundExpression>.Enumerator enumerator = node.Indices.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundExpression current = enumerator.Current;
				VisitRvalue(current);
			}
			return null;
		}

		public sealed override BoundNode VisitBinaryOperator(BoundBinaryOperator node)
		{
			ArrayBuilder<BoundBinaryOperator> instance = ArrayBuilder<BoundBinaryOperator>.GetInstance();
			BoundBinaryOperator boundBinaryOperator = node;
			BoundExpression left = node.Left;
			while (left.Kind == BoundKind.BinaryOperator)
			{
				instance.Push(boundBinaryOperator);
				boundBinaryOperator = (BoundBinaryOperator)left;
				left = boundBinaryOperator.Left;
			}
			if (_regionPlace == RegionPlace.Before && instance.Count > 0)
			{
				int num = instance.Count - 1;
				int num2 = 1;
				while (true)
				{
					if (num2 <= num)
					{
						if (instance[num2] == _firstInRegion)
						{
							EnterRegion();
							break;
						}
						num2++;
						continue;
					}
					if (boundBinaryOperator == _firstInRegion)
					{
						EnterRegion();
					}
					break;
				}
			}
			BinaryOperatorKind binaryOperatorKind = boundBinaryOperator.OperatorKind & BinaryOperatorKind.OpMask;
			if (binaryOperatorKind == BinaryOperatorKind.OrElse || binaryOperatorKind == BinaryOperatorKind.AndAlso)
			{
				VisitCondition(left);
			}
			else
			{
				VisitRvalue(left);
			}
			while (true)
			{
				switch (boundBinaryOperator.OperatorKind & BinaryOperatorKind.OpMask)
				{
				case BinaryOperatorKind.AndAlso:
				{
					LocalState stateWhenTrue = StateWhenTrue;
					LocalState stateWhenFalse3 = StateWhenFalse;
					SetState(stateWhenTrue);
					VisitCondition(boundBinaryOperator.Right);
					LocalState stateWhenTrue2 = StateWhenTrue;
					LocalState self2 = stateWhenFalse3;
					IntersectWith(ref self2, ref StateWhenFalse);
					SetConditionalState(stateWhenTrue2, self2);
					break;
				}
				case BinaryOperatorKind.OrElse:
				{
					LocalState other = StateWhenTrue;
					LocalState stateWhenFalse = StateWhenFalse;
					SetState(stateWhenFalse);
					VisitCondition(boundBinaryOperator.Right);
					LocalState self = StateWhenTrue;
					IntersectWith(ref self, ref other);
					LocalState stateWhenFalse2 = StateWhenFalse;
					SetConditionalState(self, stateWhenFalse2);
					break;
				}
				default:
					VisitRvalue(boundBinaryOperator.Right);
					break;
				}
				if (instance.Count <= 0)
				{
					break;
				}
				left = boundBinaryOperator;
				boundBinaryOperator = instance.Pop();
				BinaryOperatorKind binaryOperatorKind2 = boundBinaryOperator.OperatorKind & BinaryOperatorKind.OpMask;
				if (binaryOperatorKind2 == BinaryOperatorKind.OrElse || binaryOperatorKind2 == BinaryOperatorKind.AndAlso)
				{
					AdjustConditionalState(left);
				}
				else
				{
					Unsplit();
				}
				if (left == _lastInRegion && IsInside)
				{
					LeaveRegion();
				}
			}
			instance.Free();
			return null;
		}

		public override BoundNode VisitUserDefinedBinaryOperator(BoundUserDefinedBinaryOperator node)
		{
			VisitRvalue(node.UnderlyingExpression);
			return null;
		}

		public override BoundNode VisitUserDefinedShortCircuitingOperator(BoundUserDefinedShortCircuitingOperator node)
		{
			if (node.LeftOperand != null)
			{
				VisitRvalue(node.LeftOperand);
			}
			VisitRvalue(node.BitwiseOperator);
			return null;
		}

		public override BoundNode VisitAddHandlerStatement(BoundAddHandlerStatement node)
		{
			return VisitAddRemoveHandlerStatement(node);
		}

		public override BoundNode VisitRemoveHandlerStatement(BoundRemoveHandlerStatement node)
		{
			return VisitAddRemoveHandlerStatement(node);
		}

		public override BoundNode VisitAddressOfOperator(BoundAddressOfOperator node)
		{
			Visit(node.MethodGroup);
			return null;
		}

		public override BoundNode VisitLateAddressOfOperator(BoundLateAddressOfOperator node)
		{
			Visit(node.MemberAccess);
			return null;
		}

		private BoundNode VisitAddRemoveHandlerStatement(BoundAddRemoveHandlerStatement node)
		{
			VisitRvalue(node.EventAccess);
			VisitRvalue(node.Handler);
			return null;
		}

		public override BoundNode VisitEventAccess(BoundEventAccess node)
		{
			if (node.ReceiverOpt != null)
			{
				VisitRvalue(node.ReceiverOpt);
			}
			return null;
		}

		public override BoundNode VisitRaiseEventStatement(BoundRaiseEventStatement node)
		{
			VisitExpressionAsStatement(node.EventInvocation);
			return null;
		}

		public override BoundNode VisitParenthesized(BoundParenthesized node)
		{
			VisitRvalue(node.Expression);
			return null;
		}

		public override BoundNode VisitUnaryOperator(BoundUnaryOperator node)
		{
			if (node.OperatorKind == UnaryOperatorKind.Not)
			{
				VisitCondition(node.Operand);
				SetConditionalState(StateWhenFalse, StateWhenTrue);
			}
			else
			{
				VisitRvalue(node.Operand);
			}
			return null;
		}

		public override BoundNode VisitUserDefinedUnaryOperator(BoundUserDefinedUnaryOperator node)
		{
			VisitRvalue(node.UnderlyingExpression);
			return null;
		}

		public override BoundNode VisitNullableIsTrueOperator(BoundNullableIsTrueOperator node)
		{
			VisitRvalue(node.Operand);
			return null;
		}

		public override BoundNode VisitArrayCreation(BoundArrayCreation node)
		{
			ImmutableArray<BoundExpression>.Enumerator enumerator = node.Bounds.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundExpression current = enumerator.Current;
				VisitRvalue(current);
			}
			if (node.InitializerOpt != null && !node.InitializerOpt.Initializers.IsDefault)
			{
				ImmutableArray<BoundExpression>.Enumerator enumerator2 = node.InitializerOpt.Initializers.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					BoundExpression current2 = enumerator2.Current;
					VisitRvalue(current2);
				}
			}
			return null;
		}

		public override BoundNode VisitArrayInitialization(BoundArrayInitialization node)
		{
			ImmutableArray<BoundExpression>.Enumerator enumerator = node.Initializers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundExpression current = enumerator.Current;
				VisitRvalue(current);
			}
			return null;
		}

		public override BoundNode VisitArrayLiteral(BoundArrayLiteral node)
		{
			ImmutableArray<BoundExpression>.Enumerator enumerator = node.Bounds.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundExpression current = enumerator.Current;
				VisitRvalue(current);
			}
			VisitRvalue(node.Initializer);
			return null;
		}

		public override BoundNode VisitTupleLiteral(BoundTupleLiteral node)
		{
			return VisitTupleExpression(node);
		}

		public override BoundNode VisitConvertedTupleLiteral(BoundConvertedTupleLiteral node)
		{
			return VisitTupleExpression(node);
		}

		private BoundNode VisitTupleExpression(BoundTupleExpression node)
		{
			ImmutableArray<BoundExpression>.Enumerator enumerator = node.Arguments.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundExpression current = enumerator.Current;
				VisitRvalue(current);
			}
			return null;
		}

		public override BoundNode VisitDirectCast(BoundDirectCast node)
		{
			VisitRvalue(node.Operand);
			return null;
		}

		public override BoundNode VisitTryCast(BoundTryCast node)
		{
			VisitRvalue(node.Operand);
			return null;
		}

		public override BoundNode VisitTypeOf(BoundTypeOf node)
		{
			VisitRvalue(node.Operand);
			return null;
		}

		public override BoundNode VisitMethodGroup(BoundMethodGroup node)
		{
			VisitRvalue(node.ReceiverOpt);
			return null;
		}

		public override BoundNode VisitPropertyGroup(BoundPropertyGroup node)
		{
			VisitRvalue(node.ReceiverOpt);
			return null;
		}

		public override BoundNode VisitGetType(BoundGetType node)
		{
			VisitTypeExpression(node.SourceType);
			return null;
		}

		public override BoundNode VisitSequencePoint(BoundSequencePoint node)
		{
			VisitStatement(node.StatementOpt);
			return null;
		}

		public override BoundNode VisitSequencePointWithSpan(BoundSequencePointWithSpan node)
		{
			VisitStatement(node.StatementOpt);
			return null;
		}

		public override BoundNode VisitStatementList(BoundStatementList node)
		{
			ImmutableArray<BoundStatement>.Enumerator enumerator = node.Statements.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundStatement current = enumerator.Current;
				Visit(current);
			}
			return null;
		}

		public override BoundNode VisitSyncLockStatement(BoundSyncLockStatement node)
		{
			VisitRvalue(node.LockExpression);
			VisitStatement(node.Body);
			return null;
		}

		public override BoundNode VisitUnboundLambda(UnboundLambda node)
		{
			BoundLambda boundLambda = node.BindForErrorRecovery();
			if (boundLambda == _firstInRegion && _regionPlace == RegionPlace.Before)
			{
				EnterRegion();
			}
			VisitLambda(node.BindForErrorRecovery());
			if (boundLambda == _lastInRegion && IsInside)
			{
				LeaveRegion();
			}
			return null;
		}

		public override BoundNode VisitExitStatement(BoundExitStatement node)
		{
			_pendingBranches.Add(new PendingBranch(node, State, _nesting));
			SetUnreachable();
			return null;
		}

		public override BoundNode VisitContinueStatement(BoundContinueStatement node)
		{
			_pendingBranches.Add(new PendingBranch(node, State, _nesting));
			SetUnreachable();
			return null;
		}

		public override BoundNode VisitMyBaseReference(BoundMyBaseReference node)
		{
			return null;
		}

		public override BoundNode VisitDoLoopStatement(BoundDoLoopStatement node)
		{
			if (node.ConditionOpt != null)
			{
				if (node.ConditionIsTop)
				{
					VisitDoLoopTopConditionStatement(node);
				}
				else
				{
					VisitDoLoopBottomConditionStatement(node);
				}
			}
			else
			{
				VisitUnconditionalDoLoopStatement(node);
			}
			return null;
		}

		public void VisitDoLoopTopConditionStatement(BoundDoLoopStatement node)
		{
			LoopHead(node);
			VisitCondition(node.ConditionOpt);
			LocalState breakState;
			if (node.ConditionIsUntil)
			{
				breakState = StateWhenTrue;
				SetState(StateWhenFalse);
			}
			else
			{
				breakState = StateWhenFalse;
				SetState(StateWhenTrue);
			}
			VisitStatement(node.Body);
			ResolveContinues(node.ContinueLabel);
			LoopTail(node);
			ResolveBreaks(breakState, node.ExitLabel);
		}

		public void VisitDoLoopBottomConditionStatement(BoundDoLoopStatement node)
		{
			LoopHead(node);
			VisitStatement(node.Body);
			ResolveContinues(node.ContinueLabel);
			VisitCondition(node.ConditionOpt);
			LocalState breakState;
			if (node.ConditionIsUntil)
			{
				breakState = StateWhenTrue;
				SetState(StateWhenFalse);
			}
			else
			{
				breakState = StateWhenFalse;
				SetState(StateWhenTrue);
			}
			LoopTail(node);
			ResolveBreaks(breakState, node.ExitLabel);
		}

		private void VisitUnconditionalDoLoopStatement(BoundDoLoopStatement node)
		{
			LoopHead(node);
			VisitStatement(node.Body);
			ResolveContinues(node.ContinueLabel);
			LocalState breakState = UnreachableState();
			LoopTail(node);
			ResolveBreaks(breakState, node.ExitLabel);
		}

		public override BoundNode VisitGotoStatement(BoundGotoStatement node)
		{
			_pendingBranches.Add(new PendingBranch(node, State, _nesting));
			SetUnreachable();
			return null;
		}

		public override BoundNode VisitLabelStatement(BoundLabelStatement node)
		{
			if (ResolveBranches(node))
			{
				backwardBranchChanged = true;
			}
			LabelSymbol label = node.Label;
			LocalState other = LabelState(label);
			IntersectWith(ref State, ref other);
			_labels[label] = new LabelStateAndNesting(node, State.Clone(), _nesting);
			_labelsSeen.Add(label);
			return null;
		}

		public override BoundNode VisitThrowStatement(BoundThrowStatement node)
		{
			BoundExpression expressionOpt = node.ExpressionOpt;
			if (expressionOpt != null)
			{
				VisitRvalue(expressionOpt);
			}
			SetUnreachable();
			return null;
		}

		public override BoundNode VisitNoOpStatement(BoundNoOpStatement node)
		{
			return null;
		}

		public override BoundNode VisitNamespaceExpression(BoundNamespaceExpression node)
		{
			return null;
		}

		public override BoundNode VisitFieldInitializer(BoundFieldInitializer node)
		{
			VisitRvalue(node.InitialValue);
			return null;
		}

		public override BoundNode VisitPropertyInitializer(BoundPropertyInitializer node)
		{
			VisitRvalue(node.InitialValue);
			return null;
		}

		public override BoundNode VisitArrayLength(BoundArrayLength node)
		{
			VisitRvalue(node.Expression);
			return null;
		}

		public override BoundNode VisitConditionalGoto(BoundConditionalGoto node)
		{
			VisitCondition(node.Condition);
			if (node.JumpIfTrue)
			{
				_pendingBranches.Add(new PendingBranch(node, StateWhenTrue, _nesting));
				SetState(StateWhenFalse);
			}
			else
			{
				_pendingBranches.Add(new PendingBranch(node, StateWhenFalse, _nesting));
				SetState(StateWhenTrue);
			}
			return null;
		}

		public override BoundNode VisitXmlDocument(BoundXmlDocument node)
		{
			VisitRvalue(node.Declaration);
			ImmutableArray<BoundExpression>.Enumerator enumerator = node.ChildNodes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundExpression current = enumerator.Current;
				VisitRvalue(current);
			}
			return null;
		}

		public override BoundNode VisitXmlElement(BoundXmlElement node)
		{
			VisitRvalue(node.Argument);
			ImmutableArray<BoundExpression>.Enumerator enumerator = node.ChildNodes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundExpression current = enumerator.Current;
				VisitRvalue(current);
			}
			return null;
		}

		public override BoundNode VisitXmlAttribute(BoundXmlAttribute node)
		{
			VisitRvalue(node.Name);
			VisitRvalue(node.Value);
			return null;
		}

		public override BoundNode VisitXmlName(BoundXmlName node)
		{
			return null;
		}

		public override BoundNode VisitXmlNamespace(BoundXmlNamespace node)
		{
			VisitRvalue(node.ObjectCreation);
			return null;
		}

		public override BoundNode VisitXmlEmbeddedExpression(BoundXmlEmbeddedExpression node)
		{
			VisitRvalue(node.Expression);
			return null;
		}

		public override BoundNode VisitXmlMemberAccess(BoundXmlMemberAccess node)
		{
			VisitRvalue(node.MemberAccess);
			return null;
		}

		public override BoundNode VisitUnstructuredExceptionHandlingStatement(BoundUnstructuredExceptionHandlingStatement node)
		{
			Visit(node.Body);
			return null;
		}

		public override BoundNode VisitAwaitOperator(BoundAwaitOperator node)
		{
			VisitRvalue(node.Operand);
			return null;
		}

		public override BoundNode VisitNameOfOperator(BoundNameOfOperator node)
		{
			LocalState state = State.Clone();
			SetUnreachable();
			VisitRvalue(node.Argument);
			SetState(state);
			return null;
		}

		public override BoundNode VisitTypeAsValueExpression(BoundTypeAsValueExpression node)
		{
			Visit(node.Expression);
			return null;
		}

		public override BoundNode VisitInterpolatedStringExpression(BoundInterpolatedStringExpression node)
		{
			ImmutableArray<BoundNode>.Enumerator enumerator = node.Contents.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundNode current = enumerator.Current;
				Visit(current);
			}
			return null;
		}

		public override BoundNode VisitInterpolation(BoundInterpolation node)
		{
			VisitRvalue(node.Expression);
			VisitRvalue(node.AlignmentOpt);
			VisitRvalue(node.FormatStringOpt);
			return null;
		}

		public override BoundNode VisitParameterEqualsValue(BoundParameterEqualsValue node)
		{
			VisitRvalue(node.Value);
			return null;
		}

		public override BoundNode VisitMethodDefIndex(BoundMethodDefIndex node)
		{
			return null;
		}

		public override BoundNode VisitMaximumMethodDefIndex(BoundMaximumMethodDefIndex node)
		{
			return null;
		}

		public override BoundNode VisitModuleVersionId(BoundModuleVersionId node)
		{
			return null;
		}

		public override BoundNode VisitModuleVersionIdString(BoundModuleVersionIdString node)
		{
			return null;
		}

		public override BoundNode VisitInstrumentationPayloadRoot(BoundInstrumentationPayloadRoot node)
		{
			return null;
		}

		public override BoundNode VisitSourceDocumentIndex(BoundSourceDocumentIndex node)
		{
			return null;
		}
	}
}
