Imports Microsoft.CodeAnalysis.Text
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Structure FlowAnalysisRegionInfo
		Public ReadOnly FirstInRegion As BoundNode

		Public ReadOnly LastInRegion As BoundNode

		Public ReadOnly Region As TextSpan

		Public Sub New(ByVal _firstInRegion As BoundNode, ByVal _lastInRegion As BoundNode, ByVal _region As TextSpan)
			Me = New FlowAnalysisRegionInfo() With
			{
				.FirstInRegion = _firstInRegion,
				.LastInRegion = _lastInRegion,
				.Region = _region
			}
		End Sub
	End Structure
End Namespace