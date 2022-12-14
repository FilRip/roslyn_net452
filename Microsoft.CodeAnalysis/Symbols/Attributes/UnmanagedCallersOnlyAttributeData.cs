// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public sealed class UnmanagedCallersOnlyAttributeData
    {
        public static readonly UnmanagedCallersOnlyAttributeData Uninitialized = new(callingConventionTypes: ImmutableHashSet<INamedTypeSymbolInternal>.Empty);
        public static readonly UnmanagedCallersOnlyAttributeData AttributePresentDataNotBound = new(callingConventionTypes: ImmutableHashSet<INamedTypeSymbolInternal>.Empty);
        private static readonly UnmanagedCallersOnlyAttributeData PlatformDefault = new(callingConventionTypes: ImmutableHashSet<INamedTypeSymbolInternal>.Empty);

        public const string CallConvsPropertyName = "CallConvs";

        public static UnmanagedCallersOnlyAttributeData Create(ImmutableHashSet<INamedTypeSymbolInternal>? callingConventionTypes)
            => callingConventionTypes switch
            {
                null or { IsEmpty: true } => PlatformDefault,
                _ => new UnmanagedCallersOnlyAttributeData(callingConventionTypes)
            };

        public readonly ImmutableHashSet<INamedTypeSymbolInternal> CallingConventionTypes;

        private UnmanagedCallersOnlyAttributeData(ImmutableHashSet<INamedTypeSymbolInternal> callingConventionTypes)
        {
            CallingConventionTypes = callingConventionTypes;
        }

        public static bool IsCallConvsTypedConstant(string key, bool isField, in TypedConstant value)
        {
            return isField
                   && key == CallConvsPropertyName
                   && value.Kind == TypedConstantKind.Array
                   && (value.Values.IsDefaultOrEmpty || value.Values.All(v => v.Kind == TypedConstantKind.Type));
        }
    }
}
