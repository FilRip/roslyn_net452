using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.Debugging
{
    internal struct DynamicLocalInfo
    {
        public readonly ImmutableArray<bool> Flags;

        public readonly int SlotId;

        public readonly string LocalName;

        public DynamicLocalInfo(ImmutableArray<bool> flags, int slotId, string localName)
        {
            Flags = flags;
            SlotId = slotId;
            LocalName = localName;
        }
    }
}
