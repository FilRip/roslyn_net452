namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal enum LookupResultKind
	{
		Empty,
		NotATypeOrNamespace,
		NotAnAttributeType,
		WrongArity,
		NotCreatable,
		Inaccessible,
		NotReferencable,
		NotAValue,
		NotAVariable,
		MustNotBeInstance,
		MustBeInstance,
		NotAnEvent,
		LateBound,
		EmptyAndStopLookup,
		WrongArityAndStopLookup,
		OverloadResolutionFailure,
		NotAWithEventsMember,
		Ambiguous,
		MemberGroup,
		Good
	}
}
