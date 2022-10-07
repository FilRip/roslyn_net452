Imports Microsoft.CodeAnalysis
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Module LookupResultKindExtensions
		<Extension>
		Public Function ToCandidateReason(ByVal resultKind As LookupResultKind) As Microsoft.CodeAnalysis.CandidateReason
			Dim candidateReason As Microsoft.CodeAnalysis.CandidateReason
			Select Case resultKind
				Case LookupResultKind.Empty
				Case LookupResultKind.EmptyAndStopLookup
					candidateReason = Microsoft.CodeAnalysis.CandidateReason.None
					Exit Select
				Case LookupResultKind.NotATypeOrNamespace
					candidateReason = Microsoft.CodeAnalysis.CandidateReason.NotATypeOrNamespace
					Exit Select
				Case LookupResultKind.NotAnAttributeType
					candidateReason = Microsoft.CodeAnalysis.CandidateReason.NotAnAttributeType
					Exit Select
				Case LookupResultKind.WrongArity
				Case LookupResultKind.WrongArityAndStopLookup
					candidateReason = Microsoft.CodeAnalysis.CandidateReason.WrongArity
					Exit Select
				Case LookupResultKind.NotCreatable
					candidateReason = Microsoft.CodeAnalysis.CandidateReason.NotCreatable
					Exit Select
				Case LookupResultKind.Inaccessible
					candidateReason = Microsoft.CodeAnalysis.CandidateReason.Inaccessible
					Exit Select
				Case LookupResultKind.NotReferencable
					candidateReason = Microsoft.CodeAnalysis.CandidateReason.NotReferencable
					Exit Select
				Case LookupResultKind.NotAValue
					candidateReason = Microsoft.CodeAnalysis.CandidateReason.NotAValue
					Exit Select
				Case LookupResultKind.NotAVariable
					candidateReason = Microsoft.CodeAnalysis.CandidateReason.NotAVariable
					Exit Select
				Case LookupResultKind.MustNotBeInstance
				Case LookupResultKind.MustBeInstance
					candidateReason = Microsoft.CodeAnalysis.CandidateReason.StaticInstanceMismatch
					Exit Select
				Case LookupResultKind.NotAnEvent
					candidateReason = Microsoft.CodeAnalysis.CandidateReason.NotAnEvent
					Exit Select
				Case LookupResultKind.LateBound
					candidateReason = Microsoft.CodeAnalysis.CandidateReason.LateBound
					Exit Select
				Case LookupResultKind.OverloadResolutionFailure
					candidateReason = Microsoft.CodeAnalysis.CandidateReason.OverloadResolutionFailure
					Exit Select
				Case LookupResultKind.NotAWithEventsMember
					candidateReason = Microsoft.CodeAnalysis.CandidateReason.NotAWithEventsMember
					Exit Select
				Case LookupResultKind.Ambiguous
					candidateReason = Microsoft.CodeAnalysis.CandidateReason.Ambiguous
					Exit Select
				Case LookupResultKind.MemberGroup
					candidateReason = Microsoft.CodeAnalysis.CandidateReason.MemberGroup
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(resultKind)
			End Select
			Return candidateReason
		End Function
	End Module
End Namespace