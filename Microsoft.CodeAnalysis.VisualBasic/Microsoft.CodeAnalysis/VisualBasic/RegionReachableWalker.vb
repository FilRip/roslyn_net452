Imports System
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class RegionReachableWalker
		Inherits AbstractRegionControlFlowPass
		Private _regionStartPointIsReachable As Nullable(Of Boolean)

		Private _regionEndPointIsReachable As Nullable(Of Boolean)

		Private Sub New(ByVal info As FlowAnalysisInfo, ByVal region As FlowAnalysisRegionInfo)
			MyBase.New(info, region)
		End Sub

		Friend Shared Sub Analyze(ByVal info As FlowAnalysisInfo, ByVal region As FlowAnalysisRegionInfo, <Out> ByRef startPointIsReachable As Boolean, <Out> ByRef endPointIsReachable As Boolean)
			Dim regionReachableWalker As Microsoft.CodeAnalysis.VisualBasic.RegionReachableWalker = New Microsoft.CodeAnalysis.VisualBasic.RegionReachableWalker(info, region)
			Try
				If (Not regionReachableWalker.Analyze()) Then
					startPointIsReachable = True
					startPointIsReachable = False
				Else
					startPointIsReachable = If(regionReachableWalker._regionStartPointIsReachable.HasValue, regionReachableWalker._regionStartPointIsReachable.Value, True)
					endPointIsReachable = If(regionReachableWalker._regionEndPointIsReachable.HasValue, regionReachableWalker._regionEndPointIsReachable.Value, regionReachableWalker.State.Alive)
				End If
			Finally
				regionReachableWalker.Free()
			End Try
		End Sub

		Protected Overrides Sub EnterRegion()
			Me._regionStartPointIsReachable = New Nullable(Of Boolean)(Me.State.Alive)
			MyBase.EnterRegion()
		End Sub

		Protected Overrides Sub LeaveRegion()
			Me._regionEndPointIsReachable = New Nullable(Of Boolean)(Me.State.Alive)
			MyBase.LeaveRegion()
		End Sub
	End Class
End Namespace