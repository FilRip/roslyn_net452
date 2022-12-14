// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CodeAnalysis.Emit
{
    public enum DebugInformationFormat
    {
        Pdb = 1,
        PortablePdb = 2,
        Embedded = 3,
    }

    public static partial class DebugInformationFormatExtensions
    {
        public static bool IsValid(this DebugInformationFormat value)
        {
            return value >= DebugInformationFormat.Pdb && value <= DebugInformationFormat.Embedded;
        }

        public static bool IsPortable(this DebugInformationFormat value)
        {
            return value == DebugInformationFormat.PortablePdb || value == DebugInformationFormat.Embedded;
        }
    }
}
