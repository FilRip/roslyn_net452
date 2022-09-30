using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public sealed class UnmanagedCallersOnlyAttributeData
    {
        public static readonly UnmanagedCallersOnlyAttributeData Uninitialized = new UnmanagedCallersOnlyAttributeData(ImmutableHashSet<INamedTypeSymbolInternal>.Empty);

        public static readonly UnmanagedCallersOnlyAttributeData AttributePresentDataNotBound = new UnmanagedCallersOnlyAttributeData(ImmutableHashSet<INamedTypeSymbolInternal>.Empty);

        private static readonly UnmanagedCallersOnlyAttributeData PlatformDefault = new UnmanagedCallersOnlyAttributeData(ImmutableHashSet<INamedTypeSymbolInternal>.Empty);

        public const string CallConvsPropertyName = "CallConvs";

        public readonly ImmutableHashSet<INamedTypeSymbolInternal> CallingConventionTypes;

        public static UnmanagedCallersOnlyAttributeData Create(ImmutableHashSet<INamedTypeSymbolInternal>? callingConventionTypes)
        {
            if (callingConventionTypes == null || callingConventionTypes!.IsEmpty)
            {
                return PlatformDefault;
            }
            return new UnmanagedCallersOnlyAttributeData(callingConventionTypes);
        }

        private UnmanagedCallersOnlyAttributeData(ImmutableHashSet<INamedTypeSymbolInternal> callingConventionTypes)
        {
            CallingConventionTypes = callingConventionTypes;
        }

        public static bool IsCallConvsTypedConstant(string key, bool isField, in TypedConstant value)
        {
            if (isField && key == "CallConvs" && value.Kind == TypedConstantKind.Array)
            {
                if (!value.Values.IsDefaultOrEmpty)
                {
                    return value.Values.All((TypedConstant v) => v.Kind == TypedConstantKind.Type);
                }
                return true;
            }
            return false;
        }
    }
}
