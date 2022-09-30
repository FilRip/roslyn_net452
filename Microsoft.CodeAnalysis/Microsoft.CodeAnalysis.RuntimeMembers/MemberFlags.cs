using System;

namespace Microsoft.CodeAnalysis.RuntimeMembers
{
    [Flags()]
    public enum MemberFlags : byte
    {
        Method = 1,
        Field = 2,
        Constructor = 4,
        PropertyGet = 8,
        Property = 0x10,
        KindMask = 0x1F,
        Static = 0x20,
        Virtual = 0x40
    }
}
