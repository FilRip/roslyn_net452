// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class CSharpScriptCompilationInfo : ScriptCompilationInfo
    {
        public new CSharpCompilation? PreviousScriptCompilation { get; }

        internal CSharpScriptCompilationInfo(CSharpCompilation? previousCompilationOpt, Type? returnType, Type? globalsType)
            : base(returnType, globalsType)
        {

            PreviousScriptCompilation = previousCompilationOpt;
        }

        public override Compilation? CommonPreviousScriptCompilation => PreviousScriptCompilation;

        public CSharpScriptCompilationInfo WithPreviousScriptCompilation(CSharpCompilation? compilation) =>
            (compilation == PreviousScriptCompilation) ? this : new CSharpScriptCompilationInfo(compilation, ReturnTypeOpt, GlobalsType);

        public override ScriptCompilationInfo CommonWithPreviousScriptCompilation(Compilation? compilation) =>
            WithPreviousScriptCompilation((CSharpCompilation?)compilation);
    }
}
