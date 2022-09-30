using System.Collections.Generic;
using System.Collections.Immutable;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SynthesizedAttributeData : SourceAttributeData
	{
		internal SynthesizedAttributeData(MethodSymbol wellKnownMember, ImmutableArray<TypedConstant> arguments, ImmutableArray<KeyValuePair<string, TypedConstant>> namedArgs)
			: base(null, wellKnownMember.ContainingType, wellKnownMember, arguments, namedArgs, isConditionallyOmitted: false, hasErrors: false)
		{
		}

		internal static SynthesizedAttributeData Create(MethodSymbol constructorSymbol, WellKnownMember constructor, ImmutableArray<TypedConstant> arguments = default(ImmutableArray<TypedConstant>), ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments = default(ImmutableArray<KeyValuePair<string, TypedConstant>>))
		{
			if (Binder.GetUseSiteInfoForWellKnownTypeMember(constructorSymbol, constructor, embedVBRuntimeUsed: false).DiagnosticInfo != null)
			{
				if (WellKnownMembers.IsSynthesizedAttributeOptional(constructor))
				{
					return null;
				}
				throw ExceptionUtilities.Unreachable;
			}
			if (arguments.IsDefault)
			{
				arguments = ImmutableArray<TypedConstant>.Empty;
			}
			if (namedArguments.IsDefault)
			{
				namedArguments = ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty;
			}
			return new SynthesizedAttributeData(constructorSymbol, arguments, namedArguments);
		}
	}
}
