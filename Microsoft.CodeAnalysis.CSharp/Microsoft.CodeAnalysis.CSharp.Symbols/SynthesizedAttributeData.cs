using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public sealed class SynthesizedAttributeData : SourceAttributeData
    {
        public SynthesizedAttributeData(MethodSymbol wellKnownMember, ImmutableArray<TypedConstant> arguments, ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments)
            : base(null, wellKnownMember.ContainingType, wellKnownMember, arguments, default(ImmutableArray<int>), namedArguments, hasErrors: false, isConditionallyOmitted: false)
        {
        }

        public SynthesizedAttributeData(SourceAttributeData original)
            : base(original.ApplicationSyntaxReference, original.AttributeClass, original.AttributeConstructor, original.CommonConstructorArguments, original.ConstructorArgumentsSourceIndices, original.CommonNamedArguments, original.HasErrors, original.IsConditionallyOmitted)
        {
        }
    }
}
