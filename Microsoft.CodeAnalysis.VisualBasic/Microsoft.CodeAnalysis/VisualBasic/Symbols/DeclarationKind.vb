Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Enum DeclarationKind As Byte
		[Namespace]
		[Class]
		[Interface]
		[Structure]
		[Enum]
		[Delegate]
		[Module]
		Script
		Submission
		ImplicitClass
		EventSyntheticDelegate
	End Enum
End Namespace