Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	<Flags>
	Friend Enum ConversionKind
		FailedDueToNumericOverflow = -2147483648
		FailedDueToIntegerOverflow = -1073741824
		FailedDueToNumericOverflowMask = -1073741824
		DelegateRelaxationLevelNone = 0
		[Widening] = 1
		[Narrowing] = 2
		Identity = 5
		Numeric = 8
		WideningNumeric = 9
		NarrowingNumeric = 10
		Nullable = 16
		WideningNullable = 17
		NarrowingNullable = 18
		Reference = 32
		WideningReference = 33
		NarrowingReference = 34
		Array = 64
		WideningArray = 65
		NarrowingArray = 66
		TypeParameter = 128
		WideningTypeParameter = 129
		NarrowingTypeParameter = 130
		Value = 256
		WideningValue = 257
		NarrowingValue = 258
		[String] = 512
		WideningString = 513
		NarrowingString = 514
		[Boolean] = 1024
		NarrowingBoolean = 1026
		WideningNothingLiteral = 2049
		InvolvesNarrowingFromNumericConstant = 4096
		InvolvesEnumTypeConversions = 8192
		Lambda = 16384
		DelegateRelaxationLevelWidening = 32768
		DelegateRelaxationLevelWideningDropReturnOrArgs = 65536
		DelegateRelaxationLevelWideningToNonLambda = 98304
		DelegateRelaxationLevelNarrowing = 131072
		DelegateRelaxationLevelInvalid = 163840
		DelegateRelaxationLevelMask = 229376
		VarianceConversionAmbiguity = 262144
		MightSucceedAtRuntime = 524288
		AnonymousDelegate = 1048576
		NeedAStub = 2097152
		ConvertedToExpressionTree = 4194304
		UserDefined = 8388608
		NarrowingDueToContraVarianceInDelegate = 16777216
		InterpolatedString = 33554433
		Tuple = 67108864
		WideningTuple = 67108865
		NarrowingTuple = 67108866
		WideningNullableTuple = 67108881
		NarrowingNullableTuple = 67108882
		FailedDueToArrayLiteralElementConversion = 268435456
		FailedDueToQueryLambdaBodyMismatch = 536870912
	End Enum
End Namespace