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
#nullable restore
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
