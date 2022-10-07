Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Interface IBoundInvalidNode
		ReadOnly Property InvalidNodeChildren As ImmutableArray(Of BoundNode)
	End Interface
End Namespace