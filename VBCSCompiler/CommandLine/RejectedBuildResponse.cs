using System.IO;

#nullable enable

namespace Microsoft.CodeAnalysis.CommandLine
{
    internal sealed class RejectedBuildResponse : BuildResponse
    {
        public string Reason;

        public override ResponseType Type => ResponseType.Rejected;

        public RejectedBuildResponse(string reason) => this.Reason = reason;

        protected override void AddResponseBody(BinaryWriter writer) => BuildProtocolConstants.WriteLengthPrefixedString(writer, this.Reason);

#nullable restore
        public static RejectedBuildResponse Create(BinaryReader reader) => new(BuildProtocolConstants.ReadLengthPrefixedString(reader));
    }
}
