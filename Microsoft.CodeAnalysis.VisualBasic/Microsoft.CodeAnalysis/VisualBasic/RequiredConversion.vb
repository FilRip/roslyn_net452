Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Enum RequiredConversion
		Identity
		Any
		AnyReverse
		AnyAndReverse
		ArrayElement
		Reference
		ReverseReference
		None
		Count
	End Enum
End Namespace