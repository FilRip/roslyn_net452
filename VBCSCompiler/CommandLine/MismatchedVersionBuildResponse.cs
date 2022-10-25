using System.IO;


#nullable enable
namespace Microsoft.CodeAnalysis.CommandLine
{
    internal sealed class MismatchedVersionBuildResponse : BuildResponse
    {
        public override BuildResponse.ResponseType Type => BuildResponse.ResponseType.MismatchedVersion;

        protected override void AddResponseBody(BinaryWriter writer)
        {
        }
    }
}
