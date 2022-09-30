using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal struct RegionAnalysisContext
	{
		private readonly VisualBasicCompilation _compilation;

		private readonly Symbol _symbol;

		private readonly BoundNode _boundNode;

		private readonly BoundNode _firstInRegion;

		private readonly BoundNode _lastInRegion;

		private readonly TextSpan _region;

		public readonly bool Failed;

		internal FlowAnalysisInfo AnalysisInfo => new FlowAnalysisInfo(_compilation, _symbol, _boundNode);

		internal FlowAnalysisRegionInfo RegionInfo => new FlowAnalysisRegionInfo(_firstInRegion, _lastInRegion, _region);

		internal RegionAnalysisContext(VisualBasicCompilation compilation, Symbol member, BoundNode boundNode, BoundNode firstInRegion, BoundNode lastInRegion, TextSpan region)
		{
			this = default(RegionAnalysisContext);
			_compilation = compilation;
			_symbol = member;
			_boundNode = boundNode;
			_region = region;
			_firstInRegion = firstInRegion;
			_lastInRegion = lastInRegion;
			Failed = (object)_symbol == null || _boundNode == null || _firstInRegion == null || _lastInRegion == null;
			if (!Failed && _firstInRegion == _lastInRegion)
			{
				BoundKind kind = _firstInRegion.Kind;
				if (kind == BoundKind.TypeExpression || kind == BoundKind.NamespaceExpression)
				{
					Failed = true;
				}
			}
		}

		internal RegionAnalysisContext(VisualBasicCompilation compilation)
		{
			this = default(RegionAnalysisContext);
			_compilation = compilation;
			_symbol = null;
			_boundNode = null;
			_region = default(TextSpan);
			_firstInRegion = null;
			_lastInRegion = null;
			Failed = true;
		}
	}
}
