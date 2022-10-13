// Decompiled with JetBrains decompiler
// Type: Microsoft.CodeAnalysis.VisualBasic.CommandLine.Vbc
// Assembly: vbc, Version=3.11.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59BA59CE-D1C9-469A-AF98-699E22DB28ED
// Assembly location: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\vbc.exe

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
            Vbc compiler = new Vbc(Path.Combine(buildPaths.ClientDirectory, "vbc.rsp"), buildPaths, args, analyzerLoader);
            return ConsoleUtil.RunWithUtf8Output<int>(compiler.Arguments.Utf8Output, textWriter, tw => compiler.Run(tw));
        }
    }
}
