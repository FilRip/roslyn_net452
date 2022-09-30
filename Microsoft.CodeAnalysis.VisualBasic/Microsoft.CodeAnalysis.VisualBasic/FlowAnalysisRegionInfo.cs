using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal struct FlowAnalysisRegionInfo
	{
		public readonly BoundNode FirstInRegion;

		public readonly BoundNode LastInRegion;

		public readonly TextSpan Region;

		public FlowAnalysisRegionInfo(BoundNode _firstInRegion, BoundNode _lastInRegion, TextSpan _region)
		{
			this = default(FlowAnalysisRegionInfo);
			FirstInRegion = _firstInRegion;
			LastInRegion = _lastInRegion;
			Region = _region;
		}
	}
}
