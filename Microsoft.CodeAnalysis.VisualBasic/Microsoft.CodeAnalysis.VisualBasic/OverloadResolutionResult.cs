using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class OverloadResolutionResult<TMember> where TMember : Symbol
	{
		private readonly MemberResolutionResult<TMember>? _validResult;

		private readonly MemberResolutionResult<TMember>? _bestResult;

		private ImmutableArray<MemberResolutionResult<TMember>> _results;

		public bool Succeeded => ValidResult.HasValue;

		public MemberResolutionResult<TMember>? ValidResult => _validResult;

		public MemberResolutionResult<TMember>? BestResult => _bestResult;

		public ImmutableArray<MemberResolutionResult<TMember>> Results => _results;

		internal OverloadResolutionResult(ImmutableArray<MemberResolutionResult<TMember>> results, MemberResolutionResult<TMember>? validResult, MemberResolutionResult<TMember>? bestResult)
		{
			_results = results;
			_validResult = validResult;
			_bestResult = bestResult;
		}
	}
}
