// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public abstract class CompilationEvent
    {
        public CompilationEvent(Compilation compilation)
        {
            this.Compilation = compilation;
        }

        public Compilation Compilation { get; }
    }
}
