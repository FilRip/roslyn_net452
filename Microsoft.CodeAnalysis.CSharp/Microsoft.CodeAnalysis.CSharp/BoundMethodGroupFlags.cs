using System;

namespace Microsoft.CodeAnalysis.CSharp
{
    [Flags()]
    public enum BoundMethodGroupFlags
    {
        None = 0,
        SearchExtensionMethods = 1,
        HasImplicitReceiver = 2
    }
}
