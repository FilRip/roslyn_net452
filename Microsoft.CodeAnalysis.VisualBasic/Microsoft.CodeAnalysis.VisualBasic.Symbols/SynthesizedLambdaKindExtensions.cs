using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[StandardModule]
	internal sealed class SynthesizedLambdaKindExtensions
	{
		internal static bool IsQueryLambda(this SynthesizedLambdaKind kind)
		{
			if (kind >= SynthesizedLambdaKind.FilterConditionQueryLambda)
			{
				return kind <= SynthesizedLambdaKind.ConversionNonUserCodeQueryLambda;
			}
			return false;
		}
	}
}
