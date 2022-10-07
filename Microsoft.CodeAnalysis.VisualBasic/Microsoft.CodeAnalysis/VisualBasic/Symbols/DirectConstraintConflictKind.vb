Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	<Flags>
	Friend Enum DirectConstraintConflictKind
		None
		DuplicateTypeConstraint
		RedundantConstraint
		All
	End Enum
End Namespace