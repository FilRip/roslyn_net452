using System.Collections.Immutable;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[StandardModule]
	internal sealed class AnonymousTypeExtensions
	{
		internal static bool IsSubDescription(this ImmutableArray<AnonymousTypeField> fields)
		{
			return (object)fields.Last().Name == "Sub";
		}
	}
}
