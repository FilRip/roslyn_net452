using System;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[Flags]
	internal enum BinaryOperatorKind
	{
		Add = 1,
		Concatenate = 2,
		Like = 3,
		Equals = 4,
		NotEquals = 5,
		LessThanOrEqual = 6,
		GreaterThanOrEqual = 7,
		LessThan = 8,
		GreaterThan = 9,
		Subtract = 0xA,
		Multiply = 0xB,
		Power = 0xC,
		Divide = 0xD,
		Modulo = 0xE,
		IntegerDivide = 0xF,
		LeftShift = 0x10,
		RightShift = 0x11,
		Xor = 0x12,
		Or = 0x13,
		OrElse = 0x14,
		And = 0x15,
		AndAlso = 0x16,
		Is = 0x17,
		IsNot = 0x18,
		OpMask = 0x1F,
		Lifted = 0x20,
		CompareText = 0x40,
		UserDefined = 0x80,
		Error = 0x100,
		IsOperandOfConditionalBranch = 0x200,
		OptimizableForConditionalBranch = 0x400
	}
}
