Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	<Flags>
	Friend Enum InferenceErrorReasons As Byte
		Other
		Ambiguous
		NoBest
	End Enum
End Namespace