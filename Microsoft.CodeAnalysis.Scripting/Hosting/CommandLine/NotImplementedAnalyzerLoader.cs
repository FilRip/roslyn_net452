// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System;
using System.Reflection;

namespace Microsoft.CodeAnalysis.Scripting.Hosting
{
    public sealed class NotImplementedAnalyzerLoader : IAnalyzerAssemblyLoader
    {
        public void AddDependencyLocation(string fullPath)
        {
            throw new NotImplementedException();
        }

        public Assembly LoadFromPath(string fullPath)
        {
            throw new NotImplementedException();
        }
    }
}
