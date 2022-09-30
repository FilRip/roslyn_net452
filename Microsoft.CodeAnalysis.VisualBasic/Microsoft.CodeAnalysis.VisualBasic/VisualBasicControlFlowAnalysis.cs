using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class VisualBasicControlFlowAnalysis : ControlFlowAnalysis
	{
		private readonly RegionAnalysisContext _context;

		private ImmutableArray<SyntaxNode> _entryPoints;

		private ImmutableArray<SyntaxNode> _exitPoints;

		private object _regionStartPointIsReachable;

		private object _regionEndPointIsReachable;

		private ImmutableArray<SyntaxNode> _returnStatements;

		private bool? _succeeded;

		public override ImmutableArray<SyntaxNode> EntryPoints
		{
			get
			{
				if (_entryPoints.IsDefault)
				{
					_succeeded = !_context.Failed;
					ImmutableArray<SyntaxNode> value = (_context.Failed ? ImmutableArray<SyntaxNode>.Empty : ((IEnumerable<SyntaxNode>)EntryPointsWalker.Analyze(_context.AnalysisInfo, _context.RegionInfo, ref _succeeded)).ToImmutableArray());
					ImmutableInterlocked.InterlockedCompareExchange(ref _entryPoints, value, default(ImmutableArray<SyntaxNode>));
				}
				return _entryPoints;
			}
		}

		public override ImmutableArray<SyntaxNode> ExitPoints
		{
			get
			{
				if (_exitPoints.IsDefault)
				{
					ImmutableArray<SyntaxNode> value = (_context.Failed ? ImmutableArray<SyntaxNode>.Empty : ((IEnumerable<SyntaxNode>)ExitPointsWalker.Analyze(_context.AnalysisInfo, _context.RegionInfo)).ToImmutableArray());
					ImmutableInterlocked.InterlockedCompareExchange(ref _exitPoints, value, default(ImmutableArray<SyntaxNode>));
				}
				return _exitPoints;
			}
		}

		public sealed override bool EndPointIsReachable
		{
			get
			{
				if (_regionStartPointIsReachable == null)
				{
					ComputeReachability();
				}
				return (bool)_regionEndPointIsReachable;
			}
		}

		public sealed override bool StartPointIsReachable
		{
			get
			{
				if (_regionStartPointIsReachable == null)
				{
					ComputeReachability();
				}
				return (bool)_regionStartPointIsReachable;
			}
		}

		public override ImmutableArray<SyntaxNode> ReturnStatements => ExitPoints.WhereAsArray((SyntaxNode s) => Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(s, SyntaxKind.ReturnStatement) | Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(s, SyntaxKind.ExitSubStatement) | Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(s, SyntaxKind.ExitFunctionStatement) | Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(s, SyntaxKind.ExitOperatorStatement) | Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(s, SyntaxKind.ExitPropertyStatement));

		public sealed override bool Succeeded
		{
			get
			{
				if (!_succeeded.HasValue)
				{
					_ = EntryPoints;
				}
				return _succeeded.Value;
			}
		}

		internal VisualBasicControlFlowAnalysis(RegionAnalysisContext _context)
		{
			this._context = _context;
		}

		private void ComputeReachability()
		{
			bool startPointIsReachable = false;
			bool endPointIsReachable = false;
			if (_context.Failed)
			{
				startPointIsReachable = true;
				endPointIsReachable = true;
			}
			else
			{
				RegionReachableWalker.Analyze(_context.AnalysisInfo, _context.RegionInfo, out startPointIsReachable, out endPointIsReachable);
			}
			Interlocked.CompareExchange(ref _regionStartPointIsReachable, startPointIsReachable, null);
			Interlocked.CompareExchange(ref _regionEndPointIsReachable, endPointIsReachable, null);
		}
	}
}
