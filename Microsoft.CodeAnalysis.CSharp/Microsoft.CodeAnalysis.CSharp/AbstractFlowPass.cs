using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public abstract class AbstractFlowPass<TLocalState, TLocalFunctionState> : BoundTreeVisitor where TLocalState : AbstractFlowPass<TLocalState, TLocalFunctionState>.ILocalState where TLocalFunctionState : AbstractFlowPass<TLocalState, TLocalFunctionState>.AbstractLocalFunctionState
    {
        public class PendingBranch
        {
            public readonly BoundNode Branch;

            public bool IsConditionalState;

            public TLocalState State;

            public TLocalState StateWhenTrue;

            public TLocalState StateWhenFalse;

            public readonly LabelSymbol Label;

            public PendingBranch(BoundNode branch, TLocalState state, LabelSymbol label, bool isConditionalState = false, TLocalState stateWhenTrue = default(TLocalState), TLocalState stateWhenFalse = default(TLocalState))
            {
                Branch = branch;
                State = state.Clone();
                IsConditionalState = isConditionalState;
                if (isConditionalState)
                {
                    StateWhenTrue = stateWhenTrue.Clone();
                    StateWhenFalse = stateWhenFalse.Clone();
                }
                Label = label;
            }
        }

        protected struct SavedPending
        {
            public readonly ArrayBuilder<PendingBranch> PendingBranches;

            public readonly PooledHashSet<BoundStatement> LabelsSeen;

            public SavedPending(ArrayBuilder<PendingBranch> pendingBranches, PooledHashSet<BoundStatement> labelsSeen)
            {
                PendingBranches = pendingBranches;
                LabelsSeen = labelsSeen;
            }
        }

        public interface ILocalState
        {
            bool Reachable { get; }

            TLocalState Clone();
        }

        public abstract class AbstractLocalFunctionState
        {
            public TLocalState StateFromBottom;

            public TLocalState StateFromTop;

            public bool Visited;

            public AbstractLocalFunctionState(TLocalState stateFromBottom, TLocalState stateFromTop)
            {
                StateFromBottom = stateFromBottom;
                StateFromTop = stateFromTop;
            }
        }

        protected int _recursionDepth;

        protected readonly CSharpCompilation compilation;

        protected readonly Symbol _symbol;

        protected Symbol CurrentSymbol;

        protected readonly BoundNode methodMainNode;

        private readonly PooledDictionary<LabelSymbol, TLocalState> _labels;

        protected bool stateChangedAfterUse;

        private PooledHashSet<BoundStatement> _labelsSeen;

        protected TLocalState State;

        protected TLocalState StateWhenTrue;

        protected TLocalState StateWhenFalse;

        protected bool IsConditionalState;

        private readonly bool _nonMonotonicTransfer;

        protected RegionPlace regionPlace;

        protected readonly BoundNode firstInRegion;

        protected readonly BoundNode lastInRegion;

        protected readonly bool TrackingRegions;

        private readonly Dictionary<BoundLoopStatement, TLocalState> _loopHeadState;

        protected readonly TextSpan RegionSpan;

        protected Optional<TLocalState> NonMonotonicState;

        private SmallDictionary<LocalFunctionSymbol, TLocalFunctionState>? _localFuncVarUsages;

        protected ArrayBuilder<PendingBranch> PendingBranches { get; private set; }

        protected DiagnosticBag Diagnostics { get; }

        protected bool IsInside => regionPlace == RegionPlace.Inside;

        protected ImmutableArray<ParameterSymbol> MethodParameters
        {
            get
            {
                if (_symbol is MethodSymbol methodSymbol)
                {
                    return methodSymbol.Parameters;
                }
                return ImmutableArray<ParameterSymbol>.Empty;
            }
        }

        protected ParameterSymbol MethodThisParameter
        {
            get
            {
                ParameterSymbol thisParameter = null;
                (_symbol as MethodSymbol)?.TryGetThisParameter(out thisParameter);
                return thisParameter;
            }
        }

        public abstract bool AwaitUsingAndForeachAddsPendingBranch { get; }

        protected void SetConditionalState(TLocalState whenTrue, TLocalState whenFalse)
        {
            IsConditionalState = true;
            State = default(TLocalState);
            StateWhenTrue = whenTrue;
            StateWhenFalse = whenFalse;
        }

        protected void SetState(TLocalState newState)
        {
            StateWhenTrue = (StateWhenFalse = default(TLocalState));
            IsConditionalState = false;
            State = newState;
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
                Join(ref StateWhenTrue, ref StateWhenFalse);
                SetState(StateWhenTrue);
            }
        }

        protected AbstractFlowPass(CSharpCompilation compilation, Symbol symbol, BoundNode node, BoundNode firstInRegion = null, BoundNode lastInRegion = null, bool trackRegions = false, bool nonMonotonicTransferFunction = false)
        {
            if (firstInRegion != null && lastInRegion != null)
            {
                trackRegions = true;
            }
            if (trackRegions)
            {
                int spanStart = firstInRegion.Syntax.SpanStart;
                int length = lastInRegion.Syntax.Span.End - spanStart;
                RegionSpan = new TextSpan(spanStart, length);
            }
            PendingBranches = ArrayBuilder<PendingBranch>.GetInstance();
            _labelsSeen = PooledHashSet<BoundStatement>.GetInstance();
            _labels = PooledDictionary<LabelSymbol, TLocalState>.GetInstance();
            Diagnostics = DiagnosticBag.GetInstance();
            this.compilation = compilation;
            _symbol = symbol;
            CurrentSymbol = symbol;
            methodMainNode = node;
            this.firstInRegion = firstInRegion;
            this.lastInRegion = lastInRegion;
            _loopHeadState = new Dictionary<BoundLoopStatement, TLocalState>(ReferenceEqualityComparer.Instance);
            TrackingRegions = trackRegions;
            _nonMonotonicTransfer = nonMonotonicTransferFunction;
        }

        protected abstract string Dump(TLocalState state);

        protected string Dump()
        {
            if (!IsConditionalState)
            {
                return Dump(State);
            }
            return "true: " + Dump(StateWhenTrue) + " false: " + Dump(StateWhenFalse);
        }

        protected virtual void EnterRegion()
        {
            regionPlace = RegionPlace.Inside;
        }

        protected virtual void LeaveRegion()
        {
            regionPlace = RegionPlace.After;
        }

        protected bool RegionContains(TextSpan span)
        {
            if (span.Length == 0)
            {
                return RegionSpan.Contains(span.Start);
            }
            return RegionSpan.Contains(span);
        }

        protected virtual void EnterParameters(ImmutableArray<ParameterSymbol> parameters)
        {
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                EnterParameter(current);
            }
        }

        protected virtual void EnterParameter(ParameterSymbol parameter)
        {
        }

        protected virtual void LeaveParameters(ImmutableArray<ParameterSymbol> parameters, SyntaxNode syntax, Location location)
        {
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                LeaveParameter(current, syntax, location);
            }
        }

        protected virtual void LeaveParameter(ParameterSymbol parameter, SyntaxNode syntax, Location location)
        {
        }

        public override BoundNode Visit(BoundNode node)
        {
            return VisitAlways(node);
        }

        protected BoundNode VisitAlways(BoundNode node)
        {
            BoundNode result = null;
            if (node != null)
            {
                if (TrackingRegions)
                {
                    if (node == firstInRegion && regionPlace == RegionPlace.Before)
                    {
                        EnterRegion();
                    }
                    result = VisitWithStackGuard(node);
                    if (node == lastInRegion && regionPlace == RegionPlace.Inside)
                    {
                        LeaveRegion();
                    }
                }
                else
                {
                    result = VisitWithStackGuard(node);
                }
            }
            return result;
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

        protected virtual ImmutableArray<PendingBranch> Scan(ref bool badRegion)
        {
            SavedPending oldPending = SavePending();
            Visit(methodMainNode);
            Unsplit();
            RestorePending(oldPending);
            if (TrackingRegions && regionPlace != RegionPlace.After)
            {
                badRegion = true;
            }
            return RemoveReturns();
        }

        protected ImmutableArray<PendingBranch> Analyze(ref bool badRegion, Optional<TLocalState> initialState = default(Optional<TLocalState>))
        {
            ImmutableArray<PendingBranch> result;
            do
            {
                regionPlace = RegionPlace.Before;
                State = (initialState.HasValue ? initialState.Value : TopState());
                PendingBranches.Clear();
                stateChangedAfterUse = false;
                Diagnostics.Clear();
                result = Scan(ref badRegion);
            }
            while (stateChangedAfterUse);
            return result;
        }

        protected virtual void Free()
        {
            Diagnostics.Free();
            PendingBranches.Free();
            _labelsSeen.Free();
            _labels.Free();
        }

        protected bool ShouldAnalyzeOutParameters(out Location location)
        {
            if (!(_symbol is MethodSymbol methodSymbol) || methodSymbol.Locations.Length != 1)
            {
                location = null;
                return false;
            }
            location = methodSymbol.Locations[0];
            return true;
        }

        protected virtual TLocalState LabelState(LabelSymbol label)
        {
            if (_labels.TryGetValue(label, out var value))
            {
                return value;
            }
            value = UnreachableState();
            _labels.Add(label, value);
            return value;
        }

        protected virtual ImmutableArray<PendingBranch> RemoveReturns()
        {
            ImmutableArray<PendingBranch> result = PendingBranches.ToImmutable();
            PendingBranches.Clear();
            return result;
        }

        protected void SetUnreachable()
        {
            State = UnreachableState();
        }

        protected void VisitLvalue(BoundExpression node)
        {
            if (TrackingRegions && node == firstInRegion && regionPlace == RegionPlace.Before)
            {
                EnterRegion();
            }
            switch (node?.Kind)
            {
                case BoundKind.Parameter:
                    VisitLvalueParameter((BoundParameter)node);
                    break;
                case BoundKind.Local:
                    VisitLvalue((BoundLocal)node);
                    break;
                case BoundKind.PropertyAccess:
                    {
                        BoundPropertyAccess boundPropertyAccess = (BoundPropertyAccess)node;
                        if (Binder.AccessingAutoPropertyFromConstructor(boundPropertyAccess, _symbol))
                        {
                            SynthesizedBackingFieldSymbol synthesizedBackingFieldSymbol = (boundPropertyAccess.PropertySymbol as SourcePropertySymbolBase)?.BackingField;
                            if (synthesizedBackingFieldSymbol != null)
                            {
                                VisitFieldAccessInternal(boundPropertyAccess.ReceiverOpt, synthesizedBackingFieldSymbol);
                                break;
                            }
                        }
                        goto default;
                    }
                case BoundKind.FieldAccess:
                    {
                        BoundFieldAccess boundFieldAccess = (BoundFieldAccess)node;
                        VisitFieldAccessInternal(boundFieldAccess.ReceiverOpt, boundFieldAccess.FieldSymbol);
                        break;
                    }
                case BoundKind.EventAccess:
                    {
                        BoundEventAccess boundEventAccess = (BoundEventAccess)node;
                        VisitFieldAccessInternal(boundEventAccess.ReceiverOpt, boundEventAccess.EventSymbol.AssociatedField);
                        break;
                    }
                case BoundKind.TupleLiteral:
                case BoundKind.ConvertedTupleLiteral:
                    ((BoundTupleExpression)node).VisitAllElements(delegate (BoundExpression x, AbstractFlowPass<TLocalState, TLocalFunctionState> self)
                    {
                        self.VisitLvalue(x);
                    }, this);
                    break;
                default:
                    VisitRvalue(node);
                    break;
                case BoundKind.ThisReference:
                case BoundKind.BaseReference:
                    break;
            }
            if (TrackingRegions && node == lastInRegion && regionPlace == RegionPlace.Inside)
            {
                LeaveRegion();
            }
        }

        protected virtual void VisitLvalue(BoundLocal node)
        {
        }

        protected void VisitCondition(BoundExpression node)
        {
            Visit(node);
            AdjustConditionalState(node);
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
            else if ((object)node.Type == null || node.Type!.SpecialType != SpecialType.System_Boolean)
            {
                Unsplit();
            }
            Split();
        }

        protected virtual void VisitRvalue(BoundExpression node, bool isKnownToBeAnLvalue = false)
        {
            Visit(node);
            Unsplit();
        }

        [DebuggerHidden]
        protected virtual void VisitStatement(BoundStatement statement)
        {
            Visit(statement);
        }

        protected static bool IsConstantTrue(BoundExpression node)
        {
            return node.ConstantValue == ConstantValue.True;
        }

        protected static bool IsConstantFalse(BoundExpression node)
        {
            return node.ConstantValue == ConstantValue.False;
        }

        protected static bool IsConstantNull(BoundExpression node)
        {
            return node.ConstantValue == ConstantValue.Null;
        }

        private void LoopHead(BoundLoopStatement node)
        {
            if (_loopHeadState.TryGetValue(node, out var value))
            {
                Join(ref State, ref value);
            }
            _loopHeadState[node] = State.Clone();
        }

        private void LoopTail(BoundLoopStatement node)
        {
            TLocalState self = _loopHeadState[node];
            if (Join(ref self, ref State))
            {
                _loopHeadState[node] = self;
                stateChangedAfterUse = true;
            }
        }

        private void ResolveBreaks(TLocalState breakState, LabelSymbol label)
        {
            ArrayBuilder<PendingBranch> pendingBranches = PendingBranches;
            int count = pendingBranches.Count;
            if (count != 0)
            {
                int num = 0;
                for (int i = 0; i < count; i++)
                {
                    PendingBranch pendingBranch = pendingBranches[i];
                    if (pendingBranch.Label == label)
                    {
                        Join(ref breakState, ref pendingBranch.State);
                        continue;
                    }
                    if (num != i)
                    {
                        pendingBranches[num] = pendingBranch;
                    }
                    num++;
                }
                pendingBranches.Clip(num);
            }
            SetState(breakState);
        }

        private void ResolveContinues(LabelSymbol continueLabel)
        {
            ArrayBuilder<PendingBranch> pendingBranches = PendingBranches;
            int count = pendingBranches.Count;
            if (count == 0)
            {
                return;
            }
            int num = 0;
            for (int i = 0; i < count; i++)
            {
                PendingBranch pendingBranch = pendingBranches[i];
                if (pendingBranch.Label == continueLabel)
                {
                    Join(ref State, ref pendingBranch.State);
                    continue;
                }
                if (num != i)
                {
                    pendingBranches[num] = pendingBranch;
                }
                num++;
            }
            pendingBranches.Clip(num);
        }

        protected virtual void NoteBranch(PendingBranch pending, BoundNode gotoStmt, BoundStatement target)
        {
        }

        private bool ResolveBranches(LabelSymbol label, BoundStatement target)
        {
            bool labelStateChanged = false;
            ArrayBuilder<PendingBranch> pendingBranches = PendingBranches;
            int count = pendingBranches.Count;
            if (count != 0)
            {
                int num = 0;
                for (int i = 0; i < count; i++)
                {
                    PendingBranch pendingBranch = pendingBranches[i];
                    if (pendingBranch.Label == label)
                    {
                        ResolveBranch(pendingBranch, label, target, ref labelStateChanged);
                        continue;
                    }
                    if (num != i)
                    {
                        pendingBranches[num] = pendingBranch;
                    }
                    num++;
                }
                pendingBranches.Clip(num);
            }
            return labelStateChanged;
        }

        protected virtual void ResolveBranch(PendingBranch pending, LabelSymbol label, BoundStatement target, ref bool labelStateChanged)
        {
            TLocalState self = LabelState(label);
            if (target != null)
            {
                NoteBranch(pending, pending.Branch, target);
            }
            if (Join(ref self, ref pending.State))
            {
                labelStateChanged = true;
                _labels[label] = self;
            }
        }

        protected SavedPending SavePending()
        {
            SavedPending result = new SavedPending(PendingBranches, _labelsSeen);
            PendingBranches = ArrayBuilder<PendingBranch>.GetInstance();
            _labelsSeen = PooledHashSet<BoundStatement>.GetInstance();
            return result;
        }

        protected void RestorePending(SavedPending oldPending)
        {
            foreach (BoundStatement item in _labelsSeen)
            {
                switch (item.Kind)
                {
                    case BoundKind.LabeledStatement:
                        {
                            BoundLabeledStatement boundLabeledStatement = (BoundLabeledStatement)item;
                            stateChangedAfterUse |= ResolveBranches(boundLabeledStatement.Label, boundLabeledStatement);
                            break;
                        }
                    case BoundKind.LabelStatement:
                        {
                            BoundLabelStatement boundLabelStatement = (BoundLabelStatement)item;
                            stateChangedAfterUse |= ResolveBranches(boundLabelStatement.Label, boundLabelStatement);
                            break;
                        }
                    case BoundKind.SwitchSection:
                        {
                            BoundSwitchSection boundSwitchSection = (BoundSwitchSection)item;
                            ImmutableArray<BoundSwitchLabel>.Enumerator enumerator2 = boundSwitchSection.SwitchLabels.GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                BoundSwitchLabel current2 = enumerator2.Current;
                                stateChangedAfterUse |= ResolveBranches(current2.Label, boundSwitchSection);
                            }
                            break;
                        }
                    default:
                        throw ExceptionUtilities.UnexpectedValue(item.Kind);
                }
            }
            oldPending.PendingBranches.AddRange(PendingBranches);
            PendingBranches.Free();
            PendingBranches = oldPending.PendingBranches;
            _labelsSeen.Free();
            _labelsSeen = oldPending.LabelsSeen;
        }

        public override BoundNode DefaultVisit(BoundNode node)
        {
            Diagnostics.Add(ErrorCode.ERR_InternalError, node.Syntax.Location);
            return null;
        }

        public override BoundNode VisitAttribute(BoundAttribute node)
        {
            return null;
        }

        public override BoundNode VisitThrowExpression(BoundThrowExpression node)
        {
            VisitRvalue(node.Expression);
            SetUnreachable();
            return node;
        }

        public override BoundNode VisitPassByCopy(BoundPassByCopy node)
        {
            VisitRvalue(node.Expression);
            return node;
        }

        public override BoundNode VisitIsPatternExpression(BoundIsPatternExpression node)
        {
            VisitRvalue(node.Expression);
            bool num = node.Pattern.IsNegated(out BoundPattern innerPattern);
            VisitPattern(innerPattern);
            ImmutableHashSet<LabelSymbol> reachableLabels = node.DecisionDag.ReachableLabels;
            if (!reachableLabels.Contains(node.WhenTrueLabel))
            {
                SetState(StateWhenFalse);
                SetConditionalState(UnreachableState(), State);
            }
            else if (!reachableLabels.Contains(node.WhenFalseLabel))
            {
                SetState(StateWhenTrue);
                SetConditionalState(State, UnreachableState());
            }
            if (num)
            {
                SetConditionalState(StateWhenFalse, StateWhenTrue);
            }
            return node;
        }

        public virtual void VisitPattern(BoundPattern pattern)
        {
            Split();
        }

        public override BoundNode VisitConstantPattern(BoundConstantPattern node)
        {
            throw ExceptionUtilities.Unreachable;
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
            VisitArguments(node.Arguments, default(ImmutableArray<RefKind>), null);
            return null;
        }

        public override BoundNode VisitTupleBinaryOperator(BoundTupleBinaryOperator node)
        {
            VisitRvalue(node.Left);
            VisitRvalue(node.Right);
            return null;
        }

        public override BoundNode VisitDynamicObjectCreationExpression(BoundDynamicObjectCreationExpression node)
        {
            VisitArguments(node.Arguments, node.ArgumentRefKindsOpt, null);
            VisitRvalue(node.InitializerExpressionOpt);
            return null;
        }

        public override BoundNode VisitDynamicIndexerAccess(BoundDynamicIndexerAccess node)
        {
            VisitRvalue(node.Receiver);
            VisitArguments(node.Arguments, node.ArgumentRefKindsOpt, null);
            return null;
        }

        public override BoundNode VisitDynamicMemberAccess(BoundDynamicMemberAccess node)
        {
            VisitRvalue(node.Receiver);
            return null;
        }

        public override BoundNode VisitDynamicInvocation(BoundDynamicInvocation node)
        {
            VisitRvalue(node.Expression);
            VisitArguments(node.Arguments, node.ArgumentRefKindsOpt, null);
            return null;
        }

        protected BoundNode VisitInterpolatedStringBase(BoundInterpolatedStringBase node)
        {
            ImmutableArray<BoundExpression>.Enumerator enumerator = node.Parts.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundExpression current = enumerator.Current;
                VisitRvalue(current);
            }
            return null;
        }

        public override BoundNode VisitInterpolatedString(BoundInterpolatedString node)
        {
            return VisitInterpolatedStringBase(node);
        }

        public override BoundNode VisitUnconvertedInterpolatedString(BoundUnconvertedInterpolatedString node)
        {
            return VisitInterpolatedStringBase(node);
        }

        public override BoundNode VisitStringInsert(BoundStringInsert node)
        {
            VisitRvalue(node.Value);
            if (node.Alignment != null)
            {
                VisitRvalue(node.Alignment);
            }
            if (node.Format != null)
            {
                VisitRvalue(node.Format);
            }
            return null;
        }

        public override BoundNode VisitArgList(BoundArgList node)
        {
            return null;
        }

        public override BoundNode VisitArgListOperator(BoundArgListOperator node)
        {
            VisitArguments(node.Arguments, node.ArgumentRefKindsOpt, null);
            return null;
        }

        public override BoundNode VisitRefTypeOperator(BoundRefTypeOperator node)
        {
            VisitRvalue(node.Operand);
            return null;
        }

        public override BoundNode VisitMakeRefOperator(BoundMakeRefOperator node)
        {
            VisitRvalue(node.Operand, isKnownToBeAnLvalue: true);
            return null;
        }

        public override BoundNode VisitRefValueOperator(BoundRefValueOperator node)
        {
            VisitRvalue(node.Operand);
            return null;
        }

        public override BoundNode VisitGlobalStatementInitializer(BoundGlobalStatementInitializer node)
        {
            VisitStatement(node.Statement);
            return null;
        }

        public override BoundNode VisitLambda(BoundLambda node)
        {
            return null;
        }

        public override BoundNode VisitLocal(BoundLocal node)
        {
            return null;
        }

        public override BoundNode VisitLocalDeclaration(BoundLocalDeclaration node)
        {
            if (node.InitializerOpt != null)
            {
                VisitRvalue(node.InitializerOpt, node.LocalSymbol.RefKind != RefKind.None);
                if (node.LocalSymbol.RefKind != 0)
                {
                    WriteArgument(node.InitializerOpt, node.LocalSymbol.RefKind, null);
                }
            }
            return null;
        }

        public override BoundNode VisitBlock(BoundBlock node)
        {
            VisitStatements(node.Statements);
            return null;
        }

        private void VisitStatements(ImmutableArray<BoundStatement> statements)
        {
            ImmutableArray<BoundStatement>.Enumerator enumerator = statements.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundStatement current = enumerator.Current;
                VisitStatement(current);
            }
        }

        public override BoundNode VisitScope(BoundScope node)
        {
            VisitStatements(node.Statements);
            return null;
        }

        public override BoundNode VisitExpressionStatement(BoundExpressionStatement node)
        {
            VisitRvalue(node.Expression);
            return null;
        }

        public override BoundNode VisitCall(BoundCall node)
        {
            bool num = node.Method.CallsAreOmitted(node.SyntaxTree);
            TLocalState state = default(TLocalState);
            if (num)
            {
                state = State.Clone();
                SetUnreachable();
            }
            VisitReceiverBeforeCall(node.ReceiverOpt, node.Method);
            VisitArgumentsBeforeCall(node.Arguments, node.ArgumentRefKindsOpt);
            if (node.Method?.OriginalDefinition is LocalFunctionSymbol symbol)
            {
                VisitLocalFunctionUse(symbol, node.Syntax, isCall: true);
            }
            VisitArgumentsAfterCall(node.Arguments, node.ArgumentRefKindsOpt, node.Method);
            VisitReceiverAfterCall(node.ReceiverOpt, node.Method);
            if (num)
            {
                State = state;
            }
            return null;
        }

        protected void VisitLocalFunctionUse(LocalFunctionSymbol symbol, SyntaxNode syntax, bool isCall)
        {
            TLocalFunctionState orCreateLocalFuncUsages = GetOrCreateLocalFuncUsages(symbol);
            VisitLocalFunctionUse(symbol, orCreateLocalFuncUsages, syntax, isCall);
        }

        protected virtual void VisitLocalFunctionUse(LocalFunctionSymbol symbol, TLocalFunctionState localFunctionState, SyntaxNode syntax, bool isCall)
        {
            if (isCall)
            {
                Join(ref State, ref localFunctionState.StateFromBottom);
                Meet(ref State, ref localFunctionState.StateFromTop);
            }
            localFunctionState.Visited = true;
        }

        private void VisitReceiverBeforeCall(BoundExpression receiverOpt, MethodSymbol method)
        {
            if ((object)method == null || method.MethodKind != MethodKind.Constructor)
            {
                VisitRvalue(receiverOpt);
            }
        }

        private void VisitReceiverAfterCall(BoundExpression receiverOpt, MethodSymbol method)
        {
            if (receiverOpt == null)
            {
                return;
            }
            if ((object)method == null)
            {
                WriteArgument(receiverOpt, RefKind.Ref, null);
            }
            else if (method.TryGetThisParameter(out ParameterSymbol thisParameter) && (object)thisParameter != null && !TypeIsImmutable(thisParameter.Type))
            {
                RefKind refKind = thisParameter.RefKind;
                if (refKind.IsWritableReference())
                {
                    WriteArgument(receiverOpt, refKind, method);
                }
            }
        }

        private static bool TypeIsImmutable(TypeSymbol t)
        {
            SpecialType specialType = t.SpecialType;
            if ((uint)(specialType - 7) <= 12u || specialType == SpecialType.System_DateTime)
            {
                return true;
            }
            return t.IsNullableType();
        }

        public override BoundNode VisitIndexerAccess(BoundIndexerAccess node)
        {
            MethodSymbol readMethod = GetReadMethod(node.Indexer);
            VisitReceiverBeforeCall(node.ReceiverOpt, readMethod);
            VisitArguments(node.Arguments, node.ArgumentRefKindsOpt, readMethod);
            if ((object)readMethod != null)
            {
                VisitReceiverAfterCall(node.ReceiverOpt, readMethod);
            }
            return null;
        }

        public override BoundNode VisitIndexOrRangePatternIndexerAccess(BoundIndexOrRangePatternIndexerAccess node)
        {
            VisitRvalue(node.Receiver);
            MethodSymbol readMethod = GetReadMethod(node.LengthOrCountProperty);
            VisitReceiverAfterCall(node.Receiver, readMethod);
            VisitRvalue(node.Argument);
            Symbol patternSymbol = node.PatternSymbol;
            MethodSymbol methodSymbol2;
            if (!(patternSymbol is PropertySymbol property))
            {
                if (!(patternSymbol is MethodSymbol methodSymbol))
                {
                    throw ExceptionUtilities.UnexpectedValue(node.PatternSymbol);
                }
                methodSymbol2 = methodSymbol;
            }
            else
            {
                methodSymbol2 = GetReadMethod(property);
            }
            readMethod = methodSymbol2;
            VisitReceiverAfterCall(node.Receiver, readMethod);
            return null;
        }

        public override BoundNode VisitEventAssignmentOperator(BoundEventAssignmentOperator node)
        {
            VisitRvalue(node.ReceiverOpt);
            VisitRvalue(node.Argument);
            return null;
        }

        protected virtual void VisitArguments(ImmutableArray<BoundExpression> arguments, ImmutableArray<RefKind> refKindsOpt, MethodSymbol method)
        {
            VisitArgumentsBeforeCall(arguments, refKindsOpt);
            VisitArgumentsAfterCall(arguments, refKindsOpt, method);
        }

        private void VisitArgumentsBeforeCall(ImmutableArray<BoundExpression> arguments, ImmutableArray<RefKind> refKindsOpt)
        {
            for (int i = 0; i < arguments.Length; i++)
            {
                RefKind refKind = GetRefKind(refKindsOpt, i);
                if (refKind != RefKind.Out)
                {
                    VisitRvalue(arguments[i], refKind != RefKind.None);
                }
                else
                {
                    VisitLvalue(arguments[i]);
                }
            }
        }

        private void VisitArgumentsAfterCall(ImmutableArray<BoundExpression> arguments, ImmutableArray<RefKind> refKindsOpt, MethodSymbol method)
        {
            for (int i = 0; i < arguments.Length; i++)
            {
                RefKind refKind = GetRefKind(refKindsOpt, i);
                if (refKind != 0)
                {
                    WriteArgument(arguments[i], refKind, method);
                }
            }
        }

        protected static RefKind GetRefKind(ImmutableArray<RefKind> refKindsOpt, int index)
        {
            if (!refKindsOpt.IsDefault && refKindsOpt.Length > index)
            {
                return refKindsOpt[index];
            }
            return RefKind.None;
        }

        protected virtual void WriteArgument(BoundExpression arg, RefKind refKind, MethodSymbol method)
        {
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
                else
                {
                    VisitRvalue(current as BoundExpression);
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

        public override BoundNode VisitDelegateCreationExpression(BoundDelegateCreationExpression node)
        {
            if (node.Argument is BoundMethodGroup boundMethodGroup)
            {
                if ((object)node.MethodOpt != null && node.MethodOpt!.RequiresInstanceReceiver)
                {
                    if (TrackingRegions)
                    {
                        if (boundMethodGroup == firstInRegion && regionPlace == RegionPlace.Before)
                        {
                            EnterRegion();
                        }
                        VisitRvalue(boundMethodGroup.ReceiverOpt);
                        if (boundMethodGroup == lastInRegion && IsInside)
                        {
                            LeaveRegion();
                        }
                    }
                    else
                    {
                        VisitRvalue(boundMethodGroup.ReceiverOpt);
                    }
                }
                else if (node.MethodOpt?.OriginalDefinition is LocalFunctionSymbol symbol)
                {
                    VisitLocalFunctionUse(symbol, node.Syntax, isCall: false);
                }
            }
            else
            {
                VisitRvalue(node.Argument);
            }
            return null;
        }

        public override BoundNode VisitTypeExpression(BoundTypeExpression node)
        {
            return null;
        }

        public override BoundNode VisitTypeOrValueExpression(BoundTypeOrValueExpression node)
        {
            return Visit(node.Data.ValueExpression);
        }

        public override BoundNode VisitLiteral(BoundLiteral node)
        {
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

        public override BoundNode VisitConversion(BoundConversion node)
        {
            if (node.ConversionKind == ConversionKind.MethodGroup)
            {
                if (node.IsExtensionMethod || ((object)node.SymbolOpt != null && node.SymbolOpt!.RequiresInstanceReceiver))
                {
                    BoundExpression receiverOpt = ((BoundMethodGroup)node.Operand).ReceiverOpt;
                    if (TrackingRegions)
                    {
                        if (node.Operand == firstInRegion && regionPlace == RegionPlace.Before)
                        {
                            EnterRegion();
                        }
                        VisitRvalue(receiverOpt);
                        if (node.Operand == lastInRegion && IsInside)
                        {
                            LeaveRegion();
                        }
                    }
                    else
                    {
                        VisitRvalue(receiverOpt);
                    }
                }
                else if (node.SymbolOpt?.OriginalDefinition is LocalFunctionSymbol symbol)
                {
                    VisitLocalFunctionUse(symbol, node.Syntax, isCall: false);
                }
            }
            else
            {
                Visit(node.Operand);
            }
            return null;
        }

        public override BoundNode VisitIfStatement(BoundIfStatement node)
        {
            VisitCondition(node.Condition);
            TLocalState stateWhenTrue = StateWhenTrue;
            TLocalState stateWhenFalse = StateWhenFalse;
            SetState(stateWhenTrue);
            VisitStatement(node.Consequence);
            stateWhenTrue = State;
            SetState(stateWhenFalse);
            if (node.AlternativeOpt != null)
            {
                VisitStatement(node.AlternativeOpt);
            }
            Join(ref State, ref stateWhenTrue);
            return null;
        }

        public override BoundNode VisitTryStatement(BoundTryStatement node)
        {
            SavedPending oldPending = SavePending();
            TLocalState tryState = State.Clone();
            SavedPending oldPending2 = SavePending();
            VisitTryBlockWithAnyTransferFunction(node.TryBlock, node, ref tryState);
            TLocalState finallyState = tryState.Clone();
            TLocalState self = State;
            ImmutableArray<BoundCatchBlock>.Enumerator enumerator = node.CatchBlocks.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundCatchBlock current = enumerator.Current;
                SetState(tryState.Clone());
                VisitCatchBlockWithAnyTransferFunction(current, ref finallyState);
                Join(ref self, ref State);
            }
            RestorePending(oldPending2);
            if (node.FinallyBlockOpt != null)
            {
                SetState(finallyState);
                SavedPending oldPending3 = SavePending();
                TLocalState stateMovedUp = ReachableBottomState();
                VisitFinallyBlockWithAnyTransferFunction(node.FinallyBlockOpt, ref stateMovedUp);
                ArrayBuilder<PendingBranch>.Enumerator enumerator2 = oldPending3.PendingBranches.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    PendingBranch current2 = enumerator2.Current;
                    if (current2.Branch != null && current2.Branch.Kind != BoundKind.YieldReturnStatement)
                    {
                        updatePendingBranchState(ref current2.State, ref stateMovedUp);
                        if (current2.IsConditionalState)
                        {
                            updatePendingBranchState(ref current2.StateWhenTrue, ref stateMovedUp);
                            updatePendingBranchState(ref current2.StateWhenFalse, ref stateMovedUp);
                        }
                    }
                }
                RestorePending(oldPending3);
                Meet(ref self, ref State);
                if (_nonMonotonicTransfer)
                {
                    Join(ref self, ref stateMovedUp);
                }
            }
            SetState(self);
            RestorePending(oldPending);
            return null;
            void updatePendingBranchState(ref TLocalState stateToUpdate, ref TLocalState stateMovedUpInFinally)
            {
                Meet(ref stateToUpdate, ref State);
                if (_nonMonotonicTransfer)
                {
                    Join(ref stateToUpdate, ref stateMovedUpInFinally);
                }
            }
        }

        protected virtual void JoinTryBlockState(ref TLocalState self, ref TLocalState other)
        {
            Join(ref self, ref other);
        }

        private void VisitTryBlockWithAnyTransferFunction(BoundStatement tryBlock, BoundTryStatement node, ref TLocalState tryState)
        {
            if (_nonMonotonicTransfer)
            {
                Optional<TLocalState> nonMonotonicState = NonMonotonicState;
                NonMonotonicState = ReachableBottomState();
                VisitTryBlock(tryBlock, node, ref tryState);
                TLocalState other = NonMonotonicState.Value;
                Join(ref tryState, ref other);
                if (nonMonotonicState.HasValue)
                {
                    TLocalState self = nonMonotonicState.Value;
                    JoinTryBlockState(ref self, ref other);
                    nonMonotonicState = self;
                }
                NonMonotonicState = nonMonotonicState;
            }
            else
            {
                VisitTryBlock(tryBlock, node, ref tryState);
            }
        }

        protected virtual void VisitTryBlock(BoundStatement tryBlock, BoundTryStatement node, ref TLocalState tryState)
        {
            VisitStatement(tryBlock);
        }

        private void VisitCatchBlockWithAnyTransferFunction(BoundCatchBlock catchBlock, ref TLocalState finallyState)
        {
            if (_nonMonotonicTransfer)
            {
                Optional<TLocalState> nonMonotonicState = NonMonotonicState;
                NonMonotonicState = ReachableBottomState();
                VisitCatchBlock(catchBlock, ref finallyState);
                TLocalState other = NonMonotonicState.Value;
                Join(ref finallyState, ref other);
                if (nonMonotonicState.HasValue)
                {
                    TLocalState self = nonMonotonicState.Value;
                    JoinTryBlockState(ref self, ref other);
                    nonMonotonicState = self;
                }
                NonMonotonicState = nonMonotonicState;
            }
            else
            {
                VisitCatchBlock(catchBlock, ref finallyState);
            }
        }

        protected virtual void VisitCatchBlock(BoundCatchBlock catchBlock, ref TLocalState finallyState)
        {
            if (catchBlock.ExceptionSourceOpt != null)
            {
                VisitLvalue(catchBlock.ExceptionSourceOpt);
            }
            if (catchBlock.ExceptionFilterPrologueOpt != null)
            {
                VisitStatementList(catchBlock.ExceptionFilterPrologueOpt);
            }
            if (catchBlock.ExceptionFilterOpt != null)
            {
                VisitCondition(catchBlock.ExceptionFilterOpt);
                SetState(StateWhenTrue);
            }
            VisitStatement(catchBlock.Body);
        }

        private void VisitFinallyBlockWithAnyTransferFunction(BoundStatement finallyBlock, ref TLocalState stateMovedUp)
        {
            if (_nonMonotonicTransfer)
            {
                Optional<TLocalState> nonMonotonicState = NonMonotonicState;
                NonMonotonicState = ReachableBottomState();
                VisitFinallyBlock(finallyBlock, ref stateMovedUp);
                TLocalState other = NonMonotonicState.Value;
                Join(ref stateMovedUp, ref other);
                if (nonMonotonicState.HasValue)
                {
                    TLocalState self = nonMonotonicState.Value;
                    JoinTryBlockState(ref self, ref other);
                    nonMonotonicState = self;
                }
                NonMonotonicState = nonMonotonicState;
            }
            else
            {
                VisitFinallyBlock(finallyBlock, ref stateMovedUp);
            }
        }

        protected virtual void VisitFinallyBlock(BoundStatement finallyBlock, ref TLocalState stateMovedUp)
        {
            VisitStatement(finallyBlock);
        }

        public override BoundNode VisitExtractedFinallyBlock(BoundExtractedFinallyBlock node)
        {
            return VisitBlock(node.FinallyBlock);
        }

        public override BoundNode VisitReturnStatement(BoundReturnStatement node)
        {
            BoundNode result = VisitReturnStatementNoAdjust(node);
            PendingBranches.Add(new PendingBranch(node, State, null));
            SetUnreachable();
            return result;
        }

        protected virtual BoundNode VisitReturnStatementNoAdjust(BoundReturnStatement node)
        {
            VisitRvalue(node.ExpressionOpt, node.RefKind != RefKind.None);
            if (node.RefKind != 0)
            {
                WriteArgument(node.ExpressionOpt, node.RefKind, null);
            }
            return null;
        }

        public override BoundNode VisitThisReference(BoundThisReference node)
        {
            return null;
        }

        public override BoundNode VisitPreviousSubmissionReference(BoundPreviousSubmissionReference node)
        {
            return null;
        }

        public override BoundNode VisitHostObjectMemberReference(BoundHostObjectMemberReference node)
        {
            return null;
        }

        public override BoundNode VisitParameter(BoundParameter node)
        {
            return null;
        }

        protected virtual void VisitLvalueParameter(BoundParameter node)
        {
        }

        public override BoundNode VisitObjectCreationExpression(BoundObjectCreationExpression node)
        {
            VisitArguments(node.Arguments, node.ArgumentRefKindsOpt, node.Constructor);
            VisitRvalue(node.InitializerExpressionOpt);
            return null;
        }

        public override BoundNode VisitNewT(BoundNewT node)
        {
            VisitRvalue(node.InitializerExpressionOpt);
            return null;
        }

        public override BoundNode VisitNoPiaObjectCreationExpression(BoundNoPiaObjectCreationExpression node)
        {
            VisitRvalue(node.InitializerExpressionOpt);
            return null;
        }

        protected virtual void PropertySetter(BoundExpression node, BoundExpression receiver, MethodSymbol setter, BoundExpression value = null)
        {
            VisitReceiverAfterCall(receiver, setter);
        }

        private bool RegularPropertyAccess(BoundExpression expr)
        {
            if (expr.Kind != BoundKind.PropertyAccess)
            {
                return false;
            }
            return !Binder.AccessingAutoPropertyFromConstructor((BoundPropertyAccess)expr, _symbol);
        }

        public override BoundNode VisitAssignmentOperator(BoundAssignmentOperator node)
        {
            if (RegularPropertyAccess(node.Left))
            {
                BoundPropertyAccess boundPropertyAccess = (BoundPropertyAccess)node.Left;
                PropertySymbol propertySymbol = boundPropertyAccess.PropertySymbol;
                if (propertySymbol.RefKind == RefKind.None)
                {
                    MethodSymbol writeMethod = GetWriteMethod(propertySymbol);
                    VisitReceiverBeforeCall(boundPropertyAccess.ReceiverOpt, writeMethod);
                    VisitRvalue(node.Right);
                    PropertySetter(node, boundPropertyAccess.ReceiverOpt, writeMethod, node.Right);
                    return null;
                }
            }
            VisitLvalue(node.Left);
            VisitRvalue(node.Right, node.IsRef);
            if (node.IsRef)
            {
                RefKind refKind = ((node.Left.Kind == BoundKind.BadExpression) ? RefKind.Ref : node.Left.GetRefKind());
                WriteArgument(node.Right, refKind, null);
            }
            return null;
        }

        public override BoundNode VisitDeconstructionAssignmentOperator(BoundDeconstructionAssignmentOperator node)
        {
            VisitLvalue(node.Left);
            VisitRvalue(node.Right);
            return null;
        }

        public sealed override BoundNode VisitOutDeconstructVarPendingInference(OutDeconstructVarPendingInference node)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override BoundNode VisitCompoundAssignmentOperator(BoundCompoundAssignmentOperator node)
        {
            VisitCompoundAssignmentTarget(node);
            VisitRvalue(node.Right);
            AfterRightHasBeenVisited(node);
            return null;
        }

        protected void VisitCompoundAssignmentTarget(BoundCompoundAssignmentOperator node)
        {
            if (RegularPropertyAccess(node.Left))
            {
                BoundPropertyAccess boundPropertyAccess = (BoundPropertyAccess)node.Left;
                PropertySymbol propertySymbol = boundPropertyAccess.PropertySymbol;
                if (propertySymbol.RefKind == RefKind.None)
                {
                    MethodSymbol readMethod = GetReadMethod(propertySymbol);
                    VisitReceiverBeforeCall(boundPropertyAccess.ReceiverOpt, readMethod);
                    VisitReceiverAfterCall(boundPropertyAccess.ReceiverOpt, readMethod);
                    return;
                }
            }
            VisitRvalue(node.Left, isKnownToBeAnLvalue: true);
        }

        protected void AfterRightHasBeenVisited(BoundCompoundAssignmentOperator node)
        {
            if (RegularPropertyAccess(node.Left))
            {
                BoundPropertyAccess boundPropertyAccess = (BoundPropertyAccess)node.Left;
                PropertySymbol propertySymbol = boundPropertyAccess.PropertySymbol;
                if (propertySymbol.RefKind == RefKind.None)
                {
                    MethodSymbol writeMethod = GetWriteMethod(propertySymbol);
                    PropertySetter(node, boundPropertyAccess.ReceiverOpt, writeMethod);
                    VisitReceiverAfterCall(boundPropertyAccess.ReceiverOpt, writeMethod);
                }
            }
        }

        public override BoundNode VisitFieldAccess(BoundFieldAccess node)
        {
            VisitFieldAccessInternal(node.ReceiverOpt, node.FieldSymbol);
            return null;
        }

        private void VisitFieldAccessInternal(BoundExpression receiverOpt, FieldSymbol fieldSymbol)
        {
            if ((object)fieldSymbol != null && (fieldSymbol.IsFixedSizeBuffer || (!fieldSymbol.IsStatic && fieldSymbol.ContainingType.TypeKind == TypeKind.Struct && receiverOpt != null && receiverOpt.Kind != BoundKind.TypeExpression && (object)receiverOpt.Type != null && !receiverOpt.Type.IsPrimitiveRecursiveStruct())))
            {
                VisitLvalue(receiverOpt);
            }
            else
            {
                VisitRvalue(receiverOpt);
            }
        }

        public override BoundNode VisitFieldInfo(BoundFieldInfo node)
        {
            return null;
        }

        public override BoundNode VisitMethodInfo(BoundMethodInfo node)
        {
            return null;
        }

        public override BoundNode VisitPropertyAccess(BoundPropertyAccess node)
        {
            PropertySymbol propertySymbol = node.PropertySymbol;
            if (Binder.AccessingAutoPropertyFromConstructor(node, _symbol))
            {
                SynthesizedBackingFieldSymbol synthesizedBackingFieldSymbol = (propertySymbol as SourcePropertySymbolBase)?.BackingField;
                if (synthesizedBackingFieldSymbol != null)
                {
                    VisitFieldAccessInternal(node.ReceiverOpt, synthesizedBackingFieldSymbol);
                    return null;
                }
            }
            MethodSymbol readMethod = GetReadMethod(propertySymbol);
            VisitReceiverBeforeCall(node.ReceiverOpt, readMethod);
            VisitReceiverAfterCall(node.ReceiverOpt, readMethod);
            return null;
        }

        public override BoundNode VisitEventAccess(BoundEventAccess node)
        {
            VisitFieldAccessInternal(node.ReceiverOpt, node.EventSymbol.AssociatedField);
            return null;
        }

        public override BoundNode VisitRangeVariable(BoundRangeVariable node)
        {
            return null;
        }

        public override BoundNode VisitQueryClause(BoundQueryClause node)
        {
            VisitRvalue(node.UnoptimizedForm ?? node.Value);
            return null;
        }

        private BoundNode VisitMultipleLocalDeclarationsBase(BoundMultipleLocalDeclarationsBase node)
        {
            ImmutableArray<BoundLocalDeclaration>.Enumerator enumerator = node.LocalDeclarations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundLocalDeclaration current = enumerator.Current;
                Visit(current);
            }
            return null;
        }

        public override BoundNode VisitMultipleLocalDeclarations(BoundMultipleLocalDeclarations node)
        {
            return VisitMultipleLocalDeclarationsBase(node);
        }

        public override BoundNode VisitUsingLocalDeclarations(BoundUsingLocalDeclarations node)
        {
            if (AwaitUsingAndForeachAddsPendingBranch && node.AwaitOpt != null)
            {
                PendingBranches.Add(new PendingBranch(node, State, null));
            }
            return VisitMultipleLocalDeclarationsBase(node);
        }

        public override BoundNode VisitWhileStatement(BoundWhileStatement node)
        {
            LoopHead(node);
            VisitCondition(node.Condition);
            TLocalState stateWhenTrue = StateWhenTrue;
            TLocalState stateWhenFalse = StateWhenFalse;
            SetState(stateWhenTrue);
            VisitStatement(node.Body);
            ResolveContinues(node.ContinueLabel);
            LoopTail(node);
            ResolveBreaks(stateWhenFalse, node.BreakLabel);
            return null;
        }

        public override BoundNode VisitWithExpression(BoundWithExpression node)
        {
            VisitRvalue(node.Receiver);
            VisitObjectOrCollectionInitializerExpression(node.InitializerExpression.Initializers);
            return null;
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

        public override BoundNode VisitBinaryOperator(BoundBinaryOperator node)
        {
            if (node.OperatorKind.IsLogical())
            {
                VisitBinaryLogicalOperatorChildren(node);
            }
            else
            {
                VisitBinaryOperatorChildren(node);
            }
            return null;
        }

        public override BoundNode VisitUserDefinedConditionalLogicalOperator(BoundUserDefinedConditionalLogicalOperator node)
        {
            VisitBinaryLogicalOperatorChildren(node);
            return null;
        }

        private void VisitBinaryLogicalOperatorChildren(BoundExpression node)
        {
            ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
            BoundExpression boundExpression = node;
            while (true)
            {
                BoundExpression boundExpression2;
                switch (boundExpression.Kind)
                {
                    case BoundKind.BinaryOperator:
                        {
                            BoundBinaryOperator boundBinaryOperator = (BoundBinaryOperator)boundExpression;
                            if (boundBinaryOperator.OperatorKind.IsLogical())
                            {
                                boundExpression2 = boundExpression;
                                boundExpression = boundBinaryOperator.Left;
                                goto IL_0049;
                            }
                            break;
                        }
                    case BoundKind.UserDefinedConditionalLogicalOperator:
                        boundExpression2 = boundExpression;
                        boundExpression = ((BoundUserDefinedConditionalLogicalOperator)boundExpression2).Left;
                        goto IL_0049;
                }
                break;
            IL_0049:
                instance.Push(boundExpression2);
            }
            VisitCondition(boundExpression);
            while (true)
            {
                BoundExpression boundExpression2 = instance.Pop();
                BinaryOperatorKind operatorKind;
                BoundExpression right;
                switch (boundExpression2.Kind)
                {
                    case BoundKind.BinaryOperator:
                        {
                            BoundBinaryOperator obj2 = (BoundBinaryOperator)boundExpression2;
                            operatorKind = obj2.OperatorKind;
                            right = obj2.Right;
                            break;
                        }
                    case BoundKind.UserDefinedConditionalLogicalOperator:
                        {
                            BoundUserDefinedConditionalLogicalOperator obj = (BoundUserDefinedConditionalLogicalOperator)boundExpression2;
                            operatorKind = obj.OperatorKind;
                            right = obj.Right;
                            break;
                        }
                    default:
                        throw ExceptionUtilities.UnexpectedValue(boundExpression2.Kind);
                }
                bool flag = operatorKind.Operator() == BinaryOperatorKind.And;
                bool isBool = operatorKind.OperandTypes() == BinaryOperatorKind.Bool;
                TLocalState leftTrue = StateWhenTrue;
                TLocalState leftFalse = StateWhenFalse;
                SetState(flag ? leftTrue : leftFalse);
                AfterLeftChildOfBinaryLogicalOperatorHasBeenVisited(boundExpression2, right, flag, isBool, ref leftTrue, ref leftFalse);
                if (instance.Count == 0)
                {
                    break;
                }
                AdjustConditionalState(boundExpression2);
            }
            instance.Free();
        }

        protected virtual void AfterLeftChildOfBinaryLogicalOperatorHasBeenVisited(BoundExpression binary, BoundExpression right, bool isAnd, bool isBool, ref TLocalState leftTrue, ref TLocalState leftFalse)
        {
            Visit(right);
            AfterRightChildOfBinaryLogicalOperatorHasBeenVisited(binary, right, isAnd, isBool, ref leftTrue, ref leftFalse);
        }

        protected void AfterRightChildOfBinaryLogicalOperatorHasBeenVisited(BoundExpression binary, BoundExpression right, bool isAnd, bool isBool, ref TLocalState leftTrue, ref TLocalState leftFalse)
        {
            AdjustConditionalState(right);
            if (!isBool)
            {
                Unsplit();
                Split();
            }
            TLocalState self = StateWhenTrue;
            TLocalState self2 = StateWhenFalse;
            if (isAnd)
            {
                Join(ref self2, ref leftFalse);
            }
            else
            {
                Join(ref self, ref leftTrue);
            }
            SetConditionalState(self, self2);
            if (!isBool)
            {
                Unsplit();
            }
        }

        private void VisitBinaryOperatorChildren(BoundBinaryOperator node)
        {
            ArrayBuilder<BoundBinaryOperator> instance = ArrayBuilder<BoundBinaryOperator>.GetInstance();
            BoundBinaryOperator boundBinaryOperator = node;
            do
            {
                instance.Push(boundBinaryOperator);
                boundBinaryOperator = boundBinaryOperator.Left as BoundBinaryOperator;
            }
            while (boundBinaryOperator != null && !boundBinaryOperator.OperatorKind.IsLogical());
            VisitBinaryOperatorChildren(instance);
            instance.Free();
        }

        protected virtual void VisitBinaryOperatorChildren(ArrayBuilder<BoundBinaryOperator> stack)
        {
            BoundBinaryOperator boundBinaryOperator = stack.Pop();
            VisitRvalue(boundBinaryOperator.Left);
            while (true)
            {
                VisitRvalue(boundBinaryOperator.Right);
                if (stack.Count != 0)
                {
                    Unsplit();
                    boundBinaryOperator = stack.Pop();
                    continue;
                }
                break;
            }
        }

        public override BoundNode VisitUnaryOperator(BoundUnaryOperator node)
        {
            if (node.OperatorKind == UnaryOperatorKind.BoolLogicalNegation)
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

        public override BoundNode VisitRangeExpression(BoundRangeExpression node)
        {
            if (node.LeftOperandOpt != null)
            {
                VisitRvalue(node.LeftOperandOpt);
            }
            if (node.RightOperandOpt != null)
            {
                VisitRvalue(node.RightOperandOpt);
            }
            return null;
        }

        public override BoundNode VisitFromEndIndexExpression(BoundFromEndIndexExpression node)
        {
            VisitRvalue(node.Operand);
            return null;
        }

        public override BoundNode VisitAwaitExpression(BoundAwaitExpression node)
        {
            VisitRvalue(node.Expression);
            PendingBranches.Add(new PendingBranch(node, State, null));
            return null;
        }

        public override BoundNode VisitIncrementOperator(BoundIncrementOperator node)
        {
            if (RegularPropertyAccess(node.Operand))
            {
                BoundPropertyAccess boundPropertyAccess = (BoundPropertyAccess)node.Operand;
                PropertySymbol propertySymbol = boundPropertyAccess.PropertySymbol;
                if (propertySymbol.RefKind == RefKind.None)
                {
                    MethodSymbol readMethod = GetReadMethod(propertySymbol);
                    MethodSymbol writeMethod = GetWriteMethod(propertySymbol);
                    VisitReceiverBeforeCall(boundPropertyAccess.ReceiverOpt, readMethod);
                    VisitReceiverAfterCall(boundPropertyAccess.ReceiverOpt, readMethod);
                    PropertySetter(node, boundPropertyAccess.ReceiverOpt, writeMethod);
                    return null;
                }
            }
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
            if (node.InitializerOpt != null)
            {
                VisitArrayInitializationInternal(node, node.InitializerOpt);
            }
            return null;
        }

        private void VisitArrayInitializationInternal(BoundArrayCreation arrayCreation, BoundArrayInitialization node)
        {
            ImmutableArray<BoundExpression>.Enumerator enumerator = node.Initializers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundExpression current = enumerator.Current;
                if (current.Kind == BoundKind.ArrayInitialization)
                {
                    VisitArrayInitializationInternal(arrayCreation, (BoundArrayInitialization)current);
                }
                else
                {
                    VisitRvalue(current);
                }
            }
        }

        public override BoundNode VisitForStatement(BoundForStatement node)
        {
            if (node.Initializer != null)
            {
                VisitStatement(node.Initializer);
            }
            LoopHead(node);
            TLocalState state;
            TLocalState breakState;
            if (node.Condition != null)
            {
                VisitCondition(node.Condition);
                state = StateWhenTrue;
                breakState = StateWhenFalse;
            }
            else
            {
                state = State;
                breakState = UnreachableState();
            }
            SetState(state);
            VisitStatement(node.Body);
            ResolveContinues(node.ContinueLabel);
            if (node.Increment != null)
            {
                VisitStatement(node.Increment);
            }
            LoopTail(node);
            ResolveBreaks(breakState, node.BreakLabel);
            return null;
        }

        public override BoundNode VisitForEachStatement(BoundForEachStatement node)
        {
            VisitForEachExpression(node);
            TLocalState breakState = State.Clone();
            LoopHead(node);
            VisitForEachIterationVariables(node);
            VisitStatement(node.Body);
            ResolveContinues(node.ContinueLabel);
            LoopTail(node);
            ResolveBreaks(breakState, node.BreakLabel);
            if (AwaitUsingAndForeachAddsPendingBranch && ((CommonForEachStatementSyntax)node.Syntax).AwaitKeyword != default(SyntaxToken))
            {
                PendingBranches.Add(new PendingBranch(node, State, null));
            }
            return null;
        }

        protected virtual void VisitForEachExpression(BoundForEachStatement node)
        {
            VisitRvalue(node.Expression);
        }

        public virtual void VisitForEachIterationVariables(BoundForEachStatement node)
        {
        }

        public override BoundNode VisitAsOperator(BoundAsOperator node)
        {
            VisitRvalue(node.Operand);
            return null;
        }

        public override BoundNode VisitIsOperator(BoundIsOperator node)
        {
            VisitRvalue(node.Operand);
            return null;
        }

        public override BoundNode VisitMethodGroup(BoundMethodGroup node)
        {
            if (node.ReceiverOpt != null)
            {
                VisitRvalue(node.ReceiverOpt);
            }
            return null;
        }

        public override BoundNode VisitNullCoalescingOperator(BoundNullCoalescingOperator node)
        {
            VisitRvalue(node.LeftOperand);
            if (IsConstantNull(node.LeftOperand))
            {
                VisitRvalue(node.RightOperand);
            }
            else
            {
                TLocalState other = State.Clone();
                if (node.LeftOperand.ConstantValue != null)
                {
                    SetUnreachable();
                }
                VisitRvalue(node.RightOperand);
                Join(ref State, ref other);
            }
            return null;
        }

        public override BoundNode VisitConditionalAccess(BoundConditionalAccess node)
        {
            VisitRvalue(node.Receiver);
            if (node.Receiver.ConstantValue != null && !IsConstantNull(node.Receiver))
            {
                VisitRvalue(node.AccessExpression);
            }
            else
            {
                TLocalState other = State.Clone();
                if (IsConstantNull(node.Receiver))
                {
                    SetUnreachable();
                }
                VisitRvalue(node.AccessExpression);
                Join(ref State, ref other);
            }
            return null;
        }

        public override BoundNode VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node)
        {
            VisitRvalue(node.Receiver);
            TLocalState other = State.Clone();
            VisitRvalue(node.WhenNotNull);
            Join(ref State, ref other);
            if (node.WhenNullOpt != null)
            {
                other = State.Clone();
                VisitRvalue(node.WhenNullOpt);
                Join(ref State, ref other);
            }
            return null;
        }

        public override BoundNode VisitConditionalReceiver(BoundConditionalReceiver node)
        {
            return null;
        }

        public override BoundNode VisitComplexConditionalReceiver(BoundComplexConditionalReceiver node)
        {
            TLocalState other = State.Clone();
            VisitRvalue(node.ValueTypeReceiver);
            Join(ref State, ref other);
            other = State.Clone();
            VisitRvalue(node.ReferenceTypeReceiver);
            Join(ref State, ref other);
            return null;
        }

        public override BoundNode VisitSequence(BoundSequence node)
        {
            ImmutableArray<BoundExpression> sideEffects = node.SideEffects;
            if (!sideEffects.IsEmpty)
            {
                ImmutableArray<BoundExpression>.Enumerator enumerator = sideEffects.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    BoundExpression current = enumerator.Current;
                    VisitRvalue(current);
                }
            }
            VisitRvalue(node.Value);
            return null;
        }

        public override BoundNode VisitSequencePoint(BoundSequencePoint node)
        {
            if (node.StatementOpt != null)
            {
                VisitStatement(node.StatementOpt);
            }
            return null;
        }

        public override BoundNode VisitSequencePointExpression(BoundSequencePointExpression node)
        {
            VisitRvalue(node.Expression);
            return null;
        }

        public override BoundNode VisitSequencePointWithSpan(BoundSequencePointWithSpan node)
        {
            if (node.StatementOpt != null)
            {
                VisitStatement(node.StatementOpt);
            }
            return null;
        }

        public override BoundNode VisitStatementList(BoundStatementList node)
        {
            return VisitStatementListWorker(node);
        }

        private BoundNode VisitStatementListWorker(BoundStatementList node)
        {
            ImmutableArray<BoundStatement>.Enumerator enumerator = node.Statements.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundStatement current = enumerator.Current;
                VisitStatement(current);
            }
            return null;
        }

        public override BoundNode VisitTypeOrInstanceInitializers(BoundTypeOrInstanceInitializers node)
        {
            return VisitStatementListWorker(node);
        }

        public override BoundNode VisitUnboundLambda(UnboundLambda node)
        {
            return VisitLambda(node.BindForErrorRecovery());
        }

        public override BoundNode VisitBreakStatement(BoundBreakStatement node)
        {
            PendingBranches.Add(new PendingBranch(node, State, node.Label));
            SetUnreachable();
            return null;
        }

        public override BoundNode VisitContinueStatement(BoundContinueStatement node)
        {
            PendingBranches.Add(new PendingBranch(node, State, node.Label));
            SetUnreachable();
            return null;
        }

        public override BoundNode VisitUnconvertedConditionalOperator(BoundUnconvertedConditionalOperator node)
        {
            return VisitConditionalOperatorCore(node, isByRef: false, node.Condition, node.Consequence, node.Alternative);
        }

        public override BoundNode VisitConditionalOperator(BoundConditionalOperator node)
        {
            return VisitConditionalOperatorCore(node, node.IsRef, node.Condition, node.Consequence, node.Alternative);
        }

        protected virtual BoundNode VisitConditionalOperatorCore(BoundExpression node, bool isByRef, BoundExpression condition, BoundExpression consequence, BoundExpression alternative)
        {
            VisitCondition(condition);
            TLocalState stateWhenTrue = StateWhenTrue;
            TLocalState stateWhenFalse = StateWhenFalse;
            if (IsConstantTrue(condition))
            {
                VisitConditionalOperand(stateWhenFalse, alternative, isByRef);
                VisitConditionalOperand(stateWhenTrue, consequence, isByRef);
            }
            else if (IsConstantFalse(condition))
            {
                VisitConditionalOperand(stateWhenTrue, consequence, isByRef);
                VisitConditionalOperand(stateWhenFalse, alternative, isByRef);
            }
            else
            {
                VisitConditionalOperand(stateWhenTrue, consequence, isByRef);
                Unsplit();
                stateWhenTrue = State;
                VisitConditionalOperand(stateWhenFalse, alternative, isByRef);
                Unsplit();
                Join(ref State, ref stateWhenTrue);
            }
            return null;
        }

        private void VisitConditionalOperand(TLocalState state, BoundExpression operand, bool isByRef)
        {
            SetState(state);
            if (isByRef)
            {
                VisitLvalue(operand);
                WriteArgument(operand, RefKind.Ref, null);
            }
            else
            {
                Visit(operand);
            }
        }

        public override BoundNode VisitBaseReference(BoundBaseReference node)
        {
            return null;
        }

        public override BoundNode VisitDoStatement(BoundDoStatement node)
        {
            LoopHead(node);
            VisitStatement(node.Body);
            ResolveContinues(node.ContinueLabel);
            VisitCondition(node.Condition);
            TLocalState stateWhenFalse = StateWhenFalse;
            SetState(StateWhenTrue);
            LoopTail(node);
            ResolveBreaks(stateWhenFalse, node.BreakLabel);
            return null;
        }

        public override BoundNode VisitGotoStatement(BoundGotoStatement node)
        {
            PendingBranches.Add(new PendingBranch(node, State, node.Label));
            SetUnreachable();
            return null;
        }

        protected void VisitLabel(LabelSymbol label, BoundStatement node)
        {
            ResolveBranches(label, node);
            TLocalState other = LabelState(label);
            Join(ref State, ref other);
            _labels[label] = State.Clone();
            _labelsSeen.Add(node);
        }

        protected virtual void VisitLabel(BoundLabeledStatement node)
        {
            VisitLabel(node.Label, node);
        }

        public override BoundNode VisitLabelStatement(BoundLabelStatement node)
        {
            VisitLabel(node.Label, node);
            return null;
        }

        public override BoundNode VisitLabeledStatement(BoundLabeledStatement node)
        {
            VisitLabel(node);
            VisitStatement(node.Body);
            return null;
        }

        public override BoundNode VisitLockStatement(BoundLockStatement node)
        {
            VisitRvalue(node.Argument);
            VisitStatement(node.Body);
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

        public override BoundNode VisitUsingStatement(BoundUsingStatement node)
        {
            if (node.ExpressionOpt != null)
            {
                VisitRvalue(node.ExpressionOpt);
            }
            if (node.DeclarationsOpt != null)
            {
                VisitStatement(node.DeclarationsOpt);
            }
            VisitStatement(node.Body);
            if (AwaitUsingAndForeachAddsPendingBranch && node.AwaitOpt != null)
            {
                PendingBranches.Add(new PendingBranch(node, State, null));
            }
            return null;
        }

        public override BoundNode VisitFixedStatement(BoundFixedStatement node)
        {
            VisitStatement(node.Declarations);
            VisitStatement(node.Body);
            return null;
        }

        public override BoundNode VisitFixedLocalCollectionInitializer(BoundFixedLocalCollectionInitializer node)
        {
            VisitRvalue(node.Expression);
            return null;
        }

        public override BoundNode VisitThrowStatement(BoundThrowStatement node)
        {
            BoundExpression expressionOpt = node.ExpressionOpt;
            VisitRvalue(expressionOpt);
            SetUnreachable();
            return null;
        }

        public override BoundNode VisitYieldBreakStatement(BoundYieldBreakStatement node)
        {
            PendingBranches.Add(new PendingBranch(node, State, null));
            SetUnreachable();
            return null;
        }

        public override BoundNode VisitYieldReturnStatement(BoundYieldReturnStatement node)
        {
            VisitRvalue(node.Expression);
            PendingBranches.Add(new PendingBranch(node, State, null));
            return null;
        }

        public override BoundNode VisitDefaultLiteral(BoundDefaultLiteral node)
        {
            return null;
        }

        public override BoundNode VisitDefaultExpression(BoundDefaultExpression node)
        {
            return null;
        }

        public override BoundNode VisitUnconvertedObjectCreationExpression(BoundUnconvertedObjectCreationExpression node)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override BoundNode VisitTypeOfOperator(BoundTypeOfOperator node)
        {
            VisitTypeExpression(node.SourceType);
            return null;
        }

        public override BoundNode VisitNameOfOperator(BoundNameOfOperator node)
        {
            TLocalState state = State;
            SetState(UnreachableState());
            Visit(node.Argument);
            SetState(state);
            return null;
        }

        public override BoundNode VisitAddressOfOperator(BoundAddressOfOperator node)
        {
            VisitAddressOfOperand(node.Operand, shouldReadOperand: false);
            return null;
        }

        protected void VisitAddressOfOperand(BoundExpression operand, bool shouldReadOperand)
        {
            if (shouldReadOperand)
            {
                VisitRvalue(operand);
            }
            else
            {
                VisitLvalue(operand);
            }
            WriteArgument(operand, RefKind.Out, null);
        }

        public override BoundNode VisitPointerIndirectionOperator(BoundPointerIndirectionOperator node)
        {
            VisitRvalue(node.Operand);
            return null;
        }

        public override BoundNode VisitPointerElementAccess(BoundPointerElementAccess node)
        {
            VisitRvalue(node.Expression);
            VisitRvalue(node.Index);
            return null;
        }

        public override BoundNode VisitSizeOfOperator(BoundSizeOfOperator node)
        {
            return null;
        }

        private BoundNode VisitStackAllocArrayCreationBase(BoundStackAllocArrayCreationBase node)
        {
            VisitRvalue(node.Count);
            if (node.InitializerOpt != null && !node.InitializerOpt!.Initializers.IsDefault)
            {
                ImmutableArray<BoundExpression>.Enumerator enumerator = node.InitializerOpt!.Initializers.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    BoundExpression current = enumerator.Current;
                    VisitRvalue(current);
                }
            }
            return null;
        }

        public override BoundNode VisitStackAllocArrayCreation(BoundStackAllocArrayCreation node)
        {
            return VisitStackAllocArrayCreationBase(node);
        }

        public override BoundNode VisitConvertedStackAllocExpression(BoundConvertedStackAllocExpression node)
        {
            return VisitStackAllocArrayCreationBase(node);
        }

        public override BoundNode VisitAnonymousObjectCreationExpression(BoundAnonymousObjectCreationExpression node)
        {
            VisitArguments(node.Arguments, default(ImmutableArray<RefKind>), node.Constructor);
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
                PendingBranches.Add(new PendingBranch(node, StateWhenTrue, node.Label));
                SetState(StateWhenFalse);
            }
            else
            {
                PendingBranches.Add(new PendingBranch(node, StateWhenFalse, node.Label));
                SetState(StateWhenTrue);
            }
            return null;
        }

        public override BoundNode VisitObjectInitializerExpression(BoundObjectInitializerExpression node)
        {
            return VisitObjectOrCollectionInitializerExpression(node.Initializers);
        }

        public override BoundNode VisitCollectionInitializerExpression(BoundCollectionInitializerExpression node)
        {
            return VisitObjectOrCollectionInitializerExpression(node.Initializers);
        }

        private BoundNode VisitObjectOrCollectionInitializerExpression(ImmutableArray<BoundExpression> initializers)
        {
            ImmutableArray<BoundExpression>.Enumerator enumerator = initializers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundExpression current = enumerator.Current;
                VisitRvalue(current);
            }
            return null;
        }

        public override BoundNode VisitObjectInitializerMember(BoundObjectInitializerMember node)
        {
            if (!node.Arguments.IsDefaultOrEmpty)
            {
                MethodSymbol method = null;
                Symbol? memberSymbol = node.MemberSymbol;
                if ((object)memberSymbol != null && memberSymbol!.Kind == SymbolKind.Property)
                {
                    method = GetReadMethod((PropertySymbol)node.MemberSymbol);
                }
                VisitArguments(node.Arguments, node.ArgumentRefKindsOpt, method);
            }
            return null;
        }

        public override BoundNode VisitDynamicObjectInitializerMember(BoundDynamicObjectInitializerMember node)
        {
            return null;
        }

        public override BoundNode VisitCollectionElementInitializer(BoundCollectionElementInitializer node)
        {
            if (node.AddMethod.CallsAreOmitted(node.SyntaxTree))
            {
                TLocalState val = (val = State.Clone());
                SetUnreachable();
                VisitArguments(node.Arguments, default(ImmutableArray<RefKind>), node.AddMethod);
                State = val;
            }
            else
            {
                VisitArguments(node.Arguments, default(ImmutableArray<RefKind>), node.AddMethod);
            }
            return null;
        }

        public override BoundNode VisitDynamicCollectionElementInitializer(BoundDynamicCollectionElementInitializer node)
        {
            VisitArguments(node.Arguments, default(ImmutableArray<RefKind>), null);
            return null;
        }

        public override BoundNode VisitImplicitReceiver(BoundImplicitReceiver node)
        {
            return null;
        }

        public override BoundNode VisitFieldEqualsValue(BoundFieldEqualsValue node)
        {
            VisitRvalue(node.Value);
            return null;
        }

        public override BoundNode VisitPropertyEqualsValue(BoundPropertyEqualsValue node)
        {
            VisitRvalue(node.Value);
            return null;
        }

        public override BoundNode VisitParameterEqualsValue(BoundParameterEqualsValue node)
        {
            VisitRvalue(node.Value);
            return null;
        }

        public override BoundNode VisitDeconstructValuePlaceholder(BoundDeconstructValuePlaceholder node)
        {
            return null;
        }

        public override BoundNode VisitObjectOrCollectionValuePlaceholder(BoundObjectOrCollectionValuePlaceholder node)
        {
            return null;
        }

        public override BoundNode VisitAwaitableValuePlaceholder(BoundAwaitableValuePlaceholder node)
        {
            return null;
        }

        public sealed override BoundNode VisitOutVariablePendingInference(OutVariablePendingInference node)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public sealed override BoundNode VisitDeconstructionVariablePendingInference(DeconstructionVariablePendingInference node)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override BoundNode VisitDiscardExpression(BoundDiscardExpression node)
        {
            return null;
        }

        private static MethodSymbol GetReadMethod(PropertySymbol property)
        {
            return property.GetOwnOrInheritedGetMethod() ?? property.SetMethod;
        }

        private static MethodSymbol GetWriteMethod(PropertySymbol property)
        {
            return property.GetOwnOrInheritedSetMethod() ?? property.GetMethod;
        }

        public override BoundNode VisitConstructorMethodBody(BoundConstructorMethodBody node)
        {
            Visit(node.Initializer);
            VisitMethodBodies(node.BlockBody, node.ExpressionBody);
            return null;
        }

        public override BoundNode VisitNonConstructorMethodBody(BoundNonConstructorMethodBody node)
        {
            VisitMethodBodies(node.BlockBody, node.ExpressionBody);
            return null;
        }

        public override BoundNode VisitNullCoalescingAssignmentOperator(BoundNullCoalescingAssignmentOperator node)
        {
            TLocalState other;
            if (RegularPropertyAccess(node.LeftOperand))
            {
                BoundPropertyAccess boundPropertyAccess = (BoundPropertyAccess)node.LeftOperand;
                PropertySymbol propertySymbol = boundPropertyAccess.PropertySymbol;
                if (propertySymbol.RefKind == RefKind.None)
                {
                    MethodSymbol ownOrInheritedGetMethod = propertySymbol.GetOwnOrInheritedGetMethod();
                    VisitReceiverBeforeCall(boundPropertyAccess.ReceiverOpt, ownOrInheritedGetMethod);
                    VisitReceiverAfterCall(boundPropertyAccess.ReceiverOpt, ownOrInheritedGetMethod);
                    TLocalState state = State.Clone();
                    AdjustStateForNullCoalescingAssignmentNonNullCase(node);
                    other = State.Clone();
                    SetState(state);
                    VisitAssignmentOfNullCoalescingAssignment(node, boundPropertyAccess);
                    goto IL_00d1;
                }
            }
            VisitRvalue(node.LeftOperand, isKnownToBeAnLvalue: true);
            TLocalState state2 = State.Clone();
            AdjustStateForNullCoalescingAssignmentNonNullCase(node);
            other = State.Clone();
            SetState(state2);
            VisitAssignmentOfNullCoalescingAssignment(node, null);
            goto IL_00d1;
        IL_00d1:
            Join(ref State, ref other);
            return null;
        }

        public override BoundNode VisitReadOnlySpanFromArray(BoundReadOnlySpanFromArray node)
        {
            VisitRvalue(node.Operand);
            return null;
        }

        public override BoundNode VisitFunctionPointerInvocation(BoundFunctionPointerInvocation node)
        {
            Visit(node.InvokedExpression);
            VisitArguments(node.Arguments, node.ArgumentRefKindsOpt, node.FunctionPointer.Signature);
            return null;
        }

        public override BoundNode VisitUnconvertedAddressOfOperator(BoundUnconvertedAddressOfOperator node)
        {
            Visit(node.Operand);
            return null;
        }

        protected virtual void VisitAssignmentOfNullCoalescingAssignment(BoundNullCoalescingAssignmentOperator node, BoundPropertyAccess propertyAccessOpt)
        {
            VisitRvalue(node.RightOperand);
            if (propertyAccessOpt != null)
            {
                MethodSymbol ownOrInheritedSetMethod = propertyAccessOpt.PropertySymbol.GetOwnOrInheritedSetMethod();
                PropertySetter(node, propertyAccessOpt.ReceiverOpt, ownOrInheritedSetMethod);
            }
        }

        public override BoundNode VisitSavePreviousSequencePoint(BoundSavePreviousSequencePoint node)
        {
            return null;
        }

        public override BoundNode VisitRestorePreviousSequencePoint(BoundRestorePreviousSequencePoint node)
        {
            return null;
        }

        public override BoundNode VisitStepThroughSequencePoint(BoundStepThroughSequencePoint node)
        {
            return null;
        }

        protected virtual void AdjustStateForNullCoalescingAssignmentNonNullCase(BoundNullCoalescingAssignmentOperator node)
        {
        }

        private void VisitMethodBodies(BoundBlock blockBody, BoundBlock expressionBody)
        {
            if (blockBody == null)
            {
                Visit(expressionBody);
                return;
            }
            if (expressionBody == null)
            {
                Visit(blockBody);
                return;
            }
            TLocalState state = State.Clone();
            Visit(blockBody);
            TLocalState other = State;
            SetState(state);
            Visit(expressionBody);
            Join(ref State, ref other);
        }

        protected abstract TLocalState TopState();

        protected abstract TLocalState UnreachableState();

        protected virtual TLocalState ReachableBottomState()
        {
            return default(TLocalState);
        }

        protected abstract bool Join(ref TLocalState self, ref TLocalState other);

        protected abstract bool Meet(ref TLocalState self, ref TLocalState other);

        protected abstract TLocalFunctionState CreateLocalFunctionState(LocalFunctionSymbol symbol);

        protected TLocalFunctionState GetOrCreateLocalFuncUsages(LocalFunctionSymbol localFunc)
        {
            if (_localFuncVarUsages == null)
            {
                _localFuncVarUsages = new SmallDictionary<LocalFunctionSymbol, TLocalFunctionState>();
            }
            if (!_localFuncVarUsages!.TryGetValue(localFunc, out var value))
            {
                value = CreateLocalFunctionState(localFunc);
                _localFuncVarUsages![localFunc] = value;
            }
            return value;
        }

        public override BoundNode? VisitLocalFunctionStatement(BoundLocalFunctionStatement localFunc)
        {
            if (localFunc.Symbol.IsExtern)
            {
                return null;
            }
            Symbol currentSymbol = CurrentSymbol;
            LocalFunctionSymbol localFunctionSymbol = (LocalFunctionSymbol)(CurrentSymbol = localFunc.Symbol);
            SavedPending oldPending = SavePending();
            TLocalState state = State;
            State = TopState();
            Optional<TLocalState> nonMonotonicState = NonMonotonicState;
            if (_nonMonotonicTransfer)
            {
                NonMonotonicState = ReachableBottomState();
            }
            if (!localFunc.WasCompilerGenerated)
            {
                EnterParameters(localFunctionSymbol.Parameters);
            }
            TLocalFunctionState orCreateLocalFuncUsages = GetOrCreateLocalFuncUsages(localFunctionSymbol);
            TLocalFunctionState savedState = LocalFunctionStart(orCreateLocalFuncUsages);
            SavedPending oldPending2 = SavePending();
            if (localFunctionSymbol.IsIterator)
            {
                PendingBranches.Add(new PendingBranch(null, State, null));
            }
            VisitAlways(localFunc.Body);
            RestorePending(oldPending2);
            ImmutableArray<PendingBranch> immutableArray = RemoveReturns();
            RestorePending(oldPending);
            Location location = null;
            if (!localFunctionSymbol.Locations.IsDefaultOrEmpty)
            {
                location = localFunctionSymbol.Locations[0];
            }
            LeaveParameters(localFunctionSymbol.Parameters, localFunc.Syntax, location);
            TLocalState stateAtReturn = State;
            ImmutableArray<PendingBranch>.Enumerator enumerator = immutableArray.GetEnumerator();
            while (enumerator.MoveNext())
            {
                PendingBranch current = enumerator.Current;
                State = current.State;
                BoundNode branch = current.Branch;
                LeaveParameters(localFunctionSymbol.Parameters, branch?.Syntax, (branch != null && !branch.WasCompilerGenerated) ? null : location);
                Join(ref stateAtReturn, ref State);
            }
            if (RecordStateChange(savedState, orCreateLocalFuncUsages, ref stateAtReturn) && orCreateLocalFuncUsages.Visited)
            {
                stateChangedAfterUse = true;
                orCreateLocalFuncUsages.Visited = false;
            }
            State = state;
            NonMonotonicState = nonMonotonicState;
            CurrentSymbol = currentSymbol;
            return null;
        }

        private bool RecordStateChange(TLocalFunctionState savedState, TLocalFunctionState currentState, ref TLocalState stateAtReturn)
        {
            bool flag = LocalFunctionEnd(savedState, currentState, ref stateAtReturn);
            flag |= Join(ref currentState.StateFromTop, ref stateAtReturn);
            if (NonMonotonicState.HasValue)
            {
                TLocalState self = NonMonotonicState.Value;
                Meet(ref self, ref stateAtReturn);
                flag |= Join(ref currentState.StateFromBottom, ref self);
            }
            return flag;
        }

        protected virtual TLocalFunctionState LocalFunctionStart(TLocalFunctionState state)
        {
            return state;
        }

        protected virtual bool LocalFunctionEnd(TLocalFunctionState savedState, TLocalFunctionState currentState, ref TLocalState stateAtReturn)
        {
            return false;
        }

        public override BoundNode VisitSwitchStatement(BoundSwitchStatement node)
        {
            TLocalState self = VisitSwitchStatementDispatch(node);
            ImmutableArray<BoundSwitchSection> switchSections = node.SwitchSections;
            int num = switchSections.Length - 1;
            for (int i = 0; i <= num; i++)
            {
                VisitSwitchSection(switchSections[i], i == num);
                Join(ref self, ref State);
            }
            ResolveBreaks(self, node.BreakLabel);
            return null;
        }

        protected virtual TLocalState VisitSwitchStatementDispatch(BoundSwitchStatement node)
        {
            VisitRvalue(node.Expression);
            TLocalState other = State.Clone();
            ImmutableHashSet<LabelSymbol> reachableLabels = node.DecisionDag.ReachableLabels;
            ImmutableArray<BoundSwitchSection>.Enumerator enumerator = node.SwitchSections.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ImmutableArray<BoundSwitchLabel>.Enumerator enumerator2 = enumerator.Current.SwitchLabels.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    BoundSwitchLabel current = enumerator2.Current;
                    if (reachableLabels.Contains(current.Label) || current.HasErrors || (current == node.DefaultLabel && node.Expression.ConstantValue == null && IsTraditionalSwitch(node)))
                    {
                        SetState(other.Clone());
                    }
                    else
                    {
                        SetUnreachable();
                    }
                    VisitPattern(current.Pattern);
                    SetState(StateWhenTrue);
                    if (current.WhenClause != null)
                    {
                        VisitCondition(current.WhenClause);
                        SetState(StateWhenTrue);
                    }
                    PendingBranches.Add(new PendingBranch(current, State, current.Label));
                }
            }
            TLocalState self = UnreachableState();
            if (node.DecisionDag.ReachableLabels.Contains(node.BreakLabel) || (node.DefaultLabel == null && node.Expression.ConstantValue == null && IsTraditionalSwitch(node)))
            {
                Join(ref self, ref other);
            }
            return self;
        }

        private bool IsTraditionalSwitch(BoundSwitchStatement node)
        {
            if (compilation.LanguageVersion >= MessageID.IDS_FeatureRecursivePatterns.RequiredVersion())
            {
                return false;
            }
            if (!node.Expression.Type.IsValidV6SwitchGoverningType())
            {
                return false;
            }
            SyntaxList<SwitchSectionSyntax>.Enumerator enumerator = ((SwitchStatementSyntax)node.Syntax).Sections.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SyntaxList<SwitchLabelSyntax>.Enumerator enumerator2 = enumerator.Current.Labels.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    if (enumerator2.Current.Kind() == SyntaxKind.CasePatternSwitchLabel)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        protected virtual void VisitSwitchSection(BoundSwitchSection node, bool isLastSection)
        {
            SetState(UnreachableState());
            ImmutableArray<BoundSwitchLabel>.Enumerator enumerator = node.SwitchLabels.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundSwitchLabel current = enumerator.Current;
                VisitLabel(current.Label, node);
            }
            VisitStatementList(node);
        }

        public override BoundNode VisitSwitchDispatch(BoundSwitchDispatch node)
        {
            VisitRvalue(node.Expression);
            TLocalState state = State.Clone();
            PendingBranches.Add(new PendingBranch(node, state, node.DefaultLabel));
            ImmutableArray<(ConstantValue, LabelSymbol)>.Enumerator enumerator = node.Cases.GetEnumerator();
            while (enumerator.MoveNext())
            {
                LabelSymbol item = enumerator.Current.Item2;
                PendingBranches.Add(new PendingBranch(node, state, item));
            }
            SetUnreachable();
            return null;
        }

        public override BoundNode VisitConvertedSwitchExpression(BoundConvertedSwitchExpression node)
        {
            return VisitSwitchExpression(node);
        }

        public override BoundNode VisitUnconvertedSwitchExpression(BoundUnconvertedSwitchExpression node)
        {
            return VisitSwitchExpression(node);
        }

        private BoundNode VisitSwitchExpression(BoundSwitchExpression node)
        {
            VisitRvalue(node.Expression);
            TLocalState state = State;
            TLocalState self = UnreachableState();
            ImmutableHashSet<LabelSymbol> reachableLabels = node.DecisionDag.ReachableLabels;
            ImmutableArray<BoundSwitchExpressionArm>.Enumerator enumerator = node.SwitchArms.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundSwitchExpressionArm current = enumerator.Current;
                SetState(state.Clone());
                VisitPattern(current.Pattern);
                SetState(StateWhenTrue);
                if (!reachableLabels.Contains(current.Label) || current.Pattern.HasErrors)
                {
                    SetUnreachable();
                }
                if (current.WhenClause != null)
                {
                    VisitCondition(current.WhenClause);
                    SetState(StateWhenTrue);
                }
                VisitRvalue(current.Value);
                Join(ref self, ref State);
            }
            SetState(self);
            return node;
        }
    }
}
