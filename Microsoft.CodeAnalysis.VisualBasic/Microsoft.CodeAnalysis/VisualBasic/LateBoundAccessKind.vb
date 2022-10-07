Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	<Flags>
	Friend Enum LateBoundAccessKind
		Unknown = 0
		[Get] = 1
		[Set] = 2
		[Call] = 4
	End Enum
End Namespace