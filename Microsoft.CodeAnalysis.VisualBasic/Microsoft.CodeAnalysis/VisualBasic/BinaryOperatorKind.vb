Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	<Flags>
	Friend Enum BinaryOperatorKind
		Add = 1
		Concatenate = 2
		[Like] = 3
		Equals = 4
		NotEquals = 5
		LessThanOrEqual = 6
		GreaterThanOrEqual = 7
		LessThan = 8
		GreaterThan = 9
		Subtract = 10
		Multiply = 11
		Power = 12
		Divide = 13
		Modulo = 14
		IntegerDivide = 15
		LeftShift = 16
		RightShift = 17
		[Xor] = 18
		[Or] = 19
		[OrElse] = 20
		[And] = 21
		[AndAlso] = 22
		[Is] = 23
		[IsNot] = 24
		OpMask = 31
		Lifted = 32
		CompareText = 64
		UserDefined = 128
		[Error] = 256
		IsOperandOfConditionalBranch = 512
		OptimizableForConditionalBranch = 1024
	End Enum
End Namespace