namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal enum BoundKind : byte
	{
		TypeArguments,
		OmittedArgument,
		LValueToRValueWrapper,
		WithLValueExpressionPlaceholder,
		WithRValueExpressionPlaceholder,
		RValuePlaceholder,
		LValuePlaceholder,
		Dup,
		BadExpression,
		BadStatement,
		Parenthesized,
		BadVariable,
		ArrayAccess,
		ArrayLength,
		GetType,
		FieldInfo,
		MethodInfo,
		TypeExpression,
		TypeOrValueExpression,
		NamespaceExpression,
		MethodDefIndex,
		MaximumMethodDefIndex,
		InstrumentationPayloadRoot,
		ModuleVersionId,
		ModuleVersionIdString,
		SourceDocumentIndex,
		UnaryOperator,
		UserDefinedUnaryOperator,
		NullableIsTrueOperator,
		BinaryOperator,
		UserDefinedBinaryOperator,
		UserDefinedShortCircuitingOperator,
		CompoundAssignmentTargetPlaceholder,
		AssignmentOperator,
		ReferenceAssignment,
		AddressOfOperator,
		TernaryConditionalExpression,
		BinaryConditionalExpression,
		Conversion,
		RelaxationLambda,
		ConvertedTupleElements,
		UserDefinedConversion,
		DirectCast,
		TryCast,
		TypeOf,
		SequencePoint,
		SequencePointExpression,
		SequencePointWithSpan,
		NoOpStatement,
		MethodGroup,
		PropertyGroup,
		ReturnStatement,
		YieldStatement,
		ThrowStatement,
		RedimStatement,
		RedimClause,
		EraseStatement,
		Call,
		Attribute,
		LateMemberAccess,
		LateInvocation,
		LateAddressOfOperator,
		TupleLiteral,
		ConvertedTupleLiteral,
		ObjectCreationExpression,
		NoPiaObjectCreationExpression,
		AnonymousTypeCreationExpression,
		AnonymousTypePropertyAccess,
		AnonymousTypeFieldInitializer,
		ObjectInitializerExpression,
		CollectionInitializerExpression,
		NewT,
		DelegateCreationExpression,
		ArrayCreation,
		ArrayLiteral,
		ArrayInitialization,
		FieldAccess,
		PropertyAccess,
		EventAccess,
		Block,
		StateMachineScope,
		LocalDeclaration,
		AsNewLocalDeclarations,
		DimStatement,
		Initializer,
		FieldInitializer,
		PropertyInitializer,
		ParameterEqualsValue,
		GlobalStatementInitializer,
		Sequence,
		ExpressionStatement,
		IfStatement,
		SelectStatement,
		CaseBlock,
		CaseStatement,
		SimpleCaseClause,
		RangeCaseClause,
		RelationalCaseClause,
		DoLoopStatement,
		WhileStatement,
		ForToUserDefinedOperators,
		ForToStatement,
		ForEachStatement,
		ExitStatement,
		ContinueStatement,
		TryStatement,
		CatchBlock,
		Literal,
		MeReference,
		ValueTypeMeReference,
		MyBaseReference,
		MyClassReference,
		PreviousSubmissionReference,
		HostObjectMemberReference,
		Local,
		PseudoVariable,
		Parameter,
		ByRefArgumentPlaceholder,
		ByRefArgumentWithCopyBack,
		LateBoundArgumentSupportingAssignmentWithCapture,
		LabelStatement,
		Label,
		GotoStatement,
		StatementList,
		ConditionalGoto,
		WithStatement,
		UnboundLambda,
		Lambda,
		QueryExpression,
		QuerySource,
		ToQueryableCollectionConversion,
		QueryableSource,
		QueryClause,
		Ordering,
		QueryLambda,
		RangeVariableAssignment,
		GroupTypeInferenceLambda,
		AggregateClause,
		GroupAggregation,
		RangeVariable,
		AddHandlerStatement,
		RemoveHandlerStatement,
		RaiseEventStatement,
		UsingStatement,
		SyncLockStatement,
		XmlName,
		XmlNamespace,
		XmlDocument,
		XmlDeclaration,
		XmlProcessingInstruction,
		XmlComment,
		XmlAttribute,
		XmlElement,
		XmlMemberAccess,
		XmlEmbeddedExpression,
		XmlCData,
		ResumeStatement,
		OnErrorStatement,
		UnstructuredExceptionHandlingStatement,
		UnstructuredExceptionHandlingCatchFilter,
		UnstructuredExceptionOnErrorSwitch,
		UnstructuredExceptionResumeSwitch,
		AwaitOperator,
		SpillSequence,
		StopStatement,
		EndStatement,
		MidResult,
		ConditionalAccess,
		ConditionalAccessReceiverPlaceholder,
		LoweredConditionalAccess,
		ComplexConditionalAccessReceiver,
		NameOfOperator,
		TypeAsValueExpression,
		InterpolatedStringExpression,
		Interpolation
	}
}