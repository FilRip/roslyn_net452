// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.Diagnostics;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    /// <summary>
    /// Base class for logging compiler diagnostics.
    /// </summary>
    public abstract class ErrorLogger
    {
        public abstract void LogDiagnostic(Diagnostic diagnostic, SuppressionInfo? suppressionInfo);
    }
}
