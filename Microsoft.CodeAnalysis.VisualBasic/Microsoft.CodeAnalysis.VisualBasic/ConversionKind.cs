using System;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[Flags]
	internal enum ConversionKind
	{
		FailedDueToNumericOverflow = int.MinValue,
		FailedDueToIntegerOverflow = -1073741824,
		FailedDueToNumericOverflowMask = -1073741824,
		FailedDueToQueryLambdaBodyMismatch = 0x20000000,
		FailedDueToArrayLiteralElementConversion = 0x10000000,
		Widening = 1,
		Narrowing = 2,
		Identity = 5,
		Numeric = 8,
		WideningNumeric = 9,
		NarrowingNumeric = 0xA,
		Nullable = 0x10,
		WideningNullable = 0x11,
		NarrowingNullable = 0x12,
		Reference = 0x20,
		WideningReference = 0x21,
		NarrowingReference = 0x22,
		Array = 0x40,
		WideningArray = 0x41,
		NarrowingArray = 0x42,
		TypeParameter = 0x80,
		WideningTypeParameter = 0x81,
		NarrowingTypeParameter = 0x82,
		Value = 0x100,
		WideningValue = 0x101,
		NarrowingValue = 0x102,
		String = 0x200,
		WideningString = 0x201,
		NarrowingString = 0x202,
		Boolean = 0x400,
		NarrowingBoolean = 0x402,
		WideningNothingLiteral = 0x801,
		InvolvesNarrowingFromNumericConstant = 0x1000,
		InvolvesEnumTypeConversions = 0x2000,
		Lambda = 0x4000,
		DelegateRelaxationLevelNone = 0,
		DelegateRelaxationLevelWidening = 0x8000,
		DelegateRelaxationLevelWideningDropReturnOrArgs = 0x10000,
		DelegateRelaxationLevelWideningToNonLambda = 0x18000,
		DelegateRelaxationLevelNarrowing = 0x20000,
		DelegateRelaxationLevelInvalid = 0x28000,
		DelegateRelaxationLevelMask = 0x38000,
		VarianceConversionAmbiguity = 0x40000,
		MightSucceedAtRuntime = 0x80000,
		AnonymousDelegate = 0x100000,
		NeedAStub = 0x200000,
		ConvertedToExpressionTree = 0x400000,
		UserDefined = 0x800000,
		NarrowingDueToContraVarianceInDelegate = 0x1000000,
		InterpolatedString = 0x2000001,
		Tuple = 0x4000000,
		WideningTuple = 0x4000001,
		NarrowingTuple = 0x4000002,
		WideningNullableTuple = 0x4000011,
		NarrowingNullableTuple = 0x4000012
	}
}