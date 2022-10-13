// Decompiled with JetBrains decompiler
// Type: Microsoft.CodeAnalysis.CommandLine.IncorrectHashBuildResponse
// Assembly: vbc, Version=3.11.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59BA59CE-D1C9-469A-AF98-699E22DB28ED
// Assembly location: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\vbc.exe

using System.IO;


#nullable enable
namespace Microsoft.CodeAnalysis.CommandLine
{
    internal sealed class IncorrectHashBuildResponse : BuildResponse
    {
        public override BuildResponse.ResponseType Type => BuildResponse.ResponseType.IncorrectHash;

        protected override void AddResponseBody(BinaryWriter writer)
        {
        }
    }
}
