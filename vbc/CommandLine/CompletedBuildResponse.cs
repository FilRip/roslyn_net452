// Decompiled with JetBrains decompiler
// Type: Microsoft.CodeAnalysis.CommandLine.CompletedBuildResponse
// Assembly: vbc, Version=3.11.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59BA59CE-D1C9-469A-AF98-699E22DB28ED
// Assembly location: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\vbc.exe

using System.IO;


#nullable enable
namespace Microsoft.CodeAnalysis.CommandLine
{
    internal sealed class CompletedBuildResponse : BuildResponse
    {
        public readonly int ReturnCode;
        public readonly bool Utf8Output;
        public readonly string Output;

        public CompletedBuildResponse(int returnCode, bool utf8output, string? output)
        {
            this.ReturnCode = returnCode;
            this.Utf8Output = utf8output;
            this.Output = output ?? string.Empty;
        }

        public override BuildResponse.ResponseType Type => BuildResponse.ResponseType.Completed;

        public static CompletedBuildResponse Create(BinaryReader reader)
        {
            int returnCode = reader.ReadInt32();
            bool flag = reader.ReadBoolean();
            string str = BuildProtocolConstants.ReadLengthPrefixedString(reader);
            int num = flag ? 1 : 0;
            string output = str;
            return new CompletedBuildResponse(returnCode, num != 0, output);
        }

        protected override void AddResponseBody(BinaryWriter writer)
        {
            writer.Write(this.ReturnCode);
            writer.Write(this.Utf8Output);
            BuildProtocolConstants.WriteLengthPrefixedString(writer, this.Output);
        }
    }
}
