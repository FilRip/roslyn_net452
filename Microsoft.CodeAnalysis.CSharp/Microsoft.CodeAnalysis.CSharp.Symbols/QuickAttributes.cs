using System;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    [Flags()]
    internal enum QuickAttributes : byte
    {
        None = 0,
        TypeIdentifier = 1,
        TypeForwardedTo = 2,
        AssemblyKeyName = 4,
        AssemblyKeyFile = 8,
        AssemblySignatureKey = 0x10
    }
}
