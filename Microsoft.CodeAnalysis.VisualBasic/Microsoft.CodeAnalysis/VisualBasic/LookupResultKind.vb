Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Enum LookupResultKind
		Empty
		NotATypeOrNamespace
		NotAnAttributeType
		WrongArity
		NotCreatable
		Inaccessible
		NotReferencable
		NotAValue
		NotAVariable
		MustNotBeInstance
		MustBeInstance
		NotAnEvent
		LateBound
		EmptyAndStopLookup
		WrongArityAndStopLookup
		OverloadResolutionFailure
		NotAWithEventsMember
		Ambiguous
		MemberGroup
		Good
	End Enum
End Namespace