// Decompiled with JetBrains decompiler
// Type: Microsoft.CodeAnalysis.CommandLine.AnalyzerInconsistencyBuildResponse
// Assembly: vbc, Version=3.11.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59BA59CE-D1C9-469A-AF98-699E22DB28ED
// Assembly location: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\vbc.exe

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;


#nullable enable
namespace Microsoft.CodeAnalysis.CommandLine
{
    internal sealed class AnalyzerInconsistencyBuildResponse : BuildResponse
    {
        public override BuildResponse.ResponseType Type => BuildResponse.ResponseType.AnalyzerInconsistency;

        public ReadOnlyCollection<string> ErrorMessages { get; }

        public AnalyzerInconsistencyBuildResponse(ReadOnlyCollection<string> errorMessages) => this.ErrorMessages = errorMessages;

        protected override void AddResponseBody(BinaryWriter writer)
        {
            writer.Write(this.ErrorMessages.Count);
            foreach (string errorMessage in this.ErrorMessages)
                BuildProtocolConstants.WriteLengthPrefixedString(writer, errorMessage);
        }

        public static AnalyzerInconsistencyBuildResponse Create(
          BinaryReader reader)
        {
            int capacity = reader.ReadInt32();
            List<string> list = new List<string>(capacity);
            for (int index = 0; index < capacity; ++index)
                list.Add(BuildProtocolConstants.ReadLengthPrefixedString(reader) ?? "");
            return new AnalyzerInconsistencyBuildResponse(new ReadOnlyCollection<string>(list));
        }
    }
}
