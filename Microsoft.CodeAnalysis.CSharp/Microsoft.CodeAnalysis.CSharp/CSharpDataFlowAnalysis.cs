using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal class CSharpDataFlowAnalysis : DataFlowAnalysis
    {
        private readonly RegionAnalysisContext _context;

        private ImmutableArray<ISymbol> _variablesDeclared;

        private HashSet<Symbol> _unassignedVariables;

        private ImmutableArray<ISymbol> _dataFlowsIn;

        private ImmutableArray<ISymbol> _dataFlowsOut;

        private ImmutableArray<ISymbol> _definitelyAssignedOnEntry;

        private ImmutableArray<ISymbol> _definitelyAssignedOnExit;

        private ImmutableArray<ISymbol> _alwaysAssigned;

        private ImmutableArray<ISymbol> _readInside;

        private ImmutableArray<ISymbol> _writtenInside;

        private ImmutableArray<ISymbol> _readOutside;

        private ImmutableArray<ISymbol> _writtenOutside;

        private ImmutableArray<ISymbol> _captured;

        private ImmutableArray<IMethodSymbol> _usedLocalFunctions;

        private ImmutableArray<ISymbol> _capturedInside;

        private ImmutableArray<ISymbol> _capturedOutside;

        private ImmutableArray<ISymbol> _unsafeAddressTaken;

        private HashSet<PrefixUnaryExpressionSyntax> _unassignedVariableAddressOfSyntaxes;

        private bool? _succeeded;

        public override ImmutableArray<ISymbol> VariablesDeclared
        {
            get
            {
                if (_variablesDeclared.IsDefault)
                {
                    ImmutableArray<ISymbol> value = (Succeeded ? Normalize(VariablesDeclaredWalker.Analyze(_context.Compilation, _context.Member, _context.BoundNode, _context.FirstInRegion, _context.LastInRegion)) : ImmutableArray<ISymbol>.Empty);
                    ImmutableInterlocked.InterlockedInitialize(ref _variablesDeclared, value);
                }
                return _variablesDeclared;
            }
        }

        private HashSet<Symbol> UnassignedVariables
        {
            get
            {
                if (_unassignedVariables == null)
                {
                    HashSet<Symbol> value = (Succeeded ? UnassignedVariablesWalker.Analyze(_context.Compilation, _context.Member, _context.BoundNode) : new HashSet<Symbol>());
                    Interlocked.CompareExchange(ref _unassignedVariables, value, null);
                }
                return _unassignedVariables;
            }
        }

        public override ImmutableArray<ISymbol> DataFlowsIn
        {
            get
            {
                if (_dataFlowsIn.IsDefault)
                {
                    _succeeded = !_context.Failed;
                    ImmutableArray<ISymbol> value = (_context.Failed ? ImmutableArray<ISymbol>.Empty : Normalize(DataFlowsInWalker.Analyze(_context.Compilation, _context.Member, _context.BoundNode, _context.FirstInRegion, _context.LastInRegion, UnassignedVariables, UnassignedVariableAddressOfSyntaxes, out _succeeded)));
                    ImmutableInterlocked.InterlockedInitialize(ref _dataFlowsIn, value);
                }
                return _dataFlowsIn;
            }
        }

        public override ImmutableArray<ISymbol> DefinitelyAssignedOnEntry => ComputeDefinitelyAssignedValues().onEntry;

        public override ImmutableArray<ISymbol> DefinitelyAssignedOnExit => ComputeDefinitelyAssignedValues().onExit;

        public override ImmutableArray<ISymbol> DataFlowsOut
        {
            get
            {
                _ = DataFlowsIn;
                if (_dataFlowsOut.IsDefault)
                {
                    ImmutableArray<ISymbol> value = (Succeeded ? Normalize(DataFlowsOutWalker.Analyze(_context.Compilation, _context.Member, _context.BoundNode, _context.FirstInRegion, _context.LastInRegion, UnassignedVariables, _dataFlowsIn)) : ImmutableArray<ISymbol>.Empty);
                    ImmutableInterlocked.InterlockedInitialize(ref _dataFlowsOut, value);
                }
                return _dataFlowsOut;
            }
        }

        public override ImmutableArray<ISymbol> AlwaysAssigned
        {
            get
            {
                if (_alwaysAssigned.IsDefault)
                {
                    ImmutableArray<ISymbol> value = (Succeeded ? Normalize(AlwaysAssignedWalker.Analyze(_context.Compilation, _context.Member, _context.BoundNode, _context.FirstInRegion, _context.LastInRegion)) : ImmutableArray<ISymbol>.Empty);
                    ImmutableInterlocked.InterlockedInitialize(ref _alwaysAssigned, value);
                }
                return _alwaysAssigned;
            }
        }

        public override ImmutableArray<ISymbol> ReadInside
        {
            get
            {
                if (_readInside.IsDefault)
                {
                    AnalyzeReadWrite();
                }
                return _readInside;
            }
        }

        public override ImmutableArray<ISymbol> WrittenInside
        {
            get
            {
                if (_writtenInside.IsDefault)
                {
                    AnalyzeReadWrite();
                }
                return _writtenInside;
            }
        }

        public override ImmutableArray<ISymbol> ReadOutside
        {
            get
            {
                if (_readOutside.IsDefault)
                {
                    AnalyzeReadWrite();
                }
                return _readOutside;
            }
        }

        public override ImmutableArray<ISymbol> WrittenOutside
        {
            get
            {
                if (_writtenOutside.IsDefault)
                {
                    AnalyzeReadWrite();
                }
                return _writtenOutside;
            }
        }

        public override ImmutableArray<ISymbol> Captured
        {
            get
            {
                if (_captured.IsDefault)
                {
                    AnalyzeReadWrite();
                }
                return _captured;
            }
        }

        public override ImmutableArray<ISymbol> CapturedInside
        {
            get
            {
                if (_capturedInside.IsDefault)
                {
                    AnalyzeReadWrite();
                }
                return _capturedInside;
            }
        }

        public override ImmutableArray<ISymbol> CapturedOutside
        {
            get
            {
                if (_capturedOutside.IsDefault)
                {
                    AnalyzeReadWrite();
                }
                return _capturedOutside;
            }
        }

        public override ImmutableArray<ISymbol> UnsafeAddressTaken
        {
            get
            {
                if (_unsafeAddressTaken.IsDefault)
                {
                    AnalyzeReadWrite();
                }
                return _unsafeAddressTaken;
            }
        }

        public override ImmutableArray<IMethodSymbol> UsedLocalFunctions
        {
            get
            {
                if (_usedLocalFunctions.IsDefault)
                {
                    AnalyzeReadWrite();
                }
                return _usedLocalFunctions;
            }
        }

        private HashSet<PrefixUnaryExpressionSyntax> UnassignedVariableAddressOfSyntaxes
        {
            get
            {
                if (_unassignedVariableAddressOfSyntaxes == null)
                {
                    HashSet<PrefixUnaryExpressionSyntax> value = (Succeeded ? UnassignedAddressTakenVariablesWalker.Analyze(_context.Compilation, _context.Member, _context.BoundNode) : new HashSet<PrefixUnaryExpressionSyntax>());
                    Interlocked.CompareExchange(ref _unassignedVariableAddressOfSyntaxes, value, null);
                }
                return _unassignedVariableAddressOfSyntaxes;
            }
        }

        public sealed override bool Succeeded
        {
            get
            {
                if (!_succeeded.HasValue)
                {
                    _ = DataFlowsIn;
                }
                return _succeeded.Value;
            }
        }

        internal CSharpDataFlowAnalysis(RegionAnalysisContext context)
        {
            _context = context;
        }

        private (ImmutableArray<ISymbol> onEntry, ImmutableArray<ISymbol> onExit) ComputeDefinitelyAssignedValues()
        {
            if (_definitelyAssignedOnExit.IsDefault)
            {
                ImmutableArray<ISymbol> value = ImmutableArray<ISymbol>.Empty;
                ImmutableArray<ISymbol> value2 = ImmutableArray<ISymbol>.Empty;
                if (Succeeded)
                {
                    (HashSet<Symbol> entry, HashSet<Symbol> exit) tuple = DefinitelyAssignedWalker.Analyze(_context.Compilation, _context.Member, _context.BoundNode, _context.FirstInRegion, _context.LastInRegion);
                    HashSet<Symbol> item = tuple.entry;
                    HashSet<Symbol> item2 = tuple.exit;
                    value = Normalize(item);
                    value2 = Normalize(item2);
                }
                ImmutableInterlocked.InterlockedInitialize(ref _definitelyAssignedOnEntry, value);
                ImmutableInterlocked.InterlockedInitialize(ref _definitelyAssignedOnExit, value2);
            }
            return (_definitelyAssignedOnEntry, _definitelyAssignedOnExit);
        }

        private void AnalyzeReadWrite()
        {
            IEnumerable<Symbol> readInside;
            IEnumerable<Symbol> writtenInside;
            IEnumerable<Symbol> readOutside;
            IEnumerable<Symbol> writtenOutside;
            IEnumerable<Symbol> captured;
            IEnumerable<Symbol> unsafeAddressTaken;
            IEnumerable<Symbol> capturedInside;
            IEnumerable<Symbol> capturedOutside;
            IEnumerable<MethodSymbol> usedLocalFunctions;
            if (Succeeded)
            {
                ReadWriteWalker.Analyze(_context.Compilation, _context.Member, _context.BoundNode, _context.FirstInRegion, _context.LastInRegion, UnassignedVariableAddressOfSyntaxes, out readInside, out writtenInside, out readOutside, out writtenOutside, out captured, out unsafeAddressTaken, out capturedInside, out capturedOutside, out usedLocalFunctions);
            }
            else
            {
                readInside = (writtenInside = (readOutside = (writtenOutside = (captured = (unsafeAddressTaken = (capturedInside = (capturedOutside = Enumerable.Empty<Symbol>())))))));
                usedLocalFunctions = Enumerable.Empty<MethodSymbol>();
            }
            ImmutableInterlocked.InterlockedInitialize(ref _readInside, Normalize(readInside));
            ImmutableInterlocked.InterlockedInitialize(ref _writtenInside, Normalize(writtenInside));
            ImmutableInterlocked.InterlockedInitialize(ref _readOutside, Normalize(readOutside));
            ImmutableInterlocked.InterlockedInitialize(ref _writtenOutside, Normalize(writtenOutside));
            ImmutableInterlocked.InterlockedInitialize(ref _captured, Normalize(captured));
            ImmutableInterlocked.InterlockedInitialize(ref _capturedInside, Normalize(capturedInside));
            ImmutableInterlocked.InterlockedInitialize(ref _capturedOutside, Normalize(capturedOutside));
            ImmutableInterlocked.InterlockedInitialize(ref _unsafeAddressTaken, Normalize(unsafeAddressTaken));
            ImmutableInterlocked.InterlockedInitialize(ref _usedLocalFunctions, Normalize(usedLocalFunctions));
        }

        private static ImmutableArray<ISymbol> Normalize(IEnumerable<Symbol> data)
        {
            return ImmutableArray.CreateRange(data.Where((Symbol s) => s.CanBeReferencedByName).OrderBy((Symbol s) => s, LexicalOrderSymbolComparer.Instance).GetPublicSymbols());
        }

        private static ImmutableArray<IMethodSymbol> Normalize(IEnumerable<MethodSymbol> data)
        {
            return ImmutableArray.CreateRange(from p in data.Where((MethodSymbol s) => s.CanBeReferencedByName).OrderBy((MethodSymbol s) => s, LexicalOrderSymbolComparer.Instance)
                                              select p.GetPublicSymbol());
        }
    }
}
