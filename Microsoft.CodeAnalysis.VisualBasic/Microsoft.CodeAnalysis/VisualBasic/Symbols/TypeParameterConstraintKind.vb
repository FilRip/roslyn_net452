Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	<Flags>
	Friend Enum TypeParameterConstraintKind
		None = 0
		ReferenceType = 1
		ValueType = 2
		Constructor = 4
	End Enum
End Namespace