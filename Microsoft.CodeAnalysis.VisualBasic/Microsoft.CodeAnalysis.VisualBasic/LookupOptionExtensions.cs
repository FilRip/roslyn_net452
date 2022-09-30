using System;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	internal sealed class LookupOptionExtensions
	{
		internal const LookupOptions QueryOperatorLookupOptions = LookupOptions.MustBeInstance | LookupOptions.AllMethodsOfAnyArity | LookupOptions.MethodsOnly;

		internal const LookupOptions ConsiderationMask = LookupOptions.NamespacesOrTypesOnly | LookupOptions.LabelsOnly;

		internal static bool IsAttributeTypeLookup(this LookupOptions options)
		{
			return (options & LookupOptions.AttributeTypeOnly) == LookupOptions.AttributeTypeOnly;
		}

		internal static bool IsValid(this LookupOptions options)
		{
			LookupOptions lookupOptions = LookupOptions.MustBeInstance | LookupOptions.MustNotBeInstance;
			if ((options & lookupOptions) == lookupOptions)
			{
				return false;
			}
			return true;
		}

		internal static void ThrowIfInvalid(this LookupOptions options)
		{
			if (!IsValid(options))
			{
				throw new ArgumentException("LookupOptions has an invalid combination of options");
			}
		}

		internal static bool ShouldLookupExtensionMethods(this LookupOptions options)
		{
			return (options & (LookupOptions.AttributeTypeOnly | LookupOptions.MustNotBeInstance | LookupOptions.IgnoreExtensionMethods)) == 0;
		}
	}
}
