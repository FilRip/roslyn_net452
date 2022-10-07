Imports Microsoft.CodeAnalysis.Text
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Structure RegionAnalysisContext
		Private ReadOnly _compilation As VisualBasicCompilation

		Private ReadOnly _symbol As Symbol

		Private ReadOnly _boundNode As BoundNode

		Private ReadOnly _firstInRegion As BoundNode

		Private ReadOnly _lastInRegion As BoundNode

		Private ReadOnly _region As TextSpan

		Public ReadOnly Failed As Boolean

		Friend ReadOnly Property AnalysisInfo As FlowAnalysisInfo
			Get
				Return New FlowAnalysisInfo(Me._compilation, Me._symbol, Me._boundNode)
			End Get
		End Property

		Friend ReadOnly Property RegionInfo As FlowAnalysisRegionInfo
			Get
				Return New FlowAnalysisRegionInfo(Me._firstInRegion, Me._lastInRegion, Me._region)
			End Get
		End Property

		Friend Sub New(ByVal compilation As VisualBasicCompilation, ByVal member As Symbol, ByVal boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode, ByVal firstInRegion As Microsoft.CodeAnalysis.VisualBasic.BoundNode, ByVal lastInRegion As Microsoft.CodeAnalysis.VisualBasic.BoundNode, ByVal region As TextSpan)
			Me = New RegionAnalysisContext() With
			{
				._compilation = compilation,
				._symbol = member,
				._boundNode = boundNode,
				._region = region,
				._firstInRegion = firstInRegion,
				._lastInRegion = lastInRegion,
				.Failed = If(Me._symbol Is Nothing OrElse Me._boundNode Is Nothing OrElse Me._firstInRegion Is Nothing, True, Me._lastInRegion Is Nothing)
			}
			If (Not Me.Failed AndAlso Me._firstInRegion = Me._lastInRegion) Then
				Dim kind As BoundKind = Me._firstInRegion.Kind
				If (kind = BoundKind.TypeExpression OrElse kind = BoundKind.NamespaceExpression) Then
					Me.Failed = True
				End If
			End If
		End Sub

		Friend Sub New(ByVal compilation As VisualBasicCompilation)
			Me = New RegionAnalysisContext() With
			{
				._compilation = compilation,
				._symbol = Nothing,
				._boundNode = Nothing,
				._region = New TextSpan(),
				._firstInRegion = Nothing,
				._lastInRegion = Nothing,
				.Failed = True
			}
		End Sub
	End Structure
End Namespace