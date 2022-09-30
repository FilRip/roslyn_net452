using System;

namespace Roslyn.Utilities
{
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.GenericParameter)]
    internal sealed class NonDefaultableAttribute : Attribute
    {
    }
}
