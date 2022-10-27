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
            List<string> list = new(capacity);
            for (int index = 0; index < capacity; ++index)
                list.Add(BuildProtocolConstants.ReadLengthPrefixedString(reader) ?? "");
            return new AnalyzerInconsistencyBuildResponse(new ReadOnlyCollection<string>(list));
        }
    }
}
