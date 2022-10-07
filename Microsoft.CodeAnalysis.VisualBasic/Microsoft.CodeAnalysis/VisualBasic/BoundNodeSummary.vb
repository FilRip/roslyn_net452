Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Structure BoundNodeSummary
		Public ReadOnly LowestBoundNode As BoundNode

		Public ReadOnly HighestBoundNode As BoundNode

		Public ReadOnly LowestBoundNodeOfSyntacticParent As BoundNode

		Public Sub New(ByVal lowestBound As BoundNode, ByVal highestBound As BoundNode, ByVal lowestBoundOfSyntacticParent As BoundNode)
			Me = New BoundNodeSummary() With
			{
				.LowestBoundNode = lowestBound,
				.HighestBoundNode = highestBound,
				.LowestBoundNodeOfSyntacticParent = lowestBoundOfSyntacticParent
			}
		End Sub
	End Structure
End Namespace