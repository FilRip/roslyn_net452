Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	<Flags>
	Friend Enum PropertyAccessKind
		Unknown
		[Get]
		[Set]
	End Enum
End Namespace