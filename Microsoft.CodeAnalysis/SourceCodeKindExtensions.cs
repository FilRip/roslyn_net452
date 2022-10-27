// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CodeAnalysis
{
    public static partial class SourceCodeKindExtensions
    {
        public static SourceCodeKind MapSpecifiedToEffectiveKind(this SourceCodeKind kind)
        {
            return kind switch
            {
#pragma warning disable CS0618 // SourceCodeKind.Interactive is obsolete
                SourceCodeKind.Script or SourceCodeKind.Interactive => SourceCodeKind.Script,
#pragma warning restore CS0618 // SourceCodeKind.Interactive is obsolete
                _ => SourceCodeKind.Regular,
            };
        }

        public static bool IsValid(this SourceCodeKind value)
        {
            return value >= SourceCodeKind.Regular && value <= SourceCodeKind.Script;
        }
    }
}
