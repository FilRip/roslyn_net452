using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	internal sealed class LanguageVersionEnumBounds
	{
		internal static bool IsValid(this LanguageVersion value)
		{
			switch (value)
			{
			case LanguageVersion.VisualBasic9:
			case LanguageVersion.VisualBasic10:
			case LanguageVersion.VisualBasic11:
			case LanguageVersion.VisualBasic12:
			case LanguageVersion.VisualBasic14:
			case LanguageVersion.VisualBasic15:
			case LanguageVersion.VisualBasic15_3:
			case LanguageVersion.VisualBasic15_5:
			case LanguageVersion.VisualBasic16:
			case LanguageVersion.VisualBasic16_9:
				return true;
			default:
				return false;
			}
		}

		internal static string GetErrorName(this LanguageVersion value)
		{
			return value switch
			{
				LanguageVersion.VisualBasic9 => "9.0", 
				LanguageVersion.VisualBasic10 => "10.0", 
				LanguageVersion.VisualBasic11 => "11.0", 
				LanguageVersion.VisualBasic12 => "12.0", 
				LanguageVersion.VisualBasic14 => "14.0", 
				LanguageVersion.VisualBasic15 => "15.0", 
				LanguageVersion.VisualBasic15_3 => "15.3", 
				LanguageVersion.VisualBasic15_5 => "15.5", 
				LanguageVersion.VisualBasic16 => "16", 
				LanguageVersion.VisualBasic16_9 => "16.9", 
				_ => throw ExceptionUtilities.UnexpectedValue(value), 
			};
		}
	}
}
