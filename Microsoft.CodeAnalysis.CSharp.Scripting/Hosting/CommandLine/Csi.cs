// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System;
using System.IO;

using Microsoft.CodeAnalysis.Scripting.Hosting;

namespace Microsoft.CodeAnalysis.CSharp.Scripting.Hosting
{
    public sealed class CSharpInteractiveCompiler : CSharpCompiler
    {
        public CSharpInteractiveCompiler(string responseFile, BuildPaths buildPaths, string[] args, IAnalyzerAssemblyLoader analyzerLoader)
            // Unlike C# compiler we do not use LIB environment variable. It's only supported for historical reasons.
            : base(CSharpCommandLineParser.Script, responseFile, args, buildPaths, null, analyzerLoader)
        {
        }

        public override Type Type => typeof(CSharpInteractiveCompiler);

        public override MetadataReferenceResolver GetCommandLineMetadataReferenceResolver(TouchedFileLogger loggerOpt)
        {
            return CommandLineRunner.GetMetadataReferenceResolver(Arguments, loggerOpt);
        }

        public override void PrintLogo(TextWriter consoleOutput)
        {
            consoleOutput.WriteLine(Properties.Resources.LogoLine1, GetCompilerVersion());
            consoleOutput.WriteLine(Properties.Resources.LogoLine2);
            consoleOutput.WriteLine();
        }

        public override void PrintHelp(TextWriter consoleOutput)
        {
            consoleOutput.Write(Properties.Resources.InteractiveHelp);
        }
    }
}
