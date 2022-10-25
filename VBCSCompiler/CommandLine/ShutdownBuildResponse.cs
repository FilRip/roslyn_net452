using System.IO;


#nullable enable
namespace Microsoft.CodeAnalysis.CommandLine
{
    internal sealed class ShutdownBuildResponse : BuildResponse
    {
        public readonly int ServerProcessId;

        public ShutdownBuildResponse(int serverProcessId) => this.ServerProcessId = serverProcessId;

        public override BuildResponse.ResponseType Type => BuildResponse.ResponseType.Shutdown;

        protected override void AddResponseBody(BinaryWriter writer) => writer.Write(this.ServerProcessId);

        public static ShutdownBuildResponse Create(BinaryReader reader) => new(reader.ReadInt32());
    }
}
