using System.IO;

namespace Microsoft.CodeAnalysis.CommandLine
{
    internal sealed class ShutdownBuildResponse : BuildResponse
    {
        public readonly int ServerProcessId;

        public override ResponseType Type => ResponseType.Shutdown;

        public ShutdownBuildResponse(int serverProcessId)
        {
            ServerProcessId = serverProcessId;
        }

        protected override void AddResponseBody(BinaryWriter writer)
        {
            writer.Write(ServerProcessId);
        }

        public static ShutdownBuildResponse Create(BinaryReader reader)
        {
            return new ShutdownBuildResponse(reader.ReadInt32());
        }
    }
}
