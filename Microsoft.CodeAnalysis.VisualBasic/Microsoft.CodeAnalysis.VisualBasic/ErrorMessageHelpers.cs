using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	internal sealed class ErrorMessageHelpers
	{
		public static string ToDisplay(this Accessibility access)
		{
			return access switch
			{
				Accessibility.NotApplicable => "", 
				Accessibility.Private => "Private", 
				Accessibility.Protected => "Protected", 
				Accessibility.ProtectedOrInternal => "Protected Friend", 
				Accessibility.ProtectedAndInternal => "Private Protected", 
				Accessibility.Internal => "Friend", 
				Accessibility.Public => "Public", 
				_ => throw ExceptionUtilities.UnexpectedValue(access), 
			};
		}
	}
}
