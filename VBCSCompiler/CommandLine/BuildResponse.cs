using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace Microsoft.CodeAnalysis.CommandLine
{
    internal abstract class BuildResponse
    {
        public abstract ResponseType Type { get; }

        public async Task WriteAsync(Stream outStream, CancellationToken cancellationToken)
        {
            using MemoryStream memoryStream = new();
            using BinaryWriter writer = new(memoryStream, Encoding.Unicode);
            writer.Write((int)this.Type);
            this.AddResponseBody(writer);
            writer.Flush();
            cancellationToken.ThrowIfCancellationRequested();
            int length = checked((int)memoryStream.Length);
            await outStream.WriteAsync(BitConverter.GetBytes(length), 0, 4, cancellationToken).ConfigureAwait(false);
            memoryStream.Position = 0L;
            await memoryStream.CopyToAsync(outStream, length, cancellationToken).ConfigureAwait(false);
        }

        protected abstract void AddResponseBody(BinaryWriter writer);

        public static async Task<BuildResponse> ReadAsync(
            Stream stream,
            CancellationToken cancellationToken = default)
        {
            byte[] lengthBuffer = new byte[4];
            await BuildProtocolConstants.ReadAllAsync(stream, lengthBuffer, 4, cancellationToken).ConfigureAwait(false);
            byte[] responseBuffer = new byte[(int)BitConverter.ToUInt32(lengthBuffer, 0)];
            await BuildProtocolConstants.ReadAllAsync(stream, responseBuffer, responseBuffer.Length, cancellationToken).ConfigureAwait(false);
            using BinaryReader reader = new(new MemoryStream(responseBuffer), Encoding.Unicode);
            return reader.ReadInt32() switch
            {
                0 => new MismatchedVersionBuildResponse(),
                1 => CompletedBuildResponse.Create(reader),
                2 => AnalyzerInconsistencyBuildResponse.Create(reader),
                3 => ShutdownBuildResponse.Create(reader),
                4 => RejectedBuildResponse.Create(reader),
                5 => new IncorrectHashBuildResponse(),
                _ => throw new InvalidOperationException("Received invalid response type from server."),
            };
        }

        public enum ResponseType
        {
            MismatchedVersion,
            Completed,
            AnalyzerInconsistency,
            Shutdown,
            Rejected,
            IncorrectHash,
        }
    }
}
