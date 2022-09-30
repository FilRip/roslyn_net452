namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal enum MemberResolutionKind
	{
		Applicable = 0,
		HasUseSiteError = 2,
		Ambiguous = 3,
		BadGenericArity = 4,
		ArgumentCountMismatch = 5,
		TypeInferenceFailed = 6,
		ArgumentMismatch = 7,
		GenericConstraintsViolated = 8,
		RequiresNarrowing = 9,
		RequiresNarrowingNotFromObject = 10,
		ExtensionMethodVsInstanceMethod = 11,
		Shadowed = 12,
		LessApplicable = 13
	}
}
