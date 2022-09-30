using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[StandardModule]
	internal sealed class MethodKindExtensions
	{
		internal static string TryGetAccessorDisplayName(this MethodKind kind)
		{
			return kind switch
			{
				MethodKind.EventAdd => SyntaxFacts.GetText(SyntaxKind.AddHandlerKeyword), 
				MethodKind.EventRaise => SyntaxFacts.GetText(SyntaxKind.RaiseEventKeyword), 
				MethodKind.EventRemove => SyntaxFacts.GetText(SyntaxKind.RemoveHandlerKeyword), 
				MethodKind.PropertyGet => SyntaxFacts.GetText(SyntaxKind.GetKeyword), 
				MethodKind.PropertySet => SyntaxFacts.GetText(SyntaxKind.SetKeyword), 
				_ => null, 
			};
		}
	}
}
