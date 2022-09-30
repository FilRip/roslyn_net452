using Microsoft.Cci;

namespace Microsoft.CodeAnalysis.Emit
{
    internal struct EncHoistedLocalMetadata
    {
        public readonly string Name;

        public readonly ITypeReference Type;

        public readonly SynthesizedLocalKind SynthesizedKind;

        public EncHoistedLocalMetadata(string name, ITypeReference type, SynthesizedLocalKind synthesizedKind)
        {
            Name = name;
            Type = type;
            SynthesizedKind = synthesizedKind;
        }
    }
}
