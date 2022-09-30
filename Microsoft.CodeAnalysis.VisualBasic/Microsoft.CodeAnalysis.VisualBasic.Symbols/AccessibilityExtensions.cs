using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[StandardModule]
	internal sealed class AccessibilityExtensions
	{
		internal static string ToDiagnosticString(this Accessibility a)
		{
			return a switch
			{
				Accessibility.Public => "Public", 
				Accessibility.Internal => "Friend", 
				Accessibility.Private => "Private", 
				Accessibility.Protected => "Protected", 
				Accessibility.ProtectedOrInternal => "Protected Friend", 
				Accessibility.ProtectedAndInternal => "Private Protected", 
				_ => throw ExceptionUtilities.UnexpectedValue(a), 
			};
		}
	}
}
