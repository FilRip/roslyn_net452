// Decompiled with JetBrains decompiler
// Type: Microsoft.CodeAnalysis.CommandLine.BuildPathsAlt
// Assembly: vbc, Version=3.11.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59BA59CE-D1C9-469A-AF98-699E22DB28ED
// Assembly location: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\vbc.exe


#nullable enable
namespace Microsoft.CodeAnalysis.CommandLine
{
    internal sealed class BuildPathsAlt
    {
        internal string ClientDirectory { get; }

        internal string WorkingDirectory { get; }

        internal string? SdkDirectory { get; }

        internal string? TempDirectory { get; }

        internal BuildPathsAlt(string clientDir, string workingDir, string? sdkDir, string? tempDir)
        {
            this.ClientDirectory = clientDir;
            this.WorkingDirectory = workingDir;
            this.SdkDirectory = sdkDir;
            this.TempDirectory = tempDir;
        }
    }
}
