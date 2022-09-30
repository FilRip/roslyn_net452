using System;

namespace Microsoft.CodeAnalysis
{
    [Flags()]
    public enum SymbolFilter
    {
        None = 0,
        Namespace = 1,
        Type = 2,
        Member = 4,
        TypeAndMember = 6,
        All = 7
    }
}
