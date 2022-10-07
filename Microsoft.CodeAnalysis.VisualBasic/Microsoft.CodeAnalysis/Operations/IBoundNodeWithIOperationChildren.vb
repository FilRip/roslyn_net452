Imports Microsoft.CodeAnalysis.VisualBasic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.Operations
	Friend Interface IBoundNodeWithIOperationChildren
		ReadOnly Property Children As ImmutableArray(Of BoundNode)
	End Interface
End Namespace