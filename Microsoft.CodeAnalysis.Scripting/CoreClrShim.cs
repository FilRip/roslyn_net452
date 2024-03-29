// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System;
using System.Reflection;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis
{
    /// <summary>
    /// Shim for APIs available only on CoreCLR.
    /// </summary>
    internal static class CoreClrShim
    {
        internal static bool IsRunningOnCoreClr => AssemblyLoadContext.Type != null;

        internal static class AssemblyLoadContext
        {
            internal static readonly Type Type = ReflectionUtilities.TryGetType(
               "System.Runtime.Loader.AssemblyLoadContext, System.Runtime.Loader, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
        }
    }
}
