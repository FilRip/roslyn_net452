using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[StandardModule]
	internal sealed class DeclarationModifiersExtensions
	{
		internal static Accessibility ToAccessibility(this DeclarationModifiers modifiers)
		{
			return (modifiers & DeclarationModifiers.AllAccessibilityModifiers) switch
			{
				DeclarationModifiers.Private => Accessibility.Private, 
				DeclarationModifiers.Public => Accessibility.Public, 
				DeclarationModifiers.Protected => Accessibility.Protected, 
				DeclarationModifiers.Friend => Accessibility.Internal, 
				DeclarationModifiers.Protected | DeclarationModifiers.Friend => Accessibility.ProtectedOrInternal, 
				DeclarationModifiers.Private | DeclarationModifiers.Protected => Accessibility.ProtectedAndInternal, 
				_ => throw ExceptionUtilities.UnexpectedValue(modifiers), 
			};
		}
	}
}
