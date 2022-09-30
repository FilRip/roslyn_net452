namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal enum SynthesizedLambdaKind
	{
		UserDefined,
		DelegateRelaxationStub,
		LateBoundAddressOfLambda,
		FilterConditionQueryLambda,
		OrderingQueryLambda,
		AggregationQueryLambda,
		AggregateQueryLambda,
		FromOrAggregateVariableQueryLambda,
		LetVariableQueryLambda,
		SelectQueryLambda,
		GroupByItemsQueryLambda,
		GroupByKeysQueryLambda,
		JoinLeftQueryLambda,
		JoinRightQueryLambda,
		JoinNonUserCodeQueryLambda,
		AggregateNonUserCodeQueryLambda,
		FromNonUserCodeQueryLambda,
		GroupNonUserCodeQueryLambda,
		ConversionNonUserCodeQueryLambda
	}
}
