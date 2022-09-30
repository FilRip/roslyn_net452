using System;

namespace Microsoft.CodeAnalysis
{
    [Flags()]
    public enum NullableContext
    {
        Disabled = 0,
        WarningsEnabled = 1,
        AnnotationsEnabled = 2,
        Enabled = 3,
        WarningsContextInherited = 4,
        AnnotationsContextInherited = 8,
        ContextInherited = 0xC
    }
}
