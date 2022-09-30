using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class VisualBasicDataFlowAnalysis : DataFlowAnalysis
	{
		private readonly RegionAnalysisContext _context;

		private ImmutableArray<ISymbol> _variablesDeclared;

		private HashSet<Symbol> _unassignedVariables;

		private ImmutableArray<ISymbol> _dataFlowsIn;

		private ImmutableArray<ISymbol> _definitelyAssignedOnEntry;

		private ImmutableArray<ISymbol> _definitelyAssignedOnExit;

		private ImmutableArray<ISymbol> _dataFlowsOut;

		private ImmutableArray<ISymbol> _alwaysAssigned;

		private ImmutableArray<ISymbol> _readInside;

		private ImmutableArray<ISymbol> _writtenInside;

		private ImmutableArray<ISymbol> _readOutside;

		private ImmutableArray<ISymbol> _writtenOutside;

		private ImmutableArray<ISymbol> _captured;

		private ImmutableArray<ISymbol> _capturedInside;

		private ImmutableArray<ISymbol> _capturedOutside;

		private bool? _succeeded;

		private bool _invalidRegionDetected;

		public override ImmutableArray<ISymbol> VariablesDeclared
		{
			get
			{
				if (_variablesDeclared.IsDefault)
				{
					ImmutableArray<ISymbol> value = (_context.Failed ? ImmutableArray<ISymbol>.Empty : Normalize(VariablesDeclaredWalker.Analyze(_context.AnalysisInfo, _context.RegionInfo)));
					ImmutableInterlocked.InterlockedCompareExchange(ref _variablesDeclared, value, default(ImmutableArray<ISymbol>));
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
					HashSet<Symbol> value = (_context.Failed ? new HashSet<Symbol>() : UnassignedVariablesWalker.Analyze(_context.AnalysisInfo));
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
					ImmutableArray<ISymbol> value = (_context.Failed ? ImmutableArray<ISymbol>.Empty : Normalize(DataFlowsInWalker.Analyze(_context.AnalysisInfo, _context.RegionInfo, UnassignedVariables, ref _succeeded, ref _invalidRegionDetected)));
					ImmutableInterlocked.InterlockedCompareExchange(ref _dataFlowsIn, value, default(ImmutableArray<ISymbol>));
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
					ImmutableArray<ISymbol> value = (_context.Failed ? ImmutableArray<ISymbol>.Empty : Normalize(DataFlowsOutWalker.Analyze(_context.AnalysisInfo, _context.RegionInfo, UnassignedVariables, _dataFlowsIn)));
					ImmutableInterlocked.InterlockedCompareExchange(ref _dataFlowsOut, value, default(ImmutableArray<ISymbol>));
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
					ImmutableArray<ISymbol> value = (_context.Failed ? ImmutableArray<ISymbol>.Empty : Normalize(AlwaysAssignedWalker.Analyze(_context.AnalysisInfo, _context.RegionInfo)));
					ImmutableInterlocked.InterlockedCompareExchange(ref _alwaysAssigned, value, default(ImmutableArray<ISymbol>));
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

		internal bool InvalidRegionDetectedInternal
		{
			get
			{
				if (!Succeeded)
				{
					return _invalidRegionDetected;
				}
				return false;
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

		public override ImmutableArray<ISymbol> UnsafeAddressTaken => ImmutableArray<ISymbol>.Empty;

		public override ImmutableArray<IMethodSymbol> UsedLocalFunctions => ImmutableArray<IMethodSymbol>.Empty;

		internal VisualBasicDataFlowAnalysis(RegionAnalysisContext _context)
		{
			this._context = _context;
		}

		private (ImmutableArray<ISymbol> onEntry, ImmutableArray<ISymbol> onExit) ComputeDefinitelyAssignedValues()
		{
			if (_definitelyAssignedOnExit.IsDefault)
			{
				ImmutableArray<ISymbol> value = ImmutableArray<ISymbol>.Empty;
				ImmutableArray<ISymbol> value2 = ImmutableArray<ISymbol>.Empty;
				_ = DataFlowsIn;
				if (!_context.Failed)
				{
					(HashSet<Symbol>, HashSet<Symbol>) tuple = DefinitelyAssignedWalker.Analyze(_context.AnalysisInfo, _context.RegionInfo);
					value = Normalize(tuple.Item1);
					value2 = Normalize(tuple.Item2);
				}
				ImmutableInterlocked.InterlockedInitialize(ref _definitelyAssignedOnEntry, value);
				ImmutableInterlocked.InterlockedInitialize(ref _definitelyAssignedOnExit, value2);
			}
			return (_definitelyAssignedOnEntry, _definitelyAssignedOnExit);
		}

		private void AnalyzeReadWrite()
		{
			IEnumerable<Symbol> readInside = null;
			IEnumerable<Symbol> writtenInside = null;
			IEnumerable<Symbol> readOutside = null;
			IEnumerable<Symbol> writtenOutside = null;
			IEnumerable<Symbol> captured = null;
			IEnumerable<Symbol> capturedInside = null;
			IEnumerable<Symbol> capturedOutside = null;
			if (!Succeeded)
			{
				readInside = Enumerable.Empty<Symbol>();
				writtenInside = readInside;
				readOutside = readInside;
				writtenOutside = readInside;
				captured = readInside;
			}
			else
			{
				ReadWriteWalker.Analyze(_context.AnalysisInfo, _context.RegionInfo, ref readInside, ref writtenInside, ref readOutside, ref writtenOutside, ref captured, ref capturedInside, ref capturedOutside);
			}
			ImmutableInterlocked.InterlockedCompareExchange(ref _readInside, Normalize(readInside), default(ImmutableArray<ISymbol>));
			ImmutableInterlocked.InterlockedCompareExchange(ref _writtenInside, Normalize(writtenInside), default(ImmutableArray<ISymbol>));
			ImmutableInterlocked.InterlockedCompareExchange(ref _readOutside, Normalize(readOutside), default(ImmutableArray<ISymbol>));
			ImmutableInterlocked.InterlockedCompareExchange(ref _writtenOutside, Normalize(writtenOutside), default(ImmutableArray<ISymbol>));
			ImmutableInterlocked.InterlockedCompareExchange(ref _captured, Normalize(captured), default(ImmutableArray<ISymbol>));
			ImmutableInterlocked.InterlockedCompareExchange(ref _capturedInside, Normalize(capturedInside), default(ImmutableArray<ISymbol>));
			ImmutableInterlocked.InterlockedCompareExchange(ref _capturedOutside, Normalize(capturedOutside), default(ImmutableArray<ISymbol>));
		}

		internal ImmutableArray<ISymbol> Normalize(IEnumerable<Symbol> data)
		{
			ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
			instance.AddRange(data.Where((Symbol s) => s.CanBeReferencedByName));
			instance.Sort(LexicalOrderSymbolComparer.Instance);
			return instance.ToImmutableAndFree().As<ISymbol>();
		}
	}
}
