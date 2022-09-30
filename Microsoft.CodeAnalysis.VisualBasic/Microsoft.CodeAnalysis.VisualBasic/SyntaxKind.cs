namespace Microsoft.CodeAnalysis.VisualBasic
{
	public enum SyntaxKind : ushort
	{
		None = 0,
		List = 1,
		EmptyStatement = 2,
		EndIfStatement = 5,
		EndUsingStatement = 6,
		EndWithStatement = 7,
		EndSelectStatement = 8,
		EndStructureStatement = 9,
		EndEnumStatement = 10,
		EndInterfaceStatement = 11,
		EndClassStatement = 12,
		EndModuleStatement = 13,
		EndNamespaceStatement = 14,
		EndSubStatement = 15,
		EndFunctionStatement = 16,
		EndGetStatement = 17,
		EndSetStatement = 18,
		EndPropertyStatement = 19,
		EndOperatorStatement = 20,
		EndEventStatement = 21,
		EndAddHandlerStatement = 22,
		EndRemoveHandlerStatement = 23,
		EndRaiseEventStatement = 24,
		EndWhileStatement = 25,
		EndTryStatement = 26,
		EndSyncLockStatement = 27,
		CompilationUnit = 38,
		OptionStatement = 41,
		ImportsStatement = 42,
		SimpleImportsClause = 44,
		XmlNamespaceImportsClause = 45,
		NamespaceBlock = 48,
		NamespaceStatement = 49,
		ModuleBlock = 50,
		StructureBlock = 51,
		InterfaceBlock = 52,
		ClassBlock = 53,
		EnumBlock = 54,
		InheritsStatement = 57,
		ImplementsStatement = 58,
		ModuleStatement = 59,
		StructureStatement = 60,
		InterfaceStatement = 61,
		ClassStatement = 62,
		EnumStatement = 63,
		TypeParameterList = 66,
		TypeParameter = 67,
		TypeParameterSingleConstraintClause = 70,
		TypeParameterMultipleConstraintClause = 71,
		NewConstraint = 72,
		ClassConstraint = 73,
		StructureConstraint = 74,
		TypeConstraint = 75,
		EnumMemberDeclaration = 78,
		SubBlock = 79,
		FunctionBlock = 80,
		ConstructorBlock = 81,
		OperatorBlock = 82,
		GetAccessorBlock = 83,
		SetAccessorBlock = 84,
		AddHandlerAccessorBlock = 85,
		RemoveHandlerAccessorBlock = 86,
		RaiseEventAccessorBlock = 87,
		PropertyBlock = 88,
		EventBlock = 89,
		ParameterList = 92,
		SubStatement = 93,
		FunctionStatement = 94,
		SubNewStatement = 95,
		DeclareSubStatement = 96,
		DeclareFunctionStatement = 97,
		DelegateSubStatement = 98,
		DelegateFunctionStatement = 99,
		EventStatement = 102,
		OperatorStatement = 103,
		PropertyStatement = 104,
		GetAccessorStatement = 105,
		SetAccessorStatement = 106,
		AddHandlerAccessorStatement = 107,
		RemoveHandlerAccessorStatement = 108,
		RaiseEventAccessorStatement = 111,
		ImplementsClause = 112,
		HandlesClause = 113,
		KeywordEventContainer = 114,
		WithEventsEventContainer = 115,
		WithEventsPropertyEventContainer = 116,
		HandlesClauseItem = 117,
		IncompleteMember = 118,
		FieldDeclaration = 119,
		VariableDeclarator = 122,
		SimpleAsClause = 123,
		AsNewClause = 124,
		ObjectMemberInitializer = 125,
		ObjectCollectionInitializer = 126,
		InferredFieldInitializer = 127,
		NamedFieldInitializer = 128,
		EqualsValue = 129,
		Parameter = 132,
		ModifiedIdentifier = 133,
		ArrayRankSpecifier = 134,
		AttributeList = 135,
		Attribute = 136,
		AttributeTarget = 137,
		AttributesStatement = 138,
		ExpressionStatement = 139,
		PrintStatement = 140,
		WhileBlock = 141,
		UsingBlock = 144,
		SyncLockBlock = 145,
		WithBlock = 146,
		LocalDeclarationStatement = 147,
		LabelStatement = 148,
		GoToStatement = 149,
		IdentifierLabel = 150,
		NumericLabel = 151,
		NextLabel = 152,
		StopStatement = 153,
		EndStatement = 156,
		ExitDoStatement = 157,
		ExitForStatement = 158,
		ExitSubStatement = 159,
		ExitFunctionStatement = 160,
		ExitOperatorStatement = 161,
		ExitPropertyStatement = 162,
		ExitTryStatement = 163,
		ExitSelectStatement = 164,
		ExitWhileStatement = 165,
		ContinueWhileStatement = 166,
		ContinueDoStatement = 167,
		ContinueForStatement = 168,
		ReturnStatement = 169,
		SingleLineIfStatement = 170,
		SingleLineIfPart = 171,
		SingleLineElseClause = 172,
		MultiLineIfBlock = 173,
		ElseIfBlock = 180,
		ElseBlock = 181,
		IfStatement = 182,
		ElseIfStatement = 183,
		ElseStatement = 184,
		TryBlock = 185,
		CatchBlock = 187,
		FinallyBlock = 188,
		TryStatement = 189,
		CatchStatement = 190,
		CatchFilterClause = 191,
		FinallyStatement = 194,
		ErrorStatement = 195,
		OnErrorGoToZeroStatement = 196,
		OnErrorGoToMinusOneStatement = 197,
		OnErrorGoToLabelStatement = 198,
		OnErrorResumeNextStatement = 199,
		ResumeStatement = 200,
		ResumeLabelStatement = 201,
		ResumeNextStatement = 202,
		SelectBlock = 203,
		SelectStatement = 204,
		CaseBlock = 207,
		CaseElseBlock = 210,
		CaseStatement = 211,
		CaseElseStatement = 212,
		ElseCaseClause = 213,
		SimpleCaseClause = 214,
		RangeCaseClause = 215,
		CaseEqualsClause = 216,
		CaseNotEqualsClause = 217,
		CaseLessThanClause = 218,
		CaseLessThanOrEqualClause = 219,
		CaseGreaterThanOrEqualClause = 222,
		CaseGreaterThanClause = 223,
		SyncLockStatement = 226,
		WhileStatement = 234,
		ForBlock = 237,
		ForEachBlock = 238,
		ForStatement = 239,
		ForStepClause = 240,
		ForEachStatement = 241,
		NextStatement = 242,
		UsingStatement = 243,
		ThrowStatement = 246,
		SimpleAssignmentStatement = 247,
		MidAssignmentStatement = 248,
		AddAssignmentStatement = 249,
		SubtractAssignmentStatement = 250,
		MultiplyAssignmentStatement = 251,
		DivideAssignmentStatement = 252,
		IntegerDivideAssignmentStatement = 253,
		ExponentiateAssignmentStatement = 254,
		LeftShiftAssignmentStatement = 255,
		RightShiftAssignmentStatement = 258,
		ConcatenateAssignmentStatement = 259,
		MidExpression = 260,
		CallStatement = 261,
		AddHandlerStatement = 262,
		RemoveHandlerStatement = 263,
		RaiseEventStatement = 264,
		WithStatement = 265,
		ReDimStatement = 266,
		ReDimPreserveStatement = 267,
		RedimClause = 270,
		EraseStatement = 271,
		CharacterLiteralExpression = 272,
		TrueLiteralExpression = 273,
		FalseLiteralExpression = 274,
		NumericLiteralExpression = 275,
		DateLiteralExpression = 276,
		StringLiteralExpression = 279,
		NothingLiteralExpression = 280,
		ParenthesizedExpression = 281,
		MeExpression = 282,
		MyBaseExpression = 283,
		MyClassExpression = 284,
		GetTypeExpression = 285,
		TypeOfIsExpression = 286,
		TypeOfIsNotExpression = 287,
		GetXmlNamespaceExpression = 290,
		SimpleMemberAccessExpression = 291,
		DictionaryAccessExpression = 292,
		XmlElementAccessExpression = 293,
		XmlDescendantAccessExpression = 294,
		XmlAttributeAccessExpression = 295,
		InvocationExpression = 296,
		ObjectCreationExpression = 297,
		AnonymousObjectCreationExpression = 298,
		ArrayCreationExpression = 301,
		CollectionInitializer = 302,
		CTypeExpression = 303,
		DirectCastExpression = 304,
		TryCastExpression = 305,
		PredefinedCastExpression = 306,
		AddExpression = 307,
		SubtractExpression = 308,
		MultiplyExpression = 309,
		DivideExpression = 310,
		IntegerDivideExpression = 311,
		ExponentiateExpression = 314,
		LeftShiftExpression = 315,
		RightShiftExpression = 316,
		ConcatenateExpression = 317,
		ModuloExpression = 318,
		EqualsExpression = 319,
		NotEqualsExpression = 320,
		LessThanExpression = 321,
		LessThanOrEqualExpression = 322,
		GreaterThanOrEqualExpression = 323,
		GreaterThanExpression = 324,
		IsExpression = 325,
		IsNotExpression = 326,
		LikeExpression = 327,
		OrExpression = 328,
		ExclusiveOrExpression = 329,
		AndExpression = 330,
		OrElseExpression = 331,
		AndAlsoExpression = 332,
		UnaryPlusExpression = 333,
		UnaryMinusExpression = 334,
		NotExpression = 335,
		AddressOfExpression = 336,
		BinaryConditionalExpression = 337,
		TernaryConditionalExpression = 338,
		SingleLineFunctionLambdaExpression = 339,
		SingleLineSubLambdaExpression = 342,
		MultiLineFunctionLambdaExpression = 343,
		MultiLineSubLambdaExpression = 344,
		SubLambdaHeader = 345,
		FunctionLambdaHeader = 346,
		ArgumentList = 347,
		OmittedArgument = 348,
		SimpleArgument = 349,
		RangeArgument = 351,
		QueryExpression = 352,
		CollectionRangeVariable = 353,
		ExpressionRangeVariable = 354,
		AggregationRangeVariable = 355,
		VariableNameEquals = 356,
		FunctionAggregation = 357,
		GroupAggregation = 358,
		FromClause = 359,
		LetClause = 360,
		AggregateClause = 361,
		DistinctClause = 362,
		WhereClause = 363,
		SkipWhileClause = 364,
		TakeWhileClause = 365,
		SkipClause = 366,
		TakeClause = 367,
		GroupByClause = 368,
		JoinCondition = 369,
		SimpleJoinClause = 370,
		GroupJoinClause = 371,
		OrderByClause = 372,
		AscendingOrdering = 375,
		DescendingOrdering = 376,
		SelectClause = 377,
		XmlDocument = 378,
		XmlDeclaration = 379,
		XmlDeclarationOption = 380,
		XmlElement = 381,
		XmlText = 382,
		XmlElementStartTag = 383,
		XmlElementEndTag = 384,
		XmlEmptyElement = 385,
		XmlAttribute = 386,
		XmlString = 387,
		XmlPrefixName = 388,
		XmlName = 389,
		XmlBracketedName = 390,
		XmlPrefix = 391,
		XmlComment = 392,
		XmlProcessingInstruction = 393,
		XmlCDataSection = 394,
		XmlEmbeddedExpression = 395,
		ArrayType = 396,
		NullableType = 397,
		PredefinedType = 398,
		IdentifierName = 399,
		GenericName = 400,
		QualifiedName = 401,
		GlobalName = 402,
		TypeArgumentList = 403,
		CrefReference = 404,
		CrefSignature = 407,
		CrefSignaturePart = 408,
		CrefOperatorReference = 409,
		QualifiedCrefOperatorReference = 410,
		YieldStatement = 411,
		AwaitExpression = 412,
		AddHandlerKeyword = 413,
		AddressOfKeyword = 414,
		AliasKeyword = 415,
		AndKeyword = 416,
		AndAlsoKeyword = 417,
		AsKeyword = 418,
		BooleanKeyword = 421,
		ByRefKeyword = 422,
		ByteKeyword = 423,
		ByValKeyword = 424,
		CallKeyword = 425,
		CaseKeyword = 426,
		CatchKeyword = 427,
		CBoolKeyword = 428,
		CByteKeyword = 429,
		CCharKeyword = 432,
		CDateKeyword = 433,
		CDecKeyword = 434,
		CDblKeyword = 435,
		CharKeyword = 436,
		CIntKeyword = 437,
		ClassKeyword = 438,
		CLngKeyword = 439,
		CObjKeyword = 440,
		ConstKeyword = 441,
		ReferenceKeyword = 442,
		ContinueKeyword = 443,
		CSByteKeyword = 444,
		CShortKeyword = 445,
		CSngKeyword = 446,
		CStrKeyword = 447,
		CTypeKeyword = 448,
		CUIntKeyword = 449,
		CULngKeyword = 450,
		CUShortKeyword = 453,
		DateKeyword = 454,
		DecimalKeyword = 455,
		DeclareKeyword = 456,
		DefaultKeyword = 457,
		DelegateKeyword = 458,
		DimKeyword = 459,
		DirectCastKeyword = 460,
		DoKeyword = 461,
		DoubleKeyword = 462,
		EachKeyword = 463,
		ElseKeyword = 464,
		ElseIfKeyword = 465,
		EndKeyword = 466,
		EnumKeyword = 467,
		EraseKeyword = 468,
		ErrorKeyword = 469,
		EventKeyword = 470,
		ExitKeyword = 471,
		FalseKeyword = 474,
		FinallyKeyword = 475,
		ForKeyword = 476,
		FriendKeyword = 477,
		FunctionKeyword = 478,
		GetKeyword = 479,
		GetTypeKeyword = 480,
		GetXmlNamespaceKeyword = 481,
		GlobalKeyword = 482,
		GoToKeyword = 483,
		HandlesKeyword = 484,
		IfKeyword = 485,
		ImplementsKeyword = 486,
		ImportsKeyword = 487,
		InKeyword = 488,
		InheritsKeyword = 489,
		IntegerKeyword = 490,
		InterfaceKeyword = 491,
		IsKeyword = 492,
		IsNotKeyword = 495,
		LetKeyword = 496,
		LibKeyword = 497,
		LikeKeyword = 498,
		LongKeyword = 499,
		LoopKeyword = 500,
		MeKeyword = 501,
		ModKeyword = 502,
		ModuleKeyword = 503,
		MustInheritKeyword = 504,
		MustOverrideKeyword = 505,
		MyBaseKeyword = 506,
		MyClassKeyword = 507,
		NamespaceKeyword = 508,
		NarrowingKeyword = 509,
		NextKeyword = 510,
		NewKeyword = 511,
		NotKeyword = 512,
		NothingKeyword = 513,
		NotInheritableKeyword = 516,
		NotOverridableKeyword = 517,
		ObjectKeyword = 518,
		OfKeyword = 519,
		OnKeyword = 520,
		OperatorKeyword = 521,
		OptionKeyword = 522,
		OptionalKeyword = 523,
		OrKeyword = 524,
		OrElseKeyword = 525,
		OverloadsKeyword = 526,
		OverridableKeyword = 527,
		OverridesKeyword = 528,
		ParamArrayKeyword = 529,
		PartialKeyword = 530,
		PrivateKeyword = 531,
		PropertyKeyword = 532,
		ProtectedKeyword = 533,
		PublicKeyword = 534,
		RaiseEventKeyword = 537,
		ReadOnlyKeyword = 538,
		ReDimKeyword = 539,
		REMKeyword = 540,
		RemoveHandlerKeyword = 541,
		ResumeKeyword = 542,
		ReturnKeyword = 543,
		SByteKeyword = 544,
		SelectKeyword = 545,
		SetKeyword = 546,
		ShadowsKeyword = 547,
		SharedKeyword = 548,
		ShortKeyword = 549,
		SingleKeyword = 550,
		StaticKeyword = 551,
		StepKeyword = 552,
		StopKeyword = 553,
		StringKeyword = 554,
		StructureKeyword = 555,
		SubKeyword = 558,
		SyncLockKeyword = 559,
		ThenKeyword = 560,
		ThrowKeyword = 561,
		ToKeyword = 562,
		TrueKeyword = 563,
		TryKeyword = 564,
		TryCastKeyword = 565,
		TypeOfKeyword = 566,
		UIntegerKeyword = 567,
		ULongKeyword = 568,
		UShortKeyword = 569,
		UsingKeyword = 570,
		WhenKeyword = 571,
		WhileKeyword = 572,
		WideningKeyword = 573,
		WithKeyword = 574,
		WithEventsKeyword = 575,
		WriteOnlyKeyword = 578,
		XorKeyword = 579,
		EndIfKeyword = 580,
		GosubKeyword = 581,
		VariantKeyword = 582,
		WendKeyword = 583,
		AggregateKeyword = 584,
		AllKeyword = 585,
		AnsiKeyword = 586,
		AscendingKeyword = 587,
		AssemblyKeyword = 588,
		AutoKeyword = 589,
		BinaryKeyword = 590,
		ByKeyword = 591,
		CompareKeyword = 592,
		CustomKeyword = 593,
		DescendingKeyword = 594,
		DisableKeyword = 595,
		DistinctKeyword = 596,
		EnableKeyword = 599,
		EqualsKeyword = 600,
		ExplicitKeyword = 601,
		ExternalSourceKeyword = 602,
		ExternalChecksumKeyword = 603,
		FromKeyword = 604,
		GroupKeyword = 605,
		InferKeyword = 606,
		IntoKeyword = 607,
		IsFalseKeyword = 608,
		IsTrueKeyword = 609,
		JoinKeyword = 610,
		KeyKeyword = 611,
		MidKeyword = 612,
		OffKeyword = 613,
		OrderKeyword = 614,
		OutKeyword = 615,
		PreserveKeyword = 616,
		RegionKeyword = 617,
		SkipKeyword = 620,
		StrictKeyword = 621,
		TakeKeyword = 622,
		TextKeyword = 623,
		UnicodeKeyword = 624,
		UntilKeyword = 625,
		WarningKeyword = 626,
		WhereKeyword = 627,
		TypeKeyword = 628,
		XmlKeyword = 629,
		AsyncKeyword = 630,
		AwaitKeyword = 631,
		IteratorKeyword = 632,
		YieldKeyword = 633,
		ExclamationToken = 634,
		AtToken = 635,
		CommaToken = 636,
		HashToken = 637,
		AmpersandToken = 638,
		SingleQuoteToken = 641,
		OpenParenToken = 642,
		CloseParenToken = 643,
		OpenBraceToken = 644,
		CloseBraceToken = 645,
		SemicolonToken = 646,
		AsteriskToken = 647,
		PlusToken = 648,
		MinusToken = 649,
		DotToken = 650,
		SlashToken = 651,
		ColonToken = 652,
		LessThanToken = 653,
		LessThanEqualsToken = 654,
		LessThanGreaterThanToken = 655,
		EqualsToken = 656,
		GreaterThanToken = 657,
		GreaterThanEqualsToken = 658,
		BackslashToken = 659,
		CaretToken = 662,
		ColonEqualsToken = 663,
		AmpersandEqualsToken = 664,
		AsteriskEqualsToken = 665,
		PlusEqualsToken = 666,
		MinusEqualsToken = 667,
		SlashEqualsToken = 668,
		BackslashEqualsToken = 669,
		CaretEqualsToken = 670,
		LessThanLessThanToken = 671,
		GreaterThanGreaterThanToken = 672,
		LessThanLessThanEqualsToken = 673,
		GreaterThanGreaterThanEqualsToken = 674,
		QuestionToken = 675,
		DoubleQuoteToken = 676,
		StatementTerminatorToken = 677,
		EndOfFileToken = 678,
		EmptyToken = 679,
		SlashGreaterThanToken = 680,
		LessThanSlashToken = 683,
		LessThanExclamationMinusMinusToken = 684,
		MinusMinusGreaterThanToken = 685,
		LessThanQuestionToken = 686,
		QuestionGreaterThanToken = 687,
		LessThanPercentEqualsToken = 688,
		PercentGreaterThanToken = 689,
		BeginCDataToken = 690,
		EndCDataToken = 691,
		EndOfXmlToken = 692,
		BadToken = 693,
		XmlNameToken = 694,
		XmlTextLiteralToken = 695,
		XmlEntityLiteralToken = 696,
		DocumentationCommentLineBreakToken = 697,
		IdentifierToken = 700,
		IntegerLiteralToken = 701,
		FloatingLiteralToken = 702,
		DecimalLiteralToken = 703,
		DateLiteralToken = 704,
		StringLiteralToken = 705,
		CharacterLiteralToken = 706,
		SkippedTokensTrivia = 709,
		DocumentationCommentTrivia = 710,
		XmlCrefAttribute = 711,
		XmlNameAttribute = 712,
		ConditionalAccessExpression = 713,
		WhitespaceTrivia = 729,
		EndOfLineTrivia = 730,
		ColonTrivia = 731,
		CommentTrivia = 732,
		LineContinuationTrivia = 733,
		DocumentationCommentExteriorTrivia = 734,
		DisabledTextTrivia = 735,
		ConstDirectiveTrivia = 736,
		IfDirectiveTrivia = 737,
		ElseIfDirectiveTrivia = 738,
		ElseDirectiveTrivia = 739,
		EndIfDirectiveTrivia = 740,
		RegionDirectiveTrivia = 741,
		EndRegionDirectiveTrivia = 744,
		ExternalSourceDirectiveTrivia = 745,
		EndExternalSourceDirectiveTrivia = 746,
		ExternalChecksumDirectiveTrivia = 747,
		EnableWarningDirectiveTrivia = 748,
		DisableWarningDirectiveTrivia = 749,
		ReferenceDirectiveTrivia = 750,
		BadDirectiveTrivia = 753,
		ImportAliasClause = 754,
		NameColonEquals = 755,
		SimpleDoLoopBlock = 756,
		DoWhileLoopBlock = 757,
		DoUntilLoopBlock = 758,
		DoLoopWhileBlock = 759,
		DoLoopUntilBlock = 760,
		SimpleDoStatement = 770,
		DoWhileStatement = 771,
		DoUntilStatement = 772,
		SimpleLoopStatement = 773,
		LoopWhileStatement = 774,
		LoopUntilStatement = 775,
		WhileClause = 776,
		UntilClause = 777,
		NameOfKeyword = 778,
		NameOfExpression = 779,
		InterpolatedStringExpression = 780,
		InterpolatedStringText = 781,
		Interpolation = 782,
		InterpolationAlignmentClause = 783,
		InterpolationFormatClause = 784,
		DollarSignDoubleQuoteToken = 785,
		InterpolatedStringTextToken = 786,
		EndOfInterpolatedStringToken = 787,
		TupleExpression = 788,
		TupleType = 789,
		TypedTupleElement = 790,
		NamedTupleElement = 791,
		ConflictMarkerTrivia = 792
	}
}