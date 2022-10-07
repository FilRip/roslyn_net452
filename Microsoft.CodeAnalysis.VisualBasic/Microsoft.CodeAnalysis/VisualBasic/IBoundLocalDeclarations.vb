Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Interface IBoundLocalDeclarations
		ReadOnly Property Declarations As ImmutableArray(Of BoundLocalDeclarationBase)
	End Interface
End Namespace