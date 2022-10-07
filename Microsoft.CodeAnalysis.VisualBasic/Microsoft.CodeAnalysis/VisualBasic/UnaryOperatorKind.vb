Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	<Flags>
	Friend Enum UnaryOperatorKind
		Plus = 1
		Minus = 2
		IntrinsicOpMask = 3
		[Not] = 3
		Lifted = 4
		UserDefined = 8
		Implicit = 16
		Explicit = 32
		IsTrue = 48
		IsFalse = 64
		OpMask = 115
		[Error] = 128
	End Enum
End Namespace