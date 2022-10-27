using System.IO;

#nullable enable

namespace Microsoft.CodeAnalysis.CommandLine
{
    internal sealed class IncorrectHashBuildResponse : BuildResponse
    {
        public override ResponseType Type => ResponseType.IncorrectHash;

        protected override void AddResponseBody(BinaryWriter writer)
        {
        }
    }
}
