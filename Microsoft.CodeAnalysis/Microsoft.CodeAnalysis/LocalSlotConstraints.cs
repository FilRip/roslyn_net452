using System;

namespace Microsoft.CodeAnalysis
{
    [Flags()]
    public enum LocalSlotConstraints : byte
    {
        None = 0,
        ByRef = 1,
        Pinned = 2
    }
}
