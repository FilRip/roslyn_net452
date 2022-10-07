Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Enum OnErrorStatementKind As Byte
		GoToZero
		GoToMinusOne
		GoToLabel
		ResumeNext
	End Enum
End Namespace