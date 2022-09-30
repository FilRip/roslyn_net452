using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class DominantTypeData
	{
		public TypeSymbol ResultType;

		public RequiredConversion InferenceRestrictions;

		public bool IsStrictCandidate;

		public bool IsUnstrictCandidate;

		public DominantTypeData()
		{
			ResultType = null;
			InferenceRestrictions = RequiredConversion.Any;
			IsStrictCandidate = false;
			IsUnstrictCandidate = false;
		}
	}
}
