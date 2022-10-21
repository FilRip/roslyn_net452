using System.IO;

#nullable enable

namespace Microsoft.CodeAnalysis.CommandLine
{
    internal sealed class RejectedBuildResponse : BuildResponse
    {
        public string Reason;

        public override BuildResponse.ResponseType Type => BuildResponse.ResponseType.Rejected;

        public RejectedBuildResponse(string reason) => this.Reason = reason;

        protected override void AddResponseBody(BinaryWriter writer) => BuildProtocolConstants.WriteLengthPrefixedString(writer, this.Reason);

        public static RejectedBuildResponse Create(BinaryReader reader) => new(BuildProtocolConstants.ReadLengthPrefixedString(reader));
    }
}
