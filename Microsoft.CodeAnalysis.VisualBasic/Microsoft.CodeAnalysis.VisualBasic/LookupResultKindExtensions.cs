using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	internal sealed class LookupResultKindExtensions
	{
		public static CandidateReason ToCandidateReason(this LookupResultKind resultKind)
		{
			switch (resultKind)
			{
			case LookupResultKind.Empty:
			case LookupResultKind.EmptyAndStopLookup:
				return CandidateReason.None;
			case LookupResultKind.OverloadResolutionFailure:
				return CandidateReason.OverloadResolutionFailure;
			case LookupResultKind.NotATypeOrNamespace:
				return CandidateReason.NotATypeOrNamespace;
			case LookupResultKind.NotAnEvent:
				return CandidateReason.NotAnEvent;
			case LookupResultKind.LateBound:
				return CandidateReason.LateBound;
			case LookupResultKind.NotAnAttributeType:
				return CandidateReason.NotAnAttributeType;
			case LookupResultKind.NotAWithEventsMember:
				return CandidateReason.NotAWithEventsMember;
			case LookupResultKind.WrongArity:
			case LookupResultKind.WrongArityAndStopLookup:
				return CandidateReason.WrongArity;
			case LookupResultKind.NotCreatable:
				return CandidateReason.NotCreatable;
			case LookupResultKind.Inaccessible:
				return CandidateReason.Inaccessible;
			case LookupResultKind.NotAValue:
				return CandidateReason.NotAValue;
			case LookupResultKind.NotAVariable:
				return CandidateReason.NotAVariable;
			case LookupResultKind.NotReferencable:
				return CandidateReason.NotReferencable;
			case LookupResultKind.MustNotBeInstance:
			case LookupResultKind.MustBeInstance:
				return CandidateReason.StaticInstanceMismatch;
			case LookupResultKind.Ambiguous:
				return CandidateReason.Ambiguous;
			case LookupResultKind.MemberGroup:
				return CandidateReason.MemberGroup;
			default:
				throw ExceptionUtilities.UnexpectedValue(resultKind);
			}
		}
	}
}
