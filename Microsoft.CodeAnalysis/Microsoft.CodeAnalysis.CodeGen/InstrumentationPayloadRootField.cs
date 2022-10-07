using System.Collections.Immutable;

using Microsoft.Cci;

namespace Microsoft.CodeAnalysis.CodeGen
{
    public sealed class InstrumentationPayloadRootField : SynthesizedStaticField
    {
        public override ImmutableArray<byte> MappedData => default;

        public InstrumentationPayloadRootField(INamedTypeDefinition containingType, int analysisIndex, ITypeReference payloadType)
            : base("PayloadRoot" + analysisIndex, containingType, payloadType)
        {
        }
    }
}
