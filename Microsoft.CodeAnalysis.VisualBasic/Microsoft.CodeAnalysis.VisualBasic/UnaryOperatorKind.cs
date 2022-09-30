using System;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[Flags]
	internal enum UnaryOperatorKind
	{
		Plus = 1,
		Minus = 2,
		Not = 3,
		IntrinsicOpMask = 3,
		Lifted = 4,
		UserDefined = 8,
		Implicit = 0x10,
		Explicit = 0x20,
		IsTrue = 0x30,
		IsFalse = 0x40,
		OpMask = 0x73,
		Error = 0x80
	}
}
