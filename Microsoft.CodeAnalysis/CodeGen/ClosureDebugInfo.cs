// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CodeGen
{
    [DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
    public struct ClosureDebugInfo : IEquatable<ClosureDebugInfo>
    {
        public readonly int SyntaxOffset;
        public readonly DebugId ClosureId;

        public ClosureDebugInfo(int syntaxOffset, DebugId closureId)
        {
            SyntaxOffset = syntaxOffset;
            ClosureId = closureId;
        }

        public bool Equals(ClosureDebugInfo other)
        {
            return SyntaxOffset == other.SyntaxOffset &&
                   ClosureId.Equals(other.ClosureId);
        }

        public override bool Equals(object? obj)
        {
            return obj is ClosureDebugInfo info && Equals(info);
        }

        public override int GetHashCode()
        {
            return Hash.Combine(SyntaxOffset, ClosureId.GetHashCode());
        }

        internal string GetDebuggerDisplay()
        {
            return $"({ClosureId.GetDebuggerDisplay()} @{SyntaxOffset})";
        }
    }
}
