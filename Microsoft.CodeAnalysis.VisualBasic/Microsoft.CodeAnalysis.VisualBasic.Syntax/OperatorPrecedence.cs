namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	internal enum OperatorPrecedence : byte
	{
		PrecedenceNone,
		PrecedenceXor,
		PrecedenceOr,
		PrecedenceAnd,
		PrecedenceNot,
		PrecedenceRelational,
		PrecedenceShift,
		PrecedenceConcatenate,
		PrecedenceAdd,
		PrecedenceModulus,
		PrecedenceIntegerDivide,
		PrecedenceMultiply,
		PrecedenceNegate,
		PrecedenceExponentiate,
		PrecedenceAwait
	}
}
