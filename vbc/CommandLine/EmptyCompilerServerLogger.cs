// Decompiled with JetBrains decompiler
// Type: Microsoft.CodeAnalysis.CommandLine.EmptyCompilerServerLogger
// Assembly: vbc, Version=3.11.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59BA59CE-D1C9-469A-AF98-699E22DB28ED
// Assembly location: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\vbc.exe


#nullable enable
namespace Microsoft.CodeAnalysis.CommandLine
{
    internal sealed class EmptyCompilerServerLogger : ICompilerServerLogger
    {
        public static EmptyCompilerServerLogger Instance { get; } = new EmptyCompilerServerLogger();

        public bool IsLogging => false;

        private EmptyCompilerServerLogger()
        {
        }

        public void Log(string message)
        {
        }
    }
}
