using System;
using System.IO;

using Microsoft.CodeAnalysis.CommandLine;

#nullable enable

namespace Microsoft.CodeAnalysis.VisualBasic.CommandLine
{
    internal sealed class Vbc : VisualBasicCompiler
    {
        internal Vbc(

#nullable disable
            string responseFile,
            BuildPaths buildPaths,
            string[] args,
            IAnalyzerAssemblyLoader analyzerLoader)
            : base(VisualBasicCommandLineParser.Default, responseFile, args, buildPaths, Environment.GetEnvironmentVariable("LIB"), analyzerLoader)
        {
        }

        internal static int Run(
            string[] args,
            BuildPaths buildPaths,
            TextWriter textWriter,
            IAnalyzerAssemblyLoader analyzerLoader)
        {
            FatalError.Handler = new Action<Exception>(FailFast.OnFatalException);
            Vbc compiler = new(Path.Combine(buildPaths.ClientDirectory, "vbc.rsp"), buildPaths, args, analyzerLoader);
            return ConsoleUtil.RunWithUtf8Output<int>(compiler.Arguments.Utf8Output, textWriter, tw => compiler.Run(tw));
        }
    }
}
