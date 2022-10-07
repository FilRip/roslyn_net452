Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	<Flags>
	Friend Enum EmbeddedSymbolKind As Byte
		None = 0
		Unset = 1
		EmbeddedAttribute = 2
		VbCore = 4
		LastValue = 8
		XmlHelper = 8
		All = 14
	End Enum
End Namespace