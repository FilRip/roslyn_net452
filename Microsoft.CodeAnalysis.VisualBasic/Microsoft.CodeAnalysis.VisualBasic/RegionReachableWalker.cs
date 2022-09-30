namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class RegionReachableWalker : AbstractRegionControlFlowPass
	{
		private bool? _regionStartPointIsReachable;

		private bool? _regionEndPointIsReachable;

		internal static void Analyze(FlowAnalysisInfo info, FlowAnalysisRegionInfo region, out bool startPointIsReachable, out bool endPointIsReachable)
		{
			RegionReachableWalker regionReachableWalker = new RegionReachableWalker(info, region);
			try
			{
				if (regionReachableWalker.Analyze())
				{
					startPointIsReachable = !regionReachableWalker._regionStartPointIsReachable.HasValue || regionReachableWalker._regionStartPointIsReachable.Value;
					endPointIsReachable = (regionReachableWalker._regionEndPointIsReachable.HasValue ? regionReachableWalker._regionEndPointIsReachable.Value : regionReachableWalker.State.Alive);
				}
				else
				{
					startPointIsReachable = true;
					startPointIsReachable = false;
				}
			}
			finally
			{
				regionReachableWalker.Free();
			}
		}

		private RegionReachableWalker(FlowAnalysisInfo info, FlowAnalysisRegionInfo region)
			: base(info, region)
		{
		}

		protected override void EnterRegion()
		{
			_regionStartPointIsReachable = State.Alive;
			base.EnterRegion();
		}

		protected override void LeaveRegion()
		{
			_regionEndPointIsReachable = State.Alive;
			base.LeaveRegion();
		}
	}
}
