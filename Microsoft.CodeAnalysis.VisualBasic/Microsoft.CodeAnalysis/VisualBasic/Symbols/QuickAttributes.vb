Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	<Flags>
	Friend Enum QuickAttributes As Byte
		None = 0
		Extension = 1
		Obsolete = 2
		MyGroupCollection = 4
		TypeIdentifier = 8
	End Enum
End Namespace