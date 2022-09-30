using System.Collections.Immutable;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[StandardModule]
	internal sealed class AttributeDataExtensions
	{
		public static int IndexOfAttribute(this ImmutableArray<VisualBasicAttributeData> attributes, Symbol targetSymbol, AttributeDescription description)
		{
			int num = attributes.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				if (attributes[i].IsTargetAttribute(targetSymbol, description))
				{
					return i;
				}
			}
			return -1;
		}
	}
}
