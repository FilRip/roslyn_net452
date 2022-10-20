using System.IO;

#nullable enable

namespace Microsoft.CodeAnalysis.CommandLine
{
    internal sealed class CompletedBuildResponse : BuildResponse
    {
        public readonly int ReturnCode;

        public readonly bool Utf8Output;

        public readonly string Output;

        public override ResponseType Type => ResponseType.Completed;

        public CompletedBuildResponse(int returnCode, bool utf8output, string? output)
        {
            ReturnCode = returnCode;
            Utf8Output = utf8output;
            Output = output ?? string.Empty;
        }

        public static CompletedBuildResponse Create(BinaryReader reader)
        {
            int returnCode = reader.ReadInt32();
            bool utf8output = reader.ReadBoolean();
            string output = BuildProtocolConstants.ReadLengthPrefixedString(reader);
            return new CompletedBuildResponse(returnCode, utf8output, output);
        }

        protected override void AddResponseBody(BinaryWriter writer)
        {
            writer.Write(ReturnCode);
            writer.Write(Utf8Output);
            BuildProtocolConstants.WriteLengthPrefixedString(writer, Output);
        }
    }
}
