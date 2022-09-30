using System.Collections.Immutable;

using Microsoft.Cci;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal readonly struct CallingConventionInfo
    {
        internal readonly CallingConvention CallKind;

        internal readonly ImmutableHashSet<CustomModifier>? UnmanagedCallingConventionTypes;

        public CallingConventionInfo(CallingConvention callKind, ImmutableHashSet<CustomModifier> unmanagedCallingConventionTypes)
        {
            CallKind = callKind;
            UnmanagedCallingConventionTypes = unmanagedCallingConventionTypes;
        }
    }
}
