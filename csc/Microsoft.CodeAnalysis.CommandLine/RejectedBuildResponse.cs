using System.IO;

namespace Microsoft.CodeAnalysis.CommandLine
{
	internal sealed class RejectedBuildResponse : BuildResponse
	{
		public string Reason;

		public override ResponseType Type => ResponseType.Rejected;

		public RejectedBuildResponse(string reason)
		{
			Reason = reason;
		}

		protected override void AddResponseBody(BinaryWriter writer)
		{
			BuildProtocolConstants.WriteLengthPrefixedString(writer, Reason);
		}

		public static RejectedBuildResponse Create(BinaryReader reader)
		{
			return new RejectedBuildResponse(BuildProtocolConstants.ReadLengthPrefixedString(reader));
		}
	}
}
