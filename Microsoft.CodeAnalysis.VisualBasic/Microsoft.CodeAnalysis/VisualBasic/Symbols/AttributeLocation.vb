Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	<Flags>
	Friend Enum AttributeLocation
		None = 0
		Assembly = 1
		[Module] = 2
		Type = 4
		Method = 8
		Field = 16
		[Property] = 32
		[Event] = 64
		Parameter = 128
		[Return] = 256
		TypeParameter = 512
	End Enum
End Namespace