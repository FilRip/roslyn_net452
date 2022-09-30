using System;

namespace Microsoft.CodeAnalysis
{
    [Flags()]
    public enum NullableContextOptions
    {
        Disable = 0,
        Warnings = 1,
        Annotations = 2,
        Enable = 3
    }
}
