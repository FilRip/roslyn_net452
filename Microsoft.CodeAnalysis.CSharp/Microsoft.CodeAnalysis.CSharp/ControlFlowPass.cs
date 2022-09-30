using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal class ControlFlowPass : AbstractFlowPass<ControlFlowPass.LocalState, ControlFlowPass.LocalFunctionState>
    {
        internal struct LocalState : ILocalState
        {
            internal bool Alive;

            internal bool Reported;

            public bool Reachable => Alive;

            internal LocalState(bool live, bool reported)
            {
                Alive = live;
                Reported = reported;
            }

            public LocalState Clone()
            {
                return this;
            }
        }

        internal sealed class LocalFunctionState : AbstractLocalFunctionState
        {
            public LocalFunctionState(LocalState unreachableState)
                : base(unreachableState.Clone(), unreachableState.Clone())
            {
            }
        }

        private readonly PooledDictionary<LabelSymbol, BoundBlock> _labelsDefined = PooledDictionary<LabelSymbol, BoundBlock>.GetInstance();

        private readonly PooledHashSet<LabelSymbol> _labelsUsed = PooledHashSet<LabelSymbol>.GetInstance();

        protected bool _convertInsufficientExecutionStackExceptionToCancelledByStackGuardException;

        private readonly ArrayBuilder<(LocalSymbol symbol, BoundBlock block)> _usingDeclarations = ArrayBuilder<(LocalSymbol, BoundBlock)>.GetInstance();

        private BoundBlock _currentBlock;

        public sealed override bool AwaitUsingAndForeachAddsPendingBranch => false;

        protected override void Free()
        {
            _labelsDefined.Free();
            _labelsUsed.Free();
            _usingDeclarations.Free();
            base.Free();
        }

        internal ControlFlowPass(CSharpCompilation compilation, Symbol member, BoundNode node)
            : base(compilation, member, node, null, null, trackRegions: false, nonMonotonicTransferFunction: false)
        {
        }

        internal ControlFlowPass(CSharpCompilation compilation, Symbol member, BoundNode node, BoundNode firstInRegion, BoundNode lastInRegion)
            : base(compilation, member, node, firstInRegion, lastInRegion, trackRegions: false, nonMonotonicTransferFunction: false)
        {
        }

        protected override LocalFunctionState CreateLocalFunctionState(LocalFunctionSymbol symbol)
        {
            return new LocalFunctionState(UnreachableState());
        }

        protected override bool Meet(ref LocalState self, ref LocalState other)
        {
            LocalState localState = self;
            self.Alive &= other.Alive;
            self.Reported &= other.Reported;
            return self.Alive != localState.Alive;
        }

        protected override bool Join(ref LocalState self, ref LocalState other)
        {
            LocalState localState = self;
            self.Alive |= other.Alive;
            self.Reported &= other.Reported;
            return self.Alive != localState.Alive;
        }

        protected override string Dump(LocalState state)
        {
            return "[alive: " + state.Alive + "; reported: " + state.Reported + "]";
        }

        protected override LocalState TopState()
        {
            return new LocalState(live: true, reported: false);
        }

        protected override LocalState UnreachableState()
        {
            return new LocalState(live: false, State.Reported);
        }

        protected override LocalState LabelState(LabelSymbol label)
        {
            LocalState result = base.LabelState(label);
            result.Reported = false;
            return result;
        }

        public override BoundNode Visit(BoundNode node)
        {
            if (!(node is BoundExpression))
            {
                return base.Visit(node);
            }
            return null;
        }

        protected override ImmutableArray<PendingBranch> Scan(ref bool badRegion)
        {
            base.Diagnostics.Clear();
            ImmutableArray<PendingBranch> result = base.Scan(ref badRegion);
            foreach (LabelSymbol key in _labelsDefined.Keys)
            {
                if (!_labelsUsed.Contains(key))
                {
                    base.Diagnostics.Add(ErrorCode.WRN_UnreferencedLabel, key.Locations[0]);
                }
            }
            return result;
        }

        public static bool Analyze(CSharpCompilation compilation, Symbol member, BoundBlock block, DiagnosticBag diagnostics)
        {
            ControlFlowPass controlFlowPass = new ControlFlowPass(compilation, member, block);
            if (diagnostics != null)
            {
                controlFlowPass._convertInsufficientExecutionStackExceptionToCancelledByStackGuardException = true;
            }
            try
            {
                bool badRegion = false;
                return controlFlowPass.Analyze(ref badRegion, diagnostics);
            }
            catch (CancelledByStackGuardException ex) when (diagnostics != null)
            {
                ex.AddAnError(diagnostics);
                return true;
            }
            finally
            {
                controlFlowPass.Free();
            }
        }

        protected override bool ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException()
        {
            return _convertInsufficientExecutionStackExceptionToCancelledByStackGuardException;
        }

        protected bool Analyze(ref bool badRegion, DiagnosticBag diagnostics)
        {
            Analyze(ref badRegion);
            diagnostics?.AddRange(base.Diagnostics);
            return State.Alive;
        }

        protected override ImmutableArray<PendingBranch> RemoveReturns()
        {
            ImmutableArray<PendingBranch> result = base.RemoveReturns();
            ImmutableArray<PendingBranch>.Enumerator enumerator = result.GetEnumerator();
            while (enumerator.MoveNext())
            {
                PendingBranch current = enumerator.Current;
                if (current.Branch != null)
                {
                    switch (current.Branch.Kind)
                    {
                        case BoundKind.GotoStatement:
                            {
                                SyntaxToken token = current.Branch.Syntax.GetFirstToken();
                                SourceLocation location2 = new SourceLocation(in token);
                                base.Diagnostics.Add(ErrorCode.ERR_LabelNotFound, location2, ((BoundGotoStatement)current.Branch).Label.Name);
                                break;
                            }
                        case BoundKind.BreakStatement:
                        case BoundKind.ContinueStatement:
                            {
                                SyntaxToken token = current.Branch.Syntax.GetFirstToken();
                                SourceLocation location = new SourceLocation(in token);
                                base.Diagnostics.Add(ErrorCode.ERR_BadDelegateLeave, location);
                                break;
                            }
                    }
                }
            }
            return result;
        }

        protected override void VisitStatement(BoundStatement statement)
        {
            switch (statement.Kind)
            {
                case BoundKind.Block:
                case BoundKind.LocalFunctionStatement:
                case BoundKind.NoOpStatement:
                case BoundKind.ThrowStatement:
                case BoundKind.LabeledStatement:
                    base.VisitStatement(statement);
                    break;
                case BoundKind.StatementList:
                    VisitStatementList((BoundStatementList)statement);
                    break;
                default:
                    CheckReachable(statement);
                    base.VisitStatement(statement);
                    break;
            }
        }

        private void CheckReachable(BoundStatement statement)
        {
            if (!State.Alive && !State.Reported && !statement.WasCompilerGenerated && statement.Syntax.Span.Length != 0)
            {
                SyntaxToken token = statement.Syntax.GetFirstToken();
                base.Diagnostics.Add(ErrorCode.WRN_UnreachableCode, new SourceLocation(in token));
                State.Reported = true;
            }
        }

        protected override void VisitTryBlock(BoundStatement tryBlock, BoundTryStatement node, ref LocalState tryState)
        {
            if (node.CatchBlocks.IsEmpty)
            {
                base.VisitTryBlock(tryBlock, node, ref tryState);
                return;
            }
            SavedPending oldPending = SavePending();
            base.VisitTryBlock(tryBlock, node, ref tryState);
            RestorePending(oldPending);
        }

        protected override void VisitCatchBlock(BoundCatchBlock catchBlock, ref LocalState finallyState)
        {
            SavedPending oldPending = SavePending();
            base.VisitCatchBlock(catchBlock, ref finallyState);
            RestorePending(oldPending);
        }

        protected override void VisitFinallyBlock(BoundStatement finallyBlock, ref LocalState endState)
        {
            SavedPending oldPending = SavePending();
            SavedPending oldPending2 = SavePending();
            base.VisitFinallyBlock(finallyBlock, ref endState);
            RestorePending(oldPending2);
            ArrayBuilder<PendingBranch>.Enumerator enumerator = base.PendingBranches.GetEnumerator();
            while (enumerator.MoveNext())
            {
                PendingBranch current = enumerator.Current;
                if (current.Branch != null)
                {
                    SyntaxToken token = current.Branch.Syntax.GetFirstToken();
                    SourceLocation location = new SourceLocation(in token);
                    BoundKind kind = current.Branch.Kind;
                    if (kind - 77 > BoundKind.PropertyEqualsValue)
                    {
                        base.Diagnostics.Add(ErrorCode.ERR_BadFinallyLeave, location);
                    }
                }
            }
            RestorePending(oldPending);
        }

        protected override void VisitLabel(BoundLabeledStatement node)
        {
            _labelsDefined[node.Label] = _currentBlock;
            base.VisitLabel(node);
        }

        public override BoundNode VisitLabeledStatement(BoundLabeledStatement node)
        {
            VisitLabel(node);
            CheckReachable(node);
            VisitStatement(node.Body);
            return null;
        }

        public override BoundNode VisitGotoStatement(BoundGotoStatement node)
        {
            _labelsUsed.Add(node.Label);
            Location location = node.Syntax.Location;
            int start = location.SourceSpan.Start;
            int start2 = node.Label.Locations[0].SourceSpan.Start;
            ArrayBuilder<(LocalSymbol, BoundBlock)>.Enumerator enumerator = _usingDeclarations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                (LocalSymbol, BoundBlock) current = enumerator.Current;
                int start3 = current.Item1.Locations[0].SourceSpan.Start;
                if (start < start3 && start2 > start3)
                {
                    base.Diagnostics.Add(ErrorCode.ERR_GoToForwardJumpOverUsingVar, location);
                    break;
                }
                if (start > start3 && start2 < start3 && _labelsDefined[node.Label] == current.Item2)
                {
                    base.Diagnostics.Add(ErrorCode.ERR_GoToBackwardJumpOverUsingVar, location);
                    break;
                }
            }
            return base.VisitGotoStatement(node);
        }

        protected override void VisitSwitchSection(BoundSwitchSection node, bool isLastSection)
        {
            base.VisitSwitchSection(node, isLastSection);
            if (State.Alive)
            {
                SyntaxNode syntax = node.SwitchLabels.Last().Syntax;
                base.Diagnostics.Add(isLastSection ? ErrorCode.ERR_SwitchFallOut : ErrorCode.ERR_SwitchFallThrough, new SourceLocation(syntax), syntax.ToString());
            }
        }

        public override BoundNode VisitBlock(BoundBlock node)
        {
            BoundBlock currentBlock = _currentBlock;
            _currentBlock = node;
            int count = _usingDeclarations.Count;
            ImmutableArray<LocalSymbol>.Enumerator enumerator = node.Locals.GetEnumerator();
            while (enumerator.MoveNext())
            {
                LocalSymbol current = enumerator.Current;
                if (current.IsUsing)
                {
                    _usingDeclarations.Add((current, node));
                }
            }
            BoundNode result = base.VisitBlock(node);
            _usingDeclarations.Clip(count);
            _currentBlock = currentBlock;
            return result;
        }
    }
}
