Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Structure MemberResolutionResult(Of TMember As Symbol)
		Private ReadOnly _candidate As OverloadResolution.CandidateAnalysisResult

		Private ReadOnly _isValid As Boolean

		Friend ReadOnly Property IsExpandedParamArrayForm As Boolean
			Get
				Return Me._candidate.IsExpandedParamArrayForm
			End Get
		End Property

		Public ReadOnly Property IsValid As Boolean
			Get
				Return Me._isValid
			End Get
		End Property

		Public ReadOnly Property Member As TMember
			Get
				Return DirectCast(Me._candidate.Candidate.UnderlyingSymbol, TMember)
			End Get
		End Property

		Public ReadOnly Property Resolution As MemberResolutionKind
			Get
				Dim state As MemberResolutionKind
				If (Me._candidate.State <> OverloadResolution.CandidateAnalysisResultState.HasUnsupportedMetadata) Then
					state = DirectCast(Me._candidate.State, MemberResolutionKind)
				Else
					state = MemberResolutionKind.HasUseSiteError
				End If
				Return state
			End Get
		End Property

		Friend Sub New(ByVal candidate As OverloadResolution.CandidateAnalysisResult, ByVal isValid As Boolean)
			Me = New MemberResolutionResult(Of TMember)() With
			{
				._candidate = candidate,
				._isValid = isValid
			}
		End Sub
	End Structure
End Namespace