using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	public sealed class LanguageVersionFacts
	{
		internal static LanguageVersion CurrentVersion => LanguageVersion.VisualBasic16_9;

		public static LanguageVersion MapSpecifiedToEffectiveVersion(this LanguageVersion version)
		{
			return version switch
			{
				LanguageVersion.Latest => LanguageVersion.VisualBasic16_9, 
				LanguageVersion.Default => LanguageVersion.VisualBasic16, 
				_ => version, 
			};
		}

		public static string ToDisplayString(this LanguageVersion version)
		{
			return version switch
			{
				LanguageVersion.VisualBasic9 => "9", 
				LanguageVersion.VisualBasic10 => "10", 
				LanguageVersion.VisualBasic11 => "11", 
				LanguageVersion.VisualBasic12 => "12", 
				LanguageVersion.VisualBasic14 => "14", 
				LanguageVersion.VisualBasic15 => "15", 
				LanguageVersion.VisualBasic15_3 => "15.3", 
				LanguageVersion.VisualBasic15_5 => "15.5", 
				LanguageVersion.VisualBasic16 => "16", 
				LanguageVersion.VisualBasic16_9 => "16.9", 
				LanguageVersion.Default => "default", 
				LanguageVersion.Latest => "latest", 
				_ => throw ExceptionUtilities.UnexpectedValue(version), 
			};
		}

		public static bool TryParse(string version, ref LanguageVersion result)
		{
			if (version == null)
			{
				result = LanguageVersion.Default;
				return false;
			}
			string text = version.ToLowerInvariant();
			switch (_003CPrivateImplementationDetails_003E.ComputeStringHash(text))
			{
			case 1007465396u:
				if (EmbeddedOperators.CompareString(text, "9", TextCompare: false) == 0)
				{
					goto IL_02d3;
				}
				goto default;
			case 3791713530u:
				if (EmbeddedOperators.CompareString(text, "9.0", TextCompare: false) == 0)
				{
					goto IL_02d3;
				}
				goto default;
			case 468396612u:
				if (EmbeddedOperators.CompareString(text, "10", TextCompare: false) == 0)
				{
					goto IL_02d9;
				}
				goto default;
			case 2793341610u:
				if (EmbeddedOperators.CompareString(text, "10.0", TextCompare: false) == 0)
				{
					goto IL_02d9;
				}
				goto default;
			case 485174231u:
				if (EmbeddedOperators.CompareString(text, "11", TextCompare: false) == 0)
				{
					goto IL_02df;
				}
				goto default;
			case 224094097u:
				if (EmbeddedOperators.CompareString(text, "11.0", TextCompare: false) == 0)
				{
					goto IL_02df;
				}
				goto default;
			case 501951850u:
				if (EmbeddedOperators.CompareString(text, "12", TextCompare: false) == 0)
				{
					goto IL_02e5;
				}
				goto default;
			case 4220535924u:
				if (EmbeddedOperators.CompareString(text, "12.0", TextCompare: false) == 0)
				{
					goto IL_02e5;
				}
				goto default;
			case 401286136u:
				if (EmbeddedOperators.CompareString(text, "14", TextCompare: false) == 0)
				{
					goto IL_02eb;
				}
				goto default;
			case 3660682646u:
				if (EmbeddedOperators.CompareString(text, "14.0", TextCompare: false) == 0)
				{
					goto IL_02eb;
				}
				goto default;
			case 418063755u:
				if (EmbeddedOperators.CompareString(text, "15", TextCompare: false) == 0)
				{
					goto IL_02f1;
				}
				goto default;
			case 3786196765u:
				if (EmbeddedOperators.CompareString(text, "15.0", TextCompare: false) == 0)
				{
					goto IL_02f1;
				}
				goto default;
			case 3735863908u:
				if (EmbeddedOperators.CompareString(text, "15.3", TextCompare: false) != 0)
				{
					goto default;
				}
				result = LanguageVersion.VisualBasic15_3;
				break;
			case 3702308670u:
				if (EmbeddedOperators.CompareString(text, "15.5", TextCompare: false) != 0)
				{
					goto default;
				}
				result = LanguageVersion.VisualBasic15_5;
				break;
			case 434841374u:
				if (EmbeddedOperators.CompareString(text, "16", TextCompare: false) == 0)
				{
					goto IL_0309;
				}
				goto default;
			case 813698016u:
				if (EmbeddedOperators.CompareString(text, "16.0", TextCompare: false) == 0)
				{
					goto IL_0309;
				}
				goto default;
			case 964696587u:
				if (EmbeddedOperators.CompareString(text, "16.9", TextCompare: false) != 0)
				{
					goto default;
				}
				result = LanguageVersion.VisualBasic16_9;
				break;
			case 2470140894u:
				if (EmbeddedOperators.CompareString(text, "default", TextCompare: false) != 0)
				{
					goto default;
				}
				result = LanguageVersion.Default;
				break;
			case 1097876038u:
				if (EmbeddedOperators.CompareString(text, "latest", TextCompare: false) != 0)
				{
					goto default;
				}
				result = LanguageVersion.Latest;
				break;
			default:
				{
					result = LanguageVersion.Default;
					return false;
				}
				IL_02d9:
				result = LanguageVersion.VisualBasic10;
				break;
				IL_0309:
				result = LanguageVersion.VisualBasic16;
				break;
				IL_02d3:
				result = LanguageVersion.VisualBasic9;
				break;
				IL_02f1:
				result = LanguageVersion.VisualBasic15;
				break;
				IL_02eb:
				result = LanguageVersion.VisualBasic14;
				break;
				IL_02e5:
				result = LanguageVersion.VisualBasic12;
				break;
				IL_02df:
				result = LanguageVersion.VisualBasic11;
				break;
			}
			return true;
		}

		internal static bool DisallowInferredTupleElementNames(this LanguageVersion self)
		{
			return self < FeatureExtensions.GetLanguageVersion(Feature.InferredTupleNames);
		}

		internal static bool AllowNonTrailingNamedArguments(this LanguageVersion self)
		{
			return self >= FeatureExtensions.GetLanguageVersion(Feature.NonTrailingNamedArguments);
		}
	}
}
