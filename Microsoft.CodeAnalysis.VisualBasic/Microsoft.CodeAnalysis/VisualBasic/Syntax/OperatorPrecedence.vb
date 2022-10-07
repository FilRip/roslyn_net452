Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Friend Enum OperatorPrecedence As Byte
		PrecedenceNone
		PrecedenceXor
		PrecedenceOr
		PrecedenceAnd
		PrecedenceNot
		PrecedenceRelational
		PrecedenceShift
		PrecedenceConcatenate
		PrecedenceAdd
		PrecedenceModulus
		PrecedenceIntegerDivide
		PrecedenceMultiply
		PrecedenceNegate
		PrecedenceExponentiate
		PrecedenceAwait
	End Enum
End Namespace