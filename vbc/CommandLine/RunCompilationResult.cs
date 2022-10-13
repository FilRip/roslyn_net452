// Decompiled with JetBrains decompiler
// Type: Microsoft.CodeAnalysis.CommandLine.RunCompilationResult
// Assembly: vbc, Version=3.11.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59BA59CE-D1C9-469A-AF98-699E22DB28ED
// Assembly location: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\vbc.exe

namespace Microsoft.CodeAnalysis.CommandLine
{
    internal readonly struct RunCompilationResult
    {
        internal static readonly RunCompilationResult Succeeded = new RunCompilationResult(0);
        internal static readonly RunCompilationResult Failed = new RunCompilationResult(1);

        internal int ExitCode { get; }

        internal bool RanOnServer { get; }

        internal RunCompilationResult(int exitCode, bool ranOnServer = false)
        {
            this.ExitCode = exitCode;
            this.RanOnServer = ranOnServer;
        }
    }
}
