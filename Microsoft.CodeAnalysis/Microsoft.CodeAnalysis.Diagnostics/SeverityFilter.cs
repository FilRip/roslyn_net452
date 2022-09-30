using System;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    [Flags()]
    public enum SeverityFilter
    {
        None = 0,
        Hidden = 1,
        Info = 0x10
    }
}
