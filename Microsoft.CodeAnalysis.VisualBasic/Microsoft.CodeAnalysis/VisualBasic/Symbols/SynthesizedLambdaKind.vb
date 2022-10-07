Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Enum SynthesizedLambdaKind
		UserDefined
		DelegateRelaxationStub
		LateBoundAddressOfLambda
		FilterConditionQueryLambda
		OrderingQueryLambda
		AggregationQueryLambda
		AggregateQueryLambda
		FromOrAggregateVariableQueryLambda
		LetVariableQueryLambda
		SelectQueryLambda
		GroupByItemsQueryLambda
		GroupByKeysQueryLambda
		JoinLeftQueryLambda
		JoinRightQueryLambda
		JoinNonUserCodeQueryLambda
		AggregateNonUserCodeQueryLambda
		FromNonUserCodeQueryLambda
		GroupNonUserCodeQueryLambda
		ConversionNonUserCodeQueryLambda
	End Enum
End Namespace