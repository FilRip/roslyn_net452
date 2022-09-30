namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal struct MemberResolutionResult<TMember> where TMember : Symbol
	{
		private readonly OverloadResolution.CandidateAnalysisResult _candidate;

		private readonly bool _isValid;

		public TMember Member => (TMember)_candidate.Candidate.UnderlyingSymbol;

		public MemberResolutionKind Resolution
		{
			get
			{
				if (_candidate.State == OverloadResolution.CandidateAnalysisResultState.HasUnsupportedMetadata)
				{
					return MemberResolutionKind.HasUseSiteError;
				}
				return (MemberResolutionKind)_candidate.State;
			}
		}

		public bool IsValid => _isValid;

		internal bool IsExpandedParamArrayForm => _candidate.IsExpandedParamArrayForm;

		internal MemberResolutionResult(OverloadResolution.CandidateAnalysisResult candidate, bool isValid)
		{
			this = default(MemberResolutionResult<TMember>);
			_candidate = candidate;
			_isValid = isValid;
		}
	}
}
